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
            else
            {
                switch (value)
                {
                    case string str:
                        newArgument.Value = str;

                        break;
                    case List<string> strList:
                        if (strList.Any())
                        {
                            newArgument.Value = strList.Aggregate((a, b) => a + ',' + b);
                        }
                        else
                        {
                            newArgument.Value = null;
                        }

                        break;
                    case bool boo:
                        newArgument.Value = boo.ToString();

                        break;
                    case int num:
                        newArgument.Value = num.ToString();

                        break;
                    case decimal dec:
                        newArgument.Value = dec.ToString();

                        break;
                    default:
                        throw new ArgumentException("Type not found!");
                }
            }

            return newArgument;
        }

        private readonly IEnumerable<Argument> arguments;

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

        public int LoadIntArgument(string targetName)
        {
            Argument arg = arguments.Single(x => x.Name == targetName);
            return int.Parse(arg.Value);
        }

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

        public Nullable<int> LoadNullableIntArgument(string targetName)
        {
            Argument arg = arguments.Single(x => x.Name == targetName);

            if (arg.Value != null)
            {
                return int.Parse(arg.Value);
            }
            else
            {
                return null;
            }
        }
    }
}