using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

enum JoyStickType
{
    Fixed = 0,
    Flexible = 1,
    FlexibleOnOff = 2
}

//Terrain, NaviMesh <-- X, Z ��鿡�� �����Ǵ� ����Ƽ ���

public class Game_Mgr : MonoBehaviour
{
    public static GameObject m_BulletPrefab = null;

    public Button m_BackBtn;

    //--- UserInfo UI ���� ����
    [Header("--- UserInfo UI ---")]
    [SerializeField] private Button m_UserInfo_Btn = null;
    bool m_UInfoIsOn = false;
    public Text m_UserHPText = null;
    public GameObject m_UserInfo_Panel = null;
    public Text m_GoldText= null;
    public Text m_SkillText = null;
    public Text m_MonKillText = null;
 

    int m_CurGold = 0;
    int m_MonKillCnt = 0;
    //���� ų��



    //--- UserInfo UI ���� ����

    //--- Fixed JoyStick ó�� �κ�
    JoyStickType m_JoyStickType = JoyStickType.Fixed;

    [Header("--- JoyStick ---")]
    public GameObject m_JoySBackObj = null;
    public Image m_JoyStickImg = null;
    float m_Radius = 0.0f;
    Vector3 m_OriginPos = Vector3.zero;
    Vector3 m_Axis = Vector3.zero;
    Vector3 m_JsCacVec = Vector3.zero;
    float m_JsCacDist = 0.0f;
    //--- Fixed JoyStick ó�� �κ�

    //--- Flexible JoyStick ó�� �κ�
    public GameObject m_JoystickPickPanel = null;
    Vector3 posJoyBack;
    Vector3 dirStick;
    //--- Flexible JoyStick ó�� �κ�

    //--- �Ӹ����� ������ ����� ���� ����
    Vector3 m_StCacPos = Vector3.zero;
    [Header("--- Damage Text ---")]
    public Transform m_Damage_Canvas = null;
    public GameObject m_DamageTxtRoot = null;
    //--- �Ӹ����� ������ ����� ���� ����

    //--- Inventory ScrollView
    [Header("--- Inventory ScrollView OnOff ---")]
    public Button m_Inven_Btn = null;
    public Transform m_InvenScrollTr = null;
    bool m_Inven_ScOnOff = false;
    float m_ScSpeed = 3800.0f;
    Vector3 m_ScOnPos = new Vector3(0.0f, 0.0f, 0.0f);
    Vector3 m_ScOffPos = new Vector3(320.0f, 0.0f, 0.0f);

    public Transform m_MkInvenContent = null;
    public GameObject m_MkItemNode = null;
    public Button m_ItemSell_Btn = null;
    //--- Inventory ScrollView




    [HideInInspector] public HeroCtrl m_RefHero = null;

    //--- �̱��� ���� ������ ���� �ڵ�
    public static Game_Mgr Inst;

    private void Awake()
    {
        Inst = this;
    }
    //--- �̱��� ���� ������ ���� �ڵ�

