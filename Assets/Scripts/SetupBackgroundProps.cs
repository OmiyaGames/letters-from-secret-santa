using UnityEngine;
using System.Collections;

public class SetupBackgroundProps : MonoBehaviour
{
    public string spriteLayer = "Background";

	// Use this for initialization
	void Start ()
    {
        renderer.sortingLayerName = spriteLayer;
	}
}
