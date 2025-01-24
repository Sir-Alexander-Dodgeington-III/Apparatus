using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Microsoft.VisualBasic;
using Microsoft.VisualBasic.FileIO;

namespace importIOT
{
    public partial class importIOT : Form
    {
        DataTable dt = new DataTable();
        public string pg = "";
        public string branches = "";
        public static string[] dataWords;
        public importIOT()
        {
            InitializeComponent();
            this.CenterToScreen();
            dataGridView1.MouseClick += new MouseEventHandler(dataGridView1_MouseClick);
        }


        // Select file
        public void importButton_Click(object sender, EventArgs e)
        {
            pg = Interaction.InputBox("Please enter the Product Group (PG) for this file", "Product Group");
            branches = Interaction.InputBox("Please enter what should be displayed in the dbo.Branches field", "Branches");
            pg = pg.ToUpper();
            OpenFileDialog OFD1 = new OpenFileDialog();
            OFD1.Filter = "csv files (*.csv)|*.csv";
            OFD1.ShowDialog();
            string filePath = OFD1.FileName;
            //pathTextBox.Text = filePath;
            ReadCSV(filePath);
        }


        private void ReadCSV(string filePath)
        {
            Cursor.Current = Cursors.WaitCursor;
            int i = 1;

            string[] lines = File.ReadAllLines(filePath);


            // Get headers
            if (lines.Length > 0)
            {
                string firstLine = lines[0];

                string[] headerLabels = firstLine.Split(',');

                dt.Columns.Add("Row");

                foreach (string headerWord in headerLabels)
                {
                    dt.Columns.Add(new DataColumn(headerWord));
                }

                // Get cell data
                for (int r = 1; r < lines.Length; r++)
                {

                    string dataWords1 = lines[r].ToString();
                    TextFieldParser parser = new TextFieldParser(new StringReader(dataWords1));

                    parser.HasFieldsEnclosedInQuotes = true;
                    parser.SetDelimiters(",");

                    while (!parser.EndOfData)
                    {
                        dataWords = parser.ReadFields();
                    }

                    parser.Close();

                    DataRow dr = dt.NewRow();
                    int columnIndex = 0;
                    foreach (string headerWord in headerLabels)
                    {
                        dr["Row"] = i;
                        dr[headerWord] = dataWords[columnIndex++];
                    }
                    dt.Rows.Add(dr);
                    i++;
                }

            }

            foreach(DataRow row in dt.Rows)
            {
                row["Part"] = pg + row["Part"].ToString();
            }

            string Destination = System.Configuration.ConfigurationManager.ConnectionStrings["AUTOPART"].ConnectionString;
            foreach (DataRow row in dt.Rows)
            {
                string part = (String)row["Part"];
                using (SqlConnection destinationCon = new SqlConnection(Destination))
                {
                    SqlCommand removeDuplicates = new SqlCommand($@"
                                                                    IF EXISTS (SELECT Part FROM MERI_IOT WHERE Part = '{part}') 
                                                                    BEGIN
                                                                       DELETE FROM MERI_IOT WHERE Part = '{part}'
                                                                    END
                                                        ", destinationCon);
                    destinationCon.Open();
                    removeDuplicates.ExecuteNonQuery();
                    destinationCon.Close();
                }
            }

            // Add to table
            if (dt.Rows.Count > 0)
            {
                dataGridView1.DataSource = dt;
            }
            Cursor.Current = Cursors.Default;
        }

        private void uploadButton_Click(object sender, EventArgs e)
        {
            SqlConnection conn = new SqlConnection(Helper.ConnString("AUTOPART"));
            int i = 0;
            int rows = dataGridView1.Rows.Count;
            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                if (i == rows - 1)
                {
                    break;
                }
                using (SqlCommand cmd = new SqlCommand("sp_MERI_IOTImport", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.Add("@Part", SqlDbType.VarChar).Value = dataGridView1.Rows[i].Cells[1].Value.ToString();
                    //string Part = dataGridView1.Rows[i].Cells[1].Value.ToString();
                    cmd.Parameters.Add("@Stocked", SqlDbType.VarChar).Value = dataGridView1.Rows[i].Cells[2].Value.ToString();
                    //string Stocked = dataGridView1.Rows[i].Cells[2].Value.ToString();
                    cmd.Parameters.Add("@Last12Sales", SqlDbType.VarChar).Value = dataGridView1.Rows[i].Cells[3].Value.ToString();
                    //string Last12Sales = dataGridView1.Rows[i].Cells[3].Value.ToString();
                    cmd.Parameters.Add("@InvDmdYTDLY", SqlDbType.VarChar).Value = dataGridView1.Rows[i].Cells[4].Value.ToString();
                    //string InvDmdYTDLY = dataGridView1.Rows[i].Cells[4].Value.ToString();
                    cmd.Parameters.Add("@VIO", SqlDbType.VarChar).Value = dataGridView1.Rows[i].Cells[5].Value.ToString();
                    //string VIO = dataGridView1.Rows[i].Cells[5].Value.ToString();
                    cmd.Parameters.Add("@Vista", SqlDbType.VarChar).Value = dataGridView1.Rows[i].Cells[6].Value.ToString();
                    //string Vista = dataGridView1.Rows[i].Cells[6].Value.ToString();
                    cmd.Parameters.Add("@ADWRegion", SqlDbType.VarChar).Value = dataGridView1.Rows[i].Cells[7].Value.ToString();
                    //string ADWRegion = dataGridView1.Rows[i].Cells[7].Value.ToString();
                    cmd.Parameters.Add("@Blended", SqlDbType.VarChar).Value = dataGridView1.Rows[i].Cells[8].Value.ToString();
                    //string Blended = dataGridView1.Rows[i].Cells[8].Value.ToString();
                    cmd.Parameters.Add("@Action", SqlDbType.VarChar).Value = dataGridView1.Rows[i].Cells[9].Value.ToString();
                    //string Action = dataGridView1.Rows[i].Cells[9].Value.ToString();
                    cmd.Parameters.Add("@Manual", SqlDbType.VarChar).Value = dataGridView1.Rows[i].Cells[10].Value.ToString();
                    //string Manual = dataGridView1.Rows[i].Cells[10].Value.ToString();
                    cmd.Parameters.Add("@StdMfg", SqlDbType.VarChar).Value = dataGridView1.Rows[i].Cells[11].Value.ToString();
                    //string stdMfg = dataGridView1.Rows[i].Cells[11].Value.ToString();
                    cmd.Parameters.Add("@StdPartGroup", SqlDbType.VarChar).Value = dataGridView1.Rows[i].Cells[12].Value.ToString();
                    //string stdPartGroup = dataGridView1.Rows[i].Cells[12].Value.ToString();
                    cmd.Parameters.Add("@StdPartType", SqlDbType.VarChar).Value = dataGridView1.Rows[i].Cells[13].Value.ToString();
                    //string stdPartType = dataGridView1.Rows[i].Cells[13].Value.ToString();
                    cmd.Parameters.Add("@StdPartDesc", SqlDbType.VarChar).Value = dataGridView1.Rows[i].Cells[14].Value.ToString();
                    //string stdPartDesc = dataGridView1.Rows[i].Cells[14].Value.ToString();
                    cmd.Parameters.Add("@SubBrand", SqlDbType.VarChar).Value = dataGridView1.Rows[i].Cells[15].Value.ToString();
                    //string SubBrand = dataGridView1.Rows[i].Cells[15].Value.ToString();
                    cmd.Parameters.Add("@Grade", SqlDbType.VarChar).Value = dataGridView1.Rows[i].Cells[15].Value.ToString();
                    //string Grade = dataGridView1.Rows[i].Cells[16].Value.ToString();
                    cmd.Parameters.Add("@Super", SqlDbType.VarChar).Value = dataGridView1.Rows[i].Cells[17].Value.ToString();
                    //string Super = dataGridView1.Rows[i].Cells[17].Value.ToString();
                    cmd.Parameters.Add("@MfrPop", SqlDbType.VarChar).Value = dataGridView1.Rows[i].Cells[18].Value.ToString();
                    //string MfrPop = dataGridView1.Rows[i].Cells[18].Value.ToString();
                    cmd.Parameters.Add("@ImpDom", SqlDbType.VarChar).Value = dataGridView1.Rows[i].Cells[19].Value.ToString();
                    //string ImpDom = dataGridView1.Rows[i].Cells[19].Value.ToString();
                    cmd.Parameters.Add("@VahFam", SqlDbType.VarChar).Value = dataGridView1.Rows[i].Cells[20].Value.ToString();
                    //string VehFam = dataGridView1.Rows[i].Cells[20].Value.ToString();
                    cmd.Parameters.Add("@PromMake", SqlDbType.VarChar).Value = dataGridView1.Rows[i].Cells[21].Value.ToString();
                    //string PromMake = dataGridView1.Rows[i].Cells[21].Value.ToString();
                    cmd.Parameters.Add("@Makes", SqlDbType.VarChar).Value = dataGridView1.Rows[i].Cells[22].Value.ToString();
                    //string Makes = dataGridView1.Rows[i].Cells[22].Value.ToString();
                    cmd.Parameters.Add("@PromModel", SqlDbType.VarChar).Value = dataGridView1.Rows[i].Cells[23].Value.ToString();
                    //string PromModel = dataGridView1.Rows[i].Cells[23].Value.ToString();
                    cmd.Parameters.Add("@Models", SqlDbType.VarChar).Value = dataGridView1.Rows[i].Cells[24].Value.ToString();
                    //string Models = dataGridView1.Rows[i].Cells[24].Value.ToString();
                    cmd.Parameters.Add("@MinYear", SqlDbType.VarChar).Value = dataGridView1.Rows[i].Cells[25].Value.ToString();
                    //string MinYear = dataGridView1.Rows[i].Cells[25].Value.ToString();
                    cmd.Parameters.Add("@MaxYear", SqlDbType.VarChar).Value = dataGridView1.Rows[i].Cells[26].Value.ToString();
                    //string MaxYear = dataGridView1.Rows[i].Cells[26].Value.ToString();
                    cmd.Parameters.Add("@PerCar", SqlDbType.VarChar).Value = dataGridView1.Rows[i].Cells[27].Value.ToString();
                    //string PerCar = dataGridView1.Rows[i].Cells[27].Value.ToString();
                    cmd.Parameters.Add("@LostSales", SqlDbType.VarChar).Value = dataGridView1.Rows[i].Cells[28].Value.ToString();
                    //string LostSales = dataGridView1.Rows[i].Cells[28].Value.ToString();
                    cmd.Parameters.Add("@InvAge", SqlDbType.VarChar).Value = dataGridView1.Rows[i].Cells[29].Value.ToString();
                    //string InvAge = dataGridView1.Rows[i].Cells[29].Value.ToString();
                    cmd.Parameters.Add("@XString1", SqlDbType.VarChar).Value = dataGridView1.Rows[i].Cells[30].Value.ToString();
                    //string XString1 = dataGridView1.Rows[i].Cells[30].Value.ToString();
                    cmd.Parameters.Add("@XString2", SqlDbType.VarChar).Value = dataGridView1.Rows[i].Cells[31].Value.ToString();
                    //string XString2 = dataGridView1.Rows[i].Cells[31].Value.ToString();
                    cmd.Parameters.Add("@XString3", SqlDbType.VarChar).Value = dataGridView1.Rows[i].Cells[32].Value.ToString();
                    //string XString3 = dataGridView1.Rows[i].Cells[32].Value.ToString();
                    cmd.Parameters.Add("@XString4", SqlDbType.VarChar).Value = dataGridView1.Rows[i].Cells[33].Value.ToString();
                    //string XString4 = dataGridView1.Rows[i].Cells[33].Value.ToString();
                    cmd.Parameters.Add("@XString5", SqlDbType.VarChar).Value = dataGridView1.Rows[i].Cells[34].Value.ToString();
                    //string XString5 = dataGridView1.Rows[i].Cells[34].Value.ToString();
                    cmd.Parameters.Add("@XNum1", SqlDbType.VarChar).Value = dataGridView1.Rows[i].Cells[35].Value.ToString();
                    //string XNum1 = dataGridView1.Rows[i].Cells[35].Value.ToString();
                    cmd.Parameters.Add("@XNum2", SqlDbType.VarChar).Value = dataGridView1.Rows[i].Cells[36].Value.ToString();
                    //string XNum2 = dataGridView1.Rows[i].Cells[36].Value.ToString();
                    cmd.Parameters.Add("@XNum3", SqlDbType.VarChar).Value = dataGridView1.Rows[i].Cells[37].Value.ToString();
                    //string XNum3 = dataGridView1.Rows[i].Cells[37].Value.ToString();
                    cmd.Parameters.Add("@XNum4", SqlDbType.VarChar).Value = dataGridView1.Rows[i].Cells[38].Value.ToString();
                    //string XNum4 = dataGridView1.Rows[i].Cells[38].Value.ToString();
                    cmd.Parameters.Add("@XNum5", SqlDbType.VarChar).Value = dataGridView1.Rows[i].Cells[39].Value.ToString();
                    //string XNum5 = dataGridView1.Rows[i].Cells[39].Value.ToString();
                    cmd.Parameters.Add("@Branches", SqlDbType.VarChar).Value = branches;
                    //string Branches = dataGridView1.Rows[i].Cells[40].Value.ToString();
                    cmd.Parameters.Add("@TimeStamp", SqlDbType.VarChar).Value = DateTime.Now.ToString();

                    try
                    {
                        conn.Open();
                        cmd.ExecuteNonQuery();
                        conn.Close();
                        i++;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message, "Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                }
            }
            MessageBox.Show("Done!");
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

                    IEnumerable<string> columnNames = dt.Columns.Cast<DataColumn>().
                                                      Select(column => column.ColumnName);
                    sb.AppendLine(string.Join(",", columnNames));

                    foreach (DataRow row in dt.Rows)
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

        private void quitButton_Click(object sender, EventArgs e)
        {
            this.Close();    
        }
    }
}