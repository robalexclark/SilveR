﻿using SilveR.Helpers;
using SilveR.Models;
using SilveR.Validators;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Text;

namespace SilveR.StatsModels
{
    public class UnpairedTTestAnalysisModel : AnalysisDataModelBase
    {
        [Required]
        [CheckUsedOnceOnly]
        [DisplayName("Response")]
        public string Response { get; set; }

        [DisplayName("Response transformation")]
        public string ResponseTransformation { get; set; } = "None";

        public IEnumerable<string> TransformationsList
        {
            get { return new List<string>() { "None", "Log10", "Loge", "Square Root", "ArcSine", "Rank" }; }
        }

        [Required]
        [CheckUsedOnceOnly]
        [DisplayName("Treatment factor")]
        public string Treatment { get; set; }


        [DisplayName("Equal variance case")]
        public bool EqualVarianceCaseSelected { get; set; } = true;

        [DisplayName("Unequal variance case")]
        public bool UnequalVarianceCaseSelected { get; set; } = true;

        [DisplayName("Residuals vs. predicted plot")]
        public bool ResidualsVsPredictedPlotSelected { get; set; }

        [DisplayName("Normal probability plot")]
        public bool NormalProbabilityPlotSelected { get; set; }

        [DisplayName("Significance level")]
        public string Significance { get; set; } = "0.05";

        [DisplayName("Control group")]
        public string ControlGroup { get; set; }

        public IEnumerable<string> SignificancesList
        {
            get { return new List<string>() { "0.1", "0.05", "0.025", "0.01", "0.001" }; }
        }


        public UnpairedTTestAnalysisModel() : base("UnpairedTTestAnalysis") { }

        public UnpairedTTestAnalysisModel(IDataset dataset)
            : base(dataset, "UnpairedTTestAnalysis") { }


        public override ValidationInfo Validate()
        {
            UnpairedTTestAnalysisValidator unpairedTTestAnalysisValidator = new UnpairedTTestAnalysisValidator(this);
            return unpairedTTestAnalysisValidator.Validate();
        }

        public override string[] ExportData()
        {
            DataTable dtNew = DataTable.CopyForExport();

            //Get the response, treatment and covariate columns by removing all other columns from the new datatable
            foreach (string col in dtNew.GetVariableNames())
            {
                if (Response != col && Treatment != col)
                {
                    dtNew.Columns.Remove(col);
                }
            }

            //if the response is blank then remove that row
            dtNew.RemoveBlankRow(Response);

            //Now do transformations...
            dtNew.TransformColumn(Response, ResponseTransformation);

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
            args.Add(ArgumentHelper.ArgumentFactory(nameof(ResponseTransformation), ResponseTransformation));
            args.Add(ArgumentHelper.ArgumentFactory(nameof(Treatment), Treatment));
            args.Add(ArgumentHelper.ArgumentFactory(nameof(EqualVarianceCaseSelected), EqualVarianceCaseSelected));
            args.Add(ArgumentHelper.ArgumentFactory(nameof(UnequalVarianceCaseSelected), UnequalVarianceCaseSelected));
            args.Add(ArgumentHelper.ArgumentFactory(nameof(ResidualsVsPredictedPlotSelected), ResidualsVsPredictedPlotSelected));
            args.Add(ArgumentHelper.ArgumentFactory(nameof(NormalProbabilityPlotSelected), NormalProbabilityPlotSelected));
            args.Add(ArgumentHelper.ArgumentFactory(nameof(ControlGroup), ControlGroup));
            args.Add(ArgumentHelper.ArgumentFactory(nameof(Significance), Significance));

            return args;
        }

        public override void LoadArguments(IEnumerable<Argument> arguments)
        {
            ArgumentHelper argHelper = new ArgumentHelper(arguments);

            this.Response = argHelper.LoadStringArgument(nameof(Response));
            this.ResponseTransformation = argHelper.LoadStringArgument(nameof(ResponseTransformation));
            this.Treatment = argHelper.LoadStringArgument(nameof(Treatment));
            this.EqualVarianceCaseSelected = argHelper.LoadBooleanArgument(nameof(EqualVarianceCaseSelected));
            this.UnequalVarianceCaseSelected = argHelper.LoadBooleanArgument(nameof(UnequalVarianceCaseSelected));
            this.ResidualsVsPredictedPlotSelected = argHelper.LoadBooleanArgument(nameof(ResidualsVsPredictedPlotSelected));
            this.NormalProbabilityPlotSelected = argHelper.LoadBooleanArgument(nameof(NormalProbabilityPlotSelected));
            this.ControlGroup = argHelper.LoadStringArgument(nameof(ControlGroup));
            this.Significance = argHelper.LoadStringArgument(nameof(Significance));
        }

        public override string GetCommandLineArguments()
        {
            ArgumentFormatter argFormatter = new ArgumentFormatter();
            StringBuilder arguments = new StringBuilder();

            arguments.Append(" " + argFormatter.GetFormattedArgument(Response, true)); //4
            arguments.Append(" " + argFormatter.GetFormattedArgument(ResponseTransformation, false)); //5
            arguments.Append(" " + argFormatter.GetFormattedArgument(Treatment, true)); //6          

            arguments.Append(" " + argFormatter.GetFormattedArgument(EqualVarianceCaseSelected)); //7
            arguments.Append(" " + argFormatter.GetFormattedArgument(UnequalVarianceCaseSelected)); //8
            arguments.Append(" " + argFormatter.GetFormattedArgument(ResidualsVsPredictedPlotSelected)); //9
            arguments.Append(" " + argFormatter.GetFormattedArgument(NormalProbabilityPlotSelected)); //10

            arguments.Append(" " + argFormatter.GetFormattedArgument(ControlGroup, false)); //11
            arguments.Append(" " + argFormatter.GetFormattedArgument(Significance, false)); //12

            return arguments.ToString().Trim();
        }
    }
}