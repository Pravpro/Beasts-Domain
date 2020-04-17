using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.UI;

public class TextTriggerController : MonoBehaviour
{
    [System.Serializable]
    public class TextBlock
    {
        public Text text;
        [Header("optional")]
        public CinemachineVirtualCamera vcam;
    }

    public GameObject target;
    public float delay = 0f;
    public bool destroyOnUse = false;
    public bool freezeGame = false;
    public TextBlock[] text;

    [Header("optional")]
    public GameObject[] freezeObjects;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == target)
        {
            Coroutine WaitForText = StartCoroutine(PlayText());
        }
    }

    IEnumerator PlayText()
    {
        yield return new WaitForSeconds(delay);

        // Freeze stuff
        if (freezeGame) Time.timeScale = 0;

        RigidbodyConstraints[] constraints = new RigidbodyConstraints[freezeObjects.Length];
        for (int i = 0; i < freezeObjects.Length; i++)
        {
            constraints[i] = freezeObjects[i].GetComponent<Rigidbody>().constraints; ;
            freezeObjects[i].GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
        }

        // Set Cam
        


    }

}
