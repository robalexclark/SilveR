## ----include = FALSE----------------------------------------------------------
knitr::opts_chunk$set(
  collapse = TRUE,
  comment = "#>"
)

## ----setup--------------------------------------------------------------------
library(ggstats)
library(dplyr)
library(ggplot2)

## -----------------------------------------------------------------------------
likert_levels <- c(
  "Strongly disagree",
  "Disagree",
  "Neither agree nor disagree",
  "Agree",
  "Strongly agree"
)
set.seed(42)
df <-
  tibble(
    q1 = sample(likert_levels, 150, replace = TRUE),
    q2 = sample(likert_levels, 150, replace = TRUE, prob = 5:1),
    q3 = sample(likert_levels, 150, replace = TRUE, prob = 1:5),
    q4 = sample(likert_levels, 150, replace = TRUE, prob = 1:5),
    q5 = sample(c(likert_levels, NA), 150, replace = TRUE),
    q6 = sample(likert_levels, 150, replace = TRUE, prob = c(1, 0, 1, 1, 0))
  ) %>%
  mutate(across(everything(), ~ factor(.x, levels = likert_levels)))

likert_levels_dk <- c(
  "Strongly disagree",
  "Disagree",
  "Neither agree nor disagree",
  "Agree",
  "Strongly agree",
  "Don't know"
)
df_dk <-
  tibble(
    q1 = sample(likert_levels_dk, 150, replace = TRUE),
    q2 = sample(likert_levels_dk, 150, replace = TRUE, prob = 6:1),
    q3 = sample(likert_levels_dk, 150, replace = TRUE, prob = 1:6),
    q4 = sample(likert_levels_dk, 150, replace = TRUE, prob = 1:6),
    q5 = sample(c(likert_levels_dk, NA), 150, replace = TRUE),
    q6 = sample(
      likert_levels_dk, 150,
      replace = TRUE, prob = c(1, 0, 1, 1, 0, 1)
    )
  ) %>%
  mutate(across(everything(), ~ factor(.x, levels = likert_levels_dk)))

## -----------------------------------------------------------------------------
gglikert(df)

## -----------------------------------------------------------------------------
gglikert(df, include = q1:q3)

## -----------------------------------------------------------------------------
gglikert(df) +
  ggtitle("A Likert-type items plot", subtitle = "generated with gglikert()") +
  scale_fill_brewer(palette = "RdYlBu")

## -----------------------------------------------------------------------------
gglikert(df, sort = "ascending")

## -----------------------------------------------------------------------------
gglikert(df, sort = "ascending", sort_method = "mean")

## -----------------------------------------------------------------------------
gglikert(df, reverse_likert = TRUE)

## -----------------------------------------------------------------------------
gglikert(df, add_labels = FALSE)

## -----------------------------------------------------------------------------
gglikert(
  df,
  labels_size = 3,
  labels_accuracy = .1,
  labels_hide_below = .2,
  labels_color = "white"
)

## -----------------------------------------------------------------------------
gglikert(
  df,
  totals_include_center = TRUE,
  sort = "descending",
  sort_prop_include_center = TRUE
)

## -----------------------------------------------------------------------------
gglikert(
  df,
  totals_size = 4,
  totals_color = "blue",
  totals_fontface = "italic",
  totals_hjust = .20
)

## -----------------------------------------------------------------------------
gglikert(df, add_totals = FALSE)

## -----------------------------------------------------------------------------
if (require(labelled)) {
  df <- df %>%
    set_variable_labels(
      q1 = "first question",
      q2 = "second question",
      q3 = "this is the third question with a quite long variable label"
    )
}
gglikert(df)

## -----------------------------------------------------------------------------
gglikert(
  df,
  variable_labels = c(
    q1 = "alternative label for the first question",
    q6 = "another custom label"
  )
)

## -----------------------------------------------------------------------------
gglikert(df, y_label_wrap = 20)
gglikert(df, y_label_wrap = 200)

## -----------------------------------------------------------------------------
gglikert(df_dk)

## -----------------------------------------------------------------------------
df_dk %>%
  mutate(across(everything(), ~ factor(.x, levels = likert_levels))) %>%
  gglikert()

## -----------------------------------------------------------------------------
df_dk %>% gglikert(exclude_fill_values = "Don't know")

## ----message=FALSE------------------------------------------------------------
df_group <- df
df_group$group1 <- sample(c("A", "B"), 150, replace = TRUE)
df_group$group2 <- sample(c("a", "b", "c"), 150, replace = TRUE)

gglikert(df_group,
  q1:q6,
  facet_cols = vars(group1),
  labels_size = 3
)
gglikert(df_group,
  q1:q2,
  facet_rows = vars(group1, group2),
  labels_size = 3
)
gglikert(df_group,
  q3:q6,
  facet_cols = vars(group1),
  facet_rows = vars(group2),
  labels_size = 3
) +
  scale_x_continuous(
    labels = label_percent_abs(),
    expand = expansion(0, .2)
  )

## -----------------------------------------------------------------------------
gglikert(df_group,
  q1:q4,
  y = "group1",
  facet_rows = vars(.question),
  labels_size = 3,
  facet_label_wrap = 15
)

## -----------------------------------------------------------------------------
gglikert_stacked(df)

gglikert_stacked(
  df,
  sort = "asc",
  add_median_line = TRUE,
  add_labels = FALSE
)

gglikert_stacked(
  df_group,
  include = q1:q4,
  y = "group2"
) +
  facet_grid(
    rows = vars(.question),
    labeller = label_wrap_gen(15)
  )

## -----------------------------------------------------------------------------
gglikert_data(df) %>%
  head()

## -----------------------------------------------------------------------------
ggplot(gglikert_data(df)) +
  aes(y = .question, fill = .answer) +
  geom_bar(position = "fill")

## -----------------------------------------------------------------------------
df$sampling_weights <- runif(nrow(df))
gglikert(df, q1:q4, weights = sampling_weights)

