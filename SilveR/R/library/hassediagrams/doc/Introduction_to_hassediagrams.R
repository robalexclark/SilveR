## ----five_stage_process, echo=FALSE-------------------------------------------
library(kableExtra)
library(knitr)

# Create the table
data <- data.frame(
  Stage = c("1", "2", "3", "4", "", "5"),
  Outcome = c(
    "A list of factors", "Factor levels", "Experiment Scheme", 
    "Layout Structure", "Hasse diagram of the Layout Structure", "Degrees of freedom"
  ),
  Purpose = c(
    "Researcher should aim to identify the factors that are a) ‘of interest’ and/or b) inherent to the experimental material or c) additional potential sources of variability.",
    "To define the factor levels – this is required so that the structure of the experimental design can be identified algorithmically.",
    "The researcher defines the properties the experimental design should have, identify the class of design they wish to use and potentially the non-randomised experimental design itself.",
    "To summarise the relationships between the factors in the experimental design to help identify the structure of the experimental design.",
    "To visualise the structure of the experimental design.",
    "To identify potential weaknesses in the design (i.e. lack of replication) and degrees of freedom in a statistical analysis using a model based on the factors/generalised factors in the Layout Structure."
  ),
  Properties = c(
    "The list must contain all factors the researcher wishes to include in the experimental design.",
    "For each factor, the factor levels should be specified uniquely.",
    "A list of the factors and factor levels that define the design, a list of the properties the design should have which should respect the relationships between inherent factors, the choice of design class and, if possible, the non-randomised design.",
    "A list of factors and generalised factors and the relationships between the factors.",
    "A diagram illustrating the crossed/nested/equivalent relationships between the factors/generalised factors in the design.",
    "For each factor/generalised factor the number of levels of the factor, the number of levels present in the design and the associated degrees of freedom."
  ),
  Derived_from = c(
    "Hypotheses to be tested and background knowledge of the experimental material.",
    "Background knowledge of the practicalities of the experiment and how the experiment will be managed.",
    "The factors, factor levels and background knowledge of the experimental material.",
    "Factors, factor levels, nested/crossed/equivalent relationships between the factors as defined in the Experiment Scheme and the experimental design plan (generated using the Experimental Factors table and Layout Structure table).",
    "Layout Structure.",
    "Factor levels, experimental design and Hasse diagram."
  )
)

# Render the table
knitr::kable(
  data,
  format = "html",
  booktabs = TRUE,
  escape = FALSE,
  col.names = c(
    "Stage",
    "Outcome",
    "Purpose of stage",
    "Properties of the outcomes from the stage",
    "What the stage outcomes are derived from"
  )
) %>%
kable_styling(full_width = TRUE, bootstrap_options = c("striped", "hover", "condensed", "responsive")) %>%
    row_spec(0, bold = TRUE) %>%  # Make header bold
    kable_classic(full_width = TRUE, html_font = "Arial") %>%  # Classic table style
    column_spec(1:5, border_left = TRUE, border_right = TRUE) %>% # Add borders
    row_spec(0, extra_css = "border: 1px solid black;") %>%  # Ensure full borders
    row_spec(0:nrow(data), extra_css = "border: 1px solid black;")  # Add horizontal borders

## ----splitplot_table, echo=FALSE----------------------------------------------
library(kableExtra)
library(knitr)

# Define the table structure with correct math notation
table_data <- data.frame(
  Block = c("**Block I**", "", "$r_1$", "$r_2$", "$r_3$", "$r_4$", "", 
            "**Block II**", "", "$r_1$", "$r_2$", "$r_3$", "$r_4$", "", 
            "**Block III**", "", "$r_1$", "$r_2$", "$r_3$", "$r_4$"),
  c1 = c("", "$c_1$", "$a_0, b_0$", "$a_0, b_1$", "$a_0, b_2$", "$a_0, b_3$", "", 
         "", "$c_1$", "$a_1, b_3$", "$a_1, b_0$", "$a_1, b_1$", "$a_1, b_2$", "", 
         "", "$c_1$", "$a_2, b_2$", "$a_2, b_3$", "$a_2, b_0$", "$a_2, b_1$"),
  c2 = c("", "$c_2$", "$a_1, b_0$", "$a_1, b_1$", "$a_1, b_2$", "$a_1, b_3$", "", 
         "", "$c_2$", "$a_2, b_3$", "$a_2, b_0$", "$a_2, b_1$", "$a_2, b_2$", "", 
         "", "$c_2$", "$a_0, b_2$", "$a_0, b_3$", "$a_0, b_0$", "$a_0, b_1$"),
  c3 = c("", "$c_3$", "$a_2, b_0$", "$a_2, b_1$", "$a_2, b_2$", "$a_2, b_3$", "", 
         "", "$c_3$", "$a_0, b_3$", "$a_0, b_0$", "$a_0, b_1$", "$a_0, b_2$", "", 
         "", "$c_3$", "$a_1, b_2$", "$a_1, b_3$", "$a_1, b_0$", "$a_1, b_1$"),
  stringsAsFactors = FALSE
)

