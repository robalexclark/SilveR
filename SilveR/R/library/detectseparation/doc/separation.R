## ----setup, include = FALSE---------------------------------------------------
knitr::opts_chunk$set(
  collapse = TRUE,
  comment = "#>",
  fig.width = 6,
  fig.height = 6
)

## ---- echo = TRUE, eval = TRUE------------------------------------------------
library("detectseparation")
data("endometrial", package = "detectseparation")
endo_glm <- glm(HG ~ NV + PI + EH, family = binomial(), data = endometrial)
theta_mle <- coef(endo_glm)
summary(endo_glm)

## ---- echo = TRUE, eval = TRUE------------------------------------------------
(inf_check <- check_infinite_estimates(endo_glm))
plot(inf_check)

## ---- eval = TRUE, echo = TRUE------------------------------------------------
endo_sep <- glm(HG ~ NV + PI + EH, data = endometrial,
                family = binomial("logit"),
                method = "detect_separation")
endo_sep

## ---- echo = TRUE,  eval = TRUE-----------------------------------------------
coef(endo_glm) + coef(endo_sep)

## ---- echo = TRUE,  eval = TRUE-----------------------------------------------
coef(summary(endo_glm))[, "Std. Error"] + abs(coef(endo_sep))

## ---- echo = TRUE, eval = TRUE------------------------------------------------
update(endo_sep, solver = "glpk")

## ----  echo = TRUE, eval = TRUE-----------------------------------------------
update(endo_sep, implementation = "lpSolveAPI")

## ---- echo = TRUE, eval = TRUE------------------------------------------------
library("brglm2")
summary(update(endo_glm, method = "brglm_fit"))

