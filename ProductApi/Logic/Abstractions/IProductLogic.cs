using System;
using System.Collections.Generic;
using ProductApi.Entities;

namespace ProductApi.Logic.Abstractions
{
    public interface IProductLogic
    {
        /// <summary>
        ///     Adds a new product to the database
        /// </summary>
        /// <param name="productInput">
        ///     The product object to add
        /// </param>
        /// <exception cref="ArgumentException">
        ///     Argument Exception will be thrown if the user input is invalid
        /// </exception>
        /// <exception cref="Exception">
        ///     All server-side exceptions should be handled as a base Exception
        /// </exception>
        void AddProduct(ProductInput productInput);

        /// <summary>
        ///     Gets a list of all products.
        /// </summary>
        /// <returns>
        ///     Returns a collection of all <see cref="ProductInput" />s in the database
        /// </returns>
        /// <exception cref="Exception">
        ///     All server-side exceptions should be handled a base Exception
        /// </exception>
        IEnumerable<ProductOutput> GetAllProducts();

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
        ProductOutput GetProduct(int id);

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
        void UpdateProductName(int id, string name);

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
        void UpdateProductPrice(int id, float price);

        /// <summary>
        ///     Update the amount of available stock.
        ///     A negative amount can be specified to decrease stock.
        ///     See <see cref="ReserveProduct" /> for setting stock aside while
        ///     payment/shipping is being processed
        /// </summary>
        /// <param name="id"></param>
        /// <param name="amount"></param>
        /// <exception cref="ArgumentException">
        ///     All input based exceptions should be handled as ArgumentException
        /// </exception>
        /// <exception cref="Exception">
        ///     All server-side exceptions should be handled a base Exception
        /// </exception>
        void UpdateProductStock(int id, int amount);

        /// <summary>
        ///     Reserve some products to prevent overselling
        /// </summary>
        /// <param name="id">
        ///     The ID of the product
        /// </param>
        /// <param name="amount">
        ///     The amount of items to reserve
        /// </param>
        /// <exception cref="ArgumentException">
        ///     All input based exceptions should be handled as ArgumentException
        /// </exception>
        /// <exception cref="Exception">
        ///     All server-side exceptions should be handled a base Exception
        /// </exception>
        void ReserveProduct(int id, int amount);

        /// <summary>
        ///     Free's the reserved items and sets them as available.
        /// </summary>
        /// <param name="id">
        ///     The ID of the product
        /// </param>
        /// <param name="amount">
        ///     How many items of the product should be freed
        /// </param>
        /// <exception cref="ArgumentException">
        ///     All input based exceptions should be handled as ArgumentException
        /// </exception>
        /// <exception cref="Exception">
        ///     All server-side exceptions should be handled a base Exception
        /// </exception>
        void ClearReservedProduct(int id, int amount);

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
        void DeleteProduct(int id);
    }
}