public interface IInstantiable<TInstantiationArgument>
  where TInstantiationArgument : InstantiationArguments
{
#if UNITY_EDITOR
  void Instantiate(TInstantiationArgument arguments);
#endif
}