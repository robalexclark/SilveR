#===================================================================================================================
#R Libraries
suppressWarnings(library(R2HTML))
suppressWarnings(library(emmeans))
suppressWarnings(library(car))
suppressWarnings(library(mmrm))


#===================================================================================================================
# retrieve args
Args <- commandArgs(TRUE)

#Read in data
statdata <- read.csv(Args[3], header=TRUE, sep=",")

#Copy Args
model <- as.formula(Args[4])
timeFactor <- Args[5]
subjectFactor <- Args[6]
covariatelist <- Args[7]
covariance <- tolower(Args[8])
compareCovarianceModels <- Args[9]
responseTransform <- tolower(Args[10])
covariateTransform <- tolower(Args[11])
blocklist <- Args[12]
showANOVA <- Args[13]
showPRPlot <- Args[14]
showNormPlot <- Args[15]
showComps <- Args[16]
controlGroup <- Args[17]
sig <- 1 - as.numeric(Args[18])
showLSMeans <- Args[19]

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

#STB 14OCT2015
#Set contrast options for Marginal overall tests
#options(contrasts=c(unordered="contr.sum", ordered="contr.poly"))

# Setting up the factor names
resp <- unlist(strsplit(Args[4],"~"))[1] #get the response variable from the main model
statdata$Animal_IVS<-as.factor(eval(parse(text = paste("statdata$", subjectFactor))))
statdata$Time_IVS<-as.factor(eval(parse(text = paste("statdata$", timeFactor))))
#statdata$Time_IVS<-as.factor(eval(parse(text = paste("statdata$", timeFactor))))
statdata$yvarrr_IVS <- eval(parse(text = paste("statdata$", resp)))

#Re-ordering factor levels based on control group
dimfact<-length(unique(statdata$Time_IVS))

if (dimfact ==2 && controlGroup != "NULL") {
  temp <- c(levels(statdata$Time_IVS))
  temp = temp[!(temp %in% controlGroup)]
  levs_plot <- c(controlGroup, temp)
  statdata$Time_IVS <- factor(statdata$Time_IVS, levels=levs_plot)
}
statdata<-statdata[order(statdata$Animal_IVS, statdata$Time_IVS), ]

#Creating dataset for plotting
graphdata<-statdata

#Graphics parameter setup
Gr_palette<-palette_FUN(timeFactor)
Labelz_IVS_ <- "N"
Line_size2 <- Line_size
DisplayLSMeanslines <- "Y"
ReferenceLine <- "NULL"
XLimLow <- "NULL"
XLimHigh <- "NULL"
YLimLow <- "NULL"
YLimHigh <- "NULL"

#STB June 2015 - Taking a copy of the dataset - try to remove this dataset, required in pairwis etests zzz
#statdata_temp <-statdata

#Block factors
noblockfactors=0
if (blocklist !="NULL") {
  
  #calculating number of block factors
  tempblockChanges <-strsplit(blocklist, ",")
  blocklistx <- c("")
  for(i in 1:length(tempblockChanges[[1]]))  {
    blocklistx [length(blocklistx )+1]=(tempblockChanges[[1]][i]) 
  }
  blocklistsep <- blocklistx[-1]
  noblockfactors<-length(blocklistsep)
  
  #Defining blocks as factors
  for (i in 1:length(blocklistsep)) {
    colname <- blocklistsep[i]
    statdata[[colname]] <- as.factor(statdata[[colname]])
  }	
}

#calculating number of covariates
if (covariatelist !="NULL") {
  tempcovChanges <-strsplit(covariatelist, ",")
  txtexpectedcovChanges <- c("")
  for(i in 1:length(tempcovChanges[[1]]))  {
    txtexpectedcovChanges [length(txtexpectedcovChanges )+1]=(tempcovChanges[[1]][i]) 
  }
  covlistsep <- txtexpectedcovChanges[-1]
  nocovlist<-length(covlistsep)
}

#Defining titles
YAxisTitle <-resp
if(covariatelist !="NULL") {
  XAxisTitleCov<-covlistsep
}
CPXAxisTitle <-timeFactor

#Removing illegal characters from titles
for (i in 1:10) {
  YAxisTitle<-namereplace(YAxisTitle)
  CPXAxisTitle<-namereplace(CPXAxisTitle)
  timeFactor_plot <- namereplace(timeFactor)
  
  if(covariatelist != "NULL") {
    for (i in 1: nocovlist) {
      XAxisTitleCov[i]<-namereplace(XAxisTitleCov[i])
    }
  }
}
BTYAxisTitle<-YAxisTitle

#Add transformation to axis labels
if (responseTransform != "none") {
  YAxisTitle<-axis_relabel(responseTransform, YAxisTitle)
}

if(covariatelist != "NULL") {
  for (i in 1: nocovlist) {
    #Add transformation to axis labels
    if (covariateTransform != "none") {
      XAxisTitleCov[i]<-axis_relabel(covariateTransform, XAxisTitleCov[i])
    }
  }
}
LS_YAxisTitle<-YAxisTitle

#Taking default factor levels
levs_plot <- c(levels(statdata$Time_IVS))

#===================================================================================================================
# Titles and description
#===================================================================================================================
#STB Aug 2014 updating paired t-test to exclude covariate
if (dimfact ==2 ) { #&& covariatelist == "NULL") {
  #STB May 2012 Updating "paired"
  Title <-paste(branding, " Extended Paired t-test Analysis", sep="")
  HTML.title(Title, HR = 1, align = "left")
} else {
  Title <-paste(branding, " Within-Subject Analysis", sep="")
  HTML.title(Title, HR = 1, align = "left")
}

#Software developement version warning
if (Betawarn == "Y") {
  HTML.title("Warning", HR=2, align="left")
  HTML(BetaMessage, align="left")
}

#Response and covariate title
title<-c("Response")
if(covariatelist !="NULL") {
  if (nocovlist == 1) {
    title<-paste(title, ", covariate", sep="")
  } else {
    title<-paste(title, ", covariates", sep="")
  }
}
if (dimfact >2) {
  title<-paste(title, " and covariance structure", sep="")
}
HTML.title(title, HR=2, align="left")

add<-paste(c("The  "), resp, " response is currently being analysed by the Extended Paired t-test Analysis and Within-Subject Analysis module", sep="")

if(covariatelist !="NULL") {
  if (nocovlist == 1) {
    add<-paste(add, ", with ", covlistsep[1] , " fitted as a covariate. ", sep="")
  } 
  if (nocovlist == 2) {
    add<-paste(add, ", with ", covlistsep[1] , " and ", covlistsep[2] ," fitted as covariates. ", sep="")
  }
  if (nocovlist > 2) {	
    add<-paste(add, ", with ", sep="")	
    for (i in 1: (nocovlist -2)) {
      add <- paste (add, covlistsep[i],  ", " , sep="")
    }
    add<-paste(add, covlistsep[(nocovlist -1)] , " and ", covlistsep[nocovlist] , " fitted as covariates. ", sep="")
  }
} else {
  add<-paste(add, ". ", sep="")
}
if (responseTransform != "none") {
  add<-paste(add, c("The response has been "), responseTransform, " transformed prior to analysis. ", sep="")
}
if (covariatelist !="NULL" &&  covariateTransform != "none") {
  add<-paste(add, c("The covariate has been "), covariateTransform, " transformed prior to analysis.", sep="")
}
HTML(add, align="left")

if(covariance=="compound symmetric" && dimfact >2) {
  add4<-paste("The repeated measures mixed model analysis is using the compound symmetric covariance structure to model the within-subject correlations. When using this structure you are assuming sphericity and also that the variability of responses is the same at each level of ", timeFactor, ", see Pinherio and Bates (2002). These assumptions may not hold in practice.", sep= "")
  HTML(add4, align="left")
}
if(covariance=="autoregressive(1)" && dimfact >2) {
  add4<-paste("The repeated measures mixed model analysis is using the first order autoregressive covariance structure to model the within-subject correlations. When using this structure you are assuming the levels of ", timeFactor, " are equally spaced and also that the variability of responses are the same at each level of ", timeFactor, ", see Pinherio and Bates (2002). These assumptions may not hold in practice.", sep= "")
  HTML(add4, align="left")
  HTML("Warning: Make sure that the levels of the treatment factor occur in the correct order in the least square (predicted) means table. If they do not then this analysis may not be valid. The autoregressive covariance structure assumes that the order of the treatment factor levels is as defined in the least square (predicted) means table.", align="left")
}
if(covariance=="unstructured" && dimfact >2) {
  HTML("The repeated measures mixed model analysis is using the unstructured covariance structure to model the within-subject correlations. When using this structure you are estimating many parameters. If the numbers of subjects used is small then these estimates may be unreliable, see Pinherio and Bates (2002).", align="left")
}

