using UnityEditor;
using UnityEngine;

public class CreateSmoothCameraModifierTransitionTool : EditorWindow
{
  public enum CameraModifierTransitionDirection
  {
    FromGreenToRed,

    FromRedToGreen
  }

  private Vector3 _prevPosition;

  private Vector2 _distanceBetweenModifiers = new Vector2(256, 0);

  private float _horizontalSmoothDampTime = .6f;

  private float _verticalSmoothDampTime = .6f;

  private CameraModifierTransitionDirection _cameraModifierTransitionDirection
    = CameraModifierTransitionDirection.FromGreenToRed;

  [MenuItem("Tools/Create Smooth CameraModifier Transition")]
  static void Init()
  {
    var window = (CreateSmoothCameraModifierTransitionTool)EditorWindow.GetWindow(typeof(CreateSmoothCameraModifierTransitionTool), true);

    window.maxSize = new Vector2(512, 140);
  }

  public void OnGUI()
  {
    _horizontalSmoothDampTime = EditorGUILayout.FloatField("horizontalSmoothDampTime", _horizontalSmoothDampTime);

    _verticalSmoothDampTime = EditorGUILayout.FloatField("verticalSmoothDampTime", _verticalSmoothDampTime);

    _distanceBetweenModifiers = EditorGUILayout.Vector2Field("distanceBetweenModifiers", _distanceBetweenModifiers);

    _cameraModifierTransitionDirection = (CameraModifierTransitionDirection)EditorGUILayout.EnumPopup(_cameraModifierTransitionDirection);

    Debug.Log(Selection.activeGameObject);

    if (Selection.activeGameObject != null)
    {
      MultiWayCameraModifier originalMultiWayCameraModifier
        , transitionMultiWayCameraModifier = null
        , finalMultiWayCameraModifier = null;

      originalMultiWayCameraModifier = Selection.activeGameObject.GetComponent<MultiWayCameraModifier>();

      if (originalMultiWayCameraModifier != null)
      {
        var pressed = GUILayout.Button
          ("Create",
          new GUIStyle(GUI.skin.GetStyle("Button")) { alignment = TextAnchor.MiddleLeft });

        if (pressed)
        {
          // 180 -> green left
          // 0 -> green right
          SceneView.lastActiveSceneView.Focus();

          EditorWindow.focusedWindow.SendEvent(EditorGUIUtility.CommandEvent("Duplicate"));

          if (Selection.activeGameObject != null)
          {
            Selection.activeGameObject.transform.Translate(_distanceBetweenModifiers, Space.World);

            transitionMultiWayCameraModifier = Selection.activeGameObject.GetComponent<MultiWayCameraModifier>();

            SceneView.lastActiveSceneView.Focus();

            EditorWindow.focusedWindow.SendEvent(EditorGUIUtility.CommandEvent("Duplicate"));

            if (Selection.activeGameObject != null)
            {
              Selection.activeGameObject.transform.Translate(_distanceBetweenModifiers, Space.World);

              finalMultiWayCameraModifier = Selection.activeGameObject.GetComponent<MultiWayCameraModifier>();
            }
          }
        }
      }

      if (originalMultiWayCameraModifier != null
        && transitionMultiWayCameraModifier != null
        && finalMultiWayCameraModifier != null)
      {
        switch (_cameraModifierTransitionDirection)
        {
          case CameraModifierTransitionDirection.FromGreenToRed:

            Debug.Log(finalMultiWayCameraModifier.RedCameraModificationSettings.SmoothDampMoveSettings.HorizontalSmoothDampTime);

            originalMultiWayCameraModifier.RedCameraModificationSettings = originalMultiWayCameraModifier.GreenCameraModificationSettings.Clone();
            originalMultiWayCameraModifier.RedCameraModificationSettings.SmoothDampMoveSettings.HorizontalSmoothDampTime = _horizontalSmoothDampTime;
            originalMultiWayCameraModifier.RedCameraModificationSettings.SmoothDampMoveSettings.VerticalSmoothDampTime = _verticalSmoothDampTime;

            transitionMultiWayCameraModifier.GreenCameraModificationSettings.SmoothDampMoveSettings.HorizontalSmoothDampTime = _horizontalSmoothDampTime;
            transitionMultiWayCameraModifier.GreenCameraModificationSettings.SmoothDampMoveSettings.VerticalSmoothDampTime = _verticalSmoothDampTime;
            transitionMultiWayCameraModifier.RedCameraModificationSettings.SmoothDampMoveSettings.HorizontalSmoothDampTime = _horizontalSmoothDampTime;
            transitionMultiWayCameraModifier.RedCameraModificationSettings.SmoothDampMoveSettings.VerticalSmoothDampTime = _verticalSmoothDampTime;

            Debug.Log(finalMultiWayCameraModifier.RedCameraModificationSettings.SmoothDampMoveSettings.HorizontalSmoothDampTime);

            finalMultiWayCameraModifier.GreenCameraModificationSettings = finalMultiWayCameraModifier.RedCameraModificationSettings.Clone();
            finalMultiWayCameraModifier.GreenCameraModificationSettings.SmoothDampMoveSettings.HorizontalSmoothDampTime = _horizontalSmoothDampTime;
            finalMultiWayCameraModifier.GreenCameraModificationSettings.SmoothDampMoveSettings.VerticalSmoothDampTime = _verticalSmoothDampTime;

            break;

          case CameraModifierTransitionDirection.FromRedToGreen:

            originalMultiWayCameraModifier.GreenCameraModificationSettings = originalMultiWayCameraModifier.RedCameraModificationSettings.Clone();
            originalMultiWayCameraModifier.GreenCameraModificationSettings.SmoothDampMoveSettings.HorizontalSmoothDampTime = _horizontalSmoothDampTime;
            originalMultiWayCameraModifier.GreenCameraModificationSettings.SmoothDampMoveSettings.VerticalSmoothDampTime = _verticalSmoothDampTime;

            transitionMultiWayCameraModifier.GreenCameraModificationSettings.SmoothDampMoveSettings.HorizontalSmoothDampTime = _horizontalSmoothDampTime;
            transitionMultiWayCameraModifier.GreenCameraModificationSettings.SmoothDampMoveSettings.VerticalSmoothDampTime = _verticalSmoothDampTime;
            transitionMultiWayCameraModifier.RedCameraModificationSettings.SmoothDampMoveSettings.HorizontalSmoothDampTime = _horizontalSmoothDampTime;
            transitionMultiWayCameraModifier.RedCameraModificationSettings.SmoothDampMoveSettings.VerticalSmoothDampTime = _verticalSmoothDampTime;

            finalMultiWayCameraModifier.RedCameraModificationSettings = finalMultiWayCameraModifier.GreenCameraModificationSettings.Clone();
            finalMultiWayCameraModifier.RedCameraModificationSettings.SmoothDampMoveSettings.HorizontalSmoothDampTime = _horizontalSmoothDampTime;
            finalMultiWayCameraModifier.RedCameraModificationSettings.SmoothDampMoveSettings.VerticalSmoothDampTime = _verticalSmoothDampTime;

            break;
        }
      }
    }
  }
}