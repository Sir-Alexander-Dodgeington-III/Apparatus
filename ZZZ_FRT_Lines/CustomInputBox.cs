using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ZZZ_FRT_Lines
{
    public partial class CustomInputBox : Form
    {
        public string UserInput { get; private set; }

        public CustomInputBox(string prompt, string title, string defaultValue)
        {
            InitializeComponent();
            this.Text = title;
            this.labelPrompt.Text = prompt;
            this.textBoxInput.Text = defaultValue;
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            this.UserInput = this.textBoxInput.Text;
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        public static string Show(string prompt, string title, string defaultValue, Form owner)
        {
            using (CustomInputBox inputBox = new CustomInputBox(prompt, title, defaultValue))
            {
                inputBox.StartPosition = FormStartPosition.CenterParent;
                var result = inputBox.ShowDialog(owner);
                return result == DialogResult.OK ? inputBox.UserInput : null;
            }
        }
    }
}
