#===================================================================================================================
#R Libraries

suppressWarnings(library(R2HTML))

#===================================================================================================================
# retrieve args
Args <- commandArgs(TRUE)

#Read in data
statdata <- read.csv(Args[3], header=TRUE, sep=",")

#Copy Args

#STB - July 2012 rename response variable
xxxresponsexxx <- Args[4]
responseTransform <- Args[5]
truemean <- as.numeric(Args[6])
showCITable <- Args[7]
showNormPlot <- Args[8]
sig <- 1 - as.numeric(Args[9])

#source(paste(getwd(),"/Common_Functions.R", sep=""))

#Print args
if (Diplayargs == "Y"){
	print(Args)
}

#===================================================================================================================
#Parameter setup

#Graphical parameters
graphdata<-statdata
ReferenceLine <- "NULL"

#Breakdown the list of responses
resplist <- c()
tempChanges <- strsplit(xxxresponsexxx, ",")
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
#Setup the html file and associated css file
htmlFile <- sub(".csv", ".html", Args[3]); #determine the file name of the html file
HTMLSetFile(file=htmlFile) 
cssFile <- "r2html.css"
cssFile <- paste("'",cssFile,"'", sep="") #need to enclose in quotes when path has spaces in it
HTMLCSS(CSSfile = cssFile)

#===================================================================================================================
#Output HTML header and description
Title <-paste(branding, " One-sample t-test Analysis", sep="")
HTML.title(Title, HR = 1, align = "left")

#Response
if (resplength == 1) {
	HTML.title("Response", HR=2, align="left")
} else {
	HTML.title("Responses", HR=2, align="left")
}

if (resplength == 1) {
	add<-paste("The  ", resplistqq, " response is currently being analysed by the One-Sample t-test Analysis module. The sample mean is compared back to the fixed value ", truemean, ".", sep="")
} else {
	add<-paste("The responses ")
	if (resplength == 2) {
		add<- paste (add, resplistqq[1] , " and ", resplistqq[2] , sep="") 
	} else {
		for (m in 1: (resplength-2) ) {
			add<- paste (add, resplistqq[m] , ", ", sep ="")
		}
		add <- paste (add, resplistqq[(resplength-1)] , " and ", resplistqq[resplength] , sep="") 
	}
	add<-paste (add, " are currently being analysed by the One-Sample t-test Analysis module. The sample means are compared back to the fixed value ", truemean, ".", sep="")
}
HTML(add, align="left")

if (responseTransform != "None") {
	if (resplength == 1) {
		add2<-paste(c("The response has been "), responseTransform, " transformed prior to analysis.", sep="")
	} else {
		add2<-paste(c("The responses have been "), responseTransform, " transformed prior to analysis.", sep="")
	}
	HTML(add2, align="left")
}

#Bate and Clark comment
HTML(refxx, align="left")	

#===================================================================================================================
#Analysis
#===================================================================================================================
HTML.title("One-sample t-test summary results", HR=2, align="left")

finaltable <- matrix(nrow=resplength, ncol=6)
pval<-c(1:resplength)
for (i in 1:resplength) {
	onet <- t.test (eval(parse(text = paste("statdata$", resplist[i]))), mu=truemean, conf.level = sig)

	finaltable[i,1]<-c(resplistqq[i])
	finaltable[i,2]<-format(round(as.numeric(onet[5]), 3), nsmall=3, scientific=FALSE)
	finaltable[i,3]<-truemean
	finaltable[i,4]<-format(round(as.numeric(onet[1]), 3), nsmall=3, scientific=FALSE)
	finaltable[i,5]<-format(round(as.numeric(onet[2]), 0), nsmall=0, scientific=FALSE)
	pval[i]	<-as.numeric(onet[3])
	finaltable[i,6]<- format(round(pval[i], 4), nsmall=4, scientific=FALSE)
}

head<-c("Response", "Sample mean", "Fixed value", "t-statistic", "Degrees of freedom","p-value")
colnames(finaltable) <- head

for (i in 1:(dim(finaltable)[1]))  {
	if (pval[i]<0.0001)  {
		#STB March 2011 - formatting p-values p<0.0001
		finaltable[i,6]=format(round(0.0001, 4), nsmall=4, scientific=FALSE)
		finaltable[i,6]<- paste("<",finaltable[i,6])
	}
}

HTML(finaltable, classfirstline="second", align="left", row.names = "FALSE")

