using Moq;
using SilveR.Models;
using SilveR.StatsModels;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Xunit;

namespace Silver.UnitTests.StatsModels
{
    
    public class GraphicalAnalysisModelTests
    {
        [Fact]
        public void ScriptFileName_ReturnsCorrectString()
        {
            //Arrange
            GraphicalAnalysisModel sut = new GraphicalAnalysisModel();

            //Act
            string result = sut.ScriptFileName;

            //Assert
            Assert.Equal("GraphicalAnalysis", result);
        }

        [Fact]
        public void TransformationsList_ReturnsCorrectList()
        {
            //Arrange
            GraphicalAnalysisModel sut = new GraphicalAnalysisModel();

            //Act
            IEnumerable<string> result = sut.TransformationsList;

            //Assert
            Assert.IsAssignableFrom<IEnumerable<string>>(result);
            Assert.Equal(new List<string>() { "None", "Log10", "Loge", "Square Root", "ArcSine" }, result);
        }

        [Fact]
        public void ExportData_ReturnsCorrectStringArray()
        {
            //Arrange
            Mock<IDataset> mockDataset = new Mock<IDataset>();
            mockDataset.Setup(x => x.DatasetID).Returns(It.IsAny<int>);
            mockDataset.Setup(x => x.DatasetToDataTable()).Returns(GetTestDataTable());

            GraphicalAnalysisModel sut = GetModel(mockDataset.Object);

            //Act
            string[] result = sut.ExportData();

            //Assert
            Assert.Equal("Respivs_sp_ivs1,Treativs_sp_ivs1,Cat1,Cat2,Treatment", result[0]);
            Assert.Equal(127, result.Count()); //as blank responses are removed
            Assert.Equal("0.872,T1,1,A,control", result[3]);
        }

        //[Fact]
        //public void GetArguments_ReturnsCorrectArguments()
        //{
        //    //Arrange
        //    GraphicalAnalysisModel sut = GetModel(GetDataset());

        //    //Act
        //    List<Argument> result = sut.GetArguments().ToList();

        //    //Assert
        //    var boxPlotIncludeData = result.Single(x => x.Name == "BoxPlotIncludeData");
        //    Assert.Equal("False", boxPlotIncludeData.Value);

        //    var boxplotSelected = result.Single(x => x.Name == "BoxplotSelected");
        //    Assert.Equal("False", boxplotSelected.Value);

        //    var caseIDFactor = result.Single(x => x.Name == "CaseIDFactor");
        //    Assert.Equal("Treat 1", caseIDFactor.Value);

        //    var caseProfilesPlotSelected = result.Single(x => x.Name == "CaseProfilesPlotSelected");
        //    Assert.Equal("True", caseProfilesPlotSelected.Value);

        //    var displayLegend = result.Single(x => x.Name == "DisplayLegend");
        //    Assert.Equal("False", displayLegend.Value);

        //    var firstCatFactor = result.Single(x => x.Name == "FirstCatFactor");
        //    Assert.Equal("Cat1", firstCatFactor.Value);

        //    var histogramSelected = result.Single(x => x.Name == "HistogramSelected");
        //    Assert.Equal("True", histogramSelected.Value);

        //    var jitterSelected = result.Single(x => x.Name == "JitterSelected");
        //    Assert.Equal("False", jitterSelected.Value);

        //    var linearFitSelected = result.Single(x => x.Name == "LinearFitSelected");
        //    Assert.Equal("False", linearFitSelected.Value);

        //    var mainTitle = result.Single(x => x.Name == "MainTitle");
        //    Assert.Equal("The Title", mainTitle.Value);

        //    var normalDistSelected = result.Single(x => x.Name == "NormalDistSelected");
        //    Assert.Equal("False", normalDistSelected.Value);

        //    var outliersSelected = result.Single(x => x.Name == "OutliersSelected");
        //    Assert.Equal("False", outliersSelected.Value);

        //    var referenceLine = result.Single(x => x.Name == "ReferenceLine");
        //    Assert.Equal("5", referenceLine.Value);

        //    var response = result.Single(x => x.Name == "Response");
        //    Assert.Equal("Resp 1", response.Value);

        //    var responseTransformation = result.Single(x => x.Name == "ResponseTransformation");
        //    Assert.Equal("None", responseTransformation.Value);

        //    var semPlotIncludeData = result.Single(x => x.Name == "SEMPlotIncludeData");
        //    Assert.Equal("False", semPlotIncludeData.Value);

        //    var semPlotSelected = result.Single(x => x.Name == "SEMPlotSelected");
        //    Assert.Equal("True", semPlotSelected.Value);

        //    var semType = result.Single(x => x.Name == "SEMType");
        //    Assert.Equal("Column", semType.Value);

        //    var scatterplotSelected = result.Single(x => x.Name == "ScatterplotSelected");
        //    Assert.Equal("True", scatterplotSelected.Value);

        //    var secondCatFactor = result.Single(x => x.Name == "SecondCatFactor");
        //    Assert.Equal("Cat2", secondCatFactor.Value);

        //    var styleType = result.Single(x => x.Name == "StyleType");
        //    Assert.Equal("Overlaid", styleType.Value);

        //    var xAxis = result.Single(x => x.Name == "XAxis");
        //    Assert.Equal("Treatment", xAxis.Value);

        //    var xAxisTitle = result.Single(x => x.Name == "XAxisTitle");
        //    Assert.Equal("The x axis", xAxisTitle.Value);

        //    var xAxisTransformation = result.Single(x => x.Name == "XAxisTransformation");
        //    Assert.Equal("None", xAxisTransformation.Value);

        //    var yAxisTitle = result.Single(x => x.Name == "YAxisTitle");
        //    Assert.Equal("The y axis", yAxisTitle.Value);
        //}

        //[Fact]
        //public void LoadArguments_ReturnsCorrectArguments()
        //{
        //    //Arrange
        //    GraphicalAnalysisModel sut = new GraphicalAnalysisModel(GetDataset());

        //    List<Argument> arguments = new List<Argument>();
        //    arguments.Add(new Argument { Name = "BoxPlotIncludeData", Value = "False" });
        //    arguments.Add(new Argument { Name = "BoxplotSelected", Value = "False" });
        //    arguments.Add(new Argument { Name = "CaseIDFactor", Value = "Treat 1" });
        //    arguments.Add(new Argument { Name = "CaseProfilesPlotSelected", Value = "True" });
        //    arguments.Add(new Argument { Name = "DisplayLegend", Value = "False" });
        //    arguments.Add(new Argument { Name = "FirstCatFactor", Value = "Cat1" });
        //    arguments.Add(new Argument { Name = "HistogramSelected", Value = "True" });
        //    arguments.Add(new Argument { Name = "JitterSelected", Value = "False" });
        //    arguments.Add(new Argument { Name = "LinearFitSelected", Value = "False" });
        //    arguments.Add(new Argument { Name = "MainTitle", Value = "The Title" });
        //    arguments.Add(new Argument { Name = "NormalDistSelected", Value = "False" });
        //    arguments.Add(new Argument { Name = "OutliersSelected", Value = "False" });
        //    arguments.Add(new Argument { Name = "ReferenceLine", Value = "5" });
        //    arguments.Add(new Argument { Name = "Response", Value = "Resp 1" });
        //    arguments.Add(new Argument { Name = "ResponseTransformation", Value = "None" });
        //    arguments.Add(new Argument { Name = "SEMPlotIncludeData", Value = "False" });
        //    arguments.Add(new Argument { Name = "SEMPlotSelected", Value = "True" });
        //    arguments.Add(new Argument { Name = "SEMType", Value = "Column" });
        //    arguments.Add(new Argument { Name = "ScatterplotSelected", Value = "True" });
        //    arguments.Add(new Argument { Name = "SecondCatFactor", Value = "Cat2" });
        //    arguments.Add(new Argument { Name = "StyleType", Value = "Overlaid" });
        //    arguments.Add(new Argument { Name = "XAxis", Value = "Treatment" });
        //    arguments.Add(new Argument { Name = "XAxisTitle", Value = "The x axis" });
        //    arguments.Add(new Argument { Name = "XAxisTransformation", Value = "None" });
        //    arguments.Add(new Argument { Name = "YAxisTitle", Value = "The y axis" });

        //    Assert.Equal(25, arguments.Count);

        //    //Act
        //    sut.LoadArguments(arguments);

