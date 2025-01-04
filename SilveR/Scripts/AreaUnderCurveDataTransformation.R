#===================================================================================================================
#R Libraries

suppressWarnings(library(R2HTML))
suppressWarnings(library(dplyr))
#===================================================================================================================
# retrieve args

Args <- commandArgs(TRUE)

#Read in arguments
statdata <- read.csv(Args[3], header = TRUE, sep = ",")

DataFormatType <- tolower(Args[4])
Response <- Args[5]
SubjectFactor <- Args[6]
Time <- Args[7]

ResponseList <- Args[8]
NumericalTimepoints <- Args[9]

includeAllVariables <- Args[10]
selectedVariables <- Args[11]
aucOutputType <- tolower(Args[12])

#Print args
if (Diplayargs == "Y"){
	print(Args)
}

#Graphoical parameters
XLimLow <- "NULL"
XLimHigh <- "NULL"
YLimLow <- "NULL"
YLimHigh <- "NULL"
#===================================================================================================================
#Setup the html file and associated css file
htmlFile <- sub(".csv", ".html", Args[3]);
#determine the file name of the html file
HTMLSetFile(file = htmlFile)
.HTML.file = htmlFile

#===================================================================================================================
#Variable selection
#===================================================================================================================

#Function to carry out the Trapezium rule
trapezium<-function(x,y) sum(diff(x)*(y[-1]+y[-length(y)]))/2

#Sort out response list
if (selectedVariables != "NULL") {
	respvars <-strsplit(selectedVariables, ",")
	respvarlistx <- c("")
	for(i in 1:length(respvars[[1]]))  { 
		respvarlistx [length(respvarlistx )+1]=(respvars[[1]][i]) 
	}
	respvarlist <- respvarlistx[-1]
	norespvars<-length(respvarlist)
}

#Sort out timepoint list
if (DataFormatType == "singlemeasuresformat") {
	times <-strsplit(NumericalTimepoints, ",")
	timeslistx <- c("")
	for(i in 1:length(times[[1]]))  { 
		timeslistx [length(timeslistx )+1]=(times[[1]][i]) 
	}
	timeslist <- timeslistx[-1]
	notimes<-length(timeslist)
}

#Sort out variables list
if (DataFormatType == "singlemeasuresformat") {
	resps <-strsplit(ResponseList, ",")
	respslistx <- c("")
	for(i in 1:length(resps[[1]]))  { 
		respslistx [length(respslistx )+1]=(resps[[1]][i]) 
	}
	respslist <- respslistx[-1]
	noresps<-length(respslist)
}


#===================================================================================================================
#Titles and description
#===================================================================================================================
#Output HTML header
Title <-paste(branding, " AUC Transformation", sep="")
HTML.title(Title, HR = 1, align = "left")

#Software development version warning
if (Betawarn == "Y") {
  HTML.title("Warning", HR=2, align="left")
  HTML(BetaMessage, align="left")
}

#===================================================================================================================
#===================================================================================================================
#Align datasets
#===================================================================================================================
#===================================================================================================================
if (DataFormatType == "repeatedmeasuresformat") {
	finaldataset<-statdata
	finaldataset$SubjectFactorxxx<-eval(parse(text = paste("finaldataset$",SubjectFactor)))
	finaldataset$Timexxx<-eval(parse(text = paste("finaldataset$",Time)))
	finaldataset$Responsexxx<-eval(parse(text = paste("finaldataset$",Response)))
}

if (DataFormatType == "singlemeasuresformat") {
	tempdataset<-statdata
	tempdataset$SubjectFactorxxx<- c(1:dim(tempdataset)[1])

	finaldataset<-tempdataset
	finaldataset$Timexxx <- timeslist[1]
	finaldataset$Responsexxx <- eval(parse(text = paste("finaldataset$",respslist[1])))

	for (i in 2:noresps) {
		tempdata<-tempdataset
		tempdata$Timexxx <- timeslist[i]
		tempdata$Responsexxx <- eval(parse(text = paste("tempdata$",respslist[i])))
		finaldataset<- rbind (finaldataset, tempdata)
	}
}
finaldataset<-data.frame(finaldataset[!is.na(finaldataset$Responsexxx), ])

