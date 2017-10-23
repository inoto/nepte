using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

public class AIPlayer : MonoBehaviour
{
    public bool verboseDecisionsInfo = true;
    public int difficulty;

    public Owner owner;

    public float decisionInterval = 3;

    public List<PlayerController> enemies = new List<PlayerController>();
    public List<Planet> bases = new List<Planet>();
    public List<Planet> basesNeutral = new List<Planet>();
    public List<Planet> basesToBeCaptured = new List<Planet>();
    public List<Planet> basesToAttack = new List<Planet>();
    public List<Planet> basesInFrontLine = new List<Planet>();
    public Vector2 frontLinePoint;
    public Vector2 empireCenterPoint;
    public Planet basMain;
    
    public List<AIDecision> decisions = new List<AIDecision>();
    public AIDecision decisionBest = null;
    public List<AIDecision> almostDecisions = new List<AIDecision>();
    public float distToFar;
    public Dictionary<Planet,int> unitCountNearBeginBas = new Dictionary<Planet, int>();
    public Dictionary<Planet,int> unitCountNearTargetBas = new Dictionary<Planet, int>();

    private void Awake()
    {
        owner = GetComponent<Owner>();
    }

	void Start ()
	{
	    FillDistances();
        StartCoroutine(TakeDecision());
	}

    IEnumerator TakeDecision()
    {
        yield return new WaitForSeconds(decisionInterval);
        foreach (var b in GameController.Instance.bases)
        {
            if (b.Owner.playerNumber == owner.playerNumber)
            {
                basMain = b;
                distToFar = basMain.Trans.position.sqrMagnitude * 0.75f;
                break;
            }
        }
        while (true)
        {
            DefineEnemies();
            DefineBases();
            DefineFrontLinePoint();
            
            FillDecisions();
            ChooseBestDecision();

            yield return new WaitForSeconds(decisionInterval);
        }
    }
    
    void ChooseBestDecision()
    {
        decisionBest = null;
        almostDecisions.Clear();
        float weightMax = 0;
//        Debug.Log("decisions count: " + decisions.Count);
        foreach (var decision in decisions)
        {
//            Debug.LogFormat(decision.weight.ToString());
            if (decision.weight > weightMax)
            {
                
                weightMax = decision.weight;
                decisionBest = decision;
            }
            if (decision.weight > 0 && decision.weight < weightMax)
            {
                if (verboseDecisionsInfo)
                    Debug.Log("player " + owner.playerNumber + " almost best decision weight: " + decision.weight);
                almostDecisions.Add(decision);
            }
        }
        if (decisionBest != null)
        {
            if (decisionBest.beginBas == decisionBest.targetBas)
                Debug.Log("same base!");
            if (verboseDecisionsInfo)
            {
                Debug.Log("player " + owner.playerNumber + " best decision weight: " + decisionBest.weight);
                Debug.Log("player " + owner.playerNumber + " best decision type: " + decisionBest.type.ToString());
            }
            // проверяем можем ли мы запустить юнитов сразу из нескольких баз
            if (decisionBest.beginBases != null && decisionBest.beginBases.Count > 1)
            {
//                Debug.Log("releasing from multiple");
                foreach (var b in decisionBest.beginBases)
                {
                    b.Spawner.ReleaseUnits(decisionBest.targetBas.gameObject);
                }
            }
            else
            {
//                Debug.Log("releasing from one");
                decisionBest.beginBas.Spawner.ReleaseUnits(decisionBest.targetBas.gameObject);
            }
            // базы, на которые мы уже отправили юнитов, помечаем, чтобы не отправить снова туда
            if (decisionBest.targetBas.Owner.playerNumber == -1)
                basesToBeCaptured.Add(decisionBest.targetBas);
        }
//        else
//            Debug.Log("no best decision");
    }

    void FillDecisions()
    {
        decisions.Clear();
        foreach (var bas in GameController.Instance.bases)
        {
            foreach (var basSelf in bases)
            {
                if (bas == basSelf)
                    continue;
                decisions.Add(new AIDecision(bas, basSelf, this));
            }
            foreach (var basNeutral in basesNeutral)
            {
                if (basesToBeCaptured.Contains(basNeutral))
                    continue;
                decisions.Add(new AIDecision(bas, basNeutral, this));
            }
            foreach (var basToAttack in basesToAttack)
            {
                decisions.Add(new AIDecision(bas, basToAttack, this));
            }
        }
    }
    
    void FillDistances()
    {
        foreach (var basX in GameController.Instance.bases)
        {
            foreach (var basY in GameController.Instance.bases)
            {
                if (basX == basY)
                    continue;
                if (!basX.DictDistances.ContainsKey(basY))
                    basX.DictDistances.Add(basY, basX.Trans.position - basY.Trans.position);
            }
        }
//        Debug.Log("distances filled");
    }

    void DefineEnemies()
    {
        enemies.Clear();
        foreach (var player in GameController.Instance.playerController)
        {
            if (player.owner.playerNumber == -1)
                continue;
            if (player == owner.playerController)
                continue;
            if (!player.gameObject.activeSelf)
                continue;
            enemies.Add(player);
        }
    }

