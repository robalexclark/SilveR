using Moq;
using SilveR.Models;
using SilveR.StatsModels;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Xunit;

namespace SilveR.UnitTests.StatsModels
{
    
    public class SummaryStatisticsModelTests
    {
        [Fact]
        public void ScriptFileName_ReturnsCorrectString()
        {
            //Arrange
            SummaryStatisticsModel sut = new SummaryStatisticsModel();

            //Act
            string result = sut.ScriptFileName;

            //Assert
            Assert.Equal("SummaryStatistics", result);
        }

        [Fact]
        public void TransformationsList_ReturnsCorrectList()
        {
            //Arrange
            SummaryStatisticsModel sut = new SummaryStatisticsModel();

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

            SummaryStatisticsModel sut = GetModel(mockDataset.Object);

            //Act
            string[] result = sut.ExportData();

            //Assert
            Assert.Equal("Respivs_sp_ivs2,Resp8,Cat4,Cat5,Cat6,Cat456,catfact", result[0]);
            Assert.Equal(34, result.Count());
            Assert.StartsWith("0.939350659,0.90541248", result[33]);

            //catfactor check
            Assert.EndsWith("A 2 2 A_ 2_ 2", result[33]);
        }

        [Fact]
        public void GetArguments_ReturnsCorrectArguments()
        {
            //Arrange
            SummaryStatisticsModel sut = GetModel(GetDataset());

            //Act
            List<Argument> result = sut.GetArguments().ToList();

            //Assert
            var responses = result.Single(x => x.Name == "Responses");
            Assert.Equal("Resp 2,Resp8", responses.Value);

            var firstCatFactor = result.Single(x => x.Name == "FirstCatFactor");
            Assert.Equal("Cat4", firstCatFactor.Value);

            var secondCatFactor = result.Single(x => x.Name == "SecondCatFactor");
            Assert.Equal("Cat5", secondCatFactor.Value);

            var thirdCatFactor = result.Single(x => x.Name == "ThirdCatFactor");
            Assert.Equal("Cat6", thirdCatFactor.Value);

            var fourthCatFactor = result.Single(x => x.Name == "FourthCatFactor");
            Assert.Equal("Cat456", fourthCatFactor.Value);

            var mean = result.Single(x => x.Name == "Mean");
            Assert.Equal("True", mean.Value);

            var n = result.Single(x => x.Name == "N");
            Assert.Equal("True", n.Value);

            var medianAndQuartiles = result.Single(x => x.Name == "MedianAndQuartiles");
            Assert.Equal("False", medianAndQuartiles.Value);

            var minAndMax = result.Single(x => x.Name == "MinAndMax");
            Assert.Equal("False", minAndMax.Value);

            var normalProbabilityPlot = result.Single(x => x.Name == "NormalProbabilityPlot");
            Assert.Equal("False", normalProbabilityPlot.Value);

            var significance = result.Single(x => x.Name == "Significance");
            Assert.Equal("95", significance.Value);

            var standardDeviation = result.Single(x => x.Name == "StandardDeviation");
            Assert.Equal("True", standardDeviation.Value);

            var standardErrorOfMean = result.Single(x => x.Name == "StandardErrorOfMean");
            Assert.Equal("False", standardErrorOfMean.Value);

            var transformation = result.Single(x => x.Name == "Transformation");
            Assert.Equal("None", transformation.Value);

            var variance = result.Single(x => x.Name == "Variance");
            Assert.Equal("False", variance.Value);

            var coefficientOfVariation = result.Single(x => x.Name == "CoefficientOfVariation");
            Assert.Equal("False", coefficientOfVariation.Value);

            var confidenceInterval = result.Single(x => x.Name == "ConfidenceInterval");
            Assert.Equal("True", confidenceInterval.Value);

            var byCategoriesAndOverall = result.Single(x => x.Name == "ByCategoriesAndOverall");
            Assert.Equal("False", byCategoriesAndOverall.Value);
        }

