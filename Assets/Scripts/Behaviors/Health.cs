using UnityEngine;

[System.Serializable]
public class Health
{
    public int Max;
    public readonly int MaxNoBonuses;
    public int Current;
    [Range(0.0f, 1.0f)]
    public float Percent;

    public Health(int newMax)
    {
        Max = newMax;
        MaxNoBonuses = Max;
        Current = Max;
        Percent = 1;
    }

    public void Reset()
    {
        if (Max == Current)
        {
            return;
        }
        Max = MaxNoBonuses;
        Current = Max;
        Percent = 1;
    }
    
}
