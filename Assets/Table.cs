using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Table : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        m_Collider = GetComponent<BoxCollider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    BoxCollider2D m_Collider;
    public Vector2 GetDimensions()
    {
        return new Vector2(m_Collider.size.x * transform.localScale.x , m_Collider.size.y * transform.localScale.y);
    }
}
