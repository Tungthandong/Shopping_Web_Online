using Microsoft.EntityFrameworkCore;
using Shopping_Web.Data;
using Shopping_Web.Models;
using Shopping_Web.ViewModels;

namespace Shopping_Web.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly YugiohCardShopContext _context;

        public ProductRepository(YugiohCardShopContext context)
        {
            _context = context;
        }

        public List<Product> GetFeatureItems()
        {
            return _context.Products
                .Where(p => p.IsFeatured == true && p.ProductStatus == "active")
                .Take(6)
                .ToList();
        }

        public List<Product> GetLatestItems()
        {
            return _context.Products
                .Where(p => p.ProductStatus == "active")
                .OrderByDescending(p => p.ProductId)
                .Take(9)
                .ToList();
        }

        public Product GetProductById(int id)
        {
            return _context.Products
                .Include(p => p.Category)
                .Include(p => p.ProductVariants)
                .FirstOrDefault(p => p.ProductId == id && p.ProductStatus == "active");
        }

        public List<Product> GetProducts()
        {
            return _context.Products
                .Include(p => p.Category)
                .OrderBy(p => p.UnitPrice)
                .ToList();
        }

        public List<Product> GetFilteredProducts(decimal? minPrice, decimal? maxPrice, string? search, int? cid)
        {
            var query = _context.Products
                .Include(p => p.Category)
                .Where(p => p.ProductStatus == "active");

            if (!string.IsNullOrWhiteSpace(search))
            {
                string keyword = search.ToUpper();
                query = query.Where(p => p.ProductName.ToUpper().Contains(keyword));
            }

            if (cid != null)
                query = query.Where(p => p.Category.CategoryId == cid);

            if (minPrice.HasValue)
                query = query.Where(p => p.UnitPrice >= minPrice.Value);

            if (maxPrice.HasValue)
                query = query.Where(p => p.UnitPrice <= maxPrice.Value);

            return query.OrderBy(p => p.UnitPrice).ToList();
        }

        public void UpdateQuantity(Product product, ProductVariant pv, int quantity, string sign)
        {
            if (sign == "+")
            {
                product.UnitsInStock += quantity;
                pv.StockQuantity += quantity;
            }
            else
            {
                product.UnitsInStock -= quantity;
                pv.StockQuantity -= quantity;
            }
            product.ProductStatus = product.UnitsInStock > 0 ? "active" : "inactive";
            _context.SaveChanges();
        }

        public void UpdateProduct(Product product)
        {
            _context.Products.Update(product);
            _context.SaveChanges();
        }

        public void AddProduct(Product product)
        {
            _context.Products.Add(product);
            _context.SaveChanges();
        }

        public List<BestSeller> getBestSellers()
        {
            return _context.OrderDetails
                .Include(o => o.Order)
                .Where(o => o.Order.OrderStatus == "Completed")
                .GroupBy(od => od.ProductId)
                .Select(g => new { ProductId = g.Key, TotalSold = g.Sum(x => x.Quantity) })
                .Join(_context.Products,
                    g => g.ProductId,
                    p => p.ProductId,
                    (g, p) => new BestSeller
                    {
                        ProductId   = p.ProductId,
                        ProductName = p.ProductName,
                        TotalSold   = g.TotalSold,
                        rate        = p.UnitsInStock > 0 ? g.TotalSold / p.UnitsInStock : 0
                    })
                .ToList();
        }

        public ProductVariant GetProductVariantById(int id)
        {
            return _context.ProductVariants.FirstOrDefault(p => p.VariantId == id);
        }
    }
}
