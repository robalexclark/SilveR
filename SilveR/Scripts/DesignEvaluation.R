sessionInfo()

# Parameter = Y for SilveR version, N for standalone R code
SilveR <- "Y"
Simonbit = "Y"
options(show.error.messages = T)

###############################################################################################################
if (SilveR == "N") {
    data.folder.location <- "//uk1dsntv003/rd-mdd-statsci/StatSciEurope/Projects/2600/2610 mjc STB  - Design Database Presentation and Paper/R program/"
    #"C:/Users/stb11684/Desktop/2610 mjc STB  - Design Database Presentation and Paper/R program/"

    ###############################################################################################################

    #Set destination and font size for output graph

    pdf <- "Y"
    larger.fontlabelmultiplier <- 1 # To increase the font size of larger labels (where tier has 4 or less effects) - should be 1 by default
    middle.fontlabelmultiplier <- 1 # To increase the font size of middle size labels (where effect has an equivalent factor label and tier has >4 effects) It needs to be 0.6667 to give same size as smaller labels or 1.3333 to be same size as larger label
    smaller.fontlabelmultiplier <- 1 # To increase the font size of smaller labels (where tier has >4 effects)
    check.shared.df <- "Y"
    maxlevels.df <- "Y"
    ###############################################################################################################

    #Specify example name

    #Paper 1 Main paper figures
    #  example <- "MP1 Ex_2_1_Fig2_2"	#Figure 2.2 - Latin Square agricultural trial Layout structure  
    #  example <- "MP1 Ex_4_1_Fig4_1"	#Figure 4.1 - Block design with two treatments per block Layout structure  
    #  example <- "MP1 Ex_4_2_Fig4_2"	#Figure 4.2 - Full factorial chick experiment Layout structure
    #  example <- "MP1 Ex 4_3_Fig4_3"      #Figure 4.3 - Example from Bailey book P8 - Ryegrass Layout Structure

    #Paper 2 Main paper figures  
    # example <- "MP2 Ex_2_1_Fig3_1a"  #Figure 3.1a - Latin Square agricultural trial Restricted Layout structure  
    #	example <- "MP2 Ex_2_1_Fig3_1b"  #Figure 3.1b- Latin Square agricultural trial Restricted Layout structure  
    #	example <- "MP2 Ex_2_1_Fig3_1c"  #Figure 3.1c- Latin Square agricultural trial Restricted Layout structure  
    #	example <- "MP2 Ex_4_1_Fig4_1a"  #Figure 4.1a - Block design with two treatments per block Layout structure  	
    #	example <- "MP2 Ex_4_1_Fig4_1b"  #Figure 4.1b - Block design with two treatments per block Restricted Layout structure
    #	example <- "MP2 Ex_4_2_Fig4_2a"  #Figure 4.3 - Full factorial chick experiment Layout structure
    #	example <- "MP2 Ex_4_2_Fig4_3"   #Figure 4.3 - Full factorial chick experiment Restricted Layout structure	
    # example <- "MP2 Ex 4_3_Fig4_4"   #Figure 4.4 - Example from Bailey book P8 - Ryegrass Layout Structure
    #  example <- "MP2 Ex 4_3_Fig4_5"   #Figure 4.5 - Example from Bailey book P8 - Ryegrass Restricted Layout Structure


    #Paper 1 Supplementary Material figures
    #  example <- "SM1 Ex1"  #Figure 1 - BIBD Layout structure  
    #  example <- "SM1 Ex2"  #Figure 1 - Fractional Factorial Layout structure  
    #  example <- "SM1 Ex3"  #Figure 1 - Plackett Burman Layout structure  
    #  example <- "SM1 Ex4a"  #Figure 1 - COD Layout structure  
    example <- "SM1 Ex4b" #Figure 1 - COD with carry-over Layout structure  
    #(others to follow)


    #Main paper figures

    #	example <- "MP Ex_2_1_Fig2_2"	#Latin Square agricultural trial RDS
    #	example <- "MP Ex_2_1_Fig2_3"	#Latin Square agricultural trial FDS Scenario 1
    #	example <- "MP Ex_2_1_Fig2_4"	#Latin Square agricultural trial FDS Scenario 2
    #	example <- "MP Ex_3_1_Fig3_1a"	#Block design with two treatments per block RDS
    #	example <- "MP Ex_3_1_Fig3_1b"	#Block design with two treatments per block FDS
    #	example <- "MP Ex_3_2_Fig3_2a"	#Full factorial chick experiment RDS
    #	example <- "MP Ex_3_2_Fig3_3"	#Full factorial chick experiment FDS	
    #	example <- "MP Ex_3_3_Fig3_4"	#Split plot paper manufacture RDS
    #	example <- "MP Ex_3_3_Fig3_5"	#Split plot paper manufacture FDS
    #	example <- "MP Ex_3_4_Fig3_6"	#Wine tasting experiment RDS
    #	example <- "MP Ex_3_4_Fig3_7"	#Wine tasting experiment FDS
    #	example <- "MP example Appendix B_1"	#Two phase study FDS

    #Supplementary material figures

    #	example <- "SM Ex1_Fig1"	#Plant Experiment RDS	
    #	example <- "SM Ex1_Fig2"	#Plant Experiment FDS	
    #	example <- "SM Ex2_Fig3"	#Milk Storage Experiment FDS for randomisation 1	
    #	example <- "SM Ex2_Fig4"	#Milk Storage Experiment FDS for randomisation 2	
    #	example <- "SM Ex3_Fig5"	#Disconnected Row-Column Design RDS	
    #	example <- "SM Ex3_Fig6"	#Disconnected Row-Column Design FDS	
    #	example <- "SM Ex1_Fig8"	#Plant Experiment RDS (R version)	
    #	example <- "SM Ex1_Fig9"	#Plant Experiment FDS (R version)
    #	example <- "SM Ex2_Fig10"	#Milk Storage Experiment RDS (R version)	
    #	example <- "SM Ex2_Fig11"	#Milk Storage Experiment FDS for randomisation 1 (R version)
    #	example <- "SM Ex2_Fig12"	#Milk Storage Experiment FDS for randomisation 2 (R version)	
    #	example <- "SM Ex4_Fig13"	#Chemical Investigation Design 1 RDS	
    #	example <- "SM Ex4_Fig14"	#Chemical Investigation Design 2 RDS	
    #	example <- "SM Ex4_Fig15"	#Chemical Investigation Design 1 FDS	
    #	example <- "SM Ex5_Fig16"	#Human Interaction Experiment RDS	
    #	example <- "SM Ex5_Fig17"	#Human Interaction Experiment FDS	
    #	example <- "SM Ex5_Fig18"	#Human Interaction Experiment FDS (including Pseudofactors)	
    #	example <- "SM Ex6_Fig20"	#Cake Making Experiment RDS	
    #	example <- "SM Ex6_Fig21"	#Cake Makind Experiment FDS	
    #	example <- "SM Ex7_Fig23"	#Split-Plot Split-block Experiment 1 RDS	
    #	example <- "SM Ex7_Fig24"	#Split-Plot Split-block Experiment 1 FDS		
    #	example <- "SM Ex7_Fig25"	#Split-Plot Split-block Experiment 2 RDS		
    #	example <- "SM Ex7_Fig26"	#Split-Plot Split-block Experiment 2 FDS	
    #	example <- "SM MPEx3_4_Fig27"	#Wine tasting experiment RDS (R version)	
    #	example <- "SM MPEx3_4_Fig28"	#Wine tasting experiment FDS (R version)

    #Other examples

    #	example <- "Simon_Chem1"
    #	example <- "Simon_Chem2"
    #	example <- "Fact14"
    #	example <- "Ex_2_1_dupfactor"
    #	example <- "label test" 
    #	example <- "Ex_3_Large" 
    #	example <- "SilverTestR" 
    #	example <- "Test_equivs"      #Test program copes with 3 pairs of equivalent factors
    #	example <- "Test_equivs3"    #Tests if there are 3 equivalent factors the prgram stops with warning
    #	example <- "Test_dup_names"   #Testing for duplicate 2 or 3 letter abbreviation for names
    #	example <- "Test_dup_names3"   #Testing for duplicate 2 letter abbreviation for names where 3 letters is unique
    #	example <- "Test_contain_names"   #Testing for factors which contain names of other factors
    #	example <- "Test_blank_levels"   #Testing for factors which have levels which are blank - should also catch any blank lines
    #	example <- "SiTest1"  # Simons test of two equivalent sets of factors, one of the sets 4 equivalent factors - variable names are not distinct at third letter
    #	example <- "SiTest2"  # Simons test of two equivalent sets of factors, one of the sets 4 equivalent factors - variable names are distinct at second letter
    #	example <- "SiTest3"  # Simons test of three equivalent pairs - variable names are distinct at second letter

    #	example <-"analytical_new_example"# - New analytical example for the QM talk
    #	example <-"analytical_new_example_R1"# - New analytical example for the QM talk with first randomsiation
    #	example <-"analytical_new_example_R2"# - New analytical example for the QM talk with second randomsiation
    #	example <-"analytical_new_example_R3"# - New analytical example for the QM talk with third randomsiation
    #	example <-"analytical_new_example_R4"# - New analytical example for the QM talk with fourth randomsiation
    #	example <-"analytical_new_example_R5"# - New analytical example for the QM talk with fifth randomsiation
    #	example <-"analytical_new_example_R6"# - New analytical example for the QM talk with sixth randomsiation
    #	example <-"analytical_new_example_R7"# - New analytical example for the QM talk including the Run order factor

    #	example <-"analytical_new_example_Run_Order_1"# - New analytical example for the QM talk including the Run order factor at 4 levels
    #	example <-"analytical_new_example_Run_Order_1_R8"# - New analytical example for the QM talk including the Run order factor at 4 levels (randomise Batches across site)
    #	example <-"analytical_new_example_Run_Order_2"# - New analytical example for the QM talk including the Run order factor at 8 levels
    #	example <-"analytical_new_example_Run_Order_2_R9"# - New analytical example for the QM talk including the Run order factor at 8 levels (randomise Batches across site)

    #	example <-"fractionalfactorial"# - Fractional factorial example
    #	example <-"allfixed"# - Example with all fixed factors
    #	example <-"allnested"# - Example with all random factors
    #	example <-"Bailey_book_P145"# - Example from Bailey book P145

}
###############################################################################################################
###############################################################################################################
###############################################################################################################

if (SilveR == "Y") {
    suppressWarnings(library(R2HTML))
}
suppressWarnings(library(igraph))
suppressWarnings(library(methods))
suppressWarnings(library(MASS))

if (SilveR == "Y") {
    # retrieve args
    Args <- commandArgs(TRUE)

    #Read in data

    mydata <- read.csv(Args[3], header = TRUE, sep = ",")

    #Copy Args
    fixedEffects <- Args[4]
    randomEffects <- Args[5]

    rds <- Args[6]
    rdsPartialCrossing <- Args[7]
    rdsDegreesOfFreedom <- Args[8]

    fds <- Args[9]
    fdsPartialCrossing <- Args[10]
    fdsDegreesOfFreedom <- Args[11]
    showRandomisationArrows <- Args[12]

    randomisedFactor <- Args[13]
    randomisedToFactor <- Args[14]
    fdsEffects <- Args[15]
    fdsEffectsLabels <- Args[16]
    fdsUserLabels <- Args[17]
    userDefinedTerms <- Args[18]
    producePlotAsPDF <- Args[19]
    produceBWPlot <- Args[20]
    testForConfoundedDF <- Args[21]
    maxlevels.df_temp <- Args[22]
    smallFontMultiplier_temp <- as.numeric(Args[23])
    middleFontMultiplier_temp <- as.numeric(Args[24])
    largeFontMultiplier_temp <- as.numeric(Args[25])

    smallFontMultiplier <- smallFontMultiplier_temp / 10
    middleFontMultiplier <- middleFontMultiplier_temp / 10
    largeFontMultiplier <- largeFontMultiplier_temp / 10

    #Need to reverse Args[21] due to double negative!
    if (maxlevels.df_temp == "Y") {
        maxlevels.df <- "N"
    } else {
        maxlevels.df <- "Y"
    }


    #THis code generates the user defined fixed / random as defined by the user

    if (userDefinedTerms != "NULL") {
        # Creating the Fixed random definition vector
        Fixed_Random_Def1 <- eval(parse(text = paste("mydata$", userDefinedTerms)))
        k <- 0
        for (i in 1:length(Fixed_Random_Def1)) {
            if (Fixed_Random_Def1[i] != "") {
                k = k + 1
            }
        }
        Fixed_Random_Def <- Fixed_Random_Def1[-c((k + 1):length(Fixed_Random_Def1))]

        #This code to replace the variable Fixed_Random_Def (which has entries Fixed and Random with userfinaleffectrandom (which has levels 0 and 1) 
        if (exists("Fixed_Random_Def")) {
            userfinaleffectrandom <- c(rep(NA, length(Fixed_Random_Def)))
            for (i in 1:length(Fixed_Random_Def)) {
                if (Fixed_Random_Def[i] == "Fixed") { userfinaleffectrandom[i] = 0 }
            }
            for (i in 1:length(Fixed_Random_Def)) {
                if (Fixed_Random_Def[i] == "Random") { userfinaleffectrandom[i] = 1 }
            }
        }
    }


    #HTML output setup
    #Setup the html file and associated css file
    htmlFile <- sub(".csv", ".html", Args[3]);
    #determine the file name of the html file
    .HTML.file = htmlFile

    #Output HTML header
    HTML.title("<bf>SilveR Design Evaluation Module", HR = 1, align = "left")
    HTML.title("</bf> ", HR = 2, align = "left")
    HTML.title("This module performs the calculations required to produce Hasse diagrams of the Layout and Restricted Layout structures, as described in xxx.", HR = 0, align = "left")
    #zzz
    #	HTML.title("This module performs the calculations required to produce Hasse diagrams of the Layout and Restricted Layout structures, as described in Bate and Chatfield (2015).", HR=0, align="left")


    #Select the RDS options required design evaluation
    showRDS <- rds #takes on values Y and N depending if you want to show the RDS
    showpartialRDS <- rdsPartialCrossing #takes on values Y and N depending if you want to show partial crossing on the RDS
    showdfRDS <- rdsDegreesOfFreedom #takes on values Y and N depending if you want to show df on the RDS
    if (showpartialRDS == "Y" || showdfRDS == "Y") showRDS <- "Y"

    #Select required options for Final Design Structure evaluation
    showFDS <- fds #takes on values Y and N depending if you want to show the FDS
    showpartialFDS <- fdsPartialCrossing #takes on values Y and N depending if you want to show partial crossing on the FDS
    showdfFDS <- fdsDegreesOfFreedom #takes on values Y and N depending if you want to show df on the FDS
    showrandFDS <- showRandomisationArrows
    if (showpartialFDS == "Y" || showdfFDS == "Y" || showrandFDS == "Y") showFDS <- "Y"



    #Sorting out the input variables to match up to R program
    larger.fontlabelmultiplier <- largeFontMultiplier # To increase the font size of larger labels (where tier has 4 or less effects) - should be 1 by default
    middle.fontlabelmultiplier <- middleFontMultiplier # To increase the font size of middle size labels (where effect has an equivalent factor label and tier has >4 effects)
    smaller.fontlabelmultiplier <- smallFontMultiplier # To increase the font size of smaller labels (where tier has >4 effects)
    check.shared.df <- testForConfoundedDF
    pdf <- producePlotAsPDF

    if (pdf == "Y") {
        pdfFile <- sub(".csv", ".pdf", Args[3]);
        #determine the file name of the pdf file
        RDSPlot <- sub(".html", "RDSPlot.pdf", htmlFile)
        pdf(file = pdfFile, width = 11, height = 8)
    }


    fixedresplist <- c()
    fixedresplength <- 0

    if (fixedEffects != "NULL") {
        #Breakdown the list of fixed factors - fixedEffects
        fixedtempChanges <- strsplit(fixedEffects, ",")
        fixedexpectedChanges <- c(0)
        for (i in 1:length(fixedtempChanges[[1]])) {
            fixedexpectedChanges[length(fixedexpectedChanges) + 1] = (fixedtempChanges[[1]][i])
        }
        for (j in 1:(length(fixedexpectedChanges) - 1)) {
            fixedresplist[j] = fixedexpectedChanges[j + 1]
        }
        fixedresplength <- length(fixedresplist)
    }

    #Breakdown the list of random factors - randomEffects
    randomresplist <- c()
    randomresplength <- 0

    if (randomEffects != "NULL") {
        randomtempChanges <- strsplit(randomEffects, ",")
        randomexpectedChanges <- c(0)
        for (i in 1:length(randomtempChanges[[1]])) {
            randomexpectedChanges[length(randomexpectedChanges) + 1] = (randomtempChanges[[1]][i])
        }
        for (j in 1:(length(randomexpectedChanges) - 1)) {
            randomresplist[j] = randomexpectedChanges[j + 1]
        }
        randomresplength <- length(randomresplist)
    }

    randomfacsid <- c(rep(0, fixedresplength), rep(1, randomresplength)) #Add Mean to design dataset

    factors <- c(fixedresplist, randomresplist)


    #Rearranging the columns of mydata so that order is fixed effect first, random effects second
    mydata_temp1 <- mydata[, factors]

    len <- dim(mydata)[2]
    if (len > length(factors)) {
        mydata_temp2 <- mydata[, (length(factors) + 1):len]
        mydata <- cbind(mydata_temp1, mydata_temp2)
    } else {
        mydata <- mydata_temp1
    }

    #Creating the randomised matrix of randomisation arrows from the two dataset columns

    if (fds == "Y" && randomisedFactor != "NULL" && randomisedToFactor != "NULL") {
        simon1 <- eval(parse(text = paste("mydata$", randomisedFactor)))
        k <- 0
        for (i in 1:length(simon1)) {
            if (simon1[i] != "") {
                k = k + 1
            }
        }

        simon2 <- eval(parse(text = paste("mydata$", randomisedToFactor)))
        mydata$rand <- eval(parse(text = paste("mydata$", randomisedFactor)))
        mydata$randto <- eval(parse(text = paste("mydata$", randomisedToFactor)))
        tempsimon <- mydata[, c("rand", "randto")]
        tempsimon <- tempsimon[-c((k + 1):length(simon1)),]
        randomised <- as.matrix(tempsimon)
    }
    if (fds == "Y") {

        #This lists the abbreviated and full names of effects in the final design structure 

        mydata$fdsel <- eval(parse(text = paste("mydata$", fdsEffects)))
        tempsimonx <- mydata[, c("fdsel")]
        simon2 <- eval(parse(text = paste("mydata$", fdsEffectsLabels)))
        l <- 0
        for (i in 1:length(simon2)) {
            if (simon2[i] != "") {
                l = l + 1
            }
        }
        brief.finaldesign.effects <- as.vector(tempsimonx[-c((l + 1):length(simon2))])

        mydata$fdsem <- eval(parse(text = paste("mydata$", fdsEffectsLabels)))
        tempsimonx <- mydata[, c("fdsem")]
        simon2 <- eval(parse(text = paste("mydata$", fdsEffectsLabels)))
        l <- 0
        for (i in 1:length(simon2)) {
            if (simon2[i] != "") {
                l = l + 1
            }
        }
        finaldesign.effects <- as.vector(tempsimonx[-c((l + 1):length(simon2))])

        #mydata$fdsen <-eval(parse(text= paste ("mydata$",fdsUserLabels)))
        mydata$fdsen <- eval(parse(text = paste("mydata$", fdsUserLabels)))
        tempsimonx <- mydata[, c("fdsen")]
        #simon2<-eval(parse(text= paste ( "mydata$",fdsUserLabels)))
        simon2 <- eval(parse(text = paste("mydata$", fdsUserLabels)))
        l <- 0
        for (i in 1:length(simon2)) {
            if (simon2[i] != "") {
                l = l + 1
            }
        }
        FDSrename.brief <- as.vector(tempsimonx[-c((l + 1):length(simon2))])
        #	FDSrename.full  <- as.vector(tempsimonx [-c((l+1):length(simon2))])

        #Remove the extra columns from mydata dataset to allow creation of mydesign dataset
        dimension <- dim(mydata)[2]
        if (fdsEffectsLabels != "NULL") {
            mydata <- mydata[, - (dimension)]
        }
        dimension <- dim(mydata)[2]
        if (fdsUserLabels != "NULL") {
            mydata <- mydata[, - (dimension)]
        }
        dimension <- dim(mydata)[2]
        if (fdsEffects != "NULL") {
            mydata <- mydata[, - (dimension)]
        }
        dimension <- dim(mydata)[2]
        if (randomisedToFactor != "NULL") {
            mydata <- mydata[, - (dimension)]
        }
        dimension <- dim(mydata)[2]
        if (randomisedFactor != "NULL") {
            mydata <- mydata[, - (dimension)]
        }

        if (fds == "Y") {
            dimension <- dim(mydata)[2]
            if (fdsEffectsLabels != "NULL") {
                mydata <- mydata[, - (dimension)]
            }
            dimension <- dim(mydata)[2]
            if (fdsUserLabels != "NULL") {
                mydata <- mydata[, - (dimension)]
            }
            dimension <- dim(mydata)[2]
            if (fdsEffects != "NULL") {
                mydata <- mydata[, - (dimension)]
            }
            dimension <- dim(mydata)[2]
            if (randomisedToFactor != "NULL") {
                mydata <- mydata[, - (dimension)]
            }
            dimension <- dim(mydata)[2]
            if (randomisedFactor != "NULL") {
                mydata <- mydata[, - (dimension)]
            }
        }
    }

    #	mydesign <- cbind(Mean=rep(1,length(mydata[ ,1])),mydata[ , ])   
    mydesignxx <- cbind(Mean = rep(1, length(mydata[, 1])), mydata[,])
    factorsx <- c("Mean", factors)
    mydesign <- mydesignxx[, factorsx]




    #End of if statement for SilveR
}

