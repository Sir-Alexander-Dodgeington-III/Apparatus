using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.Graph.Drives.Item.Items.Item.Workbook.Functions.ReplaceB;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Xml;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TreeView;

namespace ZZZ_FRT_Lines
{
    public partial class KitCustomizer : Form
    {
        public string part;
        private TextBox CurrentTextbox = null;
        public DataTable dt = new DataTable();
        public DataTable alts = new DataTable();
        public DataTable dt1 = new DataTable();
        public DataTable web = new DataTable();
        public DataTable dtPartMatch = new DataTable();
        public DataTable dtKitMatch = new DataTable();
        public DataTable sorteddt = new DataTable();
        public DataTable promptDataTable = new DataTable();
        public DataTable itemsDataTable = new DataTable();
        public DataTable finalDataTable = new DataTable();
        public int Qty = 1;
        public bool firstLoad;
        public string searchValue;
        public string cellText;
        public int dataTableIndex;
        public DataGridViewCell clickedCell;
        public string searchPrompt;
        public string searchCategory;
        public DataRow foundRow;
        public int selectedRowIndex = -1;
        private DataGridViewCell selectedCell;
        public int seqno;
        public string webPrefix;
        public string webPad;
        public string DocNum;
        public string formattedDate;
        public int attempts;
        public string stringNewValue;
        int rowIndex;
        public KitCustomizer()
        {
            InitializeComponent();
            this.CenterToParent();
            //AdjustControlScale();

            foreach (System.Windows.Forms.Control control in this.Controls)
            {
                if (control is DataGridView)
                {
                    DataGridView gridView = (DataGridView)control;
                    //gridView.Rows.Clear();
                    //gridView.Columns.Clear();
                    gridView.DefaultCellStyle.Font = new System.Drawing.Font("Arial", 10F, FontStyle.Regular, GraphicsUnit.Point);
                    gridView.ColumnHeadersDefaultCellStyle.Font = new System.Drawing.Font("Arial", 10F, FontStyle.Bold, GraphicsUnit.Point);
                    gridView.Refresh();
                }
            }


            dataGridView1.GotFocus += DataGridView1_GotFocus;
            dataGridView1.LostFocus += DataGridView1_LostFocus;

            dataGridView2.GotFocus += DataGridView2_GotFocus;
            dataGridView2.LostFocus += DataGridView2_LostFocus;

            dataGridView3.SelectionChanged += DataGridView3_SelectionChanged;
            removeButton.Enabled = false;

            //getWebDocs();

        }

        private void getWebDocs()
        {
            SqlConnection connection1 = new SqlConnection(Helper.ConnString("AUTOPART"));

            connection1.Open();
            SqlDataAdapter adptr = new SqlDataAdapter(
            $@"
                Select Prefix, Seqno, SeqnoSize FROM DocNumbers NOLOCK WHERE Branch = 'BR48' AND Document LIKE '%WEB%'
            "
            , connection1);
            adptr.Fill(web);
            connection1.Close();


            SqlConnection conn = new SqlConnection(Helper.ConnString("AUTOPART"));

            using (SqlCommand cmd = new SqlCommand("sp_MERI_UpdateWebDoc", conn))
            {
                try
                {
                    conn.Open();
                    cmd.ExecuteNonQuery();
                    conn.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }

            webPrefix = web.Rows[0]["Prefix"].ToString();
            seqno = Convert.ToInt32(web.Rows[0]["Seqno"].ToString());
            webPad = web.Rows[0]["SeqnoSize"].ToString();

            DocNum = $"{webPrefix}{seqno:D6}";
            label12.Text = DocNum;
        }
        private void DataGridView3_SelectionChanged(object sender, EventArgs e)

        {
            if (!dataGridView5.Visible)
            {
                // Check if any row is selected
                if (dataGridView3.SelectedRows.Count > 0)
                {
                    // Get the index of the first selected row
                    selectedRowIndex = dataGridView3.SelectedRows[0].Index;
                }
                // If no row is selected, check if any cell is selected
                else if (dataGridView3.SelectedCells.Count > 0)
                {
                    // Get the index of the row containing the first selected cell
                    selectedRowIndex = dataGridView3.SelectedCells[0].RowIndex;
                }
                else
                {
                    // No row or cell is selected
                    selectedRowIndex = -1;
                }
            }
        }

        private void DataGridView1_GotFocus(object sender, EventArgs e)
        {
            removeButton.Enabled = false;
        }

        private void DataGridView1_LostFocus(object sender, EventArgs e)
        {
            removeButton.Enabled = false;
        }

        private void DataGridView2_GotFocus(object sender, EventArgs e)
        {
            removeButton.Enabled = false;
        }

        private void DataGridView2_LostFocus(object sender, EventArgs e)
        {
            removeButton.Enabled = false;
        }

        private void colorActiveTextbox_Leave(object sender, EventArgs e)
        {
            CurrentTextbox = (TextBox)sender;
            CurrentTextbox.BackColor = System.Drawing.Color.Empty;
        }

        private void colorActiveTextbox_Enter(object sender, EventArgs e)
        {
            CurrentTextbox = (TextBox)sender;
            CurrentTextbox.BackColor = System.Drawing.Color.Yellow;
        }

        private void kitNumber_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (e.KeyCode == Keys.Enter || e.KeyCode == Keys.Tab)
            {
                part = kitNumberTextbox.Text;
                searchPart();
                //loadDataTable();

                if(dataGridView4.Visible == true)
                {
                    this.Cursor = Cursors.Default;
                    return;
                }

                loadPrompts();

                firstLoad = false;

                this.Cursor = Cursors.Default;

            }
        }

