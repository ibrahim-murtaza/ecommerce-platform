USE ECommerceDB;
GO

-- =============================================
-- TRIGGERS IMPLEMENTATION
-- =============================================

PRINT 'Creating Triggers...';
GO

-- Trigger 1: AFTER Trigger - Update Stock After Order
-- Automatically reduces product stock when an order item is inserted
CREATE TRIGGER trg_AfterOrderItem_UpdateStock
ON OrderItem
AFTER INSERT
AS
BEGIN
    SET NOCOUNT ON;
    
    -- Update product stock for each inserted order item
    UPDATE p
    SET p.StockQuantity = p.StockQuantity - i.Quantity
    FROM Product p
    INNER JOIN inserted i ON p.ProductID = i.ProductID;
    
    PRINT 'Stock updated after order placement.';
END;
GO

-- Trigger 2: INSTEAD OF Trigger - Validate Cart Insert
-- Prevents adding items to cart if product is out of stock
CREATE TRIGGER trg_InsteadOfCart_ValidateStock
ON Cart
INSTEAD OF INSERT
AS
BEGIN
    SET NOCOUNT ON;
    
    -- Check if any products are out of stock
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
    
    -- If validation passes, insert the cart items
    INSERT INTO Cart (UserID, ProductID, Quantity, DateAdded)
    SELECT UserID, ProductID, Quantity, DateAdded
    FROM inserted;
    
    PRINT 'Cart item added successfully.';
END;
GO

-- Trigger 3: AFTER Trigger - Log Product Price Changes
-- Creates audit trail when product prices are updated
CREATE TRIGGER trg_AfterProduct_PriceUpdate
ON Product
AFTER UPDATE
AS
BEGIN
    SET NOCOUNT ON;
    
    -- Only log if price actually changed
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
        
        -- In production, you'd log this to an audit table
        -- For now, just print a message
        PRINT 'Price updated for ProductID: ' + CAST(@ProductID AS VARCHAR) + 
              ' from ' + CAST(@OldPrice AS VARCHAR) + ' to ' + CAST(@NewPrice AS VARCHAR);
    END
END;
GO

PRINT 'Triggers created successfully!';
GO