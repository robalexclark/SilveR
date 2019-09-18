using Moq;
using SilveR.Models;
using SilveR.StatsModels;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Xunit;
using static SilveR.StatsModels.DoseResponseAndNonLinearRegressionAnalysisModel;

namespace SilveR.UnitTests.StatsModels
{
    public class DoseResponseAndNonLinearRegressionAnalysisModelTests
    {
        [Fact]
        public void ScriptFileName_ReturnsCorrectString()
        {
            //Arrange
            DoseResponseAndNonLinearRegressionAnalysisModel sut = new DoseResponseAndNonLinearRegressionAnalysisModel();

            //Act
            string result = sut.ScriptFileName;

            //Assert
            Assert.Equal("DoseResponseAndNonLinearRegressionAnalysis", result);
        }

        [Fact]
        public void TransformationsList_ReturnsCorrectList()
        {
            //Arrange
            DoseResponseAndNonLinearRegressionAnalysisModel sut = new DoseResponseAndNonLinearRegressionAnalysisModel();

            //Act
            IEnumerable<string> result = sut.TransformationsList;

            //Assert
            Assert.IsAssignableFrom<IEnumerable<string>>(result);
            Assert.Equal(new List<string>() { "None", "Log10", "Loge", "Square Root", "ArcSine", "Rank" }, result);
        }

        [Fact]
        public void ExportData_FourParameter_ReturnsCorrectStringArray()
        {
            //Arrange
            Mock<IDataset> mockDataset = new Mock<IDataset>();
            mockDataset.Setup(x => x.DatasetID).Returns(It.IsAny<int>);
            mockDataset.Setup(x => x.DatasetToDataTable()).Returns(GetTestDataTable());

            DoseResponseAndNonLinearRegressionAnalysisModel sut = GetModelFourParameter(mockDataset.Object);

            //Act
            string[] result = sut.ExportData();

            //Assert
            Assert.Equal("Respivs_sp_ivs1,Dose1,QCivs_sp_ivsResp1,QCDose1,Sampleivs_sp_ivs1", result[0]);
            Assert.Equal(19, result.Count());
            Assert.StartsWith("0.856310679611651,0.1", result[12]);
        }

        [Fact]
        public void ExportData_Equation_ReturnsCorrectStringArray()
        {
            //Arrange
            Mock<IDataset> mockDataset = new Mock<IDataset>();
            mockDataset.Setup(x => x.DatasetID).Returns(It.IsAny<int>);
            mockDataset.Setup(x => x.DatasetToDataTable()).Returns(GetTestDataTable());

            DoseResponseAndNonLinearRegressionAnalysisModel sut = GetModelEquation(mockDataset.Object);

            //Act
            string[] result = sut.ExportData();

            //Assert
            Assert.Equal("Respivs_sp_ivs1,Dose1", result[0]);
            Assert.Equal(19, result.Count());
            Assert.StartsWith("0.971844660194175,1", result[15]);
        }

