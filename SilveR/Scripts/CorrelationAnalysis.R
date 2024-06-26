﻿#===================================================================================================================
#R Libraries

suppressWarnings(library(R2HTML))
suppressWarnings(library(Hmisc))
#===================================================================================================================
# retrieve args
Args <- commandArgs(TRUE)

#Read in data
statdata <- read.csv(Args[3], header=TRUE, sep=",")

#Copy Args
csResponses<-Args[4]
responseTransform<-tolower(Args[5])
firstCat<-Args[6]
secondCat<-Args[7]
thirdCat<-Args[8]
fourthCat<-Args[9]
method<-tolower(Args[10])
hypothesis<-tolower(Args[11])
estimate<-Args[12]
statistic<-Args[13]
pValueSelected<-Args[14]
scatterplotSelected<-Args[15]
matrixPlotSelected<-Args[16]
CIval<-1- as.numeric(Args[17])
ByCategoriesAndOverall<-Args[18]

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
statdataprint<-statdata

#Graphical parameters
graphdata<-statdata

XLimLow <- "NULL"
XLimHigh <- "NULL"
YLimLow <- "NULL"
YLimHigh <- "NULL"
Labelz_IVS_ <- "N"
ReferenceLine <- "NULL"

if (method == "pearson") {method2="Pearson"} 
if (method == "kendall") {method2="Kendall"} 
if (method == "spearman") {method2="Spearman"} 

if (hypothesis == "2-sided") {hypothesis2="two.sided"} 
if (hypothesis == "less than") {hypothesis2="less"} 
if (hypothesis == "greater than") {hypothesis2="greater"} 

#Breakdown the list of responses
resplist<-c()
tempChanges <-strsplit(csResponses, ",")
expectedChanges <- c(0)
for (i in 1:length(tempChanges[[1]]))  {
	expectedChanges [length(expectedChanges )+1] = (tempChanges[[1]][i]) 
} 
for (j in 1:(length(expectedChanges)-1))  {
	resplist[j] = expectedChanges[j+1] 
} 
resplength<-length(resplist)

responses<- paste ( "~" , c(resplist[1]), sep="")
for (i in 2:resplength) {
	responses<- paste(responses, c(" + "), resplist[i], sep="")
}

#===================================================================================================================
#Output HTML header and description
Title <-paste(branding, " Correlation Analysis", sep="")
HTML.title(Title, HR = 1, align = "left")

#Software developement version warning
if (Betawarn == "Y") {
	HTML.title("Warning", HR=2, align="left")
	HTML(BetaMessage, align="left")
}

#Title
title<-"Responses"
if (responseTransform != "none") {
	title<-paste(title, c(" and transformations"), sep="")
}
HTML.title(title, HR=2, align="left")

#Description
add<- "This module assesses the correlation between the response variables "
if (resplength >2 ){
	for (i in 1: (resplength-2)) {
		add<-paste(add, resplist[i], ", ", sep="")
	}
}
add<-paste(add, resplist[(resplength-1)], " and ", resplist[(resplength)], ". ", sep="")

if (responseTransform != "none") {
	add<-paste(add, c("The responses have been "), responseTransform, " transformed prior to analysis.", sep="")
}
HTML(add, align="left")

