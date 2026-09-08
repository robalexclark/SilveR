using Moq;
using SilveR.Models;
using SilveR.StatsModels;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Globalization;
using System.Linq;
using Xunit;

namespace SilveR.UnitTests.StatsModels
{
    public class HasseDiagramGeneratorModelTests
    {
        [Fact]
        public void Validate_NoFactors_ReturnsError()
        {
            System.Threading.Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
            HasseDiagramGeneratorModel sut = new HasseDiagramGeneratorModel();

            var result = sut.Validate();

            Assert.False(result.ValidatedOK);
            Assert.Equal("Enter at least one fixed or random factor.", result.ErrorMessages.Single());
        }

        [Theory]
        [InlineData(true, "The fixed factor Animal has numerical levels. Note this factor will be treated as categoric in this assessment.")]
        [InlineData(false, "The random factor Animal has numerical levels. Note this factor will be treated as categoric in this assessment.")]
        public void Validate_NumericFactor_ReturnsWarning(bool isFixedFactor, string expectedWarning)
        {
            HasseDiagramGeneratorModel sut = GetModel();
            if (isFixedFactor)
            {
                sut.FixedFactors = new[] { "Animal" };
            }
            else
            {
                sut.RandomFactors = new[] { "Animal" };
            }

            var result = sut.Validate();

            Assert.True(result.ValidatedOK);
            Assert.Equal(expectedWarning, result.WarningMessages.Single());
        }

        [Theory]
        [InlineData(true, "The fixed factor One contains only identical elements. If this is correct this variable should be removed prior to running the analysis.")]
        [InlineData(false, "The random factor One contains only identical elements. If this is correct this variable should be removed prior to running the analysis.")]
        public void Validate_SingleLevelFactor_ReturnsError(bool isFixedFactor, string expectedError)
        {
            HasseDiagramGeneratorModel sut = GetModel();
            if (isFixedFactor)
            {
                sut.FixedFactors = new[] { "One" };
            }
            else
            {
                sut.RandomFactors = new[] { "One" };
            }

            var result = sut.Validate();

            Assert.False(result.ValidatedOK);
            Assert.Equal(expectedError, result.ErrorMessages.Single());
        }

        [Fact]
        public void Validate_MultiLevelCategoricalFactor_ReturnsNoMessages()
        {
            HasseDiagramGeneratorModel sut = GetModel();
            sut.FixedFactors = new[] { "Treatment" };

            var result = sut.Validate();

            Assert.True(result.ValidatedOK);
            Assert.Empty(result.ErrorMessages);
            Assert.Empty(result.WarningMessages);
        }

        [Fact]
        public void DataAnnotations_DuplicateFactor_ReturnsPluralFactorNames()
        {
            HasseDiagramGeneratorModel sut = GetModel();
            sut.FixedFactors = new[] { "Treatment" };
            sut.RandomFactors = new[] { "Treatment" };
            List<ValidationResult> validationResults = new List<ValidationResult>();

            bool result = Validator.TryValidateObject(sut, new ValidationContext(sut), validationResults, true);

            Assert.False(result);
            Assert.Contains(validationResults, x => x.ErrorMessage == "Fixed factors (Treatment) has been selected in more than one input category, please change your input options.");
            Assert.Contains(validationResults, x => x.ErrorMessage == "Random factors (Treatment) has been selected in more than one input category, please change your input options.");
        }

        [Fact]
        public void GetCommandLineArguments_FixedAndRandomFactors_ReturnsConfiguredArguments()
        {
            System.Threading.Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
            HasseDiagramGeneratorModel sut = new HasseDiagramGeneratorModel
            {
                FixedFactors = new List<string> { "Treatment", "Time" },
                RandomFactors = new List<string> { "Block" }
            };

            string result = sut.GetCommandLineArguments();

            Assert.Equal("Treatment,Time Block N blue Y Y Y red grey 2 orange 1.5 1 1 1", result);
        }

        private static HasseDiagramGeneratorModel GetModel()
        {
            Mock<IDataset> mockDataset = new Mock<IDataset>();
            mockDataset.Setup(x => x.DatasetID).Returns(1);
            mockDataset.Setup(x => x.DatasetToDataTable()).Returns(GetTestDataTable());
            return new HasseDiagramGeneratorModel(mockDataset.Object);
        }

        private static DataTable GetTestDataTable()
        {
            DataTable dataTable = new DataTable();
            dataTable.Columns.Add("Treatment");
            dataTable.Columns.Add("Animal");
            dataTable.Columns.Add("One");
            dataTable.Rows.Add("Control", "1", "1");
            dataTable.Rows.Add("Treated", "2", "1");
            return dataTable;
        }
    }
}