#===================================================================================================================
#Case profiles plot
#===================================================================================================================
title<-c("Case profiles plot of the observed data")
if(responseTransform != "none") {
  title<-paste(title, " (on the ", responseTransform, " scale)", sep="")
}
HTML.title(title, HR=2, align="left")

scatterPlot <- sub(".html", "scatterPlot.png", htmlFile)
png(scatterPlot,width = jpegwidth, height = jpegheight, units="in", res=PlotResolution)

#STB July2013
plotFilepdf1 <- sub(".html", "scatterPlot.pdf", htmlFile)
dev.control("enable") 

#Parameter setup
graphdata$Time_IVS <- as.factor(eval(parse(text = paste("graphdata$", timeFactor))))

#Re-ordering factor levels based on control group
if (controlGroup != "NULL") {
  graphdata$Time_IVS <- factor(graphdata$Time_IVS, levels=levs_plot)
}
graphdata$yvarrr_IVS <- eval(parse(text = paste("graphdata$", resp)))
graphdata$Animal_IVS <- as.factor(eval(parse(text = paste("graphdata$", subjectFactor))))
graphdata$l_l <-graphdata$between
XAxisTitle<-CPXAxisTitle
MainTitle2 <-""
ReferenceLine = "NULL"
ShowCaseIDsInLegend <- "N"

#Colour range for individual animals on case profiles plot
temp<-IVS_F_cpplot_colour("Animal_IVS")
Gr_palette_A <- temp$Gr_palette_A
Gr_line <- temp$Gr_line
Gr_fill <-temp$Gr_fill

#GGPLOT2 code
NONCAT_CPP()

void <- HTMLInsertGraph(GraphFileName=sub("[A-Z0-9a-z,:,\\\\]*App_Data[\\\\]","", scatterPlot), Align="centre")

#STB July2013
if (pdfout=="Y") {
  pdf(file=sub("[A-Z0-9a-z,:,\\\\]*App_Data[\\\\]","", plotFilepdf1), height = pdfheight, width = pdfwidth) 
  dev.set(2) 
  dev.copy(which=3) 
  dev.off(2)
  dev.off(3)
  pdfFile_1<-sub("[A-Z0-9a-z,:,\\\\]*App_Data[\\\\]","",plotFilepdf1)
  linkToPdf1 <- paste ("<a href=\"",pdfFile_1,"\">Click here to view the PDF of the case profiles plot</a>", sep = "")
  HTML(linkToPdf1)
}

#===================================================================================================================
# Scatterplot of one response vs another (paired t-test only)
#===================================================================================================================
#STB Aug 2014 updating paired t-test to exclude covariate
if (dimfact ==2) {
  
  #Title
  title<-paste("Scatterplot of factor ", timeFactor, " level ", levels(statdata$Time_IVS)[1], " vs. level ", levels(statdata$Time_IVS)[2], sep="")
  HTML.title(title, HR=2, align="left")
  HTML("(including a dotted line indicating the 1:1 relationship)", align="left")
  
  #Seperating dataset
  plotdata<-data.frame(matrix(NA, nrow = length(levels(statdata$Animal_IVS)), ncol = 4))
  index <- 1
  
  for (i in 1:length(levels(statdata$Animal_IVS))) {
    temp<- statdata[ which(statdata$Animal_IVS==levels(statdata$Animal_IVS)[i]),]
    
    for (j in 1:length(levels(statdata$Time_IVS))) {
      temp2<- temp[ which(temp$Time_IVS==levels(temp$Time_IVS)[j]),]
      plotdata[i,j] <- eval(parse(text = paste("temp2$", resp)))[1]
      plotdata[i,(j+2)] <- temp2$Animal_IVS[1]
    }
    index<index+1
  }

  #Plot code
  ncscatterplot3 <- sub(".html", "ncscatterplot3.png", htmlFile)
  png(ncscatterplot3,width = jpegwidth, height = jpegheight, units="in", res=PlotResolution)
  
  #STB July2013
  plotFilepdf2 <- sub(".html", "ncscatterplot3.pdf", htmlFile)
  dev.control("enable") 
  
  #Graphical parameters
  graphdata<-plotdata
  graphdata$xvarrr_IVS <- plotdata$X1
  graphdata$yvarrr_IVS <- plotdata$X2
  
  YTITLE<-YAxisTitle	
  
  TimeAxisTitle<-namereplace(timeFactor)
  XAxisTitle <- paste(YAxisTitle, " [", TimeAxisTitle, ": level = ", levels(statdata$Time_IVS)[1], "]", sep = "")
  YAxisTitle <- paste(YAxisTitle, " [", TimeAxisTitle, ": level = ", levels(statdata$Time_IVS)[2], "]", sep = "")
  
  MainTitle2 <-""
  w_Gr_jitscat <- 0
  h_Gr_jitscat <- 0
  Gr_alpha <- 1
  Gr_line_type<-Line_type_dashed
  
  LinearFit <- "Y"
  ScatterPlot <- "Y"
  
  infiniteslope <- "Y"
  
  if (bandw != "N")  {
    Gr_line <-BW_line
    Gr_fill <- BW_fill
  } else {
    Gr_line <-Col_line
    Gr_fill <- Col_fill
  }
  Gr_line_type<-Line_type_dashed
  Line_size <- 0.5
  
  #GGPLOT2 code
  NONCAT_SCAT("PAIRED")
  #===================================================================================================================
  void<-HTMLInsertGraph(GraphFileName=sub("[A-Z0-9a-z,:,\\\\]*App_Data[\\\\]","", ncscatterplot3), Align="centre")
  
  #STB July2013
  if (pdfout=="Y")
  {
    pdf(file=sub("[A-Z0-9a-z,:,\\\\]*App_Data[\\\\]","", plotFilepdf2), height = pdfheight, width = pdfwidth) 
    dev.set(2) 
    dev.copy(which=3) 
    dev.off(2)
    dev.off(3)
    pdfFile_2<-sub("[A-Z0-9a-z,:,\\\\]*App_Data[\\\\]","",plotFilepdf2)
    linkToPdf2 <- paste ("<a href=\"",pdfFile_2,"\">Click here to view the PDF of the covariate plot</a>", sep = "")
    HTML(linkToPdf2)
  }
  
  #Put y axis title back as default
  YAxisTitle<-YTITLE
}

