using System;
using System.Drawing;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;


namespace QuoteCreator
{
    public partial class QuoteCreator : Form
    {
        public string windowsUser = System.Security.Principal.WindowsIdentity.GetCurrent().Name.Replace(@"MERRILLCO\", "");
        private TextBox CurrentTextbox = null;
        public DataTable dt = new DataTable();
        public DataTable dt1 = new DataTable();
        public DataTable dt2 = new DataTable();
        public DataTable dt3 = new DataTable();
        public DataTable dt4 = new DataTable();
        public DataTable newPart = new DataTable();
        public DataTable dtp = new DataTable();
        public DataTable dtp1 = new DataTable();
        public string stringPG;
        public string stringRange;
        public string calcType;
        public decimal calcValue;
        public decimal P10;
        public string UPrice;
        public string quoteNumber;
        public decimal sum = 0;
        public string sumString;
        public string VCode;
        public string description;
        public double corePrice;
        public string PGResult;
        public string rangeResult;
        public string supplierResult;
        public string input;
        public double KitPrice;
        public string costResult;
        public double oldCost;
        public double newCost;
        public double perc;
        public double min;
        public double diff;
        public bool found = true;
        //public event System.Windows.Forms.KeyEventHandler KeyDown;

        public QuoteCreator()
        {
            InitializeComponent();
            //int h = Screen.PrimaryScreen.WorkingArea.Height;
            //int w = Screen.PrimaryScreen.WorkingArea.Width;
            //this.ClientSize = new Size(w, h);
            this.CenterToScreen();


            operatorTextbox.Text = windowsUser.Substring(0, 5).ToUpper();
            customerNumberTextbox.Select();

            foreach (DataGridViewColumn col in dataGridView3.Columns)
            {
                col.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            }
        }

        private void colorActiveTextbox_Leave(object sender, EventArgs e)
        {
            CurrentTextbox = (TextBox)sender;
            CurrentTextbox.BackColor = Color.Empty;
        }

        private void colorActiveTextbox_Enter(object sender, EventArgs e)
        {
            CurrentTextbox = (TextBox)sender;
            CurrentTextbox.BackColor = Color.Yellow;
        }

        private void quitButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void quitButton2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void customerNumberTextbox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Enter)
            {
                SqlConnection conn = new SqlConnection(Helper.ConnString("AUTOPART"));
                SqlDataAdapter adapter = new SqlDataAdapter();
                SqlCommand cmd = conn.CreateCommand();
                cmd.CommandText = $@"
                                SELECT 
                                KeyCode, 
                                Name, 
                                Addra, 
                                Addre, 
                                PCode, 
                                Stel, 
                                CAST(Climit as decimal(18,2)) as Climit, 
                                Rep, 
                                PType, 
                                LType, 
                                Email, 
                                MotDueSort,
                                SCont,
                                Fax,
                                ACont,
                                CAST(Balances as decimal(18,2)) as Balances
                                FROM Customer 
                                WHERE KeyCode = '{customerNumberTextbox.Text}'
                                ";
                adapter.SelectCommand = cmd;
                conn.Open();
                adapter.Fill(dt);
                conn.Close();

                if (dt.Rows.Count == 0)
                {
                    MessageBox.Show("Invalid Customer Number");
                }
                else
                {
                    addressTextbox.Text = dt.Rows[0]["Addra"].ToString();
                    nameTaxtbox.Text = dt.Rows[0]["Name"].ToString();
                    cityTextbox.Text = dt.Rows[0]["Addre"].ToString();
                    addressTextbox.Text = dt.Rows[0]["Addra"].ToString();
                    stateTextbox.Text = dt.Rows[0]["MotDueSort"].ToString();
                    zipCodeTextbox.Text = dt.Rows[0]["PCode"].ToString();
                    accountTypeTextbox.Text = dt.Rows[0]["LType"].ToString();
                    docTypeTextbox.Text = dt.Rows[0]["PType"].ToString();
                    salesContactTextbox.Text = dt.Rows[0]["SCont"].ToString();
                    phoneTextbox.Text = dt.Rows[0]["Stel"].ToString();
                    faxTextbox.Text = dt.Rows[0]["Fax"].ToString();
                    accountContactTextbox.Text = dt.Rows[0]["ACont"].ToString();
                    climitTextbox.Text = dt.Rows[0]["Climit"].ToString();
                    repCodeTextbox.Text = dt.Rows[0]["Rep"].ToString();
                    salesRepCodeTextbox.Text = dt.Rows[0]["Rep"].ToString();
                    emailTextbox.Text = dt.Rows[0]["Email"].ToString();
                    balanceValueTextbox.Text = dt.Rows[0]["Balances"].ToString();
                    customerTextbox2.Text = "Customer       " + dt.Rows[0]["Name"].ToString() + " (" + customerNumberTextbox.Text + ")";
                    balanceAmountTextbox.Text = "Bal $" + dt.Rows[0]["Balances"].ToString();

                    if (Convert.ToDecimal(dt.Rows[0]["Balances"]) > Convert.ToDecimal(dt.Rows[0]["Climit"]))
                    {
                        balanceValueTextbox.BackColor = Color.Red;
                    }
                    else
                    {
                        balanceValueTextbox.BackColor = Color.OliveDrab;
                    }
                }
            }


            if (e.KeyData == Keys.F5)
            {
                MessageBox.Show("F5 pressed");
            }

            e.Handled = true;
            dt.Clear();
        }

