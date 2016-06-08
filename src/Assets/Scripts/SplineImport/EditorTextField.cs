using UnityEngine;

public class EditorTextField : MonoBehaviour
{
  public string Text;

#if UNITY_EDITOR
  void OnDrawGizmos()
  {
    UnityEditor.Handles.Label(transform.position, Text);
  }
#endif

}
