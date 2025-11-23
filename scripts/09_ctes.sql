USE ECommerceDB;
GO

-- =============================================
-- COMMON TABLE EXPRESSIONS (CTEs) IMPLEMENTATION
-- =============================================

PRINT 'Demonstrating CTEs...';
GO

-- CTE 1: Order Summary by User
-- Shows order statistics for each user
PRINT 'CTE 1: Order Summary by User';
GO

WITH UserOrderSummary AS (
    SELECT 
        u.UserID,
        u.FirstName,
        u.LastName,
        u.Email,
        COUNT(o.OrderID) AS TotalOrders,
        SUM(o.TotalAmount) AS TotalSpent,
        AVG(o.TotalAmount) AS AverageOrderValue,
        MAX(o.OrderDate) AS LastOrderDate
    FROM [User] u
    LEFT JOIN [Order] o ON u.UserID = o.UserID
    GROUP BY u.UserID, u.FirstName, u.LastName, u.Email
)
SELECT TOP 10
    UserID,
    FirstName,
    LastName,
    Email,
    TotalOrders,
    TotalSpent,
    AverageOrderValue,
    LastOrderDate
FROM UserOrderSummary
WHERE TotalOrders > 0
ORDER BY TotalSpent DESC;
GO

-- CTE 2: Product Sales Analysis
-- Analyzes product performance with category information
PRINT 'CTE 2: Product Sales Analysis';
GO

WITH ProductSales AS (
    SELECT 
        p.ProductID,
        p.ProductName,
        c.CategoryName,
        p.StockQuantity AS CurrentStock,
        COUNT(DISTINCT oi.OrderID) AS TimesOrdered,
        SUM(oi.Quantity) AS TotalQuantitySold,
        SUM(oi.Quantity * oi.PriceAtPurchase) AS TotalRevenue
    FROM Product p
    INNER JOIN Category c ON p.CategoryID = c.CategoryID
    LEFT JOIN OrderItem oi ON p.ProductID = oi.ProductID
    GROUP BY p.ProductID, p.ProductName, c.CategoryName, p.StockQuantity
),
CategoryStats AS (
    SELECT 
        CategoryName,
        AVG(TotalRevenue) AS AvgRevenuePerProduct,
        SUM(TotalRevenue) AS TotalCategoryRevenue
    FROM ProductSales
    GROUP BY CategoryName
)
SELECT TOP 20
    ps.ProductID,
    ps.ProductName,
    ps.CategoryName,
    ps.CurrentStock,
    ps.TimesOrdered,
    ps.TotalQuantitySold,
    ps.TotalRevenue,
    cs.AvgRevenuePerProduct AS CategoryAvgRevenue,
    CASE 
        WHEN ps.TotalRevenue > cs.AvgRevenuePerProduct THEN 'Above Average'
        ELSE 'Below Average'
    END AS PerformanceStatus
FROM ProductSales ps
INNER JOIN CategoryStats cs ON ps.CategoryName = cs.CategoryName
WHERE ps.TotalRevenue > 0
ORDER BY ps.TotalRevenue DESC;
GO

-- CTE 3: Recursive CTE - Order Status History (Demonstration)
-- Shows hierarchical status progression
PRINT 'CTE 3: Order Status Transitions';
GO

WITH OrderStatusFlow AS (
    SELECT 
        o.OrderID,
        o.UserID,
        o.OrderDate,
        o.Status,
        o.TotalAmount,
        CASE o.Status
            WHEN 'Pending' THEN 1
            WHEN 'Processing' THEN 2
            WHEN 'Shipped' THEN 3
            WHEN 'Delivered' THEN 4
            WHEN 'Cancelled' THEN 5
        END AS StatusLevel
    FROM [Order] o
)
SELECT TOP 15
    OrderID,
    UserID,
    OrderDate,
    Status,
    TotalAmount,
    StatusLevel,
    CASE 
        WHEN StatusLevel = 1 THEN 'Order Received'
        WHEN StatusLevel = 2 THEN 'Being Prepared'
        WHEN StatusLevel = 3 THEN 'In Transit'
        WHEN StatusLevel = 4 THEN 'Completed'
        WHEN StatusLevel = 5 THEN 'Cancelled'
    END AS StatusDescription
FROM OrderStatusFlow
ORDER BY OrderDate DESC;
GO

PRINT 'CTE demonstrations completed successfully!';
GO