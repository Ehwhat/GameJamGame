using UnityEngine;
using System.Collections;
[ExecuteInEditMode]
public class SimpleLerpCamera : MonoBehaviour {

    public Material TransitionMaterial;
    public Camera _camera;

    void Start()
    {
        if (_camera != null)
        {
            SetUpRenderTexture();
        }
    }

    void OnRenderImage(RenderTexture src, RenderTexture dst)
    {
        if (TransitionMaterial != null)
            Graphics.Blit(src, dst, TransitionMaterial);
    }

    void SetUpRenderTexture()
    {
        _camera.targetTexture = null;
        RenderTexture rt = new RenderTexture(_camera.pixelWidth, _camera.pixelHeight, 8);
        _camera.targetTexture = rt;
        TransitionMaterial.SetTexture("_TransitionTex", rt);
    }
}
