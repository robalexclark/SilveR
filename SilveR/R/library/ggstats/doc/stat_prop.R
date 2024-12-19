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
p <- ggplot(d) +
  aes(x = Class, fill = Survived, weight = Freq, by = Class) +
  geom_bar(position = "fill") +
  geom_text(stat = "prop", position = position_fill(.5))
p

## -----------------------------------------------------------------------------
p + facet_grid(cols = vars(Sex))

## -----------------------------------------------------------------------------
ggplot(d) +
  aes(x = Class, fill = Survived, weight = Freq, by = 1) +
  geom_bar() +
  geom_text(
    aes(label = scales::percent(after_stat(prop), accuracy = 1)),
    stat = "prop",
    position = position_stack(.5)
  )

## -----------------------------------------------------------------------------
ggplot(d) +
  aes(x = Class, fill = Sex, weight = Freq, by = Sex) +
  geom_bar(position = "dodge")

## -----------------------------------------------------------------------------
ggplot(d) +
  aes(x = Class, fill = Sex, weight = Freq, by = Sex, y = after_stat(prop)) +
  geom_bar(stat = "prop", position = "dodge") +
  scale_y_continuous(labels = scales::percent)

## -----------------------------------------------------------------------------
ggplot(d) +
  aes(x = Class, fill = Sex, weight = Freq, by = Sex, y = after_stat(prop)) +
  geom_bar(stat = "prop", position = "dodge") +
  scale_y_continuous(labels = scales::percent) +
  geom_text(
    mapping = aes(
      label = scales::percent(after_stat(prop), accuracy = .1),
      y = after_stat(0.01)
    ),
    vjust = "bottom",
    position = position_dodge(.9),
    stat = "prop"
  )

## -----------------------------------------------------------------------------
d <- diamonds |>
  dplyr::filter(!(cut == "Ideal" & clarity == "I1")) |>
  dplyr::filter(!(cut == "Very Good" & clarity == "VS2")) |>
  dplyr::filter(!(cut == "Premium" & clarity == "IF"))
p <- ggplot(d) +
  aes(x = clarity, fill = cut, by = clarity) +
  geom_bar(position = "fill")
p +
  geom_text(
    stat = "prop",
    position = position_fill(.5)
  )

## -----------------------------------------------------------------------------
p +
  geom_text(
    stat = "prop",
    position = position_fill(.5),
    complete = "fill"
  )

## -----------------------------------------------------------------------------
ggplot(diamonds) +
  aes(y = clarity, fill = cut) +
  geom_prop_bar() +
  geom_prop_text()

## -----------------------------------------------------------------------------
d <- as.data.frame(Titanic)
ggplot(d) +
  aes(x = Class, fill = Sex, weight = Freq, by = Sex) +
  geom_prop_bar(position = "dodge") +
  geom_prop_text(
    position = position_dodge(width = .9),
    vjust = - 0.5
  ) +
  scale_y_continuous(labels = scales::percent)

## -----------------------------------------------------------------------------
ggplot(diamonds) +
  aes(x = clarity, fill = cut) +
  geom_prop_bar(height = "count") +
  geom_prop_text(
    height = "count",
    labels = "count",
    labeller = scales::number
  )

