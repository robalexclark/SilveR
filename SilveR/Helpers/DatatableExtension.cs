using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace SilveR.Helpers
{
    public static class DataTableExtensionMethods
    {
        public static void TrimAllDataInDataTable(this DataTable dataTable)
        {
            foreach (DataColumn c in dataTable.Columns)
            {
                foreach (DataRow r in dataTable.Rows)
                {
                    string newValue = r[c.ColumnName].ToString().Trim();
                    r[c.ColumnName] = newValue;
                }
            }
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

        public static void RemoveBlankRow(this DataTable dtNew, string blankColumn)
        {
            if (!String.IsNullOrEmpty(blankColumn))
            {
                for (int i = dtNew.Rows.Count - 1; i >= 0; i--)
                {
                    DataRow theRow = dtNew.Rows[i];

                    if (String.IsNullOrEmpty(theRow[blankColumn].ToString()))
                    {
                        dtNew.Rows.Remove(dtNew.Rows[i]);
                    }
                }
            }
        }

        public static bool CheckIsNumeric(this DataTable dtNew, string column)
        {
            if (!String.IsNullOrEmpty(column))
            {
                foreach (DataRow row in dtNew.Rows)
                {
                    double number;
                    bool isNumeric = Double.TryParse(row[column].ToString(), out number);

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

        public static List<string> GetLevels(this DataTable dataTable, string column)
        {
            //Get a list of the distinct levels in a selected column
            List<string> distinctlevels = new List<string>();

            if (String.IsNullOrEmpty(column))
                return distinctlevels; //i.e. empty list

            List<string> levels = new List<string>();

            foreach (DataRow row in dataTable.Rows)
            {
                if (!String.IsNullOrEmpty(row[column].ToString()))
                    levels.Add(row[column].ToString());
            }

            IEnumerable<string> levelsAsIEnumerable = levels.Distinct();

            return levelsAsIEnumerable.ToList();
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

        public static bool ColumnIsNumeric(this DataTable dataTable, string column)
        {
            if (!String.IsNullOrEmpty(column))
            {
                foreach (DataRow row in dataTable.Rows)
                {
                    double number;
                    bool isNumeric = Double.TryParse(row[column].ToString(), out number);

                    if (!isNumeric && !String.IsNullOrEmpty(row[column].ToString())) return false;
                }
            }
            return true;
        }
    }
}