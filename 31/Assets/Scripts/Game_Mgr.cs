using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Unity.VisualScripting;

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

    //## UserInfo UI 관련 변수
    [Header("--- UserInfo UI ---")]
    [SerializeField] private Button m_UserInfo_Btn = null;
    

    //## Fixed JoyStick 변수
    JoyStickType m_JoyStickType = JoyStickType.Fixed;

    [Header("--- JoyStick ---")]
    public GameObject m_JoySBackObj = null;
    public Image m_JoyStickImg = null;
    float m_Radius = 0.0f;
    Vector3 m_OriginPos = Vector3.zero;
    Vector3 m_Axis = Vector3.zero;
    Vector3 m_JsCacVec = Vector3.zero;
    float m_JsCacDist = 0.0f;
  

    //## Flexible JoyStick 변수
    public GameObject m_JoystickPickPanel = null;
    Vector3 posJoyBack;
    Vector3 dirStick;
 

    [HideInInspector] public HeroCtrl m_RefHero = null;

    // Start is called before the first frame update
    void Start()
    {
        m_RefHero = FindObjectOfType<HeroCtrl>();

        m_BulletPrefab = Resources.Load("BulletPrefab") as GameObject;

        if (m_BackBtn != null)
            m_BackBtn.onClick.AddListener(() =>
            {
                SceneManager.LoadScene("LobbyScene");
            });

        #region --- Fixed Joystick 처리 부분
        if (m_JoySBackObj != null && m_JoyStickImg != null &&
           m_JoySBackObj.activeSelf == true &&
           m_JoystickPickPanel.activeSelf == false)
        {
            m_JoyStickType = JoyStickType.Fixed;

            Vector3[] v = new Vector3[4];
            m_JoySBackObj.GetComponent<RectTransform>().GetWorldCorners(v);
            //조이스틱 백그라운드의 4개의 코너 좌표를 가져온다.

            m_Radius = v[2].y - v[0].y;
            m_Radius = m_Radius / 3.0f;
            //3등분해서 반지름을 구한다.
            m_OriginPos = m_JoyStickImg.transform.position;


            EventTrigger trigger = m_JoySBackObj.GetComponent<EventTrigger>();
            //조이스틱 백그라운드에 이벤트 트리거를 추가한다.
            EventTrigger.Entry entry = new EventTrigger.Entry();
            entry.eventID = EventTriggerType.Drag;
            entry.callback.AddListener((data) =>
            {
                OnDragJoyStick((PointerEventData)data);
            });
            //조이스틱 백그라운드를 드래그 했을때 이벤트를 추가한다.
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

        #region ## Flexible Joystick 처리 부분

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

        }

        #endregion

    }

    // Update is called once per frame
    void Update()
    {

    }

    #region ## Fixed Joystick 처리 부분

    private void OnDragJoyStick(PointerEventData a_data)
    {
        //(Vector3)a_data.position = 마우스 좌표
        if (m_JoyStickImg == null)
            return;

        m_JsCacVec = (Vector3)a_data.position - m_OriginPos;
        m_JsCacVec.z = 0.0f;
        m_JsCacDist = m_JsCacVec.magnitude;
        m_Axis = m_JsCacVec.normalized;

        //조이스틱 백그라운드를 벗어나지 못하게 막는 부분
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


    #region ## Flexible  Joystick 처리 
    void OnPointerDown_Flx(PointerEventData data) //마우스 클릭시
    {
        if (data.button != PointerEventData.InputButton.Left)
            return;  // 마우스 왼쪽 버튼 클릭이 아니면 리턴(에디터에서)

        if (m_JoySBackObj == null)
            return;

        if (m_JoyStickImg == null)
            return;

        m_JoySBackObj.transform.position = data.position;
        m_JoyStickImg.transform.position = data.position;

        m_JoySBackObj.SetActive(true);
    }


    void OnPointerUp_Flx(PointerEventData data) //마우스 클릭 해제시
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
            m_JoySBackObj.SetActive(false);
        }

        m_Axis = Vector3.zero;
        m_JsCacDist = 0.0f;

        //## 캐릭터 정지 
        if (m_RefHero != null)
            m_RefHero.SetJoyStickMv(0.0f, Vector3.zero);
    }

    #region EventTriggerType.Drag
    void OnDragJoyStick_Flx(PointerEventData data)
    {
        if (data.button != PointerEventData.InputButton.Left)
            return;

        if (m_JoyStickImg == null)
            return;

        posJoyBack = m_JoySBackObj.transform.position;
        //조이스틱 백 그라운드 현재 위치 기준
        m_JsCacVec = data.position - (Vector2)posJoyBack;
        m_JsCacVec.z = 0.0f;
        m_JsCacDist = m_JsCacVec.magnitude;
        //거리
        m_Axis = m_JsCacVec.normalized;
        //방향

        //조이스틱 백그라운드를 벗어나지 못하게 
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

        //## 캐릭터 이동처리
        if (m_RefHero != null)
            m_RefHero.SetJoyStickMv(m_JsCacDist, m_Axis);

    }
    #endregion

    #region # 마우스 위에 있는지 확인
    public static bool IsPointerOverUIObject()
    //UI 오브젝트 위에 마우스가 있는지 확인 
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
    #endregion
}
