using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class StateCamController : MonoBehaviour
{
    public CinemachineFreeLook thirdPerson;

    CinemachineStateDrivenCamera SDCam;
    AxisState prevYAxis;
    bool transition;
    // Start is called before the first frame update
    void Start()
    {
        SDCam = GetComponent<CinemachineStateDrivenCamera>();
        prevYAxis = thirdPerson.m_YAxis;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (SDCam.IsLiveChild((ICinemachineCamera)thirdPerson))
        {
            // Check if CM_ThirdPerson camera just became active
            if (transition == true)
            {
                transition = false;
                thirdPerson.m_YAxis = prevYAxis;

            }
            prevYAxis = thirdPerson.m_YAxis;

            // Reset logic
            //if (Input.GetButtonDown("Reset"))
            if(Input.GetKeyDown(KeyCode.R))
            {
                StartCoroutine(ResetCam());
                
            }
        }
        else transition = true;
    }
    
    IEnumerator ResetCam()
    {
        thirdPerson.m_RecenterToTargetHeading.m_enabled = true;

        float waitTime = thirdPerson.m_RecenterToTargetHeading.m_RecenteringTime + thirdPerson.m_RecenterToTargetHeading.m_RecenterWaitTime;
        yield return new WaitForSeconds(waitTime);

        thirdPerson.m_RecenterToTargetHeading.m_enabled = false;
    }
}
