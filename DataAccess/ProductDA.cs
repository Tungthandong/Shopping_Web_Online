using Microsoft.EntityFrameworkCore;
using Shopping_Web.Models;
using Shopping_Web.ViewModels;

namespace Shopping_Web.DataAccess
{
    public class ProductDA : IProductDA
    {
        private static YugiohCardShopContext context = new YugiohCardShopContext();

        //public List<Product> FilterByPrice(decimal? minPrice, decimal? maxPrice)
        //{
        //    var product = context.Products
        //        .Where(p =>
        //            (!minPrice.HasValue || p.UnitPrice >= minPrice.Value) &&
        //            (!maxPrice.HasValue || p.UnitPrice <= maxPrice.Value) &&
        //            p.ProductStatus == "active"
        //        )
        //        .OrderBy(p => p.UnitPrice).ToList();
        //    return product;
        //}
        public List<Product> GetFeatureItems()
        {
            var featureProducts = context.Products.Where(p => p.IsFeatured == true && p.ProductStatus.Equals("active")).Take(6).ToList();
            return featureProducts;
        }

        public List<Product> GetLatestItems()
        {
            var newProducts = context.Products.Where(p => p.ProductStatus.Equals("active")).OrderByDescending(p => p.ProductId).Take(9).ToList();
            return newProducts;
        }

        public Product GetProductById(int id)
        {
            return context.Products.Include(p => p.Category).FirstOrDefault(p => p.ProductId == id && p.ProductStatus.Equals("active"));
        }

        public List<Product> GetProducts()
        {
            return context.Products.Include(p => p.Category).OrderBy(p => p.UnitPrice).ToList();
        }

        //public List<Product>? GetProductsByCategory(int? cid = null)
        //{
        //    if (cid == null) return context.Products.ToList();
        //    return context.Products.Include(p => p.Category)
        //        .Where(p => p.Category.CategoryId == cid && p.ProductStatus.Equals("active"))
        //        .ToList();
        //}

        //public List<Product> SearchList(string? search)
        //{
        //    return context.Products.Where(p => p.ProductName.ToUpper().Contains(search.ToUpper())&& p.ProductStatus == "active").ToList();
        //}
        public List<Product> GetFilteredProducts(decimal? minPrice, decimal? maxPrice, string? search, int? cid)
        {
            var query = context.Products
                .Include(p => p.Category)
                .Where(p => p.ProductStatus == "active");

            if (!string.IsNullOrEmpty(search))
            {
                string keyword = search.ToUpper();
                query = query.Where(p => p.ProductName.ToUpper().Contains(keyword));
            }

            if (cid != null)
            {
                query = query.Where(p => p.Category.CategoryId == cid);
            }

            if (minPrice.HasValue)
            {
                query = query.Where(p => p.UnitPrice >= minPrice.Value);
            }

            if (maxPrice.HasValue)
            {
                query = query.Where(p => p.UnitPrice <= maxPrice.Value);
            }

            return query.OrderBy(p => p.UnitPrice).ToList();
        }

        public void UpdateQuantity(Product product, int quantity, string sign)
        {
            if (sign == "+")
            {
                product.UnitsInStock += quantity;
            }
            else
            {
                product.UnitsInStock -= quantity;
            }
            product.ProductStatus = product.UnitsInStock > 0 ? "active" : "inactive";
            context.SaveChanges();
        }

        public void UpdateProduct(Product product)
        {
            context.Products.Update(product);
            context.SaveChanges();
        }

        public void AddProduct(Product product)
        {
            context.Products.Add(product);
            context.SaveChanges();
        }

        public List<BestSeller> getBestSellers()
        {
            var bestSeller = context.OrderDetails
                            .Include(o => o.Order)
                            .Where(o => o.Order.OrderStatus=="Completed")
                            .GroupBy(od => od.ProductId)
                            .Select(g => new
                            {
                                ProductId = g.Key,
                                TotalSold = g.Sum(x => x.Quantity)
                            })
                            .Join(context.Products,
                                  g => g.ProductId,
                                  p => p.ProductId,
                                  (g, p) => new BestSeller{
                                      ProductId = p.ProductId, 
                                      ProductName = p.ProductName, 
                                      TotalSold = g.TotalSold,
                                      rate = g.TotalSold/p.UnitsInStock
                                  })
                            .ToList();
            return bestSeller;
        }
    }
}
