using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Grid : MonoBehaviour {

	private static Grid _instance;

	public static Grid Instance { get { return _instance; } }

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

	public bool displayGridGizmos = true;
    public LayerMask unwalkableMask;
    
	public TerrainType[] walkableRegions;
	public int obstacleProximityPenalty = 10;
	Dictionary<int,int> walkableRegionsDictionary = new Dictionary<int, int>();
	LayerMask walkableMask;

    private Battleground battlegroundParent;
    private Renderer battlegroundRenderer;
    Rect rect;

    public Node[,] nodes;
    public float nodeRadius;
    public float nodeDiameter;

    public NodeGroup[,] groups;
    public float groupRadius;
    public float groupDiameter;

    public int gridCountX, gridCountY;
    public int gridSize;

    public int groupCountX, groupCountY;
    public int groupSize;

    int penaltyMin = int.MaxValue;
	int penaltyMax = int.MinValue;

	void Start()
    {
		battlegroundParent = transform.parent.gameObject.GetComponent<Battleground>();
		battlegroundRenderer = battlegroundParent.gameObject.GetComponent<SpriteRenderer>();

        nodeRadius = 0.05f;
        nodeDiameter = nodeRadius * 2;

        rect.min = battlegroundRenderer.bounds.min;
		rect.max = battlegroundRenderer.bounds.max;

        gridCountX = Mathf.RoundToInt(rect.width / Vector2.one.x * 10);
		gridCountY = Mathf.RoundToInt(rect.height / Vector2.one.y * 10);
        gridSize = gridCountX * gridCountY;
        CreateNodes();

        groupCountX = Mathf.RoundToInt(rect.width / Vector2.one.x);
        groupCountY = Mathf.RoundToInt(rect.height / Vector2.one.y);
        groupSize = groupCountX * groupCountY;
        CreateGroups();
    }

    public List<NodeGroup> GetGroupsAsList()
	{
		List<NodeGroup> list = new List<NodeGroup>();
		for (int x = 0; x < gridCountX; x++)
		{
			for (int y = 0; y < gridCountY; y++)
			{
				list.Add(groups[x, y]);
			}
		}
		return list;
	}

    void CreateNodes()
    {
        nodes = new Node[gridCountX, gridCountY];

        Vector2 diameter = new Vector2(nodeDiameter, nodeDiameter);

        Rect projRect = new Rect(rect.min, diameter);
        for (int x = 0; x < gridCountX; x++)
        {
            for (int y = 0; y < gridCountY; y++)
            {
                nodes[x, y] = new Node(projRect, diameter.x, x, y);
                projRect.yMax += diameter.x;
                projRect.yMin += diameter.x;
            }
            projRect.yMax -= diameter.y * gridCountY;
            projRect.yMin -= diameter.y * gridCountY;
            projRect.xMax += diameter.x;
            projRect.xMax += diameter.x;
        }
    }

    void CreateGroups()
    {
        groups = new NodeGroup[groupCountX, groupCountY];

		Rect projRect = new Rect(rect.min, Vector2.one);
		for (int x = 0; x < groupCountX; x++)
		{
			for (int y = 0; y < groupCountY; y++)
			{
				groups[x, y] = new NodeGroup(projRect);
				projRect.yMax += Vector2.one.y;
				projRect.yMin += Vector2.one.y;
			}
			projRect.yMax -= Vector2.one.y * groupCountY;
			projRect.yMin -= Vector2.one.y * groupCountY;
			projRect.xMax += Vector2.one.x;
			projRect.xMax += Vector2.one.x;
		}

		
	}

	public Node ClosestWalkableNode(Node node)
	{
		int maxRadius = Mathf.Max(gridCountX, gridCountY) / 2;
		for (int i = 1; i < maxRadius; i++)
		{
			Node n = FindWalkableInRadius(node.gridX, node.gridY, i);
			if (n != null)
			{
				return n;

			}
		}
		return null;
	}

	Node FindWalkableInRadius(int centreX, int centreY, int radius)
	{

		for (int i = -radius; i <= radius; i++)
		{
			int verticalSearchX = i + centreX;
			int horizontalSearchY = i + centreY;

			// top
			if (InBounds(verticalSearchX, centreY + radius))
			{
				if (nodes[verticalSearchX, centreY + radius].walkable)
				{
					return nodes[verticalSearchX, centreY + radius];
				}
			}

			// bottom
			if (InBounds(verticalSearchX, centreY - radius))
			{
				if (nodes[verticalSearchX, centreY - radius].walkable)
				{
					return nodes[verticalSearchX, centreY - radius];
				}
			}
			// right
			if (InBounds(centreY + radius, horizontalSearchY))
			{
				if (nodes[centreX + radius, horizontalSearchY].walkable)
				{
					return nodes[centreX + radius, horizontalSearchY];
				}
			}

			// left
			if (InBounds(centreY - radius, horizontalSearchY))
			{
				if (nodes[centreX - radius, horizontalSearchY].walkable)
				{
					return nodes[centreX - radius, horizontalSearchY];
				}
			}

		}

		return null;

	}

	bool InBounds(int x, int y)
	{
		return x >= 0 && x < gridCountX && y >= 0 && y < gridCountY;
	}

	public List<Node> GetNeighbours(Node node)
    {
		List<Node> neighbours = new List<Node>();

		for (int x = -1; x <= 1; x++) {
			for (int y = -1; y <= 1; y++) {
				if (x == 0 && y == 0)
					continue;

				int checkX = node.gridX + x;
				int checkY = node.gridY + y;

				if (checkX >= 0 && checkX < gridCountX && checkY >= 0 && checkY < gridCountY) {
					neighbours.Add(nodes[checkX,checkY]);
				}
			}
		}

		return neighbours;
	}

	public Node NodeFromWorldPoint(Vector2 worldPosition)
    {
		float percentX = (worldPosition.x + rect.size.x/2) / rect.size.x;
		float percentY = (worldPosition.y + rect.size.y / 2) / rect.size.y;
		percentX = Mathf.Clamp01(percentX);
		percentY = Mathf.Clamp01(percentY);

		int x = Mathf.RoundToInt((gridCountX-1) * percentX);
		int y = Mathf.RoundToInt((gridCountY-1) * percentY);
		return nodes[x,y];
	}

	void OnDrawGizmos()
    {
        if (displayGridGizmos)
        {
            Color newColor;
            if (nodes != null)
            {
                foreach (Node n in nodes)
                {
                    Gizmos.color = Color.Lerp(Color.white, Color.black, Mathf.InverseLerp(penaltyMin, penaltyMax, n.movementPenalty));
                    Gizmos.color = (n.walkable) ? Gizmos.color : Color.red;
                    newColor = Gizmos.color;
                    newColor.a = 0.5f;
                    Gizmos.color = newColor;
                    Gizmos.DrawCube(n.worldPosition, Vector3.one * (0.1f - 0.01f));
                }
            }
            if (groups != null)
            {
                foreach (NodeGroup ng in groups)
                {
                    newColor = Color.blue;
                    newColor.a = 0.5f;
                    Gizmos.color = newColor;
                    Gizmos.DrawWireCube(ng.rect.center, Vector3.one);
                }
            }
        }
	}

	[System.Serializable]
	public class TerrainType {
		public LayerMask terrainMask;
		public int terrainPenalty;
	}
}
