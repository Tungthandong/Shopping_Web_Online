using Microsoft.EntityFrameworkCore;
using Shopping_Web.Models;

namespace Shopping_Web.DataAccess
{
    public class CartDA : ICartDA
    {
        YugiohCardShopContext context = new YugiohCardShopContext();
        public string AddOrUpdateToCart(Cart c, int quantity)
        {
            var product = context.Products.FirstOrDefault(p => p.ProductId == c.ProductId);
            if (product == null)
                return "Product not found";

            var proVar = context.ProductVariants.FirstOrDefault(p => p.VariantId == c.VariantId);
            if (proVar == null)
                return "Product variant not found";

            if (quantity == 0)
                return "Invalid quantity";

            var existing = context.Carts
                .FirstOrDefault(ca => ca.Username == c.Username
                                   && ca.ProductId == c.ProductId
                                   && ca.VariantId == c.VariantId);

            if (existing == null && quantity < 0)
                return "Product not in cart to decrease";

            int totalQuantity = (existing?.Quantity ?? 0) + quantity;

            if (totalQuantity < 1)
            {
                if (existing != null)
                {
                    context.Carts.Remove(existing);
                    context.SaveChanges();
                    return "Item removed from cart";
                }
                return "Invalid quantity";
            }

            if (totalQuantity > proVar.StockQuantity)
            {
                return $"Only {proVar.StockQuantity} item(s) available. You can add at most {proVar.StockQuantity - (existing?.Quantity ?? 0)} more.";
            }

            if (existing != null)
            {
                existing.Quantity = totalQuantity;
            }
            else
            {
                c.Quantity = quantity;
                context.Carts.Add(c);
            }

            context.SaveChanges();
            return "OK";
        }

        public void Delete(Cart c)
        {
            context.Remove(c);
            context.SaveChanges();
        }

        public List<Cart> GetAllByUsername(string username)
        {
            return context.Carts.Include(c => c.Product).Include(c => c.Variant).Where(c => c.Username == username).ToList();
        }
        public List<Cart> GetProductToCheckOut(string username, int[] selectedProducts)
        {
            return context.Carts.Include(c => c.Product).Include(c => c.Variant).Where(c => c.Username == username && selectedProducts.Contains(c.VariantId)).ToList();
        }
    }
}
