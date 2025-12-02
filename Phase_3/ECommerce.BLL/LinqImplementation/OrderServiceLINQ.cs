using ECommerce.BLL.Interfaces;
using ECommerce.DAL;
using ECommerce.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ECommerce.BLL.LinqImplementation
{
    public class OrderServiceLINQ : IOrderService
    {
        private readonly ECommerceContext _context;

        public OrderServiceLINQ(ECommerceContext context)
        {
            _context = context;
        }

        public List<Order> GetAllOrders()
        {
            return _context.Orders
                .Include(o => o.OrderItems)
                .Include(o => o.User)
                .ToList();
        }

        public Order GetOrderById(int orderId, DateTime orderDate)
        {
            return _context.Orders
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                .Include(o => o.User)
                .FirstOrDefault(o => o.OrderID == orderId && o.OrderDate == orderDate)!;
        }

        public List<Order> GetOrdersByUserId(int userId)
        {
            return _context.Orders
                .Include(o => o.OrderItems)
                .Where(o => o.UserID == userId)
                .OrderByDescending(o => o.OrderDate)
                .ToList();
        }

        public void PlaceOrder(int userId, string shippingAddress, string shippingCity, string shippingPostalCode)
        {
            using var transaction = _context.Database.BeginTransaction();
            try
            {
                var orderDate = DateTime.Now;

                // Calculate total from cart
                var cartItems = _context.Carts
                    .Include(c => c.Product)
                    .Where(c => c.UserID == userId)
                    .ToList();

                if (!cartItems.Any())
                {
                    throw new InvalidOperationException("Cart is empty. Cannot place order.");
                }

                decimal totalAmount = cartItems.Sum(c => c.Product.Price * c.Quantity);

                // Create order (note: Order model doesn't have ShippingCity and ShippingPostalCode)
                var order = new Order
                {
                    UserID = userId,
                    OrderDate = orderDate,
                    TotalAmount = totalAmount,
                    Status = "Pending",
                    ShippingAddress = shippingAddress
                };

                _context.Orders.Add(order);
                _context.SaveChanges();

                // Create order items
                foreach (var cartItem in cartItems)
                {
                    var orderItem = new OrderItem
                    {
                        OrderID = order.OrderID,
                        OrderDate = orderDate,
                        ProductID = cartItem.ProductID,
                        Quantity = cartItem.Quantity,
                        PriceAtPurchase = cartItem.Product.Price
                    };
                    _context.OrderItems.Add(orderItem);

                    // Update stock (trigger would handle this in SP version, but we do it manually in LINQ)
                    var product = _context.Products.Find(cartItem.ProductID);
                    if (product != null)
                    {
                        product.StockQuantity -= cartItem.Quantity;
                    }
                }

                // Clear cart
                _context.Carts.RemoveRange(cartItems);

                _context.SaveChanges();
                transaction.Commit();
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }

        public void UpdateOrderStatus(int orderId, string newStatus)
        {
            // Find any order with this OrderID (we need to get OrderDate too)
            var order = _context.Orders.FirstOrDefault(o => o.OrderID == orderId);
            if (order != null)
            {
                order.Status = newStatus;
                _context.SaveChanges();
            }
        }

        public List<Order> GetOrdersByStatus(string status)
        {
            return _context.Orders
                .Include(o => o.OrderItems)
                .Where(o => o.Status == status)
                .ToList();
        }

        public decimal GetTotalRevenueByDateRange(DateTime startDate, DateTime endDate)
        {
            return _context.Orders
                .Where(o => o.OrderDate >= startDate && o.OrderDate <= endDate)
                .Sum(o => o.TotalAmount);
        }
    }
}