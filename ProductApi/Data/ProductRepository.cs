using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using ProductApi.Data.Abstractions;
using ProductApi.Data.Database;
using ProductApi.Entities;

namespace ProductApi.Data
{
    public class ProductRepository : IProductRepository, IDisposable
    {
        private readonly ProductContext _context;

        public ProductRepository(ProductContext context)
        {
            _context = context;
        }

        void IDisposable.Dispose()
        {
            _context?.Dispose();
            GC.SuppressFinalize(this);
        }

        /// <summary>
        ///     Adds a new product to the database
        /// </summary>
        /// <param name="input">
        ///     The product object to add
        /// </param>
        /// <exception cref="ArgumentException">
        ///     Argument Exception will be thrown if the user input is invalid
        /// </exception>
        /// <exception cref="Exception">
        ///     All server-side exceptions should be handled as a base Exception
        /// </exception>
        public void AddProduct(ProductInput input)
        {
            _context.Database.EnsureCreated();
            
            if (input == null)
                throw new ArgumentException("Product is null");
            if (string.IsNullOrWhiteSpace(input.Name))
                throw new ArgumentException($"Product Name is invalid: [{input.Name}]");
            if (input.Price <= 0)
                throw new ArgumentException($"Product Price is invalid: [{input.Price}]");

            var output = new ProductOutput
            {
                Name = input.Name,
                Price = input.Price,
            };

            try
            {
                _context.Products.Add(output);
                _context.SaveChanges();
            }
            catch (DbUpdateException e)
            {
                var innerException = e.InnerException;
                if (innerException is SqliteException)
                {
                    if (innerException.Message.StartsWith("SQLite Error 19"))
                        throw new ArgumentException($"Product with name [{input.Name}] already exists");
                    throw innerException;
                }
                throw new ArgumentException(e.Message);
            }
        }

        /// <summary>
        ///     Removes a product from the database
        /// </summary>
        /// <param name="id">
        ///     The ID of the product to delete
        /// </param>
        /// <exception cref="ArgumentException">
        ///     All input based exceptions should be handled as ArgumentException
        /// </exception>
        /// <exception cref="Exception">
        ///     All server-side exceptions should be handled a base Exception
        /// </exception>
        public void DeleteProduct(int id)
        {
            _context.Database.EnsureCreated();

            if (id <= 0)
                throw new ArgumentException($"Product ID is invalid: [{id}]");

            IQueryable<ProductOutput> products = from product in _context.Products
                where product.Id == id
                select product;
            if (!products.Any())
                throw new ArgumentException($"Product ID does not exist: [{id}]");
            ProductOutput first = products.First();
            _context.Products.Remove(first);
            _context.SaveChanges();
        }

