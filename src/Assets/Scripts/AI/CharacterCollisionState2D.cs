public struct CharacterCollisionState2D
{
  public bool Right;

  public bool Left;

  public bool Above;

  public bool Below;

  public bool BecameGroundedThisFrame;

  public bool MovingDownSlope;

  public float SlopeAngle;

  public CharacterWallState CharacterWallState;

  public bool IsFullyGrounded; // indicates whether the player is standing on an edge

  public bool WasGroundedLastFrame;

  public float LastTimeGrounded;

  public bool HasCollision()
  {
    return Below || Right || Left || Above;
  }

  public void Reset()
  {
    CharacterWallState = CharacterWallState.NotOnWall;

    Right = Left = Above = Below = BecameGroundedThisFrame = MovingDownSlope = IsFullyGrounded = false;

    SlopeAngle = 0f;
  }

  public override string ToString()
  {
    return string.Format("[CharacterCollisionState2D] r: {0}, l: {1}, a: {2}, b: {3}, movingDownSlope: {4}, angle: {5}, wasGroundedLastFrame: {6}, becameGroundedThisFrame: {7}, onWallState: {8}, ifg: {9}",
      Right, Left, Above, Below, MovingDownSlope, SlopeAngle, WasGroundedLastFrame, BecameGroundedThisFrame, CharacterWallState, IsFullyGrounded);
  }
}