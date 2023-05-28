using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogContainer : MonoBehaviour
{
    DefaultDialogReciever m_Reciever;
    // Start is called before the first frame update
    void Start()
    {
        m_Reciever = gameObject.GetComponentInChildren<DefaultDialogReciever>();
    }

    // Update is called once per frame
    void Update()
    {
        if(m_Reciever == null)
        {
            Destroy(gameObject);
        }
    }
}
