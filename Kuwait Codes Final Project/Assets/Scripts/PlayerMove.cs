using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//In the next step I worked on the player movement and made it so it was derived form a script called 'TacticsMove' because this script is going to be shared with a "bot" once I create one, instead this will be a multiplayer experience.

public class PlayerMove : TacticsMove
{
    // Start is called before the first frame update
    void Start()
    {
        Init();
    }

    // Update is called once per frame
    void Update()
    {
        //This debug is only to get a glimpse of where the unit is facing while moving.

        Debug.DrawRay(transform.position, transform.forward);

        //This if statement is telling the unit that if it's not it's turn that it should just return and not go to the move code. Basically it is disabled until it's turn.

        if (!turn)
        {
            return;
        }

        if (!moving)
        {
            FindSelectableTiles();
            CheckMouse();
        } else
        {
            //Add a move function
            Move();
        }
    }

    //This void sends a 3D ray towards the map to determine where the player has clicked. If they clicked on a slectable tile, it will also turn green to signify the selected tile.
    
    void CheckMouse()
    {
        if (Input.GetMouseButtonUp(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider.tag == "Tile")
                {
                    Tile t = hit.collider.GetComponent<Tile>();

                    if (t.selectable)
                    {
                        MoveToTile(t);
                    }
                }
            }
        }
    }
}