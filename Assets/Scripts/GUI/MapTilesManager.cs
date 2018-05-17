using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(AspectRatioFitter))]
public class MapTilesManager : UIBehaviour
{
    class TileBuildHelper : System.Tuple<Vector2Int, ObstacleType>
    {
        public Vector2Int Point { get { return Item1; } }
        public ObstacleType Obstacle { get { return Item2; } }

        public TileBuildHelper(Vector2Int item1, char item2) : base(item1, item2.TranslateToObstacleType())
        {
        }

        public TileBuildHelper(Vector2Int item1, ObstacleType item2) : base(item1, item2)
        {
        }
    }

    #region Obstacle prefabs
    [SerializeField]
    private GameObject ObstacleOneByOnePrefab;

    [SerializeField]
    private GameObject ObstacleTwoByOnePrefab;

    [SerializeField]
    private GameObject ObstacleOneByTwoPrefab;

    [SerializeField]
    private GameObject ObstacleTwoByTwoPrefab;

    [SerializeField]
    private GameObject FreeTile;
    #endregion
    #region Color definitions
    [SerializeField]
    private Color PathStartTileColor;

    [SerializeField]
    private Color PathMemberTileColor;

    [SerializeField]
    private Color PathFinishTileColor;
    #endregion

    private char[,] currentlyDisplayedMap;
    private Dictionary<Vector2Int, MapTileScript> tileColorDictionary;
    private TileBuildHelper[] obstacleList;

    private RectTransform rectTransform;

    private int MapWidth { get { return currentlyDisplayedMap != null ? currentlyDisplayedMap.GetLength(0) : 0; } }
    private int MapHeight { get { return currentlyDisplayedMap != null ? currentlyDisplayedMap.GetLength(1) : 0; } }

    private Vector2Int[] currentlyDisplayedPath;

    public void CleanPreviousMap()
    {
        GameObject[] children = new GameObject[transform.childCount];
        for(int i = 0; i < transform.childCount; ++i)
        {
            //Destroy(transform.GetChild(i).gameObject);
            children[i] = transform.GetChild(i).gameObject;
        }        

        StartCoroutine(DestroyOldTiles(children));
    }

    public void DisplayMap(char[,] map, System.Action interfaceEnablingAction)
    {
        if (rectTransform == null)
        {
            rectTransform = GetComponent<RectTransform>();
        }

        currentlyDisplayedMap = (map == null)? currentlyDisplayedMap : map;
        currentlyDisplayedPath = null;

        SetTileSize();

        /*
        TileBuildHelper[] obstacleList = CreateObstacleList();
        CreateMapTiles(obstacleList);
        */

        StartCoroutine(MapCreation(interfaceEnablingAction));
    }

    public void DisplayPath(Vector2Int[] pathCoords)
    {
        RevertPathColors();
        currentlyDisplayedPath = pathCoords;
        ApplyPathColors();
    }

    protected override void OnRectTransformDimensionsChange()
    {
        base.OnRectTransformDimensionsChange();
        SetTileSize();
    }

    private void SetTileSize()
    {
        if (currentlyDisplayedMap != null && currentlyDisplayedMap.GetLength(1) > 0)
        {
            float guiMapSize = rectTransform.rect.height;
            float tileSize = guiMapSize / currentlyDisplayedMap.GetLength(1);

            MapTileScript.SetUnitSize(tileSize);
        }
    }    

    private IEnumerator MapCreation(System.Action interfaceEnablingAction)
    {
        yield return CreateObstacleList();
        yield return CreateMapTiles(obstacleList);

        interfaceEnablingAction.Invoke();
    }

