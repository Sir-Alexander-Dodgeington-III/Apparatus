using System;
using System.Collections.Generic;
using System.Drawing;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Windows.Forms;
using Microsoft.VisualBasic;

namespace kitCreator
{
    public partial class kitCreator : Form
    {
        public string part;
        public string prompt;
        private TextBox CurrentTextbox = null;
        public DataTable dt = new DataTable();
        public DataTable dtDescr = new DataTable();
        public DataTable dataTable = new DataTable();
        public DataTable sorteddt = new DataTable();
        public DataTable promptDataTable = new DataTable();
        public DataTable itemsDataTable = new DataTable();
        public DataTable partSearchDT = new DataTable();
        public DataTable priceSearch = new DataTable();
        public string word5;
        public string promptID;
        public bool found = false;
        public string partNumber;
        public string description;
        public int Qty = 1;
        public decimal baseCost;
        public decimal basePrice;
        public bool firstLoad;
        public object oldValue;
        public string A2;
        public string cost;
        public bool errorStatus = false;
        public string searchValue;
        public string cellText;
        public string promptType;
        public string inputType;
        public int dataTableIndex;
        public DataGridViewCell clickedCell;
        public string searchPrompt;
        public int q;
        public string searchCategory;
        public string KitCat;

        public kitCreator()
        {
            InitializeComponent();
            this.CenterToParent();
            ScaleFormAndControlsToScreen();
            downButton.BackgroundImage.RotateFlip(RotateFlipType.RotateNoneFlipY);
            downButton2.BackgroundImage.RotateFlip(RotateFlipType.RotateNoneFlipY);
            searchButton.Enabled = false;
            //dataGridView2.Columns[1].ReadOnly = true;
            dataGridView2.AutoGenerateColumns = false;
            lev2Textbox.DropDownStyle = ComboBoxStyle.DropDownList;
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

        private void kitNumberTextbox_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (e.KeyCode == Keys.Enter || e.KeyCode == Keys.Tab)
            {
                this.Cursor = Cursors.WaitCursor;
                dt.Clear();
                dataGridView1.DataSource = null;
                dataGridView2.DataSource = null;
                part = kitNumberTextbox.Text;
                part = part.ToUpper();
                firstLoad = true;


                SqlConnection connection1 = new SqlConnection(Helper.ConnString("AUTOPART"));

                connection1.Open();
                SqlDataAdapter adptr = new SqlDataAdapter(
                $@"
                    DECLARE @part VARCHAR(MAX)
                    SET @part = '{part}'                             
                
                    SELECT KeyCode FROM Product (NOLOCK) WHERE KeyCode = '{part}'
            "
                , connection1);
                adptr.Fill(dt);

                if (dt.Rows.Count == 0)
                {
                    this.Cursor = Cursors.Default;
                    MessageBox.Show($@"Part number {part} has not been created in Autopart yet. Please contact the product/pricing department at phone number 712-262-1141 to get this kit added into Autopart before you proceed.", "Notice", MessageBoxButtons.OK);
                    return;
                }

                dt.Clear();
                dt.Columns.Remove("KeyCode");

                connection1.Close();


                connection1.Open();
                SqlDataAdapter adptr2 = new SqlDataAdapter(
                $@"
                    DECLARE @part VARCHAR(MAX)
                    SET @part = '{part}'                             
                
                    SELECT SubKey1 FROM MVPR (NOLOCK) WHERE Prefix = 'S' AND SubKey1 = '{part}' AND (SubKey2 = 26 OR SubKey2 = 2)
            "
                , connection1);
                adptr2.Fill(dt);

                if (dt.Rows.Count == 0)
                {
                    this.Cursor = Cursors.Default;
                    MessageBox.Show($@"The appropriate pricing does not exist for number {part}. Please contact the pricing department at phone number 712-262-1141 to get pricing added to this part in Autopart before you proceed.", "Notice", MessageBoxButtons.OK);
                    return;
                }

                dt.Clear();
                dt.Columns.Remove("SubKey1");

                connection1.Close();


                connection1.Open();
                SqlDataAdapter adptr1 = new SqlDataAdapter(
                $@"
                    DECLARE @part VARCHAR(MAX)
                    SET @part = '{part}'                             
                
                    ;WITH CTE (KitSKU, Prompt, PromptID, PromptType, InputType, ComponentSKU, ComponentQty, ComponentPrice, UpgradePrice, OptionSort, ComponentType, ComponentCost, ComponentSell, KitSell) as (
                    SELECT   I.KitSKU, I.Prompt, I.PromptID, I.PromptType, I.InputType, I.ComponentSKU, I.ComponentQty, I.ComponentPrice, I.UpgradePrice, I.OptionSort, I.ComponentType, P.A1 AS ComponentCost, 
                         P2.A1 AS ComponentSell, P3.A1 AS KitSell
                         FROM         MERI_Kit_Upgrade AS I INNER JOIN
                         Mvpr AS P ON I.ComponentSKU = P.SubKey1 INNER JOIN
                         Mvpr AS P2 ON I.ComponentSKU = P2.SubKey1 INNER JOIN
                         Mvpr AS P3 ON I.KitSKU = P3.SubKey1
                         WHERE     (P.Prefix = 'S') AND (P.SubKey2 = 27) AND (P2.Prefix = 'S') AND (P2.SubKey2 = 2) AND (P3.Prefix = 'S') AND (P3.SubKey2 = 2)
                    ),

                    CTE1 (KeyCode, word5, Description, Make, Model, StartYEar, EndYear, Level1, Level2, Level3, ImageURL) as (
                    SELECT P.KeyCode, P.word5, P.[Desc], K.Make, K.Model, K.StartYear, K.EndYear, K.Level1, K.Level2, K.Level3, K.ImageURL
                    FROM Product P 
                    LEFT OUTER JOIN MERI_KIT_INFO K ON K.InternalSKU = P.KeyCode
                    ),

                    CTE2 (ComponentSKU, LongDesc, SimplePart, Brand, MFR_SKU, KitCategory) as (
                    SELECT P.KeyCode, C.Display_Desc, P.Word5, C.Brand, C.MFR_SKU, pv.Value
                    FROM  Product AS P LEFT OUTER JOIN
                     ProdVals AS pv ON P.KeyCode = pv.Part and pv.Attribute = 'Kit Category' LEFT OUTER JOIN
                     MERI_KIT_Component AS C ON P.KeyCode = C.ComponentSKU 

                    )

                    SELECT I.KitSKU, I.ComponentSKU, III.MFR_SKU, III.LongDesc, I.ComponentQty, I.OptionSort, I.Prompt, I.PromptID, I.PromptType, I.ComponentType, 
                    I.InputType, ISNULL(I.ComponentPrice,'0.00') as ComponentPrice, ISNULL(CAST(I.UpgradePrice as Decimal(18,2)), '0.00') as UpgradePrice, 
                    III.SimplePart as word5, II.Description, II.Make, II.Model, II.StartYear, II.EndYear, II.Level1, II.Level2, II.Level3, II.ImageURL, 
                    I.KitSell as KitPrice, I.ComponentSell as ComponentA2, I.ComponentCost, III.Brand, III.KitCategory
                    FROM CTE I 
                    INNER JOIN CTE1 II ON I.KitSKU = II.KeyCode
                    INNER JOIN CTE2 III ON I.ComponentSKU = III.ComponentSKU
                    WHERE I.KitSKU = @part OR II.word5 = @part
                    ORDER BY PromptID, OptionSort ASC
            "
                , connection1);
                adptr1.Fill(dt);

                if (dt.Rows.Count == 0)
                {
                    DialogResult dialogResult = MessageBox.Show($@"You are about to create a new kit. Continue?", "Create new kit?", MessageBoxButtons.YesNo);
                    if (dialogResult == DialogResult.No)
                    {
                        this.Cursor = Cursors.Default;
                        return;
                    }
                    else
                    {
                        this.Cursor = Cursors.Default;
                        //okButton.Enabled = true;
                        updatePriceButton.Enabled = true;
                        viewKitButton.Enabled = true;
                        string substring = "KIT";
                        if (!part.StartsWith(substring))
                        {
                            part = substring + part;
                            kitNumberTextbox.Text = part;
                        }
                        else
                        {
                            kitNumberTextbox.Text = part;
                        }
                    }
                }
                else
                {

                    kitNumberTextbox.Text = dt.Rows[0]["KitSKU"].ToString();
                    kitDescriptionTextBox.Text = dt.Rows[0]["Description"].ToString();
                    part = kitNumberTextbox.Text;
                    word5 = dt.Rows[0]["word5"].ToString();
                    decimal moneyValue = Convert.ToDecimal(dt.Rows[0]["KitPrice"]);
                    sellPriceTextbox.Text = moneyValue.ToString("0.00");

                    connection1.Close();

                    loadPrompts();

                    //identifyBs();

                    decimal sumForB = 0;
                    foreach (DataRow row in dt.Rows)
                    {
                        if (row["ComponentType"].ToString() == "B")
                        {
                            decimal componentCost = Convert.ToDecimal(row["ComponentQty"]) * Convert.ToDecimal(row["ComponentCost"]);
                            sumForB += componentCost;
                        }
                    }

                    int countNullCategories = 0;
                    foreach (DataRow row in dt.Rows)
                    {
                        if (row["KitCategory"].ToString() == "")
                        {
                            countNullCategories++;
                        }
                    }

                    if(countNullCategories > 0)
                    {
                        MessageBox.Show($@"{countNullCategories} part(s) in this kit do not have a kit category. This kit can not be properly priced until all components have a kit category. Please ensure all parts have a valid kit category before attempting to save this kit.");
                    }

                    textbox1.Text = Math.Round(sumForB, 2).ToString();

                    updateMargin();

                    GetImageAndData();
                    firstLoad = false;
                    okButton.Enabled = true;
                    updatePriceButton.Enabled = true;
                    viewKitButton.Enabled = true;
                    this.Cursor = Cursors.Default;
                }

                // Define your SQL query
                string query = "" +
                    "SELECT DISTINCT Value FROM ProdVals " +
                    "WHERE Attribute = 'Kit Category' ORDER BY Value";

                // Create a SqlConnection using the connection string
                using (SqlConnection connection = new SqlConnection(Helper.ConnString("AUTOPART")))
                {
                    try
                    {
                        // Open the connection
                        connection.Open();

                        // Create a SqlCommand to execute the query
                        using (SqlCommand command = new SqlCommand(query, connection))
                        {
                            // Execute the query and get the results
                            using (SqlDataReader reader = command.ExecuteReader())
                            {
                                // Clear existing items in the ComboBox
                                comboBox1.Items.Clear();

                                // Loop through the results and add each item to the ComboBox
                                while (reader.Read())
                                {
                                    // Assuming you are reading a single column of string values
                                    comboBox1.Items.Add(reader["Value"].ToString());
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        // Handle any errors that might have occurred
                        MessageBox.Show("An error occurred: " + ex.Message);
                    }
                }

                /*foreach (DataRow row in dt.Rows)
                {
                    if (row["OptionSort"].ToString() == "1")
                    {
                        row["componentType"] = "B";
                    }
                }*/
                if (e.KeyCode == Keys.Enter)
                {
                    makeTextBox.Focus();
                }
                getEngineCats();
                CalculatePrices();
            }

        }

        private void getEngineCats()
        {
            // Define your connection string
            if (dt.Rows.Count > 0)
            {
                searchCategory = dt.Rows[0]["Level2"].ToString();
            }
            else
            {
                searchCategory = "";
            }

            SqlConnection connectionString = new SqlConnection(Helper.ConnString("AUTOPART"));
            connectionString.Open();
            SqlDataAdapter adptr1 = new SqlDataAdapter(
            $@"
                    IF Exists (
                         Select EngineCategory FROM MERI_NAP_EngineCats WHERE EngineCategory = '{searchCategory}')
                    BEGIN
                         Select EngineCategory FROM MERI_NAP_EngineCats WHERE EngineCategory = '{searchCategory}'
                         UNION ALL
                         SELECT EngineCategory FROM MERI_NAP_EngineCats WHERE EngineCategory != '{searchCategory}'                    
                    END 
                    ELSE
                    BEGIN
                         SELECT '' as EngineCategory
                         UNION ALL
                         SELECT EngineCategory FROM MERI_NAP_EngineCats
                    END
            "
            , connectionString);
            adptr1.Fill(dataTable);

            lev1Textbox.Text = "Engine Kits";
            lev2Textbox.DataSource = dataTable;
            lev2Textbox.DisplayMember = "EngineCategory";

        }

        private void loadPrompts()
        {
            promptDataTable.Clear();

            renumberPrompts();

            DataView sortedView = dt.DefaultView;
            sortedView.Sort = "PromptID ASC";
            sorteddt = sortedView.ToTable();

            var distinctValues = sorteddt.AsEnumerable()
                                .Select(row => row.Field<string>("Prompt"))
                                .Distinct();

            // Add a column to the destination DataTable
            if (promptDataTable.Columns.Contains("Prompt"))
            {
            }
            else
            {
                promptDataTable.Columns.Add("Prompt", typeof(string));
            }

            // Iterate over the distinct values and add them to the destination DataTable
            foreach (var value in distinctValues)
            {
                DataRow newRow1 = promptDataTable.NewRow();
                newRow1["Prompt"] = value;
                promptDataTable.Rows.Add(newRow1);
            }

            dataGridView1.DataSource = promptDataTable;

            dataGridView1.ClearSelection();

            foreach (DataGridViewColumn column in dataGridView1.Columns)
            {
                column.SortMode = DataGridViewColumnSortMode.NotSortable;
            }


            if (searchValue != "")
            {
                foreach (DataGridViewRow row in dataGridView1.Rows)
                {
                    foreach (DataGridViewCell cell in row.Cells)
                    {
                        if (cell.Value != null && cell.Value.ToString() == searchValue)
                        {
                            row.Selected = true;
                            // Optionally, scroll the DataGridView to make the selected row visible
                            dataGridView1.FirstDisplayedScrollingRowIndex = row.Index;
                            break; // We found the value, so no need to continue looping
                        }
                    }
                }
            }
            refreshButton.Enabled = true;
            lev1Textbox.Text = "Engine Kits";


        }

        private void GetImageAndData()
        {
            makeTextBox.Text = dt.Rows[0]["Make"].ToString();
            ModelTextbox.Text = dt.Rows[0]["Model"].ToString();
            yearTextbox1.Text = dt.Rows[0]["StartYear"].ToString();
            yearTextbox2.Text = dt.Rows[0]["EndYear"].ToString();
            lev1Textbox.Text = dt.Rows[0]["Level1"].ToString();
            lev2Textbox.Text = dt.Rows[0]["Level2"].ToString();
            lev3Textbox.Text = dt.Rows[0]["Level3"].ToString();

        }

        private void upButton_Click(object sender, EventArgs e)
        {
            string column1 = "Prompt"; // Specify the name of the first column
            string columnToUpdate = "PromptID"; // Specify the name of the column to update

            string searchValue1 = dataGridView1.SelectedCells[0].Value.ToString(); // Specify the value to search for in the first column
            int searchValue2Index = dataGridView1.SelectedCells[0].RowIndex - 1;  // Specify the value to search for in the second column
            string searchValue2 = dataGridView1.Rows[searchValue2Index].Cells["Prompt"].Value.ToString();

            // Update the value in the column for each matching row
            foreach (DataRow row in dt.Rows)
            {
                if (row[column1].ToString() == searchValue1)
                {
                    object currentValue = row[columnToUpdate];
                    row[columnToUpdate] = Convert.ToInt32(currentValue) - 1;
                }
                if (row[column1].ToString() == searchValue2)
                {
                    object currentValue = row[columnToUpdate];
                    row[columnToUpdate] = Convert.ToInt32(currentValue) + 1;
                }
            }

            promptDataTable.Clear();

            DataView sortedView = dt.DefaultView;
            sortedView.Sort = "PromptID ASC";
            sorteddt = sortedView.ToTable();

            var distinctValues = sorteddt.AsEnumerable()
                                .Select(row => row.Field<string>("Prompt"))
                                .Distinct();

            // Add a column to the destination DataTable
            if (promptDataTable.Columns.Contains("Prompt"))
            {
            }
            else
            {
                promptDataTable.Columns.Add("Prompt", typeof(string));
            }

            // Iterate over the distinct values and add them to the destination DataTable
            foreach (var value in distinctValues)
            {
                DataRow newRow1 = promptDataTable.NewRow();
                newRow1["Prompt"] = value;
                promptDataTable.Rows.Add(newRow1);
            }

            dataGridView1.DataSource = promptDataTable;

            dataGridView1.Rows[searchValue2Index + 1].Selected = true;

            int selectedRowIndex = dataGridView1.SelectedCells[0].RowIndex;
            int lastRowIndex = dataGridView1.Rows.Count - 1;
            if (selectedRowIndex > 0)
            {
                upButton.Enabled = true;
            }
            else
            {
                upButton.Enabled = false;
            }
            if (selectedRowIndex == lastRowIndex)
            {
                downButton.Enabled = false;
            }
            else
            {
                downButton.Enabled = true;
            }

            if (dataGridView1.SelectedCells.Count == 1)
            {
                DataGridViewCell currentCell = dataGridView1.SelectedCells[0];
                int currentRowIndex = currentCell.RowIndex;
                int currentColumnIndex = currentCell.ColumnIndex;

                // Ensure that the current cell is not in the first row
                if (currentRowIndex > 0)
                {
                    DataGridViewCell cellAbove = dataGridView1[currentColumnIndex, currentRowIndex - 1];

                    // Select the cell above the current cell
                    dataGridView1.CurrentCell = cellAbove;
                }
            }
        }

        private void downButton_Click(object sender, EventArgs e)
        {
            string column1 = "Prompt"; // Specify the name of the first column
            //string column2 = "PromptID"; // Specify the name of the second column
            string columnToUpdate = "PromptID"; // Specify the name of the column to update

            string searchValue1 = dataGridView1.SelectedCells[0].Value.ToString(); // Specify the value to search for in the first column
            int searchValue2Index = dataGridView1.SelectedCells[0].RowIndex + 1;  // Specify the value to search for in the second column
            string searchValue2 = dataGridView1.Rows[searchValue2Index].Cells["Prompt"].Value.ToString();

            // Update the value in the column for each matching row
            foreach (DataRow row in dt.Rows)
            {
                if (row[column1].ToString() == searchValue1)
                {
                    object currentValue = row[columnToUpdate];
                    row[columnToUpdate] = Convert.ToInt32(currentValue) + 1;
                }
                if (row[column1].ToString() == searchValue2)
                {
                    object currentValue = row[columnToUpdate];
                    row[columnToUpdate] = Convert.ToInt32(currentValue) - 1;
                }
            }

            promptDataTable.Clear();

            DataView sortedView = dt.DefaultView;
            sortedView.Sort = "PromptID ASC";
            sorteddt = sortedView.ToTable();

            var distinctValues = sorteddt.AsEnumerable()
                                .Select(row => row.Field<string>("Prompt"))
                                .Distinct();

            // Add a column to the destination DataTable
            if (promptDataTable.Columns.Contains("Prompt"))
            {
            }
            else
            {
                promptDataTable.Columns.Add("Prompt", typeof(string));
            }

            // Iterate over the distinct values and add them to the destination DataTable
            foreach (var value in distinctValues)
            {
                DataRow newRow1 = promptDataTable.NewRow();
                newRow1["Prompt"] = value;
                promptDataTable.Rows.Add(newRow1);
            }

            dataGridView1.DataSource = promptDataTable;

            dataGridView1.Rows[searchValue2Index].Selected = true;

            int selectedRowIndex = dataGridView1.SelectedCells[0].RowIndex;
            int lastRowIndex = dataGridView1.Rows.Count - 1;
            if (selectedRowIndex > 0)
            {
                upButton.Enabled = true;
            }
            else
            {
                upButton.Enabled = false;
            }
            if (selectedRowIndex == lastRowIndex)
            {
                downButton.Enabled = false;
            }
            else
            {
                downButton.Enabled = true;
            }





        }

        private void upButton2_Click(object sender, EventArgs e)
        {
            // Assuming you have the DataTable "dt" and DataGridViews "dataGridView1" and "dataGridView2"

            // Get the values from the selected cells in DataGridView1 and DataGridView2
            string value1 = dataGridView1.SelectedCells[0].Value.ToString();
            string value2 = dataGridView2.SelectedCells[0].Value.ToString();
            int origPossition = dataGridView2.SelectedCells[0].RowIndex;

            // Find the row index in the DataTable "dt" where the values match
            int rowIndex1 = -1;
            int rowIndex2 = -1;
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                if (dt.Rows[i]["Prompt"].ToString() == value1 && dt.Rows[i]["ComponentSKU"].ToString() == value2)
                {
                    rowIndex1 = i;
                    rowIndex2 = i - 1;
                    break; // We found both rows, so no need to continue the loop
                }
            }

            // Check if both rows were found in the DataTable
            if (rowIndex1 != -1 && rowIndex2 != -1)
            {
                // Swap the OptionSort values between the rows
                int optionSortValue1 = Convert.ToInt32(dt.Rows[rowIndex1]["OptionSort"]);
                int optionSortValue2 = Convert.ToInt32(dt.Rows[rowIndex2]["OptionSort"]);
                dt.Rows[rowIndex1]["OptionSort"] = optionSortValue2;
                dt.Rows[rowIndex2]["OptionSort"] = optionSortValue1;

                // Optionally, if you want to sort the DataTable based on the "OptionSort" column:
                dt.DefaultView.Sort = "PromptID, OptionSort ASC";
                dt = dt.DefaultView.ToTable();

                loadComponents();
                dataGridView2.Rows[origPossition - 1].Selected = true;

                int selectedRowIndex = dataGridView2.SelectedCells[0].RowIndex;
                int lastRowIndex = dataGridView2.Rows.Count - 1;
                if (selectedRowIndex > 0)
                {
                    upButton2.Enabled = true;
                }
                else
                {
                    this.Cursor = Cursors.WaitCursor;
                    CalculatePrices();
                    this.Cursor = Cursors.Default;
                    upButton2.Enabled = false;
                }
                if (selectedRowIndex == lastRowIndex)
                {
                    downButton2.Enabled = false;
                }
                else
                {
                    downButton2.Enabled = true;
                }
            }
        }

        private void downButton2_Click(object sender, EventArgs e)
        {
            // Get the values from the selected cells in DataGridView1 and DataGridView2
            string value1 = dataGridView1.SelectedCells[0].Value.ToString();
            string value2 = dataGridView2.SelectedCells[0].Value.ToString();
            int origPossition = dataGridView2.SelectedCells[0].RowIndex;

            // Find the row index in the DataTable "dt" where the values match
            int rowIndex1 = +1;
            int rowIndex2 = +1;
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                if (dt.Rows[i]["Prompt"].ToString() == value1 && dt.Rows[i]["ComponentSKU"].ToString() == value2)
                {
                    rowIndex1 = i;
                    rowIndex2 = i + 1;
                    break; // We found both rows, so no need to continue the loop
                }
            }

            // Check if both rows were found in the DataTable
            if (rowIndex1 != -1 && rowIndex2 != -1)
            {
                // Swap the OptionSort values between the rows
                int optionSortValue1 = Convert.ToInt32(dt.Rows[rowIndex1]["OptionSort"]);
                int optionSortValue2 = Convert.ToInt32(dt.Rows[rowIndex2]["OptionSort"]);
                dt.Rows[rowIndex1]["OptionSort"] = optionSortValue2;
                dt.Rows[rowIndex2]["OptionSort"] = optionSortValue1;

                // Optionally, if you want to sort the DataTable based on the "OptionSort" column:
                dt.DefaultView.Sort = "PromptID, OptionSort ASC";
                dt = dt.DefaultView.ToTable();

                loadComponents();
                dataGridView2.Rows[origPossition + 1].Selected = true;
            }

            int selectedRowIndex = dataGridView2.SelectedCells[0].RowIndex;
            int lastRowIndex = dataGridView2.Rows.Count - 1;

            if (selectedRowIndex == 1)
            {
                this.Cursor = Cursors.WaitCursor;
                CalculatePrices();
                this.Cursor = Cursors.Default;
            }

            if (selectedRowIndex > 0)
            {
                upButton2.Enabled = true;
            }
            else
            {
                upButton2.Enabled = false;
            }
            if (selectedRowIndex == lastRowIndex)
            {
                downButton2.Enabled = false;
            }
            else
            {
                downButton2.Enabled = true;
            }
        }

        private void deleteButton1_Click(object sender, EventArgs e)
        {
            string column1 = "Prompt"; // Specify the name of the first column
            //string columnToUpdate = "OptionSort"; // Specify the name of the column to update

            string searchValue1 = dataGridView1.SelectedCells[0].Value.ToString(); // Specify the value to search for in the first column

            DataRow[] rowsToUpdate = dt.Select($"{column1} = '{searchValue1}'");

            // Update the value in the column for each matching row
            foreach (DataRow row in rowsToUpdate)
            {
                dt.Rows.Remove(row);
            }
            dataGridView2.DataSource = null;
            loadPrompts();
            renumberPrompts();
        }

        private void renumberPrompts()
        {
            string columnToAssignID = "Prompt"; // Specify the name of the column to assign ID
            string idColumnName = "PromptID"; // Specify the name of the ID column

            // Sorting the DataTable by the "Prompt" column
            DataView dv = dt.DefaultView;
            dv.Sort = "PromptID, OptionSort";
            dt = dv.ToTable();
            

            Dictionary<object, int> valueToIDMap = new Dictionary<object, int>();
            int currentID = 1;

            foreach (DataRow row in dt.Rows)
            {
                object value = row[columnToAssignID];

                if (!valueToIDMap.ContainsKey(value))
                {
                    valueToIDMap[value] = currentID++;
                }

                row[idColumnName] = valueToIDMap[value];
            }
        }

        private void deleteButton2_Click(object sender, EventArgs e)
        {
            string column1 = "ComponentSKU"; // Specify the name of the first column
            //string columnToUpdate = "OptionSort"; // Specify the name of the column to update

            string searchValue1 = dataGridView2.SelectedCells[0].Value.ToString(); // Specify the value to search for in the first column

            DataRow[] rowsToUpdate = dt.Select($"{column1} = '{searchValue1}'");

            // Update the value in the column for each matching row
            foreach (DataRow row in rowsToUpdate)
            {
                dt.Rows.Remove(row);
            }

            loadComponents();
            renumberComponents();
            //identifyBs();
        }

        private void renumberComponents()
        {
            string column1ToAssignID = "Prompt"; // Specify the name of the first column to assign ID
            string column2ToAssignID = "ComponentSKU"; // Specify the name of the second column to assign ID
            string idColumnName = "OptionSort"; // Specify the name of the ID column

            if (promptTextbox.Visible == true)
            {
                searchValue = promptTextbox.Text;
            }
            else
            {
                searchValue = dataGridView1.SelectedCells[0].Value.ToString();
            }

            Dictionary<string, int> combinationToIDMap = new Dictionary<string, int>();
            int currentID = 1;


            // Loop through the desired section of the DataTable
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                DataRow row = dt.Rows[i];

                if (row["Prompt"].ToString() == searchValue)
                {
                    string combination = $"{row[column1ToAssignID]}_{row[column2ToAssignID]}";

                    if (!combinationToIDMap.Equals(combination))
                    {
                        combinationToIDMap[combination] = currentID++;
                    }

                    row[idColumnName] = combinationToIDMap[combination];
                }
            }

            identifyBs();
        }

