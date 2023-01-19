using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveToCollisionContact : MonoBehaviour
{

    public string Name;
    public string objTag;
    public Transform toContactPoint;
    private void OnCollisionStay(Collision collision)
    {

        if ((Name != null && collision.gameObject.name == Name) || (collision.gameObject.tag != null && collision.gameObject.tag == tag))
        {
            foreach (ContactPoint contact in collision.contacts)
            {
                //print(contact.thisCollider.name + " hit " + contact.otherCollider.name);
                // Visualize the contact point
                Debug.DrawRay(contact.point, contact.normal, Color.white);
                toContactPoint.transform.position = contact.point;
                //toContactPoint.transform.LookAt(Vector3.forward);


            }
        }

    }
}
