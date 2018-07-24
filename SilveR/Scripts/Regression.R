#===================================================================================================================
#R Libraries

suppressWarnings(library(car))
suppressWarnings(library(R2HTML))

#STB 14OCT2015
#Set contrast options for Marginal overall tests
options(contrasts=c(unordered="contr.sum", ordered="contr.poly"))

#===================================================================================================================
# retrieve args
Args <- commandArgs(TRUE)

#Read in data
statdata <- read.csv(Args[3], header=TRUE, sep=",")

#Copy Args
model <- Args[4]
covariateModel <- Args[5]
responseTransform <- Args[6]
covariateTransform <- Args[7]
FirstCatFactor <- Args[8]
treatFactors <- Args[9]
contFactors <- Args[10]
contFactorTransform <- Args[11]
blockFactors <- Args[12]
showANOVA <- Args[13]
showCoefficients <- Args[14]
showAdjustedRSquared <- Args[15]
sig <- 1 - as.numeric(Args[16])
residualsVsPredictedPlot <- Args[17]
normalProbabilityPlot <- Args[18]
cooksDistancePlot <- Args[19]
leveragesPlot <- Args[20]

scatterplotModel <- as.formula(Args[5])
showPRPlot <- residualsVsPredictedPlot
showNormPlot <- normalProbabilityPlot

#===================================================================================================================
#working directory
direct2<- unlist(strsplit(Args[3],"/"))
direct<-direct2[1]
for (i in 2:(length(direct2)-1))
{
	direct<- paste(direct, "/", direct2[i], sep = "")
}

if (FirstCatFactor == "NULL")
{
	statdata$FirstCatFactor <- "None"
	FirstCatFactor<-"FirstCatFactor"
}

source(paste(getwd(),"/InVivoStat_functions.R", sep=""))

#STB 14OCT2015
#Set contrast options for Marginal overall tests
options(contrasts=c(unordered="contr.sum", ordered="contr.poly"))
#===================================================================================================================
#Graphics parameter setup
graphdata<-statdata

if (covariateModel != "NULL")
{
	Gr_palette<-palette_FUN(FirstCatFactor)
} 
Line_size2 <- Line_size
Labelz_IVS_ <- "N"
#===================================================================================================================
#Setup the html file and associated css file
htmlFile <- sub(".csv", ".html", Args[3]); #determine the file name of the html file
HTMLSetFile(file=htmlFile) 
cssFile <- "r2html.css"
cssFile <- paste("'",cssFile,"'", sep="") #need to enclose in quotes when path has spaces in it
HTMLCSS(CSSfile = cssFile)

#===================================================================================================================
#Output HTML header
HTML.title("<bf>InVivoStat Linear Regression Analysis", HR=1, align="left")

if (is.numeric(statdata$mainEffect) == TRUE)
{
	cntrlGroup <- paste ("'",cntrlGroup,"'",sep="")
}

#============================================================================================================================

#Parameter setup

#The response
resp <- unlist(strsplit(Args[4],"~"))[1] #get the response variable from the main model

if(covariateModel != "NULL")
{
	CovariatE <- unlist(strsplit(covariateModel, "~"))[2]
} else {
	CovariatE <- "NULL"
	}

#replace illegal characters in variable names
YAxisTitle <-resp
if(covariateModel != "NULL")
{
	XAxisTitle<-unlist(strsplit(covariateModel, "~"))[2]
}

#replace illegal characters in variable names
for (i in 1:10)
{
	YAxisTitle<-namereplace(YAxisTitle)

	if(covariateModel != "NULL")
	{
		XAxisTitle<-namereplace(XAxisTitle)
	}
}
LS_YAxisTitle<-YAxisTitle


#calculating number of block factors
noblockfactors=0
if (blockFactors !="NULL")
{
	tempblockChanges <-strsplit(blockFactors, ",")
	BlockList <- c("")
	for(i in 1:length(tempblockChanges[[1]])) 
	{
		BlockList [length(BlockList )+1]=(tempblockChanges[[1]][i]) 
	}
	noblockfactors<-length(BlockList)-1
	BlocksList<-BlockList[-1]
}

#calculating number of treatment factors
notreatfactors=0

if (treatFactors !="NULL")
{
	tempChanges <-strsplit(treatFactors, ",")
	TreatList <- c("")
	for(i in 1:length(tempChanges[[1]])) 
	{ 
		TreatList [length(TreatList )+1]=(tempChanges[[1]][i]) 
	}
	notreatfactors<-length(TreatList)-1
	TreatmentsList<-TreatList[-1]

	statdata$scatterPlotColumn<-eval(parse(text = paste("statdata$",TreatmentsList[1])))

	if(notreatfactors>1)
	{
		for (k in 2:notreatfactors)
		{
			statdata$scatterPlotColumn<- paste(statdata$scatterPlotColumn , eval(parse(text = paste("statdata$",TreatmentsList[k]))))
		}
	}
}

#calculating number of continuous factors
tempcontChanges <-strsplit(contFactors, ",")
txtexpectedcontChanges <- c("")
for(i in 1:length(tempcontChanges[[1]])) 
{ 
	txtexpectedcontChanges [length(txtexpectedcontChanges )+1]=(tempcontChanges[[1]][i]) 
}
nocontfactors<-length(txtexpectedcontChanges)-1
ContinuousList <- txtexpectedcontChanges[-1]


#Testing the factorial combinations
intindex<-length(unique(statdata$scatterPlotColumn))
ind<-1

if (treatFactors !="NULL")
{
	for (i in 1:notreatfactors)
	{
		ind=ind*length(unique(eval(parse(text = paste("statdata$",TreatList[i+1])))))
	}

	if(intindex != ind)
	{
		add<-c("Unfortunately not all combinations of the levels of the treatment factors are present in the experimental design. We recommend you manually create a new factor corresponding to the combinations of the levels of the treatment factors.")
		HTML.title("</bf> ", HR=2, align="left")
		HTML.title(add, HR=0, align="left")
		print(add)
		quit()
	}
}




