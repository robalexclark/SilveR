#===================================================================================================================
# Libraries
suppressWarnings(library(hassediagrams))
suppressWarnings(library(R2HTML))
suppressWarnings(library(stringr))
#===================================================================================================================

# retrieve args
Args <- commandArgs(TRUE)

#Read in data
statdata <- read.csv(Args[3], header=TRUE, sep=",")

fixedfactors <- Args[4]
randomfactors <- Args[5]
check.confounded.df <- Args[6] == "Y"
object.colour <- Args[7]
show.partial.crossing <- Args[8] == "Y"
show.degrees.of.freedom <- Args[9] == "Y"
show.maximum.levels <- Args[10] == "Y"
font.colour <- Args[11]
line.colour <- Args[12]
line.width <- as.numeric(Args[13])
partial.crossing.line.colour <- Args[14]
partial.crossing.line.width <- as.numeric(Args[15])
small.font.size <- as.numeric(Args[16])
medium.font.size <- as.numeric(Args[17])
large.font.size <- as.numeric(Args[18])

#===================================================================================================================
#Setup the html file and associated css file
htmlFile <- sub(".csv", ".html", Args[3])
HTMLSetFile(file=htmlFile)
cssFile <- "r2html.css"
cssFile <- paste("'",cssFile,"'", sep="")
HTMLCSS(CSSfile = cssFile)

#===================================================================================================================
#Titles and description
#===================================================================================================================
Title <- paste(branding, " Hasse Diagram Generator", sep = "")
HTML.title(Title, HR = 1, align = "left")
HTML("The Hasse Diagram Generator module provides a visualization of the structure of the experimental designs using a Hasse diagram...", align = "left")

#===================================================================================================================
#Parameter manipulation
#===================================================================================================================
fixedfactors <- namereplaceHD(fixedfactors)
randomfactors <- namereplaceHD(randomfactors)
statdata <- replace_ivs_bits(statdata)

split.factors <- function(value) {
  if (is.null(value) || value == "NULL" || value == "") return(character(0))
  trimws(unlist(strsplit(value, ",")))
}

fixed.factors  <- split.factors(fixedfactors)
random.factors <- split.factors(randomfactors)
factors <- unique(c(fixed.factors, random.factors))

missing.factors <- setdiff(factors, names(statdata))
if (length(missing.factors) > 0) {
  stop(paste("The selected factors were not found in the analysis dataset:",
             paste(missing.factors, collapse = ", ")))
}

statdata <- statdata[, factors, drop = FALSE]
statdata <- statdata[complete.cases(statdata), , drop = FALSE]

if (nrow(statdata) == 0) {
  HTML.title("Warning", HR = 1, align = "left")
  HTML("The analysis dataset has no complete rows for the selected factors.")
  quit()
}

#===================================================================================================================
#Plot
#===================================================================================================================
HTML("<br>", align = "left")
HTML.title("Hasse diagram of the Layout Structure of the Experimental Design", HR = 2, align = "left")
HTML("<br>", align = "left")

scatterPlotx <- sub(".html", "scatterPlotx.png", htmlFile)
png(scatterPlotx,width = jpegwidth, height = jpegheight, units="in", res=PlotResolution)

plotFilepdf2 <- sub(".html", "scatterPlotx.pdf", htmlFile)
dev.control("enable")

#===================================================================================================================
# ⭐ NEW SECTION: Capture confounded DF error
#===================================================================================================================

confound_output <- capture.output(
  hassediagrams::hasselayout(
    datadesign = statdata,
    randomfacsid = as.integer(names(statdata) %in% random.factors),
    showpartialLS = show.partial.crossing,
    showdfLS = show.degrees.of.freedom,
    check.confound.df = check.confounded.df,
    maxlevels.df = show.maximum.levels,
    structural.colour = line.colour,
    structural.width = line.width,
    partial.colour = partial.crossing.line.colour,
    partial.width = partial.crossing.line.width,
    objects.colour = object.colour,
    df.colour = font.colour,
    larger.fontlabelmultiplier = large.font.size,
    middle.fontlabelmultiplier = medium.font.size,
    smaller.fontlabelmultiplier = small.font.size
  )
)

confound_msg <- confound_output[
  grepl("confound", confound_output, ignore.case = TRUE)
]

# Identify start and end of confounding block
start_idx <- grep("^There are", confound_output)
end_idx   <- tail(grep("Yes$", confound_output), 1)

confound_block <- character(0)

if (length(start_idx) > 0 && length(end_idx) > 0 && end_idx >= start_idx) {
  confound_block <- confound_output[start_idx:end_idx]
}

