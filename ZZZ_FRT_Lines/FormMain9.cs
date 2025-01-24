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
using System.Threading.Tasks;
using ZZZ_FRT_Lines;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.Windows.Forms.DataVisualization.Charting;
using System.Collections.Generic;
using OpenXmlPowerTools;
//using DocumentFormat.OpenXml.Office2010.PowerPoint;

//using System.Data.SqlClient;



namespace FormMain9
{
    public partial class FormMain9 : Form
    {
        public DataTable table = new DataTable();

        public DataTable dt = new DataTable();
        public DataTable dt1 = new DataTable();
        public DataTable dt2 = new DataTable();
        public DataTable dt3 = new DataTable();

        public DataTable wmsTable1 = new DataTable();
        public DataTable wmsTable2 = new DataTable();
        public DataTable wmsTable3 = new DataTable();
        public DataTable wmsTable4 = new DataTable();

        public DataTable tblParts = new DataTable();

        public string strSort;
        public string strFirstGroup;
        public string strGroupOrder;
        public string strGroupPad;
        public string items;
        public string strPadString;
        public string strSplitPartNo;
        public string Query;
        string var1;
        string pg;
        string range;


        public string sFileName;
        public string[] arrAllFiles;

        public string user = System.Security.Principal.WindowsIdentity.GetCurrent().Name.Replace(@"MERRILLCO\", "");
        public System.Timers.Timer timer1 = new System.Timers.Timer();
        public Timer timer0;
        public Timer timer2;
        public Timer timer3;
        public FormMain9()
        {
            InitializeComponent();
            this.CenterToScreen();
            this.Text = $@"Apparatus - {System.Security.Principal.WindowsIdentity.GetCurrent().Name}";
            MinMaxRequestsButton.Visible = false;
        }


        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            Environment.Exit(0);
        }

        private void FormMain9_Load(object sender, EventArgs e)
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
                    try { 
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

        private void monthEndTasksToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new PasswordForm.PasswordForm().Show();
        }

        private void clearOpenwebsArchivesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Cursor = Cursors.WaitCursor;
            int counter = 0;
            string strArchivePath = $@"\\svrsql1\AUTOPART\Mail\Inbox";
            DialogResult dialogResult = MessageBox.Show($@"This will delete all files over 60 days old from the AUTOPART\Mail\Inbox folder and all subfolders. Do you want to continue?", "NOTICE", MessageBoxButtons.YesNo);
            if (dialogResult == DialogResult.Yes)
            {
                DirectoryInfo di = new DirectoryInfo(strArchivePath);
                foreach (FileInfo file in di.EnumerateFiles())
                {
                    var creationTime = file.LastWriteTime;
                    if (creationTime < DateTime.Now.Subtract(TimeSpan.FromDays(60)))
                    {
                        file.Delete();
                        counter += 1;
                    }
                }
                foreach (DirectoryInfo dir in di.EnumerateDirectories())
                {
                    foreach (FileInfo file in dir.GetFiles("*"))
                    {
                        var creationTime = file.LastWriteTime;
                        if (creationTime < DateTime.Now.Subtract(TimeSpan.FromDays(60)))
                        {
                            file.Delete();
                            counter += 1;
                        }
                    }
                }
            }
            else if (dialogResult == DialogResult.No)
            {
                return;
            }
            this.Cursor = Cursors.Default;
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

            table.Clear();

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

        private void onPOOrBOToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new OnBOonSU.OnBOonPO().Show();
        }

