using UnityEngine;

public class CardManager : MonoBehaviour
{
    private static CardManager _instance;

    public static CardManager Instance { get { return _instance; } }

    public Bounds cardActivationBounds;
    
    private Card[] cards;

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }
        
        cardActivationBounds = GameObject.Find("CardActivationArea").GetComponent<BoxCollider>().bounds;
    }
    
    
}