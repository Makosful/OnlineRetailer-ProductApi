using System;
using System.Collections.Generic;
using Moq;
using ProductApi.Data.Abstractions;
using ProductApi.Logic;
using Xunit;

namespace ProductApi.Test.Logic.ProductLogicTests
{
    public class UpdateProductPrice
    {
        private readonly ProductLogic _productLogic;
        private readonly Mock<IProductRepository> _productRepository;

        public UpdateProductPrice()
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
        public void InputIsValid_ExpectsNothing()
        {
            // Arrange
            const int id = 1;
            const float price = 19.99f;

            // Act
            _productLogic.UpdateProductPrice(id, price);

            // Assert
            _productRepository.Verify(x =>
                x.UpdateProductPrice(id, price));
        }

        [Theory]
        [MemberData(nameof(GetInvalidIds))]
        public void ProductIdIsInvalid_ThrowArgumentException(int id)
        {
            // Arrange
            const float price = 19.99f;

            // Act
            Assert.Throws<ArgumentException>(() => _productLogic.UpdateProductPrice(id, price));

            // Assert
            _productRepository.Verify(x =>
                x.UpdateProductPrice(It.IsAny<int>(), It.IsAny<float>()), Times.Never);
        }

        [Fact]
        public void RepositoryThrowsArgumentException_ThrowArgumentException()
        {
            // Arrange
            const int id = 1;
            const float amount = 10f;
            _productRepository.Setup(x => x.UpdateProductPrice(It.IsAny<int>(), It.IsAny<float>()))
                .Throws<ArgumentException>();

            // Act
            Assert.Throws<ArgumentException>(() => _productLogic.UpdateProductPrice(id, amount));

            // Assert
            _productRepository.Verify(x =>
                x.UpdateProductPrice(id, amount), Times.Once);
        }

        [Fact]
        public void RepositoryThrowsGenericException_ThrowGenericException()
        {
            // Arrange
            const int id = 1;
            const float price = 10f;
            _productRepository.Setup(x => x.UpdateProductPrice(It.IsAny<int>(), It.IsAny<float>()))
                .Throws<Exception>();

            // Act
            Assert.Throws<Exception>(() => _productLogic.UpdateProductPrice(id, price));

            // Assert
            _productRepository.Verify(x =>
                x.UpdateProductPrice(id, price), Times.Once);
        }
    }
}