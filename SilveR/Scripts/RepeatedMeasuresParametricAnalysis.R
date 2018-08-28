#===================================================================================================================
#R Libraries

suppressWarnings(library(multcomp))
suppressWarnings(library(R2HTML))
suppressWarnings(library(nlme))
suppressWarnings(library(contrast))
suppressWarnings(library(lsmeans))

#STB 14OCT2015
#Set contrast options for Marginal overall tests
options(contrasts=c(unordered="contr.sum", ordered="contr.poly"))

#===================================================================================================================

#===================================================================================================================
# retrieve args
Args <- commandArgs(TRUE)

#Read in data
statdata <- read.csv(Args[3], header=TRUE, sep=",")

#Copy Args
model <- as.formula(Args[4])
timeFactor <- Args[5]
subjectFactor <- Args[6]
covariateModel <- Args[7]
covariance <- Args[8]
responseTransform <- Args[9]
covariateTransform <- Args[10]
FirstCatFactor <- Args[11]
treatFactors <- Args[12]
blockFactors <- Args[13]
showANOVA <- Args[14]
showPRPlot <- Args[15]
showNormPlot <- Args[16]
sig <- 1 - as.numeric(Args[17])
effectModel <- as.formula(Args[18])
effectModel2 <- Args[18]
selectedEffect <- Args[19]
showLSMeans <- Args[20]
allPairwiseTest <- Args[21]

#working directory
direct2<- unlist(strsplit(Args[3],"/"))
direct<-direct2[1]
for (i in 2:(length(direct2)-1))
{
	direct<- paste(direct, "/", direct2[i], sep = "")
}

#source(paste(getwd(),"/Common_Functions.R", sep=""))
#===================================================================================================================

#Graphics parameter setup
#===================================================================================================================
graphdata<-statdata
if (FirstCatFactor != "NULL")
{
	Gr_palette<-palette_FUN(FirstCatFactor)
}
Labelz_IVS_ <- "N"

#STB OCT2015
Line_size2 <- Line_size
#===================================================================================================================

#Setup the html file and associated css file
htmlFile <- sub(".csv", ".html", Args[3]); #determine the file name of the html file
HTMLSetFile(file=htmlFile) 
cssFile <- "r2html.css"
cssFile <- paste("'",cssFile,"'", sep="") #need to enclose in quotes when path has spaces in it
HTMLCSS(CSSfile = cssFile)

#===================================================================================================================
# Setting up the parameters
resp <- unlist(strsplit(Args[4],"~"))[1] #get the response variable from the main model
statdata$subjectzzzzzz<-as.factor(eval(parse(text = paste("statdata$", subjectFactor))))
statdata$Timezzz<-as.factor(eval(parse(text = paste("statdata$", timeFactor))))
statdata<-statdata[order(statdata$subjectzzzzzz, statdata$Timezzz), ]

#Tidying up the selectedEffect
selectedEffect<-gsub(eval(timeFactor), "Timezzz",selectedEffect) 
selectedEffect<-gsub("ivs_sp_ivs*ivs_sp_ivs", "*",selectedEffect,fixed=TRUE) 

#Removing illegal charaters

YAxisTitle <-resp
CPXAxisTitle <-timeFactor

if(FirstCatFactor != "NULL")
{
	XAxisTitle<-unlist(strsplit(covariateModel, "~"))[2]
}

for (i in 1:10)
{
	YAxisTitle<-namereplace(YAxisTitle)
	CPXAxisTitle<-namereplace(CPXAxisTitle)
	timeFactor_plot <- namereplace(timeFactor)

	if(FirstCatFactor != "NULL")
	{
		XAxisTitle<-namereplace(XAxisTitle)
	}
}
LS_YAxisTitle<-YAxisTitle

#calculating number of block and treatment factors
tempblockChanges <-strsplit(blockFactors, ",")
txtexpectedblockChanges <- c("")
for(i in 1:length(tempblockChanges[[1]])) 
{
	txtexpectedblockChanges [length(txtexpectedblockChanges )+1]=(tempblockChanges[[1]][i]) 
}
noblockfactors<-length(txtexpectedblockChanges)-1

tempChanges <-strsplit(treatFactors, ",")
txtexpectedChanges <- c("")
for(i in 1:length(tempChanges[[1]])) 
{ 
	txtexpectedChanges [length(txtexpectedChanges )+1]=(tempChanges[[1]][i]) 
}
notreatfactors<-length(txtexpectedChanges)-1

#need to add this code to the single measure module code?
if (tempblockChanges == "NULL")
{
	noblockfactors = 0
}

# Testing the factorial combinations
intindex<-length(unique(statdata$betweenwithin))
timeindex<-length(unique(eval(parse(text = paste("statdata$", timeFactor)))))

ind<-1
for (i in 1:notreatfactors)
{
	ind=ind*length(unique(eval(parse(text = paste("statdata$", txtexpectedChanges[i+1])))))
}
ind=ind*timeindex

if(intindex != ind)
{
	add<-c("Unfortunately not all combinations of the levels of the treatment factors are present, or not all combinations are present at each level of the repeated factor, in the experimental design. We recommend you manually create a new factor corresponding to the combinations of the levels of the treatment factors or remove the incomplete levels of the repeated factors from the analysis.")
	HTML.title("</bf> ", HR=2, align="left")
	HTML.title(add, HR=0, align="left")
	print(add)
	quit()
}


# Code to create varibale to test if the highest order interaction is selected

testeffects = noblockfactors
if(FirstCatFactor != "NULL")
{
testeffects = noblockfactors+1
}
emodel <-strsplit(effectModel2, "+", fixed = TRUE)

emodelChanges <- c("")
for(i in 1:length(emodel[[1]])) 
{ 
	emodelChanges [length(emodelChanges )+1]=(emodel[[1]][i]) 
}
noeffects<-length(emodelChanges)-2



#===================================================================================================================
# Titles and description
#===================================================================================================================
#Output HTML header
Title <-paste(branding, " Repeated Measures Parametric Analysis", sep="")
HTML.title(Title, HR = 1, align = "left")

#Response
title<-c("Response")
if(FirstCatFactor != "NULL")
{
	title<-paste(title, ", covariate", sep="")
}

	title<-paste(title, " and covariance structure", sep="")



HTML.title(title, HR=2, align="left")
add<-paste(c("The  "), resp, sep="")
add<-paste(add, " response is currently being analysed by the Repeated Measures Parametric Analysis module", sep="")

if(FirstCatFactor != "NULL")
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

if(covariance=="Compound Symmetric")
{
	HTML.title("</bf> ", HR=2, align="left")
	add4<-c("The repeated measures mixed model analysis is using the compound symmetric covariance structure to model the within-subject correlations. When using this structure you are assuming sphericity and also that the variability of responses is the same at each level of ")
	add4<-paste(add4, timeFactor, sep= "")
	add4<-paste(add4, ", see Pinherio and Bates (2002). These assumptions may not hold in practice.", sep ="")
	HTML.title(add4, HR=0, align="left")
}
if(covariance=="Autoregressive(1)")
{
	HTML.title("</bf> ", HR=2, align="left")
	add4<-c("The repeated measures mixed model analysis is using the first order autoregressive covariance structure to model the within-subject correlations. When using this structure you are assuming the levels of ")
	add4<-paste(add4, timeFactor, sep= "")
	add4<-paste(add4, " are equally spaced and also that the variability of responses are the same at each level of ", sep= "")
	add4<-paste(add4, timeFactor, sep= "")
	add4<-paste(add4, ", see Pinherio and Bates (2002). These assumptions may not hold in practice.", sep= "")
	HTML.title(add4, HR=0, align="left")

	HTML.title("</bf> ", HR=2, align="left")
#STB May 2012 Updating "least square (predicted) means"
	add4<-c("Warning: Make sure that the levels of the repeated factor occur in the correct order in the least square (predicted) means table. If they do not then this analysis may not be valid. The autoregressive covariance structure assumes that the order of the repeated factor levels is as defined in the least square (predicted) means table.")
	HTML.title(add4, HR=0, align="left")

}
if(covariance=="Unstructured")
{
	HTML.title("</bf> ", HR=2, align="left")
	HTML.title("<bf>The repeated measures mixed model analysis is using the unstructured covariance structure to model the within-subject correlations. When using this structure you are estimating many parameters. If the numbers of subject used is small then these estimates may be unreliable, see Pinherio and Bates (2002).", HR=0, align="left")
}


#===================================================================================================================
#Case profiles plot
#===================================================================================================================

HTMLbr()
title<-c("Categorised case profiles plot of the raw data")
if(responseTransform != "None")
{
	title<-paste(title, " (on the ", sep="")
	title<-paste(title, responseTransform, sep="")
	title<-paste(title, " scale)", sep="")
}

HTML.title(title, HR=2, align="left")
scatterPlot <- sub(".html", "scatterPlot.jpg", htmlFile)
jpeg(scatterPlot,width = jpegwidth, height = jpegheight, quality = 100)

#STB July2013
plotFilepdf1 <- sub(".html", "scatterPlot.pdf", htmlFile)
dev.control("enable") 


#===================================================================================================================
# One-categorised case profiles plot
#===================================================================================================================
#Parameter setup
graphdata$Time_IVS <- as.factor(eval(parse(text = paste("graphdata$", timeFactor))))
graphdata$yvarrr_IVS <- eval(parse(text = paste("graphdata$", resp)))
graphdata$Animal_IVS <- as.factor(eval(parse(text = paste("graphdata$", subjectFactor))))
graphdata$l_l <-graphdata$between
XAxisTitle<-CPXAxisTitle
MainTitle2 <-""
ReferenceLine = "NULL"


#Colour range for individual animals on case profiles plot
temp<-IVS_F_cpplot_colour("Animal_IVS")
Gr_palette_A <- temp$Gr_palette_A
Gr_line <- temp$Gr_line
Gr_fill <-temp$Gr_fill

#GGPLOT2 code
ONECATSEP_CPP()
#===================================================================================================================
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
	linkToPdf1 <- paste ("<a href=\"",pdfFile_1,"\">Click here to view the PDF of the categorised case profiles plot</a>", sep = "")
	HTML(linkToPdf1)
}

#HTML.title("</bf> ", HR=2, align="left")
#HTML.title("</bf> Tip: Use this plot to identify possible outlier (subjects and individual observations) and decide on suitable summary measures.", HR=0, align="left")


