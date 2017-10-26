using System.Collections.Generic;
using UnityEngine;

public class CardManager : MonoBehaviour
{
    private static CardManager _instance;

    public static CardManager Instance { get { return _instance; } }

    [HideInInspector] public Bounds CardActivationBounds;
    [HideInInspector] public UISprite DeniedArea;
    private GameObject cardContainer;
    
    private List<Card> hand;
    public int HandSize;
    public List<GameObject> Cards;

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
        
        CardActivationBounds = GameObject.Find("CardActivationArea").GetComponent<BoxCollider>().bounds;
        if (CardActivationBounds == null)
        {
            Debug.LogError("card activation area not found");
        }

        DeniedArea = GameObject.Find("DeniedArea").GetComponent<UISprite>();
        if (DeniedArea == null)
        {
            Debug.LogError("denied area not found");
        }

        cardContainer = GameObject.Find("ContainerCards");
    }

    private void Start()
    {
        GameManager.Instance.OnGameRestart += ResetCooldowns;
    }

    private void ResetCooldowns()
    {
        for (int i = 0; i < cardContainer.transform.childCount; i++)
        {
            Card tmpCard = cardContainer.transform.GetChild(i).GetComponent<Card>();
            if (tmpCard != null)
            {
                tmpCard.InCooldown = false;
            }
        }
    }
}