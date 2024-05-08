using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Store_Mgr : MonoBehaviour
{
    public Button BackBtn;

    //## Leftgroup List 관리 변수
    int g_UniqueUD = 0;

    [Header("LeftGroup UserList")]
    public ScrollRect m_LF_ScrollView;
    //scrollview 컴포넌트가 붙어있는 오브젝트
    public GameObject m_LF_SvContent;
    //scrollContent 차일드로 생성될 부모객체
    public GameObject m_LF_NodePrefab = null;
    //nodePrefab


    public Button m_LF_AddNodeBtn = null;
    public Button m_LF_SelDelBtn = null;
    public Button m_LF_MoveNodeBtn = null;
    public InputField m_LF_InputField = null;

    [HideInInspector] public LF_UserNode[] m_LF_UserNdLiad;
    //content 하위에 생성된 노드들을 관리할 배열




    //## RightGroup List 관리 변수
    int a_Item_UniqueID = 0;
    [Header("RightGroup ItemList")]
    public ScrollRect m_RT_ScrollView;
    //scrollview 컴포넌트가 붙어있는 오브젝트
    public GameObject m_RT_SvContent;
    //scrollContent 차일드로 생성될 부모객체
    public GameObject m_RT_NodePrefab = null;
    //nodePrefab

    public Button m_RT_AddNodeBtn = null;
    //노드 추가 버튼
    public Button m_RT_SelDelBtn = null;
    //선택된 노드 삭제 버튼
    public Button m_RT_MoveNodeBtn = null;
    //노드 이동 버튼

    public InputField m_RT_InputField = null;

    [HideInInspector] public RT_ItemNode[] m_RT_ItemNdList;
    //content 하위에 생성된 노드들을 관리할 배열




    // Start is called before the first frame update
    void Start()
    {
        if (BackBtn != null)
            BackBtn.onClick.AddListener(BackBtnClick);

        //## LeftGroup List 초기화
        if (m_LF_AddNodeBtn != null)
            m_LF_AddNodeBtn.onClick.AddListener(LF_AddNodeClick);
        if (m_LF_SelDelBtn != null)
            m_LF_SelDelBtn.onClick.AddListener(LF_SelDelClick);
        if (m_LF_MoveNodeBtn != null)
            m_LF_MoveNodeBtn.onClick.AddListener(LF_MoveNodeClick);

        //## RightGroup List 초기화
        if (m_RT_AddNodeBtn != null)
            m_RT_AddNodeBtn.onClick.AddListener(RT_AddNodeClick);
        if (m_RT_SelDelBtn != null)
            m_RT_SelDelBtn.onClick.AddListener(RT_SelDelClick);
        if (m_RT_MoveNodeBtn != null)
            m_RT_MoveNodeBtn.onClick.AddListener(RT_MoveNodeClick);



    }


    // Update is called once per frame
    //void Update()
    //{

    //}

    void BackBtnClick()
    {
        SceneManager.LoadScene("LobbyScene");
    }

    void LF_AddNodeClick()
    {
        if (m_LF_NodePrefab == null)
            return;

        GameObject a_UserObj = Instantiate(m_LF_NodePrefab);
        a_UserObj.transform.SetParent(m_LF_SvContent.transform, false);

        LF_UserNode a_SvNode = a_UserObj.GetComponent<LF_UserNode>();
        string a_UName = "User" + g_UniqueUD.ToString();
        int a_Level = Random.Range(2, 30);
        a_SvNode.InitInfo(g_UniqueUD, a_UName, a_Level);
        g_UniqueUD++;


    }



    void LF_SelDelClick()
    {
        m_LF_UserNdLiad = m_LF_SvContent.transform.GetComponentsInChildren<LF_UserNode>();
        int a_UsCount = m_LF_UserNdLiad.Length;

        for (int i = 0; i < a_UsCount; i++)
        {
            if (m_LF_UserNdLiad[i].m_IsSelected == true)
            {
                Destroy(m_LF_UserNdLiad[i].gameObject);
            }
        }
    }

    //## 유저 고유번호로 노드 이동
    private void LF_MoveNodeClick()
    {
        if (m_LF_InputField == null)
            return;

        string a_GetStr = m_LF_InputField.text.Trim();
        if (string.IsNullOrEmpty(a_GetStr)==true)
            return;


        //유니크 아이디 찾기
        int a_UniqueId = -1;
        if (int.TryParse(a_GetStr, out a_UniqueId) == false)
            return;

        //노드 찾아주기
        m_LF_UserNdLiad =
            m_LF_SvContent.transform.GetComponentsInChildren<LF_UserNode>();

        int a_FindIdx = -1;

        for (int i = 0; i < m_LF_UserNdLiad.Length; i++)
        {
            if (a_UniqueId == m_LF_UserNdLiad[i].m_UniqueUD)
            {
                a_FindIdx =  m_LF_UserNdLiad[i].transform.GetSiblingIndex();
                //child로 붙어있는 순서 인덱스 찾기
                break;
            }
        }

        //## child 개수 찾기
        //m_LF_UserNDList = m_LF_SvContent.transform.GetComponentsInChildren<LF_UserNode>();
        //m_LF_SvContent.transform.childCount; //content에 붙은 자식 개수 찾기
        //mlf_ScrollView.content.childCount; //content에 붙은 자식 개수 찾기


        int a_NodeCount = m_LF_UserNdLiad.Length;
        if (0<= a_FindIdx && a_FindIdx < a_NodeCount)
        {
            if (0 < a_FindIdx)
            {
                a_FindIdx = a_FindIdx +1;

                float nomalizepos = a_FindIdx / (float)a_NodeCount;
                m_LF_ScrollView.verticalNormalizedPosition = 1.0f - nomalizepos;
                //1.0f가 시작 위치 0으로 수렴할수록 endPos .

            }

        }




    }






    //# 과제 아이템 노드 추가/삭제/이동
    //-----강사 코드----


    //## 아이템 노드 추가
    void RT_AddNodeClick()
    {
        if(m_RT_NodePrefab == null)
            return;

        GameObject a_ItemObj = Instantiate(m_RT_NodePrefab);
        a_ItemObj.transform.SetParent(m_RT_SvContent.transform, false);

        RT_ItemNode a_RT_ItemNode = a_ItemObj.GetComponent<RT_ItemNode>();
        int a_ItmeType = Random.Range(0, 6);
        //랜덤 인덱스를 생성합니다.
        string a_IName = "Item" + a_Item_UniqueID.ToString();
        int a_Level = a_Item_UniqueID;
        a_RT_ItemNode.InitInfo(a_Item_UniqueID, (Item_Type)a_ItmeType, a_IName, a_Level);

        a_Item_UniqueID++;
    }


    //## 선택된 아이템 노드 삭제

    void RT_SelDelClick()
    {
        m_RT_ItemNdList = m_RT_SvContent.transform.GetComponentsInChildren<RT_ItemNode>();
        int a_ItCount = m_RT_ItemNdList.Length;
        for(int i = 0; i < a_ItCount; i++)
        {
            if (m_RT_ItemNdList[i].m_IsSelected == true)
            {
                Destroy(m_RT_ItemNdList[i].gameObject);
            }
        }
    }



    //## 아이템 고유번호로 노드 이동
    void RT_MoveNodeClick()
    {
        if (m_RT_InputField == null)
            return;

        string a_GetStr = m_RT_InputField.text.Trim();
        if (string.IsNullOrEmpty(a_GetStr) == true)
            return;

        int a_UniqueId = -1;
        if (int.TryParse(a_GetStr, out a_UniqueId) == false)
            return;

        m_RT_ItemNdList =
            m_RT_SvContent.transform.GetComponentsInChildren<RT_ItemNode>();

        int a_FindIdx = -1;

        for (int i = 0; i < m_RT_ItemNdList.Length; i++)
        {
            if (a_UniqueId == m_RT_ItemNdList[i].m_UniqueID)
            {
                a_FindIdx = m_RT_ItemNdList[i].transform.GetSiblingIndex();

                a_FindIdx = (int)(a_FindIdx /3);

                break;
            }
        }

        int a_NodeCount = 0;

        if (0 < m_RT_ItemNdList.Length)

            a_NodeCount = (int)(m_RT_ItemNdList.Length /3) +1;
       
        //찾은경우
        if (0 < a_FindIdx && a_FindIdx < a_NodeCount) 
        {
            if (0 < a_FindIdx)
            
                a_FindIdx = a_FindIdx +1;
            float normalizePos = a_FindIdx / (float)a_NodeCount;
            m_RT_ScrollView.verticalNormalizedPosition =
                1.0f - normalizePos;  

        
        }
        


       



    }












  


}
