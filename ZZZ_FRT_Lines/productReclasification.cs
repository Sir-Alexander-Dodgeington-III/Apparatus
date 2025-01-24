using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Microsoft.VisualBasic;
using System.Data.SqlClient;
using System.IO;


namespace ProductReclasification
{
    public partial class ProductReclasification : Form
    {
        public string pgRange = "";
        public string Branch = "";
        public string PG = "";
        public string Range = "";
        public decimal slope;
        public decimal absSlope;
        public decimal NabsSlope;
        public decimal AdjTrendUsage;
        public decimal sumAdjTrendUsage;
        public decimal AdjUsage;
        public int TotalUsageLast15BranchCount;
        public int TotalUsageLast15;
        public int A;
        public int BCD;
        public DataTable table = new DataTable();
        public DataTable sortedTable = new DataTable();
        public string result;
        public string username1 = System.Security.Principal.WindowsIdentity.GetCurrent().Name;


        public ProductReclasification()
        {
            InitializeComponent();
            this.CenterToScreen();

            string user = username1.Substring(10, (username1.Length - 10));

            SqlConnection conn = new System.Data.SqlClient.SqlConnection(Helper.ConnString("AUTOPART"));
            pgRange = Interaction.InputBox("Please input a Product Group (PG) or Range", "Enter PG or Range", "");
            A = Convert.ToInt32(Interaction.InputBox("Below what percentage should be considered A class", "Specify A's", "60"));
            BCD = Convert.ToInt32(Interaction.InputBox("What additional percentage should be included for B, C, and D classes?", "Specify B, C, and D's", "20"));

            if (pgRange.Length == 3)
            {
                
                conn.Open();
                string query = ($@"
                                SELECT TOP(1) PG FROM Product WHERE Pg = '{pgRange}'
                            ");
                SqlCommand cmd = new SqlCommand(query, conn);
                result = (string)cmd.ExecuteScalar();
                conn.Close();
                if (result == "")
                {
                    MessageBox.Show($"The product group '{pgRange}' is not valid");
                    return;
                }
                else
                {

                    using (SqlCommand cmd1 = new SqlCommand("sp_MERI_ReclassHistory", conn))
                    {
                        cmd1.CommandType = CommandType.StoredProcedure;

                        cmd1.Parameters.Add("@PG", SqlDbType.VarChar).Value = pgRange.ToString();
                        cmd1.Parameters.Add("@Range", SqlDbType.VarChar).Value = "ALL";
                        cmd1.Parameters.Add("@Inits", SqlDbType.VarChar).Value = user.ToString();
                        cmd1.Parameters.Add("@SubKey1", SqlDbType.VarChar).Value = A.ToString();
                        cmd1.Parameters.Add("@SubKey2", SqlDbType.VarChar).Value = BCD.ToString();

                        try
                        {
                            conn.Open();
                            cmd1.ExecuteNonQuery();
                            conn.Close();
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message, "Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }
                    }
                }
                conn.Close();
            }
            else
            {
                conn.Open();
                string query = ($@"
                                select top(1) Range FROM Product WHERE Range = '{pgRange}'
                            ");
                SqlCommand cmd = new SqlCommand(query, conn);
                result = (string)cmd.ExecuteScalar();
                conn.Close();
                if (result == "")
                {
                    MessageBox.Show($"The product range '{pgRange}' is not valid");
                    return;
                }
                else
                {
                    using (SqlCommand cmd1 = new SqlCommand("sp_MERI_ReclassHistory", conn))
                    {
                        cmd1.CommandType = CommandType.StoredProcedure;

                        cmd1.Parameters.Add("@PG", SqlDbType.VarChar).Value = pgRange.Substring(0,3).ToString();
                        cmd1.Parameters.Add("@Range", SqlDbType.VarChar).Value = pgRange;
                        cmd1.Parameters.Add("@Inits", SqlDbType.VarChar).Value = user.ToString();
                        cmd1.Parameters.Add("@SubKey1", SqlDbType.VarChar).Value = A.ToString();
                        cmd1.Parameters.Add("@SubKey2", SqlDbType.VarChar).Value = BCD.ToString();

                        try
                        {
                            conn.Open();
                            cmd1.ExecuteNonQuery();
                            conn.Close();
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message, "Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }
                    }
                }
                conn.Close();
            }


            BCD = A + BCD;
            if (BCD == 100)
            {
                MessageBox.Show("The total of the proveded percentages can not exceed 99 as this leaves nothing to calculate for classes E and N");
                return;
            }
            /*Branch = Interaction.InputBox("Please input a Branch", "Enter Branch", "");*/
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            table.Clear();
            dataGridView1.DataSource = null;

            if (pgRange.Length == 3)
            {
                PG = "'" + pgRange + "'";
                Range = "NULL";
            }
            else
            {
                Range = "'" + pgRange + "'";
                PG = "NULL";
            }

            Cursor.Current = Cursors.WaitCursor;
            SqlConnection conn = new SqlConnection(Helper.ConnString("AUTOPART"));
            conn.Open();


            string query = ($@"
                                DECLARE @PG VARCHAR(10)
                                DECLARE @Range VARCHAR(10)

                                SET @PG = {PG}
                                SET @Range = {Range}
                                
                                ;WITH CTE1 (Part, Description, PG, Range, Class, Slope, P11) as(
                                SELECT DISTINCT Part, Description, PG, Range, Class, Slope, ISNULL(P11, 0) FROM VW_MERI_Pro WHERE PG = IIF(@PG IS NULL, PG, @PG) AND Range = IIF(@Range IS NULL, Range, @Range)
                                ),

                                CTE2 (Part, PG, TotalUsageLast15, TotalUsageLast15BranchCount) as(
                                SELECT Part, PG, TotalUsageLast15, TotalUsageLast15BranchCount FROM VW_MERI_Pro
                                )

                                SELECT 
                                
                                DISTINCT I.Part, 
                                I.Description,
                                I.PG,
                                I.Range,
                                CAST(II.TotalUsageLast15 as int) as TotalUsageLast15, 
                                II.TotalUsageLast15BranchCount, 
                                CAST(IsNull(I.Slope, 0) as decimal(18,4)) as Slope, 
                                CAST(((II.TotalUsageLast15 * 1.2) + (II.TotalUsageLast15BranchCount * .8))/2 as decimal(18,2)) as 'Adj Usage', 
                                '' as AdjTrendUsage,
                                '' as '% Sales',
                                '' as CumulativeUsage, 
                                I.Class,
                                '' as AMU,
                                I.P11

                                FROM CTE1 I LEFT OUTER JOIN
                                CTE2 II ON I.Part = II.Part

                                WHERE (I.PG = IIF(@PG IS NULL, I.PG, @PG) AND I.Range = IIF(@Range IS NULL, I.Range, @Range))
                                ORDER BY CAST(IsNull(I.Slope, 0) as decimal(18,4)) Desc
                            ");


            SqlCommand cmd = new SqlCommand(query, conn);
            cmd.CommandTimeout = 300000;
            using (SqlDataAdapter a = new SqlDataAdapter(cmd))
            {
                a.Fill(table);
                conn.Close();
                int i = 0;
                int x = 0;

                List<decimal> array = new List<decimal>();
                x = 0;
                foreach (DataRow row in table.Rows)
                {
                    if (Math.Abs(Convert.ToDecimal(table.Rows[x]["Slope"])) > .1763M)
                    {
                        array.Add(Math.Abs(Convert.ToDecimal(table.Rows[x]["Slope"])));
                    }
                    x++;
                }

                var sumArray = array.Sum();
                var countArray = array.Count();

                if (sumArray != 0)
                {
                    absSlope = Convert.ToDecimal(sumArray / countArray);
                    NabsSlope = Convert.ToDecimal(sumArray / countArray) * -1;
                }
                else
                {
                    absSlope = 0;
                    NabsSlope = 0;
                }

                x = 0;
                foreach (DataRow row in table.Rows)
                {
                    TotalUsageLast15BranchCount = Convert.ToInt32(table.Rows[x]["TotalUsageLast15BranchCount"]);
                    TotalUsageLast15 = Convert.ToInt32(table.Rows[x]["TotalUsageLast15"]);
                    slope = Convert.ToDecimal(table.Rows[x]["Slope"]);
                    AdjUsage = (decimal)table.Rows[x]["Adj Usage"];


                    // **UP**
                    if (slope >= .1763M && slope <= absSlope)
                    {
                        table.Rows[x]["AdjTrendUsage"] = decimal.Multiply(AdjUsage, 1.1M);
                    }
                    // **BIG UP**
                    if (slope >= absSlope)
                    {
                        table.Rows[x]["AdjTrendUsage"] = Convert.ToDecimal(AdjUsage * 1.25M);
                    }
                    // **DOWN**
                    if (slope <= -.1763M && slope >= NabsSlope)
                    {
                        table.Rows[x]["AdjTrendUsage"] = Convert.ToDecimal(AdjUsage * .9M);
                    }
                    // **BIG DOWN**
                    if (slope <= NabsSlope)
                    {
                        table.Rows[x]["AdjTrendUsage"] = Convert.ToDecimal(AdjUsage * .75M);
                    }
                    // **FLAT**
                    if (slope >= -.1763M && slope <= .1763M)
                    {
                        table.Rows[x]["AdjTrendUsage"] = Convert.ToDecimal(AdjUsage);
                    }

                    decimal decvar = Convert.ToDecimal(table.Rows[x]["AdjTrendUsage"]);
                    decvar = decimal.Round(decvar, 2, MidpointRounding.AwayFromZero);
                    table.Rows[x]["AdjTrendUsage"] = decvar;

                    x++;
                }

                x = 0;

                List<decimal> array1 = new List<decimal>();
                foreach (DataRow row in table.Rows)
                {
                    array1.Add(Convert.ToDecimal(table.Rows[x]["AdjTrendUsage"]));
                    x++;
                }

                sumAdjTrendUsage = array1.Sum();

                x = 0;
                foreach (DataRow row in table.Rows)
                {
                    if (TotalUsageLast15 == 0 || TotalUsageLast15BranchCount == 0)
                    {
                        table.Rows[x]["% Sales"] = 0;
                    }
                    else
                    {
                        //Type type = table.Rows[x]["AdjTrendUsage"].GetType();
                        AdjTrendUsage = Convert.ToDecimal(table.Rows[x][7]);//Decimal.Parse((string)table.Rows[x]["AdjTrendUsage"], NumberStyles.AllowDecimalPoint);
                        table.Rows[x]["% Sales"] = Math.Round(Convert.ToDecimal((AdjTrendUsage / sumAdjTrendUsage)* 100),2);
                        table.Rows[x]["% Sales"] = Convert.ToDecimal(table.Rows[x]["% Sales"]);
                    }
                    x++;
                }

                int w = 0;
                foreach (DataRow row in table.Rows)
                {
                    if (w <= table.Rows.Count)
                    {
                        conn.Open();
                        string query1 = ($@"
                        INSERT INTO MERI_CalculatedTrend (Part,[Description],PG,[Range],TotalUsageLast15,TotalUsageLast15BranchCount,slope,[Adj Usage],AdjTrendUsage,[% Sales],Class,P11) 
                        VALUES (
                        '{Convert.ToString(table.Rows[w]["Part"])}',
                        '{Convert.ToString(table.Rows[w]["Description"])}',
                        '{Convert.ToString(table.Rows[w]["PG"])}',
                        '{Convert.ToString(table.Rows[w]["Range"])}',
                        {Convert.ToInt32(table.Rows[w]["TotalUsageLast15"])} ,
                        {Convert.ToInt32(table.Rows[w]["TotalUsageLast15BranchCount"])} ,
                        {Convert.ToDecimal(table.Rows[w]["Slope"])} ,
                        {Convert.ToDecimal(table.Rows[w]["Adj Usage"])} ,
                        {Convert.ToDecimal(table.Rows[w]["AdjTrendUsage"])} ,
                        {Convert.ToDecimal(table.Rows[w]["% Sales"])},
                        '{Convert.ToString(table.Rows[w]["Class"])}',
                        {Convert.ToDecimal(table.Rows[w]["P11"])})
                        ");
                        SqlCommand cmd1 = new SqlCommand(query1, conn);
                        try
                        {
                            cmd1.ExecuteNonQuery();
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message, "Error inserting recalc into table", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }
                        w++;
                        conn.Close();
                    }
                }


                conn.Open();
                string query2 = ($@"
                                SELECT * FROM MERI_CalculatedTrend ORDER BY AdjTrendUsage DESC
                            ");


                SqlCommand cmd2 = new SqlCommand(query2, conn);
                cmd.CommandTimeout = 300000;
                using (SqlDataAdapter b = new SqlDataAdapter(cmd2))
                {
                    b.Fill(sortedTable);
                }
                conn.Close();

                conn.Open();
                string query3 = ($@"
                        DELETE FROM MERI_CalculatedTrend
                        ");
                SqlCommand cmd3 = new SqlCommand(query3, conn);
                cmd3.ExecuteNonQuery();
                conn.Close();


                foreach (DataRow row in sortedTable.Rows)
                {

                    if (i == 0)
                    {
                        decimal value0 = Convert.ToDecimal(sortedTable.Rows[i]["% Sales"]);
                        sortedTable.Rows[i]["CumulativeUsage"] = value0.ToString();
                        slope = Convert.ToDecimal(sortedTable.Rows[i]["Slope"]);

                        if (value0 <= A)
                        {
                            sortedTable.Rows[i]["New Class"] = "A";
                        }
                        else if (value0 <= BCD && value0 > A && slope >= .1763M)
                        {
                            sortedTable.Rows[i]["New Class"] = "B";
                        }
                        else if (value0 <=BCD && value0 > A && slope < .1763M && slope > -.1763M)
                        {
                            sortedTable.Rows[i]["New Class"] = "C";
                        }
                        else if (value0 <= BCD && value0 > A && slope <= -.1763M)
                        {
                            sortedTable.Rows[i]["New Class"] = "D";
                        }
                        else if (value0 > BCD && value0 < 100)
                        {
                            sortedTable.Rows[i]["New Class"] = "E";
                        }

                        DataColumn Col = sortedTable.Columns.Add("Num", System.Type.GetType("System.Int32"));
                        Col.SetOrdinal(0);// to put the column in position 0;
                        sortedTable.Rows[i]["Num"] = i + 1;
                    }
                    else
                    {
                        decimal value1 = Convert.ToDecimal(sortedTable.Rows[i - 1]["CumulativeUsage"]);
                        decimal value2 = Convert.ToDecimal(sortedTable.Rows[i]["% Sales"]);
                        decimal endValue = Convert.ToDecimal((value1 + value2));
                        decimal slope = Convert.ToDecimal(sortedTable.Rows[i]["Slope"]);




                        sortedTable.Rows[i]["CumulativeUsage"] = endValue;

                        if (endValue <= A)
                        {
                            sortedTable.Rows[i]["New Class"] = "A";
                        }
                        else if (endValue <= BCD && endValue > A && slope >= .1763M)
                        {
                            sortedTable.Rows[i]["New Class"] = "B";
                        }
                        else if (endValue <= BCD && endValue > A && slope < .1763M && slope > -.1763M)
                        {
                            sortedTable.Rows[i]["New Class"] = "C";
                        }
                        else if (endValue <= BCD && endValue > A && slope <= -.1763M)
                        {
                            sortedTable.Rows[i]["New Class"] = "D";
                        }
                        else if (endValue > BCD && endValue < 100)
                        {
                            sortedTable.Rows[i]["New Class"] = "E";
                        }

                        TotalUsageLast15 = Convert.ToInt32(sortedTable.Rows[i]["TotalUsageLast15"]);
                        if (TotalUsageLast15 == 0 || endValue >= 100)
                        {
                            sortedTable.Rows[i]["New Class"] = "N";
                            if (endValue > 100)
                            {
                                sortedTable.Rows[i]["CumulativeUsage"] = 100;
                                sortedTable.Rows[i]["% Sales"] = 0.00;
                            }
                        }
                    }

                    sortedTable.Rows[i]["Num"] = i + 1;
                    i++;
                }

                //sortedTable.Columns["AMU"].ColumnName = "New Class";
                dataGridView1.DataSource = sortedTable;
                foreach (DataGridViewColumn column in dataGridView1.Columns)
                {
                    column.SortMode = DataGridViewColumnSortMode.NotSortable;
                }
            }
            Cursor.Current = Cursors.Default;
            dataGridView1.ClearSelection();
        }

        void dataGridView1_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                ContextMenuStrip my_Menu = new System.Windows.Forms.ContextMenuStrip();
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

                    IEnumerable<string> columnNames = sortedTable.Columns.Cast<DataColumn>().
                                                      Select(column => column.ColumnName);
                    sb.AppendLine(string.Join(",", columnNames));

                    foreach (DataRow row in sortedTable.Rows)
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
                        dataGridView1.ClipboardCopyMode = DataGridViewClipboardCopyMode.EnableAlwaysIncludeHeaderText;
                        Clipboard.SetDataObject(this.dataGridView1.GetClipboardContent());
                    }
                    break;
            }
        }

        private void SaveAsButton_Click(object sender, EventArgs e)
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

            IEnumerable<string> columnNames = sortedTable.Columns.Cast<DataColumn>().
                                                Select(column => column.ColumnName);
            sb.AppendLine(string.Join(",", columnNames));

            foreach (DataRow row in sortedTable.Rows)
            {
                IEnumerable<string> fields = row.ItemArray.Select(field => field.ToString());
                sb.AppendLine(string.Join(",", fields));
            }

            File.WriteAllText(currFile, sb.ToString());
            MessageBox.Show("Saved to " + currFile);
            Cursor.Current = Cursors.Default;
        }

        private void closeButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
