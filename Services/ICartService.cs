using Shopping_Web.Models;

namespace Shopping_Web.Services
{
    public interface ICartService
    {
        string AddOrUpdateToCart(Cart c, int quantity);
        List<Cart> GetAllByUsername(string username);
        void Delete(Cart c);
        List<Cart> GetProductToCheckOut(string username, int[] selectedProducts);
    }
}
