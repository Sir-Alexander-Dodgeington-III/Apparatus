using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;




namespace IOT_Form
{
    public partial class IOT_Form : Form
    {
        public DataTable table = new DataTable();
        public DataTable dt = new DataTable();
        public DataTable dt0 = new DataTable();
        public DataTable dt1 = new DataTable();
        public DataTable dtp = new DataTable();
        public DataTable dt2 = new DataTable();
        public DataTable dt3 = new DataTable();
        public DataTable dt4 = new DataTable();
        public DataTable dt5 = new DataTable();
        public DataTable dt6 = new DataTable();
        public DataSet ds = new DataSet();
        private bool loaded = false;
        private Nullable<int> Min = null;
        private Nullable<int> Max = null;
        private Nullable<int> PerCar = null;
        public string mnFlag = "";
        public string part;
        private string user = System.Security.Principal.WindowsIdentity.GetCurrent().Name.Replace(@"MERRILLCO\", "");
        public string word0 = "";
        private bool found = false;
        public decimal AMU = 0;
        public decimal period;
        public decimal sales;
        public decimal slope;
        private TextBox CurrentTextbox = null;

        public IOT_Form()
        {
            InitializeComponent();
            this.CenterToScreen();
            this.WindowState = FormWindowState.Maximized;
        }

        private void pgTextbox_TextChanged(object sender, EventArgs e)
        {
            if (pgTextbox.Text.Length > 2)
            {
                partTextbox.Focus();
            }
        }

        private void GetBranchData()
        {
            dt.Clear();
            SqlConnection conn = new SqlConnection(Helper.ConnString("AUTOPART"));
            {
                conn.Open();
                //DataSet ds1 = new DataSet();
                SqlDataAdapter adapter = new SqlDataAdapter(
                $@"
                    SELECT Branch, Name, Addrd, Addre FROM Branches WHERE Branch = '{branchTextbox.Text}'
                   ", conn);
                adapter.Fill(dt);
                if (dt.Rows.Count > 0)
                {
                    var branch = dt.Rows[0]["Branch"].ToString();
                    var name = dt.Rows[0]["Name"].ToString();
                    var city = dt.Rows[0]["Addrd"].ToString();
                    var state = dt.Rows[0]["Addre"].ToString();
                    branchTextbox.Text = branch;
                    nameTextbox.Text = name;
                    cityTextbox.Text = city;
                    stateTextbox.Text = state;
                    //this.pgListBox.DisplayMember = "Branch";
                    conn.Close();
                }
                else
                {
                    MessageBox.Show("nothing found");
                    return;
                }
            }
        }

        private void GetPartData()
        {
            dtp.Clear();
            dt1.Clear();
            SqlConnection conn = new SqlConnection(Helper.ConnString("AUTOPART"));
            {
                if (conn.State == ConnectionState.Open)
                {
                    conn.Close();
                }
                conn.Open();
                //DataSet ds1 = new DataSet();
                SqlDataAdapter adapter = new SqlDataAdapter(
                $@"
                    SELECT PG, KeyCode, [Desc], Range, Word0 FROM Product WHERE KeyCode = '{partTextbox.Text}' ORDER BY KeyCode
                   ", conn);
                adapter.Fill(dt1);
                if (dt1.Rows.Count > 0)
                {
                    part = partTextbox.Text;
                    pgTextbox.Text = dt1.Rows[0]["PG"].ToString();
                    partTextbox.Text = dt1.Rows[0]["KeyCode"].ToString();
                    descriptionTextbox.Text = dt1.Rows[0]["Desc"].ToString();
                    rangeTextbox.Text = dt1.Rows[0]["Range"].ToString();
                    word0 = dt1.Rows[0]["Word0"].ToString();
                    conn.Close();
                    found = true;
                }
                else
                {
                    if (conn.State == ConnectionState.Open)
                    {
                        conn.Close();
                    }
                    conn.Open();
                    SqlDataAdapter adapter1 = new SqlDataAdapter(
                    $@"
                    SELECT KeyCode, [Desc], PG, Range, Word0 FROM Product WHERE KeyCode LIKE '%{partTextbox.Text}%' OR Word0 LIKE '%{partTextbox.Text}%' ORDER BY KeyCode
                   ", conn);
                    adapter1.Fill(dtp);
                    if (dtp.Rows.Count == 1)
                    {
                        partTextbox.Text = dtp.Rows[0]["KeyCode"].ToString();
                        pgTextbox.Text = dtp.Rows[0]["PG"].ToString();
                        partTextbox.Text = dtp.Rows[0]["KeyCode"].ToString();
                        descriptionTextbox.Text = dtp.Rows[0]["Desc"].ToString();
                        rangeTextbox.Text = dtp.Rows[0]["Range"].ToString();
                        word0 = dtp.Rows[0]["Word0"].ToString();
                        conn.Close();
                        found = true;
                    }
                    else
                    {
                        dataGridView2.DataSource = null;
                        dataGridView2.AutoGenerateColumns = false;
                        dataGridView2.ColumnCount = 2;

                        dataGridView2.Columns[0].Name = "KeyCode";
                        dataGridView2.Columns[0].HeaderText = "KeyCode";
                        dataGridView2.Columns[0].DataPropertyName = "KeyCode";

                        dataGridView2.Columns[1].HeaderText = "Desc";
                        dataGridView2.Columns[1].Name = "Desc";
                        dataGridView2.Columns[1].DataPropertyName = "Desc";

                        dataGridView2.DataSource = dtp;
                        conn.Close();
                        dataGridView2.Visible = true;
                        dataGridView2.CurrentCell.Selected = false;
                        found = false;
                        //return;
                    }
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
                        MessageBox.Show("No IOT data found for this item");
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
                    ;WITH CTE (Branch, Part, Free) as (
                        SELECT Branch, Part, ISNULL(Free,0) FROM Stock WHERE Branch = '{branch}' AND Part = '{part}'
                    ),

                    CTE2 (Part, StockingBranches) as (
                        SELECT Part, COUNT(Branch) FROM VW_MERI_Pro WHERE Part = '{part}' AND Max > 0 GROUP BY Part
                    ),

                    CTE3 (Part, CompanyStock) as (
                        SELECT Part, SUM(FREE) FROM VW_MERI_Pro WHERE Part = '{part}' GROUP BY Part
                    ),

                    CTE4 (Part, Class, Min, Max, MNFlag, MNDate, BranchBox, PrevDateTime,LocalUsageLast15, TotalUsageLast15, MNnote, PrevMin, PrevMax, FirstStocked, LastMoved, PerCar, QtyOnPO) as (
                        SELECT 
                        Part, 
                        Class, 
                        CAST(Min as int) as Min, 
                        CAST(Max as int) as Max, 
                        MNIndicator, 
                        CAST(MNDate as date) as MNDate, 
                        CAST(BranchBox as int) as BranchBox, 
                        CAST(PrevDateTime as date) as PrevDateTime, 
                        CAST(LocalUsageLast15 as int) as LocalUsageLast15, 
                        CAST(TotalUsageLast15 as int) as TotalUsageLast15, 
                        MNnote, 
                        CAST(PrevMin as int) as PrevMin, 
                        CAST(PrevMax as int) as PrevMax, 
                        CAST(FirstStocked as date) as FirstStocked, 
                        CAST(LastMoved as date) as LastMoved, 
                        CAST(PerCar as int) as PerCar,
                        CAST(QtyOnPO as int) as QtyOnPO
                        FROM VW_MERI_Pro WHERE Branch = '{branch}' AND Part = '{part}')

                    SELECT I.Branch, CAST(I.Free as int) as Free, CAST(II.StockingBranches as int) as StockingBranches, CAST(III.CompanyStock as int) as CompanyStock, IV.Class, IV.Min, IV.Max, IV.MNFlag, IV.MNDate, IV.BranchBox, PrevDateTime, IV.LocalUsageLast15, IV.TotalUsageLast15, IV.MNnote, IV.PrevMin, IV.PrevMax, IV.FirstStocked, IV.LastMoved, IV.PerCar, IV.QtyOnPO 
                    FROM CTE as I LEFT OUTER JOIN
                    CTE2 II ON I.Part = II.Part INNER JOIN
                    CTE3 III ON I.Part = III.Part INNER JOIN
                    CTE4 IV ON I.Part = IV.Part
                   ", conn3);
                adapter3.Fill(dt3);
                if (dt3.Rows.Count > 0)
                {
                    onHandTextbox.Text = dt3.Rows[0]["Free"].ToString();
                    stockingStoresTextbox.Text = dt3.Rows[0]["StockingBranches"].ToString();
                    companyStockTextbox.Text = dt3.Rows[0]["CompanyStock"].ToString();
                    classTextbox.Text = dt3.Rows[0]["Class"].ToString();
                    minTextbox.Text = dt3.Rows[0]["Min"].ToString();
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
                    lastMovedTextbox.Text = dt3.Rows[0]["LastMoved"].ToString();
                    perCarTextbox.Text = dt3.Rows[0]["PerCar"].ToString();
                    qtyOnPo_Textbox.Text = dt3.Rows[0]["QtyOnPO"].ToString();
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
                    MessageBox.Show("nothing found");
                }
            }
        }

        private void loadDataGridView()
        {
            table.Clear();
            dataGridView1.DataSource = null;

            Cursor.Current = Cursors.WaitCursor;
            SqlConnection conn = new SqlConnection(Helper.ConnString("AUTOPART"));
            conn.Open();


            string query = ($@"
                                SELECT Branch, CAST(Free as int) as Free, CAST(Min as int) as Min, CAST(Max as int) as Max, MNIndicator as MNFlag, CAST(QtyOnPO as int) as QtyOnPo, CAST(LocalUsageLast15 as int) as LUL15 FROM VW_MERI_Pro WHERE Part = '{partTextbox.Text}'
                            ");


            SqlCommand cmd = new SqlCommand(query, conn);

            using (SqlDataAdapter a = new SqlDataAdapter(cmd))
            {
                a.Fill(table);
                dataGridView1.DataSource = table;

            }
            Cursor.Current = Cursors.Default;
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

            if (slopeTextbox.Text != "")
            {
                slope = Convert.ToDecimal(slopeTextbox.Text);
            }
            else
            {
                slope = 0;
            }

            if (totalUsageLast15Textbox.Text != "")
            {
                decimal AMU = Convert.ToDecimal(totalUsageLast15Textbox.Text) / 15;
                decimal target = .13M;
                //decimal period = (AMU - target) / slope;
                if (slope != 0)
                {
                    period = Math.Round((AMU - target) / Math.Abs(slope), 0);
                    sales = Math.Round((period + 1) * (AMU + (.5M * slope * period)), 0);
                }
                else
                {
                    period = 0;
                    sales = 0;
                }

            }
            else
            {
                period = 0;
                sales = 0;
            }

            if (slope < 0)
            {
                periodTextbox.Text = period.ToString("#.##");
                salesTextbox.Text = sales.ToString("#.##");
            }
            else
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
            CurrentTextbox = (TextBox)sender;
            if (e.KeyCode == Keys.Enter)
            {
                if (CurrentTextbox.Name == branchTextbox.Name)
                {
                    GetBranchData();
                    partTextbox.Focus();
                }
            }
            if (e.KeyCode == Keys.Enter)
            {
                if (CurrentTextbox.Name == partTextbox.Name)
                {
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
            }
        }

        private void branchTextbox_Enter(object sender, EventArgs e)
        {
            branchTextbox.BackColor = Color.Yellow;
        }

        private void partTextbox_Enter(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(branchTextbox.Text))
            {
                MessageBox.Show("Please specify a branch");
                branchTextbox.Focus();
                return;
            }
            GetBranchData();
            partTextbox.BackColor = Color.Yellow;
        }

        private void branchTextbox_Leave(object sender, EventArgs e)
        {
            branchTextbox.BackColor = Color.Empty;
        }

        private void partTextbox_Leave(object sender, EventArgs e)
        {
            partTextbox.BackColor = Color.Empty;
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            loaded = true;
            if (e.RowIndex >= 0)
            {
                dt.Clear();
                dt1.Clear();
                dt2.Clear();
                dt3.Clear();

                var branch = dataGridView1.Rows[e.RowIndex].Cells[0].Value.ToString();

                branchTextbox.Text = branch;

                GetBranchData();
                GetPartData();
                getBranchStockData();
                getIOTdata();
                loadDataGridView();
            }
        }

        private void closeButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void okButton_Click(object sender, EventArgs e)
        {
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

            mnFlag = mnFlagTextbox.Text;
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

        private void importButton_Click(object sender, EventArgs e)
        {
            new importIOT.importIOT().Show();
        }

        private void dataGridView2_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            partTextbox.Text = dataGridView2.SelectedCells[0].Value.ToString();
            part = dataGridView2.SelectedCells[0].Value.ToString();
            found = true;
            dataGridView2.Visible = false;
            GetPartData();
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

        private Size oldSize;
        private void IOT_Form_Load(object sender, EventArgs e) => oldSize = base.Size;

        protected override void OnResize(EventArgs e)
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
    }
}

