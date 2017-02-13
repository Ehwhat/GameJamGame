using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LevelMeshGenerator : MonoBehaviour {

    public SquareGrid squareGrid;

    List<Vector3> vertices;
    List<int> triangles;

    Dictionary<int, List<Triangle>> triangleDict = new Dictionary<int, List<Triangle>>();

    //18:45 EP4

    public void GenerateMesh(int[,] level, float squareSize)
    {
        squareGrid = new SquareGrid(level, squareSize);

        vertices = new List<Vector3>();
        triangles = new List<int>();

        for (int x = 0; x < squareGrid.squares.GetLength(0); x++)
        {
            for (int y = 0; y < squareGrid.squares.GetLength(1); y++)
            {
                TriangulateSquare(squareGrid.squares[x, y]);
            }
        }

        Mesh mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;

        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.RecalculateNormals();

    }

    void TriangulateSquare(MarchingSquare square) // oh boy here we go
    {
        switch (square.configuration)
        {
            case 0:
                break;

                // 1 point cases
            case 1:
                MeshFromPoints(square.centreLeft, square.centreBottom, square.bottomLeft);
                break;
            case 2:
                MeshFromPoints(square.bottomRight, square.centreBottom, square.centreRight);
                break;
            case 4:
                MeshFromPoints(square.topRight, square.centreRight, square.centreTop);
                break;
            case 8:
                MeshFromPoints(square.topLeft, square.centreTop, square.centreLeft);
                break;

            // 2 point cases

            case 3:
                MeshFromPoints(square.centreRight, square.bottomRight, square.bottomLeft, square.centreLeft);
                break;
            case 6:
                MeshFromPoints(square.centreTop, square.topRight, square.bottomRight, square.centreBottom);
                break;
            case 9:
                MeshFromPoints(square.topLeft, square.centreTop, square.centreBottom, square.bottomLeft);
                break;
            case 12:
                MeshFromPoints(square.topLeft, square.topRight, square.centreRight, square.centreLeft);
                break;
            case 5:
                MeshFromPoints(square.centreTop, square.topRight, square.centreRight, square.centreBottom, square.bottomLeft, square.centreLeft);
                break;
            case 10:
                MeshFromPoints(square.topLeft, square.centreTop, square.centreRight, square.bottomRight, square.centreBottom, square.centreLeft);
                break;

            // 3 point cases

            case 7:
                MeshFromPoints(square.centreTop, square.topRight, square.bottomRight, square.bottomLeft, square.centreLeft);
                break;
            case 11:
                MeshFromPoints(square.topLeft, square.centreTop, square.centreRight, square.bottomRight, square.bottomLeft);
                break;
            case 13:
                MeshFromPoints(square.topLeft, square.topRight, square.centreRight, square.centreBottom, square.bottomLeft);
                break;
            case 14:
                MeshFromPoints(square.topLeft, square.topRight, square.bottomRight, square.centreBottom, square.centreLeft);
                break;

            //4 Point case

            case 15:
                MeshFromPoints(square.topLeft, square.topRight, square.bottomRight, square.bottomLeft);
                break;
        }
    }

    void MeshFromPoints(params Node[] points)
    {
        AssignVertices(points);
        if(points.Length >= 3)
        {
            CreateTriangle(points[0], points[1], points[2]);
        }
        if(points.Length >= 4)
        {
            CreateTriangle(points[0], points[2], points[3]);
        }
        if (points.Length >= 5)
        {
            CreateTriangle(points[0], points[3], points[4]);
        }
        if (points.Length >= 6)
        {
            CreateTriangle(points[0], points[4], points[5]);
        }
    }

    void AssignVertices(Node[] points)
    {
        for(int i = 0; i < points.Length; i++)
        {
            if(points[i].vertexIndex == -1)
            {
                points[i].vertexIndex = vertices.Count;
                vertices.Add(points[i].pos);
            }
        }
    }

    void CreateTriangle(Node a, Node b, Node c)
    {
        triangles.Add(a.vertexIndex);
        triangles.Add(b.vertexIndex);
        triangles.Add(c.vertexIndex);

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

    int GetConnectedOutlineVertex(int vertex)
    {
        List<Triangle> triContainingVertex = triangleDict[vertex];

        for (int i = 0; i < triContainingVertex.Count; i++)
        {
            Triangle tri = triContainingVertex[i];

            for (int j = 0; j < 3; j++)
            {
                int vertB = tri[j];
                if (vertB != vertex)
                    continue;
                if (IsOutlineEdge(vertex, vertB))
                {
                    return vertB;
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
