using System;
using System.Collections.Generic;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using ProductApi.Controllers;
using ProductApi.Logic.Abstractions;
using Xunit;

namespace ProductApi.Test.Controllers.ProductsControllerTests
{
    public class UpdateProductPriceTests
    {
        private readonly ProductsController _controller;
        private readonly Mock<IProductLogic> _productLogic;

        public UpdateProductPriceTests()
        {
            _productLogic = new Mock<IProductLogic>();

            _controller = new ProductsController(
                _productLogic.Object);
        }

        /// <summary>
        ///     Member Data for xUnit that returns a collection of invalid IDs
        /// </summary>
        public static IEnumerable<object[]> GetInvalidIds()
        {
            yield return new object[] {0};
            yield return new object[] {-1};
            yield return new object[] {int.MinValue};
        }

        /// <summary>
        ///     Member Data for xUnit that returns a collection of invalid prices
        /// </summary>
        public static IEnumerable<object[]> GetInvalidPrices()
        {
            yield return new object[] {-1.00f};
            yield return new object[] {-10f};
            yield return new object[] {float.MinValue};
        }

        [Theory]
        [MemberData(nameof(GetInvalidIds))]
        public void ProductIdIsInvalid_ExpectsStatus400(int id)
        {
            // Arrange
            const float price = 10f;

            // Act
            var actionResult = _controller.UpdateProductPrice(id, price);
            var result = actionResult as ObjectResult;
            var value = result?.Value as string;

            // Assert
            result.Should().NotBeNull();
            result?.StatusCode.Should().NotBeNull().And.Be(400);
            value.Should().NotBeNull().And.Be("Product ID is invalid");
            _productLogic.Verify(x =>
                x.UpdateProductPrice(It.IsAny<int>(), It.IsAny<float>()), Times.Never);
        }

        [Theory]
        [MemberData(nameof(GetInvalidPrices))]
        public void ProductPriceIsInvalid_ExpectsStatus400(float price)
        {
            // Arrange
            const int id = 1;

            // Act
            var actionResult = _controller.UpdateProductPrice(id, price);
            var result = actionResult as ObjectResult;
            var value = result?.Value as string;

            // Assert
            result.Should().NotBeNull();
            result?.StatusCode.Should().NotBeNull().And.Be(400);
            value.Should().NotBeNull().And.Be("Product price is invalid");
            _productLogic.Verify(x =>
                x.UpdateProductPrice(It.IsAny<int>(), It.IsAny<float>()), Times.Never);
        }

        [Fact]
        public void InputIsValid_ExpectStatus200()
        {
            // Arrange
            const int id = 1;
            const float price = 10f;

            // Act
            var actionResult = _controller.UpdateProductPrice(id, price);
            var result = actionResult as ObjectResult;
            var value = result?.Value as string;

            // Assert
            result.Should().NotBeNull();
            result?.StatusCode.Should().NotBeNull().And.Be(200);
            value.Should().NotBeNull().And.Be("Updated product price");
            _productLogic.Verify(x =>
                x.UpdateProductPrice(id, price), Times.Once);
        }

        [Fact]
        public void ProductLogicThrowsArgumentException_ExpectsStatus400()
        {
            // Arrange
            const int id = 1;
            const float price = 10f;
            _productLogic.Setup(x => x.UpdateProductPrice(It.IsAny<int>(), It.IsAny<float>()))
                .Throws<ArgumentException>();

            // Act
            var actionResult = _controller.UpdateProductPrice(id, price);
            var result = actionResult as ObjectResult;
            var value = result?.Value as string;

            // Assert
            result.Should().NotBeNull();
            result?.StatusCode.Should().NotBeNull().And.Be(400);
            value.Should().NotBeNull();
            _productLogic.Verify(x =>
                x.UpdateProductPrice(id, price), Times.Once);
        }

        [Fact]
        public void ProductLogicThrowsGenericException_ExpectsStatus500()
        {
            // Arrange
            const int id = 1;
            const float price = 10f;
            _productLogic.Setup(x => x.UpdateProductPrice(It.IsAny<int>(), It.IsAny<float>()))
                .Throws<Exception>();

            // Act
            var actionResult = _controller.UpdateProductPrice(id, price);
            var result = actionResult as ObjectResult;
            var value = result?.Value as string;

            // Assert
            result.Should().NotBeNull();
            result?.StatusCode.Should().NotBeNull().And.Be(500);
            value.Should().NotBeNull();
            _productLogic.Verify(x =>
                x.UpdateProductPrice(id, price), Times.Once);
        }
    }
}