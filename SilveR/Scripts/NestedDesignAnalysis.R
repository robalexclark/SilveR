#===================================================================================================================
#R Libraries

suppressWarnings(library(R2HTML))
suppressWarnings(library(nlme))

#===================================================================================================================
# retrieve args
Args <- commandArgs(TRUE)

#Read in data
statdata <- read.csv(Args[3], header=TRUE, sep=",")

#Copy Args
responseTransform <- Args[5]
covariateTransform <- Args[6]
treatments <- Args[7]
otherFactors <- Args[8]
covariate <- Args[9]
sigLevel <- Args[10]
randomEffect1 <- Args[11]
randomEffect2 <- Args[12]
randomEffect3 <- Args[13]
randomEffect4 <- Args[14]
repList1 <- Args[15]
repList2 <- Args[16]
repList3 <- Args[17]
repList4 <- Args[18]

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
#Functions
sipowerttest<-function(delta,sd,n,sig.level) {
	NCP<-delta/(sqrt(sd*2/n))
	crit1<-qt((1-sig.level/2),df=(2*n-2))
	crit2<-qt((1-(1-sig.level/2)),df=(2*n-2))
	suppressWarnings(power<-1-pt(crit1,(2*n-2),NCP)+pt(crit2,(2*n-2),NCP))
}

#===================================================================================================================
#Model and parameter setup - check and balances
#===================================================================================================================
#Graphical parameter setup
ReferenceLine <- "NULL"

#STB Edits so that covariate can be non-null but treatments are null -  17Oct2010
if (treatments=="NULL" && covariate == "NULL")	{
	nonetreat<-eval(parse(text = paste(Args[4], "1")))
	model <- as.formula(nonetreat)
	model2 <- nonetreat
} else {
	model <- as.formula(Args[4])
	model2 <- Args[4]
} 

# Making sure random factors are factor
if (randomEffect1 != "NULL") {
	statdata$rEffect1IVS <-as.factor(eval(parse(text = paste("statdata$", randomEffect1))))
}
if (randomEffect2 != "NULL") {
	statdata$rEffect2IVS  <-as.factor(eval(parse(text = paste("statdata$", randomEffect2))))
}
if (randomEffect3 != "NULL") {
	statdata$rEffect3IVS  <-as.factor(eval(parse(text = paste("statdata$", randomEffect3))))
}
if (randomEffect4 != "NULL") {
	statdata$rEffect4IVS  <-as.factor(eval(parse(text = paste("statdata$", randomEffect4))))
}

siglevel<-as.numeric(sigLevel)
statdata$interactionEffect<-NULL

#user defined replication 
if (repList1!="NULL") {
	tempChanges <-strsplit(repList1, ",")
	userlist1 <- numeric(0)
	for(i in 1:length(tempChanges[[1]])) { userlist1 [length(userlist1)+1] = as.numeric(tempChanges[[1]][i]) } 
}
if (repList2!="NULL") {
	tempChanges <-strsplit(repList2, ",")
	userlist2 <- numeric(0)
	for(i in 1:length(tempChanges[[1]])) { userlist2 [length(userlist2)+1] = as.numeric(tempChanges[[1]][i]) } 
}
if (repList3!="NULL") {
	tempChanges <-strsplit(repList3, ",")
	userlist3 <- numeric(0)
	for(i in 1:length(tempChanges[[1]])) { userlist3 [length(userlist3)+1] = as.numeric(tempChanges[[1]][i]) } 
}
if (repList4!="NULL") {
	tempChanges <-strsplit(repList4, ",")
	userlist4 <- numeric(0)
	for(i in 1:length(tempChanges[[1]])) { userlist4 [length(userlist4)+1] = as.numeric(tempChanges[[1]][i]) } 
}

#Calculating the highest order interaction
if (treatments!="NULL") {
	treatments1 <- unlist(strsplit(treatments,","))
	lentr<-length(treatments1)
	firsttreat<-treatments1[1]
	statdata$alltreat <-eval(parse(text = paste("statdata$", firsttreat)))

	if (lentr >= 2) {
		for (i in 2:lentr) {
			secondtreat<-treatments1[i]
			statdata$alltreat2 <-eval(parse(text = paste("statdata$", secondtreat)))
			alltreat<-paste(statdata$alltreat, " ",statdata$alltreat2)
			statdata$alltreat<-NULL
			alltreat<-as.factor(alltreat)
			statdata<-cbind(statdata,alltreat)
		}
	}
} else {
	statdata$alltreat<- 1
}

#Define random model
if (randomEffect1 != "NULL" && randomEffect2 != "NULL" && randomEffect3 == "NULL" && randomEffect4 == "NULL") {
	randmodel<-as.formula(~1|rEffect1IVS)
}
if (randomEffect1 != "NULL" && randomEffect2 != "NULL" && randomEffect3 != "NULL" && randomEffect4 == "NULL") {
	randmodel<-as.formula(~1|rEffect1IVS/rEffect2IVS)
}
if (randomEffect1 != "NULL" && randomEffect2 != "NULL" && randomEffect3 != "NULL" && randomEffect4 != "NULL") {
	randmodel<-as.formula(~1|rEffect1IVS/rEffect2IVS/rEffect3IVS)
}

#Parameters set up
resp <- unlist(strsplit(Args[4],"~"))[1] #get the response variable from the main model
graphTitle0 = "Power curves for original design"
graphTitle1 = "Power curves for predicted design"

#Checking the conditions
#Only run analysis with two or more random effects
if(randomEffect1=="NULL" || randomEffect2=="NULL") {
	HTML("Unfortunately you need two nested random factors in order to use this module.", align="left")
	quit()
} 

if(repList1=="NULL" && repList2 =="NULL" && repList3 =="NULL" && repList4 =="NULL" ) {
	HTML.title("You need to define at least one list of replications of the levels of a random factor to proceed with this analysis.", HR=0, align="left")
	quit()
} 

#Check lowest order Random effect is defines observational unit
len<-dim(statdata)[1]
if (randomEffect1 != "NULL" && randomEffect2 != "NULL" && randomEffect3 == "NULL" && randomEffect4 == "NULL") {
	len2<-length(unique(eval(parse(text = paste("statdata$", randomEffect2)))))
}
if (randomEffect1 != "NULL" && randomEffect2 != "NULL" && randomEffect3 != "NULL" && randomEffect4 == "NULL") {
	len2<-length(unique(eval(parse(text = paste("statdata$", randomEffect3)))))
}
if (randomEffect1 != "NULL" && randomEffect2 != "NULL" && randomEffect3 != "NULL" && randomEffect4 != "NULL") {
	len2<-length(unique(eval(parse(text = paste("statdata$", randomEffect4)))))
}
if(len2<len) {
	HTML("The levels of the lowest order random factor do not define the observations units (rows of the dataset). Please include an extra random factor that does.", align="left")
	quit()
}


#===================================================================================================================
#Output HTML header
#===================================================================================================================
Title <-paste(branding, " Nested Design Analysis", sep="")
HTML.title(Title, HR = 1, align = "left")

#Response
title<-c("Response")
if(covariate != "NULL") {
	title<-paste(title, " and covariate", sep="")
}
HTML.title(title, HR=2, align="left")

add<-paste(c("The  "), resp, " response is currently being analysed by the Nested Design Analysis module", sep="")
if(covariate != "NULL") {
	add<-paste(add, c(", with  "), covariate, " fitted as a covariate.", sep="")
} else {
	add<-paste(add, ".", sep="")
}
HTML(add, align="left")

if (responseTransform != "None") {
	add2<-paste(c("The response has been "), responseTransform, " transformed prior to analysis.",sep="")
	HTML(add2, align="left")
}

if (covariateTransform != "None") {
	add3<-paste(c("The covariate has been "), covariateTransform, " transformed prior to analysis.", sep="")
	HTML(add3, align="left")
}

#Method
HTML.title("Methodology", HR=2, align="left")
HTML("This module uses the estimated variance components from the original dataset to predict the hypothetical statistical power than can be achieved by varying the replication of the levels of the random factors in the experimental design, see Snedecor and Cochran (1989, p239)." , align="left")

#Bate and Clark comment
HTML(refxx, align="left")	
#Warning
HTML.title("Warning", HR=2, align="left")
HTML("Warning: This module is currently under construction, care should be taken when considering the results. The results have not been verified.", align="left")

#===================================================================================================================
#Model fitting
#===================================================================================================================
modelfit<-lme(model, data=statdata, random=randmodel) 

#Definedrandom effects
cov<-VarCorr(modelfit) 

