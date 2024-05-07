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

//Terrain, NaviMesh <-- X, Z 평면에서 제공되는 유니티 기능

public class Game_Mgr : MonoBehaviour
{
    public static GameObject m_BulletPrefab = null;

    public Button m_BackBtn;

    //--- UserInfo UI 관련 변수
    [Header("--- UserInfo UI ---")]
    [SerializeField] private Button m_UserInfo_Btn = null;
    [SerializeField] private GameObject m_UserInfo_Panel = null;
    [SerializeField] private Text m_GoldTxt = null;
    [SerializeField] private Text m_SkillTxt = null;
    [SerializeField] private Text m_Mon_Txt = null;
    [SerializeField] private Text HP_Txt = null;

    //--- UserInfo UI 관련 변수

    //--- Fixed JoyStick 처리 부분
    JoyStickType m_JoyStickType = JoyStickType.Fixed;

    [Header("--- JoyStick ---")]
    public GameObject m_JoySBackObj = null;
    public Image m_JoyStickImg = null;
    float m_Radius = 0.0f;
    Vector3 m_OriginPos = Vector3.zero;
    Vector3 m_Axis = Vector3.zero;
    Vector3 m_JsCacVec = Vector3.zero;
    float m_JsCacDist = 0.0f;
    //--- Fixed JoyStick 처리 부분

    //--- Flexible JoyStick 처리 부분
    public GameObject m_JoystickPickPanel = null;
    Vector3 posJoyBack;
    Vector3 dirStick;
    //--- Flexible JoyStick 처리 부분

    //--- 머리위에 데미지 띄우기용 변수 선언
    Vector3 m_StCacPos = Vector3.zero;
    [Header("--- Damage Text ---")]
    public Transform m_Damage_Canvas = null;
    public GameObject m_DamageTxtRoot = null;
    //--- 머리위에 데미지 띄우기용 변수 선언

    //--- Inventory ScrollView
    [Header("--- Inventory ScrollView OnOff ---")]
    public Button m_Inven_Btn = null;
    public Transform m_InvenScrollTr = null;
    bool  m_Inven_ScOnOff = false;
    float m_ScSpeed = 3800.0f;
    Vector3 m_ScOnPos = new Vector3(0.0f, 0.0f, 0.0f);
    Vector3 m_ScOffPos = new Vector3(320.0f, 0.0f, 0.0f);

    public Transform  m_MkInvenContent = null;
    public GameObject m_MkItemNode = null;
    public Button m_ItemSell_Btn = null;
    //--- Inventory ScrollView

    [HideInInspector] public HeroCtrl m_RefHero = null;

    //--- 싱글턴 패턴 접근을 위한 코드
    public static Game_Mgr Inst;

    private void Awake()
    {
        Inst = this;    
    }
    //--- 싱글턴 패턴 접근을 위한 코드