#===================================================================================================================
# Insert graph (if produced)
#===================================================================================================================
void <- HTMLInsertGraph(GraphFileName=sub("[A-Z0-9a-z,:,\\\\]*App_Data[\\\\]","", scatterPlotx), Align="centre")

#===================================================================================================================
# If confounding occurred, write message into HTML
#===================================================================================================================
if (length(confound_block) > 0) {
  
  # Extract the headline statement
  headline <- confound_block[1]   # "There are 3 confounded degrees of freedom..."
  
  # Extract header lines
  header1 <- grep("Actual levels DF by Subtraction", confound_block)
  header2 <- grep("Potential confounded DF", confound_block)
  
  # Extract table 1 rows
  table1_rows <- confound_block[(header1 + 1):(header2 - 1)]
  table1 <- do.call(rbind, lapply(table1_rows, function(line) {
    parts <- strsplit(line, "\\s+")[[1]]
    data.frame(
      Factor = parts[1],
      Levels = parts[2],
      DF = parts[3],
      stringsAsFactors = FALSE
    )
  }))
  
  # Extract table 2 rows
  table2_rows <- confound_block[(header2 + 1):length(confound_block)]
  table2 <- do.call(rbind, lapply(table2_rows, function(line) {
    parts <- strsplit(line, "\\s+")[[1]]
    data.frame(
      Factor = parts[1],
      Confounded = parts[2],
      stringsAsFactors = FALSE
    )
  }))
  
  # Output to HTML
  HTML.title("Confounded Degrees of Freedom Detected", HR = 2, align = "left")
  
  # Print headline statement
  HTML(paste("<b>", headline, "</b>", sep=""), align = "left")
  
  HTML("<br>", align = "left")
  HTML("<b>Actual levels DF by Subtraction</b>", align = "left")
  HTML(table1, classfirstline = "second", align = "left", row.names = FALSE)
  
  HTML("<br>", align = "left")
  HTML("<b>Potential Confounded DF</b>", align = "left")
  HTML(table2, classfirstline = "second", align = "left", row.names = FALSE)
}





#if (length(confound_block) > 0) {
#  HTML.title("Confounded Degrees of Freedom Detected", HR = 2, align = "left")
#  HTML(paste(confound_block, collapse = "<br>"), align = "left")
#}

#===================================================================================================================
# PDF output
#===================================================================================================================
if (pdfout=="Y") {
  pdf(file=sub("[A-Z0-9a-z,:,\\\\]*App_Data[\\\\]","", plotFilepdf2), height = pdfheight, width = pdfwidth)
  dev.set(2)
  dev.copy(which=3)
  dev.off(2)
  dev.off(3)
  pdfFile_2 <- sub("[A-Z0-9a-z,:,\\\\]*App_Data[\\\\]","",plotFilepdf2)
  linkToPdf2 <- paste ("<a href=\"",pdfFile_2,"\">Click here to view the PDF of the matrixplot</a>", sep = "")
  HTML(linkToPdf2)
}

