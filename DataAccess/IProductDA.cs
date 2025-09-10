using Shopping_Web.Models;
using Shopping_Web.ViewModels;

namespace Shopping_Web.DataAccess
{
    public interface IProductDA
    {
        List<Product> GetProducts();
        //List<Product>? GetProductsByCategory(int? cid = null);
        Product GetProductById(int id);
        List<Product> GetFeatureItems();
        List<Product> GetLatestItems();
        //List<Product> SearchList(string? search);
        //List<Product> FilterByPrice(decimal? minPrice, decimal? maxPrice);
        List<Product> GetFilteredProducts(decimal? minPrice, decimal? maxPrice, string? search, int? cid);
        void UpdateQuantity(Product product, int quantity, string sign);
        void UpdateProduct(Product product);
        void AddProduct(Product product);
        List<BestSeller> getBestSellers();
    }
}
