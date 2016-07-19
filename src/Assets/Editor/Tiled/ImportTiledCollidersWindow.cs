using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using UnityEditor;
using UnityEngine;

namespace Assets.Editor.Tiled
{
  public class ImportTiledCollidersWindow : EditorWindow
  {
    private const int LABEL_CELL_WIDTH = 200;

    private const int BUTTON_WIDTH = 140;

    private static string NameValueStoreFileName = typeof(ImportTiledCollidersWindow).Name + ".xml";

    private string[] _layerNames = new string[0];

    private string _selectedLayerName;

    private string _selectedUnityLayerName;

    private ColliderType _colliderType = ColliderType.Edge;

    private Map _map;

    [MenuItem("Tools/Import Tiled Colliders")]
    internal static void Init()
    {
      var window = (ImportTiledCollidersWindow)GetWindow(typeof(ImportTiledCollidersWindow), false, "Tiled Colliders");

      window.position = new Rect(window.position.xMin + 100f, window.position.yMin + 100f, LABEL_CELL_WIDTH + BUTTON_WIDTH + 16, 300f);
    }

    private IEnumerable<string> GetUnityLayers()
    {
      for (var i = 8; i < 32; i++)
      {
        var name = LayerMask.LayerToName(i);

        if (!string.IsNullOrEmpty(name))
        {
          yield return name;
        }
      }
    }

    private void Reset()
    {
      _map = null;
      _selectedLayerName = null;
      _selectedUnityLayerName = null;

      GetUnityLayers();
    }

    private bool HasMapData()
    {
      return _map != null;
    }

    internal void OnGUI()
    {
      EditorGUILayout.BeginVertical();

      GUILayout.Label("Create Tiled Colliders", EditorStyles.boldLabel);

      DrawOpenFileRow();

      GUI.enabled = HasMapData();

      DrawSelectLayerRow();

      DrawSelectUnityLayerRow();

      DrawColliderTypeRow();

      DrawCreateButtonRow();

      GUI.enabled = true;
    }

    private void DrawOpenFileRow()
    {
      GUILayout.BeginHorizontal();

      GUILayout.Label("Load file", GUILayout.Width(LABEL_CELL_WIDTH));

      if (GUILayout.Button("Open tmx file", GUILayout.Width(BUTTON_WIDTH)))
      {
        var filePath = EditorUtility.OpenFilePanel("Load tmx file", GetDefaultFolderLocation(), "tmx");

        if (filePath.Length != 0)
        {
          PersistFolderLocation(filePath);

          Reset();

          var serializer = new XmlSerializer(typeof(Map));

          try
          {
            using (var reader = new StreamReader(filePath))
            {
              _map = (Map)serializer.Deserialize(reader);
            }

            _layerNames = _map.Layer.Select(l => l.Name).ToArray();

            Debug.Log("Map " + filePath + " loaded");
          }
          catch (Exception)
          {
            Reset();

            throw;
          }
        }
      }

      GUILayout.EndHorizontal();
    }

    private void DrawSelectLayerRow()
    {
      GUILayout.BeginHorizontal();

      GUILayout.Label("Choose Tiled layer", GUILayout.Width(LABEL_CELL_WIDTH));

      if (GUILayout.Button(_selectedLayerName ?? "Select Layer", GUILayout.Width(BUTTON_WIDTH)))
      {
        var toolsMenu = new GenericMenu();

        foreach (var layerName in _layerNames)
        {
          toolsMenu.AddItem(new GUIContent(layerName), false, (name) =>
          {
            _selectedLayerName = name.ToString();
          }, layerName);
        }

        toolsMenu.DropDown(new Rect(LABEL_CELL_WIDTH, 0, 0, 16));

        EditorGUIUtility.ExitGUI();
      }

      GUILayout.EndHorizontal();
    }

    private void DrawSelectUnityLayerRow()
    {
      GUILayout.BeginHorizontal();

      GUILayout.Label("Choose unity layer", GUILayout.Width(LABEL_CELL_WIDTH));

      if (GUILayout.Button(_selectedUnityLayerName ?? "Select Layer", GUILayout.Width(BUTTON_WIDTH)))
      {
        var toolsMenu = new GenericMenu();

        foreach (var layerName in GetUnityLayers())
        {
          toolsMenu.AddItem(new GUIContent(layerName), false, (name) =>
          {
            _selectedUnityLayerName = name.ToString();
          }, layerName);
        }

        toolsMenu.DropDown(new Rect(LABEL_CELL_WIDTH, 0, 0, 16));

        EditorGUIUtility.ExitGUI();
      }

      GUILayout.EndHorizontal();
    }

