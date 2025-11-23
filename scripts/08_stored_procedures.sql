USE ECommerceDB;
GO

-- =============================================
-- STORED PROCEDURES IMPLEMENTATION
-- =============================================

PRINT 'Creating Stored Procedures...';
GO

-- Stored Procedure 1: Place Order
-- Creates an order and associated order items from user's cart
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
        
        -- Calculate total amount from cart
        SELECT @TotalAmount = SUM(p.Price * c.Quantity)
        FROM Cart c
        INNER JOIN Product p ON c.ProductID = p.ProductID
        WHERE c.UserID = @UserID;
        
        -- Check if cart is empty
        IF @TotalAmount IS NULL OR @TotalAmount = 0
        BEGIN
            RAISERROR('Cart is empty. Cannot place order.', 16, 1);
            RETURN;
        END
        
        -- Create order
        INSERT INTO [Order] (UserID, OrderDate, TotalAmount, Status, ShippingAddress, ShippingCity, ShippingPostalCode)
        VALUES (@UserID, @OrderDate, @TotalAmount, 'Pending', @ShippingAddress, @ShippingCity, @ShippingPostalCode);
        
        SET @OrderID = SCOPE_IDENTITY();
        
        -- Create order items from cart
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
        
        -- Clear the cart
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
-- Updates the status of an order with validation
CREATE PROCEDURE sp_UpdateOrderStatus
    @OrderID INT,
    @NewStatus VARCHAR(50)
AS
BEGIN
    SET NOCOUNT ON;
    
    -- Validate status
    IF @NewStatus NOT IN ('Pending', 'Processing', 'Shipped', 'Delivered', 'Cancelled')
    BEGIN
        RAISERROR('Invalid status. Must be: Pending, Processing, Shipped, Delivered, or Cancelled', 16, 1);
        RETURN;
    END
    
    -- Check if order exists
    IF NOT EXISTS (SELECT 1 FROM [Order] WHERE OrderID = @OrderID)
    BEGIN
        RAISERROR('Order not found.', 16, 1);
        RETURN;
    END
    
    -- Update status
    UPDATE [Order]
    SET Status = @NewStatus
    WHERE OrderID = @OrderID;
    
    PRINT 'Order status updated successfully.';
END;
GO

-- Stored Procedure 3: Get Top Selling Products
-- Returns top N products by total quantity sold
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