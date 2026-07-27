using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace SilveR.Helpers
{
    public class ArgumentFormatter
    {
        private const string Version2Prefix = "ivs2_";
        private const string Version2Suffix = "_ivs";
        private static readonly Regex Version2IdentifierRegex = new Regex(
            @"ivs2_(?<length>[0-9]+)_(?<hex>[0-9A-F]+)_ivs",
            RegexOptions.Compiled | RegexOptions.CultureInvariant);
        private readonly Dictionary<string, string> charConversionList = new Dictionary<string, string>();

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
            charConversionList.Add("?", "ivs_questionmark_ivs");
            charConversionList.Add(",", "ivs_comma_ivs");
        }

        public string GetFormattedArgument(int value)
        {
            return GetFormattedArgument(value.ToString(), false);
        }


        public string GetFormattedArgument(Nullable<int> value)
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
                return "NULL";
            }
            else
            {
                StringBuilder formattedArgument = new StringBuilder();

                foreach (string item in listArguments)
                {
                    formattedArgument.Append("," + this.ConvertIllegalCharacters(item));
                }

                return formattedArgument.ToString().TrimStart(',');
            }
        }

        public string ConvertIllegalCharacters(string stringValue)
        {
            ArgumentNullException.ThrowIfNull(stringValue);

            string normalizedValue = stringValue.Normalize(NormalizationForm.FormC);
            bool escapeUnderscores = normalizedValue.Contains("ivs_", StringComparison.Ordinal)
                || normalizedValue.Contains(Version2Prefix, StringComparison.Ordinal);
            StringBuilder encodedValue = new StringBuilder();

            foreach (Rune rune in normalizedValue.EnumerateRunes())
            {
                string runeValue = rune.ToString();
                bool requiresEncoding = rune.Value > 0x7F
                    || (rune.Value == '_' && escapeUnderscores);
                if (!requiresEncoding)
                {
                    if (charConversionList.TryGetValue(runeValue, out string legacyEncoding))
                    {
                        encodedValue.Append(legacyEncoding);
                    }
                    else
                    {
                        encodedValue.Append(runeValue);
                    }
                    continue;
                }

                byte[] utf8Bytes = new UTF8Encoding(false, true).GetBytes(runeValue);
                encodedValue.Append(Version2Prefix);
                encodedValue.Append(utf8Bytes.Length.ToString(CultureInfo.InvariantCulture));
                encodedValue.Append('_');
                encodedValue.Append(Convert.ToHexString(utf8Bytes));
                encodedValue.Append(Version2Suffix);
            }

            return encodedValue.ToString();
        }

        public string ConvertCsvHeader(string csvHeader)
        {
            ArgumentNullException.ThrowIfNull(csvHeader);

            List<string> fields = new List<string>();
            StringBuilder field = new StringBuilder();
            bool insideQuotes = false;

            for (int index = 0; index < csvHeader.Length; index++)
            {
                char character = csvHeader[index];
                if (character == '"')
                {
                    if (insideQuotes && index + 1 < csvHeader.Length && csvHeader[index + 1] == '"')
                    {
                        field.Append('"');
                        index++;
                    }
                    else
                    {
                        insideQuotes = !insideQuotes;
                    }
                }
                else if (character == ',' && !insideQuotes)
                {
                    fields.Add(field.ToString());
                    field.Clear();
                }
                else
                {
                    field.Append(character);
                }
            }

            if (insideQuotes)
            {
                throw new FormatException("The CSV header contains an unterminated quoted field.");
            }

            fields.Add(field.ToString());
            return String.Join(",", fields.Select(ConvertIllegalCharacters));
        }

        public string GetFormattedArgument(bool value)
        {
            string val = value ? "Y" : "N";
            return val;
        }


        public string ConvertIllegalCharactersBack(string theString)
        {
            return ConvertIllegalCharactersBack(theString, value => value);
        }

        internal string ConvertIllegalCharactersBack(string theString, Func<string, string> encodeReplacement)
        {
            ArgumentNullException.ThrowIfNull(theString);
            ArgumentNullException.ThrowIfNull(encodeReplacement);

            // Version 1 identifiers can occur in saved analyses and historical output.
            foreach (KeyValuePair<string, string> kp in charConversionList)
            {
                if (theString.Contains(kp.Value))
                {
                    theString = theString.Replace(kp.Value, encodeReplacement(kp.Key));
                }
            }

            return Version2IdentifierRegex.Replace(theString, match =>
            {
                string hex = match.Groups["hex"].Value;
                if (!Int32.TryParse(match.Groups["length"].Value, NumberStyles.None, CultureInfo.InvariantCulture, out int expectedLength)
                    || hex.Length != expectedLength * 2)
                {
                    return match.Value;
                }

                try
                {
                    string decodedValue = new UTF8Encoding(false, true).GetString(Convert.FromHexString(hex));
                    return encodeReplacement(decodedValue);
                }
                catch (DecoderFallbackException)
                {
                    return match.Value;
                }
                catch (ArgumentException)
                {
                    return match.Value;
                }
            });
        }

    }
}
