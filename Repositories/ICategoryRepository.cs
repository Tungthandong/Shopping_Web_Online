using Shopping_Web.Models;

namespace Shopping_Web.Repositories
{
    public interface ICategoryRepository
    {
        List<Category> getCategories();
        Category GetCategory(int id);
    }
}
