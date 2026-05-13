using Microsoft.EntityFrameworkCore;
using Shopping_Web.Data;
using Shopping_Web.Models;

namespace Shopping_Web.Repositories
{
    public class CartRepository : ICartRepository
    {
        private readonly YugiohCardShopContext _context;

        public CartRepository(YugiohCardShopContext context)
        {
            _context = context;
        }

        public string AddOrUpdateToCart(Cart c, int quantity)
        {
            var product = _context.Products.FirstOrDefault(p => p.ProductId == c.ProductId);
            if (product == null) return "Product not found";

            var proVar = _context.ProductVariants.FirstOrDefault(p => p.VariantId == c.VariantId);
            if (proVar == null) return "Product variant not found";

            if (quantity == 0) return "Invalid quantity";

            var existing = _context.Carts
                .FirstOrDefault(ca => ca.Username == c.Username
                                   && ca.ProductId == c.ProductId
                                   && ca.VariantId == c.VariantId);

            if (existing == null && quantity < 0) return "Product not in cart to decrease";

            int totalQuantity = (existing?.Quantity ?? 0) + quantity;

            if (totalQuantity < 1)
            {
                if (existing != null)
                {
                    _context.Carts.Remove(existing);
                    _context.SaveChanges();
                    return "Item removed from cart";
                }
                return "Invalid quantity";
            }

            if (totalQuantity > proVar.StockQuantity)
                return $"Only {proVar.StockQuantity} item(s) available. You can add at most {proVar.StockQuantity - (existing?.Quantity ?? 0)} more.";

            if (existing != null)
                existing.Quantity = totalQuantity;
            else
            {
                c.Quantity = quantity;
                _context.Carts.Add(c);
            }

            _context.SaveChanges();
            return "OK";
        }

        public void Delete(Cart c)
        {
            var existing = _context.Carts
                .FirstOrDefault(x => x.Username == c.Username
                                  && x.ProductId == c.ProductId
                                  && x.VariantId == c.VariantId);

            if (existing != null)
            {
                _context.Carts.Remove(existing);
                _context.SaveChanges();
            }
        }

        public List<Cart> GetAllByUsername(string username)
        {
            return _context.Carts
                .Include(c => c.Product)
                .Include(c => c.Variant)
                .Where(c => c.Username == username)
                .ToList();
        }

        public List<Cart> GetProductToCheckOut(string username, int[] selectedProducts)
        {
            return _context.Carts
                .Include(c => c.Product)
                .Include(c => c.Variant)
                .Where(c => c.Username == username && selectedProducts.Contains(c.VariantId))
                .ToList();
        }
    }
}
