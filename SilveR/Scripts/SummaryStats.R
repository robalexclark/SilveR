#===================================================================================================================
#R Libraries

suppressWarnings(library(R2HTML))
#===================================================================================================================

#===================================================================================================================
# retrieve args
Args <- commandArgs(TRUE)

#Read in data
statdata <- read.csv(Args[3], header = TRUE, sep = ",")

#Copy Args
csResponses <- Args[4]
transformation <- Args[5]
firstCat <- Args[6]
secondCat <- Args[7]
thirdCat <- Args[8]
fourthCat <- Args[9]
mean <- Args[10]
N <- Args[11]
StDev <- Args[12]
Variances <- Args[13]
StErr <- Args[14]
MinMax <- Args[15]
MedianQuartile <- Args[16]
CoeffVariation <- Args[17]
confidenceLimits <- Args[18]
CIval2 <- as.numeric(Args[19])
CIval <- CIval2 / 100
NormalProbabilityPlot <- Args[20]
ByCategoriesAndOverall <- Args[21]


csResponses
transformation
firstCat
secondCat
thirdCat
fourthCat
mean
N
StDev
Variances
StErr
MinMax
MedianQuartile
CoeffVariation
confidenceLimits
CIval2
NormalProbabilityPlot
ByCategoriesAndOverall

#===================================================================================================
#Graphics parameter setup

graphdata <- statdata
#===================================================================================================
#===================================================================================================

#Setup the html file and associated css file
htmlFile <- sub(".csv", ".html", Args[3]);
#determine the file name of the html file
HTMLSetFile(file = htmlFile)
.HTML.file = htmlFile


#Output HTML header
HTML.title("<bf>Summary Statistics", HR = 1, align = "left")
HTML.title("</bf> ", HR = 2, align = "left")


#Breakdown the list of responses

resplist <- c()
tempChanges <- strsplit(csResponses, ",")
expectedChanges <- c(0)
for (i in 1:length(tempChanges[[1]])) {
    expectedChanges[length(expectedChanges) + 1] = (tempChanges[[1]][i])
}
for (j in 1:(length(expectedChanges) - 1)) {
    resplist[j] = expectedChanges[j + 1]
}
resplength <- length(resplist)

# Create the normal probability plot titles
#STB - Sept 2010

resplistqq <- resplist


#replace illegal characters in variable names
for (i in 1:10) {
    resplistqq <- namereplace(resplistqq)
}


# Loop to run the analysis