#===================================================================================================================
#Covariate plot
#===================================================================================================================

if(FirstCatFactor != "NULL")
{
	title<-c("Covariate plot of the raw data")
	if(responseTransform != "None" || covariateTransform != "None")
	{
		title<-paste(title, " (on the transformed scale)", sep="")
	} 
	HTMLbr()
	HTML.title(title, HR=2, align="left")

    	ncscatterplot3 <- sub(".html", "ncscatterplot3.jpg", htmlFile)
    	jpeg(ncscatterplot3,width = jpegwidth, height = jpegheight, quality = 100)

#STB July2013
	plotFilepdf2 <- sub(".html", "ncscatterplot3.pdf", htmlFile)
	dev.control("enable") 

#===================================================================================================================
#Graphical parameters
	graphdata<-statdata
	graphdata$xvarrr_IVS <- eval(parse(text = paste("graphdata$",unlist(strsplit(covariateModel, "~"))[2])))
	graphdata$yvarrr_IVS <- eval(parse(text = paste("graphdata$",resp)))
	graphdata$Time_IVS <- as.factor(eval(parse(text = paste("graphdata$", timeFactor))))
	graphdata$tempvariable_ivs <- paste(eval(parse(text = paste("graphdata$",FirstCatFactor))), graphdata$Time_IVS, sep = " ")
	graphdata$l_l <- graphdata$tempvariable_ivs
	graphdata$catfact <-graphdata$tempvariable_ivs

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

	Gr_palette<-palette_FUN("catfact")

#GGPLOT2 code
	OVERLAID_SCAT()
#===================================================================================================================
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
}

#===================================================================================================================
#building the covariate interaction model
#===================================================================================================================
if (AssessCovariateInteractions == "Y" && FirstCatFactor != "NULL")
{

# Defining the Response
	Resplist <-unlist(strsplit(covariateModel, "~"))[1]
	
# Defining the covariate
	Covlist <-unlist(strsplit(covariateModel, "~"))[2]

#Creating the list of model terms
	listmodel <- unlist(strsplit(Args[4],"~"))[2] #get the main model
	temChanges <-strsplit(listmodel, "+", fixed = TRUE)
	Modellist <- c("")
	for(i in 1:length(temChanges[[1]])) 
	{ 
		Modellist[i]=(temChanges[[1]][i]) 
	}

#Creating list of blocking factor
	if (noblockfactors > 0)
	{
		Blocklist <- c()
		for (i in 2:length(txtexpectedblockChanges))
		{
			Blocklist[i-1] = txtexpectedblockChanges [i]
		}
	}
#Creating the list of treatment terms
	Treatlist<-c()
	for (i in (1+noblockfactors+1):length(Modellist))
	{
		Treatlist[i-(1+noblockfactors+1)+1] = Modellist[i]
	}

#Creating the list of interaction terms
	Intlist <- c()
	for (i in 1:(length(Treatlist)))
	{
		Intlist[i] = paste (Covlist , "*" , Treatlist[i], sep = "" )
	}

#Creating the covariate interaction model
	Fulllist <- c(Treatlist,Intlist)

	CovIntModela<-c(Covlist)

	if (noblockfactors > 0)
	{
		for (i in 1:noblockfactors)
		{
			CovIntModela[i+1] <- paste(CovIntModela[i] , " + " , Blocklist[i])
		}
	}
	CovIntModelb <- CovIntModela
	for (i in 1:length(Fulllist))
	{
		CovIntModelb[i+length(CovIntModela)] <- paste(CovIntModelb[i+length(CovIntModela)-1] , " + " , Fulllist[i])
	}
	CovIntModel <- CovIntModelb[length(CovIntModelb)]
	
#Creating the formula
	CovIntForm <- paste(Resplist , " ~ ", CovIntModel , sep = "")



#Test to see if there are 0 df, in which case end module
	modelxx <- paste(CovIntForm , " + ", subjectFactor , sep = "")
	threewayfullxx<-lm(as.formula(modelxx) , data=statdata, na.action = (na.omit))
	if (df.residual(threewayfullxx) < 1)
	{
		HTMLbr()
		HTML.title("<bf>Table of overall tests of model effects, for assessing covariate interactions", HR=2, align="left")
		add<-c("When the covariate interactions are included in the statistical model there are not enough degrees of freedom to estimate all the effects, hence no table of overall tests of model effects (including the covariate interactions) has been produced.")
		HTML.title("</bf> ", HR=2, align="left")
		HTML.title(add, HR=0, align="left")	
	} else {

#Creating the analysis including covariates
		if(covariance=="Compound Symmetric")
		{
			threewayfullx<-lme(as.formula(CovIntForm), random=~1|subjectzzzzzz, data=statdata,correlation=corCompSymm(),  na.action = (na.omit), method = "REML")
		}
		if(covariance=="Autoregressive(1)")
		{
			threewayfullx<-lme(as.formula(CovIntForm), random=~1|subjectzzzzzz, correlation=corAR1(value=0.999, form=~as.numeric(Timezzz)|subjectzzzzzz, fixed =FALSE), data=statdata, na.action = (na.omit), method = "REML")
		}
		if(covariance=="Unstructured")
		{
			threewayfullx<-lme(as.formula(CovIntForm), random=~1|subjectzzzzzz, correlation= corSymm(form = ~ as.numeric(Timezzz) | subjectzzzzzz), weights=varIdent(form=~ 1 |as.numeric(Timezzz)), data=statdata, na.action = (na.omit), method = "REML")
		}

#STB Aug 2014 add in marginal sums of squares
#tempx<-anova(threewayfullx, type="sequential")
		tempx<-anova(threewayfullx, type="marginal")
	
		if (min(tempx[2], na.rm=TRUE) >= 1)
		{
			temp2x<-(tempx)
			col3x<-format(round(temp2x[3], 2), nsmall=2, scientific=FALSE)
			col4x<-format(round(temp2x[4], 4), nsmall=4, scientific=FALSE)

#STB May 2012 adding blanks
			blankx <-c(NA)

			ivsanovax<-cbind(temp2x[1],blankx, temp2x[2],blankx,col3x,blankx,col4x)
			source2x<-rownames(ivsanovax)
			source3x<-rownames(ivsanovax)
			headx<-c("Num. df"," ","Den. df"," ","F-value"," ","p-value")
			colnames(ivsanovax)<-headx

# Correction to code to ammend lowest p-value: STB Oct 2010
#	for (i in 1:(dim(ivsanovax)[1]-1)) 
			for (i in 1:(dim(ivsanovax)[1])) 
			{
				if (temp2x[i,4]<0.0001) 
				{
#STB - Mar 2011 formatting p<0.0001
#		ivsanovax[i,7]<-0.0001
					ivsanovax[i,7]=format(round(0.0001, 4), nsmall=4, scientific=FALSE)
					ivsanovax[i,7]<- paste("<",ivsanovax[i,7])
				}
			}

#Remove intercept row
			ivsanovax <- ivsanovax[-c(1), ] 
			tempyx<-gsub(pattern="Timezzz", replacement=timeFactor, rownames(ivsanovax))

#STB March 2014 - Replacing : with * in ANOVA table
			for (q in 1:length(tempyx))
			{
				tempyx<-sub(":"," * ", tempyx) 
			}
			rownames(ivsanovax)<-tempyx


#STB July 2013 Change title
			HTMLbr()
			HTML.title("<bf>Table of overall tests of model effects, for assessing covariate interactions", HR=2, align="left")
			HTML(ivsanovax, classfirstline="second", align="left")
	
			add<-c("Note: This table should only be used to assess the covariate interactions. The statistical model used to generate all the remaining results in this output does not include the covariate interactions.")
			HTML.title("</bf> ", HR=2, align="left")
			HTML.title(add, HR=0, align="left")
		}
	
		if (min(tempx[2], na.rm=TRUE) <= 0)
		{
			add<-c("The covariate interactions have not been calculated as there are zero residual degrees of freedom when all terms are included in the statistical model.")
			HTML.title("</bf> ", HR=2, align="left")
			HTML.title(add, HR=0, align="left")
			
		}
	}
}

#===================================================================================================================
#Set up final model
#===================================================================================================================
#Test to see if there are 0 df, in which case end module

model2 <- paste(Args[4] , " + ", subjectFactor , sep = "")
threewayfullxxx<-lm(as.formula(model2) , data=statdata, na.action = (na.omit))

if (df.residual(threewayfullxxx) < 1)
	{	
		HTMLbr()
		HTML.title("<bf>Table of overall tests of model effects", HR=2, align="left")
		add<-c("Unfortunately there are not enough degrees of freedom to estimate all the effects, hence no analysis has been produced. Please reduce the number of effects in your statistical model.")
		HTML.title("</bf> ", HR=2, align="left")
		HTML.title(add, HR=0, align="left")	
		quit()
	} 

#STB June 2015 - Takign a copy of the dataset
statdata_temp <-statdata

if(covariance=="Compound Symmetric")
{
	threewayfull<-lme(model, random=~1|subjectzzzzzz, data=statdata,correlation=corCompSymm(),  na.action = (na.omit), method = "REML")
}
if(covariance=="Autoregressive(1)")
{
	threewayfull<-lme(model, random=~1|subjectzzzzzz, correlation=corAR1(value=0.999, form=~as.numeric(Timezzz)|subjectzzzzzz, fixed =FALSE), data=statdata, na.action = (na.omit), method = "REML")
}
if(covariance=="Unstructured")
{
	threewayfull<-lme(model, random=~1|subjectzzzzzz, correlation= corSymm(form = ~ as.numeric(Timezzz) | subjectzzzzzz), weights=varIdent(form=~ 1 |as.numeric(Timezzz)), data=statdata, na.action = (na.omit), method = "REML")
}

#===================================================================================================================
#ANOVA Table
#===================================================================================================================

#STB Aug 2014 add in marginal sums of squares
#temp<-anova(threewayfull, type="sequential")
temp<-anova(threewayfull, type="marginal")
temp2<-(temp)
col3<-format(round(temp2[3], 2), nsmall=2, scientific=FALSE)
col4<-format(round(temp2[4], 4), nsmall=4, scientific=FALSE)

#STB May 2012 adding blanks
blank <-c(NA)

