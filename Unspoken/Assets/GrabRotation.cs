using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrabRotation : MonoBehaviour
{
    float rotationSpeed = 1f;
    private void OnMouseDrag()
    {
        float XaxisRotaion = Input.GetAxis("Mouse x") * rotationSpeed;
        float YaxisRotaion = Input.GetAxis("Mouse Y") * rotationSpeed;

        transform.Rotate(Vector3.down, XaxisRotaion);
        transform.Rotate(Vector3.right, YaxisRotaion);
    }
}
