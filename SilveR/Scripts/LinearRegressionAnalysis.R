#===================================================================================================================
#R Libraries

suppressWarnings(library(car))
suppressWarnings(library(R2HTML))

#===================================================================================================================
# retrieve args
Args <- commandArgs(TRUE)

#Read in data
statdata <- read.csv(Args[3], header=TRUE, sep=",")

#Copy Args
model <- Args[4]
covariatelist <- Args[5]
responseTransform <- Args[6]
covariateTransform <- Args[7]
FirstCatFactor <- Args[8]
treatFactors <- Args[9]
contFactors <- Args[10]
contFactorTransform <- Args[11]
blocklist <- Args[12]
showANOVA <- Args[13]
showCoefficients <- Args[14]
showAdjustedRSquared <- Args[15]
sig <- 1 - as.numeric(Args[16])
showPRPlot <- Args[17]
showNormPlot <- Args[18]
cooksDistancePlot <- Args[19]
leveragesPlot <- Args[20]

#source(paste(getwd(),"/Common_Functions.R", sep=""))

#Print args
if (Diplayargs == "Y"){
	print(Args)
}

#===================================================================================================================
#Setup the html file and associated css file
htmlFile <- sub(".csv", ".html", Args[3]); #determine the file name of the html file
HTMLSetFile(file=htmlFile) 
#cssFile <- "r2html.css"
#cssFile <- paste("'",cssFile,"'", sep="") #need to enclose in quotes when path has spaces in it
#HTMLCSS(CSSfile = cssFile)

#===================================================================================================================
#Set up parameters

#working directory
direct2<- unlist(strsplit(Args[3],"/"))
direct<-direct2[1]
for (i in 2:(length(direct2)-1)) {
	direct<- paste(direct, "/", direct2[i], sep = "")
}

#STB 14OCT2015
#Set contrast options for Marginal overall tests
options(contrasts=c(unordered="contr.sum", ordered="contr.poly"))

PlotCat <- "Not_Overall"
if (FirstCatFactor == "NULL") {
	statdata$FirstCatFactor <- "Overall"
	FirstCatFactor<-"FirstCatFactor"
	PlotCat <- "Overall"
}

#Graphics parameter setup
graphdata<-statdata
ReferenceLine <- "NULL"

if (covariatelist != "NULL") {
	Gr_palette<-palette_FUN(FirstCatFactor)
} 
Line_size2 <- Line_size
Labelz_IVS_ <- "N"

#The response
resp <- unlist(strsplit(model,"~"))[1] #get the response variable from the main model

#calculating number of covariates
nocovlist=0
if (covariatelist !="NULL") {
	tempcovChanges <-strsplit(covariatelist, ",")
	txtexpectedcovChanges <- c("")
	for(i in 1:length(tempcovChanges[[1]]))  {
		txtexpectedcovChanges [length(txtexpectedcovChanges )+1]=(tempcovChanges[[1]][i]) 
	}
	covlistsep <- txtexpectedcovChanges[-1]
	nocovlist<-length(covlistsep)
}

#calculating number of block factors
noblockfactors=0
if (blocklist !="NULL") {
	tempblockChanges <-strsplit(blocklist, ",")
	blocklistx <- c("")
	for(i in 1:length(tempblockChanges[[1]]))  {
		blocklistx [length(blocklistx )+1]=(tempblockChanges[[1]][i]) 
	}
	blocklistsep <- blocklistx[-1]
	noblockfactors<-length(blocklistsep)
}

#calculating number of treatment factors
notreatfactors=0
if (treatFactors !="NULL") {
	tempChanges <-strsplit(treatFactors, ",")
	TreatList <- c("")
	for(i in 1:length(tempChanges[[1]]))  { 
		TreatList [length(TreatList )+1]=(tempChanges[[1]][i]) 
	}
	notreatfactors<-length(TreatList)-1
	TreatmentsList<-TreatList[-1]

	statdata$scatterPlotColumn<-eval(parse(text = paste("statdata$",TreatmentsList[1])))

	if(notreatfactors>1) {
		for (k in 2:notreatfactors) {
			statdata$scatterPlotColumn<- paste(statdata$scatterPlotColumn , eval(parse(text = paste("statdata$",TreatmentsList[k]))))
		}
	}
}

#calculating number of continuous factors
tempcontChanges <-strsplit(contFactors, ",")
txtexpectedcontChanges <- c("")
for(i in 1:length(tempcontChanges[[1]]))  { 
	txtexpectedcontChanges [length(txtexpectedcontChanges )+1]=(tempcontChanges[[1]][i]) 
}
nocontfactors<-length(txtexpectedcontChanges)-1
ContinuousList <- txtexpectedcontChanges[-1]

#Testing the factorial combinations
intindex<-length(unique(statdata$scatterPlotColumn))
ind<-1

