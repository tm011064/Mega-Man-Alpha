#if UNITY_EDITOR

public partial class Ladder : IInstantiable
{
  public void Instantiate(InstantiationArguments arguments)
  {
    if (arguments != null)
    {
      Size = arguments.Bounds.size;

      transform.position = arguments.Bounds.center;
    }
  }
}

#endif
