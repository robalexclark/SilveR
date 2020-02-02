using SilveR.Models;
using SilveR.StatsModels;
using SilveR.Validators;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using Xunit;

namespace SilveR.UnitTests.StatsModels
{    
    public class AnalysisDataModelBaseTests
    {
        [Fact]
        public void AnalysisDataModelBaseTestClass_Constructor_InitializesObject()
        {
            //Arrange
            System.Threading.Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
            Dataset dataset = GetDataset();

            //Act
            AnalysisDataModelBaseTestClass sut = new AnalysisDataModelBaseTestClass(dataset, "TestScript");

            //Assert
            Assert.Equal(6, sut.DatasetID);
            Assert.Equal(new List<string>() { "Resp 1", "Resp2", "Resp 3", "Resp4", "Resp 5", "Resp 6", "Resp 7", "Resp8", "Resp:9", "Resp-10", "Resp^11", "Treat1", "Treat2", "Treat3", "Treat4", "Treat(5", "Treat£6", "Treat:7", "Treat}8", "PVTestresponse1", "PVTestresponse2", "PVTestgroup" }, sut.AvailableVariables);
            Assert.Equal(12, sut.DataTable.Rows.Count);
            Assert.Equal("TestScript", sut.ScriptFileName);
        }

        [Fact]
        public void AnalysisDataModelBaseTestClass_AvailableVariablesAllowNull_ReturnsListStringWithFirstEmptyValue()
        {
            //Arrange
            System.Threading.Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
            Dataset dataset = GetDataset();

            AnalysisDataModelBaseTestClass sut = new AnalysisDataModelBaseTestClass(dataset, "TestScript");

            //Act
            IEnumerable<string> result = sut.AvailableVariablesAllowNull;

            //Assert
            Assert.IsAssignableFrom<IEnumerable<string>>(result);
            Assert.Equal(String.Empty, result.First());
            Assert.Equal("TestScript", sut.ScriptFileName);
        }

        [Fact]
        public void AnalysisModelBaseTestClass_Constructor_InitializesObject()
        {
            //Arrange
            System.Threading.Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;

            //Act
            AnalysisModelBaseTestClass sut = new AnalysisModelBaseTestClass("TestScript");

            //Assert
            Assert.Equal("TestScript", sut.ScriptFileName);
        }


        private Dataset GetDataset()
        {
            Dataset dataset = new Dataset
            {
                DatasetID = 6,
                DatasetName = "_test dataset.xlsx [unpairedttest]",
                DateUpdated = new DateTime(2018, 11, 16, 9, 14, 35),
                TheData = "SilveRSelected,Resp 1,Resp2,Resp 3,Resp4,Resp 5,Resp 6,Resp 7,Resp8,Resp:9,Resp-10,Resp^11,Treat1,Treat2,Treat3,Treat4,Treat(5,Treat£6,Treat:7,Treat}8,PVTestresponse1,PVTestresponse2,PVTestgroup\r\nTrue,65,65,65,x,,-2,0,-2,65,65,0.1,A,A,1,A,1,A,A,A,1,1,1\r\nTrue,32,,32,32,32,32,32,0.1,32,32,0.1,A,A,1,A,1,A,A,A,2,2,1\r\nTrue,543,,543,543,543,543,543,0.2,543,543,0.2,A,A,1,A,1,A,A,A,3,3,1\r\nTrue,675,,675,675,675,675,675,0.1,675,675,0.1,A,A,1,B,1,A,A,A,4,4,1\r\nTrue,876,,876,876,876,876,876,0.2,876,876,0.2,A,A,1,B,1,A,A,A,11,10,2\r\nTrue,54,,54,54,54,54,54,0.3,54,54,0.3,A,A,1,B,1,A,A,A,12,11,2\r\nTrue,432,,,432,432,432,432,0.45,432,432,0.45,B,B,2,C,2,B,B,B,13,12,2\r\nTrue,564,,,564,564,564,564,0.2,564,564,0.2,B,B,2,C,2,B,B,,14,13,2\r\nTrue,76,,,76,76,76,76,0.14,76,76,0.14,B,B,2,C,2,B,B,,,,\r\nTrue,54,,,54,54,54,54,0.2,54,54,0.2,B,B,2,D,3,B,B,,,,\r\nTrue,32,,,32,32,32,32,0.1,32,32,0.1,B,B,2,D,3,B,B,,,,\r\nTrue,234,,,234,234,234,234,0.4,234,234,0.4,B,,2,D,3,B,B,,,,",
                VersionNo = 1
            };

            return dataset;
        }
    }


    public class AnalysisDataModelBaseTestClass : AnalysisDataModelBase
    {
        public AnalysisDataModelBaseTestClass(IDataset dataset, string scriptFileName)
            : base(dataset, scriptFileName) { }

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

        public override void LoadArguments(IEnumerable<Argument> arguments)
        {
            throw new System.NotImplementedException();
        }

        public override ValidationInfo Validate()
        {
            throw new System.NotImplementedException();
        }
    }

    public class AnalysisModelBaseTestClass : AnalysisModelBase
    {
        public AnalysisModelBaseTestClass(string scriptFileName)
            : base(scriptFileName) { }

        public override IEnumerable<Argument> GetArguments()
        {
            throw new System.NotImplementedException();
        }

        public override string GetCommandLineArguments()
        {
            throw new System.NotImplementedException();
        }

        public override void LoadArguments(IEnumerable<Argument> arguments)
        {
            throw new System.NotImplementedException();
        }

        public override ValidationInfo Validate()
        {
            throw new System.NotImplementedException();
        }
    }
}