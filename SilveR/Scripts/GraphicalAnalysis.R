#===================================================================================================================
#R Libraries

suppressWarnings(library(R2HTML))

#===================================================================================================================
# retrieve args
Args <- commandArgs(TRUE)

#Read in data
graphdata <- read.csv(Args[3], header=TRUE, sep=",")

#Copy Args
XAxisVar <- Args[4]
XAxisTransform <- Args[5]
YAxisVars <- Args[6]
YAxisTransform <- Args[7]
FirstCatFactor <- Args[8]
SecondCatFactor <- Args[9]
GraphStyle <- Args[10]
MainTitle <- Args[11]
XAxisTitle <- Args[12]
XAxisTitleHist <- Args[12]
YAxisTitle <- Args[13]
ScatterPlot <- Args[14]
LinearFit <- Args[15]
jitterfunction <- Args[16]
BoxPlot <- Args[17]
Outliers <- Args[18]
SEMPlot <- Args[19]
SEMPlotType <- Args[20]
HistogramPlot <- Args[21]
NormalDistFit <- Args[22]
CaseProfilesPlot <- Args[23]
CaseIDFactor <- Args[24]
ReferenceLine <- Args[25]
DisplayLegend <-Args[26]
displaypointBOX<-Args[27]
displaypointSEM<-Args[28]

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

#V3.2 STB OCT2015
set.seed(5041975)

#Graphical parameters

#Reference line
if (ReferenceLine != "NULL") {
	Gr_intercept <- as.numeric(ReferenceLine)
	Gr_line_typeint<-Line_type_dashed
}

#add in a catfactor 
if(!"catfact" %in% colnames(graphdata)) {
	graphdata$catfact <- c(NA)
}

if (FirstCatFactor != "NULL" && SecondCatFactor == "NULL") {
	graphdata$catfact <- eval(parse(text = paste("graphdata$",FirstCatFactor )))
}
if (FirstCatFactor == "NULL" && SecondCatFactor != "NULL") {
	graphdata$catfact <- eval(parse(text = paste("graphdata$",SecondCatFactor )))
}
if (FirstCatFactor != "NULL" && SecondCatFactor != "NULL") {
	graphdata$catfact <- paste(eval(parse(text = paste("graphdata$",FirstCatFactor ))), eval(parse(text = paste("graphdata$",SecondCatFactor ))))
}

if(FirstCatFactor != "NULL" || SecondCatFactor != "NULL") {
	Gr_palette<-palette_FUN("catfact")
}
Labelz_IVS_ <- "N"

#Set up main title
if (MainTitle == "NULL") {
	MainTitle2 = ""
} else {
	MainTitle2 <-paste(MainTitle, " \n")
	}
if (XAxisTitleHist == "NULL") {
	XAxisTitleHist = YAxisVars
} 
if (XAxisTitle == "NULL") {
	XAxisTitle = XAxisVar
}
if (YAxisTitle == "NULL") {
	YAxisTitle = YAxisVars
}

#Legend code for overlaid plots
if (DisplayLegend == "N") {
	Gr_legend_pos<-Legend_pos_abs
}

#Confirm the catfactor is categorical
graphdata$catfact <-as.factor(graphdata$catfact)

#Improve titles new variables
graphdata$first_IVS_cat<- FirstCatFactor
graphdata$second_IVS_cat<- SecondCatFactor

#Setting up the variables for GGPLOT2
graphdata$yvarrr_IVS <-eval(parse(text = paste("graphdata$", YAxisVars)))

#Adding in a blank X-axis variable for R code to work if only Histogram selected (Histogram doesn't need a X-var).
if (HistogramPlot == "Y" && ScatterPlot == "N" && BoxPlot == "N" && CaseProfilesPlot == "N" && SEMPlot == "N") {
	graphdata$xvarrr_IVS <- graphdata$yvarrr_IVS
	XAxisVar <-YAxisVars	
} else {
	graphdata$xvarrr_IVS <-eval(parse(text = paste("graphdata$", XAxisVar)))
	}
if(FirstCatFactor != "NULL" && SecondCatFactor == "NULL") {
	graphdata$l_l <-as.factor(eval(parse(text = paste("graphdata$",FirstCatFactor ))))
	graphdata$l_l <- paste(graphdata$first_IVS_cat, "=",graphdata$l_l, sep = "") 
}
if(FirstCatFactor == "NULL" && SecondCatFactor != "NULL") {
	graphdata$l_l <-as.factor(eval(parse(text = paste("graphdata$",SecondCatFactor ))))
	graphdata$l_l <- paste(graphdata$second_IVS_cat, "=",graphdata$l_l, sep = "") 
}
if(FirstCatFactor != "NULL" && SecondCatFactor != "NULL") {
	graphdata$firstcatvarrr_IVS <-as.factor(eval(parse(text = paste("graphdata$",FirstCatFactor ))))
	graphdata$secondcatvarrr_IVS <-as.factor(eval(parse(text = paste("graphdata$",SecondCatFactor ))))
	graphdata$firstcatvarrr_IVS <- paste(graphdata$first_IVS_cat, "=",graphdata$firstcatvarrr_IVS, sep = "") 
	graphdata$secondcatvarrr_IVS <- paste(graphdata$second_IVS_cat, "=",graphdata$secondcatvarrr_IVS, sep = "") 
	graphdata$l_l <-as.factor(graphdata$catfact)
}

#Adding a case ID variable if none is selected
if (CaseProfilesPlot == "N") {
	CaseIDFactor <-YAxisVars			
}

#Testing for with infinite slopes on scatterplot and re-ordering dataset if necessary
inf_slope<-IVS_F_infinite_slope()
infiniteslope <- inf_slope$infiniteslope
graphdata<-inf_slope$graphdata
graphdatax <- subset(graphdata, catvartest != "N")
graphdata<-graphdatax

#Removing illegal characters
# Bug fix Aug 2010 (STB)
CPXAxisTitle <-XAxisTitle

#replace illegal characters in variable names
for (i in 1:10) {
	YAxisTitle<-namereplace(YAxisTitle)
	XAxisTitle<-namereplace(XAxisTitle)
	CPXAxisTitle<-namereplace(CPXAxisTitle)

	if (FirstCatFactor != "NULL" && SecondCatFactor != "NULL") 	{
		graphdata$firstcatvarrr_IVS<-namereplace(graphdata$firstcatvarrr_IVS)
		graphdata$secondcatvarrr_IVS<-namereplace(graphdata$secondcatvarrr_IVS)
	}

	if ((FirstCatFactor != "NULL" && SecondCatFactor == "NULL")  || (FirstCatFactor == "NULL" && SecondCatFactor != "NULL")) {
		graphdata$l_l<-namereplace(graphdata$l_l)
	}
}
XAxisTitleHist = YAxisTitle


#===================================================================================================================
#Titles and description
#===================================================================================================================
#Output HTML header
Title <-paste(branding, " Graphics", sep="")
HTML.title(Title, HR = 1, align = "left")

#Response
title<-c("Selected variables")
HTML.title(title, HR=2, align="left")

add<-paste("The  variable ", YAxisVars , " has been selected as the response", sep="")

if(XAxisVar != "NULL") {
add<-paste(add, " and the variable ", XAxisVar, " has been selected to define the X-axis", sep="")
}
add<-paste(add, ". ", sep="")

if (is.numeric(graphdata$yvarrr_IVS)=="TRUE" && YAxisTransform != "None") {
add<-paste(add, "The variable ", YAxisVars, " has been ", YAxisTransform, " transformed.", sep="")
}

if (XAxisVar != "NULL" && is.numeric(graphdata$xvarrr_IVS)=="TRUE" && XAxisTransform != "None") {
add<-paste(add, " The variable ", XAxisVar, " has been ", XAxisTransform, " transformed.", sep="")
}

if (FirstCatFactor != "NULL" && SecondCatFactor == "NULL") {
add<-paste(add, " The variable ", FirstCatFactor, " has been selected as the categorisation factor.", sep="")
}

if (FirstCatFactor == "NULL" && SecondCatFactor != "NULL") {
add<-paste(add, " The variable ", SecondCatFactor, " has been selected as the categorisation factor.", sep="")
}

if (FirstCatFactor != "NULL" && SecondCatFactor != "NULL") {
add<-paste(add, " The variables ", FirstCatFactor, " and ", SecondCatFactor, " have been selected as the categorisation factors.", sep="")
}

if (CaseProfilesPlot != "N") {
add<-paste(add, " The variable ", CaseIDFactor, " defines the case profiles.",  sep="")
}

HTML(add, align="left")

#===================================================================================================================
#===================================================================================================================
#Scatterplot
#===================================================================================================================
#===================================================================================================================
#Global scatterplot options

#Titles
if(ScatterPlot == "Y" && FirstCatFactor == "NULL" && SecondCatFactor == "NULL"&& YAxisVars != "NULL") {
	HTML.title("Scatterplot", HR=2, align="left")
} else if(ScatterPlot == "Y" && (FirstCatFactor != "NULL" || SecondCatFactor != "NULL")&& YAxisVars != "NULL") {
	HTML.title("Categorised scatterplot", HR=2, align="left")
}

#GGPLOT2 variable options
if (LinearFit == "N") {
	Gr_alpha <- 0
	Line_type <-Line_type_blank
} else {
	Gr_alpha <- 1
	Line_type <-Line_type_solid
	}

