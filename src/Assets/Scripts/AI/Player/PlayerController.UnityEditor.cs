#if UNITY_EDITOR

public partial class PlayerController
{
  void OnDrawGizmos()
  {
    if (ActiveControlHandler != null)
    {
      ActiveControlHandler.DrawGizmos();
    }
  }
}

#endif
