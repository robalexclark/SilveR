#R Libraries

suppressWarnings(library(car))
suppressWarnings(library(R2HTML))
suppressWarnings(library(ROCR))
#===================================================================================================================
# retrieve args
Args <- commandArgs(TRUE)

#Read in data
statdata <- read.csv(Args[3], header=TRUE, sep=",")

#Copy Args
#model <- Args[4]
#scatterplotModel <- as.formula(Args[5])
#positiveResult <- Args[6]
#covariates <- Args[7]
#covariateTransform <- Args[8]
#FirstCatFactor <- Args[8]
#treatFactors <- Args[9]
#contFactors <- Args[10]
#contFactorTransform <- Args[11]
#blockFactors <- Args[12]
#showANOVA <- Args[13]
#tableOfOverallEffectTests <- Args[13]
#modelPredictionAssessment <- Args[14]
#rocCurve <- Args[15]
#sig <- 1 - as.numeric(Args[16])
#sig2 <- 1 - as.numeric(Args[16])/2
#effectModel <- as.formula(Args[16])
#effectModel2 <- Args[16]
#selectedEffect <- Args[17]
#showLSMeans <- Args[18]
#allPairwiseTest <- Args[19]
#backToControlTest <- Args[20]
#cntrlGroup <- Args[21]
#genpvals <- Args[22]

model <- Args[4]
scatterplotModel <- as.formula(Args[5])
positiveResult <- Args[6]
covariates <- Args[7]
covariateTransform <- Args[8]
treatFactors <- Args[9]
contFactors <- Args[10]
contFactorTransform <- Args[11]
blockFactors <- Args[12]
tableOfOverallEffectTests <- Args[13]
oddsRatio <- Args[14]
modelPredictionAssessment <- Args[15]
plotOfModelPredicted <- Args[16]
tableOfModelPredictions <- Args[17]
goodnessOfFitTest <- Args[18]
rocCurve <- Args[19]
sig <- 1 - as.numeric(Args[20])
sig2 <- 1 - as.numeric(Args[20])/2

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
#V3.2 STB OCT2015
set.seed(5041975)

#STB 14OCT2015
#Set contrast options for Marginal overall tests
options(contrasts=c(unordered="contr.sum", ordered="contr.poly"))

#Reconstruct the model statement
model2 <- paste( "temp_IVS_response ~ " ,  unlist(strsplit(model ,"~"))[2])

#Response manipulations
resp     <- unlist(strsplit(model ,"~"))[1] #get the response variable from the main model

#Format numeric response range to be 0 to 1 
if (is.numeric(eval(parse(text = paste("statdata$",resp))))==TRUE){
	range01 <- function(x){(x-min(x))/(max(x)-min(x))}
	statdata$temp_IVS_response<-range01(eval(parse(text = paste("statdata$",resp))))
}

if (is.numeric(eval(parse(text = paste("statdata$",resp))))==FALSE){
	statdata$temp_IVS_response_temp <- eval(parse(text = paste("statdata$",resp)))
	statdata$temp_IVS_response <- 0
	for (i in 1:length(statdata$temp_IVS_response_temp)){
		if (statdata$temp_IVS_response_temp[i] == levels(statdata$temp_IVS_response_temp)[2]) {
			statdata$temp_IVS_response[i] = 1
		}
	}
}

#Re-ordering the factor levels for the log odds ratio calculation
if (levels(as.factor(eval(parse(text = paste("statdata$",resp)))))[1] != positiveResult) {
	  statdata$temp_IVS_response = -1*statdata$temp_IVS_response + 1
}

#Number of factors in Selected effect
#factno<-length(unique (strsplit(selectedEffect, "*",fixed = TRUE)[[1]]))

#calculating number of block factors
noblockfactors=0
if (blockFactors !="NULL") {
	tempblockChanges <-strsplit(blockFactors, ",")
	blocklistx <- c("")
	for(i in 1:length(tempblockChanges[[1]]))  {
		blocklistx [length(blocklistx )+1]=(tempblockChanges[[1]][i]) 
	}
	blocklist <- blocklistx[-1]
	noblockfactors<-length(blocklist)
}

#calculating number of continuous factors
nocontfactors=0
if (contFactors !="NULL") {
	tempcontChanges <-strsplit(contFactors, ",")
	txtexpectedcontChanges <- c("")
	for(i in 1:length(tempcontChanges[[1]]))  { 
		txtexpectedcontChanges [length(txtexpectedcontChanges )+1]=(tempcontChanges[[1]][i]) 
	}
	nocontfactors<-length(txtexpectedcontChanges)-1
	ContinuousList <- txtexpectedcontChanges[-1]
}

#calculating number of treatment factors
notreatfactors = 0
if (treatFactors !="NULL") {
	tempChanges <-strsplit(treatFactors, ",")
	treatlistx <- c("")
	for(i in 1:length(tempChanges[[1]]))  { 
		treatlistx [length(treatlistx )+1]=(tempChanges[[1]][i]) 
	}
	notreatfactors<-length(treatlistx)-1
	treatlist <- treatlistx[-1]
}

