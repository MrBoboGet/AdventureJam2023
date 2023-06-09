using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LordOfFlies : OpponentScript
{
    // Start is called before the first frame update
    public GameObject CanvasObject;
    public GameObject FlyObject;



    Canvas m_FlyCanvas;
    int MaxFlyCount = 10;
    void Start()
    {
        base.Start();

        GameObject Canvas = Instantiate(CanvasObject);
        m_FlyCanvas = Canvas.GetComponent<Canvas>();
    }
    Dictionary<int, GameObject> m_FlyObjects = new Dictionary<int, GameObject>();
    int m_CurrentFlyID = 0;

    public void OnFlySwat(int FlyID)
    {
        m_FlyObjects.Remove(FlyID);
    }
    public bool FlyActive()
    {
        return (m_FlyObjects.Count >= 3 && m_PickCard);
    }

    void SpawnFly()
    {
        GameObject NewFly = Instantiate(FlyObject);
        NewFly.transform.parent = m_FlyCanvas.gameObject.transform;
        m_FlyObjects.Add(m_CurrentFlyID,NewFly);

        float Margin = 200;
        float XRange = (2560 - Margin) / 2;
        float YRange = (1440 - Margin) / 2;
        NewFly.transform.localPosition = new Vector3(Random.Range(-XRange, XRange), Random.Range(-YRange, YRange));
        NewFly.GetComponent<FlyScript>().Lord = this;
        NewFly.GetComponent<FlyScript>().ID = m_CurrentFlyID;
        if(Random.Range(0,1f) < 0.5f)
        {

            //NewFly.GetComponent<UnityEngine.UI.Image>().fl
            NewFly.transform.eulerAngles = new Vector3(0,180,0);
        }
        m_ElapsedSpawnTime = 0;
        m_CurrentFlyID += 1;
    }
    public override void SetEyeDirection(Vector2 EyeDirection)
    {

    }
    Sprite m_PreviousSprite = null;
    bool m_PickCard = false;
    public override void EnterPickCard()
    {
        m_PickCard = true;
        m_PreviousSprite = GetComponent<SpriteRenderer>().sprite;
        Animator AssociatedAnimator = GetComponent<Animator>();
        AssociatedAnimator.enabled = true;
        AssociatedAnimator.Play("Base Layer.Lord_Listen");
    }
    public override void LeavePickCard()
    {
        m_PickCard = false;
        GetComponent<SpriteRenderer>().sprite = m_PreviousSprite;
        Animator AssociatedAnimator = GetComponent<Animator>();
        AssociatedAnimator.enabled = false;
    }
    class FlyHandAnimation : HandAnimation
    {

        LordOfFlies m_Lord;

        public override void Initialize()
        {
            base.Initialize();
            AssociatedObject.transform.localPosition = m_HandStartPosition;
        }
        public FlyHandAnimation(List<GameObject> Cards, OpponentScript Opponent) : base(Cards,Opponent)
        {
            m_Lord = (LordOfFlies)Opponent;
            m_HandStartPosition = new Vector3(0, 800);
        }


        Vector3 m_HandStartPosition = new Vector3();
        float m_HandGrabY = 400;
        float m_ToPositionTime = 2f;
        float m_WaitDelay = 0.5f;
        float m_GrabSpeed = 1000f;

        float m_HandLastY = 100;


        int m_CardToGrabIndex = 2;
        float m_ElapsedGrabTime = 0;

        bool m_InPosition = false;

        float m_ElapsedLastWaitTime = 0;
        public override int Update()
        {
            if(m_ElapsedGrabTime < m_ToPositionTime)
            {
                float Speed = (m_HandGrabY - m_HandStartPosition.y) / m_ToPositionTime;
                AssociatedObject.transform.localPosition += new Vector3(0,Speed * Time.deltaTime);
                p_UpdateCardPositions();
            }
            else if(m_ElapsedGrabTime < m_ToPositionTime+m_WaitDelay)
            {
                if(m_InPosition)
                {
                    if (m_Lord.m_FlyObjects.Count >= 3)
                    {
                        //take best card
                        m_CardToGrabIndex = 1;//debug
                    }
                    else
                    {
                        m_GrabSpeed = 150f;
                    }
                }
                m_InPosition = true;
                //do nothing
                p_UpdateCardPositions();
            }
            else
            {
                //move towards card, irregardless of player shenanigans
                if(Mathf.Abs(AssociatedObject.transform.localPosition.y-m_HandLastY) < 1f)
                {
                    //wait
                    m_ElapsedLastWaitTime += Time.deltaTime;
                    if(m_ElapsedLastWaitTime >= 1)
                    {
                        return (m_CardToGrabIndex);
                    }
                }
                else
                {
                    Vector3 TargetPosition = new Vector3(m_Cards[m_CardToGrabIndex].transform.localPosition.x, m_HandLastY);
                    Vector3 Diff = TargetPosition - AssociatedObject.transform.localPosition;
                    float Distance = Diff.magnitude;
                    AssociatedObject.transform.position += Diff.normalized* Mathf.Min(m_GrabSpeed*Time.deltaTime,Distance);
                }
            }
            m_ElapsedGrabTime += Time.deltaTime;
            return (-1);
        }
    }

    public override HandAnimation GetHandAnimation(List<GameObject> Cards, OpponentScript Opponent)
    {
        return (new FlyHandAnimation(Cards, Opponent));
    }
    float m_FlySpawnDelay = 2f;

    float m_ElapsedSpawnTime = 0f;
    // Update is called once per frame
    void Update()
    {
        base.Update();
        if(m_FlyObjects.Count >= MaxFlyCount)
        {
            m_ElapsedSpawnTime = 0;
            return;
        }
        m_ElapsedSpawnTime += Time.deltaTime;
        if(m_ElapsedSpawnTime >= m_FlySpawnDelay)
        {
            SpawnFly();
        }
    }
}
