using SilveR.Helpers;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Xunit;

namespace Silver.UnitTests.Helpers
{

    public class ArgumentFormatterTests
    {
        [Fact]
        public void ConvertIllegalCharacters_StringWithAllIllegalChars_ReturnsCorrectString()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                //Arrange
                ArgumentFormatter sut = new ArgumentFormatter();

                //Act
                string nastyString = " ()/%#.-@:!\"£$^&{};|\\[]=<>'";
                string result = sut.ConvertIllegalCharacters(nastyString);

                //Assert
                Assert.Equal("ivs_sp_ivsivs_ob_ivsivs_cb_ivsivs_div_ivsivs_pc_ivsivs_hash_ivsivs_pt_ivsivs_hyphen_ivsivs_at_ivsivs_colon_ivsivs_exclam_ivsivs_dblquote_ivsivs_pound_ivsivs_dollar_ivsivs_hat_ivsivs_amper_ivsivs_obrace_ivsivs_cbrace_ivsivs_semi_ivsivs_pipe_ivsivs_slash_ivsivs_osb_ivsivs_csb_ivsivs_eq_ivsivs_lt_ivsivs_gt_ivsivs_quote_ivs", result);
            }
        }

        [Fact]
        public void GetFormattedArgument_StringIsNull_ReturnsNULL()
        {
            //Arrange
            ArgumentFormatter sut = new ArgumentFormatter();

            //Act
            string result = sut.GetFormattedArgument(null, false);

            //Assert
            Assert.Equal("NULL", result);
        }

        [Fact]
        public void GetFormattedArgument_SimpleStringValue_ReturnsCorrectString()
        {
            //Arrange
            ArgumentFormatter sut = new ArgumentFormatter();

            //Act
            string result = sut.GetFormattedArgument("TestString", true);

            //Assert
            Assert.Equal("TestString", result);
        }

        [Fact]
        public void GetFormattedArgument_ListIsNull_ReturnsNULL()
        {
            //Arrange
            ArgumentFormatter sut = new ArgumentFormatter();

            //Act
            IEnumerable<string> nullList = null;
            string result = sut.GetFormattedArgument(nullList);

            //Assert
            Assert.Equal("NULL", result);
        }

        [Fact]
        public void GetFormattedArgument_ListValue_ReturnsCorrectString()
        {
            //Arrange
            ArgumentFormatter sut = new ArgumentFormatter();

            //Act
            IEnumerable<string> list = new List<string> { "TestString1", "TestString2" };
            string result = sut.GetFormattedArgument(list);

            //Assert
            Assert.Equal("TestString1,TestString2", result);
        }

        [Fact]
        public void GetFormattedArgument_StringWithSpacesIsVariableTrue_ReturnsCorrectString()
        {
            //Arrange
            ArgumentFormatter sut = new ArgumentFormatter();

            //Act
            string result = sut.GetFormattedArgument("Test String", true);

            //Assert
            Assert.Equal("Testivs_sp_ivsString", result);
        }

        [Fact]
        public void GetFormattedArgument_StringWithSpacesIsVariableFalse_ReturnsCorrectString()
        {
            //Arrange
            ArgumentFormatter sut = new ArgumentFormatter();

            //Act
            string result = sut.GetFormattedArgument("Test String", false);

            //Assert
            Assert.Equal("\"Test String\"", result);
        }


        [Fact]
        public void GetFormattedArgument_StringWithIllegalChars_ReturnsCorrectString()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                //Arrange
                ArgumentFormatter sut = new ArgumentFormatter();

                //Act
                string result = sut.GetFormattedArgument(" ()/%#.-@:!\"£$^&{};|\\[]=<>'", true);

                //Assert
                Assert.Equal("ivs_sp_ivsivs_ob_ivsivs_cb_ivsivs_div_ivsivs_pc_ivsivs_hash_ivsivs_pt_ivsivs_hyphen_ivsivs_at_ivsivs_colon_ivsivs_exclam_ivsivs_dblquote_ivsivs_pound_ivsivs_dollar_ivsivs_hat_ivsivs_amper_ivsivs_obrace_ivsivs_cbrace_ivsivs_semi_ivsivs_pipe_ivsivs_slash_ivsivs_osb_ivsivs_csb_ivsivs_eq_ivsivs_lt_ivsivs_gt_ivsivs_quote_ivs", result);
            }
        }

        [Fact]
        public void GetFormattedArgument_True_ReturnsY()
        {
            //Arrange
            ArgumentFormatter sut = new ArgumentFormatter();

            //Act
            string result = sut.GetFormattedArgument(true);

            //Assert
            Assert.Equal("Y", result);
        }

        [Fact]
        public void GetFormattedArgument_False_ReturnsY()
        {
            //Arrange
            ArgumentFormatter sut = new ArgumentFormatter();

            //Act
            string result = sut.GetFormattedArgument(false);

            //Assert
            Assert.Equal("N", result);
        }

        [Fact]
        public void ConvertIllegalCharactersBack_StringWithAllIllegalChars_ReturnsCorrectString()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                //Arrange
                ArgumentFormatter sut = new ArgumentFormatter();

                //Act
                string formattedString = "ivs_sp_ivsivs_ob_ivsivs_cb_ivsivs_div_ivsivs_pc_ivsivs_hash_ivsivs_pt_ivsivs_hyphen_ivsivs_at_ivsivs_colon_ivsivs_exclam_ivsivs_dblquote_ivsivs_pound_ivsivs_dollar_ivsivs_hat_ivsivs_amper_ivsivs_obrace_ivsivs_cbrace_ivsivs_semi_ivsivs_pipe_ivsivs_slash_ivsivs_osb_ivsivs_csb_ivsivs_eq_ivsivs_lt_ivsivs_gt_ivsivs_quote_ivs";
                string result = sut.ConvertIllegalCharactersBack(formattedString);

                //Assert
                Assert.Equal(" ()/%#.-@:!\"£$^&{};|\\[]=<>'", result);
            }
        }
    }
}