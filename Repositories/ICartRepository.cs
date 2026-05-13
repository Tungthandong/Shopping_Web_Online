using Shopping_Web.Models;

namespace Shopping_Web.Repositories
{
    public interface ICartRepository
    {
        string AddOrUpdateToCart(Cart c, int quantity);
        List<Cart> GetAllByUsername(string username);
        void Delete(Cart c);
        List<Cart> GetProductToCheckOut(string username, int[] selectedProducts);
    }
}
