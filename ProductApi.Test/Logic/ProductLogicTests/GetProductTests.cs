using System;
using System.Collections.Generic;
using FluentAssertions;
using Moq;
using ProductApi.Data.Abstractions;
using ProductApi.Entities;
using ProductApi.Logic;
using Xunit;

namespace ProductApi.Test.Logic.ProductLogicTests
{
    public class GetProduct
    {
        private readonly ProductLogic _productLogic;
        private readonly Mock<IProductRepository> _productRepository;

        public GetProduct()
        {
            _productRepository = new Mock<IProductRepository>();

            _productLogic = new ProductLogic(
                _productRepository.Object);
        }

        /// <summary>
        ///     Member Data for xUnit that gets a set of invalid product IDs
        /// </summary>
        public static IEnumerable<object[]> GetInvalidIds()
        {
            yield return new object[] {0};
            yield return new object[] {-1};
            yield return new object[] {-10};
            yield return new object[] {int.MinValue};
        }

        [Fact]
        public void InputIsValid_ExpectProduct()
        {
            // Arrange
            const int id = 1;
            const string name = "Foobar";
            const float price = 19.99f;
            const int reserved = 10;
            const int stock = 100;
            var output = new ProductOutput
            {
                Id = id,
                Name = name,
                Price = price,
                ItemsReserved = reserved,
                ItemsInStock = stock
            };
            _productRepository.Setup(x => x.GetProduct(It.IsAny<int>()))
                .Returns<int>(y => new ProductOutput
                {
                    Id = y,
                    Name = name,
                    Price = price,
                    ItemsReserved = reserved,
                    ItemsInStock = stock
                });

            // Act
            var product = _productLogic.GetProduct(id);

            // Assert
            _productRepository.Verify(x =>
                x.GetProduct(id), Times.Once);
            product.Should().BeEquivalentTo(output);
        }

        [Theory]
        [MemberData(nameof(GetInvalidIds))]
        public void ProductIdIsInvalid_ThrowArgumentException(int id)
        {
            // Arrange
            // Nothing special

            // Act
            Assert.Throws<ArgumentException>(() => _productLogic.GetProduct(id));

            // Assert
            _productRepository.Verify(x =>
                x.GetProduct(It.IsAny<int>()), Times.Never);
        }

        [Fact]
        public void RepositoryThrowsArgumentException_ThrowArgumentException()
        {
            // Arrange
            const int id = 1;
            _productRepository.Setup(x => x.GetProduct(It.IsAny<int>()))
                .Throws<ArgumentException>();

            // Act
            Assert.Throws<ArgumentException>(() => _productLogic.GetProduct(id));

            // Arrange
            _productRepository.Verify(x =>
                x.GetProduct(id), Times.Once);
        }

        [Fact]
        public void RepositoryThrowsGenericException_ThrowGenericException()
        {
            // Arrange
            const int id = 1;
            _productRepository.Setup(x => x.GetProduct(It.IsAny<int>()))
                .Throws<Exception>();

            // Act
            Assert.Throws<Exception>(() => _productLogic.GetProduct(id));

            // Arrange
            _productRepository.Verify(x =>
                x.GetProduct(id), Times.Once);
        }
    }
}