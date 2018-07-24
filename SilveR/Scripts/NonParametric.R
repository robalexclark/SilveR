#===================================================================================================================
#R Libraries

suppressWarnings(library(mvtnorm))
suppressWarnings(library(R2HTML))
#===================================================================================================================

#===================================================================================================================
# retrieve args
Args <- commandArgs(TRUE)

#Read in data
statdata <- read.csv(Args[3], header = TRUE, sep = ",")

#Copy Args
response <- Args[4]
treatment <- Args[5]
sig <- 1 - as.numeric(Args[6])
statstest <- Args[7]
contlevel <- Args[8]

#V3.2 STB OCT2015
set.seed(5041975)

#===================================================================================================
#Create variable to use as a formula
respVTreat <- paste(response, "~", treatment, sep = "")
leng <- length(levels(as.factor(eval(parse(text = paste("statdata$", treatment))))))
blank <- c(" ")
index <- 1

#Setup the html file and associated css file
htmlFile <- sub(".csv", ".html", Args[3]);
#determine the file name of the html file
HTMLSetFile(file = htmlFile)
.HTML.file = htmlFile

#Output HTML header
HTML.title("<bf>Non-Parametric Analysis", HR = 1, align = "left")

#V3.2 STB OCT2015
set.seed(5041975)
#===================================================================================================
#Graphics parameter setup

graphdata <- statdata
displaypointBOX <- "N"
#===================================================================================================
#===================================================================================================

#Removing illegal charaters

YAxisTitle <- response
XAxisTitle <- treatment


#replace illegal characters in variable names
for (i in 1:10) {
    YAxisTitle <- namereplace(YAxisTitle)
    XAxisTitle <- namereplace(XAxisTitle)
}


#STB CC28 Oct 2011
statdata <- statdata[order(eval(parse(text = paste("statdata$", treatment)))),]

#-------------------------------------------------------------------------------------------------------
#-------------------------------------------------------------------------------------------------------
#Steels function from package
#-------------------------------------------------------------------------------------------------------
#-------------------------------------------------------------------------------------------------------

