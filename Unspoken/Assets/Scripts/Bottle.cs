using Niantic.ARDK.Utilities.Input.Legacy;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


public class Bottle : MonoBehaviour
{
    [HideInInspector]
    public Rigidbody rigidBody;
    public Animator animator;
    
    private Transform displayPoint;

    private float approachVelocity = 0.1f;
    private float idleSpeed = 0.06f;
    //this value also exists on Bottle Manager, it would be better to have a gobal value that manages this
    private float thrust = 150;


    [SerializeField]
    private GameObject messageGO;

    private GameObject questionGO;
    private GameObject answerGO;
    
    private TMP_Text questionText;
    private TMP_Text answerText;

    [SerializeField]
    private string currentQuestion;
    [SerializeField]
    private string currentAnswer;

    private enum State { Idle, Open, Display, Close, Throw}
    [SerializeField]
    private State currentState;

    private bool wayspotEnabled = false;

    //public delegate void WayspotCreator(Matrix4x4 localPose);
    //public event WayspotCreator WayspotCreated;



    private void Awake()
    {
        questionGO = messageGO.transform.GetChild(0).gameObject;
        answerGO = messageGO.transform.GetChild(1).gameObject;

        questionText = questionGO.GetComponent<TMP_Text>();
        answerText = answerGO.GetComponent<TMP_Text>();

        messageGO.SetActive(false);

        currentState = State.Idle;
        rigidBody = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        switch (currentState)
        {
            case State.Idle:
                //animator.SetBool("Selected", false);
                transform.Translate(Vector3.left * idleSpeed * Time.deltaTime);
                break;
            case State.Open:
                MoveTowardsPlayer();
                //messageGO.SetActive(true);
                break;
            case State.Display:
                //ARButton.SetActive(true);
                break;
            case State.Close:
                messageGO.SetActive(false);
                animator.SetBool("Selected", false);
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

        currentQuestion = question;
        currentAnswer = answer;

        questionText.text = question;
        answerText.text = answer;

        rigidBody.isKinematic = false;
        rigidBody.useGravity = true;

        rigidBody.AddForce(Camera.main.transform.forward * thrust);

        ChangeState("Idle");
    }

    public void Activate(GameObject displayerGO, string question)
    {
        displayPoint = displayerGO.transform;
        currentQuestion = question;
        //currentAnswer = answer;

        questionText.text = currentQuestion;
        //answerText.text = currentAnswer;

        currentState = State.Open;
    }

    public void Post(string answer)
    {
        currentAnswer = answer;

        answerText.text = currentAnswer;
    }


    public void Select(GameObject displayerGO)
    {
        displayPoint = displayerGO.transform;

        currentState = State.Open;
    }


    public void ChangeState(string newStateName)
    {
        State newState = (State)System.Enum.Parse(typeof(State), newStateName);

        currentState = newState;
    }

    public void DisplayMessage()
    {
        messageGO.SetActive(true);
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
