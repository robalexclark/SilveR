using SilveR.Helpers;
using SilveR.Models;
using SilveR.Validators;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Data;
using System.Linq;
using System.Text;

namespace SilveR.StatsModels
{
    public class HasseDiagramGeneratorModel : AnalysisDataModelBase
    {
        [Display(Name = "Fixed factors")]
        [CheckUsedOnceOnly(SingularizeDisplayName = false)]
        public IEnumerable<string> FixedFactors { get; set; }

        [Display(Name = "Random factors")]
        [CheckUsedOnceOnly(SingularizeDisplayName = false)]
        public IEnumerable<string> RandomFactors { get; set; }

        public bool CheckForConfoundedDegreesOfFreedom { get; set; } = false;

        public string ObjectColour { get; set; } = "blue";

        public bool ShowPartialCrossing { get; set; } = true;

        public bool ShowDegreesOfFreedom { get; set; } = true;

        public bool ShowMaximumLevels { get; set; } = true;

        public string FontColour { get; set; } = "red";

        public string StructuralLineColour { get; set; } = "grey";

        public decimal StructuralLineWidth { get; set; } = 2m;

        public string PartialCrossingLineColour { get; set; } = "orange";

        public decimal PartialCrossingLineWidth { get; set; } = 1.5m;

        public decimal SmallObjectFontSize { get; set; } = 1m;

        public decimal MediumObjectFontSize { get; set; } = 1m;

        public decimal LargeObjectFontSize { get; set; } = 1m;

        public HasseDiagramGeneratorModel() : base("HasseDiagramGenerator") { }

        public HasseDiagramGeneratorModel(IDataset dataset) : base(dataset, "HasseDiagramGenerator") { }

        public override ValidationInfo Validate()
        {
            HasseDiagramGeneratorValidator hasseDiagramGeneratorValidator = new HasseDiagramGeneratorValidator(this);
            return hasseDiagramGeneratorValidator.Validate();
        }

        public override string[] ExportData()
        {
            DataTable exportData = DataTable.CopyForExport();
            IEnumerable<string> selectedFactors = (FixedFactors ?? Enumerable.Empty<string>()).Concat(RandomFactors ?? Enumerable.Empty<string>());

            foreach (string columnName in exportData.GetVariableNames())
            {
                if (!selectedFactors.Contains(columnName))
                {
                    exportData.Columns.Remove(columnName);
                }
            }

            string[] csvArray = exportData.GetCSVArray();
            ArgumentFormatter argumentFormatter = new ArgumentFormatter();
            csvArray[0] = argumentFormatter.ConvertCsvHeader(csvArray[0]);
            return csvArray;
        }

        public override IEnumerable<Argument> GetArguments()
        {
            return new List<Argument>
            {
                ArgumentHelper.ArgumentFactory(nameof(FixedFactors), FixedFactors),
                ArgumentHelper.ArgumentFactory(nameof(RandomFactors), RandomFactors),
                ArgumentHelper.ArgumentFactory(nameof(CheckForConfoundedDegreesOfFreedom), CheckForConfoundedDegreesOfFreedom),
                ArgumentHelper.ArgumentFactory(nameof(ObjectColour), ObjectColour),
                ArgumentHelper.ArgumentFactory(nameof(ShowPartialCrossing), ShowPartialCrossing),
                ArgumentHelper.ArgumentFactory(nameof(ShowDegreesOfFreedom), ShowDegreesOfFreedom),
                ArgumentHelper.ArgumentFactory(nameof(ShowMaximumLevels), ShowMaximumLevels),
                ArgumentHelper.ArgumentFactory(nameof(FontColour), FontColour),
                ArgumentHelper.ArgumentFactory(nameof(StructuralLineColour), StructuralLineColour),
                ArgumentHelper.ArgumentFactory(nameof(StructuralLineWidth), StructuralLineWidth),
                ArgumentHelper.ArgumentFactory(nameof(PartialCrossingLineColour), PartialCrossingLineColour),
                ArgumentHelper.ArgumentFactory(nameof(PartialCrossingLineWidth), PartialCrossingLineWidth),
                ArgumentHelper.ArgumentFactory(nameof(SmallObjectFontSize), SmallObjectFontSize),
                ArgumentHelper.ArgumentFactory(nameof(MediumObjectFontSize), MediumObjectFontSize),
                ArgumentHelper.ArgumentFactory(nameof(LargeObjectFontSize), LargeObjectFontSize)
            };
        }