#calculating number of covariates
nocovars=0
if (covariates !="NULL") {
	tempcovChanges <-strsplit(covariates, ",")
	txtexpectedcovChanges <- c("")
	for(i in 1:length(tempcovChanges[[1]]))  {
		txtexpectedcovChanges [length(txtexpectedcovChanges )+1]=(tempcovChanges[[1]][i]) 
	}
	covlist <- txtexpectedcovChanges[-1]
	nocovars<-length(covlist)
}

#Removing illegal characters
#selectedEffect<- namereplace2(selectedEffect)
#selectedEffectx<- namereplace(selectedEffect)

#replace illegal characters in variable names
YAxisTitle <-resp
if(nocovars > 0) {
	XAxisTitleCov<-covlist
}

#replace illegal characters in variable names
for (i in 1:10) {
	YAxisTitle<-namereplace(YAxisTitle)

	if(nocovars > 0) {
		for (i in 1: nocovars) {
			XAxisTitleCov[i]<-namereplace(XAxisTitleCov[i])
		}
	}
}
LS_YAxisTitle<-YAxisTitle

#STB June 2015 - Taking a copy of the dataset
statdata_temp <-statdata

#Graphics parameter setup
graphdata<-statdata
#if (nocovars > 0) {
#	Gr_palette<-palette_FUN(FirstCatFactor)
#} 
Line_size2 <- Line_size
Labelz_IVS_ <- "N"
ReferenceLine <- "NULL"

#===================================================================================================================
#Titles and description
#===================================================================================================================
#Output HTML header
Title <-paste(branding, " Logistic Regression Analysis", sep="")

HTML.title(Title, HR = 1, align = "left")

#Software developement version warning
if (Betawarn == "Y") {
	HTML.title("Warning", HR=2, align="left")
	HTML(BetaMessage, align="left")
}

title<-c("Response")
if(nocovars > 0) {
	title<-paste(title, " and covariate", sep="")
}
HTML.title(title, HR=2, align="left")

add<-paste(c("The  "), resp, " response is currently being analysed by the Logistic Regression Analysis module", sep="")


if(nocovars > 0) {
	if (nocovars == 1) {
		add<-paste(add, ", with ", covlist[1] , " fitted as a covariate.", sep="")
	} 
	if (nocovars == 2) {
		add<-paste(add, ", with ", covlist[1] , " and ", covlist[2] ," fitted as covariates.", sep="")
	}
	if (nocovars > 2) {	
		add<-paste(add, ", with ", sep="")	
		for (i in 1: (nocovars -2)) {
		add <- paste (add, covlist[i],  ", " , sep="")
		}
		add<-paste(add, covlist[(nocovars -1)] , " and ", covlist[nocovars] , " fitted as covariates.", sep="")
	}
} else {
	add<-paste(add, ".", sep="")
}

if (nocovars > 0 && covariateTransform != "None") {
	if (nocovars == 1) {
		add<-paste(add, c("The covariate has been "), covariateTransform, " transformed prior to analysis.", sep="")
	} else {
		add<-paste(add, c("The covariates have been "), covariateTransform, " transformed prior to analysis.", sep="")
	}
}
HTML(add, align="left")


#===================================================================================================================
#Plotting Predictions
#===================================================================================================================
HTML.title("Plot of model predictions", HR=2, align="left")

threewayfull<-glm(model2, data=statdata, family = binomial(link="logit"), na.action = na.omit)

if(nocontfactors > 1) {
	HTML("This option is only available if the model contains a single continuous factor.", align="left")
}

if(nocontfactors == 1) {
	#Warning messages when there are covariates or blocking factors 
	if (noblockfactors>0 && nocovars>0 ) {
		HTML("As you have selected blocking factor(s) and covariate(s) no plot has been generated.", align="left")
	}
	if (noblockfactors==0 && nocovars>0 ) {
		HTML("As you have selected covariate(s) no plot has been generated.", align="left")
	}
	if (noblockfactors>0 && nocovars==0 ) {
		HTML("As you have selected blocking factor(s) no plot has been generated.", align="left")
	}
}




