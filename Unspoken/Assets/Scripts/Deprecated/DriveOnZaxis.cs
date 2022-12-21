using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This script moves insect up
public class DriveOnZaxis: MonoBehaviour
{
    public float speed = 10.0f;


    void Update()
    {

        transform.Translate(Vector3.left * speed * Time.deltaTime);
    }
}