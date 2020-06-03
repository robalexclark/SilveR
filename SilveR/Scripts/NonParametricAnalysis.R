#===================================================================================================================
#R Libraries

suppressWarnings(library(mvtnorm))
suppressWarnings(library(R2HTML))
suppressWarnings(library(coin))

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

#source(paste(getwd(),"/Common_Functions.R", sep=""))

#Print args
if (Diplayargs == "Y"){
	print(Args)
}

#===================================================================================================================
#Setup the html file and associated css file

htmlFile <- sub(".csv", ".html", Args[3])
#determine the file name of the html file
HTMLSetFile(file = htmlFile)
.HTML.file = htmlFile

#===================================================================================================================
#Parameter setup 

#Graphical parameter setup
graphdata <- statdata


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
#Titles and description
#===================================================================================================================
#Module Title
Title <-paste(branding, " Non-parametric Analysis", sep="")
HTML.title(Title, HR = 1, align = "left")

#Software developement version warning
if (Betawarn == "Y") {
	HTML.title("Warning", HR=2, align="left")
	HTML(BetaMessage, align="left")
}

#Response title
title <- c("Response")
HTML.title(title, HR = 2, align = "left")

#Description
add <- paste(c("The  "), response, sep = "")
add <- paste(add, " response is currently being analysed by the Non-parametric Analysis module", sep = "")

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

