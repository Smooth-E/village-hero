using UnityEngine;

public class MapBuilder : MonoBehaviour
{

    private const string GroundTag = "Ground";
    private const string PlatformTag = "Platform";

    [SerializeField] private Transform _levelParent;

    public void InitializeMap(ReimplementedMap map)
    {
        map.Tiles = new TileType[map.Width, map.Height];

        for (var i = 0; i < _levelParent.childCount; i++)
        {
            var child = _levelParent.GetChild(i);

            BoxCollider2D collider;
            if (!child.TryGetComponent<BoxCollider2D>(out collider))
                continue;

            TileType type;
            if (child.CompareTag(GroundTag))
                type = TileType.Block;
            else if (child.CompareTag(PlatformTag))
                type = TileType.OneWay;
            else
                continue;

            var colliderSize = collider.size;

            int bottomLeftX;
            int bottomLeftY;
            var bottomLeftWorldPosition = new Vector2(
                child.position.x - colliderSize.x / 2,
                child.position.y - colliderSize.y / 2
            );
            map.GetMapTileAtPoint(bottomLeftWorldPosition, out bottomLeftX, out bottomLeftY);
            
            int topRightX;
            int topRightY;
            var topRightWorldPosition = new Vector2(
                child.position.x + colliderSize.x / 2,
                child.position.y + colliderSize.y / 2
            );
            map.GetMapTileAtPoint(topRightWorldPosition, out topRightX, out topRightY);

            FillTiles(map, new Vector2Int(bottomLeftX, bottomLeftY), new Vector2Int(topRightX, topRightY), type);
        }
    }

    private void FillTiles(ReimplementedMap map, Vector2Int bottomLeft, Vector2Int topRight, TileType type)
    {
        for (var x = bottomLeft.x; x < topRight.x; x++)
        {
            for (var y = bottomLeft.y; y < topRight.y; y++) 
            {
                if (x < map.Tiles.GetLength(0) && x > 0 && y < map.Tiles.GetLength(1) && y > 0)
                    map.Tiles[x, y] = type;
                else
                    Debug.Log("FillTiles: Tile out of range!");
            }
        }
    }

}
