## ----00-options, include=FALSE, eval=TRUE-------------------------------------
library(knitr)
# global options - probably better to put out.width='60%' for pdf output
knitr::opts_chunk$set(dpi = 100, echo=TRUE, warning=FALSE, message=FALSE, eval = TRUE, cache=FALSE,
                      fig.show=TRUE, fig.asp=1,fig.align='center', out.width = '75%',
                      fig.pos= "h", out.extra = '', fig.path= 'Figures/')

colorize <- function(color, x) {
  if (knitr::is_html_output()) {
    htmlcolor = "black"
    if(color == "blue"){
      htmlcolor = "#388ECC"
    }
    if(color == "orange"){
      htmlcolor = "#F68B33"
    }
    if(color == "grey"){
      htmlcolor = "#585858"
    }
    if(color == "green"){
      htmlcolor = "#009E73"
    }
    if(color == "pink"){
      htmlcolor = "#CC79A7"
    }
    if(color == "yellow"){
      htmlcolor = "#999900"
    }
    if(color == "darkred"){
      htmlcolor = "#CC0000"
    }
    sprintf("<span style='color: %s;'>%s</span>", htmlcolor, x)
  } else {
    sprintf("\\textcolor{%s}{%s}", color, x)
  }
}

# The libraries to load
#install.packages(kableExtra)
#library(kableExtra)

## ----00-header-generation, echo=FALSE, eval=FALSE-----------------------------
#  ## run this only to re-make the logo in header.html
#  ## Create the external file
#  img <- htmltools::img(src = knitr::image_uri("InputFigures/logo-mixomics.jpg"),
#                 alt = 'logo',
#                 style = 'position:absolute; top:25px; right:1%; padding:10px;z-index:200;')
#  
#  htmlhead <- paste0('
#  <script>
#  document.write(\'<div class="logos">',img,'</div>\')
#  </script>
#  ')
#  
#  readr::write_lines(htmlhead, path = "header.html")

## ----00-analyses-diagram, eval=TRUE, echo=FALSE,fig.cap='**Different types of analyses with mixOmics** [@mixomics].The biological questions, the number of data sets to integrate, and the type of response variable, whether qualitative (classification), quantitative (regression), one (PLS1) or several (PLS) responses, all drive the choice of analytical method. All methods featured in this diagram include variable selection except rCCA. In N-integration, rCCA and PLS enable the integration of two quantitative data sets, whilst the block PLS methods (that derive from the methods from @Ten11) can integrate more than two data sets. In P-integration, our method MINT is based on multi-group PLS [@Esl14b].The following activities cover some of these methods.'----

knitr::include_graphics("InputFigures/MixOmicsAnalysesV2.png")

## ----01-fig-path, echo = FALSE------------------------------------------------
knitr::opts_chunk$set(fig.path= 'Figures/Introduction')

## ----01-methods-diagram, echo=FALSE, fig.cap="List of methods in mixOmics, sparse indicates methods that perform variable selection", out.width='100%', fig.align='center'----
knitr::include_graphics("InputFigures/Methods.png")

## ----01-cheatsheet, echo=FALSE, fig.cap="Main functions and parameters of each method", out.width= '100%', fig.align='center'----
knitr::include_graphics("InputFigures/cheatsheet.png")

## ----02-fig-path, echo = FALSE------------------------------------------------
knitr::opts_chunk$set(fig.path= 'Figures/Getting-Started')

## ----02-install-bioc, eval = FALSE--------------------------------------------
#  if (!requireNamespace("BiocManager", quietly = TRUE))
#      install.packages("BiocManager")
#   BiocManager::install("mixOmics")

## ----02-install-github, eval = FALSE------------------------------------------
#  BiocManager::install("mixOmicsTeam/mixOmics")

## ----02-load, message=FALSE---------------------------------------------------
library(mixOmics)

## ----02-read-data, eval = FALSE-----------------------------------------------
#  # from csv file
#  data <- read.csv("your_data.csv", row.names = 1, header = TRUE)
#  
#  # from txt file
#  data <- read.table("your_data.txt", header = TRUE)

## ----02-load-nutrimouse-------------------------------------------------------
data(nutrimouse)
X <- nutrimouse$gene

## ----02-pca-nutrimouse--------------------------------------------------------
MyResult.pca <- pca(X)  # 1 Run the method
plotIndiv(MyResult.pca) # 2 Plot the samples
plotVar(MyResult.pca)   # 3 Plot the variables

## ----02-spca-nutrimouse-------------------------------------------------------
MyResult.spca <- spca(X, keepX=c(5,5)) # 1 Run the method
plotIndiv(MyResult.spca)               # 2 Plot the samples
plotVar(MyResult.spca)                 # 3 Plot the variables

## ----03-fig-path, echo = FALSE------------------------------------------------
knitr::opts_chunk$set(fig.path= 'Figures/PCA/')

## ----03-load-multidrug, message=FALSE, warning=FALSE--------------------------
library(mixOmics)
data(multidrug)
X <- multidrug$ABC.trans
dim(X) # Check dimensions of data

## ----03-screeplot, fig.cap='(ref:03-screeplot)'-------------------------------
tune.pca.multi <- tune.pca(X, ncomp = 10, scale = TRUE)
plot(tune.pca.multi)
# tune.pca.multidrug$cum.var       # Outputs cumulative proportion of variance

## ----03-pca-final, echo=TRUE, message=FALSE-----------------------------------
final.pca.multi <- pca(X, ncomp = 3, center = TRUE, scale = TRUE)
# final.pca.multi  # Lists possible outputs

## -----------------------------------------------------------------------------
final.pca.multi$var.tot

## -----------------------------------------------------------------------------
final.pca.multi$prop_expl_var$X

## -----------------------------------------------------------------------------
final.pca.multi$cum.var

## ----03-pca-vars, echo=TRUE, message=FALSE------------------------------------
# Top variables on the first component only:
head(selectVar(final.pca.multi, comp = 1)$value)

## ----03-pca-sample-plot, fig.cap='(ref:03-pca-sample-plot)'-------------------
plotIndiv(final.pca.multi,
          comp = c(1, 2),   # Specify components to plot
          ind.names = TRUE, # Show row names of samples
          group = multidrug$cell.line$Class,
          title = 'ABC transporters, PCA comp 1 - 2',
          legend = TRUE, legend.title = 'Cell line')