#Creating the model

if(covariateModel != "NULL")
{
	model<- paste(resp , "~" , CovariatE , "+" , ContinuousList[1])
} else {
	model<- paste(resp , "~" , ContinuousList[1])
	}

if (nocontfactors > 1)
{
	for (x in 2:nocontfactors)
	{
		model <- paste(model, "*", ContinuousList[x])
	}
}

if (notreatfactors > 0)
{
	for (y in 1:notreatfactors)
	{
		model <- paste(model, "*", TreatmentsList[y])
	}
}

if (noblockfactors > 0)
{
	for (z in 1:noblockfactors)
	{
		model <- paste(model, "+", BlocksList[z])
	}
}

threewayfull<-lm(model, data=statdata, na.action = na.omit)


#============================================================================================================================
#Titles and description
#============================================================================================================================
#Response

title<-c("Response")
if(covariateModel != "NULL")
{
	title<-paste(title, " and covariate", sep="")
}

HTML.title(title, HR=2, align="left")
add<-paste(c("The  "), resp, sep="")
add<-paste(add, " response is currently being analysed by the Linear Regression Analysis module", sep="")

if(covariateModel != "NULL")
{
	add<-paste(add, c(", with  "), sep="")
	add<-paste(add, unlist(strsplit(covariateModel, "~"))[2], sep="")
	add<-paste(add, " fitted as a covariate.", sep="")
} else {
	add<-paste(add, ".", sep="")
}

HTML.title("</bf> ", HR=2, align="left")
HTML.title(add, HR=0, align="left")

if (responseTransform != "None" | covariateTransform != "None")
{
	HTML.title("<bf> ", HR=2, align="left")
}

if (responseTransform != "None")
{
	add2<-paste(c("The response has been "), responseTransform, sep="")
	add2<-paste(add2, " transformed prior to analysis.", sep="")
	HTML.title(add2, HR=0, align="left")
}

if (covariateTransform != "None")
{
	add3<-paste(c("The covariate has been "), covariateTransform, sep="")
	add3<-paste(add3, " transformed prior to analysis.", sep="")
	HTML.title(add3, HR=0, align="left")
}

#============================================================================================================================
#Scatterplot
#============================================================================================================================

HTMLbr()
title<-c("Scatterplots of the raw data, including best-fit regression lines")

if(responseTransform != "None" || contFactorTransform != "None")
{
	title<-paste(title, ", on the transformed scale", sep="")
} 

HTML.title(title, HR=2, align="left")

#==================================================================================================================================================
#==================================================================================================================================================
#Graphical parameters

graphdata<-statdata
graphdata$yvarrr_IVS <- eval(parse(text = paste("statdata$",resp)))
MainTitle2 <- ""
w_Gr_jit <- 0
h_Gr_jit <-  0
infiniteslope <- "N"
LinearFit <- "Y"
Line_type <-Line_type_solid
Gr_alpha <- 1
graphdata$l_l<-graphdata$scatterPlotColumn
catvartest <- "Y"