    private void DrawColliderTypeRow()
    {
      GUILayout.BeginHorizontal();

      GUILayout.Label("Choose collider type", GUILayout.Width(LABEL_CELL_WIDTH));

      if (GUILayout.Button(_colliderType.ToString(), GUILayout.Width(BUTTON_WIDTH)))
      {
        var toolsMenu = new GenericMenu();

        toolsMenu.AddItem(new GUIContent(ColliderType.Edge.ToString()), false, () => { _colliderType = ColliderType.Edge; });
        toolsMenu.AddItem(new GUIContent(ColliderType.Polygon.ToString()), false, () => { _colliderType = ColliderType.Polygon; });

        toolsMenu.DropDown(new Rect(LABEL_CELL_WIDTH, 0, 0, 16));

        EditorGUIUtility.ExitGUI();
      }

      GUILayout.EndHorizontal();
    }

    private void DrawCreateButtonRow()
    {
      GUILayout.BeginHorizontal();

      GUILayout.Label(string.Empty, GUILayout.Width(LABEL_CELL_WIDTH));

      if (!string.IsNullOrEmpty(_selectedLayerName)
        && !string.IsNullOrEmpty(_selectedUnityLayerName)
        && GUILayout.Button("Create Colliders", GUILayout.Width(BUTTON_WIDTH)))
      {
        CreateColliders();
      }

      GUILayout.EndHorizontal();
    }

    private string GetDefaultFolderLocation()
    {
      var nameValueStore = NameValueStore.Deserialize(NameValueStoreFileName);

      return nameValueStore.GetValue("defaultPath");
    }

    private void PersistFolderLocation(string filePath)
    {
      var nameValueStore = NameValueStore.Deserialize(NameValueStoreFileName);

      nameValueStore.SetValue("defaultPath", filePath);

      nameValueStore.Serialize(NameValueStoreFileName);
    }

    private void CreateColliders()
    {
      var tileIntegers = new int[_map.Height * _map.Width];

      var tileIntegersFlipped = _map
        .Layer
        .First(l => l.Name == _selectedLayerName)
        .Data
        .Text
        .Split(',')
        .Select(text => int.Parse(text));

      var rowIndex = _map.Height - 1;
      var columnIndex = 0;

      foreach (var value in tileIntegersFlipped)
      {
        var location = rowIndex * _map.Width + columnIndex;

        tileIntegers[location] = value;

        columnIndex++;

        if (columnIndex == _map.Width)
        {
          columnIndex = 0;
          rowIndex--;
        }
      }

      var matrix = new Matrix<int>(tileIntegers, _map.Height, _map.Width);

      var builder = new VertexBuilder(matrix);

      builder.Build(_map.Tilewidth, _map.Tileheight);

      var colliders = builder.GetColliderEdges();

      GameObject collidersGameObject = new GameObject("Poly Colliders");
      collidersGameObject.transform.position = Vector3.zero;

      Debug.Log("Created " + collidersGameObject);

      foreach (var points in colliders)
      {
        GameObject obj = new GameObject("Collider");

        obj.transform.position = Vector3.zero;
        obj.layer = LayerMask.NameToLayer(_selectedUnityLayerName);

        switch (_colliderType)
        {
          case ColliderType.Edge:
            AddEdgeColliders(obj, points);
            break;

          case ColliderType.Polygon:
            AddPolygonCollider(obj, points);
            break;
        }

        obj.transform.parent = collidersGameObject.transform;

        Debug.Log("Created " + obj);
      }
    }

    private void AddPolygonCollider(GameObject parent, Vector2[] points)
    {
      var polyCollider = parent.AddComponent<PolygonCollider2D>();

      polyCollider.points = points;
    }

    private void AddEdgeColliders(GameObject parent, Vector2[] points)
    {
      if (TryAddRectangle(parent, points))
      {
        return;
      }

      for (var i = 0; i < points.Length - 1; i++)
      {
        var edgeCollider = parent.AddComponent<EdgeCollider2D>();

        edgeCollider.points = new Vector2[] { points[i], points[i + 1] };
      }
    }

    private bool TryAddRectangle(GameObject parent, Vector2[] points)
    {
      if (points.Length != 4)
      {
        return false;
      }

      var xPositions = points.Select(p => (int)p.x).Distinct().ToArray();

      if (xPositions.Count() != 2)
      {
        return false;
      }

      var yPositions = points.Select(p => (int)p.y).Distinct().ToArray();

      if (yPositions.Count() != 2)
      {
        return false;
      }

      var boxCollider = parent.AddComponent<BoxCollider2D>();

      boxCollider.size = new Vector2(xPositions.Max() - xPositions.Min(), yPositions.Max() - yPositions.Min());
      boxCollider.offset = boxCollider.size / 2;
      boxCollider.transform.position = new Vector3(
        xPositions.Max() - boxCollider.size.x,
        yPositions.Max() - boxCollider.size.y);

      return true;
    }

    private enum ColliderType
    {
      Polygon,

      Edge
    }
  }
}