"npmc" <-
function(dataset, control = NULL, df = 2, alpha = 0.05) {
    mvtnorm <- require(mvtnorm, quietly = TRUE);
    if (!mvtnorm) {
        msg <- paste("npmc requires the mvtnorm-package to calculate",
                 "p-values for the test-statistics. This package is not",
                 "available on your system, so these values and the",
                 "confidence-limits will be missing values in the output!\n",
                 sep = "\n");
        warning(msg);
    }

    if (any(df == 0:2))
        dfm <- c(3, 2, 1)[df + 1]
    else {
        warning("Wrong value for df\nUsing Satterthwaite t-approximation\n");
        dfm <- 1;
    }

    if (alpha <= 0 || alpha >= 1)
        stop("alpha must be a value between 0 and 1");


    name <- attr(dataset, "name");
    desc <- attr(dataset, "description");


    ##=== Function definitions ===================================================


    ##
    ## ssq:
    ## ----
    ## Calculates a vector's sum of squares
    ##
    ssq <- function(x) sum(x * x);

    ##
    ## force.ps: 
    ## ---------
    ## Forces a matrix to be positive semidefinite by replacing 
    ## all negative eigenvalues by zero.
    ## 
    force.ps <- function(M.in) {
        eig <- eigen(M.in, symmetric = TRUE);
        spec <- eig$values;
        if (adjusted <- any(spec < 0)) {
            spec[spec < 0] <- 0;
            M <- eig$vectors %*% diag(spec) %*% t(eig$vectors);
            ginv <- diag(1 / sqrt(diag(M)));
            M.out <- ginv %*% M %*% ginv;
            ##if ((msg <- all.equal(M.in,M.out))!=TRUE) attr(M.out, "msg") <- msg;
        }
        else {
            M.out <- M.in;
        }
        attr(M.out, "adjusted") <- adjusted;
        return(M.out);
    }


    ##
    ## z.dist:
    ## -------
    ## Calculates the p-values for the teststatistics using the mvtnorm-package.
    ## The 'sides' parameter determines whether the p-values for the one-
    ## or two-sided test are calculated.
    ## The statistic is supposed to follow a multivariate t-statistic with
    ## correlation-matrix 'corr' and 'df' degrees of freedom. If df=0, the
    ## multivariate normal-distribution is used.
    ## We use the mvtnorm-package by Frank Bretz (www.bioinf.uni-hannover.de) 
    ## to calculate the corresponding p-values. These algorithms originally 
    ## calculate the value P(X<=stat) for the mentioned multivariate distributions, 
    ## i.e. the 1-sided p-value. In order to gain the 2-sided p-value as well, 
    ## we used the algorithms on the absolute value of the teststatistic in 
    ## combination with the inflated correlation-matrix
    ##   kronecker(matrix(c(1,-1,-1,1),ncol=2), corr)
    ##
    z.dist <- function(stat, corr, df = 0, sides = 2) {
        if (!mvtnorm) return(NA);

        if (sides == 2) {
            corr <- kronecker(matrix(c(1, -1, -1, 1), ncol = 2), corr);
            stat <- abs(stat);
        }
        n <- ncol(corr);
        sapply(stat, function(arg) {
            mvtnorm:::mvt(
               lower = rep(-Inf, n),
               upper = rep(arg, n),
               df = df,
               corr = corr,
               delta = rep(0, n)
               )$value;
        });
    }


    ##
    ## z.quantile:
    ## -----------
    ## Calculates the corresponding quantiles of z.dist p-values
    ## (used for the confidence-intervals)
    ##
    z.quantile <- function(p = 0.95, start = 0, corr, df = 0, sides = 2) {
        if (!mvtnorm) return(NA);

        if (z.dist(start, corr = corr, df = df, sides = sides) < p) {
            lower <- start;
            upper <- lower + 1;
            while (z.dist(upper, corr = corr, df = df, sides = sides) < p)
                upper <- upper + 1;
        }
        else {
            upper <- start;
            lower <- upper - 1;
            while (z.dist(lower, corr = corr, df = df, sides = sides) > p)
                lower <- lower - 1;
        }
        ur <- uniroot(f = function(arg) p - z.dist(arg, corr = corr, df = df, sides = sides),
                  upper = upper, lower = lower
                  );
        ur$root;
    }


    ##=== Calculations ===========================================================

    ## sort the dataset by factor
    dataset$class <- factor(dataset$class);
    datord <- order(dataset$class);
    attrs <- attributes(dataset);
    dataset <- data.frame(lapply(dataset, "[", datord));
    attributes(dataset) <- attrs;

    ## general data characteristics
    attach(dataset);
    fl <- levels(class);
    # factor-levels
    a <- nlevels(class);
    # number of factor-levels
    samples <- split(var, class);
    # split the data in separate sample-vectors
    n <- sapply(samples, length);
    # sample sizes
    detach(dataset);

    if (is.null(control)) {
        ## create indexing vectors for the all-pairs situation
        tmp <- expand.grid(1:a, 1:a);
        ind <- tmp[[1]] > tmp[[2]];
        vi <- tmp[[2]][ind];
        vj <- tmp[[1]][ind];
    }
    else {
        ## create indexing vectors for the many-to-one situation
        if (!any(fl == control)) {
            msg <- paste("Wrong control-group specification\n",
                   "The data does not contain a group with factor-level ",
                   control,
                   sep = "");
            stop(msg, FALSE);
        }
        cg <- which(fl == control);
        vi <- which((1:a) != cg);
        vj <- rep(cg, a - 1);
    }

    ## number of comparisons ( a*(a-1)/2 for all-pairs, (a-1) for many-to-one )
    nc <- length(vi);

    ## labels describing the compared groups 
    cmpid <- paste(vi, "-", vj, sep = "");

    ## pairwise pooled sample-sizes
    gn <- n[vi] + n[vj];

    ## internal rankings
    intRanks <- lapply(samples, rank);

    ## pairwise rankings
    pairRanks <- lapply(1:nc, function(arg) {
        rank(c(samples[[vi[arg]]], samples[[vj[arg]]]));
    });

    ## estimators for the relative effects
    pd <- sapply(1:nc, function(arg) {
        i <- vi[arg];
        j <- vj[arg];
        (sum(pairRanks[[arg]][(n[i] + 1):gn[arg]]) / n[j] - (n[j] + 1) / 2) / n[i];
    });

    ## Calculations for the BF-test ###################################
    ##
    dij <- dji <- list(0);

    sqij <- sapply(1:nc, function(arg) {
        i <- vi[arg];
        j <- vj[arg];
        pr <- pairRanks[[arg]][(n[i] + 1):gn[arg]];
        dij[[arg]] <<- pr - sum(pr) / n[j] - intRanks[[j]] + (n[j] + 1) / 2;
        ssq(dij[[arg]]) / (n[i] * n[i] * (n[j] - 1));
    });

    sqji <- sapply(1:nc, function(arg) {
        i <- vi[arg];
        j <- vj[arg];
        pr <- pairRanks[[arg]][1:n[i]];
        dji[[arg]] <<- pr - sum(pr) / n[i] - intRanks[[i]] + (n[i] + 1) / 2;
        ssq(dji[[arg]]) / (n[j] * n[j] * (n[i] - 1));
    });

    ## diagonal elements of the covariance-matrix
    vd.bf <- gn * (sqij / n[vj] + sqji / n[vi]);

    ## mark and correct zero variances for further calculations
    singular.bf <- (vd.bf == 0);
    vd.bf[singular.bf] <- 0.00000001;

    ## standard-deviation
    std.bf <- sqrt(vd.bf / gn);

    ## teststatistic
    t.bf <- (pd - 0.5) * sqrt(gn / vd.bf);

    ## Satterthwaite approxiamtion for the degrees of freedom
    df.sw <- (n[vi] * sqij + n[vj] * sqji) ^ 2 /
    ((n[vi] * sqij) ^ 2 / (n[vj] - 1) + (n[vj] * sqji) ^ 2 / (n[vi] - 1));
    df.sw[is.nan(df.sw)] <- Inf;

    ## choose degrees of freedom 
    df <- if (dfm < 3) max(1, if (dfm == 2) min(gn - 2) else min(df.sw)) else 0;


    ## Calculations for the Steel-test ################################
    ##
    ## the Steel-type correlation factors
    lambda <- sqrt(n[vi] / (gn + 1));

    ## diagonal elements of the covariance-matrix
    vd.st <- sapply(1:nc, function(arg) ssq(pairRanks[[arg]] - (gn[arg] + 1) / 2)) /
    (n[vi] * n[vj] * (gn - 1));

    ## mark and correct zero variances for further calculations
    singular.st <- (vd.st == 0);
    vd.st[singular.st] <- 0.00000001;

    ## standard-deviation
    std.st <- sqrt(vd.st / gn);

    ## teststatistic
    t.st <- (pd - 0.5) * sqrt(gn / vd.st);


    ## Calculate the correlation-matrices (for both, BF and Steel) ####
    ##    
    rho.bf <- rho.st <- diag(nc);
    for (x in 1:(nc - 1)) {
        for (y in (x + 1):nc) {
            i <- vi[x];
            j <- vj[x];
            v <- vi[y];
            w <- vj[y];
            p <- c(i == v, j == w, i == w, j == v);
            if (sum(p) == 1) {
                cl <- list(
                   function()(t(dji[[x]]) %*% dji[[y]]) / (n[j] * n[w] * n[i] * (n[i] - 1)),
                   function()(t(dij[[x]]) %*% dij[[y]]) / (n[i] * n[v] * n[j] * (n[j] - 1)),
                   function() - (t(dji[[x]]) %*% dij[[y]]) / (n[i] * n[w] * n[i] * (n[i] - 1)),
                   function() - (t(dij[[x]]) %*% dji[[y]]) / (n[j] * n[v] * n[j] * (n[j] - 1))
                   );
                case <- (1:4)[p];
                rho.bf[x, y] <- rho.bf[y, x] <- sqrt(gn[x] * gn[y]) / sqrt(vd.bf[x] * vd.bf[y]) * cl[[case]]();

                rho.st[x, y] <- rho.st[y, x] <- {
                    if (case > 2)
                    - 1
                    else
                        1
                } * lambda[x] * lambda[y];
            }
        }
    }
    rho.bf <- force.ps(rho.bf);
    rho.st <- force.ps(rho.st);


    ## Calculate the p-values     (BF and Steel) ######################
    ##
    p1s.bf <- 1 - z.dist(t.bf, corr = rho.bf, df = df, sides = 1);
    p2s.bf <- 1 - z.dist(t.bf, corr = rho.bf, df = df, sides = 2);

    p1s.st <- 1 - z.dist(t.st, corr = rho.st, sides = 1);
    p2s.st <- 1 - z.dist(t.st, corr = rho.st, sides = 2);


    ## Calculate the confidence-limits (BF and Steel) #################
    ##
    z.bf <- z.quantile(1 - alpha, corr = rho.bf, df = df, sides = 2);
    lcl.bf <- pd - std.bf * z.bf;
    ucl.bf <- pd + std.bf * z.bf;

    z.st <- z.quantile(1 - alpha, corr = rho.st, sides = 2);
    lcl.st <- pd - std.st * z.st;
    ucl.st <- pd + std.st * z.st;


    ##=== Output ==================================================================

    ## Create the result-datastructures ###############################
    ##    
    dataStructure <- data.frame("group index" = 1:a,
                              "class level" = fl,
                              "nobs" = n
                              );

    test.bf <- data.frame("cmp" = cmpid,
                        "gn" = gn,
                        "effect" = pd,
                        "lower.cl" = lcl.bf,
                        "upper.cl" = ucl.bf,
                        "variance" = vd.bf,
                        "std" = std.bf,
                        "statistic" = t.bf,
                        "p-value 1s" = p1s.bf,
                        "p-value 2s" = p2s.bf,
                        "zero" = singular.bf
                        );

    test.st <- data.frame("cmp" = cmpid,
                        "gn" = gn,
                        "effect" = pd,
                        "lower.cl" = lcl.st,
                        "upper.cl" = ucl.st,
                        "variance" = vd.st,
                        "std" = std.st,
                        "statistic" = t.st,
                        "p-value 1s" = p1s.st,
                        "p-value 2s" = p2s.st,
                        "zero" = singular.st
                        );

    result <- list("data" = dataset,
                 "info" = dataStructure,
                 "corr" = list("BF" = rho.bf, "Steel" = rho.st),
                 "test" = list("BF" = test.bf, "Steel" = test.st),
                 "control" = control,
                 "df.method" = dfm,
                 "df" = df,
                 "alpha" = alpha
                 );

    class(result) <- "npmc";

    return(result);

}