if (randomEffect1 != "NULL" && randomEffect2 != "NULL" && randomEffect3 == "NULL" && randomEffect4 == "NULL") {
	cov11<-as.numeric(cov[1])
	cov12<-as.numeric(cov[2])
	covs<-c(cov11, cov12)
	reffects<-c(randomEffect1, randomEffect2)
}
if (randomEffect1 != "NULL" && randomEffect2 != "NULL" && randomEffect3 != "NULL" && randomEffect4 == "NULL") {
	cov21<-as.numeric(cov[2])
	cov22<-as.numeric(cov[4])
	cov23<-as.numeric(cov[5])
	covs<-c(cov21, cov22, cov23)
	reffects<-c(randomEffect1, randomEffect2, randomEffect3)
}
if (randomEffect1 != "NULL" && randomEffect2 != "NULL" && randomEffect3 != "NULL" && randomEffect4 != "NULL") {
	cov31<-as.numeric(cov[2])
	cov32<-as.numeric(cov[4])
	cov33<-as.numeric(cov[6])
	cov34<-as.numeric(cov[7])
	covs<-c(cov31, cov32, cov33, cov34)
	reffects<-c(randomEffect1, randomEffect2, randomEffect3, randomEffect4)
}

#===================================================================================================================
#Variance components table
#===================================================================================================================
HTML.title("Table of estimated variance components", HR=2, align="left")

table<-matrix(nrow=1, ncol=(length(reffects)+1))
table[1,1] <- "Variance Components"
for ( i in 1:length(reffects)) {
	table[1,(i+1)]=format(round(covs[i], 4), nsmall=4, scientific=FALSE)
}
vars<-data.frame(table)
colnames(vars)<-c( "  ",  reffects)
HTML(vars, classfirstline="second", align="left", row.names = "FALSE")

#===================================================================================================================
#Default Replication results
#===================================================================================================================
#Random1 replication
length1<-length(unique(levels(as.factor(statdata$alltreat))))
vectorN1 <-c(1:length1)
for (i in 1:length1) {
	sub<-subset(statdata, statdata$alltreat == unique(levels(as.factor(statdata$alltreat)))[i])
	sub1<-data.frame(sub)
	vectorN1[i]=dim(sub1)[1]
}
repz1<-as.numeric(mean(vectorN1))

#Random2 replication
if (randomEffect1 != "NULL") {
	length2<-length(unique(levels(as.factor(statdata$rEffect1IVS))))
	vectorN2 <-c(1:length2)
	for (i in 1:length2) {
		sub<-subset(statdata, statdata$rEffect1IVS == unique(levels(as.factor(statdata$rEffect1IVS)))[i])
		sub2<-data.frame(sub)
		vectorN2[i]=dim(sub2)[1]
	}
	repz2<-as.numeric(mean(vectorN2))
}

#Random3 replication
if (randomEffect2 != "NULL") {
	length3<-length(unique(levels(as.factor(statdata$rEffect2IVS))))
	vectorN3 <-c(1:length3)
	for (i in 1:length3) {
		sub<-subset(statdata, statdata$rEffect2IVS == unique(levels(as.factor(statdata$rEffect2IVS)))[i])
		sub3<-data.frame(sub)
		vectorN3[i]=dim(sub3)[1]
	}
	repz3<-as.numeric(mean(vectorN3))
}

#Random4 replication
if (randomEffect3 != "NULL") {
	length4<-length(unique(levels(as.factor(statdata$rEffect3IVS))))
	vectorN4 <-c(1:length4)
	for (i in 1:length4) {
		sub<-subset(statdata, statdata$rEffect3IVS == unique(levels(as.factor(statdata$rEffect3IVS)))[i])
		sub4<-data.frame(sub)
		vectorN4[i]=dim(sub4)[1]
	}
	repz4<-as.numeric(mean(vectorN4))
}

#Calculating replication for 4 random effects
if (randomEffect1 != "NULL" && randomEffect2 != "NULL" && randomEffect3 != "NULL" && randomEffect4 != "NULL") {
	rep1<-as.numeric(format(round(repz1/repz2, 0), nsmall=0, scientific=FALSE))
	rep2<-as.numeric(format(round(repz2/repz3, 0), nsmall=0, scientific=FALSE))
	rep3<-as.numeric(format(round(repz3/repz4, 0), nsmall=0, scientific=FALSE))
	rep0<-repz4
	reps3<-c(rep1, rep2, rep3, rep0)
	reps<-reps3
}

#Calculating replication for 3 random effects
if (randomEffect1 != "NULL" && randomEffect2 != "NULL" && randomEffect3 != "NULL" && randomEffect4 == "NULL") {
	rep1<-as.numeric(format(round(repz1/repz2, 0), nsmall=0, scientific=FALSE))
	rep2<-as.numeric(format(round(repz2/repz3, 0), nsmall=0, scientific=FALSE))
	rep0<-repz3
	reps2<-c(rep1, rep2, rep0)
	reps<-reps2
}

#Calculating replication for 2 random effects
if (randomEffect1 != "NULL" && randomEffect2 != "NULL" && randomEffect3 == "NULL" && randomEffect4 == "NULL") {
	rep1<-as.numeric(format(round(repz1/repz2, 0), nsmall=0, scientific=FALSE))
	rep0<-repz2
	reps1<-c(rep1, rep0)
	reps<-reps1
}

# Calculating the Actual EMS
# One defined random effects
if (randomEffect1 != "NULL" && randomEffect2 != "NULL" && randomEffect3 == "NULL" && randomEffect4 == "NULL") {
	EMS<- cov12 + rep0*cov11
}

# Two defined random effects
if (randomEffect1 != "NULL" && randomEffect2 != "NULL" && randomEffect3 != "NULL" && randomEffect4 == "NULL") {
	EMS<-cov23 + rep0*cov22 + rep0*rep2*cov21
}

# Three defined random effects
if (randomEffect1 != "NULL" && randomEffect2 != "NULL" && randomEffect3 != "NULL" && randomEffect4 != "NULL") {
	EMS<- cov34 + rep0*cov33 + rep0*rep3*cov32 + rep0*rep3*rep2*cov31
}

table<-matrix(nrow=1, ncol=(length(reffects)+1))
table[1,1]<-"Factor replication"
for ( i in 1:length(reffects))
{
	table[1,(i+1)]=format(round(reps[i], 0), nsmall=0, scientific=FALSE)
}
vars<-data.frame(table)
colnames(vars)<-c(" ",  reffects)

HTML.title("Table of average replication in the original design", HR=2, align="left")
HTML("The following replication of the levels of the random factors are used in the power analyses, unless alternative replications are defined by the user. They are an estimate of the replication used within the original design.", align="left")
HTML(vars, classfirstline="second", align="left", row.names = "FALSE")

#===================================================================================================================
#Expected Mean Squares
#===================================================================================================================
HTML.title("Estimated between subject variance", HR=2, align="left")

EMSz<-format(round(EMS, 2), nsmall=2, scientific=FALSE)
Comment<-paste("The estimate of the between subject variance is ", EMSz, ". This is computed using the variance components, calculated from the data generated by the original design, and the average replication of the original design.", sep="")
HTML.title(Comment, HR=0, align="left")

#===================================================================================================================
#Power curve plot for an approximate original design
#===================================================================================================================
HTML.title("Power curve for an approximate original design", HR=2, align="left")

EMS2<-sqrt(EMS)
diffmin<-as.numeric(power.t.test(n=repz1, sd=EMS2, sig.level=siglevel, power=0.05, delta=NULL)[2])
diffmax<-as.numeric(power.t.test(n=repz1, sd=EMS2, sig.level=siglevel, power=0.99, delta=NULL)[2])

#STB July 2013 - increasing smoothness of curves
diffgap<-(diffmax-diffmin)/100
diffs <-c(1:100)
diffs[1]=diffmin

for (d in 2:100) {
	diffs[d] = diffmin+(d-1)*diffgap
}

# actual change using imputted values
legtitle<-c("Replication of subjects = ")
legtitle2<-rep(legtitle,length(rep1))
legtitle3<-paste(legtitle2, rep1)
sample<-repz1
temp1<-diffs
temp2<-matrix(1,nrow=length(diffs),ncol=length(sample))
for (j in 1:length(sample)) {
	for(i in 1:length(diffs)) {
		test<-sipowerttest(n=sample[j], delta=diffs[i], sd=EMS, sig.level=siglevel)
 		temp2[i,j]=test
	}
}
temp3<-100*temp2
powergraph=cbind(diffs,temp3)

powerPlot <- sub(".html", "powerPlot.jpg", htmlFile)
jpeg(powerPlot,width = jpegwidth, height = jpegheight, quality = 100)

#STB July2013
plotFilepdf1 <- sub(".html", "powerPlot.pdf", htmlFile)
dev.control("enable") 

