using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowController : MonoBehaviour
{
    public GameObject _ballPrefab;  //This will store the Ball Prefab we created earlier, so we can spawn a new Ball whenever we want
    private Camera _mainCamera;  //This will reference the MainCamera in the scene, so the ARDK can leverage the device camera
    public float force = 300.0f;
    //This function will be called when the player touches the screen. For us, we'll have this trigger the shooting of our ball from where we touch.

    public void Start()
    {
        _mainCamera = Camera.main;
    }
    public void ThrowObjects()
    {
        //Let's spawn a new ball to bounce around our space
        GameObject newBall = Instantiate(_ballPrefab);  //Spawn a new ball from our Ball Prefab
        newBall.transform.rotation = Quaternion.Euler(new Vector3(0.0f, 0.0f, 0.0f));   //Set the rotation of our new Ball
        newBall.transform.position = _mainCamera.transform.position + _mainCamera.transform.forward;    //Set the position of our new Ball to just in front of our Main Camera

        //Add velocity to our Ball, here we're telling the game to put Force behind the Ball in the direction Forward from our Camera (so, straight ahead)
        Rigidbody rigbod = newBall.GetComponent<Rigidbody>();
        rigbod.velocity = new Vector3(0f, 0f, 0f);

        rigbod.AddForce(_mainCamera.transform.forward * force);
    }
    public void SayHi()
    {



        Debug.Log("hi");
        
    }
}
