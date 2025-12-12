using System;
using System.Linq;
using ECommerce.Factory;
using ECommerce.Models;

namespace ECommerce.UI
{
    public static class IntegrationTester
    {
        public static void RunTests()
        {
            Console.WriteLine("=================================================");
            Console.WriteLine("             STARTING BACKEND TESTS");
            Console.WriteLine("=================================================");

            // 1. Factory & Basic Read
            TestProductService();

            // 2. Authentication Logic (Reg/Login)
            TestAuthFlow();

            // 3. Admin & Analytics (Revenue/Low Stock)
            TestAdminFeatures();

            // 4. Complex Order Transaction
            TestOrderFlow();

            Console.WriteLine("\nAll Backend Tests Complete. Press Enter to launch App...");
            Console.ReadLine();
        }

        private static void TestProductService()
        {
            Console.WriteLine("\n--- 1. Testing Product Switching (LINQ vs SP) ---");
            
            var linq = BLLFactory.GetProductService(BLLType.LINQ).GetAllProducts();
            Console.WriteLine($"[LINQ] Products: {linq.Count}");

            var sp = BLLFactory.GetProductService(BLLType.StoredProcedure).GetAllProducts();
            Console.WriteLine($"[SP]   Products: {sp.Count}");

            if (linq.Count > 0 && sp.Count > 0) 
                Console.WriteLine("✅ Factory Switching Works");
            else 
                Console.WriteLine("❌ Factory Error");
        }

        private static void TestAuthFlow()
        {
            Console.WriteLine("\n--- 2. Testing Registration & Login Logic ---");
            var userService = BLLFactory.GetUserService(BLLType.StoredProcedure);

            string testEmail = "tester_" + Guid.NewGuid().ToString().Substring(0,5) + "@test.com";
            string testPass = "Pass123";

            try
            {
                // A. Registration
                var newUser = new User 
                { 
                    Email = testEmail, 
                    PasswordHash = testPass, 
                    FirstName = "Test", 
                    LastName = "User", 
                    Address = "123 Code St", 
                    City = "BugFree Zone"
                };
                
                userService.AddUser(newUser);
                Console.WriteLine($"✅ Registration Successful for {testEmail}");

                bool loginSuccess = userService.ValidateUserCredentials(testEmail, testPass);
                
                if (loginSuccess)
                    Console.WriteLine("✅ Login Credentials Validated");
                else
                    Console.WriteLine("❌ Login Validation Failed");

                try {
                    userService.AddUser(newUser); 
                    Console.WriteLine("❌ Duplicate User Allowed (Fail)");
                } catch {
                    Console.WriteLine("✅ Duplicate User Prevented (Expected Exception)");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Auth Test Failed: {ex.Message}");
            }
        }

        private static void TestAdminFeatures()
        {
            Console.WriteLine("\n--- 3. Testing Admin & Analytics ---");
            
            var orderService = BLLFactory.GetOrderService(BLLType.StoredProcedure);
            DateTime start = DateTime.Now.AddYears(-5); 
            DateTime end = DateTime.Now.AddDays(1);
            
            try {
                decimal revenue = orderService.GetTotalRevenueByDateRange(start, end);
                Console.WriteLine($"✅ Revenue Calculation: ${revenue:N2}");
            } catch (Exception ex) {
                Console.WriteLine($"❌ Revenue Calc Failed: {ex.Message}");
            }
        }

        private static void TestOrderFlow()
        {
            Console.WriteLine("\n--- 4. Testing Cart & Checkout Transaction ---");
            try
            {
                int userId = 1; 
                int productId = 1; 
                int quantity = 1;

                var cartService = BLLFactory.GetCartService(BLLType.StoredProcedure);
                
                var initialCart = cartService.GetCartByUserId(userId);
                int initialCount = initialCart.Sum(c => c.Quantity);

                cartService.AddToCart(userId, productId, quantity);
                Console.WriteLine("✅ Item Added to Cart (Stock Validated)");

                var updatedCart = cartService.GetCartByUserId(userId);
                int newCount = updatedCart.Sum(c => c.Quantity);
                
                if (newCount > initialCount)
                    Console.WriteLine($"✅ Cart Count Increased ({initialCount} -> {newCount})");
                else
                    Console.WriteLine("❌ Cart Count did not increase");

                var orderService = BLLFactory.GetOrderService(BLLType.StoredProcedure);
                
                orderService.PlaceOrder(userId, "Integration Test St", "Console City", "10101");
                Console.WriteLine("✅ Order Placed Successfully via Stored Procedure");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Order Flow Failed: {ex.Message}");
            }
        }
    }
}