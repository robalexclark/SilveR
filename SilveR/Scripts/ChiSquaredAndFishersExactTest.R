#===================================================================================================================
#R Libraries

suppressWarnings(library(R2HTML))
suppressWarnings(library(Exact))

#===================================================================================================================
# retrieve args
Args <- commandArgs(TRUE)

#Read in data
statdata <- read.csv(Args[3], header=TRUE, sep=",")

#Copy Args
response <- Args[4]
GroupingFactor <- Args[5]
ResponseCategories <- Args[6]
ChiSquaredTest <- Args[7]
FishersExactTest <-Args[8]
FishersHypothesis <- Args[9]
BarnardsExactTest <- Args[10]
#BarnardsHypothesis <- Args[11]
#ControlGroup <- Args[12]
sig <- 1 - as.numeric(Args[11])

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

#Hypotheses code
if (FishersHypothesis == "Two-sided") {
	hyp <- "two.sided"
	hyptest <- "two-sided"
} 

if (FishersHypothesis == "Less-than") {
	hyp <- "less"
	hyptest <- "one-sided (less than)"
} 

if (FishersHypothesis == "Greater-than") {
	hyp <- "greater"
	hyptest <- "one-sided (greater than)"
} 

BarnardsHypothesis <- "Two-sided"
hypB <- "two.sided"
hypBtest <- "two-sided"

#===================================================================================================================
#Titles and description
#===================================================================================================================
#Output HTML header
#STB May 2012 correcting capitals
Title <-paste(branding, " Chi-squared Test and Fisher's Exact Test", sep="")
HTML.title(Title, HR = 1, align = "left")

#Response
HTML.title("Response", HR=2, align="left")
add<-paste(c("The  "), response, " response is currently being analysed by the Chi-squared Test and Fisher's Exact Test module. The response is separated into categories, as defined by the factors ",  GroupingFactor, " and ", ResponseCategories, ".", sep="")
HTML(add, align="left")

#Warning
HTML("Note, this module should be used to analyse count data that can be expressed in the form of a contingency table. These tests assess the significance of the association (contingency) between the two treatment classifications.", align="left")

#Bate and Clark comment
HTML(refxx, align="left")	