#Overall options required for both versions of the R code

Colourblue <- "blue"
Colourred <- "red"
Colourpurple <- "purple"
Colourorange <- "orange"

if (SilveR == "Y") {
    if (produceBWPlot == "Y") {
        Colourblue <- "black"
        Colourred <- "black"
        Colourpurple <- "black"
        Colourorange <- "black"

    }
}



if (SilveR == "N") {

    if (pdf == "Y") pdf(file = paste(data.folder.location, "pdfs/", example, "_output.pdf", sep = ""), width = 11, height = 8)

    #########################################################################################################
    # This section reads in the data, specifying location and number of data rows for example
    # Variables names must exist,not use underscore or asterisk and have unique first 2 characters - actually not sure if underscore is OK
    #STB update this example - resid & "*"s
    #To read in data for Example XXXX   
    #if (example=="XXXX") {
    #mydata <- read.csv(paste(data.folder.location,"XXXX.csv", sep=""),sep=",",header=TRUE,nrows=36)
    #mydesign <- cbind(Mean=rep(1,length(mydata[ ,1])),mydata[ ,-6])   #Add Mean to design and remove response variable(s)
    #randomfacsid<-c(1,1,0,0,0)    #Designate which factors are random 1=random, 0=fixed
    #This lists the abbreviated and full names of effects in the final design

    #brief.finaldesign.effects <- c("Mean","Bench","Soils","Leaf.treats","Plant=Be*So","La*So","Le*So","resid=Be*La*Le*So")
    #finaldesign.effects       <- c("Mean","Bench","Soils","Leaf.treats","Bench*Soils","Layers*Soils","Leaf.treats*Soils","Bench*Layers*Leaf.treats*Soils")
    #This shows which effects are randomised to others (using full.not abbreviated names)
    #randomised <- matrix(c("Soils","Bench*Soils","Leaf.treats","Bench*Layers*Leaf.treats*Soils"),ncol=2,byrow=T)
    #User to specify any renaming of effects or arrows
    #FDSrename.brief <- c("Mean","Bench","Soils","Leaf.treats","Plant=Be*So","La(So)","Le*So","resid=Be*Le*La(So)")
    #FDSrename.full <- c( "Mean","Bench","Soils","Leaf.treats", "Bench*Soils" ,"Layers*Soils" ,"Leaf.treats*Soils" ,"Bench*Layers*Leaf.treats*Soils")
    #}

    #########################################################################################################
    # Read in the data for examples in paper and others to test program
    source(paste(data.folder.location, "examples_input_data.txt", sep = ""))
    #if (example=="Ex_3_Fig3_2" ||example=="SM_Ex3_Fig9") {check.shared.df <- "N"} else  {check.shared.df <- "Y" }


    #rds.check.shared.tier <- 6   Set this if want to check down to a specific tier NO LONGER USED
    #fds.check.shared.tier <- 6   Set this if want to check down to a specific tier NO LONGER USED

    #########################################################################################################
    #Specify whether design evaluation has already been specified. If not then select in code below.
    design.eval <- "Y"

    #Select required design evaluation for examples where it is not specified
    if (design.eval == "N") {
        showRDS <- "Y" #takes on values Y and N depending if you want to show the RDS
        showpartialRDS <- "Y" #takes on values Y and N depending if you want to show partial crossing on the RDS
        showdfRDS <- "Y" #takes on values Y and N depending if you want to show df on the RDS
        if (showpartialRDS == "Y" || showdfRDS == "Y") showRDS <- "Y"

        showFDS <- "N" #takes on values Y and N depending if you want to show the FDS
        showpartialFDS <- "N" #takes on values Y and N depending if you want to show partial crossing on the FDS
        showdfFDS <- "N" #takes on values Y and N depending if you want to show df on the FDS
        showrandFDS <- "N" #takes on values Y and N depending if you want to show randomisation on the FDS
        if (showpartialFDS == "Y" || showdfFDS == "Y" || showrandFDS == "Y") showFDS <- "Y"
    }
    # SHOULD PUT SOME ERROR CHECKING HERE TO ENSURE CAN ONLY REQUEST FDS IF EFFECTS HAVE BEEN SPECIFIED AND
    # ARROWS FOR RANDOMISED EFFECTS IF THEY HAVE BEEN SPECIFIED

    #end of SilveR IF statement
}




##########################################################################################################
#              algorithm_functionsd R code
##########################################################################################################


#########################################################################################
#This contains the following functions used in the algorithm
#  STRUCTURE.FUN : Finds the structure of the design - crossed, partically crossed and nested factors
#  EXTRACT.FACTOR.FUN : This returns a vector with list of factors contained within the effect
#  BRIEF.EFFECT.FUN :  This returns an abbreviated name for an effect
#  SORTAOV.TERM.FUN :  This returns terms from aov or dropterm with components sorted alphabetically
#  CONFOUND.TAB.FUN : Function to provide table of main effects which are confounded and what they are confounded with
#  CONFOUND.PRINT.FUN : Function to to list effects which are confounded and what they are confounded with
#  DESIGNORDER : Provides list of effects of order i i.e. those with order i and interactions of order i-1
#                       and construct design matrix with these effects
#  DSCOORDS.FUN     : Calculates coordinates for the vertices of the Hasse diagram
#  DFS.FUN     : Calculates degrees of freedom by subtraction method and checks for shared degrees of freedom
########################################################################################



############# STRUCTURE.FUN ######################################################
#Finds the structure of the design - crossed, partically crossed and nested factors
structure.fun <- function(design) {

    design.colnames <- colnames(design)
    novar <- length(colnames(design))
    effects.table <- matrix(" ", nrow = novar, ncol = novar)
    rownames(effects.table) <- design.colnames
    colnames(effects.table) <- design.colnames
    for (r.effect in 2:novar)
        effects.table[r.effect, 1] <- 1 #Setting entries for mean column
    for (c.effect in 2:novar)
        effects.table[1, c.effect] <- 0 #Setting entries for mean row           
    for (r.effect in 2:novar) {
        for (c.effect in 2:novar) {
            if (r.effect != c.effect) {
                freq.table <- table(design[, r.effect], design[, c.effect])
                #freq.table contains number of times mth value of factor alpha occurs with kth value of factor betai
                #First condition checks whether there is a combination of alphath factor and kth value of betaith factor
                #                                                which has a freq of 0 i.e. doesn't occur
                #If first condition is satisfied then table(freq.table[ ,k])[1] is number of times 0 occurs
                #Second condition checks whether all but one row has a value 0 for kth value of betaith factor
                #If both conditions are satisfied then the kth value of betaith factor is nested within alphath factor
                nestk <- vector(, length = nrow(freq.table)) #Set up factor for all k levels of factorj to indicate whether nested within factor i
                for (k in 1:nrow(freq.table)) {
                    if (min(freq.table[k, ]) == 0 && table(freq.table[k, ])[1] + 1 == ncol(freq.table)) nestk[k] <- 1 else
                        if (min(freq.table[k, ]) == 0) nestk[k] <- 0.5 else nestk[k] <- 0
                }
                effects.table[r.effect, c.effect] <- if (all(nestk == 1)) "1" else if (all(nestk == 0)) "0" else "(0)"
            }
        }
    }
    effects.table
}


############################################################################################
############# EXTRACT.FACTOR.FUN ######################################################
#     THIS RETURNS a vector with list of factors contained within the term. Nested terms stay intact.
#  Possibly strsplit could be used instead of this function
#zzz^ change
extract.factor.fun <- function(effect) {
    pos_ <- gregexpr("\\^", effect)
    posbo <- gregexpr("\\(", effect)
    posbc <- gregexpr("\\)", effect)
    posi_ <- -1
    #This loop finds position of * which separate effects but are not contained within a nested effect
    for (k in 1:length(pos_[[1]])) {
        if (length(posbo[[1]][posbo[[1]] < pos_[[1]][k]]) == length(posbc[[1]][posbc[[1]] < pos_[[1]][k]])) {
            posi_ <- c(posi_, pos_[[1]][k])
        }
    }
    if (length(posi_) > 1) posi_ <- posi_[-1]

    if (posi_[1] == -1) { vec.factor <- effect } else {
        no.factors <- 1 + length(posi_)
        vec.factor <- rep("", no.factors)
        if (no.factors == 2) {
            vec.factor[1] <- substring(effect, 1, posi_[1] - 1)
            vec.factor[2] <- substring(effect, posi_[1] + 1, nchar(effect))
        } else {
            for (i in 1:(no.factors - 2)) {
                if (i == 1) vec.factor[1] <- substring(effect, 1, posi_[i] - 1)
                vec.factor[i + 1] <- substring(effect, posi_[i] + 1, posi_[i + 1] - 1)
                if (i == (no.factors - 2)) vec.factor[i + 2] <- substring(effect, posi_[i + 1] + 1, nchar(effect))
            }
        }
        vec.factor
    }
}


############################################################################################
############# EXTRACT.NESTFACTOR.FUN ######################################################
#     THIS RETURNS a vector with list of factors contained within the term without any terms it may nest within. e.g. A(B) is returned as A
extract.nestfactor.fun <- function(effect) {
    posbo <- gregexpr("\\(", effect)[[1]] #opening brackets
    posbc <- gregexpr("\\)", effect)[[1]] #closing brackets      
    if (posbo[1] == -1) { vec.factor <- effect } else {
        posbci.lt <- posbc[c((posbc[-length(posbc)] < posbo[-1]), T)]
        posboi.lt <- posbo[c(T, (posbc[-length(posbc)] < posbo[-1]))]
        vec.factor <- substring(effect, 1, posboi.lt[1] - 1) #Includes effect up to first opening bracket
        if (length(posbci.lt) > 1) {
            for (lt in 1:(length(posbci.lt) - 1)) {
                vec.factor <- paste(vec.factor, substring(effect, posbci.lt[lt] + 1, posboi.lt[lt + 1] - 1), sep = "") #Includes effect after closed bracket corresponding first opening bracket up to next opneing bracket    
            }
        }
        if (posbci.lt[length(posbci.lt)] != nchar(effect)) {
            vec.factor <- paste(vec.factor, substring(effect, posbci.lt[length(posbci.lt)] + 1, nchar(effect)), sep = "")
        }
    }
    vec.factor
}


############################################################################################
############# BRIEF.EFFECT.FUN ######################################################      
#     THIS RETURNS a variable length abbreviated name for an effect       
brief.effect.fun <- function(effect) {
    #zzz^ change
    pos_ <- gregexpr("\\^", effect)[[1]]
    pos_
    if (pos_[1] == -1) { brief.effect <- effect } else {
        no.factors <- 1 + length(pos_)
        if (no.factors == 2) {
            #zzz^change
            brief.effect <- paste(substring(effect, 1, min(pos_[1] - 1, abbrev.length[substring(effect, 1, pos_[1] - 1)])), "^", substring(effect, pos_[1] + 1, min(pos_[1] + abbrev.length[substring(effect, pos_[1] + 1, nchar(effect))], nchar(effect))), sep = "")
        } else {
            for (i in 1:(no.factors - 2)) {
                if (i == 1) brief.effect <- substring(effect, 1, min(pos_[1] - 1, abbrev.length[substring(effect, 1, pos_[1] - 1)]))
                #zzz^change
                brief.effect <- paste(brief.effect, "^", substring(effect, pos_[i] + 1, min(pos_[i] + abbrev.length[substring(effect, pos_[i] + 1, pos_[i + 1] - 1)], pos_[i + 1] - 1)), sep = "")
                #zzz^change
                if (i == (no.factors - 2)) brief.effect <- paste(brief.effect, "^", substring(effect, pos_[i + 1] + 1, min(pos_[i + 1] + abbrev.length[substring(effect, pos_[i + 1] + 1, nchar(effect))], nchar(effect))), sep = "")
            }
        }
    }
    brief.effect
}

############################################################################################
############# SORTAOV.TERM.FUN ######################################################
#     THIS RETURNS terms from aov or dropterm with components sorted alphabetically       

sortaov.term.fun <- function(x) {
    splitmatrixformat <- suppressWarnings(do.call(rbind, strsplit(x, ":")))
    sort.splitmatrix <- t(apply(as.matrix(splitmatrixformat), 1, sort))
    sorted.effects <- matrix(NA, nrow = dim(sort.splitmatrix)[1], ncol = dim(sort.splitmatrix)[2])
    for (k in 1:dim(sort.splitmatrix)[1]) {
        for (j in 1:length(unique(sort.splitmatrix[k, ]))) {
            sorted.effects[k, j] <- unique(sort.splitmatrix[k,])[j]
        }
    }
    sortedx <- rep("", length(x))
    for (k in 1:length(x)) {
        for (j in 1:dim(sorted.effects)[2]) {
            if (j == 1) sortedx[k] <- paste(sorted.effects[k, j])
            if (j > 1 && sortedx[k] != "" && !is.na(sorted.effects[k, j])) sortedx[k] <- paste(sortedx[k], ":", sorted.effects[k, j], sep = "")
        }
    }
    sortedx
}


############################################################################################
############# CONFOUND.TAB.FUN ######################################################
#Function to provide table of main effects which are confounded and what they are confounded with
confound.tab.fun <- function() {
    confound.main <- ""
    confound.effect <- ""
    if (no.confounded > 0) {
        for (confoundi in 1:no.confounded) {
            for (effectalli in 1:no.all) {
                if (all.nestedin.conf.table[effectalli, confoundi] == "1" & conf.nestedin.all.table[confoundi, effectalli] == "1") {
                    confound.main <- c(confound.main, confounded.main[confoundi])
                    confound.effect <- c(confound.effect, colnames(outputlistip1$designi)[effectalli])
                }
            }
        }
    }
    confound.tab <- cbind(confound.main[-1], confound.effect[-1])
}

############################################################################################
############# CONFOUND.PRINT.FUN ######################################################
#Function to to list effects which are confounded and what they are confounded with
confound.print.fun <- function(confound.tab) {
    if (nrow(confound.tab) > 0) {
        for (confoundi in 1:nrow(confound.tab)) {
            if (SilveR == "N") { cat("\nEffect ", confound.tab[confoundi, 1], " is confounded with ", confound.tab[confoundi, 2]) }
        }
    }
}


#########################################################################################
############# DESIGNORDER ######################################################
#list of effects of order i i.e. those with order i and interactions of order i-1
#construct design matrix with these effects

