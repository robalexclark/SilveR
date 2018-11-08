//using SilveR;
//using System;
//using System.Collections.Generic;
//using System.IO;

//namespace SilveR.Helpers
//{
//    public static class ArgumentConverters
//    {
//        public static Dictionary<string, string> GetCharConversionList()
//        {
//            string[] conversionLines = File.ReadAllLines(Path.Combine(Startup.ContentRootPath, "CharacterConversion.txt"));

//            Dictionary<string, string> charConversionList = new Dictionary<string, string>();

//            foreach (string s in conversionLines)
//            {
//                charConversionList.Add(s.Substring(0, 1), s.Substring(2));
//            }

//            return charConversionList;
//        }

//        public static string ConvertIllegalChars(string stringValue)
//        {
//            Dictionary<string, string> charConversionList = GetCharConversionList();

//            if (String.IsNullOrEmpty(stringValue))
//                return stringValue;

//            foreach (KeyValuePair<string, string> kp in charConversionList)
//            {
//                if (stringValue.Contains(kp.Key))
//                {
//                    stringValue = stringValue.Replace(kp.Key, kp.Value);
//                }
//            }

//            return stringValue;
//        }

//        public static string ConvertIVSCharactersBack(string theString)
//        {
//            Dictionary<string, string> charConversionList = GetCharConversionList();

//            if (String.IsNullOrEmpty(theString))
//                return theString;

//            foreach (KeyValuePair<string, string> kp in charConversionList)
//            {
//                if (theString.Contains(kp.Value))
//                {
//                    theString = theString.Replace(kp.Value, kp.Key);
//                }
//            }

//            return theString;
//        }


//        public static string FormatStringArg(string txt)
//        {
//            if (String.IsNullOrEmpty(txt))
//            {
//                txt = "NULL";
//            }
//            else
//            {
//                txt = "\"" + txt + "\"";
//            }

//            return txt;
//        }

//        public static string GetYesOrNo(bool value)
//        {
//            string val = value ? "Y" : "N";
//            return val;
//        }
//    }
//}