using System;
using System.Windows.Forms;
using Microsoft.VisualBasic;


namespace FormMain6
{
    public partial class FormMain6 : Form
    {
        public string Query;
        string var1;
        string pg;
        string range;
        public FormMain6()
        {
            InitializeComponent();
            this.CenterToScreen();
            this.Text = $@"Apparatus - {System.Security.Principal.WindowsIdentity.GetCurrent().Name}";
        }


        private void quitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void logInToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new Login_Form.LoginForm().Show();
        }

        private void bR60GeneratedPOsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new BR60generated.BR60GeneratedLines_Form().Show();
        }

        private void getReturnNoteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string pm = Interaction.InputBox("Enter the return note number", "Return Note");
            DialogResult dialogResult = MessageBox.Show("Exclude lines with rejected status?", "Exclude Rejects?", MessageBoxButtons.YesNo);

            Cursor.Current = Cursors.WaitCursor;

            if (dialogResult == DialogResult.Yes)
            {
                Query = ($@"
                                SELECt ROW_NUMBER() OVER ( ORDER BY Document) ' ' , Document, LEFT(Part,3) as PG, SUBSTRING(Part,4,99) as PartNumber, Qty, Unit, ' ' as Core, Rc, Comments FROM vw_MERI_ReturnNoteExport WHERE RNote = '{pm}' AND Status <> 'X'
                                  ");
            }
            else if (dialogResult == DialogResult.No)
            {
                Query = ($@"
                                    SELECt ROW_NUMBER() OVER ( ORDER BY Document) ' ' , Document, LEFT(Part,3) as PG, SUBSTRING(Part,4,99) as PartNumber, Qty, Unit, ' ' as Core, Rc, Comments FROM vw_MERI_ReturnNoteExport WHERE RNote = '{pm}'
                                  ");

            }

            QueryResults.QueryResults qr = new QueryResults.QueryResults(Query);
            qr.Show();
        }

        private void onHandOnPOToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new OnHandOnPo_Form.OnHandOnPO_Form().Show();
        }

        private void getCustomerXRefsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string pg = Interaction.InputBox("Please enter a PG", "Enter PG", "ALL");
            if (pg == "ALL" || pg == "all" || pg == "All")
            {
                MessageBox.Show("You must specify a product group");
                return;
            }
            else
            {
                pg = "'" + pg + "'";
            }

            Query = $@"

                     SELECT * FROM vw_MERI_CustXrefs WHERE PG = {pg} ORDER BY Part

                        ";

            QueryResults.QueryResults qr = new QueryResults.QueryResults(Query);
            qr.Show();
        }

        private void getProductValsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var1 = Interaction.InputBox("Please enter a Product Group or Range", "Enter PG or Range", "ALL");
            if (var1 == "ALL" || var1 == "all" || var1 == "All" || var1 == "")
            {
                MessageBox.Show("You must specify a product group or range!");
                return;
            }
            if (var1.Length == 3)
            {
                pg = "'" + var1 + "'";
                range = "NULL";
            }

            if (var1.Length > 3)
            {
                range = "'" + var1 + "'";
                pg = "NULL";
            }

            Query = $@"
                                DECLARE @PG VARCHAR(10)
                                DECLARE @Range VARCHAR(10)

                                SET @PG = {pg}
                                SET @Range = {range}

                                SELECT * FROM vw_MERI_ProductProdVals WHERE PG = IIF(@PG IS NULL, PG, @PG) AND Range = IIF(@Range IS NULL, Range, @Range) 

                        ";

            QueryResults.QueryResults qr = new QueryResults.QueryResults(Query);
            qr.Show();
        }

        private void getNonNumericToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string Query = $@"

                            SELECT part, value from prodvals WITH(nolock) WHERE attribute ='HazMat Weight (96)' AND value like '%[A-Z]%'

                        ";

            QueryResults.QueryResults qr = new QueryResults.QueryResults(Query);
            qr.Show();
        }

        private void onSUOrBOToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new OnBOonSU.OnBOonPO().Show();
        }

        private void promoSalesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new P55SalesCriteriaForm.P55SalesCriteriaForm().Show();
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new Information.Information().Show();
        }

        private void getSpecialTermsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new getSpecialTerms.getSpecialTerms().Show();
        }

        private void kitEditorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new kitCreator.kitCreator().Show();
        }

        private void productFileMaintKitsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new PFM.PFM().Show();
        }

        private void branchToolsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new BranchRebates.BranchRebates_Form().Show();
        }

    }
}
