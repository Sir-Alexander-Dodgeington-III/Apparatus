using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;

namespace PFM
{
    public partial class PFM : Form
    {
        private TextBox CurrentTextbox = null;
        public DataTable dt = new DataTable();
        public DataTable dt1 = new DataTable();
        public string part;
        public PFM()
        {
            InitializeComponent();
            this.CenterToScreen();
            partTextbox.Focus();
        }

        private void colorActiveTextbox_Leave(object sender, EventArgs e)
        {
            CurrentTextbox = (System.Windows.Forms.TextBox)sender;
            CurrentTextbox.BackColor = Color.Empty;
        }

        private void colorActiveTextbox_Enter(object sender, EventArgs e)
        {
            CurrentTextbox = (TextBox)sender;
            CurrentTextbox.BackColor = Color.Yellow;
        }

        private void Form3_Load(object sender, EventArgs e)
        {
            partTextbox.Focus();
            pgTextbox.Enabled= false;
            rangeTextbox.Enabled= false;
            descTextbox.Enabled= false;
            mktTextbox.Enabled= false;
            mfrTextbox.Enabled= false;
            brandTextbox.Enabled= false;
            lev1Textbox.Enabled= false;
            lev2Textbox.Enabled= false;
            lev3Textbox.Enabled= false;
            urlTextbox.Enabled= false;
            displayCheckbox.Enabled= false;
            okButton.Enabled= false;
        }

        private void partTextbox_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (e.KeyCode == Keys.Enter || e.KeyCode == Keys.Tab)
            {
                dt.Clear();
                part = partTextbox.Text;
                SqlConnection connection1 = new SqlConnection(Helper.ConnString("AUTOPART"));
                connection1.Open();
                SqlDataAdapter adptr = new SqlDataAdapter($@"
                    DECLARE @part VARCHAR(MAX)
                    SET @part = '{part}'                             
                
                    SELECT MKC.*, P.PG, P.Range FROM MERI_Kit_Component MKC INNER JOIN Product P ON P.KeyCode = MKC.ComponentSKU WHERE MKC.ComponentSKU = '{part}'
            "
                , connection1);
                adptr.Fill(dt);
                connection1.Close();

                if (dt.Rows.Count > 0)
                {
                    descTextbox.Enabled = true;
                    mktTextbox.Enabled = true;
                    mfrTextbox.Enabled = true;
                    brandTextbox.Enabled = true;
                    lev1Textbox.Enabled = true;
                    lev2Textbox.Enabled = true;
                    lev3Textbox.Enabled = true;
                    urlTextbox.Enabled = true;
                    displayCheckbox.Enabled = true;
                    okButton.Enabled = true;


                    pgTextbox.Text = dt.Rows[0]["PG"].ToString();
                    rangeTextbox.Text = dt.Rows[0]["Range"].ToString();
                    descTextbox.Text = dt.Rows[0]["Display_Desc"].ToString();
                    mfrTextbox.Text = dt.Rows[0]["MFR_SKU"].ToString();
                    mktTextbox.Text = dt.Rows[0]["Marketing_Desc"].ToString();
                    brandTextbox.Text = dt.Rows[0]["Brand"].ToString();
                    lev1Textbox.Text = dt.Rows[0]["Level1"].ToString();
                    lev2Textbox.Text = dt.Rows[0]["Level2"].ToString();
                    lev3Textbox.Text = dt.Rows[0]["Level3"].ToString();
                    urlTextbox.Text = dt.Rows[0]["ComponentURL"].ToString();
                    if (dt.Rows[0]["DisplayFlag"].ToString() == "Y")
                    {
                        displayCheckbox.Checked = true;
                    }
                    else
                    {
                        displayCheckbox.Checked = false;
                    }
                }
            }
        }