    // Start is called before the first frame update
    void Start()
    {
        Application.targetFrameRate = 60; //실행 프레임 속도 60프레임으로 고정 시키기.. 코드
        QualitySettings.vSyncCount = 0;

        Time.timeScale = 1.0f;

        GlobalUserData.LoadGameInfo();
        ReflashInGameItemScV();

        m_RefHero = FindObjectOfType<HeroCtrl>();

        m_BulletPrefab = Resources.Load("BulletPrefab") as GameObject;

        if (m_BackBtn != null)
            m_BackBtn.onClick.AddListener(() =>
            {
                SceneManager.LoadScene("LobbyScene");
            });

#region --- Fixed Joystick 처리 부분
        if(m_JoySBackObj != null && m_JoyStickImg != null &&
           m_JoySBackObj.activeSelf == true &&
           m_JoystickPickPanel.activeSelf == false)
        {
            m_JoyStickType = JoyStickType.Fixed;

            Vector3[] v = new Vector3[4];
            m_JoySBackObj.GetComponent<RectTransform>().GetWorldCorners(v);
            //v[0] : 좌측하단  v[1] : 좌측상단   v[2] : 우측상단   v[3] : 우측하단
            //v[0] 좌측하단이 0, 0 좌표인 스크린 좌표(Screen.width, Screen.height)를
            //기준으로 
            m_Radius = v[2].y - v[0].y;
            m_Radius = m_Radius / 3.0f;

            m_OriginPos = m_JoyStickImg.transform.position;

            //using UnityEngine.EventSystems;
            EventTrigger trigger = m_JoySBackObj.GetComponent<EventTrigger>();
            //m_JoySBackObj 에 AddComponent --> EventTrigger 가 추가 되어 있어야 한다.
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

#region --- Flexible Joystick 처리 부분

        if(m_JoystickPickPanel != null && m_JoySBackObj != null &&
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
            EventTrigger.Entry  entry = new EventTrigger.Entry();
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

        //--- 인벤토리 판넬 OnOff
        if (m_Inven_Btn != null)
            m_Inven_Btn.onClick.AddListener(() =>
            {
                m_Inven_ScOnOff = !m_Inven_ScOnOff;
                if (m_ItemSell_Btn != null)
                    m_ItemSell_Btn.gameObject.SetActive(m_Inven_ScOnOff);
            });

        if (m_ItemSell_Btn != null)
            m_ItemSell_Btn.onClick.AddListener(ItemSelMethod);
        //--- 인벤토리 판넬 OnOff

        if (m_UserInfo_Btn != null)
        {
            m_UserInfo_Btn.onClick.AddListener(() =>
            {
                if (m_UserInfo_Panel != null)
                {
                    m_UserInfo_Panel.SetActive(!m_UserInfo_Panel.activeSelf);
                    // Update monster count text when the panel is active
                    if (m_UserInfo_Panel.activeSelf)
                    {
                        m_Mon_Txt.text = " x " + GlobalUserData.g_MonsterKillCount.ToString();
                        m_GoldTxt.text = " x "+GlobalUserData.g_UserGold.ToString("N0");
                        m_SkillTxt.text ="x" +GlobalUserData.g_BombCount.ToString();
                        HP_Txt.text= " 200 / " + HeroCtrl.m_CurHp.ToString();

                    }
                }
            });
        }



    }// void Start()

    // Update is called once per frame
    void Update()
    {
        InvenScOnOffUpdate();
    }

#region --- Fixed Joystick 처리 부분

    private void OnDragJoyStick(PointerEventData a_data)
    {
        //(Vector3)a_data.position : 마우스 좌표
        if (m_JoyStickImg == null)
            return;

        m_JsCacVec = (Vector3)a_data.position - m_OriginPos;
        m_JsCacVec.z = 0.0f;
        m_JsCacDist = m_JsCacVec.magnitude;
        m_Axis = m_JsCacVec.normalized;

        //조이스틱 백그라운드를 벗어나지 못하게 막는 부분
        if(m_Radius < m_JsCacDist)
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

        //캐릭터 이동처리
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

        //캐릭터 이동정지
        if (m_RefHero != null)
            m_RefHero.SetJoyStickMv(0.0f, Vector3.zero);
    }

    #endregion


#region --- Flexible  Joystick 처리부분
    private void OnPointerDown_Flx(PointerEventData data) //마우스 클릭시
    {
        if (data.button != PointerEventData.InputButton.Left)
            return;  //에디터에서 마우스 왼쪽 버튼 클릭이 아니면 리턴

        if(m_JoySBackObj == null)
            return;

        if (m_JoyStickImg == null)
            return;

        m_JoySBackObj.transform.position = data.position;
        m_JoyStickImg.transform.position = data.position;

        m_JoySBackObj.SetActive(true);
    }

    private void OnPointerUp_Flx(PointerEventData data) //마우스 클릭 해제시
    {
        if (data.button != PointerEventData.InputButton.Left)
            return;

        if(m_JoySBackObj == null)
            return;

        if(m_JoyStickImg == null)
            return;

        m_JoySBackObj.transform.position = m_OriginPos;
        m_JoyStickImg.transform.position = m_OriginPos;

        if(m_JoyStickType == JoyStickType.FlexibleOnOff)
        {
            m_JoySBackObj.SetActive(false); //<-- 꺼진 상태로 시작하는 방식일 때는 비활성화 필요
        }

        m_Axis = Vector3.zero;
        m_JsCacDist = 0.0f;

        //캐릭터 정지 처리
        if (m_RefHero != null)
            m_RefHero.SetJoyStickMv(0.0f, Vector3.zero);
    }

    private void OnDragJoyStick_Flx(PointerEventData data)
    {
        if(data.button != PointerEventData.InputButton.Left)
            return;

        if(m_JoyStickImg == null)
            return;

        posJoyBack = m_JoySBackObj.transform.position;
        //조이스틱 백 그라운드 현재 위치 기준
        m_JsCacVec = data.position - (Vector2)posJoyBack;
        m_JsCacVec.z = 0.0f;
        m_JsCacDist = m_JsCacVec.magnitude; //거리
        m_Axis = m_JsCacVec.normalized;   //방향

        //조이스틱 백그라운드를 벗어나지 못하게 막는 부분
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

        //캐릭터 이동처리
        if (m_RefHero != null)
            m_RefHero.SetJoyStickMv(m_JsCacDist, m_Axis);

    }
#endregion

    public void DamageText(int a_Value, Vector3 a_OwnerPos)
    {
        GameObject a_DmgClone = Instantiate(m_DamageTxtRoot);
        if(a_DmgClone != null && m_Damage_Canvas != null)
        {
            Vector3 a_StCacPos = new Vector3(a_OwnerPos.x, 0.8f, a_OwnerPos.z + 4.0f);
            a_DmgClone.transform.SetParent(m_Damage_Canvas);
            DamageText a_DamageTx = a_DmgClone.GetComponent<DamageText>();
            a_DamageTx.DamageTxtSpawn(a_Value, new Color32(200, 0, 0, 255));
            a_DmgClone.transform.position = a_StCacPos;
        }
    }//public void DamageText(int a_Value, Vector3 a_OwnerPos)

    public static bool IsPointerOverUIObject() //UGUI의 UI들이 먼저 피킹되는지 확인하는 함수
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

    //## 인벤토리 판넬 OnOff 연출 함수
    void InvenScOnOffUpdate()
    {  

        if (m_InvenScrollTr == null)
            return;

        if(m_Inven_ScOnOff == false)
        {
            if(m_InvenScrollTr.localPosition.x < m_ScOffPos.x)
            {
                m_InvenScrollTr.localPosition =
                        Vector3.MoveTowards(m_InvenScrollTr.localPosition,
                                         m_ScOffPos, m_ScSpeed * Time.deltaTime);
            }
        }
        else //if(m_Inven_ScOnOff == true)
        {
            if (m_ScOnPos.x < m_InvenScrollTr.localPosition.x )
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
        if(a_RefItemInfo != null)
        {
            ItemValue a_Node = new ItemValue();
            a_Node.UniqueID    = a_RefItemInfo.m_ItemValue.UniqueID;
            a_Node.m_Item_Type = a_RefItemInfo.m_ItemValue.m_Item_Type;
            a_Node.m_ItemName  = a_RefItemInfo.m_ItemValue.m_ItemName;
            a_Node.m_ItemLevel = a_RefItemInfo.m_ItemValue.m_ItemLevel;
            a_Node.m_ItemStar  = a_RefItemInfo.m_ItemValue.m_ItemStar;
            GlobalUserData.g_ItemList.Add(a_Node);

            AddNodeScrollView(a_Node);  //스크롤 뷰에 추가
            GlobalUserData.ReflashItemSave(); //파일 저장
        }
    }

    void AddNodeScrollView(ItemValue a_Node)
    {
        GameObject a_ItemObj = Instantiate(m_MkItemNode);
        a_ItemObj.transform.SetParent(m_MkInvenContent, false);
        // false 일 경우 : 로컬 기준의 정보를 유지한 채 차일드화 된다.

        ItemNode a_MyItemInfo = a_ItemObj.GetComponent<ItemNode>();
        if (a_MyItemInfo != null)
            a_MyItemInfo.SetItemRsc(a_Node);
    }

    private void ReflashInGameItemScV()  //<--- InGame ScrollView 갱신
    { //GlobalUserData.g_ItemList 저장된 값을 scrollView 에 복원해 주는 함수
        ItemNode[] a_MyNodeList = 
                    m_MkInvenContent.GetComponentsInChildren<ItemNode>(true);
        for(int i = 0; i < a_MyNodeList.Length; i++)
        {
            Destroy(a_MyNodeList[i].gameObject);
        }

        for(int i = 0; i < GlobalUserData.g_ItemList.Count; i++)
        {
            AddNodeScrollView(GlobalUserData.g_ItemList[i]);    
        }
    }//private void ReflashInGameItemScV()  //<--- InGame ScrollView 갱신

    private void ItemSelMethod()
    {
        //아이템 하나당 100원씩에 판매

        //스크롤뷰의 노드를 모두 돌면서 선택되어 있는 것들만 판매하고 
        //해당 유니크 ID를 g_ItemList에서 찾아서 제거해 준다.
        ItemNode[] a_MyNodeList =
                    m_MkInvenContent.GetComponentsInChildren<ItemNode>(true);
        //true : Active가 꺼져 있는 오브젝트까지 모두 가져오는 옵션
        for(int i = 0; i < a_MyNodeList.Length; i++)
        {
            if (a_MyNodeList[i].m_SelOnOff == false)
                continue;

            //--- 글로벌 리스트에서 판매하려는 아이템의 고유번호를 찾아서 리스트에서도 제거해 줘야 한다.
            for (int b = 0; b < GlobalUserData.g_ItemList.Count; b++)
            {
                if (a_MyNodeList[i].m_UniqueID == GlobalUserData.g_ItemList[b].UniqueID)
                {
                    GlobalUserData.g_ItemList.RemoveAt(b);
                    break;
                }
            }//for(int b = 0; b < GlobalUserData.g_ItemList.Count; b++)
            //--- 글로벌 리스트에서 판매하려는 아이템의 고유번호를 찾아서 리스트에서도 제거해 줘야 한다.

            Destroy(a_MyNodeList[i].gameObject);
            //AddGold(100);  //골드 증가

        }//for(int i = 0; i < a_MyNodeList.Length; i++)

        GlobalUserData.ReflashItemSave();   //리스트 다시 저장

    }//private void ItemSelMethod()

}//public class Game_Mgr : MonoBehaviour