using SilveRModel.Helpers;
using SilveRModel.Models;
using SilveRModel.Validators;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Text;

namespace SilveRModel.StatsModel
{
    public class PValueAdjustmentModel : IAnalysisModel
    {
        public string ScriptFileName { get { return "PValueAdjustment"; } }

        private DataTable dataTable = null;
        public DataTable DataTable
        {
            get { return dataTable; }
        }

        public Nullable<int> DatasetID { get; set; }

        [DisplayName("Unadjusted p-Values")]
        [Required]
        [CheckPValue]
        public string PValues { get; set; }

        [DisplayName("Multiple comparison adjustment")]
        public string SelectedTest { get; set; }

        public List<string> MultipleComparisonTests
        {
            get { return new List<string>() { "Holm", "Hochberg", "Hommel", "Benjamini-Hochberg", "Bonferroni" }; }
        }

        public string Significance { get; set; } = "0.05";

        public List<string> SignificancesList
        {
            get { return new List<string>() { "0.1", "0.05", "0.01", "0.001" }; }
        }

        public PValueAdjustmentModel() { }

        public void ReInitialize(Dataset dataset)
        {
        }

        public ValidationInfo Validate()
        {
            PValueAdjustmentValidator pValueAdjustmentValidator = new PValueAdjustmentValidator(this);
            return pValueAdjustmentValidator.Validate();
        }

        public string[] ExportData()
        {
            return null;
        }

        public void LoadArguments(IEnumerable<Argument> arguments)
        {
            ArgumentHelper argHelper = new ArgumentHelper(arguments);

            this.PValues = argHelper.ArgumentLoader(nameof(PValues), PValues);
            this.SelectedTest = argHelper.ArgumentLoader(nameof(SelectedTest), SelectedTest);
            this.Significance = argHelper.ArgumentLoader(nameof(Significance), Significance);
        }

        public IEnumerable<Argument> GetArguments()
        {
            List<Argument> args = new List<Argument>();

            args.Add(ArgumentHelper.ArgumentFactory(nameof(PValues), PValues));
            args.Add(ArgumentHelper.ArgumentFactory(nameof(SelectedTest), SelectedTest));
            args.Add(ArgumentHelper.ArgumentFactory(nameof(Significance), Significance));

            return args;
        }

        public string GetCommandLineArguments()
        {
            StringBuilder arguments = new StringBuilder();

            arguments.Append(" " + PValues.Replace(" ", "").Replace("<", "^<")); //4
            arguments.Append(" " + SelectedTest); //5
            arguments.Append(" " + Significance.Replace("<", "^<")); //6

            return arguments.ToString();
        }

        public bool VariablesUsedOnceOnly(string memberName)
        {
            throw new NotImplementedException();
        }
    }
}