# Version 1.5.0 [2025-01-07]

## Significant Changes

 * Package now requires R (>= 3.4.0) [2017-04-21], because the next
   release of R will have stricter C header requirements that are not
   backward compatible with older versions of R.

## Deprecated and Defunct

 * The hidden R options for deescalating the error for using `useNames
   = NA` to a warning has been removed; `useNames = NA` is now always
   an error.

 * Calling `colRanks()` and `rowRanks()` without explicitly specifying
   argument `ties.method` is deprecated since version 1.3.0
   [2024-04-10]. If not explicitly specified, a deprecation warning is
   now produced every 10:th call not specifying the `ties.method`
   argument.

## Bug Fixes

 * The error message of `colTabulates()` and `rowTabulates()`
   asserting that double values are passed, reported on the class of
   the input data, not the storage type.

 
# Version 1.4.1 [2024-09-06]

## Bug Fixes

 * Fix a `runtime error: null pointer passed as argument 1, which is
   declared to never be null` bug introduced in v1.4.0 that was
   detected by the UndefinedBehaviorSanitizer (UBSan) running on CRAN.
 

# Version 1.4.0 [2024-09-03]

## Performance

 * `rowSums2()` is now significantly faster for larger matrices.
 
## Miscellaneous

 * None of the error messages use a trailing period.
 
 * Addressing changes in the C API of R-devel resulted in compiler
   errors such as `error: implicit declaration of function 'Calloc';
   did you mean 'calloc'? [-Wimplicit-function-declaration]`.

 * Addressing changes in stricter compiler flags of R-devel resulted
   in compiler warning `embedding a directive within macro arguments
   has undefined behavior [-Wembedded-directive]`.

## Deprecated and Defunct

 * Calling `colRanks()` and `rowRanks()` without explicitly specifying
   argument `ties.method` is deprecated since version 1.3.0 [2024-04-10].
   If not explicitly specified, a deprecation warning is now produced every
   25:th call not specifying the `ties.method` argument.
   

# Version 1.3.0 [2024-04-10]

## Significant Changes

 * `validateIndices()` has been removed. It had been defunct since
   version 0.63.0 (2022-11-14).

## Bug Fixes

 * Fixed two PROTECT/UNPROTECT issues detected by the 'rchk' tool.

## Deprecated and Defunct

 * Calling `colRanks()` and `rowRanks()` without explicitly specifying
   argument `ties.method` will be deprecated when using R (>=
   4.4.0). The reason is that the current default is `ties.method =
   "max"`, but we want to change that to `ties.method = "average"` to
   align it with `base::rank()`. In order to minimize the risk for
   sudden changes in results, we ask everyone to explicitly specify
   their intent. The first notice will be through deprecation
   warnings, which will only occur every 50:th call to keep the noise
   level down.  We will make it more noisy in future releases, and
   eventually also escalated to defunct errors.
 
 * Using a scalar value for argument `center` of `colSds()`,
   `rowSds()`, `colVars()`, `rowVars()`, `colMads()`, `rowMads()`,
   `colWeightedMads()`, and `rowWeightedMads()` is now defunct.


# Version 1.2.0 [2023-12-11]

## Bug Fixes

 * Error messages that report on large integers (> 2^31 - 1), would
   not render those integers correctly.

## Deprecated and Defunct

 * `useNames = NA` is defunct.


# Version 1.1.0 [2023-11-06]

## Deprecated and Defunct

 * `useNames = NA` is defunct in R (>= 4.4.0). Remains deprecated in
   R (< 4.4.0) for now.

## Miscellaneous

 * The deprecation warning for using `useNames = NA`, suggested using
   `useNames = TRUE` twice instead of also `useNames = FALSE`.
 

# Version 1.0.0 [2023-06-01]

## Significant Changes

 * `useNames = TRUE` is the new default for all functions.  For
   backward compatibility, it used to be `useNames = NA`.

 * `colQuantiles()` and `rowQuantiles()` gained argument `digits`,
   just like `stats::quantile()` gained that argument in R 4.1.0.

 * `colQuantiles()` and `rowQuantiles()` only sets quantile percentage
   names when `useNames = TRUE`, to align with how argument `names` of
   `stats::quantile()` works in base R.

## New Features

 * `colMeans2()` and `rowMeans2()` gained argument `refine`.  If
   `refine = TRUE`, then the sample average for numeric matrices are
   calculated using a two-pass scan, resulting in higher precision.
   The default is `refine = TRUE` to align it with `colMeans()`, but
   also `mean2()` in this package.  If the higher precision is not
   needed, using `refine = FALSE` will be almost twice as fast.

 * `colSds()`, `rowSds()`, `colVars()`, and `rowVars()` gained
   argument `refine`.  If `refine = TRUE`, then the sample average for
   numeric matrices are calculated using a two-pass scan, resulting in
   higher precision for the estimate of the center and therefore also
   the variance.

## Performance

 * Unnecessary checks for missing indices are eliminated, yielding
   better performance. This change does not affect user-facing API.

 * Made `colQuantiles()` and `rowQuantiles()` a bit faster for `type
   != 7L`, by making sure percentage names are only generated once,
   instead of once per column or row.

## Bug Fixes

 * Contrary to other functions in the package, and how it works in
   base R, functions `colCumsums()`, `colCumprods()`, `colCummins()`,
   `colCummaxs()`, `colRanges()`, `colRanks()`, and `colDiffs()`, plus
   the corresponding row-based versions, did not drop the `names`
   attribute when both row and column names were `NULL`. Now also
   these functions behaves the same as the case when neither row or
   column names are set.

 * `colQuantiles()` and `rowQuantiles()` did not generate quantile
   percentage names exactly the same way as `stats::quantile()`, which
   would reveal itself for certain combinations of `probs` and
   `digits`.

## Deprecated and Defunct

 * `useNames = NA` is now deprecated. Use `useNames = TRUE` or
   `useNames = FALSE` instead.


# Version 0.63.0 [2022-11-14]

## Miscellaneous

 * Package compiles again with older compilers not supporting the C99
   standard (e.g. GCC 4.8.5 (2015), which is the default on RHEL /
   CentOS 7.9).  This was the case also for matrixStats (<= 0.54.0).

 * Added more information to the error message produced when argument
   `center` for `col-` and `rowVars()` holds an invalid value.

 * Fix two compilation warnings on `a function declaration without a
   prototype is deprecated in all versions of C
   [-Wstrict-prototypes]`.

## Deprecated and Defunct

 * `validateIndices()` is now defunct and will eventually be removed
   from the package API.


# Version 0.62.0 [2022-04-18]

## New Features

 * `colCummins()`, `colCummaxs()`, `rowCummins()`, and `rowCummaxs()`
   now support also logical input.

## Miscellaneous

 * Updated native code to use the C99 constant `DBL_MAX` instead of
   legacy S constant `DOUBLE_XMAX`, which is planned to be unsupported
   in R (>= 4.2.0).
 

# Version 0.61.0 [2021-09-12]

## New Features

 * When argument `which` for `colOrderStats()` and `rowOrderStats()`
   is out of range, the error message now reports on the value of
   `which`.  Similarly, when argument `probs` for `colQuantiles()` and
   `rowQuantiles()` is out of range, the error message reports on its
   value too.
 
## Deprecated and Defunct

 * `validateIndices()` is deprecated and will eventually be removed
   from the package API.

## Miscellaneous

 * The package test for benchmark reports failed because the
   **markdown** package was not declared as a suggested package.


# Version 0.60.1 [2021-08-22]