#Calculating correlation coefficients
if(ScatterPlot == "Y" && is.numeric(graphdata$xvarrr_IVS)=="TRUE" && is.numeric(graphdata$yvarrr_IVS)=="TRUE") {
	if (FirstCatFactor == "NULL" && SecondCatFactor == "NULL") {
	catfacttemp<-rep(YAxisVars, dim(graphdata)[1])
	graphdata$catfact=catfacttemp
	}
	rows<-dim(graphdata)[1]
	cols<-dim(graphdata)[2]
	nlevels<-length(unique(as.factor(graphdata$catfact)))
	extra<-matrix(data=NA, nrow=rows, ncol=nlevels)
	for (i in 1:nlevels)	{
		for (j in 1:rows) {
			if (as.factor(graphdata$catfact)[j] == unique(as.factor(graphdata$catfact))[i]) {
				extra[j,i]<-graphdata$yvarrr_IVS[j]
			}
		}
	}
	newdata<-cbind(graphdata, extra)
	catplotdata<-data.frame(newdata)
	tempdata<-catplotdata
	catleg<-matrix(nrow=length(unique(levels(as.factor(tempdata$catfact)))),ncol=1)
	ptab<-c(0)
	rhotab<-c(0)
	inttab<-c(0)
	slopetab<-c(0)
	for (k in 1:nlevels) {
		tmpdata<-catplotdata
		tmpdata2<-subset(tmpdata, tmpdata$catfact == unique(levels(as.factor(tmpdata$catfact)))[k])

			#STB Aug 2011 - removing lines with infinite slope
			if(length(unique(tmpdata2$xvarrr_IVS)) >2 && length(unique(tmpdata2$yvarrr_IVS)) >2) {
			correlation<-cor.test(tmpdata2$yvarrr_IVS,tmpdata2$xvarrr_IVS, method="pearson")
			pcorr<-correlation$p.value
			rho<-correlation$estimate

			#Calculating regression coefficients
			threewayfull<-lm(yvarrr_IVS~xvarrr_IVS, data=tmpdata2, na.action = na.omit)			
			inttabx<-   coefficients(threewayfull)[1]
			slopetabx<- coefficients(threewayfull)[2]
			} else {
				pcorr<-1000
				rho<-1000
				inttabx<-1000
				slopetabx<-1000
			}

		#STB - March 2011 Formatting p-values p<0.001
		pcorr2<-format(round(pcorr, 4), nsmall=4, scientific=FALSE)
		rho<-format(round(rho, 3), nsmall=3, scientific=FALSE)
		inttabx<-format(round(inttabx, 4), nsmall=4, scientific=FALSE)
		slopetabx<-format(round(slopetabx, 4), nsmall=4, scientific=FALSE)
	
		if (pcorr<0.0001) {
			pcorr2=format(round(0.0001, 4), nsmall=4, scientific=FALSE)
			pcorr2<- paste("<",pcorr2)
		}

		#STB Aug 2011 - removing lines with infinite slope
		if (pcorr==1000) {
			pcorr2<- "-"
			rho<- "-"
			inttabx<-"-"
			slopetabx<-"-"
		}
		ptab[k]<-pcorr2
		rhotab[k]<-rho
		inttab[k]<-inttabx
		slopetab[k]<-slopetabx
	}

	#Preparing the correlation coefficients table
	good3<-c(" ")
	if ((FirstCatFactor == "NULL" && SecondCatFactor == "NULL")) {
		good3<-c(YAxisVars)
		temp6<-c("Response", "Correlation coefficient", "p-value")
	} 
	if ((FirstCatFactor != "NULL" && SecondCatFactor == "NULL") || (FirstCatFactor == "NULL" && SecondCatFactor != "NULL")) {
		temp6<-c("Categorisation factor levels", "Correlation coefficient", "p-value")
	} 
	if (FirstCatFactor != "NULL" && SecondCatFactor != "NULL") {
		temp6<-c("Categorisation factors level combinations", "Correlation coefficient", "p-value")
	} 
	if (FirstCatFactor != "NULL" || SecondCatFactor != "NULL") {
		for(i in 1:(length(rhotab))) {
		good3[i]<-levels(as.factor(tmpdata$catfact))[i]
		}
	} 
	corrtab<-cbind(good3, rhotab, ptab)
	colnames(corrtab)<-temp6

	#Preparing the regression line coefficients table
	good3<-c(" ")
	if ((FirstCatFactor == "NULL" && SecondCatFactor == "NULL")) {
		good3<-c(YAxisVars)
		temp6<-c("Response", "Intercept estimate", "Slope estimate")
	} 
	if ((FirstCatFactor != "NULL" && SecondCatFactor == "NULL") || (FirstCatFactor == "NULL" && SecondCatFactor != "NULL")) {
		temp6<-c("Categorisation factor levels", "Intercept estimate", "Slope estimate")
	}	
	if (FirstCatFactor != "NULL" && SecondCatFactor != "NULL") {
		temp6<-c("Categorisation factors level combinations", "Intercept estimate", "Slope estimate")
	} 
	if (FirstCatFactor != "NULL" || SecondCatFactor != "NULL") {
		for(i in 1:(length(rhotab))) {
		good3[i]<-levels(as.factor(tmpdata$catfact))[i]
		}
	} 
	esttab<-cbind(good3, inttab, slopetab)
	colnames(esttab)<-temp6
}

#Jitter warning	
if (ScatterPlot == "Y" && jitterfunction == "Y" && is.numeric(graphdata$xvarrr_IVS)=="TRUE" && is.numeric(graphdata$yvarrr_IVS)=="TRUE")  {
	HTML("Warning: The jitter function is only applicable to categorical variables. In this case the variables are both numerical and hence no jitter has been applied.", align="left")
} 

if (jitterfunction == "N" ||( is.numeric(graphdata$xvarrr_IVS)=="TRUE" && is.numeric(graphdata$yvarrr_IVS)=="TRUE"))  {
	w_Gr_jitscat <- 0
	h_Gr_jitscat <- 0
} else {
	if (is.numeric(graphdata$xvarrr_IVS)=="FALSE" && is.numeric(graphdata$yvarrr_IVS)=="TRUE") {	
		w_Gr_jitscat <- Gr_w_Gr_jit
		h_Gr_jitscat <- 0
	}
	if (is.numeric(graphdata$xvarrr_IVS)=="TRUE" && is.numeric(graphdata$yvarrr_IVS)=="FALSE") {	
		w_Gr_jitscat <- 0
		h_Gr_jitscat <- Gr_h_Gr_jit
	}
	if (is.numeric(graphdata$xvarrr_IVS)=="FALSE" && is.numeric(graphdata$yvarrr_IVS)=="FALSE") {	
			w_Gr_jitscat <- 0
			h_Gr_jitscat <- 0
	}
}

#===================================================================================================================
# NON Categorised Scatterplot (graphics + summary plots)
#===================================================================================================================
if(ScatterPlot == "Y" && FirstCatFactor == "NULL" && SecondCatFactor == "NULL" ) {
	#Plot device setup
	ncscatterplot <- sub(".html", "ncscatterplot.jpg", htmlFile)
	jpeg(ncscatterplot,width = jpegwidth, height = jpegheight, quality = 100)

	#STB July2013
	plotFilepdf14 <- sub(".html", "ncscatterplot.pdf", htmlFile)
	dev.control("enable") 

	#GGPLOT2 code
	NONCAT_SCAT("NORMAL")

	#Output code
	void<-HTMLInsertGraph(GraphFileName=sub("[A-Z0-9a-z,:,\\\\]*App_Data[\\\\]","", ncscatterplot), Align="centre")

	#STB July2013
	if (pdfout=="Y") {
		pdf(file=sub("[A-Z0-9a-z,:,\\\\]*App_Data[\\\\]","", plotFilepdf14), height = pdfheight, width = pdfwidth) 
		dev.set(2) 
		dev.copy(which=3) 
		dev.off(2)
		dev.off(3)
		pdfFile_14<-sub("[A-Z0-9a-z,:,\\\\]*App_Data[\\\\]","",plotFilepdf14)
		linkToPdf14 <- paste ("<a href=\"",pdfFile_14,"\">Click here to view the PDF of the scatterplot</a>", sep = "")
		HTML(linkToPdf14)
	}
}

#===================================================================================================================
# One Categorised Seperate Scatterplot 
#===================================================================================================================
if(ScatterPlot == "Y" && ((FirstCatFactor != "NULL" && SecondCatFactor == "NULL") || (FirstCatFactor == "NULL" && SecondCatFactor != "NULL"))  && GraphStyle == "Separate") {

	#Plot device setup
	ncscatterplot2 <- sub(".html", "ncscatterplot2.jpg", htmlFile)
	jpeg(ncscatterplot2,width = jpegwidth, height = jpegheight, quality = 100)

	#STB July2013
	plotFilepdf17 <- sub(".html", "ncscatterplot2.pdf", htmlFile)
	dev.control("enable") 

	#GGPLOT2 code
	ONECATSEP_SCAT()

	#Output code
	void<-HTMLInsertGraph(GraphFileName=sub("[A-Z0-9a-z,:,\\\\]*App_Data[\\\\]","", ncscatterplot2), Align="centre")

	#STB July2013
	if (pdfout=="Y") {
		pdf(file=sub("[A-Z0-9a-z,:,\\\\]*App_Data[\\\\]","", plotFilepdf17), height = pdfheight, width = pdfwidth) 
		dev.set(2) 
		dev.copy(which=3) 
		dev.off(2)
		dev.off(3)
		pdfFile_17<-sub("[A-Z0-9a-z,:,\\\\]*App_Data[\\\\]","",plotFilepdf17)
		linkToPdf17 <- paste ("<a href=\"",pdfFile_17,"\">Click here to view the PDF of the scatterplot</a>", sep = "")
		HTML(linkToPdf17)
	}
}

#===================================================================================================================
# Two Categorised Seperate Scatterplot 
#===================================================================================================================
scalexy<-"fixed"

# TWO CAT - seperate continuous
if(ScatterPlot == "Y" && FirstCatFactor != "NULL" && SecondCatFactor != "NULL"  && GraphStyle == "Separate") {

	#Plot device setup
	scatterPlot <- sub(".html", "scatterPlot.jpg", htmlFile)
	jpeg(scatterPlot,width = jpegwidth, height = jpegheight, quality = 100)

	#STB July2013
	plotFilepdf20 <- sub(".html", "scatterPlot.pdf", htmlFile)
	dev.control("enable") 

	#GGPLOT2 code
	TWOCATSEP_SCAT()

	#Output code
	void <- HTMLInsertGraph(GraphFileName=sub("[A-Z0-9a-z,:,\\\\]*App_Data[\\\\]","", scatterPlot), Align="centre")

	#STB July2013
	if (pdfout=="Y") {
		pdf(file=sub("[A-Z0-9a-z,:,\\\\]*App_Data[\\\\]","", plotFilepdf20), height = pdfheight, width = pdfwidth) 
		dev.set(2) 
		dev.copy(which=3) 
		dev.off(2)
		dev.off(3)
		pdfFile_20<-sub("[A-Z0-9a-z,:,\\\\]*App_Data[\\\\]","",plotFilepdf20)
		linkToPdf20 <- paste ("<a href=\"",pdfFile_20,"\">Click here to view the PDF of the scatterplot</a>", sep = "")	
		HTML(linkToPdf20)
	}

}

