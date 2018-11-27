using SilveR.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SilveR.Helpers
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
            //else if (value is int)
            //{
            //    int num = (int)value;
            //    newArgument.Value = num.ToString();
            //}
            else if (value is decimal)
            {
                decimal num = (decimal)value;
                newArgument.Value = num.ToString();
            }
            else
                throw new ArgumentException("Type not found!");

            return newArgument;
        }

        private IEnumerable<Argument> arguments;

        public ArgumentHelper(IEnumerable<Argument> arguments)
        {
            this.arguments = arguments;
        }

        public string LoadStringArgument(string targetName)
        {
            Argument arg = arguments.Single(x => x.Name == targetName);

            return arg.Value;
        }

        public bool LoadBooleanArgument(string targetName)
        {
            Argument arg = arguments.Single(x => x.Name == targetName);

            return Boolean.Parse(arg.Value);
        }

        public List<string> LoadIEnumerableArgument(string targetName)
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

        //public int LoadIntArgument(string targetName)
        //{
        //    Argument arg = arguments.Single(x => x.Name == targetName);
        //    return int.Parse(arg.Value);
        //}

        public decimal LoadDecimalArgument(string targetName)
        {
            Argument arg = arguments.Single(x => x.Name == targetName);
            return decimal.Parse(arg.Value);
        }

        public Nullable<decimal> LoadNullableDecimalArgument(string targetName)
        {
            Argument arg = arguments.Single(x => x.Name == targetName);

            if (arg.Value != null)
            {
                return decimal.Parse(arg.Value);
            }
            else
            {
                return null;
            }
        }
    }
}