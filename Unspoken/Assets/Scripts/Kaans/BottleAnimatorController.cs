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
        animBottle.SetBool("Selected", true);
    }
    public void BottleClosing()
    {
        animBottle.SetBool("Selected", false);
    }

    public void BottleOpen(bool open_)
    {
        animBottle.SetBool("Open", open_);
    }



}
