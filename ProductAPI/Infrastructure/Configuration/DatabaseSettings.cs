using System;

namespace ProductAPI.Infrastructure.Configuration
{
    /// <summary>
    /// Configuration settings for database access
    /// </summary>
    public class DatabaseSettings
    {
        /// <summary>
        /// Flag to determine if Cosmos DB should be used
        /// </summary>
        public bool UseCosmosDb { get; set; }

        /// <summary>
        /// Cosmos DB specific settings
        /// </summary>
        public CosmosDbSettings CosmosDb { get; set; } = new();
    }

    /// <summary>
    /// Configuration settings for Azure Cosmos DB
    /// </summary>
    public class CosmosDbSettings
    {
        /// <summary>
        /// Connection string to the Azure Cosmos DB account
        /// </summary>
        public string ConnectionString { get; set; } = string.Empty;

        /// <summary>
        /// Name of the database in Cosmos DB
        /// </summary>
        public string DatabaseName { get; set; } = string.Empty;

        /// <summary>
        /// Name of the container in Cosmos DB
        /// </summary>
        public string ContainerName { get; set; } = string.Empty;

        /// <summary>
        /// Path to the partition key in the document
        /// </summary>
        public string PartitionKeyPath { get; set; } = string.Empty;
    }
}