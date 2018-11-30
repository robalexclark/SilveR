#===================================================================================================================
#R Libraries

suppressWarnings(library(R2HTML))

#===================================================================================================================
# retrieve args

#Copy Args into variables
Args <- commandArgs(TRUE)
valueType <- Args[4]
meanOrResponse <- Args[5]
varianceTypeOrTreatment <- Args[6]
varianceAmntOrControl <- Args[7]
sig <- as.numeric(Args[8])
changesType <- Args[9]
changesValue <- Args[10]
plotSettingsType <- Args[11]
plotSettingsFrom <- as.numeric(Args[12])
plotSettingsTo <- as.numeric(Args[13])
graphTitle <- Args[14]

# Reading in dataset or values
if (valueType=="DatasetValues") {
	#Read in data
	statdata <- read.csv(Args[3], header=TRUE, sep=",")
	#STB NOV2015 - Update to dataset print
	statdata_print<-statdata
	response <- meanOrResponse
	treatment <- varianceTypeOrTreatment
	control <- varianceAmntOrControl
} else {
	if(meanOrResponse == "NULL") {
	groupMean <- 1
	} else  {
	groupMean <- as.numeric(meanOrResponse)
	}
	varianceType <- varianceTypeOrTreatment
	
	if(varianceType=="Variance") {
		SD <- sqrt(as.numeric(varianceAmntOrControl))
	} else {
		SD <- as.numeric(varianceAmntOrControl)
	}
}

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

#Variables set up
powerFrom <- 0
powerTo <- 105
sampleSizeFrom <- 6
sampleSizeTo <- 15

tempChanges <-strsplit(changesValue, ",")
expectedChanges <- numeric(0)
for(i in 1:length(tempChanges[[1]])) { expectedChanges [length(expectedChanges )+1] = as.numeric(tempChanges[[1]][i]) } 
expectedChanges<-sort(expectedChanges)

#Categorised factors
if(plotSettingsType=="PowerAxis") {
	if(valueType=="DatasetValues") {
		if(changesType == "Absolute") {
			if (treatment== "NULL") {
				powerFrom <- plotSettingsFrom ;
				powerTo <- plotSettingsTo ;
    				testANOVA<-aov(eval(parse(text = paste("statdata$", response)))~1)
				standev<-sqrt(anova(testANOVA)[1,3])
				sampleSizeFrom <- floor(as.numeric(power.t.test(delta=expectedChanges[length(expectedChanges)], power=((powerFrom/100)),sd=standev, sig.level=sig)[1]))
				sampleSizeTo <- ceiling(as.numeric(power.t.test(n=NULL,delta=expectedChanges[1], power=((powerTo/100)), sd=standev,sig.level=sig)[1]))
			} else {
				treatTemp<-as.factor(eval(parse(text = paste("statdata$", treatment))))
				statdata<-cbind(statdata,treatTemp)
				powerFrom <- plotSettingsFrom ;
				powerTo <- plotSettingsTo ;
				testANOVA<-aov(eval(parse(text = paste("statdata$", response)))~statdata$treatTemp)
				standev<-sqrt(anova(testANOVA)[2,3])
				sampleSizeFrom <- floor(as.numeric(power.t.test(delta=expectedChanges[length(expectedChanges)], power=((powerFrom/100)),sd=standev, sig.level=sig)[1]))
				sampleSizeTo <- ceiling(as.numeric(power.t.test(n=NULL,delta=expectedChanges[1], power=((powerTo/100)), sd=standev,sig.level=sig)[1]))
			}
		}
		else if (changesType == "Percent") {
			predmeans<- unlist(lapply(split(eval(parse(text = paste("statdata$", response))),eval(parse(text= paste("statdata$", treatment)))), mean))
			contrmean<-predmeans[control]
			treatTemp<-as.factor(eval(parse(text = paste("statdata$", treatment))))
			statdata<-cbind(statdata,treatTemp)
			groupMean <- contrmean
			meanvec<-rep(groupMean,length(expectedChanges))
			temp10<-expectedChanges/100
			temp11<-meanvec*temp10
			powerFrom <- plotSettingsFrom ;
			powerTo <- plotSettingsTo ;
  			testANOVA<-aov(eval(parse(text = paste("statdata$", response)))~statdata$treatTemp)
			standev<-sqrt(anova(testANOVA)[2,3])
			sampleSizeFrom <- floor(as.numeric(power.t.test(delta=temp11[length(temp11)], power=((powerFrom/100)),sd=standev, sig.level=sig)[1]))
			sampleSizeTo <- ceiling(as.numeric(power.t.test(n=NULL,delta=temp11[1], power=((powerTo/100)), sd=standev,sig.level=sig)[1]))
		}
	} else if(valueType=="UserValues") {
		if(changesType == "Absolute") {
			powerFrom <- plotSettingsFrom ;
			powerTo <- plotSettingsTo ;
			sampleSizeFrom <- floor(as.numeric(power.t.test(delta=expectedChanges[length(expectedChanges)],power=((powerFrom/100)),sd=SD, sig.level=sig)[1]))
			sampleSizeTo <- ceiling(as.numeric(power.t.test(n=NULL,delta=expectedChanges[1],power=((powerTo/100)), sd=SD,sig.level=sig)[1]))
		}  else if (changesType == "Percent") {
			groupMean <- as.numeric(meanOrResponse)
			meanvec<-rep(groupMean,length(expectedChanges))
			temp10<-expectedChanges/100
			temp11<-meanvec*temp10
			powerFrom <- plotSettingsFrom ;
			powerTo <- plotSettingsTo ;
			sampleSizeFrom <- floor(as.numeric(power.t.test(delta=temp11[length(temp11)], power=((powerFrom/100)),sd=SD, sig.level=sig)[1]))
			sampleSizeTo <- ceiling(as.numeric(power.t.test(n=NULL,delta=temp11[1], power=((powerTo/100)), sd=SD,sig.level=sig)[1]))
		}
	}
} else {
	sampleSizeFrom <- plotSettingsFrom;
	sampleSizeTo <- plotSettingsTo;
}