#Graphics parameters
XAxisTitle <- "Biological difference"
MainTitle2 <-""
powerFrom <-0
powerTo <-105
Legendpos<-"none"
Gr_alpha <- 1
Line_type <-Line_type_solid

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
power2data <-cbind(power2data, diffs)

#GGPLOT2
POWERPLOT_ORIGINAL()

void <- HTMLInsertGraph(GraphFileName=sub("[A-Z0-9a-z,:,\\\\]*App_Data[\\\\]","", powerPlot), Align="left")

#STB July2013
if (pdfout=="Y") {
	pdf(file=sub("[A-Z0-9a-z,:,\\\\]*App_Data[\\\\]","", plotFilepdf1), height = pdfheight, width = pdfwidth) 
	dev.set(2) 
	dev.copy(which=3) 
	dev.off(2)
	dev.off(3)
	pdfFile_1<-sub("[A-Z0-9a-z,:,\\\\]*App_Data[\\\\]","",plotFilepdf1)
	linkToPdf1 <- paste ("<a href=\"",pdfFile_1,"\">Click here to view the PDF of the plot of the power curve for the original design</a>", sep = "")
	HTML(linkToPdf1)
}

#===================================================================================================================
#===================================================================================================================
# Calculating the EMS's
#===================================================================================================================
#===================================================================================================================

#===================================================================================================================
# Two random effects
#===================================================================================================================
if (randomEffect1 != "NULL" && randomEffect2 != "NULL" && randomEffect3 == "NULL" && randomEffect4 == "NULL" && repList1!="NULL") {
	title<-paste("Power curves for a design varying the replication of ", randomEffect1, sep="")
	HTML.title(title, HR=2, align="left")

	EMSlist11<-numeric(0)
	samplesizelist11<-numeric(0)
	for(i in 1:length(userlist1)) {
		EMSlist11[i]<-cov12+ rep0*cov11
		samplesizelist11[i]<-rep0*userlist1[i]
	}

	legtitle<-paste("Replication of ",randomEffect1, "= ")
	legtitle2<-rep(legtitle,length(userlist1))
	legtitle3<-paste(legtitle2, userlist1)
	temp<-matrix(1,nrow=length(diffs),ncol=length(samplesizelist11))
	for (j in 1:length(samplesizelist11)) {
		for(i in 1:length(diffs)) {
			test<-sipowerttest(n=samplesizelist11[j], delta=diffs[i], sd=EMSlist11[j], sig.level=siglevel)
	 		temp[i,j]=test
		}
	}
	temp2<-100*temp
	powergraph=cbind(diffs,temp2)

	plot1 <- sub(".html", "plot1.jpg", htmlFile)
	jpeg(plot1,width = jpegwidth, height = jpegheight, quality = 100)

	#STB July2013
	plotFilepdf2 <- sub(".html", "plot1.pdf", htmlFile)
	dev.control("enable") 

	#Graphics parameters
	XAxisTitle <- "Biological difference"
	MainTitle2 <-""
	powerFrom <-0
	powerTo <-105
	namez <- paste("Replication of ", randomEffect1 , ": ", sep = "")

	#Generating the variable list for the lines - they are labelled V1 up to Vn
	lin_no<-dim(temp2)[2]
	lin_list<-c(2:(lin_no+1))
	lin_list2<-paste("V",lin_list,sep = "")

	gr_temp<-data.frame(cbind(diffs, temp2))
	graphdata<-melt(data=gr_temp, id=c("diffs"))

	#Colour palette definition
	temp<-IVS_F_cpplot_colour("variable")
	Gr_palette_A<-temp$Gr_palette_A
	Gr_line<-temp$Gr_line
	Gr_fill <- temp$Gr_fill	

	userlistgr<- userlist1
	randomEffect1a<-namereplace(randomEffect1)

	#GGPLOT2 code
	POWERPLOT_NEW(randomEffect1a)

	void <- HTMLInsertGraph(GraphFileName=sub("[A-Z0-9a-z,:,\\\\]*App_Data[\\\\]","", plot1), Align="left")

	#STB July2013
	if (pdfout=="Y") {
		pdf(file=sub("[A-Z0-9a-z,:,\\\\]*App_Data[\\\\]","", plotFilepdf2), height = pdfheight, width = pdfwidth) 
		dev.set(2) 
		dev.copy(which=3) 
		dev.off(2)
		dev.off(3)
		pdfFile_2<-sub("[A-Z0-9a-z,:,\\\\]*App_Data[\\\\]","",plotFilepdf2)
		linkToPdf2 <- paste ("<a href=\"",pdfFile_2,"\">Click here to view the PDF of the plot of the power curves</a>", sep = "")
		HTML(linkToPdf2)
	}

	Comment1<-paste("These power curves assume the replication of the levels of the random factor ", randomEffect2, " is ", rep0, ".", sep="") 
	HTML(Comment1, align="left")
}

if (randomEffect1 != "NULL" && randomEffect2 != "NULL" && randomEffect3 == "NULL" && randomEffect4 == "NULL" && repList2!="NULL") {
	title<-paste("Power curves for a design varying the replication of ", randomEffect2, sep="")
	HTML.title(title, HR=2, align="left")

	EMSlist12<-numeric(0)
	samplesizelist12<-numeric(0)
	for(i in 1:length(userlist2)) {
		EMSlist12[i]<-cov12 + userlist2[i]*cov11
		samplesizelist12[i]<-rep1*userlist2[i]
	}

	legtitle<-paste("Replication of ",randomEffect2 , "= ")
	legtitle2<-rep(legtitle,length(userlist2))
	legtitle3<-paste(legtitle2, userlist2)
	temp<-matrix(1,nrow=length(diffs),ncol=length(samplesizelist12))
	for (j in 1:length(samplesizelist12)) {
		for(i in 1:length(diffs)) {
			test<-sipowerttest(n=samplesizelist12[j], delta=diffs[i], sd=EMSlist12[j], sig.level=siglevel)
	 		temp[i,j]=test
		}
	}
	temp2<-100*temp
	powergraph=cbind(diffs,temp2)

	plot2 <- sub(".html", "plot2.jpg", htmlFile)
	jpeg(plot2,width = jpegwidth, height = jpegheight, quality = 100)

	#STB July2013
	plotFilepdf3 <- sub(".html", "plot2.pdf", htmlFile)
	dev.control("enable") 

	#Graphics parameters
	XAxisTitle <- "Biological difference"
	MainTitle2 <-""
	powerFrom <-0
	powerTo <-105
	namez <- paste("Replication of ", randomEffect2 , ": ", sep = "")


	#Generating the variable list for the lines - they are labelled V1 up to Vn
	lin_no<-dim(temp2)[2]
	lin_list<-c(2:(lin_no+1))
	lin_list2<-paste("V",lin_list,sep = "")

	gr_temp<-data.frame(cbind(diffs, temp2))
	graphdata<-melt(data=gr_temp, id=c("diffs"))

	#Colour palette definition
	temp<-IVS_F_cpplot_colour("variable")
	Gr_palette_A<-temp$Gr_palette_A
	Gr_line<-temp$Gr_line
	Gr_fill <- temp$Gr_fill	
	
	userlistgr<- userlist2
	randomEffect2a<-namereplace(randomEffect2)

	#GGPLOT2 code
	POWERPLOT_NEW(randomEffect2a)

	void <- HTMLInsertGraph(GraphFileName=sub("[A-Z0-9a-z,:,\\\\]*App_Data[\\\\]","", plot2), Align="left")

	#STB July2013
	if (pdfout=="Y") {
		pdf(file=sub("[A-Z0-9a-z,:,\\\\]*App_Data[\\\\]","", plotFilepdf3), height = pdfheight, width = pdfwidth) 
		dev.set(2) 
		dev.copy(which=3) 
		dev.off(2)
		dev.off(3)
		pdfFile_3<-sub("[A-Z0-9a-z,:,\\\\]*App_Data[\\\\]","",plotFilepdf3)
		linkToPdf3 <- paste ("<a href=\"",pdfFile_3,"\">Click here to view the PDF of the plot of the power curves</a>", sep = "")
		HTML(linkToPdf3)
	}

	title<-paste("These power curves are for a design varying the replication of ", randomEffect2, " within the levels of ", randomEffect1, ".", sep="")
	HTML(title, align="left")
	Comment2<-paste("These power curves assume the replication of the levels of the random factor ", randomEffect1, " is ", rep1, ".", sep="") 
	HTML(Comment2, align="left")
}

