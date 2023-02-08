/////////////////////////////////////////////////////////////////////
//Purpose: Manager which initializes other managers when needed    //
//Needed for Bottle Manager must be disabled until localization    //
//Developer: Alvaro Pazmiño                                        //
/////////////////////////////////////////////////////////////////////

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private BottleManager bottleManager;
    private CustomWaySpotAnchorManager waySpotAnchorManager;

    private void OnEnable()
    {
        VPSStatus.OnLocalized += StartGame;
    }
    private void OnDisable()
    {
        VPSStatus.OnLocalized -= StartGame;
    }

    private void Start()
    {
        bottleManager = GetComponent<BottleManager>();
        waySpotAnchorManager = GetComponent<CustomWaySpotAnchorManager>();
    }

    private void StartGame()
    {
        bottleManager.enabled = true;
        waySpotAnchorManager.LoadWayspotAnchors();
    }
}
