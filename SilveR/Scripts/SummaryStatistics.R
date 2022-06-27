#===================================================================================================================
#R Libraries

suppressWarnings(library(R2HTML))

#===================================================================================================================
# retrieve args

Args <- commandArgs(TRUE)

#Read in arguments
statdata <- read.csv(Args[3], header = TRUE, sep = ",")
csResponses <- Args[4]
transformation <- tolower(Args[5])
firstCat <- Args[6]
secondCat <- Args[7]
thirdCat <- Args[8]
fourthCat <- Args[9]
mean <- Args[10]
N <- Args[11]
sum <- Args[12]
StDev <- Args[13]
Variances <- Args[14]
StErr <- Args[15]
MinMax <- Args[16]
MedianQuartile <- Args[17]
CoeffVariation <- Args[18]
confidenceLimits <- Args[19]
CIval2 <- as.numeric(Args[20])
CIval <- CIval2 / 100
NormalProbabilityPlot <- Args[21]
ByCategoriesAndOverall <- Args[22]

#source(paste(getwd(),"/Common_Functions.R", sep=""))

#Print args
if (Diplayargs == "Y"){
	print(Args)
}

#===================================================================================================================
#Setup the html file and associated css file
htmlFile <- sub(".csv", ".html", Args[3]);
#determine the file name of the html file
HTMLSetFile(file = htmlFile)
.HTML.file = htmlFile

#===================================================================================================================
#Parameter setup

#Graphics parameter setup
graphdata <- statdata
ReferenceLine <- "NULL"
XLimLow <- "NULL"
XLimHigh <- "NULL"
YLimLow <- "NULL"
YLimHigh <- "NULL"

#Breakdown the list of responses
resplist <- c()
tempChanges <- strsplit(csResponses, ",")
expectedChanges <- c(0)
for (i in 1:length(tempChanges[[1]])) {
    expectedChanges[length(expectedChanges) + 1] = (tempChanges[[1]][i])
}
for (j in 1:(length(expectedChanges) - 1)) {
    resplist[j] = expectedChanges[j + 1]
}
resplength <- length(resplist)

# Create the normal probability plot titles
resplistqq <- resplist

#replace illegal characters in variable names
for (i in 1:10) {
    resplistqq <- namereplace(resplistqq)
}

#===================================================================================================================
#Titles and description
#===================================================================================================================
#Module Title
Title <-paste(branding, " Summary Statistics", sep="")
HTML.title(Title, HR = 1, align = "left")

#Software developement version warning
if (Betawarn == "Y") {
	HTML.title("Warning", HR=2, align="left")
	HTML(BetaMessage, align="left")
}

HTML.title("Variable selection", HR=2, align="left")
add2<-c("Response")
if (length(expectedChanges) >2) {
	add2 <- paste(add2, "s", sep="")
} 
add2 <- paste(add2, " ", csResponses,  sep="")
if (length(expectedChanges) >2) {
	add2 <- paste(add2, " are", sep="")
} else {
	add2 <- paste(add2, " is", sep="")
}
add2 <- paste(add2, " analysed in this module", sep="")
if (firstCat != "NULL" || secondCat != "NULL" || thirdCat != "NULL" || fourthCat != "NULL") {
	add2 <- paste(add2, ", with results categorised by factor", sep="")
}
if ( (firstCat != "NULL" && secondCat != "NULL") || (firstCat != "NULL" && thirdCat != "NULL") || (firstCat != "NULL" && fourthCat != "NULL") || (secondCat != "NULL" && thirdCat != "NULL") || (secondCat != "NULL" && fourthCat != "NULL") || (thirdCat != "NULL" && fourthCat != "NULL") ) {
	add2 <- paste(add2, "s", sep="")
}
if (firstCat != "NULL") {
	add2 <- paste(add2, " ", firstCat, sep="")
}
if (secondCat != "NULL") {
	add2 <- paste(add2, ", ", secondCat, sep="")
}
if (thirdCat != "NULL") {
	add2 <- paste(add2, ", ", thirdCat, sep="")
}
if (fourthCat != "NULL") {
	add2 <- paste(add2, ", ", fourthCat, sep="")
}
if (transformation == "none")
{
	add2 <- paste(add2, ".", sep="")
} else {
	add2<-paste(add2, ". The response", sep="")
	if (length(expectedChanges) >2) {
		add2 <- paste(add2, "s have been ", sep="")
	} else {
		add2 <- paste(add2, " has been ", sep="")
	}
	add2<-paste(add2, " ", transformation, " transformed prior to analysis.", sep="")
}
HTML(add2, align="left")

#===================================================================================================================
#Categorisation analysis
#===================================================================================================================
if ((firstCat != "NULL" || secondCat != "NULL" || thirdCat != "NULL" || fourthCat != "NULL") && resplength > 1 ) {
      #Overall title
      HTML.title("Categorised summary statistics", HR=2, align="left")
}

