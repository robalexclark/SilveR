#===================================================================================================================
#R Libraries

suppressWarnings(library(mvtnorm))
suppressWarnings(library(R2HTML))
suppressWarnings(library(cluster))
suppressWarnings(library(ggdendro))
suppressWarnings(library(mixOmics))
suppressWarnings(library(pls))
#===================================================================================================================

#===================================================================================================================
# retrieve args
Args <- commandArgs(TRUE)

#Read in data
statdata <- read.csv(Args[3], header=TRUE, sep=",")

#Copy Args
responses_IVS_ <- Args[4]
transformation <- Args[5]
catPred_ <- Args[6] 
contPred_ <-Args[7]
caseid_IVS_ <- Args[8]
analysisType <- Args[9]

noOfClusters <- Args[10]
distanceMethod <- Args[11]
agglomerationMethod <- Args[12]
plotLabels <- Args[13]

noOfComponents <- Args[14]

PCA_center <- Args[15]
PCA_scale <- Args[16]

#=================================================================================================================
#Graphical parameters
source(paste(getwd(),"/InVivoStat_functions.R", sep=""))
Labelz_IVS_ <- "Y"

#=================================================================================================================
#Setup the html file and associated css file
htmlFile <- sub(".csv", ".html", Args[3]); #determine the file name of the html file
HTMLSetFile(file=htmlFile) 
cssFile <- "r2html.css"
cssFile <- paste("'",cssFile,"'", sep="") #need to enclose in quotes when path has spaces in it

#=================================================================================================================
#Variable set-up

#Setting up the Case ID variable
if (caseid_IVS_ == "NULL")
{
	statdata$caseid_IVS_ <- as.factor(c(1:dim(statdata)[1]))
	caseid_IVS_name<- "Row ID"

} else {
	statdata$caseid_IVS_ <-as.factor(eval(parse(text = paste("statdata$", caseid_IVS_))))
	caseid_IVS_name<- caseid_IVS_
	}



#Breakdown the list of responses
resplist<-c()
tempChanges <-strsplit(responses_IVS_, ",")
expectedChanges <- c(0)
for (i in 1:length(tempChanges[[1]])) 
{
	expectedChanges [length(expectedChanges )+1] = (tempChanges[[1]][i]) 
} 
for (j in 1:(length(expectedChanges)-1)) 
{
	resplist[j] = expectedChanges[j+1] 
} 

#Breakdown the list of continuous predictors
Xlist<-c()
if (contPred_ != "NULL")
{

	tempChanges <-strsplit(contPred_, ",")
	expectedChanges <- c(0)
	for (i in 1:length(tempChanges[[1]])) 
	{
		expectedChanges [length(expectedChanges )+1] = (tempChanges[[1]][i]) 
	} 
	for (j in 1:(length(expectedChanges)-1)) 
	{
		Xlist[j] = expectedChanges[j+1] 
	} 
}

#categorical predictor only used in the PCA analysis, cluster analysis
if (catPred_ != "NULL")
{
	statdata$catPred_ <-as.factor(eval(parse(text = paste("statdata$", catPred_))))
}

#Categorical predictor set up if none selected by the user
if (catPred_ == "NULL")
{
	statdata$catPred_ <- as.factor(c(rep(" ", dim(statdata)[1])))
} 

#data management
Responses_IVS_  <- statdata[,resplist]
if (contPred_ != "NULL")
{
	TreatmentsPLS_IVS_ <- statdata[,Xlist]
}
Treatments_IVS_ <- statdata$catPred_
CaseIDs_IVS_    <- statdata[,"caseid_IVS_"]
rownames(Responses_IVS_)<-CaseIDs_IVS_






#=================================================================================================================
#=================================================================================================================
#=================================================================================================================
#=================================================================================================================
# Principal components analysis

