using SilveRModel.Helpers;
using SilveRModel.Models;
using SilveRModel.Validators;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq;
using System.Text;

namespace SilveRModel.StatsModel
{
    public class LinearRegressionAnalysisModel : IAnalysisModel
    {
        public string ScriptFileName { get { return "LinearRegressionAnalysis"; } }

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

        [Required]
        [CheckUsedOnceOnly]
        public string Response { get; set; }

        [HasAtLeastOneEntry]
        [CheckUsedOnceOnly]
        public List<string> Treatments { get; set; }

        [DisplayName("Other design (blocks)")]
        [CheckUsedOnceOnly]
        public List<string> OtherDesignFactors { get; set; }

        [DisplayName("Continuous factors")]
        [CheckUsedOnceOnly]
        public List<string> ContinuousFactors { get; set; }

        [DisplayName("Continuous factors transformation")]
        public string ContinuousFactorsTransformation { get; set; } = "None";

        [DisplayName("Response transformation")]
        public string ResponseTransformation { get; set; } = "None";

        public List<string> TransformationsList
        {
            get { return new List<string>() { "None", "Log10", "Loge", "Square Root", "ArcSine", "Rank" }; }
        }

        [CheckUsedOnceOnly]
        public string Covariate { get; set; }

        [DisplayName("Primary factor")]
        public string PrimaryFactor { get; set; }

        [DisplayName("Covariate transformation")]
        public string CovariateTransformation { get; set; } = "None";


        [DisplayName("ANOVA table")]
        public bool ANOVASelected { get; set; } = true;

        public bool Coefficients { get; set; } = true;

        [DisplayName("Adjusted R-squared")]
        public bool AdjustedRSquared { get; set; }

        public string Significance { get; set; } = "0.05";

        public List<string> SignificancesList
        {
            get { return new List<string>() { "0.1", "0.05", "0.01", "0.001" }; }
        }

        [DisplayName("Residuals vs predicted plot")]
        public bool ResidualsVsPredictedPlot { get; set; }

        [DisplayName("Normal probability plot")]
        public bool NormalProbabilityPlot { get; set; }

        [DisplayName("Cook's distance plot")]
        public bool CooksDistancePlot { get; set; }

        [DisplayName("Leverage plot")]
        public bool LeveragePlot { get; set; }


        public LinearRegressionAnalysisModel() { }

        public LinearRegressionAnalysisModel(Dataset dataset)
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
            LinearRegressionAnalysisValidator linearRegressionAnalysisValidator = new LinearRegressionAnalysisValidator(this);
            return linearRegressionAnalysisValidator.Validate();
        }

        public string[] ExportData()
        {
            DataTable dtNew = dataTable.CopyForExport();

            //Get the response, treatment and covariate columns by removing all other columns from the new datatable
            foreach (string columnName in dtNew.GetVariableNames())
            {
                if (Response != columnName && !Treatments.Contains(columnName) && ContinuousFactors != null && !ContinuousFactors.Contains(columnName) && OtherDesignFactors != null && !OtherDesignFactors.Contains(columnName) && Covariate != columnName)
                {
                    dtNew.Columns.Remove(columnName);
                }
            }

            //ensure that all data is trimmed
            dtNew.TrimAllDataInDataTable();

            //if the response is blank then remove that row
            dtNew.RemoveBlankRow(Response);

            //Now do transformations...
            dtNew.TransformColumn(Response, ResponseTransformation);

            if (!String.IsNullOrEmpty(Covariate)) //check that a covariate is selected
            {
                dtNew.TransformColumn(Covariate, CovariateTransformation);
            }

            //Finally, as numeric categorical variables get misinterpreted by r, we need to go through
            //each column and put them in quotes...
            foreach (string treat in Treatments)
            {
                if (dtNew.ColumnIsNumeric(treat))
                {
                    foreach (DataRow row in dtNew.Rows)
                    {
                        row[treat] = "'" + row[treat] + "'";
                    }
                }
            }

            if (OtherDesignFactors != null)
            {
                foreach (string odf in OtherDesignFactors)
                {
                    if (dtNew.ColumnIsNumeric(odf))
                    {
                        foreach (DataRow row in dtNew.Rows)
                        {
                            row[odf] = "'" + row[odf] + "'";
                        }
                    }
                }
            }

            return dtNew.GetCSVArray();
        }


