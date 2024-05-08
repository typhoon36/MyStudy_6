using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class My_ExPlan : MonoBehaviour
{
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
        //## RightGroup List 초기화
        if (m_RT_AddNodeBtn != null)
            m_RT_AddNodeBtn.onClick.AddListener(RT_AddNodeClick);
        if (m_RT_SelDelBtn != null)
            m_RT_SelDelBtn.onClick.AddListener(RT_SelDelClick);
        if (m_RT_MoveNodeBtn != null)
            m_RT_MoveNodeBtn.onClick.AddListener(RT_MoveNodeClick);
    }

    // Update is called once per frame
    void Update()
    {

    }

    //---------------------내 과제 코드---------------------

    //## 아이템 노드 추가
    void RT_AddNodeClick()
    {
        if (m_RT_NodePrefab == null)
            return;

        GameObject a_ItemObj = Instantiate(m_RT_NodePrefab);
        a_ItemObj.transform.SetParent(m_RT_SvContent.transform, false);

        RT_ItemNode a_SvNode = a_ItemObj.GetComponent<RT_ItemNode>();
        int a_Level = Random.Range(2, 30);

        // 랜덤 인덱스를 생성합니다.
        int randomIndex = Random.Range(0, a_SvNode.m_ItemImg.Length);

        // 랜덤 인덱스를 사용하여 텍스쳐를 선택합니다.
        Texture randomTexture = a_SvNode.m_ItemImg[randomIndex];

        a_SvNode.InitInfo(a_Item_UniqueID, (Item_Type)randomIndex, "Item", a_Level); // 아이템 이름을 "Item"으로 고정합니다.
        a_SvNode.m_IconImg.texture = randomTexture; // 선택한 텍스쳐를 설정합니다.
        a_Item_UniqueID++;
    }

    //## 선택된 아이템 노드 삭제
    void RT_SelDelClick()
    {
        m_RT_ItemNdList = m_RT_SvContent.transform.GetComponentsInChildren<RT_ItemNode>();
        int itemCount = m_RT_ItemNdList.Length;

        for (int i = 0; i < itemCount; i++)
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

        // 유니크 아이디 찾기
        int a_UniqueId = -1;
        if (int.TryParse(a_GetStr, out a_UniqueId) == false)
            return;

        // 노드 찾아주기
        m_RT_ItemNdList = m_RT_SvContent.transform.GetComponentsInChildren<RT_ItemNode>();

        int a_FindIdx = -1;

        for (int i = 0; i < m_RT_ItemNdList.Length; i++)
        {
            if (a_UniqueId == m_RT_ItemNdList[i].m_UniqueID)
            {
                a_FindIdx = m_RT_ItemNdList[i].transform.GetSiblingIndex();
                break;
            }
        }

        int a_NodeCount = m_RT_ItemNdList.Length;
        if (0 <= a_FindIdx && a_FindIdx < a_NodeCount)
        {
            if (0 < a_FindIdx)
            {
                a_FindIdx = a_FindIdx - 1;

                float normalizePos = a_FindIdx / (float)a_NodeCount;
                m_RT_ScrollView.verticalNormalizedPosition = 1.0f - normalizePos;
            }
        }
    }



}
