Resubmission of Login, Registration and Logout with C# from the CRUD project (Login-and-Register-main)

This project initially used microsoft access OLEDB. And now I have connected it to microsoft sql server (local) by doing some modification to existing project files. 

Firstly, since it's connecting with the database, I have openned the SSMS and created a new database from the provided SQL code that follows: 
CREATE DATABASE db_users;
GO

USE db_users;
GO

CREATE TABLE tbl_users (
    id       INT IDENTITY(1,1) PRIMARY KEY,
    username NVARCHAR(50)  NOT NULL UNIQUE,
    password NVARCHAR(100) NOT NULL
);
GO

Then created a test user following 

INSERT INTO tbl_users (username, password) VALUES ('admin', 'admin123');
GO

(This sql code can be found in the database.sql file, included in this repository)

The database and the table follows this structure: 

ScreenShots/databaseStructure.png 

Now for the connection, initially in the app.config file, a connection string tag was used and the database connection string was added there with the name connString. Extra code for it was this: 
<connectionStrings>
    <add name="connString"
         connectionString="Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=db_users;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False" />
  </connectionStrings>

Here for the data source, I used the connection string name here. Since the localDB and mssql sequence does not work in my virtual machine, but it does work with the pipeline name. 

As this project now uses SQL connection, we have used the libraries, System.Configuration and System.Data.SqlCliet at the top of the project. It was added with the frmLogin and frmRegistration file here. 

Initially the System.Configuration library was not added in the references, had to enable them from the checkbox found by right clicking the reference item in the solution explorer. 

Now the old project used oledb microsoft access connection. 

So the code contained the parameters for it. Like the OleDB library and the initializers. So first the System.Data.OleDB was removed and then all the parameters for it, like 
        OleDbConnection con = new OleDbConnection("Provider = Microsoft.Jet.OLEDB.4.0; Data Source =  np:\\\\.\\pipe\\LOCALDB#2E83B907\\tsql\\query;");
        OleDbCommand cmd = new OleDbCommand();
        OleDbDataAdapter da = new OleDbDataAdapter();
was removed. 

Instead it was replaced with parameters needed for the Sql connection. Like 
private static string myConn =
    ConfigurationManager.ConnectionStrings["connString"].ConnectionString;

Here the connection string was actually assigned with the variable called myConn to avoid repetitive usage of the raw connection string and make the code more efficient. 

and 

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

Both frmLogin and frmRegister had to be modified with the parameters here. 

The login and register buttons also used the general oleDB connection parameters so those were modified too. 

In the frmRegister file, some extended sql parameters were added, as it had to check whether the user is already registered or not and all the parameters are correct or not, so here is the following code which is a bit different than the frmLogin 
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

In the frmRegister, I have modified the file a bit, and added a feature to open frmLogin page as soon as the user successfully registers in the app, as initially the app cleared the page and stayed stationary. It's just a UX improvement over the initial code. 

The logout button was initially coded to exit the application rather than actually loggin out. It was hard coded with 
        private void btnLogout_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Goodbye Sayan");
            Application.Exit();
        }

So modified it and made it trully log out instead of closing, by showing the login page once the user confirms he/she wants to log out with this portion of code: 
            DialogResult result = MessageBox.Show("Are you sure you want to logout?", "Logout",
                                          MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                new frmLogin().Show();
                this.Close();
            }

The application was tested using the given checkbox and all the features worked correctly. 