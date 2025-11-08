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

-- Category table
CREATE TABLE Category (
    CategoryID INT PRIMARY KEY IDENTITY(1,1),
    CategoryName VARCHAR(100) NOT NULL UNIQUE,
    Description VARCHAR(500),
    IsActive BIT DEFAULT 1
);

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

-- Order table
CREATE TABLE [Order] (
    OrderID INT PRIMARY KEY IDENTITY(1,1),
    UserID INT NOT NULL,
    OrderDate DATETIME DEFAULT GETDATE(),
    TotalAmount DECIMAL(10,2) NOT NULL CHECK (TotalAmount >= 0),
    Status VARCHAR(50) NOT NULL DEFAULT 'Pending',
    ShippingAddress VARCHAR(500) NOT NULL,
    ShippingCity VARCHAR(100),
    ShippingPostalCode VARCHAR(20),
    CONSTRAINT FK_Order_User FOREIGN KEY (UserID) 
        REFERENCES [User](UserID),
    CONSTRAINT CHK_Order_Status CHECK (Status IN ('Pending', 'Processing', 'Shipped', 'Delivered', 'Cancelled'))
);

-- OrderItem table
CREATE TABLE OrderItem (
    OrderItemID INT PRIMARY KEY IDENTITY(1,1),
    OrderID INT NOT NULL,
    ProductID INT NOT NULL,
    Quantity INT NOT NULL CHECK (Quantity > 0),
    PriceAtPurchase DECIMAL(10,2) NOT NULL CHECK (PriceAtPurchase >= 0),
    CONSTRAINT FK_OrderItem_Order FOREIGN KEY (OrderID) 
        REFERENCES [Order](OrderID) ON DELETE CASCADE,
    CONSTRAINT FK_OrderItem_Product FOREIGN KEY (ProductID) 
        REFERENCES Product(ProductID)
);

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