        public string GetCommandLineArguments()
        {
            StringBuilder arguments = new StringBuilder();

            //first thing to do is to assemble the model (use the GetModel method)
            arguments.Append(" " + ArgumentConverters.ConvertIllegalChars(GetModel())); //4

            //assemble a model for the covariate plot (if a covariate has been chosen)...
            if (String.IsNullOrEmpty(Covariate)) //5
            {
                arguments.Append("NULL");
            }
            else
            {
                string covariateModel = Response + "~" + Covariate;
                arguments.Append(ArgumentConverters.ConvertIllegalChars(covariateModel));
            }

            //get transforms
            arguments.Append(" " + "\"" + ResponseTransformation + "\""); //6

            arguments.Append(" " + "\"" + CovariateTransformation + "\""); //7

            arguments.Append(" " + ArgumentConverters.GetNULLOrText(ArgumentConverters.ConvertIllegalChars(PrimaryFactor))); //8

            string treats = null;
            foreach (string treat in Treatments) treats = treats + "," + ArgumentConverters.ConvertIllegalChars(treat);
            arguments.Append(" " + treats.TrimStart(',')); //9

            string contFactors = null;
            foreach (string cont in ContinuousFactors) contFactors = contFactors + "," + ArgumentConverters.ConvertIllegalChars(cont);
            arguments.Append(" " + contFactors.TrimStart(',')); //10

            arguments.Append(" " + "\"" + ContinuousFactorsTransformation + "\"");//11

            string otherFactors = null;
            foreach (string other in Treatments) otherFactors = otherFactors + "," + ArgumentConverters.ConvertIllegalChars(other);
            arguments.Append(" " + otherFactors.TrimStart(',')); //12

            arguments.Append(" " + ArgumentConverters.GetYesOrNo(ANOVASelected)); //13
            arguments.Append(" " + ArgumentConverters.GetYesOrNo(Coefficients)); //14
            arguments.Append(" " + ArgumentConverters.GetYesOrNo(AdjustedRSquared)); //15

            arguments.Append(Significance); //16

            arguments.Append(" " + ArgumentConverters.GetYesOrNo(ResidualsVsPredictedPlot)); //17

            arguments.Append(" " + ArgumentConverters.GetYesOrNo(NormalProbabilityPlot)); //18

            arguments.Append(" " + ArgumentConverters.GetYesOrNo(CooksDistancePlot)); //19

            arguments.Append(" " + ArgumentConverters.GetYesOrNo(LeveragePlot)); //20

            return arguments.ToString();
        }



        public void LoadArguments(IEnumerable<Argument> arguments)
        {
            ArgumentHelper argHelper = new ArgumentHelper(arguments);

            this.Response = argHelper.ArgumentLoader(nameof(Response), Response);
            this.Treatments = argHelper.ArgumentLoader(nameof(Treatments), Treatments);
            this.OtherDesignFactors = argHelper.ArgumentLoader(nameof(OtherDesignFactors), OtherDesignFactors);
            this.ContinuousFactors = argHelper.ArgumentLoader(nameof(ContinuousFactors), ContinuousFactors);
            this.ResponseTransformation = argHelper.ArgumentLoader(nameof(ResponseTransformation), ResponseTransformation);
            this.Covariate = argHelper.ArgumentLoader(nameof(Covariate), Covariate);
            this.PrimaryFactor = argHelper.ArgumentLoader(nameof(PrimaryFactor), PrimaryFactor);
            this.CovariateTransformation = argHelper.ArgumentLoader(nameof(CovariateTransformation), CovariateTransformation);
            this.ANOVASelected = argHelper.ArgumentLoader(nameof(ANOVASelected), ANOVASelected);
            this.Coefficients = argHelper.ArgumentLoader(nameof(Coefficients), Coefficients);
            this.AdjustedRSquared = argHelper.ArgumentLoader(nameof(AdjustedRSquared), AdjustedRSquared);
            this.Significance = argHelper.ArgumentLoader(nameof(Significance), Significance);
            this.ResidualsVsPredictedPlot = argHelper.ArgumentLoader(nameof(ResidualsVsPredictedPlot), ResidualsVsPredictedPlot);
            this.NormalProbabilityPlot = argHelper.ArgumentLoader(nameof(NormalProbabilityPlot), NormalProbabilityPlot);
            this.CooksDistancePlot = argHelper.ArgumentLoader(nameof(CooksDistancePlot), CooksDistancePlot);
            this.LeveragePlot = argHelper.ArgumentLoader(nameof(LeveragePlot), LeveragePlot);
        }