        //    //Assert
        //    Assert.False(sut.BoxPlotIncludeData);
        //    Assert.False(sut.BoxplotSelected);
        //    Assert.Equal("Treat 1", sut.CaseIDFactor);
        //    Assert.True(sut.CaseProfilesPlotSelected);
        //    Assert.False(sut.DisplayLegend);
        //    Assert.Equal("Cat1", sut.FirstCatFactor);
        //    Assert.True(sut.HistogramSelected);
        //    Assert.False(sut.JitterSelected);
        //    Assert.False(sut.LinearFitSelected);
        //    Assert.Equal("The Title", sut.MainTitle);
        //    Assert.False(sut.NormalDistSelected);
        //    Assert.False(sut.OutliersSelected);
        //    Assert.Equal(5, sut.ReferenceLine);
        //    Assert.Equal("Resp 1", sut.Response);
        //    Assert.Equal("None", sut.ResponseTransformation);
        //    Assert.False(sut.SEMPlotIncludeData);
        //    Assert.True(sut.SEMPlotSelected);
        //    Assert.Equal(GraphicalAnalysisModel.SEMPlotType.Column, sut.SEMType);
        //    Assert.True(sut.ScatterplotSelected);
        //    Assert.Equal("Cat2", sut.SecondCatFactor);
        //    Assert.Equal(GraphicalAnalysisModel.GraphStyleType.Overlaid, sut.StyleType);
        //    Assert.Equal("Treatment", sut.XAxis);
        //    Assert.Equal("The x axis", sut.XAxisTitle);
        //    Assert.Equal("None", sut.XAxisTransformation);
        //    Assert.Equal("The y axis", sut.YAxisTitle);
        //}

        //[Fact]
        //public void GetCommandLineArguments_ReturnsCorrectString()
        //{
        //    //Arrange
        //    GraphicalAnalysisModel sut = GetModel(GetDataset());

        //    //Act
        //    string result = sut.GetCommandLineArguments();

        //    //Assert
        //    Assert.Equal("Treatment None Respivs_sp_ivs1 None Cat1 Cat2 Overlaid \"The Title\" \"The x axis\" \"The y axis\" Y N N N N Y Column Y N Y Treativs_sp_ivs1 5 N N N", result);
        //}


        private GraphicalAnalysisModel GetModel(IDataset dataset)
        {
            var model = new SilveR.StatsModels.GraphicalAnalysisModel(dataset)
            {
                BoxPlotIncludeData = false,
                BoxplotSelected = false,
                CaseIDFactor = "Treat 1",
                CaseProfilesPlotSelected = true,
                DatasetID = 8,
                DisplayLegend = false,
                FirstCatFactor = "Cat1",
                HistogramSelected = true,
                JitterSelected = false,
                LinearFitSelected = false,
                MainTitle = "The Title",
                NormalDistSelected = false,
                OutliersSelected = false,
                ReferenceLine = 5,
                Response = "Resp 1",
                ResponseTransformation = "None",
                SEMPlotIncludeData = false,
                SEMPlotSelected = true,
                SEMType = GraphicalAnalysisModel.SEMPlotType.Column,
                ScatterplotSelected = true,
                SecondCatFactor = "Cat2",
                StyleType = GraphicalAnalysisModel.GraphStyleType.Overlaid,
                XAxis = "Treatment",
                XAxisTitle = "The x axis",
                XAxisTransformation = "None",
                YAxisTitle = "The y axis"
            };

            return model;
        }

        private DataTable GetTestDataTable()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("SilveRSelected");
            dt.Columns.Add("Resp 1");
            dt.Columns.Add("Resp2");
            dt.Columns.Add("Treat 1");
            dt.Columns.Add("Cat1");
            dt.Columns.Add("Cat2");
            dt.Columns.Add("Time 1");
            dt.Columns.Add("Time2");
            dt.Columns.Add("Anima 1");
            dt.Columns.Add("Animal2");
            dt.Columns.Add("Resp3");
            dt.Columns.Add("Resp4");
            dt.Columns.Add("Cat3");
            dt.Columns.Add("Cat4");
            dt.Columns.Add("Resp5");
            dt.Columns.Add("log10Resp1");
            dt.Columns.Add("log10Resp2");
            dt.Columns.Add("logeResp1");
            dt.Columns.Add("logeResp2");
            dt.Columns.Add("SqrtResp1");
            dt.Columns.Add("SqrtResp2");
            dt.Columns.Add("Cat5");
            dt.Columns.Add("Cat6");
            dt.Columns.Add("Cat7");
            dt.Columns.Add("Cat8");
            dt.Columns.Add("Resp6");
            dt.Columns.Add("Resp7");
            dt.Columns.Add("Resp8");
            dt.Columns.Add("Resp9");
            dt.Columns.Add("ArcsineResp1");
            dt.Columns.Add("ArcsineResp2");
            dt.Columns.Add("Resp10");
            dt.Columns.Add("brain region");
            dt.Columns.Add("Treatment");

