using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public struct LevelMeshData
{
    public List<Vector3> vertices;
    public List<Vector3> solidVertices;
    public List<Vector3> solidEdgeVertices;
    public List<Vector3> edgeVertices;
}

public class LevelMeshGenerator : MonoBehaviour {

    public SquareGrid squareGrid;
    public MeshFilter walls;
    public MeshFilter edges;
    public MeshFilter floor;
    public float wallHeight = 20;
    public int tileAmountX = 10;
    public int tileAmountY = 10;

    List<Vector3> vertices;
    List<int> triangles;

    List<int> solidVertices;
    List<int> solidEdgeVertices;
    List<int> edgeTriangles;

    Dictionary<int, List<Triangle>> triangleDict = new Dictionary<int, List<Triangle>>();
    List<List<int>> outlines = new List<List<int>>();
    HashSet<int> checkedVerts = new HashSet<int>();

    //18:45 EP4

    public LevelMeshData GenerateMesh(int[,] level, float squareSize)
    {

        triangleDict.Clear();
        outlines.Clear();
        checkedVerts.Clear();
        squareGrid = new SquareGrid(level, squareSize);

        vertices = new List<Vector3>();
        triangles = new List<int>();
        solidVertices = new List<int>();
        solidEdgeVertices = new List<int>();
        edgeTriangles = new List<int>();

        for (int x = 0; x < squareGrid.squares.GetLength(0); x++)
        {
            for (int y = 0; y < squareGrid.squares.GetLength(1); y++)
            {
                TriangulateSquare(squareGrid.squares[x, y]);
            }
        }

        CalculateMeshOutlines();

        Mesh mesh = new Mesh();
        floor.GetComponent<MeshFilter>().mesh = mesh;

        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();

        Vector2[] uvs = new Vector2[vertices.Count];
        for (int i = 0; i < uvs.Length; i++)
        {
            float percentX = Mathf.InverseLerp(-level.GetLength(0)/2*squareSize, level.GetLength(0) / 2 * squareSize, vertices[i].x)*tileAmountX;
            float percentY = Mathf.InverseLerp(-level.GetLength(1) / 2 * squareSize, level.GetLength(1) / 2 * squareSize, vertices[i].z)*tileAmountY;
            uvs[i] = new Vector2(percentX, percentY);
        }

        mesh.uv = uvs;

        floor.GetComponent<MeshCollider>().sharedMesh = null;
        floor.GetComponent<MeshCollider>().sharedMesh = mesh;

        Mesh edgeMesh = new Mesh();
        edges.mesh = edgeMesh;
        


        edgeMesh.vertices = vertices.ToArray();
        edgeMesh.triangles = edgeTriangles.ToArray();
        edgeMesh.RecalculateNormals();
        edgeMesh.RecalculateBounds();

        List<int> edgeVertices = new List<int>();
        Vector2[] edgeUVs = new Vector2[vertices.Count];

        foreach (List<int> outline in outlines)
        {
            for (int i = 0; i < outline.Count; i++)
            {
                if (!solidVertices.Contains(outline[i]))
                {
                    
                    edgeVertices.Add(outline[i]);
                }

                //uvs[outline[i]] = new Vector2(0, 0);
            }
        }

        for (int i = 0; i < edgeVertices.Count; i++)
        {
            edgeUVs[edgeVertices[i]] = new Vector2(0, 1f);
        }

        edgeMesh.uv = uvs;
        edgeMesh.uv2 = edgeUVs;

        edges.GetComponent<MeshCollider>().sharedMesh = null;
        edges.GetComponent<MeshCollider>().sharedMesh = edgeMesh;

        CreateWallMesh(level, squareSize);

        LevelMeshData levelData = new LevelMeshData();
        levelData.vertices = vertices;

        List<Vector3> solidVerts = new List<Vector3>();
        foreach (int i in solidVertices)
        {
            solidVerts.Add(vertices[i]);
        }

        levelData.solidVertices = solidVerts;

        List<Vector3> edgeVerts = new List<Vector3>();
        foreach (int i in edgeVertices)
        {
            edgeVerts.Add(vertices[i]);
        }

        List<Vector3> solidEdgeVerts = new List<Vector3>();
        foreach (int i in solidEdgeVertices)
        {
            solidEdgeVerts.Add(vertices[i]);
        }


        levelData.solidEdgeVertices = solidEdgeVerts;

        levelData.edgeVertices = edgeVerts;

        return levelData;

    }

