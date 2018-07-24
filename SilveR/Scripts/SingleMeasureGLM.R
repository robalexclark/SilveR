#===================================================================================================================
#R Libraries

suppressWarnings(library(multcomp))
suppressWarnings(library(car))
suppressWarnings(library(R2HTML))
suppressWarnings(library(lsmeans))
suppressWarnings(library(MBESS))

#STB 14OCT2015
#Set contrast options for Marginal overall tests
options(contrasts = c(unordered = "contr.sum", ordered = "contr.poly"))

#===================================================================================================================
# retrieve args
Args <- commandArgs(TRUE)

#Read in data
statdata <- read.csv(Args[3], header = TRUE, sep = ",")

#Copy Args
model <- as.formula(Args[4])
scatterplotModel <- as.formula(Args[5])
covariateModel <- Args[6]
responseTransform <- Args[7]
covariateTransform <- Args[8]
FirstCatFactor <- Args[9]
treatFactors <- Args[10]
blockFactors <- Args[11]
showANOVA <- Args[12]
showPRPlot <- Args[13]
showNormPlot <- Args[14]
#----------------------------------------------------------------------------------------------
#----------------------------------------------------------------------------------------------
#SilveR Addition - arguments below adjusted
showStandardisedEffects <- Args[15]
sig2 <- 1 - as.numeric(Args[16]) / 2
#----------------------------------------------------------------------------------------------
#----------------------------------------------------------------------------------------------
sig <- 1 - as.numeric(Args[16])
effectModel <- as.formula(Args[17])
effectModel2 <- Args[17]
selectedEffect <- Args[18]
showLSMeans <- Args[19]
allPairwiseTest <- Args[20]
backToControlTest <- Args[21]
cntrlGroup <- Args[22]

cntrlGroup <- sub("-", "_ivs_dash_ivs_", cntrlGroup, fixed = TRUE)


#working directory
direct2 <- unlist(strsplit(Args[3], "/"))
direct <- direct2[1]
for (i in 2:(length(direct2) - 1)) {
    direct <- paste(direct, "/", direct2[i], sep = "")
}

#===================================================================================================================
#Graphics parameter setup

graphdata <- statdata
if (FirstCatFactor != "NULL") {
    Gr_palette <- palette_FUN(FirstCatFactor)
}
Line_size2 <- Line_size
Labelz_IVS_ <- "N"
#===================================================================================================================
#Setup the html file and associated css file
htmlFile <- sub(".csv", ".html", Args[3]);
#determine the file name of the html file
HTMLSetFile(file = htmlFile)
.HTML.file = htmlFile


#===================================================================================================================
#Output HTML header
HTML.title("<bf>Single Measure Parametric Analysis", HR = 1, align = "left")

if (is.numeric(statdata$mainEffect) == TRUE) {
    cntrlGroup <- paste("'", cntrlGroup, "'", sep = "")
}

#============================================================================================================================
#Parameter setup
statdata$mainEffect <- as.factor(statdata$mainEffect)
statdata$scatterPlotColumn <- as.factor(statdata$scatterPlotColumn)

resp <- unlist(strsplit(Args[4], "~"))[1] #get the response variable from the main model

#Number of factors in Selected effect
factno <- length(unique(strsplit(selectedEffect, "*", fixed = TRUE)[[1]]))


#Removing illegal characters
selectedEffect <- namereplace2(selectedEffect)
selectedEffectx <- namereplace(selectedEffect)

#replace illegal characters in variable names
YAxisTitle <- resp
if (FirstCatFactor != "NULL") {
    XAxisTitle <- unlist(strsplit(covariateModel, "~"))[2]
}

#replace illegal characters in variable names
for (i in 1:10) {
    YAxisTitle <- namereplace(YAxisTitle)

    if (FirstCatFactor != "NULL") {
        XAxisTitle <- namereplace(XAxisTitle)
    }
}
LS_YAxisTitle <- YAxisTitle

#ANOVA (complete)
threewayfull <- lm(model, data = statdata, na.action = na.omit)

#STB June 2015 - Takign a copy of the dataset
statdata_temp <- statdata

#calculating number of block factors
noblockfactors = 0
if (blockFactors != "NULL") {
    tempblockChanges <- strsplit(blockFactors, ",")
    txtexpectedblockChanges <- c("")
    for (i in 1:length(tempblockChanges[[1]])) {
        txtexpectedblockChanges[length(txtexpectedblockChanges) + 1] = (tempblockChanges[[1]][i])
    }
    noblockfactors <- length(txtexpectedblockChanges) - 1
}

#calculating number of treatment factors
tempChanges <- strsplit(treatFactors, ",")
txtexpectedChanges <- c("")
for (i in 1:length(tempChanges[[1]])) {
    txtexpectedChanges[length(txtexpectedChanges) + 1] = (tempChanges[[1]][i])
}
notreatfactors <- length(txtexpectedChanges) - 1

# Testing the factorial combinations
intindex <- length(unique(statdata$scatterPlotColumn))
ind <- 1
for (i in 1:notreatfactors) {
    ind = ind * length(unique(eval(parse(text = paste("statdata$", txtexpectedChanges[i + 1])))))
}

if (intindex != ind) {
    add <- c("Unfortunately not all combinations of the levels of the treatment factors are present in the experimental design. We recommend you manually create a new factor corresponding to the combinations of the levels of the treatment factors.")
    HTML.title("</bf> ", HR = 2, align = "left")
    HTML.title(add, HR = 0, align = "left")
    print(add)
    quit()
}

# Code to create varibale to test if the highest order interaction is selected
testeffects = noblockfactors
if (FirstCatFactor != "NULL") {
    testeffects = noblockfactors + 1
}
emodel <- strsplit(effectModel2, "+", fixed = TRUE)

emodelChanges <- c("")
for (i in 1:length(emodel[[1]])) {
    emodelChanges[length(emodelChanges) + 1] = (emodel[[1]][i])
}
noeffects <- length(emodelChanges) - 2



#============================================================================================================================
#Titles and description
#============================================================================================================================
#Response

title <- c("Response")
if (FirstCatFactor != "NULL") {
    title <- paste(title, " and covariate", sep = "")
}

HTML.title(title, HR = 2, align = "left")
add <- paste(c("The  "), resp, sep = "")
add <- paste(add, " response is currently being analysed by the Single Measures Parametric Analysis module", sep = "")

