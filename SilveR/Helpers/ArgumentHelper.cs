using System;
using System.Collections.Generic;
using System.Linq;
using SilveRModel.Models;

namespace SilveRModel.Helpers
{
    public class ArgumentHelper
    {
        public static Argument ArgumentFactory(string name, object value)
        {
            Argument newArgument = new Argument();
            newArgument.Name = name;
            if (value == null)
            {
                newArgument.Value = null;
            }
            else if (value is string)
            {
                string str = (string)value;
                newArgument.Value = str;
            }
            else if (value is List<string>)
            {
                List<string> strList = (List<string>)value;

                if (strList.Any())
                {
                    newArgument.Value = strList.Aggregate((a, b) => a + ',' + b);
                }
                else
                {
                    newArgument.Value = null;
                }
            }
            else if (value is bool)
            {
                bool boo = (bool)value;
                newArgument.Value = boo.ToString();
            }
            else if (value is int)
            {
                int num = (int)value;
                newArgument.Value = num.ToString();
            }
            else if (value is decimal)
            {
                decimal num = (decimal)value;
                newArgument.Value = num.ToString();
            }
            else throw new Exception("Type not found!");

            return newArgument;
        }

        private IEnumerable<Argument> arguments;

        public ArgumentHelper(IEnumerable<Argument> arguments)
        {
            this.arguments = arguments;
        }

        public string ArgumentLoader(string targetName, string target)
        {
            Argument arg = arguments.Single(x => x.Name == targetName);

            return arg.Value;
        }

        public bool ArgumentLoader(string targetName, bool target)
        {
            Argument arg = arguments.Single(x => x.Name == targetName);

            return Boolean.Parse(arg.Value);
        }

        public List<string> ArgumentLoader(string targetName, List<string> target)
        {
            Argument arg = arguments.Single(x => x.Name == targetName);

            if (arg.Value != null)
            {
                return arg.Value.Split(',').ToList();
            }
            else
            {
                return null;
            }
        }

        public int ArgumentLoader(string targetName, int target)
        {
            Argument arg = arguments.Single(x => x.Name == targetName);

            return int.Parse(arg.Value);
        }

        public decimal ArgumentLoader(string targetName, decimal target)
        {
            Argument arg = arguments.Single(x => x.Name == targetName);

            return decimal.Parse(arg.Value);
        }
    }
}