designorder <- function(level, orderdsi, designi_1, order.effectsi_1, level.order.effectsi_1, keep.nested.effectsi_1, keep.nested.interactionsi_1, keep.order.effectsiuniqi_1) {

    #-----------------------------------------------------------------------------------------------------------
    #Add interactions
    #putting all interactions between effects of order i-1 into order.effectsi
    if (SilveR == "N") { cat(date(), "\nPutting effects of order i-1 into order.effectsi") }
    order.effectsi <- ""
    keep.order.effectsiuniqi <- ""
    if (length(order.effectsi_1) > 1) {
        for (i in 1:length(order.effectsi_1)) {
            for (j in 1:length(order.effectsi_1)) {
                if (j > i) {
                    #zzz^cgange
                    int.name <- paste(order.effectsi_1[i], "^", order.effectsi_1[j], sep = "")
                    if (length(order.effectsi) > 1 || order.effectsi != "") {
                        order.effectsi <- c(order.effectsi, int.name)
                    } else order.effectsi <- int.name
                }
            }
        }
    }

    if (length(keep.nested.effectsi_1) > 1 || keep.nested.effectsi_1 != "") {
        if (length(order.effectsi) > 1 || order.effectsi != "") order.effectsi <- c(order.effectsi, keep.nested.effectsi_1) else
            order.effectsi <- keep.nested.effectsi_1
    }
    if (length(keep.nested.interactionsi_1) > 1 || keep.nested.interactionsi_1 != "") {
        if (length(order.effectsi) > 1 || order.effectsi != "") order.effectsi <- c(order.effectsi, keep.nested.interactionsi_1) else
            order.effectsi <- keep.nested.interactionsi_1
    }
    if (length(keep.order.effectsiuniqi_1) > 1 || keep.order.effectsiuniqi_1 != "") {
        if (length(order.effectsi) > 1 || order.effectsi != "") order.effectsi <- c(order.effectsi, keep.order.effectsiuniqi_1) else
            order.effectsi <- keep.order.effectsiuniqi_1
    }

    #-----------------------------------------------------------------------------------------------------------
    #Eliminating terms in order.effectsi which are repeats
    # First split terms into their components and eliminate repeats
    if (SilveR == "N") { cat(date(), "\nEliminating terms in order.effectsi which are repeats") }
    if (length(order.effectsi) > 1 || order.effectsi != "") {
        factor.effectsi <- array("", dim = c(length(order.effectsi), maxfacs))
        for (k in 1:length(order.effectsi)) {
            factor.veci <- sort(unique(extract.factor.fun(order.effectsi[k])))
            factor.veci <- c(factor.veci, rep("", (maxfacs - length(factor.veci))))
            factor.effectsi[k,] <- factor.veci
        }
        factor.effectsiuniq <- unique(factor.effectsi)

        #This now sorts factor.effectsiuniq so that effects with less terms come first - needed for nested algorithm
        for (j in 1:ncol(factor.effectsiuniq)) {
            factor.effectsiuniq <- rbind(factor.effectsiuniq[factor.effectsiuniq[, j] == "",], factor.effectsiuniq[factor.effectsiuniq[, j] != "",])
        }
        now.factor.effectsiuniq <- factor.effectsiuniq
        keep.factor.effectsiuniq <- factor.effectsiuniq
        #This restricts interactions to those with number of terms of level or less e.g. at level 4 interactions involve no more than 4 terms  
        now.factor.effectsiuniq <- now.factor.effectsiuniq[now.factor.effectsiuniq[, eval(level + 1)] == "",, drop = F]
        keep.factor.effectsiuniq <- keep.factor.effectsiuniq[keep.factor.effectsiuniq[, eval(level + 1)] != "",, drop = F]

        # Then merge components to give model interaction terms to be included at this level
        for (i in 1:nrow(now.factor.effectsiuniq)) {
            for (j in 1:maxfacs) {
                if (now.factor.effectsiuniq[i, j] != "") {
                    if (j == 1) {
                        int.val <- paste(mydesign[, paste(now.factor.effectsiuniq[i, j])], sep = "")
                        int.name <- now.factor.effectsiuniq[i, j]
                    }
                    else {
                        int.val <- paste(int.val, "_", mydesign[, paste(now.factor.effectsiuniq[i, j])], sep = "")
                        #zzz^change
                        int.name <- paste(int.name, "^", now.factor.effectsiuniq[i, j], sep = "")
                    }
                }
            }
            if (i == 1) {
                designiints <- array(int.val, dim = c(length(int.val), 1))
                order.effectsiuniq <- int.name
            } else {
                designiints <- cbind(designiints, int.val)
                order.effectsiuniq <- c(order.effectsiuniq, int.name)
            }
        }
        colnames(designiints) <- order.effectsiuniq
        # Then merge components to give model interaction terms to be included at lower levels

        if (nrow(keep.factor.effectsiuniq) > 0) {
            for (i in 1:nrow(keep.factor.effectsiuniq)) {
                for (j in 1:maxfacs) {
                    if (keep.factor.effectsiuniq[i, j] != "") {
                        if (j == 1) {
                            int.name <- keep.factor.effectsiuniq[i, j]
                        }
                        else {
                            #zzz^cgange
                            int.name <- paste(int.name, "^", keep.factor.effectsiuniq[i, j], sep = "")
                        }
                    }
                }
                if (i == 1) {
                    #                keep.designiints <- array(int.val,dim=c(length(int.val),1))
                    keep.order.effectsiuniq <- int.name
                } else {
                    #                keep.designiints <- cbind(keep.designiints,int.val)
                    keep.order.effectsiuniq <- c(keep.order.effectsiuniq, int.name)
                }
            }
            if (length(keep.order.effectsiuniq) > 1) {
                keep.order.effectsiuniqi <- unique(keep.order.effectsiuniq) #Not sure about this bit
            } else keep.order.effectsiuniqi <- ""
        }
    }

    #Next look for effects identified in preliminary effects order as level i
    if (SilveR == "N") { cat(date(), "\nNext look for effects identified in preliminary effects order as level i") }
    order.effectsxint <- main.effects.table.nestintnames[prelim.effect.order == orderdsi]
    order.effectsx <- rownames(main.effects.table)[prelim.effect.order == orderdsi]

    if (length(order.effectsx) > 0) {
        designi <- cbind(designi_1, mydesign[, paste(order.effectsx)])
        colnames(designi) <- c(colnames(designi_1), order.effectsxint)
    } else {
        designi <- designi_1
    }

    if (length(order.effectsi) > 1 || order.effectsi != "") designi <- cbind(designi, designiints)

    colnames(designi)

    effects.table.orderi <- structure.fun(designi)

    #-----------------------------------------------------------------------------------------------------------
    #Remove any effects which are confounded
    if (SilveR == "N") { cat(date(), "\nRemove any effects which are confounded") }
    rm.effect <- 0
    no.effectsi <- length(colnames(effects.table.orderi))
    for (i in 1:(no.effectsi - 1)) {
        for (j in (i + 1):no.effectsi) {
            if (effects.table.orderi[i, j] == "1" & effects.table.orderi[j, i] == "1") rm.effect <- c(rm.effect, j)
        }
    }

    if (length(rm.effect) > 1) {
        rm.effects <- unique(rm.effect[-1])
        designi <- designi[, - rm.effects]
        effects.table.orderi <- effects.table.orderi[-rm.effects, - rm.effects]
    }

    colnames(designi)

    order.effects <- colnames(designi)
    level.order.effectsi <- c(level.order.effectsi_1, rep(orderdsi, (length(colnames(designi)) - length(level.order.effectsi_1))))

    ##################################################################################################################
    #-----------------------------------------------------------------------------------------------------------
    #Remove any effects which are Nested and don't involve all factors in first term OR are Nested and at the same level
    #For those which are Nested and don't involve all factors in first term include their interaction lower down in Design
    #For those which are Nested and at the same level include lower down in design
    if (SilveR == "N") { cat(date(), "\nRemove any effects which are Nested") }
    rm.nested.effect <- 0
    keepa.nested.effect <- ""
    keep.nested.interaction <- ""
    no.effectsi <- length(colnames(effects.table.orderi))
    for (i in 1:(no.effectsi - 1)) {
        for (j in (i + 1):no.effectsi) {
            keep.nested.effectk <- ""
            if (level.order.effectsi[i] <= level.order.effectsi[j]) {

                i.factors <- extract.factor.fun(colnames(effects.table.orderi)[i])
                j.factors <- extract.factor.fun(colnames(effects.table.orderi)[j])
                if ((all(i.factors %in% j.factors)) & (level.order.effectsi[i] != level.order.effectsi[j])) ignore <- "y" else ignore <- "n"
                if (ignore == "n" & effects.table.orderi[i, j] == "(0)" & effects.table.orderi[j, i] == "1" & (length(i.factors) > 1 | length(j.factors) > 1)) {
                    rm.nested.effect <- c(rm.nested.effect, j) #Keeping col numbers of effects to be removed at this level

                    #Keeping note of effects to be incorporated lower down
                    if ((all(i.factors %in% j.factors)) & (level.order.effectsi[i] == level.order.effectsi[j])) {
                        keep.nested.interaction <- c(keep.nested.interaction, colnames(effects.table.orderi)[j]) #Keeping name of interaction effect which needs moving to a lower level
                    } else {
                        #Keeping name of interaction effect which needs moving to a lower level
                        keep.nested.factors <- unique(c(i.factors, j.factors))
                        for (k in 1:length(keep.nested.factors)) {
                            if (k == 1) { keep.nested.effectk <- keep.nested.factors[k] } else {
                                #zzz^change
                                keep.nested.effectk <- paste(keep.nested.effectk, "^", keep.nested.factors[k], sep = "")
                            }
                        }
                        keepa.nested.effect <- c(keepa.nested.effect, keep.nested.effectk)
                    }
                }
            }
        }
    }

    #-----------------------------------------------------------------------------------------------------------

    if (length(rm.nested.effect) > 1) {
        rm.nested.effect
        keepa.nested.effect
        keep.nested.interaction
        rm.nested.effects <- unique(rm.nested.effect[-1])
        rm.nested.effects
        designi <- designi[, - rm.nested.effects]
        effects.table.orderi <- effects.table.orderi[-rm.nested.effects, - rm.nested.effects]
    }

    if (length(keepa.nested.effect) > 1) {
        keepa.nested.effects <- unique(keepa.nested.effect[-1])
        keepa.nested.effects
    }
    if (length(keep.nested.interaction) > 1) {
        keep.nested.interactionsi <- unique(keep.nested.interaction[-1])
        keep.nested.interactionsi
    } else keep.nested.interactionsi <- ""

    if (length(keepa.nested.effect) > 1) {
        factor.nested.effectsi <- array("", dim = c(length(keepa.nested.effects), maxfacs))
        for (k in 1:length(keepa.nested.effects)) {
            nested.veci <- sort(unique(extract.factor.fun(keepa.nested.effects[k])))
            nested.veci <- c(nested.veci, rep("", (maxfacs - length(nested.veci))))
            factor.nested.effectsi[k,] <- nested.veci
        }
        nested.effectsiuniq <- unique(factor.nested.effectsi)
        nested.effectsiuniq
        order.nested.effectsiuniq <- ""

        # Then merge components to give model interaction terms
        if (SilveR == "N") { cat(date(), "\nThen merge components to give model interaction terms") }
        for (i in 1:nrow(nested.effectsiuniq)) {
            for (j in 1:maxfacs) {
                if (nested.effectsiuniq[i, j] != "") {
                    if (j == 1) { int.name <- nested.effectsiuniq[i, j] }
                    #zzz^change
                    else { int.name <- paste(int.name, "^", nested.effectsiuniq[i, j], sep = "") }
                }
            }
            if (i == 1) { order.nested.effectsiuniq <- int.name } else {
                order.nested.effectsiuniq <- c(order.nested.effectsiuniq, int.name)
            }
        }
        keep.nested.effectsi <- order.nested.effectsiuniq

    } else keep.nested.effectsi <- ""
    if (SilveR == "N") { cat(date()) }
    #-----------------------------------------------------------------------------------------------------------
    order.effects <- colnames(designi)
    level.order.effectsi <- c(level.order.effectsi_1, rep(orderdsi, (length(colnames(designi)) - length(level.order.effectsi_1))))
    order.effectsi <- order.effects[level.order.effectsi == orderdsi]
    # outputlist <- list(designi, order.effectsi, level.order.effectsi)
    outputlist <- list(designi = designi, level.order.effectsi = level.order.effectsi)
    outputlist$keep.nested.effectsi <- keep.nested.effectsi
    outputlist$keep.nested.interactionsi <- keep.nested.interactionsi
    outputlist$keep.order.effectsiuniqi <- keep.order.effectsiuniqi
    outputlist
}
#####################################################################################################
#--------------------------------------------------------------------------------------------------------------------------
#   MODEL.EFFECTS.FUN
#Used in checking degrees of freedom for confounding
#creating the equation terms with as.factor in front and replacing nested and * parts of effects with :
model.effects.fun <- function(x) {
    #The function assumes the first term is the mean which is removed
    model.effects.facfn <- x[-1]
    for (i in 1:length(model.effects.facfn)) {
        #zzz^change
        model.effects.facfn[i] <- gsub("\\^", ":", model.effects.facfn[i])
        model.effects.facfn[i] <- gsub("\\(", ":", model.effects.facfn[i])
        model.effects.facfn[i] <- gsub("\\)", "", model.effects.facfn[i])
        model.effects.facfn[i] <- gsub("\\:", "\\):as.factor\\(", model.effects.facfn[i])
        model.effects.facfn[i] = paste(" as.factor(", model.effects.facfn[i], ")", sep = "")
    }
    model.effects.facfn
}

#--------------------------------------------------------------------------------------------------------------------------
#creating the equation for evaluating degrees of freedom e.g. the aov call

