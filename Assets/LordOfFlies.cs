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

    void SpawnFly()
    {
        GameObject NewFly = Instantiate(FlyObject);
        NewFly.transform.parent = m_FlyCanvas.gameObject.transform;
        m_FlyObjects.Add(m_CurrentFlyID,NewFly);
        m_CurrentFlyID += 1;

        float Margin = 200;
        float XRange = (2560 - Margin) / 2;
        float YRange = (1440 - Margin) / 2;
        NewFly.transform.localPosition = new Vector3(Random.Range(-XRange, XRange), Random.Range(-YRange, YRange));
        NewFly.GetComponent<FlyScript>().Lord = this;
        NewFly.GetComponent<FlyScript>().ID = m_CurrentFlyID;
        if(Random.Range(0,1f) < 0.5f)
        {
            NewFly.transform.localScale = new Vector3();
        }
        m_ElapsedSpawnTime = 0;
    }

    class FlyHandAnimation : HandAnimation
    {
        public FlyHandAnimation(List<GameObject> Cards, OpponentScript Opponent) : base(Cards,Opponent)
        {

        }


        float m_ToPositionTime = 2f;
        float m_WaitDelay = 0.5f;
        public override int Update()
        {
        
        }
    }

    public virtual HandAnimation GetHandAnimation(List<GameObject> Cards, OpponentScript Opponent)
    {
        return (new HandAnimation(Cards, Opponent));
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
