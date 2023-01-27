using Niantic.ARDK.Utilities.Input.Legacy;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class UIManager : MonoBehaviour
{
    [SerializeField]
    private GameObject welcomeCanvas;
    [SerializeField]
    private GameObject scanCanvas;
    [SerializeField]
    private GameObject mainCanvas;
    [SerializeField]
    private GameObject writingCanvas;

    [SerializeField]
    private VPSStatus status;
    [SerializeField]
    private bool localized = false;

    private bool welcome = true;
    private CanvasGroup welcomeCanvasGroup;
    private CanvasGroup scanCanvasGroup;

    public UnityEvent OnWelcomeScreen;
    public UnityEvent OnScanScreen;

    public UnityEvent OnMainGameScreen;

    private void OnEnable()
    {
        VPSStatus.OnLocalized += InitiateGameUI;
    }

    private void OnDisable()
    {
        VPSStatus.OnLocalized -= InitiateGameUI;
    }

    private void Awake()
    {
        OnWelcomeScreen.Invoke();

        /*welcomeCanvas.SetActive(true);
        scanCanvas.SetActive(false);
        mainCanvas.SetActive(false);
        writingCanvas.SetActive(false);
        */
        welcomeCanvasGroup = welcomeCanvas.GetComponent<CanvasGroup>();
        //welcomeCanvasGroup.alpha = 1;
        scanCanvasGroup = scanCanvas.GetComponent<CanvasGroup>();
        //scanCanvasGroup.alpha = 0;
    }

    private void Start()
    {
        //welcome = false;
        StartCoroutine("FadeOutWelcome");
    }


    private void Update()
    {

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