if (firstCat != "NULL" || secondCat != "NULL" || thirdCat != "NULL" || fourthCat != "NULL") {
	for (i in 1:resplength) {
		csResponses <- resplist[i]
		#Setting up parameters and vectors
	        length <- length(unique(levels(as.factor(statdata$catfact))))
	        tablenames <- c(levels(as.factor(statdata$catfact)))
	        table <- c(1:length)
	        for (i in 1:length) {
	        	table[i] = " "
	        }
            	vectormean <- c(1:length)
            	vectorN <- c(1:length)
			vectorsum  <- c(1:length)
            	vectorStDev <- c(1:length)
            	vectorVariances <- c(1:length)
            	vectorStErr <- c(1:length)
            	vectorMin <- c(1:length)
            	vectorMax <- c(1:length)
            	vectorMedian <- c(1:length)
            	vectorLQ <- c(1:length)
            	vectorUQ <- c(1:length)
            	vectorCoeffVariation <- c(1:length)
            	vectorUCI <- c(1:length)
            	vectorLCI <- c(1:length)

		#Generating the summary stats
            	for (i in 1:length) {
                	sub <- subset(statdata, statdata$catfact == unique(levels(as.factor(statdata$catfact)))[i])
                	sub2 <- data.frame(sub)
                	if (mean == "Y") {
                	    vectormean[i] = mean(eval(parse(text = paste("sub2$", csResponses))), na.rm = TRUE)
                	}
                	if (N == "Y") {
                    		tempy <- na.omit(eval(parse(text = paste("sub2$", csResponses))))
                    		vectorN[i] = length(tempy)
                	}
                	if (sum == "Y") {
                	    vectorsum[i] = sum(eval(parse(text = paste("sub2$", csResponses))), na.rm = TRUE)
                	}
                	if (StDev == "Y") {
                    		vectorStDev[i] = sd(eval(parse(text = paste("sub2$", csResponses))), na.rm = TRUE)
                	}
                	if (Variances == "Y") {
                    		vectorVariances[i] = var(eval(parse(text = paste("sub2$", csResponses))), na.rm = TRUE)
                	}
                	if (StErr == "Y") {
                    		tempy <- na.omit(eval(parse(text = paste("sub2$", csResponses))))
                    		vectorStErr[i] = sd(eval(parse(text = paste("sub2$", csResponses))), na.rm = TRUE) / (length(tempy)) ** (0.5)
                	}
                	if (MinMax == "Y") {
                    		vectorMin[i] = suppressWarnings(min(eval(parse(text = paste("sub2$", csResponses))), na.rm = TRUE))
                	}
                	if (MinMax == "Y") {
                    		vectorMax[i] = suppressWarnings(max(eval(parse(text = paste("sub2$", csResponses))), na.rm = TRUE))
                	}
                	if (MedianQuartile == "Y") {
                    		vectorMedian[i] = median(eval(parse(text = paste("sub2$", csResponses))), na.rm = TRUE)
                	}
               	 	if (MedianQuartile == "Y") {
                    		vectorLQ[i] = quantile(eval(parse(text = paste("sub2$", csResponses))), 0.25, type = 2, na.rm = TRUE)
                	}
                	if (MedianQuartile == "Y") {
                    		vectorUQ[i] = quantile(eval(parse(text = paste("sub2$", csResponses))), 0.75, type = 2, na.rm = TRUE)
                	}
                	if (CoeffVariation == "Y") {
                    		vectorCoeffVariation[i] = 100 * sd(eval(parse(text = paste("sub2$", csResponses))), na.rm = TRUE) / abs(mean(eval(parse(text = paste("sub2$", csResponses))), na.rm = TRUE))
                	}
                	if (confidenceLimits == "Y") {
                    		tempy <- na.omit(eval(parse(text = paste("sub2$", csResponses))))
                    		vectorLCI[i] = mean(eval(parse(text = paste("sub2$", csResponses))), na.rm = TRUE) - qt(1 - (1 - CIval) / 2, (length(tempy) - 1)) * sd(eval(parse(text = paste("sub2$", csResponses))), na.rm = TRUE) / (length(tempy)) ** (0.5)
                	}
                	if (confidenceLimits == "Y") {
                    		tempy <- na.omit(eval(parse(text = paste("sub2$", csResponses))))
                    		vectorUCI[i] = mean(eval(parse(text = paste("sub2$", csResponses))), na.rm = TRUE) + qt(1 - (1 - CIval) / 2, (length(tempy) - 1)) * sd(eval(parse(text = paste("sub2$", csResponses))), na.rm = TRUE) / (length(tempy)) ** (0.5)
                	}
            	}

            	#Generating final table dataset
            	if (mean == "Y") {
                	vectormean <- format(round(vectormean, 4), nsmall = 4, scientific = FALSE)
                	table <- cbind(table, vectormean)
            	}
            	if (N == "Y") {
                	#vectorN<-format(round(vectormean, 4), nsmall=4, scientific=FALSE)
                	table <- cbind(table, vectorN)
            	}
            	if (sum == "Y") {
                	vectorsum <- format(round(vectorsum, 4), nsmall = 4, scientific = FALSE)
                	table <- cbind(table, vectorsum)
            	}
            	if (Variances == "Y") {
                	vectorVariances <- format(round(vectorVariances, 4), nsmall = 4, scientific = FALSE)
                	table <- cbind(table, vectorVariances)
            	}
            	if (StDev == "Y") {
                	vectorStDev <- format(round(vectorStDev, 4), nsmall = 4, scientific = FALSE)
                	table <- cbind(table, vectorStDev)
            	}
            	if (StErr == "Y") {
                	vectorStErr <- format(round(vectorStErr, 4), nsmall = 4, scientific = FALSE)
                	table <- cbind(table, vectorStErr)
            	}
            	if (CoeffVariation == "Y") {
                	vectorCoeffVariation <- format(round(vectorCoeffVariation, 1), nsmall = 1, scientific = FALSE)
                	table <- cbind(table, vectorCoeffVariation)
            	}
            	if (confidenceLimits == "Y") {
                	vectorLCI <- format(round(vectorLCI, 4), nsmall = 4, scientific = FALSE)
                	table <- cbind(table, vectorLCI)
            	}
            	if (confidenceLimits == "Y") {
                	vectorUCI <- format(round(vectorUCI, 4), nsmall = 4, scientific = FALSE)
                	table <- cbind(table, vectorUCI)
            	}
            	if (MinMax == "Y") {
                	vectorMin <- format(round(vectorMin, 4), nsmall = 4, scientific = FALSE)
                	table <- cbind(table, vectorMin)
            	}
            	if (MinMax == "Y") {
                	vectorMax <- format(round(vectorMax, 4), nsmall = 4, scientific = FALSE)
                	table <- cbind(table, vectorMax)
            	}
            	if (MedianQuartile == "Y") {
                	vectorMedian <- format(round(vectorMedian, 4), nsmall = 4, scientific = FALSE)
                	table <- cbind(table, vectorMedian)
            	}
            	if (MedianQuartile == "Y") {
                	vectorLQ <- format(round(vectorLQ, 4), nsmall = 4, scientific = FALSE)
                	table <- cbind(table, vectorLQ)
            	}
            	if (MedianQuartile == "Y") {
                	vectorUQ <- format(round(vectorUQ, 4), nsmall = 4, scientific = FALSE)
                	table <- cbind(table, vectorUQ)
            	}

 	       	#Generating column names
            	temp6 <- c("Categorisation Factor levels")
            	if (mean == "Y") {
            		hed1 <- c("Mean")
                	temp6 <- cbind(temp6, hed1)
            	}
            	if (N == "Y") {
                	hed2 <- c("N")
                	temp6 <- cbind(temp6, hed2)
            	}
            	if (sum == "Y") {
                	hed2x <- c("Sum")
                	temp6 <- cbind(temp6, hed2x)
            	}
            	if (Variances == "Y") {
                	hed4 <- c("Variance")
                	temp6 <- cbind(temp6, hed4)
            	}
            	if (StDev == "Y") {
                	#STB May 2012 changing header
                	hed3 <- c("Std dev")
                	temp6 <- cbind(temp6, hed3)
            	}
            	if (StErr == "Y") {
                	#STB May 2012 changing header
                	hed5 <- c("Std error")
                	temp6 <- cbind(temp6, hed5)
            	}
           	if (CoeffVariation == "Y") {
                	hed11 <- c("%CV")
                	temp6 <- cbind(temp6, hed11)
            	}
            	if (confidenceLimits == "Y") {
                	CIlow <- paste("Lower ", 100 * CIval, sep = "")
                	CIlow <- paste(CIlow, "% CI", sep = "")
                	hed12 <- c(CIlow)
                	temp6 <- cbind(temp6, hed12)
            	}
            	if (confidenceLimits == "Y") {
                	CIhigh <- paste("Upper ", 100 * CIval, sep = "")
                	CIhigh <- paste(CIhigh, "% CI", sep = "")
                	hed13 <- c(CIhigh)
                	temp6 <- cbind(temp6, hed13)
            	}
            	if (MinMax == "Y") {
                	hed6 <- c("Min")
                	temp6 <- cbind(temp6, hed6)
            	}
            	if (MinMax == "Y") {
                	hed7 <- c("Max")
                	temp6 <- cbind(temp6, hed7)
            	}
            	if (MedianQuartile == "Y") {
                	hed8 <- c("Median")
                	temp6 <- cbind(temp6, hed8)
            	}
            	if (MedianQuartile == "Y") {
                	hed9 <- c("Lower quartile")
                	temp6 <- cbind(temp6, hed9)
            	}
            	if (MedianQuartile == "Y") {
                	hed10 <- c("Upper quartile")
                	temp6 <- cbind(temp6, hed10)
            	}

  	    	#Generating row names
            	rownms <- c(" ")
            	for (i in 1:length) {
                	rownms[i] <- levels(as.factor(statdata$catfact))[i]
            	}
	    	table <- table[,-1]
            	table <- cbind(rownms, table)
            	colnames(table) <- temp6

            	#Generating title for the tables
            	add <- paste(c("Summary statistics for "), csResponses, sep = "")
            	add <- paste(add, " categorised by ", sep = "")

            	if (firstCat != "NULL" && secondCat == "NULL" && thirdCat == "NULL" && fourthCat == "NULL") {
                	add <- paste(add, firstCat, sep = "")
            	} 
	    	if (firstCat == "NULL" && secondCat != "NULL" && thirdCat == "NULL" && fourthCat == "NULL") {
                	add <- paste(add, secondCat, sep = "")
            	} 
            	if (firstCat == "NULL" && secondCat == "NULL" && thirdCat != "NULL" && fourthCat == "NULL") {
                	add <- paste(add, thirdCat, sep = "")
            	} 
            	if (firstCat == "NULL" && secondCat == "NULL" && thirdCat == "NULL" && fourthCat != "NULL") {
                	add <- paste(add, fourthCat, sep = "")
            	}
            	if (firstCat != "NULL" && secondCat != "NULL" && thirdCat == "NULL" && fourthCat == "NULL") {
                	add <- paste(add, firstCat, " and ", secondCat, sep = "")
            	}
            	if (firstCat != "NULL" && secondCat == "NULL" && thirdCat != "NULL" && fourthCat == "NULL") {
                	add <- paste(add, firstCat, " and ", thirdCat, sep = "")
            	}
            	if (firstCat != "NULL" && secondCat == "NULL" && thirdCat == "NULL" && fourthCat != "NULL") {
                	add <- paste(add, firstCat, " and ", fourthCat, sep = "")
            	}
            	if (firstCat == "NULL" && secondCat != "NULL" && thirdCat != "NULL" && fourthCat == "NULL") {
                	add <- paste(add, secondCat, " and ", thirdCat, sep = "")
            	}
            	if (firstCat == "NULL" && secondCat != "NULL" && thirdCat == "NULL" && fourthCat != "NULL") {
                	add <- paste(add, secondCat, " and ", fourthCat, sep = "")
            	}
            	if (firstCat == "NULL" && secondCat == "NULL" && thirdCat != "NULL" && fourthCat != "NULL") {
                	add <- paste(add, thirdCat, " and ", fourthCat, sep = "")
            	}
            	if (firstCat == "NULL" && secondCat != "NULL" && thirdCat != "NULL" && fourthCat != "NULL") {
                	add <- paste(add, secondCat, ", ", thirdCat, " and ", fourthCat, sep = "")
            	}
            	if (firstCat != "NULL" && secondCat != "NULL" && thirdCat != "NULL" && fourthCat == "NULL") {
               		add <- paste(add, firstCat, ", ", secondCat, " and ", thirdCat, sep = "")
            	}
            	if (firstCat != "NULL" && secondCat != "NULL" && thirdCat == "NULL" && fourthCat != "NULL") {
               		add <- paste(add, firstCat, ", ", secondCat, " and ", fourthCat, sep = "")
            	}
            	if (firstCat != "NULL" && secondCat == "NULL" && thirdCat != "NULL" && fourthCat != "NULL") {
               		add <- paste(add, firstCat, ", ", thirdCat, " and ", fourthCat, sep = "")
            	}
            	if (firstCat != "NULL" && secondCat != "NULL" && thirdCat != "NULL" && fourthCat != "NULL") {
               		add <- paste(add, firstCat, ", ", secondCat, ", ", thirdCat, " and ", fourthCat, sep = "")
            	}

		#Removing blank rows
	    	if (rownms[1] == "") {
			table2 <-table[-1,]
	    	} else {
			table2 <-table
	    	}

	    	if (resplength == 1){
            		#Output tables 
            		HTML.title(add, HR = 2, align = "left")
            	} else {
            		HTML.title(add, HR = 3, align = "left")
	   	}
	    	HTML(table2, align = "left", classfirstline = "second", row.names = "FALSE")
	}

	#===================================================================================================================
	#Normal probability plot
	#===================================================================================================================
	if (NormalProbabilityPlot != "N" && resplength > 1) {
	    HTML.title("Normal probability plots", HR = 2, align = "left")
	}
	if (NormalProbabilityPlot != "N" ) {
	#===================================================================================================================
		#Graphical plot options
	    	YAxisTitle <- "Sample Quantiles"
	    	XAxisTitle <- "Theoretical Quantiles"
	    	Line_size <- 0.5
	    	Gr_alpha <- 1
	    	Line_type <- Line_type_dashed
	    	Gr_palette<-palette_FUN("catfact")
	#===================================================================================================================
   		index <- 1
 	
		for (i in 1: (length(expectedChanges)-1)) {
			normPlot2 <- sub(".html", "IVS", htmlFile)
		    	normPlot2 <- paste(normPlot2, index, "normplot2.png", sep = "")
		    	png(normPlot2, width = jpegwidth, height = jpegheight, units="in", res=PlotResolution)
	
	    		#STB July2013
		   	plotFilepdf5 <- sub(".html", "IVS", htmlFile)
	    		plotFilepdf5 <- paste(plotFilepdf5, index, "normplot.pdf", sep = "")
	    		dev.control("enable")
		
			#Plot title text
		    	csResponses <- resplist[index]
		    	csResponsesqq <- resplistqq[index]
		    	adda <- paste(c("Categorised Normal probability plot for "), csResponsesqq ,  sep = "")
	
#			if ((firstCat != "NULL" && secondCat != "NULL") || (firstCat != "NULL" && thirdCat != "NULL") || (firstCat != "NULL" && fourthCat != "NULL") || (secondCat != "NULL" && thirdCat != "NULL") || (secondCat != "NULL" && fourthCat != "NULL") || (thirdCat != "NULL" && fourthCat != "NULL") ) {
#		        	adda <- paste(adda, "s", sep = "")
#			}
		
			if (resplength == 1) {
				HTML.title(adda, HR = 2, align = "left")
			} else {
			HTML.title(adda, HR = 3, align = "left")
		}
		
		MainTitle2 <- ""
		graphdata <- statdata
		graphdata$names <- c(1:nrow(graphdata))
		graphdata <- graphdata[!is.na(eval(parse(text = paste("graphdata$", csResponses)))),]
		graphdata$yvarrr_IVS <- eval(parse(text = paste("graphdata$", csResponses))) 


	    	#GGPLOT2 code
		#NONCAT_SCAT("QQPLOT")
		CAT_QQPLOT()
	
	#===================================================================================================================	
	    	void <- HTMLInsertGraph(GraphFileName = sub("[A-Z0-9a-z,:,\\\\]*App_Data[\\\\]", "", normPlot2), Align = "left")
	
	    	#STB July2013
	    	if (pdfout == "Y") {
	        	pdf(file = sub("[A-Z0-9a-z,:,\\\\]*App_Data[\\\\]", "", plotFilepdf5), height = pdfheight, width = pdfwidth)
	        	dev.set(2)
	        	dev.copy(which = 3)
	        	dev.off(2)
	        	dev.off(3)
	        	pdfFile_5 <- sub("[A-Z0-9a-z,:,\\\\]*App_Data[\\\\]", "", plotFilepdf5)
	        	linkToPdf5 <- paste("<a href=\"", pdfFile_5, "\">Click here to view the PDF of the normal probability plot</a>", sep = "")
	        	HTML(linkToPdf5)
	    	}
	
		index <- index +1
	     }
	
	     HTML("Tip: Check that the points lie along the dotted lines. If not then the data may be non-normally distributed.", align="left")		
	}
}




