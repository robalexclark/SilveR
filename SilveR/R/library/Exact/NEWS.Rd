\name{NEWS}
\title{NEWS file for the Exact package}

\section{Changes in Version 2.0 (2019-09-08)}{
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