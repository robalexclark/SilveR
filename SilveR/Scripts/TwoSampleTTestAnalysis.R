#===================================================================================================================
#R Libraries

suppressWarnings(library(multcomp))
suppressWarnings(library(R2HTML))
#V3.2 STB NOV2015
suppressWarnings(library(lsmeans))

#===================================================================================================================
# retrieve args
Args <- commandArgs(TRUE)

#Read in data
statdata <- read.csv(Args[3], header=TRUE, sep=",")

#Copy Args

#STB - July 2012 rename response variable
xxxresponsexxx <- Args[4]

responseTransform <- Args[5]
treatFactor <- Args[6]
equalCase <-Args[7]
unequalCase <- Args[8]
showPRPlot <- Args[9]
showNormPlot <- Args[10]
sig <- 1 - as.numeric(Args[11])

#source(paste(getwd(),"/Common_Functions.R", sep=""))
#===================================================================================================================
#Graphics parameter setup

graphdata<-statdata
Labelz_IVS_ <- "N"

#===================================================================================================================

#Setup the html file and associated css file
htmlFile <- sub(".csv", ".html", Args[3]); #determine the file name of the html file
HTMLSetFile(file=htmlFile) 
cssFile <- "r2html.css"
cssFile <- paste("'",cssFile,"'", sep="") #need to enclose in quotes when path has spaces in it
HTMLCSS(CSSfile = cssFile)

#Output HTML header
Title <-paste(branding, " Unpaired t-test Analysis", sep="")
HTML.title(Title, HR = 1, align = "left")

#Removing illegal charaters

#STB - July 2012 rename response variable
#YAxisTitle <-response
YAxisTitle <-xxxresponsexxx

XAxisTitle <-treatFactor

#replace illegal characters in variable names
for (i in 1:10)
{
	YAxisTitle<-namereplace(YAxisTitle)
	XAxisTitle<-namereplace(XAxisTitle)
}


#Generate mainEffect factor
statdata$mainEffect<-as.factor(eval(parse(text = paste("statdata$", treatFactor))))

#===================================================================================================================
#Response

title<-c("Response")

HTML.title(title, HR=2, align="left")

#STB - July 2012 rename response variable
add<-paste(c("The  "), xxxresponsexxx, sep="")
add<-paste(add, " response is currently being analysed by the Unpaired t-test Analysis module.", sep="")

HTML.title("</bf> ", HR=2, align="left")
HTML.title(add, HR=0, align="left")

if (responseTransform != "None")
{
	HTML.title("<bf> ", HR=2, align="left")
}

if (responseTransform != "None")
{
	add2<-paste(c("The response has been "), responseTransform, sep="")
	add2<-paste(add2, " transformed prior to analysis.", sep="")
	HTML.title(add2, HR=0, align="left")
}

#Bate and Clark comment
HTML.title("<bf> ", HR=2, align="left")
HTML.title(refxx, HR=0, align="left")	

#===================================================================================================================
#Scatterplot

HTMLbr()
title<-c("Scatterplot of the raw data")
if(responseTransform != "None")
{
	title<-paste(title, " (on the ", sep="")
	title<-paste(title, responseTransform, sep="")
	title<-paste(title, " scale)", sep="")
}

HTML.title(title, HR=2, align="left")



scatterPlot <- sub(".html", "scatterPlot.jpg", htmlFile)
jpeg(scatterPlot,width = jpegwidth, height = jpegheight, quality = 100)

#STB July2013
plotFilepdf <- sub(".html", "scatterPlot.pdf", htmlFile)
dev.control("enable") 

#===================================================================================================================
#Graphical parameters
graphdata<-statdata
graphdata$yvarrr_IVS <- eval(parse(text = paste("statdata$",xxxresponsexxx)))
graphdata$xvarrr_IVS <-eval(parse(text = paste("statdata$",treatFactor)))
MainTitle2 <- ""
w_Gr_jit <- 0
h_Gr_jit <-  0
infiniteslope <- "Y"

#===================================================================================================================
#GGPLOT2 code
NONCAT_SCAT("Normal")
#===================================================================================================================



void <- HTMLInsertGraph(GraphFileName=sub("[A-Z0-9a-z,:,\\\\]*App_Data[\\\\]","", scatterPlot), Align="centre")

#STB July2013
if (pdfout=="Y")
{
	pdf(file=sub("[A-Z0-9a-z,:,\\\\]*App_Data[\\\\]","", plotFilepdf), height = pdfheight, width = pdfwidth) 
	dev.set(2) 
	dev.copy(which=3) 
	dev.off(2)
	dev.off(3)
	pdfFile<-sub("[A-Z0-9a-z,:,\\\\]*App_Data[\\\\]","",plotFilepdf)
	linkToPdf <- paste ("<a href=\"",pdfFile,"\">Click here to view the PDF of the scatterplot</a>", sep = "")
	HTML(linkToPdf)
}

HTML.title("</bf> ", HR=2, align="left")
HTML.title("</bf> Tip: Use this plot to identify possible outliers.", HR=0, align="left")

#===================================================================================================================
#Equal variance case