        [Fact]
        public void LoadArguments_ReturnsCorrectArguments()
        {
            //Arrange
            SummaryStatisticsModel sut = new SummaryStatisticsModel(GetDataset());

            List<Argument> arguments = new List<Argument>();
            arguments.Add(new Argument { Name = "Responses", Value = "Resp1,Resp 2" });
            arguments.Add(new Argument { Name = "FirstCatFactor", Value = "Cat1" });
            arguments.Add(new Argument { Name = "SecondCatFactor", Value = "Cat2" });
            arguments.Add(new Argument { Name = "ThirdCatFactor", Value = "Cat3" });
            arguments.Add(new Argument { Name = "FourthCatFactor", Value = "Cat4" });
            arguments.Add(new Argument { Name = "Transformation", Value = "Log10" });
            arguments.Add(new Argument { Name = "ByCategoriesAndOverall", Value = "True" });
            arguments.Add(new Argument { Name = "CoefficientOfVariation", Value = "False" });
            arguments.Add(new Argument { Name = "ConfidenceInterval", Value = "True" });
            arguments.Add(new Argument { Name = "Mean", Value = "False" });
            arguments.Add(new Argument { Name = "MedianAndQuartiles", Value = "True" });
            arguments.Add(new Argument { Name = "MinAndMax", Value = "False" });
            arguments.Add(new Argument { Name = "N", Value = "True" });
            arguments.Add(new Argument { Name = "NormalProbabilityPlot", Value = "False" });
            arguments.Add(new Argument { Name = "Significance", Value = "90" });
            arguments.Add(new Argument { Name = "StandardDeviation", Value = "True" });
            arguments.Add(new Argument { Name = "StandardErrorOfMean", Value = "False" });
            arguments.Add(new Argument { Name = "Variance", Value = "True" });

            Assert.Equal(18, arguments.Count);

            //Act
            sut.LoadArguments(arguments);

            //Assert
            Assert.Equal(new List<string>() { "Resp1", "Resp 2" }, sut.Responses);
            Assert.Equal("Cat1", sut.FirstCatFactor);
            Assert.Equal("Cat2", sut.SecondCatFactor);
            Assert.Equal("Cat3", sut.ThirdCatFactor);
            Assert.Equal("Cat4", sut.FourthCatFactor);
            Assert.Equal("Log10", sut.Transformation);
            Assert.True(sut.ByCategoriesAndOverall);
            Assert.False(sut.CoefficientOfVariation);
            Assert.True(sut.ConfidenceInterval);
            Assert.False(sut.Mean);
            Assert.True(sut.MedianAndQuartiles);
            Assert.False(sut.MinAndMax);
            Assert.True(sut.N);
            Assert.False(sut.NormalProbabilityPlot);
            Assert.Equal(90, sut.Significance);
            Assert.True(sut.StandardDeviation);
            Assert.False(sut.StandardErrorOfMean);
            Assert.True(sut.Variance);
        }

        [Fact]
        public void GetCommandLineArguments_ReturnsCorrectString()
        {
            //Arrange
            SummaryStatisticsModel sut = GetModel(GetDataset());

            //Act
            string result = sut.GetCommandLineArguments();

            //Assert
            Assert.Equal("Respivs_sp_ivs2,Resp8 None Cat4 Cat5 Cat6 Cat456 Y Y Y N N N N N Y 95 N N", result);
        }

        private SummaryStatisticsModel GetModel(IDataset dataset)
        {
            var model = new SummaryStatisticsModel(dataset)
            {
                ByCategoriesAndOverall = false,
                CoefficientOfVariation = false,
                ConfidenceInterval = true,
                FirstCatFactor = "Cat4",
                FourthCatFactor = "Cat456",
                Mean = true,
                MedianAndQuartiles = false,
                MinAndMax = false,
                N = true,
                NormalProbabilityPlot = false,
                Responses = new System.Collections.Generic.List<string>
                {
                    "Resp 2",
                    "Resp8"
                },
                SecondCatFactor = "Cat5",
                Significance = 95,
                StandardDeviation = true,
                StandardErrorOfMean = false,
                ThirdCatFactor = "Cat6",
                Transformation = "None",
                Variance = false
            };

            return model;
        }

        private DataTable GetTestDataTable()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("SilveRSelected");
            dt.Columns.Add("Resp1");
            dt.Columns.Add("Resp 2");
            dt.Columns.Add("Resp3");
            dt.Columns.Add("Resp4");
            dt.Columns.Add("Resp5");
            dt.Columns.Add("Resp6");
            dt.Columns.Add("Resp7");
            dt.Columns.Add("Resp8");
            dt.Columns.Add("Resp9");
            dt.Columns.Add("Resp10");
            dt.Columns.Add("Resp11");
            dt.Columns.Add("Cat1");
            dt.Columns.Add("Cat2");
            dt.Columns.Add("Cat3");
            dt.Columns.Add("Cat4");
            dt.Columns.Add("Cat5");
            dt.Columns.Add("Cat6");
            dt.Columns.Add("Cat456");

