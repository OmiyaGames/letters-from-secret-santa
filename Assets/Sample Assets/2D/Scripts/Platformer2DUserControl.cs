using UnityEngine;

[RequireComponent(typeof(PlatformerCharacter2D))]
public class Platformer2DUserControl : MonoBehaviour 
{
    public const float SnapDistance = 0.01f;

    public enum RotateDirection
    {
        None,
        Clockwise,
        CounterClockwise
    }

    public enum Mode
    {
        Playing,
        Paused,
        Finished,
        Teleport,
        Respawn
    }
	
    public RotateEverything rotateEverything;
    public Transform footPosition;
    public float gapBetweenRotateCalls = 0.2f;
    public Transform carryPosition;
    public float teleporterSpeed = 10f;
    public CheckPoint defaultCheckPoint;
    [Header("Graphic Properties")]
    public SpriteRenderer characterRenderer;
    public ParticleSystem teleportParticles;
    [Header("Audio Properties")]
    public AudioMutator dieAudio;
    public AudioMutator teleportAudio;
    public AudioMutator gravityAudio;
    [Header("Control Properties")]
    public bool enableGravityTilting = true;

    static Platformer2DUserControl instance = null;
    Rigidbody2D playerBody = null;
    PlatformerCharacter2D character;
    bool jump;
    RotateDirection rotationDirection = RotateDirection.None;
    float timeHitRotateButton = 0;
    ItemPickup carriedItem = null;
    ItemSlot currentSlot = null;
    CheckPoint respawnCheckPoint = null;
    TeleporterPad teleportPoint = null;
    Mode currentMode = Mode.Playing;
    System.Action onRespawnComplete = null;

    public static CheckPoint RespawnCheckPoint
    {
        get
        {
            return instance.respawnCheckPoint;
        }
        set
        {
            if (instance.respawnCheckPoint != null)
            {
                instance.respawnCheckPoint.IsRespawnPoint = false;
            }
            instance.respawnCheckPoint = value;
            if (instance.respawnCheckPoint != null)
            {
                instance.respawnCheckPoint.IsRespawnPoint = true;
            }
        }
    }

    public static Transform FootPosition
    {
        get
        {
            return instance.footPosition;
        }
    }

    public static Transform CenterPosition
    {
        get
        {
            return instance.transform;
        }
    }

    public static ItemSlot CurrentSlot
    {
        get
        {
            return instance.currentSlot;
        }
        set
        {
            if (instance.currentSlot != null)
            {
                instance.currentSlot.IsInteractable = false;
            }
            instance.currentSlot = value;
            if (instance.currentSlot != null)
            {
                // Check if we're not carrying anything, and the slot contains an item
                if ((instance.carriedItem == null) && (instance.currentSlot.heldItem != null))
                {
                    instance.currentSlot.IsInteractable = true;
                }
                else if ((instance.carriedItem != null) && (instance.currentSlot.heldItem == null) && (instance.currentSlot.DoesSlotAcceptItem(instance.carriedItem) == true))
                {
                    instance.currentSlot.IsInteractable = true;
                }
            }
        }
    }

    public static Mode CurrentMode
    {
        get
        {
            return instance.currentMode;
        }
        private set
        {
            if(instance.currentMode != value)
            {
                instance.currentMode = value;
                switch(instance.currentMode)
                {
                    case Mode.Playing:
                        // Turn the character renderer on
                        instance.characterRenderer.enabled = true;

                        // Turn off the particles
                        instance.teleportParticles.Stop();
                        break;
                    case Mode.Teleport:
                    case Mode.Respawn:
                        // Turn the character renderer off
                        instance.characterRenderer.enabled = false;

                        // Turn on the particles
                        instance.teleportParticles.Play();
                        break;
                }
            }
        }
    }

    public static void TeleportTo(TeleporterPad pad)
    {
        instance.teleportPoint = pad;
        StartRespawn(false);
        CurrentMode = Mode.Teleport;
        instance.teleportAudio.Play();
    }

    public static void StartRespawn(bool playSound = true, System.Action respawnComplete = null)
    {
        if (CurrentMode == Mode.Playing)
        {
            instance.onRespawnComplete = respawnComplete;
            instance.playerBody.velocity = Vector2.zero;
            instance.playerBody.isKinematic = true;
            CurrentMode = Mode.Respawn;
            if (playSound == true)
            {
                instance.dieAudio.Play();
            }
            instance.character.runningAudio.Stop();
        }
    }

    public static void FinishLevel()
    {
        CurrentMode = Mode.Finished;
    }

	void Awake()
	{
        // Setup variables
        instance = this;

        // Get components
        character = GetComponent<PlatformerCharacter2D>();
        playerBody = GetComponent<Rigidbody2D>();
	}

