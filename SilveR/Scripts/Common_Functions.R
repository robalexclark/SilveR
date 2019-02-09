#Software branding
branding <- "InVivoStat"

#Display arguments
Diplayargs <- "Y"

#===================================================================================================================
#Import Graphical parameters from file
#===================================================================================================================
suppressWarnings(library(ggplot2))
suppressWarnings(library(ggrepel))
suppressWarnings(library(RColorBrewer))
suppressWarnings(library(plyr))
suppressWarnings(library(scales))
suppressWarnings(library(reshape))
suppressWarnings(library(GGally))
suppressWarnings(library(proto))
suppressWarnings(library(grid))
#suppressWarnings(library(extrafont))

#===================================================================================================================
#User option parameters
#===================================================================================================================
grparanum = read.table(paste(getwd(),"/UserOptions.txt", sep=""), skip = 22, sep=" ", nrows = 22)
grparatext = read.table(paste(getwd(),"/UserOptions.txt", sep=""))

#Line types used - solid, blank and dashed
	Line_type_solid<- paste(grparatext$V2[1],sep="")
	Line_type_dashed<- paste(grparatext$V2[2],sep="")

#Font for plots
	gr_font<- paste(grparatext$V2[3],sep="")

#Font style for plots
	gr_fontface<- paste(grparatext$V2[4],sep="")

#Font colour
	gr_textcolour<- paste(grparatext$V2[5],sep="")

#Colour fill colour
	Col_fill<- paste(grparatext$V2[6],sep="")
	BW_fill<- paste(grparatext$V2[7],sep="")

#Colour for the header bar on the seperate categorised plots
	Catbar_fill<- paste(grparatext$V2[8],sep="")

#Individual lines colour
	Col_line<- paste(grparatext$V2[9],sep="")
	BW_line<- paste(grparatext$V2[10],sep="")

#Legend parameters
	Legend_text_col<- paste(grparatext$V2[11],sep="")
	Legend_pos<- paste(grparatext$V2[12],sep="")

#Categorised plot set default
	Palette_set<- paste(grparatext$V2[13],sep="")

#Output dataset
#	showdataset <- paste(grparatext$V2[14],sep="")
showdataset <- "Y"
#pdf output
	pdfout<- paste(grparatext$V2[15],sep="")

#plot colour
	bandw<- paste(grparatext$V2[16],sep="")

#Display back-transformed geometric means
	GeomDisplay<- paste(grparatext$V2[17],sep="")

#Display covariate regression coefficients
	CovariateRegressionCoefficients<- paste(grparatext$V2[18],sep="")

#Display covariate interaction tests 
	AssessCovariateInteractions<- paste(grparatext$V2[19],sep="")

#Display labels on scatterplot (needs argument)
	scatterlabels <- paste(grparatext$V2[20],sep="")

#Disply lines on lsmeans
	DisplayLSMeanslines <- paste(grparatext$V2[21],sep="")

#Disply lines on lsmeans
	DisplaySEMlines <- paste(grparatext$V2[22],sep="")

#Font size for the main title
	Title_size  <- as.numeric(grparanum$V2[1])

#Y and X axis titles font size
	X_Title_size<- as.numeric(grparanum$V2[2])
	Y_Title_size<- as.numeric(grparanum$V2[3])

#Y and X axis titles font size
	X_Label_size<- as.numeric(grparanum$V2[4])
	Y_Label_size<- as.numeric(grparanum$V2[5])

#X-axis labels angle and horizontal adjustment
	Gr_x_angle<- as.numeric(grparanum$V2[6])
	Gr_x_hjust<- as.numeric(grparanum$V2[7])

#Y-axis labels angle and vertical adjustment
	Gr_y_angle<- as.numeric(grparanum$V2[8])
	Gr_y_vjust<- as.numeric(grparanum$V2[9])

#Size of points on plots
	Point_size<- as.numeric(grparanum$V2[10])

#Shape of points on plot
	Point_shape<- as.numeric(grparanum$V2[11])

#Size of the lines on the plots
	Line_size<- as.numeric(grparanum$V2[12])

#Legend text size
	legend_font_size  <- as.numeric(grparanum$V2[13])

#graphics sizes
	pdfwidth<- as.numeric(grparanum$V2[14])
	pdfheight<- as.numeric(grparanum$V2[15])
	jpegwidth<- grparanum$V2[16]
	jpegheight<- as.numeric(grparanum$V2[17])