#start of do loop x
#STB 06 September 2010
if (mean != "N" || N != "N" || StDev != "N" || Variances != "N" || StErr != "N" || MinMax != "N" || MedianQuartile != "N" || CoeffVariation != "N" || confidenceLimits != "N") {

    for (i in 1:resplength) {
        csResponses <- resplist[i]


        if (firstCat != "NULL" || secondCat != "NULL" || thirdCat != "NULL" || fourthCat != "NULL") {
            #Analysis if categorisation factors selected

            #Setting up parameters and vectors

            length <- length(unique(levels(as.factor(statdata$catfact))))
            tablenames <- c(levels(as.factor(statdata$catfact)))
            table <- c(1:length)
            for (i in 1:length) {
                table[i] = " "
            }
            vectormean <- c(1:length)
            vectorN <- c(1:length)
            vectorStDev <- c(1:length)
            vectorVariances <- c(1:length)
            vectorStErr <- c(1:length)
            vectorMin <- c(1:length)
            vectorMax <- c(1:length)
            vectorMedian <- c(1:length)
            vectorLQ <- c(1:length)
            vectorUQ <- c(1:length)
            vectorCoeffVariation <- c(1:length)
            vectorUCI <- c(1:length)
            vectorLCI <- c(1:length)

            for (i in 1:length) {
                sub <- subset(statdata, statdata$catfact == unique(levels(as.factor(statdata$catfact)))[i])
                sub2 <- data.frame(sub)
                if (mean == "Y") {
                    vectormean[i] = mean(eval(parse(text = paste("sub2$", csResponses))), na.rm = TRUE)
                }
                if (N == "Y") {
                    tempy <- na.omit(eval(parse(text = paste("sub2$", csResponses))))
                    vectorN[i] = length(tempy)
                }
                if (StDev == "Y") {
                    vectorStDev[i] = sd(eval(parse(text = paste("sub2$", csResponses))), na.rm = TRUE)
                }
                if (Variances == "Y") {
                    vectorVariances[i] = var(eval(parse(text = paste("sub2$", csResponses))), na.rm = TRUE)
                }
                if (StErr == "Y") {
                    tempy <- na.omit(eval(parse(text = paste("sub2$", csResponses))))
                    vectorStErr[i] = sd(eval(parse(text = paste("sub2$", csResponses))), na.rm = TRUE) / (length(tempy)) ** (0.5)
                }
                if (MinMax == "Y") {
                    vectorMin[i] = suppressWarnings(min(eval(parse(text = paste("sub2$", csResponses))), na.rm = TRUE))
                }
                if (MinMax == "Y") {
                    vectorMax[i] = suppressWarnings(max(eval(parse(text = paste("sub2$", csResponses))), na.rm = TRUE))
                }
                if (MedianQuartile == "Y") {
                    vectorMedian[i] = median(eval(parse(text = paste("sub2$", csResponses))), na.rm = TRUE)
                }
                if (MedianQuartile == "Y") {
                    vectorLQ[i] = quantile(eval(parse(text = paste("sub2$", csResponses))), 0.25, type = 2, na.rm = TRUE)
                }
                if (MedianQuartile == "Y") {
                    vectorUQ[i] = quantile(eval(parse(text = paste("sub2$", csResponses))), 0.75, type = 2, na.rm = TRUE)
                }
                if (CoeffVariation == "Y") {
                    vectorCoeffVariation[i] = 100 * sd(eval(parse(text = paste("sub2$", csResponses))), na.rm = TRUE) / mean(eval(parse(text = paste("sub2$", csResponses))), na.rm = TRUE)
                }
                if (confidenceLimits == "Y") {
                    tempy <- na.omit(eval(parse(text = paste("sub2$", csResponses))))
                    vectorLCI[i] = mean(eval(parse(text = paste("sub2$", csResponses))), na.rm = TRUE) - qt(1 - (1 - CIval) / 2, (length(tempy) - 1)) * sd(eval(parse(text = paste("sub2$", csResponses))), na.rm = TRUE) / (length(tempy)) ** (0.5)
                }
                if (confidenceLimits == "Y") {
                    tempy <- na.omit(eval(parse(text = paste("sub2$", csResponses))))
                    vectorUCI[i] = mean(eval(parse(text = paste("sub2$", csResponses))), na.rm = TRUE) + qt(1 - (1 - CIval) / 2, (length(tempy) - 1)) * sd(eval(parse(text = paste("sub2$", csResponses))), na.rm = TRUE) / (length(tempy)) ** (0.5)
                }
            }

            #Generating final table dataset

            if (mean == "Y") {
                vectormean <- format(round(vectormean, 4), nsmall = 4, scientific = FALSE)
                table <- cbind(table, vectormean)
            }
            if (N == "Y") {
                #vectorN<-format(round(vectormean, 4), nsmall=4, scientific=FALSE)
                table <- cbind(table, vectorN)
            }
            if (StDev == "Y") {
                vectorStDev <- format(round(vectorStDev, 4), nsmall = 4, scientific = FALSE)
                table <- cbind(table, vectorStDev)
            }
            if (Variances == "Y") {
                vectorVariances <- format(round(vectorVariances, 4), nsmall = 4, scientific = FALSE)
                table <- cbind(table, vectorVariances)
            }
            if (StErr == "Y") {
                vectorStErr <- format(round(vectorStErr, 4), nsmall = 4, scientific = FALSE)
                table <- cbind(table, vectorStErr)
            }
            if (MinMax == "Y") {
                vectorMin <- format(round(vectorMin, 4), nsmall = 4, scientific = FALSE)
                table <- cbind(table, vectorMin)
            }
            if (MinMax == "Y") {
                vectorMax <- format(round(vectorMax, 4), nsmall = 4, scientific = FALSE)
                table <- cbind(table, vectorMax)
            }
            if (MedianQuartile == "Y") {
                vectorMedian <- format(round(vectorMedian, 4), nsmall = 4, scientific = FALSE)
                table <- cbind(table, vectorMedian)
            }
            if (MedianQuartile == "Y") {
                vectorLQ <- format(round(vectorLQ, 4), nsmall = 4, scientific = FALSE)
                table <- cbind(table, vectorLQ)
            }
            if (MedianQuartile == "Y") {
                vectorUQ <- format(round(vectorUQ, 4), nsmall = 4, scientific = FALSE)
                table <- cbind(table, vectorUQ)
            }
            if (CoeffVariation == "Y") {
                vectorCoeffVariation <- format(round(vectorCoeffVariation, 1), nsmall = 1, scientific = FALSE)
                table <- cbind(table, vectorCoeffVariation)
            }
            if (confidenceLimits == "Y") {
                vectorLCI <- format(round(vectorLCI, 4), nsmall = 4, scientific = FALSE)
                table <- cbind(table, vectorLCI)
            }
            if (confidenceLimits == "Y") {
                vectorUCI <- format(round(vectorUCI, 4), nsmall = 4, scientific = FALSE)
                table <- cbind(table, vectorUCI)
            }

            #creating final output table
            #Generating blank line

            if (dim(table)[2] == 2) {
                header <- c(" ", " ")
            }
            if (dim(table)[2] == 3) {
                header <- c(" ", " ", " ")
            }
            if (dim(table)[2] == 4) {
                header <- c(" ", " ", " ", " ")
            }
            if (dim(table)[2] == 5) {
                header <- c(" ", " ", " ", " ", " ")
            }
            if (dim(table)[2] == 6) {
                header <- c(" ", " ", " ", " ", " ", " ")
            }
            if (dim(table)[2] == 7) {
                header <- c(" ", " ", " ", " ", " ", " ", " ")
            }
            if (dim(table)[2] == 8) {
                header <- c(" ", " ", " ", " ", " ", " ", " ", " ")
            }
            if (dim(table)[2] == 9) {
                header <- c(" ", " ", " ", " ", " ", " ", " ", " ", " ")
            }
            if (dim(table)[2] == 10) {
                header <- c(" ", " ", " ", " ", " ", " ", " ", " ", " ", " ")
            }
            if (dim(table)[2] == 11) {
                header <- c(" ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ")
            }
            if (dim(table)[2] == 12) {
                header <- c(" ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ")
            }
            if (dim(table)[2] == 13) {
                header <- c(" ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ")
            }
            if (dim(table)[2] == 14) {
                header <- c(" ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ")
            }
            if (dim(table)[2] == 15) {
                header <- c(" ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ")
            }
            if (dim(table)[2] == 16) {
                header <- c(" ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ")
            }

            table2 <- rbind(header, table)

            #Generating column names
            temp6 <- c(" ")
            if (mean == "Y") {
                hed1 <- c("Mean")
                temp6 <- cbind(temp6, hed1)
            }
            if (N == "Y") {
                hed2 <- c("N")
                temp6 <- cbind(temp6, hed2)
            }
            if (StDev == "Y") {
                #STB May 2012 changing header
                hed3 <- c("Std dev")
                temp6 <- cbind(temp6, hed3)
            }
            if (Variances == "Y") {
                hed4 <- c("Variance")
                temp6 <- cbind(temp6, hed4)
            }
            if (StErr == "Y") {
                #STB May 2012 changing header
                hed5 <- c("Std error")
                temp6 <- cbind(temp6, hed5)
            }
            if (MinMax == "Y") {
                hed6 <- c("Min")
                temp6 <- cbind(temp6, hed6)
            }
            if (MinMax == "Y") {
                hed7 <- c("Max")
                temp6 <- cbind(temp6, hed7)
            }
            if (MedianQuartile == "Y") {
                hed8 <- c("Median")
                temp6 <- cbind(temp6, hed8)
            }
            if (MedianQuartile == "Y") {
                hed9 <- c("Lower quartile")
                temp6 <- cbind(temp6, hed9)
            }
            if (MedianQuartile == "Y") {
                hed10 <- c("Upper quartile")
                temp6 <- cbind(temp6, hed10)
            }
            if (CoeffVariation == "Y") {
                hed11 <- c("%CV")
                temp6 <- cbind(temp6, hed11)
            }
            if (confidenceLimits == "Y") {
                CIlow <- paste("Lower ", 100 * CIval, sep = "")
                CIlow <- paste(CIlow, "% CI", sep = "")
                hed12 <- c(CIlow)
                temp6 <- cbind(temp6, hed12)
            }
            if (confidenceLimits == "Y") {
                CIhigh <- paste("Upper ", 100 * CIval, sep = "")
                CIhigh <- paste(CIhigh, "% CI", sep = "")
                hed13 <- c(CIhigh)
                temp6 <- cbind(temp6, hed13)
            }

            colnames(table2) <- temp6
            rownms <- c("Categorisation Factor levels")
            for (i in 1:length) {
                rownms[i + 1] <- levels(as.factor(statdata$catfact))[i]
            }
            row.names(table2) <- rownms

            add <- paste(c("Summary statistics for "), csResponses, sep = "")
            add <- paste(add, " categorised by ", sep = "")


            if (firstCat != "NULL" && secondCat == "NULL" && thirdCat == "NULL" && fourthCat == "NULL") {
                add <- paste(add, firstCat, sep = "")
            } else
                if (firstCat == "NULL" && secondCat != "NULL" && thirdCat == "NULL" && fourthCat == "NULL") {
                    add <- paste(add, secondCat, sep = "")
                } else
                    if (firstCat == "NULL" && secondCat == "NULL" && thirdCat != "NULL" && fourthCat == "NULL") {
                        add <- paste(add, thirdCat, sep = "")
                    } else
                        if (firstCat == "NULL" && secondCat == "NULL" && thirdCat == "NULL" && fourthCat != "NULL") {
                            add <- paste(add, fourthCat, sep = "")
                        }

            if (firstCat != "NULL" && secondCat != "NULL" && thirdCat == "NULL" && fourthCat == "NULL") {
                add <- paste(add, firstCat, sep = "")
                add <- paste(add, " and ", sep = "")
                add <- paste(add, secondCat, sep = "")
            }

            if (firstCat != "NULL" && secondCat == "NULL" && thirdCat != "NULL" && fourthCat == "NULL") {
                add <- paste(add, firstCat, sep = "")
                add <- paste(add, " and ", sep = "")
                add <- paste(add, thirdCat, sep = "")
            }

            if (firstCat != "NULL" && secondCat == "NULL" && thirdCat == "NULL" && fourthCat != "NULL") {
                add <- paste(add, firstCat, sep = "")
                add <- paste(add, " and ", sep = "")
                add <- paste(add, fourthCat, sep = "")
            }

            if (firstCat == "NULL" && secondCat != "NULL" && thirdCat != "NULL" && fourthCat == "NULL") {
                add <- paste(add, secondCat, sep = "")
                add <- paste(add, " and ", sep = "")
                add <- paste(add, thirdCat, sep = "")
            }

            if (firstCat == "NULL" && secondCat != "NULL" && thirdCat == "NULL" && fourthCat != "NULL") {
                add <- paste(add, secondCat, sep = "")
                add <- paste(add, " and ", sep = "")
                add <- paste(add, fourthCat, sep = "")
            }

            if (firstCat == "NULL" && secondCat == "NULL" && thirdCat != "NULL" && fourthCat != "NULL") {
                add <- paste(add, thirdCat, sep = "")
                add <- paste(add, " and ", sep = "")
                add <- paste(add, fourthCat, sep = "")
            }

            if (firstCat == "NULL" && secondCat != "NULL" && thirdCat != "NULL" && fourthCat != "NULL") {
                add <- paste(add, secondCat, sep = "")
                add <- paste(add, ", ", sep = "")
                add <- paste(add, thirdCat, sep = "")
                add <- paste(add, " and ", sep = "")
                add <- paste(add, fourthCat, sep = "")
            }

            if (firstCat != "NULL" && secondCat != "NULL" && thirdCat != "NULL" && fourthCat == "NULL") {
                add <- paste(add, firstCat, sep = "")
                add <- paste(add, ", ", sep = "")
                add <- paste(add, secondCat, sep = "")
                add <- paste(add, " and ", sep = "")
                add <- paste(add, thirdCat, sep = "")
            }

            if (firstCat != "NULL" && secondCat != "NULL" && thirdCat == "NULL" && fourthCat != "NULL") {
                add <- paste(add, firstCat, sep = "")
                add <- paste(add, ", ", sep = "")
                add <- paste(add, secondCat, sep = "")
                add <- paste(add, " and ", sep = "")
                add <- paste(add, fourthCat, sep = "")
            }

            if (firstCat != "NULL" && secondCat == "NULL" && thirdCat != "NULL" && fourthCat != "NULL") {
                add <- paste(add, firstCat, sep = "")
                add <- paste(add, ", ", sep = "")
                add <- paste(add, thirdCat, sep = "")
                add <- paste(add, " and ", sep = "")
                add <- paste(add, fourthCat, sep = "")
            }

            if (firstCat != "NULL" && secondCat != "NULL" && thirdCat != "NULL" && fourthCat != "NULL") {
                add <- paste(add, firstCat, sep = "")
                add <- paste(add, ", ", sep = "")
                add <- paste(add, secondCat, sep = "")
                add <- paste(add, ", ", sep = "")
                add <- paste(add, thirdCat, sep = "")
                add <- paste(add, " and ", sep = "")
                add <- paste(add, fourthCat, sep = "")
            }
            HTMLbr()
            HTML.title(add, HR = 0, align = "left")
            HTML(table2,, align = "left", classfirstline = "second")
            HTML.title("</bf> ", HR = 2, align = "left")
        }


        #Analysis without any categorisation factor
        if ((firstCat == "NULL" && secondCat == "NULL" && thirdCat == "NULL" && fourthCat == "NULL") || ByCategoriesAndOverall == "Y") {
            #Analysis if categorisation factors selected

            #Setting up parameters and vectors
            length <- 1
            tablenames <- c(" ")
            table <- c(1:length)
            for (i in 1:length) {
                table[i] = " "
            }
            vectormean <- c(1:length)
            vectorN <- c(1:length)
            vectorStDev <- c(1:length)
            vectorVariances <- c(1:length)
            vectorStErr <- c(1:length)
            vectorMin <- c(1:length)
            vectorMax <- c(1:length)
            vectorMedian <- c(1:length)
            vectorLQ <- c(1:length)
            vectorUQ <- c(1:length)
            vectorCoeffVariation <- c(1:length)
            vectorUCI <- c(1:length)
            vectorLCI <- c(1:length)
            #	vectorLPercentBoundaries <-c(1:length)
            #	vectorUPercentBoundaries <-c(1:length)

            for (i in 1:length) {
                sub <- statdata
                sub2 <- data.frame(sub)

                if (mean == "Y") {
                    vectormean[i] = mean(eval(parse(text = paste("sub2$", csResponses))), na.rm = TRUE)
                }
                if (N == "Y") {
                    tempy <- na.omit(eval(parse(text = paste("sub2$", csResponses))))
                    vectorN[i] = length(tempy)
                }
                if (StDev == "Y") {
                    vectorStDev[i] = sd(eval(parse(text = paste("sub2$", csResponses))), na.rm = TRUE)
                }
                if (Variances == "Y") {
                    vectorVariances[i] = var(eval(parse(text = paste("sub2$", csResponses))), na.rm = TRUE)
                }
                if (StErr == "Y") {
                    tempy <- na.omit(eval(parse(text = paste("sub2$", csResponses))))
                    vectorStErr[i] = sd(eval(parse(text = paste("sub2$", csResponses))), na.rm = TRUE) / (length(tempy)) ** (0.5)
                }
                if (MinMax == "Y") {
                    vectorMin[i] = suppressWarnings(min(eval(parse(text = paste("sub2$", csResponses))), na.rm = TRUE))
                }
                if (MinMax == "Y") {
                    vectorMax[i] = suppressWarnings(max(eval(parse(text = paste("sub2$", csResponses))), na.rm = TRUE))
                }
                if (MedianQuartile == "Y") {
                    vectorMedian[i] = median(eval(parse(text = paste("sub2$", csResponses))), na.rm = TRUE)
                }
                if (MedianQuartile == "Y") {
                    vectorLQ[i] = quantile(eval(parse(text = paste("sub2$", csResponses))), 0.25, type = 2, na.rm = TRUE)
                    #				vectorLQ[i]=fivenum(eval(parse(text = paste("sub2$", csResponses))), na.rm = TRUE)[2]
                }
                if (MedianQuartile == "Y") {
                    vectorUQ[i] = quantile(eval(parse(text = paste("sub2$", csResponses))), 0.75, type = 2, na.rm = TRUE)
                    #				vectorUQ[i]=fivenum(eval(parse(text = paste("sub2$", csResponses))), na.rm = TRUE)[4]
                }
                if (CoeffVariation == "Y") {
                    vectorCoeffVariation[i] = 100 * sd(eval(parse(text = paste("sub2$", csResponses))), na.rm = TRUE) / mean(eval(parse(text = paste("sub2$", csResponses))), na.rm = TRUE)
                }
                if (confidenceLimits == "Y") {
                    tempy <- na.omit(eval(parse(text = paste("sub2$", csResponses))))
                    vectorLCI[i] = mean(eval(parse(text = paste("sub2$", csResponses))), na.rm = TRUE) - qt(1 - (1 - CIval) / 2, (length(tempy) - 1)) * sd(eval(parse(text = paste("sub2$", csResponses))), na.rm = TRUE) / (length(tempy)) ** (0.5)
                }
                if (confidenceLimits == "Y") {
                    tempy <- na.omit(eval(parse(text = paste("sub2$", csResponses))))
                    vectorUCI[i] = mean(eval(parse(text = paste("sub2$", csResponses))), na.rm = TRUE) + qt(1 - (1 - CIval) / 2, (length(tempy) - 1)) * sd(eval(parse(text = paste("sub2$", csResponses))), na.rm = TRUE) / (length(tempy)) ** (0.5)
                }
            }

            #Generating final table dataset
            if (mean == "Y") {
                vectormean <- format(round(vectormean, 4), nsmall = 4, scientific = FALSE)
                table <- cbind(table, vectormean)
            }
            if (N == "Y") {
                #vectorN<-format(round(vectormean, 4), nsmall=4, scientific=FALSE)
                table <- cbind(table, vectorN)
            }
            if (StDev == "Y") {
                vectorStDev <- format(round(vectorStDev, 4), nsmall = 4, scientific = FALSE)
                table <- cbind(table, vectorStDev)
            }
            if (Variances == "Y") {
                vectorVariances <- format(round(vectorVariances, 4), nsmall = 4, scientific = FALSE)
                table <- cbind(table, vectorVariances)
            }
            if (StErr == "Y") {
                vectorStErr <- format(round(vectorStErr, 4), nsmall = 4, scientific = FALSE)
                table <- cbind(table, vectorStErr)
            }
            if (MinMax == "Y") {
                vectorMin <- format(round(vectorMin, 4), nsmall = 4, scientific = FALSE)
                table <- cbind(table, vectorMin)
            }
            if (MinMax == "Y") {
                vectorMax <- format(round(vectorMax, 4), nsmall = 4, scientific = FALSE)
                table <- cbind(table, vectorMax)
            }
            if (MedianQuartile == "Y") {
                vectorMedian <- format(round(vectorMedian, 4), nsmall = 4, scientific = FALSE)
                table <- cbind(table, vectorMedian)
            }
            if (MedianQuartile == "Y") {
                vectorLQ <- format(round(vectorLQ, 4), nsmall = 4, scientific = FALSE)
                table <- cbind(table, vectorLQ)
            }
            if (MedianQuartile == "Y") {
                vectorUQ <- format(round(vectorUQ, 4), nsmall = 4, scientific = FALSE)
                table <- cbind(table, vectorUQ)
            }
            if (CoeffVariation == "Y") {
                vectorCoeffVariation <- format(round(vectorCoeffVariation, 1), nsmall = 1, scientific = FALSE)
                table <- cbind(table, vectorCoeffVariation)
            }
            if (confidenceLimits == "Y") {
                vectorLCI <- format(round(vectorLCI, 4), nsmall = 4, scientific = FALSE)
                table <- cbind(table, vectorLCI)
            }
            if (confidenceLimits == "Y") {
                vectorUCI <- format(round(vectorUCI, 4), nsmall = 4, scientific = FALSE)
                table <- cbind(table, vectorUCI)
            }

            #creating final output table

            #Generating blank line

            if (dim(table)[2] == 2) {
                header <- c(" ", " ")
            }
            if (dim(table)[2] == 3) {
                header <- c(" ", " ", " ")
            }
            if (dim(table)[2] == 4) {
                header <- c(" ", " ", " ", " ")
            }
            if (dim(table)[2] == 5) {
                header <- c(" ", " ", " ", " ", " ")
            }
            if (dim(table)[2] == 6) {
                header <- c(" ", " ", " ", " ", " ", " ")
            }
            if (dim(table)[2] == 7) {
                header <- c(" ", " ", " ", " ", " ", " ", " ")
            }
            if (dim(table)[2] == 8) {
                header <- c(" ", " ", " ", " ", " ", " ", " ", " ")
            }
            if (dim(table)[2] == 9) {
                header <- c(" ", " ", " ", " ", " ", " ", " ", " ", " ")
            }
            if (dim(table)[2] == 10) {
                header <- c(" ", " ", " ", " ", " ", " ", " ", " ", " ", " ")
            }
            if (dim(table)[2] == 11) {
                header <- c(" ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ")
            }
            if (dim(table)[2] == 12) {
                header <- c(" ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ")
            }
            if (dim(table)[2] == 13) {
                header <- c(" ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ")
            }
            if (dim(table)[2] == 14) {
                header <- c(" ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ")
            }
            if (dim(table)[2] == 15) {
                header <- c(" ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ")
            }
            if (dim(table)[2] == 16) {
                header <- c(" ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ")
            }

            table2 <- rbind(header, table)

            #Generating column names
            temp6 <- c(" ")

            if (mean == "Y") {
                hed1 <- c("Mean")
                temp6 <- cbind(temp6, hed1)
            }
            if (N == "Y") {
                hed2 <- c("N")
                temp6 <- cbind(temp6, hed2)
            }
            if (StDev == "Y") {
                #STB May 2012 changing header
                hed3 <- c("Std dev")
                temp6 <- cbind(temp6, hed3)
            }
            if (Variances == "Y") {
                hed4 <- c("Variance")
                temp6 <- cbind(temp6, hed4)
            }
            if (StErr == "Y") {
                #STB May 2012 changing header
                hed5 <- c("Std error")
                temp6 <- cbind(temp6, hed5)
            }
            if (MinMax == "Y") {
                hed6 <- c("Min")
                temp6 <- cbind(temp6, hed6)
            }
            if (MinMax == "Y") {
                hed7 <- c("Max")
                temp6 <- cbind(temp6, hed7)
            }
            if (MedianQuartile == "Y") {
                hed8 <- c("Median")
                temp6 <- cbind(temp6, hed8)
            }
            if (MedianQuartile == "Y") {
                hed9 <- c("Lower quartile")
                temp6 <- cbind(temp6, hed9)
            }
            if (MedianQuartile == "Y") {
                hed10 <- c("Upper quartile")
                temp6 <- cbind(temp6, hed10)
            }
            if (CoeffVariation == "Y") {
                hed11 <- c("%CV")
                temp6 <- cbind(temp6, hed11)
            }
            if (confidenceLimits == "Y") {
                CIlow <- paste("Lower ", 100 * CIval, sep = "")
                CIlow <- paste(CIlow, "% CI", sep = "")
                hed12 <- c(CIlow)
                temp6 <- cbind(temp6, hed12)
            }
            if (confidenceLimits == "Y") {
                CIhigh <- paste("Upper ", 100 * CIval, sep = "")
                CIhigh <- paste(CIhigh, "% CI", sep = "")
                hed13 <- c(CIhigh)
                temp6 <- cbind(temp6, hed13)
            }

            colnames(table2) <- temp6

            rownms <- c(" ")
            for (i in 1:length) {
                rownms[i + 1] <- csResponses
            }

            row.names(table2) <- rownms


            if ((firstCat == "NULL" && secondCat == "NULL" && thirdCat == "NULL" && fourthCat == "NULL")) {
                HTMLbr()
                add <- paste(c("Summary statistics for "), csResponses, sep = "")
                HTML.title(add, HR = 0, align = "left")
                HTML(table2,, align = "left", classfirstline = "second")
                HTML.title("</bf> ", HR = 2, align = "left")

            } else
                if ((firstCat != "NULL" || secondCat != "NULL" || thirdCat != "NULL" || fourthCat != "NULL") && ByCategoriesAndOverall == "Y") {
                    HTMLbr()
                    add <- paste(c("Overall summary statistics, ignoring the categorisation factor(s), for "), csResponses, sep = "")
                    HTML.title(add, HR = 0, align = "left")
                    HTML(table2,, align = "left", classfirstline = "second")
                    HTML.title("</bf> ", HR = 2, align = "left")
                }
        }
    }

    #HTML.title("<bf> ", HR=2, align="left")

    if (transformation != "None") {
        HTMLbr()
        HTML.title("<bf>Transformation", HR = 2, align = "left")

        add2 <- paste(c("Responses "), transformation, sep = "")
        add2 <- paste(add2, " transformed prior to analysis.", sep = "")
        HTML.title(add2, HR = 0, align = "left")
    }

    #end of do loop x
    #STB 06 September 2010
}


