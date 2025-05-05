using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Linq;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ProductAPI.Domain;
using ProductAPI.Domain.Repositories;
using ProductAPI.Infrastructure.Configuration;

namespace ProductAPI.Infrastructure.Repositories
{
    /// <summary>
    /// Azure Cosmos DB implementation of the product repository
    /// </summary>
    public class CosmosDbProductRepository : IProductRepository, IDisposable
    {
        private readonly CosmosClient _cosmosClient;
        private readonly Container _container;
        private readonly ILogger<CosmosDbProductRepository> _logger;
        private readonly CosmosDbSettings _settings;
        private bool _isInitialized = false;

        /// <summary>
        /// Initializes a new instance of the <see cref="CosmosDbProductRepository"/> class.
        /// </summary>
        /// <param name="options">Database configuration options</param>
        /// <param name="logger">Logger instance</param>
        public CosmosDbProductRepository(
            IOptions<DatabaseSettings> options,
            ILogger<CosmosDbProductRepository> logger)
        {
            if (options == null)
                throw new ArgumentNullException(nameof(options));
                
            _settings = options.Value.CosmosDb ?? 
                throw new ArgumentException("Cosmos DB settings are not configured.", nameof(options));
                
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            
            if (string.IsNullOrEmpty(_settings.ConnectionString))
                throw new ArgumentException("Cosmos DB connection string is not configured.");
                
            if (string.IsNullOrEmpty(_settings.DatabaseName))
                throw new ArgumentException("Cosmos DB database name is not configured.");
                
            if (string.IsNullOrEmpty(_settings.ContainerName))
                throw new ArgumentException("Cosmos DB container name is not configured.");

            _cosmosClient = new CosmosClient(_settings.ConnectionString);
            _container = _cosmosClient.GetContainer(_settings.DatabaseName, _settings.ContainerName);
            
            _logger.LogInformation(
                "CosmosDB repository initialized with database {Database} and container {Container}",
                _settings.DatabaseName,
                _settings.ContainerName);
                
            InitializeAsync().GetAwaiter().GetResult();
        }

