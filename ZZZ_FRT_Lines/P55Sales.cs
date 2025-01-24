using System;
using System.Drawing;
using System.Windows.Forms;

namespace P55SalesCriteriaForm
{
    public partial class P55SalesCriteriaForm : Form
    {
        private TextBox CurrentTextbox = null;
        public string branch;
        public string promoName;
        public string rep;
        public string endDate;
        public P55SalesCriteriaForm()
        {
            InitializeComponent();
            this.CenterToScreen();
        }

        private void colorActiveTextbox_Leave(object sender, EventArgs e)
        {
            CurrentTextbox = (TextBox)sender;
            CurrentTextbox.BackColor = Color.Empty;
            if (CurrentTextbox.Text == "")
            {
                CurrentTextbox.Text = "ALL";
            }
        }

        private void colorActiveTextbox_Enter(object sender, EventArgs e)
        {
            CurrentTextbox = (TextBox)sender;
            CurrentTextbox.BackColor = Color.Yellow;
        }

        private void searchButton_Click(object sender, EventArgs e)
        {
            doStuff();
        }


        private void withDetailsCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            withTotalsCheckbox.Checked = false;
            withBranchTotalsCheckbox.Checked = false;
            withDetailsCheckbox.Checked = true;
            repTotalsCheckbox.Checked = false;
        }

        private void withTotalsCheckbox_CheckStateChanged(object sender, EventArgs e)
        {
            withDetailsCheckbox.Checked = false;
            withBranchTotalsCheckbox.Checked = false;
            withTotalsCheckbox.Checked = true;
            repTotalsCheckbox.Checked = false;
        }

        private void withBranchTotalsCheckbox_Click(object sender, EventArgs e)
        {
            withDetailsCheckbox.Checked = false;
            withBranchTotalsCheckbox.Checked = true;
            withTotalsCheckbox.Checked = false;
            repTotalsCheckbox.Checked = false;
        }

        private void repTotalsCheckbox_Click(object sender, EventArgs e)
        {
            withDetailsCheckbox.Checked = false;
            withBranchTotalsCheckbox.Checked = false;
            withTotalsCheckbox.Checked = false;
            repTotalsCheckbox.Checked = true;
        }

        private void P55SalesCriteriaForm_KeyPress(object sender, KeyEventArgs e)
        {
            if (e.Handled != true)
            {
                if (e.KeyCode == Keys.F5)
                {
                    doStuff();
                }
                else if(e.KeyCode == Keys.Enter && searchButton.Focused)
                {
                    doStuff();
                }
            }
            else
            {
                return;
            }

        }

