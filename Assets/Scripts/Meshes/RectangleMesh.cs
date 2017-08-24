using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RectangleMesh : MonoBehaviour
{

    public Vector2 size = Vector2.one;

	MeshFilter mFilter;

	void Awake()
	{

		mFilter = GetComponent<MeshFilter>();
	}

	private void Start()
	{
		CreateMesh();
	}

	void CreateMesh()
	{
		Mesh mesh = new Mesh();
		mesh.vertices = new Vector3[]
		{
			new Vector3( size.x, size.y, 0),
			new Vector3( size.x, -size.y, 0),
			new Vector3(-size.x, size.y, 0),
			new Vector3(-size.x, -size.y, 0),
		};
		mesh.uv = new Vector2[]
		{
			new Vector2(1, 1),
			new Vector2(1, 0),
			new Vector2(0, 1),
			new Vector2(0, 0),
		};
		mesh.triangles = new int[] { 0, 1, 2, 2, 1, 3, };

		mFilter.sharedMesh = mesh;
	}
}