## ---- eval = FALSE------------------------------------------------------------
#  # Interactive 3D plot will load the rgl library.
#  plotIndiv(final.pca.multi, style = '3d',
#             group = multidrug$cell.line$Class,
#            title = 'ABC transporters, PCA comp 1 - 3')

## ---- eval = FALSE------------------------------------------------------------
#  plotVar(final.pca.multi, comp = c(1, 2),
#          var.names = TRUE,
#          cex = 3,         # To change the font size
#          # cutoff = 0.5,  # For further cutoff
#          title = 'Multidrug transporter, PCA comp 1 - 2')

## ----03-pca-variable-plot, echo = FALSE, fig.cap='(ref:03-pca-variable-plot)'----
col.var <- c(rep(color.mixo(1), ncol(X)))
names(col.var) = colnames(X)
col.var[c('ABCE1', 'ABCB8')] = color.mixo(2)
col.var[c('ABCA8')] = color.mixo(4)
col.var[c('ABCC2')] = color.mixo(5)
col.var[c('ABCC12', 'ABCD2')] = color.mixo(6)

plotVar(final.pca.multi, comp = c(1, 2),
        var.names = TRUE,
        col = list(col.var),
        cex = 3,
        title = 'Multidrug transporter, PCA comp 1 - 2')

## ----03-pca-biplot, fig.cap='(ref:03-pca-biplot)'-----------------------------
biplot(final.pca.multi, group = multidrug$cell.line$Class, 
       legend.title = 'Cell line')

## ----03-pca-boxplot, fig.cap='(ref:03-pca-boxplot)'---------------------------
ABCC2.scale <- scale(X[, 'ABCC2'], center = TRUE, scale = TRUE)

boxplot(ABCC2.scale ~
        multidrug$cell.line$Class, col = color.mixo(1:9),
        xlab = 'Cell lines', ylab = 'Expression levels, scaled',
        par(cex.axis = 0.5), # Font size
        main = 'ABCC2 transporter')

## ----03-spca-tuning, fig.cap='(ref:03-spca-tuning)'---------------------------
grid.keepX <- c(seq(5, 30, 5))
# grid.keepX  # To see the grid

set.seed(30) # For reproducibility with this handbook, remove otherwise
tune.spca.result <- tune.spca(X, ncomp = 3, 
                              folds = 5, 
                              test.keepX = grid.keepX, nrepeat = 10) 

# Consider adding up to 50 repeats for more stable results
tune.spca.result$choice.keepX

plot(tune.spca.result)

## ----03-spca-final------------------------------------------------------------
# By default center = TRUE, scale = TRUE
keepX.select <- tune.spca.result$choice.keepX[1:2]

final.spca.multi <- spca(X, ncomp = 2, keepX = keepX.select)

# Proportion of explained variance:
final.spca.multi$prop_expl_var$X

## ----03-spca-sample-plot, fig.cap='(ref:03-spca-sample-plot)'-----------------
plotIndiv(final.spca.multi,
          comp = c(1, 2),   # Specify components to plot
          ind.names = TRUE, # Show row names of samples
          group = multidrug$cell.line$Class,
          title = 'ABC transporters, sPCA comp 1 - 2',
          legend = TRUE, legend.title = 'Cell line')

## ----03-spca-biplot, fig.cap='(ref:03-spca-biplot)'---------------------------
biplot(final.spca.multi, group = multidrug$cell.line$Class, 
       legend =FALSE)

## ---- eval = FALSE------------------------------------------------------------
#  plotVar(final.spca.multi, comp = c(1, 2), var.names = TRUE,
#          cex = 3, # To change the font size
#          title = 'Multidrug transporter, sPCA comp 1 - 2')

## ----03-spca-variable-plot, fig.cap='(ref:03-spca-variable-plot)', echo = FALSE----
col.var <- c(rep(color.mixo(1), ncol(X)))
names(col.var) <- colnames(X)
col.var[c("ABCA9", "ABCB5", "ABCC2","ABCD1")] <- color.mixo(4)

plotVar(final.spca.multi, comp = c(1, 2), var.names = TRUE,
        col = list(col.var), cex = 3, # To change the font size
        title = 'Multidrug transporter, sPCA comp 1 - 2')

## ----echo=TRUE, message=FALSE-------------------------------------------------
# On the first component, just a head
head(selectVar(final.spca.multi, comp = 2)$value)

## ----03-spca-loading-plot, fig.cap='(ref:03-spca-loading-plot)'---------------
plotLoadings(final.spca.multi, comp = 2)

## ----04-fig-path, echo = FALSE------------------------------------------------
knitr::opts_chunk$set(fig.path= 'Figures/PLS/')

## ----04-load-data2, echo = FALSE----------------------------------------------
devtools::install_github("mixOmicsTeam/mixOmics")
data(liver.toxicity)
X <- liver.toxicity$gene
Y <- liver.toxicity$clinic

## ----04-load-data, eval = FALSE-----------------------------------------------
#  library(mixOmics)
#  data(liver.toxicity)
#  X <- liver.toxicity$gene
#  Y <- liver.toxicity$clinic

## -----------------------------------------------------------------------------
head(data.frame(rownames(X), rownames(Y)))

## -----------------------------------------------------------------------------
y <- liver.toxicity$clinic[, "ALB.g.dL."]

## ----04-spls1-ncomp, fig.cap='(ref:04-spls1-ncomp)'---------------------------
tune.pls1.liver <- pls(X = X, Y = y, ncomp = 4, mode = 'regression')
set.seed(33)  # For reproducibility with this handbook, remove otherwise
Q2.pls1.liver <- perf(tune.pls1.liver, validation = 'Mfold', 
                      folds = 10, nrepeat = 5)
plot(Q2.pls1.liver, criterion = 'Q2')

## ----04-spls1-tuning, fig.cap='(ref:04-spls1-tuning)'-------------------------
# Set up a grid of values: 
list.keepX <- c(5:10, seq(15, 50, 5))     

# list.keepX  # Inspect the keepX grid
set.seed(33)  # For reproducibility with this handbook, remove otherwise
tune.spls1.MAE <- tune.spls(X, y, ncomp= 2, 
                            test.keepX = list.keepX, 
                            validation = 'Mfold', 
                            folds = 10,
                            nrepeat = 5, 
                            progressBar = FALSE, 
                            measure = 'MAE')
plot(tune.spls1.MAE)

