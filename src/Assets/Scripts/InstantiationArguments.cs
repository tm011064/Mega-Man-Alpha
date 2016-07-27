using System.Collections.Generic;
using UnityEngine;

public class InstantiationArguments
{
  public Bounds Bounds;

  public Dictionary<string, string> Arguments;

  public bool GetBool(string name)
  {
    return bool.Parse(Arguments[name]);
  }

  public int GetInt(string name)
  {
    return int.Parse(Arguments[name]);
  }
}
