-- =============================================
-- MASTER SCRIPT - E-COMMERCE DATABASE
-- Group 22 - Phase 2
-- Executes all database creation scripts in order
-- =============================================

PRINT '========================================';
PRINT 'STARTING DATABASE SETUP';
PRINT '========================================';
GO


PRINT 'Dropping existing database if present...';
GO

USE master;
GO

IF EXISTS (SELECT name FROM sys.databases WHERE name = 'ECommerceDB')
BEGIN
    ALTER DATABASE ECommerceDB SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    DROP DATABASE ECommerceDB;
    PRINT 'Existing database dropped.';
END
ELSE
BEGIN
    PRINT 'No existing database found.';
END
GO


PRINT '';
PRINT '========================================';
PRINT 'STEP 1: CREATING DATABASE';
PRINT '========================================';
GO

CREATE DATABASE ECommerceDB;
GO

USE ECommerceDB;
GO

PRINT 'Database created successfully!';
GO


PRINT '';
PRINT '========================================';
PRINT 'STEP 2: CREATING TABLES';
PRINT '========================================';
GO

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

CREATE TABLE Category (
    CategoryID INT PRIMARY KEY IDENTITY(1,1),
    CategoryName VARCHAR(100) NOT NULL UNIQUE,
    Description VARCHAR(500),
    IsActive BIT DEFAULT 1
);

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

PRINT 'Tables created successfully!';
GO



PRINT '';
PRINT '========================================';
PRINT 'STEP 3: CREATING PARTITIONED TABLES';
PRINT '========================================';
GO

PRINT 'Creating Partition Function...';
GO

CREATE PARTITION FUNCTION pf_OrderDate (DATETIME)
AS RANGE RIGHT FOR VALUES ('2025-01-01', '2026-01-01');
GO

PRINT 'Creating Partition Scheme...';
GO


CREATE PARTITION SCHEME ps_OrderDate
AS PARTITION pf_OrderDate
ALL TO ([PRIMARY]);
GO

PRINT 'Creating Partitioned Order Table...';
GO

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

PRINT 'Partitioned tables created successfully!';
GO


PRINT '';
PRINT '========================================';
PRINT 'STEP 4: CREATING VIEWS';
PRINT '========================================';
GO

-- View 1: Active Products with Stock
CREATE VIEW vw_ActiveProducts AS
SELECT 
    p.ProductID,
    p.ProductName,
    p.Description,
    p.Price,
    p.StockQuantity,
    c.CategoryName,
    p.DateAdded
FROM Product p
INNER JOIN Category c ON p.CategoryID = c.CategoryID
WHERE p.IsActive = 1 AND p.StockQuantity > 0;
GO

-- View 2: Product Catalog with Category Details
CREATE VIEW vw_ProductCatalog AS
SELECT 
    p.ProductID,
    p.ProductName,
    p.Description AS ProductDescription,
    p.Price,
    p.StockQuantity,
    p.ImageURL,
    p.DateAdded,
    c.CategoryID,
    c.CategoryName,
    c.Description AS CategoryDescription,
    CASE 
        WHEN p.StockQuantity = 0 THEN 'Out of Stock'
        WHEN p.StockQuantity < 10 THEN 'Low Stock'
        ELSE 'In Stock'
    END AS StockStatus
FROM Product p
INNER JOIN Category c ON p.CategoryID = c.CategoryID
WHERE p.IsActive = 1 AND c.IsActive = 1;
GO

-- View 3: Low Stock Products
CREATE VIEW vw_LowStockProducts AS
SELECT 
    p.ProductID,
    p.ProductName,
    c.CategoryName,
    p.StockQuantity,
    p.Price
FROM Product p
INNER JOIN Category c ON p.CategoryID = c.CategoryID
WHERE p.StockQuantity < 10 AND p.StockQuantity > 0 AND p.IsActive = 1;
GO

PRINT 'Views created successfully!';
GO


PRINT '';
PRINT '========================================';
PRINT 'STEP 5: CREATING INDEXES';
PRINT '========================================';
GO

-- Index 1: Non-Clustered Index on Product.CategoryID
CREATE NONCLUSTERED INDEX IX_Product_CategoryID
ON Product(CategoryID)
INCLUDE (ProductName, Price, StockQuantity);
GO

-- Index 2: Non-Clustered Index on User.Email
CREATE NONCLUSTERED INDEX IX_User_Email
ON [User](Email)
INCLUDE (FirstName, LastName, IsActive);
GO

-- Index 3: Composite Index on Product (IsActive, StockQuantity)
CREATE NONCLUSTERED INDEX IX_Product_Active_Stock
ON Product(IsActive, StockQuantity)
INCLUDE (ProductName, Price, CategoryID);
GO

