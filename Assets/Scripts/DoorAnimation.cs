using UnityEngine;
using System.Collections;

public class DoorAnimation : MonoBehaviour
{
    public string doorAnimationKey = "default";
    [Header("Transform")]
    public ParentToRotateEverything affectedTransform = null;
    [Header("Audio")]
    public AudioMutator audioScript = null;
    [Header("Animation")]
    public float animationDuration = 0.5f;
    public GoEaseType animationType = GoEaseType.Linear;
    public bool loopForever = false;
    public GoLoopType loopType = GoLoopType.PingPong;
    [Header("Hacks")]
    public bool disableParentingAutomatically = true;

    GoTween itemAnimation = null;
    readonly GoTweenConfig itemAnimationConfiguration = new GoTweenConfig();

#if UNITY_EDITOR
    public Color gizmoColor = Color.cyan;
    public Color triggerColor = Color.cyan;
#endif

    SetupTextPlatform[] affectedPlatforms = null;

    void Awake()
    {
        if (affectedTransform == null)
        {
            Debug.LogError("affectedTransform must be filled in.");
        }

        // Grab all platforms
        affectedPlatforms = affectedTransform.GetComponentsInChildren<SetupTextPlatform>();

        // Setup animation configuration
        itemAnimationConfiguration.setIterations((loopForever ? -1 : 1), loopType);
        itemAnimationConfiguration.setEaseType(animationType);

        // Setup scale and rotation
        itemAnimationConfiguration.localRotation(Quaternion.identity);
        itemAnimationConfiguration.scale(Vector3.one);
        itemAnimationConfiguration.localPosition(Vector3.zero);

        // Disable all parenting scripts
        if(disableParentingAutomatically == true)
        {
            ParentToRotateEverything[] allParentScripts = affectedTransform.GetComponentsInChildren<ParentToRotateEverything>();
            foreach (ParentToRotateEverything parentScript in allParentScripts)
            {
                parentScript.enabled = false;
            }
        }
    }

#if UNITY_EDITOR
    void OnDrawGizmos()
    {
        if (affectedTransform != null)
        {
            // Setup Gizmos
            Gizmos.color = gizmoColor;
            Gizmos.matrix = transform.localToWorldMatrix;

            SetupTextPlatform[] allPlatforms = affectedTransform.GetComponentsInChildren<SetupTextPlatform>();
            Vector3 platformColliderPosition;
            foreach(SetupTextPlatform platform in allPlatforms)
            {
                // Draw the location
                platformColliderPosition = affectedTransform.transform.InverseTransformPoint(platform.transform.position);
                platformColliderPosition.x += platform.CachedCollider.center.x;
                platformColliderPosition.y += platform.CachedCollider.center.y;
                Gizmos.DrawWireCube(platformColliderPosition, platform.CachedCollider.size);
            }
            Restarter[] allDeathTraps = affectedTransform.GetComponentsInChildren<Restarter>();
            foreach (Restarter deathTraps in allDeathTraps)
            {
                // Draw the location
                platformColliderPosition = affectedTransform.transform.InverseTransformPoint(deathTraps.transform.position);
                platformColliderPosition.x += deathTraps.CachedCollider.center.x;
                platformColliderPosition.y += deathTraps.CachedCollider.center.y;
                Gizmos.DrawWireCube(platformColliderPosition, deathTraps.CachedCollider.size);
            }

            Gizmos.color = triggerColor;
            ItemSlot[] allSlots = affectedTransform.GetComponentsInChildren<ItemSlot>();
            foreach (ItemSlot slots in allSlots)
            {
                // Draw the location
                platformColliderPosition = affectedTransform.transform.InverseTransformPoint(slots.transform.position);
                platformColliderPosition.x += slots.CachedCollider.center.x;
                platformColliderPosition.y += slots.CachedCollider.center.y;
                Gizmos.DrawWireCube(platformColliderPosition, slots.CachedCollider.size);
            }

            // Draw the direction
            Gizmos.DrawLine(Vector3.zero, Vector3.up);

            // Reset Gizmos
            Gizmos.matrix = Matrix4x4.identity;
        }
    }
#endif

    public void RunAnimation(Door door, Switch trigger)
    {
        RunAnimation();
    }

    public void RunAnimation(Door door, ItemSlot trigger)
    {
        RunAnimation();
    }

    void RunAnimation()
    {
        // Check if we need to clean up the animation
        if (itemAnimation != null)
        {
            // Clean up this item animation from the animation queue
            Go.removeTween(itemAnimation);
            itemAnimation = null;
        }

        // Create and play a new animation
        affectedTransform.transform.parent = transform;
        itemAnimation = Go.to(affectedTransform.transform, animationDuration, itemAnimationConfiguration);
        itemAnimation.setOnCompleteHandler(UpdatePlatform);
        itemAnimation.play();

        // Play audio
        audioScript.Play();
    }

    void UpdatePlatform(AbstractGoTween animation)
    {
        foreach(SetupTextPlatform platform in affectedPlatforms)
        {
            platform.UpdateSliderActivation();
        }
    }
}
