using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LevelPopulation : MonoBehaviour {

    public enum PrefabSize : int
    {
        Small = 1,
        Medium = 3,
        Large = 5
    }

    [System.Serializable]
    public class PrefabData
    {
        public GameObject prefab;
        public PrefabSize prefabSize = PrefabSize.Small;
        public float prefabPlacementRandomness = 2;
        public bool RandomRotationOnY = true;

        [Range(0,1)]
        public float levelDensity = 0.2f;
    }

    [System.Serializable]
    public class EssentialPrefabData
    {
        public GameObject prefab;
        public PrefabSize prefabSize = PrefabSize.Small;
        public float prefabPlacementRandomness = 2;
        public bool RandomRotationOnY = true;
    }


    public class Fence
    {
        public Vector3 position;
        public List<Fence> connectedFences = new List<Fence>();
    }

    public struct Coord
    {
        public int x;
        public int y;

        public Coord(int dx, int dy)
        {
            x = dx;
            y = dy;
        }

        public static bool operator == (Coord a, Coord b)
        {
            return (a.x == b.x && a.y == b.y);
        }

        public static bool operator != (Coord a, Coord b)
        {
            return !(a == b);
        }
    }

    public Transform levelObjectHolder;
    public PrefabData[] levelPrefabs;
    public EssentialPrefabData[] essentialLevelPrefabs;

    public int requiredSmallPasses = 0;
    [Range(0,1000)]
    public int smallPasses = 200;

    public int requiredMediumPasses = 0;
    [Range(0, 1000)]
    public int mediumPasses = 80;

    public int requiredBigPasses = 0;
    [Range(0, 1000)]
    public int bigPasses = 30;

    private int[,] levelPrefabMap;
    private List<Coord> availibleLevel = new List<Coord>();

    private List<Coord> bigSpots = new List<Coord>();
    private List<Coord> mediumSpots = new List<Coord>();
    private List<Coord> smallSpots = new List<Coord>();

    //private List<Coord> edgeSpots = new List<Coord>();

    private List<Fence> fences = new List<Fence>();

    private List<List<Fence>> fenceOutlines;

    private float sizeMultipler;
    private int levelWidth;
    private int levelHeight;

    LevelMeshData levelMeshData;

    public bool PopulateLevel(int[,] level, float size, LevelMeshData data)
    {
        levelPrefabMap = level;

        levelWidth = levelPrefabMap.GetLength(0);
        levelHeight = levelPrefabMap.GetLength(1);

        sizeMultipler = size;
        levelMeshData = data;


        availibleLevel.Clear();
        bigSpots.Clear();
        mediumSpots.Clear();
        smallSpots.Clear();
        fences.Clear();

        if (!FindSpots())
        {
            return false;
        }
        PlaceFences();

        PopulateSpot(smallSpots, PrefabSize.Small);
        PopulateSpot(mediumSpots, PrefabSize.Medium);
        PopulateSpot(bigSpots, PrefabSize.Large);

        return true;
    }


    public void PopulateSpot(List<Coord> spots, PrefabSize size)
    {
        List<PrefabData> prefabsAvaliable = new List<PrefabData>();
        List<EssentialPrefabData> essentials = new List<EssentialPrefabData>();
        float combinedDensity = 0;

        foreach (EssentialPrefabData prefab in essentialLevelPrefabs)
        {
            if (prefab.prefabSize == size && spots.Count > 0)
            {
                int randomChoice = Random.Range(0, spots.Count-1);
                Vector3 position = CoordToWorldSpace(spots[randomChoice]) + new Vector3(Random.Range(-prefab.prefabPlacementRandomness / 2, prefab.prefabPlacementRandomness / 2), 0, Random.Range(-prefab.prefabPlacementRandomness / 2, prefab.prefabPlacementRandomness / 2));
                Quaternion rotation = prefab.RandomRotationOnY ? Quaternion.Euler(0, Random.Range(0, 360), 0) : Quaternion.identity;
                Instantiate(prefab.prefab, position, rotation, levelObjectHolder);
                spots.RemoveAt(randomChoice);
            }
        }

        foreach (PrefabData prefab in levelPrefabs)
        {
            if(prefab.prefabSize == size)
            {
                prefabsAvaliable.Add(prefab);
                combinedDensity += prefab.levelDensity;
            }
        }
        if(prefabsAvaliable.Count > 0)
        {
            foreach(Coord spot in spots)
            {
                float randomChoice = Random.Range(0, combinedDensity);
                PrefabData prefabChoice = null;
                float currentDensity = 0;
                foreach(PrefabData prefab in prefabsAvaliable)
                {
                    if(randomChoice <= currentDensity + prefab.levelDensity)
                    {
                        prefabChoice = prefab;
                        break;
                    }
                    currentDensity += prefab.levelDensity;
                }
                Vector3 position = CoordToWorldSpace(spot) + new Vector3(Random.Range(-prefabChoice.prefabPlacementRandomness/2, prefabChoice.prefabPlacementRandomness / 2), 0, Random.Range(-prefabChoice.prefabPlacementRandomness / 2, prefabChoice.prefabPlacementRandomness / 2)); ;
                Quaternion rotation = prefabChoice.RandomRotationOnY?Quaternion.Euler(0, Random.Range(0, 360), 0):Quaternion.identity;
                Instantiate(prefabChoice.prefab, position, rotation, levelObjectHolder);

            }
        }
    }

    public void PlaceFences()
    {
        foreach (Vector3 edgeVert in levelMeshData.solidEdgeVertices)
        {
            Vector3 position = edgeVert + Vector3.left * 4 + Vector3.forward * 4;
            Fence currentFence = new Fence();
            currentFence.position = position;
            fences.Add(currentFence);
        }
        ConnectFences();
    }

    public void ConnectFences()
    {
        foreach (Fence rootFence in fences)
        {
            foreach (Fence fence in fences)
            {
                if(rootFence != fence && Vector3.Distance(fence.position, rootFence.position) < sizeMultipler + 1.6f)
                {
                    rootFence.connectedFences.Add(fence);
                }
            }
        }
        fenceOutlines = GetFenceOutlines();
    }

    public List<List<Fence>> GetFenceOutlines()
    {
        List<Fence> avalibleFences = fences;
        List<List<Fence>> outlines = new List<List<Fence>>();

        if (avalibleFences.Count > 0)
        {
            List<Fence> outline = new List<Fence>();
            Fence currentFence = avalibleFences[0];

            while (avalibleFences.Count > 1)
            {
                avalibleFences.Remove(currentFence);
                outline.Add(currentFence);
                if (currentFence.connectedFences.Count > 0)
                {
                    bool fenceFound = false;
                    foreach (Fence connectedFence in currentFence.connectedFences)
                    {
                        if (avalibleFences.Contains(connectedFence))
                        {
                            currentFence = connectedFence;
                            fenceFound = true;
                            break;
                        }
                    }
                    if (!fenceFound)
                    {
                        currentFence = avalibleFences[0];
                        outlines.Add(outline);
                        outline = new List<Fence>();
                    }
                }
                else
                {
                    currentFence = avalibleFences[0];
                    outlines.Add(outline);
                    outline = new List<Fence>();
                }
            }
        }
        return outlines;
        
        
    }

    public bool FindSpots()
    {
        FindAvailableSpaces();
        FindSpots(PrefabSize.Large, bigPasses, ref bigSpots, Color.red);
        FindSpots(PrefabSize.Medium, mediumPasses, ref mediumSpots, Color.yellow);
        FindSpots(PrefabSize.Small, smallPasses, ref smallSpots, Color.green);

        return (bigSpots.Count > requiredBigPasses && mediumSpots.Count > requiredMediumPasses && smallSpots.Count > requiredSmallPasses);

    }

    public void FindSpots(PrefabSize size, int passes, ref List<Coord> spotList, Color lineColour)
    {
        if (availibleLevel.Count > passes)
        {
            for (int i = 0; i < passes; i++)
            {
                int randomIdx = Random.Range(0, availibleLevel.Count - 1);
                Coord selectedCoord = availibleLevel[randomIdx];

                Vector3 v = CoordToWorldSpace(selectedCoord);
                //Debug.DrawLine(v, v + Vector3.up * 10, lineColour, 20);

                if (CheckIfFit(selectedCoord.x, selectedCoord.y, (int)size))
                {
                    spotList.Add(selectedCoord);
                    RemoveSpace(selectedCoord.x, selectedCoord.y, (int)size);
                }

            }
        }
    }

    public bool CheckIfFit(int x, int y, int radius = 1)
    {
        radius -= 1;
        
        for (int dx = x-radius; dx <= x+radius; dx++)
        {
            for (int dy = y - radius; dy <= y + radius; dy++)
            {
                try
                {
                    if ( (levelPrefabMap[dx, dy] == 0) && IsInLevel(dx, dy))
                        return false;
                }
                catch
                {
                    Debug.Log(dx + "," + dy);
                }
            }
        }
        return true;
    }

    public void DepopulateLevel()
    {
        for (int i = 0; i < levelObjectHolder.childCount; i++)
        {
            Destroy(levelObjectHolder.GetChild(i).gameObject);
        }
    }
    bool IsInLevel(int x, int y)
    {
        return ((x >= 0 && x < levelWidth) && (y >= 0 && y < levelHeight));
    }

    public void RemoveSpace(int x, int y, int radius = 1)
    {
        radius -= 1;
        for (int dx = x - radius/2; dx <= x + radius; dx++)
        {
            for (int dy = y - radius/2; dy <= y + radius; dy++)
            {
                Coord coord = new Coord(dx,dy);
                int index = availibleLevel.FindIndex(a => a == coord);
                if (index != -1)
                {
                    availibleLevel.RemoveAt(index);
                }
            }
        }
    }

    private void FindAvailableSpaces()
    {
        availibleLevel.Clear();
        for (int x = 0; x < levelPrefabMap.GetLength(0); x++)
        {
            for (int y = 0; y < levelPrefabMap.GetLength(1); y++)
            {
                if (levelPrefabMap[x,y] == 1 && !checkEdge(x,y))
                {

                    availibleLevel.Add(new Coord(x, y));
                }
            }
        }
    }

    private bool checkEdge(int x, int y)
    {
        for (int dx = x - 1; dx <= x + 1; dx++)
        {
            for (int dy = y - 1; dy <= y + 1; dy++)
            {
                if (levelPrefabMap[dx, dy] == 0)
                {
                    return true;
                }
            }
        }
        return false;
    }

    private Vector3 CoordToWorldSpace(Coord c)
    {
        return (new Vector3(c.x, 0, c.y) - Vector3.right * levelWidth / 2 - Vector3.forward * levelHeight / 2) * sizeMultipler;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        foreach(Coord c in bigSpots)
        {
            Gizmos.DrawCube(CoordToWorldSpace(c) + Vector3.up, Vector3.one * (int)PrefabSize.Large * sizeMultipler);
        }
        Gizmos.color = Color.yellow;
        foreach (Coord c in mediumSpots)
        {
            Gizmos.DrawCube(CoordToWorldSpace(c) + Vector3.up, Vector3.one * (int)PrefabSize.Medium * sizeMultipler);
        }
        /*Gizmos.color = Color.green;
        foreach (Coord c in smallSpots)
        {
            Gizmos.DrawCube(CoordToWorldSpace(c) + Vector3.up, Vector3.one * 3 * sizeMultipler);
        }*/

        if (levelMeshData.solidEdgeVertices != null)
        {
            Gizmos.color = Color.blue;
            /*foreach (Vector3 edgeVert in levelMeshData.solidEdgeVertices)
            {
                //Gizmos.DrawCube(edgeVert + Vector3.up + Vector3.left*4 + Vector3.forward *4, Vector3.one * 3);
            }*/
            foreach(List<Fence> outline in fenceOutlines)
            {
                for (int i = 0; i < outline.Count-1; i++)
                {
                    Gizmos.DrawLine(outline[i].position + Vector3.up, outline[i + 1].position + Vector3.up);
                }
            }
        }
    }

}