-- Index 4: Non-Clustered Index on Cart.UserID
CREATE NONCLUSTERED INDEX IX_Cart_UserID
ON Cart(UserID)
INCLUDE (ProductID, Quantity, DateAdded);
GO

PRINT 'Indexes created successfully!';
GO


PRINT '';
PRINT '========================================';
PRINT 'STEP 6: CREATING FUNCTIONS';
PRINT '========================================';
GO

-- Function 1: Calculate Cart Total for a User
CREATE FUNCTION dbo.CalculateCartTotal(@UserID INT)
RETURNS DECIMAL(10,2)
AS
BEGIN
    DECLARE @Total DECIMAL(10,2);
    
    SELECT @Total = SUM(p.Price * c.Quantity)
    FROM Cart c
    INNER JOIN Product p ON c.ProductID = p.ProductID
    WHERE c.UserID = @UserID;
    
    RETURN ISNULL(@Total, 0);
END;
GO

-- Function 2: Get User's Total Order Count
CREATE FUNCTION dbo.GetUserOrderCount(@UserID INT)
RETURNS INT
AS
BEGIN
    DECLARE @OrderCount INT;
    
    SELECT @OrderCount = COUNT(*)
    FROM [Order]
    WHERE UserID = @UserID;
    
    RETURN ISNULL(@OrderCount, 0);
END;
GO

-- Function 3: Check Product Stock Availability
CREATE FUNCTION dbo.CheckStockAvailability(@ProductID INT, @RequestedQuantity INT)
RETURNS BIT
AS
BEGIN
    DECLARE @Available BIT;
    DECLARE @CurrentStock INT;
    
    SELECT @CurrentStock = StockQuantity
    FROM Product
    WHERE ProductID = @ProductID;
    
    IF @CurrentStock >= @RequestedQuantity
        SET @Available = 1;
    ELSE
        SET @Available = 0;
    
    RETURN ISNULL(@Available, 0);
END;
GO

PRINT 'Functions created successfully!';
GO


PRINT '';
PRINT '========================================';
PRINT 'STEP 7: CREATING TRIGGERS';
PRINT '========================================';
GO

-- Trigger 1: AFTER Trigger - Update Stock After Order
CREATE TRIGGER trg_AfterOrderItem_UpdateStock
ON OrderItem
AFTER INSERT
AS
BEGIN
    SET NOCOUNT ON;
    
    UPDATE p
    SET p.StockQuantity = p.StockQuantity - i.Quantity
    FROM Product p
    INNER JOIN inserted i ON p.ProductID = i.ProductID;
    
    PRINT 'Stock updated after order placement.';
END;
GO

-- Trigger 2: INSTEAD OF Trigger - Validate Cart Insert
CREATE TRIGGER trg_InsteadOfCart_ValidateStock
ON Cart
INSTEAD OF INSERT
AS
BEGIN
    SET NOCOUNT ON;
    
    IF EXISTS (
        SELECT 1
        FROM inserted i
        INNER JOIN Product p ON i.ProductID = p.ProductID
        WHERE p.StockQuantity < i.Quantity OR p.IsActive = 0
    )
    BEGIN
        RAISERROR('Cannot add to cart: Product is out of stock or inactive.', 16, 1);
        ROLLBACK TRANSACTION;
        RETURN;
    END
    
    INSERT INTO Cart (UserID, ProductID, Quantity, DateAdded)
    SELECT UserID, ProductID, Quantity, DateAdded
    FROM inserted;
    
    PRINT 'Cart item added successfully.';
END;
GO

-- Trigger 3: AFTER Trigger - Log Product Price Changes
CREATE TRIGGER trg_AfterProduct_PriceUpdate
ON Product
AFTER UPDATE
AS
BEGIN
    SET NOCOUNT ON;
    
    IF UPDATE(Price)
    BEGIN
        DECLARE @ProductID INT, @OldPrice DECIMAL(10,2), @NewPrice DECIMAL(10,2);
        
        SELECT 
            @ProductID = i.ProductID,
            @NewPrice = i.Price,
            @OldPrice = d.Price
        FROM inserted i
        INNER JOIN deleted d ON i.ProductID = d.ProductID
        WHERE i.Price <> d.Price;
        
        PRINT 'Price updated for ProductID: ' + CAST(@ProductID AS VARCHAR) + 
              ' from ' + CAST(@OldPrice AS VARCHAR) + ' to ' + CAST(@NewPrice AS VARCHAR);
    END
END;
GO

PRINT 'Triggers created successfully!';
GO


PRINT '';
PRINT '========================================';
PRINT 'STEP 8: CREATING STORED PROCEDURES';
PRINT '========================================';
GO

