# Resubmission of Login, Registration and Logout with C# from the CRUD Project (Login-and-Register-main)

This project initially used Microsoft Access OLEDB. And now I have connected it to Microsoft SQL Server (Local) by doing some modifications to the existing project files.

# To run this program: 

1. First run this sql script in SSMS
```sql
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
```
2. Create a test user, run this in sql script in ssms again

```sql
INSERT INTO tbl_users (username, password) VALUES ('admin', 'admin123');
GO
```

3. Change the connection string with your name-pipe or database name in App.config: 
<connectionStrings>
    <add name="connString"
         connectionString="Data Source=<database namepipe here or connection string here>;Initial Catalog=db_users;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False" />
</connectionStrings>

4. Go to project > rebuild and once it completes, run. 

# How I was able to complete this task: 

Firstly, since it is connecting with the database, I opened SSMS and created a new database from the provided SQL code that follows:

```sql
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
```

Then created a test user using:

```sql
INSERT INTO tbl_users (username, password) VALUES ('admin', 'admin123');
GO
```

(This SQL code can be found in the `database.sql` file included in this repository.)

The database and the table follow this structure:

![**!\[\\[SCREENSHOT HERE: Database and tbl_users structure in SSMS\\]\](ScreenShots/databaseStructure.png)**](ScreenShots/databaseStructure.png)

---

Now for the connection, in the `App.config` file, a connection string tag was used and the database connection string was added there with the name `connString`. The code for it was:

```xml
<connectionStrings>
    <add name="connString"
         connectionString="Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=db_users;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False" />
</connectionStrings>
```

Here, for the Data Source, I used my actual SQL Server connection source. Since the normal LocalDB and MSSQL sequence does not work in my virtual machine, I used the LocalDB named-pipe connection instead.

**![\[SCREENSHOT HERE: App.config showing the connection string\]](ScreenShots/databaseConnectionString.png)**

As this project now uses an SQL Server connection, I used the libraries `System.Configuration` and `System.Data.SqlClient` at the top of the project. These were added to the `frmLogin` and `frmRegister` files.

Initially, the `System.Configuration` library was not added in the References, so I had to enable it from the checkbox found by right-clicking the References item in Solution Explorer.

**![\[SCREENSHOT HERE: System.Configuration enabled in References\]](ScreenShots/enablingSystemConfig.png)**

---

Now, the old project used an OLEDB Microsoft Access connection.

So the code contained the classes needed for it, like the OLEDB library and the initializers. First, `System.Data.OleDb` was removed and then the old OLEDB connection-related code, like:

```csharp
OleDbConnection con = new OleDbConnection("Provider = Microsoft.Jet.OLEDB.4.0; Data Source = db_users.mdb");
OleDbCommand cmd = new OleDbCommand();
OleDbDataAdapter da = new OleDbDataAdapter();
```

was removed.

Instead, it was replaced with the code needed for the SQL Server connection, like:

```csharp
private static string myConn =
    ConfigurationManager.ConnectionStrings["connString"].ConnectionString;
```

Here, the connection string was assigned to a variable called `myConn` to avoid repetitive usage of the raw connection string throughout the code and make it easier to manage.

And then the SQL Server connection and login code were used:

```csharp
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
```

Both `frmLogin` and `frmRegister` had to be modified with the required SQL Server connection code.

The Login and Register buttons also used the general OLEDB connection before, so those portions were modified too.

The `@username` and `@password` values are used as SQL parameters instead of directly joining the user's input with the SQL query. This makes the query safer and helps prevent SQL injection.



---

In the `frmRegister` file, some additional SQL logic was added, as it had to check whether the user was already registered or not and whether the given information was valid.

So here is the following code, which is a bit different from `frmLogin`:

```csharp
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
                    "Registration Success, Please Login!",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

    new frmLogin().Show();
    this.Hide();

    txtUsername.Text = "";
    txtPassword.Text = "";
    txtConPassword.Text = "";
    txtUsername.Focus();
}
```

In `frmRegister`, I modified the file a bit and added a feature to open the `frmLogin` page as soon as the user successfully registers in the app. Initially, the app cleared the page and stayed stationary.

It is just a UX improvement over the initial code.

**![\[SCREENSHOT HERE: Registration form with new user information\]](<ScreenShots/Screenshot 2026-08-20 084152.png>)**

**![\[SCREENSHOT HERE: Successful registration message / return to Login page\]](<ScreenShots/Screenshot 2026-08-20 084244.png>)**

---

The Logout button was initially coded to exit the application rather than actually logging out. It was hard-coded with:

```csharp
private void btnLogout_Click(object sender, EventArgs e)
{
    MessageBox.Show("Goodbye Sayan");
    Application.Exit();
}
```

So I modified it and made it truly log out instead of closing the whole application, by showing the Login page once the user confirms that he/she wants to log out with this portion of code:

```csharp
DialogResult result = MessageBox.Show("Are you sure you want to logout?", "Logout",
                                      MessageBoxButtons.YesNo,
                                      MessageBoxIcon.Question);

if (result == DialogResult.Yes)
{
    new frmLogin().Show();
    this.Close();
}
```

**![\[SCREENSHOT HERE: Dashboard after successful login\]](<ScreenShots/Screenshot 2026-08-20 084433.png>)**

**[![SCREENSHOT HERE: Logout confirmation message\]](<ScreenShots/Screenshot 2026-08-20 084440.png>)**

**![\[SCREENSHOT HERE: Login page after logout\]](<ScreenShots/Screenshot 2026-08-20 084450.png>)**

---

The application was tested using the given checklist, and all the features worked correctly.

The application was tested by:

* Logging in using the provided `admin / admin123` test account.
* Trying an incorrect password and confirming that the login was rejected.
* Registering a new user.
* Logging in using the newly registered user.
* Logging out and confirming that the application returned to the Login page.
* Checking `db.tbl_users` in SQL Server to confirm that the newly registered user was actually stored in the database.

**![\[SCREENSHOT HERE: tbl_users showing the newly registered user\]](<ScreenShots/Screenshot 2026-08-20 084544.png>)**