"print.npmc" <-
function(x, ...) {
    print(x$test, ...)
}


"report" <-
function(msg = NULL, style = 0, char = "-") {
    if (is.null(msg)) msg <- "";

    if (is.vector(msg))
        msg <- unlist(msg)
    else
        stop("msg must be of type vector");

    char <- substr(char, 1, 1);

    underlined <- function(arg) {
        c(arg, paste(rep(char, max(nchar(msg))), collapse = ""));
    }

    border <- function(arg) {
        n <- length(msg);
        ml <- max(nchar(msg));
        space <- paste(rep(" ", ml), collapse = "");
        line <- paste(rep(char, ml + 4), collapse = "");
        msg <- paste(msg, substr(rep(space, n), rep(1, n), ml - nchar(msg)), sep = "");
        c(line, paste(char, msg, char), line);
    }

    sfun <- list(underlined = underlined,
               border = border
               );

    if (is.numeric(style) && length(style) == 1 && any(style == 1:length(sfun)))
        msg <- sfun[[style]](msg)
    else if (is.character(style) && length(style) == 1 && !is.null(sfun[[style]]))
        msg <- sfun[[style]](msg)

    m <- matrix(msg, ncol = 1);
    colnames(m) <- "";
    rownames(m) <- rep("", length(msg));
    print.noquote(m);
}



"summary.npmc" <-
function(object, type = "both", info = TRUE, short = TRUE, corr = FALSE, ...) {
    x <- object;
    if (info) {
        name <- attr(data, "name");
        desc <- attr(data, "desc");
        df <- x$df;
        df.method <- x$df.method;
        alpha <- x$alpha;

        apm <- c(paste("Satterthwaite t-approximation (df=", df, ")", sep = ""),
             paste("simple t-approximation (df=", df, ")", sep = ""),
             "standard normal approximation"
             );
        msg <- c(paste("npmc executed", if (!is.null(name)) paste("on", name)),
             if (is.null(desc)) "" else c("", "Description:", desc, ""),
             "NOTE:",
             paste("-Used", apm[df.method]),
             paste("-Calculated simultaneous (1-", alpha, ") confidence intervals", sep = ""),
             "-The one-sided tests 'a-b' reject if group 'a' tends to",
             " smaller values than group 'b'"
             );
        report(msg, style = 2, char = "/");
        report();
    }

    if (short) {
        bf <- st <- c("cmp", "effect", "lower.cl", "upper.cl", "p.value.1s", "p.value.2s");
    }
    else {
        bf <- names(x$test$BF);
        st <- names(x$test$Steel);
    }


    content <- list();
    if (info)
        content <- c(content, list("Data-structure" = x$info));
    if (corr && type != "Steel")
        content <- c(content, list("Behrens-Fisher type correlation-matrix" = x$corr$BF));
    if (corr && type != "BF")
        content <- c(content, list("Steel type correlation-matrix" = x$corr$Steel));
    if (type != "Steel")
        content <- c(content, list("Results of the multiple Behrens-Fisher-Test" = x$test$BF[bf]));
    if (type != "BF")
        content <- c(content, list("Results of the multiple Steel-Test" = x$test$Steel[st]));

    ##h <- (list("Data-structure"=x$info, 
    ##           "Behrens-Fisher type correlation-matrix"=x$corr$BF, 
    ##           "Steel type correlation-matrix"=x$corr$Steel,
    ##           "Results of the multiple Behrens-Fisher-Test"=x$test$BF[bf], 
    ##           "Results of the multiple Steel-Test"=x$test$Steel[st]
    ##           ));

    print(content);
}








#-------------------------------------------------------------------------------------------------------
#-------------------------------------------------------------------------------------------------------
#Titles and description
#-------------------------------------------------------------------------------------------------------
#-------------------------------------------------------------------------------------------------------

#Response
title <- c("Response")
HTML.title(title, HR = 2, align = "left")
add <- paste(c("The  "), response, sep = "")
add <- paste(add, " response is currently being analysed by the Non-Parametric Analysis module", sep = "")

if (treatment != "NULL") {
    add <- paste(add, c(", with  "), sep = "")
    add <- paste(add, treatment, sep = "")
    add <- paste(add, " fitted as the treatment factor.", sep = "")
} else {
    add <- paste(add, ".", sep = "")
}

HTML.title("<bf> ", HR = 2, align = "left")
HTML.title(add, HR = 0, align = "left")


#-------------------------------------------------------------------------------------------------------
#-------------------------------------------------------------------------------------------------------
#Summary stats table
#-------------------------------------------------------------------------------------------------------
#-------------------------------------------------------------------------------------------------------

