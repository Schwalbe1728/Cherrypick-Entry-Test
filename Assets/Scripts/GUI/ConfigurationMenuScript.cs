using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConfigurationMenuScript : MonoBehaviour
{
    [SerializeField]
    private MapDisplayScript mapDisplay;

    [SerializeField]
    private WarningPopupScript warningPopup;

    [SerializeField]
    private GameObject GUIBlocker;
    private IPathfinder pathfinder;

    private Vector2Int pathStart = Vector2Int.left;
    private Vector2Int pathFinish = Vector2Int.left;

    public void SetAStarPathfinder()
    {
        pathfinder = new AStar();
    }

    public void SetDijkstraPathfinder()
    {
        pathfinder = new Dijkstra();
    }

    public void SetNumberOfObstacles(string arg)
    {
        int m;

        if (int.TryParse(arg, out m))
        {
            mapDisplay.SetM(m, DisplayCommunicate, warningPopup.ClosePopup);
        }
        else
        {
            if (!arg.Equals(""))
            {
                DisplayCommunicate("Error: Incorrect argument.");
            }
        }
    }    

    public void SetMapSize(string arg)
    {
        int n;

        if(int.TryParse(arg, out n))
        {
            mapDisplay.SetN(n, DisplayCommunicate, warningPopup.ClosePopup);
        }
        else
        {
            if (!arg.Equals(""))
            {
                DisplayCommunicate("Error: Incorrect argument.");
            }
        }
    }

    public void SetAlgorithm(int dropdownChoice)
    {
        switch(dropdownChoice)
        {
            case 0:
                SetAStarPathfinder();
                break;

            case 1:
                SetDijkstraPathfinder();
                break;
        }
    }

    public void GenerateMap()
    {
        DisableInterface();
        mapDisplay.RequestMapGeneration();
    }

    public void ChoosePathEndpoints()
    {
        bool validEnd;        

        pathStart = pathfinder.GetFreeField(Vector2Int.left, out validEnd);

        if (!validEnd)
        {
            DisplayCommunicate("Couldn't find valid map points for start and finish");
            return;
        }

        pathFinish = pathfinder.GetFreeField(pathStart, out validEnd);

        if (!validEnd)
        {
            DisplayCommunicate("Couldn't find valid map points for start and finish");
            return;
        }

        Vector2Int[] pathEnds = { pathStart, pathFinish };
        mapDisplay.HighlightPath(pathEnds);
    }

    public void GeneratePathfindingTask()
    {
        LoadMapToPathfinder();

        if (pathStart.Equals(Vector2Int.left) || pathFinish.Equals(Vector2Int.left))
        {
            ChoosePathEndpoints();
        }

        Vector2Int[] calculatedPath = pathfinder.CalculatePath(pathStart, pathFinish);

        if(calculatedPath == null || calculatedPath.Length < 1)
        {
            DisplayCommunicate("Path not found! (" + pathStart + " => " + pathFinish + ")");
            return;
        }
        else
        {
            warningPopup.ClosePopup();
            mapDisplay.HighlightPath(calculatedPath);
        }        
    }

    public void SaveMap()
    {
        mapDisplay.RequestSaveMap(DisplayCommunicate);
    }

    public void LoadMap()
    {
        mapDisplay.RequestLoadMap(DisplayCommunicate, EnableInterface);
    }

    void Awake()
    {
        SetAStarPathfinder();
        EnableInterface();

        mapDisplay.RegisterToOnMapFinished(EnableInterface);
        mapDisplay.RegisterToOnMapFinished(LoadMapToPathfinder);
        mapDisplay.RegisterToOnMapFinished(ChoosePathEndpoints);
    }

    private void LoadMapToPathfinder()
    {
        pathfinder.LoadMap(mapDisplay.GetMapTemplate());
    }
    
    private void DisplayCommunicate(string message)
    {
        warningPopup.OpenPopup(message);
        // wypisz komunikat w GUI
        //Debug.Log(message);
    }    	

    private void DisableInterface()
    {
        GUIBlocker.SetActive(true);
    }

    private void EnableInterface()
    {
        GUIBlocker.SetActive(false);
    }
}
