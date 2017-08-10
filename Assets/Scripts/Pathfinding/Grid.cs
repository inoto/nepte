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
	public float nodeRadius;
    public Vector2 gridSize;
	public TerrainType[] walkableRegions;
	public int obstacleProximityPenalty = 10;
	Dictionary<int,int> walkableRegionsDictionary = new Dictionary<int, int>();
	LayerMask walkableMask;


    private Battleground battlegroundParent;
    private Renderer battlegroundRenderer;
    Vector3 penVector;

	
    public NodeGroup[,] groups;

	public static float nodeDiameter;
	public int gridSizeX, gridSizeY;

	int penaltyMin = int.MaxValue;
	int penaltyMax = int.MinValue;

	void Start() {
		battlegroundParent = transform.parent.gameObject.GetComponent<Battleground>();
		battlegroundRenderer = battlegroundParent.gameObject.GetComponent<SpriteRenderer>();
        gridSize = battlegroundRenderer.bounds.size;

        penVector = battlegroundRenderer.bounds.min;
		penVector.x += 0.05f;
		penVector.y += 0.05f;

		nodeDiameter = nodeRadius*2;
		gridSizeX = Mathf.RoundToInt(gridSize.x/nodeDiameter);
		gridSizeY = Mathf.RoundToInt(gridSize.y/nodeDiameter);

		foreach (TerrainType region in walkableRegions) {
			walkableMask.value |= region.terrainMask.value;
			walkableRegionsDictionary.Add((int)Mathf.Log(region.terrainMask.value,2),region.terrainPenalty);
		}

		CreateGrid();
	}

	public int MaxSize {
		get {
			return gridSizeX * gridSizeY;
		}
	}

    public List<NodeGroup> GetGroupsAsList()
	{
		List<NodeGroup> list = new List<NodeGroup>();
		for (int x = 0; x < gridSizeX; x++)
		{
			for (int y = 0; y < gridSizeY; y++)
			{
				list.Add(groups[x, y]);
			}
		}
		return list;
	}

	void CreateGrid()
    {

        groups = new NodeGroup[gridSizeX, gridSizeY];

		int nodeCounter = 0;
		Rect projRect = new Rect(rect.min, Vector2.one);
		for (int x = 0; x < gridSizeY; x++)
		{
			for (int y = 0; y < gridSizeX; y++)
			{
				nodes[x, y] = new CollisionNode(projRect, this);
				projRect.xMax += Vector2.one.x;
				projRect.xMin += Vector2.one.x;
			}
			projRect.xMax -= Vector2.one.x * gridSizeX;
			projRect.xMin -= Vector2.one.x * gridSizeX;
			projRect.yMax += Vector2.one.y;
			projRect.yMax += Vector2.one.y;
		}

		
	}

	public Node ClosestWalkableNode(Node node)
	{
		int maxRadius = Mathf.Max(gridSizeX, gridSizeY) / 2;
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

    public Node ClosestToDestinationWalkableNode(Node node, Vector2 destination)
	{
		int maxRadius = Mathf.Max(gridSizeX, gridSizeY) / 2;
		for (int i = 1; i < maxRadius; i++)
		{
            Node n = FindWalkableToDestination(node, i, destination);
            if (n != null)
			{
				return n;
			}
		}
		return null;
	}

    Node FindWalkableToDestination(Node start, int radius, Vector2 destination)
	{
        Node closest = start;
        Node tmp = null;

        int centreX = start.gridX;
        int centreY = start.gridY;

		for (int i = -radius; i <= radius; i++)
		{
			int verticalSearchX = i + centreX;
			int horizontalSearchY = i + centreY;

			// top
			if (InBounds(verticalSearchX, centreY + radius))
			{
				if (grid[verticalSearchX, centreY + radius].walkable)
				{
                    tmp = grid[verticalSearchX, centreY + radius];
                    if (Vector2.Distance(closest.worldPosition, destination) < Vector2.Distance(tmp.worldPosition, destination))
                    {
                        Debug.Log("has closest");
                        closest = tmp;
                    }
				}
			}

			// bottom
			if (InBounds(verticalSearchX, centreY - radius))
			{
				if (grid[verticalSearchX, centreY - radius].walkable)
				{
                    tmp = grid[verticalSearchX, centreY - radius];
					if (Vector2.Distance(closest.worldPosition, destination) < Vector2.Distance(tmp.worldPosition, destination))
					{
                        Debug.Log("has closest");
						closest = tmp;
					}
				}
			}
			// right
			if (InBounds(centreY + radius, horizontalSearchY))
			{
				if (grid[centreX + radius, horizontalSearchY].walkable)
				{
					tmp = grid[centreX + radius, horizontalSearchY];
					if (Vector2.Distance(closest.worldPosition, destination) < Vector2.Distance(tmp.worldPosition, destination))
					{
                        Debug.Log("has closest");
						closest = tmp;
					}
				}
			}

			// left
			if (InBounds(centreY - radius, horizontalSearchY))
			{
				if (grid[centreX - radius, horizontalSearchY].walkable)
				{
					tmp = grid[centreX - radius, horizontalSearchY];
					if (Vector2.Distance(closest.worldPosition, destination) < Vector2.Distance(tmp.worldPosition, destination))
					{
                        Debug.Log("has closest");
						closest = tmp;
					}
				}
			}

		}

        return closest;
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
				if (grid[verticalSearchX, centreY + radius].walkable)
				{
					return grid[verticalSearchX, centreY + radius];
				}
			}

			// bottom
			if (InBounds(verticalSearchX, centreY - radius))
			{
				if (grid[verticalSearchX, centreY - radius].walkable)
				{
					return grid[verticalSearchX, centreY - radius];
				}
			}
			// right
			if (InBounds(centreY + radius, horizontalSearchY))
			{
				if (grid[centreX + radius, horizontalSearchY].walkable)
				{
					return grid[centreX + radius, horizontalSearchY];
				}
			}

			// left
			if (InBounds(centreY - radius, horizontalSearchY))
			{
				if (grid[centreX - radius, horizontalSearchY].walkable)
				{
					return grid[centreX - radius, horizontalSearchY];
				}
			}

		}

		return null;

	}

	bool InBounds(int x, int y)
	{
		return x >= 0 && x < gridSizeX && y >= 0 && y < gridSizeY;
	}

	public List<Node> GetNeighbours(Node node) {
		List<Node> neighbours = new List<Node>();

		for (int x = -1; x <= 1; x++) {
			for (int y = -1; y <= 1; y++) {
				if (x == 0 && y == 0)
					continue;

				int checkX = node.gridX + x;
				int checkY = node.gridY + y;

				if (checkX >= 0 && checkX < gridSizeX && checkY >= 0 && checkY < gridSizeY) {
					neighbours.Add(grid[checkX,checkY]);
				}
			}
		}

		return neighbours;
	}


	public Node NodeFromWorldPoint(Vector2 worldPosition) {
		float percentX = (worldPosition.x + gridSize.x/2) / gridSize.x;
		float percentY = (worldPosition.y + gridSize.y/2) / gridSize.y;
		percentX = Mathf.Clamp01(percentX);
		percentY = Mathf.Clamp01(percentY);

		int x = Mathf.RoundToInt((gridSizeX-1) * percentX);
		int y = Mathf.RoundToInt((gridSizeY-1) * percentY);
		return grid[x,y];
	}

	void OnDrawGizmos() {
		//Gizmos.DrawWireCube(transform.position,new Vector3(gridSize.x,gridSize.y,1));
		if (grid != null && displayGridGizmos) {
			foreach (Node n in grid) {

				Gizmos.color = Color.Lerp (Color.white, Color.black, Mathf.InverseLerp (penaltyMin, penaltyMax, n.movementPenalty));
				Gizmos.color = (n.walkable)?Gizmos.color:Color.red;
                Color newColor = Gizmos.color;
                newColor.a = 0.5f;
                Gizmos.color = newColor;
				Gizmos.DrawCube(n.worldPosition, Vector3.one * (nodeDiameter - 0.01f));
			}
		}
	}

	[System.Serializable]
	public class TerrainType {
		public LayerMask terrainMask;
		public int terrainPenalty;
	}
}
