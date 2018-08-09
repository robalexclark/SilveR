#===================================================================================================================
#Import Graphical parameters from file
#===================================================================================================================
suppressWarnings(library(ggplot2))
suppressWarnings(library(RColorBrewer))
suppressWarnings(library(plyr))
suppressWarnings(library(scales))
suppressWarnings(library(reshape))
suppressWarnings(library(GGally))
suppressWarnings(library(proto))
suppressWarnings(library(grid))
#suppressWarnings(library(extrafont))
#-------------------------------------------------------------------------------------------------------------------
#Numerical parameters
#-------------------------------------------------------------------------------------------------------------------
#grpara = data.frame(read.table(paste(getwd(),"/OutputOptionsNumeric.txt", sep="")))

#Font size for the main title
Title_size <- 20

#Y and X axis titles font size
X_Title_size <- 15
Y_Title_size <- 15

#Y and X axis titles font size
Y_Label_size <- 15
X_Label_size <- 15

#X-axis labels angle and horizontal adjustment
Gr_x_angle <- 0
Gr_x_hjust <- 0.5

#Y-axis labels angle and vertical adjustment
Gr_y_angle <- 0
Gr_y_vjust <- 0.5

#Size of points on plots
Point_size <- 4

#Shape of points on plot
Point_shape <- 21

#Size of the lines on the plots
Line_size <- 1

#Legend text size
legend_font_size <- 15

#graphics sizes
pdfwidth <- 11
pdfheight <- 8
jpegwidth <- 480
jpegheight <- 480

# When using black and white range - these are the limits (0 = white and 1 = black)
gr_bw_low <- 0.1
gr_bw_high <- 0.8

#Horizontal jitter on scatterplot
Gr_w_Gr_jit <- 0.1

#Vertical jitter on scatterplot
Gr_h_Gr_jit <- 0.1

#Error bar width for LSMeans plots and line SEM plots
ErrorBarWidth <- 0.7

#Width of bars for means with SEM column plots
ErrorBarWidth2 <- ErrorBarWidth / 2

#-------------------------------------------------------------------------------------------------------------------
#Text parameters
#-------------------------------------------------------------------------------------------------------------------
#grpara = data.frame(read.table(paste(getwd(),"/OutputOptionsText.txt", sep="")))

#Line types used - solid, blank and dashed
Line_type_solid <- "solid"
Line_type_dashed <- "dashed"

#Font for plots
gr_font <- "Helvetica"

#Font style for plots
gr_fontface <- "plain"

#Font colour
gr_textcolour <- "Black"

#Colour fill colour
Col_fill <- "royalblue1"
BW_fill <- "grey"

#Colour for the header bar on the seperate categorised plots
Catbar_fill <- "ivory2"

#Individual lines colour
Col_line <- "red"
BW_line <- "black"

#Legend parameters
Legend_text_col <- "white"
Legend_pos <- "Default"

#Categorised plot set default
Palette_set <- "Set1"

#Output dataset
showdataset <- "Y"

#pdf output
pdfout <- "N"

#plot colour
bandw <- "N"

#Display back-transformed geometric means
GeomDisplay <- "N"

#Display covariate regression coefficients
CovariateRegressionCoefficients <- "N"


#Display covariate interaction tests 
AssessCovariateInteractions <- "N"

#==================================================================================================================
#Defining the GGPLOT options

theme_set(theme_bw())
theme_map <- theme_get()

Line_type_blank <- "blank"
Legend_pos_abs <- "none"

if (bandw != "N") {
    Gr_fill <- BW_fill
    Gr_line <- BW_line
} else {
    Gr_fill <- Col_fill
    Gr_line <- Col_line
}

if (Legend_pos == "Default") {
    Gr_legend_pos <- "right"
    Gr_legend_pos2 <- "bottom"
} else {
    Gr_legend_pos <- Legend_pos
    Gr_legend_pos2 <- Legend_pos
}


#==================================================================================================================
#Font options

if (gr_font == "Courier") { gr_fontfamily <- "mono" }
if (gr_font == "Helvetica") { gr_fontfamily <- "sans" }
if (gr_font == "Times") { gr_fontfamily <- "serif" }
if (gr_font == "AvantGarde_(pdf_only)") { gr_fontfamily <- "AvantGarde" }
if (gr_font == "URWPalladio_(pdf_only)") { gr_fontfamily <- "URWPalladio" }
if (gr_font == "CenturySch_(pdf_only)") { gr_fontfamily <- "CenturySch" }
if (gr_font == "NimbusSanCond_(pdf_only)") { gr_fontfamily <- "NimbusSanCond" }
if (gr_font == "NimbusSan_(pdf_only)") { gr_fontfamily <- "NimbusSan" }
if (gr_font == "NimbusMon_(pdf_only)") { gr_fontfamily <- "NimbusMon" }
if (gr_font == "URWBookman_(pdf_only)") { gr_fontfamily <- "URWBookman" }
if (gr_font == "URWGothic_(pdf_only)") { gr_fontfamily <- "URWGothic" }
if (gr_font == "Palatino_(pdf_only)") { gr_fontfamily <- "Palatino" }
if (gr_font == "NewCenturySchoolbook_(pdf_only)") { gr_fontfamily <- "NewCenturySchoolbook" }
if (gr_font == "Helvetica-Narrow_(pdf_only)") { gr_fontfamily <- "Helvetica-Narrow" }
if (gr_font == "Bookman_(pdf_only)") { gr_fontfamily <- "Bookman" }
if (gr_font == "NimbusRom_(pdf_only)") { gr_fontfamily <- "NimbusRom" }


#==================================================================================================================
#Setting up the theme

#plot.title = element_text(size=Title_size, family = gr_fontfamily, face = gr_fontface,colour = gr_textcolour), text=element_text(size=Label_size, family = gr_fontfamily, face = gr_fontface,colour = gr_textcolour), axis.title.x = element_text(size = X_Title_size, family = gr_fontfamily, face = gr_fontface,colour = gr_textcolour), axis.title.y = element_text(size = Y_Title_size, family = gr_fontfamily, face = gr_fontface,colour = gr_textcolour), axis.text.x=element_text(hjust=Gr_x_hjust, angle=Gr_x_angle, family = gr_fontfamily, face = gr_fontface,colour = gr_textcolour), axis.text.y=element_text(vjust=Gr_y_vjust, angle=Gr_y_angle, family = gr_fontfamily, face = gr_fontface,colour = gr_textcolour)



mytheme <- theme(
    plot.title = element_text(size = Title_size,
                     family = gr_fontfamily,
                     face = gr_fontface,
                     colour = gr_textcolour),

    text = element_text(size = X_Label_size,
                     family = gr_fontfamily,
                     face = gr_fontface,
                     colour = gr_textcolour),

    axis.title.x = element_text(size = X_Title_size,
                     family = gr_fontfamily,
                     face = gr_fontface,
                     colour = gr_textcolour),

    axis.title.y = element_text(size = Y_Title_size,
                     family = gr_fontfamily,
                     face = gr_fontface,
                     colour = gr_textcolour),

    axis.text.x = element_text(size = X_Label_size,
                     hjust = Gr_x_hjust,
                     angle = Gr_x_angle,
                     family = gr_fontfamily,
                     face = gr_fontface,
                     colour = gr_textcolour),

    axis.text.y = element_text(size = Y_Label_size,
                     vjust = Gr_y_vjust,
                     angle = Gr_y_angle,
                     family = gr_fontfamily,
                     face = gr_fontface,
                     colour = gr_textcolour),

    strip.background = element_rect(fill = Catbar_fill),

    legend.title = element_text(colour = Legend_text_col,
                     family = gr_fontfamily,
                     face = gr_fontface),

    legend.text = element_text(size = legend_font_size,
                     family = gr_fontfamily,
                     face = gr_fontface)
    )


#===================================================================================================
#===================================================================================================
#FUNCTIONS TO REPLACE ILLEGAL CHARACTERS
#===================================================================================================
#===================================================================================================

namereplace2 <- function(axistitle) {

    for (i in 1:20) {
        axistitle <- gsub("ivs_sp_ivs*ivs_sp_ivs", " * ", axistitle, fixed = TRUE)
    }
    return(axistitle)
}