knitr::kable(table_data, format = "html", escape = FALSE, col.names = NULL) %>%
  kable_styling(full_width = FALSE, position = "center") %>%
  row_spec(which(table_data$Block %in% c("**Block I**", "**Block II**", "**Block III**")), 
           bold = TRUE, extra_css = "text-align: center; border: 1px solid black;") %>%
  column_spec(1:4, border_left = TRUE, border_right = TRUE) %>%  # Remove align argument
  row_spec(1:nrow(table_data), extra_css = "border: 1px solid black; text-align: center;")

## ----hasse_diagram_ls, echo=FALSE, out.width='80%', fig.align='center', fig.cap="Hasse diagram of the Restricted Layout Structure for the Split-Plot Split-Block Experiment."----
knitr::include_graphics("figures/SM2F17_output.png")

## ----restricted_layout_table, echo=FALSE--------------------------------------
library(kableExtra)
library(knitr)

# Define table data
table_data <- data.frame(
  Stage = c("6", "", "7", "", "", "8"),
  Components = c(
    "Randomisation", "Description of the randomisation", 
    "Categorisation of factors as fixed or random", "Restricted Layout Structure", 
    "Hasse diagram of the Restricted Layout Structure", "The mixed model formation"
  ),
  Purpose = c(
    "Randomise the experimental material.", 
    "Defining the randomisation in terms of randomisation objects to aid in model selection.",
    "To aid in the analysis and also model selection.",
    "To construct a list of the randomisation objects (those involved in the randomisation and the randomisation objects that nest them) to aid in model development.",
    "To visualise the structure of the experiment as implied by the randomisation performed.",
    "Identify the terms to include in the mixed model."
  ),
  Properties = c(
    "A randomised plan of the experimental design.",
    "A list of randomisation objects that define the randomisation performed with randomisation arrows connecting them (randomisation objects 'involved in the randomisation').",
    "A list of random factors and a list of fixed factors.",
    "A list of randomisation objects.",
    "A diagram illustrating the crossed and nested relationship between the randomisation objects.",
    "A list of model terms."
  ),
  Derived_From = c(
    "The randomisation performed and the un-randomised experimental design.",
    "Knowledge of the randomisation performed.",
    "Knowledge of the properties of the factors, the hypotheses being tested and the nesting of factors in the design.",
    "The Layout Structure and the randomisation object pairs that are connected by randomisation arrows.",
    "Layout Structure and the Restricted Layout Structure.",
    "The objects in the Restricted Layout Structure."
  ),
  stringsAsFactors = FALSE
)

# Render the table
knitr::kable(
  table_data, format = "html", escape = FALSE, 
  col.names = c("Stage", "Components produced", "Purpose of component", "Properties of the component", "What the component is derived from")
) %>%
  kable_styling(full_width = TRUE, bootstrap_options = c("striped", "hover", "condensed", "responsive")) %>%
    row_spec(0, bold = TRUE) %>%  # Make header bold
    kable_classic(full_width = TRUE, html_font = "Arial") %>%  # Classic table style
    column_spec(1:5, border_left = TRUE, border_right = TRUE) %>% # Add borders
    row_spec(0, extra_css = "border: 1px solid black;") %>%  # Ensure full borders
    row_spec(0:nrow(data), extra_css = "border: 1px solid black;")  # Add horizontal borders

## ----randomisation_objects_table, echo=FALSE----------------------------------
library(kableExtra)
library(knitr)

# Define table data
table_data <- data.frame(
  Randomisation_Object = c(
    "$F_v$", 
    "$F_v \\wedge F_w$", 
    "$F_v [F_w]$", 
    "$F_v \\otimes F_w$", 
    "$\\{ F_v \\otimes F_w \\} [F_x]$", 
    "$F_v \\otimes \\{ F_w [F_x] \\}$", 
    "$F_v [F_w \\otimes F_x]$", 
    "$F_x \\otimes F_v \\otimes F_w$"
  ),
  Structural_Object = c(
    "$F_v$", 
    "$F_v \\wedge F_w$", 
    "$F_v (F_w)$", 
    "$F_v \\wedge F_w$", 
    "$F_v \\wedge F_w (F_x)$", 
    "$F_v \\wedge F_w (F_x)$", 
    "$F_v (F_w \\wedge F_x)$", 
    "$F_x \\wedge F_v \\wedge F_w$"
  ),
  stringsAsFactors = FALSE
)

