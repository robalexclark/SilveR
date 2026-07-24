hassediagrams: generates the layout structure and restricted layout
structure of experimental designs
================
Damianos Michaelides, Simon Bate, Marion Chatfield

## Overview

The `hassediagrams` package provides tools to visualize the **structure
of experimental designs** using Hasse diagrams. The package determines
the structure of the design, summarised by the layout structure, and
uses this structure to generate a Hasse diagram. This diagram describes
the structure of the design and the relationships between the factors
that define the design. By considering the randomisation performed, in
conjunction with the layout structure, a list of randomisation objects
can be identified, known as the restricted layout structure. The package
can also be used to generate a Hasse Diagram of this restricted layout
structure. Objects in the restricted layout structure can be used to
identify the terms to include in the statistical model.

The package is an implementation of the methodology described in Bate
and Chatfield (2016a and 2016b).

Bate S.T., Chatfield M.J. (2016a). “Identifying the structure of the
experimental design.” Journal of Quality Technology, 48(4), 343–364.

Bate S.T., Chatfield M.J. (2016b). “Using the structure of the
experimental design and the randomization to construct a mixed model.”
Journal of Quality Technology, 48(4), 365–387.

## Features

### Functions

`hasselayout()`

Creates a Hasse diagram of the layout structure. To generate the diagram
all this is needed is a dataset consisting of the experimental factors
that define the experimental design.

`hasserls()`

Creates the Hasse diagram of the restricted layout structure. Inputs to
the function are the dataset consisting of the experimental factors, a
vector that defines the randomisation objects and a matrix that defines
the randomisation arrows.

## Installation

### Install from GitHub (Development Version)

The development version of `hassediagrams` can be installed with:

``` r
# Install devtools (if not already installed)
install.packages("devtools")

# Install hassediagrams from GitHub
devtools::install_github("GSK-Biostatistics/hassediagrams")
```

### Install from CRAN (when made available)

``` r
install.packages("hassediagrams")
```

## Usage

Load the package:

``` r
library(hassediagrams)
```

## Example: Generate Hasse Diagrams of the Layout Structure and Restricted Layout Structure

### A fractional factorial design for investigating asphalt concrete production

#### Generate the Hasse diagram of the layout structure

``` r
ls_concrete <- hasselayout(datadesign=concrete, 
                           larger.fontlabelmultiplier=1.6, 
                           smaller.fontlabelmultiplier=1.3)
```

#### Generate the Hasse diagram of the restricted layout structure

``` r
ls_concrete$str_objects
rand_spec <- ls_concrete$rand_template
rand_spec[] <- ls_concrete$str_objects       
rand_spec[length(rand_spec)] <- "AC^AG^CC^CoT^CuT --> Run"

hasserls(datadesign = concrete,
         rand.objects = rand_spec,
         larger.fontlabelmultiplier = 1.6,
         smaller.fontlabelmultiplier = 1.3)
```

## Documentation

For an introduction to the methodology, check the package vignette:

``` r
vignette("Introduction_to_hassediagrams")
```

and for an introduction to the package implementation visit the
documentation.

## Maintenance

This package is in a stable state and will only be updated for bug
fixes.

The person responsible for monitoring this package is Simon Bate
<simon.t.bate@gsk.com> and Damianos Michaelides <dm3g15@soton.ac.uk>.
