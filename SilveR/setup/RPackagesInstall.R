#------------------------------------------------------------------------------------------------------------------------------------------------
# Instructions to complete the installation – you will only need to do this once!
# Open R (that you have just installed) 
# Cut and paste the "install.packages" R code below - one line at a time - into the R console window (next to the > symbol)
# You may find that R asks you a question during the installation of each R package (this will not happen for many packages)
# We recommend you say "no" to any questions
# The final line of code that installs "mixOmics" may require to to enter "a" (for all) 
#------------------------------------------------------------------------------------------------------------------------------------------------

options(install.packages.check.source = "no")
options(install.packages.compile.from.source = "never")

install.packages("R2HTML", repo="https://www.stats.bris.ac.uk/R/")
install.packages("ggplot2", repo="https://www.stats.bris.ac.uk/R/") 
install.packages("ggrepel", repo="https://www.stats.bris.ac.uk/R/")
install.packages("plyr", repo="https://www.stats.bris.ac.uk/R/")
install.packages("reshape", repo="https://www.stats.bris.ac.uk/R/")
install.packages("GGally", repo="https://www.stats.bris.ac.uk/R/")
install.packages("proto", repo="https://www.stats.bris.ac.uk/R/")
install.packages("mvtnorm", repo="https://www.stats.bris.ac.uk/R/")
install.packages("coin", repo="https://www.stats.bris.ac.uk/R/")
install.packages("Exact", repo="https://www.stats.bris.ac.uk/R/")
install.packages("dplyr", repo="https://www.stats.bris.ac.uk/R/")
install.packages("multcompView", repo="https://www.stats.bris.ac.uk/R/")
install.packages("car", repo="https://www.stats.bris.ac.uk/R/")
install.packages("emmeans", repo="https://www.stats.bris.ac.uk/R/")
install.packages("detectseparation", repo="https://www.stats.bris.ac.uk/R/")
install.packages("ggdendro", repo="https://www.stats.bris.ac.uk/R/")
install.packages("BiocManager", repo="https://www.stats.bris.ac.uk/R/")
install.packages("contrast", repo="https://www.stats.bris.ac.uk/R/")
BiocManager::install("mixOmics")



#------------------------------------------------------------------------------------------------------------------------------------------------
# Testing your R installation 
#------------------------------------------------------------------------------------------------------------------------------------------------
#If the installation has been performed correctly, the following commands should not generate any error messages
#If errors are found when running these commands please contact invivostat@hotmail.co.uk
#------------------------------------------------------------------------------------------------------------------------------------------------
library(R2HTML)
library(ggplot2) 
library(ggrepel)
library(plyr)
library(reshape)
library(GGally)
library(proto)
library(mvtnorm)
library(coin)
library(Exact)
library(dplyr)
library(multcompView)
library(car)
library(emmeans)
library(ggdendro)
library(BiocManager)
library(contrast)
library(mixOmics)