        private void kitNumberTextbox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Tab || e.KeyData == Keys.Enter)
            {
                SqlConnection conn = new SqlConnection(Helper.ConnString("AUTOPART"));
                SqlDataAdapter adapter = new SqlDataAdapter();
                SqlCommand cmd = conn.CreateCommand();
                cmd.CommandText = $@"
                    SELECT * FROM vw_MERI_NAP_Export 
                    WHERE [Internal SKU] = 'KITEK1070' AND Type = 'B' 
                    ORDER BY LEN([Option ID]), [Option ID]
                                ";
                adapter.SelectCommand = cmd;
                conn.Open();
                adapter.Fill(dt1);
                conn.Close();

                if (dt1.Rows.Count == 0)
                {
                    partnumberSearch();
                }
                else
                {
                    kitDescriptionTextbox.Text = dt1.Rows[0]["SKU Description"].ToString();

                    listBox4.Items.Add(kitNumberTextbox.Text);

                    dataGridView1.DataSource = dt1;

                    UpdatePricing();

                }
            }


            if (e.KeyData == Keys.F10)
            {
                MessageBox.Show("F10 pressed");
            }

            e.Handled = true;
            dt.Clear();
        }

        private void UpdatePricing()
        { 
        SqlConnection conn = new SqlConnection(Helper.ConnString("AUTOPART"));
        SqlDataAdapter adapter = new SqlDataAdapter();

            string kit = kitNumberTextbox.Text;
            int i = 1;

            conn.Open();
            SqlCommand cmd = new SqlCommand($@"SELECT A1 FROM MVPR WHERE SubKey1 = '{kit}' AND Prefix = 'S' AND SubKey2 = '4'", conn);
            double result = Convert.ToDouble(cmd.ExecuteScalar());
            conn.Close();

            kitPriceTextbox.Text = result.ToString("0.00");
            double result2 = 0;

            for (int y = 0; y < dataGridView1.Rows.Count; y++)
            {
                if (Convert.ToString(dataGridView1.Rows[y].Cells["Opt"].Value) == "D")
                {
                    continue;
                }
                result2 += Math.Round(Convert.ToDouble(dataGridView1.Rows[y].Cells["Cost"].Value) * Convert.ToInt32(dataGridView1.Rows[y].Cells["Qty"].Value),2);
            }
            kitCostTextbox.Text = result2.ToString("0.00");

            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                if (Convert.ToInt32(row.Cells["Reference"].Value) == 1)
                {
                    row.DefaultCellStyle.BackColor = Color.Aqua;
                    row.DefaultCellStyle.Font = new Font("Microsoft Sans Serif", 9, FontStyle.Bold);
                }

                if (row.Cells["Opt"].Value.ToString() == "D")
                {
                    continue;
                }
                int qty = Convert.ToInt32(row.Cells["Qty"].Value);
                double cost = Convert.ToDouble(row.Cells["Cost"].Value);

                row.Cells["Lno"].Value = i;

                row.Cells["Inits"].Value = operatorTextbox.Text = windowsUser.Substring(0, 5).ToUpper();

                row.Cells["Ext"].Value = Math.Round(qty * Convert.ToDouble((cost / result2) * result),2);

                row.Cells["Unit"].Value = Math.Round(Convert.ToDouble((cost / result2) * result),2);

                i++;
            }

            double sum1 = 0;
            double sum2 = 0;
            for (int x = 0; x < dataGridView1.Rows.Count; x++)
            {
                if (Convert.ToString(dataGridView1.Rows[x].Cells["Opt"].Value) == "D")
                {
                    continue;
                }
                sum1 += Convert.ToDouble(dataGridView1.Rows[x].Cells["Ext"].Value);
                sum2 += Convert.ToDouble(dataGridView1.Rows[x].Cells["Core"].Value);

                if (x == dataGridView1.Rows.Count - 1)
                {
                    if (sum1 != Convert.ToDouble(kitPriceTextbox.Text))
                    {
                        double dif = Convert.ToDouble(kitPriceTextbox.Text) - sum1;
                        sum1 = sum1 + dif;
                        dataGridView1.Rows[x].Cells["Unit"].Value = Convert.ToDouble(dataGridView1.Rows[x].Cells["Unit"].Value) + dif;
                    }
                }
            }

            goodsTxtbox.Text = sum1.ToString("0.00");
            coreTextbox.Text = sum2.ToString("0.00");
        }

        private void UpdatePricingUnique()
        {
            if (dataGridView1.Rows.Count > 0)
            {
                int selectedrowindex = dataGridView1.SelectedCells[0].RowIndex;
                DataGridViewRow row = dataGridView1.Rows[selectedrowindex];
                string opt = Convert.ToString(row.Cells["Opt"].Value).ToUpper();

                SqlConnection conn = new SqlConnection(Helper.ConnString("AUTOPART"));
                SqlDataAdapter adapter = new SqlDataAdapter();

                switch (opt)
                {
                    case "D":
                        UpdatePricing();
                        break;

                    case "S":

                        if (conn.State == ConnectionState.Open)
                        {
                            conn.Close();
                        }
                        conn.Open();

                        SqlDataAdapter adapter1 = new SqlDataAdapter(
                        $@"
                    

                   ", conn);
                        adapter1.Fill(dtp1);


                        dataGridView5.DataSource = null;
                        dataGridView5.AutoGenerateColumns = false;
                        dataGridView5.ColumnCount = 3;

                        dataGridView5.Columns[0].Name = "Part";
                        dataGridView5.Columns[0].HeaderText = "Part";
                        dataGridView5.Columns[0].DataPropertyName = "Part";

                        dataGridView5.Columns[1].HeaderText = "Desc";
                        dataGridView5.Columns[1].Name = "Desc";
                        dataGridView5.Columns[1].DataPropertyName = "Desc";

                        dataGridView5.Columns[2].HeaderText = "Free";
                        dataGridView5.Columns[2].Name = "Free";
                        dataGridView5.Columns[2].DataPropertyName = "Free";

                        dataGridView5.DataSource = dtp1;
                        dataGridView5.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
                        conn.Close();
                        dataGridView5.Visible = true;
                        dataGridView5.CurrentCell.Selected = false;
                        found = false;

                        break;

                    case "U":
                        input = Microsoft.VisualBasic.Interaction.InputBox("Please specify a part to upgrade to", "UPGRADE", "");
                        row.Cells["Part"].Value = input;

                        conn.Open();
                        SqlCommand cmd = new SqlCommand($@"SELECT P11 FROM VW_MERI_Pri WHERE Part = '{input}' OR Word0 = {input}", conn);
                        costResult = Convert.ToString(cmd.ExecuteScalar());
                        conn.Close();
                        conn.Open();
                        cmd = new SqlCommand($@"SELECT PG FROM VW_MERI_Pro WHERE Part = '{input}' OR Word0 = {input}", conn);
                        PGResult = Convert.ToString(cmd.ExecuteScalar());
                        conn.Close();
                        conn.Open();
                        cmd = new SqlCommand($@"SELECT Range FROM VW_MERI_Pro WHERE Part = '{input}' OR Word0 = {input}", conn);
                        rangeResult = Convert.ToString(cmd.ExecuteScalar());
                        conn.Close();
                        conn.Open();
                        cmd = new SqlCommand($@"SELECT PSupp FROM VW_MERI_Pro WHERE Part = '{input}' OR Word0 = {input}", conn);
                        supplierResult = Convert.ToString(cmd.ExecuteScalar());
                        conn.Close();

                        if (costResult == "")
                        {
                            MessageBox.Show("Invalid Part!");
                            return;
                        }

                        KitPrice = Convert.ToDouble(kitPriceTextbox.Text);
                        oldCost = Convert.ToDouble(row.Cells["Cost"].Value);
                        newCost = Convert.ToDouble(costResult);
                        row.Cells["Cost"].Value = costResult.ToString();
                        row.Cells["TrCost"].Value = costResult.ToString();
                        row.Cells["PG"].Value = PGResult;
                        row.Cells["Range"].Value = rangeResult;
                        row.Cells["PSupp"].Value = supplierResult;

                        perc = .4;
                        min = 5;

                        diff = oldCost - newCost;

                        if ((perc * diff) > min)
                        {
                            kitPriceTextbox.Text = (KitPrice - oldCost + newCost + (diff * perc)).ToString();
                        }
                        else
                        {
                            kitPriceTextbox.Text = (KitPrice - oldCost + newCost + min).ToString();
                        }

                        UpdatePricingUniqueHelper();


                        conn.Close();

                        break;

                    case "":
                        UpdatePricing();
                        break;

                    default:
                        MessageBox.Show("Option must be D, S, U, R or blank");
                        break;
                }
            }
        }

        private void UpdatePricingUniqueHelper()
        {
            double result = Convert.ToDouble(kitPriceTextbox.Text);
            double result2 = 0;

            for (int y = 0; y < dataGridView1.Rows.Count; y++)
            {
                if (Convert.ToString(dataGridView1.Rows[y].Cells["Opt"].Value) == "D")
                {
                    continue;
                }
                result2 += Math.Round(Convert.ToDouble(dataGridView1.Rows[y].Cells["Cost"].Value) * Convert.ToInt32(dataGridView1.Rows[y].Cells["Qty"].Value), 2);
            }
            kitCostTextbox.Text = result2.ToString("0.00");

            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                if (Convert.ToInt32(row.Cells["Reference"].Value) == 1)
                {
                    row.DefaultCellStyle.BackColor = Color.Aqua;
                    row.DefaultCellStyle.Font = new Font("Microsoft Sans Serif", 9, FontStyle.Bold);
                }

                if (row.Cells["Opt"].Value.ToString() == "D")
                {
                    continue;
                }
                int qty = Convert.ToInt32(row.Cells["Qty"].Value);
                double cost = Convert.ToDouble(row.Cells["Cost"].Value);

                row.Cells["Ext"].Value = Math.Round(qty * Convert.ToDouble((cost / result2) * result), 2);
                row.Cells["Unit"].Value = Math.Round(Convert.ToDouble((cost / result2) * result), 2);
            }

            double sum1 = 0;
            double sum2 = 0;
            for (int x = 0; x < dataGridView1.Rows.Count; x++)
            {
                if (Convert.ToString(dataGridView1.Rows[x].Cells["Opt"].Value) == "D")
                {
                    continue;
                }
                sum1 += Convert.ToDouble(dataGridView1.Rows[x].Cells["Ext"].Value);
                sum2 += Convert.ToDouble(dataGridView1.Rows[x].Cells["Core"].Value);

                if (x == dataGridView1.Rows.Count - 1)
                {
                    if (sum1 != Convert.ToDouble(kitPriceTextbox.Text))
                    {
                        double dif = Convert.ToDouble(kitPriceTextbox.Text) - sum1;
                        sum1 = sum1 + dif;
                        dataGridView1.Rows[x].Cells["Unit"].Value = Convert.ToDouble(dataGridView1.Rows[x].Cells["Unit"].Value) + dif;
                    }
                }
            }

            goodsTxtbox.Text = sum1.ToString("0.00");
            coreTextbox.Text = sum2.ToString("0.00");
        }

        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            TabPage current = (sender as TabControl).SelectedTab;

            if (tabControl1.SelectedIndex == 1)
            {
                kitNumberTextbox.Select();
            }

            if (tabControl1.SelectedIndex == 0)
            {
                customerNumberTextbox.Select();
            }
        }

        private void dataGridView1_SelectionChanged(object sender, EventArgs e)
        {

            if (dataGridView1.SelectedCells.Count > 0)
            {
                dt2.Clear();

                int selectedrowindex = dataGridView1.SelectedCells[0].RowIndex;
                DataGridViewRow selectedRow = dataGridView1.Rows[selectedrowindex];
                string cellValue = Convert.ToString(selectedRow.Cells["Part"].Value);
                UPrice = Convert.ToString(selectedRow.Cells["Unit"].Value);

                SqlConnection conn = new SqlConnection(Helper.ConnString("AUTOPART"));
                SqlDataAdapter adapter = new SqlDataAdapter();
                SqlCommand cmd = conn.CreateCommand();
                cmd.CommandText = $@"
                SELECT B.Branch, B.Name, CAST(S.Free as int) as Free FROM Branches B INNER JOIN Stock S ON B.Branch = S.Branch
                WHERE S.Part = '{cellValue}' ORDER BY CASE WHEN B.Branch = 'BR48' THEN 1 WHEN B.Branch = 'BR34' THEN 2 WHEN B.Branch = 'BR30' THEN 3 WHEN B.Branch = 'BR60' THEN 4 ELSE 5 END, B.Branch
                                ";
                adapter.SelectCommand = cmd;
                conn.Open();
                adapter.Fill(dt2);
                dataGridView2.DataSource = dt2;
                conn.Close();



                dt4.Clear();

                cmd.CommandText = $@"
                    SELECT '1' as quantity, ListPrice,'' as disc, '' as 'WOtax', 'Customer Price' as Terms FROM VW_MERI_Pri WHERE Part = '{cellValue}'
                    UNION ALL
                    SELECT '1',ListPrice, '', A1, 'A1' FROM VW_MERI_Pri WHERE Part = '{cellValue}'
                    UNION ALL
                    SELECT '1',ListPrice, '', A2, 'A2' FROM VW_MERI_Pri WHERE Part = '{cellValue}'
                    UNION ALL
                    SELECT '1',ListPrice, '', A3, 'A3' FROM VW_MERI_Pri WHERE Part = '{cellValue}'
                    UNION ALL
                    SELECT '1',ListPrice, '', A4, 'A4' FROM VW_MERI_Pri WHERE Part = '{cellValue}'
                    UNION ALL
                    SELECT '1',ListPrice, '', P5, 'P5' FROM VW_MERI_Pri WHERE Part = '{cellValue}'
                    UNION ALL
                    SELECT '1',ListPrice, '', P6, 'P6' FROM VW_MERI_Pri WHERE Part = '{cellValue}'
                    UNION ALL
                    SELECT '1',ListPrice, '', P7, 'P7' FROM VW_MERI_Pri WHERE Part = '{cellValue}'
                    UNION ALL
                    SELECT '1',ListPrice, '', P8, 'P8' FROM VW_MERI_Pri WHERE Part = '{cellValue}'
                    UNION ALL
                    SELECT '1',ListPrice, '', P9, 'P9' FROM VW_MERI_Pri WHERE Part = '{cellValue}'
                    UNION ALL
                    SELECT '1',ListPrice, '', P10, 'P10' FROM VW_MERI_Pri WHERE Part = '{cellValue}'
                    UNION ALL
                    SELECT '1',ListPrice, '', P11, 'P11' FROM VW_MERI_Pri WHERE Part = '{cellValue}'
                                ";
                adapter.SelectCommand = cmd;
                conn.Open();
                adapter.Fill(dt4);
                dataGridView3.DataSource = dt4;

                int i = 1;
                if(dataGridView3.Rows.Count > 0)
                {
                    foreach (DataGridViewRow row in dataGridView3.Rows)
                    {
                        if (i == dataGridView3.Rows.Count)
                        {
                            break;
                        }

                        if (i == 1)
                        {
                            row.Cells["WOtax"].Value = UPrice;
                        }

                        decimal newValue = Convert.ToDecimal(row.Cells["WOtax"].Value);
                        decimal newValue1 = Convert.ToDecimal(row.Cells["ListPrice"].Value);
                        row.Cells["WOtax"].Value = newValue.ToString("0.00");
                        row.Cells["ListPrice"].Value = newValue1.ToString("0.00");
                        i++;
                    }
                }

                conn.Close();
            }
        }

        private void customerNumberTextbox_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (e.KeyData == Keys.Tab)
            {
                SqlConnection conn = new SqlConnection(Helper.ConnString("AUTOPART"));
                SqlDataAdapter adapter = new SqlDataAdapter();
                SqlCommand cmd = conn.CreateCommand();
                cmd.CommandText = $@"
                                SELECT 
                                KeyCode, 
                                Name, 
                                Addra, 
                                Addre, 
                                PCode, 
                                Stel, 
                                CAST(Climit as decimal(18,2)) as Climit, 
                                Rep, 
                                PType, 
                                LType, 
                                Email, 
                                MotDueSort,
                                SCont,
                                Fax,
                                ACont,
                                CAST(Balances as decimal(18,2)) as Balances
                                FROM Customer 
                                WHERE KeyCode = '{customerNumberTextbox.Text}'
                                ";
                adapter.SelectCommand = cmd;
                conn.Open();
                adapter.Fill(dt);
                conn.Close();

                if (dt.Rows.Count == 0)
                {
                    MessageBox.Show("Invalid Customer Number");
                }
                else
                {
                    addressTextbox.Text = dt.Rows[0]["Addra"].ToString();
                    nameTaxtbox.Text = dt.Rows[0]["Name"].ToString();
                    cityTextbox.Text = dt.Rows[0]["Addre"].ToString();
                    addressTextbox.Text = dt.Rows[0]["Addra"].ToString();
                    stateTextbox.Text = dt.Rows[0]["MotDueSort"].ToString();
                    zipCodeTextbox.Text = dt.Rows[0]["PCode"].ToString();
                    accountTypeTextbox.Text = dt.Rows[0]["LType"].ToString();
                    docTypeTextbox.Text = dt.Rows[0]["PType"].ToString();
                    salesContactTextbox.Text = dt.Rows[0]["SCont"].ToString();
                    phoneTextbox.Text = dt.Rows[0]["Stel"].ToString();
                    faxTextbox.Text = dt.Rows[0]["Fax"].ToString();
                    accountContactTextbox.Text = dt.Rows[0]["ACont"].ToString();
                    climitTextbox.Text = dt.Rows[0]["Climit"].ToString();
                    repCodeTextbox.Text = dt.Rows[0]["Rep"].ToString();
                    salesRepCodeTextbox.Text = dt.Rows[0]["Rep"].ToString();
                    emailTextbox.Text = dt.Rows[0]["Email"].ToString();
                    balanceValueTextbox.Text = dt.Rows[0]["Balances"].ToString();
                    customerTextbox2.Text = "Customer       " + dt.Rows[0]["Name"].ToString() + " (" + customerNumberTextbox.Text + ")";
                    balanceAmountTextbox.Text = "Bal $" + dt.Rows[0]["Balances"].ToString();

                    if (Convert.ToDecimal(dt.Rows[0]["Balances"]) > Convert.ToDecimal(dt.Rows[0]["Climit"]))
                    {
                        balanceValueTextbox.BackColor = Color.Red;
                    }
                    else
                    {
                        balanceValueTextbox.BackColor = Color.OliveDrab;
                    }
                }
            }


            if (e.KeyData == Keys.F5)
            {
                MessageBox.Show("F5 pressed");
            }
            dt.Clear();
        }

        private void dataGridView1_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if(dataGridView1.Columns[e.ColumnIndex].Name == "Opt")
            {
                UpdatePricingUnique();
            //    UpdateEXT();
            }
            //else
            //{
            //    UpdateEXT();
            //}      
        }

        private void quoteButton_Click(object sender, EventArgs e)
        {
            SqlConnection conn = new SqlConnection(Helper.ConnString("AUTOPART"));
            SqlDataAdapter adapter = new SqlDataAdapter();
            SqlCommand cmd = conn.CreateCommand();
            cmd.CommandText = $@"
                                DECLARE @nextdocno varchar(20)

                                EXEC [dbo].[MERI_GetNextDocNo]

                                @Branch = N'BR48',

                                @Document = N'QUOTE',

                                @nextdocno = @nextdocno OUTPUT

                                SELECT @nextdocno as N'@nextdocno'
                                ";
            conn.Open();
            quoteNumber = (string)cmd.ExecuteScalar();

            using (SqlCommand cmd1 = new SqlCommand("daInsertQheads", conn))
            {
                cmd1.CommandType = CommandType.StoredProcedure;

                cmd1.Parameters.Add("@Document", SqlDbType.VarChar).Value = quoteNumber;

                for (int i = 0; i < dataGridView1.Rows.Count; ++i)
                {
                    sum += Convert.ToDecimal(dataGridView1.Rows[i].Cells["Ext"].Value);
                    sum = decimal.Round(sum, 2, MidpointRounding.AwayFromZero);
                }

                cmd1.Parameters.Add("@Goods", SqlDbType.Money).Value = Convert.ToDecimal(sum);
                cmd1.Parameters.Add("@Vat", SqlDbType.Money).Value = 0.00;
                cmd1.Parameters.Add("@DateTime", SqlDbType.DateTime).Value = DateTime.Now;
                cmd1.Parameters.Add("@Inits", SqlDbType.VarChar).Value = operatorTextbox.Text;
                cmd1.Parameters.Add("@Acct", SqlDbType.VarChar).Value = customerNumberTextbox.Text;
                cmd1.Parameters.Add("@COrder", SqlDbType.VarChar).Value = "";
                cmd1.Parameters.Add("@Branch", SqlDbType.VarChar).Value = "BR48";
                cmd1.Parameters.Add("@Today", SqlDbType.VarChar).Value = "";
                cmd1.Parameters.Add("@Msg", SqlDbType.VarChar).Value = "";
                cmd1.Parameters.Add("@DelAdd", SqlDbType.VarChar).Value = $@"{nameTaxtbox.Text}~{addressTextbox.Text}~{phoneTextbox.Text}~~~{cityTextbox.Text}~{zipCodeTextbox.Text}~{stateTextbox.Text}";
                cmd1.Parameters.Add("@DelMeth", SqlDbType.VarChar).Value = "DEL";
                cmd1.Parameters.Add("@PCode", SqlDbType.VarChar).Value = "101";
                cmd1.Parameters.Add("@Status", SqlDbType.VarChar).Value = "";
                cmd1.Parameters.Add("@Memo", SqlDbType.VarChar).Value = "";
                cmd1.Parameters.Add("@ExpiryDate", SqlDbType.DateTime).Value = "1899-12-30 00:00:00.000";
                cmd1.Parameters.Add("@ExtraFlds", SqlDbType.VarChar).Value = "";
                cmd1.Parameters.Add("@WebStatus", SqlDbType.Int).Value = 0;
                cmd1.Parameters.Add("@GST", SqlDbType.Money).Value = 0.00;
                cmd1.Parameters.Add("@PST", SqlDbType.Money).Value = 0.00;
                cmd1.Parameters.Add("@Invoice", SqlDbType.VarChar).Value = "";
                cmd1.Parameters.Add("@DocLink", SqlDbType.VarChar).Value = "";
                cmd1.Parameters.Add("@Amended", SqlDbType.Int).Value = 1;
                cmd1.Parameters.Add("@MediaCode", SqlDbType.VarChar).Value = "";
                cmd1.Parameters.Add("@DelAdd2", SqlDbType.VarChar).Value = "";
                cmd1.Parameters.Add("@Locked", SqlDbType.VarChar).Value = "";
                cmd1.Parameters.Add("@TransactionID", SqlDbType.Int).Value = 0;
                cmd1.Parameters.Add("@InternalMsg", SqlDbType.VarChar).Value = "";
                cmd1.Parameters.Add("@NOBO", SqlDbType.VarChar).Value = "";
                cmd1.Parameters.Add("@LotteryDrawNumber", SqlDbType.VarChar).Value = "";
                cmd1.Parameters.Add("@LotteryNumber", SqlDbType.VarChar).Value = "";
                cmd1.Parameters.Add("@LotteryDrawDate", SqlDbType.DateTime).Value = "1899-12-30 00:00:00.000";
                cmd1.Parameters.Add("@WON", SqlDbType.VarChar).Value = "";
                cmd1.Parameters.Add("@WOREQUEST", SqlDbType.DateTime).Value = "1899-12-30 00:00:00.000";
                cmd1.Parameters.Add("@Split", SqlDbType.VarChar).Value = "";
                cmd1.Parameters.Add("@Delmeth2", SqlDbType.VarChar).Value = "DEL - Delivery";
                cmd1.Parameters.Add("@BasketID", SqlDbType.VarChar).Value = "";
                cmd1.Parameters.Add("@UTC", SqlDbType.DateTime).Value = DateTime.Now;
                cmd1.Parameters.Add("@ShopcardNumber", SqlDbType.VarChar).Value = "";
                cmd1.Parameters.Add("@Origin", SqlDbType.VarChar).Value = "";
                cmd1.Parameters.Add("@PKDocument", SqlDbType.VarChar).Value = "";
                /*cmd1.Parameters.Add("@RowCnt", SqlDbType.VarChar).Value = dt1.Rows.Count;*/

                try
                {
                    cmd1.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                conn.Close();
            }

            SqlCommand cmd3 = conn.CreateCommand();
            cmd3.CommandText = $@"
                                    SELECT VCode FROM Customer WHERE KeyCode = '{customerNumberTextbox.Text}'
                                ";
            conn.Open();
            VCode = (string)cmd3.ExecuteScalar();
            conn.Close();

            for (int r = 0; r < dataGridView1.Rows.Count; r++)
            {
                DataGridViewRow row = dataGridView1.Rows[r];
                using (SqlCommand cmd2 = new SqlCommand("daInsertQLines", conn))
                {
                    cmd2.CommandType = CommandType.StoredProcedure;

                    cmd2.Parameters.Add("@Document", SqlDbType.VarChar).Value = quoteNumber;
                    cmd2.Parameters.Add("@Seqno", SqlDbType.SmallInt).Value = r + 1;
                    cmd2.Parameters.Add("@Part", SqlDbType.VarChar).Value = row.Cells["Part"].Value;
                    cmd2.Parameters.Add("@Qty", SqlDbType.Money).Value = row.Cells["Qty"].Value;
                    cmd2.Parameters.Add("@CQty", SqlDbType.Money).Value = "0.00";
                    cmd2.Parameters.Add("@Rc", SqlDbType.VarChar).Value = "";
                    cmd2.Parameters.Add("@Unit", SqlDbType.Money).Value = row.Cells["Unit"].Value;
                    cmd2.Parameters.Add("@VatInc", SqlDbType.Money).Value = 0.00;
                    cmd2.Parameters.Add("@DateTime", SqlDbType.DateTime).Value = DateTime.Now;
                    cmd2.Parameters.Add("@COrder", SqlDbType.VarChar).Value = "";
                    cmd2.Parameters.Add("@DateReq", SqlDbType.DateTime).Value = "1899-12-30 00:00:00.000";
                    cmd2.Parameters.Add("@Price", SqlDbType.Money).Value = row.Cells["Unit"].Value;
                    cmd2.Parameters.Add("@RRP", SqlDbType.Money).Value = row.Cells["List"].Value;
                    cmd2.Parameters.Add("@Disc", SqlDbType.Money).Value = 0.00;
                    cmd2.Parameters.Add("@Rdisc", SqlDbType.Money).Value = 0.00;
                    cmd2.Parameters.Add("@HC", SqlDbType.Money).Value = 0.00;
                    cmd2.Parameters.Add("@VCode", SqlDbType.SmallInt).Value = 9999;
                    cmd2.Parameters.Add("@VRate", SqlDbType.Money).Value = 0.00;
                    cmd2.Parameters.Add("@Supp", SqlDbType.VarChar).Value = row.Cells["Supplier"].Value;
                    cmd2.Parameters.Add("@PG", SqlDbType.VarChar).Value = row.Cells["PG"].Value;
                    cmd2.Parameters.Add("@TrCost", SqlDbType.Money).Value = row.Cells["TrCost"].Value; ;
                    cmd2.Parameters.Add("@Today", SqlDbType.VarChar).Value = "DQ";
                    cmd2.Parameters.Add("@ExInfo", SqlDbType.VarChar).Value = "Tax=N";
                    cmd2.Parameters.Add("@Comments", SqlDbType.VarChar).Value = "";
                    cmd2.Parameters.Add("@BopDes", SqlDbType.VarChar).Value = row.Cells["Drscription"].Value;
                    cmd2.Parameters.Add("@Flags", SqlDbType.VarChar).Value = "";
                    cmd2.Parameters.Add("@StkPart", SqlDbType.VarChar).Value = row.Cells["Part"].Value;
                    cmd2.Parameters.Add("@Surch", SqlDbType.Bit).Value = 0;
                    cmd2.Parameters.Add("@Curr", SqlDbType.Bit).Value = 0;
                    cmd2.Parameters.Add("@PCode", SqlDbType.VarChar).Value = "101";
                    cmd2.Parameters.Add("@BOP", SqlDbType.VarChar).Value = "";
                    cmd2.Parameters.Add("@ExtraFlds", SqlDbType.VarChar).Value = "";
                    cmd2.Parameters.Add("@Branch", SqlDbType.VarChar).Value = "BR48";
                    cmd2.Parameters.Add("@Kit", SqlDbType.VarChar).Value = kitNumberTextbox.Text;
                    //cmd2.Parameters.Add("@Kit", SqlDbType.VarChar).Value = "";
                    cmd2.Parameters.Add("@POrder", SqlDbType.VarChar).Value = "";
                    cmd2.Parameters.Add("@POrderQty", SqlDbType.Money).Value = 0.00;
                    cmd2.Parameters.Add("@POrderSupp", SqlDbType.VarChar).Value = "";
                    cmd2.Parameters.Add("@LC3Blob", SqlDbType.VarChar).Value = "";
                    cmd2.Parameters.Add("@GSTRate", SqlDbType.Money).Value = 0.00;
                    cmd2.Parameters.Add("@PSTRate", SqlDbType.Money).Value = 0.00;
                    cmd2.Parameters.Add("@InvInits", SqlDbType.VarChar).Value = operatorTextbox.Text;
                    cmd2.Parameters.Add("@Invoice", SqlDbType.VarChar).Value = "";
                    cmd2.Parameters.Add("@ClsQty", SqlDbType.Money).Value = Convert.ToDecimal(row.Cells["Free"].Value) - Convert.ToDecimal(row.Cells["Qty"].Value);
                    cmd2.Parameters.Add("@ReasonTxt", SqlDbType.VarChar).Value = "";
                    cmd2.Parameters.Add("@BuyBackRC", SqlDbType.VarChar).Value = "";
                    cmd2.Parameters.Add("@ShipFlag", SqlDbType.VarChar).Value = "";
                    cmd2.Parameters.Add("@TrCost2", SqlDbType.Money).Value = row.Cells["TrCost"].Value; ;
                    cmd2.Parameters.Add("@SalesRep", SqlDbType.VarChar).Value = repCodeTextbox.Text;
                    cmd2.Parameters.Add("@CoreBuild", SqlDbType.VarChar).Value = "";
                    cmd2.Parameters.Add("@Vehicle", SqlDbType.VarChar).Value = "";
                    cmd2.Parameters.Add("@VehicleID", SqlDbType.VarChar).Value = "";
                    cmd2.Parameters.Add("@TimberMakeup", SqlDbType.VarChar).Value = "";
                    cmd2.Parameters.Add("@Extra1", SqlDbType.VarChar).Value = "";
                    cmd2.Parameters.Add("@Extra2", SqlDbType.VarChar).Value = "";
                    cmd2.Parameters.Add("@Extra3", SqlDbType.VarChar).Value = "";
                    cmd2.Parameters.Add("@TargetPrice", SqlDbType.Money).Value = 0.00;
                    cmd2.Parameters.Add("@FloorPrice", SqlDbType.Money).Value = 0.00;
                    cmd2.Parameters.Add("@CeillingPrice", SqlDbType.Money).Value = 0.00;
                    cmd2.Parameters.Add("@VRM", SqlDbType.VarChar).Value = "";
                    cmd2.Parameters.Add("@EngineNo", SqlDbType.VarChar).Value = "";
                    cmd2.Parameters.Add("@CeilingPrice", SqlDbType.Money).Value = 0.00;
                    cmd2.Parameters.Add("@BatchInfo", SqlDbType.VarChar).Value = "";
                    cmd2.Parameters.Add("@ListPrice", SqlDbType.Money).Value = row.Cells["List"].Value;
                    cmd2.Parameters.Add("@GenBranch", SqlDbType.VarChar).Value = "BR48";
                    cmd2.Parameters.Add("@PriceLocked", SqlDbType.VarChar).Value = "";
                    cmd2.Parameters.Add("@Discounts", SqlDbType.VarChar).Value = "";
                    cmd2.Parameters.Add("@Weight", SqlDbType.Money).Value = 0.00;
                    cmd2.Parameters.Add("@VOC", SqlDbType.Money).Value = 0.00;
                    cmd2.Parameters.Add("@NgoCode", SqlDbType.Int).Value = 0;
                    cmd2.Parameters.Add("@NgoStatus", SqlDbType.Int).Value = 0;
                    cmd2.Parameters.Add("@NgoPrice", SqlDbType.Money).Value = 0.00;
                    cmd2.Parameters.Add("@PartPosition", SqlDbType.VarChar).Value = "";
                    cmd2.Parameters.Add("@TrailerNumber", SqlDbType.VarChar).Value = "";
                    cmd2.Parameters.Add("@FittingReason", SqlDbType.VarChar).Value = "";
                    cmd2.Parameters.Add("@OrigTaxCode", SqlDbType.Int).Value = VCode;
                    cmd2.Parameters.Add("@P55Promo", SqlDbType.VarChar).Value = "";
                    cmd2.Parameters.Add("@ConditionalTax", SqlDbType.VarChar).Value = "";
                    cmd2.Parameters.Add("@OriginalCost", SqlDbType.Money).Value = 0.00;
                    cmd2.Parameters.Add("@DelMethLine", SqlDbType.VarChar).Value = "";
                    cmd2.Parameters.Add("@EquivalentPart", SqlDbType.VarChar).Value = "";
                    cmd2.Parameters.Add("@Range", SqlDbType.VarChar).Value = row.Cells["Range"].Value;
                    cmd2.Parameters.Add("@POLineNo", SqlDbType.SmallInt).Value = 0;
                    cmd2.Parameters.Add("@CoreExch", SqlDbType.VarChar).Value = "^";
                    cmd2.Parameters.Add("@SubPart", SqlDbType.VarChar).Value = "";
                    cmd2.Parameters.Add("@SubLineNo", SqlDbType.SmallInt).Value = 0;
                    cmd2.Parameters.Add("@PerQtyPrice", SqlDbType.Money).Value = 0.00;
                    cmd2.Parameters.Add("@PerQty", SqlDbType.SmallInt).Value = 0;
                    cmd2.Parameters.Add("@PriceDerivation", SqlDbType.VarChar).Value = "Customer Price";
                    cmd2.Parameters.Add("@MultiBopNo", SqlDbType.VarChar).Value = "";
                    cmd2.Parameters.Add("@MultiBopGRN", SqlDbType.Bit).Value = 0;
                    cmd2.Parameters.Add("@PKDocument", SqlDbType.Bit).Value = 0;
                    cmd2.Parameters.Add("@PKSeqno", SqlDbType.SmallInt).Value = row.Cells["lno"].Value;

                    try
                    {
                        conn.Open();
                        cmd2.ExecuteNonQuery();
                        conn.Close();
                        
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message, "Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                }
            }
            MessageBox.Show($@"Quote {quoteNumber} has been generated");

            QuoteCreator.ActiveForm.Dispose();
            QuoteCreator sd = new QuoteCreator();
            sd.Show();
        }


        private void searchButton2Button_Click(object sender, EventArgs e)
        {
            partnumberSearch();
        }

        private void dataGridView4_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            kitNumberTextbox.Text = dataGridView4.SelectedCells[0].Value.ToString();
            //part = dataGridView2.SelectedCells[0].Value.ToString();
            found = true;
            dataGridView4.Visible = false;
            partnumberSearch();            
        }

        private void partnumberSearch()
        {
            dtp.Clear();
            SqlConnection conn = new SqlConnection(Helper.ConnString("AUTOPART"));
            {
                if (conn.State == ConnectionState.Open)
                {
                    conn.Close();
                }
                conn.Open();

                SqlDataAdapter adapter1 = new SqlDataAdapter(
                $@"
                    SELECT KeyCode, [Desc] FROM Product WHERE KeyCode LIKE '%{kitNumberTextbox.Text}%' ORDER BY KeyCode
                   ", conn);
                adapter1.Fill(dtp);
                if (dtp.Rows.Count == 1)
                {
                    kitNumberTextbox.Text = dtp.Rows[0]["KeyCode"].ToString();
                    kitDescriptionTextbox.Text = dtp.Rows[0]["Desc"].ToString();
                    conn.Close();
                    found = true;

                    SqlDataAdapter adapter = new SqlDataAdapter();
                    SqlCommand cmd = conn.CreateCommand();
                    cmd.CommandText = $@"
WITH CTE0 (Part, Qty, Reference) as ( SELECT
      [Piece],
      CAST(Qty as int),
      Reference
  FROM [AUTOPART].[dbo].[Kits]
  WHERE Part = '{kitNumberTextbox.Text}'),

  CTE1 (inits, opt, Part, [Description], Free, Cost, Unit, Core, Ext, ListPrice, TrCost, Supplier, PG, Range) as (
    SELECT '', '' , P.Part, P.[Description], CAST(P.Free as int), CAST(P.P11 as decimal(18,2)), '', P.CorePrice,'',  V.ListPrice, V.P11, V.PSupp, V.PG, V.Range
    FROM VW_MERI_Pro P INNER JOIN VW_MERI_Pri V ON P.Part = V.Part
    WHERE P.Branch = 'BR48'
  )

  SELECT II.inits, II.opt, I.Part, II.Description, II.Free, I.Qty, 
  CAST(CASE WHEN II.ListPrice IS NULL THEN 0 ELSE II.ListPrice END as decimal(18,2)) as List, 
  CAST(CASE WHEN II.Cost IS NULL THEN 0 ELSE II.Cost END as decimal(18,2)) as Cost, 
  CAST(CASE WHEN II.Unit IS NULL THEN 0 ELSE II.Unit END as decimal(18,2)) as Unit, II.Core, II.Ext, II.TrCost, II.Supplier, II.PG, II.Range, I.Reference 
  FROM CTE0 I INNER JOIN CTE1 II ON I.Part = II.PArt 
  ORDER BY I.Part
                                ";
                    adapter.SelectCommand = cmd;
                    conn.Open();
                    adapter.Fill(dt1);
                    conn.Close();

                    if (dt1.Rows.Count == 0)
                    {
                        MessageBox.Show("Invalid Kit Number");
                    }
                    else
                    {

                        SqlCommand cmd4 = conn.CreateCommand();
                        cmd4.CommandText = $@"
                                    SELECT [Desc] FROM Kits WHERE Part = '{kitNumberTextbox.Text}'
                                ";
                        conn.Open();
                        string kitDescription = (string)cmd4.ExecuteScalar();

                        kitDescriptionTextbox.Text = kitDescription;
                        listBox4.Items.Add(kitNumberTextbox.Text);
                        dataGridView1.DataSource = dt1;
                        UpdatePricing();
                    }
                }
                else
                {
                    dataGridView4.DataSource = null;
                    dataGridView4.AutoGenerateColumns = false;
                    dataGridView4.ColumnCount = 2;

                    dataGridView4.Columns[0].Name = "KeyCode";
                    dataGridView4.Columns[0].HeaderText = "KeyCode";
                    dataGridView4.Columns[0].DataPropertyName = "KeyCode";

                    dataGridView4.Columns[1].HeaderText = "Desc";
                    dataGridView4.Columns[1].Name = "Desc";
                    dataGridView4.Columns[1].DataPropertyName = "Desc";

                    dataGridView4.DataSource = dtp;
                    dataGridView4.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
                    conn.Close();
                    dataGridView4.Visible = true;
                    dataGridView4.CurrentCell.Selected = false;
                    found = false;
                    //return;
                }
            }
        }

        private void dataGridView5_CellContentDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            SqlConnection conn = new SqlConnection(Helper.ConnString("AUTOPART"));
            //SqlDataAdapter adapter = new SqlDataAdapter();

            int selectedrowindex = dataGridView1.SelectedCells[0].RowIndex;
            DataGridViewRow row = dataGridView1.Rows[selectedrowindex];

            input = dataGridView5.Rows[e.RowIndex].Cells["Part"].Value.ToString();

            row.Cells["Part"].Value = input;

            conn.Open();
            SqlCommand cmd1 = new SqlCommand($@"SELECT P11 FROM VW_MERI_Pri WHERE Part = '{input}'", conn);
            costResult = Convert.ToString(cmd1.ExecuteScalar());
            conn.Close();
            conn.Open();
            cmd1 = new SqlCommand($@"SELECT PG FROM VW_MERI_Pro WHERE Part = '{input}'", conn);
            PGResult = Convert.ToString(cmd1.ExecuteScalar());
            conn.Close();
            conn.Open();
            cmd1 = new SqlCommand($@"SELECT Range FROM VW_MERI_Pro WHERE Part = '{input}'", conn);
            rangeResult = Convert.ToString(cmd1.ExecuteScalar());
            conn.Close();
            conn.Open();
            cmd1 = new SqlCommand($@"SELECT PSupp FROM VW_MERI_Pro WHERE Part = '{input}'", conn);
            supplierResult = Convert.ToString(cmd1.ExecuteScalar());
            conn.Close();

            if (costResult == "")
            {
                MessageBox.Show("Invalid Part!");
                return;
            }

            double KitPrice = Convert.ToDouble(kitPriceTextbox.Text);
            double oldCost = Convert.ToDouble(row.Cells["Cost"].Value);
            double newCost = Convert.ToDouble(costResult);
            row.Cells["Cost"].Value = costResult.ToString();
            row.Cells["TrCost"].Value = costResult.ToString();
            row.Cells["PG"].Value = PGResult;
            row.Cells["Range"].Value = rangeResult;
            row.Cells["PSupp"].Value = supplierResult;

            kitPriceTextbox.Text = (KitPrice - oldCost + newCost).ToString();

            UpdatePricingUniqueHelper();

            dataGridView5.Visible = false;

            conn.Close();
        }

        private void resetButton_Click(object sender, EventArgs e)
        {
            QuoteCreator.ActiveForm.Dispose();
            QuoteCreator sd = new QuoteCreator();
            sd.Show();
        }

        private void dataGridView1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Enter)
            {
                e.Handled = true;
            }
        }

        private void dataGridView1_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Enter)
            {
                dataGridView1.CurrentCell.Selected = false;


                if (dataGridView1.CurrentCell.ColumnIndex < dataGridView1.ColumnCount - 1)
                {
                    dataGridView1.Rows[dataGridView1.CurrentCell.RowIndex].Cells[dataGridView1.CurrentCell.ColumnIndex + 1]
                        .Selected = true;

                    dataGridView1.CurrentCell = dataGridView1.Rows[dataGridView1.CurrentCell.RowIndex - 1]
                        .Cells[dataGridView1.CurrentCell.ColumnIndex + 1];

                }
                else if (dataGridView1.CurrentCell.RowIndex < dataGridView1.RowCount - 1)
                {
                    dataGridView1.Rows[dataGridView1.CurrentCell.RowIndex + 1].Cells[0].Selected = true;
                    dataGridView1.CurrentCell = dataGridView1.Rows[dataGridView1.CurrentCell.RowIndex + 1].Cells[0];
                }


            }
        }
    }
}
