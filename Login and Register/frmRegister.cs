using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Configuration;


namespace Login_and_Register
{
    public partial class frmRegister : Form
    {
        public frmRegister()
        {
            InitializeComponent();
        }

        private static string myConn =
    ConfigurationManager.ConnectionStrings["connString"].ConnectionString;


        private void btnRegister_Click(object sender, EventArgs e)
        {
            if (txtUsername.Text.Trim() == "" || txtPassword.Text == "" || txtConPassword.Text == "")
            {
                MessageBox.Show("Username and password fields cannot be empty.",
                                "Register Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (txtPassword.Text != txtConPassword.Text)
            {
                MessageBox.Show("Passwords do not match, please re-enter.",
                                "Register Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                txtPassword.Text = "";
                txtConPassword.Text = "";
                txtPassword.Focus();
                return;
            }

            try
            {
                using (SqlConnection con = new SqlConnection(myConn))
                {
                    con.Open();

                    // is the username already taken?
                    using (SqlCommand check =
                        new SqlCommand("SELECT COUNT(*) FROM tbl_users WHERE username = @username", con))
                    {
                        check.Parameters.AddWithValue("@username", txtUsername.Text.Trim());

                        if (Convert.ToInt32(check.ExecuteScalar()) > 0)
                        {
                            MessageBox.Show("That username is already taken.");
                            txtUsername.Focus();
                            return;
                        }
                    }

                    // insert the new user
                    string register = "INSERT INTO tbl_users (username, password) VALUES (@username, @password)";

                    using (SqlCommand cmd = new SqlCommand(register, con))
                    {
                        cmd.Parameters.AddWithValue("@username", txtUsername.Text.Trim());
                        cmd.Parameters.AddWithValue("@password", txtPassword.Text);
                        cmd.ExecuteNonQuery();
                    }
                }

                MessageBox.Show("Your account has been successfully created.",
                                "Registration Success, Please Login!", MessageBoxButtons.OK, MessageBoxIcon.Information);
                new frmLogin().Show();
                this.Hide();

                txtUsername.Text = "";
                txtPassword.Text = "";
                txtConPassword.Text = "";
                txtUsername.Focus();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Database error:\n\n" + ex.Message);
            }

        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            txtUsername.Text = "";
            txtPassword.Text = "";
            txtConPassword.Text = "";
            txtUsername.Focus();
        }

        private void clickLogin_Click(object sender, EventArgs e)
        {
            new frmLogin().Show();
            this.Hide();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Goodbye");
            Application.Exit();
        }

        private void txtUsername_TextChanged(object sender, EventArgs e)
        {

        }

        private void frmRegister_Load(object sender, EventArgs e)
        {

        }
    }
}
