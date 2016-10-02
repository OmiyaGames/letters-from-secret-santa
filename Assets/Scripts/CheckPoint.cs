using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(ParentToRotateEverything))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(AudioMutator))]
public class CheckPoint : MonoBehaviour, ITransitionLocation
{
    public string activeField = "active";

    RotateEverything.Angle rotationAngle;
    Animator animator;
    AudioMutator audioPlayer;
    bool isRespawnPoint = true;

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

    public bool IsRespawnPoint
    {
        get
        {
            return isRespawnPoint;
        }
        set
        {
            if (isRespawnPoint != value)
            {
                isRespawnPoint = value;

                // Update the animator
                if (animator == null)
                {
                    animator = GetComponent<Animator>();
                }
                animator.SetBool(activeField, isRespawnPoint);

                // Update the audio player
                if (isRespawnPoint == true)
                {
                    if (audioPlayer == null)
                    {
                        audioPlayer = GetComponent<AudioMutator>();
                    }
                    audioPlayer.Play();
                }
            }
        }
    }
    
    void Awake()
    {
        isRespawnPoint = true;
        IsRespawnPoint = false;
        rotationAngle = RotateEverything.ConvertToAngle(transform.rotation);
    }

    void OnTriggerEnter2D(Collider2D playerCollider)
    {
        if((playerCollider.CompareTag("Player") == true) &&
            (Platformer2DUserControl.CurrentMode == Platformer2DUserControl.Mode.Playing)
            && (Platformer2DUserControl.RespawnCheckPoint != this))
        {
            Platformer2DUserControl.RespawnCheckPoint = this;
        }
    }
}
