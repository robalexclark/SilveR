## ----style, echo = FALSE, results = 'asis'------------------------------------
BiocStyle::markdown()

## ----setup, include = FALSE---------------------------------------------------

library(knitr)
# global options
knitr::opts_chunk$set(dpi = 100, echo=TRUE, warning=FALSE, message=FALSE, eval = TRUE,
                      fig.show=TRUE, fig.width= 7,fig.height= 6,fig.align='center', out.width = '80%'#, 
                      #fig.path= 'Figures/'
                      )

# knitr::opts_chunk$set(
#   collapse = TRUE,
#   comment = "#>"
# )

## ----overview, echo=FALSE, message=FALSE--------------------------------------
library(mixOmics)
coul <- color.mixo(1:3)

plot(0, type="n", xlim=c(0,100), ylim=c(0,100), axes=FALSE,
     xlab="",ylab="", main="mixOmics overview")
box()

# PCA
rect(xleft = 20, ybottom = 75, xright = 40, ytop = 95, col=coul[1])
text(5, 85, "PCA")

# PLS-DA
rect(xleft = 20, ybottom = 50, xright = 40, ytop = 70, col=coul[1])
rect(xleft = 43, ybottom = 50, xright = 45, ytop = 70, col=coul[2])
text(5, 60, "PLS-DA")

# PLS
rect(xleft = 20, ybottom = 25, xright = 40, ytop = 45, col=coul[1])
rect(xleft = 43, ybottom = 25, xright = 60, ytop = 45, col=coul[1])
text(5, 35, "PLS")

# DIABLO
rect(xleft = 20, ybottom = 0, xright = 40, ytop = 20, col=coul[1])
rect(xleft = 43, ybottom = 0, xright = 60, ytop = 20, col=coul[1])
points(x=61, y=10, pch=16, col=coul[3], cex=0.5)
points(x=62.5, y=10, pch=16, col=coul[3], cex=0.5)
points(x=64, y=10, pch=16, col=coul[3], cex=0.5)
rect(xleft = 65, ybottom = 0, xright = 80, ytop = 20, col=coul[1])
rect(xleft = 85, ybottom = 0, xright = 88, ytop = 20, col=coul[2])
text(5, 10, "DIABLO")

# legend
rect(xleft = 75, ybottom = 95, xright = 77, ytop = 97, col=coul[1])
text(90, 96, "Quantitative", cex=0.75)
rect(xleft = 75, ybottom = 90, xright = 77, ytop = 92, col=coul[2])
text(90, 91, "Qualitative", cex=0.75)

## ----methods, echo=FALSE, fig.cap="List of methods in mixOmics, sparse indicates methods that perform variable selection", out.width='100%', fig.align='center'----
knitr::include_graphics("XtraFigs/Methods.png")

## ----cheatsheet, echo=FALSE, fig.cap="Main functions and parameters of each method", out.width= '100%', fig.align='center'----
knitr::include_graphics("XtraFigs/cheatsheet.png")

## ---- eval = FALSE------------------------------------------------------------
#  if (!requireNamespace("BiocManager", quietly = TRUE))
#      install.packages("BiocManager")
#   BiocManager::install("mixOmics")

## ---- eval = FALSE------------------------------------------------------------
#  install.packages("devtools")
#  # then load
#  library(devtools)
#  install_github("mixOmicsTeam/mixOmics")

## ---- message=FALSE-----------------------------------------------------------
library(mixOmics)

## ---- eval = FALSE------------------------------------------------------------
#  # from csv file
#  data <- read.csv("your_data.csv", row.names = 1, header = TRUE)
#  
#  # from txt file
#  data <- read.table("your_data.txt", header = TRUE)
#  
#  # then in the argument in the mixOmics functions, just fill with:
#  # X = data

## -----------------------------------------------------------------------------
data(nutrimouse)
X <- nutrimouse$gene

## -----------------------------------------------------------------------------
MyResult.pca <- pca(X)  # 1 Run the method
plotIndiv(MyResult.pca) # 2 Plot the samples
plotVar(MyResult.pca)   # 3 Plot the variables

