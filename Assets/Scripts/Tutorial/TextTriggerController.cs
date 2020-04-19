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

    [Tooltip("The object that the trigger will detect.")]
    public GameObject target;
    [Tooltip("Time in seconds after which the textbox will display.")]
    public float delay = 0f;
    [Tooltip("Whether this trigger is one-time use or multi-time use.")]
    public bool destroyOnUse = false;
    [Tooltip("Whether to freeze game while textbox plays.")]
    public bool freezeGame = false;

    [Header("UI Stuff")]
    [Tooltip("The Canvas where the Text Box will be displayed.")]
    public Canvas canvas;
    [Tooltip("Image Object must have a child object with a Text UI Component.")]
    public Image textBox;
    [Tooltip("The text blocks that will be displayed in order.")]
    public TextBlock[] textBlocks;

    [Header("Optional")]
    [Tooltip("If not freezing the game, can alternatively freeze rigidbody constraints of objects.")]
    public Rigidbody[] freezeObjects = new Rigidbody[0];

    private RigidbodyConstraints[] constraints;
    private Text textComp;
    private int textIndex;
    private bool isPlaying = false;
    Image textBoxInstance;

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
    }

    private void Update()
    {
        if (isPlaying && m_playerInput.GetButtonDown("Jump"))
        {
            textIndex++;
            if (textIndex >= textBlocks.Length) { Coroutine WaitForFrame = StartCoroutine(EndText()); }
            else textComp.text = textBlocks[textIndex].text;

        }
    }

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

        isPlaying = true;
        // Freeze stuff
        if (freezeGame) Time.timeScale = 0;
        for (int i = 0; i < freezeObjects.Length; i++) freezeObjects[i].constraints = RigidbodyConstraints.FreezeAll;

        textBoxInstance = Instantiate(textBox, canvas.gameObject.transform);
        textComp = textBoxInstance.gameObject.GetComponentInChildren<Text>();

        if(textBlocks.Length > textIndex) textComp.text = textBlocks[textIndex].text;
    }

    IEnumerator EndText()
    {
        yield return new WaitForEndOfFrame();
        isPlaying = false;
        // Unfreeze Stuff
        if (freezeGame) Time.timeScale = 1;
        for (int i = 0; i < freezeObjects.Length; i++) freezeObjects[i].constraints = constraints[i];

        textIndex = 0;
        Destroy(textBoxInstance.gameObject);
        if (destroyOnUse) Destroy(gameObject);
    }

    public bool GetIsPlaying()
    {
        return isPlaying;
    }
}
