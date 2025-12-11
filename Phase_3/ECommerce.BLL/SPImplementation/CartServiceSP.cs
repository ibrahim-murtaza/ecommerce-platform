using ECommerce.BLL.Interfaces;
using ECommerce.DAL;
using ECommerce.Models;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ECommerce.BLL.SPImplementation
{
    public class CartServiceSP : ICartService
    {
        private readonly ECommerceContext _context;

        public CartServiceSP(ECommerceContext context)
        {
            _context = context;
        }

        public List<Cart> GetCartByUserId(int userId)
        {
            var userIdParam = new SqlParameter("@UserID", userId);

            return _context.Carts
                .FromSqlRaw("SELECT * FROM Cart WHERE UserID = @UserID", userIdParam)
                .Include(c => c.Product)
                    .ThenInclude(p => p.Category)
                .ToList();
        }

        public void AddToCart(int userId, int productId, int quantity)
        {
            // Check stock availability using the function
            var checkStockParams = new[]
            {
                new SqlParameter("@ProductID", productId),
                new SqlParameter("@RequestedQuantity", quantity),
                new SqlParameter("@Result", System.Data.SqlDbType.Bit) { Direction = System.Data.ParameterDirection.Output }
            };

            _context.Database.ExecuteSqlRaw(
                "SELECT @Result = dbo.CheckStockAvailability(@ProductID, @RequestedQuantity)",
                checkStockParams);

            bool isAvailable = (bool)checkStockParams[2].Value;

            if (!isAvailable)
            {
                throw new InvalidOperationException("Insufficient stock or product inactive.");
            }

            // Check if item already exists in cart
            var existingCartParam = new[]
            {
                new SqlParameter("@UserID", userId),
                new SqlParameter("@ProductID", productId)
            };

            var existingCart = _context.Carts
                .FromSqlRaw("SELECT * FROM Cart WHERE UserID = @UserID AND ProductID = @ProductID", existingCartParam)
                .AsEnumerable()
                .FirstOrDefault();

            if (existingCart != null)
            {
                // Update quantity
                var updateParams = new[]
                {
                    new SqlParameter("@CartID", existingCart.CartID),
                    new SqlParameter("@NewQuantity", existingCart.Quantity + quantity)
                };

                _context.Database.ExecuteSqlRaw(
                    "UPDATE Cart SET Quantity = @NewQuantity WHERE CartID = @CartID",
                    updateParams);
            }
            else
            {
                // Insert new cart item
                var insertParams = new[]
                {
                    new SqlParameter("@UserID", userId),
                    new SqlParameter("@ProductID", productId),
                    new SqlParameter("@Quantity", quantity)
                };

                _context.Database.ExecuteSqlRaw(
                    "INSERT INTO Cart (UserID, ProductID, Quantity, DateAdded) VALUES (@UserID, @ProductID, @Quantity, GETDATE())",
                    insertParams);
            }
        }

        public void UpdateCartItemQuantity(int cartId, int newQuantity)
        {
            if (newQuantity <= 0)
            {
                RemoveFromCart(cartId);
            }
            else
            {
                var parameters = new[]
                {
                    new SqlParameter("@CartID", cartId),
                    new SqlParameter("@Quantity", newQuantity)
                };

                _context.Database.ExecuteSqlRaw(
                    "UPDATE Cart SET Quantity = @Quantity WHERE CartID = @CartID",
                    parameters);
            }
        }

        public void RemoveFromCart(int cartId)
        {
            var cartIdParam = new SqlParameter("@CartID", cartId);
            _context.Database.ExecuteSqlRaw("DELETE FROM Cart WHERE CartID = @CartID", cartIdParam);
        }

        public void ClearCart(int userId)
        {
            var userIdParam = new SqlParameter("@UserID", userId);
            _context.Database.ExecuteSqlRaw("DELETE FROM Cart WHERE UserID = @UserID", userIdParam);
        }

        public decimal GetCartTotal(int userId)
        {
            // Use the CalculateCartTotal function
            var userIdParam = new SqlParameter("@UserID", userId);

            var result = _context.Database
                .SqlQueryRaw<decimal>("SELECT dbo.CalculateCartTotal(@UserID) AS Value", userIdParam)
                .AsEnumerable()
                .FirstOrDefault();

            return result;
        }
    }
}