#===================================================================================================================
#Steels function from package
#===================================================================================================================
"npmc" <-
function(dataset, control=NULL, df=2, alpha=0.05)
{
	mvtnorm <- require(mvtnorm, quietly=TRUE);
	if (!mvtnorm)
	{
    		msg <- paste("npmc requires the mvtnorm-package to calculate",
                	"p-values for the test-statistics. This package is not",
                 	"available on your system, so these values and the",
                 	"confidence-limits will be missing values in the output!\n",
                 	sep="\n");
   		warning(msg);
  	}
  
  	if (any(df==0:2)) 
    		dfm <- c(3,2,1)[df+1]
  	else
  	{
    		warning("Wrong value for df\nUsing Satterthwaite t-approximation\n");
    		dfm <- 1;
  	}

  	if (alpha<=0 || alpha>=1)
    		stop("alpha must be a value between 0 and 1");

	name <- attr(dataset, "name");
  	desc <- attr(dataset, "description");

#=== Function definitions ===================================================

# ssq: Calculates a vector's sum of squares
  	ssq <- function(x) sum(x*x);

# force.ps: Forces a matrix to be positive semidefinite by replacing all negative eigenvalues by zero.
    	force.ps <- function(M.in)
  	{
    		eig <- eigen(M.in, symmetric=TRUE);
    		spec <- eig$values;
    		if (adjusted <- any(spec<0))
    		{
      			spec[spec<0] <- 0;
      			M <- eig$vectors %*% diag(spec) %*% t(eig$vectors);
      			ginv <- diag(1/sqrt(diag(M)));
      			M.out <- ginv %*% M %*% ginv;
      			##if ((msg <- all.equal(M.in,M.out))!=TRUE) attr(M.out, "msg") <- msg;
    		} else
    		{
      			M.out <- M.in;
    		}
    		attr(M.out,"adjusted") <- adjusted; 
    		return (M.out);
  	}

# z.dist: Calculates the p-values for the teststatistics using the mvtnorm-package.
# The 'sides' parameter determines whether the p-values for the one-or two-sided test are calculated.
# The statistic is supposed to follow a multivariate t-statistic with correlation-matrix 'corr' and 'df' degrees of freedom. If df=0, the
# multivariate normal-distribution is used.
# We use the mvtnorm-package by Frank Bretz (www.bioinf.uni-hannover.de) to calculate the corresponding p-values. These algorithms originally 
# calculate the value P(X<=stat) for the mentioned multivariate distributions, i.e. the 1-sided p-value. In order to gain the 2-sided p-value as well, 
# we used the algorithms on the absolute value of the teststatistic in combination with the inflated correlation-matrix kronecker(matrix(c(1,-1,-1,1),ncol=2), corr)

  	z.dist <- function(stat, corr, df=0, sides=2)
  	{
    		if (!mvtnorm) return (NA);
    
    		if (sides==2)
    		{
      			corr <- kronecker(matrix(c(1,-1,-1,1),ncol=2), corr);
      			stat <- abs(stat);
    		}
    		n <- ncol(corr);
    		sapply(stat, function(arg) 
         	{
          		mvtnorm::: mvt(
               		lower=rep(-Inf, n), 
               		upper=rep(arg, n), 
               		df=df, 
               		corr=corr, 
               		delta=rep(0,n)
               		)$value;
         	});     
  	} 

# z.quantile: Calculates the corresponding quantiles of z.dist p-values (used for the confidence-intervals)
 
  	z.quantile <- function(p=0.95, start=0, corr, df=0, sides=2)
  	{
    		if (!mvtnorm) return (NA);

    		if (z.dist(start,corr=corr,df=df,sides=sides) < p)
    		{
      			lower <- start;
      			upper <- lower+1;
      			while(z.dist(upper,corr=corr,df=df,sides=sides) < p)
        		upper <- upper+1;
    		} else  {
      			upper <- start;
      			lower <- upper-1;
      			while(z.dist(lower,corr=corr,df=df,sides=sides) > p)
        		lower <- lower-1;
    			}
    		ur <- uniroot(f=function(arg) p-z.dist(arg,corr=corr,df=df,sides=sides), upper=upper, lower=lower);
    		ur$root;
  	}
 
#=== Calculations ===========================================================
      
# sort the dataset by factor
  	dataset$class <- factor(dataset$class);
  	datord <- order(dataset$class);
  	attrs <- attributes(dataset);
  	dataset <- data.frame(lapply(dataset, "[", datord));
  	attributes(dataset) <- attrs;
  
# general data characteristics
  	attach(dataset);
  	fl <- levels(class);             # factor-levels
  	a <- nlevels(class);             # number of factor-levels
  	samples <- split(var, class);    # split the data in separate sample-vectors
  	n <- sapply(samples, length);    # sample sizes
  	detach(dataset);

  	if (is.null(control))
  	{
    		# create indexing vectors for the all-pairs situation
    		tmp <- expand.grid(1:a, 1:a);
    		ind <- tmp[[1]] > tmp[[2]];
    		vi <- tmp[[2]][ind];
    		vj <- tmp[[1]][ind];
  	} else 
	{
    		## create indexing vectors for the many-to-one situation
    		if (!any(fl==control))
    		{
      			msg <- paste("Wrong control-group specification\n",
                   	"The data does not contain a group with factor-level ", control, sep="");
      			stop(msg, FALSE);
    		}
    		cg <- which(fl==control);
    		vi <- which((1:a)!=cg);
    		vj <- rep(cg, a-1);
  	}

 # number of comparisons ( a*(a-1)/2 for all-pairs, (a-1) for many-to-one )
  	nc <- length(vi);              
  
 # labels describing the compared groups 
  	cmpid <- paste(vi, "-", vj, sep="");
  
 # pairwise pooled sample-sizes
  	gn <- n[vi]+n[vj];
 
# internal rankings
  	intRanks <- lapply(samples, rank);
 
# pairwise rankings
  	pairRanks <- lapply(1:nc, function(arg) 
                    {
                      rank(c(samples[[vi[arg]]], samples[[vj[arg]]]));  
                    });

# estimators for the relative effects
  	pd <- sapply(1:nc, function(arg)
        	{
               		i <- vi[arg]; 
               		j <- vj[arg];
               		(sum(pairRanks[[arg]][(n[i]+1):gn[arg]])/n[j]-(n[j]+1)/2)/n[i];  
             	});
    
# Calculations for the BF-test ###################################
	dij <- dji <- list(0);

  	sqij <- sapply(1:nc, function(arg) 
               	{
                 	i <- vi[arg]; 
                 	j <- vj[arg];
                 	pr <- pairRanks[[arg]][(n[i]+1):gn[arg]];
                 	dij[[arg]] <<- pr - sum(pr)/n[j] - intRanks[[j]] + (n[j]+1)/2;
                 	ssq(dij[[arg]])/(n[i]*n[i]*(n[j]-1));
               });
  
  	sqji <- sapply(1:nc, function(arg)
               {
                 	i <- vi[arg];  
                 	j <- vj[arg];
                 	pr <- pairRanks[[arg]][1:n[i]];
                 	dji[[arg]] <<- pr - sum(pr)/n[i] - intRanks[[i]] + (n[i]+1)/2;
                 	ssq(dji[[arg]])/(n[j]*n[j]*(n[i]-1));
               });

# diagonal elements of the covariance-matrix
  	vd.bf <- gn*(sqij/n[vj] + sqji/n[vi]);

# mark and correct zero variances for further calculations
  	singular.bf <- (vd.bf==0);
  	vd.bf[singular.bf] <- 0.00000001;
  
# standard-deviation
  	std.bf <- sqrt(vd.bf/gn);

# teststatistic
  	t.bf <- (pd-0.5)*sqrt(gn/vd.bf);
  
# Satterthwaite approxiamtion for the degrees of freedom
  	df.sw <- (n[vi]*sqij + n[vj]*sqji)^2 / ((n[vi]*sqij)^2/(n[vj]-1) + (n[vj]*sqji)^2/(n[vi]-1));
  	df.sw[is.nan(df.sw)] <- Inf;

# choose degrees of freedom 
  	df <- if (dfm<3) max(1, if (dfm==2) min(gn-2) else min(df.sw)) else 0;

# Calculations for the Steel-test ################################
# the Steel-type correlation factors
  	lambda <- sqrt(n[vi]/(gn+1));
      
# diagonal elements of the covariance-matrix
  	vd.st <- sapply(1:nc, function(arg) ssq(pairRanks[[arg]]-(gn[arg]+1)/2)) / (n[vi]*n[vj]*(gn-1));

# mark and correct zero variances for further calculations
  	singular.st <- (vd.st==0);
  	vd.st[singular.st] <- 0.00000001;
  
# standard-deviation
  	std.st <- sqrt(vd.st/gn);

# teststatistic
  	t.st <- (pd-0.5)*sqrt(gn/vd.st);
  
# Calculate the correlation-matrices (for both, BF and Steel) ####
	rho.bf <- rho.st <- diag(nc);
  	for (x in 1:(nc-1))
  	{
    		for (y in (x+1):nc)
    		{
      			i <- vi[x]; j <- vj[x];
      			v <- vi[y]; w <- vj[y];
      			p <- c(i==v, j==w, i==w, j==v);
      			if (sum(p)==1) 
      			{      
        			cl <- list(
                   			function()  (t(dji[[x]]) %*% dji[[y]]) / (n[j]*n[w]*n[i]*(n[i]-1)),
                   			function()  (t(dij[[x]]) %*% dij[[y]]) / (n[i]*n[v]*n[j]*(n[j]-1)),
                   			function() -(t(dji[[x]]) %*% dij[[y]]) / (n[i]*n[w]*n[i]*(n[i]-1)),
                   			function() -(t(dij[[x]]) %*% dji[[y]]) / (n[j]*n[v]*n[j]*(n[j]-1))
                   			);
        			case <- (1:4)[p];
        			rho.bf[x,y] <- rho.bf[y,x] <- 
          			sqrt(gn[x]*gn[y]) / sqrt(vd.bf[x]*vd.bf[y]) * cl[[case]]()
        			;
        			rho.st[x,y] <- rho.st[y,x] <- 
        			{
					if (case>2) -1 else 1}*lambda[x]*lambda[y]
        				;
      			}
    		}
  	}
  	rho.bf <- force.ps(rho.bf);
  	rho.st <- force.ps(rho.st);
      

# Calculate the p-values     (BF and Steel) ######################
	p1s.bf <- 1 - z.dist(t.bf, corr=rho.bf, df=df, sides=1);
  	p2s.bf <- 1 - z.dist(t.bf, corr=rho.bf, df=df, sides=2);
   
  	p1s.st <- 1 - z.dist(t.st, corr=rho.st, sides=1);
  	p2s.st <- 1 - z.dist(t.st, corr=rho.st, sides=2);

# Calculate the confidence-limits (BF and Steel) #################
	z.bf <- z.quantile(1-alpha, corr=rho.bf, df=df, sides=2);
  	lcl.bf <- pd - std.bf*z.bf;
  	ucl.bf <- pd + std.bf*z.bf;

  	z.st <- z.quantile(1-alpha, corr=rho.st, sides=2);
  	lcl.st <- pd - std.st*z.st;
  	ucl.st <- pd + std.st*z.st;
 
#=== Output ==================================================================
    
# Create the result-datastructures ###############################
  	dataStructure <- data.frame("group index"=1:a, 
                              "class level"=fl, 
                              "nobs"=n
                              );
  
  	test.bf <- data.frame("cmp"=cmpid, 
                        "gn"=gn, 
                        "effect"=pd,
                        "lower.cl"=lcl.bf,
                        "upper.cl"=ucl.bf,
                        "variance"=vd.bf, 
                        "std"=std.bf, 
                        "statistic"=t.bf, 
                        "p-value 1s"=p1s.bf, 
                        "p-value 2s"=p2s.bf, 
                        "zero"=singular.bf
                        ); 

  	test.st <- data.frame("cmp"=cmpid, 
                        "gn"=gn, 
                        "effect"=pd, 
                        "lower.cl"=lcl.st,
                        "upper.cl"=ucl.st,
                        "variance"=vd.st, 
                        "std"=std.st, 
                        "statistic"=t.st, 
                        "p-value 1s"=p1s.st, 
                        "p-value 2s"=p2s.st, 
                        "zero"=singular.st
                        ); 

  	result <- list("data"=dataset,
                 "info"=dataStructure, 
                 "corr"=list("BF"=rho.bf, "Steel"=rho.st),
                 "test"=list("BF"=test.bf, "Steel"=test.st),
                 "control"=control,
                 "df.method"=dfm,
                 "df"=df,
                 "alpha"=alpha
                 );
  
  	class(result) <- "npmc";
  
  	return (result);
 
}

