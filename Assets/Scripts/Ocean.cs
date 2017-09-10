using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Ocean : MonoBehaviour {

	public MeshFilter meshFilter;
	public MeshRenderer meshRenderer;

	Mesh mesh;

	public Color PeakColour; //91AEF400
	public Color SurfaceColour; //0295CC00
	public Color DeepColour; //2830D800
	public Color TroughColour; //0E005A00

	public bool flatShading = true;
	// can go out of bounds it not a square mesh
	int width = 20;
	int height = 20;
	float scale = 6;

	float waveAmplitude = 5;

	//float timeToNextWave = 0;
	//public float timeBetweenRecalculates = 0.2f;

	//Vector3[] newVerts;// = new Vector3[width * height];

	// Use this for initialization
	void Start () {

		Vector3[] vertices = makeVerts (width, height);
		makeMesh (vertices);
		//newVerts = new Vector3[width * height];

	}
	
	// Update is called once per frame
	void Update () {
		moveWaves ();
	}

	Color getColourAtPosition(int x, int z, float height)
	{
		if (height > getSinHeight (x + 1, z) && height > getSinHeight (x - 1, z) && height > getSinHeight(x, z+1) && height > getSinHeight(x, z-1)) {
			// higher than neighbours in x direction
			return Color.white;
		} else if (height < getSinHeight (x + 1, z) && height < getSinHeight (x - 1, z) && height < getSinHeight(x, z+1) && height < getSinHeight(x, z-1)) {
			// lower than neighbours
			return Color.grey;
		} else {
			return Color.blue;
		}
	}

	public Color getColourAtHeight(float height)
	{
		// put height in the range of -1 to 1
		height /= waveAmplitude;

		// modifying height to get into the range of 0-1
		// if height is greater than 0.8, it can be from 0.8 tp 1.0
		// subtract by 0.8 to get from 0 to 0.2
		// divide by 0.2 to get from 0 to 1

		//TODO maybe have a ring around the peak that is white
		// 0.7 < height < 0.8 == white
		// probably woulnd't work how I want it to

		// option 2: slope based colour

		if (height > 0.8f) {
			return Color.Lerp (SurfaceColour, PeakColour, (height-0.8f)/0.2f); // Color.white;
		} else if (height > -0.8f) {
			//return Color.blue;
			return Color.Lerp (DeepColour, SurfaceColour, (height + 0.8f) / 1.6f);
		} else {
			return Color.Lerp(DeepColour, TroughColour, (height+0.8f)/(-0.2f));
		}
	}

	public float getHeightAtPosition(Vector3 position)
	{
		position = position - transform.position;
		return getSinHeight (position.x / scale, position.z / scale);
	}

	float getSinHeight(float x, float y)
	{
		x = x / (float)width;
		y = y / (float)height;
		float currentTime = Time.time;
		// each sin goes from -1 to 1
		// the range is therefore the number of sines
		// 3 sines: -3 to 3
		return (Mathf.Sin(12 * x + currentTime) + Mathf.Sin(12 * y + currentTime) + Mathf.Sin(6 * x + 0.5f * currentTime)) / 3 * waveAmplitude;
		// the higher the nuumber before the x or y, the greater the number of swells
		// multiplying time makes the waves move across faster
			// Mathf.Sin (4 * x + 2* currentTime) + Mathf.Sin(1.7f * y + 2.4f * currentTime) + Mathf.Sin(3.1f * x + 0.4f * Time.time + 2.8f * y) / 3;
			//Mathf.Sin (x * Time.time + x*x*3*Time.time) + Mathf.Sin (y * Time.time + 2 + (x+y)*Time.time) + Mathf.Sin((x+y) * Time.time + 1.4f * x * Time.time + 6.2f * y * Time.time);
			//Mathf.Sin (x * Time.time + 1.2f) + Mathf.Sin (x*x + 2 * Time.time * x + 2 * y) + Mathf.Sin (4 * x * Time.time + 0.5f * y) + Mathf.Sin(1.4f * Time.time * x * y) + Mathf.Sin (1.1f * Time.time * y + 0.2f) + Mathf.Sin (2.1f * Time.time * y);
	}

	void moveWaves()
	{
		int vertexIndex = 0;
		Vector3[] newVerts = mesh.vertices;
		Color[] colourMap = new Color[width * height]; 
		Texture2D texture;
		int[] triangles = mesh.triangles;
		//Vector2[] uvs = mesh.uv;

		// flat shading doens't look right when the mesh is moving along with the player
		// appear like you are in the same box the whole time
		if (flatShading) {
			colourMap = new Color[triangles.Length/6];
			texture = new Texture2D (width-1, height-1);
			for (int i = 0; i < triangles.Length; i++) {
				float newFlatheight = getHeightAtPosition (meshRenderer.transform.position + newVerts [i]);

				newVerts [i] = new Vector3 (newVerts [i].x, newFlatheight, newVerts [i].z);
				if (i % 6 == 0) {
					colourMap [i/6] = getColourAtHeight (newFlatheight);
				}
			}
			mesh.vertices = newVerts;
			//mesh.uv = flatUVs;

		} else {	
			texture = new Texture2D (width, height);
			for (int x = 0; x < width; x++) {
				for (int y = 0; y < height; y++) {


					//float newHeight = getSinHeight (x, y);
					float newHeight = getHeightAtPosition (meshRenderer.transform.position + new Vector3 (x * scale, 0, y * scale));
					if (!flatShading) {
						newVerts [vertexIndex] = new Vector3 (newVerts [vertexIndex].x, newHeight, newVerts [vertexIndex].z);
					}
					colourMap [vertexIndex] = getColourAtHeight (newHeight); //Color.Lerp (Color.gray, Color.blue, newHeight);
					vertexIndex++;
				}
			}
			mesh.vertices = newVerts;
		}

		//mesh.uv = uvs;
		texture.SetPixels (colourMap);
		texture.Apply ();






		//mesh.vertices = newVerts;
		//mesh.RecalculateNormals ();


		//texture.SetPixels (colourMap);
		//texture.Apply ();


		meshFilter.sharedMesh = mesh;
		meshRenderer.sharedMaterial.mainTexture = texture;

	}

	Vector3[] makeVerts(int xSize, int ySize)
	{
		Vector3[] verts = new Vector3[xSize * ySize];
		int vertIndex = 0;

		for (int x = 0; x < xSize; x++) {
			for (int y = 0; y < ySize; y++) {

				float yHeight = getSinHeight (x, y);

				verts [vertIndex] = transform.position + new Vector3 (x*scale, yHeight, y*scale);
				vertIndex++;

			}
		}

		return verts;
	}

	void makeMesh(Vector3[] verts)
	{
		// mesh
		mesh = new Mesh ();
		int[] triangles = new int[(width - 1) * (height - 1) * 6];
		int triangleIndex = 0;
		int vertexIndex = 0;
		Vector2[] uvs = new Vector2[width * height];

		// texture
		Texture2D texture = new Texture2D(width, height);
		Color[] colourMap = new Color[width * height];

		for (int x = 0; x < width; x++) {
			for (int y = 0; y < height; y++) {
				float depth = verts [vertexIndex].y;

				uvs [vertexIndex] = new Vector2 (y / (float)height, x / (float)width);

				colourMap [vertexIndex] = getColourAtHeight (depth);//Color.Lerp (Color.gray, Color.blue, depth);

				if (x < (width - 1) && y < (height - 1)) {
					triangles [triangleIndex] = vertexIndex;
					triangles [triangleIndex + 1] = vertexIndex + width + 1;
					triangles [triangleIndex + 2] = vertexIndex + width;
					triangleIndex += 3;

					triangles [triangleIndex] = vertexIndex + width + 1;
					triangles [triangleIndex + 1] = vertexIndex;
					triangles [triangleIndex + 2] = vertexIndex + 1;
					triangleIndex += 3;
				}
				vertexIndex++;
			}
		}

		if (flatShading) {
			Vector3[] flatShadedVerts = new Vector3[triangles.Length];
			Vector2[] flatUVs = new Vector2[triangles.Length];

			for (int i = 0; i < triangles.Length; i++) {
				flatShadedVerts [i] = verts [triangles [i]];
				flatUVs [i] = uvs [triangles [i]];
				triangles [i] = i;
			}
			mesh.vertices = flatShadedVerts;
			mesh.uv = flatUVs;
		} else {
			mesh.vertices = verts;
			mesh.uv = uvs;
		}
		mesh.triangles = triangles;

		mesh.RecalculateNormals ();
		mesh.MarkDynamic ();

		texture.filterMode = FilterMode.Point;
		texture.wrapMode = TextureWrapMode.Clamp;
		texture.SetPixels (colourMap);
		texture.Apply();

		meshFilter.sharedMesh = mesh;
		//meshRenderer.sharedMaterial.mainTexture = texture;

	}


}
