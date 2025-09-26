using Shopping_Web.DataAccess;
using Shopping_Web.Models;
using Shopping_Web.ViewModels;

namespace Shopping_Web.Services
{
    public class ProductService : IProductService
    {
        IProductDA _productDA;
        public ProductService(IProductDA productDA)
        {
            _productDA = productDA;
        }

        public void AddProduct(Product product)
        {
            _productDA.AddProduct(product);
        }

        //public List<Product> FilterByPrice(decimal? minPrice, decimal? maxPrice)
        //{
        //    return _productDA.FilterByPrice(minPrice, maxPrice);
        //}

        public List<Product> GetFeatureItems()
        {
            return _productDA.GetFeatureItems();
        }

        public List<Product> GetFilteredProducts(decimal? minPrice, decimal? maxPrice, string? search, int? cid)
        {
            return _productDA.GetFilteredProducts(minPrice, maxPrice, search, cid);
        }

        public List<Product> GetLatestItems()
        {
            return _productDA.GetLatestItems();
        }

        public Product GetProductById(int id)
        {
            return _productDA.GetProductById(id);
        }

        public void UpdateProduct(Product product)
        {
            _productDA.UpdateProduct(product);
        }

        public void UpdateQuantity(Product product, ProductVariant pv, int quantity, string sign)
        {
            _productDA.UpdateQuantity(product, pv, quantity, sign);
        }

        public List<Product> GetProducts()
        {
            return _productDA.GetProducts();
        }

        public List<BestSeller> getBestSellers()
        {
            return _productDA.getBestSellers();
        }

        public ProductVariant GetProductVariantById(int id)
        {
            return _productDA.GetProductVariantById(id);
        }
        //public List<Product>? GetProductsByCategory(int? cid = null)
        //{
        //    return _productDA.GetProductsByCategory(cid);
        //}

        //public List<Product> SearchList(string? search)
        //{
        //    return _productDA.SearchList(search);
        //}
    }
}
