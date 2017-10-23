using System.Collections.Generic;
using System.Runtime.Serialization.Formatters;
using UnityEngine;

[System.Serializable]
public class AIDecision
{
	public static AIPlayer aiPlayer = null;
	
	public enum Type
	{
		AttackFromFrontLine,
		Attack,
		Defense,
		OccupyNeutral,
		Allocation
	}
	public Type type;

	public Planet beginBas;
	public List<Planet> beginBases;
	public Planet targetBas;
	
	public float weight = 0;
	public float distance = 0;
	
	float sumUnitsPerSecondSelf = 0;
	float sumUnitCountSelf = 0;
	private float sum = 0;
	private bool willBeCaptured = false;

	public AIDecision(Planet newBeginBas, Planet newTargetBas, AIPlayer newAIPlayer)
	{
		aiPlayer = newAIPlayer;
		
		beginBas = newBeginBas;
		beginBases = new List<Planet>();
		targetBas = newTargetBas;

		sumUnitsPerSecondSelf = 1 / beginBas.Spawner.intervalMax;
		sumUnitCountSelf = beginBas.Spawner.unitCount + beginBas.Owner.playerController.playerUnitCount;
		
		CountWeight();
	}

	public void CountWeight()
	{
		beginBases.Clear();
		if (beginBas.Owner.playerNumber == aiPlayer.owner.playerNumber && targetBas.Owner.playerNumber == -1)
		{
			CheckUnitCount();
			CheckDistances();
			type = Type.OccupyNeutral;
		}
		else if (targetBas.Owner.playerNumber == beginBas.Owner.playerNumber && beginBas.Owner.playerNumber == aiPlayer.owner.playerNumber)
		{
			CheckToMove();
			CheckToDefend();
		}
		else if (beginBas.Owner.playerNumber == aiPlayer.owner.playerNumber && targetBas.Owner.playerNumber != aiPlayer.owner.playerNumber)
		{
			float sumUnitsPerSecond = 1 / targetBas.Spawner.intervalMax;
			float sumUnitCount = targetBas.Spawner.unitCount + targetBas.Owner.playerController.playerUnitCount;
			CheckToAttackFromFrontLine();
			CheckToAttack();
		}
		// если рядом с целевой базой враги, то никого выпускать не надо
//		if (targetBas.unitCountNearBasSelf > 1)
//			weight -= 5;
		
		// если текущая база под атакой, то никого выпускать не надо, дефим
		if (beginBas.UnitCountNearBasEnemies > 2)
			weight -= 10;
		
		// нам немного меньше интересна целевая база которая не может производить юниты
		if (targetBas.Type == Planet.PlanetType.Transit)
			weight -= 1;

		if (beginBas == targetBas)
			weight -= 10;
	}

	void CheckToAttack()
	{
		
	}

	void CheckToAttackFromFrontLine()
	{
		if (!aiPlayer.basesInFrontLine.Contains(beginBas))
			return;
		weight += 3;

		Planet closestToAttack = null;
		float distMin = 99999;
		foreach (var basToAttack in aiPlayer.basesToAttack)
		{
			float dist = (beginBas.Trans.position - basToAttack.Trans.position).sqrMagnitude;
			if (dist < distMin)
			{
				distMin = dist;
				closestToAttack = basToAttack;
			}
		}
		if (closestToAttack != targetBas)
			weight -= 1;

		float sumUnitCountSelf = 0;
		foreach (var basInFrontLine in aiPlayer.basesInFrontLine)
		{
			if ((basInFrontLine.Trans.position - targetBas.Trans.position).sqrMagnitude < aiPlayer.distToFar)
			{
				sumUnitCountSelf += basInFrontLine.Spawner.unitCount;
				beginBases.Add(basInFrontLine);
			}
		}
		if (sumUnitCountSelf * 0.9f < targetBas.Spawner.unitCount)
		{
			weight -= 3;
			beginBases.Clear();
		}
		// если у целевой базы есть свои, то повышаем вес, чтобы выслать подкрепление
		if (targetBas.UnitCountNearBasEnemies > 3)
			weight += 1;
		// если у целевой базы скопление врагов, то отменяем операцию
		if (targetBas.UnitCountNearBasSelf > 5)
		{
			weight -= 3;
			beginBases.Clear();
		}
		else
			type = Type.AttackFromFrontLine;
		
		// меняем смелость в зависимости от соотношения кол-ва баз
		if (aiPlayer.bases.Count < aiPlayer.basesToAttack.Count)
			weight -= 1;
	}