## -----------------------------------------------------------------------------
MyResult.spca <- spca(X, keepX=c(5,5)) # 1 Run the method
plotIndiv(MyResult.spca)               # 2 Plot the samples
plotVar(MyResult.spca)                 # 3 Plot the variables

## ----overview-PCA, echo=FALSE, message=FALSE----------------------------------
library(mixOmics)
coul <- color.mixo(1:3)

plot(0, type="n", xlim=c(0,100), ylim=c(83,97), axes=FALSE,
     xlab="",ylab="", main="PCA overview")
box()

# PCA
rect(xleft = 20, ybottom = 85, xright = 60, ytop = 95, col=coul[1])
text(5, 90, "PCA")

# legend
rect(xleft = 85, ybottom = 90, xright = 87, ytop = 92, col=coul[1])
text(90, 94, "Quantitative", cex=0.75)
#rect(xleft = 75, ybottom = 90, xright = 77, ytop = 92, col=coul[2])
#text(90, 91, "Qualitative", cex=0.75)


## ----echo=TRUE, message=FALSE-------------------------------------------------
library(mixOmics)
data(liver.toxicity)
X <- liver.toxicity$gene

## ---- fig.show = 'hide'-------------------------------------------------------
MyResult.pca <- pca(X)     # 1 Run the method
plotIndiv(MyResult.pca)    # 2 Plot the samples
plotVar(MyResult.pca)      # 3 Plot the variables

## ----eval=TRUE, fig.show=FALSE------------------------------------------------
plotIndiv(MyResult.pca, group = liver.toxicity$treatment$Dose.Group, 
          legend = TRUE)

## -----------------------------------------------------------------------------
plotIndiv(MyResult.pca, ind.names = FALSE,
          group = liver.toxicity$treatment$Dose.Group,
          pch = as.factor(liver.toxicity$treatment$Time.Group),
          legend = TRUE, title = 'Liver toxicity: genes, PCA comp 1 - 2',
          legend.title = 'Dose', legend.title.pch = 'Exposure')

## ----eval=TRUE, fig.show=TRUE-------------------------------------------------
MyResult.pca2 <- pca(X, ncomp = 3)
plotIndiv(MyResult.pca2, comp = c(1,3), legend = TRUE,
          group = liver.toxicity$treatment$Time.Group,
          title = 'Multidrug transporter, PCA comp 1 - 3')

## -----------------------------------------------------------------------------
plot(MyResult.pca2)
MyResult.pca2

## ----eval=TRUE, fig.show = 'hide'---------------------------------------------
# a minimal example
plotLoadings(MyResult.pca)

## ----eval=TRUE, fig.show = 'hide'---------------------------------------------
# a customized example to only show the top 100 genes 
# and their gene name
plotLoadings(MyResult.pca, ndisplay = 100, 
             name.var = liver.toxicity$gene.ID[, "geneBank"],
             size.name = rel(0.3))

## ----eval= FALSE, fig.show = 'hide'-------------------------------------------
#  plotIndiv(MyResult.pca2,
#            group = liver.toxicity$treatment$Dose.Group, style="3d",
#            legend = TRUE, title = 'Liver toxicity: genes, PCA comp 1 - 2 - 3')

## -----------------------------------------------------------------------------
MyResult.spca <- spca(X, ncomp = 3, keepX = c(15,10,5))                 # 1 Run the method
plotIndiv(MyResult.spca, group = liver.toxicity$treatment$Dose.Group,   # 2 Plot the samples
          pch = as.factor(liver.toxicity$treatment$Time.Group),
          legend = TRUE, title = 'Liver toxicity: genes, sPCA comp 1 - 2',
          legend.title = 'Dose', legend.title.pch = 'Exposure')
plotVar(MyResult.spca, cex = 1)                                        # 3 Plot the variables
# cex is used to reduce the size of the labels on the plot

## -----------------------------------------------------------------------------
selectVar(MyResult.spca, comp = 1)$value

## -----------------------------------------------------------------------------
plotLoadings(MyResult.spca)