#===================================================================================================================
# Categorised overlaid Scatterplot 
#===================================================================================================================
if(ScatterPlot == "Y" && ((FirstCatFactor != "NULL" && SecondCatFactor == "NULL") || (FirstCatFactor == "NULL" && SecondCatFactor != "NULL") || (FirstCatFactor != "NULL" && SecondCatFactor != "NULL"))    && GraphStyle == "Overlaid") {

	#Plot device setup
	ncscatterplot3 <- sub(".html", "ncscatterplot3.jpg", htmlFile)
    	jpeg(ncscatterplot3,width = jpegwidth, height = jpegheight, quality = 100)

	#STB July2013
	plotFilepdf16 <- sub(".html", "ncscatterplot3.pdf", htmlFile)
	dev.control("enable") 

	#GGPLOT2 code
	OVERLAID_SCAT()

	#Output code
	void<-HTMLInsertGraph(GraphFileName=sub("[A-Z0-9a-z,:,\\\\]*App_Data[\\\\]","", ncscatterplot3), Align="centre")

	#STB July2013
	if (pdfout=="Y") {
		pdf(file=sub("[A-Z0-9a-z,:,\\\\]*App_Data[\\\\]","", plotFilepdf16), height = pdfheight, width = pdfwidth) 
		dev.set(2) 
		dev.copy(which=3) 
		dev.off(2)
		dev.off(3)
		pdfFile_16<-sub("[A-Z0-9a-z,:,\\\\]*App_Data[\\\\]","",plotFilepdf16)
		linkToPdf16 <- paste ("<a href=\"",pdfFile_16,"\">Click here to view the PDF of the scatterplot</a>", sep = "")
		HTML(linkToPdf16)
	}

}

#===================================================================================================================
#Final scatterplot options
#===================================================================================================================
#Linear fit on categorical x-axis warning
if (ScatterPlot == "Y" && is.numeric(graphdata$xvarrr_IVS)=="FALSE" && LinearFit == "Y" ) {
	if(FirstCatFactor != "NULL" || SecondCatFactor != "NULL") {
		title<- "Warning: As the x-axis variable is categorical, it is not possible to fit the best-fit linear lines."
	} else {
		title<- "Warning: As the x-axis variable is categorical, it is not possible to fit a best-fit linear line."
	}
	HTML(title, align="left")
}

#infinite slope warning
if (ScatterPlot == "Y" && infiniteslope == "Y" && is.numeric(graphdata$xvarrr_IVS)=="TRUE" && LinearFit == "Y" ) {
	title<-paste("Warning: Some of the best-fit lines have not been fitted as all the ", XAxisVar, " values are the same for all subjects in one or more of the level(s) of the categorisation factor(s).", sep="")
	HTML(title, align="left")
}

#Corrleation and p
if(ScatterPlot == "Y" && is.numeric(graphdata$xvarrr_IVS)=="TRUE" && is.numeric(graphdata$yvarrr_IVS)=="TRUE") {
	HTML.title("Pearson's correlation coefficients and p-values", HR=2, align="left")
    	HTML(corrtab, classfirstline = "second", align = "left", row.names = "FALSE")
}

#Slope estimation
if(ScatterPlot == "Y" && is.numeric(graphdata$xvarrr_IVS)=="TRUE" && is.numeric(graphdata$yvarrr_IVS)=="TRUE" && LinearFit == "Y") {
	HTML.title("Estimates of the coefficients of the linear line", HR=2, align="left")
    	HTML(esttab, classfirstline = "second", align = "left", row.names = "FALSE")
}

#Linear fit warning
if(LinearFit == "Y" && ScatterPlot == "Y" && (is.numeric(graphdata$xvarrr_IVS)=="FALSE" || is.numeric(graphdata$yvarrr_IVS)=="FALSE")) {
	HTML("Warning: As one of the variables is non-numeric, the best fit line is not included on the scatterplot.", align="left")
}

#correlation and p warning
if(ScatterPlot == "Y" && (is.numeric(graphdata$xvarrr_IVS)=="FALSE" || is.numeric(graphdata$yvarrr_IVS)=="FALSE")) {
	HTML("Warning: As one of the variables is non-numeric, no correlation analysis has been performed.", align="left")
}

#===================================================================================================================
#===================================================================================================================
#Means with SEM plots#
#===================================================================================================================
#===================================================================================================================
#Title
if(SEMPlot == "Y" && FirstCatFactor == "NULL" && SecondCatFactor == "NULL" && YAxisVars != "NULL") {
	HTML.title("Observed means with standard errors plot", HR=2, align="left")
} else if(SEMPlot == "Y" && (FirstCatFactor != "NULL" || SecondCatFactor != "NULL") && YAxisVars != "NULL") {
	HTML.title("Categorised observed means with standard errors plot", HR=2, align="left")
}

#Warning message
#STB NOV2015 - Updating warning message
if(SEMPlot == "Y" && is.numeric(graphdata$yvarrr_IVS)==FALSE ) {
	HTML("As the Y-axis is non-numeric, no graph has been produced. Please review your selection." , align="left")
}

#Make x axis categorical
graphdata$xvarrr_IVS_SEM <-as.factor(graphdata$xvarrr_IVS)

#Jitter selection
w_Gr_jitSEM <- Gr_w_Gr_jit
h_Gr_jitSEM <- 0

#===================================================================================================================
# Non-Categorised means with SEM plot
#===================================================================================================================
if(SEMPlot == "Y" && FirstCatFactor == "NULL" && SecondCatFactor == "NULL" && is.numeric(graphdata$yvarrr_IVS)==TRUE ) {

	#Creating the summary dataset
	graphdata_SEM<-  ddply(graphdata, ~xvarrr_IVS_SEM, summarise, n=length(yvarrr_IVS), mean.y=mean(yvarrr_IVS), se.y=sd(yvarrr_IVS)/sqrt(n))

	#Plot device setup
	ncSEMPlot <- sub(".html", "ncSEMPlot.jpg", htmlFile)
	jpeg(ncSEMPlot,width = jpegwidth, height = jpegheight, quality = 100)

	#STB July2013
	plotFilepdf5 <- sub(".html", "ncSEMPlot.pdf", htmlFile)
	dev.control("enable") 

	#GGPLOT2 code
	NONCAT_SEM()

	#Output code
	void<-HTMLInsertGraph(GraphFileName=sub("[A-Z0-9a-z,:,\\\\]*App_Data[\\\\]","", ncSEMPlot), Align="centre")

	#STB July2013
	if (pdfout=="Y") {
		pdf(file=sub("[A-Z0-9a-z,:,\\\\]*App_Data[\\\\]","", plotFilepdf5), height = pdfheight, width = pdfwidth) 
		dev.set(2) 
		dev.copy(which=3) 
		dev.off(2)
		dev.off(3)
		pdfFile_5<-sub("[A-Z0-9a-z,:,\\\\]*App_Data[\\\\]","",plotFilepdf5)
		linkToPdf5 <- paste ("<a href=\"",pdfFile_5,"\">Click here to view the PDF of the means with SEMs plot</a>", sep = "")
		HTML(linkToPdf5)
	}
}

#===================================================================================================================
# ONE CAT - seperate means with SEM plot
#===================================================================================================================
if(SEMPlot == "Y" && ((FirstCatFactor != "NULL" && SecondCatFactor == "NULL") || (FirstCatFactor == "NULL" && SecondCatFactor != "NULL") ) && GraphStyle == "Separate" && is.numeric(graphdata$yvarrr_IVS)==TRUE) {

	#Creating the summary dataset
	graphdata_SEM<-  ddply(graphdata, ~xvarrr_IVS_SEM+l_l, summarise, n=length(yvarrr_IVS), mean.y=mean(yvarrr_IVS), se.y=sd(yvarrr_IVS)/sqrt(n))

	#Plot device setup
	ncSEMPlot <- sub(".html", "ncSEMPlot.jpg", htmlFile)
	jpeg(ncSEMPlot,width = jpegwidth, height = jpegheight, quality = 100)

	#STB July2013
	plotFilepdf6 <- sub(".html", "ncSEMPlot.pdf", htmlFile)
	dev.control("enable") 

	#GGPLOT2 code
	ONECATSEP_SEM()

	#Output code
	void<-HTMLInsertGraph(GraphFileName=sub("[A-Z0-9a-z,:,\\\\]*App_Data[\\\\]","", ncSEMPlot), Align="centre")

	#STB July2013
	if (pdfout=="Y") {
		pdf(file=sub("[A-Z0-9a-z,:,\\\\]*App_Data[\\\\]","", plotFilepdf6), height = pdfheight, width = pdfwidth) 
		dev.set(2) 
		dev.copy(which=3) 
		dev.off(2)
		dev.off(3)
		pdfFile_6<-sub("[A-Z0-9a-z,:,\\\\]*App_Data[\\\\]","",plotFilepdf6)
		linkToPdf6 <- paste ("<a href=\"",pdfFile_6,"\">Click here to view the PDF of the means with SEMs plot</a>", sep = "")
		HTML(linkToPdf6)
	}
}

#===================================================================================================================
# TWO CAT - seperate means with SEM plot
#===================================================================================================================
if(SEMPlot == "Y" && FirstCatFactor != "NULL" && SecondCatFactor != "NULL" && GraphStyle == "Separate" && is.numeric(graphdata$yvarrr_IVS)==TRUE) {

	#Creating the summary dataset
	graphdata_SEM<-  ddply(graphdata, ~xvarrr_IVS_SEM+firstcatvarrr_IVS+secondcatvarrr_IVS, summarise, n=length(yvarrr_IVS), mean.y=mean(yvarrr_IVS), se.y=sd(yvarrr_IVS)/sqrt(n))

	#Plot device setup
	ncSEMPlot <- sub(".html", "ncSEMPlot.jpg", htmlFile)
	jpeg(ncSEMPlot,width = jpegwidth, height = jpegheight, quality = 100)

	#STB July2013
	plotFilepdf9 <- sub(".html", "ncSEMPlot.pdf", htmlFile)
	dev.control("enable") 

	#GGPLOT2 code
	TWOCATSEP_SEM()

	#Output code
	void<-HTMLInsertGraph(GraphFileName=sub("[A-Z0-9a-z,:,\\\\]*App_Data[\\\\]","", ncSEMPlot), Align="centre")

	#STB July2013
	if (pdfout=="Y") {
		pdf(file=sub("[A-Z0-9a-z,:,\\\\]*App_Data[\\\\]","", plotFilepdf9), height = pdfheight, width = pdfwidth) 
		dev.set(2) 
		dev.copy(which=3) 
		dev.off(2)
		dev.off(3)
		pdfFile_9<-sub("[A-Z0-9a-z,:,\\\\]*App_Data[\\\\]","",plotFilepdf9)
		linkToPdf9 <- paste ("<a href=\"",pdfFile_9,"\">Click here to view the PDF of the means with SEMs plot</a>", sep = "")
		HTML(linkToPdf9)
	}
}

