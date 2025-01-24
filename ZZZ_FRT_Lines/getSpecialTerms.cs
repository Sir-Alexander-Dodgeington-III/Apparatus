using System;
using System.Drawing;
using System.Windows.Forms;

namespace getSpecialTerms
{
    public partial class getSpecialTerms : Form
    {
        private TextBox CurrentTextbox = null;
        public string branch;
        public string pg;
        public string customer;
        public string[] custArray;
        public getSpecialTerms()
        {
            InitializeComponent();
            this.CenterToScreen();
        }

        private void doStuff()
        {
            if (branchTextbox.Text == "ALL" || branchTextbox.Text == "all" || branchTextbox.Text == "All")
            {
                branch = "NULL";
            }
            else
            {
                branch = "'" + branchTextbox.Text + "'";
            }

            if (pgTextbox.Text == "ALL" || pgTextbox.Text == "all" || pgTextbox.Text == "All")
            {
                pg = "NULL";
            }
            else
            {
                pg = "'" + pgTextbox.Text + "%'";
            }

            if (customerTextbox.Text == "ALL" || customerTextbox.Text == "all" || customerTextbox.Text == "All")
            {
                customer = "NULL";
            }
            else
            {
                customer = customerTextbox.Text;
                //customer = customer.Replace(",", "\",\"");
                custArray = customer.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                customer = "";
                foreach (string i in custArray)
                {
                    customer += $"'{i}',";
                }
                customer = customer.TrimEnd(',');
                //customer = "'" + customerTextbox.Text + "'";
                //customer = customer.Replace(",", "\",");
            }

            if (customer != "NULL")
            {
                string Query = $@"
                                DECLARE @Branch VARCHAR(10)
                                DECLARE @PG VARCHAR(10)

                                SET @Branch = {branch}
                                SET @PG = {pg}

                                ;WITH CTE1(Branch, Customer, [Name], Part, Updated, [Type], BasedOn, CalcType, CalcValue) as (
                                SELECT 
                                C.Area as Branch, 
                                M.SubKey1, 
                                C.Name, 
                                M.SubKey3, 
                                M.Updated, 
                                M.SubKey2, 
                                M.BasedOn, 
                                M.CalcType, 
                                M.CalcValue 

                                FROM MVPR M INNER JOIN 
                                Customer C ON C.KeyCode = M.SubKey1

                                WHERE M.Prefix = 'K' AND M.SubKey3 LIKE IIF(@PG IS NULL, M.SubKey3, @PG) AND C.Area = IIF(@Branch IS NULL, C.Area, @Branch) AND M.SubKey1 IN ({customer})),

                                CTE2 (Branch, Customer, Part, a5) as(
                                SELECT 
                                C.Area as Branch, 
                                M.SubKey1, 
                                M.SubKey3, 
                                M.a5 

                                FROM MVPR M INNER JOIN 
                                Customer C ON C.KeyCode = M.SubKey1 

                                WHERE M.Prefix = '~' AND M.SubKey3 LIKE IIF(@PG IS NULL, M.SubKey3, @PG) AND C.Area = IIF(@Branch IS NULL, C.Area, @Branch) AND M.SubKey1 IN ({customer}))

                                SELECT 
                                I.Branch, 
                                I.Customer, 
                                I.Name, 
                                I.Part, 
                                I.Updated, 
                                I.[Type], 
                                I.BasedOn, 
                                I.CalcType, 
                                CAST(I.CalcValue as decimal(18,4)) as CalcValue, 
                                CAST(II.a5 as decimal(18,2)) as 'GUAR MARG' 

                                FROM CTE1 I LEFT OUTER JOIN 
                                CTE2 II ON I.Customer = II.Customer AND I.Part = II.Part AND I.Branch = II.Branch

                                ORDER BY I.Customer, I.Part
                        ";

                QueryResults.QueryResults qr = new QueryResults.QueryResults(Query);
                qr.Show();
            }
            else
            {
                string Query = $@"
                                DECLARE @Branch VARCHAR(10)
                                DECLARE @PG VARCHAR(10)

                                SET @Branch = {branch}
                                SET @PG = {pg}

                                ;WITH CTE1(Branch, Customer, [Name], Part, Updated, [Type], BasedOn, CalcType, CalcValue) as (
                                SELECT 
                                C.Area as Branch, 
                                M.SubKey1, 
                                C.Name, 
                                M.SubKey3, 
                                M.Updated, 
                                M.SubKey2, 
                                M.BasedOn, 
                                M.CalcType, 
                                M.CalcValue 

                                FROM MVPR M INNER JOIN 
                                Customer C ON C.KeyCode = M.SubKey1

                                WHERE M.Prefix = 'K' AND M.SubKey3 LIKE IIF(@PG IS NULL, M.SubKey3, @PG) AND C.Area = IIF(@Branch IS NULL, C.Area, @Branch)),

                                CTE2 (Branch, Customer, Part, a5) as(
                                SELECT 
                                C.Area as Branch, 
                                M.SubKey1, 
                                M.SubKey3, 
                                M.a5 

                                FROM MVPR M INNER JOIN 
                                Customer C ON C.KeyCode = M.SubKey1 

                                WHERE M.Prefix = '~' AND M.SubKey3 LIKE IIF(@PG IS NULL, M.SubKey3, @PG) AND C.Area = IIF(@Branch IS NULL, C.Area, @Branch))

                                SELECT 
                                I.Branch, 
                                I.Customer, 
                                I.Name, 
                                I.Part, 
                                I.Updated, 
                                I.[Type], 
                                I.BasedOn, 
                                I.CalcType, 
                                CAST(I.CalcValue as decimal(18,4)) as CalcValue, 
                                CAST(II.a5 as decimal(18,2)) as 'GUAR MARG' 

                                FROM CTE1 I LEFT OUTER JOIN 
                                CTE2 II ON I.Customer = II.Customer AND I.Part = II.Part AND I.Branch = II.Branch

                                ORDER BY I.Customer, I.Part
                        ";
                QueryResults.QueryResults qr = new QueryResults.QueryResults(Query);
                qr.Show();
            }
        }

        private void colorActiveTextbox_Leave(object sender, EventArgs e)
        {
            CurrentTextbox = (TextBox)sender;
            CurrentTextbox.BackColor = Color.Empty;
            if (CurrentTextbox.Text == "")
            {
                CurrentTextbox.Text = "ALL";
            }
        }

        private void colorActiveTextbox_Enter(object sender, EventArgs e)
        {
            CurrentTextbox = (TextBox)sender;
            CurrentTextbox.BackColor = Color.Yellow;
            CurrentTextbox.SelectAll();
        }

        private void getSpecialTerms_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Handled != true)
            {
                if (e.KeyCode == Keys.F5)
                {
                    doStuff();
                }
                else if (e.KeyCode == Keys.Enter && searchButton.Focused)
                {
                    doStuff();
                }
            }
            else
            {
                return;
            }
        }

        private void searchButton_Click(object sender, EventArgs e)
        {
            doStuff();
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private Size oldSize;
        private void getSpecialTerms_Load(object sender, EventArgs e) => oldSize = base.Size;

        protected override void OnResize(System.EventArgs e)
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
