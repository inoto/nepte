using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Drone : MonoBehaviour, ITargetable
{
    public enum Mode
    {
        Idle,
        Moving,
        Attacking,
        Dead
    }

	public Mode mode;

	public GameObject explosionPrefab;

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

	void Die()
	{
		mode = Mode.Dead;
		MakeExplosion();
		owner.playerController.playerUnitCount -= 1;
		PlayerController.unitCount -= 1;
		ObjectPool.Recycle(gameObject);
	}

	void MakeExplosion()
	{
		GameObject explosion = Instantiate(explosionPrefab, trans.position, trans.rotation);
		explosion.transform.parent = GameController.Instance.transform;
	}

	void SetOwnerAsInParent()
	{
		var ownerParent = trans.parent.GetComponent<Owner>();
		owner.playerNumber = ownerParent.playerNumber;
		owner.playerController = ownerParent.playerController;
	}

	void AssignMaterial()
	{
		if (mesh != null && owner != null)
			mesh.sharedMaterial = materials[owner.playerNumber];
		else
			Debug.LogError("Cannot assign material.");
	}

	public void Damage(Weapon weapon)
	{
		health.current -= weapon.damage;
		if (health.current <= 0)
		{
			Die();
			weapon.EndCombat();
		}
	}

	public GameObject GameObj
	{
		get { return gameObject; }
	}

	public bool IsDied
	{
		get { return (mode == Mode.Dead); }
	}
}