"print.npmc" <- function(x, ...)
{
	print(x$test, ...)
}

"report" <- function(msg=NULL, style=0, char="-")
{
	if (is.null(msg)) msg <- "";
 
	if (is.vector(msg))
	msg <- unlist(msg)
 
	else
 	stop("msg must be of type vector");
  
  	char <- substr(char, 1, 1);
  	underlined <- function (arg)
  	{
    		c(arg, paste(rep(char, max(nchar(msg))), collapse=""));
  	}

  	border <- function(arg) 
  	{
    		n <- length(msg);
    		ml <- max(nchar(msg));
    		space <- paste(rep(" ", ml), collapse="");
    		line <- paste(rep(char, ml+4), collapse="");
    		msg <- paste(msg, substr(rep(space, n), rep(1, n), ml-nchar(msg)), sep=""); 
    		c(line, paste(char, msg, char), line);          
  	}

  	sfun <- list(underlined = underlined, border = border);
  
  	if (is.numeric(style) && length(style)==1 && any(style==1:length(sfun)))
    	msg <- sfun[[style]](msg)
  	else if (is.character(style) && length(style)==1 && !is.null(sfun[[style]]))
    	msg <- sfun[[style]](msg)
  
  	m <- matrix(msg, ncol=1);
  	colnames(m) <- "";
  	rownames(m) <- rep("", length(msg));
  	print.noquote(m); 
}



"summary.npmc" <- function(object, type="both", info=FALSE, short=TRUE, corr=FALSE, ...)
{
  	x <- object;
  	if (info)
  	{
    		name <- attr(data, "name");
    		desc <- attr(data, "desc");
    		df <- x$df;
    		df.method <- x$df.method;
    		alpha <- x$alpha;
  
    		apm <- c(paste("Satterthwaite t-approximation (df=",df,")",sep=""),
             		paste("simple t-approximation (df=",df,")",sep=""),
             		"standard normal approximation");
    		msg <- c(paste("npmc executed", if (!is.null(name)) paste("on", name)),
             		if (is.null(desc)) "" else c("","Description:",desc,""),
             		"NOTE:",
             		paste("-Used", apm[df.method]),
             		paste("-Calculated simultaneous (1-", alpha, ") confidence intervals",sep=""),
             		"-The one-sided tests 'a-b' reject if group 'a' tends to",
             		" smaller values than group 'b'"
             		);
    		report(msg, style=2, char="/");
    		report();
  	}

  	if (short)
 	 {
    		bf <- st <- c("cmp","effect","lower.cl","upper.cl","p.value.1s","p.value.2s");
 	 }
 	 else
 	 {
    		bf  <- names(x$test$BF);
    		st <- names(x$test$Steel);
 	 }

  
  	content <- list();
  	if (info)
    		content <- c(content, list("Data-structure"=x$info));
  	if (corr && type!="Steel")
    		content <- c(content, list("Behrens-Fisher type correlation-matrix"=x$corr$BF));
  	if (corr && type!="BF")
   		 content <- c(content, list("Steel type correlation-matrix"=x$corr$Steel));
  	if (type!="Steel")
    		content <- c(content, list("Results of the multiple Behrens-Fisher-Test"=x$test$BF[bf]));
 	 if (type!="BF")
    		content <- c(content, list("Results of the multiple Steel-Test"=x$test$Steel[st]));
  
  	#h <- (list("Data-structure"=x$info, 
  	#           "Behrens-Fisher type correlation-matrix"=x$corr$BF, 
  	#           "Steel type correlation-matrix"=x$corr$Steel,
  	#           "Results of the multiple Behrens-Fisher-Test"=x$test$BF[bf], 
  	#           "Results of the multiple Steel-Test"=x$test$Steel[st]
  	#           ));
#  print(content);
content
}

#===================================================================================================================
#Summary stats table
#===================================================================================================================
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

HTML.title("Summary data", HR = 2, align = "left")
HTML(table, classfirstline = "second", align = "left", row.names = "FALSE")

#===================================================================================================================
#Boxplot of data
#===================================================================================================================
HTML.title("Boxplot", HR = 2, align = "left")

#Graphics parameter setup
graphdata$xvarrr_IVS_BP <- as.factor(eval(parse(text = paste("graphdata$", treatment))))
graphdata$yvarrr_IVS <- eval(parse(text = paste("graphdata$", response)))
MainTitle2 <- ""
BoxPlot <- "Y"
FirstCatFactor <- "NULL"
SecondCatFactor <- "NULL"
ReferenceLine <- "NULL"
BoxplotOptions <- "Outliers"