#===================================================================================================================
#Non-categorisation analysis
#===================================================================================================================
#Overall title
if ((firstCat != "NULL" || secondCat != "NULL" || thirdCat != "NULL" || fourthCat != "NULL") && ByCategoriesAndOverall == "Y" && resplength > 1) {
        HTML.title("Non-categorised summary statistics", HR=2, align="left")
}

if ((firstCat == "NULL" && secondCat == "NULL" && thirdCat == "NULL" && fourthCat == "NULL") && resplength > 1) {
        #Overall title
        HTML.title("Summary statistics", HR=2, align="left")
}

for (i in 1:resplength) {

   if ((firstCat == "NULL" && secondCat == "NULL" && thirdCat == "NULL" && fourthCat == "NULL") || ByCategoriesAndOverall == "Y") {

	csResponses <- resplist[i]

	#Setting up parameters and vectors
	length <- 1
        table <- c(1:1)
        table[1] <- csResponses

        vectormean <- c(1:length)
        vectorN <- c(1:length)
        vectorsum <- c(1:length)
        vectorStDev <- c(1:length)
        vectorVariances <- c(1:length)
        vectorStErr <- c(1:length)
        vectorMin <- c(1:length)
        vectorMax <- c(1:length)
        vectorMedian <- c(1:length)
        vectorLQ <- c(1:length)
        vectorUQ <- c(1:length)
        vectorCoeffVariation <- c(1:length)
        vectorUCI <- c(1:length)
        vectorLCI <- c(1:length)

        #Generating summary statistics
        for (i in 1:length) {
            sub <- statdata
            sub2 <- data.frame(sub)

            if (mean == "Y") {
                vectormean[i] = mean(eval(parse(text = paste("sub2$", csResponses))), na.rm = TRUE)
            }
            if (N == "Y") {
                tempy <- na.omit(eval(parse(text = paste("sub2$", csResponses))))
                vectorN[i] = length(tempy)
            }
            if (sum == "Y") {
                vectorsum[i] = sum(eval(parse(text = paste("sub2$", csResponses))), na.rm = TRUE)
            }
            if (StDev == "Y") {
                vectorStDev[i] = sd(eval(parse(text = paste("sub2$", csResponses))), na.rm = TRUE)
            }
            if (Variances == "Y") {
                vectorVariances[i] = var(eval(parse(text = paste("sub2$", csResponses))), na.rm = TRUE)
            }
            if (StErr == "Y") {
                tempy <- na.omit(eval(parse(text = paste("sub2$", csResponses))))
                vectorStErr[i] = sd(eval(parse(text = paste("sub2$", csResponses))), na.rm = TRUE) / (length(tempy)) ** (0.5)
            }
            if (MinMax == "Y") {
                vectorMin[i] = suppressWarnings(min(eval(parse(text = paste("sub2$", csResponses))), na.rm = TRUE))
            }
            if (MinMax == "Y") {
                vectorMax[i] = suppressWarnings(max(eval(parse(text = paste("sub2$", csResponses))), na.rm = TRUE))
            }
            if (MedianQuartile == "Y") {
                vectorMedian[i] = median(eval(parse(text = paste("sub2$", csResponses))), na.rm = TRUE)
            }
            if (MedianQuartile == "Y") {
                vectorLQ[i] = quantile(eval(parse(text = paste("sub2$", csResponses))), 0.25, type = 2, na.rm = TRUE)
            }
            if (MedianQuartile == "Y") {
                vectorUQ[i] = quantile(eval(parse(text = paste("sub2$", csResponses))), 0.75, type = 2, na.rm = TRUE)
            }
            if (CoeffVariation == "Y") {
                vectorCoeffVariation[i] = 100 * sd(eval(parse(text = paste("sub2$", csResponses))), na.rm = TRUE) / abs(mean(eval(parse(text = paste("sub2$", csResponses))), na.rm = TRUE))
            }
            if (confidenceLimits == "Y") {
                tempy <- na.omit(eval(parse(text = paste("sub2$", csResponses))))
                vectorLCI[i] = mean(eval(parse(text = paste("sub2$", csResponses))), na.rm = TRUE) - qt(1 - (1 - CIval) / 2, (length(tempy) - 1)) * sd(eval(parse(text = paste("sub2$", csResponses))), na.rm = TRUE) / (length(tempy)) ** (0.5)
            }
            if (confidenceLimits == "Y") {
                tempy <- na.omit(eval(parse(text = paste("sub2$", csResponses))))
                vectorUCI[i] = mean(eval(parse(text = paste("sub2$", csResponses))), na.rm = TRUE) + qt(1 - (1 - CIval) / 2, (length(tempy) - 1)) * sd(eval(parse(text = paste("sub2$", csResponses))), na.rm = TRUE) / (length(tempy)) ** (0.5)
            }
        }

        #Generating final table dataset
        if (mean == "Y") {
             vectormean <- format(round(vectormean, 4), nsmall = 4, scientific = FALSE)
             table <- cbind(table, vectormean)
        }
        if (N == "Y") {
            table <- cbind(table, vectorN)
        }
        if (sum == "Y") {
             vectorsum <- format(round(vectorsum, 4), nsmall = 4, scientific = FALSE)
             table <- cbind(table, vectorsum)
        }
        if (Variances == "Y") {
            vectorVariances <- format(round(vectorVariances, 4), nsmall = 4, scientific = FALSE)
            table <- cbind(table, vectorVariances)
        }
        if (StDev == "Y") {
            vectorStDev <- format(round(vectorStDev, 4), nsmall = 4, scientific = FALSE)
            table <- cbind(table, vectorStDev)
        }
        if (StErr == "Y") {
            vectorStErr <- format(round(vectorStErr, 4), nsmall = 4, scientific = FALSE)
            table <- cbind(table, vectorStErr)
        }
        if (CoeffVariation == "Y") {
            vectorCoeffVariation <- format(round(vectorCoeffVariation, 1), nsmall = 1, scientific = FALSE)
            table <- cbind(table, vectorCoeffVariation)
        }
        if (confidenceLimits == "Y") {
            vectorLCI <- format(round(vectorLCI, 4), nsmall = 4, scientific = FALSE)
            table <- cbind(table, vectorLCI)
        }
        if (confidenceLimits == "Y") {
            vectorUCI <- format(round(vectorUCI, 4), nsmall = 4, scientific = FALSE)
            table <- cbind(table, vectorUCI)
        }
        if (MinMax == "Y") {
            vectorMin <- format(round(vectorMin, 4), nsmall = 4, scientific = FALSE)
            table <- cbind(table, vectorMin)
        }
        if (MinMax == "Y") {
            vectorMax <- format(round(vectorMax, 4), nsmall = 4, scientific = FALSE)
            table <- cbind(table, vectorMax)
        }
        if (MedianQuartile == "Y") {
            vectorMedian <- format(round(vectorMedian, 4), nsmall = 4, scientific = FALSE)
            table <- cbind(table, vectorMedian)
        }
        if (MedianQuartile == "Y") {
            vectorLQ <- format(round(vectorLQ, 4), nsmall = 4, scientific = FALSE)
            table <- cbind(table, vectorLQ)
        }
        if (MedianQuartile == "Y") {
            vectorUQ <- format(round(vectorUQ, 4), nsmall = 4, scientific = FALSE)
            table <- cbind(table, vectorUQ)
        }

        #creating final output table

        #Generating column names
        temp6 <- c("Response")

        if (mean == "Y") {
            hed1 <- c("Mean")
            temp6 <- cbind(temp6, hed1)
        }
        if (N == "Y") {
            hed2 <- c("N")
            temp6 <- cbind(temp6, hed2)
        }
        if (sum == "Y") {
            hed2x <- c("Sum")
            temp6 <- cbind(temp6, hed2x)
        }
	if (Variances == "Y") {
            hed4 <- c("Variance")
            temp6 <- cbind(temp6, hed4)
        }
        if (StDev == "Y") {
            #STB May 2012 changing header
            hed3 <- c("Std dev")
            temp6 <- cbind(temp6, hed3)
        }
        if (StErr == "Y") {
            #STB May 2012 changing header
            hed5 <- c("Std error")
            temp6 <- cbind(temp6, hed5)
        }
        if (CoeffVariation == "Y") {
            hed11 <- c("%CV")
            temp6 <- cbind(temp6, hed11)
        }
        if (confidenceLimits == "Y") {
            CIlow <- paste("Lower ", 100 * CIval, sep = "")
            CIlow <- paste(CIlow, "% CI", sep = "")
            hed12 <- c(CIlow)
            temp6 <- cbind(temp6, hed12)
        }
        if (confidenceLimits == "Y") {
            CIhigh <- paste("Upper ", 100 * CIval, sep = "")
            CIhigh <- paste(CIhigh, "% CI", sep = "")
            hed13 <- c(CIhigh)
            temp6 <- cbind(temp6, hed13)
        }
        if (MinMax == "Y") {
            hed6 <- c("Min")
            temp6 <- cbind(temp6, hed6)
        }
        if (MinMax == "Y") {
            hed7 <- c("Max")
            temp6 <- cbind(temp6, hed7)
        }
        if (MedianQuartile == "Y") {
            hed8 <- c("Median")
            temp6 <- cbind(temp6, hed8)
        }
        if (MedianQuartile == "Y") {
            hed9 <- c("Lower quartile")
            temp6 <- cbind(temp6, hed9)
        }
        if (MedianQuartile == "Y") {
            hed10 <- c("Upper quartile")
            temp6 <- cbind(temp6, hed10)
        }


        #Generating row names
        colnames(table) <- temp6

        #Output print
        if ((firstCat == "NULL" && secondCat == "NULL" && thirdCat == "NULL" && fourthCat == "NULL")) {
            add <- paste(c("Summary statistics for "), csResponses, sep = "")
        } else {
        	if ((firstCat != "NULL" || secondCat != "NULL" || thirdCat != "NULL" || fourthCat != "NULL") && ByCategoriesAndOverall == "Y") {
             		add <- paste("Overall summary statistics for " , csResponses, ", ignoring the categorisation factor", sep = "")
		}
		if ((firstCat != "NULL" && secondCat != "NULL") || (firstCat != "NULL" && thirdCat != "NULL") || (firstCat != "NULL" && fourthCat != "NULL") || (secondCat != "NULL" && thirdCat != "NULL") || (secondCat != "NULL" && fourthCat != "NULL") || (thirdCat != "NULL" && fourthCat != "NULL") ) {
            		add <- paste(add, "s", sep = "")
		}
 	}

	if (resplength == 1) {
		HTML.title(add, HR = 2, align = "left")
	} else {
		HTML.title(add, HR = 3, align = "left")
	}
        HTML(table, align = "left", classfirstline = "second", row.names = "FALSE")
    }
}

