suppressWarnings(library(bitops))
suppressWarnings(library(splines))
suppressWarnings(library(gtools))
suppressWarnings(library(gdata))
suppressWarnings(library(caTools))
suppressWarnings(library(grid))
suppressWarnings(library(survival))
suppressWarnings(library(gplots))
suppressWarnings(library(mvtnorm))
suppressWarnings(library(multcomp))
suppressWarnings(library(car))
suppressWarnings(library(R2HTML))
suppressWarnings(library(lattice))
suppressWarnings(library(methods))


#STB 14OCT2015
#Set contrast options for Marginal overall tests
options(contrasts = c(unordered = "contr.sum", ordered = "contr.poly"))


# retrieve args
Args <- commandArgs(TRUE)

#Read in data
statdata <- read.csv(Args[3], header = TRUE, sep = ",")

#Copy Args
model <- as.formula(Args[4])
#print(model)
#print(length(grep("\\*", model)))
scatterplotModel <- as.formula(Args[5])
covariateModel <- Args[6]
responseTransform <- Args[7]
covariateTransform <- Args[8]
primFactor <- Args[9]
treatFactors <- Args[10]
blockFactors <- Args[11]
showANOVA <- Args[12]
showPRPlot <- Args[13]
showNormPlot <- Args[14]
sig <- 1 - as.numeric(Args[15])
effectModel <- as.formula(Args[16])
effectModel2 <- Args[16]
showLSMeans <- Args[17]
allPairwiseTest <- Args[18]
backToControlTest <- Args[19]
cntrlGroup <- Args[20]
showdataset <- Args[21]

#V3.2 STB OCT2015
set.seed(5041975)

#Setup the html file and associated css file
htmlFile <- sub(".csv", ".html", Args[3]);
#determine the file name of the html file
.HTML.file = htmlFile


#Output HTML header
HTML.title("<bf>Incomplete Factorial Parametric Analysis", HR = 1, align = "left")

statdata$mainEffect <- as.factor(statdata$mainEffect)
statdata$scatterPlotColumn <- as.factor(statdata$scatterPlotColumn)


resp <- unlist(strsplit(Args[4], "~"))[1] #get the response variable from the main model


#Removing illegal charaters

YAxisTitle <- resp

if (primFactor != "NULL") {
    XAxisTitle <- unlist(strsplit(covariateModel, "~"))[2]
}

for (i in 1:10) {

    # Additional characters included Aug 2010 (STB)
    YAxisTitle <- sub("ivs_tilde_ivs", "~", YAxisTitle)
    YAxisTitle <- sub("ivs_star_ivs", "*", YAxisTitle)
    YAxisTitle <- sub("ivs_plus_ivs", "+", YAxisTitle)

    YAxisTitle <- sub("ivs_sp_ivs", " ", YAxisTitle)
    YAxisTitle <- sub("ivs_ob_ivs", "(", YAxisTitle)
    YAxisTitle <- sub("ivs_cb_ivs", ")", YAxisTitle)
    YAxisTitle <- sub("ivs_div_ivs", "/", YAxisTitle)
    YAxisTitle <- sub("ivs_pc_ivs", "%", YAxisTitle)
    YAxisTitle <- sub("ivs_hash_ivs", "#", YAxisTitle)
    YAxisTitle <- sub("ivs_pt_ivs", ".", YAxisTitle)
    YAxisTitle <- sub("ivs_hyphen_ivs", "-", YAxisTitle)
    YAxisTitle <- sub("ivs_at_ivs", "@", YAxisTitle)
    YAxisTitle <- sub("ivs_colon_ivs", ":", YAxisTitle)
    YAxisTitle <- sub("ivs_exclam_ivs", "!", YAxisTitle)
    YAxisTitle <- sub("ivs_quote_ivs", "`", YAxisTitle)
    YAxisTitle <- sub("ivs_pound_ivs", "�", YAxisTitle)
    YAxisTitle <- sub("ivs_dollar_ivs", "$", YAxisTitle)
    YAxisTitle <- sub("ivs_hat_ivs", "^", YAxisTitle)
    YAxisTitle <- sub("ivs_amper_ivs", "&", YAxisTitle)
    YAxisTitle <- sub("ivs_obrace_ivs", "{", YAxisTitle)
    YAxisTitle <- sub("ivs_cbrace_ivs", "}", YAxisTitle)
    YAxisTitle <- sub("ivs_semi_ivs", ";", YAxisTitle)
    YAxisTitle <- sub("ivs_pipe_ivs", "|", YAxisTitle)
    YAxisTitle <- sub("ivs_slash_ivs", "\\", YAxisTitle)
    YAxisTitle <- sub("ivs_osb_ivs", "[", YAxisTitle)
    YAxisTitle <- sub("ivs_csb_ivs", "]", YAxisTitle)
    YAxisTitle <- sub("ivs_eq_ivs", "=", YAxisTitle)
    YAxisTitle <- sub("ivs_lt_ivs", "<", YAxisTitle)
    YAxisTitle <- sub("ivs_gt_ivs", ">", YAxisTitle)
    YAxisTitle <- sub("ivs_dblquote_ivs", "\"", YAxisTitle)

    if (primFactor != "NULL") {

        # Additional characters included Aug 2010 (STB)
        XAxisTitle <- sub("ivs_tilde_ivs", "~", XAxisTitle)
        XAxisTitle <- sub("ivs_star_ivs", "*", XAxisTitle)
        XAxisTitle <- sub("ivs_plus_ivs", "+", XAxisTitle)

        XAxisTitle <- sub("ivs_sp_ivs", " ", XAxisTitle)
        XAxisTitle <- sub("ivs_ob_ivs", "(", XAxisTitle)
        XAxisTitle <- sub("ivs_cb_ivs", ")", XAxisTitle)
        XAxisTitle <- sub("ivs_div_ivs", "/", XAxisTitle)
        XAxisTitle <- sub("ivs_pc_ivs", "%", XAxisTitle)
        XAxisTitle <- sub("ivs_hash_ivs", "#", XAxisTitle)
        XAxisTitle <- sub("ivs_pt_ivs", ".", XAxisTitle)
        XAxisTitle <- sub("ivs_hyphen_ivs", "-", XAxisTitle)
        XAxisTitle <- sub("ivs_at_ivs", "@", XAxisTitle)
        XAxisTitle <- sub("ivs_colon_ivs", ":", XAxisTitle)
        XAxisTitle <- sub("ivs_exclam_ivs", "!", XAxisTitle)
        XAxisTitle <- sub("ivs_quote_ivs", "`", XAxisTitle)
        XAxisTitle <- sub("ivs_pound_ivs", "�", XAxisTitle)
        XAxisTitle <- sub("ivs_dollar_ivs", "$", XAxisTitle)
        XAxisTitle <- sub("ivs_hat_ivs", "^", XAxisTitle)
        XAxisTitle <- sub("ivs_amper_ivs", "&", XAxisTitle)
        XAxisTitle <- sub("ivs_obrace_ivs", "{", XAxisTitle)
        XAxisTitle <- sub("ivs_cbrace_ivs", "}", XAxisTitle)
        XAxisTitle <- sub("ivs_semi_ivs", ";", XAxisTitle)
        XAxisTitle <- sub("ivs_pipe_ivs", "|", XAxisTitle)
        XAxisTitle <- sub("ivs_slash_ivs", "\\", XAxisTitle)
        XAxisTitle <- sub("ivs_osb_ivs", "[", XAxisTitle)
        XAxisTitle <- sub("ivs_csb_ivs", "]", XAxisTitle)
        XAxisTitle <- sub("ivs_eq_ivs", "=", XAxisTitle)
        XAxisTitle <- sub("ivs_lt_ivs", "<", XAxisTitle)
        XAxisTitle <- sub("ivs_gt_ivs", ">", XAxisTitle)
        XAxisTitle <- sub("ivs_dblquote_ivs", "\"", XAxisTitle)
    }
}





