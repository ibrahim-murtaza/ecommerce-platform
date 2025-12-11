using Avalonia.Controls;
using Avalonia.Data.Converters;
using Avalonia.Interactivity;
using Avalonia.Media;
using ECommerce.Factory;
using ECommerce.Models;
using ECommerce.UI.Helpers;
using ECommerce.UI.Views;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace ECommerce.UI;

public partial class MainWindow : Window
{
    private List<Product> _allProducts = new();
    private List<Category> _allCategories = new();
    private string _currentSearchText = string.Empty;

    public MainWindow()
    {
        InitializeComponent();
        InitializeWindow();
    }

    private void InitializeWindow()
    {
        // Set welcome message
        if (SessionManager.CurrentUser != null)
        {
            WelcomeTextBlock.Text = $"Welcome, {SessionManager.CurrentUser.FirstName ?? "User"}!";
        }

        // Show/hide buttons based on user type
        if (SessionManager.IsAdmin)
        {
            // Admin sees: Admin Dashboard, Toggle BLL, Logout
            ViewCartButton.IsVisible = false;
            OrdersButton.IsVisible = false;
            AdminDashboardButton.IsVisible = true;
            
            // Show admin product view (no buttons)
            UserProductsScrollViewer.IsVisible = false;
            AdminProductsScrollViewer.IsVisible = true;
            
            // Disable cart functionality for admins
            WelcomeTextBlock.Text = $"Welcome, Admin {SessionManager.CurrentUser?.FirstName ?? "User"}!";
        }
        else
        {
            // Regular user sees: Cart, Orders, Toggle BLL, Logout
            ViewCartButton.IsVisible = true;
            OrdersButton.IsVisible = true;
            AdminDashboardButton.IsVisible = false;
            
            // Show user product view (with buttons)
            UserProductsScrollViewer.IsVisible = true;
            AdminProductsScrollViewer.IsVisible = false;
        }

        UpdateBLLModeDisplay();
        LoadCategories();
        LoadProducts();
        UpdateCartCount();
    }

    private void UpdateBLLModeDisplay()
    {
        BLLModeTextBlock.Text = $"({BLLManager.GetCurrentBLLTypeName()} Mode)";
    }

    private void LoadCategories()
    {
        try
        {
            var categoryService = BLLFactory.GetCategoryService(BLLManager.CurrentBLLType);
            _allCategories = categoryService.GetActiveCategories();

            var categoryList = new List<string> { "All Categories" };
            categoryList.AddRange(_allCategories.Select(c => c.CategoryName));

            CategoryComboBox.ItemsSource = categoryList;
            CategoryComboBox.SelectedIndex = 0;
        }
        catch (Exception ex)
        {
            ShowError($"Error loading categories: {ex.Message}");
        }
    }

    private void LoadProducts(int? categoryId = null)
    {
        try
        {
            var productService = BLLFactory.GetProductService(BLLManager.CurrentBLLType);

            if (categoryId.HasValue)
            {
                _allProducts = productService.GetProductsByCategory(categoryId.Value);
            }
            else
            {
                _allProducts = productService.GetAllProducts()
                    .OrderByDescending(p => p.ProductID) // Show newest products first
                    .Take(200) // Limit to 200 most recent products for performance
                    .ToList();
            }

            // Filter only active products
            _allProducts = _allProducts.Where(p => p.IsActive).ToList();

            // Apply search filter if search text exists
            ApplySearchFilter();
            
            // Update Add to Cart button visibility after products load
            UpdateAddToCartButtonsVisibility();
        }
        catch (Exception ex)
        {
            ShowError($"Error loading products: {ex.Message}");
        }
    }
    
    private void UpdateAddToCartButtonsVisibility()
    {
        // No longer needed - template now only shows View Details for all users
        // Add to Cart removed from product cards
    }

    private void ApplySearchFilter()
    {
        var filteredProducts = _allProducts;

        if (!string.IsNullOrWhiteSpace(_currentSearchText))
        {
            filteredProducts = _allProducts
                .Where(p => p.ProductName.Contains(_currentSearchText, StringComparison.OrdinalIgnoreCase))
                .ToList();
        }

        // Update both ItemsControls (only the visible one will be shown)
        ProductsItemsControl.ItemsSource = filteredProducts;
        AdminProductsItemsControl.ItemsSource = filteredProducts;
    }

    private void SearchTextBox_TextChanged(object? sender, Avalonia.Controls.TextChangedEventArgs e)
    {
        if (sender is TextBox textBox)
        {
            _currentSearchText = textBox.Text ?? string.Empty;
            ApplySearchFilter();
        }
    }

    private void UpdateCartCount()
    {
        try
        {
            if (SessionManager.CurrentUser == null) return;

            var cartService = BLLFactory.GetCartService(BLLManager.CurrentBLLType);
            var cartItems = cartService.GetCartByUserId(SessionManager.CurrentUser.UserID);
            ViewCartButton.Content = $"🛒 View Cart ({cartItems.Count})";
        }
        catch (Exception)
        {
            ViewCartButton.Content = "🛒 View Cart (0)";
        }
    }

    private void CategoryComboBox_SelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (CategoryComboBox.SelectedIndex <= 0)
        {
            LoadProducts();
            return;
        }

        var selectedCategoryName = CategoryComboBox.SelectedItem as string;
        var category = _allCategories.FirstOrDefault(c => c.CategoryName == selectedCategoryName);

