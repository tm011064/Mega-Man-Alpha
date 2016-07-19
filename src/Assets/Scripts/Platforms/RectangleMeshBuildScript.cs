using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public partial class RectangleMeshBuildScript : BasePlatform
{
  public float VisibilityCheckInterval = .2f;

  public int Width = 16;

  public int Height = 16;

  public bool createTiles = false;

  public int TileWidth = 64;

  public int TileHeight = 64;

  public TextAnchor Anchor = TextAnchor.LowerLeft;

  public bool IsTrigger = false;

  [EnumFlag]
  public Direction ColliderSides = Direction.Left | Direction.Up | Direction.Right | Direction.Down;

#if UNITY_EDITOR
  public string PrefabMeshFolder = "Assets/Meshes/";

  public string PrefabObjectFolder = "Assets/Prefabs/TestBlocks/";

  private EdgeCollider2D[] _edgeCollidersToDestroy;

  private BoxCollider2D[] _boxCollider2DToDestroy;

  public void SafeDeleteColliders()
  {
    if (_edgeCollidersToDestroy != null)
    {
      Debug.Log("Rectangle Mesh Builder: Deleting old edge colliders...");

      for (var i = _edgeCollidersToDestroy.Length - 1; i >= 0; i--)
      {
        DestroyImmediate(_edgeCollidersToDestroy[i]);
      }

      _edgeCollidersToDestroy = null;
    }
    if (_boxCollider2DToDestroy != null)
    {
      Debug.Log("Rectangle Mesh Builder: Deleting old box colliders...");

      for (var i = _boxCollider2DToDestroy.Length - 1; i >= 0; i--)
      {
        DestroyImmediate(_boxCollider2DToDestroy[i]);
      }

      _boxCollider2DToDestroy = null;
    }

    UnityEditor.SceneView.RepaintAll();
  }

  private Vector2 GetTopLeftVector2()
  {
    switch (Anchor)
    {
      case TextAnchor.LowerCenter:
        return new Vector2(-Width / 2, Height);

      case TextAnchor.LowerLeft:
        return new Vector2(0, Height);

      case TextAnchor.MiddleCenter:
        return new Vector2(-Width / 2, Height / 2);

      default:
        throw new ArgumentException("TextAnchor " + Anchor + " not supported.");
    }
  }

  private Vector2 GetTopRightVector2()
  {
    switch (Anchor)
    {
      case TextAnchor.LowerCenter:
        return new Vector2(Width / 2, Height);

      case TextAnchor.LowerLeft:
        return new Vector2(Width, Height);

      case TextAnchor.MiddleCenter:
        return new Vector2(Width / 2, Height / 2);

      default:
        throw new ArgumentException("TextAnchor " + Anchor + " not supported.");
    }
  }

  private Vector2 GetBottomLeftVector2()
  {
    switch (Anchor)
    {
      case TextAnchor.LowerCenter:
        return new Vector2(-Width / 2, 0);

      case TextAnchor.LowerLeft:
        return new Vector2(0, 0);

      case TextAnchor.MiddleCenter:
        return new Vector2(-Width / 2, -Height / 2);

      default:
        throw new ArgumentException("TextAnchor " + Anchor + " not supported.");
    }
  }

  private Vector2 GetBottomRightVector2()
  {
    switch (Anchor)
    {
      case TextAnchor.LowerCenter:
        return new Vector2(Width / 2, 0);

      case TextAnchor.LowerLeft:
        return new Vector2(Width, 0);

      case TextAnchor.MiddleCenter:
        return new Vector2(Width / 2, -Height / 2);

      default:
        throw new ArgumentException("TextAnchor " + Anchor + " not supported.");
    }
  }

  private void SetColliders()
  {
    Debug.Log("Rectangle Mesh Builder: Destroying edge and box colliders.");

    _edgeCollidersToDestroy = gameObject.GetComponents<EdgeCollider2D>();

    foreach (var collider in _edgeCollidersToDestroy)
    {
      collider.hideFlags = HideFlags.HideInInspector;
    }

    _boxCollider2DToDestroy = gameObject.GetComponents<BoxCollider2D>();

    foreach (var collider in _boxCollider2DToDestroy)
    {
      collider.hideFlags = HideFlags.HideInInspector;
    }

    if (ColliderSides == (Direction.Left | Direction.Up | Direction.Right | Direction.Down))
    {
      Debug.Log("Rectangle Mesh Builder: Creating box collider.");

      var boxCollider = gameObject.AddComponent<BoxCollider2D>();
      boxCollider.hideFlags = HideFlags.NotEditable;
      boxCollider.size = new Vector2(Width, Height);
      boxCollider.isTrigger = IsTrigger;

      switch (Anchor)
      {
        case TextAnchor.LowerCenter:
          boxCollider.offset = new Vector2(0, Height / 2);
          break;

        case TextAnchor.LowerLeft:
          boxCollider.offset = new Vector2(Width / 2, Height / 2);
          break;

        case TextAnchor.MiddleCenter:
          boxCollider.offset = new Vector2(0, 0);
          break;

        default:
          throw new ArgumentException("TextAnchor " + Anchor + " not supported.");
      }

      Debug.Log("Rectangle Mesh Builder: Setting box collider size to " + boxCollider.size);
    }
    else if (ColliderSides == 0)
    {
      Debug.Log("No colliders");
    }
    else
    {
      List<Vector2> vectors;

      EdgeCollider2D collider;

      if (ColliderSides == (Direction.Left | Direction.Right))
      {
        Debug.Log("Rectangle Mesh Builder: Creating Left and Right edge colliders.");

        vectors = new List<Vector2>();
        vectors.Add(GetBottomLeftVector2());
        vectors.Add(GetTopLeftVector2());

        collider = gameObject.AddComponent<EdgeCollider2D>();
        collider.hideFlags = HideFlags.NotEditable;
        collider.points = vectors.ToArray();
        collider.isTrigger = IsTrigger;

        vectors = new List<Vector2>();
        vectors.Add(GetBottomRightVector2());
        vectors.Add(GetTopRightVector2());

        collider = gameObject.AddComponent<EdgeCollider2D>();
        collider.hideFlags = HideFlags.NotEditable;
        collider.points = vectors.ToArray();
        collider.isTrigger = IsTrigger;
      }
      else if (ColliderSides == (Direction.Up | Direction.Down))
      {
        Debug.Log("Rectangle Mesh Builder: Creating Top and Bottom edge colliders.");

        vectors = new List<Vector2>();
        vectors.Add(GetTopLeftVector2());
        vectors.Add(GetTopRightVector2());

        collider = gameObject.AddComponent<EdgeCollider2D>();
        collider.hideFlags = HideFlags.NotEditable;
        collider.points = vectors.ToArray();
        collider.isTrigger = IsTrigger;

        vectors = new List<Vector2>();
        vectors.Add(GetBottomLeftVector2());
        vectors.Add(GetBottomRightVector2());

        collider = gameObject.AddComponent<EdgeCollider2D>();
        collider.hideFlags = HideFlags.NotEditable;
        collider.points = vectors.ToArray();
        collider.isTrigger = IsTrigger;
      }
      else
      {
        Debug.Log("Rectangle Mesh Builder: Creating edge collider.");

        collider = gameObject.AddComponent<EdgeCollider2D>();

        collider.hideFlags = HideFlags.NotEditable;

        // go counter clockwise to find gap and then go clockwise to build vectors
        var linkedDirections = new LinkedList<Direction?>();

        var currentListNode = linkedDirections.AddFirst(
          ((ColliderSides & Direction.Left) != 0) ? (Direction?)Direction.Left : null);

        currentListNode = linkedDirections.AddAfter(
          currentListNode,
          ((ColliderSides & Direction.Up) != 0) ? (Direction?)Direction.Up : null);

        currentListNode = linkedDirections.AddAfter(
          currentListNode,
          ((ColliderSides & Direction.Right) != 0) ? (Direction?)Direction.Right : null);

        currentListNode = linkedDirections.AddAfter(
          currentListNode,
          ((ColliderSides & Direction.Down) != 0) ? (Direction?)Direction.Down : null);

        while (true)
        {
          // search backwards for gap opening
          if (currentListNode.Value.HasValue && !currentListNode.PreviousOrLast().Value.HasValue)
          {
            break;
          }

          currentListNode = currentListNode.PreviousOrLast();
        }

        // add first point
        vectors = new List<Vector2>();

        switch (currentListNode.Value.Value)
        {
          case Direction.Left:
            vectors.Add(GetBottomLeftVector2());
            break;

          case Direction.Up:
            vectors.Add(GetTopLeftVector2());
            break;

          case Direction.Right:
            vectors.Add(GetTopRightVector2());
            break;

          case Direction.Down:
            vectors.Add(GetBottomRightVector2());
            break;
        }

        while (currentListNode.Value.HasValue)
        {
          // now go forward to fill points until gap
          switch (currentListNode.Value.Value)
          {
            case Direction.Left:
              vectors.Add(GetTopLeftVector2());
              break;

            case Direction.Up:
              vectors.Add(GetTopRightVector2());
              break;

            case Direction.Right:
              vectors.Add(GetBottomRightVector2());
              break;

            case Direction.Down:
              vectors.Add(GetBottomLeftVector2());
              break;
          }

          currentListNode = currentListNode.NextOrFirst();
        }

        collider.points = vectors.ToArray();
        collider.isTrigger = IsTrigger;
      }
    }
  }

  private void ApplyChangesToDependants()
  {
    var movingPlatformCollisionTrigger = transform.FindChild("MovingPlatformCollisionTrigger");

    if (movingPlatformCollisionTrigger != null)
    {
      var boxCollider = movingPlatformCollisionTrigger.gameObject.GetComponent<BoxCollider2D>();

      if (boxCollider != null)
      {
        boxCollider.offset = new Vector2(Width / 2f, Height / 2f);
        boxCollider.size = new Vector2(Width + 2f, Height + 2f);
      }
    }
  }

  public void CreatePrefab()
  {
    var mesh = CreateTiles(TileWidth, TileHeight, Width / TileWidth, Height / TileHeight);

    UnityEditor.AssetDatabase.CreateAsset(mesh, PrefabMeshFolder + name + ".obj");
    UnityEditor.AssetDatabase.SaveAssets();
    UnityEditor.AssetDatabase.Refresh();

    var meshFilter = gameObject.GetComponent<MeshFilter>();

    meshFilter.mesh = mesh;

    var emptyPrefab = UnityEditor.PrefabUtility.CreateEmptyPrefab("Assets/Prefabs/TestBlocks/" + name + ".prefab");

    UnityEditor.PrefabUtility.ReplacePrefab(gameObject, emptyPrefab);
  }

  public void BuildObject()
  {
    SetColliders();

    var meshFilter = gameObject.GetComponent<MeshFilter>();

    if (createTiles)
    {
      meshFilter.mesh = CreateTiles(TileWidth, TileHeight, Width / TileWidth, Height / TileHeight);
    }
    else
    {
      CreatePlane();
    }

    ApplyChangesToDependants();
  }

  public Mesh CreatePlane()
  {
    var mesh = new Mesh();

    var vertices = new List<Vector3>();

    var triangles = new List<int>();

    var normals = new List<Vector3>();

    var uvs = new List<Vector2>();

    var index = 0;

    switch (Anchor)
    {
      case TextAnchor.LowerCenter:

        vertices.Add(new Vector3(-Width / 2, Height, 0)); //top-left
        vertices.Add(new Vector3(Width / 2, Height, 0)); //top-right
        vertices.Add(new Vector3(-Width / 2, 0, 0)); //bottom-left
        vertices.Add(new Vector3(Width / 2, 0, 0)); //bottom-right

        break;

      case TextAnchor.LowerLeft:

        vertices.Add(new Vector3(0, Height, 0)); //top-left
        vertices.Add(new Vector3(Width, Height, 0)); //top-right
        vertices.Add(new Vector3(0, 0, 0)); //bottom-left
        vertices.Add(new Vector3(Width, 0, 0)); //bottom-right

        break;

      case TextAnchor.MiddleCenter:

        vertices.Add(new Vector3(-Width / 2, Height / 2, 0)); //top-left
        vertices.Add(new Vector3(Width / 2, Height / 2, 0)); //top-right
        vertices.Add(new Vector3(-Width / 2, -Height / 2, 0)); //bottom-left
        vertices.Add(new Vector3(Width, -Height / 2, 0)); //bottom-right

        break;

      default:
        throw new ArgumentException("TextAnchor " + Anchor + " not supported.");
    }

    triangles.Add(index);
    triangles.Add(index + 1);
    triangles.Add(index + 2);
    triangles.Add(index + 3);
    triangles.Add(index + 2);
    triangles.Add(index + 1);

    index += 4;

    normals.Add(Vector3.forward);
    normals.Add(Vector3.forward);
    normals.Add(Vector3.forward);
    normals.Add(Vector3.forward);

    uvs.Add(new Vector2(0, 1)); //top-left
    uvs.Add(new Vector2(1, 1)); //top-right
    uvs.Add(new Vector2(0, 0)); //bottom-left
    uvs.Add(new Vector2(1, 0)); //bottom-right    

    mesh.vertices = vertices.ToArray();
    mesh.normals = normals.ToArray();
    mesh.triangles = triangles.ToArray();
    mesh.uv = uvs.ToArray();
    mesh.RecalculateNormals();

    return mesh;
  }

  Mesh CreateTiles(float tileWidth, float tileHeight, float gridWidth, float gridHeight)
  {
    var mesh = new Mesh();

    var tileSizeX = 1.0f;

    var tileSizeY = 1.0f;

    var vertices = new List<Vector3>();

    var triangles = new List<int>();

    var normals = new List<Vector3>();

    var uvs = new List<Vector2>();

    var index = 0;

    float xFrom, xTo, yFrom, yTo;

    switch (Anchor)
    {
      case TextAnchor.LowerCenter:

        xFrom = -gridWidth / 2f;
        xTo = gridWidth / 2f;
        yFrom = 0f;
        yTo = gridHeight;

        break;

      case TextAnchor.LowerLeft:

        xFrom = 0;
        xTo = gridWidth;
        yFrom = 0;
        yTo = gridHeight;

        break;

      case TextAnchor.MiddleCenter:

        xFrom = -gridWidth / 2f;
        xTo = gridWidth / 2f;
        yFrom = -gridHeight / 2f;
        yTo = gridHeight / 2f;

        break;

      default:
        throw new ArgumentException("TextAnchor " + Anchor + " not supported.");
    }

    for (var x = xFrom; x < xTo; x++)
    {
      for (var y = yFrom; y < yTo; y++)
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

    return mesh;
  }

  private static void AddVertices(float tileHeight, float tileWidth, float y, float x, ICollection<Vector3> vertices)
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
#endif
}
