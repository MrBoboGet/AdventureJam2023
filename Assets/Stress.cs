using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stress : MonoBehaviour
{
    // Start is called before the first frame update

    public List<Sprite> StressSprites = null;
    void Start()
    {
        m_SpriteRenderer = GetComponent<SpriteRenderer>();
        m_AssociatedTimer = FindObjectOfType<TimerScript>();
    }
    SpriteRenderer m_SpriteRenderer;
    int m_TotalLevels = 3;
    float m_BaseTime = 5f;
    float m_MaxTime = 2f;

    int m_StressLevel = 0;

    TimerScript m_AssociatedTimer;
    public void IncreaseStress()
    {
        m_StressLevel += 1;
        if(m_StressLevel > m_TotalLevels)
        {
            m_StressLevel = m_TotalLevels;
        }
        m_AssociatedTimer.MaxTime = m_BaseTime - (m_BaseTime - m_MaxTime) * ((float)m_StressLevel / m_TotalLevels);
        if(StressSprites != null)
        {
            m_SpriteRenderer.sprite = StressSprites[m_StressLevel];
        }
    }
    public void DecreaseStress()
    {
        m_StressLevel -= 1;
        if (m_StressLevel > m_TotalLevels)
        {
            m_StressLevel = m_TotalLevels;
        }
        m_AssociatedTimer.MaxTime = m_BaseTime - (m_BaseTime - m_MaxTime) * ((float)m_StressLevel / m_TotalLevels);
        if (StressSprites != null)
        {
            m_SpriteRenderer.sprite = StressSprites[m_StressLevel];
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
