#===================================================================================================================
#R Libraries

suppressWarnings(library(mvtnorm))
suppressWarnings(library(R2HTML))
#===================================================================================================================
# retrieve args

Args <- commandArgs(TRUE)

#Read in arguments
statdata <- read.csv(Args[3], header = TRUE, sep = ",")
response <- Args[4]
treatment <- Args[5]
block <- Args[6]
sig <- 1 - as.numeric(Args[7])
statstest <- Args[8]
contlevel <- Args[9]

#STB CC28 Oct 2011
statdata <- statdata[order(eval(parse(text = paste("statdata$", treatment)))),]

#V3.2 STB OCT2015
set.seed(5041975)

respVTreat <- paste(response, "~", treatment, sep = "")
leng <- length(levels(as.factor(eval(parse(text = paste("statdata$", treatment))))))
blank <- c(" ")
index <- 1

#Removing illegal charaters
YAxisTitle <- response
XAxisTitle <- treatment
for (i in 1:10) {
    YAxisTitle <- namereplace(YAxisTitle)
    XAxisTitle <- namereplace(XAxisTitle)
}

#===================================================================================================================
#Setup the html file and associated css file

htmlFile <- sub(".csv", ".html", Args[3])
#determine the file name of the html file
HTMLSetFile(file = htmlFile)
.HTML.file = htmlFile
#===================================================================================================================
#Overall Graphical parameter setup

graphdata <- statdata
displaypointBOX <- "N"
#===================================================================================================================

#Checking the design is a complete block design for the Friedmans and Wilcoxon tests
RunFried <- "Y"
if (block != "NULL") {
    #Testing the design is a complete block design
    dim <- length(unique(eval(parse(text = paste("statdata$", treatment)))))
    dimblock <- length(unique(eval(parse(text = paste("statdata$", block)))))
    tempdata <- statdata[!(is.na(eval(parse(text = paste("statdata$", response))))),]
    tempdata$cat <- paste(eval(parse(text = paste("statdata$", treatment))), eval(parse(text = paste("statdata$", block))))
    if (dim * dimblock != length(unique(tempdata$cat))) {
        RunFried <- "Na"
    }

    #Testing to make sure there is only one replicate of each combination
    vectorN <- c(1:length(unique(tempdata$cat)))
    for (i in 1:length(unique(tempdata$cat))) {
        sub <- data.frame(subset(tempdata, tempdata$cat == unique(levels(as.factor(tempdata$cat)))[i]))
        vectorN[i] = dim(sub)[1]
        if (vectorN[i] >= 2) {
            RunFried <- "Nb"
        }
    }
}

#-------------------------------------------------------------------------------------------------------
#-------------------------------------------------------------------------------------------------------
#Titles and description
#-------------------------------------------------------------------------------------------------------
#-------------------------------------------------------------------------------------------------------
#Module Title
HTML.title("<bf>Non-Parametric Analysis", HR = 1, align = "left")

#Response title
title <- c("Response")
HTML.title(title, HR = 2, align = "left")

#Description
add <- paste(c("The  "), response, sep = "")
add <- paste(add, " response is currently being analysed by the Non-Parametric Analysis module", sep = "")

if (treatment != "NULL") {
    add <- paste(add, c(", with  "), sep = "")
    add <- paste(add, treatment, sep = "")
    add <- paste(add, " fitted as the treatment factor", sep = "")
}

if (block != "NULL") {
    add <- paste(add, c(" and  "), sep = "")
    add <- paste(add, block, sep = "")
    add <- paste(add, " fitted as the blocking factor", sep = "")
}

add <- paste(add, ".", sep = "")
HTML(add, align = "left")

#-------------------------------------------------------------------------------------------------------
#-------------------------------------------------------------------------------------------------------
#Summary stats table
#-------------------------------------------------------------------------------------------------------
#-------------------------------------------------------------------------------------------------------

#Setting up parameters
#Decimal places
DP <- 3

#Categorisation factor
statdata$catfact <- as.factor(eval(parse(text = paste("statdata$", treatment))))
lev <- length(unique(eval(parse(text = paste("statdata$", treatment)))))
sumtable <- matrix(data = NA, nrow = (lev), ncol = 6)
rownames(sumtable) <- c(unique(levels(statdata$catfact)))

#Add entries to the table
for (i in 1:lev) {
    tempdata <- subset(statdata, statdata$catfact == unique(levels(as.factor(statdata$catfact)))[i])
    sumtable[(i), 1] <- c(unique(levels(statdata$catfact)))[i]
    sumtable[(i), 2] <- format(round(min(eval(parse(text = paste("tempdata$", response))), na.rm = TRUE), DP), nsmall = DP, scientific = FALSE)
    sumtable[(i), 3] <- format(round(boxplot.stats(split(eval(parse(text = paste("tempdata$", response))), tempdata$catfact)[[i]])$stats[2], DP), nsmall = DP, scientific = FALSE)
    sumtable[(i), 4] <- format(round(boxplot.stats(split(eval(parse(text = paste("tempdata$", response))), tempdata$catfact)[[i]])$stats[3], DP), nsmall = DP, scientific = FALSE)
    sumtable[(i), 5] <- format(round(boxplot.stats(split(eval(parse(text = paste("tempdata$", response))), tempdata$catfact)[[i]])$stats[4], DP), nsmall = DP, scientific = FALSE)
    sumtable[(i), 6] <- format(round(max(eval(parse(text = paste("tempdata$", response))), na.rm = TRUE), DP), nsmall = DP, scientific = FALSE)
}

table <- data.frame(sumtable)
colnames(table) <- c("Group", "Min", "Lower quartile", "Median", "Upper quartile", "Max")

HTML.title("<bf>Summary data", HR = 2, align = "left")
HTML(table, classfirstline = "second", align = "left", row.names = "FALSE")