## -----------------------------------------------------------------------------
choice.ncomp <- tune.spls1.MAE$choice.ncomp$ncomp
# Optimal number of variables to select in X based on the MAE criterion
# We stop at choice.ncomp
choice.keepX <- tune.spls1.MAE$choice.keepX[1:choice.ncomp]  

choice.ncomp
choice.keepX

## ----04-spls1-final-----------------------------------------------------------
spls1.liver <- spls(X, y, ncomp = choice.ncomp, keepX = choice.keepX, 
                    mode = "regression")

## ---- eval = FALSE------------------------------------------------------------
#  selectVar(spls1.liver, comp = 1)$X$name

## -----------------------------------------------------------------------------
spls1.liver$prop_expl_var$X
tune.pls1.liver$prop_expl_var$X

## ----04-spls1-sample-plot, fig.cap='(ref:04-spls1-sample-plot)', message=FALSE----
spls1.liver.c2 <- spls(X, y, ncomp = 2, keepX = c(rep(choice.keepX, 2)), 
                   mode = "regression")

plotIndiv(spls1.liver.c2,
          group = liver.toxicity$treatment$Time.Group,
          pch = as.factor(liver.toxicity$treatment$Dose.Group),
          legend = TRUE, legend.title = 'Time', legend.title.pch = 'Dose')


## ----04-spls1-sample-plot2, fig.cap='(ref:04-spls1-sample-plot2)'-------------
# Define factors for colours matching plotIndiv above
time.liver <- factor(liver.toxicity$treatment$Time.Group, 
                     levels = c('18', '24', '48', '6'))
dose.liver <- factor(liver.toxicity$treatment$Dose.Group, 
                     levels = c('50', '150', '1500', '2000'))
# Set up colours and symbols
col.liver <- color.mixo(time.liver)
pch.liver <- as.numeric(dose.liver)

plot(spls1.liver$variates$X, spls1.liver$variates$Y,
     xlab = 'X component', ylab = 'y component / scaled y',
     col = col.liver, pch = pch.liver)
legend('topleft', col = color.mixo(1:4), legend = levels(time.liver),
       lty = 1, title = 'Time')
legend('bottomright', legend = levels(dose.liver), pch = 1:4,
       title = 'Dose')

cor(spls1.liver$variates$X, spls1.liver$variates$Y)

## ----04-spls1-perf------------------------------------------------------------
set.seed(33)  # For reproducibility with this handbook, remove otherwise

# PLS1 model and performance
pls1.liver <- pls(X, y, ncomp = choice.ncomp, mode = "regression")
perf.pls1.liver <- perf(pls1.liver, validation = "Mfold", folds =10, 
                   nrepeat = 5, progressBar = FALSE)
perf.pls1.liver$measures$MSEP$summary
# To extract values across all repeats:
# perf.pls1.liver$measures$MSEP$values

# sPLS1 performance
perf.spls1.liver <- perf(spls1.liver, validation = "Mfold", folds = 10, 
                   nrepeat = 5, progressBar = FALSE)
perf.spls1.liver$measures$MSEP$summary

## -----------------------------------------------------------------------------
dim(Y)

## ----04-spls2-ncomp, fig.cap='(ref:04-spls2-ncomp)'---------------------------

tune.pls2.liver <- pls(X = X, Y = Y, ncomp = 5, mode = 'regression')

set.seed(33)  # For reproducibility with this handbook, remove otherwise
Q2.pls2.liver <- perf(tune.pls2.liver, validation = 'Mfold', folds = 10, 
                      nrepeat = 5)
plot(Q2.pls2.liver, criterion = 'Q2.total')

## ----04-spls2-tuning, fig.cap='(ref:04-spls2-tuning)', out.width = '60%', warning=FALSE----
# This code may take several min to run, parallelisation option is possible
list.keepX <- c(seq(5, 50, 5))
list.keepY <- c(3:10)

set.seed(33)  # For reproducibility with this handbook, remove otherwise
tune.spls.liver <- tune.spls(X, Y, test.keepX = list.keepX, 
                             test.keepY = list.keepY, ncomp = 2, 
                             nrepeat = 1, folds = 10, mode = 'regression', 
                             measure = 'cor', 
                            #   the following uses two CPUs for faster computation
                            # it can be commented out
                            BPPARAM = BiocParallel::SnowParam(workers = 14)
                            )

plot(tune.spls.liver)

## ----04-spls2-final-----------------------------------------------------------
#Optimal parameters
choice.keepX <- tune.spls.liver$choice.keepX
choice.keepY <- tune.spls.liver$choice.keepY
choice.ncomp <- length(choice.keepX)

spls2.liver <- spls(X, Y, ncomp = choice.ncomp, 
                   keepX = choice.keepX,
                   keepY = choice.keepY,
                   mode = "regression")

## -----------------------------------------------------------------------------
spls2.liver$prop_expl_var

## ---- eval = FALSE------------------------------------------------------------
#  selectVar(spls2.liver, comp = 1)$X$value

## ----04-spls2-vip-------------------------------------------------------------
vip.spls2.liver <- vip(spls2.liver)
# just a head
head(vip.spls2.liver[selectVar(spls2.liver, comp = 1)$X$name,1])

## ----04-spls2-stab, results = 'hide'------------------------------------------
perf.spls2.liver <- perf(spls2.liver, validation = 'Mfold', folds = 10, nrepeat = 5)
# Extract stability
stab.spls2.liver.comp1 <- perf.spls2.liver$features$stability.X$comp1
# Averaged stability of the X selected features across CV runs, as shown in Table
stab.spls2.liver.comp1[1:choice.keepX[1]]

# We extract the stability measures of only the variables selected in spls2.liver
extr.stab.spls2.liver.comp1 <- stab.spls2.liver.comp1[selectVar(spls2.liver, 
                                                                  comp =1)$X$name]

## ----04-spls2-stab-table, echo = FALSE----------------------------------------
knitr::kable(extr.stab.spls2.liver.comp1[21:40], caption = 'Stability measure (occurence of selection) of the bottom 20 variables from X selected with sPLS2 across repeated 10-fold subsampling on component 1.', longtable = TRUE)

