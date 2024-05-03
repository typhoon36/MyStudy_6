using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class My_ExPlan : MonoBehaviour
{
    //## RightGroup List ���� ����
    int a_Item_UniqueID = 0;
    [Header("RightGroup ItemList")]
    public ScrollRect m_RT_ScrollView;
    //scrollview ������Ʈ�� �پ��ִ� ������Ʈ
    public GameObject m_RT_SvContent;
    //scrollContent ���ϵ�� ������ �θ�ü
    public GameObject m_RT_NodePrefab = null;
    //nodePrefab

    public Button m_RT_AddNodeBtn = null;
    //��� �߰� ��ư
    public Button m_RT_SelDelBtn = null;
    //���õ� ��� ���� ��ư
    public Button m_RT_MoveNodeBtn = null;
    //��� �̵� ��ư

    public InputField m_RT_InputField = null;

    [HideInInspector] public RT_ItemNode[] m_RT_ItemNdList;
    //content ������ ������ ������ ������ �迭
    // Start is called before the first frame update
    void Start()
    {
        //## RightGroup List �ʱ�ȭ
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

    //---------------------�� ���� �ڵ�---------------------

    //## ������ ��� �߰�
    void RT_AddNodeClick()
    {
        if (m_RT_NodePrefab == null)
            return;

        GameObject a_ItemObj = Instantiate(m_RT_NodePrefab);
        a_ItemObj.transform.SetParent(m_RT_SvContent.transform, false);

        RT_ItemNode a_SvNode = a_ItemObj.GetComponent<RT_ItemNode>();
        int a_Level = Random.Range(2, 30);

        // ���� �ε����� �����մϴ�.
        int randomIndex = Random.Range(0, a_SvNode.m_ItemImg.Length);

        // ���� �ε����� ����Ͽ� �ؽ��ĸ� �����մϴ�.
        Texture randomTexture = a_SvNode.m_ItemImg[randomIndex];

        a_SvNode.InitInfo(a_Item_UniqueID, (Item_Type)randomIndex, "Item", a_Level); // ������ �̸��� "Item"���� �����մϴ�.
        a_SvNode.m_IconImg.texture = randomTexture; // ������ �ؽ��ĸ� �����մϴ�.
        a_Item_UniqueID++;
    }

    //## ���õ� ������ ��� ����
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


    //## ������ ������ȣ�� ��� �̵�
    void RT_MoveNodeClick()
    {
        if (m_RT_InputField == null)
            return;

        string a_GetStr = m_RT_InputField.text.Trim();
        if (string.IsNullOrEmpty(a_GetStr) == true)
            return;

        // ����ũ ���̵� ã��
        int a_UniqueId = -1;
        if (int.TryParse(a_GetStr, out a_UniqueId) == false)
            return;

        // ��� ã���ֱ�
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
