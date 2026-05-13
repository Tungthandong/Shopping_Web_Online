using Shopping_Web.Models;
using Shopping_Web.Repositories;
using Shopping_Web.ViewModels;

namespace Shopping_Web.Services
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;

        public ProductService(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        public void AddProduct(Product product)
        {
            _productRepository.AddProduct(product);
        }

        public List<Product> GetFeatureItems()
        {
            return _productRepository.GetFeatureItems();
        }

        public List<Product> GetFilteredProducts(decimal? minPrice, decimal? maxPrice, string? search, int? cid)
        {
            return _productRepository.GetFilteredProducts(minPrice, maxPrice, search, cid);
        }

        public List<Product> GetLatestItems()
        {
            return _productRepository.GetLatestItems();
        }

        public Product GetProductById(int id)
        {
            return _productRepository.GetProductById(id);
        }

        public void UpdateProduct(Product product)
        {
            _productRepository.UpdateProduct(product);
        }

        public void UpdateQuantity(Product product, ProductVariant pv, int quantity, string sign)
        {
            _productRepository.UpdateQuantity(product, pv, quantity, sign);
        }

        public List<Product> GetProducts()
        {
            return _productRepository.GetProducts();
        }

        public List<BestSeller> getBestSellers()
        {
            return _productRepository.getBestSellers();
        }

        public ProductVariant GetProductVariantById(int id)
        {
            return _productRepository.GetProductVariantById(id);
        }
    }
}
