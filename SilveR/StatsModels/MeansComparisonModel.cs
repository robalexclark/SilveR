using SilveR.Helpers;
using SilveR.Models;
using SilveR.Validators;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq;
using System.Text;

namespace SilveR.StatsModels
{
    public class MeansComparisonModel : AnalysisModelBase
    {
        [ValidateGroupMean]
        [Range(0, double.MaxValue)]
        [DisplayName("Group mean")]
        public string GroupMean { get; set; }

        [ValidateStandardDeviation]
        [Range(0, double.MaxValue)]
        [DisplayName("Standard deviation")]
        public string StandardDeviation { get; set; }

        [ValidateVariance]
        [Range(0, double.MaxValue)]
        [DisplayName("Variance")]
        public string Variance { get; set; }

        public enum ValueTypeOption { Supplied = 0, Dataset = 1 }
        private ValueTypeOption valueType = ValueTypeOption.Supplied;
        public ValueTypeOption ValueType
        {
            get { return valueType; }
            set
            {
                //if (valueType != value)
                //{
                    valueType = value;
                //}
            }
        }

        [ValidateResponseOrTreatment]
        [CheckUsedOnceOnly]
        [DisplayName("Response")]
        public string Response { get; set; }

        //[ValidateResponseOrTreatment]
        [CheckUsedOnceOnly]
        public string Treatment { get; set; }

        [DisplayName("Significance level")]
        public string Significance { get; set; } = "0.05";

        public IEnumerable<string> SignificancesList
        {
            get { return new List<string>() { "0.1", "0.05", "0.01", "0.001" }; }
        }

        [ValidateControlGroup]
        [DisplayName("Control group")]
        public string ControlGroup { get; set; }

        public enum ChangeTypeOption { Percent = 0, Absolute = 1 }
        private ChangeTypeOption changeType = ChangeTypeOption.Percent;
        public ChangeTypeOption ChangeType
        {
            get { return changeType; }
            set
            {
                //if (changeType != value)
                //{
                    changeType = value;
                //}
            }
        }

        [ValidatePercentChanges]
        [DisplayName("Percent change")]
        public string PercentChange { get; set; }// = String.Empty;

        [ValidateAbsoluteChanges]
        [DisplayName("Absolute change")]
        public string AbsoluteChange { get; set; }// = String.Empty;

        public enum PlottingRangeTypeOption { SampleSize = 0, Power = 1 }
        private PlottingRangeTypeOption plottingRangeType = PlottingRangeTypeOption.SampleSize;
        public PlottingRangeTypeOption PlottingRangeType
        {
            get { return plottingRangeType; }
            set
            {
                //if (plottingRangeType != value)
                //{
                    plottingRangeType = value;
                //}
            }
        }


        [ValidateSampleSizeFrom]
        [DisplayName("Sample size from")]
        public string SampleSizeFrom { get; set; } = "6";

        [ValidateSampleSizeTo]
        [DisplayName("Sample size to")]
        public string SampleSizeTo { get; set; } = "15";

        [ValidateCustomFrom]
        [DisplayName("Power from")]
        public string PowerFrom { get; set; } = "70";

        [ValidateCustomTo]
        [DisplayName("Power to")]
        public string PowerTo { get; set; } = "90";

        [DisplayName("Graph title")]
        public string GraphTitle { get; set; }


        public MeansComparisonModel() : this(null) { }

        public MeansComparisonModel(IDataset dataset)
            :base(dataset, "MeansComparison") { }

        public override ValidationInfo Validate()
        {
            MeansComparisonValidator meansComparisonValidator = new MeansComparisonValidator(this);
            return meansComparisonValidator.Validate();
        }

        public override string[] ExportData()
        {
            DataTable dtNew = DataTable.CopyForExport();

            //Get the response, treatment and covariate columns by removing all other columns from the new datatable
            foreach (string col in this.DataTable.GetVariableNames())
            {
                if (Response != col && Treatment != col)
                {
                    dtNew.Columns.Remove(col);
                }
            }

            //ensure that all data is trimmed
            //dtNew.TrimAllDataInDataTable();

            //if the response is blank then remove that row
            dtNew.RemoveBlankRow(Response);


            string[] csvArray = dtNew.GetCSVArray();

            //fix any columns with illegal chars here (at the end)
            ArgumentFormatter argFormatter = new ArgumentFormatter();
            csvArray[0] = argFormatter.ConvertIllegalCharacters(csvArray[0]);

            return csvArray;
        }

        public override IEnumerable<Argument> GetArguments()
        {
            List<Argument> args = new List<Argument>();

            args.Add(ArgumentHelper.ArgumentFactory(nameof(GroupMean), GroupMean));
            args.Add(ArgumentHelper.ArgumentFactory(nameof(StandardDeviation), StandardDeviation));
            args.Add(ArgumentHelper.ArgumentFactory(nameof(Variance), Variance));
            args.Add(ArgumentHelper.ArgumentFactory(nameof(ValueType), ValueType.ToString()));
            args.Add(ArgumentHelper.ArgumentFactory(nameof(Response), Response));
            args.Add(ArgumentHelper.ArgumentFactory(nameof(Treatment), Treatment));
            args.Add(ArgumentHelper.ArgumentFactory(nameof(Significance), Significance));
            args.Add(ArgumentHelper.ArgumentFactory(nameof(ControlGroup), ControlGroup));
            args.Add(ArgumentHelper.ArgumentFactory(nameof(ChangeType), ChangeType.ToString()));
            args.Add(ArgumentHelper.ArgumentFactory(nameof(PercentChange), PercentChange));
            args.Add(ArgumentHelper.ArgumentFactory(nameof(AbsoluteChange), AbsoluteChange));
            args.Add(ArgumentHelper.ArgumentFactory(nameof(PlottingRangeType), PlottingRangeType.ToString()));
            args.Add(ArgumentHelper.ArgumentFactory(nameof(SampleSizeFrom), SampleSizeFrom));
            args.Add(ArgumentHelper.ArgumentFactory(nameof(SampleSizeTo), SampleSizeTo));
            args.Add(ArgumentHelper.ArgumentFactory(nameof(PowerFrom), PowerFrom));
            args.Add(ArgumentHelper.ArgumentFactory(nameof(PowerTo), PowerTo));
            args.Add(ArgumentHelper.ArgumentFactory(nameof(GraphTitle), GraphTitle));

            return args;
        }

