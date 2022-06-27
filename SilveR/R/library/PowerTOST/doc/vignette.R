## ---- include = FALSE---------------------------------------------------------
knitr::opts_chunk$set(
  collapse = TRUE,
  comment = "#"
)

## ----rounding, echo = FALSE---------------------------------------------------
library(PowerTOST)
res <- data.frame(CV = c(rep(0.574, 3), rep(0.1, 2)),
                  regulator = c("EMA", "HC", "RU/EEU/GGC", "EMA", "HC"),
                  method = c(rep("ABEL", 2), rep("ABE", 3)),
                  L = NA, U = NA, n.GL = NA, theta1 = NA, theta2 = NA, n = NA,
                  stringsAsFactors = FALSE) # this line for R <4.0.0
for (i in 1:nrow(res)) {
  if (i <= 2) {
    LU <- scABEL(CV = res$CV[i], regulator = res$regulator[i])
    if (i == 1) {
      res[i, 4:5] <- sprintf("%.2f%%", 100*LU)
      res[i, c(6, 9)] <- sampleN.scABEL(CV = res$CV[i], regulator = res$regulator[i],
                                        design = "2x2x4", print = FALSE,
                                        details = FALSE)[["Sample size"]]
      res[i, 7:8] <- sprintf("%.6f", LU)
    } else {
      res[i, 4:5] <- sprintf("%.1f %%", 100*LU)
      res[i, 7:8] <- sprintf("%.6f", LU)
      res[i, c(6, 9)] <- sampleN.scABEL(CV = res$CV[i], regulator = res$regulator[i],
                                        design = "2x2x4", print = FALSE,
                                        details = FALSE)[["Sample size"]]
    }
  }
  if (i == 3) {
    LU <- c(0.75, 1/0.75)
    res[i, 4:5] <- sprintf("%.2f%%", 100*LU)
    res[i, 6]   <- sampleN.TOST(CV = res$CV[i], theta1 = round(LU[1], 4),
                                theta2 = round(LU[2], 4), design = "2x2x4",
                                print = FALSE, details = FALSE)[["Sample size"]]
    res[i, 7:8] <- sprintf("%.6f", LU)
    res[i, 9]   <- sampleN.TOST(CV = res$CV[i], theta1 = LU[1], theta2 = LU[2],
                                design = "2x2x4", print = FALSE,
                                details = FALSE)[["Sample size"]]
  }
  if (i == 4) {
    LU <- c(0.9, 1/0.9)
    res[i, 4:5] <- sprintf("%.2f%%", 100*LU)
    res[i, 6]   <- sampleN.TOST(CV = res$CV[i], theta0 = 0.975,
                                theta1 = round(LU[1], 4), theta2 = round(LU[2], 4),
                                print = FALSE, details = FALSE)[["Sample size"]]
    res[i, 7:8] <- sprintf("%.6f", LU)
    res[i, 9]   <- sampleN.TOST(CV = res$CV[i], theta0 = 0.975, theta1 = LU[1],
                                theta2 = LU[2], print = FALSE,
                                details = FALSE)[["Sample size"]]
  }
  if (i == 5) {
    LU <- c(0.9, 1/0.9)
    res[i, 4:5] <- sprintf("%.1f %%", c(90, 112))
    res[i, 6]   <- sampleN.TOST(CV = res$CV[i], theta0 = 0.975,
                                theta1 = 0.9, theta2 = 1.12,
                                print = FALSE, details = FALSE)[["Sample size"]]
    res[i, 7:8] <- sprintf("%.6f", LU)
    res[i, 9]   <- sampleN.TOST(CV = res$CV[i], theta0 = 0.975, theta1 = LU[1],
                                theta2 = LU[2], print = FALSE,
                                details = FALSE)[["Sample size"]]
  }
}
names(res)[c(2, 4:6)] <- c("agency", "L   ", "U   ", "n")
print(res, row.names = FALSE)

## ----inst---------------------------------------------------------------------
package <- "PowerTOST"
inst    <- package %in% installed.packages()
if (length(package[!inst]) > 0) install.packages(package[!inst])

