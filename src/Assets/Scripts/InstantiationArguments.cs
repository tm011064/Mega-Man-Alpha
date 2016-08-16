using System;
using System.Collections.Generic;
using UnityEngine;

public class InstantiationArguments
{
  public Bounds Bounds;

  public Vector2[] Vectors;

  public Dictionary<string, string> Arguments;

  public bool IsFlippedHorizontally;

  public bool IsFlippedVertically;

  public bool GetBool(string name)
  {
    if (!Arguments.ContainsKey(name))
    {
      throw new KeyNotFoundException("No key found with name '" + name + "'");
    }

    bool value;

    if (!bool.TryParse(Arguments[name], out value))
    {
      throw new ArgumentException("Unable to convert value '" + (Arguments[name] ?? "NULL") + "' to boolean");
    }

    return value;
  }

  public int GetInt(string name)
  {
    if (!Arguments.ContainsKey(name))
    {
      throw new KeyNotFoundException("No key found with name '" + name + "'");
    }

    int value;

    if (!int.TryParse(Arguments[name], out value))
    {
      throw new ArgumentException("Unable to convert value '" + (Arguments[name] ?? "NULL") + "' to integer");
    }

    return value;
  }
}
