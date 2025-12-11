using Avalonia.Controls;
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
    public partial class OrdersWindow : Window
    {
        private List<Order> _orders = new();

        public OrdersWindow()
        {
            InitializeComponent();
            LoadOrders();
        }

        private void LoadOrders()
        {
            try
            {
                if (SessionManager.CurrentUser == null)
                {
                    return;
                }

                var orderService = BLLFactory.GetOrderService(BLLManager.CurrentBLLType);
                _orders = orderService.GetOrdersByUserId(SessionManager.CurrentUser.UserID);

                DisplayOrders();
            }
            catch (Exception ex)
            {
                NoOrdersTextBlock.Text = $"Error loading orders: {ex.Message}";
                NoOrdersTextBlock.Foreground = Brushes.Red;
            }
        }

        private void DisplayOrders()
        {
            OrdersPanel.Children.Clear();

            if (_orders == null || _orders.Count == 0)
            {
                NoOrdersTextBlock.IsVisible = true;
                OrdersPanel.Children.Add(NoOrdersTextBlock);
                return;
            }

            NoOrdersTextBlock.IsVisible = false;

            foreach (var order in _orders.OrderByDescending(o => o.OrderDate))
            {
                var orderCard = CreateOrderCard(order);
                OrdersPanel.Children.Add(orderCard);
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

            // Order Header
            var headerGrid = new Grid();
            headerGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Star });
            headerGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
            headerGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });

            var orderIdText = new TextBlock
            {
                Text = $"Order #{order.OrderID}",
                FontSize = 18,
                FontWeight = FontWeight.Bold,
                VerticalAlignment = VerticalAlignment.Center
            };
            orderIdText.Bind(TextBlock.ForegroundProperty, 
                new Avalonia.Data.Binding { Source = this.FindResource("SystemControlForegroundBaseHighBrush") });
            Grid.SetColumn(orderIdText, 0);

            var statusBadge = new Border
            {
                CornerRadius = new Avalonia.CornerRadius(4),
                Padding = new Avalonia.Thickness(10, 5),
                Margin = new Avalonia.Thickness(10, 0),
                Background = GetStatusColor(order.Status)
            };
            var statusText = new TextBlock
            {
                Text = order.Status,
                Foreground = Brushes.White,
                FontWeight = FontWeight.Bold
            };
            statusBadge.Child = statusText;
            Grid.SetColumn(statusBadge, 1);

            var totalText = new TextBlock
            {
                Text = $"${order.TotalAmount:F2}",
                FontSize = 20,
                FontWeight = FontWeight.Bold,
                Foreground = Brushes.Green,
                VerticalAlignment = VerticalAlignment.Center
            };
            Grid.SetColumn(totalText, 2);

            headerGrid.Children.Add(orderIdText);
            headerGrid.Children.Add(statusBadge);
            headerGrid.Children.Add(totalText);

            mainStack.Children.Add(headerGrid);

            // Order Date
            var dateText = new TextBlock
            {
                Text = $"Ordered on: {order.OrderDate:MMMM dd, yyyy 'at' h:mm tt}",
                FontSize = 14,
                Margin = new Avalonia.Thickness(0, 5, 0, 10)
            };
            dateText.Bind(TextBlock.ForegroundProperty, 
                new Avalonia.Data.Binding { Source = this.FindResource("SystemControlForegroundBaseMediumBrush") });
            mainStack.Children.Add(dateText);

            // Shipping Address
            if (!string.IsNullOrEmpty(order.ShippingAddress))
            {
                var addressText = new TextBlock
                {
                    Text = $"📍 Shipping to: {order.ShippingAddress}",
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
                    Text = "Items:",
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
                        Text = $"  • {item.Product?.ProductName ?? "Product"} - Qty: {item.Quantity} - ${item.PriceAtPurchase:F2} each",
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

        private IBrush GetStatusColor(string status)
        {
            return status?.ToLower() switch
            {
                "pending" => new SolidColorBrush(Color.Parse("#FF9800")),
                "processing" => new SolidColorBrush(Color.Parse("#2196F3")),
                "shipped" => new SolidColorBrush(Color.Parse("#9C27B0")),
                "delivered" => new SolidColorBrush(Color.Parse("#4CAF50")),
                "cancelled" => new SolidColorBrush(Color.Parse("#F44336")),
                _ => new SolidColorBrush(Color.Parse("#757575"))
            };
        }
    }
}