model.equation.fun <- function(x) {

    #creating the equation together with dummy response
    model.equation <- paste("DummyResponse ~ ", x[1], sep = " ")
    if (length(x) > 1) {
        for (i in 2:(length(x)))
            model.equation <- paste(model.equation, x[i], sep = " + ")
    }
    model.equation
}
model.rhsequation.fun <- function(x) {
    #creating the equation without dummy response
    model.rhsequation <- paste("~ ", x[1], sep = " ")
    if (length(x) > 1) {
        for (i in 2:(length(x)))
            model.rhsequation <- paste(model.rhsequation, x[i], sep = " + ")
    }
    model.rhsequation
}
#------------------------------------------------------------------------------------
#Function to strip out "as.factor" when checking confounded degrees of freedom
strip.order.fun <- function(x) {
    x <- gsub(" ", "", x)
    x <- gsub("as.factor\\(", "", x)
    x <- gsub("\\)", "", x)
    x
}
#------------------------------------------------------------------------------------
#Function to strip out "as.factor" when checking confounded degrees of freedom from biglm
strip.order.biglm.fun <- function(x) {
    x <- gsub(" ", "", x)
    x <- gsub("as.factor", "", x)
    x <- gsub("\\(", "", x)
    #zzz^change
    x <- gsub("\\:", "\\^", x)
    x <- gsub("\\)", "", x)
    x <- gsub("Intercept", "Mean", x)
    x <- gsub("[0123456789]", "", x)
    x
}
#------------------------------------------------------------------------------------  
#Function to put interactions into alphabetical order when checking confounded degrees of freedom
sort.effectorder.fun <- function(x) {
    #zzz^change
    splitmatrixformat <- suppressWarnings(do.call(rbind, strsplit(x, "\\^")))
    sorted.effects <- apply(t(apply(as.matrix(splitmatrixformat), 1, sort)), 1, unique)
    sortedx <- rep("", length(x))
    for (k in 1:length(x)) {
        for (i in 1:length(sorted.effects[[k]])) {
            if (i == 1) sortedx[k] <- paste(sorted.effects[[k]][i])
            #zzz^change
            if (i > 1) sortedx[k] <- paste(sortedx[k], "^", sorted.effects[[k]][i], sep = "")
        }
    }
    sortedx
}
#------------------------------------------------------------------------------------
#####################################################################################################
#--------------------------------------------------------------------------------------------------------------------------
#  DSCOORDS.FUN     : Calculates coordinates for the vertices of the Hasse diagram
########################################################################################
dscoords.fun <- function(DStype) {
    if (DStype == "RDS") {
        xfinaleffects <- finaleffects
        #  xfinaldesign.effects <- names(finaleffects)
        #  xadjm <- adjm
        xceffects.table.final.brief <- ceffects.table.final.brief
    }
    if (DStype == "FDS") {
        xfinaleffects <- finalfinaleffects
        #  xfinaldesign.effects <- finaldesign.effects
        #  xadjm <- fadjm
        xceffects.table.final.brief <- finalceffects.table.final.brief
    }

    finaleffects.reverse <- c(xfinaleffects[length(xfinaleffects):2], 0, xfinaleffects[1], 0)
    coordsy <- 95 - 90 * finaleffects.reverse / finaleffects.reverse[1]
    no.perlevel.reverse <- c(table(xfinaleffects)[length(table(xfinaleffects)):2], 3) #How many effecst per level plus 3 at mean level
    max.no.perlevel <- max(no.perlevel.reverse)
    index.x <- 0
    coordsx <- rep(NA, length(coordsy))
    textlabel.size <- rep(larger.fontlabelmultiplier * 1, length(coordsy))
    textlabel.size.df <- rep(larger.fontlabelmultiplier * 0.85, length(coordsy))

    #Identifies whether terms in levels where there is only one term are nested in the term above -> single.nested
    #If so the term will be placed directly below the one above - else it will be moved
    single.nested <- rep(NA, length(no.perlevel.reverse))
    names(single.nested) <- names(no.perlevel.reverse)
    oneperlev <- which(xfinaleffects %in% names(no.perlevel.reverse[no.perlevel.reverse == 1])) #selects terms with one per level
    levs.with.one <- no.perlevel.reverse[no.perlevel.reverse == 1]
    single.nested[names(levs.with.one)[length(oneperlev)]] <- "y"
    if (length(oneperlev) != 1) {
        for (i in (length(oneperlev) - 1):1) {
            if (xceffects.table.final.brief[oneperlev, oneperlev][length(oneperlev) - i + 1, length(oneperlev) - i] == "1") {
                single.nested[names(no.perlevel.reverse) == names(levs.with.one)[i]] <- "y"
            } else {
                single.nested[names(no.perlevel.reverse) == names(levs.with.one)[i]] <- "n"
            }
        }
    }


    single.coord <- 50
    for (m in 1:length(no.perlevel.reverse)) {
        upper <- no.perlevel.reverse[m] - 1
        for (k in upper:0) {
            index.x <- index.x + 1
            if (max.no.perlevel > 2) {
                leftd <- (30 * max.no.perlevel - 28 * no.perlevel.reverse[m]) / (max.no.perlevel - 2)
                rightd <- 100 - leftd
            } else {
                leftd <- 30
                rightd <- 70
            }
            if (m == length(no.perlevel.reverse)) {
                leftd <- 0
                rightd <- 100
            }
            if (upper != 0) coordsx[index.x] <- leftd + (100 - 2 * leftd) * k / upper else {
                if (single.nested[m] == "n" && single.coord == 30) single.coord <- 50 else if (single.nested[m] == "n" && single.coord == 50) single.coord <- 30
                coordsx[index.x] <- single.coord
            }
            if (upper > 4) textlabel.size[index.x] <- smaller.fontlabelmultiplier * 0.5
            if (upper > 4) textlabel.size.df[index.x] <- smaller.fontlabelmultiplier * 0.5
            if ((length(grep("=", colnames(xceffects.table.final.brief)[length(colnames(xceffects.table.final.brief)):1][index.x])) > 0) && (textlabel.size[index.x] == smaller.fontlabelmultiplier * 0.5)) {
                textlabel.size[index.x] <- middle.fontlabelmultiplier * 0.75
            }
        }
    }

    coords.output <- list(coords = cbind(coordsx, coordsy), textlabel.size = textlabel.size, textlabel.size.df = textlabel.size.df)
    coords.output
}
#########################################################################################################
#####################################################################################################
#--------------------------------------------------------------------------------------------------------------------------
#  DFS.FUN     : Calculates degrees of freedom by subtraction method and checks for shared degrees of freedom
########################################################################################
dfs.fun <- function(DStype) {

    if (DStype == "RDS") {
        xfinaleffects <- finaleffects
        xfinaldesign.effects <- names(finaleffects)
        xadjm <- adjm
        xceffects.table.final.brief <- ceffects.table.final.brief
    }
    if (DStype == "FDS") {
        xfinaleffects <- finalfinaleffects
        xfinaldesign.effects <- finaldesign.effects
        xadjm <- fadjm
        xceffects.table.final.brief <- finalceffects.table.final.brief
    }
    #set up matrix for degrees of freedom, 1st col = Tier, 2nd col=max number of levels, 3rd col = actual number of levels, 4th col = dfs
    xdfs <- matrix(NA, nrow = length(xfinaleffects), ncol = 4)
    rownames(xdfs) <- names(xfinaleffects)
    colnames(xdfs) <- c("Tier", "Maxlev", "Actlev", "DFs")
    xdfs[, 1] <- xfinaleffects
    #Provides number of levels present in the design - all effects and individual factors - restricts this to final effects for FDS
    numberlevs <- apply(outputlistip1$designi, 2, function(x) { length(unique(x)) })
    if (DStype == "RDS") { xdfs[, 3] <- numberlevs } else if (DStype == "FDS") {
        xdfs[, 3] <- apply(outputlistip1$designi, 2, function(x) { length(unique(x)) })[finalnames.effects %in% xfinaldesign.effects]
    }

    #Set 1 degree of freedom for Mean
    xdfs[1, 2] <- 1
    xdfs[1, 4] <- 1
    #Find number of levels and dfs for first tier effects
    for (i in 1:length(xfinaleffects)) {
        if (xfinaleffects[i] == 1) {
            xdfs[i, 2] <- xdfs[i, 3]
            xdfs[i, 4] <- xdfs[i, 3] - 1
        }
    }
    #--------------------------------------------------------------------------------------------------------
    #Find max levels
    #name effects in final format e.g.nested names rather than as interactions
    if (DStype == "FDS") {
        #  names(xfinaleffects)<- finalnames.effects
        names(numberlevs) <- finalnames.effects
    }
    if (DStype == "RDS") {
        names(numberlevs) <- names(xfinaleffects)
    }
    numberlevs <- c("-" = 1, numberlevs)

    #First split finaleffects in DS into individual terms
    factor.finaldfeffectsi <- array("-", dim = c(length(xfinaleffects), maxfacs))
    for (k in 1:length(xfinaleffects)) {
        factor.finaldfveci <- sort(unique(extract.factor.fun(names(xfinaleffects)[k])))
        factor.finaldfveci <- c(factor.finaldfveci, rep("-", (maxfacs - length(factor.finaldfveci))))
        factor.finaldfeffectsi[k,] <- factor.finaldfveci
    }

    #Now multiply levels in each independent term to get max number of levels
    maxlevels <- rep(1, nrow(factor.finaldfeffectsi))
    maxlevelsf <- rep("", nrow(factor.finaldfeffectsi))
    for (i in 1:nrow(factor.finaldfeffectsi)) {
        for (j in 1:ncol(factor.finaldfeffectsi)) {
            maxlevels[i] <- maxlevels[i] * numberlevs[factor.finaldfeffectsi[i, j]]
            if (factor.finaldfeffectsi[i, 2] != "-" && maxlevels.df == "Y") maxlevelsf[i] <- paste(sep = "", "(", maxlevels[i], ")")

        }
    }
    xdfs[, 2] <- maxlevels

    #For effects below tier 1 sum the degrees of freedom for effects above and substract from actual number of levels
    for (m in eval(length(xfinaleffects[xfinaleffects < 2]) + 1):length(xfinaleffects)) {
        xdfs[m, 4] <- xdfs[m, 3] - sum(xadjm[m, 1:length(xfinaleffects[xfinaleffects < xfinaleffects[m]])] * xdfs[1:length(xfinaleffects[xfinaleffects < xfinaleffects[m]]), 4])
    }


    #################################################################################################################
    #Looking for confounded degrees of freedom
    #############################################################################################################
    #--------------------------------------------------------------------------------------------------------------

    #call function to construct the equation terms with as.factor in front and replacing nested and * parts of effects with :
    model.effects.fac <- model.effects.fun(xfinaldesign.effects)

    #--------------------------------------------------------------------------------------------------------------------------
    #Compare degrees of freedom from program with rank of model terms except last, try to indicate where they may be
    if (check.shared.df == "Y") {
        #Remove bottom/residual term
        test.terms <- xceffects.table.final.brief[-nrow(xceffects.table.final.brief), - nrow(xceffects.table.final.brief)]
        #Removes rows and columns nested within higher terms, except the mean
        for (i in nrow(test.terms):2) {
            if (any(test.terms[, i] == 1)) test.terms <- test.terms[-i, - i]
        }
        test.terms2 <- rep("NA", length(rownames(test.terms)))
        #Takes out terms which nest others
        for (k in 1:length(rownames(test.terms))) {
            test.terms2[k] <- extract.nestfactor.fun(rownames(test.terms)[k])
        }
        #Puts as.factor in front of each term
        model.terms.fac.test <- model.effects.fun(test.terms2)

        #call function to construct the righthand side of equation terms with as.factor in front and replacing nested and * parts of effects with :
        model.rhsequation <- model.rhsequation.fun(model.terms.fac.test)
        #Calculates rank of model terms ignoring the last term (residual)
        tryCatch(
       Rank_Notresid <- qr(model.matrix.default(as.formula(model.rhsequation), data = mydesign))$rank,
       error = function(err) {
        # warning handler picks up where error was generated
        print(paste("CALCULATING_RANK_ERROR:  ", err))
        Error <<- err
       }, finally = {
            Rank_Notresid <- NA
        }
       )
        if (exists("Error")) {
            #      cat ("If the Error refers to too large a size for vector it is likely that the number of model terms and number of levels is too large
            #         This means that the degrees of freedom could not be checked for confounding between terms and model fitting is likely to be a problem.")
            testmess0 <- "An error has occurred"
            testmess1 <- "If the Error refers to too large a size for vector it is likely that the number of model terms and number of levels is too large."
            testmess2 <- "This means that the degrees of freedom could not be checked for confounding between terms and model fitting is likely to be a problem."
            if (SilveR == "N") {
                print(testmess0)
                print(Error)
                print(testmess1)
                print(testmess2)
            }
            if (SilveR == "Y") {
                HTML.title("<bf>Comment", HR = 2, align = "left")
                HTML.title("<bf> ", HR = 2, align = "left")
                HTML.title(testmess, HR = 0, align = "left")
            }

        } else {
            Rank_Notresid <- qr(model.matrix.default(as.formula(model.rhsequation), data = mydesign))$rank
            #Compares rank with number of degrees of freedom calculated by method
            DFs_Notresid <- sum(xdfs[-nrow(xdfs), 4])
            xDFsDiff <- DFs_Notresid - Rank_Notresid
        }

        #If the degrees of freedom differ then it tries to find out where using aov


        #This now attempts to re-calculate the degrees of freedom and shared degrees of freedom
        if (exists("xDFsDiff")) {
            if (SilveR == "N") { cat("\nThere are ", xDFsDiff, " confounded degrees of freedom\n") }
            if (SilveR == "Y") {
                add <- paste("There are ", xDFsDiff, " confounded degrees of freedom", sep = "")

                if (xDFsDiff > 0) {
                    HTML.title("<bf>Warning", HR = 2, align = "left")
                    HTML.title("<bf> ", HR = 2, align = "left")
                    HTML.title(add, HR = 0, align = "left")
                }
            }

            if (xDFsDiff > 0) {
                tiers <- as.numeric(names(table(xdfs[, 1])))

                # For each tier, take model of all terms in that tier and below, drop each term and record dfs
                # For each term the max dfs is the number of dfs related to the term, diff dfs=max dfs-min dfs is the number of shared dfs
                #Set up matrix to contain df results
                colnames.tier.dfs <- rep("", length(tiers))
                for (k in 1:length(tiers) - 1) {
                    colnames.tier.dfs[k] <- paste("dropdfs", k, sep = "")
                }
                colnames.tier.dfs[length(tiers)] <- "sharedf"
                colnames.tier.dfs[length(tiers) + 1] <- "sharedfs"
                colnames.tier.dfs[length(tiers) + 2] <- "reviseddfs"
                tier.dfs <- matrix(NA, nrow = nrow(xceffects.table.final.brief) - 1, ncol = length(tiers) + 2, dimnames = list(rownames(xceffects.table.final.brief)[-1], colnames.tier.dfs))

                for (i in 2:length(tiers)) {
                    test.termsi <- rownames(xceffects.table.final.brief[xdfs[, 1] <= tiers[i], xdfs[, 1] <= tiers[i]]) #select terms <= tier[i]
                    #Puts as.factor in front of each term
                    model.terms.fac.testi <- model.effects.fun(test.termsi)
                    #call function to construct the righthand side of equation terms with as.factor in front and replacing nested and * parts of effects with :
                    model.equation <- model.equation.fun(model.terms.fac.testi)
                    mydata$DummyResponse = sample(1:100, dim(mydata)[1], replace = T)
                    #fit the aov model
                    suppressWarnings(droptermi <- dropterm(aov(as.formula(model.equation), data = mydata), test = "F")[1])
                    dropi <- droptermi[-1,]
                    if (any(grepl(":", rownames(droptermi)[-1]))) {
                        #checks of any terms are interactions
                        #zzz^change
                        namedropi <- gsub("\\:", "\\^", sortaov.term.fun(as.vector(strip.order.fun(rownames(droptermi)[-1]))))
                        #zzz^change
                    } else { namedropi <- gsub("\\:", "\\^", as.vector(strip.order.fun(rownames(droptermi)[-1]))) }
                    names(dropi) <- namedropi
                    dropi
                    #This finds nonested names and sorts interactions in alphabetical order    
                    nonnested.rownames.tier.dfs <- rownames(tier.dfs)
                    for (j in 1:length(rownames(tier.dfs))) {
                        #zzz^change
                        nonnested.rownames.tier.dfs[j] <- gsub("\\(", "^", nonnested.rownames.tier.dfs[j])
                        nonnested.rownames.tier.dfs[j] <- gsub("\\)", "", nonnested.rownames.tier.dfs[j])
                    }
                    tier.dfs[rownames(tier.dfs)[which(sort.effectorder.fun(nonnested.rownames.tier.dfs) %in% names(dropi))], i - 1] <- dropi

                }
                #Puts column sharedf = 0 if no shared dfs, 1 if shared dfs
                tier.dfs[(apply(tier.dfs[, - length(tiers)], 1, min, na.rm = T) == xdfs[-1, 4]), length(tiers)] <- 0
                tier.dfs[(apply(tier.dfs[, - length(tiers)], 1, min, na.rm = T) != xdfs[-1, 4]), length(tiers)] <- 1
                mindrop.dfs <- apply(tier.dfs[, - length(tiers)], 1, min, na.rm = T)
                tier.dfs <- cbind(tier = xdfs[-1, 1], tier.dfs)

                #Finds first tier with shared degrees of freedom
                sharetier <- 0
                for (i in 1:length(tiers)) {
                    if (sharetier == 0 && any(tier.dfs[tier.dfs[, 1] == i, length(tiers) + 1] == 1)) sharetier <- i
                }
                #Puts dfs into column reviseddfs in tier.dfs up to and including the shared tier
                tier.dfs[tier.dfs[, 1] <= sharetier, length(tiers) + 3] <- xdfs[-1,][tier.dfs[, 1] <= sharetier, 4]

                for (i in 1:nrow(tier.dfs)) {
                    if (tier.dfs[i, 1] > sharetier) {
                        coliname <- paste("term", i, sep = "")
                        tier.dfs <- cbind(tier.dfs, rep(NA, nrow(tier.dfs)))
                        colnames(tier.dfs)[ncol(tier.dfs)] <- coliname
                        posterms <- sort.effectorder.fun(nonnested.rownames.tier.dfs[1:i])
                        modeli <- c("mean", posterms[c(xadjm[i + 1, 2:length(finaleffects[finaleffects < finaleffects[i + 1]])] == 1, T)])
                        model.testi <- model.effects.fun(modeli)
                        #call function to construct the righthand side of equation terms with as.factor in front and replacing nested and * parts of effects with :
                        model.equationi <- model.equation.fun(model.testi)
                        mydata$DummyResponse = sample(1:100, dim(mydata)[1], replace = T)
                        suppressWarnings(anovai <- anova(aov(as.formula(model.equationi), data = mydata), test = "F")[1])
                        anovai.df <- anovai[-nrow(anovai), 1] #need to sort and put : to *
                        names(anovai.df) <- rownames(anovai)[-nrow(anovai)]

                        if (any(grepl(":", names(anovai.df)))) {
                            #checks of any terms are interactions
                            #zzz^change
                            nameanovai <- gsub("\\:", "\\^", sortaov.term.fun(as.vector(strip.order.fun(names(anovai.df)))))
                            #zzz^change
                        } else { nameanovai <- gsub("\\:", "\\^", as.vector(strip.order.fun(names(anovai.df)))) }
                        names(anovai.df) <- nameanovai

                        anovai.dfs <- rep(0, length(modeli) - 1)
                        names(anovai.dfs) <- modeli[-1]
                        anovai.dfs[names(anovai.dfs) %in% names(anovai.df)] <- anovai.df
                        anovai.dfs

                        tier.dfs[rownames(tier.dfs)[which(sort.effectorder.fun(nonnested.rownames.tier.dfs) %in% names(anovai.dfs))], ncol(tier.dfs)] <- anovai.dfs
                        tier.dfs[i, length(tiers) + 3] <- tier.dfs[i, ncol(tier.dfs)]
                    }
                }
                tier.dfs[, length(tiers) + 2] <- tier.dfs[, length(tiers) + 3] - mindrop.dfs
                print.dfs <- cbind(actual.levs = xdfs[-1, 3], dfs.by.subtract = xdfs[-1, 4], revised.dfs = tier.dfs[, length(tiers) + 3], shared.dfs = tier.dfs[, length(tiers) + 2])

                if (DStype == "RDS") {
                    #    cat("\n Confounded degrees of freedom often indicate that the design terms are not specified
                    #     appropriately or that the design needs to be changed. However, sometimes once appropriate randomisation is performed,
                    #     the randomised design structure no longer contains confounded degrees of freedom. Investigate the shared.dfs column in the following table 
                    #      which shows where shared degrees of freedom occur and/or proceed to the randomisation. The revised.dfs column 
                    #      corrects the dfs for terms which are underestimated by the subtraction method (because of shared dfs in terms above). 
                    #      It does not adjust individual terms for confounded degrees of freedom. \n\n")

                    message <- "Confounded degrees of freedom often indicate that the design terms are not specified appropriately or that the design needs to be changed. However, sometimes once appropriate randomisation is performed, the Layout Structure no longer contains confounded degrees of freedom. Investigate the shared.dfs column in the following table which shows where shared degrees of freedom occur and/or proceed to the randomisation. The revised.dfs column corrects the dfs for terms which are underestimated by the subtraction method (because of shared dfs in terms above). It does not adjust individual terms for confounded degrees of freedom."


                    if (SilveR == "N") { print(message) }
                    if (SilveR == "Y") {
                        #   HTML.title("<bf>Warning", HR=2, align="left")
                        HTML.title("<bf> ", HR = 2, align = "left")
                        HTML.title(message, HR = 0, align = "left")
                    }
                }

                if (DStype == "FDS") {
                    #    cat("\n Confounded degrees of freedom often indicate that the design terms are not specified
                    #     appropriately (for example a factor or pseudofactor or supremum may have been missed from the design supplied)
                    #      or that the design needs to be changed. Investigate the shared.dfs column in the following table 
                    #      which shows where shared degrees of freedom occur and/or proceed to the randomisation. The revised.dfs column 
                    #      corrects the dfs for terms which are underestimated by the subtraction method (because of shared dfs in terms above). 
                    #      It does not adjust individual terms for confounded degrees of freedom. \n\n")

                    message <- "Confounded degrees of freedom often indicate that the design terms are not specified appropriately (for example a factor or pseudofactor or supremum may have been missed from the design supplied) or that the design needs to be changed. Investigate the shared.dfs column in the following table which shows where shared degrees of freedom occur and/or proceed to the randomisation. The revised.dfs column corrects the dfs for terms which are underestimated by the subtraction method (because of shared dfs in terms above). It does not adjust individual terms for confounded degrees of freedom."

                    if (SilveR == "N") { print(message) }
                    if (SilveR == "Y") {
                        #  HTML.title("<bf>Warning", HR=2, align="left")
                        HTML.title("<bf> ", HR = 2, align = "left")
                        HTML.title(message, HR = 0, align = "left")
                    }
                }

                if (SilveR == "N") { print(print.dfs) }
                if (SilveR == "Y") {
                    #print.dfs2<-print(print.dfs)
                    HTMLbr()
                    #HTML(print.dfs2 , classfirstline="second", align="left")
                    HTML(print.dfs, classfirstline = "second", align = "left")
                }

            }
        }


    }

    xdfs.reverse <- rbind(xdfs[nrow(xdfs):2,], c(0, 0, 0, 0), Mean = xdfs[1,], c(0, 0, 0, 0)) #Need to add in dummydfs for dummy nodes
    maxlevelsf.reverse <- c(maxlevelsf[nrow(xdfs):2], "", "", "")
    dfs.fun.output <- list(xdfs = xdfs, xdfs.reverse = xdfs.reverse, maxlevelsf.reverse = maxlevelsf.reverse)


}