if (treatFactors !="NULL") {
	for (i in 1:notreatfactors) {
		ind=ind*length(unique(eval(parse(text = paste("statdata$",TreatList[i+1])))))
	}

	if(intindex != ind) {
		HTML("Unfortunately not all combinations of the levels of the treatment factors are present in the experimental design. We recommend you manually create a new factor corresponding to the combinations of the levels of the treatment factors.", align="left")
		quit()
	}
}

#replace illegal characters in variable names
if(covariatelist != "NULL") {
	XAxisTitleCov<-covlistsep
	for (i in 1: nocovlist) {
		XAxisTitleCov[i]<-namereplace(XAxisTitleCov[i])
	}
}

#replace illegal characters in variable names
YAxisTitle <-resp
for (i in 1:10) {
	YAxisTitle<-namereplace(YAxisTitle)
}
LS_YAxisTitle<-YAxisTitle

#Creating the model
model<- paste(resp , "~" )

if(covariatelist != "NULL") {
	for (i in 1:nocovlist) {
		model<- paste(model ,  covlistsep[i], "+")
	} 
}
model<- paste(model , ContinuousList[1])


if (nocontfactors > 1) {
	for (x in 2:nocontfactors) {
		model <- paste(model, "*", ContinuousList[x])
	}
}

if (notreatfactors > 0) {
	for (y in 1:notreatfactors) {
		model <- paste(model, "*", TreatmentsList[y])
	}
}

if (noblockfactors > 0) {
	for (z in 1:noblockfactors) {
		model <- paste(model, "+", blocklistsep[z])
	}
}

threewayfull<-lm(model, data=statdata, na.action = na.omit)

#===================================================================================================================
#Titles and description
#===================================================================================================================
#Output HTML header
Title <-paste(branding, " Linear Regression Analysis", sep="")
HTML.title(Title, HR = 1, align = "left")

#Response
title<-c("Response")
if(covariatelist != "NULL") {
	title<-paste(title, " and covariate", sep="")
}
HTML.title(title, HR=2, align="left")

add<-paste(c("The  "), resp, " response is currently being analysed by the Linear Regression Analysis module", sep="")

if(covariatelist != "NULL") {
	if (nocovlist == 1) {
		add<-paste(add, ", with ", covlistsep[1] , " fitted as a covariate.", sep="")
	} 
	if (nocovlist == 2) {
		add<-paste(add, ", with ", covlistsep[1] , " and ", covlistsep[2] ," fitted as covariates.", sep="")
	}
	if (nocovlist > 2) {	
		add<-paste(add, ", with ", sep="")	
		for (i in 1: (nocovlist -2)) {
		add <- paste (add, covlistsep[i],  ", " , sep="")
		}
		add<-paste(add, covlistsep[(nocovlist -1)] , " and ", covlistsep[nocovlist] , " fitted as covariates.", sep="")
	}
} else {
	add<-paste(add, ".", sep="")
}

if (responseTransform != "None") {
	add<-paste(add, c("The response has been "), responseTransform, " transformed prior to analysis.", sep="")
}

if (covariatelist !="NULL" && covariateTransform != "None") {
	if (nocovlist == 1) {
		add<-paste(add, c("The covariate has been "), covariateTransform, " transformed prior to analysis.", sep="")
	} else {
		add<-paste(add, c("The covariates have been "), covariateTransform, " transformed prior to analysis.", sep="")
	}
}
HTML(add, align="left")

#===================================================================================================================
#Scatterplot
#===================================================================================================================
title<-c("Scatterplots of the raw data, including best-fit regression lines")

if(responseTransform != "None" || contFactorTransform != "None") {
	title<-paste(title, ", on the transformed scale", sep="")
} 
HTML.title(title, HR=2, align="left")

#Graphical parameters
graphdata<-statdata
graphdata$yvarrr_IVS <- eval(parse(text = paste("statdata$",resp)))
MainTitle2 <- ""
w_Gr_jitscat <- 0
h_Gr_jitscat <-  0
infiniteslope <- "N"
LinearFit <- "Y"
Line_type <-Line_type_solid
Gr_alpha <- 1
graphdata$l_l<-graphdata$scatterPlotColumn
catvartest <- "Y"

