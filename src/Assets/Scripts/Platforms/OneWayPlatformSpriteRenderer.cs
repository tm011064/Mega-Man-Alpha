using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public partial class OneWayPlatformSpriteRenderer : BasePlatform
{
  public float VisibilityCheckInterval = .2f;

  public int Width = 16;

  public int Height = 16;

  public int TileWidth = 16;

  public int TileHeight = 16;

  private BoxCollider2D _visibilityCollider;

  private EdgeCollider2D _physicsCollider;

  private MeshFilter _meshFilter;

  private MeshRenderer _meshRenderer;

  protected override void OnGotVisible()
  {
    _meshRenderer.enabled = true;
  }

  protected override void OnGotHidden()
  {
    _meshRenderer.enabled = false;
  }

  private void ResetLayer()
  {
    _physicsCollider.enabled = true;
  }

  public void AllowFallThrough()
  {
    _physicsCollider.enabled = false;

    Invoke("ResetLayer", .2f);
  }

  private void ResetEdgeCollider()
  {
    _physicsCollider = gameObject.GetComponent<EdgeCollider2D>();

    if (_physicsCollider == null)
    {
      _physicsCollider = gameObject.AddComponent<EdgeCollider2D>();

      _physicsCollider.hideFlags = HideFlags.NotEditable;
    }

#if UNITY_EDITOR
    _physicsCollider.hideFlags = HideFlags.NotEditable;
#endif

    _physicsCollider.points = new Vector2[]
    {
      new Vector2(-Width / 2, Height / 2),
      new Vector2(Width / 2, Height / 2)
    };
  }

  void Awake()
  {
    var groundPlatformVisibilityMaskTransform = transform.FindChild("GroundPlatformVisibilityMask");

    GameObject visibilityMaskGameObject = null;

    if (groundPlatformVisibilityMaskTransform == null)
    {
      Logger.Info("Creating visibility check game object and attach as child.");

      visibilityMaskGameObject = new GameObject("GroundPlatformVisibilityMask");
      visibilityMaskGameObject.transform.SetParent(transform);
      visibilityMaskGameObject.layer = LayerMask.NameToLayer("Background");
      visibilityMaskGameObject.transform.localPosition = Vector3.zero;

      Instantiate(visibilityMaskGameObject, Vector3.zero, Quaternion.identity);
    }
    else
    {
      visibilityMaskGameObject = groundPlatformVisibilityMaskTransform.gameObject;
    }

    visibilityMaskGameObject.hideFlags = HideFlags.NotEditable;

    _visibilityCollider = visibilityMaskGameObject.GetComponent<BoxCollider2D>();

    if (_visibilityCollider == null)
    {
      _visibilityCollider = visibilityMaskGameObject.AddComponent<BoxCollider2D>();
      _visibilityCollider.isTrigger = true;
      _visibilityCollider.hideFlags = HideFlags.NotEditable;
    }

    ResetEdgeCollider();

    _visibilityCollider.size = new Vector2(Width + Screen.width / 2, Height + Screen.height / 2); // add some padding

    _meshFilter = GetComponent<MeshFilter>();

    _meshRenderer = _meshFilter.GetComponent<MeshRenderer>();
  }

  void Start()
  {
    CreatePlane(TileWidth, TileHeight, Width / TileWidth, Height / TileHeight);

    StartVisibilityChecks(VisibilityCheckInterval, _visibilityCollider);
  }

  void CreatePlane(int tileWidth, int tileHeight, int gridWidth, int gridHeight)
  {
    var mesh = new Mesh();

    _meshFilter.mesh = mesh;

    var tileSizeX = 1.0f;

    var tileSizeY = 1.0f;

    var vertices = new List<Vector3>();

    var triangles = new List<int>();

    var normals = new List<Vector3>();

    var uvs = new List<Vector2>();

    var index = 0;

    for (var x = -gridWidth / 2; x < gridWidth / 2; x++)
    {
      for (var y = -gridHeight / 2; y < gridHeight / 2; y++)
      {
        AddVertices(tileHeight, tileWidth, y, x, vertices);

        index = AddTriangles(index, triangles);

        AddNormals(normals);

        AddUvs(0, tileSizeY, tileSizeX, uvs, 0);
      }
    }

    mesh.vertices = vertices.ToArray();
    mesh.normals = normals.ToArray();
    mesh.triangles = triangles.ToArray();
    mesh.uv = uvs.ToArray();
    mesh.RecalculateNormals();
  }

  private static void AddVertices(int tileHeight, int tileWidth, int y, int x, ICollection<Vector3> vertices)
  {
    vertices.Add(new Vector3((x * tileWidth), (y * tileHeight), 0));
    vertices.Add(new Vector3((x * tileWidth) + tileWidth, (y * tileHeight), 0));
    vertices.Add(new Vector3((x * tileWidth) + tileWidth, (y * tileHeight) + tileHeight, 0));
    vertices.Add(new Vector3((x * tileWidth), (y * tileHeight) + tileHeight, 0));
  }

  private static int AddTriangles(int index, ICollection<int> triangles)
  {
    triangles.Add(index + 2);
    triangles.Add(index + 1);
    triangles.Add(index);
    triangles.Add(index);
    triangles.Add(index + 3);
    triangles.Add(index + 2);

    index += 4;

    return index;
  }

  private static void AddNormals(ICollection<Vector3> normals)
  {
    normals.Add(Vector3.forward);
    normals.Add(Vector3.forward);
    normals.Add(Vector3.forward);
    normals.Add(Vector3.forward);
  }

  private static void AddUvs(int tileRow, float tileSizeY, float tileSizeX, ICollection<Vector2> uvs, int tileColumn)
  {
    uvs.Add(new Vector2(tileColumn * tileSizeX, tileRow * tileSizeY));
    uvs.Add(new Vector2((tileColumn + 1) * tileSizeX, tileRow * tileSizeY));
    uvs.Add(new Vector2((tileColumn + 1) * tileSizeX, (tileRow + 1) * tileSizeY));
    uvs.Add(new Vector2(tileColumn * tileSizeX, (tileRow + 1) * tileSizeY));
  }
}
