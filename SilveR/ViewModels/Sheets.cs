using System;
using System.Collections.Generic;
using System.Data;

namespace SilveR.ViewModels
{
    public class Sheets
    {
        public List<Sheet> sheets { get; } = new List<Sheet>();
    }


    public class Sheet
    {
        public List<Row> Rows { get; } = new List<Row>();

        public string Name { get; set; } //Name is actually the datasetID

        public Sheet()
        {
        }

        public Sheet(DataTable dataTable)
        {
            Name = dataTable.TableName;

            Row headerRow = new Row();

            //header row
            foreach (DataColumn c in dataTable.Columns)
            {
                Cell cell = new Cell();

                if (c.ToString() != "SilveRSelected")
                {
                    cell.Value = c.ToString();
                }
                else //else leave blank
                {
                    cell.Enable = false;
                }

                cell.Bold = true;

                headerRow.Cells.Add(cell);
            }
            Rows.Add(headerRow);


            foreach (DataRow dataRow in dataTable.Rows)
            {
                Row row = new Row();
                row.Cells = new List<Cell>();

                bool silverSelectedColumn = true;
                foreach (var c in dataRow.ItemArray)
                {
                    Cell cell = new Cell();

                    if (silverSelectedColumn)
                    {
                        cell.Validation = new Validation();
                        cell.Validation.DataType = "list";
                        cell.Validation.From = "{True, False}";
                        cell.Validation.ShowButton = true;
                        cell.Validation.Type = "reject";
                        cell.Validation.ComparerType = "list";

                        cell.Value = c.ToString();

                        silverSelectedColumn = false;
                    }
                    else
                    {
                        double doubleVal;
                        bool isNumeric = Double.TryParse(c.ToString(), out doubleVal);

                        if (isNumeric)
                            cell.Value = doubleVal;
                        else
                            cell.Value = c.ToString();
                    }

                    row.Cells.Add(cell);
                }

                Rows.Add(row);
            }
        }

        public DataTable ToDataTable()
        {
            DataTable dataTable = new DataTable();

            bool headerRow = true;
            foreach (Row row in this.Rows)
            {
                if (headerRow)
                {
                    foreach (Cell cell in row.Cells)
                    {
                        DataColumn dataColumn;

                        if (cell.Value == null && cell.Enable == false) //then its the SilveRSelected header cell
                        {
                            dataColumn = new DataColumn("SilveRSelected", System.Type.GetType("System.Boolean"));
                        }
                        else
                        {
                            if (cell.Value == null || String.IsNullOrWhiteSpace(cell.Value.ToString()))
                                throw new Exception("Header cannot be empty");

                            dataColumn = new DataColumn(cell.Value.ToString());
                        }

                        dataTable.Columns.Add(dataColumn);
                    }

                    headerRow = false;
                }
                else
                {
                    DataRow dataRow = dataTable.NewRow();

                    for (int i = 0; i < row.Cells.Count; i++)
                    {
                        if (i == 0) //then its the SilverSelected column
                        {
                            bool isSilverSelected;
                            bool parsedOK = Boolean.TryParse(row.Cells[0].Value.ToString(), out isSilverSelected);

                            if (parsedOK)
                            {
                                dataRow[i] = isSilverSelected;
                            }
                            else //not parsed but default to selected = true
                            {
                                dataRow[i] = true;
                            }
                        }
                        else
                        {
                            dataRow[i] = row.Cells[i].Value;
                        }
                    }

                    dataTable.Rows.Add(dataRow);
                }
            }

            return dataTable;
        }
    }

    public class Row
    {
        public List<Cell> Cells { get; set; } = new List<Cell>();
    }

    public class Cell
    {
        public object Value { get; set; }

        public bool Bold { get; set; }

        public Validation Validation { get; set; }

        public bool Enable { get; set; } = true;
    }

    public class Validation
    {
        public string DataType { get; set; }
        public string ComparerType { get; set; }
        public string From { get; set; }
        public bool ShowButton { get; set; }
        public bool AllowNulls { get; set; }
        public string Type { get; set; }
    }
}