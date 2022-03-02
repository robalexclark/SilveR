# SilveR

SilveR is a cross platform (Windows, Linux & macOS) statistical analysis system with the UI written in .net core (currently 3.1.x), hosted in an Electron shell using Electron.net, local data storage in SQLLite and the statistical analysis performed in R 3.6.3.

### Branded as InVivoStat

The SilveR project is currently branded as InVivoStat as it is the cross platform replacement for the winforms version of the InVivoStat system (hosted at http://invivostat.co.uk/). However, the statistical analyses in this system are applicable to non-pharmaceutical studies. The system is designed so that the branding can be easily changed.

## Getting Started

Clone the project, open in Visual Studio, optionally run the tests, and then debug or publish. The repo contains a stripped down version of R 3.6.3 for windows so that it should produce analyses straight away (linux and mac need to install their own). You can always point the system at your own R setup (YMMV!)

### Current Build

[![Build Status](https://dev.azure.com/robalexclark/SilveR/_apis/build/status/robalexclark.SilveR?branchName=master)](https://dev.azure.com/robalexclark/SilveR/_build/latest?definitionId=9&branchName=master)

### Installing

SilveR is a self-hosted web app written in .net core, running in an Electron shell, designed to run locally on Windows, Linux and macOS.

#### [Windows](#windows)

A stripped down install of R 3.6.3 for windows with the correct packages is included in the repository. It should run out of the box in visual studio. However if you are publishing for redistribution you will need to xcopy the R folder to the root of the publish output folder.

#### [Linux](#linux)

For linux (only tested on Ubuntu 19+ so far) you will need to provide an install of R. To do this:

1) Download the setup script from https://raw.githubusercontent.com/robalexclark/SilveR/master/SilveR/setup/setup-linux.sh and R package install script from https://raw.githubusercontent.com/robalexclark/SilveR/master/SilveR/setup/RPackagesInstall.R into the same folder.

2) Run the setup script from the terminal prompt using 'sudo ./setup-linux.sh'

Note that we only support 19.x at this time. If you have a different Linux distro then as long as you can get R 3.6.x installed on your own and the subsequent R packages then you should be fine.

#### [MacOS](#macos)

For Mac you will need to install R  and run a script to install the required R packages. To do this:

1) Download and install R from https://cran.r-project.org/bin/macosx/base/R-4.1.3.pkg

2) Download the R package install script from https://raw.githubusercontent.com/robalexclark/SilveR/master/SilveR/setup/RPackagesInstall.R. Open the R editor/IDE (that was installed in the previous step). Copy and paste the code (or open the RPackagesInstall.R file) into the R editor and run it. The second half of the R script checks that the libraries have been installed correctly.

3) It seems that macOS Big Sur onwards has issues where a symlink to Rscript is missing or miconfigured after the R install (resulting in InVivoStat claiming that R is not installed). To resolve this run the following two lines in the terminal

```
sudo ln -s /Library/Frameworks/R.framework/Resources/bin/Rscript /usr/local/bin/Rscript
sudo ln -s /Library/Frameworks/R.framework/Resources/bin/R /usr/local/bin/R
```

#### Running Locally

To run the published system locally in a browser run SilveR.exe. A console window will appear stating that the system is listening on http://localhost:5000. Open your browser and navigate to that location.

To run through an analysis:
1) Click on the Data tab and import your csv or xlsx file (selecting a worksheet if necessary)
2) Click on the Statistics tab select an analysis
3) Pick a dataset to analyse
4) Select some column(s) from the data to produce results for and click Submit
5) The dataset and selected columns will be passed to the R engine (via rscript.exe) and results produced

You can open the Analysis tab to see your previous results, reanalyse to see the settings selected and re-submit your analysis.

#### Running in Electron shell
For details on how to build with the electron.net wrapper see https://github.com/ElectronNET/Electron.NET


### Architecture

Each analysis type consists of a UI defined in a View (e.g. SummaryStatistics.cshtml), a Model (e.g. SummaryStatistics.cs) and a class containing validation (e.g. SummaryStatisticsValidator.cs).

When an analysis is submitted by the user to the AnalysisController the following process occurs:

1) It is validated using any data annotations detailed in the statsmodel class and then passed through the Validate() method in the validator class.

2) If validation passes then the analysis is submitted (via a background queue) to the RProcessorService.

3) The RProcessorService exports a csv file from the selected data, and assembles a set of command line arguments from the selections made by the user with the name of the R script (e.g. SummaryStatistics.R) being passsed as the first argument.

An example of a set of command line arguments sent to R (Rscript.exe) would be as follows:

Rscript.exe SummaryStatistics.R --vanilla -args 0a76cd64-ab69-4315-84eb-809b967a6afd.csv Respivs_sp_ivs2,Resp8 None Cat4 Cat5 Cat6 Cat456 Y Y Y N N N N N Y 95 N N

The Rscript.exe process is launched via Process.Start and runs with the output from the analysis saved as an html file. This output is parsed to inline any graphics and saved (along with the R console output) to the local database.

Viewing of the results file entails the inline html being extracted from the local database and injected into a page.

## Built With

* [.net Core](https://dotnet.microsoft.com/download) - The web framework used
* [electron.net](https://github.com/ElectronNET/Electron.NET) - Hosts in local web app as a desktop applicaton
* [sqllite](https://www.nuget.org/packages/Microsoft.Data.Sqlite.Core/) - Local data storage of data files and analyses
* [R](https://www.r-project.org/) - The R system for statistical analysis, powered by R script files

## Ongoing and Future Development 

1) Finialise setup process on linux and osx
2) Refactor validation classes and methods to standardise and remove any duplication
3) Port to Blazor/razor components
4) Investigate further modules for development

## Contributing or Issues

If you would like to contribute or find an issue with the system then please raise an issue ticket. I'm happy to help developers write modules for the main branch of the system as long as the statistical methodology is reasonably mainstream. 

## License

This project is licensed under the [MIT License](https://opensource.org/licenses/MIT)
