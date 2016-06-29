using System;
using System.Linq;
using UnityEngine;

public class PlayerStateUpdateResult : IComparable<PlayerStateUpdateResult>
{
  public readonly static PlayerStateUpdateResult Unhandled = new PlayerStateUpdateResult();

  public bool IsHandled;

  public AnimationClipInfo AnimationInfo;

  private PlayerStateUpdateResult()
  {
  }

  private PlayerStateUpdateResult(
    int animationHash,
    int animationWeight = 0,
    int[] linkedShortNameHashes = null)
  {
    AnimationInfo = new AnimationClipInfo
    {
      ShortNameHash = animationHash,
      Weight = animationWeight,
      LinkedShortNameHashes = linkedShortNameHashes
    };

    IsHandled = true;
  }

  public static PlayerStateUpdateResult CreateHandled(
    string animationName,
    int animationWeight = 0,
    string[] linkedAnimationNames = null)
  {
    return new PlayerStateUpdateResult(
      Animator.StringToHash(animationName),
      animationWeight,
      linkedAnimationNames == null
        ? null
        : linkedAnimationNames.Select(name => Animator.StringToHash(name)).ToArray());
  }

  public static PlayerStateUpdateResult CreateHandled(
    int animationHash,
    int animationWeight = 0,
    int[] linkedShortNameHashes = null)
  {
    return new PlayerStateUpdateResult(animationHash, animationWeight, linkedShortNameHashes);
  }

  public static PlayerStateUpdateResult Max(
    PlayerStateUpdateResult a,
    PlayerStateUpdateResult b,
    params PlayerStateUpdateResult[] others)
  {
    PlayerStateUpdateResult max = a.CompareTo(b) > 0 ? a : b;

    if (others != null)
    {
      for (var i = 0; i < others.Length; i++)
      {
        if (others[i].CompareTo(max) > 0)
        {
          max = others[i];
        }
      }
    }

    return max;
  }

  public int CompareTo(PlayerStateUpdateResult other)
  {
    if (IsHandled)
    {
      return other.IsHandled
        ? AnimationInfo.Weight.CompareTo(other.AnimationInfo.Weight)
        : 1;
    }

    return other.IsHandled ? -1 : 0;
  }

  public class AnimationClipInfo
  {
    public int ShortNameHash;

    public int[] LinkedShortNameHashes;

    public int Weight;
  }
}