ivsanova<-cbind(temp2[1],blank, temp2[2],blank,col3,blank,col4)
source2<-rownames(ivsanova)
source3<-rownames(ivsanova)
head<-c("Num. df"," ","Den. df"," ","F-value"," ","p-value")
colnames(ivsanova)<-head

# Correction to code to ammend lowest p-value: STB Oct 2010
#	for (i in 1:(dim(ivsanova)[1]-1)) 
for (i in 1:(dim(ivsanova)[1])) 
{
	if (temp2[i,4]<0.0001) 
	{
#STB - Mar 2011 formatting p<0.0001
#		ivsanova[i,7]<-0.0001
		ivsanova[i,7]=format(round(0.0001, 4), nsmall=4, scientific=FALSE)
		ivsanova[i,7]<- paste("<",ivsanova[i,7])
	}
}

#Remove intercept row
ivsanova <- ivsanova[-c(1), ] 
tempy<-gsub(pattern="Timezzz", replacement=timeFactor, rownames(ivsanova))

#STB March 2014 - Replacing : with * in ANOVA table
for (q in 1:notreatfactors)
{
	tempy<-sub(":"," * ", tempy) 
}
	rownames(ivsanova)<-tempy

if(showANOVA=="Y")
{

#STB July 2013 Change title
	HTMLbr()
#	HTML.title("<bf>Overall tests of model effects", HR=2, align="left")
	HTML.title("<bf>Table of overall tests of model effects", HR=2, align="left")

	HTML(ivsanova, classfirstline="second", align="left")
	HTML.title("</bf> ", HR=2, align="left")

#STB August 2014 change in message
#	HTML.title("<bf>Comment: The overall tests in this table are likelihood ratio tests. ", HR=0, align="left")
	HTML.title("<bf>Comment: The overall tests in this table are marginal likelihood ratio tests, where the order they appear in the table does not influence the results.", HR=0, align="left")


	add<-paste(c("Conclusion"))
	inte<-1
	for(i in 1:(dim(ivsanova)[1]))
	{
#STB May 2012 correcting table reference
		if (ivsanova[i,7]<= (1-sig))
		{
			if (inte==1)
			{
				inte<-inte+1
				add<-paste(add, ": At the ", sep="")

				add<-paste(add, 100*(1-sig), sep="")
				add<-paste(add, "% level", sep="")

				add<-paste(add, " there is a statistically significant overall difference between the levels of ", sep="")
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
			add<-paste(add, ": There are no statistically significant overall differences, at the ", sep="")
			add<-paste(add, 100*(1-sig), sep="")
			add<-paste(add, "% level, ", sep="")

#STB July 2013 replace effects with terms and factor
			add<-paste(add, "between the levels of any of the terms in the table of overall tests", sep="")
		} else {
			add<-paste(add, ": There is no statistically significant overall difference, at the ", sep="")
			add<-paste(add, 100*(1-sig), sep="")
			add<-paste(add, "% level, ", sep="")			
			add<-paste(add, "between the levels of the treatment factor", sep="")	
		} 
	}		
	add<-paste(add, ". ", sep="")
	HTML.title("</bf> ", HR=2, align="left")
	HTML.title(add, HR=0, align="left")
	HTML.title("</bf> ", HR=2, align="left")
	HTML.title("<bf> Tip: While it is a good idea to consider the overall tests in the above table, we should not rely on them when deciding whether or not to make pairwise comparisons.", HR=0, align="left")
#STB May 2012 correcting table
	# Warning message for degrees of freedom
	if (min(ivsanova[3])<5)
	{
		HTMLbr()
		HTML.title("<bf>Warning", HR=2, align="left")
		add<-c("Unfortunately one or more of the residual degrees of freedom in the above table are low (less than 5). This may make the estimation of the underlying variability, and hence the results of the statistical tests, unreliable. This can be caused by attempting to fit too many factors, and their interactions, in the statistical model. Where appropriate we recommend you fit some of the 'Treatment' factors as 'Other design' factors. This will remove their interactions from the statistical model and therefore increase the residual degrees of freedom.")
		HTML.title("</bf> ", HR=2, align="left")
		HTML.title(add, HR=0, align="left")
		print(add)
	}
}



#===================================================================================================================
#Covariate correlation table
#===================================================================================================================

if (CovariateRegressionCoefficients == "Y"  && covariateModel != "NULL")
{
	HTMLbr()
	HTML.title("<bf>Covariate regression coefficient", HR=2, align="left")

	covtable_1<-summary(threewayfull)$tTable
	covtable<-data.frame(covtable_1)[c(2),]
	colnames(covtable) <- c("Estimate", "Std..Error", "DF", "t.value", "Pr...t..")

	covtable_2<- covtable

	covtable$Estimate <-format(round(covtable$Estimate, 3), nsmall=3, scientific=FALSE) 
	covtable$Std..Error <-format(round(covtable$Std..Error, 3), nsmall=3, scientific=FALSE) 
	covtable$DF <-format(round(covtable$DF, 0), nsmall=0, scientific=FALSE) 
	covtable$t.value <-format(round(covtable$t.value, 2), nsmall=2, scientific=FALSE) 
	covtable$Pr...t.. <-format(round(covtable$Pr...t.., 4), nsmall=4, scientific=FALSE) 

	covtable_1<- covtable

	if (as.numeric(covtable_2[1,5])<0.0001) 
	{
#STB March 2011 formatting p-values p<0.0001
#		ivsanova[i,9]<-0.0001
		covtable_1[1,5]= "<0.0001"
	}

	rz<-rownames(covtable)[1]
	rownames(covtable_1)<-c(rz)

	colnames(covtable_1)<-c("Estimate", "Std error", "DF", "t-value", "p-value")
	HTML(covtable_1, classfirstline="second", align="left")
}





#===================================================================================================================
#Diagnostic plots
#===================================================================================================================
if(showPRPlot=="Y")
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
if(showPRPlot=="Y")
{

	residualPlot <- sub(".html", "residualplot.jpg", htmlFile)
	jpeg(residualPlot,width = jpegwidth, height = jpegheight, quality = 100)

#STB July2013
	plotFilepdf3 <- sub(".html", "residualplot.pdf", htmlFile)
	dev.control("enable") 


#STB Feb 2011 Replaced Studentised with standardised
	residplot<-cbind(predict(threewayfull, level=1),residuals(threewayfull, level=1, type="pearson"))
	rownames(residplot)<-c(1:(dim(residplot)[1]))
	residplot<-data.frame(residplot)
	colnames(residplot)<-c("Predicted","Standardized_residuals")

#===================================================================================================================
#Graphical parameters
graphdata<-residplot

graphdata$yvarrr_IVS <- graphdata$Standardized_residuals
graphdata$xvarrr_IVS <- graphdata$Predicted
XAxisTitle <- "Predicted values"
YAxisTitle <- "Standardised residuals"
MainTitle2 <- "Residuals vs. predicted plot \n"
w_Gr_jit <- 0
h_Gr_jit <-  0
infiniteslope <- "Y"

if (bandw != "N") 
{
	Gr_line <-BW_line
	Gr_fill <- BW_fill
} else {
	Gr_line <-Col_line
	Gr_fill <- Col_fill
	}
Gr_line_type<-Line_type_dashed
Line_size <- 0.5

#GGPLOT2 code
	NONCAT_SCAT("RESIDPLOT")

	MainTitle2 <- ""
#===================================================================================================================
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
	HTML.title("<bf>Tip: Any observation with a standardised residual less than -3 or greater than 3 (SD) should be investigated as a possible outlier.", HR=0, align="left")
	HTML.title("<bf> ", HR=2, align="left")
	HTML.title("<bf>Comment: The standardised residuals are obtained by subtracting the fitted values from the responses and dividing by the estimated within-group standard error.", HR=0, align="left")
	#The residuals at level i are obtained by subtracting the fitted levels at that level from the response vector and dividing by the estimated within-group standard error.
}

#Normality plots
if(showNormPlot=="Y")
{
	HTMLbr()
	normPlot <- sub(".html", "normplot.jpg", htmlFile)
	jpeg(normPlot,width = jpegwidth, height = jpegheight, quality = 100)

#STB July2013
	plotFilepdf4 <- sub(".html", "normplot.pdf", htmlFile)
	dev.control("enable") 

#===================================================================================================================
#Graphical parameters

te<-qqnorm(resid(threewayfull, level=1))
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

if (bandw != "N") 
{
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

#GGPLOT2 code
NONCAT_SCAT("QQPLOT")

MainTitle2 <- ""
#===================================================================================================================
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


#===================================================================================================================
# LS Means
#===================================================================================================================

#Counting Treatment factors in selected Effect
selectedEffect2<-gsub("*", " * ",selectedEffect,fixed=TRUE) 
tempseChanges <-strsplit(selectedEffect2, " * ")

txtexpectedseChanges <- c("")
for(i in 1:length(tempseChanges[[1]])) 
{
	txtexpectedseChanges [length(txtexpectedseChanges )+1]=(tempseChanges[[1]][i]) 
}
nosefactors<-(length(txtexpectedseChanges)-2)/2


#Identify within animal degrees of freedom
	df<-anova(threewayfull)[dim(anova(threewayfull))[1],2]

#Calculate LS Means
	tabs<-lsmeans(threewayfull,eval(parse(text = paste("~",selectedEffect))), data=statdata)
	x<-summary(tabs)
	LSM<-data.frame(x)
	leng<-dim(LSM)[1]

	for (i in 1:leng)
	{
		LSM$DDF[i]<-df
	}

	LSM$Mean<-LSM$lsmean
	LSM$Lower=LSM$lsmean-qt(1-(1-sig)/2,df)*LSM$SE
	LSM$Upper=LSM$lsmean+qt(1-(1-sig)/2,df)*LSM$SE
	LSDATA<-data.frame(LSM)


#Creatign the final datasset to plot	
	LSDATA$Group_IVSq_<-LSM[,1]
	if (nosefactors > 1)
	{
		for (i in 2:nosefactors)
		{
			LSDATA$Group_IVSq_ <- paste(LSDATA$Group_IVSq_, " , " , LSDATA[,i] , sep="")
		}
	}

	if(showLSMeans=="Y")
	{
		Line_size <- Line_size2

		HTMLbr()
		CITitle<-paste("<bf>Plot of the least square (predicted) means with ",(sig*100),"% confidence intervals",sep="")
		HTML.title(CITitle, HR=2, align="left")

		meanPlot <- sub(".html", "meanplot.jpg", htmlFile)
		jpeg(meanPlot,width = jpegwidth, height = jpegheight, quality = 100)

#STB July2013
		plotFilepdf5 <- sub(".html", "meanplot.pdf", htmlFile)
		dev.control("enable") 
#===================================================================================================================
#Parameters
		graphdata<- LSDATA
		graphdata$jj_1<- graphdata$Timezzz
		Gr_alpha <- 0
		if (bandw != "N")
		{
			Gr_fill <- BW_fill
		} else {
			Gr_fill <- Col_fill
			}

		YAxisTitle <- LS_YAxisTitle
		XAxisTitle <- timeFactor_plot
		MainTitle2 <- ""

#GGPLOT2 code
		if (nosefactors == 1)
		{
			graphdata$jj_2 <- graphdata[,1]
			txtseChanges <- txtexpectedseChanges[2]
			for (i in 1:20)
			{
				txtseChanges<- namereplace(txtseChanges)
			}
			graphdata$catzz <- txtseChanges
			graphdata$jj_2 <- paste(graphdata$catzz, "=",graphdata[,1], sep = "") 

			Gr_palette<- palette_FUN("jj_2")
			LSMPLOT_2("none")
		}

		if (nosefactors == 2)
		{
			if (length(unique(levels(graphdata[,1]))) <= length(unique(levels(graphdata[,2]))))
			{
				graphdata$jj_2 <- graphdata[,1]
				txtseChanges <- txtexpectedseChanges[2]
				for (i in 1:20)
				{
					txtseChanges<- namereplace(txtseChanges)
				}
				graphdata$catzz <- txtseChanges
				graphdata$jj_2 <- paste(graphdata$catzz, "=",graphdata[,1], sep = "") 
	
				graphdata$jj_3 <- graphdata[,2]
				txtseChanges <- txtexpectedseChanges[4]
	
				for (i in 1:20)
				{
					txtseChanges<- namereplace(txtseChanges)
				}
				graphdata$catzz <- txtseChanges
				graphdata$jj_3 <- paste(graphdata$catzz, "=",graphdata[,2], sep = "") 
			} else {
					graphdata$jj_2 <- graphdata[,2]
					txtseChanges <- txtexpectedseChanges[4]
			
					for (i in 1:20)
					{
						txtseChanges<- namereplace(txtseChanges)
					}
					graphdata$catzz <- txtseChanges
					graphdata$jj_2 <- paste(graphdata$catzz, "=",graphdata[,2], sep = "") 
	
					graphdata$jj_3 <- graphdata[,1]
					txtseChanges <- txtexpectedseChanges[2]
	
					for (i in 1:20)
					{
						txtseChanges<- namereplace(txtseChanges)
					}
					graphdata$catzz <- txtseChanges
					graphdata$jj_3 <- paste(graphdata$catzz, "=",graphdata[,1], sep = "") 
				}
	
			Gr_palette<- palette_FUN("jj_2")
			LSMPLOT_2("three")
		}

		if (nosefactors == 3)
		{
			if (length(unique(levels(graphdata[,1]))) <= length(unique(levels(graphdata[,2])))  && length(unique(levels(graphdata[,1]))) <= length(unique(levels(graphdata[,3])))       )
			{
				graphdata$jj_2 <- graphdata[,1]
				txtseChanges <- txtexpectedseChanges[2]
				for (i in 1:20)
				{
					txtseChanges<- namereplace(txtseChanges)
				}
				graphdata$catzz <- txtseChanges
				graphdata$jj_2 <- paste(graphdata$catzz, "=",graphdata[,1], sep = "") 

				graphdata$jj_3 <- graphdata[,2]
				txtseChanges <- txtexpectedseChanges[4]
				for (i in 1:20)
				{
					txtseChanges<- namereplace(txtseChanges)
				}
				graphdata$catzz <- txtseChanges
				graphdata$jj_3 <- paste(graphdata$catzz, "=",graphdata[,2], sep = "") 

				graphdata$jj_4 <- graphdata[,3]
				txtseChanges <- txtexpectedseChanges[6]
				for (i in 1:20)
				{
					txtseChanges<- namereplace(txtseChanges)
				}
				graphdata$catzz <- txtseChanges
				graphdata$jj_4 <- paste(graphdata$catzz, "=",graphdata[,3], sep = "") 
			} else 	if (length(unique(levels(graphdata[,2]))) <= length(unique(levels(graphdata[,1])))  && length(unique(levels(graphdata[,2]))) <= length(unique(levels(graphdata[,3])))       )
				{
					graphdata$jj_2 <- graphdata[,2]
					txtseChanges <- txtexpectedseChanges[4]
					for (i in 1:20)
					{
						txtseChanges<- namereplace(txtseChanges)
					}
					graphdata$catzz <- txtseChanges
					graphdata$jj_2 <- paste(graphdata$catzz, "=",graphdata[,2], sep = "") 

					graphdata$jj_3 <- graphdata[,1]
					txtseChanges <- txtexpectedseChanges[2]
					for (i in 1:20)
					{
						txtseChanges<- namereplace(txtseChanges)
					}
					graphdata$catzz <- txtseChanges
					graphdata$jj_3 <- paste(graphdata$catzz, "=",graphdata[,1], sep = "") 
	
					graphdata$jj_4 <- graphdata[,3]
					txtseChanges <- txtexpectedseChanges[6]
					for (i in 1:20)
					{
						txtseChanges<- namereplace(txtseChanges)
					}
					graphdata$catzz <- txtseChanges
					graphdata$jj_4 <- paste(graphdata$catzz, "=",graphdata[,3], sep = "") 
				} else 	{
						graphdata$jj_2 <- graphdata[,3]
						txtseChanges <- txtexpectedseChanges[6]
						for (i in 1:20)
						{
							txtseChanges<- namereplace(txtseChanges)
						}
						graphdata$catzz <- txtseChanges
						graphdata$jj_2 <- paste(graphdata$catzz, "=",graphdata[,3], sep = "") 

						graphdata$jj_3 <- graphdata[,1]
						txtseChanges <- txtexpectedseChanges[2]
						for (i in 1:20)
						{
							txtseChanges<- namereplace(txtseChanges)
						}
						graphdata$catzz <- txtseChanges
						graphdata$jj_3 <- paste(graphdata$catzz, "=",graphdata[,1], sep = "") 
		
						graphdata$jj_4 <- graphdata[,2]
						txtseChanges <- txtexpectedseChanges[4]
						for (i in 1:20)
						{
							txtseChanges<- namereplace(txtseChanges)
						}
						graphdata$catzz <- txtseChanges
						graphdata$jj_4 <- paste(graphdata$catzz, "=",graphdata[,2], sep = "") 
					}
			Gr_palette<- palette_FUN("jj_2")
			LSMPLOT_2("four")
		}

		if (nosefactors > 3)
		{
			graphdata$jj_2 <- graphdata[,1]
	
			for (i in 4:nosefactors)
			{
				graphdata$jj_2 <- paste (graphdata$jj_2, ", ", graphdata[,i], sep="")
			}
	
			graphdata$jj_3 <- graphdata[,2]
			txtseChanges <- txtexpectedseChanges[4]
			for (i in 1:20)
			{
				txtseChanges<- namereplace(txtseChanges)
			}
			graphdata$catzz <- txtseChanges
			graphdata$jj_3 <- paste(graphdata$catzz, "=",graphdata[,2], sep = "") 
		
			graphdata$jj_4 <- graphdata[,3]
			txtseChanges <- txtexpectedseChanges[6]
			for (i in 1:20)
			{
				txtseChanges<- namereplace(txtseChanges)
			}
			graphdata$catzz <- txtseChanges
			graphdata$jj_4 <- paste(graphdata$catzz, "=",graphdata[,3], sep = "") 
		
			Gr_palette<- palette_FUN("jj_2")
			LSMPLOT_2("four")
		}

#===================================================================================================================
		void<-HTMLInsertGraph(GraphFileName=sub("[A-Z0-9a-z,:,\\\\]*App_Data[\\\\]","", meanPlot), Align="left")

#STB July2013
		if (pdfout=="Y")
		{
			pdf(file=sub("[A-Z0-9a-z,:,\\\\]*App_Data[\\\\]","", plotFilepdf5), height = pdfheight, width = pdfwidth) 
			dev.set(2) 
			dev.copy(which=3) 
			dev.off(2)
			dev.off(3)
			pdfFile_5<-sub("[A-Z0-9a-z,:,\\\\]*App_Data[\\\\]","",plotFilepdf5)
			linkToPdf5 <- paste ("<a href=\"",pdfFile_5,"\">Click here to view the PDF of the plot of the least square (predicted) means</a>", sep = "")
			HTML(linkToPdf5)
		}

		HTMLbr()
#Table of LS Means
		CITitle2<-paste("<bf>Table of the least square (predicted) means with ",(sig*100),"% confidence intervals",sep="")
		HTML.title(CITitle2, HR=2, align="left")

		LSDATA$Mean<-format(round(LSM$lsmean,3),nsmall=3)
		LSDATA$Lower<-format(round(LSM$Lower,3),nsmall=3)
		LSDATA$Upper<-format(round(LSM$Upper,3),nsmall=3)

		LSDATA2<-subset(LSDATA, select = -c(df, SE, asymp.LCL, asymp.UCL,DDF, lsmean, Group_IVSq_)) 

		blanq<-c(" ")
		for (i in 1:dim(LSDATA2)[2])
		{ 
			blanq[i]=" "
		}
		
		LSDATA3<-suppressWarnings(rbind(blanq,LSDATA2))

		names <- c()
		for (l in 1:nosefactors)
		{
			names[l] <- paste(unique (strsplit(selectedEffect, "*",fixed = TRUE)[[1]])[l], " ", sep = "")
		}

		names[nosefactors+1]<-timeFactor
		names[nosefactors+2]<-"Mean"
		names[nosefactors+3]<-paste("Lower ",(sig*100),"% CI",sep="")
		names[nosefactors+4]<-paste("Upper ",(sig*100),"% CI",sep="")

		colnames(LSDATA3)<-names
		rownames(LSDATA3)<-c("ID",1:(dim(LSDATA3)[1]-1))
	
		HTML(LSDATA3, classfirstline="second", align="left")
	}

#===================================================================================================================
#Back transformed geometric means plot and table 
#===================================================================================================================
	if(GeomDisplay == "Y" && showLSMeans =="Y" && (responseTransform =="Log10"||responseTransform =="Loge"))
	{
		HTMLbr()
		CITitle<-paste("<bf>Plot of the back-transformed geometric means with ",(sig*100),"% confidence intervals",sep="")
		HTML.title(CITitle, HR=2, align="left")

		add<-c("As the response was log transformed prior to analysis the least square (predicted) means are presented on the log scale. These results can be back transformed onto the original scale. These are known as the back-transformed geometric means.")
		HTML.title("</bf> ", HR=2, align="left")
		HTML.title(add, HR=0, align="left")

#===================================================================================================================
#LSMeans plot
#===================================================================================================================
		if (responseTransform =="Log10")
		{
			LSM$Mean<-10^(LSM$lsmean)
			LSM$Lower=10^(LSM$lsmean-qt(1-(1-sig)/2,df)*LSM$SE)
			LSM$Upper=10^(LSM$lsmean+qt(1-(1-sig)/2,df)*LSM$SE)
		}
		if (responseTransform =="Loge")
		{
			LSM$Mean<-exp(LSM$lsmean)
			LSM$Lower=exp(LSM$lsmean-qt(1-(1-sig)/2,df)*LSM$SE)
			LSM$Upper=exp(LSM$lsmean+qt(1-(1-sig)/2,df)*LSM$SE)
		}
		LSDATA<-data.frame(LSM)

#Creatign the final datasset to plot	
		LSDATA$Group_IVSq_<-LSM[,1]
		if (nosefactors > 1)
		{
			for (i in 2:nosefactors)
			{
				LSDATA$Group_IVSq_ <- paste(LSDATA$Group_IVSq_, " , " , LSDATA[,i] , sep="")
			}
		}

	if(showLSMeans=="Y")
	{
		Line_size <- Line_size2

		meanPlotd <- sub(".html", "meanplotd.jpg", htmlFile)
		jpeg(meanPlotd,width = jpegwidth, height = jpegheight, quality = 100)

#STB July2013
		plotFilepdf5d <- sub(".html", "meanplotd.pdf", htmlFile)
		dev.control("enable") 
#===================================================================================================================
#Parameters
		graphdata<- LSDATA
		graphdata$jj_1<- graphdata$Timezzz
		Gr_alpha <- 0
		if (bandw != "N")
		{
			Gr_fill <- BW_fill
		} else {
			Gr_fill <- Col_fill
			}

		YAxisTitle <- LS_YAxisTitle
		XAxisTitle <- timeFactor_plot
		MainTitle2 <- ""

#GGPLOT2 code
		if (nosefactors == 1)
		{
			graphdata$jj_2 <- graphdata[,1]
			txtseChanges <- txtexpectedseChanges[2]
			for (i in 1:20)
			{
				txtseChanges<- namereplace(txtseChanges)
			}
			graphdata$catzz <- txtseChanges
			graphdata$jj_2 <- paste(graphdata$catzz, "=",graphdata[,1], sep = "") 

			Gr_palette<- palette_FUN("jj_2")
			LSMPLOT_2("none")
		}

		if (nosefactors == 2)
		{
			if (length(unique(levels(graphdata[,1]))) <= length(unique(levels(graphdata[,2]))))
			{
				graphdata$jj_2 <- graphdata[,1]
				txtseChanges <- txtexpectedseChanges[2]
				for (i in 1:20)
				{
					txtseChanges<- namereplace(txtseChanges)
				}
				graphdata$catzz <- txtseChanges
				graphdata$jj_2 <- paste(graphdata$catzz, "=",graphdata[,1], sep = "") 

				graphdata$jj_3 <- graphdata[,2]
				txtseChanges <- txtexpectedseChanges[4]

				for (i in 1:20)
				{
					txtseChanges<- namereplace(txtseChanges)
				}
				graphdata$catzz <- txtseChanges
				graphdata$jj_3 <- paste(graphdata$catzz, "=",graphdata[,2], sep = "") 
			} else {
					graphdata$jj_2 <- graphdata[,2]
					txtseChanges <- txtexpectedseChanges[4]
				
					for (i in 1:20)
					{
						txtseChanges<- namereplace(txtseChanges)
					}
					graphdata$catzz <- txtseChanges
					graphdata$jj_2 <- paste(graphdata$catzz, "=",graphdata[,2], sep = "") 
		
					graphdata$jj_3 <- graphdata[,1]
					txtseChanges <- txtexpectedseChanges[2]
		
					for (i in 1:20)
					{
						txtseChanges<- namereplace(txtseChanges)
					}
					graphdata$catzz <- txtseChanges
					graphdata$jj_3 <- paste(graphdata$catzz, "=",graphdata[,1], sep = "") 
				}
	
			Gr_palette<- palette_FUN("jj_2")
			LSMPLOT_2("three")
		}

	
		if (nosefactors == 3)
		{
			if (length(unique(levels(graphdata[,1]))) <= length(unique(levels(graphdata[,2])))  && length(unique(levels(graphdata[,1]))) <= length(unique(levels(graphdata[,3])))       )
			{
				graphdata$jj_2 <- graphdata[,1]
				txtseChanges <- txtexpectedseChanges[2]
				for (i in 1:20)
				{
					txtseChanges<- namereplace(txtseChanges)
				}
				graphdata$catzz <- txtseChanges
				graphdata$jj_2 <- paste(graphdata$catzz, "=",graphdata[,1], sep = "") 
	
				graphdata$jj_3 <- graphdata[,2]
				txtseChanges <- txtexpectedseChanges[4]
				for (i in 1:20)
				{
					txtseChanges<- namereplace(txtseChanges)
				}
				graphdata$catzz <- txtseChanges
				graphdata$jj_3 <- paste(graphdata$catzz, "=",graphdata[,2], sep = "") 

				graphdata$jj_4 <- graphdata[,3]
				txtseChanges <- txtexpectedseChanges[6]
				for (i in 1:20)
				{
					txtseChanges<- namereplace(txtseChanges)
				}
				graphdata$catzz <- txtseChanges
				graphdata$jj_4 <- paste(graphdata$catzz, "=",graphdata[,3], sep = "") 
			} else 	if (length(unique(levels(graphdata[,2]))) <= length(unique(levels(graphdata[,1])))  && length(unique(levels(graphdata[,2]))) <= length(unique(levels(graphdata[,3])))       )
				{
					graphdata$jj_2 <- graphdata[,2]
					txtseChanges <- txtexpectedseChanges[4]
					for (i in 1:20)
					{
						txtseChanges<- namereplace(txtseChanges)
					}
					graphdata$catzz <- txtseChanges
					graphdata$jj_2 <- paste(graphdata$catzz, "=",graphdata[,2], sep = "") 
	
					graphdata$jj_3 <- graphdata[,1]
					txtseChanges <- txtexpectedseChanges[2]
					for (i in 1:20)
					{
						txtseChanges<- namereplace(txtseChanges)
					}
					graphdata$catzz <- txtseChanges
					graphdata$jj_3 <- paste(graphdata$catzz, "=",graphdata[,1], sep = "") 
	
					graphdata$jj_4 <- graphdata[,3]
					txtseChanges <- txtexpectedseChanges[6]
					for (i in 1:20)
					{
						txtseChanges<- namereplace(txtseChanges)
					}
					graphdata$catzz <- txtseChanges
					graphdata$jj_4 <- paste(graphdata$catzz, "=",graphdata[,3], sep = "") 
				} else 	{
						graphdata$jj_2 <- graphdata[,3]
						txtseChanges <- txtexpectedseChanges[6]
						for (i in 1:20)
						{
							txtseChanges<- namereplace(txtseChanges)
						}
						graphdata$catzz <- txtseChanges
						graphdata$jj_2 <- paste(graphdata$catzz, "=",graphdata[,3], sep = "") 
	
						graphdata$jj_3 <- graphdata[,1]
						txtseChanges <- txtexpectedseChanges[2]
						for (i in 1:20)
						{
							txtseChanges<- namereplace(txtseChanges)
						}
						graphdata$catzz <- txtseChanges
						graphdata$jj_3 <- paste(graphdata$catzz, "=",graphdata[,1], sep = "") 
			
						graphdata$jj_4 <- graphdata[,2]
						txtseChanges <- txtexpectedseChanges[4]
						for (i in 1:20)
						{
							txtseChanges<- namereplace(txtseChanges)
						}
						graphdata$catzz <- txtseChanges
						graphdata$jj_4 <- paste(graphdata$catzz, "=",graphdata[,2], sep = "") 
					}
			Gr_palette<- palette_FUN("jj_2")
			LSMPLOT_2("four")
		}
	
		if (nosefactors > 3)
		{
			graphdata$jj_2 <- graphdata[,1]
	
			for (i in 4:nosefactors)
			{
				graphdata$jj_2 <- paste (graphdata$jj_2, ", ", graphdata[,i], sep="")
			}
	
			graphdata$jj_3 <- graphdata[,2]
			txtseChanges <- txtexpectedseChanges[4]
			for (i in 1:20)
			{
				txtseChanges<- namereplace(txtseChanges)
			}
			graphdata$catzz <- txtseChanges
			graphdata$jj_3 <- paste(graphdata$catzz, "=",graphdata[,2], sep = "") 
		
			graphdata$jj_4 <- graphdata[,3]
			txtseChanges <- txtexpectedseChanges[6]
			for (i in 1:20)
			{
				txtseChanges<- namereplace(txtseChanges)
			}
			graphdata$catzz <- txtseChanges
			graphdata$jj_4 <- paste(graphdata$catzz, "=",graphdata[,3], sep = "") 
		
			Gr_palette<- palette_FUN("jj_2")
			LSMPLOT_2("four")
		}

#===================================================================================================================
		void<-HTMLInsertGraph(GraphFileName=sub("[A-Z0-9a-z,:,\\\\]*App_Data[\\\\]","", meanPlotd), Align="left")

#STB July2013
		if (pdfout=="Y")
		{
			pdf(file=sub("[A-Z0-9a-z,:,\\\\]*App_Data[\\\\]","", plotFilepdf5d), height = pdfheight, width = pdfwidth) 
			dev.set(2) 
			dev.copy(which=3) 
			dev.off(2)
			dev.off(3)
			pdfFile_5d<-sub("[A-Z0-9a-z,:,\\\\]*App_Data[\\\\]","",plotFilepdf5d)
			linkToPdf5d <- paste ("<a href=\"",pdfFile_5d,"\">Click here to view the PDF of the plot of the back-transformed geometric means</a>", sep = "")
			HTML(linkToPdf5d)
		}

		HTMLbr()
#Table of LS Means
		CITitle2<-paste("<bf>Table of the back-transformed geometric means with ",(sig*100),"% confidence intervals",sep="")
		HTML.title(CITitle2, HR=2, align="left")
	
		if (responseTransform =="Log10")
		{
			LSDATA$Mean<-format(round(LSM$Mean,3),nsmall=3)
			LSDATA$Lower<-format(round(LSM$Lower,3),nsmall=3)
			LSDATA$Upper<-format(round(LSM$Upper,3),nsmall=3)
		}
		if (responseTransform =="Loge")
		{
			LSDATA$Mean<-format(round(LSM$Mean,3),nsmall=3)
			LSDATA$Lower<-format(round(LSM$Lower,3),nsmall=3)
			LSDATA$Upper<-format(round(LSM$Upper,3),nsmall=3)
		}

		LSDATA2<-subset(LSDATA, select = -c(df, SE, asymp.LCL, asymp.UCL,DDF, lsmean, Group_IVSq_)) 

		blanq<-c(" ")
		for (i in 1:dim(LSDATA2)[2])
		{ 
			blanq[i]=" "
		}

		LSDATA3<-suppressWarnings(rbind(blanq,LSDATA2))

		names <- c()
		for (l in 1:nosefactors)
		{
			names[l] <- paste(unique (strsplit(selectedEffect, "*",fixed = TRUE)[[1]])[l], " ", sep = "")
		}
	
		names[nosefactors+1]<-timeFactor
		names[nosefactors+2]<-"Mean"
		names[nosefactors+3]<-paste("Lower ",(sig*100),"% CI",sep="")
		names[nosefactors+4]<-paste("Upper ",(sig*100),"% CI",sep="")
	
		colnames(LSDATA3)<-names
		rownames(LSDATA3)<-c("ID",1:(dim(LSDATA3)[1]-1))
	
		HTML(LSDATA3, classfirstline="second", align="left")
	}
}

#===================================================================================================================
#All pairwise tests
#===================================================================================================================
#Denominator degrees of freedom
dendf<-ivsanova[dim(ivsanova)[1],3]




#STB Jun 2015
#Creating dataset without dashes in

ivs_num_ivs <- rep(1:dim(statdata)[1])
ivs_char_ivs <- rep(factor(LETTERS[1:dim(statdata)[1]]), 1)
statdata_temp2<- data.frame(cbind(statdata_temp, ivs_num_ivs,ivs_char_ivs ))

statdata_num<- statdata_temp2[,sapply(statdata_temp2,is.numeric)]
statdata_char<- statdata_temp2[,!sapply(statdata_temp2,is.numeric)]

statdata_char2 <- as.data.frame(sapply(statdata_char,gsub,pattern="-",replacement="_ivs_dash_ivs_"))
statdata<- data.frame(cbind(statdata_num, statdata_char2))

statdata$Timezzz<-as.factor(eval(parse(text = paste("statdata$", timeFactor))))
statdata$subjectzzzzzz<-as.factor(eval(parse(text = paste("statdata$", subjectFactor))))

statdata<-statdata[order(statdata$subjectzzzzzz, statdata$Timezzz), ]

#Re-generate analysis using new dataset without dashes
if(covariance=="Compound Symmetric")
{
	threewayfull<-lme(model, random=~1|subjectzzzzzz, data=statdata,correlation=corCompSymm(),  na.action = (na.omit), method = "REML")
}
if(covariance=="Autoregressive(1)")
{
	threewayfull<-lme(model, random=~1|subjectzzzzzz, correlation=corAR1(value=0.999, form=~as.numeric(Timezzz)|subjectzzzzzz, fixed =FALSE), data=statdata, na.action = (na.omit), method = "REML")
}
if(covariance=="Unstructured")
{
	threewayfull<-lme(model, random=~1|subjectzzzzzz, correlation= corSymm(form = ~ as.numeric(Timezzz) | subjectzzzzzz), weights=varIdent(form=~ 1 |as.numeric(Timezzz)), data=statdata, na.action = (na.omit), method = "REML")
}


if (covariance == "Unstructured")
{
#Generating the differences and SEMs for the unstructured covariance
	mult.lsm <- lsmeans(threewayfull, eval(parse(text = paste("~",selectedEffect))), data=statdata, df=dendf)
	multc<-contrast(mult.lsm, method="pairwise" , adjust = "none")
	mult<-data.frame(summary(multc))
	mult$ratio <- abs(mult$estimate / mult$SE)
	mult$pvals <- 2*pt(mult$ratio, dendf, lower=FALSE)
	mult$tval<- abs(qt((1-sig)/2, dendf))
	mult$lower <- mult$estimate - mult$tval * mult$SE
	mult$upper <- mult$estimate + mult$tval * mult$SE

#Creating the rownames for the splitting below
	rows1 <-data.frame(mult$contrast)
	rows2 <-mult$contrast
	rownames(rows1)<-rows2
	rows<-rownames(rows1)


#Creating the final table tabs
	tablen<-dim(mult)[1]
	tabs<-data.frame(matrix(NA, nrow = tablen, ncol = 1))

	for (i in 1:tablen)
	{	
		tabs$V1[i]=mult$estimate[i]
	}		

	for (i in 1:tablen)
	{
		tabs$V2[i]=mult$lower[i]
	}

	for (i in 1:tablen)
	{
		tabs$V3[i]=mult$upper[i]
	}
	for (i in 1:tablen)
	{
		tabs$V4[i]=format(round(mult$SE[i], 3), nsmall=3, scientific=FALSE)
	}

	for (i in 1:tablen)
	{
		tabs$V5[i]=format(round(mult$pvals[i], 4), nsmall=4, scientific=FALSE)
	}
	for (i in 1:tablen)
	{
		tabs$V6[i]=mult$pvals[i]
	}

	for (i in 1:tablen) 
	{
		if (mult$pvals[i]<0.0001) 
		{
# STB - March 2011 formatting p<0.0001
			tabs$V5[i]<-0.0001
			tabs$V5[i]=format(round(0.0001, 4), nsmall=4, scientific=FALSE)
			tabs$V5[i]<- paste("<",tabs$V5)
		}
	}
#removing the first column of the tabs dataset
	tabs<-tabs[,-1]

} else {

#Creating the table of differences and SEMs for the AR(1) and CS structure and the tabs dataset
	mult<-glht(threewayfull, linfct=lsm(eval(parse(text = paste("pairwise ~",selectedEffect)))),df=dendf)

	multci<-confint(mult, level=sig, calpha = univariate_calpha())
	multp<-summary(mult, test=adjusted("none"))
	rows<-rownames(multci$confint)

	pvals<-multp$test$pvalues
	sigma<-multp$test$sigma
	tablen<-length(unique(rownames(multci$confint)))
	tabs<-data.frame(nrow=tablen, ncol=6)

	for (i in 1:tablen)
	{
#STB Dec 2011 formatting 3dp
		tabs[i,1]=multci$confint[i]
	}
	for (i in 1:tablen)
	{
		tabs[i,2]=multci$confint[i+tablen]
	}
	for (i in 1:tablen)
	{
		tabs[i,3]=multci$confint[i+2*tablen]
	}
	for (i in 1:tablen)
	{
		tabs[i,4]=format(round(sigma[i], 3), nsmall=3, scientific=FALSE)
	}
	for (i in 1:tablen)
	{
		tabs[i,5]=format(round(pvals[i], 4), nsmall=4, scientific=FALSE)
	}
	for (i in 1:tablen)
	{
		tabs[i,6]=pvals[i]
	}
	for (i in 1:tablen) 
	{
		if (pvals[i]<0.0001) 
		{
# STB - March 2011 formatting p<0.0001
#			tabs[i,5]<-0.0001
			tabs[i,5]=format(round(0.0001, 4), nsmall=4, scientific=FALSE)
			tabs[i,5]<- paste("<",tabs[i,5])
		}
	}
}

#Creating the list of comparisons

tell1<-t(data.frame(strsplit(rows, " - ")))[,1]
tell2<-t(data.frame(strsplit(rows, " - ")))[,2]
tell1a<-t(data.frame(strsplit(tell1, ",")))
tell2a<-t(data.frame(strsplit(tell2, ",")))
tell1b<-tell1a[,dim(tell1a)[2]]
tell2b<-tell2a[,dim(tell2a)[2]]
tellfinal<-cbind(tell1b,tell2b)


if(allPairwiseTest == "All")
{
#Creatng dataset for printing
tabs_final<-tabs
#Title
	add<-paste(c("All pairwise comparisons, without adjustment for multiplicity"))
	HTMLbr()
	HTML.title(add, HR=2, align="left")

	tabs_final[1]=format(round(tabs_final[1], 3), nsmall=3, scientific=FALSE)
	tabs_final[2]=format(round(tabs_final[2], 3), nsmall=3, scientific=FALSE)
	tabs_final[3]=format(round(tabs_final[3], 3), nsmall=3, scientific=FALSE)

#creating the final dataset for printing
	header<-c(" ", " "," ", " "," ", " ")
	tabs_final<-data.frame(rbind(header, tabs_final))

	for (i in 1:100)
	{
		rows<-sub("_.._"," ", rows, fixed=TRUE)
	}
	rows<-sub(" - "," vs. ", rows, fixed=TRUE)

#STB June 2015	
	for (i in 1:100)
	{
		rows<-sub("_ivs_dash_ivs_"," - ", rows, fixed=TRUE)
	}

	rownames(tabs_final)<-c("Comparison", rows)
	lowerCI<-paste("   Lower ",(sig*100),"% CI   ",sep="")
	upperCI<-paste("   Upper ",(sig*100),"% CI   ",sep="")
	colnames(tabs_final)<-c("   Difference   ", lowerCI, upperCI, "   Std error   ", "   p-value   ", "temp")
	tabs_final2<-subset(tabs_final, select = -c(temp)) 
	
#print table
	HTML(tabs_final2, classfirstline="second", align="left")
	HTML.title("<bf> ", HR=2, align="left")
	HTML.title("<bf>Warning: As these tests are not adjusted for multiplicity there is a risk of false positive results. Only use the pairwise tests you planned to make a-priori, these are the so called planned comparisons, see Snedecor and Cochran (1989). No options are available in this module to make multiple comparison adjustments. If you wish to apply a multiple comparison adjustment to these results then use the p-value adjustment module.", HR=0, align="left")

#===================================================================================================================
#Back transformed geometric means table 
#===================================================================================================================
	if(GeomDisplay == "Y" && (responseTransform =="Log10"||responseTransform =="Loge"))
	{

		#Creating data for printing
		tabs_final_log<-tabs
	
		add<-c("All pairwise comparisons, as back-transformed ratios")
		HTMLbr()
		HTML.title(add, HR=2, align="left")

		add<-c("As the response was log transformed prior to analysis the differences between the least square (predicted) means are presented on the log scale. These results can be back-transformed onto the original scale, where differences on the log scale become ratios when back-transformed.")
		HTML.title("</bf> ", HR=2, align="left")
		HTML.title(add, HR=0, align="left")

		if (responseTransform =="Log10")
		{
			tabs_final_log[1]<-10^tabs_final_log[1]
			tabs_final_log[2]<-10^tabs_final_log[2]
			tabs_final_log[3]<-10^tabs_final_log[3]

			tabs_final_log[1]=format(round(tabs_final_log[1], 3), nsmall=3, scientific=FALSE)
			tabs_final_log[2]=format(round(tabs_final_log[2], 3), nsmall=3, scientific=FALSE)
			tabs_final_log[3]=format(round(tabs_final_log[3], 3), nsmall=3, scientific=FALSE)
		}
		if (responseTransform =="Loge")
		{
			tabs_final_log[1]=format(round(exp(tabs_final_log[1]), 3), nsmall=3, scientific=FALSE)
			tabs_final_log[2]=format(round(exp(tabs_final_log[2]), 3), nsmall=3, scientific=FALSE)
			tabs_final_log[3]=format(round(exp(tabs_final_log[3]), 3), nsmall=3, scientific=FALSE)
		}

#creating the final dataset for printing
		header<-c(" "," "," ", " "," "," ")
		tabs_final_log <- data.frame(tabs_final_log)
		tabs_final_log <- data.frame(rbind(header, tabs_final_log))

		for (i in 1:100)
		{
			rows<-sub("_.._"," ", rows, fixed=TRUE)
		}
		rows<-sub(" vs. "," / ", rows, fixed=TRUE)


#STB June 2015	
	for (i in 1:100)
	{
		rows<-sub("_ivs_dash_ivs_"," - ", rows, fixed=TRUE)
	}

		rownames(tabs_final_log)<-c("Comparison", rows)

		lowerCI<-paste("   Lower ",(sig*100),"% CI   ",sep="")
		upperCI<-paste("   Upper ",(sig*100),"% CI   ",sep="")
		colnames(tabs_final_log)<-c("   Ratio   ", lowerCI, upperCI, "Stderror", "pvalue", "temp")
		tabs_final_log<-subset(tabs_final_log, select = -c(Stderror, pvalue , temp)) 
	
#print table
		HTML(tabs_final_log, classfirstline="second", align="left")
	}
}

#===================================================================================================================

if(allPairwiseTest == "Reduced")
{
#Title
	add<-paste(c("Pairwise comparisons within the levels of the repeated factor, without adjustment for multiplicity"))
	HTMLbr()
	HTML.title(add, HR=2, align="left")

#Creating the subsetted version of tabs dataset
	tabs <- data.frame(cbind(tabs, tellfinal))
	rownames(tabs)<-c(rows)
	tabs<-subset(tabs, tell1b==tell2b)
	tabs<-subset(tabs, select = -c(tell1b,tell2b)) 

#Creating the dataset for printing
	tabs_final<-tabs

	tabs_final[1]=format(round(tabs_final[1], 3), nsmall=3, scientific=FALSE)
	tabs_final[2]=format(round(tabs_final[2], 3), nsmall=3, scientific=FALSE)
	tabs_final[3]=format(round(tabs_final[3], 3), nsmall=3, scientific=FALSE)

#creating the final dataset for printing
	temp<-rownames(tabs_final)
	header<-c(" ", " "," ", " "," ", " ")
	tabs_final<-data.frame(rbind(header, tabs_final))

#STB June 2015
	temp<-sub(" - "," vs. ", temp, fixed=TRUE)

#STB June 2015	
	for (i in 1:100)
	{
		temp<-sub("_ivs_dash_ivs_"," - ", temp, fixed=TRUE)
	}

	rownames(tabs_final)<-c("Comparison", temp)

	lowerCI<-paste("   Lower ",(sig*100),"% CI   ",sep="")
	upperCI<-paste("   Upper ",(sig*100),"% CI   ",sep="")
	colnames(tabs_final)<-c("   Difference   ", lowerCI, upperCI, "   Std error   ", "   p-value   ", "temp")

	tabs_final2<-subset(tabs_final, select = -c(temp)) 

	HTML(tabs_final2, classfirstline="second", align="left")

	HTML.title("<bf> ", HR=2, align="left")
	HTML.title("<bf>Warning: As these tests are not adjusted for multiplicity there is a risk of false positive results. Only use the pairwise tests you planned to make a-priori, these are the so called planned comparisons, see Snedecor and Cochran (1989). No options are available in this module to make multiple comparison adjustments. If you wish to apply a multiple comparison adjustment to these results then use the p-value adjustment module.", HR=0, align="left")




#===================================================================================================================
#Back transformed geometric means table 
#===================================================================================================================
	if(GeomDisplay == "Y" && (responseTransform =="Log10"||responseTransform =="Loge"))
	{
		#Creating data for printing
		tabs_final_log<-tabs

		add<-c("Pairwise comparisons within the levels of the repeated factor, as back-transformed ratios")
		HTMLbr()
		HTML.title(add, HR=2, align="left")

		add<-c("As the response was log transformed prior to analysis the differences between the least square (predicted) means are presented on the log scale. These results can be back-transformed onto the original scale, where differences on the log scale become ratios when back-transformed.")
		HTML.title("</bf> ", HR=2, align="left")
		HTML.title(add, HR=0, align="left")

		if (responseTransform =="Log10")
		{
			tabs_final_log[1]=format(round(10^(tabs_final_log[1]), 3), nsmall=3, scientific=FALSE)
			tabs_final_log[2]=format(round(10^(tabs_final_log[2]), 3), nsmall=3, scientific=FALSE)
			tabs_final_log[3]=format(round(10^(tabs_final_log[3]), 3), nsmall=3, scientific=FALSE)
		}
		if (responseTransform =="Loge")
		{
			tabs_final_log[1]=format(round(exp(tabs_final_log[1]), 3), nsmall=3, scientific=FALSE)
			tabs_final_log[2]=format(round(exp(tabs_final_log[2]), 3), nsmall=3, scientific=FALSE)
			tabs_final_log[3]=format(round(exp(tabs_final_log[3]), 3), nsmall=3, scientific=FALSE)
		}

#creating the final dataset for printing
		temp<-rownames(tabs_final_log)

		header<-c(" ", " "," ", " "," ", " ")
		tabs_final_log<-data.frame(tabs_final_log)

		temp<-sub(" - "," / ", temp, fixed=TRUE)

#STB June 2015	
	for (i in 1:100)
	{
		temp<-sub("_ivs_dash_ivs_"," - ", temp, fixed=TRUE)
	}

		rownames(tabs_final_log)<-temp
		lowerCI<-paste("   Lower ",(sig*100),"% CI   ",sep="")
		upperCI<-paste("   Upper ",(sig*100),"% CI   ",sep="")
		colnames(tabs_final_log)<-c("   Ratio   ", lowerCI, upperCI, "Stderror", "pvalue", "temp")
		tabs_final_log<-subset(tabs_final_log, select = -c(Stderror, pvalue, temp)) 
	
		HTML(tabs_final_log, classfirstline="second", align="left")
	}
}


#===================================================================================================================
#STB March 2014 - Creating a dataset of p-values
comparisons <-paste(direct, "/Comparisons.csv", sep = "")
tabsx<- data.frame(tabs_final[,6])
row <-rownames(tabs_final)

for (i in 1:100)
{
	row<-sub(","," and ", row, fixed=TRUE)
}
tabsx<-cbind(row, tabsx)

colnames(tabsx)<-c("Comparison", "p-value")
row.names(tabsx) <- seq(nrow(tabsx)) 
tabsx <-tabsx[-1,]


#===================================================================================================================
#Conclusion
add<-paste(c("Conclusion"))
inte<-1
tempnames<-rownames(tabs_final)

for(i in 1:(dim(tabs)[1]))
{
	if (tabs$V6[i] <= (1-sig))
	{
		if (inte==1)
		{
			inte<-inte+1
			add<-paste(add, ": The following pairwise tests are statistically significantly different at the  ", sep="")
			add<-paste(add, 100*(1-sig), sep="")
			add<-paste(add, "% level: ", sep="")
			add<-paste(add, tempnames[i+1], sep="")
		} else {
			inte<-inte+1
			add<-paste(add, ", ", sep="")
			add<-paste(add, tempnames[i+1], sep="")
			}
	} 
}
if (inte==1)
{
	if (tablen >1)
	{
		add<-paste(add, ": There are no statistically significant pairwise differences.", sep="")
	} else {
			add<-paste(add, ": The pairwise difference is not statistically significant.", sep="")
		}
} else {
		add<-paste(add, ". ", sep="")
	}
HTML.title("</bf> ", HR=2, align="left")
HTML.title(add, HR=0, align="left")


#===================================================================================================================
#Analysis description
#===================================================================================================================


HTMLbr()
HTML.title("<bf>Analysis description", HR=2, align="left")

add<-c("The data were analysed using a ")
if (notreatfactors==1) 
{
	add<-paste(add, "2-way repeated measures mixed model approach, with treatment factor ", sep="")
	add<-paste(add, treatFactors, sep="")
	add<-paste(add, ", repeated factor ", sep="")
	add<-paste(add, timeFactor, sep="")
} else {
factorz<-notreatfactors+1
	add<-paste(add, factorz, sep="")
	add<-paste(add, "-way repeated measures mixed model approach, with ", sep="")

	for (i in 1:notreatfactors)
	{
		if (i<notreatfactors-1)
		{
			add<-paste(add, txtexpectedChanges[i+1], sep="")
			add<-paste(add, ", ", sep="")
		} else 	if (i<notreatfactors)
		{
			add<-paste(add, txtexpectedChanges[i+1], sep="")
			add<-paste(add, " and ", sep="")
		} else if (i==notreatfactors)
		{
	    		add<-paste(add, txtexpectedChanges[i+1], sep="")
			add<-paste(add, " as treatment factors", sep="")

			if (blockFactors != "NULL" | FirstCatFactor != "NULL") 
			{
			add<-paste(add, ", ", sep="")
			} else {
				add<-paste(add, " and ", sep="")
				}

			add<-paste(add, timeFactor, sep="")
			add<-paste(add, " as the repeated factor", sep="")

		}
	}
}

if (blockFactors != "NULL" && FirstCatFactor != "NULL") 
{
	add<-paste(add, ", ", sep="")
} else if (noblockfactors==1 && blockFactors != "NULL" && FirstCatFactor == "NULL") 
{
	add<-paste(add, " and ", sep="")
} else if (noblockfactors>1 && blockFactors != "NULL" && FirstCatFactor == "NULL") 
{
	add<-paste(add, ", ", sep="")
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
    			add<-paste(add, txtexpectedblockChanges[i+1], sep="")
				add<-paste(add, ", ", sep="")
			} else	if (i<noblockfactors)
	 		{
    			add<-paste(add, txtexpectedblockChanges[i+1], sep="")
				add<-paste(add, " and ", sep="")
			} else if (i==noblockfactors)
	 		{
    			add<-paste(add, txtexpectedblockChanges[i+1], sep="")
			}
		}
		add<-paste(add, " as blocking factors", sep="")
	}
}

