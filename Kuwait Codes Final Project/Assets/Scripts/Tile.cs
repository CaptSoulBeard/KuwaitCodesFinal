using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    //First we set our parameters with fixed values.

    public bool walkable = true;
    public bool current = false;
    public bool target = false;
    public bool selectable = false;

    public List<Tile> adjacencyList = new List<Tile>();

    //Needed BFS (Breadth First Search)

    public bool visited = false;
    public Tile parent = null;
    public int distance = 0;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //This if statement sets it so if the tile gets labeled as current/target/selectable or none of them. The tile gets colored as defined.
        if (current)
        {
            GetComponent<Renderer>().material.color = Color.magenta;
        } else if (target)
        {
            GetComponent<Renderer>().material.color = Color.green;
        } else if (selectable)
        {
            GetComponent<Renderer>().material.color = Color.red;
        } else
        {
            GetComponent<Renderer>().material.color = Color.white;
        }
    }

    //This void, if called resets the initial parameters.

    public void Reset()
    {
    adjacencyList.Clear();
    current = false;
    target = false;
    selectable = false;
    visited = false;
    parent = null;
    distance = 0;
    }

    //This void resets the initial parameters and finds the neighbouring tiles including the player's jump height.
    
    public void FindNeighbours(float jumpHeight)
    {
        Reset();

        CheckTile(Vector3.forward, jumpHeight);
        CheckTile(-Vector3.forward, jumpHeight);
        CheckTile(Vector3.right, jumpHeight);
        CheckTile(-Vector3.right, jumpHeight);
    }

    //This void is to check if there is another player/inaccessible tiles directly from the center of the tile with the halfExtents vector. (Too high or set as inaccessible)
    //It also checks an array of colliders to see if the tile the player chooses is actually accessible and ignores it if it isn't. This is done by a ray cast shot upwards from the center of the tile.

    public void CheckTile(Vector3 direction, float jumpHeight)
    {
        Vector3 halfExtents = new Vector3(0.25f, (1 + jumpHeight) / 2.0f, 0.25f);
        Collider[] colliders = Physics.OverlapBox(transform.position + direction, halfExtents);

        foreach (Collider item in colliders)
        {
            Tile tile = item.GetComponent<Tile>();
            if (tile != null && tile.walkable)
            {
                RaycastHit hit;

                if (!Physics.Raycast(tile.transform.position, Vector3.up, out hit, 1))
                {
                    adjacencyList.Add(tile);
                }

            }
        }
    }

}