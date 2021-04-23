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
    public class UpdateProductStockTests : IDisposable
    {
        private readonly ProductContext _context;
        private readonly IProductRepository _repository;

        public UpdateProductStockTests()
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

        public static IEnumerable<object[]> GetInvalidIds()
        {
            yield return new object[] {0};
            yield return new object[] {-1};
            yield return new object[] {-10};
            yield return new object[] {int.MinValue};
        }

        public static IEnumerable<object[]> GetNonexistentIds()
        {
            yield return new object[] {6};
            yield return new object[] {10};
            yield return new object[] {100};
            yield return new object[] {int.MaxValue};
        }

        [Fact]
        public void EnsureDatabaseExists()
        {
            // Arrange
            _context?.Database.EnsureDeleted();
            const int id = 1;
            const int amount = 10;
            // Act
            // Expects an Argument Exception as the database will be empty
            Assert.Throws<ArgumentException>(() => _repository.UpdateProductStock(id, amount));
            List<ProductOutput> actual = _context?.Products.ToList();
            
            // Assert
            actual.Should().BeEmpty();
        }

        [Theory]
        [MemberData(nameof(GetInvalidIds))]
        public void ProductIdIsInvalid_ThrowArgumentException(int id)
        {
            // Arrange
            const int amount = 10;
            
            // Act &
            // Assert
            Assert.Throws<ArgumentException>(() => _repository.UpdateProductStock(id, amount));
        }

        [Theory]
        [MemberData(nameof(GetInvalidIds))]
        public void ProductIdIsInvalid_ExceptionMessageShouldEqual(int id)
        {
            // Arrange
            const int amount = 10;
            var errMsg = $"Product ID is invalid: [{id}]";
            
            // Act
            ArgumentException exception = Assert.Throws<ArgumentException>(() => _repository.UpdateProductStock(id, amount));
            
            // Assert
            exception.Message.Should().BeEquivalentTo(errMsg);
        }

        [Theory]
        [MemberData(nameof(GetNonexistentIds))]
        public void ProductIdDoesNotExist_ThrowArgumentException(int id)
        {
            // Arrange
            _context?.Database.EnsureCreated();
            _context?.Products.AddRange(Products);
            _context?.SaveChanges();
            const int amount = 10;
            
            // Act &
            // Assert
            Assert.Throws<ArgumentException>(() => _repository.UpdateProductStock(id, amount));
        }

        [Theory]
        [MemberData(nameof(GetNonexistentIds))]
        public void ProductIdDoesNotExist_ExceptionMessageShouldEqual(int id)
        {
            // Arrange
            _context?.Database.EnsureCreated();
            _context?.Products.AddRange(Products);
            _context?.SaveChanges();
            const int amount = 10;
            string errMsg = $"Product ID does not exist: [{id}]";
            
            // Act
            ArgumentException exception = Assert.Throws<ArgumentException>(() => _repository.UpdateProductStock(id, amount));

            // Assert
            exception.Message.Should().BeEquivalentTo(errMsg);
        }

        [Fact]
        public void Negative_AvailableStockBecomesNegative_ThrowArgumentException()
        {
            // Arrange
            _context?.Database.EnsureCreated();
            _context?.Products.AddRange(Products);
            _context?.SaveChanges();
            const int id = 1;
            const int amount = -10;

            // Act &
            // Assert
            Assert.Throws<ArgumentException>(() => _repository.UpdateProductStock(id, amount));
        }

        [Fact]
        public void Negative_StockBecomesNegative_ThrowArgumentException()
        {
            // Arrange
            _context?.Database.EnsureCreated();
            _context?.Products.AddRange(Products);
            _context?.SaveChanges();
            const int id = 1;
            const int amount = -20; // 1 reserve, 10 stock
            
            // Act &
            // Assert
            Assert.Throws<ArgumentException>(() => _repository.UpdateProductStock(id, amount));
        }

        [Fact]
        public void StockWasUpdated_ExpectsUpdatedRecord()
        {
            // Arrange
            _context?.Database.EnsureCreated();
            _context?.Products.AddRange(Products);
            _context?.SaveChanges();
            const int id = 1;
            const int amount = 10;
            const int expected = 20;
            
            // Act
            _repository.UpdateProductStock(id, amount);
            var actual = _context?.Products.First(x => x.Id == id);
            
            // Assert
            actual.ItemsInStock.Should().Be(expected);
        }
    }
}