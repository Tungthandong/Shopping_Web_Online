using Microsoft.EntityFrameworkCore;
using Shopping_Web.Models;

namespace Shopping_Web.DataAccess
{
    public class CategoryDA : ICategoryDA
    {
        private static YugiohCardShopContext context = new YugiohCardShopContext();
        public List<Category> getCategories()
        {
            return context.Categories.ToList();
        }

        public Category GetCategory(int id)
        {
            return context.Categories.FirstOrDefault(c => c.CategoryId == id);
        }
    }
}
