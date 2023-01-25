//Under construction, manager script merging the SelectOnTouch and the ThrowOnTap scripts

//To Do:
// Enums *
// Object pool bottles * 
// Migrate Throw on Touch *
// Three Empty states for 
//Welcome *
//Scan main spot *
//Scan for water *
// Migrate Select on Touch *
// Check for Touch *
// State Machine switching between Setup and game *
// Handle Actions and events * (didn't need any events in this script)
// Create new Bottle and throw *

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
    private Bottle selectedBottle;

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

    private QuestionsManager questionManager;


    private void Awake()
    {
        currentState = initialState;

        for (int i = 0; i < bottleCount; i++)
        {
            CreateNewBottle(" ERROR ", " ERROR ") ;
        }

    }

    private void OnEnable()
    {
        //BottleActions.OnBottleLoaded += CreateNewBottle;
        BottleActions.OnBottleLoaded += RegisterBottle;
        //VPSStatus.OnLocalized += SwitchToMainGame;

    }

    private void OnDisable()
    {
        //BottleActions.OnBottleLoaded -= CreateNewBottle;
        BottleActions.OnBottleLoaded -= RegisterBottle;
        //VPSStatus.OnLocalized -= SwitchToMainGame;

    }

    private void Start()
    {
        currentState = State.Setup;
        BottleActions.OnBottlePrefabSent(bottlePrefab);
        currentState = State.MainGame;
    }



    public void CreateNewBottle(string question, string answer)
    {
        var go = Instantiate(bottlePrefab, Vector3.zero, Quaternion.identity);

        Bottle newBottle = go.GetComponent<Bottle>();

        bottlePool.Add(newBottle);
        bottleDictionary.Add(go, newBottle);

        if (currentState == State.Setup)
        {
            newBottle.gameObject.SetActive(false);
        }
        else if (currentState == State.MainGame)
        {
            newBottle.ThrowNew(bottleDisplayer,question,answer);
        }

    }

    private void RegisterBottle(GameObject bottleGO)
    {
        Bottle bottle = bottleGO.GetComponent<Bottle>();

        bottlePool.Add(bottle);
        bottleDictionary.Add(bottleGO, bottle);
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
            selectedBottle.Activate(bottleDisplayer);
        }
        else if (hit.collider.gameObject.tag == "ARButton")
        {
            selectedBottle.ChangeState("Close");
        }
    }

    public void ChangeState (string newStateName)
    {
        State newState = (State)System.Enum.Parse(typeof(State), newStateName);

        currentState = newState;
    }
}
