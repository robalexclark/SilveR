\documentclass[a4paper]{report}

%\VignetteIndexEntry{Implementing Permutation Tests}
%\VignetteDepends{libcoin}
%\VignetteKeywords{conditional inference, conditional Monte Carlo}}
%\VignettePackage{libcoin}

%% packages
\usepackage{amsfonts,amstext,amsmath,amssymb,amsthm}

\usepackage[utf8]{inputenc}

\newif\ifshowcode
\showcodetrue

\usepackage{latexsym}
%\usepackage{html}

\usepackage{listings}

\usepackage{color}
\definecolor{linkcolor}{rgb}{0, 0, 0.7}

\usepackage[%
backref,%
raiselinks,%
pdfhighlight=/O,%
pagebackref,%
hyperfigures,%
breaklinks,%
colorlinks,%
pdfpagemode=UseNone,%
pdfstartview=FitBH,%
linkcolor={linkcolor},%
anchorcolor={linkcolor},%
citecolor={linkcolor},%
filecolor={linkcolor},%
menucolor={linkcolor},%
urlcolor={linkcolor}%
]{hyperref}

\usepackage[round]{natbib}

\usepackage{underscore}

\usepackage[top=25mm,bottom=25mm,left=25mm,right=25mm]{geometry}

\usepackage{lmodern}

