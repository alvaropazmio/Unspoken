/////////////////////////////////////////////////////////////////////
//Purpose: Manager that takes care of the instantiation of bottles,//
//it also serves as a link of comunication between each bottle and //
//the user                                                         //
//Developer: Alvaro Pazmiño                                        //
/////////////////////////////////////////////////////////////////////
using Niantic.ARDK.AR.WayspotAnchors;
using Niantic.ARDK.Extensions;
using Niantic.ARDK.Utilities.Input.Legacy;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class BottleManager : MonoBehaviour
{
    List<Bottle> bottlePool = new List<Bottle>();
    Dictionary<GameObject, Bottle> bottleDictionary = new Dictionary<GameObject, Bottle>();
    private QuestionsManager questionsManager;

    [SerializeField]
    private GameObject bottlePrefab;
    [SerializeField]
    private GameObject bottleDisplayer;    
    [SerializeField]
    private Transform bottleStorage;

    private Bottle selectedBottle;

    private string currentQuestion;
    private string currentAnswer;

    private Touch currentTouch; 

    public enum State { Setup, Welcome, ScanMain, ScanTerrain, MainGame}
    public State currentState;
    public State initialState;

    public UnityEvent onBottleSelected;

    private void Awake()
    {
        questionsManager = GetComponent<QuestionsManager>();
        currentState = initialState;
    }

    private void OnEnable()
    {
        BottleActions.OnBottleLoaded += RegisterLoadedBottle;
    }

    private void OnDisable()
    {
        BottleActions.OnBottleLoaded -= RegisterLoadedBottle;
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
        }
    }

    public void PostBottle(string answer)
    {
        selectedBottle.SavePost(answer);
        CloseSelectedBottle();
    }

    public void ChangeState (string newStateName)
    {
        State newState = (State)System.Enum.Parse(typeof(State), newStateName);
        currentState = newState;
    }

}