    void DefineBases()
    {
        bases.Clear();
        basesNeutral.Clear();
        basesToAttack.Clear();
        
        Vector2 sum = Vector2.zero;
        int count = 0;
        
        foreach (var b in GameController.Instance.bases)
        {
            if (b.Owner.playerNumber == owner.playerNumber)
            {
                bases.Add(b);
                sum += (Vector2)b.Trans.position;
                count++;
                if (basesToBeCaptured.Contains(b))
                    basesToBeCaptured.Remove(b);
            }
            else if (b.Owner.playerNumber == -1)
                basesNeutral.Add(b);
            else
            {
                basesToAttack.Add(b);
                if (basesToBeCaptured.Contains(b))
                    basesToBeCaptured.Remove(b);
            }
            // считаем кол-во юнитов рядом с базой
            FillUnitCountNearBase(b);
        }
        DefineBasesInFrontLine();
        if (count > 0)
            empireCenterPoint = sum / count;
    }
    
    void FillUnitCountNearBase(Planet bas)
    {
        // кол-во юнитов считается через коллизии и кешируется прямо в базе
        // размер круга, где ищутся юниты, зависит от размера коллайдера и коэффициента 'multiplier'
        float multiplier = 1.5f;
        List<CollisionCircle> unitsNearBas =
            CollisionManager.Instance.FindBodiesInCircleArea(bas.Trans.position, bas.Collider.radius * multiplier);
        int unitsNearBasSelf = 0;
        int unitsNearBasEnemies = 0;
        for (int i = 0; i < unitsNearBas.Count; i++)
        {
            if (unitsNearBas[i].owner.playerNumber == bas.Owner.playerNumber)
                unitsNearBasSelf++;
            else
                unitsNearBasEnemies++;
        }
        bas.UnitCountNearBasSelf = unitsNearBasSelf;
        bas.UnitCountNearBasEnemies = unitsNearBasEnemies;
    }
    
    void DefineBasesInFrontLine()
    {
        basesInFrontLine.Clear();
        foreach (var basToAttack in basesToAttack)
        {
            // сначала найдём ближайшую к противнику базу
            Planet closestToEnemy = null;
            float distMin = 99999;
            foreach (var basSelf in owner.playerController.bases)
            {
                float dist = (basSelf.Trans.position - basToAttack.Trans.position).sqrMagnitude;
                // если расстояние слишком большое, то пропускаем
                if (dist > distToFar)
                    continue;
                if (dist < distMin)
                {
                    distMin = dist;
                    closestToEnemy = basSelf;
                }
            }
            if (closestToEnemy != null)
            {
                // затем проверим есть ли рядом дружественные базы, если есть, то считаем что они расположены на линии фронта
                float distNeeded = (closestToEnemy.Trans.position - basToAttack.Trans.position).sqrMagnitude * 1.4f;
                foreach (var basSelf in owner.playerController.bases)
                {
                    float dist = (basSelf.Trans.position - basToAttack.Trans.position).sqrMagnitude;
                    // 
                    if (dist < distNeeded)
                    {
                        if (!basesInFrontLine.Contains(basSelf))
                            basesInFrontLine.Add(basSelf);
                    }
                }
            }
        }
    }

    void DefineFrontLinePoint()
    {
        Planet suitable = null;
        float distMin = 9999;
        
        foreach (var b in basesToAttack)
        {
            float dist = (empireCenterPoint - (Vector2)b.Trans.position).sqrMagnitude;
            if (dist < distMin * distMin)
            {
                distMin = dist;
                suitable = b;
            }
        }
        if (basesToAttack.Count > 0)
            frontLinePoint = (Vector2)suitable.Trans.position + empireCenterPoint;
    }

    private void OnDrawGizmos()
    {
//        Gizmos.color = Color.blue;
//        Gizmos.DrawSphere(empireCenterPoint, 0.3f);
//        Gizmos.color = Color.red;
//        Gizmos.DrawSphere(frontLinePoint, 0.3f);
        if (decisionBest != null && decisionBest.targetBas != null)
        {
            Gizmos.color = Color.white;
            if (decisionBest.beginBases != null)
            {
                foreach (var basBegin in decisionBest.beginBases)
                {
                    Gizmos.DrawLine(basBegin.Trans.position, decisionBest.targetBas.Trans.position);
                    Gizmos.DrawSphere(decisionBest.targetBas.Trans.position, 0.5f);
                }
            }
            else if (decisionBest.beginBas != null)
            {
                Gizmos.DrawLine(decisionBest.beginBas.Trans.position, decisionBest.targetBas.Trans.position);
                Gizmos.DrawSphere(decisionBest.targetBas.Trans.position, 0.5f);
            }
#if UNITY_EDITOR
            Handles.Label(decisionBest.targetBas.Trans.position, decisionBest.weight.ToString());
#endif
        }
        if (almostDecisions.Count > 0)
        {
            Gizmos.color = Color.gray;
            foreach (var decision in almostDecisions)
            {
                Gizmos.DrawLine(decision.beginBas.Trans.position, decision.targetBas.Trans.position);
                Gizmos.DrawSphere(decision.targetBas.Trans.position, 0.5f);
                
#if UNITY_EDITOR
                Handles.Label(decision.targetBas.Trans.position, decision.weight.ToString());
#endif
            }
        }
    }
}