## ----04-spls2-sample-plot, fig.cap='(ref:04-spls2-sample-plot)'---------------
plotIndiv(spls2.liver, ind.names = FALSE, 
          group = liver.toxicity$treatment$Time.Group, 
          pch = as.factor(liver.toxicity$treatment$Dose.Group), 
          col.per.group = color.mixo(1:4),
          legend = TRUE, legend.title = 'Time', 
          legend.title.pch = 'Dose')

## ----04-spls2-arrow-plot, fig.cap='(ref:04-spls2-arrow-plot)'-----------------
plotArrow(spls2.liver, ind.names = FALSE, 
          group = liver.toxicity$treatment$Time.Group,
          col.per.group = color.mixo(1:4),
          legend.title = 'Time.Group')

## ----04-spls2-variable-plot, fig.cap='(ref:04-spls2-variable-plot)'-----------
plotVar(spls2.liver, cex = c(3,4), var.names = c(FALSE, TRUE))

## ----04-spls2-variable-plot2, fig.cap='(ref:04-spls2-variable-plot2)'---------
plotVar(spls2.liver,
        var.names = list(X.label = liver.toxicity$gene.ID[,'geneBank'],
        Y.label = TRUE), cex = c(3,4))

## ----04-spls2-network, fig.cap='(ref:04-spls2-network)'-----------------------
# Define red and green colours for the edges
color.edge <- color.GreenRed(50)

# X11()  # To open a new window for Rstudio
network(spls2.liver, comp = 1:2,
        cutoff = 0.7,
        shape.node = c("rectangle", "circle"),
        color.node = c("cyan", "pink"),
        color.edge = color.edge,
        # To save the plot, unotherwise:
        # save = 'pdf', name.save = 'network_liver'
        )

## ----04-spls2-cim, fig.cap='(ref:04-spls2-cim)'-------------------------------
# X11()  # To open a new window if the graphic is too large
cim(spls2.liver, comp = 1:2, xlab = "clinic", ylab = "genes",
    # To save the plot, uncomment:
    # save = 'pdf', name.save = 'cim_liver'
    )

## ----04-spls2-perf------------------------------------------------------------
# Comparisons of final models (PLS, sPLS)

## PLS
pls.liver <- pls(X, Y, mode = 'regression', ncomp = 2)
perf.pls.liver <-  perf(pls.liver, validation = 'Mfold', folds = 10, 
                        nrepeat = 5)

## Performance for the sPLS model ran earlier
perf.spls.liver <-  perf(spls2.liver, validation = 'Mfold', folds = 10, 
                         nrepeat = 5)

## ----04-spls2-perf2, fig.cap='(ref:04-spls2-perf2)', out.width = '70%'--------
plot(c(1,2), perf.pls.liver$measures$cor.upred$summary$mean, 
     col = 'blue', pch = 16, 
     ylim = c(0.6,1), xaxt = 'n',
     xlab = 'Component', ylab = 't or u Cor', 
     main = 's/PLS performance based on Correlation')
axis(1, 1:2)  # X-axis label
points(perf.pls.liver$measures$cor.tpred$summary$mean, col = 'red', pch = 16)
points(perf.spls.liver$measures$cor.upred$summary$mean, col = 'blue', pch = 17)
points(perf.spls.liver$measures$cor.tpred$summary$mean, col = 'red', pch = 17)
legend('bottomleft', col = c('blue', 'red', 'blue', 'red'), 
       pch = c(16, 16, 17, 17), c('u PLS', 't PLS', 'u sPLS', 't sPLS'))

## ----05-fig-path, echo = FALSE------------------------------------------------
knitr::opts_chunk$set(fig.path= 'Figures/PLSDA/')

## ----results = 'hold', message=FALSE------------------------------------------
library(mixOmics)
data(srbct)
X <- srbct$gene

# Outcome y that will be internally coded as dummy:
Y <- srbct$class 
dim(X); length(Y)

## -----------------------------------------------------------------------------
summary(Y)

## ----05-plsda-pca, fig.cap='(ref:05-plsda-pca)'-------------------------------
pca.srbct <- pca(X, ncomp = 3, scale = TRUE)

plotIndiv(pca.srbct, group = srbct$class, ind.names = FALSE,
          legend = TRUE, 
          title = 'SRBCT, PCA comp 1 - 2')

## ----plsda-ncomp, fig.cap='(ref:plsda-ncomp)'---------------------------------
plsda.srbct <- plsda(X,Y, ncomp = 10)

set.seed(30) # For reproducibility with this handbook, remove otherwise
perf.plsda.srbct <- perf(plsda.srbct, validation = 'Mfold', folds = 3, 
                  progressBar = FALSE,  # Set to TRUE to track progress
                  nrepeat = 10)         # We suggest nrepeat = 50

plot(perf.plsda.srbct, sd = TRUE, legend.position = 'horizontal')

## ---- eval = FALSE------------------------------------------------------------
#  perf.plsda.srbct

## ----05-plsda-final-----------------------------------------------------------
final.plsda.srbct <- plsda(X,Y, ncomp = 3)

## ----05-plsda-sample-plot-nc12, results = 'hide', fig.show = 'hide'-----------
plotIndiv(final.plsda.srbct, ind.names = FALSE, legend=TRUE,
          comp=c(1,2), ellipse = TRUE, 
          title = 'PLS-DA on SRBCT comp 1-2',
          X.label = 'PLS-DA comp 1', Y.label = 'PLS-DA comp 2')

## ----05-plsda-sample-plot-nc13, results = 'hide', fig.show = 'hide'-----------
plotIndiv(final.plsda.srbct, ind.names = FALSE, legend=TRUE,
          comp=c(2,3), ellipse = TRUE, 
          title = 'PLS-DA on SRBCT comp 2-3',
          X.label = 'PLS-DA comp 2', Y.label = 'PLS-DA comp 3')

## ----05-plsda-sample-plots, fig.cap='(ref:05-plsda-sample-plots)', echo=FALSE, fig.align='center', out.width = '50%', fig.height=4, fig.ncol = 2, fig.subcap=c('', '')----
knitr::include_graphics(c('Figures/PLSDA/05-plsda-sample-plot-nc12-1.png', 'Figures/PLSDA/05-plsda-sample-plot-nc13-1.png'))

## ----05-plsda-perf------------------------------------------------------------
set.seed(30) # For reproducibility with this handbook, remove otherwise
perf.final.plsda.srbct <- perf(final.plsda.srbct, validation = 'Mfold', 
                               folds = 3, 
                               progressBar = FALSE, # TRUE to track progress
                               nrepeat = 10) # we recommend 50 