#===================================================================================================================
# Overlaid - means with SEM plot
#===================================================================================================================
if(SEMPlot == "Y" && ((FirstCatFactor != "NULL" && SecondCatFactor == "NULL") || (FirstCatFactor == "NULL" && SecondCatFactor != "NULL")|| (FirstCatFactor != "NULL" && SecondCatFactor != "NULL"))   && GraphStyle == "Overlaid" && is.numeric(graphdata$yvarrr_IVS)==TRUE) {

	#Adding blanks where needed
#	graphdataSEMx<-subset(graphdata, graphdata$catfact != "")
#	graphdataSEM<-subset(graphdataSEMx, graphdataSEMx$xvarrr_IVS != "")
#	graphdataSEM<-filter(graphdata,  !is.na(graphdata$catfact))
	graphdataSEM<-graphdata
	graphdataSEM$Type <- "Dataset"

	catlevels <-length(unique(levels(as.factor(graphdataSEM$l_l))))
	xlevels   <-length(unique(levels(as.factor(graphdataSEM$xvarrr_IVS_SEM))))
	blankdata = data.frame(matrix(vector(), (xlevels*catlevels), dim(graphdataSEM)[2], dimnames=list(c(), c(colnames(graphdataSEM)))), stringsAsFactors=F)
	i<-1

	#Chop up dataset to seperate group combinations, test to see if it has size 0 and if so add a new 0 entry to blankdata
	for (k in 1:xlevels) {
		tempdata<-subset(graphdataSEM, graphdataSEM$xvarrr_IVS_SEM == unique(levels(as.factor(graphdataSEM$xvarrr_IVS_SEM)))[k])
		for (j in 1:catlevels) {
			tempdata2<-subset(tempdata, tempdata$l_l == unique(levels(as.factor(graphdataSEM$l_l)))[j])
			if(length(unique(tempdata2$xvarrr_IVS))==0) {
				blankdata$yvarrr_IVS[i]   = NA
				blankdata$xvarrr_IVS_SEM[i]   =unique(levels(as.factor(graphdataSEM$xvarrr_IVS_SEM)))[k]
				blankdata$l_l[i] =unique(levels(as.factor(graphdataSEM$l_l)))[j]
				i=i+1
			}	
		}
	}
	if (i > 1) {
		blankdata$Type <- "Blanks"
		blankdata<-blankdata[c(1:(i-1)),]
		graphdataSEM_overall <- rbind(blankdata,graphdataSEM)
	} else {
		graphdataSEM_overall <- graphdataSEM
	}

	#Creating the summary dataset
	graphdataSEM_means<-  ddply(graphdataSEM_overall, ~xvarrr_IVS_SEM+l_l, summarise, n=length(yvarrr_IVS), mean.y=mean(yvarrr_IVS), se.y=sd(yvarrr_IVS)/sqrt(n))

	#Plot device setup
	ncSEMPlotx <- sub(".html", "ncSEMPlotx.jpg", htmlFile)
	jpeg(ncSEMPlotx,width = jpegwidth, height = jpegheight, quality = 100)

	#STB July2013
	plotFilepdf7 <- sub(".html", "ncSEMPlotx.pdf", htmlFile)
	dev.control("enable") 

	#GGPLOT2 code
	OVERLAID_SEM()

	#Output code
	void<-HTMLInsertGraph(GraphFileName=sub("[A-Z0-9a-z,:,\\\\]*App_Data[\\\\]","", ncSEMPlotx), Align="centre")

	#STB July2013
	if (pdfout=="Y") {
		pdf(file=sub("[A-Z0-9a-z,:,\\\\]*App_Data[\\\\]","", plotFilepdf7), height = pdfheight, width = pdfwidth) 
		dev.set(2) 
		dev.copy(which=3) 
		dev.off(2)
		dev.off(3)
		pdfFile_7<-sub("[A-Z0-9a-z,:,\\\\]*App_Data[\\\\]","",plotFilepdf7)
		linkToPdf7 <- paste ("<a href=\"",pdfFile_7,"\">Click here to view the PDF of the means with SEMs plot</a>", sep = "")
		HTML(linkToPdf7)
	}

}

#===================================================================================================================
if (SEMPlot == "Y" && displaypointSEM=="Y") {
	HTML("The plot includes individual observations, denoted by black dots.", align="left")
}

#===================================================================================================================
#===================================================================================================================
# Case profiles plot
#===================================================================================================================
#===================================================================================================================
#Title
if(CaseProfilesPlot == "Y" && FirstCatFactor == "NULL" && SecondCatFactor == "NULL"&& YAxisVars != "NULL") {
	HTML.title("Case profiles plot", HR=2, align="left")
} else if(CaseProfilesPlot  == "Y" && (FirstCatFactor != "NULL" || SecondCatFactor != "NULL")&& YAxisVars != "NULL") {
	HTML.title("Categorised case profiles plot", HR=2, align="left")
}

#non-numeric Y axis warning
#STB NOV2015 - Updating warning message
if(CaseProfilesPlot == "Y"  && is.numeric(graphdata$yvarrr_IVS)==FALSE) {
	HTML.title("As the Y-axis is non-numeric, no graph has been produced. Please review your selection.", HR=0, align="left")
}

#Global variables format
graphdata$Time_IVS<-as.factor(graphdata$xvarrr_IVS)
graphdata$Subject_IVS<-eval(parse(text = paste("graphdata$", CaseIDFactor)))

if(CaseProfilesPlot == "Y" && FirstCatFactor == "NULL" && SecondCatFactor == "NULL") {
	graphdata$Animal_IVS<-as.factor(eval(parse(text = paste("graphdata$", CaseIDFactor))))
	graphdata<-graphdata[order(graphdata$Animal_IVS, graphdata$Time_IVS), ]
} else	{
	graphdata$Animal_IVS<-paste(graphdata$l_l  , "=" , as.factor(eval(parse(text = paste("graphdata$", CaseIDFactor)))), sep = "")
	graphdata<-graphdata[order(graphdata$Animal_IVS, graphdata$Time_IVS), ]
}

#Colour range for individual animals on case profiles plot
temp<-IVS_F_cpplot_colour("Animal_IVS")
Gr_palette_A <- temp$Gr_palette_A
Gr_line <- temp$Gr_line
Gr_fill <-temp$Gr_fill

#===================================================================================================================
# Non-categorised Case profiles plot
#===================================================================================================================
if(CaseProfilesPlot == "Y" && FirstCatFactor == "NULL" && SecondCatFactor == "NULL"   && is.numeric(graphdata$yvarrr_IVS)==TRUE) {

	#Plot device setup
	nccaseplot <- sub(".html", "nccaseplot.jpg", htmlFile)
	jpeg(nccaseplot,width = jpegwidth, height = jpegheight, quality = 100)

	#STB July2013
	plotFilepdf22 <- sub(".html", "nccaseplot.pdf", htmlFile)
	dev.control("enable") 

	#GGPLOT2 code
	NONCAT_CPP()

	#Output code
	void<-HTMLInsertGraph(GraphFileName=sub("[A-Z0-9a-z,:,\\\\]*App_Data[\\\\]","", nccaseplot), Align="centre")

	#STB July2013
	if (pdfout=="Y") {
		pdf(file=sub("[A-Z0-9a-z,:,\\\\]*App_Data[\\\\]","", plotFilepdf22), height = pdfheight, width = pdfwidth) 
		dev.set(2) 
		dev.copy(which=3) 
		dev.off(2)
		dev.off(3)
		pdfFile_22<-sub("[A-Z0-9a-z,:,\\\\]*App_Data[\\\\]","",plotFilepdf22)
		linkToPdf22 <- paste ("<a href=\"",pdfFile_22,"\">Click here to view the PDF of the case profile plot</a>", sep = "")
		HTML(linkToPdf22)
	}
}

#===================================================================================================================
# One-categorised case profiles plot
#===================================================================================================================
if(CaseProfilesPlot == "Y" && ((FirstCatFactor != "NULL" && SecondCatFactor == "NULL") || (FirstCatFactor == "NULL" && SecondCatFactor != "NULL")) && GraphStyle == "Separate" && is.numeric(graphdata$yvarrr_IVS)==TRUE) {

	#Plot device setup
	nccaseplot <- sub(".html", "nccaseplot.jpg", htmlFile)
	jpeg(nccaseplot,width = jpegwidth, height = jpegheight, quality = 100)

	#STB July2013
	plotFilepdf23 <- sub(".html", "nccaseplot.pdf", htmlFile)
	dev.control("enable") 

	#GGPLOT2 code
	ONECATSEP_CPP()

	#Output code
	void<-HTMLInsertGraph(GraphFileName=sub("[A-Z0-9a-z,:,\\\\]*App_Data[\\\\]","", nccaseplot), Align="centre")

	#STB July2013
	if (pdfout=="Y") {
		pdf(file=sub("[A-Z0-9a-z,:,\\\\]*App_Data[\\\\]","", plotFilepdf23), height = pdfheight, width = pdfwidth) 
		dev.set(2) 
		dev.copy(which=3) 
		dev.off(2)
		dev.off(3)
		pdfFile_23<-sub("[A-Z0-9a-z,:,\\\\]*App_Data[\\\\]","",plotFilepdf23)
		linkToPdf23 <- paste ("<a href=\"",pdfFile_23,"\">Click here to view the PDF of the case profile plot</a>", sep = "")
		HTML(linkToPdf23)
	}
}

#===================================================================================================================
# Two-cat case profiles plot
#===================================================================================================================
if(CaseProfilesPlot == "Y" && FirstCatFactor != "NULL" && SecondCatFactor != "NULL" && GraphStyle == "Separate" && is.numeric(graphdata$yvarrr_IVS)==TRUE) {

	#Plot device setup
	nccaseplot <- sub(".html", "nccaseplot.jpg", htmlFile)
	jpeg(nccaseplot,width = jpegwidth, height = jpegheight, quality = 100)

	#STB July2013
	plotFilepdf25 <- sub(".html", "nccaseplot.pdf", htmlFile)
	dev.control("enable") 

	#GGPLOT2 code
	TWOCATSEP_CPP()

	#Output code
	void<-HTMLInsertGraph(GraphFileName=sub("[A-Z0-9a-z,:,\\\\]*App_Data[\\\\]","", nccaseplot), Align="centre")

	#STB July2013
	if (pdfout=="Y") {
		pdf(file=sub("[A-Z0-9a-z,:,\\\\]*App_Data[\\\\]","", plotFilepdf25), height = pdfheight, width = pdfwidth) 
		dev.set(2) 
		dev.copy(which=3) 
		dev.off(2)
		dev.off(3)
		pdfFile_25<-sub("[A-Z0-9a-z,:,\\\\]*App_Data[\\\\]","",plotFilepdf25)
		linkToPdf25 <- paste ("<a href=\"",pdfFile_25,"\">Click here to view the PDF of the case profile plot</a>", sep = "")
		HTML(linkToPdf25)
	}
}

