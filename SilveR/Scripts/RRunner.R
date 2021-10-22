#===================================================================================================================
#R Libraries

suppressWarnings(library(R2HTML))

#===================================================================================================================
# retrieve args

Args <- commandArgs(TRUE)

#Read in arguments
statdata <- read.csv(Args[3], header = TRUE, sep = ",")

#Print args
print(Args)

#===================================================================================================================

#Setup the html file and associated css file
htmlFile <- sub(".csv", ".html", Args[3]); #determine the file name of the html file
HTMLSetFile(file=htmlFile) 
cssFile <- "r2html.css"
cssFile <- paste("'",cssFile,"'", sep="") #need to enclose in quotes when path has spaces in it
HTMLCSS(CSSfile = cssFile)



#Output HTML header
HTML.title("<bf>InVivoStat Split-Plot Analysis", HR=1, align="left")
