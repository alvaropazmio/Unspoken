/////////////////////////////////////////////////////////////////////
//Purpose: Static class that listens to important actions from the //
//bottles and alerts other scripts                                 //
//Developer: Alvaro Pazmi�o                                        //
/////////////////////////////////////////////////////////////////////
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Niantic.ARDK.AR.WayspotAnchors;
using Niantic.ARDK.Extensions;

public static class BottleActions
{

    public static Action <GameObject> OnBottlePrefabSent;

    public static Action <Bottle> OnWayspotRequested;
    public static Action <GameObject> OnBottleLoaded;
}