## Performance

 * Handling of the `useNames` argument is now done in the native code.

 * Passing `idxs`, `rows`, and `cols` arguments of type integer is now
   less efficient than it used to, because the new code re-design (see
   below) requires an internal allocation of an equally long
   `R_xlen_t` vector that is populated by indices coerced from
   `R_len_t` to `R_xlen_t` integers.

## Code Design

 * No longer using native-code implementations that are specific to
   the type of index that is passed for subsetting of vectors, rows,
   and columns.  This was done to avoid the complex use of macros that
   was cumbersome to maintain and added an extra threshold for new
   contributors to overcome.  Another advantage is that faster
   compilation time when built from source and a smaller size of
   compiled library.  In previous version `R CMD check` would produce
   a NOTE on the package installation size being large, which no
   longer is the case. The downside is that extra overhead when
   passing integer indices (see above comment).

## Bug Fixes

 * Contrary to other functions which gained new argument `useNames =
   NA` in the previous release, `colQuantiles()` and `rowQuantiles()`
   got `useNames = TRUE`.
 

# Version 0.60.0 [2021-07-26]

## New Features

 * Add row and column names support to all row and column
   functions. To return row and column names, set argument `useNames =
   TRUE`. To drop them, set `useNames = FALSE`. To preserve the
   current, inconsistent behavior, set `useNames = NA`, which, for
   backward compatibility reasons, remains the default for now.


# Version 0.59.0 [2021-05-31]

## Miscellaneous

 * Harmonized error messages.

## Bug Fixes

 * Some of the examples and package tests would allocated matrices
   with dimensions that did not match the number of elements in the
   input data.

## Deprecated and Defunct

 * Dropped `meanOver()` and `sumOver()`, and argument `method` from
   `weightedVar()`, that have been defunct since January 2018.


# Version 0.58.0 [2021-01-26]

## Significant Changes

 * `colVars()` and `rowVars()` with argument `center` now calculates
   the sample variance using the `n/(n-1)*avg((x-center)^2)` formula
   rather than the `n/(n-1)*(avg(x^2)-center^2)` formula that was used
   in the past.  Both give the same result when `center` is the
   correct sample mean estimate. The main reason for this change is
   that, if an incorrect `center` is provided, in contrast to the old
   approach, the new approach is guaranteed to give at least
   non-negative results, despite being incorrect.  BACKWARD
   COMPATIBILITY: Out of all 314 reverse dependencies on CRAN and
   Bioconductor, only four called these functions with argument
   `center`. All of them pass their package checks also after this
   update.  To further protect against a negative impact in existing
   user scripts, `colVars()` and `rowVars()` will calculate both
   versions and assert that the result is the same.  If not, an
   informative error is produced.  To limit the performance impact,
   this validation is run only once every 50:th call, a frequency that
   can be controlled by R option `matrixStats.vars.formula.freq`.
   Setting it to 0 or NULL will disable the validation.  The default
   can also be controlled by environment variable
   `R_MATRIXSTATS_VARS_FORMULA_FREQ`.  This validation framework will
   be removed in a future version of the package after it has been
   established that this change has no negative impact.

## New Features

 * Now `colWeightedMads()` and `rowWeightedMads()` accept `center` of
   the same length as the number of columns and rows, respectively.

 * `colAvgsPerRowSet()` and `rowAvgsPerRowSet()` gained argument
   `na.rm`.

 * Now `weightedMean()` and `weightedMedian()` and the corresponding
   row- and column-based functions accept logical `x`, where FALSE is
   treated as integer 0 and TRUE as 1.

 * Now `x_OP_y()` and `t_tx_OP_y()` accept logical `x` and `y`, where
   FALSE is treated as integer 0 and TRUE as 1.

## Bug Fixes

 * `colQuantiles()` and `rowQuantiles()` on a logical matrix should
   return a numeric vector for `type = 7`. However, when there were
   only missing values (= NA) in the matrix, then it would return a
   "logical" vector instead.

 * `colAvgsPerRowSet()` on a single-column matrix would produce an
   error on non-matching dimensions.  Analogously, for
   `rowAvgsPerRowSet()` and single- row matrices.

 * `colVars(x)` and `rowVars(x)` with `x` being an array would give
   the wrong value if both argument `dim.` and `center` would be
   specified.

 * The documentation was unclear on what the `center` argument should
   be. They would not detect when an incorrect specification was used,
   notably when the length of `center` did not match the matrix
   dimensions.  Now these functions give an informative error message
   when `center` is of the incorrect length.

## Deprecated and Defunct

 * Using a scalar value for argument `center` of `colSds()`,
   `rowSds()`, `colVars()`, `rowVars()`, `colMads()`, `rowMads()`,
   `colWeightedMads()`, and `rowWeightedMads()` is now deprecated.
 

# Version 0.57.0 [2020-09-25]

## New Features

 * `colCumprods()` and `rowCumprods()` now support also logical
   input. Thanks to Constantin Ahlmann-Eltze at EMBL Heidelberg for
   the patch.

## Bug Fixes

 * `colCollapse()` and `rowCollapse()` did not expand `idxs` argument
   before subsetting by `cols` and `rows`, respectively.  Thanks to
   Constantin Ahlmann-Eltze for reporting on this.

 * `colAnys()`, `rowAnys()`, `anyValue()`, `colAlls()`, `rowAlls()`,
   and `allValue()` with `value=FALSE` and _numeric_ input would
   incorrectly consider all values different from one as FALSE.  Now
   it is only values that are zero that are considered FALSE.  Thanks
   to Constantin Ahlmann-Eltze for the bug fix.


# Version 0.56.0 [2020-03-12]

## Significant Changes

 * `colQuantiles()` and `rowQuantiles()` now supports only integer,
   numeric and logical input.  Previously, it was also possible to
   pass, for instance, `character` input, but that was a mistake.  The
   restriction on input allows for further optimization of these
   functions.

 * The returned type of `colQuantiles()` and `rowQuantiles()` is now
   the same as for `stats::quantile()`, which depends on argument
   `type`.

## Performance

 * `colQuantiles()` and `rowQuantiles()` with the default `type = 7L`
   and when there are no missing values are now significantly faster
   and use significantly fewer memory allocations.

## Bug Fixes

 * `colDiffs()` and `rowDiffs()` gave an error if argument `dim.` was
   of type numeric rather than type integer.
 
 * `varDiff()`, `sdDiff()`, `madDiff()`, `iqrDiff()`, and the
   corresponding row- and column functions silently treated a `diff`
   less than zero as `diff = 0`.  Now an error is produced.
   
 * Error messages on argument `dim.` referred to non-existing argument
   `dim`.
 
 * Error messages on negative values in argument `dim.` reported a
   garbage value instead of the negative value.
 
 * The Markdown reports produced by the internal benchmark report
   generator did not add a line between tables and the following text
   (a figure caption) causing the following text to be included in a
   cell on an extra row in the table (at least when rendered on GitHub
   Wiki pages).


# Version 0.55.0 [2019-09-05]

## Significant Changes

 * `weightedVar()`, `weightedSd()`, `weightedMad()`, and their row-
   and column- specific counter parts now return a missing value if
   there are missing values in any of the weights `w` after possibly
   dropping (`x`, `w`) elements with missing values in `x` (`na.rm =
   TRUE`).  Previously, `na.rm = TRUE` would also drop (`x`, `w`)
   elements where `w` was missing.  With this change, we now have that
   for all functions in this package, `na.rm = TRUE` never applies to
   weights - only `x` values.

