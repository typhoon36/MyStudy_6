using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalUserData 
{
    public static int g_UserGold = 0;
    public static int g_BombCount = 0;
    public static string g_NickName = "User";

    public static ulong UniqueCount = 0;    //임시 Item 고유키 발급기...
    public static List<ItemValue> g_ItemList = new List<ItemValue>();

    public static void LoadGameInfo()
    {
        g_UserGold  = PlayerPrefs.GetInt("GoldCount", 0);
        g_BombCount = PlayerPrefs.GetInt("BombCount", 0);
        g_NickName  = PlayerPrefs.GetString("UserNick", "SBS사냥꾼");

        ReflashItemLoad();
    }

    public static ulong GetUnique() //임시 고유키 발급기...
    {
        UniqueCount = (ulong)PlayerPrefs.GetInt("SvUnique", 0);
        UniqueCount++;
        ulong a_Index = UniqueCount;

        //<-- 자신의 인벤토리에 있는 아이템 번호랑 겹치는 번호보다는 큰 수로
        //UniqueID가 발급되게 처리하는 부분
        if(0 < g_ItemList.Count)
        {
            for(int i = 0; i < g_ItemList.Count; i++)
            {
                if (g_ItemList[i] == null)
                    continue;

                if(a_Index <= g_ItemList[i].UniqueID)
                   a_Index = g_ItemList[i].UniqueID + 1;
            }
        }//if(0 < g_ItemList.Count)

        UniqueCount = a_Index;
        PlayerPrefs.SetInt("SvUnique", (int)UniqueCount);
        return a_Index;
    }

    public static void ReflashItemLoad()  //<-- g_ItemList 갱신
    {
        g_ItemList.Clear();

        ItemValue a_LdNode;
        int a_ItemCount = PlayerPrefs.GetInt("Item_Count", 0);
        for(int i = 0; i < a_ItemCount; i++)
        {
            a_LdNode = new ItemValue();
            string stUniqueID = PlayerPrefs.GetString($"IT_{i}_stUniqueID", "");
            ulong.TryParse(stUniqueID, out a_LdNode.UniqueID);
            a_LdNode.m_Item_Type = (Item_Type)PlayerPrefs.GetInt($"IT_{i}_Item_Type", 0);
            a_LdNode.m_ItemName  = PlayerPrefs.GetString($"IT_{i}_ItemName", "");
            a_LdNode.m_ItemLevel = PlayerPrefs.GetInt($"IT_{i}_ItemLevel", 0);
            a_LdNode.m_ItemStar  = PlayerPrefs.GetInt($"IT_{i}_ItemStar", 0);

            g_ItemList.Add(a_LdNode);
        }
    }//public static void ReflashItemLoad()  //<-- g_ItemList 갱신

    public static void ReflashItemSave()  //<-- 리스트 다시 저장
    {
        //--- 기존에 저장되어 있었던 아이템 목록 제거
        int a_ItemCount = PlayerPrefs.GetInt("Item_Count", 0);
        for(int i = 0; i < a_ItemCount + 10; i++)
        {
            PlayerPrefs.DeleteKey($"IT_{i}_stUniqueID");
            PlayerPrefs.DeleteKey($"IT_{i}_Item_Type");
            PlayerPrefs.DeleteKey($"IT_{i}_ItemName");
            PlayerPrefs.DeleteKey($"IT_{i}_ItemLevel");
            PlayerPrefs.DeleteKey($"IT_{i}_ItemStar");
        }
        PlayerPrefs.DeleteKey("Item_Count"); //아이템 수 제거
        PlayerPrefs.Save(); //폰에서 마지막 저장상태를 확실히 저장하게 하기 위해서...
        //변경된 모든 키 값을 물리적인 저장 공간에 저장
        //PlayerPrefs은 데이터를 메모리 상에 저장하고 이를 하드 드라이브에 저장한다.
        //메모리 상의 저장은 임시적인 저장으로, 갑작스러운 프로그램의 정지와 같은 문제가 발생했을 때,
        //저장이 이루어지지 않는 경우가 발생할 수 있다. PlayerPrefs의 Save() 함수는 메모리 상에 저장된
        //데이터를 하드 드라이브에 저장한다.
        //--- 기존에 저장되어 있었던 아이템 목록 제거

        //--- 새로운 리스트 저장
        ItemValue a_SvNode;
        PlayerPrefs.SetInt("Item_Count", g_ItemList.Count);
        for(int i = 0; i < g_ItemList.Count; i++)
        {
            a_SvNode = g_ItemList[i];
            PlayerPrefs.SetString($"IT_{i}_stUniqueID", a_SvNode.UniqueID.ToString());
            PlayerPrefs.SetInt($"IT_{i}_Item_Type", (int)a_SvNode.m_Item_Type);
            PlayerPrefs.SetString($"IT_{i}_ItemName", a_SvNode.m_ItemName);
            PlayerPrefs.SetInt($"IT_{i}_ItemLevel", a_SvNode.m_ItemLevel);
            PlayerPrefs.SetInt($"IT_{i}_ItemStar",  a_SvNode.m_ItemStar);
        }
        PlayerPrefs.Save(); //폰에서 마지막 저장상태를 확실히 저장하게 하기 위해서...
        //--- 새로운 리스트 저장

    }//public static void ReflashItemSave() //<-- 리스트 다시 저장
}
