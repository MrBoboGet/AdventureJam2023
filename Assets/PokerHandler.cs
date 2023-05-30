using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum DropType
{
    Table,
    Opponent
}


public class NPCOpponent
{
    public Sprite Neutral;
    public Sprite Tell;

    public Sprite PickHand;
    public Sprite HandGrab;


    public List<Card> Hand;
}



public class PokerHandler : MonoBehaviour
{
    class PickHandler
    {
        public List<GameObject> ObjectsToArrange;
        public bool PlayerPicking = true;
        public NPCOpponent Opponent;

        int m_PickedCardIndex = -1;


        GameObject m_OpponentObject = null;
        //returns index of picked card
        public int Update()
        {
            return m_PickedCardIndex;
        }
        public void Initialize()
        {
            //get scene object
            GameObject CardScene = GameObject.FindGameObjectWithTag("PickScene");
            m_OpponentObject = GameObject.FindGameObjectWithTag("Opponent");
            //set opponent sprite
            m_OpponentObject.GetComponent<UnityEngine.UI.Image>().sprite = Opponent.Neutral;
            Canvas AssociatedCanvas = CardScene.GetComponentInChildren<Canvas>();
            foreach(GameObject Object in ObjectsToArrange)
            {
                Object.transform.parent = AssociatedCanvas.gameObject.transform;
            }
            for(int i = 0; i < 5;i++)
            {
                new Vector2(-400 + i * 200, -100);
            }

        }
        public void HoverEnter(int CardIndex)
        {
            if(CardIndex == 2)
            {
                m_OpponentObject.GetComponent<UnityEngine.UI.Image>().sprite = Opponent.Tell;
            }
            else
            {
                m_OpponentObject.GetComponent<UnityEngine.UI.Image>().sprite = Opponent.Neutral;
            }
        }
        
        public void CardClicked(int Index)
        {
            m_PickedCardIndex = Index;
        }
    }
    // Start is called before the first frame update
    public GameObject CardPrefab;
    public GameObject SelectCardScene;
    public NPCOpponent TempOpponent;

    PickHandler m_PickHandler = null;

    Deck m_AssociatedDeck;
    List<GameObject> m_Hand;

    Canvas m_GlobalCanvas;
    GameObject m_SceneObject;



    int m_DiscardedCard = 0;
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
            TempOpponent.Hand.Add(m_AssociatedDeck.DrawCard());
        }
    }
    public void CardClicked(CardScript AssociatedCard)
    {
        if(m_PickHandler != null)
        {
            m_PickHandler.CardClicked(AssociatedCard.CardIndex);
        }
    }
    public void CardDropped(CardScript AssociatedCard,DropType Type)
    {
        if(Type == DropType.Opponent)
        {
            //AssociatedCard.ResetPosition();
            //print("Opponent");
            m_PickHandler = new PickHandler();
            m_PickHandler.PlayerPicking = true;
            m_PickHandler.Opponent = TempOpponent;
            //create cards
            m_SceneObject.SetActive(false);
            Instantiate(SelectCardScene);
            for(int i = 0; i < 5;i++)
            {
                GameObject NewCard = Instantiate(CardPrefab);
                CardScript AssociatedScript = NewCard.GetComponent<CardScript>();
                AssociatedScript.Hover = false;
                m_PickHandler.ObjectsToArrange.Add(AssociatedScript.gameObject);
                //AssociatedScript.gameObject.transform.parent = m_GlobalCanvas.gameObject.transform;
            }
            m_PickHandler.Initialize();
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
        if(m_PickHandler != null)
        {
            int CardIndex = m_PickHandler.Update();
            if(CardIndex != -1)
            {
                Card PickedCard = TempOpponent.Hand[CardIndex];
                TempOpponent.Hand[CardIndex] = m_AssociatedDeck.DrawCard();
                CardScript PlayerCard = m_Hand[CardIndex].GetComponent<CardScript>();
                PlayerCard.CardValue = PickedCard;
                m_Hand[CardIndex].GetComponent<UnityEngine.UI.Image>().sprite = m_AssociatedDeck.GetSprite(PlayerCard.CardValue);
            }
        }
    }
}
