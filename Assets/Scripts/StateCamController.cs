using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using Rewired;

public class StateCamController : MonoBehaviour
{
    public CinemachineFreeLook thirdPerson;

    CinemachineStateDrivenCamera SDCam;
    AxisState prevYAxis;
    bool transition;

    // player id for reference Rewired input
    // we only have one player id will always = 0
    private int m_playerID = 0;
    private Player m_playerInput;

    // Start is called before the first frame update
    void Start()
    {
        SDCam = GetComponent<CinemachineStateDrivenCamera>();
        prevYAxis = thirdPerson.m_YAxis;

        // set the input to use rewired
        CinemachineCore.GetInputAxis = ReInput.players.GetPlayer(m_playerID).GetAxis;

        // to access input using rewired
        m_playerInput = ReInput.players.GetPlayer(m_playerID);

       
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

            if (m_playerInput.GetButtonDown("Reset") && !m_playerInput.GetButton("Spell"))
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
