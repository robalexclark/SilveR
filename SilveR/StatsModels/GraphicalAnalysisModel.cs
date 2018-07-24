using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq;
using System.Text;
using SilveRModel.Helpers;
using SilveRModel.Models;
using SilveRModel.Validators;

namespace SilveRModel.StatsModel
{
    public class GraphicalAnalysisModel : IAnalysisModel
    {
        public string ScriptFileName { get { return "GraphicalAnalysis"; } }

        private DataTable dataTable;
        public DataTable DataTable
        {
            get { return dataTable; }
        }

        public Nullable<int> DatasetID { get; set; }

        private IEnumerable<string> availableVariables = new List<string>();
        public IEnumerable<string> AvailableVariables
        {
            get { return availableVariables; }
        }

        public IEnumerable<string> AvailableVariablesAllowNull
        {
            get
            {
                List<string> availableVars = availableVariables.ToList();
                availableVars.Insert(0, String.Empty);
                return availableVars.AsEnumerable();
            }
        }

        [Display(Name = "Response")]
        [Required]
        [CheckUsedOnceOnly]
        public string Response { get; set; }

        [Display(Name = "Response transformation")]
        public string ResponseTransformation { get; set; } = "None";

        public List<string> TransformationsList
        {
            get { return new List<string>() { "None", "Log10", "Loge", "Square Root", "ArcSine" }; }
        }

        [Display(Name = "X-axis variable")]
        [ValidateXAxis]
        [CheckUsedOnceOnly]
        public string XAxis { get; set; }

        [Display(Name = "X-axis transformation")]
        public string XAxisTransformation { get; set; } = "None";

        [DisplayName("1st categorisation factor")]
        [CheckUsedOnceOnly]
        public string FirstCatFactor { get; set; }

        [DisplayName("2nd categorisation factor")]
        [CheckUsedOnceOnly]
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

        [DisplayName("Histogram plot ")]
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


        public GraphicalAnalysisModel() { }

        public GraphicalAnalysisModel(Dataset dataset)
        {
            //setup model
            ReInitialize(dataset);
        }

        public void ReInitialize(Dataset dataset)
        {
            this.DatasetID = dataset.DatasetID;
            dataTable = dataset.DatasetToDataTable();

            availableVariables = dataTable.GetVariableNames();
        }

        public ValidationInfo Validate()
        {
            GraphicalAnalysisValidator graphicalAnalysisValidator = new GraphicalAnalysisValidator(this);
            return graphicalAnalysisValidator.Validate();
        }