#Graphical parameter setup
if(graphTitle=="NULL") {
	graphTitle <- ""
} else {
	graphTitle <- paste (graphTitle, " \n", sep = "")
}
ReferenceLine <- "NULL"

#===================================================================================================================
#Output HTML header
Title <-paste(branding, " 'Comparison of Means' Power Analysis", sep="")

HTML.title(Title, HR = 1, align = "left")
HTML("Power calculations made by InVivoStat assume the statistical analysis will be performed using the two sample t-test. This may lead to slightly conservative estimates of sample sizes and statistical power.", align="left")

#Bate and Clark comment
HTML(refxx, align="left")	

HTML.title("Power curve plot", HR = 1, align="left")

#===================================================================================================================
#Power analysis functions

sipowerttest<-function(delta,sd,n,sig.level) {
	NCP<-delta/(sd*sqrt(2/n))
	crit1<-qt((1-sig.level/2),df=(2*n-2))
	crit2<-qt((1-(1-sig.level/2)),df=(2*n-2))
	suppressWarnings(power<-1-pt(crit1,(2*n-2),NCP)+pt(crit2,(2*n-2),NCP))
}

# actual change using inputted values
powercurvesactual<-function(standev, diffs) {
	legtitle<-c("- size of difference")
	legtitle2<-rep(legtitle,length(diffs))
	legtitle3<-paste(diffs,legtitle2)

	if(plotSettingsType=="PowerAxis") {
		sample<-c(sampleSizeFrom:sampleSizeTo)
	} else {
		sample <- seq(sampleSizeFrom,sampleSizeTo, 0.01)
	}
	temp1<-sample
	temp2<-matrix(1,nrow=length(sample),ncol=length(diffs))
	for (j in 1:length(diffs)) {
		for(i in 1:length(sample)) {
			test<-sipowerttest(n=sample[i], delta=diffs[j], sd=standev, sig.level=sig)
	 		temp2[i,j]=test
		}
	}
	temp3<-100*temp2
	powergraph=cbind(sample,temp3)

	powerPlot <- sub(".html", "powerPlot.jpg", htmlFile)
	jpeg(powerPlot,width = jpegwidth, height = jpegheight, quality = 100)

	#STB July2013
	plotFilepdf1 <- sub(".html", "powerPlot.pdf", htmlFile)
	dev.control("enable") 

	#===================================================================================================================
	#Graphics parameters

	XAxisTitle <- "Sample size (n)"
	MainTitle2 <-graphTitle

	#Generating the variable list for the lines - they are labelled V1 up to Vn
	lin_no<-dim(temp3)[2]
	lin_list<-c(2:(lin_no+1))
	lin_list2<-paste("V",lin_list,sep = "")

	gr_temp<-data.frame(cbind(sample, temp3))
	power2data<-melt(data=gr_temp, id=c("sample"))

	#need to expand colour range for individual animals
	colourcount_P = length(unique(power2data$variable))
	getPalette_P = colorRampPalette(brewer.pal(9,"Set1"))

	if (colourcount_P >=10) {
  	Col_palette_P<-getPalette(colourcount_P)
	} else {
  		Col_palette_P<-brewer.pal(colourcount_P,"Set1")
	}

	if (bandw == "Y") {
		BW_palette_P<-grey.colors(colourcount_P, 0.1, 0.7)
		Gr_palette_P <-BW_palette_P
	} else {
	Gr_palette_P <-Col_palette_P
	}

	#GGPLOT2
	POWERPLOT_ABSOLUTE(power2data,XAxisTitle,MainTitle2,lin_list2,Gr_palette_P)
	#===================================================================================================================

	void <- HTMLInsertGraph(GraphFileName=sub("[A-Z0-9a-z,:,\\\\]*App_Data[\\\\]","", powerPlot), Align="left")

	#STB July2013
	if (pdfout=="Y") {
		pdf(file=sub("[A-Z0-9a-z,:,\\\\]*App_Data[\\\\]","", plotFilepdf1), height = pdfheight, width = pdfwidth) 
		dev.set(2) 
		dev.copy(which=3) 
		dev.off(2)
		dev.off(3)
		pdfFile_1<-sub("[A-Z0-9a-z,:,\\\\]*App_Data[\\\\]","",plotFilepdf1)
		linkToPdf1 <- paste ("<a href=\"",pdfFile_1,"\">Click here to view the PDF of the power curve plot</a>", sep = "")
		HTML(linkToPdf1)
	}
}

