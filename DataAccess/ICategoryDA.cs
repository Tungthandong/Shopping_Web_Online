using Shopping_Web.Models;

namespace Shopping_Web.DataAccess
{
    public interface ICategoryDA
    {
        List<Category> getCategories();
        Category GetCategory(int id);
    }
}
