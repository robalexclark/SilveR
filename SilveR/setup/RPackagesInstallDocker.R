sourceLibrary <- "/opt/R/site-library"
dir.create(sourceLibrary, recursive = TRUE, showWarnings = FALSE)
.libPaths(c(sourceLibrary, .Library))

options(repos = c(CRAN = "https://cloud.r-project.org"))
requiredDependencies <- c("Depends", "Imports", "LinkingTo")

packages <- c(
  "R2HTML", "Matrix", "MASS", "ggplot2", "ggrepel", "plyr", "reshape", 
  "GGally", "proto", "coin", "ROCR", "Exact", "dplyr", "multcompView", 
  "car", "emmeans", "detectseparation", "ggdendro", "BiocManager", 
  "Hmisc", "polspline", "kableExtra", "rms", "contrast", "PowerTOST", "mmrm",
  "hassediagrams"
)

install.packages(
  packages,
  lib = sourceLibrary,
  dependencies = requiredDependencies,
  ask = FALSE
)

BiocManager::install(
  "mixOmics",
  lib = sourceLibrary,
  dependencies = requiredDependencies,
  ask = FALSE,
  update = FALSE
)

# Function to check installation
check_install <- function(pkg) {
  if (require(pkg, character.only = TRUE)) {
    message(paste(pkg, "is installed."))
  } else {
    message(paste(pkg, "is NOT installed."))
  }
}

# Check each package
missing_packages <- packages[!vapply(packages, requireNamespace, logical(1), quietly = TRUE)]
if (length(missing_packages) > 0) {
  stop(paste("Required R packages are missing:", paste(missing_packages, collapse = ", ")))
}

lapply(packages, check_install)
