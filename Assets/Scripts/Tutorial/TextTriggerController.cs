using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.UI;
using Rewired;

public class TextTriggerController : MonoBehaviour
{
    [System.Serializable]
    public class TextBlock
    {
        [TextArea]
        public string text;
        public CinemachineVirtualCamera OptionalVcam;
    }
    public enum TriggerOn
    {
        onTriggerEnter = 1,
        onTriggerStay = 2,
        onTriggerExit = 3
    }

    [Space]
    [Tooltip("The object that the trigger will detect.")]
    public GameObject target;
    [Tooltip("Select what event to start text dialogue on.")]
    public TriggerOn startOn = TriggerOn.onTriggerEnter;
    [Tooltip("Delay in seconds, after which the text dialogue will display. " +
        "For On Trigger Stay event, Delay is the duration of time Target must stay in trigger for text to start.")]
    public float delay = 0f;
    [Tooltip("Whether this trigger is one-time use or multi-time use.")]
    public bool destroyOnUse = false;
    [Tooltip("Whether to freeze game while textbox plays.")]
    public bool freezeGame = false;

    [Space]
    [Header("UI Stuff")]
    [Space]
    [Tooltip("The Canvas where the Text Box will be displayed.")]
    public Canvas canvas;
    [Tooltip("Image Object must have a child object with a Text UI Component.")]
    public Image textBox;
    [Tooltip("The text blocks that will be displayed in order.")]
    public TextBlock[] textBlocks;

    [Space]
    [Header("Optional")]
    [Space]
    [Tooltip("If not freezing the game, can alternatively freeze rigidbody constraints of objects.")]
    public Rigidbody[] freezeObjects = new Rigidbody[0];

    private RigidbodyConstraints[] constraints;
    private Text textComp;
    private int textIndex;
    private bool isPlaying = false;
    private Image textBoxInstance;
    private CameraSelector camSelector;
    private float stayTime = Mathf.Infinity;

    private int m_playerID = 0;
    private Player m_playerInput;

    private void Start()
    {
        Text textCompCheck = textBox.gameObject.GetComponentInChildren<Text>();
        Debug.Assert(textCompCheck != null, "Image Object does not have a child object with a Text UI Component. YOU MESSED UP!");
        if (textCompCheck == null) Destroy(this);

        // Save all constraints
        constraints = new RigidbodyConstraints[freezeObjects.Length];
        for (int i = 0; i < freezeObjects.Length; i++) constraints[i] = freezeObjects[i].GetComponent<Rigidbody>().constraints;

        m_playerInput = ReInput.players.GetPlayer(m_playerID);
        camSelector = FindObjectOfType<CameraSelector>();
    }

    private void Update()
    {
        if (isPlaying && m_playerInput.GetButtonDown("Jump"))
        {
            textIndex++;
            if (textIndex >= textBlocks.Length) { Coroutine WaitForFrame = StartCoroutine(EndText()); }
            else SetTextBlock();

        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == target)
        {
            if (startOn == TriggerOn.onTriggerEnter)
            {
                Coroutine delayStartText = StartCoroutine(PlayText(delay));
            }

            if (startOn == TriggerOn.onTriggerStay) stayTime = Time.time + delay;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (startOn == TriggerOn.onTriggerStay && other.gameObject == target && stayTime <= Time.time)
        {
            stayTime = Mathf.Infinity;
            Coroutine delayStartText = StartCoroutine(PlayText(0));
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject == target)
        {
            if (startOn == TriggerOn.onTriggerExit)
            {
                Coroutine delayStartText = StartCoroutine(PlayText(delay));
            }
            if (startOn == TriggerOn.onTriggerStay) stayTime = Mathf.Infinity;
        }

    }
    
    IEnumerator PlayText(float delay)
    {
        yield return new WaitForSeconds(delay);

        isPlaying = true;
        // Freeze stuff
        if (freezeGame) Time.timeScale = 0;
        for (int i = 0; i < freezeObjects.Length; i++) freezeObjects[i].constraints = RigidbodyConstraints.FreezeAll;

        textBoxInstance = Instantiate(textBox, canvas.gameObject.transform);
        textComp = textBoxInstance.gameObject.GetComponentInChildren<Text>();

        SetTextBlock();
    }

    IEnumerator EndText()
    {
        yield return new WaitForEndOfFrame();
        isPlaying = false;

        // Reset cam
        camSelector.SetCamActive(-1);

        textIndex = 0;
        Destroy(textBoxInstance.gameObject);

        // Unfreeze Stuff
        for (int i = 0; i < freezeObjects.Length; i++) freezeObjects[i].constraints = constraints[i];
        if (freezeGame) Time.timeScale = 1;

        if (destroyOnUse) Destroy(gameObject);
    }

    private void SetTextBlock()
    {
        if (textBlocks.Length > textIndex)
        {
            textComp.text = textBlocks[textIndex].text;
            if (camSelector != null)
            {
                CinemachineVirtualCamera vcam = textBlocks[textIndex].OptionalVcam;
                if (vcam != null)
                {
                    int newCamIndex = camSelector.vcams.Length;
                    CinemachineVirtualCamera[] newVcams = new CinemachineVirtualCamera[newCamIndex + 1];
                    camSelector.vcams.CopyTo(newVcams, 0);
                    newVcams[newCamIndex] = vcam;
                    camSelector.vcams = newVcams;
                    camSelector.SetCamActive(newCamIndex);
                }
                else camSelector.SetCamActive(-1);
            }
        }
    }

    public bool GetIsPlaying()
    {
        return isPlaying;
    }

}
