 using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Niantic.ARDK;
using Niantic.ARDK.AR.Configuration;
using Niantic.ARDK.Utilities.Input.Legacy;
using Niantic.ARDK.AR;
using System;
using UnityEngine.Events;

public class MockupUIManager : MonoBehaviour
{
    [SerializeField]
    GameObject welcomeCanvas;

    [SerializeField]
    GameObject scanningCanvas;
    /*
    [SerializeField]
    GameObject mainGameCanvas;
    */
    private CanvasGroup welcomeCanvasGroup;
    private CanvasGroup scanningCanvasGroup;

    public UnityEvent OnFirstTouch;
    public UnityEvent OnSecondTouch;
    public UnityEvent OnThirdTouch;
    //UnityEvent OnForthTouch;

    int touchCount = 0;
    bool welcome = true;
    bool scanning = false;


    private void Awake()
    {
        welcomeCanvasGroup = welcomeCanvas.GetComponent<CanvasGroup>();
        scanningCanvasGroup = scanningCanvas.GetComponent<CanvasGroup>();

        if (OnFirstTouch == null) OnFirstTouch = new UnityEvent();
        OnFirstTouch.AddListener(TurnOffWelcomeCanvas);
        OnFirstTouch.AddListener(TurnOnScanningCanvas);

        if (OnSecondTouch == null) OnSecondTouch = new UnityEvent();


        if (OnThirdTouch == null) OnThirdTouch = new UnityEvent();

    }

    private void TurnOffWelcomeCanvas()
    {
        welcome = false;
    }

    private void TurnOnScanningCanvas()
    {
        scanning = true;
    }

    private void Update()
    {
        checkTouch();

        if (!welcome)
        {
            welcomeCanvasGroup.alpha -= Time.deltaTime;
            if (welcomeCanvasGroup.alpha <= 0)
                welcomeCanvas.SetActive(false);
        }
        if (scanning && welcomeCanvasGroup.alpha == 0)
        {
            scanningCanvasGroup.alpha += Time.deltaTime;
        }
    }

    void checkTouch()
    {
        if (PlatformAgnosticInput.touchCount > 0)
        {
            var touch = PlatformAgnosticInput.GetTouch(0);
            if (touch.phase != TouchPhase.Began) return;

            touchCount++;


            switch (touchCount)
            {
                case 0:
                    Debug.Log("Que");
                    break;
                case 1:
                    OnFirstTouch.Invoke();
                    break;
                case 2:
                    OnSecondTouch.Invoke();
                    break;
                case 3:
                    OnThirdTouch.Invoke();
                    break;
                default:
                    break;
            }

        }
    }
}
