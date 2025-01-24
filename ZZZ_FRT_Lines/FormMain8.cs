using System;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Microsoft.VisualBasic;
using System.Net.Mail;
using System.Net;
using File = System.IO.File;
using Renci.SshNet;
//using System.Threading.Tasks;
using ZZZ_FRT_Lines;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.Windows.Forms.DataVisualization.Charting;

namespace FormMain8
{
    public partial class FormMain8 : Form
    {
        public DataTable dt = new DataTable();
        public DataTable dt1 = new DataTable();
        public DataTable dt2 = new DataTable();
        public DataTable dt3 = new DataTable();

        public DataTable wmsTable1 = new DataTable();
        public DataTable wmsTable2 = new DataTable();
        public DataTable wmsTable3 = new DataTable();
        public DataTable wmsTable4 = new DataTable();

        public DataTable tblParts = new DataTable();

        public string user = System.Security.Principal.WindowsIdentity.GetCurrent().Name.Replace(@"MERRILLCO\", "");
        public System.Timers.Timer timer1 = new System.Timers.Timer();
        public Timer timer0;
        public Timer timer2;
        public Timer timer3;

        public string strSort;
        public string strFirstGroup;
        public string strGroupOrder;
        public string strGroupPad;
        public string items;
        public string strPadString;
        public string strSplitPartNo;
        public string Query;
        public string pg;
        public string range;
        public FormMain8()
        {
            InitializeComponent();
            this.CenterToScreen();
            this.Text = $@"Apparatus - {System.Security.Principal.WindowsIdentity.GetCurrent().Name}";
        } 


        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            Environment.Exit(0);
        }

