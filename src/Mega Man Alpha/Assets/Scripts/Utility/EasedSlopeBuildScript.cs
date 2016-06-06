using System.Collections.Generic;
using UnityEngine;

public class EasedSlopeBuildScript : MonoBehaviour
{
  public int TotalSteps = 32;

  public Vector2 FromPosition = Vector2.zero;

  public Vector2 ToPosition = Vector2.zero;

  public EasingType EasingType = EasingType.EaseInOutSine;

  public MeshBuilderFillType MeshBuilderFillType = MeshBuilderFillType.NoFill;

  public MeshBuilderFillDistanceType MeshBuilderFillDistanceType = MeshBuilderFillDistanceType.Relative;

  public float FillDistance = 0f;

  private EdgeCollider2D _edgeCollider;

  public void BuildObject()
  {
    Logger.Info("Start building edge collider.");

    _edgeCollider = GetComponent<EdgeCollider2D>();

    if (_edgeCollider == null)
    {
      Logger.Info("Adding edge collider component.");

      _edgeCollider = gameObject.AddComponent<EdgeCollider2D>();
    }

    var vectors = new List<Vector2>();

    var easing = new Easing();

    var width = ToPosition.x - FromPosition.x;

    var height = ToPosition.y - FromPosition.y;

    for (var i = 0; i < TotalSteps; i++)
    {
      var yPosMultiplier = 1f;

      yPosMultiplier = easing.GetValue(EasingType, i, TotalSteps);

      var vector = new Vector2(width * i / (float)TotalSteps, height * yPosMultiplier);

      vectors.Add(vector);
    }

    vectors.Add(ToPosition);

    _edgeCollider.points = vectors.ToArray();

    if (MeshBuilderFillType != MeshBuilderFillType.NoFill)
    {
      CreateMesh();
    }
  }

  private void CreateMesh()
  {
    var mf = GetComponentInChildren<MeshFilter>();

    if (mf != null)
    {
      Logger.Info("Building meshes");

      var mesh = new Mesh();

      mf.mesh = mesh;

      var vertices = new List<Vector3>();

      var triangles = new List<int>();

      var normals = new List<Vector3>();

      var uvs = new List<Vector2>();

      var index = 0;

      var fillTo = MeshBuilderFillDistanceType == MeshBuilderFillDistanceType.Relative
        ? FillDistance
        : MeshBuilderFillType == MeshBuilderFillType.Horizontal
          ? (FillDistance - transform.position.x)
          : (FillDistance - transform.position.y);

      for (var i = 1; i < _edgeCollider.points.Length; i++)
      {
        if (MeshBuilderFillType == MeshBuilderFillType.Vertical)
        {
          if (fillTo <= 0f)
          {
            vertices.Add(new Vector3(_edgeCollider.points[i - 1].x, _edgeCollider.points[i - 1].y, 0)); //top-left
            vertices.Add(new Vector3(_edgeCollider.points[i].x, _edgeCollider.points[i].y, 0)); //top-right
            vertices.Add(new Vector3(_edgeCollider.points[i - 1].x, fillTo, 0)); //bottom-left
            vertices.Add(new Vector3(_edgeCollider.points[i].x, fillTo, 0)); //bottom-right
          }
          else
          {
            vertices.Add(new Vector3(_edgeCollider.points[i - 1].x, fillTo, 0)); //top-left
            vertices.Add(new Vector3(_edgeCollider.points[i].x, fillTo, 0)); //top-right
            vertices.Add(new Vector3(_edgeCollider.points[i - 1].x, _edgeCollider.points[i - 1].y, 0)); //bottom-left
            vertices.Add(new Vector3(_edgeCollider.points[i].x, _edgeCollider.points[i].y, 0)); //bottom-right
          }
        }
        else
        {
          if (fillTo <= 0f)
          {
            vertices.Add(new Vector3(fillTo, _edgeCollider.points[i].y, 0)); //top-left
            vertices.Add(new Vector3(_edgeCollider.points[i].x, _edgeCollider.points[i].y, 0)); //top-right
            vertices.Add(new Vector3(fillTo, _edgeCollider.points[i - 1].y, 0)); //bottom-left
            vertices.Add(new Vector3(_edgeCollider.points[i - 1].x, _edgeCollider.points[i - 1].y, 0)); //bottom-right
          }
          else
          {
            vertices.Add(new Vector3(_edgeCollider.points[i].x, _edgeCollider.points[i].y, 0)); //top-left
            vertices.Add(new Vector3(fillTo, _edgeCollider.points[i].y, 0)); //top-right
            vertices.Add(new Vector3(_edgeCollider.points[i - 1].x, _edgeCollider.points[i - 1].y, 0)); //bottom-left
            vertices.Add(new Vector3(fillTo, _edgeCollider.points[i - 1].y, 0)); //bottom-right
          }
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
      }

      mesh.vertices = vertices.ToArray();
      mesh.normals = normals.ToArray();
      mesh.triangles = triangles.ToArray();
      mesh.uv = uvs.ToArray();
      mesh.RecalculateNormals();
    }
    else
    {
      Logger.Info("No mesh filter found meshes");
    }
  }
}
