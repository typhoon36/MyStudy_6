using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;

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
    public Item_Type m_Item_Type;
    public string m_ItemName = "";
    public int m_ItemLevel = 0;
    public int m_ItemStar = 0;

    public float m_AddAttack = 0.0f;    //°ø°Ý »ó½Â·ü
    public float m_AddAttSpeed = 0.0f;  //°ø¼Ó »ó½Â·ü
    public float m_AddDefence = 0.0f;   //¹æ¾î »ó½Â·ü
}

public class ItemObjInfo : MonoBehaviour
{
    [Header("----- ItemIconImg -----")]
    public Sprite[] m_ItemImg = null;

    [HideInInspector] public ItemValue m_ItemValue = new ItemValue();

    //// Start is called before the first frame update
    //void Start()
    //{
        
    //}

    //// Update is called once per frame
    //void Update()
    //{
        
    //}

    public void InitItem(Item_Type a_Item_Type, string a_Name, int a_Level, int a_Star)
    {
        m_ItemValue.UniqueID    = GlobalUserData.GetUnique();
        m_ItemValue.m_Item_Type = a_Item_Type;
        m_ItemValue.m_ItemName  = a_Name;
        m_ItemValue.m_ItemLevel = a_Level;
        m_ItemValue.m_ItemStar  = a_Star;

        SpriteRenderer a_RefRender = gameObject.GetComponent<SpriteRenderer>();
        a_RefRender.sprite = m_ItemImg[(int)a_Item_Type];
        if(a_Item_Type == Item_Type.IT_coin)
        {
            transform.localScale = new Vector3(transform.localScale.x * 3.75f,
                                               transform.localScale.y * 3.75f,
                                               transform.localScale.z);

            Vector3 a_BSize = gameObject.GetComponent<BoxCollider>().size;
            a_BSize.x = a_BSize.x / 3.75f;
            a_BSize.y = a_BSize.y / 3.75f;
            gameObject.GetComponent<BoxCollider>().size = a_BSize;
        }
        else if (a_Item_Type == Item_Type.IT_bomb)
        {
            transform.localScale = new Vector3(transform.localScale.x * 1.7f,
                                               transform.localScale.y * 1.7f,
                                               transform.localScale.z);

            Vector3 a_BSize = gameObject.GetComponent<BoxCollider>().size;
            a_BSize.x = a_BSize.x / 1.7f;
            a_BSize.y = a_BSize.y / 1.7f;
            gameObject.GetComponent<BoxCollider>().size = a_BSize;
        }

        Destroy(gameObject, 15.0f);    //15ÃÊ³»¿¡ ¸Ô¾î¾ß ÇÑ´Ù.
    }
}
