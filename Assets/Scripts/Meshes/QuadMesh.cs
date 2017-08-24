using UnityEngine;

public class QuadMesh : MonoBehaviour {

    public float size = 1.0f;

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
			new Vector3( size, size, 0),
			new Vector3( size, -size, 0),
			new Vector3(-size, size, 0),
			new Vector3(-size, -size, 0),
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
