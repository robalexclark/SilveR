\newcommand{\C}{\ifelse{latex}{\out{\textsf{C}}}{C}}
\newcommand{\nuweb}{\ifelse{latex}{\out{\textsf{nuweb}}}{nuweb}}

\name{NEWS}
\title{NEWS file for the \pkg{libcoin} package}

\section{Changes in Version 1.0-10 (2023-09-26)}{
  \itemize{
    \item Add Henric Winell as author.
    \item Enable \code{STRICT_R_HEADERS}.
    \item The \nuweb source code is now included (see \file{../inst/nuweb/}).
    \item Documentation updates.
  }
}

\section{Changes in Version 1.0-9 (2021-09-27)}{
  \itemize{
    \item \code{R_MPinv_sym} can now determine the full dimension of the packed
          input \code{x} by setting \code{n} to \code{0L}.
    \item Add \code{#define USE_FC_LEN_T} to header as requested by CRAN
          2021-09-25.
  }
}

\section{Changes in Version 1.0-8 (2021-02-08)}{
  \itemize{
    \item Fix \LaTeX problem.
  }
}

\section{Changes in Version 1.0-7 (2021-01-15)}{
  \itemize{
    \item Correct spelling in error message.
    \item Regression tests updated for \pkg{coin} 1.4-0.
  }
}

\section{Changes in Version 1.0-6 (2020-08-13)}{
  \itemize{
    \item Interface to \code{S_rcont2} changed in \pkg{stats}, thanks
          to Martin Maechler for a fix.
    \item New \C function \code{R_pack_sym} for transforming a symmetric matrix
          to lower-packed storage mode.
  }
}

\section{Changes in Version 1.0-5 (2019-08-22)}{
  \itemize{
    \item New \C function \code{R_unpack_sym} for unpacking a symmetric matrix
          stored in lower-packed storage mode.
    \item New \C functions \code{R_quadform} and \code{R_MPinv_sym} providing
          interfaces to the internal functions \code{C_quadform} and
          \code{C_MPinv_sym}.
    \item Function \code{libcoin_StandardisePermutedLinearStatistic} was renamed
          to \code{libcoin_R_StandardisePermutedLinearStatistic}.
    \item Documentation updates.
  }
}

\section{Changes in Version 1.0-4 (2019-02-28)}{
  \itemize{
    \item Bugfix in regression tests.
  }
}

\section{Changes in Version 1.0-3 (2019-02-18)}{
  \itemize{
    \item Parts of the covariance matrix were not set to zero initially.
  }
}

\section{Changes in Version 1.0-2 (2018-12-13)}{
  \itemize{
    \item Maximally selected statistics failed for large sample sizes because of
          unnecessary memory allocation.  Report and fix by Joanidis
          Kristoforos.
    \item Some additional checks for integer overflow.
  }
}

\section{Changes in Version 1.0-1 (2017-12-13)}{
  \itemize{
    \item Make valgrind happy.
  }
}

\section{Changes in Version 1.0-0 (2017-12-12)}{
  \itemize{
    \item Make the package truly literate; there is one single \nuweb file
          called \file{libcoin.w} which generates the complete package.
    \item The package can now deal with long vectors.
  }
}

\section{Changes in Version 0.9-3 (2017-07-02)}{
  \itemize{
    \item Remove copy of \code{rcont2} from the \pkg{stats} package as it is
          exported now.
  }
}

\section{Changes in Version 0.9-2 (2017-04-04)}{
  \itemize{
    \item Fix protect problem in \code{R_MaximumTest} reported by Tomas
          Kalibera.
  }
}

\section{Changes in Version 0.9-1 (2017-02-06)}{
  \itemize{
    \item Calling \pkg{libcoin}'s \C routines (using \code{.Call()}) now
          requires the entry points to be specified as \R objects, i.e., the use
          of character strings is no longer allowed.
    \item Stop when all observations are missing.
    \item 2d case with missings in at least one variable used incorrect sample
          sizes and thus produced wrong results.
  }
}

\section{Changes in Version 0.9-0 (2016-12-09)}{
  \itemize{
    \item \pkg{libcoin} published on CRAN.
  }
}
