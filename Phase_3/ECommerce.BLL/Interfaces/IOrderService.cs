using ECommerce.Models;
using System;
using System.Collections.Generic;

namespace ECommerce.BLL.Interfaces
{
    public interface IOrderService
    {
        List<Order> GetAllOrders();
        Order GetOrderById(int orderId, DateTime orderDate);
        List<Order> GetOrdersByUserId(int userId);
        void PlaceOrder(int userId, string shippingAddress, string shippingCity, string shippingPostalCode);
        void UpdateOrderStatus(int orderId, string newStatus);
        List<Order> GetOrdersByStatus(string status);
        decimal GetTotalRevenueByDateRange(DateTime startDate, DateTime endDate);
    }
}
