using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//For the camera I made a simple 2D UI with 2 buttons to rotate the camera by 45degree angles. The code simply tells the object to rotate by our defined degree. In this case our child object was the camera.

public class GameCamera : MonoBehaviour
{
    public void RotateLeft()
    {
        transform.Rotate(Vector3.up, 45, Space.Self);
    }

    public void RotateRight()
    {
        transform.Rotate(Vector3.up, -45, Space.Self);
    }
}
