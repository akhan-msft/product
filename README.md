# Product Workspace

This workspace contains two main projects:

- **ProductAPI**: A .NET 8 Web API for managing products, supporting both in-memory and Azure Cosmos DB storage.
- **ProductClient**: A React + TypeScript frontend for interacting with the ProductAPI.

---

## 1. ProductAPI

**Location:** [`ProductAPI/`](ProductAPI/)

### Features

- RESTful CRUD endpoints for products
- Supports in-memory and Azure Cosmos DB storage (configurable)
- Swagger/OpenAPI documentation
- Health checks

### Build & Run

#### Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- (Optional) [Docker](https://www.docker.com/) for containerized runs

#### Local Development

```sh
cd ProductAPI
dotnet build
dotnet run
```

The API will be available at `http://localhost:5298` by default.

#### Docker

```sh
cd ProductAPI
docker build -t product-api .
docker run -p 5298:80 product-api
```

#### Database Configuration

Edit [`ProductAPI/appsettings.json`](ProductAPI/appsettings.json):

- To use **in-memory** storage:
  ```json
  "DatabaseSettings": {
    "UseCosmosDb": false,
    ...
  }
  ```
- To use **Azure Cosmos DB**:
  ```json
  "DatabaseSettings": {
    "UseCosmosDb": true,
    "CosmosDb": {
      "ConnectionString": "<your-connection-string>",
      "DatabaseName": "ProductDb",
      "ContainerName": "Products",
      "PartitionKeyPath": "/Category"
    }
  }
  ```

---

## 2. ProductClient

**Location:** [`ProductClient/`](ProductClient/)

### Features

- React + TypeScript SPA
- Connects to ProductAPI for product management

### Build & Run

#### Prerequisites

- [Node.js (v18+ recommended)](https://nodejs.org/)
- [npm](https://www.npmjs.com/) or [yarn](https://yarnpkg.com/)

#### Local Development

```sh
cd ProductClient
npm install
npm run dev
```

The app will be available at `http://localhost:5173` (or as shown in the terminal).

#### Production Build

```sh
cd ProductClient
npm run build
```

---

## API Endpoint Configuration in ProductClient

The ProductClient needs to know where to find the ProductAPI.

- **Default location:** [`ProductClient/src/constants/apiConfig.ts`](ProductClient/src/constants/apiConfig.ts)
  ```ts
  export const API_BASE_URL = 'http://localhost:5298/api/products';
  ```

- **To change the API endpoint:**
  - Edit the `API_BASE_URL` in [`src/constants/apiConfig.ts`](ProductClient/src/constants/apiConfig.ts).
  - Or, for advanced setups, use environment variables as described in [`ProductClient/src/config/README.md`](ProductClient/src/config/README.md).

**For deployment:**  
Set the API endpoint to your deployed ProductAPI URL.

---

## Running Both Projects Together

- Use the provided [`ProductAPI/docker-compose.yml`](ProductAPI/docker-compose.yml) to run both API and client in containers:
  ```sh
  cd ProductAPI
  docker-compose up --build
  ```
- The client will wait for the API to be healthy before starting.

---

## Additional Notes

- **Swagger UI** is available at `/swagger` on the API server.
- For Cosmos DB, see [`ProductAPI/README.md`](ProductAPI/README.md) for Azure setup instructions.
- Never commit secrets or production connection strings to source control.

---