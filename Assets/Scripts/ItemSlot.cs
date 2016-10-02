using UnityEngine;
using System.Collections;

[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(TextMesh))]
[RequireComponent(typeof(Renderer))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(AudioMutator))]
public class ItemSlot : MonoBehaviour
{
    public ItemPickup heldItem = null;
    public bool isGiftSlot = false;
    public float fadeOutDuration = 0.25f;
    public string interactableField = "interactable";
    public ParticleSystem smokeParticles = null;
    [Header("Door Properties")]
    public Door optionalDoor = null;

    BoxCollider2D colliderCache = null;
    TextMesh slotLabel = null;
    float timeFadeOutStarted = -1;
    Color fontColor;
    bool isInteractable = false;
    Animator interactableIndicator = null;
    AudioMutator audioMutator = null;

    public BoxCollider2D CachedCollider
    {
        get
        {
            if (colliderCache == null)
            {
                colliderCache = GetComponent<BoxCollider2D>();
            }
            return colliderCache;
        }
    }

    public ItemPickup HeldItem
    {
        get
        {
            return heldItem;
        }
        set
        {
            if(heldItem != value)
            {
                // Check if this slot was carrying an item
                if(heldItem != null)
                {
                    // If so, change the item state
                    heldItem.CurrentState = ItemPickup.State.Carried;
                }
                
                // Set the currently held item
                heldItem = value;

                // Check if the slot is carrying an item now
                UpdateSlot(true);
            }
        }
    }

    public bool IsInteractable
    {
        get
        {
            return isInteractable;
        }
        set
        {
            if (isInteractable != value)
            {
                isInteractable = value;
                interactableIndicator.SetBool(interactableField, isInteractable);
                if (isInteractable == true)
                {
                    slotLabel.fontStyle = FontStyle.Italic;
                    if (heldItem != null)
                    {
                        heldItem.LabelStyle = FontStyle.Italic;
                    }
                    audioMutator.Play();
                }
                else
                {
                    slotLabel.fontStyle = FontStyle.Normal;
                    if (heldItem != null)
                    {
                        heldItem.LabelStyle = FontStyle.Normal;
                    }
                    audioMutator.Stop();
                }
            }
        }
    }

    public bool DoesSlotAcceptItem(ItemPickup pickup)
    {
        bool returnFlag = false;
        if((HeldItem == null) && (pickup != null) && (pickup.Text.Length == slotLabel.text.Length))
        {
            returnFlag = true;
        }
        return returnFlag;
    }

	// Use this for initialization
	void Start ()
    {
        // Grab other components
        slotLabel = GetComponent<TextMesh>();
        interactableIndicator = GetComponent<Animator>();
        audioMutator = GetComponent<AudioMutator>();

        // Grab the label color
        fontColor = slotLabel.color;

        // Update the door
        if(optionalDoor != null)
        {
            optionalDoor.AddItemSlot(this);
        }

        // Update the item
        UpdateSlot(false);
    }

    void Update()
    {
        if(timeFadeOutStarted > 0)
        {
            // Check if the duration is up
            if((Time.time - timeFadeOutStarted) > fadeOutDuration)
            {
                // If so, flag that the slot animation is finished
                timeFadeOutStarted = -1;

                // Set the font color
                if(heldItem == null)
                {
                    // Make the font label completely opaque
                    GetComponent<Renderer>().enabled = true;
                    fontColor.a = 1;
                    slotLabel.color = fontColor;
                }
                else
                {
                    // Don't show the label
                    GetComponent<Renderer>().enabled = false;
                    fontColor.a = 0;
                    slotLabel.color = fontColor;
                }
            }
            else
            {
                float ratio = Mathf.Clamp01((Time.time - timeFadeOutStarted) / fadeOutDuration);
                if (heldItem == null)
                {
                    fontColor.a = ratio;
                }
                else
                {
                    if ((smokeParticles.isPlaying == false) && (ratio > 0.5f))
                    {
                        smokeParticles.Play();
                    }
                    fontColor.a = 1f - ratio;
                }
                slotLabel.color = fontColor;
            }
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if(other.CompareTag("Player") == true)
        {
            Platformer2DUserControl.CurrentSlot = this;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if ((other.CompareTag("Player") == true) && (Platformer2DUserControl.CurrentSlot == this))
        {
            Platformer2DUserControl.CurrentSlot = null;
        }
    }

    void UpdateSlot(bool animateItem)
    {
        // Check if an item is held
        smokeParticles.Stop();
        fontColor.a = 1;
        if (heldItem != null)
        {
            // Update the state of the item
            heldItem.CurrentState = ItemPickup.State.Animating;

            // Snap the item to this slot's transform
            if (animateItem == true)
            {
                heldItem.AnimatePositioningItem(transform, UpdateItem, ItemPickup.AudioType.Place);
            }
            else
            {
                heldItem.SnapItemToTransform(transform, UpdateItem);
            }
            fontColor.a = 0;
        }

        // Check if the renderer should be turned off
        if ((heldItem != null) && (animateItem == false))
        {
            // Turn off the mesh renderer
            GetComponent<Renderer>().enabled = false;
            timeFadeOutStarted = -1;
        }
        else
        {
            // Turn on the text mesh renderer
            GetComponent<Renderer>().enabled = true;
            timeFadeOutStarted = Time.time;
            if ((heldItem == null) && (animateItem == false))
            {
                timeFadeOutStarted = -1;
            }
        }

        if (optionalDoor != null)
        {
            optionalDoor.OnItemSlotChanged(this);
        }

        // Update the font color
        slotLabel.color = fontColor;
    }

    void UpdateItem(ItemPickup pickup)
    {
        if(pickup == heldItem)
        {
            if (isGiftSlot == true)
            {
                heldItem.CurrentState = ItemPickup.State.Gift;
            }
            else
            {
                heldItem.CurrentState = ItemPickup.State.Platform;
                heldItem.UpdateSliderActivation();
            }
        }
    }

#if UNITY_EDITOR
    [Header("Editor-Only Variables")]
    public Vector2 padding = new Vector2(-0.1f, -0.3f);
    public Vector2 offset = new Vector2(0f, 0.02f);

    [ContextMenu("Setup All Colliders")]
    void SetupCentralCollider()
    {
        // Resize the box collider
        SetupTextPlatform.SetupCentralColliderStatic(CachedCollider, GetComponent<Renderer>(), transform, padding, offset);
        CachedCollider.isTrigger = true;
    }
#endif
}
