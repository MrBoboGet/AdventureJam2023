using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pass : MonoBehaviour
{
    // Start is called before the first frame update

    PokerHandler m_AssociatedHandler;
    void Start()
    {
        m_AssociatedHandler = FindObjectOfType<PokerHandler>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnPass()
    {
        m_AssociatedHandler.OnPass();
    }
}
