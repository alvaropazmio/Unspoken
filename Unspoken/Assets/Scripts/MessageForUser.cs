using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MessageForUser : MonoBehaviour
{
    private CanvasGroup canvasGroup;
    private float fadeSpeed;
    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        fadeSpeed = 0.1f;
    }

    void Update()
    {
        if (gameObject.activeSelf)
        {
            StartCoroutine("Wait");
        }
        if (canvasGroup.alpha <= 0)
        {
            canvasGroup.alpha = 1;
            gameObject.SetActive(false);
        }
    }


    IEnumerator Wait()
    {
        yield return new WaitForSeconds(2);
        StartCoroutine("FadeOut");

    }

    IEnumerator FadeOut()
    {
        while (canvasGroup.alpha > 0)
        {
            float fadeAmount = canvasGroup.alpha - (fadeSpeed * Time.deltaTime);

            canvasGroup.alpha = fadeAmount;
            yield return null;
        }
    }

}
