using System.Collections.Generic;
using UnityEngine;

public class CameraModifierInstantiationArguments : InstantiationArguments
{
  public Line2PropertyInfo[] Line2PropertyInfos;

  public BoundsPropertyInfo[] BoundsPropertyInfos;

  public class Line2PropertyInfo
  {
    public Line2 Line;

    public Dictionary<string, string> Properties;
  }

  public class BoundsPropertyInfo
  {
    public Bounds Bounds;

    public Dictionary<string, string> Properties;
  }
}
