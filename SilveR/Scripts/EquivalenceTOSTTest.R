#===================================================================================================================
#R Libraries
suppressWarnings(library(multcomp))
suppressWarnings(library(multcompView))
suppressWarnings(library(car))
suppressWarnings(library(R2HTML))
suppressWarnings(library("emmeans"))

#===================================================================================================================
# retrieve args
Args <- commandArgs(TRUE)

#Read in data
statdata <- read.csv(Args[3], header=TRUE, sep=",")

#Copy Args
model <- Args[4]
scatterplotModel <- as.formula(Args[5])
covariates <- Args[6]
responseTransform <- tolower(Args[7])
covariateTransform <- tolower(Args[8])
FirstCatFactor <- Args[9]
treatFactors <- Args[10]
blockFactors <- Args[11]
EqBtype <- tolower(Args[12])
lowerboundtest <- as.numeric(Args[13])
upperboundtest <- as.numeric(Args[14])
lowerboundtestN <- Args[13]
upperboundtestN <- Args[14]
showPRPlot <- Args[15]
showNormPlot <- Args[16]
sig <- 1 - as.numeric(Args[17])
sig2 <- 1 - as.numeric(Args[17])/2
sigeq <- 1 - as.numeric(Args[17])*2
effectModel <- as.formula(Args[18])
effectModel2 <- Args[18]
selectedEffect <- Args[19]
showLSMeans <- Args[20]
backToControlTest <- tolower(Args[21])
cntrlGroup <- Args[22]


#source(paste(getwd(),"/Common_Functions.R", sep=""))
print(Args)
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
#V3.2 STB OCT2015
set.seed(5041975)

#STB 14OCT2015
#Set contrast options for Marginal overall tests
options(contrasts=c(unordered="contr.sum", ordered="contr.poly"))

#Graphics parameter setup
graphdata<-statdata
if (FirstCatFactor != "NULL") {
	Gr_palette<-palette_FUN(FirstCatFactor)
} 
Line_size2 <- Line_size
Labelz_IVS_ <- "N"
ReferenceLine <- "NULL"

#Control group
cntrlGroup<-sub("-", "_ivs_dash_ivs_", cntrlGroup, fixed=TRUE)
if (is.numeric(statdata$mainEffect) == TRUE) {
	cntrlGroup <- paste ("'",cntrlGroup,"'",sep="")
}

statdata$mainEffect<-as.factor(statdata$mainEffect)
statdata$scatterPlotColumn<-as.factor(statdata$scatterPlotColumn)

#Response
resp <- unlist(strsplit(model ,"~"))[1] #get the response variable from the main model

#Number of factors in Selected effect
factno<-length(unique (strsplit(selectedEffect, "*",fixed = TRUE)[[1]]))

#STB June 2015 - Taking a copies of the dataset
statdata_temp <-statdata


#calculating number of block factors
noblockfactors=0
if (blockFactors !="NULL") {
	tempblockChanges <-strsplit(blockFactors, ",")
	blocklistx <- c("")
	for(i in 1:length(tempblockChanges[[1]]))  {
		blocklistx [length(blocklistx )+1]=(tempblockChanges[[1]][i]) 
	}
	blocklist <- blocklistx[-1]
	noblockfactors<-length(blocklist)
}

#calculating number of treatment factors
tempChanges <-strsplit(treatFactors, ",")
treatlistx <- c("")
for(i in 1:length(tempChanges[[1]]))  { 
	treatlistx [length(treatlistx )+1]=(tempChanges[[1]][i]) 
}
treatlist <- treatlistx[-1]
notreatfactors<-length(treatlist)

#calculating number of covariates
nocovars=0
if (covariates !="NULL") {
	tempcovChanges <-strsplit(covariates, ",")
	txtexpectedcovChanges <- c("")
	for(i in 1:length(tempcovChanges[[1]]))  {
		txtexpectedcovChanges [length(txtexpectedcovChanges )+1]=(tempcovChanges[[1]][i]) 
	}
	covlist <- txtexpectedcovChanges[-1]
	nocovars<-length(covlist)
}

#Removing illegal characters
selectedEffect<- namereplace2(selectedEffect)
selectedEffectx<- namereplace(selectedEffect)

#replace illegal characters in variable names
YAxisTitle <-resp

if(FirstCatFactor != "NULL") {
	XAxisTitleCov<-covlist
}

#replace illegal characters in variable names
for (i in 1:10) {
	YAxisTitle<-namereplace(YAxisTitle)

	if(FirstCatFactor != "NULL") {
		for (i in 1: nocovars) {
			XAxisTitleCov[i]<-namereplace(XAxisTitleCov[i])
		}
	}
}

BTYAxisTitle <- YAxisTitle
#Add transformation to axis labels
if (responseTransform != "none") {
	YAxisTitle<-axis_relabel(responseTransform, YAxisTitle)
}

if(FirstCatFactor != "NULL") {
	for (i in 1: nocovars) {
		#Add transformation to axis labels
		if (covariateTransform != "none") {
			XAxisTitleCov[i]<-axis_relabel(covariateTransform, XAxisTitleCov[i])
		}
	}
}
LS_YAxisTitle<-YAxisTitle

# Code to create varibale to test if the highest order interaction is selected
testeffects = noblockfactors
if(FirstCatFactor != "NULL") {
	testeffects = noblockfactors+nocovars
}
emodel <-strsplit(effectModel2, "+", fixed = TRUE)

emodelChanges <- c("")
for(i in 1:length(emodel[[1]]))  { 
	emodelChanges [length(emodelChanges )+1]=(emodel[[1]][i]) 
}
noeffects<-length(emodelChanges)-2



#Defining the analysis type
if (lowerboundtestN != "NULL" && upperboundtestN != "NULL") {
	AnalysisType = "two-sided"
}
if (lowerboundtestN == "NULL" && upperboundtestN != "NULL") {
	AnalysisType = "upper-sided"
}
if (lowerboundtestN != "NULL" && upperboundtestN == "NULL") {
	AnalysisType = "lower-sided"
}

#Defining the percentage bounds
lower      <- lowerboundtest
upper      <- upperboundtest

if (responseTransform =="log10") {
	lower      <- log10(lowerboundtest)
	upper      <- log10(upperboundtest)
}

if (responseTransform =="loge") {
	lower      <- log(lowerboundtest)
	upper      <- log(upperboundtest)
}

if (EqBtype == "percentage" && responseTransform == "none") {
	if (backToControlTest == "allpairwise") {
		overallmean <- mean(eval(parse(text = paste("statdata$", resp))), na.rm = TRUE)
	}
	if (backToControlTest == "comparisonstocontrol") {
		controldata <-  subset(statdata, statdata$mainEffect == cntrlGroup)
		overallmean <- mean(eval(parse(text = paste("controldata$", resp))), na.rm = TRUE)
	}

	if (AnalysisType == "two-sided") {
		lower <-  -1*overallmean*(1-lowerboundtest)
		upper <- overallmean*(upperboundtest-1)
		lowerboundtest <- lower
		upperboundtest <- upper
	}
	if (AnalysisType == "lower-sided") {
		lower <- -1*overallmean*(1-lowerboundtest)
		lowerboundtest <- lower
	}
	if (AnalysisType == "upper-sided") {
		upper <- overallmean*(upperboundtest-1)
		upperboundtest <- upper
	}
}



#===================================================================================================================
#Titles and description
#===================================================================================================================
#Output HTML header
if (AnalysisType == "two-sided") {
	Title <-paste(branding, " Equivalence TOST Test Analysis", sep="")
} else {
	Title <-paste(branding, " Equivalence Test Analysis", sep="")
}
HTML.title(Title, HR = 1, align = "left")

#Software developement version warning
if (Betawarn == "Y") {
	HTML.title("Warning", HR=2, align="left")
	HTML(BetaMessage, align="left")
}

#===================================================================================================================
# Testing the factorial combinations
ind<-1
for (i in 1:notreatfactors) {
	ind=ind*length(unique(eval(parse(text = paste("statdata$",treatlist[i])))))
}

if((length(unique(statdata$scatterPlotColumn))) != ind) {
	HTML("Unfortunately not all combinations of the levels of the treatment factors are present in the experimental design. We recommend you manually create a new factor corresponding to the combinations of the levels of the treatment factors.", align="left")
	quit()
}
#===================================================================================================================

title<-c("Response")
if(FirstCatFactor != "NULL") {
	title<-paste(title, ", covariate", sep="")
}
if (AnalysisType == "two-sided") {
	title<-paste(title, " and equivalence bounds", sep="")
} else {
	title<-paste(title, " and equivalence bound", sep="")
}
HTML.title(title, HR=2, align="left")

add<-paste(c("The  "), resp, " response is currently being analysed by the Equivalence (TOST) test Analysis module", sep="")

if(FirstCatFactor != "NULL") {
	if (nocovars == 1) {
		add<-paste(add, ", with ", covlist[1] , " fitted as a covariate.", sep="")
	} 
	if (nocovars == 2) {
		add<-paste(add, ", with ", covlist[1] , " and ", covlist[2] ," fitted as covariates.", sep="")
	}
	if (nocovars > 2) {	
		add<-paste(add, ", with ", sep="")	
		for (i in 1: (nocovars -2)) {
		add <- paste (add, covlist[i],  ", " , sep="")
		}
		add<-paste(add, covlist[(nocovars -1)] , " and ", covlist[nocovars] , " fitted as covariates.", sep="")
	}
} else {
	add<-paste(add, ".", sep="")
}

if (responseTransform != "none") {
	add<-paste(add, c("The response has been "), responseTransform, " transformed prior to analysis.", sep="")
}

if (covariates !="NULL" && covariateTransform != "none") {
	if (nocovars == 1) {
		add<-paste(add, c("The covariate has been "), covariateTransform, " transformed prior to analysis.", sep="")
	} else {
		add<-paste(add, c("The covariates have been "), covariateTransform, " transformed prior to analysis.", sep="")
	}
}
HTML(add, align="left")

low<-format(round(lowerboundtest, 2), nsmall=2, scientific=FALSE)
up<- format(round(upperboundtest, 2), nsmall=2, scientific=FALSE)


