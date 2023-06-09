using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hunk : OpponentScript
{
    public List<Sprite> FlexSprites;

    public GameObject ParticleEmitter;
    // Start is called before the first frame update
    bool m_Flexing = false;
    bool m_InFlexInitAnimation = false;



    protected override void p_DisplayDialogCategory(string CategoryName)
    {
        if(m_Flexing)
        {
            base.p_DisplayDialogCategory("Hunkwalla" + CategoryName);
        }
        else
        {
            base.p_DisplayDialogCategory(CategoryName);
        }
    }
    override  public void DisplayDialog(string DialogID, System.Action ResultingAction)
    {
        base.DisplayDialog(DialogID, ResultingAction);
        if(DialogID == "2")
        {
            p_InitializeFlexSequence();
        }
    }

    void p_InitializeFlexSequence()
    {
        m_Flexing = true;
        GetComponent<SpriteRenderer>().sprite = FlexSprites[m_CurrentFlexIndex];
        GameObject Emitter = Instantiate(ParticleEmitter);
        Emitter.transform.position = gameObject.transform.position;
        FindObjectOfType<Stress>().LockMaxStress();
        m_SpriteLock = true;
    }


    public override void HoverEnter(int CardIndex)
    {
        if(!m_Flexing)
        {
            base.HoverEnter(CardIndex);
        }
    }
    public override void HoverLeave()
    {
        if (!m_Flexing)
        {
            base.HoverLeave();
        }
    }
    void Start()
    {
        base.Start();
    }



    public override bool InAnimation()
    {
        return m_InAnimation || m_InFlexInitAnimation;
    }
    // Update is called once per frame

    public float FlexDelay = 0.333f;
    int m_CurrentFlexIndex = 0;
    float m_ElapsedFlexTime = 0;

    void Update()
    {
        base.Update();
        if(m_Flexing)
        {
            m_ElapsedFlexTime += Time.deltaTime;
            if(m_ElapsedFlexTime >= FlexDelay)
            {
                int NewFlexIndex = Random.Range(0,FlexSprites.Count);
                while(NewFlexIndex == m_CurrentFlexIndex)
                {
                    NewFlexIndex = Random.Range(0, FlexSprites.Count);
                }
                m_CurrentFlexIndex = NewFlexIndex;

                GetComponent<SpriteRenderer>().sprite = FlexSprites[m_CurrentFlexIndex];
                m_ElapsedFlexTime = 0;
            }
        }

        if(Input.GetKeyDown(KeyCode.F))
        {
            p_InitializeFlexSequence();
        }
    }
}