#===================================================================================================================
#Covariate plot
#===================================================================================================================
if(covariatelist != "NULL") {
  
  if (nocovlist == 1) {
    title<-c("Plot of the response vs. the covariate")
  } else {
    title<-c("Plot of the response vs. the covariates")
  }
  if(responseTransform != "none" || covariateTransform != "none") {
    title<-paste(title, " (on the transformed scale)", sep="")
  } 
  title<-paste(title, ", categorised by the primary factor", sep="")
  HTML.title(title, HR=2, align="left")
  
  index <- 1
  for (i in 1:nocovlist) {
    ncscatterplot3 <- sub(".html", "IVS", htmlFile)
    ncscatterplot3 <- paste(ncscatterplot3, index, "ncscatterplot3.png", sep = "")
    png(ncscatterplot3,width = jpegwidth, height = jpegheight, units="in", res=PlotResolution)
    
    #STB July2013
    plotFilepdf2 <- sub(".html", "IVS", htmlFile)
    plotFilepdf2 <- paste(plotFilepdf2, index, "ncscatterplot3.pdf", sep="")
    dev.control("enable") 
    
    #Graphical parameters
    graphdata<-statdata
    graphdata$xvarrr_IVS <- eval(parse(text = paste("graphdata$",covlistsep[i] )))
    graphdata$yvarrr_IVS <- eval(parse(text = paste("graphdata$",resp)))
    graphdata$Time_IVS <- as.factor(eval(parse(text = paste("graphdata$", timeFactor))))
    graphdata$tempvariable_ivs <- graphdata$Time_IVS
    graphdata$l_l <- graphdata$tempvariable_ivs
    graphdata$catfact <-graphdata$tempvariable_ivs
    
    XAxisTitle <- XAxisTitleCov[i]
    XAxisTitle<-namereplace(XAxisTitle)
    MainTitle2 <-""
    w_Gr_jitscat <- 0
    h_Gr_jitscat <- 0
    Legendpos <- "right"
    Gr_alpha <- 1
    Line_type <-Line_type_solid
    LinearFit <- "Y"
    GraphStyle <- "Overlaid"
    ScatterPlot <- "Y"
    FirstCatFactor <- "Temp"
    
    #Testing for with infinite slopes on scatterplot and re-ordering dataset if necessary
    inf_slope<-IVS_F_infinite_slope()
    infiniteslope <- inf_slope$infiniteslope
    graphdata<-inf_slope$graphdata
    graphdatax <- subset(graphdata, catvartest != "N")
    graphdata<-graphdatax
    
    #GGPLOT2 code
    OVERLAID_SCAT()
    
    void<-HTMLInsertGraph(GraphFileName=sub("[A-Z0-9a-z,:,\\\\]*App_Data[\\\\]","", ncscatterplot3), Align="centre")
    
    #STB July2013
    if (pdfout=="Y") {
      pdf(file=sub("[A-Z0-9a-z,:,\\\\]*App_Data[\\\\]","", plotFilepdf2), height = pdfheight, width = pdfwidth) 
      dev.set(2) 
      dev.copy(which=3) 
      dev.off(2)
      dev.off(3)
      pdfFile_2<-sub("[A-Z0-9a-z,:,\\\\]*App_Data[\\\\]","",plotFilepdf2)
      linkToPdf2 <- paste ("<a href=\"",pdfFile_2,"\">Click here to view the PDF of the covariate plot</a>", sep = "")
      HTML(linkToPdf2)
    }
    index <- index + 1
    
    #STB Aug 2011 - removing lines with infinite slope
    if (infiniteslope != "N") {
      title<-paste("Warning: The covariate has the same value for all subjects in one or more levels of the ", FirstCatFactor, " factor. Care should be taken if you want to include this covariate in the analysis.", sep="")
      HTML(title, align="left")
    }
  }
  HTML("Tip: In order to decide whether it is helpful to fit the covariate, the following should be considered:", align="left")
  HTML("a) Is there a relationship between the response and the covariate? (N.B., It is only worth fitting the covariate if there is a strong positive (or negative) relationship between them: i.e., the lines on the plot should not be horizontal).", align="left")
  HTML("b) Is the relationship similar for all treatments? (The lines on the plot should be approximately parallel).", align="left")
  HTML("c) Is the covariate influenced by the treatment? (We assume the covariate is not influenced by the treatment and so there should be no separation of the treatment groups along the x-axis on the plot).", align="left")
  HTML("These issues are discussed in more detail in Morris (1999).", align="left")
}

#===================================================================================================================
#building the covariate interaction model
#===================================================================================================================
if (AssessCovariateInteractions == "Y" && covariatelist != "NULL") {
  
  #Creating the list of model terms
  CovIntModel <- c(model)
  
  #Adding in additional interactions
  for (j in 1: nocovlist) {
    CovIntModel <- paste(CovIntModel, " + ",   "Time_IVS", " * ", covlistsep[j], sep="")
  }

  #Test to see if there are 0 df, in which case end module
  modelxx <- paste(CovIntModel , " + Animal_IVS", sep = "")
  threewayfullxx<-lm(as.formula(modelxx) , data=statdata, na.action = (na.omit))
  
  if (df.residual(threewayfullxx) < 2) {
    HTML.title("Table of overall tests of model effects, for assessing covariate interactions", HR=2, align="left")
    HTML("When the covariate interactions are included in the statistical model there are not enough degrees of freedom to reliably estimate all the effects, hence the table of overall tests of model effects (including the covariate interactions) has not been produced.", align="left")	
  } else {
    
    #Creating the analysis including covariates
    if(covariance=="compound symmetric") {
      modelxx <- paste(CovIntModel , " + cs(Time_IVS | Animal_IVS)", sep = "")
      threewayfullx<-mmrm(formula=as.formula(modelxx), data=statdata,  method = "Kenward-Roger", vcov="Kenward-Roger-Linear")
    }
    if(covariance=="autoregressive(1)") {
      modelxx <- paste(CovIntModel , " + ar1(Time_IVS | Animal_IVS)", sep = "")
      threewayfullx<-mmrm(formula=as.formula(modelxx), data=statdata,  method = "Kenward-Roger", vcov="Kenward-Roger-Linear")
    }
    if(covariance=="unstructured") {
      modelxx <- paste(CovIntModel , " + us(Time_IVS | Animal_IVS)", sep = "")
      threewayfullx<-mmrm(formula=as.formula(modelxx), data=statdata,  method = "Kenward-Roger", vcov="Kenward-Roger-Linear")
    }
    
    tempx1<- Anova(threewayfullx, ddf = "Kenward-Roger", type = "III")
    tempx <- row.names(tempx1)
    tempx <- cbind(tempx, tempx1)	  
    
    if (min(tempx[3], na.rm=TRUE) >= 1) {
      col2x<-format(round(tempx[3], 2), nsmall=2, scientific=FALSE)
      col3x<-format(round(tempx[4], 2), nsmall=2, scientific=FALSE)
      col4x<-format(round(tempx[5], 4), nsmall=4, scientific=FALSE)
      
      # Sort out effects list
      for (q in 1:length(tempx[1])) {
        tempx[[1]]<-sub(":"," * ", tempx[[1]]) 
        tempx[[1]]<-sub("Time_IVS",timeFactor, tempx[[1]]) 
      }
      
      ivsanovax<-cbind(tempx[1], tempx[2], col2x, col3x, col4x)
      headx<-c("Effect", "Num. degrees of freedom", "Denom. degrees of freedom", "F-value", "p-value")
      colnames(ivsanovax)<-headx

      # Correction to code to amend lowest p-value
      for (i in 1:(dim(ivsanovax)[1]))  {
        if (tempx[i,5]<0.0001) {
          #STB - Mar 2011 formatting p<0.0001
          ivsanovax[i,5]=format(round(0.0001, 4), nsmall=4, scientific=FALSE)
          ivsanovax[i,5]<- paste("<",ivsanovax[i,5])
        }
      }
      
      #STB July 2013 Change title
      HTML.title("Table of overall tests of model effects, for assessing covariate interactions", HR=2, align="left")
      HTML(ivsanovax, classfirstline="second", align="left", row.names = "FALSE")
      HTML("Note: This table should only be used to assess the covariate interactions. The statistical model used to generate all the remaining results in this output does not include the covariate interactions.", align="left")
    }
    
    if (min(tempx[2], na.rm=TRUE) <= 0) {
      HTML.title("Table of overall tests of model effects, for assessing covariate interactions", HR=2, align="left")
      HTML("The covariate interactions have not been calculated as there are zero residual degrees of freedom when all terms are included in the statistical model.", align="left")
    }
  }
}
#===================================================================================================================
#Set up final model
#===================================================================================================================

#Test to see if there are 0 df, in which case end module

#Creating the list of model terms
model2 <- c(model)

model3 <- paste(model2 , " + ", subjectFactor , sep = "")
threewayfullxxx<-lm(as.formula(model3) , data=statdata, na.action = (na.omit))

if (df.residual(threewayfullxxx) < 1) {	
  HTML.title("Table of overall tests of model effects", HR=2, align="left")
  HTML("Unfortunately there are not enough degrees of freedom to estimate all the effects, hence no analysis has been produced. Please reduce the number of effects in your statistical model.", align="left")	
  quit()
} 

if(covariance=="compound symmetric") {
  modelx <- paste(model2 , " + cs(Time_IVS | Animal_IVS)", sep = "")
  threewayfull<-mmrm(formula=as.formula(modelx), data=statdata,  method = "Kenward-Roger", vcov="Kenward-Roger-Linear")
}

if(covariance=="autoregressive(1)") {
  modelx <- paste(model2 , " + ar1(Time_IVS | Animal_IVS)", sep = "")
  threewayfull<-mmrm(formula=as.formula(modelx), data=statdata,  method = "Kenward-Roger", vcov="Kenward-Roger-Linear")
}

if(covariance=="unstructured") {
  modelx <- paste(model2 , " + us(Time_IVS | Animal_IVS)", sep = "")
  threewayfull<-mmrm(formula=as.formula(modelx), data=statdata,  method = "Kenward-Roger", vcov="Kenward-Roger-Linear")
}

