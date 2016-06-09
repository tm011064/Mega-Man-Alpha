using System;
using UnityEngine;

public class BezierSpline : MonoBehaviour
{
  [SerializeField]
  private Vector3[] Points;

  [SerializeField]
  private BezierControlPointMode[] Modes;

  [SerializeField]
  private bool _loop;

  private float[] _segmentLengthPercentages;

  public bool Loop
  {
    get { return _loop; }
    set
    {
      _loop = value;

      if (value == true)
      {
        Modes[Modes.Length - 1] = Modes[0];

        SetControlPoint(0, Points[0]);
      }
    }
  }

  public int ControlPointCount
  {
    get { return Points.Length; }
  }

  public Vector3 GetControlPoint(int index)
  {
    return Points[index];
  }

  public void SetControlPoint(int index, Vector3 point)
  {
    if (index % 3 == 0)
    {
      var delta = point - Points[index];

      if (_loop)
      {
        if (index == 0)
        {
          Points[1] += delta;
          Points[Points.Length - 2] += delta;
          Points[Points.Length - 1] = point;
        }
        else if (index == Points.Length - 1)
        {
          Points[0] = point;
          Points[1] += delta;
          Points[index - 1] += delta;
        }
        else
        {
          Points[index - 1] += delta;
          Points[index + 1] += delta;
        }
      }
      else
      {
        if (index > 0)
        {
          Points[index - 1] += delta;
        }
        if (index + 1 < Points.Length)
        {
          Points[index + 1] += delta;
        }
      }
    }

    Points[index] = point;

    EnforceMode(index);
  }

  public BezierControlPointMode GetControlPointMode(int index)
  {
    return Modes[(index + 1) / 3];
  }

  public void SetControlPointMode(int index, BezierControlPointMode mode)
  {
    var modeIndex = (index + 1) / 3;

    Modes[modeIndex] = mode;

    if (_loop)
    {
      if (modeIndex == 0)
      {
        Modes[Modes.Length - 1] = mode;
      }
      else if (modeIndex == Modes.Length - 1)
      {
        Modes[0] = mode;
      }
    }

    EnforceMode(index);
  }

  private void EnforceMode(int index)
  {
    var modeIndex = (index + 1) / 3;

    var mode = Modes[modeIndex];

    if (mode == BezierControlPointMode.Free
      || !_loop && (modeIndex == 0
      || modeIndex == Modes.Length - 1))
    {
      return;
    }

    var middleIndex = modeIndex * 3;

    int fixedIndex, enforcedIndex;

    if (index <= middleIndex)
    {
      fixedIndex = middleIndex - 1;

      if (fixedIndex < 0)
      {
        fixedIndex = Points.Length - 2;
      }

      enforcedIndex = middleIndex + 1;

      if (enforcedIndex >= Points.Length)
      {
        enforcedIndex = 1;
      }
    }
    else
    {
      fixedIndex = middleIndex + 1;

      if (fixedIndex >= Points.Length)
      {
        fixedIndex = 1;
      }

      enforcedIndex = middleIndex - 1;

      if (enforcedIndex < 0)
      {
        enforcedIndex = Points.Length - 2;
      }
    }

    var middle = Points[middleIndex];

    var enforcedTangent = middle - Points[fixedIndex];

    if (mode == BezierControlPointMode.Aligned)
    {
      enforcedTangent = enforcedTangent.normalized * Vector3.Distance(middle, Points[enforcedIndex]);
    }

    Points[enforcedIndex] = middle + enforcedTangent;
  }

  public int CurveCount
  {
    get { return (Points.Length - 1) / 3; }
  }

  public void CalculateLengths(int totalSegmentsPerCurve)
  {
    float totalLength;

    var segmentLengths = GetSegmentLengths(totalSegmentsPerCurve, out totalLength);
    var segmentLengthPercentages = new float[segmentLengths.Length];

    for (var i = 0; i < segmentLengths.Length; i++)
    {
      segmentLengthPercentages[i] = segmentLengths[i] / totalLength;
    }

    _segmentLengthPercentages = segmentLengthPercentages;
  }