#===================================================================================================================
#Normal probability plot
#===================================================================================================================
if (NormalProbabilityPlot != "N" && resplength > 1) {
    HTML.title("Normal probability plots", HR = 2, align = "left")
}
if (NormalProbabilityPlot != "N" ) {
#===================================================================================================================
    #Graphical plot options
    YAxisTitle <- "Sample Quantiles"
    XAxisTitle <- "Theoretical Quantiles"
    w_Gr_jitscat <- 0
    h_Gr_jitscat <- 0
    infiniteslope <- "N"
    LinearFit <- "Y"
    Line_size <- 0.5
    Gr_alpha <- 1
    Line_type <- Line_type_dashed
    ScatterPlot <- "Y"
#===================================================================================================================
    index <- 1
 
    for (i in 1: (length(expectedChanges)-1)) {
	normPlot <- sub(".html", "IVS", htmlFile)
    	normPlot <- paste(normPlot, index, "normplot.png", sep = "")
    	png(normPlot, width = jpegwidth, height = jpegheight, units="in", res=PlotResolution)

    	#STB July2013
    	plotFilepdf4 <- sub(".html", "IVS", htmlFile)
    	plotFilepdf4 <- paste(plotFilepdf4, index, "normplot.pdf", sep = "")
    	dev.control("enable")

	#Plot title text
    	csResponses <- resplist[index]
    	csResponsesqq <- resplistqq[index]
    	adda <- paste(c("Normal probability plot for "), csResponsesqq ,  sep = "")

	if (firstCat != "NULL" || secondCat != "NULL" || thirdCat != "NULL" || fourthCat != "NULL") {
		adda <- paste(adda, ", ignoring the categorisation factor", sep = "")
	}
	if ((firstCat != "NULL" && secondCat != "NULL") || (firstCat != "NULL" && thirdCat != "NULL") || (firstCat != "NULL" && fourthCat != "NULL") || (secondCat != "NULL" && thirdCat != "NULL") || (secondCat != "NULL" && fourthCat != "NULL") || (thirdCat != "NULL" && fourthCat != "NULL") ) {
        	adda <- paste(adda, "s", sep = "")
	}

	if (resplength == 1) {
		HTML.title(adda, HR = 2, align = "left")
	} else {
		HTML.title(adda, HR = 3, align = "left")
	}

    	MainTitle2 <- ""
    	te <- qqnorm(eval(parse(text = paste("statdata$", csResponses))))
    	graphdata <- data.frame(te$x, te$y)
	graphdata <- graphdata[!is.na(te$y),]
    	graphdata$xvarrr_IVS <- graphdata$te.x
    	graphdata$yvarrr_IVS <- graphdata$te.y
	graphdata$names <- c(1:length(graphdata$te.x))

    	#GGPLOT2 code
	#NONCAT_SCAT("QQPLOT")
	NONCAT_QQPLOT()

#===================================================================================================================
    	void <- HTMLInsertGraph(GraphFileName = sub("[A-Z0-9a-z,:,\\\\]*App_Data[\\\\]", "", normPlot), Align = "left")

    	#STB July2013
    	if (pdfout == "Y") {
        	pdf(file = sub("[A-Z0-9a-z,:,\\\\]*App_Data[\\\\]", "", plotFilepdf4), height = pdfheight, width = pdfwidth)
        	dev.set(2)
        	dev.copy(which = 3)
        	dev.off(2)
        	dev.off(3)
        	pdfFile_4 <- sub("[A-Z0-9a-z,:,\\\\]*App_Data[\\\\]", "", plotFilepdf4)
        	linkToPdf4 <- paste("<a href=\"", pdfFile_4, "\">Click here to view the PDF of the normal probability plot</a>", sep = "")
        	HTML(linkToPdf4)
    	}

	index <- index +1
     }

     HTML("Tip: Check that the points lie along the dotted line. If not then the data may be non-normally distributed.", align="left")
     HTML("Warning: Note the normal probability plots presented in the Summary Statistics module do not take into account any design factors. Hence the response(s) may appear to be non-normally distributed due to the effect of any design factors. If you have factors that may influence the response(s), then it is recommended that you use the normal probability plot in the Single or Repeated Measures Parametric Analysis module, which does take these factors into account.", align="left")
}

