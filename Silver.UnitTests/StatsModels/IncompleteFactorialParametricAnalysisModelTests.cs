using Moq;
using SilveR.Models;
using SilveR.StatsModels;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Xunit;

namespace Silver.UnitTests.StatsModels
{

    public class IncompleteFactorialParametricAnalysisModelTests
    {
        [Fact]
        public void ScriptFileName_ReturnsCorrectString()
        {
            //Arrange
            IncompleteFactorialParametricAnalysisModel sut = new IncompleteFactorialParametricAnalysisModel();

            //Act
            string result = sut.ScriptFileName;

            //Assert
            Assert.Equal("IncompleteFactorialParametricAnalysis", result);
        }

        [Fact]
        public void TransformationsList_ReturnsCorrectList()
        {
            //Arrange
            IncompleteFactorialParametricAnalysisModel sut = new IncompleteFactorialParametricAnalysisModel();

            //Act
            IEnumerable<string> result = sut.TransformationsList;

            //Assert
            Assert.IsAssignableFrom<IEnumerable<string>>(result);
            Assert.Equal(new List<string>() { "None", "Log10", "Loge", "Square Root", "ArcSine", "Rank" }, result);
        }

        [Fact]
        public void SignificancesList_ReturnsCorrectList()
        {
            //Arrange
            IncompleteFactorialParametricAnalysisModel sut = new IncompleteFactorialParametricAnalysisModel();

            //Act
            IEnumerable<string> result = sut.SignificancesList;

            //Assert
            Assert.IsAssignableFrom<IEnumerable<string>>(result);
            Assert.Equal(new List<string>() { "0.1", "0.05", "0.01", "0.001" }, result);
        }

        [Fact]
        public void PairwiseTestList_ReturnsCorrectList()
        {
            //Arrange
            IncompleteFactorialParametricAnalysisModel sut = new IncompleteFactorialParametricAnalysisModel();

            //Act
            IEnumerable<string> result = sut.PairwiseTestList;

            //Assert
            Assert.IsAssignableFrom<IEnumerable<string>>(result);
            Assert.Equal(new List<string>() { String.Empty, "Unadjusted (LSD)", "Holm", "Hochberg", "Hommel", "Bonferroni", "Benjamini-Hochberg" }, result);
        }

        [Fact]
        public void ExportData_ReturnsCorrectStringArray()
        {
            //Arrange
            Mock<IDataset> mockDataset = new Mock<IDataset>();
            mockDataset.Setup(x => x.DatasetID).Returns(1);
            mockDataset.Setup(x => x.DatasetToDataTable()).Returns(GetTestDataTable());

            IncompleteFactorialParametricAnalysisModel sut = GetModel(mockDataset.Object);
            sut.Treatments = new List<string>() { "Treat1" };
            sut.OtherDesignFactors = null;
            sut.Covariates = null;
            sut.SelectedEffect = "Treat1";

            //Act
            string[] result = sut.ExportData();

            //Assert
            Assert.Equal("Respivs_sp_ivs1,Treat1,catfact,scatterPlotColumn,mainEffect", result[0]);
            Assert.Equal(79, result.Count()); //as blank reponses are removed
            Assert.StartsWith("1.641107979,D1", result[32]);

            //scatterplot check
            Assert.Contains(",D1,", result[24]);

            //mainEffect check
            Assert.EndsWith(",D1", result[24]);
        }

        [Fact]
        public void ExportData_MultipleTreatments_ReturnsCorrectStringArray()
        {
            //Arrange
            Mock<IDataset> mockDataset = new Mock<IDataset>();
            mockDataset.Setup(x => x.DatasetID).Returns(1);
            mockDataset.Setup(x => x.DatasetToDataTable()).Returns(GetTestDataTable());

            IncompleteFactorialParametricAnalysisModel sut = GetModel(mockDataset.Object);

            //Act
            string[] result = sut.ExportData();

            //Assert
            Assert.Equal("Respivs_sp_ivs1,Treat1,Treat2,Block3,Covivs_sp_ivs1,catfact,scatterPlotColumn,mainEffect", result[0]);

            //scatterplot check
            Assert.Contains(",D1 F,", result[24]);

            //mainEffect check
            Assert.EndsWith(",D1 F", result[24]);
        }

        [Fact]
        public void GetArguments_ReturnsCorrectArguments()
        {
            //Arrange
            IncompleteFactorialParametricAnalysisModel sut = GetModel(GetDataset());

            //Act
            List<Argument> result = sut.GetArguments().ToList();

            //Assert
            var response = result.Single(x => x.Name == "Response");
            Assert.Equal("Resp 1", response.Value);

            var treatments = result.Single(x => x.Name == "Treatments");
            Assert.Equal("Treat1,Treat2", treatments.Value);

            var otherDesignFactors = result.Single(x => x.Name == "OtherDesignFactors");
            Assert.Equal("Block3", otherDesignFactors.Value);

            var covariates = result.Single(x => x.Name == "Covariates");
            Assert.Equal("Cov 1", covariates.Value);

            var responseTransformation = result.Single(x => x.Name == "ResponseTransformation");
            Assert.Equal("None", responseTransformation.Value);

            var covariateTransformation = result.Single(x => x.Name == "CovariateTransformation");
            Assert.Equal("None", covariateTransformation.Value);

            var primaryFactor = result.Single(x => x.Name == "PrimaryFactor");
            Assert.Equal("Treat1", primaryFactor.Value);

            var selectedEffect = result.Single(x => x.Name == "SelectedEffect");
            Assert.Equal("Treat1 * Treat2", selectedEffect.Value);

            var lsMeansSelected = result.Single(x => x.Name == "LSMeansSelected");
            Assert.Equal("False", lsMeansSelected.Value);

            var anovaSelected = result.Single(x => x.Name == "ANOVASelected");
            Assert.Equal("True", anovaSelected.Value);

            var significance = result.Single(x => x.Name == "Significance");
            Assert.Equal("0.05", significance.Value);

            var normalPlotSelected = result.Single(x => x.Name == "NormalPlotSelected");
            Assert.Equal("False", normalPlotSelected.Value);

            var prPlotSelected = result.Single(x => x.Name == "PRPlotSelected");
            Assert.Equal("True", prPlotSelected.Value);

            var allPairwise = result.Single(x => x.Name == "AllPairwise");
            Assert.Equal("Holm", allPairwise.Value);
        }

        [Fact]
        public void LoadArguments_ReturnsCorrectArguments()
        {
            //Arrange
            IncompleteFactorialParametricAnalysisModel sut = new IncompleteFactorialParametricAnalysisModel(GetDataset());

            List<Argument> arguments = new List<Argument>();
            arguments.Add(new Argument { Name = "Response", Value = "Resp 1" });
            arguments.Add(new Argument { Name = "Treatments", Value = "Treat1,Treat2" });
            arguments.Add(new Argument { Name = "OtherDesignFactors", Value = "Treat3,Cat4" });
            arguments.Add(new Argument { Name = "ResponseTransformation", Value = "Log10" });
            arguments.Add(new Argument { Name = "Covariates", Value = "Resp3" });
            arguments.Add(new Argument { Name = "PrimaryFactor", Value = "Treat2" });
            arguments.Add(new Argument { Name = "CovariateTransformation", Value = "ArcSine" });
            arguments.Add(new Argument { Name = "ANOVASelected", Value = "False" });
            arguments.Add(new Argument { Name = "PRPlotSelected", Value = "True" });
            arguments.Add(new Argument { Name = "NormalPlotSelected", Value = "False" });
            arguments.Add(new Argument { Name = "Significance", Value = "0.9" });
            arguments.Add(new Argument { Name = "SelectedEffect", Value = "Treat1 * Treat2" });
            arguments.Add(new Argument { Name = "LSMeansSelected", Value = "True" });
            arguments.Add(new Argument { Name = "AllPairwise", Value = "Holm" });

            Assert.Equal(14, arguments.Count);

            //Act
            sut.LoadArguments(arguments);

            //Assert
            Assert.Equal("Resp 1", sut.Response);
            Assert.Equal(new List<string> { "Treat1", "Treat2" }, sut.Treatments);
            Assert.Equal(new List<string> { "Treat3", "Cat4" }, sut.OtherDesignFactors);
            Assert.Equal("Log10", sut.ResponseTransformation);
            Assert.Equal(new List<string> { "Resp3" }, sut.Covariates);
            Assert.Equal("Treat2", sut.PrimaryFactor);
            Assert.Equal("ArcSine", sut.CovariateTransformation);
            Assert.False(sut.ANOVASelected);
            Assert.True(sut.PRPlotSelected);
            Assert.False(sut.NormalPlotSelected);
            Assert.Equal("0.9", sut.Significance);
            Assert.Equal("Treat1 * Treat2", sut.SelectedEffect);
            Assert.True(sut.LSMeansSelected);
            Assert.Equal("Holm", sut.AllPairwise);
        }


        [Fact]
        public void GetCommandLineArguments_ReturnsCorrectString()
        {
            //Arrange
            IncompleteFactorialParametricAnalysisModel sut = GetModel(GetDataset());

            //Act
            string result = sut.GetCommandLineArguments();

            //Assert
            Assert.Equal("Respivs_sp_ivs1~Covivs_sp_ivs1+Block3+Treat1+Treat2+Treat1*Treat2 Respivs_sp_ivs1~scatterPlotColumn Covivs_sp_ivs1 None None Treat1 Treat1,Treat2 Block3 Y Y N 0.05 Respivs_sp_ivs1~Covivs_sp_ivs1+Block3+mainEffect Treat1ivs_sp_ivs*ivs_sp_ivsTreat2 N Holm", result);
        }

        private IncompleteFactorialParametricAnalysisModel GetModel(IDataset dataset)
        {
            var model = new IncompleteFactorialParametricAnalysisModel(dataset)
            {
                ANOVASelected = true,
                AllPairwise = "Holm",
                CovariateTransformation = "None",
                Covariates = new System.Collections.Generic.List<string>
                {
                    "Cov 1"
                },
                LSMeansSelected = false,
                NormalPlotSelected = false,
                OtherDesignFactors = new System.Collections.Generic.List<string>
                {
                    "Block3"
                },
                PRPlotSelected = true,
                PrimaryFactor = "Treat1",
                Response = "Resp 1",
                ResponseTransformation = "None",
                SelectedEffect = "Treat1 * Treat2",
                Significance = "0.05",
                Treatments = new System.Collections.Generic.List<string>
                {
                    "Treat1",
                    "Treat2"
                }
            };

            return model;
        }