    void CreateWallMesh(int[,] level, float squareSize)
    {

        List<Vector3> wallVerts = new List<Vector3>();
        List<int> wallTris = new List<int>();

        Mesh wallMesh = new Mesh();


        foreach(List<int> outline in outlines)
        {
            for (int i = 0; i < outline.Count-1; i++)
            {
                int startIndex = wallVerts.Count;


                wallVerts.Add(vertices[outline[i]]);
                wallVerts.Add(vertices[outline[i+1]]);
                wallVerts.Add(vertices[outline[i]] - transform.up* wallHeight);
                wallVerts.Add(vertices[outline[i + 1]] - transform.up* wallHeight);

                wallTris.Add(startIndex + 0);
                wallTris.Add(startIndex + 2);
                wallTris.Add(startIndex + 3);

                wallTris.Add(startIndex + 3);
                wallTris.Add(startIndex + 1);
                wallTris.Add(startIndex + 0);

            }
        }
        wallMesh.vertices = wallVerts.ToArray();
        wallMesh.triangles = wallTris.ToArray();
        wallMesh.RecalculateNormals();

        /*Vector2[] uvs = new Vector2[wallVerts.Count];
        for (int i = 0; i < uvs.Length; i++)
        {
            float percentX = Mathf.InverseLerp(-level.GetLength(0) / 2 * squareSize, level.GetLength(0) / 2 * squareSize, wallVerts[i].x) * tileAmountX;
            float percentY = Mathf.InverseLerp(-level.GetLength(0) / 2 * squareSize, level.GetLength(0) / 2 * squareSize, wallVerts[i].y) * tileAmountY;
            uvs[i] = new Vector2(percentX, percentY);
        }

        wallMesh.uv = uvs;
        */
        walls.mesh = wallMesh;
    }

    void TriangulateSquare(MarchingSquare square) // oh boy here we go
    {
        switch (square.configuration)
        {
            case 0:
                break;

                // 1 point cases
            case 1:
                MeshFromPoints(false,square.centreLeft, square.centreBottom, square.bottomLeft);
                solidEdgeVertices.Add(square.bottomLeft.vertexIndex);
                break;
            case 2:
                MeshFromPoints(false, square.bottomRight, square.centreBottom, square.centreRight);
                solidEdgeVertices.Add(square.bottomRight.vertexIndex);
                break;
            case 4:
                MeshFromPoints(false, square.topRight, square.centreRight, square.centreTop);
                solidEdgeVertices.Add(square.topRight.vertexIndex);
                break;
            case 8:
                MeshFromPoints(false, square.topLeft, square.centreTop, square.centreLeft);
                solidEdgeVertices.Add(square.topLeft.vertexIndex);
                break;

            // 2 point cases

            case 3:
                MeshFromPoints(false,square.centreRight, square.bottomRight, square.bottomLeft, square.centreLeft);
                solidEdgeVertices.Add(square.bottomLeft.vertexIndex);
                solidEdgeVertices.Add(square.bottomRight.vertexIndex);
                break;
            case 6:
                MeshFromPoints(false, square.centreTop, square.topRight, square.bottomRight, square.centreBottom);
                solidEdgeVertices.Add(square.topRight.vertexIndex);
                solidEdgeVertices.Add(square.bottomRight.vertexIndex);
                break;
            case 9:
                MeshFromPoints(false, square.topLeft, square.centreTop, square.centreBottom, square.bottomLeft);
                solidEdgeVertices.Add(square.topLeft.vertexIndex);
                solidEdgeVertices.Add(square.bottomLeft.vertexIndex);
                break;
            case 12:
                MeshFromPoints(false, square.topLeft, square.topRight, square.centreRight, square.centreLeft);
                solidEdgeVertices.Add(square.topRight.vertexIndex);
                solidEdgeVertices.Add(square.topLeft.vertexIndex);
                break;
            case 5:
                MeshFromPoints(false, square.topRight, square.centreRight, square.centreBottom, square.bottomLeft, square.centreLeft, square.centreTop);
                break;
            case 10:
                MeshFromPoints(false, square.topLeft, square.centreTop, square.centreRight, square.bottomRight, square.centreBottom, square.centreLeft);
                break;

            // 3 point cases

            case 7:
                MeshFromPoints(false, square.topRight, square.bottomRight, square.bottomLeft, square.centreLeft, square.centreTop);
                solidEdgeVertices.Add(square.topRight.vertexIndex);
                solidEdgeVertices.Add(square.bottomLeft.vertexIndex);
                break;
            case 11:
                MeshFromPoints(false, square.topLeft, square.centreTop, square.centreRight, square.bottomRight, square.bottomLeft);
                solidEdgeVertices.Add(square.topLeft.vertexIndex);
                solidEdgeVertices.Add(square.bottomRight.vertexIndex);
                break;
            case 13:
                MeshFromPoints(false, square.topRight, square.centreRight, square.centreBottom, square.bottomLeft,square.topLeft);
                solidEdgeVertices.Add(square.bottomLeft.vertexIndex);
                solidEdgeVertices.Add(square.topRight.vertexIndex);
                break;
            case 14:
                MeshFromPoints(false, square.topLeft, square.topRight, square.bottomRight, square.centreBottom, square.centreLeft);
                solidEdgeVertices.Add(square.topLeft.vertexIndex);
                solidEdgeVertices.Add(square.bottomRight.vertexIndex);

                break;

            //4 Point case

            case 15:
                MeshFromPoints(true,square.topLeft, square.topRight, square.bottomRight, square.bottomLeft);
                checkedVerts.Add(square.topLeft.vertexIndex);
                checkedVerts.Add(square.bottomLeft.vertexIndex);
                checkedVerts.Add(square.topRight.vertexIndex);
                checkedVerts.Add(square.bottomRight.vertexIndex);
                solidVertices.Add(square.topLeft.vertexIndex);
                solidVertices.Add(square.bottomLeft.vertexIndex);
                solidVertices.Add(square.topRight.vertexIndex);
                solidVertices.Add(square.bottomRight.vertexIndex);
                break;
        }
    }