# Render the table
knitr::kable(
  table_data, format = "html", escape = FALSE,
  col.names = c("Randomisation object", "Corresponding structural object"),
) %>%
  kable_styling(full_width = FALSE, position = "center", bootstrap_options = c("striped", "hover", "condensed", "responsive")) %>%
  column_spec(1:2, border_left = TRUE, border_right = TRUE, width = "50%") %>%  # Removed `align`
  row_spec(0, bold = TRUE, background = "#f2f2f2", extra_css = "border: 1px solid black; text-align: center;") %>% # Header styling
  row_spec(1:nrow(table_data), extra_css = "border: 1px solid black; text-align: center;") # Row styling

## ----randomisation_nesting_table, echo=FALSE----------------------------------
library(kableExtra)
library(knitr)

# Define table data
table_data <- data.frame(
  Randomisation_Object = c(
    "$F_v$", 
    "$F_v \\wedge F_w$", 
    "$F_v [F_w]$", 
    "$F_v \\otimes F_w$", 
    "$\\{ F_v \\otimes F_w \\} [F_x]$", 
    "$F_v \\otimes \\{ F_w [F_x] \\}$", 
    "$F_v [F_w \\otimes F_x]$", 
    "$F_x \\otimes F_v \\otimes F_w$"
  ),
  Nest_Objects = c(
    "*Mean*", 
    "*Mean*, $F_w$", 
    "*Mean*, $F_w$", 
    "*Mean*, $F_v, F_w$", 
    "*Mean*, $F_v [F_x], F_w [F_x], F_x$", 
    "*Mean*, $F_v \\otimes F_x, F_w [F_x], F_v, F_x$", 
    "*Mean*, $F_v \\otimes F_x, F_v, F_w$", 
    "*Mean*, $F_x, F_v, F_w, F_x \\otimes F_v, F_x \\otimes F_w, F_v \\otimes F_w$"
  ),
  stringsAsFactors = FALSE
)

# Render the table
knitr::kable(
  table_data, format = "html", escape = FALSE,
  col.names = c("Randomisation object", "Randomisation-nest objects"),
) %>%
  kable_styling(full_width = FALSE, position = "center", bootstrap_options = c("striped", "hover", "condensed", "responsive")) %>%
  column_spec(1:2, border_left = TRUE, border_right = TRUE, width = "50%") %>%
  row_spec(0, bold = TRUE, background = "#f2f2f2", extra_css = "border: 1px solid black; text-align: center;") %>%
  row_spec(1:nrow(table_data), extra_css = "border: 1px solid black; text-align: center;")

## ----mixed_model_terms_table, echo=FALSE--------------------------------------
library(kableExtra)
library(knitr)

# Define table data
table_data <- data.frame(
  Randomisation_Object = c(
    "$F_v$", 
    "$F_v \\wedge F_w$", 
    "$F_v [F_w]$", 
    "$F_v \\otimes F_w$", 
    "$\\{ F_v \\otimes F_w \\} [F_x]$", 
    "$F_v \\otimes \\{ F_w [F_x] \\}$", 
    "$F_v [F_w \\otimes F_x]$"
  ),
  Mixed_Model_Term = c(
    "$F_v$", 
    "$F_v * F_w$", 
    "$F_v (F_w)$", 
    "$F_v * F_w$", 
    "$F_v * F_w (F_x)$", 
    "$F_v * F_w (F_x)$", 
    "$F_v (F_w * F_x)$"
  ),
  stringsAsFactors = FALSE
)

# Render the table
knitr::kable(
  table_data, format = "html", escape = FALSE,
  col.names = c("Randomisation object", "Mixed model term"),
) %>%
  kable_styling(full_width = FALSE, position = "center", bootstrap_options = c("striped", "hover", "condensed", "responsive")) %>%
  column_spec(1:2, border_left = TRUE, border_right = TRUE, width = "50%") %>%
  row_spec(0, bold = TRUE, background = "#f2f2f2", extra_css = "border: 1px solid black; text-align: center;") %>%
  row_spec(1:nrow(table_data), extra_css = "border: 1px solid black; text-align: center;")

## ----hasse_diagram_ls2, echo=FALSE, out.width='80%', fig.align='center'-------
knitr::include_graphics("figures/SM2F18_output.png")