## New Features

 * `colRanks()` and `rowRanks()` now supports the same set of
   `ties.method` as `base::rank()` plus `"dense"` as defined by
   `data.table::frank()`. For backward compatible reasons, the default
   `ties.method` remains the same as in previous versions.  Thank to
   Brian Montgomery for contributing this.

 * `colCumsums()` and `rowCumsums()` now support also logical input.

## Bug Fixes

 * `weightedVar()`, `weightedSd()`, `weightedMad()`, and their row-
   and column- specific counter parts would produce an error instead
   of returning a missing value when one of the weights is a missing
   value.

## Deprecated and Defunct

 * Calling `indexByRow(x)` where `x` is a matrix is now defunct. Use
   `indexByRow(dim(x))` instead.


# Version 0.54.0 [2018-07-23]

## Performance

 * SPEEDUP: No longer using `stopifnot()` for internal validation,
   because it comes with a great overhead.  This was only used in
   `weightedMad()`, `col-`, and `rowWeightedMads()`, as well as `col-`
   and `rowAvgsPerColSet()`.

## Bug Fixes

 * Despite being an unlikely use case, `colLogSumExps(lx)` /
   `rowLogSumExps(lx)` now also accepts integer `lx` values.

 * The error produced when using `indexByRow(dim)` with `prod(dim) >=
   2^31` would report garbage dimensions instead of `dim`.

## Deprecated and Defunct

 * Calling `indexByRow(x)`, where `x` is a matrix, is deprecated. Use
   `indexByRow(dim(x))` instead.


# Version 0.53.1 [2018-02-10]

## Code Refactoring

 * Now `col-`/`rowSds()` explicitly replicate all arguments that are
   passed to `col-`/`rowVars()`.

## Documentation

 * Added details on how `weightedMedian(x, interpolate = TRUE)` works.

## Bug Fixes

 * `colLogSumExps(lx, cols)` / `rowLogSumExps(lx, rows)` gave an error
   if `lx` has rownames / colnames.

 * `col-`/`rowQuantiles()` would lose rownames of output in certain
   cases.


# Version 0.53.0 [2018-01-23]

## New Features

 * Functions `sum2(x)` and `means2(x)` now accept also logical input
   `x`, which corresponds to using `as.integer(x)` but without the need
   for neither coercion nor internal extra copies. With `sum2(x, mode =
   "double")` it is possible to count number of TRUE elements beyond
   2^31-1, which `base::sum()` does not support.

 * Functions `col-`/`rowSums2()` and `col-`/`rowMeans2()` now accept
   also logical input `x`.

 * Function `binMeans(y, x, bx)` now accepts logical `y`, which
   corresponds to to using `as.integer(y)`, but without the need for
   coercion to integer.

 * Functions `col-`/`rowTabulates(x)` now support logical input `x`.

 * Now `count()` can count beyond 2^31-1.

 * `allocVector()` can now allocate long vectors (longer than 2^31-1).

 * Now `sum2(x, mode = "integer")` generates a warning if `typeof(x)
   == "double"` asking if `as.integer(sum2(x))` was intended.

 * Inspired by `Hmisc::wtd.var()`, when `sum(w) <= 1`, `weightedVar(x,
   w)` now produces an informative warning that the estimate is
   invalid.

## Code Refactoring

 * Harmonized the ordering of the arguments of `colAvgsPerColSet()`
   with that of `rowAvgsPerColSet()`.

## Bug Fixes

 * `col-`/`rowLogSumExp()` could core dump R for "large" number of
   columns/rows.  Thanks Brandon Stewart at Princeton University for
   reporting on this.

 * `count()` beyond 2^31-1 would return invalid results.

 * Functions `col-`/`rowTabulates(x)` did not count missing values.

 * `indexByRow(dim, idxs)` would give nonsense results if `idxs` had
   indices greater than `prod(dim)` or non-positive indices; now it
   gives an error.

 * `indexByRow(dim)` would give nonsense results when `prod(dim) >=
   2^31`; now it gives an informative error.

 * `col-`/`rowAvgsPerColSet()` would return vector rather than matrix
   if `nrow(X) <= 1`. Thanks to Peter Hickey (Johns Hopkins
   University) for troubleshooting and providing a fix.

## Deprecated and Defunct

 * Previously deprecated `meanOver()` and `sumOver()` are defunct. Use
   `mean2()` and `sum2()` instead.

 * Previously deprecated `weightedVar(x, w, method = "0.14.2")` is defunct.

 * Dropped previously defunct `weightedMedian(..., ties = "both")`.

 * Dropped previously defunct argument `centers` for
   `col-`/`rowMads()`. Use `center` instead.

 * Dropped previously defunct argument `flavor` of `colRanks()` and
   `rowRanks()`.


# Version 0.52.2 [2017-04-13]

## Bug Fixes

 * Several of the row- and column-based functions would core dump R if the
   matrix was of a data type other than logical, integer, or numeric, e.g.
   character or complex.  This is now detected and an informative error is
   produced instead.  Similarly, some vector-based functions could potentially
   core dump R or silently return a nonsense result.  Thank you Hervé Pagès,
   Bioconductor Core, for the report.

## Deprecated and Defunct

 * `rowVars(..., method = "0.14.2")` that was added for very unlikely
   needs of backward compatibility of an invalid degree-of-freedom
   term is deprecated.


# Version 0.52.1 [2017-04-04]

## Bug Fixes

 * The package test on `matrixStats:::benchmark()` tried to run even
   if not all suggested packages were available.


# Version 0.52.0 [2017-04-03]

## Significant Changes

 * Since `anyNA()` is a built-in function since R (>= 3.1.0), please
   use that instead of `anyMissing()` part of this package.  The
   latter will eventually be deprecated.  For consistency with the
   `anyNA()` name, `colAnyNAs()` and `rowAnyNAs()` are now also
   available replacing the identically `colAnyMissings()` and
   `rowAnyMissings()` functions, which will also be deprecated in a
   future release.

 * `meanOver()` was renamed to `mean2()` and `sumOver()` was renamed
   to `sum2()`.

## New Features

 * Added `colSums2()` and `rowSums2()` which work like `colSums()` and
   `rowSums()` of the **base** package but also supports efficient
   subsetting via optional arguments `rows` and `cols`.

 * Added `colMeans2()` and `rowMeans2()` which work like `colMeans()`
   and `rowMeans()` of the **base** package but also supports efficient
   subsetting via optional arguments `rows` and `cols`.

 * Functions `colDiffs()` and `rowDiffs()` gained argument `dim.`.

 * Functions `colWeightedMads()` and `rowWeightedMads()` gained
   arguments `constant` and `center`.  The current implementation only
   support scalars for these arguments, which means that the same
   values are applied to all columns and rows, respectively.  In
   previous version a hard-to-understand error would be produced if
   `center` was of length greater than one; now an more informative
   error message is given.

 * Package is now silent when loaded; it no longer displays a startup
   message.

## Software Quality

 * Continuous-integration testing is now also done on macOS, in
   addition to Linux and Windows.

 * ROBUSTNESS: Package now registers the native API using also
   `R_useDynamicSymbols()`.

## Code Refactoring

 * Cleaned up native low-level API and renamed native source code files
   to make it easier to navigate the native API.

 * Now using **roxygen2** for help and NAMESPACE (was `R.oo::Rdoc`).

