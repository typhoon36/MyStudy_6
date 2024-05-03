using System.Collections;
using System.Collections.Generic;
using TreeEditor;
using UnityEngine;
using UnityEngine.UI;

public enum MonAiState
{
    MAI_IDLE,
    //일반 상태
    MAI_Patrol,
    //순찰 상태
    MAI_AggroTrace,
    //공격당했을때의 추적 상태
    MAI_NormalTrace,
    //일반 추적 상태
    MAI_ReturnPos,
    //놓치면 원래 위치로 돌아가는 상태
    MAI_Attack
    //공격 상태
}



public class MonsterCtrl : MonoBehaviour
{
    float m_MaxHp = 100.0f;
    float m_CurHp = 100.0f;
    public Image HpBarUI = null;

    //## 몬스터의 AI 변수
    MonAiState m_AiState = MonAiState.MAI_Patrol;

    GameObject m_AggroTarget = null;
    //추적 타겟

    float m_AttackDist = 19.5f;

    //공격 거리

    float m_TraceDist = 20.0f;
    //추적 거리


    float m_MoveVelocity = 2.0f;
    //평면 이동 속도(패트롤 기준)

    Vector3 m_MoveDir = Vector3.zero;
    //평면에서의 이동 방향

    float m_NowStep = 0.0f;
    //이동계산용 변수


    //## 공격주기 변수
    float m_ShootCool = 1.0f;
    //주기 변수
    float m_AttackSpeed = 0.5f;
    //공격 속도

    //## 계산용 변수
    Vector3 a_CacVLen = Vector3.zero;
    float a_CacDist = 0.0f;

    //## 패트롤 필요 변수
    Vector3 m_BasePos = Vector3.zero;
    //기본 스폰 위치(기준점)

    bool m_bMvPtOnOff = false;
    //패트롤 온오프

    float m_WaitTime = 0.0f;

    int a_AngleRan;
    int a_LenthRan;


    Vector3 m_PatrolTarget = Vector3.zero;
    //패트롤시 움직일 목표 위치

    Vector3 m_DirMvVec = Vector3.zero;
    //패트롤시 이동 방향벡터

    double m_AddTimeCount = 0.0f;
    //이동 총 누적 시간
    double m_MoveDurTime = 0.0f;
    //목표점까지 이동하는데 걸리는 시간

    Quaternion a_CacPtRot;


    Vector3 a_CacPtAngle = Vector3.zero;

    Vector3 a_Vert;




    // Start is called before the first frame update
    void Start()
    {
        m_BasePos = this.transform.position;
        m_WaitTime = Random.Range(0.5f, 3.0f);
        m_bMvPtOnOff = false;
    }

    // Update is called once per frame
    void Update()
    {
        MonsterAI();
    }

    void OnTriggerEnter(Collider Coll)
    {
        if (Coll.gameObject.name.Contains("BulletPrefab") == true)
        {
            // if (Coll.gameObject.GetComponent<BulletCtrl>().m_AllyType == AllyType.BT_Enermy)
            if (Coll.gameObject.CompareTag(AllyType.BT_Enermy.ToString())== true)
                return; //아군의 총알이면 무시


            TakeDamage(10.0f);

            Destroy(Coll.gameObject);

        }
    }// void OnTriggerEnter(Collider Coll)

    public void TakeDamage(float a_Value)
    {
        if (m_CurHp <= 0.0f)
            return;

        Game_Mgr.Inst.DamageText((int)a_Value, this.transform.position);

        m_CurHp -= a_Value;
        if (m_CurHp < 0.0f)
            m_CurHp = 0.0f;

        if (HpBarUI != null)
            HpBarUI.fillAmount = m_CurHp / m_MaxHp;
        m_AggroTarget = Game_Mgr.Inst.m_RefHero.gameObject;
        m_AiState = MonAiState.MAI_AggroTrace;




        if (m_CurHp <= 0.0f)  //몬스터 사망 처리
        {
            // 보상
            ItemDrop();

            Destroy(gameObject);
        }



    }//public void TakeDamage(float a_Value)


