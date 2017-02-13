using UnityEngine;
using System.Collections;
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

    public string seed;
    public bool useRandomSeed;

    public bool constantGeneration = false;

    int[,] level;

    void Start()
    {
        GenerateLevel();
        StartCoroutine(GenLevel());
    }

    IEnumerator GenLevel()
    {
        while (true)
        {
            if (constantGeneration)
            {
                GenerateLevel();
                
            }
            yield return new WaitForSeconds(.5f);
        }
    }

    void GenerateLevel()
    {
        level = new int[width, height];
        RandomFillLevel();
        for (int i = 0; i < smoothAmount; i++)
        {
            SmoothLevel();
        }

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
        meshGen.GenerateMesh(borderedLevel, sizeMultiplier);
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
                if (nX >= 0 && nX < width && nY >= 0 && nY < height)
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

    void RandomFillLevel()
    {
        if (useRandomSeed)
            seed = DateTime.Now.Ticks.ToString();

        System.Random randGen = new System.Random(seed.GetHashCode());

        for (int x = padding; x < width-padding; x++)
        {
            for (int y = padding; y < height-padding; y++)
            {
                level[x, y] = (randGen.Next(0, 100) < fillAmount) ? 1 : 0;
            }
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