#===================================================================================================================
#===================================================================================================================
#Non-Categorised analysis
#===================================================================================================================
#===================================================================================================================
if ( (firstCat == "NULL" && secondCat == "NULL" && thirdCat == "NULL" && fourthCat == "NULL") || ((firstCat != "NULL"  ||  secondCat != "NULL" ||  thirdCat != "NULL"  ||  fourthCat != "NULL") && ByCategoriesAndOverall == "Y")) { 

#===================================================================================================================
#Non-categorised results
#===================================================================================================================
if ((firstCat != "NULL"  ||  secondCat != "NULL" ||  thirdCat != "NULL"  ||  fourthCat != "NULL") && ByCategoriesAndOverall == "Y") { 
	#Title for noncat plots
	HTML.title("Non-categorised results", HR=2, align="left")
}

qt<-1
for (i in 1:resplength) {
	for (j in 1:resplength) {
		if (i != j) {
			statdata$i = eval(parse(text = paste("statdata$",tempChanges[[1]][i])))
			statdata$j = eval(parse(text = paste("statdata$",tempChanges[[1]][j])))
			statdata$ij = statdata$i + statdata$j
			if(length(unique(na.omit(statdata$ij)))<=2) {
				qt<-2
			}
		}
	}
}
if(qt==2) {
	HTML.title("Warning", HR=2, align="left")
	title<- "Warning: One or more of the responses consists of only one or two observations, please remove these prior to the analysis as it is not possible to calculate correlations with them"
	HTML("Warning: One or more of the responses consists of only one or two observations, please remove these prior to the analysis as it is not possible to calculate correlations with them.", align="left")
}

if (scatterplotSelected != "N") {
	#Scatterplot
	if (resplength == 2) {
		HTML.title("Scatterplot of the observed data", HR=3, align="left")
	} else {
		HTML.title("Scatterplots of the observed data", HR=3, align="left")
	}

	for (i in 1:resplength) {
		for (j in 1:resplength) {
			if (i != j) {
				scatterPlot <- sub(".html", paste(i,j,"scatterPlot.png",sep=""), htmlFile)
				png(scatterPlot,width = jpegwidth, height = jpegheight, units="in", res=PlotResolution)

				#STB July2013
				plotFilepdf1 <- sub(".html", paste(i,j,"scatterPlot.pdf",sep=""), htmlFile)
				dev.control("enable") 

				graphdata <-statdata
				graphdata$xvarrr_IVS = eval(parse(text = paste("statdata$",tempChanges[[1]][i])))
				graphdata$yvarrr_IVS = eval(parse(text = paste("statdata$",tempChanges[[1]][j])))
	
				MainTitle2<-""
				w_Gr_jitscat <- 0
				h_Gr_jitscat <- 0
				infiniteslope <- "N"
				LinearFit <- "Y"
				Gr_alpha <- 1
				Line_type <-Line_type_solid

				XAxisTitle<-tempChanges[[1]][i]
				YAxisTitle<-tempChanges[[1]][j]

				#replace illegal characters in variable names
				for (z in 1:10) {
					YAxisTitle<-namereplace(YAxisTitle)
				}

				for (z in 1:10) {
					XAxisTitle<-namereplace(XAxisTitle)	
				}

				if (responseTransform != "none") {
					XAxisTitle<-axis_relabel(responseTransform, XAxisTitle)
				}
				if (responseTransform != "none") {
					YAxisTitle<-axis_relabel(responseTransform, YAxisTitle)
				}

				#GGPLOT2 code
				NONCAT_SCAT("NORMAL")
				void <- HTMLInsertGraph(GraphFileName=sub("[A-Z0-9a-z,:,\\\\]*App_Data[\\\\]","", scatterPlot), Align="centre")

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
			}
		}
	
		if (qt == 2) {
			HTML("Warning: One or more of the lines have not been fitted as there is only one pair of responses available.", align="left")
		}
	}
}

#===================================================================================================================
#Matrixplot
#===================================================================================================================
if (matrixPlotSelected != "N") {
	YAxisTitle<-csResponses

	#Scatterplot
	HTML.title("Matrixplot, including linear regression lines", HR=3, align="left")

	scatterPlotx <- sub(".html", "scatterPlotx.png", htmlFile)
	png(scatterPlotx,width = jpegwidth, height = jpegheight, units="in", res=PlotResolution)

	#STB July2013
	plotFilepdf2 <- sub(".html", "scatterPlotx.pdf", htmlFile)
	dev.control("enable") 

	matrixdata <- data.frame(xvarrr_IVS = NA, yvarrr_IVS = NA, firstcatvarrr_IVS = NA, secondcatvarrr_IVS=NA)

	for (i in 1:resplength) {
		for (j in 1:resplength) {
			if (i != j) {
				xvarrr_IVS = eval(parse(text = paste("statdata$",tempChanges[[1]][i])))
				yvarrr_IVS = eval(parse(text = paste("statdata$",tempChanges[[1]][j])))
				secondcatvarrr_IVS<-rep(tempChanges[[1]][i], length(xvarrr_IVS))
				firstcatvarrr_IVS<-rep(tempChanges[[1]][j], length(xvarrr_IVS))

				#removing illegal characters using GSUB command
				secondcatvarrr_IVS<-namereplaceGSUB(secondcatvarrr_IVS)
				firstcatvarrr_IVS<-namereplaceGSUB(firstcatvarrr_IVS)

				tempdata<-data.frame(xvarrr_IVS,yvarrr_IVS,firstcatvarrr_IVS,secondcatvarrr_IVS)
				matrixdata<-rbind(matrixdata,tempdata)
			}
		}
	}

	if (responseTransform != "none") {
		for (i in 1: dim(matrixdata)[1]) {
			matrixdata$secondcatvarrr_IVS[i]<-axis_relabel(responseTransform, matrixdata$secondcatvarrr_IVS[i])
		}
		for (i in 1: dim(matrixdata)[1]) {
			matrixdata$firstcatvarrr_IVS[i]<-axis_relabel(responseTransform, matrixdata$firstcatvarrr_IVS[i])
		}
	}


	graphdata<-matrixdata[-1,]
	graphdata$catfact <- paste(graphdata$firstcatvarrr_IVS, graphdata$secondcatvarrr_IVS, sep = "")
	graphdata$l_l <- paste(graphdata$firstcatvarrr_IVS, graphdata$secondcatvarrr_IVS, sep = "")

	Gr_alpha <- 1
	Line_type <-Line_type_solid
	w_Gr_jitscat <- 0
	h_Gr_jitscat <- 0
	ScatterPlot<-"Y"
	LinearFit <- "Y"
	FirstCatFactor <- "firstcatvarrr_IVS"
	SecondCatFactor <- "secondcatvarrr_IVS"
	YAxisTitle <- ""
	XAxisTitle <- ""
	scalexy<-"free"
	MainTitle2<-""
	GraphStyle <- "Overlaid"

	#Testing for with infinite slopes on scatterplot and re-ordering dataset if necessary
	inf_slope<-IVS_F_infinite_slope()
	infiniteslope <- inf_slope$infiniteslope
	graphdata<-inf_slope$graphdata

	#GGPLOT2 code
	NONCAT_MAT()

	void <- HTMLInsertGraph(GraphFileName=sub("[A-Z0-9a-z,:,\\\\]*App_Data[\\\\]","", scatterPlotx), Align="centre")

	#STB July2013
	if (pdfout=="Y") {
			pdf(file=sub("[A-Z0-9a-z,:,\\\\]*App_Data[\\\\]","", plotFilepdf2), height = pdfheight, width = pdfwidth) 
			dev.set(2) 
			dev.copy(which=3) 
			dev.off(2)
			dev.off(3)
			pdfFile_2<-sub("[A-Z0-9a-z,:,\\\\]*App_Data[\\\\]","",plotFilepdf2)
			linkToPdf2 <- paste ("<a href=\"",pdfFile_2,"\">Click here to view the PDF of the matrixplot</a>", sep = "")
			HTML(linkToPdf2)
	}
}

#===================================================================================================================
#Correlation table
#===================================================================================================================
index<-1
colsno <-5
if (estimate=="Y") {
	colsno = colsno+1
}
if (statistic=="Y") {
	colsno = colsno+1
}
if (pValueSelected=="Y") {
	colsno = colsno+1
}

#if (confidenceLimitsSelected=="Y" && method == "pearson") {colsno = colsno+2}
tablelen<-resplength*(resplength-1)/2
correlationTable <-matrix(ncol=colsno,nrow=tablelen)

k<-1

#Table column names
rownams<-matrix(ncol=colsno,nrow=1)
rownams[1,1]="Correlation"
rownams[1,2]="First variable"
rownams[1,3]=" "
rownams[1,4]="Second variable"
rownams[1,5]="n"
l<-6
if (estimate=="Y")  {
	rownams[1,l]="Correlation Coefficient"
	l=l+1
}

if (statistic=="Y") {
	rownams[1,l]="Test statistic"
	l=l+1
}

if (pValueSelected=="Y")  {
	rownams[1,l]="p-value"
}

indexn<-1


#Create a matrix correlation table
MatrixTableCoef <-matrix(ncol=resplength,nrow=resplength)
for (i in 1:resplength) {
	MatrixTableCoef[i,i] = 1
}
MatrixTableP <-matrix(ncol=resplength,nrow=resplength)
MatrixTablePara <-matrix(ncol=resplength, nrow = resplength)


id <- 1
for (i in 1:(resplength-1)) {
	for (j in (i+1):resplength) {
		correlationTable[k,1]=id
		id=id+1
		statdata$x_IVS_ = eval(parse(text = paste("statdata$",resplist[i])))
		statdata$y_IVS_ = eval(parse(text = paste("statdata$",resplist[j])))

		# calculating n
		statdata$x_IVS_ = eval(parse(text = paste("statdata$",resplist[i])))
		statdata$y_IVS_ = eval(parse(text = paste("statdata$",resplist[j])))
		statdata$xy_IVS_ = statdata$x_IVS_ + statdata$y_IVS_
		testz<-na.omit(statdata$xy_IVS_)
		pairn<-length(testz)

		if (pairn<50 && method == "kendall") {
			indexn<-2
		}

		if (pairn<10 && method == "spearman") {
			indexn<-2
		}

		if (pairn>=3) {
			correlation<-cor.test(~x_IVS_+y_IVS_, data=statdata, alternative=hypothesis2, method = method, conf.level=(1-CIval), exact = FALSE)
			correlationTable[k,2]=resplist[i]
			correlationTable[k,3]=" vs. "
			correlationTable[k,4]=resplist[j]
			correlationTable[k,5]=pairn

			MatrixTablePara[(i),(j)] = pairn
			MatrixTablePara[(j),(i)] = pairn			

			l<-6
		
			if (estimate=="Y")  {
				xstat<- format(round(correlation$estimate,3),nsmall=3)
				correlationTable[k,l]=xstat
				MatrixTableCoef[(i),(j)] = xstat
				MatrixTableCoef[(j),(i)] = xstat
				l=l+1
			}
	
			myvars <- c("x_IVS_", "y_IVS_")
			testdata2 <- statdata[myvars]
			testdata2 <- na.omit(testdata2)
		
			if (statistic=="Y") {
				Wstat<- format(round(correlation$statistic,3),nsmall=3)
				correlationTable[k,l]=Wstat
				l=l+1
			}
	
			if (pValueSelected=="Y") {
				pvalue<-format(round(correlation$p.value, 4), nsmall=4, scientific=FALSE)
				if (correlation$p.value<0.0001)  {
					pvalue=format(round(0.0001, 4), nsmall=4, scientific=FALSE)
					pvalue<- paste("<",pvalue)
				}
		
				correlationTable[k,l]=pvalue
				MatrixTableP[(i),(j)] = pvalue
				MatrixTableP[(j),(i)] = pvalue
			}
			k=k+1
		}
	}
}

#Add transformation label to the results table - not implemented for consitency with other modules
#if (responseTransform != "none") {
#	for (i in 1: dim(correlationTable)[1]) {
#		correlationTable[i,2]<-axis_relabel(responseTransform, correlationTable[i,2])
#	}
#	for (i in 1: dim(correlationTable)[1]) {
#		correlationTable[i,4]<-axis_relabel(responseTransform, correlationTable[i,4])
#	}
#}

if (k>1) {
	difference <- tablelen - (k-1)
	correlationTable2 <-matrix(ncol=colsno,nrow=(tablelen - difference))

	for (i in 1:colsno) {
		for (j in 1:(tablelen- difference)) {
			correlationTable2[j,i] = correlationTable[j,i]
		}
	}
	colnames(correlationTable2)<-rownams

	title <- c("Table of results")
	if (pValueSelected=="Y")  {
		title<-paste(title, c(" for the "), sep="")	
		if (hypothesis == "2-sided")  {
			title<-paste(title, c(" two-sided "), sep="")
		} 
		if (hypothesis == "less than") {
			title<-paste(title, c(" one-sided (less than) "), sep="")
		} 
		if (hypothesis == "greater than") {
			title<-paste(title, c(" one-sided (greater than) "), sep="")
		} 
		
		title<-paste(title, method2, c("'s "), sep="")	
	
		if (method == "pearson") {
			title<-paste(title, c("product moment "), sep="")
		}
	
		if (method == "kendall") {
			title<-paste(title, c("tau "), sep="")
		}
	
		if (method == "spearman") {
			title<-paste(title, c("rho "), sep="")
		}
		title<-paste(title, c("correlation coefficient"), sep="")	
	}
	
	HTML.title(title, HR=3, align="left")
	HTML(correlationTable2, classfirstline="second", align="left", row.names = "FALSE")

	#Correlation Matrix for Coefficients
	if (estimate=="Y")  {
		Columnnames <- c(resplist)
		Rownames<- c(" ", resplist)
		MatrixTableCoef <- data.frame(cbind(Columnnames, MatrixTableCoef))
		colnames(MatrixTableCoef) <- Rownames
		HTML.title("Matrix table of correlation coefficients", HR=3, align="left")
		HTML(MatrixTableCoef, classfirstline="second", align="left", row.names = "FALSE")
	}
	#Correlation Matrix for Coefficients
	if (pValueSelected=="Y")  {
		Columnnames <- c(resplist)
		Rownames<- c(" ", resplist)
		MatrixTableP <- data.frame(cbind(Columnnames, MatrixTableP))
		colnames(MatrixTableP) <- Rownames
		HTML.title("Matrix table of p-values", HR=3, align="left")
		HTML(MatrixTableP, classfirstline="second", align="left", row.names = "FALSE")
	}

	#Correlation Matrix for Sampel size
	if (pValueSelected=="Y" || estimate=="Y")  {
		Columnnames <- c(resplist)
		Rownames<- c(" ", resplist)
		MatrixTablePara <- data.frame(cbind(Columnnames, MatrixTablePara))
		colnames(MatrixTablePara) <- Rownames
		HTML.title("Matrix table of sample sizes", HR=3, align="left")
		HTML(MatrixTablePara, classfirstline="second", align="left", row.names = "FALSE")
	}

	CIval3 <- 100-100*CIval
	corp<-dim(correlationTable2)[2]

	add<-paste(c("Conclusion"))
	inte<-1
	for(i in 1:(dim(correlationTable2)[1])) {
		if (correlationTable2[i,corp] <= (1-CIval)|| correlationTable2[i,corp] == "< 0.0001" ) {
			if (inte==1) {
				inte<-inte+1
				add<-paste(add, ": The following pairwise correlations are statistically significantly at the  ", sep="")
				add<-paste(add, CIval3, sep="")
				add<-paste(add, "% level: ", sep="")
				add<-paste(add, correlationTable2[i,2], " vs. ", correlationTable2[i,4], sep="")
			} else {
				inte<-inte+1
				add<-paste(add, ", ", sep="")
				add<-paste(add, correlationTable2[i,2], " vs. ", correlationTable2[i,4], sep="")
			}
		} 
	}
	if (inte==1) {
	add<-paste(add, ": There are no statistically significant correlations", sep="")
	} 
	add<-paste(add, ".", sep="")
	HTML(add, align="left")

}

#===================================================================================================================
#End of non-cat if statement
}