#===================================================================================================================
#References
#===================================================================================================================
Ref_list <- R_refs()

#Bate and Clark comment
HTML(refxx, align = "left")

HTML.title("References", HR=2, align="left")
HTML(Ref_list$IVS_ref, align="left")
HTML(Ref_list$BateClark_ref, align = "left")

HTML.title("R references", HR=4, align="left")
HTML(Ref_list$R_ref, align = "left")
HTML(paste(capture.output(print(citation("R2HTML"),bibtex=F))[4], capture.output(print(citation("R2HTML"),bibtex=F))[5], sep = ""),  align="left")

HTML(paste(capture.output(print(citation("GGally"),bibtex=F))[4], capture.output(print(citation("GGally"),bibtex=F))[5], capture.output(print(citation("GGally"),bibtex=F))[6], capture.output(print(citation("GGally"),bibtex=F))[7], sep = ""),  align="left")
HTML(paste(capture.output(print(citation("RColorBrewer"),bibtex=F))[4], capture.output(print(citation("RColorBrewer"),bibtex=F))[5], sep = ""),  align="left")
HTML(paste(capture.output(print(citation("ggplot2"),bibtex=F))[4], capture.output(print(citation("ggplot2"),bibtex=F))[5], sep=""),  align="left")
HTML(paste(capture.output(print(citation("ggrepel"),bibtex=F))[4], capture.output(print(citation("ggrepel"),bibtex=F))[5], capture.output(print(citation("ggrepel"),bibtex=F))[6], sep = ""),  align="left")
HTML(paste(capture.output(print(citation("reshape"),bibtex=F))[4], capture.output(print(citation("reshape"),bibtex=F))[5], sep = ""),  align="left")
HTML(paste(capture.output(print(citation("plyr"),bibtex=F))[4], capture.output(print(citation("plyr"),bibtex=F))[5], capture.output(print(citation("plyr"),bibtex=F))[6], sep = ""),  align="left")
HTML(paste(capture.output(print(citation("scales"),bibtex=F))[4], capture.output(print(citation("scales"),bibtex=F))[5], capture.output(print(citation("scales"),bibtex=F))[6], sep = ""),  align="left")
HTML(paste(capture.output(print(citation("proto"),bibtex=F))[4], capture.output(print(citation("proto"),bibtex=F))[5], capture.output(print(citation("proto"),bibtex=F))[6], sep = ""),  align="left")
#extrafont_ref  <- capture.output(print(citation("extrafont"),bibtex=F))[4]