        private void FormMain8_Load(object sender, EventArgs e)
        {
            LoadApparatus loadingForm = new LoadApparatus();
            loadingForm.Show();

            SqlConnection conn = new SqlConnection(Helper.ConnString("AUTOPART"));


            string sqlQuery = @"
                UPDATE MERI_MinMaxChanges 
                SET Branch = (
                    SELECT C.A2
                    FROM Codes C
                    WHERE C.A1 = MERI_MinMaxChanges.Name AND C.Prefix = 'O'
                )
                WHERE Branch NOT IN (
                    SELECT Branch
                    FROM Branches
                );";

            try
            {

                conn.Open();
                using (SqlCommand command = new SqlCommand(sqlQuery, conn))
                {
                    command.ExecuteNonQuery();
                }

            }
            catch (SqlException ex)
            {
                MessageBox.Show("SQL error occurred: " + ex.Message);
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred: " + ex.Message);
            }
            conn.Close();


            string sqlQuery2 = @"
                DELETE FROM MERI_MinMaxChanges 
                WHERE Part NOT IN(Select KeyCode FROM Product)";

            try
            {

                conn.Open();
                using (SqlCommand command = new SqlCommand(sqlQuery2, conn))
                {
                    command.ExecuteNonQuery();
                }

            }
            catch (SqlException ex)
            {
                MessageBox.Show("SQL error occurred: " + ex.Message);
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred: " + ex.Message);
            }
            conn.Close();

            conn.Open();
            //DataSet ds1 = new DataSet();
            SqlDataAdapter adapter = new SqlDataAdapter(
            $@"
                    SELECT * FROM MERI_MinMaxChanges WHERE Completed != 'Y'
                   ", conn);
            adapter.Fill(dt);
            conn.Close();
            items = dt.Rows.Count.ToString();
            if (dt.Rows.Count > 0)
            {
                MinMaxRequestsButton.Visible = true;
                MinMaxRequestsButton.BackColor = System.Drawing.Color.Red;
                MinMaxRequestsButton.FlatAppearance.BorderColor = System.Drawing.Color.Black;
                MinMaxRequestsButton.FlatAppearance.BorderSize = 1;
                MinMaxRequestsButton.Text = $@"{items} new Min/Max change requests waiting";
            }
            else
            {
                MinMaxRequestsButton.Visible = false;
            }

            timer0 = new Timer();
            timer0.Tick += new EventHandler(reload_Button);
            timer0.Interval = 300000; //(in miliseconds)
            timer0.Start();

            timer2 = new Timer();
            timer2.Tick += new EventHandler(reload_Button_1);
            timer2.Interval = 600000; //(in miliseconds)
            timer2.Start();

            timer3 = new Timer();
            timer3.Tick += new EventHandler(inboxMonitor);
            timer3.Interval = 150000; //(in miliseconds)
            timer3.Start();

            loadingForm.Close();
        }

        private void reload_Button(object sender, EventArgs e)
        {
            dt.Rows.Clear();
            SqlConnection conn = new SqlConnection(Helper.ConnString("AUTOPART"));
            conn.Open();
            //DataSet ds1 = new DataSet();
            SqlDataAdapter adapter = new SqlDataAdapter(
            $@"
                    SELECT * FROM MERI_MinMaxChanges WHERE Completed != 'Y'
                   ", conn);
            adapter.Fill(dt);
            conn.Close();
            items = dt.Rows.Count.ToString();
            if (dt.Rows.Count > 0)
            {
                MinMaxRequestsButton.Visible = true;
                MinMaxRequestsButton.BackColor = System.Drawing.Color.Red;
                MinMaxRequestsButton.FlatAppearance.BorderColor = System.Drawing.Color.Black;
                MinMaxRequestsButton.FlatAppearance.BorderSize = 1;
                MinMaxRequestsButton.Text = $@"{items} new Min/Max change requests waiting";
            }
            else
            {
                MinMaxRequestsButton.Visible = false;
            }
            timer0.Stop();
            timer0.Start();
        }

        private void reload_Button_1(object sender, EventArgs e)
        {
            dt3.Rows.Clear();
            SqlConnection conn = new SqlConnection(Helper.ConnString("AUTOPART"));
            conn.Open();
            string query = ($@"
                                UPDATE MERI_MinMaxChanges 
                                SET Completed = 'Y', CompletedInits = 'AUTO', NOTES = 'EMAIL' 
                                FROM MERI_MinMaxChanges MNC 
                                INNER JOIN VW_MERI_Pro P ON MNC.Part = P.Part AND MNC.Branch = P.Branch 
                                WHERE MNC.NewMin = P.Min AND MNC.NewMax = P.Max AND MNC.Completed = ''
                            ");
            SqlCommand cmd = new SqlCommand(query, conn);
            cmd.ExecuteNonQuery();
            conn.Close();


            timer2.Stop();
            timer2.Start();
        }

        private void inboxMonitor(object sender, EventArgs e)
        {
            string username = System.Security.Principal.WindowsIdentity.GetCurrent().Name;
            if (username.Contains("adodge") || username.Contains("rfrankenstein") || username.Contains("charris") || username.Contains("jadams") || username.Contains("tpetersen") || username.Contains("admin") || username.Contains("arnold"))
            {
                int fileCount = (from file in Directory.EnumerateFiles(@"\\svrsql1\AUTOPART\Mail\Inbox", "*.xml", SearchOption.TopDirectoryOnly) select file).Count();
                string uname = "arnoldmotorsupply@arnoldgroupweb.com";
                string pword = "Merrillco1141";
                ICredentialsByHost credentials = new NetworkCredential(uname, pword);
                if (fileCount > 25)
                {
                    SmtpClient client = new SmtpClient("smtp.gmail.com", 587)
                    {
                        Host = "smtp.gmail.com",
                        Port = 587,
                        EnableSsl = true,
                        Credentials = credentials
                    };
                    try
                    {
                        MailMessage mail = new MailMessage();
                        mail.From = new MailAddress("arnoldmotorsupply@arnoldgroupweb.com");
                        mail.Subject = "Online order inbox monitor alert";
                        mail.To.Add("itdepartment@arnoldgroupweb.com");
                        mail.To.Add("autopart@arnoldgroupweb.com");
                        mail.Body = $@"Online order count threshold is above the recomended amount. The current amount is {fileCount}. Please check the status of the Autocoms on SVRAPP1 as soon as possible.";
                        //client.Send("arnoldmotorsupply@arnoldgroupweb.com", "itdepartment@arnoldgroupweb.com, autopart@arnoldgroupweb.com", "Online order inbox monitor alert", $@"Online order count threshold is above the recomended amount. The current amount is {fileCount}. Please check the status of the Autocoms on SVRAPP1 as soon as possible.");
                        client.Send(mail);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message, "Online Order Inbox Alert. Check Autocoms and less secure apps on gmail", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }

            timer3.Stop();
            timer3.Start();
        }

        private void quitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void testToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new Login_Form.LoginForm().Show();
        }

        private void bR60GeneratedPGsToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            new BR60generated.BR60GeneratedLines_Form().Show();
        }

        private void onHandOnPOToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            new OnHandOnPo_Form.OnHandOnPO_Form().Show();
        }

        private void branchRebatesToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            new BranchRebates.BranchRebates_Form().Show();
        }

        private void fuse5ExportToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new Fuse5Export.Fuse5Export_Form().Show();
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

        private void queryBuilderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new QueryBuilder.QueryBuilder().Show();
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

        private void clearOpenwebsArchivesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int counter = 0;
            string strArchivePath = $@"\\svrsql\Users\ADodge";
            DialogResult dialogResult = MessageBox.Show($@"This will delete all files over 60 days old from the AUTOPART\Mail\Inbox folder and all subfolders. Do you want to continue?", "NOTICE", MessageBoxButtons.YesNo);
            if (dialogResult == DialogResult.Yes)
            {
                DirectoryInfo dir = new DirectoryInfo(strArchivePath);
                foreach (FileInfo file in dir.EnumerateFiles())
                {
                    var creationTime = file.CreationTime;
                    if (creationTime < DateTime.Now.Subtract(TimeSpan.FromDays(60)))
                    {
                        file.Delete();
                        counter += 1;
                    }
                }
                //foreach (dir.GetDirectories())
                //{
                //foreach (FileInfo file in dir.GetFiles("*"))
                //{
                //var creationTime = file.CreationTime;
                //if (creationTime < DateTime.Now.Subtract(TimeSpan.FromDays(60)))
                //{
                //file.Delete();
                //counter += 1;
                //}
                //}
                //}
            }
            else if (dialogResult == DialogResult.No)
            {
                return;
            }
            MessageBox.Show($@"Done! {counter} files removed.");
        }

