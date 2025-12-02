using ECommerce.Models;
using System.Collections.Generic;

namespace ECommerce.BLL.Interfaces
{
    public interface ICartService
    {
        List<Cart> GetCartByUserId(int userId);
        void AddToCart(int userId, int productId, int quantity);
        void UpdateCartItemQuantity(int cartId, int newQuantity);
        void RemoveFromCart(int cartId);
        void ClearCart(int userId);
        decimal GetCartTotal(int userId);
    }
}
