using UnityEngine;
using System.Collections;

[RequireComponent(typeof(TextMesh))]
[RequireComponent(typeof(BoxCollider2D))]
public class SetupTextPlatform : MonoBehaviour
{
    public BoxCollider2D topSlider;
    public BoxCollider2D bottomSlider;
    public BoxCollider2D leftSlider;
    public BoxCollider2D rightSlider;

    BoxCollider2D colliderCache = null;

    public float Rotation
    {
        get
        {
            return transform.rotation.eulerAngles.z;
        }
    }

    public BoxCollider2D CachedCollider
    {
        get
        {
            if(colliderCache == null)
            {
                colliderCache = GetComponent<BoxCollider2D>();
            }
            return colliderCache;
        }
    }

    public virtual void Start()
    {
        UpdateSliderActivation();
        RotateEverything.AddPlatform(this);
    }

    public virtual void UpdateSliderActivation()
    {
        // enable the main collider
        CachedCollider.enabled = true;

        float rotationDecimal = Rotation / 180f;
        rotationDecimal -= Mathf.Floor(rotationDecimal);
        rotationDecimal = Mathf.Abs(rotationDecimal);

        bool vertical = ((rotationDecimal > 0.25f) && (rotationDecimal < 0.75f));

        topSlider.gameObject.SetActive(vertical == true);
        bottomSlider.gameObject.SetActive(vertical == true);
        leftSlider.gameObject.SetActive(vertical == false);
        rightSlider.gameObject.SetActive(vertical == false);
    }

#if UNITY_EDITOR
    [Header("Editor-Only Variables")]
	public float sliderWidth = 0.2f;
    public Vector2 padding = new Vector2(-0.1f, -0.3f);
    public Vector2 offset = new Vector2(0f, 0.02f);

	[ContextMenu("Setup All Colliders")]
    public void Setup()
	{
		// Resize the box collider
		SetupCentralCollider();

		// Resize the sliders
		SetupEdgeCollider();
	}

	[ContextMenu("Setup Central Collider")]
	public void SetupCentralCollider()
	{
		// Resize the box collider
        SetupCentralColliderStatic(CachedCollider, GetComponent<Renderer>(), transform, padding, offset);
	}

    [ContextMenu("Setup Edge Collider")]
    public void SetupEdgeCollider()
    {
        SetupEdgeCollidersStatic(CachedCollider, topSlider, bottomSlider, leftSlider, rightSlider, sliderWidth);
    }

    public static void SetupCentralColliderStatic(BoxCollider2D centralCollider, Renderer textRenderer, Transform textTransform, Vector2 colliderPadding, Vector2 colliderOffset)
    {
        Vector3 rendererSize = /*transform.rotation * */textRenderer.bounds.size;
        Vector3 rendererExtents = /*transform.rotation * */textRenderer.bounds.extents;

        // Resize the box collider
        centralCollider.offset = new Vector3((rendererExtents.x - (rendererSize.x / 2f)) + colliderOffset.x,
                                         (rendererExtents.y - (rendererSize.y / 2f)) + colliderOffset.y,
                                         textTransform.position.z);
        centralCollider.size = new Vector3(rendererSize.x + colliderPadding.x, rendererSize.y + colliderPadding.y, 1);
    }

    public static void SetupEdgeCollidersStatic(BoxCollider2D centralCollider, BoxCollider2D topSlider, BoxCollider2D bottomSlider, BoxCollider2D leftSlider, BoxCollider2D rightSlider, float sliderWidth)
    {
        // Resize the sliders
        float sliderDimension = centralCollider.size.y;
        leftSlider.offset = centralCollider.offset;
        leftSlider.size = new Vector2(sliderWidth, sliderDimension);
        rightSlider.offset = leftSlider.offset;
        rightSlider.size = leftSlider.size;

        // Position the edge sliders
        float platformHalfDimension = centralCollider.size.x / 2f;
        leftSlider.transform.localPosition = new Vector3(-platformHalfDimension, 0, 0);
        rightSlider.transform.localPosition = new Vector3(platformHalfDimension, 0, 0);

        // Resize the sliders
        sliderDimension = centralCollider.size.x;
        topSlider.offset = centralCollider.offset;
        topSlider.size = new Vector2(sliderDimension, sliderWidth);
        bottomSlider.offset = topSlider.offset;
        bottomSlider.size = topSlider.size;

        // Position the edge sliders
        platformHalfDimension = centralCollider.size.y / 2f;
        topSlider.transform.localPosition = new Vector3(0, -platformHalfDimension, 0);
        bottomSlider.transform.localPosition = new Vector3(0, platformHalfDimension, 0);
    }
#endif
}