        public string[] ExportData()
        {
            DataTable dtNew = dataTable.CopyForExport();

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

        public IEnumerable<Argument> GetArguments()
        {
            List<Argument> args = new List<Argument>();

            args.Add(ArgumentHelper.ArgumentFactory("Response", Response));
            args.Add(ArgumentHelper.ArgumentFactory("ResponseTransformation", ResponseTransformation));
            args.Add(ArgumentHelper.ArgumentFactory("XAxis", XAxis));
            args.Add(ArgumentHelper.ArgumentFactory("XAxisTransformation", XAxisTransformation));
            args.Add(ArgumentHelper.ArgumentFactory("FirstCatFactor", FirstCatFactor));
            args.Add(ArgumentHelper.ArgumentFactory("SecondCatFactor", SecondCatFactor));
            args.Add(ArgumentHelper.ArgumentFactory("StyleType", StyleType.ToString()));
            args.Add(ArgumentHelper.ArgumentFactory("DisplayLegend", DisplayLegend));
            args.Add(ArgumentHelper.ArgumentFactory("BoxPlotIncludeData", BoxPlotIncludeData));
            args.Add(ArgumentHelper.ArgumentFactory("SEMPlotIncludeData", SEMPlotIncludeData));
            args.Add(ArgumentHelper.ArgumentFactory("MainTitle", MainTitle));
            args.Add(ArgumentHelper.ArgumentFactory("XAxisTitle", XAxisTitle));
            args.Add(ArgumentHelper.ArgumentFactory("YAxisTitle", YAxisTitle));
            args.Add(ArgumentHelper.ArgumentFactory("ScatterplotSelected", ScatterplotSelected));
            args.Add(ArgumentHelper.ArgumentFactory("LinearFitSelected", LinearFitSelected));
            args.Add(ArgumentHelper.ArgumentFactory("JitterSelected", JitterSelected));
            args.Add(ArgumentHelper.ArgumentFactory("BoxplotSelected", BoxplotSelected));
            args.Add(ArgumentHelper.ArgumentFactory("OutliersSelected", OutliersSelected));
            args.Add(ArgumentHelper.ArgumentFactory("SEMPlotSelected", SEMPlotSelected));
            args.Add(ArgumentHelper.ArgumentFactory("SEMType", SEMType.ToString()));
            args.Add(ArgumentHelper.ArgumentFactory("HistogramSelected", HistogramSelected));
            args.Add(ArgumentHelper.ArgumentFactory("NormalDistSelected", NormalDistSelected));
            args.Add(ArgumentHelper.ArgumentFactory("CaseProfilesPlotSelected", CaseProfilesPlotSelected));
            args.Add(ArgumentHelper.ArgumentFactory("CaseIDFactor", CaseIDFactor));
            args.Add(ArgumentHelper.ArgumentFactory("ReferenceLine", ReferenceLine));

            return args;
        }

        public void LoadArguments(IEnumerable<Argument> arguments)
        {
            ArgumentHelper argHelper = new ArgumentHelper(arguments);

            this.Response = argHelper.ArgumentLoader("Response", Response);
            this.ResponseTransformation = argHelper.ArgumentLoader("ResponseTransformation", ResponseTransformation);
            this.XAxis = argHelper.ArgumentLoader("XAxis", XAxis);
            this.XAxisTransformation = argHelper.ArgumentLoader("XAxisTransformation", XAxisTransformation);
            this.FirstCatFactor = argHelper.ArgumentLoader("FirstCatFactor", FirstCatFactor);
            this.SecondCatFactor = argHelper.ArgumentLoader("SecondCatFactor", SecondCatFactor);
            this.StyleType = (GraphStyleType)Enum.Parse(typeof(GraphStyleType), argHelper.ArgumentLoader("StyleType", String.Empty), true);
            this.DisplayLegend = argHelper.ArgumentLoader("DisplayLegend", DisplayLegend);
            this.BoxPlotIncludeData = argHelper.ArgumentLoader("BoxPlotIncludeData", BoxPlotIncludeData);
            this.SEMPlotIncludeData = argHelper.ArgumentLoader("SEMPlotIncludeData", SEMPlotIncludeData);
            this.MainTitle = argHelper.ArgumentLoader("MainTitle", MainTitle);
            this.XAxisTitle = argHelper.ArgumentLoader("XAxisTitle", XAxisTitle);
            this.YAxisTitle = argHelper.ArgumentLoader("YAxisTitle", YAxisTitle);
            this.ScatterplotSelected = argHelper.ArgumentLoader("ScatterplotSelected", ScatterplotSelected);
            this.LinearFitSelected = argHelper.ArgumentLoader("LinearFitSelected", LinearFitSelected);
            this.JitterSelected = argHelper.ArgumentLoader("JitterSelected", JitterSelected);
            this.BoxplotSelected = argHelper.ArgumentLoader("BoxplotSelected", BoxplotSelected);
            this.OutliersSelected = argHelper.ArgumentLoader("OutliersSelected", OutliersSelected);
            this.SEMPlotSelected = argHelper.ArgumentLoader("SEMPlotSelected", SEMPlotSelected);
            this.SEMType = (SEMPlotType)Enum.Parse(typeof(SEMPlotType), argHelper.ArgumentLoader("SEMType", String.Empty), true);
            this.HistogramSelected = argHelper.ArgumentLoader("HistogramSelected", HistogramSelected);
            this.NormalDistSelected = argHelper.ArgumentLoader("NormalDistSelected", NormalDistSelected);
            this.CaseProfilesPlotSelected = argHelper.ArgumentLoader("CaseProfilesPlotSelected", CaseProfilesPlotSelected);
            this.CaseIDFactor = argHelper.ArgumentLoader("CaseIDFactor", CaseIDFactor);
            this.ReferenceLine = argHelper.ArgumentLoader("ReferenceLine", ReferenceLine);
        }

        public string GetCommandLineArguments()
        {
            StringBuilder arguments = new StringBuilder();

            //X-Axis variable
            arguments.Append(" " + ArgumentConverters.GetNULLOrText(ArgumentConverters.ConvertIllegalChars(XAxis))); //4

            //X-Axis transformation
            arguments.Append(" " + "\"" + XAxisTransformation + "\""); //5

            //Response variable
            arguments.Append(" " + ArgumentConverters.ConvertIllegalChars(Response)); //6

            //Response transformation
            arguments.Append(" " + "\"" + ResponseTransformation + "\""); //7

            //1st cat factor
            arguments.Append(" " + ArgumentConverters.GetNULLOrText(ArgumentConverters.ConvertIllegalChars(FirstCatFactor))); //8

            //2nd cat factor
            arguments.Append(" " + ArgumentConverters.GetNULLOrText(ArgumentConverters.ConvertIllegalChars(SecondCatFactor))); //9

            //Graph type
            arguments.Append(" " + StyleType.ToString()); //10

            //Main title
            arguments.Append(" " + ArgumentConverters.GetNULLOrText(MainTitle)); //11

            //X-Axis title
            arguments.Append(" " + ArgumentConverters.GetNULLOrText(XAxisTitle)); //12

            //Y Axis title
            arguments.Append(" " + ArgumentConverters.GetNULLOrText(YAxisTitle)); //13

            //Scatterplot
            arguments.Append(" " + ArgumentConverters.GetYesOrNo(ScatterplotSelected)); //14

            //Linear plot
            arguments.Append(" " + ArgumentConverters.GetYesOrNo(LinearFitSelected)); //15

            //Jitter
            arguments.Append(" " + ArgumentConverters.GetYesOrNo(JitterSelected)); //16

            //Boxplot
            arguments.Append(" " + ArgumentConverters.GetYesOrNo(BoxplotSelected)); //17

            //Outliers
            arguments.Append(" " + ArgumentConverters.GetYesOrNo(OutliersSelected)); //18

            //SEM Plot
            arguments.Append(" " + ArgumentConverters.GetYesOrNo(SEMPlotSelected)); //19

            //Column plot type
            arguments.Append(" " + SEMType.ToString()); //20

            //Histogram plot
            arguments.Append(" " + ArgumentConverters.GetYesOrNo(HistogramSelected)); //21

            //Normal distribution fit
            arguments.Append(" " + ArgumentConverters.GetYesOrNo(NormalDistSelected)); //22

            //Case profiles plot
            arguments.Append(" " + ArgumentConverters.GetYesOrNo(CaseProfilesPlotSelected)); //23

            //Case ID Factor
            arguments.Append(" " + ArgumentConverters.GetNULLOrText(ArgumentConverters.ConvertIllegalChars(CaseIDFactor))); //24

            //Reference Line
            arguments.Append(" " + ArgumentConverters.GetNULLOrText(ReferenceLine)); //25

            //Legend
            arguments.Append(" " + ArgumentConverters.GetYesOrNo(DisplayLegend)); //26

            //Box Plot include data
            arguments.Append(" " + ArgumentConverters.GetYesOrNo(BoxPlotIncludeData)); //27

            //SEM Plot include data
            arguments.Append(" " + ArgumentConverters.GetYesOrNo(SEMPlotIncludeData)); //28

            return arguments.ToString();
        }

        public bool VariablesUsedOnceOnly(string memberName)
        {
            object varToBeChecked = this.GetType().GetProperty(memberName).GetValue(this, null);

            if (varToBeChecked != null)
            {
                UniqueVariableChecker checker = new UniqueVariableChecker();

                if (memberName != "Response")
                    checker.AddVar(this.Response);

                if (memberName != "XAxis")
                    checker.AddVar(this.XAxis);

                if (memberName != "FirstCatFactor")
                    checker.AddVar(this.FirstCatFactor);

                if (memberName != "SecondCatFactor")
                    checker.AddVar(this.SecondCatFactor);

                return checker.DoCheck(varToBeChecked);
            }
            else
            {
                return true;
            }
        }
    }
}