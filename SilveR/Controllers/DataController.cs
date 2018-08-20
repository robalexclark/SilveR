using ExcelDataReader;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using SilveR.Models;
using SilveR.ViewModels;
using SilveRModel.Helpers;
using SilveRModel.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace SilveR.Controllers
{
    public class DataController : Controller
    {
        private readonly SilveRRepository repository;

        public DataController(SilveRRepository repository)
        {
            this.repository = repository;
        }

        [HttpGet]
        public async Task<ActionResult> Index()
        {
            ViewBag.HasDatasets = await repository.HasDatasets();

            ViewBag.InfoMessage = TempData["InfoMessage"];
            ViewBag.ErrorMessage = TempData["ErrorMessage"];

            return View();
        }

        [HttpGet]

        public async Task<ActionResult> GetDatasets()
        {
            IList<DatasetViewModel> datasets = await repository.GetDatasetViewModels();
            return Json(datasets);
        }

        [HttpGet]
        public ActionResult DataUploader()
        {
            return View("DataUploader");
        }

        //uploading a file
        [HttpPost]
        public async Task<IActionResult> DataUploader(IEnumerable<IFormFile> files)
        {
            IEnumerable<string> fileInfo = new List<string>();
            if (files.Any() == false || files.Single().Length == 0)
            {
                ViewBag.ErrorMessage = "File failed to load, please try again";
                return View();
            }
            else
            {
                ContentDispositionHeaderValue fileContent = ContentDispositionHeaderValue.Parse(files.Single().ContentDisposition);
                string fileName = Path.GetFileName(fileContent.FileName.ToString().Trim('"'));

                string filePath = Path.Combine(Path.GetTempPath(), fileName);

                if (System.IO.File.Exists(filePath))
                    System.IO.File.Delete(filePath);

                using (FileStream fileStream = new FileStream(filePath, FileMode.Create))
                {
                    files.Single().CopyTo(fileStream);
                }

                return await LoadFile(new FileInfo(filePath));
            }
        }

        private async Task<IActionResult> LoadFile(FileInfo selectedFile)
        {
            if (selectedFile.Extension.ToUpper() == ".CSV") //no necessary as already filtering...
            {
                //use the CSVReader to read in the data
                string message;
                DataTable dataTable = CSVHelper.CSVDataToDataTable(selectedFile.OpenRead(), GetCulture());

                if (dataTable == null) //then failed to be read
                {
                    message = "The CSV data needs to be in the correct format.";
                }
                else //got datatable but need to check
                {
                    message = CheckDataTable(dataTable);
                }

                if (message == null)
                {
                    //save in database
                    await SaveDatasetToDatabase(selectedFile.Name, dataTable);

                    try
                    {
                        System.IO.File.Delete(selectedFile.FullName);
                    }
                    catch { }

                    TempData["InfoMessage"] = "File imported successfully!";
                }
                else
                {
                    TempData["ErrorMessage"] = message;
                }

                return RedirectToAction("Index"); //return to file loader/loaded files display screen
            }
            else if (selectedFile.Extension.ToUpper() == ".XLS" || selectedFile.Extension.ToUpper() == ".XLSX")
            {
                System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);

                //read in xls file
                using (FileStream stream = selectedFile.OpenRead())
                {
                    using (IExcelDataReader excelReader = ExcelReaderFactory.CreateReader(stream))
                    {
                        DataSet excelDataSet = excelReader.AsDataSet(new ExcelDataSetConfiguration() { ConfigureDataTable = (_) => new ExcelDataTableConfiguration() { UseHeaderRow = true } });

                        //if there is more than one worksheet then ask the user which sheet they want, otherwise automatically load it
                        if (excelDataSet.Tables.Count == 0)
                        {
                            TempData["ErrorMessage"] = "Error reading file. No sheets with valid data could be found in the Excel workbook.";
                            return RedirectToAction("Index");
                        }
                        else if (excelDataSet.Tables.Count == 1)
                        {
                            DataTable dataTable = excelDataSet.Tables[0];
                            string message = CheckDataTable(dataTable);

                            if (message == null)
                            {
                                //save in database
                                await SaveDatasetToDatabase(selectedFile.Name, dataTable);

                                try
                                {
                                    System.IO.File.Delete(selectedFile.FullName);
                                }
                                catch { }

                                TempData["InfoMessage"] = "File imported successfully!";
                            }
                            else
                            {
                                TempData["ErrorMessage"] = message;
                            }

                            return RedirectToAction("Index"); //return to file loader/loaded files display screen
                        }
                        else //more than 1 excel sheet
                        {
                            List<string> tableNames = new List<string>();
                            foreach (DataTable t in excelDataSet.Tables)
                            {
                                tableNames.Add(t.TableName);
                            }

                            TempData["TableNames"] = tableNames;
                            return RedirectToAction("SheetSelector", new { filename = selectedFile.FullName }); //go to sheet selection screen
                        }
                    }
                }
            }
            else throw new Exception("File format not recognised.");
        }

        private void CleanUpDataTable(DataTable dataTable)
        {
            foreach (DataColumn dc in dataTable.Columns)
            {
                dc.ColumnName = dc.ColumnName.Trim();
            }

            CultureInfo culture = GetCulture();
            string decSeparator = culture.NumberFormat.NumberDecimalSeparator;

            foreach (DataRow row in dataTable.Rows)
            {
                foreach (DataColumn dc in dataTable.Columns)
                {
                    if (!String.IsNullOrEmpty(row[dc].ToString()))
                    {
                        row[dc] = row[dc].ToString().Replace(decSeparator, ".").Trim();
                    }
                }
            }
        }

        private async Task SaveDatasetToDatabase(string fileName, DataTable dataTable)
        {
            CleanUpDataTable(dataTable);

            AddSelectedColumn(dataTable);

            //get last version no based on existing names
            int lastVersionNo = 0;
            IList<Dataset> existingDatasets = await repository.GetExistingDatasets(fileName);
            if (existingDatasets.Any())
            {
                lastVersionNo = existingDatasets.Max(d => d.VersionNo);
            }

            //create entity and save...
            Dataset dataset = new Dataset();
            string[] csvArray = dataTable.GetCSVArray();
            dataset.TheData = String.Join(Environment.NewLine, csvArray);
            dataset.DatasetName = fileName;
            dataset.VersionNo = lastVersionNo + 1;
            dataset.DateUpdated = DateTime.Now;

            await repository.CreateDataset(dataset);
        }

        private void AddSelectedColumn(DataTable dataTable)
        {
            if (!dataTable.Columns.Contains("SilveRSelected"))
            {
                //add the selected column
                DataColumn col = dataTable.Columns.Add("SilveRSelected", System.Type.GetType("System.Boolean"));
                col.SetOrdinal(0);

                //automatically set selected to true for each record/row
                foreach (DataRow row in dataTable.Rows)
                {
                    row["SilveRSelected"] = true;
                }
            }
        }

        private string CheckDataTable(DataTable dataTable)
        {
            string[] tabooColumnCharList = { "+", "*", "~", "`", "\\" };

            //need to check here that no taboo characters are in the dataset
            foreach (DataColumn col in dataTable.Columns)
            {
                bool illegalCharFound = false;
                string illegalCharMessage = null;

                //check column headers first...
                foreach (string s in tabooColumnCharList)
                {
                    if (col.ColumnName.Contains(s))
                    {
                        illegalCharFound = true;
                        illegalCharMessage = "The dataset contains characters in the column headers that we cannot handle (such as + * ` ~ \\)";
                        break;
                    }
                }

                if (!illegalCharFound)
                {
                    string[] tabooDataCharList = { "`" };

                    foreach (DataRow row in dataTable.Rows)
                    {
                        foreach (string s in tabooDataCharList)
                        {
                            if (row[col.ColumnName] != null && row[col.ColumnName].ToString().Contains(s))
                            {
                                illegalCharFound = true;
                                illegalCharMessage = "The dataset contains characters in the main body of the data that we cannot handle (such as `)";
                                break;
                            }
                        }
                    }
                }

                if (illegalCharFound)
                {
                    return illegalCharMessage + Environment.NewLine + "You will need to remove any of these characters from the dataset and reimport.";
                }
            }

            return null; //null means all ok
        }

        [HttpGet]
        public ActionResult SheetSelector(string fileName)
        {
            ViewBag.FileName = fileName;
            ViewBag.SheetList = TempData["TableNames"];

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SheetSelector(string sheetSelection, string fileName)
        {
            FileInfo selectedFile = new FileInfo(fileName);

            //get the correct datatable out the excel file (again)
            //read in xls file
            using (FileStream stream = selectedFile.OpenRead())
            {
                using (IExcelDataReader excelReader = ExcelReaderFactory.CreateReader(stream))
                {
                    DataSet excelDataSet = excelReader.AsDataSet(new ExcelDataSetConfiguration() { ConfigureDataTable = (_) => new ExcelDataTableConfiguration() { UseHeaderRow = true } });

                    //save to database
                    DataTable dataTable = excelDataSet.Tables[sheetSelection];
                    string message = CheckDataTable(dataTable);

                    if (message == null) //save to database
                    {
                        string datasetName = selectedFile.Name + " [" + sheetSelection + "]";
                        await SaveDatasetToDatabase(datasetName, dataTable);

                        try
                        {
                            System.IO.File.Delete(selectedFile.FullName);
                        }
                        catch { }

                        TempData["InfoMessage"] = "File imported successfully!";
                    }
                    else
                    {
                        TempData["ErrorMessage"] = message;
                    }
                }
            }

            //return to file upload/display page
            return RedirectToAction("Index");
        }

        [HttpDelete]
        public async Task<IActionResult> Destroy(int datasetID)
        {
            await repository.DeleteDataset(datasetID);

            return RedirectToAction("Index");
        }

        private CultureInfo GetCulture()
        {
            return System.Threading.Thread.CurrentThread.CurrentCulture;
        }


        public async Task<IActionResult> ViewDataTable(int datasetID)
        {
            Dataset dataset = await repository.GetDatasetByID(datasetID);
            ViewBag.TableName = dataset.DatasetNameVersion;

            // convert string to stream
            byte[] byteArray = Encoding.UTF8.GetBytes(dataset.TheData);

            using (MemoryStream stream = new MemoryStream(byteArray))
            {
                DataTable csvData = CSVHelper.CSVDataToDataTable(stream);

                csvData.TableName = dataset.DatasetID.ToString();

                Sheet sheet = new Sheet(csvData);
                return View(sheet);
            }
        }

        [HttpPost]
        [RequestSizeLimit(valueCountLimit: int.MaxValue)] // e.g. 2 GB request limit
        public async Task<JsonResult> UpdateDataset([FromBody] Sheets sheets)
        {
            Sheet sheet = sheets.sheets.Single();

            DataTable dataTable = sheet.ToDataTable();

            Dataset dataset = new Dataset();
            dataset.DatasetID = int.Parse(sheet.Name);
            string[] csvArray = dataTable.GetCSVArray();
            dataset.TheData = String.Join(Environment.NewLine, csvArray);
            dataset.DateUpdated = DateTime.Now;

            await repository.UpdateDataset(dataset);

            return Json(true);
        }
    }



    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class RequestSizeLimitAttribute : Attribute, IAuthorizationFilter, IOrderedFilter
    {
        private readonly FormOptions _formOptions;

        public RequestSizeLimitAttribute(int valueCountLimit)
        {
            _formOptions = new FormOptions()
            {
                // tip: you can use different arguments to set each properties instead of single argument
                KeyLengthLimit = valueCountLimit,
                ValueCountLimit = valueCountLimit,
                ValueLengthLimit = valueCountLimit
            };
        }

        public int Order { get; set; }

        // taken from /a/38396065
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var contextFeatures = context.HttpContext.Features;
            var formFeature = contextFeatures.Get<IFormFeature>();

            if (formFeature == null || formFeature.Form == null)
            {
                // Setting length limit when the form request is not yet being read
                contextFeatures.Set<IFormFeature>(new FormFeature(context.HttpContext.Request, _formOptions));
            }
        }
    }
}