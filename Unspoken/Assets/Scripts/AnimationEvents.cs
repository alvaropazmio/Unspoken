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
