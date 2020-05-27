

#------------------------------------------------------------------------------------------------------------------------------------------------
# Loading R packages 
#Open R (that you have just installed) 
#Cut and paste the text below into the R console window (next to the > symbol). It should run automatically. 
#------------------------------------------------------------------------------------------------------------------------------------------------
#This code may generate error and warning messages on the log file, please ignore
install.packages("R2HTML", repo="https://www.stats.bris.ac.uk/R/")
no
install.packages("ggplot2", repo="https://www.stats.bris.ac.uk/R/") 
no
install.packages("ggrepel", repo="https://www.stats.bris.ac.uk/R/")
no
install.packages("plyr", repo="https://www.stats.bris.ac.uk/R/")
no
install.packages("reshape", repo="https://www.stats.bris.ac.uk/R/")
no
install.packages("GGally", repo="https://www.stats.bris.ac.uk/R/")
no
install.packages("proto", repo="https://www.stats.bris.ac.uk/R/")
no
install.packages("mvtnorm", repo="https://www.stats.bris.ac.uk/R/")
no
install.packages("coin", repo="https://www.stats.bris.ac.uk/R/")
no
install.packages("Exact", repo="https://www.stats.bris.ac.uk/R/")
no
install.packages("dplyr", repo="https://www.stats.bris.ac.uk/R/")
no
install.packages("multcompView", repo="https://www.stats.bris.ac.uk/R/")
no
install.packages("car", repo="https://www.stats.bris.ac.uk/R/")
no
install.packages("emmeans", repo="https://www.stats.bris.ac.uk/R/")
no
install.packages("ggdendro", repo="https://www.stats.bris.ac.uk/R/")
no
install.packages("BiocManager", repo="https://www.stats.bris.ac.uk/R/")
no
install.packages("contrast", repo="https://www.stats.bris.ac.uk/R/")
no
BiocManager::install("mixOmics")
a
no


#------------------------------------------------------------------------------------------------------------------------------------------------
# Testing R installation 
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
