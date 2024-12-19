## ----fig=TRUE, echo=TRUE, message=FALSE---------------------------------------
suppressMessages(library(gplots))
venn( list(A=1:5,B=4:6,C=c(4,8:10)) )

## ----fig=TRUE, echo=TRUE, message=FALSE---------------------------------------
v.table <- venn( list(A=1:5,B=4:6,C=c(4,8:10),D=c(4:12)) )
print(v.table)

## ----fig=TRUE, echo=TRUE, message=FALSE---------------------------------------
venn( list(A=1:5,B=4:6,C=c(4,8:10),D=c(4:12),E=c(2,4,6:9)) )

