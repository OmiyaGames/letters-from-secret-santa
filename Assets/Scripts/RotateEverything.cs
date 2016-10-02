using UnityEngine;
using System.Collections.Generic;

public class RotateEverything : MonoBehaviour
{
    public const float SnapTargetAngle = 45f;
    public static readonly Vector3 RotateAxis = Vector3.forward;
    static RotateEverything instance;

    public enum Angle
    {
        RightSideUp = 0,
        Degrees90 = 90,
        UpsideDown = 180,
        Degrees270 = 270
    }

    public static Angle ConvertToAngle(Quaternion rotation)
    {
        return ConvertToAngle(rotation.eulerAngles.z);
    }

    public static Angle ConvertToAngle(float rotation)
    {
        int angle = Mathf.RoundToInt(rotation / 90f) * 90;
        while(angle < 0)
        {
            angle += 360;
        }
        while (angle > 270)
        {
            angle -= 360;
        }
        angle = 360 - angle;
        if(angle > 270)
        {
            angle = 0;
        }
        return (Angle)angle;
    }

    public static Transform RotateTransform
    {
        get
        {
            return instance.transform;
        }
    }

    public static void AddPlatform(SetupTextPlatform platform)
    {
        if(platform != null)
        {
            instance.platformList.Add(platform);
        }
    }

    public float rotateSpeed = 5f;
    public float snapAngleDifference = 0.5f;
    public float reduceTimeScaleTo = 0.01f;

    Angle targetAngle;
    Quaternion targetAngleQuaternion, newAngle;
    float angleDifference;
    readonly List<SetupTextPlatform> platformList = new List<SetupTextPlatform>();

    public bool IsAnimated
    {
        get
        {
            return enabled;
        }
        set
        {
            if(enabled != value)
            {
                enabled = value;
                if(enabled == false)
                {
                    Time.timeScale = 1;
                }
            }
        }
    }

    public float CurrentAngle
    {
        get
        {
            return transform.rotation.eulerAngles.z;
        }
    }

    public static Angle TargetAngle
    {
        get
        {
            return instance.targetAngle;
        }
        private set
        {
            if (instance.targetAngle != value)
            {
                instance.targetAngle = value;
                instance.targetAngleQuaternion = Quaternion.Euler(0, 0, (int)instance.targetAngle);
            }
        }
    }

	// Use this for initialization
	void Awake()
    {
	    instance = this;
        transform.parent = null;
        enabled = false;
	}

    void OnDestroy()
    {
        instance = null;
    }

    void Update()
    {
        if(IsAnimated == true)
        {
            angleDifference = CurrentAngle;
            newAngle = Quaternion.Lerp(transform.rotation, targetAngleQuaternion, (rotateSpeed * Time.unscaledDeltaTime));
            if (Mathf.Abs(newAngle.eulerAngles.z - (int)TargetAngle) < snapAngleDifference)
            {
                newAngle = targetAngleQuaternion;
                AfterRotate();
                IsAnimated = false;
            }
            transform.RotateAround(Platformer2DUserControl.FootPosition.position, RotateAxis, (newAngle.eulerAngles.z - angleDifference));
        }
    }

    public void Rotate(Platformer2DUserControl.RotateDirection direction)
    {
        // allow rotation animation, and stop time!
        if(direction == Platformer2DUserControl.RotateDirection.Clockwise)
        {
            if(TargetAngle == Angle.Degrees270)
            {
                TargetAngle = Angle.RightSideUp;
            }
            else
            {
                TargetAngle = (Angle)(((int)TargetAngle) + 90);
            }
            Time.timeScale = reduceTimeScaleTo;
            IsAnimated = true;
        }
        else if(direction == Platformer2DUserControl.RotateDirection.CounterClockwise)
        {
            if(TargetAngle == Angle.RightSideUp)
            {
                TargetAngle = Angle.Degrees270;
            }
            else
            {
                TargetAngle = (Angle)(((int)TargetAngle) - 90);
            }
            Time.timeScale = reduceTimeScaleTo;
            IsAnimated = true;
        }
    }

    public static void RotateTo(ITransitionLocation transition)
    {
        if (transition != null)
        {
            // Determine the target angle
            TargetAngle = transition.RotationPosition;
            instance.IsAnimated = true;
        }
    }

    void AfterRotate()
    {
        for(int index = 0; index < platformList.Count; ++index)
        {
            platformList[index].UpdateSliderActivation();
        }
    }
}