#Normal probability plot

if (NormalProbabilityPlot != "N") {
    HTMLbr()
    HTML.title("<bf>Normal probability plot", HR = 2, align = "left")

    #====================================================================================================================================================
    #Graphical plot options
    YAxisTitle <- "Sample Quantiles"
    XAxisTitle <- "Theoretical Quantiles"
    w_Gr_jit <- 0
    h_Gr_jit <- 0
    infiniteslope <- "N"
    LinearFit <- "Y"
    Line_size <- 0.5
    Gr_alpha <- 1
    Line_type <- Line_type_dashed
    ScatterPlot <- "Y"
    #====================================================================================================================================================

    normPlot <- sub(".html", "normplot.jpg", htmlFile)
    jpeg(normPlot, width = jpegwidth, height = jpegheight, quality = 100)

    #STB July2013
    plotFilepdf4 <- sub(".html", "normplot.pdf", htmlFile)
    dev.control("enable")


    #first plot
    csResponses <- resplist[1]
    csResponsesqq <- resplistqq[1]
    adda <- paste(c("Normal probability plot for "), csResponsesqq, " \n", sep = "")
    MainTitle2 <- adda
    te <- qqnorm(eval(parse(text = paste("statdata$", csResponses))))
    graphdata <- data.frame(te$x, te$y)
    graphdata$xvarrr_IVS <- graphdata$te.x
    graphdata$yvarrr_IVS <- graphdata$te.y


    #GGPLOT2 code
    NONCAT_SCAT("QQPLOT")
    #====================================================================================================================================================
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
}



