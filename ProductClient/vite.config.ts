import { defineConfig } from 'vite'
import react from '@vitejs/plugin-react'

// https://vite.dev/config/
export default defineConfig(({ mode }) => {
  // Simplified configuration without environment loading
  return {
    plugins: [react()],
    define: {
      // Define any global constants if needed
      '__APP_ENV__': JSON.stringify(mode)
    },
    server: {
      // Configure proxy if needed for local development
      proxy: {
        // Uncomment and modify if you need to proxy API requests
        // '/api': {
        //   target: 'http://localhost:5298',
        //   changeOrigin: true,
        //   rewrite: (path) => path.replace(/^\/api/, '')
        // }
      }
    }
  }
})
