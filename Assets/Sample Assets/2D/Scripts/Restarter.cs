using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(Renderer))]
public class Restarter : MonoBehaviour
{
    BoxCollider2D colliderCache = null;

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

	void OnCollisionEnter2D (Collision2D other)
	{
		if((other.collider.CompareTag("Player") == true) && (Platformer2DUserControl.CurrentMode == Platformer2DUserControl.Mode.Playing))
        {
            Platformer2DUserControl.StartRespawn();
            RotateEverything.RotateTo(Platformer2DUserControl.RespawnCheckPoint);
        }
	}

#if UNITY_EDITOR
    [Header("Editor-Only Variables")]
    public Vector2 padding = new Vector2(-0.1f, -0.3f);
    public Vector2 offset = new Vector2(0f, 0.02f);
    public ParticleSystem fire = null;

    [ContextMenu("Setup All Colliders")]
    public void SetupCentralCollider()
    {
        // Resize the box collider
        SetupTextPlatform.SetupCentralColliderStatic(CachedCollider, renderer, transform, padding, offset);
        CachedCollider.isTrigger = false;
    }
#endif
}
