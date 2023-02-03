using Niantic.ARDK.AR.WayspotAnchors;
using Niantic.ARDK.Extensions;
using Niantic.ARDK.Utilities.Input.Legacy;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class BottleManager : MonoBehaviour
{

    [SerializeField]
    private GameObject bottlePrefab;
    [SerializeField]
    private Transform bottleStorage;
    [SerializeField]
    private GameObject bottleDisplayer;
    
    private QuestionsManager questionsManager;

    private Bottle selectedBottle;

    private string currentQuestion;
    private string currentAnswer;

    [SerializeField]
    int bottleCount;

    List<Bottle> bottlePool = new List<Bottle>();
    Dictionary<GameObject, Bottle> bottleDictionary = new Dictionary<GameObject, Bottle>();


    private Touch currentTouch; 

    [SerializeField]
    float thrust;

    int bottleIndex = 0;


    public enum State { Setup, Welcome, ScanMain, ScanTerrain, MainGame}
    //[HideInInspector]
    public State currentState;
    public State initialState;

    public UnityEvent onBottleSelected;

    private void Awake()
    {
        questionsManager = GetComponent<QuestionsManager>();
        //BottleActions.OnBottlePrefabSent(bottlePrefab);
        currentState = initialState;

        for (int i = 0; i < bottleCount; i++)
        {
            CreateNewBottle(" ERROR ") ;
        }

    }

    private void OnEnable()
    {
        //BottleActions.OnBottleLoaded += CreateNewBottle;
        BottleActions.OnBottleLoaded += RegisterLoadedBottle;
        //VPSStatus.OnLocalized += SwitchToMainGame;

    }

    private void OnDisable()
    {
        //BottleActions.OnBottleLoaded -= CreateNewBottle;
        BottleActions.OnBottleLoaded -= RegisterLoadedBottle;
        //VPSStatus.OnLocalized -= SwitchToMainGame;

    }

    private void Start()
    {
        currentState = State.Setup;
        currentState = State.MainGame;
    }

    

    public void CreateNewBottle(string question)
    {
        var bottleGO = Instantiate(bottlePrefab, bottleStorage.transform.position, Quaternion.identity);
        Bottle newBottle = bottleGO.GetComponent<Bottle>();

        bottlePool.Add(newBottle);
        bottleDictionary.Add(bottleGO, newBottle);

        
        if (currentState == State.Setup)
        {
            newBottle.gameObject.SetActive(false);
        }
        else if (currentState == State.MainGame)
        {
            newBottle.Activate(bottleDisplayer, question);
            selectedBottle = newBottle;
            currentQuestion = question;
            //newBottle.ThrowNew(bottleDisplayer,question,answer);
        }

    }

    public void CloseSelectedBottle()
    {
        if (selectedBottle != null)
        {
            selectedBottle.ChangeState("Close");
        }
    }

    public void ThrowNewBottle()
    {
        if (selectedBottle != null)
        {
            selectedBottle.ThrowNew(bottleDisplayer, currentQuestion, currentAnswer);
            selectedBottle = null;
            currentQuestion = null;
            currentAnswer = null;
        }
    }

    public void DestroyBottle()
    {
        if (selectedBottle != null)
        {
            Destroy(selectedBottle.gameObject);
            selectedBottle = null;
            currentQuestion = null;
            currentAnswer = null;
        }
    }

    private void RegisterLoadedBottle(GameObject bottleGO)
    {
        Bottle bottle = bottleGO.GetComponent<Bottle>();

        bottlePool.Add(bottle);
        bottleDictionary.Add(bottleGO, bottle);

        bottle.LoadMUMessage(questionsManager.RandomQuestion(true));
    }
    
    private void Update()
    {
        /*
        switch (currentState)
        {
            case State.Setup:

                if (!GetTouch()) return;

                ThrowPooledBottle();
                break;
            case State.Welcome:
                break;
            case State.ScanMain:
                break;
            case State.ScanTerrain:
                break;
            case State.MainGame:
                
                if (!GetTouch()) return;

                SelectBottle(currentTouch);
                break;
            default:
                break;
        }*/

        if (!GetTouch()) return;
        SelectBottle(currentTouch);

    }

    private bool GetTouch()
    {
        if (PlatformAgnosticInput.touchCount <= 0) return false;
        
        var touch = PlatformAgnosticInput.GetTouch(0);

        if (touch.phase == TouchPhase.Began && !touch.IsTouchOverUIObject())
        {
            currentTouch = touch;
            return true;
        }
        else return false;
        
    }


    private void ThrowPooledBottle()
    {
        if (bottleIndex >= bottleCount) bottleIndex = 0;

        var bottle = bottlePool[bottleIndex];

        bottle.gameObject.SetActive(true);
        bottle.transform.parent = null;
        bottle.transform.position = Camera.main.transform.position;
        bottle.transform.rotation = Quaternion.identity;

        bottle.rigidBody.velocity = Vector3.zero;
        bottle.rigidBody.AddForce(Camera.main.transform.forward * thrust);
        bottleIndex++;
    }


    private void SelectBottle(Touch touch)
    {
        Ray ray = Camera.main.ScreenPointToRay(touch.position);
        RaycastHit hit;
        if (!Physics.Raycast(ray, out hit)) return;
        if (hit.collider.gameObject.tag == "Bottle")
        {

            selectedBottle = bottleDictionary[hit.collider.gameObject];
            selectedBottle.Select(bottleDisplayer);
            onBottleSelected.Invoke();
        }/*
        else if (hit.collider.gameObject.tag == "ARButton")
        {
            selectedBottle.ChangeState("Close");
        }*/
    }


    public void PostBottle(string answer)
    {
        selectedBottle.Post(answer);
        CloseSelectedBottle();
    }

    public void ChangeState (string newStateName)
    {
        State newState = (State)System.Enum.Parse(typeof(State), newStateName);

        currentState = newState;
    }


}
