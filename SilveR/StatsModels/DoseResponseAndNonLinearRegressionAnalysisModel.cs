using SilveR.Helpers;
using SilveR.Models;
using SilveR.Validators;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Text;

namespace SilveR.StatsModels
{
    public class DoseResponseAndNonLinearRegressionAnalysisModel : AnalysisDataModelBase
    {
        public enum AnalysisOption { FourParameter = 0, Equation = 1 }
        [DisplayName("Analysis Option")]
        public AnalysisOption AnalysisType { get; set; } = AnalysisOption.FourParameter;

        [CheckUsedOnceOnly]
        [ValidateResponseOrDoseAttribute]
        [DisplayName("Response")]
        public string Response { get; set; }

        [DisplayName("Response transformation")]
        public string ResponseTransformation { get; set; } = "None";

        public IEnumerable<string> TransformationsList
        {
            get { return new List<string>() { "None", "Log10", "Loge", "Square Root", "ArcSine", "Rank" }; }
        }

        [CheckUsedOnceOnly]
        [ValidateResponseOrDoseAttribute]
        [DisplayName("Dose")]
        public string Dose { get; set; }

        public enum DoseScaleOption { Log10 = 0, Loge = 1 }
        [DisplayName("Dose scale")]
        public DoseScaleOption DoseScale {get; set; } = DoseScaleOption.Log10;

        [DisplayName("Offset")]
        public Nullable<decimal> Offset { get; set; }

        [CheckUsedOnceOnly]
        [DisplayName("QC response")]
        public string QCResponse { get; set; }

        [CheckUsedOnceOnly]
        [DisplayName("QC dose")]
        public string QCDose { get; set; }

        [CheckUsedOnceOnly]
        [DisplayName("Samples response")]
        public string SamplesResponse { get; set; }

        [DisplayName("Min coefficient")]
        public Nullable<decimal> MinCoeff { get; set; }

        [DisplayName("Max coefficient")]
        public Nullable<decimal> MaxCoeff { get; set; }

        [DisplayName("Slope coefficient")]
        public Nullable<decimal> SlopeCoeff { get; set; }

        [DisplayName("EDICC coefficient")]
        public Nullable<decimal> EDICCoeff { get; set; }

        [DisplayName("Min start value")]
        public Nullable<decimal> MinStartValue { get; set; }

        [DisplayName("Max start value")]
        public Nullable<decimal> MaxStartValue { get; set; }

        [DisplayName("Slope start value")]
        public Nullable<decimal> SlopeStartValue { get; set; }

        [DisplayName("EDICC start value")]
        public Nullable<decimal> EDICStartValue { get; set; }

        [DisplayName("Equation")]
        public string Equation { get; set; }

        [DisplayName("Start values")]
        public string StartValues { get; set; }

        [CheckUsedOnceOnly]
        [DisplayName("Y-axis variable")]
        public string EquationYAxis { get; set; }

        [CheckUsedOnceOnly]
        [DisplayName("X-axis variable")]
        public string EquationXAxis { get; set; }


        public DoseResponseAndNonLinearRegressionAnalysisModel() : base("DoseResponseAndNonLinearRegressionAnalysis") { }

        public DoseResponseAndNonLinearRegressionAnalysisModel(IDataset dataset)
            : base(dataset, "DoseResponseAndNonLinearRegressionAnalysis") { }


        public override ValidationInfo Validate()
        {
            DoseResponseAndNonLinearRegressionAnalysisValidator doseResponseAndNonLinearRegressionAnalysisValidator = new DoseResponseAndNonLinearRegressionAnalysisValidator(this);
            return doseResponseAndNonLinearRegressionAnalysisValidator.Validate();
        }

