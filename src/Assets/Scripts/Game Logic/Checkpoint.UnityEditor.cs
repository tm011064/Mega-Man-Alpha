#if UNITY_EDITOR

public partial class Checkpoint : IInstantiable<InstantiationArguments>
{
  public void Instantiate(InstantiationArguments arguments)
  {
    transform.position = arguments.Bounds.center;

    if (arguments.Properties.GetBool("Is Level Start"))
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