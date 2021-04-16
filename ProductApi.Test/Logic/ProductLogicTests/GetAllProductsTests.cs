using System;
using Moq;
using ProductApi.Data.Abstractions;
using ProductApi.Logic;
using Xunit;

namespace ProductApi.Test.Logic.ProductLogicTests
{
    public class GetAllProducts
    {
        private readonly ProductLogic _productLogic;
        private readonly Mock<IProductRepository> _productRepository;

        public GetAllProducts()
        {
            _productRepository = new Mock<IProductRepository>();

            _productLogic = new ProductLogic(
                _productRepository.Object);
        }

        [Fact]
        public void NoExceptionsThrown_ExpectsList()
        {
            // Arrange
            // Nothing specific

            // Act
            _productLogic.GetAllProducts();

            // Assert
            _productRepository.Verify(x =>
                x.GetAllProducts(), Times.Once);
        }

        [Fact]
        public void RepositoryThrowsArgumentException_ThrowArgumentException()
        {
            // Arrange
            _productRepository.Setup(x => x.GetAllProducts())
                .Throws<ArgumentException>();

            // Act
            Assert.Throws<ArgumentException>(() => _productLogic.GetAllProducts());

            // Assert
            _productRepository.Verify(x =>
                x.GetAllProducts(), Times.Once);
        }

        [Fact]
        public void RepositoryThrowsGenericException_ThrowGenericException()
        {
            // Arrange
            _productRepository.Setup(x => x.GetAllProducts())
                .Throws<Exception>();

            // Act
            Assert.Throws<Exception>(() => _productLogic.GetAllProducts());

            // Assert
            _productRepository.Verify(x =>
                x.GetAllProducts(), Times.Once);
        }
    }
}