# percentage change using iNputted values
powercurvespercentage<-function(mean,standev,pcchange) {
	meanvec<-rep(mean,length(pcchange))
	temp10<-pcchange/100
	temp11<-meanvec*temp10
	legtitle<-c("% chg fm control")
	legtitle2<-rep(legtitle,length(temp11))
	legtitle3<-paste(pcchange,legtitle2)

	if(plotSettingsType=="PowerAxis") {
		sample<-c(sampleSizeFrom:sampleSizeTo)
	} else {
		sample <- seq(sampleSizeFrom,sampleSizeTo, 0.01)
	}

	temp1<-sample
	temp2<-matrix(1,nrow=length(sample),ncol=length(temp11))
	for (j in 1:length(temp11)) {
		for(i in 1:length(sample)) {
			test<-sipowerttest(n=sample[i], delta=temp11[j], sd=standev, sig.level=sig); temp2[i,j]=test
		}
	}
	temp3<-100*temp2
	powergraph=cbind(sample,temp3)
	
	powerPlot <- sub(".html", "powerPlot.jpg", htmlFile)
	jpeg(powerPlot,width = jpegwidth, height = jpegheight, quality = 100)

	#STB July2013
	plotFilepdf1 <- sub(".html", "powerPlot.pdf", htmlFile)
	dev.control("enable") 

	#===================================================================================================================
	#Graphics parameters

	XAxisTitle <- "Sample size (n)"
	MainTitle2 <-graphTitle

	#Generating the variable list for the lines - they are labelled V1 up to Vn
	lin_no<-dim(temp3)[2]
	lin_list<-c(2:(lin_no+1))
	lin_list2<-paste("V",lin_list,sep = "")

	expectedChanges2<-paste(expectedChanges,"% ", sep="")

	gr_temp<-data.frame(cbind(sample, temp3))
	power2data<-melt(data=gr_temp, id=c("sample"))

	#need to expand colour range for individual animals
	colourcount_P = length(unique(power2data$variable))
	getPalette_P = colorRampPalette(brewer.pal(9,"Set1"))
	if (colourcount_P >=10) {
		Col_palette_P<-getPalette(colourcount_P)
	} else {
  		Col_palette_P<-brewer.pal(colourcount_P,"Set1")
	}

	if (bandw == "Y") {
		BW_palette_P<-grey.colors(colourcount_P, 0.1, 0.7)
		Gr_palette_P <-BW_palette_P
	} else {
	Gr_palette_P <-Col_palette_P
	}

	#GGPLOT2 code
	POWERPLOT_PERCENT(power2data,XAxisTitle,MainTitle2,lin_list2,Gr_palette_P,expectedChanges2)
	#===================================================================================================================
	void <- HTMLInsertGraph(GraphFileName=sub("[A-Z0-9a-z,:,\\\\]*App_Data[\\\\]","", powerPlot), Align="left")

	#STB July2013
	if (pdfout=="Y") {
	pdf(file=sub("[A-Z0-9a-z,:,\\\\]*App_Data[\\\\]","", plotFilepdf1), height = pdfheight, width = pdfwidth) 
	dev.set(2) 
	dev.copy(which=3) 
	dev.off(2)
	dev.off(3)
	pdfFile_1<-sub("[A-Z0-9a-z,:,\\\\]*App_Data[\\\\]","",plotFilepdf1)
	linkToPdf1 <- paste ("<a href=\"",pdfFile_1,"\">Click here to view the PDF of the power curve plot</a>", sep = "")
	HTML(linkToPdf1)
	}
}

