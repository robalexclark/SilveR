using ExcelDataReader;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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

            await repository.SaveDataset(dataset);
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

            //check dataset owner
            //if (dataset.AspNetUserID != userManager.GetUserId(User))
            //{
            //    RedirectToAction("Index");
            //}

            // convert string to stream
            byte[] byteArray = Encoding.UTF8.GetBytes(dataset.TheData);

            using (MemoryStream stream = new MemoryStream(byteArray))
            {
                DataTable csvData = CSVHelper.CSVDataToDataTable(stream);

                //foreach (DataColumn col in csvData.Columns)
                //{
                //    col.Caption = col.ColumnName;
                //    col.ColumnName = col.ColumnName.Replace(" ", String.Empty);
                //}

                DataColumn primaryKey = new DataColumn("TempRowID");
                csvData.Columns.Add(primaryKey);
                for (int i = 0; i < csvData.Rows.Count; i++)
                {
                    csvData.Rows[i]["TempRowID"] = i;
                }

                csvData.PrimaryKey = new DataColumn[] { csvData.Columns["TempRowID"] };

                csvData.TableName = dataset.DatasetName;
                csvData.ExtendedProperties.Add("DatasetID", dataset.DatasetID);

                return View(Json( csvData));
            }
        }

        //public ActionResult Read([DataSourceRequest] DataSourceRequest request)
        //{
        //    DataTable csvData = (DataTable)TempData["Dataset"];

        //    TempData["Dataset"] = csvData;

        //    return Json(csvData.ToDataSourceResult(request));
        //}

        //public async Task<IActionResult> Update([DataSourceRequest] DataSourceRequest request, FormCollection formCollection)
        //{
        //    DataTable dataTable = (DataTable)TempData["Dataset"];

        //    string primaryKeyValue = formCollection["TempRowID"];

        //    DataColumnCollection columns = dataTable.Columns;
        //    DataRow rowToUpdate = dataTable.Rows.Find(primaryKeyValue);

        //    foreach (var fc in formCollection)
        //    {
        //        if (columns.Contains(fc.Key))
        //        {
        //            if (fc.Key == "SilveRSelected")
        //            {
        //                rowToUpdate[fc.Key] = (fc.Value == "true");
        //            }
        //            else
        //            {
        //                rowToUpdate[fc.Key] = fc.Value;
        //            }
        //        }
        //    }

        //    //DO SAVE
        //    int datasetID = int.Parse(dataTable.ExtendedProperties["DatasetID"].ToString());

        //    DataTable dataTableToUpdate = dataTable.Copy();
        //    dataTableToUpdate.PrimaryKey = null;
        //    dataTableToUpdate.Columns.Remove("TempRowID");

        //    string message = CheckDataTable(dataTableToUpdate);
        //    if (message != null)
        //    {
        //        ViewBag.ErrorMessage = message;
        //        return View("ViewDataTable", dataTable);
        //    }
        //    else
        //    {
        //        string[] csvArray = dataTableToUpdate.GetCSVArray();

        //        string csvData = String.Join(Environment.NewLine, csvArray);
        //        await repository.UpdateDataset(datasetID, csvData);
        //    }

        //    return Json(new object());
        //}
    }
}