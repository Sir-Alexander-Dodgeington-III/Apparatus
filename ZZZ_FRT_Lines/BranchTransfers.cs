using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BranchTransfer
{
    public partial class BranchTransfer : Form
    {
        private TextBox CurrentTextbox = null;
        public string Acct;
        public DateTime startDate;
        public DateTime endDate;
        public string[] acctArray;
        public string Query;

        public BranchTransfer()
        {
            InitializeComponent();
            this.CenterToScreen();
        }

        private void canelButton_Click(object sender, EventArgs e)
        {
            this.Close();
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

        private void goButton_Click(object sender, EventArgs e)
        {
            if (acctTextbox.Text == "")
            {
                MessageBox.Show("Account can not be left blank");
                return;
            }
            else
            {
                Acct = acctTextbox.Text;
                acctArray = Acct.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                Acct = "";
                foreach (string i in acctArray)
                {
                    Acct += $"'{i}',";
                }
                Acct = Acct.TrimEnd(',');
            }
            if(startDateTextbox.Text == "")
            {
                MessageBox.Show("Start Date can not be left blank");
                return;
            }
            else
            {
                startDate = Convert.ToDateTime(startDateTextbox.Text);
            }
            if(endDateTextbox.Text == "")
            {
                endDate = DateTime.Now;
                endDateTextbox.Text = "NOW";
            }
            else
            {
                endDate = Convert.ToDateTime(endDateTextbox.Text);
            }

                Query = $@"

                            DECLARE @StartDate DateTime
                            DECLARE @EndDate DateTime

                            SET @StartDate = '{startDate}'
                            SET @EndDate = '{endDate}'

                            SELECT L.Branch, H.Document, L.InvInits, H.Acct, L.Part, L.BopDes, L.Qty, L.Unit, H.DateTime, H.COrder, L.Price, L.TrCost 
                            FROM ILines L INNER JOIN IHeads H ON L.Document = H.Document
                            WHERE H.Document LIKE '%T%' AND 
                            H.Document IN (SELECT Document FROM IHeads WHERE Acct IN ({Acct}) AND CAST(DateTime as date) BETWEEN @StartDate AND @EndDate)
                            ORDER BY Branch, DateTime ASC

                        ";
            

            QueryResults.QueryResults qr = new QueryResults.QueryResults(Query);
            qr.Show();


        }
    }
}
