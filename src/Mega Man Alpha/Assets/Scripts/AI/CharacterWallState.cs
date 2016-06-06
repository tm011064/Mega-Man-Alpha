using System;

[Flags]
public enum CharacterWallState
{
  NotOnWall = 1,

  OnLeftWall = 2,

  OnRightWall = 4,
  
  OnWall = OnLeftWall | OnRightWall
}