#===================================================================================================================
#Testing covariance model fits
#===================================================================================================================
if(compareCovarianceModels == "Y" ) {

  modelx <- paste(model2 , " + cs(Time_IVS | Animal_IVS)", sep = "")
  threewayfullCS<-mmrm(formula=as.formula(modelx), data=statdata,  method = "Kenward-Roger", vcov="Kenward-Roger-Linear")
  modely <- paste(model2 , " + ar1(Time_IVS | Animal_IVS)", sep = "")
  threewayfullAR<-mmrm(formula=as.formula(modely), data=statdata,  method = "Kenward-Roger", vcov="Kenward-Roger-Linear")
  modelz <- paste(model2 , " + us(Time_IVS | Animal_IVS)", sep = "")
  threewayfullUN<-mmrm(formula=as.formula(modelz), data=statdata,  method = "Kenward-Roger", vcov="Kenward-Roger-Linear")
  
  AICCS<- AIC(threewayfullCS) 
  AICAR<- AIC(threewayfullAR)  
  AICUN<- AIC(threewayfullUN)  
  AIC_Out<-c(AICCS , AICAR, AICUN)
  
  BICCS<- BIC(threewayfullCS) 
  BICAR<- BIC(threewayfullAR)  
  BICUN<- BIC(threewayfullUN)  
  BIC_Out<-c(BICCS , BICAR, BICUN)
  
  temp<-c(
    format(round(logLik(threewayfullCS, REML=TRUE)[1], 3), nsmall=3, scientific=FALSE), 
    format(round(logLik(threewayfullAR, REML=TRUE)[1], 3), nsmall=3, scientific=FALSE), 
    format(round(logLik(threewayfullUN, REML=TRUE)[1], 3), nsmall=3, scientific=FALSE)
  )
 
  Critnames <- c("Compound Symmetric", "Autoregressive (1)", "Unstructured")
  
  #Rounding Manipulation
  col1<-format(round(AIC_Out, 3), nsmall=3, scientific=FALSE)
  col2<-format(round(BIC_Out, 3), nsmall=3, scientific=FALSE)
  
  #Combine results
  ModelComp<-cbind(Critnames, col1, col2, temp)
  colnames(ModelComp) <- c("Covariance Structure",  "Akaike information criterion (AIC)", "Bayesian information criterion (BIC)", "Log-Likelihood")
  
  HTML.title("Comparing models with different covariance structures", HR=2, align="left")
  HTML(ModelComp, classfirstline="second", align="left", row.names = "FALSE")
  HTML("Note: When comparing covariance structures, a lower AIC or BIC value indicates a better fit.", align="left")
}

#===================================================================================================================
#ANOVA Table
#jointz=joint_tests(threewayfull,cov.reduce = symmint(0))
#HTML.title("Table of joint tests overall tests of model effects", HR=2, align="left")
#HTML(jointz, classfirstline="second", align="left", row.names = "FALSE")

#jointzz=car::Anova(threewayfull, ddf = "Kenward-Roger" , type = "III")
#HTML.title("Table of Car ANOVA overall tests of model effects", HR=2, align="left")
#HTML(jointzz, classfirstline="second", align="left", row.names = "FALSE")

#jointzzz=Anova(threewayfull, ddf = "Kenward-Roger" , type = "III")
#HTML.title("Table of Car ANOVA overall tests of model effects", HR=2, align="left")
#HTML(jointzzz, classfirstline="second", align="left", row.names = "FALSE")

#===================================================================================================================

temp1<- Anova(threewayfull, ddf = "Kenward-Roger" , type = "III")
temp <- row.names(temp1)
temp <- cbind(temp, temp1)

col2<-format(round(temp[3], 2), nsmall=2, scientific=FALSE)
col3<-format(round(temp[4], 2), nsmall=2, scientific=FALSE)
col4<-format(round(temp[5], 4), nsmall=4, scientific=FALSE)

# Sort out effects list
for (q in 1:length(temp[1])) {
  temp[[1]]<-sub(":"," * ", temp[[1]]) 
  temp[[1]]<-sub("Time_IVS",timeFactor, temp[[1]]) 
}
tempy <- temp[[1]]
ivsanova<-cbind(temp[1], temp[2], col2, col3, col4)
head<-c("Effect", "Num. degrees of freedom", "Denom. degrees of freedom", "F-value", "p-value")
colnames(ivsanova)<-head

# Correction to code to amend lowest p-value: STB Oct 2010
for (i in 1:(dim(ivsanova)[1]))  {
  if (temp[i,5]<0.0001) {
    #STB - Mar 2011 formatting p<0.0001
    ivsanova[i,5]=format(round(0.0001, 4), nsmall=4, scientific=FALSE)
    ivsanova[i,5]<- paste("<",ivsanova[i,5])
  }
}

if(showANOVA=="Y") {
  HTML.title("Table of overall tests of model effects", HR=2, align="left")
  HTML(ivsanova, classfirstline="second", align="left", row.names = "FALSE")
  HTML("Comment: The overall tests in this table are marginal likelihood ratio tests, where the order they appear in the table does not influence the results.", align="left")
  
  #Number of significant terms 
  nosigs <- 0
  for(i in 1:(dim(ivsanova)[1]))	{
    if (temp[i,5]<= (1-sig)) {
      nosigs <- nosigs+1
    }
  }
  
  add<-"Conclusion"
  index <- 0
  for(i in 1:(dim(ivsanova)[1]))	{
    if (temp[i,5]<= (1-sig)) {
      index <- index+1
      if (index == 1) {
        add<-paste(add, ": At the ", 100*(1-sig), "% level", " there is a statistically significant overall difference between the levels of ", tempy[i], sep="")
      } 
      if (index > 1 && index < nosigs) {
        add<-paste(add, ", ", tempy[i], sep="")
      } else if (index > 1 && index == nosigs) {
        add<-paste(add, " and ", tempy[i], sep="")
      }
    } 
  }
  
  if (nosigs==0) {
    if (dim(ivsanova)[1]>2) {
      add<-paste(add, ": There are no statistically significant overall differences, at the ", 100*(1-sig), "% level, ", "between the levels of any of the terms in the table of overall tests", sep="")
    } else {
      add<-paste(add, ": There is no statistically significant overall difference, at the ", 100*(1-sig), "% level, ", "between the levels of the treatment factor", sep="")
    } 
  }		
  add<-paste(add, ". ", sep="")
  HTML(add, align="left")
  HTML("Tip: While it is a good idea to consider the overall tests in the above table, we should not rely on them when deciding whether or not to make pairwise comparisons.", align="left")
  
  #STB May 2012 correcting table
  # Warning message for degrees of freedom
  if (min(unlist(temp[[3]]))<5) {
    HTML.title("Warning", HR=2, align="left")
    HTML("Unfortunately one or more of the residual degrees of freedom in the above table are low (less than 5). This may make the estimation of the underlying variability, and hence the results of the statistical tests, unreliable. This can be caused by attempting to fit too many factors, and their interactions, in the statistical model. Where appropriate we recommend you fit some of the 'Treatment' factors as 'Other design' factors. This will remove their interactions from the statistical model and therefore increase the residual degrees of freedom.", align="left")
  }
}

#===================================================================================================================
#Covariate correlation table
#===================================================================================================================
if (CovariateRegressionCoefficients == "Y"  && covariatelist != "NULL") {
  if (nocovlist == 1) {
    HTML.title("Covariate regression coefficient", HR=2, align="left")
  } else {
    HTML.title("Covariate regression coefficients", HR=2, align="left")
  }
  
  covtable_1 <-  data.frame(tidy(threewayfull, conf.int = TRUE, conf.level = sig))
  covtable_2 <-  covtable_1[c((noblockfactors+2):(nocovlist+1+noblockfactors)),]

  names <- covtable_2$term
  Estimate <-format(round(covtable_2$estimate, 3), nsmall=3, scientific=FALSE) 
  StdError <-format(round(covtable_2$std.error, 3), nsmall=3, scientific=FALSE) 
  tvalue <-format(round(covtable_2$statistic, 2), nsmall=2, scientific=FALSE) 
  Prt <-format(round(covtable_2$p.value, 4), nsmall=4, scientific=FALSE) 
  
  covtable2 <-cbind(names, Estimate, StdError, tvalue, Prt)
  
  for (k in 1:(dim(covtable2)[1])) {
    if (as.numeric(covtable_2[k,6])<0.0001)  {
      #STB March 2011 formatting p-values p<0.0001
      #ivsanova[i,9]<-0.0001
      covtable2[k,5]= "<0.0001"
    }
  }
  colnames(covtable2)<-c("Covariate", "Estimate", "Std error", "t-value", "p-value")
  HTML(covtable2, classfirstline="second", align="left", row.names = "FALSE")
}

#===================================================================================================================
#Diagnostic plots
#===================================================================================================================
if((showPRPlot=="Y" && showNormPlot=="N") || (showPRPlot=="N" && showNormPlot=="Y") ) {
  HTML.title("Diagnostic plot", HR=2, align="left")
}
if(showPRPlot=="Y" && showNormPlot=="Y") {
  HTML.title("Diagnostic plots", HR=2, align="left")
}

