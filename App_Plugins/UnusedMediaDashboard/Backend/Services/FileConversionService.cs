using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace UnusedMediaDashboard.Services
{
    public static class FileConversionService   //This whole class was yoinked from StackOverflow. Only minor changes made.
    {
        public static string Serialize<T>(IEnumerable<T> objects)
        {
            var stringBuilder = new StringBuilder();
            TextWriter output = new StringWriter(stringBuilder);
            var fields =
                from mi in typeof(T).GetMembers(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static)
                where new[] { MemberTypes.Field, MemberTypes.Property }.Contains(mi.MemberType)
                let orderAttr = (ColumnOrderAttribute)Attribute.GetCustomAttribute(mi, typeof(ColumnOrderAttribute))
                orderby orderAttr == null ? int.MaxValue : orderAttr.Order, mi.Name
                select mi;
            output.WriteLine(QuoteRecord(fields.Select(f => f.Name)));
            foreach (var record in objects)
            {
                output.WriteLine(QuoteRecord(FormatObject(fields, record)));
            }

            return stringBuilder.ToString();
        }

        private static IEnumerable<string> FormatObject<T>(IEnumerable<MemberInfo> fields, T record)
        {
            foreach (var field in fields)
            {
                if (field is FieldInfo)
                {
                    var fi = (FieldInfo)field;
                    yield return Convert.ToString(fi.GetValue(record));
                }
                else if (field is PropertyInfo)
                {
                    var pi = (PropertyInfo)field;
                    yield return Convert.ToString(pi.GetValue(record, null));
                }
                else
                {
                    throw new Exception("Unhandled case.");
                }
            }
        }

        const string CsvSeparator = ",";

        static string QuoteRecord(IEnumerable<string> record)
        {
            return String.Join(CsvSeparator, record.Select(field => QuoteField(field)).ToArray());
        }

        static string QuoteField(string field)
        {
            if (String.IsNullOrEmpty(field))
            {
                return "\"\"";
            }
            else if (field.Contains(CsvSeparator) || field.Contains("\"") || field.Contains("\r") || field.Contains("\n"))
            {
                return String.Format("\"{0}\"", field.Replace("\"", "\"\""));
            }
            else
            {
                return field;
            }
        }
    }

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class ColumnOrderAttribute : Attribute
    {
        public int Order { get; private set; }
        public ColumnOrderAttribute(int order) { Order = order; }
    }
}
