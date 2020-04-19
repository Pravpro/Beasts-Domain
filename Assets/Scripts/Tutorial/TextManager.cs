using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextManager : MonoBehaviour
{

    public int TextTriggersCount() { return GetComponentsInChildren<TextTriggerController>().Length; }

    public bool TextIsPlaying()
    {
        TextTriggerController[] textBoxes = GetComponentsInChildren<TextTriggerController>();
        for (int i = 0; i < textBoxes.Length; i++) if (textBoxes[i].GetIsPlaying()) return true;
        Debug.Log("Should not get here!");
        return false;
    }
}
