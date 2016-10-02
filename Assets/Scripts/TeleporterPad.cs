using UnityEngine;
using System.Collections;

public class TeleporterPad : MonoBehaviour, ITransitionLocation
{
    public Teleporter connectedTeleporter;
    RotateEverything.Angle rotationAngle;

    public Vector2 TransitionPosition
    {
        get
        {
            return transform.position;
        }
    }

    public RotateEverything.Angle RotationPosition
    {
        get
        {
            return rotationAngle;
        }
    }

    void Awake()
    {
        rotationAngle = RotateEverything.ConvertToAngle(transform.rotation);
    }

	void OnTriggerEnter2D(Collider2D other)
    {
        if((other.CompareTag("Player") == true) && (Platformer2DUserControl.CurrentMode == Platformer2DUserControl.Mode.Playing))
        {
            connectedTeleporter.TeleportPlayer(this);
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if ((other.CompareTag("Player") == true) && (Platformer2DUserControl.CurrentMode == Platformer2DUserControl.Mode.Playing))
        {
            connectedTeleporter.ExitTeleporter(this);
        }
    }
}
