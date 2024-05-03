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



    }

    

    

    // Update is called once per frame
    void Update()
    {
        
    }

    void BackBtnClick()
    {
        SceneManager.LoadScene("LobbyScene");
    }

    private void LF_AddNodeClick()
    {
        if(m_LF_NodePrefab == null)  
            return;
        
        GameObject a_UserObj = Instantiate(m_LF_NodePrefab);
        a_UserObj.transform.SetParent(m_LF_SvContent.transform , false);

        LF_UserNode a_SvNode = a_UserObj.GetComponent<LF_UserNode>();
        string a_UName = "User" + g_UniqueUD.ToString();
        int a_Level = Random.Range(2,30);
        a_SvNode.InitInfo(g_UniqueUD, a_UName, a_Level);
        g_UniqueUD++;


    }



    private void LF_SelDelClick()
    {

    }


    private void LF_MoveNodeClick()
    {

    }

}
