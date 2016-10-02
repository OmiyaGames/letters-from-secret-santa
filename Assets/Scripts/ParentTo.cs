using UnityEngine;
using System.Collections;

public class ParentTo : MonoBehaviour
{
    public Transform newParent;
    public bool snapPosition = true;
    public bool snapRotation = true;

	// Use this for initialization
	void Awake ()
    {
        if (newParent != null)
        {
            transform.parent = newParent;
            if(snapPosition == true)
            {
                transform.localPosition = Vector3.zero;
            }
            if(snapRotation == true)
            {
                transform.localRotation = Quaternion.identity;
            }
        }
    }
}
