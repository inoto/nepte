using System.Collections.Generic;
using System.Runtime.Serialization.Formatters;
using UnityEngine;

[System.Serializable]
public class AIDecision
{
	public static AIPlayer aiPlayer = null;
	
	public enum Type
	{
		Attack,
		Defend,
		OccupyNeutral,
		Allocation
	}
	public Type type;

	public Base beginBas;
	public List<Base> beginBases;
	public Base targetBas;
	
	public float weight = 0;
	public float distance = 0;
	
	float sumUnitsPerSecondSelf = 0;
	float sumUnitCountSelf = 0;
	private float sum = 0;
	private bool willBeCaptured = false;

	public AIDecision(Base newBeginBas, Base newTargetBas, AIPlayer newAIPlayer)
	{
		aiPlayer = newAIPlayer;
		
		beginBas = newBeginBas;
		beginBases = new List<Base>();
		targetBas = newTargetBas;
		if (targetBas.owner.playerNumber == -1)
			type = Type.OccupyNeutral;
		else if (targetBas.owner.playerNumber == beginBas.owner.playerNumber)
			type = Type.Allocation;
		else
			type = Type.Attack;

		sumUnitsPerSecondSelf = 1 / beginBas.spawner.intervalMax;
		sumUnitCountSelf = beginBas.spawner.unitCount + beginBas.owner.playerController.playerUnitCount;
		
		CountWeight();
	}

	public void CountWeight()
	{
		if (beginBas.owner.playerNumber == aiPlayer.owner.playerNumber && targetBas.owner.playerNumber == -1)
		{
			CheckUnitCount();
			CheckDistances();
		}
		else if (targetBas.owner.playerNumber == beginBas.owner.playerNumber)
		{
			
		}
		else
		{
			float sumUnitsPerSecond = 1 / targetBas.spawner.intervalMax;
			float sumUnitCount = targetBas.spawner.unitCount + targetBas.owner.playerController.playerUnitCount;
		}
		// если текущая база под атакой, то никого выпускать не надо, дефим
		if (beginBas.weapon != null && beginBas.weapon.isAttacking)
			weight -= 5;
		// нам меньше интересна целевая база которая не может производить юниты
		if (targetBas.type == Base.BaseType.Transit)
			weight -= 1;

		if (beginBas == targetBas)
			weight -= 10;
	}

	void CheckDistances()
	{
		Base closestNeutral = null;
		foreach (var basNeutral in aiPlayer.basesNeutral)
		{
			float distMin = 99999;
			float distSum = 0;
			// если база отправки не одна, то находим ближайшую для всех
			if (beginBases != null)
			{
				foreach (var basBegin in beginBases)
				{
					distSum += ((Vector2) basBegin.trans.position - (Vector2) basNeutral.trans.position).sqrMagnitude;
				}
			}
			else
			{
				distSum += ((Vector2) beginBas.trans.position - (Vector2) basNeutral.trans.position).sqrMagnitude;
			}
			if (distSum < distMin)
			{
				distMin = distSum;
				closestNeutral = basNeutral;
			}
		}
		if (closestNeutral != targetBas)
		{
			weight -= 1;
		}
	}

	void CheckUnitCount()
	{
		// необходимое кол-во юнитов чтобы полностью захватить нейтральную базу
		float targetUnitsNeeded = targetBas.spawner.unitCount + targetBas.spawner.maxCapturePoints;
		// расстояние от текущей базы до целевой
		if (beginBas.owner.playerController.bases.Count > 1)
		{
			float basesSelfUnitCount = 0;
			Base closest = null;
			bool someoneElse = false;
			// ищем суммарное кол-во юнитов в своих базах
			foreach (var basSelf in beginBas.owner.playerController.bases)
			{
				if (basSelf.weapon != null && basSelf.weapon.isAttacking)
					continue;
				basesSelfUnitCount += basSelf.spawner.unitCount;
				beginBases.Add(basSelf);
				// если набрали нужное кол-во, то дальше считать не нужно
				if (basesSelfUnitCount >= targetUnitsNeeded)
					break;
			}
			// если суммарно мы имеем большее кол-во юнитов чем в нейтральной базе, то повышаем вес решения
			if (basesSelfUnitCount >= targetUnitsNeeded)
			{
				weight += 3;
			}
			else
				beginBases = null;
		}
		else
		{
			beginBases = null;
			if (beginBas.spawner.unitCount >= targetUnitsNeeded)
			{
				weight += 3;
			}
		}

		if (targetBas.collision.collidedCount != 0)
			weight -= 1;
		if (targetBas.weapon != null && targetBas.weapon.isAttacking)
			weight -= 1;
		
	}
}
