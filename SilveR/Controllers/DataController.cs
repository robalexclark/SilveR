using ExcelDataReader;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using SilveR.Helpers;
using SilveR.Models;
using SilveR.ViewModels;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace SilveR.Controllers
{
    public class DataController : Controller
    {
        private readonly ISilveRRepository repository;

        public DataController(ISilveRRepository repository)
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
            IEnumerable<DatasetViewModel> datasets = await repository.GetDatasetViewModels();
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
            if (files.Any() == false || files.Single().Length == 0)
            {
                ViewBag.ErrorMessage = "File failed to load, please try again";
                return View();
            }
            else
            {
                IFormFile formFile = files.Single();
                ContentDispositionHeaderValue fileContent = ContentDispositionHeaderValue.Parse(formFile.ContentDisposition);
                string fileName = Path.GetFileName(fileContent.FileName.ToString().Trim('"'));

                string filePath = Path.Combine(Path.GetTempPath(), fileName);

                if (System.IO.File.Exists(filePath))
                    System.IO.File.Delete(filePath);

                using (FileStream fileStream = new FileStream(filePath, FileMode.Create))
                {
                    formFile.CopyTo(fileStream);
                }

                return await LoadFile(new FileInfo(filePath));
            }
        }

        private async Task<IActionResult> LoadFile(FileInfo selectedFile)
        {
            if (selectedFile.Extension.ToUpper() == ".CSV") //not necessary as already filtering...
            {
                //use the CSVReader to read in the data
                string message;
                DataTable dataTable = CSVConverter.CSVDataToDataTable(selectedFile.OpenRead(), System.Threading.Thread.CurrentThread.CurrentCulture);

                if (dataTable == null) //then failed to be read
                {
                    message = "The CSV data needs to be in the correct format.";
                }
                else //got datatable but need to check
                {
                    message = dataTable.CheckDataTable();
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
            else if (selectedFile.Extension.ToUpper() == ".XLS" || selectedFile.Extension.ToUpper() == ".XLSX") //excel
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
                        else if (excelDataSet.Tables.Count == 1) //one excel sheet
                        {
                            DataTable dataTable = excelDataSet.Tables[0];
                            string message = dataTable.CheckDataTable();

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
            else
                throw new Exception("File format not recognised.");
        }

        private async Task SaveDatasetToDatabase(string fileName, DataTable dataTable)
        {
            //get last version no based on existing dataset names
            int lastVersionNo = await repository.GetLastVersionNumberForDataset(fileName);
            Dataset dataset = dataTable.GetDataset(fileName, lastVersionNo);

            await repository.CreateDataset(dataset);
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
            using (FileStream stream = selectedFile.OpenRead())
            {
                using (IExcelDataReader excelReader = ExcelReaderFactory.CreateReader(stream))
                {
                    DataSet excelDataSet = excelReader.AsDataSet(new ExcelDataSetConfiguration() { ConfigureDataTable = (_) => new ExcelDataTableConfiguration() { UseHeaderRow = true } });

                    //save to database
                    DataTable dataTable = excelDataSet.Tables[sheetSelection];
                    string message = dataTable.CheckDataTable();

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
        public async Task<JsonResult> Destroy(int datasetID)
        {
            await repository.DeleteDataset(datasetID);

            return Json(true);
        }

        public async Task<IActionResult> ViewDataTable(int datasetID)
        {
            Dataset dataset = await repository.GetDatasetByID(datasetID);
            ViewBag.TableName = dataset.DatasetNameVersion;

            // convert string to stream
            byte[] byteArray = Encoding.UTF8.GetBytes(dataset.TheData);

            using (MemoryStream stream = new MemoryStream(byteArray))
            {
                DataTable dataTable = CSVConverter.CSVDataToDataTable(stream);
                dataTable.TableName = dataset.DatasetID.ToString();

                Sheet sheet = new Sheet(dataTable);
                return View(sheet);
            }
        }

        [HttpPost]
        [RequestSizeLimit(valueCountLimit: int.MaxValue)] // e.g. 2 GB request limit
        public async Task<JsonResult> UpdateDataset([FromBody] Sheets sheets)
        {
            Sheet sheet = sheets.sheets.Single();

            try
            {
                DataTable dataTable = sheet.ToDataTable();

                string message = dataTable.CheckDataTable();

                if (message == null) //then all ok
                {
                    Dataset dataset = new Dataset();
                    dataset.DatasetID = int.Parse(sheet.Name);
                    string[] csvArray = dataTable.GetCSVArray();
                    dataset.TheData = String.Join(Environment.NewLine, csvArray);
                    dataset.DateUpdated = DateTime.Now;

                    await repository.UpdateDataset(dataset);
                }
                else
                {
                    return Json(message);
                }
            }
            catch (Exception ex)
            {
                return Json(ex.Message);
            }

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