using Avalonia.Controls;
using Avalonia.Interactivity;
using ECommerce.Factory;
using ECommerce.UI.Helpers;
using System;

namespace ECommerce.UI.Views
{
    public partial class CheckoutWindow : Window
    {
        public CheckoutWindow()
        {
            InitializeComponent();
            LoadUserInfo();
            LoadCartTotal();
        }

        private void LoadUserInfo()
        {
            if (SessionManager.CurrentUser != null)
            {
                AddressTextBox.Text = SessionManager.CurrentUser.Address;
                CityTextBox.Text = SessionManager.CurrentUser.City;
            }
        }

        private void LoadCartTotal()
        {
            try
            {
                if (SessionManager.CurrentUser == null) return;

                var cartService = BLLFactory.GetCartService(BLLManager.CurrentBLLType);
                var total = cartService.GetCartTotal(SessionManager.CurrentUser.UserID);
                
                CartTotalTextBlock.Text = $"${total:F2}";
                OrderTotalTextBlock.Text = $"${total:F2}";
            }
            catch (Exception ex)
            {
                ShowMessage($"Error loading total: {ex.Message}", false);
            }
        }

        private void PlaceOrderButton_Click(object? sender, RoutedEventArgs e)
        {
            try
            {
                if (SessionManager.CurrentUser == null)
                {
                    ShowMessage("User session not found", false);
                    return;
                }

                var address = AddressTextBox.Text?.Trim();
                var city = CityTextBox.Text?.Trim();
                var postalCode = PostalCodeTextBox.Text?.Trim();

                // Validation
                if (string.IsNullOrEmpty(address))
                {
                    ShowMessage("Please enter shipping address", false);
                    return;
                }

                if (string.IsNullOrEmpty(city))
                {
                    ShowMessage("Please enter city", false);
                    return;
                }

                if (string.IsNullOrEmpty(postalCode))
                {
                    ShowMessage("Please enter postal code", false);
                    return;
                }

                // Check if cart is not empty
                var cartService = BLLFactory.GetCartService(BLLManager.CurrentBLLType);
                var cartItems = cartService.GetCartByUserId(SessionManager.CurrentUser.UserID);
                
                if (cartItems == null || cartItems.Count == 0)
                {
                    ShowMessage("Your cart is empty", false);
                    return;
                }

                // Place the order
                var orderService = BLLFactory.GetOrderService(BLLManager.CurrentBLLType);
                orderService.PlaceOrder(
                    SessionManager.CurrentUser.UserID,
                    address,
                    city,
                    postalCode
                );

                ShowMessage("Order placed successfully!", true);
                PlaceOrderButton.IsEnabled = false;

                // Close after 2 seconds
                var timer = new System.Timers.Timer(2000);
                timer.Elapsed += (s, args) =>
                {
                    timer.Stop();
                    Avalonia.Threading.Dispatcher.UIThread.Post(() => Close());
                };
                timer.Start();
            }
            catch (Exception ex)
            {
                ShowMessage($"Error placing order: {ex.Message}", false);
            }
        }

        private void CancelButton_Click(object? sender, RoutedEventArgs e)
        {
            Close();
        }

        private void ShowMessage(string message, bool isSuccess)
        {
            MessageTextBlock.Text = message;
            MessageTextBlock.Foreground = isSuccess 
                ? Avalonia.Media.Brushes.Green 
                : Avalonia.Media.Brushes.Red;
        }
    }
}
