using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ProductAPI.Domain;

namespace ProductAPI.Domain.Repositories
{
    /// <summary>
    /// Repository interface for Product entity operations
    /// </summary>
    public interface IProductRepository
    {
        /// <summary>
        /// Gets all products
        /// </summary>
        /// <returns>Collection of all products</returns>
        Task<IEnumerable<Product>> GetAllAsync();
        
        /// <summary>
        /// Gets a product by its identifier
        /// </summary>
        /// <param name="id">Product identifier</param>
        /// <returns>The product if found; otherwise, null</returns>
        Task<Product?> GetByIdAsync(Guid id);
        
        /// <summary>
        /// Adds a new product
        /// </summary>
        /// <param name="product">Product to add</param>
        /// <returns>The added product with generated ID</returns>
        Task<Product> AddAsync(Product product);
        
        /// <summary>
        /// Updates an existing product
        /// </summary>
        /// <param name="product">Product to update</param>
        /// <returns>True if the product was updated; otherwise, false</returns>
        Task<bool> UpdateAsync(Product product);
        
        /// <summary>
        /// Deletes a product by its identifier
        /// </summary>
        /// <param name="id">Product identifier</param>
        /// <returns>True if the product was deleted; otherwise, false</returns>
        Task<bool> DeleteAsync(Guid id);
        
        /// <summary>
        /// Searches for products based on various criteria
        /// </summary>
        /// <param name="query">Optional search term for product name or description</param>
        /// <param name="category">Optional category filter</param>
        /// <param name="minPrice">Optional minimum price filter</param>
        /// <param name="maxPrice">Optional maximum price filter</param>
        /// <param name="inStock">Optional availability filter</param>
        /// <param name="tags">Optional tags filter</param>
        /// <returns>Collection of matching products</returns>
        Task<IEnumerable<Product>> SearchAsync(
            string? query = null,
            string? category = null,
            decimal? minPrice = null,
            decimal? maxPrice = null,
            bool? inStock = null,
            IEnumerable<string>? tags = null);
    }
}