#Predictions when there is a single continuous factor only in the model
if(noblockfactors==0 && nocovars==0 && notreatfactors == 0 && nocontfactors == 1)
{
	#Calculating the range of the continuous factor
	Minrange = suppressWarnings(min(eval(parse(text = paste("statdata$", ContinuousList[1]))), na.rm = TRUE))
	Maxrange = suppressWarnings(max(eval(parse(text = paste("statdata$", ContinuousList[1]))), na.rm = TRUE))

	#Generating the prediction grid
	newdata<- c(1:(100*(Maxrange - Minrange)))
	id<-1
	for (i in Minrange:(Maxrange-1)) {
		for (j in 1:100) {
			newdata[id+j] <- (Minrange+i-1)+ j/100
		}
		id =id +100
	}
	newdata<-data.frame(newdata)
	colnames(newdata) <- ContinuousList[1]

	#Generating the predictions
	newdataPreds <- predict(threewayfull, newdata = newdata, type = "response")
	newdataPreds<- cbind( newdataPreds, newdata)

	#Generating the plot y-axis label
	if (is.numeric(eval(parse(text = paste("statdata$",resp))))==TRUE){
		if (as.numeric(positiveResult) == min(eval(parse(text = paste("statdata$",resp))), na.rm = TRUE)) {
			labelsz<-c(max(eval(parse(text = paste("statdata$",resp))), na.rm = TRUE), min(eval(parse(text = paste("statdata$",resp))), na.rm = TRUE))
		} else {
			labelsz<-c(min(eval(parse(text = paste("statdata$",resp))), na.rm = TRUE), max(eval(parse(text = paste("statdata$",resp))), na.rm = TRUE))
		}
	}

	if (is.numeric(eval(parse(text = paste("statdata$",resp))))==FALSE){
		labels <- c(levels(eval(parse(text = paste("statdata$",resp)))))
		positiveResult2<- c(positiveResult)
		labels2<- labels[!labels %in% positiveResult2]
		labelsz<-c(labels2, positiveResult)
	}

	#Plotting the data
	scatterPlot <- sub(".html", "scatterPlot.png", htmlFile)
	png(scatterPlot,width = jpegwidth, height = jpegheight, units="in", res=PlotResolution)
	
	#STB July2013
	plotFilepdf1 <- sub(".html", "scatterPlot.pdf", htmlFile)
	dev.control("enable") 

	#Graphical parameters
	graphdata<-statdata
	graphdata$yvarrr_IVS <- statdata$temp_IVS_response
	graphdata$xvarrr_IVS <- eval(parse(text = paste("statdata$",ContinuousList[1])))
	newdataPreds$yvarrr_IVS <- newdataPreds$newdataPreds
	newdataPreds$xvarrr_IVS <- eval(parse(text = paste("newdataPreds$",ContinuousList[1])))

	XAxisTitle <- ContinuousList[1]
	YAxisTitle <- resp
	MainTitle2 <- ""
	w_Gr_jitscat <- 0
	h_Gr_jitscat <-  0
	infiniteslope <- "Y"
	ReferenceLine <- "NULL"
	Line_type <- Line_type_solid	

	#GGPLOT2 code
	LogisticplotNonCat()
	
	void <- HTMLInsertGraph(GraphFileName=sub("[A-Z0-9a-z,:,\\\\]*App_Data[\\\\]","", scatterPlot), Align="centre")

	#STB July2013
	if (pdfout=="Y") {
		pdf(file=sub("[A-Z0-9a-z,:,\\\\]*App_Data[\\\\]","", plotFilepdf1), height = pdfheight, width = pdfwidth) 
		dev.set(2) 
		dev.copy(which=3) 
		dev.off(2)
		dev.off(3)
		pdfFile_1<-sub("[A-Z0-9a-z,:,\\\\]*App_Data[\\\\]","",plotFilepdf1)
		linkToPdf1 <- paste ("<a href=\"",pdfFile_1,"\">Click here to view the PDF of the scatterplot</a>", sep = "")
		HTML(linkToPdf1)
	}
}





















