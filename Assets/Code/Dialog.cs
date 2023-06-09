using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public interface TextReciever
{
    void SetText(string StringToAdd);
    void NewDialog();
    void Finish();
}

public class Dialog : MonoBehaviour
{
    public List<string> TextBoxes = new List<string>();
    public float CharacterSpeed = 10;

    TextReciever m_AssociatedReciever = null;
    float m_CurrentCharacters = 0;
    int m_CurrentTextIndex = 0;
    // Start is called before the first frame update
    void Start()
    {

    }
    public void SetReciever(TextReciever textReciever)
    {
        m_AssociatedReciever = textReciever;
    }

    System.Action m_FinishedAction = null;
    public void SetDoneAction(System.Action WhenFinished)
    {
        m_FinishedAction = WhenFinished;
    }
    // Update is called once per frame
    void Update()
    {
        if(m_AssociatedReciever == null)
        {
            return;
        }
        if(m_CurrentTextIndex >= TextBoxes.Count)
        {
            return;
        }

        if (m_CurrentTextIndex < TextBoxes.Count)
        {
            m_CurrentCharacters += CharacterSpeed * Time.deltaTime;
            if(m_CurrentCharacters >= TextBoxes[m_CurrentTextIndex].Length)
            {
                m_CurrentCharacters = TextBoxes[m_CurrentTextIndex].Length;
            }
            string CurrentText = TextBoxes[m_CurrentTextIndex].Substring(0,(int)m_CurrentCharacters);
            m_AssociatedReciever.SetText(CurrentText);
        }
        if (Input.GetKeyDown(KeyCode.Return))
        {
            if (m_CurrentCharacters < TextBoxes[m_CurrentTextIndex].Length)
            {
                m_CurrentCharacters = TextBoxes[m_CurrentTextIndex].Length;
            }
            else if(m_CurrentTextIndex + 1 < TextBoxes.Count)
            {
                m_CurrentTextIndex += 1;
                m_CurrentCharacters = 0;
                m_AssociatedReciever.NewDialog();
            }
            else
            {
                m_AssociatedReciever.Finish();
                m_FinishedAction();
            }
        }
    }
}
