using System;
using System.Data;
using System.IO;
using System.Windows.Forms;


namespace Multi_PO
{
    public partial class Multi_PO : Form
    {
        public string filePath;
        public string[] lines;
        public string varBranch;
        public string varPart;
        public string varGroup;
        public string varQty;
        public string varWhse;
        public bool importManual = false;
        public int x = 0;

        DataTable dt = new DataTable();
        DataTable dt1 = new DataTable();
        public Multi_PO()
        {
            InitializeComponent();
            this.CenterToScreen();
        }

        private void defaultSupplierButton_Click(object sender, EventArgs e)
        {
            new defaultSupplier_MultiPO().Show();
        }

        private void quitButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void importFile_Click(object sender, EventArgs e)
        {
            importManual = false;
            OpenFileDialog OFD1 = new OpenFileDialog();
            OFD1.Filter = "csv files (*.csv)|*.csv";
            OFD1.ShowDialog();
            filePath = OFD1.FileName;
            //pathTextBox.Text = filePath;
            ReadCSV(filePath);
        }

        private void ReadCSV(string filePath)
        {
            int i = 1;
            try
            {
                lines = File.ReadAllLines(filePath);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Get headers
            if (lines.Length > 0)
            {
                string firstLine = lines[0];


                string[] headerLabels = firstLine.Split(',');

                //dt.Columns.Add("Row");

                foreach (string headerWord in headerLabels)
                {
                    dt.Columns.Add(new DataColumn(headerWord));
                }

                // Get cell data
                for (int r = 1; r < lines.Length; r++)
                {
                    string[] dataWords = lines[r].Split(',');
                    DataRow dr = dt.NewRow();
                    int columnIndex = 0;
                    foreach (string headerWord in headerLabels)
                    {
                        //dr["Row"] = i;
                        dr[headerWord] = dataWords[columnIndex++];
                    }
                    dt.Rows.Add(dr);
                    i++;
                }
                dataGridView1.DataSource = dt;
            }
        }

        private void goButton_Click(object sender, EventArgs e)
        {
            varBranch = dt.Rows[0][0].ToString();
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                varBranch = dt.Rows[0][i].ToString();

            }
            
        }
    }
}