## ---- echo=TRUE,results='hide', fig.show=FALSE--------------------------------
selectVar(MyResult.spca, comp=2)$value
plotLoadings(MyResult.spca, comp = 2)

## ----eval=TRUE, echo=FALSE, results='hide'------------------------------------
tune.pca(X)

## ----overview-PLSDA, echo=FALSE, message=FALSE--------------------------------
library(mixOmics)
coul <- color.mixo(1:3)

plot(0, type="n", xlim=c(0,100), ylim=c(83,97), axes=FALSE,
     xlab="",ylab="", main="PLSDA overview")
box()

# PLS-DA
rect(xleft = 20, ybottom = 85, xright = 60, ytop = 95, col=coul[1])
rect(xleft = 63, ybottom = 85, xright = 65, ytop = 95, col=coul[2])
text(5, 90, "PLS-DA")


# legend
rect(xleft = 85, ybottom = 92, xright = 87, ytop = 94, col=coul[1])
text(90, 95, "Quantitative", cex=0.75)
rect(xleft = 85, ybottom = 87, xright = 87, ytop = 89, col=coul[2])
text(90, 90, "Qualitative", cex=0.75)


## ----echo=TRUE, message=FALSE-------------------------------------------------
library(mixOmics)
data(srbct)
X <- srbct$gene
Y <- srbct$class 
summary(Y)
dim(X); length(Y)

## ---- echo=TRUE,results='hide', fig.show = 'hide'-----------------------------
MyResult.splsda <- splsda(X, Y, keepX = c(50,50)) # 1 Run the method
plotIndiv(MyResult.splsda)                          # 2 Plot the samples
plotVar(MyResult.splsda)                            # 3 Plot the variables
selectVar(MyResult.splsda, comp=1)$name             # Selected variables on component 1

## ----fig.show='hide'----------------------------------------------------------
MyResult.plsda <- plsda(X,Y) # 1 Run the method
plotIndiv(MyResult.plsda)    # 2 Plot the samples
plotVar(MyResult.plsda)      # 3 Plot the variables

## -----------------------------------------------------------------------------
plotIndiv(MyResult.splsda, ind.names = FALSE, legend=TRUE,
          ellipse = TRUE, star = TRUE, title = 'sPLS-DA on SRBCT',
          X.label = 'PLS-DA 1', Y.label = 'PLS-DA 2')

## -----------------------------------------------------------------------------
plotVar(MyResult.splsda, var.names=FALSE)

## -----------------------------------------------------------------------------
plotVar(MyResult.plsda, cutoff=0.7)

## -----------------------------------------------------------------------------
background <- background.predict(MyResult.splsda, comp.predicted=2,
                                dist = "max.dist") 
plotIndiv(MyResult.splsda, comp = 1:2, group = srbct$class,
          ind.names = FALSE, title = "Maximum distance",
          legend = TRUE,  background = background)

## ---- echo=TRUE,results='hide', fig.keep='all'--------------------------------
auc.plsda <- auroc(MyResult.splsda)

## -----------------------------------------------------------------------------
MyResult.splsda2 <- splsda(X,Y, ncomp=3, keepX=c(15,10,5))

## ----eval=TRUE----------------------------------------------------------------
selectVar(MyResult.splsda2, comp=1)$value

## -----------------------------------------------------------------------------
plotLoadings(MyResult.splsda2, contrib = 'max', method = 'mean')

## ----eval=FALSE, fig.show='hide'----------------------------------------------
#  plotIndiv(MyResult.splsda2, style="3d")

## ----eval= TRUE---------------------------------------------------------------
MyResult.plsda2 <- plsda(X,Y, ncomp=10)
set.seed(30) # for reproducibility in this vignette, otherwise increase nrepeat
MyPerf.plsda <- perf(MyResult.plsda2, validation = "Mfold", folds = 3, 
                  progressBar = FALSE, nrepeat = 10) # we suggest nrepeat = 50