#Residual plots
if(showPRPlot=="Y") {
  HTML.title("Residuals vs. predicted plot", HR=3, align="left")
  
  residualPlot <- sub(".html", "residualplot.png", htmlFile)
  png(residualPlot,width = jpegwidth, height = jpegheight, units="in", res=PlotResolution)
  
  #STB July2013
  plotFilepdf3 <- sub(".html", "residualplot.pdf", htmlFile)
  dev.control("enable") 
  
  #STB Feb 2011 Replaced Studentised with standardised
  residplot<-cbind(predict(threewayfull, level=1),residuals(threewayfull, level=1, type="pearson"))
  rownames(residplot)<-c(1:(dim(residplot)[1]))
  residplot<-data.frame(residplot)
  colnames(residplot)<-c("Predicted","Standardized_residuals")
  
  #Graphical parameters
  graphdata<-residplot
  
  graphdata$yvarrr_IVS <- graphdata$Standardized_residuals
  graphdata$xvarrr_IVS <- graphdata$Predicted
  XAxisTitle <- "Predicted values"
  YAxisTitle <- "Standardised residuals"
  MainTitle2 <- " "
  w_Gr_jitscat <- 0
  h_Gr_jitscat <-  0
  infiniteslope <- "Y"
  
  if (bandw != "N")  {
    Gr_line <-BW_line
    Gr_fill <- BW_fill
  } else {
    Gr_line <-Col_line
    Gr_fill <- Col_fill
  }
  Gr_line_type<-Line_type_dashed
  Line_size <- 0.5
  
  #GGPLOT2 code
  NONCAT_SCAT("RESIDPLOT")
  
  MainTitle2 <- ""
  
  void<-HTMLInsertGraph(GraphFileName=sub("[A-Z0-9a-z,:,\\\\]*App_Data[\\\\]","", residualPlot), Align="centre")
  
  #STB July2013
  if (pdfout=="Y") {
    pdf(file=sub("[A-Z0-9a-z,:,\\\\]*App_Data[\\\\]","", plotFilepdf3), height = pdfheight, width = pdfwidth) 
    dev.set(2) 
    dev.copy(which=3) 
    dev.off(2)
    dev.off(3)
    pdfFile_3<-sub("[A-Z0-9a-z,:,\\\\]*App_Data[\\\\]","",plotFilepdf3)
    linkToPdf3 <- paste ("<a href=\"",pdfFile_3,"\">Click here to view the PDF of the residuals vs. predicted plot</a>", sep = "")
    HTML(linkToPdf3)
  }
  
  HTML("Tip: On this plot look to see if the spread of the points increases as the predicted values increase. If so the response may need transforming.",  align="left")
  HTML("Tip: Any observation with a standardised residual less than -3 or greater than 3 (SD) should be investigated as a possible outlier.",  align="left")
  HTML("Comment: The standardised residuals are obtained by subtracting the fitted values from the responses and dividing by the estimated within-group standard error.", align="left")
}