#Three-way ANOVA (complete)
threewayfull <- lm(model, data = statdata, na.action = na.omit)

#calculating number of block and treatment factors

noblockfactors = 0
if (blockFactors != "NULL") {
    tempblockChanges <- strsplit(blockFactors, ",")
    txtexpectedblockChanges <- c("")
    for (i in 1:length(tempblockChanges[[1]])) {
        txtexpectedblockChanges[length(txtexpectedblockChanges) + 1] = (tempblockChanges[[1]][i])
    }
    noblockfactors <- length(txtexpectedblockChanges) - 1
}

tempChanges <- strsplit(treatFactors, ",")
txtexpectedChanges <- c("")
for (i in 1:length(tempChanges[[1]])) {
    txtexpectedChanges[length(txtexpectedChanges) + 1] = (tempChanges[[1]][i])
}
notreatfactors <- length(txtexpectedChanges) - 1


# Code to create varibale to test if the highest order interaction is selected
testeffects = noblockfactors
if (primFactor != "NULL") {
    testeffects = noblockfactors + 1
}
emodel <- strsplit(effectModel2, "+", fixed = TRUE)

emodelChanges <- c("")
for (i in 1:length(emodel[[1]])) {
    emodelChanges[length(emodelChanges) + 1] = (emodel[[1]][i])
}
noeffects <- length(emodelChanges) - 2




#Warning
title <- c("Warning")
HTML.title(title, HR = 2, align = "left")

HTML.title("</bf> ", HR = 2, align = "left")
HTML.title("Warning: This module is currently under construction, care should be taken when considering the results. The results have not been verified.", HR = 0, align = "left")



#Response

title <- c("Response")
if (primFactor != "NULL") {
    title <- paste(title, " and covariate", sep = "")
}

HTML.title(title, HR = 2, align = "left")
add <- paste(c("The  "), resp, sep = "")
add <- paste(add, " response is currently being analysed by the Incomplete Factorial Parametric Analysis module", sep = "")