# type attributes(MyPerf.plsda) to see the different outputs
# slight bug in our output function currently see the quick fix below
#plot(MyPerf.plsda, col = color.mixo(5:7), sd = TRUE, legend.position = "horizontal")

# quick fix
matplot(MyPerf.plsda$error.rate$BER, type = 'l', lty = 1, 
        col = color.mixo(1:3), 
        main = 'Balanced Error rate')
legend('topright', 
       c('max.dist', 'centroids.dist', 'mahalanobis.dist'), 
       lty = 1,
       col = color.mixo(5:7))

## -----------------------------------------------------------------------------
MyPerf.plsda

## ----eval=TRUE----------------------------------------------------------------
list.keepX <- c(5:10,  seq(20, 100, 10))
list.keepX # to output the grid of values tested
set.seed(30) # for reproducbility in this vignette, otherwise increase nrepeat
tune.splsda.srbct <- tune.splsda(X, Y, ncomp = 3, # we suggest to push ncomp a bit more, e.g. 4
                                 validation = 'Mfold',
                                 folds = 3, dist = 'max.dist', progressBar = FALSE,
                                 measure = "BER", test.keepX = list.keepX,
                                 nrepeat = 10)   # we suggest nrepeat = 50

## -----------------------------------------------------------------------------
error <- tune.splsda.srbct$error.rate
ncomp <- tune.splsda.srbct$choice.ncomp$ncomp # optimal number of components based on t-tests on the error rate
ncomp
select.keepX <- tune.splsda.srbct$choice.keepX[1:ncomp]  # optimal number of variables to select
select.keepX
plot(tune.splsda.srbct, col = color.jet(ncomp))

## ---- fig.show = 'hide'-------------------------------------------------------
MyResult.splsda.final <- splsda(X, Y, ncomp = ncomp, keepX = select.keepX)
plotIndiv(MyResult.splsda.final, ind.names = FALSE, legend=TRUE,
          ellipse = TRUE, title="SPLS-DA, Final result")

## ----overview-PLS, echo=FALSE, message=FALSE----------------------------------
library(mixOmics)
coul <- color.mixo(1:3)

plot(0, type="n", xlim=c(0,100), ylim=c(83,97), axes=FALSE,
     xlab="",ylab="", main="PLS overview")
box()

# PLS
rect(xleft = 20, ybottom = 85, xright = 50, ytop = 95, col=coul[1])
rect(xleft = 52, ybottom = 85, xright = 73, ytop = 95, col=coul[1])
text(5, 90, "PLS")


# legend
rect(xleft = 85, ybottom = 92, xright = 87, ytop = 94, col=coul[1])
text(90, 95, "Quantitative", cex=0.75)


## ----echo=TRUE, message=FALSE-------------------------------------------------
library(mixOmics)
data(nutrimouse)
X <- nutrimouse$gene  
Y <- nutrimouse$lipid
dim(X); dim(Y)

## ----fig.show='hide'----------------------------------------------------------
MyResult.spls <- spls(X,Y, keepX = c(25, 25), keepY = c(5,5))  
plotIndiv(MyResult.spls)                                      
plotVar(MyResult.spls)                                        

## -----------------------------------------------------------------------------
plotIndiv(MyResult.spls, group = nutrimouse$genotype,
          rep.space = "XY-variate", legend = TRUE,
          legend.title = 'Genotype',
          ind.names = nutrimouse$diet,
          title = 'Nutrimouse: sPLS')

## -----------------------------------------------------------------------------
plotIndiv(MyResult.spls, group=nutrimouse$diet,
          pch = nutrimouse$genotype,
          rep.space = "XY-variate",  legend = TRUE,
          legend.title = 'Diet', legend.title.pch = 'Genotype',
          ind.names = FALSE, 
          title = 'Nutrimouse: sPLS')

## -----------------------------------------------------------------------------
plotVar(MyResult.spls, cex=c(3,2), legend = TRUE)
coordinates <- plotVar(MyResult.spls, plot = FALSE)

## ----eval= FALSE, fig.show = 'hide'-------------------------------------------
#  X11()
#  cim(MyResult.spls, comp = 1)
#  cim(MyResult.spls, comp = 1, save = 'jpeg', name.save = 'PLScim')