  private float[] GetSegmentLengths(int totalSegmentsPerCurve, out float totalLength)
  {
    totalLength = 0f;

    var curveCount = CurveCount;

    var curveLengths = new float[curveCount];

    for (var i = 0; i < curveCount; i++)
    {
      var curveLength = 0f;

      var index = i * 3;

      var lastPoint = Bezier.GetPoint(Points[index], Points[index + 1], Points[index + 2], Points[index + 3], 0f);

      for (int j = 1; j <= totalSegmentsPerCurve; j++)
      {
        var location = (float)j / (float)totalSegmentsPerCurve;

        var point = Bezier.GetPoint(Points[index], Points[index + 1], Points[index + 2], Points[index + 3], location);
        var length = (point - lastPoint).magnitude;

        curveLength += length;

        lastPoint = point;
      }

      curveLengths[i] = curveLength;

      totalLength += curveLength;
    }

    return curveLengths;
  }

  public Vector3 GetLengthAdjustedPoint(float t)
  {
    var index = 0;

    if (t >= 1f)
    {
      t = 1f;

      index = Points.Length - 4;
    }
    else
    {
      var remainingPercentage = t;

      for (var i = 0; i < _segmentLengthPercentages.Length; i++)
      {
        if (remainingPercentage > _segmentLengthPercentages[i])
        {
          remainingPercentage -= _segmentLengthPercentages[i];
        }
        else
        {
          t = remainingPercentage / _segmentLengthPercentages[i];

          index = i * 3;

          break;
        }
      }
    }

    return transform.TransformPoint(
      Bezier.GetPoint(
        Points[index],
        Points[index + 1],
        Points[index + 2],
        Points[index + 3], t));
  }

  public Vector3 GetLengthAdjustedVelocity(float t)
  {
    var index = 0;

    if (t >= 1f)
    {
      t = 1f;
      index = Points.Length - 4;
    }
    else
    {
      var remainingPercentage = t;

      for (var i = 0; i < _segmentLengthPercentages.Length; i++)
      {
        if (remainingPercentage > _segmentLengthPercentages[i])
        {
          remainingPercentage -= _segmentLengthPercentages[i];
        }
        else
        {
          t = remainingPercentage / _segmentLengthPercentages[i];

          index = i * 3;

          break;
        }
      }
    }

    return transform.TransformPoint(
      Bezier.GetFirstDerivative(
        Points[index],
        Points[index + 1],
        Points[index + 2],
        Points[index + 3], t)) - transform.position;
  }

  public Vector3 GetLengthAdjustedDirection(float t)
  {
    return GetLengthAdjustedVelocity(t).normalized;
  }

  public Vector3 GetPoint(float t)
  {
    int i;
    if (t >= 1f)
    {
      t = 1f;
      i = Points.Length - 4;
    }
    else
    {
      t = Mathf.Clamp01(t) * CurveCount;
      i = (int)t;
      t -= i;
      i *= 3;
    }

    return transform.TransformPoint(
      Bezier.GetPoint(
        Points[i],
        Points[i + 1],
        Points[i + 2],
        Points[i + 3], t));
  }

  public Vector3 GetVelocity(float t)
  {
    int i;

    if (t >= 1f)
    {
      t = 1f;
      i = Points.Length - 4;
    }
    else
    {
      t = Mathf.Clamp01(t) * CurveCount;
      i = (int)t;
      t -= i;
      i *= 3;
    }

    return transform.TransformPoint(
      Bezier.GetFirstDerivative(
        Points[i],
        Points[i + 1],
        Points[i + 2], Points[i + 3], t)) - transform.position;
  }

  public Vector3 GetDirection(float t)
  {
    return GetVelocity(t).normalized;
  }

  public void AddCurve()
  {
    var point = Points[Points.Length - 1];

    Array.Resize(ref Points, Points.Length + 3);

    point.x += 1f;

    Points[Points.Length - 3] = point;

    point.x += 1f;

    Points[Points.Length - 2] = point;

    point.x += 1f;

    Points[Points.Length - 1] = point;

    Array.Resize(ref Modes, Modes.Length + 1);

    Modes[Modes.Length - 1] = Modes[Modes.Length - 2];

    EnforceMode(Points.Length - 4);

    if (_loop)
    {
      Points[Points.Length - 1] = Points[0];

      Modes[Modes.Length - 1] = Modes[0];

      EnforceMode(0);
    }
  }

  public void Reset()
  {
    Points = new Vector3[]
    {
      new Vector3(1f, 0f, 0f),
      new Vector3(2f, 0f, 0f),
      new Vector3(3f, 0f, 0f),
      new Vector3(4f, 0f, 0f)
    };

    Modes = new BezierControlPointMode[] 
    {
      BezierControlPointMode.Free,
      BezierControlPointMode.Free
    };
  }
}
