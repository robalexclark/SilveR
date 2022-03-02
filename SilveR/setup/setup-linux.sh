chmod -R 754 .

apt-get update
apt-get install libcurl4-openssl-dev -y
apt-get install libgsl-dev -y
apt-get install r-base -y
apt-get install cmake -y
#apt-get install r-base-dev -y

Rscript RPackagesInstall.R
