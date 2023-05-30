using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
public class CardScript : MonoBehaviour,IDragHandler,IDropHandler
{
    Vector2 m_OriginalPosition;
    public Card CardValue;
    public int CardIndex = 0;
    public PokerHandler AssociatedHandler;
    public bool Hover = false;
    // Start is called before the first frame update
    void Start()
    {
        m_OriginalPosition = transform.position;
    }
    public void OnDrag(PointerEventData Data)
    {
        transform.position = Data.position;
    }
    // Update is called once per frame
    void Update()
    {
    }



    public void Drop()
    {
        transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
        enabled = false;
    }
    public void SetPosition(Vector2 NewPosition)
    {
        m_OriginalPosition = NewPosition;
        gameObject.transform.localPosition = NewPosition;
    }
    public void ResetPosition()
    {
        gameObject.transform.position = m_OriginalPosition;
    }
    public void OnDrop(PointerEventData eventData)
    {
        Camera GlobalCamera = FindObjectOfType<Camera>();
        RaycastHit2D TableHit = Physics2D.Raycast(
            GlobalCamera.ScreenToWorldPoint(eventData.position), GlobalCamera.transform.forward,Mathf.Infinity,1<<LayerMask.NameToLayer("Table"));
        if(TableHit.collider == null)
        {
            //transform.position = m_OriginalPosition;
            RaycastHit2D OpponentHit = Physics2D.Raycast(
                GlobalCamera.ScreenToWorldPoint(eventData.position), GlobalCamera.transform.forward, Mathf.Infinity, 1 << LayerMask.NameToLayer("Opponent"));
            if(OpponentHit.collider != null)
            {
                AssociatedHandler.CardDropped(this, DropType.Opponent);
            }
            else
            {
                ResetPosition();
            }
        }
        else
        {
            AssociatedHandler.CardDropped(this, DropType.Table);
        }
    }
}
