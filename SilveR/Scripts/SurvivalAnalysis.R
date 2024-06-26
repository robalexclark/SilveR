﻿#===================================================================================================================
#R Libraries

suppressWarnings(library(R2HTML))
suppressWarnings(library(survival))
#suppressWarnings(library(gridExtra))

#===================================================================================================================
# retrieve args
Args <- commandArgs(TRUE)

#Read in data
statdata <- read.csv(Args[3], header=TRUE, sep=",")
statdata_print<-statdata

#Copy Args
Responseqq <- Args[4]
Groupqq <- Args[5]
Grouprr <- Args[5]
Censorshipqq <- Args[6]
SummaryResults <- Args[7]
ComparingCurves <- Args[9]
ShowPlot <- Args[8]
sig <- 1 - as.numeric(Args[10])

#source(paste(getwd(),"/Common_Functions.R", sep=""))

#Print args
if (Diplayargs == "Y"){
	print(Args)
}

#===================================================================================================================
#Setup the html file and associated css file
htmlFile <- sub(".csv", ".html", Args[3]); #determine the file name of the html file
HTMLSetFile(file=htmlFile) 
cssFile <- "r2html.css"
cssFile <- paste("'",cssFile,"'", sep="") #need to enclose in quotes when path has spaces in it
HTMLCSS(CSSfile = cssFile)

#===================================================================================================================
#Parameter setup

#Graphics parameter setup
graphdata<-statdata
Gr_palette<-palette_FUN(Groupqq)
YAxisTitle = "Proportion surviving"
MainTitle2 = ""
Line_type<-Line_type_solid
ReferenceLine <- "NULL"
XLimLow <- "NULL"
XLimHigh <- "NULL"
YLimLow <- "NULL"
YLimHigh <- "NULL"

#replace illegal characters in variable names
XAxisTitle <-Responseqq

for (i in 1:10) {
	XAxisTitle<-namereplace(XAxisTitle)
}

for (i in 1:10) {
	Grouprr<-namereplace(Grouprr)
}

statdata$Groupzzz <-eval(parse(text = paste("statdata$", Groupqq)))

mfit <- survfit(Surv(time=eval(parse(text = paste("statdata$", Responseqq))), event = eval(parse(text = paste("statdata$", Censorshipqq)))) ~ statdata$Groupzzz, conf.type = "plain",  conf.int=sig)

nogps<-length(unique(eval(parse(text = paste("statdata$", Groupqq)))))

#vector of Group names
gpnames<-levels(as.factor(eval(parse(text = paste("statdata$", Groupqq)))))
rnms<-c(1:(length(gpnames)))
for (i in 1:length(rnms)) {
	rnms[i]=gpnames[i]
}

#===================================================================================================================
#Output HTML header
Title <-paste(branding, " Survival Analysis", sep="")
HTML.title(Title, HR = 1, align = "left")

#Software developement version warning
if (Betawarn == "Y") {
	HTML.title("Warning", HR=2, align="left")
	HTML(BetaMessage, align="left")
}

#Response and factor
HTML.title("Analysis information", HR=2, align="left")

title<-paste("The response ", Responseqq ,  " has been analysed in the Survival Analysis module, with " ,sep="")
if (Groupqq != "NULL") {
	title<-paste(title, "treatment factor " , Groupqq , " and " , sep="")
}
title<-paste(title, "censorship variable " , Censorshipqq , "." , sep="")
HTML(title, align="left")

title<-paste("Note, in this analysis ", branding , " assumes that censored observations are given the value 0 in the censorship variable ", Censorshipqq , " and 1 otherwise." , sep="")
HTML(title, align="left")

#===================================================================================================================
#Statistical results
#===================================================================================================================
if (SummaryResults == "Y") {
	HTML.title("Summary results", HR=2, align="left")

	#Creating a simple table to output
	table1<-data.frame(summary(mfit)$table)
	table2 <- table1
	for (i in 1:nogps) {
		table2[i,7] = format(round(table1[i,7],1),nsmall=1)
	}
	table2<-subset(table2, select = -c(rmean, se.rmean.))
	table2<- cbind(rnms, table2)
	lowerz <- paste("Lower ", sig*100, "% CI", sep="")
	upperz <- paste("Upper ", sig*100, "% CI", sep="")
	colnames(table2)<-c("Group" , "Records","n","Start size","Events","Median", lowerz , upperz)
	HTML(table2, classfirstline = "second", align = "left", row.names = "FALSE")
}