        public override string[] ExportData()
        {
            DataTable dtNew = DataTable.CopyForExport();

            //Get the response and treatment columns
            foreach (string columnName in dtNew.GetVariableNames())
            {
                if (Response != columnName && Dose != columnName && QCResponse != columnName && QCDose != columnName && SamplesResponse != columnName && EquationYAxis != columnName && EquationXAxis != columnName)
                {
                    dtNew.Columns.Remove(columnName);
                }
            }

            if (AnalysisType == AnalysisOption.FourParameter)
            {
                dtNew.RemoveBlankRow(Response);
            }
            else
            {
                //if the y axis column is blank then remove that row
                dtNew.RemoveBlankRow(EquationYAxis);
            }

            //Now do transformations...
            if (!String.IsNullOrEmpty(Response)) //transform ResponseVar
            {
                dtNew.TransformColumn(Response, ResponseTransformation);
            }

            if (!String.IsNullOrEmpty(QCResponse)) //transform QCResponseVar
            {
                dtNew.TransformColumn(QCResponse, ResponseTransformation);
            }

            if (!String.IsNullOrEmpty(SamplesResponse)) //transform SamplesVar
            {
                dtNew.TransformColumn(SamplesResponse, ResponseTransformation);
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

            args.Add(ArgumentHelper.ArgumentFactory(nameof(AnalysisType), AnalysisType.ToString()));
            args.Add(ArgumentHelper.ArgumentFactory(nameof(Response), Response));
            args.Add(ArgumentHelper.ArgumentFactory(nameof(ResponseTransformation), ResponseTransformation));
            args.Add(ArgumentHelper.ArgumentFactory(nameof(Dose), Dose));
            args.Add(ArgumentHelper.ArgumentFactory(nameof(DoseScale), DoseScale.ToString()));
            args.Add(ArgumentHelper.ArgumentFactory(nameof(Offset), Offset));
            args.Add(ArgumentHelper.ArgumentFactory(nameof(QCResponse), QCResponse));
            args.Add(ArgumentHelper.ArgumentFactory(nameof(QCDose), QCDose));
            args.Add(ArgumentHelper.ArgumentFactory(nameof(SamplesResponse), SamplesResponse));
            args.Add(ArgumentHelper.ArgumentFactory(nameof(MinCoeff), MinCoeff));
            args.Add(ArgumentHelper.ArgumentFactory(nameof(MaxCoeff), MaxCoeff));
            args.Add(ArgumentHelper.ArgumentFactory(nameof(SlopeCoeff), SlopeCoeff));
            args.Add(ArgumentHelper.ArgumentFactory(nameof(EDICCoeff), EDICCoeff));
            args.Add(ArgumentHelper.ArgumentFactory(nameof(MinStartValue), MinStartValue));
            args.Add(ArgumentHelper.ArgumentFactory(nameof(MaxStartValue), MaxStartValue));
            args.Add(ArgumentHelper.ArgumentFactory(nameof(SlopeStartValue), SlopeStartValue));
            args.Add(ArgumentHelper.ArgumentFactory(nameof(EDICStartValue), EDICStartValue));
            args.Add(ArgumentHelper.ArgumentFactory(nameof(Equation), Equation));
            args.Add(ArgumentHelper.ArgumentFactory(nameof(StartValues), StartValues));
            args.Add(ArgumentHelper.ArgumentFactory(nameof(EquationYAxis), EquationYAxis));
            args.Add(ArgumentHelper.ArgumentFactory(nameof(EquationXAxis), EquationXAxis));

            return args;
        }

        public override void LoadArguments(IEnumerable<Argument> arguments)
        {
            ArgumentHelper argHelper = new ArgumentHelper(arguments);

            this.AnalysisType = (AnalysisOption)Enum.Parse(typeof(AnalysisOption), argHelper.LoadStringArgument(nameof(AnalysisType)), true);
            this.Response = argHelper.LoadStringArgument(nameof(Response));
            this.ResponseTransformation = argHelper.LoadStringArgument(nameof(ResponseTransformation));
            this.Dose = argHelper.LoadStringArgument(nameof(Dose));
            this.DoseScale = (DoseScaleOption)Enum.Parse(typeof(DoseScaleOption), argHelper.LoadStringArgument(nameof(DoseScale)), true);
            this.Offset = argHelper.LoadNullableDecimalArgument(nameof(Offset));
            this.QCResponse = argHelper.LoadStringArgument(nameof(QCResponse));
            this.QCDose = argHelper.LoadStringArgument(nameof(QCDose));
            this.SamplesResponse = argHelper.LoadStringArgument(nameof(SamplesResponse));
            this.MinCoeff = argHelper.LoadNullableDecimalArgument(nameof(MinCoeff));
            this.MaxCoeff = argHelper.LoadNullableDecimalArgument(nameof(MaxCoeff));
            this.SlopeCoeff = argHelper.LoadNullableDecimalArgument(nameof(SlopeCoeff));
            this.EDICCoeff = argHelper.LoadNullableDecimalArgument(nameof(EDICCoeff));
            this.MinStartValue = argHelper.LoadNullableDecimalArgument(nameof(MinStartValue));
            this.MaxStartValue = argHelper.LoadNullableDecimalArgument(nameof(MaxStartValue));
            this.SlopeStartValue = argHelper.LoadNullableDecimalArgument(nameof(SlopeStartValue));
            this.EDICStartValue = argHelper.LoadNullableDecimalArgument(nameof(EDICStartValue));
            this.Equation = argHelper.LoadStringArgument(nameof(Equation));
            this.StartValues = argHelper.LoadStringArgument(nameof(StartValues));
            this.EquationYAxis = argHelper.LoadStringArgument(nameof(EquationYAxis));
            this.EquationXAxis = argHelper.LoadStringArgument(nameof(EquationXAxis));
        }

        public override string GetCommandLineArguments()
        {
            ArgumentFormatter argFormatter = new ArgumentFormatter();
            StringBuilder arguments = new StringBuilder();

            if (AnalysisType == AnalysisOption.FourParameter) //4
            {
                arguments.Append(" " + "FourParameter");
            }
            else
            {
                arguments.Append(" " + "Equation");
            }

            arguments.Append(" " + argFormatter.GetFormattedArgument(Response, true)); //5

            arguments.Append(" " + argFormatter.GetFormattedArgument(ResponseTransformation, false)); //6
            arguments.Append(" " + argFormatter.GetFormattedArgument(Dose, true)); //7

            //get the checkbox setting
            arguments.Append(" " + argFormatter.GetFormattedArgument(Offset.ToString(), false)); //8

            arguments.Append(" " + argFormatter.GetFormattedArgument(DoseScale.ToString(), false)); //9

            arguments.Append(" " + argFormatter.GetFormattedArgument(QCResponse, true)); //10
            arguments.Append(" " + argFormatter.GetFormattedArgument(QCDose, true)); //11
            arguments.Append(" " + argFormatter.GetFormattedArgument(SamplesResponse, true)); //12

            arguments.Append(" " + argFormatter.GetFormattedArgument(MinCoeff.ToString(), false)); //13
            arguments.Append(" " + argFormatter.GetFormattedArgument(MaxCoeff.ToString(), false)); //14
            arguments.Append(" " + argFormatter.GetFormattedArgument(SlopeCoeff.ToString(), false)); //15
            arguments.Append(" " + argFormatter.GetFormattedArgument(EDICCoeff.ToString(), false)); //16

            arguments.Append(" " + argFormatter.GetFormattedArgument(MinStartValue.ToString(), false)); //17
            arguments.Append(" " + argFormatter.GetFormattedArgument(MaxStartValue.ToString(), false)); //18
            arguments.Append(" " + argFormatter.GetFormattedArgument(SlopeStartValue.ToString(), false)); //19
            arguments.Append(" " + argFormatter.GetFormattedArgument(EDICStartValue.ToString(), false)); //20

            arguments.Append(" " + argFormatter.GetFormattedArgument(Equation, false)); //21
            arguments.Append(" " + argFormatter.GetFormattedArgument(StartValues, false)); //22

            arguments.Append(" " + argFormatter.GetFormattedArgument(EquationYAxis, true)); //23
            arguments.Append(" " + argFormatter.GetFormattedArgument(EquationXAxis, true)); //24

            return arguments.ToString().Trim();
        }
    }
}