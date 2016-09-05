using System.Collections.Generic;
using UnityEngine;

public class CameraModifierInstantiationArguments : InstantiationArguments
{
  public BoundsPropertyInfo[] BoundsPropertyInfos;

  public class BoundsPropertyInfo
  {
    public Bounds Bounds;

    public Dictionary<string, string> Properties;
  }
}