#===================================================================================================================
#Show dataset
#===================================================================================================================
if (showdataset == "Y") {

    observ <- data.frame(c(1:dim(statdata)[1]))
    colnames(observ) <- c("Observation")
    statdata <- cbind(observ, statdata)
    statdata2 <- subset(statdata, select = -c(catfact))

    HTML.title("Analysis dataset", HR = 2, align = "left")
    HTML(statdata2, classfirstline = "second", align = "left", row.names = "FALSE")
}

#===================================================================================================================
#Show arguments
#===================================================================================================================
if (OutputAnalysisOps == "Y") {
	HTML.title("Analysis options", HR=2, align="left")

	HTML(paste("Response variable(s): ", csResponses, sep=""),  align="left")

	if (transformation != "none") {
		HTML(paste("Response variable(s) transformation: ", transformation, sep=""),  align="left")
	}

	if (firstCat != "NULL") {
		HTML(paste("First categorisation factor: ", firstCat, sep=""),  align="left")
	}

	if (secondCat != "NULL") {
		HTML(paste("Second categorisation factor: ", secondCat, sep=""),  align="left")
	}

	if (thirdCat != "NULL") {
		HTML(paste("Third categorisation factor: ", thirdCat, sep=""),  align="left")
	}

	if (fourthCat != "NULL") {
		HTML(paste("Fourth categorisation factor: ", fourthCat, sep=""),  align="left")
	}
	HTML(paste("Output mean (Y/N): ", mean, sep=""),  align="left")
	HTML(paste("Output sample size  (Y/N): ", N, sep=""),  align="left")
	HTML(paste("Output sum  (Y/N): ", sum, sep=""),  align="left")
	HTML(paste("Output variance (Y/N): ", Variances, sep=""),  align="left")
	HTML(paste("Output standard deviation (Y/N): ", StDev, sep=""),  align="left")
	HTML(paste("Output standard error of mean (Y/N): ", StErr, sep=""),  align="left")
	HTML(paste("Output coefficient of variation (Y/N): ", CoeffVariation, sep=""),  align="left")
	HTML(paste("Output confidence interval of the mean (Y/N): ", confidenceLimits, sep=""),  align="left")
	
	if (confidenceLimits == "Y") {	
		HTML(paste("Confidence level (%): ", CIval2, sep=""),  align="left")
	}
	
	HTML(paste("Output normal probability plot (Y/N): ", NormalProbabilityPlot, sep=""),  align="left")
	HTML(paste("Output minimum/maximum  (Y/N): ", MinMax, sep=""),  align="left")
	HTML(paste("Output median and quartiles (Y/N): ", MedianQuartile, sep=""),  align="left")
	HTML(paste("Output results by categories & overall (Y/N): ", ByCategoriesAndOverall, sep=""),  align="left")
}