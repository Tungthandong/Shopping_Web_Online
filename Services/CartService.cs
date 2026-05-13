using Shopping_Web.Models;
using Shopping_Web.Repositories;

namespace Shopping_Web.Services
{
    public class CartService : ICartService
    {
        private readonly ICartRepository _cartRepository;

        public CartService(ICartRepository cartRepository)
        {
            _cartRepository = cartRepository;
        }

        public string AddOrUpdateToCart(Cart c, int quantity)
        {
            return _cartRepository.AddOrUpdateToCart(c, quantity);
        }

        public void Delete(Cart c)
        {
            _cartRepository.Delete(c);
        }

        public List<Cart> GetAllByUsername(string username)
        {
            return _cartRepository.GetAllByUsername(username);
        }

        public List<Cart> GetProductToCheckOut(string username, int[] selectedProducts)
        {
            return _cartRepository.GetProductToCheckOut(username, selectedProducts);
        }
    }
}