#===================================================================================================================
# Overlaid case profiles plot
#===================================================================================================================
if(CaseProfilesPlot == "Y" && ((FirstCatFactor != "NULL" && SecondCatFactor == "NULL") || (FirstCatFactor == "NULL" && SecondCatFactor != "NULL")|| (FirstCatFactor != "NULL" && SecondCatFactor != "NULL"))   && GraphStyle == "Overlaid" && is.numeric(graphdata$yvarrr_IVS)==TRUE) {

	#Plot device setup
	nccaseplot <- sub(".html", "nccaseplot.jpg", htmlFile)
	jpeg(nccaseplot,width = jpegwidth, height = jpegheight, quality = 100)

	#STB July2013
	plotFilepdf24 <- sub(".html", "nccaseplot.pdf", htmlFile)
	dev.control("enable") 

	#GGPLOT2 code
	OVERLAID_CPP()

	#Output code
	void<-HTMLInsertGraph(GraphFileName=sub("[A-Z0-9a-z,:,\\\\]*App_Data[\\\\]","", nccaseplot), Align="centre")

	#STB July2013
	if (pdfout=="Y") {
		pdf(file=sub("[A-Z0-9a-z,:,\\\\]*App_Data[\\\\]","", plotFilepdf24), height = pdfheight, width = pdfwidth) 
		dev.set(2) 
		dev.copy(which=3) 
		dev.off(2)
		dev.off(3)
		pdfFile_24<-sub("[A-Z0-9a-z,:,\\\\]*App_Data[\\\\]","",plotFilepdf24)
		linkToPdf24 <- paste ("<a href=\"",pdfFile_24,"\">Click here to view the PDF of the case profile plot</a>", sep = "")
		HTML(linkToPdf24)
	}
}

#===================================================================================================================
#===================================================================================================================
#Boxplot - y should be numerical, x should be categorical
#===================================================================================================================
#===================================================================================================================
#Titles
if(BoxPlot == "Y" && FirstCatFactor == "NULL" && SecondCatFactor == "NULL"&& YAxisVars != "NULL") {
	#STB May 2012 - change name to box-plot
	HTML.title("Box-plot", HR=2, align="left")
} else if (BoxPlot == "Y" && (FirstCatFactor != "NULL" || SecondCatFactor != "NULL")&& YAxisVars != "NULL") {
	#STB May 2012 - change name to box-plot
	HTML.title("Categorised box-plot", HR=2, align="left")
}	

if(BoxPlot == "Y" && is.numeric(graphdata$yvarrr_IVS)==FALSE) {
	HTML("As the Y-axis is non-numeric, no graph has been produced. Please review your selection.", align="left")
}

#Generating the categorical x-axis
if(BoxPlot == "Y") {
	graphdata$xvarrr_IVS_BP <-as.factor(graphdata$xvarrr_IVS)
}


#Jitter selection
w_Gr_jitBP <- Gr_w_Gr_jit
h_Gr_jitBP <- 0

#===================================================================================================================
# Non-Categorised boxplot (graphics + non-parametric)
#===================================================================================================================
if(BoxPlot == "Y" && FirstCatFactor == "NULL" && SecondCatFactor == "NULL"   && is.numeric(graphdata$yvarrr_IVS)==TRUE) {

	#Creating outliers dataset for the boxplot
	temp<-IVS_F_boxplot_outlier()
	outlierdata <-temp$outlierdata
	boxdata <- temp$boxdata
	ymin <- temp$ymin
	ymax <- temp$ymax
	range <-temp$range

	#Plot device setup
	ncboxplot <- sub(".html", "ncboxplot.jpg", htmlFile)
	jpeg(ncboxplot,width = jpegwidth, height = jpegheight, quality = 100)

	#STB July2013
	plotFilepdfx <- sub(".html", "ncboxplot.pdf", htmlFile)
	dev.control("enable") 

	#GGPLOT2 code
	NONCAT_BOX()

	#Output code
	void<-HTMLInsertGraph(GraphFileName=sub("[A-Z0-9a-z,:,\\\\]*App_Data[\\\\]","", ncboxplot), Align="centre")

	#STB July2013
	if (pdfout=="Y") {
		pdf(file=sub("[A-Z0-9a-z,:,\\\\]*App_Data[\\\\]","", plotFilepdfx), height = pdfheight, width = pdfwidth) 
		dev.set(2) 
		dev.copy(which=3) 
		dev.off(2)
		dev.off(3)
		pdfFile_x<-sub("[A-Z0-9a-z,:,\\\\]*App_Data[\\\\]","",plotFilepdfx)
		linkToPdfx <- paste ("<a href=\"",pdfFile_x,"\">Click here to view the PDF of the box-plot</a>", sep = "")
		HTML(linkToPdfx)
	}
}

#===================================================================================================================
# ONE CAT - seperate Boxplot
#===================================================================================================================
if(BoxPlot == "Y" && ((FirstCatFactor != "NULL" && SecondCatFactor == "NULL") || (FirstCatFactor == "NULL" && SecondCatFactor != "NULL")) && GraphStyle == "Separate" && is.numeric(graphdata$yvarrr_IVS)==TRUE) {

	#Generating the dataset boxdata
	graphdata$allcat_IVS<-as.factor(paste(graphdata$xvarrr_IVS_BP, "xxxIVSxxx",graphdata$l_l, sep=""))

	#setting up parameters
	sumdata<-graphdata
	levz<-length(unique(graphdata$allcat_IVS))
	lev<-levels(graphdata$allcat_IVS)

	sumtable<- matrix(data = NA, nrow = levz, ncol = 5)
	rownames(sumtable)<-c(unique(levels(sumdata$allcat_IVS)))
	colnames(sumtable)<-c("minq","lowerq","medq","upperq","maxq")

	#Add entries to the table
	for (i in 1:levz) {
		tempdata<-subset(sumdata, sumdata$allcat_IVS == unique(levels(as.factor(sumdata$allcat_IVS)))[i])
		sumtable[i,1]<-boxplot.stats(split(tempdata$yvarrr_IVS,tempdata$allcat_IVS)[[i]])$stats[1]
		sumtable[i,2]<-boxplot.stats(split(tempdata$yvarrr_IVS,tempdata$allcat_IVS)[[i]])$stats[2]
		sumtable[i,3]<-boxplot.stats(split(tempdata$yvarrr_IVS,tempdata$allcat_IVS)[[i]])$stats[3]
		sumtable[i,4]<-boxplot.stats(split(tempdata$yvarrr_IVS,tempdata$allcat_IVS)[[i]])$stats[4]
		sumtable[i,5]<-boxplot.stats(split(tempdata$yvarrr_IVS,tempdata$allcat_IVS)[[i]])$stats[5]
	}
	boxdata<-data.frame(sumtable)
	boxdata$lev<-unique(levels(as.factor(sumdata$allcat_IVS)))
	boxdata<-data.frame(boxdata, colsplit(boxdata$lev,"xxxIVSxxx", names=c("xvarrr_IVS_BP","l_l")))
	boxdata$allcat_IVS <-boxdata$lev
	boxdata$xvarrr_IVS_BP <-as.factor(boxdata$xvarrr_IVS_BP)

	#Generating a dataset of outliers
	levs<-length(lev)
	outlierdata<-graphdata[1,]
	for (k in 1:levs) {
		tempdata<-subset(graphdata, graphdata$allcat_IVS == unique(levels(as.factor(graphdata$allcat_IVS)))[k])
		lev1<-boxdata[k,1]
		lev2<-boxdata[k,5]
		newdata <- subset(tempdata, yvarrr_IVS < lev1 | yvarrr_IVS > lev2 , )
		tempdata<-tempdata[1,]
		tempdata$yvarrr_IVS = NA

		if(dim(newdata)[1] >= 1) {	
			newdata<-rbind(newdata,tempdata)
		} else {
			newdata<-tempdata
		}			

		if(dim(newdata)[1] >= 1) {
			outlierdata<-rbind(outlierdata, newdata)
		}
	}
	outlierdata<-outlierdata[-1,]

	#GGPLOT2 variable options
	if (Outliers == "N") {
		stats <- boxplot(yvarrr_IVS~allcat_IVS, data=graphdata, plot=FALSE)$stat
		ymin  <- min(stats[1,], na.rm=TRUE)
		ymax  <- max(stats[5,], na.rm=TRUE)
		range <- (ymax-ymin)*0.1
	} else {
		ymax=max(graphdata$yvarrr_IVS, na.rm=TRUE)
		ymin=min(graphdata$yvarrr_IVS, na.rm=TRUE)
		range <- (ymax-ymin)*0.1
	}

	#Plot device setup
	ncboxplot <- sub(".html", "ncboxplot.jpg", htmlFile)
	jpeg(ncboxplot,width = jpegwidth, height = jpegheight, quality = 100)

	#STB July2013
	plotFilepdf11 <- sub(".html", "ncboxplot.pdf", htmlFile)
	dev.control("enable") 

	#GGPLOT2 code
	ONECATSEP_BOX()

	#Output code
	void2<-HTMLInsertGraph(GraphFileName=sub("[A-Z0-9a-z,:,\\\\]*App_Data[\\\\]","", ncboxplot), Align="centre")

	#STB July2013
	if (pdfout=="Y") {
		pdf(file=sub("[A-Z0-9a-z,:,\\\\]*App_Data[\\\\]","", plotFilepdf11), height = pdfheight, width = pdfwidth) 
		dev.set(2) 
		dev.copy(which=3) 
		dev.off(2)
		dev.off(3)
		pdfFile_11<-sub("[A-Z0-9a-z,:,\\\\]*App_Data[\\\\]","",plotFilepdf11)
		linkToPdf11 <- paste ("<a href=\"",pdfFile_11,"\">Click here to view the PDF of the box-plot</a>", sep = "")
		HTML(linkToPdf11)
	}
}

