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
    public class DeleteProductTests
    {
        private readonly ProductsController _controller;
        private readonly Mock<IProductLogic> _productLogic;

        public DeleteProductTests()
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

        [Theory]
        [MemberData(nameof(GetInvalidIds))]
        public void ProductIdIsInvalid_ExpectsStatus400(int id)
        {
            // Arrange
            // Nothing specific

            // Act
            var actionResult = _controller.DeleteProduct(id);
            var result = actionResult as ObjectResult;
            var value = result?.Value as string;

            // Assert
            result.Should().NotBeNull();
            result?.StatusCode.Should().NotBeNull().And.Be(400);
            value.Should().NotBeNull().And.Be("Product ID is invalid");
            _productLogic.Verify(x =>
                x.DeleteProduct(It.IsAny<int>()), Times.Never());
        }

        [Fact]
        public void InputIsValid_ExpectsStatus200()
        {
            // Arrange
            const int id = 1;

            // Act
            var actionResult = _controller.DeleteProduct(id);
            var result = actionResult as ObjectResult;
            var value = result?.Value as string;

            // Assert
            result.Should().NotBeNull();
            result?.StatusCode.Should().NotBeNull().And.Be(200);
            value.Should().NotBeNull().And.Be("Product has been deleted");
            _productLogic.Verify(x =>
                x.DeleteProduct(id), Times.Once());
        }

        [Fact]
        public void ProductRepositoryThrowsArgumentException_ExpectsStatus400()
        {
            // Arrange
            const int id = 1;
            _productLogic.Setup(x => x.DeleteProduct(It.IsAny<int>()))
                .Throws<ArgumentException>();

            // Act
            var actionResult = _controller.DeleteProduct(id);
            var result = actionResult as ObjectResult;
            var value = result?.Value as string;

            // Assert
            result.Should().NotBeNull();
            result?.StatusCode.Should().NotBeNull().And.Be(400);
            value.Should().NotBeNull();
            _productLogic.Verify(x =>
                x.DeleteProduct(id), Times.Once());
        }

        [Fact]
        public void ProductRepositoryThrowsGenericException_ExpectsStatus500()
        {
            // Arrange
            const int id = 1;
            _productLogic.Setup(x => x.DeleteProduct(It.IsAny<int>()))
                .Throws<Exception>();

            // Act
            var actionResult = _controller.DeleteProduct(id);
            var result = actionResult as ObjectResult;
            var value = result?.Value as string;

            // Assert
            result.Should().NotBeNull();
            result?.StatusCode.Should().NotBeNull().And.Be(500);
            value.Should().NotBeNull();
            _productLogic.Verify(x =>
                x.DeleteProduct(id), Times.Once());
        }
    }
}