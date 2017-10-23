using UnityEngine;
using System.Collections;

[System.Serializable]
public class Health
{
    public int max;
    public int maxNoBonuses;
    public int current;
    [Range(0.0f, 1.0f)]
    public float percent;

    public Health(int _max)
    {
        max = _max;
        maxNoBonuses = max;
        current = max;
        percent = 1;
    }

    public void Reset()
    {
        if (max == current)
            return;
        max = maxNoBonuses;
        current = max;
        percent = 1;
    }
    
}
