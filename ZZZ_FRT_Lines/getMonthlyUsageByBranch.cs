using System;
using System.Drawing;
using System.Windows.Forms;

namespace getMonthlyUsageByBranch
{
    public partial class getMonthlyUsageByBranch : Form
    {
        private TextBox CurrentTextbox = null;
        public string year;
        public string branch;
        public string[] branchArray;
        public string pg;
        public string range;
        public getMonthlyUsageByBranch()
        {
            InitializeComponent();
            this.CenterToScreen();
        }

        private void textBox_Enter(object sender, EventArgs e)
        {
            CurrentTextbox = (TextBox)sender;
            CurrentTextbox.BackColor = Color.Yellow;
            CurrentTextbox.SelectAll();
        }

        private void textBox_Leave(object sender, EventArgs e)
        {
            CurrentTextbox = (TextBox)sender;
            CurrentTextbox.BackColor = Color.Empty;
            if (CurrentTextbox.Text == "")
            {
                CurrentTextbox.Text = "ALL";
            }
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void searchButton_Click(object sender, EventArgs e)
        {
            doStuff();
        }

        public void doStuff()
        {
            if (branchTextbox.Text == "ALL" || branchTextbox.Text == "all" || branchTextbox.Text == "All")
            {
                MessageBox.Show("You must specify branch");
                return;
            }
            else
            {
                branch = "'" + branchTextbox.Text + "'";
            }

            if (pgTextbox.Text == "ALL" || pgTextbox.Text == "all" || pgTextbox.Text == "All")
            {
                pg = "NULL";
            }
            else
            {
                pg = "'" + pgTextbox.Text + "'";
            }

            if (rangeTextbox.Text == "ALL" || rangeTextbox.Text == "all" || rangeTextbox.Text == "All")
            {
                range = "NULL";
            }
            else
            {
                range = "'" + rangeTextbox.Text + "'";
            }

            if (yearTextbox.Text == "ALL" || yearTextbox.Text == "all" || yearTextbox.Text == "All")
            {
                MessageBox.Show("You must specify a year");
                return;
            }
            else
            {
                year = "'" + yearTextbox.Text + "'";
            }

            if (range == "NULL" && pg == "NULL")
            {
                MessageBox.Show("You must declare a PG or Range. Both can not be left unspecified");
                return;
            }

            branch = branchTextbox.Text;
            //customer = customer.Replace(",", "\",\"");
            branchArray = branch.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            branch = "";
            foreach (string i in branchArray)
            {
                branch += $"'{i}',";
            }
            branch = branch.TrimEnd(',');

            string Query = $@"
                            DECLARE @Year VARCHAR(10)
                            DECLARE @PG VARCHAR(10)
                            DECLARE @Range VARCHAR(10)

                            SET @Year = {year}
                            SET @PG = {pg}
                            SET @Range = {range}


                            SELECT Part, Jan, Feb, Mar, Apr, May, Jun, Jul, Aug, Sep, Oct, Nov, Dec FROM vw_MERI_MVK 
                            WHERE Year = @Year AND Branch = {branch} AND PG = IIF(@PG IS NULL, PG, @PG) AND Range = IIF(@Range IS NULL, Range, @Range)
                    ";

            QueryResults.QueryResults qr = new QueryResults.QueryResults(Query);
            qr.Show();
        }
    }
}
