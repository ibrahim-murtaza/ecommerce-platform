# E-Commerce Database - Group 22

## Team Members
- Ibrahim Murtaza - 26100050
- Jibran Mazhar - 26100225
- Muhammad Ahsan Khan Lodhi - 26100096
- Muhammad Arsalan Amjad - 26100003

## Project Overview
A comprehensive e-commerce database system built with Microsoft SQL Server, featuring partitioned tables, advanced SQL features, over 1 million rows of data, and a full .NET application with dual Business Logic Layer implementations (LINQ vs Stored Procedures) using the Factory Design Pattern. Developed for CS 340 - Databases course at LUMS.

## Project Structure
```
ecommerce-platform/
├── database/
│   └── schema/
│       ├── 01_create_database.sql
│       └── 02_create_tables.sql
├── scripts/
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
└── Phase_3/
    ├── ECommerce.Models/           # Entity models (User, Product, Order, etc.)
    ├── ECommerce.DAL/              # Data Access Layer with EF Core
    ├── ECommerce.BLL/              # Business Logic Layer
    │   ├── Interfaces/             # Service interfaces
    │   ├── LinqImplementation/     # EF Core + LINQ implementations
    │   └── SPImplementation/       # Stored Procedure implementations
    ├── ECommerce.Factory/          # Factory pattern for BLL selection
    └── ECommerce.UI/               # Avalonia UI application
```

## Prerequisites
- Docker Desktop (for SQL Server container)
- .NET 9 SDK
- Python 3.8+
- pyodbc library: `pip install pyodbc`
- VS Code with SQL Server (mssql) extension

---

# Phase 1 & 2: Database Implementation

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
   'PWD=YourStrong!Passw0rd;'
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

---

# Phase 3: Application Development

## Overview
Phase 3 implements a complete .NET application with Avalonia UI that demonstrates the Factory Design Pattern by supporting two interchangeable Business Logic Layer implementations: LINQ (using Entity Framework Core) and Stored Procedures (using raw SQL).

## Technology Stack
- **Framework:** .NET 9 (cross-platform)
- **UI:** Avalonia UI 11.3.9
- **ORM:** Entity Framework Core 9.0
- **Database:** Microsoft SQL Server (Docker)
- **Pattern:** Factory Design Pattern

## Project Architecture

### Layered Architecture
```
ECommerce.UI (Presentation)
    ↓
ECommerce.Factory (Factory Pattern)
    ↓
ECommerce.BLL (Business Logic - Interfaces + 2 Implementations)
    ↓
ECommerce.DAL (Data Access with EF Core)
    ↓
ECommerce.Models (Entity Models)
    ↓
SQL Server Database
```

### Project Components

#### 1. ECommerce.Models
Entity models representing database tables:
- `User.cs` - Customer accounts
- `Product.cs` - Product catalog
- `Category.cs` - Product categories
- `Order.cs` - Customer orders (partitioned)
- `OrderItem.cs` - Order line items (partitioned)
- `Cart.cs` - Shopping cart items
- `Admin.cs` - Admin accounts

#### 2. ECommerce.DAL (Data Access Layer)
- `ECommerceContext.cs` - EF Core DbContext
- Configures composite keys for partitioned tables
- Connection string management
- Database connection: `Server=127.0.0.1,1433;Database=ECommerceDB;User Id=sa;Password=YourStrong!Passw0rd;TrustServerCertificate=True;`

#### 3. ECommerce.BLL (Business Logic Layer)

**Interfaces** (5 total):
- `IProductService` - Product CRUD, filtering, low stock alerts
- `IUserService` - User management, authentication
- `IOrderService` - Order processing, status updates
- `ICartService` - Cart operations, total calculation
- `ICategoryService` - Category management

**LINQ Implementation** (`LinqImplementation/`):
- Uses Entity Framework Core
- LINQ queries for all operations
- Manual transaction management
- Example: `ProductServiceLINQ.cs`, `UserServiceLINQ.cs`, etc.

**Stored Procedure Implementation** (`SPImplementation/`):
- Uses raw SQL and ExecuteSqlRaw
- Calls stored procedures (`sp_PlaceOrder`, `sp_UpdateOrderStatus`)
- Uses database functions (`CalculateCartTotal`, `CheckStockAvailability`)
- Uses views (`vw_LowStockProducts`, `vw_ActiveProducts`)
- Example: `ProductServiceSP.cs`, `UserServiceSP.cs`, etc.

