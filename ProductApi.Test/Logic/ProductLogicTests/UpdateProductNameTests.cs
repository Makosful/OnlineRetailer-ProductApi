using System;
using System.Collections.Generic;
using Moq;
using ProductApi.Data.Abstractions;
using ProductApi.Logic;
using Xunit;

namespace ProductApi.Test.Logic.ProductLogicTests
{
    public class UpdateProductName
    {
        private readonly ProductLogic _productLogic;
        private readonly Mock<IProductRepository> _productRepository;

        public UpdateProductName()
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

        /// <summary>
        ///     Member Data for xUnit that gets a set of invalid product names
        /// </summary>
        public static IEnumerable<object[]> GetInvalidNames()
        {
            yield return new object[] {null};
            yield return new object[] {string.Empty};
            yield return new object[] {" "};
            yield return new object[] {"  "};
        }

        [Fact]
        public void InputIsValid_ExpectsNothing()
        {
            // Arrange
            const int id = 1;
            const string name = "Foobar";

            // Act
            _productLogic.UpdateProductName(id, name);

            // Assert
            _productRepository.Verify(x =>
                x.UpdateProductName(id, name), Times.Once);
        }

        [Theory]
        [MemberData(nameof(GetInvalidIds))]
        public void ProductIdIsInvalid_ThrowArgumentException(int id)
        {
            // Arrange
            const string name = "Foobar";

            // Act
            Assert.Throws<ArgumentException>(() => _productLogic.UpdateProductName(id, name));

            // Assert
            _productRepository.Verify(x =>
                x.UpdateProductName(It.IsAny<int>(), It.IsAny<string>()), Times.Never);
        }

        [Theory]
        [MemberData(nameof(GetInvalidNames))]
        public void ProductNameIsInvalid_ThrowArgumentException(string name)
        {
            // Arrange
            const int id = 1;

            // Act
            Assert.Throws<ArgumentException>(() => _productLogic.UpdateProductName(id, name));

            // Assert
            _productRepository.Verify(x =>
                x.UpdateProductName(It.IsAny<int>(), It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public void RepositoryThrowsArgumentException_ThrowArgumentException()
        {
            // Arrange
            const int id = 1;
            const string name = "Foobar";
            _productRepository.Setup(x => x.UpdateProductName(It.IsAny<int>(), It.IsAny<string>()))
                .Throws<ArgumentException>();

            // Act
            Assert.Throws<ArgumentException>(() => _productLogic.UpdateProductName(id, name));

            // Assert
            _productRepository.Verify(x =>
                x.UpdateProductName(id, name), Times.Once);
        }

        [Fact]
        public void RepositoryThrowsGenericException_ThrowGenericException()
        {
            // Arrange
            const int id = 1;
            const string name = "Foobar";
            _productRepository.Setup(x => x.UpdateProductName(It.IsAny<int>(), It.IsAny<string>()))
                .Throws<Exception>();

            // Act
            Assert.Throws<Exception>(() => _productLogic.UpdateProductName(id, name));

            // Assert
            _productRepository.Verify(x =>
                x.UpdateProductName(id, name), Times.Once);
        }
    }
}