#second plot
if (NormalProbabilityPlot != "N" && resplength > 1) {
    normPlot2 <- sub(".html", "normplot2.jpg", htmlFile)
    jpeg(normPlot2, width = jpegwidth, height = jpegheight, quality = 100)

    #STB July2013
    plotFilepdf2 <- sub(".html", "normplot2.pdf", htmlFile)
    dev.control("enable")

    csResponses <- resplist[2]
    csResponsesqq <- resplistqq[2]
    adda <- paste(c("Normal probability plot for "), csResponsesqq, " \n", sep = "")
    MainTitle2 <- adda
    te <- qqnorm(eval(parse(text = paste("statdata$", csResponses))))
    graphdata <- data.frame(te$x, te$y)
    graphdata$xvarrr_IVS <- graphdata$te.x
    graphdata$yvarrr_IVS <- graphdata$te.y

    #GGPLOT2 code
    NONCAT_SCAT("QQPLOT")
    #====================================================================================================================================================
    #OUtput code
    void <- HTMLInsertGraph(GraphFileName = sub("[A-Z0-9a-z,:,\\\\]*App_Data[\\\\]", "", normPlot2), Align = "left")

    #STB July2013
    if (pdfout == "Y") {
        pdf(file = sub("[A-Z0-9a-z,:,\\\\]*App_Data[\\\\]", "", plotFilepdf2), height = pdfheight, width = pdfwidth)
        dev.set(2)
        dev.copy(which = 3)
        dev.off(2)
        dev.off(3)
        pdfFile_2 <- sub("[A-Z0-9a-z,:,\\\\]*App_Data[\\\\]", "", plotFilepdf2)
        linkToPdf2 <- paste("<a href=\"", pdfFile_2, "\">Click here to view the PDF of the normal probability plot</a>", sep = "")
        HTML(linkToPdf2)
    }
}


