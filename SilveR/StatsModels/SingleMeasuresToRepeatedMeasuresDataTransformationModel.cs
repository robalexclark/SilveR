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
    public class SingleMeasuresToRepeatedMeasuresDataTransformationModel : AnalysisDataModelBase
    {
        [Required]
        [HasAtLeastTwoEntries]
        [CheckUsedOnceOnly]
        [DisplayName("Responses")]
        public IEnumerable<string> Responses { get; set; }

        [CheckUsedOnceOnly]
        [DisplayName("SubjectFactor")]
        public string SubjectFactor { get; set; }

        [DisplayName("Include all variables")]
        public bool IncludeAllVariables { get; set; }

        [CheckUsedOnceOnly]
        [DisplayName("Selected variables")]
        public IEnumerable<string> SelectedVariables { get; set; }

        [DisplayName("Response")]
        public string ResponseName { get; set; }

        [DisplayName("Repeated factor")]
        public string RepeatedFactorName { get; set; }

        [CheckUsedOnceOnly]
        [DisplayName("Subject factor")]
        public string SubjectFactorName { get; set; }

        public SingleMeasuresToRepeatedMeasuresDataTransformationModel() : base("SingleMeasuresToRepeatedMeasuresDataTransformation") { }

        public SingleMeasuresToRepeatedMeasuresDataTransformationModel(IDataset dataset)
            : base(dataset, "SingleMeasuresToRepeatedMeasuresDataTransformation") { }


        public override ValidationInfo Validate()
        {
            //SummaryStatisticsValidator summaryStatisticsValidator = new SummaryStatisticsValidator(this);
            return new ValidationInfo();// summaryStatisticsValidator.Validate();
        }

        public override string[] ExportData()
        {
            DataTable dtNew = DataTable.CopyForExport();

            //Get the response, treatment and covariate columns by removing all other columns from the new datatable       
            if (!IncludeAllVariables) //ONLY if "include all variables" is not selected
            {
                foreach (string columnName in dtNew.GetVariableNames())
                {
                    if (!Responses.Contains(columnName) && (SubjectFactor == null || !SubjectFactor.Contains(columnName)) && (SelectedVariables == null || !SelectedVariables.Contains(columnName)))
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

            args.Add(ArgumentHelper.ArgumentFactory(nameof(Responses), Responses));
            args.Add(ArgumentHelper.ArgumentFactory(nameof(SubjectFactor), SubjectFactor));
            args.Add(ArgumentHelper.ArgumentFactory(nameof(IncludeAllVariables), IncludeAllVariables));
            args.Add(ArgumentHelper.ArgumentFactory(nameof(SelectedVariables), SelectedVariables));
            args.Add(ArgumentHelper.ArgumentFactory(nameof(ResponseName), ResponseName));
            args.Add(ArgumentHelper.ArgumentFactory(nameof(RepeatedFactorName), RepeatedFactorName));
            args.Add(ArgumentHelper.ArgumentFactory(nameof(SubjectFactorName), SubjectFactorName));

            return args;
        }

        public override void LoadArguments(IEnumerable<Argument> arguments)
        {
            ArgumentHelper argHelper = new ArgumentHelper(arguments);

            this.Responses = argHelper.LoadIEnumerableArgument(nameof(Responses));
            this.SubjectFactor = argHelper.LoadStringArgument(nameof(SubjectFactor));
            this.IncludeAllVariables = argHelper.LoadBooleanArgument(nameof(IncludeAllVariables));
            this.SelectedVariables = argHelper.LoadIEnumerableArgument(nameof(SelectedVariables));
            this.ResponseName = argHelper.LoadStringArgument(nameof(ResponseName));
            this.RepeatedFactorName = argHelper.LoadStringArgument(nameof(RepeatedFactorName));
            this.SubjectFactorName = argHelper.LoadStringArgument(nameof(SubjectFactorName));
        }

        public override string GetCommandLineArguments()
        {
            ArgumentFormatter argFormatter = new ArgumentFormatter();
            StringBuilder arguments = new StringBuilder();

            arguments.Append(" " + argFormatter.GetFormattedArgument(Responses)); //4
            arguments.Append(" " + argFormatter.GetFormattedArgument(SubjectFactor, true)); //5
            arguments.Append(" " + argFormatter.GetFormattedArgument(IncludeAllVariables)); //6
            arguments.Append(" " + argFormatter.GetFormattedArgument(SelectedVariables)); //7
            arguments.Append(" " + argFormatter.GetFormattedArgument(ResponseName, true)); //8
            arguments.Append(" " + argFormatter.GetFormattedArgument(RepeatedFactorName, true)); //9
            arguments.Append(" " + argFormatter.GetFormattedArgument(SubjectFactorName, true)); //10

            return arguments.ToString().Trim();
        }
    }
}