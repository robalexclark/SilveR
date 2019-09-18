chmod -R 754 .

apt-get update
apt-get install libcurl4-openssl-dev -y
apt-get install r-base -y

Rscript RPackagesInstall.R
