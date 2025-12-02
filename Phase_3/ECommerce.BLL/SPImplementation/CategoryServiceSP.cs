using ECommerce.BLL.Interfaces;
using ECommerce.DAL;
using ECommerce.Models;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace ECommerce.BLL.SPImplementation
{
    public class CategoryServiceSP : ICategoryService
    {
        private readonly ECommerceContext _context;

        public CategoryServiceSP(ECommerceContext context)
        {
            _context = context;
        }

        public List<Category> GetAllCategories()
        {
            return _context.Categories
                .FromSqlRaw("SELECT * FROM Category")
                .ToList();
        }

        public Category GetCategoryById(int id)
        {
            var idParam = new SqlParameter("@CategoryID", id);

            return _context.Categories
            .FromSqlRaw("SELECT * FROM Category WHERE CategoryID = @CategoryID", idParam)
            .AsEnumerable()
            .FirstOrDefault()!;
        }

        public List<Category> GetActiveCategories()
        {
            return _context.Categories
                .FromSqlRaw("SELECT * FROM Category WHERE IsActive = 1")
                .ToList();
        }

        public void AddCategory(Category category)
        {
            var parameters = new[]
            {
                new SqlParameter("@CategoryName", category.CategoryName),
                new SqlParameter("@Description", category.Description),
                new SqlParameter("@IsActive", category.IsActive)
            };

            _context.Database.ExecuteSqlRaw(
                "INSERT INTO Category (CategoryName, Description, IsActive) VALUES (@CategoryName, @Description, @IsActive)",
                parameters);
        }

        public void UpdateCategory(Category category)
        {
            var parameters = new[]
            {
                new SqlParameter("@CategoryID", category.CategoryID),
                new SqlParameter("@CategoryName", category.CategoryName),
                new SqlParameter("@Description", category.Description),
                new SqlParameter("@IsActive", category.IsActive)
            };

            _context.Database.ExecuteSqlRaw(
                "UPDATE Category SET CategoryName = @CategoryName, Description = @Description, IsActive = @IsActive WHERE CategoryID = @CategoryID",
                parameters);
        }

        public void DeleteCategory(int id)
        {
            var idParam = new SqlParameter("@CategoryID", id);
            _context.Database.ExecuteSqlRaw("DELETE FROM Category WHERE CategoryID = @CategoryID", idParam);
        }
    }
}