#Predictions when there is a single continuous factor only in the model
if(noblockfactors==0 && nocovars==0 && notreatfactors ==1 && nocontfactors == 1)
{

trlevs <- length(levels(eval(parse(text = paste("statdata$", treatlist[1])))))

	#Calculating the range of the continuous factor
	Minrange = suppressWarnings(min(eval(parse(text = paste("statdata$", ContinuousList[1]))), na.rm = TRUE))
	Maxrange = suppressWarnings(max(eval(parse(text = paste("statdata$", ContinuousList[1]))), na.rm = TRUE))

	#Generating the prediction grid
	newdata<- c(1:(100*(Maxrange - Minrange)))
	id<-1
	for (i in Minrange:(Maxrange-1)) {
		for (j in 1:100) {
			newdata[id+j] <- (Minrange+i-1)+ j/100
		}
		id =id +100
	}

	newdata<-data.frame(newdata)
	newdata$treat_IVS_treat<- levels(eval(parse(text = paste("statdata$", treatlist[1]))))[1]
	tempdata<-newdata
	for (i in 2:trlevs){
		temptempdata<-tempdata
		temptempdata$treat_IVS_treat<- levels(eval(parse(text = paste("statdata$", treatlist[1]))))[i]
		newdata <- rbind(newdata, temptempdata)
	}
	colnames(newdata) <- c(ContinuousList[1], treatlist[1])

	#Generating the predictions
	newdataPreds <- predict(threewayfull, newdata = newdata, type = "response")
	newdataPreds<- cbind( newdataPreds, newdata)

	#Generating the plot y-axis label
	if (is.numeric(eval(parse(text = paste("statdata$",resp))))==TRUE){
		if (as.numeric(positiveResult) == min(eval(parse(text = paste("statdata$",resp))), na.rm = TRUE)) {
			labelsz<-c(max(eval(parse(text = paste("statdata$",resp))), na.rm = TRUE), min(eval(parse(text = paste("statdata$",resp))), na.rm = TRUE))
		} else {
			labelsz<-c(min(eval(parse(text = paste("statdata$",resp))), na.rm = TRUE), max(eval(parse(text = paste("statdata$",resp))), na.rm = TRUE))
		}
	}

	if (is.numeric(eval(parse(text = paste("statdata$",resp))))==FALSE){
		labels <- c(levels(eval(parse(text = paste("statdata$",resp)))))
		positiveResult2<- c(positiveResult)
		labels2<- labels[!labels %in% positiveResult2]
		labelsz<-c(labels2, positiveResult)
	}

	#Plotting the data
	scatterPlot <- sub(".html", "scatterPlot.png", htmlFile)
	png(scatterPlot,width = jpegwidth, height = jpegheight, units="in", res=PlotResolution)
	
	#STB July2013
	plotFilepdf1 <- sub(".html", "scatterPlot.pdf", htmlFile)
	dev.control("enable") 

	#Graphical parameters
	graphdata<-statdata
	graphdata$yvarrr_IVS <- statdata$temp_IVS_response
	graphdata$xvarrr_IVS <- eval(parse(text = paste("statdata$",ContinuousList[1])))
	graphdata$l_l<- eval(parse(text = paste("graphdata$", treatlist[1])))

	newdataPreds$yvarrr_IVS <- newdataPreds$newdataPreds
	newdataPreds$xvarrr_IVS <- eval(parse(text = paste("newdataPreds$",ContinuousList[1])))
	newdataPreds$l_l<- eval(parse(text = paste("newdataPreds$", treatlist[1])))

	XAxisTitle <- ContinuousList[1]
	YAxisTitle <- resp
	MainTitle2 <- ""
	w_Gr_jitscat <- 0
	h_Gr_jitscat <-  0
	infiniteslope <- "Y"
	ReferenceLine <- "NULL"
	Line_type <- Line_type_solid	

	#GGPLOT2 code
	LogisticplotOneCat()
	
	void <- HTMLInsertGraph(GraphFileName=sub("[A-Z0-9a-z,:,\\\\]*App_Data[\\\\]","", scatterPlot), Align="centre")

	#STB July2013
	if (pdfout=="Y") {
		pdf(file=sub("[A-Z0-9a-z,:,\\\\]*App_Data[\\\\]","", plotFilepdf1), height = pdfheight, width = pdfwidth) 
		dev.set(2) 
		dev.copy(which=3) 
		dev.off(2)
		dev.off(3)
		pdfFile_1<-sub("[A-Z0-9a-z,:,\\\\]*App_Data[\\\\]","",plotFilepdf1)
		linkToPdf1 <- paste ("<a href=\"",pdfFile_1,"\">Click here to view the PDF of the scatterplot</a>", sep = "")
		HTML(linkToPdf1)
	}
}
































#===================================================================================================================
#ANOVA table
#===================================================================================================================
#Analysis call
threewayfull<-glm(model2, data=statdata, family = binomial(link="logit"), na.action = na.omit)

#Testing the degrees of freedom
if (df.residual(threewayfull)<5) {
	HTML.title("Warning", HR=2, align="left")
	HTML("Unfortunately the residual degrees of freedom are low (less than 5). This may make the estimation of the underlying variability, and hence the results of the statistical tests, unreliable. This can be caused by attempting to fit too many factors, and their interactions, in the statistical model. Where appropriate we recommend you fit some of the 'Treatment' factors as 'Other design' factors. This will remove their interactions from the statistical model and therefore increase the residual degrees of freedom.", align="left")
}

