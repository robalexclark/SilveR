#===================================================================================================================
#R Libraries

suppressWarnings(library(mvtnorm))
suppressWarnings(library(R2HTML))
suppressWarnings(library(cluster))
suppressWarnings(library(ggdendro))
suppressWarnings(library(mixOmics))
suppressWarnings(library(pls))

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

#===================================================================================================================
#Variable set-up

#Graphical parameters
Labelz_IVS_ <- "Y"
ReferenceLine <- "NULL"

#Setting up the Case ID variable
if (caseid_IVS_ == "NULL") {
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
for (i in 1:length(tempChanges[[1]]))  {
	expectedChanges [length(expectedChanges )+1] = (tempChanges[[1]][i]) 
} 
for (j in 1:(length(expectedChanges)-1))  {
	resplist[j] = expectedChanges[j+1] 
} 

#Breakdown the list of continuous predictors
Xlist<-c()
if (contPred_ != "NULL") {
	tempChanges <-strsplit(contPred_, ",")
	expectedChanges <- c(0)
	for (i in 1:length(tempChanges[[1]]))  {
		expectedChanges [length(expectedChanges )+1] = (tempChanges[[1]][i]) 
	} 
	for (j in 1:(length(expectedChanges)-1))  {
		Xlist[j] = expectedChanges[j+1] 
	} 
}

#categorical predictor only used in the PCA analysis, cluster analysis
if (catPred_ != "NULL") {
	statdata$catPred_ <-as.factor(eval(parse(text = paste("statdata$", catPred_))))
}

#Categorical predictor set up if none selected by the user
if (catPred_ == "NULL") {
	statdata$catPred_ <- as.factor(c(rep(" ", dim(statdata)[1])))
} 

#data management
Responses_IVS_  <- statdata[,resplist]
if (contPred_ != "NULL") {
	TreatmentsPLS_IVS_ <- statdata[,Xlist]
}
Treatments_IVS_ <- statdata$catPred_
CaseIDs_IVS_    <- statdata[,"caseid_IVS_"]
rownames(Responses_IVS_)<-CaseIDs_IVS_

#===================================================================================================================
#===================================================================================================================
# Principal components analysis
#===================================================================================================================
#===================================================================================================================
if (analysisType == "PCA" ) {

#PCA options
if (PCA_center == "Centered_at_zero") { 
	PCA_c <- TRUE
	PCA_c_text <- "have been centered at zero"
}

if (PCA_center == "Not_centered") {
	PCA_c <- FALSE
	PCA_c_text <- "are not centered"
}

if (PCA_scale == "Unit_variance") { 
	PCA_s <- TRUE
	PCA_s_text <- "are scaled to have unit variance"
}

if (PCA_scale == "No_scaling") {
	PCA_s <- FALSE
	PCA_s_text <- "are not scaled"
}

#===================================================================================================================
#Output HTML header
Title <-paste(branding, " Multivariate Principal Components Analysis", sep="")
HTML.title(Title, HR = 1, align = "left")

#Sub-title heading
HTML.title("Warning", HR=2, align="left")
HTML("This module is under development, care should be taken with the results.", align="left")

#Description
HTML.title("Description", HR=2, align="left")
description <- paste("The following responses are included in the PCA analysis: ", responses_IVS_, ". The responses ", PCA_c_text, " and ", PCA_s_text , ". ", sep = "")

if (length(Xlist)>1 && catPred_ != "NULL") {
	description <- paste(description, "For the PCA analysis only the first of the categorisation factors selected, namely ", Xlist[1], ", will be used within this module.", sep = "")
} 
if (length(Xlist) == 1 && catPred_ != "NULL") {
	description <- paste(description, "The categorical predictor used to categorise the plots is ", Xlist[1], ".", sep = "")
}
HTML(description, align="left")

if (contPred_ != "NULL") {
	description <- paste("The continuous predictors ", contPred_, " are ignored when using the PCA analysis tool in ", branding", ".", sep = "")
	HTML(description, align="left")
}

#===================================================================================================================
#Perform the PCA analysis
#===================================================================================================================
pca<-prcomp(Responses_IVS_ , scale=PCA_s , center=PCA_c, retx=T)

#Summary table of principal components
test<-summary(pca)
table0<-rbind(test$importance[1,],test$importance[2,],test$importance[3,])
rownames(table0)<-c("Standard deviation", "Proportion of variance", "Cumulative proportion")

HTML.title("Summary results of principal components (PC)", HR=2, align="left")
HTML(table0, classfirstline="second", align="left", row.names = "FALSE")
HTML("This table summarises the proportion of the total variance explained by each principal component.", align="left")

#===================================================================================================================
#Screeplot
HTML.title("Plot of the standard deviation of each principal component", HR=2, align="left")

scatterPlot <- sub(".html", "scatterPlot.jpg", htmlFile)
jpeg(scatterPlot)

#STB July2013
plotFilepdf <- sub(".html", "scatterPlot.pdf", htmlFile)
dev.control("enable") 

YAxisTitle<-"Standard deviation"
XAxisTitle<-"Principal component"
MainTitle2 <-""
yvarrr_IVS_SEM<- test$importance[1,]
xvarrr_IVS_SEM<- c(1:length(yvarrr_IVS_SEM))
for (i in 1:length(yvarrr_IVS_SEM)) {
	xvarrr_IVS_SEM[i] <- paste("PC", xvarrr_IVS_SEM[i], sep = "")
}
graphdata_SEM<- data.frame(yvarrr_IVS_SEM)
graphdata_SEM$xvarrr_IVS_SEM <-xvarrr_IVS_SEM

NONCAT_SEMx()
void <- HTMLInsertGraph(GraphFileName=sub("[A-Z0-9a-z,:,\\\\]*App_Data[\\\\]","", scatterPlot), Align="centre")

#STB July2013
if (pdfout=="Y") {
	pdf(file=sub("[A-Z0-9a-z,:,\\\\]*App_Data[\\\\]","", plotFilepdf)) 
	dev.set(2) 
	dev.copy(which=3) 
	dev.off(2)
	dev.off(3)
	pdfFile<-sub("[A-Z0-9a-z,:,\\\\]*App_Data[\\\\]","",plotFilepdf)
	linkToPdf <- paste ("<a href=\"",pdfFile,"\">Click here to view the PDF of the plot</a>", sep = "")
	HTML(linkToPdf)
}
HTML("This plot illustrates the standard deviation of each principal component. ", align="left")

#===================================================================================================================
#Loading table
table2<-pca$rotation

HTML.title("Principal component loadings", HR=2, align="left")
HTML(table2, classfirstline="second", align="left", row.names = "FALSE")
HTML("This table summarises the loadings of principal components. Responses with the larger absolute loadings have a greater influence on the corresponding principal component.", align="left")

#===================================================================================================================
#Biplot
HTML.title("Biplot of the first two principal components", HR=2, align="left")

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

void <- HTMLInsertGraph(GraphFileName=sub("[A-Z0-9a-z,:,\\\\]*App_Data[\\\\]","", scatterPlot2c), Align="centre")

#STB July2013
if (pdfout=="Y") {
	pdf(file=sub("[A-Z0-9a-z,:,\\\\]*App_Data[\\\\]","", plotFilepdf2c)) 
	dev.set(2) 
	dev.copy(which=3) 
	dev.off(2)
	dev.off(3)
	pdfFile_2c<-sub("[A-Z0-9a-z,:,\\\\]*App_Data[\\\\]","",plotFilepdf2c)
	linkToPdf2c <- paste ("<a href=\"",pdfFile_2c,"\">Click here to view the PDF of the plot</a>", sep = "")
	HTML(linkToPdf2c)
}
HTML("These axes are scaled to allow both loading and scores to be included on the same plot.", align="left")

#===================================================================================================================
#Categorised scatterplot of first two principal components

dat<-biplot(pca)

title<-c("Plot of the first two principal components, ")
if (catPred_ != "NULL") {
	title <-paste(title, "categorised by the ", catPred_ , " variable and ", sep = "")
}
title <-paste(title, "labelled by the ", caseid_IVS_name , " variable", sep = "")
HTML(title, align="left")

scatterPlotv <- sub(".html", "scatterPlotv.jpg", htmlFile)
jpeg(scatterPlotv)

#STB July2013
plotFilepdfv <- sub(".html", "scatterPlotv.pdf", htmlFile)
dev.control("enable")

graphdata<-cbind(pca$x , data.frame(Treatments_IVS_))
graphdata$xvarrr_IVS<-graphdata$PC1
graphdata$yvarrr_IVS<-graphdata$PC2
graphdata$l_l<-graphdata$Treatments_IVS_

YAxisTitle<-"PC2"
XAxisTitle<-"PC1"
MainTitle2 <-""
w_Gr_jitscat<-0
h_Gr_jitscat<-0
catvartest<- "N"
Gr_palette<-palette_FUN("Treatments_IVS_")
LinearFit <- "N"
obznames<- statdata$caseid_IVS_

if (catPred_ != "NULL") {
	OVERLAID_SCAT()
} else {
	if (caseid_IVS_ == "NULL") {
			obznames<- c(1:dim(statdata)[1])
	} else {
		obznames<- statdata$caseid_IVS_
	}
	infiniteslope <- "Y"
	NONCAT_SCAT("PCAPLOT")
}

void <- HTMLInsertGraph(GraphFileName=sub("[A-Z0-9a-z,:,\\\\]*App_Data[\\\\]","", scatterPlotv), Align="centre")

#STB July2013
if (pdfout=="Y") {
	pdf(file=sub("[A-Z0-9a-z,:,\\\\]*App_Data[\\\\]","", plotFilepdfv)) 
	dev.set(2) 
	dev.copy(which=3) 
	dev.off(2)
	dev.off(3)
	pdfFilev<-sub("[A-Z0-9a-z,:,\\\\]*App_Data[\\\\]","",plotFilepdfv)
	linkToPdfv <- paste ("<a href=\"",pdfFilev,"\">Click here to view the PDF of the plot</a>", sep = "")
	HTML(linkToPdfv)
}	

#===================================================================================================================
#End of PCA statement
}


#===================================================================================================================
#===================================================================================================================
# Cluster analysis
#===================================================================================================================
#===================================================================================================================
if (analysisType == "Cluster" )
{

#===================================================================================================================
#setting up the parameters
#===================================================================================================================
#no of clusters
no_clust <- as.numeric(noOfClusters)

#Distance method
if (distanceMethod == "Euclidean") {
		clus_dist <- "eu"
	}
if (distanceMethod == "Maximum") {
		clus_dist <- "maximum"
	}
if (distanceMethod == "Manhattan") {
		clus_dist <- "manhattan"
	}
if (distanceMethod == "Canberra") {
		clus_dist <- "canberra"
	}
if (distanceMethod == "Binary") {
		clus_dist <- "binary"
	}
if (distanceMethod == "Minkowski") {
		clus_dist <- "minkowski"
	}

#Agglomeration method
if (agglomerationMethod == "Ward.d2") {
		clus_meth <- "ward.D2"
	}
if (agglomerationMethod == "Single") {
		clus_meth <- "single"
	}
if (agglomerationMethod == "Complete") {
		clus_meth <- "complete"
	}
if (agglomerationMethod == "Average") {
		clus_meth <- "average"
	}

#Plot labels
if (plotLabels == "Case ID") {
		labellz <- "Case_ID"
	}
if (plotLabels == "Categorial Predictor") {
		labellz <- "Treatment"
	}

#===================================================================================================================
#Headers and descriptions
#===================================================================================================================
#STB May 2012 correcting capitals
Title <-paste(branding, " Multivariate Cluster Analysis", sep="")
HTML.title(Title, HR = 1, align = "left")
HTML("Warning, this module is under development, care should be taken with the results.", align="left")

#Description
HTML.title("Description", HR=2, align="left")
title <-paste("The cluster analysis has been performed using the ", agglomerationMethod , " clustering agglomeration method. Distances between the individual points are calculated using the ", distanceMethod , " distance measure. The analysis has been set-up to generate ", no_clust, " clusters.", sep = "")
HTML.title(title, HR=0, align="left")

description <- paste("The following responses are included in the analysis: ", responses_IVS_, ". ", sep = "")
if (plotLabels == "Case ID") {
	description <- paste(description, "The points on the scatterplots are labelled using the ", plotLabels, " variable." , sep = "")
} else {
	if (length(Xlist)>1 && catPred_ != "NULL") {
		description <- paste(description, "For the Cluster analysis, only the first of the categorisation factors selected, namely ", Xlist[1], ", will be used to categorise the scatterplots.", sep = "")
	} 
	if (length(Xlist) == 1 && catPred_ != "NULL")  {
		description <- paste(description, "The categorical predictor used to categorise the scatterplots is ", Xlist[1], ".", sep = "")
	}
}
HTML(description, align="left")

if (contPred_ != "NULL") {
	description <- paste("The continuous predictors ", contPred_, " are ignored when using the Cluster analysis tool in ", branding, ".", sep = "")
	HTML(description, align="left")
}

#===================================================================================================================
#Cluster plot
#===================================================================================================================
HTML.title("Dendogram of clusters", HR=2, align="left")

ncscatterplot4 <- sub(".html", "ncscatterplot4.jpg", htmlFile)
jpeg(ncscatterplot4,width = jpegwidth, height = jpegheight, quality = 100)

#STB July2013
plotFilepdf4 <- sub(".html", "ncscatterplot4.pdf", htmlFile)
dev.control("enable") 

pluton_dist<-dist(Responses_IVS_, method=clus_dist)
h <- hclust(pluton_dist , method=clus_meth)

YAxisTitle <- "Distance"
XAxisTitle <- "Case ID" 

ggdendro()

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

#===================================================================================================================
#Number of clusters plot
#===================================================================================================================
HTML.title("Within group sum of squares for different numbers of clusters", HR=2, align="left")

scatterPlot <- sub(".html", "scatterPlot.jpg", htmlFile)
jpeg(scatterPlot)

#STB July2013
plotFilepdf <- sub(".html", "scatterPlot.pdf", htmlFile)
dev.control("enable") 

#Code to generate plot to highlight number of clusters
n <- dim(Responses_IVS_)[1]
wss1 <- (n-1)*sum(apply(Responses_IVS_,2,var))
wss <- numeric(8)
for(i in 2:9) {
  W <- sum(kmeans(Responses_IVS_,i)$withinss)
  wss[i-1] <- W
}
wss <- c(wss1,wss)

YAxisTitle<-"Within groups sum of squares"
XAxisTitle<-"Number of clusters"
MainTitle2 <-""
yvarrr_IVS_SEM<- wss
xvarrr_IVS_SEM<- c(1:length(wss))
for (i in 1:length(wss)) {
	xvarrr_IVS_SEM[i] <- paste("", xvarrr_IVS_SEM[i], sep = "")
}
graphdata_SEM<- data.frame(yvarrr_IVS_SEM)
graphdata_SEM$xvarrr_IVS_SEM <-xvarrr_IVS_SEM

NONCAT_SEMx()

void <- HTMLInsertGraph(GraphFileName=sub("[A-Z0-9a-z,:,\\\\]*App_Data[\\\\]","", scatterPlot), Align="centre")

#STB July2013
if (pdfout=="Y") {
	pdf(file=sub("[A-Z0-9a-z,:,\\\\]*App_Data[\\\\]","", plotFilepdf)) 
	dev.set(2) 
	dev.copy(which=3) 
	dev.off(2)
	dev.off(3)
	pdfFile<-sub("[A-Z0-9a-z,:,\\\\]*App_Data[\\\\]","",plotFilepdf)
	linkToPdf <- paste ("<a href=\"",pdfFile,"\">Click here to view the PDF of the plot</a>", sep = "")
	HTML(linkToPdf)
}
HTML("This plot can be used to identify a suitable number of clusters. You should aim to reduce the within sums of squares while using as few clusters as possible.", align="left")

#===================================================================================================================
#User defined cluster results
#===================================================================================================================
HTML.title("Analysis based on user-defined number of clusters", HR=2, align="left")
title <- paste("The following analysis results are obtained when categorising the data into " , no_clust , " clusters.", sep="")
HTML(title, align="left")

#Code to produce dataset for matrix plot
HTML.title("Scatterplot of data, categorised by the user-defined number of clusters", HR=2, align="left")

#input no. of clusters
plut.km <- kmeans(Responses_IVS_ , no_clust, nstart=10)
stdata <- data.frame(Responses_IVS_ , catfact=plut.km$cluster)

#GGPLOT matrix plot
scatterPlot2 <- sub(".html", "scatterPlot2.jpg", htmlFile)
jpeg(scatterPlot2)

#STB July2013
plotFilepdf2 <- sub(".html", "scatterPlot2.pdf", htmlFile)
dev.control("enable") 

#===================================================================================================================
#Matrixplot parameters and dataset setup
#Breakdown the list of responses

resplist<-c()
tempChanges <-strsplit(responses_IVS_, ",")
expectedChanges <- c(0)
for (i in 1:length(tempChanges[[1]]))  {
	expectedChanges [length(expectedChanges )+1] = (tempChanges[[1]][i]) 
} 
for (j in 1:(length(expectedChanges)-1))  {
	resplist[j] = expectedChanges[j+1] 
} 
resplength<-length(resplist)

matrixdata <- data.frame(xvarrr_IVS = NA, yvarrr_IVS = NA, firstcatvarrr_IVS = NA, secondcatvarrr_IVS=NA, l_li=NA)
for (s in 1:length(levels(as.factor(stdata$catfact)))) {
	stdatax<-subset(stdata, stdata$catfact == unique(levels(as.factor(stdata$catfact)))[s])
	for (i in 1:resplength)	{
		for (j in 1:resplength) {
			if (i != j) {
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
w_Gr_jitscat <- 0
h_Gr_jitscat <- 0
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
graphdatax <- subset(graphdata, catvartest != "N")
graphdata<-graphdatax

Gr_palette<-palette_FUN("l_li")

#GGplot2 code
OVERLAID_MAT()

void <- HTMLInsertGraph(GraphFileName=sub("[A-Z0-9a-z,:,\\\\]*App_Data[\\\\]","", scatterPlot2), Align="centre")

#STB July2013
if (pdfout=="Y") {
	pdf(file=sub("[A-Z0-9a-z,:,\\\\]*App_Data[\\\\]","", plotFilepdf2)) 
	dev.set(2) 
	dev.copy(which=3) 
	dev.off(2)
	dev.off(3)
	pdfFile2<-sub("[A-Z0-9a-z,:,\\\\]*App_Data[\\\\]","",plotFilepdf2)
	linkToPdf2 <- paste ("<a href=\"",pdfFile2,"\">Click here to view the PDF of the plot</a>", sep = "")
	HTML(linkToPdf2)
}

#===================================================================================================================
#Individual Categorised plots
#===================================================================================================================
length <- length(unique(levels(as.factor(stdata$catfact))))
tablenames<-c(levels(as.factor(stdata$catfact)))
qt<-1

#Title for noncat plots
title<-c("Individual scatterplots, categorised by user-defined number of clusters and labelled by the ")
if (labellz == "Case_ID") {
	title<-paste(title, caseid_IVS_name , " variable", sep="")
}
if (labellz == "Treatment") {
	title<-paste(title, catPred_ , " variable", sep="")
}
HTML.title(title, HR=2, align="left")

for (d in 1:resplength) {
	for (f in 1:resplength) {
		if ( d != f) {
			ncscatterplot3 <- sub(".html", paste(d,f,"ncscatterplot3.jpg",sep=""), htmlFile)
			jpeg(ncscatterplot3,width = jpegwidth, height = jpegheight, quality = 100)

			#STB July2013
			plotFilepdf3 <- sub(".html", paste(d,f,"ncscatterplot3.pdf",sep=""), htmlFile)
			dev.control("enable") 

			graphdata <-stdata

			Gr_palette<-palette_FUN("catfact")

			graphdata$xvarrr_IVS = eval(parse(text = paste("statdata$",tempChanges[[1]][d])))
			graphdata$yvarrr_IVS = eval(parse(text = paste("statdata$",tempChanges[[1]][f])))
			graphdata$l_l <- as.factor(graphdata$catfact)
	
			MainTitle2<-""
			w_Gr_jitscat <- 0
			h_Gr_jitscat <- 0
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
			for (z in 1:10) {
				YAxisTitle<-namereplace(YAxisTitle) 
			}
			for (z in 1:10) {
				XAxisTitle<-namereplace(XAxisTitle) 
			}

			#Testing for with infinite slopes on scatterplot and re-ordering dataset if necessary
			inf_slope<-IVS_F_infinite_slope()
			infiniteslope <- inf_slope$infiniteslope
			graphdata<-inf_slope$graphdata
			graphdatax <- subset(graphdata, catvartest != "N")
			graphdata<-graphdatax

			#GGPLOT2 code
			if (labellz == "Case_ID") {
				graphdata$obznames <- statdata$caseid_IVS_
			}
			if (labellz == "Treatment") {
				graphdata$obznames <- statdata$catPred_
			}
			OVERLAID_SCAT()

			void<-HTMLInsertGraph(GraphFileName=sub("[A-Z0-9a-z,:,\\\\]*App_Data[\\\\]","", ncscatterplot3), Align="centre")

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

#===================================================================================================================
#End of cluster statement
}

#=================================================================================================================
#=================================================================================================================
#Linear Disriminant Analysis
#=================================================================================================================
#=================================================================================================================
if (analysisType == "LDA" )
{

#===================================================================================================================
#Output HTML header
#STB May 2012 correcting capitals
Title <-paste(branding, " Linear Discriminant Analysis", sep="")
HTML.title(Title, HR = 1, align = "left")
HTML.title(""Warning", this module is under development, care should be taken with the results.", HR=0, align="left")

#Description
HTML.title("Description", HR=2, align="left")
HTML("The purpose of linear discriminant analysis (LDA) is to identify the linear combinations of the original variables that gives the best possible separation between the groups." , align="left")

description <- paste("The following responses are included in the analysis: ", responses_IVS_, ". ", sep = "")
if (length(Xlist)>1 && catPred_ != "NULL") {
	description <- paste(description, "For the LDA analysis only the first of the categorisation factors selected, namely ", Xlist[1], ", will be used wto categorise the response data.", sep = "")
} else if (length(Xlist) == 1 && catPred_ != "NULL") {
	description <- paste(description, "The categorical predictor used to categorise the data is ", Xlist[1], ".", sep = "")
}
HTML(description, align="left")

if (contPred_ != "NULL") {
	description <- paste("The continuous predictors ", contPred_, " are ignored when using the LDA analysis tool in ", branding , ".", sep = "")
	HTML(description, align="left")
}

#===================================================================================================================
#Perform the analysis
#===================================================================================================================
lda_anal<-lda(x=Responses_IVS_ , grouping=Treatments_IVS_ )

#===================================================================================================================
#Summary results
#===================================================================================================================
HTML.title("Summary results", HR=2, align="left")

#Group means
tone<-data.frame(lda_anal[3])
colnames(tone)<-c(resplist)
HTML.title("Group means", HR=2, align="left")
HTML(tone, classfirstline="second", align="left", row.names = "FALSE")
HTML("Table of means for each response, categorised by the categorical predictor.", align="left")

#Coefficients of linear discriminants
ttwo<-data.frame(lda_anal[4])
tempnames<-colnames(ttwo)
tempnames<-gsub("scaling."," ", tempnames,fixed=TRUE)
colnames(ttwo)<-tempnames

HTML.title("Coefficients of the linear discriminant functions (LD)", HR=2, align="left")
HTML(ttwo, classfirstline="second", align="left" , row.names = "FALSE")
HTML("This table contains the coefficients that are used to calculate the linear combinations of the original variables (the linear discriminant functions)." , align="left")

#===================================================================================================================
#Values of the first discriminant function
#===================================================================================================================
if (dim(ttwo)[2] == 1) {
	title<-c("Values of the linear discriminant function")
} else {
	title<-c("Values of the first two linear discriminant functions (LD1 and LD2)")
}
HTML.title(title, HR=2, align="left")

predy<- data.frame(predict(lda_anal, newdata=Responses_IVS_))
tthree<-cbind(Responses_IVS_ , Treatments_IVS_ , CaseIDs_IVS_,predy)
tthree<-cbind(CaseIDs_IVS_,predy)

if (dim(ttwo)[2] == 1) { 
	tthree<-tthree[,c("LD1")]
	tthree2<-cbind(Responses_IVS_ , Treatments_IVS_ , CaseIDs_IVS_,tthree)
	colnames(tthree2)<- c(resplist, "Treatment", "Case ID", "LD1")
} else {
	tthree<-tthree[,c("x.LD1","x.LD2")]
	tthree2<-cbind(Responses_IVS_ , Treatments_IVS_ , CaseIDs_IVS_,tthree)
	colnames(tthree2)<- c(resplist, "Treatment", "Case ID", "LD1", "LD2")
}
HTML(tthree2, classfirstline="second", align="left", row.names = "FALSE")
HTML("This table contains the linear discriminant functions along with the original responses." , align="left")

#===================================================================================================================
#Histogram of first discriminant functions
#===================================================================================================================
title <- paste("Histogram of the first linear discriminant function values, categorised by ", catPred_, sep = "")
HTML.title(title, HR=2, align="left")

ncscatterplot3 <- sub(".html", "ncscatterplot3.jpg", htmlFile)
jpeg(ncscatterplot3,width = jpegwidth, height = jpegheight, quality = 100)

#STB July2013
plotFilepdf3 <- sub(".html", "ncscatterplot3.pdf", htmlFile)
dev.control("enable") 

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

normaldens<-ddply(graphdatazzz, "Catz", function (df) { data.frame(yvarrr_IVS = grid,density=dnorm(grid, mean(df$yvarrr_IVS), sd(df$yvarrr_IVS)) ) })

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

if (pdfout=="Y") {
	pdf(file=sub("[A-Z0-9a-z,:,\\\\]*App_Data[\\\\]","", plotFilepdf3), height = pdfheight, width = pdfwidth) 
	dev.set(2) 
	dev.copy(which=3) 
	dev.off(2)
	dev.off(3)
	pdfFile_3<-sub("[A-Z0-9a-z,:,\\\\]*App_Data[\\\\]","",plotFilepdf3)
	linkToPdf3 <- paste ("<a href=\"",pdfFile_3,"\">Click here to view the PDF of the plot</a>", sep = "")
	HTML(linkToPdf3)
}

#===================================================================================================================
#Histogram of second discriminant functions
#===================================================================================================================
if (dim(ttwo)[2] != 1) {
	title <- paste("Histogram of the second linear discriminant function values, categorised by ", catPred_, sep = "")
	HTML.title(title, HR=2, align="left")

	ncscatterplot3b <- sub(".html", "ncscatterplot3b.jpg", htmlFile)
	jpeg(ncscatterplot3b,width = jpegwidth, height = jpegheight, quality = 100)

	#STB July2013
	plotFilepdf3b <- sub(".html", "ncscatterplot3b.pdf", htmlFile)
	dev.control("enable") 

	#GGPLOT OPTIONS
	graphdata<-data.frame(predy$x)
	graphdata$yvarrr_IVS <- graphdata$LD2
	graphdata$firstcatvarrr_IVS <-Treatments_IVS_
	graphdata$secondcatvarrr_IVS <-c("Second linear discriminant function’s values")

	#Creating normal distribution grid
	graphdatazzz<-graphdata
	graphdatazzz$Catz<-paste(graphdatazzz$firstcatvarrr_IVS, "xxx", graphdatazzz$secondcatvarrr_IVS)
	grid <- with(graphdatazzz, seq(min(yvarrr_IVS, na.rm=TRUE), max(yvarrr_IVS, na.rm=TRUE), length=100))
	normaldens<-ddply(graphdatazzz, "Catz", function (df){data.frame(yvarrr_IVS = grid, density=dnorm(grid, mean(df$yvarrr_IVS), sd(df$yvarrr_IVS)) ) })

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

	if (pdfout=="Y") {
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

#===================================================================================================================
if (dim(ttwo)[2] != 1) {
	title <-paste("Plot of the first two linear discriminant functions, categorised by the ", catPred_ , " variable and labelled by the ", caseid_IVS_name , " variable", sep = "")
	HTML.title(title, HR=2, align="left")

	scatterPlotv <- sub(".html", "scatterPlotv.jpg", htmlFile)
	jpeg(scatterPlotv)

	#STB July2013
	plotFilepdfv <- sub(".html", "scatterPlotv.pdf", htmlFile)	
	dev.control("enable")

	graphdata<-cbind(predy$x , data.frame(Treatments_IVS_))
	graphdata$xvarrr_IVS<-graphdata$LD1
	graphdata$yvarrr_IVS<-graphdata$LD2
	graphdata$l_l<-graphdata$Treatments_IVS_

	YAxisTitle<-"Second linear discriminant function"
	XAxisTitle<-"First linear discriminant function"
	MainTitle2 <-""
	w_Gr_jitscat<-0
	h_Gr_jitscat<-0
	catvartest<- "N"
	Gr_palette<-palette_FUN("Treatments_IVS_")
	LinearFit <- "N"
	obznames<- statdata$caseid_IVS_

	OVERLAID_SCAT()
#===================================================================================================================
	void <- HTMLInsertGraph(GraphFileName=sub("[A-Z0-9a-z,:,\\\\]*App_Data[\\\\]","", scatterPlotv), Align="centre")

	#STB July2013
	if (pdfout=="Y") {
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

#===================================================================================================================
#Confusion matrix of correct and incorrect classification
#===================================================================================================================
HTML.title("Confusion matrix of classifications", HR=2, align="left")
confuzion<- table(predict(lda_anal, type="class")$class, statdata$catPred_)
colnames(confuzion)<- rownames(confuzion)
HTML(confuzion, classfirstline="second", align="left", row.names = "FALSE")
HTML("This table summarises the classification of the individual cases, based on the linear discriminant function values." , align="left")

HTML.title("Table of mis-classified cases ", HR=2, align="left")
tempmm<- data.frame(cbind (statdata$caseid_IVS_, predict(lda_anal, type="class")$class, statdata$catPred_))
newdata <- tempmm[ which(tempmm[,2] !=tempmm[,3]),]
colnames(newdata)<- c("Case ID", "Predicted group", "Actual group")
HTML(newdata, classfirstline="second", align="left", row.names = "FALSE")
HTML("This table summarises the cases that are mis-classified, based on the linear discriminant function values." , align="left")

HTML.title("Table of correctly classified cases ", HR=2, align="left")
tempmm<- data.frame(cbind (statdata$caseid_IVS_, predict(lda_anal, type="class")$class, statdata$catPred_))
newdata <- tempmm[ which(tempmm[,2] ==tempmm[,3]),]
colnames(newdata)<- c("Case ID", "Predicted group", "Actual group")
HTML(newdata, classfirstline="second", align="left", row.names = "FALSE")
HTML("This table summarises the cases that are classified correctly, based on the linear discriminant function values." , align="left")

#===================================================================================================================
#End of LDA statement
}

#===================================================================================================================
#===================================================================================================================
#Partial Least Squares
#===================================================================================================================
#===================================================================================================================
if (analysisType == "PLS" )
{
usercomp <- as.numeric(noOfComponents)

#===================================================================================================================
#Output HTML header
Title <-paste(branding, " Multivariate Partial Least Squares Analysis", sep="")
HTML.title(Title, HR = 1, align = "left")
HTML("Warning, this module is under development, care should be taken with the results.", align="left")

if (contPred_ == "NULL") {
	HTML("Unfortunately you have not selected any continuous predictor variables. This Partial Least Squares analysis implementation requires at least one such variable. Please either select a continuous predictor variable or choose another analysis option." , align="left")
	quit()
} 

#Description
HTML.title("Description", HR=2, align="left")
description <- paste("The following responses: ", responses_IVS_, " and the following continuous predictors: ", contPred_ , " are included in the analysis.", sep = "")
HTML(description, align="left")

if (catPred_ != "NULL") {
	description <- paste("The categorical predictor ", catPred_, " is ignored when using the PLS analysis tool in ", branding, ".", sep = "")
	HTML(description, align="left")
}

#===================================================================================================================
#data management
Responses<- as.matrix(Responses_IVS_)
X_IVS_X<- as.matrix(TreatmentsPLS_IVS_)

PLSDataResp<- data.frame(Responses=I(Responses))
PLSDataX<- data.frame(ivs_sp_ivs=I(X_IVS_X))
PLSData<-cbind(PLSDataResp, PLSDataX)

#Initial plot to select number of comps
HTML.title("Plot of the RMSEP vs. number of components", HR=2, align="left")
HTML("Use this plot to identify the number of components to use. Choose a value that is reasonably small but corresponds to a low RMSEP.", align="left")
test<-paste("Note the cross validation has been performed using ", dim(Responses)[1], " leave-one-out segments.", sep = "")
HTML(test, align="left")

#===================================================================================================================
#LOO version - not sure which one to go with yet, with LOO or without...
#===================================================================================================================
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
for (i in 1:(nocomps+1)) {
  for (j in 1:rows) {
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
for (i in 1:(nocomps+1)) {
  for (j in 1:rows) {
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

#===================================================================================================================
#nonLOO version
#===================================================================================================================
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
for (i in 1:(nocomps+1)) {
  for (j in 1:rows) {
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
for (i in 1:(nocomps+1)) {
  for (j in 1:rows) {
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

#===================================================================================================================
#plotting the final table
ncscatterplot3q <- sub(".html", "ncscatterplot3q.jpg", htmlFile)
jpeg(ncscatterplot3q,width = jpegwidth, height = jpegheight, quality = 100)

#STB July2013
plotFilepdf3q <- sub(".html", "ncscatterplot3q.pdf", htmlFile)
dev.control("enable") 

#GGPLOT OPTIONS
finaltesttable<-finaltesttable[order(finaltesttable$type, finaltesttable$Comp), ]
graphdata<-finaltesttable
temp<-IVS_F_cpplot_colour("type")
Gr_palette_A <- temp$Gr_palette_A

PLSCVplot(grlegend)

void<-HTMLInsertGraph(GraphFileName=sub("[A-Z0-9a-z,:,\\\\]*App_Data[\\\\]","", ncscatterplot3q), Align="centre")

if (pdfout=="Y") {
	pdf(file=sub("[A-Z0-9a-z,:,\\\\]*App_Data[\\\\]","", plotFilepdf3q), height = pdfheight, width = pdfwidth) 
	dev.set(2) 
	dev.copy(which=3) 
	dev.off(2)
	dev.off(3)
	pdfFile_3q<-sub("[A-Z0-9a-z,:,\\\\]*App_Data[\\\\]","",plotFilepdf3q)
	linkToPdf3q <- paste ("<a href=\"",pdfFile_3q,"\">Click here to view the PDF of the plot</a>", sep = "")
	HTML(linkToPdf3q)
}

#===================================================================================================================
#User defined number of components
#===================================================================================================================
if (noOfComponents == 0) {
	HTML.title("Comment", HR=2, align="left")
	HTML("Based on the above plot you should now decide how many components you wish to use. This should be defined within the Multivariate Analysis module interface." , align="left")
} else {
	title<-paste("PLS analysis based on ", usercomp , " components")
	HTML.title(title, HR=2, align="left")
	title <- paste("The following results have been generated using ", usercomp , " components.", sep="")
	HTML(title, align="left")

#===================================================================================================================
#PLS final model code
#===================================================================================================================
	PLSmodel <- plsr(Responses ~ X_IVS_X,  data = PLSData, ncomp=usercomp)
	#PLSmodel <- plsr(Responses ~ X_IVS_X,  data = PLSData, ncomp=usercomp, validation = "LOO")

#===================================================================================================================
#Score plots
#===================================================================================================================
	HTML.title("Score plots", HR=2, align="left")

	if (usercomp == 1)
	{
		HTML("As only one component has been selected, a scores plot has not been produced.", align="left")
	} else {
		HTML("Score plots are often used to look for patterns, groups or outliers in the data.", align="left")

		#Table of scores
		scores<-scores(PLSmodel)

		#Number of observations
		noobs<-dim(PLSData)[1]

		#creating scores table
		tableScores <- matrix(NA, ncol=usercomp, nrow=noobs)
		k<-1
		for (i in 1:usercomp) {
			for (j in 1:noobs) {
				tableScores[j,i] = scores[k]
				k=k+1			
			}
		}
		rownames(tableScores)<-c(1:noobs)
		coltemp<-c(1:usercomp)
		coltemp<-paste("Componentivs_sp_ivs",coltemp, sep = "")
		colnames(tableScores)<-coltemp
		tableScores<-data.frame(tableScores)

		#plotting the final table
		scatterPlotx <- sub(".html", "scatterPlotx.jpg", htmlFile)
		jpeg(scatterPlotx,width = jpegwidth, height = jpegheight, quality = 100)

		plotFilepdf2 <- sub(".html", "scatterPlotx.pdf", htmlFile)
		dev.control("enable") 

#===================================================================================================================
#Matrixplot parameters and dataset setup
#===================================================================================================================
		matrixdata <- data.frame(xvarrr_IVS = NA, yvarrr_IVS = NA, firstcatvarrr_IVS = NA, secondcatvarrr_IVS=NA)
	
		for (i in 1:usercomp) {
			for (j in 1:usercomp) {
				if (i != j) {
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
		w_Gr_jitscat <- 0
		h_Gr_jitscat <- 0
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
		graphdatax <- subset(graphdata, catvartest != "N")
		graphdata<-graphdatax

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
#Variability explained by the components
#===================================================================================================================
	expvar<-data.frame(explvar(PLSmodel))
	expvar$explvar.PLSmodel. = expvar$explvar.PLSmodel. / 100
	expvar$explvar.PLSmodel.<- format(round(expvar$explvar.PLSmodel.,2),nsmall=2)

	iden<-c(" ")
	expvar<- rbind(iden,expvar)
	colnames(expvar)<- c("Variance proportion")
	rownames(expvar)<- c("Component", 1:as.numeric(usercomp))

	HTML.title("Proportion of the variability explained by the components", HR=2, align="left")
	HTML(expvar, classfirstline="second", align="left", row.names = "FALSE")

#===================================================================================================================
#Factor loadings
#===================================================================================================================
#Table of scores
	loadings<-loadings(PLSmodel)[,1:usercomp]

	if (usercomp == 1) {
		loadings <- t(loadings)
	}

	loadings2<-melt(loadings)
	loadings2$X1<-sub("X_IVS_X", " ", loadings2$X1, fixed = TRUE)
	loadings2$X1<-namereplace(loadings2$X1)

	#Data for plot
	loadings3<-loadings2
	if (usercomp == 1) {
		colnames(loadings3)<-c("Component", "Variable level", "Loading")
	} else {
		colnames(loadings3)<-c("Variable level", "Component", "Loading")
	}

	#Tidy up component levels
	loadings3$Component<-sub("Comp", " ", loadings3$Component, fixed = TRUE)
	loadings2$X2<-sub("Comp", "Component ", loadings2$X2, fixed = TRUE)

	HTML.title("Table of the component loadings for each continuous predictor", HR=2, align="left")
	HTML(loadings3, classfirstline="second", align="left", row.names = "FALSE")

	if (usercomp > 1) {
		HTML.title("Plot of the loadings for each continuous predictor", HR=2, align="left")

		#plotting the final table
		ncscatterplot3qq <- sub(".html", "ncscatterplot3qq.jpg", htmlFile)
		jpeg(ncscatterplot3qq,width = jpegwidth, height = jpegheight, quality = 100)

		#STB July2013
		plotFilepdf3qq <- sub(".html", "ncscatterplot3qq.pdf", htmlFile)
		dev.control("enable") 

		#GGPLOT OPTIONS
		graphdata<-data.frame(loadings2)

		#Colour range for individual animals on case profiles plot
		graphdata<-graphdata[order(graphdata$X2, graphdata$X1), ]
		temp<-IVS_F_cpplot_colour("X2")
		Gr_palette_A <- temp$Gr_palette_A

		#plot(PLSmodel, "loadings", comps = 1:2, legendpos = "topleft")
		PLSloadingsplot()

		void<-HTMLInsertGraph(GraphFileName=sub("[A-Z0-9a-z,:,\\\\]*App_Data[\\\\]","", ncscatterplot3qq), Align="centre")

		if (pdfout=="Y") {
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
#===================================================================================================================
#end of if statement about user selecting no of components
}

#End of PLS statement
}

#===================================================================================================================
#References
#===================================================================================================================
Ref_list<-R_refs()

#HTML.title("Statistical references", HR=2, align="left")
#HTML(Ref_list$BateClark_ref, align="left")

HTML.title("R references", HR=2, align="left")
HTML(Ref_list$R_ref ,  align="left")
HTML(Ref_list$R2HTML_ref,  align="left")

if (analysisType == "Cluster" ) {
	HTML(Ref_list$cluster_ref,  align="left")
	HTML(Ref_list$ggdendro_ref,  align="left")
}

if (analysisType == "PLS" ) {
	HTML(Ref_list$mixOmics_ref, align="left")
}

HTML(Ref_list$GGally_ref, align="left")
HTML(Ref_list$RColorBrewers_ref,  align="left")
HTML(Ref_list$GGPLot2_ref,  align="left")
HTML(Ref_list$ggrepel_ref,  align="left")
HTML(Ref_list$reshape_ref,  align="left")
HTML(Ref_list$plyr_ref,  align="left")
HTML(Ref_list$scales_ref,  align="left")
HTML(Ref_list$PROTO_ref,  align="left")

#===================================================================================================================
#Show dataset
#===================================================================================================================
if (showdataset=="Y")
{
	statdata$catfact<-NULL	
	if (caseid_IVS_ == "NULL") {
		statdata$Cases <- statdata$caseid_IVS_

	}

	statdata$catPred_ <-NULL	
	statdata$caseid_IVS_<-NULL

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

HTML(paste("Response variables: ", responses_IVS_, sep=""), align="left")
if (transformation != "None") {
	HTML(paste("Responses transformation: ", transformation, sep=""), align="left")
}

if (catPred_ != "NULL") {
	HTML(paste("Categorical predictors: ", catPred_, sep=""), align="left")
}

if (contPred_ != "NULL") {
	HTML(paste("Continuous predictors: ", contPred_, sep=""), align="left")
}

if (caseid_IVS_ != "NULL") {
	HTML(paste("Case ID variable: ", caseid_IVS_, sep=""), align="left")
}

HTML(paste("Analysis performed: ", analysisType, sep=""), align="left")

if (noOfClusters != "NULL" && analysisType == "Cluster") {
	HTML(paste("Number of clusters: ", noOfClusters, sep=""), align="left")
}

if (distanceMethod != "NULL" && analysisType == "Cluster") {
	HTML(paste("Distance method: ", distanceMethod, sep=""), align="left")
}

if (agglomerationMethod != "NULL" && analysisType == "Cluster") {
	HTML(paste("Agglomeration method: ", agglomerationMethod, sep=""), align="left")
}

if (plotLabels != "NULL") {
	HTML(paste("Plot labels variable: ", plotLabels, sep=""), align="left")
}

if (noOfComponents != "NULL" && analysisType == "PLS") {
	HTML(paste("Number of components: ", noOfComponents, sep=""), align="left")
}

if (PCA_center != "NULL" && analysisType == "PCA") {
	HTML(paste("Response centering transformation: ", PCA_center, sep=""), align="left")
}

if (PCA_scale != "NULL" && analysisType == "PCA") {
	HTML(paste("Response scale transformation: ", PCA_scale, sep=""), align="left")
}