# actual changes using one-way ANOVA to compute variance
powercurvesactualANOVA<-function(resp, treat, diffs) {
	testANOVA<-aov(resp~treat)
	standev<-sqrt(anova(testANOVA)[2,3])
	powercurvesactual(standev, diffs)
}

#Special case, no treatment factor selected
powercurvesactualANOVA2<-function(resp, diffs) {
	testANOVA<-aov(resp~1)
	standev<-sqrt(anova(testANOVA)[1,3])
	powercurvesactual(standev, diffs)
}
 
# percentage change from control using one-way ANOVA to compute variance
powercurvespercentageANOVA<-function(resp, treat, ctrl, pcchange) {
	testANOVA<-aov(resp~treat)
	standev<-sqrt(anova(testANOVA)[2,3])
	predmeans<- unlist(lapply(split(resp, treat), mean))
	contrmean<-predmeans[ctrl]
	powercurvespercentage(contrmean, standev, pcchange)
}

#===================================================================================================================
#Geberating the power curves
#===================================================================================================================
if (valueType == "UserValues") {
	if(changesType == "Absolute") {
		powercurvesactual(SD, expectedChanges)
	} else if (changesType == "Percent") {
		powercurvespercentage(groupMean, SD, expectedChanges) 
	}
} else if (valueType == "DatasetValues") {
	if(changesType == "Absolute") {
		if(treatment== "NULL") {
			powercurvesactualANOVA2(eval(parse(text = paste("statdata$", response))), expectedChanges)
		} else {
			treatTemp<-as.factor(eval(parse(text = paste("statdata$", treatment))))
			statdata<-cbind(statdata,treatTemp)
			powercurvesactualANOVA(eval(parse(text = paste("statdata$", response))), statdata$treatTemp, expectedChanges)
		}
	} else if (changesType == "Percent") {
		treatTemp<-as.factor(eval(parse(text = paste("statdata$", treatment))))
		statdata<-cbind(statdata,treatTemp)
		powercurvespercentageANOVA(eval(parse(text = paste("statdata$", response))), statdata$treatTemp, eval(control), expectedChanges)
	}
}

