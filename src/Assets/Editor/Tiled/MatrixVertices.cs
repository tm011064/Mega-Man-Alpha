using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Editor.Tiled
{
  public class MatrixVertices
  {
    private Matrix<long> _matrix;

    private Vertex[] _vertices;

    private int _tileWidth;

    private int _tileHeight;

    public MatrixVertices(Matrix<long> matrix, int tileWidth, int tileHeight)
    {
      _matrix = matrix;
      _tileWidth = tileWidth;
      _tileHeight = tileHeight;

      var totalVertices = (_matrix.Rows + 1) * (_matrix.Columns + 1);

      _vertices = new Vertex[totalVertices];

      Initialize();
    }

    private void Initialize()
    {
      InitializeVertices();

      for (var rowIndex = 0; rowIndex < _matrix.Rows; rowIndex++)
      {
        for (var columnIndex = 0; columnIndex < _matrix.Columns; columnIndex++)
        {
          var isColliderPoint = _matrix.GetItem(rowIndex, columnIndex) > 0;

          var topLeftVertexIndex = rowIndex * (_matrix.Columns + 1) + columnIndex;

          var topRightVertexIndex = topLeftVertexIndex + 1;

          var bottomLeftVertexIndex = topLeftVertexIndex + _matrix.Columns + 1;

          var bottomRightVertexIndex = bottomLeftVertexIndex + 1;

          SetColliderEdge(topLeftVertexIndex, topRightVertexIndex, Direction.Right, isColliderPoint);
          SetColliderEdge(bottomLeftVertexIndex, bottomRightVertexIndex, Direction.Right, isColliderPoint);

          SetColliderEdge(topLeftVertexIndex, bottomLeftVertexIndex, Direction.Down, isColliderPoint);
          SetColliderEdge(topRightVertexIndex, bottomRightVertexIndex, Direction.Down, isColliderPoint);

          SetColliderEdge(topRightVertexIndex, topLeftVertexIndex, Direction.Left, isColliderPoint);
          SetColliderEdge(bottomRightVertexIndex, bottomLeftVertexIndex, Direction.Left, isColliderPoint);

          SetColliderEdge(bottomRightVertexIndex, topRightVertexIndex, Direction.Up, isColliderPoint);
          SetColliderEdge(bottomLeftVertexIndex, topLeftVertexIndex, Direction.Up, isColliderPoint);
        }
      }
    }

    private void InitializeVertices()
    {
      var index = 0;

      for (var i = 0; i <= _matrix.Rows; i++)
      {
        for (var j = 0; j <= _matrix.Columns; j++)
        {
          var point = new Vector2(
              _tileWidth * j,
              _tileHeight * i);

          point = TranslateToWorldCoordinates(point);

          _vertices[index++] = new Vertex(point);
        }
      }
    }

    private Vector2 TranslateToWorldCoordinates(Vector2 point)
    {
      return new Vector2(
        point.x,
        point.y - (_matrix.Rows * _tileHeight));
    }

    private bool CanOverwriteColliderEdge(int index, Direction direction)
    {
      return _vertices[index].Edges[direction] == Edge.NullEdge
        || (!_vertices[index].Edges[direction].IsColliderEdge);
    }

    private void SetColliderEdge(int fromIndex, int toIndex, Direction direction, bool isColliderEdge)
    {
      if (CanOverwriteColliderEdge(fromIndex, direction))
      {
        _vertices[fromIndex].Edges[direction] = new Edge
        {
          From = _vertices[fromIndex],
          To = _vertices[toIndex],
          IsColliderEdge = isColliderEdge
        };
      }
    }

    private bool FindUnvisitedColliderVertex(out Vertex vertex)
    {
      for (var i = 0; i < _vertices.Length; i++)
      {
        if (_vertices[i].IsVisited)
        {
          continue;
        }

        _vertices[i].IsVisited = true;

        if (_vertices[i].AreAllEdgesColliders()
          || _vertices[i].HasNoColliderEdges()
          )
        {
          continue;
        }

        vertex = _vertices[i];

        vertex.IsVisited = true;

        return true;
      }

      vertex = null;
      return false;
    }

    private Vertex FindEdgePoint(Vertex vertex, Direction searchDirection)
    {
      while (true)
      {
        vertex.IsVisited = true;

        if (!vertex.Edges[searchDirection].IsColliderEdge
        || (vertex.Edges[searchDirection.RotateAntiClockwise()].IsColliderEdge && vertex.Edges[searchDirection.RotateClockwise()].IsColliderEdge))
        {
          return vertex;
        }

        vertex = vertex.Edges[searchDirection].To;
      }
    }

    private Direction GetNextSearchDirectionForConvex(Vertex vertex, Direction direction)
    {
      return vertex.Edges[direction.RotateClockwise()].IsColliderEdge
        ? direction.RotateClockwise()
        : direction.RotateAntiClockwise();
    }

    private Direction GetNextSearchDirectionForConcave(Vertex vertex, Direction direction)
    {
      var antiClockwiseDirection = direction.RotateAntiClockwise();
      var clockwiseDirection = direction.RotateClockwise();
      var reverseDirection = direction.Reverse();

      if (!vertex.Edges[antiClockwiseDirection].To.Edges[reverseDirection].IsColliderEdge)
      {
        return antiClockwiseDirection;
      }

      if (!vertex.Edges[clockwiseDirection].To.Edges[reverseDirection].IsColliderEdge)
      {
        return clockwiseDirection;
      }

      if (!vertex.Edges[reverseDirection].To.Edges[antiClockwiseDirection].IsColliderEdge)
      {
        return antiClockwiseDirection;
      }

      if (!vertex.Edges[reverseDirection].To.Edges[clockwiseDirection].IsColliderEdge)
      {
        return clockwiseDirection;
      }

      throw new NotImplementedException();
    }

    private Direction GetNextSearchDirection(Vertex vertex, Direction direction)
    {
      if (!vertex.AreAllEdgesColliders())
      {
        return GetNextSearchDirectionForConvex(vertex, direction);
      }

      return GetNextSearchDirectionForConcave(vertex, direction);
    }

    public IEnumerable<TiledEdgePoints> GetTopColliderEdges()
    {
      ResetVerticesVisitStatus();

      Vertex startVertex = null;

      while (FindUnvisitedColliderVertex(out startVertex))
      {
        var searchDirection = Direction.Right;

        startVertex = FindEdgePoint(startVertex, searchDirection);

        var vertex = startVertex;

        var lastPoint = vertex.Point;

        while (true)
        {
          searchDirection = GetNextSearchDirection(vertex, searchDirection);

          var newStartVertex = vertex.Edges[searchDirection].To;

          newStartVertex.IsVisited = true;

          vertex = FindEdgePoint(newStartVertex, searchDirection);

          if (searchDirection == Direction.Left) // left since tiled uses y down coordinates
          {
            yield return new TiledEdgePoints { From = lastPoint, To = vertex.Point };
          }

          if (vertex == startVertex)
          {
            break;
          }

          lastPoint = vertex.Point;
        }
      }
    }

    private Bounds GetBounds(Vector2[] points)
    {
      if (points.Length != 4)
      {
        throw new ArgumentException("Point data is not rectangular");
      }

      var xPositions = points.Select(p => (int)p.x).Distinct().ToArray();

      if (xPositions.Count() != 2)
      {
        throw new ArgumentException("Point data is not rectangular");
      }

      var yPositions = points.Select(p => (int)p.y).Distinct().ToArray();

      if (yPositions.Count() != 2)
      {
        throw new ArgumentException("Point data is not rectangular");
      }

      var size = new Vector2(xPositions.Max() - xPositions.Min(), yPositions.Max() - yPositions.Min());
      var center = new Vector3(
        xPositions.Max() - size.x * .5f,
        yPositions.Max() - size.y * .5f);

      return new Bounds(center, size);
    }

    public IEnumerable<Bounds> GetRectangleBounds()
    {
      return GetColliderEdges()
        .Select(p => GetBounds(p));
    }

    public IEnumerable<Vector2[]> GetColliderEdges(int contraction = 0)
    {
      ResetVerticesVisitStatus();

      Vertex startVertex = null;

      while (FindUnvisitedColliderVertex(out startVertex))
      {
        var searchDirection = Direction.Right;

        startVertex = FindEdgePoint(startVertex, searchDirection);

        var vertex = startVertex;

        var vertexPoints = new List<Vector2>();

        vertexPoints.Add(vertex.Point);

        while (true)
        {
          searchDirection = GetNextSearchDirection(vertex, searchDirection);

          var newStartVertex = vertex.Edges[searchDirection].To;

          newStartVertex.IsVisited = true;

          vertex = FindEdgePoint(newStartVertex, searchDirection);

          var point = vertex.Point;

          if (contraction != 0)
          {
            var lastVertexPoint = vertexPoints.Last();

            switch (searchDirection)
            {
              case Direction.Right:
                vertexPoints[vertexPoints.Count - 1] = new Vector2(lastVertexPoint.x + contraction, lastVertexPoint.y);
                point = new Vector2(point.x - contraction, point.y);
                break;

              case Direction.Left:
                vertexPoints[vertexPoints.Count - 1] = new Vector2(lastVertexPoint.x - contraction, lastVertexPoint.y);
                point = new Vector2(point.x + contraction, point.y);
                break;

              case Direction.Up:
                vertexPoints[vertexPoints.Count - 1] = new Vector2(lastVertexPoint.x, lastVertexPoint.y - contraction);
                point = new Vector2(point.x, point.y + contraction);
                break;

              case Direction.Down:
                vertexPoints[vertexPoints.Count - 1] = new Vector2(lastVertexPoint.x, lastVertexPoint.y + contraction);
                point = new Vector2(point.x, point.y - contraction);
                break;
            }
          }

          if (vertex == startVertex)
          {
            if (contraction != 0)
            {
              switch (searchDirection)
              {
                case Direction.Right:
                case Direction.Left:
                  vertexPoints[0] = new Vector2(point.x, vertexPoints[0].y);
                  break;

                case Direction.Up:
                case Direction.Down:
                  vertexPoints[0] = new Vector2(vertexPoints[0].x, point.y);
                  break;
              }
            }

            break;
          }

          vertexPoints.Add(point);
        }

        yield return vertexPoints.ToArray();
      }
    }

    private void ResetVerticesVisitStatus()
    {
      for (var i = 0; i < _vertices.Length; i++)
      {
        _vertices[i].IsVisited = false;
      }
    }

    public override string ToString()
    {
      var output = new StringBuilder();

      for (var i = 0; i < _matrix.Length; i++)
      {
        if (i % _matrix.Columns == 0)
        {
          output.Append(Environment.NewLine);
        }

        output.Append(" " + _matrix[i] + " ");
      }

      output.Append(Environment.NewLine);

      for (var i = 0; i < _vertices.Length; i++)
      {
        if (i % (_matrix.Columns + 1) == 0)
        {
          output.Append(Environment.NewLine);
        }

        output.Append((_vertices[i].Edges[Direction.Up].IsColliderEdge) ? "|" : " ");
        output.Append((_vertices[i].Edges[Direction.Right].IsColliderEdge) ? "_" : " ");
      }

      output.Append(Environment.NewLine);

      return output.ToString();
    }
  }
}