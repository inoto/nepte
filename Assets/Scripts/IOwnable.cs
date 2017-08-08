using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IOwnable 
{
    void AddAttacker(GameObject newObj);
    bool IsActive();
    int GetOwner();
    void SetOwner(int owner);
    GameObject GetGameObject();
}