#===================================================================================================================
#Functions
#===================================================================================================================
Barnard <- function(data, Tbx = 1000, to.plot = TRUE,  to.print = TRUE) {
    if (sum(dim(data) == c(2, 2)) != 2) {
        stop("Input matrix must be a 2x2 matrix")
    }
    if (sum(is.finite(data)) != 4 || !is.numeric(data)) {
        stop("All X values must be numeric and finite")
    }
    if (sum(data[1, ]) <= 0 || sum(data[2, ]) <= 0 || sum(data[,1]) <= 0 || sum(data[, 2]) <= 0) {
        stop("Need at least one observation in each row or column")
    }
    Cs <- c(sum(data[, 1]), sum(data[, 2]))
    N <- sum(Cs)
    I <- matrix(0, Cs[1] + 1, Cs[2] + 1)
    for (i in 1:(Cs[1] + 1)) {
        I[i, ] <- rep(i, 1, Cs[2] + 1) - 1
    }
    J <- matrix(0, Cs[1] + 1, Cs[2] + 1)
    for (j in 1:(Cs[2] + 1)) {
        J[, j] <- t(rep(j, 1, Cs[1] + 1) - 1)
    }
    TX <- (I/Cs[1] - J/Cs[2])/sqrt(((I + J)/N) * (1 - ((I + J)/N)) * sum(1/Cs))
    TX[which(is.na(TX))] = 0

    #Wald Statistic:
    TXO <- abs(TX[data[1] + 1, data[3] + 1])
    idx <- matrix(0, Cs[1] + 1, Cs[2] + 1)
    idx[which(TX >= TXO)] <- 1
    B <- Cs + 1
    npa <- seq(1e-04, 0.9999, length = Tbx)
    LP <- log(npa)
    ALP <- log(1 - npa)
    E <- list(I + J)
    for (i in 2:Tbx) {
        E[[i]] <- E[[1]]
    }
    F <- list(N - E[[1]])
    for (i in 2:Tbx) {
        F[[i]] <- F[[1]]
    }
    CF <- list(sum(lgamma(B)) - (lgamma(I + 1) + lgamma(J + 1) +  lgamma(B[1] - I) + lgamma(B[2] - J)))
    for (i in 2:Tbx) {
        CF[[i]] <- CF[[1]]
    }
    replaced1 <- list(0)
    for (i in 1:Tbx) {
        replaced1[[i]] <- matrix(LP[i], Cs[1] + 1, Cs[2] + 1)
    }
    replaced2 <- list(0)
    for (i in 1:Tbx) {
        replaced2[[i]] <- matrix(ALP[i], Cs[1] + 1, Cs[2] + 1)
    }
    S <- list(0)
    for (i in 1:Tbx) {
        S[[i]] <- exp(CF[[i]] + E[[i]] * replaced1[[i]] + F[[i]] * replaced2[[i]])
    }
    replaced3 <- list(0)
    for (i in 1:Tbx) {
        replaced3[[i]] <- idx
    }
    Snew <- { }
    for (i in 1:Tbx) {
        Snew <- c(Snew, S[[i]])
    }
    nidx <- (Cs[1] + 1) * (Cs[2] + 1)
    dummy1 <- { }
    for (i in 1:Tbx) {
        dummy1 <- c(dummy1, Snew[which(replaced3[[i]] > 0) + 
            nidx * (i - 1)])
    }
    cols <- sum(idx[idx == 1])
    dummy2 <- matrix(0, cols, Tbx)
    for (i in 1:Tbx) {
        dummy2[, i] <- dummy1[(cols * i):(cols * (i + 1) - 1) - 
            (cols - 1)]
    }
    P <- { }
    for (i in 1:Tbx) {
        P <- c(P, sum(dummy2[, i]))
    }

    #1-tailed p-value:
    PV1 <- max(P)

    #2-tailed p-value:
    PV2 <- min(2 * PV1, 1)

    #Nuisance parameter:
    np <- npa[P == PV1]
    if (to.print) {
        cat("\n", noquote(paste(paste(paste("2x2 matrix Barnard's exact test:",   Tbx, seq = ""), paste(paste(Cs[1] + 1, "x", sep = ""), Cs[2] + 1, sep = ""), sep = ""), "tables were evaluated")))
        cat("\n", noquote("-----------------------------------------------------------"))
        cat("\n", noquote(c("Wald statistic = ", format(TXO, digits = 5, scientific = FALSE))))
        cat("\n", noquote(c("Nuisance parameter = ", format(np,  digits = 5, scientific = FALSE))))
        cat("\n", noquote(c("p-values: ", "1-tailed = ", format(PV1, digits = 5, scientific = FALSE), "2-tailed = ", format(PV2, digits = 5, scientific = FALSE))))
        cat("\n", noquote("-----------------------------------------------------------"), "\n", "\n")
    }
    if (to.plot) {
        plot(npa, P, type = "l", main = "Barnard's exact P-value", xlab = "Nuisance parameter", ylab = "P-value")
        points(np, PV1, col = 2)
    }
output<-list(PV1=PV1, PV2=PV2)
return(output)
}

#===================================================================================================================
#Performing the analyses
#===================================================================================================================
#Creating the dataset
statdata$zzzTreatmentAzzz<-as.factor(eval(parse(text = paste("statdata$", GroupingFactor))))
statdata$zzzTreatmentBzzz<-as.factor(eval(parse(text = paste("statdata$", ResponseCategories))))
statdata$zzzCountzzz<-eval(parse(text = paste("statdata$", response)))

lenA<-length(unique(levels(statdata$zzzTreatmentAzzz)))
lenB<-length(unique(levels(statdata$zzzTreatmentBzzz)))
fish<-matrix(nrow=lenA,ncol=lenB)

