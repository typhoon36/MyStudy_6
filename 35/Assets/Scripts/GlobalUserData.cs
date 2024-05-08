using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalUserData
{
    public static int g_UserGold = 0;
    public static int g_BombCount = 0;
    public static string g_UserName = "User";

    //## �ӽ� item ����Ű ������
    public static ulong UniqueCount = 0;
    public static List<ItemValue> g_ItemList =
        new List<ItemValue>();
    public static void LoadGameInfo()
    {
        g_UserGold = PlayerPrefs.GetInt("GoldCount", 0);
        g_BombCount = PlayerPrefs.GetInt("BombCount", 0);
        g_UserName = PlayerPrefs.GetString("UserName", "User");

        ReflashItemLoad();


    }


    //## �ӽ� item ����Ű ���� �Լ�
    public static ulong GetUnique()
    {
        UniqueCount = (ulong)PlayerPrefs.GetInt("SvUnique", 0);
        UniqueCount++;
        ulong a_Index = UniqueCount;

        //##  �ڽ� �κ��丮 �����ϴ� ������ ��ȣ���� ū�������Ѵ�.

        //## ����Ƽũ ���̵� �߱�
        if (0 < g_ItemList.Count)
        {
            for (int i = 0; i < g_ItemList.Count; i++)
            {
                if (g_ItemList[i] == null)
                    continue;

                if (a_Index <= g_ItemList[i].UniqueID)

                    a_Index = g_ItemList[i].UniqueID + 1;



            }
        }


        UniqueCount = a_Index;


        PlayerPrefs.SetInt("SvUnique", (int)UniqueCount);

        return a_Index;


    }


    //## g_ItmelIst ����
    public static void ReflashItemLoad()
    {
        g_ItemList.Clear();


        ItemValue a_LNode;
        int a_ItemCount = PlayerPrefs.GetInt("ItemCount", 0);

        for (int i = 0; i< a_ItemCount; i++)
        {
            a_LNode = new ItemValue();
            string stUniqueID = PlayerPrefs.GetString($"IT_{i}_st_UniqueID", "");
            ulong.TryParse(stUniqueID, out a_LNode.UniqueID);
            a_LNode.m_Itme_Type = (Item_Type)PlayerPrefs.GetInt($"IT_{i}_Item_Type", 0);
            a_LNode.m_ItemeName = PlayerPrefs.GetString($"IT_{i}_Item_Name", "");
            a_LNode.m_ItmeLevel = PlayerPrefs.GetInt($"IT_{i}_Item_Level", 0);

            g_ItemList.Add(a_LNode);
        }

    }

    //## ����Ʈ ������
    public static void ReflashItemSave()
    {
    
        int a_ItemCount = PlayerPrefs.GetInt("ItemCount", 0);
        for (int i = 0; i < a_ItemCount; i++)
        {
            PlayerPrefs.DeleteKey($"IT_{i}_st_UniqueID");
            PlayerPrefs.DeleteKey($"IT_{i}_Item_Type");
            PlayerPrefs.DeleteKey($"IT_{i}_Item_Name");
            PlayerPrefs.DeleteKey($"IT_{i}_Item_Level");
            PlayerPrefs.DeleteKey($"IT_{i}_Item_Star");


        }


        //�����ۼ� ����
        PlayerPrefs.DeleteKey("ItemCount");
       
        PlayerPrefs.Save();
        //���� Ű���� ��� ��������� ����.
        //(playerprefs�� �޸𸮻� �����͸� �����ϰ� �ϵ� ����̺꿡 �����ϴµ�,
        //�޸� �� ������ �ӽ÷� �����̱⿡ ���α׷��� ������ �����Ͱ� �սǵǴ� ����� ����.
        //�׷��� save()�Լ��� �ӽ������ �����͸� �ϵ� ����̺꿡 ����Ǿ� �ս��� ������)

        //�� ����Ʈ ����
        ItemValue a_SvNode;
        PlayerPrefs.SetInt("ItemCount", g_ItemList.Count);
        

        for(int i = 0; i < g_ItemList.Count; i++)
        {
            a_SvNode = g_ItemList[i];
            PlayerPrefs.SetString($"IT_{i}_st_UniqueID", a_SvNode.UniqueID.ToString());
            PlayerPrefs.SetInt($"IT_{i}_Item_Type", (int)a_SvNode.m_Itme_Type);
            PlayerPrefs.SetString($"IT_{i}_Item_Name", a_SvNode.m_ItemeName);
            PlayerPrefs.SetInt($"IT_{i}_Item_Level", a_SvNode.m_ItmeLevel);
            PlayerPrefs.SetInt($"IT_{i}_Item_Star", a_SvNode.m_ItmeStar);
        

        }
        PlayerPrefs.Save();
        //���� Ű���� ��� ��������� ����.
    }
    public static void SaveGoldAndSkillCount(int gold, int skillCount)
    {
        PlayerPrefs.SetInt("Gold", gold);
        PlayerPrefs.SetInt("SkillCount", skillCount);
        PlayerPrefs.Save();
    }



    public static void SaveGoldAndSkillCount()
    {
        PlayerPrefs.SetInt("Gold", g_UserGold);
        PlayerPrefs.SetInt("SkillCount", g_BombCount);
        PlayerPrefs.Save();
    }

    public static void LoadGoldAndSkillCount()
    {
        g_UserGold = PlayerPrefs.GetInt("Gold", 0);
        g_BombCount = PlayerPrefs.GetInt("SkillCount", 0);
    }
}