#### 4. ECommerce.Factory
**Factory Pattern Implementation:**
```csharp
public enum BLLType { LINQ, StoredProcedure }

public static class BLLFactory
{
    public static IProductService GetProductService(BLLType type)
    {
        var context = new ECommerceContext();
        return type == BLLType.LINQ 
            ? new ProductServiceLINQ(context)
            : new ProductServiceSP(context);
    }
    // Similar methods for other services...
}
```

#### 5. ECommerce.UI
Avalonia-based user interface (cross-platform).

## Setup and Running

### Prerequisites
1. **Start SQL Server Docker Container:**
   ```bash
   docker start sqlserver
   ```
   
   Or create new container:
   ```bash
   docker run -e "ACCEPT_EULA=Y" -e "SA_PASSWORD=YourStrong!Passw0rd" -p 1433:1433 --name sqlserver -v sqlserverdata:/var/opt/mssql -d mcr.microsoft.com/mssql/server
   ```

2. **Install .NET 9 SDK:**
   - Download from https://dotnet.microsoft.com/download

3. **Verify Database:**
   - Ensure Phase 1 & 2 database is set up with all data

### Build and Run

```bash
# Navigate to Phase_3 directory
cd Phase_3

# Restore dependencies
dotnet restore

# Build solution
dotnet build

# Run application
cd ECommerce.UI
dotnet run
```

## Using the Application

### Accessing Services via Factory

Always use the Factory pattern to get service instances:

```csharp
using ECommerce.Factory;

// Get LINQ implementation
var productService = BLLFactory.GetProductService(BLLType.LINQ);
var products = productService.GetAllProducts();

// Get Stored Procedure implementation
var orderService = BLLFactory.GetOrderService(BLLType.StoredProcedure);
orderService.PlaceOrder(userId, address, city, postal);

// Switch at runtime
BLLType currentType = BLLType.LINQ;
var cartService = BLLFactory.GetCartService(currentType);
```

### Available Service Methods

**IProductService:**
- `GetAllProducts()` - Retrieve all active products
- `GetProductById(int id)` - Get single product
- `GetProductsByCategory(int categoryId)` - Filter by category
- `AddProduct(Product)` / `UpdateProduct(Product)` / `DeleteProduct(int id)`
- `GetLowStockProducts()` - Uses `vw_LowStockProducts` view

**IUserService:**
- `GetAllUsers()` / `GetUserById(int id)` / `GetUserByEmail(string email)`
- `AddUser(User)` / `UpdateUser(User)` / `DeleteUser(int id)`
- `ValidateUserCredentials(string email, string passwordHash)` - Login

**IOrderService:**
- `GetAllOrders()` / `GetOrderById(int orderId, DateTime orderDate)`
- `GetOrdersByUserId(int userId)` / `GetOrdersByStatus(string status)`
- `PlaceOrder(...)` - Uses `sp_PlaceOrder` stored procedure (SP version)
- `UpdateOrderStatus(...)` - Uses `sp_UpdateOrderStatus` stored procedure (SP version)
- `GetTotalRevenueByDateRange(DateTime start, DateTime end)`

**ICartService:**
- `GetCartByUserId(int userId)`
- `AddToCart(int userId, int productId, int quantity)` - Validates stock
- `UpdateCartItemQuantity(int cartId, int newQuantity)`
- `RemoveFromCart(int cartId)` / `ClearCart(int userId)`
- `GetCartTotal(int userId)` - Uses `CalculateCartTotal` function (SP version)

**ICategoryService:**
- `GetAllCategories()` / `GetCategoryById(int id)` / `GetActiveCategories()`
- `AddCategory(Category)` / `UpdateCategory(Category)` / `DeleteCategory(int id)`

### Key Differences: LINQ vs Stored Procedures

| Feature | LINQ Implementation | SP Implementation |
|---------|-------------------|-------------------|
| Query Method | EF Core + LINQ | Raw SQL + ExecuteSqlRaw |
| Transactions | Manual (BeginTransaction) | Built into stored procedures |
| Stock Updates | Manual in code | Automatic via triggers |
| Cart Total | LINQ Sum calculation | `CalculateCartTotal` function |
| Order Placement | Multi-step LINQ | `sp_PlaceOrder` procedure |
| Views | FromSqlRaw on views | Direct SELECT from views |
| Performance | Good for simple queries | Better for complex operations |

