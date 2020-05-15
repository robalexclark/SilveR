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
    public class SingleMeasuresToRepeatedMeasuresTransformationModel : AnalysisDataModelBase
    {
        [Required]
        [CheckUsedOnceOnly]
        [DisplayName("Responses")]
        public IEnumerable<string> Responses { get; set; }

        [DisplayName("Include all variables")]
        public bool IncludeAllVariables { get; set; }

        [Required]
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

        public SingleMeasuresToRepeatedMeasuresTransformationModel() : base("SingleMeasuresToRepeatedMeasuresTransformation") { }

        public SingleMeasuresToRepeatedMeasuresTransformationModel(IDataset dataset)
            : base(dataset, "SingleMeasuresToRepeatedMeasuresTransformation") { }


        public override ValidationInfo Validate()
        {
            //SummaryStatisticsValidator summaryStatisticsValidator = new SummaryStatisticsValidator(this);
            return new ValidationInfo();// summaryStatisticsValidator.Validate();
        }

        public override string[] ExportData()
        {
            DataTable dtNew = DataTable.CopyForExport();

            //Get the response, treatment and covariate columns by removing all other columns from the new datatable
            foreach (string columnName in dtNew.GetVariableNames())
            {
                if (!Responses.Contains(columnName) && !SelectedVariables.Contains(columnName))
                {
                    dtNew.Columns.Remove(columnName);
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
            this.IncludeAllVariables = argHelper.LoadBooleanArgument(nameof(IncludeAllVariables));
            this.Responses = argHelper.LoadIEnumerableArgument(nameof(SelectedVariables));
            this.ResponseName = argHelper.LoadStringArgument(nameof(ResponseName));
            this.RepeatedFactorName = argHelper.LoadStringArgument(nameof(RepeatedFactorName));
            this.SubjectFactorName = argHelper.LoadStringArgument(nameof(SubjectFactorName));
        }

        public override string GetCommandLineArguments()
        {
            ArgumentFormatter argFormatter = new ArgumentFormatter();
            StringBuilder arguments = new StringBuilder();

            arguments.Append(" " + argFormatter.GetFormattedArgument(Responses)); //4
            arguments.Append(" " + argFormatter.GetFormattedArgument(IncludeAllVariables)); //5
            arguments.Append(" " + argFormatter.GetFormattedArgument(SelectedVariables)); //6
            arguments.Append(" " + argFormatter.GetFormattedArgument(ResponseName)); //7
            arguments.Append(" " + argFormatter.GetFormattedArgument(RepeatedFactorName)); //8
            arguments.Append(" " + argFormatter.GetFormattedArgument(SubjectFactorName)); //9

            return arguments.ToString().Trim();
        }
    }
}