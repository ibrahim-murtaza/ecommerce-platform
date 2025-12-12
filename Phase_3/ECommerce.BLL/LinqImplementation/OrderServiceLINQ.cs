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
                    .ThenInclude(oi => oi.Product)
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
                    .ThenInclude(oi => oi.Product)
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

                var cartItems = _context.Carts
                    .Include(c => c.Product)
                    .Where(c => c.UserID == userId)
                    .ToList();

                if (!cartItems.Any())
                {
                    throw new InvalidOperationException("Cart is empty. Cannot place order.");
                }

                decimal totalAmount = cartItems.Sum(c => c.Product.Price * c.Quantity);


                var orderIdQuery = _context.Database.SqlQueryRaw<int>(
                    @"DECLARE @InsertedOrders TABLE (OrderID INT, OrderDate DATETIME2);
                      INSERT INTO [Order] (UserID, OrderDate, TotalAmount, Status, ShippingAddress) 
                      OUTPUT INSERTED.OrderID, INSERTED.OrderDate INTO @InsertedOrders
                      VALUES ({0}, {1}, {2}, {3}, {4});
                      SELECT OrderID FROM @InsertedOrders;",
                    userId, orderDate, totalAmount, "Pending", shippingAddress).AsEnumerable();

                var orderId = orderIdQuery.FirstOrDefault();

                if (orderId == 0)
                {
                    throw new InvalidOperationException("Failed to create order.");
                }


                foreach (var cartItem in cartItems)
                {
                    _context.Database.ExecuteSqlRaw(
                        @"INSERT INTO OrderItem (OrderID, OrderDate, ProductID, Quantity, PriceAtPurchase) 
                          VALUES ({0}, {1}, {2}, {3}, {4})",
                        orderId, orderDate, cartItem.ProductID, cartItem.Quantity, cartItem.Product.Price);
                }


                foreach (var cartItem in cartItems)
                {
                    _context.Database.ExecuteSqlRaw(
                        @"UPDATE Product 
                          SET StockQuantity = StockQuantity - {0} 
                          WHERE ProductID = {1}",
                        cartItem.Quantity, cartItem.ProductID);
                }


                _context.Database.ExecuteSqlRaw(
                    "DELETE FROM Cart WHERE UserID = {0}", userId);

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

            _context.Database.ExecuteSqlRaw(
                "UPDATE [Order] SET Status = {0} WHERE OrderID = {1}",
                newStatus, orderId);
        }

        public List<Order> GetOrdersByStatus(string status)
        {
            return _context.Orders
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
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