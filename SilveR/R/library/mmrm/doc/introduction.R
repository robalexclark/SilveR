## ----include = FALSE----------------------------------------------------------
knitr::opts_chunk$set(
  collapse = TRUE,
  comment = "#>"
)
library(mmrm)

## ----getting-started----------------------------------------------------------
library(mmrm)
fit <- mmrm(
  formula = FEV1 ~ RACE + SEX + ARMCD * AVISIT + us(AVISIT | USUBJID),
  data = fev_data
)

## ----print--------------------------------------------------------------------
fit

## ----summary------------------------------------------------------------------
summary(fit)

## ----include = FALSE----------------------------------------------------------
library(mmrm)

## ----low-level-control, results = 'hide'--------------------------------------
mmrm_control(
  method = "Kenward-Roger",
  optimizer = c("L-BFGS-B", "BFGS"),
  n_cores = 2,
  start = c(0, 1, 1, 0, 1, 0),
  accept_singular = FALSE,
  drop_visit_levels = FALSE
)

## ----data_generation----------------------------------------------------------
gen_data <- function(
    n = 100,
    mu = -100 / 52,
    delta = 50 / 52,
    mua = 2000,
    sigmaa = 300,
    sigmab = 60,
    corab = 0.2,
    sigma = 10,
    times = c(0, 2, 6, 12, 24, 36, 52, 70, 88, 104)) {
  nt <- length(times)
  out <- data.frame(
    pts = rep(seq_len(n * 2), each = nt),
    trt = rep(c("Treatment", "Placebo"), rep(n * nt, 2)),
    time = rep(times, n * 2)
  )

  covab <- corab * sigmaa * sigmab # cov between a and b
  cov <- matrix(c(sigmaa^2, covab, covab, sigmab^2), ncol = 2) # Cov matrix for the slope and intercept
  si <- rbind(
    MASS::mvrnorm(n, mu = c(mua, mu + delta), Sigma = cov),
    MASS::mvrnorm(n, mu = c(mua, mu + delta), Sigma = cov)
  )
  idx <- rep(seq_len(n * 2), each = nt)
  out$fev1 <- si[idx, 1] + si[idx, 2] * times + rnorm(n * nt * 2, sd = sigma)
  out$trt <- factor(out$trt)
  out$time <- factor(out$time)
  out$pts <- factor(out$pts)
  return(out)
}
set.seed(123)
out <- gen_data()

## ----show_variance------------------------------------------------------------
vapply(split(out$fev1, out$time), sd, FUN.VALUE = 1)

## ----mmrm_using_emp_start, eval = !mmrm:::is_r_devel_linux_clang()------------
mmrm(fev1 ~ trt * time + us(time | pts), data = out, start = emp_start)

## ----mmrm_using_std_start, eval = FALSE---------------------------------------
# mmrm(
#   fev1 ~ trt * time + us(time | pts),
#   data = out,
#   start = std_start,
#   optimizer = "nlminb",
#   optimizer_control = list(eval.max = 1000, iter.max = 1000)
# )

## ----common-changes-reml------------------------------------------------------
fit_ml <- mmrm(
  formula = FEV1 ~ RACE + ARMCD * AVISIT + us(AVISIT | USUBJID),
  data = fev_data,
  reml = FALSE
)
fit_ml

## ----common-changes-optim-----------------------------------------------------
fit_opt <- mmrm(
  formula = FEV1 ~ RACE + ARMCD * AVISIT + us(AVISIT | USUBJID),
  data = fev_data,
  optimizer = "BFGS"
)
fit_opt

## ----common-changes-cov-------------------------------------------------------
fit_cs <- mmrm(
  formula = FEV1 ~ RACE + ARMCD * AVISIT + cs(AVISIT | USUBJID),
  data = fev_data,
  reml = FALSE
)
fit_cs

## ----common-changes-weights---------------------------------------------------
fit_wt <- mmrm(
  formula = FEV1 ~ RACE + ARMCD * AVISIT + us(AVISIT | USUBJID),
  data = fev_data,
  weights = fev_data$WEIGHT
)
fit_wt