if (randomEffect1 != "NULL" && randomEffect2 != "NULL" && randomEffect3 == "NULL" && randomEffect4 == "NULL" && repList1!="NULL" && repList2!="NULL") {
	l1<- length(userlist1) 
	l2<- length(userlist2)

	if (l1 ==l2) {
		title<-paste("Power curves for a design varying the replication of both random effects simultaneously", sep="")
		HTML.title(title, HR=2, align="left")
	
		EMSlist12<-numeric(0)
		samplesizelist12<-numeric(0)
		for(i in 1:length(userlist2)) {
			EMSlist12[i]<-cov12 + userlist2[i]*cov11
			samplesizelist12[i]<-userlist1[i]*userlist2[i]
		}
	
		legtitle<-paste("Replication of",randomEffect1, "- ")
		legtitle2<-rep(legtitle,length(userlist1))
		legtitle3<-paste(", replication of ",randomEffect2, "- ")
		legtitle4<-rep(legtitle3,length(userlist1))
		legtitle3<-paste(legtitle2, userlist1,  legtitle4, userlist2)

		temp<-matrix(1,nrow=length(diffs),ncol=length(samplesizelist12))
		for (j in 1:length(samplesizelist12)) {
			for(i in 1:length(diffs)) {
				test<-sipowerttest(n=samplesizelist12[j], delta=diffs[i], sd=EMSlist12[j], sig.level=siglevel)
		 		temp[i,j]=test
			}
		}
		temp2<-100*temp
		powergraph=cbind(diffs,temp2)

		plot12 <- sub(".html", "plot12.jpg", htmlFile)
		jpeg(plot12,width = jpegwidth, height = jpegheight, quality = 100)

		#STB July2013
		plotFilepdf4 <- sub(".html", "plot12.pdf", htmlFile)
		dev.control("enable") 

		#Graphics parameters
		XAxisTitle <- "Biological difference"
		MainTitle2 <-""
		powerFrom <-0
		powerTo <-105

		userlistx<-numeric(0)
		for(i in 1:length(userlist2)) {
			userlistx[i]<- paste(userlist1[i], ",", userlist2[i], sep="")
		}
		namez <- paste("Replication of: \n", randomEffect1, ", ", randomEffect2 , " ", sep = "")

		#Generating the variable list for the lines - they are labelled V1 up to Vn
		lin_no<-dim(temp2)[2]
		lin_list<-c(2:(lin_no+1))
		lin_list2<-paste("V",lin_list,sep = "")

		gr_temp<-data.frame(cbind(diffs, temp2))
		graphdata<-melt(data=gr_temp, id=c("diffs"))

		#Colour palette definition
		temp<-IVS_F_cpplot_colour("variable")
		Gr_palette_A<-temp$Gr_palette_A
		Gr_line<-temp$Gr_line
		Gr_fill <- temp$Gr_fill	
		userlistgr<- userlistx

		testy <- paste(randomEffect1, ", ", randomEffect2 , " ", sep = "")
		testya<-namereplace(testy)
	
		#GGPLOT2 code
		POWERPLOT_NEW(testya)
		void <- HTMLInsertGraph(GraphFileName=sub("[A-Z0-9a-z,:,\\\\]*App_Data[\\\\]","", plot12), Align="left")

		#STB July2013
		if (pdfout=="Y") {
			pdf(file=sub("[A-Z0-9a-z,:,\\\\]*App_Data[\\\\]","", plotFilepdf4), height = pdfheight, width = pdfwidth) 
			dev.set(2) 
			dev.copy(which=3) 
			dev.off(2)
			dev.off(3)
			pdfFile_4<-sub("[A-Z0-9a-z,:,\\\\]*App_Data[\\\\]","",plotFilepdf4)
			linkToPdf4 <- paste ("<a href=\"",pdfFile_4,"\">Click here to view the PDF of the plot of the power curves</a>", sep = "")
			HTML(linkToPdf4)
		}
	}
}

#===================================================================================================================
#===================================================================================================================
# Three random effects
#===================================================================================================================
#===================================================================================================================
if (randomEffect1 != "NULL" && randomEffect2 != "NULL" && randomEffect3 != "NULL" && randomEffect4 == "NULL" && repList1!="NULL") {
	title<-paste("Power curves for a design varying the replication of ", randomEffect1)
	HTML.title(title, HR=2, align="left")

	EMSlist31<-numeric(0)
	samplesizelist31<-numeric(0)
	for(i in 1:length(userlist1)) {
		EMSlist31[i]<-cov23+ rep0*cov22 + rep0*rep2*cov21
		samplesizelist31[i]<-rep0*rep2*userlist1[i]
	}

	legtitle<-paste("Replication of ",randomEffect1, "= ")
	legtitle2<-rep(legtitle,length(userlist1))
	legtitle3<-paste(legtitle2, userlist1)
	temp<-matrix(1,nrow=length(diffs),ncol=length(samplesizelist31))
	for (j in 1:length(samplesizelist31))	{
		for(i in 1:length(diffs)) {
			test<-sipowerttest(n=samplesizelist31[j], delta=diffs[i], sd=EMSlist31[j], sig.level=siglevel)
	 		temp[i,j]=test
		}
	}
	temp2<-100*temp
	powergraph=cbind(diffs,temp2)

	plot1 <- sub(".html", "plot1.jpg", htmlFile)
	jpeg(plot1,width = jpegwidth, height = jpegheight, quality = 100)

	#STB July2013
	plotFilepdf5 <- sub(".html", "plot1.pdf", htmlFile)
	dev.control("enable") 

	#Graphics parameters
	XAxisTitle <- "Biological difference"
	MainTitle2 <-""
	powerFrom <-0
	powerTo <-105
	namez <- paste("Replication of ", randomEffect1 , ": ", sep = "")

	#Generating the variable list for the lines - they are labelled V1 up to Vn
	lin_no<-dim(temp2)[2]
	lin_list<-c(2:(lin_no+1))
	lin_list2<-paste("V",lin_list,sep = "")

	gr_temp<-data.frame(cbind(diffs, temp2))
	graphdata<-melt(data=gr_temp, id=c("diffs"))

	#Colour palette definition
	temp<-IVS_F_cpplot_colour("variable")
	Gr_palette_A<-temp$Gr_palette_A
	Gr_line<-temp$Gr_line
	Gr_fill <- temp$Gr_fill	
	userlistgr<- userlist1
	randomEffect1a<-namereplace(randomEffect1)

	#GGPLOT2 code
	POWERPLOT_NEW(randomEffect1a)

	void <- HTMLInsertGraph(GraphFileName=sub("[A-Z0-9a-z,:,\\\\]*App_Data[\\\\]","", plot1), Align="left")

	#STB July2013
	if (pdfout=="Y") {
		pdf(file=sub("[A-Z0-9a-z,:,\\\\]*App_Data[\\\\]","", plotFilepdf5), height = pdfheight, width = pdfwidth) 
		dev.set(2) 
		dev.copy(which=3) 
		dev.off(2)
		dev.off(3)
		pdfFile_5<-sub("[A-Z0-9a-z,:,\\\\]*App_Data[\\\\]","",plotFilepdf5)
		linkToPdf5 <- paste ("<a href=\"",pdfFile_5,"\">Click here to view the PDF of the plot of the power curves</a>", sep = "")
		HTML(linkToPdf5)
	}
	Comment1<-paste("These power curves assume the replication of the levels of the random factor ", randomEffect2, " is ", rep2, " and the replication of the levels of the random factor ", randomEffect3, " is ", rep0, ".", sep="") 
	HTML(Comment1, align="left")
}

