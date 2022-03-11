using SilveR.Helpers;
using System;
using System.Data;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using Xunit;

namespace SilveR.UnitTests.Helpers
{
    public class CSVConverterTests
    {
        [Fact]
        public void ConvertToDataTable_GBCulture_ReturnsCorrectDataTable()
        {
            //Arrange
            System.Threading.Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
            string theData = GetTestCSVData();
            byte[] byteArray = Encoding.UTF8.GetBytes(theData);

            using (MemoryStream stream = new MemoryStream(byteArray))
            {
                //Act
                DataTable dataTable = CSVConverter.CSVDataToDataTable(stream, new System.Globalization.CultureInfo("en-GB"));

                //Assert
                Assert.Equal("SilveRSelected", dataTable.Columns[0].ColumnName);
                Assert.Equal("Resp1", dataTable.Columns[1].ColumnName);
                Assert.Equal("Trea t1", dataTable.Columns[2].ColumnName);

                Assert.True((bool)dataTable.Rows[0][0]);
                Assert.Equal("0.998758912", dataTable.Rows[0][1]);
                Assert.Equal(String.Empty, dataTable.Rows[10][4]);
                Assert.Equal("C", dataTable.Rows[10][5]);
            }
        }

        [Fact]
        public void ConvertToDataTable_DECulture__ReturnsCorrectDataTable()
        {
            //Arrange
            System.Threading.Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
            string theData = GetTestCSVData();

            //change the separator
            //if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            //{
                theData = theData.Replace(',', ';'); //list separator for De on windows
                theData = theData.Replace('.', ','); //points replaced with commas
            //}
            //else
            //{
            //    theData = theData.Replace(',', ';'); //list separator for De on windows
            //    theData = theData.Replace('.', ','); //points replaced with commas

            //    theData = theData.Replace(';', '.'); //list separator for De on linux
            //}


            byte[] byteArray = Encoding.UTF8.GetBytes(theData);

            using (MemoryStream stream = new MemoryStream(byteArray))
            {
                //Act
                DataTable dataTable = CSVConverter.CSVDataToDataTable(stream, new System.Globalization.CultureInfo("de-DE"));

                //Assert
                Assert.Equal("SilveRSelected", dataTable.Columns[0].ColumnName);
                Assert.Equal("Resp1", dataTable.Columns[1].ColumnName);
                Assert.Equal("Trea t1", dataTable.Columns[2].ColumnName);

                Assert.True((bool)dataTable.Rows[0][0]);
                Assert.Equal("0,998758912", dataTable.Rows[0][1]);
                Assert.Equal(String.Empty, dataTable.Rows[10][4]);
                Assert.Equal("C", dataTable.Rows[10][5]);
            }
        }

        [Fact]
        public void ConvertToDataTable_FRCulture__ReturnsCorrectDataTable()
        {
            //Arrange
            System.Threading.Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
            string theData = GetTestCSVData();

            //change the separator
            //if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            //{
                theData = theData.Replace(',', ';'); //list separator for De on windows
                theData = theData.Replace('.', ','); //points replaced with commas
            //}
            //else
            //{
            //    theData = theData.Replace(',', ';'); //list separator for De on windows
            //    theData = theData.Replace('.', ','); //points replaced with commas

            //    theData = theData.Replace(';', '.'); //list separator for De on linux
            //}


            byte[] byteArray = Encoding.UTF8.GetBytes(theData);

            using (MemoryStream stream = new MemoryStream(byteArray))
            {
                //Act
                DataTable dataTable = CSVConverter.CSVDataToDataTable(stream, new System.Globalization.CultureInfo("fr-FR"));

                //Assert
                Assert.Equal("SilveRSelected", dataTable.Columns[0].ColumnName);
                Assert.Equal("Resp1", dataTable.Columns[1].ColumnName);
                Assert.Equal("Trea t1", dataTable.Columns[2].ColumnName);

                Assert.True((bool)dataTable.Rows[0][0]);
                Assert.Equal("0,998758912", dataTable.Rows[0][1]);
                Assert.Equal(String.Empty, dataTable.Rows[10][4]);
                Assert.Equal("C", dataTable.Rows[10][5]);
            }
        }


        private string GetTestCSVData()
        {
            return "SilveRSelected,Resp1,Trea t1,Resp2,Treat2,Treat3,Treat4,Resp3,Treat5" + Environment.NewLine +
                    "True,0.998758912,x,1,x,A,A,missing,@" + Environment.NewLine +
                    "True,0.911332819,x,,,B,A,0.834162092,@" + Environment.NewLine +
                    "True,0.410939792,x,,,B,A,0.846718545,@" + Environment.NewLine +
                    "True,0.051020205,x,,,B,A,0.356952998,@" + Environment.NewLine +
                    "True,0.280761538,x,,,B,B,0.830276512,x" + Environment.NewLine +
                    "True,0.57038827,x,,,B,B,0.681256384,x" + Environment.NewLine +
                    "True,0.994523985,x,,,B,B,0.608030743,x" + Environment.NewLine +
                    "True,0.431388442,x,,,B,B,0.602626688,x" + Environment.NewLine +
                    "True,0.877216432,y,,,C,C,0.700375858,y" + Environment.NewLine +
                    "True,0.353903886,y,,,C,C,0.570765609,y" + Environment.NewLine +
                    "True,0.901387615,y,,,C,C,0.085058491,y" + Environment.NewLine +
                    "True,0.739097386,y,,,C,C,0.323681268,y" + Environment.NewLine +
                    "True,0.886753925,y,,,D,D,0.968650072,y" + Environment.NewLine +
                    "True,0.021624508,y,,,D,D,0.633108372,y" + Environment.NewLine +
                    "True,0.585623196,y,,,D,D,0.496530066,y" + Environment.NewLine +
                    "True,0.616738341,y,,,D,D,0.434506181,y" + Environment.NewLine +
                    "True,0.198576735,y,,,D,D,0.881195721,y" + Environment.NewLine +
                    "True,0.43343727,y,,,D,,0.647456457,y";
        }
    }
}