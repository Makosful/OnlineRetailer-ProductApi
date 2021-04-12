using System;
using Microsoft.AspNetCore.Mvc;
using ProductApi.Entities;
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
        ///     Adds a new product to the database.
        /// </summary>
        /// <param name="productInput"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult AddProduct([FromBody] ProductInput productInput)
        {
            if (productInput == null)
                return BadRequest("Body is null or malformed");
            if (string.IsNullOrWhiteSpace(productInput.Name))
                return BadRequest("Product name is null or empty");
            if (productInput.Price <= 0)
                return BadRequest("Product price is invalid");

            try
            {
                _productLogic.AddProduct(productInput);
                return Ok("Product created");
            }
            catch (ArgumentException e)
            {
                return BadRequest(e.Message);
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }

        /// <summary>
        ///     Gets a list of all products
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult GetAllProducts()
        {
            try
            {
                var products = _productLogic.GetAllProducts();

                return Ok(products);
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }

        /// <summary>
        ///     Gets a specific product
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id:int}")]
        public IActionResult GetProduct(int id)
        {
            if (id <= 0)
                return BadRequest("Product ID is invalid");

            try
            {
                var productInput = _productLogic.GetProduct(id);
                return Ok(productInput);
            }
            catch (ArgumentException e)
            {
                return BadRequest(e.Message);
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }

        /// <summary>
        ///     Updates the name of a product
        /// </summary>
        /// <param name="id"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        [HttpPut("{id:int}/name")]
        public IActionResult UpdateProductName(int id, [FromBody] string name)
        {
            if (id <= 0)
                return BadRequest("Product ID is invalid");
            if (string.IsNullOrWhiteSpace(name))
                return BadRequest("Product name is invalid");

            try
            {
                _productLogic.UpdateProductName(id, name);
                return Ok("Product name updated");
            }
            catch (ArgumentException e)
            {
                return BadRequest(e.Message);
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }

        /// <summary>
        ///     Updates the price of a product
        /// </summary>
        /// <param name="id"></param>
        /// <param name="price"></param>
        /// <returns></returns>
        [HttpPut("{id:int}/price")]
        public IActionResult UpdateProductPrice(int id, [FromBody] float price)
        {
            if (id <= 0)
                return BadRequest("Product ID is invalid");
            if (price <= 0)
                return BadRequest("Product price is invalid");

            try
            {
                _productLogic.UpdateProductPrice(id, price);
                return Ok("Updated product price");
            }
            catch (ArgumentException e)
            {
                return BadRequest(e.Message);
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }

        /// <summary>
        ///     Changes the amount of items in stock for a product.
        ///     A positive integer increases the stock.
        ///     A negative integer decreases the stock.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="amount"></param>
        /// <returns></returns>
        [HttpPut("{id:int}/stock")]
        public IActionResult UpdateProductStock(int id, [FromBody] int amount)
        {
            if (id <= 0)
                return BadRequest("Product ID is invalid");

            try
            {
                _productLogic.UpdateProductStock(id, amount);
                return Ok("Product stock updated");
            }
            catch (ArgumentException e)
            {
                return BadRequest(e.Message);
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }

        /// <summary>
        ///     Tries to reserve some product stock
        /// </summary>
        /// <param name="id"></param>
        /// <param name="amount"></param>
        /// <returns></returns>
        [HttpPost("{id:int}/reserve")]
        public IActionResult ReserveProduct(int id, [FromBody] int amount)
        {
            if (id <= 0)
                return BadRequest("Product ID is invalid");
            if (amount <= 0)
                return BadRequest("Reserve amount is invalid");

            try
            {
                _productLogic.ReserveProduct(id, amount);
                return Ok("Product reserved");
            }
            catch (ArgumentException e)
            {
                return BadRequest(e.Message);
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }

        /// <summary>
        ///     Frees up reserved products
        /// </summary>
        /// <param name="id"></param>
        /// <param name="amount"></param>
        /// <returns></returns>
        [HttpDelete("{id:int}/reserve")]
        public IActionResult ClearReservedProduct(int id, [FromBody] int amount)
        {
            if (id <= 0)
                return BadRequest("Product ID is invalid");
            if (amount <= 0)
                return BadRequest("Reserve amount is invalid");
            try
            {
                _productLogic.ClearReservedProduct(id, amount);
                return Ok("Product reserves cleared");
            }
            catch (ArgumentException e)
            {
                return BadRequest(e.Message);
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }

        /// <summary>
        ///     Removes a product from the database
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id:int}")]
        public IActionResult DeleteProduct(int id)
        {
            if (id <= 0)
                return BadRequest("Product ID is invalid");

            try
            {
                _productLogic.DeleteProduct(id);
                return Ok("Product has been deleted");
            }
            catch (ArgumentException e)
            {
                return BadRequest(e.Message);
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }
    }
}