if (randomEffect1 != "NULL" && randomEffect2 != "NULL" && randomEffect3 != "NULL" && randomEffect4 == "NULL" && repList2!="NULL") {
	title<-paste("Power curves for a design varying the replication of ", randomEffect2, sep="")
	HTML.title(title, HR=2, align="left")

	EMSlist32<-numeric(0)
	samplesizelist32<-numeric(0)
	for(i in 1:length(userlist2)) {
		EMSlist32[i]<-cov23+ rep0*cov22 + rep0*userlist2[i]*cov21
		samplesizelist32[i]<-rep0*rep1*userlist2[i]
	}

	legtitle<-paste("Replication of ",randomEffect2, "= ")
	legtitle2<-rep(legtitle,length(userlist2))
	legtitle3<-paste(legtitle2, userlist2)
	temp<-matrix(1,nrow=length(diffs),ncol=length(samplesizelist32))
	for (j in 1:length(samplesizelist32)) {
		for(i in 1:length(diffs))
		{
			test<-sipowerttest(n=samplesizelist32[j], delta=diffs[i], sd=EMSlist32[j], sig.level=siglevel)
	 		temp[i,j]=test
		}
	}
	temp2<-100*temp
	powergraph=cbind(diffs,temp2)
	plot2 <- sub(".html", "plot2.jpg", htmlFile)
	jpeg(plot2,width = jpegwidth, height = jpegheight, quality = 100)

	#STB July2013
	plotFilepdf6 <- sub(".html", "plot2.pdf", htmlFile)
	dev.control("enable") 

	#Graphics parameters
	XAxisTitle <- "Biological difference"
	MainTitle2 <-""
	powerFrom <-0
	powerTo <-105
	namez <- paste("Replication of ", randomEffect2 , ": ", sep = "")

	#Generating the variable list for the lines - they are labelled V1 up to Vn
	lin_no<-dim(temp2)[2]
	lin_list<-c(2:(lin_no+1))
	lin_list2<-paste("V",lin_list,sep = "")

	gr_temp<-data.frame(cbind(diffs, temp2))
	graphdata<-melt(data=gr_temp, id=c("diffs"))

	#Colour palette definition
	temp<-IVS_F_cpplot_colour("variable")
	Gr_palette_A<-temp$Gr_palette_A
	Gr_line<-temp$Gr_line
	Gr_fill <- temp$Gr_fill	

	userlistgr<- userlist2
	randomEffect2a<-namereplace(randomEffect2)

	#GGPLOT2 code
	POWERPLOT_NEW(randomEffect2a)

	void <- HTMLInsertGraph(GraphFileName=sub("[A-Z0-9a-z,:,\\\\]*App_Data[\\\\]","", plot2), Align="left")

	#STB July2013
	if (pdfout=="Y") {
		pdf(file=sub("[A-Z0-9a-z,:,\\\\]*App_Data[\\\\]","", plotFilepdf6), height = pdfheight, width = pdfwidth) 
		dev.set(2) 
		dev.copy(which=3) 
		dev.off(2)
		dev.off(3)
		pdfFile_6<-sub("[A-Z0-9a-z,:,\\\\]*App_Data[\\\\]","",plotFilepdf6)
		linkToPdf6 <- paste ("<a href=\"",pdfFile_6,"\">Click here to view the PDF of the plot of the power curves</a>", sep = "")
		HTML(linkToPdf6)
	}
	title<-paste("These power curves are for a design varying the replication of ", randomEffect2, " within the levels of ", randomEffect1, ".", sep="")
	HTML(title, align="left")
	Comment2<-paste("These power curves assume the replication of the levels of the random factor ", randomEffect1, " is ", rep1, " and the replication of ", randomEffect3, " is ", rep0, ".", sep="") 
	HTML(Comment2, align="left")
}

if (randomEffect1 != "NULL" && randomEffect2 != "NULL" && randomEffect3 != "NULL" && randomEffect4 == "NULL" && repList3!="NULL") {
	title<-paste("Power curves for a design varying the replication of ", randomEffect3, sep="")
	HTML.title(title, HR=2, align="left")

	EMSlist33<-numeric(0)
	samplesizelist33<-numeric(0)
	for(i in 1:length(userlist3)) {
		EMSlist33[i]<-cov23+ userlist3[i]*cov22 + userlist3[i]*rep2*cov21
		samplesizelist33[i]<-rep2*rep1*userlist3[i]
	}

	legtitle<-paste("Replication of ",randomEffect3, "= ")
	legtitle2<-rep(legtitle,length(userlist3))
	legtitle3<-paste(legtitle2, userlist3)
	temp<-matrix(1,nrow=length(diffs),ncol=length(samplesizelist33))
	for (j in 1:length(samplesizelist33)) {
		for(i in 1:length(diffs))
		{
			test<-sipowerttest(n=samplesizelist33[j], delta=diffs[i], sd=EMSlist33[j], sig.level=siglevel)
	 		temp[i,j]=test
		}
	}
	temp2<-100*temp
	powergraph=cbind(diffs,temp2)
	plot3 <- sub(".html", "plot3.jpg", htmlFile)
	jpeg(plot3,width = jpegwidth, height = jpegheight, quality = 100)

	#STB July2013
	plotFilepdf7 <- sub(".html", "plot3.pdf", htmlFile)
	dev.control("enable") 

	#Graphics parameters
	XAxisTitle <- "Biological difference"
	MainTitle2 <-""
	powerFrom <-0
	powerTo <-105
	namez <- paste("Replication of ", randomEffect3 , ": ", sep = "")

	#Generating the variable list for the lines - they are labelled V1 up to Vn
	lin_no<-dim(temp2)[2]
	lin_list<-c(2:(lin_no+1))
	lin_list2<-paste("V",lin_list,sep = "")

	gr_temp<-data.frame(cbind(diffs, temp2))
	graphdata<-melt(data=gr_temp, id=c("diffs"))

	#Colour palette definition
	temp<-IVS_F_cpplot_colour("variable")
	Gr_palette_A<-temp$Gr_palette_A
	Gr_line<-temp$Gr_line
	Gr_fill <- temp$Gr_fill	

	userlistgr<- userlist3
	randomEffect3a<-namereplace(randomEffect3)

	#GGPLOT2 code
	POWERPLOT_NEW(randomEffect3a)
	void <- HTMLInsertGraph(GraphFileName=sub("[A-Z0-9a-z,:,\\\\]*App_Data[\\\\]","", plot3), Align="left")

	#STB July2013
	if (pdfout=="Y") {
		pdf(file=sub("[A-Z0-9a-z,:,\\\\]*App_Data[\\\\]","", plotFilepdf7), height = pdfheight, width = pdfwidth) 
		dev.set(2) 
		dev.copy(which=3) 
		dev.off(2)
		dev.off(3)
		pdfFile_7<-sub("[A-Z0-9a-z,:,\\\\]*App_Data[\\\\]","",plotFilepdf7)
		linkToPdf7 <- paste ("<a href=\"",pdfFile_7,"\">Click here to view the PDF of the plots of the power curves</a>", sep = "")
		HTML(linkToPdf7)
	}
	title<-paste("These power curves are for a design varying the replication of ", randomEffect3, " within the levels of ", randomEffect2, " within the levels of ", randomEffect1, ".", sep="")
	HTML(title, align="left")
	Comment2<-paste("These power curves assume the replication of the levels of the random factor ", randomEffect1, " is ", rep1, " and the replication of ", randomEffect2, " is ", rep2, ".", sep="") 
	HTML(Comment2, align="left")
}

if (randomEffect1 != "NULL" && randomEffect2 != "NULL" && randomEffect3 != "NULL" && randomEffect4 == "NULL" && repList1!="NULL" && repList2!="NULL"&& repList3!="NULL") {
	l1<- length(userlist1) 
	l2<- length(userlist2)
	l3<- length(userlist3)

	if (l1 ==l2 && l1==l3)	 {
		title<-paste("Power curves for a design varying the replication of all random effects  simultaneously" , sep="")
		HTML.title(title, HR=2, align="left")
	
		EMSlist12<-numeric(0)
		samplesizelist12<-numeric(0)
		for(i in 1:length(userlist2)) {
			EMSlist12[i]<-cov23+ userlist3[i]*cov22 + userlist3[i]*userlist2[i]*cov21
			samplesizelist12[i]<-userlist1[i]*userlist2[i]*userlist3[i]
		}

		legtitle<-paste("Replication of",randomEffect1, "- ")
		legtitle2<-rep(legtitle,length(userlist1))
		legtitle3<-paste(", replication of ",randomEffect2, "- ")
		legtitle4<-rep(legtitle3,length(userlist1))
		legtitle5<-paste(", replication of ",randomEffect3, "- ")
		legtitle6<-rep(legtitle5,length(userlist1))
		legtitle3<-paste(legtitle2, userlist1,  legtitle4, userlist2,  legtitle6, userlist3)

		temp<-matrix(1,nrow=length(diffs),ncol=length(samplesizelist12))
		for (j in 1:length(samplesizelist12)) {
			for(i in 1:length(diffs)) {
				test<-sipowerttest(n=samplesizelist12[j], delta=diffs[i], sd=EMSlist12[j], sig.level=siglevel)
		 		temp[i,j]=test
			}
		}
		temp2<-100*temp
		powergraph=cbind(diffs,temp2)

		plot12 <- sub(".html", "plot12.jpg", htmlFile)
		jpeg(plot12,width = jpegwidth, height = jpegheight, quality = 100)

		#STB July2013
		plotFilepdf8 <- sub(".html", "plot12.pdf", htmlFile)
		dev.control("enable") 

		#Graphics parameters
		XAxisTitle <- "Biological difference"
		MainTitle2 <-""
		powerFrom <-0
		powerTo <-105

		userlistx<-numeric(0)
		for(i in 1:length(userlist2)) {
			userlistx[i]<- paste(userlist1[i], ",", userlist2[i], ",", userlist3[i], sep="")
		}
		namez <- paste("Replication of: \n", randomEffect1, ", ", randomEffect2 , ", ", randomEffect3 , " ", sep = "")

		#Generating the variable list for the lines - they are labelled V1 up to Vn
		lin_no<-dim(temp2)[2]
		lin_list<-c(2:(lin_no+1))
		lin_list2<-paste("V",lin_list,sep = "")
	
		gr_temp<-data.frame(cbind(diffs, temp2))
		graphdata<-melt(data=gr_temp, id=c("diffs"))

		#Colour palette definition
		temp<-IVS_F_cpplot_colour("variable")
		Gr_palette_A<-temp$Gr_palette_A
		Gr_line<-temp$Gr_line
		Gr_fill <- temp$Gr_fill	

		userlistgr<- userlistx
		testy<-paste(randomEffect1, ", ", randomEffect2 , ", ", randomEffect3 , " ", sep = "")
		testya<-namereplace(testy)

		#GGPLOT2 code
		POWERPLOT_NEW(testya)

		void <- HTMLInsertGraph(GraphFileName=sub("[A-Z0-9a-z,:,\\\\]*App_Data[\\\\]","", plot12), Align="left")

		#STB July2013
		if (pdfout=="Y") {
			pdf(file=sub("[A-Z0-9a-z,:,\\\\]*App_Data[\\\\]","", plotFilepdf8), height = pdfheight, width = pdfwidth) 
			dev.set(2) 
			dev.copy(which=3) 
			dev.off(2)
			dev.off(3)
			pdfFile_8<-sub("[A-Z0-9a-z,:,\\\\]*App_Data[\\\\]","",plotFilepdf8)
			linkToPdf8 <- paste ("<a href=\"",pdfFile_8,"\">Click here to view the PDF of the plot of the power curves</a>", sep = "")
			HTML(linkToPdf8)
		}

	}
}

