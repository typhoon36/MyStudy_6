using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickMark : MonoBehaviour
{
    HeroCtrl m_RefHero = null;
    // 히어로 스크립트 연결용 변수
    Vector3 m_CacVLen = Vector3.zero;
    //길이 변수 선언

    float m_AddTimer = 0.0f;
    //시간 변수 
    bool m_IsOnOff = true;
    //OnOff 변수 
    Renderer m_RefRender;
    //렌더러
    //Color32 m_WtColor = new Color32(255, 255, 255, 200);
    Color32 m_YlColor = new Color32(255, 247, 119, 60);
    Color32 m_BrColor = new Color32(0,130,255, 60);
   //색깔값들


    // Start is called before the first frame update
    void Start()
    {
      m_RefRender = gameObject.GetComponent<Renderer>();
      //렌더러 찾아오기




    }

    // Update is called once per frame
    void Update()
    {
        //## 깜빡임 애니메이션
        if(m_RefRender != null)
        {
            m_AddTimer += Time.deltaTime;

            if(0.25f <= m_AddTimer)
            {
                m_IsOnOff = !m_IsOnOff;
                //OnOff 변수 변경
                if(m_IsOnOff == true)
                    m_RefRender.material.SetColor ("_TintColor",m_YlColor);
                //색깔값 변경
                else
                    m_RefRender.material.SetColor ("_TintColor",m_BrColor);
                //색깔값 변경
                m_AddTimer = 0.0f;
            }
        }

        //## 클릭마크 끄기
        //**주인공 사망시에도 클릭마크를 꺼줘야한다.
        if(m_RefHero != null)
        {
            gameObject.SetActive(false);
            return;
        }

        if(gameObject.activeSelf == true)
        {
            if(m_RefHero.m_bMoveOnOff == false) //마우스 이동이 취소되었을때
            
                gameObject.SetActive(false); //게임오브젝트 비활성화


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
