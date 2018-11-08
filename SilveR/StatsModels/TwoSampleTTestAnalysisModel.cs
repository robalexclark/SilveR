using SilveR.Helpers;
using SilveR.Models;
using SilveR.Validators;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Text;

namespace SilveR.StatsModels
{
    public class TwoSampleTTestAnalysisModel : AnalysisModelBase
    {
        [CheckUsedOnceOnly]
        [DisplayName("Response")]
        public string Response { get; set; }

        [DisplayName("Reponse transformation")]
        public string ResponseTransformation { get; set; } = "None";

        public IEnumerable<string> TransformationsList
        {
            get { return new List<string>() { "None", "Log10", "Loge", "Square Root", "ArcSine", "Rank" }; }
        }

        [CheckUsedOnceOnly]
        [DisplayName("Treatment")]
        public string Treatment { get; set; }

        [DisplayName("Significance")]
        public string Significance { get; set; } = "0.05";

        public IEnumerable<string> SignificancesList
        {
            get { return new List<string>() { "0.1", "0.05", "0.01", "0.001" }; }
        }

        [DisplayName("Unequal variance case")]
        public bool UnequalVariance { get; set; }

        [DisplayName("Equal variance case")]
        public bool EqualVariance { get; set; } = true;

        [DisplayName("Predicated vs. residuals plot")]
        public bool PRPlot { get; set; } = true;

        [DisplayName("Normal probability plot")]
        public bool NormalPlot { get; set; }


        public TwoSampleTTestAnalysisModel() : this(null) { }

        public TwoSampleTTestAnalysisModel(IDataset dataset)
            : base(dataset, "TwoSampleTTestAnalysis") { }

        public override ValidationInfo Validate()
        {
            TwoSampleTTestAnalysisValidator twoSampleTTestAnalysisValidator = new TwoSampleTTestAnalysisValidator(this);
            return twoSampleTTestAnalysisValidator.Validate();
        }

        public override string[] ExportData()
        {
            DataTable dtNew = DataTable.CopyForExport();

            //Get the response, treatment and covariate columns by removing all other columns from the new datatable
            foreach (string columnName in dtNew.GetVariableNames())
            {
                if (Response != columnName && Treatment != columnName)
                {
                    dtNew.Columns.Remove(columnName);
                }
            }

            //if the response is blank then remove that row
            dtNew.RemoveBlankRow(Response);

            //Now do transformations...
            dtNew.TransformColumn(Response, ResponseTransformation);

            //Finally, as numeric categorical variables get misinterpreted by r, we need to go through
            //each column and put them in quotes...
            if (dtNew.CheckIsNumeric(Treatment))
            {
                foreach (DataRow row in dtNew.Rows)
                {
                    row[Treatment] = "'" + row[Treatment] + "'";
                }
            }

            return dtNew.GetCSVArray();
        }

        public override IEnumerable<Argument> GetArguments()
        {
            List<Argument> args = new List<Argument>();

            args.Add(ArgumentHelper.ArgumentFactory(nameof(Response), Response));
            args.Add(ArgumentHelper.ArgumentFactory(nameof(ResponseTransformation), ResponseTransformation));
            args.Add(ArgumentHelper.ArgumentFactory(nameof(Treatment), Treatment));
            args.Add(ArgumentHelper.ArgumentFactory(nameof(EqualVariance), EqualVariance));
            args.Add(ArgumentHelper.ArgumentFactory(nameof(UnequalVariance), UnequalVariance));
            args.Add(ArgumentHelper.ArgumentFactory(nameof(PRPlot), PRPlot));
            args.Add(ArgumentHelper.ArgumentFactory(nameof(NormalPlot), NormalPlot));
            args.Add(ArgumentHelper.ArgumentFactory(nameof(Significance), Significance));

            return args;
        }

        public override void LoadArguments(IEnumerable<Argument> arguments)
        {
            ArgumentHelper argHelper = new ArgumentHelper(arguments);

            this.Response = argHelper.ArgumentLoader(nameof(Response), Response);
            this.ResponseTransformation = argHelper.ArgumentLoader(nameof(ResponseTransformation), ResponseTransformation);
            this.Treatment = argHelper.ArgumentLoader(nameof(Treatment), Treatment);
            this.EqualVariance = argHelper.ArgumentLoader(nameof(EqualVariance), EqualVariance);
            this.UnequalVariance = argHelper.ArgumentLoader(nameof(UnequalVariance), UnequalVariance);
            this.PRPlot = argHelper.ArgumentLoader(nameof(PRPlot), PRPlot);
            this.NormalPlot = argHelper.ArgumentLoader(nameof(NormalPlot), NormalPlot);
            this.Significance = argHelper.ArgumentLoader(nameof(Significance), Significance);
        }

        public override string GetCommandLineArguments()
        {
            ArgumentFormatter argFormatter = new ArgumentFormatter();
            StringBuilder arguments = new StringBuilder();

            arguments.Append(" " + argFormatter.GetFormattedArgument(Response, true));
            arguments.Append(" " + argFormatter.GetFormattedArgument(ResponseTransformation));

            arguments.Append(" " + argFormatter.GetFormattedArgument(Treatment, true));

            arguments.Append(" " + argFormatter.GetFormattedArgument(EqualVariance));
            arguments.Append(" " + argFormatter.GetFormattedArgument(UnequalVariance));

            arguments.Append(" " + argFormatter.GetFormattedArgument(PRPlot));
            arguments.Append(" " + argFormatter.GetFormattedArgument(NormalPlot));

            arguments.Append(" " + argFormatter.GetFormattedArgument(Significance));

            return arguments.ToString();
        }

    }
}