using System;
using System.Collections.Generic;
using Moq;
using ProductApi.Data.Abstractions;
using ProductApi.Logic;
using Xunit;

namespace ProductApi.Test.Logic.ProductLogicTests
{
    public class DeleteProduct
    {
        private readonly ProductLogic _productLogic;
        private readonly Mock<IProductRepository> _productRepository;

        public DeleteProduct()
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

            // Act
            _productLogic.DeleteProduct(id);

            // Assert
            _productRepository.Verify(x =>
                x.DeleteProduct(id), Times.Once);
        }

        [Theory]
        [MemberData(nameof(GetInvalidIds))]
        public void ProductIdIsInvalid_ThrowArgumentException(int id)
        {
            // Arrange
            // Nothing specific

            // Act
            Assert.Throws<ArgumentException>(() => _productLogic.DeleteProduct(id));

            // Assert
            _productRepository.Verify(x =>
                x.DeleteProduct(It.IsAny<int>()), Times.Never);
        }

        [Fact]
        public void RepositoryThrowsArgumentException_ThrowArgumentException()
        {
            // Arrange
            const int id = 1;
            _productRepository.Setup(x => x.DeleteProduct(It.IsAny<int>()))
                .Throws<ArgumentException>();

            // Act
            Assert.Throws<ArgumentException>(() => _productLogic.DeleteProduct(id));

            // Assert
            _productRepository.Verify(x =>
                x.DeleteProduct(id), Times.Once);
        }

        [Fact]
        public void RepositoryThrowsGenericException_ThrowGenericException()
        {
            // Arrange
            const int id = 1;
            _productRepository.Setup(x => x.DeleteProduct(It.IsAny<int>()))
                .Throws<Exception>();

            // Act
            Assert.Throws<Exception>(() => _productLogic.DeleteProduct(id));

            // Assert
            _productRepository.Verify(x =>
                x.DeleteProduct(id), Times.Once);
        }
    }
}