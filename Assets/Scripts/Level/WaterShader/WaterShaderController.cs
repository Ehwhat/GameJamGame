using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WaterShaderController : MonoBehaviour {

	public Material waterMaterial;

	public AnimationCurve resolutionCurve;

	public int waterSquareResX = 20;
	public int waterSquareResY = 20;

	public float waterSquareSizeX = 12;
	public int waterSquareSizeY = 12;

	public int waterSquares = 8;

	public float scale;
	public float speed;
	public float waveDist;
	public float noiseStr;
	public float noiseWalk;

	public ReflectionProbe rp;
	 
	// Use this for initialization
	void Start () {
		//rp.center = new Vector3(waterSquares*waterSquareSizeX/2,0,waterSquares*waterSquareSizeY/2);
		Shader.SetGlobalFloat("_WaterSize",(waterSquareSizeX+waterSquareSizeY)/2);
		for(float x = 0; x < waterSquares; x++){
			for(float y = 0; y < waterSquares; y++){
				int detailMod = 1;//Mathf.RoundToInt(resolutionCurve.Evaluate(((x/waterSquares)+(y/waterSquares))/2));
				createOceanSquare(new Vector3(
					(x*waterSquareSizeX)-(waterSquareSizeX*waterSquares)/2,
					transform.position.y,
					(y*waterSquareSizeY)-(waterSquareSizeY*waterSquares)/2
				), detailMod);
			}
		}
	}

	// Update is called once per frame
	void Update () {
		Shader.SetGlobalFloat("_WaterScale", scale);
		Shader.SetGlobalFloat("_WaterSpeed", speed);
		Shader.SetGlobalFloat("_WaterDistance", waveDist);
		Shader.SetGlobalFloat("_WaterTime", Time.time);
		Shader.SetGlobalFloat("_WaterNoiseStrength", noiseStr);
		Shader.SetGlobalFloat("_WaterNoiseWalk", noiseWalk);
		//meshFilter.mesh.RecalculateNormals();
		//meshCollider.sharedMesh = meshFilter.mesh;
	}

	void createOceanSquare(Vector3 position, int detailMod){
		MeshFilter meshFilter = new GameObject("oceanSquare").AddComponent<MeshFilter>();
        meshFilter.gameObject.layer = gameObject.layer;
		MeshRenderer meshRenderer = meshFilter.gameObject.AddComponent<MeshRenderer>();
		meshRenderer.sharedMaterial = waterMaterial;
		meshFilter.transform.parent = transform;
		meshFilter.transform.position = position;
		meshFilter.transform.localScale = new Vector3(waterSquareSizeX,1,waterSquareSizeY);
		meshFilter.mesh = createPlaneMesh(meshFilter.transform, detailMod);
	}

	Mesh createPlaneMesh(Transform trans, int detailMod){
		Mesh newMesh = new Mesh();
		List<Vector3> verticeList = new List<Vector3>();
		List<Vector2> uvList = new List<Vector2>();
		List<int> triList = new List<int>();

		int width = waterSquareResX*detailMod;
		int height = waterSquareResY*detailMod;
		for (int x = 0; x < width; x++)
		{
			for (int y = 0; y < height; y++)
			{
				float xf = x;
				float yf = y;
				verticeList.Add(new Vector3(xf/(width-1),0,yf/(height-1)));
				uvList.Add(new Vector2(x, y));
				//Skip if a new square on the plane hasn't been formed
				if (x == 0 || y == 0)
					continue;
				//Adds the index of the three vertices in order to make up each of the two tris
				triList.Add(width * x + y); //Top right
				triList.Add(width * x + y - 1); //Bottom right
				triList.Add(width * (x - 1) + y - 1); //Bottom left - First triangle
				triList.Add(width * (x - 1) + y - 1); //Bottom left 
				triList.Add(width * (x - 1) + y); //Top left
				triList.Add(width * x + y); //Top right - Second triangle
			}
		}
		newMesh.vertices = verticeList.ToArray();
		newMesh.uv = uvList.ToArray();
		newMesh.triangles = triList.ToArray();
		newMesh.RecalculateNormals();
		return newMesh;
	}

	IEnumerator renderReflection(){
		while(true){
			//rp.RenderProbe();
			yield return new WaitForSeconds(0.1f);
		}	
	}

	void OnDrawGizmos(){
		Gizmos.color = Color.blue;
		Gizmos.DrawWireCube(transform.position, new Vector3(waterSquares*waterSquareSizeX,0,waterSquares*waterSquareSizeY));

	}

}