#===================================================================================================================
#References
#===================================================================================================================
HTML.title("Selected results", HR=2, align="left")

# Text if Power is selected
if(plotSettingsType=="PowerAxis") {
	#Selected results for user defined parameters
	if (valueType == "UserValues") {
		if(changesType == "Absolute") {
			sample<-c(sampleSizeFrom, floor((sampleSizeFrom+sampleSizeTo)/2), sampleSizeTo)
			for (j in 1:1) {
				for(i in 1:length(sample))  {
					test<-sipowerttest(n=sample[i], delta=expectedChanges[j], sd=SD, sig.level=sig)
					pow<-format(round(100*test,0),nsmall=0)
					text1<-paste("Assuming the significance level is set at ", 100*sig , "%, and the sample size is ", sample[i], ", the power of the experiment to detect a biologically relevant effect of size ", expectedChanges[j], " is ",pow, "%.  " , sep="")
					HTML(text1, align="left")
				}
			}
		}
		if(changesType == "Percent") {
			sample<-c(sampleSizeFrom, floor((sampleSizeFrom+sampleSizeTo)/2), sampleSizeTo)
			groupMean <- as.numeric(meanOrResponse)
			meanvec<-rep(groupMean,length(expectedChanges))
			temp10<-expectedChanges/100
			temp11<-meanvec*temp10
			for (j in 1:1) {
				for(i in 1:length(sample)) {
					test<-sipowerttest(n=sample[i], delta=temp11[j], sd=SD, sig.level=sig)
					pow<-format(round(100*test,0),nsmall=0)
					text1<-paste("Assuming the significance level is set at " , 100*sig, "%, and the sample size is " ,sample[i],", the power of the experiment to detect a biologically relevant ", expectedChanges[j],"% change from control is ",pow,"%.  " , sep="")
					HTML(text1, align="left")
				}
			}
		}
	} else if (valueType == "DatasetValues") {
		if(changesType == "Absolute") {
			sample<-c(sampleSizeFrom, floor((sampleSizeFrom+sampleSizeTo)/2), sampleSizeTo)
			if(treatment== "NULL") {
				testANOVA<-aov(eval(parse(text = paste("statdata$", response, "~1"))))
				standev<-sqrt(anova(testANOVA)[1,3])
			} else {
				treatTemp<-as.factor(eval(parse(text = paste("statdata$", treatment))))
				statdata<-cbind(statdata,treatTemp)
				testANOVA<-aov(eval(parse(text = paste("statdata$", response)))~statdata$treatTemp)
				standev<-sqrt(anova(testANOVA)[2,3])
			}
			for (j in 1:1) {
				for(i in 1:length(sample)) {
					test<-sipowerttest(n=sample[i], delta=expectedChanges[j], sd=standev, sig.level=sig)
					pow<-format(round(100*test,0),nsmall=0)
					text1<-paste("Assuming the significance level is set at ", 100*sig, "%, and the sample size is ", sample[i], ", the power of the experiment to detect a biologically relevant effect of size ", expectedChanges[j]," is ", pow, "%.  ",sep="")
					HTML(text1, align="left")
				}
			}
		}
		if(changesType == "Percent") {
			sample<-c(sampleSizeFrom, floor((sampleSizeFrom+sampleSizeTo)/2), sampleSizeTo)
			treatTemp<-as.factor(eval(parse(text = paste("statdata$", treatment))))
			statdata<-cbind(statdata,treatTemp)
			testANOVA<-aov(eval(parse(text = paste("statdata$", response)))~statdata$treatTemp)
			standev<-sqrt(anova(testANOVA)[2,3])
			predmeans<- unlist(lapply(split(eval(parse(text = paste("statdata$", response))),eval(parse(text= paste("statdata$", treatment)))), mean))			
			contrmean<-predmeans[control]
			groupMean <- contrmean
			meanvec<-rep(groupMean,length(expectedChanges))
			temp10<-expectedChanges/100
			temp11<-meanvec*temp10
			for (j in 1:1) {
				for(i in 1:length(sample))  {
					test<-sipowerttest(n=sample[i], delta=temp11[j], sd=standev, sig.level=sig)
					pow<-format(round(100*test,0),nsmall=0)
					text1<-paste("Assuming the significance level is set at ", 100*sig, "%, and the sample size is " ,sample[i], ", the power of the experiment to detect a biologically relevant ", expectedChanges[j], "% change from control is ",pow, "%.  ", sep="")
					HTML(text1, align="left")
				}
			}
		}
	}
} else	{
	#Selected results for user defined parameters
	if (valueType == "UserValues") {
		if(changesType == "Absolute") {
			sample<-c(sampleSizeFrom, floor((sampleSizeFrom+sampleSizeTo)/2), sampleSizeTo)

			for (j in 1:1) {
				for(i in 1:length(sample))  {
					test<-sipowerttest(n=sample[i], delta=expectedChanges[j], sd=SD, sig.level=sig)
					pow<-format(round(100*test,0),nsmall=0)
					text1<-paste("Assuming the significance level is set at ", 100*sig,"%, and the sample size is ", sample[i], ", the power of the experiment to detect a biologically relevant effect of size ", expectedChanges[j] , " is " , pow , "%.  " , sep="")
					HTML(text1, align="left")
					}
				}
			}
		if(changesType == "Percent") {
			sample<-c(plotSettingsFrom, floor((plotSettingsFrom+plotSettingsTo)/2), plotSettingsTo)
			groupMean <- as.numeric(meanOrResponse)
			meanvec<-rep(groupMean,length(expectedChanges))
			temp10<-expectedChanges/100
			temp11<-meanvec*temp10
			for (j in 1:1) {
				for(i in 1:length(sample)) {
					test<-sipowerttest(n=sample[i], delta=temp11[j], sd=SD, sig.level=sig)
					pow<-format(round(100*test,0),nsmall=0)
					text1<-paste("Assuming the significance level is set at ", 100*sig , "%, and the sample size is " ,  sample[i],  ", the power of the experiment to detect a biologically relevant ", expectedChanges[j], "% change from control is ", pow, "%.  ", sep="")
					HTML(text1, align="left")
					}
				}
			}
		} else if (valueType == "DatasetValues") {
		if(changesType == "Absolute") {
			sample<-c(plotSettingsFrom, floor((plotSettingsFrom+plotSettingsTo)/2), plotSettingsTo)
			if(treatment== "NULL") {
				testANOVA<-aov(eval(parse(text = paste("statdata$", response, "~1"))))
				standev<-sqrt(anova(testANOVA)[1,3])
			} else {
				treatTemp<-as.factor(eval(parse(text = paste("statdata$", treatment))))
				statdata<-cbind(statdata,treatTemp)
				testANOVA<-aov(eval(parse(text = paste("statdata$", response)))~statdata$treatTemp)
				standev<-sqrt(anova(testANOVA)[2,3])
			}
			for (j in 1:1) {
				for(i in 1:length(sample))  {
					test<-sipowerttest(n=sample[i], delta=expectedChanges[j], sd=standev, sig.level=sig)
					pow<-format(round(100*test,0),nsmall=0)
					text1<-paste("Assuming the significance level is set at ", 100*sig , "%, and the sample size is ",  sample[i], ", the power of the experiment to detect a biologically relevant effect of size ", expectedChanges[j], " is ", pow, "%.  ", sep="")
					HTML(text1, align="left")
					}
				}
			}
		if(changesType == "Percent") {
			sample<-c(plotSettingsFrom, floor((plotSettingsFrom+plotSettingsTo)/2), plotSettingsTo)
			treatTemp<-as.factor(eval(parse(text = paste("statdata$", treatment))))
			statdata<-cbind(statdata,treatTemp)		
			testANOVA<-aov(eval(parse(text = paste("statdata$", response)))~statdata$treatTemp)
			standev<-sqrt(anova(testANOVA)[2,3])
			predmeans<- unlist(lapply(split(eval(parse(text = paste("statdata$", response))),eval(parse(text= paste("statdata$", treatment)))), mean))			
			contrmean<-predmeans[control]
			groupMean <- contrmean
			meanvec<-rep(groupMean,length(expectedChanges))
			temp10<-expectedChanges/100
			temp11<-meanvec*temp10

			for (j in 1:1) {
				for(i in 1:length(sample)) {
					test<-sipowerttest(n=sample[i], delta=temp11[j], sd=standev, sig.level=sig)
					pow<-format(round(100*test,0),nsmall=0)
					text1<-paste("Assuming the significance level is set at ", 100*sig, "%, and the sample size is " , sample[i], ", the power of the experiment to detect a biologically relevant ", expectedChanges[j], "% change from control is ",pow,  "%.  ",sep="")
					HTML(text1, align="left")
					}
			}
		}
	}
} 

