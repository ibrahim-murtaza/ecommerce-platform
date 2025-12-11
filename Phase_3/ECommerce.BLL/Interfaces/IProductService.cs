using ECommerce.Models;
using System.Collections.Generic;

namespace ECommerce.BLL.Interfaces
{
    public interface IProductService
    {
        List<Product> GetAllProducts();
        Product GetProductById(int id);
        List<Product> GetProductsByCategory(int categoryId);
        void AddProduct(Product product);
        void UpdateProduct(Product product);
        void DeleteProduct(int id);
    }
}