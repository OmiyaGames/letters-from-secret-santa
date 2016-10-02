using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Renderer))]
public class ScrollTexture : MonoBehaviour
{
    public Vector2 scrollDirection;

    Material scrollingMaterial;
    Vector2 newOffset;

	// Use this for initialization
	void Start ()
    {
        scrollingMaterial = renderer.material;
	}
	
	// Update is called once per frame
	void Update ()
    {
        // Find the new offset
        newOffset = scrollingMaterial.mainTextureOffset;
        newOffset += scrollDirection * Time.deltaTime;

        // Loop the offset values
        newOffset.x = LoopAxis(newOffset.x);
        newOffset.y = LoopAxis(newOffset.y);

        // Adjust offset
        scrollingMaterial.mainTextureOffset = newOffset;
	}

    float LoopAxis(float axis)
    {
        if (axis > 1f)
        {
            while (axis > 1f)
            {
                axis -= 1f;
            }
        }
        else if (axis < 0f)
        {
            while (axis < 0f)
            {
                axis += 1f;
            }
        }
        return axis;
    }
}
