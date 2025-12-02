using ECommerce.BLL.Interfaces;
using ECommerce.DAL;
using ECommerce.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace ECommerce.BLL.LinqImplementation
{
    public class ProductServiceLINQ : IProductService
    {
        private readonly ECommerceContext _context;

        public ProductServiceLINQ(ECommerceContext context)
        {
            _context = context;
        }

        public List<Product> GetAllProducts()
        {
            return _context.Products
                .Include(p => p.Category)
                .ToList();
        }

        public Product GetProductById(int id)
        {
            return _context.Products
            .Include(p => p.Category)
            .FirstOrDefault(p => p.ProductID == id)!;
        }

        public List<Product> GetProductsByCategory(int categoryId)
        {
            return _context.Products
                .Include(p => p.Category)
                .Where(p => p.CategoryID == categoryId)
                .ToList();
        }

        public void AddProduct(Product product)
        {
            _context.Products.Add(product);
            _context.SaveChanges();
        }

        public void UpdateProduct(Product product)
        {
            _context.Products.Update(product);
            _context.SaveChanges();
        }

        public void DeleteProduct(int id)
        {
            var product = _context.Products.Find(id);
            if (product != null)
            {
                _context.Products.Remove(product);
                _context.SaveChanges();
            }
        }

        public List<Product> GetLowStockProducts()
        {
            // Uses the vw_LowStockProducts view
            return _context.Products
                .FromSqlRaw("SELECT * FROM vw_LowStockProducts")
                .ToList();
        }
    }
}
