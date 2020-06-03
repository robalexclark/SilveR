#===================================================================================================================
#R Libraries

suppressWarnings(library(R2HTML))
suppressWarnings(library(mvtnorm))

#===================================================================================================================
# retrieve args
Args <- commandArgs(TRUE)

#Read in data
statdata <- read.csv(Args[3], header=TRUE, sep=",")

#Copy Args
DoseResponseType <- Args[4]
ResponseVar <- Args[5]
ResponseTransform <- Args[6]
DoseVar <- Args[7]
Offsetz <- Args[8]
DoseTransform <- Args[9]
QCResponse <- Args[10]
QCDose <- Args[11]
Samples <- Args[12]
MinCoeff <- Args[13]
MaxCoeff <- Args[14]
SlopeCoeff <- Args[15]
ECIDCoeff <- Args[16]
MinStartValue <- Args[17]
MaxStartValue <- Args[18]
SlopeStartValue <- Args[19]
ECIDStartValue <- Args[20]
Equation <- Args[21]
StartValues <- Args[22]
EquationResponse <- Args[23]
EquationDose <- Args[24]

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
ReferenceLine <- "NULL"

#Removing illegal characters

if (DoseResponseType == "FourParameter") {
	YAxisTitle<-paste(ResponseVar, " \n", sep = "")
	XAxisTitle<-DoseVar
}

if (DoseResponseType == "Equation") {
	YAxisTitle<-EquationResponse
	XAxisTitle<-EquationDose
}

#replace illegal characters in variable names
for (i in 1:10) {
	YAxisTitle<-namereplace(YAxisTitle) 
	XAxisTitle<-namereplace(XAxisTitle)
}

#===================================================================================================================
#===================================================================================================================
#Analysis based on a user equation
#===================================================================================================================
#===================================================================================================================
#===================================================================================================================
if (DoseResponseType == "Equation") {
	#Output HTML header
	Title <-paste(branding, " Non-Linear Regression Analysis", sep="")
	HTML.title(Title, HR = 1, align = "left")

	#Software developement version warning
	if (Betawarn == "Y") {
		HTML.title("Warning", HR=2, align="left")
		HTML(BetaMessage, align="left")
	}

	#Titles and description
	HTML.title("Variable selection", HR=2, align="left")
	add<-paste(c("The  "), EquationResponse, " response is currently being analysed using the user defined equation in the Dose-response and Non-Linear Regression Analysis module. ", sep="")
	HTML(add, align="left")

	#STB October 2013 - adding a warnign message if no x variable defined in the user equation
	Equationx<-c(Equation)
	if ( grepl("x",Equationx, fixed=TRUE, ignore.case=FALSE) == FALSE) {
		HTML.title("Warning", HR=2, align="left")
		HTML("The equation entered is not of the form y=f(x). The equation must contain a (lower case) x variable for the analysis to proceed.", align="left")
	}

	#Dose fit equation
	statdata$x = eval(parse(text = paste("statdata$", EquationDose)))
	statdata$y = eval(parse(text = paste("statdata$", EquationResponse )))
	Equation2 <- eval(parse(text = paste("y~", Equation)))

	#Separate out the start values
	#STB Jan 2016 remove trailing spaces
	trim <- function (x) gsub("^\\s+|\\s+$", "", x)
	tempChanges <-strsplit(StartValues, ",")

	txtexpectedChanges <- c("")
	for(i in 1:length(tempChanges[[1]]))  { 
		txtexpectedChanges [length(txtexpectedChanges)+1]=(tempChanges[[1]][i]) 
	}
	tabs<-matrix(nrow=(length(txtexpectedChanges)-1), ncol=2)

	for (i in 2:length(txtexpectedChanges)) {
		Changes <-strsplit(txtexpectedChanges[i], "=")
		txtChanges <- c("")
		for(j in 1:length(Changes[[1]])) { 
			txtChanges [length(txtChanges)+1]=(Changes[[1]][j]) 
		}

		#STB Jan 2016 - add trim command in
		tabs[(i-1),1]=trim(txtChanges[2])
		tabs[(i-1),2]=as.numeric(txtChanges[3])*1
	}

	#Setting up the list of start values
	nameparas<-c()
	paras<-c()
	index<-1
	for (i in 1:(length(txtexpectedChanges))-1) {
		paras[i]=as.numeric(tabs[i,2])
		nameparas[i]=tabs[i,1]
	}
	names(paras)<-nameparas
	dosefit<-nls(Equation2, start=paras, data=statdata)
	#STB Sept 2011 CC26
	if (min(statdata$x, na.rm=TRUE) > 0) {
		xmin<-min(statdata$x, na.rm=TRUE)
	} else {
		xmin<-min(statdata$x, na.rm=TRUE)
	}
	if (max(statdata$x, na.rm=TRUE) > 0) {
		xmax<-max(statdata$x, na.rm=TRUE)
	} else {
		xmax<-max(statdata$x, na.rm=TRUE)
	}
	if (min(statdata$y, na.rm=TRUE) > 0) {
		ymino<-min(statdata$y, na.rm=TRUE)
	} else {
		ymino<-min(statdata$y, na.rm=TRUE)
	}
	if (max(statdata$y, na.rm=TRUE) > 0) {
		ymaxo<-max(statdata$y, na.rm=TRUE)
	} else {
		ymaxo<-max(statdata$y, na.rm=TRUE)
	}
	
	av<-seq(xmin,xmax,0.01)
	bv<-predict(dosefit,list(x =av))

	if (min(bv, na.rm=TRUE) > 0) {
		yminp<-min(bv, na.rm=TRUE)
	} else {
		yminp<-min(bv, na.rm=TRUE)
	}
	if (max(bv, na.rm=TRUE) > 0) {
		ymaxp<-max(bv, na.rm=TRUE)
	} else  {
		ymaxp<-max(bv, na.rm=TRUE)
	}

	if (ymino < yminp) {
		ymin <- ymino
	} else {
		ymin <-yminp
	}
	if (ymaxo > ymaxp) {
		ymax <- ymaxo
	} else {
		ymax <-ymaxp
	}


	#Table of parameter estimates
	table<-summary(dosefit)$parameters
	tablen<-length(unique(rownames(table)))
	tabz<-matrix(nrow=tablen, ncol=5)

	for (i in 1:tablen) {
		#STB Dec 2011 Formatting to 3dp
		tabz[i,1]=rownames(table)[i]
	}
	for (i in 1:tablen) {
		#STB Dec 2011 Formatting to 3dp
		tabz[i,2]=format(round(table[i,1], 3), nsmall=3, scientific=FALSE)
	}
	for (i in 1:tablen) {
		tabz[i,3]=format(round(table[i,2], 3), nsmall=3, scientific=FALSE)
	}
	for (i in 1:tablen) {
		tabz[i,4]=format(round(table[i,3], 2), nsmall=2, scientific=FALSE)
	}
	for (i in 1:tablen) {
		tabz[i,5]=format(round(table[i,4], 4), nsmall=4, scientific=FALSE)
	}
	for (i in 1:tablen) {
		if (as.numeric(table[i,4])<0.0001) {
			#STB - March 2011 formating p-value <0.0001
			tabz[i,5]=format(round(0.0001, 4), nsmall=4, scientific=FALSE)
			tabz[i,5]<- paste("<",tabz[i,5])
		}
	}

	colnames(tabz)<-c("Parameter", "Estimate", "Std error", "t-value", "p-value")

	#DF test
	df<-length(na.omit(eval(parse(text = paste("statdata$", EquationResponse )))))-tablen
	if (df <= 4) {
		HTML.title("Warning", HR=2, align="left")
		HTML.title("Unfortunately the residual degrees of freedom are low (less than 5). This may make the estimation of the underlying variability, and hence the results of the statistical tests, unreliable. This can be caused by attempting to model too many parameters. We recommend you fix some of the parameters."  , align="left")
	}

	#Plotting the results
	statdata$conczzzz = eval(parse(text = paste("statdata$", EquationDose)))
	statdata$respzzzz = eval(parse(text = paste("statdata$", EquationResponse )))
	HTML.title("Scatterplot of observed data including the predicted fit", HR=2, align="left")

	scatterPlot <- sub(".html", "scatterPlot.png", htmlFile)
	png(scatterPlot,width = jpegwidth, height = jpegheight, units="in", res=PlotResolution)

	#STB July2013
	plotFilepdf <- sub(".html", "scatterPlot.pdf", htmlFile)
	dev.control("enable") 

#===================================================================================================================
#Graphical parameters
#===================================================================================================================
	graphdata$xaxisvarrr_ivs<-statdata$conczzzz
	graphdata$yaxisvarrr_ivs<-statdata$respzzzz
	
	xmax<-max(graphdata$xaxisvarrr_ivs, na.rm=TRUE)
	xmin<-min(graphdata$xaxisvarrr_ivs, na.rm=TRUE)
	av<-seq(xmin,xmax,0.01)
	bv<-predict(dosefit,list(x =av))

	#Creating final dataset
	Type <- c(1:length(bv))
	for (i in 1:length(bv)) {
		Type[i]="Prediction curve"
	}
	tempdata<-data.frame(av,bv, Type)
	colnames(tempdata)<-c("Dose", "Response", "Type")

	tempdata2<-graphdata[,c("xaxisvarrr_ivs", "yaxisvarrr_ivs")]

	#STB Feb 2016 - Replace length(tempdata2) with dim(tempdata2)[1]
	Type <- c(1:dim(tempdata2)[1])
	for (i in 1:dim(tempdata2)[1]) {
		Type[i]=" Assay standards"
	}

	tempdata2<-data.frame(tempdata2, Type)
	colnames(tempdata2)<-c("Dose", "Response", "Type")
	finaldata<-rbind(tempdata, tempdata2)

	#New colour palette for this plot
	Col_palette <- c("#E41A1C" ,"black")
	BW_palette <- c("white",  "black")

	#BandW - Line included
	if (bandw == "Y") {
		Gr_palette <-BW_palette
	} else {
		Gr_palette <-Col_palette	
	}
	Line_type <- Line_type_solid
	MainTitle2 <-""

	#GGPlot2 code
	Dose_Resp_ExQC()

#===================================================================================================================
	void <- HTMLInsertGraph(GraphFileName=sub("[A-Z0-9a-z,:,\\\\]*App_Data[\\\\]","", scatterPlot), Align="centre")

#STB July2013
	if (pdfout=="Y") {	
		pdf(file=sub("[A-Z0-9a-z,:,\\\\]*App_Data[\\\\]","", plotFilepdf), height = pdfheight, width = pdfwidth) 
		dev.set(2) 
		dev.copy(which=3) 
		dev.off(2)
		dev.off(3)
		pdfFile<-sub("[A-Z0-9a-z,:,\\\\]*App_Data[\\\\]","",plotFilepdf)
		linkToPdf <- paste ("<a href=\"",pdfFile,"\">Click here to view the PDF of the scatterplot</a>", sep = "")
		HTML(linkToPdf)
	}

	#Table of parameter estimates print
	HTML.title("Table of model parameters and summary statistics", HR=2, align="left")
	HTML(tabz, classfirstline="second", align="left", row.names = "FALSE")
}

