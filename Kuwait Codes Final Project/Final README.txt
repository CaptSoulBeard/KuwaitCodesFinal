//For the camera I made a simple 2D UI with 2 buttons to rotate the camera by 45degree angles. The code simply tells the object to rotate by our defined degree. In this case our
child object was the camera.

`

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

`

//In the next step I worked on the player movement and made it so it was derived form a script called 'TacticsMove' because this script is going to be shared with an "bot"
once I create one, instead this will be a multiplayer experience.

`

public class PlayerMove : TacticsMove

`

//Next up I worked on the script for the tile to make it understand what is going on on top of it. In this case I make a bool function that makes the tile 'current' if a
player or NPC is currently standing on it.

`

public bool current = false;
public bool target = false;
public bool selectable = false;

`

//Next up we make an adjacency list so that our graphy theory BFS identifies the neighburing tiles for things that are present. First up we set a rest void to get called on
whenever we need the tile to reset it's code, we then add the neighbour finding code to search tiles the are neighbouring to the current with a jumpheight to check higher blocks.

`

public void Reset()
    {
    current = false;
    target = false;
    selectable = false;
    visited = false;
    parent = null;
    distance = 0;
    }

    public void FindNeighbours(float jumpHeight)
    {
        Reset();
        CheckTile(Vector3.forward, jumpHeight);
        CheckTile(Vector3.-forward, jumpHeight);
        CheckTile(Vector3.right, jumpHeight);
        CheckTile(Vector3.-right, jumpHeight);
    }

    public void CheckTile(Vector3 direction, float jumpHeight)
    {

    }

`