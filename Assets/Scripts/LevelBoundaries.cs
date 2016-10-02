using UnityEngine;
using System.Collections;

[RequireComponent(typeof(BoxCollider2D))]
public class LevelBoundaries : MonoBehaviour
{
    //BoxCollider2D boxCollider = null;
    bool isOutOfBounds = false;

    void OnTriggerExit2D(Collider2D other)
    {
        if ((Platformer2DUserControl.CurrentMode == Platformer2DUserControl.Mode.Playing) && (isOutOfBounds == false) && (other.CompareTag("Player") == true))
        {
            Platformer2DUserControl.StartRespawn(respawnComplete : Reset);
            RotateEverything.RotateTo(Platformer2DUserControl.RespawnCheckPoint);
            isOutOfBounds = true;
        }
    }

    void Reset()
    {
        isOutOfBounds = false;
    }
}
