## ----include = FALSE----------------------------------------------------------
knitr::opts_chunk$set(
  collapse = TRUE,
  comment = "#>"
)

## ----setup--------------------------------------------------------------------
library(ggstats)
library(ggplot2)

## -----------------------------------------------------------------------------
data(tips, package = "reshape")
ggplot(tips) +
  aes(x = day, y = tip) +
  geom_point()

## -----------------------------------------------------------------------------
ggplot(tips) +
  aes(x = day, y = tip) +
  stat_weighted_mean()

## -----------------------------------------------------------------------------
ggplot(tips) +
  aes(x = day, y = tip, group = 1) +
  stat_weighted_mean(geom = "line")

## -----------------------------------------------------------------------------
ggplot(tips) +
  aes(x = day, y = tip, group = 1) +
  geom_line(stat = "weighted_mean")

## -----------------------------------------------------------------------------
p <- ggplot(tips) +
  aes(x = day, y = tip, fill = sex) +
  stat_weighted_mean(geom = "bar", position = "dodge") +
  ylab("mean tip")
p

## -----------------------------------------------------------------------------
p + facet_grid(rows = vars(smoker))

## -----------------------------------------------------------------------------
ggplot(tips) +
  aes(x = day, y = as.integer(smoker == "Yes"), fill = sex) +
  stat_weighted_mean(geom = "bar", position = "dodge") +
  scale_y_continuous(labels = scales::percent) +
  ylab("proportion of smoker")

## -----------------------------------------------------------------------------
d <- as.data.frame(Titanic)
ggplot(d) +
  aes(x = Class, y = as.integer(Survived == "Yes"), weight = Freq, fill = Sex) +
  geom_bar(stat = "weighted_mean", position = "dodge") +
  scale_y_continuous(labels = scales::percent) +
  labs(y = "Proportion who survived")