## ----group-cov----------------------------------------------------------------
fit_cs <- mmrm(
  formula = FEV1 ~ RACE + ARMCD * AVISIT + cs(AVISIT | ARMCD / USUBJID),
  data = fev_data,
  reml = FALSE
)
VarCorr(fit_cs)

## ----kr-----------------------------------------------------------------------
fit_kr <- mmrm(
  formula = FEV1 ~ RACE + ARMCD * AVISIT + us(AVISIT | USUBJID),
  data = fev_data,
  method = "Kenward-Roger"
)

## ----kr_summary---------------------------------------------------------------
summary(fit_kr)

## ----kr_lin-------------------------------------------------------------------
fit_kr_lin <- mmrm(
  formula = FEV1 ~ RACE + ARMCD * AVISIT + us(AVISIT | USUBJID),
  data = fev_data,
  method = "Kenward-Roger",
  vcov = "Kenward-Roger-Linear"
)

## -----------------------------------------------------------------------------
fit_emp <- mmrm(
  formula = FEV1 ~ RACE + ARMCD * AVISIT + us(AVISIT | USUBJID),
  data = fev_data,
  method = "Satterthwaite",
  vcov = "Empirical"
)

## ----sparse-------------------------------------------------------------------
sparse_data <- fev_data[fev_data$AVISIT != "VIS3", ]
sparse_result <- mmrm(
  FEV1 ~ RACE + ar1(AVISIT | USUBJID),
  data = sparse_data,
  drop_visit_levels = FALSE
)

dropped_result <- mmrm(
  FEV1 ~ RACE + ar1(AVISIT | USUBJID),
  data = sparse_data
)

## ----sparse_cor---------------------------------------------------------------
cov2cor(VarCorr(sparse_result))
cov2cor(VarCorr(dropped_result))

## ----include = FALSE----------------------------------------------------------
library(mmrm)

## ----extraction-summary-fit---------------------------------------------------
fit <- mmrm(
  formula = FEV1 ~ RACE + ARMCD * AVISIT + us(AVISIT | USUBJID),
  data = fev_data
)
fit_summary <- summary(fit)

## ----extraction-summary-fit-coef----------------------------------------------
fit_summary$coefficients

## ----extraction-summary-fit-str-----------------------------------------------
str(fit_summary)

## ----residuals-response-------------------------------------------------------
fit <- mmrm(
  formula = FEV1 ~ RACE + ARMCD * AVISIT + us(AVISIT | USUBJID),
  data = fev_data
)
residuals_resp <- residuals(fit, type = "response")

## ----residuals-pearson--------------------------------------------------------
residuals_pearson <- residuals(fit, type = "pearson")

## ----residuals-norm-----------------------------------------------------------
residuals_norm <- residuals(fit, type = "normalized")

## -----------------------------------------------------------------------------
fit |>
  tidy()

## -----------------------------------------------------------------------------
fit |>
  tidy(conf.int = TRUE, conf.level = 0.9)

## -----------------------------------------------------------------------------
fit |>
  glance()

## -----------------------------------------------------------------------------
fit |>
  augment()

## -----------------------------------------------------------------------------
fit |>
  augment(interval = "confidence", type.residuals = "normalized")

## ----extraction-summary-component---------------------------------------------
component(fit, name = c("convergence", "evaluations", "conv_message"))

## -----------------------------------------------------------------------------
component(fit, name = "call")

## ----eval=FALSE---------------------------------------------------------------
# component(fit)

## ----include = FALSE----------------------------------------------------------
library(mmrm)

## ----low-level-hmmrmtmb-------------------------------------------------------
fit_mmrm(
  formula = FEV1 ~ RACE + ARMCD * AVISIT + us(AVISIT | USUBJID),
  data = fev_data,
  weights = rep(1, nrow(fev_data)),
  reml = TRUE,
  control = mmrm_control()
)

## ----include = FALSE----------------------------------------------------------
library(mmrm)

## ----1d_satterthwaite---------------------------------------------------------
fit <- mmrm(
  formula = FEV1 ~ RACE + SEX + ARMCD * AVISIT + us(AVISIT | USUBJID),
  data = fev_data
)

