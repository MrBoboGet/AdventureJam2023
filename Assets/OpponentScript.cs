using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

[System.Serializable]
public class CharacterAnimation
{
    public Sprite SpriteAnimation = null;
    public string Clip = null; 
}

public class OpponentScript : MonoBehaviour
{
    public TransitionInfo Transitions;
    public float EyeRadius = 0.1f;
    public Sprite NeutralSprite;
    public Sprite WhiteEye;
    public Sprite EyeSprite;


    public Sprite TellSprite;
    public CharacterAnimation WinSprite;
    public CharacterAnimation LoseSprite;

    public TextAsset DialogText;
    public GameObject DialogObject;
    public GameObject LoreDialog;


    GameObject m_EyeObject;
    Dictionary<string, List<string>> m_Dialog = new Dictionary<string, List<string>>();

    public PokerState AssociatedPokerState;


    float ChangeDuration = 4f;

    protected bool m_InAnimation = false;

    Canvas m_AssociatedCanvas;


    GameObject m_DialogObject;

    virtual public bool InAnimation()
    {
        return (m_InAnimation);
    }
    // Start is called before the first frame update
    public void Start()
    {
        m_AssociatedCanvas = FindObjectOfType<Canvas>();

        m_DialogObject = Instantiate(DialogObject);
        m_DialogObject.SetActive(false);
        Vector3 OriginalPosition = m_DialogObject.transform.position;
        m_DialogObject.transform.parent = m_AssociatedCanvas.gameObject.transform;
        m_DialogObject.transform.position = new Vector3();
        m_DialogObject.transform.localPosition = OriginalPosition;




        GameObject EyeObject = new GameObject("asdasd");
        EyeObject.AddComponent<SpriteRenderer>();
        EyeObject.GetComponent<SpriteRenderer>().sprite = EyeSprite;
        m_EyeObject = EyeObject;
        m_EyeObject.transform.parent = gameObject.transform;
        m_EyeObject.transform.position = new Vector3();
        m_EyeObject.transform.localScale = new Vector3(1, 1, 1);
        m_EyeObject.transform.localPosition = new Vector3();
        m_EyeObject.GetComponent<SpriteRenderer>().sortingOrder = -15;

        GameObject EyeWhite = new GameObject("asdasd");
        EyeWhite.AddComponent<SpriteRenderer>();
        EyeWhite.GetComponent<SpriteRenderer>().sprite = WhiteEye;
        EyeWhite.transform.parent = gameObject.transform;
        EyeWhite.transform.localScale = new Vector3(1, 1, 1);
        EyeWhite.transform.position = new Vector3(0, 0, 0);
        EyeWhite.transform.localPosition = new Vector3(0, 0, 0);
        EyeWhite.GetComponent<SpriteRenderer>().sortingOrder = -20;


        //initialize text

        if(DialogText == null)
        {
            return;
        }
        string CurrentTag = "";
        System.IO.StringReader Reader = new System.IO.StringReader(DialogText.text);
        while(true)
        {
            string Line = Reader.ReadLine();
            if(Line == null)
            {
                break;
            }
            if(Line == "")
            {
                continue;
            }
            if(Line.Contains("#"))
            {
                int HashTagPosition = Line.IndexOf('#');
                CurrentTag = Line.Substring(HashTagPosition + 1);
                if(!m_Dialog.ContainsKey(CurrentTag))
                {
                    m_Dialog.Add(CurrentTag, new List<string>());
                }
            }
            else
            {
                m_Dialog[CurrentTag].Add(Line);
            }
        }
    }

    public virtual void SetEyeDirection(Vector2 EyeDirection)
    {
        if(EyeDirection.x == 0 && EyeDirection.y == 0)
        {
            m_EyeObject.transform.localPosition = new Vector2();
        }
        else
        {
            m_EyeObject.transform.localPosition = EyeDirection.normalized * EyeRadius;
        }
    }
    public void HoverEnter(int CardIndex)
    {
        if(CardIndex == 2)
        {
            GetComponent<SpriteRenderer>().sprite = TellSprite;
        }
    }

