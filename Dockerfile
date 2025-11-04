# DOCKERFILE FOR BUILDING IMAGE TO RUN INTEGRATION TESTS IN
# syntax=docker/dockerfile:1.6
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-env

ENV DEBIAN_FRONTEND=noninteractive

# Configure locale
RUN apt-get update && apt-get install -y --no-install-recommends locales \
    && sed -i '/en_US.UTF-8/s/^# //g' /etc/locale.gen \
    && locale-gen en_US.UTF-8 \
    && rm -rf /var/lib/apt/lists/*
ENV LANG=en_US.UTF-8 LANGUAGE=en_US:en LC_ALL=en_US.UTF-8

# Install R runtime and dependencies
RUN apt-get update \
    && apt-get install -y --no-install-recommends \
        ca-certificates \
        curl \
        gnupg \
        dirmngr \
    && gpg --batch --keyserver keyserver.ubuntu.com \
        --recv-keys 95C0FAF38DB3CCAD0C080A7BDC78B2DDEABC47B7 \
    && gpg --batch --export 95C0FAF38DB3CCAD0C080A7BDC78B2DDEABC47B7 \
        | gpg --dearmor --yes --batch -o /usr/share/keyrings/cran-archive-keyring.gpg \
    && echo "deb [signed-by=/usr/share/keyrings/cran-archive-keyring.gpg] https://cloud.r-project.org/bin/linux/debian bookworm-cran40/" \
        > /etc/apt/sources.list.d/cran-r.list \
    && apt-get update \
    && apt-get install -y --no-install-recommends \
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
    && rm -rf /var/lib/apt/lists/* /root/.gnupg

# Prepare writable temp area for R installs
RUN mkdir -p /tmp/R/tmp && chmod -R 777 /tmp/R
ENV TMPDIR=/tmp/R/tmp

# Install R packages
COPY ./SilveR/setup/RPackagesInstallDocker.R /tmp/RPackagesInstallDocker.R
RUN python3 - <<'PY'
from pathlib import Path
p = Path("/tmp/RPackagesInstallDocker.R")
d = p.read_bytes()
if d.startswith(b'\xef\xbb\xbf'):
    d = d[3:]
p.write_bytes(d.replace(b'\r\n', b'\n'))
PY
RUN TMPDIR=/tmp/R/tmp Rscript /tmp/RPackagesInstallDocker.R

# Set working directory
WORKDIR /app

# Copy the entire solution
COPY . ./

# Restore and build .NET solution
RUN --mount=type=cache,target=/root/.nuget/packages dotnet restore
RUN --mount=type=cache,target=/root/.nuget/packages dotnet build -c Release --no-restore

# Keep container running for debugging
CMD ["tail", "-f", "/dev/null"]
