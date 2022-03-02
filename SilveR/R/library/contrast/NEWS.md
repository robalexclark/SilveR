## version 0.24

 * Bugfix: When using splines, generateData threw an error due to the variable
   not being within the dataset.

## version 0.23

 * Package reorganized to use `roxygen2`, markdown, `testthat`, code coverage, continuous testing, and other features. 

 * The degrees of freedom for `lme` models were changed (again). Previously, it would make this determination from the number of data points, fixed effect parameters, and random effect parameters. Now it follows what the `doBy` package does, using internal data from `lme` objects and takes the smallest possible degrees of freedom if variables involved in the the contrast have variable df. The previous method for counting df did not align with the results inside of the `lee` objects for simple cases. 



## version 0.22


 * Maintainer email was changed

 * A typo was fixed in the vignette



## version 0.19


 * For `lme` models, Thorn Thaler found a bug in some models where the calculation of model degrees of freedom could not be determined and proposed a fix. 




## version 0.18

 * If the sandwich estimate failed, the traditional covariance estimator is used instead.

 * Moved to a real NEWS file format.

 * A bug was fixed (spotted by Thorn Thaler) where normal (not t) distributions were being used with `lme` and `gls` models to calculate confidence intervals. 




## version 0.17

 * Fixed links in man page to point to `rms` instead of `Design`



## version 0.16

 * The package was updated to work with the `rms` package instead of the `Design` package. 

 * A bug was fixed for generalized linear models with no degrees of freedom, such as log-linear models.

 * The vignette was slightly reformatted.



## version 0.14

 * An error check was added for `lme` models that will fail if the covariance  matrix of the variance-covariance coefficients has issues (since `lme` counts the number of random effects when calculating degrees of freedom).

 * With `lme`, this problem is signaled by `lmeObject$apVar` being a character string (eg. "Non-positive definite approximate variance-covariance")




## version 0.13

 * Fixed typo in manual




## version 0.12

 * Added log fold-change transformation




## version 0.11

 * Cleaned up the package vignette a little




## version 0.10

 * Added specific namespace calls to `nlme` so that `lme4` methods do not interfere.