        /// <summary>
        /// Initializes the Cosmos DB database and container if they don't exist
        /// </summary>
        private async Task InitializeAsync()
        {
            if (_isInitialized)
                return;
                
            try
            {
                _logger.LogInformation("Ensuring Cosmos DB database and container exist");
                
                // Create database if it doesn't exist
                DatabaseResponse databaseResponse = await _cosmosClient.CreateDatabaseIfNotExistsAsync(
                    _settings.DatabaseName, 
                    ThroughputProperties.CreateAutoscaleThroughput(1000)); // Minimal autoscale throughput
                
                _logger.LogInformation("Database {DatabaseName} ensured with status code {StatusCode}", 
                    _settings.DatabaseName, 
                    databaseResponse.StatusCode);
                
                // Create container if it doesn't exist
                ContainerResponse containerResponse = await databaseResponse.Database.CreateContainerIfNotExistsAsync(
                    _settings.ContainerName,
                    _settings.PartitionKeyPath);
                
                _logger.LogInformation("Container {ContainerName} ensured with status code {StatusCode}", 
                    _settings.ContainerName, 
                    containerResponse.StatusCode);

                _isInitialized = true;
                
                // Seed data if container was just created (201 = Created)
                if (containerResponse.StatusCode == HttpStatusCode.Created)
                {
                    await SeedDataAsync();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error initializing Cosmos DB repository");
                throw;
            }
        }

        /// <inheritdoc />
        public async Task<IEnumerable<Product>> GetAllAsync()
        {
            _logger.LogInformation("Getting all products from Cosmos DB");
            
            try
            {
                var query = _container.GetItemLinqQueryable<Product>()
                    .ToFeedIterator();
                
                var results = new List<Product>();
                while (query.HasMoreResults)
                {
                    var response = await query.ReadNextAsync();
                    results.AddRange(response);
                }
                
                return results;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all products from Cosmos DB");
                throw;
            }
        }

        /// <inheritdoc />
        public async Task<Product?> GetByIdAsync(Guid id)
        {
            _logger.LogInformation("Getting product with id {ProductId} from Cosmos DB", id);
            
            try
            {
                var response = await _container.GetItemLinqQueryable<Product>()
                    .Where(p => p.Id == id)
                    .ToFeedIterator()
                    .ReadNextAsync();
                
                return response.FirstOrDefault();
            }
            catch (CosmosException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
            {
                _logger.LogInformation("Product with id {ProductId} not found in Cosmos DB", id);
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting product with id {ProductId} from Cosmos DB", id);
                throw;
            }
        }

        /// <inheritdoc />
        public async Task<Product> AddAsync(Product product)
        {
            if (product == null)
                throw new ArgumentNullException(nameof(product));
                
            _logger.LogInformation("Adding product to Cosmos DB");
            
            try
            {
                if (product.Id == Guid.Empty)
                {
                    product.Id = Guid.NewGuid();
                }
                
                product.CreatedAt = DateTime.UtcNow;
                
                var response = await _container.CreateItemAsync(
                    product,
                    new PartitionKey(product.Category));
                
                _logger.LogInformation(
                    "Added product with id {ProductId} to Cosmos DB with request charge {RequestCharge}",
                    product.Id,
                    response.RequestCharge);
                
                return response.Resource;
            }
            catch (CosmosException ex) when (ex.StatusCode == HttpStatusCode.Conflict)
            {
                _logger.LogWarning(
                    "Failed to add product with id {ProductId} to Cosmos DB - already exists",
                    product.Id);
                throw new InvalidOperationException($"Product with id {product.Id} already exists.", ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding product to Cosmos DB");
                throw;
            }
        }

        /// <inheritdoc />
        public async Task<bool> UpdateAsync(Product product)
        {
            if (product == null)
                throw new ArgumentNullException(nameof(product));
                
            if (product.Id == Guid.Empty)
                throw new ArgumentException("Product id cannot be empty", nameof(product));
                
            _logger.LogInformation("Updating product with id {ProductId} in Cosmos DB", product.Id);
            
            try
            {
                // First check if the product exists
                var existingProduct = await GetByIdAsync(product.Id);
                if (existingProduct == null)
                {
                    _logger.LogWarning(
                        "Failed to update product with id {ProductId} in Cosmos DB - not found",
                        product.Id);
                    return false;
                }
                
                // Preserve creation timestamp
                product.CreatedAt = existingProduct.CreatedAt;
                product.UpdatedAt = DateTime.UtcNow;
                
                var response = await _container.ReplaceItemAsync(
                    product,
                    product.Id.ToString(),
                    new PartitionKey(product.Category));
                
                _logger.LogInformation(
                    "Updated product with id {ProductId} in Cosmos DB with request charge {RequestCharge}",
                    product.Id,
                    response.RequestCharge);
                
                return true;
            }
            catch (CosmosException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
            {
                _logger.LogWarning(
                    "Failed to update product with id {ProductId} in Cosmos DB - not found",
                    product.Id);
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating product with id {ProductId} in Cosmos DB", product.Id);
                throw;
            }
        }

        /// <inheritdoc />
        public async Task<bool> DeleteAsync(Guid id)
        {
            _logger.LogInformation("Deleting product with id {ProductId} from Cosmos DB", id);
            
            try
            {
                // First we need to get the product to know its partition key
                var product = await GetByIdAsync(id);
                if (product == null)
                {
                    _logger.LogWarning(
                        "Failed to delete product with id {ProductId} from Cosmos DB - not found",
                        id);
                    return false;
                }
                
                var response = await _container.DeleteItemAsync<Product>(
                    id.ToString(),
                    new PartitionKey(product.Category));
                
                _logger.LogInformation(
                    "Deleted product with id {ProductId} from Cosmos DB with request charge {RequestCharge}",
                    id,
                    response.RequestCharge);
                
                return true;
            }
            catch (CosmosException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
            {
                _logger.LogWarning(
                    "Failed to delete product with id {ProductId} from Cosmos DB - not found",
                    id);
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting product with id {ProductId} from Cosmos DB", id);
                throw;
            }
        }

        /// <inheritdoc />
        public async Task<IEnumerable<Product>> SearchAsync(
            string? query = null, 
            string? category = null, 
            decimal? minPrice = null, 
            decimal? maxPrice = null, 
            bool? inStock = null, 
            IEnumerable<string>? tags = null)
        {
            _logger.LogInformation("Searching products in Cosmos DB with query: {Query}, category: {Category}, " +
                                 "minPrice: {MinPrice}, maxPrice: {MaxPrice}, inStock: {InStock}, tags: {Tags}",
                                 query, category, minPrice, maxPrice, inStock,
                                 tags != null ? string.Join(", ", tags) : null);
            
            try
            {
                // Start with all products query
                IQueryable<Product> queryable = _container.GetItemLinqQueryable<Product>();
                
                // Apply filters
                if (!string.IsNullOrWhiteSpace(category))
                {
                    var lowerCategory = category.ToLowerInvariant();
                    queryable = queryable.Where(p => p.Category.ToLower() == lowerCategory);
                }
                
                if (minPrice.HasValue)
                {
                    queryable = queryable.Where(p => p.Price >= minPrice.Value);
                }
                
                if (maxPrice.HasValue)
                {
                    queryable = queryable.Where(p => p.Price <= maxPrice.Value);
                }
                
                if (inStock.HasValue)
                {
                    queryable = queryable.Where(p => p.InStock == inStock.Value);
                }
                
                // Execute the query
                var feedIterator = queryable.ToFeedIterator();
                var results = new List<Product>();
                
                while (feedIterator.HasMoreResults)
                {
                    var response = await feedIterator.ReadNextAsync();
                    results.AddRange(response);
                }
                
                // Apply remaining filters that can't be efficiently done in the Cosmos query
                if (!string.IsNullOrWhiteSpace(query))
                {
                    var lowerQuery = query.ToLowerInvariant();
                    results = results
                        .Where(p =>
                            p.Name.ToLowerInvariant().Contains(lowerQuery) ||
                            (p.Description != null && p.Description.ToLowerInvariant().Contains(lowerQuery)))
                        .ToList();
                }
                
                if (tags != null && tags.Any())
                {
                    var searchTags = tags.ToList();
                    results = results
                        .Where(p =>
                            searchTags.Any(tag => p.Tags.Contains(tag, StringComparer.OrdinalIgnoreCase)))
                        .ToList();
                }
                
                return results;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching products in Cosmos DB");
                throw;
            }
        }

        /// <summary>
        /// Seeds initial data to the Cosmos DB container
        /// </summary>
        private async Task SeedDataAsync()
        {
            _logger.LogInformation("Seeding data to Cosmos DB");
            
            var products = new List<Product>
            {
                new Product
                {
                    Id = Guid.NewGuid(),
                    Name = "Laptop",
                    Description = "High-performance laptop with the latest processor",
                    Price = 1200.00m,
                    Category = "Electronics",
                    Tags = new List<string> { "computer", "tech", "portable" },
                    InStock = true,
                    CreatedAt = DateTime.UtcNow
                },
                new Product
                {
                    Id = Guid.NewGuid(),
                    Name = "Smartphone",
                    Description = "Latest smartphone with high-resolution camera",
                    Price = 800.00m,
                    Category = "Electronics",
                    Tags = new List<string> { "mobile", "tech", "phone" },
                    InStock = true,
                    CreatedAt = DateTime.UtcNow
                },
                new Product
                {
                    Id = Guid.NewGuid(),
                    Name = "Coffee Table",
                    Description = "Elegant coffee table made of solid wood",
                    Price = 250.00m,
                    Category = "Furniture",
                    Tags = new List<string> { "table", "wood", "living room" },
                    InStock = true,
                    CreatedAt = DateTime.UtcNow
                }
            };
            
            foreach (var product in products)
            {
                await _container.CreateItemAsync(
                    product,
                    new PartitionKey(product.Category));
            }
            
            _logger.LogInformation("Seeded {Count} products to Cosmos DB", products.Count);
        }

        /// <inheritdoc />
        public void Dispose()
        {
            // CosmosClient implements IDisposable
            if (_cosmosClient != null)
            {
                _cosmosClient.Dispose();
                _logger.LogInformation("CosmosDB client disposed");
            }
            GC.SuppressFinalize(this);
        }
    }
}