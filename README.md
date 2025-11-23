# E-Commerce Database - Group 22

## Team Members
- Ibrahim Murtaza - 26100050
- Jibran Mazhar - 26100225
- Muhammad Ahsan Khan Lodhi - 26100096
- Muhammad Arsalan Amjad - 26100003

## Project Overview
A comprehensive e-commerce database system built with Microsoft SQL Server, featuring partitioned tables, advanced SQL features, and over 1 million rows of data. Developed for CS 340 - Databases course at LUMS.

## Project Structure
```
ecommerce-platform/
├── database/
│   └── schema/
│       ├── 01_create_database.sql
│       └── 02_create_tables.sql
├── scripts/
│   ├── 01_create_database.sql
│   ├── 02_create_tables.sql
│   ├── 03_partition_tables.sql
│   ├── 04_views.sql
│   ├── 05_indexes.sql
│   ├── 06_functions.sql
│   ├── 07_triggers.sql
│   ├── 08_stored_procedures.sql
│   ├── 09_ctes.sql
│   ├── master_script.sql
│   └── data_generation/
│       ├── generate_categories.py
│       ├── generate_products.py
│       ├── load_categories_products.py
│       ├── generate_users.py
│       ├── generate_admins.py
│       ├── generate_cart.py
│       ├── load_users_cart.py
│       ├── generate_orders.py
│       ├── generate_order_items.py
│       └── load_orders.py
└── src/
```

## Prerequisites
- Docker Desktop (for SQL Server container)
- Python 3.8+
- pyodbc library: `pip install pyodbc`
- VS Code with SQL Server (mssql) extension

## Database Setup

### Option 1: Master Script (Recommended for TA/Clean Setup)
Run the combined script that creates everything:
```sql
-- From the scripts/ directory, execute:
master_script.sql
```

### Option 2: Individual Scripts (Development/Testing)
Execute SQL scripts in VS Code in this order:

1. **Create Database & Tables**
   ```bash
   scripts/01_create_database.sql
   scripts/02_create_tables.sql
   scripts/03_partition_tables.sql
   ```

2. **Create Database Objects**
   ```bash
   scripts/04_views.sql
   scripts/05_indexes.sql
   scripts/06_functions.sql
   scripts/07_triggers.sql
   scripts/08_stored_procedures.sql
   scripts/09_ctes.sql
   ```

3. **Generate & Load Data**
   
   Navigate to `scripts/data_generation/` and run:
   
   ```bash
   # Generate CSVs
   python generate_categories.py
   python generate_products.py
   python generate_users.py
   python generate_admins.py
   python generate_cart.py
   python generate_orders.py
   python generate_order_items.py
   
   # Load into database
   python load_categories_products.py
   python load_users_cart.py
   python load_orders.py
   ```
   
   **Note:** Update the database password in Python scripts before running:
   ```python
   'PWD=YourStrong!Passw0rd;'  # Replace with your SQL Server SA password
   ```

4. **Verify Data Load**
   ```sql
   SELECT 'Category' AS TableName, COUNT(*) AS [RowCount] FROM Category
   UNION ALL SELECT 'Product', COUNT(*) FROM Product
   UNION ALL SELECT 'User', COUNT(*) FROM [User]
   UNION ALL SELECT 'Admin', COUNT(*) FROM Admin
   UNION ALL SELECT 'Cart', COUNT(*) FROM Cart
   UNION ALL SELECT 'Order', COUNT(*) FROM [Order]
   UNION ALL SELECT 'OrderItem', COUNT(*) FROM OrderItem;
   ```
   
   **Expected Total: 1,060,150 rows**

## Database Schema

### Core Tables

#### User
Customer accounts with authentication and profile information.
- **Primary Key:** UserID
- **Unique:** Email
- **Key Fields:** Email, PasswordHash, FirstName, LastName, Address, City

#### Category
Product categorization structure.
- **Primary Key:** CategoryID
- **Key Fields:** CategoryName, Description, IsActive

