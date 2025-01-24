using System;
using System.Drawing;
using System.Windows.Forms;

namespace PasswordForm
{
    public partial class PasswordForm : Form
    {
        public PasswordForm()
        {
            InitializeComponent();
            this.CenterToScreen();
        }

        private void OKButton_Click(object sender, EventArgs e)
        {
            if(PasswordTextBox.Text != "tide")
            {
                MessageBox.Show("Access Denied");
            }
            else
            {
                this.Hide();
                new MonthEndForm.MonthEndForm().Show();
            }
        }

        private void QuitButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void PasswordTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                OKButton_Click(sender, e);
            }
        }

        private void PasswordTextBox_Enter(object sender, EventArgs e)
        {
            PasswordTextBox.BackColor = Color.Yellow;
        }
    }
}