        private void autologueExportToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new AutologuePricing.AutologuePriceBuilder().Show();
        }

        private void futureValuesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new FVBC.FVBC().Show();
        }

        private void lAstReceivedToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new LastReceived.LastReceived().Show();
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

        private void fVBCToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new FVBC.FVBC().Show();
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

        private void setStockToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new SetStock.SetStock().Show();
        }

        private void clearLockedStockSheetsToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            SqlConnection conn = new SqlConnection(Helper.ConnString("AUTOPART"));

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

        private void calendarOrdersToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Query = ($@"
                SELECT * FROM vw_MERI_CalendarOrders ORDER BY Sunday DESC, Monday DESC, Tuesday DESC, Wednesday DESC, Thursday DESC, Friday DESC, Saturday DESC
            ");

            QueryResults.QueryResults qr = new QueryResults.QueryResults(Query);
            qr.Show();
        }

        private void getMinMaxParametersToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new MinMaxParams.MinMaxParams().Show();
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

        private void productDataTableToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            new productDataImport.productDataForm().Show();
        }

        private void stockLevellingToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new StockLevelling_New.StockLevelling_New().Show();
        }

        private void salesRankToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new SalesRank.SalesRank().Show();
        }

        private void iOTbetaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new SinglePartSearch.SPS().Show();
        }

        private void suggestedOrderReviewToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new SOR.SOR().Show();
        }

        private void importIOTToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new importIOT.importIOT().Show();
        }

        private void productReclasificationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new ProductReclasification.ProductReclasification().Show();
        }

        private void minMaxRequestsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new MinMaxRequests.MinMaxRequests().Show();
        }

        private void partValidatorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new PartValidator.IsPart().Show();
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

        private void stockLevellingnewBetaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new StockLevelling_New.StockLevelling_New().Show();
        }

        private void MinMaxRequestsButton_Click(object sender, EventArgs e)
        {
            new MinMaxRequests.MinMaxRequests().Show();
        }

        private void minMaxAttendantToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new MinMaxAttendant.MinMaxAttendant().Show();
        }

        private void wMSSortToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new wmsSort.wmsSort().Show();
        }

        private void promoSalesP55SalesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new P55SalesCriteriaForm.P55SalesCriteriaForm().Show();
        }

        private void countSoldToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new CountSold.CountSold().Show();
        }

        private void getSpecialTermsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new getSpecialTerms.getSpecialTerms().Show();
        }

        private void stockValuesMonthlyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new StkValMonthly.StkValMonthly().Show();
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

        private void getNonNumericHazMarWeightToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string Query = $@"

                            SELECT part, value from prodvals WITH(nolock) WHERE attribute ='HazMat Weight (96)' AND value like '%[A-Z]%'

                        ";

            QueryResults.QueryResults qr = new QueryResults.QueryResults(Query);
            qr.Show();
        }

        private void deleteSpecialTermsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new DeleteSpecialTerms.DeleteSpecialTerms().Show();
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

        private void multiPOToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new Multi_PO.Multi_PO().Show();
        }

        private void averageSoldPerInvoiceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string PG = Interaction.InputBox("Please specify a product group", "Specify PG");
            string FromDate = Interaction.InputBox("Please specify a start date", "Start Date", "MM/DD/YYYY");

            string Query = $@"

                            WITH CTE(Part, Description, QtySold, Invoices, AvgInvoice) as (
                            SELECT Part, Description, CAST(SUM(Qty) as int) AS 'QTY SOLD', COUNT(Document) AS INVOICES, CAST((SUM(Qty) / COUNT(Document)) as decimal(18,2)) AS 'AVG INV'
                            FROM vw_MERI_I 
                            WHERE PG = '{PG}'  AND LType != 'NONE' AND Qty > 0 AND Document LIKE '%NV%' AND CAST(Datetime AS DATE) >= '{FromDate}'
                            GROUP BY Part, Description
                            ),

                            CTE2 (Part, Invoices) as (
                                SELECT Part, COUNT(Document)
                                FROM vw_MERI_I 
                                WHERE PG = '{PG}'  AND LType != 'NONE' AND Qty > 1 AND Document LIKE '%NV%' AND CAST(Datetime AS DATE) >= '{FromDate}'
                                GROUP BY Part
                            )

                            SELECT I.Part, I.Description, I.QtySold, I.Invoices, I.AvgInvoice, ISNULL(II.Invoices,0) as 'InvW/Qty>1' FROM CTE as I LEFT OUTER JOIN CTE2 as II ON I.Part = II.Part
                            ORDER BY I.Part

                        ";

            QueryResults.QueryResults qr = new QueryResults.QueryResults(Query);
            qr.Show();
        }

        private void clearSVRFILEArchivesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Cursor = Cursors.WaitCursor;
            int counter = 0;
            string strArchivePath = $@"\\svrfile\Scans\";
            DialogResult dialogResult = MessageBox.Show($@"This will delete all files over 300 days old from the svrfile\Scans\ folder and all subfolders. Do you want to continue?", "NOTICE", MessageBoxButtons.YesNo);
            if (dialogResult == DialogResult.Yes)
            {
                DirectoryInfo di = new DirectoryInfo(strArchivePath);
                foreach (FileInfo file in di.EnumerateFiles())
                {
                    var creationTime = file.LastWriteTime;
                    if (creationTime < DateTime.Now.Subtract(TimeSpan.FromDays(300)))
                    {
                        file.Delete();
                        counter += 1;
                    }
                }
                foreach (DirectoryInfo dir in di.EnumerateDirectories())
                {
                    foreach (FileInfo file in dir.GetFiles("*"))
                    {
                        var creationTime = file.LastWriteTime;
                        if (creationTime < DateTime.Now.Subtract(TimeSpan.FromDays(300)))
                        {
                            file.Delete();
                            counter += 1;
                        }
                    }
                }
            }
            else if (dialogResult == DialogResult.No)
            {
                return;
            }
            this.Cursor = Cursors.Default;
            MessageBox.Show($@"Done! {counter} files removed.");
        }

        private void signedInvoiceCopiesToolStripMenuItem_Click(object sender, EventArgs e)
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

        private void branchTransfersToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new BranchTransfer.BranchTransfer().Show();
        }

        private void nominalSummaryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string Period1 = Interaction.InputBox("Please enter beginning period", "Start Period", "YYYYMM");
            string Period2 = Interaction.InputBox("Please enter ending period", "End Period", "YYYYMM");

            string Query = $@"

                        SELECT NominalCode, Period, [Value] FROM vw_MERI_PayrollGL WHERE Period >= {Period1} AND Period <= {Period2} ORDER BY NominalCode, Period

                        ";

            QueryResults.QueryResults qr = new QueryResults.QueryResults(Query);
            qr.Show();
        }

        private void replaceSalesRepToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SqlConnection conn = new SqlConnection(Helper.ConnString("AUTOPART"));

            string strOldRep = Interaction.InputBox("Old Sales Rep Operator Code", "Old Rep");
            string strNewRep = Interaction.InputBox("New Sales Rep Operator Code", "New Rep");

            using (SqlCommand cmd = new SqlCommand("sp_MERI_ReplaceSalesRep", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                //set new Min
                cmd.Parameters.Add("OldRep", SqlDbType.VarChar).Value = strOldRep;

                //set new Max
                cmd.Parameters.Add("@NewRep", SqlDbType.VarChar).Value = strNewRep;

                DialogResult dialogResult = MessageBox.Show($@"You are about to replace EVERY instance of {strOldRep} with {strNewRep}. Do you want to continue?", "Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (dialogResult == DialogResult.Yes)
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
                        return;
                    }
                }

                else if (dialogResult == DialogResult.No)
                {
                    return;
                }
            }
            MessageBox.Show("Done!");
        }

        private void replaceRepByCustomerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DataTable dt = new DataTable();
            string filePath;
            string[] lines;

            DialogResult dialogResult = MessageBox.Show("This task assumes your file does NOT have headers and that the cutsomer number is in the first column, and the new sales rep is in the second column. Does your file meet these requirements?", "Notice!", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if(dialogResult == DialogResult.Yes)
            {
                OpenFileDialog OFD1 = new OpenFileDialog();
                OFD1.Filter = "csv files (*.csv)|*.csv";
                OFD1.ShowDialog();
                filePath = OFD1.FileName;

                //int i = 1;
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

                    foreach (string headerWord in headerLabels)
                    {
                        dt.Columns.Add(new DataColumn(headerWord));
                    }

                    // Get cell data
                    for (int r = 0; r < lines.Length; r++)
                    {
                        string[] dataWords = lines[r].Split(',');
                        DataRow dr = dt.NewRow();
                        int columnIndex = 0;
                        foreach (string headerWord in headerLabels)
                        {
                            dr[headerWord] = dataWords[columnIndex++];
                        }
                        dt.Rows.Add(dr);
                    }
                }
                this.Cursor = Cursors.WaitCursor;
                foreach (DataRow row in dt.Rows)
                {
                    //if (i != dt.Rows.Count - 1)
                    //{
                        SqlConnection conn = new SqlConnection(Helper.ConnString("AUTOPART"));

                        string strAcct = row[0].ToString();
                        string strNewRep = row[1].ToString();

                        using (SqlCommand cmd = new SqlCommand("sp_MERI_SetSalesRep", conn))
                        {
                            cmd.CommandType = CommandType.StoredProcedure;
                            //set new Customer Param
                            cmd.Parameters.Add("Customer", SqlDbType.VarChar).Value = strAcct;

                            //set Rep Param
                            cmd.Parameters.Add("@Rep", SqlDbType.VarChar).Value = strNewRep;

                            try
                            {
                                conn.Open();
                                cmd.ExecuteNonQuery();
                                conn.Close();
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show(ex.Message, "Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                return;
                            }
                        }
                    //}
                    //i++;
                }
                this.Cursor = Cursors.Default;
                MessageBox.Show("Task Complete!", "Notice", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else if(dialogResult == DialogResult.No)
            {
                MessageBox.Show("Please make the necessary changes to your file and try this task again.","Notice",MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
        }

        private void employeeHSAContributionsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string Period1 = Interaction.InputBox("Please enter beginning period", "Start Period", "YYYYMM");
            string Period2 = Interaction.InputBox("Please enter ending period", "End Period", "YYYYMM");

            string Query = $@"

                        SELECT NominalCode, Period, SUM(CASE when Sign = 'C' then -1*Value else Value END) AS Value
                        FROM Nledger
                        WHERE Period >= {Period1} AND Period <= {Period2} AND (NominalCode LIKE '50080-%') GROUP BY [Period], NominalCode
                        ORDER BY NominalCode, Period

                        ";

            QueryResults.QueryResults qr = new QueryResults.QueryResults(Query);
            qr.Show();
        }

        private void customerSales12000YearToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string Period1 = Interaction.InputBox("Please enter the Year", "Select Year", "YYYY");
            string DollarAmmount = Interaction.InputBox("Please enter the dollar amount in which the customer needs to have spent", "Select amount", "2000");
            string Query = $@"

                        SELECT Branch, Rep, Acct, Name, SUM(Unit * Qty) as Ext FROM VW_MERI_I 
                        WHERE YEAR(DateTime) = '{Period1}' AND LType != 'NONE'
                        GROUP BY Branch, Acct, Rep, Name
                        HAVING SUM(Unit*Qty) >= {DollarAmmount}
                        ORDER BY Branch, Rep, Acct

                        ";

            QueryResults.QueryResults qr = new QueryResults.QueryResults(Query);
            qr.Show();
        }

        private void importStockingLinesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DataTable dt = new DataTable();
            //string path = @"C:\Users\anand\Desktop\Book1.csv";
            string username = System.Security.Principal.WindowsIdentity.GetCurrent().Name;

            SqlConnection conn = new SqlConnection(Helper.ConnString("AUTOPART"));

            OpenFileDialog OFD = new OpenFileDialog();
            OFD.Filter = "csv files (*.csv)|*.csv";
            OFD.FilterIndex = 1;
            OFD.Multiselect = true;

            if (OFD.ShowDialog() == DialogResult.OK)
            {
                sFileName = OFD.FileName;
                arrAllFiles = OFD.FileNames; //used when Multiselect = true           
            }


            string csvData;
            using (StreamReader sr = new StreamReader(sFileName))
            {
                csvData = sr.ReadToEnd().ToString();
                string[] row = csvData.Split('\n');
                for (int i = 0; i < row.Count() - 1; i++)
                {
                    string[] rowData = row[i].Split(',');
                    {
                        if (i == 0)
                        {
                            for (int j = 0; j < rowData.Count(); j++)
                            {
                                dt.Columns.Add(rowData[j].Trim());
                            }
                        }
                        else
                        {
                            DataRow dr = dt.NewRow();
                            for (int k = 0; k < rowData.Count(); k++)
                            {
                                dr[k] = rowData[k].Trim().ToString();
                            }
                            dt.Rows.Add(dr);
                        }
                    }
                }
            }

            using (SqlConnection dbConnection = new SqlConnection(Helper.ConnString("AUTOPART")))
            {
                dbConnection.Open();

                SqlCommand clearTable = new SqlCommand($@"
                                                IF EXISTS (SELECT * FROM MERI_StockingLines_Import) 
                                                BEGIN
                                                    DELETE FROM MERI_StockingLines_Import
                                                END
                                    ", dbConnection);
                clearTable.CommandTimeout = 3000000;
                clearTable.ExecuteNonQuery();
                try
                {
                    using (SqlBulkCopy s = new SqlBulkCopy(dbConnection))
                    {
                        s.DestinationTableName = "MERI_StockingLines_Import";
                        //foreach (DataColumn column in dt.Columns)
                        //    s.ColumnMappings.Add(column.ToString(), column.ToString());
                        s.WriteToServer(dt);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }

            foreach (DataRow row in dt.Rows)
            {
                var part = row["Part"].ToString();
                {
                    conn.Open();
                    //DataSet ds1 = new DataSet();

                    SqlDataAdapter adapter = new SqlDataAdapter(
                    $@"
                                    SELECT 
                                        P.keycode 
                                    FROM Product P WHERE P.KeyCode = '{part}'
                                   ", conn);
                    adapter.Fill(dt1);
                    if(dt1.Rows.Count == 0)
                    {
                        MessageBox.Show($@"Part number {part} is not a valid part number!");
                        return;
                    }
                    dt1.Clear();
                    conn.Close();
                }
            }

            using (SqlCommand cmd = new SqlCommand("sp_MERI_ImportStockingLines", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                //set new Username
                string User = username.Substring(10, 5).ToUpper();
                cmd.Parameters.Add("@User", SqlDbType.VarChar).Value = User;


                try
                {
                    conn.Open();
                    cmd.ExecuteNonQuery();
                    conn.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }
            MessageBox.Show("Done!");

        }

        private void customerSalesYearToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string months = Interaction.InputBox("How many months back should I look?", "enter the number of months", "12");
            string DollarAmmount = Interaction.InputBox("Please enter the dollar amount in which the customer needs to have spent", "Select amount", "2000");
            string Query = $@"

                        SELECT        vw_MERI_I.Branch, vw_MERI_I.Acct, vw_MERI_I.Name, Customer.SCont, Customer.Stel, vw_MERI_I.Rep, Customer.Addra, Customer.Addrb, Customer.Addrc, Customer.Addrd, Customer.Addre, Customer.MotDueSort, 
                                                 Customer.PCode, SUM(vw_MERI_I.Unit * vw_MERI_I.Qty) AS Ext
                        FROM            vw_MERI_I INNER JOIN
                                                 Customer ON vw_MERI_I.Acct = Customer.KeyCode
                        WHERE        (vw_MERI_I.DateTime >= DATEADD(month, DATEDIFF(month, 0, GETDATE()) - {months}, 0)) AND (vw_MERI_I.Ltype <> 'NONE')
                        GROUP BY vw_MERI_I.Branch, vw_MERI_I.Acct, vw_MERI_I.Rep, vw_MERI_I.Name, Customer.Addra, Customer.Addrb, Customer.Addrc, Customer.Addrd, Customer.Addre, Customer.PCode, Customer.MotDueSort, Customer.Stel, 
                                                 Customer.SCont
                        HAVING        (SUM(vw_MERI_I.Unit * vw_MERI_I.Qty) >= {DollarAmmount})
                        ORDER BY vw_MERI_I.Branch, vw_MERI_I.Rep, vw_MERI_I.Acct

                        ";

            QueryResults.QueryResults qr = new QueryResults.QueryResults(Query);
            qr.Show();
        }

        private void kitMaintToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new kitCreator.kitCreator().Show();
        }

        private void branchRebatesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new BranchRebates.BranchRebates_Form().Show();
        }

        private void uploadKitToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            this.Cursor = Cursors.WaitCursor;

            string username1 = System.Security.Principal.WindowsIdentity.GetCurrent().Name;
            int SLength = username1.Length;
            string username = username1.Substring(10, (username1.Length - 10));

            string subPath = @"\\svrfile\shares\121Files\Kit_Files\"; // Your code goes here

            bool exists = Directory.Exists(subPath);

            if (!exists)
            {
                Directory.CreateDirectory(subPath);
            }



            checkForFile();
            SqlConnection conn = new SqlConnection(Helper.ConnString("AUTOPART"));
            string outputFilePath = @"\\svrfile\shares\121Files\Kit_Files\full_products.csv";

            try
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand("sp_MERI_ExportKits", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        // Get column names and write them as the first row
                        using (StreamWriter writer = new StreamWriter(outputFilePath))
                        {
                            for (int i = 0; i < reader.FieldCount; i++)
                            {
                                writer.Write(reader.GetName(i));
                                if (i < reader.FieldCount - 1)
                                {
                                    writer.Write(",");
                                }
                            }
                            writer.WriteLine();


                            // Write data rows
                            while (reader.Read())
                            {
                                for (int i = 0; i < reader.FieldCount; i++)
                                {
                                    string data = reader[i].ToString();
                                    data = data.TrimStart(); // Remove leading white space

                                    // Adding quotes to the data
                                    writer.Write("\"" + data + "\"");

                                    if (i < reader.FieldCount - 1)
                                    {
                                        writer.Write(",");
                                    }
                                }
                                writer.WriteLine();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }

           //upload file to SFTP

            this.Cursor = Cursors.WaitCursor;

            string host = "216.81.131.225";
            string ftpUsername = "121user"; // Replace with your FTP username
            string ftpPassword = "121user"; // Replace with your FTP password
            string remoteDirectory = "/home/121user/armscripts/files/";
            string localFilePath = @"\\svrfile\shares\121Files\Kit_Files\full_products.csv";

            using (var client = new SftpClient(host, ftpUsername, ftpPassword))
            {
                client.Connect();

                if (client.IsConnected)
                {
                    //Console.WriteLine("Connected to SFTP server.");

                    // Upload the file
                    UploadFile(client, localFilePath, remoteDirectory);

                    client.Disconnect();
                    //Console.WriteLine("Disconnected from SFTP server.");
                }
                else
                {
                    Console.WriteLine("Failed to connect to SFTP server.");
                }
            }

            using (SqlConnection dbConnection = new SqlConnection(Helper.ConnString("AUTOPART")))
            {
                dbConnection.Open();

                SqlCommand clearTable = new SqlCommand($@"
                                        DELETE FROM MERI_NAP_KitsToUpdate
                                    ", dbConnection);
                clearTable.CommandTimeout = 3000000;
                clearTable.ExecuteNonQuery();
            }


            this.Cursor = Cursors.Default;

        }

        static void UploadFile(SftpClient client, string localFilePath, string remoteDirectory)
        {
            var localFileName = Path.GetFileName(localFilePath);
            var remoteFilePath = $"{remoteDirectory}/{localFileName}";

            using (var fileStream = File.OpenRead(localFilePath))
            {
                client.UploadFile(fileStream, remoteFilePath);
                Console.WriteLine($"File '{localFileName}' uploaded to '{remoteFilePath}'.");
            }
        }

        private void checkForFile()
        {
            // FTP server details
            string ftpServerIP = "ftp://216.81.131.225/"; // Replace with your FTP server IP
            string ftpUsername = "121user"; // Replace with your FTP username
            string ftpPassword = "121user"; // Replace with your FTP password
            string directoryPath = "/home/121user/armscripts/files/"; // Replace with the path to the FTP directory

            FtpWebRequest request = (FtpWebRequest)WebRequest.Create(ftpServerIP + directoryPath);
            request.Credentials = new NetworkCredential(ftpUsername, ftpPassword);
            request.Method = WebRequestMethods.Ftp.ListDirectory;

            using (FtpWebResponse response = (FtpWebResponse)request.GetResponse())
            {
                using (Stream responseStream = response.GetResponseStream())
                {
                    using (StreamReader reader = new StreamReader(responseStream))
                    {
                        while (!reader.EndOfStream)
                        {
                            string fileName = reader.ReadLine();
                            if (fileName.Contains("temp"))
                            {
                                Console.WriteLine("There is already a file being processed. Please wait and try again.");
                                return;
                            }
                        }
                    }
                }
            }
        }

        private void uploadKitFullToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Cursor = Cursors.WaitCursor;

            string username1 = System.Security.Principal.WindowsIdentity.GetCurrent().Name;
            int SLength = username1.Length;
            string username = username1.Substring(10, (username1.Length - 10));

            string subPath = @"\\svrfile\shares\121Files\Kit_Files\121Files\"; 

            bool exists = Directory.Exists(subPath);

            if (!exists)
            {
                Directory.CreateDirectory(subPath);
            }


            checkForFile();
            SqlConnection conn = new SqlConnection(Helper.ConnString("AUTOPART"));
            string outputFilePath = @"\\svrfile\shares\121Files\Kit_Files\full_products.csv";

            try
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand("sp_MERI_ExportKits_Full", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        // Get column names and write them as the first row
                        using (StreamWriter writer = new StreamWriter(outputFilePath))
                        {
                            // Write data rows
                            for (int i = 0; i < reader.FieldCount; i++)
                            {
                                writer.Write(reader.GetName(i));
                                if (i < reader.FieldCount - 1)
                                {
                                    writer.Write(",");
                                }
                            }
                            writer.WriteLine();


                            // Write data rows
                            while (reader.Read())
                            {
                                for (int i = 0; i < reader.FieldCount; i++)
                                {
                                    string data = reader[i].ToString();
                                    data = data.TrimStart(); // Remove leading white space

                                    // Adding quotes to the data
                                    writer.Write("\"" + data + "\"");

                                    if (i < reader.FieldCount - 1)
                                    {
                                        writer.Write(",");
                                    }
                                }
                                writer.WriteLine();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }

            string host = "216.81.131.225";
            string ftpUsername = "121user"; // Replace with your FTP username
            string ftpPassword = "121user"; // Replace with your FTP password
            string remoteDirectory = "/home/121user/armscripts/files/";
            string localFilePath = @"\\svrfile\shares\121Files\Kit_Files\full_products.csv";

            using (var client = new SftpClient(host, ftpUsername, ftpPassword))
            {
                client.Connect();

                if (client.IsConnected)
                {
                    //Console.WriteLine("Connected to SFTP server.");

                    // Upload the file
                    UploadFile(client, localFilePath, remoteDirectory);

                    client.Disconnect();
                    //Console.WriteLine("Disconnected from SFTP server.");
                }
                else
                {
                    Console.WriteLine("Failed to connect to SFTP server.");
                }
            }



            this.Cursor = Cursors.Default;
        }

        private void productFileMaintKitsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new PFM.PFM().Show();
        }

        private void sellAKitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new ZZZ_FRT_Lines.KitCustomizer().Show();
        }

        private void timesToZeroToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new TimesToZero_Form.timesToZero().Show();
        }

        private void statementDestinationsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string Query = $@"

                        SELECT C.Area as Branch, COUNT(C.KeyCode) AS CountAccts
                        FROM  Customer C INNER JOIN
                                 DocumentDest D ON C.KeyCode = D.Keycode
                        WHERE D.Document LIKE 'STMT%'
                        GROUP BY C.Area
                        ORDER BY C.Area
                        ";

            QueryResults.QueryResults qr = new QueryResults.QueryResults(Query);
            qr.Show();
        }

        private void updateKitPricesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            massUpdateKitPrice massUpdateKitPrice = new massUpdateKitPrice();
            massUpdateKitPrice.Show();
        }

        private void salespersonRankingReportToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string MaxPeriodMonth;
            string MaxPeriodYear;
            string StartLY;
            string EndLY;
            string StartTY;
            string EndTY;
            DataTable dt = new DataTable();

            this.Cursor = Cursors.WaitCursor;

            SqlConnection conn = new SqlConnection(Helper.ConnString("ArnoldGroup2"));

            SqlCommand currentYear = new SqlCommand($@"SELECT MAX(PeriodYear) FROM SalesMaster", conn);
            conn.Open();
            MaxPeriodYear = Convert.ToString(currentYear.ExecuteScalar());
            if (MaxPeriodYear == string.Empty || MaxPeriodYear == "")
            {
                MessageBox.Show("Could not retrieve the latest period year!");
                return;
            }
            StartTY = MaxPeriodYear + "01";
            StartLY = (Convert.ToInt32(MaxPeriodYear) - 1).ToString() + "01";
            conn.Close();

            SqlCommand currentMonth = new SqlCommand($@"SELECT MAX(PeriodMonth) FROM SalesMaster WHERE PeriodYear = '{MaxPeriodYear}'", conn);
            conn.Open();
            MaxPeriodMonth = Convert.ToString(currentMonth.ExecuteScalar());
            if (MaxPeriodMonth == string.Empty || MaxPeriodMonth == "")
            {
                MessageBox.Show("Could not retrieve the latest period month!");
                return;
            }
            if (MaxPeriodMonth.Length == 1)
            {
                MaxPeriodMonth = "0" + MaxPeriodMonth;
            }
            conn.Close();
            EndTY = MaxPeriodYear + MaxPeriodMonth;
            EndLY = (Convert.ToInt32(MaxPeriodYear) - 1).ToString() + MaxPeriodMonth;


            Query = ($@"
                DECLARE @MaxPeriodMonth as VARCHAR(2) 
                DECLARE @MaxPeriodYear as VARCHAR(4) 
                DECLARE @StartLY as int 
                DECLARE @EndLY as int 
                DECLARE @StartTY as int 
                DECLARE @EndTY as int 

                SET @MaxPeriodMonth = ('{MaxPeriodMonth}') 
                SET @MaxPeriodYear = ('{MaxPeriodYear}') 
                SET @StartLY = ('{StartLY}') 
                SET @EndLY = ('{EndLY}') 
                SET @StartTY = ('{StartTY}') 
                SET @EndTY = ('{EndTY}') 

                ;WITH CTE (Branch, SalesRep, RepName, LYTD, YTD, ThisFiscalMonthLY, ThisFiscalMonth) AS (
                SELECT CM.Loc AS Branch, 
                CM.Salesperson as SalesRep, 
                SPM.FullName as RepName, 
                SUM(CASE WHEN FiscalMonth >= @StartLY AND FiscalMonth <= @EndLY THEN (NetSales + CoreSales) ELSE 0 END) AS LYTD, 
                SUM(CASE WHEN FiscalMonth >= @StartTY AND FiscalMonth <= @EndTY THEN (NetSales + CoreSales) ELSE 0 END) AS YTD, 
                SUM(CASE WHEN FiscalMonth = @EndLY THEN (NetSales + CoreSales) ELSE 0 END) AS 'ThisFiscalMonthLY', 
                SUM(CASE WHEN FiscalMonth = @EndTY THEN (NetSales + CoreSales) ELSE 0 END) AS 'ThisFiscalMonth' 

                FROM SalesMaster INNER JOIN 
                CustomerMaster AS CM ON SalesMaster.CustNo = CM.CustNo INNER JOIN 
                SalespersonMaster AS SPM ON CM.Salesperson = SPM.Salesperson 

                WHERE (CM.Salesperson NOT LIKE 'BR%') AND 
                (CM.Salesperson NOT LIKE '%STO') AND 
                (CM.SalesPerson NOT LIKE '%TERR') AND 
                (SPM.FullName NOT LIKE '%inactive%') 

                GROUP BY CM.Loc, CM.Salesperson, SPM.FullName) 

                SELECT 
                '' as 'ID',
                Branch, 
                SalesRep, 
                RepName, 
                CONVERT(varchar,CAST(LYTD as money),1) as LYTD, 
                CONVERT(varchar,CAST(YTD as money),1) as YTD, 
                CONVERT(varchar,CAST(YTD - LYTD as money),1) as CHANGE, 
                '' as '% UP OR DOWN', 
                CONVERT(varchar,CAST(ThisFiscalMonthLY as money),1) as ThisFiscalMonthLY, 
                CONVERT(varchar,CAST(ThisFiscalMonth as money),1) as ThisFiscalMonth, 
                CONVERT(varchar,CAST(ThisFiscalMonth - ThisFiscalMonthLY as money),1) as CHANGE, 
                '' as '% UP OR DOWN' 

                FROM CTE 
                ORDER BY YTD - LYTD DESC

                            ");

            SqlDataAdapter adptr1 = new SqlDataAdapter(Query, conn);
            adptr1.Fill(dt);
            if (dt.Rows.Count == 0)
            {
                MessageBox.Show("There was a problem retrieving the latest sales information!");
                return;
            }
            int rowindex = 1;
            foreach (DataRow row in dt.Rows)
            {
                double.TryParse(row[5].ToString(), out double value1);
                double.TryParse(row[4].ToString(), out double value2);
                double.TryParse(row[9].ToString(), out double value3);
                double.TryParse(row[8].ToString(), out double value4);
                row[7] = (((Convert.ToDouble(value1) - Convert.ToDouble(value2)) / Convert.ToDouble(value2)) * 100).ToString("0.00") + "%";
                row[11] = (((Convert.ToDouble(value3) - Convert.ToDouble(value4)) / Convert.ToDouble(value4)) * 100).ToString("0.00") + "%";
                row[0] = rowindex;
                rowindex++;
            }



            string username1 = System.Security.Principal.WindowsIdentity.GetCurrent().Name;
            int SLength = username1.Length;
            string username = username1.Substring(10, (username1.Length - 10));

            // Specify the path where the PDF will be saved
            string pdfPath = @"C:\Users\" + username + @"\Documents\SalespersonRankingReport" + DateTime.Now.ToString("yyyyMM") + ".pfd";

            // Create PDF document
            Document document = new Document(PageSize.A4, 10, 10, 10, 10);
            PdfWriter.GetInstance(document, new FileStream(pdfPath, FileMode.Create));
            document.Open();

            // Add header
            Font headerFont = FontFactory.GetFont(FontFactory.TIMES_BOLD, 16);
            Paragraph header = new Paragraph("Salesperson Ranking Report", headerFont);
            header.Alignment = Element.ALIGN_LEFT;
            document.Add(header);

            // Add date
            Font dateFont = FontFactory.GetFont(FontFactory.TIMES, 12);
            Paragraph date = new Paragraph(DateTime.Now.ToString("MMMM dd, yyyy"), dateFont);
            date.Alignment = Element.ALIGN_LEFT;
            document.Add(date);

            // Add some space
            document.Add(new Paragraph("\n"));

            // Create PDF table with the same number of columns as the DataTable
            PdfPTable pdfTable = new PdfPTable(dt.Columns.Count);

            // Calculate column widths
            float[] columnWidths = new float[] { 1f, 2.25f, 3f, 5.75f, 3.33f, 3.33f, 3.5f, 3f, 3.33f, 3.33f, 3.33f, 3.5f };
            //for (int i = 0; i < dt.Columns.Count; i++)
            //{
            //    columnWidths[i] = 1f; // Set default width for each column
            //}

            pdfTable.SetWidths(columnWidths);
            pdfTable.WidthPercentage = 100; // Set table width to 100% of page width

            // Set font for table content
            Font tableFont = FontFactory.GetFont(FontFactory.TIMES, 8);

            // Add table headers
            foreach (DataColumn column in dt.Columns)
            {
                PdfPCell cell = new PdfPCell(new Phrase(column.ColumnName, tableFont));
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                pdfTable.AddCell(cell);
            }

            // Add table rows
            foreach (DataRow row in dt.Rows)
            {
                foreach (var item in row.ItemArray)
                {
                    PdfPCell cell = new PdfPCell(new Phrase(item.ToString(), tableFont));
                    cell.NoWrap = true; // Ensure text does not wrap
                    pdfTable.AddCell(cell);
                }
            }

            // Add table to document
            document.Add(pdfTable);

            // Close the document
            document.Close();

            Cursor = Cursors.Default;
            MessageBox.Show($"Document created successfully and saved at {pdfPath}");
        }
    }
    
}