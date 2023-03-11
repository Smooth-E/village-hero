using UnityEngine;
using Algorithms;

[System.Serializable]
public partial class ReimplementedMap : MonoBehaviour 
{

	public Vector3 BottomLeftCorner { private set; get; }
	
	private TileType[,] _tiles;

	public const int cTileSize = 16;
	
	public int Width = 50;

	public int Height = 42;

    private bool IsTileOutsideOfGrid(int x, int y) =>
        x < 0 || x >= Width || y < 0 || y >= Height;

	public TileType GetTile(int x, int y) =>
        IsTileOutsideOfGrid(x, y) ? TileType.Block : _tiles[x, y];

    public bool IsTileOneWay(int x, int y) =>
        IsTileOutsideOfGrid(x, y) ? false : _tiles[x, y] == TileType.OneWay;

    public bool IsTileBlock(int x, int y) =>
        IsTileOutsideOfGrid(x, y) ? false : _tiles[x, y] == TileType.Block;

    public bool IsTileNotEmpty(int x, int y) =>
        IsTileOutsideOfGrid(x, y) ? false : _tiles[x, y] != TileType.Empty;
	
	public void GetMapTileAtPoint(Vector2 point, out int tileIndexX, out int tileIndexY)
	{
		tileIndexY = (int) ((point.y - BottomLeftCorner.y + cTileSize/ 2f) / cTileSize);
		tileIndexX = (int) ((point.x - BottomLeftCorner.x + cTileSize/ 2f) / cTileSize);
	}
	
	public Vector2Int GetMapTileAtPoint(Vector2 point) =>
	    new Vector2Int(
            (int) ((point.x - BottomLeftCorner.x + cTileSize / 2f) / cTileSize),
            (int) ((point.y - BottomLeftCorner.y + cTileSize / 2f) / cTileSize)
        );
	
	public Vector2 GetMapTilePosition(int tileIndexX, int tileIndexY) =>
        new Vector2(
				(float) (tileIndexX * cTileSize) + BottomLeftCorner.x,
				(float) (tileIndexY * cTileSize) + BottomLeftCorner.y
			);

	public Vector2 GetMapTilePosition(Vector2Int tileCoords) =>
        new Vector2(
			(float) (tileCoords.x * cTileSize) + BottomLeftCorner.x,
			(float) (tileCoords.y * cTileSize) + BottomLeftCorner.y
			);
	
	public bool CollidesWithMapTile(AxisAlignedBoundedBox box, int tileIndexX, int tileIndexY)
	{
		var tilePosition = GetMapTilePosition(tileIndexX, tileIndexY);
		return box.Overlaps(tilePosition, new Vector2(cTileSize / 2f, cTileSize / 2f));
	}

    public bool AnySolidBlockInRectangle(Vector2 start, Vector2 end) =>
        AnySolidBlockInRectangle(GetMapTileAtPoint(start), GetMapTileAtPoint(end));

    public bool AnySolidBlockInStripe(int x, int y0, int y1)
    {
        int startY;
        int endY;

        if (y0 <= y1)
        {
            startY = y0;
            endY = y1;
        }
        else
        {
            startY = y1;
            endY = y0;
        }

        for (int y = startY; y <= endY; ++y)
        {
            if (GetTile(x, y) == TileType.Block)
                return true;
        }

        return false;
    }

    public bool AnySolidBlockInRectangle(Vector2Int start, Vector2Int end)
    {
        int startX;
        int startY;
        int endX;
        int endY;

        if (start.x <= end.x)
        {
            startX = start.x;
            endX = end.x;
        }
        else
        {
            startX = end.x;
            endX = start.x;
        }

        if (start.y <= end.y)
        {
            startY = start.y;
            endY = end.y;
        }
        else
        {
            startY = end.y;
            endY = start.y;
        }

        for (int y = startY; y <= endY; ++y)
        {
            for (int x = startX; x <= endX; ++x)
            {
                if (GetTile(x, y) == TileType.Block)
                    return true;
            }
        }

        return false;
    }

    public byte[,] CreateByteGrid()
    {
        var width = _tiles.GetLength(0);
        var height = _tiles.GetLength(1);
        var grid = new byte[width, height];

        for (var x = 0; x < width; x++)
        {
            for (var y = 0; y <  height; y++)
                grid[x, y] = (byte) _tiles[x, y];
        }

        return grid;
    }

}