contrast <- numeric(length(component(fit, "beta_est")))
contrast[3] <- 1

df_1d(fit, contrast)

## ----1d_kr--------------------------------------------------------------------
fit_kr <- mmrm(
  formula = FEV1 ~ RACE + SEX + ARMCD * AVISIT + us(AVISIT | USUBJID),
  data = fev_data,
  method = "Kenward-Roger"
)

df_1d(fit_kr, contrast)

## ----md_satterthwaite---------------------------------------------------------
contrast <- matrix(data = 0, nrow = 2, ncol = length(component(fit, "beta_est")))
contrast[1, 2] <- contrast[2, 3] <- 1

df_md(fit, contrast)

## ----md_kr--------------------------------------------------------------------
df_md(fit_kr, contrast)

## ----emmeans------------------------------------------------------------------
library(emmeans)
lsmeans_by_visit <- emmeans(fit, ~ ARMCD | AVISIT)
lsmeans_by_visit

## ----pdiff--------------------------------------------------------------------
pairs(lsmeans_by_visit, reverse = TRUE)

## ----pdiffci------------------------------------------------------------------
confint(pairs(lsmeans_by_visit, reverse = TRUE))

## ----car_type2----------------------------------------------------------------
library(car)
Anova(fit, type = "II")

## ----car_type3----------------------------------------------------------------
Anova(fit, type = "III")

## ----include = FALSE----------------------------------------------------------
library(mmrm)

## ----warning=FALSE, message=FALSE---------------------------------------------
library(tidymodels)

## -----------------------------------------------------------------------------
model <- linear_reg() |>
  set_engine("mmrm", method = "Satterthwaite") |>
  fit(FEV1 ~ RACE + ARMCD * AVISIT + us(AVISIT | USUBJID), fev_data)
model

## -----------------------------------------------------------------------------
model_with_control <- linear_reg() |>
  set_engine("mmrm", control = mmrm_control(method = "Satterthwaite")) |>
  fit(FEV1 ~ RACE + ARMCD * AVISIT + us(AVISIT | USUBJID), fev_data)

## -----------------------------------------------------------------------------
predict(model, new_data = fev_data)

## -----------------------------------------------------------------------------
predict(
  model,
  new_data = fev_data,
  type = "raw",
  opts = list(se.fit = TRUE, interval = "prediction", nsim = 10L)
)

## -----------------------------------------------------------------------------
augment(model, new_data = fev_data) |>
  select(USUBJID, AVISIT, .resid, .pred)

## -----------------------------------------------------------------------------
mmrm_spec <- linear_reg() |>
  set_engine("mmrm", method = "Satterthwaite")

mmrm_wflow <- workflow() |>
  add_variables(outcomes = FEV1, predictors = c(RACE, ARMCD, AVISIT, USUBJID)) |>
  add_model(mmrm_spec, formula = FEV1 ~ RACE + ARMCD * AVISIT + us(AVISIT | USUBJID))

mmrm_wflow |>
  fit(data = fev_data)

## -----------------------------------------------------------------------------
mmrm_recipe <- recipe(FEV1 ~ ., data = fev_data) |>
  step_dummy(ARMCD) |>
  step_interact(terms = ~ starts_with("ARMCD"):AVISIT)

## -----------------------------------------------------------------------------
mmrm_recipe |>
  prep() |>
  juice()

## -----------------------------------------------------------------------------
mmrm_spec_with_cov <- linear_reg() |>
  set_engine(
    "mmrm",
    method = "Satterthwaite",
    covariance = as.cov_struct(~ us(AVISIT | USUBJID))
  )

## -----------------------------------------------------------------------------
(mmrm_wflow_nocov <- workflow() |>
  add_model(mmrm_spec_with_cov, formula = FEV1 ~ SEX) |>
  add_recipe(mmrm_recipe))

## -----------------------------------------------------------------------------
(fit_tidy <- fit(mmrm_wflow_nocov, data = fev_data))

## -----------------------------------------------------------------------------
fit_tidy |>
  hardhat::extract_fit_engine()