if (equalCase == "Y")
{
	HTMLbr()
	HTML.title("<bf>Unpaired t-test assuming equal variances", HR=2, align="left")

#STB - July 2012 rename response variable
	eqtest<-t.test(formula = eval(parse(text = paste("statdata$", xxxresponsexxx)))~eval(parse(text = paste("statdata$", treatFactor))), paired = FALSE, var.equal= TRUE, conf.level= sig)

	col1<-format(round(as.numeric(eqtest[1]), 3), nsmall=3, scientific=FALSE)
	col2<-format(round(as.numeric(eqtest[2]), 0), nsmall=0, scientific=FALSE)
	col3<-format(round(as.numeric(eqtest[3]), 4), nsmall=4, scientific=FALSE)
	col4<-as.numeric(eqtest[3])

	blanq<-c(" ")
	for (i in 1:1)
	{ 
		blanq[i]=" "
	}

	eqtable<-cbind(col1,blanq,col2,blanq,col3)

	head<-c("t-statistic", " ","Degrees of freedom"," ","p-value")
	colnames(eqtable)<-head
#STB May 2012 Updating "unpaired"
	rownames(eqtable)<-c("Equal variance unpaired t-test")

	for (i in 1:(dim(eqtable)[1])) 
	{
		if (as.numeric(col4[i])<0.0001) 
		{
#STB March 2011 - formatting p-values p<0.0001
#			eqtable[i,5]<-0.0001
			eqtable[i,5]=format(round(0.0001, 4), nsmall=4, scientific=FALSE)
			eqtable[i,5]<- paste("<",eqtable[i,5])
		}
	}

	HTML(eqtable, classfirstline="second", align="left")

	add<-paste(c("Conclusion"))
	if (eqtable[i,5]<= (1-sig))
	{
		add<-paste(add, ": There is a statistically significant difference between the levels of ", sep="")

#STB - July 2012 rename response variable
#		add<-paste(add, response, " at the ", 100*(1-sig) , "% level" , sep="")
		add<-paste(add, xxxresponsexxx, " at the ", 100*(1-sig) , "% level" , sep="")
		add<-paste(add, ".", sep="")
	} else {
		add<-paste(add, ": The difference between the levels of ", sep="")

#STB - July 2012 rename response variable
#		add<-paste(add, response, sep="")
		add<-paste(add, xxxresponsexxx, sep="")
		add<-paste(add, " is not statistically significant.", sep="")
		}
	HTML.title("</bf> ", HR=2, align="left")
	HTML.title(add, HR=0, align="left")


#===================================================================================================================
#Testing the degrees of freedom

	if (eqtest[2]<5)
	{
		HTMLbr()
		HTML.title("<bf>Warning", HR=2, align="left")
	
		add<-c("Unfortunately the residual degrees of freedom are low (less than 5). This may make the estimation of the underlying variability, and hence the results of the statistical tests, unreliable.")
		HTML.title("</bf> ", HR=2, align="left")
		HTML.title(add, HR=0, align="left")
		print(add)
	}

#===================================================================================================================
# Means and Planned comparisons on the main effects

#Code to calculate y-axis offset (lens) in LSMeans plot
	names<-levels(eval(parse(text = paste("statdata$", treatFactor))))
	index<-1
	for (i in 1:length(names))
	{
		temp<-names[i]
		temp<-as.character(unlist(strsplit(as.character(names[i]),"")))
		lens<-length(temp)
		if (lens>index)
		{
			index <-lens
		}
	}

#STB - July 2012 rename response variable
#	mult<-glht(lm(eval(parse(text = paste("statdata$", response)))~ mainEffect, data=statdata, na.action = na.omit), linfct=mcp(mainEffect="Means"))
	mult<-glht(lm(eval(parse(text = paste("statdata$", xxxresponsexxx)))~ mainEffect, data=statdata, na.action = na.omit), linfct=mcp(mainEffect="Means"))
#	meanPlot <- sub(".html", "meanplot.jpg", htmlFile)
#	jpeg(meanPlot,width = jpegwidth, height = jpegheight, quality = 100)
#no pdf for this plot as its not in the output

	tab1<-c()
	tab2<-c()
	tab3<-c()

	tabs<-confint(mult,level = sig, calpha = univariate_calpha() )
	test<-tabs$confint
	lengths<-length(unique(rownames(test)))

	for (i in 1:lengths)
	{
		tab1[i]<-test[i,1]
		tab2[i]<-test[i,2]
		tab3[i]<-test[i,3]
	}
#STB Dec 2011 formatting 3dp
	tab1<-format(round(tab1, 3), nsmall=3, scientific=FALSE)
	tab2<-format(round(tab2, 3), nsmall=3, scientific=FALSE)
	tab3<-format(round(tab3, 3), nsmall=3, scientific=FALSE)

	table<-c(1:lengths)
	for (i in 1:lengths)
	{
		table[i]=" "
	}

	tab <- cbind(table, tab1, table, tab2, table, tab3)

	colnames(tab)<-c(" ", "Mean", " ", paste("Lower ",(sig*100),"% CI",sep=""), " ", paste("Upper ",(sig*100),"% CI",sep=""))
	header<-c(" ", " "," "," ", " "," ")
	tables<-rbind(header, tab)
	rownames(tables)<-c("Treatment level", rownames(test))

	HTMLbr()
#STB May 2012 Updating "least square (predicted) means"
	CITitle2<-paste("<bf>Table of the least square (predicted) means with ",(sig*100),"% confidence intervals (assuming equal variances)",sep="")
	HTML.title(CITitle2, HR=2, align="left")

	HTML(tables, classfirstline="second", align="left")


#===================================================================================================================
#Calculating the size of the arithmetic difference with 95%CI
#V3.2 STB NOV2015

	add<-paste("Comparison of the  least square (predicted) means with ",(sig*100),"% confidence interval (assuming equal variances)")
	HTML.title(add, HR=2, align="left")

	mult2<-glht(lm(eval(parse(text = paste("statdata$", xxxresponsexxx)))~ mainEffect, data=statdata, na.action = na.omit), linfct=lsm(pairwise ~mainEffect))
	multci2<-confint(mult2, level=sig, calpha = univariate_calpha())
	multp2<-summary(mult2, test=adjusted("none"))

	pvals<-multp2$test$pvalues
	sigma<-multp2$test$sigma
	tablen<-length(unique(rownames(multci2$confint)))
	tabs<-matrix(nrow=tablen, ncol=5)

	for (i in 1:tablen)
	{
#STB Dec 2011 increasing means to 3dp
		tabs[i,1]=format(round(multci2$confint[i], 3), nsmall=3, scientific=FALSE)
	}

	for (i in 1:tablen)
	{
		tabs[i,2]=format(round(multci2$confint[i+tablen], 3), nsmall=3, scientific=FALSE)
	}

	for (i in 1:tablen)
	{
		tabs[i,3]=format(round(multci2$confint[i+2*tablen], 3), nsmall=3, scientific=FALSE)
	}

	for (i in 1:tablen)
	{
		tabs[i,4]=format(round(sigma[i], 3), nsmall=3, scientific=FALSE)
	}

	for (i in 1:tablen)
	{
		tabs[i,5]=format(round(pvals[i], 4), nsmall=4, scientific=FALSE)
	}

	for (i in 1:tablen) 
	{
		if (pvals[i]<0.0001) 
		{
#STB March 2011 - formatting p-values p<0.0001
#			tabs[i,5]<-0.0001
			tabs[i,5]=format(round(0.0001, 4), nsmall=4, scientific=FALSE)
			tabs[i,5]<- paste("<",tabs[i,5])
		}
	}
	
	header<-c(" ", " "," ", " "," ")

	rows<-rownames(multci2$confint)
	rows<-sub(" - "," vs. ", rows, fixed=TRUE)

#STB June 2015	
	for (i in 1:100)
	{
		rows<-sub("_ivs_dash_ivs_"," - ", rows, fixed=TRUE)
	}

	lowerCI<-paste("   Lower ",(sig*100),"% CI   ",sep="")
	upperCI<-paste("   Upper ",(sig*100),"% CI   ",sep="")

	tabls<-rbind(header, tabs)
	rownames(tabls)<-c("Comparison", rows)
	colnames(tabls)<-c("   Difference   ", lowerCI, upperCI, "   Std error   ", "   p-value   ")
	HTML(tabls, classfirstline="second", align="left")


#===================================================================================================================
#Back transformed geometric means plot and table 
#===================================================================================================================
	if(GeomDisplay == "Y" && (responseTransform =="Log10"||responseTransform =="Loge"))
	{
#Table of LS Means
		HTMLbr()
		CITitle2<-paste("Table of the back-transformed geometric means with ",(sig*100),"% confidence intervals",sep="")
		HTML.title(CITitle2, HR=2, align="left")

		add<-c("As the response was log transformed prior to analysis the least square (predicted) means are presented on the log scale. These results can be back transformed onto the original scale. These are known as the back-transformed geometric means.")
		HTML.title("</bf> ", HR=2, align="left")
		HTML.title(add, HR=0, align="left")

		tab1<-c()
		tab2<-c()
		tab3<-c()

		tabs<-confint(mult,level = sig, calpha = univariate_calpha() )
		test<-tabs$confint
		lengths<-length(unique(rownames(test)))

		for (i in 1:lengths)
		{
			tab1[i]<-test[i,1]
			tab2[i]<-test[i,2]
			tab3[i]<-test[i,3]
		}
#STB Dec 2011 formatting 3dp
		if (responseTransform =="Log10")
		{
			tab1<-format(round(10^(tab1), 3), nsmall=3, scientific=FALSE)
			tab2<-format(round(10^(tab2), 3), nsmall=3, scientific=FALSE)
			tab3<-format(round(10^(tab3), 3), nsmall=3, scientific=FALSE)
		}
		if (responseTransform =="Loge")
		{
			tab1<-format(round(exp(tab1), 3), nsmall=3, scientific=FALSE)
			tab2<-format(round(exp(tab2), 3), nsmall=3, scientific=FALSE)
			tab3<-format(round(exp(tab3), 3), nsmall=3, scientific=FALSE)
		}


		table<-c(1:lengths)
		for (i in 1:lengths)
		{	
			table[i]=" "
		}

		tab <- cbind(table, tab1, table, tab2, table, tab3)

		colnames(tab)<-c(" ", "Geometric mean", " ", paste("Lower ",(sig*100),"% CI",sep=""), " ", paste("Upper ",(sig*100),"% CI",sep=""))
		header<-c(" ", " "," "," ", " "," ")
		tables<-rbind(header, tab)
		rownames(tables)<-c("Treatment level", rownames(test))

		HTML(tables, classfirstline="second", align="left")
	

#===================================================================================================================
#Calculating the size of the geometric ratio with 95%CI
#V3.2 STB NOV2015

		add<-paste(c("Comparison between the back-transformed geometric means as a back-transformed ratio (assuming equal variances)"))
		HTMLbr()
		HTML.title(add, HR=2, align="left")

		add<-c("As the response was log transformed prior to analysis the differences between the least square (predicted) means are presented on the log scale. These results can be back-transformed onto the original scale, where differences on the log scale become ratios when back-transformed.")
		HTML.title("</bf> ", HR=2, align="left")
		HTML.title(add, HR=0, align="left")

		mult<-glht(lm(eval(parse(text = paste("statdata$", xxxresponsexxx)))~ mainEffect, data=statdata, na.action = na.omit), linfct=lsm(pairwise ~mainEffect))
		multci<-confint(mult, level=sig, calpha = univariate_calpha())
		multp<-summary(mult, test=adjusted("none"))

		pvals<-multp$test$pvalues
		sigma<-multp$test$sigma
		tablen<-length(unique(rownames(multci$confint)))
		tabs<-matrix(nrow=tablen, ncol=3)

		if (responseTransform =="Log10")
		{
			for (i in 1:tablen)
			{
#STB Dec 2011 increasing means to 3dp
				tabs[i,1]=format(round(10^(multci$confint[i]), 3), nsmall=3, scientific=FALSE)
			}

			for (i in 1:tablen)
			{
				tabs[i,2]=format(round(10^(multci$confint[i+tablen]), 3), nsmall=3, scientific=FALSE)
			}

			for (i in 1:tablen)
			{
				tabs[i,3]=format(round(10^(multci$confint[i+2*tablen]), 3), nsmall=3, scientific=FALSE)
			}
		}

		if (responseTransform =="Loge")
		{
			for (i in 1:tablen)
			{
#STB Dec 2011 increasing means to 3dp
				tabs[i,1]=format(round(exp(multci$confint[i]), 3), nsmall=3, scientific=FALSE)
			}

			for (i in 1:tablen)
			{
				tabs[i,2]=format(round(exp(multci$confint[i+tablen]), 3), nsmall=3, scientific=FALSE)
			}

			for (i in 1:tablen)
			{
				tabs[i,3]=format(round(exp(multci$confint[i+2*tablen]), 3), nsmall=3, scientific=FALSE)
			}
		}
	
		header<-c(" ", " "," ")

		rows<-rownames(multci$confint)
		rows<-sub(" - "," vs. ", rows, fixed=TRUE)

#STB June 2015	
		for (i in 1:100)
		{
			rows<-sub("_ivs_dash_ivs_"," - ", rows, fixed=TRUE)
		}

		lowerCI<-paste("   Lower ",(sig*100),"% CI   ",sep="")
		upperCI<-paste("   Upper ",(sig*100),"% CI   ",sep="")

		tabls<-rbind(header, tabs)
		rownames(tabls)<-c("Comparison", rows)
		colnames(tabls)<-c("   Ratio  ", lowerCI, upperCI)
		HTML(tabls, classfirstline="second", align="left")



#End of Log statement
	}

#End ofequl varance case
}
#===================================================================================================================
#Unequal variance case