    virtual protected void p_DisplayDialogCategory(string CategoryName)
    {
        if (m_Dialog.ContainsKey(CategoryName))
        {
            p_DisplayDialog(m_Dialog[CategoryName], 2);
        }
    }

    public void OnThink()
    {
        p_DisplayDialogCategory("Whatever");
    }

    public void HoverLeave()
    {
        GetComponent<SpriteRenderer>().sprite = NeutralSprite;
    }

    IEnumerator p_ChangeSprite(string NewSprite, float Duration)
    {
        m_InAnimation = true;
        Sprite PreviousSprite = GetComponent<SpriteRenderer>().sprite;
        Animator AssociatedAnimator = GetComponent<Animator>();
        AssociatedAnimator.enabled = true;
        AssociatedAnimator.Play("Base Layer."+NewSprite);
        float ElapsedDuration = 0;
        while (ElapsedDuration < Duration)
        {
            ElapsedDuration += Time.deltaTime;
            yield return null;
        }
        AssociatedAnimator.enabled = false;
        GetComponent<SpriteRenderer>().sprite = PreviousSprite;
        m_InAnimation = false;
    }
    IEnumerator p_ChangeSprite(CharacterAnimation Animation,float Duration)
    {
        if(Animation.Clip != null && Animation.Clip != "")
        {
            return (p_ChangeSprite(Animation.Clip, Duration));
        }
        else if(Animation.SpriteAnimation != null)
        {
            return (p_ChangeSprite(Animation.SpriteAnimation, Duration));
        }
        return (null);
    }

    IEnumerator p_ChangeSprite(Sprite NewSprite,float Duration)
    {
        m_InAnimation = true;
        Sprite OldSprite = GetComponent<SpriteRenderer>().sprite;
        GetComponent<SpriteRenderer>().sprite = NewSprite;
        float ElapsedDuration = 0;
        while(ElapsedDuration < Duration)
        {
            ElapsedDuration += Time.deltaTime;
            yield return null;
        }
        GetComponent<SpriteRenderer>().sprite = OldSprite;
        m_InAnimation = false;
    }
    int m_DialogCount = 0;


    IEnumerator p_DisplayDialog(string DialogString,float Duration)
    {
        m_DialogCount += 1;
        m_DialogObject.SetActive(true);
        m_DialogObject.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = DialogString;

        float DialogDuration = Duration;
        float ElapsedTime = 0;
        while (ElapsedTime < DialogDuration)
        {
            ElapsedTime += Time.deltaTime;
            yield return null;
        }
        m_DialogCount -= 1;
        if (m_DialogCount == 0)
        {
            m_DialogObject.SetActive(false);
        }
    }
    IEnumerator p_DisplayDialog(string DialogString)
    {
        return (p_DisplayDialog(DialogString, ChangeDuration));
        //Destroy(IngameDialogObject);
    }
    void p_DisplayDialog(List<string> PossibleDialogs,float DialougeDuration)
    {
        if(PossibleDialogs == null)
        {
            return;
        }
        string DialogToDisplay = PossibleDialogs[Random.Range(0, PossibleDialogs.Count)];
        StartCoroutine(p_DisplayDialog(DialogToDisplay,DialougeDuration));
    }
    void p_DisplayDialog(List<string> PossibleDialogs)
    {
        if (PossibleDialogs == null)
        {
            return;
        }
        string DialogToDisplay = PossibleDialogs[Random.Range(0, PossibleDialogs.Count)];
        StartCoroutine(p_DisplayDialog(DialogToDisplay,ChangeDuration));
    }
    List<IEnumerator> m_CoRoutines = new List<IEnumerator>();
    void StartCoroutine(IEnumerator NewRoutine)
    {
        m_CoRoutines.Add(NewRoutine);
    }
    public void OnWin()
    {
        StartCoroutine(p_ChangeSprite(WinSprite, ChangeDuration));
        p_DisplayDialogCategory("Win");
    }
    public void OnLose()
    {
        StartCoroutine(p_ChangeSprite(LoseSprite, ChangeDuration));
        p_DisplayDialogCategory("Lose");
    }