        private void renumberComponentsAll()
        {
            //string column1ToAssignID = "Prompt"; // Specify the name of the first column to assign ID
            //string column2ToAssignID = "ComponentSKU"; // Specify the name of the second column to assign ID
            //string idColumnName = "OptionSort"; // Specify the name of the ID column

            //Dictionary<string, int> combinationToIDMap = new Dictionary<string, int>();
            //int currentID = 1;

            // Loop through the desired section of the DataTable
            foreach (DataRow prompt in promptDataTable.Rows)
            {
                searchPrompt = prompt[0].ToString();
                q = 1;
                foreach (DataRow row in dt.Rows)
                {
                    if (row["Prompt"].ToString() == searchPrompt)
                    {
                        row["OptionSort"] = q;
                        q++;
                    }
                }

            }
        }

        private void ScaleFormAndControlsToScreen()
        {
            // Get the screen resolution
            Screen screen = Screen.PrimaryScreen;
            int screenWidth = screen.Bounds.Width;
            int screenHeight = screen.Bounds.Height;

            // Calculate the scaling factor based on the screen size
            int scaleFactor = 100; // Adjust this value as needed

            ScaleControl(this, screenWidth, screenHeight, scaleFactor);
        }

        private void ScaleControl(Control control, int screenWidth, int screenHeight, int scaleFactor)
        {
            // Scale control's size
            control.Width = control.Width * scaleFactor / 100;
            control.Height = control.Height * scaleFactor / 100;

            // Scale control's font (if applicable)
            if (control.Font != null)
            {
                float fontSize = control.Font.Size * scaleFactor / 100;
                control.Font = new Font(control.Font.FontFamily, fontSize, control.Font.Style);
            }

            // Scale other control-specific properties as needed

            // Recursively scale child controls
            foreach (Control childControl in control.Controls)
            {
                ScaleControl(childControl, screenWidth, screenHeight, scaleFactor);
            }
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            dataGridView2.DataSource = null;

            OptionCheckbox.Checked = false;
            addOnCheckbox2.Checked = false;
            upgradeCheckbox.Checked = false;
            componentPriceTextbox.Text = "";
            upgradePriceTextbox.Text = "";

            int selectedRowIndex = dataGridView1.SelectedCells[0].RowIndex;
            int lastRowIndex = dataGridView1.Rows.Count - 1;
            if (selectedRowIndex > 0)
            {
                upButton.Enabled = true;
            }
            else
            {
                upButton.Enabled = false;
            }
            if (selectedRowIndex == lastRowIndex)
            {
                downButton.Enabled = false;
            }
            else
            {
                downButton.Enabled = true;
            }

            deleteButton1.Enabled = true;
            prompt_CellContentClick();
            renumberComponents();
            loadComponents();
        }