#Overall effects Table
if(tableOfOverallEffectTests=="Y") {
	HTML.title("Analysis of deviance table", HR=2, align="left")

	# Stop process if residual sums of squares are too close to zero
	if (deviance(threewayfull)<sqrt(.Machine$double.eps)) {
		HTML("The Residual Sums of Squares is close to zero indicating the model is overfitted (too many terms are included in the model). The model should be simplified in order to generate statistical test results." , align="left")
		quit()
	}

	#STB Sept 2014 - Marginal sums of square to tie in with RM (also message below and covariate ANOVA above)	

#	temp<-Anova(threewayfull, type=c("III"), test="Wald")[-1,]
	temp<-Anova(threewayfull, type=c("III"), test="Wald")
	col1<-format(round(temp[2], 3), nsmall=3, scientific=FALSE)
	col2<-format(round(temp[3], 4), nsmall=4, scientific=FALSE)

	source<-rownames(temp)

	# Residual label in ANOVA

	#STB March 2014 - Replacing : with * in ANOVA table
	for (q in 1:notreatfactors) {
		source<-sub(":"," * ", source) 
	}	
	ivsanova<-cbind(source, temp[1], col1,col2)

	#STB May 2012 capitals changed
	head<-c("Effect", "Degrees of freedom", "Chi-square", "p-value")
	colnames(ivsanova)<-head

	for (i in 1:(dim(ivsanova)[1])) {
		if (temp[i,3]<0.0001) {
			#STB March 2011 formatting p-values p<0.0001
			#ivsanova[i,5]<-0.0001
			ivsanova[i,4]=format(round(0.0001, 4), nsmall=4, scientific=FALSE)
			ivsanova[i,4]<- paste("<",ivsanova[i,4])
		}
	}

	HTML(ivsanova, classfirstline="second", align="left", row.names = "FALSE")

	if(nocovars > 0) {
		#STB Error spotted:
		HTML("Comment: Analysis of deviance table calculated using a Type III model fit, see Armitage et al. (2001).", align="left")
	} else {
		HTML("Comment: Analysis of deviance table calculated using a Type III model fit, see Armitage et al. (2001).", align="left")
	}



	add<-paste(c("Conclusion"))
	inte<-1
	for(i in 1:(dim(ivsanova)[1])) {
		if (ivsanova[i,4]<= (1-sig)) {
			if (inte==1) {
				inte<-inte+1
				add<-paste(add, ": There is a statistically significant overall difference between the levels of ", rownames(ivsanova)[i], sep="")
			} else {
				inte<-inte+1
				add<-paste(add, ", ", rownames(ivsanova)[i],  sep="")
			}
		} 
	}

	if (inte==1) {
		if (dim(ivsanova)[1]>2) {
			add<-paste(add, ": There are no statistically significant overall differences between the levels of any of the terms in the Analysis of deviance table", sep="")
		} 
		if (dim(ivsanova)[1]<=2) {
				add<-paste(add, ": There is no statistically significant overall difference between the levels of any of the terms in the Analysis of deviance table", sep="")
		}
	} 
	add<-paste(add, ". ", sep="")
	HTML(add, align="left")
}

#===================================================================================================================
#Generating confusion matrix
#===================================================================================================================
if (modelPredictionAssessment == "Y") {
	HTML.title("Model prediction assessment", HR=2, align="left")

	threewayfull.probs <- predict(threewayfull,type = "response")
	threewayfull.pred <- data.frame(ifelse(threewayfull.probs > 0.5, "1", "0"))
	names(threewayfull.pred)<-c("Prediction")
	tempdata<- cbind(statdata, threewayfull.pred)
	temp2 <- table(tempdata$temp_IVS_response, tempdata$Prediction)

	#Print Results
	levs<- levels(as.factor(eval(parse(text = paste("statdata$",resp)))))

	if (levs[1] == positiveResult) {
		nameC1 = paste("Predicted response = ", levs[1], sep="")
		nameC2 = paste("Predicted response = ", levs[2], sep="")
		nameR1 = paste("Observed level = ", levs[1], sep="")
		nameR2 = paste("Observed level = ", levs[2], sep="")
	} else {
		nameC1 = paste("Predicted response = ", levs[2], sep="")
		nameC2 = paste("Predicted response = ", levs[1], sep="")
		nameR1 = paste("Observed level = ", levs[2], sep="")
		nameR2 = paste("Observed level = ", levs[1], sep="")
	}
	colnames(temp2) <- c(nameC1, nameC2)
	rownames(temp2) <- c(nameR1, nameR2)
	HTML(temp2, classfirstline="second", align="left", row.names = "FALSE")

	#Generating the conclusion
	temp3 <- round(100*mean(tempdata$Prediction == tempdata$temp_IVS_response), 2)
	conclusion <- paste("The model correctly classifies the responses in ", temp3, "% of the time." , sep = "")
	HTML(conclusion, align="left")
}

