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
using System.Text.RegularExpressions;

namespace SilveR.StatsModels
{
    public class RRunnerModel : AnalysisDataModelBase
    {
        [Required]
        [DisplayName("Variable A")]
        public IEnumerable<string> VariableA { get; set; }

        [DisplayName("Variable B")]
        public IEnumerable<string> VariableB { get; set; }

        [DisplayName("Variable C")]
        public IEnumerable<string> VariableC { get; set; }

        [DisplayName("Variable D")]
        public IEnumerable<string> VariableD { get; set; }

        public string RawArguments { get; set; }

        public RRunnerModel() : base("RRunner") { }

        public RRunnerModel(IDataset dataset)
            : base(dataset, "RRunner") { }

        public override ValidationInfo Validate()
        {
            return new ValidationInfo();
        }

        public override string[] ExportData()
        {
            DataTable dtNew = DataTable.CopyForExport();

            //Get the response, treatment and covariate columns by removing all other columns from the new datatable
            foreach (string columnName in dtNew.GetVariableNames())
            {
                if (!VariableA.Contains(columnName) && (VariableB == null || !VariableB.Contains(columnName)) && (VariableC == null || !VariableC.Contains(columnName) && (VariableD == null || !VariableD.Contains(columnName))))
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


        public override string GetCommandLineArguments()
        {
            ArgumentFormatter argFormatter = new ArgumentFormatter();
            StringBuilder arguments = new StringBuilder();

            arguments.Append(" " + argFormatter.GetFormattedArgument(VariableA)); //7

            arguments.Append(" " + argFormatter.GetFormattedArgument(VariableB)); //8

            arguments.Append(" " + argFormatter.GetFormattedArgument(VariableC)); //9

            arguments.Append(" " + argFormatter.GetFormattedArgument(VariableD)); //10

            IEnumerable<string> argumentLines = Regex.Split(RawArguments, "\r\n|\r|\n");

            foreach (string line in argumentLines.Where(x => !String.IsNullOrWhiteSpace(x)))
            {
                arguments.Append(" " + line.Trim());
            }

            return arguments.ToString().Trim();
        }


        public override void LoadArguments(IEnumerable<Argument> arguments)
        {
            ArgumentHelper argHelper = new ArgumentHelper(arguments);

            this.CustomRCode = argHelper.LoadStringArgument(nameof(CustomRCode));

            this.VariableA = argHelper.LoadIEnumerableArgument(nameof(VariableA));
            this.VariableB = argHelper.LoadIEnumerableArgument(nameof(VariableB));
            this.VariableC = argHelper.LoadIEnumerableArgument(nameof(VariableC));
            this.VariableD = argHelper.LoadIEnumerableArgument(nameof(VariableD));
            this.RawArguments = argHelper.LoadStringArgument(nameof(RawArguments));
        }

        public override IEnumerable<Argument> GetArguments()
        {
            List<Argument> args = new List<Argument>();

            args.Add(ArgumentHelper.ArgumentFactory(nameof(CustomRCode), CustomRCode));

            args.Add(ArgumentHelper.ArgumentFactory(nameof(VariableA), VariableA));
            args.Add(ArgumentHelper.ArgumentFactory(nameof(VariableB), VariableB));
            args.Add(ArgumentHelper.ArgumentFactory(nameof(VariableC), VariableC));
            args.Add(ArgumentHelper.ArgumentFactory(nameof(VariableD), VariableD));
            args.Add(ArgumentHelper.ArgumentFactory(nameof(RawArguments), RawArguments));

            return args;
        }
    }
}