        private void prompt_CellContentClick()
        {
            if (dt.Rows.Count > 0)
            {

                DataGridViewCell selectedCell = dataGridView1.SelectedCells[0];
                string cellText = selectedCell.Value?.ToString();

                string searchColumn = "Prompt"; // Column to search
                string targetColumn1 = "InputType"; // Column to retrieve text from
                string targetColumn2 = "PromptType"; // Column to retrieve text from

                // Find the row where the searchColumn has the searchValue
                DataRow foundRow = dt.Select($"{searchColumn} = '{cellText}'").FirstOrDefault();

                string input = foundRow.Field<string>(targetColumn1);
                string option = foundRow.Field<string>(targetColumn2);

                //string input = foundRow.Field<string>(targetColumn1);
                //string option = foundRow.Field<string>(targetColumn2);



                //string option = dt.Rows[0]["PromptType"].ToString();
                //string input = dt.Rows[0]["InputType"].ToString();

                switch (input)
                {
                    case "Radio":
                        radioButtonCheckbox.Checked = true;
                        listCheckbox.Checked = false;
                        checkboxCheckbox.Checked = false;
                        break;

                    case "Dropdown":
                        radioButtonCheckbox.Checked = false;
                        listCheckbox.Checked = true;
                        checkboxCheckbox.Checked = false;
                        break;

                    case "Checkbox":
                        radioButtonCheckbox.Checked = false;
                        listCheckbox.Checked = false;
                        checkboxCheckbox.Checked = true;
                        break;
                }

                switch (option)
                {
                    case "A":
                        addOnCheckbox.Checked = true;
                        upgradeCheckbox1.Checked = false;
                        reqOptionCheckbox.Checked = false;
                        break;

                    case "U":
                        addOnCheckbox.Checked = false;
                        upgradeCheckbox1.Checked = true;
                        reqOptionCheckbox.Checked = false;
                        break;

                    case "O":
                        addOnCheckbox.Checked = false;
                        upgradeCheckbox1.Checked = false;
                        reqOptionCheckbox.Checked = true;
                        break;
                }

            }
        }

        private void dataGridView2_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            clickedCell = dataGridView2.Rows[e.RowIndex].Cells[e.ColumnIndex];
            dataGridView2.CurrentCell = clickedCell;
            clickedCell.Selected = true;

            int selectedRowIndex = clickedCell.RowIndex; //dataGridView2.SelectedCells[0].RowIndex;
            int lastRowIndex = dataGridView2.Rows.Count - 1;
            if (selectedRowIndex > 0)
            {
                upButton2.Enabled = true;
            }
            else
            {
                upButton2.Enabled = false;
            }
            if (selectedRowIndex == lastRowIndex)
            {
                downButton2.Enabled = false;
            }
            else
            {
                downButton2.Enabled = true;
            }

            deleteButton2.Enabled = true;

            if (dt.Rows.Count > 0)
            {
                DataGridViewRow selectedCell = dataGridView2.Rows[e.RowIndex];
                string cellText = selectedCell.Cells[0].Value.ToString();
                oldValue = cellText;

                string searchColumn = "ComponentSKU"; // Column to search
                //string targetColumn1 = "InputType"; // Column to retrieve text from
                string targetColumn2 = "LongDesc"; // Column to retrieve text from
                string targetColumn3 = "ComponentPrice";
                string targetColumn4 = "UpgradePrice";
                string targetColumn5 = "Brand";
                string targetColumn6 = "MFR_SKU";

                // Find the row where the searchColumn has the searchValue
                DataRow foundRow = dt.Select($"{searchColumn} = '{cellText}'").FirstOrDefault();

                //string input = foundRow.Field<string>(targetColumn1);
                //string option = foundRow.Field<string>(targetColumn2);
                dataTableIndex = dt.Rows.IndexOf(foundRow);
                decimal compPrice = foundRow.Field<decimal>(targetColumn3);
                decimal upgradePrice = foundRow.Field<decimal>(targetColumn4);
                string longDesc = foundRow.Field<string>(targetColumn2);
                string brand = foundRow.Field<string>(targetColumn5);
                string mfrSKU = foundRow.Field<string>(targetColumn6);

                componentPriceTextbox.Text = compPrice.ToString("0.00");
                upgradePriceTextbox.Text = upgradePrice.ToString("0.00");
                componentDescriptionTextbox.Text = longDesc;
                brandTextbox.Text = brand;
                mfrTextbox.Text = mfrSKU;
            }


            // Retrieve the entire row based on the selected row index
            DataRow selectedRow = dt.Rows[dataTableIndex];

            // Access the "ComponentType" value from the row
            string componentTypeValue = selectedRow["ComponentType"].ToString();
            string inputTypeValue = selectedRow["InputType"].ToString();

            switch (componentTypeValue)
            {
                case "B":
                    OptionCheckbox.Checked = false;
                    upgradeCheckbox.Checked = false;
                    addOnCheckbox2.Checked = false;
                    break;
                case "U":
                    OptionCheckbox.Checked = false;
                    upgradeCheckbox.Checked = true;
                    addOnCheckbox2.Checked = false;
                    break;
                case "A":
                    OptionCheckbox.Checked = false;
                    upgradeCheckbox.Checked = false;
                    addOnCheckbox2.Checked = true;
                    break;
                case "O":
                    OptionCheckbox.Checked = true;
                    upgradeCheckbox.Checked = false;
                    addOnCheckbox2.Checked = false;
                    break;
                case "":
                    OptionCheckbox.Checked = false;
                    upgradeCheckbox.Checked = false;
                    addOnCheckbox2.Checked = false;
                    break;
            }

            switch (inputTypeValue)
            {
                case "Radio":
                    radioButtonCheckbox.Checked = true;
                    listCheckbox.Checked = false;
                    checkboxCheckbox.Checked = false;
                    break;

                case "Checkbox":
                    radioButtonCheckbox.Checked = false;
                    listCheckbox.Checked = false;
                    checkboxCheckbox.Checked = true;
                    break;

                case "Dropdown":
                    radioButtonCheckbox.Checked = false;
                    listCheckbox.Checked = true;
                    checkboxCheckbox.Checked = false;
                    break;

                case "":
                    radioButtonCheckbox.Checked = false;
                    listCheckbox.Checked = false;
                    checkboxCheckbox.Checked = false;
                    break;

            }

        }