## -----------------------------------------------------------------------------
perf.final.plsda.srbct$error.rate$BER[, 'max.dist']

## -----------------------------------------------------------------------------
perf.final.plsda.srbct$error.rate.class$max.dist

## ----05-plsda-bgp-max, echo = TRUE, results = 'hide', fig.show = 'hide'-------
background.max <- background.predict(final.plsda.srbct, 
                                     comp.predicted = 2,
                                     dist = 'max.dist') 

plotIndiv(final.plsda.srbct, comp = 1:2, group = srbct$class,
          ind.names = FALSE, title = 'Maximum distance',
          legend = TRUE,  background = background.max)

## ----05-plsda-bgp-cent, echo = FALSE, results = 'hide', fig.show = 'hide'-----
background.cent <- background.predict(final.plsda.srbct, 
                                      comp.predicted = 2,
                                      dist = 'centroids.dist') 

plotIndiv(final.plsda.srbct, comp = 1:2, group = srbct$class,
          ind.names = FALSE, title = 'Centroids distance',
          legend = TRUE,  background = background.cent)

## ----05-plsda-bgp-mah, echo = FALSE, results = 'hide', fig.show = 'hide'------
background.mah <- background.predict(final.plsda.srbct, 
                                     comp.predicted = 2,
                                     dist = 'mahalanobis.dist') 

plotIndiv(final.plsda.srbct, comp = 1:2, group = srbct$class,
          ind.names = FALSE, title = 'Mahalanobis distance',
          legend = TRUE,  background = background.mah)

## ----05-plsda-bgp, fig.cap='(ref:05-plsda-bgp)', echo=FALSE, fig.cap='(ref:05-plsda-bgp)', fig.align='center', out.width = '30%', fig.height=4, fig.ncol = 3, fig.subcap=c('', '', '')----
knitr::include_graphics(c('Figures/PLSDA/05-plsda-bgp-max-1.png', 'Figures/PLSDA/05-plsda-bgp-cent-1.png', 'Figures/PLSDA/05-plsda-bgp-mah-1.png'))

## -----------------------------------------------------------------------------
# Grid of possible keepX values that will be tested for each comp
list.keepX <- c(1:10,  seq(20, 100, 10))
list.keepX

## ----05-splsda-tuning---------------------------------------------------------
# This chunk takes ~ 2 min to run
# Some convergence issues may arise but it is ok as this is run on CV folds
tune.splsda.srbct <- tune.splsda(X, Y, ncomp = 4, validation = 'Mfold', 
                                 folds = 5, dist = 'max.dist', 
                                 test.keepX = list.keepX, nrepeat = 10)

## -----------------------------------------------------------------------------
# Just a head of the classification error rate per keepX (in rows) and comp
head(tune.splsda.srbct$error.rate)

## ----05-splsda-tuning-plot, fig.cap='(ref:05-splsda-tuning-plot)'-------------
# To show the error bars across the repeats:
plot(tune.splsda.srbct, sd = TRUE)

## -----------------------------------------------------------------------------
# The optimal number of components according to our one-sided t-tests
tune.splsda.srbct$choice.ncomp$ncomp

# The optimal keepX parameter according to minimal error rate
tune.splsda.srbct$choice.keepX

## ----05-splsda-final----------------------------------------------------------
# Optimal number of components based on t-tests on the error rate
ncomp <- tune.splsda.srbct$choice.ncomp$ncomp 
ncomp

# Optimal number of variables to select
select.keepX <- tune.splsda.srbct$choice.keepX[1:ncomp]  
select.keepX

splsda.srbct <- splsda(X, Y, ncomp = ncomp, keepX = select.keepX) 

## ----05-splsda-perf-----------------------------------------------------------
set.seed(34)  # For reproducibility with this handbook, remove otherwise

perf.splsda.srbct <- perf(splsda.srbct, folds = 5, validation = "Mfold", 
                  dist = "max.dist", progressBar = FALSE, nrepeat = 10)

# perf.splsda.srbct  # Lists the different outputs
perf.splsda.srbct$error.rate

## -----------------------------------------------------------------------------
perf.splsda.srbct$error.rate.class

## ----05-splsda-stab, fig.cap='(ref:05-splsda-stab)', results='hold', fig.show='hold'----
par(mfrow=c(1,2))
# For component 1
stable.comp1 <- perf.splsda.srbct$features$stable$comp1
barplot(stable.comp1, xlab = 'variables selected across CV folds', 
        ylab = 'Stability frequency',
        main = 'Feature stability for comp = 1')

# For component 2
stable.comp2 <- perf.splsda.srbct$features$stable$comp2
barplot(stable.comp2, xlab = 'variables selected across CV folds', 
        ylab = 'Stability frequency',
        main = 'Feature stability for comp = 2')
par(mfrow=c(1,1))

## -----------------------------------------------------------------------------
# First extract the name of selected var:
select.name <- selectVar(splsda.srbct, comp = 1)$name

# Then extract the stability values from perf:
stability <- perf.splsda.srbct$features$stable$comp1[select.name]

# Just the head of the stability of the selected var:
head(cbind(selectVar(splsda.srbct, comp = 1)$value, stability))

## ----05-splsda-sample-plot-nc12, echo = TRUE, results = 'hide', fig.show = 'hide', fig.path = 'Figures/PLSDA/'----
plotIndiv(splsda.srbct, comp = c(1,2),
          ind.names = FALSE,
          ellipse = TRUE, legend = TRUE,
          star = TRUE,
          title = 'SRBCT, sPLS-DA comp 1 - 2')

## ----05-splsda-sample-plot-nc23, echo = TRUE, results = 'hide', fig.show = 'hide', fig.path = 'Figures/PLSDA/'----
plotIndiv(splsda.srbct, comp = c(2,3),
          ind.names = FALSE,
          ellipse = TRUE, legend = TRUE,
          star = TRUE,
          title = 'SRBCT, sPLS-DA comp 2 - 3')

## ----05-splsda-sample-plots, fig.cap='(ref:05-splsda-sample-plots)', echo=FALSE, fig.cap='(ref:05-splsda-sample-plots)', fig.align='center', out.width = '50%', fig.height=4, fig.ncol = 2, fig.subcap=c('(a)', '(b)')----
knitr::include_graphics(c('Figures/PLSDA/05-splsda-sample-plot-nc12-1.png', 'Figures/PLSDA/05-splsda-sample-plot-nc23-1.png'))

