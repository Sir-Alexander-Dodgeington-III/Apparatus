using DocumentFormat.OpenXml.Office.Word;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Deployment.Application;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace ZZZ_FRT_Lines
{
    public partial class massUpdateKitPrice : Form
    {
        public DataTable dtListofKits = new DataTable();
        public DataTable dtPriceUpdate = new DataTable();
        //public DataTable dataTable = new DataTable();
        public decimal sumForB;
        public decimal sumForB2;
        public decimal baseCost;
        public decimal basePrice;
        public decimal kitCost;
        public decimal kitSell;
        public decimal oldKitSell;
        public bool errorStatus = false;
        public string part;
        public int processedKits = 0;
        public int progressPercentage;
        public decimal margin;


        public massUpdateKitPrice()
        {
            InitializeComponent();
            this.CenterToParent();
            this.Shown += massUpdateKitPrice_shown;
        }

        private void PerformTask()
        {
            progressBar1.Minimum = 0;
            progressBar1.Maximum = 100;
            progressBar1.Value = 0;
            loadDistinctKits();
            if (!this.IsDisposed && !this.Disposing)
            {
                updateComplete();
            }
        }

            private void massUpdateKitPrice_shown(object sender, EventArgs e)
        {
            // Start the task here
            Task.Run(() => PerformTask());
        }

        public void loadDistinctKits()
        {
            SqlConnection connectionString = new SqlConnection(Helper.ConnString("AUTOPART"));
            connectionString.Open();
            SqlDataAdapter adptr1 = new SqlDataAdapter(
            $@"
                SELECT DISTINCT KitSKU FROM MERI_Kit_Upgrade ORDER BY KitSKU
            "
            , connectionString);
            adptr1.Fill(dtListofKits);

            beginPriceUpdate();
            
        }

        public void beginPriceUpdate()
        {
            int totalKits = dtListofKits.Rows.Count;
            foreach (DataRow row in dtListofKits.Rows)
            {
                part = row["KitSKU"].ToString();
                SqlConnection connectionString = new SqlConnection(Helper.ConnString("AUTOPART"));
                connectionString.Open();
                SqlDataAdapter adptr1 = new SqlDataAdapter(
                $@"
                    DECLARE @part VARCHAR(100)
                    SET @part = '{part}'

                    ;WITH CTE (KitSKU, Prompt, PromptID, PromptType, InputType, ComponentSKU, ComponentQty, ComponentPrice, UpgradePrice, OptionSort, ComponentType, ComponentCost, ComponentSell, KitSell, Margin) AS (
                        SELECT I.KitSKU, I.Prompt, I.PromptID, I.PromptType, I.InputType, I.ComponentSKU, I.ComponentQty, I.ComponentPrice, I.UpgradePrice, I.OptionSort, I.ComponentType,
                               P.A1 AS ComponentCost, P2.A1 AS ComponentSell, P3.A1 AS KitSell, I.Margin
                        FROM MERI_Kit_Upgrade AS I
                        INNER JOIN Mvpr AS P ON I.ComponentSKU = P.SubKey1 AND P.Prefix = 'S' AND P.SubKey2 = 27
                        INNER JOIN Mvpr AS P2 ON I.ComponentSKU = P2.SubKey1 AND P2.Prefix = 'S' AND P2.SubKey2 = 2
                        INNER JOIN Mvpr AS P3 ON I.KitSKU = P3.SubKey1 AND P3.Prefix = 'S' AND P3.SubKey2 = 2
                    ),

                    CTE1 (KeyCode, word5, Description, Make, Model, StartYear, EndYear, Level1, Level2, Level3, ImageURL) AS (
                        SELECT P.KeyCode, P.word5, P.[Desc], K.Make, K.Model, K.StartYear, K.EndYear, K.Level1, K.Level2, K.Level3, K.ImageURL
                        FROM Product P 
                        LEFT JOIN MERI_KIT_INFO K ON K.InternalSKU = P.KeyCode
                    ),

                    CTE2 (ComponentSKU, LongDesc, SimplePart, Brand, MFR_SKU) AS (
                        SELECT P.KeyCode, C.Display_Desc, P.Word5, C.Brand, C.MFR_SKU
                        FROM Product P 
                        LEFT JOIN MERI_Kit_Component C ON P.KeyCode = C.ComponentSKU
                    )

                    SELECT I.KitSKU, I.ComponentSKU, III.MFR_SKU, III.LongDesc, I.ComponentQty, I.OptionSort, I.Prompt, I.PromptID, I.PromptType, I.ComponentType, I.InputType,
                           ISNULL(I.ComponentPrice, '0.00') AS ComponentPrice, ISNULL(CAST(I.UpgradePrice AS DECIMAL(18,2)), '0.00') AS UpgradePrice,
                           III.SimplePart AS word5, II.Description, II.Make, II.Model, II.StartYear, II.EndYear, II.Level1, II.Level2, II.Level3, II.ImageURL,
                           I.KitSell AS KitPrice, I.ComponentSell AS ComponentA2, I.ComponentCost, III.Brand, I.Margin
                    FROM CTE I
                    INNER JOIN CTE1 II ON I.KitSKU = II.KeyCode
                    INNER JOIN CTE2 III ON I.ComponentSKU = III.ComponentSKU
                    WHERE I.KitSKU = @part OR II.word5 = @part
                    ORDER BY I.PromptID, I.OptionSort ASC
            "
                , connectionString);
                adptr1.Fill(dtPriceUpdate);

                CalculatePrices();
                finalize();
                part = "";
                dtPriceUpdate = new DataTable();
                kitCost = 0;
                kitSell = 0;
                baseCost = 0;
                basePrice = 0;
                sumForB = 0;
                sumForB2 = 0;
                processedKits++;
                progressPercentage = (int)((double)processedKits / totalKits * 100);
                UpdateProgressBar(progressPercentage);
            }

        }

        public void UpdateProgressBar(int value)
        {
            if (progressBar1.InvokeRequired)
            {
                progressBar1.Invoke(new Action<int>(UpdateProgressBar), new object[] { value });
            }
            else
            {
                label2.Text = $"{processedKits} / {dtListofKits.Rows.Count}";
                progressBar1.Value = value;
                progressBar1.Text = progressPercentage.ToString();
                progressBar1.Refresh();
            }
        }

        private void CalculatePrices()
            {
                if (dtPriceUpdate.Rows.Count > 0)
                {
                    identifyBs();

                    //dataGridView2.ClearSelection();

                    sumForB = 0;
                    sumForB2 = 0;
                    //decimal desiredSum = Convert.ToDecimal(sellPriceTextbox.Text);
                    //decimal KitCost = Convert.ToDecimal(textbox1.Text);
                    //decimal sellPrice = Convert.ToDecimal(sellPriceTextbox.Text);
                    decimal percent = 0;


                    foreach (DataRow row in dtPriceUpdate.Rows)
                    {
                        if (row["ComponentType"].ToString() == "B")
                        {
                            sumForB += Convert.ToDecimal(row["ComponentA2"])/* * Convert.ToInt32(row["ComponentQty"])*/;
                            sumForB2 += Convert.ToDecimal(row["ComponentCost"]) * Convert.ToInt32(row["ComponentQty"]);
                        }
                    }
                    oldKitSell = Convert.ToDecimal(dtPriceUpdate.Rows[0]["KitPrice"].ToString());
                    kitCost = Math.Round(sumForB2, 2);
                    margin = Convert.ToDecimal(dtPriceUpdate.Rows[0]["Margin"].ToString());
                    margin = margin / 100;
                    margin = 1 - margin;
                    kitSell = Math.Round(sumForB2 / (margin),2);

                    if (oldKitSell == kitSell)
                    {
                        return;
                    }

                    foreach (DataRow row in dtPriceUpdate.Rows)
                        {
                            if (row["ComponentType"].ToString() == "B")
                            {
                                bool isLastBRow = row == dtPriceUpdate.AsEnumerable()
                                                        .LastOrDefault(r => r["ComponentType"].ToString() == "B");

                                if (!isLastBRow)
                                {
                                    if (row["ComponentType"].ToString() == "B")
                                    {
                                        percent = kitSell / kitCost;
                                        //sumForB += Convert.ToDecimal(row["ComponentA2"]) * Convert.ToInt32(row["ComponentQty"]);
                                    }

                                    decimal a2Value = Convert.ToDecimal(row["ComponentA2"]) * Convert.ToInt32(row["ComponentQty"]);
                                    decimal percentage = (a2Value / sumForB);
                                    int Qty = Convert.ToInt32(row["ComponentQty"]);

                                    row["ComponentPrice"] = Math.Round((Convert.ToDecimal(row["ComponentCost"]) * percent), 2);
                                }
                                else
                                {
                                    foreach (DataRow row1 in dtPriceUpdate.Rows)
                                    {
                                        int Qty = Convert.ToInt32(row1["ComponentQty"]);
                                        if (row1["ComponentType"].ToString() == "B")
                                        {
                                            bool isLastBRow1 = row1 == dtPriceUpdate.AsEnumerable()
                                                                    .LastOrDefault(r => r["ComponentType"].ToString() == "B");

                                            if (!isLastBRow1)
                                            {
                                                if (row1["ComponentType"].ToString() == "B")
                                                {
                                                    sumForB2 += Convert.ToDecimal(row1["ComponentPrice"]) * Qty;
                                                }
                                            }
                                        }
                                    }
                                    row["ComponentPrice"] = Math.Round((kitSell - sumForB2) / Convert.ToInt32(row["ComponentQty"]), 2);
                                }
                            }
                    }

                    foreach (DataRow row in dtPriceUpdate.Rows)
                    {
                        if (row["ComponentType"].ToString() == "A")
                        {
                            row["ComponentPrice"] = Math.Round(Convert.ToDecimal(row["componenta2"]), 2);
                        }
                    }


                    var commonPrices = new Dictionary<string, decimal>();

                    foreach (DataRow row in dtPriceUpdate.Rows)
                    {
                        string prompt = row["Prompt"].ToString();
                        int optionSort = Convert.ToInt32(row["OptionSort"]);
                        decimal componentPrice = Convert.ToDecimal(row["ComponentPrice"]);
                        string componentType = row["ComponentType"].ToString();

                        if (optionSort == 1 && componentType == "B")
                        {
                            commonPrices[prompt] = componentPrice;
                        }
                    }

                    // Second pass: Set ComponentPrice based on collected common prices
                    foreach (DataRow row in dtPriceUpdate.Rows)
                    {
                        string prompt = row["Prompt"].ToString();
                        int optionSort = Convert.ToInt32(row["OptionSort"]);
                        string componentType = row["ComponentType"].ToString();

                        if (componentType == "O" && optionSort != 1 && commonPrices.ContainsKey(prompt))
                        {
                            row["ComponentPrice"] = commonPrices[prompt];
                        }
                    }


                    foreach (DataRow row in dtPriceUpdate.Rows)
                    {
                        string prompt = row["Prompt"].ToString();
                        int optionSort = Convert.ToInt32(row["OptionSort"]);
                        //decimal basePrice = Convert.ToDecimal(row["ComponentCost"]);
                        decimal componentCost = Convert.ToDecimal(row["ComponentCost"]);
                        string componentType = row["ComponentType"].ToString();
                        string ComponentSKU = row["componentSKU"].ToString();

                        /*if (ComponentSKU == "SLV1131H.STD")
                        {
                            MessageBox.Show("");
                        }*/

                        if (optionSort == 1 && componentType == "B")
                        {
                            baseCost = Convert.ToDecimal(row["ComponentCost"].ToString());
                            basePrice = Convert.ToDecimal(row["ComponentPrice"].ToString());
                        }
                        else
                        {
                            if (componentType == "U" && optionSort != 1)
                            {
                                SqlConnection conn = new SqlConnection(Helper.ConnString("AUTOPART"));

                                string query = $@"SELECT MUMin, MUPercent 
                                FROM MERI_KitMarkups 
                                WHERE [Type] IN
                                (
                                SELECT Value 
                                FROM ProdVals 
                                WHERE Attribute = 'Kit Category' AND Part = '{ComponentSKU}'
                                )";

                                using (SqlCommand command = new SqlCommand(query, conn))
                                {
                                    try
                                    {
                                        conn.Open();
                                        SqlDataReader reader = command.ExecuteReader();
                                        reader.Read();

                                        if (reader.HasRows)
                                        {
                                            decimal MUMin = Convert.ToDecimal(reader.GetDecimal(0)) / Convert.ToInt32(row["ComponentQty"].ToString());
                                            decimal MUPercent = Convert.ToDecimal(reader.GetDecimal(1));

                                            //decimal basePlusMin = MUMin;

                                            decimal percMarkup;

                                            if (baseCost > componentCost)
                                            {
                                                percMarkup = Math.Round(baseCost * MUPercent, 2);
                                            }
                                            else
                                            {
                                                percMarkup = Math.Round((componentCost - baseCost) * MUPercent, 2);
                                            }



                                            if (MUMin > percMarkup)
                                            {
                                                row["ComponentPrice"] = Math.Round(basePrice + componentCost - baseCost + MUMin, 2);
                                            }
                                            else
                                            {
                                                row["ComponentPrice"] = Math.Round(basePrice + componentCost - baseCost + percMarkup, 2);
                                            }
                                            decimal upgradePrice = Convert.ToDecimal(row["ComponentPrice"]);
                                            row["UpgradePrice"] = Convert.ToDecimal(Math.Round(upgradePrice - basePrice, 2));
                                            upgradePrice = Convert.ToDecimal(Math.Round(upgradePrice - basePrice, 2));
                                            conn.Close();
                                            reader.Close();
                                        }
                                        else
                                        {
                                            if (conn.State == ConnectionState.Open)
                                            {
                                                conn.Close();
                                            }
                                            string kitCategory = Interaction.InputBox($@"Kit {part} can not be priced because part number {ComponentSKU} does not have a kit category. Please specify a kit category", "enter a kit category", "MIS");

                                            string query1 = $@"SELECT MUMin, MUPercent 
                                                        FROM MERI_KitMarkups 
                                                        WHERE [Type] = '{kitCategory}'";

                                        //using (SqlConnection conn2 = new SqlConnection(Helper.ConnString("AUTOPART")))
                                        //{
                                            SqlCommand command1 = new SqlCommand(query1, conn);

                                            try
                                            {
                                                conn.Open();
                                                SqlDataReader reader1 = command1.ExecuteReader();
                                                reader1.Read();

                                                if (reader1.HasRows)
                                                {
                                                    //SqlConnection conn = new SqlConnection(Helper.ConnString("AUTOPART"));

                                                    using (SqlCommand cmd = new SqlCommand("sp_MERI_InsertKitComponentIntoProdVals", conn))
                                                    {
                                                        cmd.CommandType = CommandType.StoredProcedure;

                                                        cmd.Parameters.Add("@PartNo", SqlDbType.VarChar).Value = ComponentSKU;

                                                        cmd.Parameters.Add("@Attribute", SqlDbType.VarChar).Value = "Kit Category";

                                                        cmd.Parameters.Add("@Value", SqlDbType.VarChar).Value = kitCategory;

                                                        cmd.Parameters.Add("@Presentation", SqlDbType.VarChar).Value = "T";

                                                        cmd.Parameters.Add("@ListOfValues", SqlDbType.VarChar).Value = "";

                                                        try
                                                        {
                                                            //conn1.Open();
                                                            cmd.ExecuteNonQuery();
                                                        }
                                                        catch (Exception ex)
                                                        {
                                                            MessageBox.Show(ex.Message, "Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                                            return;
                                                        }
                                                        finally
                                                        {
                                                            cmd.Parameters.Clear();
                                                            conn.Close();
                                                        }
                                                    }

                                                    decimal MUMin = Convert.ToDecimal(reader.GetDecimal(0)) / Convert.ToInt32(row["ComponentQty"].ToString());
                                                    decimal MUPercent = Convert.ToDecimal(reader1.GetDecimal(1));

                                                    //decimal basePlusMin = MUMin;
                                                    decimal percMarkup = Math.Round((componentCost - baseCost) * MUPercent, 2);

                                                    if (MUMin > percMarkup)
                                                    {
                                                        row["ComponentPrice"] = Math.Round(basePrice + componentCost - baseCost + MUMin, 2);
                                                    }
                                                    else
                                                    {
                                                        row["ComponentPrice"] = Math.Round(basePrice + componentCost - baseCost + percMarkup, 2);
                                                    }
                                                    decimal upgradePrice = Convert.ToDecimal(row["ComponentPrice"]);
                                                    row["UpgradePrice"] = Convert.ToDecimal(Math.Round(upgradePrice - basePrice, 2));
                                                    //upgradePrice = Convert.ToDecimal(Math.Round(upgradePrice - basePrice, 2));
                                                    if (conn.State == ConnectionState.Open)
                                                    {
                                                        conn.Close();
                                                    }
                                                reader1.Close();
                                                }
                                                else
                                                {
                                                    MessageBox.Show($@"KitCategory {kitCategory} is not a valid option.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                                    errorStatus = true;
                                                    return;
                                                }
                                            }
                                            catch (Exception ex)
                                            {
                                                MessageBox.Show("Error: " + ex.Message);
                                                if (conn.State == ConnectionState.Open)
                                                {
                                                    conn.Close();
                                                }
                                            }
                                            finally
                                            {
                                                if (conn.State == ConnectionState.Open)
                                                {
                                                    conn.Close();
                                                }

                                            }
                                        }
                                    }
                                    
                                    catch (Exception ex)
                                    {
                                        MessageBox.Show("Error: " + ex.Message);
                                        conn.Close();
                                    }

                                }
                            }
                            else
                            {
                                row["UpgradePrice"] = Convert.ToDecimal(Math.Round(0.00M, 2));
                            }
                        }
                    }

                    /*foreach (DataRow row2 in dt.Rows)
                    {
                        if (Convert.ToDouble(row2["UpgradePrice"]) < 10.00 && row2["ComponentType"].ToString() == "U")
                        {
                            row2["UpgradePrice"] = Convert.ToDecimal("10.00");
                        }
                    }*/
                }
            }

        private void identifyBs()
        {
            //renumberPrompts();
            foreach (DataRow row in dtPriceUpdate.Rows)
            {
                int optionSort = Convert.ToInt32(row["OptionSort"]);

                if (optionSort == 1)
                {
                    // Set the value of "ComponentType" to "B" for rows where "OptionSort" is equal to 1
                    if (row["PromptType"].ToString() != "A")
                    {
                        row["ComponentType"] = "B";
                    }
                }

                if (row["PromptType"].ToString() == "A")
                {
                    // Set the value of "ComponentType" to "B" for rows where "OptionSort" is equal to 1
                    row["ComponentType"] = row["PromptType"];
                }

                if (optionSort != 1 && Convert.ToString(row["ComponentType"]) == "B")
                {
                    row["ComponentType"] = row["PromptType"];
                }
            }

            sumForB = 0;
            foreach (DataRow row in dtPriceUpdate.Rows)
            {
                if (row["ComponentType"].ToString() == "B")
                {
                    decimal componentCost = Convert.ToDecimal(row["ComponentQty"]) * Convert.ToDecimal(row["ComponentCost"]);
                    sumForB += componentCost;
                }
            }

            //textbox1.Text = Math.Round(sumForB, 2).ToString();

        }

        public void finalize()
        {
            SqlConnection conn = new SqlConnection(Helper.ConnString("AUTOPART"));

            using (SqlCommand cmd = new SqlCommand("sp_MERI_deleteKitfromApparatus", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add("@KitSKU", SqlDbType.VarChar).Value = part;

                //decimal upgradePrice = Convert.ToDecimal(dt.Rows[0]["UpgradePrice"].ToString());
                //decimal componentPrice = Convert.ToDecimal(dt.Rows[0]["UpgradePrice"].ToString());

                try
                {
                    conn.Open();
                    cmd.CommandTimeout = 999999;
                    cmd.ExecuteNonQuery();
                    conn.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }

            foreach (DataRow row in dtPriceUpdate.Rows)
            {
                using (SqlCommand cmd = new SqlCommand("sp_MERI_updateKitfromApparatus", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.Add("@KitSKU", SqlDbType.VarChar).Value = part;

                    cmd.Parameters.Add("@ComponentSKU", SqlDbType.VarChar).Value = row["ComponentSKU"].ToString();

                    cmd.Parameters.Add("@PromptID", SqlDbType.Int).Value = Convert.ToInt32(row["PromptID"]);

                    cmd.Parameters.Add("@OptionSort", SqlDbType.Int).Value = Convert.ToInt32(row["OptionSort"]);

                    cmd.Parameters.Add("@Prompt", SqlDbType.VarChar).Value = row["Prompt"].ToString();

                    cmd.Parameters.Add("@PromptType", SqlDbType.VarChar).Value = row["PromptType"].ToString();

                    cmd.Parameters.Add("@ComponentQty", SqlDbType.Int).Value = Convert.ToInt32(row["ComponentQty"]);

                    cmd.Parameters.Add("@ComponentType", SqlDbType.VarChar).Value = row["ComponentType"].ToString();

                    cmd.Parameters.Add("@UpgradePrice", SqlDbType.Money).Value = Convert.ToDecimal(row["UpgradePrice"].ToString());

                    cmd.Parameters.Add("@ComponentPrice", SqlDbType.Money).Value = Convert.ToDecimal(row["componentPrice"].ToString());

                    cmd.Parameters.Add("@ComponentURL", SqlDbType.VarChar).Value = row["ImageURL"].ToString();

                    cmd.Parameters.Add("@InputType", SqlDbType.VarChar).Value = row["InputType"].ToString();

                    cmd.Parameters.Add("@Margin", SqlDbType.VarChar).Value = row["Margin"].ToString();


                    try
                    {
                        conn.Open();
                        cmd.ExecuteNonQuery();
                        conn.Close();
                        cmd.Parameters.Clear();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message, "Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                }

            }

            using (SqlCommand cmd = new SqlCommand("sp_MERI_updateKitSellinMVPRfromApparatus", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add("@KitSKU", SqlDbType.VarChar).Value = part;

                cmd.Parameters.Add("@KitPrice", SqlDbType.Money).Value = Convert.ToDecimal(kitSell);

                try
                {
                    conn.Open();
                    cmd.ExecuteNonQuery();
                    conn.Close();
                    cmd.Parameters.Clear();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }

            using (SqlCommand cmd = new SqlCommand("sp_MERI_InsertIntoKitUpdatefromApparatus", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add("@KitSKU", SqlDbType.VarChar).Value = part;

                try
                {
                    conn.Open();
                    cmd.ExecuteNonQuery();
                    conn.Close();
                    cmd.Parameters.Clear();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }
        }

        public void updateComplete()
        {
            if (this.IsDisposed || this.Disposing) return;

            MessageBox.Show("Update Complete");
            this.BeginInvoke((MethodInvoker)delegate { this.Close(); });
        }
    }
}