HTML.title("Definitions", HR=2, align="left")
HTML("Power: The chance of achieving a statistically significant test result from running an experiment, assuming there is a real biological effect to find.", align="left")
HTML("Significance level: The chance that the experiment will give a false-positive result.", align="left")
HTML("Biologically relevant effect: The size of effect that is of scientific interest.", align="left")

#===================================================================================================================
#References
#===================================================================================================================
Ref_list <- R_refs()

HTML.title("Statistical references", HR = 2, align = "left")
HTML(Ref_list$BateClark_ref, align = "left")
HTML("Harrison, DA and Brady, AR (2004). Sample size and power calculations using the noncentral t-distribution. The Stata Journal, 4(2), 142-153.", align = "left")

HTML.title("R references", HR = 2, align = "left")
HTML(Ref_list$R_ref,  align = "left")
HTML(Ref_list$GGally_ref,  align = "left")
HTML(Ref_list$RColorBrewers_ref,  align = "left")
HTML(Ref_list$GGPLot2_ref,  align = "left")
HTML(Ref_list$reshape_ref,  align = "left")
HTML(Ref_list$plyr_ref,  align = "left")
HTML(Ref_list$scales_ref, align = "left")
HTML(Ref_list$R2HTML_ref,  align = "left")
HTML(Ref_list$PROTO_ref,  align = "left")

