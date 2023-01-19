using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BottleAnimatorController : MonoBehaviour
{
    [SerializeField] private Animator animBottle;


    public void PlayTap()
    {
        animBottle.SetTrigger("Tap");
    }

    public void BottleOpening()
    {
        animBottle.SetBool("Open", true);
    }
    public void BottleClosing()
    {
        animBottle.SetBool("Open", false);
    }





}
