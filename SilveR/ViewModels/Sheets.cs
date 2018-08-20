using System;
using System.Collections.Generic;
using System.Data;

namespace SilveR.ViewModels
{
    public class Sheets
    {
        public List<Sheet> sheets { get; set; } = new List<Sheet>();
    }


    public class Sheet
    {
        public List<Row> Rows { get; set; } = new List<Row>();

        public string Name { get; set; }

        public Sheet()
        {
        }

        public Sheet(DataTable csvData)
        {
            Name = csvData.TableName;

            Row headerRow = new Row();

            foreach (DataColumn c in csvData.Columns)
            {
                Cell cell = new Cell();
                cell.Value = c.ToString();
                cell.Bold = true;

                headerRow.Cells.Add(cell);
            }
            Rows.Add(headerRow);


            foreach (DataRow dataRow in csvData.Rows)
            {
                Row row = new Row();
                row.Cells = new List<Cell>();

                foreach (var c in dataRow.ItemArray)
                {
                    Cell cell = new Cell();

                    double doubleVal;
                    bool isNumeric = Double.TryParse(c.ToString(), out doubleVal);

                    if (isNumeric)
                        cell.Value = doubleVal;
                    else
                        cell.Value = c.ToString();

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

                        if (cell.Value.ToString() == "SilveRSelected")
                        {
                            dataColumn = new DataColumn("SilveRSelected", System.Type.GetType("System.Boolean"));
                        }
                        else
                        {
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
                        if (i == 0)
                        {
                            dataRow[i] = Boolean.Parse(row.Cells[0].Value.ToString());
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
        //public int index { get; set; }
        public object Value { get; set; }

        public bool Bold { get; set; }

        //public string Format { get; set; }
    }
}