namereplace <- function(axistitle) {

    for (i in 1:20) {
        axistitle <- sub("ivs_tilde_ivs", "~", axistitle)
        axistitle <- sub("ivs_star_ivs", "*", axistitle)
        axistitle <- sub("ivs_plus_ivs", "+", axistitle)
        axistitle <- sub("ivs_sp_ivs", " ", axistitle)
        axistitle <- sub("ivs_ob_ivs", "(", axistitle)
        axistitle <- sub("ivs_cb_ivs", ")", axistitle)
        axistitle <- sub("ivs_div_ivs", "/", axistitle)
        axistitle <- sub("ivs_pc_ivs", "%", axistitle)
        axistitle <- sub("ivs_hash_ivs", "#", axistitle)
        axistitle <- sub("ivs_pt_ivs", ".", axistitle)
        axistitle <- sub("ivs_hyphen_ivs", "-", axistitle)
        axistitle <- sub("ivs_at_ivs", "@", axistitle)
        axistitle <- sub("ivs_colon_ivs", ":", axistitle)
        axistitle <- sub("ivs_exclam_ivs", "!", axistitle)
        axistitle <- sub("ivs_quote_ivs", "`", axistitle)
        axistitle <- sub("ivs_pound_ivs", "£", axistitle)
        axistitle <- sub("ivs_dollar_ivs", "$", axistitle)
        axistitle <- sub("ivs_hat_ivs", "^", axistitle)
        axistitle <- sub("ivs_amper_ivs", "&", axistitle)
        axistitle <- sub("ivs_obrace_ivs", "{", axistitle)
        axistitle <- sub("ivs_cbrace_ivs", "}", axistitle)
        axistitle <- sub("ivs_semi_ivs", ";", axistitle)
        axistitle <- sub("ivs_pipe_ivs", "|", axistitle)
        axistitle <- sub("ivs_slash_ivs", "\\", axistitle)
        axistitle <- sub("ivs_osb_ivs", "[", axistitle)
        axistitle <- sub("ivs_csb_ivs", "]", axistitle)
        axistitle <- sub("ivs_eq_ivs", "=", axistitle)
        axistitle <- sub("ivs_lt_ivs", "<", axistitle)
        axistitle <- sub("ivs_gt_ivs", ">", axistitle)
        axistitle <- sub("ivs_dblquote_ivs", "\"", axistitle)
    }
    return(axistitle)
}


namereplaceGSUB <- function(axistitle) {

    for (i in 1:20) {
        axistitle <- gsub("ivs_tilde_ivs", "~", axistitle)
        axistitle <- gsub("ivs_star_ivs", "*", axistitle)
        axistitle <- gsub("ivs_plus_ivs", "+", axistitle)
        axistitle <- gsub("ivs_sp_ivs", " ", axistitle)
        axistitle <- gsub("ivs_ob_ivs", "(", axistitle)
        axistitle <- gsub("X_IVS_X", " ", axistitle)
        axistitle <- gsub("ivs_cb_ivs", ")", axistitle)
        axistitle <- gsub("ivs_div_ivs", "/", axistitle)
        axistitle <- gsub("ivs_pc_ivs", "%", axistitle)
        axistitle <- gsub("ivs_hash_ivs", "#", axistitle)
        axistitle <- gsub("ivs_pt_ivs", ".", axistitle)
        axistitle <- gsub("ivs_hyphen_ivs", "-", axistitle)
        axistitle <- gsub("ivs_at_ivs", "@", axistitle)
        axistitle <- gsub("ivs_colon_ivs", ":", axistitle)
        axistitle <- gsub("ivs_exclam_ivs", "!", axistitle)
        axistitle <- gsub("ivs_quote_ivs", "`", axistitle)
        axistitle <- gsub("ivs_pound_ivs", "£", axistitle)
        axistitle <- gsub("ivs_dollar_ivs", "$", axistitle)
        axistitle <- gsub("ivs_hat_ivs", "^", axistitle)
        axistitle <- gsub("ivs_amper_ivs", "&", axistitle)
        axistitle <- gsub("ivs_obrace_ivs", "{", axistitle)
        axistitle <- gsub("ivs_cbrace_ivs", "}", axistitle)
        axistitle <- gsub("ivs_semi_ivs", ";", axistitle)
        axistitle <- gsub("ivs_pipe_ivs", "|", axistitle)
        axistitle <- gsub("ivs_slash_ivs", "\\", axistitle)
        axistitle <- gsub("ivs_osb_ivs", "[", axistitle)
        axistitle <- gsub("ivs_csb_ivs", "]", axistitle)
        axistitle <- gsub("ivs_eq_ivs", "=", axistitle)
        axistitle <- gsub("ivs_lt_ivs", "<", axistitle)
        axistitle <- gsub("ivs_gt_ivs", ">", axistitle)
        axistitle <- gsub("ivs_dblquote_ivs", "\"", axistitle)
    }
    return(axistitle)
}

#===================================================================================================
#===================================================================================================
#Steels function from package
#===================================================================================================
#===================================================================================================

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



#===================================================================================================
#===================================================================================================
#Graphical functions - shared code across modules
#===================================================================================================
#===================================================================================================


#Function for all modules other than Correlation module
IVS_F_infinite_slope <- function() {
    #Testing for with infinite slopes on scatterplot and re-ordering dataset if necessary
    infiniteslope <- "N"
    nlevels <- length(unique(levels(as.factor(graphdata$catfact))))
    graphdata$catvartest <- "Y" # new variable to aid subsetting for infinite slopes
    testdata <- graphdata[1,] # create a new dataset with the same header names as graphdata
    normalfaclevs <- c() # variables to re-order dataset
    singlefaclevs <- c() #variables to re-order dataset
    i <- 1
    j <- 1

    #Code to re-order dataset if best-fit line is infinite
    if (ScatterPlot == "Y" && is.numeric(graphdata$xvarrr_IVS) == "TRUE" && LinearFit == "Y" && (FirstCatFactor != "NULL" || SecondCatFactor != "NULL")) {
        for (k in 1:nlevels) {
            tempdata <- subset(graphdata, graphdata$l_l == unique(levels(as.factor(graphdata$l_l)))[k])

            tempdata_red <- tempdata[!is.na(tempdata$yvarrr_IVS),]
            tempdata_red <- tempdata_red[!is.na(tempdata_red$xvarrr_IVS),]

            if (length(unique(tempdata_red$xvarrr_IVS)) == 1) {
                infiniteslope <- "Y"
                tempdata$catvartest <- "N"
                singlefaclevs[i] <- unique(levels(as.factor(graphdata$l_l)))[k]
                i = i + 1
            } else {
                normalfaclevs[j] <- unique(levels(as.factor(graphdata$l_l)))[k]
                j = j + 1
            }
            testdata <- rbind(testdata, tempdata) #restack dataset with catvartest updated
            catfactlevz <- c(normalfaclevs, singlefaclevs)
        }

        graphdata <- testdata[-1,] #need to remove first row that was used to setup testdata

        #reorder levels for categorised plot
        if (infiniteslope == "Y" && GraphStyle == "Overlaid") {
            graphdata$l_l <- factor(graphdata$l_l, levels = catfactlevz, ordered = TRUE)
        }
    }
    output <- list(graphdata = graphdata, infiniteslope = infiniteslope)
    return(output)
}



#Function for Correlation module
IVS_F_infinite_slope_Cor <- function() {
    #Testing for with infinite slopes on scatterplot and re-ordering dataset if necessary
    infiniteslope <- "N"
    nlevels <- length(unique(levels(as.factor(graphdata$l_l))))
    graphdata$catvartest <- "Y" # new variable to aid subsetting for infinite slopes
    testdata <- graphdata[1,] # create a new dataset with the same header names as graphdata
    normalfaclevs <- c() # variables to re-order dataset
    singlefaclevs <- c() #variables to re-order dataset

    xnormalfaclevs <- c() # variables to re-order dataset (for Matrix plot)
    xsinglefaclevs <- c() #variables to re-order dataset (for Matrix plot)

    i <- 1
    j <- 1

    #Code to re-order dataset if best-fit line is infinite
    if (ScatterPlot == "Y" && is.numeric(graphdata$xvarrr_IVS) == "TRUE" && LinearFit == "Y" && (FirstCatFactor != "NULL" || SecondCatFactor != "NULL" || ThirdCatFactor != "NULL" || FourthCatFactor != "NULL")) {
        for (k in 1:nlevels) {
            tempdata <- subset(graphdata, graphdata$l_l == unique(levels(as.factor(graphdata$l_l)))[k])

            tempdata_red <- tempdata[!is.na(tempdata$yvarrr_IVS),]
            tempdata_red <- tempdata_red[!is.na(tempdata_red$xvarrr_IVS),]

            if (length(unique(tempdata_red$xvarrr_IVS)) == 1) {
                infiniteslope <- "Y"
                tempdata$catvartest <- "N"
                singlefaclevs[i] <- unique(levels(as.factor(graphdata$l_l)))[k]
                xsinglefaclevs[i] <- tempdata$l_li[1]
                i = i + 1
            } else {
                normalfaclevs[j] <- unique(levels(as.factor(graphdata$l_l)))[k]
                xnormalfaclevs[j] <- tempdata$l_li[1]
                j = j + 1
            }
            testdata <- rbind(testdata, tempdata) #restack dataset with catvartest updated
            catfactlevz <- c(normalfaclevs, singlefaclevs)
            xcatfactlevz <- c(xnormalfaclevs, xsinglefaclevs)
        }

        graphdata <- testdata[-1,] #need to remove first row that was used to setup testdata




        #For Matrixplot we need to remove the repeats in xcatfactlevz
        xcatfactlevz_rev <- rev(xcatfactlevz)
        xcatfactlevz_rev_unique <- unique(xcatfactlevz_rev)
        xcatfactlevz <- rev(xcatfactlevz_rev_unique)

        #reorder levels for categorised plot
        if (infiniteslope == "Y" && GraphStyle == "Overlaid") {
            graphdata$l_l <- factor(graphdata$l_l, levels = catfactlevz, ordered = TRUE)
        }
    }
    output <- list(graphdata = graphdata, infiniteslope = infiniteslope, catfactlevz = catfactlevz, xcatfactlevz = xcatfactlevz)
    return(output)
}







