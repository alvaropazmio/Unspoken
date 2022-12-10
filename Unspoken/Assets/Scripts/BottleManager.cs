//Under construction, manager script merging the SelectOnTouch and the ThrowOnTap scripts

using Niantic.ARDK.Utilities.Input.Legacy;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class BottleManager : MonoBehaviour
{
    public UnityEvent OnClikcedBottle;

    [SerializeField]
    private GameObject bottlePrefab;
    [SerializeField]
    private Transform bottleStorage;
    private GameObject selectedBottle;

    [SerializeField]
    int bottleCount;

    List<GameObject> bottlePool = new List<GameObject>();

    [SerializeField]
    float thrust;

    int bottleIndex = 0;

    private void Awake()
    {
        for (int i = 0; i < bottleCount; i++)
        {
            var newBottle = Instantiate(bottlePrefab, Vector3.zero, Quaternion.identity);
            bottlePool.Add(newBottle);
        }
    }

    private void Update()
    {
        
    }

    private void CheckForTouch()
    {
        if (PlatformAgnosticInput.touchCount <= 0) return;

        var touch = PlatformAgnosticInput.GetTouch(0);


        if (touch.phase == TouchPhase.Began && !touch.IsTouchOverUIObject())
        {
            Ray ray = Camera.main.ScreenPointToRay(touch.position);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider.gameObject.tag == "Bottle")
                {
                    selectedBottle = hit.collider.gameObject;
                    selectedBottle.transform.position = bottleStorage.transform.position;
                    selectedBottle.transform.parent = bottleStorage;
                    OnClikcedBottle.Invoke(); //prety sure not correct
                }
            }
            else ThrowBottle(Instantiate(bottlePrefab, Vector3.zero, Quaternion.identity)); //not sure if correct
            
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