#third plot
if (NormalProbabilityPlot != "N" && resplength > 2) {
    normPlot3 <- sub(".html", "normplot3.jpg", htmlFile)
    jpeg(normPlot3, width = jpegwidth, height = jpegheight, quality = 100)

    #STB July2013
    plotFilepdf3 <- sub(".html", "normplot3.pdf", htmlFile)
    dev.control("enable")

    csResponses <- resplist[3]
    csResponsesqq <- resplistqq[3]
    adda <- paste(c("Normal probability plot for "), csResponsesqq, " \n", sep = "")
    MainTitle2 <- adda
    te <- qqnorm(eval(parse(text = paste("statdata$", csResponses))))
    graphdata <- data.frame(te$x, te$y)
    graphdata$xvarrr_IVS <- graphdata$te.x
    graphdata$yvarrr_IVS <- graphdata$te.y

    #GGPLOT2 code
    NONCAT_SCAT("QQPLOT")
    #====================================================================================================================================================
    void <- HTMLInsertGraph(GraphFileName = sub("[A-Z0-9a-z,:,\\\\]*App_Data[\\\\]", "", normPlot3), Align = "left")

    #STB July2013
    if (pdfout == "Y") {
        pdf(file = sub("[A-Z0-9a-z,:,\\\\]*App_Data[\\\\]", "", plotFilepdf3), height = pdfheight, width = pdfwidth)
        dev.set(2)
        dev.copy(which = 3)
        dev.off(2)
        dev.off(3)
        pdfFile_3 <- sub("[A-Z0-9a-z,:,\\\\]*App_Data[\\\\]", "", plotFilepdf3)
        linkToPdf3 <- paste("<a href=\"", pdfFile_3, "\">Click here to view the PDF of the normal probability plot</a>", sep = "")
        HTML(linkToPdf3)
    }
}