for (i in 1:lenA) {
	for (j in 1:lenB) {
		sub1<-subset(statdata, statdata$zzzTreatmentAzzz == unique(levels(as.factor(statdata$zzzTreatmentAzzz)))[i])
		sub2<-subset(sub1, sub1$zzzTreatmentBzzz == unique(levels(as.factor(sub1$zzzTreatmentBzzz)))[j])
		temp<-sum(sub2$zzzCountzzz)
		if (temp != "NA") {
			fish[i,j]= sum(sub2$zzzCountzzz)
			} else {
				fish[i,j]= 0
		}
	}
}

fishdata <- data.frame(fish)
temp <-unique(levels(statdata$zzzTreatmentAzzz))
fishdata2 <- cbind(temp, fishdata)
colnames(fishdata2)<-c( " ", unique(levels(statdata$zzzTreatmentBzzz)))
row <- dim(fishdata2)[1]
col <- dim(fishdata2)[2]-1

#===================================================================================================================
#Print of contingency table
HTML.title("Contingency table of counts", HR=2, align="left")
HTML(fishdata2, classfirstline="second", align="left", row.names = "FALSE")
HTML("The values in this table are the sum of the individual entries in the imported dataset.", align="left")

#===================================================================================================================
#row totals
rowtots<-matrix(nrow = lenA, ncol=1)
for (i in 1:lenA) {
	temp = 0 
	for (j in 1:lenB) {
		temp=temp+fish[i,j]
		}
	rowtots[i,1]=temp
}

#column totals
coltots<-matrix(nrow = 1, ncol=lenB)
for (i in 1:lenB) {
	temp = 0 
	for (j in 1:lenA) {
		temp=temp+fish[j,i]
		}
	coltots[1,i]=temp
}

#Grand total
grtot = 0 
for (i in 1:lenB) {
	grtot=grtot+coltots[1,i]
}

#Expected total
chitest=1
expy<-matrix(nrow = lenA, ncol=lenB)
for (i in 1:lenA) {
	for (j in 1:lenB) {
		expy[i,j] = coltots[1,j]*rowtots[i,1]/grtot
		if (expy[i,j] <=4) {chitest = chitest+1}
	}		
}
expz<-matrix(nrow = lenA, ncol=lenB)

for(i in 1:lenA) {
	for (j in 1:lenB) {
		expz[i,j]=format(round(expy[i,j], 2), nsmall=2, scientific=FALSE)
	}
}
expz <- cbind(expz, rowtots)
te <- cbind(coltots, grtot)
expz <-rbind(expz, te)
expdata <- data.frame(expz)

#Row title
rowz<-matrix(nrow=(lenA+1), ncol=1)
for (i in 1:lenA) {
	rowz[i,1]=unique(levels(statdata$zzzTreatmentAzzz)[i])
}
rowz[(lenA+1),1]="Row totals"
expdata <- cbind (rowz, expdata)

#column total
colz<-matrix(nrow=1, ncol=(lenB+1))
for (i in 1:lenB) {
	colz[1,i]=unique(levels(statdata$zzzTreatmentBzzz)[i])
}
colz[1,(lenB+1)]="Column totals"

colnames(expdata)<-c(" ", colz)

#===================================================================================================================
#Print of Expected table
HTML.title("Table of expected results (under the null hypothesis of no association)", HR=2, align="left")
HTML(expdata, classfirstline="second", align="left", row.names = "FALSE")
HTML("The values in this table are the expected results, given the row and column totals, under the assumption of no association between the two factors.", align="left")

