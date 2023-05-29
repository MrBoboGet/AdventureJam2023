using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum DropType
{
    Table,
    Opponent
}
public class PokerHandler : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject CardPrefab;

    Deck m_AssociatedDeck;
    List<GameObject> m_Hand;

    Canvas m_GlobalCanvas;

    Vector2 GetPosition(int CardIndex)
    {
        return (new Vector2(-400 + CardIndex*200, -350));
    }

    void DrawCard(int CardIndex)
    {
        GameObject NewCard = Instantiate(CardPrefab);
        CardScript AssociatedScript = NewCard.GetComponent<CardScript>();
        AssociatedScript.gameObject.transform.parent = m_GlobalCanvas.gameObject.transform;
        AssociatedScript.CardIndex = CardIndex;
        AssociatedScript.SetPosition(GetPosition(CardIndex));
        AssociatedScript.CardValue = m_AssociatedDeck.DrawCard();
        AssociatedScript.AssociatedHandler = this;
        AssociatedScript.GetComponent<UnityEngine.UI.Image>().sprite = m_AssociatedDeck.GetSprite(AssociatedScript.CardValue);
        m_Hand[CardIndex] = NewCard;
    }
    void Start()
    {
        m_Hand = new List<GameObject>();
        for(int i = 0; i < 5;i++)
        {
            m_Hand.Add(null);
        }
        m_AssociatedDeck = FindObjectOfType<Deck>();
        m_GlobalCanvas = FindObjectOfType<Canvas>();
        //create hand
        for(int i = 0; i < 5;i++)
        {
            DrawCard(i);
        }
    }
    public void CardDropped(CardScript AssociatedCard,DropType Type)
    {
        if(Type == DropType.Opponent)
        {
            AssociatedCard.ResetPosition();
        }
        else if(Type == DropType.Table)
        {
            AssociatedCard.Drop();
            //replace card
            DrawCard(AssociatedCard.CardIndex);
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
