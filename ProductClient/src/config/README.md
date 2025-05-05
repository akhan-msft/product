# Environment Configuration Guide

This application uses environment variables to configure the backend API URL across different environments. This makes the application portable and easy to deploy to multiple environments without code changes.

## Environment Variables

The main environment variable used is:

- `VITE_API_BASE_URL` - The base URL for the API endpoints

## How Environment Variables Are Resolved

1. The application first looks for environment variables injected from the system environment
2. If not found, it looks in the appropriate `.env.[mode]` file based on the build mode
3. If still not found, it uses the default values defined in the `env.ts` file

## Setting Environment Variables for Deployment

### For Azure App Service or Static Web Apps

Add application settings in the Azure Portal or via Azure CLI:

```bash
az webapp config appsettings set --name YourAppName --resource-group YourResourceGroup --settings API_BASE_URL_PRODUCTION="https://your-prod-api.azurewebsites.net/api/products"
```

### For Docker Containers

Pass environment variables when running the container:

```bash
docker run -e API_BASE_URL_PRODUCTION="https://your-prod-api.azurewebsites.net/api/products" -p 8080:80 your-image-name
```

### For GitHub Actions or Azure DevOps Pipelines

Set the environment variables in your CI/CD pipeline:

```yaml
env:
  API_BASE_URL_PRODUCTION: https://your-prod-api.azurewebsites.net/api/products
```

## Local Development

For local development, you can:

1. Use the `.env` file (committed to source control with default development values)
2. Create a `.env.local` file (ignored by Git) with your personal override values
3. Set environment variables directly in your terminal or IDE

## Security Considerations

- Never commit sensitive connection strings, API keys, or credentials to source control
- For production, consider using Azure Key Vault or another secret management solution
- Use managed identities when deploying to Azure services whenever possible