using Shopping_Web.DataAccess;
using Shopping_Web.Models;

namespace Shopping_Web.Services
{
    public class CategoryService : ICategoryService
    {
        ICategoryDA _categoryDA;
        public CategoryService(ICategoryDA categoryDA) { 
            _categoryDA = categoryDA;
        }
        public List<Category> getCategories()
        {
            return _categoryDA.getCategories();
        }

        public Category GetCategory(int id)
        {
            return _categoryDA.GetCategory(id);
        }
    }
}
