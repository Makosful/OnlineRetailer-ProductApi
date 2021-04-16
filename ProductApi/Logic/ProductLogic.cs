using System.Collections.Generic;
using ProductApi.Data.Abstractions;
using ProductApi.Entities;
using ProductApi.Logic.Abstractions;

namespace ProductApi.Logic
{
    public class ProductLogic : IProductLogic
    {
        private readonly IProductRepository _productRepository;

        public ProductLogic(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        public void AddProduct(ProductInput productInput)
        {
            throw new System.NotImplementedException();
        }

        public IEnumerable<ProductOutput> GetAllProducts()
        {
            throw new System.NotImplementedException();
        }

        public ProductOutput GetProduct(int id)
        {
            throw new System.NotImplementedException();
        }

        public void UpdateProductName(int id, string name)
        {
            throw new System.NotImplementedException();
        }

        public void UpdateProductPrice(int id, float price)
        {
            throw new System.NotImplementedException();
        }

        public void UpdateProductStock(int id, int amount)
        {
            throw new System.NotImplementedException();
        }

        public void ReserveProduct(int id, int amount)
        {
            throw new System.NotImplementedException();
        }

        public void ClearReservedProduct(int id, int amount)
        {
            throw new System.NotImplementedException();
        }

        public void DeleteProduct(int id)
        {
            throw new System.NotImplementedException();
        }
    }
}