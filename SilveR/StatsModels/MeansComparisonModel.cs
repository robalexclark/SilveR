using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq;
using System.Text;
using SilveRModel.Helpers;
using SilveRModel.Models;
using SilveRModel.Validators;

namespace SilveRModel.StatsModel
{
    public class MeansComparisonModel : IAnalysisModel
    {
        public string ScriptFileName { get { return "MeansComparison"; } }

        private DataTable dataTable;
        public DataTable DataTable
        {
            get { return dataTable; }
        }

        public Nullable<int> DatasetID { get; set; }

        private IEnumerable<string> availableVariables = new List<string>();
        public IEnumerable<string> AvailableVariables
        {
            get { return availableVariables; }
        }

        public IEnumerable<string> AvailableVariablesAllowNull
        {
            get
            {
                List<string> availableVars = availableVariables.ToList();
                availableVars.Insert(0, String.Empty);
                return availableVars.AsEnumerable();
            }
        }

        [ValidateGroupMean]
        [Range(0, double.MaxValue)]
        [DisplayName("Group Mean")]
        public string GroupMean { get; set; }

        [ValidateStandardDeviation]
        [Range(0, double.MaxValue)]
        [DisplayName("Standard Deviation")]
        public string StandardDeviation { get; set; }

        [ValidateVariance]
        [Range(0, double.MaxValue)]
        public string Variance { get; set; }

        public enum ValueTypeOption { Supplied = 0, Dataset = 1 }
        private ValueTypeOption valueType = ValueTypeOption.Supplied;
        public ValueTypeOption ValueType
        {
            get { return valueType; }
            set
            {
                if (valueType != value)
                {
                    valueType = value;
                }
            }
        }

        [ValidateResponseOrTreatment]
        [CheckUsedOnceOnly]
        public string Response { get; set; }

        //[ValidateResponseOrTreatment]
        [CheckUsedOnceOnly]
        public string Treatment { get; set; }


        public string Significance { get; set; } = "0.05";

        public List<string> SignificancesList
        {
            get { return new List<string>() { "0.1", "0.05", "0.01", "0.001" }; }
        }

        [ValidateControlGroup]
        [DisplayName("Control Group")]
        public string ControlGroup { get; set; }

        public enum ChangeTypeOption { Percent = 0, Absolute = 1 }
        private ChangeTypeOption changeType = ChangeTypeOption.Percent;
        public ChangeTypeOption ChangeType
        {
            get { return changeType; }
            set
            {
                if (changeType != value)
                {
                    changeType = value;
                }
            }
        }

        [ValidatePercentChanges]
        [Display(Name = "Percent Change")]
        public string PercentChange { get; set; }// = String.Empty;

        [ValidateAbsoluteChanges]
        [Display(Name = "Absolute Change")]
        public string AbsoluteChange { get; set; }// = String.Empty;

        public enum PlottingRangeTypeOption { SampleSize = 0, Power = 1 }
        private PlottingRangeTypeOption plottingRangeType = PlottingRangeTypeOption.SampleSize;
        public PlottingRangeTypeOption PlottingRangeType
        {
            get { return plottingRangeType; }
            set
            {
                if (plottingRangeType != value)
                {
                    plottingRangeType = value;
                }
            }
        }


        [ValidateSampleSizeFrom]
        [Display(Name = "Sample size from")]
        public string SampleSizeFrom { get; set; } = "6";

        [ValidateSampleSizeTo]
        [Display(Name = "Sample size to")]
        public string SampleSizeTo { get; set; } = "15";

        [ValidateCustomFrom]
        [Display(Name = "Power from")]
        public string PowerFrom { get; set; } = "70";

        [ValidateCustomTo]
        [Display(Name = "Power to")]
        public string PowerTo { get; set; } = "90";

        [Display(Name = "Graph title")]
        public string GraphTitle { get; set; }


        public MeansComparisonModel() { }

        public MeansComparisonModel(Dataset dataset)
        {
            //setup model
            ReInitialize(dataset);
        }

        public void ReInitialize(Dataset dataset)
        {
            this.DatasetID = dataset.DatasetID;
            dataTable = dataset.DatasetToDataTable();

            availableVariables = dataTable.GetVariableNames();
        }

        public ValidationInfo Validate()
        {
            MeansComparisonValidator meansComparisonValidator = new MeansComparisonValidator(this);
            return meansComparisonValidator.Validate();
        }

        public string[] ExportData()
        {
            DataTable dtNew = dataTable.CopyForExport();

            //Get the response, treatment and covariate columns by removing all other columns from the new datatable
            foreach (string col in this.DataTable.GetVariableNames())
            {
                if (Response != col && Treatment != col)
                {
                    dtNew.Columns.Remove(col);
                }
            }

            //ensure that all data is trimmed
            dtNew.TrimAllDataInDataTable();

            //if the response is blank then remove that row
            dtNew.RemoveBlankRow(Response);

            return dtNew.GetCSVArray();
        }

        public IEnumerable<Argument> GetArguments()
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

