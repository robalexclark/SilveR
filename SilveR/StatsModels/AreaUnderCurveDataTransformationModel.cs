using SilveR.Helpers;
using SilveR.Models;
using SilveR.Validators;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;

namespace SilveR.StatsModels
{
    public class AreaUnderCurveDataTransformationModel : AnalysisDataModelBase
    {
        public enum InputFormatType { RepeatedMeasuresFormat = 0, SingleMeasuresFormat = 1 }
        [DisplayName("Input format")]
        public InputFormatType SelectedInputFormat { get; set; } = InputFormatType.RepeatedMeasuresFormat;

        [CheckUsedOnceOnly]
        [DisplayName("Response")]
        public string Response { get; set; }

        [CheckUsedOnceOnly]
        [DisplayName("Subject factor")]
        public string SubjectFactor { get; set; }

        [CheckUsedOnceOnly]
        [DisplayName("Time factor")]
        public string TimeFactor { get; set; }

        [CheckUsedOnceOnly]
        [DisplayName("Responses")]
        public IEnumerable<string> Responses { get; set; }

        [DisplayName("Numerical time points")]
        public string NumericalTimePoints { get; set; }

        [DisplayName("Include all variables")]
        public bool IncludeAllVariables { get; set; }

        [CheckUsedOnceOnly]
        [DisplayName("Selected variables")]
        public IEnumerable<string> SelectedVariables { get; set; }

        public enum AUCOutputType { AUCFromTime0 = 0, AUCFromInitialTimepoint = 1, AUCForChangeFromBaseline = 2 }
        [DisplayName("AUC output type")]
        public AUCOutputType AUCOutput { get; set; } = AUCOutputType.AUCFromTime0;

        public AreaUnderCurveDataTransformationModel() : base("AreaUnderCurveDataTransformation") { }

        public AreaUnderCurveDataTransformationModel(IDataset dataset)
            : base(dataset, "AreaUnderCurveDataTransformation") { }


        public override ValidationInfo Validate()
        {
            AreaUnderCurveDataTransformationValidator areaUnderCurveDataTransformationValidator = new AreaUnderCurveDataTransformationValidator(this);
            return areaUnderCurveDataTransformationValidator.Validate();
        }

        public override string[] ExportData()
        {
            DataTable dtNew = DataTable.CopyForExport();

            //Get the response, treatment and covariate columns by removing all other columns from the new datatable       
            if (!IncludeAllVariables) //ONLY if "include all variables" is not selected
            {
                foreach (string columnName in dtNew.GetVariableNames())
                {
                    if (Response != columnName && SubjectFactor != columnName && TimeFactor != columnName && (Responses == null || !Responses.Contains(columnName)) && (SelectedVariables == null || !SelectedVariables.Contains(columnName)))
                    {
                        dtNew.Columns.Remove(columnName);
                    }
                }
            }

            string[] csvArray = dtNew.GetCSVArray();

            //fix any columns with illegal chars here (at the end)
            ArgumentFormatter argFormatter = new ArgumentFormatter();
            csvArray[0] = argFormatter.ConvertIllegalCharacters(csvArray[0]);

            return csvArray;
        }

        public override IEnumerable<Argument> GetArguments()
        {
            List<Argument> args = new List<Argument>();

            args.Add(ArgumentHelper.ArgumentFactory(nameof(SelectedInputFormat), SelectedInputFormat.ToString()));
            args.Add(ArgumentHelper.ArgumentFactory(nameof(Response), Response));
            args.Add(ArgumentHelper.ArgumentFactory(nameof(SubjectFactor), SubjectFactor));
            args.Add(ArgumentHelper.ArgumentFactory(nameof(TimeFactor), TimeFactor));
            args.Add(ArgumentHelper.ArgumentFactory(nameof(Responses), Responses));
            args.Add(ArgumentHelper.ArgumentFactory(nameof(NumericalTimePoints), NumericalTimePoints));
            args.Add(ArgumentHelper.ArgumentFactory(nameof(IncludeAllVariables), IncludeAllVariables));
            args.Add(ArgumentHelper.ArgumentFactory(nameof(SelectedVariables), SelectedVariables));
            args.Add(ArgumentHelper.ArgumentFactory(nameof(AUCOutput), AUCOutput.ToString()));

            return args;
        }

        public override void LoadArguments(IEnumerable<Argument> arguments)
        {
            ArgumentHelper argHelper = new ArgumentHelper(arguments);

            this.SelectedInputFormat = (InputFormatType)Enum.Parse(typeof(InputFormatType), argHelper.LoadStringArgument(nameof(SelectedInputFormat)), true);
            this.Response = argHelper.LoadStringArgument(nameof(Response));
            this.SubjectFactor = argHelper.LoadStringArgument(nameof(SubjectFactor));
            this.TimeFactor = argHelper.LoadStringArgument(nameof(TimeFactor));
            this.Responses = argHelper.LoadIEnumerableArgument(nameof(Responses));
            this.NumericalTimePoints = argHelper.LoadStringArgument(nameof(NumericalTimePoints));
            this.IncludeAllVariables = argHelper.LoadBooleanArgument(nameof(IncludeAllVariables));
            this.SelectedVariables = argHelper.LoadIEnumerableArgument(nameof(SelectedVariables));
            this.AUCOutput = (AUCOutputType)Enum.Parse(typeof(AUCOutputType), argHelper.LoadStringArgument(nameof(AUCOutput)), true);

        }

        public override string GetCommandLineArguments()
        {
            ArgumentFormatter argFormatter = new ArgumentFormatter();
            StringBuilder arguments = new StringBuilder();

            arguments.Append(" " + argFormatter.GetFormattedArgument(SelectedInputFormat.ToString())); //4

            arguments.Append(" " + argFormatter.GetFormattedArgument(Response, true)); //5
            arguments.Append(" " + argFormatter.GetFormattedArgument(SubjectFactor, true)); //6
            arguments.Append(" " + argFormatter.GetFormattedArgument(TimeFactor, true)); //7
            arguments.Append(" " + argFormatter.GetFormattedArgument(Responses)); //8
            arguments.Append(" " + argFormatter.GetFormattedArgument(NumericalTimePoints)); //9
            arguments.Append(" " + argFormatter.GetFormattedArgument(IncludeAllVariables)); //10
            arguments.Append(" " + argFormatter.GetFormattedArgument(SelectedVariables)); //11
            arguments.Append(" " + argFormatter.GetFormattedArgument(AUCOutput.ToString())); //12

            return arguments.ToString().Trim();
        }
    }
}