using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LordOfFlies : OpponentScript
{
    // Start is called before the first frame update
    public GameObject CanvasObject;
    public GameObject FlyObject;



    Canvas m_FlyCanvas;
    void Start()
    {
        GameObject Canvas = Instantiate(CanvasObject);
        m_FlyCanvas = Canvas.GetComponent<Canvas>();
    }
    Dictionary<int, GameObject> m_FlyObjects;
    int m_CurrentFlyID = 0;

    public void OnFlySway(int FlyID)
    {
        m_FlyObjects.Remove(FlyID);
    }

    void SpawnFly()
    {
        GameObject NewFly = Instantiate(FlyObject);
        m_FlyObjects[m_CurrentFlyID] = Instantiate(NewFly);
        m_CurrentFlyID += 1;

        float Margin = 200;
        float XRange = (2560 - Margin) / 2;
        float YRange = (1440 - Margin) / 2;
        NewFly.transform.localPosition = new Vector3(Random.Range(-XRange, XRange), Random.Range(-YRange, YRange));
        NewFly.GetComponent<FlyScript>().Lord = this;
        NewFly.GetComponent<FlyScript>().ID = m_CurrentFlyID;
    }

    float m_FlySpawnDelay = 1f;

    float m_ElapsedSpawnTime = 0f;
    // Update is called once per frame
    void Update()
    {
        m_ElapsedSpawnTime += Time.deltaTime;
        if(m_ElapsedSpawnTime >= m_FlySpawnDelay)
        {
            SpawnFly();
        }
    }
}
