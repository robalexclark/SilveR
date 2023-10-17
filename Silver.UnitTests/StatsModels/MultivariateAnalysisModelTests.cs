using Moq;
using SilveR.Models;
using SilveR.StatsModels;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using Xunit;

namespace SilveR.UnitTests.StatsModels
{
    public class MultivariateAnalysisModelTests
    {
        [Fact]
        public void ScriptFileName_ReturnsCorrectString()
        {
            //Arrange
            System.Threading.Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
            MultivariateAnalysisModel sut = new MultivariateAnalysisModel();

            //Act
            string result = sut.ScriptFileName;

            //Assert
            Assert.Equal("MultivariateAnalysis", result);
        }

        [Fact]
        public void TransformationsList_ReturnsCorrectList()
        {
            //Arrange
            System.Threading.Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
            MultivariateAnalysisModel sut = new MultivariateAnalysisModel();

            //Act
            IEnumerable<string> result = sut.TransformationsList;

            //Assert
            Assert.IsAssignableFrom<IEnumerable<string>>(result);
            Assert.Equal(new List<string>() { "None", "Log10", "Loge", "Square Root", "ArcSine", "Rank" }, result);
        }

        [Fact]
        public void DistanceMethodsList_ReturnsCorrectList()
        {
            //Arrange
            System.Threading.Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
            MultivariateAnalysisModel sut = new MultivariateAnalysisModel();

            //Act
            IEnumerable<string> result = sut.DistanceMethodsList;

            //Assert
            Assert.IsAssignableFrom<IEnumerable<string>>(result);
            Assert.Equal(new List<string>() { "Euclidean", "Maximum", "Manhattan", "Canberra", "Binary", "Minkowski" }, result);
        }

        [Fact]
        public void AgglomerationMethodsList_ReturnsCorrectList()
        {
            //Arrange
            System.Threading.Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
            MultivariateAnalysisModel sut = new MultivariateAnalysisModel();

            //Act
            IEnumerable<string> result = sut.AgglomerationMethodsList;

            //Assert
            Assert.IsAssignableFrom<IEnumerable<string>>(result);
            Assert.Equal(new List<string>() { "Ward.d2", "Single", "Complete", "Average" }, result);
        }

        [Fact]
        public void PlotLabelsList_ReturnsCorrectList()
        {
            //Arrange
            System.Threading.Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
            MultivariateAnalysisModel sut = new MultivariateAnalysisModel();

            //Act
            IEnumerable<string> result = sut.PlotLabelsList;

            //Assert
            Assert.IsAssignableFrom<IEnumerable<string>>(result);
            Assert.Equal(new List<string>() { "Case ID", "Categorical Predictor" }, result);
        }

        [Fact]
        public void ResponseCentringList_ReturnsCorrectList()
        {
            //Arrange
            System.Threading.Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
            MultivariateAnalysisModel sut = new MultivariateAnalysisModel();

            //Act
            IEnumerable<string> result = sut.ResponseCentringList;

            //Assert
            Assert.IsAssignableFrom<IEnumerable<string>>(result);
            Assert.Equal(new List<string>() { "Centered at zero", "Not centered" }, result);
        }

        [Fact]
        public void ResponseScaleList_ReturnsCorrectList()
        {
            //Arrange
            System.Threading.Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
            MultivariateAnalysisModel sut = new MultivariateAnalysisModel();

            //Act
            IEnumerable<string> result = sut.ResponseScaleList;

            //Assert
            Assert.IsAssignableFrom<IEnumerable<string>>(result);
            Assert.Equal(new List<string>() { "Unit variance", "No scaling" }, result);
        }

        [Fact]
        public void ExportData_ReturnsCorrectStringArray()
        {
            //Arrange
            System.Threading.Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
            Mock<IDataset> mockDataset = new Mock<IDataset>();
            mockDataset.Setup(x => x.DatasetID).Returns(1);
            mockDataset.Setup(x => x.DatasetToDataTable()).Returns(GetTestDataTable());

            MultivariateAnalysisModel sut = GetModel(mockDataset.Object);

            //Act
            string[] result = sut.ExportData();

            //Assert
            Assert.Equal("Petalivs_sp_ivslength,Categorical,Continuous,Caseivs_sp_ivsID", result[0]);
            Assert.Equal(151, result.Count()); //as blank reponses are removed
            Assert.StartsWith("1.5,D,0.860808933734089,A32", result[32]);
        }

