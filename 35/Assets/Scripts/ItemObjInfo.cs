using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Item_Type
{
    IT_coin,
    IT_bomb,
    IT_armor,
    IT_axe,
    IT_boots,
    IT_helmets
}




public class ItemValue
{
    public ulong UniqueID = 0;
    public Item_Type m_Itme_Type;
    public string m_ItemeName = "";
    public int m_ItmeLevel = 0;
    public int m_ItmeStar = 0;

    public float m_AddAtk = 0.0f;
    //공격력
    public float m_AddAttSpeed = 0.0f;
    //공격속도
    public float m_AddDef = 0.0f;
    //방어력
}



public class ItemObjInfo : MonoBehaviour
{
    [Header("아이템 정보")] public Sprite[] m_ItemImg = null;

    [HideInInspector]public ItemValue m_ItemValue = new ItemValue();
    


    public void InitItem(Item_Type a_Item_Type, string a_Name, 
        int a_Level,int a_Star)
    {
        m_ItemValue.UniqueID = GlobalUserData.GetUnique();
        m_ItemValue.m_Itme_Type = a_Item_Type;
        m_ItemValue.m_ItemeName = a_Name;
        m_ItemValue.m_ItmeLevel = a_Level;
        m_ItemValue.m_ItmeStar = a_Star;

        SpriteRenderer a_RefRender = gameObject.GetComponent<SpriteRenderer>();
        a_RefRender.sprite = m_ItemImg[(int)a_Item_Type];

        if (a_Item_Type == Item_Type.IT_coin)
        {
            transform.localScale = new Vector3(transform.localScale.x * 3.75f,
                transform.localScale.y * 5.0f, transform.localScale.z);


            Vector3 a_BSize = gameObject.GetComponent<BoxCollider>().size;

            a_BSize.x = a_BSize.x * 3.75f;
            a_BSize.y = a_BSize.y * 3.75f;
            gameObject.GetComponent<BoxCollider>().size = a_BSize;


        }

        else if(a_Item_Type == Item_Type.IT_bomb)
        {
            transform.localScale = new Vector3(transform.localScale.x * 1.7f,
                               transform.localScale.y * 1.7f, transform.localScale.z);

            Vector3 a_BsSIze = gameObject.GetComponent<BoxCollider>().size;
            a_BsSIze.x = a_BsSIze.x / 1.7f;
            a_BsSIze.y = a_BsSIze.y / 1.7f;
            gameObject.GetComponent<BoxCollider>().size = a_BsSIze;

        }



        Destroy(gameObject, 15.0f); 
        //15초후에 삭제(아이템이 떨어지고 15초후에 삭제)
    }
}

