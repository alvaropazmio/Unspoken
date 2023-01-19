using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Niantic.ARDKExamples.WayspotAnchors;
using System;

public class VPSStatus : MonoBehaviour
{
    [SerializeField]
    private CustomWaySpotAnchorManager wayspotAnchorManager;

    [SerializeField]
    private List<TextMeshProUGUI> statusTextElements = new List<TextMeshProUGUI>();

    public static event Action OnLocalized;

    private void OnEnable()
    {
        wayspotAnchorManager.Updated += UpdateStatus;
    }

    private void OnDisable()
    {
        wayspotAnchorManager.Updated -= UpdateStatus;
    }

    private void UpdateStatus(string newStatus)
    {
        foreach (var statusTextElement in statusTextElements)
        {
            statusTextElement.text = newStatus;
            if (newStatus == "Localized")
            {
                StartCoroutine("WaitAndInvokeOnLocalized", 0.5f);
            }
        }
    }

    IEnumerator WaitAndInvokeOnLocalized(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        OnLocalized?.Invoke();
    }
}
