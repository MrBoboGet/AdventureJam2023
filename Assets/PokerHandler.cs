using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum DropType
{
    Table,
    Opponent
}

[System.Serializable]
public class NPCOpponent
{
    public Sprite Neutral;
    public Sprite Tell;

    public Sprite PickHand;
    public Sprite HandGrab;


    public List<Card> Hand = new List<Card>();
}



public class PokerHandler : MonoBehaviour
{
    public GameObject CardObject;
    class HandAnimation
    {
        public void Initialize()
        {
            //get scene object
            GameObject CardScene = GameObject.FindGameObjectWithTag("OpponentPickScene");
            AssociatedObject = GameObject.FindGameObjectWithTag("Hand");
            //set opponent sprite
            Canvas AssociatedCanvas = CardScene.GetComponentInChildren<Canvas>();
            foreach (GameObject Object in m_CardPositions)
            {
                Object.transform.parent = AssociatedCanvas.gameObject.transform;
            }
            for (int i = 0; i < m_CardPositions.Count; i++)
            {
                m_CardPositions[i].transform.localPosition = new Vector2(-400 + i * 200, -260);
                m_CardPositions[i].GetComponent<RectTransform>().sizeDelta = new Vector2(175, m_CardPositions[i].GetComponent<RectTransform>().sizeDelta.y);
            }
        }
        public HandAnimation(List<GameObject> CardPositions)
        {
            m_CardPositions = CardPositions;
        }

        GameObject AssociatedObject;
        List<GameObject> m_CardPositions;
        float m_HoverLength= 3;
        float m_HoverSpeed = 8f;
        float m_GrabDelay = 0.5f;
        float m_GrabSpeed = 30f;
        float m_GrabYLocation = 1f;
        float m_ElapsedAnimation = 0;
        int m_CurrentCardTarget = 0;

        float m_ElapsedGrabTime= 0;

        int m_GrabbedCardIndex = -1;

        float m_GrabbTransitionDelay = 1f;
        float m_ElapsedGrabTransition = 0;
        public int Update()
        {
            if (AssociatedObject == null)
            {
                return (-1);
            }
            Vector2 TargetDestination = m_CardPositions[m_CurrentCardTarget].transform.position;
            TargetDestination = FindObjectOfType<Camera>().ScreenToWorldPoint(TargetDestination);
            if (m_ElapsedAnimation < m_HoverLength ||  Mathf.Abs(TargetDestination.x - AssociatedObject.transform.position.x) > 0.1f)
            {
                //only use X
                float XDiff = TargetDestination.x - AssociatedObject.transform.position.x;

                float XToAdd = Mathf.Min(Mathf.Abs(XDiff), m_HoverSpeed*Time.deltaTime) * Mathf.Sign(XDiff);
                AssociatedObject.transform.position += new Vector3(XToAdd, 0);
                if (Mathf.Abs(XToAdd) != m_HoverSpeed * Time.deltaTime)
                {
                    int CurrentTarget = m_CurrentCardTarget;
                    while (CurrentTarget == m_CurrentCardTarget)
                    {
                        m_CurrentCardTarget = (int)Random.Range(0.0f, 4.999f);
                    }
                }
                m_ElapsedAnimation += Time.deltaTime;
            }
            else
            {
                m_ElapsedGrabTime += Time.deltaTime;
                if (m_ElapsedGrabTime > m_GrabDelay)
                {
                    if (Mathf.Abs(AssociatedObject.transform.position.y - m_GrabYLocation) < 0.1f)
                    {
                        m_ElapsedGrabTransition += Time.deltaTime;
                        if (m_ElapsedGrabTransition > m_GrabbTransitionDelay)
                        {
                            m_GrabbedCardIndex = 1;
                        }
                    }
                    else
                    {
                        float YDiff = m_GrabYLocation - AssociatedObject.transform.position.y;
                        float YDiffToAdd = Mathf.Min(m_GrabSpeed * Time.deltaTime, Mathf.Abs(YDiff)) * Mathf.Sign(YDiff);
                        AssociatedObject.transform.position += new Vector3(0, YDiffToAdd);
                    }
                }
            }
            return (m_GrabbedCardIndex);
        }

    }
    class PickHandler
    {
        public List<GameObject> ObjectsToArrange = new List<GameObject>();
        public bool PlayerPicking = true;
        public NPCOpponent Opponent;

