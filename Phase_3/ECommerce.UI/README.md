# E-Commerce Platform - Frontend UI

## Overview

This is the frontend application for the E-Commerce Platform built with **Avalonia UI** framework. The application provides a complete shopping experience with user authentication, product browsing, shopping cart, checkout, and an admin dashboard.

## Features

### User Features

- **User Authentication**: Login and registration system
- **Product Browsing**: View all products with category filtering
- **Product Details**: Detailed product view with add-to-cart functionality
- **Shopping Cart**: Manage cart items, update quantities, and view total
- **Checkout**: Complete orders with shipping information
- **Low Stock View**: View products with low inventory

### Admin Features

- **Low Stock Products**: View all products below stock threshold
- **Order Management**: View all orders with status filtering
- **Revenue Analytics**: Calculate total revenue for date ranges
- **System Statistics**: Overview of products, orders, and users

### Technical Features

- **BLL Type Switching**: Toggle between LINQ and Stored Procedure implementations at runtime
- **Factory Pattern**: Clean separation using BLL Factory for service instantiation
- **Session Management**: User session handling throughout the app
- **Responsive UI**: Modern, clean interface with Fluent Design

## Project Structure

```
ECommerce.UI/
├── Views/
│   ├── LoginWindow.axaml/.cs          # User login and registration
│   ├── ProductDetailWindow.axaml/.cs   # Product detail view
│   ├── CartWindow.axaml/.cs            # Shopping cart management
│   ├── CheckoutWindow.axaml/.cs        # Order checkout
│   └── AdminDashboardWindow.axaml/.cs  # Admin dashboard
├── ViewModels/
│   └── ViewModelBase.cs                # Base view model class
├── Helpers/
│   ├── SessionManager.cs               # User session management
│   └── BLLManager.cs                   # BLL type management
├── App.axaml/.cs                       # Application entry point
├── MainWindow.axaml/.cs                # Product listing (main page)
└── Program.cs                          # Application bootstrap
```

## Prerequisites

Before running the application, ensure you have:

1. **.NET 9.0 SDK** or later installed

   - Download from: https://dotnet.microsoft.com/download

2. **SQL Server** with the E-Commerce database set up

   - Database should be created and populated using the SQL scripts in the `database/` and `scripts/` folders

3. **Visual Studio 2022** or **VS Code** (optional but recommended)

4. **Connection String Configuration**
   - The connection string is configured in `ECommerce.DAL/ECommerceContext.cs`
   - Default: `Server=localhost;Database=ECommerceDB;Trusted_Connection=True;TrustServerCertificate=True;`
   - Update if your SQL Server configuration is different

## Setup Instructions

### Step 1: Verify Database Setup

Ensure your SQL Server database is set up:

```bash
# Navigate to the project root
cd /Users/ahsankhan7503/Desktop/ecommerce-platform

# Run the database scripts in order (if not already done):
# 1. database/schema/01_create_database.sql
# 2. database/schema/02_create_tables.sql
# 3. scripts/03_partition_tables.sql
# 4. scripts/04_views.sql
# 5. scripts/05_indexes.sql
# 6. scripts/06_functions.sql
# 7. scripts/07_triggers.sql
# 8. scripts/08_stored_procedures.sql

# Or use the master script:
# Execute scripts/master_script.sql in SQL Server Management Studio
```

### Step 2: Restore NuGet Packages

```bash
# Navigate to Phase_3 directory
cd Phase_3

# Restore all dependencies
dotnet restore
```

### Step 3: Build the Solution

```bash
# Build all projects
dotnet build

# Or build just the UI project
dotnet build ECommerce.UI/ECommerce.UI.csproj
```

### Step 4: Run the Application

```bash
# Run the UI application
dotnet run --project ECommerce.UI/ECommerce.UI.csproj
```

Alternatively, you can run from Visual Studio:

1. Open `ECommercePlatform.slnx` in Visual Studio
2. Set `ECommerce.UI` as the startup project
3. Press F5 or click "Run"

## Using the Application

### First Time Setup

1. **Register a New User**:

   - Launch the application
   - Click on the "Register" tab
   - Fill in all required fields (email, password, name, address, city)
   - Click "Register"
   - Switch to "Login" tab

2. **Login**:
   - Enter your registered email and password
   - Click "Login"
   - You'll be redirected to the main product listing page

### Shopping Flow

1. **Browse Products**:

   - View all products on the main page
   - Use the category filter to filter by category
   - Click "Low Stock Products" to view items with low inventory

2. **View Product Details**:

   - Click "View Details" on any product
   - Adjust quantity
   - Click "Add to Cart"

3. **Manage Cart**:

   - Click "View Cart" in the header
   - Update quantities using +/- buttons
   - Remove items using the delete button
   - Click "Clear Cart" to empty the cart

4. **Checkout**:
   - From the cart, click "Proceed to Checkout"
   - Verify/update shipping information
   - Click "Place Order"
   - Your cart will be cleared and order created

### Admin Features

1. **Access Admin Dashboard**:

   - Click "Admin" button in the header (📊 Admin)

2. **View Low Stock Products**:

   - Navigate to "Low Stock Products" tab
   - Click "Refresh" to reload data

3. **Manage Orders**:

   - Navigate to "Orders" tab
   - Filter by status using the dropdown
   - View all order details

4. **Calculate Revenue**:

   - Navigate to "Revenue" tab
   - Select start and end dates
   - Click "Calculate Revenue"
   - View total revenue for the date range

5. **View Statistics**:
   - Navigate to "Statistics" tab
   - View total products, orders, and users
   - Click "Refresh Statistics" to reload

### BLL Type Switching

The application supports runtime switching between LINQ and Stored Procedure implementations:

1. **From Login Screen**:

   - Click "Toggle BLL Type" button
   - Current mode is displayed

2. **From Main Window**:
   - Click "Toggle BLL" button in the header (⚙️ Toggle BLL)
   - All subsequent operations will use the selected implementation
   - Notice the mode indicator in the header: "(LINQ Mode)" or "(Stored Procedure Mode)"

This demonstrates the Factory Pattern in action!

## Testing the Application

### Test Scenarios

1. **User Authentication**:

   - Try registering with invalid data (missing fields, short password)
   - Try logging in with wrong credentials
   - Successfully register and login

2. **Product Operations**:

   - Browse products
   - Filter by category
   - View product details
   - Add items to cart with different quantities

3. **Cart Operations**:

   - Add multiple products to cart
   - Update quantities
   - Remove items
   - Clear entire cart

4. **Checkout**:

   - Proceed to checkout with items in cart
   - Try checking out with empty cart
   - Complete a successful order

5. **Admin Dashboard**:

   - View low stock products
   - Filter orders by status
   - Calculate revenue for different date ranges
   - View system statistics

6. **BLL Switching**:
   - Toggle between LINQ and SP modes
   - Verify all operations work in both modes
   - Compare performance (if desired)

## Troubleshooting

### Database Connection Issues

If you get database connection errors:

1. **Check Connection String**:

   - Open `Phase_3/ECommerce.DAL/ECommerceContext.cs`
   - Verify the connection string matches your SQL Server configuration
   - Common issues: Server name, database name, authentication method

2. **Verify SQL Server is Running**:

   ```bash
   # Check if SQL Server service is running (Windows)
   # Services -> SQL Server (MSSQLSERVER) -> Status should be "Running"
   ```

3. **Test Database Connection**:
   - Open SQL Server Management Studio (SSMS)
   - Connect using the same credentials as the connection string
   - Verify the `ECommerceDB` database exists

### Build Errors

If you encounter build errors:

1. **Clean and Rebuild**:

   ```bash
   dotnet clean
   dotnet restore
   dotnet build
   ```