            dt.Rows.Add(new object[] { "True", "0.454828601", "0.574150302", "0.545565209", "0.839947044", "0.430172879", "0.214525417", "", "0.290222444", "0.485945979", "0.485945979", "0.485945979", "B", "B", "B", "B", "1", "1", "B_ 1_ 1", });
            dt.Rows.Add(new object[] { "True", "0.083018553", "0.941443463", "0.207248092", "0.331249013", "0.939168473", "0.065447886", "", "0.686787854", "0.154836911", "0.154836911", "0.154836911", "B", "B", "B", "B", "1", "2", "B_ 1_ 2", });
            dt.Rows.Add(new object[] { "True", "0.536537397", "0.127579182", "0.594837833", "0.407886296", "0.784904525", "0.975016714", "", "0.864625851", "0.701805006", "0.701805006", "0.701805006", "B", "B", "B", "B", "1", "1", "B_ 1_ 1", });
            dt.Rows.Add(new object[] { "True", "0.545106442", "0.166476045", "0.113682087", "0.062638874", "0.758670961", "0.85047697", "", "0.638286435", "0.822402515", "0.822402515", "0.822402515", "B", "B", "B", "B", "1", "2", "B_ 1_ 2", });
            dt.Rows.Add(new object[] { "True", "0.701692321", "0.172561402", "0.68310622", "0.387706974", "0.118839154", "0.745646953", "", "0.026431414", "0.79424067", "0.79424067", "0.79424067", "B", "B", "B", "B", "2", "1", "B_ 2_ 1", });
            dt.Rows.Add(new object[] { "True", "0.531609662", "0.936855243", "0.997515555", "0.881514639", "0.330662508", "0.21005685", "", "0.448949747", "0.346793925", "0.346793925", "0.346793925", "B", "B", "B", "B", "2", "2", "B_ 2_ 2", });
            dt.Rows.Add(new object[] { "True", "0.149821174", "0.066668069", "0.81158844", "0.566594821", "0.647819088", "0.179648163", "", "0.074464567", "0.527835471", "0.527835471", "0.527835471", "B", "B", "B", "B", "2", "1", "B_ 2_ 1", });
            dt.Rows.Add(new object[] { "True", "0.605073208", "0.958433038", "0.821399839", "0.73906219", "0.442586814", "0.535938328", "", "0.08348995", "0.166124277", "0.166124277", "0.166124277", "B", "B", "B", "B", "2", "2", "B_ 2_ 2", });
            dt.Rows.Add(new object[] { "True", "0.635299468", "0.982195991", "0.179141844", "0.541093986", "0.856424619", "", "0.323740334", "0.71785102", "0.226198732", "0.226198732", "0.226198732", "C", "C", "C", "C", "1", "1", "C_ 1_ 1", });
            dt.Rows.Add(new object[] { "True", "0.487401155", "0.358152281", "0.563379317", "0.295360848", "0.157512625", "", "0.016099421", "0.709070114", "0.988917407", "0.988917407", "0.988917407", "C", "C", "C", "C", "1", "2", "C_ 1_ 2", });
            dt.Rows.Add(new object[] { "True", "0.978068231", "0.624878384", "0.159597596", "0.950215856", "0.142817129", "", "0.025944891", "0.369178959", "0.917880649", "0.917880649", "0.917880649", "C", "C", "C", "C", "1", "1", "C_ 1_ 1", });
            dt.Rows.Add(new object[] { "True", "0.819419988", "0.141932344", "0.61931814", "0.281248905", "0.818355472", "", "0.32448983", "0.717178627", "0.622627038", "", "0.622627038", "C", "C", "C", "C", "1", "2", "C_ 1_ 2", });
            dt.Rows.Add(new object[] { "True", "0.735025713", "0.128882289", "0.534777557", "0.356820109", "0.280416604", "", "0.251004293", "0.031471282", "0.290788106", "0.290788106", "0.290788106", "C", "C", "C", "C", "2", "1", "C_ 2_ 1", });
            dt.Rows.Add(new object[] { "True", "0.898854243", "0.964288811", "0.261713626", "0.541621812", "0.182799503", "", "0.981675968", "0.041314833", "0.964301475", "0.964301475", "0.964301475", "C", "C", "C", "C", "2", "2", "C_ 2_ 2", });
            dt.Rows.Add(new object[] { "True", "0.858847519", "0.491738119", "0.500848271", "0.479146728", "0.485246539", "", "0.436082919", "0.616113024", "0.057336785", "0.057336785", "0.057336785", "C", "C", "C", "C", "2", "1", "C_ 2_ 1", });
            dt.Rows.Add(new object[] { "True", "0.952810877", "0.571391222", "0.042195119", "0.911342359", "0.817591397", "", "0.78375163", "0.315895101", "0.617118525", "0.617118525", "0.617118525", "C", "C", "C", "C", "2", "2", "C_ 2_ 2", });
            dt.Rows.Add(new object[] { "True", "0.766952056", "0.844519143", "0.609590604", "0.50463171", "0.776438239", "", "0.481068366", "0.932364343", "0.961541041", "0.961541041", "0.961541041", "D", "D", "D", "D", "1", "1", "D_ 1_ 1", });
            dt.Rows.Add(new object[] { "True", "0.447464942", "0.233833079", "0.000647733", "0.199353495", "0.611274034", "", "0.115290677", "0.505890092", "0.557916389", "0.557916389", "0.557916389", "D", "D", "D", "D", "1", "2", "D_ 1_ 2", });
            dt.Rows.Add(new object[] { "True", "0.764527284", "0.685993814", "0.572712655", "0.062000809", "0.533618734", "", "0.378261273", "0.508186339", "0.109362134", "0.109362134", "0.109362134", "D", "D", "D", "D", "1", "1", "D_ 1_ 1", });
            dt.Rows.Add(new object[] { "True", "0.633782937", "0.35657302", "0.822313787", "0.121274995", "0.880911409", "", "0.477463286", "0.256842242", "0.658988877", "0.658988877", "0.658988877", "D", "D", "D", "D", "1", "2", "D_ 1_ 2", });
            dt.Rows.Add(new object[] { "True", "0.593553625", "0.114952216", "0.155024388", "0.253050345", "0.642277174", "", "0.030268418", "0.141086569", "0.822525418", "0.822525418", "0.822525418", "D", "D", "D", "D", "2", "1", "D_ 2_ 1", });
            dt.Rows.Add(new object[] { "True", "0.501480074", "0.157043182", "0.705838334", "0.418769619", "0.015451653", "", "0.999472451", "0.233456664", "0.989224437", "0.989224437", "0.989224437", "D", "D", "D", "D", "2", "2", "D_ 2_ 2", });
            dt.Rows.Add(new object[] { "True", "0.946746539", "0.532413112", "0.249604431", "0.947756798", "0.962011142", "", "0.373871107", "0.384518673", "0.56921789", "0.56921789", "0.56921789", "D", "D", "D", "D", "2", "1", "D_ 2_ 1", });
            dt.Rows.Add(new object[] { "True", "0.506633522", "0.859151138", "0.576443943", "0.130836253", "0.122751625", "", "0.237874203", "0.500226243", "0.933255571", "0.933255571", "0.933255571", "D", "E", "D", "D", "2", "2", "D_ 2_ 2", });
            dt.Rows.Add(new object[] { "True", "", "", "", "", "", "", "", "", "", "", "", "", "", "E", "", "", "", "", });
            dt.Rows.Add(new object[] { "True", "Numeric", "0.040498464", "-10", "0", "", "0.661642287", "", "0.226863512", "0.714287296", "", "1.3", "", "A", "A", "A", "1", "1", "A_ 1_ 1", });
            dt.Rows.Add(new object[] { "True", "0.247133994", "0.934653155", "0.467294095", "0.154164263", "0.755035388", "0.665151722", "", "0.340305213", "0.348826381", "", "0.348826381", "A", "A", "A", "A", "1", "2", "A_ 1_ 2", });
            dt.Rows.Add(new object[] { "True", "0.209082087", "0.43995895", "0.258552399", "0.413871649", "0.200229496", "0.666388902", "", "0.035215601", "0.06374193", "0.06374193", "0.06374193", "A", "A", "A", "A", "1", "1", "A_ 1_ 1", });
            dt.Rows.Add(new object[] { "True", "0.606501566", "0.048176019", "0.45443321", "0.844787461", "0.665203168", "0.816730965", "", "0.22081664", "0.851545392", "0.851545392", "0.851545392", "A", "A", "A", "A", "1", "2", "A_ 1_ 2", });
            dt.Rows.Add(new object[] { "True", "0.663206901", "0.348008652", "0.19988248", "0.868985717", "0.171644563", "0.326137105", "", "0.238620115", "0.699475856", "0.699475856", "0.699475856", "A", "A", "A", "A", "2", "1", "A_ 2_ 1", });
            dt.Rows.Add(new object[] { "True", "0.566303488", "0.004295257", "0.2571607", "0.520051132", "0.93257691", "0.203838981", "", "0.763706352", "0.571405707", "0.571405707", "0.571405707", "A", "A", "A", "A", "2", "2", "A_ 2_ 2", });
            dt.Rows.Add(new object[] { "True", "0.87788591", "0.395044092", "0.786559259", "0.633495523", "0.049161508", "0.113960207", "", "0.986776571", "0.344027501", "0.344027501", "0.344027501", "A", "A", "A", "A", "2", "1", "A_ 2_ 1", });
            dt.Rows.Add(new object[] { "True", "0.928850779", "0.939350659", "0.009809005", "0.770861279", "0.026496166", "0.414520232", "", "0.90541248", "0.267292424", "0.267292424", "0.267292424", "A", "A", "A", "A", "2", "2", "A_ 2_ 2", });

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