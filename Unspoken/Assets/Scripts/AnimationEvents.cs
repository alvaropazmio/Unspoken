/////////////////////////////////////////////////////////////////////
//Purpose: Class housing events related to the bottle animation    //
//not written in BottleActions.cs because these functions must be  //
//selected on the editor.                                          //
//Developer: Alvaro Pazmiño                                        //
/////////////////////////////////////////////////////////////////////
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.Events;

public class AnimationEvents : MonoBehaviour
{
    public static Action bottleClosed;
    public static Action bottleOpened;

    public void ClosingAnimationEnd()
    {
        bottleClosed?. Invoke();
    }

    public void OpeningAnimationEnd()
    {
        bottleOpened?.Invoke();
    }
}
