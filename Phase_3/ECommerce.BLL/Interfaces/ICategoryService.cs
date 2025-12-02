using ECommerce.Models;
using System.Collections.Generic;

namespace ECommerce.BLL.Interfaces
{
    public interface ICategoryService
    {
        List<Category> GetAllCategories();
        Category GetCategoryById(int id);
        List<Category> GetActiveCategories();
        void AddCategory(Category category);
        void UpdateCategory(Category category);
        void DeleteCategory(int id);
    }
}
