// Deprecated, use Bottle.cs
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class BottleBehaivor : MonoBehaviour
{
    public bool moveTowardsPlayer = false;

    private Transform displayPoint;
    private Animator animator;
    //private Rigidbody rigidbody;
    //[SerializeField]
    private float velocity = 0.2f;
    private float movementSpeed = 0.06f;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        //rigidbody = GetComponent<Rigidbody>();
    }

    void Update()
    {
        if (moveTowardsPlayer)
        {
            //rigidbody.isKinematic = true;
            //rigidbody.useGravity = false;
            transform.position = Vector3.MoveTowards(transform.position, displayPoint.position, velocity);

            float distance = Vector3.Distance(transform.position, displayPoint.position);
            if (distance <= 0)
            {
                transform.rotation = Camera.main.transform.rotation;
                TriggerAnimation();
            }
        }
        else if (animator.GetCurrentAnimatorStateInfo(0).IsName("Bottle_Idle")) 
            transform.Translate(Vector3.left * movementSpeed * Time.deltaTime);
    }

    public void Activate(GameObject displayGO)
    {
        displayPoint = displayGO.transform;
        moveTowardsPlayer = true;
    }

    private void TriggerAnimation()
    {
        animator.SetBool("Selected", true);
        //animator.SetTrigger("Selected");
        moveTowardsPlayer = false;
    }

    public void OpenAnimationFinished()
    {
        BottleAnimationActions.OnBottleOpen();
    }

    public void Deactivate()
    {
        animator.SetBool("Selected", false);
    }

    public void ReadyToThrowBack()
    {
        //rigidbody.isKinematic = false;
        //rigidbody.useGravity = true;
        BottleAnimationActions.OnBottleClosed();
    }
}
