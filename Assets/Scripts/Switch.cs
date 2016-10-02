using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(AudioMutator))]
[RequireComponent(typeof(Animator))]
public class Switch : MonoBehaviour
{
    public const string OnText = "ON";
    public const string OffText = "off";

    [Header("Common Properties")]
    public Door door;
    public string doorAnimationKey = "default";
    public bool startTriggered = false;
    public TextMesh numberIndicator;
    public float lerpColor = 10f;
    public ParticleSystem triggerParticles;

    [Header("Switch Colors")]
    public string activeField = "on";

    [Header("Sound")]
    public AudioClip onSoundEffect;
    public AudioClip offSoundEffect;

    bool isTriggered = false;
    Color switchColor, wireColor;
    AudioMutator audioCache = null;
    Animator animatorCache = null;

    public bool IsTriggered
    {
        get
        {
            return isTriggered;
        }
        set
        {
            if(isTriggered != value)
            {
                isTriggered = value;
                animatorCache.SetBool(activeField, isTriggered);

                if(isTriggered == true)
                {
                    numberIndicator.text = OnText;
                    audioCache.Audio.clip = onSoundEffect;
                    triggerParticles.Play();
                }
                else
                {
                    numberIndicator.text = OffText;
                    audioCache.Audio.clip = offSoundEffect;
                    triggerParticles.Stop();
                }
                audioCache.Play();
            }
        }
    }

    void Start()
    {
        // Grab components
        audioCache = GetComponent<AudioMutator>();
        animatorCache = GetComponent<Animator>();

        // Setup switch
        numberIndicator.text = OffText;
        animatorCache.SetBool(activeField, false);
        if (door != null)
        {
            door.AddSwitch(this);
        }

        // Set the trigger flag
        if(startTriggered == true)
        {
            TriggerSwitch();
        }
    }
    
    void OnTriggerEnter2D(Collider2D other)
    {
        if((other.CompareTag("Player") == true) && (IsTriggered == false))
        {
            TriggerSwitch();
        }
    }

    public void TriggerSwitch()
    {
        IsTriggered = true;
        if (door != null)
        {
            door.OnSwitchTriggerChanged(this);
        }
    }
}
