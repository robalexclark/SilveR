using System;
using System.Collections.Generic;
using System.Data;

namespace SilveR.Models
{
    public interface IDataset
    {
        ICollection<Analysis> Analysis { get; set; }
        int DatasetID { get; set; }
        string DatasetName { get; set; }
        string DatasetNameVersion { get; }
        DateTime DateUpdated { get; set; }
        string TheData { get; set; }
        int VersionNo { get; set; }

        DataTable DatasetToDataTable();
    }
}