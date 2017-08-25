using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class Body
{
    public bool showRadius = false;

    public float radius = 1;
    public float radiusHard = 0.5f;
    public int strength = 0;

    Drone drone;

	// Use this for initialization
	//void Awake()
	//{
	//	drone = GetComponent<Drone>();
	//}

    public Body(Drone _drone)
    {
        drone = _drone;
        CollisionManager.Instance.objects.Add(drone);
    }

    //private void Start()
    //{
    //    CollisionManager.Instance.objects.Add(drone);
    //}

    public void OnDrawGizmos()
	{
		if (showRadius)
		{
			Color newColorAgain = Color.green;
			newColorAgain.a = 0.8f;
			Gizmos.color = newColorAgain;
			Gizmos.DrawWireSphere(drone.trans.position, radius);
		}
	}
}
