using SilveR.Helpers;
using SilveR.Models;
using SilveR.Validators;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Text;

namespace SilveR.StatsModels
{
    public class GraphicalAnalysisModel : AnalysisDataModelBase
    {
        [Required]
        [CheckUsedOnceOnly]
        [DisplayName("Response")]
        public string Response { get; set; }

        [DisplayName("Response transformation")]
        public string ResponseTransformation { get; set; } = "None";

        public IEnumerable<string> TransformationsList
        {
            get { return new List<string>() { "None", "Log10", "Loge", "Square Root", "ArcSine" }; }
        }

        [ValidateXAxis]
        [CheckUsedOnceOnly]
        [DisplayName("X-axis variable")]
        public string XAxis { get; set; }

        [DisplayName("X-axis transformation")]
        public string XAxisTransformation { get; set; } = "None";

        [CheckUsedOnceOnly]
        [DisplayName("1st categorisation factor")]
        public string FirstCatFactor { get; set; }

        [CheckUsedOnceOnly]
        [DisplayName("2nd categorisation factor")]
        public string SecondCatFactor { get; set; }

        public enum GraphStyleType { Overlaid = 0, Separate = 1 }
        public GraphStyleType StyleType { get; set; } = GraphStyleType.Overlaid;

        [DisplayName("Display legend")]
        public bool DisplayLegend { get; set; } = true;

        [DisplayName("Include data")]
        public bool BoxPlotIncludeData { get; set; }

        [DisplayName("Include data")]
        public bool SEMPlotIncludeData { get; set; }

        [DisplayName("Main graph title")]
        public string MainTitle { get; set; }

        [DisplayName("X-axis title")]
        public string XAxisTitle { get; set; }

        [DisplayName("Y-axis title")]
        public string YAxisTitle { get; set; }


        [DisplayName("Categorical x-axis levels")]
        public string XAxisLevelsOrder { get; set; }

        [DisplayName("1st categorical factor levels")]
        public string FirstCatFactorLevelsOrder { get; set; }

        [DisplayName("2nd categorical factor levels")]
        public string SecondCatFactorLevelsOrder { get; set; }


        [DisplayName("Scatterplot")]
        public bool ScatterplotSelected { get; set; }

        [DisplayName("Linear fit")]
        public bool LinearFitSelected { get; set; }

        [DisplayName("Jitter")]
        public bool JitterSelected { get; set; }

        [DisplayName("Boxplot")]
        public bool BoxplotSelected { get; set; }

        [DisplayName("Outliers")]
        public bool OutliersSelected { get; set; }

        [DisplayName("SEM plot")]
        public bool SEMPlotSelected { get; set; }

        public enum SEMPlotType { Column = 0, Line = 1 }
        public SEMPlotType SEMType { get; set; } = SEMPlotType.Column;

        [DisplayName("Histogram plot")]
        public bool HistogramSelected { get; set; }

        [DisplayName("Normal dist. fit")]
        public bool NormalDistSelected { get; set; }

        [DisplayName("Case profiles plot")]
        public bool CaseProfilesPlotSelected { get; set; }

        [DisplayName("Case ID factor")]
        [ValidateCaseIDFactor]
        public string CaseIDFactor { get; set; }

        [DisplayName("Reference line")]
        public Nullable<decimal> ReferenceLine { get; set; }


        public GraphicalAnalysisModel() : base("GraphicalAnalysis") { }

        public GraphicalAnalysisModel(IDataset dataset)
            : base(dataset, "GraphicalAnalysis") { }


        public override ValidationInfo Validate()
        {
            GraphicalAnalysisValidator graphicalAnalysisValidator = new GraphicalAnalysisValidator(this);
            return graphicalAnalysisValidator.Validate();
        }