for (i in 1: length(ContinuousList)) {
	XAxisTitle <- namereplace(ContinuousList[i])
	graphdata$xvarrr_IVS <-eval(parse(text = paste("statdata$",ContinuousList[i])))

	scatterPlot <- sub(".html", paste(i,"scatterPlot.jpg",sep=""), htmlFile)
	jpeg(scatterPlot,width = jpegwidth, height = jpegheight, quality = 100)

	#STB July2013
	plotFilepdf1 <- sub(".html", paste(i,"scatterPlot.pdf",sep=""), htmlFile)
	dev.control("enable") 

	#GGPLOT2 code
	if (treatFactors !="NULL") {
		ONECATSEP_SCAT()
	} else {
		NONCAT_SCAT("none")
	}

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

if (nocontfactors > 1) {
	HTML("Note: The lines plotted here are not related to the fitted statistical model, but are individual best-fit regression lines.", align="left")
}

if(covariatelist != "NULL" && noblockfactors==0) {
	HTML("Note: The best-fit regression lines included on the plot are not adjusted for the covariate.", align="left")
}

if(covariatelist != "NULL" && noblockfactors==1) {
	HTML("Note: The best-fit regression lines included on the plot are not adjusted for the covariate or the blocking factor.", align="left")
}

if(covariatelist != "NULL" && noblockfactors > 1) {
	HTML("Note: The best-fit regression lines included on the plot are not adjusted for the covariate or the blocking factors.", align="left")
}

#===================================================================================================================
#Slope estimation
#===================================================================================================================
if (nocontfactors == 1) {
	graphdata<-statdata
	
	if (treatFactors !="NULL") {
		graphdata$catfact <- statdata$scatterPlotColumn
	} else {
		graphdata$catfact <- "Overall"
	}
	graphdata$xvarrr_IVS <- eval(parse(text = paste("statdata$",ContinuousList[1])))
	graphdata$yvarrr_IVS <- eval(parse(text = paste("statdata$",resp)))

	rows<-dim(graphdata)[1]
	cols<-dim(graphdata)[2]
	nlevels<-length(unique(as.factor(graphdata$catfact)))
	extra<-matrix(data=NA, nrow=rows, ncol=nlevels)
	for (i in 1:nlevels) {
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
			threewayfullx<-lm(yvarrr_IVS~xvarrr_IVS, data=tmpdata2, na.action = na.omit)			
			inttabx<-   coefficients(threewayfullx)[1]
			slopetabx<- coefficients(threewayfullx)[2]
		} else {
			pcorr<-1000
			rho<-1000
			inttabx<-1000
			slopetabx<-1000
		}

		#STB - March 2011 Formatting p-values p<0.0001
		pcorr2<-format(round(pcorr, 4), nsmall=4, scientific=FALSE)
		rho<-format(round(rho, 3), nsmall=3, scientific=FALSE)
		inttabx<-format(round(inttabx, 4), nsmall=4, scientific=FALSE)
		slopetabx<-format(round(slopetabx, 4), nsmall=4, scientific=FALSE)

		if (pcorr<0.0001)  {
			pcorr2=format(round(0.0001, 4), nsmall=4, scientific=FALSE)
			pcorr2<- paste("<",pcorr2)
		}
			
		#STB Aug 2011 - removing lines with infinite slope
		if (pcorr==1000)  {
			pcorr2<- "-"
			rho<- "-"
			inttabx<- "-"
			slopetabx<- "-"
		}
		ptab[k]<-pcorr2
		rhotab[k]<-rho
		inttab[k]<-inttabx
		slopetab[k]<-slopetabx
	}

	esttab<-cbind( inttab, slopetab)
	label <-c()
	if (notreatfactors > 0) {
		for(i in 1:(dim(esttab)[1])) {
		label[i]<-levels(as.factor(tmpdata$catfact))[i]
		}
	} else {
		label <-c("Overall regression line")
	}
	esttab2<-cbind(label, esttab)

	if (notreatfactors > 1) {
		good3<-c("Categorisation factor level combinations")
	} else {
		good3<-c("Categorisation Factor levels")
	} 
	temp6<-c(good3, "Intercept estimate", "Slope estimate")
	colnames(esttab2)<-temp6

	HTML.title("Estimates of the coefficients of the best-fit regression lines", HR=2, align="left")
	HTML(esttab2 , align="left" , classfirstline="second", row.names="FALSE")
}

if (nocontfactors > 1) {
	HTML("Note: As the number of continuous factors included in the analysis is more than one, the coefficients of the best-fit regression lines have not been calculated.", align="left")
}

if(covariatelist != "NULL" && noblockfactors == 0) {
	HTML("Note: The estimates of the regression coefficients are not adjusted for the covariate.", align="left")
}

if(covariatelist != "NULL" && noblockfactors == 1) {
	HTML("Note: The estimates of the regression coefficients are not adjusted for the covariate or the blocking factor.", align="left")
}

if(covariatelist != "NULL" && noblockfactors > 1) {
	HTML("Note: The estimates of the regression coefficients are not adjusted for the covariate or the blocking factors.", align="left")
}

