# DOCKERFILE FOR BUILDING IMAGE TO RUN INTEGRATION TESTS IN
# syntax=docker/dockerfile:1.6

FROM mcr.microsoft.com/dotnet/sdk:8.0-bookworm-slim AS base-deps

ENV DEBIAN_FRONTEND=noninteractive     LANG=en_US.UTF-8     LANGUAGE=en_US:en     LC_ALL=en_US.UTF-8     TMPDIR=/tmp/R/tmp

RUN apt-get update     && apt-get install -y --no-install-recommends         locales         ca-certificates         curl         gnupg         dirmngr     && sed -i '/en_US.UTF-8/s/^# //g' /etc/locale.gen     && locale-gen en_US.UTF-8     && gpg --batch --keyserver keyserver.ubuntu.com         --recv-keys 95C0FAF38DB3CCAD0C080A7BDC78B2DDEABC47B7     && gpg --batch --export 95C0FAF38DB3CCAD0C080A7BDC78B2DDEABC47B7         | gpg --dearmor --yes --batch -o /usr/share/keyrings/cran-archive-keyring.gpg     && echo "deb [signed-by=/usr/share/keyrings/cran-archive-keyring.gpg] https://cloud.r-project.org/bin/linux/debian bookworm-cran40/"         > /etc/apt/sources.list.d/cran-r.list     && apt-get purge -y --auto-remove gnupg dirmngr     && rm -rf /var/lib/apt/lists/*

RUN mkdir -p /tmp/R/tmp && chmod -R 777 /tmp/R

FROM base-deps AS r-build

RUN apt-get update     && apt-get install -y --no-install-recommends         r-base-dev         cmake         libcurl4-openssl-dev         libxml2-dev         libssl-dev         libjpeg-dev         libpng-dev         libtiff-dev         libharfbuzz-dev         libfribidi-dev         libfontconfig1-dev         libudunits2-dev         libpq-dev         libv8-dev         liblapack-dev         libblas-dev         gfortran     && rm -rf /var/lib/apt/lists/*

COPY ./SilveR/setup/RPackagesInstallDocker.R /tmp/RPackagesInstallDocker.R
RUN perl -0pi -e 's/^\x{feff}//; s/\r\n/\n/g' /tmp/RPackagesInstallDocker.R

RUN TMPDIR=/tmp/R/tmp Rscript /tmp/RPackagesInstallDocker.R     && rm /tmp/RPackagesInstallDocker.R

FROM base-deps AS base

RUN apt-get update     && apt-get install -y --no-install-recommends         r-base         libcurl4         libssl3         libxml2         libjpeg62-turbo         libpng16-16         libtiff6         libharfbuzz0b         libfribidi0         libfontconfig1         libudunits2-0         libpq5         libv8-dev         liblapack3         libblas3     && rm -rf /var/lib/apt/lists/*

FROM base AS test

WORKDIR /app

COPY --from=r-build /usr/lib/R/site-library/ /usr/lib/R/site-library/
COPY --from=r-build /usr/local/lib/R/site-library/ /usr/local/lib/R/site-library/

# Copy the entire solution and prepare for test runs
COPY . ./

RUN --mount=type=cache,target=/root/.nuget/packages dotnet restore
RUN --mount=type=cache,target=/root/.nuget/packages dotnet build -c Release --no-restore

# Keep container running for debugging
CMD ["tail", "-f", "/dev/null"]
