/////////////////////////////////////////////////////////////////////
//Purpose: Main manager in chage of the localisation aspect and the//
//saving and loading of anchors                                    //
//Developer: Alvaro Pazmiño                                         
//Based on: Lightship's WayspotAnchorManager                       // 
/////////////////////////////////////////////////////////////////////
using System;
using System.Collections.Generic;
using System.Linq;

using Niantic.ARDK;
using Niantic.ARDK.AR;
using Niantic.ARDK.AR.ARSessionEventArgs;
using Niantic.ARDK.AR.HitTest;
using Niantic.ARDK.AR.WayspotAnchors;
using Niantic.ARDK.Extensions;
using Niantic.ARDK.LocationService;
using Niantic.ARDK.Utilities;
using Niantic.ARDK.Utilities.Input.Legacy;
using Niantic.ARDKExamples.WayspotAnchors;
using Niantic.LightshipHub.Templates;
using UnityEngine;
using UnityEngine.UI;

public class CustomWaySpotAnchorManager : MonoBehaviour
{
    [SerializeField]
    private BottleManager bottleManager;
    [SerializeField]
    private GameObject _bottlePrefab;

    private IARSession _arSession;
    public WayspotAnchorService wayspotAnchorService;
    private IWayspotAnchorsConfiguration _config;

    private readonly HashSet<WayspotAnchorTracker> _wayspotAnchorTrackers = new HashSet<WayspotAnchorTracker>();
    private HashSet<IWayspotAnchor[]> _savedAnchorTrackers = new HashSet<IWayspotAnchor[]>();

    public delegate void TextUpdateHandler(string localisationStatus);
    public event TextUpdateHandler Updated;

    private string _localisationStatus;



    private void Awake()
    {
    }

    private void OnEnable()
    {
        ARSessionFactory.SessionInitialized += HandleSessionInitialized;
        BottleActions.OnBottlePrefabSent += RecievePrefab;
        BottleActions.OnWayspotRequested += HandleNewWayspot;
    }

    private void OnDisable()
    {
        ARSessionFactory.SessionInitialized -= HandleSessionInitialized;
        BottleActions.OnBottlePrefabSent -= RecievePrefab;
        BottleActions.OnWayspotRequested -= HandleNewWayspot;

    }

    private void RecievePrefab (GameObject bottlePrefab)
    {
        _bottlePrefab = bottlePrefab;
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
            Debug.LogError("WayspotAnchorService creation unsuccesfull");
            return;

    }

    private void HandleNewWayspot(Bottle newBottle)
    {
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

    public void SaveWayspotAnchors()
    {
        if(_wayspotAnchorTrackers.Count > 0)
        {
            var wayspotAnchors = wayspotAnchorService.GetAllWayspotAnchors();

            var savableAnchors = wayspotAnchors.Where(a => a.Status == WayspotAnchorStatusCode.Limited || a.Status == WayspotAnchorStatusCode.Success);
            var payloads = savableAnchors.Select(a => a.Payload);
            Debug.Log("Array Payload = " + payloads.Count());
            WayspotAnchorDataUtility.SaveLocalPayloads(payloads.ToArray());
            _savedAnchorTrackers.Add(wayspotAnchors);
        }
        else
        {
            WayspotAnchorDataUtility.SaveLocalPayloads(Array.Empty<WayspotAnchorPayload>());
        }
        UpdateLocalisationStatus($"Saved {_wayspotAnchorTrackers.Count} Wayspot Anchors.");
    }

    public void LoadWayspotAnchors()
    {
        var payloads = WayspotAnchorDataUtility.LoadLocalPayloads();
        if (payloads.Length > 0)
        {
            foreach(var payload in payloads)
            {
                var anchors = wayspotAnchorService.RestoreWayspotAnchors(payload);
                if (anchors.Length == 0)
                {
                    Debug.Log("error raised in CreateWayspotAnchors");
                    return; 
                }

                CreateLoadedGameObject(anchors[0], Vector3.zero, Quaternion.identity, true);
            }
            Debug.Log($"Ammount of anchors in payload: {payloads.Count()}");
        }
        else
        {
            UpdateLocalisationStatus("No anchors to load");
        }
    }

    private GameObject CreateLoadedGameObject
        (
            IWayspotAnchor anchor,
            Vector3 position,
            Quaternion rotation,
            bool startActive
        )
    {
        var go = Instantiate(_bottlePrefab, position, rotation);
        var tracker = go.GetComponent<WayspotAnchorTracker>();
        if (tracker == null)
        {
            return null;
        }

        tracker.gameObject.SetActive(startActive);
        tracker.AttachAnchor(anchor);
        _wayspotAnchorTrackers.Add(tracker);

        BottleActions.OnBottleLoaded(go);

        return go;
    }

    public void RestartWayspotAnchorService()
    {
        ClearAnchorGameObjects();
        wayspotAnchorService.Restart();
        WayspotAnchorDataUtility.ClearLocalPayloads();

        UpdateLocalisationStatus("Wayspot Anchor Service Restarted");
    }
    
    public void ClearAnchorGameObjects()
    {
        if (_wayspotAnchorTrackers.Count == 0)
        {
            UpdateLocalisationStatus("No anchors to clear");
            return;
        }

        foreach (var anchor in _wayspotAnchorTrackers)
            Destroy(anchor.gameObject);

        var wayspotAnchor = _wayspotAnchorTrackers.Select(go => go.WayspotAnchor).ToArray();
        wayspotAnchorService.DestroyWayspotAnchors(wayspotAnchor);

        _wayspotAnchorTrackers.Clear();
        UpdateLocalisationStatus("Cleared Wayspot Anchors");
    }

    private void PlaceAnchor(Matrix4x4 localPose, GameObject bottle)
    {
        var anchors = wayspotAnchorService.CreateWayspotAnchors(localPose);
        if (anchors == null)
            return;
        if (anchors.Length == 0)
            return; //error raised in CreateWayspotAnchors

        CreateWayspotTracker(anchors[0], bottle, true);


        UpdateLocalisationStatus("Anchor placed but not saved");
        SaveWayspotAnchors();
    }

    private void CreateWayspotTracker (IWayspotAnchor anchor, GameObject bottle, bool startActive)
    {
        var tracker = bottle.GetComponent<WayspotAnchorTracker>();
        if (tracker == null)
        {
            Debug.Log("Anchor prefab was missing WayspotAnchorTracher, so one will be added. Anchor prefab was missing WayspotAnchorTracker, so one will be added. Please consider adding a WayspotAnchorTracker to the Prefab");
            tracker = bottle.AddComponent<WayspotAnchorTracker>();
        }

        tracker.gameObject.SetActive(startActive);
        tracker.AttachAnchor(anchor);
        _wayspotAnchorTrackers.Add(tracker);
    }


    #region Handling of AR Session
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
    #endregion
}