    public void OnCall()
    {
        p_DisplayDialogCategory("Call");
    }
    public void OnFold()
    {
        p_DisplayDialogCategory("Fold");
    }
    public void OnOpponentCall()
    {
        p_DisplayDialogCategory("OpponentCall");
    }
    public void OnOpponentFold()
    {
        p_DisplayDialogCategory("OpponentFold");
    }
    public void OnRaise()
    {
        p_DisplayDialogCategory("Raise");
    }
    public void OnOpponentRaise()
    {
        p_DisplayDialogCategory("OpponentRaise");
    }
    public void OnMatch()
    {
        p_DisplayDialogCategory("Match");
    }
    public void OnOpponentMatch()
    {
        p_DisplayDialogCategory("OpponentMatch");
    }

    virtual public Move MakeMove(PokerState CurrentState)
    {
        Move ReturnValue = null;
        if (CurrentState.Call)
        {
            float RandomXD = Random.Range(0, 1f);
            if (RandomXD < 0.33f)
            {
                ReturnValue = new Move_Match();
            }
            else if (RandomXD < 0.66f)
            {
                ReturnValue = new Move_Raise();
            }
            else if (RandomXD < 1f)
            {
                ReturnValue = new Move_Fold();
            }
        }
        else
        {
            float RandomXD = Random.Range(0, 1f);
            if (RandomXD < 0.33f)
            {
                ReturnValue = new Move_StealCard();
            }
            else if (RandomXD < 0.66f)
            {
                ReturnValue = new Move_Call();
            }
            else if (RandomXD < 1)
            {
                ReturnValue = new Move_DiscardCards();
                Move_DiscardCards CardsToDiscard = (Move_DiscardCards)ReturnValue;
                Card CardToDiscard = CurrentState.OpponentHand[0];
                CurrentState.OpponentHand[0] = CurrentState.AssociatedDeck.DrawCard();
                CardsToDiscard.DiscardedCards.Add(CardToDiscard);
            }
        }
        return (ReturnValue);
    }

    // Update is called once per frame
    public void Update()
    {
        List<IEnumerator> NewRoutines = new List<IEnumerator>();
        foreach(IEnumerator Routine in m_CoRoutines)
        {
            if (Routine.MoveNext())
            {
                //remove 
                NewRoutines.Add(Routine);
            }
        }
        m_CoRoutines = NewRoutines;
    }

    public virtual HandAnimation GetHandAnimation(List<GameObject> Cards, OpponentScript Opponent)
    {
        return (new HandAnimation(Cards, Opponent));
    }
    public virtual void EnterPickCard()
    {

    }

    virtual public void DisplayDialog(string DialogID,System.Action ResultingAction)
    {
        //create dialog object with appropriate stufferino
        GameObject DialogObject = Instantiate(LoreDialog);
        Vector3 Position = DialogObject.transform.position;
        DialogObject.transform.parent = m_AssociatedCanvas.transform;
        DialogObject.transform.localPosition = Position;
        Dialog AssociatedDialog = DialogObject.GetComponentInChildren<Dialog>();
        AssociatedDialog.SetDoneAction(ResultingAction);
        AssociatedDialog.TextBoxes = m_Dialog[DialogID];
    }