#===================================================================================================================
# TWO CAT - seperate Boxplot
#===================================================================================================================
if(BoxPlot == "Y" && FirstCatFactor != "NULL" && SecondCatFactor != "NULL"  && GraphStyle == "Separate" && is.numeric(graphdata$yvarrr_IVS)==TRUE) {

	#Generating the dataset boxdata
	graphdata$allcat_IVS<-as.factor(paste(graphdata$xvarrr_IVS_BP, "xxxIVSxxx",graphdata$firstcatvarrr_IVS, "xxxIVSxxx",graphdata$secondcatvarrr_IVS,  sep=""))

	#setting up parameters
	sumdata<-graphdata
	levz<-length(unique(graphdata$allcat_IVS))
	lev<-levels(graphdata$allcat_IVS)

	sumtable<- matrix(data = NA, nrow = levz, ncol = 5)
	rownames(sumtable)<-c(unique(levels(sumdata$allcat_IVS)))
	colnames(sumtable)<-c("minq","lowerq","medq","upperq","maxq")

	#Add entries to the table
	for (i in 1:levz) {
		tempdata<-subset(sumdata, sumdata$allcat_IVS == unique(levels(as.factor(sumdata$allcat_IVS)))[i])
		sumtable[i,1]<-boxplot.stats(split(tempdata$yvarrr_IVS,tempdata$allcat_IVS)[[i]])$stats[1]
		sumtable[i,2]<-boxplot.stats(split(tempdata$yvarrr_IVS,tempdata$allcat_IVS)[[i]])$stats[2]
		sumtable[i,3]<-boxplot.stats(split(tempdata$yvarrr_IVS,tempdata$allcat_IVS)[[i]])$stats[3]
		sumtable[i,4]<-boxplot.stats(split(tempdata$yvarrr_IVS,tempdata$allcat_IVS)[[i]])$stats[4]
		sumtable[i,5]<-boxplot.stats(split(tempdata$yvarrr_IVS,tempdata$allcat_IVS)[[i]])$stats[5]
	}
	boxdata<-data.frame(sumtable)
	boxdata$lev<-unique(levels(as.factor(sumdata$allcat_IVS)))
	boxdata <- data.frame(boxdata, colsplit(boxdata$lev,"xxxIVSxxx", names=c("xvarrr_IVS_BP","firstcatvarrr_IVS","secondcatvarrr_IVS")))
	boxdata$allcat_IVS <-boxdata$lev
	boxdata$xvarrr_IVS_BP <-as.factor(boxdata$xvarrr_IVS_BP)

	#Generating a dataset of outliers
	levs<-length(lev)
	outlierdata<-graphdata[1,]
	for (k in 1:levs) {
		tempdata<-subset(graphdata, graphdata$allcat_IVS == unique(levels(as.factor(graphdata$allcat_IVS)))[k])
		lev1<-boxdata[k,1]
		lev2<-boxdata[k,5]
		newdata <- subset(tempdata, yvarrr_IVS < lev1 | yvarrr_IVS > lev2 , )
		tempdata<-tempdata[1,]
		tempdata$yvarrr_IVS = NA

		if(dim(newdata)[1] >= 1) {	
			newdata<-rbind(newdata,tempdata)
		} else {
			newdata<-tempdata
		}
		if(dim(newdata)[1] >= 1) {
			outlierdata<-rbind(outlierdata, newdata)
		}
	}
	outlierdata<-outlierdata[-1,]

	#GGPLOT2 variable options
	if (Outliers == "N") {
		stats <- boxplot(yvarrr_IVS~allcat_IVS, data=graphdata, plot=FALSE)$stat
		ymin  <- min(stats[1,], na.rm=TRUE)
		ymax  <- max(stats[5,], na.rm=TRUE)
		range <- (ymax-ymin)*0.1
	} else {
		ymax=max(graphdata$yvarrr_IVS, na.rm=TRUE)
		ymin=min(graphdata$yvarrr_IVS, na.rm=TRUE)
		range <- (ymax-ymin)*0.1
	}

	#Plot device setup
	ncboxplot <- sub(".html", "ncboxplot.jpg", htmlFile)
	jpeg(ncboxplot,width = jpegwidth, height = jpegheight, quality = 100)

	#STB July2013
	plotFilepdf13 <- sub(".html", "ncboxplot.pdf", htmlFile)
	dev.control("enable") 

	#GGPLOT2 code
	TWOCATSEP_BOX()

	#Output code
	void<-HTMLInsertGraph(GraphFileName=sub("[A-Z0-9a-z,:,\\\\]*App_Data[\\\\]","", ncboxplot), Align="centre")

	#STB July2013
	if (pdfout=="Y") {
		pdf(file=sub("[A-Z0-9a-z,:,\\\\]*App_Data[\\\\]","", plotFilepdf13), height = pdfheight, width = pdfwidth) 
		dev.set(2) 
		dev.copy(which=3) 
		dev.off(2)
		dev.off(3)
		pdfFile_13<-sub("[A-Z0-9a-z,:,\\\\]*App_Data[\\\\]","",plotFilepdf13)
		linkToPdf13 <- paste ("<a href=\"",pdfFile_13,"\">Click here to view the PDF of the box-plot</a>", sep = "")
		HTML(linkToPdf13)
	}
}

#===================================================================================================================
# ONE CAT - overlaid, Boxplot
#===================================================================================================================
if(BoxPlot == "Y" && ((FirstCatFactor != "NULL" && SecondCatFactor == "NULL") || (FirstCatFactor == "NULL" && SecondCatFactor != "NULL")|| (FirstCatFactor != "NULL" && SecondCatFactor != "NULL"))   && GraphStyle == "Overlaid" && is.numeric(graphdata$yvarrr_IVS)==TRUE) {

	#Adding blanks where needed to the original dataset (for plotting all the data)
	graphdataBOX<-graphdata
	graphdataBOX$Type <- "Dataset"

	catlevels <-length(unique(levels(as.factor(graphdataBOX$l_l))))
	xlevels   <-length(unique(levels(as.factor(graphdataBOX$xvarrr_IVS_BP))))
	blankdata = data.frame(matrix(vector(), (xlevels*catlevels), dim(graphdataBOX)[2], dimnames=list(c(), c(colnames(graphdataBOX)))), stringsAsFactors=F)
	i<-1

	#Chop up dataset to seperate group combinations, test to see if it has size 0 and if so add a new 0 entry to blankdata
	for (k in 1:xlevels) {
		tempdata<-subset(graphdataBOX, graphdataBOX$xvarrr_IVS_BP  == unique(levels(as.factor(graphdataBOX$xvarrr_IVS_BP)))[k])
		for (j in 1:catlevels) {
			tempdata2<-subset(tempdata, tempdata$l_l == unique(levels(as.factor(graphdataBOX$l_l)))[j])
			if(length(unique(tempdata2$xvarrr_IVS))==0) {
				blankdata$yvarrr_IVS[i]   = NA
				blankdata$xvarrr_IVS_BP[i]   =unique(levels(as.factor(graphdataBOX$xvarrr_IVS_BP)))[k]
				blankdata$l_l[i] =unique(levels(as.factor(graphdataBOX$l_l)))[j]
				i=i+1
			}	
		}
	}

	if (i > 1) {
		blankdata$Type <- "Blanks"
		blankdata<-blankdata[c(1:(i-1)),]
		graphdataBOX_overall <- rbind(blankdata,graphdataBOX)
	} else {
		graphdataBOX_overall <- graphdataBOX
	}

	#GGPLOT2 variable options
	if (Outliers == "N")	{
		stats <- boxplot(yvarrr_IVS~l_l, data=graphdata, plot=FALSE)$stat
		ymin  <- min(stats[1,], na.rm=TRUE)
		ymax  <- max(stats[5,], na.rm=TRUE)
		range <- (ymax-ymin)*0.1
	} else {
		ymax=max(graphdata$yvarrr_IVS, na.rm=TRUE)
		ymin=min(graphdata$yvarrr_IVS, na.rm=TRUE)
		range <- (ymax-ymin)*0.1
	}

	#Adjusting the column width if there are missing group combinations
	catlevels <-length(unique(levels(as.factor(graphdata$l_l))))
	xlevels   <-length(unique(levels(as.factor(graphdata$xvarrr_IVS_BP))))
	blankdata = data.frame(matrix(vector(), (xlevels*catlevels), dim(graphdata)[2], dimnames=list(c(), c(colnames(graphdata)))), stringsAsFactors=F)
	i<-1

	#Chop up dataset to seperate group combinations, test to see if it has size 0 and if so add a new 0 entry to blankdata
	for (k in 1:xlevels) {
		tempdata<-subset(graphdata, graphdata$xvarrr_IVS_BP == unique(levels(as.factor(graphdata$xvarrr_IVS_BP)))[k])

		for (j in 1:catlevels) {
			tempdata2<-subset(tempdata, tempdata$l_l == unique(levels(as.factor(graphdata$l_l)))[j])
			if(length(unique(tempdata2$xvarrr_IVS))==0) {
				blankdata$yvarrr_IVS[i]   = ymax*10000
				blankdata$xvarrr_IVS_BP[i]   =unique(levels(as.factor(graphdata$xvarrr_IVS_BP)))[k]
				blankdata$l_l[i] =unique(levels(as.factor(graphdata$l_l)))[j]
				i=i+1
			}	
		}
	}

	blankdata<-blankdata[c(1:(i-1)),]
	graphdata_overall_BP <- rbind(blankdata,graphdata)
	if (i ==1) {
		graphdata_overall_BP<-graphdata_overall_BP[-1,]
	}

	#Generating the dataset boxdata
	graphdata_overall_BP$allcat_IVS<-as.factor(paste(graphdata_overall_BP$xvarrr_IVS_BP, "xxxIVSxxx",graphdata_overall_BP$l_l, sep=""))

	#setting up parameters
	sumdata<-graphdata_overall_BP
	levz<-length(unique(graphdata_overall_BP$allcat_IVS))
	lev<-levels(graphdata_overall_BP$allcat_IVS)

	sumtable<- matrix(data = NA, nrow = levz, ncol = 5)
	rownames(sumtable)<-c(unique(levels(sumdata$allcat_IVS)))
	colnames(sumtable)<-c("minq","lowerq","medq","upperq","maxq")

	#Add entries to the table
	for (i in 1:levz) {
		tempdata<-subset(sumdata, sumdata$allcat_IVS == unique(levels(as.factor(sumdata$allcat_IVS)))[i])
		sumtable[i,1]<-boxplot.stats(split(tempdata$yvarrr_IVS,tempdata$allcat_IVS)[[i]])$stats[1]
		sumtable[i,2]<-boxplot.stats(split(tempdata$yvarrr_IVS,tempdata$allcat_IVS)[[i]])$stats[2]
		sumtable[i,3]<-boxplot.stats(split(tempdata$yvarrr_IVS,tempdata$allcat_IVS)[[i]])$stats[3]
		sumtable[i,4]<-boxplot.stats(split(tempdata$yvarrr_IVS,tempdata$allcat_IVS)[[i]])$stats[4]
		sumtable[i,5]<-boxplot.stats(split(tempdata$yvarrr_IVS,tempdata$allcat_IVS)[[i]])$stats[5]
	}
	boxdata<-data.frame(sumtable)
	boxdata$lev<-unique(levels(as.factor(sumdata$allcat_IVS)))
	boxdata<-data.frame(boxdata, colsplit(boxdata$lev,"xxxIVSxxx", names=c("xvarrr_IVS_BP","l_l")))
	boxdata$allcat_IVS <-boxdata$lev
	boxdata$xvarrr_IVS_BP <-as.factor(boxdata$xvarrr_IVS_BP)

	#Creating Q1 Q3 and whiskers to plot directly using the geom_boxplot function
	boxtest<-data.frame(t(boxplot(as.numeric(yvarrr_IVS)~allcat_IVS, data=graphdata_overall_BP, plot=FALSE)$stat))
	colnames(boxtest)<- c("minq","lowerq","medq","upperq","maxq")
	lev<-levels(graphdata_overall_BP$allcat_IVS)
	boxdata<-cbind(boxtest, lev)
	boxdata$allcat_IVS <-boxdata$lev
	boxdata <- data.frame(boxdata, colsplit(boxdata$allcat_IVS,"xxxIVSxxx", names=c("xvarrr_IVS_BP","l_l")))
	boxdata$xvarrr_IVS_BP <-as.factor(boxdata$xvarrr_IVS_BP)

	#Generating a dataset of outliers
	outliertest<-"N"
	levs<-length(lev)
	outlierdata<-graphdata_overall_BP[1,]
	for (k in 1:levs) {
		tempdata<-subset(graphdata_overall_BP, graphdata_overall_BP$allcat_IVS == unique(levels(as.factor(graphdata_overall_BP$allcat_IVS)))[k])
		lev1<-boxdata[k,1]
		lev2<-boxdata[k,5]
		newdata <- subset(tempdata, yvarrr_IVS < lev1 | yvarrr_IVS > lev2 , )
		tempdata<-tempdata[1,]
		tempdata$yvarrr_IVS = NA

		if(dim(newdata)[1] >= 1) {	
			newdata<-rbind(newdata,tempdata)
			outliertest<-"Y"
		} else {
			newdata<-tempdata
		}

		if(dim(newdata)[1] >= 1) {
			outlierdata<-rbind(outlierdata, newdata)
		}
	}
	outlierdata<-outlierdata[-1,]

	#Plot device setup
	ncboxplot <- sub(".html", "ncboxplot.jpg", htmlFile)
	jpeg(ncboxplot,width = jpegwidth, height = jpegheight, quality = 100)

	#STB July2013
	plotFilepdf12 <- sub(".html", "ncboxplot.pdf", htmlFile)
	dev.control("enable") 

	#GGPLOT2 code
	OVERLAID_BOX()

	#Output code
	void<-HTMLInsertGraph(GraphFileName=sub("[A-Z0-9a-z,:,\\\\]*App_Data[\\\\]","", ncboxplot), Align="centre")

	#STB July2013
	if (pdfout=="Y") {
		pdf(file=sub("[A-Z0-9a-z,:,\\\\]*App_Data[\\\\]","", plotFilepdf12), height = pdfheight, width = pdfwidth) 
		dev.set(2) 
		dev.copy(which=3) 
		dev.off(2)
		dev.off(3)
		pdfFile_12<-sub("[A-Z0-9a-z,:,\\\\]*App_Data[\\\\]","",plotFilepdf12)
		linkToPdf12 <- paste ("<a href=\"",pdfFile_12,"\">Click here to view the PDF of the box-plot</a>", sep = "")
		HTML(linkToPdf12)
	}
}

