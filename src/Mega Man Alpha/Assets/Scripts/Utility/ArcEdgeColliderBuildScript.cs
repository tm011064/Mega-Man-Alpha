using System.Collections.Generic;
using UnityEngine;

public class ArcEdgeColliderBuildScript : MonoBehaviour
{
  public int TotalSegments = 12;

  public MeshBuilderFillType MeshBuilderFillType = MeshBuilderFillType.NoFill;

  public MeshBuilderFillDistanceType MeshBuilderFillDistanceType = MeshBuilderFillDistanceType.Relative;

  public float FillDistance = 0f;

  public Material fillMaterial;

  public void BuildObject()
  {
    Logger.Info("Start building edge collider.");

    var ta = transform.FindChild("A");
    var tb = transform.FindChild("B");
    var tc = transform.FindChild("C");

    if (ta == null || tb == null || tc == null)
    {
      throw new MissingReferenceException();
    }

    var a = ta.position;
    var b = tb.position;
    var c = tc.position;

    var d = 2f * (a.x * (b.y - c.y) + b.x * (c.y - a.y) + c.x * (a.y - b.y));

    var u = new Vector3(
      ((a.x * a.x + a.y * a.y) * (b.y - c.y) + (b.x * b.x + b.y * b.y) * (c.y - a.y) + (c.x * c.x + c.y * c.y) * (a.y - b.y)) / d,
      ((a.x * a.x + a.y * a.y) * (c.x - b.x) + (b.x * b.x + b.y * b.y) * (a.x - c.x) + (c.x * c.x + c.y * c.y) * (b.x - a.x)) / d);

    var arcObject = new GameObject();

    arcObject.name = "Arc Edge Collider";
    arcObject.layer = LayerMask.NameToLayer("Platforms");

    var edgeCollider = arcObject.AddComponent<EdgeCollider2D>();

    var vectors = new List<Vector2>();

    var radius = Vector3.Distance(a, u);

    Debug.Log("u: " + u + ", a: " + a + ", c: " + c);

    var sinb = ((a.x - u.x) / radius);

    var startAngleRad = Mathf.Asin(sinb);

    Debug.Log("Start angle: " + startAngleRad * Mathf.Rad2Deg);

    var sinb2 = ((c.x - u.x) / radius);

    var endAngleRad = Mathf.Asin(sinb2);

    Debug.Log("End angle: " + endAngleRad * Mathf.Rad2Deg);

    if (startAngleRad > endAngleRad)
    {
      var temp = startAngleRad;

      startAngleRad = endAngleRad;

      endAngleRad = temp;
    }

    var startPoint = u + new Vector3(
      (float)(radius * Mathf.Cos(endAngleRad)),
      (float)(radius * Mathf.Sin(endAngleRad)));

    var sinb3 = ((startPoint.x - u.x) / radius);

    var startPointAngle = Mathf.Asin(sinb3);

    var rotAngle = startAngleRad - startPointAngle;

    startAngleRad -= rotAngle;

    endAngleRad -= rotAngle;

    var totalAngle = Mathf.Abs(startAngleRad - endAngleRad);

    var max = totalAngle;

    var step = max / (TotalSegments);

    Debug.Log(startAngleRad * Mathf.Rad2Deg);
    Debug.Log(endAngleRad * Mathf.Rad2Deg);
    Debug.Log(radius);

    var bottomPosition = float.MaxValue;

    for (var theta = endAngleRad; theta > startAngleRad - step / 2; theta -= step)
    {
      var vector = new Vector2((float)(radius * Mathf.Cos(theta)), (float)(radius * Mathf.Sin(theta)));

      vectors.Add(vector);

      if (vector.y < bottomPosition)
      {
        bottomPosition = vector.y;
      }
    }

    for (var i = 0; i < vectors.Count; i++)
    {
      vectors[i] = new Vector2(vectors[i].x, vectors[i].y - bottomPosition);
    }

    arcObject.transform.position = new Vector2(u.x, u.y + bottomPosition);

    edgeCollider.points = vectors.ToArray();

    arcObject.transform.parent = transform;

    if (MeshBuilderFillType != MeshBuilderFillType.NoFill)
    {
      CreateMesh(arcObject, edgeCollider);
    }
  }

  private void CreateMesh(GameObject arcObject, EdgeCollider2D edgeCollider)
  {
    Logger.Info("Building meshes");

    var renderer = arcObject.AddComponent<MeshRenderer>();

    renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
    renderer.receiveShadows = false;
    renderer.useLightProbes = false;
    renderer.reflectionProbeUsage = UnityEngine.Rendering.ReflectionProbeUsage.Off;
    renderer.material = fillMaterial;
    renderer.sortingLayerName = "Platforms";

    var mf = arcObject.AddComponent<MeshFilter>();

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
        ? (FillDistance - arcObject.transform.position.x)
        : (FillDistance - arcObject.transform.position.y);

    for (var i = 1; i < edgeCollider.points.Length; i++)
    {
      if (MeshBuilderFillType == MeshBuilderFillType.Vertical)
      {
        if (fillTo <= 0f)
        {
          vertices.Add(new Vector3(edgeCollider.points[i - 1].x, edgeCollider.points[i - 1].y, 0)); //top-left
          vertices.Add(new Vector3(edgeCollider.points[i].x, edgeCollider.points[i].y, 0)); //top-right
          vertices.Add(new Vector3(edgeCollider.points[i - 1].x, fillTo, 0)); //bottom-left
          vertices.Add(new Vector3(edgeCollider.points[i].x, fillTo, 0)); //bottom-right
        }
        else
        {
          vertices.Add(new Vector3(edgeCollider.points[i - 1].x, fillTo, 0)); //top-left
          vertices.Add(new Vector3(edgeCollider.points[i].x, fillTo, 0)); //top-right
          vertices.Add(new Vector3(edgeCollider.points[i - 1].x, edgeCollider.points[i - 1].y, 0)); //bottom-left
          vertices.Add(new Vector3(edgeCollider.points[i].x, edgeCollider.points[i].y, 0)); //bottom-right
        }
      }
      else
      {
        if (fillTo <= 0f)
        {
          vertices.Add(new Vector3(fillTo, edgeCollider.points[i].y, 0)); //top-left
          vertices.Add(new Vector3(edgeCollider.points[i].x, edgeCollider.points[i].y, 0)); //top-right
          vertices.Add(new Vector3(fillTo, edgeCollider.points[i - 1].y, 0)); //bottom-left
          vertices.Add(new Vector3(edgeCollider.points[i - 1].x, edgeCollider.points[i - 1].y, 0)); //bottom-right
        }
        else
        {
          vertices.Add(new Vector3(edgeCollider.points[i].x, edgeCollider.points[i].y, 0)); //top-left
          vertices.Add(new Vector3(fillTo, edgeCollider.points[i].y, 0)); //top-right
          vertices.Add(new Vector3(edgeCollider.points[i - 1].x, edgeCollider.points[i - 1].y, 0)); //bottom-left
          vertices.Add(new Vector3(fillTo, edgeCollider.points[i - 1].y, 0)); //bottom-right
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
}