## Bug Fixes

 * `rowAnys(x)` on numeric matrices `x` would return `rowAnys(x == 1)`
   and not `rowAnys(x != 0)`.  Same for `colAnys()`, `rowAlls()`, and
   `colAlls()`.  Thanks Richard Cotton for reporting on this.

 * `sumOver(x)` and `meanOver(x)` would incorrectly return -Inf or
   +Inf if the intermediate sum would have that value, even if one
   of the following elements would turn the intermediate sum into
   NaN or NA, e.g. with `x` as `c(-Inf, NaN)`, `c(-Inf, +Inf)`, or
   `c(+Inf, NA)`.

 * WORKAROUND: Benchmark reports generated by
   `matrixStats:::benchmark()` would use any custom R prompt that is
   currently set in the R session, which may not render very well.
   Now it forces the prompt to be the built-in `"> "` one.

## Deprecated and Defunct

 * The package API is only intended for matrices and vectors of type
   numeric, integer and logical.  However, a few functions would still
   return if called with a data.frame.  This was never intended to
   work and is now an error.  Specifically, functions `colAlls()`,
   `colAnys()`, `colProds()`, `colQuantiles()`, `colIQRs()`,
   `colWeightedMeans()`, `colWeightedMedians()`, and `colCollapse()`
   now produce warnings if called with a data.frame.  Same for the
   corresponding row- functions.  The use of a `data.frame will be
   produce an error in future releases.

 * `meanOver()` and `sumOver()` are deprecated because they were
   renamed to `mean2()` and `sum2()`, respectively.

 * Previously deprecated (and ignored) argument `flavor` of
   `colRanks()` and `rowRanks()` is now defunct.

 * Previously deprecated support for passing non-vector, non-matrix
   objects to `rowAlls()`, `rowAnys()`, `rowCollapse()`, and the
   corresponding column-based versions are now defunct.  Likewise,
   `rowProds()`, `rowQuantiles()`, `rowWeightedMeans()`,
   `rowWeightedMedians()`, and the corresponding column-based versions
   are also defunct.  The rationale for this is to tighten up the
   identity of the **matrixStats** package and what types of input it
   accepts.  This will also help optimize the code further.


# Version 0.51.0 [2016-10-08]

## Performance and Memory

 * SPEEDUP / CLEANUP: `rowMedians()` and `colMedians()` are now plain
   functions.  They were previously S4 methods (due to a Bioconductor
   legacy).  The package no longer imports the **methods** package.

 * SPEEDUP: Now native API is formally registered allowing for faster
   lookup of routines from R.


# Version 0.50.2 [2016-04-24]

## Bug Fixes

 * Package now installs on R (>= 2.12.0) as claimed.  Thanks to Mikko
   Korpela at Aalto University School of Science, Finland, for
   troubleshooting and providing a fix.

 * `logSumExp(c(-Inf, -Inf, ...))` would return NaN rather than
   `-Inf`. Thanks to Jason Xu (University of Washington) for reporting
   and Brennan Vincent for troubleshooting and contributing a fix.


# Version 0.50.1 [2015-12-14]

## Bug Fixes

 * The Undefined Behavior Sanitizer (UBsan) reported on a
     `memcall(src, dest, 0)` call when `dest == null`.  Thanks to Brian
   Ripley and the CRAN check tools for catching this. We could
   reproduce this with gcc 5.1.1 but not with gcc 4.9.2.


# Version 0.50.0 [2015-12-13]

## New Features

 * MAJOR FEATURE UPDATE: Subsetting arguments `idxs`, `rows` and
   `cols` were added to all functions such that the calculations are
   performed on the requested subset while avoiding creating a
   subsetted copy, i.e.  `rowVars(x, cols = 4:6)` is a much faster and
   more memory efficient version than `rowVars(x[, 4:6])` and even yet
   more efficient than `apply(x, MARGIN = 1L, FUN = var)`. These
   features were added by Dongcan Jiang, Peking University, with
   support from the Google Summer of Code program.  A great thank you
   to Dongcan and to Google for making this possible.


# Version 0.15.0 [2015-10-26]

## New Features

 * CONSISTENCY: Now all weight arguments (`w` and `W`) default to
   NULL, which corresponds to uniform weights.

## Code Refactoring

 * ROBUSTNESS: Importing **stats** functions in namespace.

## Bug Fixes

 * `weightedVar(x, w)` used the wrong bias correction factor resulting
   in an estimate that was tau too large, where `tau = ((sum(w) - 1) /
   sum(w)) / ((length(w) - 1) / length(w))`.  Thanks to Wolfgang Abele
   for reporting and troubleshooting on this.

 * `weightedVar(x)` with `length(x) = 1` returned 0 - not NA. Same for
   `weightedSd()`.

 * `weightedMedian(x, w = NA_real_)` returned `x` rather than
   `NA_real_`. This only happened for `length(w) = 1`.

 * `allocArray(dim)` failed for `prod(dim) >= .Machine$integer.max`.

## Deprecated and Defunct

 * CLEANUP: Defunct argument `centers` for `col-`/`rowMads()`; use
   `center`.

 * `weightedVar(x, w, method = "0.14.2")` is deprecated.


# Version 0.14.2 [2015-06-23]

## Bug Fixes

 * `x_OP_y()` and `t_tx_OP_y()` would return garbage on Solaris SPARC
   (and possibly other architectures as well) when input was integer
   and had missing values.


# Version 0.14.1 [2015-06-17]

## Bug Fixes

 * `product(x, na.rm = FALSE)` for integer `x` with both zeros and NAs
   returned zero rather than NA.

 * `weightedMean(x, w, na.rm = TRUE)` did not handle missing values in
   `x` properly, if it was an integer.  It would also return NaN if
   there were weights `w` with missing values, whereas
   `stats::weighted.mean()` would skip such data points.  Now
   `weightedMean()` does the same.

 * `(col|row)WeightedMedians()` did not handle infinite weights as
   `weightedMedian()` does.

 * `x_OP_y(x, y, OP, na.rm = FALSE)` returned garbage iff `x` or `y`
   had missing values of type integer.

 * `rowQuantiles()` and `rowIQRs()` did not work for single-row
   matrices.  Analogously for the corresponding column functions.

 * `rowCumsums()`, `rowCumprods()` `rowCummins()`, and `rowCummaxs()`,
   accessed out-of-bound elements for Nx0 matrices where N > 0.  The
   corresponding column methods has similar memory errors for 0xK
   matrices where K > 0.

 * `anyMissing(list(NULL))` returned NULL; now FALSE.

 * `rowCounts()` resulted in garbage if a previous column had NAs
   (because it forgot to update index kk in such cases).

 * `rowCumprods(x)` handled missing values and zeros incorrectly for
   integer `x` (not double); a zero would trump an existing missing
   value causing the following cumulative products to become zero.  It
   was only a zero that trumped NAs; any other integer would work as
   expected.  Note, this bug was not in `colCumprods()`.

 * `rowAnys(x, value, na.rm = FALSE)` did not handle missing values in
   a numeric `x` properly.  Similarly, for non-numeric and non-logical
   `x`, row- and `colAnys()`, row- and `colAlls()`, `anyValue()` and
   `allValue()` did not handle when `value` was a missing value.

 * All of the above bugs were identified and fixed by Dongcan Jiang (Peking
   University, China), who also added corresponding unit tests.


# Version 0.14.0 [2015-02-13]

## Significant Changes

 * CLEANUP: `anyMissing()` is no longer an S4 generic.  This was done
   as part of the migration of making all functions of **matrixStats**
   plain R functions, which minimizes calling overhead and it will
   also allow us to drop **methods** from the package dependencies.
   I've scanned all CRAN and Bioconductor packages depending on
   **matrixStats** and none of them relied on `anyMissing()` dispatching
   on class, so hopefully this move has little impact. The only
   remaining S4 methods are now `colMedians()` and `rowMedians()`.

## New Features

 * CONSISTENCY: Renamed argument `centers` of `col-`/`rowMads()` to
   `center`.  This is consistent with `col-`/`rowVars()`.

 * CONSISTENCY: `col-`/`rowVars()` now use `na.rm = FALSE` as the
   default (`na.rm = TRUE` was mistakenly introduced as the default in
   v0.9.7).

## Performance and Memory

 * SPEEDUP: The check for user interrupts at the C level is now done
   less frequently of the functions.  It does every k:th iteration,
   where `k = 2^20`, which is tested for using (`iter % k == 0`).  It
   turns out, at least with the default compiler optimization settings
   that I use, that this test is 3 times faster if `k = 2^n` where n is
   an integer.  The following functions checks for user interrupts:
   `logSumExp()`, `(col|row)LogSumExps()`, `(col|row)Medians()`,
   `(col|row)Mads()`, `(col|row)Vars()`, and
   `(col|row)Cum(Min|Max|prod|sum)s()`.

 * SPEEDUP: `logSumExp(x)` is now faster if `x` does not contain any
   missing values.  It is also faster if all values are missing or the
   maximum value is +Inf - in both cases it can skip the actual
   summation step.

## Software Quality

 * ROBUSTNESS/TESTS: Package tests cover 96% of the code (was 91%).

## Code Refactoring

 * CLEANUP: Package no longer depends on **R.methodsS3**.

## Bug Fixes

 * `all()` and `any()` flavored methods on non-numeric and non-logical
   (e.g.  character) vectors and matrices with `na.rm = FALSE` did not
   give results consistent with `all()` and `any()` if there were
   missing values.  For example, with `x <- c("a", NA, "b")` we have
   `all(x == "a") == FALSE` and `any(x == "a") == TRUE`, whereas our
   corresponding methods would return NA in those cases.  The methods
   fixed are `allValue()`, `anyValue()`, `col-`/`rowAlls()`, and
   `col-`/`rowAnys()`.  Added more package tests to cover these cases.

 * `logSumExp(x, na.rm = TRUE)` would return NA if all values were NA
   and `length(x) > 1`.  Now it returns -Inf for all `length(x)`:s.


# Version 0.13.1 [2015-01-21]

## Bug Fixes

 * `diff2()` with `differences >= 3` would _read_ spurious values
   beyond the allocated memory.  This error, introduced in 0.13.0, was
   harmless in the sense that the returned value was unaffected and
   still correct.  Thanks to Brian Ripley and the CRAN check tools for
   catching this.  I could reproduce it locally with valgrind.


# Version 0.13.0 [2015-01-20]

## Significant Changes

 * SPEEDUP/CLEANUP: Turned several S3 and S4 methods into plain R
   functions, which decreases the overhead of calling the functions.
   After this there are no longer any S3 methods.  Remaining S4
   methods are `anyMissing()` and `rowMedians()`.

## New Features

 * Added `weightedMean()`, which is ~10 times faster than
   `stats::weighted.mean()`.

 * Added `count(x, value)` which is a notably faster than `sum(x ==
   value)`.  This can also be used to count missing values etc.

 * Added `allValue()` and `anyValue()` for `all(x == value)` and
   `any(x == value)`.

 * Added `diff2()`, which is notably faster than `base::diff()` for
   vectors, which it is designed for.

 * Added `iqrDiff()` and `(col|row)IqrDiffs()`.

 * CONSISTENCY: Now `rowQuantiles(x, na.rm = TRUE)` returns all NAs
   for rows with missing values.  Analogously for `colQuantiles()`,
   `colIQRs()`, `rowIQRs()` and `iqr()`.  Previously, all these
   functions gave an error saying missing values are not allowed.

 * COMPLETENESS: Added corresponding "missing" vector functions for
   already existing column and row functions.  Similarly, added
   "missing" column and row functions for already existing vector
   functions, e.g. added `iqr()` and `count()` to complement already
   existing `(col|row)IQRs()` and `(col|row)Counts()` functions.

 * ROBUSTNESS: Now column and row methods give slightly more
   informative error messages if a data.frame is passed instead of a
   matrix.

## Documentation

 * Added vignette summarizing available functions.

## Performance and Memory

 * SPEEDUP: `(col|row)Diffs()` are now implemented in native code and
   notably faster than `diff()` for matrices.

 * SPEEDUP: Made `binCounts()` and `binMeans()` a bit faster.

 * SPEEDUP: Implemented `weightedMedian()` in native code, which made
   it ~3-10 times faster.  Dropped support for `ties = "both"`,
   because it would have to return two values in case of ties, which
   made the API unnecessarily complicated.  If really needed, then
   call the function twice with `ties = "min"` and `ties = "max"`.

 * SPEEDUP: `(col|row)Anys()` and `(col|row)Alls()` is now notably
   faster compared to previous versions.

## Code Refactoring

 * CLEANUP: In the effort of migrating `anyMissing()` into a plain R
   function, the specific `anyMissing()` implementations for
   data.frame:s and and list:s were dropped and is now handled by
   `anyMissing()` for `"ANY"`, which is the only S4 method remaining
   now.  In a near future release, this remaining `"ANY"` method will
   turned into a plain R function and the current S4 generic will be
   dropped.  We know of no CRAN and Bioconductor packages that rely on
   it being a generic function.  Note also that since R (>= 3.1.0)
   there is a `base::anyNA()` function that does the exact same thing
   making `anyMissing()` obsolete.

## Bug Fixes

 * `weightedMedian(..., ties = "both")` would give an error if there
   was a tie.  Added package test for this case.

## Deprecated and Defunct

 * `weightedMedian(..., ties = "both")` is now defunct.


# Version 0.12.2 [2014-12-07]

## Bug Fixes

 * CODE FIX: The native code for `product()` on integer vector
   incorrectly used C-level `abs()` on intermediate values despite
   those being doubles requiring `fabs()`.  Despite this, the
   calculated product would still be correct (at least when validated
   on several local setups as well as on the CRAN servers).  Again,
   thanks to Brian Ripley for pointing out another invalid
   integer-double coercion at the C level.

## Deprecated and Defunct

 * `weightedMedian(..., interpolate = FALSE, ties = "both")` is
   defunct.


# Version 0.12.1 [2014-12-06]

## Software Quality

 * ROBUSTNESS: Updated package tests to check methods in more scenarios,
   especially with both integer and numeric input data.

## Bug Fixes

 * `(col|row)Cumsums(x)` where `x` is integer would return garbage for
   columns (rows) containing missing values.

 * `rowMads(x)` where `x` is numeric (not integer) would give
   incorrect results for rows that had an _odd_ number of values (no
   ties).  Analogously issues with `colMads()`. Added package tests
   for such cases too.  Thanks to Brian Ripley and the CRAN check
   tools for (yet again) catching another coding mistake.  Details:
   This was because the C-level calculation of the absolute value of
   residuals toward the median would use integer-based `abs()` rather
   than double-based `fabs()`. Now it `fabs()` is used when the values
   are double and `abs()` when they are integers.


# Version 0.12.0 [2014-12-05]

 * Submitted to CRAN.


# Version 0.11.9 [2014-11-26]

## New Features

 * Added `(col|row)Cumsums()`, `(col|row)Cumprods()`,
   `(col|row)Cummins()`, and `(col|row)Cummaxs()`.

## Bug Fixes

 * `(col|row)WeightedMeans()` with all zero weights gave mean
   estimates with values 0 instead of NaN.


# Version 0.11.8 [2014-11-25]

## Performance and Memory

 * SPEEDUP: Implemented `(col|row)Mads()`, `(col|row)Sds()`, and
   `(col|row)Vars()` in native code.

 * SPEEDUP: Made `(col|row)Quantiles(x)` faster for `x` without
   missing values (and default `type = 7L` quantiles).  It should
   still be implemented in native code though.

 * SPEEDUP: Made `rowWeightedMeans()` faster.

## Bug Fixes

 * `(col|row)Medians(x)` when `x` is integer would give invalid median
   values in case (a) it was calculated as the mean of two values
   ("ties"), and (b) the sum of those values where greater than
   `.Machine$integer.max`.  Now such ties are calculated using
   floating point precision.  Add lots of package tests.


# Version 0.11.6 [2014-11-16]

## Performance and Memory

 * SPEEDUP: Now `(col|row)Mins()`, `(col|row)Maxs()`, and
   `(col|row)Ranges()` are implemented in native code providing a
   significant speedup.

 * SPEEDUP: Now `colOrderStats()` also is implemented in native code,
   which indirectly makes `colMins()`, `colMaxs()` and `colRanges()`
   faster.

 * SPEEDUP: `colTabulates(x)` no longer uses `rowTabulates(t(x))`.

 * SPEEDUP: `colQuantiles(x)` no longer uses `rowQuantiles(t(x))`.

## Deprecated and Defunct

 * CLEANUP: Argument `flavor` of `(col|row)Ranks()` is now ignored.


# Version 0.11.5 [2014-11-15]

## Significant Changes

 * `(col|row)Prods()` now uses default `method = "direct"` (was
   `"expSumLog"`).

## Performance and Memory

 * SPEEDUP: Now `colCollapse(x)` no longer utilizes
   `rowCollapse(t(x))`.  Added package tests for `(col|row)Collapse()`.

 * SPEEDUP: Now `colDiffs(x)` no longer uses `rowDiffs(t(x))`. Added
   package tests for `(col|row)Diffs()`.

 * SPEEDUP: Package no longer utilizes `match.arg()` due to its
   overhead; methods `sumOver()`, `(col|row)Prods()` and
   `(col|row)Ranks()` were updated.


# Version 0.11.4 [2014-11-14]

## New Features

 * Added support for vector input to several of the row- and column
   methods as long as the "intended" matrix dimension is specified via
   argument `dim`.  For instance, `rowCounts(x, dim = c(nrow, ncol))`
   is the same as `rowCounts(matrix(x, nrow, ncol))`, but more
   efficient since it avoids creating/allocating a temporary matrix.

## Performance and Memory

 * SPEEDUP: Now `colCounts()` is implemented in native code.
   Moreover, `(col|row)Counts()` are now also implemented in native
   code for logical input (previously only for integer and double
   input).  Added more package tests and benchmarks for these
   functions.


# Version 0.11.3 [2014-11-11]

## Significant Changes

 * Turned `sdDiff()`, `madDiff()`, `varDiff()`, `weightedSd()`,
   `weightedVar()` and `weightedMad()` into plain functions (were
   generic functions).

## Code Refactoring

 * Removed unnecessary usage of `::`.


# Version 0.11.2 [2014-11-09]

## Significant Changes

 * SPEEDUP: Implemented `indexByRow()` in native code and it is no
   longer a generic function, but a regular function, which is also
   faster to call.  The first argument of `indexByRow()` has been
   changed to `dim` such that one should use `indexByRow(dim(X))`
   instead of `indexByRow(X)` as in the past.  The latter form is
   still supported, but deprecated.

## New Features

 * Added `allocVector()`, `allocMatrix()`, and `allocArray()` for
   faster allocation numeric vectors, matrices and arrays,
   particularly when filled with non-missing values.

## Deprecated and Defunct

 * Calling `indexByRow(X)` with a matrix `X` is deprecated.  Instead
   call it with `indexByRow(dim(X))`.


# Version 0.11.1 [2014-11-07]

## New Features

 * Better support for long vectors.

 * PRECISION: Using greater floating-point precision in more internal
   intermediate calculations, where possible.

## Software Quality

 * ROBUSTNESS: Although unlikely, with long vectors support for
   `binCounts()` and `binMeans()` it is possible that a bin gets a
   higher count than what can be represented by an R integer
   (`.Machine$integer.max = 2^31-1`).  If that happens, an informative
   warning is generated and the bin count is set to
   `.Machine$integer.max`.  If this happens for `binMeans()`, the
   corresponding mean is still properly calculated and valid.

## Code Refactoring

 * CLEANUP: Cleanup and harmonized the internal C API such there are
   two well defined API levels.  The high-level API is called by R via
   `.Call()` and takes care of most of the argument validation and
   construction of the return value.  This function dispatch to
   functions in the low-level API based on data type(s) and other
   arguments.  The low-level API is written to work with basic C data
   types only.

## Bug Fixes

 * Package incorrectly redefined `R_xlen_t` on R (>= 3.0.0) systems
   where `LONG_VECTOR_SUPPORT` is not supported.


# Version 0.11.0 [2014-11-02]

## New Features

 * Added `sumOver()` and `meanOver()`, which are notably faster
   versions of `sum(x[idxs])` and `mean(x[idxs])`.  Moreover, instead
   of having to do `sum(as.numeric(x))` to avoid integer overflow when
   `x` is an integer vector, one can do `sumOver(x, mode =
   "numeric")`, which avoids the extra copy created when coercing to
   numeric (this numeric copy is also twice as large as the integer
   vector).  Added package tests and benchmark reports for these
   functions.


# Version 0.10.4 [2014-11-01]

## Performance and Memory

 * SPEEDUP: Made `anyMissing()`, `logSumExp()`, `(col|row)Medians()`,
   and `(col|row)Counts()` slightly faster by making the native code
   assign the results directly to the native vector instead of to the
   R vector, e.g. `ansp[i] = v` where `ansp = REAL(ans)` instead of
   `REAL(ans)[i] = v`.

 * Added benchmark reports for `anyMissing()` and `logSumExp()`.


# Version 0.10.3 [2014-10-01]

## Bug Fixes

 * `binMeans()` returned 0.0 instead of `NA_real_` for empty bins.


# Version 0.10.2 [2014-09-01]

## Bug Fixes

 * On some systems, the package failed to build on R (<= 2.15.3) with
   compilation error: `"redefinition of typedef 'R_xlen_t'"`.


# Version 0.10.1 [2014-06-09]

## Performance and Memory

 * Added benchmark reports for also non-**matrixStats** functions
   `col-`/`rowSums()` and `col-`/`rowMeans()`.

 * Now all `colNnn()` and `rowNnn()` methods are benchmarked in a
   combined report making it possible to also compare `colNnn(x)` with
   `rowNnn(t(x))`.


# Version 0.10.0 [2014-06-07]

## Software Quality

 * Relaxed some packages tests such that they assert numerical
   correctness via `all.equal()` rather than `identical()`.

 * Submitted to CRAN.

## Bug Fixes

 * The package tests for `product()` incorrectly assumed that the
   value of `prod(c(NaN, NA))` is uniquely defined.  However, as
   documented in `help("is.nan")`, it may be NA or NaN depending on R
   system/platform.


# Version 0.9.7 [2014-06-05]

## Bug Fixes

 * Introduced a bug in v0.9.5 causing `col-`/`rowVars()` and hence
   also `col-`/`rowSds()` to return garbage.  Add package tests for
   these now.

 * Submitted to CRAN.


# Version 0.9.6 [2014-06-04]

## New Features

 * Added `signTabulate()` for tabulating the number of negatives,
   zeros, positives and missing values.  For doubles, the number of
   negative and positive infinite values are also counted.

## Performance and Memory

 * SPEEDUP: Now `col-`/`rowProds()` utilizes new `product()` function.

 * SPEEDUP: Added `product()` for calculating the product of a numeric
   vector via the logarithm.


# Version 0.9.5 [2014-06-04]

## Significant Changes

 * SPEEDUP: Made `weightedMedian()` a plain function (was an S3
   method).

 * CLEANUP: Now only exporting plain functions and generic functions.

 * SPEEDUP: Turned more S4 methods into S3 methods,
   e.g. `rowCounts()`, `rowAlls()`, `rowAnys()`, `rowTabulates()` and
   `rowCollapse()`.

## New Features

 * Added argument `method` to `col-`/`rowProds()` for controlling how
   the product is calculated.

## Performance and Memory

 * SPEEDUP: Package is now byte compiled.

 * SPEEDUP: Made `rowProds()` and `rowTabulates()` notably faster.

 * SPEEDUP: Now `rowCounts()`, `rowAnys()`, `rowAlls()` and
   corresponding column methods can search for any value in addition
   to the default TRUE.  The search for a matching integer or double
   value is done in native code, which is notably faster (and more
   memory efficient because it avoids creating any new objects).

 * SPEEDUP: Made `colVars()` and `colSds()` notably faster and
   `rowVars()` and `rowSds()` a slightly bit faster.

 * Added benchmark reports, e.g. `matrixStats:::benchmark("colMins")`.


# Version 0.9.4 [2014-05-23]

## Significant Changes

 * SPEEDUP: Turned several S4 methods into S3 methods,
   e.g. `indexByRow()`, `madDiff()`, `sdDiff()` and `varDiff()`.


# Version 0.9.3 [2014-04-26]

## New Features

 * Added argument `trim` to `madDiff()`, `sdDiff()` and `varDiff()`.


# Version 0.9.2 [2014-04-04]

## Bug Fixes

 * The native code of `binMeans(x, bx)` would try to access an
   out-of-bounds value of argument `y` iff `x` contained elements that
   are left of all bins in `bx`.  This bug had no impact on the
   results and since no assignment was done it should also not
   crash/core dump R.  This was discovered thanks to new memtests
   (ASAN and valgrind) provided by CRAN.


# Version 0.9.1 [2014-03-31]

## Bug Fixes

 * `rowProds()` would throw `"Error in rowSums(isNeg) : `x` must be an
   array of at least two dimensions"` on matrices where all rows
   contained at least one zero.  Thanks to Roel Verbelen at KU Leuven
   for the report.


