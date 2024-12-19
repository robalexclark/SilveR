chmod -R 754 .

apt-get update
apt-get install -y libcurl4-openssl-dev
apt-get install -y libgsl-dev
apt-get install -y r-base
apt-get install -y cmake

#apt-get install r-base-dev -y

Rscript RPackagesInstall.R
