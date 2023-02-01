
using System;
using System.Collections.Generic;
using System.Linq;


using Niantic.ARDK.AR;
using Niantic.ARDK.AR.ARSessionEventArgs;
using Niantic.ARDK.Utilities;

using Niantic.ARDK.AR.WayspotAnchors;
using Niantic.ARDK.Extensions;
using Niantic.ARDK.LocationService;

using Niantic.ARDKExamples.WayspotAnchors;
using Niantic.LightshipHub.Templates;
using UnityEngine;
using static Niantic.LightshipHub.Templates.LeaveMessagesUtility;

public class CustomWaySpotAnchorManager : MonoBehaviour
{
    [SerializeField]
    private GameObject _bottlePrefab;

    private IARSession _arSession;
    public WayspotAnchorService wayspotAnchorService;

    private IWayspotAnchorsConfiguration _config;

    private readonly HashSet<WayspotAnchorTracker> _wayspotAnchorTrackers = 
        new HashSet<WayspotAnchorTracker>();
    private HashSet<IWayspotAnchor[]> _savedAnchorTrackers = new HashSet<IWayspotAnchor[]>();

    private List<string> _wayspotAnchorMessages = new List<string>();

    public delegate void TextUpdateHandler(string localisationStatus);
    public event TextUpdateHandler Updated;

    private string _localisationStatus;



    [SerializeField]
    private BottleManager bottleManager;

    private void Awake()
    {
        //Remember to set a user ID for launch
        //var userId = GetCurrentUserId();
        //ArdkGlobalConfig.SetUserIdOnLogin(userId);
        //UpdateLocalisationStatus("Initializing Session...");
    }

    private void OnEnable()
    {
        ARSessionFactory.SessionInitialized += HandleSessionInitialized;

        BottleActions.OnBottlePrefabSent += RecievePrefab;
        //!!!!! find a better place
        BottleActions.OnWayspotRequested += HandleNewWayspot;
    }

    private void OnDisable()
    {
        ARSessionFactory.SessionInitialized -= HandleSessionInitialized;

        BottleActions.OnBottlePrefabSent -= RecievePrefab;
        //!!!!! find a better place
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

    public void SaveWayspotAnchors()
    {
        var wayspotData = new LeaveMessagesUtility.MessagesData();

        if(_wayspotAnchorTrackers.Count > 0)
        {
            var wayspotAnchors = wayspotAnchorService.GetAllWayspotAnchors();
            var payloads = wayspotAnchors.Select(a => a.Payload);
            //var savableAnchors = wayspotAnchors.Where(a => a.Status == WayspotAnchorStatusCode.Limited || a.Status == WayspotAnchorStatusCode.Success);
            //var payloads = savableAnchors.Select(a => a.Payload);

            wayspotData.Payloads = payloads.Select(a => a.Serialize()).ToArray();
            wayspotData.Messages = _wayspotAnchorMessages.ToArray();
            
            Debug.Log("Array Payload = " + payloads.Count());
            //WayspotAnchorDataUtility.SaveLocalPayloads(payloads.ToArray());
            //_savedAnchorTrackers.Add(wayspotAnchors);
        }
        else
        {
            WayspotAnchorDataUtility.SaveLocalPayloads(Array.Empty<WayspotAnchorPayload>());
        }

        LeaveMessagesUtility.SaveLocalMessages(wayspotData);
        UpdateLocalisationStatus($"Saved {_wayspotAnchorTrackers.Count} Wayspot Anchors.");
    }

    public void LoadWayspotAnchors()
    {
        var _messagesData = LeaveMessagesUtility.LoadLocalMessages();



        var messages = _messagesData.Messages;
        var payloads = new List<WayspotAnchorPayload>();

        foreach (var wayspotAnchorPayload in _messagesData.Payloads)
        {
            var payload = WayspotAnchorPayload.Deserialize(wayspotAnchorPayload);
            payloads.Add(payload);
        }

        if (payloads.Count > 0)
        {
            
            for (int i = 0; i < payloads.Count; i++)
            {
                var anchors = wayspotAnchorService.RestoreWayspotAnchors(payloads[i]);
                if (anchors.Length == 0)
                {
                    Debug.Log("error raised in CreateWayspotAnchors");
                    return;
                }
                CreateLoadedGameObject(anchors[0], Vector3.zero, Quaternion.identity, true, messages[i]);
            }
        
        }
        else
        {
            Debug.Log("No messages to load");
        }
    }

    private GameObject CreateLoadedGameObject
        (
            IWayspotAnchor anchor,
            Vector3 position,
            Quaternion rotation,
            bool startActive,
            string message 
        )
    {


        var go = Instantiate(_bottlePrefab, position, rotation);

        var tracker = go.GetComponent<WayspotAnchorTracker>();
        if (tracker == null)
        {
            Debug.Log("Anchor prefab was missing WayspotAnchorTracker, so one will be added.");
            tracker = go.AddComponent<WayspotAnchorTracker>();
            //return null;
        }

        tracker.gameObject.SetActive(startActive);
        tracker.AttachAnchor(anchor);
        _wayspotAnchorTrackers.Add(tracker);
        _wayspotAnchorMessages.Add(message);

        BottleActions.OnBottleLoaded(go, message);

        return go;
    }

    
    public void RestartWayspotAnchorService()
    {


        wayspotAnchorService.Restart();
        WayspotAnchorDataUtility.ClearLocalPayloads();

        

        ClearAnchorGameObjects();
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
        {
            Destroy(anchor.gameObject);
        }
        _wayspotAnchorMessages = new List<string>();

        var wayspotAnchor = _wayspotAnchorTrackers.Select(go => go.WayspotAnchor).ToArray();
        wayspotAnchorService.DestroyWayspotAnchors(wayspotAnchor);

        _wayspotAnchorTrackers.Clear();
        _wayspotAnchorMessages.Clear();
        UpdateLocalisationStatus("Cleared Wayspot Anchors");
    }



    private void PlaceAnchor(Matrix4x4 localPose, GameObject bottleGO)
    {
        var anchors = wayspotAnchorService.CreateWayspotAnchors(localPose);
        if (anchors == null)
            return;
        if (anchors.Length == 0)
            return; //error raised in CreateWayspotAnchors

        var bottle = bottleGO.GetComponent<Bottle>();


        CreateWayspotTracker(anchors[0], bottleGO, true, bottle.encodedMessage);


        UpdateLocalisationStatus("Anchor placed... well done btw <3");
        SaveWayspotAnchors();
    }

    private void CreateWayspotTracker 
        (
         IWayspotAnchor anchor, 
         GameObject bottle, 
         bool startActive,
         string message
        )
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
        _wayspotAnchorMessages.Add(message);
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
        UpdateLocalisationStatus("Session Initialized");
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
