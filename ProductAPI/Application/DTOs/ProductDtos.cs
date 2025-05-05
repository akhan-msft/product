using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ProductAPI.Application.DTOs
{
    /// <summary>
    /// DTO for creating a new product
    /// </summary>
    public class AddProductDto
    {
        /// <summary>
        /// Name of the product
        /// </summary>
        [Required]
        public required string Name { get; set; }

        /// <summary>
        /// Detailed description of the product
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// Price of the product
        /// </summary>
        [Required]
        [Range(0, double.MaxValue, ErrorMessage = "Price must be a positive number")]
        public decimal Price { get; set; }

        /// <summary>
        /// Category the product belongs to
        /// </summary>
        [Required]
        public required string Category { get; set; }

        /// <summary>
        /// Tags for search and filtering
        /// </summary>
        public List<string>? Tags { get; set; }

        /// <summary>
        /// Availability of the product
        /// </summary>
        public bool InStock { get; set; } = true;
    }

    /// <summary>
    /// DTO for searching products
    /// </summary>
    public class SearchProductDto
    {
        /// <summary>
        /// Search term for product name or description
        /// </summary>
        public string? Query { get; set; }

        /// <summary>
        /// Filter by category
        /// </summary>
        public string? Category { get; set; }
    }

    /// <summary>
    /// DTO for updating an existing product
    /// </summary>
    public class UpdateProductDto
    {
        /// <summary>
        /// Name of the product
        /// </summary>
        public string? Name { get; set; }

        /// <summary>
        /// Detailed description of the product
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// Price of the product
        /// </summary>
        [Range(0, double.MaxValue, ErrorMessage = "Price must be a positive number")]
        public decimal? Price { get; set; }

        /// <summary>
        /// Category the product belongs to
        /// </summary>
        public string? Category { get; set; }

        /// <summary>
        /// Tags for search and filtering
        /// </summary>
        public List<string>? Tags { get; set; }

        /// <summary>
        /// Availability of the product
        /// </summary>
        public bool? InStock { get; set; }
    }

    /// <summary>
    /// DTO for product response data
    /// </summary>
    public class ProductDto
    {
        /// <summary>
        /// Unique identifier for the product
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Name of the product
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Detailed description of the product
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// Price of the product
        /// </summary>
        public decimal Price { get; set; }

        /// <summary>
        /// Category the product belongs to
        /// </summary>
        public string Category { get; set; } = string.Empty;

        /// <summary>
        /// Tags for search and filtering
        /// </summary>
        public List<string> Tags { get; set; } = new List<string>();

        /// <summary>
        /// Availability of the product
        /// </summary>
        public bool InStock { get; set; }

        /// <summary>
        /// Date when the product was created
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// Date when the product was last updated
        /// </summary>
        public DateTime? UpdatedAt { get; set; }
    }
}