        if (category != null)
        {
            LoadProducts(category.CategoryID);
        }
    }

    private void ShowAllButton_Click(object? sender, RoutedEventArgs e)
    {
        CategoryComboBox.SelectedIndex = 0;
        LoadProducts();
    }

    private void RefreshButton_Click(object? sender, RoutedEventArgs e)
    {
        LoadProducts();
        UpdateCartCount();
    }

    private void ToggleBLLButton_Click(object? sender, RoutedEventArgs e)
    {
        BLLManager.ToggleBLLType();
        UpdateBLLModeDisplay();
        LoadCategories();
        LoadProducts();
        UpdateCartCount();
    }

    private void ViewDetailsButton_Click(object? sender, RoutedEventArgs e)
    {
        if (sender is Button button && button.Tag is int productId)
        {
            var product = _allProducts.FirstOrDefault(p => p.ProductID == productId);
            if (product != null)
            {
                var detailWindow = new ProductDetailWindow(product);
                detailWindow.ShowDialog(this);
                UpdateCartCount();
            }
        }
    }

    private void AddToCartButton_Click(object? sender, RoutedEventArgs e)
    {
        // Admins cannot add to cart
        if (SessionManager.IsAdmin)
        {
            ShowError("Admins cannot add items to cart. This is a view-only interface.");
            return;
        }
        
        try
        {
            if (SessionManager.CurrentUser == null)
            {
                ShowError("Please login to add items to cart");
                return;
            }

            if (sender is Button button && button.Tag is int productId)
            {
                var cartService = BLLFactory.GetCartService(BLLManager.CurrentBLLType);
                cartService.AddToCart(SessionManager.CurrentUser.UserID, productId, 1);
                UpdateCartCount();
                ShowSuccess("Product added to cart!");
            }
        }
        catch (Exception ex)
        {
            ShowError($"Error adding to cart: {ex.Message}");
        }
    }

    private async void ViewCartButton_Click(object? sender, RoutedEventArgs e)
    {
        if (SessionManager.CurrentUser == null)
        {
            ShowError("Please login to view cart");
            return;
        }

        var cartWindow = new CartWindow();
        await cartWindow.ShowDialog(this);
        UpdateCartCount();
    }

    private async void OrdersButton_Click(object? sender, RoutedEventArgs e)
    {
        if (SessionManager.CurrentUser == null)
        {
            ShowError("Please login to view orders");
            return;
        }

        var ordersWindow = new OrdersWindow();
        await ordersWindow.ShowDialog(this);
    }

    private async void AdminDashboardButton_Click(object? sender, RoutedEventArgs e)
    {
        var adminWindow = new AdminDashboardWindow();
        await adminWindow.ShowDialog(this);
        
        // Reload products after returning from admin dashboard
        LoadProducts();
    }

    private void LogoutButton_Click(object? sender, RoutedEventArgs e)
    {
        SessionManager.CurrentUser = null;
        SessionManager.IsAdmin = false;
        var loginWindow = new LoginWindow();
        loginWindow.Show();
        this.Close();
    }

    private async void ShowError(string message)
    {
        var messageBox = new Window
        {
            Title = "Error",
            Width = 400,
            Height = 150,
            WindowStartupLocation = WindowStartupLocation.CenterOwner,
            Content = new StackPanel
            {
                Margin = new Avalonia.Thickness(20),
                Children =
                {
                    new TextBlock { Text = message, TextWrapping = Avalonia.Media.TextWrapping.Wrap },
                    new Button
                    {
                        Content = "OK",
                        HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center,
                        Margin = new Avalonia.Thickness(0, 20, 0, 0),
                        Padding = new Avalonia.Thickness(30, 10)
                    }
                }
            }
        };

        ((Button)((StackPanel)messageBox.Content).Children[1]).Click += (s, e) => messageBox.Close();
        await messageBox.ShowDialog(this);
    }

    private async void ShowSuccess(string message)
    {
        var messageBox = new Window
        {
            Title = "Success",
            Width = 400,
            Height = 150,
            WindowStartupLocation = WindowStartupLocation.CenterOwner,
            Content = new StackPanel
            {
                Margin = new Avalonia.Thickness(20),
                Children =
                {
                    new TextBlock
                    {
                        Text = message,
                        TextWrapping = Avalonia.Media.TextWrapping.Wrap,
                        Foreground = Avalonia.Media.Brushes.Green
                    },
                    new Button
                    {
                        Content = "OK",
                        HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center,
                        Margin = new Avalonia.Thickness(0, 20, 0, 0),
                        Padding = new Avalonia.Thickness(30, 10)
                    }
                }
            }
        };

        ((Button)((StackPanel)messageBox.Content).Children[1]).Click += (s, e) => messageBox.Close();
        await messageBox.ShowDialog(this);
    }
}

// Converter to show/hide OUT OF STOCK badge
public class StockToVisibilityConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is int stock)
        {
            return stock == 0;
        }
        return false;
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

// Converter to gray out out-of-stock products
public class StockToOpacityConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is int stock)
        {
            return stock == 0 ? 0.6 : 1.0;
        }
        return 1.0;
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

// Converter to change stock text color (red when 0)
public class StockToColorConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is int stock)
        {
            return stock == 0 ? new SolidColorBrush(Color.Parse("#D32F2F")) : new SolidColorBrush(Color.Parse("#666666"));
        }
        return new SolidColorBrush(Color.Parse("#666666"));
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

// Converter to disable Add to Cart button when stock is 0
public class StockToEnabledConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is int stock)
        {
            return stock > 0;
        }
        return false;
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}