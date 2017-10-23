using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
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

	public bool showGrid = true;
    public bool showDistances = true;
    public bool showFlowDirection = true;

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
		battlegroundRenderer = battlegroundParent.gameObject.GetComponent<MeshRenderer>();

        nodeDiameter = nodeRadius * 2;

        rect.min = battlegroundRenderer.bounds.min;
		rect.max = battlegroundRenderer.bounds.max;

        gridCountX = Mathf.RoundToInt(rect.size.x / nodeDiameter);
		gridCountY = Mathf.RoundToInt(rect.size.y / nodeDiameter);
        gridSize = gridCountX * gridCountY;
        CreateNodes();

        //groupCountX = Mathf.RoundToInt(rect.width / Vector2.one.x*10);
        //groupCountY = Mathf.RoundToInt(rect.height / Vector2.one.y*10);
        //groupSize = groupCountX * groupCountY;
        //CreateGroups();
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
                nodes[x, y] = new Node(projRect, projRect.center, x, y);
                projRect.yMin += diameter.x;
                projRect.yMax += diameter.x;
            }
            projRect.yMin -= diameter.y * gridCountY;
            projRect.yMax -= diameter.y * gridCountY;
            projRect.xMin += diameter.x;
            projRect.xMax += diameter.x;
        }
    }

    void CreateGroups()
    {
        groups = new NodeGroup[groupCountX, groupCountY];

		Rect projRect = new Rect(rect.min, Vector2.one*10);
		for (int x = 0; x < groupCountX; x++)
		{
			for (int y = 0; y < groupCountY; y++)
			{
				groups[x, y] = new NodeGroup(projRect);
				projRect.yMax += Vector2.one.y*10;
				projRect.yMin += Vector2.one.y*10;
			}
			projRect.yMax -= Vector2.one.y*10 * groupCountY;
			projRect.yMin -= Vector2.one.y*10 * groupCountY;
			projRect.xMax += Vector2.one.x*10;
			projRect.xMax += Vector2.one.x*10;
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

	public List<Node> FindNodesInRadius(Vector2 point, float radius)
	{
		List<Node> list = new List<Node>();
		Node node = NodeFromWorldPoint(point);

		int nodeCountInRadiusLine;
		if (radius % 1 != 0)
			nodeCountInRadiusLine = Mathf.FloorToInt(radius);
		else
			nodeCountInRadiusLine = Mathf.RoundToInt(radius)-1;

		for (int x = -nodeCountInRadiusLine; x <= nodeCountInRadiusLine; x++)
		{
			for (int y = -nodeCountInRadiusLine; y <= nodeCountInRadiusLine; y++)
			{
				if ((x == -nodeCountInRadiusLine && y == -nodeCountInRadiusLine)
					|| (x == -nodeCountInRadiusLine && y == nodeCountInRadiusLine)
					|| (x == nodeCountInRadiusLine && y == -nodeCountInRadiusLine)
					|| (x == nodeCountInRadiusLine && y == nodeCountInRadiusLine))
					continue;
				int checkX = node.gridX + x;
				int checkY = node.gridY + y;

				if (checkX >= 0 && checkX < gridCountX && checkY >= 0 && checkY < gridCountY)
				{
					list.Add(nodes[checkX, checkY]);
				}
			}
		}
		return list;
	}

	bool InBounds(int x, int y)
	{
		return x >= 0 && x < gridCountX && y >= 0 && y < gridCountY;
	}

    public Node[] GetNeighbours(Node node)
    {
		List<Node> neighbours = new List<Node>();

		for (int x = -1; x <= 1; x++) {
			for (int y = -1; y <= 1; y++) {
				if (x == 0 && y == 0)
					continue;

				int checkX = node.gridX + x;
				int checkY = node.gridY + y;

				if (checkX >= 0 && checkX < gridCountX && checkY >= 0 && checkY < gridCountY)
                {
					neighbours.Add(nodes[checkX,checkY]);
				}
			}
		}
		//Debug.Log(neighbours.Count);
        return neighbours.ToArray();
	}

	public Node NodeFromWorldPoint(Vector2 point)
    {
        //Debug.Log("wp: " + point);
        //Debug.Log("rect size x: " + rect.size.x + " rect size y: " + rect.size.y);
        float percentX = (point.x + rect.size.x / 2) / rect.size.x;
        float percentY = (point.y + rect.size.y / 2) / rect.size.y;
		//Debug.Log(percentX + " " + percentY);
		percentX = Mathf.Clamp01(percentX);
		percentY = Mathf.Clamp01(percentY);

		int x, y;

		//if (worldPosition.x < 0)
		x = Mathf.RoundToInt((gridCountX - 1) * percentX);
		y = Mathf.RoundToInt((gridCountY - 1) * percentY);
        //Debug.Log("nwp: " + nodes[x,y].worldPosition);
		return nodes[x,y];
	}

	public Node DefineNextNode(Node _node, int _playerNumber)
	{
        bool found = false;
		int closestNodeIndex = 0;
		for (int i = 0; i < _node.neigbours.Length; i++)
		{
			if (_node.neigbours[i] != null)
			{
                if (_node.neigbours[i].distance[_playerNumber] < _node.neigbours[closestNodeIndex].distance[_playerNumber]
                    && _node.neigbours[i].walkable)
				{
					closestNodeIndex = i;
                    found = true;
				}
			}
		}
        if (found || closestNodeIndex == 0)
            return _node.neigbours[closestNodeIndex];
        else
            return null;
	}

	public Node DefineNextNode(Node _node, List<Node> _currentPathNodes, int _playerNumber)
	{
		bool found = false;
		int closestNodeIndex = 0;
		for (int i = 0; i < _node.neigbours.Length; i++)
		{
			if (_node.neigbours[i] != null)
			{
				if (_node.neigbours[i].distance[_playerNumber] < _node.neigbours[closestNodeIndex].distance[_playerNumber]
                    && _node.neigbours[i].walkable
                    && !_currentPathNodes.Contains(_node.neigbours[i]))
				{
					closestNodeIndex = i;
					found = true;
				}
			}
		}
		if (found || closestNodeIndex == 0)
			return _node.neigbours[closestNodeIndex];
		else
			return null;
	}

	void OnDrawGizmos()
    {
		if (nodes != null)
		{
            Color newColor;
			if (showGrid)
			{
                foreach (Node n in nodes)
                {
                    Gizmos.color = Color.Lerp(Color.white, Color.black, Mathf.InverseLerp(penaltyMin, penaltyMax, n.movementPenalty));
                    Gizmos.color = (n.walkable) ? Gizmos.color : Color.red;
                    newColor = Gizmos.color;
                    newColor.a = 0.5f;
                    Gizmos.color = newColor;
                    Gizmos.DrawCube(n.worldPosition, Vector3.one * (1 - 0.02f));
                    if (showDistances)
                    {
#if UNITY_EDITOR
                        Handles.Label(n.worldPosition, n.distance[0].ToString());
#endif
                    }
                    if (showFlowDirection)
                    {
                        newColor = Color.white;
						newColor.a = 0.7f;
						Gizmos.color = newColor;
                        Gizmos.DrawLine(n.worldPosition, n.flowVector[0]);
                    }
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
