using UnityEngine;

public class RallyPoint : MonoBehaviour
{
    public int owner;

    private Vector3 mousePosition;

    public void SetNew(Vector3 newRallyPoint)
    {
        if (transform.position != newRallyPoint)
        {
            transform.position = newRallyPoint;
        }
    }

	public int GetOwner()
	{
		return owner;
	}

	public void SetOwner(int newOwner)
	{
		owner = newOwner;
	}

	public GameObject GetGameObject()
	{
		return gameObject;
	}
}
