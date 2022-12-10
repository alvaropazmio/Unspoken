using Niantic.ARDK.Utilities.Input.Legacy;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


public class SelectOnTouch : MonoBehaviour
{
    public UnityEvent OnBottleClicked;

    public UnityEvent OnOpenBottle;

    [SerializeField]
    private Transform bottleStorage;

    [SerializeField]
    private GameObject bottleDisplay;

    [SerializeField]
    private GameObject bottlePrefab;

    [SerializeField]
    private GameObject selectedBottle;

    public float thrust;


    private void OnEnable()
    {
        BottleAnimationActions.OnBottleOpen += ShowMessage;
        BottleAnimationActions.OnBottleClosed += ThrowBottleBack;
    }

    private void OnDisable()
    {
        BottleAnimationActions.OnBottleOpen -= ShowMessage;
        BottleAnimationActions.OnBottleClosed -= ThrowBottleBack;

    }
    private void Awake()
    {
        OnBottleClicked.AddListener(ActivateBottle);
        //OnClickedBottle.AddListener(TriggerAnimation);
    }

    private void Update()
    {
        if (PlatformAgnosticInput.touchCount > 0)
        {
            var touch = PlatformAgnosticInput.GetTouch(0);
            if (touch.phase == TouchPhase.Began)
            {

                SelectARObject(touch.position);
            }
        }
    }

    void SelectARObject(Vector3 hitPosition)
    {
        var cameraTransform = Camera.main.transform;
        Ray ray = Camera.main.ScreenPointToRay(hitPosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            if (hit.collider.gameObject.tag == "Bottle")
            {
                selectedBottle = hit.collider.gameObject;

                //selectedBottle.transform.position = bottleStorage.transform.position;
                //selectedBottle.transform.parent = bottleStorage;
                OnBottleClicked.Invoke();
            }
        }
    }

    public void ThrowBottleBack()
    {
        if (selectedBottle == null)
        {
            var bottle = Instantiate(bottlePrefab, Vector3.zero, Quaternion.identity);
            ThrowBottle(bottle);
        }
        else
        {
            ThrowBottle(selectedBottle);
            selectedBottle = null;
        }

    }

    private void ThrowBottle(GameObject bottle)
    {
        bottle.transform.position = bottleDisplay.transform.position;
        var bottleRigidBody = bottle.GetComponent<Rigidbody>();
        bottleRigidBody.velocity = Vector3.zero;
        bottleRigidBody.AddForce(Camera.main.transform.forward * thrust);
    }

    private void ActivateBottle()
    {
        var bottleBehaivor = selectedBottle.GetComponent<BottleBehaivor>();

        bottleBehaivor.Activate(bottleDisplay);
        
    }

    private void ShowMessage()
    {
        OnOpenBottle.Invoke();
    }

    public void DeactivateBottle()
    {
        selectedBottle.GetComponent<BottleBehaivor>().Deactivate();
        selectedBottle = null;
        //selectedBottle.transform.parent = null;
        //bottle.transform.position = Camera.main.transform.position;
        //bottle.transform.rotation = Quaternion.identity;
    }

    /*
    private void TriggerAnimation()
    {
        Animator bottleAnimator = selectedBottle.GetComponent<Animator>();
        if (bottleAnimator == null) return;

        bottleAnimator.SetTrigger("Selected");
    }*/
}
