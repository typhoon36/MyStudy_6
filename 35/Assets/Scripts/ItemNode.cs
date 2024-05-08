
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemNode : MonoBehaviour
{

    [HideInInspector] public ulong m_UniqueID = 0;

    [HideInInspector] public bool m_SellOnOff = false;
    public Image m_SelectImg;
    public Image m_IconImg;

    public Text m_TextInfo;

    public Sprite[] m_ItemImg = null;


    // Start is called before the first frame update
    void Start()
    {
        Button a_SelBtn = gameObject.GetComponent<Button>();
        if (a_SelBtn != null)
            a_SelBtn.onClick.AddListener(() =>
            {
                m_SellOnOff = !m_SellOnOff;
                if (m_SelectImg != null)
                    m_SelectImg.gameObject.SetActive(m_SellOnOff);
            });

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SetItemRsc(ItemValue a_Node)
    {


        if (a_Node == null)

            return;

        if (a_Node.m_Itme_Type < Item_Type.IT_armor ||
           Item_Type.IT_helmets < a_Node.m_Itme_Type)

            return;

        m_IconImg.sprite = m_ItemImg[(int)a_Node.m_Itme_Type];

        if (m_TextInfo != null)

            m_TextInfo.text =  "Lv(" + a_Node.m_ItmeLevel.ToString()+")";

        m_UniqueID = a_Node.UniqueID;



    }


}