#===================================================================================================================
#ChiSquaredTest
#===================================================================================================================
if (ChiSquaredTest == "Y") {
	HTML.title("Chi-squared test", HR=2, align="left")

	options<-(scipen=20)
	chi<-chisq.test(fishdata)
	pvalue<-format(round(chi$p.value, 4), nsmall=4, scientific=FALSE)
	for (i in 1:(length(pvalue))) {
		if (chi$p.value<0.0001) {
			pvalue[i]=format(round(0.0001, 4), nsmall=4, scientific=FALSE)
			pvalue[i]<- paste("<",pvalue[i])
		}
	}
	Wstat<- format(round(chi$statistic,2),nsmall=2)
	df<- format(round(chi$parameter,0),nsmall=0)

	#STB May 2012 adding blanks
	colname<-c(" " , "Test statistic", "Degrees of freedom", "p-value")
	rowname<-c("Result")
	temptab<-cbind(Wstat, df, pvalue)
	temptable<-data.frame(temptab)
	temptable<- cbind(rowname, temptable)
	colnames(temptable)<-colname
	row.names(temptable)<-rowname
	HTML(temptable, classfirstline="second", align="left", row.names = "FALSE")		

	if (lenA==2 && lenB==2) {
		HTML.title("Note: For the 2 x 2 case, the chi-squared test is calculated with Yates' continuity correction.", HR=0, align="left")
	}

	if (chi$p.value <= (1-sig))
		{
		add<-paste("The chi-squared test is significant at the ", 100*(1-sig), "% level of significance as the p-value is less than ", 1-sig, ".", sep="")
		} else {
		add<-paste("The chi-squared test is not significant at the ", 100*(1-sig), "% level of significance as the p-value is greater than ", 1-sig, ".", sep="")
		}
		HTML(add, align="left")

	#Warning
	if (chitest >= 2) {
		HTML("Note: As a rule of thumb the chi-squared test results may not be valid for small datasets, for example when one or more of the expected values are less than 5.", align="left")
	}
}

#===================================================================================================================
#Fisher's Exact test
#===================================================================================================================
if (FishersExactTest == "Y") {
	HTML.title("Fisher's exact test", HR=2, align="left")

	options<-(scipen=20)
	fisher<-fisher.test(x=fishdata, alternative=hyp)
	pvalue<-format(round(fisher$p.value, 4), nsmall=4, scientific=FALSE)
	for (i in 1:(length(pvalue)))  {
		if (fisher$p.value<0.0001) {
			pvalue[i]=format(round(0.0001, 4), nsmall=4, scientific=FALSE)
			pvalue[i]<- paste("<",pvalue[i])	
		}
	}
	colname<-c(" ", "p-value")
	rowname<-c("Result")
	temptab<-cbind(pvalue)
	temptable<-data.frame(temptab)
	temptable <- cbind(rowname, temptable)
	colnames(temptable)<-colname
	row.names(temptable)<-rowname
	HTML(temptable, classfirstline="second", align="left", row.names = "FALSE")		

	if (FishersHypothesis != "Two-sided" && (lenA > 2 || lenB > 2)) {
		HTML("A one-sided Fisher's exact test is only available for 2 by 2 contingency tables, hence the two-sided test is presented.", align="left")
	}

	if (FishersHypothesis != "Two-sided" && lenA == 2 && lenB==2) {
		add<-paste("The p-value presented is " , hyptest , ".", sep="")
		HTML(add, align="left")
	}

	if (fisher$p.value <= (1-sig)) {
		add<-paste("The Fisher's exact test is significant at the ", 100*(1-sig), "% level of significance as the p-value is less than ", 1-sig, ".", sep="")
	} else {
		add<-paste("The Fisher's exact test is not significant at the ", 100*(1-sig), "% level of significance as the p-value is greater than ", 1-sig, ".", sep="")
	}
	HTML(add, align="left")
}