#setting up parameters
DP <- 3
sumdata <- statdata
sumdata$catfact <- as.factor(eval(parse(text = paste("sumdata$", treatment))))
lev <- length(unique(eval(parse(text = paste("statdata$", treatment)))))
sumtable <- matrix(data = NA, nrow = (lev + 1), ncol = 5)
rownames(sumtable) <- c("Group", unique(levels(sumdata$catfact)))

#Add entries to the table
for (i in 1:lev) {
    tempdata <- subset(sumdata, sumdata$catfact == unique(levels(as.factor(sumdata$catfact)))[i])

    sumtable[(i + 1), 1] <- format(round(min(eval(parse(text = paste("tempdata$", response))), na.rm = TRUE), DP), nsmall = DP, scientific = FALSE)
    sumtable[(i + 1), 2] <- format(round(boxplot.stats(split(eval(parse(text = paste("tempdata$", response))), tempdata$catfact)[[i]])$stats[2], DP), nsmall = DP, scientific = FALSE)
    sumtable[(i + 1), 3] <- format(round(boxplot.stats(split(eval(parse(text = paste("tempdata$", response))), tempdata$catfact)[[i]])$stats[3], DP), nsmall = DP, scientific = FALSE)
    sumtable[(i + 1), 4] <- format(round(boxplot.stats(split(eval(parse(text = paste("tempdata$", response))), tempdata$catfact)[[i]])$stats[4], DP), nsmall = DP, scientific = FALSE)
    sumtable[(i + 1), 5] <- format(round(max(eval(parse(text = paste("tempdata$", response))), na.rm = TRUE), DP), nsmall = DP, scientific = FALSE)
}
sumtable[1, 1] = c(" ")

table <- data.frame(sumtable)
colnames(table) <- c("Min", "Lower quartile", "Median", "Upper quartile", "Max")
HTMLbr()
HTML.title("<bf>Summary data", HR = 2, align = "left")
HTML(table, classfirstline = "second", align = "left")

#-------------------------------------------------------------------------------------------------------
#-------------------------------------------------------------------------------------------------------
#Boxplot of data
#-------------------------------------------------------------------------------------------------------
#-------------------------------------------------------------------------------------------------------
HTMLbr()
HTML.title("<bf>Box-plot", HR = 2, align = "left")


#=======================================================================================================
#Graphics parameter setup
#=======================================================================================================
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

#STB July2013
plotFilepdfx <- sub(".html", "ncboxplot.pdf", htmlFile)
dev.control("enable")

#GGPLOT2 code
NONCAT_BOX()

#OUtput code
void <- HTMLInsertGraph(GraphFileName = sub("[A-Z0-9a-z,:,\\\\]*App_Data[\\\\]", "", ncboxplot), Align = "centre")

#STB July2013
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
#=======================================================================================================
#=======================================================================================================

HTML.title("<bf> ", HR = 2, align = "left")
HTML.title("On the box-plot the median is denoted by the horizontal line within the box. The", HR = 0, align = "left")
HTML.title("box indicates the interquartile range,", HR = 0, align = "left")
HTML.title("where the lower and upper quartiles are calculated using the type 2 method, see Hyndman and Fan (1996).", HR = 0, align = "left")
HTML.title("The whiskers extend to the most extreme data point which is no more than 1.5 times the length of the box away from the box.", HR = 0, align = "left")
HTML.title("Individual observations that lie outside the outlier range are included on the plot using circles.", HR = 0, align = "left")




#-------------------------------------------------------------------------------------------------------
#-------------------------------------------------------------------------------------------------------
#Kruskall-Wallis and Mann Whitney
#-------------------------------------------------------------------------------------------------------
#-------------------------------------------------------------------------------------------------------

