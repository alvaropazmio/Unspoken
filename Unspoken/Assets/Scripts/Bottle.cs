using Niantic.ARDK.Utilities.Input.Legacy;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


public class Bottle : MonoBehaviour
{
    [HideInInspector]
    public Rigidbody rigidBody;
    private Animator animator;
    
    private Transform displayPoint;

    private float approachVelocity = 0.2f;
    private float idleSpeed = 0.06f;
    //this value also exists on Bottle Manager, it would be better to have a gobal value that manages this
    private float thrust = 150;

    [SerializeField]
    private GameObject questionGO;
    [SerializeField]
    private GameObject answerGO;
    [SerializeField]
    private TMP_Text questionText;
    [SerializeField]
    private TMP_Text answerText;

    private enum State { Idle, Open, Display, Close, Throw}
    [SerializeField]
    private State currentState;

    private bool wayspotEnabled = false;

    //public delegate void WayspotCreator(Matrix4x4 localPose);
    //public event WayspotCreator WayspotCreated;



    private void Awake()
    {
        questionText = questionGO.GetComponent<TMP_Text>();
        answerText = answerGO.GetComponent<TMP_Text>();

        currentState = State.Idle;
        rigidBody = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        switch (currentState)
        {
            case State.Idle:
                questionGO.SetActive(false);
                answerGO.SetActive(false);
                //ARButton.SetActive(false);
                transform.Translate(Vector3.left * idleSpeed * Time.deltaTime);
                break;
            case State.Open:
                MoveTowardsPlayer();
                break;
            case State.Display:
                answerGO.SetActive(true);
                //ARButton.SetActive(true);
                break;
            case State.Close:
                animator.SetBool("Selected", false);
                answerGO.SetActive(false);
                //ARButton.SetActive(false);
                break;
            case State.Throw:
                ThrowBack();
                break;
            default:
                break;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Plane")
        {
            if (wayspotEnabled)
                return;

            //CreateWayspot(out Matrix4x4 localPose);
            //WayspotCreated.Invoke(localPose);
            Debug.Log("wow");
            wayspotEnabled = true;
            BottleActions.OnWayspotRequested(this);
        }
    }

    private void MoveTowardsPlayer()
    {
        rigidBody.isKinematic = true;
        rigidBody.useGravity = false;
        transform.position = Vector3.MoveTowards(transform.position,  displayPoint.position, approachVelocity);
        transform.rotation = displayPoint.transform.rotation;

        float distance = Vector3.Distance(transform.position, displayPoint.position);
        if (distance <= 0)
        {
            Debug.Log("BottleSelected");
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

    public void ThrowNew(GameObject displayerGO,string question, string answer)
    {
        displayPoint = displayerGO.transform;
        transform.position = displayPoint.position;
        transform.rotation = displayPoint.rotation;

        questionText.text = question;
        answerText.text = answer;

        rigidBody.isKinematic = false;
        rigidBody.useGravity = true;

        rigidBody.AddForce(Camera.main.transform.forward * thrust);

        ChangeState("Idle");
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
    /*
    private void CreateBottleMatrix(out Matrix4x4 localPose)
    {
        localPose =
            Matrix4x4.TRS
            (
                transform.position,
                transform.rotation,
                transform.localScale
            );
    }*/
}