if (primFactor != "NULL") {
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

#Scatterplot
HTMLbr()
title <- c("Scatterplot of the raw data")
if (responseTransform != "None") {
    title <- paste(title, " (on the ", sep = "")
    title <- paste(title, responseTransform, sep = "")
    title <- paste(title, " scale)", sep = "")
}

HTML.title(title, HR = 2, align = "left")
scatterPlot <- sub(".html", "scatterPlot.jpg", htmlFile)
jpeg(scatterPlot)
xyplot(scatterplotModel, statdata, col = "black", ylab = YAxisTitle, xlab = NULL, scales = list(x = list(rot = 90)))
void <- HTMLInsertGraph(GraphFileName = sub("[A-Z0-9a-z,:,\\\\]*App_Data[\\\\]", "", scatterPlot), Align = "centre")

HTML.title("</bf> ", HR = 2, align = "left")
HTML.title("</bf> Tip: Use this plot to identify possible outliers.", HR = 0, align = "left")







#Covariate plot
if (primFactor != "NULL") {

    title <- c("Covariate plot of the raw data")
    if (responseTransform != "None" || covariateTransform != "None") {
        title <- paste(title, " (on the transformed scale)", sep = "")
    }
    HTMLbr()
    HTML.title(title, HR = 2, align = "left")



    covariate <- unlist(strsplit(covariateModel, "~"))[2]

    rows <- dim(statdata)[1]
    cols <- dim(statdata)[2]
    nlevels <- length(unique(eval(parse(text = paste("statdata$", primFactor)))))

    extra <- matrix(data = NA, nrow = rows, ncol = nlevels)

    for (i in 1:nlevels) {
        for (j in 1:rows) {
            if (eval(parse(text = paste("statdata$", primFactor)))[j] == unique(eval(parse(text = paste("statdata$", primFactor))))[i]) {
                extra[j, i] <- eval(parse(text = paste("statdata$", resp)))[j]
            }
        }
    }

    newdata <- cbind(statdata, extra)
    catplotdata <- data.frame(newdata)




    for (k in 1:nlevels) {
        tempdata <- catplotdata
        tempdata2 <- subset(tempdata, eval(parse(text = paste("statdata$", primFactor))) == unique(levels(as.factor(eval(parse(text = paste("tempdata$", primFactor))))))[k])
    }

    index <- c(1:nlevels)
    newnames <- c(colnames(statdata), index)
    colnames(catplotdata) <- newnames
    ncscatterplot3 <- sub(".html", "ncscatterplot3.jpg", htmlFile)
    jpeg(ncscatterplot3)

    #STB sept 2011 CC26
    maxresp <- max(eval(parse(text = paste("statdata$", resp))), na.rm = TRUE)
    minresp <- min(eval(parse(text = paste("statdata$", resp))), na.rm = TRUE)

    #Adjusting y  axis to fit in legend
    #maxresp<-max(eval(parse(text = paste("statdata$",resp))))
    #minresp<-min(eval(parse(text = paste("statdata$",resp))))

    rangeresp <- maxresp - minresp
    maxob <- maxresp + rangeresp * length(unique(levels(as.factor(eval(parse(text = paste("statdata$", primFactor))))))) * 0.075
    minob <- minresp - rangeresp * 0.1

    cat <- c(as.factor(eval(parse(text = paste("statdata$", primFactor)))))
    par(las = 1)
    plot(as.formula(covariateModel), data = catplotdata, col = cat, pch = cat, ylim = c(minob, maxob), xlab = XAxisTitle, ylab = YAxisTitle)
    legend("topright", legend = levels(as.factor(eval(parse(text = paste("tempdata$", primFactor))))), cex = 0.6, pch = c(1:nlevels), lty = 1:nlevels, bg = "white", col = c(1:nlevels))


    #Best fit line 
    for (k in 1:nlevels) {
        tempdata <- catplotdata
        tempdata2 <- subset(tempdata, eval(parse(text = paste("tempdata$", primFactor))) == unique(levels(as.factor(eval(parse(text = paste("tempdata$", primFactor))))))[k])
        abline(lm(eval(parse(text = paste("tempdata2$", resp))) ~ eval(parse(text = paste("tempdata2$", covariate)))), col = k)
    }

    void <- HTMLInsertGraph(GraphFileName = sub("[A-Z0-9a-z,:,\\\\]*App_Data[\\\\]", "", ncscatterplot3), Align = "centre")

    #	index<-c(1:nlevels)
    #	newnames<-c(colnames(statdata),index)
    #	colnames(catplotdata)<-newnames
    #

    #
    #	covariatePlot <- sub(".html", "covariatePlot.jpg", htmlFile)
    #	jpeg(covariatePlot)
    #
    #	plot(as.formula(covariateModel), data=catplotdata, col=c(1:nlevels),pch=c(1:nlevels))
    #	void<-HTMLInsertGraph(GraphFileName=sub("[A-Z0-9a-z,:,\\\\]*App_Data[\\\\]","", covariatePlot), Align="centre")
    #
    #	for (k in 1:nlevels)
    #	{
    #		tempdata<-catplotdata
    #		tempdata2<-subset(tempdata, eval(parse(text = paste("tempdata$", primFactor))) == unique(levels(eval(parse(text = paste("tempdata$", 		primFactor)))))[k])
    #		abline(lm(covariateModel, data=tempdata2), col=k)
    #	}
    #
    #	legend("topright", legend=unique(eval(parse(text = paste("statdata$", primFactor)))),cex=0.6,pch=c(1:nlevels), lty=1:nlevels,bg="white", col=c(1:nlevels))

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







#Testing the degrees of freedom

if (df.residual(threewayfull) < 5) {
    HTMLbr()
    HTML.title("<bf>Warning", HR = 2, align = "left")

    add <- c("Unfortunately the residual degrees of freedom are low (less than 5). This may make the estimation of the underlying variability, and hence the results of the statistical tests, unreliable. This can be caused by attempting to fit too many factors, and their interactions, in the statistical model. We recommend you fit some of the treatment factors as other design factors. This will remove their interactions from the statistical model and therefore increase the residual degrees of freedom.")
    HTML.title("</bf> ", HR = 2, align = "left")
    HTML.title(add, HR = 0, align = "left")
    print(add)
}

#ANOVA Table
if (showANOVA == "Y") {

    if (primFactor != "NULL") {
        HTMLbr()
        HTML.title("<bf>Analysis of Covariance (ANCOVA) table", HR = 2, align = "left")
    } else {
        HTMLbr()
        HTML.title("<bf>Analysis of variance (ANOVA) table", HR = 2, align = "left")
    }

    temp <- anova(threewayfull)
    temp2 <- (temp)
    col1 <- format(round(temp2[2], 2), nsmall = 2, scientific = FALSE)
    col2 <- format(round(temp2[1], 0), nsmall = 0, scientific = FALSE)
    col3 <- format(round(temp2[3], 2), nsmall = 2, scientific = FALSE)
    col4 <- format(round(temp2[4], 2), nsmall = 2, scientific = FALSE)
    col5 <- format(round(temp2[5], 3), nsmall = 3, scientific = FALSE)

    blanq <- c(" ")
    for (i in 1:dim(temp)[1]) {
        blanq[i] = " "
    }


    ivsanova <- cbind(col1, blanq, col2, blanq, col3, blanq, col4, blanq, col5)

    source2 <- rownames(ivsanova)
    source3 <- rownames(ivsanova)

    ivsanova[length(unique(source2)), 7] <- " "
    ivsanova[length(unique(source2)), 9] <- " "

    head <- c("Sums of Squares", " ", "Degrees of Freedom", " ", "Mean Square", " ", "F-value", " ", "p-value")
    colnames(ivsanova) <- head


    for (i in 1:(dim(ivsanova)[1] - 1)) {
        if (temp2[i, 5] < 0.001) {
            #STB Mar 2011 - formatting p-values p<0.0010
            #			ivsanova[i,9]<-0.001
            ivsanova[i, 9] = format(round(0.001, 3), nsmall = 3, scientific = FALSE)
            ivsanova[i, 9] <- paste("<", ivsanova[i, 9])
        }
    }

    HTML(ivsanova, classfirstline = "second", align = "left")

    if (primFactor != "NULL") {
        HTML.title("</bf> ", HR = 2, align = "left")
        HTML.title("<bf>Comment: ANCOVA table calculated using a Type I model fit, see Armitage et al. (2001).", HR = 0, align = "left")
    } else {
        HTML.title("</bf> ", HR = 2, align = "left")
        HTML.title("<bf>Comment: ANOVA table calculated using a Type I model fit, see Armitage et al. (2001).", HR = 0, align = "left")
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

            if (primFactor != "NULL") {
                add <- paste(add, ": There are no statistically significant overall differences between the levels of any of the effects in the ANCOVA table.", sep = "")
            } else {
                add <- paste(add, ": There are no statistically significant overall differences between the levels of any of the effects in the ANOVA table.", sep = "")
            }
        } else {
            add <- paste(add, ": There is no statistically significant overall difference between the levels of the treatment effect.", sep = "")
        }
    } else {
        add <- paste(add, ". ", sep = "")
    }

    HTML.title("</bf> ", HR = 2, align = "left")
    HTML.title(add, HR = 0, align = "left")
    HTML.title("</bf> ", HR = 2, align = "left")
    if (primFactor != "NULL") {
        HTML.title("<bf> Tip: While it is a good idea to consider the overall tests in the ANCOVA table, we should not rely on them when deciding whether or not to make pairwise comparisons between the factor levels.", HR = 0, align = "left")
    } else {
        HTML.title("<bf> Tip: While it is a good idea to consider the overall tests in the ANOVA table, we should not rely on them when deciding whether or not to make pairwise comparisons between the factor levels.", HR = 0, align = "left")
    }
}

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
    jpeg(residualPlot)

    residplot <- data.frame(cbind(predict(threewayfull), rstudent(threewayfull)))
    colnames(residplot) <- c("Predicted", "Standardized_residuals")
    plot(Standardized_residuals ~ Predicted, data = residplot, ylab = "Externally Studentized residuals", xlab = "Predicted values", main = "Predicted vs. residuals plot")
    abline(a = 0, b = 0)
    abline(a = 2, b = 0, col = "red", lty = 3)
    abline(a = 3, b = 0, col = "red")
    abline(a = -2, b = 0, col = "red", lty = 3)
    abline(a = -3, b = 0, col = "red")

    abline(h = 0, col = "red", lty = "dotted")
    void <- HTMLInsertGraph(GraphFileName = sub("[A-Z0-9a-z,:,\\\\]*App_Data[\\\\]", "", residualPlot), Align = "centre")
    HTML.title("<bf> ", HR = 2, align = "left")
    HTML.title("<bf>Tip: On this plot look to see if the spread of the points increases as the predicted values increase. If so the response may need transforming.", HR = 0, align = "left")
    HTML.title("<bf> ", HR = 2, align = "left")
    HTML.title("<bf>Tip: Any observation with a residual less than -3 or greater than 3 (SD) should be investigated as a possible outlier.", HR = 0, align = "left")
}

