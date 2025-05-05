using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ProductAPI.Infrastructure.Configuration;

namespace ProductAPI.Infrastructure.Services
{
    /// <summary>
    /// Background service to initialize the Cosmos DB database and container during application startup
    /// </summary>
    public class DatabaseInitializationService : IHostedService
    {
        private readonly ILogger<DatabaseInitializationService> _logger;
        private readonly DatabaseSettings _databaseSettings;

        /// <summary>
        /// Initializes a new instance of the <see cref="DatabaseInitializationService"/> class.
        /// </summary>
        /// <param name="options">Database configuration options</param>
        /// <param name="logger">Logger instance</param>
        public DatabaseInitializationService(
            IOptions<DatabaseSettings> options,
            ILogger<DatabaseInitializationService> logger)
        {
            _databaseSettings = options?.Value ?? throw new ArgumentNullException(nameof(options));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Starts the service and initializes the database if necessary
        /// </summary>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Task representing the asynchronous operation</returns>
        public async Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Database initialization service starting");
            
            if (!_databaseSettings.UseCosmosDb)
            {
                _logger.LogInformation("Cosmos DB is not enabled, skipping initialization");
                return;
            }

            try
            {
                var cosmosDbSettings = _databaseSettings.CosmosDb;
                
                if (string.IsNullOrEmpty(cosmosDbSettings.ConnectionString))
                {
                    _logger.LogWarning("Cosmos DB connection string is not configured");
                    return;
                }

                _logger.LogInformation("Initializing Cosmos DB database {DatabaseName} and container {ContainerName}",
                    cosmosDbSettings.DatabaseName,
                    cosmosDbSettings.ContainerName);

                using var client = new CosmosClient(cosmosDbSettings.ConnectionString);
                
                // Create database if it doesn't exist
                var databaseResponse = await client.CreateDatabaseIfNotExistsAsync(
                    cosmosDbSettings.DatabaseName,
                    ThroughputProperties.CreateAutoscaleThroughput(1000), // Minimal autoscale throughput
                    cancellationToken: cancellationToken);
                
                _logger.LogInformation("Database {DatabaseName} initialized with status code {StatusCode}",
                    cosmosDbSettings.DatabaseName,
                    databaseResponse.StatusCode);
                
                // Create container if it doesn't exist
                var containerProperties = new ContainerProperties
                {
                    Id = cosmosDbSettings.ContainerName,
                    PartitionKeyPath = cosmosDbSettings.PartitionKeyPath
                };
                
                var containerResponse = await databaseResponse.Database.CreateContainerIfNotExistsAsync(
                    containerProperties,
                    cancellationToken: cancellationToken);
                
                _logger.LogInformation("Container {ContainerName} initialized with status code {StatusCode}",
                    cosmosDbSettings.ContainerName,
                    containerResponse.StatusCode);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error initializing Cosmos DB database");
            }
        }

        /// <summary>
        /// Stops the service
        /// </summary>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Task representing the asynchronous operation</returns>
        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Database initialization service stopping");
            return Task.CompletedTask;
        }
    }
}