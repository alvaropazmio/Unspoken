using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Niantic.ARDK.AR.WayspotAnchors;
using Niantic.ARDK.Extensions;

public static class BottleActions
{
    //I think this is not in best practices... I need to make a repository for all the values and Gameobjects that are shared between scripts
    //Its 1:12 am... I... I.. :(
    public static Action <GameObject> OnBottlePrefabSent;

    public static Action <Bottle> OnWayspotRequested;
    //public static Action <IWayspotAnchor> OnBottleLoaded;
}
