using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotating : MonoBehaviour
{
    public Joystick joystick;
    public float rotateVertical;
    public float rotateHorizontal;
    public float rotationSpeed = 2f;


    public float speed = 0.01f;
    public float timeCount = 0.0f;

    bool JoystickAvailable = true;

    public void FixJoystick()
    {
        rotateVertical = joystick.Vertical * rotationSpeed;
        rotateHorizontal = joystick.Horizontal * -rotationSpeed;
        transform.Rotate(rotateVertical, 0, rotateHorizontal);

    }

    private void FixedUpdate()
    {

        if (JoystickAvailable) FixJoystick();

    }


    public void TransformZero()
    {

        JoystickAvailable = false;
        RotateLerp();
    }
    private void RotateLerp()
    {
        timeCount = 0.0f;

        transform.rotation = Quaternion.Lerp(transform.rotation, new Quaternion(0, 0, 0, 0), timeCount * speed);
        timeCount = timeCount + Time.deltaTime;
        if (transform.rotation.y < 0.01) transform.rotation = new Quaternion(0, 0, 0, 0);
        JoystickAvailable = true;

    }
}
