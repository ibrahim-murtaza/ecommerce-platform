using ECommerce.BLL.Interfaces;
using ECommerce.DAL;
using ECommerce.Models;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace ECommerce.BLL.SPImplementation
{
    public class ProductServiceSP : IProductService
    {
        private readonly ECommerceContext _context;

        public ProductServiceSP(ECommerceContext context)
        {
            _context = context;
        }

        public List<Product> GetAllProducts()
        {
            return _context.Products
                .FromSqlRaw("SELECT * FROM Product WHERE IsActive = 1")
                .Include(p => p.Category)
                .ToList();
        }

        public Product GetProductById(int id)
        {
            var idParam = new SqlParameter("@ProductID", id);

            return _context.Products
                .FromSqlRaw("SELECT * FROM Product WHERE ProductID = @ProductID", idParam)
                .Include(p => p.Category)
                .AsEnumerable()
                .FirstOrDefault()!;
        }

        public List<Product> GetProductsByCategory(int categoryId)
        {
            var categoryParam = new SqlParameter("@CategoryID", categoryId);

            return _context.Products
                .FromSqlRaw("SELECT * FROM Product WHERE CategoryID = @CategoryID AND IsActive = 1", categoryParam)
                .Include(p => p.Category)
                .ToList();
        }

        public void AddProduct(Product product)
        {
            var parameters = new[]
            {
                new SqlParameter("@CategoryID", product.CategoryID),
                new SqlParameter("@ProductName", product.ProductName),
                new SqlParameter("@Price", product.Price),
                new SqlParameter("@StockQuantity", product.StockQuantity),
                new SqlParameter("@ImageURL", (object?)product.ImageURL ?? DBNull.Value),
                new SqlParameter("@IsActive", product.IsActive)
            };

            _context.Database.ExecuteSqlRaw(
                "INSERT INTO Product (CategoryID, ProductName, Price, StockQuantity, ImageURL, IsActive, DateAdded) " +
                "VALUES (@CategoryID, @ProductName, @Price, @StockQuantity, @ImageURL, @IsActive, GETDATE())",
                parameters);
        }

        public void UpdateProduct(Product product)
        {
            var parameters = new[]
            {
                new SqlParameter("@ProductID", product.ProductID),
                new SqlParameter("@CategoryID", product.CategoryID),
                new SqlParameter("@ProductName", product.ProductName),
                new SqlParameter("@Price", product.Price),
                new SqlParameter("@StockQuantity", product.StockQuantity),
                new SqlParameter("@ImageURL", product.ImageURL ?? (object)DBNull.Value),
                new SqlParameter("@IsActive", product.IsActive)
            };

            _context.Database.ExecuteSqlRaw(
                "UPDATE Product SET CategoryID = @CategoryID, ProductName = @ProductName, " +
                "Price = @Price, StockQuantity = @StockQuantity, " +
                "ImageURL = @ImageURL, IsActive = @IsActive WHERE ProductID = @ProductID",
                parameters);
        }

        public void DeleteProduct(int id)
        {
            var idParam = new SqlParameter("@ProductID", id);
            _context.Database.ExecuteSqlRaw("DELETE FROM Product WHERE ProductID = @ProductID", idParam);
        }
    }
}