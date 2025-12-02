using ECommerce.BLL.Interfaces;
using ECommerce.DAL;
using ECommerce.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace ECommerce.BLL.LinqImplementation
{
    public class CategoryServiceLINQ : ICategoryService
    {
        private readonly ECommerceContext _context;

        public CategoryServiceLINQ(ECommerceContext context)
        {
            _context = context;
        }

        public List<Category> GetAllCategories()
        {
            return _context.Categories
                .Include(c => c.Products)
                .ToList();
        }

        public Category GetCategoryById(int id)
        {
            return _context.Categories
            .Include(c => c.Products)
            .FirstOrDefault(c => c.CategoryID == id)!;
        }

        public List<Category> GetActiveCategories()
        {
            return _context.Categories
                .Where(c => c.IsActive == true)
                .ToList();
        }

        public void AddCategory(Category category)
        {
            _context.Categories.Add(category);
            _context.SaveChanges();
        }

        public void UpdateCategory(Category category)
        {
            _context.Categories.Update(category);
            _context.SaveChanges();
        }

        public void DeleteCategory(int id)
        {
            var category = _context.Categories.Find(id);
            if (category != null)
            {
                _context.Categories.Remove(category);
                _context.SaveChanges();
            }
        }
    }
}
