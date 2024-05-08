using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraCtrl : MonoBehaviour
{
    Camera RefCam = null;
    public CinemachineVirtualCamera m_vCam;

    //--- 카메라 줌 인아웃
    float maxDist = 20.0f;  //줌아웃의 최대값
    float minDist = 5.0f;   //줌인의 최소값
    float zoomSpeed = 1.0f; //줌인아웃의 속도
    float distance = 15.0f;
    //--- 카메라 줌 인아웃

    // Start is called before the first frame update
    void Start()
    {
        RefCam = GetComponent<Camera>();
        //if (RefCam != null)
        //    distance = RefCam.orthographicSize;   //시네머신을 사용하지 않고 기본 카메라 사용시

        if(m_vCam != null)
            distance = m_vCam.m_Lens.OrthographicSize; //시네머신을 사용하고 있는 경우
    }

    // Update is called once per frame
    void LateUpdate()
    {
        //--- PC에서만 작동되는 줌인 줌아웃 기능
        //Input.GetAxis("Mouse ScrollWheel")
        //마우스 휠 당길 때 -0.1f(size가 커지게)
        //마우스 휠 밀때 +0.1f(size가 작이지게)
        if (Input.GetAxis("Mouse ScrollWheel") < 0 && distance < maxDist)
        {
            distance += zoomSpeed;
            //RefCam.orthographicSize = distance; //Main Camera인 경우
            m_vCam.m_Lens.OrthographicSize = distance; //시네머신을 사용하는 경우
        }

        if (Input.GetAxis("Mouse ScrollWheel") > 0 && distance > minDist)
        {
            distance -= zoomSpeed;
            //RefCam.orthographicSize = distance; //Main Camera인 경우
            m_vCam.m_Lens.OrthographicSize = distance; //시네머신을 사용하는 경우
        }
        //--- PC에서만 작동되는 줌인 줌아웃 기능
    }
}
