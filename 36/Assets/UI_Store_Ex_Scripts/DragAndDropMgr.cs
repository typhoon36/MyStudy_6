using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;
using UnityEngine.UI;

public class DragAndDropMgr : MonoBehaviour
{
    public SlotScript[] m_SlotSc;
    public RawImage m_MsObj = null;

    int m_SaveIdx = -1;
    int m_DrtIdx = -1;
    //direction �ε���
    //bool m_IsPick = false;


    //## ������ �����ϰ� ��������ϴ� ���� ����
    float AniDur = 0.8f;
    //fade out ���� �ð� ����
    float m_CacTimer = 0.0f;
    float m_AddTimer = 0.0f;
    Color m_Color;



    [Header("-----TextUI----")]
    public Text m_GoldText;
    public Text m_SkillText;


    [Header("----Help TextUI----")]
    public Text m_HelpText;
    float m_HelpDur = 1.5f;
    //fade out time
    float m_HelpTimer = 0.0f;
    //timer


    // Start is called before the first frame update
    void Start()
    {
        GlobalUserData.LoadGameInfo();

        if (m_GoldText != null)
        {
            if (GlobalUserData.g_UserGold <= 0)
                m_GoldText.text = "X 00";
            else
                m_GoldText.text = "X " + GlobalUserData.g_UserGold.ToString();


        }

        if (m_SkillText != null)
        {
            if (GlobalUserData.g_BombCount <= 0)
                m_SkillText.text = "X 00";
            else
                m_SkillText.text = "X " + GlobalUserData.g_BombCount.ToString();
        }

    }

    // Update is called once per frame
    void Update()
    {
        //## ���� ���콺 ��ư�� Ŭ���� ������ ó��
        if (Input.GetMouseButtonDown(0) == true)
        {
            BuyMouseBtnDown();

        }


        //## ���� ���콺�� ������ �ִ� ������ ó��
        if (Input.GetMouseButton(0) == true)
        {
            BuyMouseBtnPress();
        }

        //## ���� ���콺 �� ������ ó��
        if (Input.GetMouseButtonUp(0) == true)
        {
            BuyMouseBtnUp();
        }

        BuyDir();

    }


    //## ���Ž� ���� ���� �Լ�
    void BuyDir()
    {
        //## ������ ������� �ϴ� ����
        if( 0.0f < m_AddTimer)
        {
            m_AddTimer -= Time.deltaTime;

            m_CacTimer= m_AddTimer / AniDur;
            m_Color = m_SlotSc[m_DrtIdx].ItemImg.color;
            m_Color.a = m_CacTimer;
            m_SlotSc[m_DrtIdx].ItemImg.color = m_Color;

            if(m_AddTimer <= 0.0f)
            {
                m_SlotSc[m_DrtIdx].ItemImg.gameObject.SetActive(false);
            }

        }
    }

    //## ���콺�� UI���� ���� �մ°� �Լ�
    bool IsCollSlot(GameObject a_CkObj)
    {
        Vector3[] v = new Vector3[4];
        a_CkObj.GetComponent<RectTransform>().GetWorldCorners(v);


        if (v[0].x < Input.mousePosition.x &&Input.mousePosition.x < v[2].x &&
            v[0].y < Input.mousePosition.y && Input.mousePosition.y < v[2].y)
        {
            return true;
        }

        return false;
        
    }

    void BuyMouseBtnDown()
    {
        m_SaveIdx = -1;



      for(int i =0; i< m_SlotSc.Length; i++)
        {

            //ù��° ������ ���źҰ�
            if (i == 1)
                continue;


                if (m_SlotSc[i].ItemImg.gameObject.activeSelf == true && IsCollSlot(m_SlotSc[i].gameObject)== true)
            {
                m_SaveIdx = i;
                m_SlotSc[i].ItemImg.gameObject.SetActive(false);
                m_MsObj.gameObject.SetActive(true);
                m_MsObj.transform.position = Input.mousePosition;
                break;
            }
        }
    }

    void BuyMouseBtnPress()
    {
        if(0 <= m_SaveIdx)
        {
            m_MsObj.transform.position = Input.mousePosition;
        }


    }

    //void BuyMouseBtnUp()
    //{
    //    if (m_SaveIdx < 0)
    //        return;

    //    for(int i =0; i< m_SlotSc.Length; i++)
    //    {
    //        if (m_SlotSc[i].ItemImg.gameObject.activeSelf == false &&
    //            IsCollSlot(m_SlotSc[i].gameObject) == true)
    //        {
    //            m_SlotSc[i].ItemImg.gameObject.SetActive(true);
    //            m_SlotSc[i].ItemImg.color = Color.white;
              
    //            m_DrtIdx = i;
    //            m_SaveIdx = -1;

    //            m_MsObj.gameObject.SetActive(false);
    //            break;
    //        }
    //    }

    //    if(0 <= m_SaveIdx)
    //    {
    //        m_SlotSc[m_SaveIdx].ItemImg.gameObject.SetActive(true);
    //        m_SaveIdx = -1;
    //        m_MsObj.gameObject.SetActive(false);
    //    }



    //}

   void BuyMouseBtnUp()
    {
        if(m_SaveIdx < 0)
        
            return;
        
        for(int i = 0; i < m_SlotSc.Length; i++)
        {
            //�ڱ� �ڸ��� ���� ��� ���źҰ�
           if(m_SaveIdx == i)
                continue;

            if (m_SlotSc[i].ItemImg.gameObject.activeSelf == false &&
                               IsCollSlot(m_SlotSc[i].gameObject) == true)
            {
                //## �����㰡
                if(100 <= GlobalUserData.g_UserGold)
                {
                    m_SlotSc[i].ItemImg.gameObject.SetActive(true);
                    m_SlotSc[i].ItemImg.color = Color.white;
                    m_DrtIdx = i;
                    m_AddTimer = AniDur;
                    m_SaveIdx = -1;
                    m_MsObj.gameObject.SetActive(false);

                    GlobalUserData.g_UserGold -= 100;
                    m_GoldText.text = "X " + GlobalUserData.g_UserGold.ToString("N0");
                    PlayerPrefs.SetInt("GoldCount", GlobalUserData.g_UserGold);

                    GlobalUserData.g_BombCount += 1;
                    m_SkillText.text = "X " + GlobalUserData.g_BombCount.ToString();

                    PlayerPrefs.SetInt("BombCount", GlobalUserData.g_BombCount);


                }


                //## ���źҰ�
                else
                {
                    m_HelpText.gameObject.SetActive(true);
                    m_HelpText.color = Color.white;
                    m_HelpTimer = m_HelpDur;
                }   

                break;

            }
            
        }

        if(0 <= m_SaveIdx)
        {
            m_SlotSc[m_SaveIdx].ItemImg.gameObject.SetActive(true);
            m_SaveIdx = -1;
            m_MsObj.gameObject.SetActive(false);
        }




    }

    
}
