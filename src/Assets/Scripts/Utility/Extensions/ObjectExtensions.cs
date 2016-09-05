using System;
using System.Reflection;
using System.Text;

public static class ObjectExtensions
{
  private const string EQUALS_SIGN = ": ";

  private static readonly string SEPARATOR = Environment.NewLine;

  public static string GetFieldValuesFormatted(this object obj, BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.Public)
  {
    var memberNames = new StringBuilder();

    var type = obj.GetType();

    foreach (var field in type.GetFields(bindingFlags))
    {
      memberNames.Append(SEPARATOR);
      memberNames.Append(field.Name);
      memberNames.Append(EQUALS_SIGN);
      memberNames.Append(field.GetValue(obj));
    }

    return memberNames.Length == 0
      ? "NO FIELDS FOUND"
      : memberNames.ToString();
  }
}
