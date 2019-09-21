chmod -R 754 .

apt-get update
apt-get install libcurl4-openssl-dev -y
apt-get install libgsl-dev -y
apt-get install r-base=3.5.2-1build1 -y

Rscript RPackagesInstall.R
