using System;
using System.Drawing;
using System.Windows.Forms;

namespace LeftvsRight
{
    public partial class LeftvsRight : Form
    {
        private TextBox CurrentTextbox = null;
        private string _text;
        private Timer _textTimer;
        public string PG;
        public string range;
        public string branch;
        public string right;
        public string left;

        public LeftvsRight()
        {
            InitializeComponent();
            this.CenterToScreen();
            Closed += Form2_Closed;
        }

        private void Form2_Load(object sender, EventArgs e)
        {
            _text = Text;
            _textTimer = new Timer();
            _textTimer.Interval = 150;
            _textTimer.Tick += _textTimer_Tick;
            _textTimer.Start();
        }

        private void _textTimer_Tick(object sender, EventArgs e)
        {
            if (Text == string.Empty)
            {
                Text = _text;
            }

            Text = Text.Substring(1);
        }

        private void Form2_Closed(object sender, EventArgs e)
        {
            _textTimer.Dispose();
        }

        private void closeButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void searchButton_Click(object sender, EventArgs e)
        {
            if (pgTextbox.Text == "" && rangeTextbox.Text == "")
            {
                MessageBox.Show("You must enter a product group or range to continue");
                return;
            }

            if (pgTextbox.Text == "")
            {
                PG = "NULL";
            }
            else
            {
                PG = "'" + pgTextbox.Text + "'";
            }
            if (rangeTextbox.Text == "")
            {
                range = "NULL";
            }
            else
            {
                range = "'" + rangeTextbox.Text + "'";
            }
            if (branchTextbox.Text == "")
            {
                branch = "NULL";
            }
            else
            {
                branch = "'" + branchTextbox.Text + "'";
            }

            if (rightTextbox.Text == "")
            {
                MessageBox.Show("You must define right");
                return;
            }
            else
            {
                right = "'" + rightTextbox.Text + "'";
            }
            if(leftTextbox.Text == "")
            {
                MessageBox.Show("You must define left");
                return;
            }
            else
            {
                left = "'" + leftTextbox.Text + "'";
            }
            

            string Query = $@"
                            DECLARE @PG VARCHAR(10);
                            DECLARE @Range VARCHAR(10);
                            DECLARE @Branch VARCHAR(10);
                            DECLARE @Left VARCHAR(10);
                            DECLARE @Right VARCHAR(10);

                            SET @PG = {PG}
                            SET @Range = {range}
                            SET @Branch = {branch}
                            SET @Left = {left}
                            SET @Right = {right}

                            ;WITH CTE0 (Branch, Base, Part, Free) as (
                                SELECT S.Branch, LEFT(S.Part, LEN(S.Part) - 1), S.Part, S.Free FROM Stock S INNER JOIN Product P ON P.KeyCode = S.Part WHERE P.PG = IIF(@PG IS NULL, P.PG, @PG) AND P.Range = IIF(P.Range IS NULL, P.Range, @Range) AND S.Branch = IIF(@Branch IS NULL, S.Branch, @Branch) AND RIGHT(S.Part,1) = @Left
                            ),

                            CTE1 (Branch, Base, Part, Free) as (
                                SELECT S.Branch, LEFT(S.Part, LEN(S.Part) - 1), S.Part, S.Free FROM Stock S INNER JOIN Product P ON P.KeyCode = S.Part WHERE P.PG = IIF(@PG IS NULL, P.PG, @PG) AND P.Range = IIF(P.Range IS NULL, P.Range, @Range) AND S.Branch = IIF(@Branch IS NULL, S.Branch, @Branch) AND RIGHT(S.Part,1) = @Right
                            )

                            SELECT I.Branch, I.Part, I.Free FROM CTE0 I WHERE I.Free = 0 AND I.Base IN (SELECT Base FROM CTE1 WHERE Free > 0) AND I.Branch NOT IN('BR30','BR60')
                            UNION
                            SELECT II.Branch, II.Part, II.Free FROM CTE1 II WHERE II.Free = 0 AND II.Base IN (SELECT Base FROM CTE0 WHERE Free > 0) AND II.Branch NOT IN('BR30','BR60')
                        ";
            QueryResults.QueryResults qr = new QueryResults.QueryResults(Query);
            qr.Show();
        }

        private void colorActiveTextbox_Leave(object sender, EventArgs e)
        {
            CurrentTextbox = (TextBox)sender;
            CurrentTextbox.BackColor = Color.Empty;
            CurrentTextbox.Text = CurrentTextbox.Text.ToUpper();
        }

        private void colorActiveTextbox_Enter(object sender, EventArgs e)
        {
            CurrentTextbox = (TextBox)sender;
            CurrentTextbox.BackColor = Color.Yellow;
        }
    }
}