#Creating outliers dataset for the boxplot
IVS_F_boxplot_outlier <- function() {

    #setting up parameters
    sumdata <- graphdata
    levz <- length(unique(graphdata$xvarrr_IVS_BP))
    lev <- levels(graphdata$xvarrr_IVS_BP)

    sumtable <- matrix(data = NA, nrow = levz, ncol = 5)
    rownames(sumtable) <- c(unique(levels(sumdata$xvarrr_IVS_BP)))
    colnames(sumtable) <- c("minq", "lowerq", "medq", "upperq", "maxq")

    #Add entries to the table
    for (i in 1:levz) {
        tempdata <- subset(sumdata, sumdata$xvarrr_IVS_BP == unique(levels(as.factor(sumdata$xvarrr_IVS_BP)))[i])

        sumtable[i, 1] <- boxplot.stats(split(tempdata$yvarrr_IVS, tempdata$xvarrr_IVS_BP)[[i]])$stats[1]
        sumtable[i, 2] <- boxplot.stats(split(tempdata$yvarrr_IVS, tempdata$xvarrr_IVS_BP)[[i]])$stats[2]
        sumtable[i, 3] <- boxplot.stats(split(tempdata$yvarrr_IVS, tempdata$xvarrr_IVS_BP)[[i]])$stats[3]
        sumtable[i, 4] <- boxplot.stats(split(tempdata$yvarrr_IVS, tempdata$xvarrr_IVS_BP)[[i]])$stats[4]
        sumtable[i, 5] <- boxplot.stats(split(tempdata$yvarrr_IVS, tempdata$xvarrr_IVS_BP)[[i]])$stats[5]
    }
    boxdata <- data.frame(sumtable)
    boxdata$lev <- unique(levels(as.factor(sumdata$xvarrr_IVS_BP)))
    boxdata$xvarrr_IVS_BP <- boxdata$lev

    #Generating a copy of the dataset containg any outliers
    levs <- length(lev)
    outlierdata <- graphdata[1,]
    for (k in 1:levs) {
        tempdata <- subset(graphdata, graphdata$xvarrr_IVS_BP == unique(levels(as.factor(graphdata$xvarrr_IVS_BP)))[k])
        lev1 <- boxdata[k, 1]
        lev2 <- boxdata[k, 5]
        newdata <- subset(tempdata, yvarrr_IVS < lev1 | yvarrr_IVS > lev2,)
        tempdata <- tempdata[1,]
        tempdata$yvarrr_IVS = NA

        if (dim(newdata)[1] >= 1) {
            newdata <- rbind(newdata, tempdata)
        } else {
            newdata <- tempdata
        }

        if (dim(newdata)[1] >= 1) {
            outlierdata <- rbind(outlierdata, newdata)
        }
    }
    outlierdata <- outlierdata[-1,]

    #GGPLOT2 variable options
    if (Outliers == "N") {
        stats <- boxplot(yvarrr_IVS ~ xvarrr_IVS_BP, data = graphdata, plot = FALSE)$stat
        ymin <- min(stats[1,], na.rm = TRUE)
        ymax <- max(stats[5,], na.rm = TRUE)
        range <- (ymax - ymin) * 0.1
    } else {
        ymax = max(graphdata$yvarrr_IVS, na.rm = TRUE)
        ymin = min(graphdata$yvarrr_IVS, na.rm = TRUE)
        range <- (ymax - ymin) * 0.1
    }

    output <- list(outlierdata = outlierdata, boxdata = boxdata, ymax = ymax, ymin = ymin, range = range)
    return(output)
}




#Colour palette selection options
palette_FUN <- function(categorisation) {
    if (bandw == "Y") {
        colourcount <- length(unique(eval(parse(text = paste("graphdata$", categorisation)))))
        BW_palette <- grey.colors(colourcount, start = gr_bw_low, end = gr_bw_high)
        Gr_palette <- BW_palette
    } else {
        colourcount <- length(unique(eval(parse(text = paste("graphdata$", categorisation)))))
        getPalette <- colorRampPalette(brewer.pal(9, Palette_set))
        if (colourcount >= 10) {
            Col_palette <- getPalette(colourcount)
        } else {
            Col_palette <- brewer.pal(colourcount, Palette_set)
        }
        Gr_palette <- Col_palette
    }
    return(Gr_palette)
}


#Colour range for individual animals on case profiles plot and power curves
IVS_F_cpplot_colour <- function(subject) {
    colourcount_A = length(unique(eval(parse(text = paste("graphdata$", subject)))))
    getPalette_A = colorRampPalette(brewer.pal(9, Palette_set))
    if (colourcount_A >= 10) {
        Col_palette_A <- getPalette_A(colourcount_A)
    } else {
        Col_palette_A <- brewer.pal(colourcount_A, Palette_set)
    }
    BW_palette_A <- grey.colors(colourcount_A, start = gr_bw_low, end = gr_bw_high)

    if (bandw != "N") {
        Gr_palette_A <- BW_palette_A
        Gr_line <- BW_line
        Gr_fill <- BW_fill
    } else {
        Gr_palette_A <- Col_palette_A
        Gr_line <- Col_line
        Gr_fill <- Col_fill
    }

    output <- list(Gr_palette_A = Gr_palette_A, Gr_line = Gr_line, Gr_fill = Gr_fill)
    return(output)
}













#===================================================================================================
#===================================================================================================
#Graphical GGPlot2 functions
#===================================================================================================
#===================================================================================================

#-------------------------------------------------------------------------------------------
#Scatterplots (Graphics, Correlation modules, resid plots, QQplots)
#-------------------------------------------------------------------------------------------
NONCAT_SCAT <- function(typez) {
    g <- ggplot(graphdata, aes(x = xvarrr_IVS, y = yvarrr_IVS)) +
        theme_map +
        mytheme +

        ylab(YAxisTitle) +
        xlab(XAxisTitle) +
        ggtitle(MainTitle2)

    if (typez == "SMPA_PLOT") {
        g1 <- g + theme(axis.text.x = element_text(hjust = 1, angle = 45))
    } else {
        g1 <- g
    }

    if (infiniteslope == "N" && is.numeric(graphdata$xvarrr_IVS) == "TRUE" && is.numeric(graphdata$yvarrr_IVS) == "TRUE" && LinearFit == "Y") {
        g2 <- g1 + geom_smooth(method = lm, se = FALSE, lty = Line_type, size = Line_size, color = alpha(Gr_line, Gr_alpha), fullrange = TRUE)
    } else {
        g2 <- g1
    }

    if (typez == "RESIDPLOT") {
        g3 <- g2 + geom_hline(yintercept = 0, lty = 1, size = Line_size, colour = Gr_line) +
            geom_hline(yintercept = -2, lty = Gr_line_type, size = Line_size, colour = Gr_line) +
            geom_hline(yintercept = -3, lty = Gr_line_type, size = Line_size, colour = Gr_line) +
            geom_hline(yintercept = 2, lty = Gr_line_type, size = Line_size, colour = Gr_line) +
            geom_hline(yintercept = 3, lty = Gr_line_type, size = Line_size, colour = Gr_line) +
          scale_y_continuous(breaks = -20:20)
        scale_x_continuous(breaks = pretty_breaks())

    } else {
        g3 <- g2
    }

    if (typez == "cook") {
        g4 <- g3 + geom_hline(yintercept = cutoff, lty = Gr_line_type, size = Line_size, colour = Gr_line)

    } else {
        g4 <- g3
    }

    if (typez == "QQPLOT") {
        g5 <- g4 + scale_x_continuous(breaks = pretty_breaks()) +
        scale_y_continuous(breaks = pretty_breaks())
    } else {
        g5 <- g4
    }

    if (typez == "PCAPLOT") {
        g6 <- g5 + geom_text(size = 3, aes(label = obznames), hjust = 0, vjust = 1)
    } else {
        g6 <- g5
    }
    g7 <- g6 + geom_point(size = Point_size, shape = Point_shape, color = "black", fill = Gr_fill, position = position_jitter(w = w_Gr_jit, h = h_Gr_jit))


    suppressWarnings(print(g7))
}


ONECATSEP_SCAT <- function() {
    g <- ggplot(graphdata, aes(x = xvarrr_IVS, y = yvarrr_IVS)) +
        theme_map +
        mytheme +

        ylab(YAxisTitle) +
        xlab(XAxisTitle) +
        ggtitle(MainTitle2) +

        geom_point(size = Point_size, colour = "black", shape = Point_shape, fill = Gr_fill, position = position_jitter(w = w_Gr_jit, h = h_Gr_jit)) +
        facet_wrap(~l_l)

    if (is.numeric(graphdata$xvarrr_IVS) == "TRUE" && is.numeric(graphdata$yvarrr_IVS) == "TRUE" && LinearFit == "Y") {
        g1 <- g + geom_smooth(graphdata = subset(graphdata, catvartest != "N"), method = "lm", se = FALSE, lty = Line_type, size = Line_size, color = alpha(Gr_line, Gr_alpha), fullrange = TRUE)
    } else {
        g1 <- g
    }
    suppressWarnings(print(g1))
}