## ----05-splsda-variable-plot, fig.cap='(ref:05-splsda-variable-plot)'---------
var.name.short <- substr(srbct$gene.name[, 2], 1, 10)
plotVar(splsda.srbct, comp = c(1,2), 
        var.names = list(var.name.short), cex = 3)

## ----05-splsda-loading-plot, fig.cap='(ref:05-splsda-loading-plot)'-----------
plotLoadings(splsda.srbct, comp = 1, method = 'mean', contrib = 'max', 
             name.var = var.name.short)

## ----05-splsda-cim, fig.width=10, fig.height=8, fig.cap='(ref:05-splsda-cim)'----
cim(splsda.srbct, row.sideColors = color.mixo(Y))

## ----05-splsda-predict1, results='hold'---------------------------------------
set.seed(33) # For reproducibility with this handbook, remove otherwise
train <- sample(1:nrow(X), 50)    # Randomly select 50 samples in training
test <- setdiff(1:nrow(X), train) # Rest is part of the test set

# Store matrices into training and test set:
X.train <- X[train, ]
X.test <- X[test,]
Y.train <- Y[train]
Y.test <- Y[test]

# Check dimensions are OK:
dim(X.train); dim(X.test)

## ----05-splsda-predict2-------------------------------------------------------
train.splsda.srbct <- splsda(X.train, Y.train, ncomp = 3, keepX = c(20,30,40))

## ----05-splsda-predict3-------------------------------------------------------
predict.splsda.srbct <- predict(train.splsda.srbct, X.test, 
                                dist = "mahalanobis.dist")

## -----------------------------------------------------------------------------
# Just the head:
head(data.frame(predict.splsda.srbct$class, Truth = Y.test))

## -----------------------------------------------------------------------------
# Compare prediction on the second component and change as factor
predict.comp2 <- predict.splsda.srbct$class$mahalanobis.dist[,2]
table(factor(predict.comp2, levels = levels(Y)), Y.test)

## -----------------------------------------------------------------------------
# Compare prediction on the third component and change as factor
predict.comp3 <- predict.splsda.srbct$class$mahalanobis.dist[,3]
table(factor(predict.comp3, levels = levels(Y)), Y.test)

## -----------------------------------------------------------------------------
# On component 3, just the head:
head(predict.splsda.srbct$predict[, , 3])

## ----05-splsda-auroc, fig.cap='(ref:05-splsda-auroc)', results='hold'---------
auc.srbct <- auroc(splsda.srbct)

## ----06-fig-path, echo = FALSE------------------------------------------------
knitr::opts_chunk$set(fig.path= 'Figures/N-Integration/')

## ----06-load-data, message=FALSE, warning=FALSE-------------------------------
library(mixOmics)
data(breast.TCGA)

# Extract training data and name each data frame
# Store as list
X <- list(mRNA = breast.TCGA$data.train$mrna, 
          miRNA = breast.TCGA$data.train$mirna, 
          protein = breast.TCGA$data.train$protein)

# Outcome
Y <- breast.TCGA$data.train$subtype
summary(Y)

## ----06-design----------------------------------------------------------------
design <- matrix(0.1, ncol = length(X), nrow = length(X), 
                dimnames = list(names(X), names(X)))
diag(design) <- 0
design 

## ----06-pls, results='hold'---------------------------------------------------
res1.pls.tcga <- pls(X$mRNA, X$protein, ncomp = 1)
cor(res1.pls.tcga$variates$X, res1.pls.tcga$variates$Y)

res2.pls.tcga <- pls(X$mRNA, X$miRNA, ncomp = 1)
cor(res2.pls.tcga$variates$X, res2.pls.tcga$variates$Y)

res3.pls.tcga <- pls(X$protein, X$miRNA, ncomp = 1)
cor(res3.pls.tcga$variates$X, res3.pls.tcga$variates$Y)

## ----06-ncomp, message=FALSE, fig.cap='(ref:06-ncomp)'------------------------
diablo.tcga <- block.plsda(X, Y, ncomp = 5, design = design)

set.seed(123) # For reproducibility, remove for your analyses
perf.diablo.tcga = perf(diablo.tcga, validation = 'Mfold', folds = 10, nrepeat = 10)

#perf.diablo.tcga$error.rate  # Lists the different types of error rates

# Plot of the error rates based on weighted vote
plot(perf.diablo.tcga)

## -----------------------------------------------------------------------------
perf.diablo.tcga$choice.ncomp$WeightedVote

## -----------------------------------------------------------------------------
ncomp <- perf.diablo.tcga$choice.ncomp$WeightedVote["Overall.BER", "centroids.dist"]

## ----06-tuning, eval = FALSE, warning=FALSE-----------------------------------
#  # chunk takes about 2 min to run
#  set.seed(123) # for reproducibility
#  test.keepX <- list(mRNA = c(5:9, seq(10, 25, 5)),
#                     miRNA = c(5:9, seq(10, 20, 2)),
#                     proteomics = c(seq(5, 25, 5)))
#  
#  tune.diablo.tcga <- tune.block.splsda(X, Y, ncomp = 2,
#                                test.keepX = test.keepX, design = design,
#                                validation = 'Mfold', folds = 10, nrepeat = 1,
#                                BPPARAM = BiocParallel::SnowParam(workers = 2),
#                                dist = "centroids.dist")
#  
#  list.keepX <- tune.diablo.tcga$choice.keepX

## ---- echo = FALSE------------------------------------------------------------
test.keepX <- list(mRNA = c(5:9, seq(10, 25, 5)),
                   miRNA = c(5:9, seq(10, 20, 2)),
                   proteomics = c(seq(5, 25, 5)))
list.keepX <- list( mRNA = c(8, 25), miRNA = c(14,5), protein = c(10, 5))

## -----------------------------------------------------------------------------
list.keepX

## ---- eval = FALSE------------------------------------------------------------
#  list.keepX <- list( mRNA = c(8, 25), miRNA = c(14,5), protein = c(10, 5))

## ----06-final, message = TRUE-------------------------------------------------
diablo.tcga <- block.splsda(X, Y, ncomp = ncomp, 
                            keepX = list.keepX, design = design)
#06.tcga   # Lists the different functions of interest related to that object

## -----------------------------------------------------------------------------
diablo.tcga$design

