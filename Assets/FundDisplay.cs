using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FundDisplay : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        m_Displays = new List<TMPro.TextMeshProUGUI>(GetComponentsInChildren<TMPro.TextMeshProUGUI>());
    }
    List<TMPro.TextMeshProUGUI> m_Displays;

    public void SetOpponentFunds(int Funds)
    {
        m_Displays[0].text = "Oppponent: " + Funds + "$";
    }
    public void SetPlayerFunds(int Funds)
    {
        m_Displays[1].text = "You: " + Funds + "$";
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
