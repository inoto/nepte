using UnityEngine;

public class Radar : MonoBehaviour
{
    public bool showRadius = false;

    public float radius = 5;

	[Header("Cache")]
	public Transform trans;
	public Drone droneParent;

	// Use this for initialization
	void Awake ()
    {
        trans = GetComponent<Transform>();
		droneParent = trans.parent.gameObject.GetComponent<Drone>();
	}

	public void OnEnable()
	{
		trans = GetComponent<Transform>();
        droneParent = trans.parent.gameObject.GetComponent<Drone>();
	}

	public void OnDrawGizmos()
	{
		if (showRadius)
		{
			Color newColorAgain = Color.yellow;
			newColorAgain.a = 0.5f;
			Gizmos.color = newColorAgain;
			Gizmos.DrawWireSphere(trans.position, radius);
		}
	}
}
