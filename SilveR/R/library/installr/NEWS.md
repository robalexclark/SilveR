installr 0.23.0 (2021-05-08)
---------------------------

UPDATED FUNCTIONS:
   * updateR - changed copy_Rprofile.site argument to copy_site_files, which will now also copy Renviron.site
   * small edits to improve 32/64 bit architecture detection and appropriate download for Python.
   * install.packages.zip - now removes query parameters from URL.

OTHER NOTES:
   * Appveyor integration added to the github repo
   * installr is now also available to install on linux/mac. The main functionality of the package is primarily geared towards Windows users, but making the package OS agnostic, allows it to be developed/tested by users from other OS (based on the request by Hadley)

BUG FIXES:
   * Fix install.GraphicsMagick
   * Fix create.global.library to allow no arguments
   * Fix install.CMake
   * Fix install.RStudio
   * Fix install.Rtools
   * Fix install.ImageMagick


installr 0.22.0 (2019-08-02)
---------------------------

OTHER NOTES:
   * Started using pkgdown :)

BUG FIXES:
   * Fix install.RStudio
   * Fix install.Rtools - force the user to choose a version to download
   * Make sure to DESCRIPTION of the package makes it clear that it is for Windows OS (and not, say Mac or Linux)


installr 0.21.3 (2019-06-09)
---------------------------

OTHER NOTES:
   * Added OS_type: windows

installr 0.21.2 (2019-06-09)
---------------------------

OTHER NOTES:
   * Updating dates.


installr 0.21.1 (2019-02-09)
---------------------------

BUG FIXES:
   * Fix install.MikTeX
   * Fix install.Rtools
   * Fix install.pandoc
   * Fix install.RStudio
   * Fix install.inno


installr 0.21.0 (2018-10-06)
---------------------------

NEW FUNCTIONS:
   * install.Java
   
UPDATED FUNCTIONS:
   * updated install.python() to check 32/64 bit Windows
   * updated install.conda() to set 32/64 bit Windows

BUG FIXES:
   * Fix install.Java() Documents
   * Fix install.ImageMagick()
   
   

installr 0.20.0 (2018-05-02)
---------------------------

NEW FUNCTIONS:
   * install.nodejs - Downloads and installs the latest version of nodejs LTS or Current for Windows.
   * is.x64 - Checks if the running OS is x64
   * install.conda - Miniconda is minimal version of anaconda for python.

BUG FIXES:
   * Fix install.ImageMagick()
   * Fix install.MikTeX
   * Fix myip


installr 0.19.0 (2017-04-21)
---------------------------

NEW FUNCTIONS:
   * install.inno - "Inno Setup is a free installer for Windows programs. First introduced in 1997, Inno Setup today rivals and even surpasses many commercial installers in feature set and stability."


BUG FIXES:
   * Fix install.Rtools()


OTHER NOTES:
   * Moved stringr to depends. R now must be >2.14



installr 0.18.1 (2016-11-11)
---------------------------

BUG FIXES:
   * Fix install.MikTeX()


installr 0.18.0 (2016-11-05)
---------------------------

NEW FUNCTIONS:
   * install.python - Downloads and installs python 2 or 3

UPDATED FUNCTIONS:
   * install.URL now gives warning if there is suspicion that the user is not connected to the internet.
   * updateR  - added cran_mirror option 



installr 0.17.8 (2016-05-14)
---------------------------