#fourth plot
if (NormalProbabilityPlot != "N" && resplength > 3) {
    normPlot4 <- sub(".html", "normplot4.jpg", htmlFile)
    jpeg(normPlot4, width = jpegwidth, height = jpegheight, quality = 100)

    #STB July2013
    plotFilepdf4 <- sub(".html", "normplot4.pdf", htmlFile)
    dev.control("enable")

    csResponses <- resplist[4]
    csResponsesqq <- resplistqq[4]
    adda <- paste(c("Normal probability plot for "), csResponsesqq, " \n", sep = "")
    MainTitle2 <- adda
    te <- qqnorm(eval(parse(text = paste("statdata$", csResponses))))
    graphdata <- data.frame(te$x, te$y)
    graphdata$xvarrr_IVS <- graphdata$te.x
    graphdata$yvarrr_IVS <- graphdata$te.y

    #GGPLOT2 code
    NONCAT_SCAT("QQPLOT")
    #====================================================================================================================================================
    #OUtput code
    void <- HTMLInsertGraph(GraphFileName = sub("[A-Z0-9a-z,:,\\\\]*App_Data[\\\\]", "", normPlot4), Align = "left")

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
}

#fifth plot
if (NormalProbabilityPlot != "N" && resplength > 4) {
    normPlot5 <- sub(".html", "normplot5.jpg", htmlFile)
    jpeg(normPlot5, width = jpegwidth, height = jpegheight, quality = 100)

    #STB July2013
    plotFilepdf5 <- sub(".html", "normplot5.pdf", htmlFile)
    dev.control("enable")

    csResponses <- resplist[5]
    csResponsesqq <- resplistqq[5]
    adda <- paste(c("Normal probability plot for "), csResponsesqq, " \n", sep = "")
    MainTitle2 <- adda
    te <- qqnorm(eval(parse(text = paste("statdata$", csResponses))))
    graphdata <- data.frame(te$x, te$y)
    graphdata$xvarrr_IVS <- graphdata$te.x
    graphdata$yvarrr_IVS <- graphdata$te.y

    #GGPLOT2 code
    NONCAT_SCAT("QQPLOT")
    #====================================================================================================================================================
    #OUtput code
    void <- HTMLInsertGraph(GraphFileName = sub("[A-Z0-9a-z,:,\\\\]*App_Data[\\\\]", "", normPlot5), Align = "left")

    #STB July2013
    if (pdfout == "Y") {
        pdf(file = sub("[A-Z0-9a-z,:,\\\\]*App_Data[\\\\]", "", plotFilepdf5), height = pdfheight, width = pdfwidth)
        dev.set(2)
        dev.copy(which = 3)
        dev.off(2)
        dev.off(3)
        pdfFile_5 <- sub("[A-Z0-9a-z,:,\\\\]*App_Data[\\\\]", "", plotFilepdf5)
        linkToPdf5 <- paste("<a href=\"", pdfFile_5, "\">Click here to view the PDF of the normal probability plot</a>", sep = "")
        HTML(linkToPdf5)
    }
}

