using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters;
using UnityEngine;

public class AIPlayer : MonoBehaviour
{
    public int difficulty;

    public Owner owner;

    public float decisionInterval = 3;

    public List<PlayerController> enemies = new List<PlayerController>();
    public List<Base> bases = new List<Base>();
    public List<Base> basesNeutral = new List<Base>();
    public List<Base> basesToBeCaptured = new List<Base>();
    public List<Base> basesToAttack = new List<Base>();
    public Vector2 frontLinePoint;
    public Vector2 empireCenterPoint;
    Base basMain;
    
    public List<AIDecision> decisions = new List<AIDecision>();
    public AIDecision decisionBest = null;
    public List<AIDecision> almostDecisions = new List<AIDecision>();

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
            Debug.LogFormat(decision.weight.ToString());
            if (decision.weight > weightMax)
            {
                Debug.Log("decision weight: " + decision.weight);
                weightMax = decision.weight;
                decisionBest = decision;
            }
            if (decision.weight > 0 && decision.weight < weightMax)
                almostDecisions.Add(decision);
        }
        if (decisionBest != null)
        {
            if (decisionBest.beginBas == decisionBest.targetBas)
                Debug.Log("fuck");
            // проверяем можем ли мы запустить юнитов сразу из нескольких баз
            if (decisionBest.beginBases != null)
            {
                foreach (var b in decisionBest.beginBases)
                {
                    b.spawner.ReleaseUnits(decisionBest.targetBas.gameObject);
                }
            }
            else
            {
                decisionBest.beginBas.spawner.ReleaseUnits(decisionBest.targetBas.gameObject);
            }
            // базы, на которые мы уже отправили юнитов, помечаем, чтобы не отправить снова туда
            if (decisionBest.targetBas.owner.playerNumber == -1)
                basesToBeCaptured.Add(decisionBest.targetBas);
        }
        else
            Debug.Log("no best decision");
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
            
