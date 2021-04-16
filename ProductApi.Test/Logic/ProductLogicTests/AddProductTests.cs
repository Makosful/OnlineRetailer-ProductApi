using System;
using System.Collections.Generic;
using Moq;
using ProductApi.Data.Abstractions;
using ProductApi.Entities;
using ProductApi.Logic;
using Xunit;

namespace ProductApi.Test.Logic.ProductLogicTests
{
    public class AddProductTests
    {
        private readonly ProductLogic _logic;
        private readonly Mock<IProductRepository> _productRepository;

        public AddProductTests()
        {
            _productRepository = new Mock<IProductRepository>();

            _logic = new ProductLogic(
                _productRepository.Object);
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
        public void RepositorySuccessfullyCalled_ExpectsNothing()
        {
            // Arrange
            var input = new ProductInput
            {
                Name = "Foobar",
                Price = 19.99f
            };

            // Act
            _logic.AddProduct(input);

            // Asset
            _productRepository.Verify(x =>
                x.AddProduct(input), Times.Once);
        }

        [Fact]
        public void InputIsNull_ThrowArgumentException()
        {
            // Arrange
            // Nothing special

            // Act
            Assert.Throws<ArgumentException>(() => _logic.AddProduct(null));

            // Assert
            _productRepository.Verify(x =>
                x.AddProduct(It.IsAny<ProductInput>()), Times.Never);
        }

        [Theory]
        [MemberData(nameof(GetEmptyStrings))]
        public void ProductNameIsInvalid_ThrowArgumentException(string name)
        {
            // Arrange
            var input = new ProductInput
            {
                Name = name,
                Price = 19.99f
            };

            // Act
            Assert.Throws<ArgumentException>(() => _logic.AddProduct(input));

            // Assert
            _productRepository.Verify(x =>
                x.AddProduct(It.IsAny<ProductInput>()), Times.Never);
        }

        [Theory]
        [MemberData(nameof(GetInvalidPrices))]
        public void ProductPriceIsInvalid_ThrowArgumentException(float price)
        {
            // Arrange
            var input = new ProductInput
            {
                Name = "Foobar",
                Price = price
            };

            // Act
            Assert.Throws<ArgumentException>(() => _logic.AddProduct(input));

            // Assert
            _productRepository.Verify(x =>
                x.AddProduct(It.IsAny<ProductInput>()), Times.Never);
        }

        [Fact]
        public void RepositoryThrowsAArgumentException_ThrowArgumentException()
        {
            // Arrange
            var input = new ProductInput
            {
                Name = "Foobar",
                Price = 19.99f
            };
            _productRepository.Setup(x => x.AddProduct(It.IsAny<ProductInput>()))
                .Throws<ArgumentException>();

            // Act
            Assert.Throws<ArgumentException>(() => _logic.AddProduct(input));

            // Assert
            _productRepository.Verify(x => x.AddProduct(input), Times.Once);
        }
    }
}