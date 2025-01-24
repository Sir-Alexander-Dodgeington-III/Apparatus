using System;
using System.Drawing;
using System.Windows.Forms;

namespace StkValMonthly
{
    public partial class StkValMonthly : Form
    {
        private TextBox CurrentTextbox = null;
        public string branch;
        public string[] branchArray;
        public StkValMonthly()
        {
            InitializeComponent();
            this.CenterToScreen();
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void searchButton_Click(object sender, EventArgs e)
        {
            doStuff();
        }

        private void doStuff()
        {

            if (periodMonthTextbox.Text == "" | periodMonthTextbox.Text == "ALL" | periodMonthTextbox.Text == "all")
            {
                MessageBox.Show("Must specify period month before continuing");
                periodMonthTextbox.Focus();
                periodMonthTextbox.SelectAll();
                return;
            }

            if (branchTextbox.Text == "ALL" | branchTextbox.Text == "all")
            {
                branch = "null";
            }
            else
            {
                branch = branchTextbox.Text;
            }

            branchArray = branch.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            branch = "";
            foreach (string i in branchArray)
            {
                branch += $"'{i}',";
            }
            
            branch = branch.TrimEnd(',');

            if (branch != "null")
            {
                string Query = $@"
                    declare @PeriodMonth as VARCHAR(10)

                    SET @PeriodMonth = '{periodMonthTextbox.Text}'

                    SELECT Branch, CAST(SUM(Free * Lat) as decimal(18,2)) AS Latest, CAST(SUM(Free * Core) as decimal(18,2)) AS Core, CAST(SUM((Free * Lat) + (Free * Core)) as decimal(18,2)) AS Ext
                    FROM StockValuesMonthly
                    WHERE Period = @PeriodMonth and Branch in({branch})
                    Group By Branch
                    ";
                QueryResults.QueryResults qr = new QueryResults.QueryResults(Query);
                qr.Show();
            }
            else
            {
                string Query = $@"
                    declare @PeriodMonth as VARCHAR(10)
                    declare @Branch as VARCHAR(10)

                    SET @PeriodMonth = '{periodMonthTextbox.Text}'
                    set @Branch = '{branch}'

                    SELECT Branch, CAST(SUM(Free * Lat) as decimal(18,2)) AS Latest, CAST(SUM(Free * Core) as decimal(18,2)) AS Core, CAST(SUM((Free * Lat) + (Free * Core)) as decimal(18,2)) AS Ext
                    FROM StockValuesMonthly
                    WHERE Period = @PeriodMonth
                    Group By Branch
                    ";
                QueryResults.QueryResults qr = new QueryResults.QueryResults(Query);
                qr.Show();
            }

        }

        private void periodMonthTextbox_Enter(object sender, EventArgs e)
        {
            CurrentTextbox = (TextBox)sender;
            CurrentTextbox.BackColor = Color.Yellow;
        }

        private void periodMonthTextbox_Leave(object sender, EventArgs e)
        {
            CurrentTextbox = (TextBox)sender;
            CurrentTextbox.BackColor = Color.Empty;
            if (CurrentTextbox.Text == "")
            {
                CurrentTextbox.Text = "ALL";
            }
        }

        private void branchTextbox_Enter(object sender, EventArgs e)
        {
            CurrentTextbox = (TextBox)sender;
            CurrentTextbox.BackColor = Color.Yellow;
        }

        private void branchTextbox_Leave(object sender, EventArgs e)
        {
            CurrentTextbox = (TextBox)sender;
            CurrentTextbox.BackColor = Color.Empty;
            if (CurrentTextbox.Text == "")
            {
                CurrentTextbox.Text = "ALL";
            }
        }
    }
}
