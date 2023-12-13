## -----------------------------------------------------------------------------
library(tidyr)
d <- iris %>%
  as_tibble() %>%
  pack(
    Sepal = starts_with("Sepal"),
    Petal = starts_with("Petal"),
    .names_sep = "."
  )
str(d)
class(d$Sepal)

## -----------------------------------------------------------------------------
library(labelled)
var_label(d$Sepal$Length) <- "Length of the sepal"
str(d)

## -----------------------------------------------------------------------------
var_label(d$Petal) <- "wrong label for Petal"
str(d)

## -----------------------------------------------------------------------------
label_attribute(d$Petal) <- "correct label for Petal"
str(d)

## -----------------------------------------------------------------------------
d <- d %>% set_variable_labels(Sepal = "Label of the Sepal df-column")
str(d)

## -----------------------------------------------------------------------------
var_label(d) <- list(Sepal = "Label of the Sepal df-column")
str(d)

## -----------------------------------------------------------------------------
d$Petal <- d$Petal %>%
  set_variable_labels(
    Length = "Petal length",
    Width = "Petal width"
  )
str(d)

## -----------------------------------------------------------------------------
d %>% get_variable_labels()

## -----------------------------------------------------------------------------
d %>% get_variable_labels(recurse = TRUE)
d %>%
  get_variable_labels(
    recurse = TRUE,
    null_action = "fill",
    unlist = TRUE
  )