#Normality plots
if (showNormPlot == "Y") {
    HTMLbr()
    normPlot <- sub(".html", "normplot.jpg", htmlFile)
    jpeg(normPlot)
    qqnorm(resid(threewayfull), main = "Normal probability plot")
    qqline(resid(threewayfull), col = "red", lty = "dotted")
    void <- HTMLInsertGraph(GraphFileName = sub("[A-Z0-9a-z,:,\\\\]*App_Data[\\\\]", "", normPlot), Align = "left")
    HTML.title("<bf> ", HR = 2, align = "left")
    HTML.title("<bf>Tip: Check that the points lie along the dotted line. If not then the data may be non-normally distributed.", HR = 0, align = "left")
}

# Means and Planned comparisons on the main effects
if (showLSMeans == "Y") {
    #Code to calculate y-axis offset (lens) in LSMeans plot
    names <- levels(statdata$mainEffect)
    index <- 1
    for (i in 1:length(names)) {
        temp <- names[i]
        temp <- as.character(unlist(strsplit(as.character(names[i]), "")))
        lens <- length(temp)
        if (lens > index) {
            index <- lens
        }
    }

    HTMLbr()
    CITitle <- paste("<bf>Plot of the predicted means with ", (sig * 100), "% confidence intervals", sep = "")
    HTML.title(CITitle, HR = 2, align = "left")

    mult <- glht(lm(effectModel, data = statdata, na.action = na.omit), linfct = mcp(mainEffect = "Means"))
    meanPlot <- sub(".html", "meanplot.jpg", htmlFile)
    jpeg(meanPlot)

    tabs <- confint(mult, level = sig, calpha = univariate_calpha())
    test <- tabs$confint
    lengths <- length(unique(rownames(test)))
    tab <- matrix(data = NA, nrow = lengths, ncol = 3)

    tab1 <- c()
    for (i in 1:lengths) {
        tab1[i] <- test[i, 1]
    }

    tab2 <- c()
    for (i in 1:lengths) {
        tab2[i] <- test[i, 2]
    }
    tab3 <- c()
    for (i in 1:lengths) {
        tab3[i] <- test[i, 3]
    }

    tabs <- confint(mult, level = sig, calpha = univariate_calpha())
    test <- tabs$confint
    lengths <- length(unique(rownames(test)))
    tab <- matrix(data = NA, nrow = lengths, ncol = 3)

    for (i in 1:lengths) {
        for (j in 1:3) {
            tab[i, j] <- test[i, j]
        }
    }

    colnames(tab) <- c("Mean", paste("Lower ", (sig * 100), "% CI", sep = ""), paste("Upper ", (sig * 100), "% CI", sep = ""))

    ta <- format(round(tab, 3), nsmall = 3)

    header <- c(" ", " ", " ")
    tables <- rbind(header, ta)

    rownames(tables) <- c("Level", rownames(test))



    #Code for LS MEans plot
    telly <- data.frame(tab)
    telly2 <- data.frame(rownames(test))
    telly <- data.frame(telly, telly2)
    nametemp <- c("Mean", "Lower", "Upper", "Group")
    colnames(telly) <- nametemp
    tmp1 <- split(telly$Mean, telly$Group)
    meanss <- sapply(tmp1, mean)
    tmp2 <- split(telly$Lower, telly$Group)
    lowerss <- sapply(tmp2, mean)
    tmp3 <- split(telly$Upper, telly$Group)
    upperss <- sapply(tmp3, mean)


    par(mar = c((lens / 2 + 2), 5, 1, 2), las = 1)
    plotCI(x = meanss, uiw = 1, liw = 1, li = lowerss, ui = upperss, xaxt = "n", xlab = " ", ylab = YAxisTitle, col = "black", barcol = "black")
    par(las = 2)
    axis(side = 1, at = 1:length(rownames(test)), cex = 0.7, labels = names(tmp1))
    void <- HTMLInsertGraph(GraphFileName = sub("[A-Z0-9a-z,:,\\\\]*App_Data[\\\\]", "", meanPlot), Align = "left")


    HTMLbr()
    CITitle2 <- paste("<bf>Table of the predicted means with ", (sig * 100), "% confidence intervals", sep = "")
    HTML.title(CITitle2, HR = 2, align = "left")


    HTML(tables, classfirstline = "second", align = "left")

    if (length(grep("\\*", effectModel)) == 0 && length(grep("\\+", effectModel)) == 0 && length(grep("\\+", model)) == 1) {
        add2 <- paste(c("The effect selected involves all treatment factors."), " ", sep = "")
    } else if (length(grep("\\*", model)) == 0 && length(grep("\\+", effectModel)) == 0 && length(grep("\\+", model)) == 0) {
        add2 <- paste(c(" The effect selected involves the treatment factor."), " ", sep = "")
    } else if (noeffects > testeffects) # the model has an interaction (need to include max interaction comment)
    #	} else	if (length(grep("\\*", model)) == 1) # the model has an interaction (need to include max interaction comment)

    {
        add2 <- paste(c("Warning: It is not advisable to draw statistical inferences about an effect if there is a significant higher-order interaction 		involving that effect. In the above plot and table we have assumed that certain higher order interactions are not significant and have removed them from the statistical model, see log for 		more details."), " ", sep = "")
    }
}

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
} else if (allPairwiseTestText == "Benjamini-Hochberg") {
    allPairwiseTest = "BH"
}