if (statstest == "MannWhitney" || (statstest == "AllComparisons" && leng == 2) || (statstest == "CompareToControl" && leng == 2)) {
    #Kruskal Wallis and Mann Whitney
    dim <- length(unique(eval(parse(text = paste("statdata$", treatment)))))

    if (dim > 2) {
        HTMLbr()
        HTML.title("<bf>Kruskal-Wallis test", HR = 2, align = "left")
        options <- (scipen = 20)
        kruskalOut <- kruskal.test(as.formula(respVTreat), data = statdata)

        pvalue <- format(round(kruskalOut$p.value, 4), nsmall = 4, scientific = FALSE)

        for (i in 1:(length(pvalue))) {
            if (kruskalOut$p.value[i] < 0.0001) {
                #STB March 2011 - formatting pvalue p<0.001
                #				pvalue[i]<-0.0001
                pvalue[i] = format(round(0.0001, 4), nsmall = 4, scientific = FALSE)
                pvalue[i] <- paste("<", pvalue[i])
            }
        }

        Wstat <- format(round(kruskalOut$statistic, 2), nsmall = 2)
        df <- format(round(kruskalOut$parameter, 0), nsmall = 0)
        colname <- c("Test statistic", " ", "Degrees of freedom", " ", "p-value")
        rowname <- c("Result")
        #STB May 2012 Adding blanks to the KW table
        blank <- c(NA)

        temptab <- cbind(Wstat, blank, df, blank, pvalue)
        temptable <- data.frame(temptab)
        colnames(temptable) <- colname
        row.names(temptable) <- rowname
        HTML(temptable, classfirstline = "second", align = "left")
    } else if (dim == 2) {
        HTMLbr()
        HTML.title("<bf> Mann-Whitney test", HR = 2, align = "left")
        mess <- c("You have selected a factor with only two levels, hence a Mann-Whitney test, also know as a Wilcoxon rank sum test, has been used to analyse the data rather than a Kruskal-Wallis test.")
        HTML.title("</bf> ", HR = 2, align = "left")
        HTML.title(mess, HR = 0, align = "left")


        wilcoxOut <- wilcox.test(as.formula(respVTreat), data = statdata, exact = TRUE, conf.int = TRUE, conf.level = sig)
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
        temptab <- cbind(Wstat, pvalue)
        temptable <- data.frame(temptab)
        colname <- c("Test statistic", "p-value")
        colnames(temptable) <- colname
        rowname <- c("Test result")
        row.names(temptable) <- rowname
        HTML(temptable, classfirstline = "second", align = "left")

        if (length(unique(eval(parse(text = paste("statdata$", response))))) == length((eval(parse(text = paste("statdata$", treatment)))))) {
            HTML.title("</bf> ", HR = 2, align = "left")
            HTML.title("<bf>As there are no ties in the responses, the exact test result has been calculated.", HR = 0, align = "left")
        }
        if (length(unique(eval(parse(text = paste("statdata$", response))))) < length((eval(parse(text = paste("statdata$", treatment)))))) {
            HTML.title("</bf> ", HR = 2, align = "left")
            HTML.title("<bf>As there are ties in the responses, the asymptotic test result has been calculated.", HR = 0, align = "left")
        }
    }

    #-------------------------------------------------------------------------------------------------------
    #-------------------------------------------------------------------------------------------------------
    #All comparisons tests
    #-------------------------------------------------------------------------------------------------------
    #-------------------------------------------------------------------------------------------------------

} else if (statstest == "AllComparisons" && leng >= 3) {
    #All comparisons test
    npmcallcomp <- function(response, treatment) {
        dataset <- data.frame(var = response, class = treatment)
        summary(npmc(dataset, df = 1, alpha = sig), type = "BF")
    }

    allpair <- npmcallcomp(eval(parse(text = paste("statdata$", response))), eval(parse(text = paste("statdata$", treatment))))
    Treatment <- allpair$'Data-structure'[2]
    Nobs <- allpair$'Data-structure'[3]
    temp3 <- cbind(Treatment, Nobs)
    header <- c(NA, " ")
    allpairtable <- rbind(header, temp3)
    allpairtable1 <- data.frame(allpairtable)

    temp4 <- c("Treatment", "Nobs")
    colnames(allpairtable1) <- temp4

    good2 <- c("Treatment ID")
    for (i in 1:length(unique(eval(parse(text = paste("statdata$", treatment)))))) { good2[i + 1] <- i }
    row.names(allpairtable1) <- good2

    # Behrens Fisher results
    HTMLbr()
    HTML.title("<bf>All pairwise comparisons - Behrens Fisher tests", HR = 2, align = "left")

    compno <- c(0, 1, 3, 6, 10, 15, 21, 28, 36, 45, 55, 66, 78, 91, 105, 120, 136, 153, 171, 190, 210, 231, 253, 276, 300, 325, 351, 378, 406, 435)
    int <- compno[length(unique(eval(parse(text = paste("statdata$", treatment)))))] + 1

    Comparison <- allpair$'Results of the multiple Behrens-Fisher-Test'[1]
    Pvalue <- format(round(allpair$'Results of the multiple Behrens-Fisher-Test'[6], 4), nsmall = 4, scientific = FALSE)

    Pvaluezzz <- format(round(allpair$'Results of the multiple Behrens-Fisher-Test'[6], 6), nsmall = 6, scientific = FALSE)
    y <- 1
    allgroup1 <- unique(eval(parse(text = paste("statdata$", treatment))))
    for (i in 1:(length(unique(eval(parse(text = paste("statdata$", treatment))))) - 1)) {
        for (j in 1:(length(unique(eval(parse(text = paste("statdata$", treatment))))) - i)) {
            allgroup1[y] <- unique(eval(parse(text = paste("statdata$", treatment))))[i]
            y <- y + 1
        }
    }

    allgroup2 <- unique(eval(parse(text = paste("statdata$", treatment))))
    z <- 1
    for (i in 1:(length(unique(eval(parse(text = paste("statdata$", treatment))))) - 1)) {
        for (j in 1:(length(unique(eval(parse(text = paste("statdata$", treatment))))) - i)) {
            allgroup2[z] <- unique(eval(parse(text = paste("statdata$", treatment))))[j + i]
            z <- z + 1
        }
    }
    allvs <- c("r")
    for (i in 1:(int - 1)) { allvs[i] <- "vs." }

    temp5 <- cbind(allgroup1, allvs, allgroup2, Pvalue)
    temp5zzz <- cbind(allgroup1, allvs, allgroup2, Pvaluezzz)

    for (i in 1:(dim(temp5)[1])) {
        if (as.numeric(temp5zzz[i, 4]) < 0.0001) {
            #STB March 2011 - formatting p-value p<0.0001
            #			temp5[i,4]<-0.0001
            temp5[i, 4] = format(round(0.0001, 4), nsmall = 4, scientific = FALSE)
            temp5[i, 4] <- paste("<", temp5[i, 4])
        }
    }

    header <- c(NA, NA, NA, NA)

    allpairtab <- rbind(header, temp5)
    allpairtable <- data.frame(allpairtab)

    temp6 <- c("Gp 1", "vs.", "Gp 2", "p-value")
    colnames(allpairtable) <- temp6

    good3 <- c("Comparison")
    for (i in 2:int) { good3[i] <- i - 1 }
    row.names(allpairtable) <- good3

    HTML(allpairtable, classfirstline = "second", align = "left")

    compno <- c(0, 1, 3, 6, 10, 15, 21, 28, 36, 45, 55, 66, 78, 91, 105, 120, 136, 153, 171, 190, 210, 231, 253, 276, 300, 325, 351, 378, 406, 435)
    int <- compno[length(unique(eval(parse(text = paste("statdata$", treatment)))))] + 1
    allvs <- c("r")
    for (i in 1:(int - 1)) { allvs[i] <- "vs." }

    y <- 1
    allgroup1 <- unique(eval(parse(text = paste("statdata$", treatment))))

    for (i in 1:(length(unique(eval(parse(text = paste("statdata$", treatment))))) - 1)) {
        for (j in 1:(length(unique(eval(parse(text = paste("statdata$", treatment))))) - i)) {
            allgroup1[y] <- unique(eval(parse(text = paste("statdata$", treatment))))[i]
            y <- y + 1
        }
    }

    allgroup2 <- unique(eval(parse(text = paste("statdata$", treatment))))
    z <- 1
    for (i in 1:(length(unique(eval(parse(text = paste("statdata$", treatment))))) - 1)) {
        for (j in 1:(length(unique(eval(parse(text = paste("statdata$", treatment))))) - i)) {
            allgroup2[z] <- unique(eval(parse(text = paste("statdata$", treatment))))[j + i]
            z <- z + 1
        }
    }

    tabletemp <- data.frame(allgroup1, allvs, allgroup2)
    blankz <- c(NA, NA, NA)
    tabletemp <- rbind(blankz, tabletemp)

    for (s in 1:(length(unique(eval(parse(text = paste("statdata$", treatment))))) - 1)) {
        for (t in s + 1:(length(unique(eval(parse(text = paste("statdata$", treatment))))) - s)) {
            # seperate out groups
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
            for (m in 1:length1) { fact1[m] <- unique(eval(parse(text = paste("statdata$", treatment))))[[s]] }
            for (n in 1:length2) { fact2[n] <- unique(eval(parse(text = paste("statdata$", treatment))))[[t]] }
            grou <- c(fact1, fact2)

            # combine the datasets
            comb1 <- cbind(grou, resp)
            finaldata <- data.frame(comb1)

            #Wilcoxon test
            wilcox <- wilcox.test(resp ~ grou, exact = FALSE)
            pv <- format(round(wilcox$p.value, 4), nsmall = 4, scientific = FALSE)
            pvzzz <- wilcox$p.value

            for (i in 1:(length(pv))) {
                if (pvzzz[i] < 0.0001) {
                    #STB March 2011 - formatting p-values p<0.0001
                    #					pv[i]<-0.0001
                    pv[i] = format(round(0.0001, 4), nsmall = 4, scientific = FALSE)
                    pv[i] <- paste("<", pv[i])
                }
            }

            #merge results
            lines <- c(pv)
            blank <- rbind(blank, lines)

            index <- index + 1
        }
    }

    blank2 <- blank
    blank <- cbind(tabletemp, blank)

    #create final table
    temp16 <- c("Gp 1", "vs.", "Gp 2", "p-value")
    colnames(blank) <- temp16

    rows <- c("Comparison")

    for (l in 1:(index - 1)) { rows[l + 1] <- l }
    row.names(blank) <- rows

    #	wilcoxcomps<-data.frame(blank)
    HTML.title("<bf>All pairwise comparisons - Asymptotic Mann-Whitney tests", HR = 2, align = "left")
    HTML(blank, classfirstline = "second", align = "left")
    #	HTML(wilcoxcomps, classfirstline="second", align="left")
    HTML.title("<bf> ", HR = 2, align = "left")
    HTML.title("<bf>Why are there two different tests presented? The Behrens Fisher tests are the recommended approach, although we still need to independently verify these results.", HR = 0, align = "left")


    #-------------------------------------------------------------------------------------------------------
    #-------------------------------------------------------------------------------------------------------
    #Comparisons back to control
    #-------------------------------------------------------------------------------------------------------
    #-------------------------------------------------------------------------------------------------------
} else if (statstest == "CompareToControl" && leng >= 3) {
    #Comparisons back to a control
    npmccontrol <- function(response, treatment, cont) {
        dataset <- data.frame(var = response, class = treatment)
        summary(npmc(dataset, df = 1, alpha = sig, control = contlevel), type = "Steels")
    };
    steel <- npmccontrol(eval(parse(text = paste("statdata$", response))), eval(parse(text = paste("statdata$", treatment))), paste(contlevel))

    Treatment <- steel$'Data-structure'[2]
    Nobs <- steel$'Data-structure'[3]
    temp3 <- cbind(Treatment, Nobs)
    header <- c(NA, " ")
    steeltable <- rbind(header, temp3)
    steeltable1 <- data.frame(steeltable)
    temp4 <- c("Treatment", "Nobs")
    colnames(steeltable1) <- temp4
    good2 <- c("Treatment ID")
    for (i in 1:length(unique(eval(parse(text = paste("statdata$", treatment)))))) { good2[i + 1] <- i }
    row.names(steeltable1) <- good2

    #Steels Test results table
    HTMLbr()
    HTML.title("<bf>Steel's all comparisons back to one", HR = 2, align = "left")

    #Comparison<-steel$'Results of the multiple Steel-Test'[1]

    group2 <- c("a")
    for (i in 1:(length(unique(eval(parse(text = paste("statdata$", treatment))))) - 1)) { group2[i] <- paste(contlevel) }
    vs <- c("r")
    for (i in 1:(length(unique(eval(parse(text = paste("statdata$", treatment))))) - 1)) { vs[i] <- "vs." }

    group1 <- unique(eval(parse(text = paste("statdata$", treatment))))
    group1 <- group1[group1 != contlevel]

    Pvalue <- format(round(steel$'Results of the multiple Steel-Test'[6], 4), nsmall = 4, scientific = FALSE)
    Pvaluezzz <- format(round(steel$'Results of the multiple Steel-Test'[6], 6), nsmall = 6, scientific = FALSE)

    temp5 <- cbind(group1, vs, group2, Pvalue)
    temp5zzz <- cbind(group1, vs, group2, Pvaluezzz)

    for (i in 1:(dim(temp5)[1])) {
        if (as.numeric(temp5zzz[i, 4]) < 0.0001) {
            #STB March 2011 formatting p-values <0.0001
            #			temp5[i,4]<-0.0001
            temp5[i, 4] = format(round(0.0001, 4), nsmall = 4, scientific = FALSE)
            temp5[i, 4] <- paste("<", temp5[i, 4])
        }
    }

    header <- c(NA, NA, NA, NA)
    temp7 <- rbind(header, temp5)
    steeltable2 <- data.frame(temp7)
    temp6 <- c("Group", "vs.", "Group", "p-value")
    colnames(steeltable2) <- temp6
    good3 <- c("Comparison")
    for (i in 2:length(unique(eval(parse(text = paste("statdata$", treatment)))))) { good3[i] <- i - 1 }
    row.names(steeltable2) <- good3
    HTML(steeltable2, classfirstline = "second", align = "left")
}