    void MonsterAI()
    {

        if (0.0f < m_ShootCool)
            m_ShootCool -= Time.deltaTime;


        //몬스터가 패트롤일시 
        if (m_AiState == MonAiState.MAI_Patrol)
        {
            if (Game_Mgr.Inst.m_RefHero != null)
            {
                Vector3 a_CacVLen = Game_Mgr.Inst.m_RefHero.transform.position
                    - this.transform.position;

                a_CacVLen.y = 0.0f;
                a_CacDist = a_CacVLen.magnitude;
                if (a_CacDist < m_TraceDist)
                {


                    m_AiState = MonAiState.MAI_NormalTrace;
                    m_AggroTarget = Game_Mgr.Inst.m_RefHero.gameObject;
                    return;
                }
            }

            //## AI 패트롤 기능
            AI_Patrol();


        }
        else if (m_AiState == MonAiState.MAI_NormalTrace)
        {
            if (m_AggroTarget == null)
            {
                m_AiState = MonAiState.MAI_Patrol;
                return;
            }

            Vector3 a_CacVLen = m_AggroTarget.transform.position
                                     - this.transform.position;
            a_CacVLen.y = 0.0f;
            a_CacDist = a_CacVLen.magnitude;

            if (a_CacDist < m_AttackDist)
            {
                m_AiState = MonAiState.MAI_Attack;
            }
            else if (a_CacDist < m_TraceDist)
            {
                m_MoveDir = a_CacVLen.normalized;
                m_MoveDir.y = 0.0f;
                m_NowStep = m_MoveVelocity *1.5f * Time.deltaTime;
                //걸음당 크기

                //일반 패트롤상태의 이동속도보다 빠르게 이동

                transform.Translate(m_MoveDir * m_NowStep, Space.World);
            }
            else
            {
                m_AiState = MonAiState.MAI_Patrol;
            }
        }

        //## 추적상태
        else if (m_AiState == MonAiState.MAI_AggroTrace)
        {
            if (m_AggroTarget == null)
            {
                m_AiState = MonAiState.MAI_Patrol;
            }

            a_CacVLen = m_AggroTarget.transform.position
                - this.transform.position;
            a_CacVLen.y = 0.0f;

            a_CacDist = a_CacVLen.magnitude;

            //## 공격거리 안에 들어오면 공격상태로 전환
            if (a_CacDist < m_AttackDist)
            {
                m_AiState = MonAiState.MAI_Attack;

            }

            if ((m_AttackDist - 2.0f) < a_CacDist) //공격거리 2m 이내로 접근 
            {
                m_MoveDir = a_CacVLen.normalized;
                m_MoveDir.y = 0.0f;
                m_NowStep = m_MoveVelocity * 10.0f * Time.deltaTime;


                transform.Translate(m_MoveDir * m_NowStep, Space.World);


            }


        }



        //## 공격상태
        else if (m_AiState == MonAiState.MAI_Attack)
        {
            if (m_AggroTarget == null)
            {
                m_AiState = MonAiState.MAI_Patrol;
                return;
            }

            Vector3 a_CacVLen = m_AggroTarget.transform.position
                - this.transform.position;

            a_CacDist = a_CacVLen.magnitude;

            if ((m_AttackDist - 2.0f) < a_CacDist)
            {
                m_MoveDir = a_CacVLen.normalized;
                m_MoveDir.y = 0.0f;
                m_NowStep = m_MoveVelocity * 1.5f *Time.deltaTime;
                transform.Translate(m_MoveDir * m_NowStep, Space.World);
            }

            if (a_CacDist < m_AttackDist)
            {
                //공격
                if (m_ShootCool <= 0.0f)
                {
                    ShootFire(); //공격
                    m_ShootCool = m_AttackSpeed;
                    //공격 주기 초기화
                }
            }
            else
            {
                m_AiState = MonAiState.MAI_NormalTrace;
            }
        }//else if (m_AiState == MonAiState.MAI_Attack)

    }//  void MonsterAI()

