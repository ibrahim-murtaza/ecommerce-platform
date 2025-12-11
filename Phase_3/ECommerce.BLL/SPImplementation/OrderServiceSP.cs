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
    public class OrderServiceSP : IOrderService
    {
        private readonly ECommerceContext _context;

        public OrderServiceSP(ECommerceContext context)
        {
            _context = context;
        }

        public List<Order> GetAllOrders()
        {
            return _context.Orders
                .FromSqlRaw("SELECT * FROM [Order]")
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                .Include(o => o.User)
                .ToList();
        }

        public Order GetOrderById(int orderId, DateTime orderDate)
        {
            var parameters = new[]
            {
                new SqlParameter("@OrderID", orderId),
                new SqlParameter("@OrderDate", orderDate)
            };

            return _context.Orders
            .FromSqlRaw("SELECT * FROM [Order] WHERE OrderID = @OrderID AND OrderDate = @OrderDate", parameters)
            .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
            .Include(o => o.User)
            .AsEnumerable()
            .FirstOrDefault()!;
        }

        public List<Order> GetOrdersByUserId(int userId)
        {
            var userIdParam = new SqlParameter("@UserID", userId);

            return _context.Orders
                .FromSqlRaw("SELECT * FROM [Order] WHERE UserID = @UserID", userIdParam)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                .OrderByDescending(o => o.OrderDate)
                .ToList();
        }

        public void PlaceOrder(int userId, string shippingAddress, string shippingCity, string shippingPostalCode)
        {
            // Use the sp_PlaceOrder stored procedure
            var parameters = new[]
            {
                new SqlParameter("@UserID", userId),
                new SqlParameter("@ShippingAddress", shippingAddress),
                new SqlParameter("@ShippingCity", shippingCity),
                new SqlParameter("@ShippingPostalCode", shippingPostalCode)
            };

            _context.Database.ExecuteSqlRaw(
                "EXEC sp_PlaceOrder @UserID, @ShippingAddress, @ShippingCity, @ShippingPostalCode",
                parameters);
            
            // Note: Stock update is handled by the trigger trg_AfterOrderItem_UpdateStock
            // or within the stored procedure itself
        }

        public void UpdateOrderStatus(int orderId, string newStatus)
        {
            // Use the sp_UpdateOrderStatus stored procedure
            var parameters = new[]
            {
                new SqlParameter("@OrderID", orderId),
                new SqlParameter("@NewStatus", newStatus)
            };

            _context.Database.ExecuteSqlRaw(
                "EXEC sp_UpdateOrderStatus @OrderID, @NewStatus",
                parameters);
        }

        public List<Order> GetOrdersByStatus(string status)
        {
            var statusParam = new SqlParameter("@Status", status);

            return _context.Orders
                .FromSqlRaw("SELECT * FROM [Order] WHERE Status = @Status", statusParam)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                .ToList();
        }

        public decimal GetTotalRevenueByDateRange(DateTime startDate, DateTime endDate)
        {
            var parameters = new[]
            {
                new SqlParameter("@StartDate", startDate),
                new SqlParameter("@EndDate", endDate)
            };

            var result = _context.Database
                .SqlQueryRaw<decimal>(
                    "SELECT ISNULL(SUM(TotalAmount), 0) FROM [Order] WHERE OrderDate >= @StartDate AND OrderDate <= @EndDate",
                    parameters)
                .AsEnumerable()
                .FirstOrDefault();

            return result;
        }
    }
}