#===================================================================================================================
#Final box-plot options
#===================================================================================================================
if(BoxPlot == "Y" && is.numeric(graphdata$yvarrr_IVS)==TRUE) {
	HTML("On the box-plot the median is denoted by the horizontal line within the box. The box indicates the interquartile range, where the lower and upper quartiles are calculated using the type 2 method, see Hyndman and Fan (1996). The whiskers extend to the most extreme data point which is no more than 1.5 times the length of the box away from the box.", align="left")

	if (Outliers=="Y" && displaypointBOX=="N") {
		HTML("Individual observations that lie outside the outlier range are included on the plot using circles.",  align="left")
	}
	if (Outliers=="Y" && displaypointBOX=="Y") {
		HTML("The plot includes individual observations, those that lie outside the outlier range are surrounded by an asterix black star.",  align="left")
	}
	if (Outliers=="N" && displaypointBOX=="Y"){
		HTML("The plot includes individual observations, denoted by black dots.", align="left")
	}
}

#===================================================================================================================
#===================================================================================================================
#Histograms
#===================================================================================================================
#===================================================================================================================
#Titles
if(HistogramPlot == "Y" && FirstCatFactor == "NULL" && SecondCatFactor == "NULL") {
	HTML.title("Density histogram", HR=2, align="left")
} else if (HistogramPlot == "Y" && (FirstCatFactor != "NULL" || SecondCatFactor != "NULL")) {
	HTML.title("Categorised density histogram", HR=2, align="left")
}

#Warning message if response is not numeric
#STB NOV2015 - Updating warning message
if(HistogramPlot == "Y" && is.numeric(graphdata$yvarrr_IVS)==FALSE) {
	HTML.title("As the response variable is non-numeric, no graph has been produced. Please review your selection.", HR=0, align="left")
}

#GGPLOT2 variable options
if (HistogramPlot == "Y" && NormalDistFit=="N") {
	Line_type <- Line_type_blank
	Gr_alpha<-0
} else {
	Line_type <- Line_type_solid
	Gr_alpha<-1
}

if (HistogramPlot == "Y" && is.numeric(graphdata$yvarrr_IVS)==TRUE) {
	#calculating the bin width
	ymax<-max(graphdata$yvarrr_IVS, na.rm=TRUE)
	ymin<-min(graphdata$yvarrr_IVS, na.rm=TRUE)
	binrange<-(ymax-ymin)/10
}

#===================================================================================================================
#Non-categorised histogram
#===================================================================================================================
if(HistogramPlot == "Y" && FirstCatFactor == "NULL" && SecondCatFactor == "NULL"   && is.numeric(graphdata$yvarrr_IVS)==TRUE) {

	#Plot device setup
	nchistplot <- sub(".html", "nchistplot.jpg", htmlFile)
	jpeg(nchistplot,width = jpegwidth, height = jpegheight, quality = 100)

	#STB July2013
	plotFilepdf1 <- sub(".html", "nchistplot.pdf", htmlFile)
	dev.control("enable") 

	#GGPLOT2 code
	NONCAT_HIS()

	#Output code
	void<-HTMLInsertGraph(GraphFileName=sub("[A-Z0-9a-z,:,\\\\]*App_Data[\\\\]","", nchistplot), Align="centre")

	#STB July2013
	if (pdfout=="Y") {
		pdf(file=sub("[A-Z0-9a-z,:,\\\\]*App_Data[\\\\]","", plotFilepdf1), height = pdfheight, width = pdfwidth) 
		dev.set(2) 
		dev.copy(which=3) 
		dev.off(2)
		dev.off(3)
		pdfFile_1<-sub("[A-Z0-9a-z,:,\\\\]*App_Data[\\\\]","",plotFilepdf1)
		linkToPdf1 <- paste ("<a href=\"",pdfFile_1,"\">Click here to view the PDF of the histogram</a>", sep = "")
		HTML(linkToPdf1)
	}
}

#===================================================================================================================
#One-categorised seperate histogram
#===================================================================================================================
if(HistogramPlot == "Y" && ((FirstCatFactor != "NULL" && SecondCatFactor == "NULL") || (FirstCatFactor == "NULL" && SecondCatFactor != "NULL")) && GraphStyle == "Separate" && is.numeric(graphdata$yvarrr_IVS)==TRUE) {

	#Dataset manipulations
	#Creating normal distribution grid
	grid <- with(graphdata, seq(min(yvarrr_IVS, na.rm=TRUE), max(yvarrr_IVS, na.rm=TRUE), length=100))
	normaldens<-ddply(graphdata, "l_l", function (df) { data.frame(yvarrr_IVS = grid, density=dnorm(grid, mean(df$yvarrr_IVS), sd(df$yvarrr_IVS)) ) })

	#Plot device setup
	nchistplot <- sub(".html", "nchistplot.jpg", htmlFile)
	jpeg(nchistplot,width = jpegwidth, height = jpegheight, quality = 100)

	#STB July2013
	plotFilepdf2 <- sub(".html", "nchistplot.pdf", htmlFile)
	dev.control("enable") 

	#GGPLOT2 code
	ONECATSEP_HIS()

	#Output code
	void<-HTMLInsertGraph(GraphFileName=sub("[A-Z0-9a-z,:,\\\\]*App_Data[\\\\]","", nchistplot), Align="centre")

	#STB July2013
	if (pdfout=="Y") {
		pdf(file=sub("[A-Z0-9a-z,:,\\\\]*App_Data[\\\\]","", plotFilepdf2), height = pdfheight, width = pdfwidth) 
		dev.set(2) 
		dev.copy(which=3) 
		dev.off(2)
		dev.off(3)
		pdfFile_2<-sub("[A-Z0-9a-z,:,\\\\]*App_Data[\\\\]","",plotFilepdf2)
		linkToPdf2 <- paste ("<a href=\"",pdfFile_2,"\">Click here to view the PDF of the histogram</a>", sep = "")
		HTML(linkToPdf2)
	}
}

#===================================================================================================================
#Two-categorised seperate histogram
#===================================================================================================================
if(HistogramPlot == "Y" && FirstCatFactor != "NULL" && SecondCatFactor != "NULL" && GraphStyle == "Separate" && is.numeric(graphdata$yvarrr_IVS)==TRUE) {

	#Dataset manipulations
	#Creating normal distribution grid
	graphdatazzz<-graphdata
	graphdatazzz$Catz<-paste(graphdatazzz$firstcatvarrr_IVS, "xxx", graphdatazzz$secondcatvarrr_IVS)
	grid <- with(graphdatazzz, seq(min(yvarrr_IVS, na.rm=TRUE), max(yvarrr_IVS, na.rm=TRUE), length=100))
	normaldens<-ddply(graphdatazzz, "Catz", function (df) { data.frame(yvarrr_IVS = grid, density=dnorm(grid, mean(df$yvarrr_IVS), sd(df$yvarrr_IVS)) ) })

	temp <- t(data.frame(strsplit(normaldens$Catz," xxx ")))
	rownames(temp)<-NULL
	colnames(temp)<-c("firstcatvarrr_IVS", "secondcatvarrr_IVS")
	normaldens<-cbind(normaldens, temp)

	#Plot device setup
	nchistplot <- sub(".html", "nchistplot.jpg", htmlFile)
	jpeg(nchistplot,width = jpegwidth, height = jpegheight, quality = 100)

	#STB July2013
	plotFilepdf4 <- sub(".html", "nchistplot.pdf", htmlFile)
	dev.control("enable") 

	#GGPLOT2 code
	TWOCATSEP_HIS()

	#Output code
	void<-HTMLInsertGraph(GraphFileName=sub("[A-Z0-9a-z,:,\\\\]*App_Data[\\\\]","", nchistplot), Align="centre")

	#STB July2013
	if (pdfout=="Y") {
		pdf(file=sub("[A-Z0-9a-z,:,\\\\]*App_Data[\\\\]","", plotFilepdf4), height = pdfheight, width = pdfwidth) 
		dev.set(2) 
		dev.copy(which=3) 
		dev.off(2)
		dev.off(3)
		pdfFile_4<-sub("[A-Z0-9a-z,:,\\\\]*App_Data[\\\\]","",plotFilepdf4)
		linkToPdf4 <- paste ("<a href=\"",pdfFile_4,"\">Click here to view the PDF of the histogram</a>", sep = "")
		HTML(linkToPdf4)
	}
}

