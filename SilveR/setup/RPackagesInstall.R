#------------------------------------------------------------------------------------------------------------------------------------------------
# Instructions to complete the installation – you will only need to do this once!
# Open R (that you have just installed) 
# Cut and paste the "install.packages" R code below - one line at a time - into the R console window (next to the > symbol)
# You may find that R asks you a question during the installation of each R package (this will not happen for many packages)
# We recommend you say "no" to any questions
# The final line of code that installs "mixOmics" may require to to enter "a" (for all) 
#------------------------------------------------------------------------------------------------------------------------------------------------

install.packages("R2HTML")
install.packages("ggplot2") 
install.packages("ggrepel")
install.packages("plyr")
install.packages("reshape")
install.packages("GGally")
install.packages("proto")
install.packages("coin")
install.packages("ROCR")
install.packages("Exact")
install.packages("dplyr")
install.packages("multcompView")
install.packages("car")
install.packages("emmeans")
install.packages("detectseparation")
install.packages("ggdendro")
install.packages("BiocManager")
install.packages("contrast")
install.packages("PowerTOST", repo="https://www.stats.bris.ac.uk/R/")
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
library(coin)
library(ROCR)
library(Exact)
library(dplyr)
library(multcompView)
library(car)
library(emmeans)
library(ggdendro)
library(BiocManager)
library(contrast)
library(mixOmics)
