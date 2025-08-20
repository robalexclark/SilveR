## ----include = FALSE----------------------------------------------------------
knitr::opts_chunk$set(
  collapse = TRUE,
  comment = "#>"
)

## ----setup--------------------------------------------------------------------
library(ggstats)
library(dplyr)
library(ggplot2)
library(patchwork)

## -----------------------------------------------------------------------------
base <-
  ggplot(diamonds) +
  aes(y = clarity, fill = cut) +
  theme(legend.position = "none")

p_stack <-
  base +
  geom_bar(position = "stack") +
  ggtitle("position_stack()")

p_diverging <-
  base +
  geom_bar(position = "diverging") +
  ggtitle("position_diverging()")

p_stack + p_diverging

## -----------------------------------------------------------------------------
p_fill <-
  base +
  geom_bar(position = "fill") +
  ggtitle("position_fill()")

p_likert <-
  base +
  geom_bar(position = "likert") +
  ggtitle("position_likert()")

p_fill + p_likert

## -----------------------------------------------------------------------------
p_1 <-
  base +
  geom_bar(position = position_diverging(cutoff = 1)) +
  ggtitle("cutoff = 1")

p_2 <-
  base +
  geom_bar(position = position_diverging(cutoff = 2)) +
  ggtitle("cutoff = 2")

p_null <-
  base +
  geom_bar(position = position_diverging(cutoff = NULL)) +
  ggtitle("cutoff = NULL")

p_3.75 <-
  base +
  geom_bar(position = position_diverging(cutoff = 3.75)) +
  ggtitle("cutoff = 3.75")

p_5 <-
  base +
  geom_bar(position = position_diverging(cutoff = 5)) +
  ggtitle("cutoff = 5")

wrap_plots(p_1, p_2, p_null, p_3.75, p_5)

## -----------------------------------------------------------------------------
wrap_plots(
  p_1 + scale_fill_likert(cutoff = 1),
  p_null + scale_fill_likert(),
  p_3.75 + scale_fill_likert(cutoff = 3.75)
)

## -----------------------------------------------------------------------------
wrap_plots(
  p_3.75,
  p_3.75 +
    scale_x_continuous(
      limits = symmetric_limits,
      labels = label_number_abs()
    )
)

## -----------------------------------------------------------------------------
ggplot(diamonds) +
  aes(y = clarity, fill = cut) +
  geom_bar(position = "diverging") +
  geom_text(
    aes(
      label =
        label_number_abs(hide_below = 800)
        (after_stat(count))
    ),
    stat = "count",
    position = position_diverging(.5)
  ) +
  scale_fill_likert() +
  scale_x_continuous(
    labels = label_number_abs(),
    limits = symmetric_limits
  )

## -----------------------------------------------------------------------------
ggplot(diamonds) +
  aes(y = clarity, fill = cut) +
  geom_diverging() +
  geom_diverging_text(
    aes(
      label =
        label_number_abs(hide_below = 800)
        (after_stat(count))
    )
  ) +
  scale_fill_likert() +
  scale_x_continuous(
    labels = label_number_abs(),
    limits = symmetric_limits
  )

## -----------------------------------------------------------------------------
ggplot(diamonds) +
  aes(y = clarity, fill = cut) +
  geom_likert() +
  geom_likert_text() +
  scale_fill_likert() +
  scale_x_continuous(
    labels = label_percent_abs()
  )

## -----------------------------------------------------------------------------
d <- Titanic |> as.data.frame()
ggplot(d) +
  aes(y = Class, fill = Sex, weight = Freq) +
  geom_pyramid() +
  geom_pyramid_text() +
  scale_x_continuous(
    labels = label_percent_abs(),
    limits = symmetric_limits
  )

