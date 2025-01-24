using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace Multi_PO
{
    public partial class defaultSupplier_MultiPO : Form
    {
        DataTable dt = new DataTable();
        public defaultSupplier_MultiPO()
        {
            InitializeComponent();
        }

        private void defaultSupplier_MultiPO_Load(object sender, EventArgs e)
        {
            SqlConnection conn = new SqlConnection(Helper.ConnString("AUTOPART"));
            {
                conn.Open();
                //DataSet ds1 = new DataSet();
                SqlDataAdapter adapter = new SqlDataAdapter(
                $@"
                    SELECT * FROM MERI_MultiPO_Defaults
                    ", conn);
                adapter.Fill(dt);

                dataGridView1.DataSource = null;
                dataGridView1.DataSource = dt;
                conn.Close();
            }
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            using (SqlConnection dbConnection = new SqlConnection(Helper.ConnString("AUTOPART")))
            {
                dbConnection.Open();

                string query = "DELETE FROM MERI_MultiPO_Defaults";
                SqlCommand command = new SqlCommand(query, dbConnection);
                command.ExecuteNonQuery();

                using (SqlBulkCopy s = new SqlBulkCopy(dbConnection))
                {
                    s.DestinationTableName = "MERI_MultiPO_Defaults";
                    foreach (var column in dt.Columns)
                        s.ColumnMappings.Add(column.ToString(), column.ToString());
                    s.WriteToServer(dt);
                }
            }
        }

        private void quitButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
