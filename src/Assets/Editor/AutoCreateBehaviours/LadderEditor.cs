using UnityEditor;

namespace Assets.Editor.AutoCreateBehaviours
{
  [CustomEditor(typeof(Ladder))]
  public class LadderEditor : UnityEditor.Editor
  {
    public override void OnInspectorGUI()
    {
      DrawDefaultInspector();

      var script = (Ladder)target;

      script.Instantiate(null);
    }
  }
}