//            // move to neutral base first if present
//            if (basesNeutral.Count > 0)
//            {
//                if (b.spawner.unitCount > 15)
//                {
//                    Base targetBas = FindSuitableBaseToGo(b, basesNeutral);
//                    if (targetBas != null)
//                    {
//                        b.spawner.ReleaseUnits(targetBas.gameObject);
//                        basesToBeCaptured.Add(targetBas);
//                    }
//                }
//            }
//            // else move units to front line or go attack
//            else
//            {
//                Base targetBas = FindSuitableBaseToAttack(b);
//                if (targetBas != null)
//                {
//                    b.spawner.ReleaseUnits(targetBas.gameObject);
////                        basesToBeCaptured.Add(targetBas);
//                }
//                else
//                {
//                    if (b.spawner.unitCount > 5)
//                    {
//                        targetBas = FindSuitableBaseToMove(frontLinePoint, bases);
//                        if (targetBas != null)
//                        {
//                            b.spawner.ReleaseUnits(targetBas.gameObject);
////                                basesToBeCaptured.Add(targetBas);
//                        }
//                    }
//                }
//            }
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
                if (!basX.dictDistances.ContainsKey(basY))
                    basX.dictDistances.Add(basY, basX.trans.position - basY.trans.position);
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
            if (b.owner.playerNumber == owner.playerNumber)
            {
                bases.Add(b);
                sum += (Vector2)b.trans.position;
                count++;
                if (basesToBeCaptured.Contains(b))
                    basesToBeCaptured.Remove(b);
            }
            else if (b.owner.playerNumber == -1)
                basesNeutral.Add(b);
            else
            {
                basesToAttack.Add(b);
                if (basesToBeCaptured.Contains(b))
                    basesToBeCaptured.Remove(b);
            }
        }
        if (count > 0)
            empireCenterPoint = sum / count;
    }

    void DefineFrontLinePoint()
    {
        Base suitable = null;
        float distMin = 9999;
        
        foreach (var b in basesToAttack)
        {
            float dist = (empireCenterPoint - (Vector2)b.trans.position).sqrMagnitude;
            if (dist < distMin * distMin)
            {
                distMin = dist;
                suitable = b;
            }
        }
        if (basesToAttack.Count > 0)
            frontLinePoint = (Vector2)suitable.trans.position + empireCenterPoint;
    }

    Base FindSuitableBaseToGo(Base basFrom, List<Base> whichList)
    {
        Base suitable = null;
        float distMin = 9999;
        foreach (var b in whichList)
        {
            if (basesToBeCaptured.Contains(b))
                continue;
            float dist = (basFrom.trans.position - b.trans.position).sqrMagnitude;
            if (dist < distMin * distMin)
            {
                distMin = dist;
                suitable = b;
            }
        }
        return suitable;
    }
    
    Base FindSuitableBaseToMove(Vector2 point, List<Base> whichList)
    {
        Base suitable = null;
        float distMin = 9999;
        foreach (var b in whichList)
        {
            if (basesToBeCaptured.Contains(b))
                continue;
            float dist = (point - (Vector2)b.trans.position).sqrMagnitude;
            if (dist < distMin * distMin)
            {
                distMin = dist;
                suitable = b;
            }
        }
        return suitable;
    }
    
    Base FindSuitableBaseToAttack(Base basFrom)
    {
        Base suitable = null;
        float distMin = 9999;
        foreach (var b in basesToAttack)
        {
            if (basFrom.spawner.unitCount < b.spawner.unitCount * 0.9)
                continue;
            float dist = (empireCenterPoint - (Vector2)b.trans.position).sqrMagnitude;
            if (dist < distMin * distMin)
            {
                distMin = dist;
                suitable = b;
            }
        }
        return suitable;
    }

    IEnumerator TakeDecision2()
    {
        yield return new WaitForSeconds(decisionInterval);
        foreach (var b in GameController.Instance.bases)
        {
            if (b.owner.playerNumber == owner.playerNumber)
            {
                basMain = b;
                break;
            }
        }
        while(true)
        {
//            Debug.Log("take decision iteration");
            DefineEnemies();
            DefineBases();
            DefineFrontLinePoint();
            
            foreach (var b in bases)
            {
                // move to neutral base first if present
                if (basesNeutral.Count > 0)
                {
                    if (b.spawner.unitCount > 15)
                    {
                        Base targetBas = FindSuitableBaseToGo(b, basesNeutral);
                        if (targetBas != null)
                        {
                            b.spawner.ReleaseUnits(targetBas.gameObject);
                            basesToBeCaptured.Add(targetBas);
                        }
                    }
                }
                // else move units to front line or go attack
                else
                {
                    Base targetBas = FindSuitableBaseToAttack(b);
                    if (targetBas != null)
                    {
                        b.spawner.ReleaseUnits(targetBas.gameObject);
//                        basesToBeCaptured.Add(targetBas);
                    }
                    else
                    {
                        if (b.spawner.unitCount > 5)
                        {
                            targetBas = FindSuitableBaseToMove(frontLinePoint, bases);
                            if (targetBas != null)
                            {
                                b.spawner.ReleaseUnits(targetBas.gameObject);
//                                basesToBeCaptured.Add(targetBas);
                            }
                        }
                    }
                }
            }
            
            yield return new WaitForSeconds(decisionInterval);
        }
    }

    private void OnDrawGizmos()
    {
//        Gizmos.color = Color.blue;
//        Gizmos.DrawSphere(empireCenterPoint, 0.3f);
//        Gizmos.color = Color.red;
//        Gizmos.DrawSphere(frontLinePoint, 0.3f);
        if (decisionBest != null)
        {
            Gizmos.color = Color.white;
            if (decisionBest.beginBases != null)
            {
                foreach (var basBegin in decisionBest.beginBases)
                {
                    Gizmos.DrawLine(basBegin.trans.position, decisionBest.targetBas.trans.position);
                    Gizmos.DrawSphere(decisionBest.targetBas.trans.position, 0.5f);
                }
            }
            else
            {
                Gizmos.DrawLine(decisionBest.beginBas.trans.position, decisionBest.targetBas.trans.position);
                Gizmos.DrawSphere(decisionBest.targetBas.trans.position, 0.5f);
            }
        }
        if (almostDecisions.Count > 0)
        {
            Gizmos.color = Color.gray;
            foreach (var decision in almostDecisions)
            {
                Gizmos.DrawLine(decision.beginBas.trans.position, decision.targetBas.trans.position);
                Gizmos.DrawSphere(decision.targetBas.trans.position, 0.5f);
            }
        }
    }
}
