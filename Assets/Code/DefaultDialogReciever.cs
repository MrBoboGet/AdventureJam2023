using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class DefaultDialogReciever : MonoBehaviour, TextReciever
{
    TMPro.TextMeshProUGUI m_TextBox = null;
    // Start is called before the first frame update
    void Start()
    {
        m_TextBox = GetComponent<TMPro.TextMeshProUGUI>();
        foreach(Dialog Diag in GetComponents<Dialog>())
        {
            Diag.SetReciever(this);
        }
    }
    public void SetText(string TextToSet)
    {
        m_TextBox.text = TextToSet;
    }
    public void NewDialog()
    {
        m_TextBox.text = "";
    }
    public void Finish()
    {
        Destroy(this.gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
