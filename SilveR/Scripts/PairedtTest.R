#===================================================================================================================
#R Libraries

suppressWarnings(library(R2HTML))
suppressWarnings(library(multcomp))
suppressWarnings(library(nlme))
suppressWarnings(library(contrast))
suppressWarnings(library(lsmeans))

#STB 14OCT2015
#Set contrast options for Marginal overall tests
options(contrasts = c(unordered = "contr.sum", ordered = "contr.poly"))

#===================================================================================================================

#===================================================================================================================
# retrieve args
Args <- commandArgs(TRUE)

#Read in data
statdata <- read.csv(Args[3], header = TRUE, sep = ",")

#Copy Args
model <- as.formula(Args[4])
timeFactor <- Args[5]
subjectFactor <- Args[6]
covariateModel <- Args[7]
covariance <- Args[8]
responseTransform <- Args[9]
covariateTransform <- Args[10]
blockFactors <- Args[11]
showANOVA <- Args[12]
showPRPlot <- Args[13]
showNormPlot <- Args[14]
sig <- 1 - as.numeric(Args[15])
showLSMeans <- Args[16]

#working directory
direct2 <- unlist(strsplit(Args[3], "/"))
direct <- direct2[1]
for (i in 2:(length(direct2) - 1)) {
    direct <- paste(direct, "/", direct2[i], sep = "")
}


#===================================================================================================
#Graphics parameter setup

graphdata <- statdata
Gr_palette <- palette_FUN(timeFactor)
Labelz_IVS_ <- "N"

#===================================================================================================
#===================================================================================================
dimfact <- length(unique(eval(parse(text = paste("statdata$", timeFactor)))))

covar <- "NULL"
covar2 <- "NULL"
if (covariateModel != "NULL") {
    covar <- unlist(strsplit(covariateModel, "~"))[1]
    covar2 <- unlist(strsplit(covariateModel, "~"))[2]
}

#Setup the html file and associated css file
htmlFile <- sub(".csv", ".html", Args[3]);
#determine the file name of the html file
HTMLSetFile(file = htmlFile)
.HTML.file = htmlFile

#Output HTML header
#STB Aug 2014 updating paired t-test to exclude covariate
if (dimfact == 2 && covariateModel == "NULL") {
    #STB May 2012 Updating "paired"
    HTML.title("<bf>Paired t-test/within-subject Analysis (paired t-test analysis)", HR = 1, align = "left")
} else {
    HTML.title("<bf>Paired t-test/within-subject Analysis", HR = 1, align = "left")
}
# Setting up the parameters
resp <- unlist(strsplit(Args[4], "~"))[1] #get the response variable from the main model
statdata$subjectzzzzzz <- as.factor(eval(parse(text = paste("statdata$", subjectFactor))))
statdata$Timezzz <- as.factor(eval(parse(text = paste("statdata$", timeFactor))))
statdata <- statdata[order(statdata$subjectzzzzzz, statdata$Timezzz),]

#Removing illegal charaters
YAxisTitle <- resp
CPXAxisTitle <- timeFactor

if (covariateModel != "NULL") {
    XAxisTitle <- covar2
}

for (i in 1:10) {
    YAxisTitle <- namereplace(YAxisTitle)
    CPXAxisTitle <- namereplace(CPXAxisTitle)

    if (covariateModel != "NULL") {
        XAxisTitle <- namereplace(XAxisTitle)
    }
}
LS_YAxisTitle <- YAxisTitle

#calculating number of block and treatment factors
tempblockChanges <- strsplit(blockFactors, ",")
txtexpectedblockChanges <- c("")
for (i in 1:length(tempblockChanges[[1]])) {
    txtexpectedblockChanges[length(txtexpectedblockChanges) + 1] = (tempblockChanges[[1]][i])
}
noblockfactors <- length(txtexpectedblockChanges) - 1

#need to add this code to the single measure module code?
if (tempblockChanges == "NULL") {
    noblockfactors = 0
}

#Response
title <- c("Response")
if (covariateModel != "NULL") {
    title <- paste(title, ", covariate", sep = "")
}

if (dimfact > 2) {
    title <- paste(title, " and covariance structure", sep = "")
}


HTML.title(title, HR = 2, align = "left")
add <- paste(c("The  "), resp, sep = "")
add <- paste(add, " response is currently being analysed by the Paired t-test/within-subject Analysis module", sep = "")
if (covariateModel != "NULL") {
    add <- paste(add, c(", with  "), sep = "")
    add <- paste(add, unlist(strsplit(covariateModel, "~"))[2], sep = "")
    add <- paste(add, " fitted as a covariate.", sep = "")
} else {
    add <- paste(add, ".", sep = "")
}

HTML.title("</bf> ", HR = 2, align = "left")
HTML.title(add, HR = 0, align = "left")

if (responseTransform != "None" | covariateTransform != "None") {
    HTML.title("<bf> ", HR = 2, align = "left")
}
if (responseTransform != "None") {
    add2 <- paste(c("The response has been "), responseTransform, sep = "")
    add2 <- paste(add2, " transformed prior to analysis.", sep = "")
    HTML.title(add2, HR = 0, align = "left")
}
if (covariateTransform != "None") {
    add3 <- paste(c("The covariate has been "), covariateTransform, sep = "")
    add3 <- paste(add3, " transformed prior to analysis.", sep = "")
    HTML.title(add3, HR = 0, align = "left")
}