        int m_PickedCardIndex = -1;


        GameObject m_OpponentObject = null;
        GameObject m_HandObject = null;
        //returns index of picked card
        int m_FinessIndex = -1;
        public int Update()
        {
            if(!PlayerPicking)
            {
                if(m_FinessIndex != -1)
                {
                    m_FinessIndex -= 1;
                }
            }
            return m_PickedCardIndex;
        }
        public void Initialize()
        {
            //get scene object
            GameObject CardScene = GameObject.FindGameObjectWithTag("PickScene");
            m_OpponentObject = GameObject.FindGameObjectWithTag("Opponent");
            //set opponent sprite
            m_OpponentObject.GetComponent<SpriteRenderer>().sprite = Opponent.Neutral;
            Canvas AssociatedCanvas = CardScene.GetComponentInChildren<Canvas>();
            foreach(GameObject Object in ObjectsToArrange)
            {
                Object.transform.parent = AssociatedCanvas.gameObject.transform;
            }
            for(int i = 0; i < ObjectsToArrange.Count;i++)
            {
                ObjectsToArrange[i].transform.localPosition = new Vector2(-400 + i * 200, -260);
                ObjectsToArrange[i].GetComponent<RectTransform>().sizeDelta = new Vector2(175, ObjectsToArrange[i].GetComponent<RectTransform>().sizeDelta.y);
            }

        }
        public void HoverEnter(int CardIndex)
        {
            if(m_OpponentObject == null)
            {
                return;
            }
            if(CardIndex == 2)
            {
                m_OpponentObject.GetComponent<SpriteRenderer>().sprite = Opponent.Tell;
            }
            else
            {
                m_OpponentObject.GetComponent<SpriteRenderer>().sprite = Opponent.Neutral;
            }
        }
        public void HoverLeave(int CardIndex)
        {
            if (m_OpponentObject == null)
            {
                return;
            }
            m_OpponentObject.GetComponent<SpriteRenderer>().sprite = Opponent.Neutral;
        }

        public void CardClicked(int Index)
        {
            m_PickedCardIndex = Index;
        }
    }
    // Start is called before the first frame update
    public GameObject CardPrefab;
    public GameObject SelectCardScene;
    public GameObject OpponentSelectCardScene;
    public NPCOpponent TempOpponent = new NPCOpponent();

    PickHandler m_PickHandler = null;
    HandAnimation m_OpponentPickHandler = null;

    Deck m_AssociatedDeck;
    List<GameObject> m_Hand;

    Canvas m_GlobalCanvas;
    GameObject m_SceneObject;

    class PokerState
    {
        public int OpponentCash = 15;
        public int PlayerCash = 15;
        public int OpponentPot = 0;
        public int PlayerPot = 0;
        public bool PlayerTurn = true;
        public bool Call = false;
    }
    class OpponentTurnState
    {
        public float ThinkTime = 2f;
        public float ElapsedThink = 0;
    }


    PokerState m_CurrentPokerState = new PokerState();
    OpponentTurnState m_CurrentOpponentState = new OpponentTurnState();

    int m_DiscardedCard = 0;
    Vector2 GetPosition(int CardIndex)
    {
        return (new Vector2(-400 + CardIndex*200, -350));
    }

    void Oof()
    {
        print("Oof");
    }
    void p_InitializeNewRound()
    {

    }
    void p_InitializeRevealSequence()
    {

    }
    void p_InitializeDefeat()
    {

    }
    void p_InitializeWin()
    {

    }