    private IEnumerator CreateObstacleList()
    {
        int ceiling = 3000;
        int counter = 0;

        List<TileBuildHelper> result = new List<TileBuildHelper>();

        for(int x = 0; x < MapWidth; x++)
        {
            for (int y = 0; y < MapHeight; y++)
            {
                if ((++counter) % ceiling == 0) yield return new WaitForEndOfFrame();

                ObstacleType obstacle = Translate(x, y);

                result.Add(new TileBuildHelper(new Vector2Int(x, y), obstacle));
                /*

                if (IsStartPointOfObstacle(obstacle))
                {
                    result.Add(new TileBuildHelper(new Vector2Int(x, y), obstacle));
                } 
                */               
            }
        }

        obstacleList = result.ToArray();
    }

    private IEnumerator CreateMapTiles(TileBuildHelper[] obstaclesToInsert)
    {
        if (tileColorDictionary == null)
        {
            tileColorDictionary = new Dictionary<Vector2Int, MapTileScript>();
        }

        tileColorDictionary.Clear();

        int ceiling = 100;
        int counter = 0;

        foreach (TileBuildHelper obstacle in obstaclesToInsert)
        {
            if(obstacle.Obstacle == ObstacleType.PartOfObstacle)
            {
                continue;
            }

            if ((++counter) % ceiling == 0) yield return new WaitForEndOfFrame();

            GameObject instancedMapElement = null;
            MapTileScript instancedTileScript = null;

            switch(obstacle.Obstacle)
            {
                case ObstacleType.None:
                    instancedMapElement = Instantiate(FreeTile, this.transform);
                    break;

                case ObstacleType.OneByOne:
                    instancedMapElement = Instantiate(ObstacleOneByOnePrefab, this.transform);
                    break;

                case ObstacleType.OneByTwo:
                    instancedMapElement = Instantiate(ObstacleOneByTwoPrefab, this.transform);
                    break;

                case ObstacleType.TwoByOne:
                    instancedMapElement = Instantiate(ObstacleTwoByOnePrefab, this.transform);
                    break;

                case ObstacleType.TwoByTwo:
                    instancedMapElement = Instantiate(ObstacleTwoByTwoPrefab, this.transform);
                    break;

            }

            instancedTileScript = instancedMapElement.GetComponent<MapTileScript>();
            instancedTileScript.SetTilePosition(obstacle.Point);

            tileColorDictionary.Add(obstacle.Point, instancedTileScript);
        }
    }

    private IEnumerator DestroyOldTiles(GameObject[] oldTiles)
    {
        int ceiling = 3000;
        int counter = 0;

        //transform.DetachChildren();

        for (int i = 0; i < oldTiles.Length; ++i)
        {
            ++counter;

            if (counter % ceiling == 0) yield return new WaitForEndOfFrame();

            Destroy(oldTiles[i]);
        }
    }

    private ObstacleType Translate(int x, int y)
    {
        return currentlyDisplayedMap[x, y].TranslateToObstacleType();
    }

    private bool IsStartPointOfObstacle(ObstacleType obstacle)
    {
        return obstacle != ObstacleType.None && obstacle != ObstacleType.PartOfObstacle;
    }

    private bool IsStartPointOfObstacle(char c)
    {
        return IsStartPointOfObstacle(c.TranslateToObstacleType());
    }

    private void ApplyPathColors()
    {
        if (currentlyDisplayedPath != null && currentlyDisplayedPath.Length > 0)
        {
            int lastIndex = currentlyDisplayedPath.Length - 1;

            tileColorDictionary[currentlyDisplayedPath[0]].ChangeTileColor(PathStartTileColor);
            tileColorDictionary[currentlyDisplayedPath[lastIndex]].ChangeTileColor(PathFinishTileColor);

            for (int i = 1; i < lastIndex; ++i)
            {
                tileColorDictionary[currentlyDisplayedPath[i]].ChangeTileColor(PathMemberTileColor);
            }
        }
    }

    private void RevertPathColors()
    {
        if (currentlyDisplayedPath != null)
        {
            foreach(Vector2Int coord in currentlyDisplayedPath)
            {
                tileColorDictionary[coord].RevertTileColor();
            }

        }
    }
}