#===================================================================================================================
#Testing the number of timepoints
#===================================================================================================================

#RM format
index<-length(unique(levels(as.factor(finaldataset$SubjectFactorxxx))))

for (i in 1:index) {
	#Break down dataset into individual animals
	sub<-subset(finaldataset, finaldataset$SubjectFactorxxx == unique(levels(as.factor(finaldataset$SubjectFactorxxx)))[i])

	if (aucOutputType!= "aucfromtime0") {
		if(dim(sub)[1] == 1) {
	  		HTML.title("Warning", HR=2, align="left")
			Desc2 = c("For at least one of the subjects there is only a single observation present and hence the AUC cannot be calculated. Please remove these subjects from the dataset prior to calculating the AUC.")
			HTML(Desc2, align="left")
			quit()
		}
	}
	if (aucOutputType== "aucfromtime0") {
		if(dim(sub)[1] == 0) {
	  		HTML.title("Warning", HR=2, align="left")
			Desc2 = c("For at least one of the subjects there is only a single observation present and hence the AUC cannot be calculated. Please remove these subjects from the dataset prior to calculating the AUC.")
			HTML(Desc2, align="left")
			quit()
		}
	}
}
#===================================================================================================================
#Description
#===================================================================================================================
#Output HTML header
Title2 <-c("Input variables")
HTML.title(Title2, HR = 1, align = "left")

if (DataFormatType == "repeatedmeasuresformat") {
	Desc1 = paste("The responses are stored in the variable ", Response, ", the subjects are stored in the variable ", SubjectFactor, " and the time points are stored in variable ", Time , ".", sep = "")
	HTML(Desc1, align="left")
}

if (DataFormatType == "singlemeasuresformat") {
	Desc1 = paste("The responses are stored in variables ", ResponseList, " and time points are ", NumericalTimepoints , ".", sep = "")
	HTML(Desc1, align="left")
}

#Time for 0
if (aucOutputType== "aucfromtime0") {
	Desc2 = c("The area under curves (AUC) have been calculated assuming that the time course starts at 0.")
	HTML(Desc2, align="left")
}
#AUC from initial timepoint (not centered at 0)
if (aucOutputType== "aucfrominitialtimepoint") {
	Desc2 = c("The area under curves (AUC) have been calculated assuming that the time course starts from the initial timepoint for each individual.")
	HTML(Desc2, align="left")
}
#Change from baseline AUC
if (aucOutputType== "aucforchangefrombaseline") {
	Desc2 = c("The area under curves (AUC) have been calculated using the change from baseline responses.")
	HTML(Desc2, align="left")
}



#===================================================================================================================
#Generate AUC
#===================================================================================================================
#Make time variable numeric
finaldataset$Time_Adjxxx = finaldataset$Timexxx
finaldataset$Time_Adjxxxx <- as.numeric(finaldataset$Time_Adjxxx)
finaldataset$Time_Adjxxx <- NULL
finaldataset$Time_Adjxxx <- finaldataset$Time_Adjxxxx
finaldataset$Time_Adjxxxx <- NULL


#Order dataset by Subject
finaldataset2<-finaldataset[order(finaldataset$SubjectFactorxxx, finaldataset$Time_Adjxxx),]

#Dataset for additional variables dataset
extradata<-data.frame(matrix(nrow=1,ncol=dim(finaldataset2)[2]))
colnames(extradata)<-colnames(finaldataset2)

#Variables required for table below
index<-length(unique(levels(as.factor(finaldataset$SubjectFactorxxx))))
test<-data.frame(nrow=index,ncol=2)

