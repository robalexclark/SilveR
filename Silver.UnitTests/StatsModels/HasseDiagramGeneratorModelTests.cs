using SilveR.StatsModels;
using System.Collections.Generic;
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

            Assert.Equal("Treatment,Time Block Y blue Y Y Y red grey 2 orange 1.5 1 1 1", result);
        }
    }
}
