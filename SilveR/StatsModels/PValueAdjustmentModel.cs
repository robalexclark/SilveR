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

        private List<string> multipleComparisonTests = new List<string>() { "Holm", "Hochberg", "Hommel", "Benjamini-Hochberg", "Bonferroni" };
        public List<string> MultipleComparisonTests
        {
            get { return multipleComparisonTests; }
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

            this.PValues = argHelper.ArgumentLoader("PValues", PValues);
            this.SelectedTest = argHelper.ArgumentLoader("SelectedTest", SelectedTest);
            this.Significance = argHelper.ArgumentLoader("Significance", Significance);
        }

        public IEnumerable<Argument> GetArguments()
        {
            List<Argument> args = new List<Argument>();

            args.Add(ArgumentHelper.ArgumentFactory("PValues", PValues));
            args.Add(ArgumentHelper.ArgumentFactory("SelectedTest", SelectedTest));
            args.Add(ArgumentHelper.ArgumentFactory("Significance", Significance));

            return args;
        }

        public string GetCommandLineArguments()
        {
            StringBuilder arguments = new StringBuilder();

            arguments.Append(" " + PValues.Replace(" ", "").Replace("<", "^<")); //4
            arguments.Append(" " + SelectedTest); //5
            arguments.Append(" " + Significance.Replace("<", "^<")); //6
            arguments.Append(" " + "N");

            return arguments.ToString();
        }

        public bool VariablesUsedOnceOnly(string memberName)
        {
            throw new NotImplementedException();
        }
    }
}