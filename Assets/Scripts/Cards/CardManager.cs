using System.Collections.Generic;
using UnityEngine;

public class CardManager : MonoBehaviour
{
    private static CardManager _instance;

    public static CardManager Instance { get { return _instance; } }

    [HideInInspector] public Bounds cardActivationBounds;
    [HideInInspector] public UISprite deniedArea;
    private GameObject cardContainer;
    
    private List<Card> hand;
    public int handSize;
    public List<GameObject> cards;

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
        if (cardActivationBounds == null)
            Debug.LogError("card activation area not found");
        
        deniedArea = GameObject.Find("DeniedArea").GetComponent<UISprite>();
        if (deniedArea == null)
            Debug.LogError("denied area not found");
        
        cardContainer = GameObject.Find("ContainerCards");
    }

    private void Start()
    {
        GameManager.Instance.OnGameRestart += ResetCooldowns;
    }

    void ResetCooldowns()
    {
        for (int i = 0; i < cardContainer.transform.childCount; i++)
        {
            Card tmpCard = cardContainer.transform.GetChild(i).GetComponent<Card>();
            if (tmpCard != null)
                tmpCard.inCooldown = false;
        }
    }
}