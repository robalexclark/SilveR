#Software branding
#branding <- "InVivoStat (beta version)"
branding <- "InVivoStat"
IVS_version<- 4.7 

#Software update
UpdateIVS <- "N"

#Beta warning
Betawarn <- "N"
BetaMessage <- "This output has been generated using the beta test version of InVivoStat. Care should be taken when making decisions based on the output."

#Display arguments
Diplayargs <- "N"

#Display pdf disabled
pdfout <- "N"

options(encoding = "UTF-8")

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
Args <- commandArgs(TRUE)
userOptions <- sub(".csv", ".useroptions", Args[3])

grparanum = read.table(userOptions, skip = 23, sep=" ")
grparatext = read.table(userOptions)

#Line types used - solid, blank and dashed
	Line_type_solid <- tolower(paste(grparatext$V2[1],sep=""))
	Line_type_dashed<- tolower(paste(grparatext$V2[2],sep=""))

#Font for plots
	gr_font<- tolower(paste(grparatext$V2[3],sep=""))

#Font style for plots
	gr_fontface<- tolower(paste(grparatext$V2[4],sep=""))

#Font colour
	gr_textcolour<- tolower(paste(grparatext$V2[5],sep=""))

#Colour fill colour
	Col_fill<- tolower(paste(grparatext$V2[6],sep=""))
	BW_fill<- tolower(paste(grparatext$V2[7],sep=""))

#Colour for the header bar on the seperate categorised plots
	Catbar_fill<- tolower(paste(grparatext$V2[8],sep=""))

#Individual lines colour
	Col_line<- tolower(paste(grparatext$V2[9],sep=""))
	BW_line<- tolower(paste(grparatext$V2[10],sep=""))

#Legend parameters
	Legend_text_col<- tolower(paste(grparatext$V2[11],sep=""))
	Legend_pos<- tolower(paste(grparatext$V2[12],sep=""))

#Categorised plot set default
	Palette_set<- tolower(paste(grparatext$V2[13],sep=""))

#Output dataset
	showdataset <- paste(grparatext$V2[14],sep="")

#Output options
	OutputAnalysisOps <- paste(grparatext$V2[15],sep="")

#plot colour
	bandw<- paste(grparatext$V2[16],sep="")

#Display back-transformed geometric means
	GeomDisplay<- tolower(paste(grparatext$V2[17],sep=""))

#Display model coefficients
	ShowCoeff<- paste(grparatext$V2[18],sep="")

#Display covariate regression coefficients
	CovariateRegressionCoefficients<- paste(grparatext$V2[19],sep="")

#Display covariate interaction tests 
	AssessCovariateInteractions<- paste(grparatext$V2[20],sep="")

#Disply lines on lsmeans
	DisplayLSMeanslines <- paste(grparatext$V2[21],sep="")

#Disply lines on lsmeans
	DisplaySEMlines <- paste(grparatext$V2[22],sep="")

#Disply point labels
	scatterlabels <- paste(grparatext$V2[23],sep="")


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
#	pdfwidth<- as.numeric(grparanum$V2[14])
	pdfwidth<-12
#	pdfheight<- as.numeric(grparanum$V2[15])
	pdfheight<-12

	jpegwidth<- grparanum$V2[14]
	jpegheight<- as.numeric(grparanum$V2[15])

#Plot resolution
	PlotResolution <- as.numeric(grparanum$V2[16])

# When using black and white range - these are the limits (0 = white and 1 = black)
	gr_bw_low<- as.numeric(grparanum$V2[17])
	gr_bw_high<- as.numeric(grparanum$V2[18])

#Horizontal jitter on scatterplot
	Gr_w_Gr_jit<- as.numeric(grparanum$V2[19])

#Vertical jitter on scatterplot
	Gr_h_Gr_jit<- as.numeric(grparanum$V2[20])

#Error bar width for LSMeans plots and line SEM plots
	ErrorBarWidth <- as.numeric(grparanum$V2[21])

#Width of bars for means with SEM column plots
	ErrorBarWidth2 <- ErrorBarWidth /2

#Fill Transparency
	FillTransparency <- as.numeric(grparanum$V2[22])
	if (FillTransparency > 1) {
		FillTransparency <- 1
	}
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

if (Legend_pos == "default") {
	Gr_legend_pos <- "right"
	Gr_legend_pos2 <- "bottom"
} else {
	Gr_legend_pos <- Legend_pos
	Gr_legend_pos2 <- Legend_pos
}


#===================================================================================================================
#Font options

if (gr_font == "courier") { gr_fontfamily <- "mono" }
if (gr_font == "helvetica") { gr_fontfamily <- "sans" }
if (gr_font == "times") { gr_fontfamily <- "serif" }
if (gr_font == "avantgarde_(pdf_only)") { gr_fontfamily <- "AvantGarde" }
if (gr_font == "urwpalladio_(pdf_only)") { gr_fontfamily <- "URWPalladio" }
if (gr_font == "centurysch_(pdf_only)") { gr_fontfamily <- "CenturySch" }
if (gr_font == "nimbussancond_(pdf_only)") { gr_fontfamily <- "NimbusSanCond" }
if (gr_font == "nimbussan_(pdf_only)") { gr_fontfamily <- "NimbusSan" }
if (gr_font == "nimbusmon_(pdf_only)") { gr_fontfamily <- "NimbusMon" }
if (gr_font == "urwbookman_(pdf_only)") { gr_fontfamily <- "URWBookman" }
if (gr_font == "urwbothic_(pdf_only)") { gr_fontfamily <- "URWGothic" }
if (gr_font == "palatino_(pdf_only)") { gr_fontfamily <- "Palatino" }
if (gr_font == "newcenturyschoolbook_(pdf_only)") { gr_fontfamily <- "NewCenturySchoolbook" }
if (gr_font == "helvetica-narrow_(pdf_only)") { gr_fontfamily <- "Helvetica-Narrow" }
if (gr_font == "bookman_(pdf_only)") { gr_fontfamily <- "Bookman" }
if (gr_font == "nimbusrom_(pdf_only)") { gr_fontfamily <- "NimbusRom" }



#===================================================================================================================
#Categorised colours options

if (Palette_set == "set1") {
	Palette_set<- "Set1"
} 
if (Palette_set == "set2") {
	Palette_set<- "Set2"
} 
if (Palette_set == "set3") {
	Palette_set<- "Set3"
}
if (Palette_set == "accent") {
	Palette_set<- "Accent"
}
if (Palette_set == "dark2") {
	Palette_set<- "Dark2"
}
if (Palette_set == "paired") {
	Palette_set<- "Paired"
}
if (Palette_set == "pastel1") {
	Palette_set<- "Pastel1"
}
if (Palette_set == "pastel2") {
	Palette_set<- "Pastel2"
}

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
	#for (i in 1:20) {
		axistitle <- gsub("ivs_questionmark_ivs", "?", axistitle)
		axistitle <- gsub("ivs_tilde_ivs", "~", axistitle)
		axistitle <- gsub("ivs_star_ivs", "*", axistitle)
		axistitle <- gsub("ivs_plus_ivs", "+", axistitle)
		axistitle <- gsub("ivs_sp_ivs", " ", axistitle)
		axistitle <- gsub("ivs_ob_ivs", "(", axistitle)
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
	#}

	return(axistitle)
}


