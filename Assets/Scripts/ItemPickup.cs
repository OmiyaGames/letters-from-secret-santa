using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(TextMesh))]
[RequireComponent(typeof(AudioMutator))]
public class ItemPickup : SetupTextPlatform
{
    public enum State
    {
        Animating,
        Platform,
        Gift,
        Carried
    }

    public enum AudioType
    {
        None,
        PickUp,
        Place
    }

    [Header("Item Properties")]
    public State startingState = State.Platform;
    public float animationDuration = 0.25f;
    public float midTweenOffset = 5;
    public GoEaseType animationType = GoEaseType.Linear;
    [Header("Audio Clips")]
    public AudioClip pickupSound;
    public AudioClip dropSound;
    
    State currentState;
    TextMesh pickupLabel = null;
    GoTween itemAnimation = null;
    System.Action<ItemPickup> onAnimationEnd = null;
    AudioMutator audioPlayer = null;
    readonly GoTweenConfig itemAnimationConfiguration = new GoTweenConfig();
    readonly Vector3[] itemTweenPath = new Vector3[] { Vector3.zero, Vector3.zero, Vector3.zero };

    public State CurrentState
    {
        get
        {
            return currentState;
        }
        set
        {
            if(currentState != value)
            {
                currentState = value;
                UpdateSliderActivation();
            }
        }
    }

    public string Text
    {
        get
        {
            return pickupLabel.text;
        }
        set
        {
            pickupLabel.text = value;
        }
    }

    public FontStyle LabelStyle
    {
        get
        {
            return pickupLabel.fontStyle;
        }
        set
        {
            pickupLabel.fontStyle = value;
        }
    }

    void PlayClip(AudioClip clip)
    {
        if(audioPlayer == null)
        {
            audioPlayer = GetComponent<AudioMutator>();
        }
        audioPlayer.Audio.clip = clip;
        audioPlayer.Play();
    }

    public override void Start()
    {
        base.Start();
        currentState = startingState;
        pickupLabel = GetComponent<TextMesh>();

        // Setup animation configuration
        itemAnimationConfiguration.setIterations(1, GoLoopType.RestartFromBeginning);
        itemAnimationConfiguration.setEaseType(animationType);
    }

    public void AnimatePositioningItem(Transform parentTo, System.Action<ItemPickup> endAnimationEvent, AudioType playSound = AudioType.None)
    {
        // Setup variables
        onAnimationEnd = endAnimationEvent;
        transform.parent = parentTo;

        // Setup animation configuration
        itemAnimationConfiguration.clearEvents();
        itemAnimationConfiguration.clearProperties();

        // Setup scale and rotation
        itemAnimationConfiguration.localRotation(Quaternion.identity);
        itemAnimationConfiguration.scale(Vector3.one);

        // Setup path
        itemTweenPath[0] = transform.localPosition;
        itemTweenPath[1] = new Vector3((transform.localPosition.x / 2f), midTweenOffset, 0);
        itemAnimationConfiguration.localPositionPath(new GoSpline(itemTweenPath));

        // Check if we need to clean up the animation
        if (itemAnimation != null)
        {
            // Clean up this item animation from the animation queue
            Go.removeTween(itemAnimation);
            itemAnimation = null;
        }

        // Create and play a new animation
        itemAnimation = Go.to(transform, animationDuration, itemAnimationConfiguration);
        itemAnimation.setOnCompleteHandler(OnAnimationEnds);
        itemAnimation.play();

        // Play a sound effect
        if(playSound == AudioType.Place)
        {
            PlayClip(dropSound);
        }
        else if(playSound == AudioType.PickUp)
        {
            PlayClip(pickupSound);
        }
    }

    public void SnapItemToTransform(Transform parentTo, System.Action<ItemPickup> endAnimationEvent)
    {
        AnimatePositioningItem(parentTo, endAnimationEvent);
        itemAnimation.complete();
    }

    public override void UpdateSliderActivation()
    {
        if(currentState == State.Platform)
        {
            base.UpdateSliderActivation();
        }
        else
        {
            // disable the main collider
            CachedCollider.enabled = false;

            // deactivate all sliders
            topSlider.gameObject.SetActive(false);
            bottomSlider.gameObject.SetActive(false);
            leftSlider.gameObject.SetActive(false);
            rightSlider.gameObject.SetActive(false);
        }
    }

    void OnAnimationEnds(AbstractGoTween tween)
    {
        if ((tween == itemAnimation) && (onAnimationEnd != null))
        {
            // Call the stored delegate
            onAnimationEnd(this);
            onAnimationEnd = null;
        }
    }
}
