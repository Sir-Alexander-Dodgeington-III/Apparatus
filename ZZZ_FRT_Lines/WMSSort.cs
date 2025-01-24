using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using System.Data.SqlClient;

namespace wmsSort
{
    public partial class wmsSort : Form
    {
        public string filePath;
        public string[] lines;
        public bool isNumber;
        public int numericValue;
        public string pg;
        public int sortRule;
        public string query;
        public DialogResult dialogResult;
        DataTable dt = new DataTable();
        DataTable dt1 = new DataTable();
        DataTable sort = new DataTable();
        public string origPartNo;
        private TextBox CurrentTextbox = null;

        public wmsSort()
        {
            InitializeComponent();
            this.CenterToScreen();
            pg = pgTextbox.Text;

            dt.Clear();
            dt1.Clear();
            sort.Clear();
        }

        // sort button controls
        // Find sort rules for entered product group
        private void sortButton_Click(object sender, EventArgs e)
        {
            SqlConnection conn1 = new SqlConnection(Helper.ConnString("AUTOPART"));
            conn1.Open();
            string query = ($@"
                    SELECT a1 FROM MERI_WMSSort WHERE subKey = '{pgTextbox.Text}'
                ");

            SqlCommand cmd1 = new SqlCommand(query, conn1);
            using (SqlDataAdapter a = new SqlDataAdapter(cmd1))
            {
                a.Fill(sort);
            }
            conn1.Close();


            // If sort rules are found, fill datatable with list of parts in PG. If not stop program
            if (sort.Rows.Count == 0)
            {
                dialogResult = MessageBox.Show($@"No sort settings found for product group {pgTextbox.Text.ToUpper()}. Would you like to apply the default sort rule?", "Notice", MessageBoxButtons.YesNo);
                if (dialogResult == DialogResult.Yes)
                {
                    pg = pgTextbox.Text;
                    SqlConnection conn0 = new SqlConnection(Helper.ConnString("AUTOPART"));
                    using (SqlCommand cmd = new SqlCommand("sp_MERI_addDefaultWMSSort", conn0))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add("@PG", SqlDbType.VarChar).Value = pg.ToString();

                        try
                        {
                            conn0.Open();
                            cmd.ExecuteNonQuery();
                            conn0.Close();

                            conn0.Open();
                            string query2 = ($@"
                                            SELECT a1 FROM MERI_WMSSort WHERE subKey = '{pgTextbox.Text}'
                                        ");

                            SqlCommand cmd2 = new SqlCommand(query2, conn0);
                            using (SqlDataAdapter a = new SqlDataAdapter(cmd2))
                            {
                                a.Fill(sort);
                            }
                            conn0.Close();

                            sortRule = Convert.ToInt32(sort.Rows[0][0].ToString());
                            conn0.Open();
                            string query3 = ($@"
                                SELECT KeyCode FROM Product WHERE PG = '{pgTextbox.Text}'
                            ");

                            SqlCommand cmd4 = new SqlCommand(query3, conn0);
                            using (SqlDataAdapter a1 = new SqlDataAdapter(cmd4))
                            {
                                a1.Fill(dt);
                            }
                            conn0.Close();


                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message, "Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }
                    }
                }
                else
                {
                    return;
                }
            }
            else
            {
                // Set sort rule based on query result
                sortRule = Convert.ToInt32(sort.Rows[0][0].ToString());

                SqlConnection conn2 = new SqlConnection(Helper.ConnString("AUTOPART"));
                conn2.Open();
                string query2 = ($@"
                    SELECT KeyCode FROM Product WHERE PG = '{pgTextbox.Text}'
                ");

                SqlCommand cmd2 = new SqlCommand(query2, conn2);
                using (SqlDataAdapter a = new SqlDataAdapter(cmd2))
                {
                    a.Fill(dt);
                }
                conn2.Close();
            }

            // add columns to datatable for splitting the part number into its alpha and numeric components
            dt.Columns.Add($"string1");
            dt.Columns.Add($"string2");
            dt.Columns.Add($"string3");
            dt.Columns.Add($"string4");
            dt.Columns.Add($"string5");
            dt.Columns.Add($"string6");
            dt.Columns.Add($"string7");
            dt.Columns.Add($"string8");
            dt.Columns.Add($"string9");
            dt.Columns.Add($"string10");

            //looping through datatable and spitting part numbers into their alpha and numeric components
            for (int r = 0; r < dt.Rows.Count; r++)
            {
                origPartNo = dt.Rows[r][0].ToString();

                string[] substrings1 = Regex.Split(origPartNo, @"[^A-Z0-9]+|(?<=[A-Z])(?=[0-9])|(?<=[0-9])(?=[A-Z])");

                List<string> list1 = new List<string>(substrings1);

                while (list1.Count() < 10)
                {
                    list1.Add("");
                }

                if (list1[1].Length > 0)
                {
                    if (isNumber = list1[1].All(char.IsLetter)) 
                    { 
                        list1.Insert(1, "");
                        list1.RemoveAt(list1.Count - 1);
                    }
                }
                if (list1[2].Length > 0)
                {
                    if (isNumber = list1[2].All(char.IsNumber)) 
                    { 
                        list1.Insert(2, "");
                        list1.RemoveAt(list1.Count - 1);
                    }
                }
                if (list1[3].Length > 0)
                {
                    if (isNumber = list1[3].All(char.IsLetter)) 
                    {
                        list1.Insert(3, "");
                        list1.RemoveAt(list1.Count - 1);
                    }
                }
                if (list1[4].Length > 0)
                {
                    if (isNumber = list1[4].All(char.IsNumber)) 
                    {
                        list1.Insert(4, "");
                        list1.RemoveAt(list1.Count - 1);
                    }
                }
                if (list1[5].Length > 0)
                {
                    if (isNumber = list1[5].All(char.IsLetter)) 
                    {
                        list1.Insert(5, "");
                        list1.RemoveAt(list1.Count - 1);
                    }
                }
                if (list1[6].Length > 0)
                {
                    if (isNumber = list1[6].All(char.IsNumber)) 
                    {
                        list1.Insert(6, "");
                        list1.RemoveAt(list1.Count - 1);
                    }
                }
                if (list1[7].Length > 0)
                {
                    if (isNumber = list1[7].All(char.IsLetter)) 
                    {
                        list1.Insert(7, "");
                        list1.RemoveAt(list1.Count - 1);
                    }
                }
                if (list1[8].Length > 0)
                {
                    if (isNumber = list1[8].All(char.IsNumber)) 
                    {
                        list1.Insert(8, "");
                        list1.RemoveAt(list1.Count - 1);
                    }
                }

                string[] substrings = list1.ToArray();

                for (int i = 1; i < substrings.Length; i++)
                {
                    foreach (var item in substrings)
                    {
                            dt.Rows[r][i] = item.ToString();
                            i++;
                    }
                }
            }

            // Post split part number into the MERI_WMSSortWorking table
            using (SqlConnection dbConnection = new SqlConnection(Helper.ConnString("AUTOPART")))
            {
                dbConnection.Open();
                string query0 = "DELETE FROM MERI_WMSSortWorking";
                SqlCommand command0 = new SqlCommand(query0, dbConnection);
                command0.ExecuteNonQuery();

                string query2 = "DELETE FROM MERI_WMSSortFinal";
                SqlCommand command2 = new SqlCommand(query2, dbConnection);
                command2.ExecuteNonQuery();

                using (SqlBulkCopy s = new SqlBulkCopy(dbConnection))
                {
                    s.DestinationTableName = "MERI_WMSSortWorking";
                    s.WriteToServer(dt);
                }

                string query1 = "DELETE FROM MERI_WMSSortWorking WHERE KeyCode = 'KeyCode'";
                SqlCommand command = new SqlCommand(query1, dbConnection);
                command.ExecuteNonQuery();
                dbConnection.Close();
            }


            dt.Clear();
            SqlConnection conn = new SqlConnection(Helper.ConnString("AUTOPART"));
            conn.Open();

            // return the list of parts in their sort order based on their sort rule
            switch (sortRule)
            {
                case 1: //  1A, 1N, 2A, 2N, 3A, 3N
                    query = ($@"
                        SELECT KeyCode,  '' as WMS_Sort FROM MERI_WMSSortWorking ORDER BY [1A], LEN([1N]), [1N] ASC, [2A], LEN([2N]), [2N] ASC, [3A], LEN([3N]), [3N] ASC
                    ");
                    break;

                case 2: //  1N, 1A, 2N, 2A, 3N, 3A
                    query = ($@"
                        SELECT KeyCode,  '' as WMS_Sort FROM MERI_WMSSortWorking ORDER BY LEN([1N]), [1N] ASC, [1A], LEN([2N]), [2N] ASC, [2A], LEN([3N]), [3N], [3A] ASC
                    ");
                    break;
                case 3: //  1A, 1N, 2A, 2N, 3A, 3N 
                    query = ($@"
                        SELECT KeyCode,  '' as WMS_Sort FROM MERI_WMSSortWorking ORDER BY [1A], LEN([1N]), [1N] ASC, [2A], LEN([2N]), [2N] ASC, [3A], LEN([3N]), [3N] ASC
                    ");
                    break;

                case 4: //  1N, 1A, 2N, 2A, 3N, 3A
                    query = ($@"
                        SELECT KeyCode,  '' as WMS_Sort FROM MERI_WMSSortWorking ORDER BY LEN([1N]), [1N] ASC, [1A], LEN([2N]), [2N] ASC, [3A], LEN([3N]), [3N] ASC
                    ");
                    break;

                case 5: //  2N, 1N
                    query = ($@"
                        SELECT KeyCode,  '' as WMS_Sort FROM MERI_WMSSortWorking ORDER BY LEN([2N]), [2N] ASC, LEN([1N]), [1N] ASC
                    ");
                    break;

                case 6: //  1N, 2N, 1A, 2A
                    query = ($@"
                        SELECT KeyCode,  '' as WMS_Sort FROM MERI_WMSSortWorking ORDER BY LEN([1N]), [1N] ASC, LEN([2N]), [2N] ASC, [1A], [2A]
                    ");
                    break;

                case 7: //  2N, 2A, 1N, 1A, 3N, 3A
                    query = ($@"
                        SELECT KeyCode,  '' as WMS_Sort FROM MERI_WMSSortWorking ORDER BY LEN([2N]),[2N] ASC, [2A], LEN([1N]), [1N] ASC, [1A], LEN([3N]), [3N] ASC, [3A]
                    ");
                    break;

                case 8: //  1N, 2A, 3N
                    query = ($@"
                        SELECT KeyCode,  '' as WMS_Sort FROM MERI_WMSSortWorking ORDER BY LEN([1N]), [1N] ASC, [2A], LEN([3N]), [3N] ASC
                    ");
                    break;

                case 9: //  1A, 1N, 2N
                    query = ($@"
                        SELECT KeyCode,  '' as WMS_Sort FROM MERI_WMSSortWorking ORDER BY [1A], LEN([1N]), [1N] ASC, LEN([2N]), [2N] ASC
                    ");
                    break;

                case 10: // 1A, 2A, 1N, 2N
                    query = ($@"
                        SELECT KeyCode,  '' as WMS_Sort FROM MERI_WMSSortWorking ORDER BY [1A], [2A], LEN([1N]), [1N] ASC, LEN([2N]), [2N] ASC
                    ");
                    break;

                case 11: // 1N, 3N, 1A, 2A, 3A
                    query = ($@"
                        SELECT KeyCode,  '' as WMS_Sort FROM MERI_WMSSortWorking ORDER BY LEN([1N]), [1N] ASC, LEN([3N]), [3N] ASC, [1A], [2A], [3A]
                    ");
                    break;

                case 12: // 1A, 1N, 2N, 2A, 3N, 3A
                    query = ($@"
                        SELECT KeyCode,  '' as WMS_Sort FROM MERI_WMSSortWorking ORDER BY [1A], LEN([1N]), [1N] ASC, LEN([2N]), [2N] ASC, [2A], LEN([3N]), [3N] ASC, [3A]
                    ");
                    break;

                case 13: // 1N, 1A, 2N, 2A, 3N, 3A
                    query = ($@"
                        SELECT KeyCode,  '' as WMS_Sort FROM MERI_WMSSortWorking ORDER BY LEN([1N]), [1N] ASC, [1A], LEN([2N]), [2N] ASC, [2A], LEN([3N]), [3N] ASC, [3A]
                    ");
                    break;
            }

            SqlConnection conn3 = new SqlConnection(Helper.ConnString("AUTOPART"));
            conn3.Open();

            SqlCommand cmd3 = new SqlCommand(query, conn3);
            using (SqlDataAdapter a = new SqlDataAdapter(cmd3))
            {
                a.Fill(dt1);
            }
            

            // create sort value, and pad the string so that it is always at 5 characters long
            for (int row = 0; row < dt1.Rows.Count; row++)
            {
                dt1.Rows[row][1] = Convert.ToString(row + 1).PadLeft(5, '0');
            }

            using (SqlBulkCopy s = new SqlBulkCopy(conn3))
            {
                s.DestinationTableName = "MERI_WMSSortFinal";
                s.WriteToServer(dt1);
            }

            // Apply the sort to the Product table
            using (SqlCommand cmd = new SqlCommand("sp_MERI_WMSSortUpdateProduct", conn3))
            {
                try
                {
                    cmd.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }
             
            conn3.Close();

            MessageBox.Show($@"{pgTextbox.Text.ToUpper()} has been resequenced.");

            this.Close();
        }

        // close button controls
        private void closeButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        // pgTextbox controls
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
    }
}
