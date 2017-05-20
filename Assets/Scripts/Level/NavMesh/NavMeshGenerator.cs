using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NavMeshGenerator : MonoBehaviour {

    [SerializeField]
    public NavMeshSurface surface;

    public void GenerateNavMesh()
    {
        surface.Bake();
        NavMesh.AddNavMeshData(surface.bakedNavMeshData, Vector3.zero, Quaternion.identity);
    }
	
}
