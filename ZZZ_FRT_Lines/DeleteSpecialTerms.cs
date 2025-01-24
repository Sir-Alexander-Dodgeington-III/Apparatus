using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;

namespace DeleteSpecialTerms
{
    public partial class DeleteSpecialTerms : Form
    {
        public DialogResult dialogResult;
        private TextBox CurrentTextbox = null;
        public Action<DataTable> SendDataTable;
        DataTable dt = new DataTable();
        public DeleteSpecialTerms()
        {
            InitializeComponent();
            this.CenterToScreen();
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void goButton_Click(object sender, EventArgs e)
        {
            if (pgTextbox.Text != "" && rangeTextbox.Text != "")
            {
                MessageBox.Show("You must specify a product group OR range, not both.");
                return;
            }

            if(pgTextbox.Text != "" && branchTextbox.Text == "ALL" && rangeTextbox.Text == "")
            {
                dialogResult = MessageBox.Show($"Are you sure you want to delete all special terms for product group {pgTextbox.Text}? Once this process is complete it can not be undone.", "Important Notice!", MessageBoxButtons.YesNo);
                if (dialogResult == DialogResult.Yes)
                {
                    SqlConnection conn = new SqlConnection(Helper.ConnString("AUTOPART"));

                        conn.Open();
                        //DataSet ds1 = new DataSet();
                        SqlDataAdapter adapter1 = new SqlDataAdapter(
                        $@"
                            SELECT * FROM MVPR WHERE Prefix = 'K' AND SubKey3 LIKE '{pgTextbox.Text}%'
                          ", conn);
                        adapter1.Fill(dt);
                        conn.Close();

                        using (SqlCommand cmd = new SqlCommand("sp_MERI_deleteSpecialTermsByPG", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add("@PG", SqlDbType.VarChar).Value = pgTextbox.Text.ToString() + "%";

                        try
                        {
                            conn.Open();
                            cmd.ExecuteNonQuery();
                            conn.Close();
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message, "Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }
                    }
                }
                else
                {
                    return;
                }
            }


            if (pgTextbox.Text != "" && branchTextbox.Text != "ALL" && rangeTextbox.Text == "")
            {
                dialogResult = MessageBox.Show($"Are you sure you want to delete all special terms for product group {pgTextbox.Text}? Once this process is complete it can not be undone.", "Important Notice!", MessageBoxButtons.YesNo);
                if (dialogResult == DialogResult.Yes)
                {
                    SqlConnection conn = new SqlConnection(Helper.ConnString("AUTOPART"));

                    conn.Open();
                    //DataSet ds1 = new DataSet();
                    SqlDataAdapter adapter1 = new SqlDataAdapter(
                    $@"
                            SELECT * FROM MVPR WHERE Prefix = 'K' AND SubKey3 LIKE '{pgTextbox.Text}%'
                          ", conn);
                    adapter1.Fill(dt);
                    conn.Close();

                    using (SqlCommand cmd = new SqlCommand("sp_MERI_deleteSpecialTermsByPGandBranch", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add("@PG", SqlDbType.VarChar).Value = pgTextbox.Text.ToString() + "%";
                        cmd.Parameters.Add("@Branch", SqlDbType.VarChar).Value = branchTextbox.Text.ToString();

                        try
                        {
                            conn.Open();
                            cmd.ExecuteNonQuery();
                            conn.Close();
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message, "Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }
                    }
                }
                else
                {
                    return;
                }
            }


            if (rangeTextbox.Text != "" && branchTextbox.Text == "ALL" && pgTextbox.Text == "")
            {
                dialogResult = MessageBox.Show($"Are you sure you want to delete all special terms for range {rangeTextbox.Text}? Once this process is complete it can not be undone.", "Important Notice!", MessageBoxButtons.YesNo);
                if (dialogResult == DialogResult.Yes)
                {
                    SqlConnection conn = new SqlConnection(Helper.ConnString("AUTOPART"));

                    conn.Open();
                    //DataSet ds1 = new DataSet();
                    SqlDataAdapter adapter2 = new SqlDataAdapter(
                    $@"
                            SELECT * FROM MVPR WHERE Prefix = 'K' AND SubKey2 = 'R' AND SubKey3 = '{rangeTextbox.Text}'
                          ", conn);
                    adapter2.Fill(dt);
                    conn.Close();

                    using (SqlCommand cmd = new SqlCommand("sp_MERI_deleteSpecialTermsByRange", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add("@Range", SqlDbType.VarChar).Value = rangeTextbox.Text.ToString();

                        try
                        {
                            conn.Open();
                            cmd.ExecuteNonQuery();
                            conn.Close();
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message, "Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }
                    }
                }
                else
                {
                    return;
                }
            }


            if (rangeTextbox.Text != "" && branchTextbox.Text != "ALL" && pgTextbox.Text == "")
            {
                dialogResult = MessageBox.Show($"Are you sure you want to delete all special terms for range {rangeTextbox.Text}? Once this process is complete it can not be undone.", "Important Notice!", MessageBoxButtons.YesNo);
                if (dialogResult == DialogResult.Yes)
                {
                    SqlConnection conn = new SqlConnection(Helper.ConnString("AUTOPART"));

                    conn.Open();
                    //DataSet ds1 = new DataSet();
                    SqlDataAdapter adapter2 = new SqlDataAdapter(
                    $@"
                            SELECT * FROM MVPR WHERE Prefix = 'K' AND SubKey2 = 'R' AND SubKey3 = '{rangeTextbox.Text}'
                          ", conn);
                    adapter2.Fill(dt);
                    conn.Close();

                    using (SqlCommand cmd = new SqlCommand("sp_MERI_deleteSpecialTermsByRangeandBranch", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add("@Range", SqlDbType.VarChar).Value = rangeTextbox.Text.ToString();
                        cmd.Parameters.Add("@Branch", SqlDbType.VarChar).Value = branchTextbox.Text.ToString();
                        try
                        {
                            conn.Open();
                            cmd.ExecuteNonQuery();
                            conn.Close();
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message, "Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }
                    }
                }
                else
                {
                    return;
                }
            }

            if (pgTextbox.Text == "" && rangeTextbox.Text == "")
            {
                MessageBox.Show("You must specify a product group or range.");
                return;
            }
            else
            {
                MessageBox.Show($@"{dt.Rows.Count} items removed. Deleted contents can be viewed/exported by clicking the 'Review' button. Once this form is closed, the deleted data will be lost forever.");
                
                reviewButton.Enabled = true;
            }
        }

        private void reviewButton_Click(object sender, EventArgs e)
        {
            DataTableContentsForm.DataTableContentsForm contents = new DataTableContentsForm.DataTableContentsForm(dt);
            contents.Show();
        }

        private void colorActiveTextbox_Enter(object sender, EventArgs e)
        {
            CurrentTextbox = (TextBox)sender;
            CurrentTextbox.BackColor = Color.Yellow;
        }


        private void colorActiveTextbox_LeaveBranch(object sender, EventArgs e)
        {
            CurrentTextbox = (TextBox)sender;
            CurrentTextbox.BackColor = Color.Empty;
            if (CurrentTextbox.Text == "")
            {
                CurrentTextbox.Text = "ALL";
            }
        }

        private void colorActiveTextbox_Leave(object sender, EventArgs e)
        {
            CurrentTextbox = (TextBox)sender;
            CurrentTextbox.BackColor = Color.Empty;
        }

    }
}
