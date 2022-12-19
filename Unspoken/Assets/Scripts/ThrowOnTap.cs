//Deprecated, use BottleManager

using Niantic.ARDK.Utilities.Input.Legacy;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowOnTap : MonoBehaviour
{
    [SerializeField]
    GameObject bottlePrefab;

    [SerializeField]
    int bottleCount;
    List<GameObject> bottlePool = new List<GameObject>();

    int bottleIndex = 0;
    [SerializeField]
    float thrust = 200f;
    private void Awake()
    {
        //Object pooling
        for (int i = 0; i < bottleCount; i++)
        {
            var newBottle = Instantiate(bottlePrefab, new Vector3(0, 0, 0), Quaternion.identity);
            bottlePool.Add(newBottle);
        }
    }


    private void Update()
    {
        if (PlatformAgnosticInput.touchCount > 0)
        {
            var touch = PlatformAgnosticInput.GetTouch(0);
            if (touch.phase == TouchPhase.Began && !touch.IsTouchOverUIObject())
            {
                if (bottleIndex >= bottleCount)
                {
                    bottleIndex = 0;
                }

                var currentBottle = bottlePool[bottleIndex];
                Rigidbody currentRB = currentBottle.GetComponent<Rigidbody>();

                currentRB.velocity = new Vector3(0,0,0);
                currentBottle.transform.position = Camera.main.transform.position;
                currentBottle.transform.rotation = Camera.main.transform.rotation;
                currentRB.AddForce(currentBottle.transform.forward * thrust);

                bottleIndex++;

            }
        }
    }



}
