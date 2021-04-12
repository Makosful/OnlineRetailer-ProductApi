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
    public class GetProductTests
    {
        private readonly ProductsController _controller;
        private readonly Mock<IProductLogic> _productLogic;

        public GetProductTests()
        {
            _productLogic = new Mock<IProductLogic>();

            _controller = new ProductsController(
                _productLogic.Object);
        }

        /// <summary>
        ///     Member Data for xUnit that returns invalid IDs
        /// </summary>
        public static IEnumerable<object[]> GetInvalidIds()
        {
            yield return new object[] {0};
            yield return new object[] {-1};
            yield return new object[] {int.MinValue};
        }

        /// <summary>
        ///     Member Data for xUnit that returns valid IDs
        /// </summary>
        public static IEnumerable<object[]> GetValidIds()
        {
            yield return new object[] {1};
            yield return new object[] {10};
        }

        [Theory]
        [MemberData(nameof(GetInvalidIds))]
        public void IdIsNegative_ExpectsStatus400(int id)
        {
            // Arrange
            // Nothing specific 

            // Act
            var actionResult = _controller.GetProduct(id);
            var result = actionResult as ObjectResult;
            var value = result?.Value as string;

            // Assert
            result.Should().NotBeNull();
            result?.StatusCode.Should().NotBeNull().And.Be(400);
            value.Should().NotBeNull().And.Be("Product ID is invalid");
            _productLogic.Verify(x =>
                x.GetProduct(It.IsAny<int>()), Times.Never);
        }

        [Theory]
        [MemberData(nameof(GetValidIds))]
        public void IdIsValid_ExpectsSingleProduct(int id)
        {
            // Arrange
            const string name = "Foobar";
            const float price = 19.99f;
            const int reserved = 10;
            const int stock = 100;
            var productOutput = new ProductOutput
            {
                Id = id,
                Name = name,
                Price = price,
                ItemsReserved = reserved,
                ItemsInStock = stock
            };
            _productLogic.Setup(x => x.GetProduct(id))
                .Returns<int>(i => new ProductOutput
                {
                    Id = i,
                    Name = name,
                    Price = price,
                    ItemsReserved = reserved,
                    ItemsInStock = stock
                });

            // Act
            var actionResult = _controller.GetProduct(id);
            var result = actionResult as ObjectResult;
            var value = result?.Value;

            // Assert
            result.Should().NotBeNull();
            result?.StatusCode.Should().NotBeNull().And.Be(200);
            value.Should().NotBeNull().And.BeEquivalentTo(productOutput);
            _productLogic.Verify(x =>
                x.GetProduct(id), Times.Once);
        }

        [Fact]
        public void ProductRepositoryThrowsArgumentException_ExpectsStatus400()
        {
            // Arrange
            const int id = 100;
            _productLogic.Setup(x => x.GetProduct(It.IsAny<int>()))
                .Throws<ArgumentException>();

            // Act
            var actionResult = _controller.GetProduct(id);
            var result = actionResult as ObjectResult;
            var value = result?.Value as string;

            // Assert
            result.Should().NotBeNull();
            result?.StatusCode.Should().NotBeNull().And.Be(400);
            value.Should().NotBeNull();
            _productLogic.Verify(x =>
                x.GetProduct(id), Times.Once);
        }

        [Fact]
        public void ProductRepositoryThrowsGenericException_ExpectsStatus500()
        {
            // Arrange
            const int id = 100;
            _productLogic.Setup(x => x.GetProduct(It.IsAny<int>()))
                .Throws<Exception>();

            // Act
            var actionResult = _controller.GetProduct(id);
            var result = actionResult as ObjectResult;
            var value = result?.Value as string;

            // Assert
            result.Should().NotBeNull();
            result?.StatusCode.Should().NotBeNull().And.Be(500);
            value.Should().NotBeNull();
            _productLogic.Verify(x =>
                x.GetProduct(id), Times.Once);
        }
    }
}