-- Stored Procedure 1: Place Order
CREATE PROCEDURE sp_PlaceOrder
    @UserID INT,
    @ShippingAddress VARCHAR(500),
    @ShippingCity VARCHAR(100),
    @ShippingPostalCode VARCHAR(20)
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRANSACTION;
    
    BEGIN TRY
        DECLARE @OrderID INT;
        DECLARE @TotalAmount DECIMAL(10,2);
        DECLARE @OrderDate DATETIME = GETDATE();
        
        SELECT @TotalAmount = SUM(p.Price * c.Quantity)
        FROM Cart c
        INNER JOIN Product p ON c.ProductID = p.ProductID
        WHERE c.UserID = @UserID;
        
        IF @TotalAmount IS NULL OR @TotalAmount = 0
        BEGIN
            RAISERROR('Cart is empty. Cannot place order.', 16, 1);
            RETURN;
        END
        
        INSERT INTO [Order] (UserID, OrderDate, TotalAmount, Status, ShippingAddress, ShippingCity, ShippingPostalCode)
        VALUES (@UserID, @OrderDate, @TotalAmount, 'Pending', @ShippingAddress, @ShippingCity, @ShippingPostalCode);
        
        SET @OrderID = SCOPE_IDENTITY();
        
        INSERT INTO OrderItem (OrderID, OrderDate, ProductID, Quantity, PriceAtPurchase)
        SELECT 
            @OrderID,
            @OrderDate,
            c.ProductID,
            c.Quantity,
            p.Price
        FROM Cart c
        INNER JOIN Product p ON c.ProductID = p.ProductID
        WHERE c.UserID = @UserID;
        
        DELETE FROM Cart WHERE UserID = @UserID;
        
        COMMIT TRANSACTION;
        
        PRINT 'Order placed successfully. OrderID: ' + CAST(@OrderID AS VARCHAR);
        SELECT @OrderID AS OrderID, @TotalAmount AS TotalAmount;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END;
GO

-- Stored Procedure 2: Update Order Status
CREATE PROCEDURE sp_UpdateOrderStatus
    @OrderID INT,
    @NewStatus VARCHAR(50)
AS
BEGIN
    SET NOCOUNT ON;
    
    IF @NewStatus NOT IN ('Pending', 'Processing', 'Shipped', 'Delivered', 'Cancelled')
    BEGIN
        RAISERROR('Invalid status. Must be: Pending, Processing, Shipped, Delivered, or Cancelled', 16, 1);
        RETURN;
    END
    
    IF NOT EXISTS (SELECT 1 FROM [Order] WHERE OrderID = @OrderID)
    BEGIN
        RAISERROR('Order not found.', 16, 1);
        RETURN;
    END
    
    UPDATE [Order]
    SET Status = @NewStatus
    WHERE OrderID = @OrderID;
    
    PRINT 'Order status updated successfully.';
END;
GO

-- Stored Procedure 3: Get Top Selling Products
CREATE PROCEDURE sp_GetTopSellingProducts
    @TopN INT = 10
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT TOP (@TopN)
        p.ProductID,
        p.ProductName,
        c.CategoryName,
        SUM(oi.Quantity) AS TotalSold,
        SUM(oi.Quantity * oi.PriceAtPurchase) AS TotalRevenue,
        AVG(oi.PriceAtPurchase) AS AveragePrice
    FROM OrderItem oi
    INNER JOIN Product p ON oi.ProductID = p.ProductID
    INNER JOIN Category c ON p.CategoryID = c.CategoryID
    GROUP BY p.ProductID, p.ProductName, c.CategoryName
    ORDER BY TotalSold DESC;
END;
GO

PRINT 'Stored Procedures created successfully!';
GO


PRINT '';
PRINT '========================================';
PRINT 'STEP 9: CTE DEMONSTRATIONS';
PRINT '========================================';
PRINT 'CTEs are query-time constructs.';
PRINT 'Example queries can be run after data is loaded.';
PRINT 'See 09_ctes.sql for CTE examples.';
GO


PRINT '';
PRINT '========================================';
PRINT 'DATABASE SETUP COMPLETE!';
PRINT '========================================';
PRINT '';
PRINT 'Next Steps:';
PRINT '1. Run Python data generation scripts in scripts/data_generation/';
PRINT '2. Run Python data loading scripts to populate tables';
PRINT '3. Verify data with: SELECT COUNT(*) FROM [TableName]';
PRINT '';
PRINT 'Total Expected Rows: 1,060,150';
PRINT '  - Category: 50';
PRINT '  - Product: 100,000';
PRINT '  - User: 10,000';
PRINT '  - Admin: 100';
PRINT '  - Cart: 50,000';
PRINT '  - Order: 500,000';
PRINT '  - OrderItem: 400,000';
PRINT '';
GO