TWOCATSEP_SCAT <- function() {
    g <- ggplot(graphdata, aes(xvarrr_IVS, yvarrr_IVS)) +
        theme_map +
        mytheme +

        ylab(YAxisTitle) +
        xlab(XAxisTitle) +
        ggtitle(MainTitle2) +

        geom_point(size = Point_size, colour = "black", shape = Point_shape, fill = Gr_fill, position = position_jitter(w = w_Gr_jit, h = h_Gr_jit)) +
        facet_grid(firstcatvarrr_IVS ~ secondcatvarrr_IVS, scale = scalexy)

    if (is.numeric(graphdata$xvarrr_IVS) == "TRUE" && is.numeric(graphdata$yvarrr_IVS) == "TRUE" && LinearFit == "Y") {
        g1 <- g + geom_smooth(graphdata = subset(graphdata, catvartest != "N"), method = "lm", se = FALSE, lty = Line_type, size = Line_size, color = alpha(Gr_line, Gr_alpha), fullrange = TRUE)
    } else {
        g1 <- g
    }
    suppressWarnings(print(g1))
}

OVERLAID_SCAT <- function() {
    g <- ggplot(graphdata, aes(xvarrr_IVS, yvarrr_IVS)) +
        theme_map +
        mytheme +
        theme(legend.position = Gr_legend_pos) +

        ylab(YAxisTitle) +
        xlab(XAxisTitle) +
        ggtitle(MainTitle2) +

        scale_color_manual(values = Gr_palette) +
        scale_fill_manual(values = Gr_palette) +
        geom_point(graphdata = subset(graphdata, catvartest != "N"), aes(fill = l_l), colour = "black", size = Point_size, shape = Point_shape, position = position_jitter(w = w_Gr_jit, h = h_Gr_jit))

    if (dim(subset(graphdata, catvartest != "N"))[1] >= 1 && is.numeric(graphdata$xvarrr_IVS) == "TRUE" && is.numeric(graphdata$yvarrr_IVS) == "TRUE" && LinearFit == "Y") {
        g1 <- g + geom_smooth(graphdata = subset(graphdata, catvartest != "N"), aes(colour = l_l), method = "lm", se = FALSE, lty = Line_type, size = Line_size, fullrange = TRUE)
    } else {
        g1 <- g
    }

    if (Labelz_IVS_ == "Y") {
        g2 <- g1 + geom_text(size = 3, aes(label = obznames), hjust = 0, vjust = 1)
    } else {
        g2 <- g1
    }

    suppressWarnings(print(g2))
}

#-------------------------------------------------------------------------------------------
#SEM plots (Graphics module)
#-------------------------------------------------------------------------------------------
NONCAT_SEM <- function() {
    g <- ggplot(graphdata, aes(x = xvarrr_IVS_SEM, y = yvarrr_IVS)) +
        theme_map +
        mytheme +

        ylab(YAxisTitle) +
        xlab(XAxisTitle) +
        ggtitle(MainTitle2)

    if (SEMPlotType == "ColumnPlot") {
        g1 <- g + geom_errorbar(data = graphdata_SEM, aes(y = mean.y, ymax = mean.y + se.y, ymin = mean.y - se.y), width = ErrorBarWidth2) +
            geom_bar(data = graphdata_SEM, aes(y = mean.y, x = xvarrr_IVS_SEM), stat = "identity", fill = Gr_fill, colour = "black") +
            geom_hline(yintercept = 0)
    } else {
        g1 <- g + geom_errorbar(data = graphdata_SEM, aes(y = mean.y, ymax = mean.y + se.y, ymin = mean.y - se.y), width = ErrorBarWidth2) +
              stat_summary(fun.y = mean, geom = 'line', aes(group = 1), colour = Gr_line) +
            geom_point(data = graphdata_SEM, aes(y = mean.y, x = xvarrr_IVS_SEM), fill = Gr_line, colour = "black", size = Point_size, shape = Point_shape)
    }

    if (displaypointSEM == "Y") {
        Point_size2 <- Point_size / 1.5
        g2 <- g1 + geom_point(size = Point_size2, shape = Point_shape, color = "black", fill = "black")
    } else {
        g2 <- g1
    }
    suppressWarnings(print(g2))
}


ONECATSEP_SEM <- function() {
    g <- ggplot(graphdata, aes(x = xvarrr_IVS_SEM, y = yvarrr_IVS)) +
        theme_map +
        mytheme +

        ylab(YAxisTitle) +
        xlab(XAxisTitle) +
        ggtitle(MainTitle2)


    if (SEMPlotType == "ColumnPlot") {
        g1 <- g + geom_errorbar(data = graphdata_SEM, aes(y = mean.y, ymax = mean.y + se.y, ymin = mean.y - se.y), width = ErrorBarWidth2) +
            geom_bar(data = graphdata_SEM, aes(y = mean.y, x = xvarrr_IVS_SEM), stat = "identity", fill = Gr_fill, colour = "black", pos = 'dodge') +
            facet_wrap(~l_l) +
            geom_hline(yintercept = 0)
    } else {
        g1 <- g + geom_errorbar(data = graphdata_SEM, aes(y = mean.y, ymax = mean.y + se.y, ymin = mean.y - se.y), width = ErrorBarWidth2) +
          stat_summary(fun.y = mean, geom = 'line', aes(group = 1), colour = Gr_line) +
            geom_point(data = graphdata_SEM, aes(y = mean.y, x = xvarrr_IVS_SEM), colour = "black", size = Point_size, shape = Point_shape, fill = Gr_line) +
          facet_wrap(~l_l)
    }

    if (displaypointSEM == "Y") {
        Point_size2 <- Point_size / 1.5
        g2 <- g1 + geom_point(size = Point_size2, shape = Point_shape, color = "black", fill = "black")
    } else {
        g2 <- g1
    }

    suppressWarnings(print(g2))
}


TWOCATSEP_SEM <- function() {
    g <- ggplot(graphdata, aes(x = xvarrr_IVS_SEM, y = yvarrr_IVS)) +
        theme_map +
        mytheme +

        ylab(YAxisTitle) +
        xlab(XAxisTitle) +
        ggtitle(MainTitle2)

    if (SEMPlotType == "ColumnPlot") {
        g1 <- g + geom_errorbar(data = graphdata_SEM, aes(y = mean.y, ymax = mean.y + se.y, ymin = mean.y - se.y), width = ErrorBarWidth2) +
            geom_bar(data = graphdata_SEM, aes(y = mean.y, x = xvarrr_IVS_SEM), stat = "identity", fill = Gr_fill, colour = "black", pos = 'dodge') +
          facet_grid(firstcatvarrr_IVS ~ secondcatvarrr_IVS) +
            geom_hline(yintercept = 0)
    } else {
        g1 <- g + geom_errorbar(data = graphdata_SEM, aes(y = mean.y, ymax = mean.y + se.y, ymin = mean.y - se.y), width = ErrorBarWidth2) +
          stat_summary(fun.y = mean, geom = 'line', aes(group = 1), colour = Gr_line) +
            geom_point(data = graphdata_SEM, aes(y = mean.y, x = xvarrr_IVS_SEM), colour = "black", size = Point_size, shape = Point_shape, fill = Gr_line) +
          facet_grid(firstcatvarrr_IVS ~ secondcatvarrr_IVS)
    }

    if (displaypointSEM == "Y") {
        Point_size2 <- Point_size / 1.5
        g2 <- g1 + geom_point(size = Point_size2, shape = Point_shape, color = "black", fill = "black")
    } else {
        g2 <- g1
    }

    suppressWarnings(print(g2))
}


OVERLAID_SEM <- function() {
    g <- ggplot(graphdataSEM_overall, aes(x = xvarrr_IVS_SEM, y = yvarrr_IVS), colour = l_l) +
        theme_map +
        mytheme +
        theme(legend.position = Gr_legend_pos) +

      ylab(YAxisTitle) +
      xlab(XAxisTitle) +
      ggtitle(MainTitle2) +

      scale_color_manual(values = Gr_palette) +
      scale_fill_manual(values = Gr_palette)

    if (SEMPlotType == "ColumnPlot") {
        g1 <- g + geom_errorbar(data = graphdataSEM_means, aes(y = mean.y, ymax = mean.y + se.y, ymin = mean.y - se.y, group = l_l), width = ErrorBarWidth2, pos = position_dodge(w = 0.9), colour = "black") +
            geom_bar(data = graphdataSEM_means, aes(y = mean.y, x = xvarrr_IVS_SEM, group = l_l, fill = l_l), stat = "identity", colour = "black", pos = 'dodge') +
            geom_hline(yintercept = 0)
    } else {
        g1 <- g + geom_errorbar(data = graphdataSEM_means, aes(y = mean.y, ymax = mean.y + se.y, ymin = mean.y - se.y, group = l_l), width = ErrorBarWidth2, pos = position_dodge(w = 0.1), colour = "black") +
          stat_summary(data = graphdataSEM, fun.y = mean, geom = 'line', aes(group = l_l, colour = l_l), pos = position_dodge(w = 0.1)) +
            geom_point(data = graphdataSEM_means, aes(y = mean.y, x = xvarrr_IVS_SEM, fill = l_l), colour = "black", size = Point_size, shape = Point_shape, pos = position_dodge(w = 0.1))
    }

    if (displaypointSEM == "Y" && SEMPlotType == "ColumnPlot") {
        Point_size2 <- Point_size / 1.5
        g2 <- g1 + geom_point(data = graphdataSEM_overall, pos = position_dodge(w = 0.9), aes(group = l_l), size = Point_size2, shape = Point_shape, color = "black", fill = "black")
    }
    if (displaypointSEM == "Y" && SEMPlotType == "LinePlot") {
        Point_size2 <- Point_size / 1.5
        g2 <- g1 + geom_point(data = graphdataSEM, pos = position_dodge(w = 0.1), aes(group = l_l), size = Point_size2, shape = Point_shape, color = "black", fill = "black")
    }
    if (displaypointSEM == "N") {
        g2 <- g1
    }

    suppressWarnings(print(g2))
}

