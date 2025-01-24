using System;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace StockLevelling_New
{
    public partial class StockLevelling_New : Form
    {
        public DataTable table = new DataTable();
        public DataTable table2 = new DataTable();
        public DataTable table3 = new DataTable();
        public DataTable table4 = new DataTable();
        public DataTable table5 = new DataTable();
        public DataTable dtId = new DataTable();
        public DataTable dtId1 = new DataTable();
        public DataRow dr;
        public int index;
        public int branchIndex;
        public string branches;
        public string query;
        public string query2;
        public string stringBranch;
        public string stringPG;
        public string stringRange;
        public string DC;
        public string[] pgsArray;
        public string[] rngsArray;
        public string line;
        public string Lines;
        public string PGphrase;
        public string rangePhrase;
        public string pgs;
        public string rngs;

        SqlConnection conn = new SqlConnection(Helper.ConnString("AUTOPART"));
        public StockLevelling_New()
        {
            InitializeComponent();
            this.CenterToScreen();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            
            conn.Open();
            DataSet ds = new DataSet();
            SqlDataAdapter adapter = new SqlDataAdapter(
            "SELECT DISTINCT Branch FROM Branches WHERE Branch NOT IN('BR20','BR25','BR43','BR44','BR56','BR57','BR58','BR59', 'BR47', 'BR40', 'BR61', 'BR30', 'BR60', 'BR80', 'BR91', 'BR92', 'BR93') ORDER BY Branch ASC", conn);
            adapter.Fill(ds);
            adapter.Fill(table);
            this.checkedListBox1.DataSource = ds.Tables[0];
            this.checkedListBox1.DisplayMember = "Branch";
            conn.Close();
        }

        private void applyButton_Click(object sender, EventArgs e)
        {
            if (conn.State == ConnectionState.Open)
            {
                conn.Close();
            }

            Cursor.Current = Cursors.WaitCursor;

            foreach (object branch in checkedListBox1.CheckedItems)
            {
                foreach (object pg in checkedListBox2.Items)
                {
                    int pgIndex = checkedListBox2.Items.IndexOf(pg);
                    stringPG = table2.Rows[pgIndex][0].ToString();
                    int branchIndex = checkedListBox1.Items.IndexOf(branch);
                    stringBranch = table.Rows[branchIndex][0].ToString();
                    conn.Open();
                    string query = ($@"
                                    SELECT * FROM MERI_StockLevelling WHERE xString1 = '{stringPG}' AND Branch = '{stringBranch}' AND Prefix = 'P'
                                ");
                    SqlCommand cmd = new SqlCommand(query, conn);
                    string result = (string)cmd.ExecuteScalar();
                    conn.Close();

                    if (result == null)
                    {
                        conn.Open();
                        string query2 = ($@"
                                INSERT INTO MERI_StockLevelling VALUES ('P','{stringBranch}','{stringPG}')
                            ");
                        SqlCommand cmd2 = new SqlCommand(query2, conn);
                        cmd2.ExecuteScalar();
                        conn.Close();
                    }
                    else
                    {
                        if (checkedListBox2.GetItemCheckState(pgIndex) == CheckState.Checked)
                        {

                        }
                        else
                        {
                            conn.Open();
                            string query2 = ($@"
                                DELETE FROM MERI_StockLevelling WHERE Prefix = 'P' AND Branch = '{stringBranch}' AND xString1 = '{stringPG}'
                            ");
                            SqlCommand cmd2 = new SqlCommand(query2, conn);
                            cmd2.ExecuteScalar();
                            conn.Close();
                        }
                    }
                }
            }

            foreach (object branch in checkedListBox1.CheckedItems)
            {
                foreach (object range in checkedListBox3.Items)
                {
                    int rangeIndex = checkedListBox3.Items.IndexOf(range);
                    stringRange = table3.Rows[rangeIndex][0].ToString();
                    int branchIndex = checkedListBox1.Items.IndexOf(branch);
                    stringBranch = table.Rows[branchIndex][0].ToString();
                    conn.Open();
                    string query = ($@"
                                    SELECT * FROM MERI_StockLevelling WHERE xString1 = '{stringRange}' AND Branch = '{stringBranch}' AND Prefix = 'R'
                                ");
                    SqlCommand cmd = new SqlCommand(query, conn);
                    string result = (string)cmd.ExecuteScalar();
                    conn.Close();

                    if (result == null)
                    {
                        conn.Open();
                        string query2 = ($@"
                                INSERT INTO MERI_StockLevelling VALUES ('R','{stringBranch}','{stringRange}')
                            ");
                        SqlCommand cmd2 = new SqlCommand(query2, conn);
                        cmd2.ExecuteScalar();
                        conn.Close();
                    }
                    else
                    {
                        if (checkedListBox3.GetItemCheckState(rangeIndex) == CheckState.Checked)
                        {

                        }
                        else
                        {
                            conn.Open();
                            string query2 = ($@"
                                DELETE FROM MERI_StockLevelling WHERE Prefix = 'R' AND Branch = '{stringBranch}' AND xString1 = '{stringRange}'
                            ");
                            SqlCommand cmd2 = new SqlCommand(query2, conn);
                            cmd2.ExecuteScalar();
                            conn.Close();
                        }
                    }
                }
            }


            conn.Open();
            string query1 = ($@"
                                SELECT BranchGroup FROM Branches WHERE Branch = '{stringBranch}'
                            ");
            SqlCommand cmd3 = new SqlCommand(query1, conn);
            DC = (string)cmd3.ExecuteScalar();
            conn.Close();

            conn.Open();
            SqlDataAdapter adapter = new SqlDataAdapter(
            $@"
                    SELECT xString1 FROM MERI_StockLevelling WHERE Branch = '{stringBranch}' AND Prefix = 'P'
                   ", conn);
            adapter.Fill(table4);
            conn.Close();
            pgsArray = table4.Rows.OfType<DataRow>().Select(k => k[0].ToString()).ToArray();


            conn.Open();
            SqlDataAdapter adapter1 = new SqlDataAdapter(
            $@"
                    SELECT xString1 FROM MERI_StockLevelling WHERE Branch = '{stringBranch}' AND Prefix = 'R'
                   ", conn);
            adapter1.Fill(table5);
            conn.Close();
            rngsArray = table5.Rows.OfType<DataRow>().Select(k => k[0].ToString()).ToArray();


            branchIndex = 0;
            foreach (object branch in checkedListBox1.CheckedItems)
            {
                stringBranch = table.Rows[branchIndex][0].ToString();

                PGphrase = String.Join(",", pgsArray);
                rangePhrase = String.Join(",", rngsArray);

                pgs = "U=STOCKLEV@1!~" + PGphrase + $@"~~~~~~~~~2!{DC}!{stringBranch}!";
                rngs = "U=STOCKLEV@1!~~" + rangePhrase + $@"~~~~~~~~2!{DC}!{stringBranch}!";



                using (StreamReader sr = new StreamReader(@"\\svrsql1\AUTOPART\VUGDocs\StockLev.vtm"))
                {

                    bool tf = false;

                    while ((line = sr.ReadLine()) != null)
                    {

                        if (line.EndsWith($@"~~~~~~~~~2!{DC}!{stringBranch}!"))
                        {
                            Lines += pgs + "\r\n";
                            tf = true;
                        }

                        if (line.EndsWith($@"~~~~~~~~2!{DC}!{stringBranch}!") && tf != true)
                        {
                            Lines += rngs + "\r\n";
                            tf = true;
                        }

                        if (tf == false)
                        {
                            Lines += line + "\r\n";
                        }

                        tf = false;

                    }
                    sr.Close();
                    using (StreamWriter sw = new StreamWriter(@"\\svrsql1\AUTOPART\VUGDocs\StockLev.vtm"))
                    {
                        sw.WriteLine(Lines);
                        sw.Close();
                    }
                    Lines = "";
                    branchIndex++;
                    //PGphrase = "";
                    //rangePhrase = "";
                    //pgs = "";
                    //rngs = "";
                    //Lines = "";
                }
                //if (File.Exists(@"\\svrsql1\AUTOPART\VUGDocs\StockLev_Test.vtm"))
                //{
                //    File.Delete(@"\\svrsql1\AUTOPART\VUGDocs\StockLev_Test.vtm");
                //}
            }

            //bool exists = Directory.Exists($@"\\svrsql1\AUTOPART\VUGDocs\StockLev_Test.vtm");

            //if (!exists) Directory.CreateDirectory($@"\\svrsql1\AUTOPART\VUGDocs\StockLev_Test.vtm");





            Cursor.Current = Cursors.Default;
            MessageBox.Show("Done!");


        }

        private void closeButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void checkedListBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            conn.Close();
            table2.Clear();
            table3.Clear();
            dtId.Clear();
            dtId1.Clear();
            this.checkedListBox2.DataSource = null;
            this.checkedListBox3.DataSource = null;
            branches = "";

            foreach (object item in checkedListBox1.CheckedItems)
            {
                int branch = checkedListBox1.Items.IndexOf(item);
                if (checkedListBox1.CheckedItems.Count > 1)
                {
                    branches = branches + "','" + table.Rows[branch][0].ToString();
                }
                else
                {
                    branches = table.Rows[branch][0].ToString();
                }
            }


            conn.Open();
            DataSet ds2 = new DataSet();
            SqlDataAdapter adapter2 = new SqlDataAdapter(
            $@"SELECT DISTINCT xString1 FROM MERI_StockLevelling WHERE Prefix = 'P' AND Branch IN('{branches}')", conn);
            adapter2.Fill(ds2);
            adapter2.Fill(table2);
            this.checkedListBox2.DataSource = ds2.Tables[0];
            this.checkedListBox2.DisplayMember = "xString1";
            int lastIndex = checkedListBox2.Items.Count - 1;
            for (int i = lastIndex; i >= 0; i--)
            {
                checkedListBox2.SetItemChecked(i, true);
            }
            conn.Close();

            conn.Open();
            DataSet ds3 = new DataSet();
            SqlDataAdapter adapter3 = new SqlDataAdapter(
            $@"SELECT DISTINCT xString1 FROM MERI_StockLevelling WHERE Prefix = 'R' AND Branch IN('{branches}')", conn);
            adapter3.Fill(ds3);
            adapter3.Fill(table3);
            this.checkedListBox3.DataSource = ds3.Tables[0];
            this.checkedListBox3.DisplayMember = "xString1";
            int lastIndex1 = checkedListBox3.Items.Count - 1;
            for (int i = lastIndex1; i >= 0; i--)
            {
                checkedListBox3.SetItemChecked(i, true);
            }
            conn.Close();

        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            table2.Clear();
            table3.Clear();

            branches = "";
            if (allBranchesCheckBox.Checked == true)
            {
                for (int i = 0; i < checkedListBox1.Items.Count; i++)
                {
                    checkedListBox1.SetItemChecked(i, true);

                }
            }
            else
            {
                for (int i = 0; i < checkedListBox1.Items.Count; i++)
                {
                    checkedListBox1.SetItemChecked(i, false);

                }
            }

            conn.Close();
            table2.Clear();
            table3.Clear();
            dtId.Clear();
            dtId1.Clear();

            foreach (object item in checkedListBox1.CheckedItems)
            {
                int branch = checkedListBox1.Items.IndexOf(item);
                if (checkedListBox1.CheckedItems.Count > 1)
                {
                    branches = branches + "','" + table.Rows[branch][0].ToString();
                }
                else
                {
                    branches = table.Rows[branch][0].ToString();
                }
            }


            conn.Open();
            DataSet ds2 = new DataSet();
            SqlDataAdapter adapter2 = new SqlDataAdapter(
            $@"SELECT DISTINCT xString1 FROM MERI_StockLevelling WHERE Prefix = 'P' AND Branch IN('{branches}')", conn);
            adapter2.Fill(ds2);
            adapter2.Fill(table2);
            this.checkedListBox2.DataSource = ds2.Tables[0];
            this.checkedListBox2.DisplayMember = "xString1";
            int lastIndex = checkedListBox2.Items.Count - 1;
            for (int i = lastIndex; i >= 0; i--)
            {
                checkedListBox2.SetItemChecked(i, true);
            }
            conn.Close();

            conn.Open();
            DataSet ds3 = new DataSet();
            SqlDataAdapter adapter3 = new SqlDataAdapter(
            $@"SELECT DISTINCT xString1 FROM MERI_StockLevelling WHERE Prefix = 'R' AND Branch IN('{branches}')", conn);
            adapter3.Fill(ds3);
            adapter3.Fill(table3);
            this.checkedListBox3.DataSource = ds3.Tables[0];
            this.checkedListBox3.DisplayMember = "xString1";
            int lastIndex1 = checkedListBox3.Items.Count - 1;
            for (int i = lastIndex1; i >= 0; i--)
            {
                checkedListBox3.SetItemChecked(i, true);
            }
            conn.Close();
        }

        private void addPG_Button_Click(object sender, EventArgs e)
        {
            pgTextBox.Text = pgTextBox.Text.ToUpper();

            if (conn.State == ConnectionState.Open)
            {
                conn.Close();
            }

            conn.Open();
            string query = ($@"
                                SELECT PG FROM Product WHERE PG = '{pgTextBox.Text}'
                            ");
            SqlCommand cmd = new SqlCommand(query, conn);
            string result = (string)cmd.ExecuteScalar();

            if (result == null)
            {
                MessageBox.Show($"The Product Group {pgTextBox.Text} can not be found", "Notice", MessageBoxButtons.OK);
                return;
            }
            else
            {
                table2.Rows.Add($@"{pgTextBox.Text}");
                this.checkedListBox2.DataSource = table2;
                this.checkedListBox2.DisplayMember = "xString1";
                int lastIndex = checkedListBox2.Items.Count - 1;
                for (int i = lastIndex; i >= 0; i--)
                {
                    checkedListBox2.SetItemChecked(i, true);
                }
            }
            pgTextBox.Text = "";
            pgTextBox.Focus();

        }

        private void addRange_Button_Click(object sender, EventArgs e)
        {
            rangeTextBox.Text = rangeTextBox.Text.ToUpper();

            if (conn.State == ConnectionState.Open)
            {
                conn.Close();
            }

            conn.Open();
            string query = ($@"
                                SELECT Range FROM Product WHERE Range = '{rangeTextBox.Text}'
                            ");
            SqlCommand cmd = new SqlCommand(query, conn);
            string result = (string)cmd.ExecuteScalar();

            if (result == null)
            {
                MessageBox.Show($"The Product Group {rangeTextBox.Text} can not be found", "Notice", MessageBoxButtons.OK);
                return;
            }
            else
            {
                table3.Rows.Add($@"{rangeTextBox.Text}");
                this.checkedListBox3.DataSource = table3;
                this.checkedListBox3.DisplayMember = "xString1";
                int lastIndex = checkedListBox3.Items.Count - 1;
                for (int i = lastIndex; i >= 0; i--)
                {
                    checkedListBox3.SetItemChecked(i, true);
                }
            }

            rangeTextBox.Text = "";
            rangeTextBox.Focus();
        }

        private void removeButton_Click(object sender, EventArgs e)
        {
            dr = null;
            index = 0;

            DialogResult dialogResult = MessageBox.Show("Do you want to remove all of the currently selected product groups and ranges?", "Notice", MessageBoxButtons.YesNo);
            if (dialogResult == DialogResult.Yes)
            {
                foreach (object branch in checkedListBox1.CheckedItems)
                {
                    foreach (object pg in checkedListBox2.CheckedItems)
                    {
                        int pgIndex = checkedListBox2.Items.IndexOf(pg);
                        stringPG = table2.Rows[pgIndex][0].ToString();
                        int branchIndex = checkedListBox1.Items.IndexOf(branch);
                        stringBranch = table.Rows[branchIndex][0].ToString();
                        
                                conn.Open();
                                string query2 = ($@"
                                DELETE FROM MERI_StockLevelling WHERE Prefix = 'P' AND Branch = '{stringBranch}' AND xString1 = '{stringPG}'
                            ");
                                SqlCommand cmd2 = new SqlCommand(query2, conn);
                                cmd2.ExecuteScalar();
                                conn.Close();
                    }
                }

                foreach (object branch in checkedListBox1.CheckedItems)
                {
                    foreach (object range in checkedListBox3.CheckedItems)
                    {
                        int rangeIndex = checkedListBox3.Items.IndexOf(range);
                        stringRange = table3.Rows[rangeIndex][0].ToString();
                        int branchIndex = checkedListBox1.Items.IndexOf(branch);
                        stringBranch = table.Rows[branchIndex][0].ToString();

                        if (conn.State == ConnectionState.Open)
                        {
                            conn.Close();
                        }

                        conn.Open();
                        string query3 = ($@"
                                DELETE FROM MERI_StockLevelling WHERE Prefix = 'R' AND Branch = '{stringBranch}' AND xString1 = '{stringRange}'
                            ");
                        SqlCommand cmd3 = new SqlCommand(query3, conn);
                        cmd3.ExecuteScalar();
                        conn.Close();
                    }
                }

                int lastIndex = checkedListBox2.Items.Count - 1;
                for (int i = lastIndex; i >= 0; i--)
                {
                    if (checkedListBox2.GetItemCheckState(i) == CheckState.Checked)
                    {
                        dr = table2.Rows[i];
                        dr.Delete();
                    }
                }
                table2.AcceptChanges();
                this.checkedListBox2.DataSource = table2;
                this.checkedListBox2.DisplayMember = "xString1";

                lastIndex = checkedListBox3.Items.Count - 1;
                for (int i = lastIndex; i >= 0; i--)
                {
                    if (checkedListBox3.GetItemCheckState(i) == CheckState.Checked)
                    {
                        dr = table3.Rows[i];
                        dr.Delete();
                    }
                }
                table3.AcceptChanges();
                this.checkedListBox3.DataSource = table3;
                this.checkedListBox3.DisplayMember = "xString1";

            }
            else if (dialogResult == DialogResult.No)
            {
                return;
            }
        }

        private void pgTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                addPG_Button_Click((object)sender, (EventArgs)e);
            }
        }

        private void rangeTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                addRange_Button_Click((object)sender, (EventArgs)e);
            }
        }
    }
}
