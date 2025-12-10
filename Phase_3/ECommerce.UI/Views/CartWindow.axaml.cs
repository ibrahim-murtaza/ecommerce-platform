using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Layout;
using ECommerce.Factory;
using ECommerce.Models;
using ECommerce.UI.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ECommerce.UI.Views
{
    public partial class CartWindow : Window
    {
        private List<Cart> _cartItems = new();

        public CartWindow()
        {
            InitializeComponent();
            LoadCart();
        }

        private void LoadCart()
        {
            try
            {
                if (SessionManager.CurrentUser == null)
                {
                    return;
                }

                var cartService = BLLFactory.GetCartService(BLLManager.CurrentBLLType);
                _cartItems = cartService.GetCartByUserId(SessionManager.CurrentUser.UserID);

                DisplayCartItems();
                UpdateTotal();
            }
            catch (Exception ex)
            {
                ShowError($"Error loading cart: {ex.Message}");
            }
        }

        private void DisplayCartItems()
        {
            CartItemsPanel.Children.Clear();

            if (_cartItems == null || _cartItems.Count == 0)
            {
                EmptyCartTextBlock.IsVisible = true;
                CheckoutButton.IsEnabled = false;
                ClearCartButton.IsEnabled = false;
                return;
            }

            EmptyCartTextBlock.IsVisible = false;
            CheckoutButton.IsEnabled = true;
            ClearCartButton.IsEnabled = true;

            foreach (var item in _cartItems)
            {
                var itemPanel = CreateCartItemPanel(item);
                CartItemsPanel.Children.Add(itemPanel);
            }
        }

        private Border CreateCartItemPanel(Cart cartItem)
        {
            var border = new Border
            {
                BorderBrush = Avalonia.Media.Brushes.LightGray,
                BorderThickness = new Avalonia.Thickness(1),
                CornerRadius = new Avalonia.CornerRadius(8),
                Padding = new Avalonia.Thickness(15),
                Margin = new Avalonia.Thickness(0, 0, 0, 10),
                Background = Avalonia.Media.Brushes.White
            };

            var grid = new Grid();
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(3, GridUnitType.Star) });
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });

            // Product Name
            var nameBlock = new TextBlock
            {
                Text = cartItem.Product.ProductName,
                FontSize = 16,
                FontWeight = Avalonia.Media.FontWeight.Bold,
                VerticalAlignment = VerticalAlignment.Center
            };
            Grid.SetColumn(nameBlock, 0);
            grid.Children.Add(nameBlock);

            // Price
            var priceBlock = new TextBlock
            {
                Text = $"${cartItem.Product.Price:F2}",
                FontSize = 14,
                VerticalAlignment = VerticalAlignment.Center
            };
            Grid.SetColumn(priceBlock, 1);
            grid.Children.Add(priceBlock);

            // Quantity controls
            var quantityPanel = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                VerticalAlignment = VerticalAlignment.Center
            };

            var decreaseBtn = new Button
            {
                Content = "-",
                Width = 30,
                Height = 30,
                Tag = cartItem.CartID
            };
            decreaseBtn.Click += DecreaseQuantity_Click;

            var quantityText = new TextBlock
            {
                Text = cartItem.Quantity.ToString(),
                FontSize = 14,
                VerticalAlignment = VerticalAlignment.Center,
                Margin = new Avalonia.Thickness(10, 0, 10, 0),
                MinWidth = 30,
                TextAlignment = Avalonia.Media.TextAlignment.Center
            };

            var increaseBtn = new Button
            {
                Content = "+",
                Width = 30,
                Height = 30,
                Tag = cartItem.CartID
            };
            increaseBtn.Click += IncreaseQuantity_Click;

            quantityPanel.Children.Add(decreaseBtn);
            quantityPanel.Children.Add(quantityText);
            quantityPanel.Children.Add(increaseBtn);

            Grid.SetColumn(quantityPanel, 2);
            grid.Children.Add(quantityPanel);

            // Subtotal
            var subtotalBlock = new TextBlock
            {
                Text = $"${(cartItem.Product.Price * cartItem.Quantity):F2}",
                FontSize = 16,
                FontWeight = Avalonia.Media.FontWeight.Bold,
                Foreground = Avalonia.Media.Brushes.Green,
                VerticalAlignment = VerticalAlignment.Center
            };
            Grid.SetColumn(subtotalBlock, 3);
            grid.Children.Add(subtotalBlock);

            // Remove button
            var removeBtn = new Button
            {
                Content = "🗑️",
                Background = Avalonia.Media.Brushes.Red,
                Foreground = Avalonia.Media.Brushes.White,
                Padding = new Avalonia.Thickness(10, 5),
                Tag = cartItem.CartID
            };
            removeBtn.Click += RemoveItem_Click;
            Grid.SetColumn(removeBtn, 4);
            grid.Children.Add(removeBtn);

            border.Child = grid;
            return border;
        }

        private void DecreaseQuantity_Click(object? sender, RoutedEventArgs e)
        {
            try
            {
                if (sender is Button button && button.Tag is int cartId)
                {
                    var cartItem = _cartItems.FirstOrDefault(c => c.CartID == cartId);
                    if (cartItem != null && cartItem.Quantity > 1)
                    {
                        var cartService = BLLFactory.GetCartService(BLLManager.CurrentBLLType);
                        cartService.UpdateCartItemQuantity(cartId, cartItem.Quantity - 1);
                        LoadCart();
                    }
                }
            }
            catch (Exception ex)
            {
                ShowError($"Error updating quantity: {ex.Message}");
            }
        }

        private void IncreaseQuantity_Click(object? sender, RoutedEventArgs e)
        {
            try
            {
                if (sender is Button button && button.Tag is int cartId)
                {
                    var cartItem = _cartItems.FirstOrDefault(c => c.CartID == cartId);
                    if (cartItem != null && cartItem.Quantity < cartItem.Product.StockQuantity)
                    {
                        var cartService = BLLFactory.GetCartService(BLLManager.CurrentBLLType);
                        cartService.UpdateCartItemQuantity(cartId, cartItem.Quantity + 1);
                        LoadCart();
                    }
                }
            }
            catch (Exception ex)
            {
                ShowError($"Error updating quantity: {ex.Message}");
            }
        }

        private void RemoveItem_Click(object? sender, RoutedEventArgs e)
        {
            try
            {
                if (sender is Button button && button.Tag is int cartId)
                {
                    var cartService = BLLFactory.GetCartService(BLLManager.CurrentBLLType);
                    cartService.RemoveFromCart(cartId);
                    LoadCart();
                }
            }
            catch (Exception ex)
            {
                ShowError($"Error removing item: {ex.Message}");
            }
        }

        private void UpdateTotal()
        {
            try
            {
                if (SessionManager.CurrentUser == null)
                {
                    TotalTextBlock.Text = "$0.00";
                    return;
                }

                var cartService = BLLFactory.GetCartService(BLLManager.CurrentBLLType);
                var total = cartService.GetCartTotal(SessionManager.CurrentUser.UserID);
                TotalTextBlock.Text = $"${total:F2}";
            }
            catch (Exception)
            {
                TotalTextBlock.Text = "$0.00";
            }
        }

        private async void CheckoutButton_Click(object? sender, RoutedEventArgs e)
        {
            if (_cartItems == null || _cartItems.Count == 0)
            {
                ShowError("Your cart is empty");
                return;
            }

            var checkoutWindow = new CheckoutWindow();
            await checkoutWindow.ShowDialog(this);
            
            // Reload cart in case checkout was successful
            LoadCart();
        }

        private async void ClearCartButton_Click(object? sender, RoutedEventArgs e)
        {
            try
            {
                if (SessionManager.CurrentUser == null) return;

                // Confirm dialog
                var confirmWindow = new Window
                {
                    Title = "Confirm",
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
                                Text = "Are you sure you want to clear your cart?",
                                TextWrapping = Avalonia.Media.TextWrapping.Wrap,
                                Margin = new Avalonia.Thickness(0, 0, 0, 20)
                            },
                            new StackPanel
                            {
                                Orientation = Orientation.Horizontal,
                                HorizontalAlignment = HorizontalAlignment.Center,
                                Children =
                                {
                                    new Button
                                    {
                                        Content = "Yes",
                                        Background = Avalonia.Media.Brushes.Red,
                                        Foreground = Avalonia.Media.Brushes.White,
                                        Padding = new Avalonia.Thickness(20, 10),
                                        Margin = new Avalonia.Thickness(0, 0, 10, 0)
                                    },
                                    new Button
                                    {
                                        Content = "No",
                                        Background = Avalonia.Media.Brushes.Gray,
                                        Foreground = Avalonia.Media.Brushes.White,
                                        Padding = new Avalonia.Thickness(20, 10)
                                    }
                                }
                            }
                        }
                    }
                };

                var yesButton = ((StackPanel)((StackPanel)confirmWindow.Content).Children[1]).Children[0] as Button;
                var noButton = ((StackPanel)((StackPanel)confirmWindow.Content).Children[1]).Children[1] as Button;

                bool? result = null;
                yesButton!.Click += (s, e) => { result = true; confirmWindow.Close(); };
                noButton!.Click += (s, e) => { result = false; confirmWindow.Close(); };

                await confirmWindow.ShowDialog(this);

                if (result == true)
                {
                    var cartService = BLLFactory.GetCartService(BLLManager.CurrentBLLType);
                    cartService.ClearCart(SessionManager.CurrentUser.UserID);
                    LoadCart();
                }
            }
            catch (Exception ex)
            {
                ShowError($"Error clearing cart: {ex.Message}");
            }
        }

        private void ContinueShoppingButton_Click(object? sender, RoutedEventArgs e)
        {
            Close();
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
                            HorizontalAlignment = HorizontalAlignment.Center,
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