# Version 0.9.0 [2014-03-26]

## New Features

 * Added `weighedVar()` and `weightedSd()`.


# Version 0.8.14 [2013-11-23]

## Performance and Memory

 * MEMORY: Updated all functions to do a better job of cleaning out
   temporarily allocated objects as soon as possible such that the
   garbage collector can remove them sooner, iff wanted.  This
   increase the chance for a smaller memory footprint.

 * Submitted to CRAN.


# Version 0.8.13 [2013-10-08]

## New Features

 * Added argument `right` to `binCounts()` and `binMeans()` to specify
   whether binning should be done by (u,v] or [u,v).  Added system
   tests validating the correctness of the two cases.

## Code Refactoring

 * Bumped up package dependencies.


# Version 0.8.12 [2013-09-26]

## Performance and Memory

 * SPEEDUP: Now utilizing `anyMissing()` everywhere possible.


# Version 0.8.11 [2013-09-21]

## Software Quality

 * ROBUSTNESS: Now importing `loadMethod` from **methods** package
   such that **matrixStats** S4-based methods also work when
   **methods** is not loaded, e.g.  when `Rscript` is used,
   cf. Section 'Default packages' in 'R Installation and
   Administration'.

 * ROBUSTNESS: Updates package system tests such that the can run with
   only the **base** package loaded.


