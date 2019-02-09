# SilveR

SilveR is a cross platform (Windows, Linux & macOS) statistical analysis system with the UI written in .net core (currently 2.1.x), local data storage in DbSQLLite and the statistical analysis performed in R 3.5.1.

## Getting Started

Clone the project, open in Visual Studio, optionally run the tests, and then debug or publish. The repo contains a stripped down version of R 3.5.1 so that it should produce analyses straight away. You can always point the system at your own R setup (YMMV!)

### Current Build

[![Build Status](https://travis-ci.com/robalexclark/SilveR.svg?branch=master)](https://travis-ci.com/robalexclark/SilveR)

### Installing

SilveR is a self-hosted web app written in .net core, designed to run locally on Windows, Linux and macOS.

Releases can be produced through the following dotnet commands:

dotnet publish -c Release -r win-x64

dotnet publish -c Release -r linux-x64

dotnet publish -c Release -r osx-x64 

For windows you will need to xcopy the R-3.5.1 folder to the root of the publish output folder. For linux and mac you will need to provide your own R install (We need help here to work out how to add a specific version of R into the build/installation process).

To run the published system run SilveR.exe. A console window will appear stating that the system is listening on http://localhost:5000. Open your browser and navigate to that location.

To run through an analysis:
1) Click on the Data tab and import your csv or xlsx file (selecting a worksheet if necessary)
2) Click on the Statistics tab select an analysis
3) Pick a dataset to analyse
4) Select some column(s) from the data to produce results for and click Submit
5) The dataset and selected columns will be passed to the R engine (via rscript.exe) and results produced

You can open the Analysis tab to see your previous results, reanalyse to see the settings selected and re-submit your analysis.

### Architecture

Each analysis type consists of a UI defined in a View (e.g. SummaryStatistics.cshtml), a Model (e.g. SummaryStatistics.cs) and a class containing validation (e.g. SummaryStatisticsValidator.cs).

When an analysis is submitted by the user to the AnalysisController the following process occurs:

1) It is validated using any data annotations detailed in the statsmodel class and then passed through the Validate() method in the validator class.

2) If validation passes then the analsysis is submitted (via a background queue) to the RProcessorService.

3) The RProcessorService exports a csv file from the selected data, and assembles a set of command line arguments from the selections made by the user with the name of the R script (e.g. SummaryStatistics.R) being passsed as the first argument.

An example of a set of command line arguments sent to R (Rscript.exe) would be as follows:

Rscript.exe SummaryStatistics.R --vanilla -args 0a76cd64-ab69-4315-84eb-809b967a6afd.csv Respivs_sp_ivs2,Resp8 None Cat4 Cat5 Cat6 Cat456 Y Y Y N N N N N Y 95 N N

The Rscript.exe process is launched via Process.Start and runs with the output from the analysis saved as an html file. This output is parsed to inline any graphics and saved (along with the R console output) to the local database.

Viewing of the results file entails the inline html being extracted from the local database and injected into a page.

## Built With

* [.net Core](https://dotnet.microsoft.com/download) - The web framework used
* [sqllite](https://www.nuget.org/packages/Microsoft.Data.Sqlite.Core/) - Local data storage of data files and analyses
* [R](https://www.r-project.org/) - The R system for statistical analysis, powered by R script files

## Ongoing and Future Development 

1) Finialise the migration of stats modules from InVivoStat (in progress)
2) Provide "wrapper" for Windows and MacOS to host application and provide browser (e.g. electron or chromely).
3) Develop installer for Windows and MacOS
4) Port to .net core 3.x and Blazor/razor components when these are released

## Contributing

TODO 


## License

This project is licensed under the MIT License - see the [LICENSE.md](LICENSE.md) file for details