if (FirstCatFactor == "NULL") 
{
	add<-paste(add, ". ", sep="")
} else if(FirstCatFactor != "NULL")	{
	add<-paste(add, " and  ", sep="")
	add<-paste(add, unlist(strsplit(covariateModel, "~"))[2], sep="")
	add<-paste(add, " as the covariate. ", sep="")
}

if (allPairwiseTest== "Y")
{
#STB May 2012 Updating "Selected"
	add<-paste(add, "This was followed by planned comparisons on the predicted means to compare the levels of the Selected effect. ", sep="")
}

if (responseTransform != "None")
{
	add<-paste(add, " The response was ", sep="")
	add<-paste(add, responseTransform, sep="")
	add<-paste(add, " transformed prior to analysis to stabilise the variance.", sep="") 
}

if (covariateTransform != "None" && responseTransform != "None")
{
	add<-paste(add, " The covariate was also ", sep="")
	add<-paste(add, covariateTransform, sep="")
	add<-paste(add, " transformed. ", sep="")
}

if (responseTransform == "None" && covariateTransform != "None")
{
	add<-paste(add, " The covariate was ", sep="")
	add<-paste(add, covariateTransform , sep="")
	add<-paste(add, " transformed prior to analysis.", sep="") 
}



HTML.title("</bf> ", HR=2, align="left")
HTML.title(add, HR=0, align="left")
if(covariance=="Compound Symmetric")
{
	add2<-c("The compound symmetric covariance structure was used to model the within-subject correlations. When using this structure we assumed that the variability of the responses was the same at each level of ")
	add2<-paste(add2, timeFactor, sep="")
  	add2<-paste(add2, " and the correlation between responses from any pair of levels of ", sep="")
	add2<-paste(add2, timeFactor, sep="")
	add2<-paste(add2, "  is the same.")
	HTML.title("</bf> ", HR=2, align="left")
	HTML.title(add2, HR=0, align="left")
}
if(covariance=="Autoregressive(1)")
{
	add2<-c("The first order autoregressive covariance structure was used to model the within-subject correlations. When using this structure we assumed that the variability of the responses was the same at each level of ")
	add2<-paste(add2, timeFactor, sep="")
  	add2<-paste(add2, ". We also assumed that the correlation between responses from any pair of levels of ", sep="")
	add2<-paste(add2, timeFactor, sep="")
	add2<-paste(add2, " was related to the distance between them.")
	HTML.title("</bf> ", HR=2, align="left")
	HTML.title(add2, HR=0, align="left")
}
if(covariance=="Unstructured")
{
	add2<-c("The unstructured covariance structure allowed the variability of the responses to be different, depending on the level of ")
	add2<-paste(add2, timeFactor, sep="")
  	add2<-paste(add2, ". This structure also allowed the correlation between responses from any pair of levels of ", sep="")
	add2<-paste(add2, timeFactor, sep="")
	add2<-paste(add2, " to be different. While this approach is the most general it should be used with care when there are few subjects, as many parameters are required to be estimated. 
These estimates may not be very reliable.")
	HTML.title("</bf> ", HR=2, align="left")
	HTML.title(add2, HR=0, align="left")
}


add<-c("A full description of mixed model theory, including information on the R nlme package used by ", branding , ", can be found in Venables and Ripley (2003) and Pinherio and Bates (2002).")
HTML.title("</bf> ", HR=2, align="left")
HTML.title(add, HR=0, align="left")


#===================================================================================================================
#Create file of comparisons
#===================================================================================================================
write.csv(tabsx, file = sub("[A-Z0-9a-z,:,\\\\]*App_Data[\\\\]","", comparisons), row.names=FALSE)


#===================================================================================================================
#References
#===================================================================================================================
Ref_list<-R_refs()

#Bate and Clark comment
HTML.title(refxx, HR=0, align="left")	

HTML.title("Statistical references", HR=2, align="left")
HTML(Ref_list$BateClark_ref, align="left")

if(FirstCatFactor != "NULL") {
	HTML("Morris TR. (1999). Experimental Design and Analysis in Animal Sciences. CABI publishing. Wallingford, Oxon (UK).", align="left")
}

HTML("Pinherio JC and Bates DM. (2000). Mixed Effects Models in S and S-Plus. Springer-Verlag. New York, Inc.", align="left")

if (allPairwiseTest != "Y") {
	HTML("Snedecor GW and Cochran WG. (1989). Statistical Methods. 8th edition;  Iowa State University Press, Iowa, USA.", align="left")
}
HTML("Venables WN and Ripley BD. (2003). Modern Applied Statistics with S. 4th Edition; Springer. New York, Inc.", align="left")

HTML.title("R references", HR=2, align="left")
HTML(Ref_list$R_ref ,  align="left")
HTML(Ref_list$GGally_ref,  align="left")
HTML(Ref_list$RColorBrewers_ref,  align="left")
HTML(Ref_list$GGPLot2_ref,  align="left")
HTML(Ref_list$reshape_ref,  align="left")
HTML(Ref_list$plyr_ref,  align="left")
HTML(Ref_list$scales_ref,  align="left")
HTML(Ref_list$nlme_ref,  align="left")
HTML(Ref_list$R2HTML_ref,  align="left")
HTML(Ref_list$PROTO_ref,  align="left")
HTML(Ref_list$Contrast_ref,  align="left")
HTML(Ref_list$LSMEANS_ref, align="left")
HTML(Ref_list$multcomp_ref,  align="left")

#===================================================================================================================
#Show dataset
#===================================================================================================================
if (showdataset=="Y") {
	statdata_temp2<-subset(statdata_temp, select = -c(between, betweenwithin, mainEffect, subjectzzzzzz, Timezzz))
	observ <- data.frame(c(1:dim(statdata_temp2)[1]))
	colnames(observ) <- c("Observation")
	statdata_temp22 <- cbind(observ, statdata_temp2)

	HTML.title("Analysis dataset", HR = 2, align = "left")
    	HTML(statdata_temp22, classfirstline = "second", align = "left", row.names = "FALSE")


}

#===================================================================================================================
#Show arguments
#===================================================================================================================
HTML.title("Analysis options", HR=2, align="left")

HTML(paste("Response variable: ", resp, sep=""),  align="left")

if (responseTransform != "None") {
	HTML(paste("Response transformation: ", responseTransform, sep=""),  align="left")
}

if(FirstCatFactor != "NULL") {
	HTML(paste("Covariate: ", unlist(strsplit(covariateModel, "~"))[2], sep=""),  align="left")
}

if (FirstCatFactor != "NULL" && covariateTransform != "None") {
	HTML(paste("Covariate transformation: ", covariateTransform, sep=""),  align="left")
}

if (FirstCatFactor != "NULL" ) {
	HTML(paste("Categorisation factor used on covariate scatterplots: ", FirstCatFactor, sep=""),  align="left")
}

HTML(paste("Treatment factors: ", treatFactors, sep=""),  align="left")
HTML(paste("Repeated factor: ", timeFactor, sep=""),  align="left")
HTML(paste("Subject factor: ", subjectFactor, sep=""),  align="left")

if (blockFactors != "NULL") {
	HTML(paste("Blocking factor(s) selected: ", blockFactors, sep=""),  align="left")
}

HTML(paste("Covariance structure fitted: ", covariance, sep=""),  align="left")
HTML(paste("Output tests of overall effects table (Y/N): ", showANOVA, sep=""),  align="left")
HTML(paste("Output predicted vs. residual plot (Y/N): ", showPRPlot, sep=""),  align="left")
HTML(paste("Output normal probability plot (Y/N): ", showNormPlot, sep=""),  align="left")
HTML(paste("Show Least Square predicted means (Y/N): ", showLSMeans, sep=""),  align="left")

if (showLSMeans != "N" && Args[19] != "NULL" ) {
	selectedEffectXX<-gsub("Timezzz",eval(timeFactor),selectedEffect) 
	HTML(paste("Selected effect (for pairwise mean comparisons): ", selectedEffectXX, sep=""),  align="left")
}

if (showLSMeans != "N" && Args[19] != "NULL" && allPairwiseTest == "Reduced") {
	HTML(paste("Post-hoc tests:  All comparisons within repeated factor levels"),  align="left")
} else {
	HTML(paste("Post-hoc tests:  All pairwise comparisons"),  align="left")
}

HTML(paste("Significance level: ", 1-sig, sep=""),  align="left")
