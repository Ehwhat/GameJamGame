using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
[RequireComponent(typeof(Camera))]
public class RenderDepth : MonoBehaviour {

	void OnEnable()
    {
        GetComponent<Camera>().depthTextureMode = DepthTextureMode.DepthNormals;
    }
}
