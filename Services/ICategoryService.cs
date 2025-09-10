using Shopping_Web.Models;

namespace Shopping_Web.Services
{
    public interface ICategoryService
    {
        List<Category> getCategories();
        Category GetCategory(int id);
    }
}
