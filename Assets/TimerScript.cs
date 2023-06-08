using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimerScript : MonoBehaviour
{
    public float MaxTime = 5f;

    float m_ElapsedTime = 0f;

    Animator m_Animator;

    public void ResetTimer()
    {
        m_ElapsedTime = 0;
    }
    // Start is called before the first frame update
    void Start()
    {
        m_Animator = GetComponent<Animator>();
    }
    public bool TimerUp()
    {
        return (m_ElapsedTime >= MaxTime);
    }

    // Update is called once per frame
    void Update()
    {
        m_ElapsedTime += Time.deltaTime;
        m_Animator.Play(m_Animator.GetCurrentAnimatorStateInfo(0).fullPathHash,-1, Mathf.Min(m_ElapsedTime / MaxTime, 1));
    }
}
