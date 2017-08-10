using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*public class CollisionGrid : MonoBehaviour
{
	private static CollisionGrid _instance;

	public static CollisionGrid Instance { get { return _instance; } }

	private void Awake()
	{
		if (_instance != null && _instance != this)
		{
			Destroy(this.gameObject);
		}
		else
		{
			_instance = this;
		}
	}

    Vector2 nodeSize = Vector2.one;

    public Rect rect;

    public int gridCountX, gridCountY;
    public int gridSize;

    public CollisionNode[,] nodes;

	// Use this for initialization
	void Start ()
    {
        Renderer bgRenderer = GameObject.Find("Battleground").GetComponent<SpriteRenderer>();
        rect.min = bgRenderer.bounds.min;
        rect.max = bgRenderer.bounds.max;
        Vector2 boundsSize = bgRenderer.bounds.size;

		gridCountX = Mathf.RoundToInt(boundsSize.x / 1.0f);
		gridCountY = Mathf.RoundToInt(boundsSize.y / 1.0f);
        gridSize = gridCountX * gridCountY;

        CreateGrid();
	}

    void CreateGrid()
    {
        nodes = new CollisionNode[gridCountX, gridCountY];

        int nodeCounter = 0;
        Rect projRect = new Rect(rect.min, Vector2.one);
        for (int x = 0; x < gridCountY; x++)
        {
            for (int y = 0; y < gridCountX; y++)
            {
                nodes[x, y] = new CollisionNode(projRect, this);
                projRect.xMax += Vector2.one.x;
				projRect.xMin += Vector2.one.x;
            }
			projRect.xMax -= Vector2.one.x * gridCountX;
			projRect.xMin -= Vector2.one.x * gridCountX;
			projRect.yMax += Vector2.one.y;
			projRect.yMax += Vector2.one.y;
        }
        CollisionManager.Instance.AddAllNodes(GetAllNodesAsList());
    }

    Get

    void CheckCollisionGrid()
    {
        for (int x = 0; x < gridCountY; x++)
        {
            for (int y = 0; y < gridCountX; y++)
            {
                nodes[x,y].CheckCollisions();
            }
        }
    }

	public CollisionNode NodeFromWorldPoint(Vector2 worldPosition)
	{
		float percentX = (worldPosition.x + gridCountX / 2) / gridCountX;
		float percentY = (worldPosition.y + gridCountY / 2) / gridCountY;
		percentX = Mathf.Clamp01(percentX);
		percentY = Mathf.Clamp01(percentY);

		int x = Mathf.RoundToInt((gridCountX - 1) * percentX);
		int y = Mathf.RoundToInt((gridCountY - 1) * percentY);
		return nodes[x, y];
	}

	void OnDrawGizmos()
	{
        if (nodes != null)
        {
            foreach (CollisionNode cn in nodes)
            {
                Color newColor = Color.blue;
                newColor.a = 0.5f;
                Gizmos.color = newColor;
                Gizmos.DrawWireCube(cn.rect.center, Vector3.one);
            }
        }
	}
}




    */