#sixth plot
if (NormalProbabilityPlot != "N" && resplength > 5) {
    normPlot6 <- sub(".html", "normplot6.jpg", htmlFile)
    jpeg(normPlot6, width = jpegwidth, height = jpegheight, quality = 100)

    #STB July2013
    plotFilepdf6 <- sub(".html", "normplot6.pdf", htmlFile)
    dev.control("enable")

    csResponses <- resplist[6]
    csResponsesqq <- resplistqq[6]
    adda <- paste(c("Normal probability plot for "), csResponsesqq, " \n", sep = "")
    MainTitle2 <- adda
    te <- qqnorm(eval(parse(text = paste("statdata$", csResponses))))
    graphdata <- data.frame(te$x, te$y)
    graphdata$xvarrr_IVS <- graphdata$te.x
    graphdata$yvarrr_IVS <- graphdata$te.y

    #GGPLOT2 code
    NONCAT_SCAT("QQPLOT")
    #====================================================================================================================================================
    #OUtput code
    void <- HTMLInsertGraph(GraphFileName = sub("[A-Z0-9a-z,:,\\\\]*App_Data[\\\\]", "", normPlot6), Align = "left")

    #STB July2013
    if (pdfout == "Y") {
        pdf(file = sub("[A-Z0-9a-z,:,\\\\]*App_Data[\\\\]", "", plotFilepdf6), height = pdfheight, width = pdfwidth)
        dev.set(2)
        dev.copy(which = 3)
        dev.off(2)
        dev.off(3)
        pdfFile_6 <- sub("[A-Z0-9a-z,:,\\\\]*App_Data[\\\\]", "", plotFilepdf6)
        linkToPdf6 <- paste("<a href=\"", pdfFile_6, "\">Click here to view the PDF of the normal probability plot</a>", sep = "")
        HTML(linkToPdf6)
    }
}

#seventh plot
if (NormalProbabilityPlot != "N" && resplength > 6) {
    normPlot7 <- sub(".html", "normplot7.jpg", htmlFile)
    jpeg(normPlot7, width = jpegwidth, height = jpegheight, quality = 100)

    #STB July2013
    plotFilepdf7 <- sub(".html", "normplot7.pdf", htmlFile)
    dev.control("enable")

    csResponses <- resplist[7]
    csResponsesqq <- resplistqq[7]
    adda <- paste(c("Normal probability plot for "), csResponsesqq, " \n", sep = "")
    MainTitle2 <- adda
    te <- qqnorm(eval(parse(text = paste("statdata$", csResponses))))
    graphdata <- data.frame(te$x, te$y)
    graphdata$xvarrr_IVS <- graphdata$te.x
    graphdata$yvarrr_IVS <- graphdata$te.y

    #GGPLOT2 code
    NONCAT_SCAT("QQPLOT")
    #====================================================================================================================================================
    #OUtput code
    void <- HTMLInsertGraph(GraphFileName = sub("[A-Z0-9a-z,:,\\\\]*App_Data[\\\\]", "", normPlot7), Align = "left")

    #STB July2013
    if (pdfout == "Y") {
        pdf(file = sub("[A-Z0-9a-z,:,\\\\]*App_Data[\\\\]", "", plotFilepdf7), height = pdfheight, width = pdfwidth)
        dev.set(2)
        dev.copy(which = 3)
        dev.off(2)
        dev.off(3)
        pdfFile_7 <- sub("[A-Z0-9a-z,:,\\\\]*App_Data[\\\\]", "", plotFilepdf7)
        linkToPdf7 <- paste("<a href=\"", pdfFile_7, "\">Click here to view the PDF of the normal probability plot</a>", sep = "")
        HTML(linkToPdf7)
    }
}

#eigth plot
if (NormalProbabilityPlot != "N" && resplength > 7) {
    normPlot8 <- sub(".html", "normplot8.jpg", htmlFile)
    jpeg(normPlot8, width = jpegwidth, height = jpegheight, quality = 100)

    #STB July2013
    plotFilepdf8 <- sub(".html", "normplot8.pdf", htmlFile)
    dev.control("enable")

    csResponses <- resplist[8]
    csResponsesqq <- resplistqq[8]
    adda <- paste(c("Normal probability plot for "), csResponsesqq, " \n", sep = "")
    MainTitle2 <- adda
    te <- qqnorm(eval(parse(text = paste("statdata$", csResponses))))
    graphdata <- data.frame(te$x, te$y)
    graphdata$xvarrr_IVS <- graphdata$te.x
    graphdata$yvarrr_IVS <- graphdata$te.y

    #GGPLOT2 code
    NONCAT_SCAT("QQPLOT")
    #====================================================================================================================================================
    #OUtput code
    void <- HTMLInsertGraph(GraphFileName = sub("[A-Z0-9a-z,:,\\\\]*App_Data[\\\\]", "", normPlot8), Align = "left")

    #STB July2013
    if (pdfout == "Y") {
        pdf(file = sub("[A-Z0-9a-z,:,\\\\]*App_Data[\\\\]", "", plotFilepdf8), height = pdfheight, width = pdfwidth)
        dev.set(2)
        dev.copy(which = 3)
        dev.off(2)
        dev.off(3)
        pdfFile_8 <- sub("[A-Z0-9a-z,:,\\\\]*App_Data[\\\\]", "", plotFilepdf8)
        linkToPdf8 <- paste("<a href=\"", pdfFile_8, "\">Click here to view the PDF of the normal probability plot</a>", sep = "")
        HTML(linkToPdf8)
    }
}

