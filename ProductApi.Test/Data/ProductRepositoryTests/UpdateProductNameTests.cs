using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using ProductApi.Data;
using ProductApi.Data.Database;
using ProductApi.Entities;
using Xunit;
using Xunit.Abstractions;

namespace ProductApi.Test.Data.ProductRepositoryTests
{
    public class UpdateProductNameTests : IDisposable
    {
        private readonly ProductContext _context;
        private readonly ProductRepository _repository;

        private readonly ITestOutputHelper _output;

        public UpdateProductNameTests(ITestOutputHelper output)
        {
            _output = output;
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

        public static IEnumerable<object[]> GetInvalidNames()
        {
            yield return new object[] {null};
            yield return new object[] {string.Empty};
            yield return new object[] {" "};
            yield return new object[] {"  "};
        }

        public static IEnumerable<object[]> GetExistingNames()
        {
            yield return new object[] {Products[1].Name};
            yield return new object[] {Products[2].Name};
            yield return new object[] {Products[3].Name};
        }

        void IDisposable.Dispose()
        {
            _context?.Database.EnsureDeleted();
            _context?.Dispose();
            GC.SuppressFinalize(this);
        }

        [Fact]
        public void EnsureDatabaseExists()
        {
            // Arrange
            _context?.Database.EnsureDeleted();
            const int id = 1;
            const string name = "Foobar";
            
            // Act
            // Expects the Not Found exception to be thrown as the Id to update does not exist
            Assert.Throws<ArgumentException>(() => _repository.UpdateProductName(id, name));
            List<ProductOutput> actual = _context?.Products.ToList();
            
            // Assert
            actual.Should().BeEmpty();
        }

        [Theory]
        [MemberData(nameof(GetInvalidIds))]
        public void ProductIdIsInvalid_ThrowArgumentException(int id)
        {
            // Arrange
            const string name = "Foobar";
            
            // Act &
            // Assert
            Assert.Throws<ArgumentException>(() => _repository.UpdateProductName(id, name));
        }

        [Theory]
        [MemberData(nameof(GetInvalidIds))]
        public void ProductIdIsInvalid_ExceptionMessageShouldEqual(int id)
        {
            // Arrange
            const string name = "Foobar";
            var errMsg = $"Product ID is invalid: [{id}]";

            // Act
            ArgumentException exception = Assert.Throws<ArgumentException>(() => _repository.UpdateProductName(id, name));

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
            const string name = "Foobar";
            
            // Act & 
            // Assert
            Assert.Throws<ArgumentException>(() => _repository.UpdateProductName(id, name));
        }

        [Theory]
        [MemberData(nameof(GetNonexistentIds))]
        public void ProductIdDoesNotExist_ExceptionMessageShouldEqual(int id)
        {
            // Arrange
            _context?.Database.EnsureCreated();
            _context?.Products.AddRange(Products);
            _context?.SaveChanges();
            const string name = "Foobar";
            var errMsg = $"Product ID does not exist: [{id}]";
            
            // Act
            ArgumentException exception = Assert.Throws<ArgumentException>(() => _repository.UpdateProductName(id, name));

            // Assert
            exception.Message.Should().BeEquivalentTo(errMsg);
        }

        [Theory]
        [MemberData(nameof(GetExistingNames))]
        public void ProductNameAlreadyExists_ThrowArgumentException(string name)
        {
            // Arrange
            _context?.Database.EnsureCreated();
            _context?.Products.AddRange(Products);
            _context?.SaveChanges();
            const int id = 1; // Reflects Products index 0
            
            // Act
            Assert.Throws<ArgumentException>(() => _repository.UpdateProductName(id, name));
            ProductOutput actual = _context?.Products.First(output => output.Id == id);
            foreach (var product in _context?.Products)
            {
                _output.WriteLine($"Id: {product.Id}, Name: {product.Name}");
            }
            
            // Assert
            actual.Should().BeEquivalentTo(Products[0]);
        }

        [Theory]
        [MemberData(nameof(GetExistingNames))]
        public void ProductNameAlreadyExists_ExceptionMessageShouldEqual(string name)
        {
            // Arrange
            _context?.Database.EnsureCreated();
            _context?.Products.AddRange(Products);
            _context?.SaveChanges();
            const int id = 1; // ID 1 and 5 can be uses
            var errMsg = $"Product Name already exists: [{name}]";
            
            // Act
            ArgumentException exception = Assert.Throws<ArgumentException>(() => _repository.UpdateProductName(id, name));
            
            // Assert
            exception.Message.Should().BeEquivalentTo(errMsg);
        }

        [Theory]
        [MemberData(nameof(GetInvalidNames))]
        public void ProductNameIsInvalid_ThrowArgumentException(string name)
        {
            // Arrange
            const int id = 1;
            
            // Act &
            // Assert
            Assert.Throws<ArgumentException>(() => _repository.UpdateProductName(id, name));
        }

        [Theory]
        [MemberData(nameof(GetInvalidNames))]
        public void ProductNameIsInvalid_ExceptionMessageShouldEqual(string name)
        {
            // Arrange
            const int id = 1;
            string errMsg = $"Product Name is invalid: [{name}]";

            // Act &
            ArgumentException exception = Assert.Throws<ArgumentException>(() => _repository.UpdateProductName(id, name));

            // Assert
            exception.Message.Should().BeEquivalentTo(errMsg);
        }

        [Fact]
        public void ProductNameWasUpdated_ExpectUpdatedRecord()
        {
            // Arrange
            _context?.Database.EnsureCreated();
            _context?.Products.AddRange(Products);
            _context?.SaveChanges();
            const int id = 1;
            const string name = "Foobar";
            
            // Act
            _repository.UpdateProductName(id, name);
            var actual = _context?.Products.FirstOrDefault(x => x.Id == id);
            
            // Assert
            Assert.NotNull(actual);
            actual.Name.Should().BeEquivalentTo(name);
        }
    }
}