if (unequalCase == "Y")
{
	HTMLbr()
	HTML.title("<bf>Unpaired t-test assuming unequal variances", HR=2, align="left")

	add<-c("The analysis presented is Welch's t-test assuming unequal variances.")
	HTML.title("</bf> ", HR=2, align="left")
	HTML.title(add, HR=0, align="left")

#STB - July 2012 rename response variable
#	eqtest<-t.test(formula = eval(parse(text = paste("statdata$", response)))~eval(parse(text = paste("statdata$", treatFactor))), paired = FALSE, var.equal= FALSE, conf.level= sig)
	eqtest<-t.test(formula = eval(parse(text = paste("statdata$", xxxresponsexxx)))~eval(parse(text = paste("statdata$", treatFactor))), paired = FALSE, var.equal= FALSE, conf.level= sig)

	col1<-format(round(as.numeric(eqtest[1]), 3), nsmall=3, scientific=FALSE)
	col2<-format(round(as.numeric(eqtest[2]), 2), nsmall=2, scientific=FALSE)
	col3<-format(round(as.numeric(eqtest[3]), 4), nsmall=4, scientific=FALSE)
	col4<-as.numeric(eqtest[3])

	blanq<-c(" ")
	for (i in 1:1)
	{ 
		blanq[i]=" "
	}

	eqtable<-cbind(col1,blanq,col2,blanq,col3)

	head<-c("t-statistic", " ","Degrees of freedom"," ","p-value")
	colnames(eqtable)<-head
#STB May 2012 Updating "unpaired"
	rownames(eqtable)<-c("Unequal variance unpaired t-test")

	for (i in 1:(dim(eqtable)[1])) 
	{
		if (as.numeric(col4[i])<0.0001) 
		{
#STB March 2011 formatting p-values p<0.0001
#			eqtable[i,5]<-0.0001
			eqtable[i,5]=format(round(0.0001, 4), nsmall=4, scientific=FALSE)
			eqtable[i,5]<- paste("<",eqtable[i,5])
		}
	}

	HTML(eqtable, classfirstline="second", align="left")

	add<-paste(c("Conclusion"))
	if (eqtable[i,5]<= (1-sig))
	{
		add<-paste(add, ": There is a statistically significant difference between the levels of ", sep="")

#STB - July 2012 rename response variable
#		add<-paste(add, response, " at the ", 100*(1-sig) , "% level" , sep="")
		add<-paste(add, xxxresponsexxx, " at the ", 100*(1-sig) , "% level" , sep="")
		add<-paste(add, ".", sep="")
	} else {
		add<-paste(add, ": The difference between the levels of ", sep="")

#STB - July 2012 rename response variable
#		add<-paste(add, response, sep="")
		add<-paste(add, xxxresponsexxx, sep="")
		add<-paste(add, " is not statistically significant.", sep="")
		}
	HTML.title("</bf> ", HR=2, align="left")
	HTML.title(add, HR=0, align="left")

#===================================================================================================================
#Testing the degrees of freedom
	if (eqtest[2]<5)
	{
		HTMLbr()
		HTML.title("<bf>Warning", HR=2, align="left")
	
		add<-c("Unfortunately the residual degrees of freedom are low (less than 5). This may make the estimation of the underlying variability, and hence the results of the statistical tests, unreliable.")
		HTML.title("</bf> ", HR=2, align="left")
		HTML.title(add, HR=0, align="left")
		print(add)
	}

#===================================================================================================================
# Means and Planned comparisons on the main effects
	tablenames<-c(levels(as.factor(statdata$mainEffect)))
	length <- length(unique(levels(as.factor(statdata$mainEffect))))
	table<-c(1:length)
	for (i in 1:length)
	{
		table[i]=" "
	}
	vectorUCI <-c(1:length)
	vectorLCI <-c(1:length)
	vectormean <-c(1:length)
	for (i in 1:length)
	{
		sub<-subset(statdata, statdata$mainEffect == unique(levels(as.factor(statdata$mainEffect)))[i])
		sub2<-data.frame(sub)

#STB - July 2012 rename response variable
#		tempy<-na.omit(eval(parse(text = paste("sub2$", response))))
		tempy<-na.omit(eval(parse(text = paste("sub2$", xxxresponsexxx))))

#STB - July 2012 rename response variable
#		vectormean[i] = mean(eval(parse(text = paste("sub2$", response))), na.rm=TRUE)
		vectormean[i] = mean(eval(parse(text = paste("sub2$", xxxresponsexxx))), na.rm=TRUE)

#STB - July 2012 rename response variable
#		vectorLCI[i]= mean(eval(parse(text = paste("sub2$", response))), na.rm=TRUE)-qt(1-(1-sig)/2, (length(tempy)-1))*sd(eval(parse(text = paste("sub2$", response))), na.rm=TRUE) / (length(tempy))**(0.5)
#		vectorUCI[i]= mean(eval(parse(text = paste("sub2$", response))), na.rm=TRUE)+qt(1-(1-sig)/2, (length(tempy)-1))*sd(eval(parse(text = paste("sub2$", response))), na.rm=TRUE) / (length(tempy))**(0.5)
		vectorLCI[i]= mean(eval(parse(text = paste("sub2$", xxxresponsexxx))), na.rm=TRUE)-qt(1-(1-sig)/2, (length(tempy)-1))*sd(eval(parse(text = paste("sub2$", xxxresponsexxx))), na.rm=TRUE) / (length(tempy))**(0.5)
		vectorUCI[i]= mean(eval(parse(text = paste("sub2$", xxxresponsexxx))), na.rm=TRUE)+qt(1-(1-sig)/2, (length(tempy)-1))*sd(eval(parse(text = paste("sub2$", xxxresponsexxx))), na.rm=TRUE) / (length(tempy))**(0.5)
	}

#Required for back transformation 
	vectormean2<- vectormean
	vectorLCI2<-vectorLCI
	vectorUCI2 <- vectorUCI

#STB Dec 2011 formatting 3dp
	vectormean<-format(round(vectormean, 3), nsmall=3, scientific=FALSE)
	table2<-cbind(table,vectormean)
	vectorLCI<-format(round(vectorLCI, 3), nsmall=3, scientific=FALSE)
	table2<-cbind(table2,table)
	table2<-cbind(table2,vectorLCI)
	vectorUCI<-format(round(vectorUCI, 3), nsmall=3, scientific=FALSE)
	table2<-cbind(table2,table)
	table2<-cbind(table2,vectorUCI)

	header<-c(" "," "," ", " ", " ", " ")
	temp6<-c("Mean")
	temp7<-c(" ")

	CIlow<-paste("Lower ", 100*(sig), sep="")
	CIlow<-paste(CIlow , "% CI", sep="")
	hed12<-c(CIlow)

	temp6<-cbind(temp7, temp6,temp7)
	temp6<-cbind(temp6,hed12)
	temp6<-cbind(temp6,temp7)

	CIhigh<-paste("Upper ", 100*(sig), sep="")
	CIhigh<-paste(CIhigh, "% CI", sep="")
	hed13<-c(CIhigh)
	temp6<-cbind(temp6,hed13)
	table2<-rbind(header, table2)

	colnames(table2)<-temp6
	rownms<-c("Treatment level")
	for(i in 1:length)
	{
		rownms[i+1]<-levels(as.factor(statdata$mainEffect))[i]
	}
	
	row.names(table2)<-rownms

	HTMLbr()
#STB May 2012 Updating "least square (predicted) means"
	CITitle2<-paste("<bf>Table of the least square (predicted) means with ",(sig*100),"% confidence intervals (assuming unequal variances)",sep="")
	HTML.title(CITitle2, HR=2, align="left")

	HTML(table2, , align="left" , classfirstline="second")
	HTML.title("</bf> ", HR=2, align="left")


#===================================================================================================================
#Calculating the size of the arithmetic difference with 95%CI
#V3.2 STB NOV2015

	add<-paste(c("Comparison of the  least square (predicted) means with ",(sig*100),"% confidence interval (assuming unequal variances)"))
	HTMLbr()
	HTML.title(add, HR=2, align="left")

#Required to generate table label only
	mult3<-glht(lm(eval(parse(text = paste("statdata$", xxxresponsexxx)))~ mainEffect, data=statdata, na.action = na.omit), linfct=lsm(pairwise ~mainEffect))
	multci3<-confint(mult3, level=sig, calpha = univariate_calpha())

	mult2<-t.test(formula = eval(parse(text = paste("statdata$", xxxresponsexxx)))~eval(parse(text = paste("statdata$", treatFactor))), paired = FALSE, var.equal= FALSE, conf.level= sig)

	pvals<-mult2$p.value
	meandiff<- mult2$estimate[1] - mult2$estimate[2]
	lowerdiff<- mult2$conf.int[1]
	upperdiff<- mult2$conf.int[2]
	tablen<-1
	tabs<-matrix(nrow=tablen, ncol=4)

	for (i in 1:tablen)
	{
#STB Dec 2011 increasing means to 3dp
		tabs[i,1]=format(round(meandiff, 3), nsmall=3, scientific=FALSE)
	}

	for (i in 1:tablen)
	{
		tabs[i,2]=format(round(lowerdiff, 3), nsmall=3, scientific=FALSE)
	}

	for (i in 1:tablen)
	{
		tabs[i,3]=format(round(upperdiff, 3), nsmall=3, scientific=FALSE)
	}

	for (i in 1:tablen)
	{
		tabs[i,4]=format(round(pvals, 4), nsmall=4, scientific=FALSE)
	}

	for (i in 1:tablen) 
	{
		if (pvals[i]<0.0001) 
		{
#STB March 2011 - formatting p-values p<0.0001
#			tabs[i,4]<-0.0001
			tabs[i,4]=format(round(0.0001, 4), nsmall=4, scientific=FALSE)
			tabs[i,4]<- paste("<",tabs[i,4])
		}
	}
	
	header<-c(" ", " "," ", " ")

	rows<-rownames(multci3$confint)
	rows<-sub(" - "," vs. ", rows, fixed=TRUE)

#STB June 2015	
	for (i in 1:100)
	{
		rows<-sub("_ivs_dash_ivs_"," - ", rows, fixed=TRUE)
	}

	lowerCI<-paste("   Lower ",(sig*100),"% CI   ",sep="")
	upperCI<-paste("   Upper ",(sig*100),"% CI   ",sep="")

	tabls<-rbind(header, tabs)
	rownames(tabls)<-c("Comparison", rows)
	colnames(tabls)<-c("   Difference   ", lowerCI, upperCI, "   p-value   ")
	HTML(tabls, classfirstline="second", align="left")


#===================================================================================================================
#Back transformed geometric means plot and table 
#===================================================================================================================
	if(GeomDisplay == "Y" && (responseTransform =="Log10"||responseTransform =="Loge"))
	{
#Table of LS Means
		HTMLbr()
		CITitle2<-paste("<bf>Table of the back-transformed geometric means with ",(sig*100),"% confidence intervals (assuming unequal variances)",sep="")
		HTML.title(CITitle2, HR=2, align="left")

		add<-c("As the response was log transformed prior to analysis the least square (predicted) means are presented on the log scale. These results can be back transformed onto the original scale. These are known as the back-transformed geometric means.")
		HTML.title("</bf> ", HR=2, align="left")
		HTML.title(add, HR=0, align="left")

#STB Dec 2011 formatting 3dp
		if (responseTransform =="Log10")
		{
			vectormean2<-format(round(10^(vectormean2), 3), nsmall=3, scientific=FALSE)
			table2<-cbind(table,vectormean2)
			vectorLCI2<-format(round(10^(vectorLCI2), 3), nsmall=3, scientific=FALSE)
			table2<-cbind(table2,table)
			table2<-cbind(table2,vectorLCI2)
			vectorUCI2<-format(round(10^(vectorUCI2), 3), nsmall=3, scientific=FALSE)
		}

		if (responseTransform =="Loge")
		{
			vectormean2<-format(round(exp(vectormean2), 3), nsmall=3, scientific=FALSE)
			table2<-cbind(table,vectormean2)
			vectorLCI2<-format(round(exp(vectorLCI2), 3), nsmall=3, scientific=FALSE)
			table2<-cbind(table2,table)
			table2<-cbind(table2,vectorLCI2)
			vectorUCI2<-format(round(exp(vectorUCI2), 3), nsmall=3, scientific=FALSE)
		}

		table2<-cbind(table2,table)
		table2<-cbind(table2,vectorUCI2)

		header<-c(" "," "," ", " ", " ", " ")
		temp6<-c("Geometric mean")
		temp7<-c(" ")

		CIlow<-paste("Lower ", 100*(sig), sep="")
		CIlow<-paste(CIlow , "% CI", sep="")
		hed12<-c(CIlow)

		temp6<-cbind(temp7, temp6,temp7)
		temp6<-cbind(temp6,hed12)
		temp6<-cbind(temp6,temp7)

		CIhigh<-paste("Upper ", 100*(sig), sep="")
		CIhigh<-paste(CIhigh, "% CI", sep="")
		hed13<-c(CIhigh)
		temp6<-cbind(temp6,hed13)
		table2<-rbind(header, table2)

		colnames(table2)<-temp6
		rownms<-c("Treatment level")
		for(i in 1:length)
		{
			rownms[i+1]<-levels(as.factor(statdata$mainEffect))[i]
		}
	
		row.names(table2)<-rownms
	
		HTML(table2, , align="left" , classfirstline="second")
		HTML.title("</bf> ", HR=2, align="left")
	

#===================================================================================================================
#Calculating the size of the geometric ratio with 95%CI
#V3.2 STB NOV2015

		add<-paste(c("Comparison between the back-transformed geometric means as a back-transformed ratio (assuming unequal variances)"))
		HTMLbr()
		HTML.title(add, HR=2, align="left")

		add<-c("As the response was log transformed prior to analysis the differences between the least square (predicted) means are presented on the log scale. These results can be back-transformed onto the original scale, where differences on the log scale become ratios when back-transformed.")
		HTML.title("</bf> ", HR=2, align="left")
		HTML.title(add, HR=0, align="left")

#Required to generate table label only
	mult3<-glht(lm(eval(parse(text = paste("statdata$", xxxresponsexxx)))~ mainEffect, data=statdata, na.action = na.omit), linfct=lsm(pairwise ~mainEffect))
	multci3<-confint(mult3, level=sig, calpha = univariate_calpha())

	mult2<-t.test(formula = eval(parse(text = paste("statdata$", xxxresponsexxx)))~eval(parse(text = paste("statdata$", treatFactor))), paired = FALSE, var.equal= FALSE, conf.level= sig)

	meandiff<- mult2$estimate[1] - mult2$estimate[2]
	lowerdiff<- mult2$conf.int[1]
	upperdiff<- mult2$conf.int[2]
	tablen<-1
	tabs<-matrix(nrow=tablen, ncol=3)

		if (responseTransform =="Log10")
		{
			for (i in 1:tablen)
			{
#STB Dec 2011 increasing means to 3dp
				tabs[i,1]=format(round(10^(meandiff), 3), nsmall=3, scientific=FALSE)
			}

			for (i in 1:tablen)
			{
				tabs[i,2]=format(round(10^(lowerdiff), 3), nsmall=3, scientific=FALSE)
			}

			for (i in 1:tablen)
			{
				tabs[i,3]=format(round(10^(upperdiff), 3), nsmall=3, scientific=FALSE)
			}
		}

		if (responseTransform =="Loge")
		{
			for (i in 1:tablen)
			{
#STB Dec 2011 increasing means to 3dp
				tabs[i,1]=format(round(exp(meandiff), 3), nsmall=3, scientific=FALSE)
			}

			for (i in 1:tablen)
			{
				tabs[i,2]=format(round(exp(lowerdiff), 3), nsmall=3, scientific=FALSE)
			}

			for (i in 1:tablen)
			{
				tabs[i,3]=format(round(exp(upperdiff), 3), nsmall=3, scientific=FALSE)
			}
		}
	
		header<-c(" ", " "," ")

		rows<-rownames(multci3$confint)
		rows<-sub(" - "," vs. ", rows, fixed=TRUE)

#STB June 2015	
		for (i in 1:100)
		{
			rows<-sub("_ivs_dash_ivs_"," - ", rows, fixed=TRUE)
		}

		lowerCI<-paste("   Lower ",(sig*100),"% CI   ",sep="")
		upperCI<-paste("   Upper ",(sig*100),"% CI   ",sep="")

		tabls<-rbind(header, tabs)
		rownames(tabls)<-c("Comparison", rows)
		colnames(tabls)<-c("   Ratio   ", lowerCI, upperCI)
		HTML(tabls, classfirstline="second", align="left")



#End of Log statement
	}




#End of unequal varinaces code
}


