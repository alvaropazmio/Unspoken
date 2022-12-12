using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartZeroRotate : MonoBehaviour
{

    // Start is called before the first frame update
    void Start()
    {
        transform.rotation = new Quaternion(0, 0, 0, 0);
    }


}