    // Start is called before the first frame update
    void Start()
    {
        Application.targetFrameRate = 60; //���� ������ �ӵ� 60���������� ���� ��Ű��.. �ڵ�
        QualitySettings.vSyncCount = 0;

        Time.timeScale = 1.0f;
        GlobalUserData.LoadGameInfo();
        ReflashUserInfoUI();
        ReflashInGameItemScV();

        m_RefHero = FindObjectOfType<HeroCtrl>();

        m_BulletPrefab = Resources.Load("BulletPrefab") as GameObject;

        if (m_BackBtn != null)
            m_BackBtn.onClick.AddListener(() =>
            {
                SceneManager.LoadScene("LobbyScene");
            });

        #region --- Fixed Joystick ó�� �κ�
        if (m_JoySBackObj != null && m_JoyStickImg != null &&
           m_JoySBackObj.activeSelf == true &&
           m_JoystickPickPanel.activeSelf == false)
        {
            m_JoyStickType = JoyStickType.Fixed;

            Vector3[] v = new Vector3[4];
            m_JoySBackObj.GetComponent<RectTransform>().GetWorldCorners(v);
            //v[0] : �����ϴ�  v[1] : �������   v[2] : �������   v[3] : �����ϴ�
            //v[0] �����ϴ��� 0, 0 ��ǥ�� ��ũ�� ��ǥ(Screen.width, Screen.height)��
            //�������� 
            m_Radius = v[2].y - v[0].y;
            m_Radius = m_Radius / 3.0f;

            m_OriginPos = m_JoyStickImg.transform.position;

            //using UnityEngine.EventSystems;
            EventTrigger trigger = m_JoySBackObj.GetComponent<EventTrigger>();
            //m_JoySBackObj �� AddComponent --> EventTrigger �� �߰� �Ǿ� �־�� �Ѵ�.
            EventTrigger.Entry entry = new EventTrigger.Entry();
            entry.eventID = EventTriggerType.Drag;
            entry.callback.AddListener((data) =>
            {
                OnDragJoyStick((PointerEventData)data);
            });
            trigger.triggers.Add(entry);

            entry = new EventTrigger.Entry();
            entry.eventID = EventTriggerType.EndDrag;
            entry.callback.AddListener((data) =>
            {
                OnEndDragJoyStick((PointerEventData)data);
            });
            trigger.triggers.Add(entry);
        }
        #endregion

        #region --- Flexible Joystick ó�� �κ�

        if (m_JoystickPickPanel != null && m_JoySBackObj != null &&
            m_JoyStickImg != null &&
            m_JoystickPickPanel.activeSelf == true)
        {
            if (m_JoySBackObj.activeSelf == true)
                m_JoyStickType = JoyStickType.Flexible;
            else
                m_JoyStickType = JoyStickType.FlexibleOnOff;

            Vector3[] v = new Vector3[4];
            m_JoySBackObj.GetComponent<RectTransform>().GetWorldCorners(v);
            m_Radius = v[2].y - v[0].y;
            m_Radius = m_Radius / 3.0f;

            m_OriginPos = m_JoyStickImg.transform.position;
            m_JoySBackObj.GetComponent<Image>().raycastTarget = false;
            m_JoyStickImg.raycastTarget = false;

            EventTrigger trigger = m_JoystickPickPanel.GetComponent<EventTrigger>();
            EventTrigger.Entry entry = new EventTrigger.Entry();
            entry.eventID = EventTriggerType.PointerDown;
            entry.callback.AddListener((data) =>
            {
                OnPointerDown_Flx((PointerEventData)data);
            });
            trigger.triggers.Add(entry);

            entry = new EventTrigger.Entry();
            entry.eventID = EventTriggerType.PointerUp;
            entry.callback.AddListener((data) =>
            {
                OnPointerUp_Flx((PointerEventData)data);
            });
            trigger.triggers.Add(entry);

            entry = new EventTrigger.Entry();
            entry.eventID = EventTriggerType.Drag;
            entry.callback.AddListener((data) =>
            {
                OnDragJoyStick_Flx((PointerEventData)data);
            });
            trigger.triggers.Add(entry);

        }//if(m_JoystickPickPanel != null && m_JoySBackObj != null &&

        #endregion

        #region --- UserInfo UI ��ư �̺�Ʈ ó��
        //## UserInfo �ǳ� onoff ó��
        m_UInfoIsOn  = m_UserInfo_Panel.activeSelf;
        
        if(m_UserInfo_Btn != null)
            m_UserInfo_Btn.onClick.AddListener(() =>
            {
                m_UInfoIsOn = !m_UInfoIsOn;
                if(m_UserInfo_Panel != null)
                m_UserInfo_Panel.SetActive(m_UInfoIsOn);
            });



        #endregion



        #region --- �κ��丮 �ǳ� �̺�Ʈ ó��
        //--- �κ��丮 �ǳ� OnOff
        if (m_Inven_Btn != null)
            m_Inven_Btn.onClick.AddListener(() =>
            {
                m_Inven_ScOnOff = !m_Inven_ScOnOff;
                if (m_ItemSell_Btn != null)
                    m_ItemSell_Btn.gameObject.SetActive(m_Inven_ScOnOff);
            });

        if (m_ItemSell_Btn != null)
            m_ItemSell_Btn.onClick.AddListener(ItemSelMethod);
        //--- �κ��丮 �ǳ� OnOff
        #endregion




    }// void Start()

    // Update is called once per frame
    void Update()
    {
        if(m_UserHPText != null && m_RefHero != null)
            m_UserHPText.text = "HP : " + m_RefHero.m_CurHp + " / " + m_RefHero.m_MaxHp;


        InvenScOnOffUpdate();
    }