#-------------------------------------------------------------------------------------------------------
#-------------------------------------------------------------------------------------------------------
#Text for conclusions and comments 
#-------------------------------------------------------------------------------------------------------
#-------------------------------------------------------------------------------------------------------
Ref_list <- R_refs()

textindex = 1

HTMLbr()
HTML.title("<bf>Analysis conclusions", HR = 2, align = "left")

if (statstest == "MannWhitney" || (statstest == "AllComparisons" && leng == 2) || (statstest == "CompareToControl" && leng == 2)) {
    dim <- length(unique(eval(parse(text = paste("statdata$", treatment)))))

    if (dim > 2) {
        if (temptab[1, 5] <= (1 - sig)) {
            add <- paste(c("There is a statistically significant overall difference between the treatment groups at the "), sep = "")
            add <- paste(add, 100 * (1 - sig), sep = "")
            add <- paste(add, "% level of significance as the p-value is less than ", sep = "")
            add <- paste(add, 1 - sig, sep = "")
            add <- paste(add, " (Kruskal-Wallis test).", sep = "")
            HTML.title(add, HR = 0, align = "left")
        } else if (temptab[1, 5] > 0.05) {
            add <- paste(c("The overall difference between the treatment groups is not statistically significant at the "), sep = "")
            add <- paste(add, 100 * (1 - sig), sep = "")
            add <- paste(add, "% level of significance as the p-value is greater than ", sep = "")
            add <- paste(add, 1 - sig, sep = "")
            add <- paste(add, " (Kruskal-Wallis test).", sep = "")
            HTML.title(add, HR = 0, align = "left")
        }
        HTMLbr()
        HTML.title("<bf>Analysis description", HR = 2, align = "left")
        HTML.title("The overall treatment effect was assessed using the non-parametric Kruskal-Wallis test, see Kruskal and Wallis (1952, 1953).", HR = 0, align = "left")
        HTML.title("<bf> ", HR = 2, align = "left")
        HTML.title("Non-parametric tests should be used if the data is non-normally distributed, the variability is different between treatment groups or the responses are not continuous and numerical. ", HR = 0, align = "left")

        #Bate and Clark comment
        HTML.title("<bf> ", HR = 2, align = "left")
        HTML.title(refxx, HR = 0, align = "left")
        HTML.title("<bf> ", HR = 2, align = "left")

        HTMLbr()
        HTML.title("<bf>Statistical references", HR = 2, align = "left")
        HTML.title("<bf> ", HR = 2, align = "left")
        HTML.title(Ref_list$BateClark_ref, HR = 0, align = "left")
        HTML.title("<bf> ", HR = 2, align = "left")
        HTML.title("<bf> Hyndman RJ and Fan Y. (1996). Sample quantiles in statistical packages. American Statistician 50, 361-365.", HR = 0, align = "left")
        HTML.title("<bf> ", HR = 2, align = "left")
        HTML.title("Kruskal, WH and Wallis, WA (1952). Use of ranks in one criterion variance analysis. JASA, 47, 583-621.", HR = 0, align = "left")
        HTML.title("<bf> ", HR = 2, align = "left")
        HTML.title("Kruskal, WH and Wallis, WA (1953). Errata for Kruskal-Wallis (1952). JASA, 48, 907-911.", HR = 0, align = "left")
    }
    else if (dim == 2) {
        if (temptab[1, 2] <= (1 - sig)) {
            add <- paste(c("The difference between the two treatment groups is statistically significant at the "), sep = "")
            add <- paste(add, 100 * (1 - sig), sep = "")
            add <- paste(add, "% level of significance as the p-value is less than ", sep = "")
            add <- paste(add, 1 - sig, sep = "")
            #add<-paste(add, ", (p=", sep="")
            #add<-paste(add, temptab[1,2], sep="")
            add <- paste(add, " (Mann-Whitney test).", sep = "")
            HTML.title(add, HR = 0, align = "left")
        } else if (temptab[1, 2] > 0.05) {
            add <- paste(c("The difference between the two treatment groups is not statistically significant at the "), sep = "")
            add <- paste(add, 100 * (1 - sig), sep = "")
            add <- paste(add, "% level of significance as the p-value is greater than ", sep = "")
            add <- paste(add, 1 - sig, sep = "")
            add <- paste(add, " (Mann-Whitney test).", sep = "")
            HTML.title(add, HR = 0, align = "left")
        }
        HTMLbr()
        HTML.title("<bf>Analysis description", HR = 2, align = "left")
        HTML.title("The difference between the two treatments was assessed using the non-parametric Mann-Whitney test, see Wilcoxon (1945), Mann and Whitney (1947).", HR = 0, align = "left")
        HTML.title("<bf> ", HR = 2, align = "left")
        HTML.title("Non-parametric tests should be used if the data is non-normally distributed, the variability is different between treatment groups or the responses are not continuous and numerical. ", HR = 0, align = "left")

        #Bate and Clark comment
        HTML.title("<bf> ", HR = 2, align = "left")
        HTML.title(refxx, HR = 0, align = "left")
        HTML.title("<bf> ", HR = 2, align = "left")

        HTMLbr()
        HTML.title("<bf>Statistical references", HR = 2, align = "left")
        HTML.title("<bf> ", HR = 2, align = "left")
        HTML.title(Ref_list$BateClark_ref, HR = 0, align = "left")
        HTML.title("<bf> ", HR = 2, align = "left")
        HTML.title("<bf> Hyndman RJ and Fan Y. (1996). Sample quantiles in statistical packages. American Statistician 50, 361-365.", HR = 0, align = "left")
        HTML.title("<bf> ", HR = 2, align = "left")
        HTML.title("Mann, HB and Whitney, DR (1947). On a test of whether one of two random variables is stochastically larger than the other. Annals of Mathematical Statistics, 18, 50-60.", HR = 0, align = "left")
        HTML.title("<bf> ", HR = 2, align = "left")
        HTML.title("Wilcoxon, F (1945). Individual comparisons by ranking methods. Biometrics Bulletin, 1, 80-83.", HR = 0, align = "left")
    }
} else if (statstest == "AllComparisons" && leng >= 3) {
    add <- c(" ")
    for (y in 2:int) {
        if (allpairtab[y, 4] <= (1 - sig)) {
            add <- paste(add, "The difference between groups ", sep = "")
            add <- paste(add, allpairtab[y, 1], sep = "")
            add <- paste(add, " and ", sep = "")
            add <- paste(add, allpairtab[y, 3], sep = "")
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
    for (q in 2:index) {
        if (blank2[q, 1] <= (1 - sig)) {
            add2 <- paste(add2, "The difference between groups ", sep = "")
            add2 <- paste(add2, blank[q, 1], sep = "")
            add2 <- paste(add2, " and ", sep = "")
            add2 <- paste(add2, blank[q, 3], sep = "")
            add2 <- paste(add2, " is statistically significant at the ", sep = "")
            add2 <- paste(add2, 100 * (1 - sig), sep = "")
            add2 <- paste(add2, "% level of significance as the p-value is less than ", sep = "")
            add2 <- paste(add2, 1 - sig, sep = "")
            add2 <- paste(add2, " (Mann-Whitney test). ", sep = "")
            textindex2 <- textindex2 + 1
        }
    }
    if (textindex == 1) {
        HTML.title("<bf>None of the all pairwise Behrens Fisher tests were significant.", HR = 0, align = "left")
    } else if (textindex > 1) {
        HTML.title(add, HR = 0, align = "left")
    }
    if (textindex2 == 1) {
        HTML.title("<bf> ", HR = 2, align = "left")
        HTML.title("<bf>None of the all pairwise Mann-Whitney tests were significant.", HR = 0, align = "left")
    } else if (textindex2 > 1) {
        HTML.title("<bf> ", HR = 2, align = "left")
        HTML.title(add2, HR = 0, align = "left")
    }
    HTMLbr()
    HTML.title("<bf>Analysis description", HR = 2, align = "left")
    HTML.title("All pairwise differences between the treatments were assessed using Behrens Fisher tests, see Munzel and Hothorn (2001) and Mann-Whitney tests, see Mann and Whitney (1947).", HR = 0, align = "left")
    HTML.title("<bf> ", HR = 2, align = "left")
    HTML.title("Non-parametric tests should be used if the data is non-normally distributed, the variability is different between treatment groups or the responses are not continuous and numerical. ", HR = 0, align = "left")

    #Bate and Clark comment
    HTML.title("<bf> ", HR = 2, align = "left")
    HTML.title(refxx, HR = 0, align = "left")
    HTML.title("<bf> ", HR = 2, align = "left")

    HTMLbr()
    HTML.title("<bf>Statistical references", HR = 2, align = "left")
    HTML.title("<bf> ", HR = 2, align = "left")
    HTML.title(Ref_list$BateClark_ref, HR = 0, align = "left")
    HTML.title("<bf> ", HR = 2, align = "left")
    HTML.title("<bf> Hyndman RJ and Fan Y. (1996). Sample quantiles in statistical packages. American Statistician 50, 361-365.", HR = 0, align = "left")
    HTML.title("<bf> ", HR = 2, align = "left")
    HTML.title("Mann, HB and Whitney, DR (1947). On a test of whether one of two random variables is stochastically larger than the other. Annals of Mathematical Statistics, 18, 50-60.", HR = 0, align = "left")
    HTML.title("<bf> ", HR = 2, align = "left")
    HTML.title("Munzel, U and Hothorn, LA (2001). A unified approach to simultaneous rank test procedures in the unbalanced one-way layout. Biometrical Journal, 43(5) 553-569.", HR = 0, align = "left")
    HTML.title("<bf> ", HR = 2, align = "left")
    HTML.title("Wilcoxon, F (1945). Individual comparisons by ranking methods. Biometrics Bulletin, 1, 80-83.", HR = 0, align = "left")
} else if (statstest == "CompareToControl" && leng >= 3) {
    add <- c(" ")
    for (y in 2:length(unique(eval(parse(text = paste("statdata$", treatment)))))) {
        if (temp7[y, 4] <= (1 - sig)) {
            add <- paste(add, "The difference between groups ", sep = "")
            add <- paste(add, temp7[y, 1], sep = "")
            add <- paste(add, " and ", sep = "")
            add <- paste(add, temp7[y, 3], sep = "")
            add <- paste(add, " is statistically significant at the ", sep = "")
            add <- paste(add, 100 * (1 - sig), sep = "")
            add <- paste(add, "% level of significance as the p-value is less than ", sep = "")
            add <- paste(add, 1 - sig, sep = "")
            add <- paste(add, " (Steel's test). ", sep = "")
            textindex <- textindex + 1
        }
    }
    if (textindex == 1) {
        HTML.title("<bf>None of the Steel's all to one comparisons were significant.", HR = 0, align = "left")
    } else if (textindex > 1) {
        HTML.title(add, HR = 0, align = "left")
    }
    HTMLbr()
    HTML.title("<bf>Analysis description", HR = 2, align = "left")
    HTML.title("The comparison of treatment groups back to a single control group was made using the non-parametric Steel's test, see Steel (1959).", HR = 0, align = "left")
    HTML.title("<bf> ", HR = 2, align = "left")
    HTML.title("Non-parametric tests should be used if the data is non-normally distributed, the variability is different between treatment groups or the responses are not continuous and numerical. ", HR = 0, align = "left")

    #Bate and Clark comment
    HTML.title("<bf> ", HR = 2, align = "left")
    HTML.title(refxx, HR = 0, align = "left")
    HTML.title("<bf> ", HR = 2, align = "left")

    HTMLbr()
    HTML.title("<bf>Statistical reference", HR = 2, align = "left")
    HTML.title("<bf> ", HR = 2, align = "left")
    HTML.title(Ref_list$BateClark_ref, HR = 0, align = "left")
    HTML.title("<bf> ", HR = 2, align = "left")
    HTML.title("<bf> Hyndman RJ and Fan Y. (1996). Sample quantiles in statistical packages. American Statistician 50, 361-365.", HR = 0, align = "left")
    HTML.title("<bf> ", HR = 2, align = "left")
    HTML.title("Steel, RGD (1959). A multiple comparison rank sum test: treatments versus control. Biometrics, 15, 560-572. ", HR = 0, align = "left")
}


#-------------------------------------------------------------------------------------------------------
#-------------------------------------------------------------------------------------------------------
#References 
#-------------------------------------------------------------------------------------------------------
#-------------------------------------------------------------------------------------------------------


HTMLbr()
HTML.title("<bf>R references", HR = 2, align = "left")

HTML.title("<bf> ", HR = 2, align = "left")
HTML.title(Ref_list$R_ref, HR = 0, align = "left")

HTML.title("<bf> ", HR = 2, align = "left")
HTML.title(Ref_list$mtvnorm_ref, HR = 0, align = "left")

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
HTML.title(Ref_list$NPMC_ref, HR = 0, align = "left")

HTML.title("<bf> ", HR = 2, align = "left")
HTML.title(Ref_list$R2HTML_ref, HR = 0, align = "left")

HTML.title("<bf> ", HR = 2, align = "left")
HTML.title(Ref_list$PROTO_ref, HR = 0, align = "left")


#----------------------------------------------------------------------------------------------------------
#Show dataset
#----------------------------------------------------------------------------------------------------------
if (showdataset == "Y") {

    statdata2 <- data.frame(eval(parse(text = paste("statdata$", response))))
    names <- c(response)
    colnames(statdata2) <- names

    statdata3 <- data.frame(eval(parse(text = paste("statdata$", treatment))))
    statdata2 <- cbind(statdata2, statdata3)
    names <- c(response, treatment)
    colnames(statdata2) <- names

    HTMLbr()
    HTML.title("<bf>Analysis dataset", HR = 2, align = "left")
    HTML(statdata2, classfirstline = "second", align = "left")
}



