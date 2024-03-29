using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtCameraY : MonoBehaviour
{

    public float damping = 1;
    // Update is called once per frame
    void Update()
    {
        var lookPos = Camera.main.transform.position - transform.position;
        //lookPos.y = 0;
        var rotation = Quaternion.LookRotation(lookPos);
        rotation.z = 0;
        rotation.x = 0;

        transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * damping);
    }
}
