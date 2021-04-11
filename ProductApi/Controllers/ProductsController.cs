using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using ProductApi.Entities;
using ProductApi.Logic;
using ProductApi.Logic.Abstractions;

namespace ProductApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly IProductLogic _productLogic;

        public ProductsController(IProductLogic productLogic)
        {
            _productLogic = productLogic;
        }

        /// <summary>
        /// Adds a new product to the database.
        /// </summary>
        /// <param name="productInput"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult AddProduct([FromBody] ProductInput productInput)
        {
            _productRepository.AddProduct(productInput);
            return Ok();
        }

        /// <summary>
        /// Gets a list of all products 
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult GetAllProducts()
        {
            IEnumerable<ProductOutput> products = _productRepository.GetAllProducts();

            return Ok();
        }

        /// <summary>
        /// Gets a specific product
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id:int}")]
        public IActionResult GetProduct(int id)
        {
            ProductOutput productInput = _productRepository.GetProduct(id);
            return Ok(productInput);
        }

        /// <summary>
        /// Updates the name of a product
        /// </summary>
        /// <param name="id"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        [HttpPut("{id:int}/name")]
        public IActionResult UpdateProductName(int id, [FromBody] string name)
        {
            _productRepository.UpdateProductName(id, name);
            return Ok();
        }
        
        /// <summary>
        /// Updates the price of a product
        /// </summary>
        /// <param name="id"></param>
        /// <param name="price"></param>
        /// <returns></returns>
        [HttpPut("{id:int}/price")]
        public IActionResult UpdateProductPrice(int id, [FromBody] decimal price)
        {
            _productRepository.UpdateProductPrice(id, price);
            return Ok();
        }

        /// <summary>
        /// Changes the amount of items in stock for a product.
        /// A positive integer increases the stock.
        /// A negative integer decreases the stock. 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="amount"></param>
        /// <returns></returns>
        [HttpPut("{id:int}/stock")]
        public IActionResult UpdateProductStock(int id, [FromBody] int amount)
        {
            _productRepository.UpdateProductStock(id, amount);
            return Ok();
        }
        
        /// <summary>
        /// Tries to reserve some product stock 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="amount"></param>
        /// <returns></returns>
        [HttpPost("{id:int}/reserve")]
        public IActionResult ReserveProduct(int id, [FromBody] int amount)
        {
            _productRepository.ReserveProduct(id, amount);
            return Ok();
        }
        
        /// <summary>
        /// Frees up reserved products 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="amount"></param>
        /// <returns></returns>
        [HttpDelete("{id:int}/reserve")]
        public IActionResult ClearReservedProduct(int id, [FromBody] int amount)
        {
            _productRepository.ClearReservedProduct(id, amount);
            return Ok();
        }

        /// <summary>
        /// Removes a product from the database
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id:int}")]
        public IActionResult DeleteProduct(int id)
        {
            _productRepository.DeleteProduct(id);
            return Ok();
        }
    }
}