    public virtual void LeavePickCard()
    {

    }
    public class HandAnimation
    {
        Camera m_Camera;
        public virtual void Initialize()
        {
            m_EventSystem = FindObjectOfType<EventSystem>();
            m_Camera = FindObjectOfType<Camera>();
            //get scene object
            GameObject CardScene = GameObject.FindGameObjectWithTag("OpponentPickScene");
            AssociatedObject = GameObject.FindGameObjectWithTag("Hand");
            //set opponent sprite
            Canvas AssociatedCanvas = CardScene.GetComponentInChildren<Canvas>();
            foreach (GameObject Object in m_Cards)
            {
                Object.transform.parent = AssociatedCanvas.gameObject.transform;
            }
            for (int i = 0; i < m_Cards.Count; i++)
            {
                m_Cards[i].transform.localPosition = new Vector2(-700 + i * 350, -450);
                m_Cards[i].GetComponent<RectTransform>().sizeDelta = new Vector2(175, m_Cards[i].GetComponent<RectTransform>().sizeDelta.y);
                m_CardPositions.Add(m_Cards[i].transform.localPosition);
            }
            m_ElapsedMoveTime = m_MoveAwayDuration + m_MoveBackDuration + m_MoveAgainDelay;
        }
        public HandAnimation(List<GameObject> Cards, OpponentScript Opponent)
        {
            m_Cards = Cards;
            m_Opponent = Opponent;
        }
        public class GrabData
        {
            public float TotalHoverLength = 3;
            public float HoverSpeed = 2000f;
            public int MaxSwitchCount = -1;
            public float GrabDelay = 0.5f;
            public float GrabSpeed = 3000f;
        }
        public HandAnimation(List<GameObject> Cards, OpponentScript Opponent,GrabData GrabInfo)
        {
            m_Cards = Cards;
            m_Opponent = Opponent;
            m_HoverLength = GrabInfo.TotalHoverLength;
            m_HoverSpeed = GrabInfo.HoverSpeed;
            m_GrabDelay = GrabInfo.GrabDelay;
            m_GrabSpeed = GrabInfo.GrabSpeed;
            m_MaxSwitchCount = GrabInfo.MaxSwitchCount;
        }

        protected GameObject AssociatedObject;
        protected List<GameObject> m_Cards;
        protected List<Vector2> m_CardPositions = new List<Vector2>();
        protected OpponentScript m_Opponent;

        //event system
        EventSystem m_EventSystem;

        float m_HoverLength = 3;
        float m_HoverSpeed = 2000f;
        float m_GrabDelay = 0.5f;
        float m_GrabSpeed = 3000f;
        float m_GrabYLocation = 3f;
        float m_ElapsedAnimation = 0;

        int m_MaxSwitchCount = -1;
        int m_CurrentCardTarget = 0;

        float m_ElapsedGrabTime = 0;

        int m_GrabbedCardIndex = -1;

        float m_GrabbTransitionDelay = 1f;
        float m_ElapsedGrabTransition = 0;



        Vector2 p_CanvasToScreenSpace(Vector2 VectorToConvert)
        {
            VectorToConvert.x += 2560 / 2;//hardcoded af
            VectorToConvert.y += 1440 / 2;
            VectorToConvert.x = (VectorToConvert.x / 2560) * Screen.width;
            VectorToConvert.y = (VectorToConvert.y / 1440) * Screen.height;
            return (VectorToConvert);
        }

        protected int p_GetGrabbedCardIndex()
        {
            int ReturnValue = -1;
            Vector3 FingerTipPosition = AssociatedObject.transform.localPosition -
                new Vector3(0, AssociatedObject.GetComponent<RectTransform>().sizeDelta.y / 2) * AssociatedObject.transform.localScale.y;
            FingerTipPosition = p_CanvasToScreenSpace(FingerTipPosition);

            PointerEventData EventData = new PointerEventData(m_EventSystem);
            EventData.position = FingerTipPosition;

            List<RaycastResult> Results = new List<RaycastResult>();
            m_EventSystem.RaycastAll(EventData, Results);
            if (Results.Count > 0)
            {
                foreach (RaycastResult Result in Results)
                {
                    CardScript ScriptComponent = Result.gameObject.GetComponent<CardScript>();
                    if (ScriptComponent != null)
                    {
                        ReturnValue = ScriptComponent.CardIndex;
                        break;
                    }
                }
            }

            return (ReturnValue);
        }

        float m_MoveDistance = 400f;
        float m_MoveAwayDuration = 0.15f;
        float m_MoveBackDuration = 0.3f;
        float m_MoveAgainDelay = 0.2f;

        float m_ElapsedMoveTime = 0;
        float m_MoveDirection = 0;

        int m_SwitchCount = 0;