#===================================================================================================================
#Barnards test (need to sort out row=Factor 1 being conditional)
#===================================================================================================================
if (BarnardsExactTest == "Y") {
	HTML.title("Barnard's exact test", HR=2, align="left")

	if (row ==2 && col == 2) {
		fishdata2<-as.matrix(fishdata)

		BT<-exact.test(fishdata2, alternative = hypB, npNumbers = 1000, interval = FALSE, method = "Z-pooled", model = "Binomial", cond.row=TRUE, to.plot=FALSE, ref.pvalue=TRUE)
		pvalueBT<-format(round(BT$p.value, 4), nsmall=4, scientific=FALSE)
		pvalueBT<-format(round(BT$p.value, 4), nsmall=4, scientific=FALSE)
		for (i in 1:(length(pvalueBT)))  {
			if (BT$p.value<0.0001) {
				pvalueBT[i]=format(round(0.0001, 4), nsmall=4, scientific=FALSE)
				pvalueBT[i]<- paste("<",pvalueBT[i])	
			}
		}
		temptabBT<-cbind(pvalueBT)

		colname<-c(" " , "p-value")
		rowname<-c("Result")
		temptableBT<-data.frame(temptabBT)
		temptableBT<-cbind(rowname, temptableBT)
		colnames(temptableBT)<-colname
		row.names(temptableBT)<-rowname
		HTML(temptableBT, classfirstline="second", align="left", row.names = "FALSE")	
		HTML("The p-value presented is two-sided.", align="left")

		if (BT$p.value <= (1-sig)) {
			add<-paste("The two-sided Barnard's exact test is significant at the ",  100*(1-sig), "% level of significance as the p-value is less than ", 1-sig, ".", sep="")
		} else {
			add<-paste("The two-sided Barnard's exact test is not significant at the ",  100*(1-sig), "% level of significance as the p-value is greater than ", 1-sig, ".", sep="")
		}
		HTML(add, align="left")

		HTML("Barnard's test is a more powerful test than Fisher's exact test in certain situations as it is an unconditional test, see Lydersen et al. (2009). This test only assumes the row totals are fixed and not the column totals, unlike Fisher's exact test.", align="left")
	} else {
		HTML("Barnard's test can only be used to analyse 2 x 2 contingency tables.", align="left")
	}
}


#===================================================================================================================
#References
#===================================================================================================================
Ref_list<-R_refs()

HTML.title("Statistical references", HR=2, align="left")
HTML(Ref_list$BateClark_ref, align="left")

if(FishersExactTest == "Y"&& row ==2 && col == 2)
{
	HTML("Lydersen S, Fagerland MW and Laake P. (2009). Recommended tests for association in 2 x 2 tables. Statistics in Medicine, 28, 1159-1175.", align="left")
}

HTML.title("R references", HR=2, align="left")
HTML(Ref_list$R_ref  ,  align="left")
HTML(Ref_list$R2HTML_ref, align="left")

if (BarnardsExactTest == "Y" && row ==2 && col == 2)
{
	HTML(Ref_list$Barnard_ref, align="left")
}

#===================================================================================================================
#Show dataset
#===================================================================================================================
printdata<-data.frame(eval(parse(text = paste("statdata$", response))))
printdata$B<-as.factor(eval(parse(text = paste("statdata$", GroupingFactor))))
printdata$C<-as.factor(eval(parse(text = paste("statdata$", ResponseCategories))))
colnames(printdata)<-c(response, GroupingFactor, ResponseCategories)

observ <- data.frame(c(1:dim(printdata)[1]))
colnames(observ) <- c("Observation")
printdata2 <- cbind(observ, printdata)

if (showdataset =="Y") {
    HTML.title("Analysis dataset", HR = 2, align = "left")
    HTML(printdata2, classfirstline = "second", align = "left", row.names = "FALSE")
}

#===================================================================================================================
#Show arguments
#===================================================================================================================
HTML.title("Analysis options", HR=2, align="left")

HTML(paste("Response variable: ", response, sep=""), align="left")
HTML(paste("Grouping variable: ", GroupingFactor, sep=""),  align="left")
HTML(paste("Response categories variable: ", ResponseCategories, sep=""),  align="left")
HTML(paste("Chi-squared test (Y/N): ", ChiSquaredTest, sep=""),  align="left")
HTML(paste("Fisher's Exact test (Y/N): ", FishersExactTest, sep=""),  align="left")
HTML(paste("Barnard's Exact test (Y/N): ", BarnardsExactTest, sep=""),  align="left")
HTML(paste("One- or two-sided test: ", FishersHypothesis, sep=""),  align="left")
HTML(paste("Significance level: ", 1-sig, sep=""),  align="left")
