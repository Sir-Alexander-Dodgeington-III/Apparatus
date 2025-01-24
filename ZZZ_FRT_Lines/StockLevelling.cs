using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace StockLevelling
{
    public partial class StockLevelling : Form
    {
        public DataTable table = new DataTable();
        public StockLevelling()
        {
            InitializeComponent();
            this.CenterToScreen();

            SqlConnection conn = new SqlConnection(Helper.ConnString("AUTOPART"));
            {
                SqlCommand sqlCmd = new SqlCommand("SELECT DISTINCT Branch FROM meri_Stocklevelling Order By Branch ASC", conn);
                conn.Open();
                SqlDataReader sqlReader = sqlCmd.ExecuteReader();

                while (sqlReader.Read())
                {
                    branchComboBox.Items.Add(sqlReader["Branch"].ToString());
                }

                sqlReader.Close();
            }
            conn.Close();
            this.branchComboBox.SelectedIndexChanged +=
            new System.EventHandler(branchComboBox_SelectedIndexChanged);
        }

        private void branchComboBox_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            branchComboBox.DataSource = null;
            PG_ListBox_Available.DataSource = null;
            Range_ListBox_Available.DataSource = null;
            rangeListBox_Current.DataSource = null;
            PG_ListBox_Current.DataSource = null;
            PG_ListBox_Current.Items.Clear();
            rangeListBox_Current.Items.Clear();
            PG_ListBox_Available.Items.Clear();
            Range_ListBox_Available.Items.Clear();


            SqlConnection conn = new SqlConnection(Helper.ConnString("AUTOPART"));
            conn.Open();
            string query = ($@"
                                SELECT BranchGroup FROM Branches WHERE Branch = '{branchComboBox.SelectedItem}'
                            ");
            SqlCommand cmd = new SqlCommand(query, conn);
            string DC = (string)cmd.ExecuteScalar();
            conn.Close();

            if (DC == "")
            {
                MessageBox.Show("The selected branch does not have a branch group. Please notify IT@arnoldgroupweb.com so that this may be added", "Notice");
                return;
            }

            string[] lines = File.ReadAllLines(@"\\svrsql1\AUTOPART\VUGDocs\StockLev.vtm");
            //List<string> lines = new List<string>();
            if (File.Exists(@"\\svrsql1\AUTOPART\VUGDocs\StockLev.vtm"))
            {
                // Read the file and display it line by line.
                IEnumerable<string> selectLines = lines.Where(line => line.EndsWith($@"~~~~~~~~~2!{DC}!{branchComboBox.SelectedItem}!"));
                IEnumerable<string> selectLines2 = lines.Where(line => line.EndsWith($@"~~~~~~~~2!{DC}!{branchComboBox.SelectedItem}!"));

                foreach (var item in selectLines)
                {
                    splitTextBox.Text = item;
                    splitTextBox.Text = splitTextBox.Text.Replace("U=STOCKLEV@1!~", "");
                    splitTextBox.Text = splitTextBox.Text.Replace($@"~~~~~~~~~2!{DC}!{branchComboBox.SelectedItem}!", "");
                }

                foreach (var item in selectLines2)
                {
                    splitTextBox2.Text = item;
                    splitTextBox2.Text = splitTextBox2.Text.Replace("U=STOCKLEV@1!~~", "");
                    splitTextBox2.Text = splitTextBox2.Text.Replace($@"~~~~~~~~2!{DC}!{branchComboBox.SelectedItem}!", "");
                }

                string PGphrase = splitTextBox.Text;
                string rangePhrase = splitTextBox2.Text;

                string[] pgs = PGphrase.Split(',');
                string[] rngs = rangePhrase.Split(',');

                Array.Sort(pgs, (x, y) => String.Compare(x, y));
                Array.Sort(rngs, (x, y) => String.Compare(x, y));


                if (branchComboBox.SelectedItem.ToString() != "Deflt")
                {
                    foreach (string p in pgs)
                    {
                        PG_ListBox_Current.Items.Add(p);
                    }
                    foreach (string r in rngs)
                    {
                        rangeListBox_Current.Items.Add(r);
                    }
                }
                else
                {
                    conn.Open();
                    DataSet ds0 = new DataSet();
                    SqlDataAdapter adapter0 = new SqlDataAdapter(
                    $"SELECT DISTINCT xstring1 FROM MERI_stocklevelling WHERE branch = 'Deflt' AND Prefix = 'P'", conn);
                    adapter0.Fill(ds0);
                    this.PG_ListBox_Current.DataSource = ds0.Tables[0];
                    this.PG_ListBox_Current.DisplayMember = "xstring1";
                    conn.Close();


                    conn.Open();
                    DataSet ds2 = new DataSet();
                    SqlDataAdapter adapter2 = new SqlDataAdapter(
                    $"SELECT DISTINCT xstring1 FROM MERI_stocklevelling WHERE branch = 'Deflt' AND Prefix = 'R'", conn);
                    adapter2.Fill(ds2);
                    this.rangeListBox_Current.DataSource = ds2.Tables[0];
                    this.rangeListBox_Current.DisplayMember = "xstring1";
                    conn.Close();
                }

                string pgAvailable = splitTextBox.Text.Replace(",", "','");
                string rangeAvailable = splitTextBox2.Text.Replace(",", "','");

                //SqlConnection conn = new SqlConnection(Helper.ConnString("AUTOPART"));
                conn.Open();
                DataSet ds = new DataSet();
                SqlDataAdapter adapter = new SqlDataAdapter(
                $"SELECT DISTINCT PG FROM vw_MERI_ProductRanges WHERE PG NOT IN('{pgAvailable}') AND PG > '99'", conn);
                adapter.Fill(ds);
                this.PG_ListBox_Available.DataSource = ds.Tables[0];
                this.PG_ListBox_Available.DisplayMember = "pg";
                conn.Close();


                //SqlConnection conn = new SqlConnection("Data Source=DBELL;Initial Catalog=part_table;Integrated Security=True");
                conn.Open();
                DataSet ds1 = new DataSet();
                SqlDataAdapter adapter1 = new SqlDataAdapter(
                $"SELECT DISTINCT Range FROM vw_MERI_ProductRanges WHERE Range NOT IN('{rangeAvailable}') AND PG > '99'", conn);
                adapter1.Fill(ds1);
                this.Range_ListBox_Available.DataSource = ds1.Tables[0];
                this.Range_ListBox_Available.DisplayMember = "Range";
                conn.Close();

                PG_ListBox_Available.SelectedItem = null;
                Range_ListBox_Available.SelectedItem = null;
            }
        }


        private void addPG_Button_Click(object sender, EventArgs e)
        {
            string selected = PG_ListBox_Available.GetItemText(PG_ListBox_Available.SelectedItem);

            if (splitTextBox2.Text.Contains(selected))
            {
                MessageBox.Show($@"The product group {selected} is already in Current Ranges. Please remove all ranges in product group {selected} before adding this product group to Current PG's", "NOTICE");
                return;
            }

            string pgCurrent = splitTextBox.Text += "," + PG_ListBox_Available.GetItemText(PG_ListBox_Available.SelectedItem);

            PG_ListBox_Available.DataSource = null;
            PG_ListBox_Available.Items.Clear();
            PG_ListBox_Current.Items.Clear();
            PG_ListBox_Available.Update();
            PG_ListBox_Available.Refresh();

            string PGphrase = splitTextBox.Text;
            string pgCurrent1 = pgCurrent.Replace(",", "','");
            string[] pgs = PGphrase.Split(',');
            Array.Sort(pgs, (x, y) => String.Compare(x, y));

            foreach (string p in pgs)
            {
                PG_ListBox_Current.Items.Add(p);
            }


            SqlConnection conn = new SqlConnection(Helper.ConnString("AUTOPART"));
            conn.Open();
            DataSet ds = new DataSet();
            SqlDataAdapter adapter = new SqlDataAdapter(
            $"SELECT DISTINCT PG FROM vw_MERI_ProductRanges WHERE PG NOT IN('{pgCurrent1}') AND PG > '99'", conn);
            adapter.Fill(ds);
            this.PG_ListBox_Available.DataSource = ds.Tables[0];
            this.PG_ListBox_Available.DisplayMember = "pg";
            conn.Close();


            PG_ListBox_Available.SelectedItem = null;
            Range_ListBox_Available.SelectedItem = null;
        }

        private void addRangeButton_Click(object sender, EventArgs e)
        {
            string selected = Range_ListBox_Available.GetItemText(Range_ListBox_Available.SelectedItem);

            if (splitTextBox.Text.Contains(selected.Substring(0,3)))
            {
                MessageBox.Show($@"The entire product group for range {selected} is already in Current PG's. Please remove product group {selected.Substring(0,3)} before adding this range to Current Ranges", "NOTICE");
                return;
            }

            string rangeCurrent = splitTextBox2.Text += "," + Range_ListBox_Available.GetItemText(Range_ListBox_Available.SelectedItem);

            Range_ListBox_Available.DataSource = null;
            Range_ListBox_Available.Items.Clear();
            rangeListBox_Current.Items.Clear();
            Range_ListBox_Available.Update();
            Range_ListBox_Available.Refresh();

            string rngphrase = splitTextBox2.Text;
            string rngCurrent1 = rangeCurrent.Replace(",", "','");
            string[] rngs = rngphrase.Split(',');
            Array.Sort(rngs, (x, y) => String.Compare(x,y));

            foreach (string p in rngs)
            {
                rangeListBox_Current.Items.Add(p);
            }


            SqlConnection conn = new SqlConnection(Helper.ConnString("AUTOPART"));
            conn.Open();
            DataSet ds = new DataSet();
            SqlDataAdapter adapter = new SqlDataAdapter(
            $"SELECT DISTINCT Range FROM vw_MERI_ProductRanges WHERE Range NOT IN('{rngCurrent1}') AND PG > '99'", conn);
            adapter.Fill(ds);
            this.Range_ListBox_Available.DataSource = ds.Tables[0];
            this.Range_ListBox_Available.DisplayMember = "Range";
            conn.Close();



            PG_ListBox_Available.SelectedItem = null;
            Range_ListBox_Available.SelectedItem = null;
        }

        private void removePG_Button_Click(object sender, EventArgs e)
        {
            string selected = PG_ListBox_Current.GetItemText(PG_ListBox_Current.SelectedItem);
            if (splitTextBox.Text.Contains("," + selected))
            {
                string pgCurrent = splitTextBox.Text.Replace($@"{"," + selected}", "");

                PG_ListBox_Available.DataSource = null;
                PG_ListBox_Available.Items.Clear();
                PG_ListBox_Current.Items.Clear();
                PG_ListBox_Available.Update();
                PG_ListBox_Available.Refresh();

                string PGphrase = pgCurrent;
                string pgCurrent1 = pgCurrent.Replace(",", "','");
                string[] pgs = PGphrase.Split(',');

                foreach (string p in pgs)
                {
                    PG_ListBox_Current.Items.Add(p);
                }


                SqlConnection conn = new SqlConnection(Helper.ConnString("AUTOPART"));
                conn.Open();
                DataSet ds = new DataSet();
                SqlDataAdapter adapter = new SqlDataAdapter(
                $"SELECT DISTINCT PG FROM vw_MERI_ProductRanges WHERE PG NOT IN('{pgCurrent1}') AND PG > '99'", conn);
                adapter.Fill(ds);
                this.PG_ListBox_Available.DataSource = ds.Tables[0];
                this.PG_ListBox_Available.DisplayMember = "pg";
                conn.Close();

                splitTextBox.Text = pgCurrent;

                PG_ListBox_Available.SelectedItem = null;
                Range_ListBox_Available.SelectedItem = null;
            }
            else
            {
                string pgCurrent = splitTextBox.Text.Replace($@"{selected}" + ",", "");

                PG_ListBox_Available.DataSource = null;
                PG_ListBox_Available.Items.Clear();
                PG_ListBox_Current.Items.Clear();
                PG_ListBox_Available.Update();
                PG_ListBox_Available.Refresh();

                string PGphrase = pgCurrent;
                string pgCurrent1 = pgCurrent.Replace(",", "','");
                string[] pgs = PGphrase.Split(',');

                foreach (string p in pgs)
                {
                    PG_ListBox_Current.Items.Add(p);
                }


                SqlConnection conn = new SqlConnection(Helper.ConnString("AUTOPART"));
                conn.Open();
                DataSet ds = new DataSet();
                SqlDataAdapter adapter = new SqlDataAdapter(
                $"SELECT DISTINCT PG FROM vw_MERI_ProductRanges WHERE PG NOT IN('{pgCurrent1}') AND PG > '99'", conn);
                adapter.Fill(ds);
                this.PG_ListBox_Available.DataSource = ds.Tables[0];
                this.PG_ListBox_Available.DisplayMember = "pg";
                conn.Close();

                splitTextBox.Text = pgCurrent;

                PG_ListBox_Available.SelectedItem = null;
                Range_ListBox_Available.SelectedItem = null;
            }


        }

        private void removeRangeButton_Click(object sender, EventArgs e)
        {
            string selected = rangeListBox_Current.GetItemText(rangeListBox_Current.SelectedItem);
            if (splitTextBox2.Text.Contains("," + selected))
            {
                string rngCurrent = splitTextBox2.Text.Replace($@",{selected}", "");

                Range_ListBox_Available.DataSource = null;
                Range_ListBox_Available.Items.Clear();
                rangeListBox_Current.Items.Clear();
                Range_ListBox_Available.Update();
                Range_ListBox_Available.Refresh();

                string RNGphrase = rngCurrent;
                string rngCurrent1 = rngCurrent.Replace(",", "','");
                string[] rngs = RNGphrase.Split(',');

                foreach (string p in rngs)
                {
                    rangeListBox_Current.Items.Add(p);
                }


                SqlConnection conn = new SqlConnection(Helper.ConnString("AUTOPART"));
                conn.Open();
                DataSet ds = new DataSet();
                SqlDataAdapter adapter = new SqlDataAdapter(
                $"SELECT DISTINCT Range FROM vw_MERI_ProductRanges WHERE Range NOT IN('{rngCurrent1}') AND PG > '99'", conn);
                adapter.Fill(ds);
                this.Range_ListBox_Available.DataSource = ds.Tables[0];
                this.Range_ListBox_Available.DisplayMember = "Range";
                conn.Close();

                splitTextBox2.Text = rngCurrent;

                PG_ListBox_Available.SelectedItem = null;
                Range_ListBox_Available.SelectedItem = null;
            }
            else
            {
                string rngCurrent = splitTextBox2.Text.Replace($@"{selected}" + ",", "");

                Range_ListBox_Available.DataSource = null;
                Range_ListBox_Available.Items.Clear();
                rangeListBox_Current.Items.Clear();
                Range_ListBox_Available.Update();
                Range_ListBox_Available.Refresh();

                string RNGphrase = rngCurrent;
                string rngCurrent1 = rngCurrent.Replace(",", "','");
                string[] rngs = RNGphrase.Split(',');

                foreach (string p in rngs)
                {
                    rangeListBox_Current.Items.Add(p);
                }


                SqlConnection conn = new SqlConnection(Helper.ConnString("AUTOPART"));
                conn.Open();
                DataSet ds = new DataSet();
                SqlDataAdapter adapter = new SqlDataAdapter(
                $"SELECT DISTINCT Range FROM vw_MERI_ProductRanges WHERE Range NOT IN('{rngCurrent1}') AND PG > '99'", conn);
                adapter.Fill(ds);
                this.Range_ListBox_Available.DataSource = ds.Tables[0];
                this.Range_ListBox_Available.DisplayMember = "Range";
                conn.Close();

                splitTextBox2.Text = rngCurrent;

                PG_ListBox_Available.SelectedItem = null;
                Range_ListBox_Available.SelectedItem = null;
            }
        }



        private void quitButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }


        private void PG_ListBox_Available_Click(object sender, EventArgs e)
        {
            Range_ListBox_Available.SelectedItem = null;
            rangeListBox_Current.SelectedItem = null;
            PG_ListBox_Current.SelectedItem = null;
        }

        private void PG_ListBox_Current_Click(object sender, EventArgs e)
        {
            PG_ListBox_Available.SelectedItem = null;
            Range_ListBox_Available.SelectedItem = null;
            rangeListBox_Current.SelectedItem = null;
        }

        private void Range_ListBox_Available_Click(object sender, EventArgs e)
        {
            PG_ListBox_Available.SelectedItem = null;
            rangeListBox_Current.SelectedItem = null;
            PG_ListBox_Current.SelectedItem = null;
        }

        private void rangeListBox_Current_Click(object sender, EventArgs e)
        {
            PG_ListBox_Current.SelectedItem = null;
            Range_ListBox_Available.SelectedItem = null;
            PG_ListBox_Available.SelectedItem = null;
        }

        private void updateButton_Click(object sender, EventArgs e)
        {
            SqlConnection conn = new SqlConnection(Helper.ConnString("AUTOPART"));
            conn.Open();
            string query = ($@"
                                SELECT BranchGroup FROM Branches WHERE Branch = '{branchComboBox.SelectedItem}'
                            ");
            SqlCommand cmd = new SqlCommand(query, conn);
            string DC = (string)cmd.ExecuteScalar();
            conn.Close();

            string PGphrase = splitTextBox.Text;
            string rangePhrase = splitTextBox2.Text;

            string[] pgsArray = PGphrase.Split(',');
            string[] rngsArray = rangePhrase.Split(',');



            conn.Open();
            string query0 = ($@"
                                Delete FROM MERI_Stocklevelling WHERE Branch = '{branchComboBox.SelectedItem}'
                            ");
            SqlCommand cmd0 = new SqlCommand(query0, conn);
            cmd0.ExecuteNonQuery();
            conn.Close();

            foreach (string p in pgsArray)
            {
                conn.Open();
                string query1 = ($@"
                            insert into meri_stocklevelling VALUES('P','{branchComboBox.SelectedItem}','{p}')
                        ");
                SqlCommand cmd1 = new SqlCommand(query1, conn);
                cmd1.ExecuteNonQuery();
                conn.Close();

            }

            foreach (string r in rngsArray)
            {
                conn.Open();
                string query2 = ($@"
                            insert into meri_stocklevelling VALUES('R','{branchComboBox.SelectedItem}','{r}') 
                        ");
                SqlCommand cmd2 = new SqlCommand(query2, conn);
                cmd2.ExecuteNonQuery();
                conn.Close();

            }




            string pgs = "U=STOCKLEV@1!~" + splitTextBox.Text + $@"~~~~~~~~~2!{DC}!{branchComboBox.SelectedItem}!";
            string rngs = "U=STOCKLEV@1!~~" + splitTextBox2.Text + $@"~~~~~~~~2!{DC}!{branchComboBox.SelectedItem}!";



            using (StreamReader sr = new StreamReader(@"\\svrsql1\AUTOPART\VUGDocs\StockLev.vtm"))
            {
                string line;
                string Lines = "";
                bool tf = false;

                while ((line = sr.ReadLine()) != null)
                {

                    if (line.EndsWith($@"~~~~~~~~~2!{DC}!{branchComboBox.SelectedItem}!"))
                    {
                        Lines += pgs + "\r\n";
                        tf = true;
                    }

                    if (line.EndsWith($@"~~~~~~~~2!{DC}!{branchComboBox.SelectedItem}!") && tf != true)
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


                if (File.Exists(@"\\svrsql1\AUTOPART\VUGDocs\StockLev.vtm"))
                {
                    File.Delete(@"\\svrsql1\AUTOPART\VUGDocs\StockLev.vtm");
                }

                using (StreamWriter sw = new StreamWriter(@"\\svrsql1\AUTOPART\VUGDocs\StockLev.vtm"))
                {
                    sw.WriteLine(Lines);
                    sw.Close();
                }

                MessageBox.Show("Done!");

            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            new ZZZ_FRT_Lines.stocklevellingForm().Show();
        }
    }
}
