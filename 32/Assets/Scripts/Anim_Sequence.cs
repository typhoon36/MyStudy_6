using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum UnitState
{
   Idle,
    //대기 상태
  Front_Walk,
  Back_Walk,
  Left_Walk,
  Right_Walk,
  Attack
}

public class Anim_Sequence : MonoBehaviour
{

    Renderer m_RefRender = null;


    public Texture[] m_Fnt_Idle = null;
    public Texture[] m_Front_Walk = null;
    public Texture[] m_Back_Walk = null;
    public Texture[] m_Left_Walk = null;
    public Texture[] m_Right_Walk = null;

    int m_FrameCount = 0;
    float m_EachAniDelay = 0.1f;
    float m_AniTickCount = 0.0f;
    int m_CurAniIdx = 0;
    Texture[] m_NowAniSocket = null;

    UnitState CurrentState = UnitState.Idle;




    // Start is called before the first frame update
    void Start()
    {
        m_RefRender = gameObject.GetComponent<Renderer>();

        m_EachAniDelay = 0.5f;
        m_NowAniSocket = m_Fnt_Idle;
        if(m_NowAniSocket !=null && 0 < m_NowAniSocket.Length)
        {
            
            m_CurAniIdx = 0;
            if(m_RefRender != null)
            {
                m_RefRender.material.SetTexture("_MainTex", m_NowAniSocket[m_CurAniIdx]);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
