using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//전체 검색 키 < Ctrl > + < , > 

public class ItemNode : MonoBehaviour
{
    [HideInInspector] public ulong m_UniqueID = 0;

    [HideInInspector] public bool m_SelOnOff = false;
    public Image m_SelectImg;
    public Image m_IconImg;
    public Text  m_TextInfo;

    public Sprite[] m_ItemImg = null;

    // Start is called before the first frame update
    void Start()
    {
        Button a_SelBtn = gameObject.GetComponent<Button>();
        if (a_SelBtn != null)
            a_SelBtn.onClick.AddListener(() =>
            {
                m_SelOnOff = !m_SelOnOff;
                if (m_SelectImg != null)
                    m_SelectImg.gameObject.SetActive(m_SelOnOff);
            });
    }

    //// Update is called once per frame
    //void Update()
    //{
        
    //}

    public void SetItemRsc(ItemValue a_Node)
    {
        if (a_Node == null)
            return;

        if(a_Node.m_Item_Type < Item_Type.IT_armor ||
            Item_Type.IT_helmets < a_Node.m_Item_Type)
            return;

        m_IconImg.sprite = m_ItemImg[(int)a_Node.m_Item_Type];

        if (m_TextInfo != null)
            m_TextInfo.text = "Lv(" + a_Node.m_ItemLevel.ToString() + ")";

        m_UniqueID = a_Node.UniqueID;
    }

}
