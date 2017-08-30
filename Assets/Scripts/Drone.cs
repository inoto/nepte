using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Drone : MonoBehaviour
{
    public enum Mode
    {
        Idle,
        MovingRally,
        MovingTarget,
        Attacking,
        Dead
    }

    [Header("Modules")]
    public Health health = new Health(100);

    [Header("Components")]
    public Transform trans;
    MeshRenderer mesh;
    public Owner owner;
    public Body body;
    public Mover mover;
    public Radar radar;

	[Header("Colors")]
	[SerializeField]
	private Material[] materials;

    private void Awake()
    {
		trans = GetComponent<Transform>();
		mesh = GetComponent<MeshRenderer>();
        owner = GetComponent<Owner>();
        body = GetComponent<Body>();
        mover = GetComponent<Mover>();
        radar = GetComponent<Radar>();
    }

    public void DelayedStart()
    {
        //SetOwnerAsInParent();
        AssignMaterial();
        mover.DelayedStart();
        owner.playerController.playerUnitCount += 1;
        PlayerController.unitCount += 1;
    }

	void SetOwnerAsInParent()
	{
		var ownerParent = trans.parent.GetComponent<Owner>();
		owner.playerNumber = ownerParent.playerNumber;
		owner.playerController = ownerParent.playerController;
	}

    public void CollisionTrigger(CollisionCircle other)
    {
        
    }

	void AssignMaterial()
	{
		if (mesh != null && owner != null)
			mesh.sharedMaterial = materials[owner.playerNumber];
		else
			Debug.LogError("Cannot assign material.");
	}

}
