using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using ProductApi.Data;
using ProductApi.Data.Abstractions;
using ProductApi.Data.Database;
using ProductApi.Entities;
using Xunit;

namespace ProductApi.Test.Data.ProductRepositoryTests
{
    public class AddProductTests : IDisposable
    {
        private readonly ProductContext _context;
        private readonly IProductRepository _repository;

        public AddProductTests()
        {
            var databaseName = Guid.NewGuid().ToString();
            var options = new DbContextOptionsBuilder<ProductContext>()
                .UseSqlite($"Data Source=./{databaseName}.db")
                .Options;
            _context = new ProductContext(options);

            _repository = new ProductRepository(_context);
        }

        private static List<ProductOutput> Products => new()
        {
            new ProductOutput {Id = 1, Name = "Foo", Price = 11.11f, ItemsReserved = 1, ItemsInStock = 10},
            new ProductOutput {Id = 2, Name = "Bar", Price = 22.22f, ItemsReserved = 2, ItemsInStock = 20},
            new ProductOutput {Id = 3, Name = "Yaa", Price = 33.33f, ItemsReserved = 3, ItemsInStock = 30},
            new ProductOutput {Id = 4, Name = "Lorem", Price = 44.44f, ItemsReserved = 4, ItemsInStock = 40},
            new ProductOutput {Id = 5, Name = "Ipsum", Price = 55.55f, ItemsReserved = 5, ItemsInStock = 50}
        };

        void IDisposable.Dispose()
        {
            _context?.Database.EnsureDeleted();
            _context?.Dispose();
            GC.SuppressFinalize(this);
        }

        public static IEnumerable<object[]> GetInvalidNames()
        {
            yield return new object[] {null};
            yield return new object[] {string.Empty};
            yield return new object[] {" "};
            yield return new object[] {"  "};
        }

        public static IEnumerable<object[]> GetInvalidPrices()
        {
            yield return new object[] {0};
            yield return new object[] {-1f};
            yield return new object[] {-10f};
            yield return new object[] {float.MinValue};
        }

        [Fact]
        public void EnsureDatabaseExists()
        {
            // Arrange
            // No queries can be made against the database if it does not exist
            _context?.Database.EnsureDeleted();
            var input = new ProductInput
            {
                Name = "Foorbar",
                Price = 19.99f
            };
            var expected = new ProductOutput
            {
                Id = 1,
                Name = input.Name,
                Price = input.Price,
                ItemsReserved = 0,
                ItemsInStock = 0
            };

            // Act
            // If the database does not exist, this will throw an exception
            _repository.AddProduct(input);
            // Get the first, and only, product
            var output = _context?.Products.ToList()[0];

            // Assert
            output.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public void ProductIsNull_ThrowArgumentException()
        {
            // Arrange
            const ProductInput input = null;

            // Act &
            // Assert
            Assert.Throws<ArgumentException>(() => _repository.AddProduct(input));
        }

        [Fact]
        public void ProductIsNull_ExceptionMessageShouldEqual()
        {
            // Arrange
            const ProductInput input = null;
            const string errMsg = "Product is null";

            // Act &
            var exception = Assert.Throws<ArgumentException>(() => _repository.AddProduct(input));

            // Assert
            exception.Message.Should().BeEquivalentTo(errMsg);
        }

        [Fact]
        public void ProductNameAlreadyExists_ThrowArgumentException()
        {
            // Arrange
            _context?.Database.EnsureCreated();
            _context?.Products.AddRange(Products);
            _context?.SaveChanges();
            var input = new ProductInput
            {
                Name = Products[0].Name,
                Price = 19.99f
            };

            // Act &
            // Assert
            Assert.Throws<ArgumentException>(() => _repository.AddProduct(input));
        }

        [Fact]
        public void ProductNameAlreadyExists_ExceptionMessageShouldEqual()
        {
            // Arrange
            _context?.Database.EnsureCreated();
            _context?.Products.AddRange(Products);
            _context?.SaveChanges();
            var name = Products[0].Name;
            var errMsg = $"Product with name [{name}] already exists";
            var input = new ProductInput
            {
                Name = name,
                Price = 19.99f
            };

            // Act
            var exception = Assert.Throws<ArgumentException>(() => _repository.AddProduct(input));

            // Assert
            exception.Message.Should().BeEquivalentTo(errMsg);
        }

        [Theory]
        [MemberData(nameof(GetInvalidNames))]
        public void ProductNameIsInvalid_ThrowArgumentException(string name)
        {
            // Arrange
            const float price = 19.99f;
            var input = new ProductInput
            {
                Name = name,
                Price = price
            };

            // Act &
            // Assert
            Assert.Throws<ArgumentException>(() => _repository.AddProduct(input));
        }

        [Theory]
        [MemberData(nameof(GetInvalidNames))]
        public void ProductNameIsInvalid_ExceptionMessageShouldEqual(string name)
        {
            // Arrange
            const float price = 19.99f;
            var input = new ProductInput
            {
                Name = name,
                Price = price
            };
            var errMsg = $"Product Name is invalid: [{name}]";

            // Act
            var exception = Assert.Throws<ArgumentException>(() => _repository.AddProduct(input));

            // Assert
            exception.Message.Should().BeEquivalentTo(errMsg);
        }

        [Theory]
        [MemberData(nameof(GetInvalidPrices))]
        public void ProductPriceIsInvalid_ThrowArgumentException(float price)
        {
            // Arrange
            const string name = "Foobar";
            var input = new ProductInput
            {
                Name = name,
                Price = price
            };
            var errMsg = $"Product Price is invalid: [{price}]";

            // Act &
            // Assert
            Assert.Throws<ArgumentException>(() => _repository.AddProduct(input));
        }

        [Theory]
        [MemberData(nameof(GetInvalidPrices))]
        public void ProductPriceIsInvalid_ExceptionMessageShouldEqual(float price)
        {
            // Arrange
            const string name = "Foobar";
            var input = new ProductInput
            {
                Name = name,
                Price = price
            };
            var errMsg = $"Product Price is invalid: [{price}]";

            // Act
            var exception = Assert.Throws<ArgumentException>(() => _repository.AddProduct(input));

            // Assert
            exception.Message.Should().BeEquivalentTo(errMsg);
        }

        [Fact]
        public void ProductWasAdded_ExpectsNewRecord()
        {
            // Arrange
            _context?.Database.EnsureCreated();
            _context?.Products.AddRange(Products);
            _context?.SaveChanges();
            const string name = "Foobar";
            const float price = 112.50f;
            var input = new ProductInput
            {
                Name = name,
                Price = price
            };
            var expected = new List<ProductOutput>();
            expected.AddRange(Products);
            expected.Add(new ProductOutput
            {
                Id = 6,
                Name = input.Name,
                Price = input.Price,
                ItemsReserved = 0,
                ItemsInStock = 0
            });

            // Act
            _repository.AddProduct(input);
            var actual = _context?.Products.ToList();

            // Assert
            actual.Should().BeEquivalentTo(expected);
        }
    }
}