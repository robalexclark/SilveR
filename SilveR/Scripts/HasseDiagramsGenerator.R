# Hasse diagram generator. The common script is prepended by the R processor.
args <- commandArgs(TRUE)

split.factors <- function(value) {
    if (is.null(value) || value == "NULL" || value == "") {
        return(character(0))
    }

    trimws(unlist(strsplit(value, ",")))
}

fixed.factors <- split.factors(args[4])
random.factors <- split.factors(args[5])
factors <- unique(c(fixed.factors, random.factors))

statdata <- read.csv(args[3], header = TRUE, sep = ",", check.names = FALSE, stringsAsFactors = TRUE)
missing.factors <- setdiff(factors, names(statdata))
if (length(missing.factors) > 0) {
    stop(paste("The selected factors were not found in the analysis dataset:", paste(missing.factors, collapse = ", ")))
}

statdata <- statdata[, factors, drop = FALSE]
complete.rows <- complete.cases(statdata)
if (!all(complete.rows)) {
    statdata <- statdata[complete.rows, , drop = FALSE]
}
if (nrow(statdata) == 0) {
    stop("The analysis dataset has no complete rows for the selected factors.")
}

check.confounded.df <- args[6] == "Y"
object.colour <- args[7]
show.partial.crossing <- args[8] == "Y"
font.colour <- args[11]
line.colour <- args[12]
line.width <- as.numeric(args[13])
partial.crossing.line.colour <- args[14]
partial.crossing.line.width <- as.numeric(args[15])
show.degrees.of.freedom <- args[9] == "Y"
show.maximum.levels <- args[10] == "Y"
small.font.size <- as.numeric(args[16])
medium.font.size <- as.numeric(args[17])
large.font.size <- as.numeric(args[18])

output.html <- sub(".csv$", ".html", args[3])
output.png <- sub(".csv$", ".png", args[3])

if (!requireNamespace("hassediagrams", quietly = TRUE)) {
    stop("The 'hassediagrams' R package is required to generate Hasse diagrams.")
}

png(output.png, width = 1100, height = 700)
tryCatch(
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
    ),
    finally = dev.off()
)

html <- c(
    "<html><head><title>Hasse Diagrams Generator</title></head><body>",
    "<h1>Hasse Diagram</h1>",
    "<p>Fixed factors: ", paste(fixed.factors, collapse = ", "), "</p>",
    "<p>Random factors: ", paste(random.factors, collapse = ", "), "</p>",
    "<p>Confounded degrees of freedom: ", if (check.confounded.df) "checked" else "not checked", "</p>",
    "<img src=\"", basename(output.png), "\" alt=\"Generated Hasse diagram\" />",
    "</body></html>"
)
writeLines(paste(html, collapse = ""), output.html)