## ---- eval = FALSE------------------------------------------------------------
#  # mRNA variables selected on component 1
#  selectVar(diablo.tcga, block = 'mRNA', comp = 1)

## ----06-diablo-plot, message=FALSE, fig.cap='(ref:06-diablo-plot)'------------
plotDiablo(diablo.tcga, ncomp = 1)

## ----06-sample-plot, message=FALSE, fig.cap='(ref:06-sample-plot)'------------
plotIndiv(diablo.tcga, ind.names = FALSE, legend = TRUE, 
          title = 'TCGA, DIABLO comp 1 - 2')

## ----06-arrow-plot, message=FALSE, fig.cap='(ref:06-arrow-plot)'--------------
plotArrow(diablo.tcga, ind.names = FALSE, legend = TRUE, 
          title = 'TCGA, DIABLO comp 1 - 2')

## ----06-correlation-plot, message=FALSE, fig.cap='(ref:06-correlation-plot)'----
plotVar(diablo.tcga, var.names = FALSE, style = 'graphics', legend = TRUE, 
        pch = c(16, 17, 15), cex = c(2,2,2), 
        col = c('darkorchid', 'brown1', 'lightgreen'),
        title = 'TCGA, DIABLO comp 1 - 2')

## ----06-circos-plot, message=FALSE, fig.cap='(ref:06-circos-plot)'------------
circosPlot(diablo.tcga, cutoff = 0.7, line = TRUE, 
           color.blocks = c('darkorchid', 'brown1', 'lightgreen'),
           color.cor = c("chocolate3","grey20"), size.labels = 1.5)

## ----06-network, fig.cap='(ref:06-network)'-----------------------------------
# X11()   # Opens a new window
network(diablo.tcga, blocks = c(1,2,3), 
        cutoff = 0.4,
        color.node = c('darkorchid', 'brown1', 'lightgreen'),
        # To save the plot, uncomment below line
        #save = 'png', name.save = 'diablo-network'
        )

## ----eval = FALSE-------------------------------------------------------------
#  # Not run
#  library(igraph)
#  myNetwork <- network(diablo.tcga, blocks = c(1,2,3), cutoff = 0.4)
#  write.graph(myNetwork$gR, file = "myNetwork.gml", format = "gml")

## ----06-loading-plot, message=FALSE, fig.cap='(ref:06-loading-plot)'----------
plotLoadings(diablo.tcga, comp = 1, contrib = 'max', method = 'median')

## ----06-cim, message=FALSE, out.width = '70%', fig.cap='(ref:06-cim)'---------
cimDiablo(diablo.tcga, color.blocks = c('darkorchid', 'brown1', 'lightgreen'),
          comp = 1, margin=c(8,20), legend.position = "right")

## ----06-perf------------------------------------------------------------------
set.seed(123) # For reproducibility with this handbook, remove otherwise
perf.diablo.tcga <- perf(diablo.tcga,  validation = 'Mfold', folds = 10, 
                         nrepeat = 10, dist = 'centroids.dist')

#perf.diablo.tcga  # Lists the different outputs

## -----------------------------------------------------------------------------
# Performance with Majority vote
perf.diablo.tcga$MajorityVote.error.rate

## -----------------------------------------------------------------------------
# Performance with Weighted vote
perf.diablo.tcga$WeightedVote.error.rate

## ----06-auroc, message=FALSE, fig.cap='(ref:06-auroc)'------------------------
auc.diablo.tcga <- auroc(diablo.tcga, roc.block = "miRNA", roc.comp = 2,
                   print = FALSE)

## ----06-predict, message = FALSE----------------------------------------------
# Prepare test set data: here one block (proteins) is missing
data.test.tcga <- list(mRNA = breast.TCGA$data.test$mrna, 
                      miRNA = breast.TCGA$data.test$mirna)

predict.diablo.tcga <- predict(diablo.tcga, newdata = data.test.tcga)
# The warning message will inform us that one block is missing

#predict.diablo # List the different outputs

## -----------------------------------------------------------------------------
confusion.mat.tcga <- get.confusion_matrix(truth = breast.TCGA$data.test$subtype, 
                     predicted = predict.diablo.tcga$WeightedVote$centroids.dist[,2])
confusion.mat.tcga

## -----------------------------------------------------------------------------
get.BER(confusion.mat.tcga)

## ----07-fig-path, echo = FALSE------------------------------------------------
knitr::opts_chunk$set(fig.path= 'Figures/N-Integration/')

## ----07-load-data, message=FALSE, warning=FALSE-------------------------------
library(mixOmics)
data(stemcells)

# The combined data set X
X <- stemcells$gene
dim(X)

# The outcome vector Y:  
Y <- stemcells$celltype 
length(Y) 

summary(Y)

## -----------------------------------------------------------------------------
study <- stemcells$study

# Number of samples per study:
summary(study)

# Experimental design
table(Y,study)

## ----07-plsda-perf,fig.cap='(ref:07-plsda-perf)'------------------------------
mint.plsda.stem <- mint.plsda(X = X, Y = Y, study = study, ncomp = 5)

set.seed(2543) # For reproducible results here, remove for your own analyses
perf.mint.plsda.stem <- perf(mint.plsda.stem) 

plot(perf.mint.plsda.stem)

## ---- results = 'hold'--------------------------------------------------------
perf.mint.plsda.stem$global.error$BER
# Type also:
# perf.mint.plsda.stem$global.error

## ----07-plsda-sample-plot1, fig.cap='(ref:07-plsda-sample-plot1)'-------------
final.mint.plsda.stem <- mint.plsda(X = X, Y = Y, study = study, ncomp = 2)

#final.mint.plsda.stem # Lists the different functions

plotIndiv(final.mint.plsda.stem, legend = TRUE, title = 'MINT PLS-DA', 
          subtitle = 'stem cell study', ellipse = T)

## ----07-plsda-sample-plot2, fig.cap='(ref:07-plsda-sample-plot2)'-------------
plsda.stem <- plsda(X = X, Y = Y, ncomp = 2)

plotIndiv(plsda.stem, pch = study,
          legend = TRUE, title = 'Classic PLS-DA',
          legend.title = 'Cell type', legend.title.pch = 'Study')

