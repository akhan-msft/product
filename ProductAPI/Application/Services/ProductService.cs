using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using ProductAPI.Application.DTOs;
using ProductAPI.Domain;
using ProductAPI.Domain.Repositories;

namespace ProductAPI.Application.Services
{
    /// <summary>
    /// Implementation of the product service
    /// </summary>
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;
        private readonly ILogger<ProductService> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProductService"/> class.
        /// </summary>
        /// <param name="productRepository">Product repository</param>
        /// <param name="logger">Logger instance</param>
        public ProductService(IProductRepository productRepository, ILogger<ProductService> logger)
        {
            _productRepository = productRepository ?? throw new ArgumentNullException(nameof(productRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <inheritdoc />
        public async Task<IEnumerable<ProductDto>> GetAllProductsAsync()
        {
            _logger.LogInformation("Getting all products");
            var products = await _productRepository.GetAllAsync();
            return products.Select(MapToDto);
        }

        /// <inheritdoc />
        public async Task<ProductDto?> GetProductByIdAsync(Guid id)
        {
            _logger.LogInformation("Getting product with id {ProductId}", id);
            var product = await _productRepository.GetByIdAsync(id);
            return product != null ? MapToDto(product) : null;
        }

        /// <inheritdoc />
        public async Task<ProductDto> CreateProductAsync(AddProductDto addProductDto)
        {
            if (addProductDto == null) 
                throw new ArgumentNullException(nameof(addProductDto));

            _logger.LogInformation("Creating new product");
            
            var product = new Product
            {
                Name = addProductDto.Name,
                Description = addProductDto.Description,
                Price = addProductDto.Price,
                Category = addProductDto.Category,
                Tags = addProductDto.Tags ?? new List<string>(),
                InStock = addProductDto.InStock
            };

            var createdProduct = await _productRepository.AddAsync(product);
            return MapToDto(createdProduct);
        }

        /// <inheritdoc />
        public async Task<ProductDto?> UpdateProductAsync(Guid id, UpdateProductDto updateProductDto)
        {
            if (updateProductDto == null)
                throw new ArgumentNullException(nameof(updateProductDto));

            _logger.LogInformation("Updating product with id {ProductId}", id);
            
            var existingProduct = await _productRepository.GetByIdAsync(id);
            if (existingProduct == null)
            {
                _logger.LogWarning("Product with id {ProductId} not found for update", id);
                return null;
            }

            // Update only provided fields
            if (updateProductDto.Name != null)
                existingProduct.Name = updateProductDto.Name;
            
            if (updateProductDto.Description != null)
                existingProduct.Description = updateProductDto.Description;
            
            if (updateProductDto.Price.HasValue)
                existingProduct.Price = updateProductDto.Price.Value;
            
            if (updateProductDto.Category != null)
                existingProduct.Category = updateProductDto.Category;
            
            if (updateProductDto.Tags != null)
                existingProduct.Tags = updateProductDto.Tags;
            
            if (updateProductDto.InStock.HasValue)
                existingProduct.InStock = updateProductDto.InStock.Value;
            
            var success = await _productRepository.UpdateAsync(existingProduct);
            
            if (!success)
            {
                _logger.LogWarning("Failed to update product with id {ProductId}", id);
                return null;
            }
            
            return MapToDto(existingProduct);
        }

        /// <inheritdoc />
        public async Task<bool> DeleteProductAsync(Guid id)
        {
            _logger.LogInformation("Deleting product with id {ProductId}", id);
            return await _productRepository.DeleteAsync(id);
        }

        /// <inheritdoc />
        public async Task<IEnumerable<ProductDto>> SearchProductsAsync(SearchProductDto searchProductDto)
        {
            if (searchProductDto == null)
                throw new ArgumentNullException(nameof(searchProductDto));

            _logger.LogInformation("Searching products with criteria: {SearchCriteria}", 
                System.Text.Json.JsonSerializer.Serialize(searchProductDto));
            
            var products = await _productRepository.SearchAsync(
                searchProductDto.Query,
                searchProductDto.Category,
                null,  // MinPrice - no longer used
                null,  // MaxPrice - no longer used
                null,  // InStock - no longer used
                null); // Tags - no longer used
            
            return products.Select(MapToDto);
        }

        /// <summary>
        /// Maps a Product entity to a ProductDto
        /// </summary>
        /// <param name="product">Product entity to map</param>
        /// <returns>Mapped ProductDto</returns>
        private static ProductDto MapToDto(Product product)
        {
            return new ProductDto
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                Category = product.Category,
                Tags = product.Tags.ToList(),
                InStock = product.InStock,
                CreatedAt = product.CreatedAt,
                UpdatedAt = product.UpdatedAt
            };
        }
    }
}