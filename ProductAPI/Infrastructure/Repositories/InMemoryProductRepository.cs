using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using ProductAPI.Domain;
using ProductAPI.Domain.Repositories;

namespace ProductAPI.Infrastructure.Repositories
{
    /// <summary>
    /// In-memory implementation of the product repository
    /// </summary>
    public class InMemoryProductRepository : IProductRepository
    {
        private readonly ConcurrentDictionary<Guid, Product> _products = new();
        private readonly ILogger<InMemoryProductRepository> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="InMemoryProductRepository"/> class.
        /// </summary>
        /// <param name="logger">Logger instance</param>
        public InMemoryProductRepository(ILogger<InMemoryProductRepository> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            SeedData();
        }

        /// <inheritdoc />
        public Task<IEnumerable<Product>> GetAllAsync()
        {
            _logger.LogInformation("Getting all products");
            return Task.FromResult<IEnumerable<Product>>(_products.Values.ToList());
        }

        /// <inheritdoc />
        public Task<Product?> GetByIdAsync(Guid id)
        {
            _logger.LogInformation("Getting product with id {ProductId}", id);
            _products.TryGetValue(id, out var product);
            return Task.FromResult(product);
        }

        /// <inheritdoc />
        public Task<Product> AddAsync(Product product)
        {
            if (product == null)
                throw new ArgumentNullException(nameof(product));

            product.Id = product.Id == Guid.Empty ? Guid.NewGuid() : product.Id;
            product.CreatedAt = DateTime.UtcNow;
            
            if (!_products.TryAdd(product.Id, product))
            {
                _logger.LogWarning("Failed to add product with id {ProductId}", product.Id);
                throw new InvalidOperationException($"Product with id {product.Id} already exists.");
            }

            _logger.LogInformation("Added product with id {ProductId}", product.Id);
            return Task.FromResult(product);
        }

        /// <inheritdoc />
        public Task<bool> UpdateAsync(Product product)
        {
            if (product == null)
                throw new ArgumentNullException(nameof(product));

            if (product.Id == Guid.Empty)
                throw new ArgumentException("Product id cannot be empty", nameof(product));

            var exists = _products.TryGetValue(product.Id, out var existingProduct);
            if (!exists)
            {
                _logger.LogWarning("Failed to update product with id {ProductId} - not found", product.Id);
                return Task.FromResult(false);
            }

            product.CreatedAt = existingProduct!.CreatedAt;
            product.UpdatedAt = DateTime.UtcNow;

            var result = _products.TryUpdate(product.Id, product, existingProduct);
            _logger.LogInformation("Updated product with id {ProductId}: {Result}", product.Id, result);
            return Task.FromResult(result);
        }

        /// <inheritdoc />
        public Task<bool> DeleteAsync(Guid id)
        {
            var result = _products.TryRemove(id, out _);
            _logger.LogInformation("Deleted product with id {ProductId}: {Result}", id, result);
            return Task.FromResult(result);
        }

        /// <inheritdoc />
        public Task<IEnumerable<Product>> SearchAsync(
            string? query = null, 
            string? category = null, 
            decimal? minPrice = null, 
            decimal? maxPrice = null, 
            bool? inStock = null, 
            IEnumerable<string>? tags = null)
        {
            _logger.LogInformation("Searching products with query: {Query}, category: {Category}, " +
                                 "minPrice: {MinPrice}, maxPrice: {MaxPrice}, inStock: {InStock}, tags: {Tags}",
                                 query, category, minPrice, maxPrice, inStock,
                                 tags != null ? string.Join(", ", tags) : null);

            var searchTags = tags?.ToList();
            
            var results = _products.Values
                .AsEnumerable();
                
            // Apply filters
            if (!string.IsNullOrWhiteSpace(query))
            {
                var lowerQuery = query.ToLowerInvariant();
                results = results.Where(p => 
                    p.Name.ToLowerInvariant().Contains(lowerQuery) || 
                    (p.Description != null && p.Description.ToLowerInvariant().Contains(lowerQuery)));
            }
            
            if (!string.IsNullOrWhiteSpace(category))
            {
                var lowerCategory = category.ToLowerInvariant();
                results = results.Where(p => p.Category.ToLowerInvariant() == lowerCategory);
            }
            
            if (minPrice.HasValue)
            {
                results = results.Where(p => p.Price >= minPrice.Value);
            }
            
            if (maxPrice.HasValue)
            {
                results = results.Where(p => p.Price <= maxPrice.Value);
            }
            
            if (inStock.HasValue)
            {
                results = results.Where(p => p.InStock == inStock.Value);
            }
            
            if (searchTags != null && searchTags.Any())
            {
                results = results.Where(p => 
                    searchTags.Any(tag => p.Tags.Contains(tag, StringComparer.OrdinalIgnoreCase)));
            }

            return Task.FromResult(results);
        }

        private void SeedData()
        {
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
                _products.TryAdd(product.Id, product);
            }

            _logger.LogInformation("Seeded {Count} products", products.Count);
        }
    }
}