#Creating outliers dataset for the boxplot
temp <- IVS_F_boxplot_outlier()
outlierdata <- temp$outlierdata
boxdata <- temp$boxdata
ymin <- temp$ymin
ymax <- temp$ymax
range <- temp$range

#Plot device setup
ncboxplot <- sub(".html", "ncboxplot.png", htmlFile)
png(ncboxplot, width = jpegwidth, height = jpegheight, units="in", res=PlotResolution)
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
    linkToPdfx <- paste("<a href=\"", pdfFile_x, "\">Click here to view the PDF of the boxplot</a>", sep = "")
    HTML(linkToPdfx)
}

#Footnote to boxplot
HTML("On the boxplot the median is denoted by the horizontal line within the box. 
The box indicates the interquartile range, where the lower and upper quartiles are calculated using the type 2 method, see Hyndman and Fan (1996). 
The whiskers extend to the most extreme data point which is no more than 1.5 times the length of the box away from the box. 
Individual observations that lie outside the outlier range are included on the plot using stars.", align = "left")

#===================================================================================================================
#Kruskall-Wallis and Mann Whitney
#===================================================================================================================
if (block == "NULL" && (statstest == "MannWhitney" || (statstest == "AllComparisons" && leng == 2) || (statstest == "CompareToControl" && leng == 2))) {
    #Kruskal Wallis and Mann Whitney
    dim <- length(unique(eval(parse(text = paste("statdata$", treatment)))))

    if (dim > 2) {
        HTML.title("Kruskal-Wallis test", HR = 2, align = "left")

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
        HTML.title("Mann-Whitney test", HR = 2, align = "left")
        comm <- c("You have selected a factor with only two levels, hence a Mann-Whitney test, also know as a Wilcoxon rank sum test, has been used to analyse the data rather than a Kruskal-Wallis test.")
        HTML(comm, align = "left")

        wilcoxOut <- wilcox.test(as.formula(respVTreat), data = statdata, exact = TRUE)

        #Assessing test type
        TestTp <- "Exact"
        if ((length(unique(eval(parse(text = paste("statdata$", response))))) < length((eval(parse(text = paste("statdata$", treatment))))) || (length(eval(parse(text = paste("statdata$", response)))) > 49))) {
            TestTp <- "Asymptotic"
        }

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
        temptab <- data.frame(cbind(rowname, Wstat, pvalue, TestTp))
        colname <- c(" ", "Test statistic", "p-value", "p-value type")
        colnames(temptab) <- colname

        HTML(temptab, classfirstline = "second", align = "left", row.names = "FALSE")

        if ((length(unique(eval(parse(text = paste("statdata$", response))))) == length((eval(parse(text = paste("statdata$", treatment)))))) && (length(eval(parse(text = paste("statdata$", response)))) < 50)) {
            HTML("As there are no ties in the responses, and the number of responses is less than 50, the exact test result has been calculated.", align = "left")
        }

        if ((length(unique(eval(parse(text = paste("statdata$", response))))) < length((eval(parse(text = paste("statdata$", treatment))))) || (length(eval(parse(text = paste("statdata$", response)))) > 49))) {
            HTML("As there are ties in some of the responses, and/or the number of responses is greater than 50, the asymptotic test result has been calculated.", HR = 0, align = "left")
        }
    }
}

#===================================================================================================================
#Friedmans test
#===================================================================================================================
if (block != "NULL" && statstest == "MannWhitney") {
    #Friedmans test
    HTML.title("Friedman Rank Sum test", HR = 2, align = "left")

    if (RunFried == "Na") {
        HTML("As not all combinations of the treatment factor and the other design (blocking) factor are present in the design, Friedman's test cannot be performed using this module. An alternative approach is to analyse the data using the Single or Repeated Measures Parametric Analysis modules, applying the rank transformation to the responses. ", align = "left")
    }

    if (RunFried == "Nb") {
        HTML("As one of the treatment factor levels occurs multiple times with one of the other design (blocking) factor levels, Friedman's test cannot be performed using this module. An alternative approach is to analyse the data using the Single or Repeated Measures Parametric Analysis modules, applying the rank transformation to the responses. ", align = "left")
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

#===================================================================================================================
#All comparisons tests - generating comparison labels
#===================================================================================================================
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

#===================================================================================================================
#All comparisons tests - no blocks
#===================================================================================================================
if (block == "NULL" && (statstest == "AllComparisons" && leng >= 3)) {
    # Behrens Fisher test results
    HTML.title("All pairwise comparisons - Behrens Fisher tests", HR = 2, align = "left")

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
    HTML.title("All pairwise comparisons - Mann-Whitney tests", HR = 2, align = "left")

    INDEX <- 1
    pv_list <- c()
    pvzzz_list <- c()
    TestTpe_list <- c()

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
            wilcox <- wilcox.test(resp ~ grou, exact = TRUE)

            #Generating regarding exact tests
            TestTpe <- "Exact"
            if ((length(unique(resp)) < length((grou)) || (length(resp) > 49))) {
                INDEX <- INDEX + 1
                TestTpe <- "Asymptotic"
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
                } else {
                    pv[i] <- paste(" ", pv[i])
                }
            }

            #merge results for all pairwise tests
            pv_list[index] <- pv
            pvzzz_list[index] <- pvzzz
            TestTpe_list[index] <- TestTpe
            index <- index + 1
        }
    }

    #create final table
    tabletemp <- data.frame(good3, allgroup1, allvs, allgroup2, pv_list, TestTpe_list)
    temp16 <- c("Comparison Number", "Gp 1", "vs.", "Gp 2", "p-value", "p-value type")
    colnames(tabletemp) <- temp16

    HTML(tabletemp, classfirstline = "second", align = "left", row.names = "FALSE")

    #Message regarding exact tests
    if (INDEX == 1) {
        HTML("As there are no ties in the responses, and the number of responses in each group is less than 50, the exact Mann-Whitney test results has been calculated.", align = "left")
    } else {
        HTML("As there are ties in some of the responses, and/or some of the groups have more than 50 responses, the asymptotic Mann-Whitney test results has been calculated in these cases.", align = "left")
    }

    HTML("Why are there two different tests presented? The Behrens Fisher tests are the recommended approach, although we still need to independently verify these results.", align = "left")
}