#===================================================================================================================
#===================================================================================================================
# Four random effects
#===================================================================================================================
#===================================================================================================================
if (randomEffect1 != "NULL" && randomEffect2 != "NULL" && randomEffect3 != "NULL" && randomEffect4 != "NULL" && repList1!="NULL") {
	title<-paste("Power curves for a design varying the replication of ", randomEffect1)
	HTML.title(title, HR=2, align="left")

	EMSlist41<-numeric(0)
	samplesizelist41<-numeric(0)
	for(i in 1:length(userlist1)) {
		EMSlist41[i]<-cov34 + rep0*cov33 + rep0*rep3*cov32 + rep0*rep3*rep2*cov31
		samplesizelist41[i]<-rep0*rep2*rep3*userlist1[i]
	}

	legtitle<-paste("Replication of ",randomEffect1, "= ")
	legtitle2<-rep(legtitle,length(userlist1))
	legtitle3<-paste(legtitle2,userlist1)
	temp<-matrix(1,nrow=length(diffs),ncol=length(samplesizelist41))
	for (j in 1:length(samplesizelist41)) {
		for(i in 1:length(diffs)) {
			test<-sipowerttest(n=samplesizelist41[j], delta=diffs[i], sd=EMSlist41[j], sig.level=siglevel)
	 		temp[i,j]=test
		}
	}
	temp2<-100*temp
	powergraph=cbind(diffs,temp2)

	plot1 <- sub(".html", "plot1.jpg", htmlFile)
	jpeg(plot1,width = jpegwidth, height = jpegheight, quality = 100)

	#STB July2013
	plotFilepdf9 <- sub(".html", "plot1.pdf", htmlFile)
	dev.control("enable") 

	#Graphics parameters
	XAxisTitle <- "Biological difference"
	MainTitle2 <-""
	powerFrom <-0
	powerTo <-105
	namez <- paste("Replication of ", randomEffect1 , ": ", sep = "")

	#Generating the variable list for the lines - they are labelled V1 up to Vn
	lin_no<-dim(temp2)[2]
	lin_list<-c(2:(lin_no+1))
	lin_list2<-paste("V",lin_list,sep = "")

	gr_temp<-data.frame(cbind(diffs, temp2))
	graphdata<-melt(data=gr_temp, id=c("diffs"))

	#Colour palette definition
	temp<-IVS_F_cpplot_colour("variable")
	Gr_palette_A<-temp$Gr_palette_A
	Gr_line<-temp$Gr_line
	Gr_fill <- temp$Gr_fill	

	userlistgr<- userlist1
	randomEffect1a<-namereplace(randomEffect1)

	#GGPLOT2 code
	POWERPLOT_NEW(randomEffect1a)

	void <- HTMLInsertGraph(GraphFileName=sub("[A-Z0-9a-z,:,\\\\]*App_Data[\\\\]","", plot1), Align="left")

	#STB July2013
	if (pdfout=="Y") {
		pdf(file=sub("[A-Z0-9a-z,:,\\\\]*App_Data[\\\\]","", plotFilepdf9), height = pdfheight, width = pdfwidth) 
		dev.set(2) 
		dev.copy(which=3) 
		dev.off(2)
		dev.off(3)
		pdfFile_9<-sub("[A-Z0-9a-z,:,\\\\]*App_Data[\\\\]","",plotFilepdf9)
		linkToPdf9 <- paste ("<a href=\"",pdfFile_9,"\">Click here to view the PDF of the plot of the power curves</a>", sep = "")
		HTML(linkToPdf9)
	}
	Comment1<-paste("These power curves assume the replication of the levels of the random factor ", randomEffect2, " is ",  rep2, ", the replication of the levels of the random factor ", randomEffect3, " is ", rep3, " and the replication of the levels of the random factor ", randomEffect4, " is ", rep0, ".", sep="") 
	HTML(Comment1, align="left")
}

if (randomEffect1 != "NULL" && randomEffect2 != "NULL" && randomEffect3 != "NULL" && randomEffect4 != "NULL" && repList2!="NULL") {
	title<-paste("Power curves for a design varying the replication of ", randomEffect2, sep="")
	HTML.title(title, HR=2, align="left")

	EMSlist42<-numeric(0)
	samplesizelist42<-numeric(0)
	for(i in 1:length(userlist2)) {
		EMSlist42[i]<-cov34 + rep0*cov33 + rep0*rep3*cov32 + rep0*rep3*userlist2[i]*cov31
		samplesizelist42[i]<-rep0*rep1*rep3*userlist2[i]
	}

	legtitle<-paste("Replication of ",randomEffect2 , "= ")
	legtitle2<-rep(legtitle,length(userlist2))
	legtitle3<-paste(legtitle2,userlist2)
	temp<-matrix(1,nrow=length(diffs),ncol=length(samplesizelist42))
	for (j in 1:length(samplesizelist42)) {
		for(i in 1:length(diffs)) {
			test<-sipowerttest(n=samplesizelist42[j], delta=diffs[i], sd=EMSlist42[j], sig.level=siglevel)
	 		temp[i,j]=test
		}
	}
	temp2<-100*temp
	powergraph=cbind(diffs,temp2)
	plot2 <- sub(".html", "plot2.jpg", htmlFile)
	jpeg(plot2,width = jpegwidth, height = jpegheight, quality = 100)

	#STB July2013
	plotFilepdf1a <- sub(".html", "plot2.pdf", htmlFile)
	dev.control("enable") 

	#Graphics parameters
	XAxisTitle <- "Biological difference"
	MainTitle2 <-""
	powerFrom <-0
	powerTo <-105
	namez <- paste("Replication of ", randomEffect2 , ": ", sep = "")

	#Generating the variable list for the lines - they are labelled V1 up to Vn
	lin_no<-dim(temp2)[2]
	lin_list<-c(2:(lin_no+1))
	lin_list2<-paste("V",lin_list,sep = "")

	gr_temp<-data.frame(cbind(diffs, temp2))
	graphdata<-melt(data=gr_temp, id=c("diffs"))

	#Colour palette definition
	temp<-IVS_F_cpplot_colour("variable")
	Gr_palette_A<-temp$Gr_palette_A
	Gr_line<-temp$Gr_line
	Gr_fill <- temp$Gr_fill	
	userlistgr<- userlist2
	randomEffect2a<-namereplace(randomEffect2)

	#GGPLOT2 code
	POWERPLOT_NEW(randomEffect2a)

	void <- HTMLInsertGraph(GraphFileName=sub("[A-Z0-9a-z,:,\\\\]*App_Data[\\\\]","", plot2), Align="left")

	#STB July2013
	if (pdfout=="Y") {
		pdf(file=sub("[A-Z0-9a-z,:,\\\\]*App_Data[\\\\]","", plotFilepdf1a), height = pdfheight, width = pdfwidth) 
		dev.set(2) 
		dev.copy(which=3) 
		dev.off(2)
		dev.off(3)
		pdfFile_1a<-sub("[A-Z0-9a-z,:,\\\\]*App_Data[\\\\]","",plotFilepdf1a)
		linkToPdf1a <- paste ("<a href=\"",pdfFile_1a,"\">Click here to view the PDF of the plot of the power curves</a>", sep = "")
		HTML(linkToPdf1a)
	}
	title<-paste("These power curves are for a design varying the replication of ", randomEffect2, " within the levels of ", randomEffect1, ".", sep="")
	HTML(title, align="left")
	Comment2<-paste("These power curves assume the replication of the levels of the random factor ", randomEffect1, " is ", rep1, ", the replication of the levels of the random factor ", randomEffect3, " is ", rep3, " and the replication of ", randomEffect4, " is ", rep0, ".", sep="") 
	HTML(Comment2, align="left")
}
 