        [Fact]
        public void GetArguments_FourParameter_ReturnsCorrectArguments()
        {
            //Arrange
            DoseResponseAndNonLinearRegressionAnalysisModel sut = GetModelFourParameter(GetDataset());

            //Act
            List<Argument> result = sut.GetArguments().ToList();

            //Assert
            var responses = result.Single(x => x.Name == "AnalysisType");
            Assert.Equal("FourParameter", responses.Value);

            var dose = result.Single(x => x.Name == "Dose");
            Assert.Equal("Dose1", dose.Value);

            var doseScale = result.Single(x => x.Name == "DoseScale");
            Assert.Equal("Log10", doseScale.Value);

            var edicCoeff = result.Single(x => x.Name == "EDICCoeff");
            Assert.Equal("8", edicCoeff.Value);

            var edicStartValue = result.Single(x => x.Name == "EDICStartValue");
            Assert.Equal("9", edicStartValue.Value);

            var equation = result.Single(x => x.Name == "Equation");
            Assert.Null(equation.Value);

            var equationXAxis = result.Single(x => x.Name == "EquationXAxis");
            Assert.Null(equationXAxis.Value);

            var equationYAxis = result.Single(x => x.Name == "EquationYAxis");
            Assert.Null(equationYAxis.Value);

            var maxCoeff = result.Single(x => x.Name == "MaxCoeff");
            Assert.Equal("4", maxCoeff.Value);

            var maxStartValue = result.Single(x => x.Name == "MaxStartValue");
            Assert.Equal("5", maxStartValue.Value);

            var minCoeff = result.Single(x => x.Name == "MinCoeff");
            Assert.Equal("2", minCoeff.Value);

            var minStartValue = result.Single(x => x.Name == "MinStartValue");
            Assert.Equal("1", minStartValue.Value);

            var offset = result.Single(x => x.Name == "Offset");
            Assert.Equal("1", offset.Value);

            var qcDose = result.Single(x => x.Name == "QCDose");
            Assert.Equal("QCDose1", qcDose.Value);

            var qcResponse = result.Single(x => x.Name == "QCResponse");
            Assert.Equal("QC Resp1", qcResponse.Value);

            var response = result.Single(x => x.Name == "Response");
            Assert.Equal("Resp 1", response.Value);

            var responseTransformation = result.Single(x => x.Name == "ResponseTransformation");
            Assert.Equal("None", responseTransformation.Value);

            var samplesResponse = result.Single(x => x.Name == "SamplesResponse");
            Assert.Equal("Sample 1", samplesResponse.Value);

            var slopeCoeff = result.Single(x => x.Name == "SlopeCoeff");
            Assert.Equal("6", slopeCoeff.Value);

            var slopeStartValue = result.Single(x => x.Name == "SlopeStartValue");
            Assert.Equal("7", slopeStartValue.Value);

            var startValues = result.Single(x => x.Name == "StartValues");
            Assert.Null(startValues.Value);
        }

        [Fact]
        public void GetArguments_Equation_ReturnsCorrectArguments()
        {
            //Arrange
            DoseResponseAndNonLinearRegressionAnalysisModel sut = GetModelEquation(GetDataset());

            //Act
            List<Argument> result = sut.GetArguments().ToList();

            //Assert
            var responses = result.Single(x => x.Name == "AnalysisType");
            Assert.Equal("Equation", responses.Value);

            var dose = result.Single(x => x.Name == "Dose");
            Assert.Null(dose.Value);

            var doseScale = result.Single(x => x.Name == "DoseScale");
            Assert.Equal("Log10", doseScale.Value);

            var edicCoeff = result.Single(x => x.Name == "EDICCoeff");
            Assert.Null(edicCoeff.Value);

            var edicStartValue = result.Single(x => x.Name == "EDICStartValue");
            Assert.Null(edicStartValue.Value);

            var equation = result.Single(x => x.Name == "Equation");
            Assert.Equal("y=mx+c", equation.Value);

            var equationXAxis = result.Single(x => x.Name == "EquationXAxis");
            Assert.Equal("Dose1", equationXAxis.Value);

            var equationYAxis = result.Single(x => x.Name == "EquationYAxis");
            Assert.Equal("Resp 1", equationYAxis.Value);

            var maxCoeff = result.Single(x => x.Name == "MaxCoeff");
            Assert.Null(maxCoeff.Value);

            var maxStartValue = result.Single(x => x.Name == "MaxStartValue");
            Assert.Null(maxStartValue.Value);

            var minCoeff = result.Single(x => x.Name == "MinCoeff");
            Assert.Null(minCoeff.Value);

            var minStartValue = result.Single(x => x.Name == "MinStartValue");
            Assert.Null(minStartValue.Value);

            var offset = result.Single(x => x.Name == "Offset");
            Assert.Null(offset.Value);

            var qcDose = result.Single(x => x.Name == "QCDose");
            Assert.Null(qcDose.Value);

            var qcResponse = result.Single(x => x.Name == "QCResponse");
            Assert.Null(qcResponse.Value);

            var response = result.Single(x => x.Name == "Response");
            Assert.Null(response.Value);

            var responseTransformation = result.Single(x => x.Name == "ResponseTransformation");
            Assert.Equal("None", responseTransformation.Value);

            var samplesResponse = result.Single(x => x.Name == "SamplesResponse");
            Assert.Null(samplesResponse.Value);

            var slopeCoeff = result.Single(x => x.Name == "SlopeCoeff");
            Assert.Null(slopeCoeff.Value);

            var slopeStartValue = result.Single(x => x.Name == "SlopeStartValue");
            Assert.Null(slopeStartValue.Value);

            var startValues = result.Single(x => x.Name == "StartValues");
            Assert.Equal("1,2,3", startValues.Value);
        }

