using System;
using System.Data.SqlClient;
using System.Windows.Forms;
using Microsoft.VisualBasic;


namespace FormMain7
{
    public partial class FormMain7 : Form
    {
        string Query;
        public FormMain7()
        {
            InitializeComponent();
            this.CenterToScreen();
            this.Text = $@"Apparatus - {System.Security.Principal.WindowsIdentity.GetCurrent().Name}";
        }

        private void quitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void onHandOnPOToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            new OnHandOnPo_Form.OnHandOnPO_Form().Show();
        }

        private void partPGSoldPerDayToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            new PartPerDay_Form.PartPerDay_Form().Show();
        }

        private void getBR30BackOrdersToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Query = ($@"
                                WITH 
                                CTE1(Branch, DateTime, Acct, COrder, BOrder, PDoc, Part, Qty) AS (SELECT        BHeads.Branch, BHeads.DateTime, BHeads.Acct, BHeads.Corder, BHeads.[Document] AS BDoc, PHeads.[Document] AS PDoc, 
                                 BLines.Part, BLines.Qty
                                FROM            BLines INNER JOIN
                                BHeads ON BLines.[Document] = BHeads.[Document] LEFT OUTER JOIN
                                RLines INNER JOIN
                                RHeads ON RLines.[Document] = RHeads.[Document] INNER JOIN
                                PLines INNER JOIN
                                PHeads ON PLines.[Document] = PHeads.[Document] ON RHeads.POrder = PHeads.[Document] ON BHeads.Corder = PHeads.[Document]
                                WHERE        (BHeads.[Document] LIKE '30B%') AND (BLines.CQty = BLines.Qty) AND (BLines.Qty > 0)), 

                                CTE2(Part, Free) AS
                                (SELECT        Part, Free
                                FROM            VW_MERI_Pro
                                WHERE        (Free > 0) AND (Branch = 'BR60')), 

                                CTE3(Part, Free) AS
                                (SELECT        Part, Free
                                FROM            Stock
                                WHERE        (Free > 0) AND (Branch = 'BR30')), 

                                CTE4(Part, Qty) AS
                                (SELECT        Part, SUM(Qty) AS Expr1
                                FROM            Locations
                                WHERE        (Qty > 0) AND (Branch = 'BR30') AND (Location IN('QT', 'QUAR'))
                                GROUP BY Part), 

                                CTE5(Part, Qty) AS
                                (SELECT        Part, SUM(Qty) AS Expr1
                                FROM            PLines AS PLines_1
                                WHERE        (Qty > 0) AND (Branch = 'BR30') AND (CQty = Qty)
                                GROUP BY Document, Part)


                                SELECT        
                                I.Branch, 
                                I.Acct, 
                                I.Part, 
                                SUM(CAST(I.Qty AS INT)) AS OnBOatBR60, 
                                ISNULL(SUM(CAST(V.Qty as int)), 0) AS BR30onPO, 
                                '' as ' ',
                                ISNULL(CAST(IV.Qty as int), 0) AS BR60_QT, 
                                ISNULL(CAST(III.Free as int), 0) - ISNULL(CAST(IV.Qty as int), 0) AS BR60_Free, 
                                ISNULL(CAST(IV.Qty as int), 0) + (ISNULL(CAST(III.Free as int), 0) - ISNULL(CAST(IV.Qty as int), 0)) AS Total_BR30_Qty, 
                                CAST(ISNULL(II.Free, 0) AS INT) AS BR60_FreeStock
                                FROM            CTE1 AS I LEFT OUTER JOIN
                                CTE4 AS IV ON I.Part = IV.Part LEFT OUTER JOIN
                                CTE5 AS V ON I.Part = V.Part LEFT OUTER JOIN
                                CTE3 AS III ON I.Part = III.Part LEFT OUTER JOIN
                                CTE2 AS II ON I.Part = II.Part
                                WHERE II.Free > 0
                                GROUP BY I.Part, I.Acct, II.Free, I.Branch, I.Qty, IV.Qty, III.Free

                                ORDER BY I.Part, I.Acct
                            ");

            QueryResults.QueryResults qr = new QueryResults.QueryResults(Query);
            qr.Show();
        }

        private void getBR60BackOrdersToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Query = ($@"
                                WITH 
                                CTE1(Branch, DateTime, Acct, COrder, BOrder, PDoc, Part, Qty) AS (SELECT        BHeads.Branch, BHeads.DateTime, BHeads.Acct, BHeads.Corder, BHeads.[Document] AS BDoc, PHeads.[Document] AS PDoc, 
                                 BLines.Part, BLines.Qty
                                FROM            BLines INNER JOIN
                                BHeads ON BLines.[Document] = BHeads.[Document] LEFT OUTER JOIN
                                RLines INNER JOIN
                                RHeads ON RLines.[Document] = RHeads.[Document] INNER JOIN
                                PLines INNER JOIN
                                PHeads ON PLines.[Document] = PHeads.[Document] ON RHeads.POrder = PHeads.[Document] ON BHeads.Corder = PHeads.[Document]
                                WHERE        (BHeads.[Document] LIKE '60B%') AND (BLines.CQty = BLines.Qty) AND (BLines.Qty > 0)), 

                                CTE2(Part, Free) AS
                                (SELECT        Part, Free
                                FROM            VW_MERI_Pro
                                WHERE        (Free > 0) AND (Branch = 'BR30')), 

                                CTE3(Part, Free) AS
                                (SELECT        Part, Free
                                FROM            Stock
                                WHERE        (Free > 0) AND (Branch = 'BR60')), 

                                CTE4(Part, Qty) AS
                                (SELECT        Part, SUM(Qty) AS Expr1
                                FROM            Locations
                                WHERE        (Qty > 0) AND (Branch = 'BR60') AND (Location IN('QT', 'QUAR'))
                                GROUP BY Part), 

                                CTE5(Part, Qty) AS
                                (SELECT        Part, SUM(Qty) AS Expr1
                                FROM            PLines AS PLines_1
                                WHERE        (Qty > 0) AND (Branch = 'BR60') AND (CQty = Qty)
                                GROUP BY Document, Part)


                                SELECT        
                                I.Branch, 
                                I.Acct, 
                                I.Part, 
                                SUM(CAST(I.Qty AS INT)) AS OnBOatBR60, 
                                ISNULL(SUM(CAST(V.Qty as int)), 0) AS BR60onPO, 
                                '' as ' ',
                                ISNULL(CAST(IV.Qty as int), 0) AS BR60_QT, 
                                ISNULL(CAST(III.Free as int), 0) - ISNULL(CAST(IV.Qty as int), 0) AS BR60_Free, 
                                ISNULL(CAST(IV.Qty as int), 0) + (ISNULL(CAST(III.Free as int), 0) - ISNULL(CAST(IV.Qty as int), 0)) AS Total_BR60_Qty, 
                                CAST(ISNULL(II.Free, 0) AS INT) AS BR30_FreeStock
                                FROM            CTE1 AS I LEFT OUTER JOIN
                                CTE4 AS IV ON I.Part = IV.Part LEFT OUTER JOIN
                                CTE5 AS V ON I.Part = V.Part LEFT OUTER JOIN
                                CTE3 AS III ON I.Part = III.Part LEFT OUTER JOIN
                                CTE2 AS II ON I.Part = II.Part
                                WHERE II.Free > 0
                                GROUP BY I.Part, I.Acct, II.Free, I.Branch, I.Qty, IV.Qty, III.Free

                                ORDER BY I.Part, I.Acct
                            ");

            QueryResults.QueryResults qr = new QueryResults.QueryResults(Query);
            qr.Show();
        }

        private void infoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new Information.Information().Show();
        }

        private void MMMPoSToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new MMMPoS.MMMPoS().Show();
        }

        private void logInToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Hide();
            new Login_Form.LoginForm().Show();
        }

        private void fillRateToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            new FillRate.FillRateReport().Show();
        }

        private void branchRebatesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new BranchRebates.BranchRebates_Form().Show();
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

        private void wIXReportToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string startDate = Interaction.InputBox("Please enter a begining date", "Start Date", "MM/DD/YYYY");
            string endDate = Interaction.InputBox("Please enter a ending date", "End Date", "MM/DD/YYYY");
            if (endDate == "MM/DD/YYYY")
            {
                endDate = DateTime.Now.ToString();
            }

            Query = ($@"
                                SELECT H.Branch, H.Acct, C.Name, L.Part, P.[Desc], L.[Document], L.InvInits, H.DateTime, L.Qty, L.Unit * L.Qty AS Sales, L.Price, SL.P8 FROM ILines as L WITH (NOLOCK) INNER JOIN IHeads as H ON L.[Document] = H.[Document] INNER JOIN Customer as C ON H.Acct = C.KeyCode INNER JOIN MERI_SELLINGLEVELS as SL ON L.Part = SL.Part INNER JOIN Product as P ON L.Part = P.KeyCode WHERE CAST(L.DateTime as date) BETWEEN '{startDate}' AND '{endDate}' AND L.PG = 'WIX' GROUP BY L.[Document], L.Part, H.Acct, H.DateTime, H.Branch, C.Name, P.[Desc], L.Price, SL.P8, L.Qty, L.Unit, L.InvInits ORDER BY H.DateTime ASc
                                  ");

            QueryResults.QueryResults qr = new QueryResults.QueryResults(Query);
            qr.Show();
        }

        private void lastReceivedToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new LastReceived.LastReceived().Show();
        }

        private void clearLockedStockSheetsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SqlConnection conn = new System.Data.SqlClient.SqlConnection(Helper.ConnString("AUTOPART"));

            string document = Interaction.InputBox("Stock sheet number:", "Please enter document number");

            if (document == "")
            {
                MessageBox.Show("successfully cancelled", "Notice", MessageBoxButtons.OK);
                return;
            }
            else
            {
                conn.Open();
                string query = ($@"
                                SELECT document FROM SHeads WHERE Document = '{document}'
                            ");
                SqlCommand cmd = new SqlCommand(query, conn);
                string result = (string)cmd.ExecuteScalar();

                if (result == null)
                {
                    MessageBox.Show($"Document number {document} not found", "Notice", MessageBoxButtons.OK);
                    return;
                }

                if (document != null)
                {
                    string query1 = ($@"
                                UPDATE SHeads SET LockedBy = NULL WHERE Document = '{document}'
                            ");
                    SqlCommand cmd1 = new SqlCommand(query1, conn);
                    cmd1.ExecuteNonQuery();
                    MessageBox.Show("Done.");
                }
            }
        }

        private void clearBulkLocManagementToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DialogResult dialogResult = MessageBox.Show("Do you wish to clear the param for Bulk Location Management?", "Notice", MessageBoxButtons.YesNo);
            if (dialogResult == DialogResult.Yes)
            {
                SqlConnection conn = new System.Data.SqlClient.SqlConnection(Helper.ConnString("AUTOPART"));
                string username = System.Security.Principal.WindowsIdentity.GetCurrent().Name;
                int stringLength = username.Length;
                string currOperator = username.Substring(10, 5);

                conn.Open();
                string query = ($@"
                                SELECT a2 FROM Codes WHERE Prefix = 'O' and KeyCode = '{currOperator}'
                            ");
                SqlCommand cmd = new SqlCommand(query, conn);
                string USRoll = (string)cmd.ExecuteScalar();

                if (USRoll == "BR30")
                {
                    string query1 = ($@"
                                UPDATE Params SET Dat = '' WHERE KeyCode = 'BULKLOC'
                            ");
                    SqlCommand cmd1 = new SqlCommand(query1, conn);
                    cmd1.ExecuteNonQuery();
                    MessageBox.Show("Done.");
                }

                if (USRoll == "BR60")
                {
                    string query1 = ($@"
                                UPDATE Params60 SET Dat = '' WHERE KeyCode = 'BULKLOC'
                            ");
                    SqlCommand cmd1 = new SqlCommand(query1, conn);
                    cmd1.ExecuteNonQuery();
                    MessageBox.Show("Done.");
                }

                if (!(USRoll == "BR30" || USRoll == "BR60"))
                {
                    MessageBox.Show("You do not have permission to clear this data");
                }
            }
            else if (dialogResult == DialogResult.No)
            {
                return;
            }
        }

        private void wMSSortToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new wmsSort.wmsSort().Show();
        }

        private void leftVsRightFreeStockSearchToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new LeftvsRight.LeftvsRight().Show();
        }

        private void singlePartSearchToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new SinglePartSearch.SPS().Show();
        }

        private void queryBuilderToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            new QueryBuilder.QueryBuilder().Show();
        }

        private void kitEditorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new kitCreator.kitCreator().Show();
        }

        private void productFileMaintKitsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new PFM.PFM().Show();
        }
    }
}

