IF DB_ID(N'db_furniture') IS NULL
BEGIN
    CREATE DATABASE db_furniture;
END
GO

USE db_furniture;
GO

IF OBJECT_ID(N'dbo.Account', N'U') IS NOT NULL DROP TABLE dbo.Account;
IF OBJECT_ID(N'dbo.Users', N'U') IS NOT NULL DROP TABLE dbo.Users;
IF OBJECT_ID(N'dbo.Categories', N'U') IS NOT NULL DROP TABLE dbo.Categories;
IF OBJECT_ID(N'dbo.Products', N'U') IS NOT NULL DROP TABLE dbo.Products;
IF OBJECT_ID(N'dbo.Showrooms', N'U') IS NOT NULL DROP TABLE dbo.Showrooms;
IF OBJECT_ID(N'dbo.Rooms', N'U') IS NOT NULL DROP TABLE dbo.Rooms;
IF OBJECT_ID(N'dbo.InventoryItems', N'U') IS NOT NULL DROP TABLE dbo.InventoryItems;
IF OBJECT_ID(N'dbo.Promotions', N'U') IS NOT NULL DROP TABLE dbo.Promotions;
IF OBJECT_ID(N'dbo.Orders', N'U') IS NOT NULL DROP TABLE dbo.Orders;
IF OBJECT_ID(N'dbo.OrderDetails', N'U') IS NOT NULL DROP TABLE dbo.OrderDetails;
IF OBJECT_ID(N'dbo.Payments', N'U') IS NOT NULL DROP TABLE dbo.Payments;
GO

CREATE TABLE dbo.Account
(
    Account_ID           BIGINT IDENTITY(1,1) PRIMARY KEY,
    UserName             NVARCHAR(255)  NOT NULL,
    Password_Hash        VARBINARY(512) NOT NULL,
    Password_Salt        VARBINARY(128) NOT NULL,
    Password_Algo        NVARCHAR(20)   NOT NULL CONSTRAINT DF_Account_Algo DEFAULT(N'PBKDF2'),
    Password_Iterations  INT            NOT NULL CONSTRAINT DF_Account_Iters DEFAULT(100000),
    Role                 NVARCHAR(10)   NOT NULL,
    Status               NVARCHAR(10)   NOT NULL CONSTRAINT DF_Account_Status DEFAULT(N'Active'),
    Create_At            DATETIME2(7)   NOT NULL CONSTRAINT DF_Account_CreateAt DEFAULT (SYSUTCDATETIME()),
    CONSTRAINT UQ_Account_UserName UNIQUE (UserName),
    CONSTRAINT CK_Account_Role CHECK (Role IN (N'Admin', N'User')),
    CONSTRAINT CK_Account_Status CHECK (Status IN (N'Active', N'Inactive', N'Banned')),
    CONSTRAINT CK_Account_Iters CHECK (Password_Iterations >= 10000)
);
GO

CREATE TABLE dbo.Users
(
    User_ID    BIGINT IDENTITY(1,1) PRIMARY KEY,
    User_Name  NVARCHAR(100) NOT NULL,
    User_Email NVARCHAR(150) NOT NULL,
    User_Phone NVARCHAR(15)  NOT NULL,
    Address    NVARCHAR(255) NULL,
    Account_ID BIGINT NULL REFERENCES dbo.Account(Account_ID)
);
GO

CREATE TABLE dbo.Categories
(
    Category_ID   BIGINT IDENTITY(1,1) PRIMARY KEY,
    Category_Name NVARCHAR(120) NOT NULL UNIQUE,
    Category_Slug NVARCHAR(160) NOT NULL,
    Description   NVARCHAR(500) NULL
);
GO

CREATE TABLE dbo.Products
(
    Product_ID          BIGINT IDENTITY(1,1) PRIMARY KEY,
    Product_Slug        NVARCHAR(255) NOT NULL,
    Product_Name        NVARCHAR(255) NOT NULL,
    Brand               NVARCHAR(255) NOT NULL DEFAULT N'Generic',
    Product_Image       NVARCHAR(500) NOT NULL,
    Product_Description NVARCHAR(2000) NULL,
    Price               DECIMAL(18,2) NOT NULL DEFAULT 0,
    Category_ID         BIGINT NOT NULL REFERENCES dbo.Categories(Category_ID)
);
GO

CREATE TABLE dbo.Showrooms
(
    Showroom_ID   BIGINT IDENTITY(1,1) PRIMARY KEY,
    Showroom_Name NVARCHAR(50) UNIQUE NOT NULL,
    [Location]    NVARCHAR(255) NOT NULL,
    Contact_Info  NVARCHAR(200) NOT NULL
);
GO

CREATE TABLE dbo.Rooms
(
    Room_ID     BIGINT IDENTITY(1,1) PRIMARY KEY,
    Showroom_ID BIGINT NOT NULL REFERENCES dbo.Showrooms(Showroom_ID),
    Room_Name   NVARCHAR(50) NOT NULL,
    Capacity    INT NOT NULL DEFAULT 100
);
GO