#Generating the text for the eq bounds
if (EqBtype == "absolute") {
	if (AnalysisType == "two-sided") {
		addEB<-paste("The lower equivalence bound is defined as ", low , " and the upper equivalence bound is defined as ", up , ". As both boundaries are defined a two one-sided (TOST) equivalence test has been performed.",   sep="")
	}
	if (AnalysisType == "lower-sided") {
		addEB<-paste("The lower equivalence bound is defined as ", low ,  ". As only a lower bound has been defined a one-sided equivalence test has been performed.",   sep="")
	}
	if (AnalysisType == "upper-sided") {
		addEB<-paste("The upper equivalence bound is defined as ", up,  ". As only an upper bound has been defined a one-sided equivalence test has been performed.",   sep="")
	}
}

if (responseTransform == "log10" || responseTransform == "loge") {
	if (AnalysisType == "two-sided") {
		addEB<-paste("The lower equivalence bound is defined as a ", low, " fold change. The upper equivalence bound is defined as ", up , " fold change. As both boundaries are defined a two one-sided (TOST) equivalence test has been performed.",   sep="")
	}
	if (AnalysisType == "lower-sided") {
		addEB<-paste("The lower equivalence bound is defined as a ", low, " fold change. As only a lower bound has been defined a one-sided equivalence test has been performed.",   sep="")
	}
	if (AnalysisType == "upper-sided") {
		addEB<-paste("The upper equivalence bound is defined as a ", up, " fold change. As only an upper bound has been defined a one-sided equivalence test has been performed.",   sep="")
	}
}

if (EqBtype == "percentage" && (responseTransform != "log10" && responseTransform != "loge") && backToControlTest == "allpairwise") {
	if (AnalysisType == "two-sided") {
		addEB<-paste("The lower equivalence bound is defined as a ", lowerboundtestN, " fold change (from the overall response mean). This is equivalent to an absolute difference of size ", low , ". The upper equivalence bound is defined as ", upperboundtestN , " fold change (from the overall response mean). This is equivalent to an absolute difference of size ", up , ". As both boundaries are defined a two one-sided (TOST) equivalence test has been performed.",   sep="")
	}
	if (AnalysisType == "lower-sided") {
		addEB<-paste("The lower equivalence bound is defined as a ", lowerboundtestN, " fold change (from the overall response mean). This is equivalent to an absolute difference of size ", low , ". As only a lower bound has been defined a one-sided equivalence test has been performed.",   sep="")
	}
	if (AnalysisType == "upper-sided") {
		addEB<-paste("The upper equivalence bound is defined as a ", upperboundtestN, " fold change (from the overall response mean). This is equivalent to an absolute difference of size ", up , ". As only an upper bound has been defined a one-sided equivalence test has been performed.",   sep="")
	}
}

if (EqBtype == "percentage" && (responseTransform != "log10" && responseTransform != "loge") && backToControlTest == "comparisonstocontrol") {
	if (AnalysisType == "two-sided") {
		addEB<-paste("The lower equivalence bound is defined as a ", lowerboundtestN, " fold change (from the control group mean). This is equivalent to an absolute difference of size ", low , ". The upper equivalence bound is defined as ", upperboundtestN , " fold change (from the control group mean). This is equivalent to an absolute difference of size ", up , ". As both boundaries are defined a two one-sided (TOST) equivalence test has been performed.",   sep="")
	}
	if (AnalysisType == "lower-sided") {
		addEB<-paste("The lower equivalence bound is defined as a ", lowerboundtestN, " fold change (from the control group mean). This is equivalent to an absolute difference of size ", low , ". As only a lower bound has been defined a one-sided equivalence test has been performed.",   sep="")
	}
	if (AnalysisType == "upper-sided") {
		addEB<-paste("The upper equivalence bound is defined as a ", upperboundtestN, " fold change (from the control group mean). This is equivalent to an absolute difference of size ", up , ". As only an upper bound has been defined a one-sided equivalence test has been performed.",   sep="")
	}
}
HTML(addEB, align="left")

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
plotFilepdf1 <- sub(".html", "scatterPlot.pdf", htmlFile)
dev.control("enable") 

#Graphical parameters
graphdata<-statdata
graphdata$yvarrr_IVS <- eval(parse(text = paste("statdata$",resp)))
graphdata$xvarrr_IVS <-statdata$scatterPlotColumn
XAxisTitle <- ""
MainTitle2 <- ""
w_Gr_jitscat <- 0
h_Gr_jitscat <-  0
infiniteslope <- "Y"

#GGPLOT2 code
NONCAT_SCAT("SMPA_PLOT")

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
HTML("Tip: Use this plot to identify possible outliers.", align="left")

#===================================================================================================================
#Covariate plot
#===================================================================================================================
if(FirstCatFactor != "NULL") {

	if (nocovars == 1) {
		title<-c("Plot of the response vs. the covariate, categorised by the primary factor")
	} else {
		title<-c("Plot of the response vs. the covariates, categorised by the primary factor")
	}
	if(responseTransform != "none" || covariateTransform != "none") {
		title<-paste(title, " (on the transformed scale)", sep="")
	} 
	HTML.title(title, HR=2, align="left")

	index <- 1
	for (i in 1:nocovars) {
		ncscatterplot3 <- sub(".html", "IVS", htmlFile)
	    	ncscatterplot3 <- paste(ncscatterplot3, index, "ncscatterplot3.png", sep = "")
		png(ncscatterplot3,width = jpegwidth, height = jpegheight, units="in", res=PlotResolution)

		#STB July2013
		plotFilepdf2 <- sub(".html", "IVS", htmlFile)
		plotFilepdf2 <- paste(plotFilepdf2, index, "ncscatterplot3.pdf", sep="")
		dev.control("enable") 

		#Graphical parameters
		graphdata<-statdata
		graphdata$xvarrr_IVS <- eval(parse(text = paste("statdata$",covlist[i])))
		graphdata$yvarrr_IVS <- eval(parse(text = paste("statdata$",resp)))
		graphdata$l_l <- eval(parse(text = paste("statdata$",FirstCatFactor))) 
		graphdata$catfact <-eval(parse(text = paste("statdata$",FirstCatFactor))) 
		XAxisTitle <- XAxisTitleCov[i]
		MainTitle2 <-""
		w_Gr_jitscat <- 0
		h_Gr_jitscat <- 0
		Legendpos <- "right"
		Gr_alpha <- 1
		Line_type <-Line_type_solid

		LinearFit <- "Y"
		GraphStyle <- "Overlaid"
		ScatterPlot <- "Y"

		#Testing for with infinite slopes on scatterplot and re-ordering dataset if necessary
		inf_slope<-IVS_F_infinite_slope()
		infiniteslope <- inf_slope$infiniteslope
		graphdata<-inf_slope$graphdata
		graphdatax <- subset(graphdata, catvartest != "N")
		graphdata<-graphdatax

		#GGPLOT2 code
		OVERLAID_SCAT()
	
		void<-HTMLInsertGraph(GraphFileName=sub("[A-Z0-9a-z,:,\\\\]*App_Data[\\\\]","", ncscatterplot3), Align="centre")

		#STB July2013
		if (pdfout=="Y") {
			pdf(file=sub("[A-Z0-9a-z,:,\\\\]*App_Data[\\\\]","", plotFilepdf2), height = pdfheight, width = pdfwidth) 
			dev.set(2) 
			dev.copy(which=3) 
			dev.off(2)
			dev.off(3)
			pdfFile_2<-sub("[A-Z0-9a-z,:,\\\\]*App_Data[\\\\]","",plotFilepdf2)
			linkToPdf2 <- paste ("<a href=\"",pdfFile_2,"\">Click here to view the PDF of the covariate plot</a>", sep = "")
			HTML(linkToPdf2)
		}
		index <- index +1
	}

	#STB Aug 2011 - removing lines with infinite slope
	if (infiniteslope != "N") {
		title<-paste("Warning: The covariate has the same value for all subjects in one or more levels of the ", FirstCatFactor, " factor. Care should be taken if you want to include this covariate in the analysis.", sep="")
		HTML(title, align="left")
	}
	HTML("Tip: In order to decide whether it is helpful to fit the covariate, the following should be considered:", align="left")
	HTML("a) Is there a relationship between the response and the covariate? (N.B., It is only worth fitting the covariate if there is a strong positive (or negative) relationship between them: i.e., the lines on the plot should not be horizontal).", align="left")
	HTML("b) Is the relationship similar for all treatments? (The lines on the plot should be approximately parallel).", align="left")
	HTML("c) Is the covariate influenced by the treatment? (We assume the covariate is not influenced by the treatment and so there should be no separation of the treatment groups along the x-axis on the plot).", align="left")
	HTML("These issues are discussed in more detail in Morris (1999).", align="left")
}

#===================================================================================================================
#building the covariate interaction model
#===================================================================================================================
if (AssessCovariateInteractions == "Y" && FirstCatFactor != "NULL") {

	#Creating the list of model terms
	CovIntModel <- c(model)

	#Adding in additional interactions
	for (i in 1:notreatfactors) {
		for (j in 1: nocovars) {
			CovIntModel <- paste(CovIntModel, " + ",  treatlist[i], " * ", covlist[j], sep="")
		}
	}

	#Performing the ANCOVA analysis
	Covintfull<-lm(as.formula(CovIntModel), data=statdata, na.action = na.omit)

	#Title + warning
	HTML.title("Analysis of Covariance (ANCOVA) table for assessing covariate interactions", HR=2, align="left")

	#Printing ANCOVA Table - note this code is reused from below - Type III SS included (SMPA only, IFPA uses Type I)
	if (df.residual(Covintfull)<1) {
		HTML("The covariate interactions have not been calculated as there are zero residual degrees of freedom when all terms are included in the statistical model." , align="left")
	} 

	# Stop process if residual sums of squares are too close to zero
	if (deviance(Covintfull)<sqrt(.Machine$double.eps)) {
		HTML("The Residual Sums of Squares is close to zero indicating the model is overfitted (too many terms are included in the model). Either the model should be simplified, to reduce the number of terms in the model, or the output option to test covariate interactions should be deselected." , align="left")
		quit()
	}

	if (df.residual(Covintfull)>0) {
		tempx<-Anova(Covintfull, type=c("III"))[-1,]

		if (tempx[dim(tempx)[1],1] != 0) {
			temp2x<-(tempx)
			col1x<-format(round(temp2x[1], 3), nsmall=3, scientific=FALSE)
			col2x<-format(round(temp2x[1]/temp2x[2], 3), nsmall=3, scientific=FALSE)
			col3x<-format(round(temp2x[3], 2), nsmall=2, scientific=FALSE)
			col4x<-format(round(temp2x[4], 4), nsmall=4, scientific=FALSE)
			sourcex<-rownames(temp2x)

			# Residual label in ANOVA
			sourcex[length(sourcex)] <- "Residual"

			#STB March 2014 - Replacing : with * in ANOVA table
			for (q in 1:notreatfactors) {
				sourcex<-sub(":"," * ", sourcex) 
			}
			ivsanovax<-cbind(sourcex, col1x, temp2x[2], col2x, col3x, col4x)

			ivsanovax[length(unique(sourcex)),5]<-" "
			ivsanovax[length(unique(sourcex)),6]<-" "

			#STB May 2012 capitals changed
			headx<-c("Effect", "Sums of squares", "Degrees of freedom","Mean square","F-value","p-value")
			colnames(ivsanovax)<-headx
			for (i in 1:(dim(ivsanovax)[1]-1))  {
				if (temp2x[i,4]<0.0001) {
					#STB March 2011 formatting p-values p<0.0001
					# ivsanovax[i,6]<-0.0001
					ivsanovax[i,6]=format(round(0.0001, 4), nsmall=4, scientific=FALSE)
					ivsanovax[i,6]<- paste("<",ivsanovax[i,6])
				}
			}
			HTML(ivsanovax, classfirstline="second", align="left", row.names = "FALSE")
			HTML("Note: This table should only be used to assess the covariate interactions. The statistical model used to generate all the remaining results in this output does not include the covariate interactions.", align="left")
			HTML("Comment: ANCOVA table calculated using a Type III model fit, see Armitage et al. (2001).", align="left")
		} 
	}
}
#===================================================================================================================
#ANOVA table
#===================================================================================================================
#Analysis call
threewayfull<-lm(model, data=statdata, na.action = na.omit)

