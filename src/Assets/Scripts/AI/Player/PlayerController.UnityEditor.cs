#if UNITY_EDITOR

public partial class PlayerController
{
  void OnDrawGizmos()
  {
    if (CurrentControlHandler != null)
    {
      CurrentControlHandler.DrawGizmos();
    }
  }
}

#endif