# When using black and white range - these are the limits (0 = white and 1 = black)
	gr_bw_low<- as.numeric(grparanum$V2[18])
	gr_bw_high<- as.numeric(grparanum$V2[19])

#Horizontal jitter on scatterplot
	Gr_w_Gr_jit<- as.numeric(grparanum$V2[20])

#Vertical jitter on scatterplot
	Gr_h_Gr_jit<- as.numeric(grparanum$V2[21])

#Error bar width for LSMeans plots and line SEM plots
	ErrorBarWidth <- as.numeric(grparanum$V2[22])

#Width of bars for means with SEM column plots
	ErrorBarWidth2 <- ErrorBarWidth /2

#===================================================================================================================
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


#===================================================================================================================
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
		colour = gr_textcolour,
		hjust = 0.5),

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
		face = gr_fontface),

	legend.key = element_blank()
)


#===================================================================================================================
#FUNCTIONS TO REPLACE ILLEGAL CHARACTERS
#===================================================================================================================
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

#===================================================================================================================
#Graphical functions - shared code across modules
#===================================================================================================================

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

#===================================================================================================================
#===================================================================================================================
#Graphical GGPlot2 functions
#===================================================================================================================
#===================================================================================================================

#===================================================================================================================
#Scatterplots (Graphics, Correlation modules, resid plots, QQplots)
#===================================================================================================================

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
			scale_y_continuous(breaks = -20:20) +
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
	g7 <- g6 + geom_point(size = Point_size, shape = Point_shape, color = "black", fill = Gr_fill, position = position_jitter(w = w_Gr_jitscat, h = h_Gr_jitscat))

	if (scatterlabels == "Y") {
		g8 <- g7 + geom_text_repel(aes(label = rownames(graphdata)), size = 3.5,force = 10, point.padding=1)
	} else {
		g8 <- g7
	}
	if (typez == "paired") {
		g9 <- g8 + geom_abline(slope=1, intercept=0, lty = Gr_line_type, size = Line_size, colour = Gr_line) 
	} else {
		g9 <- g8
	}

	if (ReferenceLine != "NULL") {
		g10 <- g9 + geom_hline(yintercept = Gr_intercept, lty = Gr_line_typeint, size = Line_size, colour = Gr_line)
	} else {
		g10 <- g9
	}
	suppressWarnings(print(g10))
}


ONECATSEP_SCAT <- function() {
	g <- ggplot(graphdata, aes(x = xvarrr_IVS, y = yvarrr_IVS)) +
		theme_map +
		mytheme +
		ylab(YAxisTitle) +
		xlab(XAxisTitle) +
		ggtitle(MainTitle2) +
		geom_point(size = Point_size, colour = "black", shape = Point_shape, fill = Gr_fill, position = position_jitter(w = w_Gr_jitscat, h = h_Gr_jitscat)) +
		facet_wrap(~l_l)

	if (is.numeric(graphdata$xvarrr_IVS) == "TRUE" && is.numeric(graphdata$yvarrr_IVS) == "TRUE" && LinearFit == "Y") {
		g1 <- g + geom_smooth(graphdata = subset(graphdata, catvartest != "N"), method = "lm", se = FALSE, lty = Line_type, size = Line_size, color = alpha(Gr_line, Gr_alpha), fullrange = TRUE)
	} else {
		g1 <- g
	}

	if (scatterlabels == "Y") {
		g2 <- g1 + geom_text_repel(aes(label = rownames(graphdata)), size = 3.5, force = 10, point.padding=1)
	} else {
		g2 <- g1
	}

	if (ReferenceLine != "NULL") {
		g3 <- g2 + geom_hline(yintercept = Gr_intercept, lty = Gr_line_typeint, size = Line_size, colour = Gr_line)
	} else {
		g3 <- g2
	}
	suppressWarnings(print(g3))
}

