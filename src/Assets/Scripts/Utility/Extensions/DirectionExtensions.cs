using System;

public static class DirectionExtensions
{
  public static Direction RotateAntiClockwise(this Direction direction)
  {
    switch (direction)
    {
      case Direction.Right: return Direction.Up;
      case Direction.Down: return Direction.Right;
      case Direction.Left: return Direction.Down;
      case Direction.Up: return Direction.Left;
    }

    throw new NotSupportedException();
  }

  public static Direction RotateClockwise(this Direction direction)
  {
    switch (direction)
    {
      case Direction.Right: return Direction.Down;
      case Direction.Down: return Direction.Left;
      case Direction.Left: return Direction.Up;
      case Direction.Up: return Direction.Right;
    }

    throw new NotSupportedException();
  }

  public static Direction Reverse(this Direction direction)
  {
    switch (direction)
    {
      case Direction.Right: return Direction.Left;
      case Direction.Down: return Direction.Up;
      case Direction.Left: return Direction.Right;
      case Direction.Up: return Direction.Down;
    }

    throw new NotSupportedException();
  }
}