﻿#===================================================================================================================
#R Libraries

suppressWarnings(library(multcomp))
suppressWarnings(library(R2HTML))
suppressWarnings(library("emmeans"))

#===================================================================================================================
# retrieve args
Args <- commandArgs(TRUE)

#Read in data
statdata <- read.csv(Args[3], header=TRUE, sep=",")

#Copy Args

#STB - July 2012 rename response variable
xxxresponsexxx <- Args[4]

responseTransform <- tolower(Args[5])
treatFactor <- Args[6]
equalCase <-Args[7]
unequalCase <- Args[8]
showPRPlot <- Args[9]
showNormPlot <- Args[10]
controlGroup <- Args[11]
sig <- 1 - as.numeric(Args[12])

#source(paste(getwd(),"/Common_Functions.R", sep=""))

#Print args
if (Diplayargs == "Y"){
	print(Args)
}

#===================================================================================================================
#Parameter setup

#Graphical parameters
graphdata<-statdata
Labelz_IVS_ <- "N"
ReferenceLine <- "NULL"
XLimLow <- "NULL"
XLimHigh <- "NULL"
YLimLow <- "NULL"
YLimHigh <- "NULL"

#Removing illegal characters
YAxisTitle <-xxxresponsexxx
XAxisTitle <-treatFactor

#replace illegal characters in variable names
for (i in 1:10) {
	YAxisTitle<-namereplace(YAxisTitle)
	XAxisTitle<-namereplace(XAxisTitle)
}

if (responseTransform != "none") {
	YAxisTitle<-axis_relabel(responseTransform, YAxisTitle)
}


#Generate mainEffect factor
statdata$mainEffect<-as.factor(eval(parse(text = paste("statdata$", treatFactor))))

