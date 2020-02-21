using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class StateCamController : MonoBehaviour
{
    public CinemachineFreeLook thirdPerson;

    CinemachineStateDrivenCamera SDCam;
    AxisState fixYAxis;
    bool transition;
    // Start is called before the first frame update
    void Start()
    {
        SDCam = GetComponent<CinemachineStateDrivenCamera>();
        fixYAxis = thirdPerson.m_YAxis;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (SDCam.IsLiveChild((ICinemachineCamera)thirdPerson))
        {
            if (transition == true)
            {
                transition = false;
                thirdPerson.m_YAxis = fixYAxis;

            }
            fixYAxis = thirdPerson.m_YAxis;
        }
        else
        {
            transition = true;
            //Debug.Log(thirdPerson.m_YAxis.Value);
            //thirdPerson.m_YAxis.Value = fixYAxis.Value;
        }
    }
}
