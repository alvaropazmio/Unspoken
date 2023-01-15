using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpinBottle : MonoBehaviour
{
    public float speed = 30;


    // Update is called once per frame
    void Update()
    {
        transform.Rotate(0f, speed * Time.deltaTime, 0f, Space.Self);
    }
}
