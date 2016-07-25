using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using UnityEditor;
using UnityEngine;

namespace Assets.Editor.Tiled
{
  public class ImportTiledProjectWindow : EditorWindow
  {
    private const int LABEL_CELL_WIDTH = 200;

    private const int BUTTON_WIDTH = 140;

    private const int LARGE_BUTTON_HEIGHT = 32;

    private const int PADDING = 8;

    private static string NameValueStoreFileName = typeof(ImportTiledProjectWindow).Name + ".xml";

    private Map _map;

    private Objecttypes _objecttypes;

    private string _tmxFilePath = "N/A";

    [MenuItem("Tools/Import Tiled Project")]
    internal static void Init()
    {
      var window = (ImportTiledProjectWindow)GetWindow(typeof(ImportTiledProjectWindow), false, "Tiled Project");

      window.position = new Rect(window.position.xMin + 100f, window.position.yMin + 100f, LABEL_CELL_WIDTH + BUTTON_WIDTH * 2 + PADDING + PADDING * 2, 300f);
    }

    private void Reset()
    {
      _map = null;
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

      DrawInfoRow();

      GUI.enabled = HasMapData();

      DrawCreateButtonRow();

      GUI.enabled = true;
    }

    private void DrawOpenFileRow()
    {
      GUILayout.BeginHorizontal();

      if (GUILayout.Button("Open tmx file", GUILayout.Width(BUTTON_WIDTH * 2 + PADDING + LABEL_CELL_WIDTH), GUILayout.Height(LARGE_BUTTON_HEIGHT)))
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

            _objecttypes = LoadObjectTypes(filePath);

            _tmxFilePath = filePath;

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

    private Objecttypes LoadObjectTypes(string tmxFilePath)
    {
      var objectTypesXmlPath = Path.Combine(
        new FileInfo(tmxFilePath).DirectoryName,
        "objecttypes.xml");

      if (File.Exists(objectTypesXmlPath))
      {
        Debug.Log("Found object types file at location '" + objectTypesXmlPath + "'");

        var serializer = new XmlSerializer(typeof(Objecttypes));

        using (var reader = new StreamReader(objectTypesXmlPath))
        {
          return (Objecttypes)serializer.Deserialize(reader);
        }
      }

      Debug.LogWarning("No object types file found at location '" + objectTypesXmlPath + "'");

      return new Objecttypes { Objecttype = new List<Objecttype>() };
    }

    private void DrawInfoRow()
    {
      GUILayout.BeginHorizontal();

      var labelWidth = 86;

      GUI.skin.label.alignment = TextAnchor.MiddleRight;
      GUILayout.Label("Loaded File", GUILayout.Width(labelWidth));
      GUI.skin.label.alignment = TextAnchor.MiddleLeft;

      GUILayout.TextField(_tmxFilePath, GUILayout.Width(BUTTON_WIDTH * 2 + PADDING + LABEL_CELL_WIDTH - labelWidth));

      GUILayout.EndHorizontal();
    }

    private void DrawCreateButtonRow()
    {
      GUILayout.BeginHorizontal();

      if (GUILayout.Button("Import Objects", GUILayout.Width(BUTTON_WIDTH * 2 + PADDING + LABEL_CELL_WIDTH), GUILayout.Height(LARGE_BUTTON_HEIGHT)))
      {
        ImportData();
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

    private void ImportData()
    {
      var tiledProjectImporter = new TiledProjectImporter(_map, _objecttypes);

      tiledProjectImporter.Import();
    }
  }
}