        private void radioButtonCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButtonCheckbox.Checked)
            {
                listCheckbox.Checked = false;
                checkboxCheckbox.Checked = false;

                //DataGridViewCell selectedCell = dataGridView1.SelectedCells[0];
                //string targetPrompt = selectedCell.Value?.ToString();

                foreach (DataRow row in dt.Rows)
                {
                    string prompt = row["Prompt"].ToString();

                    if (prompt == dataGridView1.SelectedCells[0].Value.ToString()) //targetPrompt)
                    {
                        row["InputType"] = "Radio";
                    }
                }
                dt.AcceptChanges();
            }
        }

        private void listCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            if (listCheckbox.Checked)
            {
                radioButtonCheckbox.Checked = false;
                checkboxCheckbox.Checked = false;

                //DataGridViewCell selectedCell = dataGridView1.SelectedCells[0];
                //string targetPrompt = selectedCell.Value?.ToString();

                foreach (DataRow row in dt.Rows)
                {
                    string prompt = row["Prompt"].ToString();

                    if (prompt == dataGridView1.SelectedCells[0].Value.ToString())
                    {
                        row["InputType"] = "Dropdown";
                    }
                }
                dt.AcceptChanges();
            }
        }

        private void checkboxCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            if (checkboxCheckbox.Checked)
            {
                radioButtonCheckbox.Checked = false;
                listCheckbox.Checked = false;

                //DataGridViewCell selectedCell = dataGridView1.SelectedCells[0];
                //string targetPrompt = selectedCell.Value?.ToString();

                foreach (DataRow row in dt.Rows)
                {
                    string prompt = row["Prompt"].ToString();

                    if (prompt == dataGridView1.SelectedCells[0].Value.ToString())//targetPrompt)
                    {
                        row["InputType"] = "Checkbox";
                    }
                }
                dt.AcceptChanges();
            }
        }

        private void reqOptionCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            if (reqOptionCheckbox.Checked)
            {
                upgradeCheckbox1.Checked = false;
                addOnCheckbox.Checked = false;

                //DataGridViewCell selectedCell = dataGridView1.SelectedCells[0];
                //string targetPrompt = selectedCell.Value?.ToString();

                foreach (DataRow row in dt.Rows)
                {
                    string prompt = row["Prompt"].ToString();

                    if (prompt == dataGridView1.SelectedCells[0].Value.ToString())//targetPrompt)
                    {
                        row["PromptType"] = "O";

                    }
                }
                dt.AcceptChanges();
            }
        }

        private void upgradeCheckbox1_CheckedChanged(object sender, EventArgs e)
        {
            if (upgradeCheckbox1.Checked)
            {
                reqOptionCheckbox.Checked = false;
                addOnCheckbox.Checked = false;

                DataGridViewCell selectedCell = dataGridView1.SelectedCells[0];
                string targetPrompt = selectedCell.Value?.ToString();

                foreach (DataRow row in dt.Rows)
                {
                    string prompt = row["Prompt"].ToString();

                    if (prompt == targetPrompt)
                    {
                        row["PromptType"] = "U";

                    }
                }
                dt.AcceptChanges();
            }
        }

        private void addonCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            if (addOnCheckbox.Checked)
            {
                reqOptionCheckbox.Checked = false;
                upgradeCheckbox1.Checked = false;

                DataGridViewCell selectedCell = dataGridView1.SelectedCells[0];
                string targetPrompt = selectedCell.Value?.ToString();

                foreach (DataRow row in dt.Rows)
                {
                    string prompt = row["Prompt"].ToString();

                    if (prompt == targetPrompt)
                    {
                        row["PromptType"] = "A";

                    }
                }
                dt.AcceptChanges();
            }
        }

        private void addPromptButton_Click(object sender, EventArgs e)
        {
            if (sellPriceTextbox.Text == "")
            {
                MessageBox.Show("This kit must have a price before you can add any prompts/components!", "Warning");
                sellPriceTextbox.Focus();
                return;
            }

            label12.Visible = true;
            promptTextbox.Visible = true;
            label23.Visible = true;
            radioButtonCheckbox2.Visible = true;
            listCheckbox2.Visible = true;
            checkboxCheckbox2.Visible = true;
            label22.Visible = true;
            reqOptionCheckbox2.Visible = true;
            upgradeCheckbox2.Visible = true;
            addOnCheckbox3.Visible = true;
            label21.Visible = true;
            addComponentTextbox.Visible = true;
            addButton2.Visible = true;
            cancelButton.Visible = true;

            label24.Visible = false;
            brandTextbox.Visible = false;
            label17.Visible = false;
            componentPriceTextbox.Visible = false;
            addPromptButton.Visible = false;
            label16.Visible = false;
            upgradePriceTextbox.Visible = false;
            label19.Visible = false;
            componentDescriptionTextbox.Visible = false;
            label5.Visible = false;
            partSearchTextbox.Visible = false;
            searchButton.Visible = false;

            okButton.Enabled = false;
            updatePriceButton.Enabled = false;
            viewKitButton.Enabled = false;

        }

        private void searchButton_Click(object sender, EventArgs e)
        {
            this.Cursor = Cursors.WaitCursor;
            GetPartData();
            //renumberComponents();
            CalculatePrices();
            this.Cursor = Cursors.Default;
            partSearchTextbox.Text = "";
        }

        private void GetPartData()
        {
            partSearchDT.Clear();
            SqlConnection conn = new System.Data.SqlClient.SqlConnection(Helper.ConnString("AUTOPART"));
            {
                if (conn.State == ConnectionState.Open)
                {
                    conn.Close();
                }
                conn.Open();
                //DataSet ds1 = new DataSet();
                SqlDataAdapter adapter = new SqlDataAdapter(
                $@"
                    SELECT KeyCode, [Desc] FROM Product WHERE KeyCode LIKE '{partSearchTextbox.Text}%' OR word5 LIKE '{partSearchTextbox.Text}%' ORDER BY KeyCode
                   ", conn);
                adapter.Fill(partSearchDT);

                if (partSearchDT.Rows.Count == 0)
                {
                    MessageBox.Show("No matching parts found!");
                    this.Cursor = Cursors.Default;
                    return;
                }

                if (partSearchDT.Rows.Count == 1)
                {
                    partNumber = partSearchDT.Rows[0]["KeyCode"].ToString();
                    description = partSearchDT.Rows[0]["Desc"].ToString();
                    conn.Close();
                    found = true;
                    partSearch();
                }
                else
                {
                    partSearchDT.Clear();
                    if (conn.State == ConnectionState.Open)
                    {
                        conn.Close();
                    }

                    conn.Open();
                    SqlDataAdapter adapter1 = new SqlDataAdapter(
                    $@"
                    SELECT KeyCode, [Desc] FROM Product WHERE KeyCode LIKE '%{partSearchTextbox.Text}%' OR word5 LIKE '%{partSearchTextbox.Text}%' ORDER BY KeyCode
                   ", conn);
                    adapter1.Fill(partSearchDT);

                    if (partSearchDT.Rows.Count > 1)
                    {
                        dataGridView3.DataSource = null;
                        dataGridView3.AutoGenerateColumns = false;
                        dataGridView3.ColumnCount = 2;

                        dataGridView3.Columns[0].Name = "KeyCode";
                        dataGridView3.Columns[0].HeaderText = "KeyCode";
                        dataGridView3.Columns[0].DataPropertyName = "KeyCode";

                        dataGridView3.Columns[1].HeaderText = "Desc";
                        dataGridView3.Columns[1].Name = "Desc";
                        dataGridView3.Columns[1].DataPropertyName = "Desc";

                        dataGridView3.DataSource = partSearchDT;
                        dataGridView3.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
                        conn.Close();
                        dataGridView3.Visible = true;
                        dataGridView3.CurrentCell.Selected = false;
                        found = false;
                        //return;
                    }
                }
            }
            renumberComponents();
            partSearchTextbox.Text = "";
        }

        private void partSearch()
        {
            addPart2();
        }

        private void dataGridView3_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            addPart();
        }

        private void loadComponents()
        {
            searchButton.Enabled = true;
            itemsDataTable.Clear();
            if (promptTextbox.Visible == false)
            {
                DataGridViewCell selectedCell = dataGridView1.SelectedCells[0];
                cellText = selectedCell.Value?.ToString();
            }
            else
            {
                cellText = promptTextbox.Text;
            }


            DataView sortedView = dt.DefaultView;
            sortedView.Sort = "PromptID ASC, OptionSort ASC";
            sorteddt = sortedView.ToTable();

            // Add the specific column to the destination DataTable
            string columnName = "ComponentSKU"; // Specify the column name
            //string sourcePromptName = "Prompt";
            //string sourceComponent = "ComponentSKU";
            if (itemsDataTable.Columns.Contains(columnName))
            {
                //do nothing
            }
            else
            {
                itemsDataTable.Columns.Add("ComponentSKU", typeof(string));
                itemsDataTable.Columns.Add("Desc", typeof(string));
                itemsDataTable.Columns.Add("ComponentQty", typeof(int));
            }

            foreach (DataRow sourceRow in sorteddt.Rows)
            {
                if (sourceRow[6].ToString() == cellText)
                {
                    DataRow destinationRow = itemsDataTable.NewRow();

                    // Copy the values from the specific columns
                    destinationRow["ComponentSKU"] = sourceRow["ComponentSKU"];
                    destinationRow["Desc"] = sourceRow["LongDesc"];
                    destinationRow["ComponentQty"] = sourceRow["ComponentQty"];

                    // Add the new row to the destination DataTable
                    itemsDataTable.Rows.Add(destinationRow);
                }
            }

            dataGridView2.DataSource = itemsDataTable;
            dataGridView2.ClearSelection();

            foreach (DataGridViewColumn column in dataGridView2.Columns)
            {
                column.SortMode = DataGridViewColumnSortMode.NotSortable;
            }

            //prompt_CellContentClick();
        }

        private void viewKitButton_Click(object sender, EventArgs e)
        {
            DataView sortedView = dt.DefaultView;
            sortedView.Sort = "PromptID ASC, OptionSort ASC";
            sorteddt = sortedView.ToTable();

            if (dataGridView4.Visible == false)
            {
                dataGridView4.ClearSelection();
                dataGridView4.DataSource = sorteddt;
                dataGridView4.Visible = true;
                viewKitButton.Text = "Hide Kit";
                quitButton.Enabled = false;
                updatePriceButton.Enabled = false;
                okButton.Enabled = false;
                upButton.Enabled = false;
                upButton2.Enabled = false;
                /*refreshButton.Enabled = false;*/
                downButton.Enabled = false;
                downButton2.Enabled = false;
                deleteButton1.Enabled = false;
                deleteButton2.Enabled = false;
                radioButtonCheckbox.Enabled = false;
                listCheckbox.Enabled = false;
                checkboxCheckbox.Enabled = false;
                reqOptionCheckbox.Enabled = false;
                upgradeCheckbox1.Enabled = false;
                addOnCheckbox.Enabled = false;
                OptionCheckbox.Enabled = false;
                upgradeCheckbox.Enabled = false;
                addOnCheckbox2.Enabled = false;
            }
            else
            {
                dataGridView4.Visible = false;
                viewKitButton.Text = "Review Kit";
                quitButton.Enabled = true;
                okButton.Enabled = true;
                updatePriceButton.Enabled = true;
                upButton.Enabled = true;
                upButton2.Enabled = true;
                downButton.Enabled = true;
                downButton2.Enabled = true;
                deleteButton1.Enabled = true;
                deleteButton2.Enabled = true;
                radioButtonCheckbox.Enabled = true;
                listCheckbox.Enabled = true;
                checkboxCheckbox.Enabled = true;
                reqOptionCheckbox.Enabled = true;
                upgradeCheckbox1.Enabled = true;
                addOnCheckbox.Enabled = true;
                OptionCheckbox.Enabled = true;
                upgradeCheckbox.Enabled = true;
                addOnCheckbox2.Enabled = true;
            }

        }

        private void partSearchTextbox_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                this.Cursor = Cursors.WaitCursor;
                GetPartData();
                //renumberComponents();
                CalculatePrices();
                this.Cursor = Cursors.Default;
                partSearchTextbox.Text = "";
            }
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            int countNullCategories = 0;
            foreach (DataRow row in dt.Rows)
            {
                if (row["KitCategory"].ToString() == "")
                {
                    countNullCategories++;
                }
            }

            if (lev1Textbox.Text.Contains("/"))
            {
                MessageBox.Show("Level1 can not have a '/' in it. Please remove, and try again.");
                return;
            }
            if (lev2Textbox.Text.Contains("/"))
            {
                MessageBox.Show("Level2 can not have a '/' in it. Please remove, and try again.");
                return;
            }
            if (lev3Textbox.Text.Contains("/"))
            {
                MessageBox.Show("Level3 can not have a '/' in it. Please remove, and try again.");
                return;
            }

            if (countNullCategories > 0)
            {
                MessageBox.Show("not all components have a kit category. Please fix these before saving this kit");
                return;
            }

            SqlConnection connection = new SqlConnection(Helper.ConnString("AUTOPART"));
            {
                connection.Open();

                string sql = $"SELECT * FROM Meri_Kit_Info WHERE InternalSKU != '{kitNumberTextbox.Text}' AND Display_Desc = '{kitDescriptionTextBox.Text}'";

                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    object result = command.ExecuteScalar();

                    if (result != null)
                    {
                        MessageBox.Show("A kit already exists with that description. Please give this kit a unique description!");
                        kitDescriptionTextBox.Focus();
                        return;
                    }
                }
            }


            if (Convert.ToDecimal(textbox1.Text) > Convert.ToDecimal(sellPriceTextbox.Text))
            {
                MessageBox.Show("The cost of the kit can not be greater than the selling price of the kit!");
                return;
            }

            if (lev1Textbox.Text == "")
            {
                MessageBox.Show("Level1 can not be blank");
                return;
            }

            if (lev2Textbox.Text == "")
            {
                MessageBox.Show("Level2 can not be blank");
                return;
            }

            if (lev3Textbox.Text == "")
            {
                MessageBox.Show("Level3 can not be blank");
                return;
            }

            this.Cursor = Cursors.WaitCursor;
            CalculatePrices();

            foreach (DataRow row in dt.Rows)
            {
                if (Convert.ToDouble(row["ComponentPrice"]) == 0.00)
                {
                    this.Cursor = Cursors.Default;
                    DialogResult dialogResult = MessageBox.Show($@"Part number {row["ComponentSKU"].ToString()} has a price of $0.00. Is it ok to continue?", "Price Check", MessageBoxButtons.YesNo);
                    if (dialogResult == DialogResult.No)
                    {
                        return;
                    }
                    else
                    {
                        this.Cursor = Cursors.WaitCursor;
                    }
                }
            }

            foreach (DataRow row in dt.Rows)
            {
                if (row["PromptType"].ToString() == "" || row["InputType"].ToString() == "")
                {
                    this.Cursor = Cursors.Default;
                    MessageBox.Show($@"Either the Prompt Type or the Input Type for part number {row["ComponentSKU"].ToString()} is blank. Please review the data and try again.");
                    return;
                }
            }

            foreach (DataRow row in dt.Rows)
            {
                if (row["ComponentType"].ToString() == "")
                {
                    this.Cursor = Cursors.Default;
                    MessageBox.Show($@"The Component Type for part number {row["ComponentSKU"]} in prompt {row["Prompt"]} is blank. Please review the data and try again.");
                    return;
                }
            }

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


            foreach (DataRow row in dt.Rows)
            {
                using (SqlCommand cmd = new SqlCommand("sp_MERI_UpdateKitCategories", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.Add("@KitCat", SqlDbType.VarChar).Value = row["KitCategory"].ToString();

                    cmd.Parameters.Add("@Part", SqlDbType.VarChar).Value = row["ComponentSKU"].ToString();

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


            foreach (DataRow row in dt.Rows)
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

                    cmd.Parameters.Add("@Margin", SqlDbType.VarChar).Value = Convert.ToDecimal(marginTextbox.Text);


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

            foreach (DataRow row in dt.Rows)
            {
                using (SqlCommand cmd = new SqlCommand("sp_MERI_updateKitComponentfromApparatus", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.Add("@ComponentSKU", SqlDbType.VarChar).Value = row["ComponentSKU"].ToString();
                    cmd.Parameters.Add("@displayDescription", SqlDbType.VarChar).Value = row["LongDesc"].ToString();
                    cmd.Parameters.Add("@MFR_SKU", SqlDbType.VarChar).Value = row["MFR_SKU"].ToString();
                    cmd.Parameters.Add("@brand", SqlDbType.VarChar).Value = row["Brand"].ToString();
                    cmd.Parameters.Add("@Lev1", SqlDbType.VarChar).Value = row["Level1"].ToString();
                    cmd.Parameters.Add("@Lev2", SqlDbType.VarChar).Value = row["Level2"].ToString();
                    cmd.Parameters.Add("@Lev3", SqlDbType.VarChar).Value = row["Level3"].ToString();

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

                cmd.Parameters.Add("@KitPrice", SqlDbType.Money).Value = Convert.ToDecimal(sellPriceTextbox.Text);

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

            using (SqlCommand cmd = new SqlCommand("sp_MERI_updateKitDetailsfromApparatus", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add("@KitSKU", SqlDbType.VarChar).Value = part;

                cmd.Parameters.Add("@Make", SqlDbType.VarChar).Value = makeTextBox.Text;

                cmd.Parameters.Add("@Model", SqlDbType.VarChar).Value = ModelTextbox.Text;

                cmd.Parameters.Add("@Lev1", SqlDbType.VarChar).Value = lev1Textbox.Text;

                cmd.Parameters.Add("@Lev2", SqlDbType.VarChar).Value = lev2Textbox.GetItemText(lev2Textbox.SelectedItem);

                cmd.Parameters.Add("@Lev3", SqlDbType.VarChar).Value = lev3Textbox.Text;

                cmd.Parameters.Add("@Description", SqlDbType.VarChar).Value = kitDescriptionTextBox.Text;

                cmd.Parameters.Add("@StartYear", SqlDbType.VarChar).Value = yearTextbox1.Text;

                cmd.Parameters.Add("@EndYear", SqlDbType.VarChar).Value = yearTextbox2.Text;

                cmd.Parameters.Add("@StockingIndicator", SqlDbType.VarChar).Value = "1";


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

            this.Cursor = Cursors.Default;
            MessageBox.Show("Done!");
            resetForm();
            kitNumberTextbox.Focus();
        }

        private void dataGridView3_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                dataGridView3.DataSource = null;
                dataGridView3.Visible = false;
                return;
            }

            if (e.KeyCode == Keys.Enter)
            {
                addPart();
            }
        }

        private void CalculatePrices()
        {
            if (dt.Rows.Count > 0)
            {
                identifyBs();

                dataGridView2.ClearSelection();

                decimal sumForB = 0;
                decimal sumForB2 = 0;
                decimal desiredSum = Convert.ToDecimal(sellPriceTextbox.Text);
                decimal KitCost = Convert.ToDecimal(textbox1.Text);
                decimal sellPrice = Convert.ToDecimal(sellPriceTextbox.Text);
                decimal percent = 0;


                foreach (DataRow row in dt.Rows)
                {
                    if (row["ComponentType"].ToString() == "B")
                    {
                        sumForB += Convert.ToDecimal(row["ComponentA2"]) * Convert.ToInt32(row["ComponentQty"]);
                    }
                }


                foreach (DataRow row in dt.Rows)
                {
                    if (row["ComponentType"].ToString() == "B")
                    {
                        bool isLastBRow = row == dt.AsEnumerable()
                                                .LastOrDefault(r => r["ComponentType"].ToString() == "B");

                        if (!isLastBRow)
                        {
                            if (row["ComponentType"].ToString() == "B")
                            {
                                percent = sellPrice / KitCost;
                                //sumForB += Convert.ToDecimal(row["ComponentA2"]) * Convert.ToInt32(row["ComponentQty"]);
                            }

                            decimal a2Value = Convert.ToDecimal(row["ComponentA2"]) * Convert.ToInt32(row["ComponentQty"]);
                            decimal percentage = (a2Value / sumForB);
                            int Qty = Convert.ToInt32(row["ComponentQty"]);

                            row["ComponentPrice"] = Math.Round((Convert.ToDecimal(row["ComponentCost"]) * percent), 2);
                        }
                        else
                        {
                            foreach (DataRow row1 in dt.Rows)
                            {
                                int Qty = Convert.ToInt32(row1["ComponentQty"]);
                                if (row1["ComponentType"].ToString() == "B")
                                {
                                    bool isLastBRow1 = row1 == dt.AsEnumerable()
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
                            row["ComponentPrice"] = Math.Round((desiredSum - sumForB2) / Convert.ToInt32(row["ComponentQty"]), 2);
                        }
                    }
                }

                foreach (DataRow row in dt.Rows)
                {
                    if (row["ComponentType"].ToString() == "A")
                    {
                        row["ComponentPrice"] = Math.Round(Convert.ToDecimal(row["componenta2"]), 2);
                    }
                }

                //renumberComponents();

                /*var commonPrices = new Dictionary<string, decimal>();

                foreach (DataRow row in dt.Rows)
                {
                    string prompt = row["Prompt"].ToString();
                    int optionSort = Convert.ToInt32(row["OptionSort"]);
                    decimal componentPrice = Convert.ToDecimal(row["ComponentPrice"]);
                    string componentType = row["ComponentType"].ToString();

                    if (optionSort == 1 && componentType == "B")
                    {
                        commonPrices[prompt] = componentPrice;
                    }
                    else
                    {
                        if (componentType == "O" && optionSort != 1)
                        {
                            row["ComponentPrice"] = commonPrices[prompt];
                        }
                    }
                }*/


                var commonPrices = new Dictionary<string, decimal>();

                foreach (DataRow row in dt.Rows)
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
                foreach (DataRow row in dt.Rows)
                {
                    string prompt = row["Prompt"].ToString();
                    int optionSort = Convert.ToInt32(row["OptionSort"]);
                    string componentType = row["ComponentType"].ToString();

                    if (componentType == "O" && optionSort != 1 && commonPrices.ContainsKey(prompt))
                    {
                        row["ComponentPrice"] = commonPrices[prompt];
                    }
                }


                foreach (DataRow row in dt.Rows)
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
                                        

                                        /*if (ComponentSKU == "FEDH345DCP")
                                        {
                                            MessageBox.Show("stop");
                                        }*/

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
                                        string kitCategory = Interaction.InputBox($@"The Kit category for part number {ComponentSKU} was not found. Please specify a kit category", "enter a kit category", "MIS");

                                        string query1 = $@"SELECT MUMin, MUPercent 
                                                        FROM MERI_KitMarkups 
                                                        WHERE [Type] = '{kitCategory}'";

                                        using (SqlCommand command1 = new SqlCommand(query1, conn))
                                        {
                                            try
                                            {
                                                conn.Open();
                                                SqlDataReader reader1 = command1.ExecuteReader();
                                                reader1.Read();

                                                if (reader1.HasRows)
                                                {
                                                    SqlConnection conn1 = new SqlConnection(Helper.ConnString("AUTOPART"));
                                                    using (SqlCommand cmd = new SqlCommand("sp_MERI_InsertKitComponentIntoProdVals", conn1))
                                                    {
                                                        cmd.CommandType = CommandType.StoredProcedure;

                                                        cmd.Parameters.Add("@PartNo", SqlDbType.VarChar).Value = ComponentSKU;

                                                        cmd.Parameters.Add("@Attribute", SqlDbType.VarChar).Value = "Kit Category";

                                                        cmd.Parameters.Add("@Value", SqlDbType.VarChar).Value = kitCategory;

                                                        cmd.Parameters.Add("@Presentation", SqlDbType.VarChar).Value = "T";

                                                        cmd.Parameters.Add("@ListOfValues", SqlDbType.VarChar).Value = "";

                                                        try
                                                        {
                                                            conn1.Open();
                                                            cmd.ExecuteNonQuery();
                                                            conn1.Close();
                                                            cmd.Parameters.Clear();
                                                        }
                                                        catch (Exception ex)
                                                        {
                                                            MessageBox.Show(ex.Message, "Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                                            return;
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
                                                    conn.Close();
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

                updateMargin();
            }
        }

        private void updatePriceButton_Click(object sender, EventArgs e)
        {
            this.Cursor = Cursors.WaitCursor;
            CalculatePrices();
            if (errorStatus == false)
            {
                MessageBox.Show("Prices have been recalculated");
            }
            else
            {
                errorStatus = false;
            }
            this.Cursor = Cursors.Default;
        }

        private void sellPriceTextbox_TextChanged(object sender, PreviewKeyDownEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                if (dt.Rows.Count == 0)
                {
                    MessageBox.Show("Cannot recalculate the price until items are added to this kit!");
                    return;
                }
                this.Cursor = Cursors.WaitCursor;
                updateMargin();
                CalculatePrices();
                this.Cursor = Cursors.Default;
            }
        }

        private void OptionCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            if (OptionCheckbox.Checked == true)
            {
                addOnCheckbox2.Checked = false;
                upgradeCheckbox.Checked = false;

                DataGridViewCell selectedCell = dataGridView2.SelectedCells[0];
                string cellText = selectedCell.Value?.ToString();

                DataGridViewCell selectedCell2 = dataGridView1.SelectedCells[0];
                string cellText1 = selectedCell2.Value?.ToString();

                string searchColumn = "ComponentSKU";
                string searchColumn2 = "Prompt";

                // Find the row where the searchColumn has the searchValue
                DataRow[] foundRow = dt.Select($"{searchColumn} = '{cellText}' AND {searchColumn2} = '{cellText1}'");

                if (foundRow.Length > 0)
                {
                    foundRow[0]["ComponentType"] = "O";
                    dataGridView4.DataSource = null;
                    dataGridView4.DataSource = dt;
                }
            }
        }

        private void upgradeCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            if (upgradeCheckbox.Checked == true)
            {
                OptionCheckbox.Checked = false;
                addOnCheckbox2.Checked = false;

                DataGridViewCell selectedCell = dataGridView2.SelectedCells[0];
                string cellText = selectedCell.Value?.ToString();

                DataGridViewCell selectedCell2 = dataGridView1.SelectedCells[0];
                string cellText1 = selectedCell2.Value?.ToString();

                string searchColumn = "ComponentSKU";
                string searchColumn2 = "Prompt";

                // Find the row where the searchColumn has the searchValue
                DataRow[] foundRow = dt.Select($"{searchColumn} = '{cellText}' AND {searchColumn2} = '{cellText1}'");

                if (foundRow.Length > 0)
                {
                    foundRow[0]["ComponentType"] = "U";
                    dataGridView4.DataSource = null;
                    dataGridView4.DataSource = dt;
                }
            }
        }

        private void addOnCheckbox2_CheckedChanged(object sender, EventArgs e)
        {
            if (addOnCheckbox2.Checked == true)
            {
                OptionCheckbox.Checked = false;
                upgradeCheckbox.Checked = false;

                DataGridViewCell selectedCell = dataGridView2.SelectedCells[0];
                string cellText = selectedCell.Value?.ToString();

                DataGridViewCell selectedCell2 = dataGridView1.SelectedCells[0];
                string cellText1 = selectedCell2.Value?.ToString();

                string searchColumn = "ComponentSKU";
                string searchColumn2 = "Prompt";

                // Find the row where the searchColumn has the searchValue
                DataRow[] foundRow = dt.Select($"{searchColumn} = '{cellText}' AND {searchColumn2} = '{cellText1}'");

                if (foundRow.Length > 0)
                {
                    foundRow[0]["ComponentType"] = "A";
                    dataGridView4.DataSource = null;
                    dataGridView4.DataSource = dt;
                }
            }
        }

        private void identifyBs()
        {
            renumberPrompts();
            foreach (DataRow row in dt.Rows)
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

            decimal sumForB = 0;
            foreach (DataRow row in dt.Rows)
            {
                if (row["ComponentType"].ToString() == "B")
                {
                    decimal componentCost = Convert.ToDecimal(row["ComponentQty"]) * Convert.ToDecimal(row["ComponentCost"]);
                    sumForB += componentCost;
                }
            }

            textbox1.Text = Math.Round(sumForB, 2).ToString();

        }

        private void dataGridView1_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && e.ColumnIndex >= 0)
            {
                DataGridView dataGridView = (DataGridView)sender;
                DataGridViewRow editedRow = dataGridView.Rows[e.RowIndex];
                DataGridViewCell editedCell = editedRow.Cells[e.ColumnIndex];

                // Get the new value of the edited cell
                object newValue = editedCell.Value;

                // Update all matching cells in the DataTable
                string columnName = dataGridView.Columns[e.ColumnIndex].Name;
                foreach (DataRow row in dt.Rows)
                {
                    if (row[columnName].Equals(oldValue))
                    {
                        row[columnName] = newValue;
                    }
                }
            }
        }

        private void dataGridView1_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            if (e.RowIndex >= 0 && e.ColumnIndex >= 0)
            {
                DataGridView dataGridView = (DataGridView)sender;
                oldValue = dataGridView.Rows[e.RowIndex].Cells[e.ColumnIndex].Value;
            }
        }

        private void marginTextbox_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (e.KeyCode == Keys.Enter || e.KeyCode == Keys.Tab)
            {
                double kitCost = Convert.ToDouble(textbox1.Text);
                double kitMargin = Convert.ToDouble(marginTextbox.Text);
                kitMargin = kitMargin / 100;
                kitMargin = 1 - kitMargin;

                sellPriceTextbox.Text = Convert.ToString(Math.Round(kitCost / kitMargin));
                CalculatePrices();
            }
        }

        private void sellPriceTextbox_Leave(object sender, EventArgs e)
        {
            CurrentTextbox = (TextBox)sender;
            CurrentTextbox.BackColor = Color.Empty;

            if (textbox1.Text != "")
            {
                double kitCost = Convert.ToDouble(textbox1.Text);
                double kitSell = Convert.ToDouble(sellPriceTextbox.Text);
                marginTextbox.Text = Convert.ToString(Math.Round((kitSell - kitCost) / kitSell, 2));
                identifyBs();
                CalculatePrices();
            }
        }

        private void marginTextbox_Enter(object sender, EventArgs e)
        {
            CurrentTextbox = (TextBox)sender;
            CurrentTextbox.BackColor = Color.Yellow;
            marginTextbox.SelectAll();
            //marginTextbox.Focus();
        }

        private void updateMargin()
        {
            double kitCost = Convert.ToDouble(textbox1.Text);
            double kitSell = Convert.ToDouble(sellPriceTextbox.Text);
            marginTextbox.Text = Convert.ToString(Math.Round((kitSell - kitCost) / kitSell, 4) * 100);
        }

        private void addButton2_Click(object sender, EventArgs e)
        {
            addComponent();
            return;
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            label12.Visible = false;
            promptTextbox.Visible = false;
            label23.Visible = false;
            radioButtonCheckbox2.Visible = false;
            listCheckbox2.Visible = false;
            checkboxCheckbox2.Visible = false;
            label22.Visible = false;
            reqOptionCheckbox2.Visible = false;
            upgradeCheckbox2.Visible = false;
            addOnCheckbox3.Visible = false;
            label21.Visible = false;
            addComponentTextbox.Visible = false;
            addButton2.Visible = false;
            cancelButton.Visible = false;

            label24.Visible = true;
            brandTextbox.Visible = true;
            label17.Visible = true;
            componentPriceTextbox.Visible = true;
            addPromptButton.Visible = true;
            label16.Visible = true;
            upgradePriceTextbox.Visible = true;
            label19.Visible = true;
            componentDescriptionTextbox.Visible = true;
            label5.Visible = true;
            partSearchTextbox.Visible = true;
            searchButton.Visible = true;

            okButton.Enabled = true;
            updatePriceButton.Enabled = true;
            viewKitButton.Enabled = true;

            promptTextbox.Text = "";
            addComponentTextbox.Text = "";
            radioButtonCheckbox2.Checked = false;
            checkboxCheckbox2.Checked = false;
            reqOptionCheckbox2.Checked = false;
            upgradeCheckbox2.Checked = false;
            addOnCheckbox3.Checked = false;
            listCheckbox2.Checked = false;
        }

        private void addComponentTextbox_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                addComponent();
                return;
            }
        }

        private void addComponent()
        {
            addPart3();
        }

        private void radioButtonCheckbox2_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButtonCheckbox2.Checked == true)
            {
                listCheckbox2.Checked = false;
                checkboxCheckbox2.Checked = false;
            }
        }

        private void listCheckbox2_CheckedChanged(object sender, EventArgs e)
        {
            if (listCheckbox2.Checked == true)
            {
                radioButtonCheckbox2.Checked = false;
                checkboxCheckbox2.Checked = false;
            }
        }

        private void checkboxCheckbox2_CheckedChanged(object sender, EventArgs e)
        {
            if (checkboxCheckbox2.Checked == true)
            {
                listCheckbox2.Checked = false;
                radioButtonCheckbox2.Checked = false;
            }
        }

        private void reqOptionCheckbox2_CheckedChanged(object sender, EventArgs e)
        {
            if (reqOptionCheckbox2.Checked == true)
            {
                upgradeCheckbox2.Checked = false;
                addOnCheckbox3.Checked = false;
            }
        }

        private void upgradeCheckbox2_CheckedChanged(object sender, EventArgs e)
        {
            if (upgradeCheckbox2.Checked == true)
            {
                reqOptionCheckbox2.Checked = false;
                addOnCheckbox3.Checked = false;
            }
        }

        private void addOnCheckbox3_CheckedChanged(object sender, EventArgs e)
        {
            if (addOnCheckbox3.Checked == true)
            {
                reqOptionCheckbox2.Checked = false;
                upgradeCheckbox2.Checked = false;
            }
        }

        private void addPromptButton_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (sellPriceTextbox.Text == "")
            {
                MessageBox.Show("This kit must have a price before you can add any prompts/components!");
                sellPriceTextbox.Focus();
                return;
            }

            if (e.KeyCode == Keys.Enter)
            {
                label12.Visible = true;
                promptTextbox.Visible = true;
                label23.Visible = true;
                radioButtonCheckbox2.Visible = true;
                listCheckbox2.Visible = true;
                checkboxCheckbox2.Visible = true;
                label22.Visible = true;
                reqOptionCheckbox2.Visible = true;
                upgradeCheckbox2.Visible = true;
                addOnCheckbox3.Visible = true;
                label21.Visible = true;
                addComponentTextbox.Visible = true;
                addButton2.Visible = true;
                cancelButton.Visible = true;

                label17.Visible = false;
                componentPriceTextbox.Visible = false;
                addPromptButton.Visible = false;
                label16.Visible = false;
                upgradePriceTextbox.Visible = false;
                label19.Visible = false;
                componentDescriptionTextbox.Visible = false;
                label5.Visible = false;
                partSearchTextbox.Visible = false;
                searchButton.Visible = false;

                okButton.Enabled = false;
                updatePriceButton.Enabled = false;
                viewKitButton.Enabled = false;
            }
        }

        private void radioButtonCheckbox2_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                if (radioButtonCheckbox2.Checked)
                {
                    radioButtonCheckbox2.Checked = false;
                }
                else
                {
                    radioButtonCheckbox2.Checked = true;
                }
            }
        }

        private void listCheckbox2_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                if (listCheckbox2.Checked)
                {
                    listCheckbox2.Checked = false;
                }
                else
                {
                    listCheckbox2.Checked = true;
                }
            }
        }

        private void checkboxCheckbox2_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                if (checkboxCheckbox2.Checked)
                {
                    checkboxCheckbox2.Checked = false;
                }
                else
                {
                    checkboxCheckbox2.Checked = true;
                }
            }
        }

        private void reqOptionCheckbox2_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                if (reqOptionCheckbox2.Checked)
                {
                    reqOptionCheckbox2.Checked = false;
                }
                else
                {
                    reqOptionCheckbox2.Checked = true;
                }
            }
        }

        private void upgradeCheckbox2_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                if (upgradeCheckbox2.Checked)
                {
                    upgradeCheckbox2.Checked = false;
                }
                else
                {
                    upgradeCheckbox2.Checked = true;
                }
            }
        }

        private void addOnCheckbox3_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                if (addOnCheckbox3.Checked)
                {
                    addOnCheckbox3.Checked = false;
                }
                else
                {
                    addOnCheckbox3.Checked = true;
                }
            }
        }

        private void closeButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        public void resetForm()
        {
            Action<Control.ControlCollection> func = null;
            dataGridView1.ClearSelection();
            dataTableIndex = 0;
            dataGridView1.DataSource = null;
            dataGridView2.DataSource = null;
            dt.Clear();
            sorteddt.Clear();
            promptDataTable.Clear();
            itemsDataTable.Clear();
            partSearchDT.Clear();
            priceSearch.Clear();
            dataTable.Clear();
            lev2Textbox.DataSource = null;

            foreach (Control cBox in this.Controls)
            {
                if (cBox is CheckBox)
                {
                    ((CheckBox)cBox).Checked = false;
                }
            }

            func = (controls) =>
            {
                foreach (Control control in controls)
                    if (control is TextBox)
                        (control as TextBox).Clear();
                    else
                        func(control.Controls);
            };

            func(Controls);
        }

        private void refreshButton_Click(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;
            renumberComponentsAll();
            Cursor.Current = default;
        }

        private void dataGridView2_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            object newValue = dataGridView2.Rows[e.RowIndex].Cells[2].Value;

            string componentSKU = dataGridView2.Rows[e.RowIndex].Cells["ComponentSKU"].Value.ToString();
            string prompt = dataGridView1.SelectedCells[0].Value.ToString();

            // Use LINQ to find the row(s) with the specific "ComponentSKU" and "Prompt" values
            DataRow[] foundRows = dt.Select($"ComponentSKU = '{componentSKU}' AND Prompt = '{prompt}'");

            // Iterate through the found rows and update the "ComponentQty" column
            foreach (DataRow row in foundRows)
            {
                row["ComponentQty"] = newValue;
            }
        }

        private void addEngineCatButton_Click(object sender, EventArgs e)
        {
            string engineCat = Interaction.InputBox($@"Please provide your new engine category", "Add a new Engine Category");
            if (engineCat == "")
            {
                return;
            }
            else
            {
                SqlConnection conn = new SqlConnection(Helper.ConnString("AUTOPART"));

                using (SqlCommand cmd = new SqlCommand("sp_MERI_UpdateEngineCategories", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.Add("@EngineCat", SqlDbType.VarChar).Value = engineCat;

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
            }
        }

        private void addPart()
        {
            var sortedSelectedCells = dataGridView3.SelectedCells.Cast<DataGridViewCell>()
                .OrderBy(cell => cell.RowIndex).ThenBy(cell => cell.ColumnIndex);
            foreach (DataGridViewCell cells in sortedSelectedCells)
            {
                //partSearchTextbox.Text = cells.Value.ToString();
                string partNumber = cells.Value.ToString();
                int Qty = 1;
                string description = cells.Value.ToString();

                DataRow newTableRow = dt.NewRow();

                priceSearch.Clear();
                SqlConnection conn = new SqlConnection(Helper.ConnString("AUTOPART"));
                {
                    if (conn.State == ConnectionState.Open)
                    {
                        conn.Close();
                    }
                    conn.Open();
                    //DataSet ds1 = new DataSet();
                    SqlDataAdapter adapter = new SqlDataAdapter(
                    $@"
                    SELECT   I.A1 AS ComponentA2, II.A1 AS ComponentCost, P.word5, V.Value
                    FROM         Mvpr AS I INNER JOIN
                         Mvpr AS II ON I.SubKey1 = II.SubKey1 AND I.Prefix = II.Prefix LEFT OUTER JOIN
                         Product AS P ON I.SubKey1 = P.KeyCode INNER JOIN
                         ProdVals V ON P.KeyCode = V.Part
                    WHERE     (I.Prefix = 'S') AND (I.SubKey1 = '{partNumber}') AND (I.SubKey2 = 2) AND (II.SubKey2 = 27) AND (V.Attribute = 'Kit Category')
                   ", conn);
                    adapter.Fill(priceSearch);
                    if (priceSearch.Rows.Count > 0)
                    {
                        A2 = priceSearch.Rows[0]["ComponentA2"].ToString();
                        cost = priceSearch.Rows[0]["ComponentCost"].ToString();
                        word5 = priceSearch.Rows[0]["word5"].ToString();
                        KitCat = priceSearch.Rows[0]["Value"].ToString();
                    }
                    if (conn.State == ConnectionState.Open)
                    {
                        conn.Close();
                    }
                }

                // Set values for each column in the new row
                int startIndex = 3;
                int length = partNumber.Length - 3;
                newTableRow["KitSKU"] = part;
                newTableRow["ComponentSKU"] = partNumber;
                newTableRow["ComponentQty"] = Qty;
                newTableRow["MFR_SKU"] = partNumber.Substring(startIndex, length);
                newTableRow["KitCategory"] = KitCat;

                SqlConnection conn1 = new SqlConnection(Helper.ConnString("AUTOPART"));
                {
                    if (conn1.State == ConnectionState.Open)
                    {
                        conn1.Close();
                    }
                    conn1.Open();
                    //DataSet ds1 = new DataSet();
                    SqlDataAdapter adapter = new SqlDataAdapter($@"
                    SELECT   [Internal SKU]
                    FROM     VW_MERI_NAP_Export
                    WHERE     [Internal SKU] = '{partNumber}' AND [SKU Description] = '{description}'
                   ", conn1);
                    adapter.Fill(dtDescr);
                    conn1.Close();
                }

                if(dtDescr.Rows.Count > 0) 
                {
                    newTableRow["LongDesc"] = description + " " + partNumber.Substring(startIndex, length);
                }
                else
                {
                    newTableRow["LongDesc"] = description;
                }

                dtDescr.Clear();

                newTableRow["OptionSort"] = dataGridView2.Rows.Count + 1;

                if (promptTextbox.Visible == true)
                {
                    newTableRow["Prompt"] = promptTextbox.Text;
                    searchValue = promptTextbox.Text;

                    if (radioButtonCheckbox2.Checked)
                    {
                        newTableRow["InputType"] = "Radio";
                    }
                    if (checkboxCheckbox2.Checked)
                    {
                        newTableRow["InputType"] = "Checkbox";
                    }
                    if (listCheckbox2.Checked)
                    {
                        newTableRow["InputType"] = "Dropdown";
                    }

                    if (reqOptionCheckbox2.Checked)
                    {
                        newTableRow["PromptType"] = "O";
                        newTableRow["ComponentType"] = "O";
                    }
                    if (addOnCheckbox3.Checked)
                    {
                        newTableRow["PromptType"] = "A";
                        newTableRow["ComponentType"] = "A";
                    }
                    if (upgradeCheckbox2.Checked)
                    {
                        newTableRow["PromptType"] = "U";
                        newTableRow["ComponentType"] = "U";
                    }
                }
                else
                {
                    newTableRow["Prompt"] = dataGridView1.SelectedCells[0].Value.ToString();
                    searchValue = dataGridView1.SelectedCells[0].Value.ToString();

                    if (radioButtonCheckbox.Checked)
                    {
                        newTableRow["InputType"] = "Radio";
                    }
                    if (checkboxCheckbox.Checked)
                    {
                        newTableRow["InputType"] = "Checkbox";
                    }
                    if (listCheckbox.Checked)
                    {
                        newTableRow["InputType"] = "Dropdown";
                    }

                    if (reqOptionCheckbox.Checked)
                    {
                        newTableRow["PromptType"] = "O";
                        newTableRow["ComponentType"] = "O";
                    }
                    if (addOnCheckbox.Checked)
                    {
                        newTableRow["PromptType"] = "A";
                        newTableRow["ComponentType"] = "A";
                    }
                    if (upgradeCheckbox1.Checked)
                    {
                        newTableRow["PromptType"] = "U";
                        newTableRow["ComponentType"] = "U";
                    }
                }

                newTableRow["Make"] = makeTextBox.Text;
                newTableRow["Model"] = ModelTextbox.Text;
                newTableRow["StartYear"] = yearTextbox1.Text;
                newTableRow["EndYear"] = yearTextbox2.Text;
                newTableRow["Level1"] = lev1Textbox.Text;
                newTableRow["Level2"] = lev2Textbox.Text;
                newTableRow["Level3"] = lev3Textbox.Text;
                newTableRow["Description"] = kitDescriptionTextBox.Text;
                newTableRow["word5"] = word5;
                newTableRow["KitPrice"] = sellPriceTextbox.Text;
                newTableRow["ComponentA2"] = A2;
                newTableRow["ComponentCost"] = cost;
                newTableRow["ComponentPrice"] = "0.00";
                newTableRow["UpgradePrice"] = Convert.ToDecimal("0.00");

                //renumberPrompts();

                if (promptTextbox.Visible == false)
                {
                    searchValue = dataGridView1.SelectedCells[0].Value.ToString(); // Specify the search value
                }
                else
                {
                    searchValue = promptTextbox.Text;
                }

                //string searchValue = dataGridView1.SelectedCells[0].Value.ToString(); // Specify the search value
                string columnToSearch = "Prompt"; // Specify the column to search in
                string columnToRetrieve = "PromptID"; // Specify the column to retrieve the value from

                // Find the first row that matches the condition
                DataRow selectedRow = dt.Select($"{columnToSearch} = '{searchValue}'").FirstOrDefault();
                //loadPrompts();
                if (selectedRow != null)
                {
                    object value = selectedRow[columnToRetrieve];
                    newTableRow["PromptID"] = value.ToString();
                }

                //string table = newRow.Table.ToString();
                dt.Rows.Add(newTableRow);
                renumberPrompts();
                loadPrompts();
                //loadComponents();
            }
            if (promptTextbox.Visible == true)
            {
                label12.Visible = false;
                promptTextbox.Visible = false;
                label23.Visible = false;
                radioButtonCheckbox2.Visible = false;
                listCheckbox2.Visible = false;
                checkboxCheckbox2.Visible = false;
                label22.Visible = false;
                reqOptionCheckbox2.Visible = false;
                upgradeCheckbox2.Visible = false;
                addOnCheckbox3.Visible = false;
                label21.Visible = false;
                addComponentTextbox.Visible = false;
                addButton2.Visible = false;
                cancelButton.Visible = false;

                label24.Visible = true;
                brandTextbox.Visible = true;
                label17.Visible = true;
                componentPriceTextbox.Visible = true;
                addPromptButton.Visible = true;
                label16.Visible = true;
                upgradePriceTextbox.Visible = true;
                label19.Visible = true;
                componentDescriptionTextbox.Visible = true;
                label5.Visible = true;
                partSearchTextbox.Visible = true;
                searchButton.Visible = true;

                okButton.Enabled = true;
                updatePriceButton.Enabled = true;
                viewKitButton.Enabled = true;

                promptTextbox.Text = "";
                addComponentTextbox.Text = "";
                radioButtonCheckbox2.Checked = false;
                checkboxCheckbox2.Checked = false;
                reqOptionCheckbox2.Checked = false;
                upgradeCheckbox2.Checked = false;
                addOnCheckbox3.Checked = false;
                listCheckbox2.Checked = false;


            }

            loadPrompts();
            renumberPrompts();

            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                foreach (DataGridViewCell cell in row.Cells)
                {
                    if (cell.Value != null && cell.Value.ToString() == searchValue)
                    {
                        row.Selected = true;

                        string targetPrompt = searchValue;

                        DataRow[] foundRows = dt.Select($"Prompt = '{targetPrompt}'");

                        if (foundRows.Length > 0)
                        {
                            promptType = foundRows[0]["PromptType"].ToString();
                            inputType = foundRows[0]["InputType"].ToString();
                        }

                        if (inputType == "Radio")
                        {
                            radioButtonCheckbox.Checked = true;
                        }

                        if (inputType == "Dropdown")
                        {
                            listCheckbox.Checked = true;
                        }

                        if (inputType == "Checkbox")
                        {
                            checkboxCheckbox.Checked = true;
                        }

                        if (promptType == "U")
                        {
                            upgradeCheckbox1.Checked = true;
                        }

                        if (promptType == "O")
                        {
                            reqOptionCheckbox.Checked = true;
                        }

                        if (promptType == "A")
                        {
                            addOnCheckbox.Checked = true;
                        }
                    }
                }
            }

            renumberComponents();
            loadComponents();
            CalculatePrices();
            loadPrompts();
            found = true;
            dataGridView3.Visible = false;
        }

        private void addPart2()
        {
            DataRow newRow2 = dt.NewRow();

            // Set values for each column in the new row
            newRow2["KitSKU"] = part;
            newRow2["Prompt"] = dataGridView1.SelectedCells[0].Value.ToString();
            newRow2["ComponentSKU"] = partNumber;
            newRow2["ComponentQty"] = Qty;
            newRow2["LongDesc"] = description;
            newRow2["OptionSort"] = dataGridView2.Rows.Count + 1;

            priceSearch.Clear();
            SqlConnection conn = new System.Data.SqlClient.SqlConnection(Helper.ConnString("AUTOPART"));
            {
                if (conn.State == ConnectionState.Open)
                {
                    conn.Close();
                }
                conn.Open();
                //DataSet ds1 = new DataSet();
                SqlDataAdapter adapter = new SqlDataAdapter(
                $@"
                    SELECT 
                        I.A1 AS ComponentA2, 
                        II.A1 AS ComponentCost, 
                        P.word5, 
                        V.Value
                    FROM 
                        Mvpr AS I 
                        INNER JOIN Mvpr AS II ON I.SubKey1 = II.SubKey1 AND I.Prefix = II.Prefix 
                        LEFT OUTER JOIN Product AS P ON I.SubKey1 = P.KeyCode 
                        LEFT OUTER JOIN ProdVals V ON P.KeyCode = V.Part AND V.Attribute = 'Kit Category'
                    WHERE 
                        I.Prefix = 'S' AND 
                        I.SubKey1 = '{partNumber}' AND 
                        I.SubKey2 = 2 AND 
                        II.SubKey2 = 27
                   ", conn);
                adapter.Fill(priceSearch);
                if (priceSearch.Rows.Count > 0)
                {
                    A2 = priceSearch.Rows[0]["ComponentA2"].ToString();
                    cost = priceSearch.Rows[0]["ComponentCost"].ToString();
                    word5 = priceSearch.Rows[0]["word5"].ToString();
                    KitCat = priceSearch.Rows[0]["Value"].ToString();
                }

                if (KitCat == null || KitCat == "")
                {
                    MessageBox.Show("New component does not have a Kit Category! Please provide one before saving this kit.");
                }

                if (conn.State == ConnectionState.Open)
                {
                    conn.Close();
                }
            }

            if (radioButtonCheckbox.Checked)
            {
                newRow2["InputType"] = "Radio";
            }
            if (checkboxCheckbox.Checked)
            {
                newRow2["InputType"] = "Checkbox";
            }
            if (listCheckbox.Checked)
            {
                newRow2["InputType"] = "Dropdown";
            }

            if (reqOptionCheckbox.Checked)
            {
                newRow2["PromptType"] = "O";
            }
            if (addOnCheckbox.Checked)
            {
                newRow2["PromptType"] = "A";
            }
            if (upgradeCheckbox1.Checked)
            {
                newRow2["PromptType"] = "U";
            }
            newRow2["Make"] = makeTextBox.Text;
            newRow2["Model"] = ModelTextbox.Text;
            newRow2["StartYear"] = yearTextbox1.Text;
            newRow2["EndYear"] = yearTextbox2.Text;
            newRow2["Level1"] = lev1Textbox.Text;
            newRow2["Level2"] = lev2Textbox.Text;
            newRow2["Level3"] = lev3Textbox.Text;
            newRow2["Description"] = kitDescriptionTextBox.Text;
            newRow2["word5"] = word5;
            newRow2["KitPrice"] = sellPriceTextbox.Text;
            newRow2["KitCategory"] = KitCat;
            try
            {
                if (A2 == "" || A2 == null)
                {
                    newRow2["ComponentA2"] = 0.00;
                    MessageBox.Show("New component does not have an A2 price set against it!");
                }
                else
                {
                    newRow2["ComponentA2"] = A2;
                }
            }
            catch (Exception ex)
            {
                // Handle any errors that might have occurred
                MessageBox.Show("An error occurred: " + ex.Message);
            }
            try
            {
                if (cost == "" ||cost == null)
                {
                    newRow2["ComponentCost"] = 0.00;
                    MessageBox.Show("New component does not have a cost set against it!");
                }
                else
                {
                    newRow2["ComponentCost"] = cost;
                }
            }
            catch (Exception ex)
            {
                // Handle any errors that might have occurred
                MessageBox.Show("An error occurred: " + ex.Message);
            }
            //newRow2["ComponentCost"] = cost;
            newRow2["ComponentPrice"] = "0.00";
            newRow2["UpgradePrice"] = Convert.ToDecimal("0.00");

            int startIndex = 3;
            int length = partNumber.Length - 3;
            newRow2["MFR_SKU"] = partNumber.Substring(startIndex, length);

            SqlConnection conn1 = new SqlConnection(Helper.ConnString("AUTOPART"));
            {
                if (conn1.State == ConnectionState.Open)
                {
                    conn1.Close();
                }
                conn1.Open();
                //DataSet ds1 = new DataSet();
                SqlDataAdapter adapter = new SqlDataAdapter($@"
                    SELECT   [Internal SKU]
                    FROM     VW_MERI_NAP_Export
                    WHERE     [Internal SKU] = '{partNumber}' AND [SKU Description] = '{description}'
                   ", conn1);
                adapter.Fill(dtDescr);
                conn1.Close();
            }

            if (dtDescr.Rows.Count > 0)
            {
                newRow2["LongDesc"] = description + " " + partNumber.Substring(startIndex, length);
            }
            else
            {
                newRow2["LongDesc"] = description;
            }

            dtDescr.Clear();

            string searchValue = dataGridView1.SelectedCells[0].Value.ToString();
            string columnToSearch = "Prompt";
            string columnToRetrieve = "PromptID";

            DataRow selectedRow = dt.Select($"{columnToSearch} = '{searchValue}'").FirstOrDefault();

            if (selectedRow != null)
            {
                object value = selectedRow[columnToRetrieve];
                newRow2["PromptID"] = value.ToString();
            }

            dt.Rows.Add(newRow2);

            loadComponents();

            found = true;
            dataGridView3.Visible = false;
            partSearchTextbox.Text = "";
        }

        private void addPart3()
        {
            if (addComponentTextbox.Text == "")
            {
                MessageBox.Show("You must add a component!");
                return;
            }

            if (promptTextbox.Text == "")
            {
                MessageBox.Show("You must add a prompt!");
                return;
            }

            if (radioButtonCheckbox2.Checked == false && listCheckbox2.Checked == false && checkboxCheckbox2.Checked == false)
            {
                MessageBox.Show("You must select a website appearance!");
                return;
            }

            if (reqOptionCheckbox2.Checked == false && upgradeCheckbox2.Checked == false && addOnCheckbox3.Checked == false)
            {
                MessageBox.Show("You must select a prompt type!");
                return;
            }

            string partNo = addComponentTextbox.Text;
            partNo.ToUpper();

            SqlConnection conn = new SqlConnection(Helper.ConnString("AUTOPART"));
            {
                if (conn.State == ConnectionState.Open)
                {
                    conn.Close();
                }

                partSearchDT.Clear();

                if (conn.State == ConnectionState.Open)
                {
                    conn.Close();
                }
                conn.Open();
                //DataSet ds1 = new DataSet();
                SqlDataAdapter adapter = new SqlDataAdapter(
                $@"
                    SELECT P. KeyCode, P.[Desc], V.Value FROM Product P INNER JOIN ProdVals V ON P.KeyCode = V.Part WHERE (P.KeyCode LIKE '%{partNo}%' OR P.Word5 LIKE '%{partNo}%') AND V.Attribute = 'Kit Category' ORDER BY P.KeyCode
                                ", conn);
                adapter.Fill(partSearchDT);
                if (partSearchDT.Rows.Count == 1)
                {
                    DataRow newRow3 = dt.NewRow();
                    partNumber = partSearchDT.Rows[0]["KeyCode"].ToString();
                    description = partSearchDT.Rows[0]["Desc"].ToString();
                    string KitCategory = partSearchDT.Rows[0]["Value"].ToString();
                    conn.Close();

                    newRow3["KitSKU"] = part;
                    newRow3["Prompt"] = promptTextbox.Text;
                    newRow3["ComponentSKU"] = partNo;
                    newRow3["ComponentQty"] = Qty;
                    newRow3["Description"] = description;
                    newRow3["OptionSort"] = dataGridView2.Rows.Count + 1;
                    newRow3["KitCategory"] = KitCategory;

                    priceSearch.Clear();
                    {
                        if (conn.State == ConnectionState.Open)
                        {
                            conn.Close();
                        }
                        conn.Open();
                        //DataSet ds1 = new DataSet();
                        SqlDataAdapter adapter1 = new SqlDataAdapter(
                        $@"
                                SELECT I.A1 as ComponentA2, II.A1 as ComponentCost FROM MVPR I INNER JOIN MVPR II ON I.SubKey1 = II.SubKey1 AND I.Prefix = II.Prefix WHERE I.Prefix = 'S' AND I.SubKey1 = '{partNumber}' AND I.SubKey2 = 2 AND II.SubKey2 = 27
                                ", conn);
                        adapter1.Fill(priceSearch);
                        if (priceSearch.Rows.Count > 0)
                        {
                            A2 = priceSearch.Rows[0]["ComponentA2"].ToString();
                            cost = priceSearch.Rows[0]["ComponentCost"].ToString();
                        }
                        if (conn.State == ConnectionState.Open)
                        {
                            conn.Close();
                        }
                    }

                    if (radioButtonCheckbox2.Checked)
                    {
                        newRow3["InputType"] = "Radio";
                    }
                    if (checkboxCheckbox2.Checked)
                    {
                        newRow3["InputType"] = "Checkbox";
                    }
                    if (listCheckbox2.Checked)
                    {
                        newRow3["InputType"] = "Dropdown";
                    }


                    if (reqOptionCheckbox2.Checked)
                    {
                        newRow3["PromptType"] = "O";
                    }
                    if (upgradeCheckbox2.Checked)
                    {
                        newRow3["PromptType"] = "U";
                    }
                    if (addOnCheckbox3.Checked)
                    {
                        newRow3["PromptType"] = "A";
                    }

                    //newRow3["LongDesc"] = description;
                    newRow3["ComponentType"] = "B";
                    newRow3["Make"] = makeTextBox.Text;
                    newRow3["Model"] = ModelTextbox.Text;
                    newRow3["StartYear"] = yearTextbox1.Text;
                    newRow3["EndYear"] = yearTextbox2.Text;
                    newRow3["Level1"] = lev1Textbox.Text;
                    newRow3["Level2"] = lev2Textbox.Text;
                    newRow3["Level3"] = lev3Textbox.Text;
                    newRow3["Description"] = kitDescriptionTextBox.Text;
                    newRow3["word5"] = word5;
                    newRow3["KitPrice"] = Convert.ToDecimal(sellPriceTextbox.Text);
                    newRow3["ComponentA2"] = A2;
                    newRow3["ComponentCost"] = cost;
                    newRow3["ComponentPrice"] = "0.00";
                    newRow3["UpgradePrice"] = Convert.ToDecimal("0.00");

                    int startIndex = 3;
                    int length = partNumber.Length - 3;
                    newRow3["MFR_SKU"] = partNumber.Substring(startIndex, length);

                    SqlConnection conn1 = new SqlConnection(Helper.ConnString("AUTOPART"));
                    {
                        if (conn1.State == ConnectionState.Open)
                        {
                            conn1.Close();
                        }
                        conn1.Open();
                        //DataSet ds1 = new DataSet();
                        SqlDataAdapter adapter1 = new SqlDataAdapter($@"
                    SELECT   [Internal SKU]
                    FROM     VW_MERI_NAP_Export
                    WHERE     [Internal SKU] = '{partNumber}' AND [SKU Description] = '{description}'
                   ", conn1);
                        adapter1.Fill(dtDescr);
                        conn1.Close();
                    }

                    if (dtDescr.Rows.Count > 0)
                    {
                        newRow3["LongDesc"] = description + " " + partNumber.Substring(startIndex, length);
                    }
                    else
                    {
                        newRow3["LongDesc"] = description;
                    }

                    dtDescr.Clear();

                    dt.Rows.Add(newRow3);
                    //loadPrompts();
                    if (promptTextbox.Visible == true)
                    {
                        prompt = promptTextbox.Text;
                    }
                    else
                    {
                        prompt = dataGridView1.SelectedCells[0].Value.ToString();
                    }

                    foreach (DataGridViewRow row in dataGridView1.Rows)
                    {
                        foreach (DataGridViewCell cell in row.Cells)
                        {
                            if (cell.Value != null && cell.Value.ToString().Equals(prompt))
                            {
                                row.Cells[0].Selected = true;

                                string targetPrompt = dataGridView1.SelectedCells[0].Value.ToString(); // The prompt value you want to search for

                                DataRow[] foundRows = dt.Select($"Prompt = '{targetPrompt}'");

                                if (foundRows.Length > 0)
                                {
                                    promptType = foundRows[0]["PromptType"].ToString();
                                    inputType = foundRows[0]["InputType"].ToString();
                                }

                                if (inputType == "Radio")
                                {
                                    radioButtonCheckbox.Checked = true;
                                }

                                if (inputType == "Dropdown")
                                {
                                    listCheckbox.Checked = true;
                                }

                                if (inputType == "Checkbox")
                                {
                                    checkboxCheckbox.Checked = true;
                                }

                                if (promptType == "U")
                                {
                                    upgradeCheckbox1.Checked = true;
                                }

                                if (promptType == "O")
                                {
                                    reqOptionCheckbox.Checked = true;
                                }

                                if (promptType == "A")
                                {
                                    addOnCheckbox.Checked = true;
                                }
                            }
                        }
                    }

                    renumberPrompts();
                    loadPrompts();
                    loadComponents();
                    renumberComponents();
                    identifyBs();


                    // The value you're searching for

                    foreach (DataGridViewRow row in dataGridView1.Rows)
                    {
                        foreach (DataGridViewCell cell in row.Cells)
                        {
                            if (cell.Value != null && cell.Value.ToString() == prompt)
                            {
                                row.Selected = true;
                                // Optionally, scroll the DataGridView to make the selected row visible
                                dataGridView1.FirstDisplayedScrollingRowIndex = row.Index;
                                break; // We found the value, so no need to continue looping
                            }
                        }
                    }

                }

                if (partSearchDT.Rows.Count > 1)
                {
                    dataGridView3.DataSource = null;
                    dataGridView3.AutoGenerateColumns = false;
                    dataGridView3.ColumnCount = 2;

                    dataGridView3.Columns[0].Name = "KeyCode";
                    dataGridView3.Columns[0].HeaderText = "KeyCode";
                    dataGridView3.Columns[0].DataPropertyName = "KeyCode";

                    dataGridView3.Columns[1].HeaderText = "Desc";
                    dataGridView3.Columns[1].Name = "Desc";
                    dataGridView3.Columns[1].DataPropertyName = "Desc";

                    dataGridView3.DataSource = partSearchDT;
                    dataGridView3.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
                    conn.Close();
                    dataGridView3.Visible = true;
                    dataGridView3.CurrentCell.Selected = false;
                    found = false;
                    return;
                }
                if (partSearchDT.Rows.Count == 0)
                {
                    MessageBox.Show("No matching parts found!");
                    return;
                }
            }

            if (conn.State == ConnectionState.Open)
            {
                conn.Close();
            }

            //loadPrompts();
            CalculatePrices();

            label12.Visible = false;
            promptTextbox.Visible = false;
            label23.Visible = false;
            radioButtonCheckbox2.Visible = false;
            listCheckbox2.Visible = false;
            checkboxCheckbox2.Visible = false;
            label22.Visible = false;
            reqOptionCheckbox2.Visible = false;
            upgradeCheckbox2.Visible = false;
            addOnCheckbox3.Visible = false;
            label21.Visible = false;
            addComponentTextbox.Visible = false;
            addButton2.Visible = false;
            cancelButton.Visible = false;

            label24.Visible = true;
            brandTextbox.Visible = true;
            label17.Visible = true;
            componentPriceTextbox.Visible = true;
            addPromptButton.Visible = true;
            label16.Visible = true;
            upgradePriceTextbox.Visible = true;
            label19.Visible = true;
            componentDescriptionTextbox.Visible = true;
            label5.Visible = true;
            partSearchTextbox.Visible = true;
            searchButton.Visible = true;

            okButton.Enabled = true;
            updatePriceButton.Enabled = true;
            viewKitButton.Enabled = true;

            promptTextbox.Text = "";
            addComponentTextbox.Text = "";
            radioButtonCheckbox2.Checked = false;
            checkboxCheckbox2.Checked = false;
            reqOptionCheckbox2.Checked = false;
            upgradeCheckbox2.Checked = false;
            addOnCheckbox3.Checked = false;
            listCheckbox2.Checked = false;
        }

        private void mfrTextbox_TextChanged(object sender, EventArgs e)
        {
            if(mfrTextbox.Text == "")
            {
                return;
            }
            DataGridViewCell selectedCell = dataGridView2.SelectedCells[0];
            string cellText = selectedCell.Value?.ToString();

            DataGridViewCell selectedCell2 = dataGridView1.SelectedCells[0];
            string cellText1 = selectedCell2.Value?.ToString();

            string searchColumn = "ComponentSKU";
            string searchColumn2 = "Prompt";

            // Find the row where the searchColumn has the searchValue
            DataRow[] foundRow = dt.Select($"{searchColumn} = '{cellText}' AND {searchColumn2} = '{cellText1}'");

            if (foundRow.Length > 0)
            {
                foundRow[0]["MFR_SKU"] = mfrTextbox.Text.ToString();
            }
        }

        private void urlTextbox_TextChanged(object sender, EventArgs e)
        {
            foreach (DataRow row in dt.Rows)
            {
                row["ComponentURL"] = urlTextbox.Text.ToString();
            }
        }

        private void dataGridView2_SelectionChanged(object sender, EventArgs e)
        {
            if (dataGridView2.CurrentRow != null)
            {
                string selectedValue = "";
                //string selectedPart = Convert.ToString(dataGridView2.CurrentRow.Cells[0].Value);

                foreach (DataRow row in dt.Rows)
                {
                    string part = row["ComponentSKU"].ToString();

                    if (part == dataGridView2.CurrentRow.Cells[0].Value.ToString()) //targetPart)
                    {
                        selectedValue = row["KitCategory"].ToString();
                    }
                }

                if (comboBox1.Items.Contains(selectedValue))
                {
                    comboBox1.SelectedItem = selectedValue;
                }
                else
                {
                    comboBox1.SelectedIndex = -1; // Clear selection if value is not found
                }
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            string selectedValue = comboBox1.SelectedItem.ToString();
            string selectedPart = Convert.ToString(dataGridView2.CurrentRow.Cells[0].Value);


            foreach (DataRow row in dt.Rows)
            {
                //selectedPart = row["ComponentSKU"].ToString();

                if (row["ComponentSKU"].ToString() == selectedPart) //targetPart)
                {
                    row["KitCategory"] = selectedValue;
                }
            }
            dt.AcceptChanges();
        }
    }
}