##########################################################################################################
#              NO CHANGES REQUIRED BELOW THIS POINT 
##########################################################################################################

# Checks whether any factors levels are missing
anyna <- function(x) { any(is.na(x)) }
if (length(colnames(mydesign)[apply(mydesign, 2, anyna)]) > 0) {
    cat("The following variables have missing values. Please check and complete the dataset \n", colnames(mydesign)[apply(mydesign, 2, anyna)])
    options(show.error.messages = F)
    if (pdf == "Y") dev.off()
    stop()
}

#Checks whether factor names are contained within another factor name
#XXXXXXXXSTILL NEED TO DO THIS
#input   <- c("Mean","Analyst","Batch", "Site", "EquipmentSite")
#length <-length(input)
#
#for (i in 1:10)
#{
#                for (j in 1:(length-1))
#                {
#                name1 <-input[j]
#                name1_<-paste(name1, "_", sep="")
#                                for (k in (j+1):length)
#                                {
#                                                name2 <-input[k]
#                                                name2_<-sub(name1,name1_,name2)
#
#                                                if( name2_ != name2)
#                                                {
#                                                                input[j]<-name1_
#                                                }
#                                }
#                }
#}
#input



# Need factor names to have unique two letter abbreviations else increase to 3
abbrev.length <- rep(2, length(colnames(mydesign)[-1]))
names(abbrev.length) <- colnames(mydesign)[-1]

#Silver Warning message in output
if (max(table(substring(colnames(mydesign)[-1], 1, 2))) > 1 && max(table(substring(colnames(mydesign), 1, 3))) > 1 && SilveR == "Y") {
    HTMLbr()
    HTML.title("<bf>Warning", HR = 2, align = "left")

    HTML.title("<bf> ", HR = 2, align = "left")
    HTML.title("<bf>  The program abbreviates factor names using the first 2 or 3 letters. It has found that two factors have the same abbreviation. Please rename to avoid this issue.", HR = 0, align = "left")
}