#===================================================================================================================
#===================================================================================================================
#Categorised analysis
#===================================================================================================================
#===================================================================================================================

if (firstCat != "NULL" || secondCat != "NULL" || thirdCat != "NULL" || fourthCat != "NULL") {

	#Analysis if categorisation factors selected
	#Setting up parameters and vectors

	length <- length(unique(levels(as.factor(statdata$catfact))))
	tablenames<-c(levels(as.factor(statdata$catfact)))
	qt<-1

	HTML.title("Categorised results", HR=2, align="left")
#===================================================================================================================
#Non-cat plots
#===================================================================================================================
if (scatterplotSelected != "N") {
	#Scatterplot
	if (resplength == 2) {
		HTML.title("Categorised scatterplot of the observed data", HR=3, align="left")
	} else {
		HTML.title("Categorised scatterplots of the observed data", HR=3, align="left")
	}

	for (d in 1:resplength) {
		for (f in 1:resplength) {
			if ( d != f) {
				ncscatterplot3 <- sub(".html", paste(d,f,"ncscatterplot3.png",sep=""), htmlFile)
				png(ncscatterplot3,width = jpegwidth, height = jpegheight, units="in", res=PlotResolution)

				#STB July2013
				plotFilepdf3 <- sub(".html", paste(d,f,"ncscatterplot3.pdf",sep=""), htmlFile)
				dev.control("enable") 

				graphdata <-statdata
				Gr_palette<-palette_FUN("catfact")

				graphdata$xvarrr_IVS = eval(parse(text = paste("statdata$",tempChanges[[1]][d])))
				graphdata$yvarrr_IVS = eval(parse(text = paste("statdata$",tempChanges[[1]][f])))
				graphdata$l_l <- as.factor(graphdata$catfact)

				MainTitle2<-""
				w_Gr_jitscat <- 0
				h_Gr_jitscat <- 0
				infiniteslope <- "N"
				LinearFit <- "Y"
				Gr_alpha <- 1
				Line_type <-Line_type_solid
				#Legendpos<-Legend_pos_pres
				ScatterPlot <- "Y"
				GraphStyle <- "Overlaid"
				FirstCatFactor <- "firstcatvarrr_IVS"
				SecondCatFactor <- "secondcatvarrr_IVS"
	
				XAxisTitle<-tempChanges[[1]][d]
				YAxisTitle<-tempChanges[[1]][f]

				#replace illegal characters in variable names
				for (z in 1:10) {
					YAxisTitle<-namereplace(YAxisTitle) 
				}

				for (z in 1:10) {
					XAxisTitle<-namereplace(XAxisTitle) 
				}

				if (responseTransform != "none") {
					XAxisTitle<-axis_relabel(responseTransform, XAxisTitle)
				}
				if (responseTransform != "none") {
					YAxisTitle<-axis_relabel(responseTransform, YAxisTitle)
				}

				#Testing for with infinite slopes on scatterplot and re-ordering dataset if necessary
				inf_slope<-IVS_F_infinite_slope_Cor()
				infiniteslope <- inf_slope$infiniteslope
				graphdata<-inf_slope$graphdata
				graphdatax <- subset(graphdata, catvartest != "N")
				graphdata<-graphdatax

				#GGPLOT2 code
				OVERLAID_SCAT()
				void<-HTMLInsertGraph(GraphFileName=sub("[A-Z0-9a-z,:,\\\\]*App_Data[\\\\]","", ncscatterplot3), Align="centre")

				#reorder levels for categorised plot
				if (infiniteslope == "Y" && GraphStyle == "Overlaid") {
					HTML("Warning: One or more of the lines have not been fitted as there is only one pair of responses available. The single point has also been excluded from the plot. Note the colours employed on this plot are not consistent with others within this module as it was not possible to fit a regression line for one or more groups.", align="left")
				}

				#STB July2013
				if (pdfout=="Y") {
					pdf(file=sub("[A-Z0-9a-z,:,\\\\]*App_Data[\\\\]","", plotFilepdf3), height = pdfheight, width = pdfwidth) 
					dev.set(2) 
					dev.copy(which=3) 
					dev.off(2)
					dev.off(3)
					pdfFile_3<-sub("[A-Z0-9a-z,:,\\\\]*App_Data[\\\\]","",plotFilepdf3)
					linkToPdf3 <- paste ("<a href=\"",pdfFile_3,"\">Click here to view the PDF of the categorised scatterplot</a>", sep = "")
					HTML(linkToPdf3)
				}
			}	
		}
	}
}

#===================================================================================================================
#Matrixplot
#===================================================================================================================
if (matrixPlotSelected != "N") {
	YAxisTitle<-csResponses

	#Scatterplot
	HTML.title("Categorised matrixplot, including linear regression lines", HR=3, align="left")

	xscatterPlotx <- sub(".html", "xscatterPlotx.png", htmlFile)
	png(xscatterPlotx,width = jpegwidth, height = jpegheight, units="in", res=PlotResolution)

	#STB July2013
	plotFilepdf4 <- sub(".html", "xscatterPlotx.pdf", htmlFile)
	dev.control("enable") 

	matrixdata <- data.frame(xvarrr_IVS = NA, yvarrr_IVS = NA, firstcatvarrr_IVS = NA, secondcatvarrr_IVS=NA, l_li=NA)

	for (s in 1:length(levels(as.factor(statdata$catfact)))) {
		statdatax<-subset(statdata, statdata$catfact == unique(levels(as.factor(statdata$catfact)))[s])

		for (i in 1:resplength) {
			for (j in 1:resplength) {
				if (i != j) {
					xvarrr_IVS = eval(parse(text = paste("statdatax$",tempChanges[[1]][i])))
					yvarrr_IVS = eval(parse(text = paste("statdatax$",tempChanges[[1]][j])))

					secondcatvarrr_IVS<-rep(tempChanges[[1]][i], length(xvarrr_IVS))
					firstcatvarrr_IVS<-rep(tempChanges[[1]][j], length(xvarrr_IVS))
					l_li<- unique(levels(as.factor(statdata$catfact)))[s]

					#removing illegal characters using GSUB command
					firstcatvarrr_IVS<-namereplaceGSUB(firstcatvarrr_IVS)
					secondcatvarrr_IVS<-namereplaceGSUB(secondcatvarrr_IVS)
					l_li<-namereplaceGSUB(l_li)

					tempdata<-data.frame(xvarrr_IVS,yvarrr_IVS,firstcatvarrr_IVS,secondcatvarrr_IVS,l_li)
					matrixdata<-rbind(matrixdata,tempdata)
				}
			}
		}
	}

	#Change Axis labels if transformation performed
	if (responseTransform != "none") {
		for (i in 1: dim(matrixdata)[1]) {
			matrixdata$secondcatvarrr_IVS[i]<-axis_relabel(responseTransform, matrixdata$secondcatvarrr_IVS[i])
		}
		for (i in 1: dim(matrixdata)[1]) {
			matrixdata$firstcatvarrr_IVS[i]<-axis_relabel(responseTransform, matrixdata$firstcatvarrr_IVS[i])
		}
	}

	graphdata<-matrixdata[-1,]
	graphdata$catfact <- paste(graphdata$firstcatvarrr_IVS, graphdata$secondcatvarrr_IVS, sep = "")
	graphdata$l_l <- paste(graphdata$firstcatvarrr_IVS, graphdata$secondcatvarrr_IVS, graphdata$l_li, sep = "")

	Gr_alpha <- 1
	Line_type <-Line_type_solid
	w_Gr_jitscat <- 0
	h_Gr_jitscat <- 0
	ScatterPlot<-"Y"
	LinearFit <- "Y"
	FirstCatFactor <- "firstcatvarrr_IVS"
	SecondCatFactor <- "secondcatvarrr_IVS"
	YAxisTitle <- ""
	XAxisTitle <- ""
	scalexy<-"free"
	MainTitle2<-""
	GraphStyle <- "Overlaid"

	#Testing for with infinite slopes on scatterplot and re-ordering dataset if necessary
	inf_slope<-IVS_F_infinite_slope_Cor()
	infiniteslope <- inf_slope$infiniteslope
	graphdata<-inf_slope$graphdata
	
	# Re-order the l_li levels for the matrix plot
	graphdata$l_li <- factor(graphdata$l_li, levels = inf_slope$xcatfactlevz ,ordered = TRUE)

	Gr_palette<-palette_FUN("l_li")

	#GGplot2 code
	OVERLAID_MAT()
	void <- HTMLInsertGraph(GraphFileName=sub("[A-Z0-9a-z,:,\\\\]*App_Data[\\\\]","", xscatterPlotx), Align="centre")

#STB July2013
	if (pdfout=="Y") {
		pdf(file=sub("[A-Z0-9a-z,:,\\\\]*App_Data[\\\\]","", plotFilepdf4), height = pdfheight, width = pdfwidth) 
		dev.set(2) 
		dev.copy(which=3) 
		dev.off(2)
		dev.off(3)
		pdfFile_4<-sub("[A-Z0-9a-z,:,\\\\]*App_Data[\\\\]","",plotFilepdf4)
		linkToPdf4 <- paste ("<a href=\"",pdfFile_4,"\">Click here to view the PDF of the categorised matrixplot</a>", sep = "")
		HTML(linkToPdf4)
	}
}

#===================================================================================================================
#Correlation table
#===================================================================================================================
colsno <-6
if (estimate=="Y") {
	colsno = colsno+1
}
if (statistic=="Y") {
	colsno = colsno+1
}
if (pValueSelected=="Y") {
	colsno = colsno+1
}

tablelen<-(resplength*(resplength-1)/2)*length
correlationTable <-matrix(ncol=colsno,nrow=tablelen)
k<-1

#Table column names
rownams<-matrix(ncol=colsno,nrow=1)
rownams[1,1]="Correlation"
rownams[1,2]="Categorisation factor"
rownams[1,3]="First variable"
rownams[1,4]=" "
rownams[1,5]="Second variable"
rownams[1,6]="n"
l<-7
if (estimate=="Y")  {
	rownams[1,l]="Correlation Coefficient"
	l=l+1
}

if (statistic=="Y")  {
	rownams[1,l]="Test statistic"
	l=l+1
}

if (pValueSelected=="Y")  {
	rownams[1,l]="p-value"
}

#Create a matrix correlation table
MatrixTableCatCoef <-matrix(ncol=resplength,nrow=length*resplength)
j<-0
for (p in 1:length) {
	for (i in 1:resplength) {
		MatrixTableCatCoef[i+j,i] = 1
	}
	j<-j+resplength
}
MatrixTableCatP <-matrix(ncol=resplength,nrow=length*resplength)
MatrixTableCatPara <-matrix(ncol=resplength,nrow=length*resplength)

index<-1
testcomb<-1
indexn<-1
id<-1
q<-0
for ( p in 1:length) {
	testdata<- subset(statdata, statdata$catfact== unique(levels(as.factor(statdata$catfact)))[p])

	for (i in 1:(resplength-1)) {
		for (j in (i+1):resplength) {
			correlationTable[id,1]=id
			id=id+1
			testdata$x_IVS_ = eval(parse(text = paste("testdata$",resplist[i])))
			testdata$y_IVS_ = eval(parse(text = paste("testdata$",resplist[j])))
			testdata$xy_IVS_ = testdata$x_IVS_ + testdata$y_IVS_
			testz<-na.omit(testdata$xy_IVS_)

			pairn<-length(testz)
			if (length(testz)<50 && method == "kendall") {
				indexn<-2
			}
			if (length(testz)<10 && method == "spearman") {
				indexn<-2
			}
			if (length(testz)<=2) {
				testcomb<-2
			}

			if(length(testz)>2) {

				#STB Aug 2011 - removing lines with infinite slope
				if(length(unique(testdata$x_IVS_))!=1 && length(unique(testdata$y_IVS_))!=1) {
				correlation<-cor.test(~x_IVS_+y_IVS_, data=testdata, alternative=hypothesis2, method = method, conf.level=(1-CIval), exact = FALSE)
				} else {
					correlation$estimate<-1000
					correlation$statistic<-1000
					correlation$p.value<-1000
				}

				correlationTable[k,2]=unique(levels(as.factor(statdata$catfact)))[p]	
				correlationTable[k,3]=resplist[i]
				correlationTable[k,4]=" vs. "
				correlationTable[k,5]=resplist[j]
				correlationTable[k,6]=pairn


				MatrixTableCatPara[i+q,j] = pairn
				MatrixTableCatPara[j+q,i] = pairn	
	
				l<-7
		
				if (estimate=="Y")  {
					xstat<- format(round(correlation$estimate,3),nsmall=3)
					correlationTable[k,l]=xstat
					l=l+1
					MatrixTableCatCoef[i+q,j]=xstat
					MatrixTableCatCoef[j+q,i]=xstat

					#STB Aug 2011 - removing lines with infinite slope
					if (correlation$estimate==1000)  {
						correlationTable[k,(l-1)]<- "-"
					}
				}
	
				myvars <- c("x_IVS_", "y_IVS_")
				testdata2 <- testdata[myvars]
				testdata2 <- na.omit(testdata2)

				if (statistic=="Y")  {
					Wstat<- format(round(correlation$statistic,3),nsmall=3)
					correlationTable[k,l]=Wstat
					l=l+1

					#STB Aug 2011 - removing lines with infinite slope
					if (correlation$statistic==1000) {
						correlationTable[k,(l-1)]<- "-"
					}
				}
			
				if (pValueSelected=="Y") {
					pvalue<-format(round(correlation$p.value, 4), nsmall=4, scientific=FALSE)
					if (correlation$p.value<0.0001)  {
						pvalue=format(round(0.0001, 4), nsmall=4, scientific=FALSE)
						pvalue<- paste("<",pvalue)
					}

					#STB Aug 2011 - removing lines with infinite slope
					if (correlation$p.value==1000) {
						pvalue<- "-"
					}
			
					correlationTable[k,l]=pvalue
					MatrixTableCatP[i+q,j]=pvalue
					MatrixTableCatP[j+q,i]=pvalue
				}
				k=k+1
			}
		}
	}
q<- q+resplength
}


#Add transformation label to the results table - not implemented for consitency with other modules
#if (responseTransform != "none") {
#	for (i in 1: dim(correlationTable)[1]) {
#		correlationTable[i,3]<-axis_relabel(responseTransform, correlationTable[i,3])
#	}
#	for (i in 1: dim(correlationTable)[1]) {
#		correlationTable[i,5]<-axis_relabel(responseTransform, correlationTable[i,5])
#	}
#}

if (k>2) {
	difference <- tablelen - (k-1)
	correlationTable2 <-matrix(ncol=colsno,nrow=((resplength*(resplength-1)/2)*length - difference))

	for (i in 1:colsno) {
		for (j in 1:(tablelen- difference)) {
		correlationTable2[j,i] = correlationTable[j,i]
		}
	}
	colnames(correlationTable2)<-rownams

	#Print of Expected table
	title<-c("Table of results")
	if (pValueSelected=="Y") {
		title<-paste(title, c(" for the "), sep="")	
	
		if (hypothesis == "2-sided")  {
			title<-paste(title, c(" two-sided "), sep="")
		} 
		if (hypothesis == "less than") {
			title<-paste(title, c(" one-sided (less than) "), sep="")
		} 
		if (hypothesis == "greater than")  {
			title<-paste(title, c(" one-sided (greater than) "), sep="")
		} 
	
		title<-paste(title, method2, c("'s "), sep="")	
	
		if (method == "pearson") {
			title<-paste(title, c("product moment "), sep="")
		}
		if (method == "kendall") {
			title<-paste(title, c("tau "), sep="")
		}
		if (method == "spearman") {
			title<-paste(title, c("rho "), sep="")
		}
		title<-paste(title, c("correlation coefficient "), sep="")	
	}
	
	HTML.title(title, HR=3, align="left")
	HTML(correlationTable2, classfirstline="second", align="left", row.names="FALSE")


	#Correlation Matrix for Coefficients
	if (estimate=="Y")  {
		Catnames <- rep(c(tablenames), each = length(resplist))
		Columnnames <- c(resplist)
		Rownames<- c("Categorisation factor", " ", resplist)
		MatrixTableCatCoef <- data.frame(cbind(Catnames, Columnnames, MatrixTableCatCoef))
		colnames(MatrixTableCatCoef) <- Rownames
		HTML.title("Matrix table of correlation coefficients", HR=3, align="left")
		HTML(MatrixTableCatCoef, classfirstline="second", align="left", row.names = "FALSE")
	}
	#Correlation Matrix for Coefficients
	if (pValueSelected=="Y")  {
		Catnames <- rep(c(tablenames), each = length(resplist))
		Columnnames <- c(resplist)
		Rownames<- c("Categorisation factor", " ", resplist)
		MatrixTableCatP <- data.frame(cbind(Catnames, Columnnames, MatrixTableCatP))
		colnames(MatrixTableCatP) <- Rownames
		HTML.title("Matrix table of p-values", HR=3, align="left")
		HTML(MatrixTableCatP, classfirstline="second", align="left", row.names = "FALSE")
	}
	#Correlation Matrix for Samepl size
	if (pValueSelected=="Y" || estimate=="Y")  {
		Catnames <- rep(c(tablenames), each = length(resplist))
		Columnnames <- c(resplist)
		Rownames<- c("Categorisation factor", " ", resplist)
		MatrixTableCatPara <- data.frame(cbind(Catnames, Columnnames, MatrixTableCatPara))
		colnames(MatrixTableCatPara) <- Rownames
		HTML.title("Matrix table of sample sizes", HR=3, align="left")
		HTML(MatrixTableCatPara, classfirstline="second", align="left", row.names = "FALSE")
	}


	
	CIval3 <- 100-100*CIval
	corp<-dim(correlationTable2)[2]
	add<-paste(c("Conclusion"))
	inte<-1
	for(i in 1:(dim(correlationTable2)[1])) {
		if (correlationTable2[i,corp] <= (1-CIval) || correlationTable2[i,corp] == "< 0.0001" ) {
			if (inte==1) {
				inte<-inte+1
				add<-paste(add, ": The following pairwise correlations are statistically significantly at the  ", CIval3, "% level: ", correlationTable2[i,3], " vs. ", correlationTable2[i,5], " (categorisation factor(s) level ", correlationTable2[i,2], ")",sep="")
			} else {
				inte<-inte+1
				add<-paste(add, ", ", correlationTable2[i,3], " vs. ", correlationTable2[i,5], " (categorisation factor(s) level ", correlationTable2[i,2], ")", sep="")
			}
		} 
	}

	if (inte==1) {
		add<-paste(add, ": There are no statistically significant correlations", sep="")
	} 
	add<-paste(add, ".", sep="")
	
	HTML(add, align="left")

	if (testcomb == 2) {
		HTML("Note that there is only one or two pairs of responses available for one or more of the level(s) of the categorisation factor(s) and hence no correlation analysis can be performed for the level(s) of the categorisation factor(s). ", align="left")
	}
} else {
	HTML.title("Warning", HR=2, align="left")
	HTML("As the replication of every combination of the levels of the categorical factor(s) for all pairs of responses is two or less, no correlation analysis has been performed.", align="left")
}

#===================================================================================================================
#End of If statement for cat analyisis
}

