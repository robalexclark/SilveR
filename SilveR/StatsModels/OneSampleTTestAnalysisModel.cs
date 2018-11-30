using SilveR.Helpers;
using SilveR.Models;
using SilveR.Validators;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq;
using System.Text;

namespace SilveR.StatsModels
{
    public class OneSampleTTestAnalysisModel : AnalysisDataModelBase
    {
        [HasAtLeastOneEntry]
        [CheckUsedOnceOnly]
        [DisplayName("Responses")]
        public IEnumerable<string> Responses { get; set; }

        [DisplayName("Reponse transformation")]
        public string ResponseTransformation { get; set; } = "None";

        public IEnumerable<string> TransformationsList
        {
            get { return new List<string>() { "None", "Log10", "Loge", "Square Root", "ArcSine", "Rank" }; }
        }

        [Required]
        [DisplayName("Test value")]
        public decimal TestValue { get; set; }

        [DisplayName("Significance")]
        public string Significance { get; set; } = "0.05";

        public IEnumerable<string> SignificancesList
        {
            get { return new List<string>() { "0.1", "0.05", "0.01", "0.001" }; }
        }

        [DisplayName("Confidence interval")]
        public bool ConfidenceInterval { get; set; } = true;

        [DisplayName("Normal probability plot")]
        public bool NormalPlot { get; set; }


        public OneSampleTTestAnalysisModel() : base("OneSampleTTestAnalysis") { }

        public OneSampleTTestAnalysisModel(IDataset dataset)
            : base(dataset, "OneSampleTTestAnalysis") { }

        public override ValidationInfo Validate()
        {
            OneSampleTTestAnalysisValidator oneSampleTTestAnalysisValidator = new OneSampleTTestAnalysisValidator(this);
            return oneSampleTTestAnalysisValidator.Validate();
        }

        public override string[] ExportData()
        {
            DataTable dtNew = DataTable.CopyForExport();

            //Get the response, treatment and covariate columns by removing all other columns from the new datatable
            foreach (string columnName in dtNew.GetVariableNames())
            {
                if (!Responses.Contains(columnName))
                {
                    dtNew.Columns.Remove(columnName);
                }
            }

            //Now do transformations
            foreach (string resp in Responses)
            {
                dtNew.TransformColumn(resp, ResponseTransformation);
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
            args.Add(ArgumentHelper.ArgumentFactory(nameof(ResponseTransformation), ResponseTransformation));
            args.Add(ArgumentHelper.ArgumentFactory(nameof(TestValue), TestValue));
            args.Add(ArgumentHelper.ArgumentFactory(nameof(ConfidenceInterval), ConfidenceInterval));
            args.Add(ArgumentHelper.ArgumentFactory(nameof(NormalPlot), NormalPlot));
            args.Add(ArgumentHelper.ArgumentFactory(nameof(Significance), Significance));

            return args;
        }

        public override void LoadArguments(IEnumerable<Argument> arguments)
        {
            ArgumentHelper argHelper = new ArgumentHelper(arguments);

            this.Responses = argHelper.LoadIEnumerableArgument(nameof(Responses));
            this.ResponseTransformation = argHelper.LoadStringArgument(nameof(ResponseTransformation));
            this.TestValue = argHelper.LoadDecimalArgument(nameof(TestValue));
            this.ConfidenceInterval = argHelper.LoadBooleanArgument(nameof(ConfidenceInterval));
            this.NormalPlot = argHelper.LoadBooleanArgument(nameof(NormalPlot));
            this.Significance = argHelper.LoadStringArgument(nameof(Significance));
        }

        public override string GetCommandLineArguments()
        {
            ArgumentFormatter argFormatter = new ArgumentFormatter();
            StringBuilder arguments = new StringBuilder();

            arguments.Append(" " + argFormatter.GetFormattedArgument(Responses));
            arguments.Append(" " + argFormatter.GetFormattedArgument(ResponseTransformation, false));

            arguments.Append(" " + argFormatter.GetFormattedArgument(TestValue.ToString(), false));

            arguments.Append(" " + argFormatter.GetFormattedArgument(ConfidenceInterval));
            arguments.Append(" " + argFormatter.GetFormattedArgument(NormalPlot));

            arguments.Append(" " + argFormatter.GetFormattedArgument(Significance, false));

            return arguments.ToString().Trim();
        }
    }
}