#===================================================================================================================
#Show dataset
#===================================================================================================================
if (showdataset == "Y") {
    if (valueType == "DatasetValues") {
	    observ <- data.frame(c(1:dim(statdata_print)[1]))
	    colnames(observ) <- c("Observation")
	    statdata_print2 <- cbind(observ, statdata_print)

	    HTML.title("Analysis dataset", HR = 2, align = "left")
	    HTML(statdata_print2, classfirstline = "second", align = "left", row.names = "FALSE")
    }
}

#===================================================================================================================
#Show arguments
#===================================================================================================================
HTML.title("Analysis options", HR=2, align="left")
if (valueType == "DatasetValues") { 
	HTML(paste("Analysis method: Dataset based inputs"),  align="left")
	HTML(paste("Response variable: ", meanOrResponse, sep=""),  align="left")
	HTML(paste("Treatment variable: ", varianceTypeOrTreatment, sep=""),  align="left")
	HTML(paste("Control group: ", varianceAmntOrControl, sep=""),  align="left")
} else {
	HTML(paste("Analysis method: User based inputs"),  align="left")
	HTML(paste("Mean value: ", meanOrResponse, sep=""),  align="left")
	HTML(paste("Variability estimate entered: ", varianceTypeOrTreatment, sep=""),  align="left")
	HTML(paste("Variability estimate: ", varianceAmntOrControl, sep=""),  align="left")
}

HTML(paste("Type of change investigated: ", changesType, sep=""), align="left")
HTML(paste("Change values investigated: ", changesValue, sep=""), align="left")

if (plotSettingsType == "PowerAxis")	{
	HTML(paste("Power curve plots controlled by power range:"), align="left")
	} else {
	HTML(paste("Power curve plots controlled by sample size range:"), align="left")
	}
HTML(paste("Plot setting from: ", plotSettingsFrom, sep=""), align="left")
HTML(paste("Plot setting to: ", plotSettingsTo, sep=""), align="left")
HTML(paste("Significance level: ", sig, sep=""), align="left")