## ----07-splsda-tuning---------------------------------------------------------
set.seed(2543)  # For a reproducible result here, remove for your own analyses
tune.mint.splsda.stem <- tune(X = X, Y = Y, study = study, 
                 ncomp = 2, test.keepX = seq(1, 100, 1),
                 method = 'mint.splsda', #Specify the method
                 measure = 'BER',
                 dist = "centroids.dist",
                 nrepeat = 3)

#tune.mint.splsda.stem # Lists the different types of outputs

# Mean error rate per component and per tested keepX value:
#tune.mint.splsda.stem$error.rate[1:5,]

## -----------------------------------------------------------------------------
tune.mint.splsda.stem$choice.keepX

## ----07-splsda-tuning-plot, fig.cap='(ref:07-splsda-tuning-plot)'-------------
plot(tune.mint.splsda.stem)

## ----07-splsda-final----------------------------------------------------------
final.mint.splsda.stem <- mint.splsda(X = X, Y = Y, study = study, ncomp = 2,  
                              keepX = tune.mint.splsda.stem$choice.keepX)

#mint.splsda.stem.final # Lists useful functions that can be used with a MINT object

## ----07-splsda-sample-plot1, echo = TRUE, results = 'hide', fig.show = 'hide', fig.path = 'Figures/MINT/'----
plotIndiv(final.mint.splsda.stem, study = 'global', legend = TRUE, 
          title = 'Stem cells, MINT sPLS-DA', 
          subtitle = 'Global', ellipse = T)

## ----07-splsda-sample-plot2, echo = TRUE, results = 'hide', fig.show = 'hide', fig.path = 'Figures/MINT/'----
plotIndiv(final.mint.splsda.stem, study = 'all.partial', legend = TRUE, 
          title = 'Stem cells, MINT sPLS-DA', 
          subtitle = paste("Study",1:4))

## ----07-splsda-sample-plots, fig.cap='(ref:07-splsda-sample-plots)', echo=FALSE, fig.align='center', out.width = '50%', fig.height=4, fig.ncol = 2, fig.subcap=c('', '')----
knitr::include_graphics(c('Figures/MINT/07-splsda-sample-plot1-1.png', 'Figures/MINT/07-splsda-sample-plot2-1.png'))

## ----07-splsda-correlation-plot1, echo = TRUE---------------------------------
plotVar(final.mint.splsda.stem)

## ----07-splsda-correlation-plot2, fig.cap='(ref:07-splsda-correlation-plot2)', echo = FALSE----
output <- plotVar(final.mint.splsda.stem, cex = 2, plot = FALSE)
col.var <- rep(color.mixo(1), ncol(X))
names(col.var) = colnames(X)
col.var[rownames(output)[output$x > 0.8]] <- color.mixo(4)
col.var[rownames(output)[output$x < (-0.8)]] <- color.mixo(5)

plotVar(final.mint.splsda.stem, cex = 2, col= list(col.var))

## ----07-splsda-cim, fig.cap='(ref:07-splsda-cim)', fig.width=10, fig.height=8----
# If facing margin issues, use either X11() or save the plot using the
# arguments save and name.save
cim(final.mint.splsda.stem, comp = 1, margins=c(10,5), 
    row.sideColors = color.mixo(as.numeric(Y)), row.names = FALSE,
    title = "MINT sPLS-DA, component 1")

## ----07-splsda-network, fig.cap='(ref:07-splsda-network)',fig.width=10, fig.height=8----
# If facing margin issues, use either X11() or save the plot using the
# arguments save and name.save
network(final.mint.splsda.stem, comp = 1,
        color.node = c(color.mixo(1), color.mixo(2)), 
        shape.node = c("rectangle", "circle"))

## -----------------------------------------------------------------------------
# Just a head
head(selectVar(final.mint.plsda.stem, comp = 1)$value)

## ----07-splsda-loading-plot, fig.cap='(ref:07-splsda-loading-plot)'-----------
plotLoadings(final.mint.splsda.stem, contrib = "max", method = 'mean', comp=1, 
             study="all.partial", title="Contribution on comp 1", 
             subtitle = paste("Study",1:4))

## ----07-splsda-perf-----------------------------------------------------------
set.seed(123)  # For reproducible results here, remove for your own study
perf.mint.splsda.stem.final <- perf(final.mint.plsda.stem, dist = 'centroids.dist')

perf.mint.splsda.stem.final$global.error

## ----07-splsda-auroc1, echo = TRUE, results = 'hide', fig.show = 'hide', fig.path = 'Figures/MINT/'----
auroc(final.mint.splsda.stem, roc.comp = 1)

## ----07-splsda-auroc2, echo = TRUE, results = 'hide', fig.show = 'hide', fig.path = 'Figures/MINT/'----
auroc(final.mint.splsda.stem, roc.comp = 1, roc.study = '2')

## ----07-splsda-auroc-plots, fig.cap='(ref:07-splsda-auroc-plots)', echo=FALSE, fig.align='center', out.width = '50%', fig.height=4, fig.ncol = 2, fig.subcap=c('', '')----
knitr::include_graphics(c('Figures/MINT/07-splsda-auroc1-1.png', 'Figures/MINT/07-splsda-auroc2-1.png'))

## ----07-splsda-predict--------------------------------------------------------
# We predict on study 3
indiv.test <- which(study == "3")

# We train on the remaining studies, with pre-tuned parameters
mint.splsda.stem2 <- mint.splsda(X = X[-c(indiv.test), ], 
                                Y = Y[-c(indiv.test)], 
                                study = droplevels(study[-c(indiv.test)]), 
                                ncomp = 1,  
                                keepX = 30)

mint.predict.stem <- predict(mint.splsda.stem2, newdata = X[indiv.test, ], 
                        dist = "centroids.dist",
                        study.test = factor(study[indiv.test]))

# Store class prediction with a model with 1 comp
indiv.prediction <- mint.predict.stem$class$centroids.dist[, 1]

# The confusion matrix compares the real subtypes with the predicted subtypes
conf.mat <- get.confusion_matrix(truth = Y[indiv.test],
                     predicted = indiv.prediction)

conf.mat

## -----------------------------------------------------------------------------
# Prediction error rate
(sum(conf.mat) - sum(diag(conf.mat)))/sum(conf.mat)

## ----08-mixOmics-version, echo = FALSE----------------------------------------
suppressMessages(library(mixOmics))
print(packageVersion("mixOmics"))

## ----08-sessionInfo, echo = FALSE---------------------------------------------
sessionInfo()

