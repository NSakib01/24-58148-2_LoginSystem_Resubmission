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
    public partial class frmLogin : Form
    {
        public frmLogin()
        {
            InitializeComponent();
        }

        private static string myConn =
    ConfigurationManager.ConnectionStrings["connString"].ConnectionString;


        private void btnLogin_Click(object sender, EventArgs e)
        {
           if (txtUsername.Text.Trim() == "" || txtPassword.Text == "")
    {
        MessageBox.Show("Please enter both username and password.");
        return;
    }

    try
    {
        using (SqlConnection con = new SqlConnection(myConn))
        {
            con.Open();

            string login = "SELECT COUNT(*) FROM tbl_users WHERE username = @username AND password = @password";

            using (SqlCommand cmd = new SqlCommand(login, con))
            {
                cmd.Parameters.AddWithValue("@username", txtUsername.Text.Trim());
                cmd.Parameters.AddWithValue("@password", txtPassword.Text);

                int count = Convert.ToInt32(cmd.ExecuteScalar());

                if (count == 1)
                {
                    new frmDashboard().Show();
                    this.Hide();
                }
                else
                {
                    MessageBox.Show("Wrong username or password, please try again.",
                                    "Login Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    txtUsername.Text = "";
                    txtPassword.Text = "";
                    txtUsername.Focus();
                }
            }
        }
    }
    catch (Exception ex)
    {
        MessageBox.Show("Database error:\n\n" + ex.Message);
    }

        }

        private void checkbxShowPas_CheckedChanged(object sender, EventArgs e)
        {
            if (checkbxShowPas.Checked)
            {
                txtPassword.PasswordChar = '\0';             
            }
            else
            {
                txtPassword.PasswordChar = '•';
            }
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            txtUsername.Text = "";
            txtPassword.Text = "";
            txtUsername.Focus();
        }

        private void clickRegister_Click(object sender, EventArgs e)
        {
            new frmRegister().Show();
            this.Hide();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Goodbye");
            Application.Exit();
        }

        private void frmLogin_Load(object sender, EventArgs e)
        {

        }
    }
}