        public override void LoadArguments(IEnumerable<Argument> arguments)
        {
            ArgumentHelper argHelper = new ArgumentHelper(arguments);

            this.GroupMean = argHelper.LoadStringArgument(nameof(GroupMean));
            this.StandardDeviation = argHelper.LoadStringArgument(nameof(StandardDeviation));
            this.Variance = argHelper.LoadStringArgument(nameof(Variance));
            this.ValueType = (ValueTypeOption)Enum.Parse(typeof(ValueTypeOption), argHelper.LoadStringArgument(nameof(ValueType)), true);
            this.Response = argHelper.LoadStringArgument(nameof(Response));
            this.Treatment = argHelper.LoadStringArgument(nameof(Treatment));
            this.Significance = argHelper.LoadStringArgument(nameof(Significance));
            this.ControlGroup = argHelper.LoadStringArgument(nameof(ControlGroup));
            this.ChangeType = (ChangeTypeOption)Enum.Parse(typeof(ChangeTypeOption), argHelper.LoadStringArgument(nameof(ChangeType)), true);
            this.PercentChange = argHelper.LoadStringArgument(nameof(PercentChange));
            this.AbsoluteChange = argHelper.LoadStringArgument(nameof(AbsoluteChange));
            this.PlottingRangeType = (PlottingRangeTypeOption)Enum.Parse(typeof(PlottingRangeTypeOption), argHelper.LoadStringArgument(nameof(PlottingRangeType)), true);
            this.SampleSizeFrom = argHelper.LoadStringArgument(nameof(SampleSizeFrom));
            this.SampleSizeTo = argHelper.LoadStringArgument(nameof(SampleSizeTo));
            this.PowerFrom = argHelper.LoadStringArgument(nameof(PowerFrom));
            this.PowerTo = argHelper.LoadStringArgument(nameof(PowerTo));
            this.GraphTitle = argHelper.LoadStringArgument(nameof(GraphTitle));
        }

        public override string GetCommandLineArguments()
        {
            ArgumentFormatter argFormatter = new ArgumentFormatter();
            StringBuilder arguments = new StringBuilder();

            if (ValueType == ValueTypeOption.Supplied)
            {
                arguments.Append(" " + "SuppliedValues"); //4
                arguments.Append(" " + argFormatter.GetFormattedArgument(GroupMean, false)); //5

                if (!String.IsNullOrWhiteSpace(StandardDeviation))
                {
                    arguments.Append(" " + "StandardDeviation"); //6
                    arguments.Append(" " + argFormatter.GetFormattedArgument(StandardDeviation, false)); //7
                }
                else if (!String.IsNullOrEmpty(Variance))
                {
                    arguments.Append(" " + "Variance"); //6
                    arguments.Append(" " + argFormatter.GetFormattedArgument(Variance, false)); //7
                }
                else
                    throw new InvalidOperationException("no stdev or variance supplied!");
            }
            else if (ValueType == ValueTypeOption.Dataset)
            {
                arguments.Append(" " + "DatasetValues"); //4
                arguments.Append(" " + argFormatter.GetFormattedArgument(Response, true)); //5

                arguments.Append(" " + argFormatter.GetFormattedArgument(Treatment, true));

                arguments.Append(" " + argFormatter.GetFormattedArgument(ControlGroup, true)); //7
            }

            arguments.Append(" " + argFormatter.GetFormattedArgument(Significance, false)); //8

            if (ChangeType == ChangeTypeOption.Percent)
            {
                arguments.Append(" " + "Percent"); //9
                arguments.Append(" " + argFormatter.GetFormattedArgument(PercentChange, false)); //10
            }
            else
            {
                arguments.Append(" " + "Absolute"); //9
                arguments.Append(" " + argFormatter.GetFormattedArgument(AbsoluteChange, false)); //10
            }

            if (PlottingRangeType == PlottingRangeTypeOption.SampleSize)
            {
                arguments.Append(" " + "SampleSize"); //11
                arguments.Append(" " + argFormatter.GetFormattedArgument(SampleSizeFrom, false)); //12
                arguments.Append(" " + argFormatter.GetFormattedArgument(SampleSizeTo, false)); //13
            }
            else
            {
                arguments.Append(" " + "PowerAxis"); //11
                arguments.Append(" " + argFormatter.GetFormattedArgument(PowerFrom, false)); //12
                arguments.Append(" " + argFormatter.GetFormattedArgument(PowerTo, false)); //13
            }

            arguments.Append(" " + argFormatter.GetFormattedArgument(GraphTitle, false)); //14

            return arguments.ToString().Trim();
        }
    }
}