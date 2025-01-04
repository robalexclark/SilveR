#Select dataset type
#AnalysisType <- "UserValues"
AnalysisType <- "DatasetValues"

#===================================================================================================================
#R Libraries

suppressWarnings(library(R2HTML))
if (AnalysisType == "DatasetValues") {
	suppressWarnings(library(dplyr))
	suppressWarnings(library(multcomp))
	suppressWarnings(library(multcompView))
	suppressWarnings(library(car))
	suppressWarnings(library("emmeans"))
}

#===================================================================================================================
# retrieve args

#Copy Args into variables
Args <- commandArgs(TRUE)

if (AnalysisType == "DatasetValues") {
	statdata <- read.csv(Args[3], header=TRUE, sep=",")
	valueType <- tolower(Args[4])
	response <- Args[5]
	transformation <- tolower(Args[6])
	treatment <- Args[7]
	sig <- as.numeric(Args[8])
	plotSettingsType <- tolower(Args[9])
	plotSettingsFrom <- as.numeric(Args[10])
	plotSettingsTo <- as.numeric(Args[11])
	graphTitle <- Args[12]

	statdata_print<-statdata
}

if (AnalysisType == "UserValues") {
	valueType <- tolower(Args[4])
	Treatmentmeans <- Args[5]
	Variancetype <- tolower(Args[6])
	Variance <- Args[7]
	Standdev <- Args[8]	
	sig <- as.numeric(Args[9])
	plotSettingsType <- tolower(Args[10])
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

if (AnalysisType == "UserValues") { 
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
	if (Variancetype == "variance") {
		withinvar <- as.numeric(Variance)
	}
	if (Variancetype == "standarddeviation") {
		withinvar <- as.numeric(Standdev)^2
	}
}


#Working out parameters from dataset
if(AnalysisType=="DatasetValues") {
	statdata$treatment2 <- as.factor(eval(parse(text = paste("statdata$",treatment))))
	Model <- paste(response , " ~" , "treatment2", sep = "")
	threewayfull<-lm(as.formula(Model), data=statdata, na.action = na.omit)

	#Generate variance estimate
	tempx<-Anova(threewayfull, type=c("III"))[-1,]
	withinvar<- tempx[2,1] / tempx[2,2]

	#Calculate effect variability
	tabs<-emmeans(threewayfull,eval(parse(text = paste("~","treatment2"))), data=statdata)
	x<-summary(tabs)
	betweenvar <- var(x$emmean)
	ngps <- dim(x)[1]
}

#Working out the graphical parameters
if(plotSettingsType=="poweraxis") {
	powerFrom <- plotSettingsFrom 
	powerTo <- plotSettingsTo 
	sampleSizeFrom <- format(floor(as.numeric(power.anova.test(groups=ngps, between.var=betweenvar , within.var=withinvar, power=powerFrom/100, sig.level=sig)[2])), nsmall = 0, scientific = FALSE)
	sampleSizeTo <- format(ceiling(as.numeric(power.anova.test(groups=ngps, between.var=betweenvar , within.var=withinvar, power=powerTo/100, sig.level=sig)[2])), nsmall = 0, scientific = FALSE)
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
XLimLow <- "NULL"
XLimHigh <- "NULL"
YLimLow <- "NULL"
YLimHigh <- "NULL"

#===================================================================================================================
#Output HTML header
Title <-paste(branding, " 'One-way ANOVA' Power Analysis", sep="")

#Software developement version warning
if (Betawarn == "Y") {
	HTML.title("Warning", HR=2, align="left")
	HTML(BetaMessage, align="left")
}

HTML.title(Title, HR = 1, align = "left")
HTML("The power calculations made by InVivoStat assume that the future experimental design involves one treatment factor, with equal group sizes. The data will be analysed using a balanced One-way ANOVA.", align="left")
HTML("The statistical power generated is for the overall ANOVA test (i.e. an overall difference between the group means) .", align="left")

#Bate and Clark comment
HTML(refxx, align="left")

if (AnalysisType=="DatasetValues")  {
	HTML.title("Response and treatment factor", HR = 2, align="left")
	add <- paste ("The response analysed is ", response , ",  with ", treatment , " fitted as the single treatment factor." , sep = "") 
	if (transformation != "none") {
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

powerPlot <- sub(".html", "powerPlot.png", htmlFile)
png(powerPlot,width = jpegwidth, height = jpegheight, units="in", res=PlotResolution)

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

text1<-paste("Assuming the significance level is set at ", 100*sig , "%, the number of groups is ", ngps, ", with a sample size of ",  sampleSizeFrom2, " per group, if the effect size is ", Effect , ", then the statistical power is ", Res1x, "%. " , sep="")
HTML(text1, align="left")
text2<-paste("Assuming the significance level is set at ", 100*sig , "%, the number of groups is ", ngps, ", with a sample size of ",  midsize, " per group, if the effect size is ", Effect , ", then the statistical power is ", Res2x, "%. " , sep="")
HTML(text2, align="left")
text3<-paste("Assuming the significance level is set at ", 100*sig , "%, the number of groups is ", ngps, ", with a sample size of ",  sampleSizeTo2, " per group, if the effect size is ", Effect , ", then the statistical power is ", Res3x, "%. " , sep="")
HTML(text3, align="left")

HTML.title("Definitions", HR=2, align="left")
HTML("Power: The chance of detecting a statistically significant test result from running an experiment, assuming there is a real biological effect to find.", align="left")
HTML("Significance level: The chance that the experiment will give a false-positive result.", align="left")

#===================================================================================================================
#References
#===================================================================================================================
Ref_list <- R_refs()

HTML.title("References", HR=2, align="left")
HTML(Ref_list$IVS_ref, align="left")
HTML(Ref_list$BateClark_ref, align = "left")
#HTML("Harrison, D.A. and Brady, A.R. (2004). Sample size and power calculations using the noncentral t-distribution. The Stata Journal, 4(2), 142-153.", align = "left")

HTML.title("R references", HR=4, align="left")
HTML(Ref_list$R_ref , align="left")
HTML(reference("R2HTML"))
HTML(reference("GGally"))
HTML(reference("RColorBrewer"))
HTML(reference("ggplot2"))
HTML(reference("ggrepel"))
HTML(reference("reshape"))
HTML(reference("plyr"))
HTML(reference("scales"))
HTML(reference("proto"))

if (AnalysisType == "DatasetValues") {
	HTML(reference("dplyr"))
	HTML(reference("multcomp"))
	HTML(reference("multcompView"))
	HTML(reference("car"))
	HTML(reference("emmeans"))
}
#===================================================================================================================
#Show dataset
#===================================================================================================================
if (showdataset == "Y") {
    if (AnalysisType == "DatasetValues") {
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
	if (AnalysisType == "DatasetValues") { 
		HTML(paste("One-way ANOVA Power Analysis module used: Dataset based inputs"),  align="left")
		HTML(paste("Response variable: ", response, sep=""),  align="left")

		if (transformation != "none") {
			HTML(paste("Response variable transformation: ", transformation, sep = ""),  align="left")
		}
		HTML(paste("Treatment factor: ", treatment, sep=""),  align="left")
	} else {
		HTML(paste("One-way ANOVA Power Analysis module used: User based inputs"),  align="left")

		HTML(paste("Group means: ", Treatmentmeans, sep=""),  align="left")

		if (Variancetype == "variance") {
			HTML("Variability estimate type: Variance",  align="left")
			HTML(paste("Variance estimate: ", Variance, sep=""),  align="left")
		}
		if (Variancetype == "standarddeviation") {
			HTML("Variability estimate type: Standard deviation",  align="left")
			HTML(paste("Standard deviation estimate: ", Standdev, sep=""),  align="left")
		}
	}

	HTML(paste("Significance level: ", sig, sep=""), align="left")

	if (plotSettingsType == "poweraxis")	{
		HTML(paste("Power curve plots defined by: power range"), align="left")
		} else {
		HTML(paste("Power curve plots defined by: sample size range"), align="left")
		}
	HTML(paste("Plot setting from: ", plotSettingsFrom, sep=""), align="left")
	HTML(paste("Plot setting to: ", plotSettingsTo, sep=""), align="left")
}
