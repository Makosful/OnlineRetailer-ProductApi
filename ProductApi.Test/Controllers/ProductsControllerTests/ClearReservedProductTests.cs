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
    public class ClearReservedProductTests
    {
        private readonly ProductsController _controller;
        private readonly Mock<IProductLogic> _productLogic;

        public ClearReservedProductTests()
        {
            _productLogic = new Mock<IProductLogic>();

            _controller = new ProductsController(
                _productLogic.Object);
        }

        /// <summary>
        ///     Member Data for xUnit that returns a collection of invalid IDs
        /// </summary>
        public static IEnumerable<object[]> GetInvalids()
        {
            yield return new object[] {0};
            yield return new object[] {-1};
            yield return new object[] {int.MinValue};
        }

        public static IEnumerable<object[]> GetInvalidReserves()
        {
            yield return new object[] {0};
            yield return new object[] {-1};
            yield return new object[] {-10};
            yield return new object[] {int.MinValue};
        }

        [Theory]
        [MemberData(nameof(GetInvalids))]
        public void ProductIdIsInvalid_ExpectsStatus400(int id)
        {
            // Arrange
            const int amount = 10;

            // Act
            var actionResult = _controller.ClearReservedProduct(id, amount);
            var result = actionResult as ObjectResult;
            var value = result?.Value as string;

            // Assert
            result.Should().NotBeNull();
            result?.StatusCode.Should().NotBeNull().And.Be(400);
            value.Should().NotBeNull().And.Be("Product ID is invalid");
            _productLogic.Verify(x =>
                x.ClearReservedProduct(id, amount), Times.Never);
        }

        [Theory]
        [MemberData(nameof(GetInvalidReserves))]
        public void ReservesAmountIsInvalid_ExpectsStatus400(int amount)
        {
            // Arrange
            const int id = 1;

            // Act
            var actionResult = _controller.ClearReservedProduct(id, amount);
            var result = actionResult as ObjectResult;
            var value = result?.Value as string;

            // Assert
            result.Should().NotBeNull();
            result?.StatusCode.Should().NotBeNull().And.Be(400);
            value.Should().BeEquivalentTo("Reserve amount is invalid");
        }

        [Fact]
        public void InputIsValid_ExpectsStatus200()
        {
            // Arrange
            const int id = 1;
            const int amount = 10;

            // Act
            var actionResult = _controller.ClearReservedProduct(id, amount);
            var result = actionResult as ObjectResult;
            var value = result?.Value as string;

            // Assert
            result.Should().NotBeNull();
            result?.StatusCode.Should().NotBeNull().And.Be(200);
            value.Should().NotBeNull().And.Be("Product reserves cleared");
            _productLogic.Verify(x =>
                x.ClearReservedProduct(id, amount), Times.Once);
        }

        [Fact]
        public void ProductRepositoryThrowsArgumentException_ExpectsStatus400()
        {
            // Arrange
            const int id = 1;
            const int amount = 10;
            _productLogic.Setup(x => x.ClearReservedProduct(It.IsAny<int>(), It.IsAny<int>()))
                .Throws<ArgumentException>();

            // Act
            var actionResult = _controller.ClearReservedProduct(id, amount);
            var result = actionResult as ObjectResult;
            var value = result?.Value as string;

            // Assert
            result.Should().NotBeNull();
            result?.StatusCode.Should().NotBeNull().And.Be(400);
            value.Should().NotBeNull();
            _productLogic.Verify(x =>
                x.ClearReservedProduct(It.IsAny<int>(), It.IsAny<int>()), Times.Once);
        }

        [Fact]
        public void ProductRepositoryThrowsGenericException_ExpectsStatus500()
        {
            // Arrange
            const int id = 1;
            const int amount = 10;
            _productLogic.Setup(x => x.ClearReservedProduct(It.IsAny<int>(), It.IsAny<int>()))
                .Throws<Exception>();

            // Act
            var actionResult = _controller.ClearReservedProduct(id, amount);
            var result = actionResult as ObjectResult;
            var value = result?.Value as string;

            // Assert
            result.Should().NotBeNull();
            result?.StatusCode.Should().NotBeNull().And.Be(500);
            value.Should().NotBeNull();
            _productLogic.Verify(x =>
                x.ClearReservedProduct(It.IsAny<int>(), It.IsAny<int>()), Times.Once);
        }
    }
}