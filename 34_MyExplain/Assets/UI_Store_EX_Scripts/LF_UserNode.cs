using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LF_UserNode : MonoBehaviour
{
    [HideInInspector] public int m_UniqueUD = -1;
    //���� ������ȣ

    [HideInInspector] public string m_UserName = "";
    //���� �̸�

    [HideInInspector] public int m_UserLevel = -1;
    //���� ����
    [HideInInspector] public bool m_IsSelected = false;
    //���ÿ���

    public RawImage m_SelectImg;
    public Text m_InfoText;

     void Start()
    {
        m_IsSelected = false;
        this.GetComponent<Button>().onClick.AddListener(OnClickMethod);
    }

  

    public void InitInfo(int a_UniqueUD, string a_Name, int a_Level)
    {
        m_UniqueUD = a_UniqueUD;
        m_UserName = a_Name;
        m_UserLevel = a_Level;
        m_InfoText.text = a_Name + " Lv(" + a_Level.ToString() + ")";

    }

    //## ��ư ���ý� ���� ���� ǥ�� 
    private void OnClickMethod()
    {
        m_IsSelected =!m_IsSelected;
        if(m_SelectImg != null)
        {
            m_SelectImg.gameObject.SetActive(m_IsSelected);
        }
    }
}
