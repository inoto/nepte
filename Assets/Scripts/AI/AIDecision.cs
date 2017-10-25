using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class AIDecision
{
	public static AIPlayer AiPlayer = null;
	
	public enum DecisionType
	{
		AttackFromFrontLine,
		Attack,
		Defense,
		OccupyNeutral,
		Allocation
	}
	public DecisionType Type;

	public readonly Planet BeginPlanet;
	public readonly List<Planet> BeginPlanets;
	public readonly Planet TargetPlanet;
	
	public float Weight = 0;
	
	private float sum = 0;
	private bool willBeCaptured = false;

	public AIDecision(Planet newBeginPlanet, Planet newTargetPlanet, AIPlayer newAiPlayer)
	{
		if (newBeginPlanet == null)
		{
			return;
		}
		if (newTargetPlanet == null)
		{
			return;
		}

		AiPlayer = newAiPlayer;
		
		BeginPlanet = newBeginPlanet;
		BeginPlanets = new List<Planet>();
		TargetPlanet = newTargetPlanet;

		CountWeight();
	}

	private void CountWeight()
	{
		BeginPlanets.Clear();
		if (BeginPlanet.Owner.playerNumber == AiPlayer.Owner.playerNumber && TargetPlanet.Owner.playerNumber == -1)
		{
			CheckUnitCount();
			CheckDistances();
			Type = DecisionType.OccupyNeutral;
		}
		else if (TargetPlanet.Owner.playerNumber == BeginPlanet.Owner.playerNumber && BeginPlanet.Owner.playerNumber == AiPlayer.Owner.playerNumber)
		{
			CheckToMove();
			CheckToDefend();
		}
		else if (BeginPlanet.Owner.playerNumber == AiPlayer.Owner.playerNumber && TargetPlanet.Owner.playerNumber != AiPlayer.Owner.playerNumber)
		{
			float sumUnitsPerSecond = 1 / TargetPlanet.Spawner.intervalMax;
			float sumUnitCount = TargetPlanet.Spawner.unitCount + TargetPlanet.Owner.playerController.PlayerUnitCount;
			CheckToAttackFromFrontLine();
//			CheckToAttack();
		}
		// if enemies near target planet then no need to release units
//		if (targetBas.unitCountNearBasSelf > 1)
//			weight -= 5;
		
		// if current planet is under attack then defend
		if (BeginPlanet.UnitCountNearBasEnemies > 2)
		{
			Weight -= 10;
		}

		// transit planet (which cannot produce units) is less interesting
		if (TargetPlanet.Type == Planet.PlanetType.Transit)
		{
			Weight -= 1;
		}

		if (BeginPlanet == TargetPlanet)
		{
			Weight -= 10;
		}
	}

	private void CheckToAttackFromFrontLine()
	{
		if (!AiPlayer.PlanetsInFrontLine.Contains(BeginPlanet))
		{
			return;
		}
		Weight += 3;

		Planet closestToAttack = null;
		float distMin = 99999;
		foreach (var basToAttack in AiPlayer.PlanetsEnemy)
		{
			float dist = (BeginPlanet.Trans.position - basToAttack.Trans.position).sqrMagnitude;
			if (dist < distMin)
			{
				distMin = dist;
				closestToAttack = basToAttack;
			}
		}
		if (closestToAttack != TargetPlanet)
		{
			Weight -= 1;
		}

		float sumUnitCountSelf = 0;
		foreach (var basInFrontLine in AiPlayer.PlanetsInFrontLine)
		{
			if ((basInFrontLine.Trans.position - TargetPlanet.Trans.position).sqrMagnitude < AiPlayer.DistanceToFar)
			{
				sumUnitCountSelf += basInFrontLine.Spawner.unitCount;
				BeginPlanets.Add(basInFrontLine);
			}
		}
		if (sumUnitCountSelf * 0.9f < TargetPlanet.Spawner.unitCount)
		{
			Weight -= 3;
			BeginPlanets.Clear();
		}
		// if we have units near target planet then we can help them
		if (TargetPlanet.UnitCountNearBasEnemies > 3)
		{
			Weight += 1;
		}
		// if target planet has its own units near then it's not a priority
		if (TargetPlanet.UnitCountNearBasSelf > 5)
		{
			Weight -= 3;
			BeginPlanets.Clear();
		}
		else
		{
			Type = DecisionType.AttackFromFrontLine;
		}

		// switching determination and cowardice depends on planet count
		if (AiPlayer.Planets.Count < AiPlayer.PlanetsEnemy.Count)
		{
			Weight -= 1;
		}
	}

	private void CheckToDefend()
	{
		// if ally planet is under attack then we need to help to this planet
		if (TargetPlanet.UnitCountNearBasEnemies > 1)
		{
			Weight += 3;
			float unitCount = 0;
			// choosing planet to defend it
			foreach (var basSelf in AiPlayer.Planets)
			{
				if ((basSelf.Trans.position - TargetPlanet.Trans.position).sqrMagnitude < AiPlayer.DistanceToFar)
				{
					unitCount += basSelf.Spawner.unitCount;
					BeginPlanets.Add(basSelf);
				}
			}
			// if unit count on ally planets is less than attackers count then we lose this planet
			if (unitCount < TargetPlanet.UnitCountNearBasEnemies)
			{
				Weight -= 3;
				BeginPlanets.Clear();
			}
			Type = DecisionType.Defense;
		}
	}

	private void CheckToMove()
	{
		// if target planet is in front line but main is not then we want to allocate
		if (AiPlayer.PlanetsInFrontLine.Contains(TargetPlanet) && !AiPlayer.PlanetsInFrontLine.Contains(BeginPlanet))
		{
			Weight += 3;
		}
		// small unit count is not good
		if (BeginPlanet.Spawner.unitCount < 10)
		{
			Weight -= 1;
		}
		// split units between planets in front line
		foreach (var basInFrontLine in AiPlayer.PlanetsInFrontLine)
		{
			if (TargetPlanet.Spawner.unitCount > basInFrontLine.Spawner.unitCount)
			{
				Weight -= 1;
			}
		}
		Type = DecisionType.Allocation;
	}

	private void CheckDistances()
	{
		Planet closestNeutral = null;
		foreach (var basNeutral in AiPlayer.PlanetsNeutral)
		{
			float distMin = 99999;
			float distSum = 0;
			// finding closest to all begin planets
			if (BeginPlanets.Count > 1)
			{
				foreach (var basBegin in BeginPlanets)
				{
					distSum += ((Vector2) basBegin.Trans.position - (Vector2) basNeutral.Trans.position).sqrMagnitude;
				}
			}
			else
			{
				distSum += ((Vector2) BeginPlanet.Trans.position - (Vector2) basNeutral.Trans.position).sqrMagnitude;
			}
			if (distSum < distMin)
			{
				distMin = distSum;
				closestNeutral = basNeutral;
			}
			// using magic distance value to avoid long way
//			if (distSum > aiPlayer.distToFar)
//				weight -= 1;
		}
		// not closest neutral is not good
		if (closestNeutral != TargetPlanet)
		{
			Weight -= 1;
		}
		if ((BeginPlanet.Trans.position - TargetPlanet.Trans.position).sqrMagnitude > AiPlayer.DistanceToFar)
		{
			Weight -= 3;
		}
	}

	private void CheckUnitCount()
	{
		// unit count which is enough to fully capture neutral planet
		float targetUnitsNeeded = TargetPlanet.Spawner.unitCount + TargetPlanet.Spawner.maxCapturePoints;
		// querying ally planets around for coperation
		float planetsSelfUnitCount = 0;
		if (BeginPlanet.Owner.playerController.Planets.Count > 1)
		{
			// count units in all planets
			foreach (var basSelf in BeginPlanet.Owner.playerController.Planets)
			{
				// skip planet in action
				if (basSelf.UnitCountNearBasEnemies > basSelf.Spawner.unitCount)
				{
					continue;
				}
				planetsSelfUnitCount += basSelf.Spawner.unitCount;
				BeginPlanets.Add(basSelf);
				// stop counting if got necessery amount
//				if (basesSelfUnitCount >= targetUnitsNeeded + targetBas.unitCountNearBasEnemies)
//					break;
			}
			// summary unit count should be more then unit count in neutral planet
			if (planetsSelfUnitCount >= targetUnitsNeeded)
			{
				Weight += 3;
			}
//			else
//				beginPlanet = null;
		}
		else
		{
//			beginPlanet = null;
			if (BeginPlanet.Spawner.unitCount >= targetUnitsNeeded)
			{
				Weight += 3;
			}
		}

		// checking for current capturers 
		if (TargetPlanet.UnitCountNearBasEnemies > TargetPlanet.Spawner.maxCapturePoints)
		{
			Weight -= 1;
		}
		// here we have a chance to intercept neutral planet when enemy has spent units on this
		if (TargetPlanet.Spawner.isCapturing && TargetPlanet.Spawner.captureLead != AiPlayer.Owner.playerNumber)
		{
			if (TargetPlanet.UnitCountNearBasEnemies + TargetPlanet.Spawner.unitCount < planetsSelfUnitCount &&
			    BeginPlanets.Count > 1)
			{
				Weight += 1;
			}
		}
	}
}
