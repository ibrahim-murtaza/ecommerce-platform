# E-Commerce Platform UI - File Structure

## Complete File Structure

```
ECommerce.UI/
│
├── 📄 App.axaml                           # Application XAML definition
├── 📄 App.axaml.cs                        # Application startup logic (launches LoginWindow)
├── 📄 MainWindow.axaml                    # Product listing page (main shopping page)
├── 📄 MainWindow.axaml.cs                 # Product listing logic
├── 📄 Program.cs                          # Application entry point
├── 📄 app.manifest                        # Application manifest
├── 📄 ECommerce.UI.csproj                 # Project configuration
│
├── 📄 README.md                           # Comprehensive documentation
├── 📄 IMPLEMENTATION_SUMMARY.md           # Implementation checklist & summary
│
├── 📁 Views/                              # All UI Windows/Pages
│   ├── 🪟 LoginWindow.axaml               # Login & registration UI
│   ├── 📄 LoginWindow.axaml.cs            # Login & registration logic
│   │
│   ├── 🪟 ProductDetailWindow.axaml       # Product detail view UI
│   ├── 📄 ProductDetailWindow.axaml.cs    # Product detail logic
│   │
│   ├── 🪟 CartWindow.axaml                # Shopping cart UI
│   ├── 📄 CartWindow.axaml.cs             # Shopping cart logic
│   │
│   ├── 🪟 CheckoutWindow.axaml            # Order checkout UI
│   ├── 📄 CheckoutWindow.axaml.cs         # Checkout logic
│   │
│   ├── 🪟 AdminDashboardWindow.axaml      # Admin dashboard UI
│   └── 📄 AdminDashboardWindow.axaml.cs   # Admin dashboard logic
│
├── 📁 Helpers/                            # Helper & Manager Classes
│   ├── 📄 SessionManager.cs               # User session management
│   └── 📄 BLLManager.cs                   # BLL type switching manager
│
├── 📁 ViewModels/                         # ViewModel Base Classes
│   └── 📄 ViewModelBase.cs                # Base ViewModel with INotifyPropertyChanged
│
├── 📁 bin/                                # Build output (compiled files)
└── 📁 obj/                                # Intermediate build files
```

## File Descriptions

### 🎯 Core Application Files

#### `App.axaml` & `App.axaml.cs`

- Application entry point
- Configures Fluent theme
- Launches `LoginWindow` on startup

#### `Program.cs`

- .NET application bootstrap
- Initializes Avalonia framework

#### `MainWindow.axaml` & `MainWindow.axaml.cs`

- **Main shopping page** (shown after login)
- Displays all products in grid layout
- Category filtering dropdown
- Low stock products button
- Cart button with item count
- Admin dashboard button
- BLL toggle button
- Logout functionality

### 🪟 View Windows (Views/)

#### `LoginWindow`

**Purpose**: User authentication & registration
**Features**:

- Login tab with email/password
- Register tab with full user details
- BLL type toggle
- Form validation
- Session creation on successful login

#### `ProductDetailWindow`

**Purpose**: Detailed product view
**Features**:

- Product information display
- Quantity selector (+ / -)
- Add to cart with quantity
- Stock validation
- Close button

#### `CartWindow`

**Purpose**: Shopping cart management
**Features**:

- List all cart items with details
- Update quantities (+ / - buttons)
- Remove individual items
- Clear entire cart
- Display subtotals and total
- Proceed to checkout button
- Continue shopping button

#### `CheckoutWindow`

**Purpose**: Order placement
**Features**:

- Shipping address form
- City and postal code fields
- Order total display
- Form validation
- Place order button
- Integration with OrderService

#### `AdminDashboardWindow`

**Purpose**: Administrative functions
**Features**:

- **Low Stock Tab**: DataGrid of products below threshold
- **Orders Tab**: All orders with status filtering
- **Revenue Tab**: Calculate revenue for date ranges
- **Statistics Tab**: Total products, orders, users
- Refresh buttons for each section

### 🛠️ Helper Classes (Helpers/)

#### `SessionManager.cs`

**Purpose**: Manage logged-in user session
**Properties**:

- `CurrentUser`: Currently logged-in User object (nullable)
- `IsLoggedIn`: Boolean property for login status

**Usage**:

```csharp
// Check login
if (SessionManager.IsLoggedIn)
{
    var userId = SessionManager.CurrentUser.UserID;
}

// Set user on login
SessionManager.CurrentUser = user;

// Clear on logout
SessionManager.CurrentUser = null;
```