#===================================================================================================================
#Analysis description

HTML.title("Analysis description", HR=2, align="left")

if (method == "pearson") {
	HTML("The test statistic is based on Pearson's product moment correlation coefficient cor(x, y) and follows a t distribution with length(x)-2 degrees of freedom if the samples follow independent normal distributions, see Snedecor and Cochran (1989). ", align="left")
}
if (method == "kendall") {
	HTML("This test is a rank-based measure of association and can be used when the data are not necessarily from a bivariate normal distribution, see Hollander and Wolfe (1973). The test statistic is the estimate scaled to zero mean and unit variance, and is approximately normally distributed. ", align="left")
}
if (method == "spearman")  {
	HTML("This test is a rank-based measure of association and can be used when the data are not necessarily from a bivariate normal distribution, see Hollander and Wolfe (1973). The p-values are computed using algorithm AS 89, see Best and Roberts (1975). ", align="left")
}

#===================================================================================================================
#References
#===================================================================================================================
Ref_list<-R_refs()

#Bate and Clark comment
HTML(refxx, align="left")	

HTML.title("References", HR=2, align="left")
HTML(Ref_list$IVS_ref, align="left")
HTML(Ref_list$BateClark_ref, align="left")

if (method == "pearson") {
	HTML("Snedecor, G.W. and Cochran, W.G. (1989). Statistical Methods. 8th edition; Iowa State University Press, Iowa, USA.", align="left") 
}
if (method == "spearman") {
	HTML("Best, D.J. and Roberts, D.E. (1975). Algorithm AS 89: The Upper Tail Probabilities of Spearman's rho. Applied Statistics, 24, 377-379. ", align="left")
}
if (method == "spearman" || method == "kendall") {
	HTML("Hollander, M. and Wolfe, D.A. (1973). Nonparametric Statistical Methods. New York: John Wiley & Sons. P185-194. ", align="left")
}

