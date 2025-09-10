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
            if (product == null) return "Product not found";

            var existing = context.Carts
                .FirstOrDefault(ca => ca.Username == c.Username && ca.ProductId == c.ProductId);

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

            if (totalQuantity > product.UnitsInStock)
            {
                return $"Only {product.UnitsInStock} item(s) available. You can add at most {product.UnitsInStock - (existing?.Quantity ?? 0)} more.";
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
            return context.Carts.Include(c => c.Product).Where(c => c.Username == username).ToList();
        }
        public List<Cart> GetProductToCheckOut(string username, int[] selectedProducts)
        {
            return context.Carts.Include(c => c.Product).Where(c => c.Username == username&&selectedProducts.Contains(c.Product.ProductId)).ToList();
        }
    }
}
