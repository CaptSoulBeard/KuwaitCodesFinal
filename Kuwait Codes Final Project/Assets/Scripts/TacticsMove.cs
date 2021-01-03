using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TacticsMove : MonoBehaviour
{
    //We created the new list of tiles in order to reset the list of selectable tiles once the player moves to a new location.

    List<Tile> selectableTiles = new List<Tile>();
    GameObject[] tiles;

    //We made the path a stack so that it can be calculated in reverse because we've made it so the selected tile is actually the beginning while the current position is the end.

    Stack<Tile> path = new Stack<Tile>();
    Tile currentTile;

    public bool moving = false;
    public bool turn = false;
    [SerializeField] public int move = 5;
    [SerializeField] public float jumpHeight = 2;
    [SerializeField] public float moveSpeed = 2;
    [SerializeField] public float jumpSpeed = 3;
    [SerializeField] public float jumpVelocity = 4.5f;

    Vector3 velocity = new Vector3();
    Vector3 heading = new Vector3();

    //We use half height here because the player is half the height away from the center of the tile.

    float halfHeight = 0;

    bool fallingDown = false;
    bool jumpingUp = false;
    bool movingEdge = false;
    Vector3 jumpTarget;

    //We've set up this void to gather all tiles into a single array so that we can use to access them to see their adjacency list. !!!!!NOTE: If the game has disappearing tiles this needs to be called every frame.

    protected void Init()
    {
        tiles = GameObject.FindGameObjectsWithTag("Tile");

        halfHeight = GetComponent<Collider>().bounds.extents.y;

        //"this" uses the current object as the parameter.

        TurnManager.AddUnit(this);
    }

    //Thiis void is the starting point for path finding.

    public void GetCurrentTile()
    {
        currentTile = GetTargetTile(gameObject);
        currentTile.current = true;
    }

    public Tile GetTargetTile(GameObject target)
    {
        RaycastHit hit;
        Tile tile = null;

        if (Physics.Raycast(target.transform.position, -Vector3.up, out hit, 1))
        {
            tile = hit.collider.GetComponent<Tile>();
        }

        return tile;
    }

    public void ComputedAdjacencyLists()
    {
        //tiles = GameObject.FindGameObjectsWithTag("Tile"); !!!!!NOTE: Put this code in here if you want a map the changes size. e.g. Tiles break etc.

        foreach (GameObject tile in tiles)
        {
            Tile t = tile.GetComponent<Tile>();
            t.FindNeighbours(jumpHeight);
        }
    }

    //In this void we are activating the BFS to process the tiles. It starts with one tile and grows outwards. Basically the path keeps getting longer (If I understood this correctly)

    public void FindSelectableTiles()
    {
        ComputedAdjacencyLists();
        GetCurrentTile();

        Queue<Tile> process = new Queue<Tile>();

        process.Enqueue(currentTile);
        currentTile.visited = true;
        //curretTile.parent = ?? leave as null

        while (process.Count > 0)
        {
            Tile t = process.Dequeue();

            selectableTiles.Add(t);
            t.selectable = true;

            //This if statement checks the distance and if we haven't hit the edge yet, then we can look at the neighbours.

            if (t.distance < move)
            {

                foreach (Tile tile in t.adjacencyList)
                {
                    //This sets the selected tile as the new parent to further allow the player to move.

                    if (!tile.visited)
                    {
                        tile.parent = t;
                        tile.visited = true;
                        tile.distance = 1 + t.distance;
                        process.Enqueue(tile);
                    }
                }
            }
        }
    }

    //What this void is doing is basically clearing the previous path and making our new target tile the parent so that the player is capable of moving back to it.

    public void MoveToTile(Tile tile)
    {
        path.Clear();
        tile.target = true;
        moving = true;

        Tile next = tile;
        while (next != null)
        {
            path.Push(next);
            next = next.parent;
        }
    }

    //This void allows us to move a unit from one tile to another.

    public void Move()
    {
        if (path.Count > 0)
        {
            //Peek only looks at the path but does nothing to it
            Tile t = path.Peek();
            Vector3 target = t.transform.position;

            //This line of code calculates the unit's position on top of the target tile.
            target.y += halfHeight + t.GetComponent<Collider>().bounds.extents.y;

            if (Vector3.Distance(transform.position, target) >= 0.05f)
            {

                bool jump = transform.position.y != target.y;

                if (jump)
                {
                    Jump(target);

                } else
                {
                    CalculateHeading(target);
                    SetHorizontalVelocity();
                }


                //This is locomotion (The act of moving)
                transform.forward = heading;
                transform.position += velocity * Time.deltaTime;

            } else
            {
                //In here the code tells the game that the tile's center has been reached.
                transform.position = target;
                path.Pop();
            }

        } else
        {
            RemoveSelectableTiles();
            moving = false;

            TurnManager.EndTurn();

        }
    }

    //This void will reset the tiles so that the selectable tiles reset which removes the shading on the selectable tiles.

    protected void RemoveSelectableTiles()
    {

        if (currentTile != null)
        {
            currentTile.current = false;
            currentTile = null;
        }

        foreach (Tile tile in selectableTiles)
        {
            tile.Reset();
        }

        selectableTiles.Clear();

    }

    void CalculateHeading(Vector3 target)
    {
        heading = target - transform.position;
        //Normalize makes the unit a vector.
        heading.Normalize();
    }

    void SetHorizontalVelocity()
    {
        velocity = heading * moveSpeed;
    }


    //In this void we used a state machine which:
    //First: Determines if the unit will jump up or down (Falling).
    //When a unit "Jumps" he starts in the middle of the tile and leaps to the edge of a higher tile.
    //When a unit falls they move to the edge of the current tile and leap off to the center of the lower tile.

    void Jump(Vector3 target)
    {
        if (fallingDown)
        {
            FallDownward(target);
        } else if (jumpingUp)
        {
            JumpUpward(target);
        } else if (movingEdge)
        {
            MoveToEdge();
        } else
        {
            PrepareJump(target);
        }
    }

    void PrepareJump(Vector3 target)
    {
        float targetY = target.y;

        //This line fo code prevents a unit from tilting when moving upwards to a higher tile.
        target.y = transform.position.y;

        CalculateHeading(target);

        if (transform.position.y > targetY)
        {
            fallingDown = false;
            jumpingUp = false;
            movingEdge = true;

            //This line of code finds the halfway point between the target (Center of another tile) and the current position which is the center of the tile the unit is on at the moment. This will give us the edge.

            jumpTarget = transform.position + (target - transform.position) / 2.0f;
        } else
        {
            fallingDown = false;
            jumpingUp = true;
            movingEdge = false;

            velocity = heading * moveSpeed / jumpSpeed;

            float difference = targetY - transform.position.y;

            velocity.y = jumpVelocity * (0.5f + difference / 2.0f);
        }
    }

    void FallDownward(Vector3 target)
    {
        velocity += Physics.gravity * Time.deltaTime;

        if (transform.position.y <= target.y)
        {
            fallingDown = false;

            Vector3 p = transform.position;
            p.y = target.y;
            transform.position = p;

            velocity = new Vector3();

        }
    }

    void JumpUpward(Vector3 target)
    {
        velocity += Physics.gravity * Time.deltaTime;

        if (transform.position.y > target.y)
        {
            jumpingUp = false;
            fallingDown = true;
        }
    }

    void MoveToEdge()
    {
        if (Vector3.Distance(transform.position, jumpTarget) >= 0.05f)
        {
            SetHorizontalVelocity();
        } else
        {
            movingEdge = false;
            fallingDown = true;

            velocity /= 5.0f;
            velocity.y = 1.5f;
        }
    }

    public void BeginTurn()
    {
        turn = true;
    }

    //Usually ending a turn is added after an action, not after movement. !!!NOTE: Work on adding this for the final project.

    public void EndTurn()
    {
        turn = false;
    }

}