TWOCATSEP_SCAT <- function() {
	g <- ggplot(graphdata, aes(xvarrr_IVS, yvarrr_IVS)) +
		theme_map +
		mytheme +
		ylab(YAxisTitle) +
		xlab(XAxisTitle) +
		ggtitle(MainTitle2) +
		geom_point(size = Point_size, colour = "black", shape = Point_shape, fill = Gr_fill, position = position_jitter(w = w_Gr_jitscat, h = h_Gr_jitscat)) +
		facet_grid(firstcatvarrr_IVS ~ secondcatvarrr_IVS, scale = scalexy)

	if (is.numeric(graphdata$xvarrr_IVS) == "TRUE" && is.numeric(graphdata$yvarrr_IVS) == "TRUE" && LinearFit == "Y") {
		g1 <- g + geom_smooth(graphdata = subset(graphdata, catvartest != "N"), method = "lm", se = FALSE, lty = Line_type, size = Line_size, color = alpha(Gr_line, Gr_alpha), fullrange = TRUE)
	} else {
		g1 <- g
	}

	if (scatterlabels == "Y") {
		g2 <- g1 + geom_text_repel(aes(label = rownames(graphdata)), size = 3.5, force = 10, point.padding=1)
	} else {
		g2 <- g1
	}
	if (ReferenceLine != "NULL") {
		g3 <- g2 + geom_hline(yintercept = Gr_intercept, lty = Gr_line_typeint, size = Line_size, colour = Gr_line)
	} else {
		g3 <- g2
	}
	suppressWarnings(print(g3))
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
		geom_point(graphdata = subset(graphdata, catvartest != "N"), aes(fill = l_l), colour = "black", size = Point_size, shape = Point_shape, position = position_jitter(w = w_Gr_jitscat, h = h_Gr_jitscat))

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

	if (scatterlabels == "Y") {
		g3 <- g2 + geom_text_repel(aes(label = rownames(graphdata), color = graphdata$l_l), size = 3.5,force = 10, point.padding=1, show.legend = FALSE)
	} else {
		g3 <- g2
	}
	if (ReferenceLine != "NULL") {
		g4 <- g3 + geom_hline(yintercept = Gr_intercept, lty = Gr_line_typeint, size = Line_size, colour = Gr_line)
	} else {
		g4 <- g3
	}
	suppressWarnings(print(g4))
}

#===================================================================================================================
#SEM plots (Graphics module)
#===================================================================================================================
NONCAT_SEM <- function() {
	g <- ggplot(graphdata, aes(x = xvarrr_IVS_SEM, y = yvarrr_IVS)) +
		theme_map +
		mytheme +
		ylab(YAxisTitle) +
		xlab(XAxisTitle) +
		ggtitle(MainTitle2)

	if (SEMPlotType == "Column") {
		g1 <- g + geom_errorbar(data = graphdata_SEM, aes(y = mean.y, ymax = mean.y + se.y, ymin = mean.y - se.y), width = ErrorBarWidth2) +
			geom_bar(data = graphdata_SEM, aes(y = mean.y, x = xvarrr_IVS_SEM), stat = "identity", fill = Gr_fill, colour = "black") +
			geom_hline(yintercept = 0)
	} else {
		g1 <- g + geom_errorbar(data = graphdata_SEM, aes(y = mean.y, ymax = mean.y + se.y, ymin = mean.y - se.y), width = ErrorBarWidth2) +
			geom_point(data = graphdata_SEM, aes(y = mean.y, x = xvarrr_IVS_SEM), fill = Gr_line, colour = "black", size = Point_size, shape = Point_shape)
	}

	if (DisplaySEMlines == "Y") {
		g2 <- g1 + stat_summary(fun.y = mean, geom = 'line', aes(group = 1), colour = Gr_line) 
	} else {
		g2 <- g1
	}
	
	if (displaypointSEM == "Y") {
		Point_size2 <- Point_size / 1.5
		g3 <- g2 + geom_point(size = Point_size2, shape = Point_shape, color = "black", fill = "black", position = position_jitter(w = w_Gr_jitSEM, h = h_Gr_jitSEM))
	} else {
		g3 <- g2
	}
	if (ReferenceLine != "NULL") {
		g4 <- g3 + geom_hline(yintercept = Gr_intercept, lty = Gr_line_typeint, size = Line_size, colour = Gr_line)
	} else {
		g4 <- g3
	}
	suppressWarnings(print(g4))
}