        public override string[] ExportData()
        {
            DataTable dtNew = DataTable.CopyForExport();

            foreach (string col in this.DataTable.GetVariableNames())
            {
                if (Response != col && XAxis != col && FirstCatFactor != col && SecondCatFactor != col && CaseIDFactor != col)
                {
                    dtNew.Columns.Remove(col);
                }
            }

            //if the response is blank then remove that row
            dtNew.RemoveBlankRow(Response);

            //Now do transformations...
            dtNew.TransformColumn(Response, ResponseTransformation);

            if (!String.IsNullOrEmpty(XAxis)) //check that an x axis var is selected
            {
                dtNew.TransformColumn(XAxis, XAxisTransformation);
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

            args.Add(ArgumentHelper.ArgumentFactory(nameof(Response), Response));
            args.Add(ArgumentHelper.ArgumentFactory(nameof(ResponseTransformation), ResponseTransformation));
            args.Add(ArgumentHelper.ArgumentFactory(nameof(XAxis), XAxis));
            args.Add(ArgumentHelper.ArgumentFactory(nameof(XAxisTransformation), XAxisTransformation));
            args.Add(ArgumentHelper.ArgumentFactory(nameof(FirstCatFactor), FirstCatFactor));
            args.Add(ArgumentHelper.ArgumentFactory(nameof(SecondCatFactor), SecondCatFactor));
            args.Add(ArgumentHelper.ArgumentFactory(nameof(StyleType), StyleType.ToString()));
            args.Add(ArgumentHelper.ArgumentFactory(nameof(DisplayLegend), DisplayLegend));
            args.Add(ArgumentHelper.ArgumentFactory(nameof(BoxPlotIncludeData), BoxPlotIncludeData));
            args.Add(ArgumentHelper.ArgumentFactory(nameof(SEMPlotIncludeData), SEMPlotIncludeData));
            args.Add(ArgumentHelper.ArgumentFactory(nameof(MainTitle), MainTitle));
            args.Add(ArgumentHelper.ArgumentFactory(nameof(XAxisTitle), XAxisTitle));
            args.Add(ArgumentHelper.ArgumentFactory(nameof(YAxisTitle), YAxisTitle));

            args.Add(ArgumentHelper.ArgumentFactory(nameof(XAxisLevelsOrder), FixFactorOrdering(XAxisLevelsOrder)));
            args.Add(ArgumentHelper.ArgumentFactory(nameof(FirstCatFactorLevelsOrder), FixFactorOrdering(FirstCatFactorLevelsOrder)));
            args.Add(ArgumentHelper.ArgumentFactory(nameof(SecondCatFactorLevelsOrder), FixFactorOrdering(SecondCatFactorLevelsOrder)));

            args.Add(ArgumentHelper.ArgumentFactory(nameof(ScatterplotSelected), ScatterplotSelected));
            args.Add(ArgumentHelper.ArgumentFactory(nameof(LinearFitSelected), LinearFitSelected));
            args.Add(ArgumentHelper.ArgumentFactory(nameof(JitterSelected), JitterSelected));
            args.Add(ArgumentHelper.ArgumentFactory(nameof(BoxplotSelected), BoxplotSelected));
            args.Add(ArgumentHelper.ArgumentFactory(nameof(OutliersSelected), OutliersSelected));
            args.Add(ArgumentHelper.ArgumentFactory(nameof(SEMPlotSelected), SEMPlotSelected));
            args.Add(ArgumentHelper.ArgumentFactory(nameof(SEMType), SEMType.ToString()));
            args.Add(ArgumentHelper.ArgumentFactory(nameof(HistogramSelected), HistogramSelected));
            args.Add(ArgumentHelper.ArgumentFactory(nameof(NormalDistSelected), NormalDistSelected));
            args.Add(ArgumentHelper.ArgumentFactory(nameof(CaseProfilesPlotSelected), CaseProfilesPlotSelected));
            args.Add(ArgumentHelper.ArgumentFactory(nameof(CaseIDFactor), CaseIDFactor));
            args.Add(ArgumentHelper.ArgumentFactory(nameof(ReferenceLine), ReferenceLine));

            return args;
        }

        public override void LoadArguments(IEnumerable<Argument> arguments)
        {
            ArgumentHelper argHelper = new ArgumentHelper(arguments);

            this.Response = argHelper.LoadStringArgument(nameof(Response));
            this.ResponseTransformation = argHelper.LoadStringArgument(nameof(ResponseTransformation));
            this.XAxis = argHelper.LoadStringArgument(nameof(XAxis));
            this.XAxisTransformation = argHelper.LoadStringArgument(nameof(XAxisTransformation));
            this.FirstCatFactor = argHelper.LoadStringArgument(nameof(FirstCatFactor));
            this.SecondCatFactor = argHelper.LoadStringArgument(nameof(SecondCatFactor));
            this.StyleType = (GraphStyleType)Enum.Parse(typeof(GraphStyleType), argHelper.LoadStringArgument(nameof(StyleType)), true);
            this.DisplayLegend = argHelper.LoadBooleanArgument(nameof(DisplayLegend));
            this.BoxPlotIncludeData = argHelper.LoadBooleanArgument(nameof(BoxPlotIncludeData));
            this.SEMPlotIncludeData = argHelper.LoadBooleanArgument(nameof(SEMPlotIncludeData));
            this.MainTitle = argHelper.LoadStringArgument(nameof(MainTitle));
            this.XAxisTitle = argHelper.LoadStringArgument(nameof(XAxisTitle));
            this.YAxisTitle = argHelper.LoadStringArgument(nameof(YAxisTitle));

            this.XAxisLevelsOrder = argHelper.LoadStringArgument(nameof(XAxisLevelsOrder));
            this.FirstCatFactorLevelsOrder = argHelper.LoadStringArgument(nameof(FirstCatFactorLevelsOrder));
            this.SecondCatFactorLevelsOrder = argHelper.LoadStringArgument(nameof(SecondCatFactorLevelsOrder));

            this.ScatterplotSelected = argHelper.LoadBooleanArgument(nameof(ScatterplotSelected));
            this.LinearFitSelected = argHelper.LoadBooleanArgument(nameof(LinearFitSelected));
            this.JitterSelected = argHelper.LoadBooleanArgument(nameof(JitterSelected));
            this.BoxplotSelected = argHelper.LoadBooleanArgument(nameof(BoxplotSelected));
            this.OutliersSelected = argHelper.LoadBooleanArgument(nameof(OutliersSelected));
            this.SEMPlotSelected = argHelper.LoadBooleanArgument(nameof(SEMPlotSelected));
            this.SEMType = (SEMPlotType)Enum.Parse(typeof(SEMPlotType), argHelper.LoadStringArgument(nameof(SEMType)), true);
            this.HistogramSelected = argHelper.LoadBooleanArgument(nameof(HistogramSelected));
            this.NormalDistSelected = argHelper.LoadBooleanArgument(nameof(NormalDistSelected));
            this.CaseProfilesPlotSelected = argHelper.LoadBooleanArgument(nameof(CaseProfilesPlotSelected));
            this.CaseIDFactor = argHelper.LoadStringArgument(nameof(CaseIDFactor));
            this.ReferenceLine = argHelper.LoadNullableDecimalArgument(nameof(ReferenceLine));
        }

        public override string GetCommandLineArguments()
        {
            ArgumentFormatter argFormatter = new ArgumentFormatter();
            StringBuilder arguments = new StringBuilder();

            //X-Axis variable
            arguments.Append(" " + argFormatter.GetFormattedArgument(XAxis, true)); //4

            //X-Axis transformation
            arguments.Append(" " + argFormatter.GetFormattedArgument(XAxisTransformation, false)); //5

            //Response variable
            arguments.Append(" " + argFormatter.GetFormattedArgument(Response, true)); //6

            //Response transformation
            arguments.Append(" " + argFormatter.GetFormattedArgument(ResponseTransformation, false)); //7

            //1st cat factor
            arguments.Append(" " + argFormatter.GetFormattedArgument(FirstCatFactor, true)); //8

            //2nd cat factor
            arguments.Append(" " + argFormatter.GetFormattedArgument(SecondCatFactor, true)); //9

            //Graph type
            arguments.Append(" " + argFormatter.GetFormattedArgument(StyleType.ToString(), false)); //10

            //Main title
            arguments.Append(" " + argFormatter.GetFormattedArgument(MainTitle, false)); //11

            //X-Axis title
            arguments.Append(" " + argFormatter.GetFormattedArgument(XAxisTitle, false)); //12

            //Y Axis title
            arguments.Append(" " + argFormatter.GetFormattedArgument(YAxisTitle, false)); //13


            //XAxisLevelsOrder
            arguments.Append(" " + argFormatter.GetFormattedArgument(FixFactorOrdering(XAxisLevelsOrder), false)); //14

            //FirstCatFactorLevelsOrder
            arguments.Append(" " + argFormatter.GetFormattedArgument(FixFactorOrdering(FirstCatFactorLevelsOrder), false)); //15

            //SecondCatFactorLevelsOrder
            arguments.Append(" " + argFormatter.GetFormattedArgument(FixFactorOrdering(SecondCatFactorLevelsOrder), false)); //16


            //Scatterplot
            arguments.Append(" " + argFormatter.GetFormattedArgument(ScatterplotSelected)); //17

            //Linear plot
            arguments.Append(" " + argFormatter.GetFormattedArgument(LinearFitSelected)); //18

            //Jitter
            arguments.Append(" " + argFormatter.GetFormattedArgument(JitterSelected)); //19

            //Boxplot
            arguments.Append(" " + argFormatter.GetFormattedArgument(BoxplotSelected)); //20

            //Outliers
            arguments.Append(" " + argFormatter.GetFormattedArgument(OutliersSelected)); //21

            //SEM Plot
            arguments.Append(" " + argFormatter.GetFormattedArgument(SEMPlotSelected)); //22

            //Column plot type
            arguments.Append(" " + argFormatter.GetFormattedArgument(SEMType.ToString(), false)); //23

            //Histogram plot
            arguments.Append(" " + argFormatter.GetFormattedArgument(HistogramSelected)); //24

            //Normal distribution fit
            arguments.Append(" " + argFormatter.GetFormattedArgument(NormalDistSelected)); //25

            //Case profiles plot
            arguments.Append(" " + argFormatter.GetFormattedArgument(CaseProfilesPlotSelected)); //26

            //Case ID Factor
            arguments.Append(" " + argFormatter.GetFormattedArgument(CaseIDFactor, true)); //27

            //Reference Line
            arguments.Append(" " + argFormatter.GetFormattedArgument(ReferenceLine.ToString(), false)); //28

            //Legend
            arguments.Append(" " + argFormatter.GetFormattedArgument(DisplayLegend)); //29

            //Box Plot include data
            arguments.Append(" " + argFormatter.GetFormattedArgument(BoxPlotIncludeData)); //30

            //SEM Plot include data
            arguments.Append(" " + argFormatter.GetFormattedArgument(SEMPlotIncludeData)); //31

            return arguments.ToString().Trim();
        }


        private string FixFactorOrdering(string factorOrder)
        {
            if (String.IsNullOrWhiteSpace(factorOrder))
                return null;

            string[] levels = factorOrder.Split(',');

            StringBuilder fixedString = new StringBuilder();
            foreach (string l in levels)
            {
                fixedString.Append(l.Trim() + ',');
            }

            return fixedString.ToString().TrimEnd(',');
        }
    }
}