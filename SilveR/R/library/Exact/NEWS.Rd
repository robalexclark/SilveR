\name{NEWS}
\title{NEWS file for the Exact package}

\section{Changes in Version 3.3 (2024-07-24)}{
  \itemize{

    \item{Fixed an error where the p-value was sometimes incorrect for imbalanced designs.  Only occurred when the p-value would be large, but exact test gave a much higher p-value.  Thanks to Andrea Smith for pointing out the issue}
  }
}

\section{Changes in Version 3.2 (2022-09-24)}{
  \itemize{

    \item{Added more stored tables for CSM test comparing two independent proportions.  Tables with (n1+1)x(n2+1) <= 15,000 and group ratio between 1:1 and 2:1 were added to the existing stored tables.  Stored CSM ordering matrix is around 85MB, so is loaded from the ExactData R package from GitHub using the drat R package}

    \item{Improved computation time for multinomial tests}
  }
}

\section{Changes in Version 3.1 (2021-11-26)}{
  \itemize{

    \item{Added tsmethod parameter to unconditional paired tests}

    \item{Added "UCM" test and changed names of method options for unconditional paired tests}
    
    \item{Fixed issue with unconditional paired tests where confidence bounds are not possible based on extreme delta}
    
    \item{Fixed error message for two independent sample tests when confidence bounds are either 0 or 1.  Thanks to Ping Mahling for pointing out this issue}
  }
}


\section{Changes in Version 3.0 (2021-09-01)}{
  \itemize{

    \item{Added unconditional exact tests to compare two paired proportions!  Three functions are added: paired.exact.test, paired.reject.region, and power.paired.test}
    
    \item{Fixed issues with CSM test and greatly improved computation time.  Added the parameter "useStoredCSM", which uses a stored pre-computed CSM ordering matrix for sample sizes with max(n1,n2) <= 100 for two independent proportions and sample size <= 200 for two paired proportions.  Stored CSM ordering matrix is around 25MB, so is loaded from the ExactData R package from GitHub using the drat R package}

    \item{For two independent proportions, removed method option "csm approximate", changed method option "chisq" to "pearson chisq", and removed ref.pvalue option in exact.reject.region() and power.exact.test() functions.  Added more checks to make it more user-friendly}
  }
}


\section{Changes in Version 2.1 (2020-10-01)}{
  \itemize{
    
    \item{Fixed non-inferiority (equivalence) tests and confidence interval calculations.  Code can now perform these tests and CIs for all methods}

    \item{Added tsmethod parameter.  This allows user to perform a two-sided test by either squaring the statistic (default; previous versions), or by performing one-sided test and doubling the p-value}
    
    \item{Removed precision parameter and use rootSolve R package to determine confidence interval}
  }
}


\section{Changes in Version 2.0 (2019-10-14)}{
  \itemize{
    
    \item{Added \code{exact.reject.region} function}

    \item{Added input parameters in \code{exact.test}.  Input parameters include calculating confidence intervals and non-zero null hypothesis for the difference in proportion (only "z-pooled" and "csm" methods)}

    \item{Added "Chisq" and "Yates chisq" method to \code{power.exact.test}}
    
    \item{Removed "CSM Modified" method (determined not to be level-alpha)}

    \item{Changed input parameters in \code{exact.test}.  Parameter \code{interval} changed to \code{np.interval} and \code{alpha} changed to \code{reject.alpha}}

    \item{Converted \code{power.exact.test} output to be "power.htest" object}
    
    \item{Greatly improved computation time, especially for Boschloo's test, large sample sizes, and power calculations}
  }
}

\section{Changes in Version 1.7 (2016-10-22)}{
  \itemize{
    
    \item{Converted \code{exact.test} output to be "htest" object}
    
    \item{Added "Fisher" method to \code{power.exact.test}}
    
    \item{Greatly improved computation time}
    
    \item{Fixed a floating point issue where two tables were equally extreme but had different test statistic.  Thanks to Long Qu for pointing out this issue}
  }
}

\section{Changes in Version 1.5-1.6 (2015-05-26)}{
  \itemize{
    
    \item{Resolved an error message that occurred when two proportions were nearly identical}
  }
}

\section{Changes in Version 1.4 (2013-07-22)}{
  \itemize{
    
    \item{Added "CSM", "CSM Modified", and "CSM Approximate" methods}
  }
}

\section{Changes in Version 1.2-1.3 (2013-02-23)}{
  \itemize{
    
    \item{Added \code{to.plot} and \code{ref.pvalue} input parameters to the \code{exact.test} function}

    \item{Added "Santner and Snell" method}
    
    \item{Greatly improved computation time}
  }
}

\section{Changes in Version 1.1 (2012-08-19)}{
  \itemize{
    
    \item{Changed \code{npNumbers} from an increment to a number representing the number of nuisance parameters considered}
    
    \item{Added \code{cond.row} representing whether the rows or margins are fixed for "binomial" model}
  }
}

\section{Version 1.0 (2012-03-22)}{
  \itemize{
    
    \item{First CRAN release.}
    
    \item{Can perform Z-pooled, Z-unpooled, and Boschloo's test.  Can perform "binomial" and "multinomial" models with or without interval approach recommended by Berger and Boos (1994)}
  }
}