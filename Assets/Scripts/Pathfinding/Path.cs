using System;
using System.Collections.Generic;
using UnityEngine;

[Obsolete("Not used anymore",true)]
[System.Serializable]
public class Path
{
    public int futureNodeCount = 3;
    public int realNodeCount;
    public Node[] waypoints;
    public Node destinationNode;

    public Path()
    {
        waypoints = new Node[futureNodeCount];
        for (int i = 1; i < futureNodeCount; i++)
        {
            waypoints[i] = null;
        }
    }

    public bool CreateNew(Node _nodeFrom, Node _nodeTo, int _playerNumber)
    {
        //Debug.Log("nodeFrom: " + _nodeFrom.gridX + "," + _nodeFrom.gridY + " nodeTo: " + _nodeTo.gridX + "," + _nodeTo.gridY);
        if (_nodeFrom == _nodeTo)
            return false;

        destinationNode = _nodeTo;

        List<Node> deniedNodes = new List<Node>();

        waypoints[0] = Grid.DefineNextNode(_nodeFrom, _playerNumber);
        if (waypoints[0] == null)
        {
            //Debug.Log("waipoint0 is null");
            return false;
        }
        if (waypoints[0] == _nodeTo)
        {
			//Debug.Log("destination is here");
            waypoints[0] = null;
            return false;
        }
        deniedNodes.Add(waypoints[0]);
        realNodeCount += 1;

        for (int i = 1; i < waypoints.Length; i++)
        {
            waypoints[i] = Grid.Instance.DefineNextNode(waypoints[i - 1], deniedNodes, _playerNumber);
            if (waypoints[i] == null)
            {
                //Debug.Log("waipoint" + i + " is null");
                break;
            }
            if (waypoints[i] == _nodeTo)
            {
				//Debug.Log("destination so close");
                waypoints[i] = null;
				break;
            }
            deniedNodes.Add(waypoints[i]);
            realNodeCount += 1;
        }
        //realNodeCount = waypoints.Length;
        return true;
    }

    public bool UpdateStep(int _playerNumber)
    {
        List<Node> deniedNodes = new List<Node>();

        // change values: LAST -> n -> 0 if value is present
        for (int i = 0; i < waypoints.Length - 1; i++)
        {
            if (waypoints[i + 1] != null)
            {
                waypoints[i] = waypoints[i + 1];
                deniedNodes.Add(waypoints[i]);
            }
        }
        // add new value to the LAST
        Node tmpNode = Grid.Instance.DefineNextNode(waypoints[waypoints.Length - 2], deniedNodes, _playerNumber);
        if (tmpNode != null && tmpNode != destinationNode)
            waypoints[waypoints.Length - 1] = tmpNode;
        else
            waypoints[waypoints.Length - 1] = null;
        return true;
    }

	public void DrawPath() {

        Color newColor = Color.green;
        newColor.a = 0.3f;
        Gizmos.color = newColor;
        for (int i = 0; i < realNodeCount; i++)
		{
            if (waypoints[i] != null)
            {
                Gizmos.DrawCube(waypoints[i].WorldPosition, Vector3.one * (1.0f - 0.01f));
                if (i < realNodeCount - 1)
                    Gizmos.DrawLine(waypoints[i].WorldPosition, waypoints[i + 1].WorldPosition);
            }
		}

	}

}
