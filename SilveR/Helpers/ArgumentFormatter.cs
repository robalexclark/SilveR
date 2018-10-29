using System;
using System.Collections.Generic;
using System.IO;

namespace SilveR.Helpers
{
    public class ArgumentFormatter
    {
        private Dictionary<string, string> charConversionList = new Dictionary<string, string>();

        public ArgumentFormatter()
        {
            string[] conversionLines = File.ReadAllLines(Path.Combine(Startup.ContentRootPath, "CharacterConversion.txt"));

            foreach (string s in conversionLines)
            {
                charConversionList.Add(s.Substring(0, 1), s.Substring(2));
            }
        }

        public string GetFormattedArgument(string stringValue, bool isVariable = false)
        {
            if (String.IsNullOrEmpty(stringValue))
            {
                stringValue = "NULL";
            }
            else
            {
                if (isVariable)
                {
                    stringValue = ConvertIllegalCharacters(stringValue);
                }

                stringValue = "\"" + stringValue + "\"";
            }

            return stringValue;
        }


        public string GetFormattedArgument(IEnumerable<string> listArguments)
        {
            if (listArguments == null) //6
            {
                return "NULL";
            }
            else
            {
                string formattedArgument = null;

                foreach (string item in listArguments)
                    formattedArgument = formattedArgument + "," + this.ConvertIllegalCharacters(item);

                return formattedArgument.TrimStart(',');
            }
        }


        public string ConvertIllegalCharacters(string stringValue)
        {
            foreach (KeyValuePair<string, string> kp in charConversionList)
            {
                if (stringValue.Contains(kp.Key))
                {
                    stringValue = stringValue.Replace(kp.Key, kp.Value);
                }
            }

            return stringValue;
        }

        public string GetFormattedArgument(bool value)
        {
            string val = value ? "Y" : "N";
            return val;
        }

        public string ConvertIVSCharactersBack(string theString)
        {
            foreach (KeyValuePair<string, string> kp in charConversionList)
            {
                if (theString.Contains(kp.Value))
                {
                    theString = theString.Replace(kp.Value, kp.Key);
                }
            }

            return theString;
        }
    }
}