            dt.Rows.Add(new object[] { "True", "0.644", "0.353", "T1", "1", "A", "1", "T1", "1", "A1", "-1", "0", "T1", "", "8", "-0.191114133", "-0.452225295", "-0.440056553", "-1.041287222", "0.802496106", "0.594138031", "B", "q", "w", "w", "0", "1", "-0.353", "1.2", "0.931467009162559", "0.63619360738506", "0.8712014", "pre-frontal", "control", });
            dt.Rows.Add(new object[] { "True", "0.894", "0.234", "T1", "1", "A", "2", "T2", "1", "A1", "0.894", "0.894", "T1", "A", "0.894", "-0.048662481", "-0.630784143", "-0.112049504", "-1.452434164", "0.945515732", "0.483735465", "B", "q", "w", "w", "0.894", "1", "-0.234", "0.894", "1.23917503563726", "0.504917760638409", "0.4457013", "motor cortex", "control", });
            dt.Rows.Add(new object[] { "True", "0.872", "0.914", "T1", "1", "A", "3", "T3", "1", "A1", "0.872", "0.872", "T1", "A", "", "-0.059483515", "-0.039053804", "-0.136965855", "-0.089924708", "0.933809402", "0.956033472", "C", "q", "w", "w", "0.872", "0.424327171599147", "-0.914", "0.872", "1.20491665734408", "1.27316388480605", "0.7168289", "striatum", "control", });
            dt.Rows.Add(new object[] { "True", "0.746", "0.703", "T1", "1", "A", "4", "T4", "1", "A1", "0.746", "0.746", "T1", "A", "0.746", "-0.127261173", "-0.153044675", "-0.293029679", "-0.352398387", "0.863712915", "0.838450953", "D", "q", "w", "w", "0.746", "0.528479275227173", "-0.703", "0.746", "1.04259093568174", "0.994434567822677", "1.15952", "hippocampus", "control", });
            dt.Rows.Add(new object[] { "True", "0.805", "0.983", "T1", "1", "A", "5", "T5", "1", "A1", "0.805", "0.805", "T1", "A", "0.805", "-0.09420412", "-0.007446482", "-0.216913002", "-0.017146159", "0.897217922", "0.991463565", "E", "q", "w", "w", "0.805", "0.406289006070131", "-0.983", "0.805", "1.11342845885991", "1.44004000221474", "1.258102", "raphe", "control", });
            dt.Rows.Add(new object[] { "True", "0.439", "0.72", "T1", "1", "A", "6", "T6", "1", "A1", "0.439", "0.439", "T1", "A", "0.439", "-0.35753548", "-0.142667504", "-0.823255866", "-0.328504067", "0.662570751", "0.848528137", "B", "q", "w", "w", "0.439", "1", "-0.72", "0.439", "0.724245820143992", "1.01319750009536", "1.126796", "pre-frontal", "control", });
            dt.Rows.Add(new object[] { "True", "0.734", "0.225", "T1", "2", "A", "1", "T1", "3", "A3", "0.734", "0.734", "T1", "A", "0.734", "-0.13430394", "-0.647817482", "-0.30924625", "-1.491654877", "0.856738", "0.474341649", "C", "q", "w", "w", "0.734", "0.258176490839118", "-0.225", "0.734", "1.02891130322965", "0.494216044463077", "1.41515", "striatum", "control", });
            dt.Rows.Add(new object[] { "True", "0.718", "0.121", "T1", "2", "A", "2", "T2", "3", "A3", "0.718", "0.718", "T1", "A", "0.718", "-0.143875556", "-0.91721463", "-0.33128571", "-2.111964733", "0.847348807", "0.347850543", "D", "q", "w", "w", "0.718", "0.762643530979843", "-0.121", "0.718", "1.01097274091135", "0.355277493460343", "0.5770834", "hippocampus", "control", });
            dt.Rows.Add(new object[] { "True", "0.415", "0.288", "T1", "2", "A", "3", "T3", "3", "A3", "0.415", "0.415", "T1", "A", "0.415", "-0.381951903", "-0.540607512", "-0.879476759", "-1.244794799", "0.644204936", "0.536656315", "E", "q", "w", "w", "0.415", "0.742850005414899", "-0.288", "0.415", "0.699983328832896", "0.566469445702335", "0.2261314", "raphe", "control", });
            dt.Rows.Add(new object[] { "True", "0.641", "0.406", "T1", "2", "A", "4", "T4", "3", "A3", "0.641", "0.641", "T1", "A", "0.641", "-0.19314197", "-0.391473966", "-0.444725822", "-0.901402119", "0.800624756", "0.637181293", "B", "q", "e", "w", "0.641", "1", "-0.406", "0.641", "0.928337202093871", "0.690835444027313", "1.514939", "pre-frontal", "control", });
            dt.Rows.Add(new object[] { "True", "0.832", "0.209", "T1", "2", "A", "5", "T5", "3", "A3", "0.832", "0.832", "T1", "A", "0.832", "-0.079876674", "-0.679853714", "-0.183922838", "-1.565421027", "0.91214034", "0.457165178", "B", "q", "e", "w", "0.832", "1", "-0.209", "0.832", "1.14847598638454", "0.474805170480551", "1.056859", "motor cortex", "control", });
            dt.Rows.Add(new object[] { "True", "0.259", "0.889", "T1", "2", "A", "6", "T6", "3", "A3", "0.259", "0.259", "T1", "A", "0.259", "-0.586700236", "-0.051098239", "-1.350927217", "-0.117658043", "0.508920426", "0.942867965", "C", "q", "e", "w", "0.259", "0.741230179727283", "-0.889", "0.259", "0.533930192477715", "1.23113623272659", "0.7967658", "striatum", "control", });
            dt.Rows.Add(new object[] { "True", "0.459", "0.076", "T1", "1", "A", "1", "T1", "13", "A13", "0.459", "0.459", "T1", "A", "0.459", "-0.338187314", "-1.119186408", "-0.778705069", "-2.577021939", "0.677495387", "0.275680975", "", "", "", "", "", "", "-0.076", "", "0.744352076477674", "0.279298057669285", "2.692527", "striatum", "24h", });
            dt.Rows.Add(new object[] { "True", "0.418", "0.466", "T1", "1", "A", "2", "T2", "13", "A13", "0.418", "0.418", "T1", "A", "0.418", "-0.378823718", "-0.331614083", "-0.872273846", "-0.763569645", "0.646529195", "0.682641927", "", "", "", "", "", "", "-0.466", "", "0.703026063298165", "0.751371906057732", "4.045331", "hippocampus", "24h", });
            dt.Rows.Add(new object[] { "True", "0.074", "0.422", "T1", "1", "A", "3", "T3", "13", "A13", "0.074", "0.074", "T1", "A", "0.074", "-1.13076828", "-0.374687549", "-2.603690186", "-0.862749965", "0.27202941", "0.649615271", "", "", "", "", "", "", "-0.422", "", "0.275501345316258", "0.707078279754482", "2.881532", "raphe", "24h", });
            dt.Rows.Add(new object[] { "True", "0.786", "0.134", "T1", "1", "A", "4", "T4", "13", "A13", "0.786", "0.786", "T1", "A", "0.786", "-0.104577454", "-0.872895202", "-0.240798487", "-2.009915479", "0.88656641", "0.366060104", "", "", "", "", "", "", "-0.134", "", "1.08986919263486", "0.374771721171355", "2.762485", "pre-frontal", "24h", });
            dt.Rows.Add(new object[] { "True", "0.801", "0.966", "T1", "1", "A", "5", "T5", "13", "A13", "0.801", "0.801", "T1", "A", "0.801", "-0.096367484", "-0.015022874", "-0.221894332", "-0.034591445", "0.894986033", "0.98285299", "", "", "", "", "", "", "-0.966", "", "1.10839989317912", "1.38534423803476", "3.757828", "motor cortex", "24h", });
            dt.Rows.Add(new object[] { "True", "0.746", "0.233", "T1", "1", "A", "6", "T6", "13", "A13", "0.746", "0.746", "T1", "A", "0.746", "-0.127261173", "-0.632644079", "-0.293029679", "-1.456716825", "0.863712915", "0.482700735", "", "", "", "", "", "", "-0.233", "", "1.04259093568174", "0.503735887930972", "1.844135", "striatum", "24h", });
            dt.Rows.Add(new object[] { "True", "0.933", "0.012", "T1", "1", "B", "1", "T1", "2", "A2", "0.933", "0.933", "T1", "B", "0.933", "-0.030118356", "-1.920818754", "-0.069350078", "-4.422848629", "0.965919251", "0.109544512", "B", "q", "w", "w", "0.933", "1", "-0.012", "0.933", "1.30897153632898", "0.109764792124965", "0.6660721", "motor cortex", "control", });
            dt.Rows.Add(new object[] { "True", "0.147", "0.785", "T1", "1", "B", "2", "T2", "2", "A2", "0.147", "0.147", "T1", "B", "0.147", "-0.832682665", "-0.105130343", "-1.917322692", "-0.242071561", "0.38340579", "0.886002257", "C", "q", "w", "w", "0.147", "0.375655614758385", "-0.785", "0.147", "0.393481082973961", "1.08865109100399", "1.071255", "striatum", "control", });
            dt.Rows.Add(new object[] { "True", "0.102", "0.008", "T1", "1", "B", "3", "T3", "2", "A2", "0.102", "0.102", "T1", "B", "0.102", "-0.991399828", "-2.096910013", "-2.282782466", "-4.828313737", "0.319374388", "0.089442719", "D", "q", "w", "w", "0.102", "0.525119542098466", "-0.008", "0.102", "0.325069227363818", "0.0895624074394449", "0.4557167", "hippocampus", "control", });
            dt.Rows.Add(new object[] { "True", "0.94", "0.239", "T1", "1", "B", "4", "T4", "2", "A2", "0.94", "0.94", "T1", "B", "0.94", "-0.026872146", "-0.621602099", "-0.061875404", "-1.431291727", "0.969535971", "0.488876262", "E", "q", "w", "w", "0.94", "0.410349949036843", "-0.239", "0.94", "1.32332926362445", "0.510801119116815", "1.691523", "raphe", "control", });
            dt.Rows.Add(new object[] { "True", "0.193", "0.194", "T1", "1", "B", "5", "T5", "2", "A2", "0.193", "0.193", "T1", "B", "0.193", "-0.714442691", "-0.71219827", "-1.64506509", "-1.63989712", "0.439317653", "0.440454311", "B", "q", "w", "w", "0.193", "1", "-0.194", "0.193", "0.454838960643144", "0.456104651547548", "0.4870635", "pre-frontal", "control", });
            dt.Rows.Add(new object[] { "True", "0.127", "0.988", "T1", "1", "B", "6", "T6", "2", "A2", "0.127", "0.127", "T1", "B", "0.127", "-0.896196279", "-0.005243055", "-2.063568193", "-0.012072581", "0.356370594", "0.993981891", "B", "q", "w", "w", "0.127", "1", "-0.988", "0.127", "0.364380561299932", "1.46103153466993", "1.831367", "motor cortex", "control", });
            dt.Rows.Add(new object[] { "True", "0.418", "0.747", "T1", "2", "B", "1", "T1", "4", "A4", "0.418", "0.418", "T1", "B", "0.418", "-0.378823718", "-0.126679398", "-0.872273846", "-0.291690094", "0.646529195", "0.864291617", "D", "q", "e", "w", "0.418", "0.832922101643078", "-0.747", "0.418", "0.703026063298165", "1.0437403227437", "1.80768", "hippocampus", "control", });
            dt.Rows.Add(new object[] { "True", "0.72", "0.538", "T1", "2", "B", "2", "T2", "4", "A4", "0.72", "0.72", "T1", "B", "0.72", "-0.142667504", "-0.269217724", "-0.328504067", "-0.619896719", "0.848528137", "0.733484833", "E", "q", "e", "w", "0.72", "0.339287236668002", "-0.538", "0.72", "1.01319750009536", "0.823434840141176", "0.8242433", "raphe", "control", });
            dt.Rows.Add(new object[] { "True", "0.152", "0.7", "T1", "2", "B", "3", "T3", "4", "A4", "0.152", "0.152", "T1", "B", "0.152", "-0.818156412", "-0.15490196", "-1.883874758", "-0.356674944", "0.389871774", "0.836660027", "B", "q", "e", "w", "0.152", "1", "-0.7", "0.152", "0.400492343832776", "0.991156586431192", "", "pre-frontal", "control", });
            dt.Rows.Add(new object[] { "True", "0.247", "0.48", "T1", "2", "B", "4", "T4", "4", "A4", "0.247", "0.247", "T1", "B", "0.247", "-0.607303047", "-0.318758763", "-1.398366942", "-0.733969175", "0.496990946", "0.692820323", "B", "q", "e", "w", "0.247", "1", "-0.48", "0.247", "0.520127689962925", "0.765392826220454", "", "motor cortex", "control", });
            dt.Rows.Add(new object[] { "True", "0.836", "0.32", "T1", "2", "B", "5", "T5", "4", "A4", "0.836", "0.836", "T1", "B", "0.836", "-0.077793723", "-0.494850022", "-0.179126666", "-1.139434283", "0.914330356", "0.565685425", "C", "q", "e", "w", "0.836", "0.351278316449618", "-0.32", "0.836", "1.15385125378176", "0.601264216679128", "", "striatum", "control", });
            dt.Rows.Add(new object[] { "True", "0.081", "0.987", "T1", "2", "B", "6", "T6", "4", "A4", "0.081", "0.081", "T1", "B", "0.081", "-1.091514981", "-0.005682847", "-2.513306124", "-0.01308524", "0.284604989", "0.993478737", "D", "q", "e", "w", "0.081", "0.115672851694452", "-0.987", "0.081", "0.288594351950617", "1.45653028982108", "", "hippocampus", "control", });
            dt.Rows.Add(new object[] { "True", "0.655", "0.811", "T1", "1", "B", "1", "T1", "14", "A14", "0.655", "0.655", "T1", "B", "0.655", "-0.1837587", "-0.090979146", "-0.423120043", "-0.209487225", "0.809320703", "0.900555384", "", "", "", "", "", "", "-0.811", "", "0.94299467961781", "1.12104533447375", "4.315698", "hippocampus", "24h", });
            dt.Rows.Add(new object[] { "True", "0.267", "0.139", "T1", "1", "B", "2", "T2", "14", "A14", "0.267", "0.267", "T1", "B", "0.267", "-0.573488739", "-0.8569852", "-1.320506621", "-1.973281346", "0.516720427", "0.372827038", "", "", "", "", "", "", "-0.139", "", "0.543015916546275", "0.382053865129781", "5.091865", "raphe", "24h", });
            dt.Rows.Add(new object[] { "True", "0.979", "0.134", "T1", "1", "B", "3", "T3", "14", "A14", "0.979", "0.979", "T1", "B", "0.979", "-0.009217308", "-0.872895202", "-0.021223636", "-2.009915479", "0.989444288", "0.366060104", "", "", "", "", "", "", "-0.134", "", "1.42537050734153", "0.374771721171355", "", "pre-frontal", "24h", });
            dt.Rows.Add(new object[] { "True", "0.756", "0.458", "T1", "1", "B", "4", "T4", "14", "A14", "0.756", "0.756", "T1", "B", "0.756", "-0.121478204", "-0.339134522", "-0.279713903", "-0.780886095", "0.869482605", "0.676756973", "", "", "", "", "", "", "-0.458", "", "1.05415391696227", "0.743348613906107", "", "motor cortex", "24h", });
            dt.Rows.Add(new object[] { "True", "0.146", "0.16", "T1", "1", "B", "5", "T5", "14", "A14", "0.146", "0.146", "T1", "B", "0.146", "-0.835647144", "-0.795880017", "-1.924148657", "-1.832581464", "0.382099463", "0.4", "", "", "", "", "", "", "-0.16", "", "0.39206708102556", "0.411516846067488", "", "striatum", "24h", });
            dt.Rows.Add(new object[] { "True", "0.693", "0.972", "T1", "1", "B", "6", "T6", "14", "A14", "0.693", "0.693", "T1", "B", "0.693", "-0.159266765", "-0.012333735", "-0.36672528", "-0.028399475", "0.832466216", "0.985900604", "", "", "", "", "", "", "-0.972", "", "0.983543958050574", "1.40267343249868", "", "hippocampus", "24h", });
            dt.Rows.Add(new object[] { "True", "0.774", "0.068", "T1", "2", "B", "1", "T1", "15", "A15", "0.774", "0.774", "T1", "B", "0.774", "-0.111259039", "-1.167491087", "-0.256183405", "-2.688247574", "0.879772698", "0.260768096", "", "", "", "", "", "", "-0.068", "", "1.07538385518686", "0.263817741132391", "", "raphe", "24h", });
            dt.Rows.Add(new object[] { "True", "0.926", "0.056", "T1", "2", "B", "2", "T2", "15", "A15", "0.926", "0.926", "T1", "B", "0.926", "-0.033389013", "-1.251811973", "-0.076881044", "-2.882403588", "0.962288938", "0.236643191", "", "", "", "", "", "", "-0.056", "", "1.29529498147864", "0.238909448626102", "", "pre-frontal", "24h", });
            dt.Rows.Add(new object[] { "True", "0.828", "0.846", "T1", "2", "B", "3", "T3", "15", "A15", "0.828", "0.828", "T1", "B", "0.828", "-0.081969663", "-0.072629637", "-0.188742125", "-0.167235919", "0.909945053", "0.919782583", "", "", "", "", "", "", "-0.846", "", "1.14315155429591", "1.16752609519738", "", "motor cortex", "24h", });
            dt.Rows.Add(new object[] { "True", "0.4", "0.968", "T1", "2", "B", "4", "T4", "15", "A15", "0.4", "0.4", "T1", "B", "0.4", "-0.397940009", "-0.014124643", "-0.916290732", "-0.032523192", "0.632455532", "0.98386991", "", "", "", "", "", "", "-0.968", "", "0.684719203002283", "1.39094282700242", "", "striatum", "24h", });
            dt.Rows.Add(new object[] { "True", "0.283", "0.88", "T1", "2", "B", "5", "T5", "15", "A15", "0.283", "0.283", "T1", "B", "0.283", "-0.548213564", "-0.055517328", "-1.262308381", "-0.127833372", "0.531977443", "0.938083152", "", "", "", "", "", "", "-0.88", "", "0.560934166213867", "1.21705472090523", "", "hippocampus", "24h", });
            dt.Rows.Add(new object[] { "True", "0.384", "0.761", "T1", "2", "B", "6", "T6", "15", "A15", "0.384", "0.384", "T1", "B", "0.384", "-0.415668776", "-0.118615343", "-0.957112726", "-0.273121921", "0.619677335", "0.87235314", "", "", "", "", "", "", "0.761", "", "0.668331523670804", "1.05999520767808", "", "raphe", "24h", });
            dt.Rows.Add(new object[] { "True", "0.054", "0.243", "T2", "1", "A", "1", "T1", "5", "A5", "0.054", "0.054", "T1", "A", "0.054", "-1.26760624", "-0.614393726", "-2.918771232", "-1.414693836", "0.232379001", "0.492950302", "E", "q", "e", "w", "0.054", "0.234403491215984", "0.243", "0.054", "0.234522929132543", "0.515477435762801", "", "raphe", "control", });
            dt.Rows.Add(new object[] { "True", "0.607", "0.253", "T2", "1", "A", "2", "T2", "5", "A5", "0.607", "0.607", "T1", "A", "0.607", "-0.216811309", "-0.596879479", "-0.499226488", "-1.37436579", "0.779102047", "0.502991054", "B", "q", "e", "w", "0.607", "1", "0.253", "0.607", "0.893232162945372", "0.527056004051193", "", "pre-frontal", "control", });
            dt.Rows.Add(new object[] { "True", "0.364", "0.378", "T2", "1", "A", "3", "T3", "5", "A5", "0.364", "0.364", "T1", "A", "0.364", "-0.438898616", "-0.4225082", "-1.010601411", "-0.972861083", "0.603324125", "0.614817046", "B", "q", "e", "w", "0.364", "1", "0.378", "0.364", "0.647662772037123", "0.66215396751091", "", "motor cortex", "control", });
            dt.Rows.Add(new object[] { "True", "0.524", "0.142", "T2", "1", "A", "4", "T4", "5", "A5", "0.524", "0.524", "T1", "A", "0.524", "-0.280668713", "-0.847711656", "-0.646263595", "-1.951928221", "0.723878443", "0.376828874", "C", "q", "e", "w", "0.524", "0.288014232174776", "0.142", "0.524", "0.809407388965722", "0.386370405781698", "", "striatum", "control", });
            dt.Rows.Add(new object[] { "True", "0.256", "0.423", "T2", "1", "A", "5", "T5", "5", "A5", "0.256", "0.256", "T1", "A", "0.256", "-0.591760035", "-0.373659633", "-1.362577835", "-0.8603831", "0.505964426", "0.650384502", "D", "q", "e", "w", "0.256", "0.79910118847354", "0.423", "0.256", "0.530499703318932", "0.708090513316968", "", "hippocampus", "control", });
            dt.Rows.Add(new object[] { "True", "0.221", "0.491", "T2", "1", "A", "6", "T6", "5", "A5", "0.221", "0.221", "T1", "A", "0.221", "-0.655607726", "-0.308918508", "-1.509592577", "-0.711311151", "0.470106371", "0.700713922", "E", "q", "e", "w", "0.221", "0.184098998640184", "0.491", "0.221", "0.489411292801224", "0.776397677326576", "", "raphe", "control", });
            dt.Rows.Add(new object[] { "True", "0.083", "0.576", "T2", "2", "A", "1", "T1", "7", "A7", "0.083", "0.083", "T1", "A", "0.083", "-1.080921908", "-0.239577517", "-2.488914671", "-0.551647618", "0.288097206", "0.758946638", "B", "r", "w", "w", "0.083", "0.160857259262287", "0.576", "0.083", "0.292239199691185", "0.861693899207304", "0.3674804", "motor cortex", "6h", });
            dt.Rows.Add(new object[] { "True", "0.502", "0.27", "T2", "2", "A", "2", "T2", "7", "A7", "0.502", "0.502", "T1", "A", "0.502", "-0.299296283", "-0.568636236", "-0.689155159", "-1.30933332", "0.708519583", "0.519615242", "C", "r", "w", "w", "0.502", "0.990470214922809", "0.27", "0.502", "0.78739816873082", "0.546400564137972", "0.2258767", "striatum", "6h", });
            dt.Rows.Add(new object[] { "True", "0.994", "0.306", "T2", "2", "A", "3", "T3", "7", "A7", "0.994", "0.994", "T1", "A", "0.994", "-0.002613616", "-0.514278574", "-0.006018072", "-1.184170177", "0.996995486", "0.553172667", "D", "r", "w", "w", "0.994", "0.619307756905664", "0.306", "0.994", "1.49325899031273", "0.586167863485366", "0.2656592", "hippocampus", "6h", });
            dt.Rows.Add(new object[] { "True", "0.872", "0.456", "T2", "2", "A", "4", "T4", "7", "A7", "0.872", "0.872", "T1", "A", "0.872", "-0.059483515", "-0.341035157", "-0.136965855", "-0.785262469", "0.933809402", "0.675277721", "E", "r", "w", "w", "0.872", "0.432807570530263", "0.456", "0.872", "1.20491665734408", "0.741341175247589", "0.3112526", "raphe", "6h", });
            dt.Rows.Add(new object[] { "True", "0.586", "0.751", "T2", "2", "A", "5", "T5", "7", "A7", "0.586", "0.586", "T1", "A", "0.586", "-0.232102384", "-0.124360063", "-0.534435489", "-0.286349627", "0.765506368", "0.866602562", "A", "r", "w", "w", "0.586", "0.814477177708316", "0.751", "0.586", "0.871827947309003", "1.04835302359294", "0.5494713", "pre-frontal", "6h", });
            dt.Rows.Add(new object[] { "True", "0.362", "0.291", "T2", "2", "A", "6", "T6", "7", "A7", "0.362", "0.362", "T1", "A", "0.362", "-0.441291429", "-0.536107011", "-1.016111067", "-1.234432012", "0.601664358", "0.539444158", "B", "r", "w", "w", "0.362", "0.191689273368843", "0.291", "0.362", "0.645583183760138", "0.569776842011205", "3.596838", "motor cortex", "6h", });
            dt.Rows.Add(new object[] { "True", "0.502", "0.412", "T2", "1", "A", "1", "T1", "16", "A16", "0.502", "0.502", "T1", "A", "0.502", "-0.299296283", "-0.385102784", "-0.689155159", "-0.88673193", "0.708519583", "0.641872261", "", "", "", "", "", "", "0.412", "", "0.78739816873082", "0.696937396664296", "22.71566", "pre-frontal", "48h", });
            dt.Rows.Add(new object[] { "True", "0.033", "0.563", "T2", "1", "A", "2", "T2", "16", "A16", "0.033", "0.033", "T1", "A", "0.033", "-1.48148606", "-0.249491605", "-3.411247718", "-0.574475651", "0.181659021", "0.750333259", "", "", "", "", "", "", "0.563", "", "0.182673281012858", "0.848566063698566", "15.48354", "motor cortex", "48h", });
            dt.Rows.Add(new object[] { "True", "0.576", "0.29", "T2", "1", "A", "3", "T3", "16", "A16", "0.576", "0.576", "T1", "A", "0.576", "-0.239577517", "-0.537602002", "-0.551647618", "-1.237874356", "0.758946638", "0.538516481", "", "", "", "", "", "", "0.29", "", "0.861693899207304", "0.568675503362505", "8.811939", "striatum", "48h", });
            dt.Rows.Add(new object[] { "True", "0.529", "0.28", "T2", "1", "A", "4", "T4", "16", "A16", "0.529", "0.529", "T1", "A", "0.529", "-0.276544328", "-0.552841969", "-0.636766847", "-1.272965676", "0.727323862", "0.529150262", "", "", "", "", "", "", "0.28", "", "0.814414447393559", "0.557598826699537", "13.91151", "hippocampus", "48h", });
            dt.Rows.Add(new object[] { "True", "0.413", "0.861", "T2", "1", "A", "5", "T5", "16", "A16", "0.413", "0.413", "T1", "A", "0.413", "-0.384049948", "-0.064996849", "-0.884307686", "-0.149660775", "0.642650761", "0.927900857", "", "", "", "", "", "", "0.861", "", "0.697953070287712", "1.18874246166512", "4.387292", "raphe", "48h", });
            dt.Rows.Add(new object[] { "True", "0.835", "0.096", "T2", "1", "A", "6", "T6", "16", "A16", "0.835", "0.835", "T1", "A", "0.835", "-0.078313525", "-1.017728767", "-0.180323554", "-2.343407088", "0.913783344", "0.309838668", "", "", "", "", "", "", "0.096", "", "1.15250255712413", "0.31502334522292", "0", "pre-frontal", "48h", });
            dt.Rows.Add(new object[] { "True", "0.313", "0.048", "T2", "1", "B", "1", "T1", "6", "A6", "0.313", "0.313", "T1", "B", "0.313", "-0.504455662", "-1.318758763", "-1.161552088", "-3.036554268", "0.559464029", "0.219089023", "A", "r", "w", "w", "0.313", "0.247592645242458", "0.048", "0.313", "0.593739018516901", "0.220880712075029", "1.069939", "pre-frontal", "6h", });
            dt.Rows.Add(new object[] { "True", "0.534", "0.427", "T2", "1", "B", "2", "T2", "6", "A6", "0.534", "0.534", "T1", "B", "0.534", "-0.272458743", "-0.369572125", "-0.62735944", "-0.850971266", "0.730753036", "0.65345237", "B", "r", "w", "w", "0.534", "0.818695926179455", "0.427", "0.534", "0.819424420737164", "0.712136299015676", "3.706018", "motor cortex", "6h", });
            dt.Rows.Add(new object[] { "True", "0.957", "0.226", "T2", "1", "B", "3", "T3", "6", "A6", "0.957", "0.957", "T1", "B", "0.957", "-0.019088062", "-0.645891561", "-0.043951888", "-1.48722028", "0.978263768", "0.475394573", "C", "r", "w", "w", "0.957", "0.624018008430523", "0.226", "0.957", "1.36191628710346", "0.495412471608037", "0.5707607", "striatum", "6h", });
            dt.Rows.Add(new object[] { "True", "0.789", "0.964", "T2", "1", "B", "4", "T4", "6", "A6", "0.789", "0.789", "T1", "B", "0.789", "-0.102922997", "-0.015922966", "-0.236988958", "-0.036663984", "0.88825672", "0.981835017", "D", "r", "w", "w", "0.789", "0.692187796165479", "0.964", "0.789", "1.09353600701607", "1.37990239968315", "0.9130257", "hippocampus", "6h", });
            dt.Rows.Add(new object[] { "True", "0.297", "0.687", "T2", "1", "B", "5", "T5", "6", "A6", "0.297", "0.297", "T1", "B", "0.297", "-0.527243551", "-0.163043263", "-1.21402314", "-0.375420987", "0.544977064", "0.828854631", "E", "r", "w", "w", "0.297", "0.18767314117348", "0.687", "0.297", "0.576361758972219", "0.977057308277996", "0.1848316", "raphe", "6h", });
            dt.Rows.Add(new object[] { "True", "0.752", "0.63", "T2", "1", "B", "6", "T6", "6", "A6", "0.752", "0.752", "T1", "B", "0.752", "-0.123782159", "-0.200659451", "-0.285018955", "-0.46203546", "0.867179336", "0.793725393", "A", "r", "w", "w", "0.752", "0.349246464744859", "0.63", "0.752", "1.04951004797428", "0.916909264851683", "4.711075", "pre-frontal", "6h", });
            dt.Rows.Add(new object[] { "True", "0.765", "0.94", "T2", "2", "B", "1", "T1", "8", "A8", "0.765", "0.765", "T1", "B", "0.765", "-0.116338565", "-0.026872146", "-0.267879445", "-0.061875404", "0.874642784", "0.969535971", "C", "r", "w", "w", "0.765", "0.791680755410424", "0.94", "0.765", "1.06469844606885", "1.32332926362445", "0.4436047", "striatum", "6h", });
            dt.Rows.Add(new object[] { "True", "0.469", "0.54", "T2", "2", "B", "2", "T2", "8", "A8", "0.469", "0.469", "T1", "B", "0.469", "-0.328827157", "-0.26760624", "-0.757152511", "-0.616186139", "0.684835747", "0.734846923", "D", "r", "w", "w", "0.469", "0.308025885771244", "0.54", "0.469", "0.754378268296987", "0.825440953414278", "0.6542987", "hippocampus", "6h", });
            dt.Rows.Add(new object[] { "True", "0.79", "0.41", "T2", "2", "B", "3", "T3", "8", "A8", "0.79", "0.79", "T1", "B", "0.79", "-0.102372909", "-0.387216143", "-0.235722334", "-0.891598119", "0.888819442", "0.640312424", "E", "r", "w", "w", "0.79", "0.333551159384174", "0.41", "0.79", "1.09476250873357", "0.694904937774174", "0.5152627", "raphe", "6h", });
            dt.Rows.Add(new object[] { "True", "0.006", "0.762", "T2", "2", "B", "4", "T4", "8", "A8", "0.006", "0.006", "T1", "B", "0.006", "-2.22184875", "-0.118045029", "-5.11599581", "-0.271808723", "0.077459667", "0.872926114", "A", "r", "e", "e", "0.006", "0.0172633119773322", "0.762", "0.006", "0.0775373364821692", "1.06116845905074", "1.585037", "pre-frontal", "6h", });
            dt.Rows.Add(new object[] { "True", "0.923", "0.765", "T2", "2", "B", "5", "T5", "8", "A8", "0.923", "0.923", "T1", "B", "0.923", "-0.034798299", "-0.116338565", "-0.080126044", "-0.267879445", "0.96072889", "0.874642784", "B", "r", "e", "e", "0.923", "0.754313929379719", "0.765", "0.923", "1.28961712078499", "1.06469844606885", "2.046596", "motor cortex", "6h", });
            dt.Rows.Add(new object[] { "True", "0.972", "0.844", "T2", "2", "B", "6", "T6", "8", "A8", "0.972", "0.972", "T1", "B", "0.972", "-0.012333735", "-0.073657553", "-0.028399475", "-0.169602784", "0.985900604", "0.918694726", "C", "r", "e", "e", "0.972", "0.328427438215669", "0.844", "0.972", "1.40267343249868", "1.16476292360229", "0.5307809", "striatum", "6h", });
            dt.Rows.Add(new object[] { "True", "0.067", "0.731", "T2", "1", "B", "1", "T1", "17", "A17", "0.067", "0.067", "T1", "B", "0.067", "-1.173925197", "-0.136082623", "-2.70306266", "-0.313341819", "0.258843582", "0.85498538", "", "", "", "", "", "", "0.731", "", "0.261824790465914", "1.02552265045343", "22.60261", "motor cortex", "48h", });
            dt.Rows.Add(new object[] { "True", "0.992", "0.667", "T2", "1", "B", "2", "T2", "17", "A17", "0.992", "0.992", "T1", "B", "0.992", "-0.003488328", "-0.175874166", "-0.008032172", "-0.404965233", "0.995991968", "0.81670068", "", "", "", "", "", "", "0.667", "", "1.48123391935545", "0.955670215749808", "7.540906", "striatum", "48h", });
            dt.Rows.Add(new object[] { "True", "0.265", "0.675", "T2", "1", "B", "3", "T3", "17", "A17", "0.265", "0.265", "T1", "B", "0.265", "-0.576754126", "-0.170696227", "-1.328025453", "-0.393042588", "0.514781507", "0.821583836", "", "", "", "", "", "", "0.675", "", "0.54075277439039", "0.964183715220204", "16.62212", "hippocampus", "48h", });
            dt.Rows.Add(new object[] { "True", "0.862", "0.568", "T2", "1", "B", "4", "T4", "17", "A17", "0.862", "0.862", "T1", "B", "0.862", "-0.064492734", "-0.245651664", "-0.148500008", "-0.56563386", "0.928439551", "0.753657747", "", "", "", "", "", "", "0.568", "", "1.19018996017966", "0.853609548904732", "0", "raphe", "48h", });
            dt.Rows.Add(new object[] { "True", "0.77", "0.607", "T2", "1", "B", "5", "T5", "17", "A17", "0.77", "0.77", "T1", "B", "0.77", "-0.113509275", "-0.216811309", "-0.261364764", "-0.499226488", "0.877496439", "0.779102047", "", "", "", "", "", "", "0.607", "", "1.07061671809741", "0.893232162945372", "10.12045", "pre-frontal", "48h", });
            dt.Rows.Add(new object[] { "True", "0.244", "0.691", "T2", "1", "B", "6", "T6", "17", "A17", "0.244", "0.244", "T1", "B", "0.244", "-0.612610174", "-0.160521953", "-1.410587054", "-0.369615455", "0.493963561", "0.831264098", "", "", "", "", "", "", "0.691", "", "0.516642409832622", "0.981377889931325", "8.522217", "motor cortex", "48h", });
            dt.Rows.Add(new object[] { "True", "0.644", "0.245", "T2", "2", "B", "1", "T1", "18", "A18", "0.644", "0.644", "T1", "B", "0.644", "-0.191114133", "-0.610833916", "-0.440056553", "-1.406497068", "0.802496106", "0.494974747", "", "", "", "", "", "", "0.245", "", "0.931467009162559", "0.517805768259648", "10.41242", "striatum", "48h", });
            dt.Rows.Add(new object[] { "True", "0.353", "0.423", "T2", "2", "B", "2", "T2", "18", "A18", "0.353", "0.353", "T1", "B", "0.353", "-0.452225295", "-0.373659633", "-1.041287222", "-0.8603831", "0.594138031", "0.650384502", "", "", "", "", "", "", "0.423", "", "0.63619360738506", "0.708090513316968", "4.816405", "hippocampus", "48h", });
            dt.Rows.Add(new object[] { "True", "0.402", "0.247", "T2", "2", "B", "3", "T3", "18", "A18", "0.402", "0.402", "T1", "B", "0.402", "-0.395773947", "-0.607303047", "-0.91130319", "-1.398366942", "0.634034699", "0.496990946", "", "", "", "", "", "", "0.247", "", "0.686759600304901", "0.520127689962925", "3.538163", "raphe", "48h", });
            dt.Rows.Add(new object[] { "True", "0.051", "0.511", "T2", "2", "B", "4", "T4", "18", "A18", "0.051", "0.051", "T1", "B", "0.051", "-1.292429824", "-0.2915791", "-2.975929646", "-0.671385689", "0.225831796", "0.71484264", "", "", "", "", "", "", "0.511", "", "0.227796805805005", "0.796399050924099", "29.12329", "pre-frontal", "48h", });
            dt.Rows.Add(new object[] { "True", "0.808", "0.51", "T2", "2", "B", "5", "T5", "18", "A18", "0.808", "0.808", "T1", "B", "0.808", "-0.092588639", "-0.292429824", "-0.21319322", "-0.673344553", "0.898888202", "0.714142843", "", "", "", "", "", "", "0.51", "", "1.11722555433084", "0.795398830184143", "11.63721", "motor cortex", "48h", });
            dt.Rows.Add(new object[] { "True", "0.769", "0.114", "T2", "2", "B", "6", "T6", "18", "A18", "0.769", "0.769", "T1", "B", "0.769", "-0.11407366", "-0.943095149", "-0.262664309", "-2.171556831", "0.876926451", "0.33763886", "", "", "", "", "", "", "0.114", "", "1.06942950014606", "0.344407318980734", "11.85564", "striatum", "48h", });
            dt.Rows.Add(new object[] { "True", "0.003", "0.572", "T3", "1", "A", "1", "T1", "9", "A9", "0.003", "0.003", "T1", "A", "0.003", "-2.522878745", "-0.242603971", "-5.80914299", "-0.558616288", "0.054772256", "0.756306816", "D", "r", "e", "e", "0.003", "0.697197716081965", "0.572", "0.003", "0.0547996789158197", "0.857649346368212", "0.5195104", "hippocampus", "6h", });
            dt.Rows.Add(new object[] { "True", "0.71", "0.281", "T3", "1", "A", "2", "T2", "9", "A9", "0.71", "0.71", "T1", "A", "0.71", "-0.148741651", "-0.55129368", "-0.342490309", "-1.26940061", "0.842614977", "0.530094331", "E", "r", "e", "e", "0.71", "0.390096640975123", "0.281", "0.71", "1.00212082343239", "0.558711809175266", "0.4979516", "raphe", "6h", });
            dt.Rows.Add(new object[] { "True", "0.982", "0.076", "T3", "1", "A", "3", "T3", "9", "A9", "0.982", "0.982", "T1", "A", "0.982", "-0.007888512", "-1.119186408", "-0.018163971", "-2.577021939", "0.990959131", "0.275680975", "A", "r", "e", "e", "0.982", "0.350438016687117", "0.076", "0.982", "1.43622646035762", "0.279298057669285", "", "pre-frontal", "6h", });
            dt.Rows.Add(new object[] { "True", "0.04", "0.07", "T3", "1", "A", "4", "T4", "9", "A9", "0.04", "0.04", "T1", "A", "0.04", "-1.397940009", "-1.15490196", "-3.218875825", "-2.659260037", "0.2", "0.264575131", "B", "r", "e", "e", "0.04", "0.863088884178717", "0.07", "0.04", "0.201357920790331", "0.267763327157194", "", "motor cortex", "6h", });
            dt.Rows.Add(new object[] { "True", "0.693", "0.164", "T3", "1", "A", "5", "T5", "9", "A9", "0.693", "0.693", "T1", "A", "0.693", "-0.159266765", "-0.785156152", "-0.36672528", "-1.807888851", "0.832466216", "0.404969135", "C", "r", "e", "e", "0.693", "0.607258519177342", "0.164", "0.693", "0.983543958050574", "0.416945073013139", "", "striatum", "6h", });
            dt.Rows.Add(new object[] { "True", "0.287", "0.279", "T3", "1", "A", "6", "T6", "9", "A9", "0.287", "0.287", "T1", "A", "0.287", "-0.542118103", "-0.554395797", "-1.248273063", "-1.276543497", "0.535723809", "0.528204506", "D", "r", "e", "e", "0.287", "0.0972438406744072", "0.279", "0.287", "0.565364708975684", "0.556484628991923", "", "hippocampus", "6h", });
            dt.Rows.Add(new object[] { "True", "0.239", "0.928", "T3", "2", "A", "1", "T1", "11", "A11", "0.239", "0.239", "T1", "A", "0.239", "-0.621602099", "-0.032452024", "-1.431291727", "-0.074723546", "0.488876262", "0.963327566", "", "", "", "", "", "", "0.928", "", "0.510801119116815", "1.29913920311732", "9.927444", "pre-frontal", "24h", });
            dt.Rows.Add(new object[] { "True", "0.883", "0.545", "T3", "2", "A", "2", "T2", "11", "A11", "0.883", "0.883", "T1", "A", "0.883", "-0.054039296", "-0.263603498", "-0.124430078", "-0.606969484", "0.939680797", "0.738241153", "", "", "", "", "", "", "0.545", "", "1.22169590721062", "0.830459135904746", "4.899305", "motor cortex", "24h", });
            dt.Rows.Add(new object[] { "True", "0.096", "0.388", "T3", "2", "A", "3", "T3", "11", "A11", "0.096", "0.096", "T1", "A", "0.096", "-1.017728767", "-0.411168274", "-2.343407088", "-0.946749939", "0.309838668", "0.62289646", "", "", "", "", "", "", "0.388", "", "0.31502334522292", "0.672439742609708", "1.518804", "striatum", "24h", });
            dt.Rows.Add(new object[] { "True", "0.002", "0.597", "T3", "2", "A", "4", "T4", "11", "A11", "0.002", "0.002", "T1", "A", "0.002", "-2.698970004", "-0.224025669", "-6.214608098", "-0.515838166", "0.04472136", "0.772657751", "", "", "", "", "", "", "0.597", "", "0.0447362801022473", "0.883017153806375", "1.960053", "hippocampus", "24h", });
            dt.Rows.Add(new object[] { "True", "0.96", "0.425", "T3", "2", "A", "5", "T5", "11", "A11", "0.96", "0.96", "T1", "A", "0.96", "-0.017728767", "-0.37161107", "-0.040821995", "-0.85566611", "0.979795897", "0.651920241", "", "", "", "", "", "", "0.425", "", "1.36943840600457", "0.710114027009105", "1.04352", "raphe", "24h", });
            dt.Rows.Add(new object[] { "True", "0.227", "0.536", "T3", "2", "A", "6", "T6", "11", "A11", "0.227", "0.227", "T1", "A", "0.227", "-0.643974143", "-0.27083521", "-1.482805262", "-0.623621118", "0.47644517", "0.732120209", "", "", "", "", "", "", "0.536", "", "0.496607026132979", "0.82142934018155", "14.80529", "pre-frontal", "24h", });
            dt.Rows.Add(new object[] { "True", "0.156", "0.052", "T3", "1", "A", "1", "T1", "19", "A19", "0.156", "0.156", "T1", "A", "0.156", "-0.806875402", "-1.283996656", "-1.857899272", "-2.95651156", "0.394968353", "0.228035085", "", "", "", "", "", "", "0.052", "", "0.406033403192609", "0.230059118691331", "9.217011", "hippocampus", "48h", });
            dt.Rows.Add(new object[] { "True", "0.783", "0.108", "T3", "1", "A", "2", "T2", "19", "A19", "0.783", "0.783", "T1", "A", "0.783", "-0.106238238", "-0.966576245", "-0.244622583", "-2.225624052", "0.884872872", "0.328633535", "", "", "", "", "", "", "0.108", "", "1.08622103584702", "0.334856384476762", "4.119224", "raphe", "48h", });
            dt.Rows.Add(new object[] { "True", "0.069", "0.72", "T3", "1", "A", "3", "T3", "19", "A19", "0.069", "0.069", "T1", "A", "0.069", "-1.161150909", "-0.142667504", "-2.673648774", "-0.328504067", "0.262678511", "0.848528137", "", "", "", "", "", "", "0.72", "", "0.265797152478042", "1.01319750009536", "", "pre-frontal", "48h", });
            dt.Rows.Add(new object[] { "True", "0.867", "0.977", "T3", "1", "A", "4", "T4", "19", "A19", "0.867", "0.867", "T1", "A", "0.867", "-0.061980903", "-0.010105436", "-0.142716302", "-0.023268627", "0.931128348", "0.988433103", "", "", "", "", "", "", "0.977", "", "1.19749470193976", "1.41855136343076", "", "motor cortex", "48h", });
            dt.Rows.Add(new object[] { "True", "0.856", "0.982", "T3", "1", "A", "5", "T5", "19", "A19", "0.856", "0.856", "T1", "A", "0.856", "-0.067526235", "-0.007888512", "-0.155484903", "-0.018163971", "0.92520268", "0.990959131", "", "", "", "", "", "", "0.982", "", "1.18156935671709", "1.43622646035762", "", "striatum", "48h", });
            dt.Rows.Add(new object[] { "True", "0.351", "0.505", "T3", "1", "A", "6", "T6", "19", "A19", "0.351", "0.351", "T1", "A", "0.351", "-0.454692884", "-0.296708622", "-1.046969056", "-0.68319685", "0.59245253", "0.71063352", "", "", "", "", "", "", "0.505", "", "0.634099776632575", "0.790398246734532", "", "hippocampus", "48h", });
            dt.Rows.Add(new object[] { "True", "0.388", "0.085", "T3", "1", "B", "1", "T1", "10", "A10", "0.388", "0.388", "T1", "B", "0.388", "-0.411168274", "-1.070581074", "-0.946749939", "-2.465104022", "0.62289646", "0.291547595", "E", "r", "e", "e", "0.388", "0.243086263352788", "0.085", "0.388", "0.672439742609708", "0.295844321213272", "", "raphe", "6h", });
            dt.Rows.Add(new object[] { "True", "0.945", "0.041", "T3", "1", "B", "2", "T2", "10", "A10", "0.945", "0.945", "T1", "B", "0.945", "-0.024568191", "-1.387216143", "-0.056570351", "-3.194183212", "0.972111105", "0.202484567", "A", "r", "e", "e", "0.945", "0.56151938772758", "0.041", "0.945", "1.33407074815886", "0.203894381034779", "", "pre-frontal", "6h", });
            dt.Rows.Add(new object[] { "True", "0.748", "0.197", "T3", "1", "B", "3", "T3", "10", "A10", "0.748", "0.748", "T1", "B", "0.748", "-0.126098402", "-0.705533774", "-0.290352301", "-1.62455155", "0.864869932", "0.44384682", "B", "r", "e", "e", "0.748", "0.893233872564592", "0.197", "0.748", "1.04489121297506", "0.459886966772016", "", "motor cortex", "6h", });
            dt.Rows.Add(new object[] { "True", "0.465", "0.805", "T3", "1", "B", "4", "T4", "10", "A10", "0.465", "0.465", "T1", "B", "0.465", "-0.332547047", "-0.09420412", "-0.765717873", "-0.216913002", "0.681909085", "0.897217922", "C", "r", "e", "e", "0.465", "0.661189456854452", "0.805", "0.465", "0.750369516853423", "1.11342845885991", "", "striatum", "6h", });
            dt.Rows.Add(new object[] { "True", "0.056", "0.607", "T3", "1", "B", "5", "T5", "10", "A10", "0.056", "0.056", "T1", "B", "0.056", "-1.251811973", "-0.216811309", "-2.882403588", "-0.499226488", "0.236643191", "0.779102047", "D", "r", "e", "e", "0.056", "0.752982767575993", "0.607", "0.056", "0.238909448626102", "0.893232162945372", "", "hippocampus", "6h", });
            dt.Rows.Add(new object[] { "True", "0.563", "0.945", "T3", "1", "B", "6", "T6", "10", "A10", "0.563", "0.563", "T1", "B", "0.563", "-0.249491605", "-0.024568191", "-0.574475651", "-0.056570351", "0.750333259", "0.972111105", "E", "r", "e", "e", "1.056", "0.255087363829712", "0.945", "1.056", "0.848566063698566", "1.33407074815886", "", "raphe", "6h", });
            dt.Rows.Add(new object[] { "True", "0.38", "0.811", "T3", "2", "B", "1", "T1", "12", "A12", "0.38", "0.38", "T1", "B", "0.38", "-0.420216403", "-0.090979146", "-0.967584026", "-0.209487225", "0.6164414", "0.900555384", "", "", "", "", "", "", "0.811", "", "0.664215237877967", "1.12104533447375", "3.614753", "motor cortex", "24h", });
            dt.Rows.Add(new object[] { "True", "0.887", "0.588", "T3", "2", "B", "2", "T2", "12", "A12", "0.887", "0.887", "T1", "B", "0.887", "-0.05207638", "-0.230622674", "-0.119910297", "-0.531028331", "0.941806774", "0.766811581", "", "", "", "", "", "", "0.588", "", "1.22796528897292", "0.8738589301306", "1.099048", "striatum", "24h", });
            dt.Rows.Add(new object[] { "True", "0.281", "0.249", "T3", "2", "B", "3", "T3", "12", "A12", "0.281", "0.281", "T1", "B", "0.281", "-0.55129368", "-0.603800653", "-1.26940061", "-1.390302383", "0.530094331", "0.498998998", "", "", "", "", "", "", "0.249", "", "0.558711809175266", "0.522443303201956", "5.644866", "hippocampus", "24h", });
            dt.Rows.Add(new object[] { "True", "0.051", "0.462", "T3", "2", "B", "4", "T4", "12", "A12", "0.051", "0.051", "T1", "B", "0.051", "-1.292429824", "-0.335358024", "-2.975929646", "-0.772190388", "0.225831796", "0.679705819", "", "", "", "", "", "", "0.462", "", "0.227796805805005", "0.747361486653721", "2.50735", "raphe", "24h", });
            dt.Rows.Add(new object[] { "True", "0.69", "0.45", "T3", "2", "B", "5", "T5", "12", "A12", "0.69", "0.69", "T1", "B", "0.69", "-0.161150909", "-0.346787486", "-0.371063681", "-0.798507696", "0.830662386", "0.670820393", "", "", "", "", "", "", "0.45", "", "0.980296311634579", "0.735314452816668", "5.60999", "pre-frontal", "24h", });
            dt.Rows.Add(new object[] { "True", "0.319", "0.664", "T3", "2", "B", "6", "T6", "12", "A12", "0.319", "0.319", "T1", "B", "0.319", "-0.496209317", "-0.177831921", "-1.142564176", "-0.40947313", "0.56480085", "0.814861951", "", "", "", "", "", "", "0.664", "", "0.600191906005382", "0.952490998766588", "3.280448", "motor cortex", "24h", });
            dt.Rows.Add(new object[] { "True", "0.843", "0.19", "T3", "1", "B", "1", "T1", "20", "A20", "0.843", "0.843", "T1", "B", "0.843", "-0.074172425", "-0.721246399", "-0.170788321", "-1.660731207", "0.918150314", "0.435889894", "", "", "", "", "", "", "0.19", "", "1.16338675769492", "0.451026811796262", "", "raphe", "48h", });
            dt.Rows.Add(new object[] { "True", "0.775", "0.809", "T3", "1", "B", "2", "T2", "20", "A20", "0.775", "0.775", "T1", "B", "0.775", "-0.110698297", "-0.092051478", "-0.25489225", "-0.211956362", "0.880340843", "0.899444273", "", "", "", "", "", "", "0.809", "", "1.07658028233182", "1.11849626283219", "", "pre-frontal", "48h", });
            dt.Rows.Add(new object[] { "True", "0.922", "0.842", "T3", "1", "B", "3", "T3", "20", "A20", "0.922", "0.922", "T1", "B", "0.922", "-0.035269079", "-0.074687909", "-0.081210055", "-0.171975265", "0.960208311", "0.91760558", "", "", "", "", "", "", "0.842", "", "1.28774713582023", "1.16201415366842", "", "motor cortex", "48h", });
            dt.Rows.Add(new object[] { "True", "0.194", "0.021", "T3", "1", "B", "4", "T4", "20", "A20", "0.194", "0.194", "T1", "B", "0.194", "-0.71219827", "-1.677780705", "-1.63989712", "-3.863232841", "0.440454311", "0.144913767", "", "", "", "", "", "", "0.021", "", "0.456104651547548", "0.145425819453369", "", "striatum", "48h", });
            dt.Rows.Add(new object[] { "True", "0.042", "0.925", "T3", "1", "B", "5", "T5", "20", "A20", "0.042", "0.042", "T1", "B", "0.042", "-1.37675071", "-0.033858267", "-3.170085661", "-0.077961541", "0.204939015", "0.961769203", "", "", "", "", "", "", "0.925", "", "0.206401399688167", "1.29339081030486", "", "hippocampus", "48h", });
            dt.Rows.Add(new object[] { "True", "0.046", "0.339", "T3", "1", "B", "6", "T6", "20", "A20", "0.046", "0.046", "T1", "B", "0.046", "-1.337242168", "-0.469800302", "-3.079113882", "-1.081755172", "0.214476106", "0.582237065", "", "", "", "", "", "", "0.339", "", "0.216155422241313", "0.621477541576457", "", "raphe", "48h", });
            dt.Rows.Add(new object[] { "True", "0.14", "0.778", "T3", "2", "B", "1", "T1", "21", "A21", "0.14", "0.14", "T1", "B", "0.14", "-0.853871964", "-0.109020403", "-1.966112856", "-0.251028755", "0.374165739", "0.882043083", "", "", "", "", "", "", "0.778", "", "0.383497003930933", "1.08018095750251", "", "", "", });
            dt.Rows.Add(new object[] { "True", "0.672", "0.114", "T3", "2", "B", "2", "T2", "21", "A21", "0.672", "0.672", "T1", "B", "0.672", "-0.172630727", "-0.943095149", "-0.397496938", "-2.171556831", "0.819756061", "0.33763886", "", "", "", "", "", "", "0.114", "", "0.960984953346383", "0.344407318980734", "", "", "", });
            dt.Rows.Add(new object[] { "True", "0.51", "0.031", "T3", "2", "B", "3", "T3", "21", "A21", "0.51", "0.51", "T1", "B", "0.51", "-0.292429824", "-1.508638306", "-0.673344553", "-3.473768074", "0.714142843", "0.176068169", "", "", "", "", "", "", "0.031", "", "0.795398830184143", "0.176990783486236", "", "", "", });
            dt.Rows.Add(new object[] { "True", "0.759", "0.025", "T3", "2", "B", "4", "T4", "21", "A21", "0.759", "0.759", "T1", "B", "0.759", "-0.119758224", "-1.602059991", "-0.275753502", "-3.688879454", "0.871206061", "0.158113883", "", "", "", "", "", "", "0.025", "", "1.05765373863082", "0.158780214645761", "", "", "", });
            dt.Rows.Add(new object[] { "True", "0.411", "0.793", "T3", "2", "B", "5", "T5", "21", "A21", "0.411", "0.411", "T1", "B", "0.411", "-0.386158178", "-0.100726813", "-0.889162064", "-0.231932057", "0.641092817", "0.890505474", "", "", "", "", "", "", "0.793", "", "0.695921354027359", "1.09845496432752", "", "", "", });
            dt.Rows.Add(new object[] { "True", "0.348", "0.441", "T3", "2", "B", "6", "T6", "21", "A21", "0.348", "0.348", "T1", "B", "0.348", "-0.458420756", "-0.355561411", "-1.055552799", "-0.818710404", "0.589915248", "0.664078309", "", "", "", "", "", "", "0.441", "", "0.630953876374661", "0.726260378976446", "", "", "", });

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