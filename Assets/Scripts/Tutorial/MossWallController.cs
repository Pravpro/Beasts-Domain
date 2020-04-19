using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using Cinemachine;

[RequireComponent(typeof(PlayableDirector))]
public class MossWallController : MonoBehaviour
{
    [Tooltip("Add the tutorial text trigger Object here. If added it will be destroyed with the wall.")]
    public TextTriggerController trigger;
    public CinemachineVirtualCamera optionalVcam;

    private bool isDisappearing = false;
    private PlayableDirector pd;
    private CameraSelector camSelector;

    // Start is called before the first frame update
    private GameObject babyMonster;
    void Start()
    {
        babyMonster = GameObject.Find("MonsterSmol");
        camSelector = FindObjectOfType<CameraSelector>();
        pd = GetComponent<PlayableDirector>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!babyMonster.activeSelf && !isDisappearing) MossWallDisappear();
    }

    private void MossWallDisappear()
    {
        isDisappearing = true;
        if (trigger != null) Destroy(trigger);
        if (optionalVcam != null)
        {
            int camIndex = camSelector.AddCamera(optionalVcam);
            camSelector.SetCamActive(camIndex);
        }
        pd.Play();

        Coroutine WaitForDestroy = StartCoroutine(WaitForDuration(DonePlaying));
        if(pd.state != PlayState.Playing)
        {
            Destroy(gameObject);
        }
    }

    private bool DonePlaying()
    {
        return pd.state != PlayState.Playing;
    }

    IEnumerator WaitForDuration(System.Func<bool> conditional)
    {
        yield return new WaitUntil(conditional);
        camSelector.SetCamActive(-1);
        Destroy(gameObject);
    }
}