    void MeshFromPoints(bool nonEdge, params Node[] points)
    {
        AssignVertices(nonEdge,points);
        if(points.Length >= 3)
        {
            CreateTriangle(nonEdge,points[0], points[1], points[2]);
        }
        if(points.Length >= 4)
        {
            CreateTriangle(nonEdge,points[0], points[2], points[3]);
        }
        if (points.Length >= 5)
        {
            CreateTriangle(nonEdge,points[0], points[3], points[4]);
        }
        if (points.Length >= 6)
        {
            CreateTriangle(nonEdge,points[0], points[4], points[5]);
        }
    }

    void AssignVertices(bool nonEdge,Node[] points)
    {
        for(int i = 0; i < points.Length; i++)
        {
            if(points[i].vertexIndex == -1)
            {
                points[i].vertexIndex = vertices.Count;
                vertices.Add(points[i].pos);
                if (!nonEdge)
                {
                    //edgeVertices.Add(points[i].pos);
                }
            }
        }
    }

    void CreateTriangle(bool nonEdge, Node a, Node b, Node c)
    {
        if (nonEdge)
        {
            triangles.Add(a.vertexIndex);
            triangles.Add(b.vertexIndex);
            triangles.Add(c.vertexIndex);
        }else
        {
            edgeTriangles.Add(a.vertexIndex);
            edgeTriangles.Add(b.vertexIndex);
            edgeTriangles.Add(c.vertexIndex);
        }

        Triangle tri = new Triangle(a.vertexIndex, b.vertexIndex, c.vertexIndex);
        AddTriangleToDictionary(tri.vertexIndexA, tri);
        AddTriangleToDictionary(tri.vertexIndexB, tri);
        AddTriangleToDictionary(tri.vertexIndexC, tri);
    }

    void AddTriangleToDictionary(int index, Triangle triangle)
    {
        if (triangleDict.ContainsKey(index))
        {
            triangleDict[index].Add(triangle);
        }else
        {
            List<Triangle> triList = new List<Triangle>();
            triList.Add(triangle);
            triangleDict.Add(index, triList);
        }
    }

    void CalculateMeshOutlines()
    {
        for (int vertIndex = 0; vertIndex < vertices.Count; vertIndex++)
        {
            if (!checkedVerts.Contains(vertIndex))
            {
                int newOutlineVert = GetConnectedOutlineVertex(vertIndex);
                if(newOutlineVert != -1)
                {
                    
                    checkedVerts.Add(vertIndex);
                    List<int> newOutline = new List<int>();
                    newOutline.Add(vertIndex);
                    outlines.Add(newOutline);
                    FollowOutlines(newOutlineVert, outlines.Count-1);
                    outlines[outlines.Count - 1].Add(vertIndex);
                }
            }
        }
    }

    void FollowOutlines(int vertIndex, int outlineIndex)
    {
        outlines[outlineIndex].Add(vertIndex);
        checkedVerts.Add(vertIndex);
        int nextIndex = GetConnectedOutlineVertex(vertIndex);
        if(nextIndex != -1)
        {
            FollowOutlines(nextIndex, outlineIndex);
        }
    }

