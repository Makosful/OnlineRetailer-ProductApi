using System;
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

        public void AddProduct(ProductInput product)
        {
            if (product == null)
                throw new ArgumentException("Product is null", nameof(product));
            if (string.IsNullOrWhiteSpace(product.Name))
                throw new ArgumentException($"Product name is invalid [{product.Name}]", nameof(product));
            if (product.Price <= 0)
                throw new ArgumentException($"Product price is invalid [{product.Price}]", nameof(product));

            _productRepository.AddProduct(product);
        }

        public IEnumerable<ProductOutput> GetAllProducts()
        {
            return _productRepository.GetAllProducts();
        }

        public ProductOutput GetProduct(int id)
        {
            if (id <= 0)
                throw new ArgumentException($"Product ID is invalid: [{id}]", nameof(id));

            return _productRepository.GetProduct(id);
        }

        public void UpdateProductName(int id, string name)
        {
            if (id <= 0)
                throw new ArgumentException($"Product ID is invalid: [{id}]", nameof(id));
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException($"Product name is invalid: [{name}]", nameof(name));

            _productRepository.UpdateProductName(id, name);
        }

        public void UpdateProductPrice(int id, float price)
        {
            if (id <= 0)
                throw new ArgumentException($"Product ID is invalid: [{id}]", nameof(id));

            _productRepository.UpdateProductPrice(id, price);
        }

        public void UpdateProductStock(int id, int amount)
        {
            if (id <= 0)
                throw new ArgumentException($"Product ID is invalid: [{id}]", nameof(id));
            _productRepository.UpdateProductStock(id, amount);
        }

        public void ReserveProduct(int id, int amount)
        {
            if (id <= 0)
                throw new ArgumentException($"Product ID is invalid: [{id}]", nameof(id));
            if (amount <= 0)
                throw new ArgumentException($"Reserve amount is invalid: [{amount}]");
            
            _productRepository.ReserveProduct(id, amount);
        }

        public void ClearReservedProduct(int id, int amount)
        {
            if (id <= 0)
                throw new ArgumentException($"Product ID is invalid: [{id}]", nameof(id));
            if (amount <= 0)
                throw new ArgumentException($"Reserve amount is invalid: [{amount}]");
            _productRepository.ClearReservedProducts(id, amount);
        }

        public void DeleteProduct(int id)
        {
            if (id <= 0)
                throw new ArgumentException($"Product ID is invalid: [{id}]", nameof(id));

            _productRepository.DeleteProduct(id);
        }
    }
}