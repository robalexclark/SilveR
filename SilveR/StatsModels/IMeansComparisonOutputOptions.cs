namespace SilveR.StatsModels
{
    public interface IMeansComparisonOutputOptions
    {
        string AbsoluteChange { get; set; }
        ChangeTypeOption ChangeType { get; set; }
        string GraphTitle { get; set; }
        string PercentChange { get; set; }
        PlottingRangeTypeOption PlottingRangeType { get; set; }
        string PowerFrom { get; set; }
        string PowerTo { get; set; }
        string SampleSizeFrom { get; set; }
        string SampleSizeTo { get; set; }
    }
}