        public void LoadArguments(IEnumerable<Argument> arguments)
        {
            ArgumentHelper argHelper = new ArgumentHelper(arguments);

            this.GroupMean = argHelper.ArgumentLoader(nameof(GroupMean), GroupMean);
            this.StandardDeviation = argHelper.ArgumentLoader(nameof(StandardDeviation), StandardDeviation);
            this.Variance = argHelper.ArgumentLoader(nameof(Variance), Variance);
            this.ValueType = (ValueTypeOption)Enum.Parse(typeof(ValueTypeOption), argHelper.ArgumentLoader(nameof(ValueType), String.Empty), true);
            this.Response = argHelper.ArgumentLoader(nameof(Response), Response);
            this.Treatment = argHelper.ArgumentLoader(nameof(Treatment), Treatment);
            this.Significance = argHelper.ArgumentLoader(nameof(Significance), Significance);
            this.ControlGroup = argHelper.ArgumentLoader(nameof(ControlGroup), ControlGroup);
            this.ChangeType = (ChangeTypeOption)Enum.Parse(typeof(ChangeTypeOption), argHelper.ArgumentLoader(nameof(ChangeType), String.Empty), true);
            this.PercentChange = argHelper.ArgumentLoader(nameof(PercentChange), PercentChange);
            this.AbsoluteChange = argHelper.ArgumentLoader(nameof(AbsoluteChange), AbsoluteChange);
            this.PlottingRangeType = (PlottingRangeTypeOption)Enum.Parse(typeof(PlottingRangeTypeOption), argHelper.ArgumentLoader(nameof(PlottingRangeType), String.Empty), true);
            this.SampleSizeFrom = argHelper.ArgumentLoader(nameof(SampleSizeFrom), SampleSizeFrom);
            this.SampleSizeTo = argHelper.ArgumentLoader(nameof(SampleSizeTo), SampleSizeTo);
            this.PowerFrom = argHelper.ArgumentLoader(nameof(PowerFrom), PowerFrom);
            this.PowerTo = argHelper.ArgumentLoader(nameof(PowerTo), PowerTo);
            this.GraphTitle = argHelper.ArgumentLoader(nameof(GraphTitle), GraphTitle);
        }

        public string GetCommandLineArguments()
        {
            StringBuilder arguments = new StringBuilder();

            if (ValueType == ValueTypeOption.Supplied)
            {
                arguments.Append(" " + "SuppliedValues"); //4
                arguments.Append(" " + ArgumentConverters.GetNULLOrText(GroupMean)); //5

                if (!String.IsNullOrWhiteSpace(StandardDeviation))
                {
                    arguments.Append(" " + "StandardDeviation"); //6
                    arguments.Append(" " + StandardDeviation); //7
                }
                else if (!String.IsNullOrEmpty(Variance))
                {
                    arguments.Append(" " + "Variance"); //6
                    arguments.Append(" " + Variance); //7
                }
                else
                    throw new InvalidOperationException("no stdev or variance supplied!");
            }
            else if (ValueType == ValueTypeOption.Dataset)
            {
                arguments.Append(" " + "DatasetValues"); //4
                arguments.Append(" " + ArgumentConverters.ConvertIllegalChars(Response)); //5

                arguments.Append(" " + ArgumentConverters.GetNULLOrText(ArgumentConverters.ConvertIllegalChars(Treatment)));

                arguments.Append(" " + ArgumentConverters.GetNULLOrText(ArgumentConverters.ConvertIllegalChars(ControlGroup))); //7
            }

            arguments.Append(" " + Significance); //8

            if (ChangeType == ChangeTypeOption.Percent)
            {
                arguments.Append(" " + "Percent"); //9
                arguments.Append(" " + PercentChange); //10
            }
            else
            {
                arguments.Append(" " + "Absolute"); //9
                arguments.Append(" " + AbsoluteChange); //10
            }

            if (PlottingRangeType == PlottingRangeTypeOption.SampleSize)
            {
                arguments.Append(" " + "SampleSize"); //11
                arguments.Append(" " + SampleSizeFrom); //12
                arguments.Append(" " + SampleSizeTo); //13
            }
            else
            {
                arguments.Append(" " + "PowerAxis"); //11
                arguments.Append(" " + PowerFrom); //12
                arguments.Append(" " + PowerTo); //13
            }

            arguments.Append(" " + ArgumentConverters.GetNULLOrText(GraphTitle)); //14

            return arguments.ToString();
        }

        public bool VariablesUsedOnceOnly(string memberName)
        {
            object varToBeChecked = this.GetType().GetProperty(memberName).GetValue(this, null);

            if (varToBeChecked != null)
            {
                UniqueVariableChecker checker = new UniqueVariableChecker();

                if (memberName != "Response")
                    checker.AddVar(this.Response);

                if (memberName != "Treatment")
                    checker.AddVar(this.Treatment);

                return checker.DoCheck(varToBeChecked);
            }
            else
            {
                return true;
            }
        }
    }
}