#Run code for each animal separately
for (i in 1:index) {

	#Break down dataset into individual animals
	sub<-subset(finaldataset2, finaldataset2$SubjectFactorxxx == unique(levels(as.factor(finaldataset2$SubjectFactorxxx)))[i])
	sub2<-sub
	sub$Response_Adjxxx = sub$Responsexxx

	#AUC from timepoint 0
	if (aucOutputType== "aucfromtime0") {
		temp <- sub[1:1,]
		temp$Response_Adjxxx <- 0
		temp$Time_Adjxxx <- 0
		sub <- rbind(temp, sub)
	}

	#AUC from initial timepoint (not centered at 0)
	if (aucOutputType== "aucfrominitialtimepoint") {
		sub$Time_Adjxxx = sub$Time_Adjxxx-min(sub$Time_Adjxxx, na.rm=TRUE)
	}

	#Change from baseline AUC
	if (aucOutputType== "aucforchangefrombaseline") {
		sub$Time_Adjxxx = sub$Time_Adjxxx-min(sub$Time_Adjxxx, na.rm=TRUE)
		minresp <- sub$Responsexxx[1]
		sub$Response_Adjxxx = sub$Responsexxx - minresp
	}

	#Calculate AUC for each animal
	AUC<-trapezium(sub$Time_Adjxxx,sub$Response_Adjxxx)

	#Create table for output
	test[i,1]=sub$SubjectFactorxxx[1]
	test[i,2]=format(round(AUC, 5), nsmall=5, scientific=FALSE)

	#Generate the additional dataset rows
	subdata2 <- sub2[1:1,]
	extradata<-rbind(extradata, subdata2)
}



#===================================================================================================================
# One-categorised case profiles plot
#===================================================================================================================
Title2 <-c("Case profiles plot including AUC")
HTML.title(Title2, HR = 1, align = "left")

#Global variables format
graphdata<-finaldataset2
#graphdata$Time_IVS<-as.factor(graphdata$Time_Adjxxx)
graphdata$Time_IVS<-graphdata$Time_Adjxxx
graphdata$Subject_IVS<-graphdata$SubjectFactorxxx
graphdata$l_l<-graphdata$SubjectFactorxxx
graphdata$yvarrr_IVS <- graphdata$Responsexxx
YAxisTitle <- "Response"
XAxisTitle <- "Time"
MainTitle2 <- " "
temp<-IVS_F_cpplot_colour("SubjectFactorxxx")
Gr_palette_A <- temp$Gr_palette_A
Gr_line <- temp$Gr_line
Gr_fill <-temp$Gr_fill
ReferenceLine <- "NULL"
Animal_IVS<- "SubjectFactorxxx"
	

#Adding in the 0 timepoint
if (aucOutputType== "aucfromtime0") {
	for (i in 1:index) {
		temp <- graphdata[1:1,]
		temp$Subject_IVS <- levels(as.factor(graphdata$Subject_IVS))[i]
		temp$l_l <- levels(as.factor(graphdata$Subject_IVS))[i]
		temp$yvarrr_IVS <- 0
		temp$Time_IVS <- 0
		graphdata <- rbind(temp, graphdata)
	}
}

#Change from baseline response
if (aucOutputType== "aucforchangefrombaseline") {
	graphdata2<-data.frame(matrix(nrow=1,ncol=dim(graphdata)[2]))
	colnames(graphdata2)<-colnames(graphdata)
	YAxisTitle <- "Change from baseline"

	for (i in 1:index) {
		sub<-subset(graphdata, graphdata$Subject_IVS == unique(levels(as.factor(graphdata$Subject_IVS)))[i])
		sub$Time_IVS = sub$Time_IVS-min(sub$Time_IVS, na.rm=TRUE)
		minresp <- sub$yvarrr_IVS[1]
		sub$yvarrr_IVS = sub$yvarrr_IVS - minresp
		graphdata2 <- rbind(sub, graphdata2)
	}
	graphdata<-graphdata2
}



#Plot device setup
nccaseplot <- sub(".html", "nccaseplot.png", htmlFile)
png(nccaseplot,width = jpegwidth, height = jpegheight, units="in", res=PlotResolution)

#STB July2013
plotFilepdf23 <- sub(".html", "nccaseplot.pdf", htmlFile)
dev.control("enable") 

#GGPLOT2 code
AUC_CPP()

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

if (aucOutputType== "aucforchangefrombaseline") {
	comment <- c("Note that in the AUC calculation, any area corresponding to a negative change from baseline will be subtracted from the AUC total.")
	HTML(comment, align="left")
}