#===================================================================================================================

#Diagnostic plot titles
if(showPRPlot=="Y")
{
 HTMLbr()
 HTML.title("<bf>Diagnostic plots (assuming equal variances)", HR=2, align="left")
} else {
 if(showNormPlot=="Y")
 {
  HTMLbr()
  HTML.title("<bf>Diagnostic plots (assuming equal variances)", HR=2, align="left")
 }
}

#Residual plots
if(showPRPlot=="Y")
{

#STB - July 2012 rename response variable
	threewayfull<-lm(eval(parse(text = paste("statdata$", xxxresponsexxx)))~ mainEffect, data=statdata, na.action = na.omit)

	residualPlot <- sub(".html", "residualplot.jpg", htmlFile)
	jpeg(residualPlot,width = jpegwidth, height = jpegheight, quality = 100)

#STB July2013
	plotFilepdf1 <- sub(".html", "residualplot.pdf", htmlFile)
	dev.control("enable") 

#===================================================================================================================
#Graphical parameters
graphdata<-data.frame(cbind(predict(threewayfull),rstudent(threewayfull)))

graphdata$yvarrr_IVS <- graphdata$X2
graphdata$xvarrr_IVS <- graphdata$X1
XAxisTitle <- "Predicted values"
YAxisTitle <- "Externally Studentised residuals"
MainTitle2 <- "Residuals vs. predicted plot \n"
w_Gr_jit <- 0
h_Gr_jit <-  0
infiniteslope <- "Y"

if (bandw != "N") 
{
	Gr_line <-BW_line
	Gr_fill <- BW_fill
} else {
	Gr_line <-Col_line
	Gr_fill <- Col_fill
	}
Gr_line_type<-Line_type_dashed

Line_size <- 0.5
#===================================================================================================================
#GGPLOT2 code
NONCAT_SCAT("RESIDPLOT")

MainTitle2 <- ""
#===================================================================================================================

	void<-HTMLInsertGraph(GraphFileName=sub("[A-Z0-9a-z,:,\\\\]*App_Data[\\\\]","", residualPlot), Align="centre")

#STB July2013
	if (pdfout=="Y")
	{
		pdf(file=sub("[A-Z0-9a-z,:,\\\\]*App_Data[\\\\]","", plotFilepdf1), height = pdfheight, width = pdfwidth) 
		dev.set(2) 
		dev.copy(which=3) 
		dev.off(2)
		dev.off(3)
		pdfFile_1<-sub("[A-Z0-9a-z,:,\\\\]*App_Data[\\\\]","",plotFilepdf1)
		linkToPdf1 <- paste ("<a href=\"",pdfFile_1,"\">Click here to view the PDF of the residuals vs. predicted plot</a>", sep = "")
		HTML(linkToPdf1)
	}

	HTML.title("<bf> ", HR=2, align="left")
	HTML.title("<bf>Tip: On this plot look to see if the spread of the points increases as the predicted values increase. If so the response may need transforming or the unequal variance assumption selected.", HR=0, align="left")
	HTML.title("<bf> ", HR=2, align="left")
	HTML.title("<bf>Tip: Any observation with a residual less than -3 or greater than 3 (SD) should be investigated as a possible outlier.", HR=0, align="left")
}

