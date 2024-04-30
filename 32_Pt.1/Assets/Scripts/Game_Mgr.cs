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

public class Game_Mgr : MonoBehaviour
{
    public static GameObject m_BulletPrefab = null;

    public Button m_BackBtn;

    //--- UserInfo UI 관련 변수
    [Header("--- UserInfo UI ---")]
    [SerializeField] private Button m_UserInfo_Btn = null;
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
    
    //## 머리위 데미지 띄우기 변수
    Vector3 m_StCacPos = Vector3.zero;
    [Header("--- DamageText ---")]
    public Transform m_Damage_Canvas = null;
    public GameObject m_DamageTxtRoot = null;


    [HideInInspector] public HeroCtrl m_RefHero = null;

    //## 싱글톤 패턴 접근코드

    public static Game_Mgr Inst;

    private void Awake()
    {
        Inst = this;
    }

    


    // Start is called before the first frame update
    void Start()
    {

        Application.targetFrameRate = 60;
        QualitySettings.vSyncCount = 0;

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

    }

    // Update is called once per frame
    void Update()
    {
        
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

    #region 데미지 텍스트 처리
    public void DamageText(int a_Value, Vector3 a_OwnerPos)
    {
        GameObject a_DmgClone = Instantiate(m_DamageTxtRoot, m_Damage_Canvas);
        if(a_DmgClone != null && m_Damage_Canvas != null)
        {
           Vector3 a_StCacPos = new Vector3(a_OwnerPos.x, 0.8f, a_OwnerPos.z + 4.0f);
           a_DmgClone.transform.SetParent(m_Damage_Canvas);
            DamageText a_DamageTx = a_DmgClone .GetComponent<DamageText>();
            a_DamageTx.DamageTextSpawn(a_Value, new Color32(200, 0, 0, 255));
            a_DmgClone.transform.position = a_StCacPos;


        }

    }
    #endregion

    #region ## UGUI의 UI들이 먼저 피킹되는지 확인하는 함수
    public static bool IsPointerOverUIObject() 
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
    #endregion

}//public class Game_Mgr : MonoBehaviour