#===================================================================================================================
#===================================================================================================================
#Analysis based on a 4-parameter logistic equation
#===================================================================================================================
#===================================================================================================================
if (DoseResponseType == "FourParameter") {

	#Output HTML header
	Title <-paste(branding, " Dose-response Analysis", sep="")
	HTML.title(Title, HR = 1, align = "left")

	#Software developement version warning
	if (Betawarn == "Y") {
		HTML.title("Warning", HR=2, align="left")
		HTML(BetaMessage, align="left")
	}

#===================================================================================================================
#Setting up variables
#===================================================================================================================
	# Setting up the offset parameter
	if(unique(eval(parse(text = paste("statdata$", DoseVar))))[1] == 0) {	
		if (Offsetz  == "NULL" && DoseTransform == "Log10") {
			offset <-(unique(eval(parse(text = paste("statdata$", DoseVar))))[2])/10
		} 
		if (Offsetz  == "NULL" && DoseTransform == "Loge") {
			offset <-(unique(eval(parse(text = paste("statdata$", DoseVar))))[2])/exp(1)
		} 
		if (Offsetz  != "NULL" ) {
			offset<- as.numeric(Offsetz)
		}
	} 
	if(unique(eval(parse(text = paste("statdata$", DoseVar))))[1] != 0) {
		if (Offsetz  == "NULL" && DoseTransform == "Log10") {
			offset <-(unique(eval(parse(text = paste("statdata$", DoseVar))))[1])/10
		}  
		if (Offsetz  == "NULL" && DoseTransform == "Loge") {
			offset <-(unique(eval(parse(text = paste("statdata$", DoseVar))))[1])/exp(1)
		} 
		if (Offsetz  != "NULL" ) {
			offset<- as.numeric(Offsetz)
		}
	} 

	# Setting up the fixed parameter
	if (MinCoeff !="NULL") {
		MinCoeffp <- as.numeric(MinCoeff)
	}
	if (MaxCoeff !="NULL") {
		MaxCoeffp <- as.numeric(MaxCoeff)
	}
	if (SlopeCoeff !="NULL") {
		SlopeCoeffp <- as.numeric(SlopeCoeff)
	}
	if (ECIDCoeff !="NULL" && DoseTransform == "Log10") {
		ECIDCoeffp <- log10(as.numeric(ECIDCoeff)+offset)
	} 
	if (ECIDCoeff !="NULL" && DoseTransform == "Loge") {
		ECIDCoeffp <- log(as.numeric(ECIDCoeff)+offset)
	}

	# Setting up the concentration parameters
	statdata$responsezzzz = eval(parse(text = paste("statdata$", ResponseVar)))

	if (DoseTransform == "Log10") {
		statdata$logconczzzz = log10(eval(parse(text = paste("statdata$", DoseVar)))+offset)
		statdata$conczzzz = eval(parse(text = paste("statdata$", DoseVar)))
	}  
	if (DoseTransform == "Loge") {
		statdata$logconczzzz = log(eval(parse(text = paste("statdata$", DoseVar)))+offset)
		statdata$conczzzz = eval(parse(text = paste("statdata$", DoseVar)))
	}

	#Setting up graph titles
	if (ResponseTransform == "None") {
		YAxisTitle <- YAxisTitle
	} else {
		YAxisTitle <- paste(ResponseTransform, " (", YAxisTitle, ")",  sep="")
	}

	if (DoseTransform == "Log10") {
	XAxisTitle <- paste(XAxisTitle, " (on the Log10 scale)",sep="")
	}
	if (DoseTransform == "Loge") {
	XAxisTitle <- paste(XAxisTitle, " (on the Loge scale)",sep="")
	}

	# Setting up the start parameters - need to check user defined options
	if (MinStartValue == "NULL") {
	minp<-min(unlist(lapply(split(eval(parse(text = paste("statdata$", ResponseVar))),statdata$logconczzzz),mean)), na.rm=TRUE)
	} else {
	minp<-MinStartValue
	}
	if (MaxStartValue == "NULL") {
	maxp<-max(unlist(lapply(split(eval(parse(text = paste("statdata$", ResponseVar))),statdata$logconczzzz),mean)), na.rm=TRUE)
	} else {
	maxp<-MaxStartValue
	}
	if (SlopeStartValue == "NULL") {
		slopep<-1
		if(maxp>minp) {	
			slopep=1
		} else 	{
		slopep=-1
		}
	} else {
		slopep<-SlopeStartValue
	}

	if (DoseTransform == "Log10" && ECIDStartValue != "NULL") {
		ed50p<-log10(as.numeric(ECIDStartValue))
	}  
	if (DoseTransform == "Loge" && ECIDStartValue != "NULL") {
		ed50p<-log(as.numeric(ECIDStartValue))	
	}   
	if (ECIDStartValue == "NULL") {
		temp<-sort(unique(statdata$logconczzzz))
	
	#STB Sept 2011 CC26
		ed50p<-mean(temp[-1], na.rm=TRUE)
	}

#===================================================================================================================
#Titles and description
#===================================================================================================================
	HTML.title("Response and dose variables", HR=2, align="left")

	add<-paste("The  ", ResponseVar, " response is currently being analysed by the Dose-response and Non-Linear Regression Analysis module", sep="")
	if(ResponseTransform != "None") {
		add<-paste(add, " and has been ", ResponseTransform, " transformed prior to analysis" , sep="")
		add<-paste(add, ResponseTransform, sep="")
	}
	add<-paste(add, ".", sep="")
	HTML(add, align="left")

	if(DoseTransform == "Log10") {
		add<-paste(c("The dose variable ("), DoseVar, ") has been Log10 transformed prior to analysis.",sep="")
	}  
	if(DoseTransform == "Loge") {
		add<-paste(c("The dose variable ("), DoseVar, ") has been Loge transformed prior to analysis.",sep="")
	}
	HTML(add, align="left")

#===================================================================================================================
#Fitting the model
#===================================================================================================================
	if        (MinCoeff == "NULL"&& MaxCoeff == "NULL"&& SlopeCoeff == "NULL"&& ECIDCoeff == "NULL"&& DoseTransform != "Loge") {dosefit<-nls(responsezzzz~D         + (A        -D        )/(1+10^((C         -logconczzzz)*B          )), start=list(A=maxp,B=slopep,C= ed50p,D=minp), data=statdata) 
	} else if (MinCoeff == "NULL"&& MaxCoeff == "NULL"&& SlopeCoeff == "NULL"&& ECIDCoeff == "NULL"&& DoseTransform == "Loge") {dosefit<-nls(responsezzzz~D         + (A        -D        )/(1+exp((C         -logconczzzz)*B          )), start=list(A=maxp,B=slopep,C= ed50p,D=minp), data=statdata) 
	} else if (MinCoeff == "NULL"&& MaxCoeff != "NULL"&& SlopeCoeff == "NULL"&& ECIDCoeff == "NULL"&& DoseTransform != "Loge") {dosefit<-nls(responsezzzz~D         + (MaxCoeffp-D        )/(1+10^((C         -logconczzzz)*B          )), start=list(B=slopep,C= ed50p,D=minp), data=statdata) 
	} else if (MinCoeff == "NULL"&& MaxCoeff != "NULL"&& SlopeCoeff == "NULL"&& ECIDCoeff == "NULL"&& DoseTransform == "Loge") {dosefit<-nls(responsezzzz~D         + (MaxCoeffp-D        )/(1+exp((C         -logconczzzz)*B          )), start=list(B=slopep,C= ed50p,D=minp), data=statdata) 
	} else if (MinCoeff == "NULL"&& MaxCoeff == "NULL"&& SlopeCoeff != "NULL"&& ECIDCoeff == "NULL"&& DoseTransform != "Loge") {dosefit<-nls(responsezzzz~D         + (A        -D        )/(1+10^((C         -logconczzzz)*SlopeCoeffp)), start=list(A=maxp,C= ed50p,D=minp), data=statdata) 
	} else if (MinCoeff == "NULL"&& MaxCoeff == "NULL"&& SlopeCoeff != "NULL"&& ECIDCoeff == "NULL"&& DoseTransform == "Loge") {dosefit<-nls(responsezzzz~D         + (A        -D        )/(1+exp((C         -logconczzzz)*SlopeCoeffp)), start=list(A=maxp,C= ed50p,D=minp), data=statdata) 
	} else if (MinCoeff == "NULL"&& MaxCoeff == "NULL"&& SlopeCoeff == "NULL"&& ECIDCoeff != "NULL"&& DoseTransform != "Loge") {dosefit<-nls(responsezzzz~D         + (A        -D        )/(1+10^((ECIDCoeffp-logconczzzz)*B          )), start=list(A=maxp,B=slopep,D=minp), data=statdata) 
	} else if (MinCoeff == "NULL"&& MaxCoeff == "NULL"&& SlopeCoeff == "NULL"&& ECIDCoeff != "NULL"&& DoseTransform == "Loge") {dosefit<-nls(responsezzzz~D         + (A        -D        )/(1+exp((ECIDCoeffp-logconczzzz)*B          )), start=list(A=maxp,B=slopep,D=minp), data=statdata) 
	} else if (MinCoeff != "NULL"&& MaxCoeff == "NULL"&& SlopeCoeff == "NULL"&& ECIDCoeff == "NULL"&& DoseTransform != "Loge") {dosefit<-nls(responsezzzz~MinCoeffp + (A        -MinCoeffp)/(1+10^((C         -logconczzzz)*B          )), start=list(A=maxp,B=slopep,C= ed50p), data=statdata) 
	} else if (MinCoeff != "NULL"&& MaxCoeff == "NULL"&& SlopeCoeff == "NULL"&& ECIDCoeff == "NULL"&& DoseTransform == "Loge") {dosefit<-nls(responsezzzz~MinCoeffp + (A        -MinCoeffp)/(1+exp((C         -logconczzzz)*B          )), start=list(A=maxp,B=slopep,C= ed50p), data=statdata) 
	} else if (MinCoeff == "NULL"&& MaxCoeff != "NULL"&& SlopeCoeff != "NULL"&& ECIDCoeff == "NULL"&& DoseTransform != "Loge") {dosefit<-nls(responsezzzz~D         + (MaxCoeffp-D        )/(1+10^((C         -logconczzzz)*SlopeCoeffp)), start=list(C= ed50p,D=minp), data=statdata) 
	} else if (MinCoeff == "NULL"&& MaxCoeff != "NULL"&& SlopeCoeff != "NULL"&& ECIDCoeff == "NULL"&& DoseTransform == "Loge") {dosefit<-nls(responsezzzz~D         + (MaxCoeffp-D        )/(1+exp((C         -logconczzzz)*SlopeCoeffp)), start=list(C= ed50p,D=minp), data=statdata) 
	} else if (MinCoeff == "NULL"&& MaxCoeff != "NULL"&& SlopeCoeff == "NULL"&& ECIDCoeff != "NULL"&& DoseTransform != "Loge") {dosefit<-nls(responsezzzz~D         + (MaxCoeffp-D        )/(1+10^((ECIDCoeffp-logconczzzz)*B          )), start=list(B=slopep,D=minp), data=statdata) 
	} else if (MinCoeff == "NULL"&& MaxCoeff != "NULL"&& SlopeCoeff == "NULL"&& ECIDCoeff != "NULL"&& DoseTransform == "Loge") {dosefit<-nls(responsezzzz~D         + (MaxCoeffp-D        )/(1+exp((ECIDCoeffp-logconczzzz)*B          )), start=list(B=slopep,D=minp), data=statdata) 
	} else if (MinCoeff != "NULL"&& MaxCoeff != "NULL"&& SlopeCoeff == "NULL"&& ECIDCoeff == "NULL"&& DoseTransform != "Loge") {dosefit<-nls(responsezzzz~MinCoeffp + (MaxCoeffp-MinCoeffp)/(1+10^((C         -logconczzzz)*B          )), start=list(B=slopep,C= ed50p), data=statdata)
	} else if (MinCoeff != "NULL"&& MaxCoeff != "NULL"&& SlopeCoeff == "NULL"&& ECIDCoeff == "NULL"&& DoseTransform == "Loge") {dosefit<-nls(responsezzzz~MinCoeffp + (MaxCoeffp-MinCoeffp)/(1+exp((C         -logconczzzz)*B          )), start=list(B=slopep,C= ed50p), data=statdata) 
	} else if (MinCoeff == "NULL"&& MaxCoeff == "NULL"&& SlopeCoeff != "NULL"&& ECIDCoeff != "NULL"&& DoseTransform != "Loge") {dosefit<-nls(responsezzzz~D         + (A        -D        )/(1+10^((ECIDCoeffp-logconczzzz)*SlopeCoeffp)), start=list(A=maxp,D=minp), data=statdata) 
	} else if (MinCoeff == "NULL"&& MaxCoeff == "NULL"&& SlopeCoeff != "NULL"&& ECIDCoeff != "NULL"&& DoseTransform == "Loge") {dosefit<-nls(responsezzzz~D         + (A        -D        )/(1+exp((ECIDCoeffp-logconczzzz)*SlopeCoeffp)), start=list(A=maxp,D=minp), data=statdata) 
	} else if (MinCoeff != "NULL"&& MaxCoeff == "NULL"&& SlopeCoeff != "NULL"&& ECIDCoeff == "NULL"&& DoseTransform != "Loge") {dosefit<-nls(responsezzzz~MinCoeffp + (A        -MinCoeffp)/(1+10^((C         -logconczzzz)*SlopeCoeffp)), start=list(A=maxp,C= ed50p), data=statdata) 
	} else if (MinCoeff != "NULL"&& MaxCoeff == "NULL"&& SlopeCoeff != "NULL"&& ECIDCoeff == "NULL"&& DoseTransform == "Loge") {dosefit<-nls(responsezzzz~MinCoeffp + (A        -MinCoeffp)/(1+exp((C         -logconczzzz)*SlopeCoeffp)), start=list(A=maxp,C= ed50p), data=statdata) 
	} else if (MinCoeff != "NULL"&& MaxCoeff == "NULL"&& SlopeCoeff == "NULL"&& ECIDCoeff != "NULL"&& DoseTransform != "Loge") {dosefit<-nls(responsezzzz~MinCoeffp + (A        -MinCoeffp)/(1+10^((ECIDCoeffp-logconczzzz)*B          )), start=list(A=maxp,B=slopep), data=statdata) 
	} else if (MinCoeff != "NULL"&& MaxCoeff == "NULL"&& SlopeCoeff == "NULL"&& ECIDCoeff != "NULL"&& DoseTransform == "Loge") {dosefit<-nls(responsezzzz~MinCoeffp + (A        -MinCoeffp)/(1+exp((ECIDCoeffp-logconczzzz)*B          )), start=list(A=maxp,B=slopep), data=statdata) 
	} else if (MinCoeff == "NULL"&& MaxCoeff != "NULL"&& SlopeCoeff != "NULL"&& ECIDCoeff != "NULL"&& DoseTransform != "Loge") {dosefit<-nls(responsezzzz~D         + (MaxCoeffp-D        )/(1+10^((ECIDCoeffp-logconczzzz)*SlopeCoeffp)), start=list(D=minp), data=statdata) 
	} else if (MinCoeff == "NULL"&& MaxCoeff != "NULL"&& SlopeCoeff != "NULL"&& ECIDCoeff != "NULL"&& DoseTransform == "Loge") {dosefit<-nls(responsezzzz~D         + (MaxCoeffp-D        )/(1+exp((ECIDCoeffp-logconczzzz)*SlopeCoeffp)), start=list(D=minp), data=statdata) 
	} else if (MinCoeff != "NULL"&& MaxCoeff != "NULL"&& SlopeCoeff != "NULL"&& ECIDCoeff == "NULL"&& DoseTransform != "Loge") {dosefit<-nls(responsezzzz~MinCoeffp + (MaxCoeffp-MinCoeffp)/(1+10^((C         -logconczzzz)*SlopeCoeffp)), start=list(C= ed50p), data=statdata) 
	} else if (MinCoeff != "NULL"&& MaxCoeff != "NULL"&& SlopeCoeff != "NULL"&& ECIDCoeff == "NULL"&& DoseTransform == "Loge") {dosefit<-nls(responsezzzz~MinCoeffp + (MaxCoeffp-MinCoeffp)/(1+exp((C         -logconczzzz)*SlopeCoeffp)), start=list(C= ed50p), data=statdata) 
	} else if (MinCoeff != "NULL"&& MaxCoeff != "NULL"&& SlopeCoeff == "NULL"&& ECIDCoeff != "NULL"&& DoseTransform != "Loge") {dosefit<-nls(responsezzzz~MinCoeffp + (MaxCoeffp-MinCoeffp)/(1+10^((ECIDCoeffp-logconczzzz)*B          )), start=list(B=slopep), data=statdata) 
	} else if (MinCoeff != "NULL"&& MaxCoeff != "NULL"&& SlopeCoeff == "NULL"&& ECIDCoeff != "NULL"&& DoseTransform == "Loge") {dosefit<-nls(responsezzzz~MinCoeffp + (MaxCoeffp-MinCoeffp)/(1+exp((ECIDCoeffp-logconczzzz)*B          )), start=list(B=slopep), data=statdata) 
	} else if (MinCoeff != "NULL"&& MaxCoeff == "NULL"&& SlopeCoeff != "NULL"&& ECIDCoeff != "NULL"&& DoseTransform != "Loge") {dosefit<-nls(responsezzzz~MinCoeffp + (A        -MinCoeffp)/(1+10^((ECIDCoeffp-logconczzzz)*SlopeCoeffp)), start=list(A=maxp), data=statdata) 
	} else if (MinCoeff != "NULL"&& MaxCoeff == "NULL"&& SlopeCoeff != "NULL"&& ECIDCoeff != "NULL"&& DoseTransform == "Loge") {dosefit<-nls(responsezzzz~MinCoeffp + (A        -MinCoeffp)/(1+exp((ECIDCoeffp-logconczzzz)*SlopeCoeffp)), start=list(A=maxp), data=statdata) 
	} else if (MinCoeff != "NULL"&& MaxCoeff != "NULL"&& SlopeCoeff != "NULL"&& ECIDCoeff != "NULL"&& DoseTransform != "Loge") {
		HTML.title("Warning", HR=2, align="left")
		HTML("You need to estimate at least one of the parameters, hence no analysis has been performed.", align="left")
	} else if (MinCoeff != "NULL"&& MaxCoeff != "NULL"&& SlopeCoeff != "NULL"&& ECIDCoeff != "NULL"&& DoseTransform == "Loge") {
		HTML.title("Warning", HR=2, align="left")		
		HTML("You need to estimate at least one of the parameters, hence no analysis has been performed.", align="left")
	} 

	#DF test
	table<-summary(dosefit)$parameters
	tablen<-length(unique(rownames(table)))
	df<-length(na.omit(eval(parse(text = paste("statdata$", ResponseVar )))))-tablen
	if (df <= 4) {
		HTML.title("Warning", HR=2, align="left")
		HTML("Unfortunately the residual degrees of freedom are low (less than 5). This may make the estimation of the underlying variability, and hence the results of the statistical tests, unreliable. This can be caused by attempting to model too many parameters. We recommend you fix some of the parameters.", align="left")
	}

#===================================================================================================================
# Scatterplot of responses with fit
#===================================================================================================================
	HTML.title("Scatterplot of observed data including the predicted fit", HR=2, align="left")

	scatterPlot <- sub(".html", "scatterPlot.png", htmlFile)
	png(scatterPlot,width = jpegwidth, height = jpegheight, units="in", res=PlotResolution)

	#STB July2013
	plotFilepdf <- sub(".html", "scatterPlot.pdf", htmlFile)
	dev.control("enable") 

	graphdata$xaxisvarrr_ivs<-statdata$logconczzzz
	graphdata$yaxisvarrr_ivs<-eval(parse(text = paste("statdata$", ResponseVar)))
	xmax<-max(graphdata$xaxisvarrr_ivs, na.rm=TRUE)
	xmin<-min(graphdata$xaxisvarrr_ivs, na.rm=TRUE)
	av<-seq(xmin,xmax,0.01)
	bv<-predict(dosefit,list(logconczzzz =av))

	#Creating final dataset
	Type <- c(1:length(bv))
	for (i in 1:length(bv)) {Type[i]="Prediction curve"}
	tempdata<-data.frame(av,bv, Type)
	colnames(tempdata)<-c("Dose", "Response", "Type")
	tempdata2<-graphdata[,c("xaxisvarrr_ivs", "yaxisvarrr_ivs")]

	#STB Feb 2016 - Replace length(tempdata2) with dim(tempdata2)[1]
	Type <- c(1:dim(tempdata2)[1])
	for (i in 1:dim(tempdata2)[1]) {
		Type[i]=" Assay standards"
	}
	tempdata2<-data.frame(tempdata2, Type)
	colnames(tempdata2)<-c("Dose", "Response", "Type")
	finaldata<-rbind(tempdata, tempdata2)

	#New colour palette for this plot
	Col_palette <- c("#E41A1C" ,"black")
	BW_palette <- c("white", "black")

	#BandW - Line included
	if (bandw == "Y") {
		Gr_palette <-BW_palette
	} else {
		Gr_palette <-Col_palette
	}

	Line_type <- Line_type_solid
	MainTitle2<-""

	#GGPlot2 code
	Dose_Resp_ExQC()

	#===================================================================================================================
	void <- HTMLInsertGraph(GraphFileName=sub("[A-Z0-9a-z,:,\\\\]*App_Data[\\\\]","", scatterPlot), Align="centre")

	#STB July2013
	if (pdfout=="Y") {
		pdf(file=sub("[A-Z0-9a-z,:,\\\\]*App_Data[\\\\]","", plotFilepdf), height = pdfheight, width = pdfwidth) 
		dev.set(2) 
		dev.copy(which=3) 
		dev.off(2)
		dev.off(3)
		pdfFile<-sub("[A-Z0-9a-z,:,\\\\]*App_Data[\\\\]","",plotFilepdf)
		linkToPdf <- paste ("<a href=\"",pdfFile,"\">Click here to view the PDF of the scatterplot including predicted fit</a>", sep = "")
		HTML(linkToPdf)
	}

#===================================================================================================================
#Table of parameter estimates
#===================================================================================================================
	HTML.title("Table of curve parameters and summary statistics", HR=2, align="left")

	table<-summary(dosefit)$parameters
	tablen<-length(unique(rownames(table)))
	paratp<-rownames(table)
	paranams<-c(1:tablen)

	for (i in 1:tablen) { 
		if (paratp[i] == "A") {
			paranams[i] = "Max/Min"
		}
		if (paratp[i] == "B") {
			paranams[i] = "Slope"
		} 
		if (paratp[i] == "C") {
			paranams[i] = "logED50"
		} 
		if (paratp[i] == "D") {
			paranams[i] = "Min/Max"
		} 
	}

	tabz<-matrix(nrow=tablen, ncol=5)
	for (i in 1:tablen) {
		#STB Dec 2011 Formatting to 3dp
		tabz[i,1]=rownames(table)[i]
	}
	for (i in 1:tablen) {
		#STB Dec 2011 Formatting to 3dp
		tabz[i,2]=format(round(table[i,1], 3), nsmall=3, scientific=FALSE)
	}

	for (i in 1:tablen) {
		tabz[i,3]=format(round(table[i,2], 3), nsmall=3, scientific=FALSE)
	}
	for (i in 1:tablen) {
		tabz[i,4]=format(round(table[i,3], 2), nsmall=2, scientific=FALSE)
	}
	for (i in 1:tablen) {
		tabz[i,5]=format(round(table[i,4], 4), nsmall=4, scientific=FALSE)
	}
	for (i in 1:tablen)  {
		if (as.numeric(table[i,4])<0.0001)  {
			#STB March 2011 - formatting p-value <0.00010
	#		tabz[i,5]<-0.0001
			tabz[i,5]=format(round(0.0001, 4), nsmall=4, scientific=FALSE)
			tabz[i,5]<- paste("<",tabz[i,5])
		}
	}

	colnames(tabz)<-c("Parameter", "Estimate", "Std error", "t-value", "p-value")
	HTML(tabz, classfirstline="second", align="left", row.names = "FALSE")

	ictab<-data.frame(table)

	if (ECIDCoeff == "NULL") {
		#Table of parameter estimates
		#STB May 2012 Updating "back-"
		HTML.title("Back-transformed ED50 estimate with 95% confidence intervals", HR=2, align="left")

		#No. of degrees of freedom
		df<-length(statdata$responsezzzz)-tablen
		critval95<-qmvt(0.95, df = df, tail = "both", abseps=0.000001)[1]

		for (i in 1:tablen) {
			if (rownames(ictab)[i] == "C") {
				logED50<-ictab[i,1]
				ED50ERR<-ictab[i,2]
				loglower<-logED50-as.numeric(critval95)*ED50ERR
				logupper<-logED50+as.numeric(critval95)*ED50ERR	
	
				if(DoseTransform == "Log10") {
					ED50<-10**logED50-offset
					lower<-10**loglower-offset
					upper<-10**logupper-offset
				}		
				if(DoseTransform == "Loge") {
					ED50<-exp(logED50)-offset
					lower<-exp(loglower)-offset
					upper<-exp(logupper)-offset
				}
			}
		}

		#STB Dec 2011 Formatting to 3dp
		ED50table<-matrix(nrow=1, ncol=3)
		ED50table[1,1]<-format(round(ED50,3), nsmall=3, scientific=FALSE)
		ED50table[1,2]<-format(round(lower,3), nsmall=3, scientific=FALSE)
		ED50table[1,3]<-format(round(upper,3), nsmall=3, scientific=FALSE)
		ED50table<-data.frame(ED50table)
		rownames(ED50table)<-c("Estimate")
		colnames(ED50table)<-c("ED50", "Lower 95% CI", "Upper 95% CI")
		HTML(ED50table, classfirstline="second", align="left")
	}

#===================================================================================================================
#Extracting parameter estimates
#===================================================================================================================
	if (MinCoeff !="NULL") {
	D <- as.numeric(MinCoeff)
	} else { 
		for (i in 1:tablen) {
			if (rownames(ictab)[i] == "D") {
				D <-ictab[i,1]
			}
		}
	}

	if (MaxCoeff !="NULL") {
		A <- as.numeric(MaxCoeff)
	} else { 
		for (i in 1:tablen) {
			if (rownames(ictab)[i] == "A")	{
				A <-ictab[i,1]
			}
		}
	}

	if (SlopeCoeff !="NULL") {
 		B <- as.numeric(SlopeCoeff)
	} else { 
		for (i in 1:tablen) {
			if (rownames(ictab)[i] == "B") {
				B <-ictab[i,1]
			}
		}
	}

	if (ECIDCoeff !="NULL" && DoseTransform != "Loge") {
		C <- log10(as.numeric(ECIDCoeff)+offset)
	}  
	if (ECIDCoeff !="NULL" && DoseTransform == "Loge") {
		C <- log(as.numeric(ECIDCoeff)+offset)
	}	
	if (ECIDCoeff =="NULL" ) {
		for (i in 1:tablen) {
			if (rownames(ictab)[i] == "C") {
				C <-ictab[i,1]
			}
		}
	}

	if (ECIDCoeff !="NULL" && DoseTransform == "Log10") {
		btC <- as.numeric(ECIDCoeff)
	} 
	if (ECIDCoeff !="NULL" && DoseTransform == "Loge") {
		btC <- as.numeric(ECIDCoeff)
	} 	
	if (ECIDCoeff =="NULL" && DoseTransform == "Log10") {
		for (i in 1:tablen) {
			if (rownames(ictab)[i] == "C") {
				btC <-10**(ictab[i,1])-offset
			}
		}
	}  
	if (ECIDCoeff =="NULL" && DoseTransform == "Loge") {
		for (i in 1:tablen) {
			if (rownames(ictab)[i] == "C") {
				btC <-exp(ictab[i,1])-offset
			}
		}
	}

#===================================================================================================================
#Analysis decription
#===================================================================================================================
	HTML.title("Description of the analysis results", HR=2, align="left")

	add<-c("The data was analysed using non-linear regression. ")
	if (MinCoeff == "NULL") {
		add<-paste(add, "The estimate of the minimum of the curve is ", sep="")
		if (D<A) {
			#STB Dec 2011 Formatting to 3dp
			add<-paste(add, format(round(D,3), nsmall=3, scientific=FALSE), ". ", sep="")
		} else {
			add<-paste(add, format(round(A,3), nsmall=3, scientific=FALSE), ". ", sep="")
		}
	}

	if (MaxCoeff == "NULL") {
		add<-paste(add, "The estimate of the maximum of the curve is ", sep="")
		if (A>D) {
			#STB Dec 2011 Formatting to 3dp
			add<-paste(add, format(round(A,3), nsmall=3, scientific=FALSE),  ". ", sep="")
		} else {
			add<-paste(add, format(round(D,3), nsmall=3, scientific=FALSE),  ". ", sep="")
		}
	}

	if (SlopeCoeff == "NULL") {
		add<-paste(add, "The estimate of the slope coefficient of the curve is ", format(round(B,3), nsmall=3, scientific=FALSE), ". ", sep="")
	}

	if (ECIDCoeff == "NULL") {
		add<-paste(add, "The estimate of the logED50 of the curve is ", format(round(C,3), nsmall=3, scientific=FALSE), ". ", sep="")
	}

	if (ECIDCoeff == "NULL") {
		add<-paste(add, "The back-transformed estimate of the ED50 of the curve is ", format(round(btC,3), nsmall=3, scientific=FALSE), ". ", sep="")
	}

	if (MinCoeff != "NULL") {
		add<-paste(add, "The minimum of the curve was fixed at ", MinCoeff, ". ", sep="")
	}

	if (MaxCoeff != "NULL") {
		add<-paste(add, "The maximum of the curve was fixed at ", MaxCoeff, ". ", sep="")
	}

	if (SlopeCoeff != "NULL") {
		add<-paste(add, "The slope of the curve was fixed at ", SlopeCoeff,  ". ", sep="")
	}

	if (ECIDCoeff != "NULL") {
		add<-paste(add, "The ED50 of the curve was fixed at ", ECIDCoeff, ". ", sep="")
	}

	HTML(add, align="left")

	#Bate and Clark comment
	HTML(refxx, align="left")	

#===================================================================================================================
#QC samples
#===================================================================================================================
	if (QCResponse != "NULL" && QCDose != "NULL") {
		HTML.title("Scatterplot of responses and quality control (QC) samples", HR=2, align="left")

		# Plot of data
		statdata$QCresponsezzzz = eval(parse(text = paste("statdata$", QCResponse)))

		if (DoseTransform == "Log10") {	
			statdata$logQCconczzzz = log10(eval(parse(text = paste("statdata$", QCDose))))
			statdata$QCconczzzz = eval(parse(text = paste("statdata$", QCDose)))
		} else	if (DoseTransform == "Loge") {	
			statdata$logQCconczzzz = log(eval(parse(text = paste("statdata$", QCDose))))
			statdata$QCconczzzz = eval(parse(text = paste("statdata$", QCDose)))
		}

		# setting up the dataset
		SC<-data.frame(cbind(statdata$responsezzzz, statdata$logconczzzz,statdata$conczzzz))
		SClen<-dim(SC)[1]
		for (i in 1:SClen) {
			SC$Type[i]="Assay standard curve"
		}
		colnames(SC)<-c("response","logdose","dose","Type")

		QCs<-data.frame(cbind(statdata$QCresponsezzzz, statdata$logQCconczzzz, statdata$QCconczzzz))
		QCs<-na.omit(QCs) 
		QClen<-dim(QCs)[1]
		for (i in 1:QClen) {
			QCs$Type[i]="Quality controls"
		}

		colnames(QCs)<-c("response","logdose","dose","Type")
		alldata <-data.frame(rbind(SC,QCs))
		rows<-dim(alldata)[1]
		cols<-dim(alldata)[2]
		nlevels<-length(unique(as.factor(alldata$Type)))
		extra<-matrix(data=NA, nrow=rows, ncol=nlevels)
		for (i in 1:nlevels) {
			for (j in 1:rows) {
				if (alldata$Type[j] == unique(as.factor(alldata$Type))[i]) {
					extra[j,i]<-alldata$response[j]
				}
			}
		}
		newdata<-cbind(alldata, extra)
		catplotdata<-data.frame(newdata)
		for (k in 1:nlevels) {
			tempdata<-catplotdata
			tempdata2<-subset(tempdata, tempdata$Type == unique(levels(as.factor(tempdata$Type)))[k])
		}
		index<-c(1:nlevels)
		newnames<-c(colnames(alldata),index)
		colnames(catplotdata)<-newnames

		ncscatterplot3 <- sub(".html", "ncscatterplot3.png", htmlFile)
		png(ncscatterplot3,width = jpegwidth, height = jpegheight, units="in", res=PlotResolution)

		#STB July2013
		plotFilepdf3 <- sub(".html", "ncscatterplot3.pdf", htmlFile)
		dev.control("enable") 

		graphdata$xaxisvarrr_ivs<-statdata$logconczzzz
		graphdata$yaxisvarrr_ivs<-eval(parse(text = paste("statdata$", ResponseVar)))
		graphdata$QCxaxisvarrr_ivs<-statdata$logQCconczzzz
		graphdata$QCyaxisvarrr_ivs<-statdata$QCresponsezzzz
		xmax<-max(graphdata$xaxisvarrr_ivs, na.rm=TRUE)
		xmin<-min(graphdata$xaxisvarrr_ivs, na.rm=TRUE)
		av<-seq(xmin,xmax,0.01)
		bv<-predict(dosefit,list(logconczzzz =av))

		#Creating final dataset
		Type <- c(1:length(bv))
		for (i in 1:length(bv)) {Type[i]="Prediction curve"}
		tempdata<-data.frame(av,bv, Type)
		colnames(tempdata)<-c("Dose", "Response", "Type")

		tempdata2<-graphdata[,c("xaxisvarrr_ivs", "yaxisvarrr_ivs")]
		Type <- c(1:length(tempdata2))
		for (i in 1:length(tempdata2)) {Type[i]=" Assay standards"}
		tempdata2<-data.frame(tempdata2, Type)
		colnames(tempdata2)<-c("Dose", "Response", "Type")

		tempdata3<-graphdata[,c("QCxaxisvarrr_ivs", "QCyaxisvarrr_ivs")]
		Type <- c(1:length(tempdata3))
		for (i in 1:length(tempdata3)) {
			Type[i]=" Quality controls"
		}
		tempdata3<-data.frame(tempdata3, Type)
		colnames(tempdata3)<-c("Dose", "Response", "Type")

		finaldata<-rbind(tempdata, tempdata2, tempdata3)

		#New colour palette for this plot
		Col_palette <- c("#E41A1C" ,"#377EB8","black")
		BW_palette <- c("white", "#838383", "black")

		#BandW - Line included
		if (bandw == "Y") {
			Gr_palette <-BW_palette
		} else {
			Gr_palette <-Col_palette
		}
		Line_type <- Line_type_solid
		MainTitle2<-""

		#GGPlot2 code
		Dose_Resp_IncQC()

		void<-HTMLInsertGraph(GraphFileName=sub("[A-Z0-9a-z,:,\\\\]*App_Data[\\\\]","", ncscatterplot3), Align="centre")

		#STB July2013
		if (pdfout=="Y") {
			pdf(file=sub("[A-Z0-9a-z,:,\\\\]*App_Data[\\\\]","", plotFilepdf3), height = pdfheight, width = pdfwidth) 
			dev.set(2) 
			dev.copy(which=3) 
			dev.off(2)
			dev.off(3)
			pdfFile_3<-sub("[A-Z0-9a-z,:,\\\\]*App_Data[\\\\]","",plotFilepdf3)
			linkToPdf3 <- paste ("<a href=\"",pdfFile_3,"\">Click here to view the PDF of the scatterplot</a>", sep = "")
			HTML(linkToPdf3)
		}

#===================================================================================================================
#Table of QC summary statistics
#===================================================================================================================
		HTML.title("Table of QC summary statistics", HR=2, align="left")

		if (DoseTransform == "Loge")	{
			QCs$backtran<-C-(1/B)*log(((A-D)/(QCs$response-D))-1)
			length <- length(unique(levels(as.factor(QCs$dose))))
			vectorbtmean <-c(1:length)
			vectormean <-c(1:length)
			vectorStDev <-c(1:length)
			vectorN <-c(1:length)
			for (i in 1:length) {
				sub<-subset(QCs, QCs$dose == unique(levels(as.factor(QCs$dose)))[i])
				sub2<-data.frame(sub)
				vectorbtmean[i]=exp(mean(sub2$backtran, na.rm=TRUE))-offset
				vectorStDev[i]=sqrt((exp(2*mean(sub2$backtran, na.rm=TRUE))) * (exp(sd(sub2$backtran, na.rm=TRUE)*sd(sub2$backtran, na.rm=TRUE)))*(exp(sd(sub2$backtran, na.rm=TRUE)*sd(sub2$backtran, na.rm=TRUE))-1));
				vectormean[i]=mean(sub2$dose, na.rm=TRUE)
				tempy<-na.omit(sub2$backtran)
				vectorN[i]=length(tempy)
			}
		}
		if (DoseTransform == "Log10") {
			QCs$backtran<-C-(1/B)*log10(((A-D)/(QCs$response-D))-1)
			length <- length(unique(levels(as.factor(QCs$dose))))
			vectorbtmean <-c(1:length)
			vectormean <-c(1:length)
			vectorStDev <-c(1:length)
			vectorN <-c(1:length)
			for (i in 1:length) {
				sub<-subset(QCs, QCs$dose == unique(levels(as.factor(QCs$dose)))[i])
				sub2<-data.frame(sub)
				vectorbtmean[i]=10**(mean(sub2$backtran, na.rm=TRUE))-offset
				vectorStDev[i]=sqrt((10**(2*mean(sub2$backtran, na.rm=TRUE))) * (10**(sd(sub2$backtran, na.rm=TRUE)*sd(sub2$backtran, na.rm=TRUE)))*(10**(sd(sub2$backtran, na.rm=TRUE)*sd(sub2$backtran, na.rm=TRUE))-1));
				vectormean[i]=mean(sub2$dose, na.rm=TRUE)
				tempy<-na.omit(sub2$backtran)
				vectorN[i]=length(tempy)
			}
		}

		tempdata<-data.frame(cbind(vectormean,vectorbtmean,vectorStDev,vectorN))
		tempdata$RE<-100*(vectorbtmean-vectormean)/vectormean
		tempdata$CV<-100*vectorStDev/vectorbtmean

		#STB May 2012 Updating "back-"
		colnames(tempdata)<-c("True QC mean", "Back-transformed QC mean", "Std dev of QC samples", "No. of back-transformable QCs","Relative error (%)", "Coefficient of variation (%)")

		ablen<-length(unique(rownames(tempdata)))
		tab<-matrix(nrow=ablen, ncol=7)

   		tab[,1] <- c(1:ablen)
		for (i in 1:ablen) {
			#STB Dec 2011 Formatting to 3dp
			tab[i,2]=format(round(tempdata[i,1],3), nsmall=3, scientific=FALSE)
		}
		for (i in 1:ablen) {
			tab[i,3]=format(round(tempdata[i,2], 3), nsmall=3, scientific=FALSE)
		}
		for (i in 1:ablen) {
			tab[i,4]=format(round(tempdata[i,3], 3), nsmall=3, scientific=FALSE)
		}
		for (i in 1:ablen) {
			tab[i,5]=format(round(tempdata[i,4], 0), nsmall=0, scientific=FALSE)
		}
		for (i in 1:ablen) {
			tab[i,6]=format(round(tempdata[i,5], 2), nsmall=2, scientific=FALSE)
		}
		for (i in 1:ablen) {
			tab[i,7]=format(round(tempdata[i,6], 2), nsmall=2, scientific=FALSE)
		}
		colnames(tab)<-c("QC ID number", "True QC mean", "Back-calculated QC mean", "Std dev of back-calculated QC mean",  "No. of back-calculated QCs",  "Relative error (%)",  "Coefficient of variation (%)")
	
		HTML(tab, classfirstline="second", align="left", row.names = "FALSE")
		HTML("Note: The relative error (%) and coefficient of variation (%) for an individual QC are only reliable statistics if all the QCs can be back-calculated.", align="left")
	}

#===================================================================================================================
#Samples
#===================================================================================================================
	if (Samples  != "NULL" ) {
		HTML.title("Back-calculated sample responses", HR=2, align="left")

		statdata$samplesresponsezzzz = eval(parse(text = paste("statdata$", Samples)))
		if (DoseTransform == "Loge") {
			statdata$backtranss<-format(round(exp(C-(1/B)*log(((A-D)/(statdata$samplesresponsezzzz-D))-1))-offset, 3), nsmall=3, scientific=FALSE)
		}
		if (DoseTransform == "Log10") {
			statdata$backtranss<-format(round(10**(C-(1/B)*log10(((A-D)/(statdata$samplesresponsezzzz-D))-1))-offset, 3), nsmall=3, scientific=FALSE)
		}

		samples<-cbind(statdata$samplesresponsezzzz,statdata$backtranss)

		samples<-na.omit(samples) 
		samlen<-dim(samples)[1]
		index<-c(1:samlen)
		samples2<- cbind(index, samples)
		colnames(samples2)<-c("Sample ID number", "Sample response", "Back-calculated response")
		HTML(samples2, classfirstline="second", align="left", row.names = "FALSE")
	}

#===================================================================================================================
#end of Fourparameter analysis if statement
}