if (ComparingCurves == "Y") {
	#Test results
	diff<-survdiff(Surv(time=eval(parse(text = paste("statdata$", Responseqq))), event = eval(parse(text = paste("statdata$", Censorshipqq)))) ~ eval(parse(text = paste("statdata$", Groupqq))))

	testresults<-matrix(nrow=nogps,ncol=7)
	for (i in 1:nogps) {
		testresults[i,1] = diff$n[i]
		testresults[i,2] = diff$obs[i]
		testresults[i,3] = format(round(diff$exp[i],2),nsmall=2)
		testresults[i,4] = format(round(((diff$obs[i] - diff$exp[i])^2)/diff$exp[i],2),nsmall=2)
		testresults[i,5] = format(round(((diff$obs[i] - diff$exp[i])^2)/diff$var[i,i],2),nsmall=2)
		testresults[1,6] = format(round(diff$chi,2),nsmall=2)
		testresults[1,7] = format(round(1-pchisq(diff$chi,(nogps-1)),4),nsmall=4)
	}
	zzz<-testresults[1,7]

	#STB - March 2011 Change p-value to be less than 0.0001
	if (1-pchisq(diff$chi,(nogps-1))<0.0001) {
	     testresults[1,7]=format(round(0.0001, 4), nsmall=4, scientific=FALSE)
	     testresults[1,7]<- paste("<",testresults[1,7])
	}

	testresults<-data.frame(testresults)
	testresults<- cbind(rnms, testresults)
	colnames(testresults)<-c("Group", "N", "Observed", "Expected", "(O-E)^2/E", " (O-E)^2/V", "Chi-sq", "p-value")

	HTML.title("Comparing survival curves", HR=2, align="left")
	HTML(testresults, classfirstline="second", align="left", row.names = "FALSE")

	if(as.numeric(zzz) < (1-sig)) {
		HTML(paste("Conclusion: There was a significant difference between the survival curves at the ", 100*(1-sig) , "% level.", sep=""), align="left")
	} else {
		HTML(paste("Conclusion: The survival curves were not statistically significantly different at the ", 100*(1-sig) , "% level.", sep=""), align="left")
	}
	HTML("This analysis implements the G-rho family of Harrington and Fleming (1982), with weights on each death of 1, where S is the Kaplan-Meier estimate of survival. This is the log-rank or Mantel-Haenszel test.", align="left")

	#Bate and Clark comment
	HTML(refxx, align="left")	
}

