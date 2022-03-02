## ----setup, include=FALSE-----------------------------------------------------
knitr::opts_chunk$set(
  digits = 3,
  collapse = TRUE,
  comment = "#>"
)
options(digits = 3)
library(knitr)
library(contrast)
library(nlme)
library(ggplot2)
library(geepack)
library(dplyr)
library(tidyr)
options(useFancyQuotes = FALSE, width = 80)

## -----------------------------------------------------------------------------
library(contrast)
library(dplyr)
two_factor_crossed %>% 
  group_by(diet, group) %>% 
  count()

## ----example1Plot, fig = TRUE, echo = FALSE, width = 6, height = 4.25---------
library(ggplot2)
theme_set(theme_bw() + theme(legend.position = "top"))
ggplot(two_factor_crossed) +
  aes(x = diet, y = expression, col = group, shape = group) +
  geom_point() + 
  geom_smooth(aes(group = group), method = lm, se = FALSE)

## ----example1LinearMod--------------------------------------------------------
lm_fit_1 <- lm(expression ~ (group + diet)^2, data = two_factor_crossed)
summary(lm_fit_1)

## ----example1Contrast---------------------------------------------------------
high_fat <- contrast(lm_fit_1, 
                     list(diet = "low fat", group = "vehicle"),
                     list(diet = "low fat", group = "treatment"))
print(high_fat, X = TRUE)

## ----example1ContrastStat, include = FALSE------------------------------------
basic_test_stat <- high_fat$testStat

## ----eachTest-----------------------------------------------------------------
trt_effect <-
  contrast(
    lm_fit_1,
    list(diet = levels(two_factor_crossed$diet), group = "vehicle"),
    list(diet = levels(two_factor_crossed$diet), group = "treatment")
  )
print(trt_effect, X = TRUE)

## ----meanEffect---------------------------------------------------------------
mean_effect <-
  contrast(
    lm_fit_1,
    list(diet = levels(two_factor_crossed$diet), group = "vehicle"),
    list(diet = levels(two_factor_crossed$diet), group = "treatment"),
    type = "average"
  )  
  
print(mean_effect, X = TRUE)

## ----example1Sand-------------------------------------------------------------
high_fat_sand <- 
  contrast(
    lm_fit_1, 
    list(diet = "low fat", group = "vehicle"),
    list(diet = "low fat", group = "treatment"),
    covType = "HC3"
  )
print(high_fat_sand)

## ----example1GenLinearMod-----------------------------------------------------
glm_fit_1 <- glm(2^expression ~ (group + diet)^2, 
                 data = two_factor_crossed, 
                 family = gaussian(link = "log"))
summary(glm_fit_1)
high_fat <- 
  contrast(glm_fit_1, 
           list(diet = "low fat", group = "vehicle"),
           list(diet = "low fat", group = "treatment")
  )
print(high_fat, X = TRUE)

## ----example2Data-------------------------------------------------------------
library(tidyr)

two_factor_incompl %>% 
  group_by(subject, config, day) %>% 
  count() %>% 
  ungroup() %>% 
  pivot_wider(
    id_cols = c(config, day),
    names_from = c(subject),
    values_from = c(n)
  )

## ----design2factor------------------------------------------------------------
two_factor_incompl %>% 
  group_by(group) %>% 
  count()

## ----design2gls---------------------------------------------------------------
gls_fit <-  gls(expression ~ group, 
               data = two_factor_incompl, 
               corCompSymm(form = ~ 1 | subject))
summary(gls_fit)

## ----design2glsCont-----------------------------------------------------------
print(
  contrast(
    gls_fit,
    list(group = "4:C"),
    list(group = "4:D")
  ),
  X = TRUE)     

## ----example2Plot, fig = TRUE, echo = FALSE, width = 6, height = 4.25---------
ggplot(two_factor_incompl) + 
  aes(x = day, y = expression, col = config, shape = config) + 
  geom_point() + 
   stat_summary(fun.y=mean, aes(group = config), geom = "line")

## ----design2lme---------------------------------------------------------------
lme_fit <-  lme(expression ~ group, data = two_factor_incompl, random = ~1|subject)
summary(lme_fit)

print(
  contrast(
    lme_fit, 
    list(group = "4:C"),
    list(group = "4:D")
  ),
  X = TRUE)        

## ----design2gee---------------------------------------------------------------
gee_fit <-  geese(2^expression ~ group,
                  data = two_factor_incompl,
                  id = subject,
                  family = gaussian(link = "log"),
                  corstr = "exchangeable")
summary(gee_fit)

print(
  contrast(
    gee_fit, 
    list(group = "4:C"),
    list(group = "4:D")
  ),
  X = TRUE)   

## ----ex1FC--------------------------------------------------------------------
trt_effect <- 
  contrast(lm_fit_1, 
           list(diet = levels(two_factor_crossed$diet), group = "vehicle"),
           list(diet = levels(two_factor_crossed$diet), group = "treatment"),
           fcfunc = function(u) 2^u
  )  
print(trt_effect, X = TRUE)
trt_effect$foldChange

