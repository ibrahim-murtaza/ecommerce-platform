using Avalonia.Controls;
using Avalonia.Interactivity;
using ECommerce.Factory;
using ECommerce.Models;
using ECommerce.UI.Helpers;
using System;

namespace ECommerce.UI.Views
{
    public partial class ProductDetailWindow : Window
    {
        private readonly Product _product = null!;
        private int _quantity = 1;

        public ProductDetailWindow()
        {
            InitializeComponent();
        }

        public ProductDetailWindow(Product product) : this()
        {
            _product = product;
            DisplayProductDetails();
        }

        private void DisplayProductDetails()
        {
            ProductNameTextBlock.Text = _product.ProductName;
            CategoryTextBlock.Text = _product.Category?.CategoryName ?? "N/A";
            PriceTextBlock.Text = $"${_product.Price:F2}";
            StockTextBlock.Text = _product.StockQuantity.ToString();
            ProductIdTextBlock.Text = _product.ProductID.ToString();
            QuantityTextBox.Text = _quantity.ToString();
        }

        private void DecreaseButton_Click(object? sender, RoutedEventArgs e)
        {
            if (_quantity > 1)
            {
                _quantity--;
                QuantityTextBox.Text = _quantity.ToString();
            }
        }

        private void IncreaseButton_Click(object? sender, RoutedEventArgs e)
        {
            if (_quantity < _product.StockQuantity)
            {
                _quantity++;
                QuantityTextBox.Text = _quantity.ToString();
            }
            else
            {
                ShowMessage("Cannot exceed available stock", false);
            }
        }

        private void AddToCartButton_Click(object? sender, RoutedEventArgs e)
        {
            try
            {
                if (SessionManager.CurrentUser == null)
                {
                    ShowMessage("Please login to add items to cart", false);
                    return;
                }

                // Validate quantity input
                if (!int.TryParse(QuantityTextBox.Text, out int quantity) || quantity < 1)
                {
                    ShowMessage("Please enter a valid quantity", false);
                    return;
                }

                if (quantity > _product.StockQuantity)
                {
                    ShowMessage($"Only {_product.StockQuantity} items available", false);
                    return;
                }

                var cartService = BLLFactory.GetCartService(BLLManager.CurrentBLLType);
                cartService.AddToCart(SessionManager.CurrentUser.UserID, _product.ProductID, quantity);

                ShowMessage($"Added {quantity} item(s) to cart!", true);
                
                // Reset quantity after adding
                _quantity = 1;
                QuantityTextBox.Text = "1";
            }
            catch (Exception ex)
            {
                ShowMessage($"Error: {ex.Message}", false);
            }
        }

        private void CloseButton_Click(object? sender, RoutedEventArgs e)
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
