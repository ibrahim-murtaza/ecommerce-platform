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
            UpdateBLLTypeDisplay(); // ← UNCOMMENT THIS
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
                var user = userService.GetUserByEmail(email);

                if (user != null && user.PasswordHash == password)
                {
                    SessionManager.CurrentUser = user;
                    SessionManager.IsAdmin = false;

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

        private void AdminLoginButton_Click(object? sender, RoutedEventArgs e)
        {
            try
            {
                var email = AdminLoginEmailTextBox.Text?.Trim();
                var password = AdminLoginPasswordTextBox.Text;

                if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
                {
                    AdminLoginMessageTextBlock.Text = "Please enter both email and password";
                    AdminLoginMessageTextBlock.Foreground = Avalonia.Media.Brushes.Red;
                    return;
                }

                // Query admin from database
                var context = new ECommerce.DAL.ECommerceContext();
                var admin = context.Admins.FirstOrDefault(a => a.Email == email && a.PasswordHash == password && a.IsActive);

                if (admin != null)
                {
                    // Create a temporary user object for session (admin has limited User properties)
                    SessionManager.CurrentUser = new User
                    {
                        UserID = admin.AdminID,
                        Email = admin.Email,
                        FirstName = admin.FirstName ?? string.Empty,
                        LastName = admin.LastName ?? string.Empty
                    };
                    SessionManager.IsAdmin = true;

                    var mainWindow = new MainWindow();
                    mainWindow.Show();
                    this.Close();
                }
                else
                {
                    AdminLoginMessageTextBlock.Text = "Invalid admin credentials";
                    AdminLoginMessageTextBlock.Foreground = Avalonia.Media.Brushes.Red;
                }
            }
            catch (Exception ex)
            {
                AdminLoginMessageTextBlock.Text = $"Login error: {ex.Message}";
                AdminLoginMessageTextBlock.Foreground = Avalonia.Media.Brushes.Red;
            }
        }

        private void AdminRegisterButton_Click(object? sender, RoutedEventArgs e)
        {
            try
            {
                var email = AdminRegisterEmailTextBox.Text?.Trim();
                var password = AdminRegisterPasswordTextBox.Text;
                var confirmPassword = AdminRegisterConfirmPasswordTextBox.Text;
                var firstName = AdminRegisterFirstNameTextBox.Text?.Trim();
                var lastName = AdminRegisterLastNameTextBox.Text?.Trim();
                var role = ((ComboBoxItem?)AdminRoleComboBox.SelectedItem)?.Content?.ToString() ?? "Staff";

                // Validation
                if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password) ||
                    string.IsNullOrEmpty(firstName) || string.IsNullOrEmpty(lastName))
                {
                    AdminRegisterMessageTextBlock.Text = "All fields are required";
                    AdminRegisterMessageTextBlock.Foreground = Avalonia.Media.Brushes.Red;
                    return;
                }

                if (password != confirmPassword)
                {
                    AdminRegisterMessageTextBlock.Text = "Passwords do not match";
                    AdminRegisterMessageTextBlock.Foreground = Avalonia.Media.Brushes.Red;
                    return;
                }

                if (password.Length < 6)
                {
                    AdminRegisterMessageTextBlock.Text = "Password must be at least 6 characters";
                    AdminRegisterMessageTextBlock.Foreground = Avalonia.Media.Brushes.Red;
                    return;
                }

                var context = new ECommerce.DAL.ECommerceContext();

                // Check if email already exists
                var existingAdmin = context.Admins.FirstOrDefault(a => a.Email == email);
                if (existingAdmin != null)
                {
                    AdminRegisterMessageTextBlock.Text = "Email already registered";
                    AdminRegisterMessageTextBlock.Foreground = Avalonia.Media.Brushes.Red;
                    return;
                }

                // Create new admin
                var newAdmin = new Admin
                {
                    Email = email,
                    PasswordHash = password, // In production, hash this!
                    FirstName = firstName,
                    LastName = lastName,
                    Role = role,
                    DateCreated = DateTime.Now,
                    IsActive = true
                };

                context.Admins.Add(newAdmin);
                context.SaveChanges();

                AdminRegisterMessageTextBlock.Text = "Admin registration successful! Please login.";
                AdminRegisterMessageTextBlock.Foreground = Avalonia.Media.Brushes.Green;

                // Clear form
                AdminRegisterEmailTextBox.Text = "";
                AdminRegisterPasswordTextBox.Text = "";
                AdminRegisterConfirmPasswordTextBox.Text = "";
                AdminRegisterFirstNameTextBox.Text = "";
                AdminRegisterLastNameTextBox.Text = "";
            }
            catch (Exception ex)
            {
                AdminRegisterMessageTextBlock.Text = $"Registration error: {ex.Message}";
                AdminRegisterMessageTextBlock.Foreground = Avalonia.Media.Brushes.Red;
            }
        }
    }
}
