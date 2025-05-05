using System;
using System.Collections.Generic;

namespace ProductAPI.Domain
{
    /// <summary>
    /// Represents a product entity in the system
    /// </summary>
    public class Product
    {
        /// <summary>
        /// Unique identifier for the product
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Name of the product
        /// </summary>
        public required string Name { get; set; }

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
        public required string Category { get; set; }

        /// <summary>
        /// Tags for search and filtering
        /// </summary>
        public List<string> Tags { get; set; } = new List<string>();

        /// <summary>
        /// Availability of the product
        /// </summary>
        public bool InStock { get; set; } = true;

        /// <summary>
        /// Date when the product was created
        /// </summary>
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Date when the product was last updated
        /// </summary>
        public DateTime? UpdatedAt { get; set; }
    }
}