#-------------------------------------------------------------------------------------------------------
#-------------------------------------------------------------------------------------------------------
#Boxplot of data
#-------------------------------------------------------------------------------------------------------
#-------------------------------------------------------------------------------------------------------

HTML.title("<bf>Box-plot", HR = 2, align = "left")

#Graphics parameter setup
graphdata$xvarrr_IVS_BP <- as.factor(eval(parse(text = paste("graphdata$", treatment))))
graphdata$yvarrr_IVS <- eval(parse(text = paste("graphdata$", response)))
MainTitle2 <- ""
BoxPlot <- "Y"
FirstCatFactor <- "NULL"
SecondCatFactor <- "NULL"
Outliers <- "Y"

#Creating outliers dataset for the boxplot
temp <- IVS_F_boxplot_outlier()
outlierdata <- temp$outlierdata
boxdata <- temp$boxdata
ymin <- temp$ymin
ymax <- temp$ymax
range <- temp$range

#Plot device setup
ncboxplot <- sub(".html", "ncboxplot.jpg", htmlFile)
jpeg(ncboxplot, width = jpegwidth, height = jpegheight, quality = 100)
plotFilepdfx <- sub(".html", "ncboxplot.pdf", htmlFile)
dev.control("enable")

#GGPLOT2 code
NONCAT_BOX()

#Output code
void <- HTMLInsertGraph(GraphFileName = sub("[A-Z0-9a-z,:,\\\\]*App_Data[\\\\]", "", ncboxplot), Align = "centre")
if (pdfout == "Y") {
    pdf(file = sub("[A-Z0-9a-z,:,\\\\]*App_Data[\\\\]", "", plotFilepdfx), height = pdfheight, width = pdfwidth)
    dev.set(2)
    dev.copy(which = 3)
    dev.off(2)
    dev.off(3)
    pdfFile_x <- sub("[A-Z0-9a-z,:,\\\\]*App_Data[\\\\]", "", plotFilepdfx)
    linkToPdfx <- paste("<a href=\"", pdfFile_x, "\">Click here to view the PDF of the box-plot</a>", sep = "")
    HTML(linkToPdfx)
}

