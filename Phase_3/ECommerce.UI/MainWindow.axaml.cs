using Avalonia.Controls;
using Avalonia.Interactivity;
using ECommerce.Factory;
using ECommerce.Models;
using ECommerce.UI.Helpers;
using ECommerce.UI.Views;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ECommerce.UI;

public partial class MainWindow : Window
{
    private List<Product> _allProducts = new();
    private List<Category> _allCategories = new();

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
            WelcomeTextBlock.Text = $"Welcome, {SessionManager.CurrentUser.FirstName}!";
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

    private void LoadProducts(int? categoryId = null, bool lowStockOnly = false)
    {
        try
        {
            var productService = BLLFactory.GetProductService(BLLManager.CurrentBLLType);

            if (lowStockOnly)
            {
                _allProducts = productService.GetLowStockProducts();
            }
            else if (categoryId.HasValue)
            {
                _allProducts = productService.GetProductsByCategory(categoryId.Value);
            }
            else
            {
                _allProducts = productService.GetAllProducts()
                    .Take(100)  // ← ADD THIS LINE - only load first 100 products
                    .ToList();
            }

            // Filter only active products
            _allProducts = _allProducts.Where(p => p.IsActive).ToList();

            ProductsItemsControl.ItemsSource = _allProducts;
        }
        catch (Exception ex)
        {
            ShowError($"Error loading products: {ex.Message}");
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

    private void ShowLowStockButton_Click(object? sender, RoutedEventArgs e)
    {
        LoadProducts(lowStockOnly: true);
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

    private async void AdminDashboardButton_Click(object? sender, RoutedEventArgs e)
    {
        var adminWindow = new AdminDashboardWindow();
        await adminWindow.ShowDialog(this);
    }

    private void LogoutButton_Click(object? sender, RoutedEventArgs e)
    {
        SessionManager.CurrentUser = null;
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