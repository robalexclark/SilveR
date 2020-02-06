using CsvHelper;
using System;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;

namespace SilveR.Helpers
{
    public static class CSVConverter
    {
        public static DataTable CSVDataToDataTable(Stream stream, CultureInfo cultureInfo = null)
        {
            if (cultureInfo == null)
            {
                cultureInfo = CultureInfo.InvariantCulture;
            }
            else //check that its not invariant!
            {
                if (cultureInfo == CultureInfo.InvariantCulture)
                {
                    throw new ArgumentException("If cultureInfo is provided it must not be InvariantCulture");
                }
            }

            if (System.Threading.Thread.CurrentThread.CurrentCulture != CultureInfo.InvariantCulture)
            {
                throw new ArgumentException("This method can only be executed when the underlying thread is an InvariantCulture");
            }

            DataTable dataTable = new DataTable();

            //use the csvreader to read in the csv data
            TextReader textReader = new StreamReader(stream);
            CsvParser parser = new CsvParser(textReader, cultureInfo);

            string[] headerRow = parser.Read();
            for (int i = 0; i < headerRow.Count(); i++)
            {
                DataColumn newCol;

                if (headerRow[i] == "SilveRSelected")
                {
                    newCol = new DataColumn(headerRow[i], System.Type.GetType("System.Boolean"));
                }
                else
                {
                    newCol = new DataColumn(headerRow[i], System.Type.GetType("System.String"));
                }

                dataTable.Columns.Add(newCol);
            }


            stream.Seek(0, SeekOrigin.Begin);
            textReader = new StreamReader(stream);

            CsvReader csvReader = new CsvReader(textReader, cultureInfo);

            csvReader.Read();
            csvReader.ReadHeader();
            while (csvReader.Read())
            {
                DataRow row = dataTable.NewRow();
                foreach (DataColumn column in dataTable.Columns)
                {
                    string fieldValue = csvReader.GetField(column.DataType, column.ColumnName).ToString().Trim();

                    if (cultureInfo != null)
                    {
                        fieldValue = fieldValue.Replace(',', cultureInfo.NumberFormat.NumberDecimalSeparator.Single());
                    }

                    row[column.ColumnName] = fieldValue;
                }

                dataTable.Rows.Add(row);
            }

            parser.Dispose();
            csvReader.Dispose();
            textReader.Dispose();
            stream.Dispose();

            return dataTable;
        }
    }
}