#Footnote to box-plot
footnote <- c("On the box-plot the median is denoted by the horizontal line within the box. 
The box indicates the interquartile range, where the lower and upper quartiles are calculated using the type 2 method, see Hyndman and Fan (1996). 
The whiskers extend to the most extreme data point which is no more than 1.5 times the length of the box away from the box. 
Individual observations that lie outside the outlier range are included on the plot using circles.")
HTML("</bf>")
HTML(footnote, align = "left")

#-------------------------------------------------------------------------------------------------------
#-------------------------------------------------------------------------------------------------------
#Kruskall-Wallis and Mann Whitney
#-------------------------------------------------------------------------------------------------------
#-------------------------------------------------------------------------------------------------------

if (block == "NULL" && (statstest == "MannWhitney" || (statstest == "AllComparisons" && leng == 2) || (statstest == "CompareToControl" && leng == 2))) {
    #Kruskal Wallis and Mann Whitney
    dim <- length(unique(eval(parse(text = paste("statdata$", treatment)))))

    if (dim > 2) {
        HTML.title("<bf>Kruskal-Wallis test", HR = 2, align = "left")

        options <- (scipen = 20)
        kruskalOut <- kruskal.test(as.formula(respVTreat), data = statdata)

        #Reformat p-values
        pvalue <- format(round(kruskalOut$p.value, 4), nsmall = 4, scientific = FALSE)
        for (i in 1:(length(pvalue))) {
            if (kruskalOut$p.value[i] < 0.0001) {
                #STB March 2011 - formatting pvalue p<0.001
                # pvalue[i]<-0.0001
                pvalue[i] = format(round(0.0001, 4), nsmall = 4, scientific = FALSE)
                pvalue[i] <- paste("<", pvalue[i])
            }
        }

        Wstat <- format(round(kruskalOut$statistic, 2), nsmall = 2)
        df <- format(round(kruskalOut$parameter, 0), nsmall = 0)
        colname <- c(" ", "Test statistic", "Degrees of freedom", "p-value")
        rowname <- c("Test result")
        temptab <- cbind(rowname, Wstat, df, pvalue)
        temptable <- data.frame(temptab)
        colnames(temptable) <- colname
        row.names(temptable) <- rowname
        HTML(temptable, classfirstline = "second", align = "left", row.names = "FALSE")
    } else if (dim == 2) {
        HTML.title("<bf> Mann-Whitney test", HR = 2, align = "left")
        comm <- c("You have selected a factor with only two levels, hence a Mann-Whitney test, also know as a Wilcoxon rank sum test, has been used to analyse the data rather than a Kruskal-Wallis test.")
        HTML(comm, align = "left")

        wilcoxOut <- wilcox.test(as.formula(respVTreat), data = statdata, exact = TRUE, conf.int = TRUE, conf.level = sig)

        #Reformat p-values
        pvalue <- format(round(wilcoxOut$p.value, 4), nsmall = 4, scientific = FALSE)
        for (i in 1:(length(pvalue))) {
            if (wilcoxOut$p.value[i] < 0.0001) {
                #STB March 2011 formatting p-value p<0.0001
                pvalue[i] <- 0.0001
                pvalue[i] = format(round(0.0001, 4), nsmall = 4, scientific = FALSE)
                pvalue[i] <- paste("<", pvalue[i])
            }
        }

        Wstat <- format(round(wilcoxOut$statistic, 2), nsmall = 2)
        rowname <- c("Test result")
        temptab <- data.frame(cbind(rowname, Wstat, pvalue))
        colname <- c(" ", "Test statistic", "p-value")
        colnames(temptab) <- colname

        HTML(temptab, classfirstline = "second", align = "left", row.names = "FALSE")

        if (length(unique(eval(parse(text = paste("statdata$", response))))) == length((eval(parse(text = paste("statdata$", treatment)))))) {
            HTML("As there are no ties in the responses, the exact test result has been calculated.", align = "left")
        }

        if (length(unique(eval(parse(text = paste("statdata$", response))))) < length((eval(parse(text = paste("statdata$", treatment)))))) {
            HTML("As there are ties in some of the responses, the asymptotic test result has been calculated in these cases.", HR = 0, align = "left")
        }
    }
}



#-------------------------------------------------------------------------------------------------------
#-------------------------------------------------------------------------------------------------------
#Friedmans test
#-------------------------------------------------------------------------------------------------------
#-------------------------------------------------------------------------------------------------------

if (block != "NULL" && statstest == "MannWhitney") {
    #Friedmans test
    HTML.title("<bf>Friedman Rank Sum test", HR = 2, align = "left")

    if (RunFried == "Na") {
        HTML("As not all combinations of the treatment factor and the other design (blocking) factor are present in the design, Friedman's test cannot be performed using InVivoStat. An alternative approach is to analyse the data using the Single or Repeated Measures Parametric Analysis modules, applying the rank transformation to the responses. ", align = "left")
    }

    if (RunFried == "Nb") {
        HTML("As one of the treatment factor levels occurs multiple times with one of the other design (blocking) factor levels, Friedman's test cannot be performed using InVivoStat. An alternative approach is to analyse the data using the Single or Repeated Measures Parametric Analysis modules, applying the rank transformation to the responses. ", align = "left")
    }

    if (RunFried == "Y") {
        #	       	options <- (scipen = 20)
        FriedOut <- friedman.test(y = eval(parse(text = paste("statdata$", response))), groups = eval(parse(text = paste("statdata$", treatment))), blocks = eval(parse(text = paste("statdata$", block))))

        #Reformat p-values
        pvalue <- format(round(FriedOut$p.value, 4), nsmall = 4, scientific = FALSE)
        for (i in 1:(length(pvalue))) {
            if (FriedOut$p.value[i] < 0.0001) {
                #STB March 2011 - formatting pvalue p<0.001
                # pvalue[i]<-0.0001
                pvalue[i] = format(round(0.0001, 4), nsmall = 4, scientific = FALSE)
                pvalue[i] <- paste("<", pvalue[i])
            }
        }

        Wstat <- format(round(FriedOut$statistic, 2), nsmall = 2)
        df <- format(round(FriedOut$parameter, 0), nsmall = 0)
        colname <- c(" ", "Test statistic", "Degrees of freedom", "p-value")
        rowname <- c("Test result")
        temptab <- cbind(rowname, Wstat, df, pvalue)
        temptable <- data.frame(temptab)
        colnames(temptable) <- colname
        row.names(temptable) <- rowname
        HTML(temptable, classfirstline = "second", align = "left", row.names = "FALSE")
    }
}

#-------------------------------------------------------------------------------------------------------
#-------------------------------------------------------------------------------------------------------
#All comparisons tests - generating comparison labels
#-------------------------------------------------------------------------------------------------------
#-------------------------------------------------------------------------------------------------------

if (statstest == "AllComparisons") {
    #Generating comparison labels 
    y <- 1
    allgroup1 <- unique(eval(parse(text = paste("statdata$", treatment))))[1]
    for (i in 1:(length(unique(eval(parse(text = paste("statdata$", treatment))))) - 1)) {
        for (j in 1:(length(unique(eval(parse(text = paste("statdata$", treatment))))) - i)) {
            allgroup1[y] <- unique(eval(parse(text = paste("statdata$", treatment))))[i]
            y <- y + 1
        }
    }

    allgroup2 <- unique(eval(parse(text = paste("statdata$", treatment))))[1]
    z <- 1
    for (i in 1:(length(unique(eval(parse(text = paste("statdata$", treatment))))) - 1)) {
        for (j in 1:(length(unique(eval(parse(text = paste("statdata$", treatment))))) - i)) {
            allgroup2[z] <- unique(eval(parse(text = paste("statdata$", treatment))))[j + i]
            z <- z + 1
        }
    }
    allvs <- c("r")

    compno <- c(0, 1, 3, 6, 10, 15, 21, 28, 36, 45, 55, 66, 78, 91, 105, 120, 136, 153, 171, 190, 210, 231, 253, 276, 300, 325, 351, 378, 406, 435)
    int <- compno[length(unique(eval(parse(text = paste("statdata$", treatment)))))] + 1
    for (i in 1:(int - 1)) {
        allvs[i] <- "vs."
    }

    #Define comparion number
    good3 <- c("r")
    for (i in 1:int - 1) {
        good3[i] <- i
    }
}

#-------------------------------------------------------------------------------------------------------
#-------------------------------------------------------------------------------------------------------
#All comparisons tests - no blocks
#-------------------------------------------------------------------------------------------------------
#-------------------------------------------------------------------------------------------------------

if (block == "NULL" && (statstest == "AllComparisons" && leng >= 3)) {
    # Behrens Fisher test results
    HTML.title("<bf>All pairwise comparisons - Behrens Fisher tests", HR = 2, align = "left")

    dataset <- data.frame(var = eval(parse(text = paste("statdata$", response))), class = eval(parse(text = paste("statdata$", treatment))))
    allpair <- summary(npmc(dataset, df = 1, alpha = sig), type = "BF")

    #Results
    Comparison <- allpair$'Results of the multiple Behrens-Fisher-Test'[1]

    #P-values
    Pvalue <- format(round(allpair$'Results of the multiple Behrens-Fisher-Test'[6], 4), nsmall = 4, scientific = FALSE)
    Pvaluezzz <- format(round(allpair$'Results of the multiple Behrens-Fisher-Test'[6], 6), nsmall = 6, scientific = FALSE)

    temp5 <- cbind(good3, allgroup1, allvs, allgroup2, Pvalue)
    temp5zzz <- cbind(good3, allgroup1, allvs, allgroup2, Pvaluezzz)


    #Reformat p-values
    for (i in 1:(dim(temp5)[1])) {
        if (as.numeric(temp5zzz[i, 5]) < 0.0001) {
            #STB March 2011 - formatting p-value p<0.0001
            temp5[i, 5] = format(round(0.0001, 5), nsmall = 4, scientific = FALSE)
            temp5[i, 5] <- paste("<", temp5[i, 5])
        }
    }

    allpairtable <- data.frame(temp5)
    temp6 <- c("Comparison Number", "Gp 1", "vs.", "Gp 2", "p-value")
    colnames(allpairtable) <- temp6

    HTML(allpairtable, classfirstline = "second", align = "left", row.names = "FALSE")


    # Mann-Whitney all pairwise tests
    HTML.title("<bf>All pairwise comparisons - Asymptotic Mann-Whitney tests", HR = 2, align = "left")

    #Generate the dataset to make all pairwise comparisons
    for (s in 1:(length(unique(eval(parse(text = paste("statdata$", treatment))))) - 1)) {
        for (t in s + 1:(length(unique(eval(parse(text = paste("statdata$", treatment))))) - s)) {
            #seperate out groups
            data1 <- c(1)
            data1 <- split(eval(parse(text = paste("statdata$", response))), eval(parse(text = paste("statdata$", treatment))))[[s]]
            data2 <- c(1)
            data2 <- split(eval(parse(text = paste("statdata$", response))), eval(parse(text = paste("statdata$", treatment))))[[t]]
            resp <- c(data1, data2)

            #Set up the factor names column
            length1 <- length(data1)
            length2 <- length(data2)
            fact1 <- c("a")
            fact2 <- c("b")
            for (m in 1:length1) {
                fact1[m] <- unique(eval(parse(text = paste("statdata$", treatment))))[[s]]
            }
            for (n in 1:length2) {
                fact2[n] <- unique(eval(parse(text = paste("statdata$", treatment))))[[t]]
            }
            grou <- c(fact1, fact2)

            # combine the datasets
            comb1 <- cbind(grou, resp)
            finaldata <- data.frame(comb1)

            #Wilcoxon test
            wilcox <- wilcox.test(resp ~ grou, exact = FALSE)

            #Reformat p-values
            pv <- format(round(wilcox$p.value, 4), nsmall = 4, scientific = FALSE)
            pvzzz <- wilcox$p.value
            for (i in 1:(length(pv))) {
                if (pvzzz[i] < 0.0001) {
                    #STB March 2011 - formatting p-values p<0.0001
                    #pv[i]<-0.0001
                    pv[i] = format(round(0.0001, 4), nsmall = 4, scientific = FALSE)
                    pv[i] <- paste("<", pv[i])
                }
            }

            #merge results for all pairwise tests
            lines <- c(pv)
            blank <- rbind(blank, lines)
            index <- index + 1
        }
    }

    #create final table
    blank <- blank[-1,]
    tabletemp <- data.frame(good3, allgroup1, allvs, allgroup2, blank)
    temp16 <- c("Comparison Number", "Gp 1", "vs.", "Gp 2", "p-value")
    colnames(tabletemp) <- temp16

    HTML(tabletemp, classfirstline = "second", align = "left", row.names = "FALSE")
    HTML("Why are there two different tests presented? The Behrens Fisher tests are the recommended approach, although we still need to independently verify these results.", align = "left")
}



#-------------------------------------------------------------------------------------------------------
#-------------------------------------------------------------------------------------------------------
#All comparisons tests - blocks
#-------------------------------------------------------------------------------------------------------
#-------------------------------------------------------------------------------------------------------

if (block != "NULL" && statstest == "AllComparisons") {
    # Wilcoxon signed rank tests
    HTML.title("<bf>All pairwise comparisons - Wilcoxon signed rank tests", HR = 2, align = "left")

    if (RunFried == "Na") {
        HTML("As not all combinations of the treatment factor and the other design (blocking) factor are present in the design, all pairwise comparisons have not been performed. An alternative approach is to analyse the data using the Single or Repeated Measures Parametric Analysis modules, applying the rank transformation to the responses. ", align = "left")
    }

    if (RunFried == "Nb") {
        HTML("As one of the treatment factor levels occurs multiple times with one of the other design (blocking) factor levels, all pairwise comparisons have not been performed. An alternative approach is to analyse the data using the Single or Repeated Measures Parametric Analysis modules, applying the rank transformation to the responses. ", align = "left")
    }

    if (RunFried == "Y") {
        INDEX <- 1

        #Generate the dataset to make all pairwise comparisons
        for (s in 1:(length(unique(eval(parse(text = paste("statdata$", treatment))))) - 1)) {
            for (t in s + 1:(length(unique(eval(parse(text = paste("statdata$", treatment))))) - s)) {

                tempdata1 = subset(statdata, eval(parse(text = paste("statdata$", treatment))) == unique(eval(parse(text = paste("statdata$", treatment))))[s])
                tempdata2 = subset(statdata, eval(parse(text = paste("statdata$", treatment))) == unique(eval(parse(text = paste("statdata$", treatment))))[t])

                interdata <- matrix(, nrow = length(unique(eval(parse(text = paste("statdata$", block))))), ncol = 2)
                interdata <- data.frame(interdata)

                for (k in 1:(length(unique(eval(parse(text = paste("statdata$", block))))))) {

                    tempdata1x = subset(tempdata1, eval(parse(text = paste("tempdata1$", block))) == unique(eval(parse(text = paste("tempdata1$", block))))[k])
                    tempdata2x = subset(tempdata2, eval(parse(text = paste("tempdata2$", block))) == unique(eval(parse(text = paste("tempdata2$", block))))[k])

                    interdata[k, 1] <- eval(parse(text = paste("tempdata1x$", response)))[1]
                    interdata[k, 2] <- eval(parse(text = paste("tempdata2x$", response)))[1]
                }

                #Wilcoxon test

                wilcox <- wilcox.test(interdata$X1, interdata$X2, exact = TRUE, paired = TRUE, correct = TRUE)

                #Generating regarding exact tests
                interdataX3 <- interdata$X1 - interdata$X2
                if (length(unique(interdataX3)) < length((interdataX3))) {
                    INDEX <- INDEX + 1
                }


                #Reformat p-values
                pv <- format(round(wilcox$p.value, 4), nsmall = 4, scientific = FALSE)
                pvzzz <- wilcox$p.value
                for (i in 1:(length(pv))) {
                    if (pvzzz[i] < 0.0001) {
                        #STB March 2011 - formatting p-values p<0.0001
                        #pv[i]<-0.0001
                        pv[i] = format(round(0.0001, 4), nsmall = 4, scientific = FALSE)
                        pv[i] <- paste("<", pv[i])
                    }
                }

                #merge results for all pairwise tests
                lines <- c(pv)
                blank <- rbind(blank, lines)
                index <- index + 1
            }
        }

        #create final table
        blank <- blank[-1,]

        tabletemp <- data.frame(good3, allgroup1, allvs, allgroup2, blank)
        temp16 <- c("Comparison Number", "Gp 1", "vs.", "Gp 2", "p-value")
        colnames(tabletemp) <- temp16

        HTML(tabletemp, classfirstline = "second", align = "left", row.names = "FALSE")


        #Message regarding exact tests
        if (index == 1) {
            HTML("As there are no ties in the response differences, the exact test result has been calculated.", align = "left")
        } else {
            HTML("As there are ties in some of the response differences, the asymptotic test result has been calculated in these cases.", align = "left")
        }
    }
}

#-------------------------------------------------------------------------------------------------------
#-------------------------------------------------------------------------------------------------------
#Comparisons back to control
#-------------------------------------------------------------------------------------------------------
#-------------------------------------------------------------------------------------------------------

if (block != "NULL" && statstest == "CompareToControl") {
    HTML.title("<bf>All comparisons back to one", HR = 2, align = "left")
    HTML("The 'all comparisons back to one' is not available as an 'Other design (block)' factor has been selected. Please use the  'all pairwise comparisons' option to generate the required results.", align = "left")
}

if (block == "NULL" && (statstest == "CompareToControl" && leng >= 3)) {
    #Steels all to one comparison
    HTML.title("<bf>Steel's all comparisons back to one", HR = 2, align = "left")

    dataset <- data.frame(var = eval(parse(text = paste("statdata$", response))), class = eval(parse(text = paste("statdata$", treatment))))
    steel <- summary(npmc(dataset, df = 1, alpha = sig, control = contlevel), type = "Steels")

    #Generate group comparison labels
    group2 <- c("a")
    for (i in 1:(length(unique(eval(parse(text = paste("statdata$", treatment))))) - 1)) {
        group2[i] <- paste(contlevel)
    }
    vs <- c("r")
    for (i in 1:(length(unique(eval(parse(text = paste("statdata$", treatment))))) - 1)) {
        vs[i] <- "vs."
    }
    group1 <- unique(eval(parse(text = paste("statdata$", treatment))))
    group1 <- group1[group1 != contlevel]

    #Generating observation numbers
    good3 <- c("r")
    for (i in 1:length(unique(eval(parse(text = paste("statdata$", treatment))))) - 1) {
        good3[i] <- i
    }

    #P-values
    Pvalue <- format(round(steel$'Results of the multiple Steel-Test'[6], 4), nsmall = 4, scientific = FALSE)
    Pvaluezzz <- format(round(steel$'Results of the multiple Steel-Test'[6], 6), nsmall = 6, scientific = FALSE)

    temp5 <- cbind(good3, group1, vs, group2, Pvalue)
    temp5zzz <- cbind(good3, group1, vs, group2, Pvaluezzz)

    #Reformat p-values
    for (i in 1:(dim(temp5)[1])) {
        if (as.numeric(temp5zzz[i, 5]) < 0.0001) {
            #STB March 2011 formatting p-values <0.0001
            #temp5[i,5]<-0.0001
            temp5[i, 5] = format(round(0.0001, 5), nsmall = 4, scientific = FALSE)
            temp5[i, 5] <- paste("<", temp5[i, 5])
        }
    }

    steeltable2 <- data.frame(temp5)
    temp6 <- c("Comparison number", "Group", "vs.", "Group", "p-value")
    colnames(steeltable2) <- temp6

    HTML(steeltable2, classfirstline = "second", align = "left", row.names = "FALSE")
}

#-------------------------------------------------------------------------------------------------------
#-------------------------------------------------------------------------------------------------------
#Text for conclusions and comments 
#-------------------------------------------------------------------------------------------------------
#-------------------------------------------------------------------------------------------------------
Ref_list <- R_refs()

if (RunFried == "Y") {

    if (block == "NULL") {
        HTML.title("<bf>Analysis conclusions", HR = 2, align = "left")
    }
    if (block == "NULL" && (statstest == "MannWhitney" || (statstest == "AllComparisons" && leng == 2) || (statstest == "CompareToControl" && leng == 2))) {
        dim <- length(unique(eval(parse(text = paste("statdata$", treatment)))))

        if (dim > 2) {
            if (temptab[1, 4] <= (1 - sig)) {
                add <- paste(c("There is a statistically significant overall difference between the treatment groups at the "), sep = "")
                add <- paste(add, 100 * (1 - sig), sep = "")
                add <- paste(add, "% level of significance as the p-value is less than ", sep = "")
                add <- paste(add, 1 - sig, sep = "")
                add <- paste(add, " (Kruskal-Wallis test).", sep = "")
                HTML(add, align = "left")
            } else if (temptab[1, 4] > 0.05) {
                add <- paste(c("The overall difference between the treatment groups is not statistically significant at the "), sep = "")
                add <- paste(add, 100 * (1 - sig), sep = "")
                add <- paste(add, "% level of significance as the p-value is greater than ", sep = "")
                add <- paste(add, 1 - sig, sep = "")
                add <- paste(add, " (Kruskal-Wallis test).", sep = "")
                HTML(add, align = "left")
            }

            HTML.title("<bf>Analysis description", HR = 2, align = "left")
            HTML("The overall treatment effect was assessed using the non-parametric Kruskal-Wallis test, see Kruskal and Wallis (1952, 1953).", align = "left")
            HTML("Non-parametric tests should be used if the data is non-normally distributed, the variability is different between treatment groups or the responses are not continuous and numerical. ", align = "left")

            #Bate and Clark comment
            HTML(refxx, align = "left")

            HTML.title("<bf>Statistical references", HR = 2, align = "left")
            HTML(Ref_list$BateClark_ref, align = "left")
            HTML("Hyndman RJ and Fan Y. (1996). Sample quantiles in statistical packages. American Statistician 50, 361-365.", align = "left")
            HTML("Kruskal, WH and Wallis, WA (1952). Use of ranks in one criterion variance analysis. JASA, 47, 583-621.", align = "left")
            HTML("Kruskal, WH and Wallis, WA (1953). Errata for Kruskal-Wallis (1952). JASA, 48, 907-911.", align = "left")
        } else if (dim == 2) {
            if (wilcoxOut$p.value <= (1 - sig)) {
                add <- paste(c("The difference between the two treatment groups is statistically significant at the "), sep = "")
                add <- paste(add, 100 * (1 - sig), sep = "")
                add <- paste(add, "% level of significance as the p-value is less than ", sep = "")
                add <- paste(add, 1 - sig, sep = "")
                add <- paste(add, " (Mann-Whitney test).", sep = "")
                HTML(add, align = "left")
            } else if (wilcoxOut$p.value > 0.05) {
                add <- paste(c("The difference between the two treatment groups is not statistically significant at the "), sep = "")
                add <- paste(add, 100 * (1 - sig), sep = "")
                add <- paste(add, "% level of significance as the p-value is greater than ", sep = "")
                add <- paste(add, 1 - sig, sep = "")
                add <- paste(add, " (Mann-Whitney test).", sep = "")
                HTML(add, align = "left")
            }

            HTML.title("<bf>Analysis description", HR = 2, align = "left")
            HTML("The difference between the two treatments was assessed using the non-parametric Mann-Whitney test, see Wilcoxon (1945), Mann and Whitney (1947).", align = "left")
            HTML("Non-parametric tests should be used if the data is non-normally distributed, the variability is different between treatment groups or the responses are not continuous and numerical. ", align = "left")

            #Bate and Clark comment
            HTML.title(refxx, HR = 0, align = "left")

            HTML.title("Statistical references", HR = 2, align = "left")
            HTML(Ref_list$BateClark_ref, align = "left")
            HTML("Hyndman RJ and Fan Y. (1996). Sample quantiles in statistical packages. American Statistician 50, 361-365.", align = "left")
            HTML("Mann, HB and Whitney, DR (1947). On a test of whether one of two random variables is stochastically larger than the other. Annals of Mathematical Statistics, 18, 50-60.", align = "left")
            HTML("Wilcoxon, F (1945). Individual comparisons by ranking methods. Biometrics Bulletin, 1, 80-83.", align = "left")
        }
    }

    if (block != "NULL" && statstest == "MannWhitney") {
        dim <- length(unique(eval(parse(text = paste("statdata$", treatment)))))

        if (temptab[1, 4] <= (1 - sig)) {
            add <- paste(c("There is a statistically significant overall difference between the treatment groups at the "), sep = "")
            add <- paste(add, 100 * (1 - sig), sep = "")
            add <- paste(add, "% level of significance as the p-value is less than ", sep = "")
            add <- paste(add, 1 - sig, sep = "")
            add <- paste(add, " (Friedman test).", sep = "")
            HTML(add, align = "left")
        } else if (temptab[1, 4] > 0.05) {
            add <- paste(c("The overall difference between the treatment groups is not statistically significant at the "), sep = "")
            add <- paste(add, 100 * (1 - sig), sep = "")
            add <- paste(add, "% level of significance as the p-value is greater than ", sep = "")
            add <- paste(add, 1 - sig, sep = "")
            add <- paste(add, " (Friedman test).", sep = "")
            HTML(add, align = "left")
        }

        HTML.title("<bf>Analysis description", HR = 2, align = "left")
        HTML("The overall treatment effect was assessed using the non-parametric Friedman test, see Hollander and Wolfe (1973).", align = "left")
        HTML("Non-parametric tests should be used if the data is non-normally distributed, the variability is different between treatment groups or the responses are not continuous and numerical. ", align = "left")

        #Bate and Clark comment
        HTML(refxx, align = "left")

        HTML.title("<bf>Statistical references", HR = 2, align = "left")
        HTML(Ref_list$BateClark_ref, align = "left")
        HTML("Hyndman RJ and Fan Y. (1996). Sample quantiles in statistical packages. American Statistician 50, 361-365.", align = "left")
        HTML("Hollander, M and Wolfe DA (1973). Nonparametric Statistical Methods. New York: John Wiley & Sons.", align = "left")
    }


    if (block == "NULL" && (statstest == "AllComparisons" && leng >= 3)) {
        add <- c(" ")
        textindex = 1
        for (y in 1:(int - 1)) {
            if (allpairtable[y, 5] <= (1 - sig)) {
                add <- paste(add, "The difference between groups ", sep = "")
                add <- paste(add, allpairtable[y, 2], sep = "")
                add <- paste(add, " and ", sep = "")
                add <- paste(add, allpairtable[y, 4], sep = "")
                add <- paste(add, " is statistically significant at the ", sep = "")
                add <- paste(add, 100 * (1 - sig), sep = "")
                add <- paste(add, "% level of significance as the p-value is less than ", sep = "")
                add <- paste(add, 1 - sig, sep = "")
                add <- paste(add, " (Behrens Fisher test). ", sep = "")
                textindex <- textindex + 1
            }
        }
        textindex2 <- 1
        add2 <- c(" ")
        for (q in 1:(int - 1)) {
            if (blank[q] <= (1 - sig)) {
                add2 <- paste(add2, "The difference between groups ", sep = "")
                add2 <- paste(add2, tabletemp[q, 2], sep = "")
                add2 <- paste(add2, " and ", sep = "")
                add2 <- paste(add2, tabletemp[q, 4], sep = "")
                add2 <- paste(add2, " is statistically significant at the ", sep = "")
                add2 <- paste(add2, 100 * (1 - sig), sep = "")
                add2 <- paste(add2, "% level of significance as the p-value is less than ", sep = "")
                add2 <- paste(add2, 1 - sig, sep = "")
                add2 <- paste(add2, " (Mann-Whitney test). ", sep = "")
                textindex2 <- textindex2 + 1
            }
        }
        if (textindex == 1) {
            add3 <- c("None of the pairwise Behrens Fisher tests were statistically significant at the ")
            add3 <- paste(add3, 1 - sig, sep = "")
            add3 <- paste(add3, " level.", sep = "")
            HTML(add3, align = "left")
        } else if (textindex > 1) {
            HTML(add, align = "left")
        }

        if (textindex2 == 1) {
            add3 <- c("None of the pairwise Mann-Whitney tests were statistically significant at the ")
            add3 <- paste(add3, 1 - sig, sep = "")
            add3 <- paste(add3, " level.", sep = "")
            HTML(add3, align = "left")
        } else if (textindex2 > 1) {
            HTML(add2, align = "left")
        }

        HTML.title("<bf>Analysis description", HR = 2, align = "left")
        HTML("All pairwise differences between the treatments were assessed using Behrens Fisher tests, see Munzel and Hothorn (2001) and Mann-Whitney tests, see Mann and Whitney (1947).", align = "left")
        HTML("Non-parametric tests should be used if the data is non-normally distributed, the variability is different between treatment groups or the responses are not continuous and numerical. ", align = "left")

        #Bate and Clark comment
        HTML.title(refxx, HR = 0, align = "left")

        HTML.title("<bf>Statistical references", HR = 2, align = "left")
        HTML(Ref_list$BateClark_ref, align = "left")
        HTML("Hyndman RJ and Fan Y. (1996). Sample quantiles in statistical packages. American Statistician 50, 361-365.", align = "left")
        HTML("Mann, HB and Whitney, DR (1947). On a test of whether one of two random variables is stochastically larger than the other. Annals of Mathematical Statistics, 18, 50-60.", align = "left")
        HTML("Munzel, U and Hothorn, LA (2001). A unified approach to simultaneous rank test procedures in the unbalanced one-way layout. Biometrical Journal, 43(5) 553-569.", align = "left")
        HTML("Wilcoxon, F (1945). Individual comparisons by ranking methods. Biometrics Bulletin, 1, 80-83.", align = "left")
    }

    if (block != "NULL" && statstest == "AllComparisons") {
        textindex2 <- 1
        add2 <- c(" ")
        for (q in 1:(int - 1)) {
            if (blank[q] <= (1 - sig)) {
                add2 <- paste(add2, "The difference between groups ", sep = "")
                add2 <- paste(add2, tabletemp[q, 2], sep = "")
                add2 <- paste(add2, " and ", sep = "")
                add2 <- paste(add2, tabletemp[q, 4], sep = "")
                add2 <- paste(add2, " is statistically significant at the ", sep = "")
                add2 <- paste(add2, 100 * (1 - sig), sep = "")
                add2 <- paste(add2, "% level of significance as the p-value is less than ", sep = "")
                add2 <- paste(add2, 1 - sig, sep = "")
                add2 <- paste(add2, " (Wilcoxon Signed Rank test). ", sep = "")
                textindex2 <- textindex2 + 1
            }
        }

        if (textindex2 == 1) {
            add3 <- c("None of the pairwise Wilcoxon Signed Rank tests were statistically significant at the ")
            add3 <- paste(add3, 1 - sig, sep = "")
            add3 <- paste(add3, " level.", sep = "")
            HTML(add3, align = "left")
        } else if (textindex2 > 1) {
            HTML(add2, align = "left")
        }

        HTML.title("<bf>Analysis description", HR = 2, align = "left")
        HTML("All pairwise differences between the treatments were assessed using Wilcoxon Signed Rank tests, see Hollander and Wolfe (1973).", align = "left")
        HTML("Non-parametric tests should be used if the data is non-normally distributed, the variability is different between treatment groups or the responses are not continuous and numerical. ", align = "left")

        #Bate and Clark comment
        HTML.title(refxx, HR = 0, align = "left")

        HTML.title("<bf>Statistical references", HR = 2, align = "left")
        HTML(Ref_list$BateClark_ref, align = "left")
        HTML("Hyndman RJ and Fan Y. (1996). Sample quantiles in statistical packages. American Statistician 50, 361-365.", align = "left")
        HTML("Hollander, M and Wolfe DA (1973). Nonparametric Statistical Methods. New York: John Wiley & Sons.", align = "left")
    }

    if (block == "NULL" && (statstest == "CompareToControl" && leng >= 3)) {
        add <- c(" ")
        textindex = 1
        for (y in 1:(length(unique(eval(parse(text = paste("statdata$", treatment))))) - 1)) {
            if (steeltable2[y, 5] <= (1 - sig)) {
                add <- paste(add, "The difference between groups ", sep = "")
                add <- paste(add, steeltable2[y, 2], sep = "")
                add <- paste(add, " and ", sep = "")
                add <- paste(add, steeltable2[y, 4], sep = "")
                add <- paste(add, " is statistically significant at the ", sep = "")
                add <- paste(add, 100 * (1 - sig), sep = "")
                add <- paste(add, "% level of significance as the p-value is less than ", sep = "")
                add <- paste(add, 1 - sig, sep = "")
                add <- paste(add, " (Steel's test). ", sep = "")
                textindex <- textindex + 1
            }
        }
        if (textindex == 1) {
            HTML("None of the Steel's all to one comparisons were significant.", align = "left")
        } else if (textindex > 1) {
            HTML(add, align = "left")
        }

        HTML.title("Analysis description", HR = 2, align = "left")
        HTML("The comparison of treatment groups back to a single control group was made using the non-parametric Steel's test, see Steel (1959).", align = "left")
        HTML("Non-parametric tests should be used if the data is non-normally distributed, the variability is different between treatment groups or the responses are not continuous and numerical. ", align = "left")

        #Bate and Clark comment
        HTML.title(refxx, HR = 0, align = "left")

        HTML.title("<bf>Statistical references", HR = 2, align = "left")
        HTML(Ref_list$BateClark_ref, align = "left")
        HTML("Hyndman RJ and Fan Y. (1996). Sample quantiles in statistical packages. American Statistician 50, 361-365.", align = "left")
        HTML("Steel, RGD (1959). A multiple comparison rank sum test: treatments versus control. Biometrics, 15, 560-572. ", align = "left")
    }

    if (block != "NULL" && (statstest == "CompareToControl" && leng >= 3)) {
        HTML.title("<bf>Statistical references", HR = 2, align = "left")
        HTML("Hyndman RJ and Fan Y. (1996). Sample quantiles in statistical packages. American Statistician 50, 361-365.", align = "left")
    }


}

if (RunFried != "Y") {
    HTML.title("<bf>Statistical reference", HR = 2, align = "left")
    HTML("Hyndman RJ and Fan Y. (1996). Sample quantiles in statistical packages. American Statistician 50, 361-365.", align = "left")
}

#-------------------------------------------------------------------------------------------------------
#-------------------------------------------------------------------------------------------------------
#References 
#-------------------------------------------------------------------------------------------------------
#-------------------------------------------------------------------------------------------------------
HTML.title("R references", HR = 2, align = "left")

HTML(Ref_list$R_ref, align = "left")
HTML(Ref_list$mtvnorm_ref, align = "left")
HTML(Ref_list$GGally_ref, align = "left")
HTML(Ref_list$RColorBrewers_ref, align = "left")
HTML(Ref_list$GGPLot2_ref, align = "left")
HTML(Ref_list$reshape_ref, align = "left")
HTML(Ref_list$plyr_ref, align = "left")
HTML(Ref_list$scales_ref, align = "left")
HTML(Ref_list$NPMC_ref, align = "left")
HTML(Ref_list$R2HTML_ref, align = "left")
HTML(Ref_list$PROTO_ref, align = "left")


#----------------------------------------------------------------------------------------------------------
#Show dataset
#----------------------------------------------------------------------------------------------------------
if (showdataset == "Y") {
    statdata2 <- data.frame(eval(parse(text = paste("statdata$", response))))
    statdata3 <- data.frame(eval(parse(text = paste("statdata$", treatment))))

    if (block != "NULL") {
        statdata4 <- data.frame(eval(parse(text = paste("statdata$", block))))
        statdata2 <- cbind(statdata2, statdata3, statdata4)
        names <- c(response, treatment, block)
    } else {
        statdata2 <- cbind(statdata2, statdata3)
        names <- c(response, treatment)
    }

    observ <- c(1:dim(statdata2)[1])
    statdata2 <- cbind(observ, statdata2)
    colnames(statdata2) <- c("Observation", names)

    HTMLbr()
    HTML.title("Analysis dataset", HR = 2, align = "left")
    HTML(statdata2, classfirstline = "second", align = "left", row.names = "FALSE")
}






#----------------------------------------------------------------------------------------------------------
#Show arguments
#----------------------------------------------------------------------------------------------------------

HTML.title("Analysis options", HR = 2, align = "left")
HTML(paste("Response variable: ", response, sep = ""), align = "left")
HTML(paste("Grouping variable: ", treatment, sep = ""), align = "left")

if (block != "NULL") {
    HTML(paste("Other design (blocking) variable: ", block, sep = ""), align = "left")
}

if (statstest == "MannWhitney" && dim == 2) {
    HTML(paste("Statistical test: Mann-Whitney"), align = "left")
}

if (statstest == "MannWhitney" && dim >= 3) {
    HTML(paste("Statistical test: Kruskal-Wallis"), align = "left")
}

if (statstest != "MannWhitney") {
    HTML(paste("Statistical test: ", statstest, sep = ""), align = "left")
}

if (contlevel != "NULL" && statstest == "CompareToControl") {
    HTML(paste("Control group: ", contlevel, sep = ""), align = "left")
}

HTML(paste("Significance level: ", 1 - sig, sep = ""), align = "left")