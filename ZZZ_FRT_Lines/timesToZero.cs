using System;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Windows.Forms;


namespace TimesToZero_Form
{
    public partial class timesToZero : Form
    {
        public string pg;
        public string part;
        public string branch;
        public DataTable dt = new DataTable();
        public DataTable displayTable = new DataTable();
        private TextBox CurrentTextbox = null;

        public timesToZero()
        {
            InitializeComponent();
            this.CenterToParent();
            displayTable.Columns.Add("Branch", typeof(string));
            displayTable.Columns.Add("Part", typeof(string));
            displayTable.Columns.Add("Times Sold", typeof(int));
            displayTable.Columns.Add("Days at Zero", typeof(int));
        }

        private void colorActiveTextbox_Leave(object sender, EventArgs e)
        {
            CurrentTextbox = (TextBox)sender;
            CurrentTextbox.BackColor = System.Drawing.Color.Empty;
            if (CurrentTextbox.Text == string.Empty)
            {
                CurrentTextbox.Text = "ALL";
            }
        }

        private void colorActiveTextbox_Enter(object sender, EventArgs e)
        {
            CurrentTextbox = (TextBox)sender;
            CurrentTextbox.BackColor = System.Drawing.Color.Yellow;
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;
            displayTable.Clear();
            dt.Clear();

            if (partNumberTextbox.Text == "ALL" || partNumberTextbox.Text == string.Empty)
            {
                part = "NULL";
            }
            else
            {
                part = "'" + partNumberTextbox.Text + "'";
            }

            if (PGTextbox.Text == "ALL" || PGTextbox.Text == string.Empty)
            {
                pg = "NULL";
            }
            else
            {
                pg = "'" + PGTextbox.Text + "'";
            }

            if (branchTextbox.Text == "ALL" || branchTextbox.Text == string.Empty)
            {
                branch = "NULL";
            }
            else
            {
                branch = "'" + branchTextbox.Text + "'";
            }

            if (part == "NULL" && pg == "NULL")
            {
                MessageBox.Show("Part and PG can not both be null!");
                return;
            }

            string startDate = startDateTextbox.Text;
            string endDate = endDateTextbox.Text;

            if (endDate == "ALL" || endDate == string.Empty)
            {
                endDate = DateTime.Now.ToString("MM/dd/yyyy");
            }


            SqlConnection connection1 = new SqlConnection(Helper.ConnString("AUTOPART"));

            connection1.Open();
            SqlDataAdapter adptr = new SqlDataAdapter(
            $@"
                USE BINCARDMOVEMENTS;

                DECLARE @PG VARCHAR(10);
                DECLARE @Part VARCHAR(10);
                DECLARE @Branch VARCHAR(10);

                SET @PG = {pg}
                SET @Part = {part}
                SET @Branch = {branch}

                ;WITH DateRange AS (
                    SELECT CAST('{startDate}' AS DATE) AS DateValue
                    UNION ALL
                    SELECT DATEADD(DAY, 1, DateValue)
                    FROM DateRange
                    WHERE DateValue < CAST('{endDate}' AS DATE)
                ),

                CTE AS (
                    SELECT Prefix, Branch, Part, CAST(DateTime AS DATE) AS 'date', MIN(ClsQty) as ClsQty
                    FROM BINCARDMOVEMENTS 
                    WHERE 
                    Part = IIF(@Part IS NULL, Part, @Part) AND 
                    Branch = IIF(@Branch IS NULL, Branch, @Branch) AND
                    LEFT(Part,3) = IIF(@PG IS NULL, LEFT(Part,3), @PG) AND
                    CAST(DateTime AS DATE) BETWEEN '{startDate}' AND '{endDate}'
                    Group BY Branch, Part, CAST([DateTime] as date), Prefix
                )


                SELECT II.Prefix, II.Branch, LEFT(Part,3) as PG, II.Part, II.ClsQty, CAST(DateValue AS DATE) AS DateValue
                FROM DateRange I LEFT OUTER JOIN CTE II ON I.DateValue = II.[date]
                ORDER BY DateValue ASC
                OPTION (MAXRECURSION 0);
            "
            , connection1);
            adptr.Fill(dt);


            for (int col = 0; col < dt.Columns.Count; col++)
            {
                object previousValue = DBNull.Value;

                for (int row = 0; row < dt.Rows.Count; row++)
                {
                    object currentValue = dt.Rows[row][col];

                    if (col == 0 && IsBlank(currentValue))
                    {
                        dt.Rows[row][col] = "No Sale";
                    }
                    else if (IsBlank(currentValue))
                    {
                        dt.Rows[row][col] = previousValue;
                    }
                    else
                    {
                        previousValue = currentValue;
                    }
                }
            }



            // Group by Branch and Part, then calculate counts
            var groupedData = dt.AsEnumerable()
                .GroupBy(row => new { Branch = row["Branch"], Part = row["Part"] })
                .Select(group => new
                {
                    Branch = group.Key.Branch,
                    Part = group.Key.Part,
                    Count_Prefix_I = group.Count(row => row["Prefix"].ToString() == "I"),
                    Count_ClsQty_Zero = group.Count(row => !DBNull.Value.Equals(row["ClsQty"]) && Convert.ToInt32(row["ClsQty"]) == 0)
                    //Count_ClsQty_Zero = group.Count(row => Convert.ToInt32(row["ClsQty"]) == 0)
                });

            // Fill the aggregated DataTable
            foreach (var item in groupedData)
            {
                DataRow newRow = displayTable.NewRow();
                newRow["Branch"] = item.Branch;
                newRow["Part"] = item.Part;
                newRow["Times Sold"] = item.Count_Prefix_I;
                newRow["Days at Zero"] = item.Count_ClsQty_Zero;
                displayTable.Rows.Add(newRow);
            }
            Cursor cursor = Cursors.Default;
            displayedTable.displayedTable dis = new displayedTable.displayedTable(displayTable);
            dis.Show();
        }

        static bool IsBlank(object value)
        {
            return value == null || string.IsNullOrWhiteSpace(value.ToString());
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }

}

