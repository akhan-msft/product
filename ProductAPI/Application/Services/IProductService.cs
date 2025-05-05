using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ProductAPI.Application.DTOs;

namespace ProductAPI.Application.Services
{
    /// <summary>
    /// Service interface for product operations
    /// </summary>
    public interface IProductService
    {
        /// <summary>
        /// Gets all products
        /// </summary>
        /// <returns>Collection of product DTOs</returns>
        Task<IEnumerable<ProductDto>> GetAllProductsAsync();
        
        /// <summary>
        /// Gets a product by its identifier
        /// </summary>
        /// <param name="id">Product identifier</param>
        /// <returns>Product DTO if found; otherwise, null</returns>
        Task<ProductDto?> GetProductByIdAsync(Guid id);
        
        /// <summary>
        /// Creates a new product
        /// </summary>
        /// <param name="addProductDto">Data for creating the product</param>
        /// <returns>The created product DTO</returns>
        Task<ProductDto> CreateProductAsync(AddProductDto addProductDto);
        
        /// <summary>
        /// Updates an existing product
        /// </summary>
        /// <param name="id">Product identifier</param>
        /// <param name="updateProductDto">Data for updating the product</param>
        /// <returns>The updated product DTO if successful; otherwise, null</returns>
        Task<ProductDto?> UpdateProductAsync(Guid id, UpdateProductDto updateProductDto);
        
        /// <summary>
        /// Deletes a product by its identifier
        /// </summary>
        /// <param name="id">Product identifier</param>
        /// <returns>True if the product was deleted; otherwise, false</returns>
        Task<bool> DeleteProductAsync(Guid id);
        
        /// <summary>
        /// Searches for products based on criteria
        /// </summary>
        /// <param name="searchProductDto">Search criteria</param>
        /// <returns>Collection of matching product DTOs</returns>
        Task<IEnumerable<ProductDto>> SearchProductsAsync(SearchProductDto searchProductDto);
    }
}