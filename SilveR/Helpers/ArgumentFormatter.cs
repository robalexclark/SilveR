using System;
using System.Collections.Generic;

namespace SilveR.Helpers
{
    public class ArgumentFormatter
    {
        private Dictionary<string, string> charConversionList = new Dictionary<string, string>();

        public ArgumentFormatter()
        {
            charConversionList.Add(" ", "ivs_sp_ivs");
            charConversionList.Add("(", "ivs_ob_ivs");
            charConversionList.Add(")", "ivs_cb_ivs");
            charConversionList.Add("/", "ivs_div_ivs");
            charConversionList.Add("%", "ivs_pc_ivs");
            charConversionList.Add("#", "ivs_hash_ivs");
            charConversionList.Add(".", "ivs_pt_ivs");
            charConversionList.Add("-", "ivs_hyphen_ivs");
            charConversionList.Add("@", "ivs_at_ivs");
            charConversionList.Add(":", "ivs_colon_ivs");
            charConversionList.Add("!", "ivs_exclam_ivs");
            charConversionList.Add("\"", "ivs_dblquote_ivs");
            charConversionList.Add("£", "ivs_pound_ivs");
            charConversionList.Add("$", "ivs_dollar_ivs");
            charConversionList.Add("^", "ivs_hat_ivs");
            charConversionList.Add("&", "ivs_amper_ivs");
            charConversionList.Add("{", "ivs_obrace_ivs");
            charConversionList.Add("}", "ivs_cbrace_ivs");
            charConversionList.Add(";", "ivs_semi_ivs");
            charConversionList.Add("|", "ivs_pipe_ivs");
            charConversionList.Add("\\", "ivs_slash_ivs");
            charConversionList.Add("[", "ivs_osb_ivs");
            charConversionList.Add("]", "ivs_csb_ivs");
            charConversionList.Add("=", "ivs_eq_ivs");
            charConversionList.Add("<", "ivs_lt_ivs");
            charConversionList.Add(">", "ivs_gt_ivs");
            charConversionList.Add("'", "ivs_quote_ivs");
        }

        public string GetFormattedArgument(Nullable<int> value)
        {
            if (!value.HasValue)
            {
                return  "NULL";
            }
            else
            {
                return GetFormattedArgument(value.ToString(), false);
            }
        }

        public string GetFormattedArgument(Nullable<decimal> value)
        {
            if (!value.HasValue)
            {
                return "NULL";
            }
            else
            {
                return GetFormattedArgument(value.ToString(), false);
            }
        }

        public string GetFormattedArgument(string stringValue, bool isVariable)
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

                if (stringValue.Contains(' ')) //then contains a space so wrap in quotes
                {
                    stringValue = "\"" + stringValue + "\"";
                }
            }

            return stringValue;
        }

        public string GetFormattedArgument(IEnumerable<string> listArguments)
        {
            if (listArguments == null)
            {
                //throw new NullReferenceException("listArguments can't be null!");
                return "NULL";
            }
            else
            {
                string formattedArgument = null;

                foreach (string item in listArguments)
                {
                    formattedArgument = formattedArgument + "," + this.ConvertIllegalCharacters(item);
                }

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


        public string ConvertIllegalCharactersBack(string theString)
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