namereplaceGSUB <- function(axistitle) {

	#for (i in 1:20) {
		axistitle <- gsub("ivs_questionmark_ivs", "?", axistitle)
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
 	#}
	 
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
		if (infiniteslope == "Y" && GraphStyle == "overlaid") {
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
		if (infiniteslope == "Y" && GraphStyle == "overlaid") {
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
	if (BoxplotOptions == "none") {
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


#Relabel the y-axis following transformation
axis_relabel <- function(tran, title) {
	if (tran == "log10") {
		title <- paste("log10(", title, ")", sep = "")
	}
	else if (tran == "loge") {
		title <- paste("loge(", title, ")", sep = "")
	}
	else if (tran == "square root") {
		title <- paste("sqrt(", title, ")", sep = "")
	}
	else if (tran == "arcsine") {
		title <- paste("arcsine(", title, ")", sep = "")
	}
	else if (tran == "rank") {
		title <- paste(title , " rank",  sep = "")
	}
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
			scale_x_continuous(breaks = pretty_breaks()) +
			theme(axis.text.x = element_text(hjust = 0.5, angle = 0), axis.text.y = element_text(hjust = 0.5, angle = 0))
	} else {
		g3 <- g2
	}

	if (typez == "COOK") {
		g4 <- g3 + geom_hline(yintercept = cutoff, lty = Gr_line_type, size = Line_size, colour = Gr_line) +
			theme(axis.text.x = element_text(hjust = 0.5, angle = 0), axis.text.y = element_text(hjust = 0.5, angle = 0))
	} else {
		g4 <- g3
	}

	if (typez == "QQPLOT") {
		g5 <- g4 + scale_x_continuous(breaks = pretty_breaks()) +
			scale_y_continuous(breaks = pretty_breaks()) +
			theme(axis.text.x = element_text(hjust = 0.5, angle = 0), axis.text.y = element_text(hjust = 0.5, angle = 0))
	} else {
		g5 <- g4
	}

	if (typez == "PCAPLOT") {
		g6 <- g5 + geom_text(size = 3, aes(label = obznames), hjust = 0, vjust = 1) +
			theme(axis.text.x = element_text(hjust = 0.5, angle = 0), axis.text.y = element_text(hjust = 0.5, angle = 0))
	} else {
		g6 <- g5
	}
	g7 <- g6 + geom_point(size = Point_size, shape = Point_shape, color = "black", fill = Gr_fill, position = position_jitter(w = w_Gr_jitscat, h = h_Gr_jitscat))

	if (scatterlabels == "Y") {
		g8 <- g7 + geom_text_repel(aes(label = rownames(graphdata)), size = 3.5,force = 10, point.padding=1)
	} else {
		g8 <- g7
	}
	if (typez == "PAIRED") {
		g9 <- g8 + geom_abline(slope=1, intercept=0, lty = Gr_line_type, size = Line_size, colour = Gr_line) +
			theme(axis.text.x = element_text(hjust = 0.5, angle = 0), axis.text.y = element_text(hjust = 0.5, angle = 0))
	} else {
		g9 <- g8
	}

	if (ReferenceLine != "NULL") {
		g10 <- g9 + geom_hline(yintercept = Gr_intercept, lty = Gr_line_typeint, size = Line_size, colour = Gr_line)
	} else {
		g10 <- g9
	}

#Only X range defined
	if (is.numeric(graphdata$xvarrr_IVS) == "TRUE" && XLimLow != "NULL" && XLimHigh != "NULL" && YLimLow == "NULL" && YLimHigh == "NULL") {
		g11 <- g10 + coord_cartesian(xlim=c(XLimLow_num, XLimHigh_num))
	} else {
		g11 <- g10
	}
#Only Y range defined
	if (is.numeric(graphdata$yvarrr_IVS) == "TRUE" &&  XLimLow == "NULL" && XLimHigh == "NULL" && YLimLow != "NULL" && YLimHigh != "NULL") {
		g12 <- g11 + coord_cartesian(ylim=c(YLimLow_num, YLimHigh_num))
	} else {
		g12 <- g11
	}

#X and Y defined, both numeric
	if (is.numeric(graphdata$xvarrr_IVS) == "TRUE" &&  is.numeric(graphdata$yvarrr_IVS) == "TRUE" &&  XLimLow != "NULL" && XLimHigh != "NULL" && YLimLow != "NULL" && YLimHigh != "NULL") {
		g13 <- g12 + coord_cartesian(xlim=c(XLimLow_num, XLimHigh_num), ylim=c(YLimLow_num, YLimHigh_num))
	} else {
		g13 <- g12
	}

#X and Y defined, X numeric
	if (is.numeric(graphdata$xvarrr_IVS) == "TRUE" &&  is.numeric(graphdata$yvarrr_IVS) != "TRUE" &&  XLimLow != "NULL" && XLimHigh != "NULL" && YLimLow != "NULL" && YLimHigh != "NULL") {
		g14 <- g13 + coord_cartesian(xlim=c(XLimLow_num, XLimHigh_num))
	} else {
		g14 <- g13
	}

#X and Y defined, Y numeric
	if (is.numeric(graphdata$xvarrr_IVS) != "TRUE" &&  is.numeric(graphdata$yvarrr_IVS) == "TRUE" &&  XLimLow != "NULL" && XLimHigh != "NULL" && YLimLow != "NULL" && YLimHigh != "NULL") {
		g15 <- g14 + coord_cartesian(ylim=c(YLimLow_num, YLimHigh_num))
	} else {
		g15 <- g14
	}
	suppressWarnings(print(g15))
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

	if (XLimLow != "NULL" && XLimHigh != "NULL" && YLimLow == "NULL" && YLimHigh == "NULL") {
		g4 <- g3 + coord_cartesian(xlim=c(XLimLow_num, XLimHigh_num))
	} else {
		g4 <- g3
	}

	if (XLimLow == "NULL" && XLimHigh == "NULL" && YLimLow != "NULL" && YLimHigh != "NULL") {
		g5 <- g4 + coord_cartesian(ylim=c(YLimLow_num, YLimHigh_num))
	} else {
		g5 <- g4
	}

#X and Y defined, both numeric
	if (is.numeric(graphdata$xvarrr_IVS) == "TRUE" &&  is.numeric(graphdata$yvarrr_IVS) == "TRUE" &&  XLimLow != "NULL" && XLimHigh != "NULL" && YLimLow != "NULL" && YLimHigh != "NULL") {
		g6 <- g5 + coord_cartesian(xlim=c(XLimLow_num, XLimHigh_num), ylim=c(YLimLow_num, YLimHigh_num))
	} else {
		g6 <- g5
	}

#X and Y defined, X numeric
	if (is.numeric(graphdata$xvarrr_IVS) == "TRUE" &&  is.numeric(graphdata$yvarrr_IVS) != "TRUE" &&  XLimLow != "NULL" && XLimHigh != "NULL" && YLimLow != "NULL" && YLimHigh != "NULL") {
		g7 <- g6 + coord_cartesian(xlim=c(XLimLow_num, XLimHigh_num))
	} else {
		g7 <- g6
	}

#X and Y defined, Y numeric
	if (is.numeric(graphdata$xvarrr_IVS) != "TRUE" &&  is.numeric(graphdata$yvarrr_IVS) == "TRUE" &&  XLimLow != "NULL" && XLimHigh != "NULL" && YLimLow != "NULL" && YLimHigh != "NULL") {
		g8 <- g7 + coord_cartesian(ylim=c(YLimLow_num, YLimHigh_num))
	} else {
		g8 <- g7
	}

	suppressWarnings(print(g8))
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
	if (XLimLow != "NULL" && XLimHigh != "NULL" && YLimLow == "NULL" && YLimHigh == "NULL") {
		g4 <- g3 + coord_cartesian(xlim=c(XLimLow_num, XLimHigh_num))
	} else {
		g4 <- g3
	}

	if (XLimLow == "NULL" && XLimHigh == "NULL" && YLimLow != "NULL" && YLimHigh != "NULL") {
		g5 <- g4 + coord_cartesian(ylim=c(YLimLow_num, YLimHigh_num))
	} else {
		g5 <- g4
	}

#X and Y defined, both numeric
	if (is.numeric(graphdata$xvarrr_IVS) == "TRUE" &&  is.numeric(graphdata$yvarrr_IVS) == "TRUE" &&  XLimLow != "NULL" && XLimHigh != "NULL" && YLimLow != "NULL" && YLimHigh != "NULL") {
		g6 <- g5 + coord_cartesian(xlim=c(XLimLow_num, XLimHigh_num), ylim=c(YLimLow_num, YLimHigh_num))
	} else {
		g6 <- g5
	}

#X and Y defined, X numeric
	if (is.numeric(graphdata$xvarrr_IVS) == "TRUE" &&  is.numeric(graphdata$yvarrr_IVS) != "TRUE" &&  XLimLow != "NULL" && XLimHigh != "NULL" && YLimLow != "NULL" && YLimHigh != "NULL") {
		g7 <- g6 + coord_cartesian(xlim=c(XLimLow_num, XLimHigh_num))
	} else {
		g7 <- g6
	}

#X and Y defined, Y numeric
	if (is.numeric(graphdata$xvarrr_IVS) != "TRUE" &&  is.numeric(graphdata$yvarrr_IVS) == "TRUE" &&  XLimLow != "NULL" && XLimHigh != "NULL" && YLimLow != "NULL" && YLimHigh != "NULL") {
		g8 <- g7 + coord_cartesian(ylim=c(YLimLow_num, YLimHigh_num))
	} else {
		g8 <- g7
	}
	suppressWarnings(print(g8))
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

	if (XLimLow != "NULL" && XLimHigh != "NULL" && YLimLow == "NULL" && YLimHigh == "NULL") {
		g5 <- g4 + coord_cartesian(xlim=c(XLimLow_num, XLimHigh_num))
	} else {
		g5 <- g4
	}

	if (XLimLow == "NULL" && XLimHigh == "NULL" && YLimLow != "NULL" && YLimHigh != "NULL") {
		g6 <- g5 + coord_cartesian(ylim=c(YLimLow_num, YLimHigh_num))
	} else {
		g6 <- g5
	}

#X and Y defined, both numeric
	if (is.numeric(graphdata$xvarrr_IVS) == "TRUE" &&  is.numeric(graphdata$yvarrr_IVS) == "TRUE" &&  XLimLow != "NULL" && XLimHigh != "NULL" && YLimLow != "NULL" && YLimHigh != "NULL") {
		g7 <- g6 + coord_cartesian(xlim=c(XLimLow_num, XLimHigh_num), ylim=c(YLimLow_num, YLimHigh_num))
	} else {
		g7 <- g6
	}

#X and Y defined, X numeric
	if (is.numeric(graphdata$xvarrr_IVS) == "TRUE" &&  is.numeric(graphdata$yvarrr_IVS) != "TRUE" &&  XLimLow != "NULL" && XLimHigh != "NULL" && YLimLow != "NULL" && YLimHigh != "NULL") {
		g8 <- g7 + coord_cartesian(xlim=c(XLimLow_num, XLimHigh_num))
	} else {
		g8 <- g7
	}

#X and Y defined, Y numeric
	if (is.numeric(graphdata$xvarrr_IVS) != "TRUE" &&  is.numeric(graphdata$yvarrr_IVS) == "TRUE" &&  XLimLow != "NULL" && XLimHigh != "NULL" && YLimLow != "NULL" && YLimHigh != "NULL") {
		g9 <- g8 + coord_cartesian(ylim=c(YLimLow_num, YLimHigh_num))
	} else {
		g9 <- g8
	}
	suppressWarnings(print(g9))
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

	if (SEMPlotType == "column") {
		if (FillTransparency == 1) {
			if (ErrorBarType == "sem") {
				g1 <- g + geom_errorbar(data = graphdata_SEM, aes(y = mean.y, ymax = sehigher, ymin = selower), width = ErrorBarWidth2) 
			} else {
				g1 <- g + geom_errorbar(data = graphdata_SEM, aes(y = mean.y, ymax = cihigher, ymin = cilower), width = ErrorBarWidth2) 
			}
			g2 <- g1 + geom_bar(data = graphdata_SEM, aes(y = mean.y, x = xvarrr_IVS_SEM), stat = "identity", fill = Gr_fill, colour = "black", alpha = FillTransparency) +
				 geom_hline(yintercept = 0)
		}
		if (FillTransparency < 1) {
			g1 <- g + geom_bar(data = graphdata_SEM, aes(y = mean.y, x = xvarrr_IVS_SEM), stat = "identity", fill = Gr_fill, colour = "black", alpha = FillTransparency) +
				geom_hline(yintercept = 0)
			if (ErrorBarType == "sem") {
				g2 <- g1 + geom_errorbar(data = graphdata_SEM, aes(y = mean.y, ymax = mean.y + se.y, ymin = mean.y - se.y), width = ErrorBarWidth2) 
			} else {
				g2 <- g1 + geom_errorbar(data = graphdata_SEM, aes(y = mean.y, ymax = mean.y + ci.y, ymin = mean.y - ci.y), width = ErrorBarWidth2) 
			}
		}
	} else {
		if (ErrorBarType == "sem") {
			g1 <- g + geom_errorbar(data = graphdata_SEM, aes(y = mean.y, ymax = mean.y + se.y, ymin = mean.y - se.y), width = ErrorBarWidth2) 
		} else {
			g1 <- g + geom_errorbar(data = graphdata_SEM, aes(y = mean.y, ymax = mean.y + ci.y, ymin = mean.y - ci.y), width = ErrorBarWidth2) 
		}
		g2 <- g1 + geom_point(data = graphdata_SEM, aes(y = mean.y, x = xvarrr_IVS_SEM), fill = Gr_line, colour = "black", size = Point_size, shape = Point_shape)
	}

	if (DisplaySEMlines == "Y" && SEMPlotType != "column" ) {
		g3 <- g2 + stat_summary(fun.y = mean, geom = 'line', aes(group = 1), colour = Gr_line) 
	} else {
		g3 <- g2
	}
	
	if (displaypointSEM == "Y") {
		Point_size2 <- Point_size / 1.5
		g4 <- g3 + geom_point(size = Point_size2, shape = Point_shape, color = "black", fill = "black", position = position_jitter(w = w_Gr_jitSEM, h = h_Gr_jitSEM))
	} else {
		g4 <- g3
	}
	if (ReferenceLine != "NULL") {
		g5 <- g4 + geom_hline(yintercept = Gr_intercept, lty = Gr_line_typeint, size = Line_size, colour = Gr_line)
	} else {
		g5 <- g4
	}

	if (YLimLow != "NULL" && YLimHigh != "NULL") {
		g6 <- g5 + coord_cartesian(ylim=c(YLimLow_num, YLimHigh_num))
	} else {
		g6 <- g5
	}
	suppressWarnings(print(g6))
}


ONECATSEP_SEM <- function() {
	g <- ggplot(graphdata, aes(x = xvarrr_IVS_SEM, y = yvarrr_IVS)) +
		theme_map +
		mytheme +
		ylab(YAxisTitle) +
		xlab(XAxisTitle) +
		ggtitle(MainTitle2)

	if (SEMPlotType == "column") {
		if (FillTransparency == 1) {
			if (ErrorBarType == "sem") {
				g1 <- g + geom_errorbar(data = graphdata_SEM, aes(y = mean.y, ymax = sehigher, ymin = selower), width = ErrorBarWidth2) 
			} else {
				g1 <- g + geom_errorbar(data = graphdata_SEM, aes(y = mean.y, ymax = cihigher, ymin = cilower), width = ErrorBarWidth2) 
			}
			g2 <- g1 + geom_bar(data = graphdata_SEM, aes(y = mean.y, x = xvarrr_IVS_SEM), stat = "identity", fill = Gr_fill, colour = "black", pos = 'dodge', alpha = FillTransparency) +
				facet_wrap(~l_l) +
				geom_hline(yintercept = 0)
		}
		if (FillTransparency < 1) {
			g1 <- g + geom_bar(data = graphdata_SEM, aes(y = mean.y, x = xvarrr_IVS_SEM), stat = "identity", fill = Gr_fill, colour = "black", pos = 'dodge', alpha = FillTransparency) +
				facet_wrap(~l_l) +
				geom_hline(yintercept = 0)
			if (ErrorBarType == "sem") {
				g2 <- g1 + geom_errorbar(data = graphdata_SEM, aes(y = mean.y, ymax = mean.y + se.y, ymin = mean.y - se.y), width = ErrorBarWidth2) 
			} else {
				g2 <- g1 + geom_errorbar(data = graphdata_SEM, aes(y = mean.y, ymax = mean.y + ci.y, ymin = mean.y - ci.y), width = ErrorBarWidth2) 
			}
		}

	} else {
		if (ErrorBarType == "sem") {
			g1 <- g + geom_errorbar(data = graphdata_SEM, aes(y = mean.y, ymax = mean.y + se.y, ymin = mean.y - se.y), width = ErrorBarWidth2) 
		} else {
			g1 <- g + geom_errorbar(data = graphdata_SEM, aes(y = mean.y, ymax = mean.y + ci.y, ymin = mean.y - ci.y), width = ErrorBarWidth2) 
		}
		g2 <- g1 + geom_point(data = graphdata_SEM, aes(y = mean.y, x = xvarrr_IVS_SEM), colour = "black", size = Point_size, shape = Point_shape, fill = Gr_line) +
			facet_wrap(~l_l)
	}
	if (DisplaySEMlines == "Y" && SEMPlotType != "column") {
		g3 <- g2 +stat_summary(fun.y = mean, geom = 'line', aes(group = 1), colour = Gr_line)
	} else {
		g3 <- g2
	}
	
	if (displaypointSEM == "Y") {
		Point_size2 <- Point_size / 1.5
		g4 <- g3 + geom_point(size = Point_size2, shape = Point_shape, color = "black", fill = "black", position = position_jitter(w = w_Gr_jitSEM, h = h_Gr_jitSEM), pos = 'dodge')
	} else {
		g4 <- g3
	}
	if (ReferenceLine != "NULL") {
		g5 <- g4 + geom_hline(yintercept = Gr_intercept, lty = Gr_line_typeint, size = Line_size, colour = Gr_line)
	} else {
		g5 <- g4
	}

	if (YLimLow != "NULL" && YLimHigh != "NULL") {
		g6 <- g5 + coord_cartesian(ylim=c(YLimLow_num, YLimHigh_num))
	} else {
		g6 <- g5
	}
	suppressWarnings(print(g6))
}


TWOCATSEP_SEM <- function() {
	g <- ggplot(graphdata, aes(x = xvarrr_IVS_SEM, y = yvarrr_IVS)) +
		theme_map +
		mytheme +
		ylab(YAxisTitle) +
		xlab(XAxisTitle) +
		ggtitle(MainTitle2)

	if (SEMPlotType == "column") {
		if (FillTransparency == 1) {
			if (ErrorBarType == "sem") {
				g1 <- g + geom_errorbar(data = graphdata_SEM, aes(y = mean.y, ymax = sehigher, ymin = selower), width = ErrorBarWidth2) 
			} else {
				g1 <- g + geom_errorbar(data = graphdata_SEM, aes(y = mean.y, ymax = cihigher, ymin = cilower), width = ErrorBarWidth2) 
			}
			g2 <- g1 + geom_bar(data = graphdata_SEM, aes(y = mean.y, x = xvarrr_IVS_SEM), stat = "identity", fill = Gr_fill, colour = "black", pos = 'dodge', alpha = FillTransparency) +
				facet_grid(firstcatvarrr_IVS ~ secondcatvarrr_IVS) +
				geom_hline(yintercept = 0)
		}
		if (FillTransparency < 1) {
			g1 <- g + geom_bar(data = graphdata_SEM, aes(y = mean.y, x = xvarrr_IVS_SEM), stat = "identity", fill = Gr_fill, colour = "black", pos = 'dodge', alpha = FillTransparency) +
				facet_grid(firstcatvarrr_IVS ~ secondcatvarrr_IVS) +
				geom_hline(yintercept = 0)
			if (ErrorBarType == "sem") {
				g2 <- g1 + geom_errorbar(data = graphdata_SEM, aes(y = mean.y, ymax = mean.y + se.y, ymin = mean.y - se.y), width = ErrorBarWidth2) 
			} else {
				g2 <- g1 + geom_errorbar(data = graphdata_SEM, aes(y = mean.y, ymax = mean.y + ci.y, ymin = mean.y - ci.y), width = ErrorBarWidth2) 
			}
		}
	} else {
		if (ErrorBarType == "sem") {
			g1 <- g + geom_errorbar(data = graphdata_SEM, aes(y = mean.y, ymax = mean.y + se.y, ymin = mean.y - se.y), width = ErrorBarWidth2) 
		} else {
			g1 <- g + geom_errorbar(data = graphdata_SEM, aes(y = mean.y, ymax = mean.y + ci.y, ymin = mean.y - ci.y), width = ErrorBarWidth2) 
		}
		g2 <- g1 + geom_point(data = graphdata_SEM, aes(y = mean.y, x = xvarrr_IVS_SEM), colour = "black", size = Point_size, shape = Point_shape, fill = Gr_line) +
			facet_grid(firstcatvarrr_IVS ~ secondcatvarrr_IVS)
	}

	if (DisplaySEMlines == "Y" && SEMPlotType != "column") {
		g3 <- g2 +  stat_summary(fun.y = mean, geom = 'line', aes(group = 1), colour = Gr_line)
	} else {
		g3 <- g2
	}
	
	if (displaypointSEM == "Y") {
		Point_size2 <- Point_size / 1.5
		g4 <- g3 + geom_point(size = Point_size2, shape = Point_shape, color = "black", fill = "black", position = position_jitter(w = w_Gr_jitSEM, h = h_Gr_jitSEM))
	} else {
		g4 <- g3
	}
	if (ReferenceLine != "NULL") {
		g5 <- g4 + geom_hline(yintercept = Gr_intercept, lty = Gr_line_typeint, size = Line_size, colour = Gr_line)
	} else {
		g5 <- g4
	}

	if (YLimLow != "NULL" && YLimHigh != "NULL") {
		g6 <- g5 + coord_cartesian(ylim=c(YLimLow_num, YLimHigh_num))
	} else {
		g6 <- g5
	}
	suppressWarnings(print(g6))
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

	if (SEMPlotType == "column") {
		if (FillTransparency == 1) {
			if (ErrorBarType == "sem") {
				g1 <- g + geom_errorbar(data = graphdataSEM_means, aes(y = mean.y, ymax = sehigher, ymin = selower, group = l_l), width = ErrorBarWidth2, pos = position_dodge(w = 0.9), colour = "black") 
			} else {
				g1 <- g + geom_errorbar(data = graphdataSEM_means, aes(y = mean.y, ymax = cihigher, ymin = cilower, group = l_l), width = ErrorBarWidth2, pos = position_dodge(w = 0.9), colour = "black") 
			}
			g2 <- g1 + geom_bar(data = graphdataSEM_means, aes(y = mean.y, x = xvarrr_IVS_SEM, group = l_l, fill = l_l), stat = "identity", colour = "black", pos = 'dodge', alpha = FillTransparency) +
				geom_hline(yintercept = 0)
		}
		if (FillTransparency < 1) {
			g1 <- g + geom_bar(data = graphdataSEM_means, aes(y = mean.y, x = xvarrr_IVS_SEM, group = l_l, fill = l_l), stat = "identity", colour = "black", pos = 'dodge', alpha = FillTransparency) +
				geom_hline(yintercept = 0)
			if (ErrorBarType == "sem") {
				g2 <- g1 + geom_errorbar(data = graphdataSEM_means, aes(y = mean.y, ymax = mean.y + se.y, ymin = mean.y - se.y, group = l_l), width = ErrorBarWidth2, pos = position_dodge(w = 0.9), colour = "black") 
			} else {
				g2 <- g1 + geom_errorbar(data = graphdataSEM_means, aes(y = mean.y, ymax = mean.y + ci.y, ymin = mean.y - ci.y, group = l_l), width = ErrorBarWidth2, pos = position_dodge(w = 0.9), colour = "black") 
			}
		}
	} else {
		if (ErrorBarType == "sem") {
			g1 <- g + geom_errorbar(data = graphdataSEM_means, aes(y = mean.y, ymax = mean.y + se.y, ymin = mean.y - se.y, group = l_l), width = ErrorBarWidth2, pos = position_dodge(w = 0.1), colour = "black") 
		} else {
			g1 <- g + geom_errorbar(data = graphdataSEM_means, aes(y = mean.y, ymax = mean.y + ci.y, ymin = mean.y - ci.y, group = l_l), width = ErrorBarWidth2, pos = position_dodge(w = 0.1), colour = "black") 
		}
		g2 <- g1 + geom_point(data = graphdataSEM_means, aes(y = mean.y, x = xvarrr_IVS_SEM, fill = l_l), colour = "black", size = Point_size, shape = Point_shape, pos = position_dodge(w = 0.1))
	}
	
	if (DisplaySEMlines == "Y" && SEMPlotType != "column") {
		g3 <- g2 +  stat_summary(data = graphdataSEM, fun.y = mean, geom = 'line', aes(group = l_l, colour = l_l), pos = position_dodge(w = 0.1))
	} else {
		g3 <- g2
	}
	
	if (displaypointSEM == "Y" && SEMPlotType == "column") {
		Point_size2 <- Point_size / 1.5
#		g4 <- g3 + geom_point(data = graphdataSEM_overall, pos = position_dodge(w = 0.9), aes(group = l_l), size = Point_size2, shape = Point_shape, color = "black", fill = "black", position = position_jitter(w = w_Gr_jitSEM, h = h_Gr_jitSEM))
		g4 <- g3 + geom_point(data = graphdataSEM_overall, size = Point_size2, shape = Point_shape, position = position_jitterdodge(dodge.width = 1, jitter.width = w_Gr_jitSEM))
	}

	if (displaypointSEM == "Y" && SEMPlotType == "line") {
		Point_size2 <- Point_size / 1.5
		g4 <- g3 + geom_point(data = graphdataSEM, pos = position_dodge(w = 0.1), aes(group = l_l), size = Point_size2, shape = Point_shape, color = "black", fill = "black", position = position_jitter(w = w_Gr_jitSEM, h = h_Gr_jitSEM), pos = position_dodge(w = 0.1))
	}

	if (displaypointSEM == "N") {
		g4 <- g3
	}
	if (ReferenceLine != "NULL") {
		g5 <- g4 + geom_hline(yintercept = Gr_intercept, lty = Gr_line_typeint, size = Line_size, colour = Gr_line)
	} else {
		g5 <- g4
	}

	if (YLimLow != "NULL" && YLimHigh != "NULL") {
		g6 <- g5 + coord_cartesian(ylim=c(YLimLow_num, YLimHigh_num))
	} else {
		g6 <- g5
	}
	suppressWarnings(print(g6))
}

#===================================================================================================================
#case profiles plot (Graphics module)
#===================================================================================================================
NONCAT_CPP <- function() {
	g <- ggplot(graphdata, aes(Time_IVS, yvarrr_IVS)) +
		theme_map +
		mytheme +
		theme(legend.position = Gr_legend_pos) +
		ylab(YAxisTitle) +
		xlab(XAxisTitle) +
		ggtitle(MainTitle2) +
		scale_color_manual(values = Gr_palette_A) +
		scale_fill_manual(values = Gr_palette_A) +
		geom_point(aes(colour = Animal_IVS ), size = 3, shape = 16) +
		geom_line(aes(group = Animal_IVS, color = Animal_IVS), size = Line_size)

	if (ShowCaseIDsInLegend=="N") {
		g1<- g + theme(legend.position = "none") 
	} else {
		g1<-g
	}

	if (ReferenceLine != "NULL") {
		g2 <- g1 + geom_hline(yintercept = Gr_intercept, lty = Gr_line_typeint, size = Line_size, colour = Gr_line)
	} else {
		g2 <- g1
	}

	if (XLimLow != "NULL" && XLimHigh != "NULL" && YLimLow == "NULL" && YLimHigh == "NULL") {
		g3 <- g2 + coord_cartesian(xlim=c(XLimLow_num, XLimHigh_num))
	} else {
		g3 <- g2
	}
	if (XLimLow == "NULL" && XLimHigh == "NULL" && YLimLow != "NULL" && YLimHigh != "NULL") {
		g4 <- g3 + coord_cartesian(ylim=c(YLimLow_num, YLimHigh_num))
	} else {
		g4 <- g3
	}
#X and Y defined, both numeric
	if (is.numeric(graphdata$xvarrr_IVS) == "TRUE" &&  is.numeric(graphdata$yvarrr_IVS) == "TRUE" &&  XLimLow != "NULL" && XLimHigh != "NULL" && YLimLow != "NULL" && YLimHigh != "NULL") {
		g5 <- g4 + coord_cartesian(xlim=c(XLimLow_num, XLimHigh_num), ylim=c(YLimLow_num, YLimHigh_num))
	} else {
		g5 <- g4
	}

#X and Y defined, X numeric
	if (is.numeric(graphdata$xvarrr_IVS) == "TRUE" &&  is.numeric(graphdata$yvarrr_IVS) != "TRUE" &&  XLimLow != "NULL" && XLimHigh != "NULL" && YLimLow != "NULL" && YLimHigh != "NULL") {
		g6 <- g5 + coord_cartesian(xlim=c(XLimLow_num, XLimHigh_num))
	} else {
		g6 <- g5
	}

#X and Y defined, Y numeric
	if (is.numeric(graphdata$xvarrr_IVS) != "TRUE" &&  is.numeric(graphdata$yvarrr_IVS) == "TRUE" &&  XLimLow != "NULL" && XLimHigh != "NULL" && YLimLow != "NULL" && YLimHigh != "NULL") {
		g7 <- g6 + coord_cartesian(ylim=c(YLimLow_num, YLimHigh_num))
	} else {
		g7 <- g6
	}
	suppressWarnings(print(g7))
}


ONECATSEP_CPP <- function() {
	g <- ggplot(graphdata, aes(Time_IVS, yvarrr_IVS)) +
		theme_map +
		mytheme +
		theme(legend.position = Gr_legend_pos) +
		ylab(YAxisTitle) +
		xlab(XAxisTitle) +
		ggtitle(MainTitle2) +
		scale_color_manual(values = Gr_palette_A) +
		geom_line(aes(group = Animal_IVS, color = Animal_IVS), size = Line_size) +
		geom_point(aes(colour = Animal_IVS), size = 3, shape = 16) +
		facet_wrap(~l_l) 

	if (ShowCaseIDsInLegend=="N") {
		g1<- g + theme(legend.position = "none") 
	} else {
		g1<-g
	}

	if (ReferenceLine != "NULL") {
		g2 <- g1 + geom_hline(yintercept = Gr_intercept, lty = Gr_line_typeint, size = Line_size, colour = Gr_line)
	} else {
		g2 <- g1
	}

	if (XLimLow != "NULL" && XLimHigh != "NULL" && YLimLow == "NULL" && YLimHigh == "NULL") {
		g3 <- g2 + coord_cartesian(xlim=c(XLimLow_num, XLimHigh_num))
	} else {
		g3 <- g2
	}
	if (XLimLow == "NULL" && XLimHigh == "NULL" && YLimLow != "NULL" && YLimHigh != "NULL") {
		g4 <- g3 + coord_cartesian(ylim=c(YLimLow_num, YLimHigh_num))
	} else {
		g4 <- g3
	}
#X and Y defined, both numeric
	if (is.numeric(graphdata$xvarrr_IVS) == "TRUE" &&  is.numeric(graphdata$yvarrr_IVS) == "TRUE" &&  XLimLow != "NULL" && XLimHigh != "NULL" && YLimLow != "NULL" && YLimHigh != "NULL") {
		g5 <- g4 + coord_cartesian(xlim=c(XLimLow_num, XLimHigh_num), ylim=c(YLimLow_num, YLimHigh_num))
	} else {
		g5 <- g4
	}

#X and Y defined, X numeric
	if (is.numeric(graphdata$xvarrr_IVS) == "TRUE" &&  is.numeric(graphdata$yvarrr_IVS) != "TRUE" &&  XLimLow != "NULL" && XLimHigh != "NULL" && YLimLow != "NULL" && YLimHigh != "NULL") {
		g6 <- g5 + coord_cartesian(xlim=c(XLimLow_num, XLimHigh_num))
	} else {
		g6 <- g5
	}

#X and Y defined, Y numeric
	if (is.numeric(graphdata$xvarrr_IVS) != "TRUE" &&  is.numeric(graphdata$yvarrr_IVS) == "TRUE" &&  XLimLow != "NULL" && XLimHigh != "NULL" && YLimLow != "NULL" && YLimHigh != "NULL") {
		g7 <- g6 + coord_cartesian(ylim=c(YLimLow_num, YLimHigh_num))
	} else {
		g7 <- g6
	}
	suppressWarnings(print(g7))
}


TWOCATSEP_CPP <- function() {
	g <- ggplot(graphdata, aes(Time_IVS, yvarrr_IVS)) +
		theme_map +
		mytheme +
		theme(legend.position = Gr_legend_pos) +
		ylab(YAxisTitle) +
		xlab(XAxisTitle) +
		ggtitle(MainTitle2) +
		scale_color_manual(values = Gr_palette_A) +
		geom_line(aes(group = Animal_IVS, color = Animal_IVS), size = Line_size) +
		geom_point(aes(colour = Animal_IVS), size = 3, shape = 16) +
		facet_grid(firstcatvarrr_IVS ~ secondcatvarrr_IVS)

	if (ShowCaseIDsInLegend=="N") {
		g1<- g + theme(legend.position = "none") 
	} else {
		g1<-g
	}

	if (ReferenceLine != "NULL") {
		g2 <- g1+ geom_hline(yintercept = Gr_intercept, lty = Gr_line_typeint, size = Line_size, colour = Gr_line)
	} else {
		g2 <- g1
	}

	if (XLimLow != "NULL" && XLimHigh != "NULL" && YLimLow == "NULL" && YLimHigh == "NULL") {
		g3 <- g2 + coord_cartesian(xlim=c(XLimLow_num, XLimHigh_num))
	} else {
		g3 <- g2
	}
	if (XLimLow == "NULL" && XLimHigh == "NULL" && YLimLow != "NULL" && YLimHigh != "NULL") {
		g4 <- g3 + coord_cartesian(ylim=c(YLimLow_num, YLimHigh_num))
	} else {
		g4 <- g3
	}
#X and Y defined, both numeric
	if (is.numeric(graphdata$xvarrr_IVS) == "TRUE" &&  is.numeric(graphdata$yvarrr_IVS) == "TRUE" &&  XLimLow != "NULL" && XLimHigh != "NULL" && YLimLow != "NULL" && YLimHigh != "NULL") {
		g5 <- g4 + coord_cartesian(xlim=c(XLimLow_num, XLimHigh_num), ylim=c(YLimLow_num, YLimHigh_num))
	} else {
		g5 <- g4
	}

#X and Y defined, X numeric
	if (is.numeric(graphdata$xvarrr_IVS) == "TRUE" &&  is.numeric(graphdata$yvarrr_IVS) != "TRUE" &&  XLimLow != "NULL" && XLimHigh != "NULL" && YLimLow != "NULL" && YLimHigh != "NULL") {
		g6 <- g5 + coord_cartesian(xlim=c(XLimLow_num, XLimHigh_num))
	} else {
		g6 <- g5
	}

#X and Y defined, Y numeric
	if (is.numeric(graphdata$xvarrr_IVS) != "TRUE" &&  is.numeric(graphdata$yvarrr_IVS) == "TRUE" &&  XLimLow != "NULL" && XLimHigh != "NULL" && YLimLow != "NULL" && YLimHigh != "NULL") {
		g7 <- g6 + coord_cartesian(ylim=c(YLimLow_num, YLimHigh_num))
	} else {
		g7 <- g6
	}
	suppressWarnings(print(g7))
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

	if (XLimLow != "NULL" && XLimHigh != "NULL" && YLimLow == "NULL" && YLimHigh == "NULL") {
		g2 <- g1 + coord_cartesian(xlim=c(XLimLow_num, XLimHigh_num))
	} else {
		g2 <- g1
	}
	if (XLimLow == "NULL" && XLimHigh == "NULL" && YLimLow != "NULL" && YLimHigh != "NULL") {
		g3 <- g2 + coord_cartesian(ylim=c(YLimLow_num, YLimHigh_num))
	} else {
		g3 <- g2
	}
#X and Y defined, both numeric
	if (is.numeric(graphdata$xvarrr_IVS) == "TRUE" &&  is.numeric(graphdata$yvarrr_IVS) == "TRUE" &&  XLimLow != "NULL" && XLimHigh != "NULL" && YLimLow != "NULL" && YLimHigh != "NULL") {
		g4 <- g3 + coord_cartesian(xlim=c(XLimLow_num, XLimHigh_num), ylim=c(YLimLow_num, YLimHigh_num))
	} else {
		g4 <- g3
	}

#X and Y defined, X numeric
	if (is.numeric(graphdata$xvarrr_IVS) == "TRUE" &&  is.numeric(graphdata$yvarrr_IVS) != "TRUE" &&  XLimLow != "NULL" && XLimHigh != "NULL" && YLimLow != "NULL" && YLimHigh != "NULL") {
		g5 <- g4 + coord_cartesian(xlim=c(XLimLow_num, XLimHigh_num))
	} else {
		g5 <- g4
	}

#X and Y defined, Y numeric
	if (is.numeric(graphdata$xvarrr_IVS) != "TRUE" &&  is.numeric(graphdata$yvarrr_IVS) == "TRUE" &&  XLimLow != "NULL" && XLimHigh != "NULL" && YLimLow != "NULL" && YLimHigh != "NULL") {
		g6 <- g5 + coord_cartesian(ylim=c(YLimLow_num, YLimHigh_num))
	} else {
		g6 <- g5
	}
	suppressWarnings(print(g6))
}

#AUC case profiles plot - addition of extra area call
AUC_CPP <- function() {
	g <- ggplot(graphdata, aes(Time_IVS, yvarrr_IVS)) +
		theme_map +
		mytheme +
		ylab(YAxisTitle) +
		xlab(XAxisTitle) +
		ggtitle(MainTitle2) +
		scale_color_manual(values = Gr_palette_A) +
		geom_line(aes(group = Animal_IVS, color = Animal_IVS), size = Line_size) +
		geom_point(aes(colour = Animal_IVS), size = 3, shape = 16) +
		facet_wrap(~l_l) +
		geom_area(aes(fill = Gr_fill, alpha = FillTransparency)) +
		theme(legend.position = "none") 
	suppressWarnings(print(g))
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

	if (BoxplotOptions == "outliers") {
		g1 <- g + geom_boxplot(data = boxdata, aes(x = xvarrr_IVS_BP, ymin = minq, lower = lowerq, middle = medq, upper = upperq, ymax = maxq), stat = "identity", fill = Gr_fill, alpha = FillTransparency) +
			geom_point(data = outlierdata, aes(x = xvarrr_IVS_BP, y = yvarrr_IVS), size = Point_size, shape = 8, color = "black", fill = Gr_fill)
	} else {
		g1 <- g + geom_boxplot(data = boxdata, aes(x = xvarrr_IVS_BP, ymin = minq, lower = lowerq, middle = medq, upper = upperq, ymax = maxq), stat = "identity", outlier.shape = NA, fill = Gr_fill, alpha = FillTransparency)
	}

	if (BoxplotOptions == "include data") {
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

	if (YLimLow != "NULL" && YLimHigh != "NULL") {
		g4 <- g3 + coord_cartesian(ylim=c(YLimLow_num, YLimHigh_num))
	} else {
		g4 <- g3
	}
	suppressWarnings(print(g4))
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

	if (BoxplotOptions == "outliers") {
		g1 <- g + geom_boxplot(data = boxdata, aes(x = xvarrr_IVS_BP, ymin = minq, lower = lowerq, middle = medq, upper = upperq, ymax = maxq), stat = "identity", fill = Gr_fill, alpha = FillTransparency) +
			geom_point(data = outlierdata, aes(x = xvarrr_IVS_BP, y = yvarrr_IVS), size = Point_size, shape = 8, color = "black", fill = Gr_fill)
	} else {
		g1 <- g + geom_boxplot(data = boxdata, aes(x = xvarrr_IVS_BP, ymin = minq, lower = lowerq, middle = medq, upper = upperq, ymax = maxq), stat = "identity", outlier.shape = NA, fill = Gr_fill, alpha = FillTransparency)
	}

	if (BoxplotOptions == "include data") {
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

	if (YLimLow != "NULL" && YLimHigh != "NULL") {
		g4 <- g3 + coord_cartesian(ylim=c(YLimLow_num, YLimHigh_num))
	} else {
		g4 <- g3
	}
	suppressWarnings(print(g4))
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

	if (BoxplotOptions == "outliers") {
		g1 <- g + geom_boxplot(data = boxdata, aes(x = xvarrr_IVS_BP, ymin = minq, lower = lowerq, middle = medq, upper = upperq, ymax = maxq), stat = "identity", fill = Gr_fill, alpha = FillTransparency) +
			geom_point(data = outlierdata, aes(x = xvarrr_IVS_BP, y = yvarrr_IVS), size = Point_size, shape = 8, color = "black", fill = Gr_fill)
	} else {
		g1 <- g + geom_boxplot(data = boxdata, aes(x = xvarrr_IVS_BP, ymin = minq, lower = lowerq, middle = medq, upper = upperq, ymax = maxq), stat = "identity", outlier.shape = NA, fill = Gr_fill, alpha = FillTransparency)
	}

	if (BoxplotOptions == "include data") {
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

	if (YLimLow != "NULL" && YLimHigh != "NULL") {
		g4 <- g3 + coord_cartesian(ylim=c(YLimLow_num, YLimHigh_num))
	} else {
		g4 <- g3
	}
	suppressWarnings(print(g4))
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

	if (BoxplotOptions == "outliers") {
		g1 <- g + geom_boxplot(data = boxdata, aes(x = xvarrr_IVS_BP, ymin = minq, lower = lowerq, middle = medq, upper = upperq, ymax = maxq, fill = l_l), stat = "identity", alpha = FillTransparency) +

		if (outliertest == "Y") {
			geom_point(data = outlierdata, aes(x = xvarrr_IVS_BP, y = yvarrr_IVS, fill = l_l), size = Point_size, shape = 8, color = "black", position = position_dodge(width = 0.9), alpha = FillTransparency)
		}
	} else {
		g1 <- g + geom_boxplot(data = boxdata, aes(x = xvarrr_IVS_BP, ymin = minq, lower = lowerq, middle = medq, upper = upperq, ymax = maxq, fill = l_l), stat = "identity", outlier.shape = NA, alpha = FillTransparency)
	}
	
	if (BoxplotOptions == "include data") {
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

	if (YLimLow != "NULL" && YLimHigh != "NULL") {
		g4 <- g3 + coord_cartesian(ylim=c(YLimLow_num, YLimHigh_num))
	} else {
		g4 <- g3
	}
	suppressWarnings(print(g4))
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
		geom_histogram(aes(y = ..density..), colour = "black", fill = Gr_fill, center=(binrange/2), binwidth = (binrange), alpha = FillTransparency) +
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
		geom_histogram(aes(y = ..density..), colour = "black", fill = Gr_fill,  center=(binrange/2),  binwidth = (binrange), alpha = FillTransparency) +
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
		geom_histogram(aes(y = ..density..), colour = "black", fill = Gr_fill, center=(binrange/2),  binwidth = (binrange), alpha = FillTransparency) +
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
		geom_histogram(aes(y = ..density.., fill = l_l), position = 'dodge', colour = "black", center=(binrange/2),  binwidth = (binrange), alpha = FillTransparency)

	if (NormalDistFit == "Y") {
		for (i in 1:nlevels) {
			gx = gx + stat_function(fun = dnorm, size = 1, color = Gr_palette[i], args = list(mean = mean(eval(parse(text = paste("histdata$Tzzz", i, sep = ""))), na.rm = TRUE), sd = sd(eval(parse(text = paste("histdata$Tzzz", i, sep = ""))), na.rm = TRUE)))
		}
	}
	suppressWarnings(print(gx))
}


#===================================================================================================================
#QQ Plot (Summary Stats, SMPA, RMPA, etc.)
#===================================================================================================================

NONCAT_QQPLOT <- function(typez) {
	g <- ggplot(graphdata, aes(sample = yvarrr_IVS)) +
		theme_map +
		mytheme +
		ylab(YAxisTitle) +
		xlab(XAxisTitle) +
		ggtitle(MainTitle2) +
                scale_x_continuous(breaks = pretty_breaks()) +
		scale_y_continuous(breaks = pretty_breaks()) +
		theme(axis.text.x = element_text(hjust = 0.5, angle = 0), axis.text.y = element_text(hjust = 0.5, angle = 0)) +
		stat_qq(size = Point_size, shape = Point_shape, color = "black", fill = Gr_fill) +
		stat_qq_line(lty = Line_type, size = Line_size, color = alpha(Gr_line, Gr_alpha))
	
#	if (scatterlabels == "Y") {
#		g2 <- g + geom_text_repel(label=graphdata$name[order(graphdata$yvarrr_IVS)], stat="qq")
#	} else {
		g2 <- g
#	}
	suppressWarnings(print(g2))
}

CAT_QQPLOT <- function() {
	g <- ggplot(graphdata, aes(sample = yvarrr_IVS, color = catfact)) +
		theme_map +
		mytheme +
		theme(legend.position = Gr_legend_pos) +
		ylab(YAxisTitle) +
		xlab(XAxisTitle) +
		ggtitle(MainTitle2) +
		scale_color_manual(values = Gr_palette) +
		scale_fill_manual(values = Gr_palette) +
		stat_qq(aes(fill = catfact), colour = "black", size = Point_size, shape = Point_shape) +
		stat_qq_line(aes(colour = catfact), lty = Line_type, size = Line_size)

#	if (scatterlabels == "Y") {
#		g2 <- g + geom_text_repel(label = graphdata$name[order(graphdata$yvarrr_IVS)], stat="qq" ,  size = 3.5,force = 10, point.padding=1, show.legend = FALSE)
#		g2 <- g + geom_text_repel(aes(fill=catfact), label=graphdata$name[order(graphdata$yvarrr_IVS)], stat="qq" ,  size = 3.5,force = 10, point.padding=1, show.legend = FALSE)
#		g2 <- g + geom_text_repel(label=graphdata$name[order(graphdata$yvarrr_IVS)], stat="qq" ,  size = 3.5,force = 10, point.padding=1, show.legend = FALSE)
#	} else {
		g2 <- g
#	}
	suppressWarnings(print(g2))
}

#===================================================================================================================
#Logistic regression plots
#===================================================================================================================
LogisticplotNonCat <- function() {
	g <- ggplot(graphdata, aes(x = xvarrr_IVS, y = yvarrr_IVS)) +
		theme_map +
		mytheme +
		theme(legend.position = "none") +
		ylab(YAxisTitle) +
		xlab(XAxisTitle) +
		ggtitle(MainTitle2) +
		geom_point(size = Point_size, shape = Point_shape, color = "black", fill = Gr_fill, position = position_jitter(w = w_Gr_jitscat, h = h_Gr_jitscat)) +
		geom_line(data = newdataPreds, colour = Gr_line, lty = Line_type, size = Line_size) +
		scale_y_continuous(breaks = c(0,1), labels = labelsz ) +
		scale_x_continuous(breaks = pretty_breaks()) 
	suppressWarnings(print(g))
}

LogisticplotOneCat <- function() {
	g <- ggplot(graphdata, aes(x = xvarrr_IVS, y = yvarrr_IVS)) +
		theme_map +
		mytheme +
		theme(legend.position = "none") +
		ylab(YAxisTitle) +
		xlab(XAxisTitle) +
		ggtitle(MainTitle2) +
		facet_wrap(~l_l) +
		geom_point(size = Point_size, shape = Point_shape, color = "black", fill = Gr_fill, position = position_jitter(w = w_Gr_jitscat, h = h_Gr_jitscat)) +
		geom_line(data = newdataPreds, colour = Gr_line, lty = Line_type, size = Line_size) +
		scale_y_continuous(breaks = c(0,1), labels = labelsz ) +
		scale_x_continuous(breaks = pretty_breaks()) 
	suppressWarnings(print(g))
}

LogisticplotTwoCat <- function() {
	g <- ggplot(graphdata, aes(x = xvarrr_IVS, y = yvarrr_IVS)) +
		theme_map +
		mytheme +
		theme(legend.position = "none") +
		ylab(YAxisTitle) +
		xlab(XAxisTitle) +
		ggtitle(MainTitle2) +
		facet_grid(firstcatvarrr_IVS ~ secondcatvarrr_IVS) +
		geom_point(size = Point_size, shape = Point_shape, color = "black", fill = Gr_fill, position = position_jitter(w = w_Gr_jitscat, h = h_Gr_jitscat)) +
		geom_line(data = newdataPreds, colour = Gr_line, lty = Line_type, size = Line_size) +
		scale_y_continuous(breaks = c(0,1), labels = labelsz ) +
		scale_x_continuous(breaks = pretty_breaks()) 
	suppressWarnings(print(g))
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
		g1 <- g + geom_smooth(graphdata = subset(graphdata, catvartest != "N"), method = "lm", se = FALSE, lty = Line_type, size = Line_size, color = alpha(Gr_line, Gr_alpha), fullrange = FALSE)
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
		g1 <- g + geom_smooth(graphdata = subset(graphdata, catvartest != "N"), aes(colour = l_li), method = "lm", se = FALSE, lty = Line_type, size = Line_size, fullrange = FALSE)
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
		geom_line(data = finaldata[finaldata$Type == "Prediction curve",], colour = "black", lty = Line_type, size = Line_size) +
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
		geom_line(data = finaldata[finaldata$Type == "Prediction curve",], colour = "black", lty = Line_type, size = Line_size) +
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
#Equivalence plot 
#===================================================================================================================
EQPLOT2S <- function() {
	g <- ggplot(graphdata, aes(x = X3, y = V9)) +
		theme_map +
		mytheme +
		ylab(YAxisTitle) +
		xlab(XAxisTitle) +
		ggtitle(MainTitle2) +
		scale_x_continuous(breaks = pretty_breaks()) +
		geom_point(size = Point_size, shape = Point_shape, color = "black", fill = Gr_fill)

		g1 <- g + geom_errorbar(aes(xmax = X4, xmin = X5), size = Line_size, colour = Gr_fill, width = ErrorBarWidth) +
		geom_vline(xintercept = gr_lowerEqB, lty = Gr_line_typeint, size = Line_size, colour = Gr_line) +
		geom_vline(xintercept = gr_upperEqB, lty = Gr_line_typeint, size = Line_size, colour = Gr_line)
	suppressWarnings(print(g1))
}

EQPLOT1S <- function(AnalysisType) {
	g <- ggplot(graphdata, aes(x = X3, y = V7)) +
		theme_map +
		mytheme +
		ylab(YAxisTitle) +
		xlab(XAxisTitle) +
		ggtitle(MainTitle2) +
		scale_x_continuous(breaks = pretty_breaks()) +
		geom_point(size = Point_size, shape = Point_shape, color = "black", fill = Gr_fill)

		if (AnalysisType =="upper-sided") {
			g1 <- g + geom_errorbar(aes(xmax = X3, xmin = X4), size = Line_size, colour = Gr_fill, width = ErrorBarWidth) +
			geom_vline(xintercept = gr_upperEqB, lty = Gr_line_typeint, size = Line_size, colour = Gr_line)
		}

		if (AnalysisType =="lower-sided") {
			g1 <- g + geom_errorbar(aes(xmax = X3, xmin = X4), size = Line_size, colour = Gr_fill, width = ErrorBarWidth) +
			geom_vline(xintercept = gr_lowerEqB, lty = Gr_line_typeint, size = Line_size, colour = Gr_line) 
		}
	suppressWarnings(print(g1))
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


POWERPLOT_NEW <- function(titlez, legpos) {
	g <- ggplot(graphdata, aes(x = diffs, y = value)) +
		theme_map +
		mytheme +
		guides(color = guide_legend(title = titlez)) +
		ylab("Statistical power (%)") +
		xlab(XAxisTitle) +
		ggtitle(MainTitle2) +
		coord_cartesian(ylim = c(powerFrom, powerTo)) +
		scale_x_continuous(breaks = pretty_breaks()) +
		scale_y_continuous(breaks = pretty_breaks()) +
		scale_colour_manual(name = namez, breaks = lin_list2, labels = userlistgr, values = Gr_palette_A) +
		geom_line(aes(group = variable, color = variable), size = Line_size)

		if (legpos == "right") {
			g1 <- g + theme(legend.position = "right", legend.title = element_text(colour = "black")) 
		} else {
			g1 <- g + theme(legend.position = Gr_legend_pos2, legend.title = element_text(colour = "black")) 
		}

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
		scale_y_continuous(breaks = pretty_breaks()) +
		scale_colour_manual(name = "Absolute change: ", breaks = lin_list2, labels = expectedChanges, values = Gr_palette_P) +
		geom_line(aes(group = variable, color = variable), size = Line_size)

		ranngeSS<-sampleSizeTo-sampleSizeFrom
		if (ranngeSS<15){
			g1<- g +scale_x_continuous(breaks = c(sampleSizeFrom:sampleSizeTo) )
		} else {
			g1<- g +scale_x_continuous(breaks = pretty_breaks())	
		}
	suppressWarnings(print(g1))
}

#Equivalence of Means Comparison module plots
EQPOWERPLOT_ABSOLUTE <- function(power2data, XAxisTitle, MainTitle2, lin_list2, Gr_palette_P) {
	g <- ggplot(power2data, aes(x = sample, y = value)) +
		theme_map +
		mytheme +
		theme(legend.position = Gr_legend_pos2, legend.title = element_text(colour = "black")) +
		ylab("Statistical power (%)") +
		xlab(XAxisTitle) +
		ggtitle(MainTitle2) +
		coord_cartesian(ylim = c(powerFrom, powerTo)) +
		scale_y_continuous(breaks = pretty_breaks()) +
		scale_colour_manual(name = "True bias: ", breaks = lin_list2, labels = expectedBias, values = Gr_palette_P) +
		geom_line(aes(group = variable, color = variable), size = Line_size)

		ranngeSS<-sampleSizeTo-sampleSizeFrom
		if (ranngeSS<15){
			g1<- g +scale_x_continuous(breaks = c(sampleSizeFrom:sampleSizeTo) )
		} else {
			g1<- g +scale_x_continuous(breaks = pretty_breaks())	
		}
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
		scale_y_continuous(breaks = pretty_breaks()) +
		scale_colour_manual(name = "Percent change: ", breaks = lin_list2, labels = expectedChanges2, values = Gr_palette_P) +
		geom_line(aes(group = variable, color = variable), size = Line_size)
#		 g1 <- g + geom_line(aes(group = variable, color = variable), size = Line_size)

		ranngeSS<-sampleSizeTo-sampleSizeFrom
		if (ranngeSS<15){
			g1<- g + scale_x_continuous(breaks = c(sampleSizeFrom:sampleSizeTo) )
		} else {
			g1<- g + scale_x_continuous(breaks = pretty_breaks()) 
		}
	suppressWarnings(print(g1))
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
		scale_y_continuous(breaks = pretty_breaks()) +
		geom_line(aes(group = variable), lty = Line_type, size = Line_size, color = alpha(Gr_line, Gr_alpha))

		ranngeSS<-as.numeric(sampleSizeTo)-as.numeric(sampleSizeFrom)
		if (ranngeSS<15){
			g1<- g + scale_x_continuous(breaks = c(sampleSizeFrom:sampleSizeTo) )
		} else {
			g1<- g + scale_x_continuous(breaks = pretty_breaks()) 
		}
	suppressWarnings(print(g1))
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
#ROC plot 
#===================================================================================================================

ROCPLOT <- function() {
 g = ggplot() +
		theme_map +
		mytheme +
    		xlab("False Positive Rate (1-Specificity)") +
    		ylab("True Positive Rate (Sensitivity)") +
		scale_x_continuous(breaks = pretty_breaks()) +
		scale_y_continuous(breaks = pretty_breaks()) +
#    		annotate("text", label = result, x = 0.875, y = 0.05, size = 5, colour = "red") +
		geom_line(aes(x=c(0,1),y=c(0,1)), color="grey26", lty = Line_type_dashed, size = Line_size) +
    		geom_line(data=pf,aes(x=FPR,y=TPR),colour = Gr_line , lty = Line_type_solid, size = Line_size) 

		suppressWarnings(print(g))
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
PCbiplotx <- function(PC, x = "PC1", y = "PC2") {
	#PC being a prcomp object
	data <- data.frame(obsnames = row.names(PC$x), PC$x)
	plot <- ggplot(data, aes_string(x = x, y = y)) +
		theme_map +
		mytheme +
		theme(legend.position = Gr_legend_pos) +
		geom_hline(yintercept=0, size = 0.1, linetype="dashed") +
		geom_vline(xintercept=0, size = 0.1, linetype="dashed") + 
		geom_text(alpha = .7, size = 3, aes(label = obsnames)) 
	
	datapc <- data.frame(varnames = rownames(PC$rotation), PC$rotation)
	mult <- min(
		(max(data[, y]) - min(data[, y]) / (max(datapc[, y]) - min(datapc[, y]))),
		(max(data[, x]) - min(data[, x]) / (max(datapc[, x]) - min(datapc[, x])))
	)

	datapc <- transform(datapc,
		v1 = .7 * mult * (get(x)),
		v2 = .7 * mult * (get(y))
	)

	plot2 <- plot + 
		coord_equal() +
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
		theme(legend.position = "right" , axis.text.x = element_text(hjust = 1, angle = 45) ) +
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
# Multiple plot function (combining ggplots)
#===================================================================================================================
# ggplot objects can be passed in ..., or to plotlist (as a list of ggplot objects)
# - cols:   Number of columns in layout
# - layout: A matrix specifying the layout. If present, 'cols' is ignored.
#
# If the layout is something like matrix(c(1,2,3,3), nrow=2, byrow=TRUE),
# then plot 1 will go in the upper left, 2 will go in the upper right, and
# 3 will go all the way across the bottom.
#
multiplot <- function(..., plotlist=NULL, file, cols=1, layout=NULL) {
  library(grid)

  # Make a list from the ... arguments and plotlist
  plots <- c(list(...), plotlist)

  numPlots = length(plots)

  # If layout is NULL, then use 'cols' to determine layout
  if (is.null(layout)) {
    # Make the panel
    # ncol: Number of columns of plots
    # nrow: Number of rows needed, calculated from # of cols
    layout <- matrix(seq(1, cols * ceiling(numPlots/cols)),
                    ncol = cols, nrow = ceiling(numPlots/cols))
  }

 if (numPlots==1) {
    print(plots[[1]])

  } else {
    # Set up the page
    grid.newpage()
    pushViewport(viewport(layout = grid.layout(nrow(layout), ncol(layout))))

    # Make each plot, in the correct location
    for (i in 1:numPlots) {
      # Get the i,j matrix positions of the regions that contain this subplot
      matchidx <- as.data.frame(which(layout == i, arr.ind = TRUE))

      print(plots[[i]], vp = viewport(layout.pos.row = matchidx$row,
                                      layout.pos.col = matchidx$col))
    }
  }
}

#===================================================================================================================
#References
#===================================================================================================================

refxx <- c("For more information on the theoretical approaches that are implemented within this module, see Bate and Clark (2014).")


R_refs <- function() {
	R_ref <- "R Development Core Team (2013). R: A language and environment for statistical computing. R Foundation for Statistical Computing, Vienna, Austria. URL http://www.R-project.org."
	BateClark_ref <- "Bate, S.T. and Clark, R.A. (2014). The Design and Statistical Analysis of Animal Experiments. Cambridge University Press."
	IVS_ref <- paste("When referring to InVivoStat, please cite 'InVivoStat, version ", IVS_version, "'.", sep = "")

	Refs <- list(
		R_ref = R_ref,
		BateClark_ref = BateClark_ref,
		IVS_ref = IVS_ref
             )
             return(Refs)
}
