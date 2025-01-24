using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace MinMaxAttendant
{
    public partial class MinMaxAttendant : Form
    {
        public DataTable dt = new DataTable();
        public DataTable dt1 = new DataTable();
        public DataTable dt2 = new DataTable();
        public string items;
        public System.Timers.Timer timer1 = new System.Timers.Timer();
        public Timer timer0;
        public MinMaxAttendant()
        {
            InitializeComponent();
            this.CenterToScreen();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            label1.Text = "Waiting...";
            timer0 = new Timer();
            timer0.Tick += new EventHandler(reload_Button);
            timer0.Interval = 600000; //(in miliseconds)
            //timer0.AutoReset = false;
            timer0.Start();
        }


        private void reload_Button(object sender, EventArgs e)
        {
            label1.Text = "Cleaning..."; 
            dt.Rows.Clear();
            SqlConnection conn = new SqlConnection(Helper.ConnString("AUTOPART"));
            conn.Open();
            string query = ($@"
                                UPDATE MERI_MinMaxChanges 
                                SET Completed = 'Y', CompletedInits = 'AUTO', NOTES = 'EMAIL' 
                                FROM MERI_MinMaxChanges MNC 
                                INNER JOIN VW_MERI_Pro P ON MNC.Part = P.Part AND MNC.Branch = P.Branch 
                                WHERE (MNC.NewMin = P.Min OR MNC.NewMax = P.Max) AND MNC.Completed = ''
                            ");
            SqlCommand cmd = new SqlCommand(query, conn);
            cmd.ExecuteNonQuery();
            conn.Close();
            timer0.Stop();
            timer0.Start();
            label1.Text = "Waiting...";
        }
    }
}