    int GetConnectedOutlineVertex(int vertex)
    {
        List<Triangle> triContainingVertex = triangleDict[vertex];

        for (int i = 0; i < triContainingVertex.Count; i++)
        {

            Triangle tri = triContainingVertex[i];

            for (int j = 0; j < 3; j++)
            {
                int vertB = tri[j];
                if (vertB != vertex && !checkedVerts.Contains(vertB))
                {
                    if (IsOutlineEdge(vertex, vertB))
                    {
                        return vertB;
                    }
                }
            }

        }

        return -1;

    }

    bool IsOutlineEdge(int vertexA, int vertexB)
    {
        List<Triangle> trianglesContainingA = triangleDict[vertexA];
        int sharedTriangleAmount = 0 ;
        for (int i = 0; i < trianglesContainingA.Count; i++)
        {
            if (trianglesContainingA[i].Contains(vertexB))
            {
                sharedTriangleAmount++;
                if (sharedTriangleAmount > 1)
                    break;
            }
        }
        return sharedTriangleAmount == 1;
    }

    struct Triangle
    {
        public int vertexIndexA, vertexIndexB, vertexIndexC;

        int[] verts;

        public Triangle(int a, int b, int c)
        {
            vertexIndexA = a;
            vertexIndexB = b;
            vertexIndexC = c;
            verts = new int[3];
            verts[0] = a;
            verts[1] = b;
            verts[2] = c;
        }

        public int this[int i]
        {
            get
            {
                return verts[i];
            }
        }

        public bool Contains(int vertex)
        {
            return vertex == vertexIndexA || vertex == vertexIndexB || vertex == vertexIndexC;
        }

    }

    public class SquareGrid
    {
        public MarchingSquare[,] squares;

        public SquareGrid(int[,] level, float size)
        {
            int nodeCountX = level.GetLength(0);
            int nodeCountY = level.GetLength(1);
            float mapWidth = nodeCountX * size;
            float mapHeight = nodeCountY * size;

            ControlNode[,] controlNodes = new ControlNode[nodeCountX, nodeCountY];

            for (int x = 0; x < nodeCountX; x++)
            {
                for (int y = 0; y < nodeCountY; y++)
                {
                    Vector3 pos = new Vector3(-mapWidth / 2 + x * size + size / 2, 0, -mapHeight / 2 + y * size + size / 2);
                    controlNodes[x, y] = new ControlNode(pos, (level[x, y] == 1), size);
                }
            }

            squares = new MarchingSquare[nodeCountX - 1, nodeCountY - 1];

            for (int x = 0; x < nodeCountX-1; x++)
            {
                for (int y = 0; y < nodeCountY-1; y++)
                {
                    squares[x, y] = new MarchingSquare(controlNodes[x,y+1], controlNodes[x+1,y+1], controlNodes[x+1, y], controlNodes[x,y]);
                }
            }
        }

    }

    public class MarchingSquare
    {
        public ControlNode topLeft, topRight, bottomRight, bottomLeft;
        public Node centreTop, centreRight, centreLeft, centreBottom;
        public int configuration;

        public MarchingSquare(ControlNode _topLeft, ControlNode _topRight, ControlNode _bottomRight, ControlNode _bottomLeft)
        {
            topLeft = _topLeft;
            topRight = _topRight;
            bottomLeft = _bottomLeft;
            bottomRight = _bottomRight;

            centreTop = topLeft.right;
            centreRight = bottomRight.above;
            centreBottom = bottomLeft.right;
            centreLeft = bottomLeft.above;

            if (topLeft.active)
                configuration += 8;
            if (topRight.active)
                configuration += 4;
            if (bottomRight.active)
                configuration += 2;
            if (bottomLeft.active)
                configuration += 1;

        }

    }

    public class Node
    {
        public Vector3 pos;
        public int vertexIndex = -1;

        public Node (Vector3 p)
        {
            pos = p;
            pos += new Vector3(Random.Range(-0.3f, 0.6f), Random.Range(-0.3f, 0.6f), Random.Range(-0.3f, 0.6f));
            vertexIndex = -1;
        }
    }

    public class ControlNode : Node
    {
        public bool active;
        public Node above, right;

        public ControlNode(Vector3 p, bool _active, float size) : base(p)
        {
            active = _active;
            above = new Node(pos + Vector3.forward * size / 2);
            right = new Node(pos + Vector3.right * size / 2);

        }
    }
}
