using Microsoft.VisualBasic.FileIO;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace Fuse5Export
{

    public partial class Fuse5Export_Form : Form
    {
        List<Fuse5lineItems> Fuse5lineItems = new List<Fuse5lineItems>();
        public Fuse5Export_Form()
        {
            InitializeComponent();
        }

        private void quitButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }


        private void addCustomerButton_Click(object sender, EventArgs e)
        {
            //SQL to check if item is customer
            DataAccess db = new DataAccess();

            Fuse5lineItems = db.GetItems(customersTextBox.Text);

            if (Fuse5lineItems.Count==0)
            {
                MessageBox.Show($"\"{ customersTextBox.Text }\" is not a valid customer!");
                customersTextBox.Text = "";
                return;
            }
            //
            if (string.IsNullOrEmpty(customersTextBox.Text))
            {
                MessageBox.Show("You must specify a Customer");
            }
            else
            {
                customersListBox.Items.Add(customersTextBox.Text);
                customersTextBox.Text = "";
            }
        }

        private void PGsAddButton_Click(object sender, EventArgs e)
        {
            //SQL to check if item is PG
            DataAccess1 db = new DataAccess1();

            Fuse5lineItems = db.GetItems(PGTextBox.Text);

            if (Fuse5lineItems.Count == 0)
            {
                MessageBox.Show($"\"{ PGTextBox.Text }\" is not a valid product group!");
                PGsListBox.Text="";
                return;
            }
            if (string.IsNullOrEmpty(PGTextBox.Text))
            {
                MessageBox.Show("You must specify a Product Group");
            }
            else
            {
                PGsListBox.Items.Add(PGTextBox.Text);
                PGTextBox.Text = "";
            }
        }

        private void removeCustomersButton_Click(object sender, EventArgs e)
        {
            int i = 0;
            int ITR = customersListBox.SelectedItems.Count;
            do
            {
               customersListBox.Items.Remove(customersListBox.SelectedItem);
                i++;
            } while (i < ITR);
        }

        private void PGsRemoveButton_Click(object sender, EventArgs e)
        {
            int i = 0;
            int ITR = PGsListBox.SelectedItems.Count;
            do
            {
                PGsListBox.Items.Remove(PGsListBox.SelectedItem);
                i++;
            } while (i < ITR);
        }


        private void customersTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                addCustomerButton_Click((object)sender, (EventArgs)e);
            }
        }

        private void PGTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                PGsAddButton_Click((object)sender, (EventArgs)e);
            }
        }

        private void ScriptButton_Click(object sender, EventArgs e)
        {

            if (PGsListBox.Items.Count == 0)
            {
                MessageBox.Show("You must add a Product Group.");
                return;
            }
            if (customersListBox.Items.Count == 0)
            {
                MessageBox.Show("You must add a Customer.");
                return;
            }

            //Edit script file
            if (File.Exists(@"\\svrsql1\AUTOPART\VUGDocs\Fuse5Script.vtm"))
            {
                string path = @"\\svrsql1\AUTOPART\VUGDocs\Fuse5Script.vtm";
                File.WriteAllText(path, String.Empty);
                TextWriter sw = new StreamWriter(path, true);

                sw.WriteLine("TEST ROUTINE");
                sw.WriteLine("ERC");
                sw.WriteLine("'");

                foreach (string strCust in customersListBox.Items)
                {
                    foreach (string strPG in PGsListBox.Items)
                    {
                        sw.WriteLine($@"V=524F5.vtr@{strPG}^ALL^^^^^^^^{strCust}@\\svrsql1\shared\Fuse5\{strCust}\{strPG}.txt");
                        //sw.WriteLine($@"V=524F5.vtr@{ strPG }^ALL^^^^^^^^{ strCust }^^^^@\\svrfile1\LocalUser\Fuse5\{ strCust }\{ strPG }.txt");
                    }
                }
                sw.Close();

                //Notify user
                MessageBox.Show("The script has been edited, run the Autopart Fuse5 Export menu command. After the Autopart Fuse5 Export command has finished, click the Finalize button here to prepare the files for Fuse5.");
            }
        }

        private void finalizeButton_Click(object sender, EventArgs e)
        {
            foreach (string strCust in customersListBox.Items)
            {
                foreach (string strPG in PGsListBox.Items)
                {
                    if (File.Exists($@"\\svrsql1\shared\Fuse5\{ strCust }\{ strPG }.txt"))
                    {
                        TextFieldParser tfp = new TextFieldParser($@"\\svrsql1\shared\Fuse5\{ strCust }\{ strPG }.txt");
                        tfp.HasFieldsEnclosedInQuotes = false;
                        tfp.Delimiters = new string[] { "\",\"" };
                        tfp.TrimWhiteSpace = true;

                        var csvSplitList = new List<string>();

                        // Reads all fields on the current line of the CSV file and returns as a string array
                        // Joins each field together with new delimiter "\t"
                        while (!tfp.EndOfData)
                        {
                            csvSplitList.Add(String.Join("\"\t\"", tfp.ReadFields()));
                        }

                        // Newline characters added to each line and flattens List<string> into single string
                        var formattedCsvToSave = String.Join(Environment.NewLine, csvSplitList.Select(x => x));

                        // Write single string to file
                        File.WriteAllText($@"\\svrsql1\shared\Fuse5\{ strCust }\{ strPG }.txt", "\"CAT\"\t" + "\"Item#\"\t" + "\"Description\"\t" + "\"S/U\"\t" + "\"List Price\"\t" + "\"Your Price\"\t" + "\"Core Cost\"\t" + "\"UPC Code\"\r\n" + formattedCsvToSave);
                        tfp.Close();

                    }
                }
            }

            customersListBox.Items.Clear();
            PGsListBox.Items.Clear();
            Cursor.Current = Cursors.Default;
            MessageBox.Show("Task Complete!");
        }
    }
}


