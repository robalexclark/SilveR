# Hasse diagram generator. The common script is prepended by the R processor.
args <- commandArgs(TRUE)

split.factors <- function(value) {
    if (is.null(value) || value == "NULL" || value == "") return(character(0))
    trimws(unlist(strsplit(value, ",")))
}

fixed.factors <- split.factors(args[4])
random.factors <- split.factors(args[5])
factors <- unique(c(fixed.factors, random.factors))

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

terms <- "Mean"
term.ranks <- 0
if (length(factors) > 0) {
    for (size in seq_along(factors)) {
        combinations <- combn(factors, size, simplify = FALSE)
        terms <- c(terms, vapply(combinations, function(x) paste(x, collapse = ":"), ""))
        term.ranks <- c(term.ranks, rep(size, length(combinations)))
    }
}

coordinates <- matrix(0, nrow = length(terms), ncol = 2)
for (rank in unique(term.ranks)) {
    indices <- which(term.ranks == rank)
    coordinates[indices, 1] <- seq(-1, 1, length.out = length(indices))
    coordinates[indices, 2] <- rank
}

png(output.png, width = 1100, height = 700)
par(mar = c(2, 2, 2, 2))
plot.title <- if (check.confounded.df) "Hasse Diagram (confounded DF checked)" else "Hasse Diagram (confounded DF not checked)"
plot(coordinates, type = "n", axes = FALSE, xlab = "", ylab = "", main = plot.title, ylim = c(-0.5, max(term.ranks) + 0.8))

for (upper in seq_along(terms)) {
    for (lower in seq_along(terms)) {
        if (term.ranks[upper] == term.ranks[lower] + 1) {
            lower.term <- if (terms[lower] == "Mean") character(0) else strsplit(terms[lower], ":", fixed = TRUE)[[1]]
            upper.term <- strsplit(terms[upper], ":", fixed = TRUE)[[1]]
            if (all(lower.term %in% upper.term)) {
                segments(coordinates[lower, 1], coordinates[lower, 2], coordinates[upper, 1], coordinates[upper, 2], col = line.colour, lwd = line.width)
            }
        }
    }
}

if (show.partial.crossing && length(fixed.factors) > 0 && length(random.factors) > 0) {
    fixed.indices <- which(terms %in% fixed.factors)
    random.indices <- which(terms %in% random.factors)
    for (fixed.index in fixed.indices) {
        for (random.index in random.indices) {
            segments(coordinates[fixed.index, 1], coordinates[fixed.index, 2], coordinates[random.index, 1], coordinates[random.index, 2],
                col = partial.crossing.line.colour, lwd = partial.crossing.line.width, lty = 2)
        }
    }
}

points(coordinates[, 1], coordinates[, 2], pch = 21, bg = object.colour, col = object.colour, cex = 2)
labels <- terms
if (show.degrees.of.freedom) labels <- paste0(labels, "\nDF: ", term.ranks)
if (show.maximum.levels) labels <- paste0(labels, "\nLevels: max")
label.sizes <- ifelse(term.ranks == 0, small.font.size, ifelse(term.ranks == 1, medium.font.size, large.font.size))
text(coordinates[, 1], coordinates[, 2], labels = labels, pos = 3, col = font.colour, cex = label.sizes)
dev.off()

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
