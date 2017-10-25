using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

public class AIPlayer : MonoBehaviour
{
    public bool LogDecisionsInfo = true;

    public Owner Owner;
    public float DecisionInterval = 3;

    public List<PlayerController> Enemies = new List<PlayerController>();
    public List<Planet> Planets = new List<Planet>();
    public List<Planet> PlanetsNeutral = new List<Planet>();
    public List<Planet> PlanetsToBeCaptured = new List<Planet>();
    public List<Planet> PlanetsEnemy = new List<Planet>();
    public List<Planet> PlanetsInFrontLine = new List<Planet>();
    public Vector2 FrontLinePoint;
    public Vector2 EmpireCenterPoint;
    public Planet PlanetMain;
    
    public List<AIDecision> Decisions = new List<AIDecision>();
    public AIDecision DecisionBest = null;
    public List<AIDecision> PoorDecisions = new List<AIDecision>();
    public float DistanceToFar;
    
    // moved to planet class
    // public Dictionary<Planet,int> unitCountNearBeginBas = new Dictionary<Planet, int>();
    // public Dictionary<Planet,int> unitCountNearTargetBas = new Dictionary<Planet, int>();

    private void Awake()
    {
        Owner = GetComponent<Owner>();
    }

	private void Start ()
	{
	    FillDistances();
        StartCoroutine(TakeDecision());
	}

    private IEnumerator TakeDecision()
    {
        yield return new WaitForSeconds(DecisionInterval);
        foreach (var b in GameManager.Instance.Planets)
        {
            if (b.Owner.playerNumber == Owner.playerNumber)
            {
                PlanetMain = b;
                DistanceToFar = PlanetMain.Trans.position.sqrMagnitude * 0.75f;
                break;
            }
        }
        while (true)
        {
            DefineEnemies();
            DefinePlanets();
            DefineFrontLinePoint();
            
            FillDecisions();
            ChooseBestDecision();

            yield return new WaitForSeconds(DecisionInterval);
        }
    }
    
    private void ChooseBestDecision()
    {
        DecisionBest = null;
        PoorDecisions.Clear();
        float weightMax = 0;
//      Debug.Log("decisions count: " + decisions.Count);
        foreach (var decision in Decisions)
        {
//          Debug.LogFormat(decision.weight.ToString());
            if (decision.Weight > weightMax)
            {
                weightMax = decision.Weight;
                DecisionBest = decision;
            }
            if (decision.Weight > 0 && decision.Weight < weightMax)
            {
                if (LogDecisionsInfo)
                {
                    Debug.Log("player " + Owner.playerNumber + " almost best decision weight: " + decision.Weight);
                }
                PoorDecisions.Add(decision);
            }
        }
        if (DecisionBest != null)
        {
            if (DecisionBest.BeginPlanet == DecisionBest.TargetPlanet)
            {
                Debug.Log("same planet!");
            }
            if (LogDecisionsInfo)
            {
                Debug.Log("player " + Owner.playerNumber + " best decision weight: " + DecisionBest.Weight);
                Debug.Log("player " + Owner.playerNumber + " best decision type: " + DecisionBest.Type.ToString());
            }
            // chould we release units from a couple planets?
            if (DecisionBest.BeginPlanets != null && DecisionBest.BeginPlanets.Count > 1)
            {
//              Debug.Log("releasing from multiple");
                foreach (var b in DecisionBest.BeginPlanets)
                {
                    b.Spawner.ReleaseUnits(DecisionBest.TargetPlanet.gameObject);
                }
            }
            else
            {
//              Debug.Log("releasing from one");
                DecisionBest.BeginPlanet.Spawner.ReleaseUnits(DecisionBest.TargetPlanet.gameObject);
            }
            // mark planets which were already targeted
            if (DecisionBest.TargetPlanet.Owner.playerNumber == -1)
            {
                PlanetsToBeCaptured.Add(DecisionBest.TargetPlanet);
            }
        }
//      else
//          Debug.Log("no best decision");
    }

    private void FillDecisions()
    {
        Decisions.Clear();
        foreach (var bas in GameManager.Instance.Planets)
        {
            foreach (var basSelf in Planets)
            {
                if (bas == basSelf)
                {
                    continue;
                }
                Decisions.Add(new AIDecision(bas, basSelf, this));
            }
            foreach (var basNeutral in PlanetsNeutral)
            {
                if (PlanetsToBeCaptured.Contains(basNeutral))
                {
                    continue;
                }
                Decisions.Add(new AIDecision(bas, basNeutral, this));
            }
            foreach (var basToAttack in PlanetsEnemy)
            {
                Decisions.Add(new AIDecision(bas, basToAttack, this));
            }
        }
    }
    
    private void FillDistances()
    {
        foreach (var basX in GameManager.Instance.Planets)
        {
            foreach (var basY in GameManager.Instance.Planets)
            {
                if (basX == basY)
                {
                    continue;
                }
                if (!basX.DictDistances.ContainsKey(basY))
                {
                    basX.DictDistances.Add(basY, basX.Trans.position - basY.Trans.position);
                }
            }
        }
//        Debug.Log("distances filled");
    }

    private void DefineEnemies()
    {
        Enemies.Clear();
        foreach (var player in GameManager.Instance.PlayerController)
        {
            if (player.Owner.playerNumber == -1)
            {
                continue;
            }
            if (player == Owner.playerController)
            {
                continue;
            }
            if (!player.gameObject.activeSelf)
            {
                continue;
            }
            Enemies.Add(player);
        }
    }

