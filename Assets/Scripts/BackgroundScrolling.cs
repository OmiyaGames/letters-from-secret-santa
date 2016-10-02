using UnityEngine;
using System.Collections;

public class BackgroundScrolling : MonoBehaviour
{
    public Transform follow;
    public Vector2 shiftBackgroundBy = new Vector2(0.01f, 0.01f);

    Vector3 meshPosition, diffPosition;
    Vector2 backgroundOffset;
    Material backgroundMaterial;

    void Start()
    {
        meshPosition = transform.position;
        backgroundMaterial = renderer.material;
    }

	// Update is called once per frame
	void LateUpdate ()
    {
        meshPosition.x = follow.position.x;
        meshPosition.y = follow.position.y;

        // Calculate offset
        diffPosition = meshPosition - transform.position;
        backgroundOffset.x += diffPosition.x * shiftBackgroundBy.x;
        backgroundOffset.y += diffPosition.y * shiftBackgroundBy.y;

        // Normalize texture coordinates
        while (backgroundOffset.x < 0)
        {
            backgroundOffset.x += 1;
        }
        while (backgroundOffset.x > 1)
        {
            backgroundOffset.x -= 1;
        }
        while (backgroundOffset.y < 0)
        {
            backgroundOffset.y += 1;
        }
        while (backgroundOffset.y > 1)
        {
            backgroundOffset.y -= 1;
        }

        // Set the offset
        backgroundMaterial.mainTextureOffset = Quaternion.Euler(0, 0, (360 - (int)RotateEverything.TargetAngle)) * backgroundOffset;

        // Update the mesh position
        transform.position = meshPosition;
    }
}
