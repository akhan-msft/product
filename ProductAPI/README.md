# Product API with Azure Cosmos DB Integration

This API provides CRUD operations for product management with support for multiple database providers. The application can seamlessly switch between an in-memory database (for development/testing) and Azure Cosmos DB (for production) through configuration settings.

## Database Configuration

The API supports two storage options:
1. In-memory database (default)
2. Azure Cosmos DB

### Configuration in appsettings.json

```json
"DatabaseSettings": {
  "UseCosmosDb": false,  // Set to true to use Cosmos DB or false to use in-memory storage
  "CosmosDb": {
    "ConnectionString": "", // Your Cosmos DB connection string goes here
    "DatabaseName": "ProductDb",
    "ContainerName": "Products",
    "PartitionKeyPath": "/Category"
  }
}
```

To switch between database providers:
1. Set `UseCosmosDb` to `true` to use Azure Cosmos DB or `false` to use in-memory database
2. When using Cosmos DB, ensure you have properly configured the connection string

## Creating Azure Cosmos DB Instance

Use the following Azure CLI commands in PowerShell to create a free-tier Azure Cosmos DB instance:

```powershell
# Set variables for resources
$RESOURCE_GROUP = "product-api-rg"
$LOCATION = "westus3"  # Change to your preferred region
$RANDOM_SUFFIX = -join ((48..57) + (97..122) | Get-Random -Count 8 | ForEach-Object {[char]$_})
$COSMOS_ACCOUNT_NAME = "product-api-cosmos-$RANDOM_SUFFIX"  # Unique name with random suffix
$DATABASE_NAME = "ProductDb"
$CONTAINER_NAME = "Products"
$PARTITION_KEY_PATH = "/Category"

# Create resource group
az group create --name $RESOURCE_GROUP --location $LOCATION

# Create Cosmos DB account with free tier
az cosmosdb create `
    --name $COSMOS_ACCOUNT_NAME `
    --resource-group $RESOURCE_GROUP `
    --default-consistency-level Session `
    --enable-analytical-storage false `
    --locations regionName=$LOCATION failoverPriority=0 isZoneRedundant=false

# Create database
az cosmosdb sql database create `
    --account-name $COSMOS_ACCOUNT_NAME `
    --resource-group $RESOURCE_GROUP `
    --name $DATABASE_NAME

# Create container with throughput (400 RU/s for free tier)
az cosmosdb sql container create `
    --account-name $COSMOS_ACCOUNT_NAME `
    --resource-group $RESOURCE_GROUP `
    --database-name $DATABASE_NAME `
    --name $CONTAINER_NAME `
    --partition-key-path $PARTITION_KEY_PATH `
    --throughput 400

# Get the connection string
$CONNECTION_STRING = az cosmosdb keys list `
    --name $COSMOS_ACCOUNT_NAME `
    --resource-group $RESOURCE_GROUP `
    --type connection-strings `
    --query "connectionStrings[?description=='Primary SQL Connection String'].connectionString" -o tsv

Write-Host "Connection String: $CONNECTION_STRING"
```

## Switching Between Database Providers

### To use Azure Cosmos DB:

1. Create a Cosmos DB account using the Azure CLI commands above
2. Copy the connection string from the last command output
3. Open `appsettings.json` and update the following:
   ```json
   "DatabaseSettings": {
     "UseCosmosDb": true,
     "CosmosDb": {
       "ConnectionString": "YOUR_CONNECTION_STRING_HERE",
       "DatabaseName": "ProductDb",
       "ContainerName": "Products",
       "PartitionKeyPath": "/Category"
     }
   }
   ```
4. Restart the application

### To use in-memory database:

1. Open `appsettings.json` and update the following:
   ```json
   "DatabaseSettings": {
     "UseCosmosDb": false,
     "CosmosDb": {
       // Connection settings can remain as is
     }
   }
   ```
2. Restart the application

## Security Notes

For production environments, it's recommended to:

1. Store connection strings in Azure Key Vault or as environment variables
2. Use Managed Identity when possible for Azure services
3. Apply appropriate firewall rules to restrict Cosmos DB access

## Free Tier Limitations

The Azure Cosmos DB free tier offers:
- 1000 RU/s shared throughput
- 25GB of storage
- No SLA

This is sufficient for development and small applications but monitor usage to avoid unexpected charges if exceeding these limits.

## Using Cosmos DB Emulator for Development

For local development without incurring costs, you can use the [Azure Cosmos DB Emulator](https://learn.microsoft.com/en-us/azure/cosmos-db/local-emulator) which provides a local environment that emulates the Azure Cosmos DB service.