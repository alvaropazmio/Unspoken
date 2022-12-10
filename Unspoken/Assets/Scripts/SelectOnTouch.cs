using Niantic.ARDK.Utilities.Input.Legacy;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


public class SelectOnTouch : MonoBehaviour
{
    public UnityEvent OnClickedBottle;

    [SerializeField]
    private Transform bottleStorage;

    [SerializeField]
    private GameObject bottlePrefab;

    private GameObject selectedBottle;

    public float thrust;
    private void Update()
    {
        if (PlatformAgnosticInput.touchCount > 0)
        {
            var touch = PlatformAgnosticInput.GetTouch(0);
            if (touch.phase == TouchPhase.Began)
            {

                SelectARObject(touch.position);
            }
        }
    }

    void SelectARObject(Vector3 hitPosition)
    {
        var cameraTransform = Camera.main.transform;
        Ray ray = Camera.main.ScreenPointToRay(hitPosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            if (hit.collider.gameObject.tag == "Bottle")
            {
                selectedBottle = hit.collider.gameObject;
                selectedBottle.transform.position = bottleStorage.transform.position;
                selectedBottle.transform.parent = bottleStorage;
                OnClickedBottle.Invoke();
            }
        }
    }

    public void ThrowBottleBack()
    {
        if (selectedBottle == null)
        {
            var bottle = Instantiate(bottlePrefab, Vector3.zero, Quaternion.identity);
            ThrowBottle(bottle);
        }
        else
        {
            ThrowBottle(selectedBottle);
            selectedBottle = null;
        }

    }

    private void ThrowBottle(GameObject bottle)
    {
        bottle.transform.parent = null;
        bottle.transform.position = Camera.main.transform.position;
        bottle.transform.rotation = Quaternion.identity;
        
        var bottleRigidBody = bottle.GetComponent<Rigidbody>();
        bottleRigidBody.velocity = Vector3.zero;
        bottleRigidBody.AddForce(Camera.main.transform.forward * thrust);
        
    }
}
