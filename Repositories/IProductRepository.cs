using Shopping_Web.Models;
using Shopping_Web.ViewModels;

namespace Shopping_Web.Repositories
{
    public interface IProductRepository
    {
        List<Product> GetProducts();
        Product GetProductById(int id);
        ProductVariant GetProductVariantById(int id);
        List<Product> GetFeatureItems();
        List<Product> GetLatestItems();
        List<Product> GetFilteredProducts(decimal? minPrice, decimal? maxPrice, string? search, int? cid);
        void UpdateQuantity(Product product, ProductVariant pv, int quantity, string sign);
        void UpdateProduct(Product product);
        void AddProduct(Product product);
        List<BestSeller> getBestSellers();
    }
}
