using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
public class CardScript : MonoBehaviour,IDragHandler,IPointerClickHandler,IDropHandler
{
    Vector2 m_OriginalPosition;
    // Start is called before the first frame update
    void Start()
    {
        m_OriginalPosition = transform.position;
    }
    public void OnDrag(PointerEventData Data)
    {
        transform.position = Data.position;
        print("asdsad");
    }
    // Update is called once per frame
    void Update()
    {
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        print("Clicking");
    }

    public void OnDrop(PointerEventData eventData)
    {
        Camera GlobalCamera = FindObjectOfType<Camera>();
        RaycastHit2D TableHit = Physics2D.Raycast(
            GlobalCamera.ScreenToWorldPoint(eventData.position), GlobalCamera.transform.forward,Mathf.Infinity,1<<LayerMask.NameToLayer("Table"));
        if(TableHit.collider == null)
        {
            transform.position = m_OriginalPosition;
        }
        else
        {
            transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
            Destroy(this);
        }
    }
}