#Testing the degrees of freedom

if (df.residual(threewayfull)<5) {
	HTML.title("Warning", HR=2, align="left")
	HTML("Unfortunately the residual degrees of freedom are low (less than 5). This may make the estimation of the underlying variability, and hence the results of the statistical tests, unreliable. This can be caused by attempting to fit too many factors, and their interactions, in the statistical model. Where appropriate we recommend you fit some of the 'Treatment' factors as 'Other design' factors. This will remove their interactions from the statistical model and therefore increase the residual degrees of freedom.", align="left")
}

#ANOVA Table in here

#===================================================================================================================
#Fixed effect coefficients
#===================================================================================================================
if (ShowCoeff == "Y") {
	HTML.title("Model coefficients", HR=2, align="left")
	threewayfull<-lm(model, data=statdata, na.action = na.omit)
	coeffs<- summary(threewayfull)$coefficient

	col1<-format(round(coeffs[,1], 4), nsmall=4, scientific=FALSE)
	col2<-format(round(coeffs[,2], 4), nsmall=4, scientific=FALSE)
	col3<-format(round(coeffs[,3], 2), nsmall=2, scientific=FALSE)
	col4<-format(round(coeffs[,4], 4), nsmall=4, scientific=FALSE)
	coeffsx <- cbind(col1, col2, col3, col4)
	colnames(coeffsx)<-c("Estimate", "Std. Error", "t-value", "p-value")
	for (i in 1:(dim(coeffsx)[1])) {
		if (coeffs[i,4]<0.0001) {
			coeffsx[i,4]=format(round(0.0001, 4), nsmall=4, scientific=FALSE)
			coeffsx[i,4]<- paste("<",coeffsx[i,4])
		}
	}
	HTML(coeffsx, classfirstline="second", align="left", row.names = "FALSE")
}

#===================================================================================================================
#Covariate correlation table
#===================================================================================================================
if (CovariateRegressionCoefficients == "Y" && FirstCatFactor != "NULL") {

	if (nocovars == 1) {
		HTML.title("Covariate regression coefficient", HR=2, align="left")
	} else {
		HTML.title("Covariate regression coefficients", HR=2, align="left")
	}
	covtable_1<-coef(summary(threewayfull))
	covtable<-data.frame(covtable_1)[c(2:(nocovars+1)),]

	names <- rownames(covtable)
	Estimate <-format(round(covtable$Estimate, 3), nsmall=3, scientific=FALSE) 
	StdError <-format(round(covtable$Std..Error, 3), nsmall=3, scientific=FALSE) 
	tvalue <-format(round(covtable$t.value, 2), nsmall=2, scientific=FALSE) 
	Prt <-format(round(covtable$Pr...t.., 4), nsmall=4, scientific=FALSE) 
	
	covtable2 <-cbind(names, Estimate, StdError, tvalue, Prt)

	for (k in 1:(dim(covtable2)[1])) {
		if (as.numeric(covtable[k,4])<0.0001)  {
			#STB March 2011 formatting p-values p<0.0001
			#ivsanova[i,9]<-0.0001
			covtable2[k,4]= "<0.0001"
		}
	}	
	colnames(covtable2)<-c("Covariate", "Estimate", "Std error", "t-value", "p-value")
	HTML(covtable2, classfirstline="second", align="left", row.names = "FALSE")
}



