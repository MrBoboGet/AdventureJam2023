using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.EventSystems;
public enum DropType
{
    Table,
    Opponent
}

public class Move
{

}

public static class GlobalTransitionInfo
{
    public static UnityEngine.SceneManagement.Scene BattleScene;
}
[System.Serializable]
public class TransitionInfo
{
    public UnityEngine.SceneManagement.Scene BattleScene;
    public UnityEngine.SceneManagement.Scene LoseScene;
    public UnityEngine.SceneManagement.Scene NextScene;
}
public class Move_StealCard : Move
{
    int DiscardedCardIndex = 0;
}
public class Move_DiscardCards : Move
{
    public List<Card> DiscardedCards = new List<Card>();
}

public class Move_Fold : Move
{

}
public class Move_Raise : Move
{

}
public class Move_Match : Move
{

}
public class Move_Call : Move
{

}

public class PokerState
{
    public int OpponentCash = 15;
    public int PlayerCash = 15;
    public int OpponentPot = 0;
    public int PlayerPot = 0;
    public bool PlayerTurn = true;
    public bool Call = false;
    public int TurnCount = 0;
    public int DiscardedCards = 0;

    public int PlayerHP = 3;
    public int OpponentHP = 3;

    public List<Card> PlayerHand = new List<Card>();
    public List<Card> OpponentHand = new List<Card>();
    public Deck AssociatedDeck;
}

[System.Serializable]
public class NPCOpponent
{
    public Sprite Neutral;
    public Sprite Tell;
    public Sprite PickHand;
    public Sprite HandGrab;
}



public static class GameInfo
{
    public static GameObject Opponent = null;
    public static Sprite BGSprite = null;
}

public class PokerHandler : MonoBehaviour
{
    public GameObject CardObject;

    RaiseMenu m_BettingMenu;

