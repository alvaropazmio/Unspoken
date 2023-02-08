/////////////////////////////////////////////////////////////////////
//Purpose: Manages the UI's visual effects                         //
//Developer: Alvaro Pazmiño                                        //
/////////////////////////////////////////////////////////////////////
using Niantic.ARDK.Utilities.Input.Legacy;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class UIManager : MonoBehaviour
{
    [SerializeField]
    private bool localized = false;

    [SerializeField]
    private GameObject welcomeCanvas;
    [SerializeField]
    private GameObject scanCanvas;
    [SerializeField]
    private GameObject mainCanvas;
    [SerializeField]
    private GameObject writingCanvas;
    [SerializeField]
    private GameObject readingCanvas;
    private GameObject currentCanvas;

    private CanvasGroup welcomeCanvasGroup;
    private CanvasGroup scanCanvasGroup;

    public UnityEvent OnWelcomeScreen;
    public UnityEvent OnScanScreen;
    public UnityEvent OnMainGameScreen;

    private void OnEnable()
    {
        VPSStatus.OnLocalized += InitiateGameUI;
        AnimationEvents.bottleOpened += OpenMessageCanvas;
    }

    private void OnDisable()
    {
        VPSStatus.OnLocalized -= InitiateGameUI;
        AnimationEvents.bottleOpened -= OpenMessageCanvas;
    }

    private void Awake()
    {
        OnWelcomeScreen.Invoke();

        welcomeCanvasGroup = welcomeCanvas.GetComponent<CanvasGroup>();
        scanCanvasGroup = scanCanvas.GetComponent<CanvasGroup>();
    }


    private void Start()
    {
        StartCoroutine("FadeOutWelcome");
    }

    public void SetCanvasToOpen(string canvasType)
    {
        if (canvasType == "Writing")
        {
            currentCanvas = writingCanvas;
        }
        else if (canvasType == "Reading")
        {
            currentCanvas = readingCanvas;
        }
        else Debug.Log("Check spelling");
    }

    private void OpenMessageCanvas()
    {
        if (currentCanvas == null)
        {
            Debug.Log("no current canvas");
            return;
        }

        currentCanvas.SetActive(true);
    }
    IEnumerator FadeOutWelcome()
    {
        yield return StartCoroutine("FadeOut", welcomeCanvasGroup);
        OnScanScreen.Invoke();
        StartCoroutine("FadeInScan");
    }

    IEnumerator FadeInScan()
    {
        yield return StartCoroutine("FadeIn", scanCanvasGroup);
    }


    IEnumerator FadeOut(CanvasGroup canvasGroup)
    {
        for (float f = 0.95f; f >= 0; f -= 0.1f)
        {
            canvasGroup.alpha = f;
            yield return new WaitForSeconds(0.05f);
        }
    }

    IEnumerator FadeIn(CanvasGroup canvasGroup)
    {
        for (float f = 0.05f; f <= 1; f += 0.1f)
        {
            canvasGroup.alpha = f;
            yield return new WaitForSeconds(0.05f);
        }
    }


    private void InitiateGameUI()
    {
        OnMainGameScreen.Invoke();
    }


}