#All pairwise tests

if (allPairwiseTest != "NULL") {
    if (allPairwiseTest == "none") {
        add <- paste(c("All pairwise comparisons without adjustment for multiplicity (LSD tests)"))
    } else {
        add <- paste(c("All pairwise comparisons using "))
        add <- paste(add, allPairwiseTestText, sep = "")
        add <- paste(add, "'s test", sep = "")
    }

    HTMLbr()
    HTML.title(add, HR = 2, align = "left")

    mult <- glht(lm(effectModel, data = statdata, na.action = na.omit), linfct = mcp(mainEffect = "Tukey"))
    multci <- confint(mult, level = sig, calpha = univariate_calpha())

    if (allPairwiseTest == "Tukey") {
        multp <- summary(mult)
    } else {
        multp <- summary(mult, test = adjusted(allPairwiseTest))
    }
    pvals <- multp$test$pvalues
    sigma <- multp$test$sigma
    tablen <- length(unique(rownames(multci$confint)))
    tabs <- matrix(nrow = tablen, ncol = 5)

    for (i in 1:tablen) {
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
        tabs[i, 5] = format(round(pvals[i], 3), nsmall = 3, scientific = FALSE)
    }

    for (i in 1:tablen) {
        if (pvals[i] < 0.001) {
            #STB March 2011 - formatting p-values p<0.001
            tabs[i, 5] <- 0.001
            tabs[i, 5] = format(round(0.001, 3), nsmall = 3, scientific = FALSE)
            tabs[i, 5] <- paste("<", tabs[i, 5])
        }
    }

    header <- c(" ", " ", " ", " ", " ")
    tabls <- rbind(header, tabs)

    rows <- rownames(multci$confint)
    rows <- sub(" - ", " vs. ", rows, fixed = TRUE)
    rownames(tabls) <- c("Comparison", rows)
    lowerCI <- paste("   Lower ", (sig * 100), "% CI   ", sep = "")
    upperCI <- paste("   Upper ", (sig * 100), "% CI   ", sep = "")
    colnames(tabls) <- c("   Difference   ", lowerCI, upperCI, "   SEM   ", "   p-value   ")

    HTML(tabls, classfirstline = "second", align = "left")

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

    if (allPairwiseTest == "Tukey") {
        HTML.title("<bf> ", HR = 2, align = "left")
        HTML.title("<bf>Warning: The results of Tukey's test are approximate if the sample sizes are not equal.", HR = 0, align = "left")
    }

    if (length(grep("\\*", effectModel)) == 0 && length(grep("\\+", effectModel)) == 0 && length(grep("\\+", model)) == 1) {
        add2 <- paste(c(" "), " ", sep = "")

        HTML.title("<bf> ", HR = 2, align = "left")
        HTML.title(add2, HR = 0, align = "left")

    } else if (length(grep("\\*", model)) == 0 && length(grep("\\+", effectModel)) == 0 && length(grep("\\+", model)) == 0) {
        add2 <- paste(c(" "), " ", sep = "")
        HTML.title("<bf> ", HR = 2, align = "left")
        HTML.title(add2, HR = 0, align = "left")
        #	} else	if (length(grep("\\*", model)) == 1) # the model has an interaction (need to include max interaction comment)
    } else if (noeffects > testeffects) # the model has an interaction (need to include max interaction comment)
    {
        add2 <- paste(c("Warning: It is not advisable to draw statistical inferences about an effect if there is a significant higher-order interaction 		involving that effect. In the above table we have assumed that certain higher order interactions are not significant and have removed them from the statistical model, see log for more details."), " ", sep = "")
        HTML.title("<bf> ", HR = 2, align = "left")
        HTML.title(add2, HR = 0, align = "left")
    }

    if (tablen > 1) {
        if (allPairwiseTest == "none") {
            HTML.title("<bf> ", HR = 2, align = "left")
            HTML.title("<bf>Warning: As these tests are not adjusted for multiplicity there is a risk of generating false positive results. Only use the pairwise tests you planned to make a-priori, these are the so called Planned Comparisons, see Snedecor and Cochran (1989).", HR = 0, align = "left")
        } else {
            HTML.title("<bf> ", HR = 2, align = "left")
            HTML.title("<bf>Warning: This test makes an adjustment assuming you want to make all pairwise comparisons. If this is not the case then these tests may be unduly conservative. You may wish to use Planned Comparisons (using unadjusted p-values) instead, see Snedecor and Cochran (1989), or make a manual adjustment to the unadjusted p-values using the P-value Adjustment module.", HR = 0, align = "left")
        }
    }

    if (allPairwiseTest != "none") {
        HTML.title("<bf> ", HR = 2, align = "left")
        HTML.title("<bf>Note: The confidence intervals quoted are not adjusted for multiplicity.", HR = 0, align = "left")
    }
}

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

