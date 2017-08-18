using UnityEngine;
using System.Collections;
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

	Grid grid;

	public void FillDistances(Vector2 startPoint, int player)
	{
        Node startNode = Grid.Instance.NodeFromWorldPoint(startPoint);
		Vector2 startNodePoint = startNode.worldPosition;

		startNode.distance[player] = 0;
        for (int x = 0; x < Grid.Instance.gridCountX; x++)
        {
            for (int y = 0; y < Grid.Instance.gridCountY; y++)
            {
                Grid.Instance.nodes[x, y].visited[player] = false;
            }
        }

		Queue<Node> open = new Queue<Node>();

		Node currentNode;

		open.Enqueue(startNode);
        startNode.visited[player] = true;
        //visitedSet.Add(startNode);

        int count = Grid.Instance.gridSize - 1;
		while (count > 0)
		{
            //Node[] neigbours = new Node[8];

			currentNode = open.Dequeue();

			if (!currentNode.isNeigboursFilled)
			{
                currentNode.neigbours = Grid.Instance.GetNeighbours(currentNode);
				//currentNode.neigbours = neigbours;
				currentNode.isNeigboursFilled = true;
			}

			//foreach (Node nextNode in neigbours)
            for (int i = 0; i < currentNode.neigbours.Length-1; i++)
			{
                if (currentNode.neigbours[i] == null || currentNode.neigbours[i].visited[player])
					continue;
                currentNode.neigbours[i].visited[player] = true;
				open.Enqueue(currentNode.neigbours[i]);
				//nextNode.distance[player] = 1 + currentNode.distance[player];

				float dist = (currentNode.neigbours[i].worldPosition - startNodePoint).sqrMagnitude;
                //float distClosest = (neigbours[suitableNode].worldPosition - startNodePoint).sqrMagnitude;
                //if (dist < distClosest)
                //    nextNode.distance[player] = 2 + currentNode.distance[player];
                ////suitableNode = neigbours.IndexOf(nextNode);
                //else
                currentNode.neigbours[i].distance[player] = (int)dist;//+ currentNode.distance[player];
            }
            //neigbours[suitableNode].suitable[player] = true;
            // cache neigbours
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
		startNode.parent = startNode;

		if (!startNode.walkable)
		{
			startNode = grid.ClosestWalkableNode(startNode);
		}
		if (!targetNode.walkable)
		{
			targetNode = grid.ClosestWalkableNode(targetNode);
		}

		if (startNode.walkable && targetNode.walkable)
		{
			Heap<Node> openSet = new Heap<Node>(grid.gridSize);
			HashSet<Node> closedSet = new HashSet<Node>();
			openSet.Add(startNode);

			while (openSet.Count > 0)
			{
				Node currentNode = openSet.RemoveFirst();
				closedSet.Add(currentNode);

				if (!currentNode.walkable)
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
					if (!neighbour.walkable || closedSet.Contains(neighbour))
					{
						continue;
					}

					int newMovementCostToNeighbour = currentNode.gCost + GetDistance(currentNode, neighbour) + neighbour.movementPenalty;
					if (newMovementCostToNeighbour < neighbour.gCost || !openSet.Contains(neighbour))
					{
						neighbour.gCost = newMovementCostToNeighbour;
						neighbour.hCost = GetDistance(neighbour, targetNode);
						neighbour.parent = currentNode;

						if (!openSet.Contains(neighbour))
							openSet.Add(neighbour);
						else
							openSet.UpdateItem(neighbour);
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


	Vector2[] RetracePath(Node startNode, Node endNode)
	{
		List<Node> path = new List<Node>();
		Node currentNode = endNode;

		while (currentNode != startNode)
		{
			path.Add(currentNode);
			currentNode = currentNode.parent;
		}
        Vector2[] waypoints = SimplifyPath(path);
		Array.Reverse(waypoints);
		return waypoints;
	}

	Vector2[] SimplifyPath(List<Node> path)
	{
		List<Vector2> waypoints = new List<Vector2>();
		Vector2 directionOld = Vector2.zero;

		for (int i = 1; i < path.Count; i++)
		{
			Vector2 directionNew = new Vector2(path[i - 1].gridX - path[i].gridX, path[i - 1].gridY - path[i].gridY);
			if (directionNew != directionOld)
			{
				waypoints.Add(path[i].worldPosition);
			}
			directionOld = directionNew;
		}
		return waypoints.ToArray();
	}

	int GetDistance(Node nodeA, Node nodeB)
	{
		int dstX = Mathf.Abs(nodeA.gridX - nodeB.gridX);
		int dstY = Mathf.Abs(nodeA.gridY - nodeB.gridY);

		if (dstX > dstY)
			return 14 * dstY + 10 * (dstX - dstY);
		return 14 * dstX + 10 * (dstY - dstX);
	}


}
