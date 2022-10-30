## ----setup, include = FALSE---------------------------------------------------
knitr::opts_chunk$set(
  collapse = TRUE,
  comment = "#>",
  fig.width = 6,
  fig.height = 6
)

## ---- echo = TRUE, eval = TRUE------------------------------------------------
library("detectseparation")

## ---- tiny_example------------------------------------------------------------
data <- data.frame(a = c(1, 0, 3, 2, 3, 4),
                   b = c(2, 1, 1, 4, 6, 8),
                   y = c(0, 0, 0, 1, 1, 1))

## ---- tiny_example_sep--------------------------------------------------------
glm(y ~ a + b, data = data, family = binomial("logit"), method = "detect_separation")

## ---- tiny_example_sep_lbrm---------------------------------------------------
glm(y ~ a + b, data = data, family = binomial("log"), method = "detect_separation")

## ---- tiny_example_inf_est----------------------------------------------------
glm(y ~ a + b, data = data, family = binomial("logit"), method = "detect_infinite_estimates")

## -----------------------------------------------------------------------------
glm(y ~ a + b, data = data, family = binomial("log"), method = "detect_infinite_estimates")

## ---- glm_fit-----------------------------------------------------------------
fit <- try(glm(y ~ a + b, data = data, family = binomial("log")))

## -----------------------------------------------------------------------------
args(glm.control)

## ---- tiny_example_inf_esti_log_separation------------------------------------
formula <- y ~ a + b
start <- c(-1, double(ncol(model.matrix(formula, data = data)) - 1L))
ctrl = glm.control(epsilon = 1e-8, maxit = 10000, trace = FALSE)
suppressWarnings(
  fit <- glm(formula, data = data, family = binomial("log"), start = start, control = ctrl)
)
summary(fit)

## -----------------------------------------------------------------------------
print(mm <- drop(model.matrix(formula, data) %*% coef(fit)))
abs(drop(mm)) < 1e-6

## ---- explain_output_detect_separation----------------------------------------
glm(y ~ a + b, data = data, family = binomial("logit"), method = "detect_separation")

## ---- explain_output_detect_infinite_estimates--------------------------------
glm(y ~ a + b, data = data, family = binomial("log"), method = "detect_infinite_estimates")

## -----------------------------------------------------------------------------
find_start_simple <- function(formula, data) {
  c(-1, double(ncol(model.matrix(formula, data = data)) - 1L))  
}

find_start_simple(formula, data)
max(model.matrix(formula, data = data) %*% find_start_simple(formula, data))

## -----------------------------------------------------------------------------
find_start_poisson <- function(formula, data, delta = 1) {
  b0 <- coef(glm(formula, data, family = poisson(link = "log")))
  mX <- -model.matrix(formula, data = data)[, -1L, drop = FALSE]
  b0[1] <- min(mX %*% b0[-1]) - delta
  b0
}

find_start_poisson(formula, data)
max(model.matrix(formula, data = data) %*% find_start_poisson(formula, data))

