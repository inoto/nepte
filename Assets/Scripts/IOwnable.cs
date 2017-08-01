using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IOwnable  {

    int GetOwner();
    void SetOwner(int owner);
    GameObject GetGameObject();
}