#===================================================================================================================
#All comparisons tests - blocks
#===================================================================================================================
if (block != "NULL" && statstest == "AllComparisons") {
    # Wilcoxon signed rank tests
    if (int > 2) {
        HTML.title("All pairwise comparisons - Wilcoxon Signed Rank tests", HR = 2, align = "left")
    } else {
        HTML.title("All pairwise comparisons - Wilcoxon Signed Rank test", HR = 2, align = "left")
    }

    if (RunFried == "Na") {
        HTML("As not all combinations of the treatment factor and the other design (blocking) factor are present in the design, all pairwise comparisons have not been performed. An alternative approach is to analyse the data using the Single or Repeated Measures Parametric Analysis modules, applying the rank transformation to the responses. ", align = "left")
    }

    if (RunFried == "Nb") {
        HTML("As one of the treatment factor levels occurs multiple times with one of the other design (blocking) factor levels, all pairwise comparisons have not been performed. An alternative approach is to analyse the data using the Single or Repeated Measures Parametric Analysis modules, applying the rank transformation to the responses. ", align = "left")
    }

    if (RunFried == "Y") {
        INDEX <- 1
        pv_list <- c()
        pvzzz_list <- c()
        TestTpe_list <- c()

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

                #Generating regarding exact tests
                interdataX3 <- interdata$X1 - interdata$X2
                TestTpe <- "Exact"

                #Wilcoxon test
                if ((length(unique(interdataX3)) == length((interdataX3))) && (length(interdata$X1) < 21 ) && (length(interdata$X2) < 21) ) {
                    wilcox <- wilcoxsign_test(interdata$X1 ~ interdata$X2, distribution = "exact", zero.method = "Wilcoxon")
                } else {
                    wilcox <- wilcoxsign_test(interdata$X1 ~ interdata$X2, distribution = "asymptotic", zero.method = "Wilcoxon")
                    INDEX <- INDEX + 1
                    TestTpe <- "Asymptotic"
                }

                #Reformat p-values
                pv <- format(round(as.numeric(pvalue(wilcox)), 4), nsmall = 4, scientific = FALSE)
                pvzzz <- as.numeric(pvalue(wilcox))
                for (i in 1:(length(pv))) {
                    if (pvzzz[i] < 0.0001) {
                        #STB March 2011 - formatting p-values p<0.0001
                        #pv[i]<-0.0001
                        pv[i] = format(round(0.0001, 4), nsmall = 4, scientific = FALSE)
                        pv[i] <- paste("<", pv[i])
                    } else {
                        pv[i] <- paste(" ", pv[i])
                    }
                }

                #merge results for all pairwise tests
                pv_list[index] <- pv
                pvzzz_list[index] <- pvzzz
                TestTpe_list[index] <- TestTpe
                index <- index + 1
            }
        }

        #create final table
        tabletemp <- data.frame(good3, allgroup1, allvs, allgroup2, pv_list, TestTpe_list)
        temp16 <- c("Comparison Number", "Gp 1", "vs.", "Gp 2", "p-value", "p-value type")
        colnames(tabletemp) <- temp16

        HTML(tabletemp, classfirstline = "second", align = "left", row.names = "FALSE")


        #Message regarding exact tests
        if (INDEX == 1) {
            HTML("As there are no ties in the response differences, and the number of responses in each group is less than 20, the exact test result has been calculated.", align = "left")
        } else {
            HTML("As there are ties in some of the response differences, and/or some of the groups have more than 20 responses, the asymptotic test result has been calculated in these cases.", align = "left")
        }
    }
}