        private void doStuff()
        {
            if (branchTextbox.Text == "ALL" | branchTextbox.Text == "all")
            {
                branch = "null";
            }
            else
            {
                branch = "'" + branchTextbox.Text + "'";
            }

            if (promoNameTextBox.Text == "ALL" | promoNameTextBox.Text == "all")
            {
                promoName = "null";
            }
            else
            {
                promoName = "'" + promoNameTextBox.Text + "'";
            }

            if (repTotalTextbox.Text == "ALL" | repTotalTextbox.Text == "all")
            {
                rep = "null";
            }
            else
            {
                rep = "'" + repTotalTextbox.Text + "'";
            }

            if (startDateTextbox.Text == "" | startDateTextbox.Text == "ALL" | startDateTextbox.Text == "all")
            {
                MessageBox.Show("Starting date must be specified");
                startDateTextbox.Focus();
                return;
            }

            if (endDateTextbox.Text == "" | endDateTextbox.Text == "ALL" | endDateTextbox.Text == "all")
            {
                endDate = DateTime.Now.ToString("MM/dd/yyyy");
                endDateTextbox.Text = endDate;
            }

            if (withDetailsCheckbox.Checked)
            {
                string Query = $@"
                        DECLARE @Branch VARCHAR(10)
                        DECLARE @Salesrep VARCHAR(10)
                        DECLARE @Promo VARCHAR(25)

                        SET @Branch = {branch}
                        SET @Salesrep = {rep}
                        SET @Promo = {promoName}

                        SELECT I.Branch, 
                        I.Acct, 
                        CAST(I.DateTime as date) as InvDate, 
                        M.SubKey3, 
                        M.SubKey1, 
                        CAST(I.Qty as int) as Qty, 
                        CAST(I.Unit as Decimal(18,2)) as Unit, 
                        CAST((I.Qty * I.Unit) as Decimal(18,2)) as Ext,
                        CAST(CAST(M.CStart as varchar(255)) as date) as 'Promo Start', 
                        CAST(CAST(M.CEnd as varchar(255)) as date) as 'Promo End'

                        FROM MVPR M INNER JOIN VW_MERI_I I ON M.SubKey3 = I.Part 
                        WHERE SubKey1 IN(@Promo) AND 
                        CAST(CAST(CStart as varchar(255)) as date) BETWEEN '{startDateTextbox.Text}' AND '{endDateTextbox.Text}' AND 
                        CAST(I.DateTime as date) BETWEEN 
                        CAST(CAST(M.CStart as varchar(255)) as date) 
                        AND 
                        CAST(CAST(M.CEnd as varchar(255)) as date) 
                        AND 
                        Salesrep = IIF(@Salesrep IS NULL, I.Salesrep, @Salesrep) AND
                        Branch = IIF(@Branch IS NULL, I.Branch, @Branch) AND
                        SubKey1 = IIF(@Promo IS NULL, M.SubKey3, @Promo) AND
                        I.Ltype != 'None'
                        ORDER BY I.Branch, I.Part
                        ";
                QueryResults.QueryResults qr = new QueryResults.QueryResults(Query);
                qr.Show();
            }
            else if (withTotalsCheckbox.Checked)
            {
                string Query = $@"
                        DECLARE @Branch VARCHAR(10)
                        DECLARE @Salesrep VARCHAR(10)
                        DECLARE @Promo VARCHAR(25)

                        SET @Branch = {branch}
                        SET @Salesrep = {rep}
                        SET @Promo = {promoName}
                        
                        SELECT
                        M.SubKey3, 
                        M.SubKey1, 
                        SUM(CAST(I.Qty as int)) as Qty, 
                        SUM(CAST((I.Qty * I.Unit) as Decimal(18,2))) as Ext,
                        CAST(CAST(M.CStart as varchar(255)) as date) as 'Promo Start', 
                        CAST(CAST(M.CEnd as varchar(255)) as date) as 'Promo End'

                        FROM MVPR M INNER JOIN VW_MERI_I I ON M.SubKey3 = I.Part 
                        WHERE SubKey1 IN(@Promo) AND 
                        CAST(CAST(CStart as varchar(255)) as date) BETWEEN '{startDateTextbox.Text}' AND '{endDateTextbox.Text}' AND 
                        CAST(I.DateTime as date) BETWEEN 
                        CAST(CAST(M.CStart as varchar(255)) as date)
                        AND 
                        CAST(CAST(M.CEnd as varchar(255)) as date)
                        AND 
                        Salesrep = IIF(@Salesrep IS NULL, I.Salesrep, @Salesrep) AND
                        Branch = IIF(@Branch IS NULL, I.Branch, @Branch) AND
                        SubKey1 = IIF(@Promo IS NULL, M.SubKey3, @Promo) AND
                        I.Ltype != 'None'
                        GROUP BY M.SubKey3, M.SubKey1, M.CStart, M.CEnd
                        ORDER BY M.SubKey3
                        ";
                QueryResults.QueryResults qr = new QueryResults.QueryResults(Query);
                qr.Show();
            }
            else if (withBranchTotalsCheckbox.Checked)
            {
                string Query = $@"
                        DECLARE @Branch VARCHAR(10)
                        DECLARE @Salesrep VARCHAR(10)
                        DECLARE @Promo VARCHAR(25)

                        SET @Branch = {branch}
                        SET @Salesrep = {rep}
                        SET @Promo = {promoName}
                        
                        SELECT
                        I.Branch,
                        M.SubKey3, 
                        M.SubKey1, 
                        SUM(CAST(I.Qty as int)) as Qty, 
                        SUM(CAST((I.Qty * I.Unit) as Decimal(18,2))) as Ext,
                        CAST(CAST(M.CStart as varchar(255)) as date) as 'Promo Start', 
                        CAST(CAST(M.CEnd as varchar(255)) as date) as 'Promo End'

                        FROM MVPR M INNER JOIN VW_MERI_I I ON M.SubKey3 = I.Part 
                        WHERE SubKey1 IN(@Promo) AND 
                        CAST(CAST(CStart as varchar(255)) as date) BETWEEN '{startDateTextbox.Text}' AND '{endDateTextbox.Text}' AND 
                        CAST(I.DateTime as date) BETWEEN 
                        CAST(CAST(M.CStart as varchar(255)) as date)
                        AND 
                        CAST(CAST(M.CEnd as varchar(255)) as date)
                        AND 
                        Salesrep = IIF(@Salesrep IS NULL, I.Salesrep, @Salesrep) AND
                        Branch = IIF(@Branch IS NULL, I.Branch, @Branch) AND
                        SubKey1 = IIF(@Promo IS NULL, M.SubKey3, @Promo) AND
                        I.Ltype != 'None'
                        GROUP BY I.Branch, M.SubKey3, M.SubKey1, M.CStart, M.CEnd
                        ORDER BY I.Branch, M.SubKey3
                        ";
                QueryResults.QueryResults qr = new QueryResults.QueryResults(Query);
                qr.Show();
            }
            else if (repTotalsCheckbox.Checked)
            {
                string Query = $@"
                        DECLARE @Branch VARCHAR(10)
                        DECLARE @Salesrep VARCHAR(10)
                        DECLARE @Promo VARCHAR(25)

                        SET @Branch = {branch}
                        SET @Salesrep = {rep}
                        SET @Promo = {promoName}

                        SELECT
                        I.Branch,
                        I.SalesRep,
                        M.SubKey1, 
                        SUM(CAST(I.Qty as int)) as Qty, 
                        SUM(CAST((I.Qty * I.Unit) as Decimal(18,2))) as Ext,
                        CAST(CAST(M.CStart as varchar(255)) as date) as 'Promo Start', 
                        CAST(CAST(M.CEnd as varchar(255)) as date) as 'Promo End'

                        FROM MVPR M INNER JOIN VW_MERI_I I ON M.SubKey3 = I.Part 
                        WHERE SubKey1 IN(@Promo) AND 
                        CAST(CAST(CStart as varchar(255)) as date) BETWEEN '{startDateTextbox.Text}' AND '{endDateTextbox.Text}' AND 
                        CAST(I.DateTime as date) BETWEEN 
                        CAST(CAST(M.CStart as varchar(255)) as date)
                        AND 
                        CAST(CAST(M.CEnd as varchar(255)) as date)
                        AND 
                        Salesrep = IIF(@Salesrep IS NULL, I.Salesrep, @Salesrep) AND
                        Branch = IIF(@Branch IS NULL, I.Branch, @Branch) AND
                        SubKey1 = IIF(@Promo IS NULL, M.SubKey3, @Promo) AND
                        I.Ltype != 'None'
                        GROUP BY I.Salesrep, M.SubKey1, M.CStart, M.CEnd, I.Branch
                        ORDER BY I.Branch
                        ";
                QueryResults.QueryResults qr = new QueryResults.QueryResults(Query);
                qr.Show();
            }
        }
    }
}