        [Fact]
        public void GetArguments_ReturnsCorrectArguments()
        {
            //Arrange
            System.Threading.Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
            MultivariateAnalysisModel sut = GetModel(GetDataset());

            //Act
            List<Argument> result = sut.GetArguments().ToList();

            //Assert
            var response = result.Single(x => x.Name == "Responses");
            Assert.Equal("Petal length", response.Value);

            var treatments = result.Single(x => x.Name == "CategoricalPredictor");
            Assert.Equal("Categorical", treatments.Value);

            var continuousPredictors = result.Single(x => x.Name == "ContinuousPredictors");
            Assert.Equal("Continuous", continuousPredictors.Value);

            var caseID = result.Single(x => x.Name == "CaseID");
            Assert.Equal("Case ID", caseID.Value);

            var analysisType = result.Single(x => x.Name == "AnalysisType");
            Assert.Equal("PrincipalComponentsAnalysis", analysisType.Value);

            var standardiseVariables = result.Single(x => x.Name == "StandardiseVariables");
            Assert.Equal("True", standardiseVariables.Value);

            var noOfClusters = result.Single(x => x.Name == "NoOfClusters");
            Assert.Equal("2", noOfClusters.Value);

            var distanceMethod = result.Single(x => x.Name == "DistanceMethod");
            Assert.Equal("Euclidean", distanceMethod.Value);

            var agglomerationMethod = result.Single(x => x.Name == "AgglomerationMethod");
            Assert.Equal("Ward.d2", agglomerationMethod.Value);

            var plotLabel = result.Single(x => x.Name == "PlotLabel");
            Assert.Equal("Case ID", plotLabel.Value);

            var responseCentring = result.Single(x => x.Name == "ResponseCentring");
            Assert.Equal("Centered at zero", responseCentring.Value);

            var responseScale = result.Single(x => x.Name == "ResponseScale");
            Assert.Equal("Unit variance", responseScale.Value);
        }

        [Fact]
        public void LoadArguments_ReturnsCorrectArguments()
        {
            //Arrange
            System.Threading.Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
            MultivariateAnalysisModel sut = new MultivariateAnalysisModel(GetDataset());

            List<Argument> arguments = new List<Argument>();
            arguments.Add(new Argument { Name = "Responses", Value = "Petal length" });
            arguments.Add(new Argument { Name = "CategoricalPredictor", Value = "Categorical" });
            arguments.Add(new Argument { Name = "ContinuousPredictors", Value = "Continuous" });
            arguments.Add(new Argument { Name = "ResponseTransformation", Value = "None" });
            arguments.Add(new Argument { Name = "CaseID", Value = "Case ID" });
            arguments.Add(new Argument { Name = "AnalysisType", Value = "PrincipalComponentsAnalysis" });
            arguments.Add(new Argument { Name = "StandardiseVariables", Value = "False" });
            arguments.Add(new Argument { Name = "NoOfClusters", Value = "2" });
            arguments.Add(new Argument { Name = "DistanceMethod", Value = "Euclidean" });
            arguments.Add(new Argument { Name = "AgglomerationMethod", Value = "Ward.d2" });
            arguments.Add(new Argument { Name = "PlotLabel", Value = "Case ID" });
            arguments.Add(new Argument { Name = "ResponseCentring", Value = "Centered at zero" });
            arguments.Add(new Argument { Name = "ResponseScale", Value = "Unit variance" });
            arguments.Add(new Argument { Name = "NoOfComponents", Value = "0" });

            Assert.Equal(14, arguments.Count);

            //Act
            sut.LoadArguments(arguments);

            //Assert
            Assert.Equal(new List<string> { "Petal length" }, sut.Responses);
            Assert.Equal("Categorical", sut.CategoricalPredictor);
            Assert.Equal(new List<string> { "Continuous" }, sut.ContinuousPredictors);
            Assert.Equal("None", sut.ResponseTransformation);
            Assert.Equal("Case ID", sut.CaseID);
            Assert.Equal("PrincipalComponentsAnalysis", sut.AnalysisType.ToString());
            Assert.False(sut.StandardiseVariables);
            Assert.Equal(2, sut.NoOfClusters);
            Assert.Equal("Euclidean", sut.DistanceMethod);
            Assert.Equal("Ward.d2", sut.AgglomerationMethod);
            Assert.Equal("Case ID", sut.PlotLabel);
            Assert.Equal("Centered at zero", sut.ResponseCentring);
            Assert.Equal("Unit variance", sut.ResponseScale);
        }

        [Fact]
        public void GetCommandLineArguments_ReturnsCorrectString()
        {
            //Arrange
            System.Threading.Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
            MultivariateAnalysisModel sut = GetModel(GetDataset());

            //Act
            string result = sut.GetCommandLineArguments();

            //Assert

            Assert.Equal("Petalivs_sp_ivslength None Categorical Continuous Caseivs_sp_ivsID PrincipalComponentsAnalysis 2 Y Euclidean Ward.d2 \"Case ID\" 0 Centered_at_zero Unit_variance", result);
        }

