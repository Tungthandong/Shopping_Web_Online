using Shopping_Web.Models;
using Shopping_Web.Repositories;

namespace Shopping_Web.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _categoryRepository;

        public CategoryService(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }

        public List<Category> getCategories()
        {
            return _categoryRepository.getCategories();
        }

        public Category GetCategory(int id)
        {
            return _categoryRepository.GetCategory(id);
        }
    }
}