# Version 0.8.10 [2013-09-15]

## Code Refactoring

 * CLEANUP: Now only importing two functions from the **methods**
   package.

 * Bumped up package dependencies.


# Version 0.8.9 [2013-08-29]

## New Features

 * CLEANUP: Now the package startup message acknowledges argument
   `quietly` of `library()`/`require()`.


# Version 0.8.8 [2013-07-29]

## Documentation

 * The dimension of the return value was swapped in
   `help("rowQuantiles")`.


# Version 0.8.7 [2013-07-28]

## Performance and Memory

 * SPEEDUP: Made `(col|row)Mins()` and `(col|row)Maxs()` much faster.

## Bug Fixes

 * `rowRanges(x)` on an Nx0 matrix would give an error.  Same for
   `colRanges(x)` on an 0xN matrix.  Added system tests for these and
   other special cases.


# Version 0.8.6 [2013-07-20]

## Code Refactoring

 * Bumped up package dependencies.

## Bug Fixes

 * Forgot to declare S3 methods `(col|row)WeightedMedians()`.


# Version 0.8.5 [2013-05-25]

## Performance and Memory

 * Minor speedup of `(col|row)Tabulates()` by replacing `rm()` calls
   with NULL assignments.


# Version 0.8.4 [2013-05-20]

