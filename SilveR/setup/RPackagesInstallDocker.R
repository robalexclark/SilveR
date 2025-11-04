install.packages("R2HTML", dependencies = TRUE, ask = FALSE)
install.packages("https://cran.r-project.org/src/contrib/Archive/Matrix/Matrix_1.6-0.tar.gz", repos = NULL, type = "source", dependencies = TRUE, ask = FALSE)
install.packages("https://cran.r-project.org/src/contrib/Archive/MASS/MASS_7.3-59.tar.gz", repos = NULL, type = "source", dependencies = TRUE, ask = FALSE)
install.packages("ggplot2", dependencies = TRUE, ask = FALSE) 
install.packages("ggrepel", dependencies = TRUE, ask = FALSE)
install.packages("plyr", dependencies = TRUE, ask = FALSE)
install.packages("reshape", dependencies = TRUE, ask = FALSE)
install.packages("GGally", dependencies = TRUE, ask = FALSE)
install.packages("proto", dependencies = TRUE, ask = FALSE)
install.packages("ROCR", dependencies = TRUE, ask = FALSE)
install.packages("coin", dependencies = TRUE, ask = FALSE)
install.packages("Exact", dependencies = TRUE, ask = FALSE)
install.packages("dplyr", dependencies = TRUE, ask = FALSE)
install.packages("multcompView", dependencies = TRUE, ask = FALSE)
install.packages("car", dependencies = TRUE, ask = FALSE)
install.packages("emmeans", dependencies = TRUE, ask = FALSE)
install.packages("detectseparation", dependencies = TRUE, ask = FALSE)
install.packages("ggdendro", dependencies = TRUE, ask = FALSE)
install.packages("BiocManager", dependencies = TRUE, ask = FALSE)
install.packages("Hmisc", dependencies = TRUE, ask = FALSE)
install.packages("polspline", dependencies = TRUE, ask = FALSE)
install.packages("kableExtra", dependencies = TRUE, ask = FALSE)
install.packages("https://cran.r-project.org/src/contrib/Archive/rms/rms_6.6-0.tar.gz", repos = NULL, type = "source", dependencies = TRUE, ask = FALSE)
install.packages("contrast", dependencies = TRUE, ask = FALSE)
install.packages("PowerTOST", dependencies = TRUE, ask = FALSE)
install.packages("mmrm", dependencies = TRUE, ask = FALSE)

BiocManager::install("mixOmics", dependencies = TRUE, ask = FALSE)


# List of packages to check
packages <- c(
  "R2HTML", "Matrix", "MASS", "ggplot2", "ggrepel", "plyr", "reshape", 
  "GGally", "proto", "coin", "ROCR", "Exact", "dplyr", "multcompView", 
  "car", "emmeans", "detectseparation", "ggdendro", "BiocManager", 
  "Hmisc", "polspline", "kableExtra", "rms", "contrast", "PowerTOST", "mmrm"
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
lapply(packages, check_install)
