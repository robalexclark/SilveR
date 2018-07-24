using System;

namespace SilveR.ViewModels
{
    public class DatasetViewModel
    {
        public int DatasetID { get; set; }
        public string DatasetName { get; set; }
        public int VersionNo { get; set; }
        public DateTime DateUpdated { get; set; }

        public string DatasetNameVersion
        {
            get
            {
                return this.DatasetName + " v" + this.VersionNo.ToString();
            }
        }
    }
}