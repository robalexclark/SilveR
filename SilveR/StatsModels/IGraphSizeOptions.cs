using System;

namespace SilveR.StatsModels
{
    public interface IGraphSizeOptions
    {
        PlottingRangeTypeOption PlottingRangeType { get; set; }
        Nullable<int> PowerFrom { get; set; }
        Nullable<int> PowerTo { get; set; }
        Nullable<int> SampleSizeFrom { get; set; }
        Nullable<int> SampleSizeTo { get; set; }
    }
}