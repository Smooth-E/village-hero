using UnityEngine;
using System.Collections.Generic;
using Algorithms;

[System.Serializable]
public partial class Map : MonoBehaviour 
{
	
	/// <summary>
	/// The map's position in world space. Bottom left corner.
	/// </summary>
	public Vector3 BottomLeftCorner;
	
	/// <summary>
	/// The base tile sprite prefab that populates the map.
	/// Assigned in the inspector.
	/// </summary>
	public SpriteRenderer tilePrefab;
	
	/// <summary>
	/// The path finder.
	/// </summary>
	public PathFinderFast mPathFinder;
	
	/// <summary>
	/// The nodes that are fed to pathfinder.
	/// </summary>
	[HideInInspector] public byte[,] mGrid;
	
	/// <summary>
	/// The map's tile data.
	/// </summary>
	[HideInInspector] private TileType[,] tiles;

	/// <summary>
	/// The map's sprites.
	/// </summary>
	private SpriteRenderer[,] tilesSprites;
	
	/// <summary>
	/// A parent for all the sprites. Assigned from the inspector.
	/// </summary>
	public Transform mSpritesContainer;
	
	/// <summary>
	/// The size of a tile in pixels.
	/// </summary>
	static public int cTileSize = 16;
	
	/// <summary>
	/// The width of the map in tiles.
	/// </summary>
	public int Width = 50;

	/// <summary>
	/// The height of the map in tiles.
	/// </summary>
	public int Height = 42;

    public MapRoomData mapRoomSimple;
    public MapRoomData mapRoomOneWay;

    public Camera gameCamera;
    public Bot player;
    bool[] inputs;
    bool[] prevInputs;

    int lastMouseTileX = -1;
    int lastMouseTileY = -1;

    public KeyCode goLeftKey = KeyCode.A;
    public KeyCode goRightKey = KeyCode.D;
    public KeyCode goJumpKey = KeyCode.W;
    public KeyCode goDownKey = KeyCode.S;

    public RectTransform sliderHigh;
    public RectTransform sliderLow;

    private bool IsTileOutsideOfGrid(int x, int y) =>
        x < 0 || x >= Width || y < 0 || y >= Height;

	public TileType GetTile(int x, int y) =>
        IsTileOutsideOfGrid(x, y) ? TileType.Block : tiles[x, y];

    public bool IsTileOneWay(int x, int y) =>
        IsTileOutsideOfGrid(x, y) ? false : tiles[x, y] == TileType.OneWay;

    public bool IsTileBlock(int x, int y) =>
        IsTileOutsideOfGrid(x, y) ? false : tiles[x, y] == TileType.Block;

    public bool IsTileNotEmpty(int x, int y) =>
        IsTileOutsideOfGrid(x, y) ? false : tiles[x, y] != TileType.Empty;

	public void InitPathFinder()
	{
		mPathFinder = new PathFinderFast(mGrid, this);
		
		mPathFinder.Formula                 = HeuristicFormula.Manhattan;
		// If false then diagonal movement will be prohibited
        mPathFinder.Diagonals               = false;
		// If true then diagonal movement will have higher cost
        mPathFinder.HeavyDiagonals          = false;
		// Estimate of path length
        mPathFinder.HeuristicEstimate       = 6;
        mPathFinder.PunishChangeDirection   = false;
        mPathFinder.TieBreaker              = false;
        mPathFinder.SearchLimit             = 1000000;
        mPathFinder.DebugProgress           = false;
        mPathFinder.DebugFoundPath          = false;
	}
	
	public void GetMapTileAtPoint(Vector2 point, out int tileIndexX, out int tileIndexY)
	{
		tileIndexY =(int)((point.y - BottomLeftCorner.y + cTileSize/2.0f)/(float)(cTileSize));
		tileIndexX =(int)((point.x - BottomLeftCorner.x + cTileSize/2.0f)/(float)(cTileSize));
	}
	