        private void okButton_Click(object sender, EventArgs e)
        {

            SqlConnection conn = new SqlConnection(Helper.ConnString("AUTOPART"));
            conn.Open();
            SqlDataAdapter adptr = new SqlDataAdapter($@"
                    DECLARE @part VARCHAR(MAX)
                    SET @part = '{part}'                             
                
                    SELECT Display_Desc FROM MERI_Kit_Component MKC WHERE MKC.ComponentSKU != '{part}' AND Display_Desc = '{descTextbox.Text}'
            "
            , conn);
            adptr.Fill(dt1);
            conn.Close();

            if (dt1.Rows.Count > 0)
            {
                MessageBox.Show($@"This part has a description that is already in use! Please give this part a unique description.", "Notice!");
                dt1.Clear();
                return;
            }

            if(lev1Textbox.Text == "")
            {
                MessageBox.Show("Level1 can not be blank!");
                return;
            }

            if(lev2Textbox.Text == "")
            {
                MessageBox.Show("Level2 can not be blank!");
                return;
            }

            if(descTextbox.Text == "")
            {
                MessageBox.Show("The part description can not be blank!");
                return;
            }

            using (SqlCommand cmd = new SqlCommand("sp_MERI_updateKitComponentfromPFM", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add("@ComponentSKU", SqlDbType.VarChar).Value = partTextbox.Text;
                cmd.Parameters.Add("@displayDescription", SqlDbType.VarChar).Value = descTextbox.Text;
                cmd.Parameters.Add("@mktDescription", SqlDbType.VarChar).Value = mktTextbox.Text;
                cmd.Parameters.Add("@ImageURL", SqlDbType.VarChar).Value = urlTextbox.Text;
                cmd.Parameters.Add("@MFR_SKU", SqlDbType.VarChar).Value = mfrTextbox.Text;
                cmd.Parameters.Add("@brand", SqlDbType.VarChar).Value = brandTextbox.Text;
                cmd.Parameters.Add("@Lev1", SqlDbType.VarChar).Value = lev1Textbox.Text;
                cmd.Parameters.Add("@Lev2", SqlDbType.VarChar).Value = lev2Textbox.Text;
                cmd.Parameters.Add("@Lev3", SqlDbType.VarChar).Value = lev3Textbox.Text;
                if (displayCheckbox.Checked)
                {
                    cmd.Parameters.Add("@DisplayFlag", SqlDbType.VarChar).Value = "Y";
                }
                else
                {
                    cmd.Parameters.Add("@DisplayFlag", SqlDbType.VarChar).Value = "N";
                }

                try
                {
                    conn.Open();
                    cmd.ExecuteNonQuery();
                    conn.Close();
                    cmd.Parameters.Clear();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }



            using (SqlCommand cmd = new SqlCommand("sp_MERI_InsertIntoKitUpdatefromApparatus", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add("@KitSKU", SqlDbType.VarChar).Value = partTextbox.Text;

                try
                {
                    conn.Open();
                    cmd.ExecuteNonQuery();
                    conn.Close();
                    cmd.Parameters.Clear();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }


            MessageBox.Show($@"Part {part} has been updated!");

            part = "";
            partTextbox.Text = "";
            pgTextbox.Text = "";
            rangeTextbox.Text = "";
            descTextbox.Text = "";
            mktTextbox.Text = "";
            mfrTextbox.Text = "";
            brandTextbox.Text = "";
            lev1Textbox.Text = "";
            lev2Textbox.Text = "";
            lev3Textbox.Text = "";
            urlTextbox.Text = "";
            displayCheckbox.Checked = false;

            partTextbox.Focus();
            pgTextbox.Enabled = false;
            rangeTextbox.Enabled = false;
            descTextbox.Enabled = false;
            mktTextbox.Enabled = false;
            mfrTextbox.Enabled = false;
            brandTextbox.Enabled = false;
            lev1Textbox.Enabled = false;
            lev2Textbox.Enabled = false;
            lev3Textbox.Enabled = false;
            urlTextbox.Enabled = false;
            displayCheckbox.Enabled = false;
            okButton.Enabled = false;



        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