        public IEnumerable<Argument> GetArguments()
        {
            List<Argument> args = new List<Argument>();

            args.Add(ArgumentHelper.ArgumentFactory(nameof(Response), Response));
            args.Add(ArgumentHelper.ArgumentFactory(nameof(Treatments), Treatments));
            args.Add(ArgumentHelper.ArgumentFactory(nameof(OtherDesignFactors), OtherDesignFactors));
            args.Add(ArgumentHelper.ArgumentFactory(nameof(ContinuousFactors), ContinuousFactors));
            args.Add(ArgumentHelper.ArgumentFactory(nameof(ResponseTransformation), ResponseTransformation));
            args.Add(ArgumentHelper.ArgumentFactory(nameof(Covariate), Covariate));
            args.Add(ArgumentHelper.ArgumentFactory(nameof(PrimaryFactor), PrimaryFactor));
            args.Add(ArgumentHelper.ArgumentFactory(nameof(CovariateTransformation), CovariateTransformation));
            args.Add(ArgumentHelper.ArgumentFactory(nameof(ANOVASelected), ANOVASelected));
            args.Add(ArgumentHelper.ArgumentFactory(nameof(Coefficients), Coefficients));
            args.Add(ArgumentHelper.ArgumentFactory(nameof(AdjustedRSquared), AdjustedRSquared));
            args.Add(ArgumentHelper.ArgumentFactory(nameof(Significance), Significance));
            args.Add(ArgumentHelper.ArgumentFactory(nameof(ResidualsVsPredictedPlot), ResidualsVsPredictedPlot));
            args.Add(ArgumentHelper.ArgumentFactory(nameof(NormalProbabilityPlot), NormalProbabilityPlot));
            args.Add(ArgumentHelper.ArgumentFactory(nameof(CooksDistancePlot), CooksDistancePlot));
            args.Add(ArgumentHelper.ArgumentFactory(nameof(LeveragePlot), LeveragePlot));

            return args;
        }



        private string GetModel()
        {
            //assemble the model from the information in the treatment, other factors, response and covariate boxes
            string model = Response + "~";

            if (!String.IsNullOrEmpty(Covariate))
                model = model + Covariate + "+";

            if (OtherDesignFactors != null)
            {
                foreach (string otherDesign in OtherDesignFactors)
                    model = model + otherDesign + "+";
            }

            foreach (string treat in Treatments)
                model = model + treat + "+";

            model = model.TrimEnd('+');

            return model;
        }


        public bool VariablesUsedOnceOnly(string memberName)
        {
            object varToBeChecked = this.GetType().GetProperty(memberName).GetValue(this, null);

            if (varToBeChecked != null)
            {
                UniqueVariableChecker checker = new UniqueVariableChecker();

                if (memberName != "Response")
                    checker.AddVar(this.Response);

                if (memberName != "Treatments")
                    checker.AddVars(this.Treatments);

                if (memberName != "OtherDesignFactors")
                    checker.AddVars(this.OtherDesignFactors);

                if (memberName != "ContinuousFactors")
                    checker.AddVars(this.ContinuousFactors);

                if (memberName != "Covariate")
                    checker.AddVar(this.Covariate);

                return checker.DoCheck(varToBeChecked);
            }
            else
            {
                return true;
            }
        }
    }
}