	public Vector2Int GetMapTileAtPoint(Vector2 point) =>
	    new Vector2Int(
            (int) ((point.x - BottomLeftCorner.x + cTileSize/ 2f) / (float) cTileSize),
            (int) ((point.y - BottomLeftCorner.y + cTileSize/ 2f) / (float) cTileSize)
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
	
	public bool CollidesWithMapTile(AxisAlignedBoundedBox aabb, int tileIndexX, int tileIndexY)
	{
		var tilePosition = GetMapTilePosition(tileIndexX, tileIndexY);
		return aabb.Overlaps(tilePosition, new Vector2(cTileSize / 2f, cTileSize / 2f));
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

    public void SetTile(int x, int y, TileType type)
    {
        if (x <= 1 || x >= Width - 2 || y <= 1 || y >= Height - 2)
            return;

        tiles[x, y] = type;

        if (type == TileType.Block)
        {
            mGrid[x, y] = 0;
            AutoTile(type, x, y, 1, 8, 4, 4, 4, 4);
            tilesSprites[x, y].enabled = true;
        }
        else if (type == TileType.OneWay)
        {
            mGrid[x, y] = 1;
            tilesSprites[x, y].enabled = true;

            tilesSprites[x, y].transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
            tilesSprites[x, y].transform.eulerAngles = new Vector3(0.0f, 0.0f, 0.0f);
            tilesSprites[x, y].sprite = mDirtSprites[25];
        }
        else
        {
            mGrid[x, y] = 1;
            tilesSprites[x, y].enabled = false;
        }

        AutoTile(type, x - 1, y, 1, 8, 4, 4, 4, 4);
        AutoTile(type, x + 1, y, 1, 8, 4, 4, 4, 4);
        AutoTile(type, x, y - 1, 1, 8, 4, 4, 4, 4);
        AutoTile(type, x, y + 1, 1, 8, 4, 4, 4, 4);
    }

    public void Start()
    {
        var mapRoom = mapRoomOneWay;
        mRandomNumber = new System.Random();
        
        inputs = new bool[(int)KeyInput.Count];
        prevInputs = new bool[(int)KeyInput.Count];

        //set the position
        BottomLeftCorner = transform.position;

        Width = mapRoom.width;
        Height = mapRoom.height;

        tiles = new TileType[Width, Height];
        tilesSprites = new SpriteRenderer[mapRoom.width, mapRoom.height];

        mGrid = new byte[Mathf.NextPowerOfTwo(Width), Mathf.NextPowerOfTwo(Height)];
        InitPathFinder();

        Camera.main.orthographicSize = Camera.main.pixelHeight / 2;

        for (int y = 0; y < Height; ++y)
        {
            for (int x = 0; x < Width; ++x)
            {
                tilesSprites[x, y] = Instantiate<SpriteRenderer>(tilePrefab);
                tilesSprites[x, y].transform.parent = transform;
                tilesSprites[x, y].transform.position = BottomLeftCorner + new Vector3(cTileSize * x, cTileSize * y, 10.0f);

                if (mapRoom.tileData[y * Width + x] == TileType.Empty)
                    SetTile(x, y, TileType.Empty);
                else if (mapRoom.tileData[y * Width + x] == TileType.Block)
                    SetTile(x, y, TileType.Block);
                else
                    SetTile(x, y, TileType.OneWay);
            }
        }

        for (int y = 0; y < Height; ++y)
        {
            tiles[1, y] = TileType.Block;
            tiles[Width - 2, y] = TileType.Block;
        }

        for (int x = 0; x < Width; ++x)
        {
            tiles[x, 1] = TileType.Block;
            tiles[x, Height - 2] = TileType.Block;
        }

        player.BotInit(inputs, prevInputs);
        player.mMap = this;
        player.mPosition = new Vector2(2 * Map.cTileSize, (Height / 2) * Map.cTileSize + player.mAABB.HalfSizeY);
    }

    void Update()
    {
        inputs[(int)KeyInput.Right] = Input.GetKey(goRightKey);
        inputs[(int)KeyInput.Left] = Input.GetKey(goLeftKey);
        inputs[(int)KeyInput.GoDown] = Input.GetKey(goDownKey);
        inputs[(int)KeyInput.Jump] = Input.GetKey(goJumpKey);

        if (Input.GetKeyUp(KeyCode.Mouse0))
            lastMouseTileX = lastMouseTileY = -1;

        Vector2 mousePos = Input.mousePosition;
        Vector2 cameraPos = Camera.main.transform.position;
        var mousePosInWorld = cameraPos + mousePos - new Vector2(gameCamera.pixelWidth / 2, gameCamera.pixelHeight / 2);

        int mouseTileX, mouseTileY;
        GetMapTileAtPoint(mousePosInWorld, out mouseTileX, out mouseTileY);

        Vector2 offsetMouse = (Vector2)(Input.mousePosition) - new Vector2(Camera.main.pixelWidth/2, Camera.main.pixelHeight/2);
        Vector2 bottomLeft = (Vector2)sliderLow.position + sliderLow.rect.min;
        Vector2 topRight = (Vector2)sliderHigh.position + sliderHigh.rect.max;

        if (Input.GetKeyDown(KeyCode.Tab))
            Debug.Break();

        //Debug.Log(mousePos + "   " + bottomLeft + "     " + topRight);

        if (mousePos.x > bottomLeft.x && mousePos.x < topRight.x && mousePos.y < topRight.y && mousePos.y > bottomLeft.y)
            return;

        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            player.TappedOnTile(new Vector2Int(mouseTileX, mouseTileY));
            Debug.Log(mouseTileX + "  " + mouseTileY);
        }

        if (Input.GetKey(KeyCode.Mouse1) || Input.GetKey(KeyCode.Mouse2))
        {
            if (mouseTileX != lastMouseTileX || mouseTileY != lastMouseTileY || Input.GetKeyDown(KeyCode.Mouse1) || Input.GetKeyDown(KeyCode.Mouse2))
            {
                if (!IsTileNotEmpty(mouseTileX, mouseTileY))
                    SetTile(mouseTileX, mouseTileY, Input.GetKey(KeyCode.Mouse1) ? TileType.Block : TileType.OneWay);
                else
                    SetTile(mouseTileX, mouseTileY, TileType.Empty);

                lastMouseTileX = mouseTileX;
                lastMouseTileY = mouseTileY;

                Debug.Log(mouseTileX + "  " + mouseTileY);
            }
        }
    }

    System.Random mRandomNumber;

    void AutoTile(TileType type, int x, int y, int rand4NeighbourTiles, int rand3NeighbourTiles,
        int rand2NeighbourPipeTiles, int rand2NeighbourCornerTiles, int rand1NeighbourTiles, int rand0NeighbourTiles)
    {
        if (x >= Width || x < 0 || y >= Height || y < 0)
            return;

        if (tiles[x, y] != TileType.Block)
            return;

        int tileOnLeft = tiles[x - 1, y] == tiles[x, y] ? 1 : 0;
        int tileOnRight = tiles[x + 1, y] == tiles[x, y] ? 1 : 0;
        int tileOnTop = tiles[x, y + 1] == tiles[x, y] ? 1 : 0;
        int tileOnBottom = tiles[x, y - 1] == tiles[x, y] ? 1 : 0;

        float scaleX = 1.0f;
        float scaleY = 1.0f;
        float rot = 0.0f;
        int id = 0;

        int sum = tileOnLeft + tileOnRight + tileOnTop + tileOnBottom;

        switch (sum)
        {
            case 0:
                id = 1 + mRandomNumber.Next(rand0NeighbourTiles);

                break;
            case 1:
                id = 1 + rand0NeighbourTiles + mRandomNumber.Next(rand1NeighbourTiles);

                if (tileOnRight == 1)
                    scaleX = -1;
                else if (tileOnTop == 1)
                    rot = -1;
                else if (tileOnBottom == 1)
                {
                    rot = 1;
                    scaleY = -1;
                }

                break;
            case 2:

                if (tileOnLeft + tileOnBottom == 2)
                {
                    id = 1 + rand0NeighbourTiles + rand1NeighbourTiles + rand2NeighbourPipeTiles
                        + mRandomNumber.Next(rand2NeighbourCornerTiles);
                }
                else if (tileOnRight + tileOnBottom == 2)
                {
                    id = 1 + rand0NeighbourTiles + rand1NeighbourTiles + rand2NeighbourPipeTiles
                        + mRandomNumber.Next(rand2NeighbourCornerTiles);
                    scaleX = -1;
                }
                else if (tileOnTop + tileOnLeft == 2)
                {
                    id = 1 + rand0NeighbourTiles + rand1NeighbourTiles + rand2NeighbourPipeTiles
                        + mRandomNumber.Next(rand2NeighbourCornerTiles);
                    scaleY = -1;
                }
                else if (tileOnTop + tileOnRight == 2)
                {
                    id = 1 + rand0NeighbourTiles + rand1NeighbourTiles + rand2NeighbourPipeTiles
                        + mRandomNumber.Next(rand2NeighbourCornerTiles);
                    scaleX = -1;
                    scaleY = -1;
                }
                else if (tileOnTop + tileOnBottom == 2)
                {
                    id = 1 + rand0NeighbourTiles + rand1NeighbourTiles + mRandomNumber.Next(rand2NeighbourPipeTiles);
                    rot = 1;
                }
                else if (tileOnRight + tileOnLeft == 2)
                    id = 1 + rand0NeighbourTiles + rand1NeighbourTiles + mRandomNumber.Next(rand2NeighbourPipeTiles);

                break;
            case 3:
                id = 1 + rand0NeighbourTiles + rand1NeighbourTiles + rand2NeighbourPipeTiles
                    + rand2NeighbourCornerTiles + mRandomNumber.Next(rand3NeighbourTiles);

                if (tileOnLeft == 0)
                {
                    rot = 1;
                    scaleX = -1;
                }
                else if (tileOnRight == 0)
                {
                    rot = 1;
                    scaleY = -1;
                }
                else if (tileOnBottom == 0)
                    scaleY = -1;

                break;

            case 4:
                id = 1 + rand0NeighbourTiles + rand1NeighbourTiles + rand2NeighbourPipeTiles
                    + rand2NeighbourCornerTiles + rand3NeighbourTiles + mRandomNumber.Next(rand4NeighbourTiles);

                break;
        }

        tilesSprites[x, y].transform.localScale = new Vector3(scaleX, scaleY, 1.0f);
        tilesSprites[x, y].transform.eulerAngles = new Vector3(0.0f, 0.0f, rot * 90.0f);
        tilesSprites[x, y].sprite = mDirtSprites[id - 1];
    }

    public List<Sprite> mDirtSprites;

    void FixedUpdate()
    {
        player.BotUpdate();
    }
}
