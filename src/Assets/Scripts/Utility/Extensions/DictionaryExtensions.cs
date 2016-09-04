using System;
using System.Collections.Generic;

public static class DictionaryExtensions
{
  public static bool GetBoolSafe(this Dictionary<string, string> self, string name, bool defaultValue)
  {
    if (!self.ContainsKey(name))
    {
      return defaultValue;
    }

    bool value;

    if (!bool.TryParse(self[name], out value))
    {
      return defaultValue;
    }

    return value;
  }

  public static bool GetBool(this Dictionary<string, string> self, string name)
  {
    if (!self.ContainsKey(name))
    {
      throw new KeyNotFoundException("No key found with name '" + name + "'");
    }

    bool value;

    if (!bool.TryParse(self[name], out value))
    {
      throw new ArgumentException("Unable to convert value '" + (self[name] ?? "NULL") + "' to boolean");
    }

    return value;
  }

  public static int GetInt(this Dictionary<string, string> self, string name)
  {
    if (!self.ContainsKey(name))
    {
      throw new KeyNotFoundException("No key found with name '" + name + "'");
    }

    int value;

    if (!int.TryParse(self[name], out value))
    {
      throw new ArgumentException("Unable to convert value '" + (self[name] ?? "NULL") + "' to boolean");
    }

    return value;
  }
}

