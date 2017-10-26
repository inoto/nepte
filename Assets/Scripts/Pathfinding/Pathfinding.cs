using UnityEngine;
using System.Collections.Generic;
using System.Diagnostics;
using System;

public class Pathfinding : MonoBehaviour
{
	private static Pathfinding _instance;

	public static Pathfinding Instance { get { return _instance; } }

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

        grid = GetComponent<Grid>();
	}

	private Grid grid;

	public static void FillDistances(Vector2 startPoint, int player)
	{
        Node startNode = Grid.Instance.NodeFromWorldPoint(startPoint);
		Vector2 startNodePoint = startNode.WorldPosition;

		startNode.Distance[player] = 0;
        for (int x = 0; x < Grid.Instance.GridCountX; x++)
        {
            for (int y = 0; y < Grid.Instance.GridCountY; y++)
            {
                Grid.Instance.Nodes[x, y].Visited[player] = false;
            }
        }

		Queue<Node> open = new Queue<Node>();

		Node currentNode;

		open.Enqueue(startNode);
        startNode.Visited[player] = true;

        int count = Grid.Instance.GridSize - 1;
		while (count > 0)
		{
			currentNode = open.Dequeue();

			if (!currentNode.IsNeigboursFilled)
			{
                currentNode.Neigbours = Grid.Instance.GetNeighbours(currentNode);
				currentNode.IsNeigboursFilled = true;
			}

			//foreach (Node nextNode in neigbours)
            for (int i = 0; i < currentNode.Neigbours.Length-1; i++)
			{
                if (currentNode.Neigbours[i] == null || currentNode.Neigbours[i].Visited[player])
                {
	                continue;
                }
				currentNode.Neigbours[i].Visited[player] = true;
				open.Enqueue(currentNode.Neigbours[i]);

                currentNode.Neigbours[i].Distance[player] = (int)(currentNode.Neigbours[i].WorldPosition - startNodePoint).sqrMagnitude;
                currentNode.Neigbours[i].FlowVector[player] = (currentNode.Neigbours[i].WorldPosition - currentNode.WorldPosition).normalized;
            }
            count -= 1;
		}
	}

	public void FindPath(PathRequest request, Action<PathResult> callback)
	{

		Stopwatch sw = new Stopwatch();
		sw.Start();

		Vector2[] waypoints = new Vector2[0];
		bool pathSuccess = false;

		Node startNode = grid.NodeFromWorldPoint(request.pathStart);
		Node targetNode = grid.NodeFromWorldPoint(request.pathEnd);
		startNode.Parent = startNode;

		if (!startNode.IsWalkable)
		{
			startNode = grid.ClosestWalkableNode(startNode);
		}
		if (!targetNode.IsWalkable)
		{
			targetNode = grid.ClosestWalkableNode(targetNode);
		}

		if (startNode.IsWalkable && targetNode.IsWalkable)
		{
			Heap<Node> openSet = new Heap<Node>(grid.GridSize);
			HashSet<Node> closedSet = new HashSet<Node>();
			openSet.Add(startNode);

			while (openSet.Count > 0)
			{
				Node currentNode = openSet.RemoveFirst();
				closedSet.Add(currentNode);

				if (!currentNode.IsWalkable)
				{
					currentNode = grid.ClosestWalkableNode(currentNode);
				}

				if (currentNode == targetNode)
				{
					sw.Stop();
					//print ("Path found: " + sw.ElapsedMilliseconds + " ms");
					pathSuccess = true;
					break;
				}

				foreach (Node neighbour in grid.GetNeighbours(currentNode))
				{
					if (!neighbour.IsWalkable || closedSet.Contains(neighbour))
					{
						continue;
					}

					int newMovementCostToNeighbour = currentNode.GCost + GetDistance(currentNode, neighbour);
					if (newMovementCostToNeighbour < neighbour.GCost || !openSet.Contains(neighbour))
					{
						neighbour.GCost = newMovementCostToNeighbour;
						neighbour.HCost = GetDistance(neighbour, targetNode);
						neighbour.Parent = currentNode;

						if (!openSet.Contains(neighbour))
						{
							openSet.Add(neighbour);
						}
						else
						{
							openSet.UpdateItem(neighbour);
						}
					}
				}
			}
		}
		if (pathSuccess)
		{
			waypoints = RetracePath(startNode, targetNode);
			pathSuccess = waypoints.Length > 0;
		}
		callback(new PathResult(waypoints, pathSuccess, request.callback));

	}


	private static Vector2[] RetracePath(Node startNode, Node endNode)
	{
		List<Node> path = new List<Node>();
		Node currentNode = endNode;

		while (currentNode != startNode)
		{
			path.Add(currentNode);
			currentNode = currentNode.Parent;
		}
        Vector2[] waypoints = SimplifyPath(path);
		Array.Reverse(waypoints);
		return waypoints;
	}

	private static Vector2[] SimplifyPath(List<Node> path)
	{
		List<Vector2> waypoints = new List<Vector2>();
		Vector2 directionOld = Vector2.zero;

		for (int i = 1; i < path.Count; i++)
		{
			Vector2 directionNew = new Vector2(path[i - 1].GridX - path[i].GridX, path[i - 1].GridY - path[i].GridY);
			if (directionNew != directionOld)
			{
				waypoints.Add(path[i].WorldPosition);
			}
			directionOld = directionNew;
		}
		return waypoints.ToArray();
	}

	private static int GetDistance(Node nodeA, Node nodeB)
	{
		int dstX = Mathf.Abs(nodeA.GridX - nodeB.GridX);
		int dstY = Mathf.Abs(nodeA.GridY - nodeB.GridY);

		if (dstX > dstY)
		{
			return 14 * dstY + 10 * (dstX - dstY);
		}
		return 14 * dstX + 10 * (dstY - dstX);
	}


}
