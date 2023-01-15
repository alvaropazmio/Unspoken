using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisableInSeconds : MonoBehaviour
{
    //public GameObject[] rigidObjects;
    public GameObject parentObject;

    private bool enable_ = true;
    private float inSec;
    public void DisableMessageInSec(float enableInSeconds)
    {
        inSec = enableInSeconds;
        enable_ = false;

    }
    private void Update()
    {
        if (enable_ == false)
        {

            inSec -= Time.deltaTime;
            //Debug.Log(inSec);

            if (inSec < 0)
            {
                parentObject.SetActive(false);
                enable_ = true;
                //Debug.Log(inSec);
            }

        }

    }


}


