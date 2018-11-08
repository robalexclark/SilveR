using SilveR.Helpers;
using SilveR.Models;
using SilveR.Validators;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace SilveR.StatsModels
{
    public class PValueAdjustmentModel : AnalysisModelBase
    {
        [Required]
        [CheckPValue]
        [DisplayName("Unadjusted p-Values")]
        public string PValues { get; set; }

        [DisplayName("Multiple comparison adjustment")]
        public string SelectedTest { get; set; }

        public IEnumerable<string> MultipleComparisonTests
        {
            get { return new List<string>() { "Holm", "Hochberg", "Hommel", "Benjamini-Hochberg", "Bonferroni" }; }
        }

        [DisplayName("Significance level")]
        public string Significance { get; set; } = "0.05";

        public IEnumerable<string> SignificancesList
        {
            get { return new List<string>() { "0.1", "0.05", "0.01", "0.001" }; }
        }

        public PValueAdjustmentModel()
            : base(null, "PValueAdjustment") { }


        public override ValidationInfo Validate()
        {
            PValueAdjustmentValidator pValueAdjustmentValidator = new PValueAdjustmentValidator(this);
            return pValueAdjustmentValidator.Validate();
        }

        public override string[] ExportData()
        {
            return null;
        }

        public override void LoadArguments(IEnumerable<Argument> arguments)
        {
            ArgumentHelper argHelper = new ArgumentHelper(arguments);

            this.PValues = argHelper.ArgumentLoader(nameof(PValues), PValues);
            this.SelectedTest = argHelper.ArgumentLoader(nameof(SelectedTest), SelectedTest);
            this.Significance = argHelper.ArgumentLoader(nameof(Significance), Significance);
        }

        public override IEnumerable<Argument> GetArguments()
        {
            List<Argument> args = new List<Argument>();

            args.Add(ArgumentHelper.ArgumentFactory(nameof(PValues), PValues));
            args.Add(ArgumentHelper.ArgumentFactory(nameof(SelectedTest), SelectedTest));
            args.Add(ArgumentHelper.ArgumentFactory(nameof(Significance), Significance));

            return args;
        }

        public override string GetCommandLineArguments()
        {
            StringBuilder arguments = new StringBuilder();

            arguments.Append(" " + PValues.Replace(" ", "").Replace("<", "^<")); //4
            arguments.Append(" " + SelectedTest); //5
            arguments.Append(" " + Significance.Replace("<", "^<")); //6

            return arguments.ToString();
        }
    }
}