using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IgnoreLocalRotation : MonoBehaviour
{

    // Update is called once per frame
    void FixedUpdate()
    {
        //transform.rotation = Quaternion.Euler(new Vector3(0.0f, 0.0f, 0.0f));   //Set the rotation of our new Ball

        gameObject.transform.LookAt(Vector3.down);
    }
}
