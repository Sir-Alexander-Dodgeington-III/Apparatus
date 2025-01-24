using System;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Globalization;


namespace SetStock

{
    public partial class SetStock : Form
    {
        DataTable dt = new DataTable();
        public string[] lines;
        public SetStock()
        {
            InitializeComponent();
            this.CenterToScreen();
        }


        // Select file
        public void button1_Click(object sender, EventArgs e)
        {

            OpenFileDialog OFD1 = new OpenFileDialog();
            OFD1.Filter = "csv files (*.csv)|*.csv";
            OFD1.ShowDialog();
            string filePath = OFD1.FileName;
            pathTextBox.Text = filePath;
            ReadCSV(filePath);
        }
 

        private void ReadCSV(string filePath)
        {
            int i = 1;
            try
            {
                lines = File.ReadAllLines(filePath);
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.ToString());
                return;
            }

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
                for(int r = 1; r < lines.Length; r++)
                {
                    string[] dataWords = lines[r].Split(',');
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

            // Add to table
            if (dt.Rows.Count > 0)
            {
                dataGridView1.DataSource = dt;
            }

            if (dataGridView1.Columns.Count != 9)
            {
                MessageBox.Show("The file provided does not have the required columns");
                return;
            }


        }

        private void closeButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void uploadButton_Click(object sender, EventArgs e)
        {
            SqlConnection conn = new SqlConnection(Helper.ConnString("AUTOPART"));
            int i = 0;
            DateTime date;
            int rows = dataGridView1.Rows.Count;
            string[] mnIndicators = { "M", "N", "S", "X", "Q", "R", "" };
            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                if (i == rows - 1)
                {
                    break;
                }
                using (SqlCommand cmd = new SqlCommand("sp_MERI_SetStock", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    //set branch param
                    cmd.Parameters.Add("@Branch", SqlDbType.VarChar).Value = dataGridView1.Rows[i].Cells[1].Value.ToString();
                    string branch = dataGridView1.Rows[i].Cells[1].Value.ToString();

                    //set part param
                    cmd.Parameters.Add("@Part", SqlDbType.VarChar).Value = dataGridView1.Rows[i].Cells[2].Value.ToString();
                    string part = dataGridView1.Rows[i].Cells[2].Value.ToString();

                    //set MIN param
                    cmd.Parameters.Add("@Min", SqlDbType.VarChar).Value = dataGridView1.Rows[i].Cells[3].Value.ToString();
                    string min1 = dataGridView1.Rows[i].Cells[3].Value.ToString();
                    int min = Int32.Parse(min1);

                    //set MAX param
                    cmd.Parameters.Add("@Max", SqlDbType.VarChar).Value = dataGridView1.Rows[i].Cells[4].Value.ToString();
                    string max1 = dataGridView1.Rows[i].Cells[4].Value.ToString();
                    int max = Int32.Parse(max1);

                    // set MNIndicator param
                    cmd.Parameters.Add("@MNIndicator", SqlDbType.VarChar).Value = dataGridView1.Rows[i].Cells[5].Value.ToString();
                    string mnindicator = dataGridView1.Rows[i].Cells[5].Value.ToString();

                    // Set MNDate param
                    cmd.Parameters.Add("@MNDate", SqlDbType.VarChar).Value = dataGridView1.Rows[i].Cells[6].Value.ToString();
                    string mndate = dataGridView1.Rows[i].Cells[6].Value.ToString();

                    // Set MNReason param
                    cmd.Parameters.Add("@MNReason", SqlDbType.VarChar).Value = dataGridView1.Rows[i].Cells[6].Value.ToString();
                    string mnReason = dataGridView1.Rows[i].Cells[7].Value.ToString();


                    cmd.Parameters.Add("@MinMaxInits", SqlDbType.VarChar).Value = dataGridView1.Rows[i].Cells[7].Value.ToString();
                    string inits = dataGridView1.Rows[i].Cells[8].Value.ToString();


                    // Check to make sure MAX is greater than MIN
                    if (min > max)
                    {
                        MessageBox.Show($"The MIN can not be greater than the MAX for part number {part} for branch {branch}");
                        return;
                    }

                    // make sure provided MNIndicator is in list of accepted MN Indicators
                    if (mnIndicators.Contains(mnindicator) == false)
                    {
                        MessageBox.Show($"Invalid condition indicator of {mnindicator} on part number {part} for branch {branch} (row number {i + 1})");
                        return;
                    }

                    // Make sure MNDate is in the right format
                    string[] format = { "yyyyMMdd" };
                    if (DateTime.TryParseExact(mndate, format, CultureInfo.InvariantCulture, DateTimeStyles.None, out date) || mndate == "")
                    {
                        if (mnindicator != "" && mndate == "")
                        {
                            MessageBox.Show($"MNDate can not be left empty if MNIndicator is not null. (see row number {i + 1})");
                            return;
                        }
                        if (mnindicator == "" && mndate != "")
                        {
                            MessageBox.Show($"MNIndicator can not be left empty if MNDate is not null. (see row number {i + 1})");
                            return;
                        }
                    }
                    else
                    {
                        MessageBox.Show($"Invalid MNDate of {mndate} on part number {part} for branch {branch} (row number {i + 1})");
                        return;
                    }

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
    }
}

