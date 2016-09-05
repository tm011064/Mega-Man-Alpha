using System;

public static class EnumExtensions
{
#if UNITY_EDITOR
  public static string ToMultipleStateString<T>(this T self)
  {
    throw new NotImplementedException();
  }
#endif
}