        private DataTable GetTestDataTable()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("SilveRSelected");
            dt.Columns.Add("Resp 1");
            dt.Columns.Add("Resp7");
            dt.Columns.Add("Resp8");
            dt.Columns.Add("Resp1 2");
            dt.Columns.Add("Treat1");
            dt.Columns.Add("Treat2");
            dt.Columns.Add("Treat3");
            dt.Columns.Add("Treat8");
            dt.Columns.Add("Treat9");
            dt.Columns.Add("Treat10");
            dt.Columns.Add("Treat11");
            dt.Columns.Add("Treat 15");
            dt.Columns.Add("Treat16");
            dt.Columns.Add("Blo ck1");
            dt.Columns.Add("Blo ck2");
            dt.Columns.Add("Block3");
            dt.Columns.Add("Block4");
            dt.Columns.Add("Cov 1");
            dt.Columns.Add("Cov3");
            dt.Columns.Add("Cov6");
            dt.Columns.Add("Treat1Treat2");
            dt.Columns.Add("Treat1Treat2Treat3");
            dt.Columns.Add("Treat9Treat10");
            dt.Columns.Add("Treat9Treat10Treat11");
            dt.Columns.Add("Resp 2");
            dt.Columns.Add("Resp3");
            dt.Columns.Add("Resp4");
            dt.Columns.Add("Resp5");
            dt.Columns.Add("Resp6");
            dt.Columns.Add("Resp9");
            dt.Columns.Add("Resp10");
            dt.Columns.Add("Resp11");
            dt.Columns.Add("Tre at4");
            dt.Columns.Add("T reat5");
            dt.Columns.Add("Treat6");
            dt.Columns.Add("Treat7");
            dt.Columns.Add("Treat12");
            dt.Columns.Add("Treat13");
            dt.Columns.Add("Treat14");
            dt.Columns.Add("Cov2");
            dt.Columns.Add("Cov4");
            dt.Columns.Add("Cov5");
            dt.Columns.Add("Treat 20");
            dt.Columns.Add("Treat 21");
            dt.Columns.Add("Treat 17");
            dt.Columns.Add("Treat 18");
            dt.Columns.Add("Treat 19");
            dt.Columns.Add("Resp 13");
            dt.Columns.Add("Resp14");
            dt.Columns.Add("Resp15");
            dt.Columns.Add("Treat22");
            dt.Columns.Add("Treat23");
            dt.Columns.Add("Cov7");
            dt.Columns.Add("PVTestResponse1a");
            dt.Columns.Add("PVTestResponse1b");
            dt.Columns.Add("PVTestGroup1");
            dt.Columns.Add("PVTestResponse2");
            dt.Columns.Add("PVTEstCovariate2a");
            dt.Columns.Add("PVTEstCovariate2b");
            dt.Columns.Add("PVTEstGroup2");
            dt.Columns.Add("CVResp");
            dt.Columns.Add("CVTreat1");
            dt.Columns.Add("CVTreat2");
            dt.Columns.Add("CVTreat3");
            dt.Columns.Add("CVTreat4");
            dt.Columns.Add("IFResp");
            dt.Columns.Add("IFTreat1");
            dt.Columns.Add("IFTreat2");
            dt.Columns.Add("IFTreat3");
            dt.Columns.Add("IFTreat4");
            dt.Rows.Add(new object[] { "True", "1.170460288", "0.603046662673043", "0.812263122833665", "1.170460288", "D0", "F", "TG", "C", "3", "1", "3", "A", "A", "Bk1", "Bk3", "1", "2", "0.251420288", "0.251420288", "0.653915157", "D0 F", "D0 F TG", "3 1", "3 1 3", "1.170460288", "1.170460288", "1.170460288", "1.170460288", "", "0.603046662673043", "0.356118785", "0.653915157", "D0", "D10", "D0", "D0", "D0", "F", "A", "0.251420288", "0.251420288", "0.251420288", "A", "A", "1", "q", "1", "0.0963639699688432", "0.278027714537481", "", "A", "1", "0.266515860849041", "1", "1", "1", "1", "1", "1", "1", "15", "A", "A", "1", "1", "15", "A", "A", "1", "1", });
            dt.Rows.Add(new object[] { "True", "1.301819998", "0.732601111951696", "0.447456349428076", "1.301819998", "D0", "F", "TG", "C", "3", "2", "1", "A", "B", "Bk1", "Bk1", "1", "2", "0.409977995", "0.409977995", "0.142751504", "D0 F", "D0 F TG", "3 2", "3 2 1", "1.301819998", "1.301819998", "1.301819998", "1.301819998", "", "0.732601111951696", "0.312079363", "0.142751504", "D0", "D10", "D0", "D0", "D0", "F", "A", "0.409977995", "0.409977995", "0.409977995", "B", "A", "1", "q", "1", "0.055568871198945", "0.382898816060915", "0.4545333335297", "A", "2", "0.460971770762763", "2", "2", "1", "2", "1", "2", "1", "16", "A", "A", "1", "1", "16", "A", "A", "1", "1", });
            dt.Rows.Add(new object[] { "True", "1.716827101", "0.940090281116573", "0.856354376025044", "1.716827101", "D0", "F", "TG", "C", "1", "1", "3", "A", "C", "Bk2", "Bk1", "2", "2", "0.67927756", "0.67927756", "0.56603235", "D0 F", "D0 F TG", "1 1", "1 1 3", "1.716827101", "1.716827101", "1.716827101", "1.716827101", "", "0.940090281116573", "0.54865642", "0.56603235", "D0", "D10", "D0", "D0", "D0", "F", "B", "0.67927756", "0.67927756", "0.67927756", "C", "A", "1", "q", "1", "0.460559914997265", "0.901288338592629", "0.863833239608352", "A", "3", "0.82953383870103", "3", "3", "1", "3", "3", "3", "1", "16", "A", "A", "1", "1", "16", "A", "A", "1", "1", });
            dt.Rows.Add(new object[] { "True", "1.170287058", "0.837977845863444", "0.467653941957621", "1.170287058", "D0", "F", "TG", "C", "1", "2", "1", "A", "D", "Bk2", "Bk2", "2", "2", "0.39624008", "0.39624008", "0.044637798", "D0 F", "D0 F TG", "1 2", "1 2 1", "1.170287058", "1.170287058", "1.170287058", "1.170287058", "", "0.837977845863444", "0.215032411", "0.044637798", "D0", "D10", "D0", "D0", "D0", "F", "B", "0.39624008", "0.39624008", "0.39624008", "A", "B", "1", "q", "1", "0.833652566364452", "0.571433306935469", "0.530353535439734", "A", "4", "0.610715057182273", "4", "4", "1", "4", "4", "4", "1", "14", "A", "A", "1", "1", "14", "A", "A", "1", "1", });
            dt.Rows.Add(new object[] { "True", "1.385523855", "0.0868474776039534", "0.680134733131071", "1.385523855", "D0", "F", "WT", "C", "3", "2", "2", "B", "A", "Bk1", "Bk2", "1", "2", "0.605211369", "0.605211369", "0.543254157", "D0 F", "D0 F WT", "3 2", "3 2 2", "1.385523855", "1.385523855", "1.385523855", "1.385523855", "", "0.0868474776039534", "0.173845019", "0.543254157", "D0", "D10", "D0", "D0", "D0", "F", "C", "0.605211369", "0.605211369", "0.605211369", "B", "B", "1", "q", "1", "0.656724285795664", "0.623802661315602", "0.313150319999142", "B", "1", "0.982069409206542", "11", "10", "2", "5", "5", "5", "1", "18", "A", "B", "1", "2", "18", "A", "B", "1", "2", });
            dt.Rows.Add(new object[] { "True", "1.441107979", "0.140404548426278", "0.814541080250263", "1.441107979", "D0", "F", "WT", "C", "3", "2", "3", "B", "B", "Bk1", "Bk3", "1", "2", "0.992180305", "0.992180305", "0.854148137", "D0 F", "D0 F WT", "3 2", "3 2 3", "1.441107979", "1.441107979", "1.441107979", "1.441107979", "", "0.140404548426278", "0.51040045", "0.854148137", "D0", "D10", "D0", "D0", "D0", "F", "C", "0.992180305", "0.992180305", "0.992180305", "C", "B", "1", "q", "1", "0.0220536192463925", "0.509256720353518", "0.174567990309805", "B", "2", "0.0906698179316803", "12", "11", "2", "6", "10", "11", "2", "17", "A", "B", "1", "2", "17", "A", "B", "1", "2", });
            dt.Rows.Add(new object[] { "True", "1.600527038", "0.991751224880621", "0.0406909657925691", "1.600527038", "D0", "F", "WT", "C", "1", "2", "2", "B", "C", "Bk2", "Bk3", "2", "2", "0.685151304", "0.685151304", "0.339916355", "D0 F", "D0 F WT", "1 2", "1 2 2", "1.600527038", "1.600527038", "1.600527038", "1.600527038", "", "0.991751224880621", "0.599137419", "4.7", "D0", "D10", "D0", "D0", "D0", "F", "C", "0.685151304", "0.685151304", "0.685151304", "A", "A", "2", "q", "1", "0.842352893043648", "0.568714855332386", "0.447862518907233", "B", "3", "0.0857534395418158", "13", "12", "2", "7", "11", "11", "2", "16", "A", "B", "1", "2", "16", "A", "B", "1", "2", });
            dt.Rows.Add(new object[] { "True", "1.570742573", "0.200106552575097", "0.837302048821571", "1.570742573", "D0", "F", "WT", "C", "1", "2", "3", "B", "D", "Bk2", "Bk1", "2", "2", "0.082280725", "0.082280725", "0.845987397", "D0 F", "D0 F WT", "1 2", "1 2 3", "1.570742573", "1.570742573", "1.570742573", "1.570742573", "", "0.200106552575097", "0.529847741", "", "D0", "D10", "D0", "D0", "D0", "F", "", "0.082280725", "0.082280725", "0.082280725", "B", "A", "2", "q", "1", "0.633353259254585", "0.471160037557914", "0.799330721856125", "B", "4", "0.815363668618568", "14", "13", "2", "9", "12", "12", "2", "18", "A", "B", "1", "2", "18", "A", "B", "1", "2", });
            dt.Rows.Add(new object[] { "True", "1.332792864", "0.124397094477086", "0.81927613782916", "1.332792864", "D0", "M", "TG", "C", "3", "3", "1", "C", "A", "Bk1", "Bk1", "1", "2", "0.414097984", "0.414097984", "0.817811302", "D0 M", "D0 M TG", "3 3", "3 3 1", "1.332792864", "1.332792864", "1.332792864", "1.332792864", "", "0.124397094477086", "0.800183941", "", "D0", "D10", "D0", "D0", "D0", "F", "", "0.414097984", "0.414097984", "0.414097984", "C", "A", "2", "q", "1", "0.445749520430521", "0.669680856480915", "0.539921016509775", "C", "1", "0.569032842718968", "", "", "", "10", "14", "14", "2", "17", "A", "C", "1", "3", "17", "A", "C", "1", "3", });
            dt.Rows.Add(new object[] { "True", "1.570048439", "0.915191859457472", "0.322276620180147", "1.570048439", "D0", "M", "TG", "C", "3", "3", "2", "C", "B", "Bk1", "Bk2", "1", "2", "0.067631666", "0.067631666", "0.906066356", "D0 M", "D0 M TG", "3 3", "3 3 2", "1.570048439", "1.570048439", "1.570048439", "1.570048439", "", "0.915191859457472", "0.936126132", "", "D0", "D10", "D0", "D0", "D0", "F", "", "0.067631666", "0.067631666", "0.067631666", "A", "B", "2", "q", "1", "0.960898386360586", "0.927068315067787", "0.835127944324997", "C", "2", "0.276453652553175", "", "", "", "11", "14", "14", "2", "14", "A", "C", "1", "3", "14", "A", "C", "1", "3", });
            dt.Rows.Add(new object[] { "True", "1.657046361", "0.31057873183634", "0.290013789221723", "1.657046361", "D0", "M", "TG", "C", "3", "3", "3", "C", "C", "Bk1", "Bk3", "1", "2", "0.421922675", "0.421922675", "0.969396829", "D0 M", "D0 M TG", "3 3", "3 3 3", "1.657046361", "1.657046361", "1.657046361", "1.657046361", "", "0.31057873183634", "0.689009069", "", "D0", "D10", "D0", "D0", "D0", "F", "", "0.421922675", "0.421922675", "0.421922675", "B", "B", "2", "q", "1", "0.78912845700754", "0.0158878081728098", "0.0396830829024308", "C", "3", "0.551358413198525", "", "", "", "", "", "", "", "15", "A", "C", "1", "3", "15", "A", "C", "1", "3", });
            dt.Rows.Add(new object[] { "True", "1.474137613", "0.457779840024549", "0.35474847665096", "1.474137613", "D0", "M", "TG", "C", "1", "3", "1", "C", "D", "Bk2", "Bk2", "2", "2", "0.290327192", "0.290327192", "0.718713971", "D0 M", "D0 M TG", "1 3", "1 3 1", "1.474137613", "1.474137613", "1.474137613", "1.474137613", "", "0.457779840024549", "0.653549271", "", "D0", "D10", "D0", "D0", "D0", "F", "", "0.290327192", "0.290327192", "0.290327192", "C", "B", "2", "q", "1", "0.690175209937228", "0.910114831266688", "0.463997388330815", "C", "4", "0.6553448539872", "", "", "", "", "", "", "", "0.769798133953462", "A", "C", "1", "3", "0.769798133953462", "A", "C", "1", "3", });
            dt.Rows.Add(new object[] { "True", "1.716827101", "0.0671358810975891", "0.196116189081307", "1.716827101", "D0", "M", "TG", "C", "1", "3", "2", "A", "A", "Bk2", "Bk3", "2", "2", "0.51467703", "0.51467703", "0.076617146", "D0 M", "D0 M TG", "1 3", "1 3 2", "1.716827101", "1.716827101", "1.716827101", "1.716827101", "", "0.0671358810975891", "0.701238702", "", "D0", "D10", "D0", "D0", "D0", "F", "", "0.51467703", "0.51467703", "0.51467703", "A", "A", "1", "w", "1", "0.498938084387785", "0.796378731040055", "0.383186751654319", "A", "1", "0.475746521294949", "", "", "", "", "", "", "", "0.197990831502153", "B", "A", "2", "1", "0.197990831502153", "B", "A", "2", "1", });
            dt.Rows.Add(new object[] { "True", "1.170287058", "0.552815259422122", "0.268419967518355", "1.170287058", "D0", "M", "TG", "C", "1", "3", "3", "A", "B", "Bk2", "Bk1", "2", "2", "0.85443754", "0.85443754", "0.713506941", "D0 M", "D0 M TG", "1 3", "1 3 3", "1.170287058", "1.170287058", "1.170287058", "1.170287058", "", "0.552815259422122", "0.668313021", "", "D0", "D10", "D0", "D0", "D0", "F", "", "0.85443754", "0.85443754", "0.85443754", "B", "A", "1", "w", "1", "0.653181551542021", "0.423402022771651", "0.1657508367955", "A", "2", "0.0377149362295359", "", "", "", "", "", "", "", "0.554259094477103", "B", "A", "2", "1", "0.554259094477103", "B", "A", "2", "1", });
            dt.Rows.Add(new object[] { "True", "1.063642468", "0.40505236469072", "0.43675094654099", "1.063642468", "D0", "M", "WT", "C", "1", "1", "1", "A", "C", "Bk1", "Bk1", "2", "2", "0.913914896", "0.913914896", "0.329648291", "D0 M", "D0 M WT", "1 1", "1 1 1", "1.063642468", "1.063642468", "1.063642468", "1.063642468", "", "0.40505236469072", "0.65758256", "", "D0", "D10", "D0", "D0", "D0", "F", "", "0.913914896", "0.913914896", "0.913914896", "C", "A", "1", "w", "1", "0.828401322932965", "0.758984970689022", "0.570932178887405", "A", "3", "0.488316263753079", "", "", "", "", "", "", "", "0.0992525187210886", "B", "A", "2", "1", "0.0992525187210886", "B", "A", "2", "1", });
            dt.Rows.Add(new object[] { "True", "1.244007576", "0.230600627561462", "0.542372979062827", "1.244007576", "D0", "M", "WT", "C", "1", "1", "2", "A", "D", "Bk1", "Bk2", "2", "2", "0.769521225", "0.769521225", "0.109290024", "D0 M", "D0 M WT", "1 1", "1 1 2", "1.244007576", "1.244007576", "1.244007576", "1.244007576", "", "0.230600627561462", "0.687190506", "", "D0", "D10", "D0", "D0", "D0", "F", "", "0.769521225", "0.769521225", "0.769521225", "A", "B", "1", "w", "1", "0.655656631561048", "0.207091476886771", "0.405726381451844", "A", "4", "0.818069299239549", "", "", "", "", "", "", "", "0.212098806370527", "B", "A", "2", "1", "0.212098806370527", "B", "A", "2", "1", });
            dt.Rows.Add(new object[] { "True", "1.600527038", "0.409989784660779", "0.363827997768341", "1.600527038", "D0", "M", "WT", "C", "2", "1", "1", "B", "A", "Bk2", "Bk2", "2", "2", "0.642408987", "0.642408987", "0.411345052", "D0 M", "D0 M WT", "2 1", "2 1 1", "1.600527038", "1.600527038", "1.600527038", "1.600527038", "", "0.409989784660779", "0.56299766", "", "D0", "D10", "D0", "D0", "D0", "F", "", "0.642408987", "0.642408987", "0.642408987", "B", "B", "1", "w", "1", "0.361329036696993", "0.533840704023495", "0.953779792403675", "B", "1", "0.00533938209867824", "", "", "", "", "", "", "", "0.271795352264463", "B", "B", "2", "2", "0.271795352264463", "B", "B", "2", "2", });
            dt.Rows.Add(new object[] { "True", "0.987369833", "0.721002153505759", "0.366093082878754", "0.987369833", "D0", "M", "WT", "C", "2", "1", "2", "B", "B", "Bk2", "Bk3", "2", "2", "0.309986757", "0.309986757", "0.432802217", "D0 M", "D0 M WT", "2 1", "2 1 2", "0.987369833", "0.987369833", "0.987369833", "0.987369833", "", "0.721002153505759", "0.031078452", "", "D0", "D10", "D0", "D0", "D0", "F", "", "0.309986757", "0.309986757", "0.309986757", "C", "B", "1", "w", "1", "0.517823156415715", "0.412124455647062", "", "B", "2", "0.314792053499413", "", "", "", "", "", "", "", "0.646576484947105", "B", "B", "2", "2", "0.646576484947105", "B", "B", "2", "2", });
            dt.Rows.Add(new object[] { "True", "1.370460288", "0.173476580058291", "0.611493952241674", "1.370460288", "D1", "F", "TG", "C", "2", "1", "3", "B", "C", "Bk1", "Bk1", "2", "2", "0.591264809", "0.591264809", "0.817789771", "D1 F", "D1 F TG", "2 1", "2 1 3", "1.370460288", "1.370460288", "1.370460288", "1.370460288", "", "0.173476580058291", "0.761898632", "", "D1", "D10", "D0", "D1", "D1", "F", "", "0.591264809", "0.591264809", "0.591264809", "A", "A", "2", "w", "1", "0.421485135586719", "0.223131585308148", "0.604421955892524", "B", "3", "0.297345070720787", "", "", "", "", "", "", "", "0.711940041021504", "B", "B", "2", "2", "0.711940041021504", "B", "B", "2", "2", });
            dt.Rows.Add(new object[] { "True", "1.501819998", "0.924819752477296", "0.693431965484925", "1.501819998", "D1", "F", "TG", "C", "2", "2", "1", "B", "D", "Bk1", "Bk2", "2", "1", "0.023013687", "0.023013687", "0.204990012", "D1 F", "D1 F TG", "2 2", "2 2 1", "1.501819998", "1.501819998", "1.501819998", "1.501819998", "", "0.924819752477296", "0.724670977", "", "D1", "D10", "D0", "D1", "D1", "F", "", "0.023013687", "0.023013687", "0.023013687", "B", "A", "2", "w", "1", "0.223104382188778", "0.446342939965881", "0.393322765968075", "B", "4", "0.423044796969845", "", "", "", "", "", "", "", "0.399996653032867", "B", "B", "2", "2", "0.399996653032867", "B", "B", "2", "2", });
            dt.Rows.Add(new object[] { "True", "1.585523855", "0.866116079524865", "0.406660027450881", "1.585523855", "D1", "F", "TG", "C", "2", "2", "2", "C", "A", "Bk1", "Bk3", "2", "1", "0.228751128", "0.228751128", "0.656070241", "D1 F", "D1 F TG", "2 2", "2 2 2", "1.585523855", "1.585523855", "1.585523855", "1.585523855", "", "0.866116079524865", "0.418230806", "", "D1", "D10", "D0", "D1", "D1", "F", "", "0.228751128", "0.228751128", "0.228751128", "C", "A", "2", "w", "1", "0.79754182737605", "0.048309956327143", "0.26307052257456", "C", "1", "0.0745085346038588", "", "", "", "", "", "", "", "0.206104672676506", "B", "C", "2", "3", "0.206104672676506", "B", "C", "2", "3", });
            dt.Rows.Add(new object[] { "True", "1.370460288", "0.861827198686888", "0.445787308470098", "1.370460288", "D1", "F", "TG", "C", "3", "2", "1", "C", "B", "Bk2", "Bk2", "2", "1", "0.833876926", "0.833876926", "0.907028247", "D1 F", "D1 F TG", "3 2", "3 2 1", "1.370460288", "1.370460288", "1.370460288", "1.370460288", "", "0.861827198686888", "0.587689207", "", "D1", "D10", "D0", "D1", "D1", "F", "", "0.833876926", "0.833876926", "0.833876926", "A", "B", "2", "w", "1", "0.504961249533863", "0.713563195886542", "0.442979672293395", "C", "2", "0.450267041974058", "", "", "", "", "", "", "", "0.853307895519926", "B", "C", "2", "3", "0.853307895519926", "B", "C", "2", "3", });
            dt.Rows.Add(new object[] { "True", "1.501819998", "0.216245303930937", "0.288483484835275", "1.501819998", "D1", "F", "TG", "C", "3", "2", "2", "C", "C", "Bk2", "Bk3", "2", "1", "0.716614275", "0.716614275", "0.36104316", "D1 F", "D1 F TG", "3 2", "3 2 2", "1.501819998", "1.501819998", "1.501819998", "1.501819998", "", "0.216245303930937", "0.135744804", "", "D1", "D10", "D0", "D1", "D1", "F", "", "0.716614275", "0.716614275", "0.716614275", "B", "B", "2", "w", "1", "0.485003815403439", "0.452921774306074", "0.715071534046262", "C", "3", "0.193689739098456", "", "", "", "", "", "", "", "0.712220632343034", "B", "C", "2", "3", "0.712220632343034", "B", "C", "2", "3", });
            dt.Rows.Add(new object[] { "True", "1.370460288", "0.956456905733308", "0.407631567238776", "1.370460288", "D1", "F", "TG", "C", "3", "2", "3", "C", "D", "Bk2", "Bk1", "2", "1", "0.080120144", "0.080120144", "0.345314335", "D1 F", "D1 F TG", "3 2", "3 2 3", "1.370460288", "1.370460288", "1.370460288", "1.370460288", "", "0.956456905733308", "0.01620344", "", "D1", "D10", "D0", "D1", "D1", "F", "", "0.080120144", "0.080120144", "0.080120144", "C", "B", "2", "w", "1", "0.982463942051801", "0.979450197245469", "", "C", "4", "0.267900675105304", "", "", "", "", "", "", "", "0.223704319346075", "B", "C", "2", "3", "0.223704319346075", "B", "C", "2", "3", });
            dt.Rows.Add(new object[] { "True", "1.641107979", "0.337344013697215", "0.0194565951037273", "1.641107979", "D1", "F", "WT", "C", "2", "2", "3", "A", "A", "Bk1", "Bk1", "2", "1", "0.776155131", "0.776155131", "0.468548418", "D1 F", "D1 F WT", "2 2", "2 2 3", "1.641107979", "1.641107979", "1.641107979", "1.641107979", "", "0.337344013697215", "0.477167012", "", "D1", "D10", "D0", "D1", "D1", "F", "", "0.776155131", "0.776155131", "0.776155131", "A", "A", "1", "q", "2", "0.496971408277598", "", "", "", "", "", "", "", "", "", "", "", "", "0.136412249865606", "C", "A", "3", "1", "0.136412249865606", "C", "A", "3", "1", });
            dt.Rows.Add(new object[] { "True", "1.532792864", "0.266885520346394", "0.294547513706064", "1.532792864", "D1", "F", "WT", "C", "2", "3", "1", "A", "B", "Bk1", "Bk2", "2", "1", "0.830872442", "0.830872442", "0.895445179", "D1 F", "D1 F WT", "2 3", "2 3 1", "1.532792864", "1.532792864", "1.532792864", "1.532792864", "", "0.266885520346394", "0.825342034", "", "D1", "D10", "D0", "D1", "D1", "F", "", "0.830872442", "0.830872442", "0.830872442", "B", "A", "1", "q", "2", "0.98256748110733", "", "", "", "", "", "", "", "", "", "", "", "", "0.747410595516353", "C", "A", "3", "1", "0.747410595516353", "C", "A", "3", "1", });
            dt.Rows.Add(new object[] { "True", "1.501819998", "0.574598206821078", "0.560948852023722", "1.501819998", "D1", "F", "WT", "C", "3", "3", "1", "A", "C", "Bk2", "Bk2", "2", "1", "0.071264197", "0.071264197", "0.011729616", "D1 F", "D1 F WT", "3 3", "3 3 1", "1.501819998", "1.501819998", "1.501819998", "1.501819998", "", "0.574598206821078", "0.543407026", "", "D1", "D10", "D0", "D1", "D1", "F", "", "0.071264197", "0.071264197", "0.071264197", "C", "A", "1", "q", "2", "0.0632516959529705", "", "", "", "", "", "", "", "", "", "", "", "", "0.448268661351918", "C", "A", "3", "1", "0.448268661351918", "C", "A", "3", "1", });
            dt.Rows.Add(new object[] { "True", "1.585523855", "0.340316189802516", "0.290002724973979", "1.585523855", "D1", "F", "WT", "C", "3", "3", "2", "A", "D", "Bk2", "Bk3", "2", "1", "0.097003892", "0.097003892", "0.00962974", "D1 F", "D1 F WT", "3 3", "3 3 2", "1.585523855", "1.585523855", "1.585523855", "1.585523855", "", "0.340316189802516", "0.407880898", "", "D1", "D10", "D0", "D1", "D1", "F", "", "0.097003892", "0.097003892", "0.097003892", "A", "B", "1", "q", "2", "0.427636140373732", "", "", "", "", "", "", "", "", "", "", "", "", "0.547564770867569", "C", "A", "3", "1", "0.547564770867569", "C", "A", "3", "1", });
            dt.Rows.Add(new object[] { "True", "1.770048439", "0.29402232193559", "0.962459107249869", "1.770048439", "D1", "M", "TG", "C", "2", "3", "2", "B", "A", "Bk1", "Bk3", "2", "1", "0.209579177", "0.209579177", "0.790354007", "D1 M", "D1 M TG", "2 3", "2 3 2", "1.770048439", "1.770048439", "1.770048439", "1.770048439", "", "0.29402232193559", "0.378431833", "", "D1", "D10", "D0", "D1", "D1", "M", "", "0.209579177", "0.209579177", "0.209579177", "B", "B", "1", "q", "2", "0.819525552605012", "", "", "", "", "", "", "", "", "", "", "", "", "0.104531567668928", "C", "B", "3", "2", "0.104531567668928", "C", "B", "3", "2", });
            dt.Rows.Add(new object[] { "True", "1.857046361", "0.804550729426912", "0.522219300967214", "1.857046361", "D1", "M", "TG", "C", "2", "3", "3", "B", "B", "Bk1", "Bk1", "2", "1", "0.488427414", "0.488427414", "0.717729496", "D1 M", "D1 M TG", "2 3", "2 3 3", "1.857046361", "1.857046361", "1.857046361", "1.857046361", "", "0.804550729426912", "0.278027329", "", "D1", "D10", "D0", "D1", "D1", "M", "", "0.488427414", "0.488427414", "0.488427414", "C", "B", "1", "q", "2", "0.635388275555056", "", "", "", "", "", "", "", "", "", "", "", "", "0.593481508327229", "C", "B", "3", "2", "0.593481508327229", "C", "B", "3", "2", });
            dt.Rows.Add(new object[] { "True", "1.263642468", "0.704040701705731", "0.927081799004612", "1.263642468", "D1", "M", "TG", "C", "3", "1", "1", "B", "C", "Bk1", "Bk2", "2", "1", "0.056963977", "0.056963977", "0.446229701", "D1 M", "D1 M TG", "3 1", "3 1 1", "1.263642468", "1.263642468", "1.263642468", "1.263642468", "", "0.704040701705731", "0.032708981", "", "D1", "D10", "D0", "D1", "D1", "M", "", "0.056963977", "0.056963977", "0.056963977", "A", "A", "2", "q", "2", "0.393385828593356", "", "", "", "", "", "", "", "", "", "", "", "", "0.481507831695153", "C", "B", "3", "2", "0.481507831695153", "C", "B", "3", "2", });
            dt.Rows.Add(new object[] { "True", "1.641107979", "0.456627050505169", "0.328088969451556", "1.641107979", "D1", "M", "TG", "C", "3", "3", "3", "B", "D", "Bk2", "Bk1", "2", "1", "0.886551225", "0.886551225", "0.767800036", "D1 M", "D1 M TG", "3 3", "3 3 3", "1.641107979", "1.641107979", "1.641107979", "1.641107979", "", "0.456627050505169", "0.785007278", "", "D1", "D10", "D0", "D1", "D1", "M", "", "0.886551225", "0.886551225", "0.886551225", "B", "A", "2", "q", "2", "0.015537340891965", "", "", "", "", "", "", "", "", "", "", "", "", "0.766123142013142", "C", "B", "3", "2", "0.766123142013142", "C", "B", "3", "2", });
            dt.Rows.Add(new object[] { "True", "1.532792864", "0.74559001126744", "0.373095983734427", "1.532792864", "D1", "M", "TG", "C", "1", "1", "1", "C", "A", "Bk2", "Bk2", "3", "1", "0.030910748", "0.030910748", "0.211080602", "D1 M", "D1 M TG", "1 1", "1 1 1", "1.532792864", "1.532792864", "1.532792864", "1.532792864", "", "0.74559001126744", "0.775481087", "", "D1", "D10", "D0", "D1", "D1", "M", "", "0.030910748", "0.030910748", "0.030910748", "C", "A", "2", "q", "2", "0.60548821765474", "", "", "", "", "", "", "", "", "", "", "", "", "0.952716370328942", "C", "C", "3", "3", "", "", "", "", "", });
            dt.Rows.Add(new object[] { "True", "1.770048439", "0.650827657531079", "0.386983255803045", "1.770048439", "D1", "M", "TG", "C", "1", "1", "2", "C", "B", "Bk2", "Bk3", "3", "1", "0.785780199", "0.785780199", "0.559971518", "D1 M", "D1 M TG", "1 1", "1 1 2", "1.770048439", "1.770048439", "1.770048439", "1.770048439", "", "0.650827657531079", "0.233922069", "", "D1", "D10", "D0", "D1", "D1", "M", "", "0.785780199", "0.785780199", "0.785780199", "A", "B", "2", "q", "2", "0.613073260194348", "", "", "", "", "", "", "", "", "", "", "", "", "0.126699076303079", "C", "C", "3", "3", "", "", "", "", "", });
            dt.Rows.Add(new object[] { "True", "1.444007576", "0.595058451982713", "0.0816750242951656", "1.444007576", "D1", "M", "WT", "C", "3", "1", "2", "C", "C", "Bk1", "Bk3", "2", "1", "0.654543393", "0.654543393", "0.028179208", "D1 M", "D1 M WT", "3 1", "3 1 2", "1.444007576", "1.444007576", "1.444007576", "1.444007576", "", "0.595058451982713", "0.633216256", "", "D1", "D10", "D0", "D1", "D1", "M", "", "0.654543393", "0.654543393", "0.654543393", "B", "B", "2", "q", "2", "0.689424757207339", "", "", "", "", "", "", "", "", "", "", "", "", "0.135410489298746", "C", "C", "3", "3", "", "", "", "", "", });
            dt.Rows.Add(new object[] { "True", "1.916827101", "0.280383922227745", "0.341740602821349", "1.916827101", "D1", "M", "WT", "C", "3", "1", "3", "C", "D", "Bk1", "Bk1", "2", "1", "0.481989874", "0.481989874", "0.731735705", "D1 M", "D1 M WT", "3 1", "3 1 3", "1.916827101", "1.916827101", "1.916827101", "1.916827101", "", "0.280383922227745", "0.959323939", "", "D1", "D10", "D0", "D1", "D1", "M", "", "0.481989874", "0.481989874", "0.481989874", "C", "B", "2", "q", "2", "0.623508292038176", "", "", "", "", "", "", "", "", "", "", "", "", "0.618268239315514", "C", "C", "3", "3", "", "", "", "", "", });
            dt.Rows.Add(new object[] { "True", "1.857046361", "0.98975161272649", "0.773882241701029", "1.857046361", "D1", "M", "WT", "C", "1", "1", "3", "A", "A", "Bk2", "Bk1", "3", "1", "0.35865958", "0.35865958", "0.456733225", "D1 M", "D1 M WT", "1 1", "1 1 3", "1.857046361", "1.857046361", "1.857046361", "1.857046361", "", "0.98975161272649", "0.691604257", "", "D1", "D10", "D0", "D1", "D1", "M", "", "0.35865958", "0.35865958", "0.35865958", "A", "A", "1", "w", "2", "0.143874375965833", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", });
            dt.Rows.Add(new object[] { "True", "1.263642468", "0.561915586858659", "0.5088227458051", "1.263642468", "D1", "M", "WT", "C", "1", "2", "1", "A", "B", "Bk2", "Bk2", "3", "1", "0.607058994", "0.607058994", "0.432122644", "D1 M", "D1 M WT", "1 2", "1 2 1", "1.263642468", "1.263642468", "1.263642468", "1.263642468", "", "0.561915586858659", "0.788333245", "", "D1", "D10", "D0", "D1", "D1", "M", "", "0.607058994", "0.607058994", "0.607058994", "B", "A", "1", "w", "2", "0.454927479569744", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", });
            dt.Rows.Add(new object[] { "True", "1.258312377", "0.564773978955788", "1.2", "1.258312377", "D10", "F", "TG", "A", "1", "1", "1", "A", "C", "Bk1", "Bk1", "1", "1", "0.003473612", "0", "0.01089691", "D10 F", "D10 F TG", "1 1", "1 1 1", "-1", "0", "missing", "", "321", "-1", "0.229878028", "", "", "D10", "D10", "D10", "D10", "F", "", "-1", "missing", "", "C", "A", "1", "w", "2", "0.996677676716297", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", });
            dt.Rows.Add(new object[] { "True", "1.770742573", "0.763285638585231", "0.856574324795722", "1.770742573", "D10", "F", "TG", "A", "1", "1", "2", "A", "D", "Bk1", "Bk2", "1", "1", "0.325671649", "0.325671649", "0.852331827", "D10 F", "D10 F TG", "1 1", "1 1 2", "1.770742573", "1.770742573", "1.770742573", "1.770742573", "", "0.763285638585231", "0.250039813", "", "D10", "D10", "D0", "D10", "D10", "F", "", "0.325671649", "0.325671649", "0.325671649", "A", "B", "1", "w", "2", "0.343827776778713", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", });
            dt.Rows.Add(new object[] { "True", "1.674137613", "0.439583545552745", "0.627770818786173", "1.674137613", "D10", "F", "TG", "A", "1", "1", "3", "B", "A", "Bk1", "Bk3", "1", "1", "0.632595085", "0.632595085", "0.990447908", "D10 F", "D10 F TG", "1 1", "1 1 3", "1.674137613", "1.674137613", "1.674137613", "1.674137613", "", "0.439583545552745", "0.962201316", "", "D10", "D10", "D0", "D10", "D10", "F", "", "0.632595085", "0.632595085", "0.632595085", "B", "B", "1", "w", "2", "0.485465048299329", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", });
            dt.Rows.Add(new object[] { "True", "1.641107979", "0.344765025846945", "0.0612330939849803", "1.641107979", "D10", "F", "TG", "A", "2", "1", "2", "B", "B", "Bk2", "Bk2", "1", "1", "0.157125673", "0.157125673", "0.403187671", "D10 F", "D10 F TG", "2 1", "2 1 2", "1.641107979", "1.641107979", "1.641107979", "1.641107979", "", "0.344765025846945", "0.986171659", "", "D10", "D10", "D0", "D10", "D10", "F", "", "0.157125673", "0.157125673", "0.157125673", "C", "B", "1", "w", "2", "0.0543468468739636", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", });
            dt.Rows.Add(new object[] { "True", "1.532792864", "0.148645842849191", "0.581543026491877", "1.532792864", "D10", "F", "TG", "A", "2", "1", "3", "B", "C", "Bk2", "Bk3", "1", "1", "0.34255673", "0.34255673", "0.459745166", "D10 F", "D10 F TG", "2 1", "2 1 3", "1.532792864", "1.532792864", "1.532792864", "1.532792864", "", "0.148645842849191", "0.917447146", "", "D10", "D10", "D0", "D10", "D10", "F", "", "0.34255673", "0.34255673", "0.34255673", "A", "A", "2", "w", "2", "0.96339699498589", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", });
            dt.Rows.Add(new object[] { "True", "1.770048439", "0.0621496219072206", "0.545390176726762", "1.770048439", "D10", "F", "TG", "B", "2", "2", "1", "B", "D", "Bk2", "Bk1", "1", "1", "0.460969017", "0.460969017", "0.254896714", "D10 F", "D10 F TG", "2 2", "2 2 1", "1.770048439", "1.770048439", "1.770048439", "1.770048439", "", "0.0621496219072206", "0.368602339", "", "D10", "D10", "D0", "D10", "D10", "F", "", "0.460969017", "0.460969017", "0.460969017", "B", "A", "2", "w", "2", "0.876369253335818", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", });
            dt.Rows.Add(new object[] { "True", "1.916827101", "0.361327873509002", "0.548538299005267", "1.916827101", "D10", "F", "WT", "A", "1", "2", "1", "C", "A", "Bk1", "Bk1", "1", "1", "0.927654114", "0.927654114", "0.852649548", "D10 F", "D10 F WT", "1 2", "1 2 1", "1.916827101", "1.916827101", "1.916827101", "1.916827101", "", "0.361327873509002", "0.400228217", "", "D10", "D10", "D0", "D10", "D10", "F", "", "0.927654114", "0.927654114", "0.927654114", "C", "A", "2", "w", "2", "0.92177823531874", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", });
            dt.Rows.Add(new object[] { "True", "1.370287058", "0.812372731910561", "0.405961530722458", "1.370287058", "D10", "F", "WT", "A", "1", "2", "2", "C", "B", "Bk1", "Bk2", "1", "1", "0.702380161", "0.702380161", "0.860030648", "D10 F", "D10 F WT", "1 2", "1 2 2", "1.370287058", "1.370287058", "1.370287058", "1.370287058", "", "0.812372731910561", "0.082542379", "", "D10", "D10", "D0", "D10", "D10", "F", "", "0.702380161", "0.702380161", "0.702380161", "A", "B", "2", "w", "2", "0.464973322454951", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", });
            dt.Rows.Add(new object[] { "True", "1.857046361", "0.582763750813711", "0.488252633533302", "1.857046361", "D10", "F", "WT", "B", "2", "2", "2", "C", "C", "Bk2", "Bk2", "1", "1", "0.388044205", "0.388044205", "0.948339757", "D10 F", "D10 F WT", "2 2", "2 2 2", "1.857046361", "1.857046361", "1.857046361", "1.857046361", "", "0.582763750813711", "0.551953672", "", "D10", "D10", "D0", "D10", "D10", "F", "", "0.388044205", "0.388044205", "0.388044205", "B", "B", "2", "w", "2", "0.475212371159667", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", });
            dt.Rows.Add(new object[] { "True", "1.263642468", "0.534123741454761", "0.340440930562808", "1.263642468", "D10", "F", "WT", "B", "2", "2", "3", "C", "D", "Bk2", "Bk3", "1", "1", "0.193467751", "0.193467751", "0.80932296", "D10 F", "D10 F WT", "2 2", "2 2 3", "1.263642468", "1.263642468", "1.263642468", "1.263642468", "", "0.534123741454761", "0.856083698", "", "D10", "D10", "D0", "D10", "D10", "F", "", "0.193467751", "0.193467751", "0.193467751", "C", "B", "2", "w", "2", "0.562416947293646", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", });
            dt.Rows.Add(new object[] { "True", "1.800527038", "0.523927093616605", "0.159520225255629", "1.800527038", "D10", "M", "TG", "A", "1", "2", "3", "A", "A", "Bk1", "Bk2", "1", "1", "0.286888105", "0.286888105", "0.948638079", "D10 M", "D10 M TG", "1 2", "1 2 3", "1.800527038", "1.800527038", "1.800527038", "1.800527038", "", "0.523927093616605", "0.572318227", "", "D10", "D10", "D0", "D10", "D10", "M", "", "0.286888105", "0.286888105", "0.286888105", "A", "A", "1", "q", "1", "0.905956223365035", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", });
            dt.Rows.Add(new object[] { "True", "1.187369833", "0.699313408674976", "0.859459235533828", "1.187369833", "D10", "M", "TG", "A", "1", "3", "1", "A", "B", "Bk1", "Bk3", "1", "1", "0.739410707", "0.739410707", "0.560362423", "D10 M", "D10 M TG", "1 3", "1 3 1", "1.187369833", "1.187369833", "1.187369833", "1.187369833", "", "0.699313408674976", "0.785902559", "", "D10", "D10", "D0", "D10", "D10", "M", "", "0.739410707", "0.739410707", "0.739410707", "B", "A", "1", "q", "1", "0.507491096403013", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", });
            dt.Rows.Add(new object[] { "True", "1.370460288", "0.168313596936464", "0.960479177722312", "1.370460288", "D10", "M", "TG", "A", "1", "3", "2", "A", "C", "Bk1", "Bk1", "1", "1", "0.963879625", "0.963879625", "0.062086242", "D10 M", "D10 M TG", "1 3", "1 3 2", "1.370460288", "1.370460288", "1.370460288", "1.370460288", "", "0.168313596936464", "0.530684325", "", "D10", "D10", "D0", "D10", "D10", "M", "", "0.963879625", "0.963879625", "0.963879625", "C", "A", "1", "q", "1", "0.428485940674252", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", });
            dt.Rows.Add(new object[] { "True", "1.444007576", "0.840591827206065", "0.681706057339523", "1.444007576", "D10", "M", "TG", "B", "2", "3", "1", "A", "D", "Bk2", "Bk3", "1", "1", "0.258104927", "0.258104927", "0.957998863", "D10 M", "D10 M TG", "2 3", "2 3 1", "1.444007576", "1.444007576", "1.444007576", "1.444007576", "", "0.840591827206065", "0.572115535", "", "D10", "D10", "D0", "D10", "D10", "M", "", "0.258104927", "0.258104927", "0.258104927", "A", "B", "1", "q", "1", "0.533083359510192", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", });
            dt.Rows.Add(new object[] { "True", "1.916827101", "0.663661990767209", "0.24651613145372", "1.916827101", "D10", "M", "TG", "B", "2", "3", "2", "B", "A", "Bk2", "Bk1", "1", "1", "0.059813834", "0.059813834", "0.262132204", "D10 M", "D10 M TG", "2 3", "2 3 2", "1.916827101", "1.916827101", "1.916827101", "1.916827101", "", "0.663661990767209", "0.718160147", "", "D10", "D10", "D0", "D10", "D10", "M", "", "0.059813834", "0.059813834", "0.059813834", "B", "B", "1", "q", "1", "0.197028249469745", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", });
            dt.Rows.Add(new object[] { "True", "1.370287058", "0.0357383247595688", "0.24625400698063", "1.370287058", "D10", "M", "TG", "B", "2", "3", "3", "B", "B", "Bk2", "Bk2", "1", "1", "0.642994776", "0.642994776", "0.769860109", "D10 M", "D10 M TG", "2 3", "2 3 3", "1.370287058", "1.370287058", "1.370287058", "1.370287058", "", "0.0357383247595688", "0.61170538", "", "D10", "D10", "D0", "D10", "D10", "M", "", "0.642994776", "0.642994776", "0.642994776", "C", "B", "1", "q", "1", "0.354801834989983", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", });
            dt.Rows.Add(new object[] { "True", "1.501819998", "0.219162513014847", "0.0773314108245682", "1.501819998", "D10", "M", "WT", "A", "1", "3", "3", "B", "C", "Bk1", "Bk2", "1", "1", "0.473124664", "0.473124664", "0.387642505", "D10 M", "D10 M WT", "1 3", "1 3 3", "1.501819998", "1.501819998", "1.501819998", "1.501819998", "", "0.219162513014847", "0.960307129", "", "D10", "D10", "D0", "D10", "D10", "M", "", "0.473124664", "0.473124664", "0.473124664", "A", "A", "2", "q", "1", "0.883857406283437", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", });
            dt.Rows.Add(new object[] { "True", "1.585523855", "0.606367318809271", "0.683112708084104", "1.585523855", "D10", "M", "WT", "A", "2", "1", "1", "B", "D", "Bk1", "Bk3", "1", "1", "0.768653477", "0.768653477", "0.443414606", "D10 M", "D10 M WT", "2 1", "2 1 1", "1.585523855", "1.585523855", "1.585523855", "1.585523855", "", "0.606367318809271", "0.248369014", "", "D10", "D10", "D0", "D10", "D10", "M", "", "0.768653477", "0.768653477", "0.768653477", "B", "A", "2", "q", "1", "0.57516733670594", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", });
            dt.Rows.Add(new object[] { "True", "1.800527038", "0.19741446372558", "0.671929556767691", "1.800527038", "D10", "M", "WT", "B", "3", "1", "1", "C", "A", "Bk2", "Bk3", "1", "1", "0.063026459", "0.063026459", "0.462131655", "D10 M", "D10 M WT", "3 1", "3 1 1", "1.800527038", "1.800527038", "1.800527038", "1.800527038", "", "0.19741446372558", "0.007458589", "", "D10", "D10", "D0", "D10", "D10", "M", "", "0.063026459", "0.063026459", "0.063026459", "C", "A", "2", "q", "1", "0.669761812498911", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", });
            dt.Rows.Add(new object[] { "True", "1.187369833", "0.0187752489219706", "0.231504015489546", "1.187369833", "D10", "M", "WT", "C", "3", "1", "2", "C", "B", "Bk2", "Bk1", "1", "1", "0.194179828", "0.194179828", "0.980159331", "D10 M", "D10 M WT", "3 1", "3 1 2", "1.187369833", "1.187369833", "1.187369833", "1.187369833", "", "0.0187752489219706", "0.806309829", "", "D10", "D10", "D0", "D10", "D10", "M", "", "0.194179828", "0.194179828", "0.194179828", "A", "B", "2", "q", "1", "0.375833102013638", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", });
            dt.Rows.Add(new object[] { "True", "2.02633744", "0.784305538893852", "0.359513551889353", "2.02633744", "D3", "F", "TG", "C", "1", "2", "2", "C", "C", "Bk1", "Bk3", "3", "1", "0.13770819", "0.13770819", "0.671938116", "D3 F", "D3 F TG", "1 2", "1 2 2", "2.02633744", "2.02633744", "2.02633744", "2.02633744", "", "0.784305538893852", "0.087818003", "", "D3", "D10", "D0", "D3", "D3", "F", "", "0.13770819", "0.13770819", "0.13770819", "B", "B", "2", "q", "1", "0.82416848533104", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", });
            dt.Rows.Add(new object[] { "True", "2.195303034", "0.876024476520656", "0.993920105336578", "2.195303034", "D3", "F", "TG", "C", "1", "2", "3", "C", "D", "Bk1", "Bk1", "3", "1", "0.643463015", "0.643463015", "0.415385744", "D3 F", "D3 F TG", "1 2", "1 2 3", "2.195303034", "2.195303034", "2.195303034", "2.195303034", "", "0.876024476520656", "0.315569093", "", "D3", "D10", "D0", "D3", "D3", "F", "", "0.643463015", "0.643463015", "0.643463015", "C", "B", "2", "q", "1", "0.138579639296542", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", });
            dt.Rows.Add(new object[] { "True", "2.282420265", "0.647710909362175", "0.500681510537306", "2.282420265", "D3", "F", "TG", "C", "1", "3", "1", "A", "A", "Bk1", "Bk2", "3", "1", "0.661600948", "0.661600948", "0.443806741", "D3 F", "D3 F TG", "1 3", "1 3 1", "2.282420265", "2.282420265", "2.282420265", "2.282420265", "", "0.647710909362175", "0.714716676", "", "D3", "D10", "D0", "D3", "D3", "F", "", "0.661600948", "0.661600948", "0.661600948", "A", "A", "1", "w", "1", "0.72047130054622", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", });
            dt.Rows.Add(new object[] { "True", "1.716885733", "0.343082315973712", "0.611414582597931", "1.716885733", "D3", "F", "TG", "C", "2", "2", "3", "A", "B", "Bk2", "Bk1", "3", "1", "0.368788258", "0.368788258", "0.538961313", "D3 F", "D3 F TG", "2 2", "2 2 3", "1.716885733", "1.716885733", "1.716885733", "1.716885733", "", "0.343082315973712", "0.286867695", "", "D3", "D10", "D0", "D3", "D3", "F", "", "0.368788258", "0.368788258", "0.368788258", "B", "A", "1", "w", "1", "0.855678044153857", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", });
            dt.Rows.Add(new object[] { "True", "1.746115363", "0.622752177825267", "0.913942694279479", "1.746115363", "D3", "F", "TG", "C", "2", "3", "1", "A", "C", "Bk2", "Bk2", "3", "1", "0.080808376", "0.080808376", "0.289179198", "D3 F", "D3 F TG", "2 3", "2 3 1", "1.746115363", "1.746115363", "1.746115363", "1.746115363", "", "0.622752177825267", "0.35037958", "", "D3", "D10", "D0", "D3", "D3", "F", "", "0.080808376", "0.080808376", "0.080808376", "C", "A", "1", "w", "1", "0.379371586963415", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", });
            dt.Rows.Add(new object[] { "True", "1.810308454", "0.260465421946502", "0.0425275187615783", "1.810308454", "D3", "F", "TG", "C", "2", "3", "2", "A", "D", "Bk2", "Bk3", "3", "1", "0.830390884", "0.830390884", "0.523184907", "D3 F", "D3 F TG", "2 3", "2 3 2", "1.810308454", "1.810308454", "1.810308454", "1.810308454", "", "0.260465421946502", "0.240231327", "", "D3", "D10", "D0", "D3", "D3", "F", "", "0.830390884", "0.830390884", "0.830390884", "A", "B", "1", "w", "1", "0.231619242823246", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", });
            dt.Rows.Add(new object[] { "True", "1.860750091", "0.380167845919718", "0.404441986881423", "1.860750091", "D3", "F", "WT", "C", "1", "3", "2", "B", "A", "Bk1", "Bk3", "3", "1", "0.794241899", "0.794241899", "0.600773768", "D3 F", "D3 F WT", "1 3", "1 3 2", "1.860750091", "1.860750091", "1.860750091", "1.860750091", "", "0.380167845919718", "0.8373271", "", "D3", "D10", "D0", "D3", "D3", "F", "", "0.794241899", "0.794241899", "0.794241899", "B", "B", "1", "w", "1", "0.493123045413753", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", });
            dt.Rows.Add(new object[] { "True", "1.637967891", "0.839741777545848", "0.236080162456037", "1.637967891", "D3", "F", "WT", "C", "1", "3", "3", "B", "B", "Bk1", "Bk1", "3", "1", "0.461015234", "0.461015234", "0.759455683", "D3 F", "D3 F WT", "1 3", "1 3 3", "1.637967891", "1.637967891", "1.637967891", "1.637967891", "", "0.839741777545848", "0.410502813", "", "D3", "D10", "D0", "D3", "D3", "F", "", "0.461015234", "0.461015234", "0.461015234", "C", "B", "1", "w", "1", "0.770800611519469", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", });
            dt.Rows.Add(new object[] { "True", "2.250653742", "0.24223593659305", "0.8740887574609", "2.250653742", "D3", "F", "WT", "C", "2", "3", "3", "B", "C", "Bk2", "Bk1", "3", "1", "0.480200439", "0.480200439", "0.725279", "D3 F", "D3 F WT", "2 3", "2 3 3", "2.250653742", "2.250653742", "2.250653742", "2.250653742", "", "0.24223593659305", "0.410295688", "", "D3", "D10", "D0", "D3", "D3", "F", "", "0.480200439", "0.480200439", "0.480200439", "A", "A", "2", "w", "1", "0.301267635208203", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", });
            dt.Rows.Add(new object[] { "True", "1.881722808", "0.210460280339522", "0.637014023498692", "1.881722808", "D3", "F", "WT", "C", "3", "1", "1", "B", "D", "Bk2", "Bk2", "3", "1", "0.259839003", "0.259839003", "0.507556163", "D3 F", "D3 F WT", "3 1", "3 1 1", "1.881722808", "1.881722808", "1.881722808", "1.881722808", "", "0.210460280339522", "0.176386949", "", "D3", "D10", "D0", "D3", "D3", "F", "", "0.259839003", "0.259839003", "0.259839003", "B", "A", "2", "w", "1", "0.233613808418154", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", });
            dt.Rows.Add(new object[] { "True", "1.693983444", "0.315324277777574", "0.754602480736339", "1.693983444", "D3", "M", "TG", "C", "2", "1", "1", "C", "A", "Bk1", "Bk2", "3", "1", "0.841943615", "0.841943615", "0.368487829", "D3 M", "D3 M TG", "2 1", "2 1 1", "1.693983444", "1.693983444", "1.693983444", "1.693983444", "", "0.315324277777574", "0.825593846", "", "D3", "D10", "D0", "D3", "D3", "M", "", "0.841943615", "0.841943615", "0.841943615", "C", "A", "2", "w", "1", "0.96393909534567", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", });
            dt.Rows.Add(new object[] { "True", "1.822731728", "0.0110590535519883", "0.16229031158923", "1.822731728", "D3", "M", "TG", "C", "2", "1", "2", "C", "B", "Bk1", "Bk3", "3", "1", "0.93956765", "0.93956765", "0.836553233", "D3 M", "D3 M TG", "2 1", "2 1 2", "1.822731728", "1.822731728", "1.822731728", "1.822731728", "", "0.0110590535519883", "0.1302948", "", "D3", "D10", "D0", "D3", "D3", "M", "", "0.93956765", "0.93956765", "0.93956765", "A", "B", "2", "w", "1", "0.440368931654281", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", });
            dt.Rows.Add(new object[] { "True", "2.259625123", "0.822291934077382", "0.0644771178280121", "2.259625123", "D3", "M", "TG", "C", "2", "1", "3", "C", "C", "Bk1", "Bk1", "3", "1", "0.705216899", "0.705216899", "0.339155553", "D3 M", "D3 M TG", "2 1", "2 1 3", "2.259625123", "2.259625123", "2.259625123", "2.259625123", "", "0.822291934077382", "0.000623952", "", "D3", "D10", "D0", "D3", "D3", "M", "", "0.705216899", "0.705216899", "0.705216899", "B", "B", "2", "w", "1", "0.308880403163354", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", });
            dt.Rows.Add(new object[] { "True", "1.611021829", "0.14174021444686", "0.326051780650482", "1.611021829", "D3", "M", "TG", "C", "3", "1", "2", "C", "D", "Bk2", "Bk3", "3", "1", "0.090175409", "0.090175409", "0.528609496", "D3 M", "D3 M TG", "3 1", "3 1 2", "1.611021829", "1.611021829", "1.611021829", "1.611021829", "", "0.14174021444686", "0.920407014", "", "D3", "D10", "D0", "D3", "D3", "M", "", "0.090175409", "0.090175409", "0.090175409", "C", "B", "2", "w", "1", "0.0277068791062829", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", });
            dt.Rows.Add(new object[] { "True", "2.056711957", "0.241973082616662", "0.365864768380597", "", "D3", "M", "TG", "C", "3", "1", "3", "", "", "Bk2", "Bk1", "3", "1", "0.824022633", "0.824022633", "0.440157959", "D3 M", "D3 M TG", "3 1", "3 1 3", "2.056711957", "2.056711957", "2.056711957", "2.056711957", "", "0.241973082616662", "0.685187744", "", "D3", "D10", "D0", "D3", "D3", "M", "", "0.824022633", "0.824022633", "0.824022633", "A", "A", "1", "q", "2", "0.481363080340923", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", });
            dt.Rows.Add(new object[] { "True", "2.179452748", "0.628971378234828", "0.369217265842098", "", "D3", "M", "TG", "C", "3", "2", "1", "", "", "Bk2", "Bk2", "3", "1", "0.937608097", "0.937608097", "0.155738784", "D3 M", "D3 M TG", "3 2", "3 2 1", "2.179452748", "2.179452748", "2.179452748", "2.179452748", "", "0.628971378234828", "0.002641728", "", "D3", "D10", "D0", "D3", "D3", "M", "", "0.937608097", "0.937608097", "0.937608097", "B", "A", "1", "q", "2", "0.574334111499995", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", });
            dt.Rows.Add(new object[] { "True", "1.989662521", "0.940411573968542", "0.441101188907144", "", "D3", "M", "WT", "C", "2", "2", "1", "", "", "Bk1", "Bk2", "3", "1", "0.146492169", "0.146492169", "0.999285379", "D3 M", "D3 M WT", "2 2", "2 2 1", "1.989662521", "1.989662521", "1.989662521", "1.989662521", "", "0.940411573968542", "0.058017306", "", "D3", "D10", "D0", "D3", "D3", "M", "", "0.146492169", "0.146492169", "0.146492169", "C", "A", "1", "q", "2", "0.0137299470555288", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", });
            dt.Rows.Add(new object[] { "True", "1.686946905", "0.993394945047956", "0.266103257059627", "", "D3", "M", "WT", "C", "2", "2", "2", "", "", "Bk1", "Bk3", "3", "1", "0.910921236", "0.910921236", "0.007237098", "D3 M", "D3 M WT", "2 2", "2 2 2", "1.686946905", "1.686946905", "1.686946905", "1.686946905", "", "0.993394945047956", "0.193034916", "", "D3", "D10", "D0", "D3", "D3", "M", "", "0.910921236", "0.910921236", "0.910921236", "A", "B", "1", "q", "2", "0.696945658902809", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", });
            dt.Rows.Add(new object[] { "True", "1.61676718", "0.339232644242126", "0.294094835244646", "", "D3", "M", "WT", "C", "3", "2", "2", "", "", "Bk2", "Bk3", "3", "1", "0.460446839", "0.460446839", "0.278652307", "D3 M", "D3 M WT", "3 2", "3 2 2", "1.61676718", "1.61676718", "1.61676718", "1.61676718", "", "0.339232644242126", "0.721929266", "", "D3", "D10", "D0", "D3", "D3", "M", "", "0.460446839", "0.460446839", "0.460446839", "B", "B", "1", "q", "2", "0.337930101482616", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", });
            dt.Rows.Add(new object[] { "True", "2.481805825", "0.815126101922305", "0.516703937668062", "", "D3", "M", "WT", "C", "3", "2", "3", "", "", "Bk2", "Bk1", "3", "1", "0.25973294", "0.25973294", "0.438075624", "D3 M", "D3 M WT", "3 2", "3 2 3", "2.481805825", "2.481805825", "2.481805825", "2.481805825", "", "0.815126101922305", "0.855638606", "", "D3", "D10", "D0", "D3", "D3", "M", "", "0.25973294", "0.25973294", "0.25973294", "C", "B", "1", "q", "2", "0.833218810157955", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", });
            dt.Rows.Add(new object[] { "True", "", "", "", "", "", "", "", "", "3", "3", "1", "", "", "", "", "3", "1", "", "", "0.740273982", "", "", "3 3", "3 3 1", "", "", "", "", "", "", "0.552034309", "", "", "", "", "30mg/kg", "", "", "", "", "", "", "A", "A", "2", "q", "2", "0.861462770062802", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", });
            dt.Rows.Add(new object[] { "True", "", "", "", "", "", "", "", "", "3", "3", "2", "", "", "", "", "3", "1", "", "", "0.125915186", "", "", "3 3", "3 3 2", "", "", "", "", "", "", "0.442105141", "", "", "", "", "", "", "", "", "", "", "", "B", "A", "2", "q", "2", "0.490509634695046", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", });
            dt.Rows.Add(new object[] { "True", "", "", "", "", "", "", "", "", "3", "3", "3", "", "", "", "", "3", "1", "", "", "0.288545388", "", "", "3 3", "3 3 3", "", "", "", "", "", "", "0.824363834", "", "", "", "", "", "", "", "", "", "", "", "C", "A", "2", "q", "2", "0.858485717161229", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", });
            dt.Rows.Add(new object[] { "True", "", "", "", "", "", "", "", "", "1", "1", "1", "", "", "", "", "1", "2", "", "", "0.240627052", "", "", "1 1", "1 1 1", "", "", "", "", "", "", "0.015581845", "", "", "", "", "", "", "", "", "", "", "", "A", "B", "2", "q", "2", "0.131932818345175", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", });
            dt.Rows.Add(new object[] { "True", "", "", "", "", "", "", "", "", "1", "1", "2", "", "", "", "", "1", "2", "", "", "0.97512017", "", "", "1 1", "1 1 2", "", "", "", "", "", "", "0.409608369", "", "", "", "", "", "", "", "", "", "", "", "B", "B", "2", "q", "2", "0.760267063388702", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", });
            dt.Rows.Add(new object[] { "True", "", "", "", "", "", "", "", "", "1", "1", "3", "", "", "", "", "1", "2", "", "", "0.27633178", "", "", "1 1", "1 1 3", "", "", "", "", "", "", "0.866517158", "", "", "", "", "", "", "", "", "", "", "", "C", "B", "2", "q", "2", "0.156660883121574", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", });
            dt.Rows.Add(new object[] { "True", "", "", "", "", "", "", "", "", "1", "2", "1", "", "", "", "", "1", "2", "", "", "0.789444996", "", "", "1 2", "1 2 1", "", "", "", "", "", "", "0.065734459", "", "", "", "", "", "", "", "", "", "", "", "A", "A", "1", "w", "2", "0.306993830626573", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", });
            dt.Rows.Add(new object[] { "True", "", "", "", "", "", "", "", "", "1", "2", "2", "", "", "", "", "1", "2", "", "", "0.787423408", "", "", "1 2", "1 2 2", "", "", "", "", "", "", "0.350633957", "", "", "", "", "", "", "", "", "", "", "", "B", "A", "1", "w", "2", "0.390206600309541", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", });
            dt.Rows.Add(new object[] { "True", "", "", "", "", "", "", "", "", "1", "2", "3", "", "", "", "", "1", "2", "", "", "0.529488555", "", "", "1 2", "1 2 3", "", "", "", "", "", "", "0.496621338", "", "", "", "", "", "", "", "", "", "", "", "C", "A", "1", "w", "2", "0.741224903871354", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", });
            dt.Rows.Add(new object[] { "True", "", "", "", "", "", "", "", "", "1", "3", "1", "", "", "", "", "1", "2", "", "", "0.425961489", "", "", "1 3", "1 3 1", "", "", "", "", "", "", "0.318421203", "", "", "", "", "", "", "", "", "", "", "", "A", "B", "1", "w", "2", "0.361812644675452", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", });
            dt.Rows.Add(new object[] { "True", "", "", "", "", "", "", "", "", "1", "3", "2", "", "", "", "", "1", "2", "", "", "0.612719495", "", "", "1 3", "1 3 2", "", "", "", "", "", "", "0.579106139", "", "", "", "", "", "", "", "", "", "", "", "B", "B", "1", "w", "2", "0.625339169893917", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", });
            dt.Rows.Add(new object[] { "True", "", "", "", "", "", "", "", "", "1", "3", "3", "", "", "", "", "1", "2", "", "", "0.537175813", "", "", "1 3", "1 3 3", "", "", "", "", "", "", "0.729490464", "", "", "", "", "", "", "", "", "", "", "", "C", "B", "1", "w", "2", "0.32728848864308", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", });
            dt.Rows.Add(new object[] { "True", "", "", "", "", "", "", "", "", "2", "1", "1", "", "", "", "", "1", "2", "", "", "0.237554254", "", "", "2 1", "2 1 1", "", "", "", "", "", "", "0.814990961", "", "", "", "", "", "", "", "", "", "", "", "A", "A", "2", "w", "2", "0.512686836875425", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", });
            dt.Rows.Add(new object[] { "True", "", "", "", "", "", "", "", "", "2", "1", "2", "", "", "", "", "1", "2", "", "", "0.326563784", "", "", "2 1", "2 1 2", "", "", "", "", "", "", "0.231658206", "", "", "", "", "", "", "", "", "", "", "", "B", "A", "2", "w", "2", "0.145245817819875", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", });
            dt.Rows.Add(new object[] { "True", "", "", "", "", "", "", "", "", "2", "1", "3", "", "", "", "", "1", "2", "", "", "0.110168174", "", "", "2 1", "2 1 3", "", "", "", "", "", "", "0.394246904", "", "", "", "", "", "", "", "", "", "", "", "C", "A", "2", "w", "2", "0.361326394985495", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", });
            dt.Rows.Add(new object[] { "True", "", "", "", "", "", "", "", "", "2", "2", "1", "", "", "", "", "1", "2", "", "", "0.906580712", "", "", "2 2", "2 2 1", "", "", "", "", "", "", "0.17296561", "", "", "", "", "", "", "", "", "", "", "", "A", "B", "2", "w", "2", "0.174068381702241", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", });
            dt.Rows.Add(new object[] { "True", "", "", "", "", "", "", "", "", "2", "2", "2", "", "", "", "", "1", "2", "", "", "0.652133574", "", "", "2 2", "2 2 2", "", "", "", "", "", "", "0.740499013", "", "", "", "", "", "", "", "", "", "", "", "B", "B", "2", "w", "2", "0.522052032659922", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", });
            dt.Rows.Add(new object[] { "True", "", "", "", "", "", "", "", "", "2", "2", "3", "", "", "", "", "1", "2", "", "", "0.878751541", "", "", "2 2", "2 2 3", "", "", "", "", "", "", "0.815158072", "", "", "", "", "", "", "", "", "", "", "", "C", "B", "2", "w", "2", "0.395874967441359", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", });
            dt.Rows.Add(new object[] { "True", "", "", "", "", "", "", "", "", "2", "3", "1", "", "", "", "", "1", "2", "", "", "0.282026609", "", "", "2 3", "2 3 1", "", "", "", "", "", "", "0.430207789", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", });
            dt.Rows.Add(new object[] { "True", "", "", "", "", "", "", "", "", "2", "3", "2", "", "", "", "", "1", "2", "", "", "0.689774468", "", "", "2 3", "2 3 2", "", "", "", "", "", "", "0.994652118", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", });
            dt.Rows.Add(new object[] { "True", "", "", "", "", "", "", "", "", "2", "3", "3", "", "", "", "", "1", "2", "", "", "0.932265049", "", "", "2 3", "2 3 3", "", "", "", "", "", "", "0.486378265", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", });
            dt.Rows.Add(new object[] { "True", "", "", "", "", "", "", "", "", "3", "1", "1", "", "", "", "", "1", "2", "", "", "0.376409944", "", "", "3 1", "3 1 1", "", "", "", "", "", "", "0.986372577", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", });
            dt.Rows.Add(new object[] { "True", "", "", "", "", "", "", "", "", "3", "1", "2", "", "", "", "", "1", "2", "", "", "0.985890813", "", "", "3 1", "3 1 2", "", "", "", "", "", "", "0.933385049", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", });
            dt.Rows.Add(new object[] { "True", "", "", "", "", "", "", "", "", "3", "1", "3", "", "", "", "", "1", "2", "", "", "0.783823326", "", "", "3 1", "3 1 3", "", "", "", "", "", "", "0.6450028", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", });
            dt.Rows.Add(new object[] { "True", "", "", "", "", "", "", "", "", "3", "2", "1", "", "", "", "", "1", "2", "", "", "0.192779789", "", "", "3 2", "3 2 1", "", "", "", "", "", "", "0.140207866", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", });
            dt.Rows.Add(new object[] { "True", "", "", "", "", "", "", "", "", "3", "2", "2", "", "", "", "", "1", "2", "", "", "0.780105109", "", "", "3 2", "3 2 2", "", "", "", "", "", "", "0.501636165", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", });
            dt.Rows.Add(new object[] { "True", "", "", "", "", "", "", "", "", "3", "2", "3", "", "", "", "", "1", "2", "", "", "0.604432169", "", "", "3 2", "3 2 3", "", "", "", "", "", "", "0.623601782", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", });
            dt.Rows.Add(new object[] { "True", "", "", "", "", "", "", "", "", "3", "3", "1", "", "", "", "", "1", "2", "", "", "0.028049981", "", "", "3 3", "3 3 1", "", "", "", "", "", "", "0.339661615", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", });
            dt.Rows.Add(new object[] { "True", "", "", "", "", "", "", "", "", "3", "3", "2", "", "", "", "", "1", "2", "", "", "0.608892883", "", "", "3 3", "3 3 2", "", "", "", "", "", "", "0.690194233", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", });
            dt.Rows.Add(new object[] { "True", "", "", "", "", "", "", "", "", "3", "3", "3", "", "", "", "", "1", "2", "", "", "0.005354799", "", "", "3 3", "3 3 3", "", "", "", "", "", "", "0.148334592", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", });
            dt.Rows.Add(new object[] { "True", "", "", "", "", "", "", "", "", "1", "1", "1", "", "", "", "", "2", "2", "", "", "0.95234975", "", "", "1 1", "1 1 1", "", "", "", "", "", "", "0.800808576", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", });
            dt.Rows.Add(new object[] { "True", "", "", "", "", "", "", "", "", "1", "1", "2", "", "", "", "", "2", "2", "", "", "0.789683552", "", "", "1 1", "1 1 2", "", "", "", "", "", "", "0.23822201", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", });
            dt.Rows.Add(new object[] { "True", "", "", "", "", "", "", "", "", "1", "1", "3", "", "", "", "", "2", "2", "", "", "0.01112396", "", "", "1 1", "1 1 3", "", "", "", "", "", "", "0.693493514", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", });
            dt.Rows.Add(new object[] { "True", "", "", "", "", "", "", "", "", "1", "2", "1", "", "", "", "", "2", "2", "", "", "0.259584099", "", "", "1 2", "1 2 1", "", "", "", "", "", "", "0.459734065", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", });
            dt.Rows.Add(new object[] { "True", "", "", "", "", "", "", "", "", "1", "2", "2", "", "", "", "", "2", "2", "", "", "0.7930584", "", "", "1 2", "1 2 2", "", "", "", "", "", "", "0.586761199", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", });
            dt.Rows.Add(new object[] { "True", "", "", "", "", "", "", "", "", "1", "2", "3", "", "", "", "", "2", "2", "", "", "0.935902073", "", "", "1 2", "1 2 3", "", "", "", "", "", "", "0.837870337", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", });
            dt.Rows.Add(new object[] { "True", "", "", "", "", "", "", "", "", "1", "3", "1", "", "", "", "", "2", "2", "", "", "0.81383252", "", "", "1 3", "1 3 1", "", "", "", "", "", "", "0.705590317", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", });
            dt.Rows.Add(new object[] { "True", "", "", "", "", "", "", "", "", "1", "3", "2", "", "", "", "", "2", "2", "", "", "0.248739588", "", "", "1 3", "1 3 2", "", "", "", "", "", "", "0.860506813", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", });
            dt.Rows.Add(new object[] { "True", "", "", "", "", "", "", "", "", "1", "3", "3", "", "", "", "", "2", "2", "", "", "0.568418881", "", "", "1 3", "1 3 3", "", "", "", "", "", "", "0.082818154", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", });
            dt.Rows.Add(new object[] { "True", "", "", "", "", "", "", "", "", "2", "1", "1", "", "", "", "", "2", "2", "", "", "0.870918547", "", "", "2 1", "2 1 1", "", "", "", "", "", "", "0.653157478", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", });
            dt.Rows.Add(new object[] { "True", "", "", "", "", "", "", "", "", "2", "1", "2", "", "", "", "", "2", "2", "", "", "0.069552365", "", "", "2 1", "2 1 2", "", "", "", "", "", "", "0.502378396", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", });
            dt.Rows.Add(new object[] { "True", "", "", "", "", "", "", "", "", "2", "1", "3", "", "", "", "", "2", "2", "", "", "0.214773896", "", "", "2 1", "2 1 3", "", "", "", "", "", "", "0.285103701", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", });
            dt.Rows.Add(new object[] { "True", "", "", "", "", "", "", "", "", "2", "2", "1", "", "", "", "", "2", "2", "", "", "0.265715602", "", "", "2 2", "2 2 1", "", "", "", "", "", "", "0.505758516", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", });
            dt.Rows.Add(new object[] { "True", "", "", "", "", "", "", "", "", "2", "2", "2", "", "", "", "", "2", "2", "", "", "0.829104076", "", "", "2 2", "2 2 2", "", "", "", "", "", "", "0.804257996", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", });
            dt.Rows.Add(new object[] { "True", "", "", "", "", "", "", "", "", "2", "2", "3", "", "", "", "", "2", "2", "", "", "0.028995851", "", "", "2 2", "2 2 3", "", "", "", "", "", "", "0.842461834", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", });
            dt.Rows.Add(new object[] { "True", "", "", "", "", "", "", "", "", "2", "3", "1", "", "", "", "", "2", "2", "", "", "0.684411851", "", "", "2 3", "2 3 1", "", "", "", "", "", "", "0.979581823", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", });
            dt.Rows.Add(new object[] { "True", "", "", "", "", "", "", "", "", "2", "3", "2", "", "", "", "", "2", "2", "", "", "0.526530519", "", "", "2 3", "2 3 2", "", "", "", "", "", "", "0.820111516", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", });
            dt.Rows.Add(new object[] { "True", "", "", "", "", "", "", "", "", "2", "3", "3", "", "", "", "", "2", "2", "", "", "0.871543262", "", "", "2 3", "2 3 3", "", "", "", "", "", "", "0.267555261", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", });
            dt.Rows.Add(new object[] { "True", "", "", "", "", "", "", "", "", "3", "1", "1", "", "", "", "", "2", "2", "", "", "0.097208183", "", "", "3 1", "3 1 1", "", "", "", "", "", "", "0.63770348", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", });
            dt.Rows.Add(new object[] { "True", "", "", "", "", "", "", "", "", "3", "1", "2", "", "", "", "", "2", "2", "", "", "0.352648766", "", "", "3 1", "3 1 2", "", "", "", "", "", "", "0.892896988", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", });
            dt.Rows.Add(new object[] { "True", "", "", "", "", "", "", "", "", "3", "1", "3", "", "", "", "", "2", "2", "", "", "0.933293097", "", "", "3 1", "3 1 3", "", "", "", "", "", "", "0.574862231", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", });
            dt.Rows.Add(new object[] { "True", "", "", "", "", "", "", "", "", "3", "2", "1", "", "", "", "", "2", "2", "", "", "0.668001048", "", "", "3 2", "3 2 1", "", "", "", "", "", "", "0.082828294", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", });
            dt.Rows.Add(new object[] { "True", "", "", "", "", "", "", "", "", "3", "2", "2", "", "", "", "", "2", "2", "", "", "0.163017489", "", "", "3 2", "3 2 2", "", "", "", "", "", "", "0.649623428", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", });
            dt.Rows.Add(new object[] { "True", "", "", "", "", "", "", "", "", "3", "2", "3", "", "", "", "", "2", "2", "", "", "0.166981575", "", "", "3 2", "3 2 3", "", "", "", "", "", "", "0.206084478", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", });
            dt.Rows.Add(new object[] { "True", "", "", "", "", "", "", "", "", "3", "3", "1", "", "", "", "", "2", "2", "", "", "0.37179156", "", "", "3 3", "3 3 1", "", "", "", "", "", "", "0.13737506", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", });
            dt.Rows.Add(new object[] { "True", "", "", "", "", "", "", "", "", "3", "3", "2", "", "", "", "", "2", "2", "", "", "0.414807821", "", "", "3 3", "3 3 2", "", "", "", "", "", "", "0.576234954", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", });
            dt.Rows.Add(new object[] { "True", "", "", "", "", "", "", "", "", "3", "3", "3", "", "", "", "", "2", "2", "", "", "0.641346207", "", "", "3 3", "3 3 3", "", "", "", "", "", "", "0.038940028", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", });
            dt.Rows.Add(new object[] { "True", "", "", "", "", "", "", "", "", "1", "1", "1", "", "", "", "", "3", "2", "", "", "0.262910505", "", "", "1 1", "1 1 1", "", "", "", "", "", "", "0.872421503", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", });
            dt.Rows.Add(new object[] { "True", "", "", "", "", "", "", "", "", "1", "1", "2", "", "", "", "", "3", "2", "", "", "0.459119831", "", "", "1 1", "1 1 2", "", "", "", "", "", "", "0.255396374", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", });
            dt.Rows.Add(new object[] { "True", "", "", "", "", "", "", "", "", "1", "1", "3", "", "", "", "", "3", "2", "", "", "0.42164397", "", "", "1 1", "1 1 3", "", "", "", "", "", "", "0.227187691", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", });
            dt.Rows.Add(new object[] { "True", "", "", "", "", "", "", "", "", "1", "2", "1", "", "", "", "", "3", "2", "", "", "0.106415423", "", "", "1 2", "1 2 1", "", "", "", "", "", "", "0.176753395", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", });
            dt.Rows.Add(new object[] { "True", "", "", "", "", "", "", "", "", "1", "2", "2", "", "", "", "", "3", "2", "", "", "0.394025439", "", "", "1 2", "1 2 2", "", "", "", "", "", "", "0.31719608", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", });
            dt.Rows.Add(new object[] { "True", "", "", "", "", "", "", "", "", "1", "2", "3", "", "", "", "", "3", "2", "", "", "0.437790644", "", "", "1 2", "1 2 3", "", "", "", "", "", "", "0.935162047", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", });
            dt.Rows.Add(new object[] { "True", "", "", "", "", "", "", "", "", "1", "3", "1", "", "", "", "", "3", "2", "", "", "0.784697854", "", "", "1 3", "1 3 1", "", "", "", "", "", "", "0.753240339", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", });
            dt.Rows.Add(new object[] { "True", "", "", "", "", "", "", "", "", "1", "3", "2", "", "", "", "", "3", "2", "", "", "0.243678086", "", "", "1 3", "1 3 2", "", "", "", "", "", "", "0.728951466", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", });
            dt.Rows.Add(new object[] { "True", "", "", "", "", "", "", "", "", "1", "3", "3", "", "", "", "", "3", "2", "", "", "0.424501836", "", "", "1 3", "1 3 3", "", "", "", "", "", "", "0.373812562", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", });
            dt.Rows.Add(new object[] { "True", "", "", "", "", "", "", "", "", "2", "1", "1", "", "", "", "", "3", "2", "", "", "0.099419613", "", "", "2 1", "2 1 1", "", "", "", "", "", "", "0.243853792", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", });
            dt.Rows.Add(new object[] { "True", "", "", "", "", "", "", "", "", "2", "1", "2", "", "", "", "", "3", "2", "", "", "0.239853841", "", "", "2 1", "2 1 2", "", "", "", "", "", "", "0.611345945", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", });
            dt.Rows.Add(new object[] { "True", "", "", "", "", "", "", "", "", "2", "1", "3", "", "", "", "", "3", "2", "", "", "0.135996552", "", "", "2 1", "2 1 3", "", "", "", "", "", "", "0.815897024", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", });
            dt.Rows.Add(new object[] { "True", "", "", "", "", "", "", "", "", "2", "2", "1", "", "", "", "", "3", "2", "", "", "0.692553384", "", "", "2 2", "2 2 1", "", "", "", "", "", "", "0.833467185", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", });
            dt.Rows.Add(new object[] { "True", "", "", "", "", "", "", "", "", "2", "2", "2", "", "", "", "", "3", "2", "", "", "0.048460663", "", "", "2 2", "2 2 2", "", "", "", "", "", "", "0.778111917", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", });
            dt.Rows.Add(new object[] { "True", "", "", "", "", "", "", "", "", "2", "2", "3", "", "", "", "", "3", "2", "", "", "0.479952376", "", "", "2 2", "2 2 3", "", "", "", "", "", "", "0.143495431", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", });
            dt.Rows.Add(new object[] { "True", "", "", "", "", "", "", "", "", "2", "3", "1", "", "", "", "", "3", "2", "", "", "0.920441815", "", "", "2 3", "2 3 1", "", "", "", "", "", "", "0.823613595", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", });
            dt.Rows.Add(new object[] { "True", "", "", "", "", "", "", "", "", "2", "3", "2", "", "", "", "", "3", "2", "", "", "0.303724529", "", "", "2 3", "2 3 2", "", "", "", "", "", "", "0.870389207", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", });
            dt.Rows.Add(new object[] { "True", "", "", "", "", "", "", "", "", "2", "3", "3", "", "", "", "", "3", "2", "", "", "0.36359941", "", "", "2 3", "2 3 3", "", "", "", "", "", "", "0.089924883", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", });
            dt.Rows.Add(new object[] { "True", "", "", "", "", "", "", "", "", "3", "1", "1", "", "", "", "", "3", "2", "", "", "0.867200511", "", "", "3 1", "3 1 1", "", "", "", "", "", "", "0.121133884", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", });
            dt.Rows.Add(new object[] { "True", "", "", "", "", "", "", "", "", "3", "1", "2", "", "", "", "", "3", "2", "", "", "0.893723238", "", "", "3 1", "3 1 2", "", "", "", "", "", "", "0.987545301", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", });
            dt.Rows.Add(new object[] { "True", "", "", "", "", "", "", "", "", "3", "1", "3", "", "", "", "", "3", "2", "", "", "0.047365157", "", "", "3 1", "3 1 3", "", "", "", "", "", "", "0.467247331", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", });
            dt.Rows.Add(new object[] { "True", "", "", "", "", "", "", "", "", "3", "2", "1", "", "", "", "", "3", "2", "", "", "0.740278339", "", "", "3 2", "3 2 1", "", "", "", "", "", "", "0.384536091", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", });
            dt.Rows.Add(new object[] { "True", "", "", "", "", "", "", "", "", "3", "2", "2", "", "", "", "", "3", "2", "", "", "0.389638593", "", "", "3 2", "3 2 2", "", "", "", "", "", "", "0.484897496", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", });
            dt.Rows.Add(new object[] { "True", "", "", "", "", "", "", "", "", "3", "2", "3", "", "", "", "", "3", "2", "", "", "0.199362195", "", "", "3 2", "3 2 3", "", "", "", "", "", "", "0.851948685", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", });
            dt.Rows.Add(new object[] { "True", "", "", "", "", "", "", "", "", "3", "3", "1", "", "", "", "", "3", "2", "", "", "0.261657778", "", "", "3 3", "3 3 1", "", "", "", "", "", "", "0.147444286", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", });
            dt.Rows.Add(new object[] { "True", "", "", "", "", "", "", "", "", "3", "3", "2", "", "", "", "", "3", "2", "", "", "0.343509031", "", "", "3 3", "3 3 2", "", "", "", "", "", "", "0.952360471", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", });
            dt.Rows.Add(new object[] { "True", "", "", "", "", "", "", "", "", "3", "3", "3", "", "", "", "", "3", "2", "", "", "0.833756537", "", "", "3 3", "3 3 3", "", "", "", "", "", "", "0.813524328", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", });

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