HTML.title("R references", HR=4, align="left")
HTML(Ref_list$R_ref , align="left")
HTML(reference("R2HTML"))
HTML(reference("GGally"))
HTML(reference("RColorBrewer"))
HTML(reference("ggplot2"))
HTML(reference("ggrepel"))
HTML(reference("reshape"))
HTML(reference("plyr"))
HTML(reference("scales"))
HTML(reference("proto"))
HTML(reference("Hmisc"))



#===================================================================================================================
#Show dataset
#===================================================================================================================
if (showdataset=="Y") {
	HTML.title("Analysis dataset", HR=2, align="left")

	statdataprint2<-subset(statdataprint, select = -c(catfact))

        observ <- data.frame(c(1:dim(statdataprint2)[1]))
        colnames(observ) <- c("Observation")
        statdataprint2 <- cbind(observ, statdataprint2)
	HTML(statdataprint2, classfirstline="second", align="left", row.names = "FALSE")
}

#===================================================================================================================
#Show arguments
#===================================================================================================================
if (OutputAnalysisOps == "Y") {
	HTML.title("Analysis options", HR=2, align="left")
	HTML(paste("Response variables: ", csResponses, sep=""), align="left")
	if (responseTransform != "none") {
		HTML(paste("Response variables transformation: ", responseTransform, sep=""), align="left")
	}
	if (firstCat != "NULL") {
		HTML(paste("First categorisation factor: ", firstCat, sep=""), align="left")
	}
	if (secondCat != "NULL") {
		HTML(paste("Second categorisation factor: ", secondCat, sep=""), align="left")
	}
	if (thirdCat != "NULL") {	
		HTML(paste("Third categorisation factor: ", thirdCat, sep=""), align="left")
	}
	if (fourthCat != "NULL") {	
		HTML(paste("Fourth categorisation factor: ", fourthCat, sep=""), align="left")
	}
	HTML(paste("Method: ", method2, sep=""),  align="left")
	HTML(paste("Hypothesis: ", hypothesis, sep=""),  align="left")

	HTML(paste("Display correlation coefficient (Y/N): ", estimate, sep=""),  align="left")
	HTML(paste("Display test statistic (Y/N): ", statistic, sep=""),  align="left")
	HTML(paste("Display p-values (Y/N): ", pValueSelected, sep=""),  align="left")
	HTML(paste("Generate scatterplots (Y/N): ", scatterplotSelected, sep=""),  align="left")
	HTML(paste("Generate matrixplot (Y/N): ", matrixPlotSelected, sep=""),  align="left")
	HTML(paste("Significance level: ", 1-CIval, sep=""),  align="left")
	HTML(paste("Display categorised output (Y/N): ", ByCategoriesAndOverall, sep=""),  align="left")
}