if (covariance == "Compound Symmetric" && dimfact > 2) {
    HTML.title("</bf> ", HR = 2, align = "left")
    add4 <- c("The repeated measures mixed model analysis is using the compound symmetric covariance structure to model the within-subject correlations. When using this structure you are assuming sphericity and also that the variability of responses is the same at each level of ")
    add4 <- paste(add4, timeFactor, sep = "")
    add4 <- paste(add4, ", see Pinherio and Bates (2002). These assumptions may not hold in practice.", sep = "")
    HTML.title(add4, HR = 0, align = "left")
}
if (covariance == "Autoregressive(1)" && dimfact > 2) {
    HTML.title("</bf> ", HR = 2, align = "left")
    add4 <- c("The repeated measures mixed model analysis is using the first order autoregressive covariance structure to model the within-subject correlations. When using this structure you are assuming the levels of ")
    add4 <- paste(add4, timeFactor, sep = "")
    add4 <- paste(add4, " are equally spaced and also that the variability of responses are the same at each level of ", sep = "")
    add4 <- paste(add4, timeFactor, sep = "")
    add4 <- paste(add4, ", see Pinherio and Bates (2002). These assumptions may not hold in practice.", sep = "")
    HTML.title(add4, HR = 0, align = "left")
    HTML.title("</bf> ", HR = 2, align = "left")
    #STB May 2012 Updating "least square (predicted) means"
    add4 <- c("Warning: Make sure that the levels of the repeated factor occur in the correct order in the least square (predicted) means table. If they do not then this analysis may not be valid. The autoregressive covariance structure assumes that the order of the repeated factor levels is as defined in the least square (predicted) means table.")
    HTML.title(add4, HR = 0, align = "left")
}
if (covariance == "Unstructured" && dimfact > 2) {
    HTML.title("</bf> ", HR = 2, align = "left")
    HTML.title("<bf>The repeated measures mixed model analysis is using the unstructured covariance structure to model the within-subject correlations. 
When using this structure you are estimating many parameters. If the numbers of subjects used is small then these estimates may be unreliable, see Pinherio and Bates (2002).", HR = 0, align = "left")
}



#Case profiles plot
HTMLbr()
#STB May 2012 removing "Categorised"
title <- c("Case profiles plot of the raw data")
if (responseTransform != "None") {
    title <- paste(title, " (on the ", sep = "")
    title <- paste(title, responseTransform, sep = "")
    title <- paste(title, " scale)", sep = "")
}

HTML.title(title, HR = 2, align = "left")
scatterPlot <- sub(".html", "scatterPlot.jpg", htmlFile)
jpeg(scatterPlot, width = jpegwidth, height = jpegheight, quality = 100)

#STB July2013
plotFilepdf1 <- sub(".html", "scatterPlot.pdf", htmlFile)
dev.control("enable")

#================================================================================================================================
# One-categorised case profiles plot
#================================================================================================================================
#Parameter setup
graphdata$Time_IVS <- as.factor(eval(parse(text = paste("graphdata$", timeFactor))))
graphdata$yvarrr_IVS <- eval(parse(text = paste("graphdata$", resp)))
graphdata$Animal_IVS <- as.factor(eval(parse(text = paste("graphdata$", subjectFactor))))
graphdata$l_l <- graphdata$between
XAxisTitle <- CPXAxisTitle
MainTitle2 <- ""
ReferenceLine = "NULL"

#Colour range for individual animals on case profiles plot
temp <- IVS_F_cpplot_colour("Animal_IVS")
Gr_palette_A <- temp$Gr_palette_A
Gr_line <- temp$Gr_line
Gr_fill <- temp$Gr_fill

#GGPLOT2 code
NONCAT_CPP()
#================================================================================================================================
#================================================================================================================================
void <- HTMLInsertGraph(GraphFileName = sub("[A-Z0-9a-z,:,\\\\]*App_Data[\\\\]", "", scatterPlot), Align = "centre")

#STB July2013
if (pdfout == "Y") {
    pdf(file = sub("[A-Z0-9a-z,:,\\\\]*App_Data[\\\\]", "", plotFilepdf1), height = pdfheight, width = pdfwidth)
    dev.set(2)
    dev.copy(which = 3)
    dev.off(2)
    dev.off(3)
    pdfFile_1 <- sub("[A-Z0-9a-z,:,\\\\]*App_Data[\\\\]", "", plotFilepdf1)
    linkToPdf1 <- paste("<a href=\"", pdfFile_1, "\">Click here to view the PDF of the case profiles plot</a>", sep = "")
    HTML(linkToPdf1)
}

HTML.title("</bf> ", HR = 2, align = "left")
HTML.title("</bf> Tip: Use this plot to identify possible outlier (subjects and individual observations).", HR = 0, align = "left")

#Covariate plot
if (covar != "NULL") {
    title <- c("Covariate plot of the raw data")
    if (responseTransform != "None" || covariateTransform != "None") {
        title <- paste(title, " (on the transformed scale)", sep = "")
    }
    HTMLbr()
    HTML.title(title, HR = 2, align = "left")

    ncscatterplot3 <- sub(".html", "ncscatterplot3.jpg", htmlFile)
    jpeg(ncscatterplot3, width = jpegwidth, height = jpegheight, quality = 100)

    #STB July2013
    plotFilepdf2 <- sub(".html", "ncscatterplot3.pdf", htmlFile)
    dev.control("enable")

    #================================================================================================================================================
    #================================================================================================================================================
    #Graphical parameters
    graphdata <- statdata
    graphdata$xvarrr_IVS <- eval(parse(text = paste("graphdata$", unlist(strsplit(covariateModel, "~"))[2])))
    graphdata$yvarrr_IVS <- eval(parse(text = paste("graphdata$", resp)))
    graphdata$Time_IVS <- as.factor(eval(parse(text = paste("graphdata$", timeFactor))))
    graphdata$tempvariable_ivs <- graphdata$Time_IVS
    graphdata$l_l <- graphdata$tempvariable_ivs
    graphdata$catfact <- graphdata$tempvariable_ivs

    XAxisTitle <- unlist(strsplit(covariateModel, "~"))[2]
    XAxisTitle <- namereplace(XAxisTitle)
    MainTitle2 <- ""

    w_Gr_jit <- 0
    h_Gr_jit <- 0

    Legendpos <- "right"

    Gr_alpha <- 1
    Line_type <- Line_type_solid

    LinearFit <- "Y"
    GraphStyle <- "Overlaid"
    ScatterPlot <- "Y"

    FirstCatFactor <- "Temp"

    #Testing for with infinite slopes on scatterplot and re-ordering dataset if necessary
    inf_slope <- IVS_F_infinite_slope()
    infiniteslope <- inf_slope$infiniteslope
    graphdata <- inf_slope$graphdata

    #GGPLOT2 code
    OVERLAID_SCAT()
    #================================================================================================================================================
    void <- HTMLInsertGraph(GraphFileName = sub("[A-Z0-9a-z,:,\\\\]*App_Data[\\\\]", "", ncscatterplot3), Align = "centre")

    #STB July2013
    if (pdfout == "Y") {
        pdf(file = sub("[A-Z0-9a-z,:,\\\\]*App_Data[\\\\]", "", plotFilepdf2), height = pdfheight, width = pdfwidth)
        dev.set(2)
        dev.copy(which = 3)
        dev.off(2)
        dev.off(3)
        pdfFile_2 <- sub("[A-Z0-9a-z,:,\\\\]*App_Data[\\\\]", "", plotFilepdf2)
        linkToPdf2 <- paste("<a href=\"", pdfFile_2, "\">Click here to view the PDF of the covariate plot</a>", sep = "")
        HTML(linkToPdf2)
    }

    HTML.title("</bf> ", HR = 2, align = "left")
    HTML.title("<bf> Tip: Is it worth fitting the covariate? You should consider the following:", HR = 0, align = "left")
    HTML.title("</bf> ", HR = 2, align = "left")
    HTML.title("<bf> a) Is there a relationship between the response and the covariate?... It is only worth fitting the covariate if there is a strong positive (or negative) relationship between them. The lines on the plot should not be horizontal.", HR = 0, align = "left")
    HTML.title("</bf> ", HR = 2, align = "left")
    HTML.title("<bf> b) Is the relationship similar for all treatments?... The lines on the plot should be approximately parallel. ", HR = 0, align = "left")
    HTML.title("</bf> ", HR = 2, align = "left")
    HTML.title("<bf> c) Is the covariate influenced by the treatment?... We assume the covariate is not influenced by the treatment so there should be no separation of the treatment groups along the x-axis on the plot. ", HR = 0, align = "left")
    HTML.title("</bf> ", HR = 2, align = "left")
    HTML.title("<bf> These issues are discussed in more detail in Morris (1999).", HR = 0, align = "left")
}

#============================================================================================================================
#building the covariate interaction model
#============================================================================================================================
if (AssessCovariateInteractions == "Y" && covariateModel != "NULL") {

    #STB July 2013 Change title
    HTMLbr()
    HTML.title("<bf>Table of overall tests of model effects, for assessing covariate interactions", HR = 2, align = "left")

    # Defining the Response
    Resplist <- unlist(strsplit(covariateModel, "~"))[1]

    # Defining the covariate
    Covlist <- unlist(strsplit(covariateModel, "~"))[2]

    #Creating the list of model terms
    listmodel <- unlist(strsplit(Args[4], "~"))[2] #get the main model
    temChanges <- strsplit(listmodel, "+", fixed = TRUE)
    Modellist <- c("")
    for (i in 1:length(temChanges[[1]])) {
        Modellist[i] = (temChanges[[1]][i])
    }

    #Creating list of blocking factor
    if (noblockfactors > 0) {
        Blocklist <- c()
        for (i in 2:length(txtexpectedblockChanges)) {
            Blocklist[i - 1] = txtexpectedblockChanges[i]
        }
    }
    #Creating the list of treatment terms
    Treatlist <- c()
    for (i in (1 + noblockfactors + 1):length(Modellist)) {
        Treatlist[i - (1 + noblockfactors + 1) + 1] = Modellist[i]
    }

    #Creating the list of interaction terms
    Intlist <- c()
    for (i in 1:(length(Treatlist))) {
        Intlist[i] = paste(Covlist, "*", Treatlist[i], sep = "")
    }

    #Creating the covariate interaction model
    Fulllist <- c(Treatlist, Intlist)

    CovIntModela <- c(Covlist)

    if (noblockfactors > 0) {
        for (i in 1:noblockfactors) {
            CovIntModela[i + 1] <- paste(CovIntModela[i], " + ", Blocklist[i])
        }
    }
    CovIntModelb <- CovIntModela
    for (i in 1:length(Fulllist)) {
        CovIntModelb[i + length(CovIntModela)] <- paste(CovIntModelb[i + length(CovIntModela) - 1], " + ", Fulllist[i])
    }
    CovIntModel <- CovIntModelb[length(CovIntModelb)]

    #Creating the formula
    CovIntForm <- paste(Resplist, " ~ ", CovIntModel, sep = "")



    #Test to see if there are 0 df, in which case end module
    modelxx <- paste(CovIntForm, " + ", subjectFactor, sep = "")
    threewayfullxx <- lm(as.formula(modelxx), data = statdata, na.action = (na.omit))
    print(df.residual(threewayfullxx))

    if (df.residual(threewayfullxx) < 1) {
        HTMLbr()
        HTML.title("<bf>Table of overall tests of model effects, for assessing covariate interactions", HR = 2, align = "left")
        add <- c("When the covariate interactions are included in the statistical model there are not enough degrees of freedom to estimate all the effects, hence no table of overall tests of model effects (including the covariate interactions) has been produced.")
        HTML.title("</bf> ", HR = 2, align = "left")
        HTML.title(add, HR = 0, align = "left")
    } else {

        #Creating the analysis including covariates
        if (covariance == "Compound Symmetric") {
            threewayfullx <- lme(as.formula(CovIntForm), random = ~1 | subjectzzzzzz, data = statdata, correlation = corCompSymm(), na.action = (na.omit), method = "REML")
        }
        if (covariance == "Autoregressive(1)") {
            threewayfullx <- lme(as.formula(CovIntForm), random = ~1 | subjectzzzzzz, correlation = corAR1(value = 0.999, form = ~as.numeric(Timezzz) | subjectzzzzzz, fixed = FALSE), data = statdata, na.action = (na.omit), method = "REML")
        }
        if (covariance == "Unstructured") {
            threewayfullx <- lme(as.formula(CovIntForm), random = ~1 | subjectzzzzzz, correlation = corSymm(form = ~as.numeric(Timezzz) | subjectzzzzzz), weights = varIdent(form = ~1 | as.numeric(Timezzz)), data = statdata, na.action = (na.omit), method = "REML")
        }

        #STB Aug 2014 add in marginal sums of squares
        #tempx<-anova(threewayfullx, type="sequential")
        tempx <- anova(threewayfullx, type = "marginal")

        if (min(tempx[2], na.rm = TRUE) != 0) {
            temp2x <- (tempx)
            col3x <- format(round(temp2x[3], 2), nsmall = 2, scientific = FALSE)
            col4x <- format(round(temp2x[4], 4), nsmall = 4, scientific = FALSE)

            #STB May 2012 adding blanks
            blankx <- c(NA)

            ivsanovax <- cbind(temp2x[1], blankx, temp2x[2], blankx, col3x, blankx, col4x)
            source2x <- rownames(ivsanovax)
            source3x <- rownames(ivsanovax)
            headx <- c("Num. df", " ", "Den. df", " ", "F-value", " ", "p-value")
            colnames(ivsanovax) <- headx

            # Correction to code to ammend lowest p-value: STB Oct 2010
            #	for (i in 1:(dim(ivsanovax)[1]-1)) 
            for (i in 1:(dim(ivsanovax)[1])) {
                if (temp2x[i, 4] < 0.0001) {
                    #STB - Mar 2011 formatting p<0.0001
                    #		ivsanovax[i,7]<-0.0001
                    ivsanovax[i, 7] = format(round(0.0001, 4), nsmall = 4, scientific = FALSE)
                    ivsanovax[i, 7] <- paste("<", ivsanovax[i, 7])
                }
            }

            #Remove intercept row
            ivsanovax <- ivsanovax[-c(1),]
            tempyx <- gsub(pattern = "Timezzz", replacement = timeFactor, rownames(ivsanovax))

            #STB March 2014 - Replacing : with * in ANOVA table
            for (q in 1:length(tempyx)) {
                tempyx <- sub(":", " * ", tempyx)
            }
            rownames(ivsanovax) <- tempyx

            HTML(ivsanovax, classfirstline = "second", align = "left")

            add <- c("Note: This table should only be used to assess the covariate interactions. The statistical model used to generate all the remaining results in this output does not include the covariate interactions.")
            HTML.title("</bf> ", HR = 2, align = "left")
            HTML.title(add, HR = 0, align = "left")
        }

        if (min(tempx[2], na.rm = TRUE) == 0) {
            add <- c("The covariate interactions have not been calculated as there are zero residual degrees of freedom when all terms are included in the statistical model.")
            HTML.title("</bf> ", HR = 2, align = "left")
            HTML.title(add, HR = 0, align = "left")

        }
    }
}




#-------------------------------------------------------------------------------------------------
#Set up final model
#-------------------------------------------------------------------------------------------------
#Test to see if there are 0 df, in which case end module

model2 <- paste(Args[4], " + ", subjectFactor, sep = "")
threewayfullxxx <- lm(as.formula(model2), data = statdata, na.action = (na.omit))

if (df.residual(threewayfullxxx) < 1) {
    HTMLbr()
    HTML.title("<bf>Table of overall tests of model effects", HR = 2, align = "left")
    add <- c("Unfortunately there are not enough degrees of freedom to estimate all the effects, hence no analysis has been produced. Please reduce the number of effects in your statistical model.")
    HTML.title("</bf> ", HR = 2, align = "left")
    HTML.title(add, HR = 0, align = "left")
    quit()
}


if (covariance == "Compound Symmetric") {
    threewayfull <- lme(model, random = ~1 | subjectzzzzzz, data = statdata, correlation = corCompSymm(), na.action = (na.omit), method = "REML")
}
if (covariance == "Autoregressive(1)") {
    threewayfull <- lme(model, random = ~1 | subjectzzzzzz, correlation = corAR1(value = 0.4, form = ~as.numeric(Timezzz) | subjectzzzzzz, fixed = FALSE), data = statdata, na.action = (na.omit), method = "REML")
}
if (covariance == "Unstructured") {
    threewayfull <- lme(model, random = ~1 | subjectzzzzzz, correlation = corSymm(form = ~as.numeric(Timezzz) | subjectzzzzzz), weights = varIdent(form = ~1 | as.numeric(Timezzz)), data = statdata, na.action = (na.omit), method = "REML")
}


#============================================================================================================================
#ANOVA Table
#============================================================================================================================
if (showANOVA == "Y") {

    #STB Aug 2014 add in marginal sums of squares
    #temp<-anova(threewayfull, type="sequential")
    temp <- anova(threewayfull, type = "marginal")
    temp2 <- (temp)

    blank <- c(NA)
    col3 <- format(round(temp2[3], 2), nsmall = 2, scientific = FALSE)
    col4 <- format(round(temp2[4], 4), nsmall = 4, scientific = FALSE)
    ivsanova <- cbind(temp2[1], blank, temp2[2], blank, col3, blank, col4)
    source2 <- rownames(ivsanova)
    source3 <- rownames(ivsanova)
    head <- c("Num. df", " ", "Den. df", " ", "F-value", " ", "p-value")
    colnames(ivsanova) <- head

    #STB - March 2011 Change p-value to be less than 0.0001
    #	for (i in 1:(dim(ivsanova)[1]-1)) 
    for (i in 1:(dim(ivsanova)[1])) {
        if (temp2[i, 4] < 0.0001) {
            #			ivsanova[i,7]<-0.0001
            ivsanova[i, 7] = format(round(0.0001, 4), nsmall = 4, scientific = FALSE)
            ivsanova[i, 7] <- paste("<", ivsanova[i, 7])
        }
    }

    #Remove intercept row
    ivsanova <- ivsanova[-c(1),]
    tempy <- gsub(pattern = "Timezzz", replacement = timeFactor, rownames(ivsanova))
    rownames(ivsanova) <- tempy



    #STB Aug 2014 updating paired t-test to exclude covariate
    if (dimfact == 2 && dim(ivsanova)[1] == 1) {
        HTMLbr()
        HTML.title("<bf>Paired t-test result", HR = 2, align = "left")
    } else {
        HTMLbr()
        HTML.title("<bf>Table of overall tests of model effects", HR = 2, align = "left")
    }

    HTML(ivsanova, classfirstline = "second", align = "left")
    HTML.title("</bf> ", HR = 2, align = "left")

    #STB May 2012 - correcting comment for paired t-test
    #STB Aug 2014 updating paired t-test to exclude covariate
    if (dimfact == 2 && dim(ivsanova)[1] == 1) {
        #STB August 2014 change in message
        HTML.title("<bf>Comment: The test in this table is a likelihood ratio test. ", HR = 0, align = "left")
    } else {
        #STB August 2014 change in message
        #	HTML.title("<bf>Comment: The overall test(s) in this table are likelihood ratio tests. ", HR=0, align="left")
        HTML.title("<bf>Comment: The overall test(s) in this table are marginal likelihood ratio tests, where the order they appear in the table does not influence the results.", HR = 0, align = "left")
    }
    add <- paste(c("Conclusion"))
    inte <- 1
    for (i in 1:(dim(ivsanova)[1])) {
        if (ivsanova[i, 7] <= (1 - sig)) {
            if (inte == 1) {
                inte <- inte + 1
                add <- paste(add, ": At the ", sep = "")

                add <- paste(add, 100 * (1 - sig), sep = "")
                add <- paste(add, "% level", sep = "")

                add <- paste(add, " there is a statistically significant overall difference between the levels of ", sep = "")
                add <- paste(add, rownames(ivsanova)[i], sep = "")
            } else {
                inte <- inte + 1
                add <- paste(add, ", ", sep = "")
                add <- paste(add, rownames(ivsanova)[i], sep = "")
            }
        }
    }
    if (inte == 1) {
        if (dim(ivsanova)[1] > 2) {
            add <- paste(add, ": There are no statistically significant overall differences, at the ", sep = "")
            add <- paste(add, 100 * (1 - sig), sep = "")
            add <- paste(add, "% level, ", sep = "")
            add <- paste(add, "between the levels of any of the terms in the table of overall tests", sep = "")
        } else {
            add <- paste(add, ": There is no statistically significant overall difference, at the ", sep = "")
            add <- paste(add, 100 * (1 - sig), sep = "")
            add <- paste(add, "% level, ", sep = "")
            add <- paste(add, "between the levels of the treatment factor", sep = "")
        }
    }
    add <- paste(add, ". ", sep = "")
    HTML.title("</bf> ", HR = 2, align = "left")
    HTML.title(add, HR = 0, align = "left")
    HTML.title("</bf> ", HR = 2, align = "left")

    if (dimfact > 2) {
        HTML.title("<bf> Tip: While it is a good idea to consider the overall tests in the above table, we should not rely on them when deciding whether or not to make pairwise comparisons.", HR = 0, align = "left")
    }

    # Warning message for degrees of freedom
    if (min(ivsanova[3]) < 5) {
        HTMLbr()
        HTML.title("<bf>Warning", HR = 2, align = "left")
        add <- c("Unfortunately one or more of the residual degrees of freedom in the above table are low (less than 5). This may make the estimation of the underlying variability, and hence the results of the statistical tests, unreliable. Care must be taken when assessing the results of this analysis.")
        HTML.title("</bf> ", HR = 2, align = "left")
        HTML.title(add, HR = 0, align = "left")
        print(add)
    }
}



#============================================================================================================================
#Covariate correlation table
#============================================================================================================================

if (CovariateRegressionCoefficients == "Y" && covariateModel != "NULL") {
    HTMLbr()
    HTML.title("<bf>Covariate regression coefficient", HR = 2, align = "left")

    covtable_1 <- summary(threewayfull)$tTable
    covtable <- data.frame(covtable_1)[c(2),]
    colnames(covtable) <- c("Estimate", "Std..Error", "DF", "t.value", "Pr...t..")

    covtable_2 <- covtable

    covtable$Estimate <- format(round(covtable$Estimate, 3), nsmall = 3, scientific = FALSE)
    covtable$Std..Error <- format(round(covtable$Std..Error, 3), nsmall = 3, scientific = FALSE)
    covtable$DF <- format(round(covtable$DF, 0), nsmall = 0, scientific = FALSE)
    covtable$t.value <- format(round(covtable$t.value, 2), nsmall = 2, scientific = FALSE)
    covtable$Pr...t.. <- format(round(covtable$Pr...t.., 4), nsmall = 4, scientific = FALSE)

    covtable_1 <- covtable

    if (as.numeric(covtable_2[1, 5]) < 0.0001) {
        #STB March 2011 formatting p-values p<0.0001
        #		ivsanova[i,9]<-0.0001
        covtable_1[1, 5] = "<0.0001"
    }
    rz <- rownames(covtable)[1]
    rownames(covtable_1) <- c(rz)

    colnames(covtable_1) <- c("Estimate", "Std error", "DF", "t-value", "p-value")
    HTML(covtable_1, classfirstline = "second", align = "left")
}




#============================================================================================================================
#Diagnostic plot titles
#============================================================================================================================
if (showPRPlot == "Y") {
    HTMLbr()
    HTML.title("<bf>Diagnostic plots", HR = 2, align = "left")
} else {
    if (showNormPlot == "Y") {
        HTMLbr()
        HTML.title("<bf>Diagnostic plots", HR = 2, align = "left")
    }
}

#Residual plots
if (showPRPlot == "Y") {
    residualPlot <- sub(".html", "residualplot.jpg", htmlFile)
    jpeg(residualPlot, width = jpegwidth, height = jpegheight, quality = 100)

    plotFilepdf3 <- sub(".html", "residualplot.pdf", htmlFile)
    dev.control("enable")

    #STB November 2013 include yaxt and axis2 to allign y-axis var in horizontal position
    residplot <- cbind(predict(threewayfull, level = 1), residuals(threewayfull, level = 1, type = "pearson"))
    rownames(residplot) <- c(1:(dim(residplot)[1]))
    residplot <- data.frame(residplot)
    colnames(residplot) <- c("Predicted", "Standardized_residuals")

    #==================================================================================================================================================
    #==================================================================================================================================================
    #Graphical parameters
    graphdata <- residplot

    graphdata$yvarrr_IVS <- graphdata$Standardized_residuals
    graphdata$xvarrr_IVS <- graphdata$Predicted
    XAxisTitle <- "Predicted values"
    YAxisTitle <- "Standardised residuals"
    MainTitle2 <- "Residuals vs. predicted plot \n"
    w_Gr_jit <- 0
    h_Gr_jit <- 0
    infiniteslope <- "Y"

    if (bandw != "N") {
        Gr_line <- BW_line
        Gr_fill <- BW_fill
    } else {
        Gr_line <- Col_line
        Gr_fill <- Col_fill
    }
    Gr_line_type <- Line_type_dashed

    Line_size <- 0.5

    #GGPLOT2 code
    NONCAT_SCAT("RESIDPLOT")

    MainTitle2 <- ""
    #==================================================================================================================================================
    #==================================================================================================================================================
    void <- HTMLInsertGraph(GraphFileName = sub("[A-Z0-9a-z,:,\\\\]*App_Data[\\\\]", "", residualPlot), Align = "centre")

    #STB July2013
    if (pdfout == "Y") {
        pdf(file = sub("[A-Z0-9a-z,:,\\\\]*App_Data[\\\\]", "", plotFilepdf3), height = pdfheight, width = pdfwidth)
        dev.set(2)
        dev.copy(which = 3)
        dev.off(2)
        dev.off(3)
        pdfFile_3 <- sub("[A-Z0-9a-z,:,\\\\]*App_Data[\\\\]", "", plotFilepdf3)
        linkToPdf3 <- paste("<a href=\"", pdfFile_3, "\">Click here to view the PDF of the residuals vs. predicted plot</a>", sep = "")
        HTML(linkToPdf3)
    }

    HTML.title("<bf> ", HR = 2, align = "left")
    HTML.title("<bf>Tip: On this plot look to see if the spread of the points increases as the predicted values increase. If so the response may need transforming.", HR = 0, align = "left")
    HTML.title("<bf> ", HR = 2, align = "left")
    HTML.title("<bf>Tip: Any observation with a standardised residual less than -3 or greater than 3 (SD) should be investigated as a possible outlier.", HR = 0, align = "left")
    HTML.title("<bf> ", HR = 2, align = "left")
    HTML.title("<bf>Comment: The standardised residuals at level i are obtained by subtracting the fitted levels at that level from the response vector and dividing by the estimated within-group standard error.", HR = 0, align = "left")
    #The residuals at level i are obtained by subtracting the fitted levels at that level from the response vector and dividing by the estimated within-group standard error.
}

#Normality plots
if (showNormPlot == "Y") {
    HTMLbr()
    normPlot <- sub(".html", "normplot.jpg", htmlFile)
    jpeg(normPlot, width = jpegwidth, height = jpegheight, quality = 100)

    plotFilepdf4 <- sub(".html", "normplot.pdf", htmlFile)
    dev.control("enable")

    #==================================================================================================================================================
    #==================================================================================================================================================
    #Graphical parameters

    te <- qqnorm(resid(threewayfull, level = 1))
    graphdata <- data.frame(te$x, te$y)
    graphdata$xvarrr_IVS <- graphdata$te.x
    graphdata$yvarrr_IVS <- graphdata$te.y
    YAxisTitle <- "Sample Quantiles"
    XAxisTitle <- "Theoretical Quantiles"
    MainTitle2 <- "Normal probability plot \n"
    w_Gr_jit <- 0
    h_Gr_jit <- 0
    infiniteslope <- "N"
    LinearFit <- "Y"

    if (bandw != "N") {
        Gr_line <- BW_line
        Gr_fill <- BW_fill
    } else {
        Gr_line <- Col_line
        Gr_fill <- Col_fill
    }
    Gr_line_type <- Line_type_dashed

    Line_size <- 0.5
    Gr_alpha <- 1
    Line_type <- Line_type_dashed

    #GGPLOT2 code
    NONCAT_SCAT("QQPLOT")

    MainTitle2 <- ""
    #==================================================================================================================================================
    #==================================================================================================================================================
    void <- HTMLInsertGraph(GraphFileName = sub("[A-Z0-9a-z,:,\\\\]*App_Data[\\\\]", "", normPlot), Align = "left")

    #STB July2013
    if (pdfout == "Y") {
        pdf(file = sub("[A-Z0-9a-z,:,\\\\]*App_Data[\\\\]", "", plotFilepdf4), height = pdfheight, width = pdfwidth)
        dev.set(2)
        dev.copy(which = 3)
        dev.off(2)
        dev.off(3)
        pdfFile_4 <- sub("[A-Z0-9a-z,:,\\\\]*App_Data[\\\\]", "", plotFilepdf4)
        linkToPdf4 <- paste("<a href=\"", pdfFile_4, "\">Click here to view the PDF of the normal probability plot</a>", sep = "")
        HTML(linkToPdf4)
    }

    HTML.title("<bf> ", HR = 2, align = "left")
    HTML.title("<bf>Tip: Check that the points lie along the dotted line. If not then the data may be non-normally distributed.", HR = 0, align = "left")
}


#==================================================================================================================================================
# Means and Planned comparisons on the main effects
if (showLSMeans == "Y") {
    statdata$betweenwithin <- as.factor(eval(parse(text = paste("statdata$", timeFactor))))

    #Code to calculate y-axis offset (lens) in LSMeans plot
    names <- levels(statdata$betweenwithin)
    index <- 1
    for (i in 1:length(names)) {
        temp <- names[i]
        temp <- as.character(unlist(strsplit(as.character(names[i]), "")))
        lens <- length(temp)
        if (lens > index) {
            index <- lens
        }
    }

    # LS Means
    if (covar != "NULL") {
        covariate <- unlist(strsplit(covariateModel, "~"))[2]
        covc <- eval(parse(text = paste("statdata$", covariate)))
        statdata <- data.frame(cbind(statdata, covc))
        covc2 <- "covc"
    }

    #Identify within animal degrees of freedom
    df <- anova(threewayfull)[dim(anova(threewayfull))[1], 2]


    #Calculate LS Means
    tabs <- lsmeans(threewayfull, ~ Timezzz, data = statdata)

    x <- summary(tabs)
    LSM <- data.frame(x)
    leng <- dim(LSM)[1]

    for (i in 1:leng) {
        LSM$DDF[i] <- df
    }

    LSM$Mean <- LSM$lsmean
    LSM$Lower = LSM$lsmean - qt(1 - (1 - sig) / 2, df) * LSM$SE
    LSM$Upper = LSM$lsmean + qt(1 - (1 - sig) / 2, df) * LSM$SE
    LSM$Group_IVSq_ <- LSM$Timezzz

    HTMLbr()
    CITitle <- paste("<bf>Plot of the least square (predicted) means with ", (sig * 100), "% confidence intervals", sep = "")
    HTML.title(CITitle, HR = 2, align = "left")

    meanPlot <- sub(".html", "meanplot.jpg", htmlFile)
    jpeg(meanPlot, width = jpegwidth, height = jpegheight, quality = 100)

    #STB July2013
    plotFilepdf5 <- sub(".html", "meanplot.pdf", htmlFile)
    dev.control("enable")
    #============================================================================================================================
    #============================================================================================================================
    #Parameters
    Gr_alpha <- 0
    if (bandw != "N") {
        Gr_fill <- BW_fill
    } else {
        Gr_fill <- Col_fill
    }
    YAxisTitle <- LS_YAxisTitle
    XAxisTitle <- CPXAxisTitle
    MainTitle2 <- ""
    Gr_line <- "black"
    graphdata <- LSM

    #GGPLOT2 code
    LSMPLOT_1()
    #============================================================================================================================
    #============================================================================================================================
    void <- HTMLInsertGraph(GraphFileName = sub("[A-Z0-9a-z,:,\\\\]*App_Data[\\\\]", "", meanPlot), Align = "left")

    #STB July2013
    if (pdfout == "Y") {
        pdf(file = sub("[A-Z0-9a-z,:,\\\\]*App_Data[\\\\]", "", plotFilepdf5), height = pdfheight, width = pdfwidth)
        dev.set(2)
        dev.copy(which = 3)
        dev.off(2)
        dev.off(3)
        pdfFile_5 <- sub("[A-Z0-9a-z,:,\\\\]*App_Data[\\\\]", "", plotFilepdf5)
        linkToPdf5 <- paste("<a href=\"", pdfFile_5, "\">Click here to view the PDF of the plot of the least square (predicted) means</a>", sep = "")
        HTML(linkToPdf5)
    }

    HTMLbr()

    #Table of LS Means
    CITitle2 <- paste("<bf>Table of the least square (predicted) means with ", (sig * 100), "% confidence intervals", sep = "")
    HTML.title(CITitle2, HR = 2, align = "left")

    LSMx <- LSM
    LSM$Mean <- format(round(LSM$Mean, 3), nsmall = 3)
    LSM$Lower <- format(round(LSM$Lower, 3), nsmall = 3)
    LSM$Upper <- format(round(LSM$Upper, 3), nsmall = 3)

    rblanq <- c("Level")
    rowz <- c(rblanq, levels(LSM$Timezzz))
    blanq <- c(" ", " ", " ")
    LSM <- rbind(blanq, LSM)
    rownames(LSM) <- rowz

    LSM <- subset(LSM, select = -c(df, SE, asymp.LCL, asymp.UCL, DDF, Timezzz, lsmean, Group_IVSq_))
    colnames(LSM) <- c("Mean", paste("Lower ", (sig * 100), "% CI", sep = ""), paste("Upper ", (sig * 100), "% CI", sep = ""))

    HTML(LSM, classfirstline = "second", align = "left")


    #============================================================================================================================
    #Back transformed geometric means plot and table 
    #============================================================================================================================
    if (GeomDisplay == "Y" && showLSMeans == "Y" && (responseTransform == "Log10" || responseTransform == "Loge")) {
        HTMLbr()
        CITitle <- paste("<bf>Plot of the back-transformed geometric means with ", (sig * 100), "% confidence intervals", sep = "")
        HTML.title(CITitle, HR = 2, align = "left")

        add <- c("As the response was log transformed prior to analysis the least square (predicted) means are presented on the log scale. These results can be back transformed onto the original scale. These are known as the back-transformed geometric means.")
        HTML.title("</bf> ", HR = 2, align = "left")
        HTML.title(add, HR = 0, align = "left")

        if (responseTransform == "Log10") {
            LSMx$Mean <- 10 ^ (LSMx$lsmean)
            LSMx$Lower = 10 ^ (LSMx$lsmean - qt(1 - (1 - sig) / 2, df) * LSMx$SE)
            LSMx$Upper = 10 ^ (LSMx$lsmean + qt(1 - (1 - sig) / 2, df) * LSMx$SE)
            LSMx$Group_IVSq_ <- LSMx$Timezzz
        }

        if (responseTransform == "Loge") {
            LSMx$Mean <- exp(LSMx$lsmean)
            LSMx$Lower = exp(LSMx$lsmean - qt(1 - (1 - sig) / 2, df) * LSMx$SE)
            LSMx$Upper = exp(LSMx$lsmean + qt(1 - (1 - sig) / 2, df) * LSMx$SE)
            LSMx$Group_IVSq_ <- LSMx$Timezzz
        }

        meanPlotz <- sub(".html", "meanplotz.jpg", htmlFile)
        jpeg(meanPlotz, width = jpegwidth, height = jpegheight, quality = 100)

        #STB July2013
        plotFilepdf5z <- sub(".html", "meanplotz.pdf", htmlFile)
        dev.control("enable")
        #============================================================================================================================
        #============================================================================================================================
        #Parameters
        Gr_alpha <- 0
        if (bandw != "N") {
            Gr_fill <- BW_fill
        } else {
            Gr_fill <- Col_fill
        }
        YAxisTitle <- LS_YAxisTitle
        XAxisTitle <- CPXAxisTitle
        MainTitle2 <- ""
        Gr_line <- "black"
        graphdata <- LSMx

        #GGPLOT2 code
        LSMPLOT_1()
        #============================================================================================================================
        #============================================================================================================================
        void <- HTMLInsertGraph(GraphFileName = sub("[A-Z0-9a-z,:,\\\\]*App_Data[\\\\]", "", meanPlotz), Align = "left")

        #STB July2013
        if (pdfout == "Y") {
            pdf(file = sub("[A-Z0-9a-z,:,\\\\]*App_Data[\\\\]", "", plotFilepdf5z), height = pdfheight, width = pdfwidth)
            dev.set(2)
            dev.copy(which = 3)
            dev.off(2)
            dev.off(3)
            pdfFile_5z <- sub("[A-Z0-9a-z,:,\\\\]*App_Data[\\\\]", "", plotFilepdf5z)
            linkToPdf5z <- paste("<a href=\"", pdfFile_5z, "\">Click here to view the PDF of the plot of the back-transformed geometric means</a>", sep = "")
            HTML(linkToPdf5z)
        }

        HTMLbr()

        #Table of LS Means
        CITitle2 <- paste("<bf>Table of the back-transformed geometric means with ", (sig * 100), "% confidence intervals", sep = "")
        HTML.title(CITitle2, HR = 2, align = "left")

        LSMx$Mean <- format(round(LSMx$Mean, 3), nsmall = 3)
        LSMx$Lower <- format(round(LSMx$Lower, 3), nsmall = 3)
        LSMx$Upper <- format(round(LSMx$Upper, 3), nsmall = 3)

        rblanq <- c("Level")
        rowz <- c(rblanq, levels(LSMx$Timezzz))
        blanq <- c(" ", " ", " ")
        LSMx <- rbind(blanq, LSMx)
        rownames(LSMx) <- rowz

        LSMx <- subset(LSMx, select = -c(df, SE, asymp.LCL, asymp.UCL, DDF, Timezzz, lsmean, Group_IVSq_))
        colnames(LSMx) <- c("Geometric mean", paste("Lower ", (sig * 100), "% CI", sep = ""), paste("Upper ", (sig * 100), "% CI", sep = ""))

        HTML(LSMx, classfirstline = "second", align = "left")
    }
}

#============================================================================================================================
#All pairwise tests
statdata$mainEffect <- as.factor(eval(parse(text = paste("statdata$", timeFactor))))
len <- length(levels(statdata$mainEffect))
k <- 1
Group1 <- c(1)
Group2 <- c(1)
Group6 <- c(1)
Group7 <- c(1)
Group8 <- c(1)
Group9 <- c(1)

for (i in 1:(len - 1)) {
    for (j in (i + 1):len) {
        Group1[k] = levels(statdata$mainEffect)[i]
        Group2[k] = levels(statdata$mainEffect)[j]
        k = k + 1
    }
}

Gplen <- length(Group1)
Group4 <- strsplit(Group1, "_.._")
Group5 <- strsplit(Group2, "_.._")
txtGroup4Changes <- c(" ")
for (i in 1:length(Group4[[1]])) {
    txtGroup4Changes[length(txtGroup4Changes) + 1] = (Group4[[1]][i])
}
Group4len <- length(txtGroup4Changes) - 1

for (i in 1:Gplen) {
    Group8[i] = Group4[[i]][1]
    Group9[i] = Group4[[i]][Group4len]
    Group6[i] = Group5[[i]][1]
    Group7[i] = Group5[[i]][Group4len]

    if (Group4len > 2) {
        for (j in 2:(Group4len - 1)) {
            Group8[i] = paste(Group8[i], " ", Group4[[i]][j])
            Group6[i] = paste(Group6[i], " ", Group5[[i]][j])
        }
    }
}

Group3 <- cbind(Group6, Group7, Group8, Group9)

#Calculate denomenator df
if (covar != "NULL") {
    covariate <- unlist(strsplit(covariateModel, "~"))[2]
    covc <- eval(parse(text = paste("statdata$", covariate)))
    statdata <- data.frame(cbind(statdata, covc))
    covc2 <- "covc"
    subject <- Args[6]
    subject2 <- as.factor(eval(parse(text = paste("statdata$", subject))))
    statdata <- data.frame(cbind(statdata, subject2))
    subject3 <- "subject2"
    dfmodl <- eval(parse(text = paste(resp, "~", covc2, " +", subject3, " + mainEffect")))
    dfanova <- anova(lm(dfmodl, data = statdata))
    dendf <- (dfanova[dim(dfanova)[1], 1])
} else {
    subject <- Args[6]
    subject2 <- as.factor(eval(parse(text = paste("statdata$", subject))))
    statdata <- data.frame(cbind(statdata, subject2))
    subject3 <- "subject2"
    dfmodl <- eval(parse(text = paste(resp, "~", subject3, " + mainEffect")))
    dfanova <- anova(lm(dfmodl, data = statdata))
    dendf <- (dfanova[dim(dfanova)[1], 1])
}

if (dimfact > 1) {
    if (covariance == "Compound Symmetric") {
        test <- lme(model, random = ~1 | subjectzzzzzz, correlation = corCompSymm(), data = statdata, na.action = (na.omit), method = "REML")
        mult <- glht(test, linfct = mcp(Timezzz = "Tukey"), df = dendf)
        multci <- confint(mult, level = sig, calpha = univariate_calpha())
    }
    if (covariance == "Autoregressive(1)") {
        test <- lme(model, random = ~1 | subjectzzzzzz, correlation = corAR1(form = ~as.numeric(Timezzz) | subjectzzzzzz), data = statdata, na.action = (na.omit), method = "REML")

        mult <- glht(test, linfct = mcp(Timezzz = "Tukey"), df = dendf)
        multci <- confint(mult, level = sig, calpha = univariate_calpha())
    }
    if (covariance == "Unstructured") {
        test <- lme(model, random = ~1 | subjectzzzzzz, correlation = corSymm(form = ~as.numeric(Timezzz) | subjectzzzzzz), weights = varIdent(form = ~1 | as.numeric(Timezzz)), data = statdata, na.action = (na.omit), method = "REML")
        mult <- glht(test, linfct = mcp(Timezzz = "Tukey"), df = dendf)
        multci <- confint(mult, level = sig, calpha = univariate_calpha())
    }
    multp <- summary(mult, test = adjusted("none"))
    pvals <- multp$test$pvalues
    sigma <- multp$test$sigma
    tablen <- length(unique(rownames(multci$confint)))
    tabs <- matrix(nrow = tablen, ncol = 6)

    if (dimfact > 1) {
        for (i in 1:tablen) {
            #STB Dec 2011 Formatting to 3dp
            tabs[i, 1] = format(round(multci$confint[i], 3), nsmall = 3, scientific = FALSE)
        }
        for (i in 1:tablen) {
            tabs[i, 2] = format(round(multci$confint[i + tablen], 3), nsmall = 3, scientific = FALSE)
        }
        for (i in 1:tablen) {
            tabs[i, 3] = format(round(multci$confint[i + 2 * tablen], 3), nsmall = 3, scientific = FALSE)
        }
        for (i in 1:tablen) {
            tabs[i, 4] = format(round(sigma[i], 3), nsmall = 3, scientific = FALSE)
        }
        for (i in 1:tablen) {
            tabs[i, 5] = format(round(pvals[i], 4), nsmall = 4, scientific = FALSE)
        }
        for (i in 1:tablen) {
            tabs[i, 6] = pvals[i]
        }
        for (i in 1:tablen) {
            if (pvals[i] < 0.0001) {
                #tabs[i,5]<-0.0001
                tabs[i, 5] = format(round(0.0001, 4), nsmall = 4, scientific = FALSE)
                tabs[i, 5] <- paste("<", tabs[i, 5])
            }
        }

        if (dimfact > 2) {
            add <- paste(c("All pairwise comparisons, without adjustment for multiplicity"))
        } else {
            add <- paste(c("Comparison between the least square (predicted) means, with 95% confidence interval"))
        }

        #zz		if(dimfact > 2)
        #zz		{
        HTMLbr()
        HTML.title(add, HR = 2, align = "left")
        #zz		}
        header <- c(" ", " ", " ", " ", " ", " ")
        tabls <- rbind(header, tabs)
        rows <- rownames(multci$confint)
        for (i in 1:100) {
            rows <- sub("_.._", " ", rows, fixed = TRUE)
        }
        rows <- sub(" - ", " vs. ", rows, fixed = TRUE)
        rownames(tabls) <- c("Comparison", rows)
        lowerCI <- paste("   Lower ", (sig * 100), "% CI   ", sep = "")
        upperCI <- paste("   Upper ", (sig * 100), "% CI   ", sep = "")

        #STB May 2012 correcting "SEM"
        colnames(tabls) <- c("   Difference   ", lowerCI, upperCI, "   Std error   ", "   p-value   ", "temp")
        tabls2 <- subset(tabls, select = -c(temp))

        #zz		if (dimfact > 2)
        #zz		{		
        HTML(tabls2, classfirstline = "second", align = "left")
        HTML.title("<bf> ", HR = 2, align = "left")

        if (dimfact > 2) {
            HTML.title("<bf>Tip: The p-values in this table are unadjusted for multiple comparisons. No options are available in this module to make multiple comparison adjustments because it is highly unlikely you would want to make all these pairwise comparisons. If you wish to apply a multiple comparison adjustment to these results then use the p-value adjustment module.", HR = 0, align = "left")
        }


        #============================================================================================================================
        #STB March 2014 - Creating a dataset of p-values
        comparisons <- paste(direct, "/Comparisons.csv", sep = "")
        tabsx <- data.frame(tabls[, 6])
        row <- rownames(tabls)

        for (i in 1:100) {
            row <- sub(",", " and ", row, fixed = TRUE)
        }
        tabsx <- cbind(row, tabsx)

        colnames(tabsx) <- c("Comparison", "p-value")
        row.names(tabsx) <- seq(nrow(tabsx))
        tabsx <- tabsx[-1,]


        #============================================================================================================================
        #Conclusion
        add <- paste(c("Conclusion"))
        inte <- 1

        #STB April 2011 Ammending text in results section for paired test

        if (tablen > 1) {
            for (i in 2:(dim(tabls)[1])) {
                if (tabls[i, 5] <= (1 - sig)) {
                    if (inte == 1) {
                        inte <- inte + 1
                        add <- paste(add, ": The following pairwise tests are statistically significant at the  ", sep = "")
                        add <- paste(add, 100 * (1 - sig), sep = "")
                        add <- paste(add, "% level: ", sep = "")
                        add <- paste(add, rownames(tabls)[i], sep = "")
                    } else {
                        inte <- inte + 1
                        add <- paste(add, ", ", sep = "")
                        add <- paste(add, rownames(tabls)[i], sep = "")
                    }
                }
            }
        } else {
            for (i in 2:(dim(tabls)[1])) {
                if (tabls[i, 5] <= (1 - sig)) {
                    if (inte == 1) {
                        inte <- inte + 1
                        add <- paste(add, ": The result of the paired t-test is statistically significant at the  ", sep = "")
                        add <- paste(add, 100 * (1 - sig), sep = "")
                        add <- paste(add, "% level", sep = "")
                    } else {
                        inte <- inte + 1
                        add <- paste(add, ", ", sep = "")
                        add <- paste(add, rownames(tabls)[i], sep = "")
                    }
                }
            }
        }
        if (inte == 1) {
            if (tablen > 1) {
                add <- paste(add, ": There are no statistically significant pairwise differences.", sep = "")
            } else {
                #STB May 2012 Updating "paired"
                add <- paste(add, ": The result of the paired t-test is not statistically significant.", sep = "")
            }
        } else {
            add <- paste(add, ". ", sep = "")
        }
        #zz 	if (dimfact > 2)
        #zz 	{
        HTML.title("</bf> ", HR = 2, align = "left")
        HTML.title(add, HR = 0, align = "left")
        #zz	}
        if (length(grep("\\*", model)) == 0 && length(grep("\\+", model)) == 0 && length(grep("\\+", model)) == 1) {
            add2 <- paste(c(" "), " ", sep = "")
            HTML.title("<bf> ", HR = 2, align = "left")
            HTML.title(add2, HR = 0, align = "left")
        } else if (length(grep("\\*", model)) == 0 && length(grep("\\+", model)) == 0 && length(grep("\\+", model)) == 0) {
            add2 <- paste(c(" "), " ", sep = "")
            HTML.title("<bf> ", HR = 2, align = "left")
            HTML.title(add2, HR = 0, align = "left")
        }
        if (tablen > 1) {
            HTML.title("<bf> ", HR = 2, align = "left")
            HTML.title("<bf>Warning: As these tests are not adjusted for multiplicity there is a risk of generating false positive results. Only use the pairwise tests you planned to make a-priori, these are the so called planned comparisons, see Snedecor and Cochran (1989).", HR = 0, align = "left")
        }
    }


    #===================================================================================================================
    #Back transformed geometric means table 
    #===================================================================================================================

    #zz	if(GeomDisplay == "Y" && (responseTransform =="Log10"||responseTransform =="Loge") && dimfact >2)
    if (GeomDisplay == "Y" && (responseTransform == "Log10" || responseTransform == "Loge")) {

        add <- c("All pairwise comparisons as back-transformed ratios")
        HTMLbr()
        HTML.title(add, HR = 2, align = "left")

        add <- c("As the response was log transformed prior to analysis the differences between the least square (predicted) means are presented on the log scale. These results can be back-transformed onto the original scale, where differences on the log scale become ratios when back-transformed.")
        HTML.title("</bf> ", HR = 2, align = "left")
        HTML.title(add, HR = 0, align = "left")

        tabs <- matrix(nrow = tablen, ncol = 3)

        if (dimfact > 1) {
            if (responseTransform == "Log10") {
                for (i in 1:tablen) {
                    tabs[i, 1] = format(round(10 ^ (multci$confint[i]), 3), nsmall = 3, scientific = FALSE)
                }
                for (i in 1:tablen) {
                    tabs[i, 2] = format(round(10 ^ (multci$confint[i + tablen]), 3), nsmall = 3, scientific = FALSE)
                }
                for (i in 1:tablen) {
                    tabs[i, 3] = format(round(10 ^ (multci$confint[i + 2 * tablen]), 3), nsmall = 3, scientific = FALSE)
                }
            }


            if (responseTransform == "Loge") {
                for (i in 1:tablen) {
                    tabs[i, 1] = format(round(exp(multci$confint[i]), 3), nsmall = 3, scientific = FALSE)
                }
                for (i in 1:tablen) {
                    tabs[i, 2] = format(round(exp(multci$confint[i + tablen]), 3), nsmall = 3, scientific = FALSE)
                }
                for (i in 1:tablen) {
                    tabs[i, 3] = format(round(exp(multci$confint[i + 2 * tablen]), 3), nsmall = 3, scientific = FALSE)
                }
            }


            header <- c(" ", " ", " ")
            tabls <- rbind(header, tabs)
            rows <- rownames(multci$confint)
            for (i in 1:100) {
                rows <- sub("_.._", " ", rows, fixed = TRUE)
            }
            rows <- sub(" - ", " / ", rows, fixed = TRUE)
            rownames(tabls) <- c("Comparison", rows)
            lowerCI <- paste("   Lower ", (sig * 100), "% CI   ", sep = "")
            upperCI <- paste("   Upper ", (sig * 100), "% CI   ", sep = "")

            colnames(tabls) <- c("   Difference   ", lowerCI, upperCI)
            #zz			if (dimfact > 2)
            #zz			{		
            HTML(tabls, classfirstline = "second", align = "left")
            HTML.title("<bf> ", HR = 2, align = "left")
            #zz			}	
        }
    }
}


#===================================================================================================================
#Analysis description

HTMLbr()
HTML.title("<bf>Analysis description", HR = 2, align = "left")

add <- c("The data were analysed using a ")

if (dimfact == 2 && covariateModel == "NULL") {
    add <- paste(add, "paired t-test, with treatment factor ", sep = "")
    add <- paste(add, timeFactor, sep = "")
} else {
    #STB May 2012 Updating "paired"
    add <- paste(add, "repeated measures mixed model approach, with treatment factor ", sep = "")
    add <- paste(add, timeFactor, sep = "")
}

if (blockFactors != "NULL" && timeFactor != "NULL") {
    add <- paste(add, ", ", sep = "")
} else if (noblockfactors == 1 && blockFactors != "NULL" && timeFactor == "NULL") {
    add <- paste(add, " and ", sep = "")
} else if (noblockfactors > 1 && blockFactors != "NULL" && timeFactor == "NULL") {
    add <- paste(add, ", ", sep = "")
}

if (noblockfactors == 1 && blockFactors != "NULL") {
    add <- paste(add, blockFactors, sep = "")
    add <- paste(add, " as a blocking factor", sep = "")
} else {
    if (noblockfactors > 1) # there is two or more blocking factors
    {
        for (i in 1:noblockfactors) {
            if (i < noblockfactors - 1) {
                add <- paste(add, txtexpectedblockChanges[i + 1], sep = "")
                add <- paste(add, ", ", sep = "")
            } else if (i < noblockfactors) {
                add <- paste(add, txtexpectedblockChanges[i + 1], sep = "")
                add <- paste(add, " and ", sep = "")
            } else if (i == noblockfactors) {
                add <- paste(add, txtexpectedblockChanges[i + 1], sep = "")
            }
        }
        add <- paste(add, " as blocking factors", sep = "")
    }
}

if (covar == "NULL") {
    add <- paste(add, ". ", sep = "")
} else if (timeFactor != "NULL") {
    add <- paste(add, " and  ", sep = "")
    add <- paste(add, unlist(strsplit(covariateModel, "~"))[2], sep = "")
    add <- paste(add, " as the covariate. ", sep = "")
}

if (dimfact > 2) {
    add <- paste(add, "This was followed by planned comparisons on the predicted means to compare the levels of the ", sep = "")
    add <- paste(add, timeFactor, sep = "")
    add <- paste(add, ". ", sep = "")
}
if (responseTransform != "None") {
    add <- paste(add, " The response was ", sep = "")
    add <- paste(add, responseTransform, sep = "")
    add <- paste(add, " transformed prior to analysis to stabilise the variance.", sep = "")
}

if (covariateTransform != "None" && responseTransform != "None") {
    add <- paste(add, " The covariate was also ", sep = "")
    add <- paste(add, covariateTransform, sep = "")
    add <- paste(add, " transformed. ", sep = "")
}

if (responseTransform == "None" && covariateTransform != "None") {
    add <- paste(add, " The covariate was ", sep = "")
    add <- paste(add, covariateTransform, sep = "")
    add <- paste(add, " transformed prior to analysis.", sep = "")
}


HTML.title("</bf> ", HR = 2, align = "left")
HTML.title(add, HR = 0, align = "left")

if (dimfact > 2) {
    if (covariance == "Compound Symmetric") {
        add2 <- c("The compound symmetric covariance structure was used to model the within-subject correlations. When using this structure we assumed that the variability of the responses was the same at each level of ")
        add2 <- paste(add2, timeFactor, sep = "")
        add2 <- paste(add2, " and the correlation between responses from any pair of levels of ", sep = "")
        add2 <- paste(add2, timeFactor, sep = "")
        add2 <- paste(add2, "  is the same.")
        HTML.title("</bf> ", HR = 2, align = "left")
        HTML.title(add2, HR = 0, align = "left")
    }
    if (covariance == "Autoregressive(1)") {
        add2 <- c("The first order autoregressive covariance structure was used to model the within-subject correlations. When using this structure we assumed that the variability of the responses was the same at each level of ")
        add2 <- paste(add2, timeFactor, sep = "")
        add2 <- paste(add2, ". We also assumed that the correlation between responses from any pair of levels of ", sep = "")
        add2 <- paste(add2, timeFactor, sep = "")
        add2 <- paste(add2, " was related to the distance between them.")
        HTML.title("</bf> ", HR = 2, align = "left")
        HTML.title(add2, HR = 0, align = "left")
    }
    if (covariance == "Unstructured") {
        add2 <- c("The unstructured covariance structure allowed the variability of the responses to be different, depending on the level of ")
        add2 <- paste(add2, timeFactor, sep = "")
        add2 <- paste(add2, ". This structure also allowed the correlation between responses from any pair of levels of ", sep = "")
        add2 <- paste(add2, timeFactor, sep = "")
        add2 <- paste(add2, " to be different. While this approach is the most general it should be used with care when there are few subjects, as many parameters need to be estimated. These estimates may not be very reliable.")
        HTML.title("</bf> ", HR = 2, align = "left")
        HTML.title(add2, HR = 0, align = "left")
    }
}

add <- c("A full description of mixed model theory employed, including information on the R nlme package used, can be found in Venables and Ripley (2003) and Pinherio and Bates (2002).")
HTML.title("</bf> ", HR = 2, align = "left")
HTML.title(add, HR = 0, align = "left")



#----------------------------------------------------------------------------------------------------------
#References
#----------------------------------------------------------------------------------------------------------
Ref_list <- R_refs()

#Bate and Clark comment
HTML.title("<bf> ", HR = 2, align = "left")
HTML.title(refxx, HR = 0, align = "left")
HTML.title("<bf> ", HR = 2, align = "left")


HTMLbr()
HTML.title("<bf>Statistical references", HR = 2, align = "left")

HTML.title("<bf> ", HR = 2, align = "left")
HTML.title(Ref_list$BateClark_ref, HR = 0, align = "left")

if (covar != "NULL") {
    HTML.title("<bf> ", HR = 2, align = "left")
    HTML.title("<bf> Morris TR. (1999). Experimental Design and Analysis in Animal Sciences. CABI publishing. Wallingford, Oxon (UK).", HR = 0, align = "left")
}

HTML.title("<bf> ", HR = 2, align = "left")
HTML.title("<bf> Pinherio JC and Bates DM. (2000). Mixed Effects Models in S and S-Plus. Springer-Verlag. New York, Inc.", HR = 0, align = "left")

if (dimfact > 2) {
    HTML.title("<bf> ", HR = 2, align = "left")
    HTML.title("<bf>Snedecor GW and Cochran WG. (1989). Statistical Methods. 8th edition;  Iowa State University Press, Iowa, USA.", HR = 0, align = "left")
}

HTML.title("<bf> ", HR = 2, align = "left")
HTML.title("<bf> Venables WN and Ripley BD. (2003). Modern Applied Statistics with S. 4th Edition; Springer. New York, Inc.", HR = 0, align = "left")



HTMLbr()
HTML.title("<bf>R references", HR = 2, align = "left")

HTML.title("<bf> ", HR = 2, align = "left")
HTML.title(Ref_list$R_ref, HR = 0, align = "left")

HTML.title("<bf> ", HR = 2, align = "left")
HTML.title(Ref_list$GGally_ref, HR = 0, align = "left")

HTML.title("<bf> ", HR = 2, align = "left")
HTML.title(Ref_list$RColorBrewers_ref, HR = 0, align = "left")

HTML.title("<bf> ", HR = 2, align = "left")
HTML.title(Ref_list$GGPLot2_ref, HR = 0, align = "left")

HTML.title("<bf> ", HR = 2, align = "left")
HTML.title(Ref_list$reshape_ref, HR = 0, align = "left")

HTML.title("<bf> ", HR = 2, align = "left")
HTML.title(Ref_list$plyr_ref, HR = 0, align = "left")

HTML.title("<bf> ", HR = 2, align = "left")
HTML.title(Ref_list$scales_ref, HR = 0, align = "left")

HTML.title("<bf> ", HR = 2, align = "left")
HTML.title(Ref_list$nlme_ref, HR = 0, align = "left")

HTML.title("<bf> ", HR = 2, align = "left")
HTML.title(Ref_list$R2HTML_ref, HR = 0, align = "left")

HTML.title("<bf> ", HR = 2, align = "left")
HTML.title(Ref_list$PROTO_ref, HR = 0, align = "left")

HTML.title("<bf> ", HR = 2, align = "left")
HTML.title(Ref_list$Contrast_ref, HR = 0, align = "left")

HTML.title("<bf> ", HR = 2, align = "left")
HTML.title(Ref_list$LSMEANS_ref, HR = 0, align = "left")

HTML.title("<bf> ", HR = 2, align = "left")
HTML.title(Ref_list$multcomp_ref, HR = 0, align = "left")




#----------------------------------------------------------------------------------------------------------
#Show dataset
#----------------------------------------------------------------------------------------------------------


if (showdataset == "Y") {
    HTMLbr()
    HTML.title("<bf>Analysis dataset", HR = 2, align = "left")
    statdata2 <- subset(statdata, select = -c(subjectzzzzzz, Timezzz, mainEffect, subject2))

    if (showLSMeans != "N") {
        statdata2 <- subset(statdata2, select = -c(betweenwithin))
    }

    if (covariateModel != "NULL") {
        statdata2 <- subset(statdata2, select = -c(covc))
    }

    HTML(statdata2, classfirstline = "second", align = "left")

}




#----------------------------------------------------------------------------------------------------------
#Create file of comparisons
#----------------------------------------------------------------------------------------------------------


write.csv(tabsx, file = sub("[A-Z0-9a-z,:,\\\\]*App_Data[\\\\]", "", comparisons), row.names = FALSE)





