using System;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
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

	public bool ShowGrid = true;
    public bool ShowDistances = true;
    public bool ShowFlowDirection = true;

    private Battleground battlegroundParent;
    private Renderer battlegroundRenderer;
	private Rect rect;

    public Node[,] Nodes;
    public float NodeRadius;
    public float NodeDiameter;

    public NodeGroup[,] Groups;
    public float GroupRadius;
    public float GroupDiameter;

    public int GridCountX, GridCountY;
    public int GridSize;

    public int GroupCountX, GroupCountY;
    public int GroupSize;

	private const int penaltyMin = int.MaxValue;
	private const int penaltyMax = int.MinValue;

	private void Start()
    {
		battlegroundParent = transform.parent.gameObject.GetComponent<Battleground>();
		battlegroundRenderer = battlegroundParent.gameObject.GetComponent<MeshRenderer>();

        NodeDiameter = NodeRadius * 2;

        rect.min = battlegroundRenderer.bounds.min;
		rect.max = battlegroundRenderer.bounds.max;

        GridCountX = Mathf.RoundToInt(rect.size.x / NodeDiameter);
		GridCountY = Mathf.RoundToInt(rect.size.y / NodeDiameter);
        GridSize = GridCountX * GridCountY;
        CreateNodes();

        //groupCountX = Mathf.RoundToInt(rect.width / Vector2.one.x*10);
        //groupCountY = Mathf.RoundToInt(rect.height / Vector2.one.y*10);
        //groupSize = groupCountX * groupCountY;
        //CreateGroups();
    }

	[Obsolete("Not used anymore",true)]
    public List<NodeGroup> GetGroupsAsList()
	{
		List<NodeGroup> list = new List<NodeGroup>();
		for (int x = 0; x < GridCountX; x++)
		{
			for (int y = 0; y < GridCountY; y++)
			{
				list.Add(Groups[x, y]);
			}
		}
		return list;
	}

	private void CreateNodes()
    {
        Nodes = new Node[GridCountX, GridCountY];

        Vector2 diameter = new Vector2(NodeDiameter, NodeDiameter);

        Rect projRect = new Rect(rect.min, diameter);
        for (int x = 0; x < GridCountX; x++)
        {
            for (int y = 0; y < GridCountY; y++)
            {
                Nodes[x, y] = new Node(projRect, projRect.center, x, y);
                projRect.yMin += diameter.x;
                projRect.yMax += diameter.x;
            }
            projRect.yMin -= diameter.y * GridCountY;
            projRect.yMax -= diameter.y * GridCountY;
            projRect.xMin += diameter.x;
            projRect.xMax += diameter.x;
        }
    }

	[Obsolete("Not used anymore",true)]
    private void CreateGroups()
    {
        Groups = new NodeGroup[GroupCountX, GroupCountY];

		Rect projRect = new Rect(rect.min, Vector2.one*10);
		for (int x = 0; x < GroupCountX; x++)
		{
			for (int y = 0; y < GroupCountY; y++)
			{
				Groups[x, y] = new NodeGroup(projRect);
				projRect.yMax += Vector2.one.y*10;
				projRect.yMin += Vector2.one.y*10;
			}
			projRect.yMax -= Vector2.one.y*10 * GroupCountY;
			projRect.yMin -= Vector2.one.y*10 * GroupCountY;
			projRect.xMax += Vector2.one.x*10;
			projRect.xMax += Vector2.one.x*10;
		}

		
	}

	public Node ClosestWalkableNode(Node node)
	{
		int maxRadius = Mathf.Max(GridCountX, GridCountY) / 2;
		for (int i = 1; i < maxRadius; i++)
		{
			Node n = FindWalkableInRadius(node.GridX, node.GridY, i);
			if (n != null)
			{
				return n;
			}
		}
		return null;
	}

	private Node FindWalkableInRadius(int centreX, int centreY, int radius)
	{

		for (int i = -radius; i <= radius; i++)
		{
			int verticalSearchX = i + centreX;
			int horizontalSearchY = i + centreY;

			// top
			if (InBounds(verticalSearchX, centreY + radius))
			{
				if (Nodes[verticalSearchX, centreY + radius].IsWalkable)
				{
					return Nodes[verticalSearchX, centreY + radius];
				}
			}

			// bottom
			if (InBounds(verticalSearchX, centreY - radius))
			{
				if (Nodes[verticalSearchX, centreY - radius].IsWalkable)
				{
					return Nodes[verticalSearchX, centreY - radius];
				}
			}
			// right
			if (InBounds(centreY + radius, horizontalSearchY))
			{
				if (Nodes[centreX + radius, horizontalSearchY].IsWalkable)
				{
					return Nodes[centreX + radius, horizontalSearchY];
				}
			}

			// left
			if (InBounds(centreY - radius, horizontalSearchY))
			{
				if (Nodes[centreX - radius, horizontalSearchY].IsWalkable)
				{
					return Nodes[centreX - radius, horizontalSearchY];
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
		{
			nodeCountInRadiusLine = Mathf.FloorToInt(radius);
		}
		else
		{
			nodeCountInRadiusLine = Mathf.RoundToInt(radius)-1;
		}

		for (int x = -nodeCountInRadiusLine; x <= nodeCountInRadiusLine; x++)
		{
			for (int y = -nodeCountInRadiusLine; y <= nodeCountInRadiusLine; y++)
			{
				if ((x == -nodeCountInRadiusLine && y == -nodeCountInRadiusLine)
					|| (x == -nodeCountInRadiusLine && y == nodeCountInRadiusLine)
					|| (x == nodeCountInRadiusLine && y == -nodeCountInRadiusLine)
					|| (x == nodeCountInRadiusLine && y == nodeCountInRadiusLine))
				{
					continue;
				}
				int checkX = node.GridX + x;
				int checkY = node.GridY + y;

				if (checkX >= 0 && checkX < GridCountX && checkY >= 0 && checkY < GridCountY)
				{
					list.Add(Nodes[checkX, checkY]);
				}
			}
		}
		return list;
	}

	private bool InBounds(int x, int y)
	{
		return x >= 0 && x < GridCountX && y >= 0 && y < GridCountY;
	}

    public Node[] GetNeighbours(Node node)
    {
		List<Node> neighbours = new List<Node>();

		for (int x = -1; x <= 1; x++) {
			for (int y = -1; y <= 1; y++) {
				if (x == 0 && y == 0)
				{
					continue;
				}

				int checkX = node.GridX + x;
				int checkY = node.GridY + y;

				if (checkX >= 0 && checkX < GridCountX && checkY >= 0 && checkY < GridCountY)
                {
					neighbours.Add(Nodes[checkX,checkY]);
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
		x = Mathf.RoundToInt((GridCountX - 1) * percentX);
		y = Mathf.RoundToInt((GridCountY - 1) * percentY);
        //Debug.Log("nwp: " + nodes[x,y].worldPosition);
		return Nodes[x,y];
	}

	public static Node DefineNextNode(Node node, int playerNumber)
	{
        bool found = false;
		int closestNodeIndex = 0;
		for (int i = 0; i < node.Neigbours.Length; i++)
		{
			if (node.Neigbours[i] != null)
			{
                if (node.Neigbours[i].Distance[playerNumber] < node.Neigbours[closestNodeIndex].Distance[playerNumber]
                    && node.Neigbours[i].IsWalkable)
				{
					closestNodeIndex = i;
                    found = true;
				}
			}
		}
        if (found || closestNodeIndex == 0)
        {
	        return node.Neigbours[closestNodeIndex];
        }
        else
        {
	        return null;
        }
	}

	public Node DefineNextNode(Node node, List<Node> currentPathNodes, int playerNumber)
	{
		bool found = false;
		int closestNodeIndex = 0;
		for (int i = 0; i < node.Neigbours.Length; i++)
		{
			if (node.Neigbours[i] != null)
			{
				if (node.Neigbours[i].Distance[playerNumber] < node.Neigbours[closestNodeIndex].Distance[playerNumber]
                    && node.Neigbours[i].IsWalkable
                    && !currentPathNodes.Contains(node.Neigbours[i]))
				{
					closestNodeIndex = i;
					found = true;
				}
			}
		}
		if (found || closestNodeIndex == 0)
		{
			return node.Neigbours[closestNodeIndex];
		}
		else
		{
			return null;
		}
	}

	private void OnDrawGizmos()
    {
		if (Nodes != null)
		{
            Color newColor;
			if (ShowGrid)
			{
                foreach (Node n in Nodes)
                {
                    Gizmos.color = (n.IsWalkable) ? Gizmos.color : Color.red;
                    newColor = Gizmos.color;
                    newColor.a = 0.5f;
                    Gizmos.color = newColor;
                    Gizmos.DrawCube(n.WorldPosition, Vector3.one * (1 - 0.02f));
                    if (ShowDistances)
                    {
#if UNITY_EDITOR
                        Handles.Label(n.WorldPosition, n.Distance[0].ToString());
#endif
                    }
                    if (ShowFlowDirection)
                    {
                        newColor = Color.white;
						newColor.a = 0.7f;
						Gizmos.color = newColor;
                        Gizmos.DrawLine(n.WorldPosition, n.FlowVector[0]);
                    }
                }
            }
        }
	}

	[System.Serializable]
	public class TerrainType {
		public LayerMask TerrainMask;
		public int TerrainPenalty;
	}
}