    void AI_Patrol()
    {
        if (m_bMvPtOnOff == true)
        {
            m_DirMvVec = m_PatrolTarget - this.transform.position;
            m_DirMvVec.y = 0.0f;
            m_DirMvVec.Normalize();

            m_AddTimeCount += Time.deltaTime;
            if (m_MoveDurTime <= m_AddTimeCount)
                m_bMvPtOnOff = false; //목표지점에 도착했으면 이동 중지
            else
            {
                transform.Translate(m_DirMvVec * m_MoveVelocity * Time.deltaTime, Space.World);
            }

        }
        else
        {
            m_WaitTime -= Time.deltaTime;
            if (0.0f < m_WaitTime)
            {
                return;
            }

            m_WaitTime = 0.0f;
            a_AngleRan = Random.Range(30, 301);
            a_LenthRan = Random.Range(3, 8);

            m_DirMvVec = transform.position - m_BasePos;
            m_DirMvVec.y = 0.0f;

            if (m_DirMvVec.magnitude < 1.0f)
                a_CacPtRot = Quaternion.LookRotation(transform.forward);
            else
                a_CacPtRot = Quaternion.LookRotation(m_DirMvVec);

            a_CacPtAngle = a_CacPtRot.eulerAngles;
            a_CacPtAngle.y += a_CacPtAngle.y + (float)a_AngleRan;
            a_CacPtRot.eulerAngles = a_CacPtAngle;

            a_Vert = new Vector3(0.0f, 0.0f, 1.0f);
            a_Vert = a_CacPtRot * a_Vert;

            a_Vert.Normalize();


            m_PatrolTarget = m_BasePos + (a_Vert * (float)a_LenthRan);


            m_DirMvVec = m_PatrolTarget - this.transform.position;
            m_DirMvVec.y = 0.0f;


            m_MoveDurTime = m_DirMvVec.magnitude / m_MoveVelocity;
            //목표지점까지 이동하는데 걸리는 시간 계산


            m_AddTimeCount = 0.0f;

            m_DirMvVec.Normalize();

            m_WaitTime = Random.Range(0.2f, 3.0f);
            m_bMvPtOnOff = true;



        }

    }



    //## 적 총알 발사
    void ShootFire()
    {
        if (m_AggroTarget == null)
            return;

        a_CacVLen = m_AggroTarget.transform.position
             - this.transform.position;
        a_CacVLen.y = 0.0f;
        Vector3 a_CacDir = a_CacVLen.normalized;

        GameObject a_BLClone = Instantiate(Game_Mgr.m_BulletPrefab);
        BulletCtrl a_BulletSc = a_BLClone.GetComponent<BulletCtrl>();

        a_BLClone.tag = AllyType.BT_Enermy.ToString();

        a_BulletSc.BulletSpawn(transform.position, a_CacVLen, 30.0f);



    }

    public void ItemDrop()
    {
        int a_Rnd = Random.Range(0, 6);

        GameObject a_Item = null;
        a_Item  = (GameObject)Instantiate(Resources.Load("Item_Obj"));
        a_Item.transform.position = new Vector3(transform.position.x, 0.7f,
           transform.position.z);

        if (a_Rnd == 0)
        {
            a_Item.name = "coin_Item_Obj";
        }
        else if (a_Rnd == 1)
        {
            a_Item.name = "bomb_Item_Obj";
        }
        else
        {
            Item_Type a_ItType = (Item_Type)a_Rnd;
            a_Item.name = a_ItType.ToString() + "_Item_Obj";
        }


        ItemObjInfo a_RefItmeInfo = a_Item.GetComponent<ItemObjInfo>();
        if (a_RefItmeInfo != null)
        {
            a_RefItmeInfo.InitItem((Item_Type)a_Rnd, a_Item.name,
                Random.Range(1, 6), Random.Range(1, 6));
        }



    }



}
