using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class LevelGenerator : MonoBehaviour {

    public int width, height;

    public int padding = 5;
    public float sizeMultiplier = 10;

    [Range(0, 100)]
    public int fillAmount;
    [Range(0, 10)]
    public int smoothAmount;

    [Range(0,8)]
    public int wallsNeededForNewCell = 4;
    [Range(0, 8)]
    public int wallsNeededForDeadCell = 4;

    [Range(2, 6)]
    public int smoothGridSize = 3;

    public int minIslandTileSize = 50;
    public int maxConnections = 2;

    public string seed;
    public bool useRandomSeed;

    public bool populateLevel = true;
    public bool generateGrid = false;
    public bool constantGeneration = false;
    public float timeBetweenGeneration = .5f;

    LevelMeshData levelMeshData;
    float startTime;


    int[,] level;
    int attempts;

    int realSmoothAmount;
    int realFillAmount;

    void Start()
    {
        
        StartCoroutine(GenLevel());
        realFillAmount = fillAmount;
        realSmoothAmount = smoothAmount;
    }

    IEnumerator GenLevel()
    {
        while (true)
        {
            yield return new WaitUntil(() => { return constantGeneration; });
            attempts = 0;
            GenerateLevel();
			yield return new WaitForSeconds(timeBetweenGeneration); 
        }
    }

    public bool GenerateLevel()
    {
        var watch = System.Diagnostics.Stopwatch.StartNew();
        level = new int[width, height];
        RandomFillLevel();
        for (int i = 0; i < realSmoothAmount; i++)
        {
            SmoothLevel();
        }

        ProcessLevel();

        int borderSize = 1;
        int[,] borderedLevel = new int[width + borderSize * 2, height + borderSize * 2];

        for (int x = 0; x < borderedLevel.GetLength(0); x++)
        {
            for (int y = 0; y < borderedLevel.GetLength(1); y++)
            {
                if(x >= borderSize && x < width + borderSize && y >= borderSize && y < height + borderSize)
                {
                    borderedLevel[x, y] = level[x - borderSize, y - borderSize];
                }
                else
                {
                    borderedLevel[x, y] = 0;
                }
            }
        }

        LevelMeshGenerator meshGen = GetComponent<LevelMeshGenerator>();
        levelMeshData = meshGen.GenerateMesh(borderedLevel, sizeMultiplier);

        LevelPopulation popGen = GetComponent<LevelPopulation>();
        popGen.DepopulateLevel();
        if (populateLevel)
        {
            if(!popGen.PopulateLevel(borderedLevel, sizeMultiplier, levelMeshData))
            {
                attempts++;
				if (attempts > 10) {
					return false;
				}
				return GenerateLevel();
            }
        }
        realFillAmount = fillAmount;
        realSmoothAmount = smoothAmount;

        if (generateGrid)
        {
            Grid pathfindingGrid = GetComponent<Grid>();
            pathfindingGrid.CreateGrid();
        }
        watch.Stop();
        Debug.Log("Map Generated In " + (watch.ElapsedMilliseconds) +" Seconds");
        return true;
    }

    private void SmoothLevel()
    {
        int[,] tempLevel = new int[width, height];
        for (int x = padding; x < width - padding; x++)
        {
            for (int y = padding; y < height - padding; y++)
            {
                int neighbourWallAmount = GetNeighbourWallAmount(x, y);
                if(neighbourWallAmount >= wallsNeededForDeadCell)
                {
                    tempLevel[x, y] = 1;
                }else if(neighbourWallAmount <= wallsNeededForNewCell)
                {
                    tempLevel[x, y] = 0;
                }
            }
        }
        level = tempLevel;
    }

    int GetNeighbourWallAmount(int gridX, int gridY)
    {
        int wallAmount = 0;
        int size = Mathf.FloorToInt((smoothGridSize / 2) + 0.5f);
        for(int nX = gridX-size; nX <= gridX+size; nX++)
        {
            for (int nY = gridY - size; nY <= gridY + size; nY++)
            {
                if (IsInLevel(nX, nY))
                {
                    if (nX != gridX || nY != gridY)
                        wallAmount += level[nX, nY];
                }else
                {
                    wallAmount++;
                }
            }
        }
        return wallAmount;
    }

    void ProcessLevel()
    {
        List<List<Coord>> islandRegions = GetRegions(1);
        List<List<Coord>> waterRegions = GetRegions(0);

        for (int i = 0; i < waterRegions.Count; i++)
        {
            if (waterRegions[i].Count < 10)
            {
                for (int j = 0; j < waterRegions[i].Count; j++)//(Coord tile in waterRegions[i])
                {
                    Coord tile = waterRegions[i][j];
                    level[tile.tileX, tile.tileY] = 1;
                }
            }
        }

        List<Island> survivingIslands = new List<Island>();
        for (int i = 0; i < islandRegions.Count; i++)
        {
            if (islandRegions[i].Count < minIslandTileSize)
            {
                for (int j = 0; j < islandRegions[i].Count; j++)//(Coord tile in waterRegions[i])
                {
                    Coord tile = islandRegions[i][j];
                    level[tile.tileX, tile.tileY] = 0;
                }
            }else
            {
                survivingIslands.Add(new Island(islandRegions[i], level));
            }
        }


        if (survivingIslands.Count > 0)
        {
            survivingIslands.Sort();
            survivingIslands[0].isMainIsland = true;
            survivingIslands[0].isAccessibleFromMainIsland = true;
            if (survivingIslands.Count > 1)
            {
                ConnectClosestIslands(survivingIslands);
            }
        }

    }



    void ConnectClosestIslands(List<Island> islands, bool forceAccessibilityToTheMainIsland = false)
    {

        List<Island> islandListA = new List<Island>();
        List<Island> islandListB = new List<Island>();

        if (forceAccessibilityToTheMainIsland)
        {
            for (int i = 0; i < islands.Count; i++) //foreach (Island island in islands)
            {
                Island island = islands[i];
                if (island.isAccessibleFromMainIsland) {
                    islandListB.Add(island);
                }
                else
                {
                    islandListA.Add(island);
                }
            }
        }else
        {
            islandListA = islands;
            islandListB = islands;
        }

        int bestDistance = 0;

        Coord bestTileA = new Coord();
        Coord bestTileB = new Coord();
        Island bestIslandA = new Island();
        Island bestIslandB = new Island();

        bool posConnectionFound = false;

        for (int i = 0; i < islandListA.Count; i++) //(Island islandA in islandListA)
        {
            Island islandA = islandListA[i];
            if (!forceAccessibilityToTheMainIsland)
            {
                posConnectionFound = false;
                if(islandA.connectedIslands.Count > 0) {
                    continue;
                }
            }
            for (int j = 0; j < islandListB.Count; j++)
            {
                Island islandB = islandListB[j];
                if (islandB == islandA || islandA.IsConnected(islandB))
                    continue;

                for(int tileIndexA = 0; tileIndexA < islandA.edgeTiles.Count; tileIndexA++)
                {
                    for (int tileIndexB = 0; tileIndexB < islandB.edgeTiles.Count; tileIndexB++)
                    {
                        Coord tileA = islandA.edgeTiles[tileIndexA];
                        Coord tileB = islandB.edgeTiles[tileIndexB];

                        int distanceBetweenIslands = (int)(Mathf.Pow(tileA.tileX - tileB.tileX, 2) + Mathf.Pow(tileA.tileY - tileB.tileY, 2));

                        if (distanceBetweenIslands < bestDistance || !posConnectionFound)
                        {
                            bestDistance = distanceBetweenIslands;
                            posConnectionFound = true;
                            bestTileA = tileA;
                            bestTileB = tileB;
                            bestIslandA = islandA;
                            bestIslandB = islandB;
                        }

                    }
                }
            }

            if (posConnectionFound && !forceAccessibilityToTheMainIsland)
            {
                CreatePassage(bestIslandA, bestIslandB, bestTileA, bestTileB);
            }
        }

        if (posConnectionFound && forceAccessibilityToTheMainIsland)
        {
            CreatePassage(bestIslandA, bestIslandB, bestTileA, bestTileB);
            ConnectClosestIslands(islands, true);
        }

        if (!forceAccessibilityToTheMainIsland)
        {
            ConnectClosestIslands(islands, true);
        }
    }

    void CreatePassage(Island islandA, Island islandB, Coord tileA, Coord tileB)
    {
        Island.ConnectIslands(islandA, islandB);
        Debug.DrawLine(CoordToWorldPoint(tileA), CoordToWorldPoint(tileB),Color.red);

        List<Coord> line = GetPassageLine(tileA, tileB);
        foreach(Coord c in line)
        {
            DrawCircle(c, UnityEngine.Random.Range(3,5));
        }

    }

    void DrawCircle(Coord c, int r)
    {
        for (int x = -r; x <= r; x++)
        {
            for (int y = -r; y <= r; y++)
            {
                if(x*x + y*y <= r * r)
                {
                    int realX = c.tileX + x;
                    int realY = c.tileY + y;
                    if(IsInLevel(realX, realY))
                    {
                        level[realX, realY] = 1;
                    }
                }
            }
        }
    }

    List<Coord> GetPassageLine(Coord from, Coord to)
    {
        List<Coord> line = new List<Coord>();

        int x = from.tileX;
        int y = from.tileY;

        int dx = to.tileX - from.tileX;
        int dy = to.tileY - from.tileY;

        bool inverted = false;
        int step = Math.Sign(dx);
        int gradientStep = Math.Sign(dy);

        int longest = Mathf.Abs(dx);
        int shortest = Mathf.Abs(dy);

        if(longest < shortest)
        {
            inverted = true;
            longest = Mathf.Abs(dy);
            shortest = Mathf.Abs(dx);

            step = Math.Sign(dy);
            gradientStep = Math.Sign(dx);
        }

        int gradientAcc = longest / 2;
        for (int i = 0; i < longest; i++)
        {
            line.Add(new Coord(x, y));
            if (inverted)
            {
                y += step;
            }else
            {
                x += step;
            }

            gradientAcc += shortest;
            if (gradientAcc >= longest)
            {
                if (inverted)
                {
                    x += gradientStep;
                }
                else
                {
                    y += gradientStep;
                }
                gradientAcc -= longest;
            }
        }

        return line;

    }

    Vector3 CoordToWorldPoint(Coord tile)
    {
        return new Vector3(-width / 2 + .5f + tile.tileX, 2, -height / 2 + .5f + tile.tileY)*sizeMultiplier;
    }

    List<List<Coord>> GetRegions(int tileType)
    {
        List<List<Coord>> regions = new List<List<Coord>>();
        int[,] mapflags = new int[width, height];
        for (int x = padding; x < width - padding; x++)
        {
            for (int y = padding; y < height - padding; y++)
            {
                if(mapflags[x,y] == 0 && level[x,y] == tileType)
                {
                    List<Coord> newRegion = GetRegionTiles(x, y);
                    regions.Add(newRegion);
                    foreach(Coord tile in newRegion)
                    {
                        mapflags[tile.tileX, tile.tileY] = 1;
                    }
                }
            }
        }

        return regions;

    }

    List<Coord> GetRegionTiles(int startX, int startY)
    {
        List<Coord> tiles = new List<Coord>();
        int[,] mapflags = new int[width, height];
        int tileType = level[startX, startY];

        Queue<Coord> queue = new Queue<Coord>();
        queue.Enqueue(new Coord(startX, startY));
        mapflags[startX, startY] = 1;
        while(queue.Count > 0)
        {
            Coord tile = queue.Dequeue();
            tiles.Add(tile);
            for(int x = tile.tileX-1; x <= tile.tileX+1; x++)
            {
                for (int y = tile.tileY - 1; y <= tile.tileY + 1; y++)
                {
                    if (IsInLevel(x, y) && (y == tile.tileY || x == tile.tileX))
                    {
                        if(mapflags[x,y] == 0 && level[x,y] == tileType)
                        {
                            mapflags[x, y] = 1;
                            queue.Enqueue(new Coord(x, y));
                        }
                    }
                }
            }
        }
        return tiles;
    }

    bool IsInLevel(int x, int y)
    {
        return (x >= 0 && x < width && y >= 0 && y < height);
    }

    void RandomFillLevel()
    {
        if (useRandomSeed)
            seed = DateTime.Now.Ticks.ToString();

        System.Random randGen = new System.Random(seed.GetHashCode());

        for (int x = padding; x < width-padding; x++)
        {
            for (int y = padding; y < height-padding; y++)
            {
                level[x, y] = (randGen.Next(0, 100) < realFillAmount) ? 1 : 0;
            }
        }
    }

    struct Coord
    {
        public int tileX;
        public int tileY;

        public Coord(int x, int y)
        {
            tileX = x;
            tileY = y;
        }
    }

    class Island : IComparable<Island>
    {
        public List<Coord> tiles;
        public List<Coord> edgeTiles;

        public List<Island> connectedIslands;
        public int islandSize;
        public bool isAccessibleFromMainIsland;
        public bool isMainIsland;

        public Island()
        {

        }

        public Island(List<Coord> islandTiles, int[,] level)
        {
            tiles = islandTiles;
            islandSize = tiles.Count;
            connectedIslands = new List<Island>();
            edgeTiles = new List<Coord>();
            foreach (Coord tile in tiles)
            {
                for (int x = tile.tileX - 1; x <= tile.tileX + 1; x++)
                {
                    for (int y = tile.tileY - 1; y <= tile.tileY + 1; y++)
                    {
                        if (level[x, y] == 0)
                        {
                            edgeTiles.Add(tile);
                        }
                    }
                }
            }
        }

        public void forceAccessibilityToTheMainIsland()
        {
            if (!isAccessibleFromMainIsland)
            {
                isAccessibleFromMainIsland = true;
                foreach(Island island in connectedIslands)
                {
                    island.forceAccessibilityToTheMainIsland();
                }
            }
        }

        public static void ConnectIslands(Island a, Island b)
        {
            if (a.isAccessibleFromMainIsland)
            {
                b.forceAccessibilityToTheMainIsland();
            }else if (b.isAccessibleFromMainIsland)
            {
                a.forceAccessibilityToTheMainIsland();
            }
            a.connectedIslands.Add(b);
            b.connectedIslands.Add(a);
        }

        public bool IsConnected(Island other)
        {
            return connectedIslands.Contains(other);
        }

        public int CompareTo(Island other)
        {
            return other.islandSize.CompareTo(islandSize);
        }
    }

    private void OnDrawGizmos()
    {
        /*
        if(level != null)
        {
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    Gizmos.color = (level[x, y] == 1) ? Color.black : Color.white;
                    Vector3 pos = new Vector3(-width/2 + x + .5f, 0, -height/2+y+.5f);
                    Gizmos.DrawCube(pos, Vector3.one);
                }
            }
        }*/
    }

}