#All to one comparisons
if (backToControlTest != "NULL") {
    length <- length(statdata$mainEffect)
    newcontrs <- matrix(nrow = length, ncol = 1, data = "########")
    mainEffect2 <- matrix(nrow = length, ncol = 1, data = "########")

    for (i in 1:length) {
        if (statdata$mainEffect[i] == cntrlGroup) {
            mainEffect2[i] <- paste(newcontrs[i, 1], " ", statdata$mainEffect[i])
        } else {
            mainEffect2[i] <- paste(statdata$mainEffect[i])
        }
    }
    statdata <- cbind(statdata, mainEffect2)
    statdata$mainEffect <- statdata$mainEffect2

    if (backToControlTest == "none") {
        add <- paste(c("All to one comparisons without adjustment for multiplicity (LSD tests)"))
    } else {
        add <- paste(c("All to one comparisons using "))
        add <- paste(add, backToControlTestText, sep = "")
        add <- paste(add, "'s test", sep = "")
    }

    HTMLbr()
    HTML.title("</bf> ", HR = 2, align = "left")
    HTML.title(add, HR = 2, align = "left")

    if (backToControlTest == "Dunnett") {
        ntrgps <- length(unique(statdata$mainEffect)) - 1

        if (ntrgps != 1) {
            #Dunnetts code
            # remove blank rows form the data, then calculate number of groups
            ntrgps <- length(unique(statdata$mainEffect)) - 1
            nallgps <- length(unique(statdata$mainEffect))

            #calculate the sample sizes
            samplesize <- c(1:nallgps)
            for (i in 1:nallgps) {
                samplesize[i] <- sum(statdata$mainEffect == levels(statdata$mainEffect)[i])
            }

            # calculate the total number of obs and the DF for the Dunnetts test (get this from ANOVA?)
            totalobs <- sum(samplesize)
            dfree <- df.residual(lm(effectModel, data = statdata, na.action = na.omit))

            # calculation of correlation coefficient (according to Dunnet)
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

            mult <- glht(lm(effectModel, data = statdata, na.action = na.omit), linfct = mcp(mainEffect = "Dunnet"))
            multci <- confint(mult, level = sig, calpha = univariate_calpha())
            multp <- summary(mult, test = adjusted("none"))
            pvals <- multp$test$pvalues
            tstats <- multp$test$tstat
            sigma <- multp$test$sigma
            tablen <- length(unique(rownames(multci$confint)))
            tabs <- matrix(nrow = tablen, ncol = 5)

            #Calculate p-value
            dunnett <- function(data, group) {
                pdunnett <- function(x, nallgps, dfree, cormat) {
                    1 - pmvt(lower = -x, upper = x, delta = numeric(nallgps - 1), df = dfree, corr = cormat, abseps = 0.00000001)
                }
                t <- Mod(as.numeric(multp$test$tstat))
                p <- sapply(t, pdunnett, nallgps, dfree, cormat)
                result <- cbind(t, p)
                rownames(result) <- paste(1, 2:nallgps, sep = ":")
                return(result)
            }

            dunnp <- dunnett(eval(parse(text = paste("statdata$", resp))), statdata$mainEffect)

            for (q in 1:tablen) {
                pvals[q] <- format(round(dunnp[ntrgps + q], 3), nsmall = 3, scientific = FALSE)
            }

            for (i in 1:tablen) {
                tabs[i, 1] = format(round(multci$confint[i], 3), nsmall = 3, scientific = FALSE)
            }

            #calculating the adjusted confidence intervals


            mult <- glht(lm(effectModel, data = statdata, na.action = na.omit), linfct = mcp(mainEffect = "Dunnett"))
            multci <- confint(mult, level = sig, calpha = univariate_calpha())



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
                tabs[i, 5] = pvals[i]
            }

            for (i in 1:tablen) {
                if (pvals[i] < 0.001) {
                    #STB March 2011 formatting p-values p<0.0010
                    #					tabs[i,5]<-0.001
                    tabs[i, 5] = format(round(0.001, 3), nsmall = 3, scientific = FALSE)
                    tabs[i, 5] <- paste("<", tabs[i, 5])
                }
            }

            header <- c(" ", " ", " ", " ", " ")
            tabls <- rbind(header, tabs)

            rowna <- rownames(multci$confint)
            rowna <- sub("########", "", rowna, fixed = TRUE)

            rowna <- sub(" - ", " vs. ", rowna, fixed = TRUE)
            rownames(tabls) <- c("Comparison", rowna)

            lowerCI <- paste("   Lower ", (sig * 100), "% CI   ", sep = "")
            upperCI <- paste("   Upper ", (sig * 100), "% CI   ", sep = "")
            colnames(tabls) <- c("   Difference   ", lowerCI, upperCI, "   SEM   ", "   P-value   ")
        }
        else if (ntrgps == 1) {
            mult <- glht(lm(effectModel, data = statdata, na.action = na.omit), linfct = mcp(mainEffect = "Dunnett"))
            multci <- confint(mult, level = sig, calpha = univariate_calpha())
            multp <- summary(mult, test = adjusted("none"))

            pvals <- multp$test$pvalues
            sigma <- multp$test$sigma
            tablen <- length(unique(rownames(multci$confint)))
            tabs <- matrix(nrow = tablen, ncol = 5)

            for (i in 1:tablen) {
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
                tabs[i, 5] = format(round(pvals[i], 3), nsmall = 3, scientific = FALSE)
            }

            for (i in 1:tablen) {
                if (pvals[i] < 0.001) {
                    #STB March 2011 - formattign p-values p<0.0010
                    #					tabs[i,5]<-0.001
                    tabs[i, 5] = format(round(0.001, 3), nsmall = 3, scientific = FALSE)
                    tabs[i, 5] <- paste("<", tabs[i, 5])
                }
            }

            header <- c(" ", " ", " ", " ", " ")
            tabls <- rbind(header, tabs)

            rowna <- rownames(multci$confint)
            rowna <- sub("########", "", rowna, fixed = TRUE)

            rowna <- sub(" - ", " vs. ", rowna, fixed = TRUE)
            rownames(tabls) <- c("Comparison", rowna)
            lowerCI <- paste("   Lower ", (sig * 100), "% CI   ", sep = "")
            upperCI <- paste("   Upper ", (sig * 100), "% CI   ", sep = "")
            colnames(tabls) <- c("   Difference   ", lowerCI, upperCI, "   SEM   ", "   P-value   ")
        }
    } else {
        mult <- glht(lm(effectModel, data = statdata, na.action = na.omit), linfct = mcp(mainEffect = "Dunnett"))
        multci <- confint(mult, level = sig, calpha = univariate_calpha())
        multp <- summary(mult, test = adjusted(backToControlTest))

        pvals <- multp$test$pvalues
        sigma <- multp$test$sigma
        tablen <- length(unique(rownames(multci$confint)))

        tabs <- matrix(nrow = tablen, ncol = 5)

        for (i in 1:tablen) {
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
            tabs[i, 5] = format(round(pvals[i], 3), nsmall = 3, scientific = FALSE)
        }

        for (i in 1:tablen) {
            if (pvals[i] < 0.001) {
                #STB March 2011 formatting p-values p<0.0010
                #				tabs[i,5]<-0.001
                tabs[i, 5] = format(round(0.001, 3), nsmall = 3, scientific = FALSE)
                tabs[i, 5] <- paste("<", tabs[i, 5])
            }
        }

        header <- c(" ", " ", " ", " ", " ")
        tabls <- rbind(header, tabs)

        rowna <- rownames(multci$confint)
        rowna <- sub("########", "", rowna, fixed = TRUE)
        rowna <- sub(" - ", " vs. ", rowna, fixed = TRUE)

        rownames(tabls) <- c("Comparison", rowna)
        lowerCI <- paste("   Lower ", (sig * 100), "% CI   ", sep = "")
        upperCI <- paste("   Upper ", (sig * 100), "% CI   ", sep = "")
        colnames(tabls) <- c("   Difference   ", lowerCI, upperCI, "   SEM   ", "   P-value   ")
    }

    HTML(tabls, classfirstline = "second", align = "left")

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
    #	if (length(grep("\\*", model)) == 1) # the model has an interaction (need to include max interaction comment)
    {
        add2 <- paste(c("Warning: It is not advisable to draw statistical inferences about a main effect if there is a significant higher-order interaction involving that effect. In the above table we have assumed that certain higher order interactions are not significant, see log for more details."), " ", sep = "")
        HTML.title("<bf> ", HR = 2, align = "left")
        HTML.title(add2, HR = 0, align = "left")
    }
    if (tablen > 1) {
        if (backToControlTest == "none") {
            HTML.title("<bf> ", HR = 2, align = "left")
            HTML.title("<bf>Warning: As these tests are not adjusted for multiplicity there is a risk of generating false positive results. Only use the pairwise tests you planned to make a-priori, these are the so called Planned Comparisons, see Snedecor and Cochran (1989).", HR = 0, align = "left")
        }
    }
    if (backToControlTest != "none") {
        HTML.title("<bf> ", HR = 2, align = "left")
        HTML.title("<bf>Note: The confidence intervals quoted are not adjusted for multiplicity.", HR = 0, align = "left")
    }
}

