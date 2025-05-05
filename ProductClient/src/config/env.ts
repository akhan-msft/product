// Environment-specific configuration
interface EnvironmentConfig {
  apiBaseUrl: string;
  environment: string;
}

// Get current environment
const environment = import.meta.env.MODE || 'development';

// Check for runtime environment variable placeholder that will be replaced at container startup
// This special pattern will be replaced by the container entrypoint script
const RUNTIME_PLACEHOLDER = 'RUNTIME_API_BASE_URL';

// Default API URLs by environment
const defaultApiUrls: Record<string, string> = {
  development: 'http://localhost:5298/api/products',
  staging: 'https://staging-api.yourdomain.com/api/products',
  production: 'https://api.yourdomain.com/api/products'
};

// Load API URL with following priority:
// 1. Runtime injected value (for container deployments)
// 2. Build-time environment variable from Vite
// 3. Default fallback based on environment
const apiUrl = 
  // Check if the runtime placeholder is replaced (indicates we're in a container)
  RUNTIME_PLACEHOLDER !== 'RUNTIME_API_BASE_URL' ? RUNTIME_PLACEHOLDER :
  // Otherwise use build-time variable or fallback
  import.meta.env.VITE_API_BASE_URL || defaultApiUrls[environment] || defaultApiUrls.development;

const config: EnvironmentConfig = {
  apiBaseUrl: apiUrl,
  environment: environment
};

// Log configuration in development only
if (environment === 'development') {
  console.log('Environment config:', config);
}

export default config;