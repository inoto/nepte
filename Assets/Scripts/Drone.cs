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
    public Owner owner;
    public Health health = new Health(100);
    public Body body;

    [Header("Components")]
    public Transform trans;
    MeshRenderer mesh;
    public Mover mover;
    public Radar radar;

    private void Awake()
    {
		trans = GetComponent<Transform>();
		mesh = GetComponent<MeshRenderer>();
        mover = GetComponent<Mover>();
        radar = GetComponent<Radar>();
    }

    private void Start()
    {
        body = new Body(this);

    }

    public void ActivateWithOwner()
    {
        mover.ActivateWithOwner();
        owner.playerController.playerUnitCount += 1;
        PlayerController.unitCount += 1;
    }

}