    private void DefinePlanets()
    {
        Planets.Clear();
        PlanetsNeutral.Clear();
        PlanetsEnemy.Clear();
        
        Vector2 sum = Vector2.zero;
        int count = 0;
        
        foreach (var b in GameManager.Instance.Planets)
        {
            if (b.Owner.playerNumber == Owner.playerNumber)
            {
                Planets.Add(b);
                sum += (Vector2)b.Trans.position;
                count++;
                if (PlanetsToBeCaptured.Contains(b))
                {
                    PlanetsToBeCaptured.Remove(b);
                }
            }
            else if (b.Owner.playerNumber == -1)
            {
                PlanetsNeutral.Add(b);
            }
            else
            {
                PlanetsEnemy.Add(b);
                if (PlanetsToBeCaptured.Contains(b))
                    PlanetsToBeCaptured.Remove(b);
            }
            // считаем кол-во юнитов рядом с базой
            FillUnitCountNearPlanet(b);
        }
        DefinePlanetsInFrontLine();
        if (count > 0)
        {
            EmpireCenterPoint = sum / count;
        }
    }
    
    // TODO: move method to planet class
    private void FillUnitCountNearPlanet(Planet planet)
    {
        // unit count calculates through collisions and cached in planet class
        // circle size where we are looking for units depends on collider size * multiplier
        const float multiplier = 1.5f;
        List<CollisionCircle> unitsNearBas =
            CollisionManager.Instance.FindBodiesInCircleArea(planet.Trans.position, planet.Collider.radius * multiplier);
        int unitsNearPlanetSelf = 0;
        int unitsNearPlanetEnemies = 0;
        for (int i = 0; i < unitsNearBas.Count; i++)
        {
            if (unitsNearBas[i].owner.playerNumber == planet.Owner.playerNumber)
            {
                unitsNearPlanetSelf++;
            }
            else
            {
                unitsNearPlanetEnemies++;
            }
        }
        planet.UnitCountNearBasSelf = unitsNearPlanetSelf;
        planet.UnitCountNearBasEnemies = unitsNearPlanetEnemies;
    }
    
    private void DefinePlanetsInFrontLine()
    {
        PlanetsInFrontLine.Clear();
        foreach (var basToAttack in PlanetsEnemy)
        {
            Planet closestToEnemy = null;
            float distMin = 99999;
            foreach (var basSelf in Owner.playerController.Planets)
            {
                float dist = (basSelf.Trans.position - basToAttack.Trans.position).sqrMagnitude;
                // skip if so far
                if (dist > DistanceToFar)
                {
                    continue;
                }
                if (dist < distMin)
                {
                    distMin = dist;
                    closestToEnemy = basSelf;
                }
            }
            if (closestToEnemy != null)
            {
                // then finding ally planets to be in front line
                float distNeeded = (closestToEnemy.Trans.position - basToAttack.Trans.position).sqrMagnitude * 1.4f;
                foreach (var basSelf in Owner.playerController.Planets)
                {
                    float dist = (basSelf.Trans.position - basToAttack.Trans.position).sqrMagnitude;
                    // 
                    if (dist < distNeeded)
                    {
                        if (!PlanetsInFrontLine.Contains(basSelf))
                        {
                            PlanetsInFrontLine.Add(basSelf);
                        }
                    }
                }
            }
        }
    }

    private void DefineFrontLinePoint()
    {
        Planet suitable = null;
        float distMin = 9999;
        
        foreach (var b in PlanetsEnemy)
        {
            float dist = (EmpireCenterPoint - (Vector2)b.Trans.position).sqrMagnitude;
            if (dist < distMin * distMin)
            {
                distMin = dist;
                suitable = b;
            }
        }
        if (PlanetsEnemy.Count > 0)
            FrontLinePoint = (Vector2)suitable.Trans.position + EmpireCenterPoint;
    }

    private void OnDrawGizmos()
    {
//        Gizmos.color = Color.blue;
//        Gizmos.DrawSphere(empireCenterPoint, 0.3f);
//        Gizmos.color = Color.red;
//        Gizmos.DrawSphere(frontLinePoint, 0.3f);
        if (DecisionBest != null && DecisionBest.TargetPlanet != null)
        {
            Gizmos.color = Color.white;
            if (DecisionBest.BeginPlanets != null)
            {
                foreach (var basBegin in DecisionBest.BeginPlanets)
                {
                    Gizmos.DrawLine(basBegin.Trans.position, DecisionBest.TargetPlanet.Trans.position);
                    Gizmos.DrawSphere(DecisionBest.TargetPlanet.Trans.position, 0.5f);
                }
            }
            else if (DecisionBest.BeginPlanet != null)
            {
                Gizmos.DrawLine(DecisionBest.BeginPlanet.Trans.position, DecisionBest.TargetPlanet.Trans.position);
                Gizmos.DrawSphere(DecisionBest.TargetPlanet.Trans.position, 0.5f);
            }
#if UNITY_EDITOR
            Handles.Label(DecisionBest.TargetPlanet.Trans.position, DecisionBest.Weight.ToString());
#endif
        }
        if (PoorDecisions.Count > 0)
        {
            Gizmos.color = Color.gray;
            foreach (var decision in PoorDecisions)
            {
                Gizmos.DrawLine(decision.BeginPlanet.Trans.position, decision.TargetPlanet.Trans.position);
                Gizmos.DrawSphere(decision.TargetPlanet.Trans.position, 0.5f);
                
#if UNITY_EDITOR
                Handles.Label(decision.TargetPlanet.Trans.position, decision.Weight.ToString());
#endif
            }
        }
    }
}