if (FirstCatFactor != "NULL") {
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

#============================================================================================================================
#Scatterplot
#============================================================================================================================

HTMLbr()
title <- c("Scatterplot of the raw data")
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

#==================================================================================================================================================
#==================================================================================================================================================
#Graphical parameters
graphdata <- statdata
graphdata$yvarrr_IVS <- eval(parse(text = paste("statdata$", resp)))
graphdata$xvarrr_IVS <- statdata$scatterPlotColumn
XAxisTitle <- ""
MainTitle2 <- ""
w_Gr_jit <- 0
h_Gr_jit <- 0
infiniteslope <- "Y"

#GGPLOT2 code
NONCAT_SCAT("SMPA_PLOT")
#==================================================================================================================================================
#==================================================================================================================================================

void <- HTMLInsertGraph(GraphFileName = sub("[A-Z0-9a-z,:,\\\\]*App_Data[\\\\]", "", scatterPlot), Align = "centre")

#STB July2013
if (pdfout == "Y") {
    pdf(file = sub("[A-Z0-9a-z,:,\\\\]*App_Data[\\\\]", "", plotFilepdf1), height = pdfheight, width = pdfwidth)
    dev.set(2)
    dev.copy(which = 3)
    dev.off(2)
    dev.off(3)
    pdfFile_1 <- sub("[A-Z0-9a-z,:,\\\\]*App_Data[\\\\]", "", plotFilepdf1)
    linkToPdf1 <- paste("<a href=\"", pdfFile_1, "\">Click here to view the PDF of the scatterplot</a>", sep = "")
    HTML(linkToPdf1)
}

HTML.title("</bf> ", HR = 2, align = "left")
HTML.title("</bf> Tip: Use this plot to identify possible outliers.", HR = 0, align = "left")

#============================================================================================================================
#Covariate plot
#============================================================================================================================

if (FirstCatFactor != "NULL") {

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
    graphdata$xvarrr_IVS <- eval(parse(text = paste("statdata$", unlist(strsplit(covariateModel, "~"))[2])))
    graphdata$yvarrr_IVS <- eval(parse(text = paste("statdata$", resp)))
    graphdata$l_l <- eval(parse(text = paste("statdata$", FirstCatFactor)))
    graphdata$catfact <- eval(parse(text = paste("statdata$", FirstCatFactor)))

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


    #Testing for with infinite slopes on scatterplot and re-ordering dataset if necessary
    inf_slope <- IVS_F_infinite_slope()
    infiniteslope <- inf_slope$infiniteslope
    graphdata <- inf_slope$graphdata

    #GGPLOT2 code
    OVERLAID_SCAT()
    #================================================================================================================================================
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


    #STB Aug 2011 - removing lines with infinite slope
    if (infiniteslope != "N") {
        title <- "Warning: The covariate has the same value for all subjects in one or more levels of the "
        title <- paste(title, FirstCatFactor, sep = "")
        title <- paste(title, " factor. Care should be taken if you want to include this covariate in the analysis.", sep = "")
        HTML.title("</bf> ", HR = 2, align = "left")
        HTML.title(title, HR = 0, align = "left")
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

if (AssessCovariateInteractions == "Y" && FirstCatFactor != "NULL") {

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

    #Performing the ANCOVA analysis
    Covintfull <- lm(as.formula(CovIntForm), data = statdata, na.action = na.omit)

    #Title + warning
    HTMLbr()
    HTML.title("<bf>Analysis of Covariance (ANCOVA) table for assessing covariate interactions", HR = 2, align = "left")

    #Printing ANCOVA Table - note this code is reused from below - Type III SS included
    if (df.residual(Covintfull) < 1) {
        add <- c("The covariate interactions have not been calculated as there are zero residual degrees of freedom when all terms are included in the statistical model.")
        HTML.title("</bf> ", HR = 2, align = "left")
        HTML.title(add, HR = 0, align = "left")
    } else {
        tempx <- Anova(Covintfull, type = c("III"))[-1,]

        if (tempx[dim(tempx)[1], 1] != 0) {
            temp2x <- (tempx)

            col1x <- format(round(temp2x[1], 2), nsmall = 2, scientific = FALSE)
            col2x <- format(round(temp2x[1] / temp2x[2], 3), nsmall = 3, scientific = FALSE)
            col3x <- format(round(temp2x[3], 2), nsmall = 2, scientific = FALSE)
            col4x <- format(round(temp2x[4], 4), nsmall = 4, scientific = FALSE)

            blanqx <- c(" ")
            for (i in 1:dim(tempx)[1]) {
                blanqx[i] = " "
            }
            ivsanovax <- cbind(col1x, blanqx, temp2x[2], blanqx, col2x, blanqx, col3x, blanqx, col4x)

            source2x <- rownames(ivsanovax)
            #STB March 2014 - Replacing : with * in ANOVA table
            for (q in 1:notreatfactors) {
                source2x <- sub(":", " * ", source2x)
            }
            rownames(ivsanovax) <- source2x
            source3x <- rownames(ivsanovax)

            ivsanovax[length(unique(source2x)), 7] <- " "
            ivsanovax[length(unique(source2x)), 9] <- " "
            #STB May 2012 capitals changed
            headx <- c("Sums of squares", " ", "Degrees of freedom", " ", "Mean square", " ", "F-value", " ", "p-value")
            colnames(ivsanovax) <- headx

            for (i in 1:(dim(ivsanovax)[1] - 1)) {
                if (temp2x[i, 4] < 0.0001) {
                    #STB March 2011 formatting p-values p<0.0001
                    #			ivsanovax[i,9]<-0.0001
                    ivsanovax[i, 9] = format(round(0.0001, 4), nsmall = 4, scientific = FALSE)
                    ivsanovax[i, 9] <- paste("<", ivsanovax[i, 9])
                }
            }

            HTML(ivsanovax, classfirstline = "second", align = "left")

            add <- c("Note: This table should only be used to assess the covariate interactions. The statistical model used to generate all the remaining results in this output does not include the covariate interactions.")
            HTML.title("</bf> ", HR = 2, align = "left")
            HTML.title(add, HR = 0, align = "left")
        }
    }
}
#============================================================================================================================
#ANOVA table
#============================================================================================================================
#Testing the degrees of freedom

if (df.residual(threewayfull) < 5) {
    HTMLbr()
    HTML.title("<bf>Warning", HR = 2, align = "left")
    add <- c("Unfortunately the residual degrees of freedom are low (less than 5). This may make the estimation of the underlying variability, and hence the results of the statistical tests, unreliable. This can be caused by attempting to fit too many factors, and their interactions, in the statistical model. Where appropriate we recommend you fit some of the 'Treatment' factors as 'Other design' factors. This will remove their interactions from the statistical model and therefore increase the residual degrees of freedom.")
    HTML.title("</bf> ", HR = 2, align = "left")
    HTML.title(add, HR = 0, align = "left")
}

#ANOVA Table
if (showANOVA == "Y") {

    if (FirstCatFactor != "NULL") {
        HTMLbr()
        HTML.title("<bf>Analysis of Covariance (ANCOVA) table", HR = 2, align = "left")
    } else {
        HTMLbr()
        HTML.title("<bf>Analysis of variance (ANOVA) table", HR = 2, align = "left")
    }

    #STB Sept 2014 - Marginal sums of square to tie in with RM (also message below and covariate ANOVA above)	
    temp <- Anova(threewayfull, type = c("III"))[-1,]
    #	temp<-Anova(threewayfull)

    temp2 <- (temp)

    col1 <- format(round(temp2[1], 2), nsmall = 2, scientific = FALSE)
    col2 <- format(round(temp2[1] / temp2[2], 3), nsmall = 3, scientific = FALSE)
    col3 <- format(round(temp2[3], 2), nsmall = 2, scientific = FALSE)
    col4 <- format(round(temp2[4], 4), nsmall = 4, scientific = FALSE)

    blanq <- c(" ")
    for (i in 1:dim(temp)[1]) {
        blanq[i] = " "
    }

    ivsanova <- cbind(col1, blanq, temp2[2], blanq, col2, blanq, col3, blanq, col4)

    source2 <- rownames(ivsanova)
    #STB March 2014 - Replacing : with * in ANOVA table
    for (q in 1:notreatfactors) {
        source2 <- sub(":", " * ", source2)
    }
    rownames(ivsanova) <- source2
    source3 <- rownames(ivsanova)

    ivsanova[length(unique(source2)), 7] <- " "
    ivsanova[length(unique(source2)), 9] <- " "
    #STB May 2012 capitals changed
    head <- c("Sums of squares", " ", "Degrees of freedom", " ", "Mean square", " ", "F-value", " ", "p-value")
    colnames(ivsanova) <- head


    for (i in 1:(dim(ivsanova)[1] - 1)) {
        if (temp2[i, 4] < 0.0001) {
            #STB March 2011 formatting p-values p<0.0001
            #			ivsanova[i,9]<-0.0001
            ivsanova[i, 9] = format(round(0.0001, 4), nsmall = 4, scientific = FALSE)
            ivsanova[i, 9] <- paste("<", ivsanova[i, 9])
        }
    }

    HTML(ivsanova, classfirstline = "second", align = "left")
    HTML.title("</bf> ", HR = 2, align = "left")
    if (FirstCatFactor != "NULL") {
        #STB Error spotted:
        #	HTML.title("<sTitle<-sub("ivs_colon_ivs"	,":"ML.title("<bf>Comment: ANCOVA table calculated using a Type III model fit, see Armitage et al. (2001).", HR=0, align="left")
        HTML.title("Comment: ANCOVA table calculated using a Type III model fit, see Armitage et al. (2001).", HR = 0, align = "left")
    } else {
        HTML.title("</bf> ", HR = 2, align = "left")
        HTML.title("<bf>Comment: ANOVA table calculated using a Type III model fit, see Armitage et al. (2001).", HR = 0, align = "left")
    }

    add <- paste(c("Conclusion"))
    inte <- 1
    for (i in 1:(dim(ivsanova)[1] - 1)) {
        if (ivsanova[i, 9] <= (1 - sig)) {
            if (inte == 1) {
                inte <- inte + 1
                add <- paste(add, ": There is a statistically significant overall difference between the levels of ", sep = "")
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

            if (FirstCatFactor != "NULL") {
                #STB July 2013 change wording to remove effects
                add <- paste(add, ": There are no statistically significant overall differences between the levels of any of the terms in the ANCOVA table.", sep = "")
            } else {
                add <- paste(add, ": There are no statistically significant overall differences between the levels of any of the terms in the ANOVA table.", sep = "")
            }
        } else {
            add <- paste(add, ": There is no statistically significant overall difference between the levels of the treatment factor.", sep = "")
        }
    } else {
        add <- paste(add, ". ", sep = "")
    }

    HTML.title("</bf> ", HR = 2, align = "left")
    HTML.title(add, HR = 0, align = "left")
    HTML.title("</bf> ", HR = 2, align = "left")
    if (FirstCatFactor != "NULL") {
        HTML.title("<bf> Tip: While it is a good idea to consider the overall tests in the ANCOVA table, we should not rely on them when deciding whether or not to make pairwise comparisons.", HR = 0, align = "left")
    } else {
        HTML.title("<bf> Tip: While it is a good idea to consider the overall tests in the ANOVA table, we should not rely on them when deciding whether or not to make pairwise comparisons.", HR = 0, align = "left")
    }
}

#============================================================================================================================
#Covariate correlation table
#============================================================================================================================

if (CovariateRegressionCoefficients == "Y" && FirstCatFactor != "NULL") {
    HTMLbr()
    HTML.title("<bf>Covariate regression coefficient", HR = 2, align = "left")

    covtable_1 <- coef(summary(threewayfull))
    covtable <- data.frame(covtable_1)[c(2),]

    covtable_2 <- covtable

    covtable$Estimate <- format(round(covtable$Estimate, 3), nsmall = 3, scientific = FALSE)
    covtable$Std..Error <- format(round(covtable$Std..Error, 3), nsmall = 3, scientific = FALSE)
    covtable$t.value <- format(round(covtable$t.value, 2), nsmall = 2, scientific = FALSE)
    covtable$Pr...t.. <- format(round(covtable$Pr...t.., 4), nsmall = 4, scientific = FALSE)

    covtable_1 <- covtable

    if (as.numeric(covtable_2[1, 4]) < 0.0001) {
        #STB March 2011 formatting p-values p<0.0001
        #		ivsanova[i,9]<-0.0001
        covtable_1[1, 4] = "<0.0001"
    }

    rz <- rownames(covtable)[1]
    rownames(covtable_1) <- c(rz)

    colnames(covtable_1) <- c("Estimate", "Std error", "t-value", "p-value")
    HTML(covtable_1, classfirstline = "second", align = "left")
}





#============================================================================================================================
#Diagnostic plots
#============================================================================================================================

#Diagnostic plot titles
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

    #STB July2013
    plotFilepdf3 <- sub(".html", "residualplot.pdf", htmlFile)
    dev.control("enable")

    #==================================================================================================================================================
    #==================================================================================================================================================
    #Graphical parameters
    graphdata <- data.frame(cbind(predict(threewayfull), rstudent(threewayfull)))

    graphdata$yvarrr_IVS <- graphdata$X2
    graphdata$xvarrr_IVS <- graphdata$X1
    XAxisTitle <- "Predicted values"
    YAxisTitle <- "Externally Studentised residuals"
    MainTitle2 <- "Residuals vs. predicted plot \n"
    w_Gr_jit <- 0
    h_Gr_jit <- 0
    infiniteslope <- "Y"

    #if (bandw != "N") 
    #{
    #	Gr_line <-BW_line
    #	Gr_fill <- BW_fill
    #} else {
    #	Gr_line <-Col_line
    #	Gr_fill <- Col_fill
    #	}
    Gr_line_type <- Line_type_dashed

    Line_size2 <- Line_size
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
    HTML.title("<bf>Tip: Any observation with a residual less than -3 or greater than 3 (SD) should be investigated as a possible outlier.", HR = 0, align = "left")
}

#Normality plots
if (showNormPlot == "Y") {
    HTMLbr()
    normPlot <- sub(".html", "normplot.jpg", htmlFile)
    jpeg(normPlot, width = jpegwidth, height = jpegheight, quality = 100)

    #STB July2013
    plotFilepdf4 <- sub(".html", "normplot.pdf", htmlFile)
    dev.control("enable")


    #==================================================================================================================================================
    #==================================================================================================================================================
    #Graphical parameters

    te <- qqnorm(resid(threewayfull))
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

    #	if (bandw != "N") 
    #	{
    #		Gr_line <-BW_line
    #		Gr_fill <- BW_fill
    #	} else {
    #		Gr_line <-Col_line
    #		Gr_fill <- Col_fill
    #		}
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

#============================================================================================================================
#LS Means plot and table
#============================================================================================================================
if (showLSMeans == "Y") {
    HTMLbr()
    HTMLbr()
    #STB May 2012 Updating "least square (predicted) means"
    CITitle <- paste("<bf>Plot of the least square (predicted) means with ", (sig * 100), "% confidence intervals", sep = "")
    HTML.title(CITitle, HR = 2, align = "left")

    #============================================================================================================================
    #LSMeans plot
    #============================================================================================================================

    #Calculate LS Means dataset
    tabs <- lsmeans(threewayfull, eval(parse(text = paste("~", selectedEffect))), data = statdata)
    x <- summary(tabs)

    x$Mean <- x$lsmean
    for (i in 1:dim(x)[1]) {
        x$Lower[i] <- x$lsmean[i] - x$SE[i] * qt(sig2, x$df[i])
        x$Upper[i] <- x$lsmean[i] + x$SE[i] * qt(sig2, x$df[i])
    }

    graphdata <- subset(x, select = -c(SE, df, lsmean, lower.CL, upper.CL))

    names <- c()
    for (l in 1:factno) {
        names[l] <- paste(unique(strsplit(selectedEffect, "*", fixed = TRUE)[[1]])[l], " Level", sep = "")
    }
    names[factno + 1] <- "Mean"
    names[factno + 2] <- "Lower"
    names[factno + 3] <- "Upper"

    colnames(graphdata) <- names

    #Calculating dataset for plotting - including a Group factor for higher order interactions
    graphdata$Group_IVSq_ <- graphdata[, 1]

    if (factno > 1) {
        for (y in 2:factno) {
            graphdata$Group_IVSq_ <- paste(graphdata$Group_IVSq_, ", ", graphdata[, y], sep = "")
        }
    }

    #other parameters for the plot
    Gr_alpha <- 0
    if (bandw != "N") {
        Gr_fill <- BW_fill
    } else {
        Gr_fill <- Col_fill
    }
    YAxisTitle <- LS_YAxisTitle
    XAxisTitle <- "Group"
    MainTitle2 <- ""
    #Gr_line <-"black"
    Line_size <- Line_size2

    #Code for LS MEans plot
    meanPlot <- sub(".html", "meanplot.jpg", htmlFile)
    jpeg(meanPlot, width = jpegwidth, height = jpegheight, quality = 100)

    #STB July2013
    plotFilepdf5 <- sub(".html", "meanplot.pdf", htmlFile)
    dev.control("enable")

    #GGPLOT2 code

    if (factno == 1 || factno > 4) {
        XAxisTitle <- unique(strsplit(selectedEffectx, "*", fixed = TRUE)[[1]])[1]
        LSMPLOT_1()
    }

    if (factno == 2) {
        if (length(unique(levels(graphdata[, 1]))) > length(unique(levels(graphdata[, 2])))) {
            XAxisTitle <- unique(strsplit(selectedEffectx, "*", fixed = TRUE)[[1]])[1]
            graphdata$jj_1 <- graphdata[, 1]
            legendz <- unique(strsplit(selectedEffectx, "*", fixed = TRUE)[[1]])[2]
            graphdata$catzz <- legendz
            graphdata$jj_2 <- paste(graphdata$catzz, "= ", graphdata[, 2], sep = "")
        } else {
            XAxisTitle <- unique(strsplit(selectedEffectx, "*", fixed = TRUE)[[1]])[2]
            graphdata$jj_1 <- graphdata[, 2]
            legendz <- unique(strsplit(selectedEffectx, "*", fixed = TRUE)[[1]])[1]
            graphdata$catzz <- legendz
            graphdata$jj_2 <- paste(graphdata$catzz, "= ", graphdata[, 1], sep = "")
        }
        Gr_palette <- palette_FUN("jj_2")
        LSMPLOT_2("none")
    }

    if (factno == 3) {
        if (length(unique(levels(graphdata[, 1]))) > length(unique(levels(graphdata[, 2]))) && length(unique(levels(graphdata[, 1]))) > length(unique(levels(graphdata[, 3])))) {
            XAxisTitle <- unique(strsplit(selectedEffectx, "*", fixed = TRUE)[[1]])[1]
            graphdata$jj_1 <- graphdata[, 1]

            legendz <- unique(strsplit(selectedEffectx, "*", fixed = TRUE)[[1]])[2]
            graphdata$catzz <- legendz
            graphdata$jj_2 <- paste(graphdata$catzz, "= ", graphdata[, 2], sep = "")

            legendz <- unique(strsplit(selectedEffectx, "*", fixed = TRUE)[[1]])[3]
            graphdata$catzz <- legendz
            graphdata$jj_3 <- paste(graphdata$catzz, "= ", graphdata[, 3], sep = "")
        } else if (length(unique(levels(graphdata[, 2]))) > length(unique(levels(graphdata[, 1]))) && length(unique(levels(graphdata[, 2]))) > length(unique(levels(graphdata[, 3])))) {
            XAxisTitle <- unique(strsplit(selectedEffectx, "*", fixed = TRUE)[[1]])[2]
            graphdata$jj_1 <- graphdata[, 2]

            legendz <- unique(strsplit(selectedEffectx, "*", fixed = TRUE)[[1]])[1]
            graphdata$catzz <- legendz
            graphdata$jj_2 <- paste(graphdata$catzz, "= ", graphdata[, 1], sep = "")

            legendz <- unique(strsplit(selectedEffectx, "*", fixed = TRUE)[[1]])[3]
            graphdata$catzz <- legendz
            graphdata$jj_3 <- paste(graphdata$catzz, "= ", graphdata[, 3], sep = "")
        } else {
            XAxisTitle <- unique(strsplit(selectedEffectx, "*", fixed = TRUE)[[1]])[3]
            graphdata$jj_1 <- graphdata[, 3]

            legendz <- unique(strsplit(selectedEffectx, "*", fixed = TRUE)[[1]])[1]
            graphdata$catzz <- legendz
            graphdata$jj_2 <- paste(graphdata$catzz, "= ", graphdata[, 1], sep = "")

            legendz <- unique(strsplit(selectedEffectx, "*", fixed = TRUE)[[1]])[2]
            graphdata$catzz <- legendz
            graphdata$jj_3 <- paste(graphdata$catzz, "= ", graphdata[, 2], sep = "")
        }

        Gr_palette <- palette_FUN("jj_2")
        LSMPLOT_2("three")
    }

    if (factno == 4) {
        if (length(unique(levels(graphdata[, 1]))) > length(unique(levels(graphdata[, 2]))) && length(unique(levels(graphdata[, 1]))) > length(unique(levels(graphdata[, 3]))) && length(unique(levels(graphdata[, 1]))) > length(unique(levels(graphdata[, 4])))) {
            XAxisTitle <- unique(strsplit(selectedEffectx, "*", fixed = TRUE)[[1]])[1]
            graphdata$jj_1 <- graphdata[, 1]

            legendz <- unique(strsplit(selectedEffectx, "*", fixed = TRUE)[[1]])[2]
            graphdata$catzz <- legendz
            graphdata$jj_2 <- paste(graphdata$catzz, "= ", graphdata[, 2], sep = "")

            legendz <- unique(strsplit(selectedEffectx, "*", fixed = TRUE)[[1]])[3]
            graphdata$catzz <- legendz
            graphdata$jj_3 <- paste(graphdata$catzz, "= ", graphdata[, 3], sep = "")

            legendz <- unique(strsplit(selectedEffectx, "*", fixed = TRUE)[[1]])[4]
            graphdata$catzz <- legendz
            graphdata$jj_4 <- paste(graphdata$catzz, "= ", graphdata[, 4], sep = "")
        } else if (length(unique(levels(graphdata[, 2]))) > length(unique(levels(graphdata[, 1]))) && length(unique(levels(graphdata[, 2]))) > length(unique(levels(graphdata[, 3]))) && length(unique(levels(graphdata[, 2]))) > length(unique(levels(graphdata[, 4])))) {
            XAxisTitle <- unique(strsplit(selectedEffectx, "*", fixed = TRUE)[[1]])[2]
            graphdata$jj_1 <- graphdata[, 2]

            legendz <- unique(strsplit(selectedEffectx, "*", fixed = TRUE)[[1]])[1]
            graphdata$catzz <- legendz
            graphdata$jj_2 <- paste(graphdata$catzz, "= ", graphdata[, 1], sep = "")

            legendz <- unique(strsplit(selectedEffectx, "*", fixed = TRUE)[[1]])[3]
            graphdata$catzz <- legendz
            graphdata$jj_3 <- paste(graphdata$catzz, "= ", graphdata[, 3], sep = "")

            legendz <- unique(strsplit(selectedEffectx, "*", fixed = TRUE)[[1]])[4]
            graphdata$catzz <- legendz
            graphdata$jj_4 <- paste(graphdata$catzz, "= ", graphdata[, 4], sep = "")
        } else if (length(unique(levels(graphdata[, 3]))) > length(unique(levels(graphdata[, 1]))) && length(unique(levels(graphdata[, 3]))) > length(unique(levels(graphdata[, 2]))) && length(unique(levels(graphdata[, 3]))) > length(unique(levels(graphdata[, 4])))) {
            XAxisTitle <- unique(strsplit(selectedEffectx, "*", fixed = TRUE)[[1]])[3]
            graphdata$jj_1 <- graphdata[, 3]

            legendz <- unique(strsplit(selectedEffectx, "*", fixed = TRUE)[[1]])[1]
            graphdata$catzz <- legendz
            graphdata$jj_2 <- paste(graphdata$catzz, "= ", graphdata[, 1], sep = "")

            legendz <- unique(strsplit(selectedEffectx, "*", fixed = TRUE)[[1]])[2]
            graphdata$catzz <- legendz
            graphdata$jj_3 <- paste(graphdata$catzz, "= ", graphdata[, 2], sep = "")

            legendz <- unique(strsplit(selectedEffectx, "*", fixed = TRUE)[[1]])[4]
            graphdata$catzz <- legendz
            graphdata$jj_4 <- paste(graphdata$catzz, "= ", graphdata[, 4], sep = "")
        } else {
            XAxisTitle <- unique(strsplit(selectedEffectx, "*", fixed = TRUE)[[1]])[4]
            graphdata$jj_1 <- graphdata[, 4]

            legendz <- unique(strsplit(selectedEffectx, "*", fixed = TRUE)[[1]])[1]
            graphdata$catzz <- legendz
            graphdata$jj_2 <- paste(graphdata$catzz, "= ", graphdata[, 1], sep = "")

            legendz <- unique(strsplit(selectedEffectx, "*", fixed = TRUE)[[1]])[2]
            graphdata$catzz <- legendz
            graphdata$jj_3 <- paste(graphdata$catzz, "= ", graphdata[, 2], sep = "")

            legendz <- unique(strsplit(selectedEffectx, "*", fixed = TRUE)[[1]])[3]
            graphdata$catzz <- legendz
            graphdata$jj_4 <- paste(graphdata$catzz, "= ", graphdata[, 3], sep = "")
        }


        Gr_palette <- palette_FUN("jj_2")
        LSMPLOT_2("four")
    }

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
        linkToPdf5 <- paste("<a href=\"", pdfFile_5, "\">Click here to view the PDF of the plot of least square (predicted) means</a>", sep = "")
        HTML(linkToPdf5)
    }

    #============================================================================================================================
    #Table of Least Square means
    #============================================================================================================================
    #STB May 2012 Updating "least square (predicted) means"

    CITitle2 <- paste("<bf>Table of the least square (predicted) means with ", (sig * 100), "% confidence intervals", sep = "")
    HTML.title(CITitle2, HR = 2, align = "left")

    #Calculate LS Means Table
    x <- summary(tabs)

    x$Mean <- format(round(x$lsmean, 3), nsmall = 3, scientific = FALSE)
    for (i in 1:dim(x)[1]) {
        x$Lower[i] <- format(round(x$lsmean[i] - x$SE[i] * qt(sig2, x$df[i]), 3), nsmall = 3, scientific = FALSE)
        x$Upper[i] <- format(round(x$lsmean[i] + x$SE[i] * qt(sig2, x$df[i]), 3), nsmall = 3, scientific = FALSE)
    }

    x2 <- subset(x, select = -c(SE, df, lsmean, lower.CL, upper.CL))

    blanq <- c(" ")
    for (i in 1:dim(x2)[2]) {
        blanq[i] = " "
    }

    x3 <- suppressWarnings(rbind(blanq, x2))

    names <- c()
    for (l in 1:factno) {
        names[l] <- paste(unique(strsplit(selectedEffect, "*", fixed = TRUE)[[1]])[l], " ", sep = "")
    }
    names[factno + 1] <- "Mean"
    names[factno + 2] <- paste("Lower ", (sig * 100), "% CI", sep = "")
    names[factno + 3] <- paste("Upper ", (sig * 100), "% CI", sep = "")

    colnames(x3) <- names
    rownames(x3) <- c("ID", 1:(dim(x3)[1] - 1))

    HTML(x3, classfirstline = "second", align = "left")

}




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

    #============================================================================================================================
    #LSMeans plot
    #============================================================================================================================

    #Calculate LS Means dataset
    tabs <- lsmeans(threewayfull, eval(parse(text = paste("~", selectedEffect))), data = statdata)
    x <- summary(tabs)

    if (responseTransform == "Log10") {
        x$Mean <- 10 ^ (x$lsmean)
        for (i in 1:dim(x)[1]) {
            x$Lower[i] <- 10 ^ (x$lsmean[i] - x$SE[i] * qt(sig2, x$df[i]))
            x$Upper[i] <- 10 ^ (x$lsmean[i] + x$SE[i] * qt(sig2, x$df[i]))
        }

    }

    if (responseTransform == "Loge") {
        x$Mean <- exp(x$lsmean)
        for (i in 1:dim(x)[1]) {
            x$Lower[i] <- exp(x$lsmean[i] - x$SE[i] * qt(sig2, x$df[i]))
            x$Upper[i] <- exp(x$lsmean[i] + x$SE[i] * qt(sig2, x$df[i]))
        }
    }

    graphdata <- subset(x, select = -c(SE, df, lsmean, lower.CL, upper.CL))

    names <- c()
    for (l in 1:factno) {
        names[l] <- paste(unique(strsplit(selectedEffect, "*", fixed = TRUE)[[1]])[l], " Level", sep = "")
    }
    names[factno + 1] <- "Mean"
    names[factno + 2] <- "Lower"
    names[factno + 3] <- "Upper"

    colnames(graphdata) <- names

    #Calculating dataset for plotting - including a Group factor for higher order interactions
    graphdata$Group_IVSq_ <- graphdata[, 1]

    if (factno > 1) {
        for (y in 2:factno) {
            graphdata$Group_IVSq_ <- paste(graphdata$Group_IVSq_, ", ", graphdata[, y], sep = "")
        }
    }

    #other parameters for the plot
    Gr_alpha <- 0
    if (bandw != "N") {
        Gr_fill <- BW_fill
    } else {
        Gr_fill <- Col_fill
    }
    YAxisTitle <- LS_YAxisTitle
    XAxisTitle <- "Group"
    MainTitle2 <- ""
    #Gr_line <-"black"
    Line_size <- Line_size2

    #Code for LS MEans plot
    meanPlotq <- sub(".html", "meanplotq.jpg", htmlFile)
    jpeg(meanPlotq, width = jpegwidth, height = jpegheight, quality = 100)

    #STB July2013
    plotFilepdf5q <- sub(".html", "meanplotq.pdf", htmlFile)
    dev.control("enable")

    #GGPLOT2 code

    if (factno == 1 || factno > 4) {
        XAxisTitle <- unique(strsplit(selectedEffectx, "*", fixed = TRUE)[[1]])[1]
        LSMPLOT_1()
    }

    if (factno == 2) {
        if (length(unique(levels(graphdata[, 1]))) > length(unique(levels(graphdata[, 2])))) {
            XAxisTitle <- unique(strsplit(selectedEffectx, "*", fixed = TRUE)[[1]])[1]
            graphdata$jj_1 <- graphdata[, 1]
            legendz <- unique(strsplit(selectedEffectx, "*", fixed = TRUE)[[1]])[2]
            graphdata$catzz <- legendz
            graphdata$jj_2 <- paste(graphdata$catzz, "=", graphdata[, 2], sep = "")
        } else {
            XAxisTitle <- unique(strsplit(selectedEffectx, "*", fixed = TRUE)[[1]])[2]
            graphdata$jj_1 <- graphdata[, 2]
            legendz <- unique(strsplit(selectedEffectx, "*", fixed = TRUE)[[1]])[1]
            graphdata$catzz <- legendz
            graphdata$jj_2 <- paste(graphdata$catzz, "=", graphdata[, 1], sep = "")
        }
        Gr_palette <- palette_FUN("jj_2")
        LSMPLOT_2("none")
    }

    if (factno == 3) {
        if (length(unique(levels(graphdata[, 1]))) > length(unique(levels(graphdata[, 2]))) && length(unique(levels(graphdata[, 1]))) > length(unique(levels(graphdata[, 3])))) {
            XAxisTitle <- unique(strsplit(selectedEffectx, "*", fixed = TRUE)[[1]])[1]
            graphdata$jj_1 <- graphdata[, 1]

            legendz <- unique(strsplit(selectedEffectx, "*", fixed = TRUE)[[1]])[2]
            graphdata$catzz <- legendz
            graphdata$jj_2 <- paste(graphdata$catzz, "=", graphdata[, 2], sep = "")

            legendz <- unique(strsplit(selectedEffectx, "*", fixed = TRUE)[[1]])[3]
            graphdata$catzz <- legendz
            graphdata$jj_3 <- paste(graphdata$catzz, "=", graphdata[, 3], sep = "")
        } else if (length(unique(levels(graphdata[, 2]))) > length(unique(levels(graphdata[, 1]))) && length(unique(levels(graphdata[, 2]))) > length(unique(levels(graphdata[, 3])))) {
            XAxisTitle <- unique(strsplit(selectedEffectx, "*", fixed = TRUE)[[1]])[2]
            graphdata$jj_1 <- graphdata[, 2]

            legendz <- unique(strsplit(selectedEffectx, "*", fixed = TRUE)[[1]])[1]
            graphdata$catzz <- legendz
            graphdata$jj_2 <- paste(graphdata$catzz, "=", graphdata[, 1], sep = "")

            legendz <- unique(strsplit(selectedEffectx, "*", fixed = TRUE)[[1]])[3]
            graphdata$catzz <- legendz
            graphdata$jj_3 <- paste(graphdata$catzz, "=", graphdata[, 3], sep = "")
        } else {
            XAxisTitle <- unique(strsplit(selectedEffectx, "*", fixed = TRUE)[[1]])[3]
            graphdata$jj_1 <- graphdata[, 3]

            legendz <- unique(strsplit(selectedEffectx, "*", fixed = TRUE)[[1]])[1]
            graphdata$catzz <- legendz
            graphdata$jj_2 <- paste(graphdata$catzz, "=", graphdata[, 1], sep = "")

            legendz <- unique(strsplit(selectedEffectx, "*", fixed = TRUE)[[1]])[2]
            graphdata$catzz <- legendz
            graphdata$jj_3 <- paste(graphdata$catzz, "=", graphdata[, 2], sep = "")
        }

        Gr_palette <- palette_FUN("jj_2")
        LSMPLOT_2("three")
    }

    if (factno == 4) {
        if (length(unique(levels(graphdata[, 1]))) > length(unique(levels(graphdata[, 2]))) && length(unique(levels(graphdata[, 1]))) > length(unique(levels(graphdata[, 3]))) && length(unique(levels(graphdata[, 1]))) > length(unique(levels(graphdata[, 4])))) {
            XAxisTitle <- unique(strsplit(selectedEffectx, "*", fixed = TRUE)[[1]])[1]
            graphdata$jj_1 <- graphdata[, 1]

            legendz <- unique(strsplit(selectedEffectx, "*", fixed = TRUE)[[1]])[2]
            graphdata$catzz <- legendz
            graphdata$jj_2 <- paste(graphdata$catzz, "=", graphdata[, 2], sep = "")

            legendz <- unique(strsplit(selectedEffectx, "*", fixed = TRUE)[[1]])[3]
            graphdata$catzz <- legendz
            graphdata$jj_3 <- paste(graphdata$catzz, "=", graphdata[, 3], sep = "")

            legendz <- unique(strsplit(selectedEffectx, "*", fixed = TRUE)[[1]])[4]
            graphdata$catzz <- legendz
            graphdata$jj_4 <- paste(graphdata$catzz, "=", graphdata[, 4], sep = "")
        } else if (length(unique(levels(graphdata[, 2]))) > length(unique(levels(graphdata[, 1]))) && length(unique(levels(graphdata[, 2]))) > length(unique(levels(graphdata[, 3]))) && length(unique(levels(graphdata[, 2]))) > length(unique(levels(graphdata[, 4])))) {
            XAxisTitle <- unique(strsplit(selectedEffectx, "*", fixed = TRUE)[[1]])[2]
            graphdata$jj_1 <- graphdata[, 2]

            legendz <- unique(strsplit(selectedEffectx, "*", fixed = TRUE)[[1]])[1]
            graphdata$catzz <- legendz
            graphdata$jj_2 <- paste(graphdata$catzz, "=", graphdata[, 1], sep = "")

            legendz <- unique(strsplit(selectedEffectx, "*", fixed = TRUE)[[1]])[3]
            graphdata$catzz <- legendz
            graphdata$jj_3 <- paste(graphdata$catzz, "=", graphdata[, 3], sep = "")

            legendz <- unique(strsplit(selectedEffectx, "*", fixed = TRUE)[[1]])[4]
            graphdata$catzz <- legendz
            graphdata$jj_4 <- paste(graphdata$catzz, "=", graphdata[, 4], sep = "")
        } else if (length(unique(levels(graphdata[, 3]))) > length(unique(levels(graphdata[, 1]))) && length(unique(levels(graphdata[, 3]))) > length(unique(levels(graphdata[, 2]))) && length(unique(levels(graphdata[, 3]))) > length(unique(levels(graphdata[, 4])))) {
            XAxisTitle <- unique(strsplit(selectedEffectx, "*", fixed = TRUE)[[1]])[3]
            graphdata$jj_1 <- graphdata[, 3]

            legendz <- unique(strsplit(selectedEffectx, "*", fixed = TRUE)[[1]])[1]
            graphdata$catzz <- legendz
            graphdata$jj_2 <- paste(graphdata$catzz, "=", graphdata[, 1], sep = "")

            legendz <- unique(strsplit(selectedEffectx, "*", fixed = TRUE)[[1]])[2]
            graphdata$catzz <- legendz
            graphdata$jj_3 <- paste(graphdata$catzz, "=", graphdata[, 2], sep = "")

            legendz <- unique(strsplit(selectedEffectx, "*", fixed = TRUE)[[1]])[4]
            graphdata$catzz <- legendz
            graphdata$jj_4 <- paste(graphdata$catzz, "=", graphdata[, 4], sep = "")
        } else {
            XAxisTitle <- unique(strsplit(selectedEffectx, "*", fixed = TRUE)[[1]])[4]
            graphdata$jj_1 <- graphdata[, 4]

            legendz <- unique(strsplit(selectedEffectx, "*", fixed = TRUE)[[1]])[1]
            graphdata$catzz <- legendz
            graphdata$jj_2 <- paste(graphdata$catzz, "=", graphdata[, 1], sep = "")

            legendz <- unique(strsplit(selectedEffectx, "*", fixed = TRUE)[[1]])[2]
            graphdata$catzz <- legendz
            graphdata$jj_3 <- paste(graphdata$catzz, "=", graphdata[, 2], sep = "")

            legendz <- unique(strsplit(selectedEffectx, "*", fixed = TRUE)[[1]])[3]
            graphdata$catzz <- legendz
            graphdata$jj_4 <- paste(graphdata$catzz, "=", graphdata[, 3], sep = "")
        }


        Gr_palette <- palette_FUN("jj_2")
        LSMPLOT_2("four")
    }

    #============================================================================================================================
    #============================================================================================================================
    void <- HTMLInsertGraph(GraphFileName = sub("[A-Z0-9a-z,:,\\\\]*App_Data[\\\\]", "", meanPlotq), Align = "left")

    #STB July2013
    if (pdfout == "Y") {
        pdf(file = sub("[A-Z0-9a-z,:,\\\\]*App_Data[\\\\]", "", plotFilepdf5q), height = pdfheight, width = pdfwidth)
        dev.set(2)
        dev.copy(which = 3)
        dev.off(2)
        dev.off(3)
        pdfFile_5q <- sub("[A-Z0-9a-z,:,\\\\]*App_Data[\\\\]", "", plotFilepdf5q)
        linkToPdf5q <- paste("<a href=\"", pdfFile_5q, "\">Click here to view the PDF of the plot of back transformed geometric means</a>", sep = "")
        HTML(linkToPdf5q)
    }

    #============================================================================================================================
    #Table of back transformed means
    #============================================================================================================================
    #STB May 2012 Updating "least square (predicted) means"

    CITitle2 <- paste("<bf>Table of the back-transformed geometric means with ", (sig * 100), "% confidence intervals", sep = "")
    HTML.title(CITitle2, HR = 2, align = "left")

    #Calculate LS Means Table
    x <- summary(tabs)

    if (responseTransform == "Log10") {
        x$Mean <- format(round(10 ^ (x$lsmean), 3), nsmall = 3, scientific = FALSE)
        for (i in 1:dim(x)[1]) {
            x$Lower[i] <- format(round(10 ^ (x$lsmean[i] - x$SE[i] * qt(sig2, x$df[i])), 3), nsmall = 3, scientific = FALSE)
            x$Upper[i] <- format(round(10 ^ (x$lsmean[i] + x$SE[i] * qt(sig2, x$df[i])), 3), nsmall = 3, scientific = FALSE)
        }
    }

    if (responseTransform == "Loge") {
        x$Mean <- format(round(exp(x$lsmean), 3), nsmall = 3, scientific = FALSE)
        for (i in 1:dim(x)[1]) {
            x$Lower[i] <- format(round(exp(x$lsmean[i] - x$SE[i] * qt(sig2, x$df[i])), 3), nsmall = 3, scientific = FALSE)
            x$Upper[i] <- format(round(exp(x$lsmean[i] + x$SE[i] * qt(sig2, x$df[i])), 3), nsmall = 3, scientific = FALSE)
        }
    }

    x2 <- subset(x, select = -c(SE, df, lsmean, lower.CL, upper.CL))

    blanq <- c(" ")
    for (i in 1:dim(x2)[2]) {
        blanq[i] = " "
    }

    x3 <- suppressWarnings(rbind(blanq, x2))

    names <- c()
    for (l in 1:factno) {
        names[l] <- paste(unique(strsplit(selectedEffect, "*", fixed = TRUE)[[1]])[l], " ", sep = "")
    }
    names[factno + 1] <- "Geometric mean"
    names[factno + 2] <- paste("Lower ", (sig * 100), "% CI", sep = "")
    names[factno + 3] <- paste("Upper ", (sig * 100), "% CI", sep = "")

    colnames(x3) <- names
    rownames(x3) <- c("ID", 1:(dim(x3)[1] - 1))

    HTML(x3, classfirstline = "second", align = "left")

}




#============================================================================================================================
#All Pairwise tests
#============================================================================================================================
#STB Jun 2015
#Creating dataset without dashes in

ivs_num_ivs <- rep(1:dim(statdata)[1])
ivs_char_ivs <- rep(factor(LETTERS[1:dim(statdata)[1]]), 1)
statdata_temp2 <- cbind(statdata_temp, ivs_num_ivs, ivs_char_ivs)

statdata_num <- statdata_temp2[, sapply(statdata_temp2, is.numeric)]
statdata_char <- statdata_temp2[, !sapply(statdata_temp2, is.numeric)]
statdata_char2 <- as.data.frame(sapply(statdata_char, gsub, pattern = "-", replacement = "_ivs_dash_ivs_"))
statdata <- data.frame(cbind(statdata_num, statdata_char2))



#All pairwise tests

if (allPairwiseTest != "NULL") {

    #All pairwise test options
    allPairwiseTestText = allPairwiseTest
    if (allPairwiseTestText == "Unadjusted (LSD)") {
        allPairwiseTest = "none"
    } else if (allPairwiseTestText == "Holm") {
        allPairwiseTest = "holm"
    } else if (allPairwiseTestText == "Hochberg") {
        allPairwiseTest = "hochberg"
    } else if (allPairwiseTestText == "Hommel") {
        allPairwiseTest = "hommel"
    } else if (allPairwiseTestText == "Bonferonni") {
        allPairwiseTest = "bonferroni"
        #--------------------------------------------------------------------------------------------------
        #--------------------------------------------------------------------------------------------------
        #SilveR Addition
    } else if (allPairwiseTestText == "T-max") {
        allPairwiseTest = "T-max"
        #--------------------------------------------------------------------------------------------------
        #--------------------------------------------------------------------------------------------------
    } else if (allPairwiseTestText == "Benjamini-Hochberg") {
        allPairwiseTest = "BH"
    }



    if (allPairwiseTest == "none") {
        add <- paste(c("All pairwise comparisons without adjustment for multiplicity (LSD test)"))
    } else {
        add <- paste(c("All pairwise comparisons using "))
        add <- paste(add, allPairwiseTestText, sep = "")
        add <- paste(add, "'s procedure", sep = "")
    }

    HTMLbr()
    HTML.title(add, HR = 2, align = "left")

    #--------------------------------------------------------------------------------------------------
    #--------------------------------------------------------------------------------------------------
    #SilveR Addition
    #STB Jan 2011 - T-max addition

    if (allPairwiseTest != "T-max" || allPairwiseTest != "Tukey") {
        #--------------------------------------------------------------------------------------------------
        #--------------------------------------------------------------------------------------------------
        #	#	mult<-glht(lm(effectModel, data=statdata, na.action = na.omit), linfct=mcp(mainEffect="Tukey"))
        mult <- glht(lm(model, data = statdata, na.action = na.omit), linfct = lsm(eval(parse(text = paste("pairwise ~", selectedEffect)))))
        multci <- confint(mult, level = sig, calpha = univariate_calpha())
        #--------------------------------------------------------------------------------------------------
        #--------------------------------------------------------------------------------------------------
        #SilveR Addition

        if (allPairwiseTest == "T-max") {
            mult <- glht(lm(effectModel, data = statdata, na.action = na.omit), linfct = mcp(mainEffect = "Tukey"), vcov = vcovHC)
            multci <- confint(mult, level = sig, calpha = univariate_calpha())
            multp <- summary(mult)
        }
        #--------------------------------------------------------------------------------------------------
        #--------------------------------------------------------------------------------------------------



        if (allPairwiseTest == "Tukey") {
            set.seed(3)
            #		mult<-glht(lm(effectModel, data=statdata, na.action = na.omit), linfct=mcp(mainEffect="Tukey"))
            mult <- glht(lm(model, data = statdata, na.action = na.omit), linfct = lsm(eval(parse(text = paste("pairwise ~", selectedEffect)))))
            multci <- confint(mult, level = sig, calpha = univariate_calpha())
            multp <- summary(mult)
        } else {
            multp <- summary(mult, test = adjusted(allPairwiseTest))
        }

        #--------------------------------------------------------------------------------------------------
        #--------------------------------------------------------------------------------------------------
        #SilveR Addition
    }

    pvals <- multp$test$pvalues
    sigma <- multp$test$sigma
    tablen <- length(unique(rownames(multci$confint)))
    tabs <- matrix(nrow = tablen, ncol = 5)

    for (i in 1:tablen) {
        #STB Dec 2011 increasing means to 3dp
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
        if (pvals[i] < 0.0001) {
            #STB March 2011 - formatting p-values p<0.0001
            #			tabs[i,5]<-0.0001
            tabs[i, 5] = format(round(0.0001, 4), nsmall = 4, scientific = FALSE)
            tabs[i, 5] <- paste("<", tabs[i, 5])
        }
    }

    header <- c(" ", " ", " ", " ", " ")

    rows <- rownames(multci$confint)
    rows <- sub(" - ", " vs. ", rows, fixed = TRUE)

    #STB June 2015	
    for (i in 1:100) {
        rows <- sub("_ivs_dash_ivs_", " - ", rows, fixed = TRUE)
    }

    lowerCI <- paste("   Lower ", (sig * 100), "% CI   ", sep = "")
    upperCI <- paste("   Upper ", (sig * 100), "% CI   ", sep = "")

    tabls <- rbind(header, tabs)
    rownames(tabls) <- c("Comparison", rows)
    colnames(tabls) <- c("   Difference   ", lowerCI, upperCI, "   Std error   ", "   p-value   ")
    HTML(tabls, classfirstline = "second", align = "left")


    #===================================================================================================================
    #STB March 2014 - Creating a dataset of p-values

    comparisons <- paste(direct, "/Comparisons.csv", sep = "")
    for (i in 1:tablen) {
        tabs[i, 5] = pvals[i]
    }
    tabsxx <- data.frame(tabs[, 5])
    for (i in 1:20) {
        rows <- sub(",", " and ", rows, fixed = TRUE)
    }
    tabsxx <- cbind(rows, tabsxx)
    colnames(tabsxx) <- c("Comparison", "p-value")
    row.names(tabsxx) <- seq(nrow(tabsxx))

    #===================================================================================================================
    #Conclusion
    add <- paste(c("Conclusion"))
    inte <- 1
    for (i in 2:(dim(tabls)[1])) {
        if (tabls[i, 5] <= (1 - sig)) {
            if (inte == 1) {
                inte <- inte + 1
                add <- paste(add, ": The following pairwise tests are statistically significantly different at the  ", sep = "")
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
    if (inte == 1) {
        if (tablen > 1) {
            add <- paste(add, ": There are no statistically significant pairwise differences.", sep = "")
        } else {
            add <- paste(add, ": The pairwise difference is not statistically significant.", sep = "")
        }
    } else {
        add <- paste(add, ". ", sep = "")
    }

    HTML.title("</bf> ", HR = 2, align = "left")
    HTML.title(add, HR = 0, align = "left")

    #===================================================================================================================
    if (allPairwiseTest == "Tukey") {
        HTML.title("<bf> ", HR = 2, align = "left")
        HTML.title("<bf>Warning: The results of Tukey's procedure are approximate if the sample sizes are not equal.", HR = 0, align = "left")
    }

    if (length(grep("\\*", effectModel)) == 0 && length(grep("\\+", effectModel)) == 0 && length(grep("\\+", model)) == 1) {
        add2 <- paste(c(" "), " ", sep = "")

        HTML.title("<bf> ", HR = 2, align = "left")
        HTML.title(add2, HR = 0, align = "left")

    } else if (length(grep("\\*", model)) == 0 && length(grep("\\+", effectModel)) == 0 && length(grep("\\+", model)) == 0) {
        add2 <- paste(c(" "), " ", sep = "")
        HTML.title("<bf> ", HR = 2, align = "left")
        HTML.title(add2, HR = 0, align = "left")
    } else if (noeffects > testeffects) # the model has an interaction (need to include max interaction comment)
    {

        #STB July 2013 change wording to remove effects
        add2 <- paste(c("Warning: It is not advisable to draw statistical inferences about a factor/interaction in the presence of a significant higher-order interaction involving that factor/interaction. In the above table we have assumed that certain higher-order interactions are not significant and have removed them from the statistical model, see log for more details."), " ", sep = "")
        HTML.title("<bf> ", HR = 2, align = "left")
        HTML.title(add2, HR = 0, align = "left")
    }

    if (tablen > 1) {
        if (allPairwiseTest == "none") {
            HTML.title("<bf> ", HR = 2, align = "left")
            HTML.title("<bf>Warning: As these tests are not adjusted for multiplicity there is a risk of generating false positive results. Only use the pairwise tests you planned to make a-priori, these are the so called planned comparisons, see Snedecor and Cochran (1989).", HR = 0, align = "left")
        } else {
            HTML.title("<bf> ", HR = 2, align = "left")
            HTML.title("<bf>Warning: This procedure makes an adjustment assuming you want to make all pairwise comparisons. If this is not the case then these tests may be unduly conservative. You may wish to use planned comparisons (using unadjusted p-values) instead, see Snedecor and Cochran (1989), or make a manual adjustment to the unadjusted p-values using the P-value Adjustment module.", HR = 0, align = "left")
        }
    }

    if (allPairwiseTest != "none") {
        HTML.title("<bf> ", HR = 2, align = "left")
        HTML.title("<bf>Note: The confidence intervals quoted are not adjusted for multiplicity.", HR = 0, align = "left")
    }



    #--------------------------------------------------------------------------------------------------
    #--------------------------------------------------------------------------------------------------
    #SilveR Addition


    #Effect Code difference analysis Oct 2010

    #Sample size for all pairwise comparisons
    if (allPairwiseTest != "NULL" && showStandardisedEffects == "Y") {
        if (allPairwiseTest == "none") {
            add <- paste(c("All pairwise comparisons and standardised tests without adjustment for multiplicity (LSD tests)"))
        } else {
            add <- paste(c("All pairwise comparisons and standardised tests using "))
            add <- paste(add, allPairwiseTestText, sep = "")
            add <- paste(add, "'s test", sep = "")
        }

        HTMLbr()
        HTML.title(add, HR = 2, align = "left")

        # Generating a vector of sample sizes - VectorN
        length <- length(unique(levels(as.factor(statdata$mainEffect))))
        vectorN <- c(1:length)
        for (i in 1:length) {
            sub <- subset(statdata, statdata$mainEffect == unique(levels(as.factor(statdata$mainEffect)))[i])
            sub2 <- data.frame(sub)
            tempy <- na.omit(eval(parse(text = paste("sub2$", resp))))
            vectorN[i] = length(tempy)
        }

        #Creating the standardised effects
        #Extra ANOVA table
        temp <- Anova(threewayfull)
        totSS <- sum(anova(threewayfull)[2])
        temp2 <- temp
        totalN <- sum(temp2[2]) + 1
        col1 <- temp2[1]
        col2 <- temp2[1]
        col3 <- temp2[3]
        col4 <- temp2[4]
        blanq <- c(" ")

        for (i in 1:dim(temp)[1]) {
            blanq[i] = " "
        }

        ivsanova <- cbind(col1, blanq, temp2[2], blanq, col2, blanq, col3, blanq, col4)
        lenANOVA <- dim(ivsanova)[1]
        MSERROR <- as.numeric(ivsanova[lenANOVA, 5])
        tabsextra <- matrix(nrow = tablen, ncol = 5)

        for (i in 1:tablen) {
            tabsextra[i, 3] = (multci$confint[i] / MSERROR)
        }

        firstno <- c(1:tablen)
        secondno <- c(1:tablen)

        j <- 1
        l <- 1
        for (i in 2:(length)) {
            for (k in (i):(length)) {
                firstno[j] = vectorN[k]
                j = j + 1
            }

        }

        for (i in 1:(length - 1)) {
            for (k in 1:(length - i)) {
                secondno[l] = vectorN[i]
                l = l + 1
            }

        }


        for (i in 1:tablen) {
            tabsextra[i, 1] = firstno[i]
            tabsextra[i, 2] = secondno[i]
        }
        for (i in 1:tablen) {
            test <- ci.smd(smd = tabsextra[i, 3], n.1 = tabsextra[i, 1], n.2 = tabsextra[i, 2], conf.level = sig)
            tabsextra[i, 4] = test$Lower.Conf.Limit.smd
            tabsextra[i, 5] = test$Upper.Conf.Limit.smd
        }

        for (i in 1:tablen) {
            tabsextra[i, 3] = format(round((multci$confint[i] / MSERROR), 3), nsmall = 3, scientific = FALSE)
        }

        for (i in 1:tablen) {
            tabsextra[i, 4] = format(round(as.numeric(tabsextra[i, 4]), 3), nsmall = 3, scientific = FALSE)
        }

        for (i in 1:tablen) {
            tabsextra[i, 5] = format(round(as.numeric(tabsextra[i, 5]), 3), nsmall = 3, scientific = FALSE)
        }

        header2 <- c(" ", " ", " ", " ", " ")
        tabsextra <- rbind(header2, tabsextra)

        tabtotal <- cbind(tabls, tabsextra)

        rows <- rownames(multci$confint)
        rows <- sub(" - ", " vs. ", rows, fixed = TRUE)
        rownames(tabtotal) <- c("Comparison", rows)
        lowerCI <- paste("   Lower ", (sig * 100), "% CI   ", sep = "")
        upperCI <- paste("   Upper ", (sig * 100), "% CI   ", sep = "")

        lowerCI2 <- paste("   Stand diff lower ", (sig * 100), "% CI   ", sep = "")
        upperCI2 <- paste("   Stand diff upper ", (sig * 100), "% CI   ", sep = "")

        colnames(tabtotal) <- c("   Difference   ", lowerCI, upperCI, "   SEM   ", "   p-value   ", "n1", "n2", "Stand diff", lowerCI2, upperCI2)

        HTML(tabtotal, classfirstline = "second", align = "left")

        HTML.title("</bf> ", HR = 2, align = "left")
        HTML.title("<bf>The standardised effects and confidence intervals are computed using the methods described in Kelly (2007).", HR = 0, align = "left")
    }
    #--------------------------------------------------------------------------------------------------
    #--------------------------------------------------------------------------------------------------


    #===================================================================================================================
    #Back transformed geometric means table 
    #===================================================================================================================

    if (GeomDisplay == "Y" && (responseTransform == "Log10" || responseTransform == "Loge")) {

        add <- c("All pairwise comparisons as back-transformed ratios")
        HTMLbr()
        HTML.title(add, HR = 2, align = "left")

        add <- c("As the response was log transformed prior to analysis the differences between the least square (predicted) means are presented on the log scale. These results can be back-transformed onto the original scale, where differences on the log scale become ratios when back-transformed.")
        HTML.title("</bf> ", HR = 2, align = "left")
        HTML.title(add, HR = 0, align = "left")

        #Creating the table
        tabsx <- matrix(nrow = tablen, ncol = 3)

        if (responseTransform == "Log10") {
            for (i in 1:tablen) {
                tabsx[i, 1] = format(round(10 ^ (multci$confint[i]), 3), nsmall = 3, scientific = FALSE)
            }

            for (i in 1:tablen) {
                tabsx[i, 2] = format(round(10 ^ (multci$confint[i + tablen]), 3), nsmall = 3, scientific = FALSE)
            }

            for (i in 1:tablen) {
                tabsx[i, 3] = format(round(10 ^ (multci$confint[i + 2 * tablen]), 3), nsmall = 3, scientific = FALSE)
            }
        }

        if (responseTransform == "Loge") {
            for (i in 1:tablen) {
                tabsx[i, 1] = format(round(exp(multci$confint[i]), 3), nsmall = 3, scientific = FALSE)
            }

            for (i in 1:tablen) {
                tabsx[i, 2] = format(round(exp(multci$confint[i + tablen]), 3), nsmall = 3, scientific = FALSE)
            }

            for (i in 1:tablen) {
                tabsx[i, 3] = format(round(exp(multci$confint[i + 2 * tablen]), 3), nsmall = 3, scientific = FALSE)
            }
        }


        headerx <- c(" ", " ", " ")

        rowsx <- rownames(multci$confint)
        rowsx <- sub(" - ", " / ", rowsx, fixed = TRUE)


        #STB June 2015	
        for (i in 1:100) {
            rowsx <- sub("_ivs_dash_ivs_", " - ", rowsx, fixed = TRUE)
        }

        lowerCI <- paste("   Lower ", (sig * 100), "% CI   ", sep = "")
        upperCI <- paste("   Upper ", (sig * 100), "% CI   ", sep = "")

        tablsx <- rbind(headerx, tabsx)
        rownames(tablsx) <- c("Comparison", rowsx)
        colnames(tablsx) <- c("   Ratio   ", lowerCI, upperCI)
        HTML(tablsx, classfirstline = "second", align = "left")

    }

}
#============================================================================================================================
#Back to control comparisons
#============================================================================================================================
backToControlTestText = backToControlTest
if (backToControlTestText == "Unadjusted (LSD)") {
    backToControlTest = "none"
} else if (backToControlTestText == "Holm") {
    backToControlTest = "holm"
} else if (backToControlTestText == "Hochberg") {
    backToControlTest = "hochberg"
} else if (backToControlTestText == "Hommel") {
    backToControlTest = "hommel"
} else if (backToControlTestText == "Bonferonni") {
    backToControlTest = "bonferroni"
} else if (backToControlTestText == "Benjamini-Hochberg") {
    backToControlTest = "BH"
}


#============================================================================================================================
#All to one comparisons
if (backToControlTest != "NULL") {

    #Title
    if (backToControlTest == "none") {
        add <- paste(c("All to one comparisons without adjustment for multiplicity (LSD test)"))
    } else {
        add <- paste(c("All to one comparisons using "))
        add <- paste(add, backToControlTestText, sep = "")
        add <- paste(add, "'s procedure", sep = "")
    }

    HTMLbr()
    HTML.title("</bf> ", HR = 2, align = "left")
    HTML.title(add, HR = 2, align = "left")


    #============================================================================================================================
    #Creating the table of unadjusted p-values
    #Generate all pairwise comparisons, unadjusted for multiplicity

    #---------------------------------------------------------------------------------------------------------------------
    #---------------------------------------------------------------------------------------------------------------------
    #SilveR Addition
    #STB Jan 2011 T-max code

    if (backToControlTest != "T-max") {
        #---------------------------------------------------------------------------------------------------------------------
        #---------------------------------------------------------------------------------------------------------------------
        mult <- glht(lm(model, data = statdata, na.action = na.omit), linfct = lsm(eval(parse(text = paste("pairwise ~", selectedEffect)))))
        #---------------------------------------------------------------------------------------------------------------------
        #---------------------------------------------------------------------------------------------------------------------
        #SilveR Addition
    } else {
        if (backToControlTest != "T-max") {
            mult <- glht(lm(effectModel, data = statdata, na.action = na.omit), linfct = mcp(mainEffect = "Dunnett"), vcov = vcovHC)
        }
    }
    #---------------------------------------------------------------------------------------------------------------------
    #---------------------------------------------------------------------------------------------------------------------
    multci <- confint(mult, level = sig, calpha = univariate_calpha())
    #---------------------------------------------------------------------------------------------------------------------
    #---------------------------------------------------------------------------------------------------------------------
    #SilveR Addition
    if (backToControlTest != "T-max") {
        #---------------------------------------------------------------------------------------------------------------------
        #---------------------------------------------------------------------------------------------------------------------
        multp <- summary(mult, test = adjusted("none"))
        #---------------------------------------------------------------------------------------------------------------------
        #---------------------------------------------------------------------------------------------------------------------
        #SilveR Addition
    } else {
        if (backToControlTest == "T-max") {
            multp <- summary(mult)
        }
    }
    #---------------------------------------------------------------------------------------------------------------------
    #---------------------------------------------------------------------------------------------------------------------

    #Creating a matrix of the differences
    comps <- c(rownames(multci$confint))
    diffz <- matrix(nrow = length(comps), ncol = 2)
    for (g in 1:length(comps)) {
        comps2 <- unlist(strsplit(comps[g], " - "))[1]
        comps3 <- unlist(strsplit(comps[g], " - "))[2]
        diffz[g, 1] = comps2
        diffz[g, 2] = comps3
    }

    #Creating the unadjusted full column
    pvals <- multp$test$pvalues
    sigma <- multp$test$sigma
    tstats <- Mod(as.numeric(multp$test$tstat))

    tablen <- length(unique(rownames(multci$confint)))

    tabs <- data.frame(nrow = tablen, ncol = 15)

    for (i in 1:tablen) {
        tabs[i, 1] = multci$confint[i]
    }
    for (i in 1:tablen) {
        tabs[i, 2] = multci$confint[i + tablen]
    }
    for (i in 1:tablen) {
        tabs[i, 3] = multci$confint[i + 2 * tablen]
    }
    for (i in 1:tablen) {
        tabs[i, 4] = sigma[i]
    }
    for (i in 1:tablen) {
        tabs[i, 5] = pvals[i]
    }
    for (i in 1:tablen) {
        tabs[i, 6] = diffz[i, 1]
    }
    for (i in 1:tablen) {
        tabs[i, 7] = diffz[i, 2]
    }
    for (i in 1:tablen) {
        tabs[i, 8] = tstats[i]
    }
    tabs2 <- tabs

    for (i in 1:tablen) {
        if (tabs2[i, 6] == cntrlGroup) {
            tabs2[i, 9] = -1 * tabs2[i, 1]
            tabs2[i, 10] = -1 * tabs2[i, 3]
            tabs2[i, 11] = -1 * tabs2[i, 2]
            tabs2[i, 12] = tabs2[i, 7]
            tabs2[i, 13] = tabs2[i, 6]
        } else {
            tabs2[i, 9] = tabs2[i, 1]
            tabs2[i, 10] = tabs2[i, 2]
            tabs2[i, 11] = tabs2[i, 3]
            tabs2[i, 12] = tabs2[i, 6]
            tabs2[i, 13] = tabs2[i, 7]
        }
    }

    for (i in 1:tablen) {
        tabs2[i, 14] = paste(tabs2[i, 12], " vs. ", tabs2[i, 13], sep = "")
    }

    #Subsetting to only the comparisons to control
    tabs3 <- subset(tabs2, V13 == cntrlGroup)

    if (backToControlTest == "Dunnett") {
        ntrgps <- length(unique(eval(parse(text = paste("statdata$", selectedEffect))))) - 1

        if (ntrgps != 1) {

            #============================================================================================================================
            #Dunnetts code
            # remove blank rows form the data, then calculate number of groups
            nallgps <- length(unique(eval(parse(text = paste("statdata$", selectedEffect)))))
            samplesize <- c(1:nallgps)

            #calculate the sample sizes
            for (i in 1:nallgps) {
                samplesize[i] <- sum(eval(parse(text = paste("statdata$", selectedEffect))) == levels(eval(parse(text = paste("statdata$", selectedEffect))))[i])
            }

            # calculate the total number of obs and the DF for the Dunnetts test (get this from ANOVA?)
            totalobs <- sum(samplesize)
            dfree <- df.residual(lm(model, data = statdata, na.action = na.omit))

            # calculation of correlation coefficient (according to Dunnett)
            cormat <- diag(ntrgps)
            for (j in 1:(ntrgps - 1)) {
                for (k in (j + 1):ntrgps) {
                    cormat[j, k] <- 1 / (sqrt(((samplesize[1] / samplesize[j + 1]) + 1) * ((samplesize[1] / samplesize[k + 1]) + 1)))
                    cormat[k, j] <- cormat[j, k]
                }
            }

            #call to get critical value
            critval95 <- qmvt(0.95, df = dfree, tail = "both", corr = cormat, abseps = 0.0001)[1]
            critval99 <- qmvt(0.99, df = dfree, tail = "both", corr = cormat, abseps = 0.0001)[1]
            critval999 <- qmvt(0.999, df = dfree, tail = "both", corr = cormat, abseps = 0.0001)[1]
            critvalsig <- qmvt(sig, df = dfree, tail = "both", corr = cormat, abseps = 0.0001)[1]

            pvals <- tabs3$V5
            tstats <- tabs3$V8
            sigma <- tabs3$V4

            #Calculate p-value
            dunnett <- function(data, group) {
                pdunnett <- function(x, nallgps, dfree, cormat) {
                    1 - pmvt(lower = -x, upper = x, delta = numeric(nallgps - 1), df = dfree, corr = cormat, abseps = 0.00000001)
                }
                t <- tstats
                p <- sapply(t, pdunnett, nallgps, dfree, cormat)
                return(p)
            }

            dunnp <- dunnett()

            tabs3$V15 <- dunnp
            adjpval <- dunnp

        } else {
            tabs3$V15 <- tabs3$V5
            adjpval <- tabs3$V5
        }
    } else {
        #Adjusting the p-values
        unadjpval <- tabs3$V5
        adjpval <- p.adjust(unadjpval, method = backToControlTest)
        tabs3$V15 <- adjpval
    }


    #============================================================================================================================
    #Creating final table
    tabs4 <- data.frame()
    for (i in 1:dim(tabs3)[1]) {
        tabs4[i, 1] <- format(round(tabs3[i, 9], 3), nsmall = 3, scientific = FALSE)
        tabs4[i, 2] <- format(round(tabs3[i, 10], 3), nsmall = 3, scientific = FALSE)
        tabs4[i, 3] <- format(round(tabs3[i, 11], 3), nsmall = 3, scientific = FALSE)
        tabs4[i, 4] <- format(round(tabs3[i, 4], 3), nsmall = 3, scientific = FALSE)
        tabs4[i, 5] <- format(round(tabs3[i, 15], 4), nsmall = 4, scientific = FALSE)
    }

    for (i in 1:dim(tabs3)[1]) {
        if (adjpval[i] < 0.0001) {
            #STB March 2011 - formatting p-values p<0.0001
            #			tabs4[i,5]<-0.0001
            tabs4[i, 5] = format(round(0.0001, 4), nsmall = 4, scientific = FALSE)
            tabs4[i, 5] <- paste("<", tabs4[i, 5])
        }
    }

    header <- c(" ", " ", " ", " ", " ")
    tabls <- rbind(header, tabs4)

    #STB June 2015	
    for (i in 1:100) {
        tabs3$V14 <- sub("_ivs_dash_ivs_", " - ", tabs3$V14, fixed = TRUE)
    }

    rownames(tabls) <- c("Comparison", tabs3$V14)
    lowerCI <- paste("   Lower ", (sig * 100), "% CI   ", sep = "")
    upperCI <- paste("   Upper ", (sig * 100), "% CI   ", sep = "")
    #STB May 2012 correcting "SEM"
    colnames(tabls) <- c("   Difference   ", lowerCI, upperCI, "   Std error   ", "   p-value   ")

    HTML(tabls, classfirstline = "second", align = "left")

    #============================================================================================================================
    #Plot of the comparisons back to control


    CITitle <- paste("<bf>Plot of the comparisons between the predicted means with ", (sig * 100), "% confidence intervals", sep = "")
    HTML.title(CITitle, HR = 2, align = "left")

    #Code for LS MEans plot
    meanPlotqq <- sub(".html", "meanplotqq.jpg", htmlFile)
    jpeg(meanPlotqq, width = jpegwidth, height = jpegheight, quality = 100)

    #STB July2013
    plotFilepdf5qq <- sub(".html", "meanplotqq.pdf", htmlFile)
    dev.control("enable")

    #Setting up the dataset
    graphdata <- data.frame(tabs4)
    graphdata$Mean <- as.numeric(graphdata$V1)
    graphdata$Lower <- as.numeric(graphdata$V2)
    graphdata$Upper <- as.numeric(graphdata$V3)
    graphdata$Group_IVSq_ <- tabs3$V14
    Gr_intercept <- 0
    XAxisTitle <- "Comparison"
    YAxisTitle <- "Difference between the means"
    Gr_line_type <- Line_type_dashed

    #GGPLOT2 code
    LSMPLOT_diff()

    void <- HTMLInsertGraph(GraphFileName = sub("[A-Z0-9a-z,:,\\\\]*App_Data[\\\\]", "", meanPlotqq), Align = "left")

    #STB July2013
    if (pdfout == "Y") {
        pdf(file = sub("[A-Z0-9a-z,:,\\\\]*App_Data[\\\\]", "", plotFilepdf5qq), height = pdfheight, width = pdfwidth)
        dev.set(2)
        dev.copy(which = 3)
        dev.off(2)
        dev.off(3)
        pdfFile_5qq <- sub("[A-Z0-9a-z,:,\\\\]*App_Data[\\\\]", "", plotFilepdf5qq)
        linkToPdf5qq <- paste("<a href=\"", pdfFile_5qq, "\">Click here to view the PDF of the plot of comparisons back to control</a>", sep = "")
        HTML(linkToPdf5qq)
    }

    #============================================================================================================================
    #Conclusions
    add <- paste(c("Conclusion"))
    inte <- 1
    for (i in 2:(dim(tabls)[1])) {
        if (tabls[i, 5] <= (1 - sig)) {
            if (inte == 1) {
                inte <- inte + 1
                add <- paste(add, ": The following pairwise tests are statistically significantly different at the  ", sep = "")
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
    if (inte == 1) {
        if (tablen > 1) {
            add <- paste(add, ": There are no statistically significant pairwise differences.", sep = "")
        } else {
            add <- paste(add, ": The pairwise difference is not statistically significant.", sep = "")
        }
    } else {
        add <- paste(add, ". ", sep = "")
    }

    HTML.title("</bf> ", HR = 2, align = "left")
    HTML.title(add, HR = 0, align = "left")

    if (noeffects > testeffects) # the model has an interaction (need to include max interaction comment)
    {
        add2 <- paste(c("Warning: It is not advisable to draw statistical inferences about a factor/interaction in the presence of a significant higher-order interaction involving that factor/interaction. In the above table we have assumed that certain higher order interactions are not significant, see log for more details."), " ", sep = "")
        HTML.title("<bf> ", HR = 2, align = "left")
        HTML.title(add2, HR = 0, align = "left")
    }
    if (tablen > 1) {
        if (backToControlTest == "none") {
            HTML.title("<bf> ", HR = 2, align = "left")
            HTML.title("<bf>Warning: As these tests are not adjusted for multiplicity there is a risk of generating false positive results. Only use the pairwise tests you planned to make a-priori, these are the so called planned comparisons, see Snedecor and Cochran (1989).", HR = 0, align = "left")
        }
    }
    if (backToControlTest != "none") {
        HTML.title("<bf> ", HR = 2, align = "left")
        HTML.title("<bf>Note: The confidence intervals quoted are not adjusted for multiplicity.", HR = 0, align = "left")
    }



    #----------------------------------------------------------------------------------------------------------------------------
    #----------------------------------------------------------------------------------------------------------------------------
    #SilveR Addition
    if (showStandardisedEffects == "zzz")
        #If you want to revisit this, the creation of the table of comparisons is not the current version... needs updating.
    #Generatign the list of comparisons (back to control) has been changed recently in IVS.
    #if (showStandardisedEffects =="Y")
    {


        #Effect Code difference analysis Oct 2010
        # Generating a vector of sample sizes - VectorN

        #Sample size for all pairwise comparisons


        if (backToControlTest == "none") {
            add <- paste(c("All to one comparisons with standardised effects without adjustment for multiplicity (LSD tests)"))
        } else {
            add <- paste(c("All to one comparisons with standardised effects using "))
            add <- paste(add, backToControlTestText, sep = "")
            add <- paste(add, "'s test", sep = "")
        }


        HTMLbr()
        HTML.title("</bf> ", HR = 2, align = "left")
        HTML.title(add, HR = 2, align = "left")

        length <- length(unique(levels(as.factor(statdata$mainEffect))))
        vectorN <- c(1:length)
        for (i in 1:length) {
            sub <- subset(statdata, statdata$mainEffect == unique(levels(as.factor(statdata$mainEffect)))[i])
            sub2 <- data.frame(sub)
            tempy <- na.omit(eval(parse(text = paste("sub2$", resp))))
            vectorN[i] = length(tempy)
        }

        #Creating the standardised effects
        #Extra ANOVA table
        temp <- Anova(threewayfull)
        totSS <- sum(anova(threewayfull)[2])
        temp2 <- temp
        totalN <- sum(temp2[2]) + 1
        col1 <- temp2[1]
        col2 <- temp2[1]
        col3 <- temp2[3]
        col4 <- temp2[4]
        blanq <- c(" ")

        for (i in 1:dim(temp)[1]) {
            blanq[i] = " "
        }

        ivsanova <- cbind(col1, blanq, temp2[2], blanq, col2, blanq, col3, blanq, col4)
        lenANOVA <- dim(ivsanova)[1]
        MSERROR <- as.numeric(ivsanova[lenANOVA, 5])
        tablen <- length(unique(rownames(multci$confint)))
        tabsextra <- matrix(nrow = tablen, ncol = 5)

        for (i in 1:tablen) {
            tabsextra[i, 3] = (multci$confint[i] / MSERROR)
        }

        #Generating table
        cntrlGroup2 <- paste("########  ", cntrlGroup)
        controlN <- c(1:length)
        otherN <- c(1:(length - 1))
        j <- 1
        for (i in 1:length) {
            if (unique(levels(as.factor(statdata$mainEffect)))[i] == cntrlGroup2) {
                controlN[i] = vectorN[i]
            } else {
                controlN[i] = 0
                otherN[j] = vectorN[i]
                j = j + 1
            }
        }
        contNo <- sum(controlN)

        for (i in 1:tablen) {
            tabsextra[i, 1] = contNo
            tabsextra[i, 2] = otherN[i]
        }

        for (i in 1:tablen) {
            test <- ci.smd(smd = tabsextra[i, 3], n.1 = tabsextra[i, 1], n.2 = tabsextra[i, 2], conf.level = sig)
            tabsextra[i, 4] = test$Lower.Conf.Limit.smd
            tabsextra[i, 5] = test$Upper.Conf.Limit.smd
        }

        for (i in 1:tablen) {
            tabsextra[i, 3] = format(round((multci$confint[i] / MSERROR), 3), nsmall = 3, scientific = FALSE)
        }

        for (i in 1:tablen) {
            tabsextra[i, 4] = format(round(as.numeric(tabsextra[i, 4]), 3), nsmall = 3, scientific = FALSE)
        }

        for (i in 1:tablen) {
            tabsextra[i, 5] = format(round(as.numeric(tabsextra[i, 5]), 3), nsmall = 3, scientific = FALSE)
        }

        header2 <- c(" ", " ", " ", " ", " ")
        tabsextra <- rbind(header2, tabsextra)
        tabtotal <- cbind(tabls, tabsextra)
        rows <- rownames(multci$confint)

        rows <- sub("########", "", rows, fixed = TRUE)
        rows <- sub(" - ", " vs. ", rows, fixed = TRUE)
        rownames(tabtotal) <- c("Comparison", rows)
        lowerCI <- paste("   Lower ", (sig * 100), "% CI   ", sep = "")
        upperCI <- paste("   Upper ", (sig * 100), "% CI   ", sep = "")

        lowerCI2 <- paste("   Stand diff lower ", (sig * 100), "% CI   ", sep = "")
        upperCI2 <- paste("   Stand diff upper ", (sig * 100), "% CI   ", sep = "")

        colnames(tabtotal) <- c("   Difference   ", lowerCI, upperCI, "   SEM   ", "   p-value   ", "n1", "n2", "Stand diff", lowerCI2, upperCI2)


        HTML(tabtotal, classfirstline = "second", align = "left")
        HTML.title("</bf> ", HR = 2, align = "left")
        HTML.title("<bf>The standardised effects and confidence intervals are computed using the methods described in Kelly (2007).", HR = 0, align = "left")




    }

    #----------------------------------------------------------------------------------------------------------------------------
    #----------------------------------------------------------------------------------------------------------------------------


    #============================================================================================================================
    #Back transformed geometric means table 
    #============================================================================================================================
    if (GeomDisplay == "Y" && (responseTransform == "Log10" || responseTransform == "Loge")) {

        add <- c("All to one comparisons as back-transformed ratios")
        HTMLbr()
        HTML.title(add, HR = 2, align = "left")

        add <- c("As the response was log transformed prior to analysis the differences between the least square (predicted) means are presented on the log scale. These results can be back-transformed onto the original scale, where differences on the log scale become ratios when back-transformed.")
        HTML.title("</bf> ", HR = 2, align = "left")
        HTML.title(add, HR = 0, align = "left")

        #Creating final table
        tabs4x <- data.frame()

        if (responseTransform == "Log10") {
            for (i in 1:dim(tabs3)[1]) {
                tabs4x[i, 1] <- format(round(10 ^ (tabs3[i, 9]), 3), nsmall = 3, scientific = FALSE)
                tabs4x[i, 2] <- format(round(10 ^ (tabs3[i, 10]), 3), nsmall = 3, scientific = FALSE)
                tabs4x[i, 3] <- format(round(10 ^ (tabs3[i, 11]), 3), nsmall = 3, scientific = FALSE)
            }
        }

        if (responseTransform == "Loge") {
            for (i in 1:dim(tabs3)[1]) {
                tabs4x[i, 1] <- format(round(exp(tabs3[i, 9]), 3), nsmall = 3, scientific = FALSE)
                tabs4x[i, 2] <- format(round(exp(tabs3[i, 10]), 3), nsmall = 3, scientific = FALSE)
                tabs4x[i, 3] <- format(round(exp(tabs3[i, 11]), 3), nsmall = 3, scientific = FALSE)
            }
        }

        headerx <- c(" ", " ", " ")
        tablsx <- rbind(headerx, tabs4x)

        tabs3$V14 <- sub(" vs. ", " / ", tabs3$V14, fixed = TRUE)


        #STB June 2015	
        for (i in 1:100) {
            tabs3$V14 <- sub("_ivs_dash_ivs_", " - ", tabs3$V14, fixed = TRUE)
        }

        rownames(tablsx) <- c("Comparison", tabs3$V14)
        lowerCI <- paste("   Lower ", (sig * 100), "% CI   ", sep = "")
        upperCI <- paste("   Upper ", (sig * 100), "% CI   ", sep = "")
        #STB May 2012 correcting "SEM"
        colnames(tablsx) <- c("   Ratio   ", lowerCI, upperCI)

        HTML(tablsx, classfirstline = "second", align = "left")

        #============================================================================================================================
        #Plot of the comparisons back to control

        CITitle <- paste("<bf>Plot of the comparisons between the back-transformed geometric  means with ", (sig * 100), "% confidence intervals", sep = "")
        HTML.title(CITitle, HR = 2, align = "left")


        #Code for LS MEans plot
        meanPlotqs <- sub(".html", "meanplotqs.jpg", htmlFile)
        jpeg(meanPlotqs, width = jpegwidth, height = jpegheight, quality = 100)

        #STB July2013
        plotFilepdf5qs <- sub(".html", "meanplotqs.pdf", htmlFile)
        dev.control("enable")

        #Setting up the dataset
        graphdata <- data.frame(tabs4x)
        graphdata$Mean <- as.numeric(graphdata$V1)
        graphdata$Lower <- as.numeric(graphdata$V2)
        graphdata$Upper <- as.numeric(graphdata$V3)
        graphdata$Group_IVSq_ <- tabs3$V14
        Gr_intercept <- 1
        XAxisTitle <- "Comparison"
        YAxisTitle <- "Ratio of differences between the geometric means"

        #GGPLOT2 code
        LSMPLOT_diff()

        void <- HTMLInsertGraph(GraphFileName = sub("[A-Z0-9a-z,:,\\\\]*App_Data[\\\\]", "", meanPlotqs), Align = "left")

        #STB July2013
        if (pdfout == "Y") {
            pdf(file = sub("[A-Z0-9a-z,:,\\\\]*App_Data[\\\\]", "", plotFilepdf5qs), height = pdfheight, width = pdfwidth)
            dev.set(2)
            dev.copy(which = 3)
            dev.off(2)
            dev.off(3)
            pdfFile_5qs <- sub("[A-Z0-9a-z,:,\\\\]*App_Data[\\\\]", "", plotFilepdf5qs)
            linkToPdf5qs <- paste("<a href=\"", pdfFile_5qs, "\">Click here to view the PDF of the plot of comparisons back to control</a>", sep = "")
            HTML(linkToPdf5qs)
        }
    }
}

#============================================================================================================================
#Analysis description
#============================================================================================================================
HTMLbr()
HTML.title("<bf>Analysis description", HR = 2, align = "left")

add <- c("The data were analysed using a ")

if (notreatfactors == 1) {

    if (FirstCatFactor != "NULL") {
        add <- paste(add, "1-way ANCOVA approach, with treatment factor ", sep = "")
        add <- paste(add, treatFactors, sep = "")
    } else {
        add <- paste(add, "1-way ANOVA approach, with treatment factor ", sep = "")
        add <- paste(add, treatFactors, sep = "")
    }
} else {
    add <- paste(add, notreatfactors, sep = "")
    if (FirstCatFactor != "NULL") {
        add <- paste(add, "-way ANCOVA approach, with ", sep = "")
    } else {
        add <- paste(add, "-way ANOVA approach, with ", sep = "")
    }
    for (i in 1:notreatfactors) {
        if (i < notreatfactors - 1) {
            add <- paste(add, txtexpectedChanges[i + 1], sep = "")
            add <- paste(add, ", ", sep = "")
        } else if (i < notreatfactors) {
            add <- paste(add, txtexpectedChanges[i + 1], sep = "")
            add <- paste(add, " and ", sep = "")
        } else if (i == notreatfactors) {
            add <- paste(add, txtexpectedChanges[i + 1], sep = "")
            add <- paste(add, " as treatment factors", sep = "")
        }
    }
}

if (blockFactors != "NULL" && FirstCatFactor != "NULL") {
    add <- paste(add, ", ", sep = "")
} else if (noblockfactors == 1 && blockFactors != "NULL" && FirstCatFactor == "NULL") {
    add <- paste(add, " and ", sep = "")
}

if (noblockfactors == 1 && blockFactors != "NULL") {
    add <- paste(add, blockFactors, sep = "")
    add <- paste(add, " as a blocking factor", sep = "")
} else {
    if (noblockfactors > 1) # there is two or more blocking factors
    {
        if (FirstCatFactor == "NULL") {
            add <- paste(add, " and ", sep = "")
        }
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

if (FirstCatFactor == "NULL") {
    add <- paste(add, ". ", sep = "")
} else if (FirstCatFactor != "NULL") {
    add <- paste(add, " and  ", sep = "")
    add <- paste(add, unlist(strsplit(covariateModel, "~"))[2], sep = "")
    add <- paste(add, " as the covariate. ", sep = "")
}

if (allPairwiseTest == "none" | backToControlTest == "none") {
    #STB May 2012 Updating "Selected"
    add <- paste(add, "This was followed by planned comparisons of the predicted means to compare the levels of the ", sep = "")
    add <- paste(add, selectedEffect, sep = "")
    if (factno == 1) {
        add <- paste(add, " factor. ", sep = "")
    } else {
        add <- paste(add, " interaction. ", sep = "")
    }
}

if (backToControlTest != "NULL" & backToControlTest != "none") {
    add <- paste(add, "This was followed by comparisons of the predicted means of the ", sep = "")
    add <- paste(add, selectedEffect, sep = "")
    add <- paste(add, " factor back to the control group mean using ", sep = "")
    add <- paste(add, backToControlTestText, sep = "")
    add <- paste(add, "'s procedure. ", sep = "")
}

if (allPairwiseTest != "NULL" & allPairwiseTest != "none") {
    #STB May 2012 Updating "Selected"
    add <- paste(add, "This was followed by all pairwise comparisons between the predicted means of the ", sep = "")
    add <- paste(add, selectedEffect, sep = "")
    if (factno == 1) {
        add <- paste(add, " factor ", sep = "")
    } else {
        add <- paste(add, " interaction ", sep = "")
    }
    add <- paste(add, " using ", sep = "")
    add <- paste(add, allPairwiseTestText, sep = "")
    add <- paste(add, "'s procedure. ", sep = "")
}

if (responseTransform != "None") {
    add <- paste(add, " The response was ", sep = "")
    add <- paste(add, responseTransform, sep = "")
    add <- paste(add, " transformed prior to analysis to stabilise the variance. ", sep = "")
}

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

if (showANOVA == "Y") {
    HTML.title("<bf> ", HR = 2, align = "left")
    HTML.title("<bf> Armitage P, Matthews JNS and Berry G. (2001). Statistical Methods in Medical Research. 4th edition; John Wiley & Sons. New York.", HR = 0, align = "left")
}

if (allPairwiseTest == "BH" | backToControlTest == "BH") {
    HTML.title("<bf> ", HR = 2, align = "left")
    HTML.title("<bf>Benjamini Y and Hochberg Y. (1995). Controlling the false discovery rate: a practical and powerful approach to multiple testing. Journal of the Royal Statistical Society Series B, 57, 289-300. ", HR = 0, align = "left")
}

if (allPairwiseTest == "bonferroni" | backToControlTest == "bonferroni") {
    HTML.title("<bf> ", HR = 2, align = "left")
    HTML.title("<bf>Bonferroni CE. (1936). Teoria statistica delle classi e calcolo delle probabilita. Pubblicazioni del R Istituto Superiore di Scienze Economiche e Commerciali di Firenze, 8, 3-62.", HR = 0, align = "left")
}

if (allPairwiseTest == "Tukey") {
    HTML.title("<bf> ", HR = 2, align = "left")
    HTML.title("<bf>Braun HI, ed. (1994). The collected works of John W. Tukey. Vol. VIII: Multiple comparisons:1948-1983. New York: Chapman and Hall.", HR = 0, align = "left")
}

if (backToControlTest == "Dunnett") {
    HTML.title("<bf> ", HR = 2, align = "left")
    HTML.title("<bf>Dunnett CW. (1955). A multiple comparison procedure for comparing several treatments with a control. Journal of the American Statistical Association, 50, 1096-1121.", HR = 0, align = "left")
}

if (allPairwiseTest == "hochberg" | backToControlTest == "hochberg") {
    HTML.title("<bf> ", HR = 2, align = "left")
    HTML.title("<bf>Hochberg Y. (1988). A sharper Bonferroni procedure for multiple tests of significance. Biometrika, 75, 800-803.", HR = 0, align = "left")
}

if (allPairwiseTest == "holm" | backToControlTest == "holm") {
    HTML.title("<bf> ", HR = 2, align = "left")
    HTML.title("<bf>Holm S. (1979). A simple sequentially rejective multiple test procedure. Scandinavian Journal of Statistics, 6, 65-70.", HR = 0, align = "left")
}

if (allPairwiseTest == "hommel" | backToControlTest == "hommel") {
    HTML.title("<bf> ", HR = 2, align = "left")
    HTML.title("<bf>Hommel G. (1988). A stagewise rejective multiple test procedure based on a modified Bonferroni test. Biometrika, 75, 383-386.", HR = 0, align = "left")
}

#--------------------------------------------------------------------------------------------------------------------------
#--------------------------------------------------------------------------------------------------------------------------
#SilveR Addition
if (showANOVA == "Y") {
    HTML.title("<bf> ", HR = 2, align = "left")
    HTML.title("<bf>Kelly K. (2007). Confidence intervals for standardized effect sizes: Theory, application and implementation. Journal of statistical software, 20(8), 1-24.", HR = 0, align = "left")
}
#--------------------------------------------------------------------------------------------------------------------------
#--------------------------------------------------------------------------------------------------------------------------


if (FirstCatFactor != "NULL") {
    HTML.title("<bf> ", HR = 2, align = "left")
    HTML.title("<bf> Morris TR. (1999). Experimental Design and Analysis in Animal Sciences. CABI publishing. Wallingford, Oxon (UK).", HR = 0, align = "left")
}


if (allPairwiseTest != "NULL" | backToControlTest != "NULL") {
    if (tablen > 1 & (allPairwiseTest == "none" | backToControlTest == "none" | allPairwiseTest != "NULL" | backToControlTest == "NULL")) {
        HTML.title("<bf> ", HR = 2, align = "left")
        HTML.title("<bf>Snedecor GW and Cochran WG. (1989). Statistical Methods. 8th edition;  Iowa State University Press, Iowa, USA.", HR = 0, align = "left")
    }
}



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
HTML.title(Ref_list$car_ref, HR = 0, align = "left")

HTML.title("<bf> ", HR = 2, align = "left")
HTML.title(Ref_list$R2HTML_ref, HR = 0, align = "left")

HTML.title("<bf> ", HR = 2, align = "left")
HTML.title(Ref_list$PROTO_ref, HR = 0, align = "left")

HTML.title("<bf> ", HR = 2, align = "left")
HTML.title(Ref_list$LSMEANS_ref, HR = 0, align = "left")

HTML.title("<bf> ", HR = 2, align = "left")
HTML.title(Ref_list$multcomp_ref, HR = 0, align = "left")


#--------------------------------------------------------------------------------------------------------------------------
#--------------------------------------------------------------------------------------------------------------------------
#SilveR Addition

if (showStandardisedEffects == "Y") {
    #MBESS
    HTML.title("<bf> ", HR = 2, align = "left")
    HTML.title("<bf>
	Kelly, K (2007). MBESS: Methods for the Behavioural, Educational, and Social Sciences. R package 0.0.8, URL http://CRAN.R-project.org/. 	", HR = 0, align = "left")
}
#--------------------------------------------------------------------------------------------------------------------------
#--------------------------------------------------------------------------------------------------------------------------

#----------------------------------------------------------------------------------------------------------
#Show dataset
#----------------------------------------------------------------------------------------------------------


if (showdataset == "Y") {
    HTMLbr()
    HTML.title("<bf>Analysis dataset", HR = 2, align = "left")
    statdata_temp <- subset(statdata_temp, select = -c(mainEffect, scatterPlotColumn))

    if (backToControlTest != "NULL") {
        #		statdata_temp<-subset(statdata_temp, select = -c(mainEffect))
    }
    HTML(statdata_temp, classfirstline = "second", align = "left")
}


quit()

#----------------------------------------------------------------------------------------------------------
#Create file of comparisons
#----------------------------------------------------------------------------------------------------------

if (allPairwiseTest != "NULL") {
    write.csv(tabsxx, file = sub("[A-Z0-9a-z,:,\\\\]*App_Data[\\\\]", "", comparisons), row.names = FALSE)
    #	write.csv(tabsxx, file = "Comparisons.csv", row.names=FALSE)
}






























