using Niantic.ARDK.Utilities.Input.Legacy;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//Bug, when the player moves around, the text will persist until idle state
public class Bottle : MonoBehaviour
{
    [HideInInspector]
    public Rigidbody rigidBody;
    private Animator animator;
    
    private Transform displayPoint;

    private float approachVelocity = 0.2f;
    private float idleSpeed = 0.06f;
    private float smoothTime = 0.25f;
    private Vector3 followingVelocity = Vector3.zero;
    //this value also exists on Bottle Manager, it would be better to have a gobal value that manages this
    private float thrust = 150;

    [SerializeField]
    private GameObject ARText;
    [SerializeField]
    private GameObject ARButton;

    private enum State { Idle, Open, Display, Close, Throw}
    [SerializeField]
    private State currentState;  

    private void Awake()
    {
        currentState = State.Idle;
        rigidBody = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        switch (currentState)
        {
            case State.Idle:
                ARText.SetActive(false);
                ARButton.SetActive(false);
                transform.Translate(Vector3.left * idleSpeed * Time.deltaTime);
                break;
            case State.Open:
                MoveTowardsPlayer();
                break;
            case State.Display:
                ARText.SetActive(true);
                ARButton.SetActive(true);
                //SmoothFolow();
                break;
            case State.Close:
                animator.SetBool("Selected", false);
                ARText.SetActive(false);
                ARButton.SetActive(false);
                break;
            case State.Throw:
                ThrowBack();
                break;
            default:
                break;
        }
    }
    private void MoveTowardsPlayer()
    {
        rigidBody.isKinematic = true;
        rigidBody.useGravity = false;
        transform.position = Vector3.MoveTowards(transform.position, displayPoint.position, approachVelocity);
        transform.rotation = Camera.main.transform.rotation;

        float distance = Vector3.Distance(transform.position, displayPoint.position);
        if (distance <= 0)
        {
            animator.SetBool("Selected", true);
        }
    }

    private void ThrowBack()
    {

        rigidBody.isKinematic = false;
        rigidBody.useGravity = true;

        rigidBody.AddForce(Camera.main.transform.forward * thrust);

        ChangeState("Idle");
    }

    public void ThrowNew(GameObject displayerGO)
    {
        displayPoint = displayerGO.transform;
        transform.position = displayPoint.position;
        transform.rotation = displayPoint.rotation;

        rigidBody.isKinematic = false;
        rigidBody.useGravity = true;

        rigidBody.AddForce(Camera.main.transform.forward * thrust);

        ChangeState("Idle");
    }

    private void SmoothFolow()
    {
        Vector3 targetPos = displayPoint.transform.position;
        transform.position = Vector3.SmoothDamp(transform.position, targetPos, ref followingVelocity, smoothTime);
        transform.LookAt(2 * transform.position - Camera.main.transform.position);
    }

    public void Activate(GameObject displayerGO)
    {
        displayPoint = displayerGO.transform;
        currentState = State.Open;
    }

    public void ChangeState(string newStateName)
    {
        State newState = (State)System.Enum.Parse(typeof(State), newStateName);

        currentState = newState;
    }
}
