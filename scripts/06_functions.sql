USE ECommerceDB;
GO

-- =============================================
-- FUNCTIONS IMPLEMENTATION
-- =============================================

PRINT 'Creating Functions...';
GO

-- Function 1: Calculate Cart Total for a User
-- Returns the total price of all items in a user's cart
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
-- Returns the number of orders a user has placed
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
-- Returns 1 if product has sufficient stock, 0 otherwise
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