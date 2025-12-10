# E-Commerce Platform UI - Quick Start Guide

## 🚀 Quick Start (5 Minutes)

### Prerequisites Check

- ✅ .NET 9.0 SDK installed
- ✅ SQL Server running
- ✅ Database created and populated

### Run the Application

```bash
# 1. Navigate to the project
cd /Users/ahsankhan7503/Desktop/ecommerce-platform/Phase_3

# 2. Restore dependencies (first time only)
dotnet restore

# 3. Build the solution
dotnet build

# 4. Run the UI application
dotnet run --project ECommerce.UI/ECommerce.UI.csproj
```

### First Login

**Option 1: Register New User**

1. Click "Register" tab
2. Fill in all fields
3. Click "Register"
4. Switch to "Login" tab and login

**Option 2: Use Existing User** (if database has data)

- Check your database `User` table for existing emails
- Password should match the `PasswordHash` column

### Quick Tour

1. **Browse Products**: Main window shows all products
2. **Add to Cart**: Click "Add to Cart" or "View Details"
3. **View Cart**: Click "🛒 View Cart" in header
4. **Checkout**: From cart, click "Proceed to Checkout"
5. **Admin Panel**: Click "📊 Admin" to see dashboard
6. **Toggle BLL**: Click "⚙️ Toggle BLL" to switch between LINQ/SP modes

## 📁 Project Structure

```
Phase_3/
├── ECommerce.UI/          # ← Frontend (Avalonia UI)
│   ├── Views/             # UI pages/windows
│   ├── Helpers/           # SessionManager, BLLManager
│   ├── ViewModels/        # View models
│   └── README.md          # ← Detailed UI documentation
├── ECommerce.BLL/         # Business Logic Layer
├── ECommerce.DAL/         # Data Access Layer
├── ECommerce.Factory/     # Factory Pattern
└── ECommerce.Models/      # Data Models
```

## 🔧 Configuration

### Update Database Connection

If needed, update connection string in:
**File**: `ECommerce.DAL/ECommerceContext.cs`

```csharp
optionsBuilder.UseSqlServer(
    "Server=YOUR_SERVER;Database=ECommerceDB;Trusted_Connection=True;TrustServerCertificate=True;"
);
```

## 🎯 Key Features

- ✨ User Authentication (Login/Register)
- 🛍️ Product Browsing & Search
- 🛒 Shopping Cart Management
- 📦 Order Checkout
- 📊 Admin Dashboard
- 🔄 Runtime BLL Switching (LINQ ↔ Stored Procedures)
- 📉 Low Stock Alerts
- 💰 Revenue Analytics

## 📖 Detailed Documentation

For comprehensive documentation, see:

- **UI Documentation**: `Phase_3/ECommerce.UI/README.md`

## ⚠️ Common Issues

### Database Connection Error

- Check SQL Server is running
- Verify connection string in `ECommerceContext.cs`
- Ensure database `ECommerceDB` exists

### Build Errors

```bash
dotnet clean
dotnet restore
dotnet build
```

### No Products Showing

- Run data generation scripts in `scripts/data_generation/`
- Verify tables have data in SQL Server

## 🎨 UI Screenshots Reference

### Windows Implemented:

1. **LoginWindow** - User authentication
2. **MainWindow** - Product listing with filters
3. **ProductDetailWindow** - Product details & add to cart
4. **CartWindow** - Shopping cart management
5. **CheckoutWindow** - Order placement
6. **AdminDashboardWindow** - Admin analytics & management

## 🏗️ BLL Type Switching

The app demonstrates the **Factory Pattern**:

```csharp
// Toggle between implementations
BLLManager.ToggleBLLType();

// Get service with current type
var productService = BLLFactory.GetProductService(BLLManager.CurrentBLLType);
```

Both implementations (LINQ & Stored Procedures) work identically from the UI perspective!

## 🎓 For Developers

### Adding a New Feature

1. Create service method in BLL interfaces
2. Implement in both LINQ and SP implementations
3. Add UI controls in appropriate `.axaml` file
4. Wire up in code-behind `.axaml.cs`
5. Use Factory to get service: `BLLFactory.GetXXXService(BLLManager.CurrentBLLType)`

### Session Management

```csharp
// Check if user is logged in
if (SessionManager.IsLoggedIn)
{
    var user = SessionManager.CurrentUser;
}
```

---

**Need Help?** Check the detailed README in `Phase_3/ECommerce.UI/README.md`
