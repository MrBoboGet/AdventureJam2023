using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpponentScript : MonoBehaviour
{
    public float EyeRadius = 0.1f;
    public Sprite NeutralSprite;
    public Sprite WhiteEye;
    public Sprite EyeSprite;
    public Sprite TellSprite;

    GameObject m_EyeObject;


    public PokerState AssociatedPokerState;

    // Start is called before the first frame update
    void Start()
    {
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
    }

    public void SetEyeDirection(Vector2 EyeDirection)
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
    public void HoverLeave()
    {
        GetComponent<SpriteRenderer>().sprite = NeutralSprite;
    }
    public void OnWin()
    {

    }
    public void OnLoose()
    {

    }
    public void OnThink()
    {

    }
    public void OnFold()
    {

    }
    public void OnOpponentFold()
    {

    }
    public void OnRaise()
    {

    }
    public void OnOpponentRaise()
    {

    }
    public void OnMatch()
    {

    }
    public void OnOpponentMatch()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
