#Select dataset type
valueType <- "UserValues"
#valueType <- "DatasetValues"

#===================================================================================================================
#R Libraries

suppressWarnings(library(R2HTML))
if (valueType == "DatasetValues") {
	suppressWarnings(library(dplyr))
	suppressWarnings(library(multcomp))
	suppressWarnings(library(multcompView))
	suppressWarnings(library(car))
	suppressWarnings(library(lsmeans))
}
#===================================================================================================================
# retrieve args

#Copy Args into variables
Args <- commandArgs(TRUE)

if (valueType == "DatasetValues") {
	statdata <- read.csv(Args[3], header=TRUE, sep=",")
	valueType <- Args[4]
	response <- Args[5]
	transformation <- Args[6]
	treatment <- Args[7]
	sig <- as.numeric(Args[8])
	plotSettingsType <- Args[9]
	plotSettingsFrom <- as.numeric(Args[10])
	plotSettingsTo <- as.numeric(Args[11])
	graphTitle <- Args[12]

	statdata_print<-statdata
}

if (valueType == "UserValues") {
	valueType <- Args[4]
	Treatmentmeans <- Args[5]
	Variancetype <- Args[6]
	Variance <- Args[7]
	Standdev <- Args[8]	
	sig <- as.numeric(Args[9])
	plotSettingsType <- Args[10]
	plotSettingsFrom <- as.numeric(Args[11])
	plotSettingsTo <- as.numeric(Args[12])
	graphTitle <- Args[13]
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

#Default Plotting variables set up
powerFrom <- 0
powerTo <- 105
sampleSizeFrom <- 6
sampleSizeTo <- 15

#Working out parameters from User defined parameters

if (valueType == "UserValues") { 
	#Manipulating treatment mean list
	notrs=0

	temptrChanges <-strsplit(Treatmentmeans, ",")
	trlistx <- c("")
	for(i in 1:length(temptrChanges[[1]]))  {
		trlistx [length(trlistx )+1]=(temptrChanges[[1]][i]) 
	}
	trlist <- trlistx[-1]
	ngps<-length(trlist)

	#Generating the Effectsize
	betweenvar <- var(trlist)		

	#Generating variance estimate
	if (Variancetype == "Variance") {
		withinvar <- as.numeric(Variance)
	}
	if (Variancetype == "StandardDeviation") {
		withinvar <- as.numeric(Standdev)^2
	}
}


#Working out parameters from dataset
if(valueType=="DatasetValues") {
	statdata$treatment2 <- as.factor(eval(parse(text = paste("statdata$",treatment))))
	Model <- paste(response , " ~" , "treatment2", sep = "")
	threewayfull<-lm(as.formula(Model), data=statdata, na.action = na.omit)

	#Generate variance estimate
	tempx<-Anova(threewayfull, type=c("III"))[-1,]
	withinvar<- tempx[2,1] / tempx[2,2]

	#Calculate effect variability
	tabs<-lsmeans(threewayfull,eval(parse(text = paste("~","treatment2"))), data=statdata)
	x<-summary(tabs)
	betweenvar <- var(x$lsmean)
	ngps <- dim(x)[1]
}

#Working out the graphical parameters
if(plotSettingsType=="PowerAxis") {
	powerFrom <- plotSettingsFrom 
	powerTo <- plotSettingsTo 
	sampleSizeFrom <- format(round(as.numeric(power.anova.test(groups=ngps, between.var=betweenvar , within.var=withinvar, power=powerFrom/100, sig.level=sig)[2]), 0), nsmall = 0, scientific = FALSE)
	sampleSizeTo <- format(round(as.numeric(power.anova.test(groups=ngps, between.var=betweenvar , within.var=withinvar, power=powerTo/100, sig.level=sig)[2]), 0), nsmall = 0, scientific = FALSE)
} else {
	sampleSizeFrom  <- plotSettingsFrom
	sampleSizeTo   <-  plotSettingsTo
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
Title <-paste(branding, " 'One-way ANOVA' Power Analysis", sep="")

HTML.title(Title, HR = 1, align = "left")
HTML("The power calculations made by InVivoStat assume that the future experimental design involves one treatment factor, with equal group sizes. The data will be analysed using a balanced One-way ANOVA.", align="left")
HTML("The statistical power generated is for the overall ANOVA test (i.e. an overall difference between the group means) .", align="left")

#Bate and Clark comment
HTML(refxx, align="left")

if (valueType=="DatasetValues")  {
	HTML.title("Response and treatment factor", HR = 2, align="left")
	add <- paste ("The response analysed is ", response , ",  with ", treatment , " fitted as the single treatment factor." , sep = "") 
	if (transformation != "None") {
		add<-paste(add, c("The response has been "), transformation, " transformed prior to analysis.", sep="")
	}
	HTML(add, align="left")

	#Testing the degrees of freedom

	if (df.residual(threewayfull)<5) {
		HTML("Warning: Unfortunately the residual degrees of freedom are low (less than 5). This may make the estimation of the underlying variability, and hence the results of the power analysis, unreliable.", align="left")
	}
}

	

HTML.title("Power curve plot", HR = 2, align="left")

#===================================================================================================================
#Power analysis functions

sample <- seq(sampleSizeFrom,sampleSizeTo, 0.01)
temp2<-matrix(1,nrow=length(sample),ncol=1)
for(i in 1:length(sample)) {
	test<-as.numeric(power.anova.test(groups=ngps, between.var=betweenvar , within.var=withinvar, n=sample[i], sig.level=sig)[6])
	temp2[i,1]=test
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
Legendpos<-"none"
Gr_alpha <- 1
Line_type <-Line_type_solid

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
POWERPLOT_ANOVA(XAxisTitle,MainTitle2)
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


#===================================================================================================================
#References
#===================================================================================================================
HTML.title("Selected results", HR=2, align="left")

Effect <- format(round(betweenvar, 3), nsmall = 3, scientific = FALSE)
sampleSizeFrom2 <- as.numeric(sampleSizeFrom)
sampleSizeTo2 <- as.numeric(sampleSizeTo)
midsize<-as.numeric(format(round((sampleSizeTo2 - sampleSizeFrom2)/2 +sampleSizeFrom2), 0), nsmall = 0, scientific = FALSE)


Res1<-format(round(100*as.numeric(power.anova.test(groups=ngps, between.var=betweenvar , within.var=withinvar, n=sampleSizeFrom2, sig.level=sig)[6]) , 2), nsmall = 2, scientific = FALSE)
if (as.numeric(Res1) > 99.99) {
	Res1x <- ">99.99"
} else {
	Res1x <- Res1
}
Res2<-format(round(100*as.numeric(power.anova.test(groups=ngps, between.var=betweenvar , within.var=withinvar, n=midsize, sig.level=sig)[6]) , 2), nsmall = 2, scientific = FALSE)
if (as.numeric(Res2) > 99.99) {
	Res2x <- ">99.99"
} else {
	Res2x <- Res2
}
Res3<-format(round(100*as.numeric(power.anova.test(groups=ngps, between.var=betweenvar , within.var=withinvar, n=sampleSizeTo2, sig.level=sig)[6]) , 2), nsmall = 2, scientific = FALSE)
print(Res3)
if (as.numeric(Res3) > 99.99) {
	Res3x <- ">99.99"
} else {
	Res3x <- Res3
}

text1<-paste("Assuming the significance level is set at ", 100*sig , "%, the number of groups is ", ngps, ", with a sample size of ",  sampleSizeFrom2, " per group, then if the effect size is ", Effect , ", then the statistical power is ", Res1x, "%. " , sep="")
HTML(text1, align="left")
text2<-paste("Assuming the significance level is set at ", 100*sig , "%, the number of groups is ", ngps, ", with a sample size of ",  midsize, " per group, then if the effect size is ", Effect , ", then the statistical power is ", Res2x, "%. " , sep="")
HTML(text2, align="left")
text3<-paste("Assuming the significance level is set at ", 100*sig , "%, the number of groups is ", ngps, ", with a sample size of ",  sampleSizeTo2, " per group, then if the effect size is ", Effect , ", then the statistical power is ", Res3x, "%. " , sep="")
HTML(text3, align="left")

HTML.title("Definitions", HR=2, align="left")
HTML("Power: The chance of achieving a statistically significant test result from running an experiment, assuming there is a real biological effect to find.", align="left")
HTML("Significance level: The chance that the experiment will give a false-positive result.", align="left")

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
#HTML(Ref_list$power_ref,  align = "left")
HTML(Ref_list$dplyr_ref,  align = "left")

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
	HTML(paste("Response variable: ", response, sep=""),  align="left")
	HTML(paste("Treatment variable: ", treatment, sep=""),  align="left")
} else {
	HTML(paste("Analysis method: User based inputs"),  align="left")

	HTML(paste("Treatment means: ", Treatmentmeans, sep=""),  align="left")

	if (Variancetype == "Variance") {
		HTML(paste("Variability estimate type: Variance", sep=""),  align="left")
		HTML(paste("Variance: ", Variance, sep=""),  align="left")
	}
	if (Variancetype == "StandardDeviation") {
		HTML(paste("Variability estimate type: Standard deviation", sep=""),  align="left")
		HTML(paste("Standard deviation: ", Standdev, sep=""),  align="left")
	}
}

if (plotSettingsType == "PowerAxis")	{
	HTML(paste("Power curve plots controlled by power range:"), align="left")
	} else {
	HTML(paste("Power curve plots controlled by sample size range:"), align="left")
	}
HTML(paste("Plot setting from: ", plotSettingsFrom, sep=""), align="left")
HTML(paste("Plot setting to: ", plotSettingsTo, sep=""), align="left")
HTML(paste("Significance level: ", sig, sep=""), align="left")