#### Product
Product catalog with inventory management.
- **Primary Key:** ProductID
- **Foreign Keys:** CategoryID → Category
- **Key Fields:** ProductName, Price, StockQuantity, ImageURL
- **Indexes:** IX_Product_CategoryID, IX_Product_Active_Stock

#### Cart
Active shopping cart items for users.
- **Primary Key:** CartID
- **Foreign Keys:** UserID → User, ProductID → Product
- **Key Fields:** UserID, ProductID, Quantity
- **Indexes:** IX_Cart_UserID
- **Note:** Includes INSTEAD OF trigger for stock validation

#### Order (Partitioned)
Customer orders with shipping details.
- **Primary Key:** (OrderID, OrderDate) - Composite for partitioning
- **Foreign Keys:** UserID → User
- **Key Fields:** OrderDate, TotalAmount, Status, ShippingAddress
- **Partitioning:** By OrderDate (3 partitions: <2025, 2025-2025, >2026)

#### OrderItem (Partitioned)
Individual items within orders.
- **Primary Key:** (OrderItemID, OrderDate) - Composite for partitioning
- **Foreign Keys:** (OrderID, OrderDate) → Order, ProductID → Product
- **Key Fields:** ProductID, Quantity, PriceAtPurchase
- **Partitioning:** Aligned with Order table by OrderDate
- **Note:** AFTER trigger updates product stock on insert

#### Admin
Administrative user accounts.
- **Primary Key:** AdminID
- **Key Fields:** Email, Role, IsActive

### Relationships
- User (1) → (N) Cart, Order
- Category (1) → (N) Product
- Product (1) → (N) Cart, OrderItem
- Order (1) → (N) OrderItem

## Advanced SQL Features Implemented

### 1. Table Partitioning
- **Tables:** Order, OrderItem
- **Partition Key:** OrderDate
- **Strategy:** RANGE RIGHT partitioning
- **Partitions:** 3 (historical, current, future)
- **Purpose:** Improved query performance for date-based queries

### 2. Views (3 implementations)
- **vw_ActiveProducts:** Active products with stock > 0
- **vw_ProductCatalog:** Complete product listing with category info and stock status
- **vw_LowStockProducts:** Products needing restocking (stock < 10)

### 3. Indexes (4 implementations)
- **IX_Product_CategoryID:** Non-clustered on Product.CategoryID
- **IX_User_Email:** Non-clustered on User.Email (login optimization)
- **IX_Product_Active_Stock:** Composite on (IsActive, StockQuantity)
- **IX_Cart_UserID:** Non-clustered on Cart.UserID

### 4. Functions (3 implementations)
- **CalculateCartTotal(@UserID):** Returns total cart value for a user
- **GetUserOrderCount(@UserID):** Returns number of orders placed by user
- **CheckStockAvailability(@ProductID, @RequestedQuantity):** Validates stock availability

### 5. Triggers (3 implementations)
- **trg_AfterOrderItem_UpdateStock (AFTER):** Automatically reduces stock when order placed
- **trg_InsteadOfCart_ValidateStock (INSTEAD OF):** Validates stock before adding to cart
- **trg_AfterProduct_PriceUpdate (AFTER):** Logs product price changes

### 6. Stored Procedures (3 implementations)
- **sp_PlaceOrder:** Creates order from cart, handles transaction, clears cart
- **sp_UpdateOrderStatus:** Updates order status with validation
- **sp_GetTopSellingProducts:** Returns top N products by sales volume

### 7. Common Table Expressions (CTEs)
Multiple CTE examples in `09_ctes.sql`:
- **UserOrderSummary:** Order statistics by user
- **ProductSales + CategoryStats:** Multi-level product performance analysis
- **OrderStatusFlow:** Order status tracking with hierarchical levels

## Data Generation

### Data Distribution
| Table | Row Count | Generation Method |
|-------|-----------|-------------------|
| Category | 50 | Python script |
| Product | 100,000 | Python script |
| User | 10,000 | Python script |
| Admin | 100 | Python script |
| Cart | 50,000 | Python script |
| Order | 500,000 | Python script |
| OrderItem | 400,000 | Python script |
| **Total** | **1,060,150** | |

