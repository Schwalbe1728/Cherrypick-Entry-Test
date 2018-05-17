using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestAStar : MonoBehaviour
{
    [SerializeField]
    private Vector2Int start;

    [SerializeField]
    private Vector2Int finish;

    private AStar pathfinding;

    private MapGenerator mapGenerator;
    

	// Use this for initialization
	void Start ()
    {
        pathfinding = new AStar();
        mapGenerator = new MapGenerator(12, 6);
	}
	
	// Update is called once per frame
	void Update ()
    {
	    if(Input.GetKeyUp(KeyCode.T))
        {
            pathfinding.LoadMap( mapGenerator.GenerateMap() );
            Vector2Int[] path = pathfinding.CalculatePath(start, finish);
            //pathfinding.AsyncCalculatePath(start, finish).;

            if (path == null || path.Length < 1)
            {
                Debug.Log("Path not found");
            }
            else
            {
                Debug.Log("Path found: " + path.Length);
            }

        }
	}
}
