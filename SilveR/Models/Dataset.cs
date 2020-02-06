using CsvHelper;
using CsvHelper.Configuration;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Globalization;
using System.IO;
using System.Text;

namespace SilveR.Models
{
    public partial class Dataset : IDataset
    {
        public Dataset()
        {
            Analysis = new HashSet<Analysis>();
        }

        [Key]
        public int DatasetID { get; set; }

        [Required]
        [StringLength(50)]
        public string DatasetName { get; set; }

        public int VersionNo { get; set; }

        [Required]
        public string TheData { get; set; }

        public DateTime DateUpdated { get; set; }

        public ICollection<Analysis> Analysis { get; set; }

        [NotMapped]
        public string DatasetNameVersion
        {
            get
            {
                return this.DatasetName + " v" + this.VersionNo.ToString();
            }
        }


        public DataTable DatasetToDataTable()
        {
            DataTable dataTable = new DataTable();

            // convert string to stream
            byte[] byteArray = Encoding.UTF8.GetBytes(this.TheData);

            using (MemoryStream stream = new MemoryStream(byteArray))
            {
                using (TextReader textReader = new StreamReader(stream))
                {
                    using (CsvParser parser = new CsvParser(textReader, CultureInfo.InvariantCulture))
                    {
                        string[] headerRow = parser.Read();
                        foreach (string h in headerRow)
                        {
                            dataTable.Columns.Add(new DataColumn(h));
                        }
                    }
                }
            }

            using (MemoryStream stream = new MemoryStream(byteArray))
            {
                using (TextReader textReader = new StreamReader(stream))
                {
                    using (CsvReader csv = new CsvReader(textReader, CultureInfo.InvariantCulture))
                    {
                        csv.Read();
                        csv.ReadHeader();
                        while (csv.Read())
                        {
                            if (csv["SilveRSelected"].ToString() == "True")
                            {
                                DataRow row = dataTable.NewRow();
                                foreach (DataColumn column in dataTable.Columns)
                                {
                                    row[column.ColumnName] = csv.GetField(column.DataType, column.ColumnName);
                                }
                                dataTable.Rows.Add(row);
                            }
                        }
                    }
                }
            }

            return dataTable;
        }
    }
}