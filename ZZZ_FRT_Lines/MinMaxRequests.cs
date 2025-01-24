using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;
using Microsoft.VisualBasic;



namespace MinMaxRequests
{
    public partial class MinMaxRequests : Form
    {
        public DataTable table = new DataTable();
        public DataTable dt = new DataTable();
        //public DataTable dt1 = new DataTable();
        public DataTable dt2 = new DataTable();
        public DataTable dt3 = new DataTable();
        public DataTable dt4 = new DataTable();
        public DataTable dt5 = new DataTable();
        //public DataTable dt6 = new DataTable();
        public DataTable dt7 = new DataTable();
        public DataSet ds = new DataSet();
        private bool loaded = false;
        private Nullable<int> Min = null;
        private Nullable<int> Max = null;
        private Nullable<int> PerCar = null;
        public string mnFlag = "";
        public int q = 0;
        private string user = System.Security.Principal.WindowsIdentity.GetCurrent().Name.Replace(@"MERRILLCO\", "");
        public string word0 = "";
        private bool found = true;
        private bool firstSearch = true;
        public string supplier = "";
        public string lastBranch = "";
        public decimal AMU = 0;
        public decimal period;
        public decimal sales;
        public decimal slope;
        private TextBox CurrentTextbox = null;

        public MinMaxRequests()
        {
            InitializeComponent();
            numberOfItemsLabel.Visible = false;
            this.CenterToScreen();
            nextButton.Enabled = false;
            if (q == 0)
            {
                prevButton.Enabled = false;
            }


            SqlConnection conn = new SqlConnection(Helper.ConnString("AUTOPART"));
            {
                conn.Open();
                SqlDataAdapter adapter = new SqlDataAdapter(
                $@"
                            SELECT C.Branch, C.Part, P.PG, P.Range, P.[Desc], C.Comment, C.newMin, C.newMax, C.[User], C.Inits, C.Name, P.Code3 FROM MERI_MinMaxChanges C INNER JOIN Product P ON C.Part = P.KeyCode WHERE Completed != 'Y' ORDER BY C.DateTime ASC
                           ", conn);
                adapter.Fill(dt7);
                if (dt7.Rows.Count > 0)
                {
                    int newMin = Convert.ToInt32(dt7.Rows[0]["newMin"]);
                    int newMax = Convert.ToInt32(dt7.Rows[0]["newMax"].ToString());

                    //branchTextbox.Text = branch;
                    branchTextbox.Text = dt7.Rows[0]["Branch"].ToString();
                    partTextbox.Text = dt7.Rows[0]["Part"].ToString();
                    commentTextBox.Text = dt7.Rows[0]["Comment"].ToString();
                    minTextbox.Text = newMin.ToString();
                    maxTextbox.Text = newMax.ToString();
                    requestedByTextBox.Text = dt7.Rows[0]["User"].ToString();
                    requestedByInits.Text = dt7.Rows[0]["Inits"].ToString();
                    pgTextbox.Text = dt7.Rows[0]["PG"].ToString();
                    descriptionTextbox.Text = dt7.Rows[0]["Desc"].ToString();
                    rangeTextbox.Text = dt7.Rows[0]["Range"].ToString();
                    requestedByNameTextbox.Text = dt7.Rows[0]["Name"].ToString();
                    inventoryTypeTextbox.Text = dt7.Rows[0]["Code3"].ToString();

                    //this.pgListBox.DisplayMember = "Branch";
                    conn.Close();
                    GetBranchData();
                    //GetPartData();
                    getBranchStockData();
                    getIOTdata();
                    loadDataGridView();
                    fillGraph();
                    fillGraph1();

                    if (q == dt7.Rows.Count - 1)
                    {
                        nextButton.Enabled = false;
                    }
                    else
                    {
                        nextButton.Enabled = true;
                    }

                    if (dt7.Rows.Count > 0)
                    {
                        numberOfItemsLabel.Text = $"Item {q + 1} of {dt7.Rows.Count}";
                        numberOfItemsLabel.Visible = true;
                    }
                }
            }

        }

        private Size oldSize;
        private void MinMaxRequests_Load(object sender, EventArgs e) => oldSize = base.Size;

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

        private void GetNextPart()
        {
            if (dt7.Rows.Count > 0)
            {
                int newMin = Convert.ToInt32(dt7.Rows[q]["newMin"]);
                int newMax = Convert.ToInt32(dt7.Rows[q]["newMax"].ToString());

                //branchTextbox.Text = branch;
                branchTextbox.Text = dt7.Rows[q]["Branch"].ToString();
                partTextbox.Text = dt7.Rows[q]["Part"].ToString();
                commentTextBox.Text = dt7.Rows[q]["Comment"].ToString();
                minTextbox.Text = newMin.ToString();
                maxTextbox.Text = newMax.ToString();
                requestedByTextBox.Text = dt7.Rows[q]["User"].ToString();
                requestedByInits.Text = dt7.Rows[q]["Inits"].ToString();
                pgTextbox.Text = dt7.Rows[q]["PG"].ToString();
                descriptionTextbox.Text = dt7.Rows[q]["Desc"].ToString();
                rangeTextbox.Text = dt7.Rows[q]["Range"].ToString();
                requestedByNameTextbox.Text = dt7.Rows[q]["Name"].ToString();
            }
        }

        private void GetBranchData()
        {
            //dt.Clear();
            SqlConnection conn = new SqlConnection(Helper.ConnString("AUTOPART"));
            {
                dt.Clear();
                conn.Open();
                //DataSet ds1 = new DataSet();
                var branch = branchTextbox.Text;
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
                    //branchTextbox.Text = branch;
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

            //dt1.Clear();
            SqlConnection conn1 = new SqlConnection(Helper.ConnString("AUTOPART"));
            {
                if (dt7.Rows.Count > 0)
                {
                    int newMin = Convert.ToInt32(dt7.Rows[q]["newMin"]);
                    int newMax = Convert.ToInt32(dt7.Rows[q]["newMax"].ToString());

                    //branchTextbox.Text = branch;
                    branchTextbox.Text = dt7.Rows[q]["Branch"].ToString();
                    partTextbox.Text = dt7.Rows[q]["Part"].ToString();
                    commentTextBox.Text = dt7.Rows[q]["Comment"].ToString();
                    minTextbox.Text = newMin.ToString();
                    maxTextbox.Text = newMax.ToString();
                    requestedByTextBox.Text = dt7.Rows[q]["User"].ToString();
                    requestedByInits.Text = dt7.Rows[q]["Inits"].ToString();
                    pgTextbox.Text = dt7.Rows[q]["PG"].ToString();
                    descriptionTextbox.Text = dt7.Rows[q]["Desc"].ToString();
                    rangeTextbox.Text = dt7.Rows[q]["Range"].ToString();
                    requestedByNameTextbox.Text = dt7.Rows[q]["Name"].ToString();
                    //word0 = dt7.Rows[q]["Part"].ToString();
                    conn1.Close();
                    GetBranchData();
                    //GetPartData();
                    getBranchStockData();
                    getIOTdata();
                    loadDataGridView();
                    fillGraph();
                    fillGraph1();
                }
                else
                {
                    if (firstSearch == false)
                    {
                        MessageBox.Show("no data found");
                        found = false;
                        return;
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
                    SELECT CAST(VIO as int) as VIO, CAST(Vista as int) as Vista, CAST(ADWRegion as int) as ADWRegion, CAST(Blended as DECIMAL(18,2)) as Blended, MinYear, MaxYear, Makes, Models, CAST(PerCar as int) as PerCar FROM MERI_IOT WHERE (Part = '{partTextbox.Text}')
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
                    ;WITH CTE (Branch, Part, Free, BR30Group, BR60Group) as (
                        SELECT Branch, Part, Free, BR30StoresUsageLast15, BR60StoresUsageLast15 FROM VW_MERI_Pro WHERE Branch = '{branch}' AND Part = '{part}'
                    ),

                    CTE2 (Part, StockingBranches) as (
                        SELECT Part, COUNT(Branch) FROM VW_MERI_Pro WHERE Part = '{part}' AND Max > 0 GROUP BY Part
                    ),

                    CTE3 (Part, CompanyStock) as (
                        SELECT Part, SUM(FREE) FROM VW_MERI_Pro WHERE Part = '{part}' GROUP BY Part
                    ),

                    CTE4 (Part, Class, Min, Max, MNFlag, MNDate, BranchBox, PrevDateTime,LocalUsageLast15, TotalUsageLast15, MNnote, PrevMin, PrevMax, FirstStocked, LastMoved, PerCar) as (
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
                        CAST(PerCar as int) as PerCar

                        FROM VW_MERI_Pro WHERE Branch = '{branch}' AND Part = '{part}')

                    SELECT I.Branch, CAST(I.Free as int) as Free, CAST(II.StockingBranches as int) as StockingBranches, CAST(III.CompanyStock as int) as CompanyStock, IV.Class, IV.Min, IV.Max, IV.MNFlag, IV.MNDate, IV.BranchBox, PrevDateTime, IV.LocalUsageLast15, IV.TotalUsageLast15, IV.MNnote, IV.PrevMin, IV.PrevMax, IV.FirstStocked, IV.LastMoved, IV.PerCar, CAST(I.BR30Group as int) as BR30Group, CAST(I.BR60Group as int) as BR60Group FROM CTE as I LEFT OUTER JOIN
                    CTE2 II ON I.Part = II.Part LEFT OUTER JOIN
                    CTE3 III ON I.Part = III.Part LEFT OUTER JOIN
                    CTE4 IV ON I.Part = IV.Part
                   ", conn3);
                adapter3.Fill(dt3);
                if (dt3.Rows.Count > 0)
                {
                    onHandTextbox.Text = dt3.Rows[0]["Free"].ToString();
                    stockingStoresTextbox.Text = dt3.Rows[0]["StockingBranches"].ToString();
                    companyStockTextbox.Text = dt3.Rows[0]["CompanyStock"].ToString();
                    classTextbox.Text = dt3.Rows[0]["Class"].ToString();
                    currentMinTextBox.Text = dt3.Rows[0]["Min"].ToString();
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
                    currentMaxTextBox.Text = dt3.Rows[0]["Max"].ToString();
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
                                SELECT Branch, CAST(Free as int) as Free, CAST(Min as int) as Min, CAST(Max as int) as Max, MNIndicator as MNFlag, CAST(QtyOnPO as int) as QtyOnPo FROM VW_MERI_Pro WHERE Part = '{partTextbox.Text}'
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
            decimal slope = Convert.ToDecimal(slopeTextbox.Text);

            //decimal period = (AMU - target) / slope;
            if (slope != 0)
            {
                decimal AMU = Convert.ToDecimal(totalUsageLast15Textbox.Text) / 15;
                decimal target = .13M;
                period = Math.Round((AMU - target) / Math.Abs(slope), 0);
                sales = Math.Round((period + 1) * (AMU + (.5M * slope * period)), 0);
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

            user = user.Substring(0, 5);

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
                    cmd.Parameters.Add("@MNFlag", SqlDbType.VarChar).Value = "";
                }
                else
                {
                    cmd.Parameters.Add("@MNDate", SqlDbType.VarChar).Value = DateTime.Now.ToString("yyyyMMdd");
                    cmd.Parameters.Add("@MNFlag", SqlDbType.VarChar).Value = mnFlagTextbox.Text;
                }

                //set MinMaxInits
                cmd.Parameters.Add("@MNInits", SqlDbType.VarChar).Value = user;

                //set PrevMin
                if (Min == null) { Min = 0; }
                cmd.Parameters.Add("@PrevMin", SqlDbType.VarChar).Value = Min;

                //set PrevMax
                if (Max == null) { Max = 0; }
                cmd.Parameters.Add("@PrevMax", SqlDbType.VarChar).Value = Max;

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

            conn.Open();
            string query = ($@"
                                UPDATE MERI_MinMaxChanges SET Completed = 'Y', CompletedInits = '{user}', Status = 'A' WHERE Part = '{partTextbox.Text}' AND Branch = '{branchTextbox.Text}'
                            ");

            SqlCommand cmd1 = new SqlCommand(query, conn);
            cmd1.ExecuteNonQuery();

            dt7.Rows[q].Delete();
            MessageBox.Show("Done.");

            if (q == dt7.Rows.Count - 1)
            {
               ClearTextBoxes();
            }
            else
            {
                q++;
            }

            numberOfItemsLabel.Text = $"Item {q + 1} of {dt7.Rows.Count}";

            GetNextPart();
            GetBranchData();
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

            if (q == dt7.Rows.Count - 1)
            {
                nextButton.Enabled = false;
            }
        }

        private void prevButton_Click(object sender, EventArgs e)
        {
            q--;
            numberOfItemsLabel.Text = $"Item {q + 1} of {dt7.Rows.Count}";

            GetNextPart();
            GetBranchData();
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

            if (q == 0)
            {
                prevButton.Enabled = false;
            }
        }

        private void nextButton_Click(object sender, EventArgs e)
        {
            q++;
            numberOfItemsLabel.Text = $"Item {q + 1} of {dt7.Rows.Count}";

            GetNextPart();
            GetBranchData();
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

            if (q == dt7.Rows.Count - 1)
            {
                nextButton.Enabled = false;
            }
        }

        private void removeButton_Click(object sender, EventArgs e)
        {

            string reason = Interaction.InputBox("Specify reason for denial", "Notice", "");

            SqlConnection conn = new SqlConnection(Helper.ConnString("AUTOPART"));
            conn.Open();
            string query1 = ($@"
                                UPDATE MERI_MinMaxChanges SET Completed = 'Y', CompletedInits = '{user}', Status = 'D', Notes = '{reason}' WHERE Part = '{partTextbox.Text}' AND Branch = '{branchTextbox.Text}'
                            ");

            SqlCommand cmd2 = new SqlCommand(query1, conn);
            cmd2.ExecuteNonQuery();

            DataRow dr = dt7.Rows[q];
            dr.Delete();
            dt7.AcceptChanges();
            MessageBox.Show("Done.");


            if (q == dt7.Rows.Count - 1)
            {
                ClearTextBoxes();
            }
            else
            {
                //q++;
            }

            numberOfItemsLabel.Text = $"Item {q + 1} of {dt7.Rows.Count}";

            GetNextPart();
            GetBranchData();
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

            if (q == dt7.Rows.Count - 1)
            {
                nextButton.Enabled = false;
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

    }
}

