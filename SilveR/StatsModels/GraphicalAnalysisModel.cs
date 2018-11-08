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
    public class GraphicalAnalysisModel : AnalysisModelBase
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
        private GraphStyleType styleType = GraphStyleType.Overlaid;
        public GraphStyleType StyleType
        {
            get { return styleType; }
            set
            {
                if (styleType != value)
                {
                    styleType = value;
                }
            }
        }

        [DisplayName("Display legend")]
        public bool DisplayLegend { get; set; }

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
        private SEMPlotType semType = SEMPlotType.Column;
        public SEMPlotType SEMType
        {
            get { return semType; }
            set
            {
                if (semType != value)
                {
                    semType = value;
                }
            }
        }

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
        public string ReferenceLine { get; set; }


        public GraphicalAnalysisModel() : this(null) { }

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

            //ensure that all data is trimmed
            dtNew.TrimAllDataInDataTable();

            //if the response is blank then remove that row
            dtNew.RemoveBlankRow(Response);

            //Now do transformations...
            dtNew.TransformColumn(Response, ResponseTransformation);

            if (!String.IsNullOrEmpty(XAxis)) //check that an x axis var is selected
            {
                dtNew.TransformColumn(XAxis, XAxisTransformation);
            }

            return dtNew.GetCSVArray();
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

            this.Response = argHelper.ArgumentLoader(nameof(Response), Response);
            this.ResponseTransformation = argHelper.ArgumentLoader(nameof(ResponseTransformation), ResponseTransformation);
            this.XAxis = argHelper.ArgumentLoader(nameof(XAxis), XAxis);
            this.XAxisTransformation = argHelper.ArgumentLoader(nameof(XAxisTransformation), XAxisTransformation);
            this.FirstCatFactor = argHelper.ArgumentLoader(nameof(FirstCatFactor), FirstCatFactor);
            this.SecondCatFactor = argHelper.ArgumentLoader(nameof(SecondCatFactor), SecondCatFactor);
            this.StyleType = (GraphStyleType)Enum.Parse(typeof(GraphStyleType), argHelper.ArgumentLoader(nameof(StyleType), String.Empty), true);
            this.DisplayLegend = argHelper.ArgumentLoader(nameof(DisplayLegend), DisplayLegend);
            this.BoxPlotIncludeData = argHelper.ArgumentLoader(nameof(BoxPlotIncludeData), BoxPlotIncludeData);
            this.SEMPlotIncludeData = argHelper.ArgumentLoader(nameof(SEMPlotIncludeData), SEMPlotIncludeData);
            this.MainTitle = argHelper.ArgumentLoader(nameof(MainTitle), MainTitle);
            this.XAxisTitle = argHelper.ArgumentLoader(nameof(XAxisTitle), XAxisTitle);
            this.YAxisTitle = argHelper.ArgumentLoader(nameof(YAxisTitle), YAxisTitle);
            this.ScatterplotSelected = argHelper.ArgumentLoader(nameof(ScatterplotSelected), ScatterplotSelected);
            this.LinearFitSelected = argHelper.ArgumentLoader(nameof(LinearFitSelected), LinearFitSelected);
            this.JitterSelected = argHelper.ArgumentLoader(nameof(JitterSelected), JitterSelected);
            this.BoxplotSelected = argHelper.ArgumentLoader(nameof(BoxplotSelected), BoxplotSelected);
            this.OutliersSelected = argHelper.ArgumentLoader(nameof(OutliersSelected), OutliersSelected);
            this.SEMPlotSelected = argHelper.ArgumentLoader(nameof(SEMPlotSelected), SEMPlotSelected);
            this.SEMType = (SEMPlotType)Enum.Parse(typeof(SEMPlotType), argHelper.ArgumentLoader(nameof(SEMType), String.Empty), true);
            this.HistogramSelected = argHelper.ArgumentLoader(nameof(HistogramSelected), HistogramSelected);
            this.NormalDistSelected = argHelper.ArgumentLoader(nameof(NormalDistSelected), NormalDistSelected);
            this.CaseProfilesPlotSelected = argHelper.ArgumentLoader(nameof(CaseProfilesPlotSelected), CaseProfilesPlotSelected);
            this.CaseIDFactor = argHelper.ArgumentLoader(nameof(CaseIDFactor), CaseIDFactor);
            this.ReferenceLine = argHelper.ArgumentLoader(nameof(ReferenceLine), ReferenceLine);
        }

        public override string GetCommandLineArguments()
        {
            ArgumentFormatter argFormatter = new ArgumentFormatter();
            StringBuilder arguments = new StringBuilder();

            //X-Axis variable
            arguments.Append(" " + argFormatter.GetFormattedArgument(XAxis, true)); //4

            //X-Axis transformation
            arguments.Append(" " + argFormatter.GetFormattedArgument(XAxisTransformation)); //5

            //Response variable
            arguments.Append(" " + argFormatter.GetFormattedArgument(Response, true)); //6

            //Response transformation
            arguments.Append(" " + argFormatter.GetFormattedArgument(ResponseTransformation)); //7

            //1st cat factor
            arguments.Append(" " + argFormatter.GetFormattedArgument(FirstCatFactor, true)); //8

            //2nd cat factor
            arguments.Append(" " + argFormatter.GetFormattedArgument(SecondCatFactor, true)); //9

            //Graph type
            arguments.Append(" " + argFormatter.GetFormattedArgument(StyleType.ToString())); //10

            //Main title
            arguments.Append(" " + argFormatter.GetFormattedArgument(MainTitle)); //11

            //X-Axis title
            arguments.Append(" " + argFormatter.GetFormattedArgument(XAxisTitle)); //12

            //Y Axis title
            arguments.Append(" " + argFormatter.GetFormattedArgument(YAxisTitle)); //13

            //Scatterplot
            arguments.Append(" " + argFormatter.GetFormattedArgument(ScatterplotSelected)); //14

            //Linear plot
            arguments.Append(" " + argFormatter.GetFormattedArgument(LinearFitSelected)); //15

            //Jitter
            arguments.Append(" " + argFormatter.GetFormattedArgument(JitterSelected)); //16

            //Boxplot
            arguments.Append(" " + argFormatter.GetFormattedArgument(BoxplotSelected)); //17

            //Outliers
            arguments.Append(" " + argFormatter.GetFormattedArgument(OutliersSelected)); //18

            //SEM Plot
            arguments.Append(" " + argFormatter.GetFormattedArgument(SEMPlotSelected)); //19

            //Column plot type
            arguments.Append(" " + argFormatter.GetFormattedArgument(SEMType.ToString())); //20

            //Histogram plot
            arguments.Append(" " + argFormatter.GetFormattedArgument(HistogramSelected)); //21

            //Normal distribution fit
            arguments.Append(" " + argFormatter.GetFormattedArgument(NormalDistSelected)); //22

            //Case profiles plot
            arguments.Append(" " + argFormatter.GetFormattedArgument(CaseProfilesPlotSelected)); //23

            //Case ID Factor
            arguments.Append(" " + argFormatter.GetFormattedArgument(CaseIDFactor)); //24

            //Reference Line
            arguments.Append(" " + argFormatter.GetFormattedArgument(ReferenceLine)); //25

            //Legend
            arguments.Append(" " + argFormatter.GetFormattedArgument(DisplayLegend)); //26

            //Box Plot include data
            arguments.Append(" " + argFormatter.GetFormattedArgument(BoxPlotIncludeData)); //27

            //SEM Plot include data
            arguments.Append(" " + argFormatter.GetFormattedArgument(SEMPlotIncludeData)); //28

            return arguments.ToString();
        }
    }
}