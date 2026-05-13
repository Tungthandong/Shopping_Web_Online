using Shopping_Web.Data;
using Shopping_Web.Models;

namespace Shopping_Web.Repositories
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly YugiohCardShopContext _context;

        public CategoryRepository(YugiohCardShopContext context)
        {
            _context = context;
        }

        public List<Category> getCategories()
        {
            return _context.Categories.ToList();
        }

        public Category GetCategory(int id)
        {
            return _context.Categories.FirstOrDefault(c => c.CategoryId == id);
        }
    }
}
