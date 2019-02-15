using SilveR.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text;

namespace SilveR.Helpers
{
    public static class DataTableExtension
    {
        public static void CleanUpDataTable(this DataTable dataTable)
        {
            foreach (DataColumn dc in dataTable.Columns)
            {
                dc.ColumnName = dc.ColumnName.Trim();
            }

            CultureInfo culture = System.Threading.Thread.CurrentThread.CurrentCulture;
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

        public static void AddSelectedColumn(this DataTable dataTable)
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

        public static string CheckDataTable(this DataTable dataTable)
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

        public static void TransformColumn(this DataTable data, string column, string transformation)
        {
            if (transformation == "Rank")
            {
                List<double> colData = new List<double>(); // used to hold the values from the column so they can be sorted

                foreach (DataRow r in data.Rows)
                {
                    double val; //get the value
                    bool valOK = Double.TryParse(r[column].ToString(), out val);

                    if (valOK) //then it is numeric 
                    {
                        colData.Add(val);
                    }
                }

                //sort the data...
                colData.Sort();

                //go through and give basic ranks, adding to the ranking list (which contains keyvalue pairs for storing the rank and the original value)
                List<KeyValuePair<int, double>> rankingList = new List<KeyValuePair<int, double>>();
                int rank = 1;
                foreach (double v in colData)
                {
                    rankingList.Add(new KeyValuePair<int, double>(rank, v)); //add in the rank along with the value
                    rank++;
                }

                //create another list which contains average ranks
                List<KeyValuePair<double, double>> averageRankingList = new List<KeyValuePair<double, double>>();
                foreach (KeyValuePair<int, double> kp in rankingList)
                {
                    var sameValuesRanks = rankingList.Where(v => v.Value == kp.Value); //find all the numbers with the same value (might only be one!), and copy into iEnumerable...
                    double averageRank = sameValuesRanks.Average(v => v.Key); //calculate the average rank... (will be same rank if only 1 value!)
                    averageRankingList.Add(new KeyValuePair<double, double>(averageRank, kp.Value)); //...add into the new list
                }

                //go through each row in dataset replacing the original value with the rank
                foreach (DataRow r in data.Rows)
                {
                    double val;
                    bool valOK = Double.TryParse(r[column].ToString(), out val);

                    if (valOK)
                    {
                        //find the item out of the average rank list that matches the value in the current row...
                        KeyValuePair<double, double> kp = averageRankingList.First(v => v.Value == val);
                        r[column] = kp.Key; //..and replace it with the rank
                    }
                }
            }
            else //all transformations other than Rank
            {
                foreach (DataRow r in data.Rows)
                {
                    double val; //get the value
                    bool valOK = Double.TryParse(r[column].ToString(), out val);

                    if (valOK && transformation != "None") //if reponse value is number (and actually doing a transformation)
                    {
                        switch (transformation) // do the selected transform
                        {
                            case "Log10":
                                if (val > 0)
                                    r[column] = Math.Log10(val);
                                else
                                    r[column] = null;

                                break;
                            case "Loge":
                                if (val > 0)
                                    r[column] = Math.Log(val);
                                else
                                    r[column] = null;

                                break;
                            case "Square Root":
                                if (val >= 0)
                                    r[column] = Math.Sqrt(val);
                                else
                                    r[column] = null;

                                break;
                            case "ArcSine":
                                if (val >= 0 && val <= 1)
                                    r[column] = Math.Asin(Math.Sqrt(val));
                                else
                                    r[column] = null;

                                break;
                        }
                    }
                }
            }
        }

        public static void RemoveBlankRow(this DataTable dtNew, string columnToCheck)
        {
            for (int i = dtNew.Rows.Count - 1; i >= 0; i--)
            {
                DataRow theRow = dtNew.Rows[i];

                if (String.IsNullOrEmpty(theRow[columnToCheck].ToString()))
                {
                    dtNew.Rows.Remove(dtNew.Rows[i]);
                }
            }
        }

        public static bool CheckIsNumeric(this DataTable dataTable, string column)
        {
            if (!String.IsNullOrEmpty(column))
            {
                foreach (DataRow row in dataTable.Rows)
                {
                    bool isNumeric = Double.TryParse(row[column].ToString(), out double number);

                    if (!isNumeric && !String.IsNullOrEmpty(row[column].ToString())) return false;
                }
            }

            return true;
        }

        public static string[] GetCSVArray(this DataTable dataTable)
        {
            List<string> lines = new List<string>(); //holds the output

            //get headers first...
            StringBuilder headerOut = new StringBuilder();
            bool firstHeader = true;

            foreach (DataColumn col in dataTable.Columns)
            {
                if (!firstHeader)
                {
                    headerOut = headerOut.Append(",");
                }

                headerOut = headerOut.Append(col.ColumnName.Trim());

                firstHeader = false;
            }

            //...and add it to the lines...
            lines.Add(headerOut.ToString());

            //...then go through each row and add each one to the lines...
            foreach (DataRow row in dataTable.Rows)
            {
                StringBuilder rowOut = new StringBuilder();

                bool firstColumn = true;

                foreach (DataColumn col in dataTable.Columns)
                {
                    if (!firstColumn)
                    {
                        rowOut = rowOut.Append(",");
                    }

                    string value = row[col.ColumnName].ToString();
                    rowOut = rowOut.Append(value);

                    firstColumn = false;
                }

                lines.Add(rowOut.ToString());
            }

            return lines.ToArray();
        }

        public static IEnumerable<string> GetVariableNames(this DataTable dataTable)
        {
            List<string> variables = new List<string>();
            foreach (DataColumn col in dataTable.Columns)
            {
                if (col.ColumnName != "SilveRSelected")
                {
                    variables.Add(col.ColumnName);
                }
            }

            return variables;
        }

        public static IEnumerable<string> GetLevels(this DataTable dataTable, string column)
        {
            return GetValues(dataTable, column).Distinct();
        }

        public static IEnumerable<string> GetValues(this DataTable dataTable, string column)
        {
            //Get a list of the levels in a selected column
            List<string> levels = new List<string>();

            foreach (DataRow row in dataTable.Rows)
            {
                if (!String.IsNullOrEmpty(row[column].ToString()))
                {
                    levels.Add(row[column].ToString());
                }
            }

            return levels;
        }

        public static DataTable CopyForExport(this DataTable dataTable)
        {
            DataTable dtNew = dataTable.Copy();
            dtNew.TableName = "ExportData";

            //first remove any rows that are not selected
            for (int i = 0; i < dtNew.Rows.Count; i++)
            {
                if (dtNew.Rows[i]["SilveRSelected"].ToString() != "True")
                {
                    dtNew.Rows[i].Delete();
                }
            }

            dtNew.Columns.Remove("SilveRSelected");

            return dtNew;
        }

        public static Dataset GetDataset(this DataTable dataTable, string fileName, int lastVersionNo)
        {
            //clean up the datatable, trimming spaces, and ensuring that decimal seperator is a .
            dataTable.CleanUpDataTable();

            //add the selected column to the dataset, setting all rows to "true"
            dataTable.AddSelectedColumn();

            //create entity and save...
            Dataset dataset = new Dataset();
            string[] csvArray = dataTable.GetCSVArray();
            dataset.TheData = String.Join(Environment.NewLine, csvArray);
            dataset.DatasetName = fileName;
            dataset.VersionNo = lastVersionNo + 1;
            dataset.DateUpdated = DateTime.Now;

            return dataset;
        }
    }
}