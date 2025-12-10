using Avalonia.Controls;
using Avalonia.Interactivity;
using ECommerce.Factory;
using ECommerce.Models;
using ECommerce.UI.Helpers;
using System;
using System.Linq;

namespace ECommerce.UI.Views
{
    public partial class LoginWindow : Window
    {
        public LoginWindow()
        {
            InitializeComponent();
            UpdateBLLTypeDisplay();
        }

        private void UpdateBLLTypeDisplay()
        {
            BLLTypeTextBlock.Text = $"Current Mode: {BLLManager.GetCurrentBLLTypeName()}";
        }

        private void ToggleBLLButton_Click(object? sender, RoutedEventArgs e)
        {
            BLLManager.ToggleBLLType();
            UpdateBLLTypeDisplay();
            LoginMessageTextBlock.Text = $"Switched to {BLLManager.GetCurrentBLLTypeName()} mode";
            LoginMessageTextBlock.Foreground = Avalonia.Media.Brushes.Green;
        }

        private void LoginButton_Click(object? sender, RoutedEventArgs e)
        {
            try
            {
                var email = LoginEmailTextBox.Text?.Trim();
                var password = LoginPasswordTextBox.Text;

                if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
                {
                    LoginMessageTextBlock.Text = "Please enter both email and password";
                    LoginMessageTextBlock.Foreground = Avalonia.Media.Brushes.Red;
                    return;
                }

                var userService = BLLFactory.GetUserService(BLLManager.CurrentBLLType);
                
                // For demo purposes, we'll use a simple password validation
                // In production, you'd hash the password and validate properly
                var user = userService.GetUserByEmail(email);
                
                if (user != null && user.PasswordHash == password)
                {
                    SessionManager.CurrentUser = user;
                    
                    // Open main window
                    var mainWindow = new MainWindow();
                    mainWindow.Show();
                    this.Close();
                }
                else
                {
                    LoginMessageTextBlock.Text = "Invalid email or password";
                    LoginMessageTextBlock.Foreground = Avalonia.Media.Brushes.Red;
                }
            }
            catch (Exception ex)
            {
                LoginMessageTextBlock.Text = $"Login error: {ex.Message}";
                LoginMessageTextBlock.Foreground = Avalonia.Media.Brushes.Red;
            }
        }

        private void RegisterButton_Click(object? sender, RoutedEventArgs e)
        {
            try
            {
                var email = RegisterEmailTextBox.Text?.Trim();
                var password = RegisterPasswordTextBox.Text;
                var confirmPassword = RegisterConfirmPasswordTextBox.Text;
                var firstName = RegisterFirstNameTextBox.Text?.Trim();
                var lastName = RegisterLastNameTextBox.Text?.Trim();
                var address = RegisterAddressTextBox.Text?.Trim();
                var city = RegisterCityTextBox.Text?.Trim();

                // Validation
                if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password) ||
                    string.IsNullOrEmpty(firstName) || string.IsNullOrEmpty(lastName) ||
                    string.IsNullOrEmpty(address) || string.IsNullOrEmpty(city))
                {
                    RegisterMessageTextBlock.Text = "All fields are required";
                    RegisterMessageTextBlock.Foreground = Avalonia.Media.Brushes.Red;
                    return;
                }

                if (password != confirmPassword)
                {
                    RegisterMessageTextBlock.Text = "Passwords do not match";
                    RegisterMessageTextBlock.Foreground = Avalonia.Media.Brushes.Red;
                    return;
                }

                if (password.Length < 6)
                {
                    RegisterMessageTextBlock.Text = "Password must be at least 6 characters";
                    RegisterMessageTextBlock.Foreground = Avalonia.Media.Brushes.Red;
                    return;
                }

                var userService = BLLFactory.GetUserService(BLLManager.CurrentBLLType);

                // Check if email already exists
                var existingUser = userService.GetUserByEmail(email);
                if (existingUser != null)
                {
                    RegisterMessageTextBlock.Text = "Email already registered";
                    RegisterMessageTextBlock.Foreground = Avalonia.Media.Brushes.Red;
                    return;
                }

                // Create new user
                var newUser = new User
                {
                    Email = email,
                    PasswordHash = password, // In production, hash this!
                    FirstName = firstName,
                    LastName = lastName,
                    Address = address,
                    City = city
                };

                userService.AddUser(newUser);

                RegisterMessageTextBlock.Text = "Registration successful! Please login.";
                RegisterMessageTextBlock.Foreground = Avalonia.Media.Brushes.Green;

                // Clear form
                RegisterEmailTextBox.Text = "";
                RegisterPasswordTextBox.Text = "";
                RegisterConfirmPasswordTextBox.Text = "";
                RegisterFirstNameTextBox.Text = "";
                RegisterLastNameTextBox.Text = "";
                RegisterAddressTextBox.Text = "";
                RegisterCityTextBox.Text = "";
            }
            catch (Exception ex)
            {
                RegisterMessageTextBlock.Text = $"Registration error: {ex.Message}";
                RegisterMessageTextBlock.Foreground = Avalonia.Media.Brushes.Red;
            }
        }
    }
}