#graphdataSEM_overall=subset(graphdataSEM_overall, graphdataSEM_overall$Type != "Blanks")

#-------------------------------------------------------------------------------------------
#case profiles plot (Graphics module)
#-------------------------------------------------------------------------------------------
NONCAT_CPP <- function() {
    g <- ggplot(graphdata, aes(Time_IVS, yvarrr_IVS)) +
        theme_map +
        mytheme +
        theme(legend.position = "none") +

        ylab(YAxisTitle) +
        xlab(XAxisTitle) +
        ggtitle(MainTitle2) +

        scale_color_manual(values = Gr_palette_A) +
        scale_fill_manual(values = Gr_palette_A) +
        geom_point(aes(colour = Animal_IVS), size = 3, shape = 16) +
        geom_line(aes(group = Animal_IVS, color = Animal_IVS), size = Line_size)

    if (ReferenceLine != "NULL") {
        g1 <- g + geom_hline(yintercept = Gr_intercept, lty = Gr_line_type, size = Line_size, colour = Gr_line)
    } else {
        g1 <- g
    }
    suppressWarnings(print(g1))
}


ONECATSEP_CPP <- function() {
    g <- ggplot(graphdata, aes(Time_IVS, yvarrr_IVS)) +
        theme_map +
        mytheme +
        theme(legend.position = "none") +

        ylab(YAxisTitle) +
        xlab(XAxisTitle) +
        ggtitle(MainTitle2) +

        scale_color_manual(values = Gr_palette_A) +
        geom_line(aes(group = Animal_IVS, color = Animal_IVS), size = Line_size) +
        geom_point(aes(colour = Animal_IVS), size = 3, shape = 16) +
        facet_wrap(~l_l)

    if (ReferenceLine != "NULL") {
        g1 <- g + geom_hline(yintercept = Gr_intercept, lty = Gr_line_type, size = Line_size, colour = Gr_line)
    } else {
        g1 <- g
    }
    suppressWarnings(print(g1))
}


TWOCATSEP_CPP <- function() {
    g <- ggplot(graphdata, aes(Time_IVS, yvarrr_IVS)) +
        theme_map +
        mytheme +
        theme(legend.position = "none") +

        ylab(YAxisTitle) +
      xlab(XAxisTitle) +
        ggtitle(MainTitle2) +

        scale_color_manual(values = Gr_palette_A) +
        geom_line(aes(group = Animal_IVS, color = Animal_IVS), size = Line_size) +
        geom_point(aes(colour = Animal_IVS), size = 3, shape = 16) +
        facet_grid(firstcatvarrr_IVS ~ secondcatvarrr_IVS)

    if (ReferenceLine != "NULL") {
        g1 <- g + geom_hline(yintercept = Gr_intercept, lty = Gr_line_type, size = Line_size, colour = Gr_line)
    } else {
        g1 <- g
    }
    suppressWarnings(print(g1))
}


OVERLAID_CPP <- function() {
    g <- ggplot(graphdata, aes(Time_IVS, yvarrr_IVS, color = l_l)) +
        theme_map +
        mytheme +
        theme(legend.position = Gr_legend_pos) +

        ylab(YAxisTitle) +
        xlab(XAxisTitle) +
        ggtitle(MainTitle2) +

        scale_color_manual(values = Gr_palette) +
        geom_point(aes(colour = l_l), size = 3, shape = 16) +
        geom_line(aes(group = Animal_IVS), size = Line_size)

    if (ReferenceLine != "NULL") {
        g1 <- g + geom_hline(yintercept = Gr_intercept, lty = Gr_line_type, size = Line_size, colour = Gr_line)
    } else {
        g1 <- g
    }
    suppressWarnings(print(g1))
}

#-------------------------------------------------------------------------------------------
#Box plot (Graphics module)
#-------------------------------------------------------------------------------------------
NONCAT_BOX <- function() {
    g <- ggplot() +
        theme_map +
        mytheme +

        ylab(YAxisTitle) +
        xlab(XAxisTitle) +
        ggtitle(MainTitle2) +

        coord_cartesian(ylim = c(ymin - range, ymax + range))

    if (Outliers == "Y") {
        g1 <- g + geom_boxplot(data = boxdata, aes(x = xvarrr_IVS_BP, ymin = minq, lower = lowerq, middle = medq, upper = upperq, ymax = maxq), stat = "identity", fill = Gr_fill) +
            geom_point(data = outlierdata, aes(x = xvarrr_IVS_BP, y = yvarrr_IVS), size = Point_size, shape = Point_shape, color = "black", fill = Gr_fill)
    } else {
        g1 <- g + geom_boxplot(data = boxdata, aes(x = xvarrr_IVS_BP, ymin = minq, lower = lowerq, middle = medq, upper = upperq, ymax = maxq), stat = "identity", outlier.shape = NA, fill = Gr_fill)
    }

    if (displaypointBOX == "Y") {
        Point_size2 <- Point_size / 1.5
        g2 <- g1 + geom_point(data = graphdata, aes(x = xvarrr_IVS, y = yvarrr_IVS), size = Point_size2, shape = Point_shape, color = "black", fill = "black")
    } else {
        g2 <- g1
    }
    suppressWarnings(print(g2))
}



ONECATSEP_BOX <- function() {
    g <- ggplot() +
        theme_map +
        mytheme +

        ylab(YAxisTitle) +
        xlab(XAxisTitle) +

        ggtitle(MainTitle2) +
        facet_wrap(~l_l) +
        coord_cartesian(ylim = c(ymin - range, ymax + range))

    if (Outliers == "Y") {
        g1 <- g + geom_boxplot(data = boxdata, aes(x = xvarrr_IVS_BP, ymin = minq, lower = lowerq, middle = medq, upper = upperq, ymax = maxq), stat = "identity", fill = Gr_fill) +
            geom_point(data = outlierdata, aes(x = xvarrr_IVS_BP, y = yvarrr_IVS), size = Point_size, shape = Point_shape, color = "black", fill = Gr_fill)
    } else {
        g1 <- g + geom_boxplot(data = boxdata, aes(x = xvarrr_IVS_BP, ymin = minq, lower = lowerq, middle = medq, upper = upperq, ymax = maxq), stat = "identity", outlier.shape = NA, fill = Gr_fill)
    }
    if (displaypointBOX == "Y") {
        #Create dataset with the same facet for original dataset
        graphdata2xxx <- graphdata
        graphdata2xxx$xvarrr_IVS_BP <- graphdata2xxx$xvarrr_IVS
        graphdata2xxx$yvarrr_IVS_BP <- graphdata2xxx$yvarrr_IVS
        graphdata2xxx$l_ll <- graphdata2xxx$l_l

        Point_size2 <- Point_size / 1.5
        g2 <- g1 + geom_point(data = graphdata2xxx, aes(x = xvarrr_IVS_BP, y = yvarrr_IVS_BP), size = Point_size2, shape = Point_shape, color = "black", fill = "black")
    } else {
        g2 <- g1
    }

    suppressWarnings(print(g2))
}