if (ShowPlot == "Y") {
	#Code to print table in the HTML output
	HTML.title("Kaplan-Meier survival plot", HR=2, align="left")

	plotx <- sub(".html", "plotx.png", htmlFile)
	png(plotx,width = jpegwidth, height = jpegheight, units="in", res=PlotResolution)

	#STB July2013
	plotFilepdf1 <- sub(".html", "plotx.pdf", htmlFile)
	dev.control("enable") 

	#Creating Dataset
	ystratalabs <- as.character(levels(summary(mfit)$strata))
	m <- max(nchar(ystratalabs))
	ystrataname <- "Strata"
	times <- seq(0, max(mfit$time), by = 100)

	grdata <- data.frame(time = mfit$time, n.risk = mfit$n.risk, n.event = mfit$n.event, surv = mfit$surv, strata = summary(mfit, censored = T)$strata, upper = mfit$upper, lower = mfit$lower)
	levels(grdata$strata) <- ystratalabs
	zeros <- data.frame(time = 0, surv = 1, strata = factor(ystratalabs, levels=levels(grdata$strata)), upper = 1, lower = 1)
	grdata <- rbind.fill(zeros, grdata)
	grdata2<-cbind(grdata, read.table(text = as.character(grdata$strata), sep = "="))
	grdata2$first_IVS_cat<- Grouprr
#	grdata2$V3 <- paste(grdata2$first_IVS_cat, "=",grdata2$V2, "  ", sep = "") 
	grdata2$V3 <- grdata2$V2

	#Testing for groups with no censored data and re-ordering dataset if necessary
	infiniteslope<-"N"
	nlevels <-length(unique(levels(as.factor(grdata2$V2))))
	grdata2$catvartest <- "Y" # new variable to aid subsetting for no censored data
	testdata<-grdata2[1,]     # create a new dataset with the same header names as graphdata
	normalfaclevs<-c()	   # variables to re-order dataset
	singlefaclevs<-c()	   #variables to re-order dataset
	i<-1
	j<-1

	#Code to re-order dataset if best-fit line is infinite
	for (k in 1:nlevels) {
		tempdata<-subset(grdata2, grdata2$V3 == unique(levels(as.factor(grdata2$V3)))[k])
		tempdata<-tempdata[complete.cases(tempdata[,c("time","surv")]),]

		if(min(tempdata$n.event, na.rm=TRUE)!=0) {
			infiniteslope<-"Y"
			tempdata$catvartest <- "N"
			singlefaclevs[i] <- unique(levels(as.factor(grdata2$V3)))[k]
			i=i+1
		} else {
			normalfaclevs[j] <- unique(levels(as.factor(grdata2$V3)))[k]
			j=j+1
		}
		testdata<-rbind(testdata,tempdata)  #restack dataset with catvartest updated
		catfactlevz<-c(normalfaclevs,singlefaclevs)
	}
	grdata2<-testdata[-1,] #need to remove first row that was used to setup testdata

	#reorder levels for categorised plot
	if (infiniteslope == "Y"){
		grdata2$V3 <- factor(grdata2$V3, levels = catfactlevz ,ordered = TRUE)
	}

	#GGPLOT2 code	
	SURVIVALPLOT()
	#===================================================================================================================
	void <- HTMLInsertGraph(GraphFileName=sub("[A-Z0-9a-z,:,\\\\]*App_Data[\\\\]","", plotx), Align="centre")

	#STB July2013
	if (pdfout=="Y") {
		pdf(file=sub("[A-Z0-9a-z,:,\\\\]*App_Data[\\\\]","", plotFilepdf1), height = pdfheight, width = pdfwidth) 
		dev.set(2) 
		dev.copy(which=3) 
		dev.off(2)
		dev.off(3)
		pdfFile_1<-sub("[A-Z0-9a-z,:,\\\\]*App_Data[\\\\]","",plotFilepdf1)
		linkToPdf1 <- paste ("<a href=\"",pdfFile_1,"\">Click here to view the PDF of the scatterplot</a>", sep = "")
		HTML(linkToPdf1)
	}

	#Oct 2013 - inclusion of explanation of x on the KM plot
	if (suppressWarnings(min(eval(parse(text = paste("statdata$", Censorshipqq))), na.rm = TRUE)) == 0 ) {
		HTML("Censored observations are highlighted on the Kaplan-Meier survival plot with a filled circle.", align="left")
	}
}

#===================================================================================================================
#References
#===================================================================================================================
Ref_list<-R_refs()

HTML.title("References", HR=2, align="left")
HTML(Ref_list$IVS_ref, align="left")
HTML(Ref_list$BateClark_ref,  align="left")

if (ComparingCurves == "Y") {
	HTML("Harrington, D.P. and Fleming, T.R. (1982). A class of rank test procedures for censored survival data. Biometrika 69, 553-566.", align="left")
}

HTML.title("R references", HR=4, align="left")
HTML(reference("R2HTML"))
HTML(reference("GGally"))
HTML(reference("RColorBrewer"))
HTML(reference("ggplot2"))
HTML(reference("ggrepel"))
HTML(reference("reshape"))
HTML(reference("plyr"))
HTML(reference("scales"))
HTML(reference("proto"))

HTML(reference("survival"))
HTML(reference("gridExtra"))


#===================================================================================================================
#Show dataset
#===================================================================================================================
if (showdataset=="Y") {
	observ <- data.frame(c(1:dim(statdata_print)[1]))
	colnames(observ) <- c("Observation")
	statdata_print2 <- cbind(observ, statdata_print)

	HTML.title("Analysis dataset", HR = 2, align = "left")
    	HTML(statdata_print2, classfirstline = "second", align = "left", row.names = "FALSE")

}

#===================================================================================================================
#Show arguments
#===================================================================================================================
if (OutputAnalysisOps == "Y") {
	HTML.title("Analysis options", HR=2, align="left")

	HTML(paste("Response variable: ", Responseqq, sep=""), align="left")
	HTML(paste("Treatment factor: ", Groupqq, sep=""),  align="left")
	HTML(paste("Censorship variable: ", Censorshipqq, sep=""),  align="left")
	HTML(paste("Output summary results (Y/N): ", SummaryResults, sep=""), align="left")
	HTML(paste("Output Kaplan Meir survival plot: ", ShowPlot, sep=""),  align="left")
	HTML(paste("Output survival curve comparison (Y/N): ", ComparingCurves, sep=""),  align="left")
	HTML(paste("Significance level: ", 1-sig, sep=""),  align="left")
}