    #region --- Fixed Joystick ó�� �κ�

    private void OnDragJoyStick(PointerEventData a_data)
    {
        //(Vector3)a_data.position : ���콺 ��ǥ
        if (m_JoyStickImg == null)
            return;

        m_JsCacVec = (Vector3)a_data.position - m_OriginPos;
        m_JsCacVec.z = 0.0f;
        m_JsCacDist = m_JsCacVec.magnitude;
        m_Axis = m_JsCacVec.normalized;

        //���̽�ƽ ��׶��带 ����� ���ϰ� ���� �κ�
        if (m_Radius < m_JsCacDist)
        {
            m_JoyStickImg.transform.position =
                            m_OriginPos + m_Axis * m_Radius;
        }
        else
        {
            m_JoyStickImg.transform.position =
                            m_OriginPos + m_Axis * m_JsCacDist;
        }

        if (1.0f < m_JsCacDist)
            m_JsCacDist = 1.0f;

        //ĳ���� �̵�ó��
        if (m_RefHero != null)
            m_RefHero.SetJoyStickMv(m_JsCacDist, m_Axis);
    }

    private void OnEndDragJoyStick(PointerEventData data)
    {
        if (m_JoyStickImg == null)
            return;

        m_Axis = Vector3.zero;
        m_JoyStickImg.transform.position = m_OriginPos;
        m_JsCacDist = 0.0f;

        //ĳ���� �̵�����
        if (m_RefHero != null)
            m_RefHero.SetJoyStickMv(0.0f, Vector3.zero);
    }

    #endregion


    #region --- Flexible  Joystick ó���κ�
    private void OnPointerDown_Flx(PointerEventData data) //���콺 Ŭ����
    {
        if (data.button != PointerEventData.InputButton.Left)
            return;  //�����Ϳ��� ���콺 ���� ��ư Ŭ���� �ƴϸ� ����

        if (m_JoySBackObj == null)
            return;

        if (m_JoyStickImg == null)
            return;

        m_JoySBackObj.transform.position = data.position;
        m_JoyStickImg.transform.position = data.position;

        m_JoySBackObj.SetActive(true);
    }

    private void OnPointerUp_Flx(PointerEventData data) //���콺 Ŭ�� ������
    {
        if (data.button != PointerEventData.InputButton.Left)
            return;

        if (m_JoySBackObj == null)
            return;

        if (m_JoyStickImg == null)
            return;

        m_JoySBackObj.transform.position = m_OriginPos;
        m_JoyStickImg.transform.position = m_OriginPos;

        if (m_JoyStickType == JoyStickType.FlexibleOnOff)
        {
            m_JoySBackObj.SetActive(false); //<-- ���� ���·� �����ϴ� ����� ���� ��Ȱ��ȭ �ʿ�
        }

        m_Axis = Vector3.zero;
        m_JsCacDist = 0.0f;

        //ĳ���� ���� ó��
        if (m_RefHero != null)
            m_RefHero.SetJoyStickMv(0.0f, Vector3.zero);
    }

    private void OnDragJoyStick_Flx(PointerEventData data)
    {
        if (data.button != PointerEventData.InputButton.Left)
            return;

        if (m_JoyStickImg == null)
            return;

        posJoyBack = m_JoySBackObj.transform.position;
        //���̽�ƽ �� �׶��� ���� ��ġ ����
        m_JsCacVec = data.position - (Vector2)posJoyBack;
        m_JsCacVec.z = 0.0f;
        m_JsCacDist = m_JsCacVec.magnitude; //�Ÿ�
        m_Axis = m_JsCacVec.normalized;   //����

        //���̽�ƽ ��׶��带 ����� ���ϰ� ���� �κ�
        if (m_Radius < m_JsCacDist)
        {
            m_JsCacDist = m_Radius;
            m_JoyStickImg.transform.position =
                                    posJoyBack + m_Axis * m_Radius;
        }
        else
        {
            m_JoyStickImg.transform.position = data.position;
        }

        if (1.0f < m_JsCacDist)
            m_JsCacDist = 1.0f;

        //ĳ���� �̵�ó��
        if (m_RefHero != null)
            m_RefHero.SetJoyStickMv(m_JsCacDist, m_Axis);

    }
    #endregion