## ----eval=FALSE---------------------------------------------------------------
#  X11()
#  network(MyResult.spls, comp = 1)
#  network(MyResult.spls, comp = 1, cutoff = 0.6, save = 'jpeg', name.save = 'PLSnetwork')
#  # save as graph object for cytoscape
#  myNetwork <- network(MyResult.spls, comp = 1)$gR

## -----------------------------------------------------------------------------
plotArrow(MyResult.spls,group=nutrimouse$diet, legend = TRUE,
          X.label = 'PLS comp 1', Y.label = 'PLS comp 2', legend.title = 'Diet')

## -----------------------------------------------------------------------------
MySelectedVariables <- selectVar(MyResult.spls, comp = 1)
MySelectedVariables$X$name # Selected genes on component 1
MySelectedVariables$Y$name # Selected lipids on component 1

## -----------------------------------------------------------------------------
plotLoadings(MyResult.spls, comp = 1, size.name = rel(0.5))

## ----eval=TRUE----------------------------------------------------------------
MyResult.pls <- pls(X,Y, ncomp = 4)  
set.seed(30) # for reproducibility in this vignette, otherwise increase nrepeat
perf.pls <- perf(MyResult.pls, validation = "Mfold", folds = 5,
                  progressBar = FALSE, nrepeat = 10)
plot(perf.pls, criterion = 'Q2.total')

## ---- eval=TRUE, message='hide'-----------------------------------------------
list.keepX <- c(2:10, 15, 20)
# tuning based on correlations
set.seed(30) # for reproducibility in this vignette, otherwise increase nrepeat
tune.spls.cor <- tune.spls(X, Y, ncomp = 3,
                           test.keepX = list.keepX,
                           validation = "Mfold", folds = 5,
                           nrepeat = 10, progressBar = FALSE,
                           measure = 'cor')
plot(tune.spls.cor, measure = 'cor')

## -----------------------------------------------------------------------------
tune.spls.cor$choice.keepX

## -----------------------------------------------------------------------------
# tuning both X and Y
set.seed(30) # for reproducibility in this vignette, otherwise increase nrepeat
tune.spls.cor.XY <- tune.spls(X, Y, ncomp = 3,
                           test.keepX = c(8, 20, 50),
                           test.keepY = c(4, 8, 16),
                           validation = "Mfold", folds = 5,
                           nrepeat = 10, progressBar = FALSE,
                           measure = 'cor')
## visualise correlations
plot(tune.spls.cor.XY, measure = 'cor')
## visualise RSS
plot(tune.spls.cor.XY, measure = 'RSS')

## ----overview-DIABLO, echo=FALSE, message=FALSE-------------------------------
library(mixOmics)
coul <- color.mixo(1:3)

plot(0, type="n", xlim=c(0,100), ylim=c(83,97), axes=FALSE,
     xlab="",ylab="", main="DIABLO overview")
box()


rect(xleft = 12, ybottom = 85, xright = 30, ytop = 95, col=coul[1])
rect(xleft = 32, ybottom = 85, xright = 45, ytop = 95, col=coul[1])
points(x=47, y=90, pch=16, col='black', cex=0.5)
points(x=48.5, y=90, pch=16, col='black', cex=0.5)
points(x=50, y=90, pch=16, col='black', cex=0.5)
rect(xleft = 52, ybottom = 85, xright = 61, ytop = 95, col=coul[1])
rect(xleft = 63, ybottom = 85, xright = 65, ytop = 95, col=coul[2])
text(5, 90, "DIABLO")


# legend
rect(xleft = 85, ybottom = 92, xright = 87, ytop = 94, col=coul[1])
text(90, 95, "Quantitative", cex=0.75)
rect(xleft = 85, ybottom = 87, xright = 87, ytop = 89, col=coul[2])
text(90, 90, "Qualitative", cex=0.75)


