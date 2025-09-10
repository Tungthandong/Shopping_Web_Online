using Shopping_Web.DataAccess;
using Shopping_Web.Models;

namespace Shopping_Web.Services
{
    public class CartService : ICartService
    {
        ICartDA _cartDA;
        public CartService(ICartDA cartDA)
        {
            _cartDA = cartDA;
        }
        public string AddOrUpdateToCart(Cart c, int quantity)
        {
            return _cartDA.AddOrUpdateToCart(c, quantity);
        }

        public void Delete(Cart c)
        {
            _cartDA.Delete(c);
        }

        public List<Cart> GetAllByUsername(string username)
        {
            return _cartDA.GetAllByUsername(username);
        }

        public List<Cart> GetProductToCheckOut(string username, int[] selectedProducts)
        {
            return _cartDA.GetProductToCheckOut(username, selectedProducts);
        }
    }
}
