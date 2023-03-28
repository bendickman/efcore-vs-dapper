using Bogus;
using Dapper;
using EfCoreVsDapper.Entities;
using System.Data;

namespace EfCoreVsDapper
{
    public class ProductGenerator
    {
        private readonly Faker<Product> _faker;
        private readonly IDbConnection _dbConnection;

        public ProductGenerator(IDbConnection dbConnection)
        {
            _faker = new Faker<Product>()
                .RuleFor(p => p.Id, f => f.Random.Int(2))
                .RuleFor(p => p.Name, f => f.Random.Word())
                .RuleFor(p => p.Description, f => f.Random.Words(5))
                .RuleFor(p => p.Price, f => f.Random.Decimal(1, 100));
            _dbConnection = dbConnection;
        }

        public async Task SeedProductsAsync(int count)
        {
            var products = _faker.Generate(count);

            foreach (var product in products)
            {
                await _dbConnection.ExecuteAsync(
                    "INSERT INTO Products (Id, Name, Description, Price) VALUES (@Id, @Name, @Description, @Price)", 
                    new { product.Id, product.Name, product.Description, product.Price});
            }
        }

        public async Task CleanupProductsAsync()
        {
            Console.WriteLine("Deleting Products");

            var result = await _dbConnection
                .ExecuteAsync("DELETE FROM Products");

            Console.WriteLine($"Deleted {result} Products");
        }
    }
}
