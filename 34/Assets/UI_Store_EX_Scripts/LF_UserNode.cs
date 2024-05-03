using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LF_UserNode : MonoBehaviour
{
    [HideInInspector] public int m_UniqueUD = -1;
    //유저 고유번호

    [HideInInspector] public string m_UserName = "";
    //유저 이름

    [HideInInspector] public int m_UserLevel = -1;
    //유저 레벨
    [HideInInspector] public bool m_IsSelected = false;
    //선택여부

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

    //## 버튼 선택시 선택 상태 표시 
    private void OnClickMethod()
    {
        m_IsSelected =!m_IsSelected;
        if(m_SelectImg != null)
        {
            m_SelectImg.gameObject.SetActive(m_IsSelected);
        }
    }
}
