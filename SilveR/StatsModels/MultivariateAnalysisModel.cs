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
    public class MultivariateAnalysisModel : AnalysisDataModelBase
    {
        [Required]
        [CheckUsedOnceOnly]
        [HasAtLeastTwoEntries]
        [DisplayName("Responses")]
        public IEnumerable<string> Responses { get; set; }

        [DisplayName("Response transformation")]
        public string ResponseTransformation { get; set; } = "None";

        public IEnumerable<string> TransformationsList
        {
            get { return new List<string>() { "None", "Log10", "Loge", "Square Root", "ArcSine", "Rank" }; }
        }

        [CheckUsedOnceOnly]
        [DisplayName("Categorical predictor")]
        public string CategoricalPredictor { get; set; }

        [DisplayName("Continuous predictors")]
        [CheckUsedOnceOnly]
        public IEnumerable<string> ContinuousPredictors { get; set; }

        [CheckUsedOnceOnly]
        [DisplayName("Case ID")]
        public string CaseID { get; set; }


        public enum AnalysisOption { PrincipalComponentsAnalysis = 1, ClusterAnalysis = 2, LinearDiscriminantAnalysis = 3, PartialLeastSquares = 4 }
        [DisplayName("Analysis type")]
        public AnalysisOption AnalysisType { get; set; } = AnalysisOption.PrincipalComponentsAnalysis;


        [DisplayName("No. of clusters")]
        [Range(2, 1000, ErrorMessage = "Value for {0} must be between {1} and {2}.")]
        public int NoOfClusters { get; set; } = 2;

        [DisplayName("Distance method")]
        public string DistanceMethod { get; set; } = "Euclidean";

        public IEnumerable<string> DistanceMethodsList
        {
            get { return new List<string>() { "Euclidean", "Maximum", "Manhattan", "Canberra", "Binary", "Minkowski" }; }
        }

        [DisplayName("Agglomeration method")]
        public string AgglomerationMethod { get; set; } = "Ward.d2";

        public IEnumerable<string> AgglomerationMethodsList
        {
            get { return new List<string>() { "Ward.d2", "Single", "Complete", "Average" }; }
        }

        [DisplayName("Plot labels")]
        public string PlotLabel { get; set; } = "Case ID";

        public IEnumerable<string> PlotLabelsList
        {
            get { return new List<string>() { "Case ID", "Categorical Predictor" }; }
        }

        [DisplayName("Response centring")]
        public string ResponseCentring { get; set; } = "Centered at zero";

        public IEnumerable<string> ResponseCentringList
        {
            get { return new List<string>() { "Centered at zero", "Not centered" }; }
        }

        [DisplayName("Response scale")]
        public string ResponseScale { get; set; } = "Unit variance";

        public IEnumerable<string> ResponseScaleList
        {
            get { return new List<string>() { "Unit variance", "No scaling" }; }
        }

        [DisplayName("No. of Components")]
        public int NoOfComponents { get; set; }


        public MultivariateAnalysisModel() : base("MultivariateAnalysis") { }

        public MultivariateAnalysisModel(IDataset dataset)
            : base(dataset, "MultivariateAnalysis") { }

        public override ValidationInfo Validate()
        {
            MultivariateAnalysisValidator multivariateAnalysisValidator = new MultivariateAnalysisValidator(this);
            return multivariateAnalysisValidator.Validate();
        }

        public override string[] ExportData()
        {
            DataTable dtNew = DataTable.CopyForExport();

            //Get the response, treatment and covariate columns by removing all other columns from the new datatable
            foreach (string columnName in dtNew.GetVariableNames())
            {
                //if (!Responses.Contains(columnName) && (CategoricalPredictor != columnName || (CategoricalPredictor == columnName && AnalysisType != AnalysisOption.LinearDiscriminantAnalysis)) && (ContinuousPredictors == null || !ContinuousPredictors.Contains(columnName) || (ContinuousPredictors.Contains(columnName) && AnalysisType == AnalysisOption.PartialLeastSquares)) && CaseID != columnName)
                if (!Responses.Contains(columnName) && CategoricalPredictor != columnName && (ContinuousPredictors == null || !ContinuousPredictors.Contains(columnName)) && CaseID != columnName)
                {
                    dtNew.Columns.Remove(columnName);
                }
            }

            //if the response is blank then remove that row   
            foreach (string response in Responses)
            {
                dtNew.RemoveBlankRow(response);
            }

            //Now do transformations...
            foreach (string response in Responses)
            {
                dtNew.TransformColumn(response, ResponseTransformation);
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

            //first thing to do is to assemble the model (use the GetModel method)
            arguments.Append(" " + argFormatter.GetFormattedArgument(Responses)); //4
            arguments.Append(" " + argFormatter.GetFormattedArgument(ResponseTransformation, false)); //5
            arguments.Append(" " + argFormatter.GetFormattedArgument(CategoricalPredictor, true)); //6
            arguments.Append(" " + argFormatter.GetFormattedArgument(ContinuousPredictors)); //7
            arguments.Append(" " + argFormatter.GetFormattedArgument(CaseID, true)); //8
            arguments.Append(" " + argFormatter.GetFormattedArgument(AnalysisType.ToString(), false)); //9
            arguments.Append(" " + argFormatter.GetFormattedArgument(NoOfClusters)); //10
            arguments.Append(" " + argFormatter.GetFormattedArgument(DistanceMethod, false)); //11
            arguments.Append(" " + argFormatter.GetFormattedArgument(AgglomerationMethod, false)); //12
            arguments.Append(" " + argFormatter.GetFormattedArgument(PlotLabel, false)); //13
            arguments.Append(" " + argFormatter.GetFormattedArgument(NoOfComponents)); //14
            arguments.Append(" " + argFormatter.GetFormattedArgument(ResponseCentring.Replace(' ', '_'), false)); //15
            arguments.Append(" " + argFormatter.GetFormattedArgument(ResponseScale.Replace(' ', '_'), false)); //16

            return arguments.ToString().Trim();
        }


        public override void LoadArguments(IEnumerable<Argument> arguments)
        {
            ArgumentHelper argHelper = new ArgumentHelper(arguments);

            this.Responses = argHelper.LoadIEnumerableArgument(nameof(Responses));
            this.ResponseTransformation = argHelper.LoadStringArgument(nameof(ResponseTransformation));
            this.CategoricalPredictor = argHelper.LoadStringArgument(nameof(CategoricalPredictor));
            this.ContinuousPredictors = argHelper.LoadIEnumerableArgument(nameof(ContinuousPredictors));
            this.CaseID = argHelper.LoadStringArgument(nameof(CaseID));
            this.AnalysisType = (AnalysisOption)Enum.Parse(typeof(AnalysisOption), argHelper.LoadStringArgument(nameof(AnalysisType)), true);
            this.NoOfClusters = argHelper.LoadIntArgument(nameof(NoOfClusters));
            this.DistanceMethod = argHelper.LoadStringArgument(nameof(DistanceMethod));
            this.AgglomerationMethod = argHelper.LoadStringArgument(nameof(AgglomerationMethod));
            this.PlotLabel = argHelper.LoadStringArgument(nameof(PlotLabel));
            this.NoOfComponents = argHelper.LoadIntArgument(nameof(NoOfComponents));
            this.ResponseCentring = argHelper.LoadStringArgument(nameof(ResponseCentring));
            this.ResponseScale = argHelper.LoadStringArgument(nameof(ResponseScale));
        }

        public override IEnumerable<Argument> GetArguments()
        {
            List<Argument> args = new List<Argument>();

            args.Add(ArgumentHelper.ArgumentFactory(nameof(Responses), Responses));
            args.Add(ArgumentHelper.ArgumentFactory(nameof(ResponseTransformation), ResponseTransformation));
            args.Add(ArgumentHelper.ArgumentFactory(nameof(CategoricalPredictor), CategoricalPredictor));
            args.Add(ArgumentHelper.ArgumentFactory(nameof(ContinuousPredictors), ContinuousPredictors));
            args.Add(ArgumentHelper.ArgumentFactory(nameof(CaseID), CaseID));
            args.Add(ArgumentHelper.ArgumentFactory(nameof(AnalysisType), AnalysisType.ToString()));
            args.Add(ArgumentHelper.ArgumentFactory(nameof(NoOfClusters), NoOfClusters));
            args.Add(ArgumentHelper.ArgumentFactory(nameof(DistanceMethod), DistanceMethod));
            args.Add(ArgumentHelper.ArgumentFactory(nameof(AgglomerationMethod), AgglomerationMethod));
            args.Add(ArgumentHelper.ArgumentFactory(nameof(PlotLabel), PlotLabel));
            args.Add(ArgumentHelper.ArgumentFactory(nameof(NoOfComponents), NoOfComponents));
            args.Add(ArgumentHelper.ArgumentFactory(nameof(ResponseCentring), ResponseCentring));
            args.Add(ArgumentHelper.ArgumentFactory(nameof(ResponseScale), ResponseScale));

            return args;
        }
    }
}