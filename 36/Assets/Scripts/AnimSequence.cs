using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public enum UnitState
{
    Idle,
    Front_Walk,
    Back_Walk,
    Left_Walk,
    Right_Walk,
    Attack
}

public class AnimSequence : MonoBehaviour
{
    Renderer m_RefRender = null;

    public Texture[] m_Frt_Idle = null;
    public Texture[] m_Front_Wk = null;
    public Texture[] m_Back_Wk  = null;
    public Texture[] m_Left_Wk  = null;
    public Texture[] m_Right_Wk = null;

    int m_FrameCount = 0;
    float m_EachAniDelay = 0.1f;
    float m_AniTickCount = 0.0f;
    int m_CurAniInx = 0;
    Texture[] m_NowAniSocket = null;

    UnitState currentState = UnitState.Idle;

    // Start is called before the first frame update
    void Start()
    {
        m_RefRender = gameObject.GetComponent<Renderer>();

        m_EachAniDelay = 0.5f;
        m_NowAniSocket = m_Frt_Idle;
        if(m_NowAniSocket != null && 0 < m_NowAniSocket.Length)
        {
            m_CurAniInx = 0;
            if (m_RefRender != null)
                m_RefRender.material.SetTexture("_MainTex", m_NowAniSocket[m_CurAniInx]);
        }
    }

    // Update is called once per frame
    void Update()
    {
        UpdateFrameAni();
    }

    public void UpdateFrameAni()
    {
        if(m_NowAniSocket == null)
            return;

        m_FrameCount = m_NowAniSocket.Length;

        if (m_FrameCount <= 0)
            return;

        m_AniTickCount += Time.deltaTime;
        if(m_EachAniDelay < m_AniTickCount) //다음 플레임
        {
            m_CurAniInx++;
            if (m_FrameCount <= m_CurAniInx)  //마지막 플레임일 때
                m_CurAniInx = 0;

            if (m_RefRender != null)
                m_RefRender.material.SetTexture("_MainTex", m_NowAniSocket[m_CurAniInx]);

            m_AniTickCount = 0.0f;
        }
    }

    public void ChangeAniState(UnitState a_NewState)
    {
        if (currentState == a_NewState)
            return;

        if(a_NewState == UnitState.Idle)
        {
            if (m_Frt_Idle == null) return;
            if (m_Frt_Idle.Length <= 0) return;
            m_NowAniSocket = m_Frt_Idle;
        }
        else if(a_NewState == UnitState.Front_Walk)
        {
            if (m_Front_Wk == null) return;
            if (m_Front_Wk.Length <= 0) return;
            m_NowAniSocket = m_Front_Wk;
        }
        else if(a_NewState == UnitState.Back_Walk)
        {
            if (m_Back_Wk == null) return;
            if (m_Back_Wk.Length <= 0) return;
            m_NowAniSocket = m_Back_Wk;
        }
        else if(a_NewState == UnitState.Left_Walk)
        {
            if (m_Left_Wk == null) return;
            if (m_Left_Wk.Length <= 0) return;
            m_NowAniSocket = m_Left_Wk;
        }
        else if(a_NewState == UnitState.Right_Walk)
        {
            if (m_Right_Wk == null) return;
            if(m_Right_Wk.Length <= 0) return;
            m_NowAniSocket = m_Right_Wk;
        }

        if (a_NewState == UnitState.Idle)
            m_EachAniDelay = 0.5f;
        else
            m_EachAniDelay = 0.15f; //딜레이를 다르게 주고 싶을 때

        m_CurAniInx = 0;
        m_AniTickCount = 0;
        currentState = a_NewState;
        if(m_RefRender != null)
            m_RefRender.material.SetTexture("_MainTex", m_NowAniSocket[m_CurAniInx]);

    }//public void ChangeAniState(UnitState a_NewState)

    //캐릭터의 이동방향에 따라서 애니메이션 모션 상태를 바꿔주는 함수
    public void CheckAnimDir(float a_Angle)
    {
        if(50.0f < a_Angle && a_Angle < 130.0f)
        {
            ChangeAniState(UnitState.Right_Walk);
        }
        else if(130.0f <= a_Angle && a_Angle <= 230.0f)
        {
            ChangeAniState(UnitState.Front_Walk);
        }
        else if(230.0f < a_Angle && a_Angle < 310.0f)
        {
            ChangeAniState(UnitState.Left_Walk);
        }
        else
        {
            ChangeAniState(UnitState.Back_Walk);
        }
    }//public void CheckAnimDir(float a_Angle)

}
