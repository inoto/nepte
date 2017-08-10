using System.Collections;
using System.Collections.Generic;
using UnityEngine;

interface ICollidable
{
    Node GetNode();
}

public class CollisionManager : MonoBehaviour
{
    private static CollisionManager _instance;

    public static CollisionManager Instance { get { return _instance; } }

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

    private List<Node> nodeList;
    private List<Transform> unitTransforms = new List<Transform>();
    private List<Transform> baseTransforms = new List<Transform>();

    public void AddAllNodes(List<Node> list)
    {
        nodeList = new List<Node>(list);
    }

    public void AddUnitTransform(Transform unitTransform)
    {
        unitTransforms.Add(unitTransform);
    }

    public void RemoveUnitTransform(Transform unitTransform)
    {
        unitTransforms.Remove(unitTransform);
    }

	public void AddBaseTransform(Transform baseTransform)
	{
		baseTransforms.Add(baseTransform);
	}

	public void RemoveBaseTransform(Transform baseTransform)
	{
		baseTransforms.Remove(baseTransform);
	}

	public void Update()
	{
        CheckNodesAndUnitPoints();
	}

    void CheckNodesAndUnitPoints()
    {
        foreach (Node node in nodeList)
        {
            foreach (Transform trans in unitTransforms)
            {
                
                if (node.rect.Contains(trans.position))
                {
                    node.walkable = false;
                    node.prisoner = trans.gameObject;
                }
                else
				{
                    if (node.prisoner == trans.gameObject)
                    {
                        node.walkable = true;
                        node.prisoner = null;
                    }
                    else
                    {
                        
                    }
				} 
            }
        }
    }

    void CheckNodesAndBases()
    {
		foreach (Node node in nodeList)
		{
			foreach (Transform trans in baseTransforms)
			{

				if (node.rect.Contains(trans.position))
				{
					node.walkable = false;
					node.prisoner = trans.gameObject;
				}
				else
				{
					if (node.prisoner != trans.gameObject)
					{
						node.walkable = true;
						node.prisoner = null;
					}
				}
			}
		}
    }
}