ONECATSEP_SEM <- function() {
	g <- ggplot(graphdata, aes(x = xvarrr_IVS_SEM, y = yvarrr_IVS)) +
		theme_map +
		mytheme +
		ylab(YAxisTitle) +
		xlab(XAxisTitle) +
		ggtitle(MainTitle2)

	if (SEMPlotType == "Column") {
		g1 <- g + geom_errorbar(data = graphdata_SEM, aes(y = mean.y, ymax = mean.y + se.y, ymin = mean.y - se.y), width = ErrorBarWidth2) +
			geom_bar(data = graphdata_SEM, aes(y = mean.y, x = xvarrr_IVS_SEM), stat = "identity", fill = Gr_fill, colour = "black", pos = 'dodge') +
			facet_wrap(~l_l) +
			geom_hline(yintercept = 0)
	} else {
		g1 <- g + geom_errorbar(data = graphdata_SEM, aes(y = mean.y, ymax = mean.y + se.y, ymin = mean.y - se.y), width = ErrorBarWidth2) +
			geom_point(data = graphdata_SEM, aes(y = mean.y, x = xvarrr_IVS_SEM), colour = "black", size = Point_size, shape = Point_shape, fill = Gr_line) +
			facet_wrap(~l_l)
	}
	if (DisplaySEMlines == "Y") {
		g2 <- g1 +stat_summary(fun.y = mean, geom = 'line', aes(group = 1), colour = Gr_line)
	} else {
		g2 <- g1
	}
	
	if (displaypointSEM == "Y") {
		Point_size2 <- Point_size / 1.5
		g3 <- g2 + geom_point(size = Point_size2, shape = Point_shape, color = "black", fill = "black", position = position_jitter(w = w_Gr_jitSEM, h = h_Gr_jitSEM), pos = 'dodge')
	} else {
		g3 <- g2
	}
	if (ReferenceLine != "NULL") {
		g4 <- g3 + geom_hline(yintercept = Gr_intercept, lty = Gr_line_typeint, size = Line_size, colour = Gr_line)
	} else {
		g4 <- g3
	}
	suppressWarnings(print(g4))
}


TWOCATSEP_SEM <- function() {
	g <- ggplot(graphdata, aes(x = xvarrr_IVS_SEM, y = yvarrr_IVS)) +
		theme_map +
		mytheme +
		ylab(YAxisTitle) +
		xlab(XAxisTitle) +
		ggtitle(MainTitle2)

	if (SEMPlotType == "Column") {
		g1 <- g + geom_errorbar(data = graphdata_SEM, aes(y = mean.y, ymax = mean.y + se.y, ymin = mean.y - se.y), width = ErrorBarWidth2) +
			geom_bar(data = graphdata_SEM, aes(y = mean.y, x = xvarrr_IVS_SEM), stat = "identity", fill = Gr_fill, colour = "black", pos = 'dodge') +
			facet_grid(firstcatvarrr_IVS ~ secondcatvarrr_IVS) +
			geom_hline(yintercept = 0)
	} else {
		g1 <- g + geom_errorbar(data = graphdata_SEM, aes(y = mean.y, ymax = mean.y + se.y, ymin = mean.y - se.y), width = ErrorBarWidth2) +
			geom_point(data = graphdata_SEM, aes(y = mean.y, x = xvarrr_IVS_SEM), colour = "black", size = Point_size, shape = Point_shape, fill = Gr_line) +
			facet_grid(firstcatvarrr_IVS ~ secondcatvarrr_IVS)
	}

	if (DisplaySEMlines == "Y") {
		g2 <- g1 +  stat_summary(fun.y = mean, geom = 'line', aes(group = 1), colour = Gr_line)
	} else {
		g2 <- g1
	}
	
	if (displaypointSEM == "Y") {
		Point_size2 <- Point_size / 1.5
		g3 <- g2 + geom_point(size = Point_size2, shape = Point_shape, color = "black", fill = "black", position = position_jitter(w = w_Gr_jitSEM, h = h_Gr_jitSEM))
	} else {
		g3 <- g2
	}
	if (ReferenceLine != "NULL") {
		g4 <- g3 + geom_hline(yintercept = Gr_intercept, lty = Gr_line_typeint, size = Line_size, colour = Gr_line)
	} else {
		g4 <- g3
	}
	suppressWarnings(print(g4))
}


