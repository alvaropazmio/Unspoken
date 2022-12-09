using Niantic.ARDK.Utilities.Input.Legacy;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


public class SelectOnTouch : MonoBehaviour
{
    public UnityEvent OnClickedBottle;

    private void Update()
    {
        if (PlatformAgnosticInput.touchCount > 0)
        {
            var touch = PlatformAgnosticInput.GetTouch(0);
            if (touch.phase == TouchPhase.Began)
            {

                selectARObject(touch.position);
            }
        }
    }

    void selectARObject(Vector3 hitPosition)
    {
        var cameraTransform = Camera.main.transform;
        Ray ray = Camera.main.ScreenPointToRay(hitPosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            if (hit.collider.gameObject.tag == "Bottle")
            {
                OnClickedBottle.Invoke();
            }
        }
    }
}
