using CsvHelper;
using System.Data;
using System.IO;
using System.Linq;

namespace SilveR.Helpers
{
    public static class CSVConverter
    {
        public static DataTable CSVDataToDataTable(Stream stream, bool useCultureSeparator)
        {
            DataTable dataTable = new DataTable();

            //use the csvreader to read in the csv data
            TextReader textReader = new StreamReader(stream);
            CsvParser parser = new CsvParser(textReader);
            if (!useCultureSeparator)
            {
                parser.Configuration.Delimiter = ","; // override any culture as already will be loaded into the db and converted into that format FIX THIS! Otherwise the parser will use the default culture
            }

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

            CsvReader csv = new CsvReader(textReader);
            if (!useCultureSeparator)
            {
                parser.Configuration.Delimiter = ","; // override any culture as already will be loaded into the db and converted into that format
            }

            csv.Read();
            csv.ReadHeader();
            while (csv.Read())
            {
                DataRow row = dataTable.NewRow();
                foreach (DataColumn column in dataTable.Columns)
                {
                    string fieldValue = csv.GetField(column.DataType, column.ColumnName).ToString().Trim();

                    if (useCultureSeparator) // then need to convert any , to .
                    {
                        fieldValue = fieldValue.Replace(',', '.');
                    }

                    row[column.ColumnName] = fieldValue;
                }

                dataTable.Rows.Add(row);
            }

            parser.Dispose();
            csv.Dispose();
            textReader.Dispose();
            stream.Dispose();

            return dataTable;
        }
    }
}