        [Fact]
        public void LoadArguments_ReturnsCorrectArguments()
        {
            //Arrange
            DoseResponseAndNonLinearRegressionAnalysisModel sut = new DoseResponseAndNonLinearRegressionAnalysisModel(GetDataset());

            List<Argument> arguments = new List<Argument>();
            arguments.Add(new Argument { Name = "AnalysisType", Value = "FourParameter" });
            arguments.Add(new Argument { Name = "Dose", Value = "Dose1" });
            arguments.Add(new Argument { Name = "DoseScale", Value = "Log10" });
            arguments.Add(new Argument { Name = "EDICCoeff", Value = "8" });
            arguments.Add(new Argument { Name = "EDICStartValue", Value = "9" });
            arguments.Add(new Argument { Name = "Equation", Value = "y=mx+c" });
            arguments.Add(new Argument { Name = "EquationXAxis", Value = "Resp2" });
            arguments.Add(new Argument { Name = "EquationYAxis", Value = "Dose2" });
            arguments.Add(new Argument { Name = "MaxCoeff", Value = "4" });
            arguments.Add(new Argument { Name = "MaxStartValue", Value = "5" });
            arguments.Add(new Argument { Name = "MinCoeff", Value = "2" });
            arguments.Add(new Argument { Name = "MinStartValue", Value = "1" });
            arguments.Add(new Argument { Name = "Offset", Value = "1" });
            arguments.Add(new Argument { Name = "QCDose", Value = "QCDose1" });
            arguments.Add(new Argument { Name = "QCResponse", Value = "QC Resp1" });
            arguments.Add(new Argument { Name = "Response", Value = "Resp 1" });
            arguments.Add(new Argument { Name = "ResponseTransformation", Value = "None" });
            arguments.Add(new Argument { Name = "SamplesResponse", Value = "Sample 1" });
            arguments.Add(new Argument { Name = "SlopeCoeff", Value = "6" });
            arguments.Add(new Argument { Name = "SlopeStartValue", Value = "7" });
            arguments.Add(new Argument { Name = "StartValues", Value = "1,2,3" });

            Assert.Equal(21, arguments.Count);

            //Act
            sut.LoadArguments(arguments);

            //Assert
            Assert.Equal("FourParameter", sut.AnalysisType.ToString());
            Assert.Equal("Dose1", sut.Dose);
            Assert.Equal("Log10", sut.DoseScale.ToString());
            Assert.Equal(8, sut.EDICCoeff);
            Assert.Equal(9, sut.EDICStartValue);
            Assert.Equal("y=mx+c", sut.Equation);
            Assert.Equal("Resp2", sut.EquationXAxis);
            Assert.Equal("Dose2", sut.EquationYAxis);
            Assert.Equal(4, sut.MaxCoeff);
            Assert.Equal(1, sut.MinStartValue);
            Assert.Equal(1, sut.Offset);
            Assert.Equal("QCDose1", sut.QCDose);
            Assert.Equal("QC Resp1", sut.QCResponse);
            Assert.Equal("Resp 1", sut.Response);
            Assert.Equal("None", sut.ResponseTransformation);
            Assert.Equal("Sample 1", sut.SamplesResponse);
            Assert.Equal(6, sut.SlopeCoeff);
            Assert.Equal(7, sut.SlopeStartValue);
            Assert.Equal("1,2,3", sut.StartValues);
        }

