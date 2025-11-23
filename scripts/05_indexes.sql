USE ECommerceDB;
GO

-- =============================================
-- INDEXES IMPLEMENTATION
-- =============================================

PRINT 'Creating Indexes...';
GO

-- Index 1: Non-Clustered Index on Product.CategoryID
-- Improves performance for category-based product queries
CREATE NONCLUSTERED INDEX IX_Product_CategoryID
ON Product(CategoryID)
INCLUDE (ProductName, Price, StockQuantity);
GO

-- Index 2: Non-Clustered Index on User.Email
-- Speeds up user login and email lookup operations
CREATE NONCLUSTERED INDEX IX_User_Email
ON [User](Email)
INCLUDE (FirstName, LastName, IsActive);
GO

-- Index 3: Composite Index on Product (IsActive, StockQuantity)
-- Optimizes queries filtering by active products with stock
CREATE NONCLUSTERED INDEX IX_Product_Active_Stock
ON Product(IsActive, StockQuantity)
INCLUDE (ProductName, Price, CategoryID);
GO

-- Index 4: Non-Clustered Index on Cart.UserID
-- Improves cart retrieval performance for users
CREATE NONCLUSTERED INDEX IX_Cart_UserID
ON Cart(UserID)
INCLUDE (ProductID, Quantity, DateAdded);
GO

PRINT 'Indexes created successfully!';
GO