TWOCATSEP_BOX <- function() {
    g <- ggplot() +
        theme_map +
        mytheme +

        ylab(YAxisTitle) +
        xlab(XAxisTitle) +
        ggtitle(MainTitle2) +

        facet_grid(firstcatvarrr_IVS ~ secondcatvarrr_IVS) +
        coord_cartesian(ylim = c(ymin - range, ymax + range))

    if (Outliers == "Y") {
        g1 <- g + geom_boxplot(data = boxdata, aes(x = xvarrr_IVS_BP, ymin = minq, lower = lowerq, middle = medq, upper = upperq, ymax = maxq), stat = "identity", fill = Gr_fill) +
            geom_point(data = outlierdata, aes(x = xvarrr_IVS_BP, y = yvarrr_IVS), size = Point_size, shape = Point_shape, color = "black", fill = Gr_fill)
    } else {
        g1 <- g + geom_boxplot(data = boxdata, aes(x = xvarrr_IVS_BP, ymin = minq, lower = lowerq, middle = medq, upper = upperq, ymax = maxq), stat = "identity", outlier.shape = NA, fill = Gr_fill)
    }
    if (displaypointBOX == "Y") {
        #Create dataset with the same facet for original dataset
        graphdata2xxx <- graphdata
        graphdata2xxx$xvarrr_IVS_BP <- graphdata2xxx$xvarrr_IVS
        graphdata2xxx$yvarrr_IVS_BP <- graphdata2xxx$yvarrr_IVS
        graphdata2xxx$firstcatvarrr_IVS_BP <- graphdata2xxx$firstcatvarrr_IVS
        graphdata2xxx$secondcatvarrr_IVS_BP <- graphdata2xxx$secondcatvarrr_IVS

        Point_size2 <- Point_size / 1.5
        g2 <- g1 + geom_point(data = graphdata2xxx, aes(x = xvarrr_IVS_BP, y = yvarrr_IVS_BP), size = Point_size2, shape = Point_shape, color = "black", fill = "black")
    } else {
        g2 <- g1
    }

    suppressWarnings(print(g2))
}


OVERLAID_BOX <- function() {
    g <- ggplot() +
        theme_map +
        mytheme +
        theme(legend.position = Gr_legend_pos) +

        ylab(YAxisTitle) +
        xlab(XAxisTitle) +
        ggtitle(MainTitle2) +

        scale_color_manual(values = Gr_palette) +
        scale_fill_manual(values = Gr_palette) +
        coord_cartesian(ylim = c(ymin - range, ymax + range))

    if (Outliers == "Y") {
        g1 <- g + geom_boxplot(data = boxdata, aes(x = xvarrr_IVS_BP, ymin = minq, lower = lowerq, middle = medq, upper = upperq, ymax = maxq, fill = l_l), stat = "identity") +
            if (outliertest == "Y") {
                geom_point(data = outlierdata, aes(x = xvarrr_IVS_BP, y = yvarrr_IVS, fill = l_l), size = Point_size, shape = Point_shape, color = "black", position = position_dodge(width = 0.9))
            }
    } else {
        g1 <- g + geom_boxplot(data = boxdata, aes(x = xvarrr_IVS_BP, ymin = minq, lower = lowerq, middle = medq, upper = upperq, ymax = maxq, fill = l_l), stat = "identity", outlier.shape = NA)
    }
    if (displaypointBOX == "Y") {
        Point_size2 <- Point_size / 1.5
        g2 <- g1 + geom_point(pos = position_dodge(w = 0.9), data = graphdataBOX_overall, aes(x = xvarrr_IVS_BP, y = yvarrr_IVS, group = l_l), size = Point_size2, shape = Point_shape, color = "black", fill = "black")
    } else {
        g2 <- g1
    }

    suppressWarnings(print(g2))
}

#-------------------------------------------------------------------------------------------
#Histograms plot (Graphics module)
#-------------------------------------------------------------------------------------------
NONCAT_HIS <- function() {
    g <- ggplot(graphdata, aes(x = yvarrr_IVS)) +
        theme_map +
        mytheme +

        ylab("Density") +
        xlab(YAxisTitle) +
        ggtitle(MainTitle2) +

        geom_histogram(aes(y = ..density..), colour = "black", fill = Gr_fill, binwidth = (binrange)) +
        stat_function(fun = dnorm, args = list(mean = mean(graphdata$yvarrr_IVS), sd = sd(graphdata$yvarrr_IVS)), alpha = Gr_alpha, color = Gr_line, size = Line_size)
    suppressWarnings(print(g))
}


ONECATSEP_HIS <- function() {
    g <- ggplot(graphdata, aes(x = yvarrr_IVS)) +
        theme_map +
        mytheme +

        ylab("Density") +
        xlab(YAxisTitle) +
        ggtitle(MainTitle2) +

        geom_histogram(aes(y = ..density..), colour = "black", fill = Gr_fill, binwidth = (binrange)) +
        geom_line(aes(y = density), size = 1, data = normaldens, colour = Gr_line, lty = Line_type) +
        facet_wrap(~l_l)
    suppressWarnings(print(g))
}


TWOCATSEP_HIS <- function() {
    g <- ggplot(graphdatazzz, aes(x = yvarrr_IVS)) +
        theme_map +
        mytheme +

        ylab("Density") +
        xlab(YAxisTitle) +
        ggtitle(MainTitle2) +

        geom_histogram(aes(y = ..density..), colour = "black", fill = Gr_fill, binwidth = (binrange)) +
        geom_line(aes(y = density), size = 1, data = normaldens, colour = Gr_line, lty = Line_type) +
        facet_grid(firstcatvarrr_IVS ~ secondcatvarrr_IVS)
    suppressWarnings(print(g))
}


OVERLAID_HIS <- function() {
    gx <- ggplot(histdata, aes(x = yvarrr_IVS)) +
        theme_map +
        mytheme +
        theme(legend.position = Gr_legend_pos) +

        ylab("Density") +
        xlab(YAxisTitle) +
        ggtitle(MainTitle2) +

        scale_fill_manual(values = Gr_palette) +
        geom_histogram(aes(y = ..density.., fill = l_l), position = 'dodge', colour = "black", binwidth = (binrange))

    if (NormalDistFit == "Y") {
        for (i in 1:nlevels) {
            gx = gx + stat_function(fun = dnorm, size = 1, color = Gr_palette[i], args = list(mean = mean(eval(parse(text = paste("histdata$Tzzz", i, sep = ""))), na.rm = TRUE), sd = sd(eval(parse(text = paste("histdata$Tzzz", i, sep = ""))), na.rm = TRUE)))

        }
    }
    suppressWarnings(print(gx))
}




#-------------------------------------------------------------------------------------------
#Matrix plot (correlation module)
#-------------------------------------------------------------------------------------------

NONCAT_MAT <- function() {
    g <- ggplot(graphdata, aes(xvarrr_IVS, yvarrr_IVS)) +
        theme_map +
        mytheme +

        ylab(YAxisTitle) +
        xlab(XAxisTitle) +
        ggtitle(MainTitle2) +

        geom_point(size = Point_size, colour = "black", shape = Point_shape, fill = Gr_fill, position = position_jitter(w = w_Gr_jit, h = h_Gr_jit)) +
        facet_grid(firstcatvarrr_IVS ~ secondcatvarrr_IVS, scales = scalexy)

    if (is.numeric(graphdata$xvarrr_IVS) == "TRUE" && is.numeric(graphdata$yvarrr_IVS) == "TRUE" && LinearFit == "Y") {
        g1 <- g + geom_smooth(graphdata = subset(graphdata, catvartest != "N"), method = "lm", se = FALSE, lty = Line_type, size = Line_size, color = alpha(Gr_line, Gr_alpha), fullrange = TRUE)
    } else {
        g1 <- g
    }
    suppressWarnings(print(g1))
}


OVERLAID_MAT <- function() {
    g <- ggplot(graphdata, aes(xvarrr_IVS, yvarrr_IVS)) +
        theme_map +
        mytheme +
        theme(legend.position = Gr_legend_pos) +

        ylab(YAxisTitle) +
        xlab(XAxisTitle) +
        ggtitle(MainTitle2) +

        scale_color_manual(values = Gr_palette) +
        scale_fill_manual(values = Gr_palette) +

        geom_point(graphdata = subset(graphdata, catvartest != "N"), aes(fill = l_li), colour = "black", size = Point_size, shape = Point_shape, position = position_jitter(w = w_Gr_jit, h = h_Gr_jit)) +
        facet_grid(firstcatvarrr_IVS ~ secondcatvarrr_IVS, scales = scalexy)

    if (dim(subset(graphdata, catvartest != "N"))[1] >= 1 && is.numeric(graphdata$xvarrr_IVS) == "TRUE" && is.numeric(graphdata$yvarrr_IVS) == "TRUE" && LinearFit == "Y") {
        g1 <- g + geom_smooth(graphdata = subset(graphdata, catvartest != "N"), aes(colour = l_li), method = "lm", se = FALSE, lty = Line_type, size = Line_size, fullrange = TRUE)
    } else {
        g1 <- g
    }
    suppressWarnings(print(g1))
}




#-------------------------------------------------------------------------------------------
#Dose response plots       
#-------------------------------------------------------------------------------------------

Dose_Resp_ExQC <- function() {
    g <- ggplot(finaldata, aes(x = Dose, y = Response, fill = Type)) +
        theme_map +
        mytheme +
        theme(legend.position = "none") +

        ylab(YAxisTitle) +
        xlab(XAxisTitle) +
        ggtitle(MainTitle2) +

        scale_fill_manual(values = Gr_palette) +
        geom_line(data = finaldata[finaldata$Type == "Prediction curve",], colour = "Black", lty = Line_type, size = Line_size) +
        geom_point(data = finaldata[finaldata$Type != "Prediction curve",], aes(col = Type), size = Point_size, colour = "black", shape = Point_shape)

    suppressWarnings(print(g))
}

