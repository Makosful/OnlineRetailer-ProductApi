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
    public class UpdateProductStockTests
    {
        private readonly ProductsController _controller;
        private readonly Mock<IProductLogic> _productLogic;

        public UpdateProductStockTests()
        {
            _productLogic = new Mock<IProductLogic>();

            _controller = new ProductsController(
                _productLogic.Object);
        }

        /// <summary>
        ///     Member Data fpr xUnit to get a collection of invalid IDs
        /// </summary>
        public static IEnumerable<object[]> GetInvalidIds()
        {
            yield return new object[] {0};
            yield return new object[] {-1};
            yield return new object[] {int.MinValue};
        }

        [Theory]
        [MemberData(nameof(GetInvalidIds))]
        public void ProductIdIsInvalid_ExpectsStatus400(int id)
        {
            // Arrange
            const int amount = 10;

            // Act
            var actionResult = _controller.UpdateProductStock(id, amount);
            var result = actionResult as ObjectResult;
            var value = result?.Value as string;

            // Assert
            result.Should().NotBeNull();
            result?.StatusCode.Should().NotBeNull().And.Be(400);
            value.Should().NotBeNull().And.Be("Product ID is invalid");
            _productLogic.Verify(x =>
                x.UpdateProductStock(It.IsAny<int>(), It.IsAny<int>()), Times.Never);
        }

        [Fact]
        public void InputIsValid_ExpectsStatus200()
        {
            // Arrange
            const int id = 1;
            const int amount = 10;

            // Act
            var actionResult = _controller.UpdateProductStock(id, amount);
            var result = actionResult as ObjectResult;
            var value = result?.Value as string;

            // Assert
            result.Should().NotBeNull();
            result?.StatusCode.Should().NotBeNull().And.Be(200);
            value.Should().NotBeNull().And.Be("Product stock updated");
            _productLogic.Verify(x =>
                x.UpdateProductStock(id, amount), Times.Once);
        }

        [Fact]
        public void ProductLogicThrowsArgumentException_ExpectsStatus400()
        {
            // Arrange
            const int id = 1;
            const int amount = 10;
            _productLogic.Setup(x => x.UpdateProductStock(It.IsAny<int>(), It.IsAny<int>()))
                .Throws<ArgumentException>();

            // Act
            var actionResult = _controller.UpdateProductStock(id, amount);
            var result = actionResult as ObjectResult;
            var value = result?.Value as string;

            // Assert
            result.Should().NotBeNull();
            result?.StatusCode.Should().NotBeNull().And.Be(400);
            value.Should().NotBeNull();
            _productLogic.Verify(x =>
                x.UpdateProductStock(id, amount), Times.Once);
        }

        [Fact]
        public void ProductLogicThrowsGenericException_ExpectsStatus400()
        {
            // Arrange
            const int id = 1;
            const int amount = 10;
            _productLogic.Setup(x => x.UpdateProductStock(It.IsAny<int>(), It.IsAny<int>()))
                .Throws<Exception>();

            // Act
            var actionResult = _controller.UpdateProductStock(id, amount);
            var result = actionResult as ObjectResult;
            var value = result?.Value as string;

            // Assert
            result.Should().NotBeNull();
            result?.StatusCode.Should().NotBeNull().And.Be(500);
            value.Should().NotBeNull();
            _productLogic.Verify(x =>
                x.UpdateProductStock(id, amount), Times.Once);
        }
    }
}