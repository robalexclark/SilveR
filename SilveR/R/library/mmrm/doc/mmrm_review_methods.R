## ----include = FALSE----------------------------------------------------------
knitr::opts_chunk$set(
  collapse = TRUE, comment = "#>", out.width = "100%",
  dpi = 150, fig.path = "mmrm-"
)

## ----review-setup, message=FALSE, warning=FALSE, include=FALSE----------------
library(MASS)
library(clusterGeneration)
library(dplyr)
library(purrr)
library(microbenchmark)
library(stringr)
library(mmrm)
library(lme4)
library(nlme)
library(glmmTMB)
library(sasr)
library(knitr)
library(emmeans)
library(ggplot2)

set.seed(5123)

## ----warning=FALSE, message=FALSE, echo=FALSE---------------------------------
# format table in markdown
cached_mmrm_results$conv_time_fev %>%
  arrange(median) %>%
  transmute(
    Implementation = expression,
    Median = median,
    `First Quartile` = lower,
    `Third Quartile` = upper
  ) %>%
  knitr::kable(
    caption = "Comparison of convergence times: milliseconds", digits = 2
  )

## ----warning=FALSE, message=FALSE, echo=FALSE---------------------------------
# format table in markdown
cached_mmrm_results$conv_time_bcva %>%
  arrange(median) %>%
  transmute(
    Implementation = expression,
    Median = median,
    `First Quartile` = lower,
    `Third Quartile` = upper
  ) %>%
  knitr::kable(
    caption = "Comparison of convergence times: seconds", digits = 2
  )

## ----review-treatment-fev, echo = FALSE, warning = FALSE, message = FALSE-----
# plot estimates
ggplot(
  cached_mmrm_results$rel_diff_ests_tbl_fev,
  aes(x = parameter, y = rel_diff, color = estimator, shape = converged)
) +
  geom_point(position = position_dodge(width = 0.5)) +
  geom_hline(yintercept = 0, linetype = 2, alpha = 0.5) +
  scale_color_discrete(name = "Procedure") +
  scale_shape_discrete(name = "Convergence") +
  ylab("Relative Difference") +
  xlab("Marginal Treatment Effect") +
  ggtitle("Average Treatment Effect Estimates Relative to SAS Estimates") +
  theme_classic()

## ----review-treatment-bcva, echo = FALSE, warning = FALSE, message = FALSE----
# plot estimates
ggplot(
  cached_mmrm_results$rel_diff_ests_tbl_bcva,
  aes(x = parameter, y = rel_diff, color = estimator, shape = converged)
) +
  geom_point(position = position_dodge(width = 0.5)) +
  geom_hline(yintercept = 0, linetype = 2, alpha = 0.5) +
  scale_color_discrete(name = "Procedure") +
  scale_shape_discrete(name = "Convergence") +
  ylab("Relative Difference") +
  xlab("Marginal Treatment Effect") +
  ggtitle("Average Treatment Effect Estimates Relative to SAS Estimates") +
  theme_classic()

# excluding glmmTMB
cached_mmrm_results$rel_diff_ests_tbl_bcva %>%
  dplyr::filter(estimator != "glmmTMB") %>%
  ggplot(
    aes(x = parameter, y = rel_diff, color = estimator, shape = converged)
  ) +
  geom_point(position = position_dodge(width = 0.5)) +
  geom_hline(yintercept = 0, linetype = 2, alpha = 0.5) +
  scale_color_discrete(name = "Procedure") +
  scale_shape_discrete(name = "Convergence") +
  ylab("Relative Difference") +
  xlab("Marginal Treatment Effect") +
  ggtitle(
    "Average Treatment Effect Estimates Relative to SAS Estimates
      (Excluding glmmTMB)"
  ) +
  theme_classic()

## ----review-missingness-table, warning=FALSE, message=FALSE, echo=FALSE-------
## construct the table
cached_mmrm_results$df_missingness %>%
  kable(caption = "Number of patients per visit")

## ----review-convergence-rate-missingness, warning=FALSE, message=FALSE, echo=FALSE----
## plot the convergence rates
cached_mmrm_results$conv_rate %>%
  mutate(
    missingness = factor(
      missingness,
      levels = c("none", "mild", "moderate", "high")
    )
  ) %>%
  ggplot(aes(x = method, y = convergence_rate)) +
  geom_point() +
  facet_grid(rows = vars(missingness)) +
  xlab("Method") +
  ylab("Convergence Rate (10 Replicates)") +
  ggtitle("Convergence Rates by Missingness Levels") +
  scale_y_continuous(labels = scales::percent_format(accuracy = 1)) +
  theme_bw()

## ----review-session-info, echo=FALSE------------------------------------------
sessionInfo()

