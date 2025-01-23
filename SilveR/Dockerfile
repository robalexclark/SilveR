FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-env

# Set the working directory
WORKDIR /app

# Copy the entire solution
COPY .. ./

# Restore the dependencies for the solution
RUN dotnet restore

# Build the solution
RUN dotnet build -c Release --no-restore


# Keep the container running to allow exec
CMD ["tail", "-f", "/dev/null"]