    public TransitionInfo Transitions;
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
            //set opponent sprite
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
                //m_OpponentObject.GetComponent<SpriteRenderer>().sprite = Opponent.Tell;
            }
            else
            {
                //m_OpponentObject.GetComponent<SpriteRenderer>().sprite = Opponent.Neutral;
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
    public GameObject RevealCardsScene;
    public AudioClip OofSound;

    public NPCOpponent TempOpponent = new NPCOpponent();

    PickHandler m_PickHandler = null;
    OpponentScript.HandAnimation m_OpponentPickHandler = null;

    Deck m_AssociatedDeck;
    //List<GameObject> m_Hand;
    List<GameObject> m_HandObjects;

    Canvas m_GlobalCanvas;
    GameObject m_SceneObject;

    class OpponentTurnState
    {
        public float ThinkTime = 2f;
        public float ElapsedThink = 0;
    }


    PokerState m_CurrentPokerState = new PokerState();
    OpponentTurnState m_CurrentOpponentState = new OpponentTurnState();

    bool m_PokerPaused = false;

    int m_DiscardedCard = 0;
    Vector2 GetPosition(int CardIndex)
    {
        //return (new Vector2(-400 + CardIndex*200, -350));
        return (new Vector2(-500 + CardIndex*250, -575));
    }

    
    void Oof()
    {
        m_StressObject.IncreaseStress();
        AudioSource.PlayClipAtPoint(OofSound, new Vector3());
        print("Oof");
    }

    IEnumerator p_WaitForAnimation() 
    {
        int t = 0;
        while(t < 5)
        {
            t += 1;
            yield return null;
        }
        while(m_OpponentObject.InAnimation())
        {
            yield return null;
        }
        //destroy cards
        GameObject[] Objects = GameObject.FindGameObjectsWithTag("Card");
        foreach(GameObject Object in Objects)
        {
            Destroy(Object);
        }
        m_CurrentPokerState.AssociatedDeck.ResetDeck();
        for(int i = 0; i < 5;i++)
        {
            DrawCard(i);
            m_CurrentPokerState.OpponentHand[i] = m_CurrentPokerState.AssociatedDeck.DrawCard(); 
        }

        m_PokerPaused = false;

        //check if anyone has negative hp
        if(m_CurrentPokerState.PlayerCash < 0)
        {
            m_PokerPaused = true;
            FindObjectOfType<LifeContainer>().DecreasePlayerHP();
            m_CurrentPokerState.PlayerHP -= 1;
            m_CurrentPokerState.PlayerCash = 15;
            m_CurrentPokerState.OpponentCash = 15;
            if(m_CurrentPokerState.PlayerHP == 0)
            {
                //load loose scene
                UnityEngine.SceneManagement.SceneManager.LoadScene(Transitions.LoseScene.name);
            }
            //initialize dialog, could be special state......................
            m_OpponentObject.DisplayDialog("RoundWin", () =>
             {
                 m_PokerPaused = false;
                 p_InitializeNewRound();
             });
        }
        else if(m_CurrentPokerState.OpponentCash < 0)
        {
            m_PokerPaused = true;
            m_CurrentPokerState.OpponentHP -= 1;
            m_CurrentPokerState.PlayerCash = 15;
            m_CurrentPokerState.OpponentCash = 15;
            m_OpponentObject.DisplayDialog(""+(3-m_CurrentPokerState.OpponentHP), () =>
            {
                if (m_CurrentPokerState.OpponentHP == 0)
                {
                    //load loose scene
                    UnityEngine.SceneManagement.SceneManager.LoadScene(Transitions.NextScene.name);
                }
                m_PokerPaused = false;
                p_InitializeNewRound(); 
            });
            FindObjectOfType<LifeContainer>().DecreaseOpponentHP();
        }


    }


    void p_InitializeNewRound()
    {
        m_CurrentPokerState.PlayerPot = 2;
        m_CurrentPokerState.OpponentPot = 2;
        m_CurrentPokerState.PlayerCash -= 2;
        m_CurrentPokerState.OpponentCash -= 2;
        m_CurrentPokerState.TurnCount = 0;


        
        m_CurrentPokerState.Call = false;
        p_UpdatePot();
        m_PokerPaused = true;
        m_CurrentPokerState.PlayerTurn = true;
        //if someone is at negative money, they loose hp
        m_BettingMenu.gameObject.SetActive(false);


        StartCoroutine(p_WaitForAnimation());
        //
    }
    void p_StartBettingSequence()
    {
        m_CurrentPokerState.Call = true;
        if(m_CurrentPokerState.PlayerTurn)
        {
            m_BettingMenu.gameObject.SetActive(true);
        }
        else
        {
            m_BettingMenu.gameObject.SetActive(false);
        }
    }

    enum PokerHand
    {
        Null,
        Pair,
        TwoPair,
        Triss,
        Straight,
        Flush,
        FullHouse,
        FourOfAKind
        
    }
    int PairCount(List<Card> Hand,int TargetCount)
    {
        int ReturnValue = 0;
        List<int> CountMap = new List<int>();
        for(int i = 0; i < 13;i++)
        {
            CountMap.Add(0);
        }
        foreach(Card CurrentCard in Hand)
        {
            CountMap[CurrentCard.Value - 1] += 1;
        }
        foreach(int Count in CountMap)
        {
            if(Count == TargetCount)
            {
                ReturnValue += 1;
            }
        }
        return(ReturnValue);
    }
    bool HasPair(List<Card> Hand)
    {
        return (PairCount(Hand, 2) == 1);
    }
    bool HasTwoPair(List<Card> Hand)
    {
        return (PairCount(Hand, 2) == 2);
    }
    bool HasTriss(List<Card> Hand)
    {
        return (PairCount(Hand,3) == 1);
    }
    bool HasFlush(List<Card> Hand)
    {
        bool ReturnValue = true;
        CardType TargetType = Hand[0].Type;
        for(int i = 1; i < 5;i++)
        {
            if(Hand[i].Type != TargetType)
            {
                ReturnValue = false;
                break;
            }
        }
        return (ReturnValue);
    }
    bool HasFourOfAKind(List<Card> Hand)
    {
        return (PairCount(Hand, 4) == 1);
    }
    bool HasStraight(List<Card> Hand)
    {
        bool ReturnValue = true;
        List<Card> HandCopy = new List<Card>(Hand);
        HandCopy.Sort((lhs, rhs) => (lhs.Value != 1 ?  lhs.Value : 14).CompareTo( rhs.Value != 1 ? rhs.Value : 14) );
        for(int i = 0; i < 4; i++)
        {
            if(HandCopy[i].Value != HandCopy[i+1].Value+1 && !(HandCopy[i].Value == 13 && HandCopy[i+1].Value == 14))
            {
                ReturnValue = false;
                break;
            }
        }
        return (ReturnValue);
    }
        
    PokerHand GetHand(List<Card> Hand)
    {
        PokerHand ReturnValue = PokerHand.Null;
        if(HasPair(Hand))
        {
            ReturnValue = PokerHand.Pair;
        }
        if(HasTwoPair(Hand))
        {
            ReturnValue = PokerHand.TwoPair;
        }
        if(HasTriss(Hand))
        {
            ReturnValue = PokerHand.Triss;
        }
        if(HasPair(Hand) && HasTriss(Hand))
        {
            ReturnValue = PokerHand.FullHouse;
        }
        if(HasFlush(Hand))
        {
            ReturnValue = PokerHand.Flush;
        }
        if(HasStraight(Hand))
        {
            ReturnValue = PokerHand.Straight;
        }
        if(HasFourOfAKind(Hand))
        {
            ReturnValue = PokerHand.FourOfAKind;
        }
        return (ReturnValue);
    }


    public bool Less(List<Card> FirstHand,List<Card> SecondHand)
    {
        int Result = 0;
        PokerHand LeftHand = GetHand(FirstHand);
        PokerHand RightHand = GetHand(SecondHand);
        Result = LeftHand.CompareTo(RightHand);
        if(Result == 0)
        {
            int MaxLeft = FirstHand.Max( (lhs) => lhs.Value != 1 ? lhs.Value : 14);
            int MaxRight = SecondHand.Max( (lhs) => lhs.Value != 1 ? lhs.Value : 14);
            return (MaxLeft < MaxRight);
        }
        return (Result == -1);
    }

    bool m_RevealSequence = false;
    IEnumerator p_RevealSequence(List<GameObject> PlayerCards,List<GameObject> OpponentCards,PokerHandler AssociatedHandler,GameObject RevealScene)
    {
        AssociatedHandler.m_RevealSequence = true;
        float TargetPosition = -100;
        float RevealDelay = 1f;
        float ElapsedTime = 0;
        while(ElapsedTime < RevealDelay)
        {
            ElapsedTime += Time.deltaTime;
            yield return null;
        }
        float RevealSpeed = 200f;
        while( Mathf.Abs(PlayerCards[0].transform.localPosition.y - TargetPosition) >= 0.1f)
        {
            for(int i = 0; i < 5;i++)
            {
                float YSpeed = Mathf.Min(Mathf.Abs(PlayerCards[0].transform.localPosition.y - TargetPosition), RevealSpeed*Time.deltaTime);
                PlayerCards[i].transform.localPosition += new Vector3(0, YSpeed);
                OpponentCards[i].transform.localPosition += new Vector3(0,-YSpeed);
            }
            yield return null;
        }
        float WaitDelay = 1f;
        ElapsedTime = 0;
        while (ElapsedTime < WaitDelay)
        {
            ElapsedTime += Time.deltaTime;
            yield return null;
        }
        //determine winner


        AssociatedHandler.m_RevealSequence = false;
        AssociatedHandler.m_SceneObject.SetActive(true);
        m_OpponentObject.gameObject.SetActive(true);
        Destroy(RevealScene);
        if (Less(m_CurrentPokerState.PlayerHand, m_CurrentPokerState.OpponentHand))
        {
            AssociatedHandler.p_PlayerLostRound();
            m_OpponentObject.OnWin();
        }
        else
        {
            m_OpponentObject.OnLose();
            AssociatedHandler.p_PlayerWonRound();
        }
    }

    void p_InitializeRevealSequence()
    {
        GameObject RevealCardSceneObject = Instantiate(RevealCardsScene);
        Canvas AssociatedCanvas = RevealCardSceneObject.GetComponentInChildren<Canvas>();
        m_SceneObject.SetActive(false);
        m_OpponentObject.gameObject.SetActive(false);
        List<GameObject> PlayerCards = new List<GameObject>();
        List<GameObject> OpponentCards = new List<GameObject>();
        for(int i = 0; i < 5;i++)
        {
            GameObject PlayerCard = Instantiate(CardPrefab);
            PlayerCard.transform.parent = AssociatedCanvas.transform;
            PlayerCard.transform.localPosition = new Vector2(-400 + i * 200, -500);
            CardScript AssociatedScript = PlayerCard.GetComponent<CardScript>();
            AssociatedScript.CardValue = m_AssociatedDeck.DrawCard();
            AssociatedScript.GetComponent<UnityEngine.UI.Image>().sprite = m_AssociatedDeck.GetSprite(m_CurrentPokerState.PlayerHand[i]);

            GameObject OpponentCard = Instantiate(CardPrefab);
            OpponentCard.transform.parent = AssociatedCanvas.transform;
            OpponentCard.transform.localPosition = new Vector2(-400 + i * 200, 500);
            AssociatedScript = OpponentCard.GetComponent<CardScript>();
            AssociatedScript.CardValue = m_AssociatedDeck.DrawCard();
            AssociatedScript.GetComponent<UnityEngine.UI.Image>().sprite = m_AssociatedDeck.GetSprite(m_CurrentPokerState.OpponentHand[i]);

            PlayerCards.Add(PlayerCard);
            OpponentCards.Add(OpponentCard);
        }
        StartCoroutine(p_RevealSequence(PlayerCards, OpponentCards, this, RevealCardSceneObject));
    }
    void p_PlayerLostRound()
    {
        m_CurrentPokerState.OpponentCash += m_CurrentPokerState.PlayerPot + m_CurrentPokerState.OpponentPot;
        m_CurrentPokerState.PlayerPot = 0;
        m_CurrentPokerState.OpponentPot = 0;
        p_InitializeNewRound();
    }
    void p_PlayerWonRound()
    {
        m_CurrentPokerState.PlayerCash += m_CurrentPokerState.PlayerPot + m_CurrentPokerState.OpponentPot;
        m_CurrentPokerState.PlayerPot = 0;
        m_CurrentPokerState.OpponentPot = 0;
        p_InitializeNewRound();
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
        //AssociatedScript.transform.SetParent(m_GlobalCanvas.transform, true);
        if(m_HandObjects[CardIndex] != null)
        {
            Destroy(m_HandObjects[CardIndex]);
        }
        m_HandObjects[CardIndex] = NewCard;
        m_CurrentPokerState.PlayerHand[CardIndex] = AssociatedScript.CardValue;
    }
    private OpponentScript m_OpponentObject;
    TimerScript m_Timer;
    Stress m_StressObject;
    void Start()
    {
        GlobalTransitionInfo.BattleScene = Transitions.BattleScene;
        m_StressObject = FindObjectOfType<Stress>();
        m_Timer = FindObjectOfType<TimerScript>();
        m_OpponentObject = FindObjectOfType<OpponentScript>();
        m_HandObjects = new List<GameObject>();
        for(int i = 0; i < 5;i++)
        {
            m_HandObjects.Add(null);
            m_CurrentPokerState.PlayerHand.Add(null);
        }
        m_SceneObject = GameObject.FindGameObjectWithTag("PokerScene");
        m_AssociatedDeck = FindObjectOfType<Deck>();
        m_CurrentPokerState.AssociatedDeck = m_AssociatedDeck;
        m_BettingMenu = GameObject.FindObjectOfType<RaiseMenu>(true);
        m_GlobalCanvas = m_SceneObject.GetComponentInChildren<Canvas>();
        //create hand
        for(int i = 0; i < 5;i++)
        {
            DrawCard(i);
            m_CurrentPokerState.OpponentHand.Add(m_AssociatedDeck.DrawCard());
        }
        p_InitializeNewRound();
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
            m_OpponentObject.HoverEnter(AssociatedCard.CardIndex);
            m_PickHandler.HoverEnter(AssociatedCard.CardIndex);
        }
    }
    public void CardHoverLeave(CardScript AssociatedCard)
    {
        if (m_PickHandler != null)
        {
            m_OpponentObject.HoverLeave();
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


    void p_DiscardCard(int CardIndex,Vector2 TargetPosition)
    {

    }

    public void CardDropped(CardScript AssociatedCard,DropType Type)
    {
        if(!m_CurrentPokerState.PlayerTurn || m_CurrentPokerState.Call || m_PokerPaused)
        {
            AssociatedCard.ResetPosition();
            Oof();
            return;
        }
        if(Type == DropType.Opponent)
        {
            //AssociatedCard.ResetPosition();
            //print("Opponent");
            AssociatedCard.ResetPosition();
            m_ReplacedCardIndex = AssociatedCard.CardIndex;
            m_PickHandler = new PickHandler();
            m_PickHandler.PlayerPicking = true;
            m_PickHandler.Opponent = TempOpponent;
            //create cards
            m_SceneObject.SetActive(false);
            Instantiate(SelectCardScene);
            p_SetZoomCamera();
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
            m_CurrentPokerState.TurnCount += 1;
            m_CurrentPokerState.PlayerTurn = false;
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
            m_CurrentPokerState.DiscardedCards += 1;
            if(m_CurrentPokerState.DiscardedCards == 2)
            {
                m_CurrentPokerState.TurnCount += 1;
                m_CurrentPokerState.PlayerTurn = false;
            }
        }
    }




    void p_UpdatePot()
    {
        FindObjectOfType<FundDisplay>().SetOpponentFunds(m_CurrentPokerState.OpponentCash);
        FindObjectOfType<FundDisplay>().SetPlayerFunds(m_CurrentPokerState.PlayerCash);
    }
    public void OnRaise()
    {
        int NewTotalPot = m_CurrentPokerState.OpponentPot + 2;
        int TotalRaise = NewTotalPot - m_CurrentPokerState.PlayerPot;
        m_CurrentPokerState.PlayerPot += TotalRaise;
        m_CurrentPokerState.PlayerCash -= TotalRaise;
        m_CurrentPokerState.PlayerTurn = false;

        m_OpponentObject.OnOpponentRaise();
        p_UpdatePot();
    }
    public void OnMatch()
    {
        int NewTotalPot = m_CurrentPokerState.OpponentPot;
        int TotalRaise = NewTotalPot - m_CurrentPokerState.PlayerPot;
        m_CurrentPokerState.PlayerPot += TotalRaise;
        m_CurrentPokerState.PlayerCash -= TotalRaise;
        m_CurrentPokerState.PlayerTurn = false;
        m_OpponentObject.OnOpponentMatch();
        p_UpdatePot();
        p_InitializeRevealSequence();
        //reveal sequence
    }
    public void OnFold()
    {
        m_CurrentPokerState.OpponentCash += m_CurrentPokerState.PlayerPot + m_CurrentPokerState.OpponentPot;
        m_CurrentPokerState.PlayerPot = 0;
        m_CurrentPokerState.OpponentPot = 0;
        m_CurrentPokerState.PlayerTurn = false;
        m_OpponentObject.OnOpponentFold();
        p_InitializeNewRound();
        p_UpdatePot();
    }

    public void OnPass()
    {
        if(!m_CurrentPokerState.PlayerTurn)
        {
            Oof();
            return;
        }
        m_CurrentPokerState.PlayerTurn = false;
        m_CurrentPokerState.TurnCount += 1;
    }
    public void OnCall()
    {
        if(m_CurrentPokerState.TurnCount < 4)
        {
            Oof();
            return;
        }
        m_CurrentPokerState.PlayerTurn = false;
        p_StartBettingSequence();
        m_OpponentObject.OnOpponentCall();
        m_CurrentPokerState.PlayerCash -= 2;
        m_CurrentPokerState.PlayerPot += 2;
    }
    float m_ZoomCamera = 2;
    float m_ZoomPosition = 3;
    

    void p_SetZoomCamera()
    {
        FindObjectOfType<Camera>().orthographicSize = 2;
        FindObjectOfType<Camera>().transform.position = new Vector3(0,3,-10);
    }
    void p_SetNormalCamera()
    {
        FindObjectOfType<Camera>().orthographicSize = 5;
        FindObjectOfType<Camera>().transform.position = new Vector3(0, 0,-10);
    }
    void p_InitializeOpponentPick()
    {
        //AssociatedCard.ResetPosition();
        //print("Opponent");
        //create cards
        m_SceneObject.SetActive(false);
        Instantiate(OpponentSelectCardScene);
        p_SetZoomCamera();
        //camera settings
        List<GameObject> CardObjects = new List<GameObject>();
        for (int i = 0; i < 5; i++)
        {
            GameObject NewCard = Instantiate(CardPrefab);
            CardScript AssociatedScript = NewCard.GetComponent<CardScript>();
            AssociatedScript.CardIndex = i;
            AssociatedScript.Hover = true;
            AssociatedScript.GetComponent<UnityEngine.UI.Image>().sprite = m_HandObjects[i].GetComponent<UnityEngine.UI.Image>().sprite;
            AssociatedScript.AssociatedHandler = this;
            CardObjects.Add(NewCard);
        }
        m_OpponentPickHandler = m_OpponentObject.GetHandAnimation(CardObjects,m_OpponentObject);
        m_OpponentObject.EnterPickCard();
    }


    void p_ResetOpponentEyes()
    {
        m_OpponentObject.SetEyeDirection(new Vector2(0, 0));
    }
    void p_SetOpponentEyesMouse()
    {
        Vector3 EyeCenter = new Vector3(Screen.width / 2, Screen.height / 2);
        m_OpponentObject.SetEyeDirection(Input.mousePosition - EyeCenter);
    }
    bool m_Initialised = false;
    // Update is called once per frame
    
    
    IEnumerator DelayEnumerator(System.Action Callable, float Duration)
    {
        float ElapsedTime = 0;
        while (ElapsedTime < Duration)
        {
            ElapsedTime += Time.deltaTime;
            yield return null;
        }
        Callable();
    }
    void Delay(System.Action Callable,float Duration)
    {
        StartCoroutine(DelayEnumerator(Callable, Duration));
    }
    void Update()
    {
        //DEBUG AFFFF
        if(Input.GetKeyDown(KeyCode.J))
        {
            FindObjectOfType<LifeContainer>().DecreasePlayerHP();
        }
        if(Input.GetKeyDown(KeyCode.Space))
        {
            m_CurrentPokerState.OpponentCash = -1;
            p_InitializeNewRound();
        }
        if(Input.GetKeyDown(KeyCode.K))
        {
            FindObjectOfType<LifeContainer>().DecreaseOpponentHP();
        }
        //


        if(m_RevealSequence || m_PokerPaused)
        {
            m_Timer.gameObject.SetActive(false);
            return;
        }
        if(m_CurrentPokerState.PlayerTurn)
        {
            if(!m_Timer.isActiveAndEnabled)
            {
                m_Timer.ResetTimer();
            }
            m_Timer.gameObject.SetActive(true);
            if(m_Timer.TimerUp())
            {
                if(m_CurrentPokerState.Call)
                {
                    OnFold();
                }
                else
                {
                    //discard random card
                    int CardToDiscard = Random.Range(0, m_HandObjects.Count);
                    //random target location
                    Vector2 Origin = new Vector2(0, -5);
                    Vector2 Destination = new Vector2(Random.Range(-6.5f, 6.5f), Random.Range(-4, -2));
                    Sprite CardSprite = m_HandObjects[CardToDiscard].GetComponent<UnityEngine.UI.Image>().sprite;
                    StartCoroutine(p_ThrowCard(Origin, Destination, CardSprite));
                    DrawCard(CardToDiscard);
                    m_CurrentPokerState.PlayerTurn = false;
                }
            }
        }
        else
        {
            m_Timer.gameObject.SetActive(false);
        }
        if(m_PickHandler != null)
        {
            p_SetOpponentEyesMouse();
            if (!m_Initialised)
            {
                m_PickHandler.Initialize();
                m_Initialised = true;
            }
            int CardIndex = m_PickHandler.Update();
            if(CardIndex != -1)
            {
                Card PickedCard = m_CurrentPokerState.OpponentHand[CardIndex];
                m_CurrentPokerState.OpponentHand[CardIndex] = m_AssociatedDeck.DrawCard();
                CardScript PlayerCard = m_HandObjects[CardIndex].GetComponent<CardScript>();
                PlayerCard.CardValue = PickedCard;
                m_CurrentPokerState.PlayerHand[m_ReplacedCardIndex] = PickedCard;
                m_HandObjects[m_ReplacedCardIndex].GetComponent<UnityEngine.UI.Image>().sprite = m_AssociatedDeck.GetSprite(PlayerCard.CardValue);

                m_SceneObject.SetActive(true);
                p_SetNormalCamera();
                Destroy(GameObject.FindGameObjectWithTag("PickScene"));
                p_ResetOpponentEyes();
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
                m_SceneObject.SetActive(true);
                p_SetNormalCamera();
                Destroy(GameObject.FindGameObjectWithTag("OpponentPickScene"));
                m_OpponentPickHandler = null;
                p_ResetOpponentEyes();
                DrawCard(GrabbedCardIndex);
                m_OpponentObject.LeavePickCard();
            }
        }
        else 
        {
            if(!m_CurrentPokerState.PlayerTurn)
            {
                if(m_CurrentOpponentState.ElapsedThink == 0)
                {
                    //random how long think
                    if(Random.Range(0f,1f) < 0.4f)
                    {
                        //m_OpponentObject.OnThink();
                        Delay(() => m_OpponentObject.OnThink(), 1);
                        m_CurrentOpponentState.ThinkTime = 4f;
                    }
                    else
                    {
                        m_CurrentOpponentState.ThinkTime = 2;
                    }
                }
                m_CurrentOpponentState.ElapsedThink += Time.deltaTime;
                if(m_CurrentOpponentState.ElapsedThink >= m_CurrentOpponentState.ThinkTime)
                {
                    //make move
                    Move NewMove = m_OpponentObject.MakeMove(m_CurrentPokerState);
                    m_CurrentPokerState.PlayerTurn = true;
                    m_CurrentPokerState.TurnCount += 1;
                    m_CurrentPokerState.DiscardedCards = 0;
                    if (m_CurrentPokerState.Call)
                    {
                        if (NewMove is Move_Raise)
                        {
                            m_OpponentObject.OnRaise();
                            int NewTotalPot = m_CurrentPokerState.PlayerPot + 2;
                            int TotalRaise = NewTotalPot - m_CurrentPokerState.OpponentPot;
                            m_CurrentPokerState.OpponentPot += TotalRaise;
                            m_CurrentPokerState.OpponentCash -= TotalRaise;
                            m_BettingMenu.gameObject.SetActive(true);
                            m_BettingMenu.SetPot(m_CurrentPokerState.OpponentPot, m_CurrentPokerState.PlayerPot);
                            p_UpdatePot();
                        }
                        else if (NewMove is Move_Match)
                        {
                            m_OpponentObject.OnMatch();
                            int NewTotalPot = m_CurrentPokerState.PlayerPot;
                            int TotalRaise = NewTotalPot - m_CurrentPokerState.OpponentPot;
                            m_CurrentPokerState.OpponentPot += TotalRaise;
                            m_CurrentPokerState.OpponentCash -= TotalRaise;
                            m_BettingMenu.gameObject.SetActive(true);
                            m_BettingMenu.SetPot(m_CurrentPokerState.OpponentPot, m_CurrentPokerState.PlayerPot);
                            p_UpdatePot();
                        }
                        else if(NewMove is Move_Fold)
                        {
                            m_OpponentObject.OnFold();
                            m_CurrentPokerState.PlayerCash += m_CurrentPokerState.PlayerPot + m_CurrentPokerState.OpponentPot;
                            m_CurrentPokerState.PlayerPot = 0;
                            m_CurrentPokerState.OpponentPot = 0;
                            m_BettingMenu.gameObject.SetActive(true);
                            m_BettingMenu.SetPot(m_CurrentPokerState.OpponentPot, m_CurrentPokerState.PlayerPot);
                            p_InitializeNewRound();
                            p_UpdatePot();
                        }
                    }
                    else
                    {
                        if(NewMove is Move_Call)
                        {
                            m_OpponentObject.OnCall();
                            m_BettingMenu.SetPot(m_CurrentPokerState.OpponentPot, m_CurrentPokerState.PlayerPot);
                            p_StartBettingSequence();
                        }
                        else if(NewMove is Move_DiscardCards)
                        {
                            foreach(Card DiscardedCard in ((Move_DiscardCards)NewMove).DiscardedCards)
                            {
                                Vector2 Origin = new Vector2(0, 0);
                                //random destination
                                Vector2 Destination = new Vector2(Random.Range(-6.5f, 6.5f), Random.Range(-2, 0));
                                Sprite AssociatedSprite = m_CurrentPokerState.AssociatedDeck.GetSprite(DiscardedCard);
                                StartCoroutine(p_ThrowCard(Origin, Destination, AssociatedSprite));
                            }
                        }
                        else if(NewMove is Move_StealCard)
                        {
                            p_InitializeOpponentPick();
                        }
                    }
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
