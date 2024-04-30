using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Monster_Ctrl : MonoBehaviour
{
    float m_MaxHp = 100.0f;
    float m_CurHp = 100.0f;
    public Image HpBarUI = null;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnTriggerEnter(Collider coll)
    {
        if (coll.gameObject.name.Contains("BulletPrefab")==true)
        {

            TakeDamage(10.0f);

            Destroy(coll.gameObject);
        }
    }

    public void TakeDamage(float a_Value)
    {
        if(m_CurHp <= 0.0f)
            return;

        Game_Mgr.Inst.DamageText((int) a_Value, this.transform.position);

        m_CurHp -= a_Value;
        if(m_CurHp < 0.0f)
            m_CurHp = 0.0f;
           
        

        if(HpBarUI != null)
        
            HpBarUI.fillAmount = m_CurHp / m_MaxHp;

        if (m_CurHp <= 0.0f) //몬스터 사망시
        {
            //보상 로직


            Destroy(gameObject);
        }


    }

}
