using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//## ���� ��� ��
//����:ĳ�̹��׿� ���� �ν�����â�� �ȹٲ� ����(���� �� �ٽ� �־��ֱ� Ȥ�� ����Ƽ �����)

public class DamageText : MonoBehaviour
{
    Text m_RefText = null;

    public AnimationCurve ScaleCurve = new AnimationCurve
        (new Keyframe[] { new Keyframe(0.0f, 0.03f), new Keyframe(0.2f, 0.061f) });

    public AnimationCurve moveCurve = new AnimationCurve
        (new Keyframe[] { new Keyframe(0.19f, 0.0f), new Keyframe(0.65f, 2.8f) });

    public AnimationCurve alphaCurve = new AnimationCurve
        (new Keyframe[] { new Keyframe(0.40f,1.0f), new Keyframe(1.0f, 0.0f) });

    //## ���� ���� ����
    float m_StartTime = 0.0f;
    float m_CurTime = 0.0f;

    Vector3 m_CacScVec = Vector3.zero;
    float m_CacScale = 0.0f;

    Vector3 m_CacCurPos = Vector3.zero;
    float m_MvOffset = 0.0f;

    Color m_Color = new Color32 (200,0, 0, 255);
   float m_alpha = 1.0f;


    float m_DamageVal = 10.0f;

    // Start is called before the first frame update
    void Start()
    {
        if(m_RefText == null)
            m_RefText = this .gameObject.GetComponentInChildren<Text>();

        if(m_RefText != null)
        {
           m_Color = m_RefText.color;
            m_RefText.text = "-" + m_DamageVal.ToString();
        }

        m_StartTime = Time.time;

        Destroy(this.gameObject, 1.5f);
    }

    // Update is called once per frame
    void Update()
    {
        m_CurTime = Time.time;

        //## ��Ī ȿ�� ���
        m_CacScale = ScaleCurve.Evaluate(m_CurTime - m_StartTime);
        m_CacScVec.x = m_CacScale;
        m_CacScVec.y = m_CacScale;
        m_CacScVec.z = 1.0f;

        m_RefText.transform.localScale = m_CacScVec;

        //## �̵�ȿ�� ����
        m_MvOffset = moveCurve.Evaluate(m_CurTime - m_StartTime);
        m_CacCurPos.z = m_MvOffset;
        m_RefText.transform.localPosition = m_CacCurPos;

        //## ���İ� ȿ�� ����
        m_alpha = alphaCurve.Evaluate(m_CurTime - m_StartTime);
        m_Color.a = m_alpha;
        m_RefText.color = m_Color;


    }

    public void DamageTextSpawn(float a_Damage, Color a_Color)
    {
        m_DamageVal = a_Damage;
        m_RefText = this.gameObject.GetComponentInChildren<Text>();

        if(m_RefText != null)
        {
            m_RefText.color = m_Color;
            m_RefText.text = "-" + m_DamageVal.ToString();
        }
    }

}