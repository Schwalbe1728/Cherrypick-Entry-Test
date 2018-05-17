using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapDisplayScript : MonoBehaviour
{
    private MapTilesManager tilesManager;
    private MapSerializationScript serializationScript;

    private MapGenerator mapGenerator;
    private char[,] mapTemplate;

    public void SetN(int n, System.Action<string> messageAction, System.Action successAction)
    {
        if(!mapGenerator.SetNParameter(n))
        {
            messageAction.Invoke("The minimal size of map is " + MapGenerator.MinSize + ".");
        }
        else
        {
            successAction.Invoke();
        }
    }

    public void SetM(int m, System.Action<string> messageAction, System.Action successAction)
    {
        if (!mapGenerator.SetMParameter(m))
        {
            messageAction.Invoke("The minimal size of map is " + MapGenerator.MinObstacles + ".");
        }
        else
        {
            successAction.Invoke();
        }
    }

    public void RequestMapGeneration(System.Action interfaceDisablingFunction, System.Action interfaceEnablingFunction)
    {
        interfaceDisablingFunction.Invoke();
        mapTemplate = mapGenerator.GenerateMap();
        DisplayMap(interfaceEnablingFunction);
    }

    public void HighlightPath(Vector2Int[] path)
    {
        tilesManager.DisplayPath(path);
    }

    public char[,] GetMapTemplate()
    {
        return mapTemplate;
    }    

    public void RequestSaveMap(System.Action<string> messageAction)
    {
        if(!serializationScript.SaveMap(mapTemplate))
        {
            messageAction.Invoke("Save Failed");
        }
    }

    public void RequestLoadMap(System.Action<string> messageAction, System.Action interfaceEnablingAction)
    {
        if (serializationScript.LoadMap(out mapTemplate))
        {
            DisplayMap(interfaceEnablingAction);
        }
        else
        {
            messageAction.Invoke("Load Failed");
            interfaceEnablingAction.Invoke();
        }
    }

    void Start()
    {
        mapGenerator = new MapGenerator();
        tilesManager = GetComponentInChildren<MapTilesManager>();
        serializationScript = GetComponent<MapSerializationScript>();
        //RequestMapGeneration();        
    }

    private void DisplayMap(System.Action interfaceEnablingAction)
    {
        tilesManager.CleanPreviousMap();
        tilesManager.DisplayMap(mapTemplate, interfaceEnablingAction);
    }
}