OVERLAID_SEM <- function() {
	g <- ggplot(graphdataSEM_overall, aes(x = xvarrr_IVS_SEM, y = yvarrr_IVS, fill=l_l), colour = l_l ) +
		theme_map +
		mytheme +
		theme(legend.position = Gr_legend_pos) +
		ylab(YAxisTitle) +
		xlab(XAxisTitle) +
		ggtitle(MainTitle2) +
		scale_color_manual(values = Gr_palette) +
		scale_fill_manual(values = Gr_palette)

	if (SEMPlotType == "Column") {
		g1 <- g + geom_errorbar(data = graphdataSEM_means, aes(y = mean.y, ymax = mean.y + se.y, ymin = mean.y - se.y, group = l_l), width = ErrorBarWidth2, pos = position_dodge(w = 0.9), colour = "black") +
			geom_bar(data = graphdataSEM_means, aes(y = mean.y, x = xvarrr_IVS_SEM, group = l_l, fill = l_l), stat = "identity", colour = "black", pos = 'dodge') +
			geom_hline(yintercept = 0)
	} else {
		g1 <- g + geom_errorbar(data = graphdataSEM_means, aes(y = mean.y, ymax = mean.y + se.y, ymin = mean.y - se.y, group = l_l), width = ErrorBarWidth2, pos = position_dodge(w = 0.1), colour = "black") +
			geom_point(data = graphdataSEM_means, aes(y = mean.y, x = xvarrr_IVS_SEM, fill = l_l), colour = "black", size = Point_size, shape = Point_shape, pos = position_dodge(w = 0.1))
	}
	
	if (DisplaySEMlines == "Y") {
		g2 <- g1 +  stat_summary(data = graphdataSEM, fun.y = mean, geom = 'line', aes(group = l_l, colour = l_l), pos = position_dodge(w = 0.1))
	} else {
		g2 <- g1
	}
	
	if (displaypointSEM == "Y" && SEMPlotType == "Column") {
		Point_size2 <- Point_size / 1.5
#		g3 <- g2 + geom_point(data = graphdataSEM_overall, pos = position_dodge(w = 0.9), aes(group = l_l), size = Point_size2, shape = Point_shape, color = "black", fill = "black", position = position_jitter(w = w_Gr_jitSEM, h = h_Gr_jitSEM))
		g3 <- g2 + geom_point(data = graphdataSEM_overall, size = Point_size2, shape = Point_shape, position = position_jitterdodge(dodge.width = 1, jitter.width = w_Gr_jitSEM))
	}

	if (displaypointSEM == "Y" && SEMPlotType == "Line") {
		Point_size2 <- Point_size / 1.5
		g3 <- g2 + geom_point(data = graphdataSEM, pos = position_dodge(w = 0.1), aes(group = l_l), size = Point_size2, shape = Point_shape, color = "black", fill = "black", position = position_jitter(w = w_Gr_jitSEM, h = h_Gr_jitSEM), pos = position_dodge(w = 0.1))
	}

	if (displaypointSEM == "N") {
		g3 <- g2
	}
	if (ReferenceLine != "NULL") {
		g4 <- g3 + geom_hline(yintercept = Gr_intercept, lty = Gr_line_typeint, size = Line_size, colour = Gr_line)
	} else {
		g4 <- g3
	}
	suppressWarnings(print(g4))
}

#===================================================================================================================
#case profiles plot (Graphics module)
#===================================================================================================================
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
		g1 <- g + geom_hline(yintercept = Gr_intercept, lty = Gr_line_typeint, size = Line_size, colour = Gr_line)
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
		g1 <- g + geom_hline(yintercept = Gr_intercept, lty = Gr_line_typeint, size = Line_size, colour = Gr_line)
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
		g1 <- g + geom_hline(yintercept = Gr_intercept, lty = Gr_line_typeint, size = Line_size, colour = Gr_line)
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
		g1 <- g + geom_hline(yintercept = Gr_intercept, lty = Gr_line_typeint, size = Line_size, colour = Gr_line)
	} else {
		g1 <- g
	}
	suppressWarnings(print(g1))
}