#Normality plots
if(showNormPlot=="Y") {
  HTML.title("Normal probability plot", HR=3, align="left")
  
  normPlot <- sub(".html", "normplot.png", htmlFile)
  png(normPlot,width = jpegwidth, height = jpegheight, units="in", res=PlotResolution)
  
  #STB July2013
  plotFilepdf4 <- sub(".html", "normplot.pdf", htmlFile)
  dev.control("enable") 
  
  #Graphical parameters
  te<-qqnorm(resid(threewayfull, level=1))
  graphdata<-data.frame(te$x,te$y)
  graphdata$xvarrr_IVS <-graphdata$te.x
  graphdata$yvarrr_IVS <-graphdata$te.y
  graphdata$names <- c(1:length(graphdata$te.x))
  YAxisTitle <-"Sample Quantiles"
  XAxisTitle <-"Theoretical Quantiles"
  MainTitle2 <- " "
  w_Gr_jitscat <- 0
  h_Gr_jitscat <-  0
  infiniteslope <- "N"
  LinearFit <- "Y"
  
  if (bandw != "N") {
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
  
  #GGPLOT2 code
  #NONCAT_SCAT("QQPLOT")
  NONCAT_QQPLOT()
  
  MainTitle2 <- ""
  
  void<-HTMLInsertGraph(GraphFileName=sub("[A-Z0-9a-z,:,\\\\]*App_Data[\\\\]","", normPlot), Align="left")
  
  #STB July2013
  if (pdfout=="Y") {
    pdf(file=sub("[A-Z0-9a-z,:,\\\\]*App_Data[\\\\]","", plotFilepdf4), height = pdfheight, width = pdfwidth) 
    dev.set(2) 
    dev.copy(which=3) 
    dev.off(2)
    dev.off(3)
    pdfFile_4<-sub("[A-Z0-9a-z,:,\\\\]*App_Data[\\\\]","",plotFilepdf4)
    linkToPdf4 <- paste ("<a href=\"",pdfFile_4,"\">Click here to view the PDF of the normal probability plot</a>", sep = "")
    HTML(linkToPdf4)
  }
  HTML("Tip: Check that the points lie along the dotted line. If not then the data may be non-normally distributed.", align="left")
}

#===================================================================================================================
# Plot and table of LS Means
#===================================================================================================================
if(showLSMeans=="Y") {
  
  #Calculate LS Means
  LSDATA<-data.frame(emmeans(threewayfull,~Time_IVS, level=sig))
  LSDATA$Meanp<-format(round(LSDATA$emmean,3),nsmall=3)
  LSDATA$Lowerp<-format(round(LSDATA$lower.CL,3),nsmall=3)
  LSDATA$Upperp<-format(round(LSDATA$upper.CL,3),nsmall=3)
  
  LSDATA$Mean  <- as.numeric(LSDATA$emmean)
  LSDATA$Lower <- as.numeric(LSDATA$lower.CL)
  LSDATA$Upper <- as.numeric(LSDATA$upper.CL)

  #===================================================================================================================
  # Table of LS Means
  #===================================================================================================================
  if ( (responseTransform != "log10" && responseTransform != "loge") || (responseTransform == "log10" && GeomDisplay != "geometricmeansonly") || (responseTransform == "loge" && GeomDisplay != "geometricmeansonly") ) {
    
    CITitle2<-paste("Table of the least square (predicted) means with ",(sig*100),"% confidence intervals",sep="")
    HTML.title(CITitle2, HR=2, align="left")
    
    LSDATA2<-subset(LSDATA, select = -c(df, SE, lower.CL, upper.CL,df, emmean, Mean, Upper, Lower)) 	
  
    names <- c()
    names[1]<- timeFactor
    names[2]<-"Mean"
    names[3]<-paste("Lower ",(sig*100),"% CI",sep="")
    names[4]<-paste("Upper ",(sig*100),"% CI",sep="")
    
    colnames(LSDATA2)<-names
    rownames(LSDATA2)<-c("ID",1:(dim(LSDATA2)[1]-1))
    HTML(LSDATA2, classfirstline="second", align="left", row.names = "FALSE")
    
    #===================================================================================================================
    # Plot of LS Means
    #===================================================================================================================
    
    #Creating the final dataset to plot
    graphdata<- LSDATA
    graphdata$jj_1<- graphdata$Time_IVS
    graphdata$Group_IVSq_<-graphdata[,1]
 
    CITitle<-paste("Plot of the least square (predicted) means with ",(sig*100),"% confidence intervals",sep="")
    HTML.title(CITitle, HR=2, align="left")
    
    meanPlot <- sub(".html", "meanplot.png", htmlFile)
    png(meanPlot,width = jpegwidth, height = jpegheight, units="in", res=PlotResolution)
    
    #STB July2013
    plotFilepdf5 <- sub(".html", "meanplot.pdf", htmlFile)
    dev.control("enable") 
    
    #Parameters
    Line_size <- Line_size2
    Gr_alpha <- 0
    if (bandw != "N") {
      Gr_fill <- BW_fill
    } else {
      Gr_fill <- Col_fill
    }
    
    YAxisTitle <- LS_YAxisTitle
    XAxisTitle <- timeFactor_plot
    MainTitle2 <- ""
    
    #GGPLOT2 code
    LSMPLOT_1()
    
    void<-HTMLInsertGraph(GraphFileName=sub("[A-Z0-9a-z,:,\\\\]*App_Data[\\\\]","", meanPlot), Align="left")
    
    #STB July2013
    if (pdfout=="Y") {
      pdf(file=sub("[A-Z0-9a-z,:,\\\\]*App_Data[\\\\]","", plotFilepdf5), height = pdfheight, width = pdfwidth) 
      dev.set(2) 
      dev.copy(which=3) 
      dev.off(2)
      dev.off(3)
      pdfFile_5<-sub("[A-Z0-9a-z,:,\\\\]*App_Data[\\\\]","",plotFilepdf5)
      linkToPdf5 <- paste ("<a href=\"",pdfFile_5,"\">Click here to view the PDF of the plot of the least square (predicted) means</a>", sep = "")
      HTML(linkToPdf5)
    }
  }
}

#===================================================================================================================
#Back transformed geometric means table and plot 
#===================================================================================================================
if(showLSMeans =="Y" && (responseTransform =="log10"||responseTransform =="loge")) {
  if ( (responseTransform == "log10" && GeomDisplay != "notdisplayed") || (responseTransform == "loge" && GeomDisplay != "notdisplayed") ) {
    
    if (responseTransform =="log10") {
      LSDATA$Mean  <- 10^(as.numeric(LSDATA$emmean))
      LSDATA$Lower <- 10^(as.numeric(LSDATA$lower.CL))
      LSDATA$Upper <- 10^(as.numeric(LSDATA$upper.CL))
    }
    if (responseTransform =="loge") {
      LSDATA$Mean  <- exp(as.numeric(LSDATA$emmean))
      LSDATA$Lower <- exp(as.numeric(LSDATA$lower.CL))
      LSDATA$Upper <- exp(as.numeric(LSDATA$upper.CL))
    }
    
    #===================================================================================================================
    #Table of back transformed plot
    #===================================================================================================================
    CITitle2<-paste("Table of the back-transformed geometric means with ",(sig*100),"% confidence intervals",sep="")
    HTML.title(CITitle2, HR=2, align="left")
    
    if (GeomDisplay == "geometricmeansandpredictedmeansonlogscale") {
      HTML("As the response was log transformed prior to analysis the least square (predicted) means are presented on the log scale. These results can be back transformed onto the original scale. These are known as the back-transformed geometric means.", align="left")
    }
    if (GeomDisplay == "geometricmeansonly") {
      HTML("As the response was log transformed prior to analysis the least square (predicted) means are presented back transformed onto the original scale. These are known as the back-transformed geometric means.", align="left")
    }
    
    if (responseTransform =="log10") {
      LSDATA$Meanp<-format(round(LSDATA$Mean,3),nsmall=3)
      LSDATA$Lowerp<-format(round(LSDATA$Lower,3),nsmall=3)
      LSDATA$Upperp<-format(round(LSDATA$Upper,3),nsmall=3)
    }
    if (responseTransform =="loge") {
      LSDATA$Meanp<-format(round(LSDATA$Mean,3),nsmall=3)
      LSDATA$Lowerp<-format(round(LSDATA$Lower,3),nsmall=3)
      LSDATA$Upperp<-format(round(LSDATA$Upper,3),nsmall=3)
    }
    LSDATA2<-subset(LSDATA, select = -c(df, SE, lower.CL, upper.CL,df, emmean, Mean, Lower, Upper)) 
    
    names[1]<- timeFactor
    names[2]<-"Mean"
    names[3]<-paste("Lower ",(sig*100),"% CI",sep="")
    names[4]<-paste("Upper ",(sig*100),"% CI",sep="")
    
    colnames(LSDATA2)<-names
    rownames(LSDATA2)<-c("ID",1:(dim(LSDATA2)[1]-1))
    HTML(LSDATA2, classfirstline="second", align="left", row.names = "FALSE")
    
    #===================================================================================================================
    #Back transformed geometric means plot 
    #===================================================================================================================
    CITitle<-paste("Plot of the back-transformed geometric means with ",(sig*100),"% confidence intervals",sep="")
    HTML.title(CITitle, HR=2, align="left")
    
    #CreatinG the final datasset to plot
    graphdata<- LSDATA
    graphdata$jj_1<- graphdata$Time_IVS
    graphdata$Group_IVSq_<-graphdata[,1]
  
    meanPlotd <- sub(".html", "meanplotd.png", htmlFile)
    png(meanPlotd,width = jpegwidth, height = jpegheight, units="in", res=PlotResolution)
    
    #STB July2013
    plotFilepdf5d <- sub(".html", "meanplotd.pdf", htmlFile)
    dev.control("enable") 
    
    #Parameters
    Line_size <- Line_size2
    Gr_alpha <- 0
    if (bandw != "N") {
      Gr_fill <- BW_fill
    } else {
      Gr_fill <- Col_fill
    }
    
    YAxisTitle <- BTYAxisTitle
    XAxisTitle <- timeFactor_plot
    MainTitle2 <- ""
    
    #GGPLOT2 code
    LSMPLOT_1()
    
    void<-HTMLInsertGraph(GraphFileName=sub("[A-Z0-9a-z,:,\\\\]*App_Data[\\\\]","", meanPlotd), Align="left")
    
    #STB July2013
    if (pdfout=="Y") {
      pdf(file=sub("[A-Z0-9a-z,:,\\\\]*App_Data[\\\\]","", plotFilepdf5d), height = pdfheight, width = pdfwidth) 
      dev.set(2) 
      dev.copy(which=3) 
      dev.off(2)
      dev.off(3)
      pdfFile_5d<-sub("[A-Z0-9a-z,:,\\\\]*App_Data[\\\\]","",plotFilepdf5d)
      linkToPdf5d <- paste ("<a href=\"",pdfFile_5d,"\">Click here to view the PDF of the plot of the back-transformed geometric means</a>", sep = "")
      HTML(linkToPdf5d)
    }
  }
}

#===================================================================================================================
#Pairwise tests general code
#===================================================================================================================
if (showComps == "Y") {
  
  #Define CI limits for tables
  lowerCI<-paste("Lower ",(sig*100),"% CI",sep="")
  upperCI<-paste("Upper ",(sig*100),"% CI",sep="")
  
  #Creating a dataset without any dashes or spaces in factor levels
  statdata_num<- statdata[,sapply(statdata,is.numeric)]
  statdata_char<- statdata[,!sapply(statdata,is.numeric)]
  statdata_char2 <- as.data.frame(sapply(statdata_char,gsub,pattern="-",replacement="xxxivsdashivsxxx"))
  statdata_char3 <- as.data.frame(sapply(statdata_char2,gsub,pattern=" ",replacement="xxxivsspaceivsxxx"))
  statdata_comp<- data.frame(cbind(statdata_num, statdata_char3)) 
  statdata_comp$Animal_IVS<-as.factor(eval(parse(text = paste("statdata_comp$", subjectFactor))))
  statdata_comp$Time_IVS<-as.factor(eval(parse(text = paste("statdata_comp$", timeFactor))))
  statdata_comp$Time_IVS<-as.factor(eval(parse(text = paste("statdata_comp$", timeFactor))))

  #Re-fit the model using the dataset without spaces
  if(covariance=="compound symmetric") {
    modelx <- paste(model2 , " + cs(Time_IVS | Animal_IVS)", sep = "")
    threewayfull_comp<-mmrm(formula=as.formula(modelx), data=statdata_comp,  method = "Kenward-Roger", vcov="Kenward-Roger-Linear")
  }
  if(covariance=="autoregressive(1)") {
    modelx <- paste(model2 , " + ar1(Time_IVS | Animal_IVS)", sep = "")
    threewayfull_comp<-mmrm(formula=as.formula(modelx), data=statdata_comp,  method = "Kenward-Roger", vcov="Kenward-Roger-Linear")
  }
  if(covariance=="unstructured") {
    modelx <- paste(model2 , " + us(Time_IVS | Animal_IVS)", sep = "")
    threewayfull_comp<-mmrm(formula=as.formula(modelx), data=statdata_comp,  method = "Kenward-Roger", vcov="Kenward-Roger-Linear")
  }  
  
  #Calculate all pairwise comparisons
  comps<-emmeans(threewayfull_comp,~Time_IVS, level=sig)
  comparisons<-data.frame(pairs(comps, adjust="none", infer = c(TRUE, TRUE) ))
  namescomp <- colnames(comparisons)
  comparisons$contrast <- gsub("Time_IVS","",comparisons$contrast, fixed=TRUE) 
  
  #Separating out levels
  out <- strsplit(comparisons$contrast, "-")
  comparisonsx <- trimws(do.call(rbind, out))
  comparisons <- cbind(comparisons, comparisonsx)
  colnames(comparisons) <-c(namescomp, "FirstGPIVS", "SecondGPIVS")
  comparisons$FirstGPIVS <- gsub(" " , " , ",comparisons$FirstGPIVS, fixed=TRUE)
  comparisons$SecondGPIVS <- gsub(" " , " , ",comparisons$SecondGPIVS, fixed=TRUE)
  
  #Isolate First Day
#  out1 <- strsplit(comparisons$FirstGPIVS, " ")
#  comparisonsx <- do.call(rbind, out1)
#  comparisons <- cbind(comparisons, comparisonsx)
#  colnames(comparisons)[dim(comparisons)[2]] <- "Day1"
  
  #Isolate  Second Day
#  out2 <- strsplit(comparisons$SecondGPIVS, " ")
#  comparisonsx <- do.call(rbind, out2)
#  comparisons <- cbind(comparisons, comparisonsx)
#  colnames(comparisons)[dim(comparisons)[2]] <- "Day2"


  #Swapping around control group levels
  for (i in 1: dim(comparisons)[1])  {
      if (comparisons$FirstGPIVS[i] == controlGroup) {
      comparisons$FirstGPIVSx[i] <- comparisons$SecondGPIVS[i]
      comparisons$SecondGPIVSx[i] <- comparisons$FirstGPIVS[i]
      comparisons$estimatex[i] <- -1*comparisons$estimate[i]
      comparisons$lower.CLx[i] <- -1*comparisons$upper.CL[i]
      comparisons$upper.CLx[i] <- -1*comparisons$lower.CL[i]
    }
    if (comparisons$FirstGPIVS[i] != controlGroup) {
      comparisons$FirstGPIVSx[i] <- comparisons$FirstGPIVS[i]
      comparisons$SecondGPIVSx[i] <- comparisons$SecondGPIVS[i]
      comparisons$estimatex[i] <- comparisons$estimate[i]
      comparisons$lower.CLx[i] <- comparisons$lower.CL[i]
      comparisons$upper.CLx[i] <- comparisons$upper.CL[i]
    }
  }

  #Reordering dataset with control at top
tempcomps1  <- subset(comparisons, SecondGPIVSx == controlGroup)
tempcomps2  <- subset(comparisons, SecondGPIVSx != controlGroup)
comparisons <- rbind(tempcomps1, tempcomps2)
  
  #Define Group variables for tables, logs and conclusion
  comparisons$comparison <- paste("(", comparisons$FirstGPIVSx, ") - (", comparisons$SecondGPIVSx, ")") 
  comparisons$comparison <- gsub("xxxivsspaceivsxxx"," ",comparisons$comparison, fixed=TRUE) 
  comparisons$comparison <- gsub("xxxivsdashivsxxx"," - ",comparisons$comparison, fixed=TRUE) 

  comparisons$comparisonL <- paste("(", comparisons$FirstGPIVSx, ") / (", comparisons$SecondGPIVSx, ")") 
  comparisons$comparisonL <- gsub("xxxivsspaceivsxxx"," ",comparisons$comparisonL, fixed=TRUE) 
  comparisons$comparisonL <- gsub("xxxivsdashivsxxx"," - ",comparisons$comparisonL, fixed=TRUE) 

  comparisons$comparisonC <- paste("(", comparisons$FirstGPIVSx, ") vs. (", comparisons$SecondGPIVSx, ")") 
  comparisons$comparisonC <- gsub("xxxivsspaceivsxxx"," ",comparisons$comparisonC, fixed=TRUE) 
  comparisons$comparisonC <- gsub("xxxivsdashivsxxx"," - ",comparisons$comparisonC, fixed=TRUE) 

  #Adjusting p-values
  comparisons$pval<- format(round(comparisons$p.value, 4), nsmall=4, scientific=FALSE)
  for (i in 1:dim(comparisons)[1])  {
    if (comparisons$p.value[i] <0.0001) {
      comparisons$pval[i]="<0.0001"
    }
  }
  
  #Back transforming logged results
  if (responseTransform =="log10") {
    comparisons$Lestimatex=format(round(10^comparisons$estimatex, 3), nsmall=3, scientific=FALSE) 
    comparisons$Llower.CLx=format(round(10^comparisons$lower.CLx, 3), nsmall=3, scientific=FALSE) 
    comparisons$Lupper.CLx=format(round(10^comparisons$upper.CLx, 3), nsmall=3, scientific=FALSE) 
  }
  
  if (responseTransform =="loge") {
    comparisons$Lestimatex=format(round(exp(comparisons$estimatex), 3), nsmall=3, scientific=FALSE) 
    comparisons$Llower.CLx=format(round(exp(comparisons$lower.CLx), 3), nsmall=3, scientific=FALSE) 
    comparisons$Lupper.CLx=format(round(exp(comparisons$upper.CLx), 3), nsmall=3, scientific=FALSE) 
  } 
  
  comparisons$estimatex=format(round(comparisons$estimatex, 3), nsmall=3, scientific=FALSE) 
  comparisons$lower.CLx=format(round(comparisons$lower.CLx, 3), nsmall=3, scientific=FALSE) 
  comparisons$upper.CLx=format(round(comparisons$upper.CLx, 3), nsmall=3, scientific=FALSE) 
  


  #Title
  if ( (responseTransform != "log10" && responseTransform != "loge") || (responseTransform == "log10" && GeomDisplay != "geometricmeansonly") || (responseTransform == "loge" && GeomDisplay != "geometricmeansonly") ) {
    HTML.title("All pairwise comparisons, without adjustment for multiplicity", HR=2, align="left")
  }
  
  #Create all pairwise tests dataset
  AllP <- subset(comparisons, select = c(comparison,estimatex, lower.CLx, upper.CLx , SE, pval))
  colnames(AllP)<-c("Comparison", "Difference", lowerCI, upperCI, "Std error", "p-value")
  
  #print table
  if ( (responseTransform != "log10" && responseTransform != "loge") || (responseTransform == "log10" && GeomDisplay != "geometricmeansonly") || (responseTransform == "loge" && GeomDisplay != "geometricmeansonly") ) {
    HTML(AllP, classfirstline="second", align="left", row.names = "FALSE")
  }

  #===================================================================================================================
  #Back transformed geometric means table 
  #===================================================================================================================
  if(responseTransform =="log10"||responseTransform =="loge") {
    if ( GeomDisplay != "notdisplayed") {
      HTML.title("All pairwise comparisons, as back-transformed ratios", HR=2, align="left")
      
      if (GeomDisplay == "geometricmeansandpredictedmeansonlogscale") {
        HTML("As the response was log transformed prior to analysis the differences between the least square (predicted) means are presented on the log scale. These results can be back-transformed, where differences on the log scale become ratios when back-transformed.", align="left")
      }
      if (GeomDisplay == "geometricmeansonly") {
        HTML("As the response was log transformed prior to analysis the differences between the least square (predicted) means are back-transformed, where differences on the log scale become ratios when back-transformed.", align="left")
      }
    }
    
    #Create all pairwise tests dataset
    AllPL <- subset(comparisons, select = c(comparisonL,Lestimatex, Llower.CLx, Lupper.CLx , pval))
    colnames(AllPL)<-c("Comparison","Ratio", lowerCI, upperCI, "p-value")
    
    if ( GeomDisplay != "notdisplayed") {
      #print table
      HTML(AllPL, classfirstline="second", align="left", row.names = "FALSE")
    }
    
  }

  #Conclusion
  add<-paste(c("Conclusion"))
  inte<-1

  for(i in 1:(dim(comparisons)[1])) {
    if (comparisons$p.value[i]  <= (1-sig)) {
      if (inte==1) {
        inte<-inte+1
        if (dimfact ==2) {
          add<-paste(add, ": The pairwise comparison is statistically significant at the ", sep="")
        } else {
          add<-paste(add, ": The following pairwise comparisons are statistically significant at the ", sep="")
        }
        add<-paste(add, 100*(1-sig), sep="")
        add<-paste(add, "% level", sep="")
        if (dimfact > 2) {
          add<-paste(add, " : [1] ", comparisons$comparisonC[i], sep="")
        }
      } else {
        add<-paste(add, ", [", inte, "] ", sep="")
        add<-paste(add, comparisons$comparisonC[i], sep="")
        inte<-inte+1
      }
    } 
  }
  
  if (inte==1) {
    if (dim(comparisons)[1] >1) {
      add<-paste(add, ": There are no statistically significant pairwise comparisons.", sep="")
    } else {
      add<-paste(add, ": The pairwise comparison is not statistically significant.", sep="")
    }
  } else {
    add<-paste(add, ". ", sep="")
  }
  HTML(add, align="left")
  if (dimfact > 2) {
    HTML("Warning: As these tests are not adjusted for multiplicity there is a risk of false positive results. Only use the pairwise comparison you planned to make a-priori, these are the so called Planned Comparisons, see Snedecor and Cochran (1989). No options are available in this module to make multiple comparison adjustments. If you wish to apply a multiple comparison adjustment to these results then use the P-value Adjustment module.", align="left")
  }
  
}
#===================================================================================================================
#Analysis description
HTML.title("Analysis description", HR=2, align="left")

add<-c("The data were analysed using ")

if (dimfact ==2 ) { #&& covariatelist == "NULL") {
  add<-paste(add, "an extended paired t-test utilizing the mmrm R package, with treatment factor ", timeFactor, sep="")
} else {
  add<-paste(add, "a repeated measures mixed model approach utilizing the mmrm R package, with treatment factor ", timeFactor, sep="")
}

if (blocklist != "NULL" && covariatelist != "NULL")  {
  add<-paste(add, ", ", sep="")
} else if (blocklist != "NULL" && covariatelist == "NULL")  {
  add<-paste(add, " and ", sep="")
} 

if (blocklist != "NULL") {
  if (noblockfactors==1)  {
    add<-paste(add, blocklist, " as the blocking factor", sep="")
  } 
  
  if(noblockfactors>1) {
    for (i in 1:noblockfactors) {
      if (i<(noblockfactors-1)) {
        add<-paste(add, blocklistsep[i], ", ", sep="")
      } else	if (i==(noblockfactors-1)) {
        add<-paste(add, blocklistsep[i], " and ", sep="")
      } else if (i==noblockfactors) {
        add<-paste(add, blocklistsep[i], sep="")
      }
    }
    add<-paste(add, " as blocking factors", sep="")
  } 
}

if (covariatelist == "NULL") {
  add<-paste(add, ". ", sep="")
} else {
  if (nocovlist==1) {
    add<-paste(add, " and ", covlistsep[1], " as the covariate.", sep="")
  } else {
    add<-paste(add, " and ", sep="")
    for (i in 1:nocovlist) {
      if (i<(nocovlist-1))	{
        add<-paste(add, covlistsep[i], ", ", sep="")
      } else 	if (i==(nocovlist-1)) {
        add<-paste(add, covlistsep[i], " and ", sep="")
      } else if (i==nocovlist) {
        add<-paste(add, covlistsep[i], " as covariates.", sep="")
      }
    }
  }
}

if (dimfact > 2) {
  add<-paste(add, "This was followed by planned comparisons on the predicted means to compare the levels of the ", timeFactor, ". ",  sep="")
}

if (responseTransform != "none") {
  add<-paste(add, " The response was ", responseTransform, " transformed prior to analysis to stabilise the variance.", sep="")
}

if (covariateTransform != "none" && responseTransform != "none") {
  add<-paste(add, " The covariate was also ", covariateTransform, " transformed. ", sep="")
}

if (responseTransform == "none" && covariateTransform != "none") {
  add<-paste(add, " The covariate was ", covariateTransform , " transformed prior to analysis.",sep="")
}

HTML(add, align="left")

if (dimfact ==2) {
  HTML("The paired t-test performed by InVivoStat is defined as an extended paired t-test because the data is analysed using a repeated measures mixed model approach. This allows for any additional blocking factors and/or covariates to be taken into account as well as the pairing within the design.", align="left")
}

if(dimfact > 2) {
  if(covariance=="compound symmetric")
  {
    add2<-paste("The compound symmetric covariance structure was used to model the within-subject correlations. When using this structure we assumed that the variability of the responses was the same at each level of ", timeFactor, " and the correlation between responses from any pair of levels of ", timeFactor, "  is the same." , sep="")
    HTML(add2, align="left")
  }
  if(covariance=="autoregressive(1)") {
    add2<-paste("The first order autoregressive covariance structure was used to model the within-subject correlations. When using this structure we assumed that the variability of the responses was the same at each level of ", timeFactor, ". We also assumed that the correlation between responses from any pair of levels of ", timeFactor, " was related to the distance between them.",  sep="")
    HTML(add2, align="left")
  }
  if(covariance=="unstructured") {
    add2<-paste("The unstructured covariance structure allowed the variability of the responses to be different, depending on the level of ", timeFactor, ". This structure also allowed the correlation between responses from any pair of levels of ", timeFactor,  " to be different. While this approach is the most general it should be used with care when there are few subjects, as many parameters need to be estimated. These estimates may not be very reliable.", sep="")
    HTML(add2, align="left")
  }
} 

add<-paste("A full description of mixed model theory can be found in Venables and Ripley (2003) and Pinherio and Bates (2002).", sep="")
HTML(add, align="left")

#===================================================================================================================
#Create file of comparisons
#===================================================================================================================
#write.csv(tabsx, file = sub("[A-Z0-9a-z,:,\\\\]*App_Data[\\\\]","", comparisons), row.names=FALSE)

#===================================================================================================================
#References
#===================================================================================================================
Ref_list<-R_refs()

#Bate and Clark comment
HTML(refxx, align="left")	

HTML.title("References", HR=2, align="left")
HTML(Ref_list$IVS_ref, align="left")
HTML(Ref_list$BateClark_ref, align="left")

if(covariatelist != "NULL") {
  HTML("Morris, T.R. (1999). Experimental Design and Analysis in Animal Sciences. CABI publishing. Wallingford, Oxon (UK).", align="left")
}

HTML("Pinherio, J.C. and Bates, D.M. (2000). Mixed Effects Models in S and S-Plus. Springer-Verlag. New York, Inc.", align="left")

if (dimfact > 2) {
  HTML("Snedecor, G.W. and Cochran, W.G. (1989). Statistical Methods. 8th edition;  Iowa State University Press, Iowa, USA.", align="left")
}

HTML("Venables, W.N. and Ripley, B.D. (2003). Modern Applied Statistics with S. 4th Edition; Springer. New York, Inc.", align="left")

HTML.title("R references", HR=4, align="left")
HTML(Ref_list$R_ref , align="left")
HTML(reference("car"))
HTML(reference("emmeans"))
HTML(reference("GGally"))
HTML(reference("ggplot2"))
HTML(reference("ggrepel"))
HTML(reference("mmrm"))
HTML(reference("R2HTML"))
HTML(reference("RColorBrewer"))
HTML(reference("reshape"))
HTML(reference("plyr"))
HTML(reference("proto"))
HTML(reference("scales"))

#===================================================================================================================
#Show dataset
#===================================================================================================================
if (showdataset=="Y") {
  
  observ <- data.frame(c(1:dim(statdataprint)[1]))
  colnames(observ) <- c("Observation")
  statdataprint2 <- cbind(observ, statdataprint)
  
  HTML.title("Analysis dataset", HR = 2, align = "left")
  HTML(statdataprint2, classfirstline = "second", align = "left", row.names = "FALSE")
  
  
}

#===================================================================================================================
#Show arguments
#===================================================================================================================
if (OutputAnalysisOps == "Y") {
  HTML.title("Analysis options", HR=2, align="left")
  
  HTML(paste("Response variable: ", resp, sep=""), align="left")
  
  if (responseTransform != "none") {
    HTML(paste("Response variable transformation: ", responseTransform, sep=""), align="left")
  }
  
  HTML(paste("Treatment factor: ", timeFactor, sep=""), align="left")
  
  HTML(paste("Subject factor: ", subjectFactor, sep=""), align="left")
  
  if (noblockfactors > 0) {
    HTML(paste("Other design (block) factor(s): ", blocklist, sep=""), align="left")
  }
  
  if (covariatelist !="NULL") {
    HTML(paste("Covariate(s): ", covariatelist, sep=""), align="left")
  }
  
  if (covariateTransform != "none" && covariatelist != "NULL") {
    HTML(paste("Covariate(s) transformation: ", covariateTransform, sep=""), align="left")
  }
  
  HTML(paste("Covariance structure: ", covariance, sep=""), align="left")
  HTML(paste("Compare covariance models: ", compareCovarianceModels, sep=""),  align="left")
  
  HTML(paste("Output table of overall effect tests (Y/N): ", showANOVA, sep=""), align="left")
  HTML(paste("Output residuals vs. predicted plot (Y/N): ", showPRPlot, sep=""), align="left")
  HTML(paste("Output normal probability plot (Y/N): ", showNormPlot, sep=""), align="left")
  HTML(paste("Output least square (predicted) means (Y/N): ", showLSMeans, sep=""), align="left")
  HTML(paste("Output all pairwise comparisons (Y/N): ", showComps, sep=""), align="left")
  HTML(paste("Control group: ", controlGroup, sep=""), align="left")
  HTML(paste("Significance level: ", 1-sig, sep=""), align="left")
}