\newcommand{\pkg}[1]{\textbf{#1}}
\newcommand{\proglang}[1]{\textsf{#1}}
\newcommand{\code}[1]{\texttt{#1}}
\newcommand{\file}[1]{\texttt{#1}}

\newcommand{\R}{\mathbb{R} }
\newcommand{\Prob}{\mathbb{P} }
\newcommand{\N}{\mathbb{N} }
%\newcommand{\C}{\mathbb{C} }
\newcommand{\V}{\mathbb{V}} %% cal{\mbox{\textnormal{Var}}} }
\newcommand{\E}{\mathbb{E}} %%mathcal{\mbox{\textnormal{E}}} }
\newcommand{\Var}{\mathbb{V}} %%mathcal{\mbox{\textnormal{Var}}} }
\newcommand{\argmin}{\operatorname{argmin}\displaylimits}
\newcommand{\argmax}{\operatorname{argmax}\displaylimits}
\newcommand{\LS}{\mathcal{L}_n}
\newcommand{\TS}{\mathcal{T}_n}
\newcommand{\LSc}{\mathcal{L}_{\text{comb},n}}
\newcommand{\LSbc}{\mathcal{L}^*_{\text{comb},n}}
\newcommand{\F}{\mathcal{F}}
\newcommand{\A}{\mathcal{A}}
\newcommand{\yn}{y_{\text{new}}}
\newcommand{\z}{\mathbf{z}}
\newcommand{\X}{\mathbf{X}}
\newcommand{\Y}{\mathbf{Y}}
\newcommand{\sX}{\mathcal{X}}
\newcommand{\sY}{\mathcal{Y}}
\newcommand{\T}{\mathbf{T}}
\newcommand{\x}{\mathbf{x}}
\renewcommand{\a}{\mathbf{a}}
\newcommand{\xn}{\mathbf{x}_{\text{new}}}
\newcommand{\y}{\mathbf{y}}
\newcommand{\w}{\mathbf{w}}
\newcommand{\sbullet}{\mathbin{\vcenter{\hbox{\scalebox{0.5}{$\bullet$}}}}}
\newcommand{\wdot}{\mathbf{w}_{\sbullet}}
\renewcommand{\t}{\mathbf{t}}
\newcommand{\M}{\mathbf{M}}
\renewcommand{\vec}{\text{vec}}
\newcommand{\B}{\mathbf{B}}
\newcommand{\K}{\mathbf{K}}
\newcommand{\W}{\mathbf{W}}
\newcommand{\D}{\mathbf{D}}
\newcommand{\I}{\mathbf{I}}
\newcommand{\bS}{\mathbf{S}}
\newcommand{\cellx}{\pi_n[\x]}
\newcommand{\partn}{\pi_n(\mathcal{L}_n)}
\newcommand{\err}{\text{Err}}
\newcommand{\ea}{\widehat{\text{Err}}^{(a)}}
\newcommand{\ecv}{\widehat{\text{Err}}^{(cv1)}}
\newcommand{\ecvten}{\widehat{\text{Err}}^{(cv10)}}
\newcommand{\eone}{\widehat{\text{Err}}^{(1)}}
\newcommand{\eplus}{\widehat{\text{Err}}^{(.632+)}}
\newcommand{\eoob}{\widehat{\text{Err}}^{(oob)}}
\newcommand{\mub}{\boldsymbol{\mu}}
\newcommand{\Sigmab}{\boldsymbol{\Sigma}}


\author{Torsten Hothorn \\ Universit\"at Z\"urich}

\title{The \pkg{libcoin} Package}

\begin{document}

\pagenumbering{roman}
\maketitle
\tableofcontents

\chapter*{Licence}

{\setlength{\parindent}{0cm}
Copyright (C) 2017-2023 Torsten Hothorn \\

This file is part of the \pkg{libcoin} \proglang{R} add-on package. \\

\pkg{libcoin} is free software: you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation, version 2. \\

\pkg{libcoin} is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details. \\

You should have received a copy of the GNU General Public License
along with \pkg{libcoin}.  If not, see <http://www.gnu.org/licenses/>.
}

\chapter{Introduction}
\pagenumbering{arabic}

The \pkg{libcoin} package implements a generic framework for permutation
tests. We assume that we are provided with $n$ observations
\begin{eqnarray*}
(\Y_i, \X_i, w_i, \text{block}_i), \quad i = 1, \dots, N.
\end{eqnarray*}
The variables $\Y$ and $\X$ from sample spaces $\mathcal{Y}$ and
$\mathcal{X}$ may
be measured at arbitrary scales and may be multivariate as well. In addition
to those measurements, case weights $w_i \in \N$ and a factor $\text{block}_i \in \{1, \dots, B\}$
coding for $B$ independent blocks may
be available.
We are interested in testing the null hypothesis of independence of $\Y$ and
$\X$
\begin{eqnarray*}
H_0: D(\Y \mid \X) = D(\Y)
\end{eqnarray*}
against arbitrary alternatives. \cite{strasserweber1999} suggest to derive
scalar test statistics for testing $H_0$ from multivariate linear statistics
of a specific linear form. Let $\A \subseteq \{1, \dots, N\}$ denote some subset of the
observation numbers and consider the linear statistic
\begin{eqnarray} \label{linstat}
\T(\A) = \vec\left(\sum_{i \in \A} w_i g(\X_i) h(\Y_i, \{\Y_i \mid i \in \A\})^\top\right)
\in \R^{PQ}.
\end{eqnarray}
Here, $g: \mathcal{X} \rightarrow \R^P$ is a transformation of
$\X$ known as the \emph{regression function} and $h: \mathcal{Y} \times
\mathcal{Y}^n \rightarrow \R^Q$ is a transformation of $\Y$ known as the
\emph{influence function}, where the latter may depend on $\Y_i$ for $i \in \A$
in a permutation symmetric way.  We will give specific examples on how to choose
$g$ and $h$ later on.

With $\x_i = g(\X_i) \in \R^P$ and $\y_i = h(\Y_i, \{\Y_i, i \in \A\}) \in \R^Q$ we write
\begin{eqnarray} \label{linstatsimple}
\T(\A) = \vec\left(\sum_{i \in \A} w_i \x_i \y_i^\top\right)
\in \R^{PQ}.
\end{eqnarray}
The \pkg{libcoin} package doesn't handle neither $g$ nor $h$, this is the job
of \pkg{coin} and we therefore continue with $\x_i$ and $\y_i$.

The distribution of $\T$  depends on the joint
distribution of $\Y$ and $\X$, which is unknown under almost all practical
circumstances. At least under the null hypothesis one can dispose of this
dependency by fixing $\X_i, i \in \A$ and conditioning on all possible
permutations $S(\A)$ of the responses $\Y_i, i \in \A$.
This principle leads to test procedures known
as \textit{permutation tests}.
The conditional expectation $\mub(\A) \in \R^{PQ}$ and covariance
$\Sigmab(\A) \in \R^{PQ \times PQ}$
of $\T$ under $H_0$ given
all permutations $\sigma \in S(\A)$ of the responses are derived by
\cite{strasserweber1999}:
\begin{eqnarray}
\mub(\A) & = & \E(\T(\A) \mid S(\A)) = \vec \left( \left( \sum_{i \in \A} w_i \x_i \right) \E(h \mid S(\A))^\top
\right), \nonumber \\
\Sigmab(\A) & = & \V(\T(\A) \mid S(\A)) \nonumber \\
& = &
    \frac{\wdot}{\wdot(\A) - 1}  \V(h \mid S(\A)) \otimes
        \left(\sum_{i \in \A} w_i  \x_i \otimes w_i \x_i^\top \right)
\label{expectcovar}
\\
& - & \frac{1}{\wdot(\A) - 1}  \V(h \mid S(\A))  \otimes \left(
        \sum_{i \in \A} w_i \x_i \right)
\otimes \left( \sum_{i \in \A} w_i \x_i\right)^\top
\nonumber
\end{eqnarray}
where $\wdot(\A) = \sum_{i \in \A} w_i$ denotes the sum of the case weights,
and $\otimes$ is the Kronecker product. The conditional expectation of the
influence function is
\begin{eqnarray*}
\E(h \mid S(\A)) = \wdot(\A)^{-1} \sum_{i \in \A} w_i \y_i \in
\R^Q
\end{eqnarray*}
with corresponding $Q \times Q$ covariance matrix
\begin{eqnarray*}
\V(h \mid S(\A)) = \wdot(\A)^{-1} \sum_{i \in \A} w_i \left(\y_i - \E(h \mid S(\A)) \right) \left(\y_i  - \E(h \mid S(\A))\right)^\top.
\end{eqnarray*}

With $A_b = \{i \mid \text{block}_i = b\}$ we get $\T = \sum_{b = 1}^B T(\A_b)$,
$\mub = \sum_{b = 1}^B \mub(\A_b)$ and $\Sigmab = \sum_{b = 1}^B \Sigmab(\A_b)$.

Having the conditional expectation and covariance at hand we are able to
standardize a linear statistic $\T \in \R^{PQ}$ of the form
(\ref{linstatsimple}). Univariate test statistics~$c$ mapping an observed linear
statistic $\t \in
\R^{PQ}$ into the real line can be of arbitrary form.  An obvious choice is
the maximum of the absolute values of the standardized linear statistic
\begin{eqnarray*}
c_\text{max}(\t, \mub, \Sigmab)  = \max \left| \frac{\t -
\mub}{\text{diag}(\Sigmab)^{1/2}} \right|
\end{eqnarray*}
utilizing the conditional expectation $\mub$ and covariance matrix
$\Sigmab$. The application of a quadratic form $c_\text{quad}(\t, \mub,
\Sigmab)  =
(\t - \mub) \Sigmab^+ (\t - \mub)^\top$ is one alternative, although
computationally more expensive because the Moore-Penrose
inverse $\Sigmab^+$ of $\Sigmab$ is involved.

The definition of one- and two-sided $p$-values used for the computations in
the \pkg{libcoin} package is
\begin{eqnarray*}
P(c(\T, \mub, \Sigmab) &\le& c(\t, \mub, \Sigmab)) \quad \text{(less)} \\
P(c(\T, \mub, \Sigmab) &\ge& c(\t, \mub, \Sigmab)) \quad \text{(greater)}\\
P(|c(\T, \mub, \Sigmab)| &\le& |c(\t, \mub, \Sigmab)|) \quad \text{(two-sided).}
\end{eqnarray*}
Note that for quadratic forms only two-sided $p$-values are available
and that in the one-sided case maximum type test statistics are replaced by
\begin{eqnarray*}
\min \left( \frac{\t - \mub}{\text{diag}(\Sigmab)^{1/2}} \right)
    \quad \text{(less) and }
\max \left( \frac{\t - \mub}{\text{diag}(\Sigmab)^{1/2}} \right)
    \quad \text{(greater).}
\end{eqnarray*}

This single source file implements and documents the \pkg{libcoin} package
following the literate programming paradigm. The keynote lecture on literate
programming by Donald E.~Knuth given at useR! 2016 in Stanford very much
motivated this little experiment.

\chapter{\proglang{R} Code}

\section{\proglang{R} User Interface}

@o libcoin.R -cp
@{
@<R Header@>
@<LinStatExpCov@>
@<LinStatExpCov1d@>
@<LinStatExpCov2d@>
@<vcov LinStatExpCov@>
@<doTest@>
@<Contrasts@>
@}

The \pkg{libcoin} package implements two \proglang{R} functions, \code{LinStatExpCov()} and
\code{doTest()} for the computation of linear statistics, their expectation
and covariance as well as for the computation of test statistics and
$p$-values. There are two interfaces: One (labelled ``1d'') when the data is
available as matrices \code{X} and \code{Y}, both with the same number of
rows $N$. The second interface (labelled ``2d'') handles the case when the
data is available in aggregated form; details will be explained later.

@d LinStatExpCov Prototype
@{(X, Y, ix = NULL, iy = NULL, weights = integer(0),
 subset = integer(0), block = integer(0), checkNAs = TRUE,
 varonly = FALSE, nresample = 0, standardise = FALSE,
 tol = sqrt(.Machine$double.eps))@}

@d LinStatExpCov
@{
LinStatExpCov <-
function@<LinStatExpCov Prototype@>
{
    if (missing(X) && !is.null(ix) && is.null(iy)) {
        X <- ix
        ix <- NULL
    }

    if (missing(X)) X <- integer(0)

    ## <FIXME> for the time being only!!! </FIXME>
##    if (length(subset) > 0) subset <- sort(subset)

    if (is.null(ix) && is.null(iy))
        .LinStatExpCov1d(X = X, Y = Y,
                         weights = weights, subset = subset,
                         block = block, checkNAs = checkNAs,
                         varonly = varonly, nresample = nresample,
                         standardise = standardise, tol = tol)
    else if (!is.null(ix) && !is.null(iy))
        .LinStatExpCov2d(X = X, Y = Y, ix = ix, iy = iy,
                         weights = weights, subset = subset,
                         block = block, checkNAs = checkNAs,
                         varonly = varonly, nresample = nresample,
                         standardise = standardise, tol = tol)
    else
        stop("incorrect call to ", sQuote("LinStatExpCov()"))
}
@}

\subsection{One-Dimensional Case (``1d'')}

We assume that $\x_i$ and $\y_i$ for $i = 1, \dots, N$ are available as
numeric matrices \code{X} and \code{Y} with $N$ rows as well as $P$ and $Q$
columns, respectively. The special case of a dummy matrix
\code{X} with $P$ columns can also be represented by a factor at $P$ levels.
The vector of case weights \code{weights} can be stored as \code{integer}
or \code{double} (possibly resulting from an aggregation of $N > $
\code{INT_MAX} observations). The subset vector \code{subset} may contain
the elements $1, \dots, N$ as \code{integer} or \code{double} (for $N > $
\code{INT_MAX}) and can be longer than $N$. The \code{subset} vector MUST be
sorted. \code{block} is a factor at $B$ levels of length $N$.

@d Check weights, subset, block
@{
if (is.null(weights)) weights <- integer(0)

if (length(weights) > 0) {
    if (!((N == length(weights)) && all(weights >= 0)))
        stop("incorrect weights")
    if (checkNAs) stopifnot(!anyNA(weights))
}

if (is.null(subset)) subset <- integer(0)

if (length(subset) > 0 && checkNAs) {
    rs <- range(subset)
    if (anyNA(rs)) stop("no missing values allowed in subset")
    if (!((rs[2] <= N) && (rs[1] >= 1L)))
        stop("incorrect subset")
}

if (is.null(block)) block <- integer(0)

if (length(block) > 0) {
    if (!((N == length(block)) && is.factor(block)))
        stop("incorrect block")
    if (checkNAs) stopifnot(!anyNA(block))
}
@}

Missing values are only allowed in \code{X} and
\code{Y}, all other vectors must not contain \code{NA}s. Missing values are
dealt with by excluding the corresponding observations from the subset
vector.

@d Handle Missing Values
@{
ms <- !complete.cases(X, Y)
if (all(ms))
    stop("all observations are missing")
if (any(ms)) {
    if (length(subset) > 0) {
        if (all(subset %in% which(ms)))
            stop("all observations are missing")
        subset <- subset[!(subset %in% which(ms))]
    } else {
        subset <- seq_len(N)[-which(ms)]
    }
}
@}

The logical argument \code{varonly} triggers the computation of the diagonal
elements of the covariance matrix $\Sigmab$ only. \code{nresample} permuted linear statistics
under the null hypothesis $H_0$ are returned on the original and
standardised scale (the latter only when \code{standardise} is \code{TRUE}).
Variances smaller than \code{tol} are treated as being zero.

@d LinStatExpCov1d
@{
.LinStatExpCov1d <-
function(X, Y, weights = integer(0), subset = integer(0), block = integer(0),
         checkNAs = TRUE, varonly = FALSE, nresample = 0, standardise = FALSE,
         tol = sqrt(.Machine$double.eps))
{
    if (NROW(X) != NROW(Y))
        stop("dimensions of X and Y don't match")
    N <- NROW(X)

    if (is.integer(X)) {
        if (is.null(attr(X, "levels")) || checkNAs) {
            rg <- range(X)
            if (anyNA(rg))
                stop("no missing values allowed in X")
            stopifnot(rg[1] > 0) # no missing values allowed here!
            if (is.null(attr(X, "levels")))
                attr(X, "levels") <- seq_len(rg[2])
        }
    }

    if (is.factor(X) && checkNAs)
        stopifnot(!anyNA(X))

    @<Check weights, subset, block@>

    if (checkNAs) {
        @<Handle Missing Values@>
    }

    ret <- .Call(R_ExpectationCovarianceStatistic, X, Y, weights, subset,
                 block, as.integer(varonly), as.double(tol))
    ret$varonly <- as.logical(ret$varonly)
    ret$Xfactor <- as.logical(ret$Xfactor)
    if (nresample > 0) {
        ret$PermutedLinearStatistic <-
            .Call(R_PermutedLinearStatistic, X, Y, weights, subset,
                  block, as.double(nresample))
        if (standardise)
            ret$StandardisedPermutedLinearStatistic <-
                .Call(R_StandardisePermutedLinearStatistic, ret)
    }
    class(ret) <- c("LinStatExpCov1d", "LinStatExpCov")
    ret
}
@}

Here is a simple example. We have five groups and a uniform outcome (rounded
to one digit) and want
to test independence of group membership and outcome. The simplest way is
to set-up the dummy matrix explicitly:
<<1dex-1>>=
isequal <-
function(a, b) {
    attributes(a) <- NULL
    attributes(b) <- NULL
    if (!isTRUE(all.equal(a, b))) {
        print(a, digits = 10)
        print(b, digits = 10)
        FALSE
    } else
        TRUE
}
library("libcoin")
set.seed(290875)
x <- gl(5, 20)
y <- round(runif(length(x)), 1)
ls1 <- LinStatExpCov(X = model.matrix(~ x - 1), Y = matrix(y, ncol = 1))
ls1$LinearStatistic
tapply(y, x, sum)
@@
The linear statistic is simply the sum of the response in each group.
Alternatively, we can compute the same object without setting-up the dummy
matrix:
<<1dex-2>>=
ls2 <- LinStatExpCov(X = x, Y = matrix(y, ncol = 1))
all.equal(ls1[-grep("Xfactor", names(ls1))],
          ls2[-grep("Xfactor", names(ls2))])
@@
The results are identical, except for a logical indicating that a factor was
used to represent the dummy matrix \code{X}.

\subsection{Two-Dimensional Case (``2d'')}

Sometimes the data takes only a few unique values and considerable
computational speedups can be achieved taking this information into account.
Let \code{ix} denote an integer vector with elements $0, \dots, L_x$ of
length $N$ and
\code{iy} an integer vector with elements $0, \dots, L_y$, also of length
$N$. The matrix
\code{X} is now of dimension $(L_x + 1) \times P$ and the matrix \code{Y}
of dimension $(L_y + 1) \times Q$. The combination of \code{X} and \code{ix}
means that the $i$th observation corresponds to the row \code{X[ix[i] + 1,]}.
This looks cumbersome in \proglang{R} notation but is a very efficient way
of dealing with missing values at \proglang{C} level. By convention,
elements of \code{ix} being zero denote a missing value (\code{NA}s are not
allowed in \code{ix} and \code{iy}). Thus, the first row of \code{X}
corresponds to a missing value. If the first row is simply zero, missing
values do not contribute to any of the sums computed later. Even more
important is the fact that all entities, such as linear statistics etc., can
be computed from the two-way tabulation (therefore the abbrevation ``2d'')
over the $N$ elements of \code{ix} and \code{iy}. Once such a
table was computed, the remaining computations can be performed in
dimension $L_x \times L_y$, typically much smaller than $N$.

@d LinStatExpCov2d
@{
.LinStatExpCov2d <-
function(X = numeric(0), Y, ix, iy, weights = integer(0), subset = integer(0),
         block = integer(0), checkNAs = TRUE, varonly = FALSE, nresample = 0,
         standardise = FALSE, tol = sqrt(.Machine$double.eps))
{
    IF <- function(x) is.integer(x) || is.factor(x)

    if (!((length(ix) == length(iy)) && IF(ix) && IF(iy)))
        stop("incorrect ix and/or iy")
    N <- length(ix)

    @<Check ix@>

    @<Check iy@>

    if (length(X) > 0) {
        if (!(NROW(X) == (length(attr(ix, "levels")) + 1) &&
              all(complete.cases(X))))
            stop("incorrect X")
    }

    if (!(NROW(Y) == (length(attr(iy, "levels")) + 1) &&
          all(complete.cases(Y))))
        stop("incorrect Y")

    @<Check weights, subset, block@>

    ret <- .Call(R_ExpectationCovarianceStatistic_2d, X, ix, Y, iy,
                 weights, subset, block, as.integer(varonly), as.double(tol))
    ret$varonly <- as.logical(ret$varonly)
    ret$Xfactor <- as.logical(ret$Xfactor)
    if (nresample > 0) {
        ret$PermutedLinearStatistic <-
            .Call(R_PermutedLinearStatistic_2d, X, ix, Y, iy, block, nresample, ret$Table)
        if (standardise)
            ret$StandardisedPermutedLinearStatistic <-
                .Call(R_StandardisePermutedLinearStatistic, ret)
    }
    class(ret) <- c("LinStatExpCov2d", "LinStatExpCov")
    ret
}
@}

\code{ix} and \code{iy} can be factors but without any missing values

@d Check ix
@{
if (is.null(attr(ix, "levels"))) {
    rg <- range(ix)
    if (anyNA(rg))
        stop("no missing values allowed in ix")
    stopifnot(rg[1] >= 0)
    attr(ix, "levels") <- seq_len(rg[2])
} else {
    ## lev can be data.frame (see inum::inum)
    lev <- attr(ix, "levels")
    if (!is.vector(lev)) lev <- seq_len(NROW(lev))
    attr(ix, "levels") <- lev
    if (checkNAs) stopifnot(!anyNA(ix))
}
@}

@d Check iy
@{
if (is.null(attr(iy, "levels"))) {
    rg <- range(iy)
    if (anyNA(rg))
        stop("no missing values allowed in iy")
    stopifnot(rg[1] >= 0)
    attr(iy, "levels") <- seq_len(rg[2])
} else {
    ## lev can be data.frame (see inum::inum)
    lev <- attr(iy, "levels")
    if (!is.vector(lev)) lev <- seq_len(NROW(lev))
    attr(iy, "levels") <- lev
    if (checkNAs) stopifnot(!anyNA(iy))
}
@}

In our small example, we can set-up the data in the following way
<<2dex-1>>=
X <- rbind(0, diag(nlevels(x)))
ix <- unclass(x)
ylev <- sort(unique(y))
Y <- rbind(0, matrix(ylev, ncol = 1))
iy <- .bincode(y, breaks = c(-Inf, ylev, Inf))
ls3 <- LinStatExpCov(X = X, ix = ix, Y = Y, iy = iy)
all.equal(ls1[-grep("Table", names(ls1))],
          ls3[-grep("Table", names(ls3))])
### works also with factors
ls3 <- LinStatExpCov(X = X, ix = factor(ix), Y = Y, iy = factor(iy))
all.equal(ls1[-grep("Table", names(ls1))],
          ls3[-grep("Table", names(ls3))])
@@
Similar to the one-dimensional case, we can also omit the \code{X} matrix
here
<<2dex-2>>=
ls4 <- LinStatExpCov(ix = ix, Y = Y, iy = iy)
all.equal(ls3[-grep("Xfactor", names(ls3))],
          ls4[-grep("Xfactor", names(ls4))])
@@
It is important to note that all computations are based on the tabulations
<<2dex-3>>=
ls3$Table
xtabs(~ ix + iy)
@@
where the former would record missing values in the first row / column.

\subsection{Methods and Tests}

Objects of class \code{"LinStatExpCov"} returned by \code{LinStatExpCov()}
contain the symmetric covariance matrix as a vector of the lower triangular
elements. The \code{vcov} method allows to extract the full covariance
matrix from such an object.

@d vcov LinStatExpCov
@{
vcov.LinStatExpCov <-
function(object, ...)
{
    if (object$varonly)
        stop("cannot extract covariance matrix")
    drop(.Call(R_unpack_sym, object$Covariance, NULL, 0L))
}
@}

<<vcov-1>>=
ls1$Covariance
vcov(ls1)
@@

The most important task is, however, to compute test statistics and
$p$-values. \code{doTest()} allows to compute the statistics $c_\text{max}$
(taking \code{alternative} into account) and $c_\text{quad}$ along with the
corresponding $p$-values. If \code{nresample = 0} was used in the call to
\code{LinStatExpCov()}, $p$-values are obtained from the limiting asymptotic
distribution whenever such a thing is available at reasonable costs.
Otherwise, the permutation $p$-value is returned (along with the permuted
test statistics when \code{PermutedStatistics} is \code{TRUE}). The
$p$-values (\code{lower = FALSE}) or $(1 - p)$-values (\code{lower = TRUE})
can be computed on the log-scale.

@d doTest Prototype
@{(object, teststat = c("maximum", "quadratic", "scalar"),
 alternative = c("two.sided", "less", "greater"), pvalue = TRUE,
 lower = FALSE, log = FALSE, PermutedStatistics = FALSE,
 minbucket = 10L, ordered = TRUE, maxselect = object$Xfactor,
 pargs = GenzBretz())@}

@d doTest
@{
### note: lower = FALSE => p-value; lower = TRUE => 1 - p-value
doTest <-
function@<doTest Prototype@>
{
    teststat <- match.arg(teststat, choices = c("maximum", "quadratic", "scalar"))
    if (!any(teststat == c("maximum", "quadratic", "scalar")))
        stop("incorrect teststat")
    alternative <- alternative[1]
    if (!any(alternative == c("two.sided", "less", "greater")))
        stop("incorrect alternative")

    if (maxselect)
        stopifnot(object$Xfactor)

    if (teststat == "quadratic" || maxselect) {
        if (alternative != "two.sided")
            stop("incorrect alternative")
    }

    test <- which(c("maximum", "quadratic", "scalar") == teststat)
    if (test == 3) {
        if (length(object$LinearStatistic) != 1)
            stop("scalar test statistic not applicable")
        test <- 1L # scalar is maximum internally
    }
    alt <- which(c("two.sided", "less", "greater") == alternative)

    if (!pvalue && (NCOL(object$PermutedLinearStatistic) > 0))
        object$PermutedLinearStatistic <- matrix(NA_real_, nrow = 0, ncol = 0)

    if (!maxselect) {
        if (teststat == "quadratic") {
            ret <- .Call(R_QuadraticTest, object, as.integer(pvalue), as.integer(lower),
                         as.integer(log), as.integer(PermutedStatistics))
        } else {
            ret <- .Call(R_MaximumTest, object, as.integer(alt), as.integer(pvalue),
                         as.integer(lower), as.integer(log), as.integer(PermutedStatistics),
                         as.integer(pargs$maxpts), as.double(pargs$releps),
                         as.double(pargs$abseps))
            if (teststat == "scalar") {
                var <- if (object$varonly) object$Variance else object$Covariance
                ret$TestStatistic <- object$LinearStatistic - object$Expectation
                ret$TestStatistic <-
                    if (var > object$tol) ret$TestStatistic / sqrt(var) else NaN
            }
        }
    } else {
        ret <- .Call(R_MaximallySelectedTest, object, as.integer(ordered), as.integer(test),
                     as.integer(minbucket), as.integer(lower), as.integer(log))
    }
    if (!PermutedStatistics) ret$PermutedStatistics <- NULL
    ret
}
@}

<<doTest-1>>=
### c_max test statistic
### no p-value
doTest(ls1, teststat = "maximum", pvalue = FALSE)
### p-value
doTest(ls1, teststat = "maximum")
### log(p)-value
doTest(ls1, teststat = "maximum", log = TRUE)
### (1-p)-value
doTest(ls1, teststat = "maximum", lower = TRUE)
### log(1 - p)-value
doTest(ls1, teststat = "maximum", lower = TRUE, log = TRUE)
### quadratic
doTest(ls1, teststat = "quadratic")
@@

Sometimes we are interested in contrasts of linear statistics and their
corresponding properties. Examples include linear-by-linear association
tests, where we assign numeric scores to each level of a factor. To
implement this, we implement \code{lmult()} so that we can then left-multiply a
matrix to an object of class \code{"LinStatExpCov"}.

@d Contrasts
@{
lmult <-
function(x, object)
{
    stopifnot(!object$varonly)
    stopifnot(is.numeric(x))
    if (is.vector(x)) x <- matrix(x, nrow = 1)
    P <- object$dimension[1]
    stopifnot(ncol(x) == P)
    Q <- object$dimension[2]
    ret <- object
    xLS <- x %*% matrix(object$LinearStatistic, nrow = P)
    xExp <- x %*% matrix(object$Expectation, nrow = P)
    xExpX <- x %*% matrix(object$ExpectationX, nrow = P)
    if (Q == 1) {
        xCov <- tcrossprod(x %*% vcov(object), x)
    } else {
        zmat <- matrix(0, nrow = P * Q, ncol = nrow(x))
        mat <- rbind(t(x), zmat)
        mat <- mat[rep.int(seq_len(nrow(mat)), Q - 1),, drop = FALSE]
        mat <- rbind(mat, t(x))
        mat <- matrix(mat, ncol = Q * nrow(x))
        mat <- t(mat)
        xCov <- tcrossprod(mat %*% vcov(object), mat)
    }
    if (!is.matrix(xCov)) xCov <- matrix(xCov)
    if (length(object$PermutedLinearStatistic) > 0) {
        xPS <- apply(object$PermutedLinearStatistic, 2, function(y)
                     as.vector(x %*% matrix(y, nrow = P)))
        if (!is.matrix(xPS)) xPS <- matrix(xPS, nrow = 1)
        ret$PermutedLinearStatistic <- xPS
    }
    ret$LinearStatistic <- as.vector(xLS)
    ret$Expectation <- as.vector(xExp)
    ret$ExpectationX <- as.vector(xExpX)
    ret$Covariance <- as.vector(xCov[lower.tri(xCov, diag = TRUE)])
    ret$Variance <- diag(xCov)
    ret$dimension <- c(NROW(x), Q)
    ret$Xfactor <- FALSE
    if (length(object$StandardisedPermutedLinearStatistic) > 0)
        ret$StandardisedPermutedLinearStatistic <-
            .Call(R_StandardisePermutedLinearStatistic, ret)
    ret
}
@}

Here is an example for a linear-by-linear association test.
<<Contrasts-1>>=
set.seed(29)
ls1d <- LinStatExpCov(X = model.matrix(~ x - 1), Y = matrix(y, ncol = 1),
                      nresample = 10, standardise = TRUE)
set.seed(29)
ls1s <- LinStatExpCov(X = as.double(1:5)[x], Y = matrix(y, ncol = 1),
                      nresample = 10, standardise = TRUE)
ls1c <- lmult(1:5, ls1d)
stopifnot(isequal(ls1c, ls1s))
set.seed(29)
ls1d <- LinStatExpCov(X = model.matrix(~ x - 1), Y = matrix(c(y, y), ncol = 2),
                      nresample = 10, standardise = TRUE)
set.seed(29)
ls1s <- LinStatExpCov(X = as.double(1:5)[x], Y = matrix(c(y, y), ncol = 2),
                      nresample = 10, standardise = TRUE)
ls1c <- lmult(1:5, ls1d)
stopifnot(isequal(ls1c, ls1s))
@@

\subsection{Tabulations}

The tabulation of \code{ix} and \code{iy} can be computed without
necessarily computing the corresponding linear statistics via
\code{ctabs()}.

@d ctabs Prototype
@{(ix, iy = integer(0), block = integer(0), weights = integer(0),
 subset = integer(0), checkNAs = TRUE)@}

@o ctabs.R -cp
@{
@<R Header@>
ctabs <-
function@<ctabs Prototype@>
{
    stopifnot(is.integer(ix) || is.factor(ix))
    N <- length(ix)

    @<Check ix@>

    if (length(iy) > 0) {
        stopifnot(length(iy) == N)
        stopifnot(is.integer(iy) || is.factor(iy))
        @<Check iy@>
    }

    @<Check weights, subset, block@>

    if (length(iy) == 0)
        if (length(block) == 0)
            .Call(R_OneTableSums, ix, weights, subset)
        else
            .Call(R_TwoTableSums, ix, block, weights, subset)[, -1, drop = FALSE]
    else if (length(block) == 0)
        .Call(R_TwoTableSums, ix, iy, weights, subset)
    else
        .Call(R_ThreeTableSums, ix, iy, block, weights, subset)
}
@}

<<ctabsex-1>>=
t1 <- ctabs(ix = ix, iy = iy)
t2 <- xtabs(~ ix + iy)
max(abs(t1[-1, -1] - t2))
@@

\section{Manual Pages}

@o LinStatExpCov.Rd -cp
@{
\name{LinStatExpCov}
\alias{LinStatExpCov}
\alias{lmult}
\title{
  Linear Statistics with Expectation and Covariance
}
\description{
  Strasser-Weber type linear statistics and their expectation
  and covariance under the independence hypothesis
}
\usage{
LinStatExpCov@<LinStatExpCov Prototype@>
lmult(x, object)
}
\arguments{
  \item{X}{numeric matrix of transformations.}
  \item{Y}{numeric matrix of influence functions.}
  \item{ix}{an optional integer vector expanding \code{X}.}
  \item{iy}{an optional integer vector expanding \code{Y}.}
  \item{weights}{an optional integer vector of non-negative case weights.}
  \item{subset}{an optional integer vector defining a subset of observations.}
  \item{block}{an optional factor defining independent blocks of observations.}
  \item{checkNAs}{a logical for switching off missing value checks.  This
    included switching off checks for suitable values of \code{subset}.
    Use at your own risk.}
  \item{varonly}{a logical asking for variances only.}
  \item{nresample}{an integer defining the number of permuted statistics to draw.}
  \item{standardise}{a logical asking to standardise the permuted statistics.}
  \item{tol}{tolerance for zero variances.}
  \item{x}{a contrast matrix to be left-multiplied in case \code{X} was a factor.}
  \item{object}{an object of class \code{"LinStatExpCov"}.}
}
\details{
  The function, after minimal preprocessing, calls the underlying C code
  and computes the linear statistic, its expectation and covariance and,
  optionally, \code{nresample} samples from its permutation distribution.

  When both \code{ix} and \code{iy} are missing, the number of rows of
  \code{X} and \code{Y} is the same, ie the number of observations.

  When \code{X} is missing and \code{ix} a factor, the code proceeds as
  if \code{X} were a dummy matrix of \code{ix} without explicitly
  computing this matrix.

  Both \code{ix} and \code{iy} being present means the code treats them
  as subsetting vectors for \code{X} and \code{Y}.  Note that \code{ix = 0}
  or \code{iy = 0} means that the corresponding observation is missing
  and the first row or \code{X} and \code{Y} must be zero.

  \code{lmult} allows left-multiplication of a contrast matrix when \code{X}
  was (equivalent to) a factor.
}
\value{
  A list.
}
\references{
  Strasser, H. and Weber, C.  (1999).  On the asymptotic theory of permutation
  statistics.  \emph{Mathematical Methods of Statistics} \bold{8}(2), 220--250.
}
\examples{
wilcox.test(Ozone ~ Month, data = airquality, subset = Month \%in\% c(5, 8),
            exact = FALSE, correct = FALSE)

aq <- subset(airquality, Month \%in\% c(5, 8))
X <- as.double(aq$Month == 5)
Y <- as.double(rank(aq$Ozone, na.last = "keep"))
doTest(LinStatExpCov(X, Y))
}
\keyword{htest}
@}

@o doTest.Rd -cp
@{
\name{doTest}
\alias{doTest}
\title{
  Permutation Test
}
\description{
  Perform permutation test for a linear statistic
}
\usage{
doTest@<doTest Prototype@>
}
\arguments{
  \item{object}{an object returned by \code{\link{LinStatExpCov}}.}
  \item{teststat}{type of test statistic to use.}
  \item{alternative}{alternative for scalar or maximum-type statistics.}
  \item{pvalue}{a logical indicating if a p-value shall be computed.}
  \item{lower}{a logical indicating if a p-value (\code{lower} is \code{FALSE})
    or 1 - p-value (\code{lower} is \code{TRUE}) shall be returned.}
  \item{log}{a logical, if \code{TRUE} probabilities are log-probabilities.}
  \item{PermutedStatistics}{a logical, return permuted test statistics.}
  \item{minbucket}{minimum weight in either of two groups for maximally selected
    statistics.}
  \item{ordered}{a logical, if \code{TRUE} maximally selected statistics assume
    that the cutpoints are ordered.}
  \item{maxselect}{a logical, if \code{TRUE} maximally selected statistics are
    computed.  This requires that \code{X} was an implicitly defined design
    matrix in \code{\link{LinStatExpCov}}.}
  \item{pargs}{arguments as in \code{\link[mvtnorm:algorithms]{GenzBretz}}.}
}
\details{
  Computes a test statistic, a corresponding p-value and, optionally, cutpoints
  for maximally selected statistics.
}
\value{
  A list.
}
\keyword{htest}
@}

@o ctabs.Rd -cp
@{
\name{ctabs}
\alias{ctabs}
\title{
  Cross Tabulation
}
\description{
  Efficient weighted cross tabulation of two factors and a block
}
\usage{
ctabs@<ctabs Prototype@>
}
\arguments{
  \item{ix}{a integer of positive values with zero indicating a missing.}
  \item{iy}{an optional integer of positive values with zero indicating a
    missing.}
  \item{block}{an optional blocking factor without missings.}
  \item{weights}{an optional vector of case weights, integer or double.}
  \item{subset}{an optional integer vector indicating a subset.}
  \item{checkNAs}{a logical for switching off missing value checks.}
}
\details{
  A faster version of \code{xtabs(weights ~ ix + iy + block, subset)}.
}
\value{
  If \code{block} is present, a three-way table. Otherwise,
  a one- or two-dimensional table.
}
\examples{
ctabs(ix = 1:5, iy = 1:5, weights = 1:5 / 5)
}
\keyword{univar}
@}

\chapter{\proglang{C} Code}

The main motivation to implement the \pkg{libcoin} package comes from the
demand to compute high-dimensional linear statistics (with large $P$ and
$Q$) and the corresponding test statistics very often, either for sampling
from the permutation null distribution $H_0$ or for different subsets of the
data. Especially the latter task can be performed \emph{without} actually
subsetting the data via the \code{subset} argument very efficiently (in
terms of memory consumption and, depending on the circumstances, speed).

We start with the definition of some macros and global variables in the
header files.

\section{Header and Source Files}

@o libcoin_internal.h -cc
@{
@<C Header@>
@<R Includes@>
@<C Macros@>
@<C Global Variables@>
@}

These includes provide some \proglang{R} infrastructure at \proglang{C}
level.

@d R Includes
@{
#define STRICT_R_HEADERS
#define USE_FC_LEN_T
#include <float.h>        /* for DBL_MIN */
#include <R.h>
#include <Rinternals.h>
#include <Rversion.h>     /* for R_VERSION */
#include <R_ext/Lapack.h> /* for dspev */
#ifndef FCONE
# define FCONE
#endif
@}

We need three macros: \code{S} computes the element $\sigma_{ij}$ of a
symmetric $n \times n$ matrix when only the lower triangular elements are
stored. \code{LE} implements $\le$ with some tolerance, \code{GE} implements
$\ge$.

@d C Macros
@{
#define S(i, j, n) ((i) >= (j) ? (n) * (j) + (i) - (j) * ((j) + 1) / 2 : (n) * (i) + (j) - (i) * ((i) + 1) / 2)
#define LE(x, y, tol) ((x) < (y)) || (fabs((x) - (y)) < (tol))
#define GE(x, y, tol) ((x) > (y)) || (fabs((x) - (y)) < (tol))
@| S LE GE
@}

@d C Global Variables
@{
#define ALTERNATIVE_twosided				1
#define ALTERNATIVE_less				2
#define ALTERNATIVE_greater				3

#define TESTSTAT_maximum				1
#define TESTSTAT_quadratic				2

#define LinearStatistic_SLOT				0
#define Expectation_SLOT				1
#define Covariance_SLOT					2
#define Variance_SLOT					3
#define ExpectationX_SLOT				4
#define varonly_SLOT					5
#define dim_SLOT					6
#define ExpectationInfluence_SLOT			7
#define CovarianceInfluence_SLOT			8
#define VarianceInfluence_SLOT				9
#define Xfactor_SLOT					10
#define tol_SLOT					11
#define PermutedLinearStatistic_SLOT			12
#define StandardisedPermutedLinearStatistic_SLOT	13
#define TableBlock_SLOT					14
#define Sumweights_SLOT					15
#define Table_SLOT					16

#define DoSymmetric					1
#define DoCenter 					1
#define DoVarOnly 					1
#define Power1 						1
#define Power2 						2
#define Offset0 					0
@| LinearStatistic_SLOT Expectation_SLOT Covariance_SLOT Variance_SLOT
ExpectationX_SLOT varonly_SLOT dim_SLOT
ExpectationInfluence_SLOT CovarianceInfluence_SLOT VarianceInfluence_SLOT
Xfactor_SLOT tol_SLOT PermutedLinearStatistic_SLOT StandardisedPermutedLinearStatistic_SLOT
TableBlock_SLOT Sumweights_SLOT Table_SLOT DoSymmetric DoCenter DoVarOnly Power1
Power2 Offset0
@}

The corresponding header file contains definitions of
functions that can be called via \code{.Call()} from the \pkg{libcoin}
package. In addition, packages linking to \pkg{libcoin} can access these
function at \proglang{C} level (at your own risk, of course!).

@o libcoin.h -cc
@{
@<C Header@>
#include "libcoin_internal.h"
@<Function Prototypes@>
@}

@d Function Prototypes
@{
extern @<R_ExpectationCovarianceStatistic Prototype@>;
extern @<R_PermutedLinearStatistic Prototype@>;
extern @<R_StandardisePermutedLinearStatistic Prototype@>;
extern @<R_ExpectationCovarianceStatistic_2d Prototype@>;
extern @<R_PermutedLinearStatistic_2d Prototype@>;
extern @<R_QuadraticTest Prototype@>;
extern @<R_MaximumTest Prototype@>;
extern @<R_MaximallySelectedTest Prototype@>;
extern @<R_ExpectationInfluence Prototype@>;
extern @<R_CovarianceInfluence Prototype@>;
extern @<R_ExpectationX Prototype@>;
extern @<R_CovarianceX Prototype@>;
extern @<R_Sums Prototype@>;
extern @<R_KronSums Prototype@>;
extern @<R_KronSums_Permutation Prototype@>;
extern @<R_colSums Prototype@>;
extern @<R_OneTableSums Prototype@>;
extern @<R_TwoTableSums Prototype@>;
extern @<R_ThreeTableSums Prototype@>;
extern @<R_order_subset_wrt_block Prototype@>;
extern @<R_quadform Prototype@>;
extern @<R_kronecker Prototype@>;
extern @<R_MPinv_sym Prototype@>;
extern @<R_unpack_sym Prototype@>;
extern @<R_pack_sym Prototype@>;
@}

The \proglang{C} file \file{libcoin.c} contains all \proglang{C}
functions and corresponding \proglang{R} interfaces.

@o libcoin.c -cc
@{
@<C Header@>
#include "libcoin_internal.h"
#include <R_ext/stats_stubs.h> 	/* for S_rcont2 */
#include <mvtnormAPI.h>		/* for calling mvtnorm */
@<Function Definitions@>
@}

@d Function Definitions
@{
@<MoreUtils@>
@<Memory@>
@<P-Values@>
@<KronSums@>
@<colSums@>
@<SimpleSums@>
@<Tables@>
@<Utils@>
@<LinearStatistics@>
@<Permutations@>
@<ExpectationCovariances@>
@<Test Statistics@>
@<User Interface@>
@<2d User Interface@>
@<Tests@>
@}

\section{Variables}

$N$ is the number of observations

@d R N Input
@{
SEXP N,
@|N
@}

which at \proglang{C} level is represented as \code{R_xlen_t} to allow for
$N > $ \code{INT_MAX}

@d C integer N Input
@{
R_xlen_t N
@|N
@}

The regressors $\x_i, i = 1, \dots, N$

@d R x Input
@{
SEXP x,
@|x
@}

are either represented as a real matrix with $N$ rows and $P$ columns

@d C integer P Input
@{
int P
@|P
@}

@d C real x Input
@{
double *x,
@<C integer N Input@>,
@<C integer P Input@>,
@|x
@}

or as a factor (an integer at \proglang{C} level) at $P$ levels

@d C integer x Input
@{
int *x,
@<C integer N Input@>,
@<C integer P Input@>,
@|x
@}

The influence functions are also either a $N \times Q$ real matrix

@d R y Input
@{
SEXP y,
@|y
@}

@d C integer Q Input
@{
int Q
@|Q
@}

@d C real y Input
@{
double *y,
@<C integer Q Input@>,
@|y
@}

or a factor at $Q$ levels

@d C integer y Input
@{
int *y,
@<C integer Q Input@>,
@|y
@}

The case weights $w_i, i = 1, \dots, N$

@d R weights Input
@{
SEXP weights
@|weights
@}

can be constant one (\code{XLENGTH(weights) == 0} or \code{weights = integer(0)})
or integer-valued, with \code{HAS_WEIGHTS == 0} in the former case

@d C integer weights Input
@{
int *weights,
int HAS_WEIGHTS,
@|weights, HAS_WEIGHTS
@}

Case weights larger than \code{INT_MAX} are stored as double

@d C real weights Input
@{
double *weights,
int HAS_WEIGHTS,
@|weights, HAS_WEIGHTS
@}

The sum of all case weights is a double

@d C sumweights Input
@{
double sumweights
@|sumweights
@}

Subsets $\A \subseteq \{1, \dots, N\}$ are \proglang{R} style indices

@d R subset Input
@{
SEXP subset
@|subset
@}

are either not existent (\code{XLENGTH(subset) == 0}) or of length

@d C integer Nsubset Input
@{
R_xlen_t Nsubset
@|Nsubset
@}

Optionally, one can specify a subset of the subset via

@d C subset range Input
@{
R_xlen_t offset,
@<C integer Nsubset Input@>
@|offset
@}

where \code{offset} is a \proglang{C} style index for \code{subset}.

Subsets are stored either as integer

@d C integer subset Input
@{
int *subset,
@<C subset range Input@>
@|subset
@}

or double (to allow for indices larger than \code{INT_MAX})

@d C real subset Input
@{
double *subset,
@<C subset range Input@>
@|subset
@}

Blocks $\text{block}_i, i = 1, \dots, N$

@d R block Input
@{
SEXP block
@|block
@}

at $B$ levels

@d C integer B Input
@{
int B
@|B
@}

are stored as a factor

@d C integer block Input
@{
int *block,
@<C integer B Input@>,
@|block
@}

The tabulation of block (potentially in subsets) is

@d R blockTable Input
@{
SEXP blockTable
@|blockTable
@}

where the table is of length $B + 1$ and the first element
counts the number of missing values (although these are NOT allowed in
block).

\subsection{Example Data and Code}

We start with setting-up some toy data sets to be used as test bed. The data
over both the 1d and the 2d case, including case weights, subsets and blocks.
<<ex-setup>>=
N <- 20L
P <- 3L
Lx <- 10L
Ly <- 5L
Q <- 4L
B <- 2L
iX2d <- rbind(0, matrix(runif(Lx * P), nrow = Lx))
ix <- sample(1:Lx, size = N, replace = TRUE)
levels(ix) <- 1:Lx
ixf <- factor(ix, levels = 1:Lx, labels = 1:Lx)
x <- iX2d[ix + 1,]
Xfactor <- diag(Lx)[ix,]
iY2d <- rbind(0, matrix(runif(Ly * Q), nrow = Ly))
iy <- sample(1:Ly, size = N, replace = TRUE)
levels(iy) <- 1:Ly
iyf <- factor(iy, levels = 1:Ly, labels = 1:Ly)
y <- iY2d[iy + 1,]
weights <- sample(0:5, size = N, replace = TRUE)
block <- sample(gl(B, ceiling(N / B))[1:N])
subset <- sort(sample(1:N, floor(N * 1.5), replace = TRUE))
subsety <- sample(1:N, floor(N * 1.5), replace = TRUE)
r1 <- rep(1:ncol(x), ncol(y))
r1Xfactor <- rep(1:ncol(Xfactor), ncol(y))
r2 <- rep(1:ncol(y), each = ncol(x))
r2Xfactor <- rep(1:ncol(y), each = ncol(Xfactor))
@@

As a benchmark, we implement linear statistics, their expectation and
covariance, taking case weights, subsets and blocks into account, at \proglang{R}
level. In a sense, the core of the \pkg{libcoin} package is ``just'' a less
memory-hungry and sometimes faster version of this simple function.

<<Rlibcoin>>=
LSEC <-
function(X, Y, weights = integer(0), subset = integer(0), block = integer(0))
{
    if (length(weights) == 0) weights <- rep.int(1, NROW(X))
    if (length(subset) == 0) subset <- seq_len(NROW(X))

    X <- X[subset,, drop = FALSE]
    Y <- Y[subset,, drop = FALSE]
    weights <- weights[subset]

    if (length(block) == 0) {
        w. <- sum(weights)
        wX <- weights * X
        wY <- weights * Y
        ExpX <- colSums(wX)
        ExpY <- colSums(wY) / w.
        CovX <- crossprod(X, wX)
        Yc <- t(t(Y) - ExpY)
        CovY <- crossprod(Yc, weights * Yc) / w.
        T <- crossprod(X, wY)
        Exp <- kronecker(ExpY, ExpX)
        Cov <- w. / (w. - 1) * kronecker(CovY, CovX) -
                1 / (w. - 1) * kronecker(CovY, tcrossprod(ExpX))

        list(LinearStatistic = as.vector(T), Expectation = as.vector(Exp),
             Covariance = Cov, Variance = diag(Cov))
    } else {
        block <- block[subset]
        ret <- list(LinearStatistic = 0, Expectation = 0,
                    Covariance = 0, Variance = 0)
        for (b in levels(block)) {
            tmp <- LSEC(X = X, Y = Y, weights = weights, subset = which(block == b))
            for (l in names(ret)) ret[[l]] <- ret[[l]] + tmp[[l]]
        }
        ret
    }
}
@@

<<cmpr>>=
cmpr <-
function(ret1, ret2)
{
    if (inherits(ret1, "LinStatExpCov")) {
        if (!ret1$varonly)
            ret1$Covariance <- vcov(ret1)
    }
    ret1 <- ret1[!sapply(ret1, is.null)]
    ret2 <- ret2[!sapply(ret2, is.null)]
    nm1 <- names(ret1)
    nm2 <- names(ret2)
    nm <- c(nm1, nm2)
    nm <- names(table(nm))[table(nm) == 2]
    isequal(ret1[nm], ret2[nm])
}
@@

We now compute the linear statistic along with corresponding expectation,
variance and covariance for later reuse.

<<benchmarks>>=
LECVxyws <- LinStatExpCov(x, y, weights = weights, subset = subset)
LEVxyws <- LinStatExpCov(x, y, weights = weights, subset = subset, varonly = TRUE)
@@

The following tests compare the high-level \proglang{R} implementation
(function \code{LSEC()}) with the 1d and 2d \proglang{C} level
implementations in the two sitations with and without specification of
\code{X} (ie, the dummy matrix in the latter case).

<<tests>>=
### with X given
testit <-
function(...)
{
    a <- LinStatExpCov(x, y, ...)
    b <- LSEC(x, y, ...)
    d <- LinStatExpCov(X = iX2d, ix = ix, Y = iY2d, iy = iy, ...)
    cmpr(a, b) && cmpr(d, b)
}

stopifnot(
    testit() && testit(weights = weights) &&
    testit(subset = subset) && testit(weights = weights, subset = subset) &&
    testit(block = block) && testit(weights = weights, block = block) &&
    testit(subset = subset, block = block) &&
    testit(weights = weights, subset = subset, block = block)
)

### without dummy matrix X
testit <-
function(...)
{
    a <- LinStatExpCov(X = ix, y, ...)
    b <- LSEC(Xfactor, y, ...)
    d <- LinStatExpCov(X = integer(0), ix = ix, Y = iY2d, iy = iy, ...)
    cmpr(a, b) && cmpr(d, b)
}

stopifnot(
    testit() && testit(weights = weights) &&
    testit(subset = subset) && testit(weights = weights, subset = subset) &&
    testit(block = block) && testit(weights = weights, block = block) &&
    testit(subset = subset, block = block) &&
    testit(weights = weights, subset = subset, block = block)
)
@@

All three implementations give the same results.

\section{Conventions}

Functions starting with \code{R_} are \proglang{C} functions callable via
\code{.Call()} from \proglang{R}. That means they all return \code{SEXP}.
These functions allocate memory handled by \proglang{R}.

Functions starting with \code{RC_} are \proglang{C} functions with
\code{SEXP} or pointer arguments and possibly an \code{SEXP} return value.

Functions starting with \code{C_} are \proglang{C} functions with pointer
arguments only and return a scalar or nothing.

Return values (arguments modified by a function) are named \code{ans},
sometimes with dimension (for example: \code{PQ_ans}).

\section{\proglang{C} User Interface}

\subsection{One-Dimensional Case (``1d'')}

@d User Interface
@{
@<RC_ExpectationCovarianceStatistic@>
@<R_ExpectationCovarianceStatistic@>
@<R_PermutedLinearStatistic@>
@<R_StandardisePermutedLinearStatistic@>
@}

The data are given as $\x_i$ and $\y_i$ for $i = 1, \dots, N$, optionally
with case weights, subset and blocks. The latter three variables are ignored when
specified as \code{integer(0)}.

@d User Interface Input
@{
@<R x Input@>
@<R y Input@>
@<R weights Input@>,
@<R subset Input@>,
@<R block Input@>,
@}

@d R_ExpectationCovarianceStatistic Prototype
@{
SEXP R_ExpectationCovarianceStatistic
(
    @<User Interface Input@>
    SEXP varonly,
    SEXP tol
)
@}

This function can be called from other packages.

@o libcoinAPI.h -cc
@{
@<C Header@>
#include <R_ext/Rdynload.h>
#include <libcoin.h>

extern SEXP libcoin_R_ExpectationCovarianceStatistic(
    SEXP x, SEXP y, SEXP weights, SEXP subset, SEXP block, SEXP varonly,
    SEXP tol
) {
    static SEXP(*fun)(SEXP, SEXP, SEXP, SEXP, SEXP, SEXP, SEXP) = NULL;
    if (fun == NULL)
        fun = (SEXP(*)(SEXP, SEXP, SEXP, SEXP, SEXP, SEXP, SEXP))
            R_GetCCallable("libcoin", "R_ExpectationCovarianceStatistic");
    return fun(x, y, weights, subset, block, varonly, tol);
}
@}

The \proglang{C} interface essentially sets-up the necessary memory and
calls a \proglang{C} level function for the computations.

@d R_ExpectationCovarianceStatistic
@{
@<R_ExpectationCovarianceStatistic Prototype@>
{
    SEXP ans;

    @<Setup Dimensions@>

    PROTECT(ans = RC_init_LECV_1d(P, Q, INTEGER(varonly)[0], B, TYPEOF(x) == INTSXP, REAL(tol)[0]));

    RC_ExpectationCovarianceStatistic(x, y, weights, subset, block, ans);

    UNPROTECT(1);
    return(ans);
}
@|R_ExpectationCovarianceStatistic
@}

$P$, $Q$ and $B$ are first extracted from the data. The case where \code{X}
is an implicitly specified dummy matrix, the dimension $P$ is the number of
levels of \code{x}.

@d Setup Dimensions
@{
int P, Q, B;

if (TYPEOF(x) == INTSXP) {
    P = NLEVELS(x);
} else {
    P = NCOL(x);
}
Q = NCOL(y);

B = 1;
if (LENGTH(block) > 0)
    B = NLEVELS(block);
@}

The core function first computes the linear statistic (as there is no need
to pay attention to blocks) and, in a second step, starts a loop over
potential blocks.

FIXME:  \code{x} being an integer (\code{Xfactor}) with some 0 elements is not handled
        correctly (as \code{sumweights} doesn't take this information into
        account; use \code{subset} to exclude these missings (as done in
        \code{LinStatExpCov()})

@d RC_ExpectationCovarianceStatistic
@{
void RC_ExpectationCovarianceStatistic
(
    @<User Interface Input@>
    SEXP ans
) {
    @<C integer N Input@>;
    @<C integer P Input@>;
    @<C integer Q Input@>;
    @<C integer B Input@>;
    double *sumweights, *table;
    double *ExpInf, *VarInf, *CovInf, *ExpX, *ExpXtotal, *VarX, *CovX;
    double *tmpV, *tmpCV;
    SEXP nullvec, subset_block;

    @<Extract Dimensions@>

    @<Compute Linear Statistic@>

    @<Setup Memory and Subsets in Blocks@>

    /* start with subset[0] */
    R_xlen_t offset = (R_xlen_t) table[0];

    for (int b = 0; b < B; b++) {

        @<Compute Sum of Weights in Block@>

        /* don't do anything for empty blocks or blocks with weight 1 */
        if (sumweights[b] > 1) {

            @<Compute Expectation Linear Statistic@>

            @<Compute Covariance Influence@>

            if (C_get_varonly(ans)) {
                @<Compute Variance Linear Statistic@>
            } else {
                @<Compute Covariance Linear Statistic@>
            }
        }

        /* next iteration starts with subset[cumsum(table[1:(b + 1)])] */
        offset += (R_xlen_t) table[b + 1];
    }

    @<Compute Variance from Covariance@>

    R_Free(ExpX); R_Free(VarX); R_Free(CovX);
    UNPROTECT(2);
}
@|RC_ExpectationCovarianceStatistic
@}

The dimensions are available from the return object:

@d Extract Dimensions
@{
P = C_get_P(ans);
Q = C_get_Q(ans);
N = NROW(x);
B = C_get_B(ans);
@}

The linear statistic $\T(\A)$ can be computed without taking blocks into account.

@d Compute Linear Statistic
@{
RC_LinearStatistic(x, N, P, REAL(y), Q, weights, subset,
                   Offset0, XLENGTH(subset),
                   C_get_LinearStatistic(ans));
@}

We next extract memory from the return object and allocate some additional
memory. The most important step is to tabulate blocks and to order the
subset with respect to blocks. In absense of block, this just returns
subset.

@d Setup Memory and Subsets in Blocks
@{
ExpInf = C_get_ExpectationInfluence(ans);
VarInf = C_get_VarianceInfluence(ans);
CovInf = C_get_CovarianceInfluence(ans);
ExpXtotal = C_get_ExpectationX(ans);
for (int p = 0; p < P; p++) ExpXtotal[p] = 0.0;
ExpX = R_Calloc(P, double);
/* Fix by Joanidis Kristoforos: P > INT_MAX is possible
   for maximally selected statistics (when X is an integer).
   2018-12-13 */
if (C_get_varonly(ans)) {
    VarX = R_Calloc(P, double);
    CovX = R_Calloc(1, double);
} else {
    VarX = R_Calloc(1, double);
    CovX = R_Calloc(PP12(P), double);
}
table = C_get_TableBlock(ans);
sumweights = C_get_Sumweights(ans);
PROTECT(nullvec = allocVector(INTSXP, 0));

if (B == 1) {
    table[0] = 0.0;
    table[1] = RC_Sums(N, nullvec, subset, Offset0, XLENGTH(subset));
} else {
    RC_OneTableSums(INTEGER(block), N, B + 1, nullvec, subset, Offset0,
                    XLENGTH(subset), table);
}
if (table[0] > 0)
    error("No missing values allowed in block");
PROTECT(subset_block = RC_order_subset_wrt_block(N, subset, block,
                                                 VECTOR_ELT(ans, TableBlock_SLOT)));
@}

We compute $\mub(\A)$ based on $\E(h \mid S(\A))$ and $\sum_{i \in \A} w_i \x_i$
for the subset given by subset and the $b$th level of block. The expectation
is initialised zero when $b = 0$ and values add-up over blocks.

@d Compute Sum of Weights in Block
@{
/* compute sum of case weights in block b of subset */
if (table[b + 1] > 0) {
    sumweights[b] = RC_Sums(N, weights, subset_block,
                            offset, (R_xlen_t) table[b + 1]);
} else {
    /* offset = something and Nsubset = 0 means Nsubset = N in
       RC_Sums; catch empty or zero-weight block levels here */
    sumweights[b] = 0.0;
}
@}

@d Compute Expectation Linear Statistic
@{
RC_ExpectationInfluence(N, y, Q, weights, subset_block, offset,
                        (R_xlen_t) table[b + 1], sumweights[b], ExpInf + b * Q);
RC_ExpectationX(x, N, P, weights, subset_block, offset,
                (R_xlen_t) table[b + 1], ExpX);
for (int p = 0; p < P; p++) ExpXtotal[p] += ExpX[p];
C_ExpectationLinearStatistic(P, Q, ExpInf + b * Q, ExpX, b,
                             C_get_Expectation(ans));
@}

The covariance $\V(h \mid S(\A))$ is now computed for the subset given by
subset and the $b$th level of block. Note that \code{CovInf} stores the
values for each block in the return object (for later reuse).

@d Compute Covariance Influence
@{
/* C_ordered_Xfactor and C_unordered_Xfactor need both VarInf and CovInf */
RC_CovarianceInfluence(N, y, Q, weights, subset_block, offset,
                      (R_xlen_t) table[b + 1], ExpInf + b * Q, sumweights[b],
                      !DoVarOnly, CovInf + b * Q * (Q + 1) / 2);
/* extract variance from covariance */
tmpCV = CovInf + b * Q * (Q + 1) / 2;
tmpV = VarInf + b * Q;
for (int q = 0; q < Q; q++) tmpV[q] = tmpCV[S(q, q, Q)];
@}

We can now compute the variance or covariance of the linear statistic
$\Sigmab(\A)$:

@d Compute Variance Linear Statistic
@{
RC_CovarianceX(x, N, P, weights, subset_block, offset,
               (R_xlen_t) table[b + 1], ExpX, DoVarOnly, VarX);
C_VarianceLinearStatistic(P, Q, VarInf + b * Q, ExpX, VarX, sumweights[b],
                          b, C_get_Variance(ans));
@}

@d Compute Covariance Linear Statistic
@{
RC_CovarianceX(x, N, P, weights, subset_block, offset,
               (R_xlen_t) table[b + 1], ExpX, !DoVarOnly, CovX);
C_CovarianceLinearStatistic(P, Q, CovInf + b * Q * (Q + 1) / 2,
                            ExpX, CovX, sumweights[b], b,
                            C_get_Covariance(ans));
@}

@d Compute Variance from Covariance
@{
/* always return variances */
if (!C_get_varonly(ans)) {
    for (int p = 0; p < mPQB(P, Q, 1); p++)
        C_get_Variance(ans)[p] = C_get_Covariance(ans)[S(p, p, mPQB(P, Q, 1))];
}
@}

The computation of permuted linear statistics is done outside this general
function. The user interface is the same, except for an additional number of
permutations to be specified.

@d R_PermutedLinearStatistic Prototype
@{
SEXP R_PermutedLinearStatistic
(
    @<User Interface Input@>
    SEXP nresample
)
@}

This function can be called from other packages.

@o libcoinAPI.h -cc
@{
extern SEXP libcoin_R_PermutedLinearStatistic(
    SEXP x, SEXP y, SEXP weights, SEXP subset, SEXP block, SEXP nresample
) {
    static SEXP(*fun)(SEXP, SEXP, SEXP, SEXP, SEXP, SEXP) = NULL;
    if (fun == NULL)
        fun = (SEXP(*)(SEXP, SEXP, SEXP, SEXP, SEXP, SEXP))
            R_GetCCallable("libcoin", "R_PermutedLinearStatistic");
    return fun(x, y, weights, subset, block, nresample);
}
@}

The dimensions are extracted from the data in the same ways as above. The
function differentiates between the absense and presense of blocks.
Case weights are removed by expanding subset accordingly. Once within-block
permutations were set-up the Kronecker product of \code{X} and \code{Y} is
computed. Note that this function returns the matrix of permuted linear
statistics; the \proglang{R} interface assigns this matrix to the
corresponding element of the \code{LinStatExpCov} object (because we are not
allowed to modify existing \proglang{R} objects at \proglang{C} level).

@d R_PermutedLinearStatistic
@{
@<R_PermutedLinearStatistic Prototype@>
{
    SEXP ans, expand_subset, block_subset, perm, tmp, blockTable;
    double *linstat;
    int PQ;
    @<C integer N Input@>;
    @<C integer Nsubset Input@>;
    R_xlen_t inresample;

    @<Setup Dimensions@>
    PQ = mPQB(P, Q, 1);
    N = NROW(y);
    inresample = (R_xlen_t) REAL(nresample)[0];

    PROTECT(ans = allocMatrix(REALSXP, PQ, inresample));
    PROTECT(expand_subset = RC_setup_subset(N, weights, subset));
    Nsubset = XLENGTH(expand_subset);
    PROTECT(tmp = allocVector(REALSXP, Nsubset));
    PROTECT(perm = allocVector(REALSXP, Nsubset));

    GetRNGstate();
    if (B == 1) {
        for (R_xlen_t np = 0; np < inresample; np++) {
            @<Setup Linear Statistic@>
            C_doPermute(REAL(expand_subset), Nsubset, REAL(tmp), REAL(perm));
            RC_KronSums_Permutation(x, NROW(x), P, REAL(y), Q, expand_subset,
                                    Offset0, Nsubset, perm, linstat);
        }
    } else {
        PROTECT(blockTable = allocVector(REALSXP, B + 1));
        /* same as RC_OneTableSums(block, noweights, expand_subset) */
        RC_OneTableSums(INTEGER(block), XLENGTH(block), B + 1, weights, subset, Offset0,
                        XLENGTH(subset), REAL(blockTable));
        PROTECT(block_subset = RC_order_subset_wrt_block(XLENGTH(block), expand_subset,
                                                         block, blockTable));

        for (R_xlen_t np = 0; np < inresample; np++) {
            @<Setup Linear Statistic@>
            C_doPermuteBlock(REAL(block_subset), Nsubset, REAL(blockTable),
                             B + 1, REAL(tmp), REAL(perm));
            RC_KronSums_Permutation(x, NROW(x), P, REAL(y), Q, block_subset,
                                    Offset0, Nsubset, perm, linstat);
        }
        UNPROTECT(2);
    }
    PutRNGstate();

    UNPROTECT(4);
    return(ans);
}
@|R_PermutedLinearStatistic
@}

@d Setup Linear Statistic
@{
if (np % 256 == 0) R_CheckUserInterrupt();
linstat = REAL(ans) + PQ * np;
for (int p = 0; p < PQ; p++)
    linstat[p] = 0.0;
@}

This small function takes an object containing permuted linear statistics
and returns the matrix of standardised linear statistics.

@d R_StandardisePermutedLinearStatistic Prototype
@{
SEXP R_StandardisePermutedLinearStatistic
(
    SEXP LECV
)
@}

This function can be called from other packages.

@o libcoinAPI.h -cc
@{
extern SEXP libcoin_R_StandardisePermutedLinearStatistic(
    SEXP LECV
) {
    static SEXP(*fun)(SEXP) = NULL;
    if (fun == NULL)
        fun = (SEXP(*)(SEXP))
            R_GetCCallable("libcoin", "R_StandardisePermutedLinearStatistic");
    return fun(LECV);
}
@}

@d R_StandardisePermutedLinearStatistic
@{
@<R_StandardisePermutedLinearStatistic Prototype@>
{
    SEXP ans;
    R_xlen_t nresample = C_get_nresample(LECV);
    double *ls;
    if (!nresample) return(R_NilValue);
    int PQ = C_get_P(LECV) * C_get_Q(LECV);

    PROTECT(ans = allocMatrix(REALSXP, PQ, nresample));

    for (R_xlen_t np = 0; np < nresample; np++) {
        ls = REAL(ans) + PQ * np;
        /* copy first; standarisation is in place */
        for (int p = 0; p < PQ; p++)
            ls[p] = C_get_PermutedLinearStatistic(LECV)[p + PQ * np];
        if (C_get_varonly(LECV)) {
            C_standardise(PQ, ls, C_get_Expectation(LECV),
                          C_get_Variance(LECV), 1, C_get_tol(LECV));
        } else {
            C_standardise(PQ, ls, C_get_Expectation(LECV),
                          C_get_Covariance(LECV), 0, C_get_tol(LECV));
        }
    }
    UNPROTECT(1);
    return(ans);
}
@}

\subsection{Two-Dimensional Case (``2d'')}

@d 2d User Interface
@{
@<RC_ExpectationCovarianceStatistic_2d@>
@<R_ExpectationCovarianceStatistic_2d@>
@<R_PermutedLinearStatistic_2d@>
@}

@d 2d User Interface Input
@{
@<R x Input@>
SEXP ix,
@<R y Input@>
SEXP iy,
@<R weights Input@>,
@<R subset Input@>,
@<R block Input@>,
@}

@d R_ExpectationCovarianceStatistic_2d Prototype
@{
SEXP R_ExpectationCovarianceStatistic_2d
(
    @<2d User Interface Input@>
    SEXP varonly,
    SEXP tol
)
@}

This function can be called from other packages.

@o libcoinAPI.h -cc
@{
extern SEXP libcoin_R_ExpectationCovarianceStatistic_2d(
    SEXP x, SEXP ix, SEXP y, SEXP iy, SEXP weights, SEXP subset, SEXP block,
    SEXP varonly, SEXP tol
) {
    static SEXP(*fun)(SEXP, SEXP, SEXP, SEXP, SEXP, SEXP, SEXP, SEXP, SEXP) = NULL;
    if (fun == NULL)
        fun = (SEXP(*)(SEXP, SEXP, SEXP, SEXP, SEXP, SEXP, SEXP, SEXP, SEXP))
            R_GetCCallable("libcoin", "R_ExpectationCovarianceStatistic_2d");
    return fun(x, ix, y, iy, weights, subset, block, varonly, tol);
}
@}

@d R_ExpectationCovarianceStatistic_2d
@{
@<R_ExpectationCovarianceStatistic_2d Prototype@>
{
    SEXP ans;
    @<C integer N Input@>;
    @<C integer Nsubset Input@>;
    int Xfactor;

    N = XLENGTH(ix);
    Nsubset = XLENGTH(subset);
    Xfactor = XLENGTH(x) == 0;

    @<Setup Dimensions 2d@>

    PROTECT(ans = RC_init_LECV_2d(P, Q, INTEGER(varonly)[0],
                                  Lx, Ly, B, Xfactor, REAL(tol)[0]));

    if (B == 1) {
        RC_TwoTableSums(INTEGER(ix), N, Lx + 1, INTEGER(iy), Ly + 1,
                        weights, subset, Offset0, Nsubset,
                        C_get_Table(ans));
    } else {
        RC_ThreeTableSums(INTEGER(ix), N, Lx + 1, INTEGER(iy), Ly + 1,
                          INTEGER(block), B, weights, subset, Offset0, Nsubset,
                          C_get_Table(ans));
    }
    RC_ExpectationCovarianceStatistic_2d(x, ix, y, iy, weights,
                                         subset, block, ans);

    UNPROTECT(1);
    return(ans);
}
@|R_ExpectationCovarianceStatistic_2d
@}

@d Setup Dimensions 2d
@{
int P, Q, B, Lx, Ly;

if (XLENGTH(x) == 0) {
    P = NLEVELS(ix);
} else {
    P = NCOL(x);
}
Q = NCOL(y);

B = 1;
if (XLENGTH(block) > 0)
    B = NLEVELS(block);

Lx = NLEVELS(ix);
Ly = NLEVELS(iy);
@}

@d Linear Statistic 2d
@{
if (Xfactor) {
    for (int j = 1; j < Lyp1; j++) {     /* j = 0 means NA */
        for (int i = 1; i < Lxp1; i++) { /* i = 0 means NA */
            for (int q = 0; q < Q; q++)
                linstat[q * (Lxp1 - 1) + (i - 1)] +=
                    btab[j * Lxp1 + i] * REAL(y)[q * Lyp1 + j];
        }
    }
} else {
    for (int p = 0; p < P; p++) {
        for (int q = 0; q < Q; q++) {
            int qPp = q * P + p;
            int qLy = q * Lyp1;
            for (int i = 0; i < Lxp1; i++) {
                int pLxi = p * Lxp1 + i;
                for (int j = 0; j < Lyp1; j++)
                    linstat[qPp] += REAL(y)[qLy + j] * REAL(x)[pLxi] * btab[j * Lxp1 + i];
            }
        }
    }
}
@}

@d 2d Total Table
@{
for (int i = 0; i < Lxp1 * Lyp1; i++)
    table2d[i] = 0.0;
for (int b = 0; b < B; b++) {
    for (int i = 0; i < Lxp1; i++) {
        for (int j = 0; j < Lyp1; j++)
            table2d[j * Lxp1 + i] += table[b * Lxp1 * Lyp1 + j * Lxp1 + i];
    }
}
@}

@d Col Row Total Sums
@{
/* Remember: first row / column count NAs */
/* column sums */
for (int q = 1; q < Lyp1; q++) {
    csum[q] = 0;
    for (int p = 1; p < Lxp1; p++)
        csum[q] += btab[q * Lxp1 + p];
}
csum[0] = 0; /* NA */
/* row sums */
for (int p = 1; p < Lxp1; p++) {
    rsum[p] = 0;
    for (int q = 1; q < Lyp1; q++)
        rsum[p] += btab[q * Lxp1 + p];
}
rsum[0] = 0; /* NA */
/* total sum */
sumweights[b] = 0;
for (int i = 1; i < Lxp1; i++) sumweights[b] += rsum[i];
@}

@d 2d Expectation
@{
RC_ExpectationInfluence(NROW(y), y, Q, Rcsum, subset, Offset0, 0, sumweights[b], ExpInf);

if (LENGTH(x) == 0) {
    for (int p = 0; p < P; p++)
        ExpX[p] = rsum[p + 1];
    } else {
        RC_ExpectationX(x, NROW(x), P, Rrsum, subset, Offset0, 0, ExpX);
}

C_ExpectationLinearStatistic(P, Q, ExpInf, ExpX, b, C_get_Expectation(ans));
@}

@d 2d Covariance
@{
/* C_ordered_Xfactor needs both VarInf and CovInf */
RC_CovarianceInfluence(NROW(y), y, Q, Rcsum, subset, Offset0, 0, ExpInf, sumweights[b],
                       !DoVarOnly, C_get_CovarianceInfluence(ans));
for (int q = 0; q < Q; q++)
    C_get_VarianceInfluence(ans)[q] = C_get_CovarianceInfluence(ans)[S(q, q, Q)];

if (C_get_varonly(ans)) {
    if (LENGTH(x) == 0) {
        for (int p = 0; p < P; p++) CovX[p] = ExpX[p];
    } else {
        RC_CovarianceX(x, NROW(x), P, Rrsum, subset, Offset0, 0, ExpX, DoVarOnly, CovX);
    }
    C_VarianceLinearStatistic(P, Q, C_get_VarianceInfluence(ans),
                              ExpX, CovX, sumweights[b], b,
                              C_get_Variance(ans));
} else {
    if (LENGTH(x) == 0) {
        for (int p = 0; p < PP12(P); p++) CovX[p] = 0.0;
        for (int p = 0; p < P; p++) CovX[S(p, p, P)] = ExpX[p];
    } else {
        RC_CovarianceX(x, NROW(x), P, Rrsum, subset, Offset0, 0, ExpX, !DoVarOnly, CovX);
    }
    C_CovarianceLinearStatistic(P, Q, C_get_CovarianceInfluence(ans),
                                ExpX, CovX, sumweights[b], b,
                                C_get_Covariance(ans));
}
@}

@d RC_ExpectationCovarianceStatistic_2d
@{
void RC_ExpectationCovarianceStatistic_2d
(
    @<2d User Interface Input@>
    SEXP ans
) {
    @<2d Memory@>

    @<2d Total Table@>

    linstat = C_get_LinearStatistic(ans);
    for (int p = 0; p < mPQB(P, Q, 1); p++)
        linstat[p] = 0.0;

    for (int b = 0; b < B; b++) {
        btab = table + Lxp1 * Lyp1 * b;

        @<Linear Statistic 2d@>

        @<Col Row Total Sums@>

        @<2d Expectation@>

        @<2d Covariance@>
    }

    /* always return variances */
    if (!C_get_varonly(ans)) {
        for (int p = 0; p < mPQB(P, Q, 1); p++)
            C_get_Variance(ans)[p] = C_get_Covariance(ans)[S(p, p, mPQB(P, Q, 1))];
    }

    R_Free(CovX);
    R_Free(table2d);
    UNPROTECT(2);
}
@|RC_ExpectationCovarianceStatistic
@}

@d 2d Memory
@{
SEXP Rcsum, Rrsum;
int P, Q, Lxp1, Lyp1, B, Xfactor;
double *ExpInf, *ExpX, *CovX;
double *table, *table2d, *csum, *rsum, *sumweights, *btab, *linstat;

P = C_get_P(ans);
Q = C_get_Q(ans);

ExpInf = C_get_ExpectationInfluence(ans);
ExpX = C_get_ExpectationX(ans);
table = C_get_Table(ans);
sumweights = C_get_Sumweights(ans);

Lxp1 = C_get_dimTable(ans)[0];
Lyp1 = C_get_dimTable(ans)[1];
B = C_get_B(ans);
Xfactor = C_get_Xfactor(ans);

if (C_get_varonly(ans)) {
    CovX = R_Calloc(P, double);
} else {
    CovX = R_Calloc(PP12(P), double);
}

table2d = R_Calloc(Lxp1 * Lyp1, double);
PROTECT(Rcsum = allocVector(REALSXP, Lyp1));
csum = REAL(Rcsum);
PROTECT(Rrsum = allocVector(REALSXP, Lxp1));
rsum = REAL(Rrsum);
@}

<<permutations-2d>>=
LinStatExpCov(X = iX2d, ix = ix, Y = iY2d, iy = iy,
              weights = weights, subset = subset, nresample = 10)$PermutedLinearStatistic
@@

@d R_PermutedLinearStatistic_2d Prototype
@{
SEXP R_PermutedLinearStatistic_2d
(
    @<R x Input@>
    SEXP ix,
    @<R y Input@>
    SEXP iy,
    @<R block Input@>,
    SEXP nresample,
    SEXP itable
)
@}

This function can be called from other packages.

@o libcoinAPI.h -cc
@{
extern SEXP libcoin_R_PermutedLinearStatistic_2d(
    SEXP x, SEXP ix, SEXP y, SEXP iy, SEXP block, SEXP nresample,
    SEXP itable
) {
    static SEXP(*fun)(SEXP, SEXP, SEXP, SEXP, SEXP, SEXP, SEXP) = NULL;
    if (fun == NULL)
        fun = (SEXP(*)(SEXP, SEXP, SEXP, SEXP, SEXP, SEXP, SEXP))
            R_GetCCallable("libcoin", "R_PermutedLinearStatistic_2d");
    return fun(x, ix, y, iy, block, nresample, itable);
}
@}

@d R_PermutedLinearStatistic_2d
@{
@<R_PermutedLinearStatistic_2d Prototype@>
{
    SEXP ans, Ritable;
    int *csum, *rsum, *sumweights, *jwork, *table, *rtable2, maxn = 0, Lxp1, Lyp1, *btab, PQ, Xfactor;
    R_xlen_t inresample;
    double *fact, *linstat;

    @<Setup Dimensions 2d@>

    PQ = mPQB(P, Q, 1);
    Xfactor = XLENGTH(x) == 0;
    Lxp1 = Lx + 1;
    Lyp1 = Ly + 1;
    inresample = (R_xlen_t) REAL(nresample)[0];

    PROTECT(ans = allocMatrix(REALSXP, PQ, inresample));

    @<Setup Working Memory@>

    @<Convert Table to Integer@>

    for (int b = 0; b < B; b++) {
        btab = INTEGER(Ritable) + Lxp1 * Lyp1 * b;
        @<Col Row Total Sums@>
        if (sumweights[b] > maxn) maxn = sumweights[b];
    }

    @<Setup Log-Factorials@>

    GetRNGstate();

    for (R_xlen_t np = 0; np < inresample; np++) {
        @<Setup Linear Statistic@>

        for (int p = 0; p < Lxp1 * Lyp1; p++)
            table[p] = 0;

        for (int b = 0; b < B; b++) {
            @<Compute Permuted Linear Statistic 2d@>
        }
    }

    PutRNGstate();

    R_Free(csum); R_Free(rsum); R_Free(sumweights); R_Free(rtable2);
    R_Free(jwork); R_Free(fact); R_Free(table);
    UNPROTECT(2);
    return(ans);
}
@|R_PermutedLinearStatistic_2d
@}

@d Convert Table to Integer
@{
PROTECT(Ritable = allocVector(INTSXP, LENGTH(itable)));
for (int i = 0; i < LENGTH(itable); i++) {
    if (REAL(itable)[i] > INT_MAX)
        error("cannot deal with weights larger INT_MAX in R_PermutedLinearStatistic_2d");
    INTEGER(Ritable)[i] = (int) REAL(itable)[i];
}
@}

@d Setup Working Memory
@{
csum = R_Calloc(Lyp1 * B, int);
rsum = R_Calloc(Lxp1 * B, int);
sumweights = R_Calloc(B, int);
table = R_Calloc(Lxp1 * Lyp1, int);
rtable2 = R_Calloc(Lx * Ly , int);
jwork = R_Calloc(Lyp1, int);
@}

@d Setup Log-Factorials
@{
fact = R_Calloc(maxn + 1, double);
/* Calculate log-factorials.  fact[i] = lgamma(i+1) */
fact[0] = fact[1] = 0.;
for (int j = 2; j <= maxn; j++)
    fact[j] = fact[j - 1] + log(j);
@}

Note: the interface to \code{S_rcont2} changed in \textsf{R}-4.1.0.
@d Compute Permuted Linear Statistic 2d
@{
#if defined(R_VERSION) && R_VERSION >= R_Version(4, 1, 0)
            S_rcont2(Lx, Ly,
                     rsum + Lxp1 * b + 1,
                     csum + Lyp1 * b + 1,
                     sumweights[b], fact, jwork, rtable2);
#else
            S_rcont2(&Lx, &Ly,
                     rsum + Lxp1 * b + 1,
                     csum + Lyp1 * b + 1,
                     sumweights + b, fact, jwork, rtable2);
#endif

for (int j1 = 1; j1 <= Lx; j1++) {
    for (int j2 = 1; j2 <= Ly; j2++)
        table[j2 * Lxp1 + j1] = rtable2[(j2 - 1) * Lx + (j1 - 1)];
}
btab = table;
@<Linear Statistic 2d@>
@}

\section{Tests}

@d Tests
@{
@<R_QuadraticTest@>
@<R_MaximumTest@>
@<R_MaximallySelectedTest@>
@}

@d R_QuadraticTest Prototype
@{
SEXP R_QuadraticTest
(
    @<R LECV Input@>,
    SEXP pvalue,
    SEXP lower,
    SEXP give_log,
    SEXP PermutedStatistics
)
@}

This function can be called from other packages.

@o libcoinAPI.h -cc
@{
extern SEXP libcoin_R_QuadraticTest(
    SEXP LECV, SEXP pvalue, SEXP lower, SEXP give_log, SEXP PermutedStatistics
) {
    static SEXP(*fun)(SEXP, SEXP, SEXP, SEXP, SEXP) = NULL;
    if (fun == NULL)
        fun = (SEXP(*)(SEXP, SEXP, SEXP, SEXP, SEXP))
            R_GetCCallable("libcoin", "R_QuadraticTest");
    return fun(LECV, pvalue, lower, give_log, PermutedStatistics);
}
@}

@d R_QuadraticTest
@{
@<R_QuadraticTest Prototype@>
{
    SEXP ans, stat, pval, names, permstat;
    double *MPinv, *ls, st, pst, *ex;
    int rank, P, Q, PQ, greater = 0;
    R_xlen_t nresample;

    @<Setup Test Memory@>

    MPinv = R_Calloc(PP12(PQ), double); /* was: C_get_MPinv(LECV); */
    C_MPinv_sym(C_get_Covariance(LECV), PQ, C_get_tol(LECV), MPinv, &rank);

    REAL(stat)[0] = C_quadform(PQ, C_get_LinearStatistic(LECV),
                               C_get_Expectation(LECV), MPinv);

    if (!PVALUE) {
        UNPROTECT(2);
        R_Free(MPinv);
        return(ans);
    }

    if (C_get_nresample(LECV) == 0) {
        REAL(pval)[0] = C_chisq_pvalue(REAL(stat)[0], rank, LOWER, GIVELOG);
    } else {
        nresample = C_get_nresample(LECV);
        ls = C_get_PermutedLinearStatistic(LECV);
        st = REAL(stat)[0];
        ex = C_get_Expectation(LECV);
        greater = 0;
        for (R_xlen_t np = 0; np < nresample; np++) {
            pst = C_quadform(PQ, ls + PQ * np, ex, MPinv);
            if (GE(pst, st, C_get_tol(LECV)))
                greater++;
            if (PSTAT) REAL(permstat)[np] = pst;
        }
        REAL(pval)[0] = C_perm_pvalue(greater, nresample, LOWER, GIVELOG);
    }

    UNPROTECT(2);
    R_Free(MPinv);
    return(ans);
}
@}

@d Setup Test Memory
@{
P = C_get_P(LECV);
Q = C_get_Q(LECV);
PQ = mPQB(P, Q, 1);

if (C_get_varonly(LECV) && PQ > 1)
        error("cannot compute adjusted p-value based on variances only");
/*  if (C_get_nresample(LECV) > 0 && INTEGER(PermutedStatistics)[0]) { */
    PROTECT(ans = allocVector(VECSXP, 3));
    PROTECT(names = allocVector(STRSXP, 3));
    SET_VECTOR_ELT(ans, 2, permstat = allocVector(REALSXP, C_get_nresample(LECV)));
    SET_STRING_ELT(names, 2, mkChar("PermutedStatistics"));
/*  } else {
    PROTECT(ans = allocVector(VECSXP, 2));
    PROTECT(names = allocVector(STRSXP, 2));
}
*/
SET_VECTOR_ELT(ans, 0, stat = allocVector(REALSXP, 1));
SET_STRING_ELT(names, 0, mkChar("TestStatistic"));
SET_VECTOR_ELT(ans, 1, pval = allocVector(REALSXP, 1));
SET_STRING_ELT(names, 1, mkChar("p.value"));
namesgets(ans, names);
REAL(pval)[0] = NA_REAL;
int LOWER = INTEGER(lower)[0];
int GIVELOG = INTEGER(give_log)[0];
int PVALUE = INTEGER(pvalue)[0];
int PSTAT = INTEGER(PermutedStatistics)[0];
@}

@d R_MaximumTest Prototype
@{
SEXP R_MaximumTest
(
    @<R LECV Input@>,
    SEXP alternative,
    SEXP pvalue,
    SEXP lower,
    SEXP give_log,
    SEXP PermutedStatistics,
    SEXP maxpts,
    SEXP releps,
    SEXP abseps
)
@}

This function can be called from other packages.

@o libcoinAPI.h -cc
@{
extern SEXP libcoin_R_MaximumTest(
    SEXP LECV, SEXP alternative, SEXP pvalue, SEXP lower, SEXP give_log,
    SEXP PermutedStatistics, SEXP maxpts, SEXP releps, SEXP abseps
) {
  static SEXP(*fun)(SEXP, SEXP, SEXP, SEXP, SEXP, SEXP, SEXP, SEXP, SEXP) = NULL;
    if (fun == NULL)
        fun = (SEXP(*)(SEXP, SEXP, SEXP, SEXP, SEXP, SEXP, SEXP, SEXP, SEXP))
            R_GetCCallable("libcoin", "R_MaximumTest");
    return fun(LECV, alternative, pvalue, lower, give_log, PermutedStatistics, maxpts, releps,
               abseps);
}
@}

@d R_MaximumTest
@{
@<R_MaximumTest Prototype@>
{
    SEXP ans, stat, pval, names, permstat;
    double st, pst, *ex, *cv, *ls, tl;
    int P, Q, PQ, vo, alt, greater;
    R_xlen_t nresample;

    @<Setup Test Memory@>

    if (C_get_varonly(LECV)) {
        cv = C_get_Variance(LECV);
    } else {
        cv = C_get_Covariance(LECV);
    }
    REAL(stat)[0] = C_maxtype(PQ, C_get_LinearStatistic(LECV),
        C_get_Expectation(LECV), cv, C_get_varonly(LECV), C_get_tol(LECV),
        INTEGER(alternative)[0]);
    if (!PVALUE) {
        UNPROTECT(2);
        return(ans);
    }

    if (C_get_nresample(LECV) == 0) {
        if (C_get_varonly(LECV) && PQ > 1) {
            REAL(pval)[0] = NA_REAL;
            UNPROTECT(2);
            return(ans);
        }
        REAL(pval)[0] = C_maxtype_pvalue(REAL(stat)[0], cv,
            PQ, INTEGER(alternative)[0], LOWER, GIVELOG, INTEGER(maxpts)[0],
            REAL(releps)[0], REAL(abseps)[0], C_get_tol(LECV));
    } else {
        nresample = C_get_nresample(LECV);
        ls = C_get_PermutedLinearStatistic(LECV);
        ex = C_get_Expectation(LECV);
        vo = C_get_varonly(LECV);
        alt = INTEGER(alternative)[0];
        st = REAL(stat)[0];
        tl = C_get_tol(LECV);
        greater = 0;
        for (R_xlen_t np = 0; np < nresample; np++) {
            pst = C_maxtype(PQ, ls + PQ * np, ex, cv, vo, tl, alt);
            if (alt == ALTERNATIVE_less) {
                if (LE(pst, st, tl)) greater++;
            } else {
                if (GE(pst, st, tl)) greater++;
            }
            if (PSTAT) REAL(permstat)[np] = pst;
        }
        REAL(pval)[0] = C_perm_pvalue(greater, nresample, LOWER, GIVELOG);
    }
    UNPROTECT(2);
    return(ans);
}
@}

@d R_MaximallySelectedTest Prototype
@{
SEXP R_MaximallySelectedTest
(
    SEXP LECV,
    SEXP ordered,
    SEXP teststat,
    SEXP minbucket,
    SEXP lower,
    SEXP give_log
)
@}

This function can be called from other packages.

@o libcoinAPI.h -cc
@{
extern SEXP libcoin_R_MaximallySelectedTest(
    SEXP LECV, SEXP ordered, SEXP teststat, SEXP minbucket, SEXP lower, SEXP give_log
) {
    static SEXP(*fun)(SEXP, SEXP, SEXP, SEXP, SEXP, SEXP) = NULL;
    if (fun == NULL)
        fun = (SEXP(*)(SEXP, SEXP, SEXP, SEXP, SEXP, SEXP))
            R_GetCCallable("libcoin", "R_MaximallySelectedTest");
    return fun(LECV, ordered, teststat, minbucket, lower, give_log);
}
@}

@d R_MaximallySelectedTest
@{
@<R_MaximallySelectedTest Prototype@>
{
    SEXP ans, index, stat, pval, names, permstat;
    int P, mb;

    P = C_get_P(LECV);
    mb = INTEGER(minbucket)[0];

    PROTECT(ans = allocVector(VECSXP, 4));
    PROTECT(names = allocVector(STRSXP, 4));
    SET_VECTOR_ELT(ans, 0, stat = allocVector(REALSXP, 1));
    SET_STRING_ELT(names, 0, mkChar("TestStatistic"));
    SET_VECTOR_ELT(ans, 1, pval = allocVector(REALSXP, 1));
    SET_STRING_ELT(names, 1, mkChar("p.value"));
    SET_VECTOR_ELT(ans, 3, permstat = allocVector(REALSXP, C_get_nresample(LECV)));
    SET_STRING_ELT(names, 3, mkChar("PermutedStatistics"));
    REAL(pval)[0] = NA_REAL;

    if (INTEGER(ordered)[0]) {
        SET_VECTOR_ELT(ans, 2, index = allocVector(INTSXP, 1));
        C_ordered_Xfactor(LECV, mb, INTEGER(teststat)[0],
                          INTEGER(index), REAL(stat), REAL(permstat),
                          REAL(pval), INTEGER(lower)[0],
                          INTEGER(give_log)[0]);
        if (REAL(stat)[0] > 0)
            INTEGER(index)[0]++; /* R style indexing */
    } else {
        SET_VECTOR_ELT(ans, 2, index = allocVector(INTSXP, P));
        C_unordered_Xfactor(LECV, mb, INTEGER(teststat)[0],
                            INTEGER(index), REAL(stat), REAL(permstat),
                            REAL(pval), INTEGER(lower)[0],
                            INTEGER(give_log)[0]);
    }

    SET_STRING_ELT(names, 2, mkChar("index"));
    namesgets(ans, names);

    UNPROTECT(2);
    return(ans);
}
@}

\section{Test Statistics}

@d Test Statistics
@{
@<C_maxstand_Covariance@>
@<C_maxstand_Variance@>
@<C_minstand_Covariance@>
@<C_minstand_Variance@>
@<C_maxabsstand_Covariance@>
@<C_maxabsstand_Variance@>
@<C_quadform@>
@<R_quadform@>
@<C_maxtype@>
@<C_standardise@>
@<C_ordered_Xfactor@>
@<C_unordered_Xfactor@>
@}

@d C_maxstand_Covariance
@{
double C_maxstand_Covariance
(
    const int PQ,
    const double *linstat,
    const double *expect,
    const double *covar_sym,
    const double tol
) {
    double ans = R_NegInf, tmp = 0.0;

    for (int p = 0; p < PQ; p++) {
        tmp = 0.0;
        if (covar_sym[S(p, p, PQ)] > tol)
            tmp = (linstat[p] - expect[p]) / sqrt(covar_sym[S(p, p, PQ)]);
        if (tmp > ans) ans = tmp;
    }
    return(ans);
}
@|C_maxstand_Covariance
@}

@d C_maxstand_Variance
@{
double C_maxstand_Variance
(
    const int PQ,
    const double *linstat,
    const double *expect,
    const double *var,
    const double tol
) {
    double ans = R_NegInf, tmp = 0.0;

    for (int p = 0; p < PQ; p++) {
        tmp = 0.0;
        if (var[p] > tol)
            tmp = (linstat[p] - expect[p]) / sqrt(var[p]);
        if (tmp > ans) ans = tmp;
    }
    return(ans);
}
@|C_maxstand_Variance
@}

@d C_minstand_Covariance
@{
double C_minstand_Covariance
(
    const int PQ,
    const double *linstat,
    const double *expect,
    const double *covar_sym,
    const double tol
) {
    double ans = R_PosInf, tmp = 0.0;

    for (int p = 0; p < PQ; p++) {
        tmp = 0.0;
        if (covar_sym[S(p, p, PQ)] > tol)
            tmp = (linstat[p] - expect[p]) / sqrt(covar_sym[S(p, p, PQ)]);
        if (tmp < ans) ans = tmp;
    }
    return(ans);
}
@|C_minstand_Covariance
@}

@d C_minstand_Variance
@{
double C_minstand_Variance
(
    const int PQ,
    const double *linstat,
    const double *expect,
    const double *var,
    const double tol
) {
    double ans = R_PosInf, tmp = 0.0;

    for (int p = 0; p < PQ; p++) {
        tmp = 0.0;
        if (var[p] > tol)
            tmp = (linstat[p] - expect[p]) / sqrt(var[p]);
        if (tmp < ans) ans = tmp;
    }
    return(ans);
}
@|C_minstand_Variance
@}

@d C_maxabsstand_Covariance
@{
double C_maxabsstand_Covariance
(
    const int PQ,
    const double *linstat,
    const double *expect,
    const double *covar_sym,
    const double tol
) {
    double ans = R_NegInf, tmp = 0.0;

    for (int p = 0; p < PQ; p++) {
        tmp = 0.0;
        if (covar_sym[S(p, p, PQ)] > tol)
            tmp = fabs((linstat[p] - expect[p]) /
                  sqrt(covar_sym[S(p, p, PQ)]));
        if (tmp > ans) ans = tmp;
    }
    return(ans);
}
@|C_maxabsstand_Covariance
@}

@d C_maxabsstand_Variance
@{
double C_maxabsstand_Variance
(
    const int PQ,
    const double *linstat,
    const double *expect,
    const double *var,
    const double tol
) {
    double ans = R_NegInf, tmp = 0.0;

    for (int p = 0; p < PQ; p++) {
        tmp = 0.0;
        if (var[p] > tol)
            tmp = fabs((linstat[p] - expect[p]) / sqrt(var[p]));
        if (tmp > ans) ans = tmp;
    }
    return(ans);
}
@|C_maxabsstand_Variance
@}

<<quadform>>=
MPinverse <-
function(x, tol = sqrt(.Machine$double.eps))
{
    SVD <- svd(x)
    pos <- SVD$d > max(tol * SVD$d[1L], 0)
    inv <- SVD$v[, pos, drop = FALSE] %*%
             ((1/SVD$d[pos]) * t(SVD$u[, pos, drop = FALSE]))
    list(MPinv = inv, rank = sum(pos))
}

quadform <-
function(linstat, expect, MPinv)
{
    censtat <- linstat - expect
    censtat %*% MPinv %*% censtat
}

linstat <- ls1$LinearStatistic
expect <- ls1$Expectation
MPinv <- MPinverse(vcov(ls1))$MPinv
MPinv_sym <- MPinv[lower.tri(MPinv, diag = TRUE)]
qf1 <- quadform(linstat, expect, MPinv)
qf2 <- .Call(libcoin:::R_quadform, linstat, expect, MPinv_sym)

stopifnot(isequal(qf1, qf2))
@@

@d R_quadform Prototype
@{
SEXP R_quadform
(
    SEXP linstat,
    SEXP expect,
    SEXP MPinv_sym
)
@}

This function can be called from other packages.

@o libcoinAPI.h -cc
@{
extern SEXP libcoin_R_quadform(
    SEXP linstat, SEXP expect, SEXP MPinv_sym
) {
    static SEXP(*fun)(SEXP, SEXP, SEXP) = NULL;
    if (fun == NULL)
        fun = (SEXP(*)(SEXP, SEXP, SEXP))
            R_GetCCallable("libcoin", "R_quadform");
    return fun(linstat, expect, MPinv_sym);
}
@}

@d R_quadform
@{
@<R_quadform Prototype@>
{
    SEXP ans;
    int n, PQ;
    double *dlinstat, *dexpect, *dMPinv_sym, *dans;

    n = NCOL(linstat);
    PQ = NROW(linstat);
    dlinstat = REAL(linstat);
    dexpect = REAL(expect);
    dMPinv_sym = REAL(MPinv_sym);

    PROTECT(ans = allocVector(REALSXP, n));
    dans = REAL(ans);
    for (int i = 0; i < n; i++)
      dans[i] = C_quadform(PQ, dlinstat + PQ * i, dexpect, dMPinv_sym);

    UNPROTECT(1);
    return(ans);
}
@|R_quadform
@}

@d C_quadform
@{
double C_quadform
(
    const int PQ,
    const double *linstat,
    const double *expect,
    const double *MPinv_sym
) {
    double ans = 0.0, tmp = 0.0;

    for (int q = 0; q < PQ; q++) {
        tmp = 0.0;
        for (int p = 0; p < PQ; p++)
            tmp += (linstat[p] - expect[p]) * MPinv_sym[S(p, q, PQ)];
        ans += tmp * (linstat[q] - expect[q]);
    }

    return(ans);
}
@|C_quadform
@}

@d C_maxtype
@{
double C_maxtype
(
    const int PQ,
    const double *linstat,
    const double *expect,
    const double *covar,
    const int varonly,
    const double tol,
    const int alternative
) {
    double ret = 0.0;

    if (varonly) {
        if (alternative == ALTERNATIVE_twosided) {
            ret = C_maxabsstand_Variance(PQ, linstat, expect, covar, tol);
        } else if (alternative == ALTERNATIVE_less) {
            ret = C_minstand_Variance(PQ, linstat, expect, covar, tol);
        } else if (alternative == ALTERNATIVE_greater) {
            ret = C_maxstand_Variance(PQ, linstat, expect, covar, tol);
        }
    } else {
        if (alternative == ALTERNATIVE_twosided) {
            ret = C_maxabsstand_Covariance(PQ, linstat, expect, covar, tol);
        } else if (alternative == ALTERNATIVE_less) {
            ret = C_minstand_Covariance(PQ, linstat, expect, covar, tol);
        } else if (alternative == ALTERNATIVE_greater) {
            ret = C_maxstand_Covariance(PQ, linstat, expect, covar, tol);
        }
    }
    return(ret);
}
@|C_maxtype
@}

@d C_standardise
@{
void C_standardise
(
    const int PQ,
    double *linstat,      /* in place standardisation */
    const double *expect,
    const double *covar,
    const int varonly,
    const double tol
) {
    double var;

    for (int p = 0; p < PQ; p++) {
        if (varonly) {
            var = covar[p];
        } else {
            var = covar[S(p, p, PQ)];
        }
        if (var > tol) {
            linstat[p] = (linstat[p] - expect[p]) / sqrt(var);
        } else {
            linstat[p] = NAN;
        }
    }
}
@|C_standardise
@}

@d P-Values
@{
@<C_chisq_pvalue@>
@<C_perm_pvalue@>
@<C_norm_pvalue@>
@<C_maxtype_pvalue@>
@}

@d C_chisq_pvalue
@{
/* lower = 1 means p-value, lower = 0 means 1 - p-value */
double C_chisq_pvalue
(
    const double stat,
    const int df,
    const int lower,
    const int give_log
) {
    return(pchisq(stat, (double) df, lower, give_log));
}
@|C_chisq_pvalue
@}

@d C_perm_pvalue
@{
double C_perm_pvalue
(
    const int greater,
    const double nresample,
    const int lower,
    const int give_log
) {
    double ret;

    if (give_log) {
         if (lower) {
             ret = log1p(- (double) greater / nresample);
         } else {
             ret = log(greater) - log(nresample);
         }
    } else {
        if (lower) {
            ret = 1.0 - (double) greater / nresample;
        } else {
            ret = (double) greater / nresample;
        }
    }
    return(ret);
}
@|C_perm_pvalue
@}

@d C_norm_pvalue
@{
double C_norm_pvalue
(
    const double stat,
    const int alternative,
    const int lower,
    const int give_log
) {
    double ret;

    if (alternative == ALTERNATIVE_less) {
        return(pnorm(stat, 0.0, 1.0, 1 - lower, give_log));
    } else if (alternative == ALTERNATIVE_greater) {
        return(pnorm(stat, 0.0, 1.0, lower, give_log));
    } else if (alternative == ALTERNATIVE_twosided) {
        if (lower) {
            ret = pnorm(fabs(stat)*-1.0, 0.0, 1.0, 1, 0);
            if (give_log) {
                return(log1p(- 2 * ret));
            } else {
                return(1 - 2 * ret);
            }
        } else {
            ret = pnorm(fabs(stat)*-1.0, 0.0, 1.0, 1, give_log);
            if (give_log) {
                return(ret + log(2));
            } else {
                return(2 * ret);
            }
        }
    }
    return(NA_REAL);
}
@}

@d C_maxtype_pvalue
@{
double C_maxtype_pvalue
(
    const double stat,
    const double *Covariance,
    const int n,
    const int alternative,
    const int lower,
    const int give_log,
    int maxpts, /* const? */
    double releps,
    double abseps,
    double tol
) {
    int nu = 0, inform, i, j, sub, nonzero, *infin, *index, rnd = 0;
    double ans, myerror, *lowerbnd, *upperbnd, *delta, *corr, *sd;

    /* univariate problem */
    if (n == 1)
        return(C_norm_pvalue(stat, alternative, lower, give_log));

    @<Setup mvtnorm Memory@>

    @<Setup mvtnorm Correlation@>

    /* call mvtnorm's mvtdst C function defined in mvtnorm/include/mvtnormAPI.h */
    mvtnorm_C_mvtdst(&nonzero, &nu, lowerbnd, upperbnd, infin, corr, delta,
                     &maxpts, &abseps, &releps, &myerror, &ans, &inform, &rnd);

    /* inform == 0 means: everything is OK */
    switch (inform) {
        case 0: break;
        case 1: warning("cmvnorm: completion with ERROR > EPS"); break;
        case 2: warning("cmvnorm: N > 1000 or N < 1");
                ans = 0.0;
                break;
        case 3: warning("cmvnorm: correlation matrix not positive semi-definite");
                ans = 0.0;
                break;
        default: warning("cmvnorm: unknown problem in MVTDST");
                ans = 0.0;
    }
    R_Free(corr); R_Free(sd); R_Free(lowerbnd); R_Free(upperbnd);
    R_Free(infin); R_Free(delta); R_Free(index);

    /* ans = 1 - p-value */
    if (lower) {
        if (give_log)
            return(log(ans));   /* log(1 - p-value) */
        return(ans);            /* 1 - p-value */
    } else {
        if (give_log)
            return(log1p(ans)); /* log(p-value) */
        return(1 - ans);        /* p-value */
    }
}
@|C_maxtype_pvalue
@}

@d Setup mvtnorm Memory
@{
if (n == 2)
    corr = R_Calloc(1, double);
else
    corr = R_Calloc(n + ((n - 2) * (n - 1))/2, double);

sd = R_Calloc(n, double);
lowerbnd = R_Calloc(n, double);
upperbnd = R_Calloc(n, double);
infin = R_Calloc(n, int);
delta = R_Calloc(n, double);
index = R_Calloc(n, int);

/* determine elements with non-zero variance */

nonzero = 0;
for (i = 0; i < n; i++) {
    if (Covariance[S(i, i, n)] > tol) {
        index[nonzero] = i;
        nonzero++;
    }
}
@}

\code{mvtdst} assumes the unique elements of the triangular
covariance matrix to be passed as argument \code{CORREL}

@d Setup mvtnorm Correlation
@{
for (int nz = 0; nz < nonzero; nz++) {
    /* handle elements with non-zero variance only */
    i = index[nz];

    /* standard deviations */
    sd[i] = sqrt(Covariance[S(i, i, n)]);

    if (alternative == ALTERNATIVE_less) {
        lowerbnd[nz] = stat;
        upperbnd[nz] = R_PosInf;
        infin[nz] = 1;
    } else if (alternative == ALTERNATIVE_greater) {
        lowerbnd[nz] = R_NegInf;
        upperbnd[nz] = stat;
        infin[nz] = 0;
    } else if (alternative == ALTERNATIVE_twosided) {
        lowerbnd[nz] = fabs(stat) * -1.0;
        upperbnd[nz] = fabs(stat);
        infin[nz] = 2;
    }

    delta[nz] = 0.0;

    /* set up vector of correlations, i.e., the upper
       triangular part of the covariance matrix) */
    for (int jz = 0; jz < nz; jz++) {
        j = index[jz];
        sub = (int) (jz + 1) + (double) ((nz - 1) * nz) / 2 - 1;
        if (sd[i] == 0.0 || sd[j] == 0.0)
            corr[sub] = 0.0;
        else
            corr[sub] = Covariance[S(i, j, n)] / (sd[i] * sd[j]);
    }
}
@}

@d maxstat Xfactor Variables
@{
SEXP LECV,
const int minbucket,
const int teststat,
int *wmax,
double *maxstat,
double *bmaxstat,
double *pval,
const int lower,
const int give_log
@}

@d C_ordered_Xfactor
@{
void C_ordered_Xfactor
(
    @<maxstat Xfactor Variables@>
) {
    @<Setup maxstat Variables@>

    @<Setup maxstat Memory@>

    wmax[0] = NA_INTEGER;

    for (int p = 0; p < P; p++) {
        sumleft += ExpX[p];
        sumright -= ExpX[p];

        for (int q = 0; q < Q; q++) {
            mlinstat[q] += linstat[q * P + p];
            for (R_xlen_t np = 0; np < nresample; np++)
                mblinstat[q + np * Q] += blinstat[q * P + p + np * PQ];
            mexpect[q] += expect[q * P + p];
            if (B == 1) {
                @<Compute maxstat Variance / Covariance Directly@>
            } else {
                @<Compute maxstat Variance / Covariance from Total Covariance@>
            }
        }

        if ((sumleft >= minbucket) && (sumright >= minbucket) && (ExpX[p] > 0)) {
            ls = mlinstat;
            /* compute MPinv only once */
            if (teststat != TESTSTAT_maximum)
                C_MPinv_sym(mcovar, Q, tol, mMPinv, &rank);
            @<Compute maxstat Test Statistic@>
            if (tmp > maxstat[0]) {
                wmax[0] = p;
                maxstat[0] = tmp;
            }

            for (R_xlen_t np = 0; np < nresample; np++) {
                ls = mblinstat + np * Q;
                @<Compute maxstat Test Statistic@>
                if (tmp > bmaxstat[np])
                    bmaxstat[np] = tmp;
            }
        }
    }
    @<Compute maxstat Permutation P-Value@>
    R_Free(mlinstat); R_Free(mexpect); R_Free(mblinstat);
    R_Free(mvar); R_Free(mcovar); R_Free(mMPinv);
    if (nresample == 0) R_Free(blinstat);
}
@|C_ordered_Xfactor
@}

@d Setup maxstat Variables
@{
double *linstat, *expect, *covar, *varinf, *covinf, *ExpX, *blinstat, tol, *ls;
int P, Q, B;
R_xlen_t nresample;

double *mlinstat, *mblinstat, *mexpect, *mvar, *mcovar, *mMPinv,
       tmp, sumleft, sumright, sumweights;
int rank, PQ, greater;

Q = C_get_Q(LECV);
P = C_get_P(LECV);
PQ = mPQB(P, Q, 1);
B = C_get_B(LECV);
if (B > 1) {
    if (C_get_varonly(LECV))
        error("need covariance for maximally statistics with blocks");
    covar = C_get_Covariance(LECV);
} else {
    covar = C_get_Variance(LECV); /* make -Wall happy */
}
linstat = C_get_LinearStatistic(LECV);
expect = C_get_Expectation(LECV);
ExpX = C_get_ExpectationX(LECV);
/* both need to be there */
varinf = C_get_VarianceInfluence(LECV);
covinf = C_get_CovarianceInfluence(LECV);
nresample = C_get_nresample(LECV);
if (nresample > 0)
    blinstat = C_get_PermutedLinearStatistic(LECV);
tol = C_get_tol(LECV);
@}

@d Setup maxstat Memory
@{
mlinstat = R_Calloc(Q, double);
mexpect = R_Calloc(Q, double);
if (teststat == TESTSTAT_maximum) {
    mvar = R_Calloc(Q, double);
    /* not needed, but allocate anyway to make -Wmaybe-uninitialized happy */
    mcovar = R_Calloc(1, double);
    mMPinv = R_Calloc(1, double);
} else {
    mcovar = R_Calloc(Q * (Q + 1) / 2, double);
    mMPinv = R_Calloc(Q * (Q + 1) / 2, double);
    /* not needed, but allocate anyway to make -Wmaybe-uninitialized happy */
    mvar = R_Calloc(1, double);
}
if (nresample > 0) {
    mblinstat = R_Calloc(Q * nresample, double);
} else { /* not needed, but allocate anyway to make -Wmaybe-uninitialized happy */
    mblinstat = R_Calloc(1, double);
    blinstat = R_Calloc(1, double);
}

maxstat[0] = 0.0;

for (int q = 0; q < Q; q++) {
    mlinstat[q] = 0.0;
    mexpect[q] = 0.0;
    if (teststat == TESTSTAT_maximum)
        mvar[q] = 0.0;
    for (R_xlen_t np = 0; np < nresample; np++) {
        mblinstat[q + np * Q] = 0.0;
        bmaxstat[np] = 0.0;
    }
}
if (teststat == TESTSTAT_quadratic) {
    for (int q = 0; q < Q * (Q + 1) / 2; q++)
        mcovar[q] = 0.0;
}

sumleft = 0.0;
sumright = 0.0;
for (int p = 0; p < P; p++)
    sumright += ExpX[p];
sumweights = sumright;
@}

@d Compute maxstat Variance / Covariance from Total Covariance
@{
if (teststat == TESTSTAT_maximum) {
    for (int pp = 0; pp < p; pp++)
        mvar[q] += 2 * covar[S(pp + q * P, p + P * q, mPQB(P, Q, 1))];
    mvar[q] += covar[S(p + q * P, p + P * q, mPQB(P, Q, 1))];
} else {
    for (int qq = 0; qq <= q; qq++) {
        for (int pp = 0; pp < p; pp++)
            mcovar[S(q, qq, Q)] += 2 * covar[S(pp + q * P, p + P * qq, mPQB(P, Q, 1))];
        mcovar[S(q, qq, Q)] += covar[S(p + q * P, p + P * qq, mPQB(P, Q, 1))];
    }
}
@}

@d Compute maxstat Variance / Covariance Directly
@{
/* does not work with blocks! */
if (teststat == TESTSTAT_maximum) {
    C_VarianceLinearStatistic(1, Q, varinf, &sumleft, &sumleft,
                              sumweights, 0, mvar);
} else {
    C_CovarianceLinearStatistic(1, Q, covinf, &sumleft, &sumleft,
                                sumweights, 0, mcovar);
}
@}

@d Compute maxstat Test Statistic
@{
if (teststat == TESTSTAT_maximum) {
    tmp = C_maxtype(Q, ls, mexpect, mvar, 1, tol,
                    ALTERNATIVE_twosided);
} else {
    tmp = C_quadform(Q, ls, mexpect, mMPinv);
}
@}

@d Compute maxstat Permutation P-Value
@{
if (nresample > 0) {
    greater = 0;
    for (R_xlen_t np = 0; np < nresample; np++) {
        if (bmaxstat[np] > maxstat[0]) greater++;
    }
    pval[0] = C_perm_pvalue(greater, nresample, lower, give_log);
}
@}

@d C_unordered_Xfactor
@{
void C_unordered_Xfactor
(
    @<maxstat Xfactor Variables@>
) {
    double *mtmp;
    int qPp, nc, *levels, Pnonzero, *indl, *contrast;

    @<Setup maxstat Variables@>

    @<Setup maxstat Memory@>
    mtmp = R_Calloc(P, double);

    for (int p = 0; p < P; p++) wmax[p] = NA_INTEGER;

    @<Count Levels@>

    for (int j = 1; j < mi; j++) { /* go though all splits */

        @<Setup unordered maxstat Contrasts@>

        @<Compute unordered maxstat Linear Statistic and Expectation@>

        if (B == 1) {
            @<Compute unordered maxstat Variance / Covariance Directly@>
        } else {
            @<Compute unordered maxstat Variance / Covariance from Total Covariance@>
        }

        if ((sumleft >= minbucket) && (sumright >= minbucket)) {
            ls = mlinstat;
            /* compute MPinv only once */
            if (teststat != TESTSTAT_maximum)
                C_MPinv_sym(mcovar, Q, tol, mMPinv, &rank);
            @<Compute maxstat Test Statistic@>
            if (tmp > maxstat[0]) {
                for (int p = 0; p < Pnonzero; p++)
                    wmax[levels[p]] = contrast[levels[p]];
                maxstat[0] = tmp;
            }

            for (R_xlen_t np = 0; np < nresample; np++) {
                ls = mblinstat + np * Q;
                @<Compute maxstat Test Statistic@>
                if (tmp > bmaxstat[np])
                    bmaxstat[np] = tmp;
            }
        }
    }

    @<Compute maxstat Permutation P-Value@>

    R_Free(mlinstat); R_Free(mexpect); R_Free(levels); R_Free(contrast); R_Free(indl); R_Free(mtmp);
    R_Free(mblinstat); R_Free(mvar); R_Free(mcovar); R_Free(mMPinv);
    if (nresample == 0) R_Free(blinstat);
}
@|C_unordered_Xfactor
@}

@d Count Levels
@{
contrast = R_Calloc(P, int);
Pnonzero = 0;
for (int p = 0; p < P; p++) {
    if (ExpX[p] > 0) Pnonzero++;
}
levels = R_Calloc(Pnonzero, int);
nc = 0;
for (int p = 0; p < P; p++) {
    if (ExpX[p] > 0) {
        levels[nc] = p;
        nc++;
    }
}

if (Pnonzero >= 31)
    error("cannot search for unordered splits in >= 31 levels");

int mi = 1;
for (int l = 1; l < Pnonzero; l++) mi *= 2;
indl = R_Calloc(Pnonzero, int);
for (int p = 0; p < Pnonzero; p++) indl[p] = 0;
@}

@d Setup unordered maxstat Contrasts
@{
/* indl determines if level p is left or right */
int jj = j;
for (int l = 1; l < Pnonzero; l++) {
    indl[l] = (jj%2);
    jj /= 2;
}

sumleft = 0.0;
sumright = 0.0;
for (int p = 0; p < P; p++) contrast[p] = 0;
for (int p = 0; p < Pnonzero; p++) {
    sumleft += indl[p] * ExpX[levels[p]];
    sumright += (1 - indl[p]) * ExpX[levels[p]];
    contrast[levels[p]] = indl[p];
}
@}

@d Compute unordered maxstat Linear Statistic and Expectation
@{
for (int q = 0; q < Q; q++) {
    mlinstat[q] = 0.0;
    mexpect[q] = 0.0;
    for (R_xlen_t np = 0; np < nresample; np++)
        mblinstat[q + np * Q] = 0.0;
    for (int p = 0; p < P; p++) {
        qPp = q * P + p;
        mlinstat[q] += contrast[p] * linstat[qPp];
        mexpect[q] += contrast[p] * expect[qPp];
        for (R_xlen_t np = 0; np < nresample; np++)
            mblinstat[q + np * Q] += contrast[p] * blinstat[q * P + p + np * PQ];
    }
}
@}

@d Compute unordered maxstat Variance / Covariance from Total Covariance
@{
if (teststat == TESTSTAT_maximum) {
    for (int q = 0; q < Q; q++) {
        mvar[q] = 0.0;
        for (int p = 0; p < P; p++) {
            qPp = q * P + p;
            mtmp[p] = 0.0;
            for (int pp = 0; pp < P; pp++)
                mtmp[p] += contrast[pp] * covar[S(pp + q * P, qPp, PQ)];
	}
        for (int p = 0; p < P; p++)
            mvar[q] += contrast[p] * mtmp[p];
    }
} else {
    for (int q = 0; q < Q; q++) {
        for (int qq = 0; qq <= q; qq++)
            mcovar[S(q, qq, Q)] = 0.0;
        for (int qq = 0; qq <= q; qq++) {
            for (int p = 0; p < P; p++) {
                mtmp[p] = 0.0;
                for (int pp = 0; pp < P; pp++)
                    mtmp[p] += contrast[pp] * covar[S(pp + q * P, p + P * qq,
                                                      mPQB(P, Q, 1))];
            }
            for (int p = 0; p < P; p++)
                mcovar[S(q, qq, Q)] += contrast[p] * mtmp[p];
        }
    }
}
@}

@d Compute unordered maxstat Variance / Covariance Directly
@{
if (teststat == TESTSTAT_maximum) {
    C_VarianceLinearStatistic(1, Q, varinf, &sumleft, &sumleft,
                              sumweights, 0, mvar);
} else {
    C_CovarianceLinearStatistic(1, Q, covinf, &sumleft, &sumleft,
                                sumweights, 0, mcovar);
}
@}

\section{Linear Statistics}

@d LinearStatistics
@{
@<RC_LinearStatistic@>
@}

@d RC_LinearStatistic Prototype
@{
void RC_LinearStatistic
(
    @<R x Input@>
    @<C integer N Input@>,
    @<C integer P Input@>,
    @<C real y Input@>
    @<R weights Input@>,
    @<R subset Input@>,
    @<C subset range Input@>,
    @<C KronSums Answer@>
)
@}

@d RC_LinearStatistic
@{
@<RC_LinearStatistic Prototype@>
{
    double center;

    RC_KronSums(x, N, P, y, Q, !DoSymmetric, &center, &center, !DoCenter, weights,
                subset, offset, Nsubset, PQ_ans);
}
@|RC_LinearStatistic
@}

\section{Expectation and Covariance}

@d ExpectationCovariances
@{
@<RC_ExpectationInfluence@>
@<R_ExpectationInfluence@>
@<RC_CovarianceInfluence@>
@<R_CovarianceInfluence@>
@<RC_ExpectationX@>
@<R_ExpectationX@>
@<RC_CovarianceX@>
@<R_CovarianceX@>
@<C_ExpectationLinearStatistic@>
@<C_CovarianceLinearStatistic@>
@<C_VarianceLinearStatistic@>
@}

\subsection{Linear Statistic}

@d C_ExpectationLinearStatistic
@{
void C_ExpectationLinearStatistic
(
    @<C integer P Input@>,
    @<C integer Q Input@>,
    double *ExpInf,
    double *ExpX,
    const int add,
    double *PQ_ans
) {
    if (!add)
        for (int p = 0; p < mPQB(P, Q, 1); p++) PQ_ans[p] = 0.0;

    for (int p = 0; p < P; p++) {
        for (int q = 0; q < Q; q++)
            PQ_ans[q * P + p] += ExpX[p] * ExpInf[q];
    }
}
@|C_ExpectationLinearStatistic
@}

@d C_CovarianceLinearStatistic
@{
void C_CovarianceLinearStatistic
(
    @<C integer P Input@>,
    @<C integer Q Input@>,
    double *CovInf,
    double *ExpX,
    double *CovX,
    @<C sumweights Input@>,
    const int add,
    double *PQPQ_sym_ans
) {
    double f1 = sumweights / (sumweights - 1);
    double f2 = 1.0 / (sumweights - 1);
    double tmp, *PP_sym_tmp;

    if (mPQB(P, Q, 1) == 1) {
        tmp = f1 * CovInf[0] * CovX[0];
        tmp -= f2 * CovInf[0] * ExpX[0] * ExpX[0];
        if (add) {
            PQPQ_sym_ans[0] += tmp;
        } else {
            PQPQ_sym_ans[0] = tmp;
        }
    } else {
        PP_sym_tmp = R_Calloc(PP12(P), double);
        C_KronSums_sym_(ExpX, 1, P,
                        PP_sym_tmp);
        for (int p = 0; p < PP12(P); p++)
            PP_sym_tmp[p] = f1 * CovX[p] - f2 * PP_sym_tmp[p];
        C_kronecker_sym(CovInf, Q, PP_sym_tmp, P, 1 - (add >= 1),
                        PQPQ_sym_ans);
        R_Free(PP_sym_tmp);
    }
}
@|C_CovarianceLinearStatistic
@}

@d C_VarianceLinearStatistic
@{
void C_VarianceLinearStatistic
(
    @<C integer P Input@>,
    @<C integer Q Input@>,
    double *VarInf,
    double *ExpX,
    double *VarX,
    @<C sumweights Input@>,
    const int add,
    double *PQ_ans
) {
    if (mPQB(P, Q, 1) == 1) {
        C_CovarianceLinearStatistic(P, Q, VarInf, ExpX, VarX,
                                    sumweights, (add >= 1),
                                    PQ_ans);
    } else {
        double *P_tmp;
        P_tmp = R_Calloc(P, double);
        double f1 = sumweights / (sumweights - 1);
        double f2 = 1.0 / (sumweights - 1);
        for (int p = 0; p < P; p++)
            P_tmp[p] = f1 * VarX[p] - f2 * ExpX[p] * ExpX[p];
        C_kronecker(VarInf, 1, Q, P_tmp, 1, P, 1 - (add >= 1),
                    PQ_ans);
        R_Free(P_tmp);
    }
}
@|C_VarianceLinearStatistic
@}

\subsection{Influence}

<<ExpectationInfluence>>=
sumweights <- sum(weights[subset])
expecty <- colSums(y[subset, ] * weights[subset]) / sumweights

a0 <- expecty
a1 <- .Call(libcoin:::R_ExpectationInfluence, y, weights, subset)
a2 <- .Call(libcoin:::R_ExpectationInfluence, y, as.double(weights), as.double(subset))
a3 <- .Call(libcoin:::R_ExpectationInfluence, y, weights, as.double(subset))
a4 <- .Call(libcoin:::R_ExpectationInfluence, y, as.double(weights), subset)
a5 <- LinStatExpCov(x, y, weights = weights, subset = subset)$ExpectationInfluence

stopifnot(isequal(a0, a1) && isequal(a0, a2) &&
          isequal(a0, a3) && isequal(a0, a4) &&
          isequal(a0, a5))
@@

@d R_ExpectationInfluence Prototype
@{
SEXP R_ExpectationInfluence
(
    @<R y Input@>
    @<R weights Input@>,
    @<R subset Input@>
)
@}

@d R_ExpectationInfluence
@{
@<R_ExpectationInfluence Prototype@>
{
    SEXP ans;
    @<C integer Q Input@>;
    @<C integer N Input@>;
    @<C integer Nsubset Input@>;
    double sumweights;

    Q = NCOL(y);
    N = XLENGTH(y) / Q;
    Nsubset = XLENGTH(subset);

    sumweights = RC_Sums(N, weights, subset, Offset0, Nsubset);

    PROTECT(ans = allocVector(REALSXP, Q));
    RC_ExpectationInfluence(N, y, Q, weights, subset, Offset0, Nsubset, sumweights, REAL(ans));
    UNPROTECT(1);
    return(ans);
}
@|R_ExpectationInfluence
@}

@d RC_ExpectationInfluence Prototype
@{
void RC_ExpectationInfluence
(
    @<C integer N Input@>,
    @<R y Input@>
    @<C integer Q Input@>,
    @<R weights Input@>,
    @<R subset Input@>,
    @<C subset range Input@>,
    @<C sumweights Input@>,
    @<C colSums Answer@>
)
@}

@d RC_ExpectationInfluence
@{
@<RC_ExpectationInfluence Prototype@>
{
    double center;

    RC_colSums(REAL(y), N, Q, Power1, &center, !DoCenter, weights,
               subset, offset, Nsubset, P_ans);
    for (int q = 0; q < Q; q++)
        P_ans[q] = P_ans[q] / sumweights;
}
@|RC_ExpectationInfluence
@}

<<CovarianceInfluence>>=
sumweights <- sum(weights[subset])
yc <- t(t(y) - expecty)
r1y <- rep(1:ncol(y), ncol(y))
r2y <- rep(1:ncol(y), each = ncol(y))
a0 <- colSums(yc[subset, r1y] * yc[subset, r2y] * weights[subset]) / sumweights
a0 <- matrix(a0, ncol = ncol(y))
vary <- diag(a0)

a0 <- a0[lower.tri(a0, diag = TRUE)]
a1 <- .Call(libcoin:::R_CovarianceInfluence, y, weights, subset, 0L)
a2 <- .Call(libcoin:::R_CovarianceInfluence, y, as.double(weights), as.double(subset), 0L)
a3 <- .Call(libcoin:::R_CovarianceInfluence, y, weights, as.double(subset), 0L)
a4 <- .Call(libcoin:::R_CovarianceInfluence, y, as.double(weights), subset, 0L)
a5 <- LinStatExpCov(x, y, weights = weights, subset = subset)$CovarianceInfluence

stopifnot(isequal(a0, a1) && isequal(a0, a2) &&
          isequal(a0, a3) && isequal(a0, a4) &&
          isequal(a0, a5))

a0 <- vary
a1 <- .Call(libcoin:::R_CovarianceInfluence, y, weights, subset, 1L)
a2 <- .Call(libcoin:::R_CovarianceInfluence, y, as.double(weights), as.double(subset), 1L)
a3 <- .Call(libcoin:::R_CovarianceInfluence, y, weights, as.double(subset), 1L)
a4 <- .Call(libcoin:::R_CovarianceInfluence, y, as.double(weights), subset, 1L)
a5 <- LinStatExpCov(x, y, weights = weights, subset = subset, varonly = TRUE)$VarianceInfluence

stopifnot(isequal(a0, a1) && isequal(a0, a2) &&
          isequal(a0, a3) && isequal(a0, a4) &&
          isequal(a0, a5))
@@

@d R_CovarianceInfluence Prototype
@{
SEXP R_CovarianceInfluence
(
    @<R y Input@>
    @<R weights Input@>,
    @<R subset Input@>,
    SEXP varonly
)
@}

@d R_CovarianceInfluence
@{
@<R_CovarianceInfluence Prototype@>
{
    SEXP ans;
    SEXP ExpInf;
    @<C integer Q Input@>;
    @<C integer N Input@>;
    @<C integer Nsubset Input@>;
    double sumweights;

    Q = NCOL(y);
    N = XLENGTH(y) / Q;
    Nsubset = XLENGTH(subset);

    PROTECT(ExpInf = R_ExpectationInfluence(y, weights, subset));

    sumweights = RC_Sums(N, weights, subset, Offset0, Nsubset);

    if (INTEGER(varonly)[0]) {
        PROTECT(ans = allocVector(REALSXP, Q));
    } else {
        PROTECT(ans = allocVector(REALSXP, Q * (Q + 1) / 2));
    }
    RC_CovarianceInfluence(N, y, Q, weights, subset, Offset0, Nsubset, REAL(ExpInf), sumweights,
                           INTEGER(varonly)[0], REAL(ans));
    UNPROTECT(2);
    return(ans);
}
@|R_CovarianceInfluence
@}

@d RC_CovarianceInfluence Prototype
@{
void RC_CovarianceInfluence
(
    @<C integer N Input@>,
    @<R y Input@>
    @<C integer Q Input@>,
    @<R weights Input@>,
    @<R subset Input@>,
    @<C subset range Input@>,
    double *ExpInf,
    @<C sumweights Input@>,
    int VARONLY,
    @<C KronSums Answer@>
)
@}

@d RC_CovarianceInfluence
@{
@<RC_CovarianceInfluence Prototype@>
{
    if (VARONLY) {
        RC_colSums(REAL(y), N, Q, Power2, ExpInf, DoCenter, weights,
                   subset, offset, Nsubset, PQ_ans);
        for (int q = 0; q < Q; q++)
            PQ_ans[q] = PQ_ans[q] / sumweights;
    } else {
        RC_KronSums(y, N, Q, REAL(y), Q, DoSymmetric, ExpInf, ExpInf, DoCenter, weights,
                    subset, offset, Nsubset, PQ_ans);
        for (int q = 0; q < Q * (Q + 1) / 2; q++)
            PQ_ans[q] = PQ_ans[q] / sumweights;
    }
}
@|RC_CovarianceInfluence
@}

\subsection{X}

@d R_ExpectationX Prototype
@{
SEXP R_ExpectationX
(
    @<R x Input@>
    SEXP P,
    @<R weights Input@>,
    @<R subset Input@>
)
@}

@d R_ExpectationX
@{
@<R_ExpectationX Prototype@>
{
    SEXP ans;
    @<C integer N Input@>;
    @<C integer Nsubset Input@>;

    N = XLENGTH(x) / INTEGER(P)[0];
    Nsubset = XLENGTH(subset);

    PROTECT(ans = allocVector(REALSXP, INTEGER(P)[0]));
    RC_ExpectationX(x, N, INTEGER(P)[0], weights, subset,
                    Offset0, Nsubset, REAL(ans));
    UNPROTECT(1);
    return(ans);
}
@|R_ExpectationX
@}

@d RC_ExpectationX Prototype
@{
void RC_ExpectationX
(
    @<R x Input@>
    @<C integer N Input@>,
    @<C integer P Input@>,
    @<R weights Input@>,
    @<R subset Input@>,
    @<C subset range Input@>,
    @<C OneTableSums Answer@>
)
@}

@d RC_ExpectationX
@{
@<RC_ExpectationX Prototype@>
{
    double center;

    if (TYPEOF(x) == INTSXP) {
        double* Pp1tmp = R_Calloc(P + 1, double);
        RC_OneTableSums(INTEGER(x), N, P + 1, weights, subset, offset, Nsubset, Pp1tmp);
        for (int p = 0; p < P; p++) P_ans[p] = Pp1tmp[p + 1];
        R_Free(Pp1tmp);
    } else {
        RC_colSums(REAL(x), N, P, Power1, &center, !DoCenter, weights, subset, offset, Nsubset, P_ans);
    }
}
@|RC_ExpectationX
@}

<<ExpectationCovarianceX>>=
a0 <- colSums(x[subset, ] * weights[subset])
a1 <- .Call(libcoin:::R_ExpectationX, x, P, weights, subset);
a2 <- .Call(libcoin:::R_ExpectationX, x, P, as.double(weights), as.double(subset))
a3 <- .Call(libcoin:::R_ExpectationX, x, P, weights, as.double(subset))
a4 <- .Call(libcoin:::R_ExpectationX, x, P, as.double(weights), subset)

stopifnot(isequal(a0, a1) && isequal(a0, a2) &&
          isequal(a0, a3) && isequal(a0, a4) &&
          isequal(a0, LECVxyws$ExpectationX))

a0 <- colSums(x[subset, ]^2 * weights[subset])
a1 <- .Call(libcoin:::R_CovarianceX, x, P, weights, subset, 1L)
a2 <- .Call(libcoin:::R_CovarianceX, x, P, as.double(weights), as.double(subset), 1L)
a3 <- .Call(libcoin:::R_CovarianceX, x, P, weights, as.double(subset), 1L)
a4 <- .Call(libcoin:::R_CovarianceX, x, P, as.double(weights), subset, 1L)

stopifnot(isequal(a0, a1) && isequal(a0, a2) &&
          isequal(a0, a3) && isequal(a0, a4))

a0 <- as.vector(colSums(Xfactor[subset, ] * weights[subset]))
a1 <- .Call(libcoin:::R_ExpectationX, ix, Lx, weights, subset)
a2 <- .Call(libcoin:::R_ExpectationX, ix, Lx, as.double(weights), as.double(subset))
a3 <- .Call(libcoin:::R_ExpectationX, ix, Lx, weights, as.double(subset))
a4 <- .Call(libcoin:::R_ExpectationX, ix, Lx, as.double(weights), subset)

stopifnot(isequal(a0, a1) && isequal(a0, a2) &&
          isequal(a0, a3) && isequal(a0, a4))

a1 <- .Call(libcoin:::R_CovarianceX, ix, Lx, weights, subset, 1L)
a2 <- .Call(libcoin:::R_CovarianceX, ix, Lx, as.double(weights), as.double(subset), 1L)
a3 <- .Call(libcoin:::R_CovarianceX, ix, Lx, weights, as.double(subset), 1L)
a4 <- .Call(libcoin:::R_CovarianceX, ix, Lx, as.double(weights), subset, 1L)

stopifnot(isequal(a0, a1) && isequal(a0, a2) &&
          isequal(a0, a3) && isequal(a0, a4))

r1x <- rep(1:ncol(Xfactor), ncol(Xfactor))
r2x <- rep(1:ncol(Xfactor), each = ncol(Xfactor))
a0 <- colSums(Xfactor[subset, r1x] * Xfactor[subset, r2x] * weights[subset])
a0 <- matrix(a0, ncol = ncol(Xfactor))
vary <- diag(a0)

a0 <- a0[lower.tri(a0, diag = TRUE)]
a1 <- .Call(libcoin:::R_CovarianceX, ix, Lx, weights, subset, 0L)
a2 <- .Call(libcoin:::R_CovarianceX, ix, Lx, as.double(weights), as.double(subset), 0L)
a3 <- .Call(libcoin:::R_CovarianceX, ix, Lx, weights, as.double(subset), 0L)
a4 <- .Call(libcoin:::R_CovarianceX, ix, Lx, as.double(weights), subset, 0L)

stopifnot(isequal(a0, a1) && isequal(a0, a2) &&
          isequal(a0, a3) && isequal(a0, a4))
@@

@d R_CovarianceX Prototype
@{
SEXP R_CovarianceX
(
    @<R x Input@>
    SEXP P,
    @<R weights Input@>,
    @<R subset Input@>,
    SEXP varonly
)
@}

@d R_CovarianceX
@{
@<R_CovarianceX Prototype@>
{
    SEXP ans;
    SEXP ExpX;
    @<C integer N Input@>;
    @<C integer Nsubset Input@>;

    N = XLENGTH(x) / INTEGER(P)[0];
    Nsubset = XLENGTH(subset);

    PROTECT(ExpX = R_ExpectationX(x, P, weights, subset));

    if (INTEGER(varonly)[0]) {
        PROTECT(ans = allocVector(REALSXP, INTEGER(P)[0]));
    } else {
        PROTECT(ans = allocVector(REALSXP, INTEGER(P)[0] * (INTEGER(P)[0] + 1) / 2));
    }
    RC_CovarianceX(x, N, INTEGER(P)[0], weights, subset, Offset0, Nsubset, REAL(ExpX),
                   INTEGER(varonly)[0], REAL(ans));
    UNPROTECT(2);
    return(ans);
}
@|R_CovarianceX
@}

@d RC_CovarianceX Prototype
@{
void RC_CovarianceX
(
    @<R x Input@>
    @<C integer N Input@>,
    @<C integer P Input@>,
    @<R weights Input@>,
    @<R subset Input@>,
    @<C subset range Input@>,
    double *ExpX,
    int VARONLY,
    @<C KronSums Answer@>
)
@}

@d RC_CovarianceX
@{
@<RC_CovarianceX Prototype@>
{
    double center;

    if (TYPEOF(x) == INTSXP) {
        if (VARONLY) {
            for (int p = 0; p < P; p++) PQ_ans[p] = ExpX[p];
        } else {
            for (int p = 0; p < PP12(P); p++)
                PQ_ans[p] = 0.0;
            for (int p = 0; p < P; p++)
                PQ_ans[S(p, p, P)] = ExpX[p];
        }
    } else {
        if (VARONLY) {
            RC_colSums(REAL(x), N, P, Power2, &center, !DoCenter, weights,
                       subset, offset, Nsubset, PQ_ans);
        } else {
            RC_KronSums(x, N, P, REAL(x), P, DoSymmetric, &center, &center, !DoCenter, weights,
                        subset, offset, Nsubset, PQ_ans);
        }
    }
}
@|RC_CovarianceX
@}

\section{Computing Sums}

The core concept of all functions in the section is the computation of
various sums over observations, case weights, or blocks. We start with an
initialisation of the loop over all observations

@d init subset loop
@{
R_xlen_t diff = 0;
s = subset + offset;
w = weights;
/* subset is R-style index in 1:N */
if (Nsubset > 0)
    diff = (R_xlen_t) s[0] - 1;
@}

and loop over $i = 1, \dots, N$ when no subset was specified or
over the subset of the subset given by \code{offset} and \code{Nsubset},
allowing for number of observations larger than \code{INT_MAX}

@d start subset loop
@{
for (R_xlen_t i = 0; i < (Nsubset == 0 ? N : Nsubset) - 1; i++)
@}

After computions in the loop, we compute the next element

@d continue subset loop
@{
if (Nsubset > 0) {
    /* NB: diff also works with R style index */
    diff = (R_xlen_t) s[1] - s[0];
    if (diff < 0)
        error("subset not sorted");
    s++;
} else {
    diff = 1;
}
@}

\subsection{Simple Sums}

@d SimpleSums
@{
@<C_Sums_dweights_dsubset@>
@<C_Sums_iweights_dsubset@>
@<C_Sums_iweights_isubset@>
@<C_Sums_dweights_isubset@>
@<RC_Sums@>
@<R_Sums@>
@}

<<SimpleSums>>=
a0 <- sum(weights[subset])
a1 <- .Call(libcoin:::R_Sums, N, weights, subset)
a2 <- .Call(libcoin:::R_Sums, N, as.double(weights), as.double(subset))
a3 <- .Call(libcoin:::R_Sums, N, weights, as.double(subset))
a4 <- .Call(libcoin:::R_Sums, N, as.double(weights), subset)

stopifnot(isequal(a0, a1) && isequal(a0, a2) &&
          isequal(a0, a3) && isequal(a0, a4))
@@

@d R_Sums Prototype
@{
SEXP R_Sums
(
    @<R N Input@>
    @<R weights Input@>,
    @<R subset Input@>
)
@}

@d R_Sums
@{
@<R_Sums Prototype@>
{
    SEXP ans;
    @<C integer Nsubset Input@>;

    Nsubset = XLENGTH(subset);

    PROTECT(ans = allocVector(REALSXP, 1));
    REAL(ans)[0] = RC_Sums(INTEGER(N)[0], weights, subset, Offset0, Nsubset);
    UNPROTECT(1);

    return(ans);
}
@|R_Sums
@}

@d RC_Sums Prototype
@{
double RC_Sums
(
    @<C integer N Input@>,
    @<R weights Input@>,
    @<R subset Input@>,
    @<C subset range Input@>
)
@}

@d RC_Sums
@{
@<RC_Sums Prototype@>
{
    if (XLENGTH(weights) == 0) {
        if (XLENGTH(subset) == 0) {
            return((double) N);
        } else {
            return((double) Nsubset);
        }
    }
    if (TYPEOF(weights) == INTSXP) {
        if (TYPEOF(subset) == INTSXP) {
            return(C_Sums_iweights_isubset(N, INTEGER(weights), XLENGTH(weights),
                                           INTEGER(subset), offset, Nsubset));
        } else {
            return(C_Sums_iweights_dsubset(N, INTEGER(weights), XLENGTH(weights),
                                           REAL(subset), offset, Nsubset));
        }
    } else {
        if (TYPEOF(subset) == INTSXP) {
            return(C_Sums_dweights_isubset(N, REAL(weights), XLENGTH(weights),
                                           INTEGER(subset), offset, Nsubset));
        } else {
            return(C_Sums_dweights_dsubset(N, REAL(weights), XLENGTH(weights),
                                           REAL(subset), offset, Nsubset));
        }
    }
}
@|RC_Sums
@}

@d C_Sums_dweights_dsubset
@{
double C_Sums_dweights_dsubset
(
    @<C integer N Input@>,
    @<C real weights Input@>
    @<C real subset Input@>
) {
    double *s, *w;
    @<Sums Body@>
}
@|C_Sums_dweights_dsubset
@}

@d C_Sums_iweights_dsubset
@{
double C_Sums_iweights_dsubset
(
    @<C integer N Input@>,
    @<C integer weights Input@>
    @<C real subset Input@>
) {
    double *s;
    int *w;
    @<Sums Body@>
}
@|C_Sums_iweights_dsubset
@}

@d C_Sums_iweights_isubset
@{
double C_Sums_iweights_isubset
(
    @<C integer N Input@>,
    @<C integer weights Input@>
    @<C integer subset Input@>
) {
    int *s, *w;
    @<Sums Body@>
}
@|C_Sums_iweights_isubset
@}

@d C_Sums_dweights_isubset
@{
double C_Sums_dweights_isubset
(
    @<C integer N Input@>,
    @<C real weights Input@>
    @<C integer subset Input@>
) {
    int *s;
    double *w;
    @<Sums Body@>
}
@|C_Sums_dweights_isubset
@}

@d Sums Body
@{
double ans = 0.0;

if (Nsubset > 0) {
    if (!HAS_WEIGHTS) return((double) Nsubset);
} else {
    if (!HAS_WEIGHTS) return((double) N);
}

@<init subset loop@>
@<start subset loop@>
{
    w = w + diff;
    ans += w[0];
    @<continue subset loop@>
}
w = w + diff;
ans += w[0];

return(ans);
@}

\subsection{Kronecker Sums}

@d KronSums
@{
@<C_KronSums_dweights_dsubset@>
@<C_KronSums_iweights_dsubset@>
@<C_KronSums_iweights_isubset@>
@<C_KronSums_dweights_isubset@>
@<C_XfactorKronSums_dweights_dsubset@>
@<C_XfactorKronSums_iweights_dsubset@>
@<C_XfactorKronSums_iweights_isubset@>
@<C_XfactorKronSums_dweights_isubset@>
@<RC_KronSums@>
@<R_KronSums@>
@<C_KronSums_Permutation_isubset@>
@<C_KronSums_Permutation_dsubset@>
@<C_XfactorKronSums_Permutation_isubset@>
@<C_XfactorKronSums_Permutation_dsubset@>
@<RC_KronSums_Permutation@>
@<R_KronSums_Permutation@>
@}

<<KronSums>>=
r1 <- rep(1:ncol(x), ncol(y))
r2 <- rep(1:ncol(y), each = ncol(x))

a0 <- colSums(x[subset, r1] * y[subset, r2] * weights[subset])
a1 <- .Call(libcoin:::R_KronSums, x, P, y, weights, subset, 0L)
a2 <- .Call(libcoin:::R_KronSums, x, P, y, as.double(weights), as.double(subset), 0L)
a3 <- .Call(libcoin:::R_KronSums, x, P, y, weights, as.double(subset), 0L)
a4 <- .Call(libcoin:::R_KronSums, x, P, y, as.double(weights), subset, 0L)

stopifnot(isequal(a0, a1) && isequal(a0, a2) &&
          isequal(a0, a3) && isequal(a0, a4))

a0 <- as.vector(colSums(Xfactor[subset, r1Xfactor] *
                        y[subset, r2Xfactor] * weights[subset]))
a1 <- .Call(libcoin:::R_KronSums, ix, Lx, y, weights, subset, 0L)
a2 <- .Call(libcoin:::R_KronSums, ix, Lx, y, as.double(weights), as.double(subset), 0L)
a3 <- .Call(libcoin:::R_KronSums, ix, Lx, y, weights, as.double(subset), 0L)
a4 <- .Call(libcoin:::R_KronSums, ix, Lx, y, as.double(weights), subset, 0L)

stopifnot(isequal(a0, a1) && isequal(a0, a2) &&
          isequal(a0, a3) && isequal(a0, a4))
@@

@d R_KronSums Prototype
@{
SEXP R_KronSums
(
    @<R x Input@>
    SEXP P,
    @<R y Input@>
    @<R weights Input@>,
    @<R subset Input@>,
    SEXP symmetric
)
@}

@d R_KronSums
@{
@<R_KronSums Prototype@>
{
    SEXP ans;
    @<C integer Q Input@>;
    @<C integer N Input@>;
    @<C integer Nsubset Input@>;

    double center;

    Q = NCOL(y);
    N = XLENGTH(y) / Q;
    Nsubset = XLENGTH(subset);

    if (INTEGER(symmetric)[0]) {
        PROTECT(ans = allocVector(REALSXP, INTEGER(P)[0] * (INTEGER(P)[0] + 1) / 2));
    } else {
        PROTECT(ans = allocVector(REALSXP, INTEGER(P)[0] * Q));
    }
    RC_KronSums(x, N, INTEGER(P)[0], REAL(y), Q, INTEGER(symmetric)[0], &center, &center,
                !DoCenter, weights, subset, Offset0, Nsubset, REAL(ans));
    UNPROTECT(1);
    return(ans);
}
@|R_KronSums
@}

@d RC_KronSums Prototype
@{
void RC_KronSums
(
    @<RC KronSums Input@>
    @<R weights Input@>,
    @<R subset Input@>,
    @<C subset range Input@>,
    @<C KronSums Answer@>
)
@}

@d RC_KronSums
@{
@<RC_KronSums Prototype@>
{
    if (TYPEOF(x) == INTSXP) {
        @<KronSums Integer x@>
    } else {
        @<KronSums Double x@>
    }
}
@|RC_KronSums
@}

@d RC KronSums Input
@{
@<R x Input@>
@<C integer N Input@>,
@<C integer P Input@>,
@<C real y Input@>
const int SYMMETRIC,
double *centerx,
double *centery,
const int CENTER,
@}

@d C KronSums Input
@{
@<C real x Input@>
@<C real y Input@>
const int SYMMETRIC,
double *centerx,
double *centery,
const int CENTER,
@}

@d C KronSums Answer
@{
double *PQ_ans
@}

@d KronSums Integer x
@{
if (SYMMETRIC) error("not implemented");
if (CENTER) error("not implemented");
if (TYPEOF(weights) == INTSXP) {
    if (TYPEOF(subset) == INTSXP) {
        C_XfactorKronSums_iweights_isubset(INTEGER(x), N, P, y, Q,
            INTEGER(weights), XLENGTH(weights) > 0, INTEGER(subset),
            offset, Nsubset, PQ_ans);
    } else {
        C_XfactorKronSums_iweights_dsubset(INTEGER(x), N, P, y, Q,
            INTEGER(weights), XLENGTH(weights) > 0, REAL(subset),
            offset, Nsubset, PQ_ans);
    }
} else {
    if (TYPEOF(subset) == INTSXP) {
        C_XfactorKronSums_dweights_isubset(INTEGER(x), N, P, y, Q,
            REAL(weights), XLENGTH(weights) > 0, INTEGER(subset),
            offset, Nsubset, PQ_ans);
    } else {
        C_XfactorKronSums_dweights_dsubset(INTEGER(x), N, P, y, Q,
            REAL(weights), XLENGTH(weights) > 0, REAL(subset),
            offset, Nsubset, PQ_ans);
    }
}
@}

@d KronSums Double x
@{
if (TYPEOF(weights) == INTSXP) {
    if (TYPEOF(subset) == INTSXP) {
        C_KronSums_iweights_isubset(REAL(x), N, P, y, Q, SYMMETRIC, centerx, centery, CENTER,
            INTEGER(weights), XLENGTH(weights) > 0, INTEGER(subset),
            offset, Nsubset, PQ_ans);
    } else {
        C_KronSums_iweights_dsubset(REAL(x), N, P, y, Q, SYMMETRIC, centerx, centery, CENTER,
            INTEGER(weights), XLENGTH(weights) > 0, REAL(subset),
            offset, Nsubset, PQ_ans);
    }
} else {
    if (TYPEOF(subset) == INTSXP) {
        C_KronSums_dweights_isubset(REAL(x), N, P, y, Q, SYMMETRIC, centerx, centery, CENTER,
            REAL(weights), XLENGTH(weights) > 0, INTEGER(subset),
            offset, Nsubset, PQ_ans);
    } else {
        C_KronSums_dweights_dsubset(REAL(x), N, P, y, Q, SYMMETRIC, centerx, centery, CENTER,
            REAL(weights), XLENGTH(weights) > 0, REAL(subset),
            offset, Nsubset, PQ_ans);
    }
}
@}

@d C_KronSums_dweights_dsubset
@{
void C_KronSums_dweights_dsubset
(
    @<C KronSums Input@>
    @<C real weights Input@>
    @<C real subset Input@>,
    @<C KronSums Answer@>
) {
    double *s, *w;
    @<KronSums Body@>
}
@|C_KronSums_dweights_dsubset
@}

@d C_KronSums_iweights_dsubset
@{
void C_KronSums_iweights_dsubset
(
    @<C KronSums Input@>
    @<C integer weights Input@>
    @<C real subset Input@>,
    @<C KronSums Answer@>
) {
    double *s;
    int *w;
    @<KronSums Body@>
}
@|C_KronSums_iweights_dsubset
@}

@d C_KronSums_iweights_isubset
@{
void C_KronSums_iweights_isubset
(
    @<C KronSums Input@>
    @<C integer weights Input@>
    @<C integer subset Input@>,
    @<C KronSums Answer@>
) {
    int *s, *w;
    @<KronSums Body@>
}
@|C_KronSums_iweights_isubset
@}

@d C_KronSums_dweights_isubset
@{
void C_KronSums_dweights_isubset
(
    @<C KronSums Input@>
    @<C real weights Input@>
    @<C integer subset Input@>,
    @<C KronSums Answer@>
) {
    int *s;
    double *w;
    @<KronSums Body@>
}
@|C_KronSums_dweights_isubset
@}

@d KronSums Body
@{
    double *xx, *yy, cx = 0.0, cy = 0.0, *thisPQ_ans;
    int idx;

    for (int p = 0; p < P; p++) {
        for (int q = (SYMMETRIC ? p : 0); q < Q; q++) {
            /* SYMMETRIC is column-wise, default
               is row-wise (maybe need to change this) */
            if (SYMMETRIC) {
                idx = S(p, q, P);
            } else {
                idx = q * P + p;
            }
            PQ_ans[idx] = 0.0;
            thisPQ_ans = PQ_ans + idx;
            yy = y + N * q;
            xx = x + N * p;

            if (CENTER) {
                cx = centerx[p];
                cy = centery[q];
            }
            @<init subset loop@>
            @<start subset loop@>
            {
                xx = xx + diff;
                yy = yy + diff;
                if (HAS_WEIGHTS) {
                    w = w + diff;
                    if (CENTER) {
                        thisPQ_ans[0] += (xx[0] - cx) * (yy[0] - cy) * w[0];
                    } else {
                        thisPQ_ans[0] += xx[0] * yy[0] * w[0];
                    }
                } else {
                    if (CENTER) {
                        thisPQ_ans[0] += (xx[0] - cx) * (yy[0] - cy);
                    } else {
                        thisPQ_ans[0] += xx[0] * yy[0];
                    }
                }
                @<continue subset loop@>
            }
            xx = xx + diff;
            yy = yy + diff;
            if (HAS_WEIGHTS) {
                w = w + diff;
                thisPQ_ans[0] += (xx[0] - cx) * (yy[0] - cy) * w[0];
            } else {
                thisPQ_ans[0] += (xx[0] - cx) * (yy[0] - cy);
            }
        }
    }
@}

\subsubsection{Xfactor Kronecker Sums}

@d C XfactorKronSums Input
@{
@<C integer x Input@>
@<C real y Input@>
@}

@d C_XfactorKronSums_dweights_dsubset
@{
void C_XfactorKronSums_dweights_dsubset
(
    @<C XfactorKronSums Input@>
    @<C real weights Input@>
    @<C real subset Input@>,
    @<C KronSums Answer@>
) {
    double *s, *w;
    @<XfactorKronSums Body@>
}
@|C_XfactorKronSums_dweights_dsubset
@}

@d C_XfactorKronSums_iweights_dsubset
@{
void C_XfactorKronSums_iweights_dsubset
(
    @<C XfactorKronSums Input@>
    @<C integer weights Input@>
    @<C real subset Input@>,
    @<C KronSums Answer@>
) {
    double *s;
    int *w;
    @<XfactorKronSums Body@>
}
@|C_XfactorKronSums_iweights_dsubset
@}

@d C_XfactorKronSums_iweights_isubset
@{
void C_XfactorKronSums_iweights_isubset
(
    @<C XfactorKronSums Input@>
    @<C integer weights Input@>
    @<C integer subset Input@>,
    @<C KronSums Answer@>
) {
    int *s, *w;
    @<XfactorKronSums Body@>
}
@|C_XfactorKronSums_iweights_isubset
@}

@d C_XfactorKronSums_dweights_isubset
@{
void C_XfactorKronSums_dweights_isubset
(
    @<C XfactorKronSums Input@>
    @<C real weights Input@>
    @<C integer subset Input@>,
    @<C KronSums Answer@>
) {
    int *s;
    double *w;
    @<XfactorKronSums Body@>
}
@|C_XfactorKronSums_dweights_isubset
@}

@d XfactorKronSums Body
@{
int *xx, ixi;
double *yy;

for (int p = 0; p < mPQB(P, Q, 1); p++) PQ_ans[p] = 0.0;

for (int q = 0; q < Q; q++) {
    yy = y + N * q;
    xx = x;
    @<init subset loop@>
    @<start subset loop@>
    {
        xx = xx + diff;
        yy = yy + diff;
        ixi = xx[0] - 1;
        if (HAS_WEIGHTS) {
            w = w + diff;
            if (ixi >= 0)
                PQ_ans[ixi + q * P] += yy[0] * w[0];
        } else {
            if (ixi >= 0)
                PQ_ans[ixi + q * P] += yy[0];
        }
        @<continue subset loop@>
    }
    xx = xx + diff;
    yy = yy + diff;
    ixi = xx[0] - 1;
    if (HAS_WEIGHTS) {
        w = w + diff;
        if (ixi >= 0)
            PQ_ans[ixi + q * P] += yy[0] * w[0];
    } else {
        if (ixi >= 0)
            PQ_ans[ixi + q * P] += yy[0];
    }
}
@}

\subsubsection{Permuted Kronecker Sums}

<<KronSums-Permutation>>=
a0 <- colSums(x[subset, r1] * y[subsety, r2])
a1 <- .Call(libcoin:::R_KronSums_Permutation, x, P, y, subset, subsety)
a2 <- .Call(libcoin:::R_KronSums_Permutation, x, P, y, as.double(subset), as.double(subsety))

stopifnot(isequal(a0, a1) && isequal(a0, a2))

a0 <- as.vector(colSums(Xfactor[subset, r1Xfactor] * y[subsety, r2Xfactor]))
a1 <- .Call(libcoin:::R_KronSums_Permutation, ix, Lx, y, subset, subsety)
a2 <- .Call(libcoin:::R_KronSums_Permutation, ix, Lx, y, as.double(subset), as.double(subsety))

stopifnot(isequal(a0, a1) && isequal(a0, a2))
@@

@d R_KronSums_Permutation Prototype
@{
SEXP R_KronSums_Permutation
(
    @<R x Input@>
    SEXP P,
    @<R y Input@>
    @<R subset Input@>,
    SEXP subsety
)
@}

@d R_KronSums_Permutation
@{
@<R_KronSums_Permutation Prototype@>
{
    SEXP ans;
    @<C integer Q Input@>;
    @<C integer N Input@>;
    @<C integer Nsubset Input@>;

    Q = NCOL(y);
    N = XLENGTH(y) / Q;
    Nsubset = XLENGTH(subset);

    PROTECT(ans = allocVector(REALSXP, INTEGER(P)[0] * Q));
    RC_KronSums_Permutation(x, N, INTEGER(P)[0], REAL(y), Q, subset, Offset0, Nsubset,
                            subsety, REAL(ans));
    UNPROTECT(1);
    return(ans);
}
@|R_KronSums_Permutation
@}

@d RC_KronSums_Permutation Prototype
@{
void RC_KronSums_Permutation
(
    @<R x Input@>
    @<C integer N Input@>,
    @<C integer P Input@>,
    @<C real y Input@>
    @<R subset Input@>,
    @<C subset range Input@>,
    SEXP subsety,
    @<C KronSums Answer@>
)
@}

@d RC_KronSums_Permutation
@{
@<RC_KronSums_Permutation Prototype@>
{
    if (TYPEOF(x) == INTSXP) {
        if (TYPEOF(subset) == INTSXP) {
            C_XfactorKronSums_Permutation_isubset(INTEGER(x), N, P, y, Q,
                                                  INTEGER(subset), offset, Nsubset,
                                                  INTEGER(subsety), PQ_ans);
        } else {
            C_XfactorKronSums_Permutation_dsubset(INTEGER(x), N, P, y, Q,
                                                  REAL(subset), offset, Nsubset,
                                                  REAL(subsety), PQ_ans);
        }
    } else {
        if (TYPEOF(subset) == INTSXP) {
            C_KronSums_Permutation_isubset(REAL(x), N, P, y, Q,
                                           INTEGER(subset), offset, Nsubset,
                                           INTEGER(subsety), PQ_ans);
        } else {
            C_KronSums_Permutation_dsubset(REAL(x), N, P, y, Q,
                                           REAL(subset), offset, Nsubset,
                                           REAL(subsety), PQ_ans);
        }
    }
}
@|RC_KronSums_Permutation
@}

@d C_KronSums_Permutation_dsubset
@{
void C_KronSums_Permutation_dsubset
(
    @<C real x Input@>
    @<C real y Input@>
    @<C real subset Input@>,
    double *subsety,
    @<C KronSums Answer@>
) {
    @<KronSums Permutation Body@>
}
@|C_KronSums_Permutation_dsubset
@}

@d C_KronSums_Permutation_isubset
@{
void C_KronSums_Permutation_isubset
(
    @<C real x Input@>
    @<C real y Input@>
    @<C integer subset Input@>,
    int *subsety,
    @<C KronSums Answer@>
) {
    @<KronSums Permutation Body@>
}
@|C_KronSums_Permutation_isubset
@}

Because \code{subset} might not be ordered (in the presence of blocks) we
have to go through all elements explicitly here.

@d KronSums Permutation Body
@{
R_xlen_t qP, qN, pN, qPp;

for (int q = 0; q < Q; q++) {
    qN = q * N;
    qP = q * P;
    for (int p = 0; p < P; p++) {
        qPp = qP + p;
        PQ_ans[qPp] = 0.0;
        pN = p * N;
        for (R_xlen_t i = offset; i < Nsubset; i++)
            PQ_ans[qPp] += y[qN + (R_xlen_t) subsety[i] - 1] *
                           x[pN + (R_xlen_t) subset[i] - 1];
    }
}
@}

\subsubsection{Xfactor Permuted Kronecker Sums}

@d C_XfactorKronSums_Permutation_dsubset
@{
void C_XfactorKronSums_Permutation_dsubset
(
    @<C integer x Input@>
    @<C real y Input@>
    @<C real subset Input@>,
    double *subsety,
    @<C KronSums Answer@>
) {
    @<XfactorKronSums Permutation Body@>
}
@|C_XfactorKronSums_Permutation_dsubset
@}

@d C_XfactorKronSums_Permutation_isubset
@{
void C_XfactorKronSums_Permutation_isubset
(
    @<C integer x Input@>
    @<C real y Input@>
    @<C integer subset Input@>,
    int *subsety,
    @<C KronSums Answer@>
) {
    @<XfactorKronSums Permutation Body@>
}
@|C_XfactorKronSums_Permutation_isubset
@}

@d XfactorKronSums Permutation Body
@{
R_xlen_t qP, qN;

for (int p = 0; p < mPQB(P, Q, 1); p++) PQ_ans[p] = 0.0;

for (int q = 0; q < Q; q++) {
    qP = q * P;
    qN = q * N;
    for (R_xlen_t i = offset; i < Nsubset; i++)
        PQ_ans[x[(R_xlen_t) subset[i] - 1] - 1 + qP] += y[qN + (R_xlen_t) subsety[i] - 1];
}
@}

\subsection{Column Sums}

@d colSums
@{
@<C_colSums_dweights_dsubset@>
@<C_colSums_iweights_dsubset@>
@<C_colSums_iweights_isubset@>
@<C_colSums_dweights_isubset@>
@<RC_colSums@>
@<R_colSums@>
@}

<<colSums>>=
a0 <- colSums(x[subset, ] * weights[subset])
a1 <- .Call(libcoin:::R_colSums, x, weights, subset)
a2 <- .Call(libcoin:::R_colSums, x, as.double(weights), as.double(subset))
a3 <- .Call(libcoin:::R_colSums, x, weights, as.double(subset))
a4 <- .Call(libcoin:::R_colSums, x, as.double(weights), subset)

stopifnot(isequal(a0, a1) && isequal(a0, a2) &&
          isequal(a0, a3) && isequal(a0, a4))
@@

@d R_colSums Prototype
@{
SEXP R_colSums
(
    @<R x Input@>
    @<R weights Input@>,
    @<R subset Input@>
)
@}

@d R_colSums
@{
@<R_colSums Prototype@>
{
    SEXP ans;
    int P;
    @<C integer N Input@>;
    @<C integer Nsubset Input@>;
    double center;

    P = NCOL(x);
    N = XLENGTH(x) / P;
    Nsubset = XLENGTH(subset);

    PROTECT(ans = allocVector(REALSXP, P));
    RC_colSums(REAL(x), N, P, Power1, &center, !DoCenter, weights, subset, Offset0,
               Nsubset, REAL(ans));
    UNPROTECT(1);
    return(ans);
}
@|R_colSums
@}

@d RC_colSums Prototype
@{
void RC_colSums
(
    @<C colSums Input@>
    @<R weights Input@>,
    @<R subset Input@>,
    @<C subset range Input@>,
    @<C colSums Answer@>
)
@}

@d RC_colSums
@{
@<RC_colSums Prototype@>
{
    if (TYPEOF(weights) == INTSXP) {
        if (TYPEOF(subset) == INTSXP) {
            C_colSums_iweights_isubset(x, N, P, power, centerx, CENTER,
                                       INTEGER(weights), XLENGTH(weights) > 0, INTEGER(subset),
                                       offset, Nsubset, P_ans);
        } else {
            C_colSums_iweights_dsubset(x, N, P, power, centerx, CENTER,
                                       INTEGER(weights), XLENGTH(weights) > 0, REAL(subset),
                                       offset, Nsubset, P_ans);
        }
    } else {
        if (TYPEOF(subset) == INTSXP) {
            C_colSums_dweights_isubset(x, N, P, power, centerx, CENTER,
                                       REAL(weights), XLENGTH(weights) > 0, INTEGER(subset),
                                       offset, Nsubset, P_ans);
        } else {
            C_colSums_dweights_dsubset(x, N, P, power, centerx, CENTER,
                                       REAL(weights), XLENGTH(weights) > 0, REAL(subset),
                                       offset, Nsubset, P_ans);
        }
    }
}
@|RC_colSums
@}

@d C colSums Input
@{
@<C real x Input@>
const int power,
double *centerx,
const int CENTER,
@}

@d C colSums Answer
@{
double *P_ans
@}

@d C_colSums_dweights_dsubset
@{
void C_colSums_dweights_dsubset
(
    @<C colSums Input@>
    @<C real weights Input@>
    @<C real subset Input@>,
    @<C colSums Answer@>
) {
    double *s, *w;
    @<colSums Body@>
}
@|C_colSums_dweights_dsubset
@}

@d C_colSums_iweights_dsubset
@{
void C_colSums_iweights_dsubset
(
    @<C colSums Input@>
    @<C integer weights Input@>
    @<C real subset Input@>,
    @<C colSums Answer@>
) {
    double *s;
    int *w;
    @<colSums Body@>
}
@|C_colSums_iweights_dsubset
@}

@d C_colSums_iweights_isubset
@{
void C_colSums_iweights_isubset
(
    @<C colSums Input@>
    @<C integer weights Input@>
    @<C integer subset Input@>,
    @<C colSums Answer@>
) {
    int *s, *w;
    @<colSums Body@>
}
@|C_colSums_iweights_isubset
@}

@d C_colSums_dweights_isubset
@{
void C_colSums_dweights_isubset
(
    @<C colSums Input@>
    @<C real weights Input@>
    @<C integer subset Input@>,
    @<C colSums Answer@>
) {
    int *s;
    double *w;
    @<colSums Body@>
}
@|C_colSums_dweights_isubset
@}

@d colSums Body
@{
double *xx, cx = 0.0;

for (int p = 0; p < P; p++) {
    P_ans[0] = 0.0;
    xx = x + N * p;
    if (CENTER) {
        cx = centerx[p];
    }
    @<init subset loop@>
    @<start subset loop@>
    {
        xx = xx + diff;
        if (HAS_WEIGHTS) {
            w = w + diff;
            P_ans[0] += pow(xx[0] - cx, power) * w[0];
        } else {
            P_ans[0] += pow(xx[0] - cx, power);
        }
        @<continue subset loop@>
    }
    xx = xx + diff;
    if (HAS_WEIGHTS) {
        w = w + diff;
        P_ans[0] += pow(xx[0] - cx, power) * w[0];
    } else {
        P_ans[0] += pow(xx[0] - cx, power);
    }
    P_ans++;
}
@}

\subsection{Tables}

\subsubsection{OneTable Sums}

@d Tables
@{
@<C_OneTableSums_dweights_dsubset@>
@<C_OneTableSums_iweights_dsubset@>
@<C_OneTableSums_iweights_isubset@>
@<C_OneTableSums_dweights_isubset@>
@<RC_OneTableSums@>
@<R_OneTableSums@>
@<C_TwoTableSums_dweights_dsubset@>
@<C_TwoTableSums_iweights_dsubset@>
@<C_TwoTableSums_iweights_isubset@>
@<C_TwoTableSums_dweights_isubset@>
@<RC_TwoTableSums@>
@<R_TwoTableSums@>
@<C_ThreeTableSums_dweights_dsubset@>
@<C_ThreeTableSums_iweights_dsubset@>
@<C_ThreeTableSums_iweights_isubset@>
@<C_ThreeTableSums_dweights_isubset@>
@<RC_ThreeTableSums@>
@<R_ThreeTableSums@>
@}

<<OneTableSum>>=
a0 <- as.vector(xtabs(weights ~ ixf, subset = subset))
a1 <- ctabs(ix, weights = weights, subset = subset)[-1]
a2 <- ctabs(ix, weights = as.double(weights), subset = as.double(subset))[-1]
a3 <- ctabs(ix, weights = weights, subset = as.double(subset))[-1]
a4 <- ctabs(ix, weights = as.double(weights), subset = subset)[-1]

stopifnot(isequal(a0, a1) && isequal(a0, a2) &&
          isequal(a0, a3) && isequal(a0, a4))
@@

@d R_OneTableSums Prototype
@{
SEXP R_OneTableSums
(
    @<R x Input@>
    @<R weights Input@>,
    @<R subset Input@>
)
@}

@d R_OneTableSums
@{
@<R_OneTableSums Prototype@>
{
    SEXP ans;
    @<C integer N Input@>;
    @<C integer Nsubset Input@>;
    int P;

    N = XLENGTH(x);
    Nsubset = XLENGTH(subset);
    P = NLEVELS(x) + 1;

    PROTECT(ans = allocVector(REALSXP, P));
    RC_OneTableSums(INTEGER(x), N, P, weights, subset,
                    Offset0, Nsubset, REAL(ans));
    UNPROTECT(1);
    return(ans);
}
@|R_OneTableSums
@}

@d RC_OneTableSums Prototype
@{
void RC_OneTableSums
(
    @<C OneTableSums Input@>
    @<R weights Input@>,
    @<R subset Input@>,
    @<C subset range Input@>,
    @<C OneTableSums Answer@>
)
@}

@d RC_OneTableSums
@{
@<RC_OneTableSums Prototype@>
{
    if (TYPEOF(weights) == INTSXP) {
        if (TYPEOF(subset) == INTSXP) {
            C_OneTableSums_iweights_isubset(x, N, P,
                                        INTEGER(weights), XLENGTH(weights) > 0, INTEGER(subset),
                                        offset, Nsubset, P_ans);
        } else {
            C_OneTableSums_iweights_dsubset(x, N, P,
                                        INTEGER(weights), XLENGTH(weights) > 0, REAL(subset),
                                        offset, Nsubset, P_ans);
        }
    } else {
        if (TYPEOF(subset) == INTSXP) {
            C_OneTableSums_dweights_isubset(x, N, P,
                                        REAL(weights), XLENGTH(weights) > 0, INTEGER(subset),
                                        offset, Nsubset, P_ans);
        } else {
            C_OneTableSums_dweights_dsubset(x, N, P,
                                        REAL(weights), XLENGTH(weights) > 0, REAL(subset),
                                        offset, Nsubset, P_ans);
        }
    }
}
@|RC_OneTableSums
@}

@d C OneTableSums Input
@{
@<C integer x Input@>
@}

@d C OneTableSums Answer
@{
double *P_ans
@}

@d C_OneTableSums_dweights_dsubset
@{
void C_OneTableSums_dweights_dsubset
(
    @<C OneTableSums Input@>
    @<C real weights Input@>
    @<C real subset Input@>,
    @<C OneTableSums Answer@>
) {
    double *s, *w;
    @<OneTableSums Body@>
}
@|C_OneTableSums_dweights_dsubset
@}

@d C_OneTableSums_iweights_dsubset
@{
void C_OneTableSums_iweights_dsubset
(
    @<C OneTableSums Input@>
    @<C integer weights Input@>
    @<C real subset Input@>,
    @<C OneTableSums Answer@>
) {
    double *s;
    int *w;
    @<OneTableSums Body@>
}
@|C_OneTableSums_iweights_dsubset
@}

@d C_OneTableSums_iweights_isubset
@{
void C_OneTableSums_iweights_isubset
(
    @<C OneTableSums Input@>
    @<C integer weights Input@>
    @<C integer subset Input@>,
    @<C OneTableSums Answer@>
) {
    int *s, *w;
    @<OneTableSums Body@>
}
@|C_OneTableSums_iweights_isubset
@}

@d C_OneTableSums_dweights_isubset
@{
void C_OneTableSums_dweights_isubset
(
    @<C OneTableSums Input@>
    @<C real weights Input@>
    @<C integer subset Input@>,
    @<C OneTableSums Answer@>
) {
    int *s;
    double *w;
    @<OneTableSums Body@>
}
@|C_OneTableSums_dweights_isubset
@}

@d OneTableSums Body
@{
int *xx;

for (int p = 0; p < P; p++) P_ans[p] = 0.0;

xx = x;
@<init subset loop@>
@<start subset loop@>
{
    xx = xx + diff;
    if (HAS_WEIGHTS) {
        w = w + diff;
        P_ans[xx[0]] += (double) w[0];
    } else {
        P_ans[xx[0]]++;
    }
    @<continue subset loop@>
}
xx = xx + diff;
if (HAS_WEIGHTS) {
    w = w + diff;
    P_ans[xx[0]] += w[0];
} else {
    P_ans[xx[0]]++;
}
@}

\subsubsection{TwoTable Sums}

<<TwoTableSum>>=
a0 <- xtabs(weights ~ ixf + iyf, subset = subset)
class(a0) <- "matrix"
dimnames(a0) <- NULL
attributes(a0)$call <- NULL
a1 <- ctabs(ix, iy, weights = weights, subset = subset)[-1, -1]
a2 <- ctabs(ix, iy, weights = as.double(weights),
            subset = as.double(subset))[-1, -1]
a3 <- ctabs(ix, iy, weights = weights, subset = as.double(subset))[-1, -1]
a4 <- ctabs(ix, iy, weights = as.double(weights), subset = subset)[-1, -1]

stopifnot(isequal(a0, a1) && isequal(a0, a2) &&
          isequal(a0, a3) && isequal(a0, a4))
@@

@d R_TwoTableSums Prototype
@{
SEXP R_TwoTableSums
(
    @<R x Input@>
    @<R y Input@>
    @<R weights Input@>,
    @<R subset Input@>
)
@}

@d R_TwoTableSums
@{
@<R_TwoTableSums Prototype@>
{
    SEXP ans, dim;
    @<C integer N Input@>;
    @<C integer Nsubset Input@>;
    int P, Q;

    N = XLENGTH(x);
    Nsubset = XLENGTH(subset);
    P = NLEVELS(x) + 1;
    Q = NLEVELS(y) + 1;

    PROTECT(ans = allocVector(REALSXP, mPQB(P, Q, 1)));
    PROTECT(dim = allocVector(INTSXP, 2));
    INTEGER(dim)[0] = P;
    INTEGER(dim)[1] = Q;
    dimgets(ans, dim);
    RC_TwoTableSums(INTEGER(x), N, P, INTEGER(y), Q,
                    weights, subset, Offset0, Nsubset, REAL(ans));
    UNPROTECT(2);
    return(ans);
}
@|R_TwoTableSums
@}


@d RC_TwoTableSums Prototype
@{
void RC_TwoTableSums
(
    @<C TwoTableSums Input@>
    @<R weights Input@>,
    @<R subset Input@>,
    @<C subset range Input@>,
    @<C TwoTableSums Answer@>
)
@}

@d RC_TwoTableSums
@{
@<RC_TwoTableSums Prototype@>
{
    if (TYPEOF(weights) == INTSXP) {
        if (TYPEOF(subset) == INTSXP) {
            C_TwoTableSums_iweights_isubset(x, N, P, y, Q,
                                        INTEGER(weights), XLENGTH(weights) > 0, INTEGER(subset),
                                        offset, Nsubset, PQ_ans);
        } else {
            C_TwoTableSums_iweights_dsubset(x, N, P, y, Q,
                                        INTEGER(weights), XLENGTH(weights) > 0, REAL(subset),
                                        offset, Nsubset, PQ_ans);
        }
    } else {
        if (TYPEOF(subset) == INTSXP) {
            C_TwoTableSums_dweights_isubset(x, N, P, y, Q,
                                        REAL(weights), XLENGTH(weights) > 0, INTEGER(subset),
                                        offset, Nsubset, PQ_ans);
        } else {
            C_TwoTableSums_dweights_dsubset(x, N, P, y, Q,
                                        REAL(weights), XLENGTH(weights) > 0, REAL(subset),
                                        offset, Nsubset, PQ_ans);
        }
    }
}
@|RC_TwoTableSums
@}

@d C TwoTableSums Input
@{
@<C integer x Input@>
@<C integer y Input@>
@}

@d C TwoTableSums Answer
@{
double *PQ_ans
@}

@d C_TwoTableSums_dweights_dsubset
@{
void C_TwoTableSums_dweights_dsubset
(
    @<C TwoTableSums Input@>
    @<C real weights Input@>
    @<C real subset Input@>,
    @<C TwoTableSums Answer@>
) {
    double *s, *w;
    @<TwoTableSums Body@>
}
@|C_TwoTableSums_dweights_dsubset
@}

@d C_TwoTableSums_iweights_dsubset
@{
void C_TwoTableSums_iweights_dsubset
(
    @<C TwoTableSums Input@>
    @<C integer weights Input@>
    @<C real subset Input@>,
    @<C TwoTableSums Answer@>
) {
    double *s;
    int *w;
    @<TwoTableSums Body@>
}
@|C_TwoTableSums_iweights_dsubset
@}

@d C_TwoTableSums_iweights_isubset
@{
void C_TwoTableSums_iweights_isubset
(
    @<C TwoTableSums Input@>
    @<C integer weights Input@>
    @<C integer subset Input@>,
    @<C TwoTableSums Answer@>
) {
    int *s, *w;
    @<TwoTableSums Body@>
}
@|C_TwoTableSums_iweights_isubset
@}

@d C_TwoTableSums_dweights_isubset
@{
void C_TwoTableSums_dweights_isubset
(
    @<C TwoTableSums Input@>
    @<C real weights Input@>
    @<C integer subset Input@>,
    @<C TwoTableSums Answer@>
) {
    int *s;
    double *w;
    @<TwoTableSums Body@>
}
@|C_TwoTableSums_dweights_isubset
@}

@d TwoTableSums Body
@{
int *xx, *yy;

for (int p = 0; p < Q * P; p++) PQ_ans[p] = 0.0;

yy = y;
xx = x;
@<init subset loop@>
@<start subset loop@>
{
    xx = xx + diff;
    yy = yy + diff;
    if (HAS_WEIGHTS) {
        w = w + diff;
        PQ_ans[yy[0] * P + xx[0]] += (double) w[0];
    } else {
        PQ_ans[yy[0] * P + xx[0]]++;
    }
    @<continue subset loop@>
}
xx = xx + diff;
yy = yy + diff;
if (HAS_WEIGHTS) {
    w = w + diff;
    PQ_ans[yy[0] * P + xx[0]] += w[0];
} else {
    PQ_ans[yy[0] * P + xx[0]]++;
}
@}

\subsubsection{ThreeTable Sums}

<<ThreeTableSum>>=
a0 <- xtabs(weights ~ ixf + iyf + block, subset = subset)
class(a0) <- "array"
dimnames(a0) <- NULL
attributes(a0)$call <- NULL
a1 <- ctabs(ix, iy, block, weights, subset)[-1, -1,]
a2 <- ctabs(ix, iy, block, as.double(weights), as.double(subset))[-1,-1,]
a3 <- ctabs(ix, iy, block, weights, as.double(subset))[-1,-1,]
a4 <- ctabs(ix, iy, block, as.double(weights), subset)[-1,-1,]

stopifnot(isequal(a0, a1) && isequal(a0, a2) &&
          isequal(a0, a3) && isequal(a0, a4))
@@

@d R_ThreeTableSums Prototype
@{
SEXP R_ThreeTableSums
(
    @<R x Input@>
    @<R y Input@>
    @<R block Input@>,
    @<R weights Input@>,
    @<R subset Input@>
)
@}

@d R_ThreeTableSums
@{
@<R_ThreeTableSums Prototype@>
{
    SEXP ans, dim;
    @<C integer N Input@>;
    @<C integer Nsubset Input@>;
    int P, Q, B;

    N = XLENGTH(x);
    Nsubset = XLENGTH(subset);
    P = NLEVELS(x) + 1;
    Q = NLEVELS(y) + 1;
    B = NLEVELS(block);

    PROTECT(ans = allocVector(REALSXP, mPQB(P, Q, B)));
    PROTECT(dim = allocVector(INTSXP, 3));
    INTEGER(dim)[0] = P;
    INTEGER(dim)[1] = Q;
    INTEGER(dim)[2] = B;
    dimgets(ans, dim);
    RC_ThreeTableSums(INTEGER(x), N, P, INTEGER(y), Q,
                      INTEGER(block), B,
                      weights, subset, Offset0, Nsubset, REAL(ans));
    UNPROTECT(2);
    return(ans);
}
@|R_ThreeTableSums
@}

@d RC_ThreeTableSums Prototype
@{
void RC_ThreeTableSums
(
    @<C ThreeTableSums Input@>
    @<R weights Input@>,
    @<R subset Input@>,
    @<C subset range Input@>,
    @<C ThreeTableSums Answer@>
)
@}

@d RC_ThreeTableSums
@{
@<RC_ThreeTableSums Prototype@>
{
    if (TYPEOF(weights) == INTSXP) {
        if (TYPEOF(subset) == INTSXP) {
            C_ThreeTableSums_iweights_isubset(x, N, P, y, Q, block, B,
                                        INTEGER(weights), XLENGTH(weights) > 0, INTEGER(subset),
                                        offset, Nsubset, PQL_ans);
        } else {
            C_ThreeTableSums_iweights_dsubset(x, N, P, y, Q, block, B,
                                        INTEGER(weights), XLENGTH(weights) > 0, REAL(subset),
                                        offset, Nsubset, PQL_ans);
        }
    } else {
        if (TYPEOF(subset) == INTSXP) {
            C_ThreeTableSums_dweights_isubset(x, N, P, y, Q, block, B,
                                        REAL(weights), XLENGTH(weights) > 0, INTEGER(subset),
                                        offset, Nsubset, PQL_ans);
        } else {
            C_ThreeTableSums_dweights_dsubset(x, N, P, y, Q, block, B,
                                        REAL(weights), XLENGTH(weights) > 0, REAL(subset),
                                        offset, Nsubset, PQL_ans);
        }
    }
}
@|RC_ThreeTableSums
@}

@d C ThreeTableSums Input
@{
@<C integer x Input@>
@<C integer y Input@>
@<C integer block Input@>
@}

@d C ThreeTableSums Answer
@{
double *PQL_ans
@}

@d C_ThreeTableSums_dweights_dsubset
@{
void C_ThreeTableSums_dweights_dsubset
(
    @<C ThreeTableSums Input@>
    @<C real weights Input@>
    @<C real subset Input@>,
    @<C ThreeTableSums Answer@>
) {
    double *s, *w;
    @<ThreeTableSums Body@>
}
@|C_ThreeTableSums_dweights_dsubset
@}

@d C_ThreeTableSums_iweights_dsubset
@{
void C_ThreeTableSums_iweights_dsubset
(
    @<C ThreeTableSums Input@>
    @<C integer weights Input@>
    @<C real subset Input@>,
    @<C ThreeTableSums Answer@>
) {
    double *s;
    int *w;
    @<ThreeTableSums Body@>
}
@|C_ThreeTableSums_iweights_dsubset
@}

@d C_ThreeTableSums_iweights_isubset
@{
void C_ThreeTableSums_iweights_isubset
(
    @<C ThreeTableSums Input@>
    @<C integer weights Input@>
    @<C integer subset Input@>,
    @<C ThreeTableSums Answer@>
) {
    int *s, *w;
    @<ThreeTableSums Body@>
}
@|C_ThreeTableSums_iweights_isubset
@}

@d C_ThreeTableSums_dweights_isubset
@{
void C_ThreeTableSums_dweights_isubset
(
    @<C ThreeTableSums Input@>
    @<C real weights Input@>
    @<C integer subset Input@>,
    @<C ThreeTableSums Answer@>
) {
    int *s;
    double *w;
    @<ThreeTableSums Body@>
}
@|C_ThreeTableSums_dweights_isubset
@}

@d ThreeTableSums Body
@{
int *xx, *yy, *bb, PQ = mPQB(P, Q, 1);

for (int p = 0; p < PQ * B; p++) PQL_ans[p] = 0.0;

yy = y;
xx = x;
bb = block;
@<init subset loop@>
@<start subset loop@>
{
    xx = xx + diff;
    yy = yy + diff;
    bb = bb + diff;
    if (HAS_WEIGHTS) {
        w = w + diff;
        PQL_ans[(bb[0] - 1) * PQ + yy[0] * P + xx[0]] += (double) w[0];
    } else {
        PQL_ans[(bb[0] - 1) * PQ + yy[0] * P + xx[0]]++;
    }
    @<continue subset loop@>
}
xx = xx + diff;
yy = yy + diff;
bb = bb + diff;
if (HAS_WEIGHTS) {
    w = w + diff;
    PQL_ans[(bb[0] - 1) * PQ + yy[0] * P + xx[0]] += w[0];
} else {
    PQL_ans[(bb[0] - 1) * PQ + yy[0] * P + xx[0]]++;
}
@}

\section{Utilities}

\subsection{Blocks}

<<blocks>>=
sb <- sample(block)

ns1 <- do.call(c, tapply(subset, sb[subset], function(i) i))
ns2 <- .Call(libcoin:::R_order_subset_wrt_block, y, integer(0), subset, sb)

stopifnot(isequal(ns1, ns2))
@@

@d Utils
@{
@<C_setup_subset@>
@<C_setup_subset_block@>
@<C_order_subset_wrt_block@>
@<RC_order_subset_wrt_block@>
@<R_order_subset_wrt_block@>
@}

@d R_order_subset_wrt_block Prototype
@{
SEXP R_order_subset_wrt_block
(
    @<R y Input@>
    @<R weights Input@>,
    @<R subset Input@>,
    @<R block Input@>
)
@}

@d R_order_subset_wrt_block
@{
@<R_order_subset_wrt_block Prototype@>
{
    @<C integer N Input@>;
    SEXP blockTable, ans;

    N = XLENGTH(y) / NCOL(y);

    if (XLENGTH(weights) > 0)
        error("cannot deal with weights here");

    if (NLEVELS(block) > 1) {
        PROTECT(blockTable = R_OneTableSums(block, weights, subset));
    } else {
        PROTECT(blockTable = allocVector(REALSXP, 2));
        REAL(blockTable)[0] = 0.0;
        REAL(blockTable)[1] = RC_Sums(N, weights, subset, Offset0, XLENGTH(subset));
    }

    PROTECT(ans = RC_order_subset_wrt_block(N, subset, block, blockTable));

    UNPROTECT(2);
    return(ans);
}
@|R_order_subset_wrt_block
@}

@d RC_order_subset_wrt_block Prototype
@{
SEXP RC_order_subset_wrt_block
(
    @<C integer N Input@>,
    @<R subset Input@>,
    @<R block Input@>,
    @<R blockTable Input@>
)
@}

@d RC_order_subset_wrt_block
@{
@<RC_order_subset_wrt_block Prototype@>
{
    SEXP ans;
    int NOBLOCK = (XLENGTH(block) == 0 || XLENGTH(blockTable) == 2);

    if (XLENGTH(subset) > 0) {
        if (NOBLOCK) {
            return(subset);
        } else {
            PROTECT(ans = allocVector(TYPEOF(subset), XLENGTH(subset)));
            C_order_subset_wrt_block(subset, block, blockTable, ans);
            UNPROTECT(1);
            return(ans);
        }
    } else {
        PROTECT(ans = allocVector(TYPEOF(subset), N));
        if (NOBLOCK) {
            C_setup_subset(N, ans);
        } else {
            C_setup_subset_block(N, block, blockTable, ans);
        }
        UNPROTECT(1);
        return(ans);
    }
}
@|RC_order_subset_wrt_block
@}

@d C_setup_subset
@{
void C_setup_subset
(
    @<C integer N Input@>,
    SEXP ans
) {
    for (R_xlen_t i = 0; i < N; i++) {
        /* ans is R style index in 1:N */
        if (TYPEOF(ans) == INTSXP) {
            INTEGER(ans)[i] = i + 1;
        } else {
            REAL(ans)[i] = (double) i + 1;
        }
    }
}
@|C_setup_subset
@}

@d C_setup_subset_block
@{
void C_setup_subset_block
(
    @<C integer N Input@>,
    @<R block Input@>,
    @<R blockTable Input@>,
    SEXP ans
) {
    double *cumtable;
    int Nlevels = LENGTH(blockTable);

    cumtable = R_Calloc(Nlevels, double);
    for (int k = 0; k < Nlevels; k++) cumtable[k] = 0.0;

    /* table[0] are missings, ie block == 0 ! */
    for (int k = 1; k < Nlevels; k++)
        cumtable[k] = cumtable[k - 1] + REAL(blockTable)[k - 1];

    for (R_xlen_t i = 0; i < N; i++) {
        /* ans is R style index in 1:N */
        if (TYPEOF(ans) == INTSXP) {
            INTEGER(ans)[(int) cumtable[INTEGER(block)[i]]++] = i + 1;
        } else {
            REAL(ans)[(R_xlen_t) cumtable[INTEGER(block)[i]]++] = (double) i + 1;
        }
    }

    R_Free(cumtable);
}
@|C_setup_subset_block
@}

@d C_order_subset_wrt_block
@{
void C_order_subset_wrt_block
(
    @<R subset Input@>,
    @<R block Input@>,
    @<R blockTable Input@>,
    SEXP ans
) {
    double *cumtable;
    int Nlevels = LENGTH(blockTable);

    cumtable = R_Calloc(Nlevels, double);
    for (int k = 0; k < Nlevels; k++) cumtable[k] = 0.0;

    /* table[0] are missings, ie block == 0 ! */
    for (int k = 1; k < Nlevels; k++)
        cumtable[k] = cumtable[k - 1] + REAL(blockTable)[k - 1];

    /* subset is R style index in 1:N */
    if (TYPEOF(subset) == INTSXP) {
        for (R_xlen_t i = 0; i < XLENGTH(subset); i++)
            INTEGER(ans)[(int) cumtable[INTEGER(block)[INTEGER(subset)[i] - 1]]++] = INTEGER(subset)[i];
    } else {
        for (R_xlen_t i = 0; i < XLENGTH(subset); i++)
            REAL(ans)[(R_xlen_t) cumtable[INTEGER(block)[(R_xlen_t) REAL(subset)[i] - 1]]++] = REAL(subset)[i];
    }

    R_Free(cumtable);
}
@|C_order_subset_wrt_block
@}

@d RC_setup_subset Prototype
@{
SEXP RC_setup_subset
(
    @<C integer N Input@>,
    @<R weights Input@>,
    @<R subset Input@>
)
@}

Because this will only be used when really needed (in Permutations) we can
be a little bit more generous with memory here. The return value is always
\code{REALSXP}.

@d RC_setup_subset
@{
@<RC_setup_subset Prototype@>
{
    SEXP ans, mysubset;
    R_xlen_t sumweights;

    if (XLENGTH(subset) == 0) {
        PROTECT(mysubset = allocVector(REALSXP, N));
        C_setup_subset(N, mysubset);
    } else {
        PROTECT(mysubset = coerceVector(subset, REALSXP));
    }

    if (XLENGTH(weights) == 0) {
        UNPROTECT(1);
        return(mysubset);
    }

    sumweights = (R_xlen_t) RC_Sums(N, weights, mysubset, Offset0, XLENGTH(subset));
    PROTECT(ans = allocVector(REALSXP, sumweights));

    R_xlen_t itmp = 0;
    for (R_xlen_t i = 0; i < XLENGTH(mysubset); i++) {
        if (TYPEOF(weights) == REALSXP) {
            for (R_xlen_t j = 0; j < REAL(weights)[(R_xlen_t) REAL(mysubset)[i] - 1]; j++)
                REAL(ans)[itmp++] = REAL(mysubset)[i];
        } else {
            for (R_xlen_t j = 0; j < INTEGER(weights)[(R_xlen_t) REAL(mysubset)[i] - 1]; j++)
                REAL(ans)[itmp++] = REAL(mysubset)[i];
        }
    }
    UNPROTECT(2);
    return(ans);
}
@|RC_setup_subset
@}

\subsection{Permutation Helpers}

@d Permutations
@{
@<RC_setup_subset@>
@<C_Permute@>
@<C_doPermute@>
@<C_PermuteBlock@>
@<C_doPermuteBlock@>
@}

@d C_Permute
@{
void C_Permute
(
    double *subset,
    @<C integer Nsubset Input@>,
    double *ans
) {
    R_xlen_t n = Nsubset, j;

    for (R_xlen_t i = 0; i < Nsubset; i++) {
        j = n * unif_rand();
        ans[i] = subset[j];
        subset[j] = subset[--n];
    }
}
@|C_Permute
@}

@d C_doPermute
@{
void C_doPermute
(
    double *subset,
    @<C integer Nsubset Input@>,
    double *Nsubset_tmp,
    double *perm
) {
    Memcpy(Nsubset_tmp, subset, Nsubset);
    C_Permute(Nsubset_tmp, Nsubset, perm);
}
@|C_doPermute
@}

@d C_PermuteBlock
@{
void C_PermuteBlock
(
    double *subset,
    double *table,
    int Nlevels,
    double *ans
) {
    double *px, *pans;

    px = subset;
    pans = ans;

    for (R_xlen_t j = 0; j < Nlevels; j++) {
        if (table[j] > 0) {
            C_Permute(px, (R_xlen_t) table[j], pans);
            px += (R_xlen_t) table[j];
            pans += (R_xlen_t) table[j];
        }
    }
}
@|C_PermuteBlock
@}

@d C_doPermuteBlock
@{
void C_doPermuteBlock
(
    double *subset,
    @<C integer Nsubset Input@>,
    double *table,
    int Nlevels,
    double *Nsubset_tmp,
    double *perm
) {
    Memcpy(Nsubset_tmp, subset, Nsubset);
    C_PermuteBlock(Nsubset_tmp, table, Nlevels, perm);
}
@|C_doPermuteBlock
@}

\subsection{Other Utils}

@d MoreUtils
@{
@<NROW@>
@<NCOL@>
@<NLEVELS@>
@<C_kronecker@>
@<R_kronecker@>
@<C_kronecker_sym@>
@<C_KronSums_sym@>
@<C_MPinv_sym@>
@<R_MPinv_sym@>
@<R_unpack_sym@>
@<R_pack_sym@>
@}

@d NROW
@{
int NROW
(
    SEXP x
) {
    SEXP a;
    a = getAttrib(x, R_DimSymbol);
    if (a == R_NilValue) return(XLENGTH(x));
    if (TYPEOF(a) == REALSXP)
        return(REAL(a)[0]);
    return(INTEGER(a)[0]);
}
@|NROW
@}

@d NCOL
@{
int NCOL
(
    SEXP x
) {
    SEXP a;
    a = getAttrib(x, R_DimSymbol);
    if (a == R_NilValue) return(1);
    if (TYPEOF(a) == REALSXP)
        return(REAL(a)[1]);
    return(INTEGER(a)[1]);
}
@|NCOL
@}

@d NLEVELS
@{
int NLEVELS
(
    SEXP x
) {
    SEXP a;
    int maxlev = 0;

    a = getAttrib(x, R_LevelsSymbol);
    if (a == R_NilValue) {
        if (TYPEOF(x) != INTSXP)
            error("cannot determine number of levels");
        for (R_xlen_t i = 0; i < XLENGTH(x); i++) {
            if (INTEGER(x)[i] > maxlev)
                maxlev = INTEGER(x)[i];
        }
        return(maxlev);
    }
    return(NROW(a));
}
@|NLEVELS
@}

Check for integer overflow when computing $P (P + 1) / 2$ and $P Q$.

@d PP12
@{
int PP12
(
    int P
) {
    double dP = (double) P;
    double ans;

    ans = dP * (dP + 1) / 2;

    if (ans > INT_MAX)
        error("cannot allocate memory: number of levels too large");

    return((int) ans);
}
@|PP12
@}

@d mPQB
@{
int mPQB
(
    int P,
    int Q,
    int B
) {
    double ans = P * Q * B;

    if (ans > INT_MAX)
        error("cannot allocate memory: number of levels too large");

    return((int) ans);
}
@|mPQB
@}

<<kronecker>>=
A <- matrix(runif(12), ncol = 3)
B <- matrix(runif(10), ncol = 2)

K1 <- kronecker(A, B)
K2 <- .Call(libcoin:::R_kronecker, A, B)

stopifnot(isequal(K1, K2))
@@

@d R_kronecker Prototype
@{
SEXP R_kronecker
(
    SEXP A,
    SEXP B
)
@}

This function can be called from other packages.

@o libcoinAPI.h -cc
@{
extern SEXP libcoin_R_kronecker(
    SEXP A, SEXP B
) {
    static SEXP(*fun)(SEXP, SEXP) = NULL;
    if (fun == NULL)
        fun = (SEXP(*)(SEXP, SEXP))
            R_GetCCallable("libcoin", "R_kronecker");
    return fun(A, B);
}
@}

@d R_kronecker
@{
@<R_kronecker Prototype@>
{
    int m, n, r, s;
    SEXP ans;

    if (!isReal(A) || !isReal(B))
        error("R_kronecker: A and / or B are not of type REALSXP");

    m = NROW(A);
    n = NCOL(A);
    r = NROW(B);
    s = NCOL(B);

    PROTECT(ans = allocMatrix(REALSXP, m * n, r * s));
    C_kronecker(REAL(A), m, n, REAL(B), r, s, 1, REAL(ans));
    UNPROTECT(1);
    return(ans);
}
@}

@d C_kronecker
@{
void C_kronecker
(
    const double *A,
    const int m,
    const int n,
    const double *B,
    const int r,
    const int s,
    const int overwrite,
    double *ans
) {
    int mr, js, ir;
    double y;

    if (overwrite) {
        for (int i = 0; i < m * r * n * s; i++) ans[i] = 0.0;
    }

    mr = m * r;
    for (int i = 0; i < m; i++) {
        ir = i * r;
        for (int j = 0; j < n; j++) {
            js = j * s;
            y = A[j*m + i];
            for (int k = 0; k < r; k++) {
                for (int l = 0; l < s; l++)
                    ans[(js + l) * mr + ir + k] += y * B[l * r + k];
            }
        }
    }
}
@|C_kronecker
@}

@d C_kronecker_sym
@{
void C_kronecker_sym
(
    const double *A,
    const int m,
    const double *B,
    const int r,
    const int overwrite,
    double *ans
) {
    int mr, js, ir, s;
    double y;

    mr = m * r;
    s = r;

    if (overwrite) {
        for (int i = 0; i < mr * (mr + 1) / 2; i++) ans[i] = 0.0;
    }

    for (int i = 0; i < m; i++) {
        ir = i * r;
        for (int j = 0; j <= i; j++) {
            js = j * s;
            y = A[S(i, j, m)];
            for (int k = 0; k < r; k++) {
                for (int l = 0; l < (j < i ? s : k + 1); l++) {
                    ans[S(ir + k, js + l, mr)] += y * B[S(k, l, r)];
                }
            }
        }
    }
}
@|C_kronecker_sym
@}

@d C_KronSums_sym
@{
/* sum_i (t(x[i,]) %*% x[i,]) */
void C_KronSums_sym_
(
    @<C real x Input@>
    double *PP_sym_ans
) {
    int pN, qN, SpqP;

    for (int q = 0; q < P; q++) {
        qN = q * N;
        for (int p = 0; p <= q; p++) {
            PP_sym_ans[S(p, q, P)] = 0.0;
            pN = p * N;
            SpqP = S(p, q, P);
            for (int i = 0; i < N; i++)
                 PP_sym_ans[SpqP] += x[qN + i] * x[pN + i];
        }
    }
}
@|C_KronSums_sym
@}

<<MPinv>>=
covar <- vcov(ls1)
covar_sym <- ls1$Covariance
n <- (sqrt(1 + 8 * length(covar_sym)) - 1) / 2

tol <- sqrt(.Machine$double.eps)
MP1 <- MPinverse(covar, tol)
MP2 <- .Call(libcoin:::R_MPinv_sym, covar_sym, as.integer(n), tol)

lt <- lower.tri(covar, diag = TRUE)
stopifnot(isequal(MP1$MPinv[lt], MP2$MPinv) &&
          isequal(MP1$rank, MP2$rank))
@@

@d R_MPinv_sym Prototype
@{
SEXP R_MPinv_sym
(
    SEXP x,
    SEXP n,
    SEXP tol
)
@}

This function can be called from other packages.

@o libcoinAPI.h -cc
@{
extern SEXP libcoin_R_MPinv_sym(
    SEXP x, SEXP n, SEXP tol
) {
    static SEXP(*fun)(SEXP, SEXP, SEXP) = NULL;
    if (fun == NULL)
        fun = (SEXP(*)(SEXP, SEXP, SEXP))
            R_GetCCallable("libcoin", "R_MPinv_sym");
    return fun(x, n, tol);
}
@}

@d R_MPinv_sym
@{
@<R_MPinv_sym Prototype@>
{
    int m;
    SEXP ans, names, MPinv, rank;

    m = INTEGER(n)[0];
    if (m == 0)
        m = (int) (sqrt(0.25 + 2 * LENGTH(x)) - 0.5);

    PROTECT(ans = allocVector(VECSXP, 2));
    PROTECT(names = allocVector(STRSXP, 2));
    SET_VECTOR_ELT(ans, 0, MPinv = allocVector(REALSXP, LENGTH(x)));
    SET_STRING_ELT(names, 0, mkChar("MPinv"));
    SET_VECTOR_ELT(ans, 1, rank = allocVector(INTSXP, 1));
    SET_STRING_ELT(names, 1, mkChar("rank"));
    namesgets(ans, names);

    C_MPinv_sym(REAL(x), m, REAL(tol)[0], REAL(MPinv), INTEGER(rank));

    UNPROTECT(2);
    return(ans);
}
@|R_MPinv_sym
@}

@d C_MPinv_sym
@{
void C_MPinv_sym
(
    const double *x,
    const int n,
    const double tol,
    double *dMP,
    int *rank
) {
    double *val, *vec, dtol, *rx, *work, valinv;
    int valzero = 0, info = 0, kn;

    if (n == 1) {
        if (x[0] > tol) {
            dMP[0] = 1 / x[0];
            rank[0] = 1;
        } else {
            dMP[0] = 0;
            rank[0] = 0;
        }
    } else {
        rx = R_Calloc(n * (n + 1) / 2, double);
        Memcpy(rx, x, n * (n + 1) / 2);
        work = R_Calloc(3 * n, double);
        val = R_Calloc(n, double);
        vec = R_Calloc(n * n, double);

        F77_CALL(dspev)("V", "L", &n, rx, val, vec, &n, work,
                        &info FCONE FCONE);

        dtol = val[n - 1] * tol;

        for (int k = 0; k < n; k++)
            valzero += (val[k] < dtol);
        rank[0] = n - valzero;

        for (int k = 0; k < n * (n + 1) / 2; k++) dMP[k] = 0.0;

        for (int k = valzero; k < n; k++) {
            valinv = 1 / val[k];
            kn = k * n;
            for (int i = 0; i < n; i++) {
                for (int j = 0; j <= i; j++) {
                    /* MP is symmetric */
                    dMP[S(i, j, n)] += valinv * vec[kn + i] * vec[kn + j];
                }
            }
        }
        R_Free(rx); R_Free(work); R_Free(val); R_Free(vec);
    }
}
@}

<<unpack>>=
m <- matrix(c(3, 2, 1,
              2, 4, 2,
              1, 2, 5),
            ncol = 3)

s <- m[lower.tri(m, diag = TRUE)]
u1 <- .Call(libcoin:::R_unpack_sym, s, NULL, 0L)
u2 <- .Call(libcoin:::R_unpack_sym, s, NULL, 1L)

stopifnot(isequal(m, u1) && isequal(diag(m), u2))
@@

@d R_unpack_sym Prototype
@{
SEXP R_unpack_sym
(
    SEXP x,
    SEXP names,
    SEXP diagonly
)
@}

This function can be called from other packages.

@o libcoinAPI.h -cc
@{
extern SEXP libcoin_R_unpack_sym(
    SEXP x, SEXP names, SEXP diagonly
) {
    static SEXP(*fun)(SEXP, SEXP, SEXP) = NULL;
    if (fun == NULL)
        fun = (SEXP(*)(SEXP, SEXP, SEXP))
            R_GetCCallable("libcoin", "R_unpack_sym");
    return fun(x, names, diagonly);
}
@}

@d R_unpack_sym
@{
@<R_unpack_sym Prototype@>
{
    R_xlen_t n, k = 0;
    SEXP ans, dimnames;
    double *dx, *dans;

    /* m = n * (n + 1)/2 <=> n^2 + n - 2 * m = 0 */
    n = sqrt(0.25 + 2 * XLENGTH(x)) - 0.5;

    dx = REAL(x);
    if (INTEGER(diagonly)[0]) {
        PROTECT(ans = allocVector(REALSXP, n));
        if (names != R_NilValue) {
            namesgets(ans, names);
        }
        dans = REAL(ans);
        for (R_xlen_t i = 0; i < n; i++) {
            dans[i] = dx[k];
            k += n - i;
        }
    } else {
        PROTECT(ans = allocMatrix(REALSXP, n, n));
        if (names != R_NilValue) {
            PROTECT(dimnames = allocVector(VECSXP, 2));
            SET_VECTOR_ELT(dimnames, 0, names);
            SET_VECTOR_ELT(dimnames, 1, names);
            dimnamesgets(ans, dimnames);
            UNPROTECT(1);
        }
        dans = REAL(ans);
        for (R_xlen_t i = 0; i < n; i++) {
            dans[i * n + i] = dx[k];     /* diagonal */
            k++;
            for (R_xlen_t j = i + 1; j < n; j++) {
                dans[i * n + j] = dx[k]; /* lower triangular */
                dans[j * n + i] = dx[k]; /* upper triangular */
                k++;
            }
        }
    }

    UNPROTECT(1);
    return ans;
}
@|R_unpack_sym
@}

<<pack>>=
m <- matrix(c(4, 3, 2, 1,
              3, 5, 4, 2,
              2, 4, 6, 5,
              1, 2, 5, 7),
            ncol = 4)

s <- m[lower.tri(m, diag = TRUE)]
p <- .Call(libcoin:::R_pack_sym, m)

stopifnot(isequal(s, p))
@@

@d R_pack_sym Prototype
@{
SEXP R_pack_sym
(
    SEXP x
)
@}

This function can be called from other packages.

@o libcoinAPI.h -cc
@{
extern SEXP libcoin_R_pack_sym(
    SEXP x
) {
    static SEXP(*fun)(SEXP) = NULL;
    if (fun == NULL)
        fun = (SEXP(*)(SEXP))
            R_GetCCallable("libcoin", "R_pack_sym");
    return fun(x);
}
@}

@d R_pack_sym
@{
@<R_pack_sym Prototype@>
{
    R_xlen_t n, k = 0;
    SEXP ans;
    double *dx, *dans;

    n = NROW(x);
    dx = REAL(x);
    PROTECT(ans = allocVector(REALSXP, n * (n + 1) / 2));
    dans = REAL(ans);

    for (R_xlen_t i = 0; i < n; i++) {
        for (R_xlen_t j = i; j < n; j++) {
          dans[k] = dx[i * n + j];
          k++;
        }
    }

    UNPROTECT(1);
    return ans;
}
@|R_pack_sym
@}

\section{Memory}

@d Memory
@{
@<C_get_P@>
@<C_get_Q@>
@<PP12@>
@<mPQB@>
@<C_get_varonly@>
@<C_get_Xfactor@>
@<C_get_LinearStatistic@>
@<C_get_Expectation@>
@<C_get_Variance@>
@<C_get_Covariance@>
@<C_get_ExpectationX@>
@<C_get_ExpectationInfluence@>
@<C_get_CovarianceInfluence@>
@<C_get_VarianceInfluence@>
@<C_get_TableBlock@>
@<C_get_Sumweights@>
@<C_get_Table@>
@<C_get_dimTable@>
@<C_get_B@>
@<C_get_nresample@>
@<C_get_PermutedLinearStatistic@>
@<C_get_tol@>
@<RC_init_LECV_1d@>
@<RC_init_LECV_2d@>
@}

@d R LECV Input
@{
SEXP LECV
@|LECV
@}

@d C_get_P
@{
int C_get_P
(
    @<R LECV Input@>
) {
    return(INTEGER(VECTOR_ELT(LECV, dim_SLOT))[0]);
}
@|C_get_P
@}

@d C_get_Q
@{
int C_get_Q
(
    @<R LECV Input@>
) {
    return(INTEGER(VECTOR_ELT(LECV, dim_SLOT))[1]);
}
@|C_get_Q
@}

@d C_get_varonly
@{
int C_get_varonly
(
    @<R LECV Input@>
) {
    return(INTEGER(VECTOR_ELT(LECV, varonly_SLOT))[0]);
}
@|C_get_varonly
@}

@d C_get_Xfactor
@{
int C_get_Xfactor
(
    @<R LECV Input@>
) {
    return(INTEGER(VECTOR_ELT(LECV, Xfactor_SLOT))[0]);
}
@|C_get_Xfactor
@}

@d C_get_LinearStatistic
@{
double* C_get_LinearStatistic
(
    @<R LECV Input@>
) {
    return(REAL(VECTOR_ELT(LECV, LinearStatistic_SLOT)));
}
@|C_get_LinearStatistic
@}

@d C_get_Expectation
@{
double* C_get_Expectation
(
    @<R LECV Input@>
) {
    return(REAL(VECTOR_ELT(LECV, Expectation_SLOT)));
}
@|C_get_Expectation
@}

@d C_get_Variance
@{
double* C_get_Variance
(
    @<R LECV Input@>
) {
    int PQ = C_get_P(LECV) * C_get_Q(LECV);
    double *var, *covar;

    if (isNull(VECTOR_ELT(LECV, Variance_SLOT))) {
        SET_VECTOR_ELT(LECV, Variance_SLOT,
                       allocVector(REALSXP, PQ));
        if (!isNull(VECTOR_ELT(LECV, Covariance_SLOT))) {
            covar = REAL(VECTOR_ELT(LECV, Covariance_SLOT));
            var = REAL(VECTOR_ELT(LECV, Variance_SLOT));
            for (int p = 0; p < PQ; p++)
                var[p] = covar[S(p, p, PQ)];
        }
    }
    return(REAL(VECTOR_ELT(LECV, Variance_SLOT)));
}
@|C_get_Variance
@}

@d C_get_Covariance
@{
double* C_get_Covariance
(
    @<R LECV Input@>
) {
    int PQ = C_get_P(LECV) * C_get_Q(LECV);
    if (C_get_varonly(LECV) && PQ > 1)
        error("Cannot extract covariance from variance only object");
    if (C_get_varonly(LECV) && PQ == 1)
        return(C_get_Variance(LECV));
    return(REAL(VECTOR_ELT(LECV, Covariance_SLOT)));
}
@|C_get_Covariance
@}

@d C_get_ExpectationX
@{
double* C_get_ExpectationX
(
    @<R LECV Input@>
) {
    return(REAL(VECTOR_ELT(LECV, ExpectationX_SLOT)));
}
@|C_get_ExpectationX
@}

@d C_get_ExpectationInfluence
@{
double* C_get_ExpectationInfluence
(
    @<R LECV Input@>
) {
    return(REAL(VECTOR_ELT(LECV, ExpectationInfluence_SLOT)));
}
@|C_get_ExpectationInfluence
@}

@d C_get_CovarianceInfluence
@{
double* C_get_CovarianceInfluence
(
    @<R LECV Input@>
) {
    return(REAL(VECTOR_ELT(LECV, CovarianceInfluence_SLOT)));
}
@|C_get_CovarianceInfluence
@}

@d C_get_VarianceInfluence
@{
double* C_get_VarianceInfluence
(
    @<R LECV Input@>
) {
    return(REAL(VECTOR_ELT(LECV, VarianceInfluence_SLOT)));
}
@|C_get_VarianceInfluence
@}

@d C_get_TableBlock
@{
double* C_get_TableBlock
(
    @<R LECV Input@>
) {
    if (VECTOR_ELT(LECV, TableBlock_SLOT) == R_NilValue)
        error("object does not contain table block slot");
    return(REAL(VECTOR_ELT(LECV, TableBlock_SLOT)));
}
@|C_get_TableBlock
@}

@d C_get_Sumweights
@{
double* C_get_Sumweights
(
    @<R LECV Input@>
) {
    if (VECTOR_ELT(LECV, Sumweights_SLOT) == R_NilValue)
        error("object does not contain sumweights slot");
    return(REAL(VECTOR_ELT(LECV, Sumweights_SLOT)));
}
@|C_get_Sumweights
@}

@d C_get_Table
@{
double* C_get_Table
(
    @<R LECV Input@>
) {
    if (LENGTH(LECV) <= Table_SLOT)
        error("Cannot extract table from object");
    return(REAL(VECTOR_ELT(LECV, Table_SLOT)));
}
@|C_get_Table
@}

@d C_get_dimTable
@{
int* C_get_dimTable
(
    @<R LECV Input@>
) {
    if (LENGTH(LECV) <= Table_SLOT)
        error("Cannot extract table from object");
    return(INTEGER(getAttrib(VECTOR_ELT(LECV, Table_SLOT),
                             R_DimSymbol)));
}
@|C_get_dimTable
@}

@d C_get_B
@{
int C_get_B
(
    @<R LECV Input@>
) {
    if (VECTOR_ELT(LECV, TableBlock_SLOT) != R_NilValue)
        return(LENGTH(VECTOR_ELT(LECV, Sumweights_SLOT)));
    return(C_get_dimTable(LECV)[2]);
}
@|C_get_B
@}

@d C_get_nresample
@{
R_xlen_t C_get_nresample
(
    @<R LECV Input@>
) {
    int PQ = C_get_P(LECV) * C_get_Q(LECV);
    return(XLENGTH(VECTOR_ELT(LECV, PermutedLinearStatistic_SLOT)) / PQ);
}
@|C_get_nresample
@}

@d C_get_PermutedLinearStatistic
@{
double* C_get_PermutedLinearStatistic
(
    @<R LECV Input@>
) {
    return(REAL(VECTOR_ELT(LECV, PermutedLinearStatistic_SLOT)));
}
@|C_get_PermutedLinearStatistic
@}

@d C_get_tol
@{
double C_get_tol
(
    @<R LECV Input@>
) {
    return(REAL(VECTOR_ELT(LECV, tol_SLOT))[0]);
}
@|C_get_tol
@}

@d Memory Input Checks
@{
if (P <= 0)
    error("P is not positive");

if (Q <= 0)
    error("Q is not positive");

if (B <= 0)
    error("B is not positive");

if (varonly < 0 || varonly > 1)
    error("varonly is not 0 or 1");

if (Xfactor < 0 || Xfactor > 1)
    error("Xfactor is not 0 or 1");

if (tol <= DBL_MIN)
    error("tol is not positive");
@}

@d Memory Names
@{
PROTECT(names = allocVector(STRSXP, Table_SLOT + 1));
SET_STRING_ELT(names, LinearStatistic_SLOT, mkChar("LinearStatistic"));
SET_STRING_ELT(names, Expectation_SLOT, mkChar("Expectation"));
SET_STRING_ELT(names, varonly_SLOT, mkChar("varonly"));
SET_STRING_ELT(names, Variance_SLOT, mkChar("Variance"));
SET_STRING_ELT(names, Covariance_SLOT, mkChar("Covariance"));
SET_STRING_ELT(names, ExpectationX_SLOT, mkChar("ExpectationX"));
SET_STRING_ELT(names, dim_SLOT, mkChar("dimension"));
SET_STRING_ELT(names, ExpectationInfluence_SLOT,
               mkChar("ExpectationInfluence"));
SET_STRING_ELT(names, Xfactor_SLOT, mkChar("Xfactor"));
SET_STRING_ELT(names, CovarianceInfluence_SLOT,
               mkChar("CovarianceInfluence"));
SET_STRING_ELT(names, VarianceInfluence_SLOT,
               mkChar("VarianceInfluence"));
SET_STRING_ELT(names, TableBlock_SLOT, mkChar("TableBlock"));
SET_STRING_ELT(names, Sumweights_SLOT, mkChar("Sumweights"));
SET_STRING_ELT(names, PermutedLinearStatistic_SLOT,
               mkChar("PermutedLinearStatistic"));
SET_STRING_ELT(names, StandardisedPermutedLinearStatistic_SLOT,
               mkChar("StandardisedPermutedLinearStatistic"));
SET_STRING_ELT(names, tol_SLOT, mkChar("tol"));
SET_STRING_ELT(names, Table_SLOT, mkChar("Table"));
@}

@d R_init_LECV
@{
SEXP vo, d, names, tolerance, tmp;
int PQ;

@<Memory Input Checks@>
PQ = mPQB(P, Q, 1);
@<Memory Names@>

/* Table_SLOT is always last and only used in 2d case, ie omitted here */
PROTECT(ans = allocVector(VECSXP, Table_SLOT + 1));
SET_VECTOR_ELT(ans, LinearStatistic_SLOT, allocVector(REALSXP, PQ));
SET_VECTOR_ELT(ans, Expectation_SLOT, allocVector(REALSXP, PQ));
SET_VECTOR_ELT(ans, varonly_SLOT, vo = allocVector(INTSXP, 1));
INTEGER(vo)[0] = varonly;
if (varonly) {
    SET_VECTOR_ELT(ans, Variance_SLOT, tmp = allocVector(REALSXP, PQ));
} else {
    /* always return variance */
    SET_VECTOR_ELT(ans, Variance_SLOT, tmp = allocVector(REALSXP, PQ));
    SET_VECTOR_ELT(ans, Covariance_SLOT,
                   tmp = allocVector(REALSXP, PP12(PQ)));
}
SET_VECTOR_ELT(ans, ExpectationX_SLOT, allocVector(REALSXP, P));
SET_VECTOR_ELT(ans, dim_SLOT, d = allocVector(INTSXP, 2));
INTEGER(d)[0] = P;
INTEGER(d)[1] = Q;
SET_VECTOR_ELT(ans, ExpectationInfluence_SLOT,
               tmp = allocVector(REALSXP, B * Q));
for (int q = 0; q < B * Q; q++) REAL(tmp)[q] = 0.0;

/* should always _both_ be there */
SET_VECTOR_ELT(ans, VarianceInfluence_SLOT,
               tmp = allocVector(REALSXP, B * Q));
for (int q = 0; q < B * Q; q++) REAL(tmp)[q] = 0.0;

SET_VECTOR_ELT(ans, CovarianceInfluence_SLOT,
               tmp = allocVector(REALSXP, B * Q * (Q + 1) / 2));
for (int q = 0; q < B * Q * (Q + 1) / 2; q++) REAL(tmp)[q] = 0.0;

SET_VECTOR_ELT(ans, Xfactor_SLOT, allocVector(INTSXP, 1));
INTEGER(VECTOR_ELT(ans, Xfactor_SLOT))[0] = Xfactor;
SET_VECTOR_ELT(ans, TableBlock_SLOT, tmp = allocVector(REALSXP, B + 1));
for (int q = 0; q < B + 1; q++) REAL(tmp)[q] = 0.0;
SET_VECTOR_ELT(ans, Sumweights_SLOT, allocVector(REALSXP, B));
SET_VECTOR_ELT(ans, PermutedLinearStatistic_SLOT,
               allocMatrix(REALSXP, 0, 0));
SET_VECTOR_ELT(ans, StandardisedPermutedLinearStatistic_SLOT,
               allocMatrix(REALSXP, 0, 0));
SET_VECTOR_ELT(ans, tol_SLOT, tolerance = allocVector(REALSXP, 1));
REAL(tolerance)[0] = tol;
namesgets(ans, names);

@<Initialise Zero@>
@}

@d Initialise Zero
@{
/* set inital zeros */
for (int p = 0; p < PQ; p++) {
    C_get_LinearStatistic(ans)[p] = 0.0;
    C_get_Expectation(ans)[p] = 0.0;
    if (varonly)
        C_get_Variance(ans)[p] = 0.0;
}
if (!varonly) {
    for (int p = 0; p < PP12(PQ); p++)
        C_get_Covariance(ans)[p] = 0.0;
}
for (int q = 0; q < Q; q++) {
    C_get_ExpectationInfluence(ans)[q] = 0.0;
    C_get_VarianceInfluence(ans)[q] = 0.0;
}
for (int q = 0; q < Q * (Q + 1) / 2; q++)
    C_get_CovarianceInfluence(ans)[q] = 0.0;
@}

@d RC_init_LECV_1d
@{
SEXP RC_init_LECV_1d
(
    @<C integer P Input@>,
    @<C integer Q Input@>,
    int varonly,
    @<C integer B Input@>,
    int Xfactor,
    double tol
) {
    SEXP ans;

    @<R_init_LECV@>

    SET_VECTOR_ELT(ans, TableBlock_SLOT,
                   allocVector(REALSXP, B + 1));

    SET_VECTOR_ELT(ans, Sumweights_SLOT,
                   allocVector(REALSXP, B));

    UNPROTECT(2);
    return(ans);
}
@|RC_init_LECV_1d
@}

@d RC_init_LECV_2d
@{
SEXP RC_init_LECV_2d
(
    @<C integer P Input@>,
    @<C integer Q Input@>,
    int varonly,
    int Lx,
    int Ly,
    @<C integer B Input@>,
    int Xfactor,
    double tol
) {
    SEXP ans, tabdim, tab;

    if (Lx <= 0)
        error("Lx is not positive");

    if (Ly <= 0)
        error("Ly is not positive");

    @<R_init_LECV@>

    PROTECT(tabdim = allocVector(INTSXP, 3));
    INTEGER(tabdim)[0] = Lx + 1;
    INTEGER(tabdim)[1] = Ly + 1;
    INTEGER(tabdim)[2] = B;
    SET_VECTOR_ELT(ans, Table_SLOT,
                   tab = allocVector(REALSXP,
                       INTEGER(tabdim)[0] *
                       INTEGER(tabdim)[1] *
                       INTEGER(tabdim)[2]));
    dimgets(tab, tabdim);

    UNPROTECT(3);
    return(ans);
}
@|RC_init_LECV_2d
@}

\chapter{Package Infrastructure}

@o AAA.R -cp
@{
@<R Header@>
.onUnload <-
function(libpath)
    library.dynam.unload("libcoin", libpath)
@}

@o DESCRIPTION -cp
@{
Package: libcoin
Title: Linear Test Statistics for Permutation Inference
Date: 2023-09-26
Version: 1.0-10
Authors@@R: person("Torsten", "Hothorn", role = c("aut", "cre"),
                  email = "Torsten.Hothorn@@R-project.org")
Description: Basic infrastructure for linear test statistics and permutation
  inference in the framework of Strasser and Weber (1999) <https://epub.wu.ac.at/102/>.
  This package must not be used by end-users. CRAN package 'coin' implements all
  user interfaces and is ready to be used by anyone.
Depends: R (>= 3.4.0)
Suggests: coin
Imports: stats, mvtnorm
LinkingTo: mvtnorm
NeedsCompilation: yes
License: GPL-2
@}

@o NAMESPACE -cp
@{
useDynLib(libcoin, .registration = TRUE)

importFrom("stats", complete.cases, vcov)
importFrom("mvtnorm", GenzBretz)

export(LinStatExpCov, doTest, ctabs, lmult)

S3method(vcov, LinStatExpCov)
@}

Add flag \code{-g} to \code{PKG_CFLAGS} for \code{operf} profiling (this is
not portable).
@o Makevars -cc
@{
PKG_CFLAGS=$(C_VISIBILITY)
PKG_LIBS = $(LAPACK_LIBS) $(BLAS_LIBS) $(FLIBS)
@}

@o libcoin-win.def -cc
@{
LIBRARY libcoin.dll
EXPORTS
  R_init_libcoin
@}

Other packages can link against \pkg{libcoin}. A small example package
is contained in \texttt{libcoin/inst/C_API_example}.

@o libcoin-init.c -cc
@{
@<C Header@>
#include "libcoin.h"
#include <R_ext/Rdynload.h>
#include <R_ext/Visibility.h>

#define CALLDEF(name, n) {#name, (DL_FUNC) &name, n}
#define REGCALL(name) R_RegisterCCallable("libcoin", #name, (DL_FUNC) &name)

static const R_CallMethodDef callMethods[] = {
    CALLDEF(R_ExpectationCovarianceStatistic, 7),
    CALLDEF(R_PermutedLinearStatistic, 6),
    CALLDEF(R_StandardisePermutedLinearStatistic, 1),
    CALLDEF(R_ExpectationCovarianceStatistic_2d, 9),
    CALLDEF(R_PermutedLinearStatistic_2d, 7),
    CALLDEF(R_QuadraticTest, 5),
    CALLDEF(R_MaximumTest, 9),
    CALLDEF(R_MaximallySelectedTest, 6),
    CALLDEF(R_ExpectationInfluence, 3),
    CALLDEF(R_CovarianceInfluence, 4),
    CALLDEF(R_ExpectationX, 4),
    CALLDEF(R_CovarianceX, 5),
    CALLDEF(R_Sums, 3),
    CALLDEF(R_KronSums, 6),
    CALLDEF(R_KronSums_Permutation, 5),
    CALLDEF(R_colSums, 3),
    CALLDEF(R_OneTableSums, 3),
    CALLDEF(R_TwoTableSums, 4),
    CALLDEF(R_ThreeTableSums, 5),
    CALLDEF(R_order_subset_wrt_block, 4),
    CALLDEF(R_quadform, 3),
    CALLDEF(R_kronecker, 2),
    CALLDEF(R_MPinv_sym, 3),
    CALLDEF(R_unpack_sym, 3),
    CALLDEF(R_pack_sym, 1),
    {NULL, NULL, 0}
};
@}

@o libcoin-init.c -cc
@{
void attribute_visible R_init_libcoin
(
    DllInfo *dll
) {
    R_registerRoutines(dll, NULL, callMethods, NULL, NULL);
    R_useDynamicSymbols(dll, FALSE);
    R_forceSymbols(dll, TRUE);
    REGCALL(R_ExpectationCovarianceStatistic);
    REGCALL(R_PermutedLinearStatistic);
    REGCALL(R_StandardisePermutedLinearStatistic);
    REGCALL(R_ExpectationCovarianceStatistic_2d);
    REGCALL(R_PermutedLinearStatistic_2d);
    REGCALL(R_QuadraticTest);
    REGCALL(R_MaximumTest);
    REGCALL(R_MaximallySelectedTest);
    REGCALL(R_ExpectationInfluence);
    REGCALL(R_CovarianceInfluence);
    REGCALL(R_ExpectationX);
    REGCALL(R_CovarianceX);
    REGCALL(R_Sums);
    REGCALL(R_KronSums);
    REGCALL(R_KronSums_Permutation);
    REGCALL(R_colSums);
    REGCALL(R_OneTableSums);
    REGCALL(R_TwoTableSums);
    REGCALL(R_ThreeTableSums);
    REGCALL(R_order_subset_wrt_block);
    REGCALL(R_quadform);
    REGCALL(R_kronecker);
    REGCALL(R_MPinv_sym);
    REGCALL(R_unpack_sym);
    REGCALL(R_pack_sym);
}
@}

@d R Header
@{
###    Copyright (C) 2017-2023 Torsten Hothorn
###
###    This file is part of the 'libcoin' R add-on package.
###
###    'libcoin' is free software: you can redistribute it and/or modify
###    it under the terms of the GNU General Public License as published by
###    the Free Software Foundation, version 2.
###
###    'libcoin' is distributed in the hope that it will be useful,
###    but WITHOUT ANY WARRANTY; without even the implied warranty of
###    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
###    GNU General Public License for more details.
###
###    You should have received a copy of the GNU General Public License
###    along with 'libcoin'.  If not, see <http://www.gnu.org/licenses/>.
###
###
###    DO NOT EDIT THIS FILE
###
###    Edit 'libcoin.w' and run 'nuweb -r libcoin.w'
@}

@d C Header
@{
/*
    Copyright (C) 2017-2023 Torsten Hothorn

    This file is part of the 'libcoin' R add-on package.

    'libcoin' is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, version 2.

    'libcoin' is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with 'libcoin'.  If not, see <http://www.gnu.org/licenses/>.


    DO NOT EDIT THIS FILE

    Edit 'libcoin.w' and run 'nuweb -r libcoin.w'
*/
@}

\chapter*{Index}

\section*{Files}

@f

\section*{Fragments}

@m

\section*{Identifiers}

@u

\bibliographystyle{plainnat}
\bibliography{libcoin}

\end{document}