    public void ShowOpponentDialog(string DialogToShow)
    {

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
        m_SceneObject = GameObject.FindGameObjectWithTag("PokerScene");
        m_AssociatedDeck = FindObjectOfType<Deck>();
        m_GlobalCanvas = m_SceneObject.GetComponentInChildren<Canvas>();
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
    public void CardHoverEnter(CardScript AssociatedCard)
    {
        if(m_PickHandler != null)
        {
            m_PickHandler.HoverEnter(AssociatedCard.CardIndex);
        }
    }
    public void CardHoverLeave(CardScript AssociatedCard)
    {
        if (m_PickHandler != null)
        {
            m_PickHandler.HoverLeave(AssociatedCard.CardIndex);
        }
    }
    private IEnumerator p_ThrowCard(Vector2 Origin,Vector2 Target,Sprite AssociatedSprite)
    {
        GameObject CardInScene = Instantiate(CardObject);
        CardInScene.transform.parent = m_SceneObject.transform;
        CardInScene.transform.position = Origin;
        CardInScene.GetComponent<SpriteRenderer>().sprite = AssociatedSprite;
        

        float ThrowSpeed = 6;
        float RotationSpeed = 135f;
        CardInScene.transform.eulerAngles = new Vector3(60, 0, Random.Range(0f, 360f));
        while( ((Vector2)CardInScene.transform.position - Target).magnitude >= 0.01f)
        {
            Vector2 ThrowDirection = Target- (Vector2)CardInScene.transform.position;
            ThrowDirection = ThrowDirection.normalized * Mathf.Min(ThrowSpeed * Time.deltaTime, ThrowDirection.magnitude);
            CardInScene.transform.position += (Vector3)ThrowDirection;
            CardInScene.transform.eulerAngles += new Vector3(0, 0,RotationSpeed*Time.deltaTime);
            yield return null;
        }
    }
    int m_ReplacedCardIndex = 0;
    public void CardDropped(CardScript AssociatedCard,DropType Type)
    {
        if(!m_CurrentPokerState.PlayerTurn || m_CurrentPokerState.Call)
        {
            AssociatedCard.ResetPosition();
            Oof();
            return;
        }
        if(Type == DropType.Opponent)
        {
            //AssociatedCard.ResetPosition();
            //print("Opponent");
            m_ReplacedCardIndex = AssociatedCard.CardIndex;
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
                AssociatedScript.CardIndex = i;
                AssociatedScript.Hover = false;
                m_PickHandler.ObjectsToArrange.Add(AssociatedScript.gameObject);
                AssociatedScript.GetComponent<UnityEngine.UI.Image>().sprite = m_AssociatedDeck.GetCardBack();
                AssociatedScript.AssociatedHandler = this;
                //AssociatedScript.gameObject.transform.parent = m_GlobalCanvas.gameObject.transform;
            }
            m_PickHandler.Initialize();
            AssociatedCard.ResetPosition();
        }
        else if(Type == DropType.Table)
        {
            Vector2 Target =FindObjectOfType<Camera>().ScreenToWorldPoint(AssociatedCard.gameObject.transform.position);
            Vector2 Origin = new Vector2(0, -5);
            Sprite SpriteToThrow = AssociatedCard.gameObject.GetComponent<UnityEngine.UI.Image>().sprite;

            StartCoroutine(p_ThrowCard(Origin, Target, SpriteToThrow));

            AssociatedCard.Drop();
            //replace card
            DrawCard(AssociatedCard.CardIndex);
        }
        m_CurrentPokerState.PlayerTurn = false;
    }