        [Fact]
        public void GetCommandLineArguments_FourParameter_ReturnsCorrectString()
        {
            //Arrange
            DoseResponseAndNonLinearRegressionAnalysisModel sut = GetModelFourParameter(GetDataset());

            //Act
            string result = sut.GetCommandLineArguments();

            //Assert
            Assert.Equal("FourParameter Respivs_sp_ivs1 None Dose1 1 Log10 QCivs_sp_ivsResp1 QCDose1 Sampleivs_sp_ivs1 2 4 6 8 1 5 7 9 NULL NULL NULL NULL", result);
        }

        [Fact]
        public void GetCommandLineArguments_Equation_ReturnsCorrectString()
        {
            //Arrange
            DoseResponseAndNonLinearRegressionAnalysisModel sut = GetModelEquation(GetDataset());

            //Act
            string result = sut.GetCommandLineArguments();

            //Assert
            Assert.Equal("Equation NULL None NULL NULL Log10 NULL NULL NULL NULL NULL NULL NULL NULL NULL NULL NULL y=mx+c 1,2,3 Respivs_sp_ivs1 Dose1", result);
        }


        private DoseResponseAndNonLinearRegressionAnalysisModel GetModelFourParameter(IDataset dataset)
        {
            var model = new DoseResponseAndNonLinearRegressionAnalysisModel(dataset)
            {
                AnalysisType = AnalysisOption.FourParameter,
                Dose = "Dose1",
                DoseScale = DoseScaleOption.Log10,
                EDICCoeff = 8m,
                EDICStartValue = 9m,
                Equation = null,
                EquationXAxis = null,
                EquationYAxis = null,
                MaxCoeff = 4m,
                MaxStartValue = 5m,
                MinCoeff = 2m,
                MinStartValue = 1m,
                Offset = 1m,
                QCDose = "QCDose1",
                QCResponse = "QC Resp1",
                Response = "Resp 1",
                ResponseTransformation = "None",
                SamplesResponse = "Sample 1",
                SlopeCoeff = 6m,
                SlopeStartValue = 7m,
                StartValues = null
            };

            return model;
        }

        private DoseResponseAndNonLinearRegressionAnalysisModel GetModelEquation(IDataset dataset)
        {
            var model = new DoseResponseAndNonLinearRegressionAnalysisModel(dataset)
            {
                AnalysisType = AnalysisOption.Equation,
                Dose = null,
                DoseScale = DoseScaleOption.Log10,
                EDICCoeff = null,
                EDICStartValue = null,
                Equation = "y=mx+c",
                EquationXAxis = "Dose1",
                EquationYAxis = "Resp 1",
                MaxCoeff = null,
                MaxStartValue = null,
                MinCoeff = null,
                MinStartValue = null,
                Offset = null,
                QCDose = null,
                QCResponse = null,
                Response = null,
                ResponseTransformation = "None",
                SamplesResponse = null,
                SlopeCoeff = null,
                SlopeStartValue = null,
                StartValues = "1,2,3"
            };

            return model;
        }