### Data Generation Features
- Realistic names, emails, addresses
- Random but valid product prices ($5-$9999)
- Stock quantities (0-1000)
- Order dates spanning 2 years
- Status distribution: 65% Delivered, 15% Shipped, 10% Processing, 5% Pending, 5% Cancelled
- Order items aligned with order dates for partitioning

## Troubleshooting

### Trigger Issues During Data Load
If you encounter trigger validation errors when loading cart data:

1. Disable the cart validation trigger:
   ```sql
   USE ECommerceDB;
   GO
   DISABLE TRIGGER trg_InsteadOfCart_ValidateStock ON Cart;
   GO
   ```

2. Run the data loading script

3. Re-enable the trigger:
   ```sql
   ENABLE TRIGGER trg_InsteadOfCart_ValidateStock ON Cart;
   GO
   ```

**Note:** The `load_users_cart.py` script handles this automatically.

### Connection Issues
If Python scripts fail to connect:
- Verify SQL Server container is running: `docker ps`
- Check connection string server name (usually `localhost` or `localhost,1433`)
- Verify SA password matches your Docker container setup
- Ensure ODBC Driver 17 for SQL Server is installed

### Performance Tips
- Data loading takes approximately 15-20 minutes for 1M+ rows
- Triggers are temporarily disabled during bulk inserts for performance
- Use batch sizes of 1000-5000 rows for optimal performance

## Testing Queries

### View All Products in a Category
```sql
SELECT * FROM vw_ProductCatalog 
WHERE CategoryName = 'Electronics';
```

### Calculate User's Cart Total
```sql
SELECT dbo.CalculateCartTotal(1) AS CartTotal;
```

### Get Top 10 Selling Products
```sql
EXEC sp_GetTopSellingProducts @TopN = 10;
```

### Check Partition Distribution
```sql
SELECT 
    p.partition_number,
    p.rows,
    rv.value AS partition_boundary
FROM sys.partitions p
INNER JOIN sys.indexes i ON p.object_id = i.object_id AND p.index_id = i.index_id
LEFT JOIN sys.partition_range_values rv ON p.partition_number = rv.boundary_id + 1
WHERE i.object_id = OBJECT_ID('[Order]')
AND i.index_id <= 1;
```

### User Order History (CTE Example)
```sql
WITH UserOrders AS (
    SELECT 
        o.OrderID,
        o.OrderDate,
        o.TotalAmount,
        o.Status,
        COUNT(oi.OrderItemID) AS ItemCount
    FROM [Order] o
    INNER JOIN OrderItem oi ON o.OrderID = oi.OrderID AND o.OrderDate = oi.OrderDate
    WHERE o.UserID = 1
    GROUP BY o.OrderID, o.OrderDate, o.TotalAmount, o.Status
)
SELECT * FROM UserOrders ORDER BY OrderDate DESC;
```

## Phase 2 Implementation Notes

### Work Distribution
- **Arsalan:** Table Partitioning (Order, OrderItem)
- **Ahsan:** Views, Indexes, Category/Product data generation
- **Jibran:** Functions, Triggers, User/Admin/Cart data generation
- **Ibrahim:** Stored Procedures, CTEs, Order/OrderItem data generation, Integration

### Key Design Decisions
1. **Partitioning Strategy:** Date-based partitioning on orders for improved query performance on time-range queries
2. **Composite Primary Keys:** Required for partitioned tables (OrderID + OrderDate)
3. **Trigger Management:** Automatic disable/enable during bulk loads for performance
4. **Data Generation:** Python + CSV approach for speed and reliability over pure SQL
5. **Stock Management:** Trigger-based automatic stock updates on order placement

### Normalization
Database is normalized to 3NF:
- No repeating groups
- All non-key attributes fully dependent on primary key
- No transitive dependencies

## License
Academic project for CS 340 - Databases, LUMS (Fall 2025)