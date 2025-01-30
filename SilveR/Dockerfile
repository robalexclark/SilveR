# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base

WORKDIR /app
EXPOSE 5000

# Install R, build tools, and dependencies for compiling packages
RUN apt-get update && apt-get install -y --no-install-recommends \
    r-base-dev \
    cmake \
    libcurl4-openssl-dev \
    libxml2-dev \
    libssl-dev \
    libjpeg-dev \
    libpng-dev \
    libtiff-dev \
    libharfbuzz-dev \
    libfribidi-dev \
    libfontconfig1-dev \
    libudunits2-dev \
    libpq-dev \
    libv8-dev \
    liblapack-dev \
    libblas-dev \
    gfortran \
    && apt-get clean \
    && rm -rf /var/lib/apt/lists/*

# Create writable temporary and library directories for R
RUN mkdir -p /tmp/R/tmp && chmod -R 777 /tmp/R

# Set TMPDIR globally
ENV TMPDIR=/tmp/R/tmp

# Copy the R script for installing packages globally
COPY SilveR/setup/RPackagesInstallDocker.R /tmp/RPackagesInstallDocker.R

# Install R packages globally (with TMPDIR explicitly set)
RUN TMPDIR=/tmp/R/tmp Rscript /tmp/RPackagesInstallDocker.R

# Other stages (build, publish, final) remain unchanged
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["SilveR/SilveR.csproj", "SilveR/"]
RUN dotnet restore "./SilveR/SilveR.csproj"
COPY . .
WORKDIR "/src/SilveR"
RUN dotnet build "./SilveR.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./SilveR.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "SilveR.dll"]