#===================================================================================================================
#Normality plots
if(showNormPlot=="Y")
{

#STB - July 2012 rename response variable
	threewayfull<-lm(eval(parse(text = paste("statdata$", xxxresponsexxx)))~ mainEffect, data=statdata, na.action = na.omit)

	HTMLbr()
	normPlot <- sub(".html", "normplot.jpg", htmlFile)
	jpeg(normPlot,width = jpegwidth, height = jpegheight, quality = 100)

#STB July2013
	plotFilepdf2 <- sub(".html", "normplot.pdf", htmlFile)
	dev.control("enable") 

#===================================================================================================================
#Graphical parameters

te<-qqnorm(resid(threewayfull))
graphdata<-data.frame(te$x,te$y)
graphdata$xvarrr_IVS <-graphdata$te.x
graphdata$yvarrr_IVS <-graphdata$te.y
YAxisTitle <-"Sample Quantiles"
XAxisTitle <-"Theoretical Quantiles"
MainTitle2 <- "Normal probability plot \n"
w_Gr_jit <- 0
h_Gr_jit <-  0
infiniteslope <- "N"
LinearFit <- "Y"

if (bandw != "N") 
{
	Gr_line <-BW_line
	Gr_fill <- BW_fill
} else {
	Gr_line <-Col_line
	Gr_fill <- Col_fill
	}
Gr_line_type<-Line_type_dashed

Line_size <- 0.5
Gr_alpha <- 1
Line_type <-Line_type_dashed
#===================================================================================================================
#GGPLOT2 code
NONCAT_SCAT("QQPLOT")

MainTitle2 <- ""
#===================================================================================================================

	void<-HTMLInsertGraph(GraphFileName=sub("[A-Z0-9a-z,:,\\\\]*App_Data[\\\\]","", normPlot), Align="left")

#STB July2013
	if (pdfout=="Y")
	{
		pdf(file=sub("[A-Z0-9a-z,:,\\\\]*App_Data[\\\\]","", plotFilepdf2), height = pdfheight, width = pdfwidth) 
		dev.set(2) 
		dev.copy(which=3) 
		dev.off(2)
		dev.off(3)
		pdfFile_2<-sub("[A-Z0-9a-z,:,\\\\]*App_Data[\\\\]","",plotFilepdf2)
		linkToPdf2 <- paste ("<a href=\"",pdfFile_2,"\">Click here to view the PDF of the normal probability plot</a>", sep = "")
		HTML(linkToPdf2)
	}

	HTML.title("<bf> ", HR=2, align="left")
	HTML.title("<bf>Tip: Check that the points lie along the dotted line. If not then the data may be non-normally distributed.", HR=0, align="left")
}

