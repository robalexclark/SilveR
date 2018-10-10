#===================================================================================================================
#R Libraries

suppressWarnings(library(multcomp))
suppressWarnings(library(car))
suppressWarnings(library(R2HTML))
suppressWarnings(library(lsmeans))

#===================================================================================================================
# retrieve args
Args <- commandArgs(TRUE)

#Read in data
statdata <- read.csv(Args[3], header=TRUE, sep=",")

#Copy Args
model <- Args[4]
scatterplotModel <- as.formula(Args[5])
covariateModel <- Args[6]
responseTransform <- Args[7]
covariateTransform <- Args[8]
FirstCatFactor <- Args[9]
treatFactors <- Args[10]
blockFactors <- Args[11]
showANOVA <- Args[12]
showPRPlot <- Args[13]
showNormPlot <- Args[14]
sig <- 1 - as.numeric(Args[15])
sig2 <- 1 - as.numeric(Args[15])/2
effectModel <- as.formula(Args[16])
effectModel2 <- Args[16]
selectedEffect <- Args[17]
showLSMeans <- Args[18]
allPairwiseTest <- Args[19]
backToControlTest <- Args[20]
cntrlGroup <- Args[21]

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
if (FirstCatFactor != "NULL") {
	Gr_palette<-palette_FUN(FirstCatFactor)
} 
Line_size2 <- Line_size
Labelz_IVS_ <- "N"
#V3.2 STB OCT2015
set.seed(5041975)

#STB 14OCT2015
#Set contrast options for Marginal overall tests
options(contrasts=c(unordered="contr.sum", ordered="contr.poly"))

#Set working directory for p-values dataset
direct2<- unlist(strsplit(Args[3],"/"))
direct<-direct2[1]
for (i in 2:(length(direct2)-1)) {
	direct<- paste(direct, "/", direct2[i], sep = "")
}

statdata$mainEffect<-as.factor(statdata$mainEffect)
statdata$scatterPlotColumn<-as.factor(statdata$scatterPlotColumn)

#Response
resp <- unlist(strsplit(Args[4],"~"))[1] #get the response variable from the main model

#Number of factors in Selected effect
factno<-length(unique (strsplit(selectedEffect, "*",fixed = TRUE)[[1]]))

#calculating number of block factors
noblockfactors=0
if (blockFactors !="NULL") {
	tempblockChanges <-strsplit(blockFactors, ",")
	txtexpectedblockChanges <- c("")
	for(i in 1:length(tempblockChanges[[1]]))  {
		txtexpectedblockChanges [length(txtexpectedblockChanges )+1]=(tempblockChanges[[1]][i]) 
	}
	noblockfactors<-length(txtexpectedblockChanges)-1
}

#calculating number of treatment factors
tempChanges <-strsplit(treatFactors, ",")
txtexpectedChanges <- c("")
for(i in 1:length(tempChanges[[1]]))  { 
	txtexpectedChanges [length(txtexpectedChanges )+1]=(tempChanges[[1]][i]) 
}
notreatfactors<-length(txtexpectedChanges)-1

#Removing illegal characters
selectedEffect<- namereplace2(selectedEffect)
selectedEffectx<- namereplace(selectedEffect)

#replace illegal characters in variable names
YAxisTitle <-resp
if(FirstCatFactor != "NULL") {
	XAxisTitle<-unlist(strsplit(covariateModel, "~"))[2]
}

#replace illegal characters in variable names
for (i in 1:10) {
	YAxisTitle<-namereplace(YAxisTitle)

	if(FirstCatFactor != "NULL") {
		XAxisTitle<-namereplace(XAxisTitle)
	}
}
LS_YAxisTitle<-YAxisTitle

# Code to create varibale to test if the highest order interaction is selected
testeffects = noblockfactors
if(FirstCatFactor != "NULL") {
	testeffects = noblockfactors+1
}
emodel <-strsplit(effectModel2, "+", fixed = TRUE)

emodelChanges <- c("")
for(i in 1:length(emodel[[1]]))  { 
	emodelChanges [length(emodelChanges )+1]=(emodel[[1]][i]) 
}
noeffects<-length(emodelChanges)-2
#===================================================================================================================
#Titles and description
#===================================================================================================================
#Output HTML header
Title <-paste(branding, " Incomplete Factorial Parametric Analysis", sep="")
HTML.title(Title, HR = 1, align = "left")#Response

title<-c("Response")
if(FirstCatFactor != "NULL") {
	title<-paste(title, " and covariate", sep="")
}
HTML.title(title, HR=2, align="left")

add<-paste(c("The  "), resp, " response is currently being analysed by the Incomplete Factorial Parametric Analysis module", sep="")
if(FirstCatFactor != "NULL") {
	add<-paste(add, c(", with  "), unlist(strsplit(covariateModel, "~"))[2], " fitted as a covariate.", sep="")
} else {
	add<-paste(add, ".", sep="")
}

if (responseTransform != "None") {
	add<-paste(add, c("The response has been "), responseTransform, " transformed prior to analysis.", sep="")
}

if (covariateTransform != "None") {
	ad3<-paste(add, c("The covariate has been "), covariateTransform, " transformed prior to analysis.", sep="")
}
HTML(add, align="left")

#Warning
HTML.title("Warning", HR=2, align="left")
HTML("Warning: This module is currently under construction, care should be taken when considering the results. The results have not been verified.", align="left")

#===================================================================================================================
#Scatterplot
#===================================================================================================================
title<-c("Scatterplot of the raw data")
if(responseTransform != "None") {
	title<-paste(title, " (on the ", responseTransform, " scale)", sep="")
}
HTML.title(title, HR=2, align="left")

scatterPlot <- sub(".html", "scatterPlot.jpg", htmlFile)
jpeg(scatterPlot,width = jpegwidth, height = jpegheight, quality = 100)

#STB July2013
plotFilepdf1 <- sub(".html", "scatterPlot.pdf", htmlFile)
dev.control("enable") 

#Graphical parameters
graphdata<-statdata
graphdata$yvarrr_IVS <- eval(parse(text = paste("statdata$",resp)))
graphdata$xvarrr_IVS <-statdata$scatterPlotColumn
XAxisTitle <- ""
MainTitle2 <- ""
w_Gr_jit <- 0
h_Gr_jit <-  0
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
	title<-c("Covariate plot of the raw data")
	if(responseTransform != "None" || covariateTransform != "None") {
		title<-paste(title, " (on the transformed scale)", sep="")
	} 
	HTML.title(title, HR=2, align="left")

	ncscatterplot3 <- sub(".html", "ncscatterplot3.jpg", htmlFile)
	jpeg(ncscatterplot3,width = jpegwidth, height = jpegheight, quality = 100)

	#STB July2013
	plotFilepdf2 <- sub(".html", "ncscatterplot3.pdf", htmlFile)
	dev.control("enable") 

	#Graphical parameters
	graphdata<-statdata
	graphdata$xvarrr_IVS <- eval(parse(text = paste("statdata$",unlist(strsplit(covariateModel, "~"))[2])))
	graphdata$yvarrr_IVS <- eval(parse(text = paste("statdata$",resp)))
	graphdata$l_l <- eval(parse(text = paste("statdata$",FirstCatFactor))) 
	graphdata$catfact <-eval(parse(text = paste("statdata$",FirstCatFactor))) 
	XAxisTitle <- unlist(strsplit(covariateModel, "~"))[2]
	XAxisTitle<-namereplace(XAxisTitle)
	MainTitle2 <-""

	w_Gr_jit <- 0
	h_Gr_jit <- 0

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

	#STB Aug 2011 - removing lines with infinite slope
	if (infiniteslope != "N") {
		title<-paste("Warning: The covariate has the same value for all subjects in one or more levels of the ", FirstCatFactor, " factor. Care should be taken if you want to include this covariate in the analysis.", sep="")
		HTML(title, align="left")
	}
	HTML("Tip: In order to decide whether it is helpful to fit the covariate, the following should be considered:", align="left")
	HTML("a) Is there a relationship between the response and the covariate? (N.B., It is only worth fitting the covariate if there is a strong positive (or negative) relationship between them: i.e., the lines on the plot should not be horizontal).", align="left")
	HTML("b) Is the relationship similar for all treatments? (The lines on the plot should be approximately parallel).", align="left")
	HTML("c) Is the covariate influenced by the treatment? (We assume the covariate is not influenced by the treatment and so there should be no separation of the treatment groups along the x-axis on the plot).", HR=0, align="left")
	HTML("These issues are discussed in more detail in Morris (1999).", align="left")
}

