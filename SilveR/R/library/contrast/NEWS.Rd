
\name{NEWS}
\title{News for Package \pkg{contrast}}
\newcommand{\cpkg}{\href{https://CRAN.R-project.org/package=#1}{\pkg{#1}}}

\section{Changes in version 0.22}{

\itemize{
\item Maintainer email was changed
\item A typo was fixed in the vingette
}
}

\section{Changes in version 0.19}{

\itemize{
\item For \code{lme} models, Thorn Thaler found a bug in some models where the calculaiton of model degrees of freedon could not be determined and proposed a fix. 
}
}


\section{Changes in version 0.18}{
\itemize{
\item If the sandwich estimate failed, the traditional covariance estimator is used instead.

\item Moved to a real NEWS file format.

\item A bug was fixed (spotted by Thorn Thaler) where normal (not t) distributions were being used with \code{lme} and \code{gls} models to calculate confidence intervals. 
}
}


\section{Changes in version 0.17}{
\itemize{
\item Fixed links in man page to point to \cpkg{rms} instead of \cpkg{Design}
}
}

\section{Changes in version 0.16}{
\itemize{
\item The package was updated to work with the \cpkg{rms} package instead of the \cpkg{Design} package. 

\item A bug was fixed for generalized linear models with no degrees of freedom, such
as log-linear models.

\item The vignette was slightly reformatted.
}
}

\section{Changes in version 0.14}{
\itemize{
\item An error check was added for \code{lme} models that will fail if the covariance 
matrix of the variance-covariance coefficients has issues (since \code{lme} 
counts the number of random effects when calculating degrees of freedom).

\item With \code{lme}, this problem is signaled by \code{lmeObject$apVar} being a character 
string (eg. "Non-positive definite approximate variance-covariance")
}
}


\section{Changes in version 0.13}{
\itemize{
\item Fixed typo in manual
}
}


\section{Changes in version 0.12}{
\itemize{
\item Added log fold-change transformation
}
}


\section{Changes in version 0.11}{
\itemize{
\item Cleaned up the package vignette a little
}
}


\section{Changes in version 0.10}{
\itemize{
\item Added specific namespace calls to \cpkg{nlme} so that \cpkg{lme4} methods do not interfere.
}
}