if (max(table(substring(colnames(mydesign)[-1], 1, 2))) > 1) {
    if (max(table(substring(colnames(mydesign), 1, 3))) > 1) {
        cat("The program abbreviates factor names using the first 2 or 3 letters.
          It has found that two factors have the same abbreviation. 
          Please rename to avoid this issue. \n\n")
        options(show.error.messages = F)
        if (pdf == "Y") dev.off()
        stop()
    } else {
        abbrev.length[substring(names(abbrev.length), 1, 2) %in% (names(table(substring(colnames(mydesign), 1, 2)))[table(substring(colnames(mydesign), 1, 2)) == 2])] <- 3
    }
}

#Check for confounded main effects
main.effects.table <- structure.fun(mydesign)

rm.effecti <- 0
rm.effectj <- 0
no.effectsi <- length(colnames(main.effects.table))
for (i in 1:(no.effectsi - 1)) {
    for (j in (i + 1):no.effectsi) {
        if (main.effects.table[i, j] == "1" & main.effects.table[j, i] == "1") {
            rm.effecti <- c(rm.effecti, i)
            rm.effectj <- c(rm.effectj, j)
        }
    }
}






if (length(rm.effecti) > 1) {
    rm.effecti <- rm.effecti[-1]
    rm.effectj <- rm.effectj[-1]

    for (k in (1:length(rm.effecti))) {
        cat(colnames(main.effects.table)[rm.effecti[k]], " is confounded with ", colnames(main.effects.table)[rm.effectj[k]], "\n\n")
    }

    #Silver Warning message in output
    if (SilveR == "Y") {
        if (max(table(rm.effecti)) > 1 || max(table(rm.effectj)) > 1) {
            HTMLbr()
            HTML.title("<bf>Warning", HR = 2, align = "left")
            HTML.title("<bf> ", HR = 2, align = "left")
            HTML.title("<bf>  There are three (or more) main effects which are confounded. The program only processes single or pairs of equivalent factors. Please remove at least one factor from any triple set of equivalent factors.", HR = 0, align = "left")
        } else {
            HTML.title("<bf>Warning", HR = 2, align = "left")
            HTML.title("<bf> ", HR = 2, align = "left")
            HTML.title("<bf>  There are main effects which are confounded. If the confounding is unintentional review the design and correct as appropriate. The program will proceed by using only one factor for each equivalent pair of factors.", HR = 0, align = "left")
        }
    }

    if (max(table(rm.effecti)) > 1 || max(table(rm.effectj)) > 1) {
        cat("There are three (or more) main effects which are confounded.
          	The program only processes single or pairs of equivalent factors.
          	Please remove at least one factor from any triple set of equivalent factors\n\n")
        options(show.error.messages = F)

        if (pdf == "Y") dev.off()
        stop()
    } else {
        cat("There are main effects which are confounded.
             		If the confounding is unintentional review the design and correct as appropriate.
             		The program will proceed by using only one factor for each equivalent pair of factors\n\n")
    }

    # Only one factor is retained for each pair of equivalent (completely confounded) factors 
    cat("\nInitial rows of Data\n")
    print(head(mydesign))
    mydesign <- mydesign[, - (rm.effectj)] #Removes additional equivalent factor
    randomfacsid <- randomfacsid[-(rm.effectj - 1)] #Removes the unnecessary identification of fixed/random
    cat("\nInitial rows of Design excluding all but one of equivalent factors\n")
    print(head(mydesign))
    confound.factor <- cbind(colnames(main.effects.table)[rm.effectj], colnames(main.effects.table)[rm.effecti]) #retains equivalent factors in table to combine with confound.tab later            
}



#Provides number of levels present in each factor in the design to check whether there is a residual (obs.unit) factor
levs <- apply(mydesign, 2, function(x) { length(unique(x)) })
resid.yn <- "Y"
if (nrow(mydesign) %in% levs) resid.yn <- "N"
if (resid.yn == "Y") {
    mydesign <- cbind(mydesign, obs.unit = (1:nrow(mydata)))
    abbrev.length <- c(abbrev.length, obs.unit = 8)
}

maxfacs <- length(colnames(mydesign))
#STB should the cbind also be done for SilvR - why is resdi not defined as random in the SilvR version?
#Show which factors are random or fixed (1=random, 0=fixed) and create a vector random factors
randomfacsidm <- c(0, randomfacsid) #Adds a 0 (fixed) for Mean
if (resid.yn == "Y") randomfacsidm <- c(randomfacsidm, 1) #Adds a 1 (random) for residual
if (SilveR == "N") { cbind(randomfacsidm, colnames(mydesign)) }
randomfacs <- colnames(mydesign)[randomfacsidm == 1]
if (SilveR == "N") { randomfacs }



#################################################################################################################

keep.order.effects0 <- "Mean"

#Construct main effects table and find preliminary effect order
main.effects.table <- structure.fun(mydesign)

orig.order <- order(rownames(main.effects.table))
xtable <- main.effects.table[orig.order, orig.order] #sort into alphabetical order

prelim.effect.order <- c(rep(NA, nrow(xtable)))
for (i in 1:nrow(main.effects.table))
    prelim.effect.order[i] <- length(xtable[i, (xtable[i,] == "1")])
if (SilveR == "N") { prelim.effect.order }
xtable.print <- as.data.frame(cbind(xtable, prelim.effect.order))

#Sorts the factors into preliminary effect order
sort.prelim <- order(prelim.effect.order)
sort.ord <- c(1:nrow(xtable))[orig.order][sort.prelim]

main.effects.table <- main.effects.table[sort.ord, sort.ord]
main.effects.table.print <- as.data.frame(cbind(main.effects.table, prelim.effect.order = prelim.effect.order[sort.prelim]))
prelim.effect.order <- prelim.effect.order[sort.prelim]

#STB
if (SilveR == "N") { colnames(mydesign)[sort.ord] }
###################################################

main.effects.table.print <- data.frame(lapply(main.effects.table.print, as.character), stringsAsFactors = FALSE)
rownames(main.effects.table.print) <- rownames(main.effects.table)
main.effects.table.print[1, (2:(ncol(main.effects.table.print) - 1))] <- "(0)"
if (SilveR == "N") { main.effects.table.print }
main.effects.table.nestnames <- rownames(main.effects.table)
main.effects.table.nestintnames <- rownames(main.effects.table)

for (i in 2:length(rownames(main.effects.table))) {
    first <- 1
    for (j in 2:length(rownames(main.effects.table))) {
        if (main.effects.table[i, j] == 1 & i != j) {
            if (first == 1) { main.effects.table.nestnames[i] <- paste(main.effects.table.nestnames[i], "(", rownames(main.effects.table)[j], sep = "") } else
                #zzz^change
                main.effects.table.nestnames[i] <- paste(main.effects.table.nestnames[i], "^", rownames(main.effects.table)[j], sep = "")
            first <- 0
            #zzz^change
            main.effects.table.nestintnames[i] <- paste(main.effects.table.nestintnames[i], "^", rownames(main.effects.table)[j], sep = "")
        }
    }
    if (first == 0) { main.effects.table.nestnames[i] <- paste(main.effects.table.nestnames[i], ")", sep = "") }
}


if (SilveR == "N") { main.effects.table.nestnames }
if (SilveR == "N") { main.effects.table.nestintnames }


##########################################################################################################
#Find effects in effect order 1
orderi <- 1
order.effects1 <- rownames(main.effects.table[prelim.effect.order == orderi,, drop = F])
order.effects <- c("Mean", order.effects1) #Add in effects from lower order i.e. order 0 = Mean
level.order.effects1 <- c(0, rep(1, length(order.effects1))) #identify which order the effect is added
design1 <- mydesign[, order.effects]
effects.table.order1 <- structure.fun(design1)

############################################################################################################
#list of effects of order 2 i.e. those with order 2 and interactions of order 1
#construct design matrix with these effects
orderi <- 2
outputlist2 <- designorder(2, orderi, design1, order.effects1, level.order.effects1, "", "", "")
level.order.effectsi_1 <- level.order.effects1
outputlistip1 <- outputlist2
order.effectsi_1 <- order.effects1

if (SilveR == "N") { date() }

###############################################################################################


for (level in 3:eval(maxfacs - 1)) {
    if (SilveR == "N") { cat(date(), "\nEvaluating effects of order ", level, ", from a total of ", eval(maxfacs - 1)) }
    outputlisti <- outputlistip1
    designi <- outputlisti$designi
    level.order.effectsi <- c(level.order.effectsi_1, rep(eval(orderi), (length(colnames(designi)) - length(level.order.effectsi_1))))
    orderip1 <- eval(orderi + 1)
    order.effectsi <- colnames(designi)[level.order.effectsi == orderi]

    ###############################################################################################################
    #list of effects of order i i.e. those with order i and interactions of order i-1
    #construct design matrix with these effects

    outputlistip1 <- designorder(level, orderip1, designi, order.effectsi, level.order.effectsi, outputlisti$keep.nested.effectsi, outputlisti$keep.nested.interactionsi, outputlisti$keep.order.effectsiuniqi)

    level.order.effectsi_1 <- level.order.effectsi
    order.effectsi_1 <- order.effectsi
    orderi <- orderip1
}


#-----------------------------------------------------------------------------------------------------------   
if (SilveR == "N") { date() }

#Associating
finaleffects <- outputlistip1$level.order.effectsi
names(finaleffects) <- colnames(outputlistip1$designi)
#Establish which final effects are random assuming that an interaction of two fixed effects is fixed
finaleffectrandom <- rep(NA, length(names(finaleffects)))
for (m in 1:length(names(finaleffects))) {
    finaleffectrandom[m] <- 0
    if (length(randomfacs) > 0) {
        for (i in 1:length(randomfacs)) {
            finaleffectrandom[m] <- max(grep(randomfacs[i], names(finaleffects)[m]), finaleffectrandom[m])
        }
    }
}
names(finaleffectrandom) <- names(finaleffects)

#Find the relationship between all effects in the unrandomised design structure
effects.table.final <- structure.fun(outputlistip1$designi)
#Abbreviate the column headings for two tables - one without confounded names and one with
#Also replace 0 in row for Mean by (0)
effects.table.final.brief <- effects.table.final
effects.table.final.brief[1, 2:ncol(effects.table.final.brief)] <- "(0)"
ceffects.table.final.brief <- effects.table.final
ceffects.table.final.brief[1, 2:ncol(effects.table.final.brief)] <- "(0)"
brief.colnames <- sapply(colnames(effects.table.final), brief.effect.fun)
colnames(effects.table.final.brief) <- brief.colnames
colnames(ceffects.table.final.brief) <- brief.colnames


#finding main effects which are confounded with other effects
confounded.main <- colnames(mydesign)[sort.ord[-1]][!(colnames(mydesign)[sort.ord[-1]] %in% colnames(outputlistip1$designi))]

no.confounded <- length(confounded.main)
no.all <- length(colnames(outputlistip1$designi))
conf.nestedin.all.table <- matrix(" ", nrow = no.confounded, ncol = no.all)
all.nestedin.conf.table <- matrix(" ", nrow = no.all, ncol = no.confounded)
#First find out which final effects are nested within the confounded main effects
for (effectalli in 1:no.all) {
    if (no.confounded > 0) {
        for (confoundi in 1:no.confounded) {
            freq.table <- table(mydesign[, sort.ord][, confounded.main[confoundi]], outputlistip1$designi[, effectalli])
            nestk <- vector(, length = nrow(freq.table)) #Set up factor for all k levels of factorj to indicate whether nested within factor i
            for (k in 1:nrow(freq.table)) {
                if (min(freq.table[k, ]) == 0 && table(freq.table[k, ])[1] + 1 == ncol(freq.table)) nestk[k] <- 1 else
                    if (min(freq.table[k, ]) == 0) nestk[k] <- 0.5 else nestk[k] <- 0
            }
            conf.nestedin.all.table[confoundi, effectalli] <- if (all(nestk == 1)) "1" else if (all(nestk == 0)) "0" else "(0)"
        }
    }
}

#Second find out which confounded main effects are nested within the final effects
if (no.confounded > 0) {
    for (confoundi in 1:no.confounded) {
        for (effectalli in 1:no.all) {

            freq.table <- table(outputlistip1$designi[, effectalli], mydesign[, sort.ord][, confounded.main[confoundi]])

            nestk <- vector(, length = nrow(freq.table)) #Set up factor for all k levels of factorj to indicate whether nested within factor i
            for (k in 1:nrow(freq.table)) {
                if (min(freq.table[k, ]) == 0 && table(freq.table[k, ])[1] + 1 == ncol(freq.table)) nestk[k] <- 1 else
                    if (min(freq.table[k, ]) == 0) nestk[k] <- 0.5 else nestk[k] <- 0
            }
            all.nestedin.conf.table[effectalli, confoundi] <- if (all(nestk == 1)) "1" else if (all(nestk == 0)) "0" else "(0)"
        }
    }
}

#Get table of main effects which are confounded and what they are confounded with
confound.tab <- confound.tab.fun()

#If there are factors confounded with each other need to add this to the third column of confound.tab, else third col will equal first column
confound.tab <- cbind(confound.tab, confound.tab[, 1])
if (exists("confound.factor")) {
    confound.factor <- cbind(confound.factor, confound.factor[, 1])
    if (dim(confound.tab)[1] > 0) {
        confound.factork <- NULL
        #This checks for equivalent factors also equivalent to an interaction
        for (j in 1:nrow(confound.tab)) {
            for (k in 1:nrow(confound.factor)) {
                if (confound.tab[j, 3] == confound.factor[k, 2]) {
                    confound.tab[j, 3] <- paste(confound.factor[k, 3], "=", confound.tab[j, 3], sep = "")
                    confound.factork <- c(confound.factork, k)
                    confound.factork.y <- "Y"
                }
            }
        }
        #f there are factors confounded with each other need to add this to confound.tab
        if (exists("confound.factork.y")) {
            confound.tab <- rbind(confound.factor[-confound.factork,], confound.tab)
        }
    } else { confound.tab <- confound.factor }
}


########################################################################################################


#produces matrix of confounded main effects and effects confounded with
if (no.confounded > 0) {
    contain <- rep(NA, nrow(confound.tab))
    for (i in 1:nrow(confound.tab)) {
        contain[i] <- confound.tab[i, 1] %in% extract.factor.fun(confound.tab[i, 2])
    }
    confound.tab <- cbind(confound.tab, contain)
    if (SilveR == "N") { confound.tab }
} else {
    confound.tab <- cbind(confound.tab, confound.tab[, 3])

}

contain.false <- confound.tab[confound.tab[, 4] == "FALSE", 1] #Selects factors equivalent to an interaction which doesn't contain them
confound.tab <- cbind(confound.tab, inclequiv.factors = confound.tab[, 2]) #puts in last column original names including names of equivalent factors

#Removes equivalent factors from names of interactions
for (i in 1:length(contain.false)) {
    if ((length(contain.false) > 0) && (contain.false[i] != confound.tab[i, 1])) {
        #zzz^change
        removeb <- paste("\\^", contain.false[i], sep = "")
        #zzz^change
        removea <- paste(contain.false[i], "\\^", sep = "")
        #zzz^change
        removeb.brief <- paste("\\^", substring(contain.false[i], 1, abbrev.length[contain.false[i]]), sep = "")
        #zzz^change
        removea.brief <- paste(substring(contain.false[i], 1, abbrev.length[contain.false[i]]), "\\^", sep = "")
        confound.tab[, 2] <- gsub(paste(removeb), "", confound.tab[, 2])
        confound.tab[, 2] <- gsub(paste(removea), "", confound.tab[, 2])
        names(finaleffects) <- gsub(paste(removea), "", names(finaleffects))
        names(finaleffects) <- gsub(paste(removeb), "", names(finaleffects))
        brief.colnames <- gsub(paste(removea.brief), "", brief.colnames)
        brief.colnames <- gsub(paste(removeb.brief), "", brief.colnames)
        colnames(effects.table.final.brief) <- gsub(paste(removea.brief), "", colnames(effects.table.final.brief))
        colnames(effects.table.final.brief) <- gsub(paste(removeb.brief), "", colnames(effects.table.final.brief))
        rownames(effects.table.final.brief) <- gsub(paste(removea), "", rownames(effects.table.final.brief))
        rownames(effects.table.final.brief) <- gsub(paste(removeb), "", rownames(effects.table.final.brief))

    }
}
if (SilveR == "N") {

    confound.tab


    # An interaction might be expected to have the same random/fixed designation of an equivalent factor. This code checks.
    # Checks random factors

    for (i in 1:nrow(confound.tab)) {
        if ((confound.tab[i, 1] %in% randomfacs) && (finaleffectrandom[confound.tab[i, 2]] == 0)) {
            cat("\n", confound.tab[i, 1], " was defined as random but it is equivalent to ", names(finaleffectrandom[confound.tab[i, 2]]),
        " which has all its component factors as fixed. \n", names(finaleffectrandom[confound.tab[i, 2]]),
        " will be assumed to be random to align with", confound.tab[i, 1],
        " If this is incorrect please change the random/fixed designation of the factor")
            finaleffectrandom[confound.tab[i, 2]] <- 1
        }

        #Checks fixed factors
        if (!(confound.tab[i, 1] %in% randomfacs) && (finaleffectrandom[confound.tab[i, 2]] == 1)) {
            cat("\n", confound.tab[i, 1], " was defined as fixed but it is equivalent to ", names(finaleffectrandom[confound.tab[i, 2]]),
        " which is considered random. \n", names(finaleffectrandom[confound.tab[i, 2]]),
        " will be assumed to be random .",
        " If this is incorrect please change the random/fixed designation of the term")
        }
    }
}
#STB: Need code equivalent to the above for SilvR


#This code checks whether the user has redefined the random/fixed terms and replaces the finaleffectrandom if they have. 
if (exists("userfinaleffectrandom")) {
    names(userfinaleffectrandom) <- names(finaleffectrandom)
    bothfinaleffectrandom <- cbind(finaleffectrandom, userfinaleffectrandom)

    #Generate SilveR printed version
    Default_designation <- c(rep(NA, length(finaleffectrandom)))
    User_defined_designation <- c(rep(NA, length(finaleffectrandom)))
    for (i in 1:length(finaleffectrandom)) {
        if (finaleffectrandom[i] == "1") { Default_designation[i] <- "Random" } else { Default_designation[i] <- "Fixed " }

        if (userfinaleffectrandom[i] == "1") { User_defined_designation[i] <- "Random" } else { User_defined_designation[i] <- "Fixed " }
    }
    names(Default_designation) <- names(finaleffectrandom)
    names(User_defined_designation) <- names(finaleffectrandom)
    bothfinaleffectrandom_SilveR <- cbind(Default_designation, User_defined_designation)

    if (any(userfinaleffectrandom != finaleffectrandom)) {
        cat("The random/fixed designation of a term has been changed by the user. The user designation will be used by the program but please chack it is appropriate.\n")
        if (SilveR == "N") { print(bothfinaleffectrandom) }
        if (SilveR == "Y") { print(bothfinaleffectrandom_SilveR) }
        finaleffectrandom <- userfinaleffectrandom
    }
}








#Producing Nested names
main.effects.table.order <- main.effects.table.print[order(prelim.effect.order), order(prelim.effect.order)]
#Identifies which main effects nest other main effects
main.effects.mat <- matrix(NA, nrow = nrow(main.effects.table.order), ncol = ncol(main.effects.table.order), dimnames = dimnames(main.effects.table.order))
for (i in 1:nrow(main.effects.table.order)) {
    for (j in 1:ncol(main.effects.table.order)) {
        if (main.effects.table.order[i, j] == "1") main.effects.mat[i, j] <- 1
        if (main.effects.table.order[i, j] == "0") main.effects.mat[i, j] <- 0
        if (main.effects.table.order[i, j] == "(0)") main.effects.mat[i, j] <- 0
        if (main.effects.table.order[i, j] == " ") main.effects.mat[i, j] <- 0
    }
}
#Identifies which main effects directly nest other main effects
effects.equiv.interaction <- confound.tab[(confound.tab[, 4] == "FALSE"), 1] #identfy which effects are equivalent to interactions
#Then remove these from main.effects.mat as nested names are not required for them
if (nrow(confound.tab) > 0) {
    if ((length(effects.equiv.interaction) > 0) && (any(rownames(main.effects.mat) %in% effects.equiv.interaction))) {
        main.effects.mata <- main.effects.mat[-which(rownames(main.effects.mat) %in% effects.equiv.interaction), - which(rownames(main.effects.mat) %in% effects.equiv.interaction)]
    } else { main.effects.mata <- main.effects.mat }
    for (i in 1:nrow(main.effects.mata)) {
        for (j in 1:ncol(main.effects.mata)) {
            if (main.effects.mata[i, j] == 1) {
                for (k in 1:nrow(main.effects.mata)) {
                    main.effects.mata[i, k] <- max(main.effects.mata[i, k] - main.effects.mata[j, k], 0)
                }
            }
        }
    }
}
#Put names of confounded main effects into brief colnames e.g. Plot.Wine=Co*Ro*Sq*Tr
if (dim(confound.tab)[1] > 0) {
    for (i in 1:dim(confound.tab)[1]) {
        for (j in 1:length(colnames(ceffects.table.final.brief))) {
            if (names(colnames(ceffects.table.final.brief))[j] == confound.tab[i, 5]) {
                colnames(ceffects.table.final.brief)[j] <- paste(confound.tab[i, 3], "=", brief.colnames[confound.tab[i, 5]], sep = "")
            }
        }
    }
}


#Produces nested names for all main effects
main.effects.mat.brief <- substr(rownames(main.effects.mata), 1, c(4, abbrev.length[rownames(main.effects.mata)[-1]]))
main.effects.table.nestnames <- rownames(main.effects.mata)
main.effects.mat.nestbrief <- main.effects.mat.brief
#STB
if (SilveR == "N") { main.effects.table.nestnames }
if (SilveR == "N") { main.effects.mat.nestbrief }

for (i in 2:length(rownames(main.effects.mata))) {
    if (no.confounded > 0) {
        #Check to see if the effect is a confounded effect and that the interaction contains the main effect
        if (rownames(main.effects.mata)[i] %in% confound.tab[, 1] && (confound.tab[confound.tab[, 1] == rownames(main.effects.mata)[i], 4] == T)) {
            first <- 1 #Indicates whether it is the first part of the nested effect - to produce (
            for (j in 2:length(colnames(main.effects.mata))) {
                if (main.effects.mata[i, j] == 1 & i != j) {
                    if (first == 1) {
                        main.effects.table.nestnames[i] <- paste(main.effects.table.nestnames[i], "(", main.effects.table.nestnames[colnames(main.effects.mata) == colnames(main.effects.mata)[j]], sep = "")
                        main.effects.mat.nestbrief[i] <- paste(main.effects.mat.nestbrief[i], "(", main.effects.mat.nestbrief[main.effects.mat.brief == main.effects.mat.brief[j]], sep = "")
                    } else {
                        #zzz^change
                        main.effects.table.nestnames[i] <- paste(main.effects.table.nestnames[i], "^", main.effects.table.nestnames[rownames(main.effects.mata) == rownames(main.effects.mata)[j]], sep = "")
                        #zzz^change
                        main.effects.mat.nestbrief[i] <- paste(main.effects.mat.nestbrief[i], "^", main.effects.mat.nestbrief[-1][main.effects.mat.brief[-1] == main.effects.mat.brief[j]], sep = "")
                    }
                    first <- 0
                }
            }
            if (first == 0) {
                main.effects.table.nestnames[i] <- paste(main.effects.table.nestnames[i], ")", sep = "")
                main.effects.mat.nestbrief[i] <- paste(main.effects.mat.nestbrief[i], ")", sep = "")
            }
        }
    }
}

#Produces nested names for only main effects (and brief main effects) which need substituting
main.effects.table.nestnames <- cbind(rownames(main.effects.mata), main.effects.table.nestnames, main.effects.mat.brief, main.effects.mat.nestbrief)


if (SilveR == "N") { main.effects.table.nestnames }
nested.names <- main.effects.table.nestnames[main.effects.table.nestnames[, 1] %in% confound.tab[confound.tab[, 4] == T, 1],, drop = F]
nested.names <- cbind(nested.names, confound.tab[confound.tab[, 4] == T, 2])
if (SilveR == "N") { nested.names }
#Matrix of individual effects constituting effects in model
namesind.effects <- matrix(NA, nrow = length(names(finaleffects)), ncol = ncol(main.effects.mata))
for (i in 1:length(names(finaleffects))) {
    extract <- extract.factor.fun(names(finaleffects)[i])
    for (j in 1:length(extract)) {
        namesind.effects[i, j] <- extract[j]
    }
}
if (SilveR == "N") { namesind.effects }
#Matrix of brief individual effects constituting effects (without "namesind.briefeffects" and with confounded names "cnamesind.briefeffects") in model
namesind.briefeffects <- matrix(NA, nrow = length(colnames(effects.table.final.brief)), ncol = ncol(main.effects.mata))
for (i in 1:length(colnames(effects.table.final.brief))) {
    extract <- extract.factor.fun(colnames(effects.table.final.brief)[i])
    for (j in 1:length(extract)) {
        namesind.briefeffects[i, j] <- extract[j]
    }
}

cnamesind.briefeffects <- matrix(NA, nrow = length(colnames(ceffects.table.final.brief)), ncol = ncol(main.effects.mat))
for (i in 1:length(colnames(ceffects.table.final.brief))) {
    extract <- extract.factor.fun(colnames(ceffects.table.final.brief)[i])
    for (j in 1:length(extract)) {
        cnamesind.briefeffects[i, j] <- extract[j]
    }
}
if (SilveR == "N") { cnamesind.briefeffects }

#Confounded effects where main effect included in confounded interaction split into individual names
nestednamesind <- matrix(NA, nrow = nrow(nested.names), ncol = ncol(main.effects.mata))
if (nrow(nested.names) > 0) {
    for (i in 1:nrow(nested.names)) {
        extract <- extract.factor.fun(nested.names[i, 5])
        for (j in 1:length(extract)) {
            nestednamesind[i, j] <- extract[j]
        }
    }
}
if (SilveR == "N") { nestednamesind }
nestednamesind.brief <- substr(nestednamesind, 1, abbrev.length[nestednamesind])

#For individual terms in each effect - remove those nested within another
for (m in 1:nrow(namesind.effects)) {
    #     ALL TERMS SEPARATED INTO A TABLE  OF INDIVIUAL FACTRS AS COMPONENTS
    if (nrow(nested.names) > 0) {
        for (k in nrow(nested.names):1) {
            #          NESTED MAIN EFFECTS
            if (nested.names[k, 1] %in% namesind.effects[m, ]) {
                if (ncol(namesind.effects) > 0) {
                    for (r in 1:ncol(namesind.effects)) {
                        #      ACROSS COLUMNS OF IND FACTORS WITHIN TERMS
                        if (ncol(nestednamesind) > 0) {
                            for (p in 1:ncol(nestednamesind)) {
                                #      ACROSS COLUMNS OF IND FACTORS WITHIN NESTED NAMES
                                if (!is.na(namesind.effects[m, r]) && !is.na(nestednamesind[k, p]) && nested.names[k, 1] != namesind.effects[m, r]
                      && namesind.effects[m, r] == nestednamesind[k, p]) { namesind.effects[m, r] <- NA }
                            }
                        }
                    }
                }
            }
        }
    }
}

for (m in 1:nrow(namesind.briefeffects)) {
    if (nrow(nested.names) > 0) {
        for (k in nrow(nested.names):1) {
            if (nested.names[k, 3] %in% namesind.briefeffects[m, ]) {
                if (ncol(namesind.briefeffects) > 0) {
                    for (r in 1:ncol(namesind.briefeffects)) {
                        if (ncol(nestednamesind.brief) > 0) {
                            for (p in 1:ncol(nestednamesind.brief)) {
                                if (!is.na(namesind.briefeffects[m, r]) && !is.na(nestednamesind.brief[k, p]) && nested.names[k, 3] != namesind.briefeffects[m, r] && namesind.briefeffects[m, r] == nestednamesind.brief[k, p]) {
                                    cnamesind.briefeffects[m, r] <- gsub(nestednamesind.brief[k, p], "", cnamesind.briefeffects[m, r]) #Replaces nested main effect by "" - note this may be contained within a term
                                    if (!is.na(cnamesind.briefeffects[m, r]) && cnamesind.briefeffects[m, r] == "") { cnamesind.briefeffects[m, r] <- NA }
                                    #Replaces cells in matrix which are "" by NA
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}

#Where a nested confounded effect occurs - swap nested name for single name e.g. Interval(Sq) instead of Interval    
finalnamesind.effects <- namesind.effects
finalnamesind.briefeffects <- cnamesind.briefeffects
finalnames.effects <- rep(NA, nrow(namesind.effects))
finalnames.briefeffects <- rep(NA, nrow(namesind.effects))
for (m in 1:nrow(namesind.effects)) {
    for (r in 1:ncol(namesind.effects)) {
        if (nrow(nested.names) > 0) {
            for (k in 1:nrow(nested.names)) {
                if (nested.names[k, 1] %in% namesind.effects[m, r]) {
                    finalnamesind.effects[m, r] <- gsub(nested.names[k, 1], nested.names[k, 2], namesind.effects[m, r])
                    #For confounded effects ConfoundedName=abbreviated effect it is only the abbreviated effect which needs swapping
                    #So code checks for an = sign in name. If no "=" then all nested names are replaced. If an "=" is present then it replaces the last confounded term  
                    if (length(strsplit(cnamesind.briefeffects[m, r], "=")[[1]]) <= 1) {
                        finalnamesind.briefeffects[m, r] <- sub(nested.names[k, 3], nested.names[k, 4], cnamesind.briefeffects[m, r])
                    } else {
                        finalnamesind.briefeffects[m, r] <- paste(substr(cnamesind.briefeffects[m, r], 1, gregexpr("=", cnamesind.briefeffects[m, r])[[1]][length(strsplit(cnamesind.briefeffects[m, r], "=")[[1]]) - 1]),
                   nested.names[k, 4], sep = "")
                    }
                }
            }
        }
    }
    #zzz^change
    finalnames.effects[m] <- paste(finalnamesind.effects[m, !is.na(finalnamesind.effects[m,])], collapse = "^")
    #zzz^change
    finalnames.briefeffects[m] <- paste(finalnamesind.briefeffects[m, !is.na(finalnamesind.briefeffects[m,])], collapse = "^")
    #zzz^change
    finalnames.briefeffects[m] <- gsub("=\\^", "=", finalnames.briefeffects[m])
}



print("Error between these points")


#save(list=c("outputlistip1","mydata","mydesign","confounded.main", "finalnames.effects","finaleffects","ceffects.table.final.brief",
#  "nested.names","finalnames.briefeffects", "randomfacsid","brief.finaldesign.effects" ,"finaldesign.effects" ,
#  "randomised" ,"showRDS","showpartialRDS","showdfRDS","showFDS","showpartialFDS","showdfFDS" ,"showrandFDS","finaleffectrandom",
#   "confound.tab","maxfacs","larger.fontlabelmultiplier","smaller.fontlabelmultiplier") ,
#  file=paste(data.folder.location,example,".Rdata",sep=""))

#load(file=paste(data.folder.location,example,".Rdata",sep=""))
#########################################################################################################
######################################################################################################### 
#########################################################################################################
#Outputting the Evaluation of the Design and Hasse diagram
if (showRDS == "Y") {
    if (SilveR == "Y") {
        HTMLbr()
        HTMLbr()
        HTML.title("<bf>The Layout Structure", HR = 1, align = "left")
    }

    names(finaleffects) <- finalnames.effects
    if (SilveR == "N") { cat("\nThe factors / generalised factors in the Layout Structure together with their level in the structure are:\n") }

    if (SilveR == "N") { print(finaleffects) }
    if (SilveR == "N") { cat("\nThe following table shows the relationships between the factors and generalised factors in the Layout Structure\n") }

    if (SilveR == "N") { print(ceffects.table.final.brief) }
    #Check that nested effects are random
    typenested <- cbind(finaleffectrandom[names(finaleffects) %in% nested.names[, 2]], nested.names[, 2])
    fixednested <- typenested[typenested[, 1] == 0, 2]
    if (length(fixednested) > 0) {
        if (SilveR == "N") { cat("\nThe following nested factors have been designated as fixed. If correct consider whether it is possible to cross the effects in the design\n", fixednested) }
    }


    #Any confounded effeames for only main effects (and  and nested by a final effect must be confounded with it
    confound.print.fun(confound.tab)
    if (SilveR == "N") { cat("\n\nThe following graph shows the Hasse diagram of the Layout Structure indicating the relationships between the factors") }

    #Creating a graph of the simple Hasse diagram
    #Need a matrix indicating which effects are to be linked by lines (indicated by 1s in the matrix)
    #First treat partially crossed and fully crossed as the same
    adjm <- matrix(NA, nrow = nrow(ceffects.table.final.brief), ncol = ncol(ceffects.table.final.brief), dimnames = dimnames(ceffects.table.final.brief))

    for (i in 1:nrow(ceffects.table.final.brief)) {
        for (j in 1:ncol(ceffects.table.final.brief)) {
            if (ceffects.table.final.brief[i, j] == "1") adjm[i, j] <- 1
            if (ceffects.table.final.brief[i, j] == "0") adjm[i, j] <- 0
            if (ceffects.table.final.brief[i, j] == "(0)") adjm[i, j] <- 0
            if (ceffects.table.final.brief[i, j] == " ") adjm[i, j] <- 0
        }
    }
    #Then remove lines which link to nested effects lower in the design e.g. A links to A*B and A*B links to A*B*C but A does not link to A*B*C in diagram.
    adjm.adjust <- adjm
    for (j in 1:ncol(adjm)) {
        for (i in 1:nrow(adjm)) {
            if (adjm.adjust[i, j] == 1) {
                for (k in 1:nrow(adjm)) {
                    adjm.adjust[k, j] <- max(adjm.adjust[k, j] - adjm[k, i], 0)
                }
            }
        }
    }

    #This adds dummy variables so that whole width 0f plotting space is used 
    adjm.adjust <- rbind(aaadum1 = adjm.adjust[1,], Mean = adjm.adjust[1,], zzzdum2 = adjm.adjust[1,], adjm.adjust[-1,])
    adjm.adjust <- cbind(aaadum1 = adjm.adjust[, 1], Mean = adjm.adjust[, 1], zzzdum2 = adjm.adjust[, 1], adjm.adjust[, -1])

    # For plot to work need to reverse the order of effects                      
    adjm.reverse <- adjm.adjust[nrow(adjm.adjust):1, ncol(adjm.adjust):1]
    if (SilveR == "N") { adjm.reverse }

    g1 <- graph.adjacency(adjm.reverse, mode = "undirected")

    g <- simplify(g1)
    V(g)$label <- V(g)$name

    #This section calculates the coordinates for the vertices of the Hasse diagram  
    dscoords <- dscoords.fun("RDS")
    g$layout <- dscoords$coords

    #########################################################################################################

    vertex.label.font <- rep(2, length(colnames(adjm.reverse)))
    vertex.label.color.blue <- c(rep(Colourblue, length(colnames(adjm.reverse)) - 3), "transparent", Colourblue, "transparent")
    vertex.label.color.black <- c(rep("black", length(colnames(adjm.reverse)) - 3), "transparent", "black", "transparent")
    vertex.label.color.red <- c(rep(Colourred, length(colnames(adjm.reverse)) - 3), "transparent", Colourred, "transparent")

    #Set up plot for underlining random effects
    # Default assumes that interaction of two fixed effects is fixed - should allow user to modify
    #Put list identifying random effects in reverse order
    finaleffectrandom.reverse <- c(finaleffectrandom[length(finaleffectrandom):1], 0, 0)

    adjm.reverse.blank <- adjm.reverse
    #Replace characters by underscores to produce an underline
    for (m in 1:length(colnames(adjm.reverse.blank))) {
        if (finaleffectrandom.reverse[m] == 1) {
            colnames(adjm.reverse.blank)[m] <- paste("", paste(rep("_", nchar(colnames(adjm.reverse.blank)[m])), collapse = ""))
        } else {
            colnames(adjm.reverse.blank)[m] <- ""
        }
    }
    g2 <- graph.adjacency(adjm.reverse.blank, mode = "undirected")

    g2a <- simplify(g2)
    V(g2a)$label <- V(g2a)$name
    g2a$layout <- dscoords$coords
    vcount(g2a) #nodes  This used to be g2a[[1]] as per print.default(g2a)
    g2a.edges <- get.edges(g2a, 1:ecount(g2a))[, 1] - 1 #lines joining  4,5,6  This used to be g2a[[3]] as per print.default(g2a)
    node.dumg <- c(vcount(g2a) - 3, vcount(g2a) - 1) #This is the numbers for the dummy nodes I think
    edge.color <- rep("grey", length(g2a.edges))
    edge.color[g2a.edges %in% node.dumg] <- "transparent"
    par(mar = c((2 * (max(larger.fontlabelmultiplier, smaller.fontlabelmultiplier) - 1) + 1) * 0.8, (5 * (max(larger.fontlabelmultiplier, smaller.fontlabelmultiplier) - 1) + 1) * 0.4, 0.2, (5 * (max(larger.fontlabelmultiplier, smaller.fontlabelmultiplier) - 1) + 1) * 0.4))


    if (SilveR == "Y" && pdf == "N" && showRDS == "Y") {
        RDSPlot <- sub(".html", "RDSPlot.jpg", htmlFile)
        jpeg(RDSPlot)
    }

    #Places white edged transparent vertex circles, underlining for random effects and draws lines
    plot(g2a, asp = FALSE, add = F, vertex.label.color = vertex.label.color.black, vertex.label.cex = dscoords$textlabel.size, vertex.label.font = vertex.label.font, vertex.label.degree = pi / 2, vertex.label.dist = (0.7 * (max(larger.fontlabelmultiplier, smaller.fontlabelmultiplier) - 1) + 1) * 0.11, vertex.size = 5, vertex.color = "transparent", vertex.shape = "circle", vertex.frame.color = "white", edge.color = edge.color)
    #Adds names of effects
    plot(g, asp = FALSE, add = T, vertex.label.color = vertex.label.color.blue, vertex.label.cex = dscoords$textlabel.size, vertex.label.font = vertex.label.font, vertex.size = 0, vertex.color = "transparent", vertex.frame.color = "transparent",
 vertex.shape = "circle", edge.lty = 0)


}


#-----------------------------------------------------------------------------------------------------------      

#set up matrix for degrees of freedom, 1st col = Tier, 2nd col=max number of levels, 3rd col = actual number of levels, 4th col = dfs
if (showdfRDS == "Y") {
    RDS.output <- dfs.fun("RDS")
    if (SilveR == "N") { cat("\ndfs =  \n") }

    g4 <- g
    V(g4)$label <- paste(sep = "", "[", RDS.output$xdfs.reverse[, 3], RDS.output$maxlevelsf.reverse[], ",", RDS.output$xdfs.reverse[, 4], "]")
    #Add degrees of freedom
    plot(g4, asp = FALSE, add = T, vertex.label.color = vertex.label.color.red, vertex.label.cex = dscoords$textlabel.size.df,
  vertex.label.font = vertex.label.font, vertex.label.degree = pi / 2, vertex.label.dist = (0.7 * (max(larger.fontlabelmultiplier, smaller.fontlabelmultiplier) - 1) + 1) * 0.25, vertex.size = 0, vertex.color = "transparent",
  vertex.frame.color = "transparent", vertex.shape = "circle", edge.lty = 0)
    # RDS.output$xdfs   #Not sure why but doesn't print out if line put after cat statement

    if (SilveR == "N") { RDS.output$xdfs }

}


#-----------------------------------------------------------------------------------------------------------    
#Identify effects which are partially crossed
#I have not checked whether the derivation of partially crossed effects is correct 
#Another way which may be better is by using E(g)[DEFINE HERE WHICH ONES]$lty <- 2
#Note for can choose type of dotted/dashed line (2-6) if wish

if (showpartialRDS == "Y") {
    adjm3 <- matrix(0, nrow = nrow(adjm), ncol = ncol(adjm), dimnames = dimnames(adjm))
    for (i in 1:nrow(ceffects.table.final.brief)) {
        for (j in 1:ncol(ceffects.table.final.brief)) {
            if (ceffects.table.final.brief[i, j] == "(0)" && ceffects.table.final.brief[j, i] == "(0)") adjm3[i, j] <- 1
        }
    }
    #Then remove lines which link to nested effects lower in the design e.g. A links to A*B and A*B links to A*B*C but A does not link to A*B*C in diagram.
    adjm3.adjust <- adjm3
    for (j in 1:ncol(adjm3)) {
        for (i in 1:nrow(adjm3)) {
            if (adjm3.adjust[i, j] == 1) {
                for (k in 1:nrow(adjm3)) {
                    adjm3.adjust[k, j] <- max(adjm3.adjust[k, j] - adjm3[k, i], 0)
                }
            }
        }
    }

    #This adds dummy variables so that whole width of plotting space is used 
    adjm3.adjust <- rbind(aaadum1 = adjm3.adjust[1,], Mean = adjm3.adjust[1,], zzzdum2 = adjm3.adjust[1,], adjm3.adjust[-1,])
    adjm3.adjust <- cbind(aaadum1 = adjm3.adjust[, 1], Mean = adjm3.adjust[, 1], zzzdum2 = adjm3.adjust[, 1], adjm3.adjust[, -1])

    # For plot to work need to reverse the order of effects
    adjm3.reverse <- adjm3.adjust[nrow(adjm3.adjust):1, ncol(adjm3.adjust):1]
    if (SilveR == "N") { adjm3.reverse }


    g3 <- graph.adjacency(adjm3.reverse, mode = "undirected") #Change this to "directed" for randomization arrows                     
    g3 <- simplify(g3)
    V(g3)$label <- V(g3)$name
    # if (example=="Ex_Fact14") E(g3)$label <- c("[df=1]")
    g3$layout <- dscoords$coords

    plot(g3, asp = FALSE, add = TRUE, vertex.label.color = "transparent", vertex.label.cex = dscoords$textlabel.size,
  vertex.label.font = 2, vertex.size = 0, vertex.color = "transparent", vertex.frame.color = "transparent",
  edge.label.color = Colourred, edge.label.font = 2, edge.color = Colourorange, edge.lty = 3)
}

if (SilveR == "Y" && pdf == "N" && showRDS == "Y") {
    HTMLbr()
    HTML.title("<bf>Hasse diagram of the Layout Structure", HR = 2, align = "left")
    void <- HTMLInsertGraph(GraphFileName = sub("[A-Z0-9a-z,:,\\\\]*App_Data[\\\\]", "", RDSPlot), Align = "left")
}

if (SilveR == "Y" && pdf == "Y" && showRDS == "Y") {
    pdfFile2 <- sub("[A-Z0-9a-z,:,\\\\]*App_Data[\\\\]", "", pdfFile)
    linkToPdf <- paste("<a href=\"http://silverstats.co.uk/Rreports/", pdfFile2, "\">Click here to view the PDF of the Hasse diagram of the Layout Structure</a>", sep = "")
    HTML(linkToPdf)
}
if (pdf == "Y" && showFDS == "N") dev.off()

#-----------------------------------------------------------------------------------------------------------      
#########################################################################################################
#########################################################################################################
#########################################################################################################
if (showFDS == "Y") {

    if (SilveR == "Y") {
        HTMLbr()
        HTMLbr()
        HTML.title("<bf>The Restricted Layout Structure", HR = 1, align = "left")
    }
    if (SilveR == "Y" && pdf == "N") {
        FDSPlot <- sub(".html", "FDSPlot.jpg", htmlFile)
        jpeg(FDSPlot)
    }

    if (SilveR == "Y" && pdf == "Y") {
        pdfFileF <- sub(".csv", "x.pdf", Args[3]);
        #determine the file name of the pdf file
        FDSPlot <- sub(".html", "FDSPlot.pdf", htmlFile)
        pdf(file = pdfFileF, width = 11, height = 8)
    }


    #Summarising and Plotting the Final Design structure with degrees of freedom
    #This reduces the effects to those effects selected in the final design through the randomization
    #finaldesign.effects and brief.finaldesign.effects are specified by the user                           

    #Firstly the names of effects selected by the user are checked against those in the layout structure
    if (SilveR == "N") {
        finalnames.wrong <- "N"
        if (sum(finaldesign.effects %in% rownames(ceffects.table.final.brief)) != length(finaldesign.effects)) {
            cat("\n The selected final terms  ", finaldesign.effects[!(finaldesign.effects %in% rownames(ceffects.table.final.brief))], "  is not in the terms in the layout structure  \n", rownames(ceffects.table.final.brief))
            finalnames.wrong <- "Y"
        }
        if (sum(brief.finaldesign.effects %in% colnames(ceffects.table.final.brief)) != length(brief.finaldesign.effects)) {
            cat("\n The selected final abbreviated terms   ", brief.finaldesign.effects[!(brief.finaldesign.effects %in% colnames(ceffects.table.final.brief))], "  is not in the abbreviated terms in the layout structure  \n", colnames(ceffects.table.final.brief))
            finalnames.wrong <- "Y"
        }
        if ((exists("randomised")) && (sum(randomised %in% rownames(ceffects.table.final.brief)) != length(randomised))) {
            cat("\n The selected terms indicating the randomisation  ", randomised[!(randomised %in% rownames(ceffects.table.final.brief))], "  is not in the terms in the layout structure  \n", rownames(ceffects.table.final.brief))
            finalnames.wrong <- "Y"
        }
        if (finalnames.wrong == "Y") {
            if (pdf == "Y") dev.off()
            stop()
        }
    }

    finalceffects.table.final.brief <- ceffects.table.final.brief[finaldesign.effects, brief.finaldesign.effects]

    #select final effects in final design structure
    finalfinaleffects <- finaleffects[finalnames.effects %in% finaldesign.effects]

    #Outputting the Evaluation of the Design
    if (SilveR == "N") { cat("\nThe randomisation objects in the Restricted Layout Structure together with their level in the design are:\n") }
    names(finalfinaleffects) <- finalnames.effects[finalnames.effects %in% finaldesign.effects]
    if (SilveR == "N") { print(finalfinaleffects) }

    if (SilveR == "N") { cat("\nThe following table shows the relationships between the randomisation objects in the Restricted Layout Structure\n") }
    if (SilveR == "N") { print(finalceffects.table.final.brief) }

    #Check that nested effects are random
    names(finaleffectrandom) <- finalnames.effects
    typenested <- cbind(finaleffectrandom[names(finalfinaleffects)][names(finalfinaleffects)[names(finalfinaleffects) %in% nested.names[, 2]]], nested.names[, 2])
    fixednested <- typenested[typenested[, 1] == 0, 2]
    if (length(fixednested) > 0) {
        if (SilveR == "N") { cat("\nThe following nested factors have been designated as fixed. If correct consider whether it is possible to cross the effects in the design\n", fixednested) }
    }

    #Any confounded effects which are both nested within and nested by a final effect must be confounded with it

    confound.print.fun(confound.tab)


    #zzz - NEEDS ADAPTING FOR MIXED MODEL CURRENTLY ANOVA  
    #Constructs the righthand side of equation terms with as.factor in front and replacing nested and * parts of effects with :
    model.equation.final <- model.equation.fun(model.effects.fun(rownames(finalceffects.table.final.brief)))
    if (SilveR == "N") {
        cat("The suggested mixed model to be fitted is: \n", substring(model.equation.final, 6))
    }


    #Need a matrix indicating which effects are to be linked by lines (indicated by 1s in the matrix)
    #First treat partially crossed and fully crossed as the same
    fadjm <- matrix(NA, nrow = nrow(finalceffects.table.final.brief), ncol = ncol(finalceffects.table.final.brief), dimnames = dimnames(finalceffects.table.final.brief))

    #Allows user renaming of effects e.g. effect named as nested or randomization arrows introduced
    #  if (exists("FDSrename.brief") && exists("FDSrename.full")) {
    #      rownames(fadjm) <- FDSrename.full
    #      colnames(fadjm) <- FDSrename.brief
    #      } 

    if (exists("FDSrename.brief")) {
        colnames(fadjm) <- FDSrename.brief
    }

    for (i in 1:nrow(finalceffects.table.final.brief)) {
        for (j in 1:ncol(finalceffects.table.final.brief)) {
            if (finalceffects.table.final.brief[i, j] == "1") fadjm[i, j] <- 1
            if (finalceffects.table.final.brief[i, j] == "0") fadjm[i, j] <- 0
            if (finalceffects.table.final.brief[i, j] == "(0)") fadjm[i, j] <- 0
            if (finalceffects.table.final.brief[i, j] == " ") fadjm[i, j] <- 0
        }
    }
    #Then remove lines which link to nested effects lower in the design e.g. A links to A*B and A*B links to A*B*C but A does not link to A*B*C in diagram.
    fadjm.adjust <- fadjm
    for (j in 1:ncol(fadjm)) {
        for (i in 1:nrow(fadjm)) {
            if (fadjm.adjust[i, j] == 1) {
                for (k in 1:nrow(fadjm)) {
                    fadjm.adjust[k, j] <- max(fadjm.adjust[k, j] - fadjm[k, i], 0)
                }
            }
        }
    }


    #This adds dummy variables so that whole width 0f plotting space is used
    fadjm.adjust <- rbind(aaadum1 = fadjm.adjust[1,], Mean = fadjm.adjust[1,], zzzdum2 = fadjm.adjust[1,], fadjm.adjust[-1,])
    fadjm.adjust <- cbind(aaadum1 = fadjm.adjust[, 1], Mean = fadjm.adjust[, 1], zzzdum2 = fadjm.adjust[, 1], fadjm.adjust[, -1])


    #This removes a line if it is to be drawn as a randomised arrow  
    if (showrandFDS == "Y") {
        randomisedmat <- matrix(0, nrow = nrow(fadjm.adjust), ncol = ncol(fadjm.adjust), dimnames = list(rownames(fadjm.adjust), rownames(fadjm.adjust)))
        if (exists("randomised")) {
            for (i in 1:nrow(randomised)) {
                randomisedmat[randomised[i, 2], randomised[i, 1]] <- 1
            }
        }


        colnames(randomisedmat) <- colnames(fadjm.adjust)
        #Allows user renaming of effects e.g. effect named as nested or randomization arrows introduced
        #  if (exists("FDSrename.brief") && exists("FDSrename.full")) {
        #      rownames(randomisedmat) <- FDSrename.full
        #      colnames(randomisedmat) <- FDSrename.brief
        #      } 

        if (exists("FDSrename.brief")) {
            colnames(randomisedmat)[c(-1, -3)] <- FDSrename.brief
        }

        if (SilveR == "N") { randomisedmat }
        fadjm.adjust <- fadjm.adjust - randomisedmat
    }

    # For plot to work need to reverse the order of effects                      
    fadjm.reverse <- fadjm.adjust[nrow(fadjm.adjust):1, ncol(fadjm.adjust):1]
    if (SilveR == "N") { fadjm.reverse }

    fg1 <- graph.adjacency(fadjm.reverse, mode = "undirected")

    fg <- simplify(fg1)
    V(fg)$label <- V(fg)$name

    #This section calculates the coordinates for the vertices of the Hasse diagram  
    dscoords <- dscoords.fun("FDS")
    fg$layout <- dscoords$coords


    if (SilveR == "N") { cat("\n\nThe following graph shows a Hasse diagram of the Restricted Layout Structure indicating the relationships between the randomisationobjects in the Restricted Layout Structure") }

    vertex.label.font <- rep(2, length(colnames(fadjm.reverse)))
    vertex.label.color.blue <- c(rep(Colourblue, length(colnames(fadjm.reverse)) - 3), "transparent", Colourblue, "transparent")
    vertex.label.color.black <- c(rep("black", length(colnames(fadjm.reverse)) - 3), "transparent", "black", "transparent")
    vertex.label.color.red <- c(rep(Colourred, length(colnames(fadjm.reverse)) - 3), "transparent", Colourred, "transparent")

    #Set up plot for underlining random effects
    # Default assumes that interaction of two fixed effects is fixed - should allow user to modify
    #Put list identifying random effects in reverse order
    finalfinaleffectrandom <- finaleffectrandom[finalnames.effects %in% finaldesign.effects]
    finalfinaleffectrandom.reverse <- c(finalfinaleffectrandom[length(finalfinaleffectrandom):1], 0, 0)

    fadjm.reverse.blank <- fadjm.reverse
    #Replace characters by underscores to produce underlines
    for (m in 1:length(colnames(fadjm.reverse.blank))) {
        if (finalfinaleffectrandom.reverse[m] == 1) {
            colnames(fadjm.reverse.blank)[m] <- paste("", paste(rep("_", nchar(colnames(fadjm.reverse.blank)[m])), collapse = ""))
        } else {
            colnames(fadjm.reverse.blank)[m] <- ""
        }
    }
    fg2 <- graph.adjacency(fadjm.reverse.blank, mode = "undirected")

    fg2a <- simplify(fg2)
    V(fg2a)$label <- V(fg2a)$name
    fg2a$layout <- dscoords$coords
    vcount(fg2a) #nodes  This used to be fg2a[[1]] as per print.default(fg2a)
    fg2a.edges <- get.edges(fg2a, 1:ecount(fg2a))[, 1] - 1 #lines joining  4,5,6  This used to be fg2a[[3]] as per print.default(fg2a)
    node.dumg <- c(vcount(fg2a) - 3, vcount(fg2a) - 1) #This is the numbers for the dummy nodes I think
    edge.color <- rep("grey", length(fg2a.edges))
    edge.color[fg2a.edges %in% node.dumg] <- "transparent"
    par(mar = c((2 * (max(larger.fontlabelmultiplier, smaller.fontlabelmultiplier) - 1) + 1) * 1.2, (5 * (max(larger.fontlabelmultiplier, smaller.fontlabelmultiplier) - 1) + 1) * 0.4, 0.2, (5 * (max(larger.fontlabelmultiplier, smaller.fontlabelmultiplier) - 1) + 1) * 0.4))



    #Places white edged transparent vertex circles, underlining for random effects and draws lines
    plot(fg2a, asp = FALSE, add = F, vertex.label.color = vertex.label.color.black, vertex.label.cex = dscoords$textlabel.size,
         vertex.label.font = vertex.label.font, vertex.label.degree = pi / 2, vertex.label.dist = (0.7 * (max(larger.fontlabelmultiplier, smaller.fontlabelmultiplier) - 1) + 1) * 0.11, vertex.size = 5, vertex.color = "transparent", vertex.shape = "circle",
         vertex.frame.color = "white", edge.color = edge.color)
    #Adds names of effects
    plot(fg, asp = FALSE, add = T, vertex.label.color = vertex.label.color.blue, vertex.label.cex = dscoords$textlabel.size,
         vertex.label.font = vertex.label.font, vertex.size = 0, vertex.color = "transparent", vertex.frame.color = "transparent",
         vertex.shape = "circle", edge.lty = 0)
}

#-----------------------------------------------------------------------------------------------------------      

#Sets up matrix for degrees of freedom, 1st col = Tier, 2nd col=max number of levels, 3rd col = actual number of levels, 4th col = dfs
#Adds degrees of freedom to plot

if (showdfFDS == "Y") {
    FDS.output <- dfs.fun("FDS")
    if (SilveR == "N") { FDS.output$xdfs }
    fg4fix <- fg
    fg4rand <- fg
    dflab4fix <- NULL
    dflab4rand <- NULL
    for (i in 1:length(V(fg2a)$name)) {
        if (V(fg2a)$name[i] == "") {
            dflab4fix[i] <- paste(sep = "", "[", FDS.output$xdfs.reverse[i, 3], FDS.output$maxlevelsf.reverse[i], ",", FDS.output$xdfs.reverse[i, 4], "]")
            dflab4rand[i] <- ""
        }
        else {
            dflab4rand[i] <- paste(sep = "", "[", FDS.output$xdfs.reverse[i, 3], FDS.output$maxlevelsf.reverse[i], ",", FDS.output$xdfs.reverse[i, 4], "]")
            dflab4fix[i] <- ""
        }
    }

    V(fg4fix)$label <- dflab4fix
    V(fg4rand)$label <- dflab4rand

    vertex.label.dist.df4fix <- (0.7 * (max(larger.fontlabelmultiplier, smaller.fontlabelmultiplier) - 1) + 1) * 0.25
    vertex.label.dist.df4rand <- (0.7 * (max(larger.fontlabelmultiplier, smaller.fontlabelmultiplier) - 1) + 1) * 0.32

    #Add degrees of freedom
    plot(fg4fix, asp = FALSE, add = T, vertex.label.color = vertex.label.color.red, vertex.label.cex = dscoords$textlabel.size.df,
       vertex.label.font = vertex.label.font, vertex.label.degree = pi / 2, vertex.label.dist = vertex.label.dist.df4fix, vertex.size = 0, vertex.color = "transparent",
       vertex.frame.color = "transparent", vertex.shape = "circle", edge.lty = 0)
    plot(fg4rand, asp = FALSE, add = T, vertex.label.color = vertex.label.color.red, vertex.label.cex = dscoords$textlabel.size.df,
       vertex.label.font = vertex.label.font, vertex.label.degree = pi / 2, vertex.label.dist = vertex.label.dist.df4rand, vertex.size = 0, vertex.color = "transparent",
       vertex.frame.color = "transparent", vertex.shape = "circle", edge.lty = 0)
}


#-----------------------------------------------------------------------------------------------------------      
#-----------------------------------------------------------------------------------------------------------    
#Identify effects which are partially crossed
#I have not checked whether the derivation of partially crossed effects is correct 
#Another way which may be better is by using E(g)[DEFINE HERE WHICH ONES]$lty <- 2
#Note for can choose type of dotted/dashed line (2-6) if wish
if (showpartialFDS == "Y") {
    fadjm3 <- matrix(0, nrow = nrow(fadjm), ncol = ncol(fadjm), dimnames = dimnames(fadjm))
    for (i in 1:nrow(finalceffects.table.final.brief)) {
        for (j in 1:ncol(finalceffects.table.final.brief)) {
            if (finalceffects.table.final.brief[i, j] == "(0)" && finalceffects.table.final.brief[j, i] == "(0)") fadjm3[i, j] <- 1
        }
    }
    #Then remove lines which link to nested effects lower in the design e.g. A links to A*B and A*B links to A*B*C but A does not link to A*B*C in diagram.
    fadjm3.adjust <- fadjm3
    for (j in 1:ncol(fadjm3)) {
        for (i in 1:nrow(fadjm3)) {
            if (fadjm3.adjust[i, j] == 1) {
                for (k in 1:nrow(fadjm3)) {
                    fadjm3.adjust[k, j] <- max(fadjm3.adjust[k, j] - fadjm3[k, i], 0)
                }
            }
        }
    }

    #This adds dummy variables so that whole width of plotting space is used 
    fadjm3.adjust <- rbind(aaadum1 = fadjm3.adjust[1,], Mean = fadjm3.adjust[1,], zzzdum2 = fadjm3.adjust[1,], fadjm3.adjust[-1,])
    fadjm3.adjust <- cbind(aaadum1 = fadjm3.adjust[, 1], Mean = fadjm3.adjust[, 1], zzzdum2 = fadjm3.adjust[, 1], fadjm3.adjust[, -1])

    # For plot to work need to reverse the order of effects
    fadjm3.reverse <- fadjm3.adjust[nrow(fadjm3.adjust):1, ncol(fadjm3.adjust):1]
    if (SilveR == "N") { fadjm3.reverse }

    fg3 <- graph.adjacency(fadjm3.reverse, mode = "undirected") #Change this to "directed" for randomization arrows                     
    fg3 <- simplify(fg3)
    V(fg3)$label <- V(fg3)$name
    #  if (example=="Ex_Fact14") E(fg3)$label <- c(paste("[df=","1","]",sep=""))
    fg3$layout <- dscoords$coords

    plot(fg3, asp = FALSE, add = TRUE, vertex.label.color = "transparent", vertex.label.cex = dscoords$textlabel.size,
       vertex.label.font = 2, vertex.size = 0, vertex.color = "transparent", vertex.frame.color = "transparent",
       edge.label.color = Colourred, edge.label.font = 2, edge.color = Colourorange, edge.lty = 3)
    if (SilveR == "N") { date() }

}


#-----------------------------------------------------------------------------------------------------------    
#Identify effects which are randomised to others
#Another way which may be better is by using E(g)[DEFINE HERE WHICH ONES]$lty <- 2
#Note for can choose type of dotted/dashed line (2-6) if wish
if (showrandFDS == "Y") {

    # For plot to work need to reverse the order of effects
    randomisedmat.reverse <- randomisedmat[nrow(randomisedmat):1, ncol(randomisedmat):1]
    if (SilveR == "N") { randomisedmat.reverse }

    fg5 <- graph.adjacency(randomisedmat.reverse, mode = "directed") #Change this to "directed" for randomization arrows                     
    fg5 <- simplify(fg5)
    V(fg5)$label <- V(fg5)$name
    fg5$layout <- dscoords$coords

    plot(fg5, asp = FALSE, add = TRUE, vertex.label.color = "transparent", vertex.label.cex = dscoords$textlabel.size,
       vertex.label.font = 2, vertex.size = 0, vertex.color = "transparent", vertex.frame.color = "transparent",
       edge.color = Colourpurple, edge.lty = 5, edge.width = 2, edge.arrow.mode = 1)
    if (SilveR == "N") { date() }

}

if (SilveR == "Y" && pdf == "N" && showFDS == "Y") {
    HTMLbr()
    HTML.title("<bf>Hasse diagram of the Restricted Layout Structure", HR = 2, align = "left")
    void <- HTMLInsertGraph(GraphFileName = sub("[A-Z0-9a-z,:,\\\\]*App_Data[\\\\]", "", FDSPlot), Align = "left")
}



if (SilveR == "Y" && pdf == "Y" && showFDS == "Y") {
    pdfFile2F <- sub("[A-Z0-9a-z,:,\\\\]*App_Data[\\\\]", "", pdfFileF)
    linkToPdfF <- paste("<a href=\"http://silverstats.co.uk/Rreports/", pdfFile2F, "\">Click here to view the PDF of the Hasse diagram of the Restricted Layout Structure</a>", sep = "")
    HTML(linkToPdfF)
}
if (pdf == "Y" && showFDS == "Y") dev.off()


#Saving output to files
#save.image(file=paste(data.folder.location,example,".Rdata",sep=""))

#save(list=c("outputlistip1","mydesign","confounded.main"),
#  file=paste(data.folder.location,example,".Rdata",sep=""))

#load(file=paste(data.folder.location,example,".Rdata",sep=""))


if (SilveR == "Y" && showFDS == "N") {

    #defining length of dataset
    if (dim(mydata)[1] >= length(finalnames.effects)) {
        lengthy <- dim(mydata)[1]
        mydata3 <- mydata
    } else {
        lengthy <- length(finalnames.effects)
        diffy <- length(finalnames.effects) - dim(mydata)[1]
        datadd <- data.frame(matrix(NA, nrow = diffy, ncol = dim(mydata)[2]))
        colnames(datadd) <- colnames(mydata)
        mydata3 <- rbind(mydata, datadd)


    }

    #Creating the RDS dataset

    Randomise_from <- c(rep(NA, lengthy))
    Randomise_to <- c(rep(NA, lengthy))
    RLS_Effects <- c(rep(NA, lengthy))
    RLS_Labels <- c(rep(NA, lengthy))
    User_RLS_Labels <- c(rep(NA, lengthy))
    LS_Effects <- c(rep(0, lengthy))
    LS_Labels <- c(rep(NA, lengthy))
    Fixed_Random_Def <- c(rep(NA, lengthy))

    for (i in 1:lengthy) {
        LS_Effects[i] <- finalnames.effects[i]
        RLS_Effects[i] <- finalnames.effects[i]
    }
    for (i in 1:lengthy) {
        LS_Labels[i] <- c(colnames(adjm))[i]
        RLS_Labels[i] <- c(colnames(adjm))[i]
        User_RLS_Labels[i] <- c(colnames(adjm))[i]
    }



    for (i in 1:length(finaleffectrandom)) {
        if (finaleffectrandom[i] == 0) { Fixed_Random_Def[i] = "Fixed" }
    }
    for (i in 1:length(finaleffectrandom)) {
        if (finaleffectrandom[i] == 1) { Fixed_Random_Def[i] = "Random" }
    }

    #if (testForConfoundedDF == "Y")
    #{
    #Leny<- dim(mydata)[2]
    #mydata2<-mydata[ , - Leny]
    #} else {
    #  mydata2<-mydata
    #}

    FRDdataz <- cbind(mydata3, LS_Labels, Fixed_Random_Def, LS_Effects, Randomise_from, Randomise_to, RLS_Labels, RLS_Effects, User_RLS_Labels)

    HTMLbr()
    HTML.title("<bf>Table of the data for creation of the Restricted Layout Structure", HR = 2, align = "left")
    HTML(FRDdataz, classfirstline = "second", align = "left")
    HTML.title("</bf> ", HR = 2, align = "left")
    HTML.title("To produce the Restricted Layout Structure, cut and paste this dataset into Excel and use the LS_effects variable to generate/amend
             the last five variables.", HR = 0, align = "left")
    HTML.title("</bf> ", HR = 2, align = "left")
    HTML.title("The columns Randomise_from and Randomise_to define the randomisations performed, one row per randomisation. The
             Randomise_from variable contains the terms that are at the start of the randomisation arrows (see xxx). The
             Randomise_to variable contains the terms that are at the end of the randomisation arrows. The entries in these columns are taken from the 
             LS_effects variable.", HR = 0, align = "left")

    #zzz
    #             Randomise_from variable contains the terms that are at the start of the randomisation arrows (see Bate and Chatfield 2015). The

    HTML.title("</bf> ", HR = 2, align = "left")
    HTML.title("We recommend deleting rows from the RLS_Labels, RLS_Effects, User_RLS_Labels and Fixed_Random_Def variables that correspond to terms that 
             need to be excluded from the Restricted Layout Structure.", HR = 0, align = "left")
    HTML.title("</bf> ", HR = 2, align = "left")
    HTML.title("The Fixed_Random_Def variable describes whether the term is designated as fixed or random by the program, the user can change this. 
             The LS_Labels variable is for information only.", HR = 0, align = "left")

}




if (SilveR == "Y") {


    #References

    HTMLbr()
    HTML.title("<bf>Statistical references", HR = 2, align = "left")

    #zzz  
    #  HTML.title("<bf> ", HR=2, align="left")
    #  HTML.title("<bf> Bate S.T. and Chatfield M.J. (2015). Using experimental design and randomisation to construct a mixed model. Submitted for publication.", HR=0, align="left")

    HTMLbr()
    HTML.title("<bf>R references", HR = 2, align = "left")

    HTML.title("<bf> ", HR = 2, align = "left")
    HTML.title("<bf>   R Development Core Team (2008). R: A language and environment for statistical computing. R Foundation for Statistical Computing, Vienna, Austria. ISBN 3-900051-07-0, URL http://www.R-project.org.", HR = 0, align = "left")

    #igraph
    HTML.title("<bf> ", HR = 2, align = "left")
    HTML.title("<bf> 
             Csardi G, Nepusz T: The igraph software package for complex network research, InterJournal, Complex Systems 1695. 2006. http://igraph.sf.net 
             ", HR = 0, align = "left")

    #R2HTML
    HTML.title("<bf> ", HR = 2, align = "left")
    HTML.title("<bf>
           Lecoutre, Eric (2003). The R2HTML Package. R News, Vol 3. N. 3, Vienna, Austria.
           ", HR = 0, align = "left")
}


#4) Put in trap error     - DONE
#1) remove terms which nest - DONE
#2) larger designs - DONE - can now cope with wine example. Error is trapped if example is even larger.
#5) put code in for rds   - DONE
#3) try to pick up where the problem is - DONE
#6) Ex3_Fig 3_2: large - DONE - rejigged coords so effects which are not nested are not below each other
#7) Coping with design with factors not in term order - DONE
#8) Change notation for mean relationships (0) not 0  - DONE
#9) Put = for any number of pairs of Equivalent factors, and lower down just use factor which occurred first in the dataset  - DONE 
#10) The program detects if more than 2 factors are equivalent and stops giving a message to the user - DONE
#11) Check wine example -  Halfplot and 3 pairs of factors - DONE
#12) Program now allows 2 or 3 letter abbreviations. If 3 letter abbrev not different then then program stops with a warning. - DONE
#13) Aded maxlevels.df<-"Y" to identify whether or not max possible levels should be displayed - DONE - Note it should be put in the user input
#14) Interactions put in term order level and alphabetical order within that- DONE
#15) Added a	middle.fontlabelmultiplier <- middleFontMultiplier to have a larger font size for equivalent factors - DONE except Simon needs to change SilvR
#16) Checking when an equivalent factor has a different fixed random designation of the term it is equivalent to. - DONE
#17) Allowing the user to change the random fixed/random designation of terms - DONE except SILVER version needs changing.
#18) Are there any messages which need to stop the program after?
#19) model statement - currently prints out ANOVA statement, needs adapting for mixed model.
#20) Needs tidying up


#21) Best way to extract the confounded terms S4b(ii) (remove generalised factors due to nesting) - if two generalised factors are confounded, we only include the combination of them currently and do not retain the original generalised factors.



