if (randomEffect1 != "NULL" && randomEffect2 != "NULL" && randomEffect3 != "NULL" && randomEffect4 != "NULL" && repList3!="NULL") {
	title<-paste("Power curves for a design varying the replication of ", randomEffect3, sep="")
	HTML.title(title, HR=2, align="left")

	EMSlist43<-numeric(0)
	samplesizelist43<-numeric(0)
	for(i in 1:length(userlist3)) {
		EMSlist43[i]<-cov34 + rep0*cov33 + rep0*userlist3[i]*cov32 + rep0*userlist3[i]*rep2*cov31
		samplesizelist43[i]<-rep2*rep1*rep0*userlist3[i]
	}

	legtitle<-paste("Replication of ",randomEffect3, "= ")
	legtitle2<-rep(legtitle,length(userlist3))
	legtitle3<-paste(legtitle2,userlist3)
	temp<-matrix(1,nrow=length(diffs),ncol=length(samplesizelist43))
	for (j in 1:length(samplesizelist43)) {
		for(i in 1:length(diffs)) {
			test<-sipowerttest(n=samplesizelist43[j], delta=diffs[i], sd=EMSlist43[j], sig.level=siglevel)
	 		temp[i,j]=test
		}
	}
	temp2<-100*temp
	powergraph=cbind(diffs,temp2)

	plot3 <- sub(".html", "plot3.jpg", htmlFile)
	jpeg(plot3,width = jpegwidth, height = jpegheight, quality = 100)

	#STB July2013
	plotFilepdf1b <- sub(".html", "plot3.pdf", htmlFile)
	dev.control("enable") 

	#Graphics parameters
	XAxisTitle <- "Biological difference"
	MainTitle2 <-""
	powerFrom <-0
	powerTo <-105
	namez <- paste("Replication of ", randomEffect3 , ": ", sep = "")

	#Generating the variable list for the lines - they are labelled V1 up to Vn
	lin_no<-dim(temp2)[2]
	lin_list<-c(2:(lin_no+1))
	lin_list2<-paste("V",lin_list,sep = "")

	gr_temp<-data.frame(cbind(diffs, temp2))
	graphdata<-melt(data=gr_temp, id=c("diffs"))

	#Colour palette definition
	temp<-IVS_F_cpplot_colour("variable")
	Gr_palette_A<-temp$Gr_palette_A
	Gr_line<-temp$Gr_line
	Gr_fill <- temp$Gr_fill	

	userlistgr<- userlist3
	randomEffect3a<-namereplace(randomEffect3)

	#GGPLOT2 code
	POWERPLOT_NEW(randomEffect3a)

	void <- HTMLInsertGraph(GraphFileName=sub("[A-Z0-9a-z,:,\\\\]*App_Data[\\\\]","", plot3), Align="left")

	#STB July2013
	if (pdfout=="Y") {
		pdf(file=sub("[A-Z0-9a-z,:,\\\\]*App_Data[\\\\]","", plotFilepdf1b), height = pdfheight, width = pdfwidth) 
		dev.set(2) 
		dev.copy(which=3) 
		dev.off(2)
		dev.off(3)
		pdfFile_1b<-sub("[A-Z0-9a-z,:,\\\\]*App_Data[\\\\]","",plotFilepdf1b)
		linkToPdf1b <- paste ("<a href=\"",pdfFile_1b,"\">Click here to view the PDF of the plot of the power curves</a>", sep = "")
		HTML(linkToPdf1b)
	}
	title<-paste("These power curves are for a design varying the replication of ", randomEffect3, " within the levels of ", randomEffect2, " within the levels of ", randomEffect1, ".", sep="")
	HTML(title, align="left")
	Comment2<-paste("These power curves assume the replication of the levels of the random factor ", randomEffect1, " is ", rep1, ", the replication of the levels of the random factor ", randomEffect2, " is ",  rep2, " and the replication of ", randomEffect4, " is ", rep0, ".", sep="") 
	HTML(Comment2, align="left")
}

if (randomEffect1 != "NULL" && randomEffect2 != "NULL" && randomEffect3 != "NULL" && randomEffect4 != "NULL" && repList4!="NULL") {
	title<-paste("Power curves for a design varying the replication of ", randomEffect4, sep="")
	HTML.title(title, HR=2, align="left")

	EMSlist44<-numeric(0)
	samplesizelist44<-numeric(0)
	for(i in 1:length(userlist4)) {
		EMSlist44[i]<-cov34 + rep0*cov33 + userlist4[i]*rep3*cov32 + rep3*userlist4[i]*rep2*cov31
		samplesizelist44[i]<-rep2*rep1*rep3*userlist4[i]
	}

	legtitle<-paste("Replication of ",randomEffect4, "= ")
	legtitle2<-rep(legtitle,length(userlist4))
	legtitle3<-paste(legtitle2,userlist4)
	temp<-matrix(1,nrow=length(diffs),ncol=length(samplesizelist44))
	for (j in 1:length(samplesizelist44)) {
		for(i in 1:length(diffs)) {
			test<-sipowerttest(n=samplesizelist44[j], delta=diffs[i], sd=EMSlist44[j], sig.level=siglevel)
	 		temp[i,j]=test
		}
	}
	temp2<-100*temp
	powergraph=cbind(diffs,temp2)
	plot4 <- sub(".html", "plot4.jpg", htmlFile)
	jpeg(plot4,width = jpegwidth, height = jpegheight, quality = 100)

	#STB July2013
	plotFilepdf1d <- sub(".html", "plot4.pdf", htmlFile)
	dev.control("enable") 

	#Graphics parameters
	XAxisTitle <- "Biological difference"
	MainTitle2 <-""
	powerFrom <-0
	powerTo <-105
	namez <- paste("Replication of ", randomEffect4 , ": ", sep = "")

	#Generating the variable list for the lines - they are labelled V1 up to Vn
	lin_no<-dim(temp2)[2]
	lin_list<-c(2:(lin_no+1))
	lin_list2<-paste("V",lin_list,sep = "")

	gr_temp<-data.frame(cbind(diffs, temp2))
	graphdata<-melt(data=gr_temp, id=c("diffs"))

	#Colour palette definition
	temp<-IVS_F_cpplot_colour("variable")
	Gr_palette_A<-temp$Gr_palette_A
	Gr_line<-temp$Gr_line
	Gr_fill <- temp$Gr_fill	

	userlistgr<- userlist4
	randomEffect4a<-namereplace(randomEffect4)

	#GGPLOT2 code
	POWERPLOT_NEW(randomEffect4a)

	void <- HTMLInsertGraph(GraphFileName=sub("[A-Z0-9a-z,:,\\\\]*App_Data[\\\\]","", plot4), Align="left")

	#STB July2013
	if (pdfout=="Y") {
		pdf(file=sub("[A-Z0-9a-z,:,\\\\]*App_Data[\\\\]","", plotFilepdf1d), height = pdfheight, width = pdfwidth) 
		dev.set(2) 
		dev.copy(which=3) 
		dev.off(2)
		dev.off(3)
		pdfFile_1d<-sub("[A-Z0-9a-z,:,\\\\]*App_Data[\\\\]","",plotFilepdf1d)
		linkToPdf1d <- paste ("<a href=\"",pdfFile_1d,"\">Click here to view the PDF of the plot of the power curves</a>", sep = "")
		HTML(linkToPdf1d)
	}
	title<-paste("These power curves are for a design varying the replication of ", randomEffect4, " within the levels of ", randomEffect3, " within the levels of  ", randomEffect2, " within the levels of ", randomEffect1, ".", sep="")
	HTML(title, align="left")
	Comment2<-paste("These power curves assume the replication of the levels of the random factor ", randomEffect1, " is ", rep1, ", the replication of the levels of the random factor ", randomEffect2, " is ", rep2, " and the replication of ", randomEffect3, " is ", rep3, ".", sep="") 
	HTML(Comment2, align="left")
}

