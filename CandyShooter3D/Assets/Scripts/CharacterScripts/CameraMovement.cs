using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    void LateUpdate()
    {
        if (transform.localRotation.eulerAngles.x > (int)CameraRotationLimitsX.Min && transform.localRotation.eulerAngles.x < (int)CameraRotationLimitsX.Max)
        {
            transform.localRotation = Quaternion.Euler(transform.rotation.eulerAngles.x > 180 ? (int)CameraRotationLimitsX.Max : (int)CameraRotationLimitsX.Min, 0f, 0f);
        }
    }
}
public enum CameraRotationLimitsX
{
    Min = 70,
    Max = 290
}