#===================================================================================================================
#Covariate plot
#===================================================================================================================
if(covariatelist != "NULL") {

	if (nocovlist == 1) {
		title<-c("Plot of the response vs. the covariate")
	} else {
		title<-c("Plot of the response vs. the covariates")
	}

	if (PlotCat != "Overall") {
		title<-paste(title, ", categorised by the primary factor", sep="")
	}
	if(responseTransform != "None" || covariateTransform != "None") {
		title<-paste(title, " (on the transformed scale)", sep="")
	} 
	HTML.title(title, HR=2, align="left")

	index <- 1
	for (i in 1:nocovlist) {
		ncscatterplot3 <- sub(".html", "IVS", htmlFile)
	    	ncscatterplot3 <- paste(ncscatterplot3, index, "ncscatterplot3.jpg", sep = "")
		jpeg(ncscatterplot3,width = jpegwidth, height = jpegheight, quality = 100)

		#STB July2013
		plotFilepdf2 <- sub(".html", "IVS", htmlFile)
		plotFilepdf2 <- paste(plotFilepdf2, index, "ncscatterplot3.pdf", sep="")
		dev.control("enable") 

		#Graphical parameters
		graphdata<-statdata
		graphdata$xvarrr_IVS <- eval(parse(text = paste("statdata$",covlistsep[i])))
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
	HTML("c) Is the covariate influenced by the treatment? (We assume the covariate is not influenced by the treatment and so there should be no separation of the treatment groups along the x-axis on the plot).", HR=0, align="left")
	HTML("These issues are discussed in more detail in Morris (1999).", align="left")
}

#===================================================================================================================
#building the covariate interaction model
#===================================================================================================================
if(AssessCovariateInteractions == "Y" && covariatelist != "NULL") {

	#Creating the list of model terms
	CovIntModel <- c(model)

	#Adding in additional continuous factor interactions
	for (i in 1:nocontfactors) {
		for (j in 1: nocovlist) {
			CovIntModel <- paste(CovIntModel, " + ",  ContinuousList[i], " * ", covlistsep[j], sep="")
		}
	}

	#Adding in additional categorical factor interactions
	if (treatFactors != "NULL") {
		for (i in 1:notreatfactors) {
			for (j in 1: nocovlist) {
				CovIntModel <- paste(CovIntModel, " + ",  TreatmentsList[i], " * ", covlistsep[j], sep="")
			}
		}
	}

	#Performing the ANCOVA analysis
	Covintfull<-lm(as.formula(CovIntModel), data=statdata, na.action = na.omit)

	#Title + warning
	HTML.title("Analysis of Covariance (ANCOVA) table for assessing covariate interactions", HR=2, align="left")

	# Stop process if residual sums of squares are too close to zero
	if (deviance(Covintfull)<sqrt(.Machine$double.eps)) {
		HTML("The Residual Sums of Squares is close to zero indicating the model is overfitted (too many terms are included in the model). The model should be simplified in order to generate statistical test results." , align="left")
		quit()
	}

	#Printing ANCOVA Table - note this code is reused from below - Type III SS included
	if (df.residual(Covintfull)<1) {
		HTML("The covariate interactions have not been calculated as there are zero residual degrees of freedom when all terms are included in the statistical model." , align="left")
	} else {
		tempx<-Anova(Covintfull, type=c("III"))[-1,]

		if (tempx[dim(tempx)[1],1] != 0) {
			temp2x<-(tempx)
			col1x<-format(round(temp2x[1], 2), nsmall=2, scientific=FALSE)
			col2x<-format(round(temp2x[1]/temp2x[2], 3), nsmall=3, scientific=FALSE)
			col3x<-format(round(temp2x[3], 2), nsmall=2, scientific=FALSE)
			col4x<-format(round(temp2x[4], 4), nsmall=4, scientific=FALSE)

			sourcex<-rownames(temp2x)

			# Residual label in ANOVA
			sourcex[length(sourcex)] <- "Residual"

			#STB March 2014 - Replacing : with * in ANOVA table
			for (q in 1:100) {
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
		} 
	}
}

#===================================================================================================================
#ANOVA table
#===================================================================================================================
#Testing the degrees of freedom

if (df.residual(threewayfull)<5) {
	HTML.title("Warning", HR=2, align="left")
	HTML("Unfortunately the residual degrees of freedom are low (less than 5). This may make the estimation of the underlying variability, and hence the results of the statistical tests, unreliable. This can be caused by attempting to fit too many factors, and their interactions, in the statistical model. Where appropriate we recommend you fit some of the 'Treatment' factors as 'Other design' factors. This will remove their interactions from the statistical model and therefore increase the residual degrees of freedom.", align="left")
}

#ANOVA Table
if(showANOVA=="Y") {
	if(covariatelist != "NULL") {
		HTML.title("Analysis of Covariance (ANCOVA) table", HR=2, align="left")
	} else {
		HTML.title("Analysis of variance (ANOVA) table", HR=2, align="left")
	}

	# Stop process if residual sums of squares are too close to zero
	if (deviance(threewayfull)<sqrt(.Machine$double.eps)) {
		HTML("The Residual Sums of Squares is close to zero indicating the model is overfitted (too many terms are included in the model). The model should be simplified in order to generate statistical test results." , align="left")
		quit()
	}

	#STB Sept 2014 - Marginal sums of square to tie in with RM (also message below and covariate ANOVA above)	
	temp<-Anova(threewayfull, type=c("III"))[-1,]
	col1<-format(round(temp[1], 2), nsmall=2, scientific=FALSE)
	col2<-format(round(temp[1]/temp[2], 3), nsmall=3, scientific=FALSE)
	col3<-format(round(temp[3], 2), nsmall=2, scientific=FALSE)
	col4<-format(round(temp[4], 4), nsmall=4, scientific=FALSE)

	source<-rownames(temp)

	#STB March 2014 - Replacing : with * in ANOVA table
	for (q in 1:100) {
		source<-sub(":"," * ", source) 
	}	

	# Residual label in ANOVA
	source[length(source)] <- "Residual"

	ivsanova<-cbind(source, col1,temp[2],col2,col3,col4)

	ivsanova[length(unique(source)),5]<-" "
	ivsanova[length(unique(source)),6]<-" "

	#STB May 2012 capitals changed
	head<-c("Effect", "Sums of squares", "Degrees of freedom", "Mean square", "F-value", "p-value")
	colnames(ivsanova)<-head

	for (i in 1:(dim(ivsanova)[1]-1)) {
		if (temp[i,4]<0.0001) {
			#STB March 2011 formatting p-values p<0.0001
			#ivsanova[i,6]<-0.0001
			ivsanova[i,6]=format(round(0.0001, 4), nsmall=4, scientific=FALSE)
			ivsanova[i,6]<- paste("<",ivsanova[i,6])
		}
	}

	HTML(ivsanova, classfirstline="second", align="left", row.names = "FALSE")

	if(covariatelist != "NULL") {
		#STB Error spotted:
		#HTML.title("<sTitle<-sub("ivs_colon_ivs"	,":"ML.title("<bf>Comment: ANCOVA table calculated using a Type III model fit, see Armitage et al. (2001).", HR=0, align="left")
		HTML("Comment: ANCOVA table calculated using a Type III model fit, see Armitage et al. (2001).", align="left")
	} else {
		HTML("Comment: ANOVA table calculated using a Type III model fit, see Armitage et al. (2001).", align="left")
	}

	add<-paste(c("Conclusion"))
	inte<-1
	for(i in 1:(dim(ivsanova)[1]-1)) {
		if (ivsanova[i,6]<= (1-sig)) {
			if (inte==1) {
				inte<-inte+1
				add<-paste(add, ": There is a statistically significant overall effect of ", ivsanova[i,1], sep="")
			} else {
				inte<-inte+1
				add<-paste(add, ", ", ivsanova[i,1],  sep="")
			}
		} 
	}

	if (inte==1) {
		if (dim(ivsanova)[1]>2) {
			if(covariatelist != "NULL") {

			#STB July 2013 change wording to remove effects
			add<-paste(add, ": There are no statistically significant overall effects in the ANCOVA table", sep="")
			} else {
			add<-paste(add, ": There are no statistically significant overall effects in the ANOVA table", sep="")
			}
		} 
	if (dim(ivsanova)[1]<=2) {
			add<-paste(add, ": There are no statistically significant overall effects", sep="")
		}
	} 
	add<-paste(add, ". ", sep="")

	HTML(add, align="left")
	if(covariatelist != "NULL") {
		HTML("Tip: While it is a good idea to consider the overall tests in the ANCOVA table, we should not rely on them when deciding whether or not to make pairwise comparisons.", align="left")
	} else { 
		HTML("Tip: While it is a good idea to consider the overall tests in the ANOVA table, we should not rely on them when deciding whether or not to make pairwise comparisons.", align="left")
	}
}

#===================================================================================================================
#Table of regression coefficients
#===================================================================================================================
options(contrasts=c("contr.SAS", "contr.SAS"))
threewayfull2<-lm(model, data=statdata, na.action = na.omit)

if (showCoefficients == "Y") {
	HTML.title("Table of model coefficients", HR=2, align="left")

	temp1<-coefficients(threewayfull2) # model coefficients
	temp2<- confint(threewayfull2, level=sig) # CIs for model parameters 
	temp3<- cbind(temp1, temp2)
	tablenames<-rownames(temp3)

	for (i in 1:100) {
		tablenames<- sub(":", " * ", tablenames)
	}	
	rownames(temp3)<-tablenames
	colnam<-c("Estimate", "lower","upper")
	colnam[2]<-paste("Lower ",(sig*100),"% CI",sep="")
	colnam[3]<-paste("Upper ",(sig*100),"% CI",sep="")
	
	colnames(temp3)<- colnam

	HTML(temp3, classfirstline="second", align="left", row.names="FALSE")
	HTML("Note: These model coefficients can be added together to obtain the model-based estimates of the relationships between the factors and the response, see Chambers and Hastie (1992).", align="left")
}

#===================================================================================================================
#Covariate correlation table
#===================================================================================================================
if (CovariateRegressionCoefficients == "Y" && covariatelist != "NULL") {

	if (nocovlist == 1) {
		HTML.title("Covariate regression coefficient", HR=2, align="left")
	} else {
		HTML.title("Covariate regression coefficients", HR=2, align="left")
	}
	covtable_1<-coef(summary(threewayfull))
	covtable<-data.frame(covtable_1)[c(2:(nocovlist+1)),]

	names <- rownames(covtable)
	Estimate <-format(round(covtable$Estimate, 3), nsmall=3, scientific=FALSE) 
	StdError <-format(round(covtable$Std..Error, 3), nsmall=3, scientific=FALSE) 
	tvalue <-format(round(covtable$t.value, 2), nsmall=2, scientific=FALSE) 
	Prt <-format(round(covtable$Pr...t.., 4), nsmall=4, scientific=FALSE) 
	
	covtable2 <-cbind(names, Estimate, StdError, tvalue, Prt)

	if (as.numeric(covtable[1,4])<0.0001)  {
		#STB March 2011 formatting p-values p<0.0001
		#ivsanova[i,9]<-0.0001
		covtable2[1,5]= "<0.0001"
	}
	colnames(covtable2)<-c("Covariate", "Estimate", "Std error", "t-value", "p-value")
	HTML(covtable2, classfirstline="second", align="left", row.names = "FALSE")
}

#===================================================================================================================
#R squared
#===================================================================================================================
if (showAdjustedRSquared =="Y") {
	HTML.title("R-squared and adjusted R-squared statistics", HR=2, align="left")

	rsq    <- format(round(summary(threewayfull)$r.squared,4),nsmall=4)
	adjrsq <- format(round(summary(threewayfull)$adj.r.squared,4),nsmall=4)
	rtab<-cbind(rsq,adjrsq)
	colnames(rtab)<-c("R-squared", "Adjusted R-sq")
	rownames(rtab)<- c("Estimate")
	HTML(rtab, classfirstline="second", align="left", row.names="FALSE")
	HTML("The R-squared is the fraction of the variance explained by the model. A value close to 1 implies the statistical model fits the data well. 
	Unfortunately adding additional variables to the statistical model will always increase R-sq, regardless of their importance. The Adjusted R-sq adjusts for the number of terms in the model and may decrease if over-fitting has occurred. 
	If there is a large difference between R-sq and Adjusted R-sq, then non-significant terms may have been included in the statistical model.
	", align="left")
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
	MainTitle2 <- "Residuals vs. predicted plot \n"
	w_Gr_jitscat <- 0
	h_Gr_jitscat <-  0
	infiniteslope <- "Y"
	Gr_line_type<-Line_type_dashed
	Line_size2 <- Line_size
	Line_size <- 0.5
	MainTitle2 <- ""

	#GGPLOT2 code
	NONCAT_SCAT("RESIDPLOT")

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
	MainTitle2 <- "Normal probability plot \n"
	w_Gr_jitscat <- 0
	h_Gr_jitscat <-  0
	infiniteslope <- "N"
	LinearFit <- "Y"
	Gr_line_type<-Line_type_dashed
	Line_size <- 0.5
	Gr_alpha <- 1
	Line_type <-Line_type_dashed
	MainTitle2 <- ""

	#GGPLOT2 code
	NONCAT_SCAT("QQPLOT")

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
#Cooks distance
#===================================================================================================================
if (cooksDistancePlot =="Y") {
	HTML.title("Cook's distance plot", HR=2, align="left")

	threewayfull<-lm(model, data=statdata, na.action = na.omit)
	cooks<-cooks.distance(threewayfull)
	graphdata<- na.omit(statdata)
	graphdata<-cbind(graphdata, cooks)
	graphdata$yvarrr_IVS<-graphdata$cooks
	graphdata$xvarrr_IVS<- c(1:length(cooks))
	XAxisTitle<-"Response number"
	YAxisTitle<-"Cook's distance"
	LinearFit <- "N"
	infiniteslope <- "N"
	Gr_line_type<-Line_type_dashed
	w_Gr_jitscat <- 0
	h_Gr_jitscat <-  0

	#cutoff <- 4/((nrow(graphdata)-length(threewayfull$coefficients)-2)) 
	cutoff <- 4/nrow(graphdata) 

	#Plot device setup
	ncscatterplotx <- sub(".html", "ncscatterplotx.jpg", htmlFile)
	jpeg(ncscatterplotx,width = jpegwidth, height = jpegheight, quality = 100)

	#STB July2013
	plotFilepdf16 <- sub(".html", "ncscatterplotx.pdf", htmlFile)
	dev.control("enable") 

	#GGPLOT2 code
	NONCAT_SCAT("cook")

	#Output code
	void<-HTMLInsertGraph(GraphFileName=sub("[A-Z0-9a-z,:,\\\\]*App_Data[\\\\]","", ncscatterplotx), Align="centre")

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

	HTML("This plot should be used to assess whether there are any potential outliers in the dataset. Observations 
	where the Cook's distance are above the cut-off line should be investigated further. Note the cut-off line has 
	been calculated using the 4/n approach, where n is the number of observations in the dataset. 
	", align="left")
}

#===================================================================================================================
#Leverage plot
#===================================================================================================================
if (leveragesPlot =="Y") {
	HTML.title("Leverage plot", HR=2, align="left")

	lev <- hat(model.matrix(threewayfull))
	graphdata<- na.omit(statdata)
	graphdata<-cbind(graphdata, lev)
	graphdata$yvarrr_IVS<-graphdata$lev
	graphdata$xvarrr_IVS<- c(1:length(lev))
	XAxisTitle<-"Response number"
	YAxisTitle<-"Leverage"
	LinearFit <- "N"
	infiniteslope <- "N"
	w_Gr_jitscat <- 0
	h_Gr_jitscat <-  0

	#Plot device setup
	ncscatterplot <- sub(".html", "ncscatterplot.jpg", htmlFile)
	jpeg(ncscatterplot,width = jpegwidth, height = jpegheight, quality = 100)

	#STB July2013
	plotFilepdf15 <- sub(".html", "ncscatterplot.pdf", htmlFile)
	dev.control("enable") 

	#GGPLOT2 code
	NONCAT_SCAT("none")

	#Output code
	void<-HTMLInsertGraph(GraphFileName=sub("[A-Z0-9a-z,:,\\\\]*App_Data[\\\\]","", ncscatterplot), Align="centre")

	#STB July2013
	if (pdfout=="Y") {
		pdf(file=sub("[A-Z0-9a-z,:,\\\\]*App_Data[\\\\]","", plotFilepdf15), height = pdfheight, width = pdfwidth) 
		dev.set(2) 
		dev.copy(which=3) 
		dev.off(2)
		dev.off(3)
		pdfFile_15<-sub("[A-Z0-9a-z,:,\\\\]*App_Data[\\\\]","",plotFilepdf15)
		linkToPdf15 <- paste ("<a href=\"",pdfFile_15,"\">Click here to view the PDF of the scatterplot</a>", sep = "")
		HTML(linkToPdf15)
	}

	HTML("This plot indicates the relative influence of the observations. Observations with a high leverage may be unduly influencing the statistical model.", align="left")
}

#===================================================================================================================
#Analysis description
#===================================================================================================================
HTML.title("Analysis description", HR=2, align="left")

add<-c("The data were analysed using an ")
if (nocontfactors==1 && notreatfactors==0)  {
	if(covariatelist != "NULL") {
		add<-paste(add, "1-way ANCOVA approach, with ", ContinuousList, " as the continuous factor", sep="")
	} else {
		add<-paste(add, "1-way ANOVA approach, with ", ContinuousList, " as the continuous factor", sep="")
	}
} else {
	nofact<-nocontfactors + notreatfactors
	add<-paste(add, nofact, sep="")
	if(covariatelist != "NULL") {
		add<-paste(add, "-way ANCOVA approach, with", sep="")
	} else {
		add<-paste(add, "-way ANOVA approach, with", sep="")
	}
	for (i in 1:nocontfactors) {
		if (i<nocontfactors-1)	{
			add<-paste(add, " ", ContinuousList[i], ",",sep="")
		} else 	if (i<nocontfactors) {
			add<-paste(add,  " ", ContinuousList[i], " and", sep="")
		} else if (i==nocontfactors) {
			if (nocontfactors==1) {
				add<-paste(add, " ", ContinuousList[i], " as the continuous factor", sep="")
			} else {
				add<-paste(add, " ", ContinuousList[i], " as the continuous factors", sep="")
			}
		}
	}

	if (notreatfactors !=0) {
		for (i in 1:notreatfactors) {
			if (i<notreatfactors-1)	{
				add<-paste(add, ", ", TreatmentsList[i],  sep="")
			} else 	if (i<notreatfactors) {
				add<-paste(add, ", ", TreatmentsList[i], " and", sep="")
			} else if (i==notreatfactors) {
				if (notreatfactors==1) {
					add<-paste(add, ", ", TreatmentsList[i], " as the treatment factor", sep="")
				} else {
					add<-paste(add, " ", TreatmentsList[i], " as the treatment factors", sep="")
				}
			}
		}
	}
}
if (blocklist != "NULL" && covariatelist != "NULL")  {
	add<-paste(add, ", ", sep="")
} else if (noblockfactors==1 && blocklist != "NULL" && covariatelist == "NULL")  {
	add<-paste(add, " and ", sep="")
} 
	
if (noblockfactors==1 && blocklist != "NULL")  {
	add<-paste(add, blocklist, " as a blocking factor", sep="")
} else {
	if(noblockfactors>1)  {
		if (covariatelist == "NULL") {
			add<-paste(add, " and ", sep="")
		}
		for (i in 1:noblockfactors) {
			if (i<noblockfactors-1) {
				add<-paste(add, blocklistsep[i], ", ", sep="")
			} else	if (i<noblockfactors) {
				add<-paste(add, blocklistsep[i], " and ", sep="")
			} else if (i==noblockfactors) {
				add<-paste(add, blocklistsep[i], sep="")
			}
		}
		add<-paste(add, " as the blocking factors", sep="")
	}
}
if (covariatelist == "NULL") {
	add<-paste(add, ". ", sep="")
} else {
	add<-paste(add, " and ",  sep="")
	if (nocovlist == 1) {
		add<-paste(add, covlistsep[1], " as the covariate.", sep="")
	} else {
		for (i in 1:nocovlist) {
			if (i<nocovlist-1)	{
				add<-paste(add, covlistsep[i], ", ", sep="")
			} else 	if (i<nocovlist) {
				add<-paste(add, covlistsep[i], " and ", sep="")
			} else if (i==nocovlist) {
				add<-paste(add, covlistsep[i], " as the covariates.", sep="")
			}
		}
	}
}

if (responseTransform != "None") {
	add<-paste(add, " The response was ", responseTransform, " transformed prior to analysis to stabilise the variance. ", sep="")
}

if (covariatelist !="NULL" && covariateTransform != "None") {
	if (nocovlist == 1) {
		add<-paste(add, c("The covariate has been "), covariateTransform, " transformed prior to analysis.", sep="")
	} else {
		add<-paste(add, c("The covariates have been "), covariateTransform, " transformed prior to analysis.", sep="")
	}
}
HTML(add, align="left")

#===================================================================================================================
#References
#===================================================================================================================
Ref_list<-R_refs()

#Bate and Clark comment
HTML.title(refxx, HR=0, align="left")	

HTML.title("Statistical references", HR=2, align="left")
HTML(Ref_list$BateClark_ref, align="left")

if(showANOVA=="Y") {
	HTML("Armitage P, Matthews JNS and Berry G. (2001). Statistical Methods in Medical Research. 4th edition; John Wiley & Sons. New York.", align="left")
}
if(showCoefficients == "Y") {
	HTML("Chambers JM and Hastie TJ. (1992). Statistical Models in S. Wadsworth and Brooks-Cole advanced books and software.", align="left")
}
if(covariatelist != "NULL") {
	HTML("Morris TR. (1999). Experimental Design and Analysis in Animal Sciences. CABI publishing. Wallingford, Oxon (UK).", align="left")
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

#===================================================================================================================
#Show dataset
#===================================================================================================================
if (showdataset=="Y") {
	statdata$scatterPlotColumn <-NULL
	statdata$FirstCatFactor <-NULL

	observ <- data.frame(c(1:dim(statdata)[1]))
	colnames(observ) <- c("Observation")
	statdata2 <- cbind(observ, statdata)

    	HTML.title("Analysis dataset", HR = 2, align = "left")
    	HTML(statdata2, classfirstline = "second", align = "left", row.names = "FALSE")
}

#===================================================================================================================
#Show arguments
#===================================================================================================================
HTML.title("Analysis options", HR=2, align="left")

HTML(paste("Response variable: ", resp, sep=""),  align="left")
if (responseTransform != "None") {
	HTML(paste("Response transformation: ", responseTransform, sep=""),  align="left")
}

if(covariatelist != "NULL") {
	HTML(paste("Covariate(s): ", covariatelist, sep=""), align="left")
}

if (covariatelist != "NULL" && covariateTransform != "None") {
	if (nocontfactors == 1) {
		HTML(paste("Covariate transformation: ", covariateTransform, sep=""),  align="left")
	} else {
		HTML(paste("Covariates transformation: ", covariateTransform, sep=""),  align="left")
	}
}

if (covariatelist != "NULL" ) {
	HTML(paste("Categorisation factor used on covariate scatterplots: ", FirstCatFactor, sep=""),  align="left")
}

if(contFactors != "NULL") {
	HTML(paste("Continuous treatment factors included in model: ", contFactors, sep=""),  align="left")
}
if (contFactorTransform != "None"){
	if (nocontfactors == 1) {
		HTML(paste("Continuous factor transformation: ", contFactorTransform, sep=""),  align="left")
	} else {
		HTML(paste("Continuous factors transformation: ", contFactorTransform, sep=""),  align="left")
	}
}

if(treatFactors != "NULL") {
	HTML(paste("Categorical treatment factors included in model: ", treatFactors, sep=""),  align="left")
}

if (blocklist != "NULL") {
	HTML(paste("Blocking factor(s) selected: ", blocklistsep, sep=""),  align="left")
}
HTML(paste("Model fitted: ", model, sep=""),  align="left")
HTML(paste("Output ANOVA table (Y/N): ", showANOVA, sep=""),  align="left")
HTML(paste("Output model coefficients (Y/N): ", showCoefficients, sep=""),  align="left")
HTML(paste("Output Adjusted R-squared (Y/N): ", showAdjustedRSquared, sep=""),  align="left")
HTML(paste("Output predicted vs. residual plot (Y/N): ", showPRPlot, sep=""),  align="left")
HTML(paste("Output normal probability plot (Y/N): ", showNormPlot, sep=""),  align="left")
HTML(paste("Output Cook's distance plot (Y/N): ", cooksDistancePlot, sep=""),  align="left")
HTML(paste("Output leverage plot (Y/N): ", leveragesPlot, sep=""),  align="left")
HTML(paste("Significance level: ", 1-sig, sep=""),  align="left")