## ----message=FALSE------------------------------------------------------------
library(mixOmics)
data(breast.TCGA)
# extract training data and name each data frame
X <- list(mRNA = breast.TCGA$data.train$mrna, 
          miRNA = breast.TCGA$data.train$mirna, 
          protein = breast.TCGA$data.train$protein)
Y <- breast.TCGA$data.train$subtype
summary(Y)

list.keepX <- list(mRNA = c(16, 17), miRNA = c(18,5), protein = c(5, 5))

## ---- fig.show = 'hide'-------------------------------------------------------
MyResult.diablo <- block.splsda(X, Y, keepX=list.keepX)
plotIndiv(MyResult.diablo)
plotVar(MyResult.diablo)

## ----eval= TRUE---------------------------------------------------------------
MyResult.diablo2 <- block.plsda(X, Y)

## -----------------------------------------------------------------------------
plotIndiv(MyResult.diablo, 
          ind.names = FALSE, 
          legend=TRUE, cex=c(1,2,3),
          title = 'BRCA with DIABLO')

## -----------------------------------------------------------------------------
plotVar(MyResult.diablo, var.names = c(FALSE, FALSE, TRUE),
        legend=TRUE, pch=c(16,16,1))

## -----------------------------------------------------------------------------
plotDiablo(MyResult.diablo, ncomp = 1)

## -----------------------------------------------------------------------------
circosPlot(MyResult.diablo, cutoff=0.7)

## ----eval=FALSE, fig.height=8, fig.width=8------------------------------------
#  # minimal example with margins improved:
#  # cimDiablo(MyResult.diablo, margin=c(8,20))
#  
#  # extended example:
#  cimDiablo(MyResult.diablo, color.blocks = c('darkorchid', 'brown1', 'lightgreen'), comp = 1, margin=c(8,20), legend.position = "right")

## -----------------------------------------------------------------------------
#plotLoadings(MyResult.diablo, contrib = "max")
plotLoadings(MyResult.diablo, comp = 2, contrib = "max")

## ---- eval = FALSE, fig.show = 'hide'-----------------------------------------
#  network(MyResult.diablo, blocks = c(1,2,3),
#          color.node = c('darkorchid', 'brown1', 'lightgreen'),
#          cutoff = 0.6, save = 'jpeg', name.save = 'DIABLOnetwork')

## ---- eval = TRUE-------------------------------------------------------------
set.seed(123) # for reproducibility in this vignette
MyPerf.diablo <- perf(MyResult.diablo, validation = 'Mfold', folds = 5, 
                   nrepeat = 10, 
                   dist = 'centroids.dist')

#MyPerf.diablo  # lists the different outputs

# Performance with Majority vote
#MyPerf.diablo$MajorityVote.error.rate

## ---- echo=TRUE,results='hide',fig.keep='all'---------------------------------
Myauc.diablo <- auroc(MyResult.diablo, roc.block = "miRNA", roc.comp = 2)

## -----------------------------------------------------------------------------
# prepare test set data: here one block (proteins) is missing
X.test <- list(mRNA = breast.TCGA$data.test$mrna, 
                      miRNA = breast.TCGA$data.test$mirna)

Mypredict.diablo <- predict(MyResult.diablo, newdata = X.test)
# the warning message will inform us that one block is missing
#Mypredict.diablo # list the different outputs

## -----------------------------------------------------------------------------
confusion.mat <- get.confusion_matrix(
                truth = breast.TCGA$data.test$subtype, 
                predicted = Mypredict.diablo$MajorityVote$centroids.dist[,2])
kable(confusion.mat)
get.BER(confusion.mat)

## -----------------------------------------------------------------------------
MyResult.diablo$design

## -----------------------------------------------------------------------------
MyDesign <- matrix(c(0, 0.1, 0.3,
                     0.1, 0, 0.9,
                     0.3, 0.9, 0),
                   byrow=TRUE,
                   ncol = length(X), nrow = length(X),
                 dimnames = list(names(X), names(X)))
MyDesign
MyResult.diablo.design <- block.splsda(X, Y, keepX=list.keepX, design=MyDesign)

## -----------------------------------------------------------------------------
sessionInfo()

