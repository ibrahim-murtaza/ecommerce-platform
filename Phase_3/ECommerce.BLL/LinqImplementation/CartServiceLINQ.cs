using ECommerce.BLL.Interfaces;
using ECommerce.DAL;
using ECommerce.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace ECommerce.BLL.LinqImplementation
{
    public class CartServiceLINQ : ICartService
    {
        private readonly ECommerceContext _context;

        public CartServiceLINQ(ECommerceContext context)
        {
            _context = context;
        }

        public List<Cart> GetCartByUserId(int userId)
        {
            return _context.Carts
                .Include(c => c.Product)
                    .ThenInclude(p => p.Category)
                .Where(c => c.UserID == userId)
                .ToList();
        }

        public void AddToCart(int userId, int productId, int quantity)
        {
            // Check if product exists and has stock
            var product = _context.Products.Find(productId);
            if (product == null || !product.IsActive)
            {
                throw new InvalidOperationException("Product not found or inactive.");
            }

            if (product.StockQuantity < quantity)
            {
                throw new InvalidOperationException("Insufficient stock.");
            }

            // Check if item already in cart
            var existingCartItem = _context.Carts
                .FirstOrDefault(c => c.UserID == userId && c.ProductID == productId);

            if (existingCartItem != null)
            {
                // Update quantity using raw SQL to avoid trigger issues
                _context.Database.ExecuteSqlRaw(
                    "UPDATE Cart SET Quantity = Quantity + {0} WHERE CartID = {1}",
                    quantity, existingCartItem.CartID);
            }
            else
            {
                // Add new cart item using raw SQL to work with INSTEAD OF trigger
                _context.Database.ExecuteSqlRaw(
                    "INSERT INTO Cart (UserID, ProductID, Quantity, DateAdded) VALUES ({0}, {1}, {2}, GETDATE())",
                    userId, productId, quantity);
            }
        }

        public void UpdateCartItemQuantity(int cartId, int newQuantity)
        {
            var cartItem = _context.Carts.Find(cartId);
            if (cartItem != null)
            {
                if (newQuantity <= 0)
                {
                    _context.Carts.Remove(cartItem);
                }
                else
                {
                    cartItem.Quantity = newQuantity;
                }
                _context.SaveChanges();
            }
        }

        public void RemoveFromCart(int cartId)
        {
            var cartItem = _context.Carts.Find(cartId);
            if (cartItem != null)
            {
                _context.Carts.Remove(cartItem);
                _context.SaveChanges();
            }
        }

        public void ClearCart(int userId)
        {
            var cartItems = _context.Carts.Where(c => c.UserID == userId).ToList();
            _context.Carts.RemoveRange(cartItems);
            _context.SaveChanges();
        }

        public decimal GetCartTotal(int userId)
        {
            return _context.Carts
                .Include(c => c.Product)
                .Where(c => c.UserID == userId)
                .Sum(c => c.Product.Price * c.Quantity);
        }
    }
}
