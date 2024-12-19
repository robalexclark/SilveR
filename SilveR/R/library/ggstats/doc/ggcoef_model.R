## ----include = FALSE----------------------------------------------------------
knitr::opts_chunk$set(
  collapse = TRUE,
  comment = "#>"
)

## ----setup--------------------------------------------------------------------
library(ggstats)

## ----include=FALSE------------------------------------------------------------
if (
  !broom.helpers::.assert_package("emmeans", boolean = TRUE)
) {
  knitr::opts_chunk$set(eval = FALSE)
}

## ----ggcoef-reg---------------------------------------------------------------
data(tips, package = "reshape")
mod_simple <- lm(tip ~ day + time + total_bill, data = tips)
ggcoef_model(mod_simple)

## ----ggcoef-titanic-----------------------------------------------------------
d_titanic <- as.data.frame(Titanic)
d_titanic$Survived <- factor(d_titanic$Survived, c("No", "Yes"))
mod_titanic <- glm(
  Survived ~ Sex * Age + Class,
  weights = Freq,
  data = d_titanic,
  family = binomial
)
ggcoef_model(mod_titanic, exponentiate = TRUE)

## -----------------------------------------------------------------------------
library(labelled)
tips_labelled <- tips |>
  set_variable_labels(
    day = "Day of the week",
    time = "Lunch or Dinner",
    total_bill = "Bill's total"
  )
mod_labelled <- lm(tip ~ day + time + total_bill, data = tips_labelled)
ggcoef_model(mod_labelled)

## -----------------------------------------------------------------------------
ggcoef_model(
  mod_simple,
  variable_labels = c(
    day = "Week day",
    time = "Time (lunch or dinner ?)",
    total_bill = "Total of the bill"
  )
)

## -----------------------------------------------------------------------------
ggcoef_model(
  mod_simple,
  variable_labels = c(
    day = "Week day",
    time = "Time (lunch or dinner ?)",
    total_bill = "Total of the bill"
  ),
  facet_labeller = ggplot2::label_wrap_gen(10)
)

## -----------------------------------------------------------------------------
ggcoef_model(mod_simple, facet_row = NULL, colour_guide = TRUE)

## -----------------------------------------------------------------------------
ggcoef_model(mod_titanic, exponentiate = TRUE)
ggcoef_model(
  mod_titanic,
  exponentiate = TRUE,
  show_p_values = FALSE,
  signif_stars = FALSE,
  add_reference_rows = FALSE,
  categorical_terms_pattern = "{level} (ref: {reference_level})",
  interaction_sep = " x "
) +
  ggplot2::scale_y_discrete(labels = scales::label_wrap(15))

## -----------------------------------------------------------------------------
mod_titanic2 <- glm(
  Survived ~ Sex * Age + Class,
  weights = Freq,
  data = d_titanic,
  family = binomial,
  contrasts = list(Sex = contr.sum, Class = contr.treatment(4, base = 3))
)
ggcoef_model(mod_titanic2, exponentiate = TRUE)

## -----------------------------------------------------------------------------
mod_poly <- lm(Sepal.Length ~ poly(Petal.Width, 3) + Petal.Length, data = iris)
ggcoef_model(mod_poly)

## -----------------------------------------------------------------------------
ggcoef_model(
  mod_titanic2,
  exponentiate = TRUE,
  no_reference_row = "Sex"
)
ggcoef_model(
  mod_titanic2,
  exponentiate = TRUE,
  no_reference_row = broom.helpers::all_dichotomous()
)
ggcoef_model(
  mod_titanic2,
  exponentiate = TRUE,
  no_reference_row = broom.helpers::all_categorical(),
  categorical_terms_pattern = "{level}/{reference_level}"
)

## -----------------------------------------------------------------------------
ggcoef_model(mod_simple, intercept = TRUE)

## -----------------------------------------------------------------------------
ggcoef_model(mod_simple, conf.int = FALSE)

## -----------------------------------------------------------------------------
ggcoef_model(mod_simple, significance = NULL)

## -----------------------------------------------------------------------------
ggcoef_model(mod_simple, colour = NULL)

## -----------------------------------------------------------------------------
ggcoef_model(mod_simple, include = c("time", "total_bill"))

## -----------------------------------------------------------------------------
ggcoef_model(mod_simple, include = dplyr::starts_with("t"))

## -----------------------------------------------------------------------------
ggcoef_model(mod_simple, stripped_rows = FALSE)

## -----------------------------------------------------------------------------
ggcoef_model(mod_simple) +
  ggplot2::xlab("Coefficients") +
  ggplot2::ggtitle("Custom title") +
  ggplot2::scale_color_brewer(palette = "Set1") +
  ggplot2::theme(legend.position = "right")

## -----------------------------------------------------------------------------
ggcoef_table(mod_simple)
ggcoef_table(mod_titanic, exponentiate = TRUE)

## -----------------------------------------------------------------------------
ggcoef_table(
  mod_simple,
  table_stat = c("label", "estimate", "std.error", "ci"),
  ci_pattern = "{conf.low} to {conf.high}",
  table_stat_label = list(
    estimate = scales::label_number(accuracy = .001),
    conf.low = scales::label_number(accuracy = .01),
    conf.high = scales::label_number(accuracy = .01),
    std.error = scales::label_number(accuracy = .001),
    label = toupper
  ),
  table_header = c("Term", "Coef.", "SE", "CI"),
  table_witdhs = c(2, 3)
)

## -----------------------------------------------------------------------------
library(nnet)
hec <- as.data.frame(HairEyeColor)
mod <- multinom(
  Hair ~ Eye + Sex,
  data = hec,
  weights = hec$Freq
)
ggcoef_multinom(
  mod,
  exponentiate = TRUE
)
ggcoef_multinom(
  mod,
  exponentiate = TRUE,
  type = "faceted"
)

## ----fig.height=9, fig.width=6------------------------------------------------
ggcoef_multinom(
  mod,
  exponentiate = TRUE,
  type = "table"
)

## -----------------------------------------------------------------------------
ggcoef_multinom(
  mod,
  type = "faceted",
  y.level_label = c("Brown" = "Brown\n(ref: Black)"),
  exponentiate = TRUE
)

## -----------------------------------------------------------------------------
library(pscl)
data("bioChemists", package = "pscl")
mod <- zeroinfl(art ~ fem * mar | fem + mar, data = bioChemists)

ggcoef_multicomponents(mod)
ggcoef_multicomponents(mod, type = "f")

## ----fig.height=7, fig.width=6------------------------------------------------
ggcoef_multicomponents(mod, type = "t")
ggcoef_multicomponents(
  mod,
  type = "t",
  component_label = c(conditional = "Count", zero_inflated = "Zero-inflated")
)

## -----------------------------------------------------------------------------
mod1 <- lm(Fertility ~ ., data = swiss)
mod2 <- step(mod1, trace = 0)
mod3 <- lm(Fertility ~ Agriculture + Education * Catholic, data = swiss)
models <- list(
  "Full model" = mod1,
  "Simplified model" = mod2,
  "With interaction" = mod3
)

ggcoef_compare(models)
ggcoef_compare(models, type = "faceted")

## ----echo=FALSE---------------------------------------------------------------
broom.helpers::supported_models |>
  knitr::kable()