#===================================================================================================================
#Comparisons back to control - no blocks
#===================================================================================================================
if (block == "NULL" && (statstest == "CompareToControl" && leng >= 3)) {
    #Steels all to one comparison
    HTML.title("Steel's all comparisons back to one", HR = 2, align = "left")

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


#===================================================================================================================
#Comparisons back to control - blocks
#===================================================================================================================
if (block != "NULL" && (statstest == "CompareToControl" )) {
    HTML.title("All comparisons back to one", HR = 2, align = "left")
    HTML("The 'all comparisons back to one' is not available as an 'Other design (block)' factor has been selected. Please use the  'all pairwise comparisons' option to generate the required results.", align = "left")
}
#===================================================================================================================
#Text for conclusions and comments 
#===================================================================================================================
Ref_list <- R_refs()

if (RunFried == "Y") {
    if (block == "NULL" && (statstest == "MannWhitney" || (statstest == "AllComparisons" && leng == 2) || (statstest == "CompareToControl" && leng == 2))) {
        HTML.title("Analysis conclusions", HR = 2, align = "left")
        dim <- length(unique(eval(parse(text = paste("statdata$", treatment)))))

        if (dim > 2) {
            if (temptab[1, 4] <= (1 - sig)) {
                add <- paste(c("There is a statistically significant overall difference between the treatment factor levels at the "), sep = "")
                add <- paste(add, 100 * (1 - sig), sep = "")
                add <- paste(add, "% level of significance as the p-value is less than ", sep = "")
                add <- paste(add, 1 - sig, sep = "")
                add <- paste(add, " (Kruskal-Wallis test).", sep = "")
                HTML(add, align = "left")
            } else if (temptab[1, 4] > (1 - sig)) {
                add <- paste(c("The overall difference between the treatment factor levels is not statistically significant at the "), sep = "")
                add <- paste(add, 100 * (1 - sig), sep = "")
                add <- paste(add, "% level of significance as the p-value is greater than ", sep = "")
                add <- paste(add, 1 - sig, sep = "")
                add <- paste(add, " (Kruskal-Wallis test).", sep = "")
                HTML(add, align = "left")
            }

            HTML.title("Analysis description", HR = 2, align = "left")
            HTML("The overall treatment effect was assessed using the non-parametric Kruskal-Wallis test, see Kruskal and Wallis (1952, 1953).", align = "left")
            HTML("Non-parametric tests should be used if the data is non-normally distributed, the variability of the responses varies across treatments or the responses are not continuous and numerical. ", align = "left")

            #Bate and Clark comment
            HTML(refxx, align = "left")

		if (UpdateIVS == "N") {
			HTML.title("Statistical references", HR=2, align="left")
		}
		if (UpdateIVS == "Y") {
			HTML.title("References", HR=2, align="left")
			HTML(Ref_list$IVS_ref, align="left")
		}
            HTML(Ref_list$BateClark_ref, align = "left")
            HTML("Hyndman, R.J. and Fan, Y. (1996). Sample quantiles in statistical packages. American Statistician 50, 361-365.", align = "left")
            HTML("Kruskal, W.H. and Wallis, W.A. (1952). Use of ranks in one criterion variance analysis. JASA, 47, 583-621.", align = "left")
            HTML("Kruskal, W.H. and Wallis, W.A. (1953). Errata for Kruskal-Wallis (1952). JASA, 48, 907-911.", align = "left")
        } else if (dim == 2) {
            if (wilcoxOut$p.value <= (1 - sig)) {
                add <- paste(c("The difference between the two treatment factor levels is statistically significant at the "), sep = "")
                add <- paste(add, 100 * (1 - sig), sep = "")
                add <- paste(add, "% level of significance as the p-value is less than ", sep = "")
                add <- paste(add, 1 - sig, sep = "")
                add <- paste(add, " (Mann-Whitney test).", sep = "")
                HTML(add, align = "left")
            } else if (wilcoxOut$p.value > (1 - sig)) {
                add <- paste(c("The difference between the two treatment factor levels is not statistically significant at the "), sep = "")
                add <- paste(add, 100 * (1 - sig), sep = "")
                add <- paste(add, "% level of significance as the p-value is greater than ", sep = "")
                add <- paste(add, 1 - sig, sep = "")
                add <- paste(add, " (Mann-Whitney test).", sep = "")
                HTML(add, align = "left")
            }

            HTML.title("Analysis description", HR = 2, align = "left")
            HTML("The difference between the two treatments was assessed using the non-parametric Mann-Whitney test with continuity correction, see Wilcoxon (1945), Mann and Whitney (1947). ", align = "left")
            HTML("Note that the literature is not unanimous about the definition of the Mann-Whitney test. The two most common definitions correspond to the sum of the ranks of the first sample with the minimum value subtracted or not: R subtracts. It seems Wilcoxon's original paper used the unadjusted sum of the ranks but subsequent tables subtracted the minimum. ", align = "left")
            HTML("Non-parametric tests should be used if the data is non-normally distributed, the variability of the responses varies across treatments or the responses are not continuous and numerical. ", align = "left")

            #Bate and Clark comment
            HTML(refxx,  align = "left")

		if (UpdateIVS == "N") {
			HTML.title("Statistical references", HR=2, align="left")
		}
		if (UpdateIVS == "Y") {
			HTML.title("References", HR=2, align="left")
			HTML(Ref_list$IVS_ref, align="left")
		}
            HTML(Ref_list$BateClark_ref, align = "left")
            HTML("Hyndman, R.J. and Fan, Y. (1996). Sample quantiles in statistical packages. American Statistician 50, 361-365.", align = "left")
            HTML("Mann, H.B. and Whitney, D.R. (1947). On a test of whether one of two random variables is stochastically larger than the other. Annals of Mathematical Statistics, 18, 50-60.", align = "left")
            HTML("Wilcoxon, F. (1945). Individual comparisons by ranking methods. Biometrics Bulletin, 1, 80-83.", align = "left")
        }
    }

    if (block != "NULL" && statstest == "MannWhitney") {
       HTML.title("Analysis conclusions", HR = 2, align = "left")
       dim <- length(unique(eval(parse(text = paste("statdata$", treatment)))))

        if (temptab[1, 4] <= (1 - sig)) {
            add <- paste(c("There is a statistically significant overall difference between the treatment factor levels at the "), sep = "")
            add <- paste(add, 100 * (1 - sig), sep = "")
            add <- paste(add, "% level of significance as the p-value is less than ", sep = "")
            add <- paste(add, 1 - sig, sep = "")
            add <- paste(add, " (Friedman test).", sep = "")
            HTML(add, align = "left")
        } else if (temptab[1, 4] > (1 - sig)) {
            add <- paste(c("The overall difference between the treatment factor levels is not statistically significant at the "), sep = "")
            add <- paste(add, 100 * (1 - sig), sep = "")
            add <- paste(add, "% level of significance as the p-value is greater than ", sep = "")
            add <- paste(add, 1 - sig, sep = "")
            add <- paste(add, " (Friedman test).", sep = "")
            HTML(add, align = "left")
        }

        HTML.title("Analysis description", HR = 2, align = "left")
        HTML("The overall treatment effect was assessed using the non-parametric Friedman test, see Hollander and Wolfe (1973).", align = "left")
        HTML("Non-parametric tests should be used if the data is non-normally distributed, the variability of the responses varies across treatments or the responses are not continuous and numerical. ", align = "left")

        #Bate and Clark comment
        HTML(refxx, align = "left")

 	if (UpdateIVS == "N") {
		HTML.title("Statistical references", HR=2, align="left")
	}
	if (UpdateIVS == "Y") {
		HTML.title("References", HR=2, align="left")
		HTML(Ref_list$IVS_ref, align="left")
	}
        HTML(Ref_list$BateClark_ref, align = "left")
        HTML("Hyndman, R.J. and Fan, Y. (1996). Sample quantiles in statistical packages. American Statistician 50, 361-365.", align = "left")
        HTML("Hollander, M. and Wolfe, D.A. (1973). Nonparametric Statistical Methods. New York: John Wiley & Sons.", align = "left")
    }


    if (block == "NULL" && (statstest == "AllComparisons" && leng >= 3)) {
      HTML.title("Analysis conclusions", HR = 2, align = "left")
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
            if (pvzzz_list[q] <= (1 - sig)) {
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
            add3 <- c("None of the pairwise Behrens Fisher tests are statistically significant at the ")
            add3 <- paste(add3, 100 * (1 - sig), sep = "")
            add3 <- paste(add3, "% level of significance as the p-values are all greater than ", sep = "")
            add3 <- paste(add3, 1 - sig, ".", sep = "")
            HTML(add3, align = "left")
        } else if (textindex > 1) {
            HTML(add, align = "left")
        }

        if (textindex2 == 1) {
            add3 <- c("None of the pairwise Mann-Whitney tests are statistically significant at the ")
            add3 <- paste(add3, 100 * (1 - sig), sep = "")
            add3 <- paste(add3, "% level of significance as the p-values are all greater than ", sep = "")
            add3 <- paste(add3, 1 - sig, ".", sep = "")
            HTML(add3, align = "left")
        } else if (textindex2 > 1) {
            HTML(add2, align = "left")
        }

        HTML.title("Analysis description", HR = 2, align = "left")
        HTML("All pairwise differences between the treatments were assessed using Behrens Fisher tests, see Munzel and Hothorn (2001) and Mann-Whitney tests with continuity correction, see Mann and Whitney (1947).", align = "left")
        HTML("Note that the literature is not unanimous about the definition of the Mann-Whitney test. The two most common definitions correspond to the sum of the ranks of the first sample with the minimum value subtracted or not: R subtracts. It seems Wilcoxon's original paper, Wilcoxon (1945), used the unadjusted sum of the ranks but subsequent tables subtracted the minimum. ", align = "left")
        HTML("Non-parametric tests should be used if the data is non-normally distributed, the variability of the responses varies across treatments or the responses are not continuous and numerical. ", align = "left")

        #Bate and Clark comment
        HTML(refxx,  align = "left")

	if (UpdateIVS == "N") {
		HTML.title("Statistical references", HR=2, align="left")
	}
	if (UpdateIVS == "Y") {
		HTML.title("References", HR=2, align="left")
		HTML(Ref_list$IVS_ref, align="left")
	}
        HTML(Ref_list$BateClark_ref, align = "left")
        HTML("Hyndman, R.J. and Fan, Y. (1996). Sample quantiles in statistical packages. American Statistician 50, 361-365.", align = "left")
        HTML("Mann, H.B. and Whitney, D.R. (1947). On a test of whether one of two random variables is stochastically larger than the other. Annals of Mathematical Statistics, 18, 50-60.", align = "left")
        HTML("Munzel, U. and Hothorn, L.A. (2001). A unified approach to simultaneous rank test procedures in the unbalanced one-way layout. Biometrical Journal, 43(5) 553-569.", align = "left")
        HTML("Wilcoxon, F. (1945). Individual comparisons by ranking methods. Biometrics Bulletin, 1, 80-83.", align = "left")
    }

    if (block != "NULL" && statstest == "AllComparisons") {
       HTML.title("Analysis conclusions", HR = 2, align = "left")
       textindex2 <- 1
        add2 <- c(" ")
        for (q in 1:(int - 1)) {
            if (pvzzz_list[q] <= (1 - sig)) {
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

        if (textindex2 == 1 && int == 2) {
            add3 <- c("The pairwise Wilcoxon Signed Rank test is not statistically significant at the ")
            add3 <- paste(add3, 100 * (1 - sig), sep = "")
            add3 <- paste(add3, "% level of significance as the p-value is greater than ", sep = "")
            add3 <- paste(add3, 1 - sig, ".", sep = "")
            HTML(add3, align = "left")
        } else if (textindex2 == 1 && int > 2) {
            add3 <- c("None of the pairwise Wilcoxon Signed Rank tests are statistically significant at the ")
            add3 <- paste(add3, 100 * (1 - sig), sep = "")
            add3 <- paste(add3, "% level of significance as the p-values are greater than ", sep = "")
            add3 <- paste(add3, 1 - sig, ".", sep = "")
            HTML(add3, align = "left")
        } else if (textindex2 > 1) {
            HTML(add2, align = "left")
        }

        HTML.title("Analysis description", HR = 2, align = "left")
        if (int > 2) {
            HTML("All pairwise differences between the treatments were assessed using Wilcoxon Signed Rank tests with continuity correction, see Hollander and Wolfe (1973).", align = "left")
        } else {
            HTML("All pairwise differences between the treatments were assessed using a Wilcoxon Signed Rank test with continuity correction, see Hollander and Wolfe (1973).", align = "left")
        }
        HTML("Note that the literature is not unanimous about the definition of the Wilcoxon Signed Rank test. The two most common definitions correspond to the sum of the ranks of the first sample with the minimum value subtracted or not: R subtracts. It seems Wilcoxon's original paper used the unadjusted sum of the ranks but subsequent tables subtracted the minimum. ", align = "left")
        HTML("Non-parametric tests should be used if the data is non-normally distributed, the variability of the responses varies across treatments or the responses are not continuous and numerical. ", align = "left")

        #Bate and Clark comment
        HTML(refxx,  align = "left")

	if (UpdateIVS == "N") {
		HTML.title("Statistical references", HR=2, align="left")
	}
	if (UpdateIVS == "Y") {
		HTML.title("References", HR=2, align="left")
		HTML(Ref_list$IVS_ref, align="left")
	}
        HTML(Ref_list$BateClark_ref, align = "left")
        HTML("Hyndman, R.J. and Fan, Y. (1996). Sample quantiles in statistical packages. American Statistician 50, 361-365.", align = "left")
        HTML("Hollander, M. and Wolfe, D.A. (1973). Nonparametric Statistical Methods. New York: John Wiley & Sons.", align = "left")
    }

    if (block == "NULL" && (statstest == "CompareToControl" && leng >= 3)) {
       HTML.title("Analysis conclusions", HR = 2, align = "left")
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
            add <- c("None of the Steel's all to one comparisons are significant at the ")
            add <- paste(add, 100 * (1 - sig), sep = "")
            add <- paste(add, "% level of significance as the p-values are all greater than ", sep = "")
            add <- paste(add, 1 - sig, ".", sep = "")
            HTML(add, align = "left")
        } else if (textindex > 1) {
            HTML(add, align = "left")
        }

        HTML.title("Analysis description", HR = 2, align = "left")
        HTML("The comparison of treatment factor levels back to a single control group was made using the non-parametric Steel's test, see Steel (1959).", align = "left")
        HTML("Non-parametric tests should be used if the data is non-normally distributed, the variability of the responses varies across treatments or the responses are not continuous and numerical. ", align = "left")

        #Bate and Clark comment
        HTML(refxx,  align = "left")

	if (UpdateIVS == "N") {
		HTML.title("Statistical references", HR=2, align="left")
	}
	if (UpdateIVS == "Y") {
		HTML.title("References", HR=2, align="left")
		HTML(Ref_list$IVS_ref, align="left")
	}
        HTML(Ref_list$BateClark_ref, align = "left")
        HTML("Hyndman, R.J. and Fan, Y. (1996). Sample quantiles in statistical packages. American Statistician 50, 361-365.", align = "left")
        HTML("Steel, R.G.D. (1959). A multiple comparison rank sum test: treatments versus control. Biometrics, 15, 560-572. ", align = "left")
    }

    if (block != "NULL" && (statstest == "CompareToControl" && leng >= 3)) {
	if (UpdateIVS == "N") {
		HTML.title("Statistical references", HR=2, align="left")
	}
	if (UpdateIVS == "Y") {
		HTML.title("References", HR=2, align="left")
		HTML(Ref_list$IVS_ref, align="left")
	}
        HTML("Hyndman, R.J. and Fan, Y. (1996). Sample quantiles in statistical packages. American Statistician 50, 361-365.", align = "left")
    }


}

if (RunFried != "Y") {
    HTML.title("Statistical reference", HR = 2, align = "left")
    HTML("Hyndman, R.J. and Fan, Y. (1996). Sample quantiles in statistical packages. American Statistician 50, 361-365.", align = "left")
}

#===================================================================================================================
#References 
#===================================================================================================================
if (UpdateIVS == "N") {
	HTML.title("R references", HR=2, align="left")
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
	HTML(Ref_list$COIN_ref, align = "left")
}
if (UpdateIVS == "Y") {
	HTML.title("R references", HR=4, align="left")
	HTML(Ref_list$R_ref, align = "left")
	HTML(paste(capture.output(print(citation("R2HTML"),bibtex=F))[4], capture.output(print(citation("R2HTML"),bibtex=F))[5], sep = ""),  align="left")

	HTML(paste(capture.output(print(citation("GGally"),bibtex=F))[4], capture.output(print(citation("GGally"),bibtex=F))[5], capture.output(print(citation("GGally"),bibtex=F))[6], capture.output(print(citation("GGally"),bibtex=F))[7], sep = ""),  align="left")
	HTML(paste(capture.output(print(citation("RColorBrewer"),bibtex=F))[4], capture.output(print(citation("RColorBrewer"),bibtex=F))[5], sep = ""),  align="left")
	HTML(paste(capture.output(print(citation("ggplot2"),bibtex=F))[4], capture.output(print(citation("ggplot2"),bibtex=F))[5], sep=""),  align="left")
	HTML(paste(capture.output(print(citation("ggrepel"),bibtex=F))[4], capture.output(print(citation("ggrepel"),bibtex=F))[5], capture.output(print(citation("ggrepel"),bibtex=F))[6], sep = ""),  align="left")
	HTML(paste(capture.output(print(citation("reshape"),bibtex=F))[4], capture.output(print(citation("reshape"),bibtex=F))[5], sep = ""),  align="left")
	HTML(paste(capture.output(print(citation("plyr"),bibtex=F))[4], capture.output(print(citation("plyr"),bibtex=F))[5], capture.output(print(citation("plyr"),bibtex=F))[6], sep = ""),  align="left")
	HTML(paste(capture.output(print(citation("scales"),bibtex=F))[4], capture.output(print(citation("scales"),bibtex=F))[5], capture.output(print(citation("scales"),bibtex=F))[6], sep = ""),  align="left")
	HTML(paste(capture.output(print(citation("proto"),bibtex=F))[4], capture.output(print(citation("proto"),bibtex=F))[5], capture.output(print(citation("proto"),bibtex=F))[6], sep = ""),  align="left")
	#extrafont_ref  <- capture.output(print(citation("extrafont"),bibtex=F))[4]

	HTML(paste(capture.output(print(citation("coin"),bibtex=F))[5], capture.output(print(citation("coin"),bibtex=F))[6], capture.output(print(citation("coin"),bibtex=F))[7], capture.output(print(citation("coin"),bibtex=F))[8], sep = ""),  align="left")
	HTML(paste(capture.output(print(citation("mvtnorm"),bibtex=F))[4], capture.output(print(citation("mvtnorm"),bibtex=F))[5], capture.output(print(citation("mvtnorm"),bibtex=F))[6], capture.output(print(citation("mvtnorm"),bibtex=F))[7], sep = ""),  align="left")
}



#===================================================================================================================
#Show dataset
#===================================================================================================================
if (showdataset == "Y") {
    statdata2 <- data.frame(eval(parse(text = paste("statdata$", response))))
    statdata3 <- data.frame(eval(parse(text = paste("statdata$", treatment))))

    if (block != "NULL") {
        statdata4 <- data.frame(eval(parse(text = paste("statdata$", block))))
        statdata5 <- cbind(statdata2, statdata3, statdata4)
        names <- c(response, treatment, block)
    } else {
        statdata5 <- cbind(statdata2, statdata3)
        names <- c(response, treatment)
    }
    colnames(statdata5) <- names
    observ <- data.frame(c(1:dim(statdata5)[1]))
    colnames(observ) <- c("Observation")
    statdata6 <- cbind(observ, statdata5)

    HTML.title("Analysis dataset", HR = 2, align = "left")
    HTML(statdata6, classfirstline = "second", align = "left", row.names = "FALSE")
}

#===================================================================================================================
#Show arguments
#===================================================================================================================
if (OutputAnalysisOps == "Y") {
	HTML.title("Analysis options", HR = 2, align = "left")
	HTML(paste("Response variable: ", response, sep = ""), align = "left")
	HTML(paste("Treatment factor: ", treatment, sep = ""), align = "left")
	
	if (block != "NULL") {
	    HTML(paste("Other design (block) factor: ", block, sep = ""), align = "left")
	}

	if (statstest == "MannWhitney" && dim == 2) {
	    HTML(paste("Analysis type: Mann-Whitney"), align = "left")
	}

	if (statstest == "MannWhitney" && dim >= 3) {
	    HTML(paste("Analysis type: Kruskal-Wallis"), align = "left")
	}

	if (statstest != "MannWhitney") {
	    HTML(paste("Analysis type: ", statstest, sep = ""), align = "left")
	}

	HTML(paste("Significance level: ", 1 - sig, sep = ""), align = "left")

	if (contlevel != "NULL" && statstest == "CompareToControl") {
	    HTML(paste("Control group: ", contlevel, sep = ""), align = "left")
	}
}