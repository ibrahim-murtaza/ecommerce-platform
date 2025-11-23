USE ECommerceDB;
GO

-- =============================================
-- VIEWS IMPLEMENTATION
-- =============================================

PRINT 'Creating Views...';
GO

-- View 1: Active Products with Stock
-- Shows all active products that are currently in stock
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
-- Comprehensive product listing with category information
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
-- Products that need restocking (less than 10 units)
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