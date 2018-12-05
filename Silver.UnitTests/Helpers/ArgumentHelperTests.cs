using SilveR.Helpers;
using SilveR.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Xunit;

namespace Silver.UnitTests.Helpers
{
    
    public class ArgumentHelperTests
    {
        [Fact]
        public void ArgumentFactory_NullString_ReturnsNull()
        {
            //Arrange,Act
            Argument sut = ArgumentHelper.ArgumentFactory("TestName", null);

            //Assert
            Assert.Equal("TestName", sut.Name);
            Assert.Null(sut.Value);
        }

        [Fact]
        public void ArgumentFactory_String_ReturnsString()
        {
            //Arrange,Act
            Argument sut = ArgumentHelper.ArgumentFactory("TestName", "TestValue");

            //Assert
            Assert.Equal("TestName", sut.Name);
            Assert.Equal("TestValue", sut.Value);
        }


        [Fact]
        public void ArgumentFactory_StringList_ReturnsString()
        {
            //Arrange,Act
            Argument sut = ArgumentHelper.ArgumentFactory("TestName", new List<string> { "TestValue1", "TestValue2", "TestValue3" });

            //Assert
            Assert.Equal("TestName", sut.Name);
            Assert.Equal("TestValue1,TestValue2,TestValue3", sut.Value);
        }

        [Fact]
        public void ArgumentFactory_EmptyList_ReturnsNull()
        {
            //Arrange,Act
            Argument sut = ArgumentHelper.ArgumentFactory("TestName", new List<string>());

            //Assert
            Assert.Equal("TestName", sut.Name);
            Assert.Null(sut.Value);
        }

        [Fact]
        public void ArgumentFactory_Bool_ReturnsString()
        {
            //Arrange,Act
            Argument sut = ArgumentHelper.ArgumentFactory("TestName", true);

            //Assert
            Assert.Equal("TestName", sut.Name);
            Assert.Equal("True", sut.Value);
        }

        [Fact]
        public void ArgumentFactory_Decimal_ReturnsString()
        {
            //Arrange,Act
            Argument sut = ArgumentHelper.ArgumentFactory("TestName", 1.23M);

            //Assert
            Assert.Equal("TestName", sut.Name);
            Assert.Equal("1.23", sut.Value);
        }

        [Fact]
        public void ArgumentFactory_Int_ReturnsString()
        {
            //Arrange,Act
            Argument sut = ArgumentHelper.ArgumentFactory("TestName", 123);

            //Assert
            Assert.Equal("TestName", sut.Name);
            Assert.Equal("123", sut.Value);
        }

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
        public void LoadIEnumerableArgument_IsNull_ReturnsNull()
        {
            //Arrange
            var arguments = GetArguments();
            arguments.Single(x => x.Name == "Treatments").Value = null;
            ArgumentHelper sut = new ArgumentHelper(arguments);

            //Act
            IEnumerable<string> result = sut.LoadIEnumerableArgument("Treatments");

            //Assert
            Assert.Null(result);
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