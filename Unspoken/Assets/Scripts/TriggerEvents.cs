using System;
using UnityEngine;
using UnityEngine.Events;


public class TriggerEvents : MonoBehaviour
{

    public string Name;
    public string tag;

    public UnityEvent onCollisionEnter;
    public UnityEvent onCollisionStay;
    public UnityEvent onCollisionExit;

    private void OnTriggerEnter(Collider other)
    {
        if ((Name != null && other.gameObject.name == Name) || (other.gameObject.tag != null && other.gameObject.tag == tag))
            onCollisionEnter.Invoke();

    }


    private void OnTriggerStay(Collider other)
    {
        if ((Name != null && other.gameObject.name == Name) || (other.gameObject.tag != null && other.gameObject.tag == tag))
        {
            onCollisionStay.Invoke();


        }
    }
    private void OnTriggerExit(Collider other)
    {
        if ((Name != null && other.gameObject.name == Name) || (other.gameObject.tag != null && other.gameObject.tag == tag))
            onCollisionExit.Invoke();
    }



}
