using SilveR.StatsModels;
using System.Collections.Generic;
using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace SilveR.IntegrationTests
{
    public class HasseDiagramGeneratorTests : IClassFixture<SilveRTestWebApplicationFactory<Startup>>
    {
        private const string ModuleName = "HasseDiagramGenerator";
        private static readonly string[] ConfoundedDegreeOfFreedomFactors =
        {
            "Subject_DF",
            "Day_DF",
            "Room_DF",
            "Period_DF",
            "Method_DF",
            "Test_DF"
        };

        private readonly SilveRTestWebApplicationFactory<Startup> _factory;

        public HasseDiagramGeneratorTests(SilveRTestWebApplicationFactory<Startup> factory)
        {
            _factory = factory;
        }

        [Fact]
        public async Task HDG1()
        {
            HasseDiagramGeneratorModel model = CreateModel();

            HttpResponseMessage response = await Post(model);
            IEnumerable<string> errors = await Helpers.ExtractErrors(response);

            Assert.Contains("Enter at least one fixed or random factor.", errors);
            Helpers.SaveOutput(ModuleName, nameof(HDG1), errors);
        }

        [Fact]
        public async Task HDG2()
        {
            HasseDiagramGeneratorModel model = CreateModel();
            model.FixedFactors = new[] { "Treatment" };
            model.RandomFactors = new[] { "Treatment" };

            HttpResponseMessage response = await Post(model);
            IEnumerable<string> errors = await Helpers.ExtractErrors(response);

            Assert.Contains("Fixed factors (Treatment) has been selected in more than one input category, please change your input options.", errors);
            Assert.Contains("Random factors (Treatment) has been selected in more than one input category, please change your input options.", errors);
            Helpers.SaveOutput(ModuleName, nameof(HDG2), errors);
        }

        [Fact]
        public async Task HDG3()
        {
            HasseDiagramGeneratorModel model = CreateModel();
            model.FixedFactors = new[] { "Animal" };

            HttpResponseMessage response = await Post(model);
            IEnumerable<string> warnings = await Helpers.ExtractWarnings(response);

            Assert.Contains("The fixed factor Animal has numerical levels. Note this factor will be treated as categoric in this assessment.", warnings);
            Helpers.SaveOutput(ModuleName, nameof(HDG3), warnings);
        }

        [Fact]
        public async Task HDG4()
        {
            HasseDiagramGeneratorModel model = CreateModel();
            model.RandomFactors = new[] { "Animal" };

            HttpResponseMessage response = await Post(model);
            IEnumerable<string> warnings = await Helpers.ExtractWarnings(response);

            Assert.Contains("The random factor Animal has numerical levels. Note this factor will be treated as categoric in this assessment.", warnings);
            Helpers.SaveOutput(ModuleName, nameof(HDG4), warnings);
        }

        [Fact]
        public async Task HDG5()
        {
            HasseDiagramGeneratorModel model = CreateModel();
            model.FixedFactors = new[] { "One" };

            HttpResponseMessage response = await Post(model);
            IEnumerable<string> errors = await Helpers.ExtractErrors(response);

            Assert.Contains("The fixed factor One contains only identical elements. If this is correct this variable should be removed prior to running the analysis.", errors);
            Helpers.SaveOutput(ModuleName, nameof(HDG5), errors);
        }

        [Fact]
        public async Task HDG6()
        {
            HasseDiagramGeneratorModel model = CreateModel();
            model.RandomFactors = new[] { "One" };

            HttpResponseMessage response = await Post(model);
            IEnumerable<string> errors = await Helpers.ExtractErrors(response);

            Assert.Contains("The random factor One contains only identical elements. If this is correct this variable should be removed prior to running the analysis.", errors);
            Helpers.SaveOutput(ModuleName, nameof(HDG6), errors);
        }

        [Fact]
        public async Task HDG7()
        {
            HasseDiagramGeneratorModel model = CreateModel();
            model.FixedFactors = new[] { "Treatment", "Cage_N" };
            model.RandomFactors = new[] { "Animal" };

            await AssertOutput(model, nameof(HDG7));
        }

        [Fact]
        public async Task HDG8()
        {
            HasseDiagramGeneratorModel model = CreateModel();
            model.FixedFactors = new[] { "Treatment", "Cage_X" };
            model.RandomFactors = new[] { "Animal" };

            await AssertOutput(model, nameof(HDG8));
        }

        [Fact]
        public async Task HDG9()
        {
            HasseDiagramGeneratorModel model = CreateConfoundedDegreeOfFreedomModel();
            model.CheckForConfoundedDegreesOfFreedom = true;

            await AssertOutput(model, nameof(HDG9));
        }

        [Fact]
        public async Task HDG10()
        {
            HasseDiagramGeneratorModel model = CreateConfoundedDegreeOfFreedomModel();
            model.CheckForConfoundedDegreesOfFreedom = false;

            await AssertOutput(model, nameof(HDG10));
        }

        [Fact]
        public async Task HDG11()
        {
            HasseDiagramGeneratorModel model = CreateCustomOutputModel();

            await AssertOutput(model, nameof(HDG11));
        }

        [Fact]
        public async Task HDG12()
        {
            HasseDiagramGeneratorModel model = CreateCustomOutputModel();
            model.ShowPartialCrossing = false;

            await AssertOutput(model, nameof(HDG12));
        }

        [Fact]
        public async Task HDG13()
        {
            HasseDiagramGeneratorModel model = CreateCustomOutputModel();
            model.ShowPartialCrossing = false;
            model.ShowDegreesOfFreedom = false;

            await AssertOutput(model, nameof(HDG13));
        }

        [Fact]
        public async Task HDG14()
        {
            HasseDiagramGeneratorModel model = CreateCustomOutputModel();
            model.ShowMaximumLevels = false;

            await AssertOutput(model, nameof(HDG14));
        }

        [Fact]
        public async Task HDG15()
        {
            HasseDiagramGeneratorModel model = CreateCustomOutputModel();
            model.FontColour = "blue";

            await AssertOutput(model, nameof(HDG15));
        }

        [Fact]
        public async Task HDG16()
        {
            HasseDiagramGeneratorModel model = CreateCustomOutputModel();
            model.FontColour = "blue";
            model.StructuralLineColour = "green";

            await AssertOutput(model, nameof(HDG16));
        }

        [Fact]
        public async Task HDG17()
        {
            HasseDiagramGeneratorModel model = CreateCustomOutputModel();
            model.FontColour = "blue";
            model.StructuralLineColour = "green";
            model.StructuralLineWidth = 4m;

            await AssertOutput(model, nameof(HDG17));
        }

        [Fact]
        public async Task HDG18()
        {
            HasseDiagramGeneratorModel model = CreateCustomOutputModel();
            model.FontColour = "blue";
            model.StructuralLineColour = "green";
            model.StructuralLineWidth = 4m;
            model.PartialCrossingLineColour = "black";

            await AssertOutput(model, nameof(HDG18));
        }

        [Fact]
        public async Task HDG19()
        {
            HasseDiagramGeneratorModel model = CreateCustomOutputModel();
            model.FontColour = "blue";
            model.StructuralLineColour = "green";
            model.StructuralLineWidth = 4m;
            model.PartialCrossingLineColour = "black";
            model.PartialCrossingLineWidth = 4m;

            await AssertOutput(model, nameof(HDG19));
        }

        [Fact]
        public async Task HDG20()
        {
            HasseDiagramGeneratorModel model = CreateCustomOutputModel();
            model.FontColour = "blue";
            model.StructuralLineColour = "green";
            model.StructuralLineWidth = 4m;
            model.PartialCrossingLineColour = "black";
            model.PartialCrossingLineWidth = 4m;
            model.SmallObjectFontSize = 3m;

            await AssertOutput(model, nameof(HDG20));
        }

        [Fact]
        public async Task HDG21()
        {
            HasseDiagramGeneratorModel model = CreateCustomOutputModel();
            model.FontColour = "blue";
            model.StructuralLineColour = "green";
            model.StructuralLineWidth = 4m;
            model.PartialCrossingLineColour = "black";
            model.PartialCrossingLineWidth = 4m;
            model.SmallObjectFontSize = 3m;
            model.MediumObjectFontSize = 3m;

            await AssertOutput(model, nameof(HDG21));
        }

        [Fact]
        public async Task HDG22()
        {
            HasseDiagramGeneratorModel model = CreateCustomOutputModel();
            model.FontColour = "blue";
            model.StructuralLineColour = "green";
            model.StructuralLineWidth = 4m;
            model.PartialCrossingLineColour = "black";
            model.PartialCrossingLineWidth = 4m;
            model.SmallObjectFontSize = 3m;
            model.MediumObjectFontSize = 3m;
            model.LargeObjectFontSize = 3m;

            await AssertOutput(model, nameof(HDG22));
        }

        private HasseDiagramGeneratorModel CreateModel()
        {
            KeyValuePair<int, string> dataset = _factory.SheetNames.SingleOrDefault(x => String.Equals(x.Value, "Hasse diagrams", System.StringComparison.OrdinalIgnoreCase));
            if (dataset.Equals(default(KeyValuePair<int, string>)))
            {
                throw new InvalidOperationException("The integration-test database does not contain the 'Hasse diagrams' dataset. Available datasets: " + String.Join(", ", _factory.SheetNames.Values));
            }

            return new HasseDiagramGeneratorModel
            {
                DatasetID = dataset.Key
            };
        }

        private HasseDiagramGeneratorModel CreateConfoundedDegreeOfFreedomModel()
        {
            HasseDiagramGeneratorModel model = CreateModel();
            model.FixedFactors = ConfoundedDegreeOfFreedomFactors;
            return model;
        }

        private HasseDiagramGeneratorModel CreateCustomOutputModel()
        {
            HasseDiagramGeneratorModel model = CreateConfoundedDegreeOfFreedomModel();
            model.CheckForConfoundedDegreesOfFreedom = false;
            model.ObjectColour = "red";
            return model;
        }

        private async Task<HttpResponseMessage> Post(HasseDiagramGeneratorModel model)
        {
            HttpClient client = _factory.CreateClient();
            return await client.PostAsync("Analyses/HasseDiagramGenerator", new FormUrlEncodedContent(model.ToKeyValue()));
        }

        private async Task AssertOutput(HasseDiagramGeneratorModel model, string testName)
        {
            HttpClient client = _factory.CreateClient();
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, ModuleName, new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput(ModuleName, model, testName, statsOutput);

            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", ModuleName, testName + ".html"));
            Assert.Equal(Helpers.SanitizeHtml(expectedHtml), Helpers.SanitizeHtml(statsOutput.HtmlResults));
        }
    }
}