#ninth plot
if (NormalProbabilityPlot != "N" && resplength > 8) {
    normPlot9 <- sub(".html", "normplot9.jpg", htmlFile)
    jpeg(normPlot9, width = jpegwidth, height = jpegheight, quality = 100)

    #STB July2013
    plotFilepdf9 <- sub(".html", "normplot9.pdf", htmlFile)
    dev.control("enable")

    csResponses <- resplist[9]
    csResponsesqq <- resplistqq[9]
    adda <- paste(c("Normal probability plot for "), csResponsesqq, " \n", sep = "")
    MainTitle2 <- adda
    te <- qqnorm(eval(parse(text = paste("statdata$", csResponses))))
    graphdata <- data.frame(te$x, te$y)
    graphdata$xvarrr_IVS <- graphdata$te.x
    graphdata$yvarrr_IVS <- graphdata$te.y

    #GGPLOT2 code
    NONCAT_SCAT("QQPLOT")
    #====================================================================================================================================================
    #OUtput code
    void <- HTMLInsertGraph(GraphFileName = sub("[A-Z0-9a-z,:,\\\\]*App_Data[\\\\]", "", normPlot9), Align = "left")

    #STB July2013
    if (pdfout == "Y") {
        pdf(file = sub("[A-Z0-9a-z,:,\\\\]*App_Data[\\\\]", "", plotFilepdf9), height = pdfheight, width = pdfwidth)
        dev.set(2)
        dev.copy(which = 3)
        dev.off(2)
        dev.off(3)
        pdfFile_9 <- sub("[A-Z0-9a-z,:,\\\\]*App_Data[\\\\]", "", plotFilepdf9)
        linkToPdf9 <- paste("<a href=\"", pdfFile_9, "\">Click here to view the PDF of the normal probability plot</a>", sep = "")
        HTML(linkToPdf9)
    }
}

#tenth plot
if (NormalProbabilityPlot != "N" && resplength > 9) {
    normPlot2a <- sub(".html", "normplot2a.jpg", htmlFile)
    jpeg(normPlot2a, width = jpegwidth, height = jpegheight, quality = 100)

    #STB July2013
    plotFilepdf2a <- sub(".html", "normplot2a.pdf", htmlFile)
    dev.control("enable")

    csResponses <- resplist[10]
    csResponsesqq <- resplistqq[10]
    adda <- paste(c("Normal probability plot for "), csResponsesqq, " \n", sep = "")
    MainTitle2 <- adda
    te <- qqnorm(eval(parse(text = paste("statdata$", csResponses))))
    graphdata <- data.frame(te$x, te$y)
    graphdata$xvarrr_IVS <- graphdata$te.x
    graphdata$yvarrr_IVS <- graphdata$te.y

    #GGPLOT2 code
    NONCAT_SCAT("QQPLOT")
    #====================================================================================================================================================
    #OUtput code
    void <- HTMLInsertGraph(GraphFileName = sub("[A-Z0-9a-z,:,\\\\]*App_Data[\\\\]", "", normPlot2a), Align = "left")

    #STB July2013
    if (pdfout == "Y") {
        pdf(file = sub("[A-Z0-9a-z,:,\\\\]*App_Data[\\\\]", "", plotFilepdf2a), height = pdfheight, width = pdfwidth)
        dev.set(2)
        dev.copy(which = 3)
        dev.off(2)
        dev.off(3)
        pdfFile_2a <- sub("[A-Z0-9a-z,:,\\\\]*App_Data[\\\\]", "", plotFilepdf2a)
        linkToPdf2a <- paste("<a href=\"", pdfFile_2a, "\">Click here to view the PDF of the normal probability plot</a>", sep = "")
        HTML(linkToPdf2a)
    }
}

#other plots
if (NormalProbabilityPlot != "N" && resplength > 10) {
    HTML.title("<bf> ", HR = 2, align = "left")
    HTML.title("<bf>Normal probability plots are only produced for the first ten selected responses.", HR = 0, align = "left")
}

if (NormalProbabilityPlot != "N" && resplength < 10) {
    HTML.title("<bf> ", HR = 2, align = "left")
    HTML.title("<bf>Tip: Check that the points lie along the dotted line. If not then the data may be non-normally distributed.", HR = 0, align = "left")

    if (firstCat == "NULL" && secondCat == "NULL" && thirdCat == "NULL" && fourthCat == "NULL") {
        add2 <- c(" ")
        HTML.title(add2, HR = 0, align = "left")
    } else if ((firstCat != "NULL" && secondCat == "NULL" && thirdCat == "NULL" && fourthCat == "NULL") || (firstCat == "NULL" && secondCat != "NULL" && thirdCat == "NULL" && fourthCat == "NULL") || (firstCat == "NULL" && secondCat == "NULL" && thirdCat != "NULL" && fourthCat == "NULL") || (firstCat == "NULL" && secondCat == "NULL" && thirdCat == "NULL" && fourthCat != "NULL")) {
        HTML.title("<bf> ", HR = 2, align = "left")
        add3 <- c("Warning: This Normal probability plot does not take into account the categorisation factor. If you wish to assess normality after taking the categorisation factor into account, please use the plot in the Single Measures Parametric Analysis module.")
        HTML.title(add3, HR = 0, align = "left")
    } else {
        add4 <- c("Warning: This Normal probability plot does not take into account the categorisation factors. If you wish to assess normality after taking the categorisation factors into account, please use the plot in the Single Measures Parametric Analysis module.")
        HTML.title("<bf> ", HR = 2, align = "left")
        HTML.title(add4, HR = 0, align = "left")
    }
}


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
HTML.title(Ref_list$R2HTML_ref, HR = 0, align = "left")

HTML.title("<bf> ", HR = 2, align = "left")
HTML.title(Ref_list$PROTO_ref, HR = 0, align = "left")


#----------------------------------------------------------------------------------------------------------
#Show dataset
#----------------------------------------------------------------------------------------------------------


if (showdataset == "Y") {
    HTMLbr()
    HTML.title("<bf>Analysis dataset", HR = 2, align = "left")
    statdata2 <- subset(statdata, select = -c(catfact))
    HTML(statdata2, classfirstline = "second", align = "left")
}