#===================================================================================================================
#Overlaid histogram
#===================================================================================================================
if(HistogramPlot == "Y" && ((FirstCatFactor != "NULL" && SecondCatFactor == "NULL") || (FirstCatFactor == "NULL" && SecondCatFactor != "NULL")|| (FirstCatFactor != "NULL" && SecondCatFactor != "NULL"))   && GraphStyle == "Overlaid" && is.numeric(graphdata$yvarrr_IVS)==TRUE) {

	#Dataset manipulations
	#Creating a dataset with responses in seperate columns
	rows<-dim(graphdata)[1]
	cols<-dim(graphdata)[2]
	nlevels<-length(unique(graphdata$catfact))
	extra<-matrix(data=NA, nrow=rows, ncol=nlevels)

	for (i in 1:nlevels) {
		for (j in 1:rows) {
			if (graphdata$catfact[j] == unique(graphdata$catfact)[i]) {
				extra[j,i]<-graphdata$yvarrr_IVS[j]
			}
		}
	}
	colnames(extra)<-paste("Tzzz",rep(1:nlevels), sep="")
	histdata<-cbind(graphdata, extra)

	#Plot device setup
	nchistplot <- sub(".html", "nchistplot.jpg", htmlFile)
	jpeg(nchistplot,width = jpegwidth, height = jpegheight, quality = 100)

	#STB July2013
	plotFilepdf3z <- sub(".html", "nchistplot.pdf", htmlFile)
	dev.control("enable") 

	#GGPLOT2 code
	OVERLAID_HIS()

	#Output code	
	void<-HTMLInsertGraph(GraphFileName=sub("[A-Z0-9a-z,:,\\\\]*App_Data[\\\\]","", nchistplot), Align="centre")

	#STB July2013
	if (pdfout=="Y") {
		pdf(file=sub("[A-Z0-9a-z,:,\\\\]*App_Data[\\\\]","", plotFilepdf3z), height = pdfheight, width = pdfwidth) 
		dev.set(2) 
		dev.copy(which=3) 
		dev.off(2)
		dev.off(3)
		pdfFile_3z<-sub("[A-Z0-9a-z,:,\\\\]*App_Data[\\\\]","",plotFilepdf3z)
		linkToPdf3z <- paste ("<a href=\"",pdfFile_3z,"\">Click here to view the PDF of the histogram</a>", sep = "")
		HTML(linkToPdf3z)
	}
}

#===================================================================================================================
# References
#===================================================================================================================
Ref_list <- R_refs()

#Bate and Clark comment
HTML(refxx, align = "left")

HTML.title("Statistical references", HR = 2, align = "left")
HTML(Ref_list$BateClark_ref, align = "left")

if (BoxPlot == "Y" && is.numeric(graphdata$yvarrr_IVS) == TRUE) {
    HTML("Hyndman RJ and Fan Y. (1996). Sample quantiles in statistical packages. American Statistician 50, 361-365. ", align = "left")
}

HTML.title("R references", HR = 2, align = "left")
HTML(Ref_list$R_ref, align = "left")
HTML(Ref_list$GGally_ref, align = "left")
HTML(Ref_list$RColorBrewers_ref,  align = "left")
HTML(Ref_list$GGPLot2_ref,  align = "left")
HTML(Ref_list$ggrepel_ref,  align="left")
HTML(Ref_list$reshape_ref,  align = "left")
HTML(Ref_list$plyr_ref,  align = "left")
HTML(Ref_list$scales_ref,  align = "left")
HTML(Ref_list$R2HTML_ref,  align = "left")
HTML(Ref_list$PROTO_ref,  align = "left")


#===================================================================================================================
#Show dataset
#===================================================================================================================
graphdata2 <- data.frame(graphdata$yvarrr_IVS)
names <- c(YAxisVars)
colnames(graphdata2) <- names

if (XAxisVar != "NULL") {
    graphdata3 <- data.frame(graphdata$xvarrr_IVS)
    graphdata2 <- cbind(graphdata2, graphdata3)
    names <- c(YAxisVars, XAxisVar)
    colnames(graphdata2) <- names
}

if (FirstCatFactor != "NULL") {
    graphdata3 <- data.frame(eval(parse(text = paste("graphdata$", FirstCatFactor))))
    graphdata2 <- cbind(graphdata2, graphdata3)
    names <- c(names, FirstCatFactor)
    colnames(graphdata2) <- names
}

if (SecondCatFactor != "NULL") {
    graphdata3 <- data.frame(eval(parse(text = paste("graphdata$", SecondCatFactor))))
    graphdata2 <- cbind(graphdata2, graphdata3)
    names <- c(names, SecondCatFactor)
    colnames(graphdata2) <- names
}

if (CaseProfilesPlot == "Y") {
    graphdata3 <- data.frame(graphdata$Subject_IVS)
    graphdata2 <- cbind(graphdata2, graphdata3)
    names <- c(names, CaseIDFactor)
    colnames(graphdata2) <- names
}


if (showdataset == "Y") {
    observ <- data.frame(c(1:dim(graphdata2)[1]))
    colnames(observ) <- c("Observation")
    graphdata3 <- cbind(observ, graphdata2)

    HTML.title("Analysis dataset", HR = 2, align = "left")
    HTML(graphdata3, classfirstline = "second", align = "left", row.names = "FALSE")
}


#===================================================================================================================
#Show arguments
#===================================================================================================================

HTML.title("Analysis options", HR=2, align="left")

if (YAxisVars != "NULL") {
	HTML(paste("Response variable: ", YAxisVars, sep=""),  align="left")
}

if (YAxisTransform != "None") {
	HTML(paste("Y-axis transformation: ", YAxisTransform, sep=""),  align="left")
}

if (XAxisVar != "NULL") {
	HTML(paste("X-axis variable: ", XAxisVar, sep=""),  align="left")
}

if (XAxisTransform  != "None") {
	HTML(paste("X-axis transformation: ", XAxisTransform , sep=""),  align="left")
}

if (FirstCatFactor != "NULL") {
	HTML(paste("First categorisation factor: ", FirstCatFactor, sep=""),  align="left")
}

if (SecondCatFactor != "NULL") {
	HTML(paste("Second categorisation factor: ", SecondCatFactor, sep=""),  align="left")
}

if (GraphStyle != "NULL" && (FirstCatFactor != "NULL" || SecondCatFactor != "NULL")) {
	HTML(paste("Categorised gragh style: ", GraphStyle, sep=""),  align="left")
}

if (MainTitle != "NULL") {
	HTML(paste("User defined main title: ", MainTitle, sep=""),  align="left")
}

if (XAxisTitle != "NULL") {
	HTML(paste("X-axis variable title (except for Histogram): ", XAxisTitle, sep=""),  align="left")
}

if (YAxisTitle != "NULL") {
	HTML(paste("Response variable title: ", YAxisTitle, sep=""),  align="left")
}


if (DisplayLegend != "N" && (FirstCatFactor != "NULL" || SecondCatFactor != "NULL")) {
	HTML(paste("Display legend (Y/N): ", DisplayLegend, sep=""),  align="left")
}


if (ScatterPlot != "NULL") {
	HTML(paste("Generate Scatterplot (Y/N): ", ScatterPlot, sep=""),  align="left")
}

if (LinearFit != "N" && ScatterPlot != "NULL") {
	HTML(paste("Apply linear fit (Y/N): ", LinearFit, sep=""),  align="left")
}

if (jitterfunction != "N" && ScatterPlot != "NULL") {
	HTML(paste("Apply jitter function (Y/N): ", jitterfunction, sep=""),  align="left")
}


if (BoxPlot != "N") {
	HTML(paste("Generate Box-plot (Y/N): ", BoxPlot, sep=""),  align="left")
}

if (Outliers != "N" && BoxPlot != "N") {
	HTML(paste("Display outliers on Box-plot (Y/N): ", Outliers, sep=""),  align="left")
}

if (displaypointBOX != "NULL" &&BoxPlot != "N") {
	HTML(paste("Display points on Box-plot (Y/N): ", displaypointBOX, sep=""),  align="left")
}


if (SEMPlot != "N") {
	HTML(paste("Generate Means with SEM plot (Y/N): ", SEMPlot, sep=""),  align="left")
}

if (SEMPlotType != "NULL" && SEMPlot != "N") {
	HTML(paste("Means with SEM plot type: ", SEMPlotType, sep=""),  align="left")
}

if (displaypointSEM != "NULL" && SEMPlot != "N") {
	HTML(paste("Display points on Means with SEM plot (Y/N): ", displaypointSEM, sep=""),  align="left")
}

if (HistogramPlot != "N") {
	HTML(paste("Generate Histogram (Y/N): ", HistogramPlot, sep=""),  align="left")
}

if (NormalDistFit != "NULL" && HistogramPlot != "N") {
	HTML(paste("Display normal distribution on histogram (Y/N): ", NormalDistFit, sep=""),  align="left")
}

if (CaseProfilesPlot != "N") {
	HTML(paste("Generate Case Profiles plot (Y/N): ", CaseProfilesPlot, sep=""),  align="left")
}

if (CaseIDFactor != "NULL" && CaseProfilesPlot != "N") {
	HTML(paste("Case ID variable for Case profiles plot: ", CaseIDFactor, sep=""),  align="left")
}

if (ReferenceLine != "NULL" ) {
	HTML(paste("Reference line included on Case profiles plot: ", ReferenceLine, sep=""),  align="left")
}

