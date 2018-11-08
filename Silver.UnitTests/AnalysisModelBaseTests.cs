using Moq;
using SilveR.Models;
using SilveR.StatsModels;
using SilveR.Validators;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Silver.UnitTests
{
    public class AnalysisModelBaseTests
    {
        [Fact]
        public void Constructor_InitializesObject()
        {
            //Arrange
            Mock<IDataset> mockDataset = new Mock<IDataset>();
            mockDataset.Setup(x => x.DatasetID).Returns(1);
            mockDataset.Setup(x => x.DatasetToDataTable()).Returns(TestDataTable.GetTestDataTable());

            //Act
            AnalysisTestClass sut = new AnalysisTestClass(mockDataset.Object);

            //Assert
            Assert.Equal(1, sut.DatasetID);
            Assert.Equal(new List<string>() { "Resp1", "Resp 2", "Resp3", "Resp4", "Resp5", "Resp6", "Resp7", "Resp8", "Resp9", "Resp10", "Resp11", "Cat1", "Cat2", "Cat3", "Cat4", "Cat5", "Cat6", "Cat456" }, sut.AvailableVariables);
            Assert.Equal(33, sut.DataTable.Rows.Count);
        }

        [Fact]
        public void AvailableVariablesAllowNull_ReturnsListStringWithFirstEmptyValue()
        {
            //Arrange
            Mock<IDataset> mockDataset = new Mock<IDataset>();
            mockDataset.Setup(x => x.DatasetID).Returns(1);
            mockDataset.Setup(x => x.DatasetToDataTable()).Returns(TestDataTable.GetTestDataTable());
            AnalysisTestClass sut = new AnalysisTestClass(mockDataset.Object);

            //Act
            IEnumerable<string> result = sut.AvailableVariablesAllowNull;

            //Assert
            Assert.IsAssignableFrom<IEnumerable<string>>(result);
            Assert.Equal(String.Empty, result.First());
        }
    }

    public class AnalysisTestClass : AnalysisModelBase
    {
        public AnalysisTestClass(IDataset dataset)
            : base(dataset, "TestScript") { }

        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }

        public override string[] ExportData()
        {
            throw new System.NotImplementedException();
        }

        public override IEnumerable<Argument> GetArguments()
        {
            throw new System.NotImplementedException();
        }

        public override string GetCommandLineArguments()
        {
            throw new System.NotImplementedException();
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override void LoadArguments(IEnumerable<Argument> arguments)
        {
            throw new System.NotImplementedException();
        }

        public override string ToString()
        {
            return base.ToString();
        }

        public override ValidationInfo Validate()
        {
            throw new System.NotImplementedException();
        }
    }
}