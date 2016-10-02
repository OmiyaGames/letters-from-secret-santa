using UnityEngine;
using System.Collections;

public interface ITransitionLocation
{
    Vector2 TransitionPosition
    {
        get;
    }
    RotateEverything.Angle RotationPosition
    {
        get;
    }
}