        public override void LoadArguments(IEnumerable<Argument> arguments)
        {
            ArgumentHelper argHelper = new ArgumentHelper(arguments);
            FixedFactors = argHelper.LoadIEnumerableArgument(nameof(FixedFactors));
            RandomFactors = argHelper.LoadIEnumerableArgument(nameof(RandomFactors));
            CheckForConfoundedDegreesOfFreedom = argHelper.LoadBooleanArgument(nameof(CheckForConfoundedDegreesOfFreedom));
            ObjectColour = argHelper.LoadStringArgument(nameof(ObjectColour));
            ShowPartialCrossing = argHelper.LoadBooleanArgument(nameof(ShowPartialCrossing));
            ShowDegreesOfFreedom = argHelper.LoadBooleanArgument(nameof(ShowDegreesOfFreedom));
            ShowMaximumLevels = argHelper.LoadBooleanArgument(nameof(ShowMaximumLevels));
            FontColour = argHelper.LoadStringArgument(nameof(FontColour));
            StructuralLineColour = argHelper.LoadStringArgument(nameof(StructuralLineColour));
            StructuralLineWidth = argHelper.LoadDecimalArgument(nameof(StructuralLineWidth));
            PartialCrossingLineColour = argHelper.LoadStringArgument(nameof(PartialCrossingLineColour));
            PartialCrossingLineWidth = argHelper.LoadDecimalArgument(nameof(PartialCrossingLineWidth));
            SmallObjectFontSize = argHelper.LoadDecimalArgument(nameof(SmallObjectFontSize));
            MediumObjectFontSize = argHelper.LoadDecimalArgument(nameof(MediumObjectFontSize));
            LargeObjectFontSize = argHelper.LoadDecimalArgument(nameof(LargeObjectFontSize));
        }

        public override string GetCommandLineArguments()
        {
            ArgumentFormatter argumentFormatter = new ArgumentFormatter();
            StringBuilder arguments = new StringBuilder();
            arguments.Append(argumentFormatter.GetFormattedArgument(FixedFactors));
            arguments.Append(" " + argumentFormatter.GetFormattedArgument(RandomFactors));
            arguments.Append(" " + argumentFormatter.GetFormattedArgument(CheckForConfoundedDegreesOfFreedom));
            arguments.Append(" " + argumentFormatter.GetFormattedArgument(ObjectColour, false));
            arguments.Append(" " + argumentFormatter.GetFormattedArgument(ShowPartialCrossing));
            arguments.Append(" " + argumentFormatter.GetFormattedArgument(ShowDegreesOfFreedom));
            arguments.Append(" " + argumentFormatter.GetFormattedArgument(ShowMaximumLevels));
            arguments.Append(" " + argumentFormatter.GetFormattedArgument(FontColour, false));
            arguments.Append(" " + argumentFormatter.GetFormattedArgument(StructuralLineColour, false));
            arguments.Append(" " + argumentFormatter.GetFormattedArgument(StructuralLineWidth.ToString(CultureInfo.InvariantCulture), false));
            arguments.Append(" " + argumentFormatter.GetFormattedArgument(PartialCrossingLineColour, false));
            arguments.Append(" " + argumentFormatter.GetFormattedArgument(PartialCrossingLineWidth.ToString(CultureInfo.InvariantCulture), false));
            arguments.Append(" " + argumentFormatter.GetFormattedArgument(SmallObjectFontSize.ToString(CultureInfo.InvariantCulture), false));
            arguments.Append(" " + argumentFormatter.GetFormattedArgument(MediumObjectFontSize.ToString(CultureInfo.InvariantCulture), false));
            arguments.Append(" " + argumentFormatter.GetFormattedArgument(LargeObjectFontSize.ToString(CultureInfo.InvariantCulture), false));
            return arguments.ToString();
        }
    }
}