2. **Check .NET Version**:

   ```bash
   dotnet --version
   # Should be 9.0 or later
   ```

3. **Verify Project References**:
   - All projects should reference each other correctly
   - Check `.csproj` files for proper `<ProjectReference>` tags

### Runtime Errors

If the application crashes or shows errors:

1. **Check Database Data**:

   - Ensure tables have data (run data generation scripts if needed)
   - Verify all required tables exist

2. **Check Logs**:

   - Look at the console output for error messages
   - Note the specific operation that failed

3. **Try Different BLL Mode**:
   - If LINQ mode fails, try Stored Procedure mode (or vice versa)
   - This helps isolate the issue

## Development Notes

### Adding New Features

1. **New Window/Page**:

   - Create `.axaml` file in `Views/` folder
   - Create corresponding `.axaml.cs` code-behind file
   - Reference from other windows using `new YourWindow().Show()` or `.ShowDialog()`

2. **Using BLL Services**:

   ```csharp
   using ECommerce.Factory;
   using ECommerce.UI.Helpers;

   // Get service with current BLL type
   var productService = BLLFactory.GetProductService(BLLManager.CurrentBLLType);
   var products = productService.GetAllProducts();
   ```

3. **Accessing Current User**:

   ```csharp
   using ECommerce.UI.Helpers;

   if (SessionManager.IsLoggedIn)
   {
       var userId = SessionManager.CurrentUser.UserID;
       var userName = SessionManager.CurrentUser.FirstName;
   }
   ```

### Code Style

- Follow C# naming conventions
- Use async/await for long-running operations
- Handle exceptions gracefully with try-catch blocks
- Provide user feedback for all operations

## Architecture Overview

```
┌─────────────────────────────────────────────────────────────┐
│                        ECommerce.UI                          │
│  (Avalonia UI - Views, ViewModels, Helpers)                 │
└────────────────────┬────────────────────────────────────────┘
                     │
                     ▼
┌─────────────────────────────────────────────────────────────┐
│                    ECommerce.Factory                         │
│  (Factory Pattern - Creates BLL instances based on type)    │
└────────────────────┬────────────────────────────────────────┘
                     │
                     ▼
┌─────────────────────────────────────────────────────────────┐
│                     ECommerce.BLL                            │
│  (Business Logic Layer)                                      │
│  ├─ LINQ Implementation (LinqImplementation/)               │
│  └─ Stored Procedure Implementation (SPImplementation/)     │
└────────────────────┬────────────────────────────────────────┘
                     │
                     ▼
┌─────────────────────────────────────────────────────────────┐
│                     ECommerce.DAL                            │
│  (Data Access Layer - Entity Framework Core)                │
└────────────────────┬────────────────────────────────────────┘
                     │
                     ▼
┌─────────────────────────────────────────────────────────────┐
│                     ECommerce.Models                         │
│  (Data Models/Entities)                                      │
└─────────────────────────────────────────────────────────────┘
                     │
                     ▼
┌─────────────────────────────────────────────────────────────┐
│                   SQL Server Database                        │
│  (ECommerceDB)                                               │
└─────────────────────────────────────────────────────────────┘
```

## Key Technologies

- **Avalonia UI 11.3.9**: Cross-platform XAML-based UI framework
- **.NET 9.0**: Application framework
- **Entity Framework Core**: ORM for database access
- **SQL Server**: Database backend
- **Factory Pattern**: Design pattern for BLL instantiation
- **MVVM Pattern**: Separation of concerns (Models, Views, ViewModels)

## Support

For issues or questions:

1. Check the troubleshooting section above
2. Review the database scripts to ensure proper setup
3. Verify all prerequisites are met
4. Check console output for detailed error messages

## License

See LICENSE file in the project root.

## Contributors

- Person 1: Frontend Development (UI)

---

**Happy Shopping! 🛒**
