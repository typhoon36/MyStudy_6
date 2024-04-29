using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class HeroCtrl : MonoBehaviour
{
    //--- 키보드 이동 관련 변수 선언
    float h, v;                 //키보드 입력값을 받기 위한 변수
    float m_MoveSpeed = 10.0f;  //초당 10m 이동속도

    Vector3 m_DirVec;           //이동하려는 방향 벡터 변수
    //--- 키보드 이동 관련 변수 선언

    //## 좌표 계산용 변수들...
    Vector3 m_CurPos;
    Vector3 m_CacEndVec;
    //--- 좌표 계산용 변수들...

    //## 총알 발사 관련 변수 선언
    float m_AttSpeed = 0.1f;  //공격속도(공속)
    float m_CacAtTick = 0.0f;  //기관총 발사 주기 만들기..
    float m_ShootRange = 30.0f; //사거리
    //--- 총알 발사 관련 변수 선언

    //##JoyStick 이동 처리 변수
    float m_JoyMvLen = 0.0f;
    Vector3 m_JoyMvDir = Vector3.zero;
    //--- JoyStick 이동 처리 변수

    //## 마우스 클릭 이동 관련 변수 (Mouse Picking Move)
    [HideInInspector] public bool m_bMoveOnOff = false; //현재 마우스 피킹으로 이동 중인지? 의 여부
    Vector3 m_TargetPos;   
    //마우스 피킹 목표점
    float m_CacStep;        
    //스탭 계산용 변수

    Vector3 m_PickVec = Vector3.zero;
    public ClickMark m_ClickMark = null;
    //--- 마우스 클릭 이동 관련 변수 (Mouse Picking Move)

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        MousePickCtrl();

        KeyBDUpdate();
        JoyStickMvUpdate();
        MousePickUpdate();

        //## 총알 발사 코드
        if (0.0f < m_CacAtTick)
            m_CacAtTick -= Time.deltaTime;

        if (Input.GetMouseButton(1) == true) //마우스 오른쪽 버튼 클릭시...
        {
            if (m_CacAtTick <= 0.0f)
            {
                Shoot_Fire(Camera.main.ScreenToWorldPoint(Input.mousePosition));

                m_CacAtTick = m_AttSpeed;
            }
        }
       
    }

    #region ## 키보드 이동
    void KeyBDUpdate()  //키보드 이동처리
    {
        h = Input.GetAxisRaw("Horizontal");
        v = Input.GetAxisRaw("Vertical");

        if (h != 0.0f || v != 0.0f)  //이동 키보드를 조작하고 있으면...
        {
            m_DirVec = (Vector3.right * h) + (Vector3.forward * v);
            if (1.0f < m_DirVec.magnitude)
                m_DirVec.Normalize();

            transform.Translate(m_DirVec * m_MoveSpeed * Time.deltaTime);
        }
    }//void KeyBDUpdate()  //키보드 이동처리

    #endregion

    #region ## 조이스틱 이동

    public void SetJoyStickMv(float a_JoyMvLen, Vector3 a_JoyMvDir)
    {
        m_JoyMvLen = a_JoyMvLen;
        if (0.0f < a_JoyMvLen)
        {
            m_JoyMvDir = new Vector3(a_JoyMvDir.x, 0.0f, a_JoyMvDir.y);
        }
    }

    public void JoyStickMvUpdate()
    {
        if (h != 0.0f || v != 0.0f)
            return;

        //--- 조이스틱 이동 코드
        if (0.0f < m_JoyMvLen)
        {
            m_DirVec = m_JoyMvDir;
            float a_MvStep = m_MoveSpeed * Time.deltaTime;
            transform.Translate(m_JoyMvDir * m_JoyMvLen * a_MvStep, Space.Self);
        }
    }

    #endregion

    #region  ## 마우스 클릭 이동

    float m_Tick = 0.0f;

    void MousePickCtrl()
    {
        //##누르고 있는 위치로 계속 이동 시키기...
        //if (0.0f < m_Tick)
        //    m_Tick -= Time.deltaTime;

        //if (m_Tick <= 0.0f)
        //{
        //    if (Input.GetMouseButton(0) == true)  //마우스 왼쪽 버튼 클릭시
        //    {
        //        m_PickVec = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        //        SetMsPicking(m_PickVec);
        //        m_Tick = 0.1f;
        //    }
        //}
       

        if (Input.GetMouseButtonDown(0) == true &&
            Game_Mgr.IsPointerOverUIObject() == false)  //마우스 왼쪽 버튼 클릭시
        {
            m_PickVec = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            SetMsPicking(m_PickVec);

            if (m_ClickMark != null)
                m_ClickMark.PlayEff(m_PickVec, this);
        }

    }


    void SetMsPicking(Vector3 a_Pos)
    {
        Vector3 a_CacVec = a_Pos - this.transform.position;
        a_CacVec.y = 0.0f;
        if (a_CacVec.magnitude < 1.0f)
            return;

        m_bMoveOnOff = true;

        m_DirVec = a_CacVec;
        m_DirVec.Normalize();
        m_TargetPos = new Vector3(a_Pos.x, transform.position.y, a_Pos.z);
    }

    void MousePickUpdate()
    {
        if (0.0f < m_JoyMvLen || (h != 0.0f || v != 0.0f)) //조이스틱, 키보드로 움직이는 중이면
            m_bMoveOnOff = false;   //즉시 마우스 이동 취소

        if (m_bMoveOnOff == true)
        {
            m_CacStep = Time.deltaTime * m_MoveSpeed;  //이번에 한걸음 길이(보폭)
            Vector3 a_CacEndVec = m_TargetPos - transform.position;
            a_CacEndVec.y = 0.0f;

            if (a_CacEndVec.magnitude <= m_CacStep)
            { //목표점까지의 거리보다 보폭이 크거나 같으면 도착으로 본다.
                //transform.position = m_TargetPos;
                m_bMoveOnOff = false;
            }
            else
            {
                m_DirVec = a_CacEndVec;
                m_DirVec.Normalize();
                transform.Translate(m_DirVec * m_CacStep, Space.World);
            }
        }//if(m_bMoveOnOff == true)
    }// void MousePickUpdate()

    #endregion

    public void Shoot_Fire(Vector3 a_Pos) 
        //매개변수로 목표 지점을 받는다.
    {  // 클릭 이벤트가 발생했을 때 호출.

        GameObject a_Obj = Instantiate(Game_Mgr.m_BulletPrefab);
        //오브젝트의 클론(복사체) 생성 

        m_CacEndVec = a_Pos - transform.position;
        m_CacEndVec.y = 0.0f;

        BulletCtrl a_BulletSc = a_Obj.GetComponent<BulletCtrl>();
        a_BulletSc.BulletSpawn(transform.position, m_CacEndVec.normalized, m_ShootRange);
    }
}
