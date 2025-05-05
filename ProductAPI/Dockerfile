FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy only the project files first to leverage Docker layer caching
COPY ["ProductAPI.csproj", "./"]
RUN dotnet restore "ProductAPI.csproj"

# Copy the rest of the source code
COPY . .
RUN dotnet build "ProductAPI.csproj" -c Release -o /app/build
RUN dotnet publish "ProductAPI.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Use the smaller aspnet runtime image for the final stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final



# Create a non-root user for security
RUN adduser --disabled-password --gecos "" appuser

WORKDIR /app
COPY --from=build /app/publish .

# Set proper ownership
##RUN chown -R appuser:appuser /app
#USER appuser

# Configure ASP.NET Core
ENV ASPNETCORE_URLS=http://+:80
# Don't use Development in production containers
#ENV ASPNETCORE_ENVIRONMENT=Production
# Enable Docker container detection
#ENV DOTNET_RUNNING_IN_CONTAINER=true
# Set for better performance in containerized environments
#ENV DOTNET_gcServer=0

# Health check to verify application is running properly
#HEALTHCHECK --interval=30s --timeout=3s --retries=3 \
#  CMD curl -f http://localhost:80/health || exit 1

# Expose the port
EXPOSE 80

ENTRYPOINT ["dotnet", "ProductAPI.dll"]