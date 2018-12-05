using SilveR.Models;
using SilveR.StatsModels;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Silver.UnitTests.StatsModels
{
    public class OneWayANOVAUserInputsModelTests
    {
        [Fact]
        public void ScriptFileName_ReturnsCorrectString()
        {
            //Arrange
            OneWayANOVAUserBasedInputsModel sut = new OneWayANOVAUserBasedInputsModel();

            //Act
            string result = sut.ScriptFileName;

            //Assert
            Assert.Equal("OneWayANOVAUserBasedInputs", result);
        }

        [Fact]
        public void SignificancesList_ReturnsCorrectList()
        {
            //Arrange
            OneWayANOVAUserBasedInputsModel sut = new OneWayANOVAUserBasedInputsModel();

            //Act
            IEnumerable<string> result = sut.SignificancesList;

            //Assert
            Assert.IsAssignableFrom<IEnumerable<string>>(result);
            Assert.Equal(new List<string>() { "0.1", "0.05", "0.01", "0.001" }, result);
        }


        [Fact]
        public void GetArguments_ReturnsCorrectArguments()
        {
            //Arrange
            OneWayANOVAUserBasedInputsModel sut = GetModel();

            //Act
            List<Argument> result = sut.GetArguments().ToList();

            //Assert
            var graphTitle = result.Single(x => x.Name == "GraphTitle");
            Assert.Equal("TitleTest", graphTitle.Value);

            var effectSizeEstimate = result.Single(x => x.Name == "EffectSizeEstimate");
            Assert.Equal("TreatmentMeanSquare", effectSizeEstimate.Value);

            var treatmentMeanSquare = result.Single(x => x.Name == "TreatmentMeanSquare");
            Assert.Equal("12.5", treatmentMeanSquare.Value);

            var means = result.Single(x => x.Name == "Means");
            Assert.Equal("55", means.Value);

            var variabilityEstimate = result.Single(x => x.Name == "VariabilityEstimate");
            Assert.Equal("Variance", variabilityEstimate.Value);

            var residualMeanSquare = result.Single(x => x.Name == "ResidualMeanSquare");
            Assert.Null(residualMeanSquare.Value);

            var variance = result.Single(x => x.Name == "Variance");
            Assert.Equal("42", variance.Value);

            var standardDeviation = result.Single(x => x.Name == "StandardDeviation");
            Assert.Null(standardDeviation.Value);

            var noOfTreatmentGroups = result.Single(x => x.Name == "NoOfTreatmentGroups");
            Assert.Equal("5", noOfTreatmentGroups.Value);

            var plottingRangeType = result.Single(x => x.Name == "PlottingRangeType");
            Assert.Equal("Power", plottingRangeType.Value);

            var powerFrom = result.Single(x => x.Name == "PowerFrom");
            Assert.Equal("70", powerFrom.Value);

            var powerTo = result.Single(x => x.Name == "PowerTo");
            Assert.Equal("90", powerTo.Value);

            var sampleSizeFrom = result.Single(x => x.Name == "SampleSizeFrom");
            Assert.Null(sampleSizeFrom.Value);

            var sampleSizeTo = result.Single(x => x.Name == "SampleSizeTo");
            Assert.Null(sampleSizeTo.Value);

            var significance = result.Single(x => x.Name == "Significance");
            Assert.Equal("0.05", significance.Value);
        }

        [Fact]
        public void LoadArguments_ReturnsCorrectArguments()
        {
            //Arrange
            OneWayANOVAUserBasedInputsModel sut = new OneWayANOVAUserBasedInputsModel();

            List<Argument> arguments = new List<Argument>();
            arguments.Add(new Argument { Name = "GraphTitle", Value = "TitleTest" });
            arguments.Add(new Argument { Name = "EffectSizeEstimate", Value = "TreatmentMeanSquare" });
            arguments.Add(new Argument { Name = "TreatmentMeanSquare", Value = "12.5" });
            arguments.Add(new Argument { Name = "Means", Value = "55" });
            arguments.Add(new Argument { Name = "VariabilityEstimate", Value = "Variance" });
            arguments.Add(new Argument { Name = "ResidualMeanSquare" });
            arguments.Add(new Argument { Name = "Variance", Value = "42" });
            arguments.Add(new Argument { Name = "StandardDeviation" });
            arguments.Add(new Argument { Name = "NoOfTreatmentGroups", Value = "5" });
            arguments.Add(new Argument { Name = "PlottingRangeType", Value = "Power" });
            arguments.Add(new Argument { Name = "PowerFrom", Value = "70" });
            arguments.Add(new Argument { Name = "PowerTo", Value = "90" });
            arguments.Add(new Argument { Name = "SampleSizeFrom" });
            arguments.Add(new Argument { Name = "SampleSizeTo" });
            arguments.Add(new Argument { Name = "Significance", Value = "0.05" });

            Assert.Equal(15, arguments.Count);

            //Act
            sut.LoadArguments(arguments);

            //Assert
            Assert.Equal("TreatmentMeanSquare", sut.EffectSizeEstimate.ToString());
            Assert.Equal("12.5", sut.TreatmentMeanSquare.ToString());
            Assert.Equal("55", sut.Means);
            Assert.Equal("Variance", sut.VariabilityEstimate.ToString());
            Assert.Null(sut.ResidualMeanSquare);
            Assert.Equal("42", sut.Variance.ToString());
            Assert.Null(sut.StandardDeviation);
            Assert.Equal("55", sut.Means);
            Assert.Equal(5, sut.NoOfTreatmentGroups);
            Assert.Equal("TitleTest", sut.GraphTitle);
            Assert.Equal(PlottingRangeTypeOption.Power, sut.PlottingRangeType);
            Assert.Equal(70, sut.PowerFrom);
            Assert.Equal(90, sut.PowerTo);
            Assert.Null(sut.SampleSizeFrom);
            Assert.Null(sut.SampleSizeTo);
            Assert.Equal("0.05", sut.Significance);
        }

        [Fact]
        public void GetCommandLineArguments_ReturnsCorrectString()
        {
            //Arrange
            OneWayANOVAUserBasedInputsModel sut = GetModel();

            //Act
            string result = sut.GetCommandLineArguments();

            //Assert
            Assert.Equal("UserValues TreatmentMeanSquare 12.5 55 Variance NULL 42 NULL 5 0.05 PowerAxis 70 90 TitleTest", result);
        }


        private OneWayANOVAUserBasedInputsModel GetModel()
        {
            var model = new OneWayANOVAUserBasedInputsModel
            {
                EffectSizeEstimate = EffectSizeEstimate.TreatmentMeanSquare,
                TreatmentMeanSquare = 12.5m,
                Means = "55",
                VariabilityEstimate = VariabilityEstimate.Variance,
                ResidualMeanSquare = null,
                Variance = 42,
                StandardDeviation = null,
                NoOfTreatmentGroups = 5,
                GraphTitle = "TitleTest",
                PlottingRangeType = PlottingRangeTypeOption.Power,
                PowerFrom = 70,
                PowerTo = 90,
                SampleSizeFrom = null,
                SampleSizeTo = null,
                Significance = "0.05"
            };

            return model;
        }
    }
}