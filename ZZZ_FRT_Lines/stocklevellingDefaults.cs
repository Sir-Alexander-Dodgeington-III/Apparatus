using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;


namespace ZZZ_FRT_Lines
{
    public partial class stocklevellingForm : Form
    {
        public DataTable table = new DataTable();
        public stocklevellingForm()
        {
            InitializeComponent();
            this.CenterToScreen();

            SqlConnection conn2 = new SqlConnection(Helper.ConnString("AUTOPART"));
            {
                conn2.Open();
                pgTextBox.Text = pgTextBox.Text.Replace(",", "','");
                string pgString = pgTextBox.Text;     
                DataSet ds1 = new DataSet();
                SqlDataAdapter adapter = new SqlDataAdapter(
                $@"; WITH DfltPGs (xstring1) as (
                    SELECT xstring1 FROM MERI_StockLevelling WHERE Branch = 'Deflt' AND Prefix = 'P'
                ),

                CurrPGs (Branch, xstring1) as (
                SELECT Branch, xstring1 FROM MERI_StockLevelling WHERE Prefix = 'P'
                )

                SELECT II.Branch FROM DfltPGs I LEFT OUTER JOIN
                CurrPGs II ON I.xstring1 = II.xstring1
                GROUP BY II.Branch
                HAVING COUNT(I.xstring1) <> (SELECT COUNT(xstring1) FROM MERI_StockLevelling WHERE Prefix = 'P' AND Branch = 'Deflt')
                ORDER BY II.Branch ASC", conn2);
                adapter.Fill(ds1);
                this.pgListBox.DataSource = ds1.Tables[0];
                this.pgListBox.DisplayMember = "Branch";
                conn2.Close();

            }

            SqlConnection conn3 = new SqlConnection(Helper.ConnString("AUTOPART"));
            {
                conn3.Open();
                rngTextBox.Text = rngTextBox.Text.Replace(",", "','");
                string rngString = rngTextBox.Text;
                DataSet ds2 = new DataSet();
                SqlDataAdapter adapter1 = new SqlDataAdapter(
                $@"; WITH DfltPGs (xstring1) as (
                    SELECT xstring1 FROM MERI_StockLevelling WHERE Branch = 'Deflt' AND Prefix = 'R'
                ),

                CurrPGs (Branch, xstring1) as (
                SELECT Branch, xstring1 FROM MERI_StockLevelling WHERE Prefix = 'R'
                )

                SELECT II.Branch FROM DfltPGs I LEFT OUTER JOIN
                CurrPGs II ON I.xstring1 = II.xstring1
                GROUP BY II.Branch
                HAVING COUNT(I.xstring1) <> (SELECT COUNT(xstring1) FROM MERI_StockLevelling WHERE Prefix = 'R' AND Branch = 'Deflt')
                ORDER BY II.Branch ASC", conn3);
                adapter1.Fill(ds2);
                this.rangeListBox.DataSource = ds2.Tables[0];
                this.rangeListBox.DisplayMember = "Branch";
                conn3.Close();
            }
        }

        private void closeButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