#===================================================================================================================
#LS Means plot and table
#===================================================================================================================
if(showLSMeans =="Y") {
	if ( responseTransform != "log10" && responseTransform != "loge")  {

		#Calculate LS Means dataset
		tabs<-emmeans(threewayfull,eval(parse(text = paste("~",selectedEffect))), data=statdata)
		x<-summary(tabs)
	
		x$Mean <-x$emmean 
		for (i in 1:dim(x)[1]) {
			x$Lower[i] <- x$emmean[i]  - x$SE[i]*qt(sig2, x$df[i])
			x$Upper[i] <- x$emmean[i]  + x$SE[i]*qt(sig2, x$df[i])
		}
		graphdata<-subset(x, select = -c(SE, df,emmean, lower.CL, upper.CL )) 

		names <- c()
		for (l in 1:factno) {
			names[l] <- paste(unique (strsplit(selectedEffect, "*",fixed = TRUE)[[1]])[l], " Level", sep = "")
		}
		names[factno+1]<-"Mean"
		names[factno+2]<-"Lower"
		names[factno+3]<-"Upper"
		colnames(graphdata)<-names
	
#===================================================================================================================
#Table of Least Square means
#===================================================================================================================
		#STB May 2012 Updating "least square (predicted) means"
		CITitle2<-paste("Table of the least square (predicted) means with ",(sig*100),"% confidence intervals",sep="")
		HTML.title(CITitle2, HR=2, align="left")

		#Calculate LS Means Table
		x<-summary(tabs)

		x$Mean <-format(round(x$emmean, 3), nsmall=3, scientific=FALSE) 
		for (i in 1:dim(x)[1]) {
			x$Lower[i] <- format(round(x$emmean[i]  - x$SE[i]*qt(sig2, x$df[i]), 3), nsmall=3, scientific=FALSE) 
			x$Upper[i] <- format(round(x$emmean[i]  + x$SE[i]*qt(sig2, x$df[i]), 3), nsmall=3, scientific=FALSE) 
		}

		names <- c("")
		for (l in 1:factno) {
			names[l+1] <- paste(unique (strsplit(selectedEffect, "*",fixed = TRUE)[[1]])[l], " ", sep = "")
		}

		x2<-subset(x, select = -c(SE, df,emmean, lower.CL, upper.CL )) 

		observ <- data.frame(c(1:dim(x)[1]))
		x2 <- cbind(observ, x2)	

		names[1]<-"Mean ID"
		names[factno+2]<-"Mean"
		names[factno+3]<-paste("Lower ",(sig*100),"% CI",sep="")
		names[factno+4]<-paste("Upper ",(sig*100),"% CI",sep="")
	
		colnames(x2)<-names
		HTML(x2, classfirstline="second", align="left", row.names = "FALSE")

#===================================================================================================================
#LS Means plot code
#===================================================================================================================
		#STB May 2012 Updating "least square (predicted) means"
		CITitle<-paste("Plot of the least square (predicted) means with ",(sig*100),"% confidence intervals",sep="")
		HTML.title(CITitle, HR=2, align="left")

		#Calculating dataset for plotting - including a Group factor for higher order interactions
		graphdata$Group_IVSq_ <- graphdata[,1]
	
		if (factno > 1) {
			for (y in 2:factno) {
				graphdata$Group_IVSq_ <- paste(graphdata$Group_IVSq_, ", ", graphdata[,y], sep = "") 
			}
		}	
	
		#other parameters for the plot
		Gr_alpha <- 0
		if (bandw != "N") {
			Gr_fill <- BW_fill
		} else {
			Gr_fill <- Col_fill
		}
		YAxisTitle <- LS_YAxisTitle
		XAxisTitle <- "Group"
		MainTitle2 <- ""
		Line_size <- Line_size2
	
		#Code for LS MEans plot
		meanPlot <- sub(".html", "meanplot.png", htmlFile)
		png(meanPlot,width = jpegwidth, height = jpegheight, units="in", res=PlotResolution)
	
		#STB July2013
		plotFilepdf5 <- sub(".html", "meanplot.pdf", htmlFile)
		dev.control("enable") 
	
		#GGPLOT2 code
		if (factno == 1 || factno > 4) {
			XAxisTitle <- unique (strsplit(selectedEffectx, "*",fixed = TRUE)[[1]])[1]
			LSMPLOT_1()
		}
	
		if (factno == 2) {
			if (length(unique(levels(graphdata[,1]))) > length(unique(levels(graphdata[,2])))) {
				XAxisTitle <- unique (strsplit(selectedEffectx, "*",fixed = TRUE)[[1]])[1]
				graphdata$jj_1 <- graphdata[,1]
				legendz <- unique (strsplit(selectedEffectx, "*",fixed = TRUE)[[1]])[2]
				graphdata$catzz<-legendz
				graphdata$jj_2 <- paste(graphdata$catzz, "= ",graphdata[,2], sep = "") 
	
			} else {
				XAxisTitle <- unique (strsplit(selectedEffectx, "*",fixed = TRUE)[[1]])[2]
				graphdata$jj_1 <- graphdata[,2]
				legendz <- unique (strsplit(selectedEffectx, "*",fixed = TRUE)[[1]])[1]
				graphdata$catzz<-legendz
				graphdata$jj_2 <- paste(graphdata$catzz, "= ",graphdata[,1], sep = "") 
			}
			Gr_palette<- palette_FUN("jj_2")
			LSMPLOT_2("none")
		}
	
		if (factno == 3) {
			if (length(unique(levels(graphdata[,1]))) > length(unique(levels(graphdata[,2]))) &&  length(unique(levels(graphdata[,1]))) > length(unique(levels(graphdata[,3]))  )) {
				XAxisTitle <- unique (strsplit(selectedEffectx, "*",fixed = TRUE)[[1]])[1]
				graphdata$jj_1 <- graphdata[,1]
				legendz <- unique (strsplit(selectedEffectx, "*",fixed = TRUE)[[1]])[2]
				graphdata$catzz<-legendz
				graphdata$jj_2 <- paste(graphdata$catzz, "= ",graphdata[,2], sep = "") 
				legendz <- unique (strsplit(selectedEffectx, "*",fixed = TRUE)[[1]])[3]
				graphdata$catzz<-legendz
				graphdata$jj_3 <- paste(graphdata$catzz, "= ",graphdata[,3], sep = "") 
	
			} else  if (length(unique(levels(graphdata[,2]))) > length(unique(levels(graphdata[,1]))) &&  length(unique(levels(graphdata[,2]))) > length(unique(levels(graphdata[,3])))  ) {
					XAxisTitle <- unique (strsplit(selectedEffectx, "*",fixed = TRUE)[[1]])[2]
					graphdata$jj_1 <- graphdata[,2]
					legendz <- unique (strsplit(selectedEffectx, "*",fixed = TRUE)[[1]])[1]
					graphdata$catzz<-legendz
					graphdata$jj_2 <- paste(graphdata$catzz, "= ",graphdata[,1], sep = "") 
					legendz <- unique (strsplit(selectedEffectx, "*",fixed = TRUE)[[1]])[3]
					graphdata$catzz<-legendz
					graphdata$jj_3 <- paste(graphdata$catzz, "= ",graphdata[,3], sep = "") 
	
				} else {
					XAxisTitle <- unique (strsplit(selectedEffectx, "*",fixed = TRUE)[[1]])[3]
					graphdata$jj_1 <- graphdata[,3]
					legendz <- unique (strsplit(selectedEffectx, "*",fixed = TRUE)[[1]])[1]
					graphdata$catzz<-legendz
					graphdata$jj_2 <- paste(graphdata$catzz, "= ",graphdata[,1], sep = "") 
					legendz <- unique (strsplit(selectedEffectx, "*",fixed = TRUE)[[1]])[2]
					graphdata$catzz<-legendz
					graphdata$jj_3 <- paste(graphdata$catzz, "= ",graphdata[,2], sep = "") 
				}
			Gr_palette<- palette_FUN("jj_2")
			LSMPLOT_2("three")
		}
	
		if (factno == 4) {
			if (length(unique(levels(graphdata[,1]))) > length(unique(levels(graphdata[,2]))) &&  length(unique(levels(graphdata[,1]))) > length(unique(levels(graphdata[,3]))) &&  length(unique(levels(graphdata[,1]))) > length(unique(levels(graphdata[,4])))) {
				XAxisTitle <- unique (strsplit(selectedEffectx, "*",fixed = TRUE)[[1]])[1]
				graphdata$jj_1 <- graphdata[,1]
				legendz <- unique (strsplit(selectedEffectx, "*",fixed = TRUE)[[1]])[2]
				graphdata$catzz<-legendz
				graphdata$jj_2 <- paste(graphdata$catzz, "= ",graphdata[,2], sep = "") 
				legendz <- unique (strsplit(selectedEffectx, "*",fixed = TRUE)[[1]])[3]
				graphdata$catzz<-legendz
				graphdata$jj_3 <- paste(graphdata$catzz, "= ",graphdata[,3], sep = "") 
				legendz <- unique (strsplit(selectedEffectx, "*",fixed = TRUE)[[1]])[4]
				graphdata$catzz<-legendz
				graphdata$jj_4 <- paste(graphdata$catzz, "= ",graphdata[,4], sep = "") 
	
			} else	if (length(unique(levels(graphdata[,2]))) > length(unique(levels(graphdata[,1]))) &&  length(unique(levels(graphdata[,2]))) > length(unique(levels(graphdata[,3]))) &&  length(unique(levels(graphdata[,2]))) > length(unique(levels(graphdata[,4])))) {
					XAxisTitle <- unique (strsplit(selectedEffectx, "*",fixed = TRUE)[[1]])[2]
					graphdata$jj_1 <- graphdata[,2]
					legendz <- unique (strsplit(selectedEffectx, "*",fixed = TRUE)[[1]])[1]
					graphdata$catzz<-legendz
					graphdata$jj_2 <- paste(graphdata$catzz, "= ",graphdata[,1], sep = "") 
					legendz <- unique (strsplit(selectedEffectx, "*",fixed = TRUE)[[1]])[3]
					graphdata$catzz<-legendz
					graphdata$jj_3 <- paste(graphdata$catzz, "= ",graphdata[,3], sep = "") 
					legendz <- unique (strsplit(selectedEffectx, "*",fixed = TRUE)[[1]])[4]
					graphdata$catzz<-legendz
					graphdata$jj_4 <- paste(graphdata$catzz, "= ",graphdata[,4], sep = "") 
	
				} else 	if (length(unique(levels(graphdata[,3]))) > length(unique(levels(graphdata[,1]))) &&  length(unique(levels(graphdata[,3]))) > length(unique(levels(graphdata[,2]))) &&  length(unique(levels(graphdata[,3]))) > length(unique(levels(graphdata[,4])))) {
						XAxisTitle <- unique (strsplit(selectedEffectx, "*",fixed = TRUE)[[1]])[3]
						graphdata$jj_1 <- graphdata[,3]
						legendz <- unique (strsplit(selectedEffectx, "*",fixed = TRUE)[[1]])[1]
						graphdata$catzz<-legendz
						graphdata$jj_2 <- paste(graphdata$catzz, "= ",graphdata[,1], sep = "") 
						legendz <- unique (strsplit(selectedEffectx, "*",fixed = TRUE)[[1]])[2]
						graphdata$catzz<-legendz
						graphdata$jj_3 <- paste(graphdata$catzz, "= ",graphdata[,2], sep = "") 
						legendz <- unique (strsplit(selectedEffectx, "*",fixed = TRUE)[[1]])[4]
						graphdata$catzz<-legendz
						graphdata$jj_4 <- paste(graphdata$catzz, "= ",graphdata[,4], sep = "") 
	
					} else  {
						XAxisTitle <- unique (strsplit(selectedEffectx, "*",fixed = TRUE)[[1]])[4]
						graphdata$jj_1 <- graphdata[,4]
						legendz <- unique (strsplit(selectedEffectx, "*",fixed = TRUE)[[1]])[1]
						graphdata$catzz<-legendz
						graphdata$jj_2 <- paste(graphdata$catzz, "= ",graphdata[,1], sep = "") 
						legendz <- unique (strsplit(selectedEffectx, "*",fixed = TRUE)[[1]])[2]
						graphdata$catzz<-legendz 
						graphdata$jj_3 <- paste(graphdata$catzz, "= ",graphdata[,2], sep = "") 
						legendz <- unique (strsplit(selectedEffectx, "*",fixed = TRUE)[[1]])[3]
						graphdata$catzz<-legendz
						graphdata$jj_4 <- paste(graphdata$catzz, "= ",graphdata[,3], sep = "") 
					} 
				Gr_palette<- palette_FUN("jj_2")
				LSMPLOT_2("four")
		}
	
		void<-HTMLInsertGraph(GraphFileName=sub("[A-Z0-9a-z,:,\\\\]*App_Data[\\\\]","", meanPlot), Align="left")
	
		#STB July2013
		if (pdfout=="Y") {
			pdf(file=sub("[A-Z0-9a-z,:,\\\\]*App_Data[\\\\]","", plotFilepdf5), height = pdfheight, width = pdfwidth) 
			dev.set(2) 
			dev.copy(which=3) 
			dev.off(2)
			dev.off(3)
			pdfFile_5<-sub("[A-Z0-9a-z,:,\\\\]*App_Data[\\\\]","",plotFilepdf5)
			linkToPdf5 <- paste ("<a href=\"",pdfFile_5,"\">Click here to view the PDF of the plot of least square (predicted) means</a>", sep = "")
			HTML(linkToPdf5)
		}
	}
}

