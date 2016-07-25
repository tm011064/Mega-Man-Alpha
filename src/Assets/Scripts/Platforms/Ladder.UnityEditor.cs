#if UNITY_EDITOR

public partial class Ladder : IInstantiable
{
  public void Instantiate(InstantiationArguments arguments = null)
  {
    if (arguments != null)
    {
      Size = arguments.Bounds.size;

      transform.position = arguments.Bounds.center;
    }
  }
}

#endif