    public void DamageText(int a_Value, Vector3 a_OwnerPos)
    {
        GameObject a_DmgClone = Instantiate(m_DamageTxtRoot);
        if (a_DmgClone != null && m_Damage_Canvas != null)
        {
            Vector3 a_StCacPos = new Vector3(a_OwnerPos.x, 0.8f, a_OwnerPos.z + 4.0f);
            a_DmgClone.transform.SetParent(m_Damage_Canvas);
            DamageText a_DamageTx = a_DmgClone.GetComponent<DamageText>();
            a_DamageTx.DamageTxtSpawn(a_Value, new Color32(200, 0, 0, 255));
            a_DmgClone.transform.position = a_StCacPos;
        }
    }//public void DamageText(int a_Value, Vector3 a_OwnerPos)

    public static bool IsPointerOverUIObject() //UGUI�� UI���� ���� ��ŷ�Ǵ��� Ȯ���ϴ� �Լ�
    {
        PointerEventData a_EDCurPos = new PointerEventData(EventSystem.current);

#if !UNITY_EDITOR && (UNITY_IPHONE || UNITY_ANDROID)

			List<RaycastResult> results = new List<RaycastResult>();
			for (int i = 0; i < Input.touchCount; ++i)
			{
				a_EDCurPos.position = Input.GetTouch(i).position;  
				results.Clear();
				EventSystem.current.RaycastAll(a_EDCurPos, results);
                if (0 < results.Count)
                    return true;
			}

			return false;
#else
        a_EDCurPos.position = Input.mousePosition;
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(a_EDCurPos, results);
        return (0 < results.Count);
#endif
    }//public bool IsPointerOverUIObject() 

    void InvenScOnOffUpdate()
    {   //�κ��丮 �ǳ� OnOff ���� �Լ�

        if (m_InvenScrollTr == null)
            return;

        if (m_Inven_ScOnOff == false)
        {
            if (m_InvenScrollTr.localPosition.x < m_ScOffPos.x)
            {
                m_InvenScrollTr.localPosition =
                        Vector3.MoveTowards(m_InvenScrollTr.localPosition,
                                         m_ScOffPos, m_ScSpeed * Time.deltaTime);
            }
        }
        else //if(m_Inven_ScOnOff == true)
        {
            if (m_ScOnPos.x < m_InvenScrollTr.localPosition.x)
            {
                m_InvenScrollTr.localPosition =
                        Vector3.MoveTowards(m_InvenScrollTr.localPosition,
                                        m_ScOnPos, m_ScSpeed * Time.deltaTime);
            }

        }//else //if(m_Inven_ScOnOff == true)
    }//void InvenScOnOffUpdate()

    public void InvenAddItem(GameObject a_Obj)
    {
        ItemObjInfo a_RefItemInfo = a_Obj.GetComponent<ItemObjInfo>();
        if (a_RefItemInfo != null)
        {
            ItemValue a_Node = new ItemValue();
            a_Node.UniqueID    = a_RefItemInfo.m_ItemValue.UniqueID;
            a_Node.m_Item_Type = a_RefItemInfo.m_ItemValue.m_Item_Type;
            a_Node.m_ItemName  = a_RefItemInfo.m_ItemValue.m_ItemName;
            a_Node.m_ItemLevel = a_RefItemInfo.m_ItemValue.m_ItemLevel;
            a_Node.m_ItemStar  = a_RefItemInfo.m_ItemValue.m_ItemStar;
            GlobalUserData.g_ItemList.Add(a_Node);

            AddNodeScrollView(a_Node);  //��ũ�� �信 �߰�
            GlobalUserData.ReflashItemSave(); //���� ����
        }
    }

    void AddNodeScrollView(ItemValue a_Node)
    {
        GameObject a_ItemObj = Instantiate(m_MkItemNode);
        a_ItemObj.transform.SetParent(m_MkInvenContent, false);
        // false �� ��� : ���� ������ ������ ������ ä ���ϵ�ȭ �ȴ�.

        ItemNode a_MyItemInfo = a_ItemObj.GetComponent<ItemNode>();
        if (a_MyItemInfo != null)
            a_MyItemInfo.SetItemRsc(a_Node);
    }