## Documentation

 * CRAN POLICY: Now all Rd `\usage{}` lines are at most 90 characters
   long.


# Version 0.8.3 [2013-05-10]

## Performance and Memory

 * SPEEDUP: `binCounts()` and `binMeans()` now uses Hoare's Quicksort for
   presorting `x` before counting/averaging.  They also no longer test in
   every iteration (== for every data point) whether the last bin has been
   reached or not, but only after completing a bin.


# Version 0.8.2 [2013-05-02]

## Documentation

 * Minor corrections and updates to help pages.


# Version 0.8.1 [2013-05-02]

## Bug Fixes

 * Native code of `logSumExp()` used an invalid check for missing
   value of an integer argument.  Detected by Brian Ripley upon CRAN
   submission.


# Version 0.8.0 [2013-05-01]

## New Features

 * Added `logSumExp(lx)` and `(col|row)LogSumExps(lx)` for accurately
   computing of `log(sum(exp(lx)))` for standalone vectors, and row
   and column vectors of matrices. Thanks to Nakayama (Japan) for the
   suggestion and contributing a draft in R.


# Version 0.7.1 [2013-04-23]

## New Features

 * Added argument `preserveShape` to `colRanks()`. For backward
   compatibility the default is `preserveShape = FALSE`, but it may
   change in the future.

## Bug Fixes

 * Since v0.6.4, `(col|row)Ranks()` gave the incorrect results for
   integer matrices with missing values.

 * Since v0.6.4, `(col|row)Medians()` for integers would calculate
   ties as `floor(tieAvg)`.


# Version 0.7.0 [2013-01-14]

## New Features

 * Now `(col|row)Ranks()` support `"max"` (default), `"min"` and
   `"average"` for argument `ties.method`.  Added system tests
   validation these cases.  Thanks Peter Langfelder (UCLA) for
   contributing this.


# Version 0.6.4 [2013-01-13]

## New Features

 * Added argument `ties.method` to `rowRanks()` and `colRanks()`, but
   still only support for `"max"` (as before).

## Code Refactoring

 * ROBUSTNESS: Lots of cleanup of the internal/native code.  Native code for
   integer and double cases have been harmonized and are now generated from a
   common code template.  This was inspired by code contributions from Peter
   Langfelder (UCLA).


# Version 0.6.3 [2013-01-13]

## New Features

 * Added `anyMissing()` for data type `raw`, which always returns FALSE.

