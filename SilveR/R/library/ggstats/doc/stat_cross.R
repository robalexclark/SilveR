## ----include = FALSE----------------------------------------------------------
knitr::opts_chunk$set(
  collapse = TRUE,
  comment = "#>"
)

## ----setup--------------------------------------------------------------------
library(ggstats)
library(ggplot2)

## -----------------------------------------------------------------------------
d <- as.data.frame(Titanic)
ggplot(d) +
  aes(x = Class, y = Survived, weight = Freq, size = after_stat(observed)) +
  stat_cross() +
  scale_size_area(max_size = 20)

## ----fig.height=6, fig.width=6------------------------------------------------
ggplot(d) +
  aes(
    x = Class, y = Survived, weight = Freq,
    size = after_stat(observed), fill = after_stat(std.resid)
  ) +
  stat_cross(shape = 22) +
  scale_fill_steps2(breaks = c(-3, -2, 2, 3), show.limits = TRUE) +
  scale_size_area(max_size = 20)

## -----------------------------------------------------------------------------
ggplot(d) +
  aes(x = Class, y = Survived, weight = Freq) +
  geom_tile(fill = "white", colour = "black") +
  geom_text(stat = "cross", mapping = aes(label = after_stat(observed))) +
  theme_minimal()

## -----------------------------------------------------------------------------
ggplot(d) +
  aes(
    x = Class, y = Survived, weight = Freq,
    label = scales::percent(after_stat(col.prop), accuracy = .1),
    fill = after_stat(std.resid)
  ) +
  stat_cross(shape = 22, size = 30) +
  geom_text(stat = "cross") +
  scale_fill_steps2(breaks = c(-3, -2, 2, 3), show.limits = TRUE) +
  facet_grid(rows = vars(Sex)) +
  labs(fill = "Standardized residuals") +
  theme_minimal()

