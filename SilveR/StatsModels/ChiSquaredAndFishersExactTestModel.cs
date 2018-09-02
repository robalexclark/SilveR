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
    public class ChiSquaredAndFishersExactTestModel : IAnalysisModel
    {
        public string ScriptFileName { get { return "ChiSquaredAndFishersExactTest"; } }

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

        [Required]
        [CheckUsedOnceOnly]
        public string GroupingFactor { get; set; }

        [Required]
        [CheckUsedOnceOnly]
        public string ResponseCategories { get; set; }


        [DisplayName("Chi-squared test")]
        public bool ChiSquaredTest { get; set; } = true;

        [DisplayName("Fisher's Exact test")]
        public bool FishersExactTest { get; set; } = true;

        [Display(Name = "Hypothesis")]
        public string Hypothesis { get; set; } = "0.05";

        public List<string> HypothesesList
        {
            get { return new List<string>() { "Two-sided", "Less-than", "Greater-than" }; }
        }

        [DisplayName("Barnard's test")]
        public bool BarnardsTest { get; set; }

        [Display(Name = "Significance")]
        public string Significance { get; set; } = "0.05";

        public List<string> SignificancesList
        {
            get { return new List<string>() { "0.1", "0.05", "0.01", "0.001" }; }
        }


        public ChiSquaredAndFishersExactTestModel() { }

        public ChiSquaredAndFishersExactTestModel(Dataset dataset)
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
            ChiSquaredAndFishersExactTestValidator chiSquaredAndFishersExactTestValidator = new ChiSquaredAndFishersExactTestValidator(this);
            return chiSquaredAndFishersExactTestValidator.Validate();
        }

        public string[] ExportData()
        {
            DataTable dtNew = dataTable.CopyForExport();

            //Get the response, treatment and covariate columns by removing all other columns from the new datatable
            foreach (string col in dtNew.GetVariableNames())
            {
                if (Response != col && GroupingFactor != col && ResponseCategories != col)
                {
                    dtNew.Columns.Remove(col);
                }
            }

            //ensure that all data is trimmed
            dtNew.TrimAllDataInDataTable();

            //if the response is blank then remove that row
            dtNew.RemoveBlankRow(Response);

            return dtNew.GetCSVArray();
        }

        public IEnumerable<Argument> GetArguments()
        {
            List<Argument> args = new List<Argument>();

            args.Add(ArgumentHelper.ArgumentFactory(nameof(Response), Response));
            args.Add(ArgumentHelper.ArgumentFactory(nameof(GroupingFactor), GroupingFactor));
            args.Add(ArgumentHelper.ArgumentFactory(nameof(ResponseCategories), ResponseCategories));
            args.Add(ArgumentHelper.ArgumentFactory(nameof(ChiSquaredTest), ChiSquaredTest));
            args.Add(ArgumentHelper.ArgumentFactory(nameof(FishersExactTest), FishersExactTest));
            args.Add(ArgumentHelper.ArgumentFactory(nameof(Hypothesis), Hypothesis));
            args.Add(ArgumentHelper.ArgumentFactory(nameof(BarnardsTest), BarnardsTest));
            args.Add(ArgumentHelper.ArgumentFactory(nameof(Significance), Significance));

            return args;
        }

        public void LoadArguments(IEnumerable<Argument> arguments)
        {
            ArgumentHelper argHelper = new ArgumentHelper(arguments);

            this.Response = argHelper.ArgumentLoader(nameof(Response), Response);
            this.GroupingFactor = argHelper.ArgumentLoader(nameof(GroupingFactor), GroupingFactor);
            this.ResponseCategories = argHelper.ArgumentLoader(nameof(ResponseCategories), ResponseCategories);
            this.ChiSquaredTest = argHelper.ArgumentLoader(nameof(ChiSquaredTest), ChiSquaredTest);
            this.FishersExactTest = argHelper.ArgumentLoader(nameof(FishersExactTest), FishersExactTest);
            this.Hypothesis = argHelper.ArgumentLoader(nameof(Hypothesis), Hypothesis);
            this.BarnardsTest = argHelper.ArgumentLoader(nameof(BarnardsTest), BarnardsTest);
            this.Significance = argHelper.ArgumentLoader(nameof(Significance), Significance);
        }

        public string GetCommandLineArguments()
        {
            StringBuilder arguments = new StringBuilder();

            arguments.Append(" " + ArgumentConverters.GetNULLOrText(ArgumentConverters.ConvertIllegalChars(Response))); //4
            arguments.Append(" " + ArgumentConverters.GetNULLOrText(ArgumentConverters.ConvertIllegalChars(GroupingFactor))); //5
            arguments.Append(" " + ArgumentConverters.GetNULLOrText(ArgumentConverters.ConvertIllegalChars(ResponseCategories))); //6          

            arguments.Append(" " + ArgumentConverters.GetYesOrNo(ChiSquaredTest)); //7
            arguments.Append(" " + ArgumentConverters.GetYesOrNo(FishersExactTest)); //8
            arguments.Append(" " + Hypothesis); //9
            arguments.Append(" " + ArgumentConverters.GetYesOrNo(BarnardsTest)); //10
            arguments.Append(" " + Significance); //11

            arguments.Append(" " + "N");

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

                if (memberName != "GroupingFactor")
                    checker.AddVar(this.GroupingFactor);

                if (memberName != "ResponseCategories")
                    checker.AddVar(this.ResponseCategories);

                return checker.DoCheck(varToBeChecked);
            }
            else
            {
                return true;
            }
        }
    }
}