#===================================================================================================================
#References
#===================================================================================================================
Ref_list <- R_refs()

if (UpdateIVS == "N") {
	HTML.title("Statistical references", HR=2, align="left")
}
if (UpdateIVS == "Y") {
	HTML.title("References", HR=2, align="left")
	HTML(Ref_list$IVS_ref, align="left")
}
HTML(Ref_list$BateClark_ref, align = "left")

if (UpdateIVS == "N") {
	HTML.title("R references", HR=2, align="left")
	HTML(Ref_list$R_ref, align = "left")
	HTML(Ref_list$mtvnorm_ref, align = "left")
	HTML(Ref_list$GGally_ref, align = "left")
	HTML(Ref_list$RColorBrewers_ref, align = "left")
	HTML(Ref_list$GGPLot2_ref, align = "left")
	HTML(Ref_list$ggrepel_ref,  align="left")
	HTML(Ref_list$reshape_ref, align = "left")
	HTML(Ref_list$plyr_ref, align = "left")
	HTML(Ref_list$scales_ref, align = "left")
	HTML(Ref_list$R2HTML_ref, align = "left")
	HTML(Ref_list$PROTO_ref, align = "left")
}
if (UpdateIVS == "Y") {
	HTML.title("R references", HR=4, align="left")
	HTML(Ref_list$R_ref, align = "left")
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

	HTML(paste(capture.output(print(citation("mvtnorm"),bibtex=F))[4], capture.output(print(citation("mvtnorm"),bibtex=F))[5], capture.output(print(citation("mvtnorm"),bibtex=F))[6], capture.output(print(citation("mvtnorm"),bibtex=F))[7], sep = ""),  align="left")
}


