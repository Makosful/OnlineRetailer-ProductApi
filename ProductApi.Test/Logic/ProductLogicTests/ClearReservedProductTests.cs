using System;
using System.Collections.Generic;
using Moq;
using ProductApi.Data.Abstractions;
using ProductApi.Logic;
using Xunit;

namespace ProductApi.Test.Logic.ProductLogicTests
{
    public class ClearReservedProduct
    {
        private readonly ProductLogic _productLogic;
        private readonly Mock<IProductRepository> _productRepository;

        public ClearReservedProduct()
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

        public static IEnumerable<object[]> GetInvalidReserves()
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
            const int amount = 10;

            // Act
            _productLogic.ClearReservedProduct(id, amount);

            // Assert
            _productRepository.Verify(x =>
                x.ClearReservedProducts(id, amount), Times.Once);
        }

        [Theory]
        [MemberData(nameof(GetInvalidIds))]
        public void ProductIdIsInvalid_ThrowsArgumentException(int id)
        {
            // Arrange
            const int amount = 10;
            _productRepository.Setup(x => x.ClearReservedProducts(It.IsAny<int>(), It.IsAny<int>()))
                .Throws<ArgumentException>();

            // Act
            Assert.Throws<ArgumentException>(() => _productLogic.ClearReservedProduct(id, amount));

            // Arrange
            _productRepository.Verify(x =>
                x.ClearReservedProducts(It.IsAny<int>(), It.IsAny<int>()), Times.Never);
        }

        [Theory]
        [MemberData(nameof(GetInvalidReserves))]
        public void ReserveAmountIsInvalid_ThrowArgumentException(int amount)
        {
            // Arrange
            const int id = 1;

            // Act
            Assert.Throws<ArgumentException>(() => _productLogic.ClearReservedProduct(id, amount));

            // Assert
            _productRepository.Verify(x =>
                x.ClearReservedProducts(id, amount), Times.Never);
        }

        [Fact]
        public void RepositoryThrowsArgumentException_ThrowArgumentException()
        {
            // Arrange
            const int id = 1;
            const int amount = 10;
            _productRepository.Setup(x => x.ClearReservedProducts(It.IsAny<int>(), It.IsAny<int>()))
                .Throws<ArgumentException>();

            // Act
            Assert.Throws<ArgumentException>(() => _productLogic.ClearReservedProduct(id, amount));

            // Assert
            _productRepository.Verify(x =>
                x.ClearReservedProducts(id, amount), Times.Once);
        }

        [Fact]
        public void RepositoryThrowsGenericException_ThrowGenericException()
        {
            // Arrange
            const int id = 1;
            const int amount = 10;
            _productRepository.Setup(x => x.ClearReservedProducts(It.IsAny<int>(), It.IsAny<int>()))
                .Throws<Exception>();

            // Act
            Assert.Throws<Exception>(() => _productLogic.ClearReservedProduct(id, amount));

            // Assert
            _productRepository.Verify(x =>
                x.ClearReservedProducts(id, amount), Times.Once);
        }
    }
}