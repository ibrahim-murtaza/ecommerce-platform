using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Layout;
using Avalonia.Media;
using ECommerce.Factory;
using ECommerce.Models;
using ECommerce.UI.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ECommerce.UI.Views
{
    public partial class AdminDashboardWindow : Window
    {
        public AdminDashboardWindow()
        {
            InitializeComponent();
            InitializeDashboard();
        }

        private void InitializeDashboard()
        {
            // Set default dates
            StartDatePicker.SelectedDate = DateTime.Now.AddMonths(-1);
            EndDatePicker.SelectedDate = DateTime.Now;

            // Initialize order status filter
            var statuses = new List<string> { "All Statuses", "Pending", "Processing", "Shipped", "Delivered", "Cancelled" };
            OrderStatusComboBox.ItemsSource = statuses;
            OrderStatusComboBox.SelectedIndex = 0;

            // Load categories for product form
            LoadCategories();

            // Load initial data
            LoadOrders();
            LoadStatistics();
        }

        private void LoadCategories()
        {
            try
            {
                var categoryService = BLLFactory.GetCategoryService(BLLManager.CurrentBLLType);
                var categories = categoryService.GetActiveCategories();
                
                CategoryComboBox.ItemsSource = categories;
                CategoryComboBox.DisplayMemberBinding = new Avalonia.Data.Binding("CategoryName");
                if (categories.Count > 0)
                {
                    CategoryComboBox.SelectedIndex = 0;
                }
            }
            catch (Exception ex)
            {
                ShowError($"Error loading categories: {ex.Message}");
            }
        }

        private void LoadOrders(string? status = null)
        {
            try
            {
                var orderService = BLLFactory.GetOrderService(BLLManager.CurrentBLLType);
                List<Order> orders;

                if (!string.IsNullOrEmpty(status) && status != "All Statuses")
                {
                    orders = orderService.GetOrdersByStatus(status);
                }
                else
                {
                    orders = orderService.GetAllOrders();
                }

                DisplayOrders(orders);
            }
            catch (Exception ex)
            {
                ShowError($"Error loading orders: {ex.Message}");
            }
        }
        
        private void DisplayOrders(List<Order> orders)
        {
            OrdersStackPanel.Children.Clear();

            if (orders == null || orders.Count == 0)
            {
                var noOrdersText = new TextBlock
                {
                    Text = "No orders found.",
                    FontSize = 16,
                    Margin = new Avalonia.Thickness(20),
                    HorizontalAlignment = HorizontalAlignment.Center
                };
                noOrdersText.Bind(TextBlock.ForegroundProperty,
                    new Avalonia.Data.Binding { Source = this.FindResource("SystemControlForegroundBaseMediumBrush") });
                OrdersStackPanel.Children.Add(noOrdersText);
                return;
            }

            foreach (var order in orders.OrderByDescending(o => o.OrderDate))
            {
                var orderCard = CreateOrderCard(order);
                OrdersStackPanel.Children.Add(orderCard);
            }
        }

        private Border CreateOrderCard(Order order)
        {
            var border = new Border
            {
                BorderThickness = new Avalonia.Thickness(1),
                CornerRadius = new Avalonia.CornerRadius(8),
                Padding = new Avalonia.Thickness(20),
                Margin = new Avalonia.Thickness(0, 0, 0, 15)
            };

            border.Bind(Border.BorderBrushProperty,
                new Avalonia.Data.Binding { Source = this.FindResource("SystemControlForegroundBaseMediumLowBrush") });
            border.Bind(Border.BackgroundProperty,
                new Avalonia.Data.Binding { Source = this.FindResource("SystemControlBackgroundAltHighBrush") });

            var mainStack = new StackPanel();

            // Order Header with Status Editor
            var headerGrid = new Grid();
            headerGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Star });
            headerGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
            headerGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });

            var orderInfoStack = new StackPanel { Orientation = Orientation.Vertical };
            
            var orderIdText = new TextBlock
            {
                Text = $"Order #{order.OrderID}",
                FontSize = 18,
                FontWeight = FontWeight.Bold
            };
            orderIdText.Bind(TextBlock.ForegroundProperty,
                new Avalonia.Data.Binding { Source = this.FindResource("SystemControlForegroundBaseHighBrush") });
            orderInfoStack.Children.Add(orderIdText);

            var userText = new TextBlock
            {
                Text = $"User ID: {order.UserID}",
                FontSize = 12,
                Margin = new Avalonia.Thickness(0, 2, 0, 0)
            };
            userText.Bind(TextBlock.ForegroundProperty,
                new Avalonia.Data.Binding { Source = this.FindResource("SystemControlForegroundBaseMediumBrush") });
            orderInfoStack.Children.Add(userText);

            Grid.SetColumn(orderInfoStack, 0);

            // Status Editor
            var statusStack = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                VerticalAlignment = VerticalAlignment.Center,
                Margin = new Avalonia.Thickness(10, 0)
            };

            var statusLabel = new TextBlock
            {
                Text = "Status:",
                VerticalAlignment = VerticalAlignment.Center,
                Margin = new Avalonia.Thickness(0, 0, 5, 0)
            };
            statusLabel.Bind(TextBlock.ForegroundProperty,
                new Avalonia.Data.Binding { Source = this.FindResource("SystemControlForegroundBaseHighBrush") });
            statusStack.Children.Add(statusLabel);

            var statusComboBox = new ComboBox
            {
                Width = 120,
                VerticalAlignment = VerticalAlignment.Center,
                ItemsSource = new[] { "Pending", "Processing", "Shipped", "Delivered", "Cancelled" },
                SelectedItem = order.Status,
                Tag = order.OrderID
            };
            statusComboBox.SelectionChanged += StatusComboBox_SelectionChanged;
            statusStack.Children.Add(statusComboBox);

            Grid.SetColumn(statusStack, 1);

            var totalText = new TextBlock
            {
                Text = $"${order.TotalAmount:F2}",
                FontSize = 20,
                FontWeight = FontWeight.Bold,
                Foreground = Brushes.Green,
                VerticalAlignment = VerticalAlignment.Center,
                Margin = new Avalonia.Thickness(10, 0, 0, 0)
            };
            Grid.SetColumn(totalText, 2);

            headerGrid.Children.Add(orderInfoStack);
            headerGrid.Children.Add(statusStack);
            headerGrid.Children.Add(totalText);

            mainStack.Children.Add(headerGrid);

            // Order Date
            var dateText = new TextBlock
            {
                Text = $"Ordered on: {order.OrderDate:MMMM dd, yyyy 'at' h:mm tt}",
                FontSize = 14,
                Margin = new Avalonia.Thickness(0, 10, 0, 5)
            };
            dateText.Bind(TextBlock.ForegroundProperty,
                new Avalonia.Data.Binding { Source = this.FindResource("SystemControlForegroundBaseMediumBrush") });
            mainStack.Children.Add(dateText);

            // Shipping Address
            if (!string.IsNullOrEmpty(order.ShippingAddress))
            {
                var addressText = new TextBlock
                {
                    Text = $"📍 {order.ShippingAddress}",
                    FontSize = 14,
                    Margin = new Avalonia.Thickness(0, 0, 0, 10)
                };
                addressText.Bind(TextBlock.ForegroundProperty,
                    new Avalonia.Data.Binding { Source = this.FindResource("SystemControlForegroundBaseHighBrush") });
                mainStack.Children.Add(addressText);
            }

            // Order Items
            if (order.OrderItems != null && order.OrderItems.Any())
            {
                var itemsHeader = new TextBlock
                {
                    Text = $"Items ({order.OrderItems.Count}):",
                    FontWeight = FontWeight.Bold,
                    FontSize = 14,
                    Margin = new Avalonia.Thickness(0, 10, 0, 5)
                };
                itemsHeader.Bind(TextBlock.ForegroundProperty,
                    new Avalonia.Data.Binding { Source = this.FindResource("SystemControlForegroundBaseHighBrush") });
                mainStack.Children.Add(itemsHeader);

                foreach (var item in order.OrderItems)
                {
                    var itemText = new TextBlock
                    {
                        Text = $"  • {item.Product?.ProductName ?? $"Product ID: {item.ProductID}"} - Qty: {item.Quantity} × ${item.PriceAtPurchase:F2} = ${item.Quantity * item.PriceAtPurchase:F2}",
                        FontSize = 13,
                        Margin = new Avalonia.Thickness(15, 2, 0, 2)
                    };
                    itemText.Bind(TextBlock.ForegroundProperty,
                        new Avalonia.Data.Binding { Source = this.FindResource("SystemControlForegroundBaseHighBrush") });
                    mainStack.Children.Add(itemText);
                }
            }

            border.Child = mainStack;
            return border;
        }

        private void StatusComboBox_SelectionChanged(object? sender, SelectionChangedEventArgs e)
        {
            if (sender is ComboBox comboBox && comboBox.Tag is int orderId)
            {
                var newStatus = comboBox.SelectedItem?.ToString();
                if (string.IsNullOrEmpty(newStatus))
                    return;

                try
                {
                    var orderService = BLLFactory.GetOrderService(BLLManager.CurrentBLLType);
                    orderService.UpdateOrderStatus(orderId, newStatus);
                    
                    // Refresh the orders list
                    LoadOrders(OrderStatusComboBox.SelectedItem?.ToString());
                }
                catch (Exception ex)
                {
                    ShowError($"Error updating order status: {ex.Message}");
                    // Revert the combobox selection
                    LoadOrders(OrderStatusComboBox.SelectedItem?.ToString());
                }
            }
        }

        private void LoadStatistics()
        {
            try
            {
                var productService = BLLFactory.GetProductService(BLLManager.CurrentBLLType);
                var orderService = BLLFactory.GetOrderService(BLLManager.CurrentBLLType);
                var userService = BLLFactory.GetUserService(BLLManager.CurrentBLLType);

                var products = productService.GetAllProducts();
                var orders = orderService.GetAllOrders();
                var users = userService.GetAllUsers();

                TotalProductsTextBlock.Text = products.Count.ToString();
                TotalOrdersTextBlock.Text = orders.Count.ToString();
                TotalUsersTextBlock.Text = users.Count.ToString();
            }
            catch (Exception ex)
            {
                ShowError($"Error loading statistics: {ex.Message}");
            }
        }

        private void RefreshOrdersButton_Click(object? sender, RoutedEventArgs e)
        {
            LoadOrders();
        }

        private void ShowAllOrdersButton_Click(object? sender, RoutedEventArgs e)
        {
            OrderStatusComboBox.SelectedIndex = 0;
            LoadOrders();
        }

        private void OrderStatusComboBox_SelectionChanged(object? sender, SelectionChangedEventArgs e)
        {
            if (OrderStatusComboBox.SelectedIndex > 0)
            {
                var selectedStatus = OrderStatusComboBox.SelectedItem as string;
                LoadOrders(selectedStatus);
            }
            else
            {
                LoadOrders();
            }
        }

        private void CalculateRevenueButton_Click(object? sender, RoutedEventArgs e)
        {
            try
            {
                if (StartDatePicker.SelectedDate == null || EndDatePicker.SelectedDate == null)
                {
                    ShowError("Please select both start and end dates");
                    return;
                }

                var startDate = StartDatePicker.SelectedDate.Value.DateTime;
                var endDate = EndDatePicker.SelectedDate.Value.DateTime;

                if (startDate > endDate)
                {
                    ShowError("Start date must be before end date");
                    return;
                }

                var orderService = BLLFactory.GetOrderService(BLLManager.CurrentBLLType);
                var revenue = orderService.GetTotalRevenueByDateRange(startDate, endDate);

                RevenueTextBlock.Text = $"${revenue:F2}";
                RevenueDateRangeTextBlock.Text = $"From {startDate:yyyy-MM-dd} to {endDate:yyyy-MM-dd}";
            }
            catch (Exception ex)
            {
                ShowError($"Error calculating revenue: {ex.Message}");
            }
        }

        private void RefreshStatsButton_Click(object? sender, RoutedEventArgs e)
        {
            LoadStatistics();
        }

        private void AddProductButton_Click(object? sender, RoutedEventArgs e)
        {
            try
            {
                // Validate inputs
                if (string.IsNullOrWhiteSpace(ProductNameTextBox.Text))
                {
                    ShowError("Product name is required.");
                    return;
                }

                if (CategoryComboBox.SelectedItem == null)
                {
                    ShowError("Please select a category.");
                    return;
                }

                if (!decimal.TryParse(PriceTextBox.Text, out decimal price) || price <= 0)
                {
                    ShowError("Please enter a valid price greater than 0.");
                    return;
                }

                if (!int.TryParse(StockQuantityTextBox.Text, out int stock) || stock < 0)
                {
                    ShowError("Please enter a valid stock quantity (0 or greater).");
                    return;
                }

                // Create product
                var product = new Product
                {
                    ProductName = ProductNameTextBox.Text.Trim(),
                    CategoryID = ((Category)CategoryComboBox.SelectedItem).CategoryID,
                    Price = price,
                    StockQuantity = stock,
                    ImageURL = null,
                    IsActive = true
                };

                // Add product to database
                var productService = BLLFactory.GetProductService(BLLManager.CurrentBLLType);
                productService.AddProduct(product);

                ShowSuccess($"Product '{product.ProductName}' added successfully!");
                
                // Clear form
                ClearProductForm();
                
                // Refresh statistics
                LoadStatistics();
            }
            catch (Exception ex)
            {
                ShowError($"Error adding product: {ex.Message}");
            }
        }

        private void ClearFormButton_Click(object? sender, RoutedEventArgs e)
        {
            ClearProductForm();
        }

        private void ClearProductForm()
        {
            ProductNameTextBox.Text = string.Empty;
            PriceTextBox.Text = string.Empty;
            StockQuantityTextBox.Text = string.Empty;
            if (CategoryComboBox.ItemCount > 0)
            {
                CategoryComboBox.SelectedIndex = 0;
            }
        }

        private async void ShowSuccess(string message)
        {
            if (!this.IsVisible)
            {
                Console.WriteLine($"Success: {message}");
                return;
            }

            var successWindow = new Window
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
                            Margin = new Avalonia.Thickness(0, 0, 0, 20)
                        },
                        new Button
                        {
                            Content = "OK",
                            HorizontalAlignment = HorizontalAlignment.Center,
                            Padding = new Avalonia.Thickness(30, 10)
                        }
                    }
                }
            };

            if (successWindow.Content is StackPanel panel && panel.Children[1] is Button okButton)
            {
                okButton.Click += (s, e) => successWindow.Close();
            }

            await successWindow.ShowDialog(this);
        }

        private async void ShowError(string message)
        {
            // If window is not shown yet, just log to console
            if (!this.IsVisible)
            {
                Console.WriteLine($"Error: {message}");
                return;
            }

            var errorWindow = new Window
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
                        new TextBlock
                        {
                            Text = message,
                            TextWrapping = Avalonia.Media.TextWrapping.Wrap,
                            Foreground = Avalonia.Media.Brushes.Red
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

            ((Button)((StackPanel)errorWindow.Content).Children[1]).Click += (s, e) => errorWindow.Close();
            await errorWindow.ShowDialog(this);
        }
    }
}
