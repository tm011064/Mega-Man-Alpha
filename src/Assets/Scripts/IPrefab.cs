
public interface IInstantiable
{
#if UNITY_EDITOR
  void Instantiate(InstantiationArguments arguments = null);
#endif
}