#Analysis description

HTMLbr()
HTML.title("<bf>Analysis description", HR = 2, align = "left")

add <- c("The data were analysed using a ")

if (notreatfactors == 1) {

    if (primFactor != "NULL") {
        add <- paste(add, "1-way ANCOVA approach, with treatment factor ", sep = "")
        add <- paste(add, treatFactors, sep = "")
    } else {
        add <- paste(add, "1-way ANOVA approach, with treatment factor ", sep = "")
        add <- paste(add, treatFactors, sep = "")
    }
} else {
    add <- paste(add, notreatfactors, sep = "")
    if (primFactor != "NULL") {
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

if (blockFactors != "NULL" && primFactor != "NULL") {
    add <- paste(add, ", ", sep = "")
} else if (noblockfactors == 1 && blockFactors != "NULL" && primFactor == "NULL") {
    add <- paste(add, " and ", sep = "")
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

if (primFactor == "NULL") {
    add <- paste(add, ". ", sep = "")
} else if (primFactor != "NULL") {
    add <- paste(add, " and  ", sep = "")
    add <- paste(add, unlist(strsplit(covariateModel, "~"))[2], sep = "")
    add <- paste(add, " as the covariate. ", sep = "")
}

if (allPairwiseTest == "none" | backToControlTest == "none") {
    add <- paste(add, "This was followed by Planned Comparisons on the predicted means to compare the levels of the selected effect. ", sep = "")
}

if (backToControlTest != "NULL" & backToControlTest != "none") {
    add <- paste(add, "This was followed by comparisons of the predicted means back to the control mean using the ", sep = "")
    add <- paste(add, backToControlTestText, sep = "")
    add <- paste(add, " test. ", sep = "")
}

if (allPairwiseTest != "NULL" & allPairwiseTest != "none") {
    add <- paste(add, "This was followed by all pairwise comparisons between the predicted means of the selected effect using the ", sep = "")
    add <- paste(add, allPairwiseTestText, sep = "")
    add <- paste(add, " test. ", sep = "")
}

if (responseTransform != "None") {
    add <- paste(add, " The response was ", sep = "")
    add <- paste(add, responseTransform, sep = "")
    add <- paste(add, " transformed prior to analysis to stabilise the variance. ", sep = "")
}

HTML.title("</bf> ", HR = 2, align = "left")
HTML.title(add, HR = 0, align = "left")

#References

HTMLbr()
HTML.title("<bf>Statistical references", HR = 2, align = "left")


if (showANOVA == "Y") {
    HTML.title("<bf> ", HR = 2, align = "left")
    HTML.title("<bf> Armitage P, Matthews JNS and Berry G. (2001). Statistical Methods in Medical Research. 4th edition; John Wiley & Sons. New York.", HR = 0, align = "left")
}

if (allPairwiseTest == "BH" | backToControlTest == "BH") {
    HTML.title("<bf> ", HR = 2, align = "left")
    HTML.title("<bf>Benjamini Y and Hochberg Y. (1995). Controlling the false discovery rate: a practical and powerful approach to multiple testing. 	Journal of the Royal Statistical Society Series B, 57, 289-300. ", HR = 0, align = "left")
}

if (allPairwiseTest == "bonferroni" | backToControlTest == "bonferroni") {
    HTML.title("<bf> ", HR = 2, align = "left")
    HTML.title("<bf>Bonferroni CE (1936). Teoria statistica delle classi e calcolo delle probabilita.  Pubblicazioni del R Istituto Superiore di Scienze 	Economiche e Commerciali di Firenze, 8, 3-62.", HR = 0, align = "left")
}

if (allPairwiseTest == "Tukey") {
    HTML.title("<bf> ", HR = 2, align = "left")
    HTML.title("<bf>Braun HI, ed (1994). The collected works of John W. Tukey. Vol. VIII: Multiple comparisons:1948-1983. New York: Chapman and Hall.", HR = 0, align = "left")
}

if (backToControlTest == "Dunnett") {
    HTML.title("<bf> ", HR = 2, align = "left")
    HTML.title("<bf>Dunnett CW. (1955). A multiple comparison procedure for comparing several treatments with a control. Journal of the American 
	Statistical Association, 50, 1096-1121.", HR = 0, align = "left")
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

if (primFactor != "NULL") {
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
HTML.title("<bf>   R Development Core Team (2008). R: A language and environment for statistical computing. R Foundation for Statistical Computing, Vienna, Austria. ISBN 3-900051-07-0, URL http://www.R-project.org.", HR = 0, align = "left")

#mtvnorm
HTML.title("<bf> ", HR = 2, align = "left")
HTML.title("<bf>   Alan Genz, Frank Bretz, Torsten Hothorn with contributions by Tetsuhisa Miwa, Xuefei Mi, Friedrich Leisch and Fabian Scheipl	(2008). mvtnorm: Multivariate Normal and t Distributions. R package version 0.9-0.
	", HR = 0, align = "left")

#lattice
HTML.title("<bf> ", HR = 2, align = "left")
HTML.title("<bf> 
	Deepayan Sarkar (2009). lattice: Lattice Graphics. R package version 0.17-22. http://CRAN.R-project.org/package=lattice
	", HR = 0, align = "left")

#gplots
HTML.title("<bf> ", HR = 2, align = "left")
HTML.title("<bf>
	Gregory R. Warnes. Includes R source code and/or documentation contributed by (in alphabetical order): Ben Bolker, Lodewijk Bonebakker, Robert Gentleman, Wolfgang Huber, Andy Liaw, Thomas Lumley, Martin Maechler, Arni Magnusson, Steffen Moeller, Marc Schwartz and Bill Venables (2009). gplots: Various R programming tools for plotting data. R package version 2.7.1. http://CRAN.R-project.org/package=gplots
	", HR = 0, align = "left")

#gtools
HTML.title("<bf> ", HR = 2, align = "left")
HTML.title("<bf>
	Gregory R. Warnes. Includes R source code and/or documentation contributed by Ben Bolker and Thomas Lumley (2009). gtools: Various R programming tools. R package version 2.6.1. http://CRAN.R-project.org/package=gtools
	", HR = 0, align = "left")

#gdata
HTML.title("<bf> ", HR = 2, align = "left")
HTML.title("<bf>
	Gregory R. Warnes, Gregor Gorjanc. Includes R source code and/or documentation contributed by Ben Bolker and Thomas Lumley. (2008). gdata: Various R programming tools for data manipulation. R package version 2.4.2.
	", HR = 0, align = "left")
#caTools
HTML.title("<bf> ", HR = 2, align = "left")
HTML.title("<bf>
	Jarek Tuszynski (2008). caTools: Tools: moving window statistics, GIF, Base64, ROC AUC, etc.. R package version 1.9.
	", HR = 0, align = "left")

#car
HTML.title("<bf> ", HR = 2, align = "left")
HTML.title("<bf>   
John Fox. I am grateful to Douglas Bates, David Firth, Michael Friendly, Gregor Gorjanc, Spencer Graves, Richard Heiberger, Georges Monette, Henric Nilsson, Derek Ogle, Brian Ripley, Sanford Weisberg, and Achim Zeleis for various suggestions and contributions. (2008). car: Companion to Applied Regression. R package version 1.2-8. http://www.r-project.org, http://socserv.socsci.mcmaster.ca/jfox/
	", HR = 0, align = "left")

#nlme
HTML.title("<bf> ", HR = 2, align = "left")
HTML.title("<bf> 
	Jose Pinheiro, Douglas Bates, Saikat DebRoy, Deepayan Sarkar and the R Core team (2008). nlme: Linear and Nonlinear Mixed Effects Models. R package version 3.1-90.
	", HR = 0, align = "left")

#R2HTML
HTML.title("<bf> ", HR = 2, align = "left")
HTML.title("<bf>
	Lecoutre, Eric (2003). The R2HTML Package. R News, Vol 3. N. 3, Vienna, Austria.
	", HR = 0, align = "left")

#Contrast
HTML.title("<bf> ", HR = 2, align = "left")
HTML.title("<bf>   
	Max Kuhn, contributions from Steve Weston, Jed Wing and James Forester (2009). contrast: A collection of contrast methods. R package version 0.9.
	", HR = 0, align = "left")

#bitops
HTML.title("<bf> ", HR = 2, align = "left")
HTML.title("<bf>
	S original by Steve Dutky initial R port, extensions by Martin Maechler. revised and modified by Steve Dutky (2009). bitops: Functions for Bitwise operations. R package version 1.0-4.1.
	", HR = 0, align = "left")

#Survival
HTML.title("<bf> ", HR = 2, align = "left")
HTML.title("<bf>
	Terry Therneau and original R port by Thomas Lumley (2009). survival: Survival analysis, including penalised likelihood.. R package version 2.35-4. http://CRAN.R-project.org/package=survival
	", HR = 0, align = "left")

#multcomp
HTML.title("<bf> ", HR = 2, align = "left")
HTML.title("<bf>    
	Torsten Hothorn, Frank Bretz and Peter Westfall with contributions by Richard M. Heiberger (2007). multcomp: Simultaneous Inference for	General Linear Hypotheses. R package version 0.991-8.
	", HR = 0, align = "left")






if (showdataset == "Y") {
    HTMLbr()
    HTML.title("<bf>Analysis dataset", HR = 2, align = "left")
    statdata2 <- subset(statdata, select = -c(mainEffect, scatterPlotColumn))

    if (backToControlTest != "NULL") {
        statdata2 <- subset(statdata2, select = -c(mainEffect2))
    }
    HTML(statdata2, classfirstline = "second", align = "left")
}










