using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class SimpleBlit : MonoBehaviour
{
    [Range(0,1)]public float _cutoff = 0;
    public Material TransitionMaterial;

    void OnRenderImage(RenderTexture src, RenderTexture dst)
    {
        if (TransitionMaterial != null)
            Graphics.Blit(src, dst, TransitionMaterial);
    }

    void Update()
    {
        TransitionMaterial.SetFloat("_Cutoff", _cutoff);
    }

    public void LerpCutoff(float start, float end, float time)
    {
        StopAllCoroutines();
        StartCoroutine(Lerp(start, end, time));
    }

    IEnumerator Lerp(float start, float end, float time)
    {
        float current = 0;
        while (current < time)
        {
            _cutoff = Mathf.Lerp(start, end, current / time);
            current += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
    }
}