        protected void p_UpdateCardPositions()
        {
            if (m_ElapsedMoveTime > m_MoveAwayDuration + m_MoveBackDuration + m_MoveAgainDelay)
            {
                if (Input.GetKeyDown(KeyCode.D))
                {
                    m_ElapsedMoveTime = 0;
                    m_MoveDirection = 1;
                }
                else if (Input.GetKeyDown(KeyCode.A))
                {
                    m_ElapsedMoveTime = 0;
                    m_MoveDirection = -1;
                }
            }
            else if (m_ElapsedMoveTime < m_MoveAwayDuration)
            {
                //calculate speed
                float Speed = m_MoveDistance / m_MoveAwayDuration;
                foreach (GameObject Object in m_Cards)
                {
                    Object.transform.localPosition += new Vector3(Speed * m_MoveDirection, 0) * Time.deltaTime;
                }
            }
            else if (m_ElapsedMoveTime < m_MoveAwayDuration + m_MoveBackDuration)
            {
                float Speed = m_MoveDistance / m_MoveBackDuration;
                foreach (GameObject Object in m_Cards)
                {
                    Object.transform.localPosition -= new Vector3(Speed * m_MoveDirection, 0) * Time.deltaTime;
                }
            }
            m_ElapsedMoveTime += Time.deltaTime;
        }
        virtual public int Update()
        {
            if (AssociatedObject == null)
            {
                return (-1);
            }
            Vector2 TargetDestination = m_CardPositions[m_CurrentCardTarget];

            Vector3 FingerTipPosition = AssociatedObject.transform.localPosition -
                new Vector3(0, AssociatedObject.GetComponent<RectTransform>().sizeDelta.y / 2) * AssociatedObject.transform.localScale.y;
            FingerTipPosition = p_CanvasToScreenSpace(FingerTipPosition);
            m_Opponent.SetEyeDirection(FingerTipPosition - new Vector3(Screen.width / 2, Screen.height / 2));
            if(m_MaxSwitchCount != -1 && m_SwitchCount >=  m_MaxSwitchCount)
            {
                m_ElapsedAnimation = m_HoverLength;
            }
            if (m_ElapsedAnimation < m_HoverLength || Mathf.Abs(TargetDestination.x - AssociatedObject.transform.localPosition.x) > 1f)
            {
                //only use X
                float XDiff = TargetDestination.x - AssociatedObject.transform.localPosition.x;
                m_ElapsedAnimation += Time.deltaTime;
                float XToAdd = Mathf.Min(Mathf.Abs(XDiff), m_HoverSpeed * Time.deltaTime) * Mathf.Sign(XDiff);
                AssociatedObject.transform.localPosition += new Vector3(XToAdd, 0);
                if (Mathf.Abs(XToAdd) < m_HoverSpeed * Time.deltaTime - 1)
                {
                    m_SwitchCount += 1;
                    if(m_ElapsedAnimation < m_HoverLength)
                    {
                        int CurrentTarget = m_CurrentCardTarget;
                        while (CurrentTarget == m_CurrentCardTarget)
                        {
                            m_CurrentCardTarget = (int)Random.Range(0.0f, 4.999f);
                        }
                    }
                }
            }
            else
            {
                m_ElapsedGrabTime += Time.deltaTime;
                if (m_ElapsedGrabTime > m_GrabDelay)
                {
                    if (Mathf.Abs(AssociatedObject.transform.localPosition.y - m_GrabYLocation) < 0.1f)
                    {
                        int CardIndex = p_GetGrabbedCardIndex();
                        if (CardIndex != -1)
                        {
                            m_ElapsedGrabTransition += Time.deltaTime;
                            if (m_ElapsedGrabTransition > m_GrabbTransitionDelay)
                            {
                                //determine grabbed card
                                //m_GrabbedCardIndex = 1;
                                m_GrabbedCardIndex = p_GetGrabbedCardIndex();
                            }
                            return (m_GrabbedCardIndex);
                        }
                        //return (m_GrabbedCardIndex);
                    }
                    else
                    {
                        float YDiff = m_GrabYLocation - AssociatedObject.transform.localPosition.y;
                        float YDiffToAdd = Mathf.Min(m_GrabSpeed * Time.deltaTime, Mathf.Abs(YDiff)) * Mathf.Sign(YDiff);
                        AssociatedObject.transform.localPosition += new Vector3(0, YDiffToAdd);
                    }
                }
            }
            p_UpdateCardPositions();
            return (m_GrabbedCardIndex);
        }
    }
}