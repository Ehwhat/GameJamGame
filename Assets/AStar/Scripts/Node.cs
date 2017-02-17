using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : IHeapItem<Node>  {

    public bool walkable;
    public Vector3 worldPosition;

    public int gridX;
    public int gridY;
    public int movementPenalty;

    public int gCost;
    public int hCost;
    public Node parent;
    int heapIndex;


    public Node(bool walkable_, Vector3 worldPos_, int gridX_, int gridY_, int penalty_)
    {
        walkable = walkable_;
        worldPosition = worldPos_;
        gridX = gridX_;
        gridY = gridY_;
        movementPenalty = penalty_;
    }

    public int fCost
    {
        get
        {
            return gCost + hCost;
        }
    }

    public int HeapIndex
    {
        get
        {
            return heapIndex;
        }
        set
        {
            heapIndex = value;
        }
    }

    public int CompareTo(Node nodeToCompare)
    {
        int compare = fCost.CompareTo(nodeToCompare.fCost);
        if (compare == 0)
        {
            compare = hCost.CompareTo(nodeToCompare.hCost);
        }
        return -compare;
    }
}