	void CheckToDefend()
	{
		// если дружественная база под атакой множества юнитов, то повышаем вес
		if (targetBas.UnitCountNearBasEnemies > 1)
		{
			weight += 3;
			float unitCount = 0;
			// ищем ближайшие дружественные базы для дефа
			foreach (var basSelf in aiPlayer.bases)
			{
				if ((basSelf.Trans.position - targetBas.Trans.position).sqrMagnitude < aiPlayer.distToFar)
				{
					unitCount += basSelf.Spawner.unitCount;
					beginBases.Add(basSelf);
				}
			}
			// если атакована перевалочная база, то надо дефать, ибо она не умеет обороняться
//			if (targetBas.type == Base.BaseType.Transit)
//				weight += 3;
			// если кол-во юнитов во всех дружественных базах меньше чем кол-во атакующих, то понижаем вес (отдаём базу)
			if (unitCount < targetBas.UnitCountNearBasEnemies)
			{
				weight -= 3;
				beginBases.Clear();
			}
			type = Type.Defense;
		}
	}

	void CheckToMove()
	{
		// если целевая база на линии фронта, а начальная база нет, то повышаем вес
		if (aiPlayer.basesInFrontLine.Contains(targetBas) && !aiPlayer.basesInFrontLine.Contains(beginBas))
		{
			weight += 3;
		}
		// не выпускаем юнитов на линию фронта если их мало
		if (beginBas.Spawner.unitCount < 10)
		{
			weight -= 1;
		}
		// балансируем кол-во юнитов в базах на линии фронта
		foreach (var basInFrontLine in aiPlayer.basesInFrontLine)
		{
			if (targetBas.Spawner.unitCount > basInFrontLine.Spawner.unitCount)
				weight -= 1;
		}
		type = Type.Allocation;
	}

	void CheckDistances()
	{
		Planet closestNeutral = null;
		foreach (var basNeutral in aiPlayer.basesNeutral)
		{
			float distMin = 99999;
			float distSum = 0;
			// если база отправки не одна, то находим ближайшую для всех
			if (beginBases.Count > 1)
			{
				foreach (var basBegin in beginBases)
				{
					distSum += ((Vector2) basBegin.Trans.position - (Vector2) basNeutral.Trans.position).sqrMagnitude;
				}
			}
			else
			{
				distSum += ((Vector2) beginBas.Trans.position - (Vector2) basNeutral.Trans.position).sqrMagnitude;
			}
			if (distSum < distMin)
			{
				distMin = distSum;
				closestNeutral = basNeutral;
			}
			// если суммарное расстояние больше предела, то не суёмся, ибо врятли успеем
//			if (distSum > aiPlayer.distToFar)
//				weight -= 1;
		}
		// если целевая база не самая ближайшая нейтральная, то понижаем вес
		if (closestNeutral != targetBas)
			weight -= 1;
		if ((beginBas.Trans.position - targetBas.Trans.position).sqrMagnitude > aiPlayer.distToFar)
			weight -= 3;
	}

	void CheckUnitCount()
	{
		// необходимое кол-во юнитов чтобы полностью захватить нейтральную базу
		float targetUnitsNeeded = targetBas.Spawner.unitCount + targetBas.Spawner.maxCapturePoints;
		// опрашиваем соседние дружественные базы для кооперации
		float basesSelfUnitCount = 0;
		if (beginBas.Owner.playerController.bases.Count > 1)
		{
			// ищем суммарное кол-во юнитов в своих базах
			foreach (var basSelf in beginBas.Owner.playerController.bases)
			{
				// если сосед воюет, то пропускает его
				if (basSelf.UnitCountNearBasEnemies > basSelf.Spawner.unitCount)
					continue;
				basesSelfUnitCount += basSelf.Spawner.unitCount;
				beginBases.Add(basSelf);
				// если набрали нужное кол-во, то дальше считать не нужно
//				if (basesSelfUnitCount >= targetUnitsNeeded + targetBas.unitCountNearBasEnemies)
//					break;
			}
			// если суммарно мы имеем большее кол-во юнитов чем в нейтральной базе, то повышаем вес решения
			if (basesSelfUnitCount >= targetUnitsNeeded)
			{
				weight += 3;
			}
//			else
//				beginBases = null;
		}
		else
		{
//			beginBases = null;
			if (beginBas.Spawner.unitCount >= targetUnitsNeeded)
			{
				weight += 3;
			}
		}

		// если уже есть захватчики
		if (targetBas.UnitCountNearBasEnemies > targetBas.Spawner.maxCapturePoints)
			weight -= 1;
		// если база захватывается врагом, то есть шанс перехватить её
		if (targetBas.Spawner.isCapturing && targetBas.Spawner.captureLead != aiPlayer.owner.playerNumber)
		{
			if (targetBas.UnitCountNearBasEnemies + targetBas.Spawner.unitCount < basesSelfUnitCount &&
			    beginBases.Count > 1)
			{
				weight += 1;
//				Debug.Log("interception");
			}
		}

		// добавить в переменные ближайшую базу врага
		// если расстояние до ближайшей базы врага меньше чем до нейтральной базы, то снижаем вес
		if (aiPlayer.basesInFrontLine.Count > 0)
		{
			
		}
	}
}
