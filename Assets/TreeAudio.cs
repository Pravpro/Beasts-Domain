using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class TreeAudio : MonoBehaviour
{
    public AudioClip Fall;
    public AudioSource Tree;

    public void TreeFall()
    {
        Tree.Play();
        Tree.pitch = Random.Range(0.7f, 1.2f);
    }  
}