add<-paste(c("Conclusion"))
inte<-1
for(i in 1:(dim(finaltable)[1])) {
	if (pval[i] <= (1-sig)) {
		if (inte==1) {
			inte<-inte+1
			add<-paste(add, ": There is a statistically significant difference between the mean of  ", resplistqq[i], sep="")
		} else {
			inte<-inte+1
			add<-paste(add, ", ", resplistqq[i],  sep="")
		}
	}
}

if (inte >1) {
	add <- paste (add, " and the test value ", truemean , sep = "") 
}

if (inte==1) {
	if (dim(finaltable)[1]>2) {
		add<-paste(add, ": There are no statistically significant overall differences between the sample means and the test value ", truemean, sep="")
	} 
	if (dim(finaltable)[1]<=2) {
		add<-paste(add, ": The difference between the sample mean and ", truemean, " is not statistically significant", sep="")
	}
} 

add<-paste(add, ". ", sep="")
HTML(add, align="left")

#===================================================================================================================
#Testing the degrees of freedom
#===================================================================================================================
minval <- suppressWarnings(min(finaltable[ ,5], na.rm = TRUE))
if (minval<5) {
	HTML("Warning: Unfortunately the residual degrees of freedom are low (less than 5). This may make the estimation of the underlying variability, and hence the results of the statistical tests, unreliable.", align="left")
}

#===================================================================================================================
#Calculate a confidence interval around the mean - untransformed
#===================================================================================================================
if (showCITable == "Y") {

	citable <- matrix(nrow=resplength, ncol=4)
	for (i in 1:resplength) {
		onet <- t.test (eval(parse(text = paste("statdata$", resplist[i]))), mu=truemean, conf.level = sig)
		citable[i,1] <- resplistqq[i]
		citable[i,2] <- format(round(as.numeric(onet[5]), 3), nsmall=3, scientific=FALSE)
		citable[i,3] <- format(round(as.numeric(onet$conf.int[1]), 3), nsmall=3, scientific=FALSE)  
		citable[i,4] <- format(round(as.numeric(onet$conf.int[2]), 3), nsmall=3, scientific=FALSE)  
	}
	lowerCI<-paste("Lower ",(sig*100),"% CI",sep="")  
	upperCI<-paste("Upper ",(sig*100),"% CI",sep="")  
	colnames(citable) <- c("Response" , "Sample Mean" ,  lowerCI ,  upperCI)

	if (resplength ==1) {
		CITitle<-paste("Table of the sample mean with ",(sig*100),"% confidence interval",sep="")
	} else {
		CITitle<-paste("Table of the sample means with ",(sig*100),"% confidence intervals",sep="")
	}
	HTML.title(CITitle, HR=2, align="left")
	HTML(citable, classfirstline="second", align="left", row.names = "FALSE")
}
#===================================================================================================================
#Calculate a confidence interval around the mean - log10 transformed
#===================================================================================================================

if (showCITable == "Y" && responseTransform == "Log10") {
	citable <- matrix(nrow=resplength, ncol=4)
	for (i in 1:resplength) {
		onet <- t.test (eval(parse(text = paste("statdata$", resplist[i]))), mu=truemean, conf.level = sig)
		citable[i,1] <- resplistqq[i]
		citable[i,2] <- format(round(as.numeric(10^(as.numeric(onet[5]))), 3), nsmall=3, scientific=FALSE)
		citable[i,3] <- format(round(as.numeric(10^(onet$conf.int[1])), 3), nsmall=3, scientific=FALSE)  
		citable[i,4] <- format(round(as.numeric(10^(onet$conf.int[2])), 3), nsmall=3, scientific=FALSE) 
	}
	lowerCI<-paste("Lower ",(sig*100),"% CI",sep="")  
	upperCI<-paste("Upper ",(sig*100),"% CI",sep="")  
	colnames(citable) <- c("Response" , "Geometric Mean" ,  lowerCI ,  upperCI)

	if (resplength ==1) {
		CITitle<-paste("Table of the back-transformed geometric mean with ",(sig*100),"% confidence interval",sep="")
	} else {
		CITitle<-paste("Table of the back-transformed geometric means with ",(sig*100),"% confidence intervals",sep="")
	}
	HTML.title(CITitle, HR=2, align="left")
	HTML(citable, classfirstline="second", align="left", row.names = "FALSE")
}

#===================================================================================================================
#Calculate a confidence interval around the mean - ln transformed
#===================================================================================================================

