## ----include = FALSE----------------------------------------------------------
knitr::opts_chunk$set(
  collapse = TRUE,
  comment = "#>"
)
library(mmrm)

## -----------------------------------------------------------------------------
fit <- mmrm(
  formula = FEV1 ~ RACE + SEX + ARMCD * AVISIT + us(AVISIT | USUBJID),
  data = fev_data,
  control = mmrm_control(method = "Between-Within")
)
summary(fit)

## -----------------------------------------------------------------------------
head(model.matrix(fit), 1)