for (i in 1: length(ContinuousList))
{
	XAxisTitle <- namereplace(ContinuousList[i])
	graphdata$xvarrr_IVS <-eval(parse(text = paste("statdata$",ContinuousList[i])))

	scatterPlot <- sub(".html", paste(i,"scatterPlot.jpg",sep=""), htmlFile)
	jpeg(scatterPlot,width = jpegwidth, height = jpegheight, quality = 100)

#STB July2013
	plotFilepdf1 <- sub(".html", paste(i,"scatterPlot.pdf",sep=""), htmlFile)
	dev.control("enable") 


#GGPLOT2 code
	if (treatFactors !="NULL")
	{
		ONECATSEP_SCAT()
	} else {
		NONCAT_SCAT("none")
		}

#==================================================================================================================================================
#==================================================================================================================================================
	void <- HTMLInsertGraph(GraphFileName=sub("[A-Z0-9a-z,:,\\\\]*App_Data[\\\\]","", scatterPlot), Align="centre")

#STB July2013
	if (pdfout=="Y")
	{
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

if (nocontfactors > 1)
{
	HTML.title("</bf> ", HR=2, align="left")
	HTML.title("</bf> Note: The lines plotted here are not related to the fitted statistical model, but are individual best-fit regression lines.", HR=0, align="left")
}

if(covariateModel != "NULL" && noblockfactors==0)
{
	HTML.title("</bf> ", HR=2, align="left")
	title<- "Note: The best-fit regression lines included on the plot are not adjusted for the covariate."
	HTML.title(title, HR=0, align="left")
}

if(covariateModel != "NULL" && noblockfactors==1)
{
	HTML.title("</bf> ", HR=2, align="left")
	title<- "Note: The best-fit regression lines included on the plot are not adjusted for the covariate or the blocking factor."
	HTML.title(title, HR=0, align="left")
}

if(covariateModel != "NULL" && noblockfactors > 1)
{
	HTML.title("</bf> ", HR=2, align="left")
	title<- "Note: The best-fit regression lines included on the plot are not adjusted for the covariate or the blocking factors."
	HTML.title(title, HR=0, align="left")
}



#============================================================================================================================
#Slope estimation
#============================================================================================================================

if (nocontfactors == 1)
{
	graphdata<-statdata
	
	if (treatFactors !="NULL")
	{
		graphdata$catfact <- statdata$scatterPlotColumn
	} else {
		graphdata$catfact <- "None"
		}

	graphdata$xvarrr_IVS <- eval(parse(text = paste("statdata$",ContinuousList[1])))
	graphdata$yvarrr_IVS <- eval(parse(text = paste("statdata$",resp)))

	rows<-dim(graphdata)[1]
	cols<-dim(graphdata)[2]
	nlevels<-length(unique(as.factor(graphdata$catfact)))
	extra<-matrix(data=NA, nrow=rows, ncol=nlevels)
	for (i in 1:nlevels)
	{
		for (j in 1:rows)
		{
			if (as.factor(graphdata$catfact)[j] == unique(as.factor(graphdata$catfact))[i])
			{
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
	for (k in 1:nlevels)
	{
		tmpdata<-catplotdata
		tmpdata2<-subset(tmpdata, tmpdata$catfact == unique(levels(as.factor(tmpdata$catfact)))[k])

#STB Aug 2011 - removing lines with infinite slope
		if(length(unique(tmpdata2$xvarrr_IVS)) >2 && length(unique(tmpdata2$yvarrr_IVS)) >2)
		{
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

		if (pcorr<0.0001) 
		{
			pcorr2=format(round(0.0001, 4), nsmall=4, scientific=FALSE)
			pcorr2<- paste("<",pcorr2)
		}

#		rho<-format(round(rho, 2), nsmall=2, scientific=FALSE)

		
#STB Aug 2011 - removing lines with infinite slope
		if (pcorr==1000) 
		{
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

	corrtab<-cbind( rhotab, " ", ptab)
	header<-c(" "," "," ")
	if (notreatfactors>0)
	{
		corrtab<-rbind(header, corrtab)
	}	
	temp6<-c("Correlation coefficient", " ","p-value")
	colnames(corrtab)<-temp6
	if (notreatfactors>1)
	{
		good3<-c("Categorisation factor level combinations")
	} else {
		good3<-c("Categorisation Factor levels")
		} 

	if (notreatfactors > 0 )
	{
		for(i in 1:(dim(corrtab)[1]-1))
		{
		good3[i+1]<-levels(as.factor(tmpdata$catfact))[i]
		}
	} else {
		good3 <-c("Overall correlation")
		}
	row.names(corrtab)<-good3


	esttab<-cbind( inttab, " ", slopetab)
	header<-c(" "," "," ")
	if (notreatfactors > 0)
	{
		esttab<-rbind(header, esttab)
	}	
	temp6<-c("Intercept estimate", " ","Slope estimate")
	colnames(esttab)<-temp6
	if (notreatfactors > 1)
	{
		good3<-c("Categorisation factor level combinations")
	} else {
		good3<-c("Categorisation Factor levels")
		} 

	if (notreatfactors > 0)
	{
		for(i in 1:(dim(esttab)[1]-1))
		{
		good3[i+1]<-levels(as.factor(tmpdata$catfact))[i]
		}
	} else {
		good3 <-c("Overall regression line")
		}
	row.names(esttab)<-good3

	HTMLbr()
	title<-c("Estimates of the coefficients of the best-fit regression lines")
	HTML.title(title, HR=2, align="left")
	HTML(esttab, , align="left" , classfirstline="second")
}

if (nocontfactors > 1)
{
	HTML.title("</bf> ", HR=2, align="left")
	HTML.title("</bf> Note: As the number of continuous factors included in the analysis is more than one, the coefficients of the best-fit regression lines have not been calculated.", HR=0, align="left")
}

if(covariateModel != "NULL" && noblockfactors == 0)
{
	HTML.title("</bf> ", HR=2, align="left")
	HTML.title("</bf> Note: The estimates of the regression coefficients are not adjusted for the covariate.", HR=0, align="left")
}

if(covariateModel != "NULL" && noblockfactors == 1)
{
	HTML.title("</bf> ", HR=2, align="left")
	HTML.title("</bf> Note: The estimates of the regression coefficients are not adjusted for the covariate or the blocking factor.", HR=0, align="left")
}

if(covariateModel != "NULL" && noblockfactors > 1)
{
	HTML.title("</bf> ", HR=2, align="left")
	HTML.title("</bf> Note: The estimates of the regression coefficients are not adjusted for the covariate or the blocking factors.", HR=0, align="left")
}

#============================================================================================================================
#Covariate plot
#============================================================================================================================

if(covariateModel != "NULL")
{

	if (nocontfactors == 1)
	{
		title<-c("Covariate plot of the raw data (ignoring continuous factor)")
	} else {
		title<-c("Covariate plot of the raw data (ignoring continuous factors)")
		}	
	if(responseTransform != "None" || covariateTransform != "None")
	{
		title<-paste(title, " on the transformed scale", sep="")
	} 
	HTMLbr()
	HTML.title(title, HR=2, align="left")

	ncscatterplot3 <- sub(".html", "ncscatterplot3.jpg", htmlFile)
	jpeg(ncscatterplot3,width = jpegwidth, height = jpegheight, quality = 100)

#STB July2013
	plotFilepdf2 <- sub(".html", "ncscatterplot3.pdf", htmlFile)
	dev.control("enable") 

#================================================================================================================================================
#================================================================================================================================================
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

	if (FirstCatFactor != "NULL" && notreatfactors > 0)
	{
		OVERLAID_SCAT()
	} else {
		NONCAT_SCAT("none")
		}
#================================================================================================================================================
#================================================================================================================================================
	
	void<-HTMLInsertGraph(GraphFileName=sub("[A-Z0-9a-z,:,\\\\]*App_Data[\\\\]","", ncscatterplot3), Align="centre")

#STB July2013
	if (pdfout=="Y")
	{
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
	if (infiniteslope != "N")
	{
		title<- "Warning: The covariate has the same value for all subjects in one or more levels of the "
		title<-paste(title, FirstCatFactor, sep="")
		title<-paste(title, " factor. Care should be taken if you want to include this covariate in the analysis.", sep="")
		HTML.title("</bf> ", HR=2, align="left")
		HTML.title(title, HR=0, align="left")
	}

	if (FirstCatFactor != "NULL" && notreatfactors > 0)
	{
	HTML.title("</bf> ", HR=2, align="left")
	HTML.title("<bf> Tip: In order to decide whether it is helpful to fit the covariate, the following should be considered:", HR=0, align="left")
	HTML.title("</bf> ", HR=2, align="left")
	HTML.title("<bf> a) Is there a relationship between the response and the covariate? (N.B., It is only worth fitting the covariate if there is a strong positive (or negative) relationship between them: i.e., the lines on the plot should not be horizontal).", HR=0, align="left")
	HTML.title("</bf> ", HR=2, align="left")
	HTML.title("<bf> b) Is the relationship similar for all treatments? (The lines on the plot should be approximately parallel).", HR=0, align="left")
	HTML.title("</bf> ", HR=2, align="left")
	HTML.title("<bf> c) Is the covariate influenced by the treatment? (We assume the covariate is not influenced by the treatment and so there should be no separation of the treatment groups along the x-axis on the plot).", HR=0, align="left")
	HTML.title("</bf> ", HR=2, align="left")
	HTML.title("<bf> These issues are discussed in more detail in Morris (1999).", HR=0, align="left")
	} else {
		HTML.title("</bf> ", HR=2, align="left")
		HTML.title("<bf> Tip: In order to decide whether it is helpful to fit the covariate, consider the relationship between the response and the covariate. It is only worth fitting the covariate if there is a strong positive (or negative) relationship between them: i.e., the lines on the plot should not be horizontal. This is discussed in more detail in Morris (1999).", HR=0, align="left")
		}
}


#============================================================================================================================
#building the covariate interaction model
#============================================================================================================================
if(AssessCovariateInteractions == "Y" &&covariateModel != "NULL")
{

#Creating the model

	if(covariateModel != "NULL")
	{
		model2<- paste(resp , "~" , CovariatE , "+" , ContinuousList[1])
	} 

	if (nocontfactors > 1)
	{
		for (x in 2:nocontfactors)
		{
			model2 <- paste(model2, "*", ContinuousList[x])
		}
	}

	if (notreatfactors > 0)
	{
		for (y in 1:notreatfactors)
		{
			model2 <- paste(model2, "*", TreatmentsList[y])
		}
	}

	model2<- paste(model2 , "*", CovariatE) 

	if (noblockfactors > 0)
	{
		for (z in 1:noblockfactors)
		{
			model2 <- paste(model2, "+", BlocksList[z])
		}
	}



#Performing the ANCOVA analysis
Covintfull<-lm(as.formula(model2), data=statdata, na.action = na.omit)

#Title + warning
	HTMLbr()
	HTML.title("<bf>Analysis of Covariance (ANCOVA) table for assessing covariate interactions", HR=2, align="left")

#Printing ANCOVA Table - note this code is reused from below - Type III SS included

#==================================================================================================================================================
#Testing the degrees of freedom

	if (df.residual(Covintfull)<1)
	{
		add<-c("The covariate interactions have not been calculated as there are zero residual degrees of freedom when all terms are included in the statistical model.")
		HTML.title("</bf> ", HR=2, align="left")
		HTML.title(add, HR=0, align="left")
	} else {
		tempx<-Anova(Covintfull, type=c("III"))[-1,]
#	tempx<-anova(Covintfull)

		if (tempx[dim(tempx)[1],1] != 0)
		{
			temp2x<-(tempx)

			col1x<-format(round(temp2x[1], 2), nsmall=2, scientific=FALSE)
			col2x<-format(round(temp2x[1]/temp2x[2], 3), nsmall=3, scientific=FALSE)
			col3x<-format(round(temp2x[3], 2), nsmall=2, scientific=FALSE)
			col4x<-format(round(temp2x[4], 4), nsmall=4, scientific=FALSE)

			blanqx<-c(" ")
			for (i in 1:dim(tempx)[1])
			{ 
				blanqx[i]=" "
			}
			ivsanovax<-cbind(col1x,blanqx,temp2x[2],blanqx,col2x,blanqx,col3x,blanqx,col4x)
	
			source2x<-rownames(ivsanovax)
#STB March 2014 - Replacing : with * in ANOVA table
			for (q in 1:(notreatfactors+nocontfactors))
			{
				source2x<-sub(":"," * ", source2x) 
			}	
			rownames(ivsanovax)<-source2x
			source3x<-rownames(ivsanovax)

			ivsanovax[length(unique(source2x)),7]<-" "
			ivsanovax[length(unique(source2x)),9]<-" "
#STB May 2012 capitals changed
			headx<-c("Sums of squares", " ","Degrees of freedom"," ","Mean square"," ","F-value"," ","p-value")
			colnames(ivsanovax)<-headx

			for (i in 1:(dim(ivsanovax)[1]-1)) 
			{
				if (temp2x[i,4]<0.0001) 
				{
#STB March 2011 formatting p-values p<0.0001
#			ivsanovax[i,9]<-0.0001
					ivsanovax[i,9]=format(round(0.0001, 4), nsmall=4, scientific=FALSE)
					ivsanovax[i,9]<- paste("<",ivsanovax[i,9])
				}
			}

			HTML(ivsanovax, classfirstline="second", align="left")

			add<-c("Note: This table should only be used to assess the covariate interactions. The statistical model used to generate all the remaining results in this output does not include the covariate interactions.")
			HTML.title("</bf> ", HR=2, align="left")
			HTML.title(add, HR=0, align="left")
		} 
	}
}
#============================================================================================================================
#ANOVA table
#============================================================================================================================

#ANOVA Table
if(showANOVA=="Y")
{

	if(covariateModel!= "NULL")
	{
	HTMLbr()
	HTML.title("<bf>Analysis of Covariance (ANCOVA) table", HR=2, align="left")
	} else {
	HTMLbr()
	HTML.title("<bf>Analysis of variance (ANOVA) table", HR=2, align="left")
	}


#==================================================================================================================================================
#Testing the degrees of freedom

if (df.residual(threewayfull)<5)
{
	add<-c("Unfortunately the residual degrees of freedom are low (less than 5). This may make the estimation of the underlying variability, and hence the results of the statistical tests, unreliable. This can be caused by attempting to fit too many factors, and their interactions, in the statistical model. Where appropriate we recommend you fit some of the 'Treatment' factors as 'Other design' factors. This will remove their interactions from the statistical model and therefore increase the residual degrees of freedom.")
	HTML.title("</bf> ", HR=2, align="left")
	HTML.title(add, HR=0, align="left")
}
#==================================================================================================================================================


#STB Sept 2014 - Marginal sums of square to tie in with RM (also message below and covariate ANOVA above)	

	tempxX<-anova(threewayfull)
	if (tempxX[dim(tempxX)[1],1] == 0)
	{
		add<-c("The ANOVA have not been calculated as there are zero residual degrees of freedom when all terms are included in the statistical model.")
		HTML.title("</bf> ", HR=2, align="left")
		HTML.title(add, HR=0, align="left")
		quit()
	} else {

	temp<-Anova(threewayfull, type=c("III") )[-1,]
#	temp<-anova(threewayfull)
		temp2<-temp

		col1<-format(round(temp2[1], 2), nsmall=2, scientific=FALSE)
		col2<-format(round(temp2[1]/temp2[2], 3), nsmall=3, scientific=FALSE)
		col3<-format(round(temp2[3], 2), nsmall=2, scientific=FALSE)
		col4<-format(round(temp2[4], 4), nsmall=4, scientific=FALSE)

		blanq<-c(" ")
		for (i in 1:dim(temp)[1])
		{ 
			blanq[i]=" "
		}

		ivsanova<-cbind(col1,blanq,temp2[2],blanq,col2,blanq,col3,blanq,col4)

		source2<-rownames(ivsanova)
#STB March 2014 - Replacing : with * in ANOVA table
		for (q in 1:(notreatfactors+nocontfactors))
		{
			source2<-sub(":"," * ", source2) 
		}	
		rownames(ivsanova)<-source2
		source3<-rownames(ivsanova)

		ivsanova[length(unique(source2)),7]<-" "
		ivsanova[length(unique(source2)),9]<-" "
#STB May 2012 capitals changed
		head<-c("Sums of squares", " ","Degrees of freedom"," ","Mean square"," ","F-value"," ","p-value")
		colnames(ivsanova)<-head
	
		for (i in 1:(dim(ivsanova)[1]-1)) 
		{
			if (temp2[i,4]<0.0001) 
			{
#STB March 2011 formatting p-values p<0.0001
				ivsanova[i,9]=format(round(0.0001, 4), nsmall=4, scientific=FALSE)
				ivsanova[i,9]<- paste("<",ivsanova[i,9])
			}
		}

		HTML(ivsanova, classfirstline="second", align="left")
		HTML.title("</bf> ", HR=2, align="left")

		if(covariateModel != "NULL")
		{
#STB Error spotted:
#	HTML.title("<sTitle<-sub("ivs_colon_ivs"	,":"ML.title("<bf>Comment: ANCOVA table calculated using a Type III model fit, see Armitage et al. (2001).", HR=0, align="left")
		HTML.title("Comment: ANCOVA table calculated using a Type III model fit, see Armitage et al. (2001).", HR=0, align="left")
		} else {
		HTML.title("</bf> ", HR=2, align="left")
		HTML.title("<bf>Comment: ANOVA table calculated using a Type III model fit, see Armitage et al. (2001).", HR=0, align="left")
		}

		add<-paste(c("Conclusion"))
		inte<-1
		for(i in 1:(dim(ivsanova)[1]-1))
		{
			if (ivsanova[i,9]<= (1-sig))
			{
				if (inte==1)
				{
					inte<-inte+1
					add<-paste(add, ": There is a statistically significant effect of ", sep="")
					add<-paste(add, rownames(ivsanova)[i], sep="")
				} else {
					inte<-inte+1
					add<-paste(add, ", ", sep="")
					add<-paste(add, rownames(ivsanova)[i], sep="")
				}
			} 
		}
		if (inte==1)
		{
			if (dim(ivsanova)[1]>2)
			{
	
				if(covariateModel != "NULL")
				{
#STB July 2013 change wording to remove effects
				add<-paste(add, ": None of the terms in the ANCOVA table are statistically significant.", sep="")
				} else {
				add<-paste(add, ": None of the terms in the ANOVA table are statistically significant.", sep="")
				}
			} else {
				add<-paste(add, ": The continuous factor is not statistically significant.", sep="")
			}
		} else {
			add<-paste(add, ". ", sep="")
		}				
	
		HTML.title("</bf> ", HR=2, align="left")
		HTML.title(add, HR=0, align="left")
	}
}
#============================================================================================================================
#Table of regression coefficients
#============================================================================================================================
options(contrasts=c("contr.SAS", "contr.SAS"))
threewayfull2<-lm(model, data=statdata, na.action = na.omit)

if (showCoefficients == "Y")
{
	HTMLbr()
	HTML.title("<bf>Table of model coefficients", HR=2, align="left")
	temp1<-coefficients(threewayfull2) # model coefficients

	temp2<- confint(threewayfull2, level=sig) # CIs for model parameters 
	temp3<- cbind(temp1, temp2)

	tablenames<-rownames(temp3)
#	tablenames<- sub(" Treatment", " ", tablenames)

	for (i in 1:100)
	{
		tablenames<- sub(":", " * ", tablenames)
	}	
	rownames(temp3)<-tablenames
	colnam<-c("Estimate", "lower","upper")
	colnam[2]<-paste("Lower ",(sig*100),"% CI",sep="")
	colnam[3]<-paste("Upper ",(sig*100),"% CI",sep="")
	
	colnames(temp3)<- colnam

	HTML(temp3, classfirstline="second", align="left")

	HTML.title("<bf> ", HR=2, align="left")
	HTML.title("<bf>Note: These model coefficients can be added together to obtain the model-based estimates of the relationships between the factors and the response, see Chambers and Hastie (1992).", HR=0, align="left")

}



#============================================================================================================================
#Covariate correlation table
#============================================================================================================================

if (CovariateRegressionCoefficients == "Y" && covariateModel != "NULL")
{
	HTMLbr()
	HTML.title("<bf>Covariate regression coefficients", HR=2, align="left")

	covtable_1<-coef(summary(threewayfull))
	covtable<-data.frame(covtable_1)[c(2),]

	covtable_2<- covtable

	covtable$Estimate <-format(round(covtable$Estimate, 4), nsmall=4, scientific=FALSE) 
	covtable$Std..Error <-format(round(covtable$Std..Error, 3), nsmall=3, scientific=FALSE) 
	covtable$t.value <-format(round(covtable$t.value, 2), nsmall=2, scientific=FALSE) 
	covtable$Pr...t.. <-format(round(covtable$Pr...t.., 4), nsmall=4, scientific=FALSE) 

	covtable_1<- covtable

	if (as.numeric(covtable_2[1,4])<0.0001) 
	{
#STB March 2011 formatting p-values p<0.0001
#		ivsanova[i,9]<-0.0001
		covtable_1[1,4]= "<0.0001"
	}

	rz<-rownames(covtable)[1]
	rownames(covtable_1)<-c(rz)

	colnames(covtable_1)<-c("Estimate", "Std error", "t-value", "p-value")
	HTML(covtable_1, classfirstline="second", align="left")
}




#============================================================================================================================
#R squared
#============================================================================================================================

if (showAdjustedRSquared =="Y")
{
	HTMLbr()
	HTML.title("<bf>R-squared and Adjusted R-squared statistics", HR=2, align="left")

	rsq    <- format(round(summary(threewayfull)$r.squared,4),nsmall=4)
	adjrsq <- format(round(summary(threewayfull)$adj.r.squared,4),nsmall=4)

	rtab<-cbind(rsq,adjrsq)

	colnames(rtab)<-c("R-squared", "Adjusted R-sq")
	rownames(rtab)<- c("Estimate")
	HTML(rtab, classfirstline="second", align="left")

	HTML.title("</bf> ", HR=2, align="left")
	HTML.title("The R-squared is the fraction of the variance explained by the model. A value close to 1 implies the statistical model fits the data well. 
	Unfortunately adding additional variables to the statistical model will always increase R-sq, regardless of their importance. The Adjusted R-sq adjusts for the number of terms in the model and may decrease if over-fitting has occurred. 
	If there is a large difference between R-sq and Adjusted R-sq, then non-significant terms may have been included in the statistical model.
	", HR=0, align="left")
}


#============================================================================================================================
#Diagnostic plots
#============================================================================================================================

#Diagnostic plot titles
if(residualsVsPredictedPlot=="Y" || normalProbabilityPlot == "Y")
{
	HTMLbr()
	HTML.title("<bf>Diagnostic plots", HR=2, align="left")
} else {
	if(showNormPlot=="Y")
	{
		HTMLbr()
		HTML.title("<bf>Diagnostic plots", HR=2, align="left")
	}
}

#Residual plots
if(residualsVsPredictedPlot=="Y")
{
	residualPlot <- sub(".html", "residualplot.jpg", htmlFile)
	jpeg(residualPlot,width = jpegwidth, height = jpegheight, quality = 100)

#STB July2013
	plotFilepdf3 <- sub(".html", "residualplot.pdf", htmlFile)
	dev.control("enable") 

#==================================================================================================================================================
#==================================================================================================================================================
#Graphical parameters
graphdata<-data.frame(cbind(predict(threewayfull),rstudent(threewayfull)))

graphdata$yvarrr_IVS <- graphdata$X2
graphdata$xvarrr_IVS <- graphdata$X1
XAxisTitle <- "Predicted values"
YAxisTitle <- "Externally Studentised residuals"
MainTitle2 <- "Residuals vs. predicted plot \n"
w_Gr_jit <- 0
h_Gr_jit <-  0
infiniteslope <- "Y"

#if (bandw != "N") 
#{
#	Gr_line <-BW_line
#	Gr_fill <- BW_fill
#} else {
#	Gr_line <-Col_line
#	Gr_fill <- Col_fill
#	}
Gr_line_type<-Line_type_dashed

Line_size2 <- Line_size
Line_size <- 0.5

#GGPLOT2 code
NONCAT_SCAT("RESIDPLOT")

MainTitle2 <- ""
#==================================================================================================================================================
#==================================================================================================================================================

	void<-HTMLInsertGraph(GraphFileName=sub("[A-Z0-9a-z,:,\\\\]*App_Data[\\\\]","", residualPlot), Align="centre")

#STB July2013
	if (pdfout=="Y")
	{
		pdf(file=sub("[A-Z0-9a-z,:,\\\\]*App_Data[\\\\]","", plotFilepdf3), height = pdfheight, width = pdfwidth) 
		dev.set(2) 
		dev.copy(which=3) 
		dev.off(2)
		dev.off(3)
		pdfFile_3<-sub("[A-Z0-9a-z,:,\\\\]*App_Data[\\\\]","",plotFilepdf3)
		linkToPdf3 <- paste ("<a href=\"",pdfFile_3,"\">Click here to view the PDF of the residuals vs. predicted plot</a>", sep = "")
		HTML(linkToPdf3)
	}

	HTML.title("<bf> ", HR=2, align="left")
	HTML.title("<bf>Tip: On this plot look to see if the spread of the points increases as the predicted values increase. If so the response may need transforming.", HR=0, align="left")
	HTML.title("<bf> ", HR=2, align="left")
	HTML.title("<bf>Tip: Any observation with a residual less than -3 or greater than 3 (SD) should be investigated as a possible outlier.", HR=0, align="left")
}

#Normality plots
if(normalProbabilityPlot=="Y")
{
	HTMLbr()
	normPlot <- sub(".html", "normplot.jpg", htmlFile)
	jpeg(normPlot,width = jpegwidth, height = jpegheight, quality = 100)

#STB July2013
	plotFilepdf4 <- sub(".html", "normplot.pdf", htmlFile)
	dev.control("enable") 


#==================================================================================================================================================
#==================================================================================================================================================
#Graphical parameters

	te<-qqnorm(resid(threewayfull))
	graphdata<-data.frame(te$x,te$y)
	graphdata$xvarrr_IVS <-graphdata$te.x
	graphdata$yvarrr_IVS <-graphdata$te.y
	YAxisTitle <-"Sample Quantiles"
	XAxisTitle <-"Theoretical Quantiles"
	MainTitle2 <- "Normal probability plot \n"
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
#==================================================================================================================================================
#==================================================================================================================================================
	void<-HTMLInsertGraph(GraphFileName=sub("[A-Z0-9a-z,:,\\\\]*App_Data[\\\\]","", normPlot), Align="left")

#STB July2013
	if (pdfout=="Y")
	{
		pdf(file=sub("[A-Z0-9a-z,:,\\\\]*App_Data[\\\\]","", plotFilepdf4), height = pdfheight, width = pdfwidth) 
		dev.set(2) 
		dev.copy(which=3) 
		dev.off(2)
		dev.off(3)
		pdfFile_4<-sub("[A-Z0-9a-z,:,\\\\]*App_Data[\\\\]","",plotFilepdf4)
		linkToPdf4 <- paste ("<a href=\"",pdfFile_4,"\">Click here to view the PDF of the normal probability plot</a>", sep = "")
		HTML(linkToPdf4)
	}

	HTML.title("<bf> ", HR=2, align="left")
	HTML.title("<bf>Tip: Check that the points lie along the dotted line. If not then the data may be non-normally distributed.", HR=0, align="left")
}







#============================================================================================================================
#Cooks distance
#============================================================================================================================

if (cooksDistancePlot =="Y")
{
	HTMLbr()
	HTML.title("<bf>Cook's distance plot", HR=2, align="left")

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

#OUtput code
	void<-HTMLInsertGraph(GraphFileName=sub("[A-Z0-9a-z,:,\\\\]*App_Data[\\\\]","", ncscatterplotx), Align="centre")

#STB July2013
	if (pdfout=="Y")
	{
		pdf(file=sub("[A-Z0-9a-z,:,\\\\]*App_Data[\\\\]","", plotFilepdf16), height = pdfheight, width = pdfwidth) 
		dev.set(2) 
		dev.copy(which=3) 
		dev.off(2)
		dev.off(3)
		pdfFile_16<-sub("[A-Z0-9a-z,:,\\\\]*App_Data[\\\\]","",plotFilepdf16)
		linkToPdf16 <- paste ("<a href=\"",pdfFile_16,"\">Click here to view the PDF of the scatterplot</a>", sep = "")
		HTML(linkToPdf16)
	}

	HTML.title("</bf> ", HR=2, align="left")
	HTML.title("
	This plot should be used to assess whether there are any potential outliers in the dataset. Observations 
	where the Cook's distance are above the cut-off line should be investigated further. Note the cut-off line has 
	been calculated using the 4/n approach, where n is the number of observations in the dataset. 
	", HR=0, align="left")
}


#============================================================================================================================
#Leverage plot
#============================================================================================================================

if (leveragesPlot =="Y")
{
	HTMLbr()
	HTML.title("<bf>Leverage plot", HR=2, align="left")

	lev <- hat(model.matrix(threewayfull))
	graphdata<- na.omit(statdata)
	graphdata<-cbind(graphdata, lev)
	graphdata$yvarrr_IVS<-graphdata$lev
	graphdata$xvarrr_IVS<- c(1:length(lev))
	XAxisTitle<-"Response number"
	YAxisTitle<-"Leverage"
	LinearFit <- "N"
	infiniteslope <- "N"

#Plot device setup
	ncscatterplot <- sub(".html", "ncscatterplot.jpg", htmlFile)
	jpeg(ncscatterplot,width = jpegwidth, height = jpegheight, quality = 100)

#STB July2013
	plotFilepdf15 <- sub(".html", "ncscatterplot.pdf", htmlFile)
	dev.control("enable") 

#GGPLOT2 code
	NONCAT_SCAT("none")

#OUtput code
	void<-HTMLInsertGraph(GraphFileName=sub("[A-Z0-9a-z,:,\\\\]*App_Data[\\\\]","", ncscatterplot), Align="centre")

#STB July2013
	if (pdfout=="Y")
	{
		pdf(file=sub("[A-Z0-9a-z,:,\\\\]*App_Data[\\\\]","", plotFilepdf15), height = pdfheight, width = pdfwidth) 
		dev.set(2) 
		dev.copy(which=3) 
		dev.off(2)
		dev.off(3)
		pdfFile_15<-sub("[A-Z0-9a-z,:,\\\\]*App_Data[\\\\]","",plotFilepdf15)
		linkToPdf15 <- paste ("<a href=\"",pdfFile_15,"\">Click here to view the PDF of the scatterplot</a>", sep = "")
		HTML(linkToPdf15)
	}


	HTML.title("</bf> ", HR=2, align="left")
	HTML.title("
	This plot indicates the relative influence of the observations. Observations with a high leverage may be unduly influencing the 
	statistical model.
	", HR=0, align="left")
}



#============================================================================================================================
#Analysis description
#============================================================================================================================
HTMLbr()
HTML.title("<bf>Analysis description", HR=2, align="left")

add<-c("The data were analysed using an ")

if (nocontfactors==1) 
{
	if(covariateModel != "NULL")
	{
		add<-paste(add, "ANCOVA approach, with continuous factor ", sep="")
	} else {
			add<-paste(add, "ANOVA approach, with continuous factor ", sep="")
		}
	add<-paste(add, ContinuousList, sep="")
} else {
	if(covariateModel != "NULL")
	{
		add<-paste(add, "ANCOVA approach, with continuous factors ", sep="")
	} else {
			add<-paste(add, "ANOVA approach, with continuous factors ", sep="")
		}
		for (i in 1:nocontfactors)
		{
			if (i<nocontfactors)
			{
				add<-paste(add, ContinuousList[i], sep="")
				add<-paste(add, ", ", sep="")
			} else {
					if (i==nocontfactors)
					{
						add<-paste(add, ContinuousList[i], sep="")
					}
				}
		}	
	}


if (notreatfactors==1) 
{
	add<-paste(add, " and categorical treatment factor ", sep="")
	add<-paste(add, treatFactors, sep="")
} 

if (notreatfactors>1) 
{

	add<-paste(add, " and categorical treatment factors ", sep="")

	for (i in 1:notreatfactors)
	{
	 	if (i<notreatfactors)
		{
			add<-paste(add, TreatList[i+1], sep="")
			add<-paste(add, ", ", sep="")
		} 
		if (i==notreatfactors)
		{
			add<-paste(add, TreatList[i+1], sep="")
		}
	}
}

if (blockFactors != "NULL" && covariateModel != "NULL") 
{
	add<-paste(add, ", ", sep="")
} else if (noblockfactors==1 && blockFactors != "NULL" && covariateModel == "NULL") 
{
	add<-paste(add, " and ", sep="")
} 
	
if (noblockfactors==1 && blockFactors != "NULL") 
{
	add<-paste(add, blockFactors, sep="")
	add<-paste(add, " as a blocking factor", sep="")
} else {
	if(noblockfactors>1) # there is two or more blocking factors
	{
		for (i in 1:noblockfactors)
		{
			if (i<noblockfactors-1)
			{
				add<-paste(add, BlockList[i+1], sep="")
				add<-paste(add, ", ", sep="")
			} else	if (i<noblockfactors)
			{
				add<-paste(add, BlockList[i+1], sep="")
				add<-paste(add, " and ", sep="")
			} else if (i==noblockfactors)
			{
				add<-paste(add, BlockList[i+1], sep="")
			}
		}
		add<-paste(add, " as blocking factors", sep="")
	}
}

if (covariateModel == "NULL") 
{
	add<-paste(add, ". ", sep="")
} else if(covariateModel != "NULL")	{
	add<-paste(add, " and  ", sep="")
	add<-paste(add, unlist(strsplit(covariateModel, "~"))[2], sep="")
	add<-paste(add, " as the covariate. ", sep="")
}



if (responseTransform != "None")
{
	add<-paste(add, " The response was ", sep="")
	add<-paste(add, responseTransform, sep="")
	add<-paste(add, " transformed prior to analysis to stabilise the variance. ", sep="")
}

HTML.title("</bf> ", HR=2, align="left")
HTML.title(add, HR=0, align="left")


#----------------------------------------------------------------------------------------------------------
#References
#----------------------------------------------------------------------------------------------------------
Ref_list<-R_refs()

#Bate and Clark comment
HTML.title("<bf> ", HR=2, align="left")
HTML.title(refxx, HR=0, align="left")	
HTML.title("<bf> ", HR=2, align="left")

HTMLbr()
HTML.title("<bf>Statistical references", HR=2, align="left")

HTML.title("<bf> ", HR=2, align="left")
HTML.title(Ref_list$BateClark_ref, HR=0, align="left")

if(showANOVA=="Y")
{
	HTML.title("<bf> ", HR=2, align="left")
	HTML.title("<bf> Armitage P, Matthews JNS and Berry G. (2001). Statistical Methods in Medical Research. 4th edition; John Wiley & Sons. New York.", HR=0, align="left")
}

if(showCoefficients == "Y")
{
	HTML.title("<bf> ", HR=2, align="left")
	HTML.title("<bf> Chambers JM and Hastie TJ. (1992). Statistical Models in S. Wadsworth and Brooks-Cole advanced books and software.", HR=0, align="left")
}

if(covariateModel != "NULL")
{
	HTML.title("<bf> ", HR=2, align="left")
	HTML.title("<bf> Morris TR. (1999). Experimental Design and Analysis in Animal Sciences. CABI publishing. Wallingford, Oxon (UK).", HR=0, align="left")
}


HTMLbr()
HTML.title("<bf>R references", HR=2, align="left")

HTML.title("<bf> ", HR=2, align="left")
HTML.title(Ref_list$R_ref , HR=0, align="left")

HTML.title("<bf> ", HR=2, align="left")
HTML.title(Ref_list$GGally_ref, HR=0, align="left")

HTML.title("<bf> ", HR=2, align="left")
HTML.title(Ref_list$RColorBrewers_ref, HR=0, align="left")

HTML.title("<bf> ", HR=2, align="left")
HTML.title(Ref_list$GGPLot2_ref, HR=0, align="left")

HTML.title("<bf> ", HR=2, align="left")
HTML.title(Ref_list$reshape_ref, HR=0, align="left")

HTML.title("<bf> ", HR=2, align="left")
HTML.title(Ref_list$plyr_ref, HR=0, align="left")

HTML.title("<bf> ", HR=2, align="left")
HTML.title(Ref_list$scales_ref, HR=0, align="left")

HTML.title("<bf> ", HR=2, align="left")
HTML.title(Ref_list$car_ref, HR=0, align="left")

HTML.title("<bf> ", HR=2, align="left")
HTML.title(Ref_list$R2HTML_ref, HR=0, align="left")

HTML.title("<bf> ", HR=2, align="left")
HTML.title(Ref_list$PROTO_ref, HR=0, align="left")


#----------------------------------------------------------------------------------------------------------
#Show dataset
#----------------------------------------------------------------------------------------------------------


if (showdataset=="Y")
{

	statdata$scatterPlotColumn <-NULL
	statdata$FirstCatFactor <-NULL
	HTMLbr()
	HTML.title("<bf>Analysis dataset", HR=2, align="left")
	HTML(statdata, classfirstline="second", align="left")
}














