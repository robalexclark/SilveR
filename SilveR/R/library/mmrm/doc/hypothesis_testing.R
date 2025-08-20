## -----------------------------------------------------------------------------
library(mmrm)
fit <- mmrm(FEV1 ~ ARMCD + RACE + ARMCD * RACE + ar1(AVISIT | USUBJID), data = fev_data)

## -----------------------------------------------------------------------------
x <- component(fit, "x_matrix")
x0 <- x[, c(1, 2)]
x1 <- x[, c(3, 4)]
x2 <- x[, c(5, 6)]
m <- diag(rep(1, nrow(x))) - x0 %*% solve(t(x0) %*% x0) %*% t(x0) # solve is used because the matrix is inversible
sub_mat <- solve(t(x1) %*% m %*% x1) %*% t(x1) %*% m %*% x2
sub_mat

## ----eval=FALSE---------------------------------------------------------------
# fit <- mmrm(FEV1 ~ ARMCD + AVISIT + ARMCD * AVISIT + ar1(AVISIT | USUBJID), data = fev_data)
# Anova(fit)

