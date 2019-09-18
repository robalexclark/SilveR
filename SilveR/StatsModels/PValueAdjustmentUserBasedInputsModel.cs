using SilveR.Helpers;
using SilveR.Models;
using SilveR.Validators;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Runtime.InteropServices;
using System.Text;

namespace SilveR.StatsModels
{
    public class PValueAdjustmentUserBasedInputsModel : AnalysisModelBase
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

        public PValueAdjustmentUserBasedInputsModel() : base("PValueAdjustmentUserBasedInputs") { }


        public override ValidationInfo Validate()
        {
            PValueAdjustmentUserBasedInputsValidator pValueAdjustmentUserBasedInputsValidator = new PValueAdjustmentUserBasedInputsValidator(this);
            return pValueAdjustmentUserBasedInputsValidator.Validate();
        }

        public override void LoadArguments(IEnumerable<Argument> arguments)
        {
            ArgumentHelper argHelper = new ArgumentHelper(arguments);

            this.PValues = argHelper.LoadStringArgument(nameof(PValues));
            this.SelectedTest = argHelper.LoadStringArgument(nameof(SelectedTest));
            this.Significance = argHelper.LoadStringArgument(nameof(Significance));
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

            string formattedPValues = PValues.Replace(" ", "");

            //need to escape the < symbol on windows (linux ok)
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                formattedPValues = formattedPValues.Replace("<", "^<");

            arguments.Append(" " + formattedPValues); //4
            arguments.Append(" " + SelectedTest); //5
            arguments.Append(" " + Significance); //6

            return arguments.ToString().Trim();
        }
    }
}