if (analysisType == "PCA" )
{

#=================================================================================================================
#PCA options

if (PCA_center == "Centered_at_zero")
{ 
	PCA_c <- TRUE
	PCA_c_text <- "have been centered at zero"
}

if (PCA_center == "Not_centered")
{
	PCA_c <- FALSE
	PCA_c_text <- "are not centered"
}

if (PCA_scale == "Unit_variance")
{ 
	PCA_s <- TRUE
	PCA_s_text <- "are scaled to have unit variance"
}

if (PCA_scale == "No_scaling")
{
	PCA_s <- FALSE
	PCA_s_text <- "are not scaled"
}


#=================================================================================================================
#Output HTML header
#STB May 2012 correcting capitals
HTML.title("<bf>InVivoStat Multivariate Principal Components Analysis", HR=1, align="left")


#Sub-title heading
title<-c("Warning")
HTML.title(title, HR=2, align="left")
HTML.title("</bf> ", HR=2, align="left")
HTML.title("This module is under development, care should be taken with the results.", HR=0, align="left")

#Description
title<-c("Description")
HTML.title(title, HR=2, align="left")
HTML.title("</bf> ", HR=2, align="left")
description <- c("The following responses are included in the PCA analysis: ")
description <- paste(description, responses_IVS_, ". The responses ", sep = "")
description <- paste(description, PCA_c_text, " and ", PCA_s_text , ". ", sep = "")

if (length(Xlist)>1 && catPred_ != "NULL")
{
	description <- paste(description, "For the PCA analysis only the first of the categorisation factors selected, namely ", Xlist[1], ", will be used within this module.", sep = "")
} else if (length(Xlist) == 1 && catPred_ != "NULL") {
	description <- paste(description, "The categorical predictor used to categorise the plots is ", Xlist[1], ".", sep = "")
}
HTML.title(description, HR=0, align="left")


if (contPred_ != "NULL")
{
	HTML.title("</bf> ", HR=2, align="left")

	description <- c("The continuous predictors ")
	description <- paste(description, contPred_, " are ignored when using the PCA analysis tool in InVivoStat.", sep = "")
	HTML.title(description, HR=0, align="left")
}




#=================================================================================================================
#Perform the PCA analysis
pca<-prcomp(Responses_IVS_ , scale=PCA_s , center=PCA_c, retx=T)
#=================================================================================================================

#Summary table of principal components
test<-summary(pca)
table0<-rbind(test$importance[1,],test$importance[2,],test$importance[3,])
rownames(table0)<-c("Standard deviation", "Proportion of variance", "Cumulative proportion")

HTMLbr()
title<-c("Summary results of principal components (PC)")
HTML.title(title, HR=2, align="left")
HTML(table0, classfirstline="second", align="left")
HTML.title("This table summarises the proportion of the total variance explained by each principal component.", HR=0, align="left")


#=================================================================================================================
#Screeplot
HTMLbr()
title<-c("Plot of the standard deviation of each principal component")
HTML.title(title, HR=2, align="left")

scatterPlot <- sub(".html", "scatterPlot.jpg", htmlFile)
jpeg(scatterPlot)

#STB July2013
plotFilepdf <- sub(".html", "scatterPlot.pdf", htmlFile)
dev.control("enable") 

#=================================================================================================================
YAxisTitle<-"Standard deviation"
XAxisTitle<-"Principal component"
MainTitle2 <-""
yvarrr_IVS_SEM<- test$importance[1,]
xvarrr_IVS_SEM<- c(1:length(yvarrr_IVS_SEM))
for (i in 1:length(yvarrr_IVS_SEM))
{
	xvarrr_IVS_SEM[i] <- paste("PC", xvarrr_IVS_SEM[i], sep = "")
}
graphdata_SEM<- data.frame(yvarrr_IVS_SEM)
graphdata_SEM$xvarrr_IVS_SEM <-xvarrr_IVS_SEM

NONCAT_SEMx()
#=============================================================================================
void <- HTMLInsertGraph(GraphFileName=sub("[A-Z0-9a-z,:,\\\\]*App_Data[\\\\]","", scatterPlot), Align="centre")

#STB July2013
if (pdfout=="Y")
{
	pdf(file=sub("[A-Z0-9a-z,:,\\\\]*App_Data[\\\\]","", plotFilepdf)) 
	dev.set(2) 
	dev.copy(which=3) 
	dev.off(2)
	dev.off(3)
	pdfFile<-sub("[A-Z0-9a-z,:,\\\\]*App_Data[\\\\]","",plotFilepdf)
	linkToPdf <- paste ("<a href=\"",pdfFile,"\">Click here to view the PDF of the plot</a>", sep = "")
	HTML(linkToPdf)
}

HTMLbr()
HTML.title("This plot illustrates the standard deviation of each principal component. ", HR=0, align="left")


#=================================================================================================================
#Loading table
table2<-pca$rotation

HTMLbr()
title<-c("Principal component loadings")
HTML.title(title, HR=2, align="left")
HTML(table2, classfirstline="second", align="left")
HTML.title("This table summarises the loadings of principal components. Responses with the larger absolute loadings have a greater influence on the corresponding principal component.", HR=0, align="left")

#=================================================================================================================
#Biplot
HTMLbr()
title<-c("Biplot of the first two principal components")
HTML.title(title, HR=2, align="left")

scatterPlot2c <- sub(".html", "scatterPlot2c.jpg", htmlFile)
jpeg(scatterPlot2c)

#STB July2013
plotFilepdf2c <- sub(".html", "scatterPlot2c.pdf", htmlFile)
dev.control("enable") 

#Replace unusual characters in labels on arrows
labells<-rownames(pca$rotation)
labells<-namereplaceGSUB(labells)
rownames(pca$rotation)<-labells

PCbiplot(pca)
#=================================================================================================================
void <- HTMLInsertGraph(GraphFileName=sub("[A-Z0-9a-z,:,\\\\]*App_Data[\\\\]","", scatterPlot2c), Align="centre")

#STB July2013
#STB July2013
if (pdfout=="Y")
{
	pdf(file=sub("[A-Z0-9a-z,:,\\\\]*App_Data[\\\\]","", plotFilepdf2c)) 
	dev.set(2) 
	dev.copy(which=3) 
	dev.off(2)
	dev.off(3)
	pdfFile_2c<-sub("[A-Z0-9a-z,:,\\\\]*App_Data[\\\\]","",plotFilepdf2c)
	linkToPdf2c <- paste ("<a href=\"",pdfFile_2c,"\">Click here to view the PDF of the plot</a>", sep = "")
	HTML(linkToPdf2c)
}
#Sub-title heading
HTML.title("</bf> ", HR=2, align="left")
HTML.title("These axes are scaled to allow both loading and scores to be included on the same plot.", HR=0, align="left")


#=================================================================================================================
HTMLbr()
title<-c("Plot of the first two principal components, ")

if (catPred_ != "NULL")
{
	title <-paste(title, "categorised by the ", catPred_ , " variable and ", sep = "")
}
title <-paste(title, "labelled by the ", caseid_IVS_name , " variable", sep = "")
HTML.title(title, HR=2, align="left")

#Categorised scatterplot of first two principal components
#data management


scatterPlotv <- sub(".html", "scatterPlotv.jpg", htmlFile)
jpeg(scatterPlotv)

#STB July2013
plotFilepdfv <- sub(".html", "scatterPlotv.pdf", htmlFile)
dev.control("enable")

dat<-biplot(pca)

#=================================================================================================================
graphdata<-cbind(pca$x , data.frame(Treatments_IVS_))
graphdata$xvarrr_IVS<-graphdata$PC1
graphdata$yvarrr_IVS<-graphdata$PC2
graphdata$l_l<-graphdata$Treatments_IVS_

YAxisTitle<-"PC2"
XAxisTitle<-"PC1"
MainTitle2 <-""
w_Gr_jit<-0
h_Gr_jit<-0
catvartest<- "N"
Gr_palette<-palette_FUN("Treatments_IVS_")
LinearFit <- "N"
obznames<- statdata$caseid_IVS_

if (catPred_ != "NULL")
{
	OVERLAID_SCAT()
} else {
		if (caseid_IVS_ == "NULL")
		{
			obznames<- c(1:dim(statdata)[1])
		} else {
				obznames<- statdata$caseid_IVS_
			}
		infiniteslope <- "Y"
		NONCAT_SCAT("PCAPLOT")
	}

#=================================================================================================================
void <- HTMLInsertGraph(GraphFileName=sub("[A-Z0-9a-z,:,\\\\]*App_Data[\\\\]","", scatterPlotv), Align="centre")

#STB July2013
if (pdfout=="Y")
{
	pdf(file=sub("[A-Z0-9a-z,:,\\\\]*App_Data[\\\\]","", plotFilepdfv)) 
	dev.set(2) 
	dev.copy(which=3) 
	dev.off(2)
	dev.off(3)
	pdfFilev<-sub("[A-Z0-9a-z,:,\\\\]*App_Data[\\\\]","",plotFilepdfv)
	linkToPdfv <- paste ("<a href=\"",pdfFilev,"\">Click here to view the PDF of the plot</a>", sep = "")
	HTML(linkToPdfv)
}	

#End of PCA statement
}
#=================================================================================================================
#=================================================================================================================
#=================================================================================================================
#=================================================================================================================




















#=================================================================================================================
#=================================================================================================================
#=================================================================================================================
#=================================================================================================================
# Cluster analysis

