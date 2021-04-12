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
    public class UpdateProductNameTests
    {
        private readonly ProductsController _controller;
        private readonly Mock<IProductLogic> _productLogic;

        public UpdateProductNameTests()
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
        ///     Member Data for xUnit that returns a collection of valid IDs
        /// </summary>
        public static IEnumerable<object[]> GetValidIds()
        {
            yield return new object[] {1};
            yield return new object[] {10};
            yield return new object[] {int.MaxValue};
        }

        /// <summary>
        ///     Member Data for xUnit that returns a collection of invalid names
        /// </summary>
        public static IEnumerable<object[]> GetInvalidNames()
        {
            yield return new object[] {null};
            yield return new object[] {string.Empty};
            yield return new object[] {" "};
            yield return new object[] {"  "};
        }

        [Theory]
        [MemberData(nameof(GetInvalidIds))]
        public void IdIsInvalid_ExpectsStatus400(int id)
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
                x.UpdateProductName(It.IsAny<int>(), It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public void IdIsValid_ExpectsLogicCalled()
        {
            // Arrange
            const int id = 1;
            const string name = "Foobar";

            // Act
            var actionResult = _controller.UpdateProductName(id, name);
            var result = actionResult as ObjectResult;
            var value = result?.Value as string;

            // Assert
            result.Should().NotBeNull();
            result?.StatusCode.Should().NotBeNull().And.Be(200);
            value.Should().NotBeNull().And.Be("Product name updated");
            _productLogic.Verify(x =>
                x.UpdateProductName(id, name), Times.Once);
        }

        [Theory]
        [MemberData(nameof(GetInvalidNames))]
        public void ProductNameIsInvalid_ExpectsStatus400(string name)
        {
            // Arrange
            const int id = 1;

            // Act
            var actionResult = _controller.UpdateProductName(id, name);
            var result = actionResult as ObjectResult;
            var value = result?.Value as string;

            // Assert
            result.Should().NotBeNull();
            result?.StatusCode.Should().NotBeNull().And.Be(400);
            value.Should().NotBeNull().And.Be("Product name is invalid");
            _productLogic.Verify(x =>
                x.UpdateProductName(It.IsAny<int>(), It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public void ProductLogicThrowsArgumentException_ExpectsStatus400()
        {
            // Arrange
            const int id = 1;
            const string name = "Foobar";
            _productLogic.Setup(x => x.UpdateProductName(It.IsAny<int>(), It.IsAny<string>()))
                .Throws<ArgumentException>();

            // Act
            var actionResult = _controller.UpdateProductName(id, name);
            var result = actionResult as ObjectResult;
            var value = result?.Value as string;

            // Assert
            result.Should().NotBeNull();
            result?.StatusCode.Should().NotBeNull().And.Be(400);
            value.Should().NotBeNull();
            _productLogic.Verify(x =>
                x.UpdateProductName(id, name), Times.Once);
        }

        [Fact]
        public void ProductLogicThrowsGenericException_ExpectsStatus500()
        {
            // Arrange
            const int id = 1;
            const string name = "Foobar";
            _productLogic.Setup(x => x.UpdateProductName(It.IsAny<int>(), It.IsAny<string>()))
                .Throws<Exception>();

            // Act
            var actionResult = _controller.UpdateProductName(id, name);
            var result = actionResult as ObjectResult;
            var value = result?.Value as string;

            // Assert
            result.Should().NotBeNull();
            result?.StatusCode.Should().NotBeNull().And.Be(500);
            value.Should().NotBeNull();
            _productLogic.Verify(x =>
                x.UpdateProductName(id, name), Times.Once);
        }

        [Fact]
        public void InputIsValid_ExpectsStatus200()
        {
            // Arrange
            const int id = 1;
            const string name = "Foobar";

            // Act
            var actionResult = _controller.UpdateProductName(id, name);
            var result = actionResult as ObjectResult;
            var value = result?.Value as string;

            // Assert
            result.Should().NotBeNull();
            result?.StatusCode.Should().NotBeNull().And.Be(200);
            value.Should().NotBeNull().And.Be("Product name updated");
            _productLogic.Verify(x =>
                x.UpdateProductName(id, name), Times.Once);
        }
    }
}