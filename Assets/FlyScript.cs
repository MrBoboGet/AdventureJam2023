using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
public class FlyScript : MonoBehaviour,IPointerClickHandler
{
    // Start is called before the first frame update

    public LordOfFlies Lord;
    public int ID = 0;

    public float AnimationSpeed = 10;
    public List<Sprite> FlyAnimation = new List<Sprite>();
    public Sprite DeadSprite;


    UnityEngine.UI.Image m_Image;


    float m_ElapsedDeadTime = 0;
    IEnumerator p_DeadDelay()
    {
        
        while(m_ElapsedDeadTime < 0.5f)
        {
            m_ElapsedDeadTime += Time.deltaTime;
            yield return null;
        }
        Destroy(gameObject);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Lord.OnFlySwat(ID);
        StartCoroutine(p_DeadDelay());
    }

    void Start()
    {
        m_Image = GetComponent<UnityEngine.UI.Image>();
    }

    // Update is called once per frame

    int m_AnimationIndex = 0;
    float m_ElapsedAnimation = 0;
    void Update()
    {
        if(m_ElapsedDeadTime > 0)
        {
            m_Image.sprite = DeadSprite;
            return;
        }
        m_ElapsedAnimation += Time.deltaTime;
        if(m_ElapsedAnimation >= AnimationSpeed || (Lord.FlyActive() && m_ElapsedAnimation > AnimationSpeed/2))
        {
            m_AnimationIndex += 1;
            if(m_AnimationIndex >= FlyAnimation.Count)
            {
                m_AnimationIndex = 0;
            }
            m_ElapsedAnimation = 0;
            m_Image.sprite = FlyAnimation[m_AnimationIndex];
        }
    }
}
