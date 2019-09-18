using SilveR.Helpers;
using SilveR.Models;
using SilveR.Validators;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Text;

namespace SilveR.StatsModels
{
    public class ChiSquaredAndFishersExactTestModel : AnalysisDataModelBase
    {
        [Required]
        [CheckUsedOnceOnly]
        [DisplayName("Response")]
        public string Response { get; set; }

        [Required]
        [CheckUsedOnceOnly]
        [DisplayName("Grouping factor")]
        public string GroupingFactor { get; set; }

        [Required]
        [CheckUsedOnceOnly]
        [DisplayName("Response categories")]
        public string ResponseCategories { get; set; }


        [DisplayName("Chi-squared test")]
        public bool ChiSquaredTest { get; set; } = true;

        [DisplayName("Fisher's exact test")]
        public bool FishersExactTest { get; set; } = true;

        [DisplayName("Hypothesis")]
        public string Hypothesis { get; set; } = "0.05";

        public IEnumerable<string> HypothesesList
        {
            get { return new List<string>() { "Two-sided", "Less-than", "Greater-than" }; }
        }

        [DisplayName("Barnard's test")]
        public bool BarnardsTest { get; set; }

        [DisplayName("Significance level")]
        public string Significance { get; set; } = "0.05";

        public IEnumerable<string> SignificancesList
        {
            get { return new List<string>() { "0.1", "0.05", "0.01", "0.001" }; }
        }


        public ChiSquaredAndFishersExactTestModel() : base("ChiSquaredAndFishersExactTest") { }

        public ChiSquaredAndFishersExactTestModel(IDataset dataset)
            : base(dataset, "ChiSquaredAndFishersExactTest") { }


        public override ValidationInfo Validate()
        {
            ChiSquaredAndFishersExactTestValidator chiSquaredAndFishersExactTestValidator = new ChiSquaredAndFishersExactTestValidator(this);
            return chiSquaredAndFishersExactTestValidator.Validate();
        }

        public override string[] ExportData()
        {
            DataTable dtNew = DataTable.CopyForExport();

            //Get the response, treatment and covariate columns by removing all other columns from the new datatable
            foreach (string col in dtNew.GetVariableNames())
            {
                if (Response != col && GroupingFactor != col && ResponseCategories != col)
                {
                    dtNew.Columns.Remove(col);
                }
            }

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

            args.Add(ArgumentHelper.ArgumentFactory(nameof(Response), Response));
            args.Add(ArgumentHelper.ArgumentFactory(nameof(GroupingFactor), GroupingFactor));
            args.Add(ArgumentHelper.ArgumentFactory(nameof(ResponseCategories), ResponseCategories));
            args.Add(ArgumentHelper.ArgumentFactory(nameof(ChiSquaredTest), ChiSquaredTest));
            args.Add(ArgumentHelper.ArgumentFactory(nameof(FishersExactTest), FishersExactTest));
            args.Add(ArgumentHelper.ArgumentFactory(nameof(Hypothesis), Hypothesis));
            args.Add(ArgumentHelper.ArgumentFactory(nameof(BarnardsTest), BarnardsTest));
            args.Add(ArgumentHelper.ArgumentFactory(nameof(Significance), Significance));

            return args;
        }

        public override void LoadArguments(IEnumerable<Argument> arguments)
        {
            ArgumentHelper argHelper = new ArgumentHelper(arguments);

            this.Response = argHelper.LoadStringArgument(nameof(Response));
            this.GroupingFactor = argHelper.LoadStringArgument(nameof(GroupingFactor));
            this.ResponseCategories = argHelper.LoadStringArgument(nameof(ResponseCategories));
            this.ChiSquaredTest = argHelper.LoadBooleanArgument(nameof(ChiSquaredTest));
            this.FishersExactTest = argHelper.LoadBooleanArgument(nameof(FishersExactTest));
            this.Hypothesis = argHelper.LoadStringArgument(nameof(Hypothesis));
            this.BarnardsTest = argHelper.LoadBooleanArgument(nameof(BarnardsTest));
            this.Significance = argHelper.LoadStringArgument(nameof(Significance));
        }

        public override string GetCommandLineArguments()
        {
            ArgumentFormatter argFormatter = new ArgumentFormatter();
            StringBuilder arguments = new StringBuilder();

            arguments.Append(" " + argFormatter.GetFormattedArgument(Response, true)); //4
            arguments.Append(" " + argFormatter.GetFormattedArgument(GroupingFactor, true)); //5
            arguments.Append(" " + argFormatter.GetFormattedArgument(ResponseCategories, true)); //6          

            arguments.Append(" " + argFormatter.GetFormattedArgument(ChiSquaredTest)); //7
            arguments.Append(" " + argFormatter.GetFormattedArgument(FishersExactTest)); //8
            arguments.Append(" " + argFormatter.GetFormattedArgument(Hypothesis, false)); //9
            arguments.Append(" " + argFormatter.GetFormattedArgument(BarnardsTest)); //10
            arguments.Append(" " + argFormatter.GetFormattedArgument(Significance, false)); //11

            return arguments.ToString().Trim();
        }
    }
}