#===================================================================================================================
#Box plot (Graphics module)
#===================================================================================================================
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
			geom_point(data = outlierdata, aes(x = xvarrr_IVS_BP, y = yvarrr_IVS), size = Point_size, shape = 8, color = "black", fill = Gr_fill)
	} else {
		g1 <- g + geom_boxplot(data = boxdata, aes(x = xvarrr_IVS_BP, ymin = minq, lower = lowerq, middle = medq, upper = upperq, ymax = maxq), stat = "identity", outlier.shape = NA, fill = Gr_fill)
	}

	if (displaypointBOX == "Y") {
		Point_size2 <- Point_size / 1.5
		g2 <- g1 + geom_point(data = graphdata, aes(x = xvarrr_IVS, y = yvarrr_IVS), size = Point_size2, shape = Point_shape, color = "black", fill = "black", position = position_jitter(w = w_Gr_jitBP, h = h_Gr_jitBP))
	} else {
		g2 <- g1
	}
	if (ReferenceLine != "NULL") {
		g3 <- g2 + geom_hline(yintercept = Gr_intercept, lty = Gr_line_typeint, size = Line_size, colour = Gr_line)
	} else {
		g3 <- g2
	}
	suppressWarnings(print(g3))
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
			geom_point(data = outlierdata, aes(x = xvarrr_IVS_BP, y = yvarrr_IVS), size = Point_size, shape = 8, color = "black", fill = Gr_fill)
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
		g2 <- g1 + geom_point(data = graphdata2xxx, aes(x = xvarrr_IVS_BP, y = yvarrr_IVS_BP), size = Point_size2, shape = Point_shape, color = "black", fill = "black", position = position_jitter(w = w_Gr_jitBP, h = h_Gr_jitBP))
	} else {
		g2 <- g1
	}
	if (ReferenceLine != "NULL") {
		g3 <- g2 + geom_hline(yintercept = Gr_intercept, lty = Gr_line_typeint, size = Line_size, colour = Gr_line)
	} else {
		g3 <- g2
	}
	suppressWarnings(print(g3))
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
			geom_point(data = outlierdata, aes(x = xvarrr_IVS_BP, y = yvarrr_IVS), size = Point_size, shape = 8, color = "black", fill = Gr_fill)
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
		g2 <- g1 + geom_point(data = graphdata2xxx, aes(x = xvarrr_IVS_BP, y = yvarrr_IVS_BP), size = Point_size2, shape = Point_shape, color = "black", fill = "black", position = position_jitter(w = w_Gr_jitBP, h = h_Gr_jitBP))
	} else {
		g2 <- g1
	}
	if (ReferenceLine != "NULL") {
		g3 <- g2 + geom_hline(yintercept = Gr_intercept, lty = Gr_line_typeint, size = Line_size, colour = Gr_line)
	} else {
		g3 <- g2
	}
	suppressWarnings(print(g3))
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
			geom_point(data = outlierdata, aes(x = xvarrr_IVS_BP, y = yvarrr_IVS, fill = l_l), size = Point_size, shape = 8, color = "black", position = position_dodge(width = 0.9))
		}
	} else {
		g1 <- g + geom_boxplot(data = boxdata, aes(x = xvarrr_IVS_BP, ymin = minq, lower = lowerq, middle = medq, upper = upperq, ymax = maxq, fill = l_l), stat = "identity", outlier.shape = NA)
	}
	
	if (displaypointBOX == "Y") {
		Point_size2 <- Point_size / 1.5
		g2 <- g1 + geom_point(data = graphdataBOX_overall, aes(x = xvarrr_IVS_BP, y = yvarrr_IVS, group = l_l, fill=l_l), size = Point_size2, shape = Point_shape, position = position_jitterdodge(dodge.width = 1, jitter.width = w_Gr_jitBP))
	} else {
		g2 <- g1
	}
	if (ReferenceLine != "NULL") {
		g3 <- g2 + geom_hline(yintercept = Gr_intercept, lty = Gr_line_typeint, size = Line_size, colour = Gr_line)
	} else {
		g3 <- g2
	}
	suppressWarnings(print(g3))
}

#===================================================================================================================
#Histograms plot (Graphics module)
#===================================================================================================================

NONCAT_HIS <- function() {
	g <- ggplot(graphdata, aes(x = yvarrr_IVS)) +
		theme_map +
		mytheme +
		ylab("Density") +
		xlab(YAxisTitle) +
		ggtitle(MainTitle2) +
		geom_histogram(aes(y = ..density..), colour = "black", fill = Gr_fill, center=(binrange/2), binwidth = (binrange)) +
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
		geom_histogram(aes(y = ..density..), colour = "black", fill = Gr_fill,  center=(binrange/2),  binwidth = (binrange)) +
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
		geom_histogram(aes(y = ..density..), colour = "black", fill = Gr_fill, center=(binrange/2),  binwidth = (binrange)) +
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
		geom_histogram(aes(y = ..density.., fill = l_l), position = 'dodge', colour = "black", center=(binrange/2),  binwidth = (binrange))

	if (NormalDistFit == "Y") {
		for (i in 1:nlevels) {
			gx = gx + stat_function(fun = dnorm, size = 1, color = Gr_palette[i], args = list(mean = mean(eval(parse(text = paste("histdata$Tzzz", i, sep = ""))), na.rm = TRUE), sd = sd(eval(parse(text = paste("histdata$Tzzz", i, sep = ""))), na.rm = TRUE)))
		}
	}
	suppressWarnings(print(gx))
}