    public void OnRaise()
    {
        int NewTotalPot = m_CurrentPokerState.OpponentPot + 2;
        int TotalRaise = NewTotalPot - m_CurrentPokerState.PlayerPot;
        m_CurrentPokerState.PlayerPot += TotalRaise;
        m_CurrentPokerState.PlayerCash -= TotalRaise;
    }
    public void OnMatch()
    {
        int NewTotalPot = m_CurrentPokerState.OpponentPot;
        int TotalRaise = NewTotalPot - m_CurrentPokerState.PlayerPot;
        m_CurrentPokerState.PlayerPot += TotalRaise;
        m_CurrentPokerState.PlayerCash -= TotalRaise;
    }
    public void OnFold()
    {
        m_CurrentPokerState.OpponentCash += m_CurrentPokerState.PlayerPot + m_CurrentPokerState.OpponentPot;
        m_CurrentPokerState.PlayerPot = 0;
        m_CurrentPokerState.OpponentPot = 0;
        p_InitializeNewRound();
    }
    public void OnCall()
    {

    }

    void p_InitializeOpponentPick()
    {
        //AssociatedCard.ResetPosition();
        //print("Opponent");
        //create cards
        m_SceneObject.SetActive(false);
        Instantiate(OpponentSelectCardScene);
        List<GameObject> CardObjects = new List<GameObject>();
        for (int i = 0; i < 5; i++)
        {
            GameObject NewCard = Instantiate(CardPrefab);
            CardScript AssociatedScript = NewCard.GetComponent<CardScript>();
            AssociatedScript.CardIndex = i;
            AssociatedScript.Hover = true;
            AssociatedScript.GetComponent<UnityEngine.UI.Image>().sprite = m_Hand[i].GetComponent<UnityEngine.UI.Image>().sprite;
            AssociatedScript.AssociatedHandler = this;
            CardObjects.Add(NewCard);
        }
        m_OpponentPickHandler = new HandAnimation(CardObjects);
    }

    bool m_Initialised = false;
    // Update is called once per frame
    void Update()
    {
        if(m_PickHandler != null)
        {
            if(!m_Initialised)
            {
                m_PickHandler.Initialize();
                m_Initialised = true;
            }
            int CardIndex = m_PickHandler.Update();
            if(CardIndex != -1)
            {
                Card PickedCard = TempOpponent.Hand[CardIndex];
                TempOpponent.Hand[CardIndex] = m_AssociatedDeck.DrawCard();
                CardScript PlayerCard = m_Hand[CardIndex].GetComponent<CardScript>();
                PlayerCard.CardValue = PickedCard;
                m_Hand[m_ReplacedCardIndex].GetComponent<UnityEngine.UI.Image>().sprite = m_AssociatedDeck.GetSprite(PlayerCard.CardValue);

                m_SceneObject.SetActive(true);

                Destroy(GameObject.FindGameObjectWithTag("PickScene"));
                m_PickHandler = null;
            }
        }
        else if(m_OpponentPickHandler != null)
        {
            if(!m_Initialised)
            {
                m_OpponentPickHandler.Initialize();
                m_Initialised = true;
            }
            int GrabbedCardIndex = m_OpponentPickHandler.Update();
            if(GrabbedCardIndex != -1)
            {
                DrawCard(GrabbedCardIndex);
                m_SceneObject.SetActive(true);
                Destroy(GameObject.FindGameObjectWithTag("OpponentPickScene"));
                m_OpponentPickHandler = null;
            }
        }
        else 
        {
            if(!m_CurrentPokerState.PlayerTurn)
            {
                m_CurrentOpponentState.ElapsedThink += Time.deltaTime;
                if(m_CurrentOpponentState.ElapsedThink >= m_CurrentOpponentState.ThinkTime)
                {
                    //make move
                    if(Random.Range(0,2) < 1f)
                    {
                        //take card
                    }
                    else
                    {
                        //do nothing
                        p_InitializeOpponentPick();
                    }
                    m_CurrentPokerState.PlayerTurn = true;
                }
            }
            else
            {
                m_CurrentOpponentState.ElapsedThink = 0;
            }
            m_Initialised = false;
        }


        //Test pick handler
        if(Input.GetKeyDown(KeyCode.P))
        {
            p_InitializeOpponentPick();
        }
    }
}
