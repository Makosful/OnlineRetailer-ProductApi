using System;
using System.Collections.Generic;
using ProductApi.Entities;

namespace ProductApi.Data.Abstractions
{
    public interface IProductRepository
    {
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
        public void AddProduct(ProductInput input);

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
        public void DeleteProduct(int id);

        /// <summary>
        ///     Gets a list of all products.
        /// </summary>
        /// <returns>
        ///     Returns a collection of all <see cref="ProductInput" />s in the database
        /// </returns>
        /// <exception cref="Exception">
        ///     All server-side exceptions should be handled a base Exception
        /// </exception>
        public IEnumerable<ProductOutput> GetAllProducts();

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
        public ProductOutput GetProduct(int id);

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
        public void UpdateProductName(int id, string name);

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
        public void UpdateProductPrice(int id, float price);

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
        public void UpdateProductStock(int id, int amount);

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
        public void ReserveProduct(int id, int amount);

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
        public void ClearReservedProducts(int id, int amount);
    }
}