#===================================================================================================================
#Matrix plot (correlation module)
#===================================================================================================================
NONCAT_MAT <- function() {
	g <- ggplot(graphdata, aes(xvarrr_IVS, yvarrr_IVS)) +
		theme_map +
		mytheme +
		ylab(YAxisTitle) +
		xlab(XAxisTitle) +
		ggtitle(MainTitle2) +
		geom_point(size = Point_size, colour = "black", shape = Point_shape, fill = Gr_fill, position = position_jitter(w = w_Gr_jitscat, h = h_Gr_jitscat)) +
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
		geom_point(graphdata = subset(graphdata, catvartest != "N"), aes(fill = l_li), colour = "black", size = Point_size, shape = Point_shape, position = position_jitter(w = w_Gr_jitscat, h = h_Gr_jitscat)) +
		facet_grid(firstcatvarrr_IVS ~ secondcatvarrr_IVS, scales = scalexy)

	if (dim(subset(graphdata, catvartest != "N"))[1] >= 1 && is.numeric(graphdata$xvarrr_IVS) == "TRUE" && is.numeric(graphdata$yvarrr_IVS) == "TRUE" && LinearFit == "Y") {
		g1 <- g + geom_smooth(graphdata = subset(graphdata, catvartest != "N"), aes(colour = l_li), method = "lm", se = FALSE, lty = Line_type, size = Line_size, fullrange = TRUE)
	} else {
		g1 <- g
	}
	suppressWarnings(print(g1))
}

#===================================================================================================================
#Dose response plots 
#===================================================================================================================
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

#===================================================================================================================
#LSMeans plot 
#===================================================================================================================
LSMPLOT_1 <- function() {
	g <- ggplot(graphdata, aes(x = Group_IVSq_, y = Mean)) +
		theme_map +
		mytheme +
		ylab(YAxisTitle) +
		xlab(XAxisTitle) +
		ggtitle(MainTitle2) +
		scale_y_continuous(breaks = pretty_breaks()) +
		geom_errorbar(aes(ymax = Upper, ymin = Lower), size = Line_size, colour = Gr_fill, width = ErrorBarWidth) +
		geom_point(size = Point_size, shape = Point_shape, color = "black", fill = Gr_fill) 

	if (DisplayLSMeanslines == "Y") {
		g1 <- g + stat_summary(fun.y = mean, geom = 'line', aes(group = 1), size = Line_size, colour = Gr_fill, lty = Line_type_solid) 
	} else {
		g1 <- g
	}
	suppressWarnings(print(g1))
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
		geom_point(aes(fill = jj_2), colour = "black", size = Point_size, shape = Point_shape, pos = position_dodge(w = 0.2))

	if (DisplayLSMeanslines == "Y") {
		g1 <- g + stat_summary(fun.y = mean, geom = 'line', aes(group = jj_2, colour = jj_2), size = Line_size, pos = position_dodge(w = 0.2), lty = Line_type_solid) 
	} else {
		g1 <- g
	}

	if (number == "three") {
		g2 <- g1 + facet_wrap(~jj_3)
	} else if (number == "four") {
		g2 <- g1 + facet_grid(jj_3 ~ jj_4)
	} else {
		g2 <- g1
	}
	suppressWarnings(print(g2))
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

#===================================================================================================================
#Power plot 
#===================================================================================================================

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
		scale_x_continuous(breaks = pretty_breaks()) +
		geom_line(aes(group = variable), lty = Line_type, size = Line_size, color = alpha(Gr_line, Gr_alpha))
	suppressWarnings(print(g))
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
		scale_colour_manual(name = namez, breaks = lin_list2, labels = userlistgr, values = Gr_palette_A) +
		geom_line(aes(group = variable, color = variable), size = Line_size)
	suppressWarnings(print(g))
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
		scale_colour_manual(name = "Absolute change: ", breaks = lin_list2, labels = expectedChanges, values = Gr_palette_P) +
		geom_line(aes(group = variable, color = variable), size = Line_size)
	suppressWarnings(print(g))
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
		scale_colour_manual(name = "Percent change: ", breaks = lin_list2, labels = expectedChanges2, values = Gr_palette_P) +
		geom_line(aes(group = variable, color = variable), size = Line_size)
#		 g1 <- g + geom_line(aes(group = variable, color = variable), size = Line_size)
	suppressWarnings(print(g))
}


