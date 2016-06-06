using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditorInternal;
using System.Reflection;

#endif

[CustomEditor(typeof(LineRenderer))]
[CanEditMultipleObjects]
public class LineRendererSortingLayersEditor : Editor
{
  public override void OnInspectorGUI()
  {
    base.OnInspectorGUI();

    serializedObject.Update();

    SerializedProperty sortingLayerID = serializedObject.FindProperty("m_SortingLayerID");
    SerializedProperty sortingOrder = serializedObject.FindProperty("m_SortingOrder");

    var firstHoriz = EditorGUILayout.BeginHorizontal();

    EditorGUI.BeginChangeCheck();

    EditorGUI.BeginProperty(firstHoriz, GUIContent.none, sortingLayerID);

    var layerNames = GetSortingLayerNames();

    var layerID = GetSortingLayerUniqueIDs();

    var selected = -1;

    var id = sortingLayerID.intValue;

    for (var i = 0; i < layerID.Length; i++)
    {
      if (id == layerID[i])
      {
        selected = i;
      }
    }

    if (selected == -1)
    {
      for (var i = 0; i < layerID.Length; i++)
      {
        if (layerID[i] == 0)
        {
          selected = i;
        }
      }
    }

    selected = EditorGUILayout.Popup("Sorting Layer", selected, layerNames);

    sortingLayerID.intValue = layerID[selected];

    EditorGUI.EndProperty();

    EditorGUILayout.EndHorizontal();

    EditorGUILayout.BeginHorizontal();

    EditorGUI.BeginChangeCheck();

    EditorGUILayout.PropertyField(sortingOrder, new GUIContent("Order in Layer"));

    EditorGUILayout.EndHorizontal();

    serializedObject.ApplyModifiedProperties();
  }

  public string[] GetSortingLayerNames()
  {
    var internalEditorUtilityType = typeof(InternalEditorUtility);

    var sortingLayersProperty =
      internalEditorUtilityType.GetProperty("sortingLayerNames", BindingFlags.Static | BindingFlags.NonPublic);

    return (string[])sortingLayersProperty.GetValue(null, new object[0]);
  }

  public int[] GetSortingLayerUniqueIDs()
  {
    var internalEditorUtilityType = typeof(InternalEditorUtility);

    var sortingLayerUniqueIDsProperty =
      internalEditorUtilityType.GetProperty("sortingLayerUniqueIDs", BindingFlags.Static | BindingFlags.NonPublic);

    return (int[])sortingLayerUniqueIDsProperty.GetValue(null, new object[0]);
  }
}
