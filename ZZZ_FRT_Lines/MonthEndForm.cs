using System;
using System.Data.SqlClient;
using System.Windows.Forms;
using System.Configuration;
using Microsoft.VisualBasic;



namespace MonthEndForm
{
    public partial class MonthEndForm : Form
    {
        public MonthEndForm()
        {
            InitializeComponent();
            this.CenterToScreen();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            DoneLabel1.Text = "Loading....";
            DoneLabel1.Visible = true;

            string Source = ConfigurationManager.ConnectionStrings["AUTOPART1"].ConnectionString;
            string Destination = ConfigurationManager.ConnectionStrings["ArnoldGroup2"].ConnectionString;


            SqlConnection conn = new SqlConnection(Helper.ConnString("ArnoldGroup2"));
            conn.Open();
            string query = ($@"
                                DELETE FROM [ArnoldGroup].[dbo].[CustomerMaster]
                            ");
            SqlCommand cmd = new SqlCommand(query, conn);
            cmd.ExecuteNonQuery();
            conn.Close();


            using (SqlConnection sourceCon = new SqlConnection(Source))
                {
                    SqlCommand cmd1 = new SqlCommand($@"
                        SELECT RTRIM(Customer.KeyCode) AS CustNo, 
                        RTRIM(Customer.Name) AS Customer,  
                        RTRIM(Customer.Addra) AS ShipToAddr,  
                        RTRIM(Customer.Addre) AS ShipToCity,  
                        RTRIM(Customer.MotDueSort) AS ShipToState,
                        RTRIM(Customer.PCode) AS ShipToZip, 
                        RTRIM(Customer.Stel) AS Phone,  
                        RTRIM(Customer.Area) AS Loc,  
                        RTRIM(Customer.Rep) AS Salesperson,  
                        CASE WHEN Customer.LType = 'NONE' THEN 'TRUE' ELSE 'FALSE' END AS Internal,
                        RTRIM(Customer.Lob) as LineOfBusiness,  
                        CASE WHEN Customer.DeadAccount = '' THEN 'N' ELSE Customer.DeadAccount END AS DeadAccount
                        FROM Customer 
                        WHERE Customer.KeyCode != '' AND Customer.Name != '' AND Customer.Area != '' AND Customer.Rep != '' AND Customer.LType != '' 
                        ORDER BY Customer.KeyCode", sourceCon);
                    sourceCon.Open();
                    using (SqlDataReader rdr = cmd1.ExecuteReader())
                    {
                        using (SqlConnection destinationCon = new SqlConnection(Destination))
                        {
                            using (SqlBulkCopy bc = new SqlBulkCopy(destinationCon))
                            {
                                bc.BatchSize = 60000;
                                bc.NotifyAfter = 1000;
                                //bc.SqlRowsCopied += (sender, eventArgs) =>
                                //{
                                //    lblProgRpt.Text += eventArgs.RowsCopied + " loaded...." + "<br/>";
                                //    lblMsg.Text = "In " + bc.BulkCopyTimeout + " Sec " + eventArgs.RowsCopied + " Copied.";  
                                // };

                                bc.DestinationTableName = "CustomerMaster";
                                destinationCon.Open();
                                bc.WriteToServer(rdr);
                            }
                        }
                    }
                }
            DoneLabel1.Text = "Done";

            }

        private void QuitButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        /*private void UpdateBuyingGroupsButton_Click(object sender, EventArgs e)
        {
            DoneLabel2.Text = "Loading....";
            DoneLabel2.Visible = true;

            string Source = ConfigurationManager.ConnectionStrings["AUTOPART1"].ConnectionString;
            string Destination = ConfigurationManager.ConnectionStrings["ArnoldGroup2"].ConnectionString;


            SqlConnection conn = new SqlConnection(Helper.ConnString("ArnoldGroup2"));
            conn.Open();
            string query = ($@"
                                DELETE FROM [ArnoldGroup].[dbo].[BuyingGroups]
                            ");
            SqlCommand cmd = new SqlCommand(query, conn);
            cmd.ExecuteNonQuery();
            conn.Close();


            using (SqlConnection sourceCon = new SqlConnection(Source))
            {
                SqlCommand cmd1 = new SqlCommand($@"
                                        SELECT CustBuyGrpBindings.Cust, 
                                        Customer.Name, 
                                        CustBuyGrpBindings.BuyingGroup 
                                        FROM Customer INNER JOIN 
                                        CustBuyGrpBindings ON Customer.KeyCode = CustBuyGrpBindings.Cust
                                                    ", sourceCon);
                sourceCon.Open();
                using (SqlDataReader rdr = cmd1.ExecuteReader())
                {
                    using (SqlConnection destinationCon = new SqlConnection(Destination))
                    {
                        using (SqlBulkCopy bc = new SqlBulkCopy(destinationCon))
                        {
                            bc.BatchSize = 60000;
                            bc.NotifyAfter = 1000;
                            bc.DestinationTableName = "BuyingGroups";
                            destinationCon.Open();
                            bc.WriteToServer(rdr);
                        }
                    }
                }
            }
            DoneLabel2.Text = "Done";

        }*/

        /*private void UpdateProductGroupsButton_Click(object sender, EventArgs e)
        {
            DoneLabel3.Text = "Loading....";
            DoneLabel3.Visible = true;

            string Source = ConfigurationManager.ConnectionStrings["AUTOPART1"].ConnectionString;
            string Destination = ConfigurationManager.ConnectionStrings["ArnoldGroup2"].ConnectionString;


            SqlConnection conn = new SqlConnection(Helper.ConnString("ArnoldGroup2"));
            conn.Open();
            string query = ($@"
                                DELETE FROM [ArnoldGroup].[dbo].[LineMaster]
                            ");
            SqlCommand cmd = new SqlCommand(query, conn);
            cmd.ExecuteNonQuery();
            conn.Close();


            using (SqlConnection sourceCon = new SqlConnection(Source))
            {
                SqlCommand cmd1 = new SqlCommand($@"
                                    SELECT RTrim([KeyCode]) as Line, 
                                    RTrim([a1]) as Description 
                                    FROM [AUTOPART].[dbo].[Codes] (NOLOCK) 
                                    WHERE Prefix = 'LCO'
                                                    ", sourceCon);
                sourceCon.Open();
                using (SqlDataReader rdr = cmd1.ExecuteReader())
                {
                    using (SqlConnection destinationCon = new SqlConnection(Destination))
                    {
                        using (SqlBulkCopy bc = new SqlBulkCopy(destinationCon))
                        {
                            bc.BatchSize = 60000;
                            bc.NotifyAfter = 1000;
                            bc.DestinationTableName = "LineMaster";
                            destinationCon.Open();
                             bc.WriteToServer(rdr);
                        }
                    }
                }
            }
            DoneLabel3.Text = "Done";
        }*/

        /*private void UpdateProductRangesButton_Click(object sender, EventArgs e)
        {
            DoneLabel4.Text = "Loading....";
            DoneLabel4.Visible = true;

            string Source = ConfigurationManager.ConnectionStrings["AUTOPART1"].ConnectionString;
            string Destination = ConfigurationManager.ConnectionStrings["ArnoldGroup2"].ConnectionString;


            SqlConnection conn = new SqlConnection(Helper.ConnString("ArnoldGroup2"));
            conn.Open();
            string query = ($@"
                                DELETE FROM [ArnoldGroup].[dbo].[SublineMaster]
                            ");
            SqlCommand cmd = new SqlCommand(query, conn);
            cmd.ExecuteNonQuery();
            conn.Close();


            using (SqlConnection sourceCon = new SqlConnection(Source))
            {
                SqlCommand cmd1 = new SqlCommand($@"
                                SELECT LEFT(KeyCode,3) as Line, 
                                substring(KeyCode, 4, 2) as SubLine, 
                                [a1] as Description 
                                FROM [AUTOPART].[dbo].[Codes] (NOLOCK) 
                                WHERE Prefix = 'R'
                                                    ", sourceCon);
                sourceCon.Open();
                using (SqlDataReader rdr = cmd1.ExecuteReader())
                {
                    using (SqlConnection destinationCon = new SqlConnection(Destination))
                    {
                        using (SqlBulkCopy bc = new SqlBulkCopy(destinationCon))
                        {
                            bc.BatchSize = 60000;
                            bc.NotifyAfter = 1000;
                            bc.DestinationTableName = "SublineMaster";
                            destinationCon.Open();
                            bc.WriteToServer(rdr);
                        }
                    }
                }
            }
            DoneLabel4.Text = "Done";
        }*/

        private void UpdateSalesRepsButton_Click(object sender, EventArgs e)
        {
            DoneLabel5.Text = "Loading....";
            DoneLabel5.Visible = true;

            string Source = ConfigurationManager.ConnectionStrings["AUTOPART1"].ConnectionString;
            string Destination = ConfigurationManager.ConnectionStrings["ArnoldGroup2"].ConnectionString;


            SqlConnection conn = new SqlConnection(Helper.ConnString("ArnoldGroup2"));
            conn.Open();
            string query = ($@"
                                DELETE FROM [ArnoldGroup].[dbo].[SalespersonMaster]
                            ");
            SqlCommand cmd = new SqlCommand(query, conn);
            cmd.ExecuteNonQuery();
            conn.Close();


            using (SqlConnection sourceCon = new SqlConnection(Source))
            {
                SqlCommand cmd1 = new SqlCommand($@"
                                SELECT RTRIM(KeyCode), 
                                RTRIM(a1),
                                CASE WHEN a1 LIKE '%SALES' THEN 'TRUE' ELSE 'FALSE' END as Ledger, 
                                RTRIM(RIGHT(a3,2))
                                FROM [AUTOPART].[dbo].[Codes](NOLOCK) 
                                WHERE Prefix = 'RC'
                                                    ", sourceCon);
                sourceCon.Open();
                using (SqlDataReader rdr = cmd1.ExecuteReader())
                {
                    using (SqlConnection destinationCon = new SqlConnection(Destination))
                    {
                        using (SqlBulkCopy bc = new SqlBulkCopy(destinationCon))
                        {
                            bc.BatchSize = 60000;
                            bc.NotifyAfter = 1000;
                            bc.DestinationTableName = "SalespersonMaster";
                            destinationCon.Open();
                            bc.WriteToServer(rdr);
                        }
                    }
                }
            }
            DoneLabel5.Text = "Done";
        }

        private void UpdateSalesButton_Click(object sender, EventArgs e)
        {
            DoneLabel6.Text = "Loading....";
            DoneLabel6.Visible = true;

            string Source = ConfigurationManager.ConnectionStrings["AUTOPART1"].ConnectionString;
            string Destination = ConfigurationManager.ConnectionStrings["ArnoldGroup2"].ConnectionString;

            string PeriodYear = Interaction.InputBox("Please enter the period year", "Current Year", "YYYY");
            string PeriodMonth = Interaction.InputBox("Please enter the period month", "Current Year", "MM");
            string StartDate = Interaction.InputBox("Please enter a start date", "Start Date", "MM/DD/YYYY");
            string EndDate = Interaction.InputBox("Please enter a end date", "End Date", "MM/DD/YYYY");
            string FiscalMonth = PeriodYear + PeriodMonth;

            using (SqlConnection sourceCon = new SqlConnection(Source))
            {
                SqlCommand cmd1 = new SqlCommand($@"
                                            SELECT [pg] as Line, 
                                            SUBSTRING(range,4,2) as SubLine, 
                                            [acct] as CustNo ,
                                            CAST('{ PeriodMonth }' as int) as PeriodMonth, 
                                            CAST('{ PeriodYear }' as int) as PeriodYear, 
                                            CAST('{ FiscalMonth }' as int) as FiscalMonth, 
                                            SUBSTRING(branch,3,2) as Loc, 
                                            RTRIM(salesrep) as SalesPerson, 
                                            '0.00' as GrossSales, 
                                            SUM([todaynetcost]) as GrossCosts, 
                                            CAST('0' as int) as GrossUnits,
                                            SUM(todaycoresales) as CoreSales, 
                                            '0.00' as CoreCosts, 
                                            '0.00' as CoreRetSales, 
                                            '0.00' as CoreRetCosts, 
                                            '0.00' as DefRetSales, 
                                            '0.00' as DefRetCosts, 
                                            '0.00' as NewRetSales, 
                                            '0.00' as NewRetCosts, 
                                            CAST('0' as int) as TotalRetUnits, 
                                            CAST('0' as int) as DefRetUnits, 
                                            CAST('0' as int) as NewRetUnits, 
                                            SUM(todaynetsales) as NetSales, 
                                            SUM([todayGP]) as GP, 
                                            CAST(SUM([todayqty]) as int) as Quantity
                                            FROM [AUTOPART].[dbo].[vw_MERI_SalesByBranchDateRange] (NOLOCK) 
                                            WHERE CAST(Date as date) BETWEEN '{ StartDate }' AND '{ EndDate }' 
                                            GROUP BY 
                                            pg, 
                                            range, 
                                            acct, 
                                            branch, 
                                            salesrep
                                                    ", sourceCon);
                sourceCon.Open();
                cmd1.CommandTimeout = 300000;
                    using (SqlConnection destinationCon = new SqlConnection(Destination))
                    {
                        SqlCommand CreateNewTable = new SqlCommand($@"
                                            IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='MERI_{PeriodYear}SalesMaster' AND xtype='U')
                                            create table ArnoldGroup.dbo.MERI_{PeriodYear}SalesMaster (
	                                            Line varchar(10) not null,
	                                            Subline varchar(10) null,
	                                            CustNo varchar(10) not null,
	                                            PeriodMonth int not null,
	                                            PeriodYear int not null,
	                                            FiscalMonth int not null,
	                                            Loc varchar(5) not null,
	                                            Salesperson varchar(10) null,
	                                            GrossSales money null,
	                                            GrossCosts money null,
	                                            GrossUnits int null,
	                                            CoreSales money null,
	                                            CoreCosts money null,
	                                            CoreRetSales money null,
	                                            CoreRetCosts money null,
	                                            DefRetSales money null,
	                                            DefRetCosts money null,
	                                            NewRetSales money null,
	                                            NewRetCosts money null,
	                                            TotalRetUnits int null,
	                                            DefRetUnits int null,
	                                            NewRetUnits int null,
	                                            NetSales money null,
	                                            GrossProfit money null,
	                                            NetUnits int null
                                            )
                            ", destinationCon);
                        destinationCon.Open();
                        CreateNewTable.CommandTimeout = 300000;
                        CreateNewTable.ExecuteNonQuery();
                        destinationCon.Close();
                    }

                    if (Convert.ToInt32(PeriodMonth) == 12)
                    {
                        using (SqlConnection destinationCon = new SqlConnection(Destination))
                        {
                        int nextYear = Convert.ToInt32(PeriodYear);
                        nextYear = nextYear + 1;
                            SqlCommand CreateNewTable1 = new SqlCommand($@"
                                            IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='MERI_{nextYear}SalesMaster' AND xtype='U')
                                            create table Arnoldgroup.dbo.MERI_{nextYear}SalesMaster (
	                                            Line varchar(10) not null,
	                                            Subline varchar(10) null,
	                                            CustNo varchar(10) not null,
	                                            PeriodMonth int not null,
	                                            PeriodYear int not null,
	                                            FiscalMonth int not null,
	                                            Loc varchar(5) not null,
	                                            Salesperson varchar(10) null,
	                                            GrossSales money null,
	                                            GrossCosts money null,
	                                            GrossUnits int null,
	                                            CoreSales money null,
	                                            CoreCosts money null,
	                                            CoreRetSales money null,
	                                            CoreRetCosts money null,
	                                            DefRetSales money null,
	                                            DefRetCosts money null,
	                                            NewRetSales money null,
	                                            NewRetCosts money null,
	                                            TotalRetUnits int null,
	                                            DefRetUnits int null,
	                                            NewRetUnits int null,
	                                            NetSales money null,
	                                            GrossProfit money null,
	                                            NetUnits int null
                                                                        )
                                                        ", destinationCon);
                            destinationCon.Open();
                            CreateNewTable1.CommandTimeout = 300000;
                            CreateNewTable1.ExecuteNonQuery();
                            destinationCon.Close();
                    }
                }



                using (SqlConnection destinationCon = new SqlConnection(Destination))
                {
                    SqlCommand removeDuplicates = new SqlCommand($@"
                                                IF EXISTS (SELECT * FROM MERI_{PeriodYear}SalesMaster WHERE PeriodMonth = '{PeriodMonth}') 
                                                BEGIN
                                                    DELETE FROM MERI_{PeriodYear}SalesMaster WHERE PeriodMonth = '{PeriodMonth}'
                                                END
                                    ", destinationCon);
                    destinationCon.Open();
                    removeDuplicates.CommandTimeout = 3000000;
                    removeDuplicates.ExecuteNonQuery();
                    destinationCon.Close();
                }



                using (SqlDataReader rdr = cmd1.ExecuteReader())
                {
                    using (SqlConnection destinationCon = new SqlConnection(Destination))
                    {
                        using (SqlBulkCopy bc = new SqlBulkCopy(destinationCon))
                        {
                            bc.BatchSize = 60000;
                            bc.NotifyAfter = 1000;
                            bc.DestinationTableName = $@"MERI_{PeriodYear}SalesMaster";
                            destinationCon.Open();
                            bc.WriteToServer(rdr);
                            destinationCon.Close();
                        }
                    }
                }

                    SqlConnection conn2 = new SqlConnection(Helper.ConnString("ArnoldGroup2"));

                    conn2.Open();
                    string query1 = ($@"
                                        IF EXISTS (SELECT * FROM SalesMaster WHERE PeriodMonth = '{PeriodMonth}' AND PeriodYear = '{PeriodYear}') 
                                        BEGIN
                                        DELETE FROM SalesMaster WHERE PeriodMonth = '{PeriodMonth}' AND PeriodYear = '{PeriodYear}'
                                        END
                                     ");
                    SqlCommand cmd2 = new SqlCommand(query1, conn2);
                    cmd2.ExecuteNonQuery();
                    conn2.Close();


                    conn2.Open();
                    string query2 = ($@"
                                    INSERT INTO SalesMaster 
                                    (
                                    Line,
                                    Subline,
                                    CustNo,
                                    PeriodMonth,
                                    PeriodYear,
                                    FiscalMonth,
                                    Loc,
                                    Salesperson,
                                    GrossSales,
                                    GrossCosts,
                                    GrossUnits,
                                    CoreSales,
                                    CoreCosts,
                                    CoreRetSales,
                                    CoreRetCosts,
                                    DefRetSales,
                                    DefRetCosts,
                                    NewRetSales,
                                    NewRetCosts,
                                    TotalRetUnits,
                                    DefRetUnits,
                                    NewRetUnits,
                                    NetSales,
                                    GrossProfit,
                                    NetUnits
                                    )

                                    SELECT 
                                    Line,
                                    Subline,
                                    CustNo,
                                    PeriodMonth,
                                    PeriodYear,
                                    FiscalMonth,
                                    Loc,
                                    Salesperson,
                                    GrossSales,
                                    GrossCosts,
                                    GrossUnits,
                                    CoreSales,
                                    CoreCosts,
                                    CoreRetSales,
                                    CoreRetCosts,
                                    DefRetSales,
                                    DefRetCosts,
                                    NewRetSales,
                                    NewRetCosts,
                                    TotalRetUnits,
                                    DefRetUnits,
                                    NewRetUnits,
                                    NetSales,
                                    GrossProfit,
                                    NetUnits

                                    FROM MERI_{PeriodYear}SalesMaster WHERE PeriodMonth = '{PeriodMonth}'
                                    ");
                    SqlCommand cmd3 = new SqlCommand(query2, conn2);
                    cmd3.ExecuteNonQuery();
                    conn2.Close();
                }
            DoneLabel6.Text = "Done";
        }
    }
    }
    