#===================================================================================================================
#Back transformed geometric means plot and table 
#===================================================================================================================
if(showLSMeans =="Y") {
	if ( (responseTransform == "log10") || (responseTransform == "loge") ) {

		#Calculate LS Means dataset
		tabs<-emmeans(threewayfull,eval(parse(text = paste("~",selectedEffect))), data=statdata)
		x<-summary(tabs)

		if (responseTransform =="log10") {
			x$Mean <-10^(x$emmean)
			for (i in 1:dim(x)[1]) {
				x$Lower[i] <- 10^(x$emmean[i]  - x$SE[i]*qt(sig2, x$df[i]))
				x$Upper[i] <- 10^(x$emmean[i]  + x$SE[i]*qt(sig2, x$df[i]))
			}
		}
	
		if (responseTransform =="loge") {
			x$Mean <-exp(x$emmean)
			for (i in 1:dim(x)[1]) {
				x$Lower[i] <- exp(x$emmean[i]  - x$SE[i]*qt(sig2, x$df[i]))
				x$Upper[i] <- exp(x$emmean[i]  + x$SE[i]*qt(sig2, x$df[i]))
			}
		}
		graphdata<-subset(x, select = -c(SE, df,emmean, lower.CL, upper.CL )) 
	
		names <- c()
		for (l in 1:factno) {
			names[l] <- paste(unique (strsplit(selectedEffect, "*",fixed = TRUE)[[1]])[l], " Level", sep = "")
		}
		names[factno+1]<-"Mean"
		names[factno+2]<-"Lower"
		names[factno+3]<-"Upper"
	
		colnames(graphdata)<-names


#===================================================================================================================
#Back transformed geometric table 
#===================================================================================================================
		#STB May 2012 Updating "least square (predicted) means"
		CITitle2<-paste("Table of the back-transformed geometric means with ",(sig*100),"% confidence intervals",sep="")
		HTML.title(CITitle2, HR=2, align="left")

		if (GeomDisplay == "geometricmeansandpredictedmeansonlogscale") {
			HTML("As the response was log transformed prior to analysis the least square (predicted) means are presented on the log scale. These results can be back transformed onto the original scale. These are known as the back-transformed geometric means.", align="left")
		}
		if (GeomDisplay == "geometricmeansonly") {
			HTML("As the response was log transformed prior to analysis the least square (predicted) means are presented back transformed onto the original scale. These are known as the back-transformed geometric means.", align="left")
		}

		#Calculate LS Means Table
		x<-summary(tabs)

		if (responseTransform =="log10") {
			x$Mean <-format(round(10^(x$emmean), 3), nsmall=3, scientific=FALSE) 
			for (i in 1:dim(x)[1]) {
				x$Lower[i] <- format(round(10^(x$emmean[i]  - x$SE[i]*qt(sig2, x$df[i])), 3), nsmall=3, scientific=FALSE)  
				x$Upper[i] <- format(round(10^(x$emmean[i]  + x$SE[i]*qt(sig2, x$df[i])), 3), nsmall=3, scientific=FALSE)  
			}
		}
	
		if (responseTransform =="loge") {
			x$Mean <-format(round(exp(x$emmean), 3), nsmall=3, scientific=FALSE) 
			for (i in 1:dim(x)[1]) {
				x$Lower[i] <- format(round(exp(x$emmean[i]  - x$SE[i]*qt(sig2, x$df[i])), 3), nsmall=3, scientific=FALSE)  
				x$Upper[i] <- format(round(exp(x$emmean[i]  + x$SE[i]*qt(sig2, x$df[i])), 3), nsmall=3, scientific=FALSE)  
			}
		}
	
		names <- c("")
		for (l in 1:factno)
		{
			names[l+1] <- paste(unique (strsplit(selectedEffect, "*",fixed = TRUE)[[1]])[l], " ", sep = "")
		}
	
		x2<-subset(x, select = -c(SE, df,emmean, lower.CL, upper.CL )) 
	
		observ <- data.frame(c(1:dim(x)[1]))
		x2 <- cbind(observ, x2)
	
		names[1]<-"Mean ID"
		names[factno+2]<-"Geometric mean"
		names[factno+3]<-paste("Lower ",(sig*100),"% CI",sep="")
		names[factno+4]<-paste("Upper ",(sig*100),"% CI",sep="")
	
		colnames(x2)<-names
		HTML(x2, classfirstline="second", align="left", row.names = "FALSE")
#===================================================================================================================
#Geometric means plot
#===================================================================================================================

		CITitle<-paste("Plot of the back-transformed geometric means with ",(sig*100),"% confidence intervals",sep="")
		HTML.title(CITitle, HR=2, align="left")
	
		#Calculating dataset for plotting - including a Group factor for higher order interactions
		graphdata$Group_IVSq_ <- graphdata[,1]
	
		if (factno > 1) {
			for (y in 2:factno) {
				graphdata$Group_IVSq_ <- paste(graphdata$Group_IVSq_, ", ", graphdata[,y], sep = "") 
			}
		}	
	
		#other parameters for the plot
		Gr_alpha <- 0
		if (bandw != "N") {
			Gr_fill <- BW_fill
		} else {
			Gr_fill <- Col_fill
		}
		YAxisTitle <- BTYAxisTitle
	
		XAxisTitle <- "Group"
		MainTitle2 <- ""
		#Gr_line <-"black"
		Line_size <- Line_size2
	
		#Code for LS MEans plot
		meanPlotq <- sub(".html", "meanplotq.png", htmlFile)
		png(meanPlotq,width = jpegwidth, height = jpegheight, units="in", res=PlotResolution)
	
		#STB July2013
		plotFilepdf5q <- sub(".html", "meanplotq.pdf", htmlFile)
		dev.control("enable") 
	
		#GGPLOT2 code
		if (factno == 1 || factno > 4) {
			XAxisTitle <- unique (strsplit(selectedEffectx, "*",fixed = TRUE)[[1]])[1]
			LSMPLOT_1()
		}
	
		if (factno == 2) {
			if (length(unique(levels(graphdata[,1]))) > length(unique(levels(graphdata[,2])))) {
				XAxisTitle <- unique (strsplit(selectedEffectx, "*",fixed = TRUE)[[1]])[1]
				graphdata$jj_1 <- graphdata[,1]
				legendz <- unique (strsplit(selectedEffectx, "*",fixed = TRUE)[[1]])[2]
				graphdata$catzz<-legendz
				graphdata$jj_2 <- paste(graphdata$catzz, "=",graphdata[,2], sep = "") 
			} else {
				XAxisTitle <- unique (strsplit(selectedEffectx, "*",fixed = TRUE)[[1]])[2]
				graphdata$jj_1 <- graphdata[,2]
				legendz <- unique (strsplit(selectedEffectx, "*",fixed = TRUE)[[1]])[1]
				graphdata$catzz<-legendz
				graphdata$jj_2 <- paste(graphdata$catzz, "=",graphdata[,1], sep = "") 
			}
			Gr_palette<- palette_FUN("jj_2")
			LSMPLOT_2("none")
		}
	
		if (factno == 3) {
			if (length(unique(levels(graphdata[,1]))) > length(unique(levels(graphdata[,2]))) &&  length(unique(levels(graphdata[,1]))) > length(unique(levels(graphdata[,3]))  )) {
				XAxisTitle <- unique (strsplit(selectedEffectx, "*",fixed = TRUE)[[1]])[1]
				graphdata$jj_1 <- graphdata[,1]
				legendz <- unique (strsplit(selectedEffectx, "*",fixed = TRUE)[[1]])[2]
				graphdata$catzz<-legendz
				graphdata$jj_2 <- paste(graphdata$catzz, "=",graphdata[,2], sep = "") 
				legendz <- unique (strsplit(selectedEffectx, "*",fixed = TRUE)[[1]])[3]
				graphdata$catzz<-legendz
				graphdata$jj_3 <- paste(graphdata$catzz, "=",graphdata[,3], sep = "") 
	
			} else  if (length(unique(levels(graphdata[,2]))) > length(unique(levels(graphdata[,1]))) &&  length(unique(levels(graphdata[,2]))) > length(unique(levels(graphdata[,3])))  ) {
				XAxisTitle <- unique (strsplit(selectedEffectx, "*",fixed = TRUE)[[1]])[2]
				graphdata$jj_1 <- graphdata[,2]
				legendz <- unique (strsplit(selectedEffectx, "*",fixed = TRUE)[[1]])[1]
				graphdata$catzz<-legendz
				graphdata$jj_2 <- paste(graphdata$catzz, "=",graphdata[,1], sep = "") 
				legendz <- unique (strsplit(selectedEffectx, "*",fixed = TRUE)[[1]])[3]
				graphdata$catzz<-legendz
				graphdata$jj_3 <- paste(graphdata$catzz, "=",graphdata[,3], sep = "") 
	
			} else {
				XAxisTitle <- unique (strsplit(selectedEffectx, "*",fixed = TRUE)[[1]])[3]
				graphdata$jj_1 <- graphdata[,3]
				legendz <- unique (strsplit(selectedEffectx, "*",fixed = TRUE)[[1]])[1]
				graphdata$catzz<-legendz
				graphdata$jj_2 <- paste(graphdata$catzz, "=",graphdata[,1], sep = "") 
				legendz <- unique (strsplit(selectedEffectx, "*",fixed = TRUE)[[1]])[2]
				graphdata$catzz<-legendz
				graphdata$jj_3 <- paste(graphdata$catzz, "=",graphdata[,2], sep = "") 
			}
			Gr_palette<- palette_FUN("jj_2")
			LSMPLOT_2("three")
		}
	
		if (factno == 4) {
			if (length(unique(levels(graphdata[,1]))) > length(unique(levels(graphdata[,2]))) &&  length(unique(levels(graphdata[,1]))) > length(unique(levels(graphdata[,3]))) &&  length(unique(levels(graphdata[,1]))) > length(unique(levels(graphdata[,4])))) {
				XAxisTitle <- unique (strsplit(selectedEffectx, "*",fixed = TRUE)[[1]])[1]
				graphdata$jj_1 <- graphdata[,1]
				legendz <- unique (strsplit(selectedEffectx, "*",fixed = TRUE)[[1]])[2]
				graphdata$catzz<-legendz
				graphdata$jj_2 <- paste(graphdata$catzz, "=",graphdata[,2], sep = "") 
				legendz <- unique (strsplit(selectedEffectx, "*",fixed = TRUE)[[1]])[3]
				graphdata$catzz<-legendz
				graphdata$jj_3 <- paste(graphdata$catzz, "=",graphdata[,3], sep = "") 
				legendz <- unique (strsplit(selectedEffectx, "*",fixed = TRUE)[[1]])[4]
				graphdata$catzz<-legendz
				graphdata$jj_4 <- paste(graphdata$catzz, "=",graphdata[,4], sep = "") 
		
			} else	if (length(unique(levels(graphdata[,2]))) > length(unique(levels(graphdata[,1]))) &&  length(unique(levels(graphdata[,2]))) > length(unique(levels(graphdata[,3]))) &&  length(unique(levels(graphdata[,2]))) > length(unique(levels(graphdata[,4])))) {
				XAxisTitle <- unique (strsplit(selectedEffectx, "*",fixed = TRUE)[[1]])[2]
				graphdata$jj_1 <- graphdata[,2]
				legendz <- unique (strsplit(selectedEffectx, "*",fixed = TRUE)[[1]])[1]
				graphdata$catzz<-legendz
				graphdata$jj_2 <- paste(graphdata$catzz, "=",graphdata[,1], sep = "") 
				legendz <- unique (strsplit(selectedEffectx, "*",fixed = TRUE)[[1]])[3]
				graphdata$catzz<-legendz
				graphdata$jj_3 <- paste(graphdata$catzz, "=",graphdata[,3], sep = "") 
				legendz <- unique (strsplit(selectedEffectx, "*",fixed = TRUE)[[1]])[4]
				graphdata$catzz<-legendz
				graphdata$jj_4 <- paste(graphdata$catzz, "=",graphdata[,4], sep = "") 
	
			} else 	if (length(unique(levels(graphdata[,3]))) > length(unique(levels(graphdata[,1]))) &&  length(unique(levels(graphdata[,3]))) > length(unique(levels(graphdata[,2]))) &&  length(unique(levels(graphdata[,3]))) > length(unique(levels(graphdata[,4])))) {
				XAxisTitle <- unique (strsplit(selectedEffectx, "*",fixed = TRUE)[[1]])[3]
				graphdata$jj_1 <- graphdata[,3]
				legendz <- unique (strsplit(selectedEffectx, "*",fixed = TRUE)[[1]])[1]
				graphdata$catzz<-legendz
				graphdata$jj_2 <- paste(graphdata$catzz, "=",graphdata[,1], sep = "") 
				legendz <- unique (strsplit(selectedEffectx, "*",fixed = TRUE)[[1]])[2]
				graphdata$catzz<-legendz
				graphdata$jj_3 <- paste(graphdata$catzz, "=",graphdata[,2], sep = "") 
				legendz <- unique (strsplit(selectedEffectx, "*",fixed = TRUE)[[1]])[4]
				graphdata$catzz<-legendz
				graphdata$jj_4 <- paste(graphdata$catzz, "=",graphdata[,4], sep = "") 
	
			} else  {
				XAxisTitle <- unique (strsplit(selectedEffectx, "*",fixed = TRUE)[[1]])[4]
				graphdata$jj_1 <- graphdata[,4]
				legendz <- unique (strsplit(selectedEffectx, "*",fixed = TRUE)[[1]])[1]
				graphdata$catzz<-legendz
				graphdata$jj_2 <- paste(graphdata$catzz, "=",graphdata[,1], sep = "") 
				legendz <- unique (strsplit(selectedEffectx, "*",fixed = TRUE)[[1]])[2]
				graphdata$catzz<-legendz
				graphdata$jj_3 <- paste(graphdata$catzz, "=",graphdata[,2], sep = "") 
				legendz <- unique (strsplit(selectedEffectx, "*",fixed = TRUE)[[1]])[3]
				graphdata$catzz<-legendz
				graphdata$jj_4 <- paste(graphdata$catzz, "=",graphdata[,3], sep = "") 
			} 
			Gr_palette<- palette_FUN("jj_2")
			LSMPLOT_2("four")
		}
	
		void<-HTMLInsertGraph(GraphFileName=sub("[A-Z0-9a-z,:,\\\\]*App_Data[\\\\]","", meanPlotq), Align="left")
	
		#STB July2013
		if (pdfout=="Y") {
			pdf(file=sub("[A-Z0-9a-z,:,\\\\]*App_Data[\\\\]","", plotFilepdf5q), height = pdfheight, width = pdfwidth) 
			dev.set(2) 
			dev.copy(which=3) 
			dev.off(2)
			dev.off(3)
			pdfFile_5q<-sub("[A-Z0-9a-z,:,\\\\]*App_Data[\\\\]","",plotFilepdf5q)
			linkToPdf5q <- paste ("<a href=\"",pdfFile_5q,"\">Click here to view the PDF of the plot of back transformed geometric means</a>", sep = "")
			HTML(linkToPdf5q)
		}
	}
}




