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
    public class AddProductTests
    {
        private readonly ProductsController _controller;
        private readonly Mock<IProductLogic> _productLogic;

        public AddProductTests()
        {
            _productLogic = new Mock<IProductLogic>();

            _controller = new ProductsController(
                _productLogic.Object);
        }
        
        /// <summary>
        ///     Member Data for xUnit that gets basic empty strings
        /// </summary>
        public static IEnumerable<object[]> GetEmptyStrings()
        {
            yield return new object[] {null};
            yield return new object[] {string.Empty};
            yield return new object[] {" "};
            yield return new object[] {"  "};
        }

        /// <summary>
        ///     Member Data for xUnit that gets a set of invalid product prices
        /// </summary>
        public static IEnumerable<object[]> GetInvalidPrices()
        {
            yield return new object[] {0.00f};
            yield return new object[] {-0.01f};
            yield return new object[] {-1.00f};
            yield return new object[] {float.MinValue};
        }

        [Fact]
        public void ProductIsNull_ExpectsStatus400()
        {
            // Arrange
            // Nothing specific 

            // Act
            var actionResult = _controller.AddProduct(null);
            var result = actionResult as ObjectResult;
            var value = result?.Value as string;

            // Assert
            result.Should().NotBeNull();
            result?.StatusCode.Should().Be(400);
            value.Should().NotBeNull().And.Be("Body is null or malformed");
            _productLogic.Verify(x =>
                x.AddProduct(It.IsAny<ProductInput>()), Times.Never);
        }

        [Theory]
        [MemberData(nameof(GetEmptyStrings))]
        public void ProductName_IsNullOrEmpty_ExpectsStatus400(string name)
        {
            // Arrange
            var productInput = new ProductInput
            {
                Name = name,
                Price = 0.00f
            };

            // Act
            var actionResult = _controller.AddProduct(productInput);
            var result = actionResult as ObjectResult;
            var value = result?.Value as string;

            // Assert
            result.Should().NotBeNull();
            result?.StatusCode.Should().Be(400);
            value.Should().NotBeNull().And.Be("Product name is null or empty");
            _productLogic.Verify(x =>
                x.AddProduct(It.IsAny<ProductInput>()), Times.Never);
        }

        [Theory]
        [MemberData(nameof(GetInvalidPrices))]
        public void ProductPrice_IsNegative_ExpectsStatus400(float price)
        {
            // Arrange
            var productInput = new ProductInput
            {
                Name = "Foobar",
                Price = price
            };

            // Act
            var actionResult = _controller.AddProduct(productInput);
            var result = actionResult as ObjectResult;
            var value = result?.Value as string;

            // Assert
            result.Should().NotBeNull();
            result?.StatusCode.Should().Be(400);
            value.Should().NotBeNull().And.Be("Product price is invalid");
            _productLogic.Verify(x =>
                x.AddProduct(It.IsAny<ProductInput>()), Times.Never);
        }

        [Fact]
        public void Product_IsValid_ExpectsStatus200()
        {
            // Arrange
            var productInput = new ProductInput
            {
                Name = "Foobar",
                Price = 420.96f
            };

            // Act
            var actionResult = _controller.AddProduct(productInput);
            var result = actionResult as ObjectResult;
            var value = result?.Value as string;

            // Assert
            result.Should().NotBeNull();
            result?.StatusCode.Should().Be(200);
            value.Should().NotBeNull().And.Be("Product created");
            _productLogic.Verify(x =>
                x.AddProduct(productInput), Times.Once);
        }

        [Fact]
        public void ProductRepository_ThrowsArgumentException_ExpectsStatus400()
        {
            // Arrange
            _productLogic.Setup(x => x.AddProduct(It.IsAny<ProductInput>()))
                .Throws<ArgumentException>();
            var productInput = new ProductInput
            {
                Name = "Foobar",
                Price = 420.96f
            };

            // Act
            var actionResult = _controller.AddProduct(productInput);
            var result = actionResult as ObjectResult;
            var value = result?.Value as string;

            //Assert
            result.Should().NotBeNull();
            result?.StatusCode.Should().NotBeNull().And.Be(400);
            value.Should().NotBeNull();
            _productLogic.Verify(x =>
                x.AddProduct(productInput), Times.Once);
        }

        [Fact]
        public void ProductRepository_ThrowsGenericException_ExpectsStatus500()
        {
            // Arrange
            _productLogic.Setup(x => x.AddProduct(It.IsAny<ProductInput>()))
                .Throws<Exception>();
            var productInput = new ProductInput
            {
                Name = "Foobar",
                Price = 420.96f
            };

            // Act
            var actionResult = _controller.AddProduct(productInput);
            var result = actionResult as ObjectResult;
            var value = result?.Value as string;

            //Assert
            result.Should().NotBeNull();
            result?.StatusCode.Should().NotBeNull().And.Be(500);
            value.Should().NotBeNull();
            _productLogic.Verify(x =>
                x.AddProduct(productInput), Times.Once);
        }
    }
}