## SQL Server Features Usage

The application leverages all Phase 2 database features:

1. **Partitioned Tables:** Order and OrderItem queries work with partitioned data
2. **Views:** `GetLowStockProducts()` uses `vw_LowStockProducts`
3. **Indexes:** All queries benefit from indexes (CategoryID, Email, Active/Stock)
4. **Functions:** `GetCartTotal()` calls `CalculateCartTotal` function (SP version)
5. **Triggers:** 
   - Stock auto-updates on order (SP version relies on trigger)
   - Cart validation trigger (bypassed in LINQ, used in SP)
6. **Stored Procedures:** `PlaceOrder()` and `UpdateOrderStatus()` use stored procedures (SP version)

## Testing

### Manual Testing
The application has been tested with:
- ✅ Database connection successful
- ✅ LINQ implementation: Retrieved 100,000 products, 50 categories
- ✅ SP implementation: Retrieved 94,967 active products, 50 categories
- ✅ Factory pattern working correctly
- ✅ Both implementations functional

### To Test Services:
```csharp
// Test LINQ
var productServiceLinq = BLLFactory.GetProductService(BLLType.LINQ);
var products = productServiceLinq.GetAllProducts();
Console.WriteLine($"LINQ found {products.Count} products");

// Test SP
var productServiceSp = BLLFactory.GetProductService(BLLType.StoredProcedure);
var productsSp = productServiceSp.GetAllProducts();
Console.WriteLine($"SP found {productsSp.Count} products");
```

## Troubleshooting

### Common Issues

**Issue:** "Server was not found or was not accessible"
**Solution:** Start Docker container: `docker start sqlserver`

**Issue:** Build errors about nullable reference types
**Solution:** Already fixed with `!` operators and null coalescing

**Issue:** Different row counts between LINQ and SP
**Solution:** Expected behavior - SP version filters by `IsActive = 1`

**Issue:** Connection string errors
**Solution:** Verify password matches Docker container in `ECommerceContext.cs`

**Issue:** EF Core errors about composite keys
**Solution:** Order and OrderItem use composite keys (OrderID + OrderDate) for partitioning

### Build Issues
If you get `.NET 10.0 not supported` error:
- All projects should target `net9.0` in `.csproj` files
- EF Core packages should be version `9.0.0`

## Phase 3 Work Distribution
- **Arsalan:** Project structure, Models, DAL setup, initial configuration
- **Ibrahim:** All BLL interfaces and implementations (LINQ + SP), Factory pattern
- **Ahsan:** UI development (in progress)
- **Jibran:** Integration testing (in progress)

## Phase 3 Implementation Notes

### Key Design Decisions
1. **Factory Pattern:** Enables runtime switching between LINQ and SP implementations
2. **Interface Segregation:** Each service has a dedicated interface
3. **Consistent API:** Both implementations provide identical functionality
4. **Cross-Platform:** .NET 9 + Avalonia supports Windows, Mac, Linux
5. **Connection Management:** Each service instantiation gets a new DbContext

### Testing Strategy
- Unit test individual service methods
- Integration test complete workflows
- Runtime switching validation
- Performance comparison between LINQ and SP

---

# Testing Queries

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

---

# Summary

## Phase 1 & 2 (Database): Complete ✅
- SQL Server database with 1M+ rows
- Partitioned tables (Order, OrderItem)
- Views, Indexes, Functions, Triggers, Stored Procedures, CTEs
- Comprehensive data generation scripts

## Phase 3 (Application): Core Complete ✅
- .NET 9 + Avalonia UI application
- Factory Design Pattern implemented
- Dual BLL: LINQ (EF Core) + Stored Procedures
- 5 service interfaces with full implementations
- Database integration verified
- UI development in progress
- Integration testing in progress

## Normalization
Database is normalized to 3NF:
- No repeating groups
- All non-key attributes fully dependent on primary key
- No transitive dependencies

## License
Academic project for CS 340 - Databases, LUMS (Fall 2025)