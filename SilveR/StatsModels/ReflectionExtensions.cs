using System;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace SilveR.StatsModels
{
    public static class ReflectionExtensions
    {
        public static string GetPropertyDisplayName<T>(Expression<Func<T, object>> propertyExpression, bool keepPlural = false)
        {
            string displayName;

            MemberInfo memberInfo = GetPropertyInformation(propertyExpression.Body);
            if (memberInfo == null)
            {
                throw new ArgumentException("No property reference expression was found.", nameof(propertyExpression));
            }

            DisplayNameAttribute attr = memberInfo.GetAttribute<DisplayNameAttribute>(false);
            if (attr == null)
            {
                displayName = memberInfo.Name;
            }
            else
            {
                displayName = attr.DisplayName;
            }

            if (!keepPlural)
                displayName = displayName.TrimEnd('s');

            return displayName;
        }

        public static T GetAttribute<T>(this MemberInfo member, bool isRequired)
            where T : Attribute
        {
            object attribute = member.GetCustomAttributes(typeof(T), false).SingleOrDefault();

            if (attribute == null && isRequired)
            {
                throw new ArgumentException(String.Format(CultureInfo.InvariantCulture, "The {0} attribute must be defined on member {1}", typeof(T).Name, member.Name));
            }

            return (T)attribute;
        }

        public static MemberInfo GetPropertyInformation(Expression propertyExpression)
        {
            MemberExpression memberExpr = propertyExpression as MemberExpression;
            if (memberExpr == null)
            {
                UnaryExpression unaryExpr = propertyExpression as UnaryExpression;
                if (unaryExpr != null && unaryExpr.NodeType == ExpressionType.Convert)
                {
                    memberExpr = unaryExpr.Operand as MemberExpression;
                }
            }

            if (memberExpr != null && memberExpr.Member.MemberType == MemberTypes.Property)
            {
                return memberExpr.Member;
            }

            return null;
        }
    }
}