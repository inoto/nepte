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

    [Header("Components")]
    public Transform trans;
    MeshRenderer mesh;
    public Mover move;

    private void Awake()
    {
		trans = GetComponent<Transform>();
		mesh = GetComponent<MeshRenderer>();
        move = GetComponent<Mover>();
    }

    public void ActivateWithOwner()
    {
        move.ActivateWithOwner();
    }

}