if (showCITable == "Y" && responseTransform == "Loge") {
	citable <- matrix(nrow=resplength, ncol=4)
	for (i in 1:resplength) {
		onet <- t.test (eval(parse(text = paste("statdata$", resplist[i]))), mu=truemean, conf.level = sig)
		citable[i,1] <- resplistqq[i]
		citable[i,2] <- format(round(as.numeric(exp(as.numeric(onet[5]))), 3), nsmall=3, scientific=FALSE)
		citable[i,3] <- format(round(as.numeric(exp(onet$conf.int[1])), 3), nsmall=3, scientific=FALSE)  
		citable[i,4] <- format(round(as.numeric(exp(onet$conf.int[2])), 3), nsmall=3, scientific=FALSE) 
	}
	lowerCI<-paste("Lower ",(sig*100),"% CI",sep="")  
	upperCI<-paste("Upper ",(sig*100),"% CI",sep="")  
	colnames(citable) <- c("Response" , "Geometric Mean" ,  lowerCI ,  upperCI)

	if (resplength ==1) {
		CITitle<-paste("Table of the back-transformed geometric mean with ",(sig*100),"% confidence interval",sep="")
	} else {
		CITitle<-paste("Table of the back-transformed geometric means with ",(sig*100),"% confidence intervals",sep="")
	}
	HTML.title(CITitle, HR=2, align="left")
	HTML(citable, classfirstline="second", align="left", row.names = "FALSE")
}


#===================================================================================================================
#Diagnostic plots
#===================================================================================================================
if (showNormPlot != "N" && resplength > 1) {
    HTML.title("Normal probability plots", HR = 2, align = "left")
}

if (showNormPlot != "N" && resplength == 1) {
    HTML.title("Normal probability plot", HR = 2, align = "left")
}

if (showNormPlot != "N" ) {
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
 
    for (i in 1: resplength) {
	normPlot <- sub(".html", "IVS", htmlFile)
    	normPlot <- paste(normPlot, index, "normplot.jpg", sep = "")
    	jpeg(normPlot, width = jpegwidth, height = jpegheight, quality = 100)

    	#STB July2013
    	plotFilepdf4 <- sub(".html", "IVS", htmlFile)
    	plotFilepdf4 <- paste(plotFilepdf4, index, "normplot.pdf", sep = "")
    	dev.control("enable")

	#Plot title text
    	csResponses <- resplist[index]
    	csResponsesqq <- resplistqq[index]
    	adda <- paste(c("Normal probability plot for "), csResponsesqq ,  sep = "")

	if (resplength == 1) {
		HTML.title(adda, HR = 2, align = "left")
	} else {
		HTML.title(adda, HR = 3, align = "left")
	}

    	MainTitle2 <- ""
    	te <- qqnorm(eval(parse(text = paste("statdata$", csResponses))))
    	graphdata <- data.frame(te$x, te$y)
    	graphdata$xvarrr_IVS <- graphdata$te.x
    	graphdata$yvarrr_IVS <- graphdata$te.y

    	#GGPLOT2 code
    	NONCAT_SCAT("QQPLOT")
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
Ref_list<-R_refs()

HTML.title("Statistical references", HR=2, align="left")
HTML(Ref_list$BateClark_ref, align="left")

HTML.title("R references", HR=2, align="left")
HTML(Ref_list$R_ref ,  align="left")
HTML(Ref_list$GGally_ref,  align="left")
HTML(Ref_list$RColorBrewers_ref,  align="left")
HTML(Ref_list$GGPLot2_ref,  align="left")
HTML(Ref_list$ggrepel_ref,  align="left")
HTML(Ref_list$reshape_ref,  align="left")
HTML(Ref_list$plyr_ref,  align="left")
HTML(Ref_list$scales_ref,  align="left")
HTML(Ref_list$R2HTML_ref,  align="left")
HTML(Ref_list$PROTO_ref,  align="left")


#===================================================================================================================
#Show dataset
#===================================================================================================================
if (showdataset=="Y") {
	observ <- data.frame(c(1:dim(statdata)[1]))
	colnames(observ) <- c("Observation")
	statdata22 <- cbind(observ, statdata)

	HTML.title("Analysis dataset", HR = 2, align = "left")
    	HTML(statdata22, classfirstline = "second", align = "left", row.names = "FALSE")

}

#===================================================================================================================
#Show arguments
#===================================================================================================================
HTML.title("Analysis options", HR=2, align="left")

HTML(paste("Response variable: ", resplistqq, sep=""),  align="left")
 
if (responseTransform != "None")
{ 
	HTML(paste("Response transformation: ", responseTransform, sep=""), align="left")
}
HTML(paste("Test value: ", truemean, sep=""),  align="left")
HTML(paste("Display confidence interval table (Y/N): ", showCITable, sep=""),  align="left")
HTML(paste("Display normal probability plot (Y/N): ", showNormPlot, sep=""),  align="left")
HTML(paste("Significance level: ", 1-sig, sep=""), align="left")

