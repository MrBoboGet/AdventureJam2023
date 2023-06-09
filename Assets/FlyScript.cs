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


    UnityEngine.UI.Image m_Image;
    public void OnPointerClick(PointerEventData eventData)
    {
        Lord.OnFlySwat(ID);
        Destroy(gameObject);
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
        m_ElapsedAnimation += Time.deltaTime;
        if(m_ElapsedAnimation >= AnimationSpeed)
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
