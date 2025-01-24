using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using System.Text;
using System.Windows.Forms;
namespace AutologuePricing
{
    public partial class AutologuePriceBuilder : Form
    {
        List<ALItems> ALItems = new List<ALItems>();
        public DataTable table = new DataTable();
        public AutologuePriceBuilder()
        {
            InitializeComponent();
            this.CenterToScreen();
        }

        private void quitButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void addButton_Click(object sender, EventArgs e)
        {
            //SQL to check if item is PG
            DataAccess db = new DataAccess();

            pgTextBox.Text = pgTextBox.Text.ToUpper();
            ALItems = db.GetItems(pgTextBox.Text);

            if (ALItems.Count == 0)
            {
                MessageBox.Show($"\"{ pgTextBox.Text }\" is not a valid product group!");
                pgListBox.Text = "";
                return;
            }
            if (string.IsNullOrEmpty(pgTextBox.Text))
            {
                MessageBox.Show("You must specify a Product Group");
            }
            else
            {
                pgListBox.Items.Add(pgTextBox.Text);
                pgTextBox.Text = "";
            }
        }

        private void removeButton_Click(object sender, EventArgs e)
        {
            int i = 0;
            int ITR = pgListBox.SelectedItems.Count;
            do
            {
                pgListBox.Items.Remove(pgListBox.SelectedItem);
                i++;
            } while (i < ITR);
        }

        private void pgTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                addButton_Click((object)sender, (EventArgs)e);
            }
        }

        private void goButton_Click(object sender, EventArgs e)
        {

            Cursor.Current = Cursors.WaitCursor;
            SqlConnection conn = new SqlConnection(Helper.ConnString("AUTOPART"));
            conn.Open();
            string pg = string.Empty;
            for (int i = 0; i < pgListBox.Items.Count; i++)
            {
                if (i >= 1)
                {
                    pg = pg + "', '" + pgListBox.Items[i].ToString();
                }
                else
                {
                    pg = pg + pgListBox.Items[i].ToString();
                }
            }

            //if(pgListBox.Items.Count == 1)
            //{
            //    pg = pg.Substring(pg.Length-2);
            //}

            pg = "'" + pg + "'";

            string query = ($@"
                                SELECT SUBSTRING(Range,4,99) as 'CAT', 
                                PG as 'LINE', 
                                SUBSTRING(Part,4,99) as PART, 
                                [Description] as 'DESC', 
                                A1 as 'P1', 
                                A2 as 'P2', 
                                A3 as 'P3', 
                                A4 as 'P4', 
                                P9, 
                                CorePrice as 'CORE' 
                    
                                FROM VW_MERI_Pri 
                                WHERE PG IN({pg})
                             ");

            SqlCommand cmd = new SqlCommand(query, conn);
            using (SqlDataAdapter a = new SqlDataAdapter(cmd))
            {
                //add row number
                //table.Columns.Add("#", typeof(int));
                //table.Columns[0].AutoIncrement = true;
                //table.Columns[0].AutoIncrementSeed = 1;
                //table.Columns[0].AutoIncrementStep = 1;
                //
                a.Fill(table);
                dataGridView1.DataSource = table;
                //dataGridView1.Columns[0].SortMode = DataGridViewColumnSortMode.NotSortable;

            }
            Cursor.Current = Cursors.Default;

            //int x = 0;
            string username1 = System.Security.Principal.WindowsIdentity.GetCurrent().Name;
            int SLength = username1.Length;
            string username = username1.Substring(10, (username1.Length - 10));

            string subPath = @"C:\Users\" + username + @"\Documents\AutologPriceFiles\"; ; // Your code goes here
            bool exists = Directory.Exists(subPath);

            if (!exists)
                Directory.CreateDirectory(subPath);

            string currFile = @"C:\Users\" + username + @"\Documents\AutologPriceFiles\AutologuePriceFile.csv";

            Cursor.Current = Cursors.WaitCursor;

            StringBuilder sb = new StringBuilder();

            IEnumerable<string> columnNames = table.Columns.Cast<DataColumn>().
                                                Select(column => column.ColumnName);
            sb.AppendLine(string.Join(",", columnNames));

            foreach (DataRow row in table.Rows)
            {
                IEnumerable<string> fields = row.ItemArray.Select(field => field.ToString());
                sb.AppendLine(string.Join(",", fields));
            }

            File.WriteAllText(currFile, sb.ToString());
            MessageBox.Show("Saved to " + currFile);
            Cursor.Current = Cursors.Default;



            //string receiverEmailId1 = "adodge@arnoldgroupweb.com";
            string receiverEmailId1 = "partsgenie@autologue.com";
            string receiverEmailId2 = "nsimington@arnoldgroupweb.com";
            string senderName = username;
            var toAddress1 = new MailAddress(receiverEmailId1, "Jim");
            var toAddress2 = new MailAddress(receiverEmailId2, "nsimington");
            string body = "Attached is the Autologue Price upate for product groups " + pg + " as of " + DateTime.Now;


            string uname = "arnoldmotorsupply@arnoldgroupweb.com";
            string pword = "Merrillco1141";
            ICredentialsByHost credentials = new NetworkCredential(uname, pword);


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
                mail.Subject = "Autologue Pricing Update";
                mail.To.Add(toAddress1);
                mail.To.Add(toAddress2);
                mail.Body = body;
                Attachment data = new Attachment(
                    @"C:\Users\" + username + @"\Documents\AutologPriceFiles\AutologuePriceFile.csv",
                MediaTypeNames.Application.Octet);
                // your path may look like Server.MapPath("~/file.ABC")
                mail.Attachments.Add(data);
                client.Send(mail);
                MessageBox.Show("The message has been sent.");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Unable to send. Please contact your IT department.", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            
        }

    }
}
