using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(AudioMutator))]
[RequireComponent(typeof(ParentToRotateEverything))]
public class GoalTrigger : MonoBehaviour
{
    [Header("Sound Effects")]
    public AudioClip enter;

    AudioMutator mutator = null;
    RotateEverything.Angle goalOrientation;

    void Start()
    {
        mutator = GetComponent<AudioMutator>();
        goalOrientation = RotateEverything.ConvertToAngle(transform.rotation);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if((other.CompareTag("Player") == true) && (Platformer2DUserControl.CurrentMode == Platformer2DUserControl.Mode.Playing) && (RotateEverything.TargetAngle == goalOrientation))
        {
            // Indicate the level is finished
            Platformer2DUserControl.FinishLevel();

            // Play audio
            mutator.Audio.clip = enter;
            mutator.Play();

            // Transition to the next level
            SceneTransition transition = Singleton.Get<SceneTransition>();
            if(Application.loadedLevel >= GameSettings.NumLevels)
            {
                transition.LoadLevel(GameSettings.MenuLevel);
            }
            else
            {
                transition.LoadLevel(Application.loadedLevel + 1);
            }
        }
    }
}