CREATE TABLE dbo.InventoryItems
(
    Inventory_ID BIGINT IDENTITY(1,1) PRIMARY KEY,
    Room_ID      BIGINT NOT NULL REFERENCES dbo.Rooms(Room_ID),
    Product_ID   BIGINT NOT NULL REFERENCES dbo.Products(Product_ID),
    Quantity     INT NOT NULL DEFAULT 0,
    [Condition]  NVARCHAR(20) NOT NULL DEFAULT N'New'
);
GO

CREATE TABLE dbo.Promotions
(
    Promotion_ID BIGINT IDENTITY(1,1) PRIMARY KEY,
    Product_ID   BIGINT NOT NULL REFERENCES dbo.Products(Product_ID),
    Start_Time   DATETIME2 NOT NULL,
    End_Time     DATETIME2 NOT NULL,
    [Type]       NVARCHAR(50) NOT NULL CHECK ([Type] IN (N'Percent', N'Amount')),
    Discount     INT NOT NULL DEFAULT 10,
    PromoPrice   DECIMAL(18,2) NOT NULL DEFAULT 0
);
GO

CREATE TABLE dbo.Orders
(
    Order_ID    BIGINT IDENTITY(1,1) PRIMARY KEY,
    User_ID     BIGINT NOT NULL REFERENCES dbo.Users(User_ID),
    Create_Time DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
    [Status]    NVARCHAR(50) NOT NULL DEFAULT N'Pending'
);
GO

CREATE TABLE dbo.OrderDetails
(
    OrderDetail_ID BIGINT IDENTITY(1,1) PRIMARY KEY,
    Order_ID       BIGINT NOT NULL REFERENCES dbo.Orders(Order_ID) ON DELETE CASCADE,
    Product_ID     BIGINT NOT NULL REFERENCES dbo.Products(Product_ID),
    UnitPrice      DECIMAL(18,2) NOT NULL,
    Quantity       INT NOT NULL
);
GO

CREATE TABLE dbo.Payments
(
    Payment_ID    BIGINT IDENTITY(1,1) PRIMARY KEY,
    [User_ID]     BIGINT NOT NULL REFERENCES dbo.Users(User_ID),
    Order_ID      BIGINT NOT NULL REFERENCES dbo.Orders(Order_ID),
    Amount        INT NOT NULL,
    PaymentMethod NVARCHAR(50) NOT NULL CHECK (PaymentMethod IN (N'Card', N'BankTransfer', N'EWallet', N'Cash', N'Paypal', N'Other')),
    PaymentStatus NVARCHAR(50) NOT NULL CHECK (PaymentStatus IN (N'Pending', N'Completed', N'Failed', N'Cancelled', N'Refunded')),
    PaymentTime   DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME()
);
GO

-- Seed admin account (admin / admin123)
INSERT INTO dbo.Account (UserName, Password_Hash, Password_Salt, Password_Algo, Password_Iterations, Role, Status)
VALUES (N'admin', 0XC7B2C3644A0764617F287DB8785A906E910E7E5E89B4EE9FE05272ABDEBCC123, 0X0A6E3C18180B7D63B491F3E3C69D350B, N'PBKDF2', 100000, N'Admin', N'Active');
GO

-- Seed categories
INSERT INTO dbo.Categories (Category_Name, Category_Slug, Description) VALUES
(N'Sofa', N'sofa', N'Sofa phòng khách'),
(N'Bàn', N'ban', N'Bàn gỗ tự nhiên'),
(N'Ghế', N'ghe', N'Ghế ăn, ghế làm việc'),
(N'Tủ - Kệ', N'tu-ke', N'Tủ quần áo, kệ tivi');
GO

-- Seed products
INSERT INTO dbo.Products (Product_Slug, Product_Name, Brand, Product_Image, Product_Description, Price, Category_ID) VALUES
(N'sofa-ni-goc-l', N'Sofa Nỉ Góc L', N'FurniCo', N'/uploads/posters/sofa1.jpg', N'Sofa nỉ cao cấp', 8900000, 1),
(N'ban-tra-go-soi', N'Bàn Trà Gỗ Sồi', N'OakHouse', N'/uploads/posters/ban1.jpg', N'Bàn trà phong cách Bắc Âu', 2900000, 2),
(N'ghe-an-boc-da', N'Ghế Ăn Bọc Da', N'Seatly', N'/uploads/posters/ghe1.jpg', N'Ghế bọc da êm ái', 1200000, 3);
GO

-- Seed showrooms & rooms
INSERT INTO dbo.Showrooms (Showroom_Name, [Location], Contact_Info) VALUES
(N'Furni District 1', N'Q.1, TP.HCM', N'028-123-4567'),
(N'Furni HaNoi', N'Ba Đình, Hà Nội', N'024-765-4321');

INSERT INTO dbo.Rooms (Showroom_ID, Room_Name, Capacity) VALUES
(1, N'Phòng Trưng Bày 1', 150),
(1, N'Phòng Trưng Bày 2', 100),
(2, N'Phòng Trưng Bày 1', 120);
GO

-- Seed users
INSERT INTO dbo.Users (User_Name, User_Email, User_Phone, Address, Account_ID)
VALUES (N'Khách Lẻ', N'guest@example.com', N'0900000000', N'HCM', NULL);
GO