#===================================================================================================================
#Generate printout from repeated measures data
#===================================================================================================================
if (DataFormatType == "repeatedmeasuresformat") {
	ID<- c(1:index)
	test2<-cbind(ID, test)

	namez<- c("Observation No.", SubjectFactor,"AUC")
	colnames(test2) <-namez

	extradata<-extradata[-1,]
	extradata2 <- select(extradata, -c(SubjectFactorxxx, Timexxx, Responsexxx, Time_Adjxxx))

	#Add in addiitonal variables
	extradata3<-extradata2

	if (selectedVariables != "NULL") {
	for (i in 1:norespvars) {
		extradata3 <- select(extradata2, c(respvarlist))
		}
	}

	if (includeAllVariables == "Y") {
		extradata3<- select(extradata2, -c(SubjectFactor, Time, Response))
	}
	names<-colnames(extradata3)

	if (includeAllVariables == "Y" || selectedVariables != "NULL") {
		test2<-cbind(test2, extradata3)
		colnames(test2) <- c(namez, names)
	}

	title<-c("Table of AUC responses")
	HTML.title(title, HR=1, align="left")
	HTML(test2, classfirstline="second", align="left", row.names = "FALSE")

	Desc3 = c("The AUC responses are calculated using the trapezoid approach.")
	HTML(Desc3, align="left")

	if (includeAllVariables == "Y" || selectedVariables != "NULL") {
		comment <- c("Note that for the additional variables included in the AUC dataset, the values quoted (for each subject) correspond to the level at the initial timepoint.")
		HTML(comment, align="left")
	}
}



#===================================================================================================================
#Generate printout from single measures data
#===================================================================================================================
if (DataFormatType == "singlemeasuresformat") {
	ID<- c(1:index)
	test2<-cbind(ID, test)

	namez<- c("Observation No.", "Subject factor","AUC")
	colnames(test2) <-namez

	extradata<-extradata[-1,]
	extradata2 <- select(extradata, -c(SubjectFactorxxx, Timexxx, Responsexxx, Time_Adjxxx))

	#Adda additional variables
	extradata3<-extradata2
	for (i in 1:noresps) {
		extradata3 <- select(extradata3, -c(respslist[i]))
	}

	if (selectedVariables != "NULL") {
	for (i in 1:norespvars) {
		extradata3 <- select(extradata2, c(respvarlist))
		}
	}

#	if (includeAllVariables != "NULL") {
#		for (i in 1:noresps) {
#			extradata3 <- select(extradata3, -c(respslist[i]))
#		}
#	}
	names<-colnames(extradata3)

	if (includeAllVariables == "Y" || selectedVariables != "NULL") {
		test2<-cbind(test2, extradata3)
		colnames(test2) <- c(namez, names)
	}

	title<-c("Table of AUC responses")
	HTML.title(title, HR=1, align="left")
	HTML(test2, classfirstline="second", align="left", row.names = "FALSE")

	Desc3 = c("The AUC responses are calculated using the trapezoid approach.")
	HTML(Desc3, align="left")
}

#===================================================================================================================
#References
#===================================================================================================================
Ref_list<-R_refs()

HTML.title("References", HR=2, align="left")
HTML(Ref_list$IVS_ref, align="left")
#HTML(Ref_list$BateClark_ref, align="left")

HTML.title("R references", HR=4, align="left")
HTML(Ref_list$R_ref ,  align="left")
HTML(reference("R2HTML"))
HTML(reference("dplyr"))

#===================================================================================================================
#Print of input data
#===================================================================================================================
if (DataFormatType == "repeatedmeasuresformat") {
	Title1 <-c("The original dataset in repeated measures format")
}
if (DataFormatType == "singlemeasuresformat") {
	Title1 <-c("The original dataset in single measures format")
}

#statdatax<-na.omit(statdata)
HTML.title(Title1, HR = 1, align = "left")
HTML(statdata, classfirstline = "second", align = "left")

#===================================================================================================================
#Generate dataset
#===================================================================================================================
AUCData <- sub(".csv", "AUCData.csv",  Args[3])
write.csv(test2, file = sub("[A-Z0-9a-z,:,\\\\]*App_Data[\\\\]","", AUCData), row.names=FALSE)