#### `BLLManager.cs`

**Purpose**: Manage BLL implementation type switching
**Properties**:

- `CurrentBLLType`: Current BLL type (LINQ or StoredProcedure)

**Methods**:

- `ToggleBLLType()`: Switch between implementations
- `GetCurrentBLLTypeName()`: Get display name of current type

**Usage**:

```csharp
// Get service with current BLL type
var service = BLLFactory.GetProductService(BLLManager.CurrentBLLType);

// Toggle BLL type
BLLManager.ToggleBLLType();

// Display current mode
string mode = BLLManager.GetCurrentBLLTypeName(); // "LINQ" or "Stored Procedure"
```

### 📊 ViewModel Base (ViewModels/)

#### `ViewModelBase.cs`

**Purpose**: Base class for ViewModels
**Features**:

- Implements `INotifyPropertyChanged`
- `OnPropertyChanged()` method
- `SetProperty()` helper method

**Usage**:

```csharp
public class MyViewModel : ViewModelBase
{
    private string _name;
    public string Name
    {
        get => _name;
        set => SetProperty(ref _name, value);
    }
}
```

## 📋 Documentation Files

### `README.md`

**Comprehensive documentation including**:

- Project overview
- Complete feature list
- Setup instructions
- Usage guide
- Troubleshooting
- Architecture diagrams
- Development notes

### `IMPLEMENTATION_SUMMARY.md`

**Implementation checklist with**:

- All deliverables
- Requirements checklist
- Architecture overview
- Testing guide
- Success criteria

## 🔗 Dependencies

### Project References

- `ECommerce.Factory` - Factory pattern for BLL instantiation
- `ECommerce.BLL` - Business logic layer
- `ECommerce.Models` - Data models

### NuGet Packages

- `Avalonia` (11.3.9) - UI framework
- `Avalonia.Desktop` (11.3.9) - Desktop support
- `Avalonia.Themes.Fluent` (11.3.9) - Fluent design theme
- `Avalonia.Fonts.Inter` (11.3.9) - Inter font family
- `Avalonia.Diagnostics` (11.3.9) - Debug tools

## 🎨 UI Design Pattern

### Window Hierarchy

```
LoginWindow (Startup)
    └─> MainWindow (After Login)
            ├─> ProductDetailWindow (Modal Dialog)
            ├─> CartWindow (Modal Dialog)
            │       └─> CheckoutWindow (Modal Dialog)
            └─> AdminDashboardWindow (Modal Dialog)
```

### Navigation Flow

1. User starts at `LoginWindow`
2. After login → `MainWindow` opens, LoginWindow closes
3. From MainWindow:
   - Click product → `ProductDetailWindow` opens as dialog
   - Click cart → `CartWindow` opens as dialog
   - Click admin → `AdminDashboardWindow` opens as dialog
   - Click logout → Returns to `LoginWindow`

## 🏗️ Code Organization

### XAML Files (.axaml)

- Define UI layout
- Styling and appearance
- Data binding setup
- Event handler names

### Code-Behind Files (.axaml.cs)

- Event handlers
- Business logic integration
- Service calls via Factory
- Navigation logic
- Validation

### Helper Classes

- Stateless utility functions
- Global state management
- Reusable components

## 🎯 Key Design Patterns Used

1. **Factory Pattern** - BLL service instantiation
2. **Singleton Pattern** - SessionManager, BLLManager (static classes)
3. **MVVM Pattern** - ViewModelBase for data binding
4. **Observer Pattern** - INotifyPropertyChanged implementation
5. **Strategy Pattern** - Interchangeable BLL implementations (LINQ/SP)

## 📱 UI Components Used

- `Window` - Application windows
- `Button` - Interactive buttons
- `TextBlock` - Text display
- `TextBox` - Text input
- `ComboBox` - Dropdown selections
- `DataGrid` - Tabular data display
- `ItemsControl` - Product grid
- `StackPanel` - Vertical/horizontal layouts
- `Grid` - Complex layouts
- `Border` - Visual containers
- `ScrollViewer` - Scrollable content
- `TabControl` - Tabbed interface (Admin Dashboard)
- `DatePicker` - Date selection (Revenue)

---

**Total Files Created**: 23 files
**Total Lines of Code**: ~2,500+ lines
**Windows Implemented**: 6 major windows
**Helper Classes**: 3 classes
**Documentation Files**: 2 comprehensive guides
