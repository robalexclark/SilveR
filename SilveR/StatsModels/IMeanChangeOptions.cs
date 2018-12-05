using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SilveR.StatsModels
{
    interface IMeanChangeOptions
    {
        string AbsoluteChange { get; set; }
        ChangeTypeOption ChangeType { get; set; }
        string PercentChange { get; set; }
    }
}