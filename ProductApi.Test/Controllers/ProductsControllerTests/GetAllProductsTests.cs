using System;
using System.Collections.Generic;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using ProductApi.Controllers;
using ProductApi.Entities;
using ProductApi.Logic.Abstractions;
using Xunit;

namespace ProductApi.Test.Controllers.ProductsControllerTests
{
    public class GetAllProductsTests
    {
        private readonly ProductsController _controller;
        private readonly Mock<IProductLogic> _productLogic;

        public GetAllProductsTests()
        {
            _productLogic = new Mock<IProductLogic>();

            _controller = new ProductsController(
                _productLogic.Object);
        }

        [Fact]
        public void ProductRepositoryThrowsGenericException_ExpectsStatus500()
        {
            // Arrange
            _productLogic.Setup(x => x.GetAllProducts())
                .Throws<Exception>();

            // Act
            var actionResult = _controller.GetAllProducts();
            var result = actionResult as ObjectResult;
            var value = result?.Value;

            // Assert
            result.Should().NotBeNull();
            result?.StatusCode.Should().NotBeNull().And.Be(500);
            value?.Should().NotBeNull();
            _productLogic.Verify(x =>
                x.GetAllProducts(), Times.Once);
        }

        [Fact]
        public void ProductRepositoryReturnsList_ExpectsList()
        {
            // Arrange
            List<ProductOutput> outputs = new()
            {
                new ProductOutput {Id = 1, Name = "Foo", Price = 1.00f, ItemsReserved = 10, ItemsInStock = 100},
                new ProductOutput {Id = 1, Name = "Bar", Price = 2.00f, ItemsReserved = 15, ItemsInStock = 110},
                new ProductOutput {Id = 1, Name = "Yaa", Price = 3.00f, ItemsReserved = 20, ItemsInStock = 120}
            };
            _productLogic.Setup(x => x.GetAllProducts())
                .Returns(() => outputs);

            // Act
            var actionResult = _controller.GetAllProducts();
            var result = actionResult as ObjectResult;
            var value = result?.Value;

            // Assert
            result.Should().NotBeNull();
            result?.StatusCode.Should().NotBeNull().And.Be(200);
            value.Should().NotBeNull().And.BeEquivalentTo(outputs);
        }
    }
}