    void Start()
    {
        // Setup variables
        currentSlot = null;
        RespawnCheckPoint = defaultCheckPoint;

        // Configure particles
        ParticleSystem[] allParticles = teleportParticles.GetComponentsInChildren<ParticleSystem>();
        foreach (ParticleSystem particle in allParticles)
        {
            particle.renderer.sortingLayerID = characterRenderer.sortingLayerID;
            particle.renderer.sortingOrder = characterRenderer.sortingOrder;
        }
        teleportParticles.Stop();

        // Reset the time scale
        Time.timeScale = 1;
    }

    void OnDestroy()
    {
        instance = null;
    }

    void Update ()
    {
        if (CurrentMode == Mode.Playing)
        {
            // Read the jump input in Update so button presses aren't missed.
            if (CrossPlatformInput.GetButtonDown("Jump") == true)
            {
                jump = true;
            }

            // Read in gravity input
            if (enableGravityTilting == true)
            {
                UpdateGravityRotation();
            }

            // Check if we're picking up something
            UpdateItemPickup();

            // Check if we're pausing the game
            if (CrossPlatformInput.GetButtonDown("Pause") == true)
            {
                CurrentMode = Mode.Paused;
                PauseMenu.Show(AfterPauseMenuClosed);
            }
        }
        else if (CurrentMode == Mode.Respawn)
        {
            UpdateTeleportPosition(RespawnCheckPoint);
        }
        else if (CurrentMode == Mode.Teleport)
        {
            UpdateTeleportPosition(teleportPoint);
        }
        else if (CurrentMode == Mode.Teleport)
        {
            if (CrossPlatformInput.GetButtonDown("Pause") == true)
            {
                PauseMenu.Hide();
            }
        }
    }

	void FixedUpdate()
	{
        if (CurrentMode == Mode.Playing)
        {
            float h = CrossPlatformInput.GetAxis("Horizontal");

            // Pass all parameters to the character control script.
            character.Move(h, jump);

            // Reset the jump input once it has been used.
            jump = false;

            // Check if we need to rotate the level
            if (rotationDirection != RotateDirection.None)
            {
                // Rotate the room
                rotateEverything.Rotate(rotationDirection);
                gravityAudio.Play();
                rotationDirection = RotateDirection.None;
            }
        }
    }

    void UpdateTeleportPosition(ITransitionLocation transition)
    {
        Vector2 difference = playerBody.position - transition.TransitionPosition;
        if ((difference.sqrMagnitude < (SnapDistance * SnapDistance)) && (rotateEverything.IsAnimated == false))
        {
            // Snap the character to the target position
            playerBody.position = transition.TransitionPosition;

            // Make the character dynamic again
            playerBody.isKinematic = false;

            // Reset flags
            CurrentMode = Mode.Playing;

            // Run the function pointer
            if (onRespawnComplete != null)
            {
                onRespawnComplete();
                onRespawnComplete = null;
            }
        }
        else
        {
            // Animate the character movement to the target
            playerBody.position = Vector2.Lerp(playerBody.position, transition.TransitionPosition, (Time.unscaledDeltaTime * teleporterSpeed));
        }
    }

    void UpdateItemPickup()
    {
        if ((currentSlot != null) && (currentSlot.IsInteractable == true) && (CrossPlatformInput.GetButtonDown("Pickup") == true))
        {
            if ((currentSlot.HeldItem != null) && (carriedItem == null))
            {
                // Move the slot item to the player
                carriedItem = currentSlot.HeldItem;
                currentSlot.HeldItem = null;

                // Animate carrying items
                carriedItem.AnimatePositioningItem(carryPosition, null, ItemPickup.AudioType.PickUp);
            }
            else if ((currentSlot.HeldItem == null) && (carriedItem != null))
            {
                // Moved the carried item to the slot
                currentSlot.HeldItem = carriedItem;
                carriedItem = null;
            }
        }
    }

    void UpdateGravityRotation()
    {
        if (((Time.time - timeHitRotateButton) > gapBetweenRotateCalls) && (rotateEverything.IsAnimated == false))
        {
            if (CrossPlatformInput.GetButtonDown("Rotate Clockwise") == true)
            {
                rotationDirection = RotateDirection.Clockwise;
                timeHitRotateButton = Time.time;
            }
            else if (CrossPlatformInput.GetButtonDown("Rotate CounterClockwise") == true)
            {
                rotationDirection = RotateDirection.CounterClockwise;
                timeHitRotateButton = Time.time;
            }
        }
    }

    void AfterPauseMenuClosed(PauseMenu.ClickedAction action)
    {
        if(action == PauseMenu.ClickedAction.Continue)
        {
            CurrentMode = Mode.Playing;
        }
        else
        {
            CurrentMode = Mode.Finished;
        }
    }
}