HTML("Warning! For this diagram to be accurate the levels of the factors in the original dataset need to be distinct and have a practical meaning. 
So, for example, in a study involving 20 animals, 10 per group, they should be numbered 1 to 20 in the dataset and not 1 to 10 within each group.", align = "left")

#===================================================================================================================
# References
#===================================================================================================================
HTML("<br>", align = "left")
HTML.title("Hasse diagram properties", HR = 2, align = "left")

HTML("On the diagram, factors/generalised factors that are crossed in the experimental design, such as in factorial designs, are represented by diamond structural line patterns on the diagram. The factors/generalised factors are placed horizonally alongside each other with the factor/generalised factor that nests both factors forming the top of the diamond, and the generalised factor that involves both factors forming the bottom of the diamond.", align = "left") 
if (show.partial.crossing == "Y") {
  HTML("If the two factors/generalised factors are partially crossed, then a dotted line is included in the diagram linking them.", align = "left") 
}
HTML("If a factor is nested within another, then it is placed below the nesting factor with a vertical line linking the two factors.", align = "left") 
HTML("Factors corresponding to random effects are underlined.", align = "left") 
if (show.degrees.of.freedom == "Y"){
  HTML("The labels underneath each structural objects of the form [a,b] contain (a) the total number of levels of the factor/generalised factor and (b) the factor/generalised factor degrees of freedom.", align = "left") 
}
if (show.maximum.levels == "Y"){
  HTML("The labels underneath each structural objects of the form [a(b),c] contain (a) the total number of potential levels of the factor/generalised factor (b) the number of levels of the factor/generalised factor that are present in the design and (c) and the factor/generalised factor degrees of freedom.", align = "left") 
}


HTML("<br>", align = "left")
HTML.title("Definitions", HR = 2, align = "left")

HTML("For ease of defining key concepts and for introducing notation, consider two factors A and B.", align = "left") 

HTML("Factor A is said to be nested within factor B, denoted as A(B), if each level of A occurs with one and only one level of B but at least one level of B occurs with multiple levels of A (Montgomery, 2017, Chapter 7) . In this case, A is said to be finer than B, and B is coarser than A.", align = "left") 

HTML("The two factors, A and B, are fully crossed if all levels of A occur with all levels of B and vice versa (Montgomery, 2017, Chapter 7).", align = "left") 

HTML("Following Bailey (1996) and Tjur (1984), a generalised factor whose levels correspond to combinations of the crossed factors A and B is defined as: A &#8743; B.", align = "left")

HTML("Two factors, A and B, are partially crossed if they are not fully crossed, but at least one level of A occurs with more than one level of B and vice versa.", align = "left")

HTML("Two factors A and B are defined as equivalent if, for every occurrence of a level of A within the design, the same level of B occurs, and vice versa. In other words, the two factors are identical
apart from the names of their levels, see Bailey (2008, p. 170) and Tjur (1984).", align = "left")

HTML("The layout structure consists of (i) a list of structural objects consisting of the factors that define the experimental design (identified in stage 1), (ii) the generalised factors that are implied by the structure of the experimental design, (iii) a description of the nested, crossed, and equivalent relationships between the factors, as defined by the experimental design.", align = "left")


#===================================================================================================================
# References
#===================================================================================================================

HTML("<br>", align = "left")
HTML.title("References", HR = 2, align = "left")

Ref_list <- R_refs()
HTML(Ref_list$IVS_ref, align = "left")
HTML("Bailey, R.A. (1996) Orthogonal partitions in designed experiments. Designs, Codes and Cryptography, 8:45&#8211;77.", align = "left")
HTML("Bailey, R.A. (2008) Design of comparative experiments, volume 25. Cambridge University Press.", align = "left")
HTML("Bate, S.T. and Chatfield, M.J. (2016) Identifying the structure of the experimental design. Journal of Quality Technology 48(4): 343-364.", align = "left")
HTML("Montgomery, D.C. (2017) Design and analysis of experiments. John wiley & sons.", align = "left")
HTML("Tjur, T. (1984) Analysis of variance models in orthogonal designs. International Statistical Review/Revue Internationale de Statistique, pages 33&#8211;65.", align = "left")

HTML.title("R references", HR = 4, align = "left")
HTML(Ref_list$R_ref, align = "left")
HTML(reference("R2HTML"))
HTML(reference("hassediagrams"))

#===================================================================================================================
# Show dataset
#===================================================================================================================

if (showdataset == "Y") {
  HTML("<br>", align = "left")
  HTML.title("Analysis dataset", HR = 2, align = "left")
  HTML(statdata, classfirstline = "second", align = "left", row.names = "FALSE")
}

#===================================================================================================================
# Show arguments
#===================================================================================================================

if (OutputAnalysisOps == "Y") {
  HTML("<br>", align = "left")
  HTML.title("Analysis options", HR = 2, align = "left")
  if (length( fixed.factors) != 0) {HTML(paste("Fixed factor: ", fixed.factors, sep = ""), align = "left")}
  if (length(random.factors) != 0) {HTML(paste("Random factor: ", random.factors, sep = ""), align = "left")}
  HTML(paste("Check confounded DF? ", check.confounded.df, sep = ""), align = "left")
  HTML(paste("Structural object text colour: ", object.colour, sep = ""), align = "left")
  HTML(paste("Show partial crossing on diagram: ", show.partial.crossing, sep = ""), align = "left")
  HTML(paste("Show degrees of freedom on diagram: ", show.degrees.of.freedom, sep = ""), align = "left")
  HTML(paste("Show max levels on diagram: ", show.maximum.levels, sep = ""), align = "left")
  HTML(paste("Font colour: ", font.colour, sep = ""), align = "left")
  HTML(paste("Structural line colour: ", line.colour, sep = ""), align = "left")
  HTML(paste("Structural line width: ", line.width, sep = ""), align = "left")
  HTML(paste("Partial crossing line colour: ", partial.crossing.line.colour, sep = ""), align = "left")
  HTML(paste("Partial crossing line width: ", partial.crossing.line.width, sep = ""), align = "left")
  HTML(paste("Small font size: ", small.font.size, sep = ""), align = "left")
  HTML(paste("Medium font size: ", medium.font.size, sep = ""), align = "left")
  HTML(paste("Large font size: ", large.font.size, sep = ""), align = "left")
}

