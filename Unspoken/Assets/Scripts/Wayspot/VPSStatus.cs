using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class VPSStatus : MonoBehaviour
{
    [SerializeField]
    private WaySpotAnchorManager wayspotAnchorManager;

    [SerializeField]
    private List<TextMeshProUGUI> statusTextElements = new List<TextMeshProUGUI>();

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
        }
    }
}
