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
    public class SingleMeasuresToRepeatedMeasuredTransformationModel : AnalysisDataModelBase
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
        public IEnumerable<string> Response { get; set; }

        [DisplayName("Repeated factor")]
        public string RepeatedFactor { get; set; }

        [CheckUsedOnceOnly]
        [DisplayName("Subject factor")]
        public string SubjectFactor { get; set; }    

        public SingleMeasuresToRepeatedMeasuredTransformationModel() : base("SingleMeasuresToRepeatedMeasuredTransformationModel") { }

        public SingleMeasuresToRepeatedMeasuredTransformationModel(IDataset dataset)
            : base(dataset, "SingleMeasuresToRepeatedMeasuredTransformation") { }


        public override ValidationInfo Validate()
        {
            //SummaryStatisticsValidator summaryStatisticsValidator = new SummaryStatisticsValidator(this);
            return null;// summaryStatisticsValidator.Validate();
        }

        public override string[] ExportData()
        {
            DataTable dtNew = DataTable.CopyForExport();

            //Get the response, treatment and covariate columns by removing all other columns from the new datatable
            foreach (string columnName in dtNew.GetVariableNames())
            {
                if (!Responses.Contains(columnName) && !SelectedVariables.Contains(columnName)  && RepeatedFactor != columnName && SubjectFactor != columnName )
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


            return args;
        }

        public override void LoadArguments(IEnumerable<Argument> arguments)
        {
            ArgumentHelper argHelper = new ArgumentHelper(arguments);

            this.Responses = argHelper.LoadIEnumerableArgument(nameof(Responses));

        }

        public override string GetCommandLineArguments()
        {
            ArgumentFormatter argFormatter = new ArgumentFormatter();
            StringBuilder arguments = new StringBuilder();

            arguments.Append(" " + argFormatter.GetFormattedArgument(Responses)); //4

         

            return arguments.ToString().Trim();
        }
    }
}