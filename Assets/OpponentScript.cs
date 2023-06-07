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

    public Sprite WinSprite;
    public Sprite LoseSprite;

    public TextAsset Dialog;


    public GameObject DialogObject;

    GameObject m_EyeObject;
    Dictionary<string, List<string>> m_Dialog = new Dictionary<string, List<string>>();

    public PokerState AssociatedPokerState;


    float ChangeDuration = 4f;

    bool m_InAnimation = false;

    Canvas m_AssociatedCanvas;


    public bool InAnimation()
    {
        return (m_InAnimation);
    }
    // Start is called before the first frame update
    void Start()
    {
        m_AssociatedCanvas = FindObjectOfType<Canvas>();

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

        if(Dialog == null)
        {
            return;
        }
        string CurrentTag = "";
        System.IO.StringReader Reader = new System.IO.StringReader(Dialog.text);
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
                m_Dialog.Add(CurrentTag, new List<string>());
            }
            else
            {
                m_Dialog[CurrentTag].Add(Line);
            }
        }
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
    IEnumerator p_DisplayDialog(string DialogString)
    {
        GameObject IngameDialogObject = Instantiate(DialogObject);
        Vector3 OriginalPosition = IngameDialogObject.transform.position;
        //IngameDialogObject.transform.localPosition = IngameDialogObject.transform.position;
        //IngameDialogObject.transform.position = new Vector3();
        IngameDialogObject.transform.parent = m_AssociatedCanvas.gameObject.transform;

        IngameDialogObject.transform.position = new Vector3();
        IngameDialogObject.transform.localPosition = OriginalPosition;
        IngameDialogObject.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = DialogString;

        float DialogDuration = ChangeDuration;
        float ElapsedTime = 0;
        while(ElapsedTime < DialogDuration)
        {
            ElapsedTime += Time.deltaTime;
            yield return null;
        }
        Destroy(IngameDialogObject);
    }
    void p_DisplayDialog(List<string> PossibleDialogs)
    {
        if(PossibleDialogs == null)
        {
            return;
        }
        string DialogToDisplay = PossibleDialogs[Random.Range(0, PossibleDialogs.Count)];
        StartCoroutine(p_DisplayDialog(DialogToDisplay));
    }

    public void OnWin()
    {
        StartCoroutine(p_ChangeSprite(WinSprite, ChangeDuration));
        if(m_Dialog.ContainsKey("Win"))
        {
            p_DisplayDialog(m_Dialog["Win"]);
        }
    }
    public void OnLose()
    {
        StartCoroutine(p_ChangeSprite(LoseSprite, ChangeDuration));
        if (m_Dialog.ContainsKey("Lose"))
        {
            p_DisplayDialog(m_Dialog["Lose"]);
        }
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
