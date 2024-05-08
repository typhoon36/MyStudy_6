using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RT_ItemNode : MonoBehaviour
{
    public Texture[] m_ItemImg = null;

    [HideInInspector] public int m_UniqueID = -1;
    [HideInInspector] public string m_ItemName = "";
    [HideInInspector] public int m_Level = -1;
    [HideInInspector] public bool m_IsSelected = false;

    public Image m_SelctImg;
    public RawImage m_IconImg;
    public Text m_InfoText;



    // Start is called before the first frame update
    void Start()
    {
        m_IsSelected = false;

        this.GetComponent<Button>().onClick.AddListener(()=>
        {
            //선택 상태 변경
            m_IsSelected = !m_IsSelected;
            if(m_SelctImg != null)
            {
                m_SelctImg.gameObject.SetActive(m_IsSelected);
            }

        });
    }

    //// Update is called once per frame
    //void Update()
    //{
        
    //}

    public  void InitInfo(int a_UniqueID, Item_Type a_ItemType,
        string a_Name,  int a_Level)
    {
        m_UniqueID = a_UniqueID;
        m_ItemName = a_Name;
        m_Level = a_Level;
        
        m_InfoText.text = m_ItemName + " Lv(" + m_Level.ToString()+")";
        m_IconImg.texture = m_ItemImg[(int)a_ItemType];



    }

}
