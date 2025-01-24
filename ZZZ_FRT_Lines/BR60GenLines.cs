using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BR60generated
{
    public partial class BR60GeneratedLines_Form : Form
    {
        private void AlreadyInList()
        {
            MessageBox.Show("The item " + addTextBox.Text + " is already in the list!");
            addTextBox.Text = "";
            MondayCheckBox.Checked = false;
            TuesdayCheckBox.Checked = false;
            WednesdayCheckBox.Checked = false;
            ThursdayCheckBox.Checked = false;
            FridayCheckBox.Checked = false;
            return;
        }

        private void NotPG()
        {
            MessageBox.Show("The item " + addTextBox.Text + " is not a valid product group!");
            addTextBox.Text = "";
            MondayCheckBox.Checked = false;
            TuesdayCheckBox.Checked = false;
            WednesdayCheckBox.Checked = false;
            ThursdayCheckBox.Checked = false;
            FridayCheckBox.Checked = false;
            return;
        }
        List<addItems> addItems = new List<addItems>();

        public BR60GeneratedLines_Form()
        {
            InitializeComponent();
            this.CenterToScreen();


            string[] lines = File.ReadAllLines(@"\\svrsql1\AUTOPART\VUGDocs\orders.vtm");
            //List<string> lines = new List<string>();
            if (File.Exists(@"\\svrsql1\AUTOPART\VUGDocs\orders.vtm"))
            {
                // Read the file and display it line by line.
                IEnumerable<string> selectLines = lines.Where(line => line.StartsWith("[SUN]"));
                IEnumerable<string> selectLines1 = lines.Where(line => line.StartsWith("[MON]"));
                IEnumerable<string> selectLines2 = lines.Where(line => line.StartsWith("[TUE]"));
                IEnumerable<string> selectLines3 = lines.Where(line => line.StartsWith("[WED]"));
                IEnumerable<string> selectLines4 = lines.Where(line => line.StartsWith("[THU]"));

                BR60Lines_ListBox.DataSource = File.ReadAllLines(@"\\svrsql1\AUTOPART\VUGDocs\orders.vtm");

                foreach (var item in selectLines)
                {
                    //BR60Lines_ListBox.Items.Add(item);
                    MondayTextBox.Text = item;
                    MondayTextBox.Text = MondayTextBox.Text.Replace("*BR60***BR60", "");
                    MondayTextBox.Text = MondayTextBox.Text.Replace("[SUN]U=Orders@!*", "");
                }

                foreach (var item in selectLines1)
                {
                    //BR60Lines_ListBox.Items.Add(item);
                    TuesdayTextBox.Text = item;
                    TuesdayTextBox.Text = TuesdayTextBox.Text.Replace("*BR60***BR60", "");
                    TuesdayTextBox.Text = TuesdayTextBox.Text.Replace("[MON]U=Orders@!*", "");
                }

                foreach (var item in selectLines2)
                {
                    //BR60Lines_ListBox.Items.Add(item);
                    WednesdayTextBox.Text = item;
                    WednesdayTextBox.Text = WednesdayTextBox.Text.Replace("*BR60***BR60", "");
                    WednesdayTextBox.Text = WednesdayTextBox.Text.Replace("[TUE]U=Orders@!*", "");
                }

                foreach (var item in selectLines3)
                {
                    //BR60Lines_ListBox.Items.Add(item);
                    ThursdayTextBox.Text = item;
                    ThursdayTextBox.Text = ThursdayTextBox.Text.Replace("*BR60***BR60", "");
                    ThursdayTextBox.Text = ThursdayTextBox.Text.Replace("[WED]U=Orders@!*~", "");
                }

                foreach (var item in selectLines4)
                {
                    //BR60Lines_ListBox.Items.Add(item);
                    FridayTextBox.Text = item;
                    FridayTextBox.Text = FridayTextBox.Text.Replace("*BR60***BR60", "");
                    FridayTextBox.Text = FridayTextBox.Text.Replace("[THU]U=Orders@!*", "");
                }
                //BR60Lines_ListBox.Items.Add(lines);
            }

        }

        private void QuitButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void OkButton_Click(object sender, EventArgs e)
        {
            string Monday = "[SUN]U=Orders@!*" + MondayTextBox.Text + "*BR60***BR60";
            string Tuesday = "[MON]U=Orders@!*" + TuesdayTextBox.Text + "*BR60***BR60";
            string Wednesday = "[TUE]U=Orders@!*" + WednesdayTextBox.Text + "*BR60***BR60";
            string Thursday = "[WED]U=Orders@!*~" + ThursdayTextBox.Text + "*BR60***BR60";
            string Friday = "[THU]U=Orders@!*" + FridayTextBox.Text + "*BR60***BR60";

            using (StreamReader sr = new StreamReader(@"\\svrsql1\AUTOPART\VUGDocs\orders.vtm"))
            {
                string line;
                string Lines = "";
                bool tf = false;

                while ((line = sr.ReadLine()) != null)
                {

                    if (line.StartsWith("[SUN]"))
                    {
                        Lines += Monday + "\r\n";
                        tf = true;
                    }

                    if (line.StartsWith("[MON]"))
                    {
                        Lines += Tuesday + "\r\n";
                        tf = true;
                    }

                    if (line.StartsWith("[TUE]"))
                    {
                        Lines += Wednesday + "\r\n";
                        tf = true;
                    }

                    if (line.StartsWith("[WED]"))
                    {
                        Lines += Thursday + "\r\n";
                        tf = true;
                    }

                    if (line.StartsWith("[THU]"))
                    {
                        Lines += Friday + "\r\n";
                        tf = true;
                    }

                    if (tf == false)
                    {
                        Lines += line + "\r\n";
                    }

                    tf = false;

                }
                sr.Close();


                if (File.Exists(@"\\svrsql1\AUTOPART\VUGDocs\ORDERS.VTM"))
                {
                    File.Delete(@"\\svrsql1\AUTOPART\VUGDocs\ORDERS.VTM");
                }

                using (StreamWriter sw = new StreamWriter(@"\\svrsql1\AUTOPART\VUGDocs\ORDERS.VTM"))
                {
                    sw.WriteLine(Lines);
                    sw.Close();
                }
            }

                if (BR60Lines_ListBox.Items.Count > 0)
                {
                    //UpdateBinding();
                    BR60Lines_ListBox.DataSource = null;
                    BR60Lines_ListBox.DisplayMember = null;
                    BR60Lines_ListBox.Items.Clear();
                }

            this.Controls.Clear();

            this.InitializeComponent();
            string[] lines1 = File.ReadAllLines(@"\\svrsql1\AUTOPART\VUGDocs\orders.vtm");
            //List<string> lines = new List<string>();
            if (File.Exists(@"\\svrsql1\AUTOPART\VUGDocs\orders.vtm"))
            {
                // Read the file and display it line by line.
                IEnumerable<string> selectLines = lines1.Where(line1 => line1.StartsWith("[SUN]"));
                IEnumerable<string> selectLines1 = lines1.Where(line1 => line1.StartsWith("[MON]"));
                IEnumerable<string> selectLines2 = lines1.Where(line1 => line1.StartsWith("[TUE]"));
                IEnumerable<string> selectLines3 = lines1.Where(line1 => line1.StartsWith("[WED]"));
                IEnumerable<string> selectLines4 = lines1.Where(line1 => line1.StartsWith("[THU]"));

                BR60Lines_ListBox.DataSource = File.ReadAllLines(@"\\svrsql1\AUTOPART\VUGDocs\orders.vtm");

                foreach (var item in selectLines)
                {
                    //BR60Lines_ListBox.Items.Add(item);
                    MondayTextBox.Text = item;
                    MondayTextBox.Text = MondayTextBox.Text.Replace("*BR60***BR60", "");
                    MondayTextBox.Text = MondayTextBox.Text.Replace("[SUN]U=Orders@!*", "");
                }

                foreach (var item in selectLines1)
                {
                    //BR60Lines_ListBox.Items.Add(item);
                    TuesdayTextBox.Text = item;
                    TuesdayTextBox.Text = TuesdayTextBox.Text.Replace("*BR60***BR60", "");
                    TuesdayTextBox.Text = TuesdayTextBox.Text.Replace("[MON]U=Orders@!*", "");
                }

                foreach (var item in selectLines2)
                {
                    //BR60Lines_ListBox.Items.Add(item);
                    WednesdayTextBox.Text = item;
                    WednesdayTextBox.Text = WednesdayTextBox.Text.Replace("*BR60***BR60", "");
                    WednesdayTextBox.Text = WednesdayTextBox.Text.Replace("[TUE]U=Orders@!*", "");
                }

                foreach (var item in selectLines3)
                {
                    //BR60Lines_ListBox.Items.Add(item);
                    ThursdayTextBox.Text = item;
                    ThursdayTextBox.Text = ThursdayTextBox.Text.Replace("*BR60***BR60", "");
                    ThursdayTextBox.Text = ThursdayTextBox.Text.Replace("[WED]U=Orders@!*~", "");
                }

                foreach (var item in selectLines4)
                {
                    //BR60Lines_ListBox.Items.Add(item);
                    FridayTextBox.Text = item;
                    FridayTextBox.Text = FridayTextBox.Text.Replace("*BR60***BR60", "");
                    FridayTextBox.Text = FridayTextBox.Text.Replace("[THU]U=Orders@!*", "");
                }
                //BR60Lines_ListBox.Items.Add(lines);
            }


            MessageBox.Show(@"Saved to Orders.vtm");
        }

        private void addBotton_Click(object sender, EventArgs e)
        {
            addTextBox.Text = addTextBox.Text.ToUpper();
            if (MondayCheckBox.Checked && addBotton.Text != null)
            {
                DataAccess1 db = new DataAccess1();
                addItems = db.GetItems(addTextBox.Text);

                if (addItems.Count > 0)
                {
                    if (MondayTextBox.Text.Contains(addTextBox.Text))
                    {
                        AlreadyInList();
                    }
                    else
                    {
                        MondayTextBox.Text = MondayTextBox.Text + "," + addTextBox.Text;
                        MondayCheckBox.Checked = false;
                    }
                }
                else
                {
                    NotPG();
                }
            }

            if (TuesdayCheckBox.Checked && addBotton.Text != null)
            {
                DataAccess1 db = new DataAccess1();
                addItems = db.GetItems(addTextBox.Text);

                if (addItems.Count > 0)
                {
                    if (TuesdayTextBox.Text.Contains(addTextBox.Text))
                    {
                        AlreadyInList();
                    }
                    else
                    {
                        TuesdayTextBox.Text = TuesdayTextBox.Text + "," + addTextBox.Text;
                        TuesdayCheckBox.Checked = false;
                    }
                }
                else
                {
                    NotPG();
                }
            }

            if (WednesdayCheckBox.Checked && addBotton.Text != null)
            {
                DataAccess1 db = new DataAccess1();
                addItems = db.GetItems(addTextBox.Text);

                if (addItems.Count > 0)
                {
                    if (WednesdayTextBox.Text.Contains(addTextBox.Text))
                    {
                        AlreadyInList();
                    }
                    else
                    {
                       WednesdayTextBox.Text = WednesdayTextBox.Text + "," + addTextBox.Text;
                        WednesdayCheckBox.Checked = false;
                    }
                }
                else
                {
                    NotPG();
                }
            }


            if (ThursdayCheckBox.Checked && addBotton.Text != null)
            {
                DataAccess2 db = new DataAccess2();
                addItems = db.GetItems(addTextBox.Text);

                if (addItems.Count > 0)
                {
                    if (ThursdayTextBox.Text.Contains(addTextBox.Text))
                    {
                        AlreadyInList();
                    }
                    else
                    {
                        ThursdayTextBox.Text = ThursdayTextBox.Text + "," + addTextBox.Text;
                        ThursdayCheckBox.Checked = false;
                    }
                }
                else
                {
                    MessageBox.Show("The item you are trying to add is not a product range!");
                    addTextBox.Text = "";
                    MondayCheckBox.Checked = false;
                    TuesdayCheckBox.Checked = false;
                    WednesdayCheckBox.Checked = false;
                    ThursdayCheckBox.Checked = false;
                    FridayCheckBox.Checked = false;
                    return;
                }
            }

            if (FridayCheckBox.Checked && addBotton.Text != null)
            {
                DataAccess1 db = new DataAccess1();
                addItems = db.GetItems(addTextBox.Text);

                if (addItems.Count > 0)
                {
                    if (FridayTextBox.Text.Contains(addTextBox.Text))
                    {
                        AlreadyInList();
                    }
                    else
                    {
                        FridayTextBox.Text = FridayTextBox.Text + "," + addTextBox.Text;
                        FridayCheckBox.Checked = false;
                    }
                }
                else
                {
                    NotPG();
                }
            }


        }

        private void removeButton_Click(object sender, EventArgs e)
        {
            if (MondayCheckBox.Checked && addBotton.Text != null)
            {
                MondayTextBox.Text = MondayTextBox.Text.Replace("," + addTextBox.Text, "");
                MondayCheckBox.Checked = false;
            }

            if (TuesdayCheckBox.Checked && addBotton.Text != null)
            {
                TuesdayTextBox.Text = TuesdayTextBox.Text.Replace("," + addTextBox.Text, "");
                TuesdayCheckBox.Checked = false;
            }

            if (WednesdayCheckBox.Checked && addBotton.Text != null)
            {
                WednesdayTextBox.Text = WednesdayTextBox.Text.Replace("," + addTextBox.Text, "");
                WednesdayCheckBox.Checked = false;
            }

            if (ThursdayCheckBox.Checked && addBotton.Text != null)
            {
                ThursdayTextBox.Text = ThursdayTextBox.Text.Replace("," + addTextBox.Text, "");
                ThursdayCheckBox.Checked = false;
            }

            if (FridayCheckBox.Checked && addBotton.Text != null)
            {
                FridayTextBox.Text = FridayTextBox.Text.Replace("," + addTextBox.Text, "");
                FridayCheckBox.Checked = false;
            }

            addTextBox.Text = "";
        }

        private void button1_Click(object sender, EventArgs e)
        {
            new BR60GenLines_Fulfillment.BR60GenLines_Fulfillment().Show();
        }
    }
}