#Re-ordering factor levels based on control group
if (controlGroup != "NULL") {
	temp <- c(levels(statdata$mainEffect))
	temp = temp[!(temp %in% controlGroup)]
	levs_plot <- c(controlGroup, temp)
	statdata$mainEffect <- factor(statdata$mainEffect, levels=levs_plot)
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
Title <-paste(branding, " Unpaired t-test Analysis", sep="")
HTML.title(Title, HR = 1, align = "left")

#Software developement version warning
if (Betawarn == "Y") {
	HTML.title("Warning", HR=2, align="left")
	HTML(BetaMessage, align="left")
}

#Response
HTML.title("Response", HR=2, align="left")

#STB - July 2012 rename response variable
add<-paste("The  ", xxxresponsexxx, " response is currently being analysed by the Unpaired t-test Analysis module.", sep="")
HTML(add, align="left")

if (responseTransform != "none") {
	add2<-paste(c("The response has been "), responseTransform, " transformed prior to analysis.", sep="")
	HTML(add2, align="left")
}

#Bate and Clark comment
HTML(refxx, align="left")	

#===================================================================================================================
#Scatterplot
#===================================================================================================================
title<-c("Scatterplot of the observed data")
if(responseTransform != "none") {
	title<-paste(title, " (on the ", responseTransform, " scale)", sep="")
}

HTML.title(title, HR=2, align="left")

scatterPlot <- sub(".html", "scatterPlot.png", htmlFile)
png(scatterPlot,width = jpegwidth, height = jpegheight, units="in", res=PlotResolution)

#STB July2013
plotFilepdf <- sub(".html", "scatterPlot.pdf", htmlFile)
dev.control("enable") 

#Graphical parameters
graphdata<-statdata
graphdata$yvarrr_IVS <- eval(parse(text = paste("statdata$",xxxresponsexxx)))
graphdata$xvarrr_IVS <- statdata$mainEffect
MainTitle2 <- ""
w_Gr_jitscat <- 0
h_Gr_jitscat <-  0
infiniteslope <- "Y"

NONCAT_SCAT("Normal")

void <- HTMLInsertGraph(GraphFileName=sub("[A-Z0-9a-z,:,\\\\]*App_Data[\\\\]","", scatterPlot), Align="centre")

#STB July2013
if (pdfout=="Y") {
	pdf(file=sub("[A-Z0-9a-z,:,\\\\]*App_Data[\\\\]","", plotFilepdf), height = pdfheight, width = pdfwidth) 
	dev.set(2) 
	dev.copy(which=3) 
	dev.off(2)
	dev.off(3)
	pdfFile<-sub("[A-Z0-9a-z,:,\\\\]*App_Data[\\\\]","",plotFilepdf)
	linkToPdf <- paste ("<a href=\"",pdfFile,"\">Click here to view the PDF of the scatterplot</a>", sep = "")
	HTML(linkToPdf)
}


HTML("Tip: Use this plot to identify possible outliers.", align="left")



#===================================================================================================================
#Equal variance case
#===================================================================================================================
if (equalCase == "Y") {
	HTML.title("Statistical analysis results assuming equal variances", HR=2, align="left")

	HTML.title("Unpaired t-test result", HR=3, align="left")

#STB - July 2012 rename response variable
	eqtest<-t.test(formula = eval(parse(text = paste("statdata$", xxxresponsexxx)))~statdata$mainEffect, paired = FALSE, var.equal= TRUE, conf.level= sig)

	col1<-format(round(as.numeric(eqtest[1]), 3), nsmall=3, scientific=FALSE)
	col2<-format(round(as.numeric(eqtest[2]), 0), nsmall=0, scientific=FALSE)
	col3<-format(round(as.numeric(eqtest[3]), 4), nsmall=4, scientific=FALSE)
	col4<-as.numeric(eqtest[3])
	col5<-c("Equal variance unpaired t-test")

	eqtable<-cbind(col5, col1,col2,col3)

	head<-c(" ", "t-statistic", "Degrees of freedom","p-value")
	colnames(eqtable) <- head
	for (i in 1:(dim(eqtable)[1]))  {
		if (as.numeric(col4[i])<0.0001)  {
			#STB March 2011 - formatting p-values p<0.0001
			# eqtable[i,4]<-0.0001
			eqtable[i,4]=format(round(0.0001, 4), nsmall=4, scientific=FALSE)
			eqtable[i,4]<- paste("<",eqtable[i,4])
		}
	}

	HTML(eqtable, classfirstline="second", align="left", row.names = "FALSE")

	add<-paste(c("Conclusion"))
	if (as.numeric(col4[i])<= (1-sig)) {
		add<-paste(add, ": There is a statistically significant difference between the levels of ", xxxresponsexxx, " at the ", 100*(1-sig) , "% level" , ".",  sep="")
	} else {
		add<-paste(add, ": The difference between the levels of ", xxxresponsexxx, " is not statistically significant.", sep="")
	}
	HTML(add, align="left")
}

#===================================================================================================================
#Testing the degrees of freedom
#===================================================================================================================
if (equalCase == "Y" && eqtest[2]<5) {
	HTML.title("Warning", HR=3, align="left")
	HTML("Unfortunately the residual degrees of freedom are low (less than 5). This may make the estimation of the underlying variability, and hence the results of the statistical tests, unreliable.", align="left")
}

#===================================================================================================================
# Means and Planned comparisons on the main effects
#===================================================================================================================
if (equalCase == "Y") { 
	#Code to calculate y-axis offset (lens) in LSMeans plot
	names<-levels(eval(parse(text = paste("statdata$", treatFactor))))
	index<-1
	for (i in 1:length(names)) {
		temp<-names[i]
		temp<-as.character(unlist(strsplit(as.character(names[i]),"")))
		lens<-length(temp)
		if (lens>index) {
			index <-lens
		}
	}

	mult<-glht(lm(eval(parse(text = paste("statdata$", xxxresponsexxx)))~ mainEffect, data=statdata, na.action = na.omit), linfct=mcp(mainEffect="Means"))

	tab1<-c()
	tab2<-c()
	tab3<-c()
	tabs<-confint(mult,level = sig, calpha = univariate_calpha() )
	test<-tabs$confint
	lengths<-length(unique(rownames(test)))

	for (i in 1:lengths) {
		tab1[i]<-test[i,1]
		tab2[i]<-test[i,2]
		tab3[i]<-test[i,3]
	}

	#STB Dec 2011 formatting 3dp
	tab1<-format(round(tab1, 3), nsmall=3, scientific=FALSE)
	tab2<-format(round(tab2, 3), nsmall=3, scientific=FALSE)
	tab3<-format(round(tab3, 3), nsmall=3, scientific=FALSE)

	
	tables <- cbind(rownames(test), tab1, tab2, tab3)
	colnames(tables)<-c("Treatment level", "Mean", paste("Lower ",(sig*100),"% CI",sep=""), paste("Upper ",(sig*100),"% CI",sep=""))

	#STB May 2012 Updating "least square (predicted) means"

	if ( (responseTransform != "log10" && responseTransform != "loge") || (responseTransform == "log10" && GeomDisplay != "geometricmeansonly") || (responseTransform == "loge" && GeomDisplay != "geometricmeansonly") ) {
		CITitle2<-paste("Table of the least square (predicted) means with ",(sig*100),"% confidence intervals",sep="")
		HTML.title(CITitle2, HR=3, align="left")
		HTML(tables, classfirstline="second", align="left", row.names = "FALSE")
	}

	#===================================================================================================================
	#Calculating the size of the arithmetic difference with 95%CI

	mult2<-glht(lm(eval(parse(text = paste("statdata$", xxxresponsexxx)))~ mainEffect, data=statdata, na.action = na.omit), linfct=lsm(pairwise ~mainEffect))
	multci2<-confint(mult2, level=sig, calpha = univariate_calpha())
	multp2<-summary(mult2, test=adjusted("none"))
	pvals<-multp2$test$pvalues
	sigma<-multp2$test$sigma
	tablen<-length(unique(rownames(multci2$confint)))
	tabs<-matrix(nrow=tablen, ncol=5)

	for (i in 1:tablen) {
		#STB Dec 2011 increasing means to 3dp
		tabs[i,1]=format(round(multci2$confint[i], 3), nsmall=3, scientific=FALSE)
		tabs[i,2]=format(round(multci2$confint[i+tablen], 3), nsmall=3, scientific=FALSE)
		tabs[i,3]=format(round(multci2$confint[i+2*tablen], 3), nsmall=3, scientific=FALSE)
		tabs[i,4]=format(round(sigma[i], 3), nsmall=3, scientific=FALSE)
		tabs[i,5]=format(round(pvals[i], 4), nsmall=4, scientific=FALSE)
	}
	for (i in 1:tablen) {
		if (pvals[i]<0.0001)  {
			#STB March 2011 - formatting p-values p<0.0001
			#tabs[i,5]<-0.0001
			tabs[i,5]=format(round(0.0001, 4), nsmall=4, scientific=FALSE)
			tabs[i,5]<- paste("<",tabs[i,5])
		}
	}
	

	rows<-rownames(multci2$confint)

	for (i in 1:100) {
		rows<-sub("_ivs_dash_ivs_"," - ", rows, fixed=TRUE)
	}
	lowerCI<-paste("   Lower ",(sig*100),"% CI   ",sep="")
	upperCI<-paste("   Upper ",(sig*100),"% CI   ",sep="")

	tabls<-cbind(rows, tabs)
	colnames(tabls)<-c("Comparison", "Difference", lowerCI, upperCI, "Std error", "p-value")

	if ( (responseTransform != "log10" && responseTransform != "loge") || (responseTransform == "log10" && GeomDisplay != "geometricmeansonly") || (responseTransform == "loge" && GeomDisplay != "geometricmeansonly") ) {
		add <- paste("Comparison of the  least square (predicted) means with ",(sig*100),"% confidence interval",sep="")
		HTML.title(add, HR=3, align="left")
		HTML(tabls, classfirstline="second", align="left", row.names = "FALSE")
	}
}
#===================================================================================================================
#Back transformed geometric means table 
#===================================================================================================================
if(equalCase == "Y" && (responseTransform =="log10"||responseTransform =="loge")) {

	if ( (responseTransform == "log10" && GeomDisplay != "notdisplayed") || (responseTransform == "loge" && GeomDisplay != "notdisplayed") ) {
		#Table of LS Means
		CITitle2<-paste("Table of the back-transformed geometric means with ",(sig*100),"% confidence intervals",sep="")
		HTML.title(CITitle2, HR=3, align="left")

		if (GeomDisplay == "geometricmeansandpredictedmeansonlogscale") {
			HTML("As the response was log transformed prior to analysis the least square (predicted) means are presented on the log scale. These results can be back transformed onto the original scale. These are known as the back-transformed geometric means.", align="left")
		}
		if (GeomDisplay == "geometricmeansonly") {
			HTML("As the response was log transformed prior to analysis the least square (predicted) means are presented back transformed onto the original scale. These are known as the back-transformed geometric means.", align="left")
		}

		tab1<-c()
		tab2<-c()
		tab3<-c()

		tabsx<-confint(mult,level = sig, calpha = univariate_calpha() )
		test<-tabsx$confint
		lengths<-length(unique(rownames(test)))

		for (i in 1:lengths) {
			tab1[i]<-test[i,1]
			tab2[i]<-test[i,2]
			tab3[i]<-test[i,3]
		}
	
		#STB Dec 2011 formatting 3dp
		if (responseTransform =="log10") {
			tab1<-format(round(10^(tab1), 3), nsmall=3, scientific=FALSE)
			tab2<-format(round(10^(tab2), 3), nsmall=3, scientific=FALSE)
			tab3<-format(round(10^(tab3), 3), nsmall=3, scientific=FALSE)
		}
		if (responseTransform =="loge") {
			tab1<-format(round(exp(tab1), 3), nsmall=3, scientific=FALSE)
			tab2<-format(round(exp(tab2), 3), nsmall=3, scientific=FALSE)
			tab3<-format(round(exp(tab3), 3), nsmall=3, scientific=FALSE)
		}

		tabx <- cbind(rownames(test), tab1, tab2, tab3)
		colnames(tabx)<-c("Treatment level ", "Geometric mean", paste("Lower ",(sig*100),"% CI",sep=""), paste("Upper ",(sig*100),"% CI",sep=""))
		HTML(tabx, classfirstline="second", align="left", row.names = "FALSE")

#===================================================================================================================
#Calculating the size of the geometric ratio with 95%CI
#V3.2 STB NOV2015
#===================================================================================================================
		HTML.title("Comparison of the geometric means as a back-transformed ratio", HR=3, align="left")
		HTML("As the response was log transformed prior to analysis the differences between the least square (predicted) means are presented on the log scale. These results can be back-transformed onto the original scale, where differences on the log scale become ratios when back-transformed.", align="left")

		mult<-glht(lm(eval(parse(text = paste("statdata$", xxxresponsexxx)))~ mainEffect, data=statdata, na.action = na.omit), linfct=lsm(pairwise ~mainEffect))
		multci<-confint(mult, level=sig, calpha = univariate_calpha())
		multp<-summary(mult, test=adjusted("none"))

		pvals<-multp$test$pvalues
		sigma<-multp$test$sigma
		tablen<-length(unique(rownames(multci$confint)))
		tabsz<-matrix(nrow=tablen, ncol=4)

		if (responseTransform =="log10") {
			for (i in 1:tablen) {
				tabsz[i,1]=format(round(10^(multci$confint[i]), 3), nsmall=3, scientific=FALSE)
				tabsz[i,2]=format(round(10^(multci$confint[i+tablen]), 3), nsmall=3, scientific=FALSE)
				tabsz[i,3]=format(round(10^(multci$confint[i+2*tablen]), 3), nsmall=3, scientific=FALSE)
			}
		}
		if (responseTransform =="loge") {
			for (i in 1:tablen) {
				tabsz[i,1]=format(round(exp(multci$confint[i]), 3), nsmall=3, scientific=FALSE)
				tabsz[i,2]=format(round(exp(multci$confint[i+tablen]), 3), nsmall=3, scientific=FALSE)
				tabsz[i,3]=format(round(exp(multci$confint[i+2*tablen]), 3), nsmall=3, scientific=FALSE)
			}
		}
		
		tabsz[,4]<- tabs[,5]
		rows<-rownames(multci$confint)

		#STB2019
		rows<-sub(" - "," / ", rows, fixed=TRUE)

		#STB June 2015	
		for (i in 1:100) {
			rows<-sub("_ivs_dash_ivs_"," - ", rows, fixed=TRUE)
		}
		lowerCI<-paste("   Lower ",(sig*100),"% CI   ",sep="")
		upperCI<-paste("   Upper ",(sig*100),"% CI   ",sep="")
	
		tablsz<-cbind(rows, tabsz)
		colnames(tablsz)<-c("Comparison","Ratio", lowerCI, upperCI, "p-value")
		HTML(tablsz, classfirstline="second", align="left", row.names = "FALSE")
	}
}

#===================================================================================================================
#Diagnostic plots (equal variance case)
#===================================================================================================================
if (equalCase == "Y") {
	if((showPRPlot=="Y" && showNormPlot=="N") || (showPRPlot=="N" && showNormPlot=="Y") ) {
			HTML.title("Diagnostic plot (assuming equal variances)", HR=2, align="left")
	}
	if(showPRPlot=="Y" && showNormPlot=="Y") {
			HTML.title("Diagnostic plots (assuming equal variances)", HR=2, align="left")
	}

	#Residual plots
	if(showPRPlot=="Y") {
		HTML.title("Residuals vs. predicted plot", HR=3, align="left")

		#STB - July 2012 rename response variable
		threewayfull<-lm(eval(parse(text = paste("statdata$", xxxresponsexxx)))~ mainEffect, data=statdata, na.action = na.omit)
		residualPlot <- sub(".html", "residualplot.png", htmlFile)
		png(residualPlot,width = jpegwidth, height = jpegheight, units="in", res=PlotResolution)

		#STB July2013
		plotFilepdf1 <- sub(".html", "residualplot.pdf", htmlFile)
		dev.control("enable") 

		#Graphical parameters
		graphdata<-data.frame(cbind(predict(threewayfull),rstudent(threewayfull)))

		graphdata$yvarrr_IVS <- graphdata$X2
		graphdata$xvarrr_IVS <- graphdata$X1
		XAxisTitle <- "Predicted values"
		YAxisTitle <- "Externally Studentised residuals"
		MainTitle2 <- " "
		w_Gr_jitscat <- 0
		h_Gr_jitscat <-  0
		infiniteslope <- "Y"

		if (bandw != "N")  {
			Gr_line <-BW_line
			Gr_fill <- BW_fill
		} else {
			Gr_line <-Col_line
			Gr_fill <- Col_fill
		}
		Gr_line_type<-Line_type_dashed
		Line_size <- 0.5

		NONCAT_SCAT("RESIDPLOT")
	
		MainTitle2 <- ""
	
		void<-HTMLInsertGraph(GraphFileName=sub("[A-Z0-9a-z,:,\\\\]*App_Data[\\\\]","", residualPlot), Align="centre")

		#STB July2013
		if (pdfout=="Y") {
			pdf(file=sub("[A-Z0-9a-z,:,\\\\]*App_Data[\\\\]","", plotFilepdf1), height = pdfheight, width = pdfwidth) 
			dev.set(2) 
			dev.copy(which=3) 
			dev.off(2)
			dev.off(3)
			pdfFile_1<-sub("[A-Z0-9a-z,:,\\\\]*App_Data[\\\\]","",plotFilepdf1)
			linkToPdf1 <- paste ("<a href=\"",pdfFile_1,"\">Click here to view the PDF of the residuals vs. predicted plot</a>", sep = "")
			HTML(linkToPdf1)
		}
	
		HTML("Tip: On this plot look to see if the spread of the points increases as the predicted values increase. If so the response may need transforming or the unequal variance assumption selected.", align="left")
		HTML("Tip: Any observation with a residual less than -3 or greater than 3 (SD) should be investigated as a possible outlier.", align="left")
	}

#===================================================================================================================
	#Normality plots
	if(showNormPlot=="Y") {
		HTML.title("Normal probability plot", HR=3, align="left")
	
		#STB - July 2012 rename response variable
		threewayfull<-lm(eval(parse(text = paste("statdata$", xxxresponsexxx)))~ mainEffect, data=statdata, na.action = na.omit)
	
		normPlot <- sub(".html", "normplot.png", htmlFile)
		png(normPlot,width = jpegwidth, height = jpegheight, units="in", res=PlotResolution)
	
		#STB July2013
		plotFilepdf2 <- sub(".html", "normplot.pdf", htmlFile)
		dev.control("enable") 
	
		#Graphical parameters
		te<-qqnorm(resid(threewayfull))
		graphdata<-data.frame(te$x,te$y)
		graphdata$xvarrr_IVS <-graphdata$te.x
		graphdata$yvarrr_IVS <-graphdata$te.y
		graphdata$names <- c(1:length(graphdata$te.x))
		YAxisTitle <-"Sample Quantiles"
		XAxisTitle <-"Theoretical Quantiles"
		MainTitle2 <- " "
		w_Gr_jitscat <- 0
		h_Gr_jitscat <-  0
		infiniteslope <- "N"
		LinearFit <- "Y"
	
		if (bandw != "N")  {
			Gr_line <-BW_line
			Gr_fill <- BW_fill
		} else {
			Gr_line <-Col_line
			Gr_fill <- Col_fill
		}
		Gr_line_type<-Line_type_dashed
	
		Line_size <- 0.5
		Gr_alpha <- 1
		Line_type <-Line_type_dashed
	
		#NONCAT_SCAT("QQPLOT")
		NONCAT_QQPLOT()
		MainTitle2 <- ""
	
		void<-HTMLInsertGraph(GraphFileName=sub("[A-Z0-9a-z,:,\\\\]*App_Data[\\\\]","", normPlot), Align="left")
	
	#STB July2013
		if (pdfout=="Y") {
			pdf(file=sub("[A-Z0-9a-z,:,\\\\]*App_Data[\\\\]","", plotFilepdf2), height = pdfheight, width = pdfwidth) 
			dev.set(2) 
			dev.copy(which=3) 
			dev.off(2)
			dev.off(3)
			pdfFile_2<-sub("[A-Z0-9a-z,:,\\\\]*App_Data[\\\\]","",plotFilepdf2)
			linkToPdf2 <- paste ("<a href=\"",pdfFile_2,"\">Click here to view the PDF of the normal probability plot</a>", sep = "")
			HTML(linkToPdf2)
		}
	
		HTML("Tip: Check that the points lie along the dotted line. If not then the data may be non-normally distributed.", align="left")
	}
}	

#===================================================================================================================
#Unequal variance case
#===================================================================================================================
if (unequalCase == "Y") {
	HTML.title("Statistical analysis results assuming unequal variances", HR=2, align="left")
	HTML("The analysis presented is Welch's t-test assuming unequal variances, Welch (1947).", align="left")

	HTML.title("Unpaired t-test result", HR=3, align="left")

	#STB - July 2012 rename response variable
	# eqtest<-t.test(formula = eval(parse(text = paste("statdata$", response)))~eval(parse(text = paste("statdata$", treatFactor))), paired = FALSE, var.equal= FALSE, conf.level= sig)
	eqtest<-t.test(formula = eval(parse(text = paste("statdata$", xxxresponsexxx)))~ statdata$mainEffect, paired = FALSE, var.equal= FALSE, conf.level= sig)

	col1<-format(round(as.numeric(eqtest[1]), 3), nsmall=3, scientific=FALSE)
	col2<-format(round(as.numeric(eqtest[2]), 2), nsmall=2, scientific=FALSE)
	col3<-format(round(as.numeric(eqtest[3]), 4), nsmall=4, scientific=FALSE)
	col4<-as.numeric(eqtest[3])
	col5<- c("Unequal variance unpaired t-test")

	eqtable<-cbind(col5,col1,col2,col3)

	head<-c(" ","t-statistic", "Degrees of freedom","p-value")
	colnames(eqtable)<-head

	for (i in 1:(dim(eqtable)[1])) {
		if (as.numeric(col4[i])<0.0001) {
			#STB March 2011 formatting p-values p<0.0001
			#eqtable[i,4]<-0.0001
			eqtable[i,4]=format(round(0.0001, 4), nsmall=4, scientific=FALSE)
			eqtable[i,4]<- paste("<",eqtable[i,4])
		}
	}

	HTML(eqtable, classfirstline="second", align="left", row.names = "FALSE")

	add<-paste(c("Conclusion"))
	if (as.numeric(col4[i]) <= (1-sig)) {
		add<-paste(add, ": There is a statistically significant difference between the levels of ", xxxresponsexxx, " at the ", 100*(1-sig) , "% level" , ".", sep="")
	} else {
		add<-paste(add, ": The difference between the levels of ", xxxresponsexxx, " is not statistically significant.", sep="")
	}
	HTML(add, align="left")
}

#===================================================================================================================
#Diagnostic plots (unequal variance case)
#===================================================================================================================
if (unequalCase == "Y") {
	if((showPRPlot=="Y" && showNormPlot=="N") || (showPRPlot=="N" && showNormPlot=="Y") ) {
			HTML.title("Diagnostic plot (assuming unequal variances)", HR=2, align="left")
	}
	if(showPRPlot=="Y" && showNormPlot=="Y") {
			HTML.title("Diagnostic plots (assuming unequal variances)", HR=2, align="left")
	}

	#Residual plots
	if(showPRPlot=="Y") {
		HTML.title("Residuals vs. predicted plot", HR=3, align="left")
		HTML("This plot is not available for the unequal variance case.", align="left")
	}

#===================================================================================================================
	#Normality plots
	if(showNormPlot=="Y") {
		HTML.title("Normal probability plot", HR=3, align="left")
	

		normPlotx <- sub(".html", "normplotx.png", htmlFile)
		png(normPlotx,width = jpegwidth, height = jpegheight, units="in", res=PlotResolution)
	
		#STB July2013
		plotFilepdf2x <- sub(".html", "normplotx.pdf", htmlFile)
		dev.control("enable") 
	
		#Graphical parameters
		YAxisTitle <-"Sample Quantiles"
		XAxisTitle <-"Theoretical Quantiles"
		w_Gr_jitscat <- 0
		h_Gr_jitscat <-  0
		infiniteslope <- "N"
		LinearFit <- "Y"

		if (bandw != "N")  {
			Gr_line <-BW_line
			Gr_fill <- BW_fill
		} else {
			Gr_line <-Col_line
			Gr_fill <- Col_fill
		}
		Gr_line_type<-Line_type_dashed
	
		Line_size <- 0.5
		Gr_alpha <- 1
		Line_type <-Line_type_dashed
	
		sub <- subset(statdata, statdata$mainEffect == unique(levels(as.factor(statdata$mainEffect)))[1])
		te <- qqnorm(eval(parse(text = paste("sub$", xxxresponsexxx))))

		graphdata<-data.frame(te$x,te$y)
		graphdata$xvarrr_IVS <-graphdata$te.x
		graphdata$yvarrr_IVS <-graphdata$te.y

		temp1data <- graphdata
		temp1data$Effect <- unique(levels(as.factor(statdata$mainEffect)))[1]

		sub <- subset(statdata, statdata$mainEffect == unique(levels(as.factor(statdata$mainEffect)))[2])
		te <- qqnorm(eval(parse(text = paste("sub$", xxxresponsexxx))))
		graphdata<-data.frame(te$x,te$y)
		graphdata$xvarrr_IVS <-graphdata$te.x
		graphdata$yvarrr_IVS <-graphdata$te.y

		temp2data <- graphdata
		temp2data$Effect <- unique(levels(as.factor(statdata$mainEffect)))[2]
		temp3data <- statdata
		temp3data$names <- c(1:length(statdata$mainEffect))
		temp4data <- temp3data[order(eval(parse(text = paste("temp3data$", xxxresponsexxx)))),]
	
		temp5data <- rbind(temp1data, temp2data)
		temp7data <- temp5data[order(temp5data$yvarrr_IVS),]
		temp8data <- cbind(temp7data, temp4data)
		temp9data <- temp8data[order(temp8data$names),]

		graphdata <- temp9data
		graphdata$catfact <- graphdata$mainEffect
		Gr_palette<-palette_FUN("catfact")
		MainTitle2 <- " "

		CAT_QQPLOT()
		void<-HTMLInsertGraph(GraphFileName=sub("[A-Z0-9a-z,:,\\\\]*App_Data[\\\\]","", normPlotx), Align="left")
	
	#STB July2013
		if (pdfout=="Y") {
			pdf(file=sub("[A-Z0-9a-z,:,\\\\]*App_Data[\\\\]","", plotFilepdf2), height = pdfheight, width = pdfwidth) 
			dev.set(2) 
			dev.copy(which=3) 
			dev.off(2)
			dev.off(3)
			pdfFile_2<-sub("[A-Z0-9a-z,:,\\\\]*App_Data[\\\\]","",plotFilepdf2)
			linkToPdf2 <- paste ("<a href=\"",pdfFile_2,"\">Click here to view the PDF of the normal probability plot</a>", sep = "")
			HTML(linkToPdf2)
		}
		HTML("Note: The normal probability plot includes one line per group as the variances are estimated separately for each group.", align="left")
		HTML("Tip: Check that the points lie along the dotted line. If not then the data may be non-normally distributed.", align="left")
	}
}	

#===================================================================================================================
# Means and Planned comparisons on the main effects
#===================================================================================================================
if (unequalCase == "Y") {
	tablenames<-c(levels(as.factor(statdata$mainEffect)))
	length <- length(unique(levels(as.factor(statdata$mainEffect))))

	vectorUCI <-c(1:length)
	vectorLCI <-c(1:length)
	vectormean <-c(1:length)
	rownms<-c(1:length)

	for (i in 1:length) {
		sub<-subset(statdata, statdata$mainEffect == unique(levels(as.factor(statdata$mainEffect)))[i])
		sub2<-data.frame(sub)
		tempy<-na.omit(eval(parse(text = paste("sub2$", xxxresponsexxx))))
		vectormean[i] = mean(eval(parse(text = paste("sub2$", xxxresponsexxx))), na.rm=TRUE)
		vectorLCI[i]= mean(eval(parse(text = paste("sub2$", xxxresponsexxx))), na.rm=TRUE)-qt(1-(1-sig)/2, (length(tempy)-1))*sd(eval(parse(text = paste("sub2$", xxxresponsexxx))), na.rm=TRUE) / (length(tempy))**(0.5)
		vectorUCI[i]= mean(eval(parse(text = paste("sub2$", xxxresponsexxx))), na.rm=TRUE)+qt(1-(1-sig)/2, (length(tempy)-1))*sd(eval(parse(text = paste("sub2$", xxxresponsexxx))), na.rm=TRUE) / (length(tempy))**(0.5)
	}
	
	#Required for back transformation 
	vectormean2<- vectormean
	vectorLCI2<-vectorLCI
	vectorUCI2 <- vectorUCI

	#STB Dec 2011 formatting 3dp
	vectormean<-format(round(vectormean, 3), nsmall=3, scientific=FALSE)
	vectorLCI<-format(round(vectorLCI, 3), nsmall=3, scientific=FALSE)
	vectorUCI<-format(round(vectorUCI, 3), nsmall=3, scientific=FALSE)

	for(i in 1:length) {
		rownms[i]<-levels(as.factor(statdata$mainEffect))[i]
	}

	table2<-cbind(rownms,vectormean,vectorLCI,vectorUCI)

	CIlow<-paste("Lower ", 100*(sig), "% CI", sep="")
	CIhigh<-paste("Upper ", 100*(sig), "% CI", sep="")
	colnames(table2)<-c("Treatment level", "Mean", CIlow, CIhigh)

	#STB May 2012 Updating "least square (predicted) means"

	if ( (responseTransform != "log10" && responseTransform != "loge") || (responseTransform == "log10" && GeomDisplay != "geometricmeansonly") || (responseTransform == "loge" && GeomDisplay != "geometricmeansonly") ) {
		CITitle2<-paste("Table of the least square (predicted) means with ",(sig*100),"% confidence intervals",sep="")
		HTML.title(CITitle2, HR=3, align="left")
		HTML(table2, align="left" , classfirstline="second", row.names = "FALSE")
	}
#===================================================================================================================
#Calculating the size of the arithmetic difference with 95%CI
#===================================================================================================================

	#Required to generate table label only
	mult3<-glht(lm(eval(parse(text = paste("statdata$", xxxresponsexxx)))~ mainEffect, data=statdata, na.action = na.omit), linfct=lsm(pairwise ~mainEffect))
	multci3<-confint(mult3, level=sig, calpha = univariate_calpha())
	mult2<-t.test(formula = eval(parse(text = paste("statdata$", xxxresponsexxx)))~ statdata$mainEffect, paired = FALSE, var.equal= FALSE, conf.level= sig)

	pvals<-mult2$p.value
	meandiff<- mult2$estimate[1] - mult2$estimate[2]
	lowerdiff<- mult2$conf.int[1]
	upperdiff<- mult2$conf.int[2]
	tablen<-1
	tabs<-matrix(nrow=tablen, ncol=4)

	for (i in 1:tablen) {
		#STB Dec 2011 increasing means to 3dp
		tabs[i,1]=format(round(meandiff, 3), nsmall=3, scientific=FALSE)
		tabs[i,2]=format(round(lowerdiff, 3), nsmall=3, scientific=FALSE)
		tabs[i,3]=format(round(upperdiff, 3), nsmall=3, scientific=FALSE)
		tabs[i,4]=format(round(pvals, 4), nsmall=4, scientific=FALSE)
	}

	for (i in 1:tablen)  {
		if (pvals[i]<0.0001) {
			#STB March 2011 - formatting p-values p<0.0001
			#tabs[i,4]<-0.0001
			tabs[i,4]=format(round(0.0001, 4), nsmall=4, scientific=FALSE)
			tabs[i,4]<- paste("<",tabs[i,4])
		}
	}
	

	rows<-rownames(multci3$confint)

#STB2019
#	rows<-sub(" - "," vs. ", rows, fixed=TRUE)

	#STB June 2015	
	for (i in 1:100) {
		rows<-sub("_ivs_dash_ivs_"," - ", rows, fixed=TRUE)
	}

	lowerCI<-paste("   Lower ",(sig*100),"% CI   ",sep="")
	upperCI<-paste("   Upper ",(sig*100),"% CI   ",sep="")

	tabls<-cbind(rows, tabs)
	colnames(tabls)<-c("Comparison", "Difference", lowerCI, upperCI, "p-value")

	if ( (responseTransform != "log10" && responseTransform != "loge") || (responseTransform == "log10" && GeomDisplay != "geometricmeansonly") || (responseTransform == "loge" && GeomDisplay != "geometricmeansonly") ) {
		add <-paste ("Comparison of the  least square (predicted) means with ",(sig*100),"% confidence interval" , sep="") 
		HTML.title(add, HR=3, align="left")
		HTML(tabls, classfirstline="second", align="left", row.names = "FALSE")
	}
}
#===================================================================================================================
#Back transformed geometric means plot and table 
#===================================================================================================================
if(unequalCase == "Y" && (responseTransform =="log10"||responseTransform =="loge")) {
	if ( (responseTransform == "log10" && GeomDisplay != "notdisplayed") || (responseTransform == "loge" && GeomDisplay != "notdisplayed") ) {
		#Table of LS Means
		CITitle2<-paste("Table of the back-transformed geometric means with ",(sig*100),"% confidence intervals",sep="")
		HTML.title(CITitle2, HR=3, align="left")

		if (GeomDisplay == "geometricmeansandpredictedmeansonlogscale") {
			HTML("As the response was log transformed prior to analysis the least square (predicted) means are presented on the log scale. These results can be back transformed onto the original scale. These are known as the back-transformed geometric means.", align="left")
		}
		if (GeomDisplay == "geometricmeansonly") {
			HTML("As the response was log transformed prior to analysis the least square (predicted) means are presented back transformed onto the original scale. These are known as the back-transformed geometric means.", align="left")
		}

		#STB Dec 2011 formatting 3dp
		if (responseTransform =="log10") {
			vectormean2<-format(round(10^(vectormean2), 3), nsmall=3, scientific=FALSE)
			vectorLCI2<-format(round(10^(vectorLCI2), 3), nsmall=3, scientific=FALSE)
			vectorUCI2<-format(round(10^(vectorUCI2), 3), nsmall=3, scientific=FALSE)
		}
	
		if (responseTransform =="loge") {
			vectormean2<-format(round(exp(vectormean2), 3), nsmall=3, scientific=FALSE)
			vectorLCI2<-format(round(exp(vectorLCI2), 3), nsmall=3, scientific=FALSE)
			vectorUCI2<-format(round(exp(vectorUCI2), 3), nsmall=3, scientific=FALSE)
		}
	
		rownms<-c(1:length)
		for(i in 1:length) {
		rownms[i]<-levels(as.factor(statdata$mainEffect))[i]
		}
	
		table2<-cbind(rownms,vectormean2,vectorLCI2,vectorUCI2)
	
		CIlow<-paste("Lower ", 100*(sig), "% CI", sep="")
		CIhigh<-paste("Upper ", 100*(sig), "% CI", sep="")
		colnames(table2)<-c("Treatment level", "Geometric Mean", CIlow, CIhigh)
		
		HTML(table2, , align="left" , classfirstline="second", row.names = "FALSE")

#===================================================================================================================
#Calculating the size of the geometric ratio with 95%CI
#===================================================================================================================
		HTML.title("Comparison of the geometric means as a back-transformed ratio", HR=3, align="left")
		HTML("As the response was log transformed prior to analysis the differences between the least square (predicted) means are presented on the log scale. These results can be back-transformed onto the original scale, where differences on the log scale become ratios when back-transformed.", align="left")
	
		#Required to generate table label only
		mult3<-glht(lm(eval(parse(text = paste("statdata$", xxxresponsexxx)))~ mainEffect, data=statdata, na.action = na.omit), linfct=lsm(pairwise ~mainEffect))
		multci3<-confint(mult3, level=sig, calpha = univariate_calpha())
	
		mult2<-t.test(formula = eval(parse(text = paste("statdata$", xxxresponsexxx)))~ statdata$mainEffect, paired = FALSE, var.equal= FALSE, conf.level= sig)
	
		meandiff<- mult2$estimate[1] - mult2$estimate[2]
		lowerdiff<- mult2$conf.int[1]
		upperdiff<- mult2$conf.int[2]
		tablen<-1
		tabsx<-matrix(nrow=tablen, ncol=4)
	
		if (responseTransform =="log10") {
			for (i in 1:tablen) {
				tabsx[i,1]=format(round(10^(meandiff), 3), nsmall=3, scientific=FALSE)
				tabsx[i,2]=format(round(10^(lowerdiff), 3), nsmall=3, scientific=FALSE)
				tabsx[i,3]=format(round(10^(upperdiff), 3), nsmall=3, scientific=FALSE)
			}
		}
	
		if (responseTransform =="loge")	{
			for (i in 1:tablen) {
				tabsx[i,1]=format(round(exp(meandiff), 3), nsmall=3, scientific=FALSE)
				tabsx[i,2]=format(round(exp(lowerdiff), 3), nsmall=3, scientific=FALSE)
				tabsx[i,3]=format(round(exp(upperdiff), 3), nsmall=3, scientific=FALSE)
			}
		}
		tabsx[,4] <- tabs[,4]	
		rows<-rownames(multci3$confint)

		#STB2019
		rows<-sub(" - "," / ", rows, fixed=TRUE)

		#STB June 2015	
		for (i in 1:100) {
			rows<-sub("_ivs_dash_ivs_"," - ", rows, fixed=TRUE)
		}
	
		lowerCI<-paste("   Lower ",(sig*100),"% CI   ",sep="")
		upperCI<-paste("   Upper ",(sig*100),"% CI   ",sep="")
	
		tablsx<-cbind(rows, tabsx)
		colnames(tablsx)<-c("Comparison", "Ratio", lowerCI, upperCI, "p-value")
		HTML(tablsx, classfirstline="second", align="left")
	}
}
#===================================================================================================================
#References
#===================================================================================================================
Ref_list<-R_refs()

HTML.title("References", HR=2, align="left")
HTML(Ref_list$IVS_ref, align="left")
HTML(Ref_list$BateClark_ref, align="left")

if(unequalCase == "Y") {
	HTML("Welch, B.L. (1947). The generalization of Student's problem when several different population variances are involved. Biometrika, 34(1-2), 28-35.", align="left")
}

HTML.title("R references", HR=4, align="left")
HTML(Ref_list$R_ref ,  align="left")
HTML(reference("R2HTML"))
HTML(reference("GGally"))
HTML(reference("RColorBrewer"))
HTML(reference("ggplot2"))
HTML(reference("ggrepel"))
HTML(reference("reshape"))
HTML(reference("plyr"))
HTML(reference("scales"))
HTML(reference("proto"))

HTML(reference("multcomp"))
HTML(reference("emmeans"))
#===================================================================================================================
#Show dataset
#===================================================================================================================
if (showdataset=="Y") {
	statdata2<-subset(statdata, select = -c(mainEffect))

	observ <- data.frame(c(1:dim(statdata2)[1]))
	colnames(observ) <- c("Observation")
	statdata22 <- cbind(observ, statdata2)

	HTML.title("Analysis dataset", HR = 2, align = "left")
    	HTML(statdata22, classfirstline = "second", align = "left", row.names = "FALSE")

}

#===================================================================================================================
#Show arguments
#===================================================================================================================
if (OutputAnalysisOps == "Y") {
	HTML.title("Analysis options", HR=2, align="left")

	HTML(paste("Response variable: ", xxxresponsexxx, sep=""),  align="left")

	if (responseTransform != "none")
	{ 
		HTML(paste("Response variable transformation: ", responseTransform, sep=""), align="left")
	}

	HTML(paste("Treatment factor: ", treatFactor, sep=""),  align="left")
	HTML(paste("Display equal variance case (Y/N): ", equalCase, sep=""),  align="left")
	HTML(paste("Display unequal variance case (Y/N): ", unequalCase, sep=""),  align="left")
	HTML(paste("Show residuals vs. predicted plot (Y/N): ", showPRPlot, sep=""), align="left")
	HTML(paste("Display normal probability plot (Y/N): ", showNormPlot, sep=""),  align="left")
	HTML(paste("Control group: ", controlGroup, sep=""),  align="left")
	HTML(paste("Significance level: ", 1-sig, sep=""), align="left")
}
