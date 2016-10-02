using UnityEngine;
using System.Collections;

[RequireComponent(typeof(LineRenderer))]
public class Teleporter : MonoBehaviour
{
    public TeleporterPad pad1;
    public TeleporterPad pad2;
    public float tilePerDistance = 1;
    public float coolDownDuration = 1f;

    LineRenderer lineRenderer;
    Material lineMaterial;
    Vector2 lineMaterialTiling, padDistance;
    TeleporterPad lastEnteredPad = null;
    float lastTeleported = -1;

    void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineMaterial = lineRenderer.material;
        lineMaterialTiling = lineMaterial.mainTextureScale;
    }

	public void TeleportPlayer(TeleporterPad pad)
    {
        if((lastEnteredPad == null) && ((lastTeleported < 0) || ((Time.time - lastTeleported) > coolDownDuration)))
        {
            // FIXME: animated teleportation
            if(pad1 == pad)
            {
                lastEnteredPad = pad1;
                Platformer2DUserControl.TeleportTo(pad2);
                RotateEverything.RotateTo(pad2);
            }
            else if(pad2 == pad)
            {
                lastEnteredPad = pad2;
                Platformer2DUserControl.TeleportTo(pad1);
                RotateEverything.RotateTo(pad1);
            }
        }
    }

    public void ExitTeleporter(TeleporterPad pad)
    {
        if(lastEnteredPad != null)
        {
            if((pad1 == pad) && (lastEnteredPad == pad2))
            {
                lastEnteredPad = null;
                lastTeleported = Time.time;
            }
            else if((pad2 == pad) && (lastEnteredPad == pad1))
            {
                lastEnteredPad = null;
                lastTeleported = Time.time;
            }
        }
    }

    void Update()
    {
        lineRenderer.SetPosition(0, pad1.transform.position);
        lineRenderer.SetPosition(1, pad2.transform.position);

        // Adjust line renderer material
        padDistance = pad1.transform.position - pad2.transform.position;
        lineMaterialTiling.x = padDistance.magnitude * tilePerDistance;
        lineMaterial.mainTextureScale = lineMaterialTiling;
    }
}