#===================================================================================================================
#building the covariate interaction model
#===================================================================================================================
if (AssessCovariateInteractions == "Y" && FirstCatFactor != "NULL") {

	# Defining the Response
	Resplist <-unlist(strsplit(covariateModel, "~"))[1]
	
	# Defining the covariate
	Covlist <-unlist(strsplit(covariateModel, "~"))[2]

	#Creating the list of model terms
	listmodel <- unlist(strsplit(model,"~"))[2] #get the main model
	temChanges <-strsplit(listmodel, "+", fixed = TRUE)
	Modellist <- c("")
	for(i in 1:length(temChanges[[1]]))  { 
		Modellist[i]=(temChanges[[1]][i]) 
	}

	#Creating list of blocking factor
	if (noblockfactors > 0) {
		Blocklist <- c()
		for (i in 2:length(txtexpectedblockChanges)) {
			Blocklist[i-1] = txtexpectedblockChanges [i]
		}
	}

	#Creating the list of treatment terms
	Treatlist<-c()
	for (i in (1+noblockfactors+1):length(Modellist)) {
		Treatlist[i-(1+noblockfactors+1)+1] = Modellist[i]
	}

	#Creating the list of interaction terms
	Intlist <- c()
	for (i in 1:(length(Treatlist))) {
		Intlist[i] = paste (Covlist , "*" , Treatlist[i], sep = "" )
	}

	#Creating the covariate interaction model
	Fulllist <- c(Treatlist,Intlist)
	CovIntModela<-c(Covlist)
	if (noblockfactors > 0) {
		for (i in 1:noblockfactors) {
			CovIntModela[i+1] <- paste(CovIntModela[i] , " + " , Blocklist[i])
		}
	}
	CovIntModelb <- CovIntModela
	for (i in 1:length(Fulllist)) {
		CovIntModelb[i+length(CovIntModela)] <- paste(CovIntModelb[i+length(CovIntModela)-1] , " + " , Fulllist[i])
	}
	CovIntModel <- CovIntModelb[length(CovIntModelb)]
	
	#Creating the formula
	CovIntForm <- paste(Resplist , " ~ ", CovIntModel , sep = "")

	#Performing the ANCOVA analysis
	Covintfull<-lm(as.formula(CovIntForm), data=statdata, na.action = na.omit)

	#Title + warning
	HTML.title("Analysis of Covariance (ANCOVA) table for assessing covariate interactions", HR=2, align="left")

	#Printing ANCOVA Table - note this code is reused from below 
	temx<-anova(Covintfull)

	if (temx[dim(temx)[1],1] != 0) {
		tempx<-anova(Covintfull)
		temp2x<-(tempx)
		col1x<-format(round(temp2x[2], 2), nsmall=2, scientific=FALSE)
		col2x<-format(round(temp2x[3], 3), nsmall=3, scientific=FALSE)
		col3x<-format(round(temp2x[4], 2), nsmall=2, scientific=FALSE)
		col4x<-format(round(temp2x[5], 4), nsmall=4, scientific=FALSE)

		sourcex<-rownames(temp2x)
		#STB March 2014 - Replacing : with * in ANOVA table
		for (q in 1:notreatfactors) {
			sourcex<-sub(":"," * ", sourcex) 
		}
		ivsanovax<-cbind(sourcex, col1x, temp2x[1], col2x, col3x, col4x)

		ivsanovax[length(unique(sourcex)),5]<-" "
		ivsanovax[length(unique(sourcex)),6]<-" "

		#STB May 2012 capitals changed
		headx<-c("Effect", "Sums of squares", "Degrees of freedom","Mean square","F-value","p-value")
		colnames(ivsanovax)<-headx

		for (i in 1:(dim(ivsanovax)[1]-1))  {
			if (temp2x[i,5]<0.0001) {
				#STB March 2011 formatting p-values p<0.0001
				# ivsanovax[i,6]<-0.0001
				ivsanovax[i,6]=format(round(0.0001, 4), nsmall=4, scientific=FALSE)
				ivsanovax[i,6]<- paste("<",ivsanovax[i,6])
			}
		}
		HTML(ivsanovax, classfirstline="second", align="left", row.names = "FALSE")
		HTML("Note: This table should only be used to assess the covariate interactions. The statistical model used to generate all the remaining results in this output does not include the covariate interactions.", align="left")
	} 

	if (temx[dim(temx)[1],1] == 0) {
	HTML("The covariate interactions have not been calculated as there are zero residual degrees of freedom when all terms are included in the statistical model.", align="left")
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
	HTML.title("Unfortunately the residual degrees of freedom are low (less than 5). This may make the estimation of the underlying variability, and hence the results of the statistical tests, unreliable. This can be caused by attempting to fit too many factors, and their interactions, in the statistical model. Where appropriate we recommend you fit some of the 'Treatment' factors as 'Other design' factors. This will remove their interactions from the statistical model and therefore increase the residual degrees of freedom.", align="left")
}

if(showANOVA=="Y") {
	if(FirstCatFactor != "NULL") {
		HTML.title("Analysis of Covariance (ANCOVA) table", HR=2, align="left")
	} else {
		HTML.title("Analysis of variance (ANOVA) table", HR=2, align="left")
	}

	temp<-anova(threewayfull)
	col1<-format(round(temp[2], 2), nsmall=2, scientific=FALSE)
	col2<-format(round(temp[1], 0), nsmall=0, scientific=FALSE)
	col3<-format(round(temp[3], 3), nsmall=3, scientific=FALSE)
	col4<-format(round(temp[4], 2), nsmall=2, scientific=FALSE)
	col5<-format(round(temp[5], 4), nsmall=4, scientific=FALSE)

	source<-rownames(temp)
	#STB March 2014 - Replacing : with * in ANOVA table
	for (q in 1:notreatfactors) {
		source<-sub(":"," * ", source) 
	}	
	ivsanova<-cbind(source, col1,temp[2],col2,col3,col4)

	ivsanova[length(unique(source)),5]<-" "
	ivsanova[length(unique(source)),6]<-" "

	#STB May 2012 capitals changed
	head<-c("Effect", "Sums of squares", "Degrees of freedom", "Mean square", "F-value", "p-value")
	colnames(ivsanova)<-head

	for (i in 1:(dim(ivsanova)[1]-1)) {
		if (temp[i,5]<0.0001) {
			#STB March 2011 formatting p-values p<0.0001
			#ivsanova[i,6]<-0.0001
			ivsanova[i,6]=format(round(0.0001, 4), nsmall=4, scientific=FALSE)
			ivsanova[i,6]<- paste("<",ivsanova[i,6])
		}
	}

	HTML(ivsanova, classfirstline="second", align="left", row.names = "FALSE")

	if(FirstCatFactor != "NULL") {
		#STB Error spotted:
		#HTML.title("<sTitle<-sub("ivs_colon_ivs"	,":"ML.title("<bf>Comment: ANCOVA table calculated using a Type I model fit, see Armitage et al. (2001).", HR=0, align="left")
		HTML("Comment: ANCOVA table calculated using a Type I model fit, see Armitage et al. (2001).", align="left")
	} else {
		HTML("Comment: ANOVA table calculated using a Type I model fit, see Armitage et al. (2001).", align="left")
	}

	add<-paste(c("Conclusion"))
	inte<-1
	for(i in 1:(dim(ivsanova)[1]-1)) {
		if (ivsanova[i,6]<= (1-sig)) {
			if (inte==1) {
				inte<-inte+1
				add<-paste(add, ": There is a statistically significant overall difference between the levels of ", rownames(ivsanova)[i], sep="")
			} else {
				inte<-inte+1
				add<-paste(add, ", ", rownames(ivsanova)[i],  sep="")
			}
		} 
	}
	add<-paste(add, ". ", sep="")

	if (inte==1) {
		if (dim(ivsanova)[1]>2) {
			if(FirstCatFactor != "NULL") {

			#STB July 2013 change wording to remove effects
			add<-paste(add, ": There are no statistically significant overall differences between the levels of any of the terms in the ANCOVA table.", sep="")
			} else {
			add<-paste(add, ": There are no statistically significant overall differences between the levels of any of the terms in the ANOVA table.", sep="")
			}
		} 
	if (dim(ivsanova)[1]<=2) {
			add<-paste(add, ": There is no statistically significant overall difference between the levels of the treatment factor.", sep="")
		}
	} 

	HTML(add, align="left")
	if(FirstCatFactor != "NULL") {
		HTML("Tip: While it is a good idea to consider the overall tests in the ANCOVA table, we should not rely on them when deciding whether or not to make pairwise comparisons.", align="left")
	} else { 
		HTML("Tip: While it is a good idea to consider the overall tests in the ANOVA table, we should not rely on them when deciding whether or not to make pairwise comparisons.", align="left")
	}
}

#===================================================================================================================
#Covariate correlation table
#===================================================================================================================
if (CovariateRegressionCoefficients == "Y" && FirstCatFactor != "NULL") {
	HTML.title("Covariate regression coefficient", HR=2, align="left")

	covtable_1<-coef(summary(threewayfull))
	covtable<-data.frame(covtable_1)[c(2),]
	covtable_2<- covtable
	covtable$Estimate <-format(round(covtable$Estimate, 3), nsmall=3, scientific=FALSE) 
	covtable$Std..Error <-format(round(covtable$Std..Error, 3), nsmall=3, scientific=FALSE) 
	covtable$t.value <-format(round(covtable$t.value, 2), nsmall=2, scientific=FALSE) 
	covtable$Pr...t.. <-format(round(covtable$Pr...t.., 4), nsmall=4, scientific=FALSE) 
	covtable_1<- covtable

	if (as.numeric(covtable_2[1,4])<0.0001)  {
		#STB March 2011 formatting p-values p<0.0001
		#ivsanova[i,9]<-0.0001
		covtable_1[1,4]= "<0.0001"
	}

	rz<-rownames(covtable)[1]
	rownames(covtable_1)<-c(rz)

	colnames(covtable_1)<-c("Estimate", "Std error", "t-value", "p-value")
	HTML(covtable_1, classfirstline="second", align="left", row.names = "FALSE")
}

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

	residualPlot <- sub(".html", "residualplot.jpg", htmlFile)
	jpeg(residualPlot,width = jpegwidth, height = jpegheight, quality = 100)

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
	w_Gr_jit <- 0
	h_Gr_jit <-  0
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

	normPlot <- sub(".html", "normplot.jpg", htmlFile)
	jpeg(normPlot,width = jpegwidth, height = jpegheight, quality = 100)

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
	w_Gr_jit <- 0
	h_Gr_jit <-  0
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
#LS Means plot and table
#===================================================================================================================
if(showLSMeans =="Y") {
	#STB May 2012 Updating "least square (predicted) means"
	CITitle<-paste("Plot of the least square (predicted) means with ",(sig*100),"% confidence intervals",sep="")
	HTML.title(CITitle, HR=2, align="left")

	#Calculate LS Means dataset
	tabs<-lsmeans(threewayfull,eval(parse(text = paste("~",selectedEffect))), data=statdata)
	x<-summary(tabs)
	x<-na.omit(x)

	x$Mean <-x$lsmean 
	for (i in 1:dim(x)[1]) {
		x$Lower[i] <- x$lsmean[i]  - x$SE[i]*qt(sig2, x$df[i])
		x$Upper[i] <- x$lsmean[i]  + x$SE[i]*qt(sig2, x$df[i])
	}
	graphdata<-subset(x, select = -c(SE, df,lsmean, lower.CL, upper.CL )) 

	names <- c()
	for (l in 1:factno) {
		names[l] <- paste(unique (strsplit(selectedEffect, "*",fixed = TRUE)[[1]])[l], " Level", sep = "")
	}
	names[factno+1]<-"Mean"
	names[factno+2]<-"Lower"
	names[factno+3]<-"Upper"
	colnames(graphdata)<-names

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
	meanPlot <- sub(".html", "meanplot.jpg", htmlFile)
	jpeg(meanPlot,width = jpegwidth, height = jpegheight, quality = 100)

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
#===================================================================================================================
#Table of Least Square means
#===================================================================================================================
if(showLSMeans =="Y") {
	#STB May 2012 Updating "least square (predicted) means"
	CITitle2<-paste("Table of the least square (predicted) means with ",(sig*100),"% confidence intervals",sep="")
	HTML.title(CITitle2, HR=2, align="left")

	#Calculate LS Means Table
	x<-summary(tabs)
	x<-na.omit(x)

	x$Mean <-format(round(x$lsmean, 3), nsmall=3, scientific=FALSE) 
	for (i in 1:dim(x)[1]) {
		x$Lower[i] <- format(round(x$lsmean[i]  - x$SE[i]*qt(sig2, x$df[i]), 3), nsmall=3, scientific=FALSE) 
		x$Upper[i] <- format(round(x$lsmean[i]  + x$SE[i]*qt(sig2, x$df[i]), 3), nsmall=3, scientific=FALSE) 
	}

	names <- c("")
	for (l in 1:factno)
	{
		names[l+1] <- paste(unique (strsplit(selectedEffect, "*",fixed = TRUE)[[1]])[l], " ", sep = "")
	}

	x2<-subset(x, select = -c(SE, df,lsmean, lower.CL, upper.CL )) 

	observ <- data.frame(c(1:dim(x)[1]))
	x2 <- cbind(observ, x2)

	names[1]<-"Mean ID"
	names[factno+2]<-"Mean"
	names[factno+3]<-paste("Lower ",(sig*100),"% CI",sep="")
	names[factno+4]<-paste("Upper ",(sig*100),"% CI",sep="")

	colnames(x2)<-names
	HTML(x2, classfirstline="second", align="left", row.names = "FALSE")
}

#===================================================================================================================
#Back transformed geometric means plot and table 
#===================================================================================================================
if(GeomDisplay == "Y" && showLSMeans =="Y" && (responseTransform =="Log10"||responseTransform =="Loge")) {
	CITitle<-paste("Plot of the back-transformed geometric means with ",(sig*100),"% confidence intervals",sep="")
	HTML.title(CITitle, HR=2, align="left")
	HTML.title("As the response was log transformed prior to analysis the least square (predicted) means are presented on the log scale. These results can be back transformed onto the original scale. These are known as the back-transformed geometric means.", HR=0, align="left")

#===================================================================================================================
#LSMeans plot
#===================================================================================================================
#Calculate LS Means dataset
	tabs<-lsmeans(threewayfull,eval(parse(text = paste("~",selectedEffect))), data=statdata)
	x<-summary(tabs)
	x<-na.omit(x)

	if (responseTransform =="Log10") {
		x$Mean <-10^(x$lsmean)
		for (i in 1:dim(x)[1]) {
			x$Lower[i] <- 10^(x$lsmean[i]  - x$SE[i]*qt(sig2, x$df[i]))
			x$Upper[i] <- 10^(x$lsmean[i]  + x$SE[i]*qt(sig2, x$df[i]))
		}
	}

	if (responseTransform =="Loge") {
		x$Mean <-exp(x$lsmean)
		for (i in 1:dim(x)[1]) {
			x$Lower[i] <- exp(x$lsmean[i]  - x$SE[i]*qt(sig2, x$df[i]))
			x$Upper[i] <- exp(x$lsmean[i]  + x$SE[i]*qt(sig2, x$df[i]))
		}
	}
	graphdata<-subset(x, select = -c(SE, df,lsmean, lower.CL, upper.CL )) 

	names <- c()
	for (l in 1:factno) {
		names[l] <- paste(unique (strsplit(selectedEffect, "*",fixed = TRUE)[[1]])[l], " Level", sep = "")
	}
	names[factno+1]<-"Mean"
	names[factno+2]<-"Lower"
	names[factno+3]<-"Upper"

	colnames(graphdata)<-names

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
	#Gr_line <-"black"
	Line_size <- Line_size2

	#Code for LS MEans plot
	meanPlotq <- sub(".html", "meanplotq.jpg", htmlFile)
	jpeg(meanPlotq,width = jpegwidth, height = jpegheight, quality = 100)

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
#===================================================================================================================
#Table of back transformed means
#===================================================================================================================
if(GeomDisplay == "Y" && showLSMeans =="Y" && (responseTransform =="Log10"||responseTransform =="Loge")) {

	#STB May 2012 Updating "least square (predicted) means"
	CITitle2<-paste("Table of the back-transformed geometric means with ",(sig*100),"% confidence intervals",sep="")
	HTML.title(CITitle2, HR=2, align="left")

	#Calculate LS Means Table
	x<-summary(tabs)
	x<-na.omit(x)

	if (responseTransform =="Log10") {
		x$Mean <-format(round(10^(x$lsmean), 3), nsmall=3, scientific=FALSE) 
		for (i in 1:dim(x)[1]) {
			x$Lower[i] <- format(round(10^(x$lsmean[i]  - x$SE[i]*qt(sig2, x$df[i])), 3), nsmall=3, scientific=FALSE)  
			x$Upper[i] <- format(round(10^(x$lsmean[i]  + x$SE[i]*qt(sig2, x$df[i])), 3), nsmall=3, scientific=FALSE)  
		}
	}

	if (responseTransform =="Loge") {
		x$Mean <-format(round(exp(x$lsmean), 3), nsmall=3, scientific=FALSE) 
		for (i in 1:dim(x)[1]) {
			x$Lower[i] <- format(round(exp(x$lsmean[i]  - x$SE[i]*qt(sig2, x$df[i])), 3), nsmall=3, scientific=FALSE)  
			x$Upper[i] <- format(round(exp(x$lsmean[i]  + x$SE[i]*qt(sig2, x$df[i])), 3), nsmall=3, scientific=FALSE)  
		}
	}

	names <- c("")
	for (l in 1:factno)
	{
		names[l+1] <- paste(unique (strsplit(selectedEffect, "*",fixed = TRUE)[[1]])[l], " ", sep = "")
	}

	x2<-subset(x, select = -c(SE, df,lsmean, lower.CL, upper.CL )) 

	observ <- data.frame(c(1:dim(x)[1]))
	x2 <- cbind(observ, x2)

	names[1]<-"Mean ID"
	names[factno+2]<-"Mean"
	names[factno+3]<-paste("Lower ",(sig*100),"% CI",sep="")
	names[factno+4]<-paste("Upper ",(sig*100),"% CI",sep="")

	colnames(x2)<-names
	HTML(x2, classfirstline="second", align="left", row.names = "FALSE")
}

#===================================================================================================================
#All Pairwise tests
#===================================================================================================================
#All pairwise tests
if(allPairwiseTest != "NULL") {

	#All pairwise test options
	allPairwiseTestText = allPairwiseTest
	if(allPairwiseTestText=="Unadjusted (LSD)") {
		allPairwiseTest= "none"
	} else if (allPairwiseTestText=="Holm") {
		allPairwiseTest= "holm"
	} else if (allPairwiseTestText=="Hochberg") {
		allPairwiseTest= "hochberg"
	} else if (allPairwiseTestText=="Hommel") {
		allPairwiseTest= "hommel"
	} else if (allPairwiseTestText=="Bonferroni") {
		allPairwiseTest= "bonferroni"
	} else if (allPairwiseTestText=="Benjamini-Hochberg") {
		allPairwiseTest= "BH"
	}

	if (allPairwiseTest== "none") {
		add<-paste(c("All pairwise comparisons without adjustment for multiplicity (LSD test)"))
	} else {
		add<-paste("All pairwise comparisons using ", allPairwiseTestText, "'s procedure", sep="")
	}
	HTML.title(add, HR=2, align="left")

	#Go back to old method
	#mult<-glht(lm(model, data=statdata, na.action = na.omit), linfct=lsm(eval(parse(text = paste("pairwise ~",selectedEffect)))))
	mult<-glht(lm(effectModel, data=statdata, na.action = na.omit), linfct=mcp(mainEffect="Tukey"))
	multci<-confint(mult, level=sig, calpha = univariate_calpha())

	if (allPairwiseTest== "Tukey") {
		set.seed(3)	
		#Does work - go back to old method
		#mult<-glht(lm(model, data=statdata, na.action = na.omit),  linfct=lsm(eval(parse(text = paste("pairwise ~",selectedEffect)))))
		mult<-glht(lm(effectModel, data=statdata, na.action = na.omit), linfct=mcp(mainEffect="Tukey"))
		multci<-confint(mult, level=sig, calpha = univariate_calpha())
		multp<-summary(mult)
	} else {
		multp<-summary(mult, test=adjusted(allPairwiseTest))
	}
	pvals<-multp$test$pvalues
	sigma<-multp$test$sigma
	tablen<-length(unique(rownames(multci$confint)))
	tabs<-matrix(nrow=tablen, ncol=5)

	for (i in 1:tablen) {
		#STB Dec 2011 increasing means to 3dp
		tabs[i,1]=format(round(multci$confint[i], 3), nsmall=3, scientific=FALSE)
	}
	for (i in 1:tablen) {
		tabs[i,2]=format(round(multci$confint[i+tablen], 3), nsmall=3, scientific=FALSE)
	}
	for (i in 1:tablen) {
		tabs[i,3]=format(round(multci$confint[i+2*tablen], 3), nsmall=3, scientific=FALSE)
	}
	for (i in 1:tablen) {
		tabs[i,4]=format(round(sigma[i], 3), nsmall=3, scientific=FALSE)
	}
	for (i in 1:tablen) {
		tabs[i,5]=format(round(pvals[i], 4), nsmall=4, scientific=FALSE)
	}
	for (i in 1:tablen)  {
		if (pvals[i]<0.0001)  {
			#STB March 2011 - formatting p-values p<0.0001
			#tabs[i,5]<-0.0001
			tabs[i,5]=format(round(0.0001, 4), nsmall=4, scientific=FALSE)
			tabs[i,5]<- paste("<",tabs[i,5])
		}
	}
	
	rows<-rownames(multci$confint)
	rows<-sub(" - "," vs. ", rows, fixed=TRUE)

	lowerCI<-paste("   Lower ",(sig*100),"% CI   ",sep="")
	upperCI<-paste("   Upper ",(sig*100),"% CI   ",sep="")

	tabls<-cbind(rows, tabs)
	colnames(tabls)<-c("Comparison", "Difference", lowerCI, upperCI, "Std error", "p-value")
	HTML(tabls, classfirstline="second", align="left", row.names = "FALSE")
	rownames(tabls) <- rows
#===================================================================================================================
	#STB March 2014 - Creating a dataset of p-values

	comparisons <-paste(direct, "/Comparisons.csv", sep = "")
	for (i in 1:tablen) {
		tabs[i,5]=pvals[i]
	}
	tabsxx<- data.frame(tabs[,5])
	for (i in 1:20) {
		rows<-sub(","," and ", rows, fixed=TRUE)
	}	
	tabsxx<-cbind(rows, tabsxx)
	colnames(tabsxx)<-c("Comparison", "p-value")
	row.names(tabsxx) <- seq(nrow(tabsxx)) 

#===================================================================================================================
	#Conclusion
	add<-paste(c("Conclusion"))
	inte<-1
	for(i in 1:(dim(tabls)[1])) {
		if (tabls[i,6]<= (1-sig)) {
			if (inte==1) {
				inte<-inte+1
				add<-paste(add, ": The following pairwise tests are statistically significantly different at the  ", 100*(1-sig), "% level: ", rownames(tabls)[i], sep="")
			} else {
				inte<-inte+1
				add<-paste(add, ", ", rownames(tabls)[i], sep="")
			}
		} 
	}
	if (inte==1) {
		if (tablen >1) {
			add<-paste(add, ": There are no statistically significant pairwise differences.", sep="")
		} else {
			add<-paste(add, ": The pairwise difference is not statistically significant.", sep="")
		}
	} else {
		add<-paste(add, ". ", sep="")
	}
	HTML(add, align="left")

#===================================================================================================================
	if (allPairwiseTest == "Tukey") {
		HTML("Warning: The results of Tukey's procedure are approximate if the sample sizes are not equal.", align="left")
	}
#	if(length(grep("\\*", effectModel)) == 0 && length(grep("\\+", effectModel)) == 0 && length(grep("\\+", model)) == 1)  {
#		add2<-paste(c(" "), " ", sep="")
#		HTML.title(add2, HR=0, align="left")
#	} else	if (length(grep("\\*", model)) == 0 && length(grep("\\+", effectModel)) == 0 && length(grep("\\+", model)) == 0) {
#		add2<-paste(c(" "), " ", sep="")
#		HTML.title("<bf> ", HR=2, align="left")
#		HTML.title(add2, HR=0, align="left")
#	} 
	if (noeffects>testeffects)  {
		HTML("Warning: It is not advisable to draw statistical inferences about a factor/interaction in the presence of a significant higher-order interaction involving that factor/interaction. In the above table we have assumed that certain higher-order interactions are not significant and have removed them from the statistical model, see log for more details.", align="left")
	}

	if (tablen >1) {
		if (allPairwiseTest == "none") {
			HTML("Warning: As these tests are not adjusted for multiplicity there is a risk of generating false positive results. Only use the pairwise tests you planned to make a-priori, these are the so called planned comparisons, see Snedecor and Cochran (1989).", align="left")
		} else {
			HTML("Warning: This procedure makes an adjustment assuming you want to make all pairwise comparisons. If this is not the case then these tests may be unduly conservative. You may wish to use planned comparisons (using unadjusted p-values) instead, see Snedecor and Cochran (1989), or make a manual adjustment to the unadjusted p-values using the ", branding , " P-value Adjustment module.", align="left")
		}
	}
	if (allPairwiseTest!= "none") {
		HTML("Note: The confidence intervals quoted are not adjusted for multiplicity.", align="left")
	}
} 

#===================================================================================================================
#Back transformed geometric means table 
#===================================================================================================================
if(allPairwiseTest != "NULL") {
	if(GeomDisplay == "Y" && (responseTransform =="Log10"||responseTransform =="Loge")) {
		HTML.title("All pairwise comparisons as back-transformed ratios", HR=2, align="left")
		HTML("As the response was log transformed prior to analysis the differences between the least square (predicted) means are presented on the log scale. These results can be back-transformed onto the original scale, where differences on the log scale become ratios when back-transformed.", align="left")

		#Creating the table
		tabsx<-matrix(nrow=tablen, ncol=3)
		if (responseTransform =="Log10") {
			for (i in 1:tablen) {
				tabsx[i,1]=format(round(10^(multci$confint[i]), 3), nsmall=3, scientific=FALSE)
			}
			for (i in 1:tablen) {
				tabsx[i,2]=format(round(10^(multci$confint[i+tablen]), 3), nsmall=3, scientific=FALSE)
			}
			for (i in 1:tablen) {
				tabsx[i,3]=format(round(10^(multci$confint[i+2*tablen]), 3), nsmall=3, scientific=FALSE)
			}
		}
		if (responseTransform =="Loge") {
			for (i in 1:tablen) {
				tabsx[i,1]=format(round(exp(multci$confint[i]), 3), nsmall=3, scientific=FALSE)
			}
			for (i in 1:tablen) {
				tabsx[i,2]=format(round(exp(multci$confint[i+tablen]), 3), nsmall=3, scientific=FALSE)
			}
			for (i in 1:tablen) {
				tabsx[i,3]=format(round(exp(multci$confint[i+2*tablen]), 3), nsmall=3, scientific=FALSE)
			}
		}

		rowsx<-rownames(multci$confint)
		rowsx<-sub(" - "," / ", rowsx, fixed=TRUE)

		lowerCI<-paste("   Lower ",(sig*100),"% CI   ",sep="")
		upperCI<-paste("   Upper ",(sig*100),"% CI   ",sep="")
		tablsx<-cbind(rowsx, tabsx)
		colnames(tablsx)<-c("Comparison","Ratio", lowerCI, upperCI)
		HTML(tablsx, classfirstline="second", align="left", row.names = "FALSE")
	}
}

#===================================================================================================================
#Back to control comparisons
#===================================================================================================================
backToControlTestText <- backToControlTest

if(backToControlTestText=="Unadjusted (LSD)") {
	backToControlTest= "none"
} else if (backToControlTestText=="Holm") {
	backToControlTest= "holm"
} else if (backToControlTestText=="Hochberg") {
	backToControlTest= "hochberg"
} else if (backToControlTestText=="Hommel") {
	backToControlTest= "hommel"
} else if (backToControlTestText=="Bonferroni") {
	backToControlTest= "bonferroni"
} else if (backToControlTestText=="Benjamini-Hochberg") {
	backToControlTest= "BH"
}

#===================================================================================================================
#All to one comparisons
if(backToControlTest != "NULL") {

	#Title
	if (backToControlTest== "none") {
		add<-paste(c("All to one comparisons without adjustment for multiplicity (LSD test)"))
	} else {
		add<-paste("All to one comparisons using ", backToControlTestText, "'s procedure", sep="")
	}
	HTML.title(add, HR=2, align="left")

	#Creating the table of unadjusted p-values
	#Generate all pairwise comparisons, unadjusted for multiplicity

	#Go back to old version
	#mult<-glht(lm(model, data=statdata, na.action = na.omit), linfct=lsm(eval(parse(text = paste("pairwise ~",selectedEffect)))))
	mult<-glht(lm(effectModel, data=statdata, na.action = na.omit), linfct=mcp(mainEffect="Tukey"))
	multci<-confint(mult, level=sig, calpha = univariate_calpha())
	multp<-summary(mult, test=adjusted("none"))

	#Creating a matrix of the differences
	comps<-c(rownames(multci$confint))
	diffz <-matrix(nrow=length(comps), ncol=2)
	for (g in 1:length(comps)) {
		comps2<-unlist(strsplit(comps[g]," - " ))[1]
		comps3<-unlist(strsplit(comps[g]," - " ))[2]
		diffz[g,1] = comps2
		diffz[g,2] = comps3
	}

	#Creating the unadjusted full column
	pvals<-multp$test$pvalues
	sigma<-multp$test$sigma
	tstats<-Mod(as.numeric(multp$test$tstat))
	tablen<-length(unique(rownames(multci$confint)))
	tabs<-data.frame(nrow=tablen, ncol=15)

	for (i in 1:tablen) {
		tabs[i,1]=multci$confint[i]
	}
	for (i in 1:tablen) {
		tabs[i,2]=multci$confint[i+tablen]
	}
	for (i in 1:tablen) {
		tabs[i,3]=multci$confint[i+2*tablen]
	}
	for (i in 1:tablen) {
		tabs[i,4]=sigma[i]
	}
	for (i in 1:tablen) {
		tabs[i,5]=pvals[i]
	}
	for (i in 1:tablen) {
		tabs[i,6]= diffz[i,1]
	}
	for (i in 1:tablen) {
		tabs[i,7]= diffz[i,2]
	}
	for (i in 1:tablen) {
		tabs[i,8]= tstats[i]
	}
	tabs2<- tabs

	for ( i in 1:tablen) {
		if (tabs2[i,6] == cntrlGroup) {
			tabs2[i,9] = -1*tabs2[i,1]
			tabs2[i,10] = -1*tabs2[i,3]
			tabs2[i,11] = -1*tabs2[i,2]
			tabs2[i,12] = tabs2[i,7]
			tabs2[i,13] = tabs2[i,6]
		} else {
			tabs2[i,9] = tabs2[i,1]
			tabs2[i,10] = tabs2[i,2]
			tabs2[i,11] = tabs2[i,3]	
			tabs2[i,12] = tabs2[i,6]
			tabs2[i,13] = tabs2[i,7]
		}
	}

	for ( i in 1:tablen) {
		tabs2[i,14] = paste(tabs2[i,12],  " vs. ", tabs2[i,13], sep = "")
	}

	#Subsetting to only the comparisons to control
	tabs3<-subset(tabs2, tabs2$V13 == cntrlGroup)

	if (backToControlTest== "Dunnett") { 
		ntrgps<-length(unique(eval(parse(text = paste("statdata$",selectedEffect)))))-1

		if (ntrgps!=1)	{

			#Dunnetts code
			# remove blank rows form the data, then calculate number of groups
			nallgps<-length(unique(eval(parse(text = paste("statdata$",selectedEffect)))))
			samplesize<-c(1:nallgps)
			
			#calculate the sample sizes
			for (i in 1:nallgps) {
				samplesize[i]<- sum(eval(parse(text = paste("statdata$",selectedEffect))) == levels(eval(parse(text = paste("statdata$",selectedEffect))))[i])
			}

			# calculate the total number of obs and the DF for the Dunnetts test (get this from ANOVA?)
			totalobs<-sum(samplesize)
			dfree<-df.residual(lm(model, data=statdata, na.action = na.omit))

			# calculation of correlation coefficient (according to Dunnett)
			cormat<-diag(ntrgps)
			for (j in 1:(ntrgps-1)) {
				for (k in (j+1):ntrgps) {
					cormat[j,k]<-1/(sqrt(((samplesize[1]/samplesize[j+1])+1)*((samplesize[1]/samplesize[k+1])+1)))
					cormat[k,j]<-cormat[j,k]	
				}
			}

			#call to get critical value
			critval95<-qmvt(0.95, df = dfree, tail = "both", corr=cormat, abseps=0.0001)[1]
			critval99<-qmvt(0.99, df = dfree, tail = "both", corr=cormat, abseps=0.0001)[1]
			critval999<-qmvt(0.999, df = dfree, tail = "both", corr=cormat, abseps=0.0001)[1]
			critvalsig<-qmvt(sig, df = dfree, tail = "both", corr=cormat, abseps=0.0001)[1]
			pvals <-tabs3$V5
			tstats<-tabs3$V8
			sigma <-tabs3$V4

			#Calculate p-value
			dunnett <- function(data,  group)  {
				pdunnett <- function(x, nallgps, dfree, cormat) {
					1-pmvt(lower=-x, upper=x, delta=numeric(nallgps-1), df=dfree, corr=cormat, abseps=0.00000001)
				}
				t<-tstats
				p <- sapply(t, pdunnett, nallgps, dfree, cormat)      
				return(p)
			}
			dunnp<-dunnett()
			tabs3$V15<-dunnp
			adjpval<-dunnp
		
		} else {
			tabs3$V15<-tabs3$V5
			adjpval<-tabs3$V5
		}
	} else {
		#Adjusting the p-values
		unadjpval<-tabs3$V5
		adjpval<-p.adjust(unadjpval, method = backToControlTest)
		tabs3$V15<-adjpval
	}

#===================================================================================================================
	#Creating final table
	tabs4<-data.frame()
	for ( i in 1:dim(tabs3)[1]) {
		tabs4[i,1]<-format(round(tabs3[i,9], 3), nsmall=3, scientific=FALSE)
		tabs4[i,2]<-format(round(tabs3[i,10], 3), nsmall=3, scientific=FALSE)
		tabs4[i,3]<-format(round(tabs3[i,11], 3), nsmall=3, scientific=FALSE)
		tabs4[i,4]<-format(round(tabs3[i,4], 3), nsmall=3, scientific=FALSE)
		tabs4[i,5]<-format(round(tabs3[i,15], 4), nsmall=4, scientific=FALSE)
	}

	for (i in 1:dim(tabs3)[1])  {
		if (adjpval[i]<0.0001)  {
			#STB March 2011 - formatting p-values p<0.0001
			#tabs4[i,5]<-0.0001
			tabs4[i,5]=format(round(0.0001, 4), nsmall=4, scientific=FALSE)
			tabs4[i,5]<- paste("<",tabs4[i,5])
		}
	}

	tabls<-cbind(tabs3$V14, tabs4)

	lowerCI<-paste("   Lower ",(sig*100),"% CI   ",sep="")
	upperCI<-paste("   Upper ",(sig*100),"% CI   ",sep="")
	#STB May 2012 correcting "SEM"
	colnames(tabls)<-c("Comparison", "Difference", lowerCI, upperCI, "Std error", "p-value")

	HTML(tabls, classfirstline="second", align="left", row.names = "FALSE")

#===================================================================================================================
	#Plot of the comparisons back to control
	CITitle<-paste("Plot of the comparisons between the predicted means with ",(sig*100),"% confidence intervals",sep="")
	HTML.title(CITitle, HR=2, align="left")

	#Code for LS MEans plot
	meanPlotqq <- sub(".html", "meanplotqq.jpg", htmlFile)
	jpeg(meanPlotqq,width = jpegwidth, height = jpegheight, quality = 100)

	#STB July2013
	plotFilepdf5qq <- sub(".html", "meanplotqq.pdf", htmlFile)
	dev.control("enable") 

	#Setting up the dataset
	graphdata<-data.frame(tabs4)
	graphdata$Mean<-as.numeric(graphdata$V1)
	graphdata$Lower<-as.numeric(graphdata$V2)
	graphdata$Upper<-as.numeric(graphdata$V3)
	graphdata$Group_IVSq_<-tabs3$V14
	Gr_intercept <- 0
	XAxisTitle <- "Comparison"
	YAxisTitle <- "Difference between the means"

	#GGPLOT2 code
	LSMPLOT_diff()
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
	#Conclusions
	add<-paste(c("Conclusion"))
	inte<-1

	for(i in 1:(dim(tabls)[1])) {
		if (tabls[i,6]<= (1-sig)) {
			if (inte==1) {
				inte<-inte+1
				add<-paste(add, ": The following pairwise tests are statistically significantly different at the  ", 100*(1-sig), "% level: ", rownames(tabls)[i], sep="")
			} else {
				inte<-inte+1
				add<-paste(add, ", ", rownames(tabls)[i], sep="")
			}
		} 
	}
	if (inte==1) {
		if (tablen >1) {
			add<-paste(add, ": There are no statistically significant pairwise differences.", sep="")
		} else {
			add<-paste(add, ": The pairwise difference is not statistically significant.", sep="")
		}
	} else {
		add<-paste(add, ". ", sep="")
	}
	HTML(add, align="left")

	if (noeffects>testeffects)  {
		HTML("Warning: It is not advisable to draw statistical inferences about a factor/interaction in the presence of a significant higher-order interaction involving that factor/interaction. In the above table we have assumed that certain higher order interactions are not significant, see log for more details.", align="left")
	}
	if (tablen > 1) {
		if (backToControlTest== "none") {
			HTML("Warning: As these tests are not adjusted for multiplicity there is a risk of generating false positive results. Only use the pairwise tests you planned to make a-priori, these are the so called planned comparisons, see Snedecor and Cochran (1989).", align="left")
		}
	} 
	if (backToControlTest != "none") {
		HTML("Note: The confidence intervals quoted are not adjusted for multiplicity.", align="left")
	}
}

#===================================================================================================================
#Back transformed geometric means table 
#===================================================================================================================
if(backToControlTest != "NULL" && GeomDisplay == "Y" && (responseTransform =="Log10"||responseTransform =="Loge")) {
	HTML.title("All to one comparisons as back-transformed ratios", HR=2, align="left")
	HTML("As the response was log transformed prior to analysis the differences between the least square (predicted) means are presented on the log scale. These results can be back-transformed onto the original scale, where differences on the log scale become ratios when back-transformed.", align="left")

#Creating final table
	tabs4x<-data.frame()

	if (responseTransform =="Log10") {
		for ( i in 1:dim(tabs3)[1]) {
			tabs4x[i,1]<-format(round(10^(tabs3[i,9]), 3), nsmall=3, scientific=FALSE)
			tabs4x[i,2]<-format(round(10^(tabs3[i,10]), 3), nsmall=3, scientific=FALSE)
			tabs4x[i,3]<-format(round(10^(tabs3[i,11]), 3), nsmall=3, scientific=FALSE)
		}
	}
	if (responseTransform =="Loge") {
		for ( i in 1:dim(tabs3)[1]) {
			tabs4x[i,1]<-format(round(exp(tabs3[i,9]), 3), nsmall=3, scientific=FALSE)
			tabs4x[i,2]<-format(round(exp(tabs3[i,10]), 3), nsmall=3, scientific=FALSE)
			tabs4x[i,3]<-format(round(exp(tabs3[i,11]), 3), nsmall=3, scientific=FALSE)
		}
	}

	tabs3$V14<-sub(" vs. "," / ", tabs3$V14, fixed=TRUE)

	lowerCI<-paste("Lower",(sig*100),"% CI",sep="")
	upperCI<-paste("Upper",(sig*100),"% CI",sep="")

	tablsx <- cbind(tabs3$V14, tabs4x)
	#STB May 2012 correcting "SEM"
	colnames(tablsx)<-c("Comparison", "Ratio", lowerCI, upperCI)
	
	HTML(tablsx, classfirstline="second", align="left", row.names = "FALSE")
	
#===================================================================================================================
	#Plot of the comparisons back to control
	CITitle<-paste("Plot of the comparisons between the back-transformed geometric  means with ",(sig*100),"% confidence intervals",sep="")
	HTML.title(CITitle, HR=2, align="left")

	#Code for LS MEans plot
	meanPlotqs <- sub(".html", "meanplotqs.jpg", htmlFile)
	jpeg(meanPlotqs,width = jpegwidth, height = jpegheight, quality = 100)

	#STB July2013
	plotFilepdf5qs <- sub(".html", "meanplotqs.pdf", htmlFile)
	dev.control("enable") 

	#Setting up the dataset
	graphdata<-data.frame(tabs4x)
	graphdata$Mean<-as.numeric(graphdata$V1)
	graphdata$Lower<-as.numeric(graphdata$V2)
	graphdata$Upper<-as.numeric(graphdata$V3)
	graphdata$Group_IVSq_<-tabs3$V14
	Gr_intercept <- 1
	XAxisTitle <- "Comparison"
	YAxisTitle <- "Ratio of differences between the geometric means"

	#GGPLOT2 code
	LSMPLOT_diff()

	void<-HTMLInsertGraph(GraphFileName=sub("[A-Z0-9a-z,:,\\\\]*App_Data[\\\\]","", meanPlotqs), Align="left")

	#STB July2013
	if (pdfout=="Y") {
			pdf(file=sub("[A-Z0-9a-z,:,\\\\]*App_Data[\\\\]","", plotFilepdf5qs), height = pdfheight, width = pdfwidth) 
			dev.set(2) 
			dev.copy(which=3) 
			dev.off(2)
			dev.off(3)
			pdfFile_5qs<-sub("[A-Z0-9a-z,:,\\\\]*App_Data[\\\\]","",plotFilepdf5qs)
			linkToPdf5qs <- paste ("<a href=\"",pdfFile_5qs,"\">Click here to view the PDF of the plot of comparisons back to control</a>", sep = "")
			HTML(linkToPdf5qs)
	}
}

#===================================================================================================================
#Analysis description
#===================================================================================================================
HTML.title("Analysis description", HR=2, align="left")

add<-c("The data were analysed using a ")

if (notreatfactors==1)  {
	if(FirstCatFactor != "NULL") {
		add<-paste(add, "1-way ANCOVA approach, with treatment factor ", treatFactors, sep="")
	} else {
		add<-paste(add, "1-way ANOVA approach, with treatment factor ", treatFactors, sep="")
	}
} else {
	add<-paste(add, notreatfactors, sep="")
	if(FirstCatFactor != "NULL") {
		add<-paste(add, "-way ANCOVA approach, with ", sep="")
	} else {
		add<-paste(add, "-way ANOVA approach, with ", sep="")
	}
	for (i in 1:notreatfactors) {
		if (i<notreatfactors-1)	{
			add<-paste(add, txtexpectedChanges[i+1], ", ", sep="")
		} else 	if (i<notreatfactors) {
			add<-paste(add, txtexpectedChanges[i+1], " and ", sep="")
		} else if (i==notreatfactors) {
			add<-paste(add, txtexpectedChanges[i+1], " as treatment factors", sep="")
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
				add<-paste(add, txtexpectedblockChanges[i+1], ", ", sep="")
			} else	if (i<noblockfactors) {
				add<-paste(add, txtexpectedblockChanges[i+1], " and ", sep="")
			} else if (i==noblockfactors) {
				add<-paste(add, txtexpectedblockChanges[i+1], sep="")
			}
		}
		add<-paste(add, " as blocking factors", sep="")
	}
}
if (FirstCatFactor == "NULL") {
	add<-paste(add, ". ", sep="")
} else if(FirstCatFactor != "NULL")	{
	add<-paste(add, " and  ", unlist(strsplit(covariateModel, "~"))[2], " as the covariate. ", sep="")
}

if (allPairwiseTest== "none" | backToControlTest== "none") {
	#STB May 2012 Updating "Selected"
	add<-paste(add, "This was followed by planned comparisons of the predicted means to compare the levels of the ", selectedEffect , sep="")
	if (factno == 1) {
		add<-paste(add, " factor. ", sep="")
	} else {
		add<-paste(add, " interaction. ", sep="")
	}
}

if (backToControlTest!= "NULL" & backToControlTest!= "none") {
	add<-paste(add, "This was followed by comparisons of the predicted means of the ", selectedEffect , " factor back to the control group mean using ", backToControlTestText , "'s procedure. ", sep="")
}

if (allPairwiseTest!= "NULL" & allPairwiseTest!= "none") {
	#STB May 2012 Updating "Selected"
	add<-paste(add, "This was followed by all pairwise comparisons between the predicted means of the ", selectedEffect , sep="")
	if (factno == 1) {
		add<-paste(add, " factor ", sep="")
	} else {
		add<-paste(add, " interaction ", sep="")
	}
	add<-paste(add, " using ", allPairwiseTestText , "'s procedure. ", sep="")
}

if (responseTransform != "None") {
	add<-paste(add, " The response was ", responseTransform, " transformed prior to analysis to stabilise the variance. ", sep="")
}

HTML(add, align="left")

#===================================================================================================================
#Create file of comparisons
#===================================================================================================================
#if(allPairwiseTest != "NULL") {
#	write.csv(tabsxx, file = sub("[A-Z0-9a-z,:,\\\\]*App_Data[\\\\]","", comparisons), row.names=FALSE)
#}

#===================================================================================================================
#References
#===================================================================================================================
Ref_list<-R_refs()

#Bate and Clark comment
HTML(refxx,  align="left")	

HTML.title("Statistical references", HR=2, align="left")
HTML(Ref_list$BateClark_ref,  align="left")

if(showANOVA=="Y") {
	HTML("<bf> Armitage P, Matthews JNS and Berry G. (2001). Statistical Methods in Medical Research. 4th edition; John Wiley & Sons. New York.",  align="left")
}

if (allPairwiseTest== "BH" | backToControlTest=="BH") {
	HTML("<bf>Benjamini Y and Hochberg Y. (1995). Controlling the false discovery rate: a practical and powerful approach to multiple testing. Journal of the Royal Statistical Society Series B, 57, 289-300. ",  align="left")
}

if (allPairwiseTest== "bonferroni" | backToControlTest=="bonferroni") {
	HTML("<bf>Bonferroni CE. (1936). Teoria statistica delle classi e calcolo delle probabilita. Pubblicazioni del R Istituto Superiore di Scienze Economiche e Commerciali di Firenze, 8, 3-62.",  align="left")
} 

if (allPairwiseTest== "Tukey") {	
	HTML("<bf>Braun HI, ed. (1994). The collected works of John W. Tukey. Vol. VIII: Multiple comparisons:1948-1983. New York: Chapman and Hall.",  align="left")
} 

if (backToControlTest== "Dunnett") {
	HTML("<bf>Dunnett CW. (1955). A multiple comparison procedure for comparing several treatments with a control. Journal of the American Statistical Association, 50, 1096-1121.",  align="left")
}

if (allPairwiseTest== "hochberg" | backToControlTest=="hochberg") {
	HTML("<bf>Hochberg Y. (1988). A sharper Bonferroni procedure for multiple tests of significance. Biometrika, 75, 800-803.",  align="left")
} 

if (allPairwiseTest== "holm" | backToControlTest=="holm") {
	HTML("<bf>Holm S. (1979). A simple sequentially rejective multiple test procedure. Scandinavian Journal of Statistics, 6, 65-70.",  	align="left")
} 

if (allPairwiseTest== "hommel" | backToControlTest=="hommel") {
	HTML("<bf>Hommel G. (1988). A stagewise rejective multiple test procedure based on a modified Bonferroni test. Biometrika, 75, 383-386.",  	align="left")
}

if(FirstCatFactor != "NULL") {
	HTML("<bf> Morris TR. (1999). Experimental Design and Analysis in Animal Sciences. CABI publishing. Wallingford, Oxon (UK).",  align="left")
}

if (allPairwiseTest != "NULL" | backToControlTest !="NULL") {
	if (tablen >1 & (allPairwiseTest== "none" | backToControlTest=="none"|allPairwiseTest!= "NULL" | backToControlTest=="NULL") ) 	{
		HTML("<bf>Snedecor GW and Cochran WG. (1989). Statistical Methods. 8th edition;  Iowa State University Press, Iowa, USA.",  align="left")
	}
}

HTML.title("R references", HR=2, align="left")
HTML(Ref_list$R_ref ,  align="left")
HTML(Ref_list$GGally_ref,  align="left")
HTML(Ref_list$RColorBrewers_ref,  align="left")
HTML(Ref_list$GGPLot2_ref,  align="left")
HTML(Ref_list$ggrepel_ref,  align="left")
HTML(Ref_list$reshape_ref,  align="left")
HTML(Ref_list$plyr_ref,  align="left")
HTML(Ref_list$scales_ref,  align="left")
HTML(Ref_list$car_ref,  align="left")
HTML(Ref_list$R2HTML_ref,  align="left")
HTML(Ref_list$PROTO_ref,  align="left")
HTML(Ref_list$LSMEANS_ref,  align="left")
HTML(Ref_list$multcomp_ref,  align="left")


#===================================================================================================================
#Show dataset
#===================================================================================================================


if (showdataset=="Y")
{
	statdata_temp<-subset(statdata, select = -c(mainEffect, scatterPlotColumn,catfact))

	observ <- data.frame(c(1:dim(statdata_temp)[1]))
	colnames(observ) <- c("Observation")
	statdata_temp2 <- cbind(observ, statdata_temp)

	HTML.title("Analysis dataset", HR = 2, align = "left")
    	HTML(statdata_temp2, classfirstline = "second", align = "left", row.names = "FALSE")

}

#===================================================================================================================
#Show arguments
#===================================================================================================================
HTML.title("Analysis options", HR=2, align="left")

HTML(paste("Response variable: ", unlist(strsplit(Args[4],"~"))[1], sep=""), align="left")

if (responseTransform != "None") {
	HTML(paste("Response transformation: ", responseTransform, sep=""), align="left")
}

HTML(paste("Treatment factors: ", treatFactors, sep=""), align="left")

if (blockFactors != "NULL") {
	HTML(paste("Blocking factor(s) selected: ", blockFactors, sep=""), align="left")
}

if(FirstCatFactor != "NULL") {
	HTML(paste("Covariate: ", unlist(strsplit(covariateModel, "~"))[2], sep=""), align="left")
}

if (FirstCatFactor != "NULL" && covariateTransform != "None") {
	HTML(paste("Covariate transformation: ", covariateTransform, sep=""), align="left")
}

if (FirstCatFactor != "NULL" ) {
	HTML(paste("Categorisation factor used on covariate scatterplots: ", FirstCatFactor, sep=""), align="left")
}

HTML(paste("Model fitted: ", unlist(strsplit(Args[4],"~"))[-1], sep=""), align="left")
HTML(paste("Output ANOVA table (Y/N): ", showANOVA, sep=""), align="left")
HTML(paste("Output predicted vs. residual plot (Y/N): ", showPRPlot, sep=""), align="left")
HTML(paste("Output normal probability plot (Y/N): ", showNormPlot, sep=""), align="left")
HTML(paste("Show Least Square predicted means (Y/N): ", showLSMeans, sep=""), align="left")

if (showLSMeans != "N" && (Args[19] != "NULL" | backToControlTest != "NULL" ) ) {
	HTML(paste("Selected effect (for pairwise mean comparisons): ", selectedEffect, sep=""), align="left")
}

if (Args[19] != "NULL") {
	HTML(paste("All pairwise tests procedure: ", Args[19], sep=""), align="left")
}

if (backToControlTest != "NULL" && backToControlTest != "none") {
	HTML(paste("Comparisons back to control procedure: ", backToControlTest, sep=""), align="left")
}

if (backToControlTest == "none") {
	HTML(paste("Comparisons back to control procedure: Unadjusted (LSD)"), align="left")
}

if ( backToControlTest != "NULL" ) {
	HTML(paste("Control group to compare back to: ", cntrlGroup, sep=""), align="left")
}
HTML(paste("Significance level: ", 1-sig, sep=""), align="left")




