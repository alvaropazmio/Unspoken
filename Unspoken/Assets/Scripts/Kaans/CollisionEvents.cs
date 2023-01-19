using System;
using UnityEngine;
using UnityEngine.Events;


public class CollisionEvents : MonoBehaviour
{

    public string Name;
    public string tag;

    public UnityEvent onCollisionEnter;
    public UnityEvent onCollisionStay;
    public UnityEvent onCollisionExit;

    private void OnCollisionEnter(Collision collision)
    {
        if ((Name != null &&  collision.gameObject.name == Name) || (collision.gameObject.tag != null && collision.gameObject.tag == tag))
            onCollisionEnter.Invoke();


    }


    private void OnCollisionStay(Collision collision)
    {
        if ((Name != null && collision.gameObject.name == Name) || (collision.gameObject.tag != null && collision.gameObject.tag == tag))
        {
            onCollisionStay.Invoke();


        }

    }

    private void OnCollisionExit(Collision collision)
    {
        if ((Name != null && collision.gameObject.name == Name) || (collision.gameObject.tag != null && collision.gameObject.tag == tag))
            onCollisionExit.Invoke();
    }



}