Dose_Resp_IncQC <- function() {
    g <- ggplot(finaldata, aes(x = Dose, y = Response, fill = Type)) +
        theme_map +
        mytheme +
        theme(legend.position = Gr_legend_pos2) +

        ylab(YAxisTitle) +
        xlab(XAxisTitle) +
        ggtitle(MainTitle2) +

        scale_fill_manual(values = Gr_palette, breaks = c(' Assay standards', ' Quality controls', 'Prediction curve'), labels = c('Assay standards', 'Quality controls', ' ')) +
        geom_line(data = finaldata[finaldata$Type == "Prediction curve",], colour = "Black", lty = Line_type, size = Line_size) +
        geom_point(data = finaldata[finaldata$Type != "Prediction curve",], aes(col = Type), size = Point_size, colour = "black", shape = Point_shape)

    suppressWarnings(print(g))
}


#-------------------------------------------------------------------------------------------
#LSMeans plot       
#-------------------------------------------------------------------------------------------

LSMPLOT_1 <- function() {
    g <- ggplot(graphdata, aes(x = Group_IVSq_, y = Mean)) +
      theme_map +
        mytheme +

      ylab(YAxisTitle) +
      xlab(XAxisTitle) +
      ggtitle(MainTitle2) +

        scale_y_continuous(breaks = pretty_breaks()) +
      geom_errorbar(aes(ymax = Upper, ymin = Lower), size = Line_size, colour = Gr_fill, width = ErrorBarWidth) +
      stat_summary(fun.y = mean, geom = 'line', aes(group = 1), size = Line_size, colour = Gr_fill, lty = Line_type_solid) +
      geom_point(size = Point_size, shape = Point_shape, color = "black", fill = Gr_fill)

    suppressWarnings(print(g))
}


LSMPLOT_2 <- function(number) {
    g <- ggplot(graphdata, aes(x = jj_1, y = Mean), colour = jj_2) +
      theme_map +
        mytheme +
        theme(legend.position = Gr_legend_pos) +

      ylab(YAxisTitle) +
      xlab(XAxisTitle) +
      ggtitle(MainTitle2) +

      scale_color_manual(values = Gr_palette) +
      scale_fill_manual(values = Gr_palette) +
        scale_y_continuous(breaks = pretty_breaks()) +
      geom_errorbar(aes(group = jj_2, ymax = Upper, ymin = Lower, colour = jj_2), size = Line_size, pos = position_dodge(w = 0.2), width = ErrorBarWidth) +
       stat_summary(fun.y = mean, geom = 'line', aes(group = jj_2, colour = jj_2), size = Line_size, pos = position_dodge(w = 0.2), lty = Line_type_solid) +
        geom_point(aes(fill = jj_2), colour = "black", size = Point_size, shape = Point_shape, pos = position_dodge(w = 0.2))

    if (number == "three") {
        g1 <- g + facet_wrap(~jj_3)
    } else if (number == "four") {
        g1 <- g + facet_grid(jj_3 ~ jj_4)
    } else {
        g1 <- g
    }

    suppressWarnings(print(g1))
}



LSMPLOT_diff <- function() {
    g <- ggplot(graphdata, aes(x = Group_IVSq_, y = Mean)) +
      theme_map +
        mytheme +

      ylab(YAxisTitle) +
      xlab(XAxisTitle) +
      ggtitle(MainTitle2) +

        scale_y_continuous(breaks = pretty_breaks()) +
      geom_errorbar(aes(ymax = Upper, ymin = Lower), size = Line_size, colour = Gr_fill, width = ErrorBarWidth) +
      geom_point(size = Point_size, shape = Point_shape, color = "black", fill = Gr_fill) +
        geom_hline(yintercept = Gr_intercept, lty = Gr_line_type, size = Line_size, colour = Gr_line)

    suppressWarnings(print(g))
}


#-------------------------------------------------------------------------------------------
#Power plot       
#-------------------------------------------------------------------------------------------

#Nested design analysis plots
POWERPLOT_ORIGINAL <- function() {
    g <- ggplot(power2data, aes(x = diffs, y = value)) +
        theme_map +
        mytheme +
        theme(legend.position = Gr_legend_pos2, legend.title = element_text(colour = "black")) +

        ylab("Statistical power (%)") +
        xlab(XAxisTitle) +
        ggtitle(MainTitle2) +

        coord_cartesian(ylim = c(powerFrom, powerTo)) +
        scale_y_continuous(breaks = pretty_breaks()) +
        scale_x_continuous(breaks = pretty_breaks())
    g1 <- g + geom_line(aes(group = variable), lty = Line_type, size = Line_size, color = alpha(Gr_line, Gr_alpha))

    suppressWarnings(print(g1))
}

POWERPLOT_NEW <- function(titlez) {
    g <- ggplot(graphdata, aes(x = diffs, y = value)) +
      theme_map +
        mytheme +
        theme(legend.position = Gr_legend_pos2, legend.title = element_text(colour = "black")) +
      guides(color = guide_legend(title = titlez)) +
      ylab("Statistical power (%)") +
      xlab(XAxisTitle) +
      ggtitle(MainTitle2) +

      coord_cartesian(ylim = c(powerFrom, powerTo)) +
      scale_x_continuous(breaks = pretty_breaks()) +
      scale_y_continuous(breaks = pretty_breaks()) +
      scale_colour_manual(name = namez, breaks = lin_list2, labels = userlistgr, values = Gr_palette_A)
    g1 <- g + geom_line(aes(group = variable, color = variable), size = Line_size)

    suppressWarnings(print(g1))
}



#Means Comparison module plots

POWERPLOT_ABSOLUTE <- function(power2data, XAxisTitle, MainTitle2, lin_list2, Gr_palette_P) {
    g <- ggplot(power2data, aes(x = sample, y = value)) +
      theme_map +
        mytheme +
        theme(legend.position = Gr_legend_pos2, legend.title = element_text(colour = "black")) +

      ylab("Statistical power (%)") +
      xlab(XAxisTitle) +
      ggtitle(MainTitle2) +

      coord_cartesian(ylim = c(powerFrom, powerTo)) +
      scale_x_continuous(breaks = pretty_breaks()) +
      scale_y_continuous(breaks = pretty_breaks()) +
      scale_colour_manual(name = "Absolute change: ", breaks = lin_list2, labels = expectedChanges, values = Gr_palette_P)
    g1 <- g + geom_line(aes(group = variable, color = variable), size = Line_size)

    suppressWarnings(print(g1))
}

POWERPLOT_PERCENT <- function(power2data, XAxisTitle, MainTitle2, lin_list2, Gr_palette_P, expectedChanges2) {
    g <- ggplot(power2data, aes(x = sample, y = value)) +
      theme_map +
        mytheme +
        theme(legend.position = Gr_legend_pos2, legend.title = element_text(colour = "black")) +

      ylab("Statistical power (%)") +
      xlab(XAxisTitle) +
      ggtitle(MainTitle2) +

      coord_cartesian(ylim = c(powerFrom, powerTo)) +
      scale_x_continuous(breaks = pretty_breaks()) +
      scale_y_continuous(breaks = pretty_breaks()) +
      scale_colour_manual(name = "Percent change: ", breaks = lin_list2, labels = expectedChanges2, values = Gr_palette_P)
    g1 <- g + geom_line(aes(group = variable, color = variable), size = Line_size)

    suppressWarnings(print(g1))
}


#-------------------------------------------------------------------------------------------
#Survival plot       
#-------------------------------------------------------------------------------------------

SURVIVALPLOT <- function() {
    p <- ggplot(grdata2, aes(time, surv, group = V3)) +
    theme_map +
    mytheme +
    theme(legend.position = Gr_legend_pos2) +

    ylab(YAxisTitle) +
    xlab(XAxisTitle) +
    ggtitle(MainTitle2) +

    coord_cartesian(ylim = c(-0.05, 1.05)) +
    scale_x_continuous(breaks = pretty_breaks()) +
    scale_y_continuous(breaks = pretty_breaks()) +
    scale_color_manual(values = Gr_palette) +
    scale_fill_manual(values = Gr_palette) +
        geom_step(aes(colour = V3), lty = Line_type, size = Line_size)
    grdatax = subset(grdata2, grdata2$n.event == 0)

    if (dim(grdatax)[1] > 0) {
        p1 <- p + geom_point(data = grdatax, aes(x = time, y = surv, fill = V3), colour = "black", size = Point_size, shape = Point_shape)
    } else {
        p1 <- p
    }
    suppressWarnings(print(p1))
}



#-------------------------------------------------------------------------------------------
#Multivariate plots 
#-------------------------------------------------------------------------------------------
#Line plot of the variances
NONCAT_SEMx <- function() {
    g <- ggplot(graphdata_SEM, aes(x = xvarrr_IVS_SEM, y = yvarrr_IVS_SEM)) +
        theme_map +
        mytheme +
        theme(legend.position = Gr_legend_pos) +

        ylab(YAxisTitle) +
        xlab(XAxisTitle) +
        ggtitle(MainTitle2) +

            stat_summary(fun.y = mean, geom = 'line', aes(group = 1), colour = Gr_line)
    g1 <- g + geom_point(fill = Gr_line, colour = "black", size = Point_size, shape = Point_shape)
    suppressWarnings(print(g1))
}




