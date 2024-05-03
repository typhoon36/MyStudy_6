using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class HeroCtrl : MonoBehaviour
{
    //--- Ű���� �̵� ���� ���� ����
    float h, v;                 //Ű���� �Է°��� �ޱ� ���� ����
    float m_MoveSpeed = 10.0f;  //�ʴ� 10m �̵��ӵ�

    Vector3 m_DirVec;           //�̵��Ϸ��� ���� ���� ����
    //--- Ű���� �̵� ���� ���� ����

    //## ��ǥ ���� ������...
    Vector3 m_CurPos;
    Vector3 m_CacEndVec;
    //--- ��ǥ ���� ������...

    //## �Ѿ� �߻� ���� ���� ����
    float m_AttSpeed = 0.1f;  //���ݼӵ�(����)
    float m_CacAtTick = 0.0f;  //����� �߻� �ֱ� �����..
    float m_ShootRange = 30.0f; //��Ÿ�
    //--- �Ѿ� �߻� ���� ���� ����

    //##JoyStick �̵� ó�� ����
    float m_JoyMvLen = 0.0f;
    Vector3 m_JoyMvDir = Vector3.zero;
    //--- JoyStick �̵� ó�� ����

    //## ���콺 Ŭ�� �̵� ���� ���� (Mouse Picking Move)
    [HideInInspector] public bool m_bMoveOnOff = false; //���� ���콺 ��ŷ���� �̵� ������? �� ����
    Vector3 m_TargetPos;   
    //���콺 ��ŷ ��ǥ��
    float m_CacStep;        
    //���� ���� ����

    Vector3 m_PickVec = Vector3.zero;
    public ClickMark m_ClickMark = null;
    //--- ���콺 Ŭ�� �̵� ���� ���� (Mouse Picking Move)

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

        //## �Ѿ� �߻� �ڵ�
        if (0.0f < m_CacAtTick)
            m_CacAtTick -= Time.deltaTime;

        if (Input.GetMouseButton(1) == true) //���콺 ������ ��ư Ŭ����...
        {
            if (m_CacAtTick <= 0.0f)
            {
                Shoot_Fire(Camera.main.ScreenToWorldPoint(Input.mousePosition));

                m_CacAtTick = m_AttSpeed;
            }
        }
       
    }

    #region ## Ű���� �̵�
    void KeyBDUpdate()  //Ű���� �̵�ó��
    {
        h = Input.GetAxisRaw("Horizontal");
        v = Input.GetAxisRaw("Vertical");

        if (h != 0.0f || v != 0.0f)  //�̵� Ű���带 �����ϰ� ������...
        {
            m_DirVec = (Vector3.right * h) + (Vector3.forward * v);
            if (1.0f < m_DirVec.magnitude)
                m_DirVec.Normalize();

            transform.Translate(m_DirVec * m_MoveSpeed * Time.deltaTime);
        }
    }//void KeyBDUpdate()  //Ű���� �̵�ó��

    #endregion

    #region ## ���̽�ƽ �̵�

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

        //--- ���̽�ƽ �̵� �ڵ�
        if (0.0f < m_JoyMvLen)
        {
            m_DirVec = m_JoyMvDir;
            float a_MvStep = m_MoveSpeed * Time.deltaTime;
            transform.Translate(m_JoyMvDir * m_JoyMvLen * a_MvStep, Space.Self);
        }
    }

    #endregion

    #region  ## ���콺 Ŭ�� �̵�

    float m_Tick = 0.0f;

    void MousePickCtrl()
    {
        //##������ �ִ� ��ġ�� ��� �̵� ��Ű��...
        //if (0.0f < m_Tick)
        //    m_Tick -= Time.deltaTime;

        //if (m_Tick <= 0.0f)
        //{
        //    if (Input.GetMouseButton(0) == true)  //���콺 ���� ��ư Ŭ����
        //    {
        //        m_PickVec = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        //        SetMsPicking(m_PickVec);
        //        m_Tick = 0.1f;
        //    }
        //}
       

        if (Input.GetMouseButtonDown(0) == true &&
            Game_Mgr.IsPointerOverUIObject() == false)  //���콺 ���� ��ư Ŭ����
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
        if (0.0f < m_JoyMvLen || (h != 0.0f || v != 0.0f)) //���̽�ƽ, Ű����� �����̴� ���̸�
            m_bMoveOnOff = false;   //��� ���콺 �̵� ���

        if (m_bMoveOnOff == true)
        {
            m_CacStep = Time.deltaTime * m_MoveSpeed;  //�̹��� �Ѱ��� ����(����)
            Vector3 a_CacEndVec = m_TargetPos - transform.position;
            a_CacEndVec.y = 0.0f;

            if (a_CacEndVec.magnitude <= m_CacStep)
            { //��ǥ�������� �Ÿ����� ������ ũ�ų� ������ �������� ����.
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
        //�Ű������� ��ǥ ������ �޴´�.
    {  // Ŭ�� �̺�Ʈ�� �߻����� �� ȣ��.

        GameObject a_Obj = Instantiate(Game_Mgr.m_BulletPrefab);
        //������Ʈ�� Ŭ��(����ü) ���� 

        m_CacEndVec = a_Pos - transform.position;
        m_CacEndVec.y = 0.0f;

        BulletCtrl a_BulletSc = a_Obj.GetComponent<BulletCtrl>();
        a_BulletSc.BulletSpawn(transform.position, m_CacEndVec.normalized, m_ShootRange);
    }
}