if (analysisType == "Cluster" )
{

#setting up the parameters

#no of clusters
no_clust <- as.numeric(noOfClusters)

#Distance method
if (distanceMethod == "Euclidean")
	{
		clus_dist <- "eu"
	}
if (distanceMethod == "Maximum")
	{
		clus_dist <- "maximum"
	}
if (distanceMethod == "Manhattan")
	{
		clus_dist <- "manhattan"
	}
if (distanceMethod == "Canberra")
	{
		clus_dist <- "canberra"
	}
if (distanceMethod == "Binary")
	{
		clus_dist <- "binary"
	}
if (distanceMethod == "Minkowski")
	{
		clus_dist <- "minkowski"
	}

#Agglomeration method
if (agglomerationMethod == "Ward.d2")
	{
		clus_meth <- "ward.D2"
	}
if (agglomerationMethod == "Single")
	{
		clus_meth <- "single"
	}
if (agglomerationMethod == "Complete")
	{
		clus_meth <- "complete"
	}
if (agglomerationMethod == "Average")
	{
		clus_meth <- "average"
	}


#Plot labels
if (plotLabels == "Case ID")
	{
		labellz <- "Case_ID"
	}
if (plotLabels == "Categorial Predictor")
	{
		labellz <- "Treatment"
	}






#Output HTML header
#STB May 2012 correcting capitals
HTML.title("<bf>InVivoStat Multivariate Cluster Analysis", HR=1, align="left")


#Sub-title heading
title<-c("Warning")
HTML.title(title, HR=2, align="left")
HTML.title("</bf> ", HR=2, align="left")
HTML.title("This module is under development, care should be taken with the results.", HR=0, align="left")


#Description
title<-c("Description")
HTML.title(title, HR=2, align="left")
HTML.title("</bf> ", HR=2, align="left")

title <-c("The cluster analysis has been performed using the ")
title <-paste(title, agglomerationMethod , " clustering agglomeration method. Distances between the individual points are calculated using the ", sep = "")
title <- paste (title, distanceMethod , " distance measure. The analysis has been set-up to generate ", no_clust, " clusters.", sep = "") 
HTML.title(title, HR=0, align="left")

HTML.title("</bf> ", HR=2, align="left")
description <- c("The following responses are included in the analysis: ")
description <- paste(description, responses_IVS_, ". ", sep = "")

if (plotLabels == "Case ID")
{
	description <- paste(description, "The points on the scatterplots are labelled using the ", plotLabels, " variable." , sep = "")
} else {
		if (length(Xlist)>1 && catPred_ != "NULL")
		{
			description <- paste(description, "For the Cluster analysis, only the first of the categorisation factors selected, namely ", Xlist[1], ", will be used to categorise the scatterplots.", sep = "")
		} else if (length(Xlist) == 1 && catPred_ != "NULL") 
			{
				description <- paste(description, "The categorical predictor used to categorise the scatterplots is ", Xlist[1], ".", sep = "")
			}
	}
HTML.title(description, HR=0, align="left")



if (contPred_ != "NULL")
{
	HTML.title("</bf> ", HR=2, align="left")

	description <- c("The continuous predictors ")
	description <- paste(description, contPred_, " are ignored when using the Cluster analysis tool in InVivoStat.", sep = "")
	HTML.title(description, HR=0, align="left")
}




#=================================================================================================================
#Cluster plot
HTMLbr()
title<-c("Dendogram of clusters")
HTML.title(title, HR=2, align="left")

ncscatterplot4 <- sub(".html", "ncscatterplot4.jpg", htmlFile)
jpeg(ncscatterplot4,width = jpegwidth, height = jpegheight, quality = 100)

#STB July2013
plotFilepdf4 <- sub(".html", "ncscatterplot4.pdf", htmlFile)
dev.control("enable") 

#=================================================================================================================
pluton_dist<-dist(Responses_IVS_, method=clus_dist)
h <- hclust(pluton_dist , method=clus_meth)

YAxisTitle <- "Distance"
XAxisTitle <- "Case ID" 

ggdendro()
#=================================================================================================================
void<-HTMLInsertGraph(GraphFileName=sub("[A-Z0-9a-z,:,\\\\]*App_Data[\\\\]","", ncscatterplot4), Align="centre")
#STB July2013
if (pdfout=="Y")
{
	pdf(file=sub("[A-Z0-9a-z,:,\\\\]*App_Data[\\\\]","", plotFilepdf4), height = pdfheight, width = pdfwidth) 
	dev.set(2) 
	dev.copy(which=3) 
	dev.off(2)
	dev.off(3)
	pdfFile_4<-sub("[A-Z0-9a-z,:,\\\\]*App_Data[\\\\]","",plotFilepdf4)
	linkToPdf4 <- paste ("<a href=\"",pdfFile_4,"\">Click here to view the PDF of the dendogram</a>", sep = "")
	HTML(linkToPdf4)
}

#=================================================================================================================
#Number of clusters plot
HTMLbr()
title<-c("Within group sum of squares for different numbers of clusters")
HTML.title(title, HR=2, align="left")

scatterPlot <- sub(".html", "scatterPlot.jpg", htmlFile)
jpeg(scatterPlot)

#STB July2013
plotFilepdf <- sub(".html", "scatterPlot.pdf", htmlFile)
dev.control("enable") 

#Code to generate plot to highlight number of clusters
n <- dim(Responses_IVS_)[1]
wss1 <- (n-1)*sum(apply(Responses_IVS_,2,var))
wss <- numeric(8)
for(i in 2:9){
  W <- sum(kmeans(Responses_IVS_,i)$withinss)
  wss[i-1] <- W
}
wss <- c(wss1,wss)

#=================================================================================================================
YAxisTitle<-"Within groups sum of squares"
XAxisTitle<-"Number of clusters"
MainTitle2 <-""
yvarrr_IVS_SEM<- wss
xvarrr_IVS_SEM<- c(1:length(wss))
for (i in 1:length(wss))
{
	xvarrr_IVS_SEM[i] <- paste("", xvarrr_IVS_SEM[i], sep = "")
}
graphdata_SEM<- data.frame(yvarrr_IVS_SEM)
graphdata_SEM$xvarrr_IVS_SEM <-xvarrr_IVS_SEM

NONCAT_SEMx()
#=================================================================================================================
void <- HTMLInsertGraph(GraphFileName=sub("[A-Z0-9a-z,:,\\\\]*App_Data[\\\\]","", scatterPlot), Align="centre")

#STB July2013
if (pdfout=="Y")
{
	pdf(file=sub("[A-Z0-9a-z,:,\\\\]*App_Data[\\\\]","", plotFilepdf)) 
	dev.set(2) 
	dev.copy(which=3) 
	dev.off(2)
	dev.off(3)
	pdfFile<-sub("[A-Z0-9a-z,:,\\\\]*App_Data[\\\\]","",plotFilepdf)
	linkToPdf <- paste ("<a href=\"",pdfFile,"\">Click here to view the PDF of the plot</a>", sep = "")
	HTML(linkToPdf)
}

HTML.title("</bf> ", HR=2, align="left")
HTML.title("This plot can be used to identify a suitable number of clusters. You should aim to reduce the within sums of squares while using as few clusters as possible.", HR=0, align="left")

#=================================================================================================================
HTML.title("</bf> ", HR=2, align="left")
HTMLbr()
title<-c("Analysis based on user-defined number of clusters")
HTML.title(title, HR=2, align="left")

HTML.title("</bf> ", HR=2, align="left")
title<-c("The following analysis results are obtained when categorising the data into ")
title <- paste(title , no_clust , " clusters.", sep="")
HTML.title(title, HR=0, align="left")



#Code to produce dataset for matrix plot
title<-c("Scatterplot of data, categorised by the user-defined number of clusters")
HTML.title(title, HR=2, align="left")

#input no. of clusters
plut.km <- kmeans(Responses_IVS_ , no_clust, nstart=10)
stdata <- data.frame(Responses_IVS_ , catfact=plut.km$cluster)

#=================================================================================================================
#GGPLOT matrix plot

scatterPlot2 <- sub(".html", "scatterPlot2.jpg", htmlFile)
jpeg(scatterPlot2)

#STB July2013
plotFilepdf2 <- sub(".html", "scatterPlot2.pdf", htmlFile)
dev.control("enable") 

#====================================================================================================================================================
#Matrixplot parameters and dataset setup
#Breakdown the list of responses
resplist<-c()
tempChanges <-strsplit(responses_IVS_, ",")
expectedChanges <- c(0)
for (i in 1:length(tempChanges[[1]])) 
{
	expectedChanges [length(expectedChanges )+1] = (tempChanges[[1]][i]) 
} 
for (j in 1:(length(expectedChanges)-1)) 
{
	resplist[j] = expectedChanges[j+1] 
} 
resplength<-length(resplist)

matrixdata <- data.frame(xvarrr_IVS = NA, yvarrr_IVS = NA, firstcatvarrr_IVS = NA, secondcatvarrr_IVS=NA, l_li=NA)

for (s in 1:length(levels(as.factor(stdata$catfact))))
{
	stdatax<-subset(stdata, stdata$catfact == unique(levels(as.factor(stdata$catfact)))[s])

	for (i in 1:resplength)
	{
		for (j in 1:resplength)
		{
			if (i != j)
			{
			xvarrr_IVS = eval(parse(text = paste("stdatax$",tempChanges[[1]][i])))
			yvarrr_IVS = eval(parse(text = paste("stdatax$",tempChanges[[1]][j])))

			secondcatvarrr_IVS<-rep(tempChanges[[1]][i], length(xvarrr_IVS))
			firstcatvarrr_IVS<-rep(tempChanges[[1]][j], length(xvarrr_IVS))
			l_li<- unique(levels(as.factor(stdata$catfact)))[s]

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
	graphdata<-matrixdata[-1,]
	graphdata$catfact <- paste(graphdata$firstcatvarrr_IVS, graphdata$secondcatvarrr_IVS, sep = "")
	graphdata$catfact_2 <- paste(graphdata$firstcatvarrr_IVS, graphdata$secondcatvarrr_IVS, graphdata$l_li, sep = "")
	graphdata$l_l <- paste(graphdata$firstcatvarrr_IVS, graphdata$secondcatvarrr_IVS, sep = "")

	Gr_alpha <- 1
	Line_type <-Line_type_solid
	w_Gr_jit <- 0
	h_Gr_jit <- 0
	ScatterPlot<-"Y"
	LinearFit <- "N"
	FirstCatFactor <- "firstcatvarrr_IVS"
	SecondCatFactor <- "secondcatvarrr_IVS"
	YAxisTitle <- ""
	XAxisTitle <- ""
	scalexy<-"free"
	MainTitle2<-""
	GraphStyle <- "Overlaid"
	Gr_legend_pos<-"none"

#Testing for with infinite slopes on scatterplot and re-ordering dataset if necessary
	inf_slope<-IVS_F_infinite_slope()
	infiniteslope <- inf_slope$infiniteslope
	graphdata<-inf_slope$graphdata

	Gr_palette<-palette_FUN("l_li")

#GGplot2 code
OVERLAID_MAT()
#=================================================================================================================
void <- HTMLInsertGraph(GraphFileName=sub("[A-Z0-9a-z,:,\\\\]*App_Data[\\\\]","", scatterPlot2), Align="centre")

#STB July2013
	if (pdfout=="Y")
	{
		pdf(file=sub("[A-Z0-9a-z,:,\\\\]*App_Data[\\\\]","", plotFilepdf2)) 
		dev.set(2) 
		dev.copy(which=3) 
		dev.off(2)
		dev.off(3)
		pdfFile2<-sub("[A-Z0-9a-z,:,\\\\]*App_Data[\\\\]","",plotFilepdf2)
		linkToPdf2 <- paste ("<a href=\"",pdfFile2,"\">Click here to view the PDF of the plot</a>", sep = "")
		HTML(linkToPdf2)
	}
#=================================================================================================================

#Categorised plots

length <- length(unique(levels(as.factor(stdata$catfact))))
tablenames<-c(levels(as.factor(stdata$catfact)))

qt<-1

#Title for noncat plots
HTMLbr()
title<-c("Individual scatterplots, categorised by user-defined number of clusters and labelled by the ")

if (labellz == "Case_ID")
{
title<-paste(title, caseid_IVS_name , " variable", sep="")
}

if (labellz == "Treatment")
{
title<-paste(title, catPred_ , " variable", sep="")
}
HTML.title(title, HR=2, align="left")

for (d in 1:resplength)
{
	for (f in 1:resplength)
	{
		if ( d != f)
		{
			ncscatterplot3 <- sub(".html", paste(d,f,"ncscatterplot3.jpg",sep=""), htmlFile)
			jpeg(ncscatterplot3,width = jpegwidth, height = jpegheight, quality = 100)

#STB July2013
			plotFilepdf3 <- sub(".html", paste(d,f,"ncscatterplot3.pdf",sep=""), htmlFile)
			dev.control("enable") 
#=================================================================================================================
			graphdata <-stdata

			Gr_palette<-palette_FUN("catfact")

			graphdata$xvarrr_IVS = eval(parse(text = paste("statdata$",tempChanges[[1]][d])))
			graphdata$yvarrr_IVS = eval(parse(text = paste("statdata$",tempChanges[[1]][f])))
			graphdata$l_l <- as.factor(graphdata$catfact)
	
			MainTitle2<-""
			w_Gr_jit <- 0
			h_Gr_jit <- 0
			Gr_legend_pos<-"none"
			infiniteslope <- "N"
			LinearFit <- "N"
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
			for (z in 1:10)
			{
				YAxisTitle<-namereplace(YAxisTitle) 
			}

			for (z in 1:10)
			{
				XAxisTitle<-namereplace(XAxisTitle) 
			}

#Testing for with infinite slopes on scatterplot and re-ordering dataset if necessary
			inf_slope<-IVS_F_infinite_slope()
			infiniteslope <- inf_slope$infiniteslope
			graphdata<-inf_slope$graphdata

#GGPLOT2 code
			if (labellz == "Case_ID")
			{
				graphdata$obznames <- statdata$caseid_IVS_
			}
			if (labellz == "Treatment")
			{
				graphdata$obznames <- statdata$catPred_
			}
			OVERLAID_SCAT()
#====================================================================================================================================================
			void<-HTMLInsertGraph(GraphFileName=sub("[A-Z0-9a-z,:,\\\\]*App_Data[\\\\]","", ncscatterplot3), Align="centre")
#STB July2013
			if (pdfout=="Y")
			{
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
#=================================================================================================================

#End of cluster statement
}
#=================================================================================================================
#=================================================================================================================
#=================================================================================================================
#=================================================================================================================














#=================================================================================================================
#=================================================================================================================
#=================================================================================================================
#=================================================================================================================
if (analysisType == "LDA" )
{

#Output HTML header
#STB May 2012 correcting capitals
HTML.title("<bf>InVivoStat Linear Discriminant Analysis", HR=1, align="left")

#Sub-title heading
title<-c("Warning")
HTML.title(title, HR=2, align="left")
HTML.title("</bf> ", HR=2, align="left")
HTML.title("This module is under development, care should be taken with the results.", HR=0, align="left")

#Description
title<-c("Description")
HTML.title(title, HR=2, align="left")
HTML.title("</bf> ", HR=2, align="left")
HTML.title("The purpose of linear discriminant analysis (LDA) is to identify the linear combinations of the original variables that gives the best possible separation between the groups." , HR=0, align="left")
HTML.title("</bf> ", HR=2, align="left")

description <- c("The following responses are included in the analysis: ")
description <- paste(description, responses_IVS_, ". ", sep = "")
if (length(Xlist)>1 && catPred_ != "NULL")
{
	description <- paste(description, "For the LDA analysis only the first of the categorisation factors selected, namely ", Xlist[1], ", will be used wto categorise the response data.", sep = "")
} else if (length(Xlist) == 1 && catPred_ != "NULL") {
	description <- paste(description, "The categorical predictor used to categorise the data is ", Xlist[1], ".", sep = "")
}
HTML.title(description, HR=0, align="left")


if (contPred_ != "NULL")
{
	HTML.title("</bf> ", HR=2, align="left")

	description <- c("The continuous predictors ")
	description <- paste(description, contPred_, " are ignored when using the LDA analysis tool in InVivoStat.", sep = "")
	HTML.title(description, HR=0, align="left")
}



#=================================================================================================================
lda_anal<-lda(x=Responses_IVS_ , grouping=Treatments_IVS_ )
#=================================================================================================================

#Summary results
HTMLbr()
title<-c("Summary results")
HTML.title(title, HR=2, align="left")

#Group means
tone<-data.frame(lda_anal[3])
colnames(tone)<-c(resplist)
title<-c("Group means")
HTML.title(title, HR=2, align="left")
HTML(tone, classfirstline="second", align="left")
HTML("Table of means for each response, categorised by the categorical predictor.", classfirstline="second", align="left")



#Coefficients of linear discriminants
HTMLbr()
ttwo<-data.frame(lda_anal[4])
tempnames<-colnames(ttwo)
tempnames<-gsub("scaling."," ", tempnames,fixed=TRUE)
colnames(ttwo)<-tempnames
title<-c("Coefficients of the linear discriminant functions (LD)")
HTML.title(title, HR=2, align="left")
HTML(ttwo, classfirstline="second", align="left")
HTML.title("This table contains the coefficients that are used to calculate the linear combinations of the original variables (the linear discriminant functions)." , HR=0, align="left")



#=================================================================================================================
#Values of the first discriminant function
if (dim(ttwo)[2] == 1)
{
	title<-c("Values of the linear discriminant function")
} else {
	title<-c("Values of the first two linear discriminant functions (LD1 and LD2)")
}
HTMLbr()
HTML.title(title, HR=2, align="left")

predy<- data.frame(predict(lda_anal, newdata=Responses_IVS_))
tthree<-cbind(Responses_IVS_ , Treatments_IVS_ , CaseIDs_IVS_,predy)
tthree<-cbind(CaseIDs_IVS_,predy)


if (dim(ttwo)[2] == 1)
{ 
tthree<-tthree[,c("LD1")]
tthree2<-cbind(Responses_IVS_ , Treatments_IVS_ , CaseIDs_IVS_,tthree)

colnames(tthree2)<- c(resplist, "Treatment", "Case ID", "LD1")
} else {
tthree<-tthree[,c("x.LD1","x.LD2")]
tthree2<-cbind(Responses_IVS_ , Treatments_IVS_ , CaseIDs_IVS_,tthree)
colnames(tthree2)<- c(resplist, "Treatment", "Case ID", "LD1", "LD2")
}
HTML(tthree2, classfirstline="second", align="left")
HTML.title("This table contains the linear discriminant functions along with the original responses." , HR=0, align="left")

#=================================================================================================================
#Histogram of first discriminant functions

title<-c("Histogram of the first linear discriminant function values, categorised by ")
title <- paste(title, catPred_, sep = "")
HTMLbr()
HTML.title(title, HR=2, align="left")

ncscatterplot3 <- sub(".html", "ncscatterplot3.jpg", htmlFile)
jpeg(ncscatterplot3,width = jpegwidth, height = jpegheight, quality = 100)

#STB July2013
plotFilepdf3 <- sub(".html", "ncscatterplot3.pdf", htmlFile)
dev.control("enable") 


#===================================================================================================
#GGPLOT OPTIONS
predy<- predict(lda_anal, newdata=Responses_IVS_)
graphdata<-data.frame(predy$x)
graphdata$yvarrr_IVS <- graphdata$LD1
graphdata$firstcatvarrr_IVS <-Treatments_IVS_
graphdata$secondcatvarrr_IVS <-c("First linear discriminant function’s values")


#Creating normal distribution grid
graphdatazzz<-graphdata
graphdatazzz$Catz<-paste(graphdatazzz$firstcatvarrr_IVS, "xxx", graphdatazzz$secondcatvarrr_IVS)
grid <- with(graphdatazzz, seq(min(yvarrr_IVS, na.rm=TRUE), max(yvarrr_IVS, na.rm=TRUE), length=100))
normaldens<-ddply(graphdatazzz, "Catz", function (df)
{
  data.frame(
    yvarrr_IVS = grid,
    density=dnorm(grid, mean(df$yvarrr_IVS), sd(df$yvarrr_IVS))
  )
})

temp <- t(data.frame(strsplit(normaldens$Catz," xxx ")))
rownames(temp)<-NULL
colnames(temp)<-c("firstcatvarrr_IVS", "secondcatvarrr_IVS")
normaldens<-cbind(normaldens, temp)

YAxisTitle <- ""
XAxisTitle <- ""
MainTitle2 <- ""
Line_type <- Line_type_blank
Gr_alpha<-0

#calculating the bin width
ymax<-max(graphdatazzz$yvarrr_IVS, na.rm=TRUE)
ymin<-min(graphdatazzz$yvarrr_IVS, na.rm=TRUE)
binrange<-(ymax-ymin)/20


TWOCATSEP_HIS()
void<-HTMLInsertGraph(GraphFileName=sub("[A-Z0-9a-z,:,\\\\]*App_Data[\\\\]","", ncscatterplot3), Align="centre")

if (pdfout=="Y")
{
	pdf(file=sub("[A-Z0-9a-z,:,\\\\]*App_Data[\\\\]","", plotFilepdf3), height = pdfheight, width = pdfwidth) 
	dev.set(2) 
	dev.copy(which=3) 
	dev.off(2)
	dev.off(3)
	pdfFile_3<-sub("[A-Z0-9a-z,:,\\\\]*App_Data[\\\\]","",plotFilepdf3)
	linkToPdf3 <- paste ("<a href=\"",pdfFile_3,"\">Click here to view the PDF of the plot</a>", sep = "")
	HTML(linkToPdf3)
}
#=================================================================================================================

#Histogram of second discriminant functions
if (dim(ttwo)[2] != 1)
{
	title<-c("Histogram of the second linear discriminant function values, categorised by ")
	title <- paste(title, catPred_, sep = "")
	HTMLbr()
	HTML.title(title, HR=2, align="left")

	ncscatterplot3b <- sub(".html", "ncscatterplot3b.jpg", htmlFile)
	jpeg(ncscatterplot3b,width = jpegwidth, height = jpegheight, quality = 100)

#STB July2013
	plotFilepdf3b <- sub(".html", "ncscatterplot3b.pdf", htmlFile)
	dev.control("enable") 

#===================================================================================================
#GGPLOT OPTIONS
	graphdata<-data.frame(predy$x)
	graphdata$yvarrr_IVS <- graphdata$LD2
	graphdata$firstcatvarrr_IVS <-Treatments_IVS_
	graphdata$secondcatvarrr_IVS <-c("Second linear discriminant function’s values")

#Creating normal distribution grid
	graphdatazzz<-graphdata
	graphdatazzz$Catz<-paste(graphdatazzz$firstcatvarrr_IVS, "xxx", graphdatazzz$secondcatvarrr_IVS)
	grid <- with(graphdatazzz, seq(min(yvarrr_IVS, na.rm=TRUE), max(yvarrr_IVS, na.rm=TRUE), length=100))
	normaldens<-ddply(graphdatazzz, "Catz", function (df)	
	{
	  data.frame(
	    yvarrr_IVS = grid,
	    density=dnorm(grid, mean(df$yvarrr_IVS), sd(df$yvarrr_IVS))
	  )
	})

	temp <- t(data.frame(strsplit(normaldens$Catz," xxx ")))
	rownames(temp)<-NULL
	colnames(temp)<-c("firstcatvarrr_IVS", "secondcatvarrr_IVS")
	normaldens<-cbind(normaldens, temp)

	YAxisTitle <- ""
	XAxisTitle <- ""
	MainTitle2 <- ""
	Line_type <- Line_type_blank
	Gr_alpha<-0

#calculating the bin width
	ymax<-max(graphdatazzz$yvarrr_IVS, na.rm=TRUE)
	ymin<-min(graphdatazzz$yvarrr_IVS, na.rm=TRUE)
	binrange<-(ymax-ymin)/20


	TWOCATSEP_HIS()
	void<-HTMLInsertGraph(GraphFileName=sub("[A-Z0-9a-z,:,\\\\]*App_Data[\\\\]","", ncscatterplot3b), Align="centre")

	if (pdfout=="Y")
	{
		pdf(file=sub("[A-Z0-9a-z,:,\\\\]*App_Data[\\\\]","", plotFilepdf3b), height = pdfheight, width = pdfwidth) 
		dev.set(2) 
		dev.copy(which=3) 
		dev.off(2)
		dev.off(3)
		pdfFile_3b<-sub("[A-Z0-9a-z,:,\\\\]*App_Data[\\\\]","",plotFilepdf3b)
		linkToPdf3b <- paste ("<a href=\"",pdfFile_3b,"\">Click here to view the PDF of the plot</a>", sep = "")
		HTML(linkToPdf3b)
	}
}

#=================================================================================================================
HTMLbr()
if (dim(ttwo)[2] != 1)
{
	title<-c("Plot of the first two linear discriminant functions, categorised by the ")
	title <-paste(title, catPred_ , " variable and labelled by the ", caseid_IVS_name , " variable", sep = "")
	HTML.title(title, HR=2, align="left")

	scatterPlotv <- sub(".html", "scatterPlotv.jpg", htmlFile)
	jpeg(scatterPlotv)

#STB July2013
	plotFilepdfv <- sub(".html", "scatterPlotv.pdf", htmlFile)	
	dev.control("enable")

#=================================================================================================================
	graphdata<-cbind(predy$x , data.frame(Treatments_IVS_))
	graphdata$xvarrr_IVS<-graphdata$LD1
	graphdata$yvarrr_IVS<-graphdata$LD2
	graphdata$l_l<-graphdata$Treatments_IVS_

	YAxisTitle<-"Second linear discriminant function"
	XAxisTitle<-"First linear discriminant function"
	MainTitle2 <-""
	w_Gr_jit<-0
	h_Gr_jit<-0
	catvartest<- "N"
	Gr_palette<-palette_FUN("Treatments_IVS_")
	LinearFit <- "N"
	obznames<- statdata$caseid_IVS_

	OVERLAID_SCAT()
#=================================================================================================================
	void <- HTMLInsertGraph(GraphFileName=sub("[A-Z0-9a-z,:,\\\\]*App_Data[\\\\]","", scatterPlotv), Align="centre")

#STB July2013
	if (pdfout=="Y")
	{
		pdf(file=sub("[A-Z0-9a-z,:,\\\\]*App_Data[\\\\]","", plotFilepdfv)) 
		dev.set(2) 
		dev.copy(which=3) 
		dev.off(2)
		dev.off(3)
		pdfFilev<-sub("[A-Z0-9a-z,:,\\\\]*App_Data[\\\\]","",plotFilepdfv)
		linkToPdfv <- paste ("<a href=\"",pdfFilev,"\">Click here to view the PDF of the plot</a>", sep = "")
		HTML(linkToPdfv)
	}	
}
#=================================================================================================================
#Confusion matrix

HTMLbr()
title<-c("Confusion matrix of classifications")
HTML.title(title, HR=2, align="left")
confuzion<- table(predict(lda_anal, type="class")$class, statdata$catPred_)
colnames(confuzion)<- rownames(confuzion)
HTML(confuzion, classfirstline="second", align="left")
HTML.title("This table summarises the classification of the individual cases, based on the linear discriminant function values." , HR=0, align="left")




HTMLbr()
title<-c("Table of mis-classified cases ")
HTML.title(title, HR=2, align="left")

tempmm<- data.frame(cbind (statdata$caseid_IVS_, predict(lda_anal, type="class")$class, statdata$catPred_))
newdata <- tempmm[ which(tempmm[,2] !=tempmm[,3]),]
colnames(newdata)<- c("Case ID", "Predicted group", "Actual group")
HTML(newdata, classfirstline="second", align="left")
HTML.title("This table summarises the cases that are mis-classified, based on the linear discriminant function values." , HR=0, align="left")


HTMLbr()
title<-c("Table of correctly classified cases ")
HTML.title(title, HR=2, align="left")

tempmm<- data.frame(cbind (statdata$caseid_IVS_, predict(lda_anal, type="class")$class, statdata$catPred_))
newdata <- tempmm[ which(tempmm[,2] ==tempmm[,3]),]
colnames(newdata)<- c("Case ID", "Predicted group", "Actual group")
HTML(newdata, classfirstline="second", align="left")
HTML.title("This table summarises the cases that are classified correctly, based on the linear discriminant function values." , HR=0, align="left")



#End of LDA statement
}
#=================================================================================================================
#=================================================================================================================
#=================================================================================================================
#=================================================================================================================











#=================================================================================================================
#=================================================================================================================
#=================================================================================================================
#=================================================================================================================
if (analysisType == "PLS" )
{
usercomp <- as.numeric(noOfComponents)

#=================================================================================================================
#Output HTML header
#STB May 2012 correcting capitals
HTML.title("<bf>InVivoStat Multivariate Partial Least Squares Analysis", HR=1, align="left")


#Sub-title heading
title<-c("Warning")
HTML.title(title, HR=2, align="left")
HTML.title("</bf> ", HR=2, align="left")
HTML.title("This module is under development, care should be taken with the results.", HR=0, align="left")

if (contPred_ == "NULL")
{
	HTML.title("</bf> ", HR=2, align="left")
	HTML.title("Unfortunately you have not selected any continuous predictor variables. InVivoStat's Partial Least Squares analysis requires at least one such variable. Please either select a continuous predictor variable or choose another analysis option." , HR=0, align="left")
	quit()
} 


#Description
title<-c("Description")
HTML.title(title, HR=2, align="left")
HTML.title("</bf> ", HR=2, align="left")
description <- c("The following responses: ")
description <- paste(description, responses_IVS_, " and the following continuous predictors: ", contPred_ , " are included in the analysis.", sep = "")
HTML.title(description, HR=0, align="left")


if (catPred_ != "NULL")
{
	HTML.title("</bf> ", HR=2, align="left")

	description <- c("The categorical predictor ")
	description <- paste(description, catPred_, " is ignored when using the PLS analysis tool in InVivoStat.", sep = "")
	HTML.title(description, HR=0, align="left")
}

#=================================================================================================================
#data management
Responses<- as.matrix(Responses_IVS_)
X_IVS_X<- as.matrix(TreatmentsPLS_IVS_)

PLSDataResp<- data.frame(Responses=I(Responses))
PLSDataX<- data.frame(ivs_sp_ivs=I(X_IVS_X))
PLSData<-cbind(PLSDataResp, PLSDataX)


#Initial plot to select number of comps
HTMLbr()
title<-c("Plot of the RMSEP vs. number of components")
HTML.title(title, HR=2, align="left")

HTML.title("</bf> ", HR=2, align="left")
HTML.title("Use this plot to identify the number of components to use. Choose a value that is reasonably small but corresponds to a low RMSEP", HR=0, align="left")


HTML.title("</bf> ", HR=2, align="left")
test<-c("Note the cross validation has been performed using ")
test<-paste(test, dim(Responses)[1], " leave-one-out segments.", sep = "")
HTML.title(test, HR=0, align="left")

















#======================================================================================================================
#======================================================================================================================
#LOO version - not sure which one to go with yet, with LOO or without...


#PLS model code
PLSmodel <- plsr(Responses ~ ivs_sp_ivs,  data = PLSData, validation = "LOO")
test<-RMSEP(PLSmodel)
test2<-test$val

#number of responses
rows<-dim(PLSData$Responses)[2]

#number of comps
nocomps<-length(test$val)/(2*rows)-1

#creating CV table
tableCV <- matrix(NA, ncol=rows, nrow=(nocomps+1))
k<-1
for (i in 1:(nocomps+1))
{
  for (j in 1:rows)
  {
  	tableCV[i,j]=test2[k]
  	k=k+2
  }
}
tableCV2<-data.frame(tableCV)
rownames<- c("0",1:nocomps)
tableCV3<-cbind(rownames, tableCV2)
rownames(tableCV3)<-rownames
colnames(tableCV3)<- c("Comp",colnames(Responses))

#melting the table
tableCV4<-melt(tableCV3)
tableCV4$type<-"CV"

#creating adjCV table
tableCV <- matrix(NA, ncol=rows, nrow=(nocomps+1))
k<-2
for (i in 1:(nocomps+1))
{
  for (j in 1:rows)
  {
    tableCV[i,j]=test2[k]
    k=k+2
  }
}
tableCV2<-data.frame(tableCV)
rownames<- c("0", 1:nocomps)
tableCV3<-cbind(rownames, tableCV2)
rownames(tableCV3)<-rownames
colnames(tableCV3)<- c("Comp",colnames(Responses))

#melting the table
tableCV4a<-melt(tableCV3)
tableCV4a$type<-"adjCV"

#creating the final table
finaltesttable<-rbind(tableCV4,tableCV4a)
finaltesttable$variable<-namereplace(finaltesttable$variable)
grlegend<-"right"

#======================================================================================================================
#======================================================================================================================








#======================================================================================================================
#======================================================================================================================
#nonLOO version

#PLS model code
PLSmodel <- plsr(Responses ~ ivs_sp_ivs,  data = PLSData)
test<-RMSEP(PLSmodel)
test2<-test$val

#number of responses
rows<-dim(PLSData$Responses)[2]

#number of comps
nocomps<-length(test$val)/(rows)-1

#creating CV table
tableCV <- matrix(NA, ncol=rows, nrow=(nocomps+1))
k<-1
for (i in 1:(nocomps+1))
{
  for (j in 1:rows)
  {
	tableCV[i,j]=test2[k]
	k=k+1
 }
}
tableCV2<-data.frame(tableCV)
rownames<- c("0",1:nocomps)
tableCV3<-cbind(rownames, tableCV2)
rownames(tableCV3)<-rownames
colnames(tableCV3)<- c("Comp",colnames(Responses))

#melting the table
tableCV4<-melt(tableCV3)
tableCV4$type<-"CV"

#creating adjCV table
tableCV <- matrix(NA, ncol=rows, nrow=(nocomps+1))
k<-1
for (i in 1:(nocomps+1))
{
  for (j in 1:rows)
  {
    tableCV[i,j]=test2[k]
    k=k+1
  }
}
tableCV2<-data.frame(tableCV)
rownames<- c("0", 1:nocomps)
tableCV3<-cbind(rownames, tableCV2)
rownames(tableCV3)<-rownames
colnames(tableCV3)<- c("Comp",colnames(Responses))

#melting the table
tableCV4a<-melt(tableCV3)
tableCV4a$type<-"adjCV"

#creating the final table
#finaltesttable<-rbind(tableCV4,tableCV4a)
finaltesttable<-rbind(tableCV4)
finaltesttable$variable<-namereplace(finaltesttable$variable)
grlegend<-"none"

#======================================================================================================================
#======================================================================================================================






#==================================================================================================
#plotting the final table
ncscatterplot3q <- sub(".html", "ncscatterplot3q.jpg", htmlFile)
jpeg(ncscatterplot3q,width = jpegwidth, height = jpegheight, quality = 100)

#STB July2013
plotFilepdf3q <- sub(".html", "ncscatterplot3q.pdf", htmlFile)
dev.control("enable") 


#===================================================================================================
#GGPLOT OPTIONS
finaltesttable<-finaltesttable[order(finaltesttable$type, finaltesttable$Comp), ]
graphdata<-finaltesttable
temp<-IVS_F_cpplot_colour("type")
Gr_palette_A <- temp$Gr_palette_A

PLSCVplot(grlegend)

void<-HTMLInsertGraph(GraphFileName=sub("[A-Z0-9a-z,:,\\\\]*App_Data[\\\\]","", ncscatterplot3q), Align="centre")

if (pdfout=="Y")
{
	pdf(file=sub("[A-Z0-9a-z,:,\\\\]*App_Data[\\\\]","", plotFilepdf3q), height = pdfheight, width = pdfwidth) 
	dev.set(2) 
	dev.copy(which=3) 
	dev.off(2)
	dev.off(3)
	pdfFile_3q<-sub("[A-Z0-9a-z,:,\\\\]*App_Data[\\\\]","",plotFilepdf3q)
	linkToPdf3q <- paste ("<a href=\"",pdfFile_3q,"\">Click here to view the PDF of the plot</a>", sep = "")
	HTML(linkToPdf3q)
}
#=================================================================================================================




#Run this code if user selects number of components
if (noOfComponents == 0)
{
	HTMLbr()
	title<-c("Comment")
	HTML.title(title, HR=2, align="left")
	HTML.title("Based on the above plot you should now decide how many components you wish to use. This should be defined in the InVivoStat Multivariate Analysis module interface." , HR=0, align="left")
}	else {

#==================================================================================================================
#User defined number of components
#==================================================================================================================
	HTMLbr()
	title<-c("PLS analysis based on ")
	title<-paste(title, usercomp , " components")
	HTML.title(title, HR=2, align="left")

	HTML.title("</bf> ", HR=2, align="left")
	title <- paste("The following results have been generated using ", usercomp , " components.", sep="")
	HTML.title(title, HR=0, align="left")

#====================================================================================================================
#PLS final model code
	PLSmodel <- plsr(Responses ~ X_IVS_X,  data = PLSData, ncomp=usercomp)
#PLSmodel <- plsr(Responses ~ X_IVS_X,  data = PLSData, ncomp=usercomp, validation = "LOO")
#====================================================================================================================

	HTMLbr()
	HTML.title("Score plots", HR=2, align="left")

	if (usercomp == 1)
	{
		HTML.title("As only one component has been selected, a scores plot has not been produced.", HR=0, align="left")
	} else {
		HTML.title("Score plots are often used to look for patterns, groups or outliers in the data.", HR=0, align="left")

#Table of scores
		scores<-scores(PLSmodel)

#Number of observations
		noobs<-dim(PLSData)[1]

#creating scores table
		tableScores <- matrix(NA, ncol=usercomp, nrow=noobs)
		k<-1
		for (i in 1:usercomp)
		{
			for (j in 1:noobs)
			{
				tableScores[j,i] = scores[k]
				k=k+1			
			}
		}
		rownames(tableScores)<-c(1:noobs)
		coltemp<-c(1:usercomp)
		coltemp<-paste("Componentivs_sp_ivs",coltemp, sep = "")
		colnames(tableScores)<-coltemp
		tableScores<-data.frame(tableScores)

#==================================================================================================
#plotting the final table
		scatterPlotx <- sub(".html", "scatterPlotx.jpg", htmlFile)
		jpeg(scatterPlotx,width = jpegwidth, height = jpegheight, quality = 100)

		plotFilepdf2 <- sub(".html", "scatterPlotx.pdf", htmlFile)
		dev.control("enable") 


#====================================================================================================================================================
#Matrixplot parameters and dataset setup
#====================================================================================================================================================
		matrixdata <- data.frame(xvarrr_IVS = NA, yvarrr_IVS = NA, firstcatvarrr_IVS = NA, secondcatvarrr_IVS=NA)
	
		for (i in 1:usercomp)
		{
			for (j in 1:usercomp)
			{
				if (i != j)
				{
					xvarrr_IVS = eval(parse(text = paste("tableScores$",coltemp[i])))
					yvarrr_IVS = eval(parse(text = paste("tableScores$",coltemp[j])))
					secondcatvarrr_IVS<-rep(coltemp[i], length(xvarrr_IVS))
					firstcatvarrr_IVS<-rep(coltemp[j], length(xvarrr_IVS))

#removing illegal characters using GSUB command
					secondcatvarrr_IVS<-namereplaceGSUB(secondcatvarrr_IVS)
					firstcatvarrr_IVS<-namereplaceGSUB(firstcatvarrr_IVS)
		
					tempdata<-data.frame(xvarrr_IVS,yvarrr_IVS,firstcatvarrr_IVS,secondcatvarrr_IVS)
					matrixdata<-rbind(matrixdata,tempdata)
				}
			}
		}

		graphdata<-matrixdata[-1,]
		graphdata$catfact <- paste(graphdata$firstcatvarrr_IVS, graphdata$secondcatvarrr_IVS, sep = "")
		graphdata$l_l <- paste(graphdata$firstcatvarrr_IVS, graphdata$secondcatvarrr_IVS, sep = "")

		Gr_alpha <- 1
		Line_type <-Line_type_solid
		w_Gr_jit <- 0
		h_Gr_jit <- 0
		ScatterPlot<-"Y"
		LinearFit <- "N"
		FirstCatFactor <- "firstcatvarrr_IVS"
		SecondCatFactor <- "secondcatvarrr_IVS"
		YAxisTitle <- "Score (1)"
		XAxisTitle <- "Score (2)"
		scalexy<-"free"
		MainTitle2<-""
		GraphStyle <- "Overlaid"

#Testing for with infinite slopes on scatterplot and re-ordering dataset if necessary
		inf_slope<-IVS_F_infinite_slope()
		infiniteslope <- inf_slope$infiniteslope
		graphdata<-inf_slope$graphdata

#GGPLOT2 code
		NONCAT_MAT()
#====================================================================================================================================================
		void <- HTMLInsertGraph(GraphFileName=sub("[A-Z0-9a-z,:,\\\\]*App_Data[\\\\]","", scatterPlotx), Align="centre")

#STB July2013
		if (pdfout=="Y")
		{
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
#=================================================================================================================

#=================================================================================================================
#Variability explained by the components

	expvar<-data.frame(explvar(PLSmodel))
	expvar$explvar.PLSmodel. = expvar$explvar.PLSmodel. / 100
	expvar$explvar.PLSmodel.<- format(round(expvar$explvar.PLSmodel.,2),nsmall=2)

	iden<-c(" ")
	expvar<- rbind(iden,expvar)
	colnames(expvar)<- c("Variance proportion")
	rownames(expvar)<- c("Component", 1:as.numeric(usercomp))

	HTMLbr()
	title<-c("Proportion of the variability explained by the components")
	HTML.title(title, HR=2, align="left")

	HTML.title("<bf> ", HR=2, align="left")
	HTML(expvar, classfirstline="second", align="left")

#=================================================================================================================
#Factor loadings
#=================================================================================================================
#Table of scores
	loadings<-loadings(PLSmodel)[,1:usercomp]

	if (usercomp == 1)
	{
		loadings <- t(loadings)
	}

	loadings2<-melt(loadings)
	loadings2$X1<-sub("X_IVS_X", " ", loadings2$X1, fixed = TRUE)
	loadings2$X1<-namereplace(loadings2$X1)

#Data for plot
	loadings3<-loadings2

	if (usercomp == 1)
	{
		colnames(loadings3)<-c("Component", "Variable level", "Loading")
	} else {
		colnames(loadings3)<-c("Variable level", "Component", "Loading")
		}
#Tidy up component levels
	loadings3$Component<-sub("Comp", " ", loadings3$Component, fixed = TRUE)
	loadings2$X2<-sub("Comp", "Component ", loadings2$X2, fixed = TRUE)

	HTMLbr()
	title<-c("Table of the component loadings for each continuous predictor")
	HTML.title(title, HR=2, align="left")
	HTML(loadings3, classfirstline="second", align="left")


	if (usercomp > 1)
	{

		HTMLbr()
		title<-c("Plot of the loadings for each continuous predictor")
		HTML.title(title, HR=2, align="left")

#==================================================================================================
#plotting the final table
		ncscatterplot3qq <- sub(".html", "ncscatterplot3qq.jpg", htmlFile)
		jpeg(ncscatterplot3qq,width = jpegwidth, height = jpegheight, quality = 100)

#STB July2013
		plotFilepdf3qq <- sub(".html", "ncscatterplot3qq.pdf", htmlFile)
		dev.control("enable") 

#===================================================================================================
#GGPLOT OPTIONS
		graphdata<-data.frame(loadings2)

#Colour range for individual animals on case profiles plot
		graphdata<-graphdata[order(graphdata$X2, graphdata$X1), ]
		temp<-IVS_F_cpplot_colour("X2")
		Gr_palette_A <- temp$Gr_palette_A

#plot(PLSmodel, "loadings", comps = 1:2, legendpos = "topleft")
		PLSloadingsplot()

		void<-HTMLInsertGraph(GraphFileName=sub("[A-Z0-9a-z,:,\\\\]*App_Data[\\\\]","", ncscatterplot3qq), Align="centre")

		if (pdfout=="Y")
		{
			pdf(file=sub("[A-Z0-9a-z,:,\\\\]*App_Data[\\\\]","", plotFilepdf3qq), height = pdfheight, width = pdfwidth) 
			dev.set(2) 
			dev.copy(which=3) 
			dev.off(2)
			dev.off(3)
			pdfFile_3qq<-sub("[A-Z0-9a-z,:,\\\\]*App_Data[\\\\]","",plotFilepdf3qq)
			linkToPdf3qq <- paste ("<a href=\"",pdfFile_3qq,"\">Click here to view the PDF of the plot</a>", sep = "")
			HTML(linkToPdf3qq)
		}
	}
#=================================================================================================================
#end of if statement about user selecting no of components
}

#End of PLS statement
}






#=================================================================================================================
#=================================================================================================================
#=================================================================================================================
#=================================================================================================================




#----------------------------------------------------------------------------------------------------------
#References
#----------------------------------------------------------------------------------------------------------
Ref_list<-R_refs()

#HTMLbr()
#HTML.title("<bf>Statistical references", HR=2, align="left")

#HTML.title("<bf> ", HR=2, align="left")
#HTML.title(Ref_list$BateClark_ref, HR=0, align="left")

HTMLbr()
HTML.title("<bf>R references", HR=2, align="left")

HTML.title("<bf> ", HR=2, align="left")
HTML.title(Ref_list$R_ref , HR=0, align="left")

HTML.title("<bf> ", HR=2, align="left")
HTML.title(Ref_list$R2HTML_ref, HR=0, align="left")


if (analysisType == "Cluster" )
{
	HTML.title("<bf> ", HR=2, align="left")
	HTML.title(Ref_list$cluster_ref, HR=0, align="left")

	HTML.title("<bf> ", HR=2, align="left")
	HTML.title(Ref_list$ggdendro_ref, HR=0, align="left")
}


if (analysisType == "PLS" )
{
	HTML.title("<bf> ", HR=2, align="left")
	HTML.title(Ref_list$mixOmics_ref, HR=0, align="left")
}


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
HTML.title(Ref_list$PROTO_ref, HR=0, align="left")


#----------------------------------------------------------------------------------------------------------
#Show dataset
#----------------------------------------------------------------------------------------------------------


if (showdataset=="Y")
{
	HTMLbr()
	HTML.title("<bf>Analysis dataset", HR=2, align="left")

	statdata$catfact<-NULL	

	if (caseid_IVS_ == "NULL")
	{
		statdata$Cases <- statdata$caseid_IVS_

	}

	statdata$catPred_ <-NULL	

	statdata$caseid_IVS_<-NULL




	HTML(statdata, classfirstline="second", align="left")
}

#----------------------------------------------------------------------------------------------------------------------#
#----------------------------------------------------------------------------------------------------------------------#