## Software Quality

 * ROBUSTNESS: Added system test for `anyMissing()`.

 * ROBUSTNESS: Now S3 methods are declared in the namespace.


# Version 0.6.2 [2012-11-15]

## Software Quality

 * CRAN POLICY: Made `example(weightedMedian)` faster.


# Version 0.6.1 [2012-10-10]

## Bug Fixes

 * In some cases `binCounts()` and `binMeans()` could try to go past
   the last bin resulting a core dump.

 * `binCounts()` and `binMeans()` would return random/garbage values
   for bins that were beyond the last data point.


# Version 0.6.0 [2012-10-04]

## New Features

 * Added `binMeans()` for fast sample-mean calculation in bins.
   Thanks to Martin Morgan at the Fred Hutchinson Cancer Research
   Center, Seattle, for contributing the core code for this.

 * Added `binCounts()` for fast element counting in bins.


# Version 0.5.3 [2012-09-10]

## Software Quality

 * CRAN POLICY: Replaced the `.Internal(psort(...))` call with a call
   to a new internal partial sorting function, which utilizes the
   native `rPsort()` part of the R internals.


# Version 0.5.2 [2012-07-02]

## Code Refactoring

 * Updated package dependencies to match CRAN.


# Version 0.5.1 [2012-06-25]

## New Features

 * GENERALIZATION: Now `(col|row)Prods()` handle missing values.

## Code Refactoring

 * Package now only imports the **methods** package.

## Bug Fixes

 * In certain cases, `(col|row)Prods()` would return NA instead of 0
   for some elements.  Added a redundancy test for the case.  Thanks
   Brenton Kenkel at University of Rochester for reporting on this.


# Version 0.5.0 [2012-04-16]

## New Features

 * Added `weightedMad()` from **aroma.core** v2.5.0.

 * Added `weightedMedian()` from **aroma.light** v1.25.2.

## Code Refactoring

 * This package no longer depends on the **aroma.light** package for
   any of its functions.

 * Now this package only imports **R.methodsS3**, meaning it no longer
   loads **R.methodsS3** when it is loaded.


# Version 0.4.5 [2012-03-19]

## New Features

 * Updated the default argument `centers` of `rowMads()`/`colMads()`
   to explicitly be `(col|row)Medians(x,...)`.  The default behavior
   has not changed.


# Version 0.4.4 [2012-03-05]

## Software Quality

 * ROBUSTNESS: Added system/redundancy tests for
   `rowMads()`/`colMads()`.

 * CRAN: Made the system tests "lighter" by default, but full tests
   can still be run, cf. `tests/*.R` scripts.

## Bug Fixes

 * `colMads()` would return the incorrect estimates. This bug was
   introduced in **matrixStats** v0.4.0 (2011-11-11).


# Version 0.4.3 [2011-12-11]

## Bug Fixes

 * `rowMedians(..., na.rm = TRUE)` did not handle NaN (only NA).  The
   reason for this was the the native code used `ISNA()` to test for
   NA and NaN, but it should have been `ISNAN()`, which is opposite to
   how `is.na()` and `is.nan()` at the R level work.  Added system
   tests for this case.


# Version 0.4.2 [2011-11-29]

## New Features

 * Added `rowAvgsPerColSet()` and `colAvgsPerRowSet()`.


# Version 0.4.1 [2011-11-25]

## Documentation

 * Added help pages with an example to `rowIQRs()` and `colIQRs()`.

 * Added example to `rowQuantiles()`.

## Bug Fixes

 * `rowIQRs()` and `colIQRs()` would return the 25% and the 75%
   quantiles, not the difference between them.  Thanks Pierre Neuvial
   at CNRS, Evry, France for the report.


# Version 0.4.0 [2011-11-11]

## Significant Changes

 * Dropped the previously introduced expansion of `center` in
   `rowMads()` and `colMads()`.  It added unnecessary overhead if not
   needed.

## New Features

 * Added `rowRanks()` and `colRanks()`.  Thanks Hector Corrada Bravo
   (University of Maryland) and Harris Jaffee (John Hopkins).


# Version 0.3.0 [2011-10-13]

## Performance and Memory

 * SPEEDUP/LESS MEMORY: `colMedians(x)` no longer uses
   `rowMedians(t(x))`; instead there is now an optimized native-code
   implementation.  Also, `colMads()` utilizes the new `colMedians()`
   directly.  This improvement was kindly contributed by Harris Jaffee
   at Biostatistics of John Hopkins, USA.

## Software Quality

 * Added additional unit tests for `colMedians()` and `rowMedians()`.


# Version 0.2.2 [2010-10-06]

## New Features

 * Now the result of `(col|row)Quantiles()` contains column names.


# Version 0.2.1 [2010-04-05]

## New Features

 * Added a startup message when package is loaded.

## Code Refactoring

 * CLEANUP: Removed obsolete internal `.First.lib()` and
   `.Last.lib()`.


# Version 0.2.0 [2010-03-30]

## Documentation

 * Fixed some incorrect cross references.


# Version 0.1.9 [2010-02-03]

## Bug Fixes

 * `(col|row)WeightedMeans(..., na.rm = TRUE)` would incorrectly treat
   missing values as zeros.  Added corresponding redundancy tests
   (also for the median case).  Thanks Pierre Neuvial for reporting
   this.


# Version 0.1.8 [2009-11-13]

## Bug Fixes

 * `colRanges(x)` would return a matrix of wrong dimension if `x` did
   not have any missing values.  This would affect all functions
   relying on `colRanges()`, e.g. `colMins()` and `colMaxs()`.  Added
   a redundancy test for this case.  Thanks Pierre Neuvial at UC
   Berkeley for reporting this.

 * `(col|row)Ranges()` return a matrix with dimension names.


# Version 0.1.7 [2009-06-20]

## Bug Fixes

 * WORKAROUND: Cannot use `"%#x"` in `rowTabulates()` when creating
   the column names of the result matrix.  It gave an error OSX with R
   v2.9.0 devel (2009-01-13 r47593b) current the OSX server at
   R-forge.


# Version 0.1.6 [2009-06-17]

## Documentation

 * Updated the help example for `rowWeightedMedians()` to run
   conditionally on **aroma.light**, which is only a suggested
   package - not a required one.  This in order to prevent `R CMD
   check` to fail on CRAN, which prevents it for building binaries (as
   it currently happens on their OSX servers).


# Version 0.1.5 [2009-02-04]

## Bug Fixes

 * For some errors in `rowOrderStats()`, the stack would not become
   UNPROTECTED before calling error.


# Version 0.1.4 [2009-02-02]

## New Features

 * Added methods `(col|row)Weighted(Mean|Median)s()` for weighted
   averaging.

## Documentation

 * Added help to more functions.

## Software Quality

 * Package passes `R CMD check` flawlessly.


# Version 0.1.3 [2008-07-30]

## New Features

 * Added `(col|row)Tabulates()` for integer and raw matrices.

## Bug Fixes

 * `rowCollapse()` was broken and returned the wrong elements.


# Version 0.1.2 [2008-04-13]

## New Features

 * Added `(col|row)Collapse()`.

 * Added `varDiff()`, `sdDiff()`, and `madDiff()`.

 * Added `indexByRow()`.


# Version 0.1.1 [2008-03-25]

## New Features

 * Added `(col|row)OrderStats()`.

 * Added `(col|row)Ranges()` and `(col|row)(Min|Max)s()`.

 * Added `colMedians()`.

 * Now `anyMissing()` support most data types as structures.


# Version 0.1.0 [2007-11-26]

## New Features

 * Imported the `rowNnn()` methods from **Biobase**.

 * Created.