#===================================================================================================================
#Pairwise equivalence tests table
#===================================================================================================================
lowerCI<-paste("   Lower one-sided ",(sig*100),"% CI   ",sep="")
upperCI<-paste("   Upper one-sided ",(sig*100),"% CI   ",sep="")

#Creating input dataset without dashes in
ivs_num_ivs <- rep(1:dim(statdata)[1])
ivs_char_ivs <- rep(factor(LETTERS[1:dim(statdata)[1]]), 1)
statdata_temp2<- cbind(statdata_temp, ivs_num_ivs,ivs_char_ivs )
statdata_num<- statdata_temp2[,sapply(statdata_temp2,is.numeric)]
statdata_char<- statdata_temp2[,!sapply(statdata_temp2,is.numeric)]
statdata_char2 <- as.data.frame(sapply(statdata_char,gsub,pattern="-",replacement="_ivs_dash_ivs_"))
statdata<- data.frame(cbind(statdata_num, statdata_char2))


#Creating all pairwise results
mult<-glht(lm(model, data=statdata, na.action = na.omit), linfct=lsm(eval(parse(text = paste("pairwise ~",selectedEffect)))))
multci<-confint(mult, level=sigeq, calpha = univariate_calpha())
tablen<-length(unique(rownames(multci$confint)))
rows<-rownames(multci$confint)
for (i in 1:1000) {
	rows<-sub("_ivs_dash_ivs_"," - ", rows, fixed=TRUE)
}

#Creating a matrix of the difference labels
comps<-c(rownames(multci$confint))
diffz <-matrix(nrow=length(comps), ncol=2)
for (g in 1:length(comps)) {
	comps2<-unlist(strsplit(comps[g]," - " ))[1]
	comps3<-unlist(strsplit(comps[g]," - " ))[2]
	diffz[g,1] = comps2
	diffz[g,2] = comps3
}

#Generating a dataset with numeric levels in
if (AnalysisType == "two-sided") {
	tabsNum<-data.frame(matrix(nrow=tablen, ncol=8))
} else {
	tabsNum<-data.frame(matrix(nrow=tablen, ncol=6))
}

#Entries for both one-sided and two-sided tests
for (i in 1:tablen) {
	tabsNum[i,1]= diffz[i,1]
	tabsNum[i,2]= diffz[i,2]
	tabsNum[i,3]=multci$confint[i]
}

#Two-sided table entries
if (AnalysisType == "two-sided") {
	for (i in 1:tablen) {
		tabsNum[i,9] = paste(tabsNum[i,1] , " - ", tabsNum[i,2], sep  = "")
		tabsNum[i,4]=multci$confint[i+tablen]
		tabsNum[i,5]=multci$confint[i+2*tablen]
		tabsNum[i,6]=lowerboundtest
		tabsNum[i,7]=upperboundtest
		tabsNum[i,8]="Inconclusive"
	}
	for (i in 1:tablen) {
		if(tabsNum[i,5] < lower) {
			tabsNum[i,8]="Not equivalent"
		}
		if(tabsNum[i,4] > upper) {
			tabsNum[i,8]="Not equivalent"
		}
		if(tabsNum[i,5] < upper && tabsNum[i,4] > lower) {
			tabsNum[i,8]="Equivalent"
		}
	}
	if(backToControlTest == "comparisonstocontrol") {
		tabsNum2 <- tabsNum
		for ( i in 1:tablen) {
			if (tabsNum[i,1] == cntrlGroup) {
				tabsNum2[i,3] = -1*tabsNum[i,3]
				tabsNum2[i,4] = -1*tabsNum[i,5]
				tabsNum2[i,5] = -1*tabsNum[i,4]
				tabsNum2[i,1] = tabsNum[i,2]
				tabsNum2[i,2] = tabsNum[i,1]
				tabsNum2[i,9] = paste(tabsNum[i,2] , " - ", tabsNum[i,1], sep  = "")
			} else {
				tabsNum2[i,9] = paste(tabsNum[i,1] , " - ", tabsNum[i,2], sep  = "")
			}
		}
		tabsNum <- tabsNum2
		tabsNum<-subset(tabsNum, tabsNum[,2] == cntrlGroup)
	}
}

#One-sided upper limit table entries
if (AnalysisType == "upper-sided") {
	for (i in 1:tablen) {
		tabsNum[i,7] = paste(tabsNum[i,1] , " - ", tabsNum[i,2], sep  = "")
		tabsNum[i,4]=multci$confint[i+2*tablen]
		tabsNum[i,5]=upperboundtest
		tabsNum[i,6]="Inconclusive"
	}
	for (i in 1:tablen) {
		if(tabsNum[i,3] > upper) {
			tabsNum[i,6]="Not equivalent"
		}
		if(tabsNum[i,3] < upper && tabsNum[i,4] < upper) {
			tabsNum[i,6]="Equivalent"
		}
	}
	if(backToControlTest == "comparisonstocontrol") {
		tabsNum2 <- tabsNum
		for ( i in 1:tablen) {
			if (tabsNum[i,1] == cntrlGroup) {
				tabsNum2[i,3] = -1*tabsNum[i,3]
				tabsNum2[i,4] = -1*multci$confint[i+tablen]
				tabsNum2[i,1] = tabsNum[i,2]
				tabsNum2[i,2] = tabsNum[i,1]
				tabsNum2[i,7] = paste(tabsNum[i,2] , " - ", tabsNum[i,1], sep  = "")
			} else {
				tabsNum2[i,7] = paste(tabsNum[i,1] , " - ", tabsNum[i,2], sep  = "")
			}
		}
		tabsNum <- tabsNum2
		tabsNum<-subset(tabsNum, tabsNum[,2] == cntrlGroup)
	}
}
	
#One-sided lower limit table entries
if (AnalysisType == "lower-sided") {
	for (i in 1:tablen) {
		tabsNum[i,7] = paste(tabsNum[i,1] , " - ", tabsNum[i,2], sep  = "")
		tabsNum[i,4]=multci$confint[i+tablen]
		tabsNum[i,5]=lowerboundtest
		tabsNum[i,6]="Inconclusive"
	}
	for (i in 1:tablen) {
		if(tabsNum[i,3] < lower) {
			tabsNum[i,6]="Not equivalent"
		}
		if(tabsNum[i,3] > lower && tabsNum[i,4] > lower) {
			tabsNum[i,6]="Equivalent"
		}
	}

	if(backToControlTest == "comparisonstocontrol") {
		tabsNum2 <- tabsNum
		for ( i in 1:tablen) {
			if (tabsNum[i,1] == cntrlGroup) {
				tabsNum2[i,3] = -1*tabsNum[i,3]
				tabsNum2[i,4] = -1*multci$confint[i+2*tablen]
				tabsNum2[i,1] = tabsNum[i,2]
				tabsNum2[i,2] = tabsNum[i,1]
				tabsNum2[i,7] = paste(tabsNum[i,2] , " - ", tabsNum[i,1], sep  = "")
			} else {
				tabsNum2[i,7] = paste(tabsNum[i,1] , " - ", tabsNum[i,2], sep  = "")
			}
		}
		tabsNum <- tabsNum2
		tabsNum<-subset(tabsNum, tabsNum[,2] == cntrlGroup)
	}
}

#Back transforming the log transformed values
if(responseTransform =="log10") {
	for (i in 1:dim(tabsNum)[1]) {	
		tabsNum[i,3] = 10^(tabsNum[i,3])
		tabsNum[i,4] = 10^(tabsNum[i,4])
		if (AnalysisType == "two-sided") {
			tabsNum[i,5] = 10^(tabsNum[i,5])
			tabsNum[i,9] = paste(tabsNum[i,1] , " / ", tabsNum[i,2])
		} else {
			tabsNum[i,7] = paste(tabsNum[i,1] , " / ", tabsNum[i,2])
		}
	}
}

