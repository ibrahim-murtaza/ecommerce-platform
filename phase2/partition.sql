
USE master;
GO

CREATE DATABASE ECommerceDB;
GO

USE ECommerceDB;
GO

-- User table
CREATE TABLE [User] (
    UserID INT PRIMARY KEY IDENTITY(1,1),
    Email VARCHAR(255) NOT NULL UNIQUE,
    PasswordHash VARCHAR(255) NOT NULL,
    FirstName VARCHAR(100) NOT NULL,
    LastName VARCHAR(100) NOT NULL,
    PhoneNumber VARCHAR(20),
    Address VARCHAR(500),
    City VARCHAR(100),
    PostalCode VARCHAR(20),
    DateJoined DATETIME DEFAULT GETDATE(),
    IsActive BIT DEFAULT 1
);
GO

-- Category table
CREATE TABLE Category (
    CategoryID INT PRIMARY KEY IDENTITY(1,1),
    CategoryName VARCHAR(100) NOT NULL UNIQUE,
    Description VARCHAR(500),
    IsActive BIT DEFAULT 1
);
GO

-- Product table
CREATE TABLE Product (
    ProductID INT PRIMARY KEY IDENTITY(1,1),
    CategoryID INT NOT NULL,
    ProductName VARCHAR(200) NOT NULL,
    Description TEXT,
    Price DECIMAL(10,2) NOT NULL CHECK (Price >= 0),
    StockQuantity INT NOT NULL DEFAULT 0 CHECK (StockQuantity >= 0),
    ImageURL VARCHAR(500),
    DateAdded DATETIME DEFAULT GETDATE(),
    IsActive BIT DEFAULT 1,
    CONSTRAINT FK_Product_Category FOREIGN KEY (CategoryID) 
        REFERENCES Category(CategoryID)
);
GO

-- Cart table
CREATE TABLE Cart (
    CartID INT PRIMARY KEY IDENTITY(1,1),
    UserID INT NOT NULL,
    ProductID INT NOT NULL,
    Quantity INT NOT NULL DEFAULT 1 CHECK (Quantity > 0),
    DateAdded DATETIME DEFAULT GETDATE(),
    CONSTRAINT FK_Cart_User FOREIGN KEY (UserID) 
        REFERENCES [User](UserID) ON DELETE CASCADE,
    CONSTRAINT FK_Cart_Product FOREIGN KEY (ProductID) 
        REFERENCES Product(ProductID)
);
GO

-- Admin table
CREATE TABLE Admin (
    AdminID INT PRIMARY KEY IDENTITY(1,1),
    Email VARCHAR(255) NOT NULL UNIQUE,
    PasswordHash VARCHAR(255) NOT NULL,
    FirstName VARCHAR(100) NOT NULL,
    LastName VARCHAR(100) NOT NULL,
    Role VARCHAR(50) DEFAULT 'Admin',
    DateCreated DATETIME DEFAULT GETDATE(),
    IsActive BIT DEFAULT 1
);
GO

-- =============================================
-- SECTION 1: TABLE PARTITIONING 

PRINT 'Creating Partition Function...';
GO


-- This creates 3 partitions based on OrderDate:
CREATE PARTITION FUNCTION pf_OrderDate (DATETIME)
AS RANGE RIGHT FOR VALUES ('2025-01-01', '2026-01-01');
GO

PRINT 'Creating Partition Scheme...';
GO

-- Create Partition Scheme
-- Maps all partitions to PRIMARY filegroup
CREATE PARTITION SCHEME ps_OrderDate
AS PARTITION pf_OrderDate
ALL TO ([PRIMARY]);
GO

PRINT 'Creating Partitioned Order Table...';
GO

-- Create Order table WITH partitioning
-- Note: Primary key MUST include the partition key (OrderDate)
CREATE TABLE [Order] (
    OrderID INT IDENTITY(1,1),
    UserID INT NOT NULL,
    OrderDate DATETIME DEFAULT GETDATE(),
    TotalAmount DECIMAL(10,2) NOT NULL CHECK (TotalAmount >= 0),
    Status VARCHAR(50) NOT NULL DEFAULT 'Pending',
    ShippingAddress VARCHAR(500) NOT NULL,
    ShippingCity VARCHAR(100),
    ShippingPostalCode VARCHAR(20),
    CONSTRAINT PK_Order PRIMARY KEY (OrderID, OrderDate),
    CONSTRAINT FK_Order_User FOREIGN KEY (UserID) 
        REFERENCES [User](UserID),
    CONSTRAINT CHK_Order_Status CHECK (Status IN ('Pending', 'Processing', 'Shipped', 'Delivered', 'Cancelled'))
) ON ps_OrderDate(OrderDate);
GO

PRINT 'Creating Partitioned OrderItem Table...';
GO

-- Create OrderItem table WITH partitioning
-- OrderDate column added to align with Order table partition key
CREATE TABLE OrderItem (
    OrderItemID INT IDENTITY(1,1),
    OrderID INT NOT NULL,
    OrderDate DATETIME NOT NULL,
    ProductID INT NOT NULL,
    Quantity INT NOT NULL CHECK (Quantity > 0),
    PriceAtPurchase DECIMAL(10,2) NOT NULL CHECK (PriceAtPurchase >= 0),
    CONSTRAINT PK_OrderItem PRIMARY KEY (OrderItemID, OrderDate),
    CONSTRAINT FK_OrderItem_Order FOREIGN KEY (OrderID, OrderDate) 
        REFERENCES [Order](OrderID, OrderDate) ON DELETE CASCADE,
    CONSTRAINT FK_OrderItem_Product FOREIGN KEY (ProductID) 
        REFERENCES Product(ProductID)
) ON ps_OrderDate(OrderDate);
GO

PRINT 'Table Partitioning completed successfully!';
GO

PRINT '';
PRINT '========================================';
PRINT 'PHASE 2 - SECTION 1: PARTITIONING COMPLETE';
PRINT '========================================';
GO

-- ===================
