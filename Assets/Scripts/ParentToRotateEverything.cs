using UnityEngine;
using System.Collections;

public class ParentToRotateEverything : MonoBehaviour
{
	// Use this for initialization
	void Start ()
    {
        if (enabled == true)
        {
            transform.parent = RotateEverything.RotateTransform;
        }
	}
}
