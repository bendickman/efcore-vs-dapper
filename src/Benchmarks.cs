using BenchmarkDotNet.Attributes;
using Dapper;
using EfCoreVsDapper.Database;
using EfCoreVsDapper.Entities;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace EfCoreVsDapper
{
    [MemoryDiagnoser]
    public class Benchmarks
    {
        private ProductGenerator _productGenerator;

        private Product _product = new()
        {
            Id = 1,
            Name = "Test Product",
            Description = "Test Product Description",
            Price = 19.99m
        };

        private IDbConnection _dbConnection;

        private ApplicationDbContext _dbContext;

        private static readonly string _selectProductsQuery = "SELECT * FROM Products";
        private static readonly string _selectProductByIdQuery = "SELECT Id, Name, Description, Price FROM Products Where Id = @Id";
        private static readonly string _productsCountQuery = "SELECT COUNT(*) FROM Products";
        private static readonly string _insertProductQuery = "INSERT INTO Products (Id, Name, Description, Price) VALUES (@Id, @Name, @Description, @Price)";

        [GlobalSetup]
        public async void Setup()
        {
            _dbConnection = new SqliteConnection($"Data Source=C:\\DevRoot\\efcore-vs-dapper\\src\\product.db");
            _productGenerator = new ProductGenerator(_dbConnection);
            _dbContext = new ApplicationDbContext(_dbConnection);

            await _dbConnection.ExecuteAsync(_insertProductQuery, _product);
            await _productGenerator.SeedProductsAsync(10);
        }

        [GlobalCleanup]
        public async void Cleanup()
        {
            await _productGenerator.CleanupProductsAsync();
        }

        [Benchmark]
        public async Task<List<Product>> EF_List()
        {
            return await _dbContext.Products.ToListAsync();
        }

        [Benchmark]
        public async Task<List<Product>> Dapper_List()
        {
            var results = await _dbConnection
                .QueryAsync<Product>(_selectProductsQuery);

            return results.ToList();
        }

        [Benchmark]
        public async Task<Product> EF_Find()
        {
            return await _dbContext
                .Products
                .FindAsync(_product.Id);
        }

        [Benchmark]
        public async Task<Product> EF_Single()
        {
            return await _dbContext
                .Products
                .SingleOrDefaultAsync(p => p.Id == _product.Id);
        }

        [Benchmark]
        public async Task<Product> Dapper_GetById()
        {
            return await _dbConnection
                .QuerySingleOrDefaultAsync<Product>(_selectProductByIdQuery, new { _product.Id });
        }
    }
}