        private void fillRateToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            new FillRate.FillRateReport().Show();
        }

        private void getReturnNoteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string pm = Interaction.InputBox("Enter the return note number", "Return Note");
            DialogResult dialogResult = MessageBox.Show("Exclude lines with rejected status?", "Exclude Rejects?", MessageBoxButtons.YesNo);

            this.Cursor = Cursors.WaitCursor;

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

        private void onSUOnBOToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new OnBOonSU.OnBOonPO().Show();
        }

        private void wixReportToolStripMenuItem_Click(object sender, EventArgs e)
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

        private void clearBulkLocManagementToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DialogResult dialogResult = MessageBox.Show("Do you wish to clear the param for Bulk Location Management?", "Notice", MessageBoxButtons.YesNo);
            if (dialogResult == DialogResult.Yes)
            {
                SqlConnection conn = new SqlConnection(Helper.ConnString("AUTOPART"));
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

        private void singlePartSearchToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new SinglePartSearch.SPS().Show();
        }

        private void suggestedOrderReviewToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new SOR.SOR().Show();
        }

        private void setStockToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new SetStock.SetStock().Show();
        }

        private void salesRankToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new SalesRank.SalesRank().Show();
        }

        private void calendarOrdersToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Query = ($@"
                SELECT * FROM vw_MERI_CalendarOrders ORDER BY Sunday DESC, Monday DESC, Tuesday DESC, Wednesday DESC, Thursday DESC, Friday DESC, Saturday DESC
            ");

            QueryResults.QueryResults qr = new QueryResults.QueryResults(Query);
            qr.Show();
        }

        private void getExpiringMNDatesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string startDate = Interaction.InputBox("Please enter a start Date (MM/DD/YYYY)", "MN Expire");
            string endDate = Interaction.InputBox("Please enter a end date ()MM/DD/YYYY", "MN Expire");

            Query = ($@"
                    SELECT 
                    Stock.Branch, 
                    Product.PG, 
                    COUNT(Stock.Part) AS SKUs 

                    FROM Stock (nolock) 

                    INNER JOIN Product (nolock) ON Stock.Part = Product.Keycode WHERE CAST(Stock.MNDate as date) BETWEEN '{startDate}' and '{endDate}' 
                    GROUP BY Stock.Branch, Product.PG 
                    ORDER BY COUNT(Stock.Part) DESC
                ");

            QueryResults.QueryResults qr = new QueryResults.QueryResults(Query);
            qr.Show();
        }

        private void getMinMaxParamsetersToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new MinMaxParams.MinMaxParams().Show();
        }

        private void autologueExportToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new AutologuePricing.AutologuePriceBuilder().Show();
        }

        private void proposedChangesFutureValueToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new FVBC.FVBC().Show();
        }

        private void getCostPriceRulesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            pg = Interaction.InputBox("Please enter a product group", "Select Product Group");

            if (pg == "")
            {
                MessageBox.Show("You must enter a product group!");
                return;
            }

            Query = ($@"
                SELECT * FROM vw_MERI_CostPriceRulesPG WHERE Left(Parts,3) = '{pg}' OR Substring(Parts,3,3) = '{pg}'
            ");

            QueryResults.QueryResults qr = new QueryResults.QueryResults(Query);
            qr.Show();
        }

        private void getUPCsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string strPGorRange = Interaction.InputBox("Please enter a PG or Range", "Enter PG or Range");

            this.Cursor = Cursors.WaitCursor;
            SqlConnection conn = new SqlConnection(Helper.ConnString("AUTOPART"));
            conn.Open();

            if (strPGorRange.Length == 3)
            {
                Query = ($@"
                    SELECT Part, UPC, Qty FROM vw_MERI_UPC WHERE PG = '{strPGorRange}'
                ");

                this.Cursor = Cursors.Default;
            }
            else
            {
                Query = ($@"
                    SELECT Part, UPC, Qty FROM vw_MERI_UPC WHERE Range = '{strPGorRange}'
                ");

                this.Cursor = Cursors.Default;
            }
            QueryResults.QueryResults qr = new QueryResults.QueryResults(Query);
            qr.Show();
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

        private void partValidatorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new PartValidator.IsPart().Show();
        }

        private void wMSSortToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string PG = Interaction.InputBox("Specify product group");

            if (PG == "")
            {
                MessageBox.Show("You must enter a product group!");
                return;
            }
            else
            {
                this.Cursor = Cursors.WaitCursor;
            }

            SqlConnection conn = new SqlConnection(Helper.ConnString("AUTOPART"));
            conn.Open();
            //DataSet ds1 = new DataSet();
            SqlDataAdapter adapter = new SqlDataAdapter(
            $@"
                    SELECT SubKey FROM MERI_WMSSort WHERE Prefix = 'L'
                   ", conn);
            adapter.Fill(wmsTable1);
            conn.Close();
            int items = wmsTable1.Rows.Count;
            if (items > 0)
            {
                string lockedBy = wmsTable1.Rows[0]["SubKey"].ToString();
                MessageBox.Show($@"Table is locked by {lockedBy}");
                this.Cursor = Cursors.Default;
                return;
            }


            SqlDataAdapter adapter1 = new SqlDataAdapter(
            $@"
                    SELECT a1 FROM MERI_WMSSort WHERE Prefix = 'P' AND SubKey = '{PG}'
                   ", conn);
            adapter1.Fill(wmsTable2);
            conn.Close();
            int items1 = wmsTable2.Rows.Count;
            if (items1 == 0)
            {
                MessageBox.Show($@"No sort settings found for Product Group {PG}");
                this.Cursor = Cursors.Default;
                return;
            }
            else
            {
                strSort = wmsTable2.Rows[0]["a1"].ToString();
            }


            SqlDataAdapter adapter2 = new SqlDataAdapter(
                $@"
                    SELECT a1, Seq1, Just1, Seq2, Just2, Seq3, Just3, Seq4, Just4, Seq5, Just5, Seq6, Just6 FROM MERI_WMSSort WHERE Prefix = 'S' AND SubKey = '{strSort}'
                   ", conn);
            adapter2.Fill(wmsTable3);
            conn.Close();
            int items2 = wmsTable3.Rows.Count;
            if (items2 == 0)
            {
                MessageBox.Show($@"No sort settings found for Product Group {PG}");
                this.Cursor = Cursors.Default;
                return;
            }
            else
            {
                strFirstGroup = wmsTable3.Rows[0]["a1"].ToString();
                strGroupOrder = wmsTable3.Rows[0]["Seq1"].ToString() + "," + wmsTable3.Rows[0]["Seq2"].ToString() + "," + wmsTable3.Rows[0]["Seq3"].ToString() + "," + wmsTable3.Rows[0]["Seq4"].ToString() + "," + wmsTable3.Rows[0]["Seq5"].ToString() + "," + wmsTable3.Rows[0]["Seq6"].ToString();
                strGroupPad = wmsTable3.Rows[0]["Just1"].ToString() + "," + wmsTable3.Rows[0]["Just2"].ToString() + "," + wmsTable3.Rows[0]["Just3"].ToString() + "," + wmsTable3.Rows[0]["Just4"].ToString() + "," + wmsTable3.Rows[0]["Just5"].ToString() + "," + wmsTable3.Rows[0]["Just6"].ToString();
            }


            using (SqlCommand cmd = new SqlCommand("sp_MERI_WMSSortLock", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                //set user lock
                cmd.Parameters.Add("@LockUser", SqlDbType.VarChar).Value = user;

                try
                {
                    conn.Open();
                    cmd.ExecuteNonQuery();
                    conn.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    this.Cursor = Cursors.Default;
                    return;
                }
            }


            using (SqlCommand cmd = new SqlCommand("sp_MERI_WMSSortClearWork", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                try
                {
                    conn.Open();
                    cmd.ExecuteNonQuery();
                    conn.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    this.Cursor = Cursors.Default;
                    return;
                }
            }


            using (SqlCommand cmd = new SqlCommand("sp_MERI_WMSSortPopluateWork", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@PG", SqlDbType.VarChar).Value = PG;

                try
                {
                    conn.Open();
                    cmd.ExecuteNonQuery();
                    conn.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    this.Cursor = Cursors.Default;
                    return;
                }
            }

            SqlDataAdapter adapter3 = new SqlDataAdapter(
                $@"
                    SELECT Part FROM MERI_WMSSortWorking
                   ", conn);
            adapter3.Fill(tblParts);

            for (int r = 0; r < tblParts.Rows.Count; r++)
            {
                using (SqlCommand cmd = new SqlCommand("sp_MERI_WMSSortUpdate", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@Part", SqlDbType.VarChar).Value = tblParts.Rows[r]["Part"].ToString();
                    cmd.Parameters.Add("@Sort", SqlDbType.VarChar).Value = SplitPartNo(tblParts.Rows[r]["Part"].ToString(), strFirstGroup, strGroupOrder, strGroupPad).ToString();

                    try
                    {
                        conn.Open();
                        cmd.ExecuteNonQuery();
                        conn.Close();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message, "Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        this.Cursor = Cursors.Default;
                        return;
                    }
                }
            }

            using (SqlCommand cmd = new SqlCommand("sp_MERI_WMSSortSequence", conn))
            {
                try
                {
                    conn.Open();
                    cmd.ExecuteNonQuery();
                    conn.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    this.Cursor = Cursors.Default;
                    return;
                }
            }

            using (SqlCommand cmd = new SqlCommand("sp_MERI_WMSSortUpdateProduct", conn))
            {
                try
                {
                    conn.Open();
                    cmd.ExecuteNonQuery();
                    conn.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    this.Cursor = Cursors.Default;
                    return;
                }
            }

            using (SqlCommand cmd = new SqlCommand("sp_MERI_WMSSortUnlock", conn))
            {
                try
                {
                    conn.Open();
                    cmd.ExecuteNonQuery();
                    conn.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    this.Cursor = Cursors.Default;
                    return;
                }
            }

            MessageBox.Show($@"Product group {PG} has been resequenced");
            this.Cursor = Cursors.Default;

        }

        public string SplitPartNo(string PartNumber, string FirstGroup, string GroupOrder, string GroupPad)
        {
            int x;
            int intA;
            int intN;
            string strPartNo;
            string strCh;
            String[] strAl = new String[9];
            String[] strNu = new String[9];
            string strSortKey;
            string[] strGroupOrder;
            string[] strGroupPad;

            strGroupOrder = GroupOrder.Split(',');
            strGroupPad = GroupPad.Split(',');
            int endOfPart = PartNumber.Length - 3;
            strPartNo = PartNumber.Substring(3, endOfPart);
            intA = 1;
            intN = 1;

            if (FirstGroup == "A" && Char.IsNumber(Convert.ToChar(strPartNo.Substring(1, 1))))
            {
                intA = 2;
            }

            if (FirstGroup == "N" && Char.IsLetter(Convert.ToChar(strPartNo.Substring(1, 1))))
            {
                intN = 2;
            }

            for (x = 1; x < strPartNo.Length; x++)
            {
                strCh = strPartNo.Substring(x, 1);
                if (strCh == "-")
                {
                    if (Convert.ToString(strAl.GetValue(intA)) != "")
                    {
                        intA++;
                    }
                    if (Convert.ToString(strNu.GetValue(intN)) != "")
                    {
                        intN++;
                    }
                }
                else
                {
                    if (char.IsNumber(Convert.ToChar(strCh)))
                    {
                        if (Convert.ToString(strAl.GetValue(intA)) != "")
                        {
                            intA++;
                            strNu.SetValue(strNu.GetValue(intN) + strCh, intN);
                        }
                    }
                    else
                    {
                        if (char.IsLetter(Convert.ToChar(strCh)))
                        {
                            if (Convert.ToString(strNu.GetValue(intN)) != "")
                            {
                                intN++;
                                strAl.SetValue(strAl.GetValue(intA) + strCh, intA);
                            }
                        }
                    }
                }
            }

            strSortKey = "X";
            for (x = 0; x < strGroupOrder.Length; x++)
            {
                if (Convert.ToString(strGroupOrder.GetValue(x)) == "1A")
                {
                    strSortKey = strSortKey + PadString(Convert.ToString(strAl.GetValue(1)), Convert.ToString(strGroupPad.GetValue(x)));
                }
                if (Convert.ToString(strGroupOrder.GetValue(x)) == "2A")
                {
                    strSortKey = strSortKey + PadString(Convert.ToString(strAl.GetValue(2)), Convert.ToString(strGroupPad.GetValue(x)));
                }
                if (Convert.ToString(strGroupOrder.GetValue(x)) == "3A")
                {
                    strSortKey = strSortKey + PadString(Convert.ToString(strAl.GetValue(3)), Convert.ToString(strGroupPad.GetValue(x)));
                }
                if (Convert.ToString(strGroupOrder.GetValue(x)) == "4A")
                {
                    strSortKey = strSortKey + PadString(Convert.ToString(strAl.GetValue(4)), Convert.ToString(strGroupPad.GetValue(x)));
                }
                if (Convert.ToString(strGroupOrder.GetValue(x)) == "1N")
                {
                    strSortKey = strSortKey + PadString(Convert.ToString(strNu.GetValue(1)), Convert.ToString(strGroupPad.GetValue(x)));
                }
                if (Convert.ToString(strGroupOrder.GetValue(x)) == "2N")
                {
                    strSortKey = strSortKey + PadString(Convert.ToString(strNu.GetValue(2)), Convert.ToString(strGroupPad.GetValue(x)));
                }
                if (Convert.ToString(strGroupOrder.GetValue(x)) == "3N")
                {
                    strSortKey = strSortKey + PadString(Convert.ToString(strNu.GetValue(3)), Convert.ToString(strGroupPad.GetValue(x)));
                }
                if (Convert.ToString(strGroupOrder.GetValue(x)) == "4N")
                {
                    strSortKey = strSortKey + PadString(Convert.ToString(strNu.GetValue(4)), Convert.ToString(strGroupPad.GetValue(x)));
                }
            }
            return strSplitPartNo = strSortKey;
        }

        public string PadString(string strInput, string strDirection)
        {
            if (strDirection == "R")
            {
                if (strInput.Length > 16)
                {
                    strPadString = strInput.Substring(0, 16);
                }
                else
                {
                    strPadString = strInput.PadLeft(16 - strInput.Length);
                }
            }
            else if (strDirection == "L")
            {
                if (strInput.Length > 16)
                {
                    strPadString = strInput.Substring(strInput.Length - 16, 16);
                }
                else
                {
                    strPadString = strInput.PadRight(16 - strInput.Length);
                }
            }
            else
            {
                strPadString = "";
            }
            return strPadString;
        }

        private void promoSalesP55SalesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new P55SalesCriteriaForm.P55SalesCriteriaForm().Show();
        }

        private void wMSSortToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            new wmsSort.wmsSort().Show();
        }

        private void leftVsRightFreeStockSearchToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new LeftvsRight.LeftvsRight().Show();
        }

        private void webSalesLast6MonthsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Query = (
        $@"
                    SELECT 
                    Branch, 
                    Acct, 
                    Name, 
                    CAST(ISNULL(SUM(Qty * Unit),0) as decimal(18,2)) as 'Web Sales' 

                    FROM VW_MERI_I NOLOCK 

                    WHERE WebOrder != '' AND 
                    CAST(DateTime as date) >= CAST(DATEADD(m, -6, GETDATE()) as date) AND 
                    ISNUMERIC(Acct) = 1 

                    GROUP BY 
                    Branch, 
                    Acct, 
                    Name 

                    ORDER BY 
                    Branch, 
                    LEN(Acct), 
                    Acct
                   ");

            QueryResults.QueryResults qr = new QueryResults.QueryResults(Query);
            qr.Show();
        }

        private void signedInvoicesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int counter = 0;
            string strSourceDir = $@"\\svropenwebs\pdf\scans\invoices";
            string strDestDir = $@"\\svropenwebs\pdf\scans";
            DirectoryInfo dirInfo = new DirectoryInfo(strSourceDir);

            string strAcct;
            string strBranch;
            string strDocument;
            SqlConnection conn = new SqlConnection(Helper.ConnString("AUTOPART"));
            int intQtyLimit = 0;

            // Quantity Limit if provided
            string QtyLimit = Interaction.InputBox("Maximum number of documents to process", "Please enter number of documents to process");
            if (QtyLimit == "")
            {
                DialogResult result = MessageBox.Show("Are you sure you want to move all PDF files in the folder?", "Verify", MessageBoxButtons.YesNo);
                if (result == DialogResult.No)
                {
                    return;
                }
            }

            FileInfo[] fileArray = dirInfo.GetFiles("*.pdf");
            //FileInfo PDFfile;
            foreach (FileInfo f in fileArray)
            {
                strDocument = f.Name.Substring(0, Strings.Len(f.Name) - 4);
                // Query for Branch and Acct
                SqlCommand BranchQuery = new SqlCommand($@"SELECT Branch FROM IHeads WITH (nolock) WHERE Prefix = 'I' AND Document = '{strDocument}'", conn);
                SqlCommand AcctQuery = new SqlCommand($@"SELECT Acct FROM IHeads WITH (nolock) WHERE Prefix = 'I' AND Document = '{strDocument}'", conn);
                conn.Open();
                strBranch = (string)BranchQuery.ExecuteScalar();
                strAcct = (string)AcctQuery.ExecuteScalar();
                conn.Close();
                if (strBranch != null)
                {
                    // Check for existance of destination
                    if (!Directory.Exists(Path.Combine(strDestDir, strBranch, strAcct, "INVOICES")))
                        Directory.CreateDirectory(Path.Combine(strDestDir, strBranch, strAcct, "INVOICES"));
                    // Move File
                    if (!File.Exists(Path.Combine(strDestDir, strBranch, strAcct, "INVOICES") + @"\" + strDocument + ".PDF"))
                    {
                        File.Move(Path.Combine(strSourceDir, strDocument + ".pdf"), Path.Combine(strDestDir, strBranch, strAcct, "INVOICES") + @"\" + strDocument + ".PDF");
                        counter += 1;
                    }
                    if (intQtyLimit > 0 & counter == intQtyLimit)
                        break;
                }
            }
            Interaction.MsgBox("Moved " + Convert.ToString(counter) + " files.", Constants.vbOKOnly, "Finished");
        }

        private void queryBuilderToolStripMenuItem_Click_1(object sender, EventArgs e)
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

        private void productDataTableToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new productDataImport.productDataForm().Show();
        }

        private void importIOTToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new IOT_Form.IOT_Form().Show();
        }

        private void minMaxRequestsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new MinMaxRequests.MinMaxRequests().Show();
        }

        private void getPGByPrimeSupplierToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string supplier = Interaction.InputBox("Please enter a supplier", "Enter supplier", "");
            if (supplier == "")
            {
                MessageBox.Show("You must specify a prime supplier");
                return;
            }
            else
            {
                String[] suppArray = supplier.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                supplier = "";
                foreach (string i in suppArray)
                {
                    supplier += $"'{i}',";
                }
                supplier = supplier.TrimEnd(',');
                //customer = "'" + customerTextbox.Text + "'";
                //customer = customer.Replace(",", "\",");
            }

            Query = $@"

                                SELECT DISTINCT PG FROM vw_MERI_Pro WHERE PSupp IN ({supplier}) ORDER BY PG

                        ";

            QueryResults.QueryResults qr = new QueryResults.QueryResults(Query);
            qr.Show();
        }

        private void getMonthlyUsageByBranchToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new getMonthlyUsageByBranch.getMonthlyUsageByBranch().Show();
        }

        private void getMonthlyUsageByBranchGroupToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new getMonthlyUsageByBranchGroup.getMonthlyUsageByBranchGroup().Show();
        }

        private void timesToZeroToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new TimesToZero_Form.timesToZero().Show();
        }

        private void productReclassificationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new ProductReclasification.ProductReclasification().Show();
        }

        private void historyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string pgRange = Interaction.InputBox("Please input a Product Group (PG) or Range", "Enter PG or Range", "");

            if (pgRange.Length == 3)
            {
                pg = "'" + pgRange + "'";
                range = "NULL";
            }
            else
            {
                range = "'" + pgRange + "'";
                pg = "NULL";
            }

            this.Cursor = Cursors.WaitCursor;
            SqlConnection conn = new SqlConnection(Helper.ConnString("AUTOPART"));
            conn.Open();


            string query = ($@"
                                DECLARE @PG VARCHAR(10)
                                DECLARE @Range VARCHAR(10)

                                SET @PG = {pg}
                                SET @Range = {range}
                                

                                SELECT * FROM MERI_ReclassHistory WHERE PG = IIF(@PG IS NULL, PG, @PG) AND Range = IIF(@Range IS NULL, Range, @Range)
                            ");
            QueryResults.QueryResults qr = new QueryResults.QueryResults(Query);
            qr.Show();
        }
    }
}
