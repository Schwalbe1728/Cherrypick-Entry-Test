using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MapTileScript : MonoBehaviour
{
    private delegate void UnitSizeChanged();

    private static float OneUnitToPixels = 40;
    private static UnitSizeChanged OnUnitSizeChanged;

    [SerializeField]
    [Range(1, 2)]
    private int Width = 1;

    [SerializeField]
    [Range(1, 2)]
    private int Height = 1;

    [SerializeField]
    private Vector2Int MapPosition;

    private RectTransform rectTransform;
    private Image image;

    private Color tileColorBackup;    

    public static void SetUnitSize(float size)
    {
        OneUnitToPixels = size;

        if(OnUnitSizeChanged != null)
        {
            OnUnitSizeChanged();
        }
    }

    public void SetTilePosition(Vector2Int point)
    {
        MapPosition = point;
        ChangeTilePosition();
    }

    public void ChangeTileColor(Color _color)
    {
        image.color = _color;
    }

    public void RevertTileColor()
    {
        image.color = tileColorBackup;
    }

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        image = GetComponent<Image>();

        tileColorBackup = image.color;

        ChangeTileSize();

        OnUnitSizeChanged += ChangeTileSize;
    }

    void OnValidate()
    {
        if(rectTransform == null)
        {
            rectTransform = GetComponent<RectTransform>();
        }

        ChangeTileSize();
    }

    void OnDestroy()
    {
        OnUnitSizeChanged -= ChangeTileSize;
    }

    private void ChangeTilePosition()
    {
        rectTransform.anchoredPosition =
            new Vector2(MapPosition.x, -MapPosition.y) * OneUnitToPixels;
    }

    private void ChangeTileSize()
    {
        if(rectTransform != null)
        {
            rectTransform.sizeDelta =
                new Vector2(Width, Height) * OneUnitToPixels;

            ChangeTilePosition();
        }
    }
}
