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

/*Creating a test user*/

INSERT INTO tbl_users (username, password) VALUES ('admin', 'admin123');
GO
