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
    public class NestedDesignAnalysisModel : AnalysisModelBase
    {
        [Required]
        [CheckUsedOnceOnly]
        [DisplayName("Response")]
        public string Response { get; set; }

        [HasAtLeastOneEntry]
        [CheckUsedOnceOnly]
        [DisplayName("Treatments")]
        public IEnumerable<string> Treatments { get; set; }

        [CheckUsedOnceOnly]
        [DisplayName("Other design (blocks)")]
        public IEnumerable<string> OtherDesignFactors { get; set; }

        [DisplayName("Response transformation")]
        public string ResponseTransformation { get; set; } = "None";

        public IEnumerable<string> TransformationsList
        {
            get { return new List<string>() { "None", "Log10", "Loge", "Square Root", "ArcSine", "Rank" }; }
        }

        [CheckUsedOnceOnly]
        [DisplayName("Covariate")]
        public string Covariate { get; set; }

        [DisplayName("Covariate transformation")]
        public string CovariateTransformation { get; set; } = "None";

        [CheckUsedOnceOnly]
        [DisplayName("Random factor 1")]
        public string RandomFactor1 { get; set; }

        [CheckUsedOnceOnly]
        [DisplayName("Random factor 2")]
        public string RandomFactor2 { get; set; }

        [CheckUsedOnceOnly]
        [DisplayName("Random factor 3")]
        public string RandomFactor3 { get; set; }

        [CheckUsedOnceOnly]
        [DisplayName("Random factor 4")]
        public string RandomFactor4 { get; set; }

        [DisplayName("Design option 1")]
        public string DesignOption1 { get; set; }

        [DisplayName("Design option 2")]
        public string DesignOption2 { get; set; }

        [DisplayName("Design option 3")]
        public string DesignOption3 { get; set; }

        [DisplayName("Design option 4")]
        public string DesignOption4 { get; set; }

        [DisplayName("Significance level")]
        public string Significance { get; set; } = "0.05";

        public IEnumerable<string> SignificancesList
        {
            get { return new List<string>() { "0.1", "0.05", "0.01", "0.001" }; }
        }


        public NestedDesignAnalysisModel() : this(null) { }

        public NestedDesignAnalysisModel(IDataset dataset)
            : base(dataset, "NestedDesignAnalysis") { }



        public override ValidationInfo Validate()
        {
            NestedDesignAnalysisValidator nestedDesignAnalysisValidator = new NestedDesignAnalysisValidator(this);
            return nestedDesignAnalysisValidator.Validate();
        }

        public override string[] ExportData()
        {
            DataTable dtNew = DataTable.CopyForExport();

            //Get the response, treatment and covariate columns by removing all other columns from the new datatable
            foreach (string columnName in dtNew.GetVariableNames())
            {
                if (Response != columnName && !Treatments.Contains(columnName) && (OtherDesignFactors == null || !OtherDesignFactors.Contains(columnName)) && Covariate != columnName && RandomFactor1 != columnName && RandomFactor2 != columnName && RandomFactor3 != columnName && RandomFactor4 != columnName)
                {
                    dtNew.Columns.Remove(columnName);
                }
            }

            //ensure that all data is trimmed
            //dtNew.TrimAllDataInDataTable();

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
                if (dtNew.CheckIsNumeric(treat))
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
                    if (dtNew.CheckIsNumeric(odf))
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


        public override string GetCommandLineArguments()
        {
            ArgumentFormatter argFormatter = new ArgumentFormatter();
            StringBuilder arguments = new StringBuilder();

            //first thing to do is to assemble the model (use the GetModel method)
            arguments.Append(" " + argFormatter.GetFormattedArgument(GetModel(), true)); //4

            //get transforms
            arguments.Append(" " + argFormatter.GetFormattedArgument(ResponseTransformation, false)); //5

            arguments.Append(" " + argFormatter.GetFormattedArgument(CovariateTransformation, false)); //6

            arguments.Append(" " + argFormatter.GetFormattedArgument(Treatments)); //7

            arguments.Append(" " + argFormatter.GetFormattedArgument(OtherDesignFactors)); //8

            arguments.Append(" " + argFormatter.GetFormattedArgument(Covariate, true)); //9
            arguments.Append(" " + argFormatter.GetFormattedArgument(Significance, false)); //10

            arguments.Append(" " + argFormatter.GetFormattedArgument(RandomFactor1, true)); //11
            arguments.Append(" " + argFormatter.GetFormattedArgument(RandomFactor2, true)); //12
            arguments.Append(" " + argFormatter.GetFormattedArgument(RandomFactor3, true)); //13
            arguments.Append(" " + argFormatter.GetFormattedArgument(RandomFactor4, true)); //14

            arguments.Append(" " + argFormatter.GetFormattedArgument(DesignOption1, true)); //15
            arguments.Append(" " + argFormatter.GetFormattedArgument(DesignOption2, true)); //16
            arguments.Append(" " + argFormatter.GetFormattedArgument(DesignOption3, true)); //17
            arguments.Append(" " + argFormatter.GetFormattedArgument(DesignOption4, true)); //18

            return arguments.ToString().Trim();
        }

        public override void LoadArguments(IEnumerable<Argument> arguments)
        {
            ArgumentHelper argHelper = new ArgumentHelper(arguments);

            this.Response = argHelper.LoadStringArgument(nameof(Response));
            this.Treatments = argHelper.LoadIEnumerableArgument(nameof(Treatments));
            this.OtherDesignFactors = argHelper.LoadIEnumerableArgument(nameof(OtherDesignFactors));
            this.ResponseTransformation = argHelper.LoadStringArgument(nameof(ResponseTransformation));
            this.Covariate = argHelper.LoadStringArgument(nameof(Covariate));
            this.CovariateTransformation = argHelper.LoadStringArgument(nameof(CovariateTransformation));
            this.RandomFactor1 = argHelper.LoadStringArgument(nameof(RandomFactor1));
            this.RandomFactor2 = argHelper.LoadStringArgument(nameof(RandomFactor2));
            this.RandomFactor3 = argHelper.LoadStringArgument(nameof(RandomFactor3));
            this.RandomFactor4 = argHelper.LoadStringArgument(nameof(RandomFactor4));
            this.Significance = argHelper.LoadStringArgument(nameof(Significance));
            this.DesignOption1 = argHelper.LoadStringArgument(nameof(DesignOption1));
            this.DesignOption2 = argHelper.LoadStringArgument(nameof(DesignOption2));
            this.DesignOption3 = argHelper.LoadStringArgument(nameof(DesignOption3));
            this.DesignOption4 = argHelper.LoadStringArgument(nameof(DesignOption4));
        }

        public override IEnumerable<Argument> GetArguments()
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
    }
}