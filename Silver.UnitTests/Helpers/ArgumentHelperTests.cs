using SilveR.Helpers;
using SilveR.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Xunit;

namespace Silver.UnitTests
{
    [ExcludeFromCodeCoverageAttribute]
    public class ArgumentHelperTests
    {
        [Fact]
        public void LoadStringArgument_ReturnsCorrectString()
        {
            //Arrange
            ArgumentHelper sut = new ArgumentHelper(GetArguments());

            //Act
            string result = sut.LoadStringArgument("Response");

            //Assert
            Assert.Equal("Resp 1", result);
        }

        [Fact]
        public void LoadBooleanArgument_ReturnsCorrectBool()
        {
            //Arrange
            ArgumentHelper sut = new ArgumentHelper(GetArguments());

            //Act
            bool result = sut.LoadBooleanArgument("ANOVASelected");

            //Assert
            Assert.True(result);
        }

        [Fact]
        public void LoadIEnumerableArgument_ReturnsCorrectIEnumerable()
        {
            //Arrange
            ArgumentHelper sut = new ArgumentHelper(GetArguments());

            //Act
            IEnumerable<string> result = sut.LoadIEnumerableArgument("Treatments");

            //Assert
            Assert.Equal(new List<string> { "Treat1", "Treat 2", "Treat 3" }, result);
        }

        [Fact]
        public void LoadDecimalArgument_ReturnsCorrectDecimal()
        {
            //Arrange
            ArgumentHelper sut = new ArgumentHelper(GetArguments());

            //Act
            decimal result = sut.LoadDecimalArgument("Significance");

            //Assert
            Assert.Equal(0.05m, result);
        }

        [Fact]
        public void LoadNullableDecimalArgument_ReturnsCorrectDecimal()
        {
            //Arrange
            ArgumentHelper sut = new ArgumentHelper(GetArguments());

            //Act
            Nullable<decimal> result = sut.LoadNullableDecimalArgument("Slope");

            //Assert
            Assert.Equal(1.23m, result);
        }

        [Fact]
        public void LoadDecimalArgument_ReturnsNull()
        {
            //Arrange
            ArgumentHelper sut = new ArgumentHelper(GetArguments());

            //Act
            Nullable<decimal> result = sut.LoadNullableDecimalArgument("Origin");

            //Assert
            Assert.Null(result);
        }


        private List<Argument> GetArguments()
        {
            List<Argument> arguments = new List<Argument>();
            arguments.Add(new Argument() { Name = "Response", Value = "Resp 1" });
            arguments.Add(new Argument() { Name = "Treatments", Value = "Treat1,Treat 2,Treat 3" });
            arguments.Add(new Argument() { Name = "ANOVASelected", Value = "True" });
            arguments.Add(new Argument() { Name = "Significance", Value = "0.05" });
            arguments.Add(new Argument() { Name = "Slope", Value = "1.23" });
            arguments.Add(new Argument() { Name = "Origin" });

            return arguments;
        }
    }
}