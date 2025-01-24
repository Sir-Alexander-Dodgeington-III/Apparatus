using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace Login_Form
{

    public partial class LoginForm : Form
    {
        List<addItems> addItems = new List<addItems>();
        public string Operator;
        public string opcode;
        private string opCode
        {
            get { return opcode; }
            set { opCode = Operator; }
        }

        public LoginForm()
        {
            InitializeComponent();
            this.CenterToScreen();
        }

        public void LoginButton_Click(object sender, EventArgs e)
        {
            string username = System.Security.Principal.WindowsIdentity.GetCurrent().Name;
            if (File.Exists(@"C:\Users\" + username + @"\Documents\MyFile.csv"))
            {
                if (string.IsNullOrEmpty(UserTextBox.Text))
                {
                    MessageBox.Show("you must enter a username");
                    return;
                }

                if (string.IsNullOrEmpty(PassTextBox.Text))
                {
                    MessageBox.Show("you must enter a password");
                    return;
                }

                DataAccess db = new DataAccess();
                addItems = db.GetItems(UserTextBox.Text);

                if (addItems.Count < 1)
                {
                    MessageBox.Show("invalid username");
                    return;
                }
            }

            DataAccess1 db1 = new DataAccess1();
            addItems = db1.GetItems(PassTextBox.Text, UserTextBox.Text);
            if (addItems.Count < 1)
            {
                MessageBox.Show("invalid password");
                PassTextBox.Text = "";
                return;
            }

            
            Operator = "'" + UserTextBox.Text + "'";
            GetUSerRoll(Operator);

        }

        public string GetUSerRoll(string Operator)
        {
            //var dataset = new DataSet();

            SqlConnection conn = new SqlConnection(Helper.ConnString("AUTOPART"));
            conn.Open();
            string query = ($@"
                                SELECT TOP(1) RIGHT(a21,1) FROM Codes WHERE Prefix = 'O' AND KeyCode = {Operator}
                            ");
            SqlCommand cmd = new SqlCommand(query, conn);
            string USRoll = (string)cmd.ExecuteScalar();
            //MessageBox.Show(USRoll);
            if (string.Compare(USRoll, "9")==0)
            {
                this.Hide();
                new FormMain9.FormMain9().Show();
            }
            if (string.Compare(USRoll, "8") == 0)
            {
                this.Hide();
                new FormMain8.FormMain8().Show();
            }
            if (string.Compare(USRoll, "7") == 0)
            {
                this.Hide();
                new FormMain7.FormMain7().Show();
            }
            if (string.Compare(USRoll, "6") == 0)
            {
                this.Hide();
                new FormMain6.FormMain6().Show();
            }
            if (string.Compare(USRoll, "5") == 0)
            {
                this.Hide();
                new FormMain3.FormMain3().Show();
            }
            if (string.Compare(USRoll, "4") == 0)
            {
                this.Hide();
                new FormMain3.FormMain3().Show();
            }
            if (string.Compare(USRoll, "3") == 0)
            {
                this.Hide();
                new FormMain3.FormMain3().Show();
            }
            if (string.Compare(USRoll, "2") == 0)
            {
                this.Hide();
                new FormMain3.FormMain3().Show();
            }
            if (string.Compare(USRoll, "1") == 0)
            {
                this.Hide();
                new FormMain3.FormMain3().Show();
            }

            return USRoll;
            
        }

        private void QuitButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void UserTextBox_Enter(object sender, EventArgs e)
        {
            UserTextBox.BackColor = Color.Yellow;
        }

        private void UserTextBox_Leave(object sender, EventArgs e)
        {
            UserTextBox.BackColor = Color.Empty;
        }

        private void PassTextBox_Enter(object sender, EventArgs e)
        {
            PassTextBox.BackColor = Color.Yellow;
        }

        private void PassTextBox_Leave(object sender, EventArgs e)
        {
            PassTextBox.BackColor = Color.Empty;
        }

        private void PassTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                LoginButton_Click(sender, e);
            }
        }
    }
}