        /// <summary>
        ///     Gets a list of all products.
        /// </summary>
        /// <returns>
        ///     Returns a collection of all <see cref="ProductInput" />s in the database
        /// </returns>
        /// <exception cref="Exception">
        ///     All server-side exceptions should be handled a base Exception
        /// </exception>
        public IEnumerable<ProductOutput> GetAllProducts()
        {
            _context.Database.EnsureCreated();
            
            try
            {
                var queryable = from products in _context.Products
                    select products;

                return queryable.ToList();
            }
            catch (SqliteException ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        /// <summary>
        ///     Get a specific product
        /// </summary>
        /// <param name="id">
        ///     The ID of the product to get
        /// </param>
        /// <returns>
        ///     Returns a single <see cref="ProductInput" /> if it's found.
        ///     Otherwise, see exceptions
        /// </returns>
        /// <exception cref="ArgumentException">
        ///     All input based exceptions should be handled as ArgumentException
        /// </exception>
        /// <exception cref="Exception">
        ///     All server-side exceptions should be handled a base Exception
        /// </exception>
        public ProductOutput GetProduct(int id)
        {
            _context.Database.EnsureCreated();
            
            if (id <= 0)
                throw new ArgumentException($"Product ID is invalid: [{id}]");
            var queryable = from product in _context.Products
                where product.Id == id
                select product;

            if (!queryable.Any())
                throw new ArgumentException($"Product ID does not exist: [{id}]");
            return queryable.First();
        }

        /// <summary>
        ///     Update the name of a product
        /// </summary>
        /// <param name="id">
        ///     The ID of the product
        /// </param>
        /// <param name="name">
        ///     The new name of the product
        /// </param>
        /// <exception cref="ArgumentException">
        ///     All input based exceptions should be handled as ArgumentException
        /// </exception>
        /// <exception cref="Exception">
        ///     All server-side exceptions should be handled a base Exception
        /// </exception>
        public void UpdateProductName(int id, string name)
        {
            _context.Database.EnsureCreated();
            if (id <= 0)
                throw new ArgumentException($"Product ID is invalid: [{id}]");
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException($"Product Name is invalid: [{name}]");

            try
            {
                var queryable = from products in _context.Products
                    where products.Name == name
                    select products;
                if (queryable.Any())
                    throw new ArgumentException($"Product Name already exists: [{name}]");
            }
            catch (Exception e)
            {
                throw new ArgumentException(e.Message);
            }

            try
            {
                var queryable = from product in _context.Products
                    where product.Id == id
                    select product;

                if (!queryable.Any())
                    throw new ArgumentException($"Product ID does not exist: [{id}]");
                
                var first = queryable.First();
                first.Name = name;
                _context.Products.Update(first);
                _context.SaveChanges();
            }
            catch (Exception e)
            {
                var innerException = e.InnerException;
                if (innerException is not SqliteException) 
                    throw new ArgumentException(e.Message);

                throw new ArgumentException(e.Message);
            }
        }

        /// <summary>
        ///     Update the price for a product
        /// </summary>
        /// <param name="id">
        ///     The ID of the product
        /// </param>
        /// <param name="price">
        ///     The new price for the product
        /// </param>
        /// <exception cref="ArgumentException">
        ///     All input based exceptions should be handled as ArgumentException
        /// </exception>
        /// <exception cref="Exception">
        ///     All server-side exceptions should be handled a base Exception
        /// </exception>
        public void UpdateProductPrice(int id, float price)
        {
            _context.Database.EnsureCreated();
            
            if (id <= 0)
                throw new ArgumentException($"Product ID is invalid: [{id}]");
            if (price <= 0)
                throw new ArgumentException($"Product Price is invalid: [{price}]");

            try
            {
                var queryable = from product in _context.Products
                    where product.Id == id
                    select product;

                if (!queryable.Any())
                    throw new ArgumentException($"Product ID does not exist: [{id}]");
                var first = queryable.First();

                first.Price = price;
                _context.SaveChanges();
            }
            catch (Exception e)
            {
                if (e is SqliteException)
                    throw new ArgumentException(e.Message);
                throw;
            }
        }

        /// <summary>
        ///     Update the amount of available stock.
        ///     A negative amount can be specified to decrease stock.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="amount"></param>
        /// <exception cref="ArgumentException">
        ///     All input based exceptions should be handled as ArgumentException
        /// </exception>
        /// <exception cref="Exception">
        ///     All server-side exceptions should be handled a base Exception
        /// </exception>
        public void UpdateProductStock(int id, int amount)
        {
            _context.Database.EnsureCreated();
            
            if (id <= 0)
                throw new ArgumentException($"Product ID is invalid: [{id}]");

            try
            {
                var queryable = from product in _context.Products
                    where product.Id == id
                    select product;
                var actual = queryable.FirstOrDefault();
                if (actual == null)
                    throw new ArgumentException($"Product ID does not exist: [{id}]");

                var availableStock = actual.ItemsInStock - actual.ItemsReserved;
                if (availableStock + amount < 0)
                    throw new ArgumentException("");
                if (actual.ItemsInStock + amount < 0)
                    throw new ArgumentException("");
                

                actual.ItemsInStock += amount;
                _context.SaveChanges();
            }
            catch (Exception e)
            {
                throw;
            }
        }

        /// <summary>
        ///     Reserve some products to prevent overselling.
        ///     A negative amount can be specified to free reserved stock
        /// </summary>
        /// <param name="id"></param>
        /// <param name="amount"></param>
        /// <exception cref="ArgumentException">
        ///     All input based exceptions should be handled as ArgumentException
        /// </exception>
        /// <exception cref="Exception">
        ///     All server-side exceptions should be handled a base Exception
        /// </exception>
        public void ReserveProduct(int id, int amount)
        {
            _context.Database.EnsureCreated();
            
            if (id <= 0)
                throw new ArgumentException($"Product ID is invalid: [{id}]");
            try
            {
                var queryable = from product in _context.Products
                    where product.Id == id
                    select product;
                var first = queryable.FirstOrDefault();
                if (first == null)
                    throw new ArgumentException($"Product ID does not exist: [{id}]");

                var availableStock = first.ItemsInStock - first.ItemsReserved;
                if (availableStock - amount < 0)
                    throw new ArgumentException("");
                if (first.ItemsReserved + amount < 0)
                    throw new ArgumentException("");
                
                first.ItemsReserved += amount;
                _context.SaveChanges();
            }
            catch (Exception e)
            {
                throw;
            }
        }

        /// <summary>
        ///     Frees reserved stock by removing the specified amount from both
        ///     stock and reserve.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="amount"></param>
        /// <exception cref="ArgumentException">
        ///     All input based exceptions should be handled as ArgumentException
        /// </exception>
        /// <exception cref="Exception">
        ///     All server-side exceptions should be handled a base Exception
        /// </exception>
        public void ClearReservedProducts(int id, int amount)
        {
            _context.Database.EnsureCreated();

            if (id <= 0)
                throw new ArgumentException($"Product ID is invalid: [{id}]");
            if (amount < 0)
                throw new ArgumentException($"Clear amount must be greater than 0: [{amount}]");

            try
            {
                var queryable = from product in _context.Products
                    where product.Id == id
                    select product;
                
                if (!queryable.Any())
                    throw new ArgumentException($"Product ID does not exist: [{id}]");
                var first = queryable.First();

                int reserved = first.ItemsReserved;
                int stock = first.ItemsInStock;
                if (reserved - amount < 0 || stock - amount < 0)
                    throw new ArgumentException("");
                
                first.ItemsReserved -= amount;
                first.ItemsInStock-= amount;
                _context.SaveChanges();
            }
            catch (Exception e)
            {
                throw;
            }
        }
    }
}