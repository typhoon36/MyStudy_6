using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickMark : MonoBehaviour
{
    HeroCtrl m_RefHero = null;
    // ����� ��ũ��Ʈ ����� ����
    Vector3 m_CacVLen = Vector3.zero;
    //���� ���� ����

    float m_AddTimer = 0.0f;
    //�ð� ���� 
    bool m_IsOnOff = true;
    //OnOff ���� 
    Renderer m_RefRender;
    //������
    //Color32 m_WtColor = new Color32(255, 255, 255, 200);
    Color32 m_YlColor = new Color32(255, 247, 119, 60);
    Color32 m_BrColor = new Color32(0,130,255, 60);
   //���򰪵�


    // Start is called before the first frame update
    void Start()
    {
      m_RefRender = gameObject.GetComponent<Renderer>();
      //������ ã�ƿ���




    }

    // Update is called once per frame
    void Update()
    {
        //## ������ �ִϸ��̼�
        if(m_RefRender != null)
        {
            m_AddTimer += Time.deltaTime;

            if(0.25f <= m_AddTimer)
            {
                m_IsOnOff = !m_IsOnOff;
                //OnOff ���� ����
                if(m_IsOnOff == true)
                    m_RefRender.material.SetColor ("_TintColor",m_YlColor);
                //���� ����
                else
                    m_RefRender.material.SetColor ("_TintColor",m_BrColor);
                //���� ����
                m_AddTimer = 0.0f;
            }
        }

        //## Ŭ����ũ ����
        //**���ΰ� ����ÿ��� Ŭ����ũ�� ������Ѵ�.
        if(m_RefHero != null)
        {
            gameObject.SetActive(false);
            return;
        }

        if(gameObject.activeSelf == true)
        {
            if(m_RefHero.m_bMoveOnOff == false) //���콺 �̵��� ��ҵǾ�����
            
                gameObject.SetActive(false); //���ӿ�����Ʈ ��Ȱ��ȭ


            m_CacVLen = m_RefHero.transform.position - transform.position;
            m_CacVLen.y = 0.0f;
            if(m_CacVLen.magnitude < 1.0f)
                gameObject.SetActive(false);
            
        }

    }

    public void PlayEff(Vector3 a_PickVec , HeroCtrl a_RefHero)
    {
        m_RefHero = a_RefHero;



        transform.position = new Vector3(a_PickVec.x, 0.8f, a_PickVec.z);
        gameObject.SetActive(true);

    }

    public void ClickMarkOnOff(bool val)
    {
        gameObject.SetActive(val);
    }


}
