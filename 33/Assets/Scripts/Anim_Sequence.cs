using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

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
        if (m_NowAniSocket !=null && 0 < m_NowAniSocket.Length)
        {

            m_CurAniIdx = 0;
            if (m_RefRender != null)
            {
                m_RefRender.material.SetTexture("_MainTex", m_NowAniSocket[m_CurAniIdx]);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        UpdateFrameAni();
    }

    public void UpdateFrameAni()
    {
        if (m_NowAniSocket == null)
            return;


        m_FrameCount = m_NowAniSocket.Length;

        if (m_FrameCount <= 0)
            return;

        m_AniTickCount += Time.deltaTime;

        //다음 프레임으로 넘어가기
        if (m_EachAniDelay < m_AniTickCount)
        {

            m_CurAniIdx++;

            //마지막 프레임시
            if (m_FrameCount <= m_CurAniIdx)

                m_CurAniIdx = 0;

            if (m_RefRender != null)
            {
                m_RefRender.material.SetTexture("_MainTex",
                    m_NowAniSocket[m_CurAniIdx]);

                m_AniTickCount = 0.0f;
            }

        }



    }
    public void ChangeAniState(UnitState a_newState)
    {
        if (CurrentState == a_newState)

            return;
        if (a_newState == UnitState.Idle)
        {
            if (m_Fnt_Idle == null)

                return;


            if (m_Fnt_Idle.Length <= 0)

                return;

            m_NowAniSocket = m_Fnt_Idle;
        }

        else if (a_newState == UnitState.Front_Walk)
        {
            if (m_Front_Walk == null)

                return;

            if (m_Front_Walk.Length <= 0)

                return;

            m_NowAniSocket = m_Front_Walk;
        }

        else if (a_newState == UnitState.Back_Walk)
        {
            if (m_Back_Walk == null)

                return;

            if (m_Back_Walk.Length <= 0)

                return;

            m_NowAniSocket = m_Back_Walk;
        }

        else if (a_newState == UnitState.Left_Walk)
        {
            if (m_Left_Walk == null)

                return;

            if (m_Left_Walk.Length <= 0)

                return;

            m_NowAniSocket = m_Left_Walk;
        }

        else if (a_newState == UnitState.Right_Walk)
        {
            if (m_Right_Walk == null)

                return;

            if (m_Right_Walk.Length <= 0)

                return;

            m_NowAniSocket = m_Right_Walk;
        }

        if(a_newState == UnitState.Idle)
        m_EachAniDelay = 0.5f;
        else
            m_EachAniDelay = 0.15f;


        m_CurAniIdx = 0;
        m_AniTickCount = 0.0f;
        CurrentState = a_newState;
        if(m_RefRender != null)
            m_RefRender.material.SetTexture("_MainTex",
                m_NowAniSocket[m_CurAniIdx]);

    }

    //##캐릭터의 이동방향에 따라 모션상태 변경 
    public void CheckAnimDir(float a_Angle)
    {
        if(50.0f<a_Angle && a_Angle < 130.0f)
        {
            ChangeAniState(UnitState.Right_Walk);
        }

        //130~230도
         else if(130.0f <= a_Angle && a_Angle <= 230.0f)
        {
            ChangeAniState(UnitState.Front_Walk);
        }

        //230~310도
        else if(230.0f < a_Angle && a_Angle < 310.0f)
        {
            ChangeAniState(UnitState.Left_Walk);
        }

        //310~360도, 0~50도
        else
        {
            ChangeAniState(UnitState.Back_Walk);
        }


    }


}