if(responseTransform =="loge") {
	for (i in 1:dim(tabsNum)[1]) {	
		tabsNum[i,3] = exp(tabsNum[i,3])
		tabsNum[i,4] = exp(tabsNum[i,4])
		if (AnalysisType == "two-sided") {
			tabsNum[i,5] = exp(tabsNum[i,5])
			tabsNum[i,9] = paste(tabsNum[i,1] , " / ", tabsNum[i,2])
		} else {
			tabsNum[i,7] = paste(tabsNum[i,1] , " / ", tabsNum[i,2])
		}
	}
}




#Creating the final dataset
if (AnalysisType == "two-sided") {
	tabs<-data.frame(matrix(nrow=dim(tabsNum)[1], ncol=7))
} else {
	tabs<-data.frame(matrix(nrow=dim(tabsNum)[1], ncol=5))
}

#Adding table names
if (AnalysisType == "two-sided" && (responseTransform != "log10" && responseTransform != "loge")) {
	colnames(tabs)<-c("Comparison", "Difference", lowerCI, upperCI, "Lower equivalence bound", "Upper equivalence bound", "Equivalence assessment")
}
if (AnalysisType == "lower-sided" && (responseTransform != "log10" && responseTransform != "loge")) {
	colnames(tabs)<-c("Comparison", "Difference", lowerCI, "Lower equivalence bound", "Equivalence assessment")
}
if (AnalysisType == "upper-sided" && (responseTransform != "log10" && responseTransform != "loge")) {
	colnames(tabs)<-c("Comparison", "Difference", upperCI, "Upper equivalence bound", "Equivalence assessment")
}
if (AnalysisType == "two-sided" && (responseTransform == "log10" || responseTransform == "loge")) {
	colnames(tabs)<-c("Comparison", "Fold change", lowerCI, upperCI, "Lower equivalence bound", "Upper equivalence bound", "Equivalence assessment")
}
if (AnalysisType == "lower-sided" && (responseTransform == "log10" || responseTransform == "loge")) {
	colnames(tabs)<-c("Comparison", "Fold change", lowerCI, "Lower equivalence bound", "Equivalence assessment")
}
if (AnalysisType == "upper-sided" && (responseTransform == "log10" || responseTransform == "loge")) {
	colnames(tabs)<-c("Comparison", "Fold change", upperCI, "Upper equivalence bound", "Equivalence assessment")
}

#Creating the final table
for (i in 1:dim(tabsNum)[1]) {
	tabs[i,2]=format(round(tabsNum[i,3], 3), nsmall=3, scientific=FALSE)
	tabs[i,3]=format(round(tabsNum[i,4], 3), nsmall=3, scientific=FALSE)
}

if (AnalysisType == "two-sided") {
	for (i in 1:dim(tabsNum)[1]) {
		tabs[i,1]=tabsNum[i,9]
		tabs[i,4]=format(round(tabsNum[i,5], 3), nsmall=3, scientific=FALSE)
		tabs[i,5]=tabsNum[i,6]
		tabs[i,6]=tabsNum[i,7]
#		tabs[i,5]=format(round(tabsNum[i,6], 3), nsmall=3, scientific=FALSE)
#		tabs[i,6]=format(round(tabsNum[i,7], 3), nsmall=3, scientific=FALSE)
		tabs[i,7]=tabsNum[i,8]
	}
}
if (AnalysisType == "lower-sided" || AnalysisType == "upper-sided") {
	for (i in 1:dim(tabsNum)[1]) {
		tabs[i,1]=tabsNum[i,7]
		tabs[i,4]=tabsNum[i,5]
#		tabs[i,4]=format(round(tabsNum[i,5], 3), nsmall=3, scientific=FALSE)
		tabs[i,5]=tabsNum[i,6]
	}
}

#Pairwise tests title
if(backToControlTest != "comparisonstocontrol") {
	add<-paste(c("All pairwise equivalence assessments"))
} else {
	add<-paste(c("All to one pairwise equivalence assessments"))
}
HTML.title(add, HR=2, align="left")

HTML(tabs, classfirstline="second", align="left", row.names = "FALSE")



#===================================================================================================================
#Plot of pairwise equivalence tests
#===================================================================================================================
#Plot of the equivalence results
if (AnalysisType == "two-sided" ) {
	CITitle<-paste("Plot of the comparisons between the predicted means with ",(sig*100),"% one-sided confidence intervals along with equivalence bounds",sep="")
	HTML.title(CITitle, HR=2, align="left")
} 
if ( AnalysisType == "upper-sided" || AnalysisType == "lower-sided"  ) {
	CITitle<-paste("Plot of the comparisons between the predicted means with ",(sig*100),"% one-sided confidence interval along with equivalence bound",sep="")
	HTML.title(CITitle, HR=2, align="left")
}

#Code for EQ plot
meanPlotqq <- sub(".html", "meanplotqq.png", htmlFile)
png(meanPlotqq,width = jpegwidth, height = jpegheight, units="in", res=PlotResolution)

#STB July2013
plotFilepdf5qq <- sub(".html", "meanplotqq.pdf", htmlFile)
dev.control("enable") 

#Setting up the dataset
graphdata<-data.frame(tabsNum)
Gr_intercept <- 0
if (responseTransform != "log10" && responseTransform != "loge") {
	XAxisTitle <- "Difference"
} else {
	XAxisTitle <- "Fold change"
}
YAxisTitle <- "Comparison"
Gr_line_type<-Line_type_dashed
Gr_line_typeint<-Line_type_dashed

if (AnalysisType == "two-sided") {
	gr_lowerEqB<-lowerboundtest
	gr_upperEqB<-upperboundtest
}
if (AnalysisType == "lower-sided") {
	gr_lowerEqB<-lowerboundtest
}
if (AnalysisType == "upper-sided") {
	gr_upperEqB<-upperboundtest
}

#GGPLOT2 code
if (AnalysisType == "two-sided") {
	EQPLOT2S()
} else {
	EQPLOT1S(AnalysisType)
}
void<-HTMLInsertGraph(GraphFileName=sub("[A-Z0-9a-z,:,\\\\]*App_Data[\\\\]","", meanPlotqq), Align="left")
	
#STB July2013
if (pdfout=="Y") {
	pdf(file=sub("[A-Z0-9a-z,:,\\\\]*App_Data[\\\\]","", plotFilepdf5qq), height = pdfheight, width = pdfwidth) 
	dev.set(2) 
	dev.copy(which=3) 
	dev.off(2)
	dev.off(3)
	pdfFile_5qq<-sub("[A-Z0-9a-z,:,\\\\]*App_Data[\\\\]","",plotFilepdf5qq)
	linkToPdf5qq <- paste ("<a href=\"",pdfFile_5qq,"\">Click here to view the PDF of the plot of comparisons back to control</a>", sep = "")
	HTML(linkToPdf5qq)
}	

#===================================================================================================================
#Conclusion
add<-paste(c("Conclusion"))
HTML.title(add, HR=2, align="left")

if (noeffects>testeffects)  {
	HTML("Warning: It is not advisable to draw statistical inferences about a factor/interaction in the presence of a significant higher-order interaction involving that factor/interaction. ", align="left")
}

#Relabel entries
for(i in 1:(dim(tabs)[1])) {
	tabs[i,1] <- sub("-", " and ", tabs[i,1])
	tabs[i,1] <- sub("/", " and ", tabs[i,1])
}

inte<-1
for(i in 1:(dim(tabs)[1])) {
	if (AnalysisType == "two-sided") {
		if (tabs[i,7] ==  "Equivalent") {
			if (inte==1) {
				inte<-inte+1
				add<-paste(add, ": The following means are deemed equivalent at the  ", 100*(1-sig), "% level: ", tabs[i,1], sep="")
			} else {
				inte<-inte+1
				add<-paste(add, ", ", tabs[i,1], sep="")
			}
		} 
	} else {
		if (tabs[i,5] ==  "Equivalent") {
			if (inte==1) {
				inte<-inte+1
				add<-paste(add, ": The following means are deemed equivalent at the  ", 100*(1-sig), "% level: ", tabs[i,1], sep="")
			} else {
				inte<-inte+1
				add<-paste(add, ", ", tabs[i,1], sep="")
			}
		} 
	}
}
	
if (inte==1) {
	if (dim(tabs)[1] >1) {
		add<-paste(add, ": There are no equivalent means.", sep="")
	} else {
		add<-paste(add, ": The means are not equivalent.", sep="")
	}
} else {
	add<-paste(add, ". ", sep="")
}
HTML(add, align="left")

#===================================================================================================================
#Diagnostic plots
#===================================================================================================================
if((showPRPlot=="Y" && showNormPlot=="N") || (showPRPlot=="N" && showNormPlot=="Y") ) {
		HTML.title("Diagnostic plot", HR=2, align="left")
}
if(showPRPlot=="Y" && showNormPlot=="Y") {
		HTML.title("Diagnostic plots", HR=2, align="left")
}

#Residual plots
if(showPRPlot=="Y") {
	HTML.title("Residuals vs. predicted plot", HR=3, align="left")

	residualPlot <- sub(".html", "residualplot.png", htmlFile)
	png(residualPlot,width = jpegwidth, height = jpegheight, units="in", res=PlotResolution)

	#STB July2013
	plotFilepdf3 <- sub(".html", "residualplot.pdf", htmlFile)
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

	Gr_line_type<-Line_type_dashed

	Line_size2 <- Line_size
	Line_size <- 0.5

	#GGPLOT2 code
	NONCAT_SCAT("RESIDPLOT")

	MainTitle2 <- ""

	void<-HTMLInsertGraph(GraphFileName=sub("[A-Z0-9a-z,:,\\\\]*App_Data[\\\\]","", residualPlot), Align="centre")

	#STB July2013
	if (pdfout=="Y") {
		pdf(file=sub("[A-Z0-9a-z,:,\\\\]*App_Data[\\\\]","", plotFilepdf3), height = pdfheight, width = pdfwidth) 
		dev.set(2) 
		dev.copy(which=3) 
		dev.off(2)
		dev.off(3)
		pdfFile_3<-sub("[A-Z0-9a-z,:,\\\\]*App_Data[\\\\]","",plotFilepdf3)
		linkToPdf3 <- paste ("<a href=\"",pdfFile_3,"\">Click here to view the PDF of the residuals vs. predicted plot</a>", sep = "")
		HTML(linkToPdf3)
	}
	HTML("Tip: On this plot look to see if the spread of the points increases as the predicted values increase. If so the response may need transforming.", align="left")
	HTML("Tip: Any observation with a residual less than -3 or greater than 3 (SD) should be investigated as a possible outlier.", align="left")
}