        private void searchPart()
        {
            this.Cursor = Cursors.WaitCursor;
            dtKitMatch.Columns.Clear();
            dtKitMatch.Rows.Clear();
            dt.Columns.Clear();
            dt.Rows.Clear();
            dataGridView1.DataSource = null;
            dataGridView1.Rows.Clear();
            dataGridView2.DataSource = null;
            dataGridView2.Rows.Clear();
            dataGridView3.DataSource = null;
            dataGridView3.Rows.Clear();
            dataGridView4.DataSource = null;
            dataGridView4.Rows.Clear();
            dataGridView5.DataSource = null;
            dataGridView5.Rows.Clear();
            part = kitNumberTextbox.Text;
            part = part.ToUpper();
            firstLoad = true;


            SqlConnection connection1 = new SqlConnection(Helper.ConnString("AUTOPART"));

            connection1.Open();
            SqlDataAdapter adptr = new SqlDataAdapter(
            $@"
                    DECLARE @part VARCHAR(MAX)
                    SET @part = '{part}'                             
                
                    SELECT KeyCode, [Desc] FROM Product (NOLOCK) WHERE KeyCode = '{part}'"
            , connection1);
            adptr.Fill(dtKitMatch);
            connection1.Close();

            if (dtKitMatch.Rows.Count == 1)
            {
                kitNumberTextbox.Text = dtKitMatch.Rows[0][0].ToString();
                kitDescriptionTextbox.Text = dtKitMatch.Rows[0][1].ToString();
                loadDataTable();
                return;
            }


            if (dtKitMatch.Rows.Count == 0)
            {
                connection1.Open();
                SqlDataAdapter adptr2 = new SqlDataAdapter(
                $@"
                    DECLARE @part VARCHAR(MAX)
                    SET @part = '{part}'                             
                
                    SELECT KeyCode, [Desc] FROM Product (NOLOCK) WHERE KeyCode LIKE '%{part}%' AND KeyCode IN(SELECT KitSKU FROM MERI_Kit_Upgrade)"
                , connection1);
                adptr2.Fill(dtKitMatch);
                connection1.Close();

                foreach (DataRow row in dtKitMatch.Rows)
                {
                    int rowIndex = dataGridView4.Rows.Add();
                    DataGridViewRow newRow = dataGridView4.Rows[rowIndex];
                    newRow.Cells[0].Value = row["KeyCode"].ToString();
                    newRow.Cells[1].Value = row["Desc"].ToString();
                }

                dataGridView4.Visible = true;
                // Switch focus to dataGridView4
                dataGridView4.Focus();

                // If dataGridView4 has rows, select the first row
                if (dataGridView4.Rows.Count > 0)
                {
                    dataGridView4.CurrentCell = dataGridView4.Rows[0].Cells[0];
                    dataGridView4.Rows[0].Cells[0].Selected = true;
                }

                return;
            }

            if (dtKitMatch.Rows.Count == 0)
            {
                this.Cursor = Cursors.Default;
                MessageBox.Show($@"Invalid Part!", "Notice", MessageBoxButtons.OK);
                return;
            }

            dtKitMatch.Columns.Clear();

            //foreach(DataColumn col in dtKitMatch.Columns)
            //{
            //    dtKitMatch.Columns.Remove(col);
            //}
            
            connection1.Close();
        }

        private void loadDataTable()
        {
            try
            {
                if (string.IsNullOrEmpty(part))
                {
                    // Handle the case where part is null or empty
                    return;
                }

                dt = new DataTable();

                string connString = Helper.ConnString("AUTOPART");
                using (SqlConnection connection1 = new SqlConnection(connString))
                {
                    connection1.Open();

                    string query = $@"
                DECLARE @part VARCHAR(MAX)
                SET @part = '{part}'    

                SELECT E.[Internal SKU], E.[SKU Description], E.[Option ID], 
                E.[Option] AS Prompt, E.Component, E.[Component Description], 
                E.[Component Qty], E.Price, E.[Sort Sequence], E.Make, E.Model, 
                E.Year, E.Level1, E.Level2, E.Level3, S.Free, E.[Upgrade Price]
                FROM  vw_MERI_NAP_Export AS E LEFT OUTER JOIN
                         Stock AS S ON E.Component = S.Part
                WHERE (E.[Internal SKU] = '{part}') AND (S.Branch = 'BR48')
                ORDER BY LEN(E.[Option ID]), E.[Option ID], LEN(E.[Sort Sequence]), E.[Sort Sequence] ASC
            ";

                    using (SqlCommand command = new SqlCommand(query, connection1))
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        // Load the data reader into the DataTable
                        dt.Load(reader);
                    }
                }

                if (dt.Rows.Count > 0)
                {
                    kitNumberTextbox.Text = dt.Rows[0]["Internal SKU"].ToString();
                    kitDescriptionTextbox.Text = dt.Rows[0]["SKU Description"].ToString();
                    makeTextbox.Text = dt.Rows[0]["Make"].ToString();
                    modelTextbox.Text = dt.Rows[0]["Model"].ToString();
                    yearRangeTextbox.Text = dt.Rows[0]["Year"].ToString();
                    level1Textbox.Text = dt.Rows[0]["Level1"].ToString();
                    level2Textbox.Text = dt.Rows[0]["Level2"].ToString();
                    level3Textbox.Text = dt.Rows[0]["Level3"].ToString();
                }
                else
                {
                    // Handle the case where there are no rows in the DataTable.
                    kitNumberTextbox.Text = string.Empty;
                    kitDescriptionTextbox.Text = string.Empty;
                    makeTextbox.Text = string.Empty;
                    modelTextbox.Text = string.Empty;
                    yearRangeTextbox.Text = string.Empty;
                    level1Textbox.Text = string.Empty;
                    level2Textbox.Text = string.Empty;
                    level3Textbox.Text = string.Empty;
                }
            }
            catch (Exception ex)
            {
                // Log or display the exception details
                MessageBox.Show($"An error occurred: {ex.Message}");
                // Optionally, rethrow or handle the exception appropriately
                // throw;
            }
        }

        private void loadPrompts()
        {
            promptDataTable.Clear();

            DataView sortedView = dt.DefaultView;
            //sortedView.Sort = "LEN(Option ID), Option ID, Sort Sequence ASC";
            //ORDER BY E.[Option ID], LEN(E.[Sort Sequence]), E.[Sort Sequence] ASC
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

            custNumTextbox.Focus();


        }

        private void dataGridView1_SelectionChanged(object sender, EventArgs e)
        {
            dataGridView2.DataSource = null;
            prompt_CellContentClick();
            loadComponents();
        }

