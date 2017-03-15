using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(Camera))]
public class CameraTracking : MonoBehaviour {

    public List<Transform> trackedTransforms;

    public float isoOffset = 45;
    public float zoomOffset = 5;
    public Vector3 cameraOffset = new Vector3(-7, 10, 0);

    Vector3 currentCameraTarget;
	float lastZoom;
    new Camera camera;

	// Use this for initialization
	void Start () {
        camera = GetComponent<Camera>();
	}
	
	// Update is called once per frame
	void Update () {
        transform.position = CalculateCameraPosition();
        camera.orthographicSize = CalculateCameraZoom();

    }

	public float GetLastZoomResult(){
		return lastZoom;
	}

	public Vector3 GetLastPosition(){
		return transform.InverseTransformPoint(currentCameraTarget);
	}

    Vector3 CalculateCameraPosition()
    {
        Vector3 distProduct = Vector3.zero;

        if (trackedTransforms.Count > 0)
        {
            distProduct = trackedTransforms[0].position;

            if (trackedTransforms.Count > 1)
            {
                for (int i = 1; i < trackedTransforms.Count; i++)
                {
                    distProduct += trackedTransforms[i].position;

                }
            }

            distProduct /= trackedTransforms.Count;
            distProduct += Quaternion.AngleAxis(isoOffset, Vector3.up) * cameraOffset;

        }
		currentCameraTarget = distProduct;

        return distProduct;

    }

    float CalculateCameraZoom()
    {
        float zoomResult = 0;

        Vector3 currentDesiredPos = transform.InverseTransformPoint(currentCameraTarget);

        for (int i = 0; i < trackedTransforms.Count; i++)
        {
            Vector3 localPos = transform.InverseTransformPoint(trackedTransforms[i].position);
            Vector3 desiredPos = localPos - currentDesiredPos;

            zoomResult = Mathf.Max(zoomResult, Mathf.Abs(desiredPos.y));
            zoomResult = Mathf.Max(zoomResult, Mathf.Abs(desiredPos.x)/camera.aspect);

        }

        zoomResult += zoomOffset;
		lastZoom = zoomResult;

        return zoomResult;

    }

    public void RegisterTransform(Transform t)
    {
        trackedTransforms.Add(t);
    }

    public void DeregisterTransform(Transform t)
    {
        trackedTransforms.Remove(t);
    }

}