#===================================================================================================================
#Show dataset
#===================================================================================================================
if (showdataset == "Y") {
    if (DoseResponseType == "FourParameter") {
        statdata2 <- subset(statdata, select = -c(responsezzzz, logconczzzz, conczzzz))
        if (QCResponse != "NULL" && QCDose != "NULL") {
            statdata2 <- subset(statdata2, select = -c(QCresponsezzzz, logQCconczzzz, QCconczzzz))
        }
        if (Samples != "NULL") {
            statdata2 <- subset(statdata2, select = -c(samplesresponsezzzz, backtranss))
        }
    }

    if (DoseResponseType == "Equation") {
        statdata2 <- subset(statdata, select = -c(respzzzz, x, y, conczzzz))
    }

    observ <- data.frame(c(1:dim(statdata2)[1]))
    colnames(observ) <- c("Observation")
    statdata3 <- cbind(observ, statdata2)

    HTML.title("Analysis dataset", HR = 2, align = "left")
    HTML(statdata3, classfirstline = "second", align = "left", row.names = "FALSE")
}


#===================================================================================================================
#Show arguments
#===================================================================================================================
if (OutputAnalysisOps == "Y") {
	HTML.title("Analysis options", HR=2, align="left")

	if (DoseResponseType == "FourParameter")
	{ 
		HTML(paste("Response variable: ", ResponseVar, sep=""), align="left")

		if (ResponseTransform != "None") {
			HTML(paste("Response variable transformation: ", ResponseTransform, sep=""), align="left")
		}
		HTML(paste("Dose variable: ", DoseVar, sep=""), align="left")
	
		HTML(paste("Dose variable transformation: ", DoseTransform, sep=""), align="left")

		if (Offsetz != "NULL") {
			HTML(paste("Dose variable transformation offset: ", Offsetz, sep=""), align="left")
		}

		if (QCResponse != "NULL") {
			HTML(paste("Quality control response variable: ", QCResponse, sep=""), align="left")
		}

		if (QCDose != "NULL") {	
			HTML(paste("Quality control dose variable: ", QCDose, sep=""), align="left")
		}

		if (Samples != "NULL") {
			HTML(paste("Samples variable: ", Samples, sep=""), align="left")
		}

		if (MinCoeff != "NULL")	{
			HTML(paste("Minimum parameter fixed at: ", MinCoeff, sep=""), align="left")
		}
	
		if (MaxCoeff != "NULL") {
			HTML(paste("Maximum parameter fixed at: ", MaxCoeff, sep=""), align="left")
		}

		if (SlopeCoeff != "NULL") {
			HTML(paste("Slope parameter fixed at: ", SlopeCoeff, sep=""),  align="left")
		}

		if (ECIDCoeff != "NULL") {
			HTML(paste("xD50 parameter fixed at: ", ECIDCoeff, sep=""),  align="left")
		}

		if (MinStartValue != "NULL") {
			HTML(paste("Minimum parameter start value: ", MinStartValue, sep=""),  align="left")
		}

		if (MaxStartValue != "NULL") {
			HTML(paste("Maximum parameter start value: ", MaxStartValue, sep=""),  align="left")
		}
	
		if (SlopeStartValue != "NULL") {
			HTML(paste("Slope parameter start value: ", SlopeStartValue, sep=""),  align="left")
		}

		if (ECIDStartValue != "NULL") {
			HTML(paste("xD50 parameter start value: ", ECIDStartValue, sep=""),  align="left")
		}
	} else {
		HTML(paste("User defined equation: ", Equation, sep=""),  align="left")
		HTML(paste("User defined parameter start values: ", StartValues, sep=""),  align="left")
		HTML(paste("Y-axis variable: ", EquationResponse , sep=""),  align="left")
		HTML(paste("X-axis variable: ", EquationDose, sep=""),  align="left")
	}
}