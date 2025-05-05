#!/bin/bash

# This script replaces environment variables in the built JavaScript files
# It allows us to inject configuration at container runtime rather than build time

# Find all JS files in the build directory
JS_FILES=$(find /usr/share/nginx/html -type f -name "*.js")

# Replace environment variables in each file
for file in $JS_FILES; do
  # Check if API_BASE_URL environment variable is set
  if [ ! -z "$API_BASE_URL" ]; then
    # Replace the placeholder with the actual value
    sed -i "s|RUNTIME_API_BASE_URL|$API_BASE_URL|g" $file
  fi
  
  # Add more environment variables here if needed
done

echo "Environment variables injected successfully"