#===================================================================================================================
#ROC Curve
#===================================================================================================================
if (rocCurve == "Y") {
	HTML.title("Receiver operating characteristic (ROC) curve", HR=2, align="left")

	p <- threewayfull.probs <- predict(threewayfull,type = "response")
	pr <- prediction(threewayfull.probs, eval(parse(text = paste("statdata$",resp))))
	perf <- performance(pr, measure = "tpr", x.measure = "fpr")

	pf = data.frame(FPR=perf@x.values[[1]],TPR=perf@y.values[[1]])

	#Area Under the Curve
	auc = round(as.numeric(performance (pr, "auc")@y.values),2)
	result = paste("AUC = ", round(auc,2))

	#ROC plot
	ROCPlot <- sub(".html", "ROCPlot.png", htmlFile)
	png(ROCPlot,width = jpegwidth, height = jpegheight, units="in", res=PlotResolution)
	
	plotFilepdf1 <- sub(".html", "ROCPlot.pdf", htmlFile)
	dev.control("enable") 
	#GGPLOT2 code
	ROCPLOT()
	void <- HTMLInsertGraph(GraphFileName=sub("[A-Z0-9a-z,:,\\\\]*App_Data[\\\\]","", ROCPlot), Align="centre")

	#Conclusions
	conclusion1 <- c("The ROC curve (Receiver Operating Characteristic) is a way to assess how good the model is at identifying (i) true positive results (or successes), measured by 'sensitivity' - the probability of predicting a true positive/success and (ii) true negative results (or failures), measured by 1-'specificity' - the probability of predicting a false positive/success. The best decision rule is high on sensitivity and low on 1-specificity. The ROC curve illustrates this.")
	
	conclusion2 <- c("A model that predicts no better than chance will have an ROC curve that closely follows the diagonal dotted line. The further the curve is above the diagonal line, the better the model is at discriminating between positives and negatives.")

	HTML(conclusion1, align="left")
	HTML(conclusion2, align="left")

	#Area Under the Curve text
	AUCresult<- paste("The ROC curve can also be summarised by the area under curve (AUC). The closer the AUC is to 1, 
	the better the model is at discriminating between positives and negatives. In this case the AUC of the curve is ", auc, ".", sep = "") 
	HTML(AUCresult, align="left")
} 


#===================================================================================================================
#Goodness of fit test
#===================================================================================================================
HTML.title("Hosmer-Lemeshow goodness of fit test", HR=2, align="left")

temprepsp<-c(statdata$temp_IVS_response)

#Generating the quantiles
neffects <-sum(temp[1])-1
vect <- vector(mode="numeric", length=neffects)
for (i in 1:neffects) {
	vect[i] <- (0.9 - 0.1)/(neffects+1)*i+0.1
}

#Generate model predictions
pihat <- threewayfull$fitted
brks <- c(0,quantile(pihat, probs=vect),1)
brksL<-length(brks)-1
brkschi<-length(brks)-2

#categorise the observations according to deciles of the predicted probabilities
pihatcat <- cut(pihat, breaks=brks, labels=FALSE)

#Cycle through the groups 1 to x, counting the number observed 0s and 1s, and calculating the expected number of 0s and 1s
#To calculate the latter, we find the mean of the predicted probabilities in each group, and multiply this by the group size, which here is 10:
meanprobs <- array(0, dim=c(brksL,2))
expevents <- array(0, dim=c(brksL,2))
obsevents <- array(0, dim=c(brksL,2))

for (i in 1:brksL) {
	meanprobs[i,1] <- mean(pihat[pihatcat==i])
	expevents[i,1] <- sum(pihatcat==i)*meanprobs[i,1]
	obsevents[i,1] <- sum(temprepsp[pihatcat==i])

	meanprobs[i,2] <- mean(1-pihat[pihatcat==i])
	expevents[i,2] <- sum(pihatcat==i)*meanprobs[i,2]
	obsevents[i,2] <- sum(temprepsp[pihatcat==i])
}


#we can calculate the Hosmer-Lemeshow test statistic by the sum of (observed-expected)^2/expected across the 10x2 cells of the table
hosmerlemechi <- sum((obsevents-expevents)^2 / expevents)
col1a<-format(round(hosmerlemechi, 2), nsmall=2, scientific=FALSE)

hosmerlemepval <- pchisq(hosmerlemechi, df=brkschi, lower.tail=FALSE) 
col2a=format(round(hosmerlemepval, 4), nsmall=4, scientific=FALSE)
if (hosmerlemepval<0.0001) {
	col2a<- "<0.0001"
}

hosmerlemetable <- data.frame(t(c("Test result", col1a, brkschi, col2a)))
colnames(hosmerlemetable)<-c(" ", "Chi-sq value", "Degrees of freedom", "p-value")
HTML(hosmerlemetable, classfirstline="second", align="left", row.names = "FALSE")

add<-paste(c("Conclusion"))
if (hosmerlemepval<= (1-sig)) {
	add<-paste(add, ": The Hosmer-Lemeshow goodness of fit test is a statistically significant at the ", 1-sig , " level (", col2a ,"), indicating the model does not fit well.", sep="")
} 
if (hosmerlemepval > (1-sig)) {
	add<-paste(add, ": The Hosmer-Lemeshow goodness of fit test is not statistically significant at the ", 1-sig , " level (", col2a ,"), indicating there is no evidence of poor fit.", sep="")
} 

HTML(add, align="left")

comment <- c("Note: The number of bins used in this caluclation is two more than the number of model parameters estimated.")
HTML(comment, align="left")
 
#===================================================================================================================
#Odds ratio
#===================================================================================================================
HTML.title("Odds ratio", HR=2, align="left")

names <- rownames(data.frame(coef(threewayfull)))
names<-names[-1]
oddsR<- data.frame(exp(cbind(OR = coef(threewayfull), confint(threewayfull, level=(sig)))))
oddsR <- oddsR[-1,]
oddsR<-cbind(names, oddsR)
colnames(oddsR) <- c("Parameter", "Odds ratio", paste("Lower ",(sig*100),"% CI",sep=""), paste("Upper ",(sig*100),"% CI",sep=""))
HTML(oddsR, classfirstline="second", align="left", row.names = "FALSE")

note<- c("Note: Confidence intervals are based on the profiled log-likelihood function.")
HTML(note, align="left")
 
note2<- c("To interpret these results: For a one-unit increase in the parameter, the odds ratio (given as a fold-change) 
indicates the corresponding increase/decrease in the probability that the response gives a 'positive result'." )
HTML(note2, align="left")



#===================================================================================================================
#Predictions
#===================================================================================================================
HTML.title("Model predictions", HR=2, align="left")

predictions<- predict(threewayfull, type = "response")
Observations <- c(1:dim(statdata)[1])

predicts <- cbind(Observations, statdata, predictions)
colnames(predicts) <- c("Observation number", colnames(statdata), "Prediction")
if (is.numeric(eval(parse(text = paste("statdata$",resp))))==FALSE){
	predicts2 = subset(predicts, select = -c(temp_IVS_response_temp, temp_IVS_response) )
}
if (is.numeric(eval(parse(text = paste("statdata$",resp))))==TRUE){
	predicts2 = subset(predicts, select = -c(temp_IVS_response) )
}
HTML(predicts2, classfirstline="second", align="left", row.names = "FALSE")



#===================================================================================================================
#Analysis description
#===================================================================================================================
HTML.title("Analysis description", HR=2, align="left")

add<-c("The data were analysed using a ")
if (nocontfactors==1 && notreatfactors==0)  {
		add<-paste(add, "1-way logistic regression approach, with ", ContinuousList, " as the continuous factor", sep="")
} else {
	nofact<-nocontfactors + notreatfactors
	add<-paste(add, nofact, sep="")
	if(nocovars != 0) {
		add<-paste(add, "-way logistic regression approach, with", sep="")
	} else {
		add<-paste(add, "-way logistic regression approach, with", sep="")
	}
	for (i in 1:nocontfactors) {
		if (i<nocontfactors-1)	{
			add<-paste(add, " ", ContinuousList[i], ",",sep="")
		} else 	if (i<nocontfactors) {
			add<-paste(add,  " ", ContinuousList[i], " and", sep="")
		} else if (i==nocontfactors) {
			if (nocontfactors==1) {
				add<-paste(add, " ", ContinuousList[i], " as the continuous factor", sep="")
			} else {
				add<-paste(add, " ", ContinuousList[i], " as the continuous factors", sep="")
			}
		}
	}

	if (notreatfactors !=0) {
		for (i in 1:notreatfactors) {
			if (i<notreatfactors-1)	{
				add<-paste(add, ", ", treatlist[i],  sep="")
			} else 	if (i<notreatfactors) {
				add<-paste(add, ", ", treatlist[i], " and", sep="")
			} else if (i==notreatfactors) {
				if (notreatfactors==1) {
					add<-paste(add, ", ", treatlist[i], " as the categorical factor", sep="")
				} else {
					add<-paste(add, " ", treatlist[i], " as the categorical factors", sep="")
				}
			}
		}
	}
}
if (noblockfactors != 0 && nocovars != 0)  {
	add<-paste(add, ", ", sep="")
} else if (noblockfactors==1 && blocklist != "NULL" && nocovars == 0)  {
	add<-paste(add, " and ", sep="")
} 
	
if (noblockfactors==1 && blocklist != "NULL")  {
	add<-paste(add, blocklist, " as a blocking factor", sep="")
} else {
	if(noblockfactors>1)  {
		if (covariatelist == "NULL") {
			add<-paste(add, " and ", sep="")
		}
		for (i in 1:noblockfactors) {
			if (i<noblockfactors-1) {
				add<-paste(add, blocklistsep[i], ", ", sep="")
			} else	if (i<noblockfactors) {
				add<-paste(add, blocklistsep[i], " and ", sep="")
			} else if (i==noblockfactors) {
				add<-paste(add, blocklistsep[i], sep="")
			}
		}
		add<-paste(add, " as the blocking factors", sep="")
	}
}
if (nocovars == 0) {
	add<-paste(add, ". ", sep="")
} else {
	add<-paste(add, " and ",  sep="")
	if (nocovlist == 1) {
		add<-paste(add, covlistsep[1], " as the covariate.", sep="")
	} else {
		for (i in 1:nocovlist) {
			if (i<nocovlist-1)	{
				add<-paste(add, covlistsep[i], ", ", sep="")
			} else 	if (i<nocovlist) {
				add<-paste(add, covlistsep[i], " and ", sep="")
			} else if (i==nocovlist) {
				add<-paste(add, covlistsep[i], " as the covariates.", sep="")
			}
		}
	}
}

if (nocovars != 0 && covariateTransform != "None") {
	if (nocovlist == 1) {
		add<-paste(add, c("The covariate has been "), covariateTransform, " transformed prior to analysis.", sep="")
	} else {
		add<-paste(add, c("The covariates have been "), covariateTransform, " transformed prior to analysis.", sep="")
	}
}

add<-paste(add, " A positive result has been defined by the ", positiveResult , " response.", sep = "")
HTML(add, align="left")




#===================================================================================================================
#References
#===================================================================================================================
Ref_list<-R_refs()

#Bate and Clark comment
HTML(refxx, align="left")	

if (UpdateIVS == "N") {
	HTML.title("Statistical references", HR=2, align="left")
}
if (UpdateIVS == "Y") {
	HTML.title("References", HR=2, align="left")
	HTML(Ref_list$IVS_ref, align="left")
}
HTML(Ref_list$BateClark_ref, align="left")

#if(showANOVA=="Y") {
#	HTML("Armitage, P., Matthews, J.N.S. and Berry, G. (2001). Statistical Methods in Medical Research. 4th edition; John Wiley & Sons. New York.", align="left")
#}
#if(showCoefficients == "Y") {
#	HTML("Chambers, J.M. and Hastie, T.J. (1992). Statistical Models in S. Wadsworth and Brooks-Cole advanced books and software.", align="left")
#}
#if(covariatelist != "NULL") {
#	HTML("Morris, T.R. (1999). Experimental Design and Analysis in Animal Sciences. CABI publishing. Wallingford, Oxon (UK).", align="left")
#}

if (UpdateIVS == "N") {
	HTML.title("R references", HR=2, align="left")
}
if (UpdateIVS == "Y") {
	HTML.title("R references", HR=4, align="left")
}
HTML(Ref_list$R_ref ,  align="left")
HTML(Ref_list$GGally_ref,  align="left")
HTML(Ref_list$RColorBrewers_ref,  align="left")
HTML(Ref_list$GGPLot2_ref,  align="left")
HTML(Ref_list$ggrepel_ref,  align="left")
HTML(Ref_list$reshape_ref,  align="left")
HTML(Ref_list$plyr_ref,  align="left")
HTML(Ref_list$scales_ref,  align="left")
HTML(Ref_list$car_ref,  align="left")
HTML(Ref_list$R2HTML_ref,  align="left")
HTML(Ref_list$PROTO_ref,  align="left")
HTML(Ref_list$ROCRref,  align="left")

#===================================================================================================================
#Show dataset
#===================================================================================================================


if (showdataset=="Y")
{
	statdata_temp<-subset(statdata_temp, select = -c(temp_IVS_response_temp, temp_IVS_response))

	observ <- data.frame(c(1:dim(statdata_temp)[1]))
	colnames(observ) <- c("Observation")
	statdata_temp2 <- cbind(observ, statdata_temp)

	HTML.title("Analysis dataset", HR = 2, align = "left")
    	HTML(statdata_temp2, classfirstline = "second", align = "left", row.names = "FALSE")

}

#===================================================================================================================
#Show arguments - to be sorted
#===================================================================================================================
if (OutputAnalysisOps == "Y") {
	HTML.title("Analysis options", HR=2, align="left")

	HTML(paste("Response variable: ", resp, sep=""), align="left")
	
	if (responseTransform != "None") {
		HTML(paste("Response variable transformation: ", responseTransform, sep=""), align="left")
	}
	
	HTML(paste("Treatment factor(s): ", treatFactors, sep=""), align="left")
	
	if (blockFactors != "NULL") {
		HTML(paste("Other design (block) factor(s): ", blockFactors, sep=""), align="left")
	}

	if(FirstCatFactor != "NULL") {
		HTML(paste("Covariate(s): ", covariates, sep=""), align="left")
	}

	if (FirstCatFactor != "NULL" ) {
		HTML(paste("Primary factor: ", FirstCatFactor, sep=""), align="left")
	}

	if (FirstCatFactor != "NULL" && covariateTransform != "None") {
		HTML(paste("Covariate(s) transformation: ", covariateTransform, sep=""), align="left")
	}

	HTML(paste("Output ANOVA table (Y/N): ", showANOVA, sep=""), align="left")
	HTML(paste("Output residuals vs. predicted plot (Y/N): ", showPRPlot, sep=""), align="left")
	HTML(paste("Output normal probability plot (Y/N): ", showNormPlot, sep=""), align="left")
	HTML(paste("Significance level: ", 1-sig, sep=""), align="left")

	if (showLSMeans != "N" && (Args[19] != "NULL" | backToControlTest != "NULL" ) ) {
		HTML(paste("Selected effect (for pairwise mean comparisons): ", selectedEffect, sep=""), align="left")
	}

	HTML(paste("Output least square (predicted) means (Y/N): ", showLSMeans, sep=""), align="left")
	

	if (Args[19] != "NULL") {
		HTML(paste("All pairwise comparisons procedure: ", allPairwiseTest, sep=""), align="left")
	}

	if (backToControlTest != "NULL" && backToControlTest != "none") {
		HTML(paste("Comparisons back to control procedure: ", backToControlTest, sep=""), align="left")
	}

	if (backToControlTest == "none") {
		HTML(paste("Comparisons back to control procedure: Unadjusted (LSD)"), align="left")
	}

	if ( backToControlTest != "NULL" ) {
		HTML(paste("Control group: ", cntrlGroup, sep=""), align="left")
	}
}

#===================================================================================================================
#===================================================================================================================
#===================================================================================================================
#===================================================================================================================
#===================================================================================================================
#===================================================================================================================
quit()