POWERPLOT_ANOVA <- function(XAxisTitle, MainTitle2) {
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
		geom_line(aes(group = variable), lty = Line_type, size = Line_size, color = alpha(Gr_line, Gr_alpha))
	suppressWarnings(print(g))
}

#===================================================================================================================
#Survival plot 
#===================================================================================================================

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

#===================================================================================================================
#Multivariate plots 
#===================================================================================================================

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

#===================================================================================================================
#References
#===================================================================================================================

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
	NPMC_ref <- "Joerg Helms and Ullrich Munzel (2008). NPMC: Nonparametric Multiple Comparisons. R package version1.0-7."
	R2HTML_ref <- "Lecoutre, Eric (2003). The R2HTML Package. R News, Vol 3. N. 3, Vienna, Austria."
	PROTO_ref <- "Louis Kates and Thomas Petzoldt (2012). proto: Prototype object-based programming. R package version 0.3-10. http://CRAN.R-project.org/package=proto"
	Contrast_ref <- "Max Kuhn, contributions from Steve Weston, Jed Wing, James Forester and Thorn Thaler (2013). contrast: A collection of contrast methods. R package version 0.19. http://CRAN.R-project.org/package=contrast"
	LSMEANS_ref <- "Russell V. Lenth (2014). lsmeans: Least-Squares Means. R package version 2.00-1. http://CRAN.R-project.org/package=lsmeans"
	Survival_ref <- "Therneau T (2014). _A Package for Survival Analysis in S_. R package version 2.37-7, URL: http://CRAN.R-project.org/package=survival."
	multcomp_ref <- "Torsten Hothorn, Frank Bretz and Peter Westfall (2008). Simultaneous  Inference in General Parametric Models. Biometrical Journal 50(3),  346--363."
	extrafont_ref <- "Winston Chang, (2014). extrafont: Tools for using fonts. R package version 0.17. http://CRAN.R-project.org/package=extrafont"
	COIN_ref <- "Torsten Hothorn, Kurt Hornik, Mark A. van de Wiel, Achim Zeileis (2008). Implementing a Class of Permutation Tests: The coin Package. Journal of Statistical Software 28(8), 1-23. URL http://www.jstatsoft.org/v28/i08/."
	ggrepel_ref <- "Kamil Slowikowski (2018). ggrepel: Automatically Position Non-Overlapping Text Labels with 'ggplot2'. R package version 0.8.0. https://CRAN.R-project.org/package=ggrepel"
	mcview_ref<-  "Spencer Graves, Hans-Peter Piepho and Luciano Selzer with help from Sundar Dorai-Raj (2015). multcompView: Visualizations of Paired Comparisons. R package version 0.1-7. https://CRAN.R-project.org/package=multcompView"
	cluster_ref <- "Maechler, M., Rousseeuw, P., Struyf, A., Hubert, M., Hornik, K.(2013).  cluster: Cluster Analysis Basics and Extensions. R package version 1.14.4."
	ggdendro_ref <- "Andrie de Vries and Brian D. Ripley (2013). ggdendro: Tools for extracting dendrogram and tree diagram plot data for use with ggplot.. R package version 0.1-14. http://CRAN.R-project.org/package=ggdendro"
	mixOmics_ref <- "Sebastien Dejean, Ignacio Gonzalez, Kim-Anh Le Cao with contributions from Pierre Monget, Jeff Coquery, FangZou Yao, Benoit Liquet and Florian Rohart (2013). mixOmics: Omics Data Integration Project. R package version 5.0-1. http://CRAN.R-project.org/package=mixOmics"
	dplyr_ref <- "Hadley Wickham, Romain François, Lionel Henry and Kirill Müller (2018). dplyr: A Grammar of Data Manipulation. R package version 0.7.6. https://CRAN.R-project.org/package=dplyr"

	BateClark_ref <- "Bate ST and Clark RA. (2014). The Design and Statistical Analysis of Animal Experiments. Cambridge University Press."
	
	Barnard_ref <- "Peter Calhoun (2013). Exact: Unconditional Exact Test. R package version 1.4. http://CRAN.R-project.org/package=Exact."

	power_ref <- "Stephane Champely (2018). pwr: Basic Functions for Power Analysis. R package version 1.2-2. https://CRAN.R-project.org/package=pwr"


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
		extrafont_ref = extrafont_ref,
		COIN_ref=COIN_ref,
		ggrepel_ref = ggrepel_ref,
		mcview_ref = mcview_ref,
		power_ref = power_ref,
		dplyr_ref = dplyr_ref
	)
	return(Refs)
}