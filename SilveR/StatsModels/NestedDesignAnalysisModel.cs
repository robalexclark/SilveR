using Combinatorics;
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
    public class NestedDesignAnalysisModel : IAnalysisModel
    {
        public string ScriptFileName { get { return "NestedDesignAnalysis"; } }

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

        [DisplayName("Response transformation")]
        public string ResponseTransformation { get; set; } = "None";

        public List<string> TransformationsList
        {
            get { return new List<string>() { "None", "Log10", "Loge", "Square Root", "ArcSine", "Rank" }; }
        }

        [CheckUsedOnceOnly]
        public string Covariate { get; set; }

        [DisplayName("Covariate transformation")]
        public string CovariateTransformation { get; set; } = "None";

        [CheckUsedOnceOnly]
        public string RandomFactor1 { get; set; }

        [CheckUsedOnceOnly]
        public string RandomFactor2 { get; set; }

        [CheckUsedOnceOnly]
        public string RandomFactor3 { get; set; }

        [CheckUsedOnceOnly]
        public string RandomFactor4 { get; set; }

        public string DesignOption1 { get; set; }

        public string DesignOption2 { get; set; }

        public string DesignOption3 { get; set; }

        public string DesignOption4 { get; set; }


        public string Significance { get; set; } = "0.05";

        public List<string> SignificancesList
        {
            get { return new List<string>() { "0.1", "0.05", "0.01", "0.001" }; }
        }


        public NestedDesignAnalysisModel() { }

        public NestedDesignAnalysisModel(Dataset dataset)
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
            NestedDesignAnalysisValidator nestedDesignAnalysisValidator = new NestedDesignAnalysisValidator(this);
            return nestedDesignAnalysisValidator.Validate();
        }

        public string[] ExportData()
        {
            DataTable dtNew = dataTable.CopyForExport();

            //Get the response, treatment and covariate columns by removing all other columns from the new datatable
            foreach (string columnName in dtNew.GetVariableNames())
            {
                if (Response != columnName && !Treatments.Contains(columnName) && OtherDesignFactors != null && !OtherDesignFactors.Contains(columnName) && Covariate != columnName && RandomFactor1 != columnName && RandomFactor2 != columnName && RandomFactor3 != columnName && RandomFactor4 != columnName)
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

            //get transforms
            arguments.Append(" " + "\"" + ResponseTransformation + "\""); //5

            arguments.Append(" " + "\"" + CovariateTransformation + "\""); //6

            string treats = null;
            foreach (string treat in Treatments) treats = treats + "," + ArgumentConverters.ConvertIllegalChars(treat);
            arguments.Append(" " + treats.TrimStart(',')); //7

            string blocks = null;
            if (OtherDesignFactors != null)
            {
                foreach (string otherDesign in OtherDesignFactors) blocks = blocks + "," + ArgumentConverters.ConvertIllegalChars(otherDesign);
            }

            if (String.IsNullOrEmpty(blocks)) //8
                arguments.Append(" " + "NULL");
            else
                arguments.Append(" " + blocks.TrimStart(','));

            arguments.Append(" " + ArgumentConverters.ConvertIllegalChars(ArgumentConverters.GetNULLOrText(Covariate))); //9
            arguments.Append(" " + Significance); //10

            arguments.Append(" " + ArgumentConverters.ConvertIllegalChars(ArgumentConverters.GetNULLOrText(RandomFactor1))); //11
            arguments.Append(" " + ArgumentConverters.ConvertIllegalChars(ArgumentConverters.GetNULLOrText(RandomFactor2))); //12
            arguments.Append(" " + ArgumentConverters.ConvertIllegalChars(ArgumentConverters.GetNULLOrText(RandomFactor3))); //13
            arguments.Append(" " + ArgumentConverters.ConvertIllegalChars(ArgumentConverters.GetNULLOrText(RandomFactor4))); //14

            arguments.Append(" " + ArgumentConverters.ConvertIllegalChars(ArgumentConverters.GetNULLOrText(DesignOption1))); //15
            arguments.Append(" " + ArgumentConverters.ConvertIllegalChars(ArgumentConverters.GetNULLOrText(DesignOption2))); //16
            arguments.Append(" " + ArgumentConverters.ConvertIllegalChars(ArgumentConverters.GetNULLOrText(DesignOption3))); //17
            arguments.Append(" " + ArgumentConverters.ConvertIllegalChars(ArgumentConverters.GetNULLOrText(DesignOption4))); //18

            return arguments.ToString();
        }

        public void LoadArguments(IEnumerable<Argument> arguments)
        {
            ArgumentHelper argHelper = new ArgumentHelper(arguments);

            this.Response = argHelper.ArgumentLoader(nameof(Response), Response);
            this.Treatments = argHelper.ArgumentLoader(nameof(Treatments), Treatments);
            this.OtherDesignFactors = argHelper.ArgumentLoader(nameof(OtherDesignFactors), OtherDesignFactors);
            this.ResponseTransformation = argHelper.ArgumentLoader(nameof(ResponseTransformation), ResponseTransformation);
            this.Covariate = argHelper.ArgumentLoader(nameof(Covariate), Covariate);
            this.CovariateTransformation = argHelper.ArgumentLoader(nameof(CovariateTransformation), CovariateTransformation);
            this.RandomFactor1 = argHelper.ArgumentLoader(nameof(RandomFactor1), RandomFactor1);
            this.RandomFactor2 = argHelper.ArgumentLoader(nameof(RandomFactor2), RandomFactor2);
            this.RandomFactor3 = argHelper.ArgumentLoader(nameof(RandomFactor3), RandomFactor3);
            this.RandomFactor4 = argHelper.ArgumentLoader(nameof(RandomFactor4), RandomFactor4);
            this.Significance = argHelper.ArgumentLoader(nameof(Significance), Significance);
            this.DesignOption1 = argHelper.ArgumentLoader(nameof(DesignOption1), DesignOption1);
            this.DesignOption2 = argHelper.ArgumentLoader(nameof(DesignOption2), DesignOption2);
            this.DesignOption3 = argHelper.ArgumentLoader(nameof(DesignOption3), DesignOption3);
            this.DesignOption4 = argHelper.ArgumentLoader(nameof(DesignOption4), DesignOption4);
        }

        public IEnumerable<Argument> GetArguments()
        {
            List<Argument> args = new List<Argument>();

            args.Add(ArgumentHelper.ArgumentFactory(nameof(Response), Response));
            args.Add(ArgumentHelper.ArgumentFactory(nameof(Treatments), Treatments));
            args.Add(ArgumentHelper.ArgumentFactory(nameof(OtherDesignFactors), OtherDesignFactors));
            args.Add(ArgumentHelper.ArgumentFactory(nameof(ResponseTransformation), ResponseTransformation));
            args.Add(ArgumentHelper.ArgumentFactory(nameof(Covariate), Covariate));
            args.Add(ArgumentHelper.ArgumentFactory(nameof(CovariateTransformation), CovariateTransformation));
            args.Add(ArgumentHelper.ArgumentFactory(nameof(RandomFactor1), RandomFactor1));
            args.Add(ArgumentHelper.ArgumentFactory(nameof(RandomFactor2), RandomFactor2));
            args.Add(ArgumentHelper.ArgumentFactory(nameof(RandomFactor3), RandomFactor3));
            args.Add(ArgumentHelper.ArgumentFactory(nameof(RandomFactor4), RandomFactor4));
            args.Add(ArgumentHelper.ArgumentFactory(nameof(Significance), Significance));
            args.Add(ArgumentHelper.ArgumentFactory(nameof(DesignOption1), DesignOption1));
            args.Add(ArgumentHelper.ArgumentFactory(nameof(DesignOption2), DesignOption2));
            args.Add(ArgumentHelper.ArgumentFactory(nameof(DesignOption3), DesignOption3));
            args.Add(ArgumentHelper.ArgumentFactory(nameof(DesignOption4), DesignOption4));

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

                if (memberName != "Covariate")
                    checker.AddVar(this.Covariate);

                if (memberName != "RandomFactor1")
                    checker.AddVar(this.RandomFactor1);

                if (memberName != "RandomFactor2")
                    checker.AddVar(this.RandomFactor2);

                if (memberName != "RandomFactor3")
                    checker.AddVar(this.RandomFactor3);

                if (memberName != "RandomFactor4")
                    checker.AddVar(this.RandomFactor4);
                
                return checker.DoCheck(varToBeChecked);
            }
            else
            {
                return true;
            }
        }
    }
}