#===================================================================================================================
#References
#===================================================================================================================
Ref_list<-R_refs()

HTML.title("Statistical references", HR=2, align="left")
HTML(Ref_list$BateClark_ref, align="left")

if(unequalCase == "Y") {
	HTML("Welch BL. (1947). The generalization of Student's problem when several different population variances are involved. Biometrika, 34(1-2), 28-35.", align="left")
}

HTML.title("R references", HR=2, align="left")
HTML(Ref_list$R_ref ,  align="left")
HTML(Ref_list$GGally_ref,  align="left")
HTML(Ref_list$RColorBrewers_ref,  align="left")
HTML(Ref_list$GGPLot2_ref,  align="left")
HTML(Ref_list$reshape_ref,  align="left")
HTML(Ref_list$plyr_ref,  align="left")
HTML(Ref_list$scales_ref,  align="left")
HTML(Ref_list$R2HTML_ref,  align="left")
HTML(Ref_list$PROTO_ref,  align="left")
HTML(Ref_list$multcomp_ref,  align="left")

#===================================================================================================================
#Show dataset
#===================================================================================================================
if (showdataset=="Y") {
	statdata2<-subset(statdata, select = -c(mainEffect))

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

HTML(paste("Response variable: ", xxxresponsexxx, sep=""),  align="left")

if (responseTransform != "None")
{ 
	HTML(paste("Response transformation: ", responseTransform, sep=""), align="left")
}

HTML(paste("Treatment variable: ", treatFactor, sep=""),  align="left")
HTML(paste("Display equal variance case (Y/N): ", equalCase, sep=""),  align="left")
HTML(paste("Display unequal variance case (Y/N): ", unequalCase, sep=""),  align="left")
HTML(paste("Show residuals vs. predicted plot (Y/N): ", showPRPlot, sep=""), align="left")
HTML(paste("Display normal probability plot (Y/N): ", showNormPlot, sep=""),  align="left")
HTML(paste("Significance level: ", 1-sig, sep=""), align="left")