        private void dataGridView1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Right || e.KeyCode == Keys.Tab)
            {
                // Switch focus to dataGridView2
                dataGridView2.Focus();

                // If dataGridView2 has rows, select the first row
                if (dataGridView2.Rows.Count > 0)
                {
                    dataGridView2.CurrentCell = dataGridView2.Rows[0].Cells[0];
                    dataGridView2.Rows[0].Cells[0].Selected = true;
                }

                // Mark the event as handled
                e.Handled = true;
            }
        }

        private void dataGridView2_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Left)
            {
                // Switch focus to dataGridView2
                dataGridView2.ClearSelection();
                dataGridView1.Focus();
                // Mark the event as handled
                e.Handled = true;
            }

            if (e.KeyCode == Keys.Enter)
            {
                keyHandler();
                // Switch focus back to dataGridView1
                dataGridView1.Focus();

                // Get the current selected index
                int currentIndex = dataGridView1.CurrentRow.Index;

                
                if (currentIndex == dataGridView1.Rows.Count - 1)
                {
                    MessageBox.Show("Kit Complete!");
                    dataGridView3.Focus();

                    if (dataGridView3.Rows.Count > 0)
                    {
                        dataGridView3.Rows[0].Selected = true;
                    }
                    return;
                }

                // If the current index is not the last index, select the next row
                if (currentIndex < dataGridView1.Rows.Count - 1)
                {
                    dataGridView1.CurrentCell = dataGridView1.Rows[currentIndex + 1].Cells[0];
                }

                dataGridView2.Focus();

                if (dataGridView2.Rows.Count > 0)
                {
                    dataGridView2.Rows[0].Selected = true;
                }

                // Mark the event as handled
                e.Handled = true;
            }

        }

        private void dataGridView2_CellContentDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            keyHandler();

            // Switch focus back to dataGridView1
            dataGridView1.Focus();

            // Get the current selected index
            int currentIndex = dataGridView1.CurrentRow.Index;

            if (currentIndex == dataGridView1.Rows.Count - 1)
            {
                MessageBox.Show("Kit Complete!");
                dataGridView3.Focus();

                if (dataGridView3.Rows.Count > 0)
                {
                    dataGridView3.Rows[0].Selected = true;
                }
                return;
            }

            // If the current index is not the last index, select the next row
            if (currentIndex < dataGridView1.Rows.Count - 1)
            {
                dataGridView1.CurrentCell = dataGridView1.Rows[currentIndex + 1].Cells[0];
            }
        }
        
        private void prompt_CellContentClick()
        {
            if (dt.Rows.Count > 0 && dataGridView1.SelectedCells.Count > 0)
            {
                /*DataGridViewCell selectedCell = dataGridView1.SelectedCells[0];
                cellText = selectedCell.Value?.ToString();

                string searchColumn = "Prompt"; // Column to search


                // Find the row where the searchColumn has the searchValue
                foundRow = dt.Select($"{searchColumn} = '{cellText}'").FirstOrDefault();*/

                // Get the first selected cell's row index
                int rowIndex = dataGridView1.SelectedCells[0].RowIndex;

                // Get the value of the first cell in the selected row
                object cellValue = dataGridView1.Rows[rowIndex].Cells[0].Value;

                // Convert the cell value to a string (assuming it's a string)
                cellText = cellValue != null ? cellValue.ToString() : string.Empty;
                string searchColumn = "Prompt"; // Column to search
                foundRow = dt.Select($"{searchColumn} = '{cellText}'").FirstOrDefault();
                // Now, cellText contains the string value of the selected cell
                // You can use this variable as needed
            }
            else
            {
                return;
            }
        }

        private void loadComponents()
        {
            itemsDataTable.Clear();

            DataView sortedView = dt.DefaultView;
            sortedView.Sort = "Option ID ASC, Sort Sequence ASC";
            sorteddt = sortedView.ToTable();

            // Add the specific column to the destination DataTable
            string columnName = "Component"; // Specify the column name
            if (itemsDataTable.Columns.Contains(columnName))
            {
                //do nothing
            }
            else
            {
                itemsDataTable.Columns.Add("Component", typeof(string));
                itemsDataTable.Columns.Add("Component Description", typeof(string));
                itemsDataTable.Columns.Add("Free", typeof(int));
                itemsDataTable.Columns.Add("Component Qty", typeof(int));
                itemsDataTable.Columns.Add("Upgrade Price", typeof(string));
            }

            foreach (DataRow sourceRow in sorteddt.Rows)
            {
                if (sourceRow[3].ToString() == cellText)
                {
                    DataRow destinationRow = itemsDataTable.NewRow();

                    // Copy the values from the specific columns
                    destinationRow["Component"] = sourceRow["Component"];
                    destinationRow["Component Description"] = sourceRow["Component Description"];
                    destinationRow["Free"] = sourceRow["Free"];
                    destinationRow["Component Qty"] = sourceRow["Component Qty"];
                    destinationRow["Upgrade Price"] = sourceRow["Upgrade Price"];

                    // Add the new row to the destination DataTable
                    itemsDataTable.Rows.Add(destinationRow);
                }
            }


            dataGridView2.Rows.Clear();

            // Populate the DataGridView with the data from the DataTable
            foreach (DataRow dataRow in itemsDataTable.Rows)
            {
                // Create a new row in the DataGridView
                int rowIndex = dataGridView2.Rows.Add();

                // Map the values from the DataTable to the DataGridView
                for (int columnIndex = 0; columnIndex < itemsDataTable.Columns.Count; columnIndex++)
                {
                    dataGridView2.Rows[rowIndex].Cells[columnIndex].Value = dataRow[columnIndex];
                }
            }

            //dataGridView2.DataSource = itemsDataTable;
            dataGridView2.ClearSelection();

            foreach (DataGridViewColumn column in dataGridView2.Columns)
            {
                column.SortMode = DataGridViewColumnSortMode.NotSortable;
            }

        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            Utilities.ResetAllControls(this);
            dataGridView1.Rows.Clear();
            dataGridView2.Rows.Clear();
            dataGridView3.Rows.Clear();
            finalDataTable.Rows.Clear();
            kitNumberTextbox.Focus();
        }

        private void quitButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void finalizeButton_Click(object sender, EventArgs e)
        {
            if(poTextbox.Text == "")
            {
                MessageBox.Show("Purchase Order can not be left blank!");
                poTextbox.Focus();
                return;
            }

            if(custNumTextbox.Text == "" || custNameTextbox.Text == "")
            {
                MessageBox.Show("Customer Information can not be left blank!");
                custNumTextbox.Focus();
                return;
            }

            // Text to be saved as XML
            int lineNum = 1;
            string XMLOrderLine = "";
            DateTime date = DateTime.Now;
            formattedDate = date.ToString("yyyy-MM-ddTHH:mm");

            foreach (DataGridViewRow row in dataGridView3.Rows)
            {
                // Ensure the row is not a new row
                if (!row.IsNewRow)
                {
                    // Iterate through each cell in the row
                    foreach (DataGridViewCell cell in row.Cells)
                    {
                        // Check if the cell value is not null and is a string
                        if (cell.Value != null && cell.Value is string cellValue)
                        {
                            // Replace the "&" character with "and"
                            //Note: @ ! ‘ ~ & and ^ cannot be used
                            string newValue = cellValue.Replace("&", "and")
                            .Replace("^", "")
                            .Replace("@", "")
                            .Replace("!", "")
                            .Replace("~", "");
                            // Update the cell value with the new value
                            cell.Value = newValue;
                        }
                    }
                }
            }

            foreach (DataGridViewRow row in dataGridView3.Rows)
            {
                if (!row.IsNewRow)
                {
                    XMLOrderLine = XMLOrderLine + 
                "\r\n      <OrderLine>" +
                "\r\n        <LineNumber>" + lineNum + "</LineNumber>" +
                "\r\n        <Product>" +
                "\r\n          <BuyersProductCode>" + row.Cells["Component"].Value.ToString() + "</BuyersProductCode>" +
                "\r\n          <Description>" + row.Cells["Description"].Value.ToString() + "</Description>" +
                "\r\n        </Product>" +
                "\r\n        <Quantity>" +
                "\r\n          <Amount>" + row.Cells["Quantity"].Value.ToString() + "</Amount>" +
                "\r\n        </Quantity>" +
                "\r\n        <Price>" +
                "\r\n          <UnitPrice>" + row.Cells["Price"].Value.ToString() + "</UnitPrice>" +
                "\r\n        </Price>" +
                "\r\n        <OrderLineInformation />" +
                "\r\n        <PartMessage />" +
                "\r\n      </OrderLine>";
                }
                lineNum++;
            }

                    string xmlText = 
                "<?xml version=\"1.0\" encoding=\"utf-8\" standalone=\"yes\"?>" +
                "\r\n<?xml-stylesheet type=\"text/xsl\" href=\"eBIS_MAM.xsl\"?>" +
                "\r\n<biztalk_1 xmlns=\"urn:schemas-biztalk-org:biztalk:biztalk_1\">" +
                "\r\n  <header>" +
                "\r\n    <delivery>" +
                "\r\n      <message />" +
                "\r\n      <to>" +
                "\r\n        <address />" +
                "\r\n      </to>" +
                "\r\n      <from>" +
                "\r\n        <address />" +
                "\r\n      </from>" +
                "\r\n    </delivery>" +
                "\r\n    <manifest>" +
                "\r\n      <document>" +
                "\r\n        <name>Order</name>" +
                "\r\n        <description>OW Order</description>" +
                "\r\n      </document>" +
                "\r\n    </manifest>" +
                "\r\n  </header>" +
                "\r\n  <body>" +
                "\r\n    <Order xmlns=\"urn:www.basda.org/schema/eBIS-XML_order_v3.00.xml\">" +
                "\r\n      <OrderHead>" +
                "\r\n        <OriginatingSoftware>" +
                "\r\n          <SoftwareProduct>ACX-MERI</SoftwareProduct>" +
                "\r\n        </OriginatingSoftware>" +
                "\r\n        <OrderType Code=\"PUO\" />" +
                "\r\n        <Function Code=\"FIO\" />" +
                "\r\n      </OrderHead>" +
                "\r\n      <OrderReferences>" +
                "\r\n        <BuyersOrderNumber>" + poTextbox.Text + "</BuyersOrderNumber>" +
                "\r\n        <SuppliersOrderReference>" + DocNum + "</SuppliersOrderReference>" +
                "\r\n      </OrderReferences>" +
                "\r\n      <OrderDate>" + formattedDate + "</OrderDate>" +
                "\r\n      <Supplier>" +
                "\r\n        <SupplierReferences>" +
                "\r\n          <BuyersCodeForSupplier>BR48</BuyersCodeForSupplier>" +
                "\r\n        </SupplierReferences>" +
                "\r\n      </Supplier>" +
                "\r\n      <Buyer>" +
                "\r\n        <BuyerReferences>" +
                "\r\n          <SuppliersCodeForBuyer>"+ custNumTextbox.Text +"</SuppliersCodeForBuyer>" +
                "\r\n        </BuyerReferences>" +
                "\r\n        <Party>MERI</Party>" +
                "\r\n        <Address />" +
                "\r\n        <Contact>" +
                "\r\n          <DDI />" +
                "\r\n          <Email />" +
                "\r\n        </Contact>" +
                "\r\n      </Buyer>" +
                "\r\n      <Delivery>" +
                "\r\n        <DeliverTo>" +
                "\r\n          <DeliverToReferences>" +
                "\r\n            <BuyersCodeForDelivery>"+ custNumTextbox.Text + "</BuyersCodeForDelivery>" +
                "\r\n          </DeliverToReferences>" +
                "\r\n          <Party>MERI</Party>" +
                "\r\n          <Address />" +
                "\r\n          <Contact>" +
                "\r\n            <DDI />" +
                "\r\n            <Email />" +
                "\r\n          </Contact>" +
                "\r\n        </DeliverTo>" +
                "\r\n        <DeliveryInformation>Method: DEL</DeliveryInformation>" +
                "\r\n      </Delivery>" +
                XMLOrderLine +
                "\r\n      <SpecialInstructions>" + descriptionLabel.Text + "</SpecialInstructions>" +
                "\r\n    </Order>" +
                "\r\n  </body>" +
                "\r\n</biztalk_1>";

            // File path where XML will be saved
            string filePath = $@"\\svrsql1\AUTOPART\Mail\Inbox\BR48\OW{poTextbox.Text}-{DocNum}.XML";

            string ebuText = $@"EMAIL~~email@email.com~~Purchase Order {poTextbox.Text}~~~~~OW{poTextbox.Text}-{DocNum}.xml";
            string ebuFilePath = $@"\\svrsql1\AUTOPART\Mail\Inbox\BR48\OW{poTextbox.Text}-{DocNum}.ebu";

            // Write XML text to a file using StreamWriter
            using (StreamWriter writer = new StreamWriter(filePath))
            {
                writer.Write(xmlText);
                writer.Close();
            }
            using (StreamWriter writer = new StreamWriter(ebuFilePath))
            {
                writer.Write(ebuText);
                writer.Close();
            }

            getWebDocs();
            MessageBox.Show($@"Order Created Successfully! Order number is {DocNum}.");
            Utilities.ResetAllControls(this);

            //label12.Text = "";
        }

        private void removeButton_Click(object sender, EventArgs e)
        {
            if (selectedRowIndex >= 0 && selectedRowIndex < finalDataTable.Rows.Count)
            {
                finalDataTable.Rows.RemoveAt(selectedRowIndex);

                //dataGridView3.DataSource = null;
                dataGridView3.Rows.Clear();
                //dataGridView3.Columns.Clear();
                //dataGridView3.DataSource = finalDataTable;

                foreach (DataRow dataRow in finalDataTable.Rows)
                {
                    // Create a new row in the DataGridView
                    int rowIndex = dataGridView3.Rows.Add();

                    // Map the values from the DataTable to the DataGridView
                    for (int columnIndex = 0; columnIndex < finalDataTable.Columns.Count; columnIndex++)
                    {
                        dataGridView3.Rows[rowIndex].Cells[columnIndex].Value = dataRow[columnIndex];
                    }
                }

                decimal total = 0;
                foreach (DataGridViewRow row in dataGridView3.Rows)
                {
                    if (row.Cells["Ext"].Value != null)
                    {
                        decimal extValue;
                        if (decimal.TryParse(row.Cells["EXT"].Value.ToString(), out extValue))
                        {
                            total += extValue;
                        }
                    }
                }
                totalTextbox.Text = total.ToString();
            }

        }

        private void keyHandler()
        {
            dataGridView2.Focus();
            // If dataGridView2 has no selected cells, select the first cell
            if (dataGridView2.SelectedCells.Count == 0 && dataGridView2.Rows.Count > 0)
            {
                dataGridView2.CurrentCell = dataGridView2.Rows[0].Cells[0];
                selectedCell = dataGridView2.Rows[0].Cells[0];
                dataGridView2.Rows[0].Cells[0].Selected = true;
            }
            else
            {
                selectedCell = dataGridView2.SelectedCells[0];
                dataGridView2.Rows[0].Cells[0].Selected = true;
            }

            
            cellText = selectedCell.Value?.ToString();

            string searchColumn = "Component"; // Column to search


            // Find the row where the searchColumn has the searchValue
            foundRow = dt.Select($"{searchColumn} = '{cellText}'").FirstOrDefault();




            DataView sortedView = dt.DefaultView;
            sortedView.Sort = "Option ID ASC, Sort Sequence ASC";
            sorteddt = sortedView.ToTable();

            // Add the specific column to the destination DataTable
            string columnName = "Component"; // Specify the column name
            if (finalDataTable.Columns.Contains(columnName))
            {
                //do nothing
            }
            else
            {
                finalDataTable.Columns.Add("Prompt", typeof(string));
                finalDataTable.Columns.Add("Component", typeof(string));
                finalDataTable.Columns.Add("Component Description", typeof(string));
                finalDataTable.Columns.Add("Free", typeof(int));
                finalDataTable.Columns.Add("Component Qty", typeof(int));
                finalDataTable.Columns.Add("Price", typeof(double));
                finalDataTable.Columns.Add("Ext", typeof(double));
            }

            foreach (DataRow sourceRow in sorteddt.Rows)
            {
                if (sourceRow[4].ToString() == cellText)
                {
                    DataRow destinationRow = finalDataTable.NewRow();

                    // Copy the values from the specific columns
                    destinationRow["Prompt"] = sourceRow["Prompt"];
                    destinationRow["Component"] = sourceRow["Component"];
                    destinationRow["Component Description"] = sourceRow["Component Description"];
                    destinationRow["Free"] = sourceRow["Free"];
                    destinationRow["Component Qty"] = sourceRow["Component Qty"];
                    destinationRow["Price"] = sourceRow["Price"];
                    //for testing
                    double price = Convert.ToDouble(sourceRow["Price"]);
                    int qty = Convert.ToInt32(sourceRow["Component Qty"]);

                    destinationRow["Ext"] = Convert.ToDouble(sourceRow["Component Qty"]) * Convert.ToDouble(sourceRow["Price"]);

                    // Add the new row to the destination DataTable
                    finalDataTable.Rows.Add(destinationRow);
                }
            }

            dataGridView3.Rows.Clear();

            // Populate the DataGridView with the data from the DataTable
            foreach (DataRow dataRow in finalDataTable.Rows)
            {
                // Create a new row in the DataGridView
                int rowIndex = dataGridView3.Rows.Add();

                // Map the values from the DataTable to the DataGridView
                for (int columnIndex = 0; columnIndex < finalDataTable.Columns.Count; columnIndex++)
                {
                    dataGridView3.Rows[rowIndex].Cells[columnIndex].Value = dataRow[columnIndex];
                }
            }

            //dataGridView3.DataSource = finalDataTable;
            for (int i = 0; i < dataGridView1.Columns.Count; i++)
            {
                if(i == 1)
                {
                    dataGridView1.Columns[i].ReadOnly = false;
                }
                else
                {
                    dataGridView1.Columns[i].ReadOnly = true;
                }
            }
            //dataGridView3.ClearSelection();

            foreach (DataGridViewColumn column in dataGridView3.Columns)
            {
                column.SortMode = DataGridViewColumnSortMode.NotSortable;
            }

            decimal total = 0;
            foreach (DataGridViewRow row in dataGridView3.Rows)
            {
                if (row.Cells["Ext"].Value != null)
                {
                    decimal extValue;
                    if (decimal.TryParse(row.Cells["EXT"].Value.ToString(), out extValue))
                    {
                        total += extValue;
                    }
                }
            }
            totalTextbox.Text = total.ToString();
        }

        private void custNumTextbox_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (e.KeyCode == Keys.Enter || e.KeyCode == Keys.Tab)
            {
                this.Cursor = Cursors.WaitCursor;
                SqlConnection connection1 = new SqlConnection(Helper.ConnString("AUTOPART"));

                connection1.Open();
                SqlDataAdapter adptr = new SqlDataAdapter(
                $@"                              
                    SELECT Area, KeyCode, Name 
                    FROM Customer (NOLOCK) 
                    WHERE LType != 'NONE' AND (KeyCode = '{custNumTextbox.Text}' OR 
                    Name LIKE '%{custNumTextbox.Text}%' )
                    ORDER BY Area, Name
                "
                , connection1);
                adptr.Fill(dt1);
                this.Cursor = Cursors.Default;

                if (dt1.Rows.Count == 0)
                {
                    MessageBox.Show($@"Customer not on file!", "Notice", MessageBoxButtons.OK);
                    return;
                }
                else if (dt1.Rows.Count == 1)
                {
                    custNameTextbox.Text = dt1.Rows[0]["Name"].ToString();
                }
                else if (dt1.Rows.Count > 1)
                {
                    foreach (DataRow row in dt1.Rows)
                    {
                        int rowIndex = dataGridView6.Rows.Add();
                        DataGridViewRow newRow = dataGridView6.Rows[rowIndex];
                        newRow.Cells[0].Value = row[0].ToString();
                        newRow.Cells[1].Value = row[1].ToString();
                        newRow.Cells[2].Value = row[2].ToString();
                        dataGridView6.Visible = true;
                        dataGridView6.Focus();
                        dataGridView6.CurrentCell = dataGridView6.Rows[0].Cells[0];
                        dataGridView6.Rows[0].Cells[0].Selected = true;
                    }
                    return;
                }

                connection1.Close();
                dt1.Clear();
                dt1.Columns.Remove("KeyCode");
                dt1.Columns.Remove("Name");

                connection1.Close();

                if (dataGridView1.Rows.Count > 0)
                {
                    dataGridView1.Focus();
                    dataGridView1.CurrentCell = dataGridView1.Rows[0].Cells[0];
                    dataGridView1.Rows[0].Cells[0].Selected = true;
                }
            }
        }

        private (string part, string description, string free) GetNewDataFromDatabase(string newValue)
        {
            string part = string.Empty;
            string description = string.Empty;
            string free = string.Empty;
            bool resultsFound = false;

            //string connectionString = "your_connection_string_here";
            string query = "SELECT P.KeyCode, P.[Desc], CAST(S.Free as INTEGER) as Free FROM Product P INNER JOIN Stock S on P.KeyCode = S.Part WHERE P.KeyCode = @newValue AND S.Branch = 'BR48'";


            using (SqlConnection connection = new SqlConnection(Helper.ConnString("AUTOPART")))
            {
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@newValue", newValue);

                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                if (reader.Read())
                {
                    part = reader["KeyCode"].ToString();
                    description = reader["Desc"].ToString();
                    free = reader["Free"].ToString();
                    resultsFound = true;
                    attempts = 1;
                }
                reader.Close();
            }

            if (!resultsFound)
            {
                noMatch();
            }

            return (part, description, free);
        }

        public void noMatch()
        {
            this.Cursor = Cursors.WaitCursor;
            SqlConnection connection1 = new SqlConnection(Helper.ConnString("AUTOPART"));
            SqlDataAdapter adptr1 = new SqlDataAdapter(
            $@"
            SELECT P.KeyCode, P.[Desc], CAST(S.Free as INTEGER) as Free FROM Product P INNER JOIN Stock S on P.KeyCode = S.Part WHERE (P.KeyCode LIKE '%{stringNewValue}%' OR P.Word5 LIKE '%{stringNewValue}%') AND S.Branch = 'BR48' ORDER BY P.KeyCode
            "
            , connection1);
            adptr1.Fill(dtPartMatch);

            if(dtPartMatch.Rows.Count == 0)
            {
                this.Cursor = Cursors.Default;
                MessageBox.Show("Invalid Part!");
                return;
            }

            foreach (DataRow row in dtPartMatch.Rows)
            {
                int rowIndex = dataGridView5.Rows.Add();
                DataGridViewRow newRow = dataGridView5.Rows[rowIndex];
                newRow.Cells[0].Value = row["KeyCode"].ToString();
                newRow.Cells[1].Value = row["Desc"].ToString();
                newRow.Cells[2].Value = row["Free"].ToString();
                attempts = 2;
            }
            dataGridView5.Visible = true;
            dataGridView5.Focus();

            // If dataGridView2 has rows, select the first row
            if (dataGridView5.Rows.Count > 0)
            {
                dataGridView5.CurrentCell = dataGridView5.Rows[0].Cells[0];
                dataGridView5.Rows[0].Cells[0].Selected = true;
            }
            this.Cursor = Cursors.Default;
            return;
        }

        private void AdjustControlScale()
        {
            // Get the current screen resolution
            Rectangle screenBounds = Screen.PrimaryScreen.Bounds;
            float screenWidth = screenBounds.Width;
            float screenHeight = screenBounds.Height;

            // Calculate the scaling factors for width and height
            float widthScaleFactor = screenWidth / 1920; // Assuming base width of 1920 pixels
            float heightScaleFactor = screenHeight / 1080; // Assuming base height of 1080 pixels

            // Adjust form size
            this.Width = (int)Math.Round(this.Width * widthScaleFactor);
            this.Height = (int)Math.Round(this.Height * heightScaleFactor);

            // Loop through all controls on the form and adjust their size and position
            foreach (System.Windows.Forms.Control control in Controls)
            {
                // Adjust size of each control
                control.Width = (int)Math.Round(control.Width * widthScaleFactor);
                control.Height = (int)Math.Round(control.Height * heightScaleFactor);

                // Adjust location of each control
                control.Left = (int)Math.Round(control.Left * widthScaleFactor);
                control.Top = (int)Math.Round(control.Top * heightScaleFactor);

                // If control is a font-based control (e.g., Label, Button), adjust font size
                if (control is Button || control is Label || control is TextBox)
                {
                    float currentFontSize = control.Font.Size;
                    float newFontSize = currentFontSize * Math.Min(widthScaleFactor, heightScaleFactor);
                    control.Font = new System.Drawing.Font(control.Font.FontFamily, newFontSize, control.Font.Style);
                }
            }
        }

        private void dataGridView4_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                this.Cursor = Cursors.WaitCursor;
                kitNumberTextbox.Text = dataGridView4.CurrentRow.Cells[0].Value.ToString();
                part = dataGridView4.CurrentRow.Cells[0].Value.ToString();
                dataGridView4.Visible = false;
                loadDataTable();
                loadPrompts();
                firstLoad = false;
                this.Cursor = Cursors.Default;
            }
            if (e.KeyCode == Keys.Escape)
            {
                dataGridView4.Visible = false;
                kitNumberTextbox.Text = "";
                kitNumberTextbox.Focus();
                this.Cursor = Cursors.Default;
                return;
            }
        }

        private void dataGridView4_CellContentDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            this.Cursor = Cursors.WaitCursor;
            kitNumberTextbox.Text = dataGridView4.CurrentRow.Cells[0].Value.ToString();
            part = dataGridView4.CurrentRow.Cells[0].Value.ToString();
            dataGridView4.Visible = false;
            loadDataTable();
            loadPrompts();
            firstLoad = false;
            this.Cursor = Cursors.Default;
        }

        private void dataGridView5_CellContentDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            // Get the new value from the second column
            var newValue = dataGridView5.Rows[e.RowIndex].Cells[0].Value.ToString();
            stringNewValue = newValue.ToString();

            // Perform the SQL query to fetch the new data
            var newValues = GetNewDataFromDatabase(newValue);

            if (attempts == 1)
            {
                dataGridView3.Rows[rowIndex].Cells[1].Value = newValues.part;
                dataGridView3.Rows[rowIndex].Cells[2].Value = newValues.description;
                dataGridView3.Rows[rowIndex].Cells[3].Value = newValues.free;
            }
            dataGridView5.Visible = false;
        }

        private void dataGridView5_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                var currentCell = dataGridView5.CurrentCell;
                var newValue = currentCell.Value?.ToString();
                stringNewValue = newValue.ToString();

                // Perform the SQL query to fetch the new data
                var newValues = GetNewDataFromDatabase(newValue);

                if (attempts == 1)
                {
                    dataGridView3.Rows[rowIndex].Cells[1].Value = newValues.part;
                    dataGridView3.Rows[rowIndex].Cells[2].Value = newValues.description;
                    dataGridView3.Rows[rowIndex].Cells[3].Value = newValues.free;
                }
                dataGridView5.Visible = false;
            }
            if (e.KeyCode == Keys.Escape)
            {
                dtPartMatch.Clear();
                dtPartMatch.Rows.Clear();
                dtPartMatch.Columns.Clear();
                dataGridView3.DataSource = null;
                dataGridView3.Rows.Clear();
                foreach (DataRow dataRow in finalDataTable.Rows)
                {
                    // Create a new row in the DataGridView
                    int rowIndex = dataGridView3.Rows.Add();

                    // Map the values from the DataTable to the DataGridView
                    for (int columnIndex = 0; columnIndex < finalDataTable.Columns.Count; columnIndex++)
                    {
                        dataGridView3.Rows[rowIndex].Cells[columnIndex].Value = dataRow[columnIndex];
                    }
                }

                dataGridView5.Visible = false;
            }
        }

        private void dataGridView3_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            // Check if the changed cell is in the second column
            if (e.ColumnIndex == 1)
            {
                rowIndex = dataGridView3.CurrentCell.RowIndex;
                // Get the new value from the second column
                var newValue = dataGridView3.Rows[e.RowIndex].Cells[1].Value.ToString();
                stringNewValue = newValue.ToString();

                this.Cursor = Cursors.WaitCursor;
                // Perform the SQL query to fetch the new data
                var newValues = GetNewDataFromDatabase(newValue);
                this.Cursor = Cursors.Default;

                if (attempts == 1)
                {
                    dataGridView3.Rows[e.RowIndex].Cells[1].Value = newValues.part;
                    dataGridView3.Rows[e.RowIndex].Cells[2].Value = newValues.description;
                    dataGridView3.Rows[e.RowIndex].Cells[3].Value = newValues.free;

                }
                if (attempts > 1)
                {
                    dataGridView5.Focus();

                    // If dataGridView2 has rows, select the first row
                    if (dataGridView5.Rows.Count > 0)
                    {
                        dataGridView5.CurrentCell = dataGridView5.Rows[0].Cells[0];
                        dataGridView5.Rows[0].Cells[0].Selected = true;
                    }
                    return;
                }
                int columnIndex = 1;
                if (rowIndex >= 0 && rowIndex < dataGridView1.Rows.Count)
                {
                    if (columnIndex >= 0 && columnIndex < dataGridView3.Columns.Count)
                    {
                        dataGridView3.Rows[rowIndex].Cells[columnIndex].Selected = true;
                        dataGridView3.CurrentCell = dataGridView3.Rows[rowIndex].Cells[columnIndex];
                    }
                }
            }
        }

        private void dataGridView6_CellContentDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            var currentCell = dataGridView6.CurrentCell;
            var newValue = dataGridView6.Rows[currentCell.RowIndex].Cells[1];
            string currentCellValue = newValue?.Value?.ToString();

            var nextCell = dataGridView6.Rows[currentCell.RowIndex].Cells[2];
            string nextCellValue = nextCell?.Value?.ToString();

            custNumTextbox.Text = currentCellValue;
            custNameTextbox.Text = nextCellValue;

            dataGridView6.Visible = false;

            if (dataGridView1.Rows.Count > 0)
            {
                dataGridView1.Focus();
                dataGridView1.CurrentCell = dataGridView1.Rows[0].Cells[0];
                dataGridView1.Rows[0].Cells[0].Selected = true;
            }
        }

        private void dataGridView6_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (e.KeyCode == Keys.Enter || e.KeyCode == Keys.Tab)
            {
                var currentCell = dataGridView6.CurrentCell;
                var newValue = dataGridView6.Rows[currentCell.RowIndex].Cells[1];
                string currentCellValue = newValue?.Value?.ToString();

                var nextCell = dataGridView6.Rows[currentCell.RowIndex].Cells[2];
                string nextCellValue = nextCell?.Value?.ToString();

                custNumTextbox.Text = currentCellValue;
                custNameTextbox.Text = nextCellValue;

                dataGridView6.Visible = false;

                if (dataGridView1.Rows.Count > 0)
                {
                    dataGridView1.Focus();
                    dataGridView1.CurrentCell = dataGridView1.Rows[0].Cells[0];
                    dataGridView1.Rows[0].Cells[0].Selected = true;
                }
            }
            if (e.KeyCode == Keys.Escape)
            {
                dt1 = new DataTable();
                dataGridView6.Rows.Clear();
                dataGridView6.DataSource = null;
                dataGridView6.Visible = false;
                custNumTextbox.Focus();
            }
        }

        private void dataGridView3_Enter(object sender, EventArgs e)
        {
            removeButton.Enabled = true;
            altsButton.Enabled = true;
        }

        private void dataGridView2_Enter(object sender, EventArgs e)
        {
            removeButton.Enabled = false;
            altsButton.Enabled = false;
        }

        private void dataGridView1_Enter(object sender, EventArgs e)
        {
            removeButton.Enabled= false;
            altsButton.Enabled= false;
        }

        private void dataGridView6_Enter(object sender, EventArgs e)
        {
            removeButton.Enabled = false;
            altsButton.Enabled= false;
        }

        private void dataGridView5_Enter(object sender, EventArgs e)
        {
            removeButton.Enabled = false;
            altsButton.Enabled= false;
        }

        private void dataGridView4_Enter(object sender, EventArgs e)
        {
            removeButton.Enabled = false;
            altsButton.Enabled= false;
        }

        private void altsButton_Click(object sender, EventArgs e)
        {
            alts = new DataTable();
            altsGridView.Rows.Clear();

            //DataGridViewRow selectedRow = dataGridView3.SelectedRows[0];
            string nameValue = dataGridView3.Rows[selectedRowIndex].Cells[1].Value.ToString();

            //var currentCell = dataGridView3;
            //var newValue = currentCell.Value?.ToString();
            //string currentCellValue = newValue.ToString();

            SqlConnection connection1 = new SqlConnection(Helper.ConnString("AUTOPART"));
            SqlDataAdapter adptr1 = new SqlDataAdapter(
            $@"
            SELECT A.AlternatePart as KeyCode, P.[Desc], CAST(S.Free as int) as Free FROM Alternatives A 
            INNER JOIN Product P ON P.KeyCode = A.AlternatePart 
            INNER JOIN Stock S ON A.AlternatePart = S.Part
            WHERE S.Branch = 'BR48' AND A.Part = '{nameValue}'
            "
            , connection1);
            adptr1.Fill(alts);

            if (alts.Rows.Count == 0)
            {
                this.Cursor = Cursors.Default;
                MessageBox.Show("No alternatives to this part exist!");
                return;
            }

            foreach (DataRow row in alts.Rows)
            {
                int rowIndex = altsGridView.Rows.Add();
                DataGridViewRow newRow = altsGridView.Rows[rowIndex];
                newRow.Cells[0].Value = row["KeyCode"].ToString();
                newRow.Cells[1].Value = row["Desc"].ToString();
                newRow.Cells[2].Value = row["Free"].ToString();
                attempts = 2;
            }
            altsGridView.Visible = true;
            altsGridView.Focus();
        }

        private void altsGridView_CellContentDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            var currentCell = altsGridView.CurrentCell;
            var newValue = currentCell.Value?.ToString();
            stringNewValue = newValue.ToString();

            // Perform the SQL query to fetch the new data
            var newValues = GetNewDataFromDatabase(newValue);

            dataGridView3.Rows[selectedRowIndex].Cells[1].Value = newValues.part;
            dataGridView3.Rows[selectedRowIndex].Cells[2].Value = newValues.description;
            dataGridView3.Rows[selectedRowIndex].Cells[3].Value = newValues.free;
            altsGridView.Visible = false;
        }

        private void altsGridView_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                var currentCell = altsGridView.CurrentCell;
                var newValue = currentCell.Value?.ToString();
                stringNewValue = newValue.ToString();

                // Perform the SQL query to fetch the new data
                var newValues = GetNewDataFromDatabase(newValue);

                dataGridView3.Rows[selectedRowIndex].Cells[1].Value = newValues.part;
                dataGridView3.Rows[selectedRowIndex].Cells[2].Value = newValues.description;
                dataGridView3.Rows[selectedRowIndex].Cells[3].Value = newValues.free;
                altsGridView.Visible = false;
            }
            if (e.KeyCode == Keys.Escape)
            {
                alts.Clear();
                alts.Rows.Clear();
                alts.Columns.Clear();
                //dataGridView3.DataSource = null;
                //dataGridView3.Rows.Clear();
                /*foreach (DataRow dataRow in finalDataTable.Rows)
                {
                    // Create a new row in the DataGridView
                    int rowIndex = dataGridView3.Rows.Add();

                    // Map the values from the DataTable to the DataGridView
                    for (int columnIndex = 0; columnIndex < finalDataTable.Columns.Count; columnIndex++)
                    {
                        dataGridView3.Rows[rowIndex].Cells[columnIndex].Value = dataRow[columnIndex];
                    }
                }*/

                altsGridView.Visible = false;
            }
        }
    }

    public class Utilities
    {
        public static void ResetAllControls(System.Windows.Forms.Control form)
        {
            foreach (System.Windows.Forms.Control control in form.Controls)
            {
                if (control is TextBox)
                {
                    TextBox textBox = (TextBox)control;
                    textBox.Text = null;
                    textBox.BackColor = System.Drawing.Color.Empty;
                }

                if (control is ComboBox)
                {
                    ComboBox comboBox = (ComboBox)control;
                    if (comboBox.Items.Count > 0)
                        comboBox.SelectedIndex = 0;
                }


                if (control is DataGridView)
                {
                    DataGridView gridView = (DataGridView)control;
                    //gridView.Rows.Clear();
                    //gridView.Columns.Clear();
                    gridView.DataSource = null;
                }

                if (control is ListBox)
                {
                    ListBox listBox = (ListBox)control;
                    listBox.ClearSelected();
                }
            }
        }
    }
}