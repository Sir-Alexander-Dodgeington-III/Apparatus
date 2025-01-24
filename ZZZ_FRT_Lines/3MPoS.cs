using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace MMMPoS
{
    public partial class MMMPoS : Form
    {
        public DataTable table = new DataTable();
        public MMMPoS()
        {
            InitializeComponent();
            this.CenterToScreen();

            table.Clear();

            string startDate = Interaction.InputBox("Please enter a start date", "Start Date", "MM/DD/YYYY");
            string endDate = Interaction.InputBox("Please enter a end date", "End Date", "MM/DD/YYYY");

            Cursor.Current = Cursors.WaitCursor;
            SqlConnection conn = new SqlConnection(Helper.ConnString("AUTOPART"));
            conn.Open();


            string query = ($@"
                                SELECT 'MERRI' AS PartnerID, 'The Merrill Company' AS PartnerName, IH.DateTime AS InvoiceDate, IL.[Document] AS InvoiceNumber,
                                IL.Part AS ProductCatalogNumber, M.A12 AS ProductUPC, P.[Desc] AS ProductDescription, IL.Qty AS Quantity, 'EACH' AS UnitOfMeasure, 
                                SL.P11 AS PartnerUnitCost, SL.P7 AS SuggestedUserPrice, IL.Qty* SL.P7 AS SuggestedExtendedPrice, 'USD' AS CurrencyCode, IH.Acct AS ShipToCustomerID, 
                                C.Name AS ShipToCustomerName,  (C.Addra + ' ' + ISNULL(C.addrb,'') + ' ' + ISNULL(C.addrc,'') + ' ' + ISNULL(C.addrd,'')) AS ShipToCustomerAddress, 
                                C.Addre AS ShipToCity, C.MotDueSort AS ShipToState, C.PCode AS ShipToPostalCode, 'US' AS ShipToCountry 
                                FROM ILines AS IL 
                                INNER JOIN IHeads AS IH ON IL.[Document] = IH.[Document] 
                                INNER JOIN Product AS P ON IL.Part = P.KeyCode 
                                INNER JOIN Customer AS C ON IH.Acct = C.KeyCode 
                                INNER JOIN Mvpr AS M ON IL.Part = M.SubKey1 
                                INNER JOIN MERI_SELLINGLEVELS as SL ON IL.Part = SL.Part 
                                WHERE (IL.PG = 'MMM') AND (CAST(IH.DateTime AS Date) BETWEEN '{ startDate }' AND '{ endDate }') AND (ISNUMERIC(IH.Acct) = 1)
                                AND (M.Prefix = 'L') AND (M.Qty = 1) 
                                GROUP BY IH.DateTime, IL.[Document], IL.Part, P.[Desc], IL.Unit, IL.Qty, IL.TrCost, IH.Acct, C.Name, C.Addra, 
                                C.Addrb, C.Addrc, C.Addrd, C.Addre, C.PCode, C.MotDue, C.Area, C.MotDueSort, M.A12, SL.P7, SL.P11
                            ");


            SqlCommand cmd = new SqlCommand(query, conn);
            cmd.CommandTimeout = 300000;
            using (SqlDataAdapter a = new SqlDataAdapter(cmd))
            {
                //add row number
                //table.Columns.Add("#", typeof(int));
                //table.Columns[0].AutoIncrement = true;
                //table.Columns[0].AutoIncrementSeed = 1;
                //table.Columns[0].AutoIncrementStep = 1;
                //
                a.Fill(table);
                dataGridView1.DataSource = table;
                dataGridView1.Columns[0].SortMode = DataGridViewColumnSortMode.NotSortable;

            }
            Cursor.Current = Cursors.Default;

            dataGridView1.MouseClick += new MouseEventHandler(dataGridView1_MouseClick);
        }

        void dataGridView1_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                ContextMenuStrip my_Menu = new ContextMenuStrip();
                int Mouse_XY_Position = dataGridView1.HitTest(e.X, e.Y).RowIndex;

                if (Mouse_XY_Position >= 0)
                {
                    my_Menu.Items.Add("Save as CSV").Name = "Save as CSV";
                    my_Menu.Items.Add("Select All").Name = "Select All";
                    my_Menu.Items.Add("Copy").Name = "Copy";
                    my_Menu.Items.Add("Copy with headers").Name = "Copy with headers";
                }

                my_Menu.Show(dataGridView1, new Point(e.X, e.Y));
                my_Menu.AutoClose = true;
                my_Menu.ItemClicked += new ToolStripItemClickedEventHandler(my_Menu_ItemClicked);
            }
        }

        void my_Menu_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

            switch (e.ClickedItem.Name.ToString())
            {
                case "Save as CSV":

                    int x = 0;
                    string username1 = System.Security.Principal.WindowsIdentity.GetCurrent().Name;
                    int SLength = username1.Length;
                    string username = username1.Substring(10, (username1.Length - 10));
                    string currFile = @"C:\Users\" + username + @"\Documents\MyFile.csv";

                    if (File.Exists(currFile))
                    {
                        do
                        {
                            x++;
                            currFile = @"C:\Users\" + username + @"\Documents\MyFile" + x + ".csv";
                        } while (File.Exists(currFile));
                    }

                    Cursor.Current = Cursors.WaitCursor;

                    StringBuilder sb = new StringBuilder();

                    IEnumerable<string> columnNames = table.Columns.Cast<DataColumn>().
                                                      Select(column => column.ColumnName);
                    sb.AppendLine(string.Join(",", columnNames));

                    foreach (DataRow row in table.Rows)
                    {
                        IEnumerable<string> fields = row.ItemArray.Select(field => field.ToString());
                        sb.AppendLine(string.Join(",", fields));
                    }

                    File.WriteAllText(currFile, sb.ToString());
                    MessageBox.Show("Saved to " + currFile);
                    Cursor.Current = Cursors.Default;
                    break;

                case "Select All":
                    dataGridView1.SelectAll();
                    break;

                case "Copy":
                    if (this.dataGridView1.GetCellCount(DataGridViewElementStates.Selected) > 0)
                    {
                        dataGridView1.ClipboardCopyMode = DataGridViewClipboardCopyMode.EnableWithoutHeaderText;
                        Clipboard.SetDataObject(this.dataGridView1.GetClipboardContent());
                    }
                    break;

                case "Copy with headers":
                    if (this.dataGridView1.GetCellCount(DataGridViewElementStates.Selected) > 0)
                    {
                        dataGridView1.ClipboardCopyMode = DataGridViewClipboardCopyMode.EnableWithAutoHeaderText;
                        Clipboard.SetDataObject(this.dataGridView1.GetClipboardContent());
                    }
                    break;
            }
        }

        private void QuitButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        public void SaveButton_Click(object sender, EventArgs e)
        {
            int x = 0;
            string username1 = System.Security.Principal.WindowsIdentity.GetCurrent().Name;
            int SLength = username1.Length;
            string username = username1.Substring(10, (username1.Length - 10));
            string currFile = @"C:\Users\" + username + @"\Documents\MyFile.csv";

            if (File.Exists(currFile))
            {
                do
                {
                    x++;
                    currFile = @"C:\Users\" + username + @"\Documents\MyFile" + x + ".csv";
                } while (File.Exists(currFile));
            }

            Cursor.Current = Cursors.WaitCursor;

            StringBuilder sb = new StringBuilder();

            IEnumerable<string> columnNames = table.Columns.Cast<DataColumn>().
                                                Select(column => column.ColumnName);
            sb.AppendLine(string.Join(",", columnNames));

            foreach (DataRow row in table.Rows)
            {
                IEnumerable<string> fields = row.ItemArray.Select(field => field.ToString());
                sb.AppendLine(string.Join(",", fields));
            }

            File.WriteAllText(currFile, sb.ToString());
            MessageBox.Show("Saved to " + currFile);
            Cursor.Current = Cursors.Default;
        }
    }
}