    private void ReflashInGameItemScV()  //<--- InGame ScrollView ����
    { //GlobalUserData.g_ItemList ����� ���� scrollView �� ������ �ִ� �Լ�
        ItemNode[] a_MyNodeList =
                    m_MkInvenContent.GetComponentsInChildren<ItemNode>(true);
        for (int i = 0; i < a_MyNodeList.Length; i++)
        {
            Destroy(a_MyNodeList[i].gameObject);
        }

        for (int i = 0; i < GlobalUserData.g_ItemList.Count; i++)
        {
            AddNodeScrollView(GlobalUserData.g_ItemList[i]);
        }
    }//private void ReflashInGameItemScV()  //<--- InGame ScrollView ����

    private void ItemSelMethod()
    {
        //������ �ϳ��� 100������ �Ǹ�

        //��ũ�Ѻ��� ��带 ��� ���鼭 ���õǾ� �ִ� �͵鸸 �Ǹ��ϰ� 
        //�ش� ����ũ ID�� g_ItemList���� ã�Ƽ� ������ �ش�.
        ItemNode[] a_MyNodeList =
                    m_MkInvenContent.GetComponentsInChildren<ItemNode>(true);
        //true : Active�� ���� �ִ� ������Ʈ���� ��� �������� �ɼ�
        for (int i = 0; i < a_MyNodeList.Length; i++)
        {
            if (a_MyNodeList[i].m_SelOnOff == false)
                continue;

            //--- �۷ι� ����Ʈ���� �Ǹ��Ϸ��� �������� ������ȣ�� ã�Ƽ� ����Ʈ������ ������ ��� �Ѵ�.
            for (int b = 0; b < GlobalUserData.g_ItemList.Count; b++)
            {
                if (a_MyNodeList[i].m_UniqueID == GlobalUserData.g_ItemList[b].UniqueID)
                {
                    GlobalUserData.g_ItemList.RemoveAt(b);
                    break;
                }
            }//for(int b = 0; b < GlobalUserData.g_ItemList.Count; b++)
            //--- �۷ι� ����Ʈ���� �Ǹ��Ϸ��� �������� ������ȣ�� ã�Ƽ� ����Ʈ������ ������ ��� �Ѵ�.

            Destroy(a_MyNodeList[i].gameObject);
            AddGold(100);  //��� ����

        }//for(int i = 0; i < a_MyNodeList.Length; i++)

        GlobalUserData.ReflashItemSave();   //����Ʈ �ٽ� ����

    }//private void ItemSelMethod()


    public void AddMonKill(int a_killCn = 1)
    {
        m_MonKillCnt += a_killCn;
        m_MonKillText.text = " X  " + m_MonKillCnt.ToString();
    }

    public void AddGold(int a_Gold = 5)
    {
       m_CurGold += a_Gold;// �̹� ������������ ���� ��尪

        //## �ִ�ġ ����

        if (GlobalUserData.g_UserGold <= int.MaxValue - a_Gold)
            GlobalUserData.g_UserGold +=  a_Gold;
        else
        GlobalUserData.g_UserGold = int.MaxValue; //��ü ��尪0

     
        m_GoldText.text = " X  " + GlobalUserData.g_UserGold.ToString("N0");
        //��ȭǥ��ó�� 1,000�̷��� ǥ�����ְ� NO�� ǥ��

        PlayerPrefs.SetInt("GoldCount", GlobalUserData.g_UserGold);
    
    }


    public void AddBoomSkill(int a_Skill = 1)
    {
        GlobalUserData.g_BombCount += a_Skill;
        if(GlobalUserData.g_BombCount <= 0)
            m_SkillText.text = "X 00";
        else
            m_SkillText.text = " X " + GlobalUserData.g_BombCount.ToString();
        m_SkillText.text = " X  " + GlobalUserData.g_BombCount.ToString();
        PlayerPrefs.SetInt("BombCount", GlobalUserData.g_BombCount);
    }



    public void ReflashUserInfoUI() 
    {
        if(m_GoldText != null)
        {
            if(GlobalUserData.g_UserGold <= 0)
               m_GoldText.text = "X 00";
            else
                m_GoldText.text = " X " + GlobalUserData.g_UserGold.ToString("N0");
        }

        if(m_SkillText != null)
        {
            if(GlobalUserData.g_BombCount <= 0)
                m_SkillText.text = "X 00";
            else
                m_SkillText.text = " X " + GlobalUserData.g_BombCount.ToString();
        }




    }


}//public class Game_Mgr : MonoBehaviour