#Normality plots
if(showNormPlot=="Y") {
	HTML.title("Normal probability plot", HR=3, align="left")

	normPlot <- sub(".html", "normplot.png", htmlFile)
	png(normPlot,width = jpegwidth, height = jpegheight, units="in", res=PlotResolution)

	#STB July2013
	plotFilepdf4 <- sub(".html", "normplot.pdf", htmlFile)
	dev.control("enable") 

	#Graphical parameters
	te<-qqnorm(resid(threewayfull))
	graphdata<-data.frame(te$x,te$y)
	graphdata$xvarrr_IVS <-graphdata$te.x
	graphdata$yvarrr_IVS <-graphdata$te.y
	YAxisTitle <-"Sample Quantiles"
	XAxisTitle <-"Theoretical Quantiles"
	MainTitle2 <- " "
	w_Gr_jitscat <- 0
	h_Gr_jitscat <-  0
	infiniteslope <- "N"
	LinearFit <- "Y"

	Gr_line_type<-Line_type_dashed
	Line_size <- 0.5
	Gr_alpha <- 1
	Line_type <-Line_type_dashed

	#GGPLOT2 code
	NONCAT_SCAT("QQPLOT")

	MainTitle2 <- ""
	#===================================================================================================================
	void<-HTMLInsertGraph(GraphFileName=sub("[A-Z0-9a-z,:,\\\\]*App_Data[\\\\]","", normPlot), Align="left")

	#STB July2013
	if (pdfout=="Y") {
		pdf(file=sub("[A-Z0-9a-z,:,\\\\]*App_Data[\\\\]","", plotFilepdf4), height = pdfheight, width = pdfwidth) 
		dev.set(2) 
		dev.copy(which=3) 
		dev.off(2)
		dev.off(3)
		pdfFile_4<-sub("[A-Z0-9a-z,:,\\\\]*App_Data[\\\\]","",plotFilepdf4)
		linkToPdf4 <- paste ("<a href=\"",pdfFile_4,"\">Click here to view the PDF of the normal probability plot</a>", sep = "")
		HTML(linkToPdf4)
	}
	HTML("Tip: Check that the points lie along the dotted line. If not then the data may be non-normally distributed.", align="left")
}

#===================================================================================================================
#Analysis description
#===================================================================================================================
HTML.title("Analysis description", HR=2, align="left")

add<-c("The data were analysed using a ")

if (AnalysisType == "two-sided") {
	add<- paste(add, "Two One-Sided (TOST) equivalence test, see Limentani et al. 2005", sep = "")
} else {
	add<- paste(add, "one-sided equivalence test, see Limentani et al. 2005", sep = "")
}
if (notreatfactors==1)  {
	if(FirstCatFactor != "NULL") {
		add<-paste(add, ", with ", treatFactors, " as the treatment factor", sep="")
	} else {
		add<-paste(add, ", with ", treatFactors, " as the treatment factor", sep="")
	}
} else {
	if(FirstCatFactor != "NULL") {
		add<-paste(add, ", with ", sep="")
	} else {
		add<-paste(add, ", with ", sep="")
	}
	for (i in 1:notreatfactors) {
		if (i<notreatfactors-1)	{
			add<-paste(add, treatlist[i], ", ", sep="")
		} else 	if (i<notreatfactors) {
			add<-paste(add, treatlist[i], " and ", sep="")
		} else if (i==notreatfactors) {
			add<-paste(add, treatlist[i], " as the treatment factors", sep="")
		}
	}
}

if (blockFactors != "NULL" && FirstCatFactor != "NULL")  {
	add<-paste(add, ", ", sep="")
} else if (noblockfactors==1 && blockFactors != "NULL" && FirstCatFactor == "NULL")  {
	add<-paste(add, " and ", sep="")
} 
	
if (noblockfactors==1 && blockFactors != "NULL")  {
	add<-paste(add, blockFactors, " as a blocking factor", sep="")
} else {
	if(noblockfactors>1)  {
		if (FirstCatFactor == "NULL") {
			add<-paste(add, " and ", sep="")
		}
		for (i in 1:noblockfactors) {
			if (i<noblockfactors-1) {
				add<-paste(add, blocklist[i], ", ", sep="")
			} else	if (i<noblockfactors) {
				add<-paste(add, blocklist[i], " and ", sep="")
			} else if (i==noblockfactors) {
				add<-paste(add, blocklist[i], sep="")
			}
		}
		add<-paste(add, " as the blocking factors", sep="")
	}
}
if (FirstCatFactor == "NULL") {
	add<-paste(add, ". ", sep="")
} else {
	add<-paste(add, " and ",  sep="")
	if (nocovars == 1) {
		add<-paste(add, covlist[1], " as the covariate.", sep="")
	} else {
		for (i in 1:nocovars) {
			if (i<nocovars-1)	{
				add<-paste(add, covlist[i], ", ", sep="")
			} else 	if (i<nocovars) {
				add<-paste(add, covlist[i], " and ", sep="")
			} else if (i==nocovars) {
				add<-paste(add, covlist[i], " as the covariates.", sep="")
			}
		}
	}
}

if (responseTransform != "none") {
	add<-paste(add, " The response was ", responseTransform, " transformed prior to analysis to stabilise the variance. ", sep="")
}

HTML(add, align="left")



#===================================================================================================================
#References
#===================================================================================================================
Ref_list<-R_refs()

#Bate and Clark comment
HTML(refxx,  align="left")	

HTML.title("References", HR=2, align="left")
HTML(Ref_list$IVS_ref, align="left")
HTML(Ref_list$BateClark_ref,  align="left")

HTML("<bf> Limentani, G.B., Ringo, M.C., Ye, F., Bergquist, M.L. and MCSorley E.O. (2005). Beyond the t-test: Statistical equivalence testing. Analytical Chemistry, 77(11), 221-226.", align="left")

if (AssessCovariateInteractions == "Y" && FirstCatFactor != "NULL") {
	HTML("<bf> Armitage, P., Matthews, J.N.S. and Berry, G. (2001). Statistical Methods in Medical Research. 4th edition; John Wiley & Sons. New York.",  align="left")
}
if(FirstCatFactor != "NULL") {
	HTML("<bf> Morris, T.R. (1999). Experimental Design and Analysis in Animal Sciences. CABI publishing. Wallingford, Oxon (UK).",  align="left")
}

HTML.title("R references", HR=4, align="left")
HTML(Ref_list$R_ref ,  align="left")
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

HTML(paste(capture.output(print(citation("multcomp"),bibtex=F))[4], capture.output(print(citation("multcomp"),bibtex=F))[5], capture.output(print(citation("multcomp"),bibtex=F))[6], sep = ""),  align="left")
HTML(paste(capture.output(print(citation("multcompView"),bibtex=F))[4], capture.output(print(citation("multcompView"),bibtex=F))[5], capture.output(print(citation("multcompView"),bibtex=F))[6], capture.output(print(citation("multcompView"),bibtex=F))[7], sep = ""),  align="left")
HTML(paste(capture.output(print(citation("car"),bibtex=F))[4], capture.output(print(citation("car"),bibtex=F))[5], capture.output(print(citation("car"),bibtex=F))[6], sep = ""),  align="left")
HTML(paste(capture.output(print(citation("emmeans"),bibtex=F))[4], capture.output(print(citation("emmeans"),bibtex=F))[5], capture.output(print(citation("emmeans"),bibtex=F))[6], sep = ""),  align="left")

#===================================================================================================================
#Show dataset
#===================================================================================================================


if (showdataset=="Y")
{
	statdata_temp<-subset(statdata_temp, select = -c(mainEffect, scatterPlotColumn,catfact))

	observ <- data.frame(c(1:dim(statdata_temp)[1]))
	colnames(observ) <- c("Observation")
	statdata_temp2 <- cbind(observ, statdata_temp)

	HTML.title("Analysis dataset", HR = 2, align = "left")
    	HTML(statdata_temp2, classfirstline = "second", align = "left", row.names = "FALSE")

}

#===================================================================================================================
#Show arguments
#===================================================================================================================
if (OutputAnalysisOps == "Y") {
	HTML.title("Analysis options", HR=2, align="left")

	HTML(paste("Response variable: ", resp, sep=""), align="left")
	
	if (responseTransform != "none") {
		HTML(paste("Response variable transformation: ", responseTransform, sep=""), align="left")
	}
	
	HTML(paste("Treatment factor(s): ", treatFactors, sep=""), align="left")
	
	if (blockFactors != "NULL") {
		HTML(paste("Other design (block) factor(s): ", blockFactors, sep=""), align="left")
	}

	if(FirstCatFactor != "NULL") {
		HTML(paste("Covariate(s): ", covariates, sep=""), align="left")
	}

	if (FirstCatFactor != "NULL" ) {
		HTML(paste("Primary factor: ", FirstCatFactor, sep=""), align="left")
	}

	if (FirstCatFactor != "NULL" && covariateTransform != "none") {
		HTML(paste("Covariate(s) transformation: ", covariateTransform, sep=""), align="left")
	}


	HTML(paste("Equivalence bounds type: ", EqBtype, sep=""), align="left")
	if (lowerboundtestN != "NULL") {
		HTML(paste("Lower equivalence bound: ", lowerboundtest, sep=""), align="left")
	}
	if (upperboundtestN != "NULL") {
		HTML(paste("Upper equivalence bound: ", upperboundtest, sep=""), align="left")
	}

	HTML(paste("Output residuals vs. predicted plot (Y/N): ", showPRPlot, sep=""), align="left")
	HTML(paste("Output normal probability plot (Y/N): ", showNormPlot, sep=""), align="left")
	HTML(paste("Significance level: ", 1-sig, sep=""), align="left")

	if (showLSMeans != "N" && (Args[19] != "NULL" | backToControlTest != "NULL" ) ) {
		HTML(paste("Selected effect (for pairwise mean comparisons): ", selectedEffect, sep=""), align="left")
	}

	HTML(paste("Output least square (predicted) means (Y/N): ", showLSMeans, sep=""), align="left")
	
	if (backToControlTest == "none") {
		HTML(paste("Comparisons back to control procedure (Y/N): Y"), align="left")
	}

	if ( backToControlTest != "null" ) {
		HTML(paste("Control group: ", cntrlGroup, sep=""), align="left")
	}
}
