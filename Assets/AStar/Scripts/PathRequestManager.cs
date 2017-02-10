﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Threading;

public class PathRequestManager : MonoBehaviour {

    Queue<PathResult> results = new Queue<PathResult>();

    static PathRequestManager instance;

    Pathfinding pathfinding;

   


    void Awake()
    {
        instance = this;
        pathfinding = GetComponent<Pathfinding>();
    }

    void Update()
    {
        if (results.Count > 0)
        {
            int itemInQueue = results.Count;
            lock (results)
            {
                for (int i=0; i < itemInQueue; i++)
                {
                    PathResult result = results.Dequeue();
                    result.callback(result.path, result.success);
                }
            }
        }
    }

    public static void RequestPath(PathRequest request)
    {
        ThreadStart threadStart = delegate{
            instance.pathfinding.FindPath(request, instance.FinishedProcessingPath);
        };
        threadStart.Invoke();
    }



    public void FinishedProcessingPath(PathResult result)
    {
        lock (results)
        {
            results.Enqueue(result);
        }
    } 
}

public struct PathResult
{
    public Vector3[] path;
    public bool success;
    public Action<Vector3[], bool> callback;

    public PathResult(Vector3[] path, bool success, Action<Vector3[], bool> callback)
    {
        this.path = path;
        this.success = success;
        this.callback = callback;
    }
}

public struct PathRequest
{
    public Vector3 pathStart;
    public Vector3 pathEnd;
    public Action<Vector3[], bool> callback;

    public PathRequest(Vector3 start_, Vector3 end_, Action<Vector3[], bool> callback_)
    {
        pathStart = start_;
        pathEnd = end_;
        callback = callback_;
    }
}