if (randomEffect1 != "NULL" && randomEffect2 != "NULL" && randomEffect3 != "NULL" && randomEffect4 != "NULL" && repList1!="NULL" && repList2!="NULL" && repList3!="NULL" && repList4!="NULL") {
	l1<- length(userlist1) 
	l2<- length(userlist2)
	l3<- length(userlist3)
	l4<- length(userlist4)

	if (l1 ==l2 && l1==l3 && l1==l4) {
		title<-paste("Power curves for a design varying the replication of all random effects  simultaneously" , sep="")
		HTML.title(title, HR=2, align="left")

		EMSlist12<-numeric(0)
		samplesizelist12<-numeric(0)
		for(i in 1:length(userlist2)) {
			EMSlist12[i]<-cov34 + userlist4[i]*cov33 + userlist4[i]*userlist3[i]*cov32 + userlist4[i]*userlist3[i]*userlist2[i]*cov31
			samplesizelist12[i]<-userlist1[i]*userlist2[i]*userlist3[i]*userlist4[i]
		}

		legtitle<-paste("Rep. of",randomEffect1, "- ")
		legtitle2<-rep(legtitle,length(userlist1))
		legtitle3<-paste(", rep. of ",randomEffect2, "- ")
		legtitle4<-rep(legtitle3,length(userlist1))
		legtitle5<-paste(", rep. of ",randomEffect3, "- ")
		legtitle6<-rep(legtitle5,length(userlist1))
		legtitle7<-paste(", rep. of ",randomEffect4, "- ")
		legtitle8<-rep(legtitle5,length(userlist1))
		legtitle3<-paste(legtitle2, userlist1,  legtitle4, userlist2,  legtitle6, userlist3,  legtitle8, userlist4)

		temp<-matrix(1,nrow=length(diffs),ncol=length(samplesizelist12))
		for (j in 1:length(samplesizelist12)) {
			for(i in 1:length(diffs))
			{
				test<-sipowerttest(n=samplesizelist12[j], delta=diffs[i], sd=EMSlist12[j], sig.level=siglevel)
		 		temp[i,j]=test
			}
		}
		temp2<-100*temp
		powergraph=cbind(diffs,temp2)
		plot12 <- sub(".html", "plot12.jpg", htmlFile)
		jpeg(plot12,width = jpegwidth, height = jpegheight, quality = 100)

		#STB July2013
		plotFilepdf1e <- sub(".html", "plot12.pdf", htmlFile)
		dev.control("enable") 

		#Graphics parameters
		XAxisTitle <- "Biological difference"
		MainTitle2 <-""
		powerFrom <-0
		powerTo <-105

		userlistx<-numeric(0)
		for(i in 1:length(userlist2)) {
			userlistx[i]<- paste(userlist1[i], ",", userlist2[i], ",", userlist3[i], ",", userlist4[i], sep="")
		}
		namez <- paste("Replication of: \n", randomEffect1, ", ", randomEffect2 , ", ", randomEffect3 , ", ", randomEffect4 , "  ", sep = "")

		#Generating the variable list for the lines - they are labelled V1 up to Vn
		lin_no<-dim(temp2)[2]
		lin_list<-c(2:(lin_no+1))
		lin_list2<-paste("V",lin_list,sep = "")

		gr_temp<-data.frame(cbind(diffs, temp2))
		graphdata<-melt(data=gr_temp, id=c("diffs"))
		
		#Colour palette definition
		temp<-IVS_F_cpplot_colour("variable")
		Gr_palette_A<-temp$Gr_palette_A
		Gr_line<-temp$Gr_line
		Gr_fill <- temp$Gr_fill	

		userlistgr<- userlistx
		testy<-paste(randomEffect1, ", ", randomEffect2 , ", ", randomEffect3 , ", ", randomEffect4 , "  ", sep = "")
		testya<-namereplace(testy)

		#GGPLOT2 code
		POWERPLOT_NEW(testya)

		void <- HTMLInsertGraph(GraphFileName=sub("[A-Z0-9a-z,:,\\\\]*App_Data[\\\\]","", plot12), Align="left")

		#STB July2013
		if (pdfout=="Y") {
			pdf(file=sub("[A-Z0-9a-z,:,\\\\]*App_Data[\\\\]","", plotFilepdf1e), height = pdfheight, width = pdfwidth) 
			dev.set(2) 
			dev.copy(which=3) 
			dev.off(2)
			dev.off(3)
			pdfFile_1e<-sub("[A-Z0-9a-z,:,\\\\]*App_Data[\\\\]","",plotFilepdf1e)
			linkToPdf1e <- paste ("<a href=\"",pdfFile_1e,"\">Click here to view the PDF of the plot of the power curves</a>", sep = "")
			HTML(linkToPdf1e)
		}

	}
}

#===================================================================================================================
#References
#===================================================================================================================
Ref_list<-R_refs()

HTML.title("Statistical references", HR=2, align="left")
HTML(Ref_list$BateClark_ref,  align="left")
HTML("Snedecor GW and Cochran WG. (1989). Statistical Methods (8th Edition). Iowa State University Press, Ames, Iowa.", align="left")

HTML("R references", HR=2, align="left")
HTML(Ref_list$R_ref ,  align="left")
HTML(Ref_list$GGally_ref,  align="left")
HTML(Ref_list$RColorBrewers_ref,  align="left")
HTML(Ref_list$GGPLot2_ref,  align="left")
HTML(Ref_list$reshape_ref,  align="left")
HTML(Ref_list$plyr_ref,  align="left")
HTML(Ref_list$scales_ref,  align="left")
HTML(Ref_list$nlme_ref,  align="left")
HTML(Ref_list$R2HTML_ref,  align="left")
HTML(Ref_list$PROTO_ref,  align="left")

#===================================================================================================================
#Show dataset
#===================================================================================================================
if (showdataset=="Y") {
	statdata2<-subset(statdata, select = -c(alltreat))
	if (randomEffect1 != "NULL") {
		statdata2<-subset(statdata2, select = -c(rEffect1IVS))
	}
	if (randomEffect2 != "NULL") {
		statdata2<-subset(statdata2, select = -c(rEffect2IVS))
	}
	if (randomEffect3 != "NULL") {
		statdata2<-subset(statdata2, select = -c(rEffect3IVS))
	}
	if (randomEffect4 != "NULL") {
		statdata2<-subset(statdata2, select = -c(rEffect4IVS))
	}

	observ <- data.frame(c(1:dim(statdata2)[1]))
	colnames(observ) <- c("Observation")
	statdata22 <- cbind(observ, statdata2)

    	HTML.title("Analysis dataset", HR = 2, align = "left")
    	HTML(statdata22, classfirstline = "second", align = "left", row.names = "FALSE")

}

#===================================================================================================================
#Show arguments
#===================================================================================================================
HTML.title("Analysis options", HR=2, align="left")

HTML(paste("Response variable: ", resp, sep=""), align="left")

if (responseTransform != "None") {
	HTML(paste("Response transformation: ", responseTransform, sep=""), align="left")
}

if (covariate != "NULL") {
	HTML(paste("Covariate variable: ", covariate, sep=""), align="left")
}

if (covariateTransform != "None") {
	HTML(paste("Covariate transformation: ", covariateTransform, sep=""), align="left")
}

if (treatments != "NULL") {
	HTML(paste("Treatment factors: ", treatments, sep=""), align="left")
}

if (otherFactors != "NULL") {
	HTML(paste("Other design (blocks) factors: ", otherFactors, sep=""), align="left")
}

if (randomEffect1 != "NULL") {
	HTML(paste("Random factor 1: ", randomEffect1, sep=""), align="left")
}

if (randomEffect2 != "NULL") {
	HTML(paste("Random factor 2: ", randomEffect2, sep=""), align="left")
}

if (randomEffect3 != "NULL") {
	HTML(paste("Random factor 3: ", randomEffect3, sep=""), align="left")
}

if (randomEffect4 != "NULL") {
	HTML(paste("Random factor 4: ", randomEffect4, sep=""), align="left")
}

if (repList1 != "NULL" && randomEffect1 != "NULL") {
	HTML(paste("Replication options for random factor 1: ", repList1, sep=""), align="left")
}

if (repList2 != "NULL" && randomEffect2 != "NULL") {
	HTML(paste("Replication options for random factor 2: ", repList2, sep=""), align="left")
}

if (repList3 != "NULL" && randomEffect3 != "NULL") {
	HTML(paste("Replication options for random factor 3: ", repList3, sep=""), align="left")
}

if (repList4 != "NULL" && randomEffect4 != "NULL") {
	HTML(paste("Replication options for random factor 4: ", repList4, sep=""), align="left")
}

HTML(paste("Significance level: ", siglevel, sep=""), align="left")




