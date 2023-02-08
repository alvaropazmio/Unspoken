/////////////////////////////////////////////////////////////////////
//Purpose: Class that houses all the functionallity and data for   //
//the bottles                                                      //
//Developer: Alvaro Pazmiño                                        //
/////////////////////////////////////////////////////////////////////
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

    private void Awake()
    {
        rigidBody = GetComponent<Rigidbody>();
        
        questionGO = messageGO.transform.GetChild(0).gameObject;
        answerGO = messageGO.transform.GetChild(1).gameObject;

        questionText = questionGO.GetComponent<TMP_Text>();
        answerText = answerGO.GetComponent<TMP_Text>();

        messageGO.SetActive(false);

        currentState = State.Idle;
    }

    private void OnEnable()
    {
        AnimationEvents.bottleClosed += ThrowBack;
        AnimationEvents.bottleOpened += DisplayMessage;
    }

    private void OnDisable()
    {
        AnimationEvents.bottleClosed -= ThrowBack;
        AnimationEvents.bottleOpened -= DisplayMessage;

    }
    private void Update()
    {
        #region State switch statement
        switch (currentState)
        {
            case State.Idle:
                animator.SetBool("Selected", false);
                animator.SetBool("Open", false);
                messageGO.SetActive(false);
                break;
            case State.Open:
                animator.SetBool("Open", false);
                MoveTowardsPlayer();
                break;
            case State.Display:
                animator.SetBool("Open", true);
                break;
            case State.Close:
                messageGO.SetActive(false);
                animator.SetBool("Selected", false);
                animator.SetBool("Open", false);
                break;
            case State.Throw:
                break;
            default:
                break;
        }
        #endregion
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
        ChangeState("Throw");

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

    //This function is called when the bottle is first loaded, Activation in this context is giving a bottle an answer
    public void Activate(GameObject displayerGO, string question)
    {
        displayPoint = displayerGO.transform;
        currentQuestion = question;

        questionText.text = question;

        currentState = State.Open;
    }
 
    public void SavePost(string answer)
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

    //Makes sure only bottles with questions and answers are able to be displayed
    public void DisplayMessage()
    {
        if (currentAnswer != "")
        {
            messageGO.SetActive(true);
        }
    }


    public void LoadMUMessage(string message)
    {
        animator.SetBool("Loaded", true);

        string[] messageArray = new string[2];

        messageArray = message.Split(" + ");

        currentQuestion = messageArray[0];
        currentAnswer = messageArray[1];

        questionText.text = currentQuestion;
        answerText.text = currentAnswer;
    }
}
