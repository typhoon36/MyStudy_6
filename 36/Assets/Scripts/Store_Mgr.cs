using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Store_Mgr : MonoBehaviour
{
    public Button BackBtn;

    //---------- LeftGroup User List 관리 변수
    int g_UniqueUD = 0;
    [Header("------ LeftGroup UserList ------")]
    public ScrollRect m_LF_ScrollView;          //ScrollRect  컴포넌트가 붙어 있는 게임 오브젝트
    public GameObject m_LF_SvContent;           //ScrollContent 차일드로 생성될 Parent 객체
    public GameObject m_LF_NodePrefab = null;   //Node Prefab

    public Button m_LF_AddNodeBtn  = null;       //노드 추가 버튼
    public Button m_LF_SelDelBtn   = null;       //선택 노드 삭제 버튼
    public Button m_LF_MoveNodeBtn = null;       //찾는 노드로 이동 버튼
    public InputField m_LF_InputField = null;    //찾을 유니크 ID 입력 필드

    [HideInInspector] public LF_UserNode[] m_LF_UserNdLiad;
    //Content 하위의 차일드 목록을 찾기 위한 변수
    //---------- LeftGroup User List 관리 변수

    //---------- RightGroup Item List 관리 변수
    int a_Item_UniqueID = 0;
    [Header("------ RightGroup ItemList ------")]
    public ScrollRect m_RT_ScrollView;          //ScrollRect 컴포넌트가 붙어있는 게임 오브젝트
    public GameObject m_RT_SvContent;           //ScrollContent 차일드로 생성될 Parent 객체
    public GameObject m_RT_NodePrefab = null;   //Node Prefab

    public Button m_RT_AddNodeBtn = null;       //노드 추가 버튼
    public Button m_RT_SelDelBtn = null;        //선택 노드 삭제 버튼
    public Button m_RT_MoveNodeBtn = null;      //찾는 노드로 이동 버튼
    public InputField m_RT_InputField = null;   //찾을 유니크 ID 입력 필드

    [HideInInspector] public RT_ItemNode[] m_RT_ItemNdList;
    //Content 하위의 차일드 목록을 찾기 위한 변수
    //---------- RightGroup Item List 관리 변수

    // Start is called before the first frame update
    void Start()
    {
        if (BackBtn != null)
            BackBtn.onClick.AddListener(BackBtnClick);

        //---- LeftGroup User List 관리 초기화 코드
        if (m_LF_AddNodeBtn != null)
            m_LF_AddNodeBtn.onClick.AddListener(LF_AddNodeClick);

        if (m_LF_SelDelBtn != null)
            m_LF_SelDelBtn.onClick.AddListener(LF_SelDelClick);

        if (m_LF_MoveNodeBtn != null)
            m_LF_MoveNodeBtn.onClick.AddListener(LF_MoveNodeClick);
        //---- LeftGroup User List 관리 초기화 코드

        //---- RightGroup Item List 관리 초기화 코드
        if (m_RT_AddNodeBtn != null)
            m_RT_AddNodeBtn.onClick.AddListener(RT_AddNodeClick);

        if (m_RT_SelDelBtn != null)
            m_RT_SelDelBtn.onClick.AddListener(RT_SelDelClick);

        if (m_RT_MoveNodeBtn != null)
            m_RT_MoveNodeBtn.onClick.AddListener(RT_MoveNodeClick);
        //---- RightGroup Item List 관리 초기화 코드
    }

    // Update is called once per frame
    void Update()
    {

    }


    private void LF_AddNodeClick()
    {
        if (m_LF_NodePrefab == null)
            return;

        GameObject a_UserObj = Instantiate(m_LF_NodePrefab);
        a_UserObj.transform.SetParent(m_LF_SvContent.transform, false);

        LF_UserNode a_SvNode = a_UserObj.GetComponent<LF_UserNode>();
        string a_UName = "User_" + g_UniqueUD.ToString();
        int a_Level = Random.Range(2, 30);
        a_SvNode.InitInfo(g_UniqueUD, a_UName, a_Level);
        g_UniqueUD++;
    }

    private void LF_SelDelClick()
    {
        m_LF_UserNdLiad = m_LF_SvContent.transform.GetComponentsInChildren<LF_UserNode>();

        int a_UsCount = m_LF_UserNdLiad.Length;
        for(int i = 0; i < a_UsCount; i++)
        {
            if (m_LF_UserNdLiad[i].m_SelectOnOff == true)
            {
                Destroy(m_LF_UserNdLiad[i].gameObject);
            }
        }
    }

    private void LF_MoveNodeClick()
    {
        if (m_LF_InputField == null)
            return;

        string a_GetStr = m_LF_InputField.text.Trim();
        if(string.IsNullOrEmpty(a_GetStr) == true)
            return;

        int a_UniqueID = -1;
        if(int.TryParse(a_GetStr, out a_UniqueID) == false)
            return;

        m_LF_UserNdLiad =
            m_LF_SvContent.transform.GetComponentsInChildren<LF_UserNode>();
        int a_FindIdx = -1;
        for(int i = 0; i < m_LF_UserNdLiad.Length; i++)
        {
            if(a_UniqueID == m_LF_UserNdLiad[i].m_UniqueID)
            {
                a_FindIdx = m_LF_UserNdLiad[i].transform.GetSiblingIndex();
                //차일드로 붙어 있는 순서 인덱스를 가져오는 함수
                break;
            }
        }

        // m_LF_UserNdList.Length : Content 에 붙어 있는 차일드 개수 얻어오기
        // m_LF_SvContent.transform.childCount : Content 에 붙어 있는 차일드 개수 얻어오기
        // m_LF_ScrollView.content.transform.childCount : Content 에 붙어 있는 차일드 개수 얻어오기
        int a_NodeCount = m_LF_UserNdLiad.Length;
        if(0 <= a_FindIdx && a_FindIdx < a_NodeCount)
        {
            if (0 < a_FindIdx)
                a_FindIdx = a_FindIdx + 1; //마지막 노드로 이동하려고 할 때 확실히 이동시키려는 코드

            float nomalizePostion = a_FindIdx / (float)a_NodeCount;
            m_LF_ScrollView.verticalNormalizedPosition = 1.0f - nomalizePostion;
            //1.0일 때가 시작 위치,  0.0에 가까워질수록 끝 위치
        }
    }

    private void RT_AddNodeClick()
    {
        if (m_RT_NodePrefab == null)
            return;

        GameObject a_ItemObj = Instantiate(m_RT_NodePrefab);
        a_ItemObj.transform.SetParent(m_RT_SvContent.transform, false);

        RT_ItemNode a_RT_ItemNode = a_ItemObj.GetComponent<RT_ItemNode>();
        int a_ItemType = Random.Range(0, 6); // 0 ~ 5
        string a_IName = "Item_" + a_Item_UniqueID.ToString();
        int a_Level = a_Item_UniqueID;
        a_RT_ItemNode.InitInfo(a_Item_UniqueID, (Item_Type)a_ItemType, a_IName, a_Level);

        a_Item_UniqueID++;
    }

    private void RT_SelDelClick()
    {
        m_RT_ItemNdList = m_RT_SvContent.transform.GetComponentsInChildren<RT_ItemNode>();
        int a_ItCount = m_RT_ItemNdList.Length;
        for(int i = 0; i < a_ItCount; i++)
        {
            if (m_RT_ItemNdList[i].m_SelectOnOff == true)
            {
                Destroy(m_RT_ItemNdList[i].gameObject);
            }
        }
    }//private void RT_SelDelClick()

    private void RT_MoveNodeClick()
    {
        if (m_RT_InputField == null)
            return;

        string a_GetStr = m_RT_InputField.text.Trim();
        if (string.IsNullOrEmpty(a_GetStr) == true)
            return;

        int a_UniqueID = -1;
        if (int.TryParse(a_GetStr, out a_UniqueID) == false)
            return;

        m_RT_ItemNdList =
                m_RT_SvContent.transform.GetComponentsInChildren<RT_ItemNode>();
        int a_FindIdx = -1;
        for(int i = 0; i < m_RT_ItemNdList.Length; i++)
        {
            if(a_UniqueID == m_RT_ItemNdList[i].m_UniqueID)
            {
                a_FindIdx = m_RT_ItemNdList[i].transform.GetSiblingIndex();
                //차일드로 붙어 있는 순서 인덱스를 가져오는 함수
                a_FindIdx = (int)(a_FindIdx / 3);
                break;
            }//if(a_UniqueID == m_RT_ItemNdList[i].m_UniqueID)
        }//for(int i = 0; i < m_RT_ItemNdList.Length; i++)

        int a_NodeCount = 0;
        if (0 < m_RT_ItemNdList.Length)
            a_NodeCount = (int)(m_RT_ItemNdList.Length / 3) + 1;

        if(0 <= a_FindIdx && a_FindIdx < a_NodeCount)  //찾은 경우
        {
            if (0 < a_FindIdx)
                a_FindIdx = a_FindIdx + 1;

            float normalizePosition = a_FindIdx / (float)a_NodeCount;
            m_RT_ScrollView.verticalNormalizedPosition = 
                                             1.0f - normalizePosition;
            //1.0일때가 시작 위치,  0.0에 가까워질 수록 끝 위치
        }//if(0 <= a_FindIdx && a_FindIdx < a_NodeCount)  //찾은 경우

    }//void RT_MoveNodeClick() 


    void BackBtnClick()
    {
        SceneManager.LoadScene("LobbyScene");
    }
}
