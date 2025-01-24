using System;
using System.Collections.Generic;
using System.Drawing;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Configuration;
using System.Windows.Forms.DataVisualization.Charting;



namespace SOR
{
    public partial class SOR : Form
    {
        public DataTable table = new DataTable();
        public DataTable dt = new DataTable();
        public DataTable dt1 = new DataTable();
        public DataTable dt2 = new DataTable();
        public DataTable dt3 = new DataTable();
        public DataTable dt4 = new DataTable();
        public DataTable dt5 = new DataTable();
        public DataTable dt6 = new DataTable();
        public DataTable dt7 = new DataTable();
        public DataTable dt8 = new DataTable();
        public DataTable dtExport = new DataTable();
        public DataSet ds = new DataSet();
        public DataTable dtSuppliers = new DataTable();
        private bool loaded = false;
        private Nullable<int> Min = null;
        private Nullable<int> Max = null;
        private Nullable<int> PerCar = null;
        public string mnFlag = "";
        public int q = 0;
        public int z = 0;
        private string user = System.Security.Principal.WindowsIdentity.GetCurrent().Name.Replace(@"MERRILLCO\", "");
        public string word0 = "";
        private bool found = true;
        private bool firstSearch = true;
        public bool f9Pressed = false;
        public bool f8Pressed = false;
        public string supplier = "";
        public string lastBranch = "";
        public string lastVendor;
        public string supplierName;
        public string branch;
        public string usageState = "local";
        public double runningTotal = 0.00;
        public decimal period;
        public decimal sales;
        public DialogResult dialogResult;
        private TextBox CurrentTextbox = null;
        public float f_HeightRatio = new float();
        public float f_WidthRatio = new float();

        public SOR()
        {
            InitializeComponent();
            DataColumn dc = new DataColumn("Part", typeof(String));
            dtExport.Columns.Add(dc);
            dc = new DataColumn("Qty", typeof(String));
            dtExport.Columns.Add(dc);

            numberOfItemsLabel.Visible = false;
            branchTextbox.Items.Add("");
            branchTextbox.Items.Add("BR30");
            branchTextbox.Items.Add("BR43");
            branchTextbox.Items.Add("BR48");
            branchTextbox.Items.Add("BR51");
            branchTextbox.Items.Add("BR52");
            branchTextbox.Items.Add("BR60");
            branchTextbox.Items.Add("BR91");

            usage6Label.Text = "Group Usage Last 6:";
            usage12Label.Text = "Group Usage Last 12:";
            usageLastYear.Text = $@"Group Usage {Convert.ToInt32(DateTime.Now.Year) - 1}:";
            oneAgoLabel.Text = DateTime.Now.AddMonths(-0).ToString("MMMM");
            twoAgoLabel.Text = DateTime.Now.AddMonths(-1).ToString("MMMM");
            threeAgoLabel.Text = DateTime.Now.AddMonths(-2).ToString("MMMM");
            fourAgoLabel.Text = DateTime.Now.AddMonths(-3).ToString("MMMM");
            this.CenterToScreen();
            if (q == 0)
            {
                prevButton.Enabled = false;
            }

            listSuppliers.Select();
            this.WindowState = FormWindowState.Maximized;
        }

        private void GetPartData()
        {
            if (q > 0)
            {
                prevButton.Enabled = true;
            }

            if (q == 0)
            {
                prevButton.Enabled = false;
            }

            if (q == dt7.Rows.Count - 1)
            {
                nextButton.Enabled = false;
            }
            else
            {
                nextButton.Enabled = true;
            }

            dt1.Clear();
            SqlConnection conn1 = new SqlConnection(Helper.ConnString("AUTOPART"));
            {
                if (dt7.Rows.Count > 0 && dt8.Rows.Count > 0)
                {
                    orderTotalLabel.Visible = true;
                    suggestedOrderTotalTextBox.Visible = true;

                    orderQtyTextBox.Text = Convert.ToInt32(dt7.Rows[q]["RndQty"]).ToString();
                    reqQtyTextBox.Text = Convert.ToInt32(dt7.Rows[q]["CalcQty"]).ToString();
                    pgTextbox.Text = dt7.Rows[q]["Group"].ToString();
                    partTextbox.Text = dt7.Rows[q]["Part"].ToString();
                    descriptionTextbox.Text = dt7.Rows[q]["PDesc"].ToString();
                    rangeTextbox.Text = dt7.Rows[q]["Code1"].ToString();
                    word0 = dt7.Rows[q]["Part"].ToString();
                    boxQtyTextBox.Text = Convert.ToInt32(dt7.Rows[q]["BoxQty"]).ToString();
                    QtyOnPOtextBox.Text = Convert.ToInt32(dt7.Rows[q]["POQty"]).ToString();
                    QtyOnBOtextBox.Text = Convert.ToInt32(dt7.Rows[q]["BOQty"]).ToString();
                    vendorWhseTextBox.Text = Convert.ToString(dt7.Rows[q]["VendorWhse"]);
                    suggestedOrderTotalTextBox.Text = Convert.ToDecimal(dt7.Compute("SUM(SuggestTotal)", string.Empty)).ToString();
                    caseQtyTextBox.Text = Convert.ToInt32(dt7.Rows[q]["CaseQty"]).ToString();
                    palletQtyTextbox.Text = dt7.Rows[q]["PalletQty"].ToString();
                    fulSalesTextbox.Text = dt7.Rows[q]["FulSales"].ToString();
                    inventoryTypeTextbox.Text = dt7.Rows[q]["Code3"].ToString();

                    if (fulSalesTextbox.Text != "")
                    {
                        fulSalesTextbox.BackColor = Color.Red;
                    }
                    else
                    {
                        fulSalesTextbox.BackColor = Color.Empty;
                    }

                    lastMovedTextbox.Text = dt7.Rows[q]["LastRFV"].ToString();


                    conn1.Open();
                    SqlCommand cmd = new SqlCommand(
                    $@"
                    SELECT TOP(1) MAX(I.DATETIME)
                    FROM NLINES AS I INNER JOIN NHEADS AS II ON I.DOCUMENT = II.DOCUMENT
                    WHERE I.BRANCH = '{branchTextbox.Text}' AND II.SUPP = '{supplier}' AND Part = '{partTextbox.Text}' AND I.RC = 'N' AND RNOTE <>'' GROUP BY I.PART, II.MSG
                    ", conn1);
                    lastRGAtextBox.Text = Convert.ToString(cmd.ExecuteScalar());

                    if (lastRGAtextBox.Text != "")
                    {
                        lastRGAtextBox.BackColor = Color.Red;
                    }
                    else
                    {
                        lastRGAtextBox.BackColor = Color.Empty;
                    }

                    conn1.Close();

                    if (usageState == "local")
                    {
                        oneAgoTextBox.Text = dt7.Rows[q]["0ago"].ToString();
                        twoAgoTextBox.Text = dt7.Rows[q]["1ago"].ToString();
                        threeAgoTextBox.Text = dt7.Rows[q]["2ago"].ToString();
                        fourAgoTextBox.Text = dt7.Rows[q]["3ago"].ToString();
                        usageLast6Textbox.Text = dt7.Rows[q]["LocalUsageLast6"].ToString();
                        usageLast12Textbox.Text = dt7.Rows[q]["LocalUsageLast12"].ToString();
                        //usageLastYearTextbox.Text = dt7.Rows[q]["LocalUsageLastYear"].ToString();
                    }
                    else
                    {
                        oneAgoTextBox.Text = dt8.Rows[q]["0ago"].ToString();
                        twoAgoTextBox.Text = dt8.Rows[q]["1ago"].ToString();
                        threeAgoTextBox.Text = dt8.Rows[q]["2ago"].ToString();
                        fourAgoTextBox.Text = dt8.Rows[q]["3ago"].ToString();
                        usageLast6Textbox.Text = dt8.Rows[q]["LocalUsageLast6"].ToString();
                        usageLast12Textbox.Text = dt8.Rows[q]["LocalUsageLast12"].ToString();
                        //usageLastYearTextbox.Text = dt8.Rows[q]["LocalUsageLastYear"].ToString();
                    }

                    conn1.Close();
                }
                else
                {
                    //if (firstSearch == false)
                    //{
                        MessageBox.Show("no data found");
                        found = false;
                        return;
                    //}
                }
            }
        }

        private void getIOTdata()
        {
            dt2.Clear();
            SqlConnection conn2 = new SqlConnection(Helper.ConnString("AUTOPART"));
            {
                conn2.Open();
                //DataSet ds1 = new DataSet();
                SqlDataAdapter adapter2 = new SqlDataAdapter(
                $@"
                    SELECT CAST(VIO as int) as VIO, CAST(Vista as int) as Vista, CAST(ADWRegion as int) as ADWRegion, CAST(Blended as DECIMAL(18,2)) as Blended, MinYear, MaxYear, Makes, Models, CAST(PerCar as int) as PerCar FROM MERI_IOT WHERE (Part = '{partTextbox.Text}' OR Part = '{word0}')
                   ", conn2);
                adapter2.Fill(dt2);
                if (dt2.Rows.Count > 0)
                {
                    VIO_Textbox.Text = dt2.Rows[0]["VIO"].ToString();
                    vistaTextbox.Text = dt2.Rows[0]["Vista"].ToString();
                    adwRegionTextbox.Text = dt2.Rows[0]["ADWRegion"].ToString();
                    blendedTextbox.Text = dt2.Rows[0]["Blended"].ToString();
                    minYearTextbox.Text = dt2.Rows[0]["MinYear"].ToString();
                    maxYearText_box.Text = dt2.Rows[0]["MaxYear"].ToString();
                    modelTextbox.Text = dt2.Rows[0]["Models"].ToString();
                    makeTextbox.Text = dt2.Rows[0]["Makes"].ToString();
                    //perCarTextbox.Text = dt2.Rows[0]["PerCar"].ToString();
                    conn2.Close();
                }
                else
                {
                    if (loaded == false)
                    {
                        //MessageBox.Show("No IOT data found for this item");
                        VIO_Textbox.Text = "";
                        vistaTextbox.Text = "";
                        adwRegionTextbox.Text = "";
                        blendedTextbox.Text = "";
                        minYearTextbox.Text = "";
                        maxYearText_box.Text = "";
                        modelTextbox.Text = "";
                        makeTextbox.Text = "";
                        //perCarTextbox.Text = "";
                    }
                }
            }
        }

        private void getBranchStockData()
        {
            dt3.Clear();
            var branch = branchTextbox.Text;
            var part = partTextbox.Text;
            SqlConnection conn3 = new SqlConnection(Helper.ConnString("AUTOPART"));
            {
                conn3.Open();
                //DataSet ds1 = new DataSet();
                SqlDataAdapter adapter3 = new SqlDataAdapter(
                $@"
DECLARE @CutoffDate DATE = DATEADD(MONTH, -15, DATEFROMPARTS(YEAR(GETDATE()), MONTH(GETDATE()), 1));

WITH 
    CTE_0 as (
        SELECT Part, SUM(CASE 
        WHEN CAST(LEFT([Period], 4) + '-' + RIGHT([Period], 2) + '-01' AS DATE) >= @CutoffDate
        THEN Usage 
        ELSE 0 
    END) AS BR30Group
FROM MERI_Usages U WITH (NOLOCK)
WHERE 
    U.Part = '{part}' AND 
    Branch IN (SELECT Branch FROM Branches WHERE BranchGroup = 'BR30') 
GROUP BY Part
    ),

    CTE_00 as (
        SELECT 
        Part, 
        SUM(CASE 
        WHEN CAST(LEFT([Period], 4) + '-' + RIGHT([Period], 2) + '-01' AS DATE) >= @CutoffDate
        THEN Usage 
        ELSE 0 
    END) AS BR60Group
FROM MERI_Usages U WITH (NOLOCK)
WHERE 
    U.Part = '{part}' AND 
    Branch IN (SELECT Branch FROM Branches WHERE BranchGroup = 'BR60') 
GROUP BY Part
    ),

    CTE_Local as (
        SELECT 
        Part, 
        SUM(CASE 
        WHEN CAST(LEFT([Period], 4) + '-' + RIGHT([Period], 2) + '-01' AS DATE) >= @CutoffDate
        THEN Usage 
        ELSE 0 
    END) AS LocalUsageLast15
FROM MERI_Usages U WITH (NOLOCK)
WHERE 
    U.Part = '{part}' AND 
    Branch = '{branch}'
GROUP BY Part
    ),

    CTE_Total as (
        SELECT 
        Part, 
        SUM(CASE 
        WHEN CAST(LEFT([Period], 4) + '-' + RIGHT([Period], 2) + '-01' AS DATE) >= @CutoffDate
        THEN Usage 
        ELSE 0 
    END) AS TotalUsageLast15
FROM MERI_Usages U WITH (NOLOCK)
WHERE 
    U.Part = '{part}'
GROUP BY Part
    ),

    CTE_I AS (
        SELECT 
            Branch, 
            Part, 
            Free
        FROM 
            Stock
        WHERE 
            Branch = '{branch}' AND 
            Part = '{part}'
    ),
    CTE_II AS (
        SELECT 
            Part, 
            COUNT(Branch) AS StockingBranches 
        FROM 
            VW_MERI_Pro 
        WHERE 
            Part = '{part}' AND 
            Max > 0 
        GROUP BY 
            Part
    ),
    CTE_III AS (
        SELECT 
            Part, 
            SUM(FREE) AS CompanyStock 
        FROM 
            VW_MERI_Pro 
        WHERE 
            Part = '{part}' 
        GROUP BY 
            Part
    ),
    CTE_IV AS (
        SELECT 
            Pro.Part, 
            Pro.Class, 
            CAST(Pro.Min AS int) AS Min, 
            CAST(Pro.Max AS int) AS Max, 
            Pro.MNIndicator AS MNFlag, 
            CAST(Pro.MNDate AS date) AS MNDate, 
            CAST(Pro.BranchBox AS int) AS BranchBox, 
            CAST(Pro.PrevDateTime AS date) AS PrevDateTime, 
            --CAST(Pro.LocalUsageLast15 AS int) AS LocalUsageLast15, 
            --CAST(Pro.TotalUsageLast15 AS int) AS TotalUsageLast15, 
            Pro.MNnote, 
            CAST(Pro.PrevMin AS int) AS PrevMin, 
            CAST(Pro.PrevMax AS int) AS PrevMax, 
            CAST(Pro.FirstStocked AS date) AS FirstStocked,  
            CAST(Pro.PerCar AS int) AS PerCar,
            CASE 
                WHEN Pri.NPD != 0 THEN CAST(Pri.Cost - (Pri.Cost * (Pri.NPD / 100)) AS decimal(18,2)) 
                WHEN Pri.NPD = 0 AND Pri.Unit = 0 THEN CAST(Pri.COST AS decimal(18,2))
                ELSE CAST(Pri.COST / Pri.Unit AS decimal(18,2)) 
            END AS Cost
        FROM 
            VW_MERI_Pro AS Pro 
        INNER JOIN 
            VW_MERI_Pri AS Pri ON Pro.Part = Pri.Part
        WHERE 
            Pro.Branch = '{branch}' AND 
            Pro.Part = '{part}'
    ),
    CTE_V AS (
        SELECT 
            Part, 
            Qty AS PB_Qty, 
            Cost AS PB_Cost 
        FROM 
            VW_MERI_Pri 
        WHERE 
            Qty > 0
    )
SELECT 
    I.Branch, 
    CAST(I.Free AS int) AS Free, 
    ISNULL(CAST(II.StockingBranches AS int), 0) AS StockingBranches, 
    CAST(III.CompanyStock AS int) AS CompanyStock, 
    IV.Class, 
    IV.Min, 
    IV.Max, 
    IV.MNFlag, 
    IV.MNDate, 
    IV.BranchBox, 
    IV.PrevDateTime, 
    CAST(L.LocalUsageLast15 as int) as LocalUsageLast15, 
    CAST(T.TotalUsageLast15 as int) as TotalUsageLast15, 
    IV.MNnote, 
    IV.PrevMin, 
    IV.PrevMax, 
    IV.FirstStocked, 
    IV.PerCar, 
    IV.Cost, 
    ISNULL(V.PB_Qty, 0) AS PB_Qty, 
    ISNULL(V.PB_Cost, 0) AS PB_Cost, 
    CAST(Z.BR30Group AS int) AS BR30Group, 
    CAST(ZZ.BR60Group AS int) AS BR60Group 
FROM 
    CTE_I AS I 
LEFT OUTER JOIN 
    CTE_II AS II ON I.Part = II.Part 
LEFT OUTER JOIN 
    CTE_III AS III ON I.Part = III.Part 
LEFT OUTER JOIN 
    CTE_IV AS IV ON I.Part = IV.Part 
LEFT OUTER JOIN 
    CTE_V AS V ON I.Part = V.Part
LEFT OUTER JOIN
    CTE_0 AS Z ON I.Part = Z.Part
LEFT OUTER JOIN
    CTE_00 as ZZ ON I.Part = ZZ.Part
LEFT OUTER JOIN
    CTE_Local as L ON I.Part = L.Part
LEFT OUTER JOIN
    CTE_Total as T ON I.Part = T.Part
                   ", conn3);
                adapter3.Fill(dt3);
                if (dt3.Rows.Count > 0)
                {
                    onHandTextbox.Text = dt3.Rows[0]["Free"].ToString();
                    stockingStoresTextbox.Text = dt3.Rows[0]["StockingBranches"].ToString();
                    companyStockTextbox.Text = dt3.Rows[0]["CompanyStock"].ToString();
                    classTextbox.Text = dt3.Rows[0]["Class"].ToString();
                    minTextbox.Text = dt3.Rows[0]["Min"].ToString();
                    br30GroupTextBox.Text = dt3.Rows[0]["BR30Group"].ToString();
                    br60GroupTextBox.Text = dt3.Rows[0]["BR60Group"].ToString();
                    
                    try
                    {
                        Min = Convert.ToInt32(dt3.Rows[0]["Min"].ToString());
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message, "Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    maxTextbox.Text = dt3.Rows[0]["Max"].ToString();
                    try
                    {
                        Max = Convert.ToInt32(dt3.Rows[0]["Max"].ToString());
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message, "Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    mnFlagTextbox.Text = dt3.Rows[0]["MNFlag"].ToString();
                    mnFlag = dt3.Rows[0]["MNFlag"].ToString();
                    mnDateTextbox.Text = dt3.Rows[0]["MNDate"].ToString();
                    if (Convert.ToDateTime(mnDateTextbox.Text) <= new DateTime(1920, 1, 1))
                    {
                        mnDateTextbox.Text = "";
                    }
                    branchBoxTextbox.Text = dt3.Rows[0]["BranchBox"].ToString();
                    prevDateTimeTextbox.Text = dt3.Rows[0]["PrevDateTime"].ToString();
                    localUsageLast15Textbox.Text = dt3.Rows[0]["LocalUsageLast15"].ToString();
                    totalUsageLast15Textbox.Text = dt3.Rows[0]["TotalUsageLast15"].ToString();
                    mnNoteTextbox.Text = dt3.Rows[0]["MNnote"].ToString();
                    prevMinTextbox.Text = dt3.Rows[0]["PrevMin"].ToString();
                    prevMaxTextbox.Text = dt3.Rows[0]["PrevMax"].ToString();
                    firstStockedTextbox.Text = dt3.Rows[0]["FirstStocked"].ToString();
                    perCarTextbox.Text = dt3.Rows[0]["PerCar"].ToString();
                    costTextBox.Text = dt3.Rows[0]["Cost"].ToString();
                    if (Convert.ToInt32(dt3.Rows[0]["PB_Qty"]) > 0)
                    {
                        pbQtyTextBox.Text = Convert.ToInt32(dt3.Rows[0]["PB_Qty"]).ToString();
                        pbPriceTextBox.Text = Math.Round(Convert.ToDouble(dt3.Rows[0]["PB_Cost"]), 2).ToString();
                        pbPriceTextBox.BackColor = Color.Red;
                        pbQtyTextBox.BackColor = Color.Red;
                    }
                    else
                    {
                        pbQtyTextBox.Text = "";
                        pbPriceTextBox.Text = "";
                        pbPriceTextBox.BackColor = default;
                        pbQtyTextBox.BackColor = default;
                    }

                    try
                    {
                        PerCar = Convert.ToInt32(dt3.Rows[0]["PerCar"].ToString());
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message, "Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    conn3.Close();
                }
                else
                {
                    if (firstSearch == false)
                    {
                        MessageBox.Show("nothing found");
                        return;
                    }
                }
            }
        }

        private void loadDataGridView()
        {
            table.Clear();
            dataGridView1.DataSource = null;

            System.Windows.Forms.Cursor.Current = Cursors.WaitCursor;
            SqlConnection conn = new SqlConnection(Helper.ConnString("AUTOPART"));
            conn.Open();


            string query = ($@"
                                DECLARE @CutoffDate DATE = DATEADD(MONTH, -15, DATEFROMPARTS(YEAR(GETDATE()), MONTH(GETDATE()), 1));

                                SELECT        
                                    P.Branch, 
                                    CAST(P.Free AS INT) AS Free, 
                                    CAST(P.Min AS INT) AS Min, 
                                    CAST(P.Max AS INT) AS Max, 
                                    P.MNIndicator AS MNFlag, 
                                    CAST(P.QtyOnPO AS INT) AS QtyOnPO, 
                                    CAST(SUM(CASE WHEN CAST(LEFT([Period], 4) + '-' + RIGHT([Period], 2) + '-01' AS DATE) >= @CutoffDate THEN Usage ELSE 0 END) AS INT) AS LUL15, 
                                    CAST(P.Safety AS INT) AS Safety
                                
                                FROM            VW_MERI_Pro AS P LEFT OUTER JOIN
                                                         MERI_USAGES AS U ON P.Part = U.Part AND P.Branch = U.Branch
                                WHERE        (P.Part = '{partTextbox.Text}')
                                GROUP BY P.Branch, P.Min, P.Max, P.MNIndicator, P.QtyOnPO, P.Safety, P.Free
                                ORDER BY CASE WHEN P.Branch = 'BR30' THEN 1 WHEN P.Branch = 'BR60' THEN 2 ELSE 3 END, P.Branch
                            ");


            SqlCommand cmd = new SqlCommand(query, conn);

            using (SqlDataAdapter a = new SqlDataAdapter(cmd))
            {
                a.Fill(table);
                dataGridView1.DataSource = table;
            }
            System.Windows.Forms.Cursor.Current = Cursors.Default;
            dataGridView1.ClearSelection();
        }

        private void fillGraph()
        {
            dt4.Rows.Clear();
            var part = partTextbox.Text;
            SqlConnection conn = new SqlConnection(Helper.ConnString("AUTOPART"));
            SqlDataAdapter adapter = new SqlDataAdapter();
            SqlCommand cmd = conn.CreateCommand();
            cmd.CommandText = $@"

                                DECLARE @Date1 VARCHAR(10)
                                DECLARE @Date2 VARCHAR(10)
                                DECLARE @Date3 VARCHAR(10)
                                DECLARE @Date4 VARCHAR(10)
                                DECLARE @Date5 VARCHAR(10)
                                DECLARE @Date6 VARCHAR(10)
                                DECLARE @Date7 VARCHAR(10)
                                DECLARE @Date8 VARCHAR(10)
                                DECLARE @Date9 VARCHAR(10)
                                DECLARE @Date10 VARCHAR(10)
                                DECLARE @Date11 VARCHAR(10)
                                DECLARE @Date12 VARCHAR(10)
                                DECLARE @Date13 VARCHAR(10)
                                DECLARE @Date14 VARCHAR(10)
                                DECLARE @Date15 VARCHAR(10)
                                DECLARE @Date16 VARCHAR(10)
                                DECLARE @Date17 VARCHAR(10)
                                DECLARE @Date18 VARCHAR(10)
                                DECLARE @branch VARCHAR(4)
                                DECLARE @part VARCHAR(MAX)

                                SET @Date1 = FORMAT(DATEADD(month, -18, GETDATE()),'yyyyMM')
                                SET @Date2 = FORMAT(DATEADD(month, -17, GETDATE()),'yyyyMM')
                                SET @Date3 = FORMAT(DATEADD(month, -16, GETDATE()),'yyyyMM')
                                SET @Date4 = FORMAT(DATEADD(month, -15, GETDATE()),'yyyyMM')
                                SET @Date5 = FORMAT(DATEADD(month, -14, GETDATE()),'yyyyMM')
                                SET @Date6 = FORMAT(DATEADD(month, -13, GETDATE()),'yyyyMM')
                                SET @Date7 = FORMAT(DATEADD(month, -12, GETDATE()),'yyyyMM')
                                SET @Date8 = FORMAT(DATEADD(month, -11, GETDATE()),'yyyyMM')
                                SET @Date9 = FORMAT(DATEADD(month, -10, GETDATE()),'yyyyMM')
                                SET @Date10 = FORMAT(DATEADD(month, -9, GETDATE()),'yyyyMM')
                                SET @Date11 = FORMAT(DATEADD(month, -8, GETDATE()),'yyyyMM')
                                SET @Date12 = FORMAT(DATEADD(month, -7, GETDATE()),'yyyyMM')
                                SET @date13 = FORMAT(DATEADD(month, -6, GETDATE()),'yyyyMM')
                                SET @Date14 = FORMAT(DATEADD(month, -5, GETDATE()),'yyyyMM')
                                SET @Date15 = FORMAT(DATEADD(month, -4, GETDATE()),'yyyyMM')
                                SET @Date16 = FORMAT(DATEADD(month, -3, GETDATE()),'yyyyMM')
                                SET @Date17 = FORMAT(DATEADD(month, -2, GETDATE()),'yyyyMM')
                                SET @Date18 = FORMAT(DATEADD(month, -1, GETDATE()),'yyyyMM')
                                SET @part = '{part}'


                                SELECT '1' as period, ISNULL(SUM(Usage)/5,'0.00') as Usage FROM MERI_Usages WHERE Part = @part AND Period BETWEEN @Date1 AND @Date5
                                UNION ALL
                                SELECT '2' as period, ISNULL(SUM(Usage)/5,'0.00') as Usage FROM MERI_Usages WHERE Part = @part AND Period BETWEEN @Date2 AND @Date6
                                UNION ALL
                                SELECT '3' as period, ISNULL(SUM(Usage)/5,'0.00') as Usage FROM MERI_Usages WHERE Part = @part AND Period BETWEEN @Date3 AND @Date7
                                UNION ALL
                                SELECT '4' as period, ISNULL(SUM(Usage)/5,'0.00') as Usage FROM MERI_Usages WHERE Part = @part AND Period BETWEEN @Date4 AND @Date8
                                UNION ALL
                                SELECT '5' as period, ISNULL(SUM(Usage)/5,'0.00') as Usage FROM MERI_Usages WHERE Part = @part AND Period BETWEEN @Date5 AND @Date9
                                UNION ALL
                                SELECT '6' as period, ISNULL(SUM(Usage)/5,'0.00') as Usage FROM MERI_Usages WHERE Part = @part AND Period BETWEEN @Date6 AND @Date10
                                UNION ALL
                                SELECT '7' as period, ISNULL(SUM(Usage)/5,'0.00') as Usage FROM MERI_Usages WHERE Part = @part AND Period BETWEEN @Date7 AND @Date11
                                UNION ALL
                                SELECT '8' as period, ISNULL(SUM(Usage)/5,'0.00') as Usage FROM MERI_Usages WHERE Part = @part AND Period BETWEEN @Date8 AND @Date12
                                UNION ALL
                                SELECT '9' as period, ISNULL(SUM(Usage)/5,'0.00') as Usage FROM MERI_Usages WHERE Part = @part AND Period BETWEEN @Date9 AND @Date13
                                UNION ALL
                                SELECT '10' as period, ISNULL(SUM(Usage)/5,'0.00') as Usage FROM MERI_Usages WHERE Part = @part AND Period BETWEEN @Date10 AND @Date14
                                UNION ALL
                                SELECT '11' as period, ISNULL(SUM(Usage)/5,'0.00') as Usage FROM MERI_Usages WHERE Part = @part AND Period BETWEEN @Date11 AND @Date15
                                UNION ALL
                                SELECT '12' as period, ISNULL(SUM(Usage)/5,'0.00') as Usage FROM MERI_Usages WHERE Part = @part AND Period BETWEEN @Date12 AND @Date16
                                UNION ALL
                                SELECT '13' as period, ISNULL(SUM(Usage)/5,'0.00') as Usage FROM MERI_Usages WHERE Part = @part AND Period BETWEEN @Date13 AND @Date17
                                UNION ALL
                                SELECT '14' as period, ISNULL(SUM(Usage)/5,'0.00') as Usage FROM MERI_Usages WHERE Part = @part AND Period BETWEEN @Date14 AND @Date18


                                ";
            adapter.SelectCommand = cmd;


            conn.Open();
            adapter.Fill(ds);
            adapter.Fill(dt4);
            conn.Close();


            chart1.ChartAreas[0].AxisX.Interval = 1;
            chart1.DataSource = dt4;

            //Mapping a field with x-value of chart
            chart1.Series[0].XValueMember = "Period";

            //Mapping a field with y-value of Chart
            chart1.Series[0].YValueMembers = "Usage";

            //Bind the DataTable with Chart
            //chart1.DataBind();
            conn.Close();

        }

        private void fillGraph1()
        {
            dt5.Rows.Clear();
            SqlConnection conn = new SqlConnection(Helper.ConnString("AUTOPART"));
            SqlCommand cmd = conn.CreateCommand();

            var a = dt4.Rows[0][1].ToString();
            var b = dt4.Rows[1][1].ToString();
            var c = dt4.Rows[2][1].ToString();
            var d = dt4.Rows[3][1].ToString();
            var e = dt4.Rows[4][1].ToString();
            var f = dt4.Rows[5][1].ToString();
            var g = dt4.Rows[6][1].ToString();
            var h = dt4.Rows[7][1].ToString();
            var i = dt4.Rows[8][1].ToString();
            var j = dt4.Rows[9][1].ToString();
            var k = dt4.Rows[10][1].ToString();
            var l = dt4.Rows[11][1].ToString();
            var m = dt4.Rows[12][1].ToString();
            var n = dt4.Rows[13][1].ToString();

            if (String.IsNullOrEmpty(a)) { a = "0.00"; }
            if (String.IsNullOrEmpty(b)) { b = "0.00"; }
            if (String.IsNullOrEmpty(c)) { c = "0.00"; }
            if (String.IsNullOrEmpty(d)) { d = "0.00"; }
            if (String.IsNullOrEmpty(e)) { e = "0.00"; }
            if (String.IsNullOrEmpty(f)) { f = "0.00"; }
            if (String.IsNullOrEmpty(g)) { g = "0.00"; }
            if (String.IsNullOrEmpty(h)) { h = "0.00"; }
            if (String.IsNullOrEmpty(i)) { i = "0.00"; }
            if (String.IsNullOrEmpty(j)) { j = "0.00"; }
            if (String.IsNullOrEmpty(k)) { k = "0.00"; }
            if (String.IsNullOrEmpty(l)) { l = "0.00"; }
            if (String.IsNullOrEmpty(m)) { m = "0.00"; }
            if (String.IsNullOrEmpty(n)) { n = "0.00"; }



            conn.Open();
            //DataSet ds1 = new DataSet();
            SqlDataAdapter adapter1 = new SqlDataAdapter(
            $@"


                DECLARE @1 FLOAT
                DECLARE @2 FLOAT
                DECLARE @3 FLOAT
                DECLARE @4 FLOAT
                DECLARE @5 FLOAT
                DECLARE @6 FLOAT
                DECLARE @7 FLOAT
                DECLARE @8 FLOAT
                DECLARE @9 FLOAT
                DECLARE @10 FLOAT
                DECLARE @11 FLOAT
                DECLARE @12 FLOAT
                DECLARE @13 FLOAT
                DECLARE @14 FLOAT

                SET @1 = {a}
                SET @2 = {b}
                SET @3 = {c}
                SET @4 = {d}
                SET @5 = {e}
                SET @6 = {f}
                SET @7 = {g}
                SET @8 = {h}
                SET @9 = {i}
                SET @10 = {j}
                SET @11 = {k}
                SET @12 = {l}
                SET @13 = {m}
                SET @14 = {n}

                SET ARITHABORT ON;

                DECLARE @OurData TABLE
                    (
                    x NUMERIC(18,6) NOT NULL,
                    y NUMERIC(18,6) NOT NULL
                    );
                  INSERT INTO @OurData
                    (x, y)
                  SELECT 
                   x,y
                   FROM (VALUES
                  (1,@1),(2,@2),(3,@3),(4,@4),(5,@5),(6,@6),(7,@7),(8,@8),(9,@9),
                  (10,@10),(11,@11),(12,@12),(13,@13),(14,@14)
                  )f(x,y)
                  SELECT ((Sy * Sxx) - (Sx * Sxy)) / ((N * (Sxx)) - (Sx * Sx)) AS a, CAST(((N * Sxy) - (Sx * Sy)) / ((N * Sxx) - (Sx * Sx)) as decimal (18,2)) AS b
                    FROM
                      (
                      SELECT SUM([@OurData].x) AS Sx, SUM([@OurData].y) AS Sy,
                        SUM([@OurData].x * [@OurData].x) AS Sxx,
                        SUM([@OurData].x * [@OurData].y) AS Sxy,
                        COUNT(*) AS N
                        FROM @OurData
                      ) sums;

                   ", conn);

            adapter1.Fill(dt5);
            conn.Close();


            string start1 = dt5.Rows[0]["a"].ToString();
            string interval1 = dt5.Rows[0]["b"].ToString();
            //string interval2 = dt5.Rows[0]["r"].ToString();

            var start = Convert.ToDecimal(start1);
            var interval = (Convert.ToDecimal(interval1));

            SqlConnection conn1 = new SqlConnection(Helper.ConnString("AUTOPART"));
            string username = System.Security.Principal.WindowsIdentity.GetCurrent().Name;
            int stringLength = username.Length;
            string currOperator = username.Substring(10, 5);

            conn1.Open();
            string query = ($@"
                                Select Slope FROM VW_MERI_PartSlope WHERE Part = '{partTextbox.Text}'
                            ");
            SqlCommand cmd1 = new SqlCommand(query, conn1);
            slopeTextbox.Text = Convert.ToString(cmd1.ExecuteScalar());

            decimal slope = Convert.ToDecimal(slopeTextbox.Text);
            //decimal period = (AMU - target) / slope;
            if (slope != 0 && totalUsageLast15Textbox.Text != "")
            {
                decimal AMU = Convert.ToDecimal(totalUsageLast15Textbox.Text) / 15;
                decimal target = .13M;
                period = Math.Round((AMU - target) / Math.Abs(slope), 0);
                sales = Math.Round((period + 1) * (AMU + (.5M * slope * period)), 0);
                periodTextbox.Text = period.ToString("#");
                salesTextbox.Text = sales.ToString("#");
            }
            else if (slope < 0)
            {
                periodTextbox.Text = period.ToString("#");
                salesTextbox.Text = sales.ToString("#");
            }

            if (slope >= 0)
            {
                periodTextbox.Text = "";
                salesTextbox.Text = "";
            }

            if (periodTextbox.Text != "" && sales == 0)
            {
                salesTextbox.Text = "0";
            }
            if (companyStockTextbox.Text == "")
            {
                companyStockTextbox.Text = "0";
            }
            if (Convert.ToDecimal(sales) < Convert.ToDecimal(companyStockTextbox.Text) && slope < 0)
            {
                salesTextbox.BackColor = Color.Red;
            }
            else
            {
                salesTextbox.BackColor = Color.Empty;
            }


            var S = start;
            var a1 = start + (interval * 1);
            var b1 = start + (interval * 2);
            var c1 = start + (interval * 3);
            var d1 = start + (interval * 4);
            var e1 = start + (interval * 5);
            var f1 = start + (interval * 6);
            var g1 = start + (interval * 7);
            var h1 = start + (interval * 8);
            var i1 = start + (interval * 9);
            var j1 = start + (interval * 10);
            var k1 = start + (interval * 11);
            var l1 = start + (interval * 12);
            var m1 = start + (interval * 13);

            double S1 = Convert.ToDouble(S);
            double a2 = Convert.ToDouble(a1);
            double b2 = Convert.ToDouble(b1);
            double c2 = Convert.ToDouble(c1);
            double d2 = Convert.ToDouble(d1);
            double e2 = Convert.ToDouble(e1);
            double f2 = Convert.ToDouble(f1);
            double g2 = Convert.ToDouble(g1);
            double h2 = Convert.ToDouble(h1);
            double i2 = Convert.ToDouble(i1);
            double j2 = Convert.ToDouble(j1);
            double k2 = Convert.ToDouble(k1);
            double l2 = Convert.ToDouble(l1);
            double m2 = Convert.ToDouble(m1);

            chart1.Series[1].Points.Clear();


            chart1.Series[1].Points.Add(S1, 1);
            chart1.Series[1].Points.Add(a2, 2);
            chart1.Series[1].Points.Add(b2, 3);
            chart1.Series[1].Points.Add(c2, 4);
            chart1.Series[1].Points.Add(d2, 5);
            chart1.Series[1].Points.Add(e2, 6);
            chart1.Series[1].Points.Add(f2, 7);
            chart1.Series[1].Points.Add(g2, 8);
            chart1.Series[1].Points.Add(h2, 9);
            chart1.Series[1].Points.Add(i2, 10);
            chart1.Series[1].Points.Add(j2, 11);
            chart1.Series[1].Points.Add(k2, 12);
            chart1.Series[1].Points.Add(l2, 13);
            chart1.Series[1].Points.Add(m2, 14);

        }

        private void Form1_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            //CurrentTextbox = (ComboBox)sender;
            if (supplier == "")
            {
                vendorChanged();
            }
            else
            {
                var commands = Convert.ToString(listSuppliers.Text).Split(' ', (char)2);
                var selectedSupplier = commands[0];

                if (supplier != Convert.ToString(selectedSupplier) && firstSearch == false)
                {
                    dialogResult = MessageBox.Show("Changing the vendor number or branch without saving will result in a loss of all current changes. Do you wish to continue?", "Important Notice!", MessageBoxButtons.YesNo);
                    if (dialogResult == DialogResult.Yes)
                    {
                        if (e.KeyCode == Keys.Enter)
                        {
                            q = 0;
                            vendorChanged();
                        }
                    }
                    if (dialogResult == DialogResult.No)
                    {
                        listSuppliers.Text = Convert.ToString(supplierName);
                        firstSearch = true;
                    }
                }
            }
            branchGroupButton.Visible = true;
            DeleteOrderButton.Visible = true;
        }

        private void colorActiveTextbox_Leave(object sender, EventArgs e)
        {
            CurrentTextbox = (TextBox)sender;
            CurrentTextbox.BackColor = Color.Empty;
        }

        private void colorActiveTextbox_Enter(object sender, EventArgs e)
        {
            CurrentTextbox = (TextBox)sender;
            CurrentTextbox.BackColor = Color.Yellow;
        }

        private void closeButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            firstSearch = true;
            Nullable<int> newMin = null;
            try
            {
                newMin = Convert.ToInt32(minTextbox.Text);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
                minTextbox.Text = Min.ToString();
                return;
            }
            Nullable<int> newMax = null;
            try
            {
                newMax = Convert.ToInt32(maxTextbox.Text);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
                maxTextbox.Text = Max.ToString();
                return;
            }

            if (newMin > newMax)
            {
                MessageBox.Show($"Min can not be greater than Max");
                minTextbox.Text = Min.ToString();
                maxTextbox.Text = Max.ToString();
                return;
            }

            if (perCarTextbox.Text != "")
            {
                Nullable<int> newPerCar = null;
                try
                {
                    newPerCar = Convert.ToInt32(perCarTextbox.Text);
                }

                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    maxTextbox.Text = Max.ToString();
                    return;
                }

                if (newMin > 0 && newMin < Convert.ToInt32(perCarTextbox.Text))
                {
                    MessageBox.Show($"Min can not be greater than zero and less than the Per Car value");
                    minTextbox.Text = Min.ToString();
                    return;
                }

                if (newMax > 0 && newMax < Convert.ToInt32(perCarTextbox.Text))
                {
                    MessageBox.Show($"Max can not be greater than zero and less than the Per Car value");
                    maxTextbox.Text = Max.ToString();
                    return;
                }
            }
            else
            {
                perCarTextbox.Text = "0";
            }


            user = user.Substring(0, 5);

            if (mnDateTextbox.Text == "" && mnFlagTextbox.Text != "")
            {
                MessageBox.Show($"New MN Date: Null");
            }
            else
            {
                MessageBox.Show($"New MN Date: {DateTime.Now.ToString("yyyyMMdd")}");
            }
            MessageBox.Show("done");
            updatePart();
        }

        private void updatePart()
        {
            SqlConnection conn = new SqlConnection(Helper.ConnString("AUTOPART"));

            using (SqlCommand cmd = new SqlCommand("sp_MERI_updatePartfromApparatus", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                //set new Min
                cmd.Parameters.Add("@Min", SqlDbType.VarChar).Value = minTextbox.Text;

                //set new Max
                cmd.Parameters.Add("@Max", SqlDbType.VarChar).Value = maxTextbox.Text;

                //set PerCar
                cmd.Parameters.Add("@PerCar", SqlDbType.VarChar).Value = perCarTextbox.Text;

                //set Branch
                cmd.Parameters.Add("@Branch", SqlDbType.VarChar).Value = branchTextbox.Text;

                //set Part
                cmd.Parameters.Add("@part", SqlDbType.VarChar).Value = partTextbox.Text;

                //set MN Date
                if (mnFlagTextbox.Text == "")
                {
                    cmd.Parameters.Add("@MNDate", SqlDbType.VarChar).Value = "";
                }
                else
                {
                    cmd.Parameters.Add("@MNDate", SqlDbType.VarChar).Value = DateTime.Now.ToString("yyyyMMdd");
                }

                //set MinMaxInits
                cmd.Parameters.Add("@MNInits", SqlDbType.VarChar).Value = user;

                //set PrevMin
                cmd.Parameters.Add("@PrevMin", SqlDbType.VarChar).Value = Min;

                //set PrevMax
                cmd.Parameters.Add("@PrevMax", SqlDbType.VarChar).Value = Max;

                cmd.Parameters.Add("@MNFlag", SqlDbType.VarChar).Value = mnFlag;

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

        private void prevButton_Click(object sender, EventArgs e)
        {
            dtExport.Rows.RemoveAt(dtExport.Rows.Count - 1);

            q--;
            numberOfItemsLabel.Text = $"Item {q + 1} of {dt7.Rows.Count}";

            GetPartData();
            if (found == false)
            {
                return;
            }
            getBranchStockData();
            getIOTdata();
            loadDataGridView();
            if (chart1.Series[1].Points.Count > 0)
            {
                for (int i = 0; i < chart1.Series[1].Points.Count; i++)
                {
                    chart1.Series[1].Points.RemoveAt(i);
                }
            }

            fillGraph();
            fillGraph1();
            orderQtyTextBox.Focus();
            orderQtyTextBox.SelectAll();
            if (pbQtyTextBox.Text != "")
            {
                if (Convert.ToInt32(pbQtyTextBox.Text) < Convert.ToInt32(orderQtyTextBox.Text))
                {
                    runningTotal -= Math.Round(Convert.ToDouble(dt3.Rows[0]["PB_Cost"]), 2) * Convert.ToInt32(dt7.Rows[q]["RndQty"]);
                    runningTotal = Math.Round(runningTotal, 2);
                    runningTotalLabel.Text = runningTotal.ToString();
                }
                else
                {
                    runningTotal -= Math.Round(Convert.ToDouble(costTextBox.Text) * Convert.ToDouble(orderQtyTextBox.Text), 2);
                    runningTotal = Math.Round(runningTotal, 2);
                    runningTotalLabel.Text = runningTotal.ToString();
                }
            }
            else
            {
                runningTotal -= Math.Round(Convert.ToDouble(costTextBox.Text) * Convert.ToDouble(orderQtyTextBox.Text), 2);
                runningTotal = Math.Round(runningTotal, 2);
                runningTotalLabel.Text = runningTotal.ToString();
            }
        }

        private void nextButton_Click(object sender, EventArgs e)
        {
            double qty = Convert.ToDouble(orderQtyTextBox.Text) / Convert.ToDouble(boxQtyTextBox.Text);
            if (qty != (int)qty)
            {
                dialogResult = MessageBox.Show("The order amount endered is not an increment of the box quantity. Do you wish to continue?", "Important Notice!", MessageBoxButtons.YesNo);
                if (dialogResult == DialogResult.No)
                {
                    orderQtyTextBox.Text = Convert.ToInt32(dt7.Rows[q]["RndQty"]).ToString();
                    orderQtyTextBox.Focus();
                    orderQtyTextBox.SelectAll();
                    return;
                }
            }

            dt7.Rows[q]["RndQty"] = orderQtyTextBox.Text;
            if (pbQtyTextBox.Text != "")
            {
                if (Convert.ToInt32(pbQtyTextBox.Text) < Convert.ToInt32(orderQtyTextBox.Text))
                {
                    runningTotal += Convert.ToDouble(pbPriceTextBox.Text) * Convert.ToDouble(orderQtyTextBox.Text);
                    runningTotalLabel.Text = Convert.ToDecimal(runningTotal).ToString();
                }
                else
                {
                    runningTotal += Convert.ToDouble(costTextBox.Text) * Convert.ToDouble(orderQtyTextBox.Text);
                    runningTotalLabel.Text = Convert.ToDecimal(runningTotal).ToString();
                }
            }
            else
            {
                runningTotal += Convert.ToDouble(costTextBox.Text) * Convert.ToDouble(orderQtyTextBox.Text);
                runningTotalLabel.Text = Convert.ToDecimal(runningTotal).ToString();
            }

            DataRow newItem = dtExport.NewRow();
            newItem[0] = partTextbox.Text.ToString();
            newItem[1] = orderQtyTextBox.Text.ToString();
            dtExport.Rows.Add(newItem);

            q++;
            numberOfItemsLabel.Text = $"Item {q + 1} of {dt7.Rows.Count}";

            //GetBranchData();
            GetPartData();
            if (found == false)
            {
                return;
            }
            getBranchStockData();
            getIOTdata();
            loadDataGridView();
            if (chart1.Series[1].Points.Count > 0)
            {
                for (int i = 0; i < chart1.Series[1].Points.Count; i++)
                {
                    chart1.Series[1].Points.RemoveAt(i);
                }
            }

            fillGraph();
            fillGraph1();

            if (q > 0)
            {
                suggestedOrderTotalTextBox.Visible = false;
                orderTotalLabel.Visible = false;
            }

            orderQtyTextBox.Focus();
            orderQtyTextBox.SelectAll();
        }

        private void orderQtyTextBox_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                dt7.Rows[q]["CalcQty"] = orderQtyTextBox.Text;
                orderQtyTextBox.BackColor = Color.LightGreen;
            }
        }

        private void exportButton_Click(object sender, EventArgs e)
        {
            var commands = Convert.ToString(listSuppliers.Text).Split(' ', (char)2);
            var selectedSupplier = commands[0];
            var branch = branchTextbox.Text;

            if (q == dt7.Rows.Count - 1 && Convert.ToInt32(orderQtyTextBox.Text) > 0)
            {
                DataRow newItem = dtExport.NewRow();
                newItem[0] = partTextbox.Text.ToString();
                newItem[1] = orderQtyTextBox.Text.ToString();
                dtExport.Rows.Add(newItem);
            }

            if (Convert.ToDecimal(runningTotalLabel.Text) < Convert.ToDecimal(minOrderTextBox.Text))
            {
                dialogResult = MessageBox.Show("The current order total does not meet the minumum purchase requirement. Do you wish to continue anyway?", "Notice", MessageBoxButtons.YesNo);
                if (dialogResult == DialogResult.No)
                {
                    return;
                }
            }

            for (int x = 0; x <= dtExport.Rows.Count - 1; x++)
            {
                string part = dtExport.Rows[x]["Part"].ToString();
                SqlConnection conn = new SqlConnection(Helper.ConnString("AUTOPART"));

                using (SqlCommand cmd = new SqlCommand("sp_MERI_updateReviewfromApparatus", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    //set new Min
                    cmd.Parameters.Add("@Part", SqlDbType.VarChar).Value = part;

                    //set new Max
                    cmd.Parameters.Add("@Branch", SqlDbType.VarChar).Value = branch;

                    //set PerCar
                    cmd.Parameters.Add("@Supplier", SqlDbType.VarChar).Value = selectedSupplier;

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

            }

            for (int i = dtExport.Rows.Count - 1; i >= 0; i--)
            {
                DataRow dr = dtExport.Rows[i];
                if (dr["Qty"].ToString() == "0")
                    dr.Delete();
            }

            dtExport.AcceptChanges();

            SaveFileDialog SFD1 = new SaveFileDialog();
            SFD1.Filter = "csv files (*.csv)|*.csv";
            SFD1.ShowDialog();
            string filePath = SFD1.FileName;


            try
            {
                //checked for the datatable dtCSV not empty
                if (dtExport != null && dtExport.Rows.Count > 0)
                {
                    // create object for the StringBuilder class
                    StringBuilder sb = new StringBuilder();

                    // Get name of columns from datatable and assigned to the string array
                    string[] columnNames = dtExport.Columns.Cast<DataColumn>().Select(column => column.ColumnName).ToArray();

                    // Create comma sprated column name based on the items contains string array columnNames
                    sb.AppendLine(string.Join(",", columnNames));

                    // Fatch rows from datatable and append values as comma saprated to the object of StringBuilder class 
                    foreach (DataRow row in dtExport.Rows)
                    {
                        IEnumerable<string> fields = row.ItemArray.Select(field => string.Concat("\"", field.ToString().Replace("\"", "\"\""), "\""));
                        sb.AppendLine(string.Join(",", fields));
                    }

                    // save the file
                    sb.Remove(0, Convert.ToString(sb).Split('\n').FirstOrDefault().Length + 1);
                    File.WriteAllText($@"{filePath}", sb.ToString());
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                throw;
            }

            dtExport.Clear();
            q = 0;
            runningTotalLabel.Text = "0.00";
            runningTotal = 0;
            vendorChanged();

            ActiveForm.Dispose();
            SOR sd = new SOR();
            sd.Show();

        }

        private void removeButton_Click(object sender, EventArgs e)
        {
            dt7.Rows[q].Delete();

            if (q >= dt7.Rows.Count - 1)
            {
                q--;
            }

            //GetBranchData();
            GetPartData();
            if (found == false)
            {
                return;
            }
            getBranchStockData();
            getIOTdata();
            loadDataGridView();
            if (chart1.Series[1].Points.Count > 0)
            {
                for (int i = 0; i < chart1.Series[1].Points.Count; i++)
                {
                    chart1.Series[1].Points.RemoveAt(i);
                }
            }

            fillGraph();
            fillGraph1();

        }

        public void branchTextbox_SelectedIndexChanged(object sender, EventArgs e)
        {
            listSuppliers.DataSource = null;
            dtSuppliers.Clear();
            dtSuppliers.AcceptChanges();

            ClearTextBoxes();

            dt.Clear();
            SqlConnection conn = new SqlConnection(Helper.ConnString("AUTOPART"));
            {
                conn.Open();
                branch = branchTextbox.Text;
                SqlDataAdapter adapter = new SqlDataAdapter(
                $@"
                    SELECT Branch, Name, Addrd, Addre FROM Branches WHERE Branch = '{branch}'
                   ", conn);
                adapter.Fill(dt);
                if (dt.Rows.Count > 0)
                {
                    var name = dt.Rows[0]["Name"].ToString();
                    var city = dt.Rows[0]["Addrd"].ToString();
                    var state = dt.Rows[0]["Addre"].ToString();
                    branchTextbox.Text = branch;
                    nameTextbox.Text = name;
                    cityTextbox.Text = city;
                    stateTextbox.Text = state;
                    conn.Close();
                }
                else
                {
                    if (firstSearch == false)
                    {
                        MessageBox.Show("nothing found");
                        return;
                    }
                }

                SqlConnection conn0 = new SqlConnection(Helper.ConnString("AUTOPART"));
                {
                    conn0.Open();
                    //DataSet ds1 = new DataSet();
                    SqlDataAdapter adapter0 = new SqlDataAdapter(
                    $"SELECT '' as Supplier, '' as SupplierName UNION ALL SELECT R.Supplier, R.Supplier + ' - ' + S.Name as SupplierName FROM Review R INNER JOIN Supplier S ON R.Supplier = S.KeyCode WHERE R.Branch = '{branch}' AND R.RndQty > 0 GROUP BY R.Supplier, S.Name", conn0);
                    adapter0.Fill(dtSuppliers);

                    this.listSuppliers.DataSource = dtSuppliers;
                    this.listSuppliers.DisplayMember = "SupplierName";

                }
            }
        }

        public void ClearTextBoxes()
        {
            Action<Control.ControlCollection> func = null;

            func = (controls) =>
            {
                foreach (Control control in controls)
                    if (control is TextBox)
                        (control as TextBox).Clear();
                    else
                        func(control.Controls);
            };

            func(Controls);
        }

        private void vendorChanged()
        {
            vendorChanged(Cursor);
        }

        private void vendorChanged(System.Windows.Forms.Cursor cursor)
        {
            supplierName = listSuppliers.Text;
            var commands = supplierName.Split(' ', (char)2);
            supplier = commands[0];
            System.Windows.Forms.Cursor.Current = Cursors.WaitCursor;
            dt8.Clear();
            dt7.Clear();

            using (var connection = new SqlConnection(Helper.ConnString("AUTOPART")))
            {
                connection.Open();

                // Step 1: Populate dt7
                dt7 = ExecuteQuery(connection, GenerateQueryForDt7(supplier, branch));

                // Step 2: Populate dt8
                dt8 = ExecuteQuery(connection, GenerateQueryForDt8(supplier, branch));

                // Step 3: Fetch minimum order value
                minOrderTextBox.Text = FetchMinOrderValue(connection, supplier);

                connection.Close();
            }

            minOrderLabel.Visible = true;
            minOrderTextBox.Visible = true;
            cancelButton.Enabled = true;

            // Execute other functions
            GetPartData();
            if (!found) return;

            getBranchStockData();
            getIOTdata();
            loadDataGridView();

            // Clear chart points
            ClearChartPoints(chart1);

            fillGraph();
            fillGraph1();

            numberOfItemsLabel.Visible = true;
            numberOfItemsLabel.Text = $"Item {q + 1} of {dt7.Rows.Count}";

            // Validate order total
            ValidateOrderTotal();

            orderQtyTextBox.Focus();
            orderQtyTextBox.SelectAll();
            System.Windows.Forms.Cursor.Current = Cursors.Default;
        }

        private DataTable ExecuteQuery(SqlConnection connection, string query)
        {
            var dataTable = new DataTable();
            using (var adapter = new SqlDataAdapter(query, connection))
            {
                adapter.Fill(dataTable);
            }
            return dataTable;
        }

        private string FetchMinOrderValue(SqlConnection connection, string supplier)
        {
            string query = "SELECT MinPord FROM Supplier WHERE KeyCode = @supplier";
            using (var cmd = new SqlCommand(query, connection))
            {
                cmd.Parameters.AddWithValue("@supplier", supplier);
                return Convert.ToString(cmd.ExecuteScalar());
            }
        }

        private void ClearChartPoints(Chart chart)
        {
            if (chart.Series[1].Points.Count > 0)
            {
                chart.Series[1].Points.Clear();
            }
        }

        private void ValidateOrderTotal()
        {
            if (decimal.TryParse(suggestedOrderTotalTextBox.Text, out var suggestedTotal) &&
                decimal.TryParse(minOrderTextBox.Text, out var minOrder) &&
                suggestedTotal < minOrder)
            {
                suggestedOrderTotalTextBox.BackColor = Color.Red;
                MessageBox.Show("This suggested order does not meet the minimum purchase requirement.");
            }
        }

        private string GenerateQueryForDt7(string supplier, string branch)
        {
            return $@"
-- Temporary table for PartsCTE
CREATE TABLE #PartsCTE (Part NVARCHAR(100));

-- Insert data into PartsCTE
INSERT INTO #PartsCTE (Part)
SELECT R.Part
FROM Review R WITH (NOLOCK)
WHERE R.Supplier = '{supplier}' AND R.Branch = '{branch}' AND R.RndQty > 0;

-- Temporary table for CTE
CREATE TABLE #CTE (
    Branch NVARCHAR(50),
    RndQty INT,
    CalcQty DECIMAL(18,2),
    [Group] NVARCHAR(50),
    Part NVARCHAR(100),
    PDesc NVARCHAR(255),
    Code1 NVARCHAR(50),
    BoxQty INT,
    BOQty INT,
    POQty INT,
    VendorWhse NVARCHAR(50),
    SuggestTotal DECIMAL(18,2),
    CaseQty INT,
    PalletQty INT,
    Code3 NVARCHAR(50)
);

-- Insert data into CTE
INSERT INTO #CTE
SELECT R.Branch, R.RndQty, ISNULL(R.CalcQty, 0) as CalcQty, R.[Group], R.Part, R.PDesc, R.Code1, ISNULL(NULLIF(P.BoxQty, ''), 1) AS BoxQty, R.BOQty, R.POQty, P.VendorWhse,
    CAST(SUM(R.RndQty * R.Unit) AS DECIMAL(18,2)) AS SuggestTotal, ISNULL(P.CaseQty, 0) AS CaseQty, P.PalletQty, P.Code3
FROM Review R WITH (NOLOCK)
INNER JOIN VW_MERI_Pri P WITH (NOLOCK) ON R.Part = P.Part
WHERE R.Supplier = '{supplier}' AND R.Branch = '{branch}' AND R.RndQty > 0 AND P.Qty = 0
GROUP BY R.Branch, R.RndQty, R.CalcQty, R.[Group], R.Part, R.PDesc, R.Code1, P.BoxQty, R.BOQty, R.POQty, P.VendorWhse, P.CaseQty, P.PalletQty, P.Code3;

-- Temporary table for CTE2
CREATE TABLE #CTE2 (
    Part NVARCHAR(100),
    [0ago] int
);

-- Insert data into CTE2
INSERT INTO #CTE2
--SELECT Part, CAST(SUM(Usage) as int) FROM MERI_USAGES WHERE 
--Branch IN (SELECT Branch FROM Branches WHERE BranchGroup = '{branch}') AND 
--Part IN (SELECT Part FROM Review WHERE Supplier = '{supplier}' AND Branch = '{branch}') AND 
--[Period] = (SELECT MAX([Period]) FROM MERI_Usages)
--GROUP BY Part
SELECT 
    R.Part, 
    ISNULL(CAST(SUM(U.Usage) AS INT),0) AS TotalUsage
FROM 
    Review R
LEFT OUTER JOIN 
    MERI_Usages U 
ON 
    R.Part = U.Part 
    AND U.[Period] = (SELECT MAX([Period]) FROM MERI_Usages) 
    AND U.Branch IN (SELECT Branch FROM Branches WHERE BranchGroup = '{branch}')
WHERE 
    R.Supplier = '{supplier}' 
    AND R.Branch = '{branch}'
GROUP BY 
    R.Part


-- Temporary table for CTE3
CREATE TABLE #CTE3 (
    Part NVARCHAR(100),
    [1ago] int,
    [2ago] int,
    [3ago] int,
    LocalUsageLast6 int,
    LocalUsageLast12 int);

-- Insert data into CTE3
INSERT INTO #CTE3
SELECT 
    U.Part,
    SUM(CASE 
        WHEN CAST(LEFT(U.[Period], 4) + '-' + RIGHT(U.[Period], 2) + '-01' AS DATE) = DATEADD(MONTH, -1, DATEFROMPARTS(YEAR(GETDATE()), MONTH(GETDATE()), 1))
        THEN U.Usage 
        ELSE 0 
    END) AS '1ago', -- Last month's usage
    SUM(CASE 
        WHEN CAST(LEFT(U.[Period], 4) + '-' + RIGHT(U.[Period], 2) + '-01' AS DATE) = DATEADD(MONTH, -2, DATEFROMPARTS(YEAR(GETDATE()), MONTH(GETDATE()), 1))
        THEN U.Usage 
        ELSE 0 
    END) AS '2ago', -- Usage 2 months ago
    SUM(CASE 
        WHEN CAST(LEFT(U.[Period], 4) + '-' + RIGHT(U.[Period], 2) + '-01' AS DATE) = DATEADD(MONTH, -3, DATEFROMPARTS(YEAR(GETDATE()), MONTH(GETDATE()), 1))
        THEN U.Usage 
        ELSE 0 
    END) AS '3ago', -- Usage 3 months ago
    SUM(CASE 
        WHEN CAST(LEFT(U.[Period], 4) + '-' + RIGHT(U.[Period], 2) + '-01' AS DATE) >= DATEADD(MONTH, -6, DATEFROMPARTS(YEAR(GETDATE()), MONTH(GETDATE()), 1))
        THEN U.Usage 
        ELSE 0 
    END) AS UsageLast6, -- Usage 6 months ago
    SUM(CASE 
        WHEN CAST(LEFT(U.[Period], 4) + '-' + RIGHT(U.[Period], 2) + '-01' AS DATE) >= DATEADD(MONTH, -12, DATEFROMPARTS(YEAR(GETDATE()), MONTH(GETDATE()), 1))
        THEN U.Usage 
        ELSE 0 
    END) AS UsageLast12 -- Usage 12 months ago
    --SUM(CASE 
    --    WHEN CAST(LEFT(U.[Period], 4) + '-' + RIGHT(U.[Period], 2) + '-01' AS DATE) = DATEADD(YEAR, -1, DATEFROMPARTS(YEAR(GETDATE()), MONTH(GETDATE()), 1))
    --    THEN U.Usage 
    --    ELSE 0 
    --END) AS UsageLastYear -- Usage 1 year ago
FROM 
    MERI_Usages U WITH (NOLOCK) 
WHERE
    U.Part IN (SELECT R.Part FROM Review R WHERE R.Supplier = '{supplier}' AND R.Branch = '{branch}' AND R.RndQty > 0)
    AND U.Branch IN (SELECT Branch FROM Branches WHERE BranchGroup = '{branch}')
GROUP BY
    U.Part;

-- Temporary table for CTE4
CREATE TABLE #CTE4 (
    Part NVARCHAR(100),
    LastRFV DATETIME
);

-- Insert data into CTE4
INSERT INTO #CTE4
SELECT R.Part, MAX(R.DateTime) AS LastRFV 
FROM RLines R WITH (NOLOCK)
WHERE R.Branch = '{branch}' AND R.Document IN (SELECT Document FROM RHeads WHERE Supp = '{supplier}')
GROUP BY R.Part;

-- Temporary table for CTE5
CREATE TABLE #CTE5 (
    Part NVARCHAR(100),
    FulSales INT
);

-- Insert data into CTE5
INSERT INTO #CTE5
SELECT Part, CAST(SUM(Qty) AS INT) AS FulSales 
FROM VW_MERI_I WITH (NOLOCK)
WHERE DateTime > DATEADD(MONTH, -15, GETDATE()) AND Rep = '30FUL' AND Part IN (SELECT Part FROM #PartsCTE)
GROUP BY Part;

-- Final query to join all the components

SELECT 
    I.RndQty, I.CalcQty, I.[Group], I.Part, I.PDesc, I.Code1, I.BoxQty, 
    CAST(ISNULL(II.[0ago], 0) as int) AS [0ago], CAST(ISNULL(III.[1ago], 0) as int) AS [1ago], 
    CAST(ISNULL(III.[2ago], 0) as int) AS [2ago], CAST(ISNULL(III.[3ago], 0) as int) AS [3ago], 
    CAST(ISNULL(III.LocalUsageLast6, 0) as int) AS LocalUsageLast6, 
    CAST(ISNULL(III.LocalUsageLast12, 0) as int) AS LocalUsageLast12, 
    I.BOQty, I.POQty, I.VendorWhse, I.SuggestTotal, 
    CASE WHEN I.CaseQty IS NULL THEN 0 ELSE CAST(I.CaseQty AS INT) END as CaseQty, 
    I.PalletQty, IV.LastRFV, V.FulSales, I.Code3
FROM #CTE I 
LEFT JOIN #CTE2 II ON I.Part = II.Part 
LEFT JOIN #CTE3 III ON I.Part = III.Part 
LEFT JOIN #CTE4 IV ON I.Part = IV.Part 
LEFT JOIN #CTE5 V ON I.Part = V.Part
GROUP BY 
    I.RndQty, I.CalcQty, I.[Group], I.Part, I.PDesc, I.Code1, I.BoxQty, 
    II.[0ago], III.[1ago], III.[2ago], III.[3ago], I.BOQty, I.POQty, I.VendorWhse, 
    I.SuggestTotal, I.CaseQty, I.PalletQty, IV.LastRFV, V.FulSales, I.Code3, LocalUsageLast6, LocalUsageLast12
ORDER BY I.VendorWhse, I.Part;

DROP TABLE #PartsCTE;
DROP TABLE #CTE;
DROP TABLE #CTE2;
DROP TABLE #CTE3;
DROP TABLE #CTE4;
DROP TABLE #CTE5;";
        }

        private string GenerateQueryForDt8(string supplier, string branch)
        {
            return $@"
 ;WITH 
                        CTE(Branch, RndQty, CalcQty, [Group], Part, PDesc, Code1, BoxQty, VendorWhse, SuggestTotal, CaseQty, PalletQty, Code3) as (
                        SELECT R.Branch, R.RndQty, R.CalcQty, R.[Group], R.Part, R.PDesc, R.Code1, CASE WHEN P.BoxQty = '' THEN 1 ELSE P.BoxQty end as BoxQty, P.VendorWhse, CAST(SUM(R.RndQty * R.Unit) as decimal(18,2)), case when P.CaseQty IS NULL THEN 0 else P.CaseQty END as CaseQty, P.PalletQty, P.Code3
                        FROM Review R WITH (NOLOCK) INNER JOIN 
                        VW_MERI_Pri P WITH (NOLOCK) ON R.Part = P.Part
                        WHERE R.Supplier = '{supplier}' AND R.Branch = '{branch}' AND R.RndQty > 0 AND P.Qty = 0
                        GROUP BY R.Branch, R.RndQty, R.CalcQty, R.[Group], R.Part, R.PDesc, R.Code1, P.BoxQty, P.VendorWhse, P.CaseQty, P.PalletQty, P.Code3
                        ),

                        CTE1(Part, [0ago]) as (
SELECT 
    R.Part, 
    ISNULL(CAST(SUM(U.Usage) AS INT),0) AS TotalUsage
FROM 
    Review R
LEFT OUTER JOIN 
    MERI_Usages U 
ON 
    R.Part = U.Part 
    AND U.[Period] = (SELECT MAX([Period]) FROM MERI_Usages) 
    --AND U.Branch IN (SELECT Branch FROM Branches WHERE BranchGroup = '{branch}')
WHERE 
    R.Supplier = '{supplier}'
    AND R.Branch = '{branch}'
GROUP BY 
    R.Part
                        ),

                        CTE2(Part,[1ago],[2ago],[3ago], LocalUsageLast6, LocalUsageLast12) as (
SELECT 
    U.Part,
    SUM(CASE 
        WHEN CAST(LEFT(U.[Period], 4) + '-' + RIGHT(U.[Period], 2) + '-01' AS DATE) = DATEADD(MONTH, -1, DATEFROMPARTS(YEAR(GETDATE()), MONTH(GETDATE()), 1))
        THEN U.Usage 
        ELSE 0 
    END) AS '1ago', -- Last month's usage
    SUM(CASE 
        WHEN CAST(LEFT(U.[Period], 4) + '-' + RIGHT(U.[Period], 2) + '-01' AS DATE) = DATEADD(MONTH, -2, DATEFROMPARTS(YEAR(GETDATE()), MONTH(GETDATE()), 1))
        THEN U.Usage 
        ELSE 0 
    END) AS '2ago', -- Usage 2 months ago
    SUM(CASE 
        WHEN CAST(LEFT(U.[Period], 4) + '-' + RIGHT(U.[Period], 2) + '-01' AS DATE) = DATEADD(MONTH, -3, DATEFROMPARTS(YEAR(GETDATE()), MONTH(GETDATE()), 1))
        THEN U.Usage 
        ELSE 0 
    END) AS '3ago', -- Usage 3 months ago
    SUM(CASE 
        WHEN CAST(LEFT(U.[Period], 4) + '-' + RIGHT(U.[Period], 2) + '-01' AS DATE) >= DATEADD(MONTH, -6, DATEFROMPARTS(YEAR(GETDATE()), MONTH(GETDATE()), 1))
        THEN U.Usage 
        ELSE 0 
    END) AS UsageLast6, -- Usage 6 months ago
    SUM(CASE 
        WHEN CAST(LEFT(U.[Period], 4) + '-' + RIGHT(U.[Period], 2) + '-01' AS DATE) >= DATEADD(MONTH, -12, DATEFROMPARTS(YEAR(GETDATE()), MONTH(GETDATE()), 1))
        THEN U.Usage 
        ELSE 0 
    END) AS UsageLast12 -- Usage 12 months ago
    --SUM(CASE 
    --    WHEN CAST(LEFT(U.[Period], 4) + '-' + RIGHT(U.[Period], 2) + '-01' AS DATE) = DATEADD(YEAR, -1, DATEFROMPARTS(YEAR(GETDATE()), MONTH(GETDATE()), 1))
    --    THEN U.Usage 
    --    ELSE 0 
    --END) AS UsageLastYear -- Usage 1 year ago
FROM 

    MERI_Usages U 
    --AND U.Branch IN (SELECT Branch FROM Branches WHERE BranchGroup = '{branch}')
WHERE 
    U.Part IN (SELECT Part FROM Review R WHERE R.Supplier = '{supplier}' 
    AND R.Branch = '{branch}' 
    AND R.RndQty > 0)
GROUP BY 
    U.Part

                        )

                        SELECT I.RndQty, I.CalcQty, I.[Group], I.Part, I.PDesc, I.Code1, I.BoxQty, CAST(ISNULL(Z.[0ago],0) as int) as '0ago', CAST(ISNULL(II.[1ago],0) as int) as '1ago', CAST(ISNULL(II.[2ago],0) as int) as '2ago', CAST(ISNULL(II.[3ago],0) as int) as '3ago', CAST(ISNULL(II.LocalUsageLast6,0) as int) as LocalUsageLast6, 
                        CAST(ISNULL(II.LocalUsageLast12,0) as int) as LocalUsageLast12, I.VendorWhse, I.SuggestTotal, I.CaseQty, I.PalletQty, I.Code3
                        FROM CTE I LEFT OUTER JOIN
                        CTE1 Z on I.Part = Z.Part INNER JOIN 
                        CTE2 II ON I.Part = II.Part
                        GROUP BY I.RndQty, I.CalcQty, I.[Group], I.Part, I.PDesc, I.Code1, I.BoxQty, LocalUsageLast6, Z.[0ago], II.[1ago], II.[2ago], II.[3ago], II.LocalUsageLast12, I.VendorWhse, I.SuggestTotal, I.CaseQty, I.PalletQty, I.Code3
                        ORDER BY I.VendorWhse, I.Part";
        }

        public Size oldSize;
        private void SOR_Load(object sender, EventArgs e) => oldSize = base.Size;

        protected override void OnResize(System.EventArgs e)
        {
            base.OnResize(e);

            foreach (Control cnt in this.Controls)
                ResizeAll(cnt, base.Size);

            oldSize = base.Size;
        }

        private void ResizeAll(Control control, Size newSize)
        {
            int width = newSize.Width - oldSize.Width;
            control.Left += (control.Left * width) / oldSize.Width;
            control.Width += (control.Width * width) / oldSize.Width;

            int height = newSize.Height - oldSize.Height;
            control.Top += (control.Top * height) / oldSize.Height;
            control.Height += (control.Height * height) / oldSize.Height;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            // switch between local and total usage
            if (usageState == "total")
            {
                oneAgoTextBox.Text = dt7.Rows[q]["0ago"].ToString();
                twoAgoTextBox.Text = dt7.Rows[q]["1ago"].ToString();
                threeAgoTextBox.Text = dt7.Rows[q]["2ago"].ToString();
                fourAgoTextBox.Text = dt7.Rows[q]["3ago"].ToString();
                usageLast6Textbox.Text = dt7.Rows[q]["LocalUsageLast6"].ToString();
                usageLast12Textbox.Text = dt7.Rows[q]["LocalUsageLast12"].ToString();
                //usageLastYearTextbox.Text = dt7.Rows[q]["LocalUsageLastYear"].ToString();
                branchGroupButton.Text = "Current: Group usage";
                branchGroupButton.BackColor = Color.White;
                branchGroupButton.ForeColor = Color.Black;
                usage6Label.Text = "Group Usage Last 6:";
                usage12Label.Text = "Group Usage Last 12:";
                usageLastYear.Text = $@"Group Usage {Convert.ToInt32(DateTime.Now.Year) - 1}:";
                usageState = "local";
            }
            else
            {
                usage6Label.Text = "Total Usage Last 6:";
                usage12Label.Text = "Total Usage Last 12:";
                usageLastYear.Text = $@"Total Usage {Convert.ToInt32(DateTime.Now.Year) - 1}:";
                oneAgoTextBox.Text = dt8.Rows[q]["0ago"].ToString();
                twoAgoTextBox.Text = dt8.Rows[q]["1ago"].ToString();
                threeAgoTextBox.Text = dt8.Rows[q]["2ago"].ToString();
                fourAgoTextBox.Text = dt8.Rows[q]["3ago"].ToString();
                usageLast6Textbox.Text = dt8.Rows[q]["LocalUsageLast6"].ToString();
                usageLast12Textbox.Text = dt8.Rows[q]["LocalUsageLast12"].ToString();
                //usageLastYearTextbox.Text = dt8.Rows[q]["LocalUsageLastYear"].ToString();
                branchGroupButton.Text = "Current: Total usage";
                branchGroupButton.BackColor = Color.Black;
                branchGroupButton.ForeColor = Color.White;
                usageState = "total";
            }

        }

        private void SOR_KeyPress(object sender, KeyEventArgs e)
        {
            if (e.Handled != true)
            {
                if (e.KeyCode == Keys.F9)
                {
                    f9Pressed = true;
                    F9Pressed();
                    e.Handled = true;
                }

                if (e.KeyCode == Keys.F8)
                {
                    f8Pressed = true;
                    F8Pressed();
                    e.Handled = true;
                }
            }
        }

        public void F9Pressed()
        {
            if (f9Pressed == false)
            {
                return;
            }
            if (nextButton.Enabled == false)
            {
                return;
            }

            double qty = Convert.ToDouble(orderQtyTextBox.Text) / Convert.ToDouble(boxQtyTextBox.Text);
            if (qty != (int)qty)
            {
                dialogResult = MessageBox.Show("The order amount endered is not an increment of the box quantity. Do you wish to continue?", "Important Notice!", MessageBoxButtons.YesNo);
                if (dialogResult == DialogResult.No)
                {
                    orderQtyTextBox.Text = Convert.ToInt32(dt7.Rows[q]["RndQty"]).ToString();
                    orderQtyTextBox.Focus();
                    orderQtyTextBox.SelectAll();
                    return;
                }
            }

            dt7.Rows[q]["RndQty"] = orderQtyTextBox.Text;
            if (pbQtyTextBox.Text != "")
            {
                if (Convert.ToInt32(pbQtyTextBox.Text) < Convert.ToInt32(orderQtyTextBox.Text))
                {
                    runningTotal += Convert.ToDouble(pbPriceTextBox.Text) * Convert.ToDouble(orderQtyTextBox.Text);
                    runningTotalLabel.Text = Convert.ToDecimal(runningTotal).ToString();
                }
                else
                {
                    runningTotal += Convert.ToDouble(costTextBox.Text) * Convert.ToDouble(orderQtyTextBox.Text);
                    runningTotalLabel.Text = Convert.ToDecimal(runningTotal).ToString();
                }
            }
            else
            {
                runningTotal += Convert.ToDouble(costTextBox.Text) * Convert.ToDouble(orderQtyTextBox.Text);
                runningTotalLabel.Text = Convert.ToDecimal(runningTotal).ToString();
            }

            DataRow newItem = dtExport.NewRow();
            newItem[0] = partTextbox.Text.ToString();
            newItem[1] = orderQtyTextBox.Text.ToString();
            dtExport.Rows.Add(newItem);

            q++;
            numberOfItemsLabel.Text = $"Item {q + 1} of {dt7.Rows.Count}";

            //GetBranchData();
            GetPartData();
            if (found == false)
            {
                return;
            }
            getBranchStockData();
            getIOTdata();
            loadDataGridView();
            if (chart1.Series[1].Points.Count > 0)
            {
                for (int i = 0; i < chart1.Series[1].Points.Count; i++)
                {
                    chart1.Series[1].Points.RemoveAt(i);
                }
            }

            fillGraph();
            fillGraph1();

            if (q > 0)
            {
                suggestedOrderTotalTextBox.Visible = false;
                orderTotalLabel.Visible = false;
            }

            orderQtyTextBox.Focus();
            orderQtyTextBox.SelectAll();
            f9Pressed = false;
        }

        public void F8Pressed()
        {
            if (f8Pressed == false)
            {
                return;
            }
            if (prevButton.Enabled == false)
            {
                return;
            }

            dtExport.Rows.RemoveAt(dtExport.Rows.Count - 1);

            q--;
            numberOfItemsLabel.Text = $"Item {q + 1} of {dt7.Rows.Count}";

            GetPartData();
            if (found == false)
            {
                return;
            }
            getBranchStockData();
            getIOTdata();
            loadDataGridView();
            if (chart1.Series[1].Points.Count > 0)
            {
                for (int i = 0; i < chart1.Series[1].Points.Count; i++)
                {
                    chart1.Series[1].Points.RemoveAt(i);
                }
            }

            fillGraph();
            fillGraph1();
            orderQtyTextBox.Focus();
            orderQtyTextBox.SelectAll();
            if (pbQtyTextBox.Text != "")
            {
                if (Convert.ToInt32(pbQtyTextBox.Text) < Convert.ToInt32(orderQtyTextBox.Text))
                {
                    runningTotal -= Math.Round(Convert.ToDouble(pbPriceTextBox.Text) * Convert.ToDouble(orderQtyTextBox.Text), 2);
                    runningTotal = Math.Round(runningTotal, 2);
                    runningTotalLabel.Text = runningTotal.ToString();
                }
                else
                {
                    runningTotal -= Math.Round(Convert.ToDouble(costTextBox.Text) * Convert.ToDouble(orderQtyTextBox.Text), 2);
                    runningTotal = Math.Round(runningTotal, 2);
                    runningTotalLabel.Text = runningTotal.ToString();
                }
            }
            else
            {
                runningTotal -= Math.Round(Convert.ToDouble(costTextBox.Text) * Convert.ToDouble(orderQtyTextBox.Text), 2);
                runningTotal = Math.Round(runningTotal, 2);
                runningTotalLabel.Text = runningTotal.ToString();
            }
            f8Pressed = false;

            if (q == 0)
            {
                suggestedOrderTotalTextBox.Visible = true;
                orderTotalLabel.Visible = true;
            }

            f8Pressed = false;
        }

        private void DeleteOrderButton_Click(object sender, EventArgs e)
        {
            if (q > 0)
            {
                dialogResult = MessageBox.Show($"This will delete this order. The changes made up to this point will not be saved. Continue?\n \n{q} reviewed items will be lost.", "Important Notice!", MessageBoxButtons.YesNo);
            }
            else
            {
                dialogResult = MessageBox.Show("Delete this order?", "Important Notice!", MessageBoxButtons.YesNo);
            }
            if (dialogResult == DialogResult.Yes)
            {
                System.Windows.Forms.Cursor.Current = Cursors.WaitCursor;
                q = 0;
                var commands = Convert.ToString(listSuppliers.Text).Split(' ', (char)2);
                var selectedSupplier = commands[0];
                var branch = branchTextbox.Text;

                int y = listSuppliers.SelectedIndex;

                DataRow dr = dtSuppliers.Rows[y];
                dr.Delete();
                dtSuppliers.AcceptChanges();

                listSuppliers.SelectedIndex = 0;

                for (int x = 0; x <= dt7.Rows.Count - 1; x++)
                {
                    string part = dt7.Rows[x]["Part"].ToString();
                    SqlConnection conn = new SqlConnection(Helper.ConnString("AUTOPART"));

                    using (SqlCommand cmd = new SqlCommand("sp_MERI_updateReviewfromApparatus", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        //set new Min
                        cmd.Parameters.Add("@Part", SqlDbType.VarChar).Value = part;

                        //set new Max
                        cmd.Parameters.Add("@Branch", SqlDbType.VarChar).Value = branch;

                        //set PerCar
                        cmd.Parameters.Add("@Supplier", SqlDbType.VarChar).Value = selectedSupplier;

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
                }


                dtExport.Clear();

                dt1.Clear();
                dt2.Clear();
                dt3.Clear();
                dt5.Clear();
                dt6.Clear();
                dt7.Clear();
                dt8.Clear();

                foreach (Control ctrl in ActiveForm.Controls)
                {
                    if (ctrl is TextBox)
                    {
                        ((TextBox)(ctrl)).Text = "";
                    }
                }
                dataGridView1.DataSource = null;
                foreach (var series in chart1.Series)
                {
                    series.Points.Clear();
                }

                supplier = "";

                numberOfItemsLabel.Visible = false;
                usage6Label.Text = "Local Usage Last 6:";
                usage12Label.Text = "Local Usage Last 12:";
                usageLastYear.Text = $@"Local Usage {Convert.ToInt32(DateTime.Now.Year) - 1}:";
                oneAgoLabel.Text = DateTime.Now.AddMonths(-0).ToString("MMMM");
                twoAgoLabel.Text = DateTime.Now.AddMonths(-1).ToString("MMMM");
                threeAgoLabel.Text = DateTime.Now.AddMonths(-2).ToString("MMMM");
                fourAgoLabel.Text = DateTime.Now.AddMonths(-3).ToString("MMMM");
                if (q == 0)
                {
                    prevButton.Enabled = false;
                }

                listSuppliers.Select();
                runningTotalLabel.Text = "0.00";
                runningTotal = 0;
                System.Windows.Forms.Cursor.Current = Cursors.Default;
            }
            else
            {
                return;
            }
        }

        private void SOR_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F9)
            {
                f9Pressed = false;
            }
            if (e.KeyCode == Keys.F8)
            {
                f8Pressed = false;
            }
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            if (q > 0)
            {
                dialogResult = MessageBox.Show("Are you sure you want to cancel? The order will remain, but you will lose all of your progress.", "Important Notice!", MessageBoxButtons.YesNo);
                if (dialogResult == DialogResult.Yes)
                {
                    supplier = "";
                    firstSearch = true;
                    Utilities.ResetAllControls(this);
                    chart1.DataSource = null;
                    numberOfItemsLabel.Text = "Item ? of items ??";
                    cancelButton.Enabled = false;
                }
                else
                {
                    return;
                }
            }
            else
            {
                supplier = "";
                firstSearch = true;
                runningTotalLabel.Text = "0.00";
                runningTotal = 0;
                Utilities.ResetAllControls(this);
                chart1.DataSource = null;
                numberOfItemsLabel.Text = "Item ? of items ??";
                cancelButton.Enabled = false;
            }
        }
    }
    public class Utilities
    {
        public static void ResetAllControls(Control form)
        {
            foreach (Control control in form.Controls)
            {
                if (control is TextBox)
                {
                    TextBox textBox = (TextBox)control;
                    textBox.Text = null;
                    textBox.BackColor = Color.Empty;
                }

                if (control is ComboBox)
                {
                    ComboBox comboBox = (ComboBox)control;
                    if (comboBox.Items.Count > 0)
                        comboBox.SelectedIndex = 0;
                }


                if (control is DataGridView)
                {
                    DataGridView gridView = (DataGridView)control;
                    //gridView.Rows.Clear();
                    gridView.DataSource = null;
                }

                if (control is ListBox)
                {
                    ListBox listBox = (ListBox)control;
                    listBox.ClearSelected();
                }
            }
        }
    }
}

