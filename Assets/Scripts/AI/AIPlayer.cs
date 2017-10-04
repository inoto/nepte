﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIPlayer : MonoBehaviour
{
    public int difficulty;

    // cache
    public PlayerController playerController;

    enum Decisions
    {
        AttackBase,
        AttackCluster,
        AttackCenter
    }
    Decisions decision;

    List<PlayerController> enemies;

	// Use this for initialization
	void Start ()
    {
        enemies = new List<PlayerController>();

        playerController = gameObject.GetComponent<PlayerController>();

        StartCoroutine(TakeDecision());
	}

    public void DefineEnemies()
    {
        //int activePlayers = 0;
        //foreach(GameObject player in GameController.Instance.playerControllerObject)
        //{
        //    if (player.activeSelf)
        //        activePlayers += 1;
        //}
        //if (activePlayers == enemies.Count)
            //return;

        enemies.Clear();
        for (int i = 0; i < GameController.Instance.players; i++)
        {
            if (playerController.owner.playerNumber == i)
                continue;
            if (!playerController.gameObject.activeSelf)
                continue;
            enemies.Add(GameController.Instance.playerController[i]);
        }
    }

    IEnumerator TakeDecision()
    {
        yield return new WaitForSeconds(3.0f);
        while(true)
        {
            
            int randDes = Random.Range(0, 2);
            switch (randDes)
            {
                case 0:
                    DecisionAttackBase();
                    break;
                case 1:
                    DecisionAttackCluster();
                    break;
                case 2:
                    DecisionAttackCenter();
                    //Debug.Log("player's " + playerController.owner + " decision is center");
                    break;
            }
            yield return new WaitForSeconds(3.0f + playerController.owner.playerNumber);
        }
    }

    void DecisionAttackBase()
    {
        decision = Decisions.AttackBase;

        DefineEnemies();

        int randInt = Random.Range(0, enemies.Count);
		//Debug.Log("player " + playerController.owner + " want to attack base of player " + randInt);
		Node node = Grid.Instance.NodeFromWorldPoint(enemies[randInt].gameObject.transform.position);
        //playerController.rallyPoint.SetNew(node.worldPosition);
    }

	void DecisionAttackCluster()
	{
		decision = Decisions.AttackCluster;

        DecisionAttackBase();
	}

	void DecisionAttackCenter()
	{
        decision = Decisions.AttackCenter;

        Node node = Grid.Instance.NodeFromWorldPoint(new Vector2(0, 0));
        //playerController.rallyPoint.SetNew(node.worldPosition);
	}

	//public void DecisionDefend()
    //{
    //    decision = Decisions.Defend;
    //    playerController.rallyPoint.gameObject.transform.position = playerController.transform.position;
    //}
}
