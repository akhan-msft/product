# Azure Container Apps Lab Setup Guide

This guide will walk you through setting up a new Azure Container Apps (ACA) environment, using an existing Azure Container Registry (ACR), and deploying two container apps (.NET API and React SPA) using Azure CLI in PowerShell.

---

## Prerequisites

- Azure CLI installed ([Install Guide](https://docs.microsoft.com/en-us/cli/azure/install-azure-cli))
- Logged in to Azure:  
  ```powershell
  az login
  ```
- Existing Azure Container Registry (ACR) in a separate resource group

---

## 1. Set Environment Variables

Replace the values as needed.

```powershell
$RESOURCE_GROUP = "aca-lab-rg"
$LOCATION = "westus"
$ACA_ENV_NAME = "aca-lab-env"
$ACR_NAME = ""           
$ACR_RG = ""      
$DOTNET_APP_NAME = "productapi-app"
$REACT_APP_NAME = "productclient-app"
```

---

## 2. Create a New Resource Group

```powershell
az group create --name $RESOURCE_GROUP --location $LOCATION
```

---

## 3. Create a New Azure Container Apps Environment

```powershell
az containerapp env create `
  --name $ACA_ENV_NAME `
  --resource-group $RESOURCE_GROUP `
  --location $LOCATION
```

---

## 4. Log in to Azure Container Registry (ACR)

```powershell
az acr login --name $ACR_NAME -g $ACR_RG 
```

---

## 5. Build and Push Docker Images to ACR

Navigate to each app's directory and build/push using Azure CLI:

### .NET API App

```powershell
cd .\ProductAPI
az acr build --registry $ACR_NAME -g $ACR_RG --image productapi-app:latest .
cd ..
```

### React SPA App

```powershell
cd .\ProductClient
az acr build --registry $ACR_NAME -g $ACR_RG --image productclient-app:latest .
cd ..
```

---

## 6. Get ACR Login Server

```powershell
$ACR_LOGIN_SERVER = az acr show --name $ACR_NAME --resource-group $ACR_RG --query "loginServer" -o tsv
```

---

## 7. Create Container Apps in the ACA Environment

### .NET API Container App

```powershell
az containerapp create `
  --name $DOTNET_APP_NAME `
  --resource-group $RESOURCE_GROUP `
  --environment $ACA_ENV_NAME `
  --image "$ACR_LOGIN_SERVER/productapi-app:latest" `
  --cpu 1.0 `
  --memory 2.0Gi `
  --min-replicas 1 `
  --max-replicas 4 `
  --ingress external `
  --target-port 80
```

### React SPA Container App    --registry-server $ACR_LOGIN_SERVER `

```powershell
az containerapp create `
  --name $REACT_APP_NAME `
  --resource-group $RESOURCE_GROUP `
  --environment $ACA_ENV_NAME `
  --image "$ACR_LOGIN_SERVER/productclient-app:latest" `
  --cpu 1.0 `
  --memory 2.0Gi `
  --min-replicas 1 `
  --max-replicas 4 `
  --ingress external `
  --target-port 80
```

---

## 8. (Optional) Show Container App Endpoints

```powershell
az containerapp show --name $DOTNET_APP_NAME --resource-group $RESOURCE_GROUP --query "properties.configuration.ingress.fqdn" -o tsv
az containerapp show --name $REACT_APP_NAME --resource-group $RESOURCE_GROUP --query "properties.configuration.ingress.fqdn" -o tsv
```

---
## Optional - to update an ACA to a newer image
```powershell
az containerapp update `
  --name $DOTNET_APP_NAME `
  --resource-group $RESOURCE_GROUP `
  --image "$ACR_LOGIN_SERVER/productclient-app:latest" `
  --set imagePullPolicy=Always
```

## Add an HTTP scaling rule for number of requests
```powershell
az containerapp update `
  --name $DOTNET_APP_NAME `
  --resource-group $RESOURCE_GROUP `
  --scale-rule-name http-concurrency-rule `
  --scale-rule-http-concurrency 5
```

## Notes


---