using Microsoft.VisualBasic;
using System;
using System.Windows.Forms;

namespace FormMain3
{
    public partial class FormMain3 : Form
    {
        string Query;
        public FormMain3()
        {
            InitializeComponent();
            this.CenterToScreen();
            this.Text = $@"Apparatus - {System.Security.Principal.WindowsIdentity.GetCurrent().Name}";
        }

        private void quitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void logInToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Hide();
            new Login_Form.LoginForm().Show();
        }

        private void partPGSoldPerDayToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            new PartPerDay_Form.PartPerDay_Form().Show();
        }

        private void onHandOnPOToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            new OnHandOnPo_Form.OnHandOnPO_Form().Show();
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

        private void infoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new Information.Information().Show();
        }

        private void kitEditorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new kitCreator.kitCreator().Show();
        }

        private void productFileMaintKitsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new PFM.PFM().Show();
        }

        private void branchRebatesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new BranchRebates.BranchRebates_Form().Show();
        }

        private void kitCustomizerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new ZZZ_FRT_Lines.KitCustomizer().Show();
        }

        private void sORToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new SOR.SOR().Show();
        }

        private void updateKitPricingToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new ZZZ_FRT_Lines.massUpdateKitPrice().Show();
        }
    }
}
