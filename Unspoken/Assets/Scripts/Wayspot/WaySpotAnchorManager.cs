using System;
using System.Collections.Generic;
using System.Linq;

using Niantic.ARDK;
using Niantic.ARDK.AR;
using Niantic.ARDK.AR.ARSessionEventArgs;
using Niantic.ARDK.AR.HitTest;
using Niantic.ARDK.AR.WayspotAnchors;
using Niantic.ARDK.Configuration;
using Niantic.ARDK.Extensions;
using Niantic.ARDK.LocationService;
using Niantic.ARDK.Utilities;
using Niantic.ARDK.Utilities.Input.Legacy;

using UnityEngine;
using UnityEngine.UI;

public class WaySpotAnchorManager : MonoBehaviour
{
    private IARSession _arSession;
    public WayspotAnchorService wayspotAnchorService;

    private IWayspotAnchorsConfiguration _config;

    private readonly HashSet<WayspotAnchorTracker> _wayspotAnchorTrackers = new HashSet<WayspotAnchorTracker>();

    public delegate void TextUpdateHandler(string localisationStatus);
    public event TextUpdateHandler Updated;

    private string _localisationStatus;

    private void Awake()
    {
        //Remember to set a user ID for launch and final presentation
        //var userId = GetCurrentUserId();
        //ArdkGlobalConfig.SetUserIdOnLogin(userId);
        UpdateLocalisationStatus("Initializing Session...");
    }

    private void OnEnable()
    {
        ARSessionFactory.SessionInitialized += HandleSessionInitialized;
        //!!!!! find a better place
        BottleActions.OnBottleCreated += HandleNewWayspot;
    }

    private void OnDisable()
    {
        ARSessionFactory.SessionInitialized -= HandleSessionInitialized;
        //!!!!! find a better place
        BottleActions.OnBottleCreated -= HandleNewWayspot;

    }

    private void OnDestroy()
    {
        if (wayspotAnchorService != null)
        {
            wayspotAnchorService.LocalizationStateUpdated -= LocalizationStateUpdated;
            wayspotAnchorService.Dispose();
        }
    }

    private void Update()
    {
        if (wayspotAnchorService == null)
            return;

        //have to get the Matrix4x4 of the bottle with it's position, rotation and scale.

    }

    private void HandleNewWayspot(Bottle newBottle)
    {
        //check if I can have this checkpoint for the wayspot anchor service
        /*if (wayspotAnchorService == null)
            Debug.Log("Something went wrong");
            return;
        */
        if (wayspotAnchorService.LocalizationState == LocalizationState.Localized)
        {
            var localPose =
                Matrix4x4.TRS
                (
                    newBottle.transform.position,
                    newBottle.transform.rotation,
                    newBottle.transform.localScale
                );

            PlaceAnchor(localPose, newBottle.gameObject);

        }
    }


    private void PlaceAnchor(Matrix4x4 localPose, GameObject bottle)
    {
        var anchors = wayspotAnchorService.CreateWayspotAnchors(localPose);
        if (anchors.Length == 0)
            return; //error raised in CreateWayspotAnchors


        CreateWayspotTracker(anchors[0], bottle, true);

        UpdateLocalisationStatus("Anchor placed... well done btw <3");
    }

    private void CreateWayspotTracker (IWayspotAnchor anchor, GameObject bottle, bool startActive)
    {
        var tracker = bottle.GetComponent<WayspotAnchorTracker>();
        if (tracker == null)
        {
            Debug.Log("Anchor prefab was missing WayspotAnchorTracher, so one will be added.");
            tracker = bottle.AddComponent<WayspotAnchorTracker>();
        }

        tracker.gameObject.SetActive(startActive);
        tracker.AttachAnchor(anchor);
        _wayspotAnchorTrackers.Add(tracker);
    }

    private void HandleSessionInitialized(AnyARSessionInitializedArgs args)
    {
        UpdateLocalisationStatus("Session Initialized");
        _arSession = args.Session;
        _arSession.Ran += HandleSessionRan;
    }

    private void HandleSessionRan(ARSessionRanArgs args)
    {
        _arSession.Ran -= HandleSessionRan;
        wayspotAnchorService = CreateWaySpotAnchorService();
        wayspotAnchorService.LocalizationStateUpdated += OnLocalizationStateUpdate;
        UpdateLocalisationStatus("Session running");
    }

    private void OnLocalizationStateUpdate(LocalizationStateUpdatedArgs args)
    {
        UpdateLocalisationStatus(args.State.ToString());
    }

    private WayspotAnchorService CreateWaySpotAnchorService()
    {
        var locationService = LocationServiceFactory.Create(_arSession.RuntimeEnvironment);
        locationService.Start();

        if (_config == null)
            _config = WayspotAnchorsConfigurationFactory.Create();

        var wayspotAnchorService = 
            new WayspotAnchorService
            (
                _arSession,
                locationService,
                _config
            );

        wayspotAnchorService.LocalizationStateUpdated += LocalizationStateUpdated;

        return wayspotAnchorService;
    }

    private void LocalizationStateUpdated(LocalizationStateUpdatedArgs args)
    {
        UpdateLocalisationStatus(args.State.ToString());
    }

    private void UpdateLocalisationStatus(string newStatus)
    {
        _localisationStatus = newStatus;
        Updated?.Invoke(newStatus);
    }
}