#Biplot

PCbiplot <- function(PC, x = "PC1", y = "PC2") {
    #PC being a prcomp object
    data <- data.frame(obsnames = row.names(PC$x), PC$x)
    plot <- ggplot(data, aes_string(x = x, y = y)) +
        theme_map +
        mytheme +
        theme(legend.position = Gr_legend_pos) +

        geom_text(alpha = .7, size = 3, aes(label = obsnames)) +
     geom_hline(aes(0), size = .2) +
        geom_vline(aes(0), size = .2)

    datapc <- data.frame(varnames = rownames(PC$rotation), PC$rotation)
    mult <- min(
            (max(data[, y]) - min(data[, y]) / (max(datapc[, y]) - min(datapc[, y]))),
            (max(data[, x]) - min(data[, x]) / (max(datapc[, x]) - min(datapc[, x])))
            )
    datapc <- transform(datapc,
                    v1 = .7 * mult * (get(x)),
                    v2 = .7 * mult * (get(y))
                    )
    plot2 <- plot + coord_equal() +
        geom_text(data = datapc, aes(x = v1, y = v2, label = varnames), size = 5, vjust = 1, color = Gr_line) +
        geom_segment(data = datapc, aes(x = 0, y = 0, xend = v1, yend = v2), arrow = arrow(length = unit(0.2, "cm")), color = Gr_line)

    suppressWarnings(print(plot2))
}



#Cluster dendrogram
ggdendro <- function() {
    g <- ggdendrogram(h, theme_dendro = FALSE, rotate = TRUE) +
        theme_map +
        mytheme +

        ylab(YAxisTitle) +
        xlab(XAxisTitle)

    suppressWarnings(print(g))

}



#PLS CV and advCV plot
PLSCVplot <- function(grlegend) {
    g <- ggplot(finaltesttable, aes(Comp, value)) +
        theme_map +
        mytheme +
        theme(legend.position = grlegend) +

        ylab("RMSEP") +
        xlab("Number of components") +
        ggtitle(" ") +

        scale_color_manual(values = Gr_palette_A) +
        geom_point(aes(colour = type), size = 3, shape = 16) +
        geom_line(aes(group = type, color = type), size = Line_size) +
        facet_wrap(~variable, scales = "free_y")

    suppressWarnings(print(g))
}

#PLS loadings plot
PLSloadingsplot <- function() {
    g <- ggplot(graphdata, aes(X1, value)) +
        theme_map +
        mytheme +
        theme(legend.position = "right") +

        ylab("loading value") +
        xlab("variable") +
        ggtitle(" ") +
        scale_color_manual(values = Gr_palette_A) +
        scale_fill_manual(values = Gr_palette_A) +
        geom_point(aes(colour = X2), size = 3, shape = 16) +
        geom_line(aes(group = X2, color = X2), size = Line_size) +
        geom_hline(yintercept = 0, lty = 3, size = 0.8, colour = "black")
    suppressWarnings(print(g))
}


#===================================================================================================
#===================================================================================================
#References
#===================================================================================================
#===================================================================================================
refxx <- c("For more information on the theoretical approaches that are implemented within this module, see Bate and Clark (2014).")


R_refs <- function() {
    R_ref <- "R Development Core Team (2013). R: A language and environment for statistical computing. R Foundation for Statistical Computing, Vienna, Austria. URL http://www.R-project.org."
    mtvnorm_ref <- "Alan Genz, Frank Bretz, Tetsuhisa Miwa, Xuefei Mi, Friedrich Leisch, Fabian Scheipl, Torsten Hothorn (2014). mvtnorm: Multivariate Normal and t Distributions. R package version 0.9-9997. URL http://CRAN.R-project.org/package=mvtnorm"
    GridExtra_ref <- "Baptiste Auguie (2012). gridExtra: functions in Grid graphics. R  package version 0.9.1. http://CRAN.R-project.org/package=gridExtra"
    GGally_ref <- "Barret Schloerke, Jason Crowley, Di Cook, Heike Hofmann, Hadley Wickham, Francois Briatte, Moritz Marbach and Edwin Thoen (2014). GGally: Extension to ggplot2. R package version 0.4.5. http://CRAN.R-project.org/package=GGally"
    RColorBrewers_ref <- "Erich Neuwirth (2011). RColorBrewer: ColorBrewer palettes. R package  version 1.0-5. http://CRAN.R-project.org/package=RColorBrewer"
    GGPLot2_ref <- "H. Wickham. ggplot2: elegant graphics for data analysis. Springer New York, 2009."
    reshape_ref <- "H. Wickham. Reshaping data with the reshape package. Journal of Statistical Software, 21(12), 2007."
    plyr_ref <- "Hadley Wickham (2011). The Split-Apply-Combine Strategy for Data Analysis. Journal of Statistical Software, 40(1), 1-29. URL http://www.jstatsoft.org/v40/i01/."
    scales_ref <- "Hadley Wickham (2012). scales: Scale functions for graphics. R package version 0.2.3. http://CRAN.R-project.org/package=scales"
    car_ref <- "John Fox and Sanford Weisberg (2011). An {R} Companion to Applied Regression, Second Edition. Thousand Oaks CA: Sage. URL: http://socserv.socsci.mcmaster.ca/jfox/Books/Companion"
    nlme_ref <- "Jose Pinheiro, Douglas Bates, Saikat DebRoy, Deepayan Sarkar and the R Development Core Team (2013). nlme: Linear and Nonlinear Mixed Effects Models. R package version 3.1-111."
    NPMC_ref <- "Joerg Helms and Ullrich Munzel (2008). NPMC: Nonparametric Multiple Comparisons. R package version	1.0-7."
    R2HTML_ref <- "Lecoutre, Eric (2003). The R2HTML Package. R News, Vol 3. N. 3, Vienna, Austria."
    PROTO_ref <- "Louis Kates and Thomas Petzoldt (2012). proto: Prototype object-based programming. R package version 0.3-10. http://CRAN.R-project.org/package=proto"
    Contrast_ref <- "Max Kuhn, contributions from Steve Weston, Jed Wing, James Forester and Thorn Thaler (2013). contrast: A collection of contrast methods. R package version 0.19. http://CRAN.R-project.org/package=contrast"
    LSMEANS_ref <- "Russell V. Lenth (2014). lsmeans: Least-Squares Means. R package version 2.00-1. http://CRAN.R-project.org/package=lsmeans"
    Survival_ref <- "Therneau T (2014). _A Package for Survival Analysis in S_. R package version 2.37-7, URL: http://CRAN.R-project.org/package=survival."
    multcomp_ref <- "Torsten Hothorn, Frank Bretz and Peter Westfall (2008). Simultaneous  Inference in General Parametric Models. Biometrical Journal 50(3),  346--363."
    extrafont_ref <- "Winston Chang, (2014). extrafont: Tools for using fonts. R package version 0.17. http://CRAN.R-project.org/package=extrafont"


    cluster_ref <- "Maechler, M., Rousseeuw, P., Struyf, A., Hubert, M., Hornik, K.(2013).  cluster: Cluster Analysis Basics and Extensions. R package version 1.14.4."
    ggdendro_ref <- "Andrie de Vries and Brian D. Ripley (2013). ggdendro: Tools for extracting dendrogram and tree diagram plot data for use with ggplot.. R package version 0.1-14. http://CRAN.R-project.org/package=ggdendro"
    mixOmics_ref <- "Sebastien Dejean, Ignacio Gonzalez, Kim-Anh Le Cao with contributions from Pierre Monget, Jeff Coquery, FangZou Yao, Benoit Liquet and Florian Rohart (2013). mixOmics: Omics Data Integration Project. R package version 5.0-1. http://CRAN.R-project.org/package=mixOmics"

    BateClark_ref <- "Bate ST and Clark RA. (2014). The Design and Statistical Analysis of Animal Experiments. Cambridge University Press."

    Barnard_ref <- "Peter Calhoun (2013). Exact: Unconditional Exact Test. R package version 1.4. http://CRAN.R-project.org/package=Exact."


    Refs <- list(
            R_ref = R_ref,
            mtvnorm_ref = mtvnorm_ref,
            GridExtra_ref = GridExtra_ref,
            GGally_ref = GGally_ref,
            RColorBrewers_ref = RColorBrewers_ref,
            GGPLot2_ref = GGPLot2_ref,
            reshape_ref = reshape_ref,
            plyr_ref = plyr_ref,
            scales_ref = scales_ref,
            car_ref = car_ref,
            nlme_ref = nlme_ref,
            NPMC_ref = NPMC_ref,
            R2HTML_ref = R2HTML_ref,
            PROTO_ref = PROTO_ref,
            Contrast_ref = Contrast_ref,
            LSMEANS_ref = LSMEANS_ref,
            Survival_ref = Survival_ref,
            multcomp_ref = multcomp_ref,
            cluster_ref = cluster_ref,
            ggdendro_ref = ggdendro_ref,
            mixOmics_ref = mixOmics_ref,
            BateClark_ref = BateClark_ref,
            Barnard_ref = Barnard_ref,
            extrafont_ref = extrafont_ref
               )

    return(Refs)
}








