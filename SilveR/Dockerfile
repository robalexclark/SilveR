# syntax=docker/dockerfile:1.6

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base-deps

ENV DEBIAN_FRONTEND=noninteractive \
    LANG=en_US.UTF-8 \
    LANGUAGE=en_US:en \
    LC_ALL=en_US.UTF-8 \
    ASPNETCORE_URLS=http://0.0.0.0:5000 \
    TMPDIR=/tmp/R/tmp

RUN apt-get update \
    && apt-get install -y --no-install-recommends \
        locales \
        ca-certificates \
        curl \
    && sed -i '/en_US.UTF-8/s/^# //g' /etc/locale.gen \
    && locale-gen en_US.UTF-8 \
    && rm -rf /var/lib/apt/lists/*

RUN mkdir -p /tmp/R/tmp && chmod -R 777 /tmp/R

RUN apt-get update \
    && apt-get install -y --no-install-recommends \
        dirmngr \
        software-properties-common \
    && curl -fsSL https://cloud.r-project.org/bin/linux/ubuntu/marutter_pubkey.asc \
        -o /etc/apt/trusted.gpg.d/cran_ubuntu_key.asc \
    && add-apt-repository "deb https://cloud.r-project.org/bin/linux/ubuntu noble-cran40/" \
    && rm -rf /var/lib/apt/lists/*

FROM base-deps AS r-build

RUN apt-get update \
    && apt-get install -y --no-install-recommends \
        r-base-dev=4.6.1-1.2404.1 \
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
        libnode-dev \
        liblapack-dev \
        libblas-dev \
        gfortran \
    && rm -rf /var/lib/apt/lists/*

COPY ./setup/RPackagesInstallDocker.R /tmp/RPackagesInstallDocker.R

RUN perl -0pi -e 's/^\x{feff}//; s/\r\n/\n/g' /tmp/RPackagesInstallDocker.R

RUN TMPDIR=/tmp/R/tmp R_LIBS_SITE=/opt/R/site-library Rscript /tmp/RPackagesInstallDocker.R \
    && rm /tmp/RPackagesInstallDocker.R

FROM base-deps AS base

RUN apt-get update \
    && apt-get install -y --no-install-recommends \
        r-base=4.6.1-1.2404.1 \
        libcurl4t64 \
        libssl3t64 \
        libxml2 \
        libjpeg-turbo8 \
        libpng16-16t64 \
        libtiff6 \
        libharfbuzz0b \
        libfribidi0 \
        libfontconfig1 \
        libudunits2-0 \
        libpq5 \
        libnode109 \
        liblapack3 \
        libblas3 \
    && rm -rf /var/lib/apt/lists/*

ENV R_LIBS_SITE=/opt/R/site-library

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src/SilveR
COPY SilveR.csproj ./
RUN dotnet restore ./SilveR.csproj
COPY . .
RUN dotnet build "./SilveR.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./SilveR.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
EXPOSE 5000
COPY --from=r-build /opt/R/site-library/ /opt/R/site-library/
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "SilveR.dll"]
