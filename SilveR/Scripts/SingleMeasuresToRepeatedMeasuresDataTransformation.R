#===================================================================================================================
#R Libraries

suppressWarnings(library(R2HTML))
suppressWarnings(library(reshape))
suppressWarnings(library(dplyr))
#===================================================================================================================
# retrieve args

Args <- commandArgs(TRUE)

#Read in arguments
statdata <- read.csv(Args[3], header = TRUE, sep = ",")

responses <- Args[4]
subjectFactor <- Args[5]
includeAllVariables <- Args[6]
selectedVariables <- Args[7]
responseNameID <- Args[8]
repeatedFactorNameID <- Args[9]
subjectFactorNameID <- Args[10]

#Print args
if (Diplayargs == "Y"){
	print(Args)
}

#===================================================================================================================
#Setup the html file and associated css file
htmlFile <- sub(".csv", ".html", Args[3]);
#determine the file name of the html file
HTMLSetFile(file = htmlFile)
.HTML.file = htmlFile

#===================================================================================================================
#Variable selection
#===================================================================================================================
#Sort out response list
resps <-strsplit(responses, ",")
resplistx <- c("")
for(i in 1:length(resps[[1]]))  { 
	resplistx [length(resplistx )+1]=(resps[[1]][i]) 
}
resplist <- resplistx[-1]
noresps<-length(resplist)

#Sort out variables list
if (selectedVariables != "NULL") {
	vars <-strsplit(selectedVariables, ",")
	varslistx <- c("")
	for(i in 1:length(vars[[1]]))  { 
		varslistx [length(varslistx )+1]=(vars[[1]][i]) 
	}
	varslist <- varslistx[-1]
	novars<-length(varslist)
}

#Generate list of variables excluding the responses
fullnames<- colnames(statdata)
fullnames<-setdiff(fullnames, resplist)

#Sort out subject factor
statdata$Subject.factor<- c(1:dim(statdata)[1])
if (subjectFactor != "NULL") {
  statdata$Subject.factor<- eval(parse(text = paste("statdata$",subjectFactor)))
}

#Repeated factor name
repeatedFactorName <- "Repeated.factor"
if (repeatedFactorNameID != "NULL") {
	repeatedFactorName <- repeatedFactorNameID
}

#Subject factor name
subjectFactorName <- "Subject.factor"
if (subjectFactor != "NULL") {
	subjectFactorName <- subjectFactor
}
if (subjectFactorNameID != "NULL") {
	subjectFactorName <- subjectFactorNameID
}

#Response name
responseName <- "Response"
if (responseNameID != "NULL") {
	responseName <- responseNameID
}
#===================================================================================================================
#Titles and description
#===================================================================================================================
#Output HTML header
Title <-paste(branding, " Single Measures to Repeated Measures Dataset Transformation", sep="")
HTML.title(Title, HR = 1, align = "left")

#Software development version warning
if (Betawarn == "Y") {
  HTML.title("Warning", HR=2, align="left")
  HTML(BetaMessage, align="left")
}

#===================================================================================================================
#Generate variables list 
#===================================================================================================================
#Generate a list of variables when all variables included
fullnames<-colnames(statdata)
for (i in 1:noresps) {
	fullnames<- fullnames[fullnames != resplist[i]]
}
fullnames<- fullnames[fullnames != "Subject.factor"]

#Generate variable when only subject factor selected
if (includeAllVariables !="Y" && subjectFactor != "NULL") {
  fullnames <- c(subjectFactor)
}
if (includeAllVariables !="Y" && subjectFactor == "NULL") {
  fullnames <- c("Subject.factor")
}

if (selectedVariables !="NULL") {
	for (i in 1:novars) {
		fullnames<-cbind(fullnames, varslist[i])
	}
}

#===================================================================================================================
#Transformation 
#===================================================================================================================
RMlong <- melt(statdata, 
               id.vars=c(fullnames), 
               measure.vars=c(resplist),
               variable_name="Repeated.factor",
               na.rm=TRUE
               )

#Rename value variable
RMlong$Response <- RMlong$value
RMlong$value <-NULL

#Replace any dodgy characters 
Repeated.factor<-namereplaceGSUB(RMlong$Repeated.factor)
RMlong$Repeated.factor<-NULL
RMlong<-cbind(RMlong, Repeated.factor)

#Rename variable names
if (responseNameID != "NULL") {
	colnames(RMlong)[colnames(RMlong) == "Response"] <- responseNameID
}
if (repeatedFactorNameID != "NULL") {
	colnames(RMlong)[colnames(RMlong) == "Repeated.factor"] <- repeatedFactorNameID
}
if (subjectFactorNameID != "NULL") {
	colnames(RMlong)[colnames(RMlong) == subjectFactor] <- subjectFactorNameID
}

#===================================================================================================================
#The original dataset 
#===================================================================================================================
#Output HTML header
Title1 <-c("The original dataset in single measures format")
HTML.title(Title1, HR = 1, align = "left")

Desc1 = paste("The responses are stored in variables ", responses, ".", sep = "")
if (subjectFactor != "NULL") {
	Desc1 = paste(Desc1, " The subject factor levels are stored in variable ")
	Desc1 = paste(Desc1, subjectFactor, ". ", sep = "")
}
HTML(Desc1, align="left")

statdata2<-statdata
statdata2$Subject.factor <- NULL
tempnames<-colnames(statdata2)
ID<- c(1:dim(statdata2)[1])
statdata2<-cbind(ID, statdata2)
colnames(statdata2)<-c("Observation No.", tempnames)

HTML(statdata2, classfirstline = "second", align = "left", row.names = "FALSE")

#===================================================================================================================
#The transformed dataset 
#===================================================================================================================
#Output HTML header
Title2 <-c("The transformed dataset in repeated measures format")
HTML.title(Title2, HR = 1, align = "left")

Desc = c("The subject factor levels are stored in variable ")
Desc = paste(Desc, subjectFactorName, ". ", sep = "")
Desc = paste(Desc, "The repeated factor levels are stored in variable ", repeatedFactorName, ". ", sep = "")
Desc = paste(Desc, "The responses are stored in variable ", responseName, ".", sep = "")
HTML(Desc, align="left")

RMlong2<-RMlong
tempnames2<-colnames(RMlong2)
tempnames2<-namereplaceGSUB(tempnames2)
ID2<- c(1:dim(RMlong2)[1])
RMlong2<-cbind(ID2, RMlong2)
colnames(RMlong2)<-c("Observation No.", tempnames2)
HTML(RMlong2, classfirstline = "second", align = "left", row.names = "FALSE")
HTML(c("This dataset has now been stored alongside all your other imported datasets and is ready for analysis in the Repeated Measures Parametric Analysis module."), align="left")

#===================================================================================================================
#Generate dataset
#===================================================================================================================
RepeatedMeasuresData <- sub(".csv", "RepeatedMeasuresData.csv",  Args[3])
write.csv(RMlong2, file = sub("[A-Z0-9a-z,:,\\\\]*App_Data[\\\\]","", RepeatedMeasuresData), row.names=FALSE)


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
HTML(reference("reshape"))
HTML(reference("dplyr"))