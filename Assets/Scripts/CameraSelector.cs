using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraSelector : MonoBehaviour
{
    public CinemachineStateDrivenCamera stateCam;
    public CinemachineVirtualCamera[] vcams;

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Alpha1)){
            SetCamActive(-1);
        } else if (Input.GetKeyDown(KeyCode.Alpha2)){
            SetCamActive(0);
        } else if (Input.GetKeyDown(KeyCode.Alpha3)){
            SetCamActive(1);
        } else if (Input.GetKeyDown(KeyCode.Alpha4)){
            SetCamActive(2);
        } else if (Input.GetKeyDown(KeyCode.Alpha5)){
            SetCamActive(3);
        } else if (Input.GetKeyDown(KeyCode.Alpha6)){
            SetCamActive(4);
        } else if (Input.GetKeyDown(KeyCode.Alpha7)){
            SetCamActive(5);
        }
    }

    void SetCamActive(int index)
    {
        if (index == -1)
        {
            stateCam.Priority = 15;
            for (int i = 0; i < vcams.Length; i++) vcams[i].Priority = 10;
        }
        else if (vcams.Length > index)
        {
            stateCam.Priority = 10;
            for (int i = 0; i < vcams.Length; i++)
            {
                if (i == index) vcams[i].Priority = 15;
                else vcams[i].Priority = 10;
            }
        }
    }
}
