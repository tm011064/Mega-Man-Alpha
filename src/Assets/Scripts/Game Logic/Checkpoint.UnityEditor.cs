#if UNITY_EDITOR

public partial class Checkpoint : IInstantiable
{
  public void Instantiate(InstantiationArguments arguments)
  {
    transform.position = arguments.Bounds.center;

    if (arguments.GetBool("Is Level Start"))
    {
      Index = 0;
    }
    else
    {
      Index = 1;
    }
  }
}

#endif