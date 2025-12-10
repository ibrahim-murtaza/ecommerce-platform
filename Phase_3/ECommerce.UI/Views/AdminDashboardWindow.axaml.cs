using Avalonia.Controls;
using Avalonia.Interactivity;
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

            // Load initial data
            LoadLowStockProducts();
            LoadOrders();
            LoadStatistics();
        }

        private void LoadLowStockProducts()
        {
            try
            {
                var productService = BLLFactory.GetProductService(BLLManager.CurrentBLLType);
                var lowStockProducts = productService.GetLowStockProducts();
                LowStockDataGrid.ItemsSource = lowStockProducts;
            }
            catch (Exception ex)
            {
                ShowError($"Error loading low stock products: {ex.Message}");
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

                OrdersDataGrid.ItemsSource = orders;
            }
            catch (Exception ex)
            {
                ShowError($"Error loading orders: {ex.Message}");
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

        private void RefreshLowStockButton_Click(object? sender, RoutedEventArgs e)
        {
            LoadLowStockProducts();
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

        private async void ShowError(string message)
        {
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