        private DataTable GetTestDataTable()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("SilveRSelected");
            dt.Columns.Add("Resp 1");
            dt.Columns.Add("Dose1");
            dt.Columns.Add("QC Resp1");
            dt.Columns.Add("QCDose1");
            dt.Columns.Add("QCDose2");
            dt.Columns.Add("Sample 1");
            dt.Columns.Add("Resp2");
            dt.Columns.Add("Resp3");
            dt.Columns.Add("Resp4");
            dt.Columns.Add("Resp5");
            dt.Columns.Add("Resp6");
            dt.Columns.Add("Resp7");
            dt.Columns.Add("Resp8");
            dt.Columns.Add("Resp9");
            dt.Columns.Add("Resp10");
            dt.Columns.Add("Dose2");
            dt.Columns.Add("Dose3");
            dt.Columns.Add("Dose4");
            dt.Columns.Add("Dose5");
            dt.Columns.Add("Dose6");
            dt.Columns.Add("Dose7");
            dt.Columns.Add("Dose8");
            dt.Columns.Add("QCResp2");
            dt.Columns.Add("QCResp3");
            dt.Columns.Add("QCResp4");
            dt.Columns.Add("QCResp5");
            dt.Columns.Add("QCDose3");
            dt.Columns.Add("QCDose4");
            dt.Columns.Add("Sample2");
            dt.Columns.Add("Sample3");
            dt.Columns.Add("Sample4");
            dt.Columns.Add("Sample5");
            dt.Rows.Add(new object[] { "True", "0.0203883495145631", "0", "0.75", "0.0794328234724281", "0.00794328234724281", "0.2", "5", "missing", "", "-1.1", "0", "1.2", "2.1", "0.980582524271845", "104.900562526053", "", "missing", "0", "0", "0", "0", "-5", "missing", "-1.2", "0", "1.2", "", "missing", "missing", "0", "-1.2", "1.2", });
            dt.Rows.Add(new object[] { "True", "0.0310679611650485", "0", "0.72", "0.0794328234724281", "0.00794328234724281", "0.6", "", "3.2", "3.2", "0.001", "3.2", "0.001", "", "0.990291262135922", "104.151202294568", "0", "0", "0", "0", "0", "0", "-5", "72", "0.5", "72", "0.5", "2.1", "2.1", "45", "72", "0.5", "0.5", });
            dt.Rows.Add(new object[] { "True", "0.0310679611650485", "0", "0.77", "0.0794328234724281", "0.00794328234724281", "0.4", "", "3.2", "3.2", "0.001", "3.2", "0.001", "3.2", "0.995145631067961", "104.89263655946", "0", "0", "0", "0", "0", "0", "-5", "77", "0.3", "77", "0.3", "2.1", "2.1", "87", "77", "0.3", "0.3", });
            dt.Rows.Add(new object[] { "True", "0.0330097087378641", "0.001", "0.05", "0.0316227766016838", "0.0316227766016838", "0.2", "", "3.4", "3.4", "0.0067", "3.4", "0.0067", "", "0.963106796116505", "104.764839829866", "0.001", "0.001", "0.001", "0.001", "0.001", "0", "-5", "5", "0.7", "5", "0.7", "1", "1", "120", "5", "0.7", "0.7", });
            dt.Rows.Add(new object[] { "True", "0.0310679611650485", "0.001", "0.47", "0.0316227766016838", "0.0316227766016838", "0.5435", "", "3.2", "3.2", "0.3456", "3.2", "0.3456", "3.2", "0.956310679611651", "14.3678185545044", "0.001", "0.001", "0.001", "0.001", "0.001", "0", "-2", "47", "0.2", "47", "0.2", "1", "1", "-1.1", "47", "0.2", "0.2", });
            dt.Rows.Add(new object[] { "True", "0.0514563106796116", "0.001", "0.42", "0.0316227766016838", "0.0316227766016838", "0.675543", "", "5.3", "5.3", "0.001", "5.3", "0.001", "", "0.971844660194175", "14.8007816109063", "0.001", "0.001", "0.001", "0.001", "0.001", "0", "-2", "42", "0.7", "42", "0.7", "1", "1", "45", "42", "0.7", "0.7", });
            dt.Rows.Add(new object[] { "True", "0.274757281553398", "0.01", "0.38", "0.00794328234724281", "0.0794328234724281", "0.8765", "", "28.3", "28.3", "0.001", "28.3", "0.001", "", "0.778640776699029", "14.1819025518702", "0.01", "0.01", "0.01", "0.01", "0.001", "0", "-2", "38", "-0.4", "38", "-0.4", "0.3", "0.3", "78", "38", "-0.4", "-0.4", });
            dt.Rows.Add(new object[] { "True", "0.246601941747573", "0.01", "0.37", "0.00794328234724281", "0.0794328234724281", "0.232", "", "25.4", "25.4", "0.01", "25.4", "0.01", "", "0.810679611650485", "14.6785976737855", "0.01", "0.01", "0.01", "0.01", "0.001", "0", "-2", "37", "-0.6", "37", "-0.6", "0.3", "0.3", "23", "37", "-0.6", "-0.6", });
            dt.Rows.Add(new object[] { "True", "0.302912621359223", "0.01", "0.35", "0.00794328234724281", "0.0794328234724281", "0.99", "", "31.2", "31.2", "0.001", "31.2", "0.001", "31.2", "0.856310679611651", "34.1278439704564", "0.01", "0.01", "0.01", "0.01", "0.001", "0", "2", "35", "-0.8", "35", "-0.8", "0.3", "0.3", "5", "35", "-0.8", "-0.8", });
            dt.Rows.Add(new object[] { "True", "0.778640776699029", "0.1", "", "", "", "0.02", "", "80.2", "80.2", "0.001", "80.2", "0.001", "", "0.274757281553398", "34.6736172668371", "0.1", "0.1", "0.1", "0.01", "0.001", "0", "2", "", "", "", "", "", "", "7", "", "", "", });
            dt.Rows.Add(new object[] { "True", "0.810679611650485", "0.1", "", "", "", "0.01", "", "83.5", "83.5", "0.001", "83.5", "0.001", "83.5", "0.246601941747573", "34.2888360316494", "0.1", "0.1", "0.1", "0.01", "0.001", "0", "2", "", "", "", "", "", "", "90", "", "", "", });
            dt.Rows.Add(new object[] { "True", "0.856310679611651", "0.1", "", "", "", "", "", "88.2", "88.2", "0.01", "88.2", "0.01", "", "0.302912621359223", "34.7597103046069", "0.1", "0.1", "0.1", "0.01", "0.001", "0", "2", "", "", "", "", "", "", "", "", "", "", });
            dt.Rows.Add(new object[] { "True", "0.963106796116505", "1", "", "", "", "", "", "99.2", "99.2", "0.01", "99.2", "0.01", "99.2", "0.0330097087378641", "64.9090134657513", "1", "1", "0.1", "0.01", "0.001", "0", "3", "", "", "", "", "", "", "", "", "", "", });
            dt.Rows.Add(new object[] { "True", "0.956310679611651", "1", "", "", "", "", "", "98.5", "98.5", "0.01", "98.5", "0.01", "", "0.0310679611650485", "64.2679841246206", "1", "1", "0.1", "0.01", "0.001", "0", "3", "", "", "", "", "", "", "", "", "", "", });
            dt.Rows.Add(new object[] { "True", "0.971844660194175", "1", "", "", "", "", "", "100.1", "100.1", "0.1", "100.1", "0.1", "100.1", "0.0514563106796116", "64.3630175155722", "1", "1", "0.1", "0.01", "0.001", "0", "3", "", "", "", "", "", "", "", "", "", "", });
            dt.Rows.Add(new object[] { "True", "0.980582524271845", "10", "", "", "", "", "", "101", "101", "0.1", "101", "0.1", "", "0.0203883495145631", "64.9026966205013", "10", "10", "0.1", "0.01", "0.001", "0", "3", "", "", "", "", "", "", "", "", "", "", });
            dt.Rows.Add(new object[] { "True", "0.990291262135922", "10", "", "", "", "", "", "102", "102", "0.1", "102", "0.1", "102", "0.0310679611650485", "104.832024120313", "10", "10", "0.1", "0.01", "0.001", "0", "4", "", "", "", "", "", "", "", "", "", "", });
            dt.Rows.Add(new object[] { "True", "0.995145631067961", "10", "", "", "", "", "", "102.5", "102.5", "0.05", "102.5", "0.05", "", "0.0310679611650485", "104.173109626331", "10", "10", "0.1", "0.01", "0.001", "0", "4", "", "", "", "", "", "", "", "", "", "", });
            dt.Rows.Add(new object[] { "True", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "104.777810661416", "", "", "", "", "", "", "4", "", "", "", "", "", "", "", "", "", "", });
            dt.Rows.Add(new object[] { "True", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "104.501126125404", "", "", "", "", "", "", "4", "", "", "", "", "", "", "", "", "", "", });

            return dt;
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
}