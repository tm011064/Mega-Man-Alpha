﻿using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Assets.Editor.Tiled
{
  public class VertexBuilder
  {
    private Matrix<int> _matrix;

    private Vertex[] _vertices;

    public VertexBuilder(Matrix<int> matrix)
    {
      _matrix = matrix;

      var totalVertices = (_matrix.Rows + 1) * (_matrix.Columns + 1);

      _vertices = new Vertex[totalVertices];
    }

    public void Build(int tileWidth, int tileHeight)
    {
      InitializeVertices(tileWidth, tileHeight);

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

    private void InitializeVertices(int tileWidth, int tileHeight)
    {
      var index = 0;

      for (var i = 0; i <= _matrix.Rows; i++)
      {
        for (var j = 0; j <= _matrix.Columns; j++)
        {
          _vertices[index++] = new Vertex(
            new Vector2(
              tileWidth * j,
              tileHeight * i));
        }
      }
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

    public IEnumerable<Vector2[]> GetColliderEdges()
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

          if (vertex == startVertex)
          {
            break;
          }

          vertexPoints.Add(vertex.Point);
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