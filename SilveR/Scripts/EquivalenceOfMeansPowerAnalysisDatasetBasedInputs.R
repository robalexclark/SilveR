#===================================================================================================================
#R Libraries

suppressWarnings(library(R2HTML))
suppressWarnings(library(PowerTOST))

#===================================================================================================================
# retrieve args

#Copy Args into variables
Args <- commandArgs(TRUE)
valueType <- tolower(Args[4])
meanOrResponse <- Args[5]
varianceTypeOrTreatment <- Args[6]
varianceAmntOrControl <- Args[7]
sig <- as.numeric(Args[8])
Difflist <- Args[9]
EqBtype <- tolower(Args[10])
lowerboundtest <- as.numeric(Args[11])
upperboundtest <- as.numeric(Args[12])
lowerboundtestN <- Args[11]
upperboundtestN <- Args[12]
plotSettingsType <- tolower(Args[13])
plotSettingsFrom <- as.numeric(Args[14])
plotSettingsTo <- as.numeric(Args[15])
graphTitle <- Args[16]

# Reading in dataset or values
if (valueType=="datasetvalues") {
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

#Defining the bias list
tempChanges <-strsplit(Difflist, ",")
expectedBias <- numeric(0)
for(i in 1:length(tempChanges[[1]])) { expectedBias [length(expectedBias )+1] = as.numeric(tempChanges[[1]][i]) } 
expectedBias<-sort(expectedBias)

#Variables set up
powerFrom <- 0
powerTo <- 105
sampleSizeFrom <- 6
sampleSizeTo <- 15

#Defining the analysis type
if (lowerboundtestN != "NULL" && upperboundtestN != "NULL") {
	AnalysisType = "two-sided"
}
if (lowerboundtestN == "NULL" && upperboundtestN != "NULL") {
	AnalysisType = "upper-sided"
}
if (lowerboundtestN != "NULL" && upperboundtestN == "NULL") {
	AnalysisType = "lower-sided"
}

#Defining the equivalence bounds
lower      <- lowerboundtest
upper      <- upperboundtest

if (EqBtype == "percentage" ) {
	if (varianceAmntOrControl == "NULL") {
		overallmean <- mean(eval(parse(text = paste("statdata$", response))), na.rm = TRUE)
	}
	if (varianceAmntOrControl != "NULL") {
		tempdata <- statdata
		tempdata$temp <- paste("T", eval(parse(text = paste("statdata$", varianceTypeOrTreatment))), sep="")
		temocont<-paste("T", control, sep = "")
		if (is.numeric(eval(parse(text = paste("statdata$", varianceTypeOrTreatment)))) == TRUE) {
			tempdata$temp <- paste("T'", eval(parse(text = paste("statdata$", varianceTypeOrTreatment))), "'",sep="")
		}
		controldata <-  subset(tempdata, tempdata$temp == temocont)
		overallmean <- mean(eval(parse(text = paste("controldata$", response))), na.rm = TRUE)
	}

	if (AnalysisType == "two-sided") {
		lower <-  -1*overallmean*(lowerboundtest/100)
		upper <- overallmean*(upperboundtest/100)
		lowerboundtest <- lower
		upperboundtest <- upper
	}
	if (AnalysisType == "lower-sided") {
		lower <- -1*overallmean*(lowerboundtest/100)
		lowerboundtest <- lower
	}
	if (AnalysisType == "upper-sided") {
		upper <- overallmean*(upperboundtest/100)
		upperboundtest <- upper
	}
}


#Define the plotting range
if(plotSettingsType=="poweraxis") {
	if(valueType=="datasetvalues") {
		if (treatment== "NULL") {
			powerFrom <- plotSettingsFrom ;
			powerTo <- plotSettingsTo ;
   				testANOVA<-aov(eval(parse(text = paste("statdata$", response)))~1)
			standev<-sqrt(anova(testANOVA)[1,3])
			sampleSizeFrom <- 0.5*floor(as.numeric(sampleN.TOST(targetpower = ((powerFrom/100)), alpha=sig, logscale = FALSE, theta1 = lower, theta2 = upper, theta0 = expectedBias[1], CV = standev, design = "parallel")[7]))
			sampleSizeTo <- 0.5*ceiling(as.numeric(sampleN.TOST(targetpower = ((powerTo/100)), alpha=sig, logscale = FALSE, theta1 = lower, theta2 = upper, theta0 = expectedBias[length(expectedBias)], CV = standev, design = "parallel")[7]))
		} else {
			treatTemp<-as.factor(eval(parse(text = paste("statdata$", treatment))))
			statdata<-cbind(statdata,treatTemp)
			powerFrom <- plotSettingsFrom ;
			powerTo <- plotSettingsTo ;
			testANOVA<-aov(eval(parse(text = paste("statdata$", response)))~statdata$treatTemp)
			standev<-sqrt(anova(testANOVA)[2,3])
			sampleSizeFrom <- 0.5*floor(as.numeric(sampleN.TOST(targetpower = ((powerFrom/100)), alpha=sig, logscale = FALSE, theta1 = lower, theta2 = upper, theta0 = expectedBias[1], CV = standev, design = "parallel")[7]))
			sampleSizeTo <- 0.5*ceiling(as.numeric(sampleN.TOST(targetpower = ((powerTo/100)), alpha=sig, logscale = FALSE, theta1 = lower, theta2 = upper, theta0 = expectedBias[length(expectedBias)], CV = standev, design = "parallel")[7]))
		}
	} else if(valueType=="uservalues") {
		powerFrom <- plotSettingsFrom ;
		powerTo <- plotSettingsTo ;
		sampleSizeFrom <- 0.5*floor(as.numeric(sampleN.TOST(targetpower = ((powerFrom/100)), alpha=sig, logscale = FALSE, theta1 = lower, theta2 = upper, theta0 = expectedBias[1], CV = standev, design = "parallel")[7]))
		sampleSizeTo <- 0.5*ceiling(as.numeric(sampleN.TOST(targetpower = ((powerTo/100)), alpha=sig, logscale = FALSE, theta1 = lower, theta2 = upper, theta0 = expectedBias[length(expectedBias)], CV = standev, design = "parallel")[7]))
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
XLimLow <- "NULL"
XLimHigh <- "NULL"
YLimLow <- "NULL"
YLimHigh <- "NULL"



#===================================================================================================================
#Output HTML header
Title <-paste(branding, " 'Equivalence of Means' Power Analysis", sep="")
HTML.title(Title, HR = 1, align = "left")

#Software developement version warning
if (Betawarn == "Y") {
	HTML.title("Warning", HR=2, align="left")
	HTML(BetaMessage, align="left")
}

HTML("Power calculations made by InVivoStat assume the statistical analysis will be performed using the TOST (Two One-Sided Test) of equivalence. This may lead to slightly conservative estimates of sample sizes and statistical power.", align="left")

#Bate and Clark comment
HTML(refxx, align="left")	

HTML.title("Power curve plot", HR = 1, align="left")

#===================================================================================================================
#Power analysis functions

# actual change using inputted values
powercurvesactual<-function(standev) {
	legtitle<-c("- size of difference")

	if(plotSettingsType=="poweraxis") {
		sample<-2*c(sampleSizeFrom:sampleSizeTo)
	} else {
		sample <- 2*seq(sampleSizeFrom,sampleSizeTo, 1)
	}
	temp1<-sample
	temp2<-matrix(1,nrow=length(sample),ncol=length(expectedBias))
	for (j in 1:length(expectedBias)) {
		for(i in 1:length(sample)) {
			test<-power.TOST(n=sample[i], alpha=sig,   logscale = FALSE, theta1 = lower, theta2 = upper, theta0 = expectedBias[j], CV = standev, design = "parallel")
	 		temp2[i,j]=test
		}
	}
	temp3<-100*temp2
	sample <- sample/2
	powergraph=cbind(sample,temp3)

	powerPlot <- sub(".html", "powerPlot.png", htmlFile)
	png(powerPlot,width = jpegwidth, height = jpegheight, units="in", res=PlotResolution)

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
	EQPOWERPLOT_ABSOLUTE(power2data,XAxisTitle,MainTitle2,lin_list2,Gr_palette_P)
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
powercurvesactualANOVA<-function(resp, treat) {
	testANOVA<-aov(resp~treat)
	standev<-sqrt(anova(testANOVA)[2,3])
	powercurvesactual(standev)
}

#Special case, no treatment factor selected
powercurvesactualANOVA2<-function(resp) {
	testANOVA<-aov(resp~1)
	standev<-sqrt(anova(testANOVA)[1,3])
	powercurvesactual(standev)
}
 


#===================================================================================================================
#Generating the power curves
#===================================================================================================================
if (valueType == "uservalues") {
	powercurvesactual(SD)
} else if (valueType == "datasetvalues") {
	if(treatment== "NULL") {
		powercurvesactualANOVA2(eval(parse(text = paste("statdata$", response))))
	} else {
		treatTemp<-as.factor(eval(parse(text = paste("statdata$", treatment))))
		statdata<-cbind(statdata,treatTemp)
		powercurvesactualANOVA(eval(parse(text = paste("statdata$", response))), statdata$treatTemp)
	}
}

#===================================================================================================================
#Selected Results
#===================================================================================================================
HTML.title("Selected results", HR=2, align="left")

# Text if Power is selected
if(plotSettingsType=="poweraxis") {
	#Selected results for user defined parameters
	if (valueType == "uservalues") {
		sample<-c(sampleSizeFrom, floor((sampleSizeFrom+sampleSizeTo)/2), sampleSizeTo)
		sample <- sample*2
		for (i in 1:length(sample))  {
			test<-power.TOST(n=sample[i], alpha=sig,   logscale = FALSE, theta1 = lower, theta2 = upper, theta0 = expectedBias[length(expectedBias)], CV = standev, design = "parallel")
			pow<-format(round(100*test,0),nsmall=0)
			text1<-paste("Assuming the significance level is set at ", 100*sig , "%, and the sample size is ", sample[i]/2, ", the power of the experiment for a bias of size ", expectedBias[length(expectedBias)], " is ",pow, "%.  " , sep="")
			HTML(text1, align="left")
		}
	} else if (valueType == "datasetvalues") {
		sample<-c(sampleSizeFrom, floor((sampleSizeFrom+sampleSizeTo)/2), sampleSizeTo)
		sample <- sample*2
		if(treatment== "NULL") {
			testANOVA<-aov(eval(parse(text = paste("statdata$", response, "~1"))))
			standev<-sqrt(anova(testANOVA)[1,3])
		} else {
			treatTemp<-as.factor(eval(parse(text = paste("statdata$", treatment))))
			statdata<-cbind(statdata,treatTemp)
			testANOVA<-aov(eval(parse(text = paste("statdata$", response)))~statdata$treatTemp)
			standev<-sqrt(anova(testANOVA)[2,3])
		}
		for(i in 1:length(sample)) {
			test<-power.TOST(n=sample[i], alpha=sig,   logscale = FALSE, theta1 = lower, theta2 = upper, theta0 = expectedBias[length(expectedBias)], CV = standev, design = "parallel")
			pow<-format(round(100*test,0),nsmall=0)
			text1<-paste("Assuming the significance level is set at ", 100*sig, "%, and the sample size is ", sample[i]/2, ", the power of the experiment for a bias of size ", expectedBias[length(expectedBias)]," is ", pow, "%.  ",sep="")
			HTML(text1, align="left")
		}
	}
} else {
	#Selected results for user defined parameters
	if (valueType == "uservalues") {
		sample <- sample*2
		sample<-c(sampleSizeFrom, floor((sampleSizeFrom+sampleSizeTo)/2), sampleSizeTo)
		for(i in 1:length(sample))  {
			test<-power.TOST(n=sample[i], alpha=sig,   logscale = FALSE, theta1 = lower, theta2 = upper, theta0 = expectedBias[length(expectedBias)], CV = standev, design = "parallel")
			pow<-format(round(100*test,0),nsmall=0)
			text1<-paste("Assuming the significance level is set at ", 100*sig,"%, and the sample size is ", sample[i]/2, ", the power of the experiment for a bias of size ", expectedBias[length(expectedBias)] , " is " , pow , "%.  " , sep="")
			HTML(text1, align="left")
		}
	} else if (valueType == "datasetvalues") {
		sample<-c(plotSettingsFrom, floor((plotSettingsFrom+plotSettingsTo)/2), plotSettingsTo)
		sample <- sample*2
		if(treatment== "NULL") {
			testANOVA<-aov(eval(parse(text = paste("statdata$", response, "~1"))))
			standev<-sqrt(anova(testANOVA)[1,3])
		} else {
			treatTemp<-as.factor(eval(parse(text = paste("statdata$", treatment))))
			statdata<-cbind(statdata,treatTemp)
			testANOVA<-aov(eval(parse(text = paste("statdata$", response)))~statdata$treatTemp)
			standev<-sqrt(anova(testANOVA)[2,3])
		}
		for(i in 1:length(sample))  {
			test<-power.TOST(n=sample[i], alpha=sig,   logscale = FALSE, theta1 = lower, theta2 = upper, theta0 = expectedBias[length(expectedBias)], CV = standev, design = "parallel")
			pow<-format(round(100*test,0),nsmall=0)
			text1<-paste("Assuming the significance level is set at ", 100*sig , "%, and the sample size is ",  sample[i]/2, ", the power of the experiment for a bias of size ", expectedBias[length(expectedBias)], " is ", pow, "%.  ", sep="")
			HTML(text1, align="left")
		}
	}
}
 

HTML.title("Definitions", HR=2, align="left")
HTML("Power: The chance of detecting a statistically significant test result from running an experiment, assuming there is a real biological effect to find.", align="left")
HTML("Significance level: The chance that the experiment will give a false-positive result.", align="left")
HTML("True bias: The true difference between the two groups that you are trying to confirm are equivalent.", align="left")

#===================================================================================================================
#References
#===================================================================================================================
Ref_list <- R_refs()

HTML.title("References", HR=2, align="left")
HTML(Ref_list$IVS_ref, align="left")
HTML(Ref_list$BateClark_ref, align = "left")
#HTML("Harrison, D.A. and Brady, A.R. (2004). Sample size and power calculations using the noncentral t-distribution. The Stata Journal, 4(2), 142-153.", align = "left")

HTML.title("R references", HR=4, align="left")
HTML(Ref_list$R_ref,  align = "left")
HTML(paste(capture.output(print(citation("R2HTML"),bibtex=F))[4], capture.output(print(citation("R2HTML"),bibtex=F))[5], sep = ""),  align="left")

HTML(paste(capture.output(print(citation("PowerTOST"),bibtex=F))[4], capture.output(print(citation("PowerTOST"),bibtex=F))[5], capture.output(print(citation("PowerTOST"),bibtex=F))[6], sep = ""),  align="left")
HTML(paste(capture.output(print(citation("GGally"),bibtex=F))[4], capture.output(print(citation("GGally"),bibtex=F))[5], capture.output(print(citation("GGally"),bibtex=F))[6], capture.output(print(citation("GGally"),bibtex=F))[7], sep = ""),  align="left")
HTML(paste(capture.output(print(citation("RColorBrewer"),bibtex=F))[4], capture.output(print(citation("RColorBrewer"),bibtex=F))[5], sep = ""),  align="left")
HTML(paste(capture.output(print(citation("ggplot2"),bibtex=F))[4], capture.output(print(citation("ggplot2"),bibtex=F))[5], sep=""),  align="left")
HTML(paste(capture.output(print(citation("ggrepel"),bibtex=F))[4], capture.output(print(citation("ggrepel"),bibtex=F))[5], capture.output(print(citation("ggrepel"),bibtex=F))[6], sep = ""),  align="left")
HTML(paste(capture.output(print(citation("reshape"),bibtex=F))[4], capture.output(print(citation("reshape"),bibtex=F))[5], sep = ""),  align="left")
HTML(paste(capture.output(print(citation("plyr"),bibtex=F))[4], capture.output(print(citation("plyr"),bibtex=F))[5], capture.output(print(citation("plyr"),bibtex=F))[6], sep = ""),  align="left")
HTML(paste(capture.output(print(citation("scales"),bibtex=F))[4], capture.output(print(citation("scales"),bibtex=F))[5], capture.output(print(citation("scales"),bibtex=F))[6], sep = ""),  align="left")
HTML(paste(capture.output(print(citation("proto"),bibtex=F))[4], capture.output(print(citation("proto"),bibtex=F))[5], capture.output(print(citation("proto"),bibtex=F))[6], sep = ""),  align="left")
#extrafont_ref  <- capture.output(print(citation("extrafont"),bibtex=F))[4]

#===================================================================================================================
#Show dataset
#===================================================================================================================
if (showdataset == "Y") {
    if (valueType == "datasetvalues") {
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
if (OutputAnalysisOps == "Y") {
	HTML.title("Analysis options", HR=2, align="left")
	if (valueType == "datasetvalues") { 
		HTML(paste("Comparison of Means Power Analysis module used: Dataset based inputs"),  align="left")
		HTML(paste("Response variable: ", meanOrResponse, sep=""),  align="left")
		HTML(paste("Treatment factor: ", varianceTypeOrTreatment, sep=""),  align="left")
		HTML(paste("Control group: ", varianceAmntOrControl, sep=""),  align="left")
	} else {
		HTML(paste("Comparison of Means Power Analysis module used: User based inputs"),  align="left")
		HTML(paste("Group mean: ", meanOrResponse, sep=""),  align="left")

		if (varianceTypeOrTreatment == "StandardDeviation") {
			HTML("Variability estimate entered as: Standard deviation",  align="left")
			HTML(paste("Standard deviation estimate: ", varianceAmntOrControl, sep=""),  align="left")
		} else {
			HTML("Variability estimate entered as: Variance",  align="left")
			HTML(paste("Variance estimate: ", varianceAmntOrControl, sep=""),  align="left")
		}
	}
	HTML(paste("Significance level: ", sig, sep=""), align="left")

	HTML(paste("Type of change investigated: ", EqBtype, sep=""), align="left")
	HTML(paste("Change values investigated: ", expectedBias, sep=""), align="left")
	
	if (plotSettingsType == "poweraxis")	{
		HTML(paste("Power curve plots controlled by: power range"), align="left")
		} else {
		HTML(paste("Power curve plots controlled by: sample size range"), align="left")
		}
	HTML(paste("Plot setting from: ", plotSettingsFrom, sep=""), align="left")
	HTML(paste("Plot setting to: ", plotSettingsTo, sep=""), align="left")
}
print(lower)
print(upper)