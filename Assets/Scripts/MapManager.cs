//Sabrina Jackson
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UIElements;

public class MapManager : MonoBehaviour
{
    private static MapManager _instance;
    //add prefab for start point
    public OverlayTile startTilePrefab;
    public GameObject container;
    
    public static MapManager Instance
    {
        get { return _instance; }
    }

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }
    }

    void Start()
    {
        var tileMap = gameObject.GetComponentInChildren<Tilemap>();
        BoundsInt bounds = tileMap.cellBounds;

        //loop through all tiles
        for (int i = bounds.max.z; i > bounds.min.z; i--)
        {
            for (int j = bounds.min.y; j < bounds.max.y; j++)
            {
                for (int k = bounds.min.x; k < bounds.max.x; k++)
                {
                    var tileLocation = new Vector3Int(k, j, i);
                    if (tileMap.HasTile(tileLocation))
                    {
                        var select = Instantiate(startTilePrefab, container.transform);
                        var cellWorldPosition = tileMap.GetCellCenterWorld(tileLocation);
                        
                        select.transform.position = new Vector3(cellWorldPosition.x, cellWorldPosition.y, cellWorldPosition.z + 1);
                        select.GetComponent<SpriteRenderer>().sortingOrder = tileMap.GetComponent<TilemapRenderer>().sortingOrder;
                    }
                }
            }
        }
    }
}