        private MultivariateAnalysisModel GetModel(IDataset dataset)
        {
            MultivariateAnalysisModel model = new MultivariateAnalysisModel(dataset)
            {
                Responses = new List<string> { "Petal length" },
                CategoricalPredictor = "Categorical",
                ContinuousPredictors = new List<string>() { "Continuous" },
                CaseID = "Case ID",
                ResponseTransformation = "None",
                AnalysisType = MultivariateAnalysisModel.AnalysisOption.PrincipalComponentsAnalysis,
                NoOfClusters = 2,
                DistanceMethod = "Euclidean",
                AgglomerationMethod = "Ward.d2",
                PlotLabel = "Case ID",
                ResponseCentring = "Centered at zero",
                ResponseScale = "Unit variance",
                NoOfComponents = 0
            };

            return model;
        }

        private DataTable GetTestDataTable()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("SilveRSelected");
            dt.Columns.Add("Sepal length");
            dt.Columns.Add("Sepal width");
            dt.Columns.Add("Petal length");
            dt.Columns.Add("Petal width");
            dt.Columns.Add("Species");
            dt.Columns.Add("Petal width (reduced)");
            dt.Columns.Add("Species (reduced)");
            dt.Columns.Add("Categorical");
            dt.Columns.Add("Continuous");
            dt.Columns.Add("Case ID");
            dt.Rows.Add(new object[] { "True", "5.1", "3.5", "1.4", "0.2", "I. setosa", "", "I. setosa", "A", "0.335512636813198", "A1", });
            dt.Rows.Add(new object[] { "True", "4.9", "3", "1.4", "0.2", "I. setosa", "0.2", "", "B", "0.698664032777821", "A2", });
            dt.Rows.Add(new object[] { "True", "4.7", "3.2", "1.3", "0.2", "I. setosa", "0.2", "I. setosa", "C", "0.0158340287929346", "A3", });
            dt.Rows.Add(new object[] { "True", "4.6", "3.1", "1.5", "0.2", "I. setosa", "0.2", "I. setosa", "D", "0.893406489287247", "A4", });
            dt.Rows.Add(new object[] { "True", "5", "3.6", "1.4", "0.3", "I. setosa", "0.3", "I. setosa", "A", "0.713529873547215", "A5", });
            dt.Rows.Add(new object[] { "True", "5.4", "3.9", "1.7", "0.4", "I. setosa", "0.4", "I. setosa", "B", "0.10203418432085", "A6", });
            dt.Rows.Add(new object[] { "True", "4.6", "3.4", "1.4", "0.3", "I. setosa", "0.3", "I. setosa", "C", "0.394339975610907", "A7", });
            dt.Rows.Add(new object[] { "True", "5", "3.4", "1.5", "0.2", "I. setosa", "0.2", "I. setosa", "D", "0.117653407874262", "A8", });
            dt.Rows.Add(new object[] { "True", "4.4", "2.9", "1.4", "0.2", "I. setosa", "0.2", "I. setosa", "A", "0.885451680807561", "A9", });
            dt.Rows.Add(new object[] { "True", "4.9", "3.1", "1.5", "0.1", "I. setosa", "0.1", "I. setosa", "B", "0.790181664602587", "A10", });
            dt.Rows.Add(new object[] { "True", "5.4", "3.7", "1.5", "0.2", "I. setosa", "0.2", "I. setosa", "C", "0.692050257465093", "A11", });
            dt.Rows.Add(new object[] { "True", "4.8", "3.4", "1.6", "0.2", "I. setosa", "0.2", "I. setosa", "D", "0.877855576745672", "A12", });
            dt.Rows.Add(new object[] { "True", "4.8", "3", "1.4", "0.1", "I. setosa", "0.1", "I. setosa", "A", "0.545901964422923", "A13", });
            dt.Rows.Add(new object[] { "True", "4.3", "3", "1.1", "0.1", "I. setosa", "0.1", "I. setosa", "B", "0.237695362262642", "A14", });
            dt.Rows.Add(new object[] { "True", "5.8", "4", "1.2", "0.2", "I. setosa", "0.2", "I. setosa", "C", "0.220000268524801", "A15", });
            dt.Rows.Add(new object[] { "True", "5.7", "4.4", "1.5", "0.4", "I. setosa", "0.4", "I. setosa", "D", "0.00996427626778651", "A16", });
            dt.Rows.Add(new object[] { "True", "5.4", "3.9", "1.3", "0.4", "I. setosa", "0.4", "I. setosa", "A", "0.547187937992468", "A17", });
            dt.Rows.Add(new object[] { "True", "5.1", "3.5", "1.4", "0.3", "I. setosa", "0.3", "I. setosa", "B", "0.832462473189898", "A18", });
            dt.Rows.Add(new object[] { "True", "5.7", "3.8", "1.7", "0.3", "I. setosa", "0.3", "I. setosa", "C", "0.669528210484899", "A19", });
            dt.Rows.Add(new object[] { "True", "5.1", "3.8", "1.5", "0.3", "I. setosa", "0.3", "I. setosa", "D", "0.853091004896353", "A20", });
            dt.Rows.Add(new object[] { "True", "5.4", "3.4", "1.7", "0.2", "I. setosa", "0.2", "I. setosa", "A", "0.721148447650635", "A21", });
            dt.Rows.Add(new object[] { "True", "5.1", "3.7", "1.5", "0.4", "I. setosa", "0.4", "I. setosa", "B", "0.136706727037616", "A22", });
            dt.Rows.Add(new object[] { "True", "4.6", "3.6", "1", "0.2", "I. setosa", "0.2", "I. setosa", "C", "0.339262307827746", "A23", });
            dt.Rows.Add(new object[] { "True", "5.1", "3.3", "1.7", "0.5", "I. setosa", "0.5", "I. setosa", "D", "0.643145386721504", "A24", });
            dt.Rows.Add(new object[] { "True", "4.8", "3.4", "1.9", "0.2", "I. setosa", "0.2", "I. setosa", "A", "0.939735394007646", "A25", });
            dt.Rows.Add(new object[] { "True", "5", "3", "1.6", "0.2", "I. setosa", "0.2", "I. setosa", "B", "0.789601100639465", "A26", });
            dt.Rows.Add(new object[] { "True", "5", "3.4", "1.6", "0.4", "I. setosa", "0.4", "I. setosa", "C", "0.349353887338268", "A27", });
            dt.Rows.Add(new object[] { "True", "5.2", "3.5", "1.5", "0.2", "I. setosa", "0.2", "I. setosa", "D", "0.315979696549859", "A28", });
            dt.Rows.Add(new object[] { "True", "5.2", "3.4", "1.4", "0.2", "I. setosa", "0.2", "I. setosa", "A", "0.441419787838927", "A29", });
            dt.Rows.Add(new object[] { "True", "4.7", "3.2", "1.6", "0.2", "I. setosa", "0.2", "I. setosa", "B", "0.555100529314767", "A30", });
            dt.Rows.Add(new object[] { "True", "4.8", "3.1", "1.6", "0.2", "I. setosa", "0.2", "I. setosa", "C", "0.705521280731987", "A31", });
            dt.Rows.Add(new object[] { "True", "5.4", "3.4", "1.5", "0.4", "I. setosa", "0.4", "I. setosa", "D", "0.860808933734089", "A32", });
            dt.Rows.Add(new object[] { "True", "5.2", "4.1", "1.5", "0.1", "I. setosa", "0.1", "I. setosa", "A", "0.998069358692611", "A33", });
            dt.Rows.Add(new object[] { "True", "5.5", "4.2", "1.4", "0.2", "I. setosa", "0.2", "I. setosa", "B", "0.0572462549561801", "A34", });
            dt.Rows.Add(new object[] { "True", "4.9", "3.1", "1.5", "0.2", "I. setosa", "0.2", "I. setosa", "C", "0.0408285826201266", "A35", });
            dt.Rows.Add(new object[] { "True", "5", "3.2", "1.2", "0.2", "I. setosa", "0.2", "I. setosa", "D", "0.40698715922983", "A36", });
            dt.Rows.Add(new object[] { "True", "5.5", "3.5", "1.3", "0.2", "I. setosa", "0.2", "I. setosa", "A", "0.172859274542041", "A37", });
            dt.Rows.Add(new object[] { "True", "4.9", "3.6", "1.4", "0.1", "I. setosa", "0.1", "I. setosa", "B", "0.470184836532752", "A38", });
            dt.Rows.Add(new object[] { "True", "4.4", "3", "1.3", "0.2", "I. setosa", "0.2", "I. setosa", "C", "0.103766635382854", "A39", });
            dt.Rows.Add(new object[] { "True", "5.1", "3.4", "1.5", "0.2", "I. setosa", "0.2", "I. setosa", "D", "0.555277407513619", "A40", });
            dt.Rows.Add(new object[] { "True", "5", "3.5", "1.3", "0.3", "I. setosa", "0.3", "I. setosa", "A", "0.611800439143317", "A41", });
            dt.Rows.Add(new object[] { "True", "4.5", "2.3", "1.3", "0.3", "I. setosa", "0.3", "I. setosa", "B", "0.4339868672903", "A42", });
            dt.Rows.Add(new object[] { "True", "4.4", "3.2", "1.3", "0.2", "I. setosa", "0.2", "I. setosa", "C", "0.579798252303908", "A43", });
            dt.Rows.Add(new object[] { "True", "5", "3.5", "1.6", "0.6", "I. setosa", "0.6", "I. setosa", "D", "0.934383920535331", "A44", });
            dt.Rows.Add(new object[] { "True", "5.1", "3.8", "1.9", "0.4", "I. setosa", "0.4", "I. setosa", "A", "0.525218574732697", "A45", });
            dt.Rows.Add(new object[] { "True", "4.8", "3", "1.4", "0.3", "I. setosa", "0.3", "I. setosa", "B", "0.507036033730375", "A46", });
            dt.Rows.Add(new object[] { "True", "5.1", "3.8", "1.6", "0.2", "I. setosa", "0.2", "I. setosa", "C", "0.876346950293046", "A47", });
            dt.Rows.Add(new object[] { "True", "4.6", "3.2", "1.4", "0.2", "I. setosa", "0.2", "I. setosa", "D", "0.102888365751863", "A48", });
            dt.Rows.Add(new object[] { "True", "5.3", "3.7", "1.5", "0.2", "I. setosa", "0.2", "I. setosa", "A", "0.379052849684447", "A49", });
            dt.Rows.Add(new object[] { "True", "5", "3.3", "1.4", "0.2", "I. setosa", "0.2", "I. setosa", "B", "0.812279288699097", "A50", });
            dt.Rows.Add(new object[] { "True", "7", "3.2", "4.7", "1.4", "I. versicolor", "1.4", "I. versicolor", "C", "0.621344091446512", "A51", });
            dt.Rows.Add(new object[] { "True", "6.4", "3.2", "4.5", "1.5", "I. versicolor", "1.5", "I. versicolor", "D", "0.47163743990281", "A52", });
            dt.Rows.Add(new object[] { "True", "6.9", "3.1", "4.9", "1.5", "I. versicolor", "1.5", "I. versicolor", "A", "0.151180936779436", "A53", });
            dt.Rows.Add(new object[] { "True", "5.5", "2.3", "4", "1.3", "I. versicolor", "1.3", "I. versicolor", "B", "0.826680402265555", "A54", });
            dt.Rows.Add(new object[] { "True", "6.5", "2.8", "4.6", "1.5", "I. versicolor", "1.5", "I. versicolor", "C", "0.974841090830527", "A55", });
            dt.Rows.Add(new object[] { "True", "5.7", "2.8", "4.5", "1.3", "I. versicolor", "1.3", "I. versicolor", "D", "0.852852051920284", "A56", });
            dt.Rows.Add(new object[] { "True", "6.3", "3.3", "4.7", "1.6", "I. versicolor", "1.6", "I. versicolor", "A", "0.306975159925016", "A57", });
            dt.Rows.Add(new object[] { "True", "4.9", "2.4", "3.3", "1", "I. versicolor", "1", "I. versicolor", "B", "0.91647063491974", "A58", });
            dt.Rows.Add(new object[] { "True", "6.6", "2.9", "4.6", "1.3", "I. versicolor", "1.3", "I. versicolor", "C", "0.0133508397273989", "A59", });
            dt.Rows.Add(new object[] { "True", "5.2", "2.7", "3.9", "1.4", "I. versicolor", "1.4", "I. versicolor", "D", "0.195938999637729", "A60", });
            dt.Rows.Add(new object[] { "True", "5", "2", "3.5", "1", "I. versicolor", "1", "I. versicolor", "A", "0.474521180175222", "A61", });
            dt.Rows.Add(new object[] { "True", "5.9", "3", "4.2", "1.5", "I. versicolor", "1.5", "I. versicolor", "B", "0.818821080657806", "A62", });
            dt.Rows.Add(new object[] { "True", "6", "2.2", "4", "1", "I. versicolor", "1", "I. versicolor", "C", "0.799233404911128", "A63", });
            dt.Rows.Add(new object[] { "True", "6.1", "2.9", "4.7", "1.4", "I. versicolor", "1.4", "I. versicolor", "D", "0.922851800482066", "A64", });
            dt.Rows.Add(new object[] { "True", "5.6", "2.9", "3.6", "1.3", "I. versicolor", "1.3", "I. versicolor", "A", "0.924366109061615", "A65", });
            dt.Rows.Add(new object[] { "True", "6.7", "3.1", "4.4", "1.4", "I. versicolor", "1.4", "I. versicolor", "B", "0.292869397310129", "A66", });
            dt.Rows.Add(new object[] { "True", "5.6", "3", "4.5", "1.5", "I. versicolor", "1.5", "I. versicolor", "C", "0.971214606108357", "A67", });
            dt.Rows.Add(new object[] { "True", "5.8", "2.7", "4.1", "1", "I. versicolor", "1", "I. versicolor", "D", "0.571081397540188", "A68", });
            dt.Rows.Add(new object[] { "True", "6.2", "2.2", "4.5", "1.5", "I. versicolor", "1.5", "I. versicolor", "A", "0.289295604279777", "A69", });
            dt.Rows.Add(new object[] { "True", "5.6", "2.5", "3.9", "1.1", "I. versicolor", "1.1", "I. versicolor", "B", "0.0259598210828972", "A70", });
            dt.Rows.Add(new object[] { "True", "5.9", "3.2", "4.8", "1.8", "I. versicolor", "1.8", "I. versicolor", "C", "0.933244172585466", "A71", });
            dt.Rows.Add(new object[] { "True", "6.1", "2.8", "4", "1.3", "I. versicolor", "1.3", "I. versicolor", "D", "0.295960689796918", "A72", });
            dt.Rows.Add(new object[] { "True", "6.3", "2.5", "4.9", "1.5", "I. versicolor", "1.5", "I. versicolor", "A", "0.823716504899356", "A73", });
            dt.Rows.Add(new object[] { "True", "6.1", "2.8", "4.7", "1.2", "I. versicolor", "1.2", "I. versicolor", "B", "0.825983377425496", "A74", });
            dt.Rows.Add(new object[] { "True", "6.4", "2.9", "4.3", "1.3", "I. versicolor", "1.3", "I. versicolor", "C", "0.7714785136223", "A75", });
            dt.Rows.Add(new object[] { "True", "6.6", "3", "4.4", "1.4", "I. versicolor", "1.4", "I. versicolor", "D", "0.612011891363328", "A76", });
            dt.Rows.Add(new object[] { "True", "6.8", "2.8", "4.8", "1.4", "I. versicolor", "1.4", "I. versicolor", "A", "0.746926588418504", "A77", });
            dt.Rows.Add(new object[] { "True", "6.7", "3", "5", "1.7", "I. versicolor", "1.7", "I. versicolor", "B", "0.362011147673961", "A78", });
            dt.Rows.Add(new object[] { "True", "6", "2.9", "4.5", "1.5", "I. versicolor", "1.5", "I. versicolor", "C", "0.692452991647381", "A79", });
            dt.Rows.Add(new object[] { "True", "5.7", "2.6", "3.5", "1", "I. versicolor", "1", "I. versicolor", "D", "0.804311643260931", "A80", });
            dt.Rows.Add(new object[] { "True", "5.5", "2.4", "3.8", "1.1", "I. versicolor", "1.1", "I. versicolor", "A", "0.871824425759684", "A81", });
            dt.Rows.Add(new object[] { "True", "5.5", "2.4", "3.7", "1", "I. versicolor", "1", "I. versicolor", "B", "0.718355996836689", "A82", });
            dt.Rows.Add(new object[] { "True", "5.8", "2.7", "3.9", "1.2", "I. versicolor", "1.2", "I. versicolor", "C", "0.150399983828327", "A83", });
            dt.Rows.Add(new object[] { "True", "6", "2.7", "5.1", "1.6", "I. versicolor", "1.6", "I. versicolor", "D", "0.367437086159636", "A84", });
            dt.Rows.Add(new object[] { "True", "5.4", "3", "4.5", "1.5", "I. versicolor", "1.5", "I. versicolor", "A", "0.778977218089348", "A85", });
            dt.Rows.Add(new object[] { "True", "6", "3.4", "4.5", "1.6", "I. versicolor", "1.6", "I. versicolor", "B", "0.365517706963811", "A86", });
            dt.Rows.Add(new object[] { "True", "6.7", "3.1", "4.7", "1.5", "I. versicolor", "1.5", "I. versicolor", "C", "0.682461960676216", "A87", });
            dt.Rows.Add(new object[] { "True", "6.3", "2.3", "4.4", "1.3", "I. versicolor", "1.3", "I. versicolor", "D", "0.257834695132658", "A88", });
            dt.Rows.Add(new object[] { "True", "5.6", "3", "4.1", "1.3", "I. versicolor", "1.3", "I. versicolor", "A", "0.943645078131908", "A89", });
            dt.Rows.Add(new object[] { "True", "5.5", "2.5", "4", "1.3", "I. versicolor", "1.3", "I. versicolor", "B", "0.617821724025407", "A90", });
            dt.Rows.Add(new object[] { "True", "5.5", "2.6", "4.4", "1.2", "I. versicolor", "1.2", "I. versicolor", "C", "0.521714489910348", "A91", });
            dt.Rows.Add(new object[] { "True", "6.1", "3", "4.6", "1.4", "I. versicolor", "1.4", "I. versicolor", "D", "0.0286323432706287", "A92", });
            dt.Rows.Add(new object[] { "True", "5.8", "2.6", "4", "1.2", "I. versicolor", "1.2", "I. versicolor", "A", "0.431987075950654", "A93", });
            dt.Rows.Add(new object[] { "True", "5", "2.3", "3.3", "1", "I. versicolor", "1", "I. versicolor", "B", "0.93498497003334", "A94", });
            dt.Rows.Add(new object[] { "True", "5.6", "2.7", "4.2", "1.3", "I. versicolor", "1.3", "I. versicolor", "C", "0.616102781547618", "A95", });
            dt.Rows.Add(new object[] { "True", "5.7", "3", "4.2", "1.2", "I. versicolor", "1.2", "I. versicolor", "D", "0.144092332385486", "A96", });
            dt.Rows.Add(new object[] { "True", "5.7", "2.9", "4.2", "1.3", "I. versicolor", "1.3", "I. versicolor", "A", "0.926859217826922", "A97", });
            dt.Rows.Add(new object[] { "True", "6.2", "2.9", "4.3", "1.3", "I. versicolor", "1.3", "I. versicolor", "B", "0.948509422417991", "A98", });
            dt.Rows.Add(new object[] { "True", "5.1", "2.5", "3", "1.1", "I. versicolor", "1.1", "I. versicolor", "C", "0.469487391442765", "A99", });
            dt.Rows.Add(new object[] { "True", "5.7", "2.8", "4.1", "1.3", "I. versicolor", "1.3", "I. versicolor", "D", "0.567658537187576", "A100", });
            dt.Rows.Add(new object[] { "True", "6.3", "3.3", "6", "2.5", "I. virginica", "2.5", "I. virginica", "A", "0.568733146249937", "A101", });
            dt.Rows.Add(new object[] { "True", "5.8", "2.7", "5.1", "1.9", "I. virginica", "1.9", "I. virginica", "B", "0.107226730595382", "A102", });
            dt.Rows.Add(new object[] { "True", "7.1", "3", "5.9", "2.1", "I. virginica", "2.1", "I. virginica", "C", "0.337214753654646", "A103", });
            dt.Rows.Add(new object[] { "True", "6.3", "2.9", "5.6", "1.8", "I. virginica", "1.8", "I. virginica", "D", "0.615284980994377", "A104", });
            dt.Rows.Add(new object[] { "True", "6.5", "3", "5.8", "2.2", "I. virginica", "2.2", "I. virginica", "A", "0.79199520073206", "A105", });
            dt.Rows.Add(new object[] { "True", "7.6", "3", "6.6", "2.1", "I. virginica", "2.1", "I. virginica", "B", "0.632800690002741", "A106", });
            dt.Rows.Add(new object[] { "True", "4.9", "2.5", "4.5", "1.7", "I. virginica", "1.7", "I. virginica", "C", "0.732282679490626", "A107", });
            dt.Rows.Add(new object[] { "True", "7.3", "2.9", "6.3", "1.8", "I. virginica", "1.8", "I. virginica", "D", "0.0189914168877111", "A108", });
            dt.Rows.Add(new object[] { "True", "6.7", "2.5", "5.8", "1.8", "I. virginica", "1.8", "I. virginica", "A", "0.115785591392677", "A109", });
            dt.Rows.Add(new object[] { "True", "7.2", "3.6", "6.1", "2.5", "I. virginica", "2.5", "I. virginica", "B", "0.148902897353968", "A110", });
            dt.Rows.Add(new object[] { "True", "6.5", "3.2", "5.1", "2", "I. virginica", "2", "I. virginica", "C", "0.308911162578388", "A111", });
            dt.Rows.Add(new object[] { "True", "6.4", "2.7", "5.3", "1.9", "I. virginica", "1.9", "I. virginica", "D", "0.276709743744915", "A112", });
            dt.Rows.Add(new object[] { "True", "6.8", "3", "5.5", "2.1", "I. virginica", "2.1", "I. virginica", "A", "0.73111248133864", "A113", });
            dt.Rows.Add(new object[] { "True", "5.7", "2.5", "5", "2", "I. virginica", "2", "I. virginica", "B", "0.748602954561507", "A114", });
            dt.Rows.Add(new object[] { "True", "5.8", "2.8", "5.1", "2.4", "I. virginica", "2.4", "I. virginica", "C", "0.181912147434858", "A115", });
            dt.Rows.Add(new object[] { "True", "6.4", "3.2", "5.3", "2.3", "I. virginica", "2.3", "I. virginica", "D", "0.619359571446823", "A116", });
            dt.Rows.Add(new object[] { "True", "6.5", "3", "5.5", "1.8", "I. virginica", "1.8", "I. virginica", "A", "0.462970974746419", "A117", });
            dt.Rows.Add(new object[] { "True", "7.7", "3.8", "6.7", "2.2", "I. virginica", "2.2", "I. virginica", "B", "0.574484435049354", "A118", });
            dt.Rows.Add(new object[] { "True", "7.7", "2.6", "6.9", "2.3", "I. virginica", "2.3", "I. virginica", "C", "0.985715563912682", "A119", });
            dt.Rows.Add(new object[] { "True", "6", "2.2", "5", "1.5", "I. virginica", "1.5", "I. virginica", "D", "0.849100916112326", "A120", });
            dt.Rows.Add(new object[] { "True", "6.9", "3.2", "5.7", "2.3", "I. virginica", "2.3", "I. virginica", "A", "0.30023984962297", "A121", });
            dt.Rows.Add(new object[] { "True", "5.6", "2.8", "4.9", "2", "I. virginica", "2", "I. virginica", "B", "0.977754573169241", "A122", });
            dt.Rows.Add(new object[] { "True", "7.7", "2.8", "6.7", "2", "I. virginica", "2", "I. virginica", "C", "0.49185851239179", "A123", });
            dt.Rows.Add(new object[] { "True", "6.3", "2.7", "4.9", "1.8", "I. virginica", "1.8", "I. virginica", "D", "0.328592412367862", "A124", });
            dt.Rows.Add(new object[] { "True", "6.7", "3.3", "5.7", "2.1", "I. virginica", "2.1", "I. virginica", "A", "0.201343628264605", "A125", });
            dt.Rows.Add(new object[] { "True", "7.2", "3.2", "6", "1.8", "I. virginica", "1.8", "I. virginica", "B", "0.531963621798535", "A126", });
            dt.Rows.Add(new object[] { "True", "6.2", "2.8", "4.8", "1.8", "I. virginica", "1.8", "I. virginica", "C", "0.346896677239784", "A127", });
            dt.Rows.Add(new object[] { "True", "6.1", "3", "4.9", "1.8", "I. virginica", "1.8", "I. virginica", "D", "0.719546024231648", "A128", });
            dt.Rows.Add(new object[] { "True", "6.4", "2.8", "5.6", "2.1", "I. virginica", "2.1", "I. virginica", "A", "0.906431198287405", "A129", });
            dt.Rows.Add(new object[] { "True", "7.2", "3", "5.8", "1.6", "I. virginica", "1.6", "I. virginica", "B", "0.463963596516692", "A130", });
            dt.Rows.Add(new object[] { "True", "7.4", "2.8", "6.1", "1.9", "I. virginica", "1.9", "I. virginica", "C", "0.417462443909442", "A131", });
            dt.Rows.Add(new object[] { "True", "7.9", "3.8", "6.4", "2", "I. virginica", "2", "I. virginica", "D", "0.534581031939432", "A132", });
            dt.Rows.Add(new object[] { "True", "6.4", "2.8", "5.6", "2.2", "I. virginica", "2.2", "I. virginica", "A", "0.589323097751078", "A133", });
            dt.Rows.Add(new object[] { "True", "6.3", "2.8", "5.1", "1.5", "I. virginica", "1.5", "I. virginica", "B", "0.225794740699674", "A134", });
            dt.Rows.Add(new object[] { "True", "6.1", "2.6", "5.6", "1.4", "I. virginica", "1.4", "I. virginica", "C", "0.337102303404179", "A135", });
            dt.Rows.Add(new object[] { "True", "7.7", "3", "6.1", "2.3", "I. virginica", "2.3", "I. virginica", "D", "0.202291059214273", "A136", });
            dt.Rows.Add(new object[] { "True", "6.3", "3.4", "5.6", "2.4", "I. virginica", "2.4", "I. virginica", "A", "0.215551910999837", "A137", });
            dt.Rows.Add(new object[] { "True", "6.4", "3.1", "5.5", "1.8", "I. virginica", "1.8", "I. virginica", "B", "0.877973952778865", "A138", });
            dt.Rows.Add(new object[] { "True", "6", "3", "4.8", "1.8", "I. virginica", "1.8", "I. virginica", "C", "0.337994850816534", "A139", });
            dt.Rows.Add(new object[] { "True", "6.9", "3.1", "5.4", "2.1", "I. virginica", "2.1", "I. virginica", "D", "0.653891877057995", "A140", });
            dt.Rows.Add(new object[] { "True", "6.7", "3.1", "5.6", "2.4", "I. virginica", "2.4", "I. virginica", "A", "0.687790000648985", "A141", });
            dt.Rows.Add(new object[] { "True", "6.9", "3.1", "5.1", "2.3", "I. virginica", "2.3", "I. virginica", "B", "0.996628698188791", "A142", });
            dt.Rows.Add(new object[] { "True", "5.8", "2.7", "5.1", "1.9", "I. virginica", "1.9", "I. virginica", "C", "0.523774521768432", "A143", });
            dt.Rows.Add(new object[] { "True", "6.8", "3.2", "5.9", "2.3", "I. virginica", "2.3", "I. virginica", "D", "0.469822553867916", "A144", });
            dt.Rows.Add(new object[] { "True", "6.7", "3.3", "5.7", "2.5", "I. virginica", "2.5", "I. virginica", "A", "0.869494721350576", "A145", });
            dt.Rows.Add(new object[] { "True", "6.7", "3", "5.2", "2.3", "I. virginica", "2.3", "I. virginica", "B", "0.440813758982784", "A146", });
            dt.Rows.Add(new object[] { "True", "6.3", "2.5", "5", "1.9", "I. virginica", "1.9", "I. virginica", "C", "0.405459463793047", "A147", });
            dt.Rows.Add(new object[] { "True", "6.5", "3", "5.2", "2", "I. virginica", "2", "I. virginica", "D", "0.554347412188594", "A148", });
            dt.Rows.Add(new object[] { "True", "6.2", "3.4", "5.4", "2.3", "I. virginica", "2.3", "I. virginica", "A", "0.697128996668886", "A149", });
            dt.Rows.Add(new object[] { "True", "5.9", "3", "5.1", "1.8", "I. virginica", "1.8", "I. virginica", "B", "0.993320237193141", "A150", });

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