BUG FIXES:
   * Fix "Error in setInternet2(TRUE) : use != NA is defunct" when using updateR()
   * Fix install.CMake() ( fix issue #48 )
   * In require2 change ask from TRUE to FALSE

installr 0.17.7 (2016-04-13)
---------------------------

* Moved stringr and curl to suggest (so to allow users of R version 2.11-3.00 to also use installr for updating their R)


installr 0.17.6 (2016-04-09)
---------------------------

BUG FIXES:
   * copy.packages.between.libraries - can now deal with a file name such as "R-3.2.4revised-win.exe". (by fixing R_version_in_a_folder which was used inside get.installed.R.folders)


installr 0.17.5 (2016-03-20)
---------------------------

UPDATED FUNCTIONS:
   * install.URL now uses download.file again, but gained an option download_fun for using curl::curl_download (for install.npptor)

BUG FIXES:
   * install.R() - can now deal with a file name such as "R-3.2.4revised-win.exe"


installr 0.17.4 (2016-03-19)
---------------------------
BUG FIXES:
   * URL: http://cran.rstudio.com (moved to https://cran.rstudio.com/)



installr 0.17.3 (2016-03-18)
---------------------------

OTHER NOTES:
   * Added stringr and curl as imports to the package (so things would be easier to maintain)
   
UPDATED FUNCTIONS:
   * install.URL now uses curl::curl_download instead of download.file (this deals with issues in sourceforge)
   
BUG FIXES:
   * install.npptor - works again
   * updateR - can now deal with a file name such as "R-3.2.4revised-win.exe"



installr 0.17.2 (2016-02-12)
---------------------------

UPDATED FUNCTIONS:
   * updateR: to_checkMD5sums - now set to FALSE by default (it keeps throwing errors...)


installr 0.17.1 (2015-11-08)
---------------------------

BUG FIXES:
   * install.git() - works again



installr 0.17.0 (2015-09-11)
---------------------------

UPDATED FUNCTIONS:
   * updateR - new parameters added:
      *  setInternet2 - so people would not have the error: "Error in file(con, "r") : cannot open the connection"
      *  fast - overrides other parameters, so to make the installation as fast as possible for the user: no news, installr R, copy packages and Rprofile, keep
old packages, updated packages, without quiting current R or starting the new R.
don't use GUI, check MD5sums, keep installed file in the getwd.
      * The default parameters in "fast" are set to make it easier to update R (on Windows) for people who are blind. (thanks to Jonathan Godfrey)
      
   
OTHER NOTES:
   * Updated troubleshooting in the README.
   * rename use_GUI to GUI.

installr 0.16.0 (2015-06-11)
---------------------------

UPDATED FUNCTIONS:
   * R_version_in_a_folder - now returns NULL if no folder was found.
   * updateR - notices when no sub-R-folder was found - and tells the user what to do (instead of stopping the function - solves issue #27 )
   
BUG FIXES:
   * moving "utils" to depends allows add.installr.GUI to work again.
   * install.notepadpp - works again
   

installr 0.15.17 (2015-06-03)
---------------------------

OTHER NOTES:
   * Added (MORE) :: calls to Fix:  "no visible global function definition for"


installr 0.15.16 (2015-06-02)
---------------------------

UPDATED FUNCTIONS:
   * checkMD5sums2 - now uses warning instead of cat (to help user understand this is a warning and not an error)
   * install.R - now omits the files  ‘bin/R.exe’, ‘bin/Rscript.exe’  from MD5 checksum (since they seem to fail for R 3.2.0 :\ )
   * updateR - reminds the user to stop if running it from RStudio.

OTHER NOTES:
   * Added :: calls to Fix:  "no visible global function definition for"

installr 0.15.15 (2015-03-13)
---------------------------

UPDATED FUNCTIONS:
   * install.ImageMagick - fix (thanks to Fernando and Jazzy)


installr 0.15.14 (2015-02-04)
---------------------------
OTHER NOTES:
   * minor fixes.
   * Added Berry Boessenkool as ctr.


installr 0.15.13 (2014-12-10)
---------------------------

NEW FUNCTIONS:
   * xlsx2csv - turning xlsx to csv using VB.


installr 0.15.12 (2014-11-21)
---------------------------

OTHER NOTES:
   * Changed suppressMessages(require(installr)) to suppressMessages(library(installr))
   * Added a few notes on troubleshooting.


installr 0.15.11 (2014-08-20)
---------------------------

UPDATED FUNCTIONS:
   * updateR - now has updater as an alias.


installr 0.15.10 (2014-08-12)
---------------------------

NEW FUNCTIONS:
   * install.Texmaker
   * install.CMake (as requested by stnava: https://github.com/talgalili/installr/issues/18)


installr 0.15.9 (2014-07-27)
---------------------------

UPDATED FUNCTIONS:
   * updateR - added copy_Rprofile.site option. Requested by: rsuhada, leeper (at: https://github.com/talgalili/installr/issues/13)


installr 0.15.8 (2014-07-24)
---------------------------

UPDATED FUNCTIONS:
   * require2 - fix ask=FALSE


installr 0.15.7 (2014-07-11)
---------------------------

UPDATED FUNCTIONS:
   * R_version_in_a_folder - updated so it could handle "patched" versions of R. (bug report thanks to rogerjbos i.e.: https://github.com/rogerjbos)

installr 0.15.6 (2014-07-10)
---------------------------

OTHER NOTES:
   * Changed all R script files from .r to .R!

installr 0.15.5 (2014-07-04)
---------------------------
NEW FUNCTIONS:
   * rename_r_to_R -  Rename files' extensions in a folder from .r to .R.

NEW FILES ADDED:
   * rename_r_to_R.R

installr 0.15.4 (2014-06-26)
---------------------------

OTHER NOTES:
   * Changed the menu item to be "installr" instead of "Update" (in both zzz.r and startup.r)


installr 0.15.3 (2014-06-14)
---------------------------

OTHER NOTES:
   * Fixes to doc in kill_all_Rscript_s and uninstall.packages.
   * installr 0.15.3 is to be shipped to CRAN


installr 0.15.2 (2014-06-14)
---------------------------

BUG FIXES:
   * install.pandoc - works again. The guys there moved git from googlecode to github, with also a tweak where version numbers can include hyphens ("-").


installr 0.15.1 (2014-06-14)
---------------------------

NEW FUNCTIONS:
   * get_pid - get the process id by its name
   * kill_pid - kills the process of all the scripts with some pid
   * kill_process - kills process with some name


BUG FIXES:
   * install.git - works again. The guys there moved git from googlecode to github. (props to Tom Bancroft for the bug report)


installr 0.15.0 (2014-06-08)
---------------------------

NEW FUNCTIONS:
   * get_tasklist - returns a data.frame with the current running processes. Windows only.
   * get_Rscript_PID - returns all of the process IDs (i.e.: PID) of the Rscript.exe that are running. Windows only.
   * kill_all_Rscript_s - kills the process of all the Rscript.exe that are running (you can schedule how long it should wait before doing it). Windows only.


UPDATED FUNCTIONS:
   * install.URL - made sure to exit the function if it could not download the file

NEW FILES ADDED:
   * kill_pid.r


OTHER NOTES:
   * Added copyrights to all .r files.


installr 0.14.5 (2014-05-19)
---------------------------

UPDATED FUNCTIONS: (by Boris Hejblum)
   * Update pkgDNLs_worldmapcolor    
      Updating pkgDNLs_worldmapcolor function with ggplot2::aes_string (better
      for R CMD CHECK).
      Also, the 'data' directory of the package is not on the CRAN (but is on
      the github version of the package). pkgDNLs_worldmapcolor(...) needs the
      'WorldBordersData.RData' file in order to work.

installr 0.14.4 (2014-05-08)
---------------------------
UPDATED FUNCTIONS:
   * install.pandoc - the project has moved from http://code.google.com/p/pandoc/downloads/list to https://github.com/jgm/pandoc/releases - so the function had to be updated... (props to  Patrick Kelley for the bug report)
   * install.url - tweaks to the massage.

installr 0.14.3 (2014-05-06)
---------------------------
UPDATED FUNCTIONS:
	* install.URL - added installer_option A character of the command line arguments
	* install.R, updaeR - added silent installation mode (source: http://hiratake55.wordpress.com/2014/05/01/how-to-do-a-silent-install-of-r/)

UPDATED DESCRIPTION:
    * Bump version to 0.14.3
	* added Takekatsu Hiramura as a contributor.
	
installr 0.14.2 (2014-04-17)
---------------------------
NEW FUNCTIONS ADDED:
   * uninstall.packages - removes packages from R.


installr 0.14.1 (2014-04-12)
---------------------------
UPDATED FUNCTIONS:
   * download_RStudio_CRAN_data - added the override and massage parameters (makes sure we do not have to download files already downloaded)


installr 0.14.0 (2014-04-02)
---------------------------
OTHER NOTES:
   * Bump version to 0.14.0 to signify the number of bug fixes from the original 0.13.0.
   * installr 0.14.0 is to be shipped to CRAN.


installr 0.13.8 (2014-03-19)
---------------------------
UPDATED FUNCTIONS:
   * install.Cygwin - JackStat   Fixed URL and added in 32 and 64 bit options. roxygenized


installr 0.13.7 (2014-03-13)
---------------------------
UPDATED FUNCTIONS:
   * R_version_in_a_folder - Handled the case of using one directory for subsequent overwriting R installations by selecting the last one. Contributed by Dieter Menne.



installr 0.13.6 (2014-03-12)
---------------------------

UPDATED FUNCTIONS:
   * install.URL - new parameters added:
         * massage (TRUE) parameter. The function now prints (cat) if the file was downloaded succesfully or not, combined with the location of the file. It also informs the user if the file is later installed/removed or not.
   * updateR, install.R - new parameters added:
         * keep_install_file parameter (default is FALSE). Based on the request by Boral here: http://www.r-statistics.com/2014/03/r-3-0-3-is-released/
   * install.URL, install.R, updateR - new parameters added:
         * download_dir (tempdir) character. Allowing the user to choose into which directory the file should be downloaded.

installr 0.13.5 (2014-03-01)
---------------------------

UPDATED FUNCTIONS:
   * os.manage - shouldn't ask for minutes if no appropriate option was selected. Don't ask for minutes if 'Cancel' is selected or window was closed without selecting any option. Props to Kornelius Rohmeyer for the commit (rohmeyer@small-projects.de).


installr 0.13.4 (2014-02-06)
---------------------------

UPDATED FUNCTIONS:
   * install.pandoc - 
      * if the installation failed, no need to ask about restarting the computer!
      * retiring "use_regex" from service. (props to Scott Milligan for the bug report.)


installr 0.13.3 (2014-02-01)
---------------------------

OTHER NOTES:
   * DESCRIPTION:
      * Fixed the usage of person()
      * minor tweaks to Title and Description
   * installr 0.13.3 is to be shipped to CRAN (thanks to the patient guidance of Uwe Ligges!)

installr 0.13.2 (2014-01-31)
---------------------------

OTHER NOTES:
   * moved tools from imports to suggest in order to avoid "call not declared from" error.
   * Added "RdTags" as an internal function, since using tools:::RdTags caused too many problems.
   * Fixed a single "@" in a roxygen2 doc (which caused erros)
   * Changed main e-mail to be @gmail instead of @math.tau.ac.il.
   * installr 0.13.2 is to be shipped to CRAN.



installr 0.13.1 (2014-01-31)
---------------------------

UPDATED FUNCTIONS:
   * read_RStudio_CRAN_data - Added a packages= option to read_RStudio_CRAN_data. read_RStudio_CRAN_data fails on out-of-memory (in the rbindlist call) if you've downloaded many RStudio CRAN logs.  This patch adds a packages= option if you're only interested in a subset of packages, which subsets the daily log data on the fly.  Other such filters might also be added.
   * Changed DESCRIPTION - removed maintainer (to be made automatically)

OTHER NOTES:
   * installr 0.13.1 is to be shipped to CRAN.


installr 0.13.0 (2014-01-30)
---------------------------

UPDATED FUNCTIONS:
   * make checkMD5 issue warnings (maybe with a dialog box) instead of errors (since the current state of affairs is more hassle than it is worth...)

BUG FIXES:
   * Updated some functions and documents in order to pass CRAN checks: RStudio_CRAN_data_folder, pkgDNLs_worldmapcolor, fetch_tag_from_Rd, add_to_.First_in_Rprofile.site, remove_from_.First_in_Rprofile.site, add_load_installr_on_startup_menu.
   * README.md - some tweaks to work with the HTML option on CRAN.

OTHER NOTES:
   * installr 0.13.0 is to be shipped to CRAN.



installr 0.12.2 (2013-11-26)
---------------------------
UPDATED FUNCTIONS:
   * install.pandoc - added "to_restart" parameter. With a massage to remind the user they should restart their computer after pandoc is installed.


installr 0.12.1 (2013-08-23)
---------------------------
BUG FIXES:
   * install.URL - didn't work on Windows. It was because "system" did not run .exe, and "shell" was the correct choice to use here.


installr 0.12.0 (2013-08-21)
---------------------------
NEW FUNCTIONS ADDED:
   * pkgDNLs_worldmapcolor (kindly contributed by Boris Hejblum) - A worldmap colored by the number of downloads for a given package
   * fetch_tag_from_Rd - A function to extract elements from R's help file.
   * package_authors - finding all the authors for this (and other) packages.

OTHER NOTES:
   * Gave credit to other contributers of this package in the DESCRIPTION file (using the fetch_tag_from_Rd function). Contributers' names are sorted alphabetically.


installr 0.11.0 (2013-08-17)
---------------------------
NEW FEATURES:
   * Adds a menu-item on startup to allow the automatic loading of installr whenever starting R.

NEW FUNCTIONS ADDED:
   * add_load_installr_on_startup_menu
   * add_remove_installr_from_startup_menu
   * add_to_.First_in_Rprofile.site
   * is_in_.First_in_Rprofile.site
   * load_installr_on_startup
   * remove_from_.First_in_Rprofile.site
   * rm_installr_from_startup

NEW FILES ADDED:
   * startup.r

installr 0.10.3 (2013-07-13)
---------------------------
IMPORTANT NOTES:
   * TODO: this release is a step before 0.11.0 - when mac will be supported.

UPDATED FUNCTIONS:
   * checkMD5sums2 - added "..."
   * check.for.updates.R - added a "pat" (pattern) parameter (this function might need more tweaking)
   * install.URL - moved to using "system" instead of "shell" (purpose - to allow the function to be useful for mac users as well.)

NEW FUNCTIONS ADDED:
   * up_folder - gets a vector of characters of folder paths, and returns them after turning them into a file.path once their tail was trimmed (through head(n = - 1).


BUG FIXES:
   * install.Rtools - now returns FALSE gracefully if the user chooses to abort.

OTHER NOTES:
   * moved to using "system" instead of "shell" (purpose - to allow the function to be useful for mac users as well.)



installr 0.10.2 (2013-07-06)
---------------------------

UPDATED FUNCTIONS:
   * updateR - added a test to be sure the new R version was installed in the same relative directory as the old R version (otherwise, the copy-packages function should not work)

OTHER NOTES:
   * Updating the docs a bit (description).


installr 0.10.1 (2013-07-04)
---------------------------

OTHER NOTES:
   * Fixed some .Rd width problems
   * installr 0.10.1 is to be shipped to CRAN.


installr 0.10.0 (2013-07-02)
---------------------------

NEW FUNCTIONS ADDED:
   * uninstall.R
   * is.empty - function added for checking if an object is empty (e.g: of zero length)
   * install.Rdevel - usefull for when developing R packages (as this version contains the latest checks.)

UPDATED FUNCTIONS:
   * check.for.updates.R - added a date for the latest R version in the window prompt.

OTHER NOTES:
   * Moving to the versioning scheme suggested by Yihui (see: http://yihui.name/en/2013/06/r-package-versioning/)
   * is.x.r file created - moving various "is.*" functions to this file.
   * Added a welcome massage when loading the package.
   * Updated the README.md with some of the newer functions.


installr 0.9.5 (2013-06-12)
---------------------------

NEW FUNCTIONS ADDED:
   * download_RStudio_CRAN_data
   * read_RStudio_CRAN_data
   * barplot_package_users_per_day
   * lineplot_package_downloads
   * format_RStudio_CRAN_data
   * most_downloaded_packages

UPDATED FUNCTIONS:
   * require2 - the 'package' parameter no longer needs quotes. (just like in the require function)

OTHER NOTES:
   * RStudio_CRAN_data.r file created



installr 0.9.4 (2013-05-21)
---------------------------

NEW FUNCTIONS ADDED:
   * myip - return your ip address.
   * freegeoip - Geolocate IP addresses in R (contributed by Heuristic Andrew)
   * restart_RGui - a function to restart Rgui from Rgui
   
UPDATED FUNCTIONS:
   * require2 - added the "ask" parameter.

OTHER NOTES:
   * geo_functions.r file created
   * cranometer - function moved to the geo_functions.r file.


installr 0.9.3 (2013-05-18)
---------------------------

UPDATED FUNCTIONS:
   * install.R - started using checkMD5sums2 instead of checkMD5sums in order to allow the manual removal of the files ‘etc/Rconsole’ and ‘etc/Rprofile.site’ from the checksums (they are changed during the R installation process, and always give an error massage)
   * checkMD5sums2 - added the omit_files parameter (allowing the removal of the files ‘etc/Rconsole’ and ‘etc/Rprofile.site’ from the checksums of an R installation)
   * get.installed.R.folders - making the function more robust, by having it rely on the README files instead of the folder names. (based on a bug report by GilesCrane - thanks.)
   * R_version_in_a_folder function added (get the version of R installed in a folder. Based on the README file name inside the folder)


installr 0.9.2 (2013-05-14)
---------------------------

NEW FUNCTIONS ADDED:
   * cranometer - Estimates the speed of each CRAN mirror by measuring the time it takes to download the NEWS file. (including an example of creating a world-map based on this data)
   * require2 - just like "require", only makes sure to download and install the package in case it is not present on the system (useful for examples...)

OTHER NOTES:
   * More documentation.  Updated README.md
   * Fixed width of various elements in the documentation
   * installr 0.9.2 is to be shipped to CRAN.


installr 0.9.1 (2013-04-04)
---------------------------

NEW FUNCTIONS ADDED:
   * checkMD5sums2 - Just like checkMD5sums from the tools package, but with the added md5file parameter. (useful for manually checking if the downloaded R EXE installer is fine, combined with this MD5 file: http://cran.rstudio.com/bin/windows/base/md5sum.txt)
   * browse.latest.R.NEWS() - sends the user to the latest R NEWS file.

UPDATED FUNCTIONS:
   * install.URL - updated the error massage to explain better what (probably) went wrong.  And encourage the users to e-mail me with bug reports.
   * add.installr.GUI - made sure the function wouldn't add items if "Update" is already present. e.g: made a fail-safe mechanism in case the function installr:::add.installr.GUI() is run more than once (my thanks goes to Dieter Menne for the suggestion)
   * install.R - Updated the warning massage on install.R in case checkMD5sum fails.
   * updateR -
      ** added the to_checkMD5sums parameter to updateR (to be passed on to install.R() )
      ** it now uses the browse.latest.R.NEWS() function.

BUG FIXES:
   * install.pandoc - fixed it to work with the new URL scheme they've chosen for their site (they moved to MSI instead of .EXE about two weeks ago...)
   * updateR - fixed a typo in the installation massage (thanks to Henrik Pärn)

OTHER NOTES:
   * Fixed wrongly used capital letters (for example, in the NEWS file)


installr 0.9 (2013-03-29)
---------------------------

UPDATED FUNCTIONS:
   * Made sure to include a lower-case version of all install.X functions.

OTHER NOTES:
   * More documentation.  Updated README.md
   * installr 0.9 is to be shipped to CRAN.



installr 0.8.8 (2013-03-26)
---------------------------
NEW FUNCTIONS ADDED:
   * install.notepadpp
   * install.npptor
   * install.Cygwin
   * install.LaTeX2RTF



installr 0.8.7 (2013-03-16)
---------------------------
NEW FUNCTIONS ADDED:
   * install.SWFTools
   * install.FFmpeg
   * install.7zip - for unzipping of FFmpeg
   * system.PATH - to see what is in the users PATH for running .exe programs.

UPDATED FUNCTIONS:
   * os.manage - now asks the user in how many minutes to perform the operation.

BUG FIXES:
   * Fixed a bug in install.LyX.rd that caused it to not load properly when using install_github (props goes to Richard Cotton for catching the bug.)

installr 0.8.6
--------------

NEW FUNCTIONS ADDED:
   * install.ImageMagick
   * install.GraphicsMagick
   * install.LyX
   * os.manage, os.shutdown, os.restart, os.hibernate, os.sleep, os.lock (set of functions to turn off the computer after simulation or the likes)

UPDATED FUNCTIONS:
   * install.Rtools - removed "latest_Frozen", added "check".  Made the function know which Rtools to install based on your R version (and if it is not known - it asks the user to choose a version)
   * Removed shutdown (turned into os.shutdown), and made sure it uses "force" shutdown.


installr 0.8.5
--------------

NEW FUNCTIONS ADDED:
   * installr - allows the user to choose which software to install (from a GUI or console based menu system)
   * A new GUI for Rgui - added the "Update" to the menu, with three submenu items: Update R, Update R Packages, and Installing software. (thanks to Dason and Yihui Xie for their ideas and help)
   * shutdown - Shut down the operating system with the command `shutdown'.  A modified version of Yihui's shutdown function from the {fun} package (see: https://github.com/yihui/fun/blob/master/R/shutdown.R)
   * is.RStudio - checks if the current R session is running within RStudio or not.
   * is.Rgui - checks if the current R session is running within Rgui or not.
   * add.installr.GUI and remove.installr.GUI - for adding a menu system to Rgui

UPDATED FUNCTIONS:
   * install.URL - 
      * added a "wait" parameter with default as F (it now means that R doesn't wait for the installation of the software to finish before it releases the console).  And also "..." with access to the shell command run.
      * Made sure that if keep_install_file = FALSE then wait= TRUE (since otherwise, we'll erase the file before we get to run it)
      * output now returns TRUE/FLASE on whether the installation worked or not.
   * install.?? - now returns TRUE/FLASE on whether to installation worked or not.
   * install.R - Now uses MD5sums to check the newly installed R has all of the files it should.
   * updateR - 
      * added a menu based GUI!
      * added setInternet2(TRUE) for when updating packages (to help it work with specification of proxies, etc.). (this seems to be safe for regular users - but I hope it won't cause new problems.).  Thanks to a bug report by Gilbert Pétain-Coup.
      * now checks if R was installed or not (Based on the output of install.R), and removed the need to ask the user that.
   * ask.user.yn.question - added a menu based GUI.
   * check.for.updates.R - added a menu based GUI.

BUG FIXES:
   * Fixed a bug in updateR, to make sure it will be able to open the new Rgui, and also close the old R.
   * Fixed a bug in updateR, "Error in !install_R : 'install_R' is missing" (install_R was called in too early) (thanks to AC for the bug report!)


installr 0.8 (2013-03-05)
-------------------------

FIXES FOR CRAN SUBMISSION:
   * Fixed some spelling mistakes in DESCRIPTION,
   * Made sure to run the checks in the latest R version (R 2.15.3, oh the irony)
   * Changed update.R to be called updateR (in order to avoid confusing it as an S3 variation to 'update')
   * installr 0.8 is to be shipped to CRAN.


My thanks goes to Prof Brian Ripley for his help.


installr 0.7
--------------

NEW FUNCTIONS ADDED:
   * ask.user.yn.question - Asks the user for one yes/no question.

UPDATED FUNCTIONS:
   * update.R  - fixed a bug in quit_R, and added an option to open the Rgui of the new R. Started using ask.user.yn.question in the function (to make it more readable).
   * fixed some parameters not defined in the function.
   * made install.MikTeX more friendly in case a wrong version number is specified.

OTHER NOTES:
   * More documentation.  Updated README.md (fixed \link vs \url)

installr 0.6
------------

NEW FUNCTIONS ADDED:
   * get.installed.R.folders - Returns folder names with R installations
   * copy.packages.between.libraries - Copies all packages from one library folder to another

UPDATED FUNCTIONS:
   * update.R  - major update.  Now the user can copy his packages from the old R version to the new version.
   * ask.user.for.a.row - new "questions_text" parameter

OTHER NOTES:
   * Updated the NEWS
   * More documentation
   * Added "\dontrun{" so to make the package pass CRAN tests (http://stackoverflow.com/questions/12038160/how-to-not-run-an-example-using-roxygen2)


installr 0.3-0.5
----------------

NEW FUNCTIONS ADDED:
   * source.https
   * install.MikTeX
   * install.git
   * install.RStudio
   * install.GitHub
   * create.global.library - a merge of create.global.library.oldR and create.global.library.newR (from the post: http://www.r-statistics.com/2011/04/how-to-upgrade-r-on-windows-7/)
   * is.windows() - so when the function is loaded it is checked if the current OS is windows or not.

UPDATED FUNCTIONS:
   * isntall.R - removed extra parameter.
   * update.R - allow the user to review the NEWS of the newer version

OTHER NOTES:

   * Updated the NEWS format and text
   * Updated the description file
   * Changed the name of the package from installR to installr.
   * Added a README.md	
   * Added documentation to all the functions in install.r (via roxygen2)


installR 0.2
------------

NEW FUNCTIONS ADDED:

   * install.packages.zip - for installing package from a url of the ZIP file
   * install.Rtools - for installing Rtools (allowing the user to choose which version to download)	
   * update.R - for checking if we have the latest version of R - and if not - download and install it.
   * install.R
	


installR 0.1 (2013-03-01) 
-------------------------

NEW FUNCTIONS ADDED:

   * install.pandoc() function is created.

OTHER NOTES:

	* Includes skeletons for some functions that will be added in the future.
	

	

TODO for future releases:
-------------------------
	* Better integration with the "global library" strategy
	* make the os.manage work by running an Rscript, in order to allow for their use in long running of knitr/Sweave projects.
   * Add a rate function for the package
   * Add automatic check for a new R version every X time.
   * a way to copy/move Rprofile.site from one installation to the next such as "C:\Program Files\R\R-2.15.2\etc\Rprofile.site" to "C:\Program Files\R\R-2.15.3\etc\Rprofile.site"? (requested by: Farrel Buchinsky)
   * Add more checks for when copying library folders. If it is found the user has more than one library - show him the options of what he is copying from where to where - and ask the user to approve.  If he denies, to ask him which folder to choose "from", and which "to" copy the libraries into.
   *add a general package help file.
   * Fix install.RStudio to act differently in case it is being run from within RStudio (keeping the installation file for the user to run it)
   *  Add control on which mirror to use when running install.R()
   *  allow a user to browse and find the old and new R installation on his system, in case there is a change in file directory from the standard directory structure.
   * consider having a way to deal with R version installed in other folders.
   * create updateRdevel?
   * install.Rpatch # TODO someday
   * install.python # TODO someday
   * install.java # with correct downloading for each type of windows R (version 32 or 64 bits...
   * install.qpdf - http://sourceforge.net/projects/qpdf/files/qpdf/
