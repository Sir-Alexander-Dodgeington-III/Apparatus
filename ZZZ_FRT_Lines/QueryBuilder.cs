using System;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace QueryBuilder
{
    public partial class QueryBuilder : Form
    {
        public bool typed = true;
        public QueryBuilder()
        {
            InitializeComponent();
            this.CenterToScreen();
            string username = System.Security.Principal.WindowsIdentity.GetCurrent().Name;
            if (username.Contains("jrusk") || username.Contains("kguerttman") || username.Contains("AKOHRS") || username.Contains("bmiller") || username.Contains("abergstrom"))
            {
                FromComboBox.Text = "VW_MERI_Pro";
            }
            if (username.Contains("nsimington") || username.Contains("sschoning") || username.Contains("astethem"))
            {
                FromComboBox.Text = "VW_MERI_Pri";
            }

            FromComboBox.Items.Add("VW_MERI_I");
            FromComboBox.Items.Add("VW_MERI_IOT");
            FromComboBox.Items.Add("VW_MERI_PartSlope");
            FromComboBox.Items.Add("VW_MERI_EndOfLife");
            FromComboBox.Items.Add("VW_MERI_15MonthUsage");
            FromComboBox.Items.Add("VW_MERI_ReplenishCheck");
            FromComboBox.Items.Add("MERI_ProductImport");
            FromComboBox.Items.Add("VW_MERI_Pro");
            FromComboBox.Items.Add("VW_MERI_Pri");
            FromComboBox.Items.Add("VW_MERI_OpenPO");
            FromComboBox.Items.Add("VW_MERI_ProList");
            FromComboBox.Items.Add("VW_MERI_PriList");
            FromComboBox.Items.Add("VW_MERI_Usages_By_Month_AND_Year");
            FromComboBox.Items.Add("VW_MERI_VendorsCostTab");
            FromComboBox.Items.Add("VW_MERI_Rec");
            FromComboBox.Items.Add("VW_MERI_Locations2");
            FromComboBox.Items.Add("MERI_ReclassHistory");
            FromComboBox.Items.Add("VW_MERI_PartListValidate");
            FromComboBox.Items.Add("MERI_ReviewHist");
        }

        private void QuitButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void GroupByCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (GroupByCheckBox.Checked)
            {
                GroupByTextBox.Visible = true;
                GroupByInputText.Visible = true;
                OrderByCheckBox.Visible = true;
            }

            if (!GroupByCheckBox.Checked)
            {
                GroupByTextBox.Visible = false;
                GroupByInputText.Visible = false;
                OrderByCheckBox.Visible = false;
            }
        }

        private void OrderByCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (OrderByCheckBox.Checked)
            {
                OrderByTextBox.Visible = true;
                OrderByInputTextBox.Visible = true;
            }

            if (!OrderByCheckBox.Checked)
            {
                OrderByTextBox.Visible = false;
                OrderByInputTextBox.Visible = false;
            }
        }

        private void WhereCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (WhereCheckBox.Checked)
            {
                WhereTextBox.Visible = true;
                WhereInputText.Visible = true;
                GroupByCheckBox.Visible = true;
            }

            if (!WhereCheckBox.Checked)
            {
                WhereTextBox.Visible = false;
                WhereInputText.Visible = false;
                GroupByCheckBox.Visible = false;
            }
        }

        private void SearchButton_Click(object sender, EventArgs e)
        {
            QueryString.Visible = true;

            QueryString.Text = SelectTextBox.Text + ' ' + SelectInputTextBox.Text + ' ' + FromTextBox.Text + ' ' + FromComboBox.Text;
            if (WhereCheckBox.Checked)
            {
                QueryString.Text += ' ' + WhereTextBox.Text + ' ' + WhereInputText.Text;
            }
            if (GroupByCheckBox.Checked)
            {
                QueryString.Text += ' ' + GroupByTextBox.Text + ' ' + GroupByInputText.Text;
            }
            if (OrderByCheckBox.Checked)
            {
                QueryString.Text += ' ' + OrderByTextBox.Text + ' ' + OrderByInputTextBox.Text;
            }
            //QueryResults.QueryResults qr = new QueryResults.QueryResults(QueryString.Text);
            //qr.Show();

            
            string Query = QueryString.Text;
            QueryResults.QueryResults qr = new QueryResults.QueryResults(Query);
            qr.Show();
        }

        private void saveButton_Click(object sender, EventArgs e)
        {
            string username1 = System.Security.Principal.WindowsIdentity.GetCurrent().Name;
            string user = username1.Substring(10, (username1.Length - 10));

            bool exists = Directory.Exists($@"C:\Users\{user}\Documents\Saved Queries");

            if (!exists) Directory.CreateDirectory($@"C:\Users\{user}\Documents\Saved Queries");

            try
            {
                Stream myStream;
                SaveFileDialog saveFileDialog1 = new SaveFileDialog();

                saveFileDialog1.Filter = "txt files (*.txt)|*.txt";
                saveFileDialog1.FilterIndex = 2;
                saveFileDialog1.RestoreDirectory = true;
                saveFileDialog1.Title = "Where to save query?";
                saveFileDialog1.InitialDirectory = $@"C:\Users\{user}\Documents\Saved Queries";

                if (saveFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    if ((myStream = saveFileDialog1.OpenFile()) != null)
                    {
                        myStream.Close();
                        StreamWriter writer = new StreamWriter(saveFileDialog1.OpenFile());
                        writer.WriteLine(SelectInputTextBox.Text.ToString());
                        writer.WriteLine(FromComboBox.Text.ToString());
                        writer.WriteLine(WhereInputText.Text.ToString());
                        writer.WriteLine(GroupByInputText.Text.ToString());
                        writer.WriteLine(OrderByInputTextBox.Text.ToString());
                        writer.Dispose();
                        writer.Close();
                        //myStream.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                return;
            }
        }

        private void recoverQuery_Click(object sender, EventArgs e)
        {

            string username1 = System.Security.Principal.WindowsIdentity.GetCurrent().Name;
            string user = username1.Substring(10, (username1.Length - 10));

            try
            {
                using (OpenFileDialog openFileDialog = new OpenFileDialog())
                {
                    openFileDialog.InitialDirectory = $@"C:\Users\{user}\Documents\Saved Queries";
                    openFileDialog.Filter = "txt files (*.txt)|*.txt";
                    openFileDialog.FilterIndex = 2;
                    openFileDialog.RestoreDirectory = true;
                    openFileDialog.Title = "Which query would you like to recover?";

                    if (openFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        //Get the path of specified file
                        string filePath = openFileDialog.FileName;

                        //Read the contents of the file into a stream
                        var fileStream = openFileDialog.OpenFile();

                    }
                    SelectInputTextBox.Text = File.ReadLines($"{openFileDialog.FileName}").First();
                    FromComboBox.Text = File.ReadLines($"{openFileDialog.FileName}").Skip(1).Take(1).First();
                    WhereInputText.Text = File.ReadLines($"{openFileDialog.FileName}").Skip(2).Take(1).First();
                    if (WhereInputText.Text != "") WhereCheckBox.Checked = true;
                    GroupByInputText.Text = File.ReadLines($"{openFileDialog.FileName}").Skip(3).Take(1).First();
                    if (GroupByInputText.Text != "") GroupByCheckBox.Checked = true;
                    OrderByInputTextBox.Text = File.ReadLines($"{openFileDialog.FileName}").Skip(4).Take(1).First();
                    if (OrderByInputTextBox.Text != "") OrderByCheckBox.Checked = true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void FromComboBox_TextChanged(object sender, EventArgs e)
        {
            ColumnNamesComboBox.Enabled = true;
            ColumnNamesComboBox.Items.Clear();
            if (typed == false)
            {
                SelectInputTextBox.Text = "";
            }
            ColumnNamesComboBox.Items.Add("");

            SqlConnection conn = new SqlConnection(Helper.ConnString("AUTOPART"));
            conn.Open();

                string query =
                $@"
                    SELECT COLUMN_NAME

                    FROM INFORMATION_SCHEMA.COLUMNS

                    WHERE TABLE_NAME = '{FromComboBox.Text}'

                    ORDER BY ORDINAL_POSITION
                ";

                SqlCommand cmd = new SqlCommand(query, conn);
                SqlDataReader sqlReader = cmd.ExecuteReader();

                while (sqlReader.Read())
                {
                    ColumnNamesComboBox.Items.Add(sqlReader["COLUMN_NAME"].ToString());

                }


                conn.Close();
                sqlReader.Close();
        }


        private void ColumnNamesComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            typed = false;

            if (ColumnNamesComboBox.SelectedIndex > -1)
            {
                if (SelectInputTextBox.Text != "")
                {
                    SelectInputTextBox.Text += ", " + ColumnNamesComboBox.SelectedItem.ToString();
                }
                else
                {
                    SelectInputTextBox.Text = ColumnNamesComboBox.SelectedItem.ToString();
                }
            }
        }
    }
}

